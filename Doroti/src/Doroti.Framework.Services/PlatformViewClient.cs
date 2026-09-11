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
    public event Action? Focused;
    public PlatformViewClient(DorotiView owner, PlatformViewRequest request)
        : this(owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews,
            DartUiInvocation.Managed("PlatformViewClient.create")), request) { }
    public PlatformViewClient(IPlatformViewHostCapability host, PlatformViewRequest request)
    {
        _host = host;
        _host.ViewFocused += OnFocused;
        _creation = CreateAsync(request);
    }
    public Task<PlatformViewHandle> Ready => _creation;
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
            catch { return; } // Creation failed/cancelled; coordinator owns partial resource recovery.
            await _host.DisposeAsync(handle);
        }
        finally { _lifetime.Dispose(); }
    }
}
