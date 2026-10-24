using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTestbedApp.Desktop;

public sealed class LinuxDesktopStartup : IDorotiDesktopApplicationStartup
{
    public void Configure(DesktopApplicationBuilder desktop)
    {
        if (Environment.GetEnvironmentVariable("DOROTI_DESKTOP_LIFETIME") == "Explicit")
            desktop.LifetimePolicy = WindowLifetimePolicy.Explicit;
        desktop.UseMainWindow(desktop.DefaultMainWindow with
        {
            Options = new WindowOptions
            {
                Title = "Doroti Linux Desktop",
                Size = new Size(600, 500),
                MinimumSize = new Size(350, 300),
                StartupVisibility = WindowStartupVisibility.PlatformDefault,
                Appearance = new() { BackgroundColor = new Color(0xffffffff) },
            },
            OnCreated = async (context, ct) =>
            {
                await context.Window.WaitUntilReadyToShowAsync(ct);
            },
        });
    }
}
