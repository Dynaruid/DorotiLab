using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTemplateApp.Desktop;

public sealed class LinuxDesktopStartup : IDorotiDesktopApplicationStartup
{
    public void Configure(DesktopApplicationBuilder desktop)
    {
        desktop.UseMainWindow(desktop.LegacyMainWindow with
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
                // Qt/compositor activation remains a request; read State.Focused.
                await context.Window.FocusAsync(ct);
            },
        });
    }
}
