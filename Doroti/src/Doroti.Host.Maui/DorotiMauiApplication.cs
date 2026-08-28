using Doroti.Hosting;
using Microsoft.Extensions.DependencyInjection;
#if MACOS
using Microsoft.Maui.Platforms.MacOS.Essentials;
using Microsoft.Maui.Platforms.MacOS.Hosting;
#else
using SkiaSharp.Views.Maui.Controls.Hosting;
#if MACCATALYST
using SkiaSharp.Views.Maui.Controls;
#elif IOS
using SkiaSharp.Views.Maui.Controls;
#endif
#endif

namespace Doroti.Host.Maui;

public static class DorotiMauiApplicationBuilderExtensions
{
    public static MauiAppBuilder UseDorotiApplication<TStartup>(
        this MauiAppBuilder builder,
        DorotiLaunchContext launchContext)
        where TStartup : IDorotiApplicationStartup, new()
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(launchContext);
        var descriptor = DorotiApplicationFactory.Create<TStartup>(launchContext);
        return builder.UseDorotiApplication(descriptor);
    }

    public static MauiAppBuilder UseDorotiApplication(
        this MauiAppBuilder builder,
        DorotiApplicationDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(descriptor);
        builder
#if MACOS
            .UseMauiAppMacOS<DorotiMauiApplication>()
            .AddMacOSEssentials()
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiMacOSMetalSurface, DorotiMacOSMetalSurfaceHandler>());
#else
            .UseMauiApp<DorotiMauiApplication>()
            .UseSkiaSharp()
#if WINDOWS
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiWindowsDxgiElement, DorotiWindowsDxgiElementHandler>());
#elif MACCATALYST
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<SKGLView, DorotiMacCatalystSkglViewHandler>());
#elif IOS
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<SKGLView, DorotiIosMetalViewHandler>());
#else
            ;
#endif
#endif
        builder.Services.AddSingleton(descriptor);
        return builder;
    }
}

public sealed class DorotiMauiApplication(DorotiApplicationDescriptor descriptor) : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = activationState;
        var title = descriptor.ViewConfiguration.title;
        return new(new ContentPage
        {
            Title = title,
            Content = new DorotiMauiSurface(descriptor),
        })
        {
            Title = title,
        };
    }
}
