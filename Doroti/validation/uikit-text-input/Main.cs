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
    protected override MauiApp CreateMauiApp() => MauiApp.CreateBuilder().UseMauiApp<InputApplication>().Build();
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
            () => new Entry { Opacity = 0, WidthRequest = 1, HeightRequest = 1 },
            () => new Editor { Opacity = 0, WidthRequest = 1, HeightRequest = 1 },
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
