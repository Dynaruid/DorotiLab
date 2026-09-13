#if IOS && !MACCATALYST
using SKGLView = Doroti.Host.Maui.DorotiSkiaView;
#endif
using Doroti.Hosting;
using Microsoft.Extensions.DependencyInjection;
#if MACOS
using Microsoft.Maui.Platforms.MacOS.Essentials;
using Microsoft.Maui.Platforms.MacOS.Hosting;
using Microsoft.Maui.Platforms.MacOS.Platform;
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
#if ANDROID
        if (DorotiGraphiteView.Enabled)
        {
            var info = global::Android.App.Application.Context.ApplicationInfo!;
            Doroti.Skia.Vulkan.GraphiteNativeLibrary.ConfigureAndroid(info.SourceDir!, info.NativeLibraryDir!, info.SplitSourceDirs);
        }
#elif WINDOWS
        if (WindowsCompositionSurfaceFeature.GraphiteEnabled) WindowsCompositionSurfaceFeature.ConfigureGraphiteLibrary();
#endif
        var descriptor = DorotiApplicationFactory.Create<TStartup>(launchContext);
        return builder.UseDorotiApplication(descriptor);
    }

    public static MauiAppBuilder UseDorotiApplication(
        this MauiAppBuilder builder,
        DorotiApplicationDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(descriptor);
#if ANDROID
        if (DorotiGraphiteView.Enabled)
        {
            var info = global::Android.App.Application.Context.ApplicationInfo!;
            Doroti.Skia.Vulkan.GraphiteNativeLibrary.ConfigureAndroid(info.SourceDir!, info.NativeLibraryDir!, info.SplitSourceDirs);
        }
#elif WINDOWS
        if (WindowsCompositionSurfaceFeature.GraphiteEnabled) WindowsCompositionSurfaceFeature.ConfigureGraphiteLibrary();
#endif
        builder
#if MACOS
            .UseMauiAppMacOS<DorotiMauiApplication>()
            .AddMacOSEssentials()
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiMacOSMetalSurface, DorotiMacOSMetalSurfaceHandler>()
                    .AddHandler<DorotiMauiSurface, DorotiMacOSLayoutHandler>());
#else
            .UseMauiApp<DorotiMauiApplication>()
#if !IOS || MACCATALYST
            .UseSkiaSharp()
#endif
#if WINDOWS
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiWindowsDxgiElement, DorotiWindowsDxgiElementHandler>());
#elif MACCATALYST
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<SKGLView, DorotiMacCatalystSkglViewHandler>()
                    .AddHandler<DorotiGraphiteView, DorotiUIKitGraphiteViewHandler>());
#elif IOS
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiUIKitEntry, DorotiUIKitEntryHandler>()
                    .AddHandler<DorotiUIKitEditor, DorotiUIKitEditorHandler>()
                    .AddHandler<SKGLView, DorotiIosMetalViewHandler>()
                    .AddHandler<DorotiGraphiteView, DorotiUIKitGraphiteViewHandler>());
#elif ANDROID
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiGraphiteView, DorotiAndroidVulkanViewHandler>());
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
        var window = new Window(new ContentPage
        {
#if MACOS
            BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent,
#endif
            SafeAreaEdges = Microsoft.Maui.SafeAreaEdges.None,
            Title = title,
            Content = new DorotiMauiSurface(descriptor),
        })
        {
            Title = title,
        };
#if MACOS
        // Allow the native backdrop to cover the titlebar. The AppKit page
        // container keeps interactive content below the window controls.
        // AppKitWindowBackdrop applies the requested unified or solid titlebar
        // without changing content layout when the option changes.
        MacOSWindow.SetFullSizeContentView(window, true);
        MacOSWindow.SetTitlebarTransparent(window, false);
        MacOSWindow.SetTitleVisibility(window, MacOSTitleVisibility.Visible);
#endif
        return window;
    }
}
