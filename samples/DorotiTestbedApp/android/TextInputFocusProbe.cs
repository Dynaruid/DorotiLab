using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Doroti.Host.Maui;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;
using System.Text.Json;

namespace DorotiTestbedApp.Android;

/// <summary>Opt-in native endpoint regression, with synthetic touch/IME commands on a live Android host.</summary>
internal static class TextInputFocusProbe
{
    internal static void Start(MainActivity activity) => _ = Run(activity);

    private static async Task Run(MainActivity activity)
    {
        var path = System.IO.Path.Combine(activity.ExternalCacheDir!.AbsolutePath!, "text-focus-probe.json");
        MauiTextInputBridge? bridge = null;
        ClipboardManager? clipboard = null;
        ClipData? previousClip = null;
        try
        {
            DorotiMauiSurface? surface = null;
            DorotiAndroidVulkanView? native = null;
            await Wait(() =>
            {
                surface = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page is Microsoft.Maui.Controls.ContentPage page
                    ? FindSurface(page.Content) : null;
                native = (surface?.Children.OfType<DorotiGraphiteView>().FirstOrDefault()?.Handler?.PlatformView as DorotiAndroidViewContainer)?.Surface;
                return surface?.Diagnostics is not null && native is { IsShown: true, HasWindowFocus: true };
            });
            bridge = surface!.TextInput;
            clipboard = (ClipboardManager)activity.GetSystemService(Context.ClipboardService)!;
            previousClip = clipboard.PrimaryClip;
            var states = new List<DorotiTextEditingState>();
            bridge.EditingStateChanged += states.Add;
            var cases = new List<object>();
            try
            {
                foreach (var type in new[] { DorotiTextInputType.text, DorotiTextInputType.multiline })
                {
                    bridge.SetClient(new(type, DorotiTextInputAction.done, DorotiTextCapitalization.none, false, false, true, true), new("seed", new(4, 4), null));
                    bridge.ShowTextInput();
                    EditText? input = null;
                    // MAUI can dispose its service wrapper while refreshing the
                    // keyboard. Reacquire it for each query rather than retain it.
                    bool IsServed(EditText editor) => ((InputMethodManager)activity.GetSystemService(Context.InputMethodService)!).InvokeIsActive(editor);
                    await Wait(() =>
                    {
                        input = bridge.Inputs.Select(i => i.Handler?.PlatformView).OfType<EditText>().SingleOrDefault(i => i.IsFocused);
                        return input is { IsFocused: true } && IsServed(input);
                    });
                    await Task.Delay(250);
                    var endpoint = input!;
                    using var info = new EditorInfo();
                    using var connection = endpoint.OnCreateInputConnection(info) ?? throw new InvalidOperationException("No native InputConnection.");
                    Require(connection.SetComposingText("한", 1), "Composing text was rejected.");
                    var composingStart = BaseInputConnection.GetComposingSpanStart(endpoint.EditableText!);
                    var composingEnd = BaseInputConnection.GetComposingSpanEnd(endpoint.EditableText!);
                    Require(composingStart >= 0 && composingEnd > composingStart, "The synthetic composing command did not create a span.");
                    // Exercise the exact SurfaceView touch path that previously stole EditText focus.
                    var now = global::Android.OS.SystemClock.UptimeMillis();
                    using (var down = MotionEvent.Obtain(now, now, MotionEventActions.Down, 1, 1, 0))
                        Require(native!.OnTouchEvent(down), "Background down was not routed.");
                    using (var up = MotionEvent.Obtain(now, now + 1, MotionEventActions.Up, 1, 1, 0))
                        Require(native!.OnTouchEvent(up), "Background up was not routed.");
                    // Keep composition, touch and commit in one UI turn to
                    // measure touch handling without concurrent IME commands.
                    Require(BaseInputConnection.GetComposingSpanStart(endpoint.EditableText!) == composingStart
                        && BaseInputConnection.GetComposingSpanEnd(endpoint.EditableText!) == composingEnd,
                        $"Background touch changed composition: before={composingStart}:{composingEnd}, after={BaseInputConnection.GetComposingSpanStart(endpoint.EditableText!)}:{BaseInputConnection.GetComposingSpanEnd(endpoint.EditableText!)}.");
                    Require(connection.CommitText("한글", 1), "IME commit after background touch was rejected.");
                    await Task.Delay(250);
                    Require(endpoint.IsFocused && IsServed(endpoint),
                        $"Background touch stole the IME endpoint: focused={endpoint.IsFocused}, active={IsServed(endpoint)}, surfaceFocused={native!.IsFocused}.");
                    clipboard.PrimaryClip = ClipData.NewPlainText("Doroti focus regression", " 붙여넣기");
                    Require(endpoint.OnTextContextMenuItem(global::Android.Resource.Id.Paste), "Native clipboard paste was rejected.");
                    await Wait(() => states.LastOrDefault().text == "seed한글 붙여넣기");
                    Require(endpoint.Text == "seed한글 붙여넣기" && states[^1].selection.extentOffset == endpoint.Text.Length,
                        "Paste did not reach the bridge with the current selection.");
                    cases.Add(new { type = type.ToString(), focused = endpoint.IsFocused, imeActive = IsServed(endpoint), text = endpoint.Text });
                    bridge.ClearClient();
                    Require(!bridge.HasClient && !endpoint.IsFocused, "ClearClient retained native text focus.");
                    using var clearedDown = MotionEvent.Obtain(now + 2, now + 2, MotionEventActions.Down, 1, 1, 0);
                    native!.OnTouchEvent(clearedDown);
                    Require(native.IsFocused, "The surface could not regain focus after ClearClient.");
                }
            }
            finally { bridge.EditingStateChanged -= states.Add; }
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(new { status = "PASS", cases, physicalTouch = "notVerified", keyboardClipboardUi = "notVerified" }));
        }
        catch (Exception error) { System.IO.File.WriteAllText(path, JsonSerializer.Serialize(new { status = "FAIL", error = error.ToString() })); }
        finally
        {
            bridge?.ClearClient();
            if (clipboard is not null)
            {
                if (previousClip is not null) clipboard.PrimaryClip = previousClip;
                else if (OperatingSystem.IsAndroidVersionAtLeast(28)) clipboard.ClearPrimaryClip();
            }
        }
    }

    private static DorotiMauiSurface? FindSurface(Microsoft.Maui.Controls.View? view) => view as DorotiMauiSurface
        ?? (view as Microsoft.Maui.Controls.Layout)?.Children.OfType<Microsoft.Maui.Controls.View>().Select(FindSurface).FirstOrDefault(s => s is not null);

    private static async Task Wait(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (!condition()) { if (DateTime.UtcNow > deadline) throw new TimeoutException("Android focus probe condition did not complete."); await Task.Delay(50); }
    }
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}
