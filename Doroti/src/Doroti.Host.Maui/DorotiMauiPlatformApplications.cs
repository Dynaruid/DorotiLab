using Doroti.Hosting;

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
        ConfigurePlatform(builder);
        return builder.UseDorotiApplication(CreateApplicationDescriptor()).Build();
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
#endif
