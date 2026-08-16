using CommunityToolkit.Maui.Markup;
using Doroti.Host.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DorotiDemoApp.WinUI;

public sealed partial class WindowsMauiApplication : MauiWinUIApplication
{
    public WindowsMauiApplication()
    {
        InitializeComponent();
        UnhandledException += (_, args) => WriteStartupFailure(args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            WriteStartupFailure(args.ExceptionObject as Exception ?? new InvalidOperationException(args.ExceptionObject.ToString()));
    }

    protected override MauiApp CreateMauiApp() => MauiApp.CreateBuilder()
        .UseMauiApp<WindowsBootstrapApplication>()
        .UseMauiCommunityToolkitMarkup()
        .UseSkiaSharp()
        .Build();

    private static void WriteStartupFailure(Exception exception)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
        if (!string.IsNullOrWhiteSpace(path))
        {
            File.WriteAllText(path + ".exception.txt", exception.ToString());
        }
    }
}

internal sealed class WindowsBootstrapApplication : Application
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
