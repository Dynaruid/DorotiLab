using Doroti.Host.Maui;
namespace DorotiTemplateApp.WinUI;
public sealed partial class App : DorotiMauiWinUIApplication
{
    public App() => InitializeComponent();
    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());
}
