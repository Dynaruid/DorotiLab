using System.Text;
using System.Text.Json;
using Doroti.Ui;
using Microsoft.Web.WebView2.Core;

namespace Doroti.Host.WindowsAppSdk;

internal sealed partial class WindowsWebViewComposition
{
    private static WebViewOptions ParseOptions(ReadOnlyMemory<byte> parameters)
    {
        var text = Encoding.UTF8.GetString(parameters.Span);
        if (
            text.StartsWith("doroti-webview:", StringComparison.Ordinal)
            && !text.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
        )
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Unknown WebView creation protocol."
            );
        }

        var options = text.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
            ? JsonSerializer.Deserialize(
                text[WebViewOptions.Prefix.Length..],
                WebViewJsonContext.Default.WebViewOptions
            ) ?? throw new WebViewException(WebViewError.InvalidRequest, "Missing WebView options.")
            : new WebViewOptions(parameters.IsEmpty ? null : text);
        options.Validate();
        return options;
    }

    private static async Task<CoreWebView2Environment> CreateEnvironment(string folder)
    {
        var options = new CoreWebView2EnvironmentOptions();
        options.CustomSchemeRegistrations =
        [
            new CoreWebView2CustomSchemeRegistration("doroti-app")
            {
                HasAuthorityComponent = true,
                TreatAsSecure = 1,
                AllowedOrigins = { "doroti-app://content" },
            },
        ];
        try
        {
            return await CoreWebView2Environment.CreateWithOptionsAsync(null, folder, options);
        }
        catch (Exception error)
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "WebView2 runtime/environment creation failed: " + error.Message
            );
        }
    }

    private sealed partial class Instance
    {
        private readonly WebViewOptions _options;
        private readonly CancellationTokenSource _commandsClosed = new();
        private CancellationTokenSource _documentChanged = new();
        private long _requestId,
            _navigationId,
            _documentGeneration;
        private ulong _nativeNavigation;
        private bool _loading,
            _commandsDisabled,
            _processFailed,
            _navigationRequested;
        private int _pending;
        private string? _expectedHtmlNavigation;
        internal Queue<string> NavigationEvents { get; } = new();

        private void TraceNavigation(string value)
        {
            NavigationEvents.Enqueue(value);
            while (NavigationEvents.Count > 16)
            {
                NavigationEvents.Dequeue();
            }

            if (
                !string.IsNullOrEmpty(
                    Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE")
                )
            )
            {
                Console.Error.WriteLine(
                    $"doroti.webview.navigation={_handle.InstanceGeneration}:{value}"
                );
            }
        }

        private readonly SynchronizationContext _commandContext =
            SynchronizationContext.Current
            ?? throw new InvalidOperationException(
                "WebView creation requires the owner UI dispatcher."
            );
        public event Action<WebViewEvent>? WebViewChanged;

        private void ConnectCommands()
        {
            Core.NavigationStarting += Starting;
            Core.ContentLoading += ContentLoading;
            Core.NewWindowRequested += NewWindow;
            Core.PermissionRequested += Permission;
            Core.DownloadStarting += Download;
            Core.LaunchingExternalUriScheme += ExternalUri;
            Core.ProcessFailed += ProcessFailed;
            Core.Settings.AreDefaultScriptDialogsEnabled = false;
            Core.Settings.AreHostObjectsAllowed = false;
            if (_options.Resources is { Count: > 0 })
            {
                Core.AddWebResourceRequestedFilter(
                    "doroti-app://*",
                    CoreWebView2WebResourceContext.All,
                    CoreWebView2WebResourceRequestSourceKinds.All
                );
                Core.WebResourceRequested += ResourceRequested;
            }
        }

        private bool Allows(string? value)
        {
            if (value == "about:blank")
            {
                return true;
            }

            if (
                !Uri.TryCreate(value, UriKind.Absolute, out var uri)
                || !string.IsNullOrEmpty(uri.UserInfo)
            )
            {
                return false;
            }

            if (uri.Scheme == "doroti-app")
            {
                return uri.Host == "content"
                    && uri.IsDefaultPort
                    && _options.Resources is { Count: > 0 };
            }

            return uri.Scheme is "https" or "http"
                && (
                    _options.AllowedOrigins is null
                    || _options.AllowedOrigins.Contains(
                        uri.GetLeftPart(UriPartial.Authority),
                        StringComparer.Ordinal
                    )
                );
        }

        private void Starting(object? sender, CoreWebView2NavigationStartingEventArgs args)
        {
            var requestedHtml =
                !args.IsRedirected
                && _expectedHtmlNavigation is { } expected
                && args.Uri == expected;
            TraceNavigation(
                $"started:{args.NavigationId}:html={requestedHtml}:allowed={Allows(args.Uri)}"
            );
            _expectedHtmlNavigation = null;
            if (_commandsDisabled || _processFailed || !(requestedHtml || Allows(args.Uri)))
            {
                args.Cancel = true;
                return;
            }
            if (_nativeNavigation != args.NavigationId)
            {
                _nativeNavigation = args.NavigationId;
                _navigationId++;
                if (!_navigationRequested)
                {
                    AdvanceDocument();
                }

                _navigationRequested = false;
            }
            _loading = true;
            Loaded = false;
            Emit(WebViewEventKind.Started, url: args.Uri);
        }

        private void AdvanceDocument()
        {
            _documentGeneration++;
            _documentChanged.Cancel();
            _documentChanged.Dispose();
            _documentChanged = new();
        }

        private void Navigate(Action action)
        {
            // Invalidate pending evaluation immediately, before WebView2 queues
            // NavigationStarting behind a busy renderer script.
            AdvanceDocument();
            _navigationRequested = true;
            InitialHtml = null;
            _loading = true;
            action();
        }

        private void NavigateHtml(string html) =>
            Navigate(() =>
            {
                _expectedHtmlNavigation =
                    "data:text/html;charset=utf-8;base64,"
                    + Convert.ToBase64String(Encoding.UTF8.GetBytes(html));
                Core.NavigateToString(html);
            });

        private void ContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs args)
        {
            if (!_commandsDisabled && args.NavigationId == _nativeNavigation)
            {
                Emit(WebViewEventKind.Committed);
            }
        }

        private void CompleteNavigation(CoreWebView2NavigationCompletedEventArgs args)
        {
            if (_commandsDisabled || args.NavigationId != _nativeNavigation)
            {
                return;
            }

            _loading = false;
            if (args.IsSuccess && _options.MessageOrigins is { Length: > 0 })
            {
                var generation = _documentGeneration;
                _commandContext.Post(
                    state =>
                    {
                        if (!_commandsDisabled && generation == _documentGeneration)
                        {
                            _ = InstallBridge();
                        }
                    },
                    null
                );
            }
            Emit(
                args.IsSuccess ? WebViewEventKind.Completed : WebViewEventKind.Failed,
                args.IsSuccess ? null : args.WebErrorStatus.ToString()
            );
        }

        private async Task InstallBridge()
        {
            try
            {
                await Evaluate(
                    ++_requestId,
                    "window.doroti={postMessage:(()=>{let request=0;const generation="
                        + _documentGeneration.ToString(
                            System.Globalization.CultureInfo.InvariantCulture
                        )
                        + ";return (name,payload)=>window.chrome.webview.postMessage({version:1,documentGeneration:generation,requestId:++request,name,payload});})()};undefined",
                    CancellationToken.None
                );
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.TraceInformation("WebView bridge: " + error.Message);
            }
        }

        private void ReceiveMessage(CoreWebView2WebMessageReceivedEventArgs args)
        {
            if (
                _commandsDisabled
                || _processFailed
                || !Uri.TryCreate(args.Source, UriKind.Absolute, out var source)
                || _options.MessageOrigins?.Contains(
                    source.GetLeftPart(UriPartial.Authority),
                    StringComparer.Ordinal
                ) != true
            )
            {
                return;
            }

            var text = args.WebMessageAsJson;
            if (Encoding.UTF8.GetByteCount(text) > 64 * 1024)
            {
                return;
            }

            try
            {
                using var json = JsonDocument.Parse(text);
                var root = json.RootElement;
                if (
                    root.GetProperty("version").GetInt32() != 1
                    || root.GetProperty("documentGeneration").GetInt64() != _documentGeneration
                )
                {
                    return;
                }

                var name = root.GetProperty("name").GetString();
                var request = root.GetProperty("requestId").GetInt64();
                if (string.IsNullOrEmpty(name) || name.Length > 128 || request <= 0)
                {
                    return;
                }

                WebViewChanged?.Invoke(
                    new(
                        _handle,
                        _navigationId,
                        _documentGeneration,
                        WebViewEventKind.Message,
                        args.Source,
                        MessageName: name,
                        MessageJson: root.GetProperty("payload").GetRawText(),
                        MessageRequestId: request
                    )
                );
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.TraceInformation(
                    "Rejected WebView message: " + error.Message
                );
            }
        }

        private void NewWindow(object? sender, CoreWebView2NewWindowRequestedEventArgs args) =>
            args.Handled = true;

        private void Permission(object? sender, CoreWebView2PermissionRequestedEventArgs args)
        {
            args.State = CoreWebView2PermissionState.Deny;
            args.SavesInProfile = false;
            args.Handled = true;
        }

        private void Download(object? sender, CoreWebView2DownloadStartingEventArgs args)
        {
            args.Cancel = true;
            args.Handled = true;
        }

        private void ExternalUri(
            object? sender,
            CoreWebView2LaunchingExternalUriSchemeEventArgs args
        ) => args.Cancel = true;

        private void ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs args)
        {
            _processFailed = true;
            _loading = false;
            _documentChanged.Cancel();
            Emit(WebViewEventKind.ProcessFailed, args.ProcessFailedKind.ToString());
        }

        private void Emit(WebViewEventKind kind, string? error = null, string? url = null)
        {
            if (_commandsDisabled)
            {
                return;
            }

            try
            {
                WebViewChanged?.Invoke(
                    new(
                        _handle,
                        _navigationId,
                        _documentGeneration,
                        kind,
                        url ?? Core.Source,
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
        )
        {
            // Native completion may already be queued when close/process failure
            // cancels its waiter. Do not read a closed COM object or emit success.
            if (_commandsDisabled)
            {
                throw new WebViewException(WebViewError.Closed, "WebView is closed.");
            }

            if (_processFailed)
            {
                throw new WebViewException(WebViewError.ProcessFailed, "WebView process failed.");
            }

            return new(
                request,
                _navigationId,
                _documentGeneration,
                json,
                undefined,
                Core.Source,
                Core.DocumentTitle,
                _loading || InitialHtml is not null,
                Core.CanGoBack,
                Core.CanGoForward,
                features
            );
        }

        public Task<WebViewResult> ExecuteAsync(
            WebViewCommand command,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_commandsDisabled)
            {
                throw new WebViewException(WebViewError.Closed, "WebView is closed.");
            }

            if (_processFailed)
            {
                throw new WebViewException(
                    WebViewError.ProcessFailed,
                    "Recreate the failed WebView; document state is not recovered."
                );
            }

            if (!Enum.IsDefined(command.Operation))
            {
                throw new WebViewException(WebViewError.Unsupported, "Unknown command.");
            }

            if (
                command.DocumentGeneration != 0
                && command.DocumentGeneration != _documentGeneration
            )
            {
                throw new WebViewException(
                    WebViewError.NavigationChanged,
                    "Command refers to an old document."
                );
            }

            if (Encoding.UTF8.GetByteCount(command.Text ?? "") > 2 * 1024 * 1024)
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
                                ScriptMessages: _options.MessageOrigins is { Length: > 0 },
                                AppContentScheme: _options.Resources is { Count: > 0 }
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
                            "URL rejected by navigation policy."
                        );
                    }

                    Navigate(() => Core.Navigate(command.Text));
                    break;
                case WebViewOperation.LoadHtml:
                    NavigateHtml(command.Text ?? "");
                    break;
                case WebViewOperation.Reload:
                    Navigate(Core.Reload);
                    break;
                case WebViewOperation.Stop:
                    Core.Stop();
                    break;
                case WebViewOperation.Back:
                    if (!Core.CanGoBack)
                    {
                        throw new WebViewException(
                            WebViewError.InvalidRequest,
                            "No back history entry."
                        );
                    }

                    Navigate(Core.GoBack);
                    break;
                case WebViewOperation.Forward:
                    if (!Core.CanGoForward)
                    {
                        throw new WebViewException(
                            WebViewError.InvalidRequest,
                            "No forward history entry."
                        );
                    }

                    Navigate(Core.GoForward);
                    break;
                case WebViewOperation.EvaluateJavaScript:
                    if (_loading || InitialHtml is not null)
                    {
                        throw new WebViewException(
                            WebViewError.NotReady,
                            "Wait for document load."
                        );
                    }

                    return Evaluate(request, command.Text ?? "undefined", cancellationToken);
                case WebViewOperation.ClearData:
                    return ClearData(request, cancellationToken);
            }
            return Task.FromResult(Snapshot(request));
        }

        private async Task<WebViewResult> Evaluate(
            long request,
            string script,
            CancellationToken cancellationToken
        )
        {
            if (_pending >= 16)
            {
                throw new WebViewException(
                    WebViewError.Busy,
                    "At most 16 WebView operations can be pending."
                );
            }

            Interlocked.Increment(ref _pending);
            var generation = _documentGeneration;
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _commandsClosed.Token,
                _documentChanged.Token
            );
            timeout.CancelAfter(TimeSpan.FromSeconds(15));
            Task<string>? native = null;
            try
            {
                var encoded = JsonEncodedText.Encode(script).ToString();
                var wrapper =
                    "(()=>{try{const v=(0,eval)(\""
                    + encoded
                    + "\");if(v&&typeof v.then==='function')throw Error('Promise results are unsupported');if(v===undefined)return {undefined:true};const json=JSON.stringify(v);if(json===undefined)throw Error('Result is not JSON serializable');return {json};}catch(e){return {error:String(e)}}})()";
                native = Core.ExecuteScriptAsync(wrapper).AsTask();
                var result = await native.WaitAsync(timeout.Token);
                if (generation != _documentGeneration)
                {
                    throw new WebViewException(WebViewError.NavigationChanged, "Document changed.");
                }

                using var json = JsonDocument.Parse(result);
                if (json.RootElement.TryGetProperty("error", out var error))
                {
                    throw new WebViewException(
                        WebViewError.JavaScript,
                        error.GetString() ?? "Evaluation failed."
                    );
                }

                if (json.RootElement.TryGetProperty("undefined", out _))
                {
                    return Snapshot(request, undefined: true);
                }

                var value = json.RootElement.GetProperty("json").GetString();
                if (Encoding.UTF8.GetByteCount(value ?? "") > 2 * 1024 * 1024)
                {
                    throw new WebViewException(WebViewError.JavaScript, "Result exceeds 2 MiB.");
                }

                return Snapshot(request, value);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new WebViewException(
                    _commandsDisabled ? WebViewError.Closed
                        : _processFailed ? WebViewError.ProcessFailed
                        : generation != _documentGeneration ? WebViewError.NavigationChanged
                        : WebViewError.JavaScript,
                    "WebView evaluation invalidated or timed out."
                );
            }
            finally
            {
                // Canceling a caller does not cancel Chromium execution. Keep its
                // admission slot until the native callback really completes.
                if (native is { IsCompleted: false })
                {
                    _ = ReleaseNative(native);
                }
                else
                {
                    Interlocked.Decrement(ref _pending);
                }
            }
        }

        private async Task ReleaseNative(Task native)
        {
            try
            {
                await native.ConfigureAwait(false);
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.TraceInformation(
                    "Retired WebView operation: " + error.Message
                );
            }
            finally
            {
                Interlocked.Decrement(ref _pending);
            }
        }

        private async Task<WebViewResult> ClearData(
            long request,
            CancellationToken cancellationToken
        )
        {
            if (_pending >= 16)
            {
                throw new WebViewException(
                    WebViewError.Busy,
                    "At most 16 WebView operations can be pending."
                );
            }

            Interlocked.Increment(ref _pending);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _commandsClosed.Token
            );
            timeout.CancelAfter(TimeSpan.FromSeconds(15));
            Task? native = null;
            try
            {
                native = Core
                    .Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.AllProfile)
                    .AsTask();
                await native.WaitAsync(timeout.Token);
                // Runtime AllProfile does not clear the registered custom-scheme
                // DOM origin. Explicitly clear that fixed, host-owned origin too.
                if (_content.Count != 0)
                {
                    native = Core.CallDevToolsProtocolMethodAsync(
                            "Storage.clearDataForOrigin",
                            "{\"origin\":\"doroti-app://content\",\"storageTypes\":\"all\"}"
                        )
                        .AsTask();
                    await native.WaitAsync(timeout.Token);
                }
                return Snapshot(request);
            }
            finally
            {
                if (native is { IsCompleted: false })
                {
                    _ = ReleaseNative(native);
                }
                else
                {
                    Interlocked.Decrement(ref _pending);
                }
            }
        }

        private void CloseCommands()
        {
            if (_commandsDisabled)
            {
                return;
            }

            _commandsDisabled = true;
            _commandsClosed.Cancel();
            _documentChanged.Cancel();
            Core.NavigationStarting -= Starting;
            Core.ContentLoading -= ContentLoading;
            Core.NewWindowRequested -= NewWindow;
            Core.PermissionRequested -= Permission;
            Core.DownloadStarting -= Download;
            Core.LaunchingExternalUriScheme -= ExternalUri;
            Core.ProcessFailed -= ProcessFailed;
            Core.WebResourceRequested -= ResourceRequested;
            WebViewChanged = null;
        }
    }
}
