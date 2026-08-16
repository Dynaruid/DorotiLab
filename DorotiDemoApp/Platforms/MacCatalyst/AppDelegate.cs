using CommunityToolkit.Maui.Markup;
using Doroti.Host.Maui;
using Foundation;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DorotiDemoApp.Platforms.MacCatalyst;

[Register("AppDelegate")]
public sealed class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiApp.CreateBuilder()
        .UseMauiApp<MacCatalystBootstrapApplication>()
        .UseMauiCommunityToolkitMarkup()
        .UseSkiaSharp()
        .Build();
}

internal sealed class MacCatalystBootstrapApplication : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = activationState;
        return new(new ContentPage
        {
            Content = new DorotiMauiSurface(global::App.Definition, global::App.ViewConfiguration),
        });
    }
}
