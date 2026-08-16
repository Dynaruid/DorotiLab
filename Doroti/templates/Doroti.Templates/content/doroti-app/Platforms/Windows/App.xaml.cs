using CommunityToolkit.Maui.Markup;
using System.Reflection;
using Doroti.Host.Maui;
using Doroti.Hosting;
using Doroti.Ui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DorotiApp.WinUI;

public sealed partial class WindowsMauiApplication : MauiWinUIApplication
{
    public WindowsMauiApplication() => InitializeComponent();

    protected override MauiApp CreateMauiApp() => MauiApp.CreateBuilder()
        .UseMauiApp<WindowsBootstrapApplication>()
        .UseMauiCommunityToolkitMarkup()
        .UseSkiaSharp()
        .Build();
}

internal sealed class WindowsBootstrapApplication : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = activationState;
        var appType = Assembly.GetExecutingAssembly().GetType("DorotiApp.App", throwOnError: true)!;
        var definition = (Func<IDorotiViewEntrypoint>)appType.GetProperty("Definition")!.GetValue(null)!;
        var configuration = (DorotiViewConfiguration)appType.GetProperty("ViewConfiguration")!.GetValue(null)!;
        return new(new ContentPage { Content = new DorotiMauiSurface(definition, configuration) });
    }
}
