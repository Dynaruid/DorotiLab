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
            WriteStartupFailure(args.ExceptionObject as Exception ?? new InvalidOperationException(args.ExceptionObject.ToString()));
    }

    protected sealed override MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.ConfigureLifecycleEvents(events =>
            events.AddWindows(windows => windows.OnWindowCreated(window =>
                window.Closed += HandlePlatformWindowClosed)));
        ConfigurePlatform(builder);
        return builder.UseDorotiApplication(CreateApplicationDescriptor()).Build();
    }

    private static void HandlePlatformWindowClosed(
        object sender,
        Microsoft.UI.Xaml.WindowEventArgs args)
    {
        _ = sender;
        _ = args;
        // The Doroti Windows runner owns one window. Angle's render worker can
        // still be waiting on the dispatcher after that window closes, so the
        // normal WinUI exit request alone may not let `dotnet run` return.
        Microsoft.UI.Xaml.Application.Current.Exit();
        Environment.Exit(0);
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
    protected DorotiMauiUIApplicationDelegate()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            DorotiMauiSurface.WriteFailure(args.ExceptionObject as Exception ??
                new InvalidOperationException(args.ExceptionObject.ToString()));
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
public abstract class DorotiMauiAndroidApplication(IntPtr handle, Android.Runtime.JniHandleOwnership ownership)
    : MauiApplication(handle, ownership)
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
            DorotiMauiSurface.WriteFailure(args.ExceptionObject as Exception ??
                new InvalidOperationException(args.ExceptionObject.ToString()));
    }

    protected sealed override MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var descriptor = CreateApplicationDescriptor();
        _terminateAfterLastWindowClosed = descriptor.ViewConfiguration.terminateAfterLastWindowClosed;
        ConfigurePlatform(builder);
        return builder.UseDorotiApplication(descriptor).Build();
    }

    public sealed override bool ApplicationShouldTerminateAfterLastWindowClosed(
        NSApplication sender)
    {
        _ = sender;
        return _terminateAfterLastWindowClosed;
    }

    protected abstract DorotiApplicationDescriptor CreateApplicationDescriptor();

    protected virtual void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
#endif
