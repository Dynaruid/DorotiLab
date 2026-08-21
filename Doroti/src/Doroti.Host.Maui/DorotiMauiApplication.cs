using CommunityToolkit.Maui.Markup;
using Doroti.Hosting;
using Microsoft.Extensions.DependencyInjection;
#if MACOS
using Microsoft.Maui.Platforms.MacOS.Essentials;
using Microsoft.Maui.Platforms.MacOS.Hosting;
#else
using SkiaSharp.Views.Maui.Controls.Hosting;
#if WINDOWS
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
            .UseMauiCommunityToolkitMarkup()
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiMacOSMetalSurface, DorotiMacOSMetalSurfaceHandler>());
#else
            .UseMauiApp<DorotiMauiApplication>()
            .UseMauiCommunityToolkitMarkup()
            .UseSkiaSharp();
#if WINDOWS
        builder.ConfigureMauiHandlers(handlers =>
            handlers.AddHandler<SKGLView, DorotiWindowsSkiaViewHandler>());
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
        return new(new ContentPage
        {
            Title = descriptor.ViewConfiguration.title,
            Content = new DorotiMauiSurface(descriptor),
        });
    }
}
