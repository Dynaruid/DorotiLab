using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private readonly PixelTextureRegistry _textures;
    private readonly SynchronizationContext? _textureOwnerContext;
    private int _textureInvalidationQueued;
    private long _textureRevision;
    public long TextureRevision => Interlocked.Read(ref _textureRevision);
    public TextureRegistry Textures => _textures;
    public Func<TextureFeatureSupport>? TextureFeatureProvider { set => _textures.FeatureProvider = value; }

    private void RequestTextureFrame()
    {
        Interlocked.Increment(ref _textureRevision);
        if (Volatile.Read(ref _disposed))
            return;
        if (
            _textureOwnerContext is { } context
            && !ReferenceEquals(context, SynchronizationContext.Current)
        )
        {
            if (Interlocked.Exchange(ref _textureInvalidationQueued, 1) != 0)
                return;
            try
            {
                context.Post(
                    _ =>
                    {
                        Interlocked.Exchange(ref _textureInvalidationQueued, 0);
                        if (!Volatile.Read(ref _disposed))
                            _host.RequestInvalidate();
                    },
                    null
                );
            }
            catch
            {
                Interlocked.Exchange(ref _textureInvalidationQueued, 0);
                throw;
            }
            return;
        }
        _host.RequestInvalidate();
    }

    private static bool ContainsTexture(IReadOnlyList<SceneCommand> commands, int start, int end)
    {
        for (var i = start; i < end; i++)
        {
            if (
                commands[i].Operation == "texture"
                || (
                    commands[i].HostPayload is SceneRetainedPayload retained
                    && ContainsTexture(retained.Commands, 0, retained.Commands.Count)
                )
            )
                return true;
        }
        return false;
    }

    private void DrawTexture(SKCanvas canvas, SceneTexturePayload texture)
    {
        if (texture.FrozenFrame is SkiaExternalTextureFrame frozen)
        {
            frozen.Draw(canvas, ToRect(texture.Bounds), ToSamplingOptions(texture.FilterQuality));
            return;
        }
        if (_textures.DrawExternal(canvas, texture))
            return;
        var frame = _textures.Acquire(texture.TextureId, texture.Freeze);
        if (frame is null)
            return;
        try
        {
            canvas.DrawImage(
                frame.Image,
                ToRect(texture.Bounds),
                ToSamplingOptions(texture.FilterQuality)
            );
        }
        finally
        {
            frame.Release();
        }
    }

    private sealed class PixelTextureRegistry(Action invalidate) : TextureRegistry, IDisposable
    {
        private readonly Action _invalidate = invalidate;

        // Process-unique IDs prevent a stale/cross-view ID from resolving to another producer.
        private static long _nextId;
        private readonly object _gate = new();
        private readonly Dictionary<long, Entry> _entries = [];
        private readonly Dictionary<long, ExternalEntry> _external = [];
        internal Func<int, int, CancellationToken, ValueTask<SurfaceTextureEntry>>? SurfaceFactory;
        private bool _disposed;
        internal Func<NativeTextureEntry>? NativeFactory;
        internal Func<TextureFeatureSupport>? FeatureProvider;
        public override TextureFeatureSupport Features => _disposed
            ? new(false, Reason: "Texture owner is retired.")
            : FeatureProvider?.Invoke() ?? new(AndroidProducerSurface: SurfaceFactory is not null,
                Reason: NativeFactory is null ? "No native GPU importer attached." : "Native importer device has not been exposed.");

        public override NativeTextureEntry CreateNativeTexture()
        {
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                return NativeFactory?.Invoke() ?? base.CreateNativeTexture();
            }
        }

        public override ValueTask<SurfaceTextureEntry> CreateSurfaceTextureAsync(
            int width,
            int height,
            CancellationToken cancellationToken = default
        )
        {
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                return SurfaceFactory is { } factory
                    ? factory(width, height, cancellationToken)
                    : base.CreateSurfaceTextureAsync(width, height, cancellationToken);
            }
        }

        internal SkiaExternalTextureRegistration RegisterExternal(ISkiaExternalTextureSource source)
        {
            ArgumentNullException.ThrowIfNull(source);
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                var entry = new ExternalEntry(this, Interlocked.Increment(ref _nextId), source);
                _external.Add(entry.Id, entry);
                return entry;
            }
        }

        internal IDisposable CaptureFrame(long id, bool freeze)
        {
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                if (_external.TryGetValue(id, out var external)) return external.CaptureFrame(freeze);
                if (!_entries.ContainsKey(id))
                    throw new InvalidOperationException("Texture submission belongs to a retired or different view registry.");
                return new PixelFrame(Acquire(id, freeze));
            }
        }

        private sealed class PixelFrame(SkiaImageHandle? image) : SkiaExternalTextureFrame
        {
            private SkiaImageHandle? _image = image;
            public override void Draw(SKCanvas canvas, SKRect destination, SKSamplingOptions sampling)
            {
                if (_image is { } frame) canvas.DrawImage(frame.Image, destination, sampling);
            }
            public override void Dispose() => Interlocked.Exchange(ref _image, null)?.Release();
        }

        internal bool DrawExternal(SKCanvas canvas, SceneTexturePayload texture)
        {
            ExternalEntry? entry;
            lock (_gate)
                _external.TryGetValue(texture.TextureId, out entry);
            if (entry is null)
                return false;
            entry.Draw(canvas, texture);
            return true;
        }

        public override TextureEntry CreateTexture()
        {
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                var entry = new Entry(this, Interlocked.Increment(ref _nextId));
                _entries.Add(entry.Id, entry);
                return entry;
            }
        }

        internal SkiaImageHandle? Acquire(long id, bool freeze)
        {
            lock (_gate)
            {
                if (!_entries.TryGetValue(id, out var entry))
                    return null;
                if (entry.Pending is not null && (!freeze || entry.Current is null))
                {
                    entry.Current?.Release();
                    entry.Current = entry.Pending;
                    entry.Pending = null;
                }
                return (SkiaImageHandle?)entry.Current?.Clone();
            }
        }

        public void Dispose()
        {
            ExternalEntry[] external;
            lock (_gate)
            {
                if (_disposed)
                    return;
                _disposed = true;
                foreach (var entry in _entries.Values)
                    entry.ReleaseFrames();
                _entries.Clear();
                external = _external.Values.ToArray();
                _external.Clear();
                SurfaceFactory = null;
                NativeFactory = null;
            }
            foreach (var entry in external)
                entry.Dispose();
        }

        private sealed class ExternalEntry(
            PixelTextureRegistry owner,
            long id,
            ISkiaExternalTextureSource source
        ) : SkiaExternalTextureRegistration
        {
            private readonly object _drawGate = new();
            private bool _released;
            public override long Id => id;

            public override void MarkFrameAvailable()
            {
                if (!Volatile.Read(ref _released))
                    owner._invalidate();
            }

            internal SkiaExternalTextureFrame CaptureFrame(bool freeze)
            {
                lock (_drawGate)
                {
                    ObjectDisposedException.ThrowIf(_released, this);
                    return source.CaptureFrame(freeze);
                }
            }

            internal void Draw(SKCanvas canvas, SceneTexturePayload texture)
            {
                lock (_drawGate)
                    if (!_released)
                        source.Draw(
                            canvas,
                            ToRect(texture.Bounds),
                            ToSamplingOptions(texture.FilterQuality),
                            texture.Freeze
                        );
            }

            public override void Dispose()
            {
                lock (owner._gate)
                    owner._external.Remove(Id);
                lock (_drawGate)
                {
                    if (_released)
                        return;
                    _released = true;
                    source.Dispose();
                }
                owner._invalidate();
            }
        }

        private sealed class Entry(PixelTextureRegistry owner, long id) : TextureEntry
        {
            public override long Id => id;
            internal SkiaImageHandle? Pending;
            internal SkiaImageHandle? Current;
            private bool _released;

            public override void PushFrame(
                ReadOnlySpan<byte> pixels,
                int width,
                int height,
                int rowBytes = 0
            )
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
                var packedRow = checked(width * 4);
                if (rowBytes == 0)
                    rowBytes = packedRow;
                ArgumentOutOfRangeException.ThrowIfLessThan(rowBytes, packedRow);
                var requiredBytes = checked((long)(height - 1) * rowBytes + packedRow);
                if (pixels.Length < requiredBytes)
                    throw new ArgumentException(
                        "The pixel buffer is shorter than the frame dimensions and stride.",
                        nameof(pixels)
                    );
                lock (owner._gate)
                {
                    ObjectDisposedException.ThrowIf(_released || owner._disposed, this);
                    using var colorSpace = SKColorSpace.CreateSrgb();
                    var image =
                        SKImage.FromPixelCopy(
                            new SKImageInfo(
                                width,
                                height,
                                SKColorType.Rgba8888,
                                SKAlphaType.Premul,
                                colorSpace
                            ),
                            pixels,
                            rowBytes
                        )
                        ?? throw new InvalidOperationException("Texture pixel allocation failed.");
                    Pending?.Release();
                    Pending = new SkiaImageHandle(image);
                }
                // Never invoke a host callback while holding the registry lock.
                owner._invalidate();
            }

            internal void ReleaseFrames()
            {
                _released = true;
                Pending?.Release();
                Current?.Release();
                Pending = null;
                Current = null;
            }

            public override void Dispose()
            {
                lock (owner._gate)
                {
                    if (_released)
                        return;
                    owner._entries.Remove(Id);
                    ReleaseFrames();
                }
                owner._invalidate();
            }
        }
    }
}
