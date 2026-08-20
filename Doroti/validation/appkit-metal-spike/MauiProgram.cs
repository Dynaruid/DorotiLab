using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platforms.MacOS.Essentials;
using Microsoft.Maui.Platforms.MacOS.Hosting;

namespace Doroti.Validation.AppKitMetalSpike;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiAppMacOS<App>()
            .AddMacOSEssentials()
            .UseMauiCommunityToolkitMarkup()
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiMetalSurface, DorotiMetalSurfaceHandler>());
        return builder.Build();
    }
}
