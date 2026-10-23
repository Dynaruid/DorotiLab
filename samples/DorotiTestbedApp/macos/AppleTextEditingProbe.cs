#if MACOS || IOS || MACCATALYST
using Doroti.Host.Maui;
using Doroti.Ui;
using Foundation;
using System.Text.Json;
using System.Runtime.InteropServices;
using Microsoft.Maui.Controls;
#if MACOS
using AppKit;
using Microsoft.Maui.Platforms.MacOS.Platform;
#else
using UIKit;
using Microsoft.Maui.Platform;
#endif

namespace DorotiTestbedApp.Apple;

internal sealed record AppleTextEditingCase(string Endpoint, string Name, int Edits, int Callbacks, int? AdditionalNativeTextReads = null);
internal sealed record AppleTextEditingResult(string Status, List<AppleTextEditingCase> Cases,
    int Callbacks, string PhysicalIme = "notVerified", string InputToDisplayLatency = "notVerified");

internal static partial class AppleFeatureProbe
{
    internal static async Task RunTextEditingAsync(string output)
    {
        try
        {
            File.AppendAllText(output + ".launch", "probe task started\n");
            await Task.Delay(1500);
            File.AppendAllText(output + ".launch", "probe delay finished\n");
            var result = await VerifyTextEditing(output);
            File.WriteAllText(output, JsonSerializer.Serialize(result, AppleFeatureProbeJsonContext.Default.AppleTextEditingResult));
        }
        catch (Exception error) { File.WriteAllText(output + ".error", error.ToString()); }
    }

    // Real native editing APIs and every bridge callback are compared. This is
    // deliberately separate from physical keyboard/OS IME and displayed latency.
    private static async Task<AppleTextEditingResult> VerifyTextEditing(string output)
    {
        AbsoluteLayout? layer = null;
        MauiTextInputBridge? bridge = null;
        InputView? active = null;
        var cases = new List<AppleTextEditingCase>();
        var observed = new List<DorotiTextEditingState>();
        var errors = new List<string>();
        Action<DorotiTextEditingState>? respond = null;
        var phase = "attach";
        void Phase(string value)
        {
            phase = value;
            File.AppendAllText(output + ".progress", value + "\n");
        }
        Phase(phase);
        await OnUi(() =>
        {
            var context = Application.Current!.Windows[0].Page!.Handler!.MauiContext!;
            layer = new AbsoluteLayout();
            var handler = layer.ToHandler(context);
#if MACOS
            var window = (NSWindow)Application.Current.Windows[0].Handler!.PlatformView!;
            window.MakeKeyAndOrderFront(null);
            window.ContentView!.AddSubview((NSView)handler.PlatformView!);
            bridge = new MauiTextInputBridge(() => new Entry(), () => new Editor(), layer, attachOnDemand: true);
#else
            UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>().SelectMany(s => s.Windows)
                .First(w => w.IsKeyWindow).RootViewController!.View!.AddSubview((UIView)handler.PlatformView!);
            bridge = new MauiTextInputBridge(() => new DorotiUIKitEntry(), () => new DorotiUIKitEditor(), layer, attachOnDemand: true);
#endif
            bridge.EditingStateChanged += state =>
            {
                try
                {
                    var native = ReadAppleProbeState(layer!.Children.OfType<InputView>().Single());
                    observed.Add(state);
                    if (state != native) errors.Add($"{phase}: published={state}; native={native}");
                    respond?.Invoke(state);
                }
                catch (Exception error) { errors.Add(phase + ": observer: " + error); }
            };
        });

        async Task Prepare(DorotiTextInputType type, bool secure, string text, int start, int end)
        {
            respond = null;
            await OnUi(() => bridge!.SetClient(new(type, DorotiTextInputAction.done,
                DorotiTextCapitalization.none, false, secure, false, false), new(text, new(start, end), null)));
            var deadline = Environment.TickCount64 + 10_000;
            while (true)
            {
                var ready = false;
                await OnUi(() => ready = !bridge!.InputMutationPending && bridge.OwnsNativeFocus);
                if (ready) break;
                Require(Environment.TickCount64 < deadline, "Native client mutation/focus did not finish: " + phase);
                await Task.Delay(20);
            }
            await OnUi(() =>
            {
                active = layer!.Children.OfType<InputView>().Single();
                Require(bridge!.OwnsNativeFocus, "Text probe did not own native focus: " + type);
                SelectAppleProbeRange(active, start, end);
                observed.Clear();
            });
        }

        try
        {
            foreach (var (type, secure, endpoint) in new[] {
                (DorotiTextInputType.text, false, "Entry"),
                (DorotiTextInputType.multiline, false, "Editor"),
                (DorotiTextInputType.text, true, "SecureEntry") })
            {
                foreach (var (name, before, start, end, replacement, repetitions) in new[] {
                    ("repeated-insert", "aaaa", 2, 2, "a", 1),
                    ("repeated-backspace", "aaaaa", 3, 3, "", 1),
                    ("emoji-insert", "😀😀", 2, 2, "😀", 1),
                    ("emoji-backspace", "😀😀😀", 4, 4, "", 1),
                    ("space-insert", "    ", 2, 2, " ", 1),
                    ("selection-replace", "abXYcd", 2, 4, "한글", 1),
                    ("rapid-insert", "aaaa", 2, 2, "a", 12),
                    ("rapid-backspace", new string('a', 16), 14, 14, "", 12) })
                {
                    Phase(endpoint + "/" + name);
                    await Prepare(type, secure, before, start, end);
                    int? extraReads = null;
#if MACOS
                    var previousReads = bridge!.MacOSNativeTextReadCount;
#endif
                    await OnUi(() =>
                    {
                        for (var edit = 0; edit < repetitions; edit++)
                        {
                            var count = observed.Count;
                            EditAppleProbe(active!, replacement);
                            Require(observed.Count > count, phase + ": edit " + edit + " was not published in its native UI turn; "
                                + "native=" + ReadAppleProbeState(active!) + "; managed=" + active!.Text
                                + "; pending=" + bridge!.InputMutationPending + "; errors=" + string.Join("; ", errors));
                        }
                        var expectedText = replacement.Length > 0
                            ? before[..start] + string.Concat(Enumerable.Repeat(replacement, repetitions)) + before[end..]
                            : before.Remove(start - repetitions * (name.StartsWith("emoji") ? 2 : 1),
                                repetitions * (name.StartsWith("emoji") ? 2 : 1));
                        var caret = replacement.Length > 0 ? start + repetitions * replacement.Length
                            : start - repetitions * (name.StartsWith("emoji") ? 2 : 1);
                        Require(ReadAppleProbeState(active!) == new DorotiTextEditingState(expectedText, new(caret, caret), null),
                            phase + ": unexpected native result " + ReadAppleProbeState(active!));
                    });
                    await Task.Delay(60);
                    await OnUi(() => Require(observed.Count > 0 && observed[^1] == ReadAppleProbeState(active!),
                        phase + ": final native state was not published."));
                    Require(errors.Count == 0, string.Join("\n", errors));
#if MACOS
                    extraReads = bridge!.MacOSNativeTextReadCount - previousReads;
                    Require(extraReads == 0, phase + ": ordinary input copied native text again: " + extraReads);
#endif
                    cases.Add(new(endpoint, name, repetitions, observed.Count, extraReads));
                }

                if (secure) continue; // Secure endpoints intentionally do not expose marked text.
                Phase(endpoint + "/marked-text-echo");
                await Prepare(type, false, "ab", 1, 1);
                await OnUi(() =>
                {
                    respond = state => bridge!.UpdateState(state);
                    MarkAppleProbe(active!, "你好", 1);
                });
                await Task.Delay(80);
                await OnUi(() =>
                {
                    var native = ReadAppleProbeState(active!);
                    Require(native == new DorotiTextEditingState("a你好b", new(2, 2), new(1, 3)),
                        phase + ": echo changed marked state: " + native);
                    Require(observed.Count > 0 && observed[^1] == native, phase + ": marked state was not published.");
                    MarkAppleProbe(active!, "你好", 0);
                });
                await Task.Delay(60);
                await OnUi(() =>
                {
                    Require(ReadAppleProbeState(active!) == new DorotiTextEditingState("a你好b", new(1, 1), new(1, 3)),
                        phase + ": cursor-only marked update lost range.");
                    Require(observed[^1] == ReadAppleProbeState(active!), phase + ": cursor-only state was not published.");
                    EditAppleProbe(active!, "你");
                });
                await Task.Delay(60);
                Require(errors.Count == 0, string.Join("\n", errors));
                await OnUi(() => Require(ReadAppleProbeState(active!) == new DorotiTextEditingState("a你b", new(2, 2), null)
                    && observed[^1] == ReadAppleProbeState(active!), phase + ": marked replacement was not committed."));
                cases.Add(new(endpoint, "marked-text-echo-and-commit", 3, observed.Count));

                Phase(endpoint + "/unmark-and-cancel");
                await Prepare(type, false, "ab", 1, 1);
                await OnUi(() => MarkAppleProbe(active!, "你好", 1));
                await Task.Delay(60);
                await OnUi(() => UnmarkAppleProbe(active!));
                await Task.Delay(60);
                await OnUi(() =>
                {
                    var native = ReadAppleProbeState(active!);
                    Require(native.text == "a你好b" && native.composingRange is null && observed[^1] == native,
                        phase + ": same-text unmark was not published: " + native);
                    SelectAppleProbeRange(active!, 1, 3);
                    MarkAppleProbe(active!, "XY", 1);
                });
                await Task.Delay(60);
                await OnUi(() => { MarkAppleProbe(active!, "", 0); UnmarkAppleProbe(active!); });
                await Task.Delay(60);
                await OnUi(() => Require(ReadAppleProbeState(active!) == new DorotiTextEditingState("ab", new(1, 1), null)
                    && observed[^1] == ReadAppleProbeState(active!), phase + ": empty preedit cancellation was not published."));
                Require(errors.Count == 0, string.Join("\n", errors));
                cases.Add(new(endpoint, "same-text-unmark-and-cancel", 5, observed.Count));

                Phase(endpoint + "/formatter");
                await Prepare(type, false, "ab", 1, 1);
                await OnUi(() =>
                {
                    respond = state => bridge!.UpdateState(state with { text = state.text.ToUpperInvariant() });
                    EditAppleProbe(active!, "x");
                });
                await Task.Delay(80);
                await OnUi(() => Require(ReadAppleProbeState(active!) == new DorotiTextEditingState("AXB", new(2, 2), null),
                    phase + ": formatter was not applied: " + ReadAppleProbeState(active!)));
                cases.Add(new(endpoint, "formatter", 1, observed.Count));

                Phase(endpoint + "/latest-formatter");
                await Prepare(type, false, "ab", 1, 1);
                await OnUi(() =>
                {
                    respond = state =>
                    {
                        bridge!.UpdateState(state with { text = state.text.ToUpperInvariant() });
                        bridge.UpdateState(state with { text = state.text.ToUpperInvariant() + "!" });
                    };
                    EditAppleProbe(active!, "x");
                });
                await Task.Delay(80);
                await OnUi(() => Require(ReadAppleProbeState(active!) == new DorotiTextEditingState("AXB!", new(2, 2), null),
                    phase + ": latest formatter was not applied: " + ReadAppleProbeState(active!)));
                cases.Add(new(endpoint, "latest-formatter", 1, observed.Count));

                Phase(endpoint + "/selection-formatter");
                await Prepare(type, false, "ab", 1, 1);
                await OnUi(() =>
                {
                    respond = state => bridge!.UpdateState(state with { selection = new(1, 1) });
                    EditAppleProbe(active!, "x");
                });
                await Task.Delay(80);
                await OnUi(() => Require(ReadAppleProbeState(active!) == new DorotiTextEditingState("axb", new(1, 1), null),
                    phase + ": same-text formatter selection was lost: " + ReadAppleProbeState(active!)));
                cases.Add(new(endpoint, "selection-formatter", 1, observed.Count));

                Phase(endpoint + "/queued-response-after-next-edit");
                await Prepare(type, false, "ab", 1, 1);
                await OnUi(() =>
                {
                    var first = true;
                    respond = state =>
                    {
                        if (!first) return;
                        first = false;
                        bridge!.UpdateState(state with { text = state.text.ToUpperInvariant() });
                    };
                    EditAppleProbe(active!, "x");
                    EditAppleProbe(active!, "y");
                });
                await Task.Delay(80);
                await OnUi(() => Require(ReadAppleProbeState(active!) == new DorotiTextEditingState("axyb", new(3, 3), null),
                    phase + ": old formatter overwrote next edit: " + ReadAppleProbeState(active!)));
                Require(errors.Count == 0, string.Join("\n", errors));
                cases.Add(new(endpoint, "queued-response-after-next-edit", 2, observed.Count));

                Phase(endpoint + "/queued-response-after-new-client");
                await Prepare(type, false, "ab", 1, 1);
                await OnUi(() =>
                {
                    respond = state =>
                    {
                        respond = null;
                        var next = type == DorotiTextInputType.text ? DorotiTextInputType.multiline : DorotiTextInputType.text;
                        bridge!.SetClient(new(next, DorotiTextInputAction.done, DorotiTextCapitalization.none,
                            false, false, false, false), state);
                        bridge.UpdateState(state with { text = state.text.ToUpperInvariant() });
                    };
                    EditAppleProbe(active!, "x");
                });
                await Task.Delay(150);
                await OnUi(() =>
                {
                    active = layer!.Children.OfType<InputView>().Single();
                    Require(ReadAppleProbeState(active) == new DorotiTextEditingState("axb", new(2, 2), null),
                        phase + ": old formatter changed new client: " + ReadAppleProbeState(active));
                });
                Require(errors.Count == 0, string.Join("\n", errors));
                cases.Add(new(endpoint, "queued-response-after-new-client", 1, observed.Count));

                Phase(endpoint + "/show-and-configuration-during-preedit");
                await Prepare(type, false, "ab", 1, 1);
                await OnUi(() => MarkAppleProbe(active!, "你好", 1));
                await Task.Delay(60);
                await OnUi(() => bridge!.ShowTextInput());
                await Task.Delay(80);
                await OnUi(() =>
                {
                    Require(ReadAppleProbeState(active!) == new DorotiTextEditingState("a你好b", new(2, 2), new(1, 3)),
                        phase + ": ShowTextInput reset marked text: " + ReadAppleProbeState(active!));
                    var next = type == DorotiTextInputType.text ? DorotiTextInputType.multiline : DorotiTextInputType.text;
                    bridge!.UpdateConfiguration(new(next, DorotiTextInputAction.done, DorotiTextCapitalization.none,
                        false, false, false, false));
                });
                await Task.Delay(150);
                await OnUi(() =>
                {
                    active = layer!.Children.OfType<InputView>().Single();
                    Require(ReadAppleProbeState(active) == new DorotiTextEditingState("a你好b", new(2, 2), null),
                        phase + ": endpoint configuration lost preedit text/selection: " + ReadAppleProbeState(active));
                });
                Require(errors.Count == 0, string.Join("\n", errors));
                cases.Add(new(endpoint, "show-and-configuration-during-preedit", 3, observed.Count));
            }
            return new("PASS", cases, cases.Sum(c => c.Callbacks));
        }
        catch (Exception error) { throw new Exception("Apple native text probe: " + phase, error); }
        finally
        {
            respond = null;
            await OnUi(() => bridge!.Dispose());
            await Task.Delay(60); // UIKit disposal uses the ordered mutation queue.
            await OnUi(() =>
            {
#if MACOS
                ((NSView)layer!.Handler!.PlatformView!).RemoveFromSuperview();
#else
                ((UIView)layer!.Handler!.PlatformView!).RemoveFromSuperview();
#endif
                layer.Handler.DisconnectHandler();
            });
        }
    }

#if MACOS
    // deleteBackward: returns void. NSObject.PerformSelector treats its result
    // as an object and can try to wrap an undefined return register.
    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void SendAppleDeleteBackward(IntPtr receiver, IntPtr selector, IntPtr sender);

    private static NSTextView AppleProbeEditor(InputView input) => input.Handler!.PlatformView switch
    {
        NSTextView view => view,
        NSTextField field => (NSTextView)field.CurrentEditor!,
        NSScrollView scroll => (NSTextView)scroll.DocumentView!,
        _ => throw new Exception("Unsupported AppKit text endpoint.")
    };
#endif

    private static DorotiTextEditingState ReadAppleProbeState(InputView input)
    {
#if MACOS
        var native = AppleProbeEditor(input);
        var range = native.SelectedRange;
        var marked = native.MarkedRange;
        return new(native.Value, new((int)range.Location, (int)(range.Location + range.Length)),
            native.HasMarkedText ? new((int)marked.Location, (int)(marked.Location + marked.Length)) : null);
#else
        var native = (IUITextInput)input.Handler!.PlatformView!;
        var beginning = native.BeginningOfDocument;
        var range = native.SelectedTextRange!;
        var text = input.Handler!.PlatformView is UITextField field ? field.Text! : ((UITextView)input.Handler.PlatformView!).Text!;
        var marked = native.MarkedTextRange;
        return new(text, new((int)native.GetOffsetFromPosition(beginning, range.Start), (int)native.GetOffsetFromPosition(beginning, range.End)),
            marked is null ? null : new((int)native.GetOffsetFromPosition(beginning, marked.Start), (int)native.GetOffsetFromPosition(beginning, marked.End)));
#endif
    }

    private static void SelectAppleProbeRange(InputView input, int start, int end)
    {
#if MACOS
        AppleProbeEditor(input).SetSelectedRange(new NSRange(start, end - start));
#else
        var native = (IUITextInput)input.Handler!.PlatformView!;
        native.SelectedTextRange = native.GetTextRange(native.GetPosition(native.BeginningOfDocument, start)!,
            native.GetPosition(native.BeginningOfDocument, end)!);
#endif
    }

    private static void EditAppleProbe(InputView input, string replacement)
    {
#if MACOS
        var native = AppleProbeEditor(input);
        if (replacement.Length == 0) SendAppleDeleteBackward(native.Handle, ObjCRuntime.Selector.GetHandle("deleteBackward:"), IntPtr.Zero);
        else native.InsertText(new NSString(replacement), native.HasMarkedText ? native.MarkedRange : native.SelectedRange);
#else
        var native = (IUIKeyInput)input.Handler!.PlatformView!;
        if (replacement.Length == 0) native.DeleteBackward();
        else native.InsertText(replacement);
#endif
    }

    private static void MarkAppleProbe(InputView input, string text, int cursor)
    {
#if MACOS
        var native = AppleProbeEditor(input);
        native.SetMarkedText(new NSString(text), new NSRange(cursor, 0), native.HasMarkedText ? native.MarkedRange : native.SelectedRange);
#else
        ((IUITextInput)input.Handler!.PlatformView!).SetMarkedText(text, new NSRange(cursor, 0));
#endif
    }

    private static void UnmarkAppleProbe(InputView input)
    {
#if MACOS
        AppleProbeEditor(input).UnmarkText();
#else
        ((IUITextInput)input.Handler!.PlatformView!).UnmarkText();
#endif
    }
}
#endif
