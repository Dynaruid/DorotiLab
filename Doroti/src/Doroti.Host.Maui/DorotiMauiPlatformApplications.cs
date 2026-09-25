using Doroti.Hosting;
#if WINDOWS || MACCATALYST
using Microsoft.Maui.LifecycleEvents;
#endif
#if MACCATALYST
using Microsoft.Maui.Platform;
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
#if IOS || MACCATALYST
    public override UIKit.UISceneConfiguration GetConfiguration(
        UIKit.UIApplication application,
        UIKit.UISceneSession connectingSceneSession,
        UIKit.UISceneConnectionOptions options
    )
    {
        var configuration = base.GetConfiguration(application, connectingSceneSession, options);
        // A direct type reference keeps the registered delegate in trimmed/AOT apps.
#if MACCATALYST
        configuration.DelegateClass = new ObjCRuntime.Class(typeof(DorotiMacCatalystSceneDelegate));
#else
        configuration.DelegateClass = new ObjCRuntime.Class(typeof(DorotiMauiSceneDelegate));
#endif
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
#if MACCATALYST
        builder.ConfigureLifecycleEvents(events =>
            events.AddiOS(ios =>
                ios.SceneWillConnect(
                    (scene, session, options) =>
                    {
                        // A previously saved implicit UIKit scene can have no configuration
                        // name and retain MAUI's base delegate. Its WillConnect only creates
                        // a window for MAUI's named configuration. Complete that missing path
                        // using the public MAUI scene API, without replacing the native delegate.
                        if (
                            session.Configuration.Name != "__MAUI_DEFAULT_SCENE_CONFIGURATION__"
                            && scene.Delegate is MauiUISceneDelegate { Window: null } sceneDelegate
                            && IPlatformApplication.Current?.Application is { } app
                        )
                        {
                            sceneDelegate.CreatePlatformWindow(app, scene, session, options);
                            if (sceneDelegate.Window is { } native)
                                app.Windows.FirstOrDefault(window =>
                                        ReferenceEquals(window.Handler?.PlatformView, native)
                                    )
                                    ?.Created();
                        }
                    }
                )
            )
        );
#endif
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
public abstract class DorotiMacOSMauiApplication : MacOSMauiApplication, IPlatformApplication
{
    private bool _terminateAfterLastWindowClosed;
    private DorotiApplicationDescriptor? _descriptor;
    private IApplication? _desktopApplication;
    private Doroti.Desktop.DorotiWindowManager? _desktopManager;
    private bool _terminationPending;
    IApplication IPlatformApplication.Application => _desktopApplication ?? base.Application;

    public override void DidFinishLaunching(Foundation.NSNotification notification)
    {
        _descriptor = CreateApplicationDescriptor();
        if (!Doroti.Desktop.DesktopApplication.TryGetDefinition(_descriptor, out _))
        {
            base.DidFinishLaunching(notification);
            return;
        }
        // The pinned preview creates WindowHandler directly and shows before mapping
        // content. Own only the opted-in Desktop launch; legacy launch stays upstream.
        IPlatformApplication.Current = this;
        var app = CreateMauiApp();
        var context = new MacOSMauiContext(app.Services).MakeApplicationScope(this);
        Services = context.Services;
        _desktopApplication =
            Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<IApplication>(
                Services
            );
        Microsoft.Maui.Platforms.MacOS.Handlers.MenuBarManager.SetupDefaultMenuBar(
            Services.GetService(typeof(MacOSMenuBarOptions)) as MacOSMenuBarOptions
        );
        var applicationHandler = new Microsoft.Maui.Platforms.MacOS.Handlers.ApplicationHandler();
        applicationHandler.SetMauiContext(context);
        applicationHandler.SetVirtualView(_desktopApplication);
        var window = _desktopApplication.CreateWindow(new ActivationState(context));
        var handler = new AppKitDesktopWindowHandler();
        handler.SetMauiContext(context);
        handler.SetVirtualView(window);
        window.Created();
        OnStarted();
    }

    internal void AttachDesktopManager(Doroti.Desktop.DorotiWindowManager manager)
    {
        _desktopManager = manager;
        manager.ExitRequested += () =>
            NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
            {
                if (!_terminationPending)
                    NSApplication.SharedApplication.Terminate(this);
            });
    }

    public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
    {
        if (_desktopManager is null || _desktopManager.GetWindows().Count == 0)
            return NSApplicationTerminateReply.Now;
        if (!_terminationPending)
        {
            _terminationPending = true;
            _ = DecideTerminationAsync(sender);
        }
        return NSApplicationTerminateReply.Later;
    }

    private async Task DecideTerminationAsync(NSApplication application)
    {
        // Reply only after AppKit has received Later, even if close completes inline.
        await Task.Yield();
        var allow = true;
        try
        {
            foreach (var window in _desktopManager!.GetWindows())
                if (!await window.CloseAsync())
                {
                    allow = false;
                    break;
                }
        }
        catch (Exception error)
        {
            allow = false;
            DorotiMauiSurface.WriteFailure(error);
        }
        application.BeginInvokeOnMainThread(() =>
        {
            _terminationPending = false;
            application.ReplyToApplicationShouldTerminate(allow);
        });
    }

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
        var descriptor = _descriptor ??= CreateApplicationDescriptor();
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
        return _desktopManager is null && _terminateAfterLastWindowClosed;
    }

    protected abstract DorotiApplicationDescriptor CreateApplicationDescriptor();

    protected virtual void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
#endif
