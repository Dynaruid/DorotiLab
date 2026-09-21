using System.Text;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

public static class PlatformEffectFixtureProbe
{
    public static DorotiView? Owner { get; internal set; }
    public static WebViewController? WebView { get; internal set; }
    public static Action<int>? SetStage { get; internal set; }
    public static Action<bool>? SetRasterSource { get; internal set; }
    public static Action<PlatformEffectStyle>? SetStyle { get; internal set; }
    public static Action<double>? SetStrength { get; internal set; }
}

internal sealed class PlatformEffectFixture : StatefulWidget
{
    public override IState createState() => new FixtureState();

    private sealed class FixtureState : State<PlatformEffectFixture>
    {
        private bool _blur = true,
            _block;
        private int _taps;
        private double _strength = .75;
        private PlatformEffectStyle? _customStyle;
        private bool _rasterSource;
        private bool _mounted = true,
            _second,
            _moved;
        private int _generation;
        private WebViewController? _primaryController,
            _secondaryController;
        private static readonly byte[] Html = Encoding.UTF8.GetBytes(
            """
            <!doctype html><meta charset="utf-8"><style>
            body{margin:0;font:22px sans-serif;background:repeating-conic-gradient(#eee 0% 25%,#999 0% 50%) 0/40px 40px}
            input,button{font:22px sans-serif;margin:20px;padding:10px}#moving{background:#1877cc;color:white;width:150px;padding:20px;animation:move 3s infinite alternate}
            @keyframes move{to{transform:translateX(220px)}}p{margin:20px}</style>
            <input value="Native WebView2 text"><button style="position:absolute;left:360px;top:80px;z-index:2;margin:0;width:80px;height:55px" onclick="this.textContent=Number(this.textContent)+1;window.chrome?.webview?.postMessage(this.textContent)">0</button>
            <div id="moving">Live animation</div><p>Native backdrop source ABCDE 12345</p><div style="height:900px">Scroll native content</div>
            """
        );
        private static readonly byte[] SecondHtml = Encoding.UTF8.GetBytes(
            Encoding
                .UTF8.GetString(Html)
                .Replace("#eee", "#ffeeaa")
                .Replace("#999", "#dd9944")
                .Replace("Native WebView2 text", "Second native WebView")
        );

        public override void dispose()
        {
            PlatformEffectFixtureProbe.Owner = null;
            PlatformEffectFixtureProbe.WebView = null;
            PlatformEffectFixtureProbe.SetStage = null;
            PlatformEffectFixtureProbe.SetStrength = null;
            PlatformEffectFixtureProbe.SetStyle = null;
            PlatformEffectFixtureProbe.SetRasterSource = null;
            if (_primaryController is { } primary)
            {
                _ = primary.DisposeAsync();
            }

            if (_secondaryController is { } secondary)
            {
                _ = secondary.DisposeAsync();
            }

            base.dispose();
        }

        public override Widget build(BuildContext context)
        {
            if (
                !string.IsNullOrEmpty(
                    Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE")
                )
            )
            {
                Environment.SetEnvironmentVariable(
                    "DOROTI_PLATFORM_EFFECT_PROBE_STATE",
                    $"{_blur},{_block},{_taps}"
                );
            }

            PlatformEffectFixtureProbe.SetStrength = strength =>
                setState(() =>
                {
                    _customStyle = null;
                    _strength = strength;
                });
            PlatformEffectFixtureProbe.SetRasterSource = value =>
                setState(() => _rasterSource = value);
            PlatformEffectFixtureProbe.SetStyle = style => setState(() => _customStyle = style);
            PlatformEffectFixtureProbe.SetStage = stage =>
                setState(() =>
                {
                    _blur = stage != 1;
                    _block = stage == 2;
                    _second = stage is 3 or 4;
                    _moved = stage == 4;
                    var mounted = stage != 5;
                    if (mounted && !_mounted)
                    {
                        _generation++;
                    }

                    _mounted = mounted;
                });
            var owner = View.of(context);
            PlatformEffectFixtureProbe.Owner = owner;
            var host = owner.RequireCapability<IPlatformViewHostCapability>(
                DorotiCapabilityIds.PlatformViews,
                DartUiInvocation.Managed("PlatformEffectFixture")
            );
            if (
                OperatingSystem.IsBrowser()
                || OperatingSystem.IsMacOS()
                || OperatingSystem.IsWindows()
                || OperatingSystem.IsAndroid()
                || (
                    OperatingSystem.IsLinux()
                    && host.QuerySupport(
                        new(0, "doroti/webview", PlatformViewComposition.InterleavedComposition)
                    ).WebViewCommands
                )
            )
            {
                if (_mounted)
                {
                    _primaryController ??= new(
                        owner,
                        new(
                            Html: Encoding.UTF8.GetString(Html),
                            Profile: OperatingSystem.IsBrowser()
                                ? WebViewProfile.BrowserDefault
                                : WebViewProfile.Ephemeral
                        )
                    );
                }
                else if (_primaryController is { } primary)
                {
                    _primaryController = null;
                    _ = primary.DisposeAsync();
                }
                if (_mounted && _second)
                {
                    _secondaryController ??= new(
                        owner,
                        new(
                            Html: Encoding.UTF8.GetString(SecondHtml),
                            Profile: OperatingSystem.IsBrowser()
                                ? WebViewProfile.BrowserDefault
                                : WebViewProfile.Ephemeral
                        )
                    );
                }
                else if (_secondaryController is { } secondary)
                {
                    _secondaryController = null;
                    _ = secondary.DisposeAsync();
                }
                if (OperatingSystem.IsWindows() && _primaryController is { } windowsWeb)
                {
                    WindowsWebViewEvidence.Start(owner, windowsWeb);
                    WindowsEffectCalibration.Start(owner, windowsWeb);
                }
                if (OperatingSystem.IsLinux() && _primaryController is { } qtWeb)
                {
                    QtWebViewEvidence.Start(owner, qtWeb);
                    WindowsEffectCalibration.Start(owner, qtWeb, "DOROTI_QT_EFFECT_CALIBRATION");
                }
                if (OperatingSystem.IsAndroid() && _primaryController is { } androidWeb)
                {
                    AndroidWebViewEvidence.Start(owner, androidWeb);
                    WindowsEffectCalibration.Start(
                        owner,
                        androidWeb,
                        "DOROTI_ANDROID_EFFECT_CALIBRATION"
                    );
                }
            }
            PlatformEffectFixtureProbe.WebView = _primaryController;
            var request = new PlatformViewRequest(
                0,
                "doroti/webview",
                PlatformViewComposition.InterleavedComposition,
                CreationParameters: Html
            );
            var descriptor = new PlatformViewDescriptor(
                "doroti/webview",
                Html,
                PlatformViewStrategyPolicy.RequireRequested
            );
            var support = host.QuerySupport(request);
            if (!support.Supported)
            {
                return new M.Scaffold(
                    body: new Center(
                        child: new Text(support.Reason ?? "WebView attachment unavailable")
                    )
                );
            }

            return new M.Scaffold(
                appBar: new M.AppBar(title: new Text("Native WebView + PlatformEffect")),
                body: new Column(
                    children:
                    [
                        new Wrap(
                            children:
                            [
                                new M.TextButton(
                                    onPressed: () => setState(() => _blur = !_blur),
                                    child: new Text(_blur ? "Disable blur" : "Enable blur")
                                ),
                                new M.TextButton(
                                    onPressed: () => setState(() => _block = !_block),
                                    child: new Text(_block ? "Block input" : "Pass through input")
                                ),
                                new Text($"Foreground taps: {_taps}"),
                            ]
                        ),
                        new SizedBox(
                            width: 620,
                            height: 400,
                            child: new Stack(
                                children:
                                [
                                    .. _mounted
                                        ? new Widget[]
                                        {
                                            new Positioned(
                                                left: 0,
                                                top: 0,
                                                width: 620,
                                                height: 400,
                                                child: _primaryController is { } primaryWeb
                                                    ? new WebViewWidget(
                                                        primaryWeb,
                                                        key: new Doroti.Framework.Foundation.ValueKey<int>(
                                                            _generation
                                                        )
                                                    )
                                                    : new PlatformView(
                                                        owner,
                                                        descriptor,
                                                        key: new Doroti.Framework.Foundation.ValueKey<int>(
                                                            _generation
                                                        )
                                                    )
                                            ),
                                        }
                                        : [],
                                    .. _mounted && _second
                                        ? new Widget[]
                                        {
                                            new Positioned(
                                                left: 70,
                                                top: 45,
                                                width: 180,
                                                height: 160,
                                                child: new Container(color: new Color(0xff00aa55))
                                            ),
                                            new Positioned(
                                                left: 210,
                                                top: 100,
                                                width: 310,
                                                height: 210,
                                                child: _secondaryController is { } secondWeb
                                                    ? new WebViewWidget(
                                                        secondWeb,
                                                        key: new Doroti.Framework.Foundation.ValueKey<int>(
                                                            -_generation - 1
                                                        )
                                                    )
                                                    : new PlatformView(
                                                        owner,
                                                        descriptor with
                                                        {
                                                            CreationParameters = SecondHtml,
                                                        },
                                                        key: new Doroti.Framework.Foundation.ValueKey<int>(
                                                            -_generation - 1
                                                        )
                                                    )
                                            ),
                                        }
                                        : [],
                                    .. _rasterSource
                                        ? new Widget[]
                                        {
                                            new Positioned(
                                                left: 0,
                                                top: 0,
                                                width: 620,
                                                height: 400,
                                                child: new Row(
                                                    children:
                                                    [
                                                        new Expanded(
                                                            child: new Container(
                                                                color: new Color(0xff000000)
                                                            )
                                                        ),
                                                        new Expanded(
                                                            child: new Container(
                                                                color: new Color(0xffffffff)
                                                            )
                                                        ),
                                                    ]
                                                )
                                            ),
                                        }
                                        : [],
                                    new Positioned(
                                        left: _moved ? 180 : 130,
                                        top: _moved ? 100 : 65,
                                        width: 320,
                                        height: 210,
                                        child: new PointerInterceptor(
                                            new PlatformEffect(
                                                style: _blur
                                                    ? _customStyle
                                                        ?? new(
                                                            Strength: _strength,
                                                            Tint: 0x33ffffff
                                                        )
                                                    : new(Strength: 0, Tint: 0x33ffffff),
                                                child: new Center(
                                                    child: new Text("Sharp Doroti foreground")
                                                )
                                            ),
                                            intercepting: _block
                                        )
                                    ),
                                    new Positioned(
                                        left: 190,
                                        top: 285,
                                        width: 240,
                                        height: 60,
                                        child: new PointerInterceptor(
                                            new M.ElevatedButton(
                                                onPressed: () => setState(() => _taps++),
                                                child: new Text("Doroti button")
                                            )
                                        )
                                    ),
                                ]
                            )
                        ),
                        new M.TextField(
                            decoration: new M.InputDecoration(labelText: "Doroti focus return")
                        ),
                        new Wrap(
                            children:
                            [
                                new M.TextButton(
                                    onPressed: () => setState(() => _second = !_second),
                                    child: new Text("Second WebView")
                                ),
                                new M.TextButton(
                                    onPressed: () => setState(() => _moved = !_moved),
                                    child: new Text("Move effect")
                                ),
                                new M.TextButton(
                                    onPressed: () =>
                                        setState(() =>
                                        {
                                            _mounted = !_mounted;
                                            if (_mounted)
                                            {
                                                _generation++;
                                            }
                                        }),
                                    child: new Text(
                                        _mounted ? "Dispose WebViews" : "Create WebViews"
                                    )
                                ),
                            ]
                        ),
                        .. OperatingSystem.IsMacOS()
                            ? new Widget[]
                            {
                                new Wrap(
                                    children:
                                    [
                                        new Text(
                                            $"Blur radius: {_customStyle?.Sigma ?? (_strength * 16):0.0}"
                                        ),
                                        new SizedBox(
                                            width: 200,
                                            child: new M.Slider(
                                                value: _customStyle?.Sigma ?? (_strength * 16),
                                                max: 64,
                                                onChanged: radius =>
                                                    setState(() =>
                                                        _customStyle = (
                                                            _customStyle
                                                            ?? new(
                                                                Strength: _strength,
                                                                Tint: 0x33ffffff
                                                            )
                                                        ) with
                                                        {
                                                            Match =
                                                                PlatformEffectMatchPolicy.ExactSigma,
                                                            ExactSigma = radius,
                                                        }
                                                    )
                                            )
                                        ),
                                        new Text(
                                            $"Saturation: {_customStyle?.Saturation ?? 1:0.0}"
                                        ),
                                        new SizedBox(
                                            width: 160,
                                            child: new M.Slider(
                                                value: _customStyle?.Saturation ?? 1,
                                                max: 2,
                                                onChanged: saturation =>
                                                    setState(() =>
                                                        _customStyle = (
                                                            _customStyle
                                                            ?? new(
                                                                Strength: _strength,
                                                                Tint: 0x33ffffff
                                                            )
                                                        ) with
                                                        {
                                                            Saturation = saturation,
                                                        }
                                                    )
                                            )
                                        ),
                                        new Text(
                                            $"Tint opacity: {((_customStyle?.Tint ?? 0x33ffffff) >> 24) / 255.0:0.00}"
                                        ),
                                        new SizedBox(
                                            width: 150,
                                            child: new M.Slider(
                                                value: ((_customStyle?.Tint ?? 0x33ffffff) >> 24)
                                                    / 255.0,
                                                onChanged: opacity =>
                                                    setState(() =>
                                                    {
                                                        var style =
                                                            _customStyle
                                                            ?? new PlatformEffectStyle(
                                                                Strength: _strength,
                                                                Tint: 0x33ffffff
                                                            );
                                                        _customStyle = style with
                                                        {
                                                            Tint =
                                                                (style.Tint & 0x00ffffff)
                                                                | (
                                                                    (uint)Math.Round(opacity * 255)
                                                                    << 24
                                                                ),
                                                        };
                                                    })
                                            )
                                        ),
                                        new M.TextButton(
                                            onPressed: () =>
                                                setState(() =>
                                                    _customStyle = (
                                                        _customStyle ?? new(Strength: _strength)
                                                    ) with
                                                    {
                                                        Tint = 0,
                                                    }
                                                ),
                                            child: new Text("Clear tint")
                                        ),
                                        new M.TextButton(
                                            onPressed: () =>
                                                setState(() =>
                                                    _customStyle = (
                                                        _customStyle ?? new(Strength: _strength)
                                                    ) with
                                                    {
                                                        Tint = 0x443c82f6,
                                                    }
                                                ),
                                            child: new Text("Blue tint")
                                        ),
                                    ]
                                ),
                            }
                            : [],
                    ]
                )
            );
        }
    }
}
