using Doroti.Ui;

namespace Doroti.Framework.Services;

/// <summary>Owns one native PlatformView across widget detach/rebuild. Dispose explicitly after use.</summary>
public sealed class WebViewController : IAsyncDisposable
{
    private readonly PlatformViewClient _client;
    private readonly IWebViewHostCapability _web;
    private int _closed;
    private int _attached;
    public DorotiView Owner { get; }
    public Task<PlatformViewHandle> Ready => _client.Ready;
    public event Action<WebViewEvent>? Changed;
    public event Action? Focused;

    public WebViewController(DorotiView owner, WebViewOptions? options = null)
    {
        Owner = owner;
        var host = owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews,
            DartUiInvocation.Managed("WebViewController.create"));
        var support = host.QuerySupport(new PlatformViewRequest(0, "doroti/webview", PlatformViewComposition.InterleavedComposition));
        if (!support.Supported || !support.WebViewCommands)
            throw new WebViewException(WebViewError.Unsupported, support.Reason ?? "This backend has no WebView controller adapter.");
        _web = host as IWebViewHostCapability ?? throw new WebViewException(WebViewError.Unsupported, "Host has no WebView command adapter.");
        _client = new PlatformViewClient(host, new PlatformViewDescriptor("doroti/webview", (options ?? new()).Encode(),
            PlatformViewStrategyPolicy.RequireRequested));
        _web.WebViewChanged += OnChanged;
        _client.Focused += OnFocused;
    }
    private void OnFocused() => Focused?.Invoke();
    private void OnChanged(WebViewEvent value)
    {
        if (_closed != 0 || !Ready.IsCompletedSuccessfully || value.Handle != Ready.Result) return;
        Owner.DispatchPlatformEvent(() => { if (_closed == 0) Changed?.Invoke(value); });
    }
    public Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken cancellationToken = default)
    {
        if (_closed != 0) throw new WebViewException(WebViewError.Closed, "WebView is closed.");
        if (!Ready.IsCompletedSuccessfully) throw new WebViewException(WebViewError.NotReady, "Await Ready before issuing commands.");
        return _web.ExecuteWebViewAsync(Ready.Result, command, cancellationToken);
    }
    public ValueTask SetFocusAsync(bool focused) => _client.SetFocusAsync(focused);
    public void AttachWidget()
    {
        if (_closed != 0) throw new WebViewException(WebViewError.Closed, "WebView is closed.");
        if (Interlocked.CompareExchange(ref _attached, 1, 0) != 0)
            throw new InvalidOperationException("A WebViewController can be attached to one widget at a time.");
    }
    public void DetachWidget() => Interlocked.Exchange(ref _attached, 0);
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _closed, 1) == 0)
        {
            _web.WebViewChanged -= OnChanged;
            _client.Focused -= OnFocused;
            Changed = null; Focused = null;
        }
        return _client.DisposeAsync();
    }
}
