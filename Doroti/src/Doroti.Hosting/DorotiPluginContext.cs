using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Optional extension to the existing handler ABI for owner-specific requests.</summary>
public interface IDorotiViewPluginHandler : IDorotiNativePluginHandler
{
    ValueTask<ReadOnlyMemory<byte>?> HandleAsync(DorotiPluginContext context, string channel,
        string codec, ReadOnlyMemory<byte>? message, CancellationToken cancellationToken = default);
}

/// <summary>One view's plugin resources. Tokens are never shared between owners.</summary>
public sealed class DorotiPluginContext(DorotiViewCapabilities capabilities) : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<string, (IDisposable Value, CancellationTokenRegistration Cancellation)> _resources = new(StringComparer.Ordinal);
    private bool _disposed;
    public DorotiViewCapabilities Capabilities { get; } = capabilities;

    public string Retain(IDisposable resource, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (_disposed) { resource.Dispose(); throw new ObjectDisposedException(nameof(DorotiPluginContext)); }
            var id = Guid.NewGuid().ToString("N");
            _resources.Add(id, (resource, default));
            var registration = cancellationToken.Register(() => Release(id));
            if (_resources.ContainsKey(id)) _resources[id] = (resource, registration);
            else registration.Dispose();
            return id;
        }
    }

    public T Require<T>(string id) where T : class, IDisposable
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _resources.TryGetValue(id, out var resource) && resource.Value is T value
                ? value : throw new InvalidOperationException("Plugin resource is unknown or released.");
        }
    }

    public void Release(string id)
    {
        (IDisposable Value, CancellationTokenRegistration Cancellation) resource;
        lock (_gate) { if (!_resources.Remove(id, out resource)) return; }
        resource.Cancellation.Dispose();
        resource.Value.Dispose();
    }

    public void Dispose()
    {
        (IDisposable Value, CancellationTokenRegistration Cancellation)[] resources;
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            resources = _resources.Values.ToArray();
            _resources.Clear();
        }
        List<Exception> errors = [];
        foreach (var resource in resources)
            try { resource.Cancellation.Dispose(); resource.Value.Dispose(); } catch (Exception error) { errors.Add(error); }
        if (errors.Count != 0) throw new AggregateException(errors);
    }
}
