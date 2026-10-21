#if IOS
using Doroti.Ui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Doroti.Host.Maui;

// Only Doroti's own page uses this handler; native app pages retain their usual controller.
internal sealed class DorotiSystemUiPage : ContentPage
{
    internal Brightness? StatusBarBrightness { get; set; }
}

internal sealed class DorotiSystemUiPageHandler : PageHandler
{
    protected override Microsoft.Maui.Platform.ContentView CreatePlatformView()
    {
        ViewController ??= new SystemUiPageViewController(VirtualView, MauiContext!);
        return base.CreatePlatformView();
    }

    private sealed class SystemUiPageViewController(
        Microsoft.Maui.IView view, Microsoft.Maui.IMauiContext context)
        : PageViewController(view, context)
    {
        public override UIStatusBarStyle PreferredStatusBarStyle() => (CurrentView as DorotiSystemUiPage)?.StatusBarBrightness switch
        {
            Brightness.dark => UIStatusBarStyle.LightContent,
            Brightness.light => UIStatusBarStyle.DarkContent,
            _ => base.PreferredStatusBarStyle(),
        };
    }
}
#endif
