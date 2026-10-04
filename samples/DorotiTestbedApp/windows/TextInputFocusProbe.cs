using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.Maui;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.UI.Xaml.Controls;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;

namespace DorotiTestbedApp.WinUI;

/// <summary>Opt-in live MAUI focus regression; native messages and edits are synthetic.</summary>
internal static class TextInputFocusProbe
{
    internal static void Start(Microsoft.UI.Xaml.Window window)
    {
        if (Environment.GetEnvironmentVariable("DOROTI_TEXT_FOCUS_PROBE") is not { Length: > 0 } path) return;
        _ = Task.Run(async () =>
        {
            DorotiMauiSurface? surface = null;
            try
            {
                await Wait(() =>
                {
                    surface = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page is Microsoft.Maui.Controls.ContentPage page
                        ? FindSurface(page.Content) : null;
                    return surface?.FrameworkView is not null && surface.WindowsSurface.NativeHost?.IsLoaded == true;
                });
                var cases = new List<object>();
                foreach (var (type, secure) in new[] { (DorotiTextInputType.text, false), (DorotiTextInputType.multiline, false), (DorotiTextInputType.text, true) })
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        surface!.TextInput.SetClient(new(type, DorotiTextInputAction.done, DorotiTextCapitalization.none, false, secure, true, true), new("seed", new(4, 4), null)));
                    await Wait(() => surface!.TextInput.OwnsNativeFocus);
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var bridge = surface!.TextInput;
                        var native = surface.WindowsSurface;
                        var states = new List<DorotiTextEditingState>();
                        bridge.EditingStateChanged += states.Add;
                        var downs = 0;
                        void Observe(MauiSurfacePointerData data) { if (data.Change == PointerChange.down) downs++; }
                        native.Pointer += Observe;
                        try
                        {
                            native.RequestFocus(true);
                            Require(bridge.OwnsNativeFocus, "View focus activation stole the native text endpoint.");
                            var inputWindow = WindowsRootMouseInput.FindInputWindow(native.WindowHandle);
                            Require(inputWindow != 0, "No live WinUI input HWND.");
                            Require((native.NativeHost!.InputOwner as Microsoft.UI.Xaml.FrameworkElement)?.AllowFocusOnInteraction == false,
                                "WinUI can automatically steal text focus before pointer dispatch.");
                            // The live overlay host uses XAML routing. Attach the
                            // raw Composition mouse path to the same input HWND
                            // to exercise its focus policy with real native peers.
                            using var rootMouse = native.UsesNativeWindowOutput ? null : new WindowsRootMouseInput(
                                inputWindow, () => 0, native.HandleNativePointer, () => bridge.OwnsNativeFocus);
                            SendMessageW(inputWindow, 0x0201, 1, (nint)(4 | 100 << 16));
                            SendMessageW(inputWindow, 0x0202, 0, (nint)(4 | 100 << 16));
                            Require(downs == 1 && bridge.OwnsNativeFocus, $"Background click: routedDowns={downs}, textFocused={bridge.OwnsNativeFocus}, focusedElement={Microsoft.UI.Xaml.Input.FocusManager.GetFocusedElement(native.NativeHost!.XamlRoot)?.GetType().Name}.");
                            var input = bridge.Inputs.Single(i => i.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement element && MauiWindowsKeyboard.OwnsFocus(element));
                            if (input.Handler?.PlatformView is Microsoft.Maui.Platform.MauiPasswordTextBox { IsPassword: true } secureText)
                                secureText.Password += " edited";
                            else if (input.Handler?.PlatformView is TextBox text) text.Text += " edited";
                            else if (input.Handler?.PlatformView is PasswordBox password) password.Password += " edited";
                            else throw new InvalidOperationException("Unexpected native editing endpoint.");
                            // Native TextChanged/MAUI state publication can finish
                            // on a later dispatcher turn.
                            await Wait(() => states.Count > 0);
                            Require(states.LastOrDefault().text == "seed edited", $"Native edit: type={type}, secure={secure}, text={input.Text}, published={states.LastOrDefault().text}, states={states.Count}.");
                            cases.Add(new { type = type.ToString(), secure, focused = bridge.OwnsNativeFocus, text = input.Text, routedDowns = downs });
                            bridge.YieldWindowsNativeFocus();
                            Require(bridge.HasClient && !bridge.OwnsNativeFocus, "Yield retained native ownership of a framework client.");
                            native.RequestFocus(true);
                            Require(MauiWindowsKeyboard.OwnsFocus(native.NativeHost!.InputOwner), "A retained client blocked explicit native focus transfer.");
                            bridge.ClearClient();
                            Require(!bridge.HasClient && !bridge.OwnsNativeFocus, "ClearClient retained text input focus.");
                            native.RequestFocus(true);
                            Require(MauiWindowsKeyboard.OwnsFocus(native.NativeHost!.InputOwner), "The surface did not regain focus after ClearClient.");
                        }
                        finally { bridge.EditingStateChanged -= states.Add; native.Pointer -= Observe; }
                    });
                }
                File.WriteAllText(path, JsonSerializer.Serialize(new { status = "PASS", cases, physicalPointer = "notVerified", physicalIme = "notVerified" }));
            }
            catch (Exception error) { File.WriteAllText(path, JsonSerializer.Serialize(new { status = "FAIL", error = error.ToString() })); }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    surface?.TextInput.ClearClient();
                    surface?.PrepareFrameworkClose();
                });
                if (surface is not null)
                    await MainThread.InvokeOnMainThreadAsync(surface.PrepareDesktopCloseAsync);
                await MainThread.InvokeOnMainThreadAsync(window.Close);
            }
        });
    }

    private static DorotiMauiSurface? FindSurface(Microsoft.Maui.Controls.View? view) => view as DorotiMauiSurface
        ?? (view as Microsoft.Maui.Controls.Layout)?.Children.OfType<Microsoft.Maui.Controls.View>().Select(FindSurface).FirstOrDefault(s => s is not null);
    private static async Task Wait(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (!await MainThread.InvokeOnMainThreadAsync(condition)) { if (DateTime.UtcNow > deadline) throw new TimeoutException("Windows focus probe condition did not complete."); await Task.Delay(50); }
    }
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint SendMessageW(nint window, uint message, nuint wparam, nint lparam);
}
