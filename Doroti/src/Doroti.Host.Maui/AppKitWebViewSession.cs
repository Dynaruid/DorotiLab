#if MACOS
using AppKit;
using CoreGraphics;
using Doroti.Ui;
using Foundation;
using System.Text.Json;
using WebKit;

namespace Doroti.Host.Maui;

/// <summary>Commands/delegates for the WKWebView owned by AppKitPlatformViewFactory.
/// No second view, compositor, private WebKit API, or reflection serialization.</summary>
internal sealed class AppKitWebViewSession : IDisposable
{
    private sealed class NativeWebView(
        WKWebViewConfiguration configuration,
        Action beforeFocus,
        Action focused
    ) : WKWebView(CGRect.Empty, configuration)
    {
        public override bool BecomeFirstResponder()
        {
            beforeFocus();
            var accepted = base.BecomeFirstResponder();
            if (accepted)
            {
                focused();
            }

            return accepted;
        }

        public override void MouseDown(NSEvent theEvent)
        {
            beforeFocus();
            base.MouseDown(theEvent);
        }
    }

    private sealed class NavigationDelegate(AppKitWebViewSession owner) : WKNavigationDelegate
    {
        public override void DecidePolicy(
            WKWebView webView,
            WKNavigationAction action,
            Action<WKNavigationActionPolicy> decisionHandler
        )
        {
            var url = action.Request.Url?.AbsoluteString;
            decisionHandler(
                !owner._closed && action.TargetFrame is not null && owner.Allows(url)
                    ? WKNavigationActionPolicy.Allow
                    : WKNavigationActionPolicy.Cancel
            );
        }

        public override void DidStartProvisionalNavigation(
            WKWebView webView,
            WKNavigation navigation
        ) => owner.Started(navigation);

        public override void DidCommitNavigation(WKWebView webView, WKNavigation navigation) =>
            owner.Notify(navigation, WebViewEventKind.Committed);

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation) =>
            owner.Notify(navigation, WebViewEventKind.Completed);

        public override void DidFailNavigation(
            WKWebView webView,
            WKNavigation navigation,
            NSError error
        ) => owner.Notify(navigation, WebViewEventKind.Failed, error.LocalizedDescription);

        public override void DidFailProvisionalNavigation(
            WKWebView webView,
            WKNavigation navigation,
            NSError error
        ) => owner.Notify(navigation, WebViewEventKind.Failed, error.LocalizedDescription);

        public override void ContentProcessDidTerminate(WKWebView webView)
        {
            if (owner._closed)
            {
                return;
            }

            owner._failed = true;
            owner.CancelPending(
                WebViewError.ProcessFailed,
                "WebKit content process terminated; recreate the controller."
            );
            owner.Emit(WebViewEventKind.ProcessFailed, "WebKit content process terminated.");
        }
    }

    private sealed class UiDelegate : WKUIDelegate
    {
        public override WKWebView? CreateWebView(
            WKWebView webView,
            WKWebViewConfiguration configuration,
            WKNavigationAction navigationAction,
            WKWindowFeatures windowFeatures
        ) => null;
    }

    private sealed class MessageHandler(AppKitWebViewSession owner)
        : NSObject,
            IWKScriptMessageHandler
    {
        public void DidReceiveScriptMessage(
            WKUserContentController controller,
            WKScriptMessage message
        ) => owner.ReceiveMessage(message);
    }

    private readonly PlatformViewHandle _handle;
    private readonly WebViewOptions _options;
    private readonly Action<WebViewEvent> _changed;
    private readonly WKWebsiteDataStore _store;
    private readonly WKWebViewConfiguration _configuration;
    private readonly NavigationDelegate _navigation;
    private readonly UiDelegate _ui = new();
    private readonly MessageHandler? _messages;
    private readonly AppKitWebViewContent? _content;
    private readonly List<WKNavigation> _navigations = [];
    private readonly Dictionary<long, TaskCompletionSource<WebViewResult>> _pending = [];
    private long _requestId,
        _navigationId,
        _documentGeneration;
    private nint _activeNavigation;
    private bool _closed,
        _failed;
    internal WKWebView View { get; }

    internal AppKitWebViewSession(
        PlatformViewHandle handle,
        string parameters,
        Action beforeFocus,
        Action focused,
        Action<WebViewEvent> changed,
        IApplicationResourceHostCapability? resources
    )
    {
        _handle = handle;
        _changed = changed;
        if (
            parameters.StartsWith("doroti-webview:", StringComparison.Ordinal)
            && !parameters.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
        )
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Unknown WebView creation protocol version."
            );
        }

        _options = parameters.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
            ? JsonSerializer.Deserialize(
                parameters[WebViewOptions.Prefix.Length..],
                WebViewJsonContext.Default.WebViewOptions
            )
                ?? throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "Missing WebView settings."
                )
            : new WebViewOptions(parameters);
        _options.Validate();
        if (_options.Resources is { Count: > 0 } && resources is null)
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Host has no application resources."
            );
        }

        _store =
            _options.Profile == WebViewProfile.SharedPersistent
                ? WKWebsiteDataStore.DefaultDataStore
                : WKWebsiteDataStore.NonPersistentDataStore;
        _configuration = new WKWebViewConfiguration { WebsiteDataStore = _store };
        if (_options.Resources is { Count: > 0 } routes)
        {
            _content = new AppKitWebViewContent(
                resources
                    ?? throw new WebViewException(
                        WebViewError.Unsupported,
                        "Host has no application resources."
                    ),
                routes
            );
            _configuration.SetUrlSchemeHandler(_content, "doroti-app");
        }
        if (_options.MessageOrigins is { Length: > 0 })
        {
            _messages = new MessageHandler(this);
            _configuration.UserContentController.AddScriptMessageHandler(_messages, "doroti");
        }
        _navigation = new NavigationDelegate(this);
        View = new NativeWebView(_configuration, beforeFocus, focused)
        {
            NavigationDelegate = _navigation,
            UIDelegate = _ui,
        };
        BeginNavigation(
            View.LoadHtmlString(_options.Html ?? "<!doctype html><meta charset=utf-8>", null!)
        );
    }

    private bool Allows(string? url)
    {
        if (url == "about:blank")
        {
            return true;
        }

        if (
            _content is not null
            && Uri.TryCreate(url, UriKind.Absolute, out var appUri)
            && appUri.Scheme == "doroti-app"
            && appUri.Host == "content"
            && string.IsNullOrEmpty(appUri.UserInfo)
        )
        {
            return true;
        }

        if (
            !Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https")
            || !string.IsNullOrEmpty(uri.UserInfo)
        )
        {
            return false;
        }

        return _options.AllowedOrigins is not { } allowed
            || allowed.Contains(uri.GetLeftPart(UriPartial.Authority), StringComparer.Ordinal);
    }

    private void BeginNavigation(WKNavigation? navigation)
    {
        if (navigation is null)
        {
            throw new WebViewException(WebViewError.InvalidRequest, "WebKit rejected navigation.");
        }

        AdvanceDocument();
        RememberNavigation(navigation);
        _activeNavigation = navigation.Handle;
    }

    private void RememberNavigation(WKNavigation navigation)
    {
        _navigations.Add(navigation);
        // Retain recent navigation objects so stale callbacks cannot become a new
        // document, and native handle reuse cannot alias one of these generations.
        if (_navigations.Count > 64)
        {
            _navigations.RemoveAt(0);
        }
    }

    private void AdvanceDocument()
    {
        _navigationId++;
        _documentGeneration++;
        CancelPending(
            WebViewError.NavigationChanged,
            "The document changed before this command completed."
        );
    }

    private void Started(WKNavigation navigation)
    {
        if (_closed)
        {
            return;
        }

        if (_activeNavigation != navigation.Handle)
        {
            if (_navigations.Any(item => item.Handle == navigation.Handle))
            {
                return;
            }

            AdvanceDocument();
            RememberNavigation(navigation);
            _activeNavigation = navigation.Handle;
        }
        Emit(WebViewEventKind.Started);
    }

    private void Notify(WKNavigation navigation, WebViewEventKind kind, string? error = null)
    {
        if (_closed || navigation.Handle != _activeNavigation)
        {
            return;
        }

        if (kind == WebViewEventKind.Committed && _messages is not null)
        {
            InstallBridge();
        }

        Emit(kind, error);
    }

    private void InstallBridge()
    {
        var generation = _documentGeneration;
        var script =
            "(()=>{let request=0;const generation="
            + generation.ToString(System.Globalization.CultureInfo.InvariantCulture)
            + ";window.doroti={postMessage:(name,payload)=>{window.webkit.messageHandlers.doroti.postMessage(JSON.stringify({version:1,documentGeneration:generation,requestId:++request,name,payload}));}}})()";
        View.EvaluateJavaScript(
            script,
            (_, error) =>
            {
                if (!_closed && generation == _documentGeneration && error is not null)
                {
                    System.Diagnostics.Trace.TraceError(error.LocalizedDescription);
                }
            }
        );
    }

    private void ReceiveMessage(WKScriptMessage message)
    {
        if (_closed || _failed || !message.FrameInfo.MainFrame || message.Body is not NSString body)
        {
            return;
        }

        var origin = message.FrameInfo.SecurityOrigin;
        var actualOrigin = origin.Protocol + "://" + origin.Host;
        if (
            origin.Port != 0
            && !(origin.Protocol == "https" && origin.Port == 443)
            && !(origin.Protocol == "http" && origin.Port == 80)
        )
        {
            actualOrigin += ":" + origin.Port;
        }

        if (_options.MessageOrigins?.Contains(actualOrigin, StringComparer.Ordinal) != true)
        {
            return;
        }

        var text = body.ToString();
        if (System.Text.Encoding.UTF8.GetByteCount(text) > 64 * 1024)
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(text);
            var value = document.RootElement;
            if (
                value.GetProperty("version").GetInt32() != 1
                || value.GetProperty("documentGeneration").GetInt64() != _documentGeneration
            )
            {
                return;
            }

            var name = value.GetProperty("name").GetString();
            var request = value.GetProperty("requestId").GetInt64();
            if (string.IsNullOrEmpty(name) || name.Length > 128 || request <= 0)
            {
                return;
            }

            _changed(
                new(
                    _handle,
                    _navigationId,
                    _documentGeneration,
                    WebViewEventKind.Message,
                    message.FrameInfo.Request.Url?.AbsoluteString,
                    MessageName: name,
                    MessageJson: value.GetProperty("payload").GetRawText(),
                    MessageRequestId: request
                )
            );
        }
        catch (Exception exception)
        {
            System.Diagnostics.Trace.TraceInformation(
                "Rejected WebView message: " + exception.Message
            );
        }
    }

    private void Emit(WebViewEventKind kind, string? error = null)
    {
        try
        {
            _changed(
                new(
                    _handle,
                    _navigationId,
                    _documentGeneration,
                    kind,
                    View.Url?.AbsoluteString,
                    error
                )
            );
        }
        catch (Exception exception)
        {
            System.Diagnostics.Trace.TraceError(exception.ToString());
        }
    }

    private WebViewResult Snapshot(
        long request,
        string? json = null,
        bool undefined = false,
        WebViewFeatures? features = null
    ) =>
        new(
            request,
            _navigationId,
            _documentGeneration,
            json,
            undefined,
            View.Url?.AbsoluteString,
            View.Title,
            View.IsLoading,
            View.CanGoBack,
            View.CanGoForward,
            features
        );

    internal Task<WebViewResult> ExecuteAsync(
        WebViewCommand command,
        CancellationToken cancellationToken
    )
    {
        AppKitPlatformViewDispatcher.VerifyThread();
        cancellationToken.ThrowIfCancellationRequested();
        if (_closed)
        {
            throw new WebViewException(WebViewError.Closed, "WebView is closed.");
        }

        if (_failed)
        {
            throw new WebViewException(WebViewError.ProcessFailed, "Recreate the failed WebView.");
        }

        if (!Enum.IsDefined(command.Operation))
        {
            throw new WebViewException(WebViewError.Unsupported, "Unknown WebView operation.");
        }

        if (command.DocumentGeneration != 0 && command.DocumentGeneration != _documentGeneration)
        {
            throw new WebViewException(
                WebViewError.NavigationChanged,
                "Command refers to a stale document."
            );
        }

        if (System.Text.Encoding.UTF8.GetByteCount(command.Text ?? "") > 2 * 1024 * 1024)
        {
            throw new WebViewException(WebViewError.InvalidRequest, "Command exceeds 2 MiB.");
        }

        var request = ++_requestId;
        switch (command.Operation)
        {
            case WebViewOperation.Features:
                return Task.FromResult(
                    Snapshot(
                        request,
                        features: new(
                            true,
                            true,
                            true,
                            true,
                            true,
                            ScriptMessages: _messages is not null,
                            AppContentScheme: _content is not null
                        )
                    )
                );
            case WebViewOperation.State:
                break;
            case WebViewOperation.Navigate:
                if (!Allows(command.Text) || command.Text == "about:blank")
                {
                    throw new WebViewException(
                        WebViewError.InvalidRequest,
                        "URL is outside the HTTP(S)/app-content navigation policy."
                    );
                }

                using (var url = new NSUrl(command.Text!))
                using (var nativeRequest = new NSUrlRequest(url))
                {
                    BeginNavigation(View.LoadRequest(nativeRequest));
                }

                break;
            case WebViewOperation.LoadHtml:
                BeginNavigation(View.LoadHtmlString(command.Text ?? "", null!));
                break;
            case WebViewOperation.Reload:
                BeginNavigation(View.Reload());
                break;
            case WebViewOperation.Back:
                if (!View.CanGoBack)
                {
                    throw new WebViewException(
                        WebViewError.InvalidRequest,
                        "No back history entry."
                    );
                }

                BeginNavigation(View.GoBack());
                break;
            case WebViewOperation.Forward:
                if (!View.CanGoForward)
                {
                    throw new WebViewException(
                        WebViewError.InvalidRequest,
                        "No forward history entry."
                    );
                }

                BeginNavigation(View.GoForward());
                break;
            case WebViewOperation.Stop:
                View.StopLoading();
                break;
            case WebViewOperation.EvaluateJavaScript:
                if (View.IsLoading)
                {
                    throw new WebViewException(
                        WebViewError.NotReady,
                        "Wait for document load before evaluating JavaScript."
                    );
                }

                return Evaluate(request, command.Text ?? "undefined", cancellationToken);
            case WebViewOperation.ClearData:
                return Clear(request, cancellationToken);
        }
        return Task.FromResult(Snapshot(request));
    }

    private TaskCompletionSource<WebViewResult> Admit(long request)
    {
        if (_pending.Count >= 32)
        {
            throw new WebViewException(
                WebViewError.Busy,
                "At most 32 asynchronous WebView commands may be pending."
            );
        }

        var completion = new TaskCompletionSource<WebViewResult>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        _pending.Add(request, completion);
        return completion;
    }

    private Task<WebViewResult> Evaluate(
        long request,
        string script,
        CancellationToken cancellationToken
    )
    {
        var completion = Admit(request);
        var generation = _documentGeneration;
        // JSON envelope preserves undefined versus null, and rejects Promise/BigInt/cycles.
        // The source string is JSON-escaped; it is never interpolated as executable wrapper code.
        var source = JsonEncodedText.Encode(script).ToString();
        var wrapper =
            "(()=>{try{const v=(0,eval)(\""
            + source
            + "\");if(v&&typeof v.then==='function')throw Error('Promise results are unsupported');"
            + "if(v===undefined)return JSON.stringify({undefined:true});const j=JSON.stringify(v);if(j===undefined)throw Error('Unserializable result');"
            + "return JSON.stringify({json:j})}catch(e){return JSON.stringify({error:String(e)})}})()";
        View.EvaluateJavaScript(
            wrapper,
            (value, error) =>
            {
                if (_closed || generation != _documentGeneration || !_pending.Remove(request))
                {
                    return;
                }

                try
                {
                    if (error is not null)
                    {
                        throw new WebViewException(
                            WebViewError.JavaScript,
                            error.LocalizedDescription
                        );
                    }

                    var text =
                        value?.ToString()
                        ?? throw new WebViewException(
                            WebViewError.JavaScript,
                            "Missing JavaScript envelope."
                        );
                    if (System.Text.Encoding.UTF8.GetByteCount(text) > 2 * 1024 * 1024)
                    {
                        throw new WebViewException(
                            WebViewError.JavaScript,
                            "Result exceeds 2 MiB."
                        );
                    }

                    using var envelope = JsonDocument.Parse(text);
                    if (envelope.RootElement.TryGetProperty("error", out var failure))
                    {
                        throw new WebViewException(WebViewError.JavaScript, failure.GetString()!);
                    }

                    var undefined = envelope.RootElement.TryGetProperty("undefined", out _);
                    completion.TrySetResult(
                        Snapshot(
                            request,
                            undefined ? null : envelope.RootElement.GetProperty("json").GetString(),
                            undefined
                        )
                    );
                }
                catch (Exception exception)
                {
                    completion.TrySetException(exception);
                }
            }
        );
        return AwaitResult(request, completion.Task, cancellationToken);
    }

    private Task<WebViewResult> Clear(long request, CancellationToken cancellationToken)
    {
        var completion = Admit(request);
        // Explicitly clears the entire selected profile, including other shared-profile views.
        _store.RemoveDataOfTypes(
            WKWebsiteDataStore.AllWebsiteDataTypes,
            NSDate.DistantPast,
            () =>
            {
                if (!_closed && _pending.Remove(request))
                {
                    completion.TrySetResult(Snapshot(request));
                }
            }
        );
        return AwaitResult(request, completion.Task, cancellationToken);
    }

    private async Task<WebViewResult> AwaitResult(
        long request,
        Task<WebViewResult> task,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            await new AppKitPlatformViewDispatcher().InvokeAsync(() =>
            {
                if (_pending.Remove(request, out var pending))
                {
                    pending.TrySetCanceled();
                }

                return ValueTask.CompletedTask;
            });
        }
    }

    private void CancelPending(WebViewError error, string message)
    {
        foreach (var completion in _pending.Values)
        {
            completion.TrySetException(new WebViewException(error, message));
        }

        _pending.Clear();
    }

    internal void Close()
    {
        if (_closed)
        {
            return;
        }

        _closed = true;
        CancelPending(WebViewError.Closed, "WebView closed before command completion.");
        View.StopLoading();
        _content?.Close();
        if (_messages is not null)
        {
            _configuration.UserContentController.RemoveScriptMessageHandler("doroti");
        }

        View.NavigationDelegate = null!;
        View.UIDelegate = null!;
    }

    public void Dispose()
    {
        Close();
        _navigation.Dispose();
        _ui.Dispose();
        _messages?.Dispose();
        _content?.Dispose();
        _configuration.Dispose();
        _navigations.Clear();
        if (_options.Profile == WebViewProfile.Ephemeral)
        {
            _store.Dispose();
        }
    }
}
#endif
