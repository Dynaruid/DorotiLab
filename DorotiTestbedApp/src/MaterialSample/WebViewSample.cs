using System.Text;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

/// <summary>A product-facing WebView sample with a basic attachment fallback for hosts
/// that do not yet expose the optional navigation/JavaScript command adapter.</summary>
internal sealed class WebViewSample : StatefulWidget
{
    public override IState createState() => new WebViewSampleState();
}

internal sealed class WebViewSampleState : State<WebViewSample>
{
    private const string InitialAddress = "https://www.youtube.com/watch?v=hI9HQfCAw64";
    private const string InitialHtml = """
        <!doctype html>
        <html lang="en">
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>Doroti WebView sample</title>
          <style>
            :root { color-scheme: light dark; font-family: system-ui, sans-serif; }
            body { margin: 0; min-height: 100vh; color: #172033; background: linear-gradient(135deg, #eef4ff, #f8efff); }
            main { max-width: 760px; margin: auto; padding: 40px 24px 80px; }
            .eyebrow { color: #6650a4; font-weight: 700; letter-spacing: .08em; text-transform: uppercase; }
            h1 { margin: 8px 0 12px; font-size: clamp(2rem, 6vw, 4rem); line-height: 1; }
            .card { margin-top: 28px; padding: 24px; border: 1px solid #c7c7d7; border-radius: 24px; background: #ffffffcc; box-shadow: 0 18px 50px #55447722; }
            label { display: grid; gap: 8px; margin: 18px 0; font-weight: 650; }
            input, button { box-sizing: border-box; border-radius: 12px; font: inherit; }
            input { width: 100%; padding: 12px 14px; color: #172033; background: white; border: 1px solid #77778a; }
            button { padding: 12px 18px; color: white; background: #6750a4; border: 0; cursor: pointer; }
            #pulse { width: 72px; height: 72px; margin-top: 24px; border-radius: 20px; background: #7d5260; animation: pulse 1.8s ease-in-out infinite alternate; }
            @keyframes pulse { to { transform: translateX(min(45vw, 280px)) rotate(24deg); background: #386a20; } }
            @media (prefers-color-scheme: dark) {
              body { color: #eeeaf4; background: linear-gradient(135deg, #171825, #281c30); }
              .eyebrow { color: #d0bcff; } .card { background: #211f2acc; border-color: #4b4758; }
            }
          </style>
        </head>
        <body>
          <main>
            <div class="eyebrow">Native WebView</div>
            <h1>Web content inside Doroti</h1>
            <p>This page is live HTML. Try native text input, the counter, scrolling, and the CSS animation.</p>
            <section class="card">
              <label>Native text input <input value="Doroti WebView input"></label>
              <button id="counter" type="button">Count: 0</button>
              <div id="pulse" aria-label="Animated block"></div>
            </section>
          </main>
          <script>
            let count = 0;
            document.querySelector('#counter').addEventListener('click', event => {
              event.currentTarget.textContent = `Count: ${++count}`;
            });
          </script>
        </body>
        </html>
        """;

    private readonly TextEditingController _address = new(InitialAddress);
    private DorotiView? _owner;
    private WebViewController? _controller;
    private PlatformViewDescriptor? _fallback;
    private WebViewFeatures? _features;
    private WebViewResult? _state;
    private string? _unavailable;
    private string? _error;
    private string? _scriptResult;
    private string _status = "Preparing WebView";
    private bool _initialized;
    private bool _busy;
    private int _generation;
    private bool _panelSupported;
    private bool _panelVisible = true;
    private readonly ValueNotifier<bool> _draggingPanel = new(false);
    private readonly ValueNotifier<Offset> _panelOffset = new(new(24, 24));
    private Offset _panelDragOrigin;
    private Offset _pointerDragOrigin;

    internal static bool UsesNativeOverlay(BuildContext context)
    {
        var owner = View.of(context);
        if (!owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformViews)) return false;
        var host = owner.RequireCapability<IPlatformViewHostCapability>(
            DorotiCapabilityIds.PlatformViews,
            DartUiInvocation.Managed("WebViewSample.support"));
        var interleaved = Query(host, PlatformViewComposition.InterleavedComposition);
        var support = interleaved.Supported ? interleaved : Query(host, PlatformViewComposition.NativeOverlay);
        return support.Supported && support.Composition == PlatformViewComposition.NativeOverlay;
    }

    private static PlatformViewSupport Query(IPlatformViewHostCapability host, PlatformViewComposition composition) =>
        host.QuerySupport(new PlatformViewRequest(0, "doroti/webview", composition));

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (_initialized) return;
        _initialized = true;
        _owner = View.of(context);
        if (!_owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformViews))
        {
            _unavailable = "This runner does not register platform views, so the WebView sample is unavailable.";
            return;
        }

        try
        {
            var host = _owner.RequireCapability<IPlatformViewHostCapability>(
                DorotiCapabilityIds.PlatformViews,
                DartUiInvocation.Managed("WebViewSample.create"));
            var interleaved = Query(host, PlatformViewComposition.InterleavedComposition);
            var support = interleaved.Supported ? interleaved : Query(host, PlatformViewComposition.NativeOverlay);
            _panelSupported = interleaved.Supported && interleaved.NativeBackdropBlur &&
                interleaved.Capabilities?.Effect is { LiveSourceSampling: true, MaximumEffects: > 0, MaximumSigma: >= 10 };
            if (!support.Supported)
            {
                _unavailable = support.Reason ?? "This runner does not support the WebView sample.";
                return;
            }

            if (interleaved.Supported && interleaved.WebViewCommands)
            {
                // Explicit qualification option for old emulator providers which
                // cannot isolate transient profiles. Never silently weaken the default.
                var profile = Environment.GetEnvironmentVariable("DOROTI_SAMPLE_WEBVIEW_PROFILE") == "shared"
                    ? WebViewProfile.SharedPersistent : WebViewProfile.Ephemeral;
                _controller = new WebViewController(_owner, new WebViewOptions(Html: InitialHtml, Profile: profile));
                _controller.Changed += WebViewChanged;
                _ = InitializeControllerAsync(_controller);
            }
            else
            {
                _fallback = new PlatformViewDescriptor(
                    "doroti/webview",
                    Encoding.UTF8.GetBytes(InitialHtml),
                    PlatformViewStrategyPolicy.RequireRequested,
                    support.Composition);
                _status = "Interactive local HTML · basic attachment API";
            }
        }
        catch (Exception exception)
        {
            _unavailable = exception.Message;
        }
    }

    private async Task InitializeControllerAsync(WebViewController controller)
    {
        try
        {
            await controller.Ready.ConfigureAwait(false);
            var features = await controller.ExecuteAsync(new(WebViewOperation.Features)).ConfigureAwait(false);
            var state = await controller.ExecuteAsync(features.Features?.Navigation == true
                ? new(WebViewOperation.Navigate, InitialAddress)
                : new(WebViewOperation.State)).ConfigureAwait(false);
            Update(() =>
            {
                _features = features.Features;
                ApplyState(state);
                _status = state.IsLoading ? "Loading" : "WebView ready · controller API";
            });
        }
        catch (ObjectDisposedException) { }
        catch (Exception exception) { Update(() => _error = exception.Message); }
    }

    private void WebViewChanged(WebViewEvent value)
    {
        Update(() =>
        {
            _status = value.Kind switch
            {
                WebViewEventKind.Started => "Loading",
                WebViewEventKind.Committed => "Document committed",
                WebViewEventKind.Completed => "Loaded",
                WebViewEventKind.Failed => "Navigation failed",
                WebViewEventKind.ProcessFailed => "WebView process failed",
                WebViewEventKind.Message => "Message received",
                _ => value.Kind.ToString(),
            };
            _error = value.Error;
        });
        if (value.Kind is WebViewEventKind.Committed or WebViewEventKind.Completed or WebViewEventKind.Failed)
            _ = RefreshStateAsync();
    }

    private async Task RefreshStateAsync()
    {
        var controller = _controller;
        if (controller is null) return;
        try
        {
            var state = await controller.ExecuteAsync(new(WebViewOperation.State)).ConfigureAwait(false);
            Update(() => ApplyState(state));
        }
        catch (ObjectDisposedException) { }
        catch (Exception exception) { Update(() => _error = exception.Message); }
    }

    private void ApplyState(WebViewResult value)
    {
        _state = value;
        if (!string.IsNullOrWhiteSpace(value.Url) && value.Url != "about:blank") _address.text = value.Url;
    }

    private void Update(Action change)
    {
        var owner = _owner;
        if (owner is null) return;
        try { owner.DispatchPlatformEvent(() => { if (mounted) setState(change); }); }
        catch (ObjectDisposedException) { }
    }

    private async void Run(WebViewOperation operation, string? text = null)
    {
        var controller = _controller;
        if (controller is null || _busy) return;
        setState(() => { _busy = true; _error = null; _scriptResult = null; });
        try
        {
            var result = await controller.ExecuteAsync(new(operation, text)).ConfigureAwait(false);
            Update(() =>
            {
                ApplyState(result);
                if (operation == WebViewOperation.EvaluateJavaScript)
                    _scriptResult = result.IsUndefined ? "undefined" : result.Json;
                _busy = false;
            });
        }
        catch (Exception exception) { Update(() => { _error = exception.Message; _busy = false; }); }
    }

    private void Navigate()
    {
        var text = _address.text.Trim();
        if (!text.Contains("://", StringComparison.Ordinal)) text = "https://" + text;
        if (!Uri.TryCreate(text, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
        {
            setState(() => _error = "Enter an absolute HTTP or HTTPS URL.");
            return;
        }
        _address.text = uri.AbsoluteUri;
        Run(WebViewOperation.Navigate, uri.AbsoluteUri);
    }

    private void ReloadFallback() => setState(() => { _generation++; _error = null; });

    private Widget ControllerToolbar()
    {
        var navigation = _features?.Navigation == true && !_busy;
        return new Padding(padding: EdgeInsets.CreateAll(12), child: new Column(mainAxisSize: MainAxisSize.min, children:
        [
            new Row(children:
            [
                new M.IconButton(tooltip: "Back", onPressed: navigation && _state?.CanGoBack == true ? () => Run(WebViewOperation.Back) : null, icon: new Icon(M.Icons.arrow_back)),
                new M.IconButton(tooltip: "Forward", onPressed: navigation && _state?.CanGoForward == true ? () => Run(WebViewOperation.Forward) : null, icon: new Icon(M.Icons.arrow_forward)),
                new M.IconButton(tooltip: "Reload", onPressed: navigation ? () => Run(WebViewOperation.Reload) : null, icon: new Icon(M.Icons.refresh)),
                new M.IconButton(tooltip: "Local sample", onPressed: navigation ? () => Run(WebViewOperation.LoadHtml, InitialHtml) : null, icon: new Icon(M.Icons.home)),
                new Expanded(child: new M.TextField(
                    controller: _address,
                    enabled: navigation,
                    textInputAction: TextInputAction.go,
                    onSubmitted: _ => Navigate(),
                    decoration: new M.InputDecoration(
                        labelText: "Address",
                        prefixIcon: new Icon(M.Icons.language),
                        border: new M.OutlineInputBorder()))),
                new SizedBox(width: 8),
                new M.FilledButton(onPressed: navigation ? Navigate : null, child: new Text("Go")),
            ]),
            new SizedBox(height: 8),
            new Row(children:
            [
                new Expanded(child: new Text(_state?.Title is { Length: > 0 } title ? title : _status, maxLines: 1, overflow: TextOverflow.ellipsis)),
                new M.TextButton(
                    onPressed: _features?.JavaScript == true && _state?.IsLoading != true && !_busy
                        ? () => Run(WebViewOperation.EvaluateJavaScript, "document.title") : null,
                    child: new Text("Read title with JS")),
                .. _panelSupported ? new Widget[] { PanelToggle() } : [],
            ]),
        ]));
    }

    private Widget FallbackToolbar() => new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 16, vertical: 10),
        child: new Row(children:
        [
            new Icon(M.Icons.language),
            new SizedBox(width: 12),
            new Expanded(child: new Text(_status)),
            new M.TextButton(onPressed: ReloadFallback, child: new Text("Reset page")),
            .. _panelSupported ? new Widget[] { PanelToggle() } : [],
        ]));

    private void TogglePanel() => setState(() => { _panelVisible = !_panelVisible; _draggingPanel.value = false; });

    private Widget PanelToggle() => new M.TextButton(onPressed: TogglePanel,
        child: new Row(mainAxisSize: MainAxisSize.min, children:
        [
            new Icon(M.Icons.blur_on, size: 18),
            new SizedBox(width: 6),
            new Text(_panelVisible ? "Hide panel" : "Show panel"),
        ]));

    private Widget WebViewSurface(Widget webView) => new LayoutBuilder(builder: (context, constraints) =>
    {
        var width = Math.Min(320, constraints.maxWidth);
        var height = Math.Min(180, constraints.maxHeight);
        var maxX = Math.Max(0, constraints.maxWidth - width);
        var maxY = Math.Max(0, constraints.maxHeight - height);
        var pixelRatio = MediaQuery.devicePixelRatioOf(context);
        var theme = M.Theme.of(context);
        return new Stack(children:
        [
            // Keep this slot stable so toggling/moving the panel preserves the browser.
            new Positioned(left: 0, top: 0, right: 0, bottom: 0, child: new RepaintBoundary(child: webView)),
            .. _panelSupported && _panelVisible && width >= 180 && height >= 112 ? new Widget[]
            {
                // Only the transform listens to motion; keep layout, pictures and the browser stable.
                // The outer boundary must cover the travel area: a panel-sized parent
                // rejects new pointer downs after the translated child leaves its bounds.
                new Positioned(left: 0, top: 0, right: 0, bottom: 0,
                    child: new RepaintBoundary(child: new Align(alignment: Alignment.topLeft,
                    child: new ValueListenableBuilder<Offset>(valueListenable: _panelOffset,
                        builder: (_, offset, child) => Transform.CreateTranslate(
                            // Keep the panel's raster phase stable while moving; the
                            // native host can then move its cached pixels directly.
                            offset: new Offset(Math.Clamp(Math.Round(offset.dx * pixelRatio) / pixelRatio, 0, maxX),
                                Math.Clamp(Math.Round(offset.dy * pixelRatio) / pixelRatio, 0, maxY)),
                            child: child),
                    child: new SizedBox(width: width, height: height,
                    child: new RepaintBoundary(child: new PointerInterceptor(new PlatformEffect(
                        style: new(Strength: .625, Tint: theme.brightness == Brightness.dark ? 0x99211f26u : 0x99ffffffu),
                        child: new M.Material(type: M.MaterialType.transparency,
                            child: new Container(
                                decoration: new BoxDecoration(border: Border.CreateAll(color: theme.colorScheme.outlineVariant)),
                                child: new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children:
                                [
                                    new Row(children:
                                    [
                                        new Expanded(child: new ValueListenableBuilder<bool>(
                                            valueListenable: _draggingPanel,
                                            builder: (_, dragging, child) => new MouseRegion(
                                                cursor: dragging ? SystemMouseCursors.grabbing : SystemMouseCursors.grab, child: child),
                                            child: new GestureDetector(
                                                behavior: HitTestBehavior.opaque,
                                                onPanStart: details =>
                                                {
                                                    _draggingPanel.value = true;
                                                    _panelDragOrigin = new Offset(Math.Clamp(_panelOffset.value.dx, 0, maxX), Math.Clamp(_panelOffset.value.dy, 0, maxY));
                                                    _pointerDragOrigin = details.globalPosition;
                                                },
                                                onPanUpdate: details =>
                                                {
                                                    var delta = details.globalPosition - _pointerDragOrigin;
                                                    _panelOffset.value = new Offset(
                                                        Math.Clamp(_panelDragOrigin.dx + delta.dx, 0, maxX),
                                                        Math.Clamp(_panelDragOrigin.dy + delta.dy, 0, maxY));
                                                },
                                                onPanEnd: _ => _draggingPanel.value = false,
                                                onPanCancel: () => _draggingPanel.value = false,
                                                child: new SizedBox(height: 48, child: new Padding(
                                                    padding: EdgeInsets.CreateSymmetric(horizontal: 12),
                                                    child: new Row(children:
                                                    [
                                                        new Icon(M.Icons.drag_indicator, size: 20),
                                                        new SizedBox(width: 8),
                                                        new Expanded(child: new Text("Floating panel", maxLines: 1,
                                                            overflow: TextOverflow.ellipsis, style: theme.textTheme.titleSmall)),
                                                    ])))))),
                                        new M.IconButton(tooltip: "Hide panel", onPressed: TogglePanel, icon: new Icon(M.Icons.close)),
                                    ]),
                                    new M.Divider(height: 1),
                                    new Expanded(child: new SingleChildScrollView(child: new Padding(
                                        padding: EdgeInsets.CreateAll(16),
                                        child: new Column(crossAxisAlignment: CrossAxisAlignment.start, children:
                                        [
                                            new Text("Backdrop blur", style: theme.textTheme.titleMedium),
                                            new SizedBox(height: 8),
                                            new Text("Drag the header to move this panel. The page remains interactive outside it."),
                                        ])))),
                                ])))))))))))
            } : [],
        ]);
    });

    public override Widget build(BuildContext context)
    {
        if (_unavailable is { } unavailable)
            return new Center(child: new Padding(padding: EdgeInsets.CreateAll(32), child: new Column(
                mainAxisSize: MainAxisSize.min,
                children: [new Icon(M.Icons.web_asset_outlined, size: 48), new SizedBox(height: 16), new Text("WebView unavailable"), new SizedBox(height: 8), new Text(unavailable)])));

        var webView = _controller is { } controller
            ? (Widget)new WebViewWidget(controller)
            : _fallback is { } fallback && _owner is { } owner
                ? new PlatformView(owner, fallback, key: new ValueKey<int>(_generation))
                : new Center(child: new M.CircularProgressIndicator());
        // Toolbar/ink animations must not invalidate the surrounding app bar and
        // navigation raster while a native browser is interleaved with this page.
        return new RepaintBoundary(child: new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children:
        [
            new ClipRect(child: new RepaintBoundary(child: _controller is null ? FallbackToolbar() : ControllerToolbar())),
            .. _busy ? new Widget[] { new M.LinearProgressIndicator() } : [],
            .. _error is { } error ? new Widget[] { new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 16, vertical: 6), child: new Text(error)) } : [],
            .. _scriptResult is { } result ? new Widget[] { new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 16, vertical: 6), child: new Text($"JavaScript result: {result}")) } : [],
            new Expanded(child: new Padding(padding: EdgeInsets.CreateFromLTRB(12, 0, 12, 12), child: new ClipRect(child: WebViewSurface(webView)))),
        ]));
    }

    public override void dispose()
    {
        if (_controller is { } controller)
        {
            controller.Changed -= WebViewChanged;
            _ = controller.DisposeAsync();
        }
        _panelOffset.dispose();
        _draggingPanel.dispose();
        _address.dispose();
        base.dispose();
    }
}
