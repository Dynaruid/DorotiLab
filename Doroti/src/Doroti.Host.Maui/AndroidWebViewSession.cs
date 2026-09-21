#if ANDROID
using Android.Content;
using Android.Graphics;
using Android.Webkit;
using AndroidX.WebKit;
using Doroti.Ui;
using System.Text;
using System.Text.Json;
using NativeWebView = Android.Webkit.WebView;

namespace Doroti.Host.Maui;

/// <summary>SDK commands for the native instance owned by AndroidPlatformViewHost.</summary>
internal sealed class AndroidWebViewSession : IAsyncDisposable
{
    private const string ContentOrigin = "https://appassets.androidplatform.net";
    private const string PrivatePrefix = "doroti-transient-v1-";
    private static bool _cleanedProfiles;
    private readonly PlatformViewHandle _handle;
    private readonly WebViewOptions _options;
    private readonly Action<WebViewEvent> _changed;
    private readonly Dictionary<string, byte[]> _content;
    private readonly Client _client;
    private readonly Chrome _chrome = new();
    private readonly Messages? _messages;
    private readonly string? _privateProfile;
    private readonly IProfile? _profile;
    private readonly CancellationTokenSource _closedToken = new();
    private CancellationTokenSource _documentToken = new();
    private long _request,
        _navigation,
        _document;
    private int _pending;
    private bool _closed,
        _failed,
        _loading,
        _disposed;
    private Task? _disposal;
    private string? _expectedUrl;
    internal NativeWebView View { get; }

    internal static WebViewOptions Parse(string parameters)
    {
        if (
            parameters.StartsWith("doroti-webview:", StringComparison.Ordinal)
            && !parameters.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
        )
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Unknown WebView creation protocol."
            );
        }

        var options = parameters.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
            ? JsonSerializer.Deserialize(
                parameters[WebViewOptions.Prefix.Length..],
                WebViewJsonContext.Default.WebViewOptions
            )
                ?? throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "Missing WebView settings."
                )
            : new WebViewOptions(Html: parameters, Profile: WebViewProfile.SharedPersistent);
        options.Validate();
        return options;
    }

    internal static async Task<Dictionary<string, byte[]>> LoadContent(
        WebViewOptions options,
        IApplicationResourceHostCapability? resources,
        CancellationToken token
    )
    {
        var result = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        long total = 0;
        foreach (var (path, route) in options.Resources ?? [])
        {
            var resourceHost =
                resources
                ?? throw new WebViewException(
                    WebViewError.Unsupported,
                    "Application resources are unavailable."
                );
            var entry =
                resourceHost.Resources.SingleOrDefault(item => item.Key == route.ResourceKey)
                ?? throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "Resource is not in the application manifest: " + route.ResourceKey
                );
            if (entry.Length > 8 * 1024 * 1024 || (total += entry.Length) > 64 * 1024 * 1024)
            {
                throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "App content exceeds 8 MiB per resource or 64 MiB per view."
                );
            }

            var bytes = await resourceHost.LoadAsync(route.ResourceKey, token);
            if (bytes.Length != entry.Length)
            {
                throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "Resource length differs from manifest."
                );
            }

            result.Add(path, bytes.ToArray());
        }
        return result;
    }

    internal AndroidWebViewSession(
        Context context,
        PlatformViewHandle handle,
        WebViewOptions options,
        Dictionary<string, byte[]> content,
        Action<WebViewEvent> changed
    )
    {
        _handle = handle;
        _options = options;
        _content = content;
        _changed = changed;
        var multiProfile = WebViewFeature.IsFeatureSupported(WebViewFeature.MultiProfile);
        if (options.Profile == WebViewProfile.Ephemeral && (!multiProfile || !CanClear))
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Transient profiles require provider isolation and acknowledged data deletion. Explicitly select SharedPersistent."
            );
        }

        if (
            options.MessageOrigins is { Length: > 0 }
            && !WebViewFeature.IsFeatureSupported(WebViewFeature.WebMessageListener)
        )
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Provider has no origin-aware main-frame message listener."
            );
        }
        // Transient profiles may use disk. Delete abandoned Doroti-owned profiles
        // once per process, before any view in this process acquires a profile.
        if (multiProfile && !_cleanedProfiles)
        {
            var store =
                IProfileStore.Instance
                ?? throw new WebViewException(
                    WebViewError.Unsupported,
                    "Profile store unavailable."
                );
            foreach (var name in store.AllProfileNames ?? [])
            {
                if (name.StartsWith(PrivatePrefix, StringComparison.Ordinal))
                {
                    store.DeleteProfile(name);
                }
            }

            _cleanedProfiles = true;
        }
        View = new NativeWebView(context);
        _client = new Client(this);
        try
        {
            if (options.Profile == WebViewProfile.Ephemeral)
            {
                _privateProfile = PrivatePrefix + Guid.NewGuid().ToString("N");
                WebViewCompat.SetProfile(View, _privateProfile);
            }
            if (multiProfile)
            {
                _profile = WebViewCompat.GetProfile(View);
            }

            View.Settings.JavaScriptEnabled = true;
            View.Settings.DomStorageEnabled = true;
            View.Settings.AllowFileAccess = false;
            View.Settings.AllowContentAccess = false;
            View.Settings.MixedContentMode = MixedContentHandling.NeverAllow;
            View.Settings.JavaScriptCanOpenWindowsAutomatically = false;
            View.Settings.SetSupportMultipleWindows(true);
            View.Settings.MediaPlaybackRequiresUserGesture = true;
            View.SetWebViewClient(_client);
            View.SetWebChromeClient(_chrome);
            if (options.MessageOrigins is { Length: > 0 } origins)
            {
                _messages = new Messages(this);
                WebViewCompat.AddWebMessageListener(
                    View,
                    "dorotiNative",
                    origins.Select(MapOrigin).ToArray(),
                    _messages
                );
            }
            // Do not queue a synthetic blank load before the caller's first URI.
            // Older Chromium can finish that cancelled blank load with the new
            // URL/title already active and omit the new document's finish callback.
            if (options.Html is { } html)
            {
                NavigateHtml(html);
            }

            Android.Util.Log.Info(
                "DorotiWebView",
                $"provider={WebViewCompat.GetCurrentWebViewPackage(context)?.VersionName} multiProfile={multiProfile} profile={options.Profile} clear={CanClear}"
            );
        }
        catch
        {
            DestroyNative();
            View.Dispose();
            throw;
        }
    }

    private static string MapOrigin(string origin) =>
        origin == "doroti-app://content" ? ContentOrigin : origin;

    private static string MapUrl(string url) =>
        url.StartsWith("doroti-app://content/", StringComparison.Ordinal)
            ? ContentOrigin + url["doroti-app://content".Length..]
            : url;

    private string? PublicUrl(string? url) =>
        _content.Count > 0 && url?.StartsWith(ContentOrigin + "/", StringComparison.Ordinal) == true
            ? "doroti-app://content" + url[ContentOrigin.Length..]
            : url;

    private bool Allows(string? url)
    {
        if (url == "about:blank")
        {
            return true;
        }

        if (
            !Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("https" or "http")
            || !string.IsNullOrEmpty(uri.UserInfo)
        )
        {
            return false;
        }

        if (uri.GetLeftPart(UriPartial.Authority) == ContentOrigin)
        {
            return _content.Count > 0;
        }

        return _options.AllowedOrigins is not { } allowed
            || allowed.Contains(uri.GetLeftPart(UriPartial.Authority), StringComparer.Ordinal);
    }

    private bool CanClear => WebViewFeature.IsFeatureSupported(WebViewFeature.DeleteBrowsingData);

    private void Advance(string? url)
    {
        _navigation++;
        _document++;
        _loading = true;
        _expectedUrl = url;
        _documentToken.Cancel();
        _documentToken.Dispose();
        _documentToken = new();
    }

    private void NavigateHtml(string html)
    {
        Advance("about:blank");
        View.LoadDataWithBaseURL("about:blank", html, "text/html", "UTF-8", null);
    }

    private void Emit(WebViewEventKind kind, string? error = null)
    {
        if (_closed || (_failed && kind != WebViewEventKind.ProcessFailed))
        {
            return;
        }

        try
        {
            _changed(new(_handle, _navigation, _document, kind, PublicUrl(View.Url), error));
        }
        catch (Exception exception)
        {
            Android.Util.Log.Error("DorotiWebView", exception.ToString());
        }
    }

    private void Verify()
    {
        AndroidPlatformViewDispatcher.VerifyThread();
        if (_closed)
        {
            throw new WebViewException(WebViewError.Closed, "WebView is closed.");
        }

        if (_failed)
        {
            throw new WebViewException(
                WebViewError.ProcessFailed,
                "Renderer terminated; recreate the controller. Document state is not recovered."
            );
        }
    }

    private WebViewResult Snapshot(
        long request,
        string? json = null,
        bool undefined = false,
        WebViewFeatures? features = null
    )
    {
        Verify();
        return new(
            request,
            _navigation,
            _document,
            json,
            undefined,
            PublicUrl(View.Url),
            View.Title,
            _loading,
            View.CanGoBack(),
            View.CanGoForward(),
            features
        );
    }

    internal Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken token)
    {
        Verify();
        token.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(command.Operation))
        {
            throw new WebViewException(WebViewError.Unsupported, "Unknown WebView operation.");
        }

        if (command.DocumentGeneration != 0 && command.DocumentGeneration != _document)
        {
            throw new WebViewException(
                WebViewError.NavigationChanged,
                "Command belongs to an old document."
            );
        }

        if (Encoding.UTF8.GetByteCount(command.Text ?? "") > 2 * 1024 * 1024)
        {
            throw new WebViewException(WebViewError.InvalidRequest, "Command exceeds 2 MiB.");
        }

        var request = ++_request;
        switch (command.Operation)
        {
            case WebViewOperation.Features:
                return Task.FromResult(
                    Snapshot(
                        request,
                        features: new(
                            true,
                            true,
                            WebViewFeature.IsFeatureSupported(WebViewFeature.MultiProfile)
                                && CanClear,
                            true,
                            CanClear,
                            ScriptMessages: _messages is not null,
                            AppContentScheme: _content.Count > 0
                        )
                    )
                );
            case WebViewOperation.State:
                break;
            case WebViewOperation.Navigate:
                var url = MapUrl(command.Text ?? "");
                if (!Allows(url) || url == "about:blank")
                {
                    throw new WebViewException(
                        WebViewError.InvalidRequest,
                        "URL rejected by navigation policy."
                    );
                }

                Advance(url);
                View.LoadUrl(url);
                break;
            case WebViewOperation.LoadHtml:
                NavigateHtml(command.Text ?? "");
                break;
            case WebViewOperation.Reload:
                if (_document == 0)
                {
                    throw new WebViewException(
                        WebViewError.NotReady,
                        "Load a document before reloading."
                    );
                }

                Advance(View.Url);
                View.Reload();
                break;
            case WebViewOperation.Stop:
                View.StopLoading();
                _loading = false;
                break;
            case WebViewOperation.Back:
                if (!View.CanGoBack())
                {
                    throw new WebViewException(
                        WebViewError.InvalidRequest,
                        "No back history entry."
                    );
                }

                Advance(null);
                View.GoBack();
                break;
            case WebViewOperation.Forward:
                if (!View.CanGoForward())
                {
                    throw new WebViewException(
                        WebViewError.InvalidRequest,
                        "No forward history entry."
                    );
                }

                Advance(null);
                View.GoForward();
                break;
            case WebViewOperation.EvaluateJavaScript:
                if (_loading || _document == 0)
                {
                    throw new WebViewException(
                        WebViewError.NotReady,
                        "Load a document and wait for completion."
                    );
                }

                return Evaluate(request, command.Text ?? "undefined", token);
            case WebViewOperation.ClearData:
                return ClearData(request, token);
        }
        return Task.FromResult(Snapshot(request));
    }

    private sealed class ValueCallback(Action<string?> completed) : Java.Lang.Object, IValueCallback
    {
        public void OnReceiveValue(Java.Lang.Object? value) => completed(value?.ToString());
    }

    private async Task<WebViewResult> Evaluate(long request, string script, CancellationToken token)
    {
        if (_pending >= 16)
        {
            throw new WebViewException(
                WebViewError.Busy,
                "At most 16 native operations may be pending."
            );
        }

        var generation = _document;
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(
            token,
            _closedToken.Token,
            _documentToken.Token
        );
        timeout.CancelAfter(TimeSpan.FromSeconds(15));
        var completion = new TaskCompletionSource<string?>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var callback = new ValueCallback(value =>
        {
            Interlocked.Decrement(ref _pending);
            completion.TrySetResult(value);
        });
        var encoded = JsonEncodedText.Encode(script).ToString();
        var wrapper =
            "(()=>{try{const v=(0,eval)(\""
            + encoded
            + "\");if(v&&typeof v.then==='function')throw Error('Promise results are unsupported');if(v===undefined)return {undefined:true};const json=JSON.stringify(v);if(json===undefined)throw Error('Result is not JSON serializable');return {json};}catch(e){return {error:String(e)}}})()";
        Interlocked.Increment(ref _pending);
        try
        {
            View.EvaluateJavascript(wrapper, callback);
        }
        catch
        {
            Interlocked.Decrement(ref _pending);
            callback.Dispose();
            throw;
        }
        // A cancelled caller does not cancel Chromium. Hold the admission slot and
        // callback peer until the real callback, including after document replacement.
        _ = completion.Task.ContinueWith(_ => callback.Dispose(), TaskScheduler.Default);
        try
        {
            var result = await completion.Task.WaitAsync(timeout.Token);
            Verify();
            if (generation != _document)
            {
                throw new WebViewException(WebViewError.NavigationChanged, "Document changed.");
            }

            if (Encoding.UTF8.GetByteCount(result ?? "") > 2 * 1024 * 1024)
            {
                throw new WebViewException(WebViewError.JavaScript, "Result exceeds 2 MiB.");
            }

            using var json = JsonDocument.Parse(result ?? "null");
            if (json.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new WebViewException(WebViewError.JavaScript, "Invalid evaluation result.");
            }

            if (json.RootElement.TryGetProperty("error", out var error))
            {
                throw new WebViewException(
                    WebViewError.JavaScript,
                    error.GetString() ?? "Evaluation failed."
                );
            }

            return json.RootElement.TryGetProperty("undefined", out _)
                ? Snapshot(request, undefined: true)
                : Snapshot(request, json.RootElement.GetProperty("json").GetString());
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            throw new WebViewException(
                _closed ? WebViewError.Closed
                    : _failed ? WebViewError.ProcessFailed
                    : generation != _document ? WebViewError.NavigationChanged
                    : WebViewError.JavaScript,
                "Evaluation invalidated or timed out."
            );
        }
    }

    private async Task<WebViewResult> ClearData(long request, CancellationToken token)
    {
        if (!CanClear)
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Provider cannot acknowledge complete browsing-data deletion."
            );
        }

        if (_pending >= 16)
        {
            throw new WebViewException(WebViewError.Busy, "Too many pending operations.");
        }

        var completion = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var callback = new Java.Lang.Runnable(() =>
        {
            Interlocked.Decrement(ref _pending);
            completion.TrySetResult();
        });
        Interlocked.Increment(ref _pending);
        try
        {
            WebStorageCompat.DeleteBrowsingData(
                _profile?.WebStorage ?? WebStorage.Instance,
                callback
            );
        }
        catch
        {
            Interlocked.Decrement(ref _pending);
            callback.Dispose();
            throw;
        }
        _ = completion.Task.ContinueWith(_ => callback.Dispose(), TaskScheduler.Default);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(
            token,
            _closedToken.Token
        );
        timeout.CancelAfter(TimeSpan.FromSeconds(15));
        await completion.Task.WaitAsync(timeout.Token);
        return Snapshot(request);
    }

    private sealed class Client(AndroidWebViewSession owner) : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(
            NativeWebView? view,
            IWebResourceRequest? request
        ) => owner._closed || !owner.Allows(request?.Url?.ToString());

        public override void OnPageStarted(NativeWebView? view, string? url, Bitmap? favicon)
        {
            if (owner._closed || owner._failed)
            {
                return;
            }

            owner.TraceNavigation("started", url);
            // Ready is attachment readiness. A slow provider may deliver the
            // constructor's blank-document callback after a command replaced it.
            if (
                owner._loading
                && url == "about:blank"
                && owner._expectedUrl is not (null or "about:blank")
            )
            {
                return;
            }

            if (!owner.Allows(url))
            {
                owner.View.StopLoading();
                owner._loading = false;
                owner.Emit(WebViewEventKind.Failed, "Navigation rejected.");
                return;
            }
            if (!owner._loading || (owner._expectedUrl is not null && owner._expectedUrl != url))
            {
                owner.Advance(url);
            }

            owner._expectedUrl = url;
            owner.Emit(WebViewEventKind.Started);
        }

        public override void OnPageCommitVisible(NativeWebView? view, string? url)
        {
            if (!owner._closed && url == owner._expectedUrl)
            {
                owner.Emit(WebViewEventKind.Committed);
            }
        }

        public override void OnPageFinished(NativeWebView? view, string? url)
        {
            owner.TraceNavigation("finished", url);
            if (owner._closed || owner._failed || url != owner._expectedUrl)
            {
                return;
            }

            owner._loading = false;
            if (owner._messages is not null)
            {
                owner.View.EvaluateJavascript(
                    "(()=>{let request=0;const generation="
                        + owner._document.ToString(
                            System.Globalization.CultureInfo.InvariantCulture
                        )
                        + ";window.doroti={postMessage:(name,payload)=>dorotiNative.postMessage(JSON.stringify({version:1,documentGeneration:generation,requestId:++request,name,payload}))}})()",
                    null
                );
            }

            owner.Emit(WebViewEventKind.Completed);
        }

        public override void OnReceivedError(
            NativeWebView? view,
            IWebResourceRequest? request,
            WebResourceError? error
        )
        {
            if (request?.IsForMainFrame == true)
            {
                owner._loading = false;
                owner.Emit(WebViewEventKind.Failed, error?.Description?.ToString());
            }
        }

        public override void OnReceivedSslError(
            NativeWebView? view,
            SslErrorHandler? handler,
            Android.Net.Http.SslError? error
        )
        {
            handler?.Cancel();
            owner._loading = false;
            owner.Emit(WebViewEventKind.Failed, "TLS validation failed.");
        }

        public override bool OnRenderProcessGone(
            NativeWebView? view,
            RenderProcessGoneDetail? detail
        )
        {
            if (owner._closed)
            {
                return true;
            }

            owner._failed = true;
            owner._loading = false;
            owner._documentToken.Cancel();
            owner.Emit(WebViewEventKind.ProcessFailed, "Android WebView renderer terminated.");
            // The coordinator still owns attachment retirement. Prevent drawing the
            // dead renderer until the application disposes/recreates its controller.
            owner.View.Visibility = Android.Views.ViewStates.Invisible;
            return true;
        }

        public override WebResourceResponse? ShouldInterceptRequest(
            NativeWebView? view,
            IWebResourceRequest? request
        ) => owner.Resource(request);
    }

    private void TraceNavigation(string kind, string? url)
    {
        if (
            !_closed
            && Environment.GetEnvironmentVariable("DOROTI_ANDROID_WEBVIEW_EVIDENCE") is not null
        )
        {
            Android.Util.Log.Info(
                "DorotiWebView",
                $"{kind} instance={_handle.InstanceId} url={url} expected={_expectedUrl} progress={View.Progress}"
            );
        }
    }

    private sealed class Chrome : WebChromeClient
    {
        public override void OnPermissionRequest(PermissionRequest? request) => request?.Deny();

        public override bool OnCreateWindow(
            NativeWebView? view,
            bool isDialog,
            bool isUserGesture,
            Android.OS.Message? resultMsg
        ) => false;

        public override bool OnShowFileChooser(
            NativeWebView? webView,
            IValueCallback? filePathCallback,
            FileChooserParams? fileChooserParams
        )
        {
            filePathCallback?.OnReceiveValue(null);
            return true;
        }

        public override void OnGeolocationPermissionsShowPrompt(
            string? origin,
            GeolocationPermissions.ICallback? callback
        ) => callback?.Invoke(origin, false, false);
    }

    private sealed class Messages(AndroidWebViewSession owner)
        : Java.Lang.Object,
            WebViewCompat.IWebMessageListener
    {
        public void OnPostMessage(
            NativeWebView? view,
            WebMessageCompat? message,
            Android.Net.Uri? origin,
            bool mainFrame,
            JavaScriptReplyProxy? reply
        )
        {
            if (
                owner._closed
                || owner._failed
                || owner._loading
                || !mainFrame
                || origin is null
                || owner
                    ._options.MessageOrigins?.Select(MapOrigin)
                    .Contains(origin.ToString(), StringComparer.Ordinal) != true
            )
            {
                return;
            }

            var text = message?.Data;
            if (text is null || Encoding.UTF8.GetByteCount(text) > 64 * 1024)
            {
                return;
            }

            try
            {
                using var json = JsonDocument.Parse(text);
                var root = json.RootElement;
                if (
                    root.GetProperty("version").GetInt32() != 1
                    || root.GetProperty("documentGeneration").GetInt64() != owner._document
                )
                {
                    return;
                }

                var name = root.GetProperty("name").GetString();
                var id = root.GetProperty("requestId").GetInt64();
                if (string.IsNullOrEmpty(name) || name.Length > 128 || id <= 0)
                {
                    return;
                }

                owner._changed(
                    new(
                        owner._handle,
                        owner._navigation,
                        owner._document,
                        WebViewEventKind.Message,
                        owner.PublicUrl(owner.View.Url),
                        MessageName: name,
                        MessageJson: root.GetProperty("payload").GetRawText(),
                        MessageRequestId: id
                    )
                );
            }
            catch (Exception exception)
            {
                Android.Util.Log.Info("DorotiWebView", "Rejected message: " + exception.Message);
            }
        }
    }

    private WebResourceResponse? Resource(IWebResourceRequest? request)
    {
        if (
            !Uri.TryCreate(request?.Url?.ToString(), UriKind.Absolute, out var uri)
            || uri.Host != "appassets.androidplatform.net"
        )
        {
            return null;
        }

        WebResourceResponse Response(
            int status,
            string reason,
            byte[] bytes,
            int start,
            int count,
            IDictionary<string, string> headers,
            string mime = "text/plain"
        ) =>
            new(
                mime,
                "UTF-8",
                status,
                reason,
                headers,
                new MemoryStream(bytes, start, count, writable: false)
            );
        if (
            _closed
            || uri.Scheme != "https"
            || !uri.IsDefaultPort
            || uri.UserInfo.Length != 0
            || uri.AbsolutePath.Contains('%')
            || !_content.TryGetValue(uri.AbsolutePath, out var bytes)
            || _options.Resources?.TryGetValue(uri.AbsolutePath, out var route) != true
            || route is null
            || request?.Method is not ("GET" or "HEAD")
        )
        {
            return Response(404, "Not Found", [], 0, 0, new Dictionary<string, string>());
        }

        var headers = new Dictionary<string, string>
        {
            ["Cache-Control"] = "no-store",
            ["Accept-Ranges"] = "bytes",
            ["X-Content-Type-Options"] = "nosniff",
        };
        var start = 0;
        var end = bytes.Length - 1;
        var status = 200;
        var range = request
            .RequestHeaders?.FirstOrDefault(pair =>
                pair.Key.Equals("Range", StringComparison.OrdinalIgnoreCase)
            )
            .Value;
        if (range is not null)
        {
            var parts = range.StartsWith("bytes=", StringComparison.Ordinal)
                ? range[6..].Split('-')
                : [];
            var valid = parts.Length == 2 && !range.Contains(',');
            if (valid && parts[0].Length == 0)
            {
                valid = int.TryParse(parts[1], out var suffix) && suffix > 0;
                if (valid)
                {
                    start = Math.Max(0, bytes.Length - suffix);
                }
            }
            else if (valid)
            {
                valid = int.TryParse(parts[0], out start) && start >= 0;
                if (valid && parts[1].Length > 0)
                {
                    valid = int.TryParse(parts[1], out end);
                }
                end = Math.Min(end, bytes.Length - 1);
            }
            if (!valid || start > end || start >= bytes.Length)
            {
                return Response(
                    416,
                    "Range Not Satisfiable",
                    [],
                    0,
                    0,
                    new Dictionary<string, string> { ["Content-Range"] = $"bytes */{bytes.Length}" }
                );
            }

            status = 206;
            headers["Content-Range"] = $"bytes {start}-{end}/{bytes.Length}";
        }
        var length = Math.Max(0, end - start + 1);
        headers["Content-Length"] = length.ToString(
            System.Globalization.CultureInfo.InvariantCulture
        );
        return Response(
            status,
            status == 206 ? "Partial Content" : "OK",
            bytes,
            start,
            request.Method == "HEAD" ? 0 : length,
            headers,
            route.MimeType
        );
    }

    internal void CloseCommands()
    {
        if (_closed)
        {
            return;
        }

        _closed = true;
        _closedToken.Cancel();
        _documentToken.Cancel();
        if (_messages is not null)
        {
            WebViewCompat.RemoveWebMessageListener(View, "dorotiNative");
        }

        if (!_failed)
        {
            View.StopLoading();
        }
    }

    private void DestroyNative()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        CloseCommands();
        View.Destroy();
        _client.Dispose();
        _chrome.Dispose();
        _messages?.Dispose();
        _documentToken.Dispose();
        _closedToken.Dispose();
    }

    public ValueTask DisposeAsync() => new(_disposal ??= DisposeCoreAsync());

    private async Task DisposeCoreAsync()
    {
        DestroyNative();
        try
        {
            if (_privateProfile is not null && _profile is not null)
            {
                // Loaded profiles cannot be deleted in this process, even after
                // WebView.destroy(). Clear their data with an acknowledgement now;
                // remove the empty profile shell before loading profiles next run.
                var completion = new TaskCompletionSource(
                    TaskCreationOptions.RunContinuationsAsynchronously
                );
                var callback = new Java.Lang.Runnable(() => completion.TrySetResult());
                WebStorageCompat.DeleteBrowsingData(_profile.WebStorage, callback);
                _ = completion.Task.ContinueWith(_ => callback.Dispose(), TaskScheduler.Default);
                await completion.Task.WaitAsync(TimeSpan.FromSeconds(15));
            }
        }
        finally
        {
            _profile?.Dispose();
        }
    }
}
#endif
