using Doroti.Ui;

namespace Doroti.Framework.Services;

/// <summary>Captures an explicit owner and removes its callback on disposal.</summary>
public sealed class PlatformViewClient : IAsyncDisposable
{
    private readonly IPlatformViewHostCapability _host;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly Task<PlatformViewHandle> _creation;
    private Task? _disposal;
    private readonly object _gate = new();
    private PlatformViewHandle? _handle;
    private readonly long _instanceId;
    public event Action? Focused;
    public PlatformViewClient(DorotiView owner, PlatformViewRequest request)
        : this(owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews,
            DartUiInvocation.Managed("PlatformViewClient.create")), request) { }
    public PlatformViewClient(IPlatformViewHostCapability host, PlatformViewRequest request)
    {
        _host = host;
        _instanceId = request.InstanceId;
        _host.ViewFocused += OnFocused;
        _creation = CreateAsync(request);
    }
    public PlatformViewClient(DorotiView owner, PlatformViewDescriptor descriptor)
        : this(owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews,
            DartUiInvocation.Managed("PlatformViewClient.create")), descriptor) { }
    public PlatformViewClient(IPlatformViewHostCapability host, PlatformViewDescriptor descriptor)
    {
        _host = host;
        _instanceId = host.AllocateInstanceId();
        if (descriptor.Input != PlatformViewInputPolicy.DirectNative)
            throw new NotSupportedException("Gesture arena dispatch requires an explicitly qualified host.");
        var request = new PlatformViewRequest(_instanceId, descriptor.ViewType, descriptor.Composition,
            CreationParameters: descriptor.CreationParameters);
        if (descriptor.Strategy == PlatformViewStrategyPolicy.PlatformPreferred && !host.QuerySupport(request).Supported)
            request = request with { Composition = PlatformViewComposition.NativeOverlay };
        _host.ViewFocused += OnFocused;
        _creation = CreateAsync(request);
    }
    public Task<PlatformViewHandle> Ready => _creation;
    public Task DisposalCompletion { get { lock (_gate) return _disposal ?? Task.CompletedTask; } }
    public ValueTask SetFocusAsync(bool focused)
    {
        lock (_gate)
            return _disposal is null && _handle is { } handle
                ? _host.SetFocusAsync(handle, focused, _lifetime.Token) : ValueTask.CompletedTask;
    }
    private async Task<PlatformViewHandle> CreateAsync(PlatformViewRequest request)
    {
        var handle = await _host.CreateAsync(request, _lifetime.Token);
        lock (_gate) _handle = handle;
        return handle;
    }
    private void OnFocused(PlatformViewHandle handle)
    {
        lock (_gate) if (_disposal is null && _handle == handle) Focused?.Invoke();
    }
    public ValueTask DisposeAsync()
    {
        lock (_gate)
        {
            if (_disposal is not null) return new(_disposal);
            _host.ViewFocused -= OnFocused;
            Focused = null;
            _disposal = DisposeCoreAsync();
            return new(_disposal);
        }
    }
    private async Task DisposeCoreAsync()
    {
        _lifetime.Cancel();
        try
        {
            PlatformViewHandle handle;
            try { handle = await _creation; }
            catch
            {
                // Ready may report cancellation before a slow SDK factory returns its native object.
                // Disposal completion includes that late object's actual recovery.
                await _host.GetDisposalCompletion(_instanceId).ConfigureAwait(false);
                return;
            }
            await _host.DisposeAsync(handle);
        }
        finally { _lifetime.Dispose(); }
    }
}
