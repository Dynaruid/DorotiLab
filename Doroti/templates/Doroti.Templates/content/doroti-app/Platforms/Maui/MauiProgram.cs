using CommunityToolkit.Maui.Markup;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DorotiApp.Platforms.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp.CreateBuilder()
        .UseMauiApp<DorotiMauiApplication>()
        .UseMauiCommunityToolkitMarkup()
        .UseSkiaSharp()
        .Build();
}
