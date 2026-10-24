using Doroti.Host.Maui;

namespace DorotiTestbedApp.WinUI;

public sealed partial class App : DorotiMauiWinUIApplication
{
    public App() => InitializeComponent();

    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
