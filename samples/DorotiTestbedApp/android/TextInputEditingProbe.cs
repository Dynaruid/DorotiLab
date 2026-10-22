using Android.Views.InputMethods;
using Android.Widget;
using Doroti.Host.Maui;
using Doroti.Ui;
using System.Diagnostics;
using System.Text.Json;

namespace DorotiTestbedApp.Android;

/// <summary>Opt-in regression for every published state during native mid-text edits.</summary>
internal static class TextInputEditingProbe
{
    internal static void Start(MainActivity activity) => _ = Run(activity);

    private static async Task Run(MainActivity activity)
    {
        var path = System.IO.Path.Combine(activity.ExternalCacheDir!.AbsolutePath!, "text-editing-probe.json");
        MauiTextInputBridge? bridge = null;
        var cases = new List<object>();
        var mismatches = new List<object>();
        var selectionReadCosts = new List<object>();
        try
        {
            DorotiMauiSurface? surface = null;
            await Wait(() =>
            {
                surface = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page is Microsoft.Maui.Controls.ContentPage page
                    ? FindSurface(page.Content) : null;
                return surface?.Diagnostics is not null;
            });
            bridge = surface!.TextInput;
            foreach (var type in new[] { DorotiTextInputType.text, DorotiTextInputType.multiline })
            {
                bridge.SetClient(new(type, DorotiTextInputAction.done, DorotiTextCapitalization.none, false, false, true, true), new("aaaa", new(2, 2), null));
                bridge.ShowTextInput();
                EditText? endpoint = null;
                await Wait(() =>
                {
                    endpoint = bridge.Inputs.Select(i => i.Handler?.PlatformView).OfType<EditText>().SingleOrDefault(i => i.IsFocused);
                    return endpoint is not null;
                });
                await Task.Delay(250);
                var input = endpoint!;
                using var info = new EditorInfo();
                using var connection = input.OnCreateInputConnection(info) ?? throw new InvalidOperationException("No InputConnection.");
                if (type == DorotiTextInputType.text)
                {
                    var managed = bridge.Inputs.Single(i => ReferenceEquals(i.Handler?.PlatformView, input));
                    foreach (var length in new[] { 32, 8192 })
                    {
                        var text = new string('a', length);
                        input.Text = text;
                        input.SetSelection(length / 2);
                        int FullTextRead()
                        {
                            var start = input.SelectionStart;
                            var end = input.SelectionEnd;
                            return start >= 0 && end >= 0 && start <= text.Length && end <= text.Length
                                && string.Equals(input.Text, text, StringComparison.Ordinal) ? start : -1;
                        }
                        int BufferLengthRead()
                        {
                            var start = input.SelectionStart;
                            var end = input.SelectionEnd;
                            return start >= 0 && end >= 0 && start <= text.Length && end <= text.Length
                                && string.Equals(managed.Text, text, StringComparison.Ordinal)
                                && input.EditableText?.Length() == text.Length ? start : -1;
                        }
                        object Measure(Func<int> read)
                        {
                            for (var i = 0; i < 2; i++) Require(read() == length / 2,
                                $"Selection read warmup failed: expected={length / 2}, native={input.SelectionStart}:{input.SelectionEnd}, nativeLength={input.Text?.Length}, managedLength={managed.Text?.Length}.");
                            var samples = new double[12];
                            var allocated = GC.GetAllocatedBytesForCurrentThread();
                            for (var i = 0; i < samples.Length; i++)
                            {
                                var before = Stopwatch.GetTimestamp();
                                var result = read();
                                samples[i] = Stopwatch.GetElapsedTime(before).TotalMicroseconds;
                                Require(result == length / 2, "Selection read changed the cursor.");
                            }
                            allocated = GC.GetAllocatedBytesForCurrentThread() - allocated;
                            Array.Sort(samples);
                            return new { medianMicroseconds = samples[samples.Length / 2], maxMicroseconds = samples[^1], allocatedBytesPerRead = allocated / samples.Length };
                        }
                        selectionReadCosts.Add(new { length, fullText = Measure(FullTextRead), bufferLength = Measure(BufferLengthRead) });
                    }
                    input.Text = "aaaa";
                    input.SetSelection(2);
                }
                var states = new List<DorotiTextEditingState>();
                var operation = "";
                void OnState(DorotiTextEditingState state)
                {
                    states.Add(state);
                    var nativeStart = Math.Min(input.SelectionStart, input.SelectionEnd);
                    var nativeEnd = Math.Max(input.SelectionStart, input.SelectionEnd);
                    if (state.text != input.Text || state.selection.baseOffset != nativeStart || state.selection.extentOffset != nativeEnd)
                        mismatches.Add(new { type = type.ToString(), operation, text = state.text,
                            publishedStart = state.selection.baseOffset, publishedEnd = state.selection.extentOffset,
                            nativeStart, nativeEnd });
                    // Mirror synchronous framework acceptance without disturbing IME composition.
                    bridge.UpdateState(state);
                }
                bridge.EditingStateChanged += OnState;
                try
                {
                    void Edit(string name, Func<bool> edit)
                    {
                        operation = name;
                        states.Clear();
                        Require(edit(), name + " was rejected.");
                        // Text must reach the framework in the same UI turn, including rapid repeats.
                        Require(states.Count > 0, name + " did not publish text synchronously.");
                        Require(states[^1].text == input.Text
                            && states[^1].selection.baseOffset == Math.Min(input.SelectionStart, input.SelectionEnd)
                            && states[^1].selection.extentOffset == Math.Max(input.SelectionStart, input.SelectionEnd),
                            name + " did not settle to the native state.");
                        cases.Add(new { type = type.ToString(), name, publications = states.Count, text = input.Text,
                            start = input.SelectionStart, end = input.SelectionEnd });
                    }

                    // Keep the synthetic commands in one UI turn. A real IME
                    // can otherwise finish our composing span between commands.
                    Edit("repeated-character-insert", () => connection.CommitText("a", 1));
                    Require(input.Text == "aaaaa" && input.SelectionStart == 3, "Mid-text insertion moved the native caret.");
                    Edit("repeated-character-backspace", () => connection.DeleteSurroundingText(1, 0));
                    Require(input.Text == "aaaa" && input.SelectionStart == 2, "Mid-text deletion moved the native caret.");
                    Edit("rapid-middle-typing", () =>
                    {
                        for (var i = 0; i < 12; i++) Require(connection.CommitText("a", 1), "Rapid insert failed.");
                        return true;
                    });
                    Edit("rapid-middle-backspace", () =>
                    {
                        for (var i = 0; i < 12; i++) Require(connection.DeleteSurroundingText(1, 0), "Rapid deletion failed.");
                        return true;
                    });
                    Edit("korean-composing", () => connection.SetComposingText("한", 1));
                    Require(BaseInputConnection.GetComposingSpanStart(input.EditableText!) == 2
                        && BaseInputConnection.GetComposingSpanEnd(input.EditableText!) == 3, "Framework echo disturbed composition.");
                    Edit("korean-composing-update", () => connection.SetComposingText("한글", 1));
                    Require(BaseInputConnection.GetComposingSpanStart(input.EditableText!) == 2
                        && BaseInputConnection.GetComposingSpanEnd(input.EditableText!) == 4, "Framework echo disturbed updated composition.");
                    Edit("korean-commit", () => connection.CommitText("한국어", 1));
                    Require(connection.SetSelection(1, 4), "Range selection was rejected.");
                    Edit("selection-replacement", () => connection.CommitText("가나", 1));
                    Edit("emoji-middle-insert", () => connection.CommitText("😀", 1));
                    // Also inspect the coalesced selection notification after
                    // the continuous text callbacks have all completed.
                    await Task.Delay(30);
                    Require(states[^1].text == input.Text
                        && states[^1].selection.baseOffset == Math.Min(input.SelectionStart, input.SelectionEnd)
                        && states[^1].selection.extentOffset == Math.Max(input.SelectionStart, input.SelectionEnd),
                        "Deferred selection did not retain the latest native state.");
                }
                finally
                {
                    bridge.EditingStateChanged -= OnState;
                    bridge.ClearClient();
                }
            }
            Require(mismatches.Count == 0, $"Published {mismatches.Count} transient states with a cursor different from Android's live selection.");
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(new { status = "PASS", cases, mismatches, selectionReadCosts, physicalKeyboardInput = "notVerified" }));
        }
        catch (Exception error)
        {
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(new { status = "FAIL", error = error.ToString(), cases, mismatches }));
        }
        finally { bridge?.ClearClient(); }
    }

    private static DorotiMauiSurface? FindSurface(Microsoft.Maui.Controls.View? view) => view as DorotiMauiSurface
        ?? (view as Microsoft.Maui.Controls.Layout)?.Children.OfType<Microsoft.Maui.Controls.View>().Select(FindSurface).FirstOrDefault(s => s is not null);

    private static async Task Wait(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (!condition()) { if (DateTime.UtcNow > deadline) throw new TimeoutException("Android editing probe did not become ready."); await Task.Delay(50); }
    }

    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}
