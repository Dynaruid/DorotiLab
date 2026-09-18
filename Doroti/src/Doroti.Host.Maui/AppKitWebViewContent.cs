#if MACOS
using Doroti.Ui;
using Foundation;
using WebKit;

namespace Doroti.Host.Maui;

/// <summary>Exact manifest-key mapping; never reads arbitrary filesystem paths.</summary>
internal sealed class AppKitWebViewContent(IApplicationResourceHostCapability resources,
    IReadOnlyDictionary<string, WebViewResource> routes) : NSObject, IWKUrlSchemeHandler
{
    private readonly Dictionary<nint, CancellationTokenSource> _tasks = [];
    private bool _closed;
    public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask task)
    {
        if (_closed) return;
        if (_tasks.Count >= 16)
        {
            using var error = new NSError(new NSString("Doroti.WebView.Content.Busy"), 1);
            task.DidFailWithError(error);
            return;
        }
        var cancellation = new CancellationTokenSource();
        _tasks.Add(task.Handle, cancellation);
        _ = Respond(task, cancellation);
    }
    public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask task)
    {
        if (_tasks.Remove(task.Handle, out var cancellation)) cancellation.Cancel();
    }
    private async Task Respond(IWKUrlSchemeTask task, CancellationTokenSource cancellation)
    {
        var key = task.Handle;
        try
        {
            var request = task.Request;
            if (request.HttpMethod != "GET" || !Uri.TryCreate(request.Url?.AbsoluteString, UriKind.Absolute, out var uri) ||
                uri.Scheme != "doroti-app" || uri.Host != "content" || !string.IsNullOrEmpty(uri.UserInfo) ||
                uri.AbsolutePath.Contains('%') || !routes.TryGetValue(uri.AbsolutePath, out var route) ||
                request.Headers?["Range"] is not null)
                throw new WebViewException(WebViewError.InvalidRequest, "Unknown app resource or unsupported method/Range request.");
            var manifest = resources.Resources.SingleOrDefault(resource => resource.Key == route.ResourceKey)
                ?? throw new WebViewException(WebViewError.InvalidRequest, "Resource is not registered in the application manifest.");
            if (manifest.Length > 8 * 1024 * 1024) throw new WebViewException(WebViewError.InvalidRequest, "App resource exceeds 8 MiB.");
            var bytes = await resources.LoadAsync(route.ResourceKey, cancellation.Token).ConfigureAwait(false);
            await new AppKitPlatformViewDispatcher().InvokeAsync(() =>
            {
                if (_closed || cancellation.IsCancellationRequested || !_tasks.Remove(key)) return ValueTask.CompletedTask;
                using var response = new NSUrlResponse(request.Url!, route.MimeType, bytes.Length, "utf-8");
                using var data = NSData.FromArray(bytes.ToArray());
                task.DidReceiveResponse(response); task.DidReceiveData(data); task.DidFinish();
                return ValueTask.CompletedTask;
            });
        }
        catch (Exception error)
        {
            await new AppKitPlatformViewDispatcher().InvokeAsync(() =>
            {
                if (!_closed && !cancellation.IsCancellationRequested && _tasks.Remove(key))
                {
                    using var info = NSDictionary.FromObjectAndKey(new NSString(error.Message), NSError.LocalizedDescriptionKey);
                    using var nativeError = new NSError(new NSString("Doroti.WebView.Content"), 1, info);
                    task.DidFailWithError(nativeError);
                }
                return ValueTask.CompletedTask;
            });
        }
        finally { cancellation.Dispose(); }
    }
    internal void Close()
    {
        _closed = true;
        foreach (var cancellation in _tasks.Values) cancellation.Cancel();
        _tasks.Clear();
    }
    protected override void Dispose(bool disposing) { if (disposing) Close(); base.Dispose(disposing); }
}
#endif
