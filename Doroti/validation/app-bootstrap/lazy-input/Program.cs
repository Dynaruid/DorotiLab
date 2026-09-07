using Doroti.Host.Maui;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

DispatcherProvider.SetCurrent(new InlineProvider());
var entries = 0;
var editors = 0;
var layout = new Grid();
var bridge = new MauiTextInputBridge(() => { entries++; return new Entry(); },
    () => { editors++; return new Editor(); }, layout, attachOnDemand: true);
void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
var configuration = new DorotiTextInputConfiguration(DorotiTextInputType.text, DorotiTextInputAction.done,
    DorotiTextCapitalization.none, false, false, true, true);
var state = new DorotiTextEditingState("한글", new(2, 2), null);
bridge.Suspend(); bridge.Resume(); bridge.HideTextInput(); bridge.ClearClient();
Require(entries == 0 && editors == 0 && bridge.Inputs.Count == 0, "No-client lifecycle allocated hidden inputs.");
bridge.SetClient(configuration, state);
Require(entries == 1 && editors == 0 && layout.Children.Count == 1, "First single-line client allocation/attach differs.");
Require(bridge.Inputs[0].Text == "한글" && bridge.Inputs[0].CursorPosition == 2, "First editing state lost.");
bridge.Suspend();
Require(layout.Children.Count == 0, "Suspend retained native visual child.");
bridge.Resume();
Require(layout.Children.Count == 1 && entries == 1, "Resume recreated input.");
bridge.SetClient(configuration with { inputType = DorotiTextInputType.multiline }, state);
Require(entries == 1 && editors == 1 && layout.Children.Single() is Editor, "Multiline switch did not replace active child.");
bridge.SetClient(configuration, state);
Require(entries == 1 && editors == 1 && layout.Children.Single() is Entry, "Single-line return recreated input.");
var edits = 0;
bridge.EditingStateChanged += _ => edits++;
bridge.Dispose(); bridge.Dispose();
Require(layout.Children.Count == 0, "Dispose did not detach inputs.");
foreach (var input in bridge.Inputs) input.Text = "after disposal";
Require(edits == 0, "Disposed bridge published an edit.");
var rejected = false;
try { bridge.SetClient(configuration, state); } catch (ObjectDisposedException) { rejected = true; }
Require(rejected && entries == 1 && editors == 1, "Disposed bridge recreated a client.");
Console.WriteLine("Lazy MAUI managed input lifecycle: PASS (native handler/IME notVerified)");

sealed class InlineProvider : IDispatcherProvider
{
    public IDispatcher GetForCurrentThread() => new InlineDispatcher();
}
sealed class InlineDispatcher : IDispatcher
{
    public bool IsDispatchRequired => false;
    public bool Dispatch(Action action) { action(); return true; }
    public bool DispatchDelayed(TimeSpan delay, Action action) { action(); return true; }
    public IDispatcherTimer CreateTimer() => throw new NotSupportedException();
}
