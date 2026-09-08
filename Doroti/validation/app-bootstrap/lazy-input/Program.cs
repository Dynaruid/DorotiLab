using Doroti.Host.Maui;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

var dispatcher = new InputDispatcher();
DispatcherProvider.SetCurrent(new InputDispatcherProvider(dispatcher));
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

// Match Android's split GL/UI threading without replacing the production bridge.
// Native views are represented by real MAUI InputViews; only the dispatcher and
// native handler boundary are controlled here.
var threadedLayout = new Grid();
Entry CreateEntry()
{
    Require(!dispatcher.IsDispatchRequired, "Input factory ran off the UI thread.");
    var input = new Entry();
    input.PropertyChanged += (_, _) => Require(!dispatcher.IsDispatchRequired, "MAUI input mutated off the UI thread.");
    return input;
}
using var threaded = new MauiTextInputBridge(CreateEntry, () => new Editor(), threadedLayout, attachOnDemand: true);
void OnWorker(Action action) => Task.Run(action).GetAwaiter().GetResult();
OnWorker(() => threaded.SetClient(configuration, state));
Require(threaded.Inputs.Count == 0, "Worker allocated the native input synchronously.");
dispatcher.Flush();
var active = (Entry)threaded.Inputs.Single();
Require(active.Text == "한글", "Queued attachment lost its text.");
var changes = new List<DorotiTextEditingState>();
threaded.EditingStateChanged += changes.Add;
OnWorker(() => threaded.UpdateState(new("pasted", new(6, 6), null)));
Require(active.Text == "한글", "Worker updated the native input before UI dispatch.");
dispatcher.Flush();
Require(active.Text == "pasted" && changes.Count == 0, "Framework paste echoed as a native edit.");
OnWorker(threaded.ClearClient);
Require(active.Text == "pasted", "Worker cleared the native input before UI dispatch.");
// An old endpoint can still notify while a newer client is queued. Its edits
// must not be delivered through the already replaced framework subscription.
active.Text = "stale";
Require(changes.Count == 0, "Pending detach published an old client's native edit.");
threaded.SetClient(configuration, new("new client", new(10, 10), null));
dispatcher.Flush();
Require(active.Text == "new client" && threaded.HasClient && threadedLayout.Children.Count == 1,
    "Delayed ClearClient erased or detached the newer client.");
active.Text = "new client pasted";
active.Text += "!";
Require(changes.Count == 2 && changes[^1].text == "new client pasted!", "Input stopped after native paste.");
OnWorker(threaded.ShowTextInput);
active.Text += "typing";
Require(changes.Count == 3 && changes[^1].text.EndsWith("typing", StringComparison.Ordinal),
    "A pending keyboard visibility command suppressed a valid native edit.");
dispatcher.Flush();
OnWorker(threaded.ClearClient);
dispatcher.Flush();
Require(!threaded.HasClient && active.Text == "" && threadedLayout.Children.Count == 0, "Worker detach was incomplete.");
OnWorker(() => threaded.SetClient(configuration, state));
OnWorker(threaded.Dispose);
dispatcher.Flush();
Require(threadedLayout.Children.Count == 0, "Disposed bridge attached queued input.");
Console.WriteLine("MAUI input UI-thread dispatch: worker attach/update/clear, FIFO client replacement, stale edit rejection, native paste then typing, disposal PASS");

sealed class InputDispatcherProvider(InputDispatcher dispatcher) : IDispatcherProvider
{
    public IDispatcher GetForCurrentThread() => dispatcher;
}
sealed class InputDispatcher : IDispatcher
{
    private readonly int _thread = Environment.CurrentManagedThreadId;
    private readonly System.Collections.Concurrent.ConcurrentQueue<Action> _pending = new();
    public bool IsDispatchRequired => Environment.CurrentManagedThreadId != _thread;
    public bool Dispatch(Action action)
    {
        if (IsDispatchRequired) _pending.Enqueue(action);
        else action();
        return true;
    }
    public void Flush() { while (_pending.TryDequeue(out var action)) action(); }
    public bool DispatchDelayed(TimeSpan delay, Action action) { action(); return true; }
    public IDispatcherTimer CreateTimer() => throw new NotSupportedException();
}
