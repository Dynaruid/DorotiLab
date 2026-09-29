using System.Text.Json;
using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTestbedApp.Desktop;

internal static class MultiWindowProbe
{
    internal static async Task RunAsync(DesktopWindowContext main, WindowCreateOptions template, string path)
    {
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var ct = deadline.Token;
        var second = await main.Windows.CreateWindowAsync(template with
        {
            Options = template.Options with { Title = "Doroti second window", Size = new Size(560, 600),
                StartupVisibility = template.Options.StartupVisibility },
            OnCreated = null,
        }, ct);
        await second.WaitUntilReadyToShowAsync(ct);
        await second.ShowAsync(ct);
        if (main.Windows.GetWindows().Count != 2 || second.Id == main.Window.Id) throw new Exception("Two native windows were not registered.");
        await main.Window.SetTitleAsync("Doroti first window", ct);
        await main.Window.SetSizeAsync(new Size(470, 650), ct);
        await second.SetSizeAsync(new Size(580, 620), ct);
        var before = new { first = new[] { main.Window.State.ClientSize.width, main.Window.State.ClientSize.height },
            second = new[] { second.State.ClientSize.width, second.State.ClientSize.height },
            firstScale = main.Window.State.Scale, secondScale = second.State.Scale };
        await main.Window.CloseAsync(ct);
        if (second.State.Closed || main.Windows.GetWindows().Count != 1) throw new Exception("Closing first window closed its survivor.");
        await second.SetTitleAsync("Doroti survivor", ct);
        await second.SetSizeAsync(new Size(600, 640), ct);
        await second.CloseAsync(ct);
        if (main.Windows.GetWindows().Count != 0) throw new Exception("Window registry was not drained.");
        if (main.Windows.LifetimePolicy == WindowLifetimePolicy.Explicit && !await main.Windows.RequestExitAsync(ct))
            throw new Exception("Explicit exit was rejected.");
        File.WriteAllText(path, JsonSerializer.Serialize(new { before, survivor = second.State.Closed,
            remaining = main.Windows.GetWindows().Count, lifetime = main.Windows.LifetimePolicy.ToString() }));
    }
}
