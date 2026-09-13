using Doroti.Host.Maui;
using Doroti.Ui;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using UIKit;

UIApplication.Main(args, null, typeof(InputDelegate));

[Register("InputDelegate")]
public sealed class InputDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiApp.CreateBuilder().UseMauiApp<InputApplication>().ConfigureMauiHandlers(handlers => handlers.AddHandler<DorotiUIKitEntry, DorotiUIKitEntryHandler>().AddHandler<DorotiUIKitEditor, DorotiUIKitEditorHandler>()).Build();
}

public sealed class InputApplication : Application
{
    protected override Window CreateWindow(IActivationState? activationState) => new(new InputPage());
}

public sealed class InputPage : ContentPage
{
    private readonly Grid _layout = new();
    public InputPage()
    {
        Content = _layout;
        Loaded += (_, _) => Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), Run);
    }

    private async void Run()
    {
        var log = new List<string>();
        using var bridge = new MauiTextInputBridge(
            () => new DorotiUIKitEntry { Opacity = 0, WidthRequest = 1, HeightRequest = 1 },
            () => new DorotiUIKitEditor { Opacity = 0, WidthRequest = 1, HeightRequest = 1 },
            _layout, attachOnDemand: true);
        var edits = new List<DorotiTextEditingState>();
        bridge.EditingStateChanged += state => { edits.Add(state); log.Add($"edit: {state}"); bridge.UpdateState(state); };
        var changingFrameworkClient = false;
        var reenteredClientChange = false;
        bridge.FocusChanged += _ => reenteredClientChange |= changingFrameworkClient;
        try
        {
            var configuration = new DorotiTextInputConfiguration(DorotiTextInputType.text,
                DorotiTextInputAction.search, DorotiTextCapitalization.none, false, false, true, true);
            // Native focus/keyboard callbacks must not run until the framework
            // has finished installing the connection returned by SetClient.
            // Search switches clients during a tap; synchronous UIKit focus
            // can otherwise reenter that unfinished attach/detach operation.
            changingFrameworkClient = true;
            bridge.SetClient(configuration, new("", new(0, 0), null));
            changingFrameworkClient = false;
            bridge.ShowTextInput();
            await Task.Delay(500);
            Require(!reenteredClientChange, "UIKit focus reentered an unfinished framework client change.");
            var input = (Entry)bridge.Inputs.Single();
            var native = (UITextField)input.Handler!.PlatformView!;
            log.Add($"focus={native.IsFirstResponder} alpha={native.Alpha} enabled={native.Enabled} readOnly={input.IsReadOnly} max={input.MaxLength}");
            Require(native.IsFirstResponder, "Native field did not receive focus.");
            foreach (var ch in "red")
            {
                native.InsertText(ch.ToString());
                await Task.Delay(100);
                log.Add($"insert {ch}: native={native.Text} maui={input.Text} edits={edits.Count}");
            }
            Require(edits.Count > 0 && edits[^1].text == "red", "Native typing did not reach the bridge.");
            changingFrameworkClient = true;
            bridge.ClearClient();
            bridge.SetClient(configuration, new("", new(0, 0), null));
            changingFrameworkClient = false;
            bridge.ShowTextInput();
            await Task.Delay(500);
            native = (UITextField)input.Handler!.PlatformView!;
            log.Add($"reattached focus={native.IsFirstResponder}");
            native.InsertText("blue");
            await Task.Delay(100);
            log.Add($"reattached native={native.Text} maui={input.Text}");
            Require(edits[^1].text == "blue", "Typing stopped after replacing the text client.");
            Require(!reenteredClientChange, "UIKit focus reentered framework client replacement.");
            native.SetMarkedText("ㅎ", new NSRange(1, 0));
            await Task.Delay(100);
            Require(input.Text == "blueㅎ" && native.MarkedTextRange is not null,
                "The framework round trip discarded Korean marked text.");
            native.SetMarkedText("한", new NSRange(1, 0));
            await Task.Delay(100);
            Require(input.Text == "blue한" && native.MarkedTextRange is not null,
                "The framework round trip ended Korean composition.");
            native.UnmarkText();
            native.InsertText("글");
            await Task.Delay(100);
            Require(edits[^1].text == "blue한글", "Committed Korean text did not reach the bridge.");
            native.DeleteBackward();
            await Task.Delay(100);
            Require(edits[^1].text == "blue한", "Native Backspace did not reach the bridge.");
            var cursorEvents = new List<DorotiFloatingCursorEvent>();
            bridge.FloatingCursorChanged += cursorEvents.Add;
            bridge.SetCaretRect(new Doroti.Ui.Rect(30, 40, 32, 60));
            await Task.Delay(100);
            Require(input.WidthRequest > 100 && input.HeightRequest > 100, "IME proxy bounds would clamp 2D keyboard movement.");
            bridge.UpdateState(new(input.Text, new(0, 3), null));
            await Task.Delay(100);
            Require(native.GetSelectionRects(native.SelectedTextRange!).Length == 0,
                "Hidden Entry exposes native selection-handle geometry.");
            Require(native.GetCaretRectForPosition(native.SelectedTextRange!.Start).Height == 20,
                "Suppressing native handles removed the keyboard's caret geometry.");
            var cursorText = input.Text;
            native.BeginFloatingCursor(new CoreGraphics.CGPoint(20, 30));
            native.UpdateFloatingCursor(new CoreGraphics.CGPoint(90, 85));
            native.UpdateFloatingCursor(new CoreGraphics.CGPoint(-10, 5));
            native.EndFloatingCursor();
            Require(cursorEvents.Count == 4 && cursorEvents[0].phase == DorotiFloatingCursorPhase.start &&
                cursorEvents[1].offset == new Doroti.Ui.Offset(70, 55) && cursorEvents[2].offset == new Doroti.Ui.Offset(-30, -25) &&
                cursorEvents[3].phase == DorotiFloatingCursorPhase.end, "Native keyboard did not forward both axes and drag lifecycle.");
            Require(input.Text == cursorText, "Floating cursor changed text.");
            bridge.ClearClient();
            await Task.Delay(100);
            native.UpdateFloatingCursor(new CoreGraphics.CGPoint(100, 100));
            Require(cursorEvents.Count == 4, "Detached client delivered a stale keyboard drag.");
            bridge.SetClient(configuration with { inputType = DorotiTextInputType.multiline }, new("first\nsecond", new(0, 5), null));
            bridge.ShowTextInput();
            await Task.Delay(300);
            var editor = bridge.Inputs.OfType<Editor>().Single();
            var textView = (UITextView)editor.Handler!.PlatformView!;
            bridge.SetCaretRect(new Doroti.Ui.Rect(100, 180, 102, 200));
            await Task.Delay(100);
            Require(textView.GetSelectionRects(textView.SelectedTextRange!).Length == 0,
                "Hidden Editor exposes native selection-handle geometry.");
            Require(textView.GetCaretRectForPosition(textView.SelectedTextRange!.Start).Height == 20,
                "Multiline keyboard lost its caret geometry.");
            textView.BeginFloatingCursor(new CoreGraphics.CGPoint(0, 0));
            textView.UpdateFloatingCursor(new CoreGraphics.CGPoint(40, 80));
            textView.EndFloatingCursor();
            Require(cursorEvents.Count == 7 && cursorEvents[5].offset == new Doroti.Ui.Offset(40, 80), "Multiline keyboard cursor lost Y displacement.");
            if (OperatingSystem.IsIOSVersionAtLeast(16))
            {
                var menuEvents = new List<string>();
                bridge.SystemContextMenuEvent += (method, _) => menuEvents.Add(method);
                await bridge.RunUIKitMenuMutation(() => bridge.ShowSystemContextMenu(new CoreGraphics.CGRect(20, 100, 100, 30),
                    [new("copy", null, null), new("cut", null, null), new("paste", null, null)]));
                await Task.Delay(500);
                var menu = ((UIView)_layout.Handler!.PlatformView!).Subviews.SelectMany(v => v.Interactions).OfType<UIEditMenuInteraction>().Single();
                Require(menu.View is not null, "Native context menu was not attached to the visible view.");
                Require(textView.IsFirstResponder, "Showing the native menu stole input focus.");
                UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("copy:"), textView, null, null);
                Require(UIPasteboard.General.String == "first", "Native menu Copy lost the framework selection.");
                UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("cut:"), textView, null, null);
                await Task.Delay(100);
                Require(edits[^1].text == "\nsecond", "Native menu Cut was not published.");
                UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("paste:"), textView, null, null);
                await Task.Delay(300);
                Require(edits[^1].text == "first\nsecond", "Native menu Paste was not published.");
                await bridge.RunUIKitMenuMutation(() => bridge.ShowSystemContextMenu(new CoreGraphics.CGRect(20, 100, 100, 30), [new("paste", null, null)]));
                await Task.Delay(300);
                ((UIView)_layout.Handler!.PlatformView!).Subviews.SelectMany(v => v.Interactions).OfType<UIEditMenuInteraction>().Single().DismissMenu();
                await Task.Delay(500);
                Require(menuEvents.Contains("ContextMenu.onDismissSystemContextMenu"), "Native menu dismissal was not published.");
                bridge.HideSystemContextMenu();
            }
            log.Add("Native selection geometry: no duplicate handle rectangles, caret and selection preserved PASS");
            log.Add("Native floating cursor: both axes, Entry/Editor, stale-client rejection PASS");
            log.Add("Native edit menu: presentation, focus, Copy/Cut/Paste, dismissal PASS");
            log.Add("PASS");
        }
        catch (Exception error) { log.Add("FAIL: " + error); }
        var result = string.Join('\n', log);
        File.WriteAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "result.txt"), result);
        Console.WriteLine(result);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
