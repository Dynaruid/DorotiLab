#if MACOS
using AppKit;
using CoreGraphics;
using Doroti.Framework.Services;
using Doroti.Host.Maui;
using Doroti.Ui;
using Foundation;
using WebKit;

namespace DorotiTestbedApp.MacOS;

internal static class MacOSWebViewEvidence
{
    private static readonly AppKitPlatformViewDispatcher Dispatcher = new();

    private static Task OnUi(Action action) =>
        Dispatcher
            .InvokeAsync(() =>
            {
                action();
                return ValueTask.CompletedTask;
            })
            .AsTask();

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static IEnumerable<NSView> Descendants(NSView? root)
    {
        if (root is null)
            yield break;
        yield return root;
        foreach (var child in root.Subviews)
        foreach (var node in Descendants(child))
            yield return node;
    }

    private static NSView[] Views() =>
        Microsoft
            .Maui.Controls.Application.Current!.Windows.Select(window =>
                window.Handler?.PlatformView
            )
            .OfType<NSWindow>()
            .SelectMany(window => Descendants(window.ContentView))
            .ToArray();

    private static WKWebView[] WebViews() =>
        Views()
            .OfType<WKWebView>()
            .Where(view =>
                view.Identifier?.StartsWith("doroti-platform-view-") == true
                && view.Superview?.Hidden == false
            )
            .ToArray();

    private static async Task Until(Func<bool> test)
    {
        var deadline = DateTime.UtcNow.AddSeconds(25);
        while (DateTime.UtcNow < deadline)
        {
            var done = false;
            await OnUi(() => done = test());
            if (done)
                return;
            await Task.Delay(100);
        }
        throw new TimeoutException("AppKit product condition did not become ready.");
    }

    private static PlatformViewHandle Handle(WKWebView view)
    {
        var parts = view.Identifier!.Split('-');
        return new(ulong.Parse(parts[^3]), long.Parse(parts[^2]), long.Parse(parts[^1]));
    }

    internal static async Task CaptureAsync(string path)
    {
        var lines = new List<string>
        {
            $"renderer={(Environment.GetEnvironmentVariable("DOROTI_MACOS_GRAPHITE") == "0" ? "Ganesh" : "Graphite")}-Metal",
            $"OS={Environment.OSVersion}",
            "strategy=WKWebView + public Core Image Gaussian/color backdrop + sibling Metal foreground",
            "physicalInput=notVerified",
            "visualMatch=notVerified",
            "nativeAot=notVerified",
        };
        try
        {
            WKWebView? web = null;
            await Until(() =>
            {
                web = WebViews().SingleOrDefault();
                return web is { IsLoading: false };
            });
            var owner = PlatformEffectFixtureProbe.Owner!;
            var host = owner.RequireCapability<IPlatformViewHostCapability>(
                DorotiCapabilityIds.PlatformViews,
                DartUiInvocation.Managed("AppKitProbe")
            );
            var commands = (IWebViewHostCapability)host;
            var handle = Handle(web!);
            Task<WebViewResult> Command(
                WebViewOperation op,
                string? text = null,
                long generation = 0
            ) => commands.ExecuteWebViewAsync(handle, new(op, text, generation));
            Task<WebViewResult> Js(string script) =>
                Command(WebViewOperation.EvaluateJavaScript, script);
            var features = (await Command(WebViewOperation.Features)).Features!;
            Check(
                features.JavaScript
                    && features.EphemeralProfile
                    && !features.ScriptMessages
                    && !features.AppContentScheme,
                "feature contract"
            );
            Check(
                (await Js("document.querySelector('input').value='preserved-web-state'")).Json
                    == "\"preserved-web-state\"",
                "initial HTML/JS"
            );
            var position = (
                await Js("document.querySelector('#moving').getBoundingClientRect().x")
            ).Json;
            var animationDeadline = DateTime.UtcNow.AddSeconds(8);
            var animationChanged = false;
            while (!animationChanged && DateTime.UtcNow < animationDeadline)
            {
                await Task.Delay(200);
                animationChanged =
                    (await Js("document.querySelector('#moving').getBoundingClientRect().x")).Json
                    != position;
            }
            Check(animationChanged, "live DOM animation after first visible WebKit frame");
            await Js("scrollTo(0,100)");
            Check(double.Parse((await Js("scrollY")).Json!) > 0, "native scrolling");
            await Js(
                "scrollTo(0,0);document.querySelector('#moving').style.animationPlayState='paused'"
            );
            lines.Add(
                "PASS public command adapter / feature query / initial HTML / live animation / scroll"
            );
            foreach (var strength in new[] { 0.0, .25, .375, .75, 1.0, .375 })
            {
                await OnUi(() => PlatformEffectFixtureProbe.SetStrength!(strength));
                await Task.Delay(450);
                await Capture(
                    path,
                    $"strength-{strength.ToString(System.Globalization.CultureInfo.InvariantCulture)}-{lines.Count}"
                );
                lines.Add($"PASS strength={strength} capture");
            }
            await OnUi(() => PlatformEffectFixtureProbe.SetStrength!(.75));
            foreach (var appearance in new[] { NSAppearance.NameDarkAqua, NSAppearance.NameAqua })
            {
                await OnUi(() => web!.Window!.Appearance = NSAppearance.GetAppearance(appearance));
                await Task.Delay(400);
                await Capture(
                    path,
                    appearance == NSAppearance.NameAqua ? "theme-light" : "theme-dark"
                );
            }
            await Js("document.querySelector('#moving').style.animationPlayState='running'");
            await Capture(path, "live-a");
            await Task.Delay(700);
            await Capture(path, "live-b");
            await Js("document.querySelector('#moving').style.animationPlayState='paused'");
            for (var stage = 0; stage < 7; stage++)
            {
                var current = stage;
                await OnUi(() => PlatformEffectFixtureProbe.SetStage!(current));
                await Task.Delay(800);
                await OnUi(() =>
                {
                    var webs = WebViews();
                    Check(
                        webs.Length
                            == (
                                current == 5 ? 0
                                : current is 3 or 4 ? 2
                                : 1
                            ),
                        $"stage {current} WebView count"
                    );
                    if (current < 5)
                        Check(webs.Contains(web!), "identity preservation");
                    if (current == 5)
                        return;
                    var overlay = webs[0].Superview!.Superview!;
                    var effects = overlay
                        .Subviews.Where(view =>
                            view.Identifier == "doroti-platform-effect" && !view.Hidden
                        )
                        .ToArray();
                    Check(
                        effects.Length == (current == 1 ? 0 : 1),
                        $"stage {current} material count"
                    );
                    Check(
                        effects.All(view => view.AlphaValue == 1),
                        "full-opacity backdrop filter"
                    );
                    var point = new CGPoint(
                        webs[0].Superview!.Frame.X + 150,
                        webs[0].Superview!.Frame.Y + 90
                    );
                    var hit = overlay.HitTest(overlay.ConvertPointToView(point, overlay.Superview));
                    Check(
                        current == 2
                            ? hit is DorotiMacOSMetalView
                            : hit is not null && hit is not DorotiMacOSMetalView,
                        $"stage {current} shield/pass-through hit={hit?.GetType().Name}"
                    );
                    Check(
                        effects.All(view => view.HitTest(CGPoint.Empty) is null),
                        "effect input transparency"
                    );
                });
                if (current < 5)
                    Check(
                        (await Js("document.querySelector('input').value")).Json
                            == "\"preserved-web-state\"",
                        "editing state"
                    );
                await Capture(path, $"stage-{stage}");
                lines.Add($"PASS stage={stage} identity / input routing / effect lifecycle");
            }
            await Until(() =>
            {
                web = WebViews().SingleOrDefault();
                return web is { IsLoading: false };
            });
            handle = Handle(web!);
            if (Environment.GetEnvironmentVariable("DOROTI_MACOS_CUSTOM_BLUR_PROBE") == "1")
            {
                await Js(
                    "window.calibration=document.createElement('div');calibration.style.cssText='position:fixed;inset:0;z-index:9999;pointer-events:none;background:linear-gradient(to right,#000 50%,#fff 50%)';document.body.appendChild(calibration)"
                );
                foreach (var sigma in new[] { 0.0, 2, 4, 8, 16, 32, 64 })
                {
                    await ApplyStyle(
                        new(Match: PlatformEffectMatchPolicy.ExactSigma, ExactSigma: sigma, Tint: 0)
                    );
                    await Task.Delay(450);
                    await Capture(path, $"edge-{sigma}");
                }
                await OnUi(() => PlatformEffectFixtureProbe.SetRasterSource!(true));
                foreach (var sigma in new[] { 0.0, 4, 16 })
                {
                    await ApplyStyle(
                        new(Match: PlatformEffectMatchPolicy.ExactSigma, ExactSigma: sigma, Tint: 0)
                    );
                    await Task.Delay(450);
                    await Capture(path, $"raster-edge-{sigma}");
                }
                await OnUi(() => PlatformEffectFixtureProbe.SetRasterSource!(false));
                await Js(
                    "calibration.style.background='linear-gradient(to right,rgb(220,70,50) 50%,rgb(30,110,220) 50%)'"
                );
                foreach (var saturation in new[] { 0.0, 1, 2 })
                {
                    await ApplyStyle(
                        new(
                            Match: PlatformEffectMatchPolicy.ExactSigma,
                            ExactSigma: 8,
                            Saturation: saturation,
                            Tint: 0
                        )
                    );
                    await Task.Delay(450);
                    await Capture(path, $"saturation-{saturation}");
                }
                await ApplyStyle(
                    new(
                        Match: PlatformEffectMatchPolicy.ExactSigma,
                        ExactSigma: 0,
                        Saturation: 0,
                        Tint: 0
                    )
                );
                await Task.Delay(450);
                await Capture(path, "saturation-only");
                await ApplyStyle(
                    new(
                        Match: PlatformEffectMatchPolicy.ExactSigma,
                        ExactSigma: 8,
                        Tint: 0x6600ff00
                    )
                );
                await Task.Delay(450);
                await Capture(path, "tint-green");
                await ApplyStyle(
                    new(Match: PlatformEffectMatchPolicy.ExactSigma, ExactSigma: 8, Tint: 0)
                );
                NSView? referenceTint = null;
                try
                {
                    await OnUi(() =>
                    {
                        var effect = Views().OfType<AppKitPlatformBlurView>().Single();
                        referenceTint = new NSView(effect.Frame) { WantsLayer = true };
                        using var color = NSColor.FromSrgb(0, 1, 0, .4f);
                        referenceTint.Layer!.BackgroundColor = color.CGColor;
                        referenceTint.Layer.ZPosition = 1000;
                        effect.Superview!.AddSubview(referenceTint);
                    });
                    await Task.Delay(450);
                    await Capture(path, "tint-native-reference");
                }
                finally
                {
                    await OnUi(() =>
                    {
                        referenceTint?.RemoveFromSuperview();
                        referenceTint?.Dispose();
                    });
                }
                await Js("calibration.remove()");
                await OnUi(() => PlatformEffectFixtureProbe.SetStrength!(.75));
                lines.Add("CAPTURE edge-spread calibration; numerical verification separate");
            }
            Check((await Js("undefined")).IsUndefined, "undefined result");
            Check((await Js("null")).Json == "null", "null result");
            Check(
                (await Js("({a:[true,1,'한글']})")).Json == "{\"a\":[true,1,\"한글\"]}",
                "JSON result"
            );
            await Reject(() => Js("throw Error('probe')"), WebViewError.JavaScript);
            await Reject(() => Js("Promise.resolve(1)"), WebViewError.JavaScript);
            await Reject(() => Js("let a={};a.self=a;a"), WebViewError.JavaScript);
            await Reject(
                () => Command(WebViewOperation.Navigate, "file:///etc/passwd"),
                WebViewError.InvalidRequest
            );
            var prior = await Command(WebViewOperation.State);
            await Command(
                WebViewOperation.LoadHtml,
                "<!doctype html><title>second document</title><input value='second'>"
            );
            await Until(() => web is { IsLoading: false } && web.Title == "second document");
            await Reject(
                () =>
                    commands.ExecuteWebViewAsync(
                        handle,
                        new(WebViewOperation.EvaluateJavaScript, "1", prior.DocumentGeneration)
                    ),
                WebViewError.NavigationChanged
            );
            var pendingJs = Js(
                "(()=>{const until=Date.now()+300;while(Date.now()<until){};return 'old-document'})()"
            );
            await Command(
                WebViewOperation.LoadHtml,
                "<!doctype html><title>second document</title><input value='replacement'>"
            );
            await Reject(() => pendingJs, WebViewError.NavigationChanged);
            await Until(() => web is { IsLoading: false } && web.Title == "second document");
            await Command(WebViewOperation.ClearData);
            lines.Add(
                "PASS JS null/undefined/JSON/error/cycle/Promise rejection / file policy / document generation / profile clear"
            );
            await using (
                var content = new WebViewController(
                    owner,
                    new(
                        AllowedOrigins: [],
                        Resources: new()
                        {
                            ["/index.html"] = new("webview/index.html", "text/html"),
                            ["/style.css"] = new("webview/style.css", "text/css"),
                        },
                        MessageOrigins: ["doroti-app://content"]
                    )
                )
            )
            {
                await content.Ready;
                var messages = new List<WebViewEvent>();
                content.Changed += value =>
                {
                    if (value.Kind == WebViewEventKind.Message)
                        lock (messages)
                            messages.Add(value);
                };
                var available = (
                    await content.ExecuteAsync(new(WebViewOperation.Features))
                ).Features!;
                Check(
                    available.AppContentScheme && available.ScriptMessages,
                    "content/message features"
                );
                await content.ExecuteAsync(
                    new(WebViewOperation.Navigate, "doroti-app://content/index.html")
                );
                var loaded = false;
                for (var attempt = 0; attempt < 50; attempt++)
                {
                    var state = await content.ExecuteAsync(new(WebViewOperation.State));
                    if (!state.IsLoading && state.Title == "App content")
                    {
                        loaded = true;
                        break;
                    }
                    await Task.Delay(100);
                }
                Check(loaded, "manifest HTML resource");
                Check(
                    (
                        await content.ExecuteAsync(
                            new(
                                WebViewOperation.EvaluateJavaScript,
                                "getComputedStyle(document.querySelector('#content')).color"
                            )
                        )
                    ).Json == "\"rgb(12, 34, 56)\"",
                    "relative CSS/MIME resource"
                );
                await content.ExecuteAsync(
                    new(
                        WebViewOperation.EvaluateJavaScript,
                        "doroti.postMessage('probe',{value:'한글'})"
                    )
                );
                for (var attempt = 0; attempt < 20; attempt++)
                {
                    lock (messages)
                        if (messages.Count > 0)
                            break;
                    await Task.Delay(100);
                }
                lock (messages)
                    Check(
                        messages.Count == 1
                            && messages[0].MessageName == "probe"
                            && messages[0].MessageJson!.Contains("한글"),
                        "trusted main-frame message"
                    );
                await content.ExecuteAsync(
                    new(WebViewOperation.LoadHtml, "<title>untrusted</title>")
                );
                for (var attempt = 0; attempt < 50; attempt++)
                {
                    var state = await content.ExecuteAsync(new(WebViewOperation.State));
                    if (!state.IsLoading && state.Title == "untrusted")
                        break;
                    await Task.Delay(100);
                }
                await content.ExecuteAsync(
                    new(
                        WebViewOperation.EvaluateJavaScript,
                        "doroti.postMessage('forged',{origin:'doroti-app://content'})"
                    )
                );
                await Task.Delay(200);
                lock (messages)
                    Check(messages.Count == 1, "payload origin cannot forge native origin");
            }
            lines.Add(
                "PASS manifest HTML / relative CSS / MIME / trusted main-frame bridge / forged-origin rejection / handler disposal"
            );
            await using (
                var controller = new WebViewController(
                    owner,
                    new(Html: "<title>controller</title>", AllowedOrigins: [])
                )
            )
            {
                await controller.Ready;
                Check(
                    (await controller.ExecuteAsync(new(WebViewOperation.Features)))
                        .Features!
                        .Navigation,
                    "public controller"
                );
                await Reject(
                    () =>
                        controller.ExecuteAsync(
                            new(WebViewOperation.Navigate, "https://example.com")
                        ),
                    WebViewError.InvalidRequest
                );
                await controller.DisposeAsync();
                await Reject(
                    () => controller.ExecuteAsync(new(WebViewOperation.State)),
                    WebViewError.Closed
                );
            }
            lines.Add(
                "PASS public controller creation / origin policy / disposal / closed command"
            );
            await OnUi(() => web!.Window!.SetContentSize(new CGSize(900, 700)));
            await Task.Delay(600);
            Check((await Js("document.title")).Json == "\"second document\"", "resize identity");
            await Capture(path, "resized");
            lines.Add("PASS resize preserves native document");
            lines.Add("RESULT=PASS");
        }
        catch (Exception error)
        {
            lines.Add("RESULT=FAIL\n" + error);
        }
        File.WriteAllLines(path, lines);
    }

    private static async Task ApplyStyle(PlatformEffectStyle style)
    {
        await OnUi(() => PlatformEffectFixtureProbe.SetStyle!(style));
        await Until(() =>
        {
            var effect = Views().OfType<AppKitPlatformBlurView>().SingleOrDefault();
            return style.Sigma == 0 && style.Saturation == 1
                ? effect is null || effect.Hidden
                : effect is { Hidden: false } && effect.AppliedStyle == style;
        });
    }

    private static async Task Reject(Func<Task<WebViewResult>> action, WebViewError expected)
    {
        try
        {
            await action();
        }
        catch (WebViewException error) when (error.Code == expected)
        {
            return;
        }
        throw new InvalidOperationException($"Expected WebView error {expected}.");
    }

    private static async Task Capture(string path, string name)
    {
        if (Environment.GetEnvironmentVariable("DOROTI_MACOS_CAPTURE") != "1")
            return;
        await OnUi(() =>
        {
            var window = Views().First(view => view.Window is not null).Window!;
            var effect = Views()
                .FirstOrDefault(view => view.Identifier == "doroti-platform-effect");
            var bounds = effect?.ConvertRectToView(effect.Bounds, null) ?? CGRect.Empty;
            var metadata = FormattableString.Invariant(
                $"{name}\n{window.WindowNumber}\n{bounds.X},{window.Frame.Height - bounds.Y - bounds.Height},{bounds.Width},{bounds.Height}\n{window.BackingScaleFactor}"
            );
            File.WriteAllText(path + ".capture", metadata);
        });
        var deadline = DateTime.UtcNow.AddSeconds(20);
        while (File.Exists(path + ".capture") && DateTime.UtcNow < deadline)
            await Task.Delay(50);
        Check(!File.Exists(path + ".capture"), "Screenshot driver did not acknowledge capture.");
    }
}
#endif
