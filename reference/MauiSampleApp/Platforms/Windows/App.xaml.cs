namespace MauiSampleApp.WinUI;

public sealed partial class WindowsMauiApplication : MauiWinUIApplication
{
    public WindowsMauiApplication() => InitializeComponent();

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
