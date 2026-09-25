using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTemplateApp.Desktop;

public sealed class MacCatalystDesktopStartup : IDorotiDesktopApplicationStartup
{
    public void Configure(DesktopApplicationBuilder desktop)
    {
        desktop.LifetimePolicy = WindowLifetimePolicy.Explicit;
        desktop.UseMainWindow(
            desktop.LegacyMainWindow with
            {
                Options = new WindowOptions
                {
                    Title = "DorotiTemplateApp",
                    Size = new Size(600, 500),
                    MinimumSize = new Size(350, 300),
                    StartupVisibility = WindowStartupVisibility.PlatformDefault,
                },
            }
        );
    }
}
