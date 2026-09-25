using Doroti.Hosting;
#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
#endif
#if MACOS
using AppKit;
using Microsoft.Maui.Platforms.MacOS.Platform;
#endif

namespace Doroti.Host.Maui;

#if WINDOWS
public abstract class DorotiMauiWinUIApplication : MauiWinUIApplication
{
    protected DorotiMauiWinUIApplication()
    {
        UnhandledException += (_, args) => WriteStartupFailure(args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            WriteStartupFailure(
                args.ExceptionObject as Exception
                    ?? new InvalidOperationException(args.ExceptionObject.ToString())
            );
    }

    protected sealed override MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var descriptor = CreateApplicationDescriptor();
        var desktopManaged = Doroti.Desktop.DesktopApplication.TryGetDefinition(descriptor, out _);
        builder.ConfigureLifecycleEvents(events =>
            events.AddWindows(windows =>
                windows
                    .OnPlatformWindowSubclassed(
                        (window, _) =>
                        {
                            if (WindowsCompositionSurfaceFeature.Enabled)
                            {
                                WindowsNativeCaption.Enable(window);
                            }
                            if (desktopManaged)
                                WindowsDesktopWindowHost.PrepareNativeWindow(window);
                        }
                    )
                    .OnWindowCreated(window =>
                    {
                        if (WindowsNativeCaption.IsEnabled(window))
                            window.AppWindow.Title = window.Title;
                        if (!desktopManaged)
                            window.Closed += HandlePlatformWindowClosed;
                    })
            )
        );
        ConfigurePlatform(builder);
        return builder.UseDorotiApplication(descriptor).Build();
    }

    private static void HandlePlatformWindowClosed(
        object sender,
        Microsoft.UI.Xaml.WindowEventArgs args
    )
    {
        _ = sender;
        _ = args;
        // The official Graphite close path drains workers while the dispatcher
        // is alive. Do not turn incomplete cleanup into exit-code-zero success.
        Microsoft.UI.Xaml.Application.Current.Exit();
    }

    protected abstract DorotiApplicationDescriptor CreateApplicationDescriptor();

    protected virtual void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;

    private static void WriteStartupFailure(Exception exception)
    {
        DorotiMauiSurface.WriteFailure(exception);
    }
}
#elif IOS || MACCATALYST
public abstract class DorotiMauiUIApplicationDelegate : MauiUIApplicationDelegate
{
#if IOS && !MACCATALYST
    public override UIKit.UISceneConfiguration GetConfiguration(
        UIKit.UIApplication application,
        UIKit.UISceneSession connectingSceneSession,
        UIKit.UISceneConnectionOptions options
    )
    {
        var configuration = base.GetConfiguration(application, connectingSceneSession, options);
        // A direct type reference keeps the registered delegate in trimmed/AOT apps.
        configuration.DelegateClass = new ObjCRuntime.Class(typeof(DorotiMauiSceneDelegate));
        return configuration;
    }
#endif

    protected DorotiMauiUIApplicationDelegate()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            DorotiMauiSurface.WriteFailure(
                args.ExceptionObject as Exception
                    ?? new InvalidOperationException(args.ExceptionObject.ToString())
            );
    }

    protected sealed override MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        ConfigurePlatform(builder);
        return builder.UseDorotiApplication(CreateApplicationDescriptor()).Build();
    }

    protected abstract DorotiApplicationDescriptor CreateApplicationDescriptor();

    protected virtual void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
#elif ANDROID
public abstract class DorotiMauiAndroidApplication(
    IntPtr handle,
    Android.Runtime.JniHandleOwnership ownership
) : MauiApplication(handle, ownership)
{
    protected sealed override MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        ConfigurePlatform(builder);
        return builder.UseDorotiApplication(CreateApplicationDescriptor()).Build();
    }

    protected abstract DorotiApplicationDescriptor CreateApplicationDescriptor();

    protected virtual void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
#elif MACOS
public abstract class DorotiMacOSMauiApplication : MacOSMauiApplication
{
    private bool _terminateAfterLastWindowClosed;

    protected DorotiMacOSMauiApplication()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            DorotiMauiSurface.WriteFailure(
                args.ExceptionObject as Exception
                    ?? new InvalidOperationException(args.ExceptionObject.ToString())
            );
    }

    protected sealed override MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var descriptor = CreateApplicationDescriptor();
        _terminateAfterLastWindowClosed = descriptor
            .ViewConfiguration
            .terminateAfterLastWindowClosed;
        ConfigurePlatform(builder);
        return builder.UseDorotiApplication(descriptor).Build();
    }

    public sealed override bool ApplicationShouldTerminateAfterLastWindowClosed(
        NSApplication sender
    )
    {
        _ = sender;
        return _terminateAfterLastWindowClosed;
    }

    protected abstract DorotiApplicationDescriptor CreateApplicationDescriptor();

    protected virtual void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
#endif
