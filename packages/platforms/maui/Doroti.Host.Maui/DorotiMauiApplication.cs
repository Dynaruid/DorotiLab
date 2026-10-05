using Doroti.Hosting;
using Microsoft.Extensions.DependencyInjection;
#if IOS && !MACCATALYST
using SKGLView = Doroti.Host.Maui.DorotiSkiaView;
#endif

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
    private static void ValidateFrameConfiguration()
    {
        try
        {
            Doroti.Skia.Rendering.NativeFrameConfiguration.ValidateEnvironment();
#if ANDROID
            Doroti.Skia.Rendering.NativeFrameConfiguration.ValidateSettings(name =>
                Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Intent?.GetStringExtra(name));
#endif
        }
        catch (ArgumentException error)
        {
            Console.Error.WriteLine(error.Message);
            DorotiMauiSurface.WriteFailure(error);
            throw;
        }
    }

    internal static void PrepareProcess()
    {
        ValidateFrameConfiguration();
#if ANDROID
        if (DorotiGraphiteView.Enabled)
        {
            var info = Android.App.Application.Context.ApplicationInfo!;
            Doroti.Skia.Vulkan.GraphiteNativeLibrary.ConfigureAndroid(info.SourceDir!, info.NativeLibraryDir!, info.SplitSourceDirs);
        }
#elif WINDOWS
        if (WindowsCompositionSurfaceFeature.GraphiteEnabled) WindowsCompositionSurfaceFeature.ConfigureGraphiteLibrary();
#endif
    }

    /// <summary>Prepare the native provider before application startup constructs its descriptor.</summary>
    public static MauiAppBuilder UseDorotiApplication(this MauiAppBuilder builder,
        Func<DorotiApplicationDescriptor> configureApplication)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configureApplication);
        PrepareProcess();
        return builder.ConfigureDorotiApplication(configureApplication());
    }

    public static MauiAppBuilder UseDorotiApplication<TStartup>(
        this MauiAppBuilder builder,
        DorotiLaunchContext launchContext
    )
        where TStartup : IDorotiApplicationStartup, new()
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(launchContext);
        return builder.UseDorotiApplication(() => DorotiApplicationFactory.Create<TStartup>(launchContext));
    }

    internal static MauiAppBuilder ConfigureDorotiApplication(
        this MauiAppBuilder builder,
        DorotiApplicationDescriptor descriptor
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(descriptor);
        builder
#if MACOS
            .UseMauiAppMacOS<DorotiMauiApplication>()
            .AddMacOSEssentials()
            .ConfigureMauiHandlers(handlers =>
                handlers
                    .AddHandler<DorotiMacOSMetalSurface, DorotiMacOSMetalSurfaceHandler>()
                    .AddHandler<DorotiMauiSurface, DorotiMacOSLayoutHandler>()
                    .AddHandler<AbsoluteLayout, DorotiMacOSLayoutHandler>()
                    .AddHandler<MauiSemanticsLayout, MauiSemanticsLayoutHandler>()
            );
#else
            .UseMauiApp<DorotiMauiApplication>()
#if !IOS || MACCATALYST
            .UseSkiaSharp()
#endif
#if WINDOWS
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiWindowsDxgiElement, DorotiWindowsDxgiElementHandler>()
                    .AddHandler<MauiSemanticsLayout, MauiSemanticsLayoutHandler>()
            );
#elif MACCATALYST
            .ConfigureMauiHandlers(handlers =>
                handlers
                    .AddHandler<DorotiUIKitEntry, DorotiUIKitEntryHandler>()
                    .AddHandler<DorotiUIKitEditor, DorotiUIKitEditorHandler>()
                    .AddHandler<MauiSemanticsLayout, MauiSemanticsLayoutHandler>()
                    .AddHandler<SKGLView, DorotiMacCatalystSkglViewHandler>()
                    .AddHandler<DorotiGraphiteView, DorotiUIKitGraphiteViewHandler>()
            );
#elif IOS
            .ConfigureMauiHandlers(handlers =>
                handlers
                    .AddHandler<DorotiUIKitEntry, DorotiUIKitEntryHandler>()
                    .AddHandler<DorotiUIKitEditor, DorotiUIKitEditorHandler>()
                    .AddHandler<MauiSemanticsLayout, MauiSemanticsLayoutHandler>()
                    .AddHandler<SKGLView, DorotiIosMetalViewHandler>()
                    .AddHandler<DorotiGraphiteView, DorotiUIKitGraphiteViewHandler>()
            );
#elif ANDROID
            .ConfigureMauiHandlers(handlers =>
                handlers.AddHandler<DorotiGraphiteView, DorotiAndroidVulkanViewHandler>()
            );
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
    private DorotiSharedHostSession? _framework;
    private DorotiApplicationBoundary? _application;
    private readonly HashSet<DorotiMauiSurface> _surfaces = [];
    private Task? _stop;
    private bool _stopping;
#if ANDROID
    private DorotiMauiSurface? _androidSurface;
#endif
    private DorotiSharedHostSession Framework => _framework ??= new(descriptor, new MauiApplicationDispatcher());
    private void AttachApplication(DorotiApplicationBoundary boundary) => _application ??= boundary.Retain();
#if MACCATALYST
    internal bool UsesDesktop =>
        Doroti.Desktop.DesktopApplication.TryGetDefinition(descriptor, out _);
#endif

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (_stopping) throw new InvalidOperationException("The MAUI application is stopping.");
        _ = activationState;
#if WINDOWS
        if (Doroti.Desktop.DesktopApplication.TryGetDefinition(descriptor, out var desktop))
            return WindowsDesktopWindowHost.CreateMainWindow(descriptor, desktop!);
#elif MACOS
        if (Doroti.Desktop.DesktopApplication.TryGetDefinition(descriptor, out var desktop))
            return AppKitDesktopWindowHost.CreateMainWindow(descriptor, desktop!);
#elif MACCATALYST
        if (Doroti.Desktop.DesktopApplication.TryGetDefinition(descriptor, out var desktop))
        {
            if (!OperatingSystem.IsMacCatalystVersionAtLeast(16))
                throw new PlatformNotSupportedException(
                    "The Catalyst Desktop adapter requires Mac Catalyst 16 or later."
                );
            if (Windows.Count > 0)
                throw new NotSupportedException("Create additional Catalyst windows through DesktopWindowContext.Windows.");
            return MacCatalystDesktopWindowHost.CreateMainWindow(descriptor, desktop!);
        }
#endif
        var title = descriptor.ViewConfiguration.title;
        var surface =
#if ANDROID
            _androidSurface ??
#endif
            new DorotiMauiSurface(descriptor, Framework.AllocateViewId())
        {
            SharedFramework = Framework,
            SharedApplication = _application,
            SharedApplicationFactory = () => _application,
            ApplicationAttached = AttachApplication,
            OwnsApplicationActivation = Windows.Count == 0,
            SurfaceDisposed = value =>
            {
                _surfaces.Remove(value);
#if ANDROID
                if (ReferenceEquals(_androidSurface, value)) _androidSurface = null;
#endif
            },
#if WINDOWS
            OwnsWindowContent = true,
#endif
        };
#if ANDROID
        _androidSurface = surface;
        // Android can replace its Activity/Window while the logical view and
        // widget State remain alive. Move the existing MAUI content to the new
        // OS shell, retaining framework, IME and navigation ownership.
        if (surface.Parent is ContentPage previous && ReferenceEquals(previous.Content, surface))
            previous.Content = null;
#endif
        _surfaces.Add(surface);
        var window = new Window(
            new ContentPage
            {
#if MACOS || WINDOWS
                BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent,
#endif
                SafeAreaEdges = Microsoft.Maui.SafeAreaEdges.None,
                Title = title,
                Content = surface,
            }
        )
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

    /// <summary>Stops scene-owned views and finally releases the shared mobile application tree.</summary>
    public Task StopAsync()
    {
        if (!Microsoft.Maui.ApplicationModel.MainThread.IsMainThread)
            throw new InvalidOperationException("Request application shutdown on the native main thread.");
#if WINDOWS || MACOS || MACCATALYST
        if (Doroti.Desktop.DesktopApplication.TryGetDefinition(descriptor, out _))
            throw new NotSupportedException("Desktop shutdown uses DesktopWindowContext.Windows.RequestExitAsync.");
#endif
        if (_stop is not null && !_stop.IsFaulted) return _stop;
        _stopping = true;
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _stop = completion.Task;
        _ = CompleteStopAsync(completion);
        return _stop;
    }

    private async Task CompleteStopAsync(TaskCompletionSource completion)
    {
        try { await StopCoreAsync(); completion.TrySetResult(); }
        catch (Exception error) { completion.TrySetException(error); }
    }

    private async Task StopCoreAsync()
    {
        foreach (var surface in _surfaces.ToArray().Reverse()) await surface.DisposeAsync();
        await new MauiApplicationDispatcher().InvokeAsync(() =>
            Doroti.Runtime.DorotiCleanup.Run(() => _framework?.Dispose(), () => _application?.Dispose()));
    }
}
