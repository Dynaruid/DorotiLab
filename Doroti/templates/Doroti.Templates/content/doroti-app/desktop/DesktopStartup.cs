using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTemplateApp.Desktop;

public sealed class DesktopStartup : IDorotiDesktopApplicationStartup
{
    public void Configure(DesktopApplicationBuilder desktop) =>
        desktop.UseMainWindow(
            desktop.LegacyMainWindow with
            {
                Options = new WindowOptions
                {
                    Title = "DorotiTemplateApp",
                    Size = new Size(450, 800),
                    MinimumSize = new Size(350, 500),
                    Centered = true,
                },
            }
        );
}
