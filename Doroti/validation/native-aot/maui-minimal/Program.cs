using Foundation;
using UIKit;

namespace Doroti.Validation.NativeAot.MauiMinimal;

public static class Program
{
    public static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}

[Register("AppDelegate")]
public sealed class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiApp.CreateBuilder().UseMauiApp<MinimalApplication>().Build();
}

public sealed class MinimalApplication : Application
{
    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new ContentPage { Content = new Label { Text = "MAUI NativeAOT dependency repro" } });
}
