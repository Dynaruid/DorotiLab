using System.Diagnostics;
using System.Text.Json;
using Doroti.Host.Maui;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;

namespace DorotiTestbedApp.WinUI;

/// <summary>Live WinUI text callbacks; synthetic edits, not physical keyboard/IME timing.</summary>
internal static class TextInputEditingProbe
{
    internal static void Start(Microsoft.UI.Xaml.Window window)
    {
        if (Environment.GetEnvironmentVariable("DOROTI_TEXT_EDITING_PROBE") is not { Length: > 0 } path) return;
        _ = Task.Run(async () =>
        {
            DorotiMauiSurface? surface = null;
            var phase = "surface";
            try
            {
                await Wait(() =>
                {
                    surface = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page is Microsoft.Maui.Controls.ContentPage page
                        ? FindSurface(page.Content) : null;
                    return surface?.FrameworkView is not null && surface.WindowsSurface.NativeHost?.IsLoaded == true;
                });
                var cases = new List<object>();
                foreach (var type in new[] { DorotiTextInputType.text, DorotiTextInputType.multiline })
                {
                    foreach (var (name, before, start, length, replacement, after, caret, repetitions) in new[]
                    {
                        ("repeated-insert", "aaaa", 2, 0, "a", "aaaaa", 3, 1),
                        ("repeated-delete", "aaaaa", 2, 1, "", "aaaa", 2, 1),
                        ("emoji-insert", "😀😀", 2, 0, "😀", "😀😀😀", 4, 1),
                        ("emoji-delete", "😀😀😀", 2, 2, "", "😀😀", 2, 1),
                        ("space-insert", "    ", 2, 0, " ", "     ", 3, 1),
                        ("selection-replace", "abXYcd", 2, 2, "한글", "ab한글cd", 4, 1),
                        ("rapid-insert", "aaaa", 2, 0, "a", new string('a', 16), 14, 12),
                        ("rapid-delete", new string('a', 16), 2, 1, "", "aaaa", 2, 12),
                    })
                    {
                        phase = $"{type}/{name}/focus";
                        await MainThread.InvokeOnMainThreadAsync(() => surface!.TextInput.SetClient(
                            new(type, DorotiTextInputAction.done, DorotiTextCapitalization.none, false, false, true, true),
                            new(before, new(start, start + length), null)));
                        await MainThread.InvokeOnMainThreadAsync(() => surface!.TextInput.ShowTextInput());
                        await Wait(() => surface!.TextInput.OwnsNativeFocus);
                        TextBox? native = null;
                        var callbacks = new List<object>();
                        var errors = new List<string>();
                        var editTimer = new Stopwatch();
                        void Observe(DorotiTextEditingState state)
                        {
                            var expected = native!;
                            callbacks.Add(new { state.text, state.selection, state.composingRange,
                                nativeText = expected.Text,
                                managedText = surface!.TextInput.Inputs.Single(i => ReferenceEquals(i.Handler?.PlatformView, expected)).Text,
                                syntheticEditToCallbackMicroseconds = editTimer.Elapsed.TotalMicroseconds,
                                nativeStart = expected.SelectionStart, nativeLength = expected.SelectionLength });
                            if (state.text != expected.Text || state.selection.baseOffset != expected.SelectionStart
                                || state.selection.extentOffset != expected.SelectionStart + expected.SelectionLength)
                                errors.Add(JsonSerializer.Serialize(callbacks.Last()));
                        }
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            phase = $"{type}/{name}/edit";
                            native = surface!.TextInput.Inputs.Select(i => i.Handler?.PlatformView).OfType<TextBox>()
                                .Single(t => MauiWindowsKeyboard.OwnsFocus(t));
                            native.Select(start, length);
                            surface.TextInput.EditingStateChanged += Observe;
                        });
                        try
                        {
                            for (var edit = 0; edit < repetitions; edit++)
                            {
                                var position = start + (length == 0 ? edit * replacement.Length : 0);
                                var previousCount = callbacks.Count;
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    native!.Select(position, length);
                                    editTimer.Restart();
                                    native.SelectedText = replacement;
                                    native.Select(position + replacement.Length, 0);
                                });
                                await Wait(() => callbacks.Count > previousCount);
                            }
                            await Wait(() => callbacks.Count > 0 && native!.Text == after
                                && native.SelectionStart == caret && native.SelectionLength == 0);
                            // Include queued selection-only publications after the native text callback.
                            await MainThread.InvokeOnMainThreadAsync(() => Task.CompletedTask);
                            Require(errors.Count == 0, $"{type}/{name}: callback mismatch: {string.Join("; ", errors)}");
                            cases.Add(new { type = type.ToString(), name, after, caret, repetitions, callbacks });
                        }
                        finally
                        {
                            await MainThread.InvokeOnMainThreadAsync(() => surface!.TextInput.EditingStateChanged -= Observe);
                        }
                    }
                }
                File.WriteAllText(path, JsonSerializer.Serialize(new { status = "PASS", cases,
                    physicalIme = "notVerified", inputToDisplayLatency = "notVerified", framePacing = "notVerified" }));
            }
            catch (Exception error) { File.WriteAllText(path, JsonSerializer.Serialize(new { status = "FAIL", phase, error = error.ToString() })); }
            finally
            {
                // The product window manager owns the complete close sequence.
                await MainThread.InvokeOnMainThreadAsync(() => surface?.TextInput.ClearClient());
                await MainThread.InvokeOnMainThreadAsync(window.Close);
            }
        });
    }

    private static DorotiMauiSurface? FindSurface(Microsoft.Maui.Controls.View? view) => view as DorotiMauiSurface
        ?? (view as Microsoft.Maui.Controls.Layout)?.Children.OfType<Microsoft.Maui.Controls.View>().Select(FindSurface).FirstOrDefault(s => s is not null);

    private static async Task Wait(Func<bool> condition)
    {
        var timer = Stopwatch.StartNew();
        while (!await MainThread.InvokeOnMainThreadAsync(condition))
        {
            if (timer.Elapsed > TimeSpan.FromSeconds(30)) throw new TimeoutException("WinUI editing probe condition timed out.");
            await Task.Delay(30);
        }
    }

    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
