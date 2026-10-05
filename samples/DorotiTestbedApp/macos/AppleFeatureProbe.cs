#if MACOS || IOS || MACCATALYST
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Host.Maui;
using Doroti.Hosting;
using Doroti.Ui;
using Foundation;
using Microsoft.Maui.Controls;
#if MACOS
using AppKit;
using Microsoft.Maui.Platforms.MacOS.Platform;
#else
using Microsoft.Maui.Platform;
using UIKit;
#endif
using Rect = Doroti.Ui.Rect;

namespace DorotiTestbedApp.Apple;

internal sealed record AppleFeatureProbeResult(string Status, string WebView, string Semantics,
    string Input, string PhysicalIme, string VoiceOver);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(AppleFeatureProbeResult))]
internal sealed partial class AppleFeatureProbeJsonContext : JsonSerializerContext;

// Mirrored in ios/ and macos/ to honor the runner source boundary.
// Testbed-only native provider checks. Physical IME and VoiceOver are separate gates.
internal static class AppleFeatureProbe
{
    private static async Task OnUi(Action action) =>
        await AppleWebViewUi.Dispatcher.InvokeAsync(() =>
        {
            action();
            return ValueTask.CompletedTask;
        });

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new Exception(message);
    }

    internal static async Task RunAsync(string output)
    {
        try
        {
            await Task.Delay(1500);
            await VerifyWebView();
            await VerifySemanticsAndInput();
            File.WriteAllText(
                output,
                JsonSerializer.Serialize(
                    new AppleFeatureProbeResult("PASS",
                        "native factory commands/HTML/JS/app resource/message/stale generation/close",
                        "hierarchy/order/role/native action/disabled/password/removal/owner disposal",
                        "configuration-only text/selection and multiline/password endpoint transitions",
                        "notVerified", "notVerified"),
                    AppleFeatureProbeJsonContext.Default.AppleFeatureProbeResult
                )
            );
        }
        catch (Exception error)
        {
            File.WriteAllText(output + ".error", error.ToString());
        }
    }

    private sealed class ProbeResources : IApplicationResourceHostCapability
    {
        private readonly byte[] _bytes = Encoding.UTF8.GetBytes(
            "<meta charset=utf-8><title>app 한글</title><p>resource</p>"
        );
        public IReadOnlyList<DorotiApplicationResource> Resources =>
            [new("probe.html", "asset", null, null, "fixture", _bytes.Length)];

        public ValueTask<ReadOnlyMemory<byte>> LoadAsync(
            string key,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            Require(key == "probe.html", "Unexpected resource key.");
            return ValueTask.FromResult<ReadOnlyMemory<byte>>(_bytes);
        }

        public DorotiApplicationResource ResolveFont(string family) =>
            throw new NotSupportedException();

        public DorotiApplicationResource ResolveLocalization(string locale) =>
            throw new NotSupportedException();
    }

    private static async Task VerifyWebView()
    {
        IPlatformViewInstance? instance = null;
#if MACOS
        NSView? parent = null;
#else
        UIView? parent = null;
#endif
        var messages = new List<WebViewEvent>();
        await OnUi(() =>
        {
            parent = new();
#if MACOS
            IPlatformViewFactory factory = new AppKitPlatformViewFactory(
                () => parent!,
                "doroti/webview",
                resources: () => new ProbeResources()
            );
#else
            IPlatformViewFactory factory = new UIKitPlatformViewFactory(() => parent!, "doroti/webview", () => { }, () => new ProbeResources());
#endif
            var options = new WebViewOptions(
                "<title>initial 한글</title><input>",
                Resources: new() { ["/probe.html"] = new("probe.html", "text/html") },
                MessageOrigins: ["doroti-app://content"]
            );
            instance = factory
                .CreateAsync(new(900, 1, 1), options.Encode(), _ => { }, default)
                .GetAwaiter()
                .GetResult();
            ((IPlatformWebViewInstance)instance).WebViewChanged += value => messages.Add(value);
        });
        async Task<WebViewResult> Command(
            WebViewOperation operation,
            string? text = null,
            long generation = 0
        )
        {
            Task<WebViewResult>? result = null;
            await OnUi(() =>
                result = ((IPlatformWebViewInstance)instance!).ExecuteAsync(
                    new(operation, text, generation),
                    default
                )
            );
            return await result!;
        }
        async Task<WebViewResult> Loaded(string expectedTitle)
        {
            var deadline = DateTime.UtcNow.AddSeconds(20);
            while (DateTime.UtcNow < deadline)
            {
                var state = await Command(WebViewOperation.State);
                if (!state.IsLoading && state.Title == expectedTitle)
                    return state;
                await Task.Delay(50);
            }
            throw new TimeoutException("WebKit load did not complete: " + expectedTitle);
        }
        try
        {
            var features = (await Command(WebViewOperation.Features)).Features!;
            Require(
                features.JavaScript
                    && features.BackForward
                    && features.ScriptMessages
                    && features.AppContentScheme,
                "Apple WebView controller features missing."
            );
            await Loaded("initial 한글");
            Require(
                (
                    await Command(WebViewOperation.EvaluateJavaScript, "({text:'한글',value:42})")
                ).Json?.Contains("42") == true,
                "WebKit JSON result lost."
            );
            Require(
                (await Command(WebViewOperation.EvaluateJavaScript, "undefined")).IsUndefined,
                "Undefined was collapsed."
            );
            Require(
                (await Command(WebViewOperation.EvaluateJavaScript, "null")).Json == "null",
                "Null result lost."
            );
            try
            {
                await Command(WebViewOperation.EvaluateJavaScript, "Promise.resolve(1)");
                throw new Exception("Promise result admitted.");
            }
            catch (WebViewException error) when (error.Code == WebViewError.JavaScript) { }
            var before = await Command(WebViewOperation.State);
            await Command(WebViewOperation.LoadHtml, "<title>changed</title><p>한글</p>");
            var after = await Loaded("changed");
            Require(
                after.DocumentGeneration > before.DocumentGeneration,
                "Document generation did not advance."
            );
            try
            {
                await Command(WebViewOperation.EvaluateJavaScript, "1", before.DocumentGeneration);
                throw new Exception("Stale command admitted.");
            }
            catch (WebViewException error) when (error.Code == WebViewError.NavigationChanged) { }
            await Command(WebViewOperation.Navigate, "doroti-app://content/probe.html");
            await Loaded("app 한글");
            await Command(
                WebViewOperation.EvaluateJavaScript,
                "window.doroti.postMessage('probe',{text:'한글'})"
            );
            await Task.Delay(100);
            await OnUi(() =>
                Require(
                    messages.Any(value =>
                        value.Kind == WebViewEventKind.Message && value.MessageName == "probe"
                    ),
                    "App-content script message did not reach the controller."
                )
            );
            await Command(WebViewOperation.ClearData);
            await OnUi(() => instance!.DisableInputAsync().GetAwaiter().GetResult());
            try
            {
                await Command(WebViewOperation.State);
                throw new Exception("Closed WebView accepted command.");
            }
            catch (WebViewException error) when (error.Code == WebViewError.Closed) { }
        }
        finally
        {
            await OnUi(() =>
            {
                instance?.DisposeAsync().GetAwaiter().GetResult();
                parent?.Dispose();
            });
        }
    }

    private static async Task VerifySemanticsAndInput()
    {
        AbsoluteLayout? layer = null;
        MauiSemanticsBridge? bridge = null;
        MauiTextInputBridge? input = null;
        var actions = new List<(int, SemanticsAction)>();
        var inputActions = new List<DorotiTextInputAction>();
        var parent = new SemanticsNodeUpdate(
            1,
            Rect.fromLTWH(0, 0, 240, 200),
            "list",
            null,
            SemanticsAction.scrollDown,
            [2, 3],
            role: SemanticsRole.list
        );
        var item = new SemanticsNodeUpdate(
            2,
            Rect.fromLTWH(10, 10, 100, 30),
            "activate",
            null,
            SemanticsAction.tap | SemanticsAction.increase | SemanticsAction.decrease,
            [],
            indexInParent: 1
        );
        var password = new SemanticsNodeUpdate(
            3,
            Rect.fromLTWH(10, 50, 100, 30),
            "password",
            "SECRET",
            SemanticsAction.copy | SemanticsAction.setText,
            [],
            new(isTextField: true, isObscured: true),
            indexInParent: 0
        );
        await OnUi(() =>
        {
            var context = Microsoft
                .Maui
                .Controls
                .Application
                .Current!
                .Windows[0]
                .Page!
                .Handler!
                .MauiContext!;
            layer = new AbsoluteLayout();
            var handler = layer.ToHandler(context);
#if MACOS
            NSApplication.SharedApplication.KeyWindow!.ContentView!.AddSubview(
                (NSView)handler.PlatformView!
            );
#else
            UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>().SelectMany(scene => scene.Windows)
                .First(window => window.IsKeyWindow).RootViewController!.View!.AddSubview((UIView)handler.PlatformView!);
#endif
            bridge = new MauiSemanticsBridge(layer);
            bridge.Update(
                new(1, [parent, item, password], SemanticsUpdateUrgency.immediate),
                (id, action, _) => actions.Add((id, action))
            );
        });
        await Task.Delay(150);
        AppleSemanticsView? oldNative = null;
        await OnUi(() =>
        {
            var root = layer!.Children.OfType<MauiSemanticsLayout>().Single();
            Require(
                root.Children.OfType<MauiSemanticsLayout>()
                    .Select(child => child.Node!.id)
                    .SequenceEqual(new[] { 3, 2 }),
                "Semantic child traversal order lost."
            );
            var nativeRoot = (AppleSemanticsView)root.Handler!.PlatformView!;
            var children = root.Children.OfType<MauiSemanticsLayout>().ToArray();
            var secret = (AppleSemanticsView)children[0].Handler!.PlatformView!;
            oldNative = (AppleSemanticsView)children[1].Handler!.PlatformView!;
            Require(
                ((Entry)children[0].Children[0]).Text != "SECRET",
                "Password copied into native proxy."
            );
#if MACOS
            Require(
                nativeRoot.AccessibilityRole == NSAccessibilityRoles.ListRole
                    && nativeRoot.AccessibilityChildren!.Length == 2,
                "AppKit semantic role/children missing."
            );
            Require(
                secret.AccessibilityValue is null && secret.AccessibilityCustomActions!.Length == 0,
                "Password exposed by AppKit provider."
            );
            Require(oldNative.AccessibilityPerformPress(), "AppKit native press rejected.");
            Require(oldNative.AccessibilityPerformIncrement(), "AppKit native increment rejected.");
#else
            Require(nativeRoot.AccessibilityContainerType == UIAccessibilityContainerType.List && nativeRoot.AccessibilityElementCount() == 2,
                "UIKit semantic list/children missing.");
            Require(secret.AccessibilityValue is null && secret.AccessibilityCustomActions!.Length == 0, "Password exposed by UIKit provider.");
            Require(oldNative.AccessibilityActivate(), "UIKit activation rejected.");
            oldNative.AccessibilityIncrement();
#endif
            Require(
                actions.SequenceEqual(
                    new[] { (2, SemanticsAction.tap), (2, SemanticsAction.increase) }
                ),
                "Native action dispatched to wrong node."
            );
            var disabled = item with { flags = new(isEnabled: Tristate.isFalse) };
            bridge!.Update(
                new(2, [parent, disabled, password], SemanticsUpdateUrgency.immediate),
                (id, action, _) => actions.Add((id, action))
            );
        });
        await Task.Delay(100);
        await OnUi(() =>
        {
#if MACOS
            Require(!oldNative!.AccessibilityPerformPress(), "Disabled native node activated.");
#else
            Require(!oldNative!.AccessibilityActivate(), "Disabled native node activated.");
#endif
            bridge!.Update(
                new(
                    3,
                    [parent with { children = [3] }, password],
                    SemanticsUpdateUrgency.immediate
                ),
                (id, action, _) => actions.Add((id, action))
            );
        });
        await Task.Delay(100);
        await OnUi(() =>
        {
#if MACOS
            Require(
                !oldNative!.AccessibilityPerformPress(),
                "Removed node accepted stale native callback."
            );
#else
            Require(!oldNative!.AccessibilityActivate(), "Removed node accepted stale native callback.");
#endif
            bridge!.Dispose();
            layer!.Children.Clear();
#if MACOS
            input = new MauiTextInputBridge(
                () => new Entry(),
                () => new Editor(),
                layer,
                attachOnDemand: true
            );
#else
            input = new MauiTextInputBridge(() => new DorotiUIKitEntry(), () => new DorotiUIKitEditor(), layer, attachOnDemand: true);
#endif
            input.ActionPerformed += action => inputActions.Add(action);
            input.SetClient(
                new(
                    DorotiTextInputType.text,
                    DorotiTextInputAction.done,
                    DorotiTextCapitalization.none,
                    false,
                    false,
                    true,
                    true
                ),
                new("한글😀abc", new(2, 4), null)
            );
        });
        await Task.Delay(150);
        await OnUi(() =>
        {
            var initial = layer!.Children.OfType<InputView>().Single();
            Require(
                initial.CursorPosition == 2 && initial.SelectionLength == 2,
                "Initial native attachment lost selection."
            );
        });
        foreach (
            var type in new[]
            {
                DorotiTextInputType.text,
                DorotiTextInputType.multiline,
                DorotiTextInputType.text,
            }
        )
        {
            await OnUi(() =>
                input!.UpdateConfiguration(
                    new(
                        type,
                        DorotiTextInputAction.search,
                        DorotiTextCapitalization.words,
                        false,
                        type == DorotiTextInputType.text,
                        false,
                        false
                    )
                )
            );
            await Task.Delay(150);
            await OnUi(() =>
            {
                var current = layer!.Children.OfType<InputView>().Single();
                Require(
                    current.Text == "한글😀abc"
                        && current.CursorPosition == 2
                        && current.SelectionLength == 2,
                    "Input configuration update ("
                        + type
                        + ") lost text/selection: "
                        + current.Text
                        + " "
                        + current.CursorPosition
                        + ":"
                        + current.SelectionLength
                );
#if IOS || MACCATALYST
                var nativeInput = (IUITextInput)current.Handler!.PlatformView!;
                var range = nativeInput.SelectedTextRange!;
                Require(
                    nativeInput.GetOffsetFromPosition(nativeInput.BeginningOfDocument, range.Start)
                        == 2
                        && nativeInput.GetOffsetFromPosition(
                            nativeInput.BeginningOfDocument,
                            range.End
                        ) == 4,
                    "UIKit native selection differs from MAUI state."
                );
                if (current.Handler.PlatformView is UITextField field)
                    Require(
                        field.SecureTextEntry && field.ReturnKeyType == UIReturnKeyType.Search,
                        "UIKit traits at "
                            + type
                            + ": secure="
                            + field.SecureTextEntry
                            + ", key="
                            + field.ReturnKeyType
                            + ", entryPassword="
                            + ((Entry)current).IsPassword
                    );
                else if (current.Handler.PlatformView is UITextView editor)
                    Require(
                        editor.ReturnKeyType == UIReturnKeyType.Search,
                        "UIKit multiline action was not applied."
                    );
                if (current.Handler.PlatformView is DorotiUIKitTextView multiline)
                {
                    var beforeText = multiline.Text;
                    multiline.InsertText("\n");
                    Require(
                        multiline.Text == beforeText
                            && inputActions.SequenceEqual(new[] { DorotiTextInputAction.search }),
                        "Multiline Return did not dispatch search without inserting a newline."
                    );
                }
#endif
            });
        }
        await OnUi(() =>
        {
#if IOS || MACCATALYST
            Require(
                inputActions.SequenceEqual(new[] { DorotiTextInputAction.search }),
                "Configuration/endpoint changes submitted a spurious input action."
            );
#endif
            input!.Dispose();
            bridge!.Dispose();
            var native = layer!.Handler!.PlatformView!;
#if MACOS
            ((NSView)native).RemoveFromSuperview();
#else
            ((UIView)native).RemoveFromSuperview();
#endif
            layer.Handler.DisconnectHandler();
        });
    }
}
#endif
