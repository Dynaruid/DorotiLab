using CoreGraphics;
using Doroti.Host.Maui;
using Foundation;
using UIKit;
using WebKit;

namespace DorotiTestbedApp.iOS;

// Explicit opt-in, runs the actual widget -> scene -> Metal/native path. No physical touch claim.
internal static class PlatformViewEvidence
{
    private static readonly UIKitPlatformViewDispatcher Dispatcher = new();

    internal static async Task CaptureAsync()
    {
        if (Environment.GetEnvironmentVariable("DOROTI_UIKIT_EVIDENCE") != "1")
            return;
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Path.GetFileName(
                Environment.GetEnvironmentVariable("DOROTI_UIKIT_EVIDENCE_NAME")
                    ?? "platform-views-evidence.txt"
            )
        );
        if (Environment.GetEnvironmentVariable("DOROTI_TESTBED_WEBVIEW_PAGE_PROBE") == "1")
        {
            await CaptureWebViewSample(path);
            return;
        }
        if (Environment.GetEnvironmentVariable("DOROTI_TESTBED_MODE") == "platform-effects")
        {
            await CaptureEffects(path);
            return;
        }
        var lines = new List<string>
        {
            $"UTC={DateTime.UtcNow:O}",
            $"OS={UIDevice.CurrentDevice.SystemVersion}",
            $"GPU={Metal.MTLDevice.SystemDefault?.Name}",
            "renderer=Graphite-Metal",
            "input=programmatic UIKit hit-test/control actions; physical=notVerified",
        };
        try
        {
            UIButton? button = null;
            UITextField? editor = null;
            await Until(() =>
            {
                button = Controls().OfType<UIButton>().SingleOrDefault();
                editor = Controls().OfType<UITextField>().SingleOrDefault();
                return button is not null && editor is not null;
            });
            var identity = button!.Handle;
            var editorIdentity = editor!.Handle;
            await OnUi(() =>
            {
                button.SendActionForControlEvents(UIControlEvent.TouchUpInside);
                Check(
                    button.Title(UIControlState.Normal) == "Native clicks: 1",
                    "native activation exactly once"
                );
                editor.Text = "preserved-native-state";
            });
            for (var stage = 0; stage < 10; stage++)
            {
                var current = stage;
                await OnUi(() => PlatformViewFixtureProbe.SetStage!(current));
                await Task.Delay(700);
                await OnUi(() =>
                {
                    Check(
                        button.Handle == identity
                            && editor.Handle == editorIdentity
                            && editor.Text == "preserved-native-state",
                        "identity/state"
                    );
                    var overlay = button.Superview!.Superview!;
                    var origin = button.Superview.Frame.Location;
                    var point = new CGPoint(origin.X + 215, origin.Y + 90);
                    var hit = overlay.HitTest(point, null);
                    var shield = current is 1 or 2 or 5 or 6 or 9;
                    Check(
                        shield ? hit is DorotiUIKitGraphiteView : hit == editor,
                        $"stage {current} hit target {hit?.GetType().Name}"
                    );
                    var slots = overlay
                        .Subviews.Where(view =>
                            !view.Hidden && view.GetType().Name == "UIKitPlatformRasterSurface"
                        )
                        .ToArray();
                    if (current == 5)
                    {
                        Check(slots.Length >= 3, "R/N/R/N/R Metal surfaces");
                        Check(
                            overlay
                                .Subviews.OfType<UIKitPlatformBlurView>()
                                .Any(effect =>
                                    !effect.Hidden && Math.Abs(effect.AppliedIntensity - .2) < .0001
                                ),
                            "platform-view sample has a live MatchCommon backdrop"
                        );
                    }
                    lines.Add(
                        $"PASS stage={current} identity/state hit={hit!.GetType().Name} rasters={slots.Length} scale={overlay.Window!.Screen.Scale}"
                    );
                });
            }
            await OnUi(() =>
            {
                Check(editor.BecomeFirstResponder(), "native editor focus");
                editor.InsertText("-input");
                Check(editor.Text!.EndsWith("-input"), "native text insertion");
                editor.ResignFirstResponder();
            });
            lines.Add("PASS native focus/text insertion/resign (IME composition not tested)");
            for (var cycle = 0; cycle < 10; cycle++)
            {
                UIView[] retiring = [];
                await OnUi(() =>
                {
                    retiring = Controls();
                    PlatformViewFixtureProbe.ToggleMounted!();
                });
                await Until(() =>
                    Controls().Length == 0 && retiring.All(view => view.Handle == IntPtr.Zero)
                );
                await OnUi(() => PlatformViewFixtureProbe.ToggleMounted!());
                await Until(() => Controls().Length == 2);
            }
            lines.Add("PASS create/dispose=10 cycles (native handles disposed before recreation)");
            lines.Add("RESULT=PASS");
        }
        catch (Exception error)
        {
            lines.Add("RESULT=FAIL\n" + error);
        }
        File.WriteAllLines(path, lines);
        Console.WriteLine(string.Join("\n", lines));
    }

    private static async Task CaptureEffects(string path)
    {
        var lines = new List<string>
        {
            $"GPU={Metal.MTLDevice.SystemDefault?.Name}",
            "renderer=Graphite-Metal",
            "strategy=WKWebView + sibling CAMetalLayer + UIVisualEffectView",
            "visualMatch=notVerified",
            "physicalInput=notVerified",
        };
        try
        {
            WKWebView? web = null;
            await Until(() =>
            {
                web = Controls().OfType<WKWebView>().SingleOrDefault();
                return web is { IsLoading: false };
            });
            await Until(() =>
                web!.Window?.WindowScene?.ActivationState == UISceneActivationState.ForegroundActive
            );
            await OnUi(() =>
                Check(
                    web!.Window!.WindowScene!.Delegate is DorotiMauiSceneDelegate,
                    "MAUI Scene delegate attached"
                )
            );
            lines.Add("PASS foreground-active MAUI UIWindowScene");
            var identity = web!.Handle;
            Check(
                await JavaScript(web, "document.querySelector('input').value='preserved-web-state'")
                    == "preserved-web-state",
                "initial HTML ready"
            );
            var animationBefore = await JavaScript(
                web,
                "document.querySelector('#moving').getBoundingClientRect().x.toString()"
            );
            var animationChanged = false;
            // IsLoading=false can precede WebKit's first visible animation frame,
            // especially while the first scene is connecting on Simulator.
            for (var attempt = 0; attempt < 40 && !animationChanged; attempt++)
            {
                await Task.Delay(200);
                animationChanged =
                    await JavaScript(
                        web,
                        "document.querySelector('#moving').getBoundingClientRect().x.toString()"
                    ) != animationBefore;
            }
            Check(animationChanged, "live DOM animation within eight seconds");
            await OnUi(() =>
            {
                web.ScrollView.SetContentOffset(new CGPoint(0, 100), false);
                Check(web.ScrollView.ContentOffset.Y > 0, "native scroll");
                web.ScrollView.SetContentOffset(CGPoint.Empty, false);
            });
            lines.Add(
                "PASS live DOM animation/native scroll (compositor pixel freshness checked separately)"
            );
            if (Environment.GetEnvironmentVariable("DOROTI_UIKIT_EFFECT_APPEARANCE_CAPTURE") == "1")
                await CaptureAppearance(web, path, lines);
            for (var stage = 0; stage < 7; stage++)
            {
                var current = stage;
                await OnUi(() => PlatformEffectFixtureProbe.SetStage!(current));
                await Task.Delay(1000);
                if (current < 5)
                    Check(
                        await JavaScript(web, "document.querySelector('input').value")
                            == "preserved-web-state",
                        "WebView editing state"
                    );
                await OnUi(() =>
                {
                    var webs = Controls().OfType<WKWebView>().ToArray();
                    Check(
                        webs.Length
                            == (
                                current == 5 ? 0
                                : current is 3 or 4 ? 2
                                : 1
                            ),
                        "WKWebView count"
                    );
                    if (current < 5)
                        Check(webs.Contains(web) && web.Handle == identity, "WKWebView identity");
                    if (current != 5)
                    {
                        var overlay = webs[0].Superview!.Superview!;
                        var effects = overlay
                            .Subviews.OfType<UIVisualEffectView>()
                            .Where(view => !view.Hidden)
                            .ToArray();
                        Check(
                            effects.Length == (current == 1 ? 0 : 1),
                            "material attachment/removal"
                        );
                        Check(
                            effects.All(effect =>
                                effect.Alpha == 1 && !effect.UserInteractionEnabled
                            ),
                            "material alpha/input"
                        );
                        var origin = webs[0].Superview!.Frame.Location;
                        var hit = overlay.HitTest(new CGPoint(origin.X + 150, origin.Y + 90), null);
                        Check(
                            current == 2
                                ? hit is DorotiUIKitGraphiteView
                                : hit is not DorotiUIKitGraphiteView,
                            "effect shield/pass-through"
                        );
                        lines.Add(
                            $"PASS stage={current} webviews={webs.Length} materials={effects.Length} hit={hit?.GetType().Name}"
                        );
                    }
                    else
                        lines.Add("PASS stage=5 all WebViews removed");
                });
            }
            lines.Add("RESULT=PASS (hierarchy/input routing; screenshot comparison separate)");
        }
        catch (Exception error)
        {
            lines.Add("RESULT=FAIL\n" + error);
        }
        File.WriteAllLines(path, lines);
        Console.WriteLine(string.Join("\n", lines));
    }

    private static async Task CaptureAppearance(WKWebView web, string path, List<string> lines)
    {
        UIView? overlay = null;
        var oldStyle = UIUserInterfaceStyle.Unspecified;
        var idle = false;
        await OnUi(() =>
        {
            overlay = web.Superview!.Superview!;
            oldStyle = overlay.OverrideUserInterfaceStyle;
            idle = UIApplication.SharedApplication.IdleTimerDisabled;
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        });
        try
        {
            await JavaScript(
                web,
                "document.querySelector('#moving').style.animation='none'; document.querySelector('#moving').style.transform='translateX(170px)'; 'frozen'"
            );
            foreach (var strength in new[] { .25, .375, .75, 1.0 })
            foreach (var theme in new[] { UIUserInterfaceStyle.Light, UIUserInterfaceStyle.Dark })
            {
                var name = FormattableString.Invariant($"{strength:0.000}-{theme}");
                var ack = path + ".ack-" + name;
                File.Delete(ack);
                await OnUi(() => PlatformEffectFixtureProbe.SetStrength!(strength));
                await Task.Delay(300);
                await OnUi(() => overlay!.OverrideUserInterfaceStyle = theme);
                await Task.Delay(250);
                await OnUi(() =>
                {
                    var effect = overlay!
                        .Subviews.OfType<UIVisualEffectView>()
                        .Single(view => !view.Hidden);
                    lines.Add($"DIAGNOSTIC {name} {effect}");
                    Check(
                        effect is UIKitPlatformBlurView blur
                            && Math.Abs(blur.AppliedIntensity - strength * 16 / 30) < .0001,
                        "calibrated strength reached UIKit animator"
                    );
                    Check(
                        effect.OverrideUserInterfaceStyle == UIUserInterfaceStyle.Light
                            && effect.TraitCollection.UserInterfaceStyle
                                == UIUserInterfaceStyle.Light,
                        "effect must be theme independent"
                    );
                    Check(effect.Alpha == 1, "effect alpha remains one");
                    var bounds = effect.ConvertRectToView(effect.Bounds, effect.Window);
                    File.WriteAllText(
                        path + ".geometry",
                        FormattableString.Invariant(
                            $"{bounds.X},{bounds.Y},{bounds.Width},{bounds.Height},{effect.Window!.Screen.Scale}"
                        )
                    );
                    var window = effect.Window!;
                    using var renderer = new UIGraphicsImageRenderer(window.Bounds.Size);
                    using var screenshot = renderer.CreateImage(_ =>
                        window.DrawViewHierarchy(window.Bounds, true)
                    );
                    using var png = screenshot.AsPNG();
                    File.WriteAllBytes(path + ".window-" + name + ".png", png!.ToArray());
                });
                File.WriteAllText(path + ".capture", name);
                var deadline = DateTime.UtcNow.AddSeconds(45);
                while (!File.Exists(ack))
                {
                    if (DateTime.UtcNow > deadline)
                        throw new TimeoutException("Screenshot acknowledgement: " + name);
                    await Task.Delay(100);
                }
                lines.Add("PASS appearance capture " + name);
            }
        }
        finally
        {
            await OnUi(() =>
            {
                overlay!.OverrideUserInterfaceStyle = oldStyle;
                PlatformEffectFixtureProbe.SetStrength!(.75);
                UIApplication.SharedApplication.IdleTimerDisabled = idle;
            });
            await JavaScript(
                web,
                "document.querySelector('#moving').style.animation=''; document.querySelector('#moving').style.transform=''; 'resumed'"
            );
        }
    }

    private sealed class EmbedEvidence : NSObject, IWKScriptMessageHandler
    {
        internal TaskCompletionSource<string> Result { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void DidReceiveScriptMessage(
            WKUserContentController controller,
            WKScriptMessage message
        )
        {
            if (message.FrameInfo.SecurityOrigin.Host == "www.youtube.com")
                Result.TrySetResult(message.Body.ToString() ?? "");
        }
    }

    private static async Task CaptureWebViewSample(string path)
    {
        var lines = new List<string>
        {
            $"UTC={DateTime.UtcNow:O}",
            $"OS={UIDevice.CurrentDevice.SystemVersion}",
        };
        WKWebView? web = null;
        using var embed = new EmbedEvidence();
        try
        {
            await Until(() =>
            {
                web = Controls().OfType<WKWebView>().SingleOrDefault();
                return web is not null
                    && !web.IsLoading
                    && web.Superview!.Superview!.Subviews.OfType<UIKitPlatformBlurView>()
                        .Any(effect =>
                            !effect.Hidden && Math.Abs(effect.AppliedIntensity - 1.0 / 3) < .0001
                        );
            });
            lines.Add("PASS actual WebView sample has a visible UIKit blur panel at strength .625");
            var baseUri = await JavaScript(web!, "document.baseURI");
            Check(
                baseUri == $"https://{NSBundle.MainBundle.BundleIdentifier!.ToLowerInvariant()}/",
                "app identity base URL"
            );
            lines.Add("PASS HTML base URL=" + baseUri);
            var html = await JavaScript(web!, "document.documentElement.outerHTML");
            await OnUi(() =>
            {
                Check(web!.Configuration.AllowsInlineMediaPlayback, "inline media enabled");
                var controller = web.Configuration.UserContentController;
                controller.AddScriptMessageHandler(embed, "embedEvidence");
                using var scriptText = new NSString(
                    """
                    if (location.hostname === 'www.youtube.com') {
                      setTimeout(() => window.webkit.messageHandlers.embedEvidence.postMessage(JSON.stringify({
                        url: location.href, referrer: document.referrer, title: document.title,
                        player: !!document.querySelector('#movie_player'),
                        error: document.querySelector('.ytp-error-content-wrap')?.innerText || ''
                      })), 8000);
                    }
                    """
                );
                using var script = new WKUserScript(
                    scriptText,
                    WKUserScriptInjectionTime.AtDocumentEnd,
                    false
                );
                controller.AddUserScript(script);
                // Reload() navigates to the synthetic app-identity URL. Reload the
                // local HTML instead, as the sample's Reset page action does.
                using var baseUrl = new NSUrl(baseUri);
                web.LoadHtmlString(html, baseUrl);
            });
            var result = await embed.Result.Task.WaitAsync(TimeSpan.FromSeconds(45));
            lines.Add("YouTube frame=" + result);
            using var document = System.Text.Json.JsonDocument.Parse(result);
            Check(
                document.RootElement.GetProperty("referrer").GetString() == baseUri,
                "YouTube receives app referrer"
            );
            Check(document.RootElement.GetProperty("player").GetBoolean(), "YouTube player loaded");
            Check(
                string.IsNullOrEmpty(document.RootElement.GetProperty("error").GetString()),
                "YouTube player has no configuration error"
            );
            lines.Add(
                "PASS YouTube frame loaded with app referrer; playback/physical input not verified"
            );
            lines.Add("RESULT=PASS");
        }
        catch (Exception error)
        {
            lines.Add("RESULT=FAIL " + error);
        }
        finally
        {
            await OnUi(() =>
            {
                web?.Configuration.UserContentController.RemoveScriptMessageHandler(
                    "embedEvidence"
                );
                web?.Configuration.UserContentController.RemoveAllUserScripts();
            });
            File.WriteAllLines(path, lines);
        }
    }

    private static async Task<string> JavaScript(WKWebView web, string script)
    {
        var completion = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        await OnUi(() =>
            web.EvaluateJavaScript(
                script,
                (value, error) =>
                {
                    if (error is not null)
                        completion.TrySetException(
                            new InvalidOperationException(error.LocalizedDescription)
                        );
                    else
                        completion.TrySetResult(value?.ToString() ?? "");
                }
            )
        );
        return await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
    }

    private static Task OnUi(Action action) =>
        Dispatcher
            .InvokeAsync(() =>
            {
                action();
                return ValueTask.CompletedTask;
            })
            .AsTask();

    private static async Task Until(Func<bool> predicate)
    {
        for (var attempt = 0; attempt < 300; attempt++)
        {
            var done = false;
            await OnUi(() => done = predicate());
            if (done)
                return;
            await Task.Delay(100);
        }
        throw new TimeoutException("Product native hierarchy did not reach the expected state.");
    }

    private static UIView[] Controls() =>
        Microsoft
            .Maui.Controls.Application.Current!.Windows.Select(window =>
                window.Handler?.PlatformView
            )
            .OfType<UIWindow>()
            .SelectMany(Descendants)
            .Where(view =>
                view.AccessibilityIdentifier?.StartsWith(
                    "doroti-platform-view-",
                    StringComparison.Ordinal
                ) == true
                && view.Superview?.Hidden == false
            )
            .ToArray();

    private static IEnumerable<UIView> Descendants(UIView root)
    {
        yield return root;
        foreach (var child in root.Subviews)
        foreach (var descendant in Descendants(child))
            yield return descendant;
    }

    private static void Check(bool condition, string label)
    {
        if (!condition)
            throw new InvalidOperationException(label);
    }
}
