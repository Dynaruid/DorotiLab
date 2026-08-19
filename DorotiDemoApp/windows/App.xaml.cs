using Doroti.Host.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace DorotiDemoApp.WinUI;

public sealed partial class App : DorotiMauiWinUIApplication
{
    public App() => InitializeComponent();

    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    protected override void ConfigurePlatform(MauiAppBuilder builder) =>
        builder.Services.AddSingleton<WindowsPlatformHook>();
}

internal sealed class WindowsPlatformHook;
