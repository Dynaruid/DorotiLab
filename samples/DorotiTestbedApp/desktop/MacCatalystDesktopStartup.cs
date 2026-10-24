using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTestbedApp.Desktop;

/// <summary>UIKit scene defaults are explicit; this sample does not inherit AppKit materials.</summary>
public sealed class MacCatalystDesktopStartup : IDorotiDesktopApplicationStartup
{
    public void Configure(DesktopApplicationBuilder desktop)
    {
        desktop.LifetimePolicy = WindowLifetimePolicy.Explicit;
        desktop.UseMainWindow(
            desktop.DefaultMainWindow with
            {
                Options = new WindowOptions
                {
                    Title = "Doroti Catalyst Desktop",
                    Size = new Size(600, 500),
                    MinimumSize = new Size(350, 300),
                    StartupVisibility = WindowStartupVisibility.PlatformDefault,
                    Appearance = new() { BackgroundColor = new Color(0xffffffff) },
                },
                OnCreated = async (context, ct) =>
                {
                    await context.Window.WaitUntilReadyToShowAsync(ct);
                },
            }
        );
    }
}
