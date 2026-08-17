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
        var path = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
        if (!string.IsNullOrWhiteSpace(path)) File.WriteAllText(path + ".exception.txt", exception.ToString());
    }
}
#elif MACCATALYST
public abstract class DorotiMauiUIApplicationDelegate<TStartup> : MauiUIApplicationDelegate
    where TStartup : IDorotiApplicationStartup, new()
{
    protected sealed override MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        ConfigurePlatform(builder);
        return builder.UseDorotiApplication<TStartup>(DorotiLaunchContext.Create(
            "MacCatalyst", "maccatalyst-arm64", Environment.GetCommandLineArgs().Skip(1))).Build();
    }

    protected virtual void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
#endif
