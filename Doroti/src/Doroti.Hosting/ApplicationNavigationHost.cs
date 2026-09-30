using System.Text.Json;
using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Bounded, owner-thread activation queue and versioned restoration checkpoint.</summary>
public sealed class ApplicationNavigationHost : IApplicationNavigationHostCapability, IDisposable
{
    public const int MaximumCheckpointBytes = 4 * 1024 * 1024;
    private readonly Queue<ApplicationActivation> _pending = new();
    private readonly Queue<string> _recent = new();
    private readonly HashSet<string> _seen = new(StringComparer.Ordinal);
    private readonly Action<string>? _save;
    private readonly Action<string, string?, bool>? _report;
    private Action<ApplicationActivation>? _subscriber;
    private byte[]? _restoration;
    private bool _disposed;
    private bool _delivering;
    private bool _cleanShutdown;
    public ApplicationActivation Current { get; private set; }
    public bool RestorationEnabled => _save is not null;
    public bool PreviousShutdownWasClean { get; }
    public string? RestoreFailure { get; private set; }
    public void ReportPersistenceFailure(string message) { if (!_disposed) RestoreFailure = message; }

    public ApplicationNavigationHost(string? initialLocation = null, string? checkpoint = null,
        Action<string>? save = null, Action<string, string?, bool>? report = null, string? initialStateJson = null,
        ApplicationActivationSource initialSource = ApplicationActivationSource.Launch)
    {
        _save = save;
        _report = report;
        string location = "/";
        string? state = null;
        if (checkpoint is not null)
        {
            try
            {
                if (checkpoint.Length > MaximumCheckpointBytes * 2) throw new FormatException("Checkpoint is too large.");
                using var document = JsonDocument.Parse(checkpoint);
                var root = document.RootElement;
                if (root.GetProperty("version").GetInt32() != 1) throw new FormatException("Unsupported checkpoint version.");
                location = root.GetProperty("location").GetString()!;
                ValidateLocation(location);
                state = root.TryGetProperty("state", out var savedState) ? savedState.GetString() : null;
                if (state is not null) { using var json = JsonDocument.Parse(state); }
                PreviousShutdownWasClean = root.TryGetProperty("cleanShutdown", out var clean) && clean.GetBoolean();
                if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.String)
                {
                    _restoration = Convert.FromBase64String(data.GetString()!);
                    if (_restoration.Length > MaximumCheckpointBytes) throw new FormatException("Restoration data is too large.");
                }
                else if (data.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined))
                    throw new FormatException("Restoration data must be base64 text or null.");
            }
            catch (Exception error) when (error is JsonException or FormatException or InvalidOperationException or KeyNotFoundException or ArgumentException)
            {
                RestoreFailure = error.Message;
                location = "/";
                state = null;
                _restoration = null;
                PreviousShutdownWasClean = false;
            }
        }
        if (initialLocation is not null)
        {
            ValidateLocation(initialLocation);
            // An explicit cold link wins over a saved navigation stack. Do not
            // let Router restoration redirect it to a previous destination.
            if (initialLocation != location) _restoration = null;
            location = initialLocation;
            state = initialStateJson;
            if (state is not null) { using var json = JsonDocument.Parse(state); }
        }
        Current = new(Guid.NewGuid().ToString("N"), location, initialSource, true, state);
        Checkpoint(); // mark running before the first frame; a crash is distinguishable from graceful close
    }

    public static void ValidateLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location) || location.Length > 8192 || location.Any(char.IsControl) ||
            location.Contains('\\') || location.StartsWith("//", StringComparison.Ordinal) ||
            !Uri.TryCreate(location, UriKind.RelativeOrAbsolute, out var uri) ||
            (!location.StartsWith('/') && !uri.IsAbsoluteUri))
            throw new ArgumentException("A route must be an absolute URI or a root-relative path.", nameof(location));
        if (!location.StartsWith('/') && uri.IsAbsoluteUri && (uri.Scheme is "javascript" or "data" or "file"))
            throw new ArgumentException("This URI scheme is not an application route.", nameof(location));
    }

    public bool Activate(ApplicationActivation activation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(activation.Id);
        ValidateLocation(activation.Location);
        if (activation.StateJson is not null) { using var state = JsonDocument.Parse(activation.StateJson); }
        if (_seen.Contains(activation.Id)) return false;
        if (_pending.Count == 32) throw new InvalidOperationException("Router activation queue is full.");
        _seen.Add(activation.Id);
        _recent.Enqueue(activation.Id);
        if (_recent.Count > 128) _seen.Remove(_recent.Dequeue());
        _pending.Enqueue(activation);
        Drain();
        return true;
    }

    public IDisposable Subscribe(Action<ApplicationActivation> handler)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(handler);
        if (_subscriber is not null) throw new InvalidOperationException("One Router owns each view's navigation.");
        _subscriber = handler;
        try { Drain(); }
        catch { _subscriber = null; throw; }
        return new Subscription(() => { if (_subscriber == handler) _subscriber = null; });
    }

    private void Drain()
    {
        if (_delivering) return;
        _delivering = true;
        try
        {
            while (!_disposed && _subscriber is { } handler && _pending.TryDequeue(out var activation))
            {
                Current = activation;
                handler(activation);
                Checkpoint();
            }
        }
        finally { _delivering = false; }
    }

    public void ReportRoute(string location, string? stateJson, bool replace)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ValidateLocation(location);
        if (stateJson is not null) { using var state = JsonDocument.Parse(stateJson); }
        _report?.Invoke(location, stateJson, replace);
        Current = Current with { Location = location, StateJson = stateJson };
        Checkpoint();
    }

    public ReadOnlyMemory<byte>? ReadRestoration() => _restoration is null ? null : new(_restoration.ToArray());
    public void WriteRestoration(ReadOnlyMemory<byte> data)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (data.Length > MaximumCheckpointBytes) throw new ArgumentOutOfRangeException(nameof(data));
        _restoration = data.ToArray();
        Checkpoint();
    }
    public void DiscardRestoration() { ObjectDisposedException.ThrowIf(_disposed, this); _restoration = null; Checkpoint(); }
    public void Checkpoint()
    {
        if (_save is null) return;
        try
        {
            using var buffer = new MemoryStream();
            using (var writer = new Utf8JsonWriter(buffer))
            {
                writer.WriteStartObject();
                writer.WriteNumber("version", 1);
                writer.WriteString("location", Current.Location);
                writer.WriteString("state", Current.StateJson);
                if (_restoration is null) writer.WriteNull("data");
                else writer.WriteBase64String("data", _restoration);
                writer.WriteBoolean("cleanShutdown", _cleanShutdown);
                writer.WriteEndObject();
            }
            _save(System.Text.Encoding.UTF8.GetString(buffer.GetBuffer(), 0, checked((int)buffer.Length)));
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        { RestoreFailure = error.Message; }
    }
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _subscriber = null;
        _pending.Clear();
        _cleanShutdown = true;
        Checkpoint();
    }
    private sealed class Subscription(Action dispose) : IDisposable
    {
        private Action? _dispose = dispose;
        public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }
}
