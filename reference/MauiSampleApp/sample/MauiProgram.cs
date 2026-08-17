using CommunityToolkit.Maui.Markup;

namespace MauiSampleApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp.CreateBuilder()
        .UseMauiApp<DorotiMauiApplication>()
        .UseMauiCommunityToolkitMarkup()
        .Build();
}
