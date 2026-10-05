using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>A registration preserves generic contract identity without reflection,
/// byte envelopes, codecs or dynamic assembly activation.</summary>
public abstract class DorotiManagedPluginRegistration
{
    private protected DorotiManagedPluginRegistration(string pluginId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginId);
        PluginId = pluginId;
    }
    public string PluginId { get; }
    internal abstract void Register(DorotiManagedPluginRegistry registry);
    public static DorotiManagedPluginRegistration Create<TRequest, TResult>(string pluginId,
        Func<IDorotiManagedPluginHandler<TRequest, TResult>> factory) => new Registration<TRequest, TResult>(pluginId, factory);

    private sealed class Registration<TRequest, TResult>(string pluginId,
        Func<IDorotiManagedPluginHandler<TRequest, TResult>> factory) : DorotiManagedPluginRegistration(pluginId)
    {
        private readonly Func<IDorotiManagedPluginHandler<TRequest, TResult>> _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        internal override void Register(DorotiManagedPluginRegistry registry) => registry.Register(PluginId, _factory());
    }
}

public sealed class DorotiManagedPluginRegistry(DorotiCapabilityLifetime lifetime) : IDorotiManagedPluginInvoker, IDisposable
{
    private sealed record Binding<TRequest, TResult>(IDorotiManagedPluginHandler<TRequest, TResult> Handler);
    private readonly DorotiCapabilityLifetime _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
    private readonly Dictionary<string, object> _bindings = new(StringComparer.Ordinal);
    private bool _sealed;
    private bool _disposed;

    internal void Register<TRequest, TResult>(string pluginId, IDorotiManagedPluginHandler<TRequest, TResult> handler)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_sealed) throw new InvalidOperationException("The managed plugin registry is frozen.");
        ArgumentNullException.ThrowIfNull(handler);
        if (_bindings.ContainsKey(pluginId)) throw new InvalidOperationException($"Managed plugin is duplicated: {pluginId}.");
        _lifetime.Own(handler);
        _bindings.Add(pluginId, new Binding<TRequest, TResult>(handler));
    }

    internal void Seal() => _sealed = true;
    public void Configure(IEnumerable<DorotiManagedPluginRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(registrations);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_sealed) throw new InvalidOperationException("The managed plugin registry is already configured.");
        foreach (var registration in registrations) registration.Register(this);
        Seal();
    }

    public async ValueTask<TResult> InvokeAsync<TRequest, TResult>(DorotiView view, string pluginId, TRequest request,
        DorotiUiInvocation invocation, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(view);
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginId);
        if (!_sealed) throw new InvalidOperationException("Managed plugins cannot run before registry sealing.");
        if (!_bindings.TryGetValue(pluginId, out var value) || value is not Binding<TRequest, TResult> binding)
            throw new DorotiCapabilityException(DorotiCapabilityIds.ManagedPlugins, view.viewId, invocation,
                $"plugin '{pluginId}' is missing or its typed request/result identity differs", view.targetIdentity);
        using var handlerLease = _lifetime.AcquireOwned(binding.Handler);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, view.InvocationLifetime);
        Task<TResult>? pending = null;
        await view.DispatchPlatformEventAsync(() =>
        {
            // AsTask consumes this ValueTask exactly once; the handler starts on
            // its application owner with the selected view's environment.
            pending = binding.Handler.HandleAsync(new(view.SceneOwner, invocation), request, linked.Token).AsTask();
        }, linked.Token);
        var result = await pending!;
        linked.Token.ThrowIfCancellationRequested();
        return result;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _bindings.Clear();
        // Handlers belong to the application lifetime, never this borrowing registry.
    }
}
