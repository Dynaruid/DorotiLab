using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using Doroti.Graphics.DisplayList;
using Doroti.Ui;
using UiImage = Doroti.Ui.Image;

namespace Doroti.Host.Web;

[SupportedOSPlatform("browser")]
internal sealed class CanvasKitResourceRegistry :
    IBrowserDisplayListResources,
    ICanvasKitResourceCallback,
    IDisposable
{
    private static long _nextResourceId;
    private readonly object _gate = new();
    private readonly Dictionary<DisplayResourceReference, Entry> _entries = [];
    private readonly Dictionary<string, DisplayResourceReference> _effects = new(StringComparer.Ordinal);
    private DisplayResourceReference? _defaultFont;
    private bool _disposed;

    public DisplayResourceReference DefaultFont
    {
        get
        {
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                var reference = _defaultFont ?? throw new InvalidOperationException(
                    "CanvasKit text requires a registered fallback font before scene encoding.");
                if (!_entries.TryGetValue(reference, out var entry) || entry.Completion.Task.IsFaulted)
                    throw new InvalidOperationException(
                        "CanvasKit text fallback font did not reach a live retained state.");
                return reference;
            }
        }
    }

    internal string RegisterFont(ReadOnlyMemory<byte> bytes, string family = "DorotiFallback")
    {
        if (bytes.IsEmpty) throw new ArgumentException("Font bytes cannot be empty.", nameof(bytes));
        ArgumentException.ThrowIfNullOrWhiteSpace(family);
        var ownedBytes = bytes.ToArray();
        var reference = CreateReference(DisplayResourceKind.Font);
        var entry = CreateEntry(
            reference,
            ownedBytes,
            "font",
            new
            {
                schema = "doroti.canvaskit-resource/v1",
                viewId = 0,
                kind = "font",
                id = reference.Id,
                version = reference.Version,
                family,
                sha256 = Convert.ToHexStringLower(SHA256.HashData(ownedBytes)),
            });
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _entries.Add(reference, entry);
            var previousDefault = _defaultFont;
            _defaultFont ??= reference;
            try
            {
                Register(entry);
            }
            catch
            {
                _entries.Remove(reference);
                _defaultFont = previousDefault;
                throw;
            }
        }
        return $"{family}/{reference.Id}/{reference.Version}";
    }

    internal async ValueTask<UiImage> RegisterImageAsync(
        ulong viewId,
        ReadOnlyMemory<byte> bytes,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        if (bytes.IsEmpty) throw new ArgumentException("Image bytes cannot be empty.", nameof(bytes));
        var ownedBytes = bytes.ToArray();
        var reference = CreateReference(DisplayResourceKind.Image);
        var entry = CreateEntry(
            reference,
            ownedBytes,
            "image",
            new
            {
                schema = "doroti.canvaskit-resource/v1",
                viewId,
                kind = "image",
                id = reference.Id,
                version = reference.Version,
                sha256 = Convert.ToHexStringLower(SHA256.HashData(ownedBytes)),
            });
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _entries.Add(reference, entry);
            try
            {
                Register(entry);
            }
            catch
            {
                _entries.Remove(reference);
                throw;
            }
        }
        CanvasKitResourceReceipt receipt;
        try
        {
            receipt = await entry.Completion.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken)
                .ConfigureAwait(false);
        }
        catch
        {
            Release(reference);
            throw;
        }
        if (receipt.Width <= 0 || receipt.Height <= 0)
        {
            Release(reference);
            throw new InvalidDataException(
                $"CanvasKit decoded image {reference.Id}/{reference.Version} without valid dimensions.");
        }
        var shared = new CanvasKitImageResource(this, reference);
        return new(viewId, receipt.Width, receipt.Height, shared.Release)
        {
            HostHandle = new CanvasKitImageHandle(shared),
        };
    }

    public DisplayResourceReference ResolveImage(UiImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        ObjectDisposedException.ThrowIf(image.debugDisposed, image);
        if (image.HostHandle is not CanvasKitImageHandle handle || !ReferenceEquals(handle.Registry, this))
            throw new InvalidOperationException(
                "CanvasKit DisplayList cannot reference an image owned by another renderer/resource registry.");
        return handle.Reference;
    }

    public DisplayResourceReference ResolveRuntimeEffect(FragmentShaderState effect)
    {
        ArgumentNullException.ThrowIfNull(effect);
        var bytes = Encoding.UTF8.GetBytes(effect.Source);
        var hash = Convert.ToHexStringLower(SHA256.HashData(bytes));
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_effects.TryGetValue(hash, out var existing)) return existing;
            var reference = CreateReference(DisplayResourceKind.RuntimeEffect);
            var entry = CreateEntry(
                reference,
                bytes,
                "runtime-effect",
                new
                {
                    schema = "doroti.canvaskit-resource/v1",
                    viewId = 0,
                    kind = "runtime-effect",
                    id = reference.Id,
                    version = reference.Version,
                    debugName = effect.DebugName,
                    sha256 = hash,
                });
            _entries.Add(reference, entry);
            _effects.Add(hash, reference);
            try
            {
                Register(entry);
            }
            catch
            {
                _entries.Remove(reference);
                _effects.Remove(hash);
                throw;
            }
            return reference;
        }
    }

    public DisplayResourceDescriptor Describe(DisplayResourceReference reference)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_entries.TryGetValue(reference, out var entry) || entry.Completion.Task.IsFaulted)
                throw new KeyNotFoundException(
                    $"CanvasKit resource {reference.Kind}/{reference.Id}/{reference.Version} is not live.");
            return entry.Descriptor;
        }
    }

    public void CompleteResource(
        long resourceId,
        long generation,
        string terminal,
        string reason,
        string receiptJson)
    {
        Entry? entry;
        lock (_gate)
        {
            entry = _entries.Values.FirstOrDefault(value =>
                value.Reference.Id == checked((ulong)resourceId) &&
                value.Reference.Version == checked((uint)generation));
        }
        if (entry is null) return;
        if (!string.Equals(terminal, "retained", StringComparison.Ordinal))
        {
            FailRegistration(entry, new InvalidDataException(
                $"CanvasKit resource {resourceId}/{generation} failed: {reason}"));
            return;
        }
        CanvasKitResourceReceipt receipt;
        try
        {
            receipt = string.IsNullOrWhiteSpace(receiptJson) || receiptJson == "{}"
                ? new()
                : JsonSerializer.Deserialize<CanvasKitResourceReceipt>(receiptJson, JsonOptions) ?? new();
        }
        catch (JsonException exception)
        {
            FailRegistration(entry, new InvalidDataException(
                $"CanvasKit resource {resourceId}/{generation} returned an invalid receipt.", exception));
            return;
        }
        entry.Completion.TrySetResult(receipt);
    }

    internal void RetainSceneResources(IReadOnlyList<DisplayResourceReference> references)
    {
        ArgumentNullException.ThrowIfNull(references);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            foreach (var reference in references)
            {
                if (!_entries.TryGetValue(reference, out var entry) || entry.Completion.Task.IsFaulted)
                    throw new ObjectDisposedException(
                        $"CanvasKit resource {reference.Kind}/{reference.Id}/{reference.Version}");
            }
            foreach (var reference in references)
            {
                checked { _entries[reference].RefCount++; }
            }
        }
    }

    internal void ReleaseSceneResources(IReadOnlyList<DisplayResourceReference> references)
    {
        ArgumentNullException.ThrowIfNull(references);
        foreach (var reference in references) Release(reference);
    }

    internal void Retain(DisplayResourceReference reference)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_entries.TryGetValue(reference, out var entry))
                throw new ObjectDisposedException($"CanvasKit resource {reference.Id}/{reference.Version}");
            checked { entry.RefCount++; }
        }
    }

    internal void Release(DisplayResourceReference reference)
    {
        Entry? release = null;
        lock (_gate)
        {
            if (!_entries.TryGetValue(reference, out var entry)) return;
            if (--entry.RefCount > 0) return;
            _entries.Remove(reference);
            if (reference.Kind == DisplayResourceKind.RuntimeEffect)
            {
                foreach (var key in _effects.Where(pair => pair.Value == reference).Select(pair => pair.Key).ToArray())
                    _effects.Remove(key);
            }
            release = entry;
        }
        if (release is not null)
        {
            try
            {
                BrowserCanvasKitInterop.ReleaseResource(
                    checked((long)release.Reference.Id), release.Reference.Version);
            }
            catch
            {
                // A failed JS bridge is already a terminal renderer condition. Do
                // not turn Image.Dispose or a scene receipt into a second failure.
            }
        }
    }

    public void Dispose()
    {
        Entry[] entries;
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            entries = _entries.Values.ToArray();
            _entries.Clear();
            _effects.Clear();
            _defaultFont = null;
        }
        foreach (var entry in entries)
        {
            entry.Completion.TrySetException(new ObjectDisposedException(nameof(CanvasKitResourceRegistry)));
            try
            {
                BrowserCanvasKitInterop.ReleaseResource(
                    checked((long)entry.Reference.Id), entry.Reference.Version);
            }
            catch
            {
                // The UI Worker may already be tearing down its bridge. Managed
                // ownership is terminal even when the final release cannot cross JS.
            }
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static DisplayResourceReference CreateReference(DisplayResourceKind kind) =>
        new(kind, checked((ulong)Interlocked.Increment(ref _nextResourceId)), 1);

    private static Entry CreateEntry(
        DisplayResourceReference reference,
        byte[] bytes,
        string kind,
        object descriptor)
    {
        var hash = SHA256.HashData(bytes);
        var fingerprint = new DisplayResourceFingerprint(
            BinaryPrimitives.ReadUInt64LittleEndian(hash),
            BinaryPrimitives.ReadUInt64LittleEndian(hash.AsSpan(sizeof(ulong))));
        return new(
            reference,
            new(reference, fingerprint),
            kind,
            JsonSerializer.Serialize(descriptor),
            bytes);
    }

    private void Register(Entry entry) => BrowserCanvasKitInterop.RegisterResource(
        checked((long)entry.Reference.Id),
        entry.Reference.Version,
        entry.Kind,
        entry.DescriptorJson,
        entry.Bytes,
        this);

    private void FailRegistration(Entry entry, Exception error)
    {
        var removed = false;
        lock (_gate)
        {
            if (_entries.Remove(entry.Reference))
            {
                removed = true;
                if (_defaultFont == entry.Reference)
                    _defaultFont = _entries.Keys.FirstOrDefault(value => value.Kind == DisplayResourceKind.Font) is { Id: > 0 } font
                        ? font
                        : null;
                foreach (var key in _effects
                    .Where(pair => pair.Value == entry.Reference)
                    .Select(pair => pair.Key)
                    .ToArray())
                    _effects.Remove(key);
            }
        }
        entry.Completion.TrySetException(error);
        if (!removed) return;
        try
        {
            BrowserCanvasKitInterop.ReleaseResource(
                checked((long)entry.Reference.Id), entry.Reference.Version);
        }
        catch
        {
        }
    }

    private sealed class Entry(
        DisplayResourceReference reference,
        DisplayResourceDescriptor descriptor,
        string kind,
        string descriptorJson,
        byte[] bytes)
    {
        internal DisplayResourceReference Reference { get; } = reference;
        internal DisplayResourceDescriptor Descriptor { get; } = descriptor;
        internal string Kind { get; } = kind;
        internal string DescriptorJson { get; } = descriptorJson;
        internal byte[] Bytes { get; } = bytes;
        internal TaskCompletionSource<CanvasKitResourceReceipt> Completion { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal int RefCount { get; set; } = 1;
    }

    private sealed record CanvasKitResourceReceipt(
        int Width = 0,
        int Height = 0,
        string ColorType = "");
}

[SupportedOSPlatform("browser")]
internal sealed class CanvasKitImageResource(
    CanvasKitResourceRegistry registry,
    DisplayResourceReference reference)
{
    private readonly object _gate = new();
    private int _references = 1;

    internal CanvasKitResourceRegistry Registry { get; } = registry;

    internal DisplayResourceReference Reference { get; } = reference;

    internal void Retain()
    {
        lock (_gate)
        {
            if (_references <= 0)
                throw new ObjectDisposedException(nameof(CanvasKitImageResource));
            Registry.Retain(Reference);
            checked { _references++; }
        }
    }

    internal void Release()
    {
        lock (_gate)
        {
            if (_references <= 0)
                throw new InvalidOperationException("CanvasKit image was released more than once.");
            _references--;
            Registry.Release(Reference);
        }
    }
}

[SupportedOSPlatform("browser")]
internal sealed class CanvasKitImageHandle(CanvasKitImageResource resource) : IDorotiImageHandle
{
    internal CanvasKitResourceRegistry Registry => resource.Registry;
    internal DisplayResourceReference Reference => resource.Reference;

    public IDorotiImageHandle Clone()
    {
        resource.Retain();
        return new CanvasKitImageHandle(resource);
    }

    public void Release() => resource.Release();
}
