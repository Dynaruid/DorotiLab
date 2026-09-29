using System.Text.Json;
using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTestbedApp.Desktop;

internal static class LinuxMultiWindowProbe
{
    internal static async Task RunAsync(DesktopWindowContext main, WindowCreateOptions template, string path)
    {
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var ct = deadline.Token;
        var exited = 0;
        main.Windows.ExitRequested += () => Interlocked.Increment(ref exited);
        async Task<DorotiWindowController> Create(string title) => await main.Windows.CreateWindowAsync(template with
        {
            Options = template.Options with { Title = title, Size = new(560, 500), MinimumSize = new(300, 250), StartupVisibility = WindowStartupVisibility.PlatformDefault },
            OnCreated = null,
        }, ct);
        async Task Size(DorotiWindowController window, double width, double height)
        {
            await window.SetSizeAsync(new(width, height), ct);
            while (window.State.ClientSize != new Size(width, height)) await Task.Delay(25, ct);
        }
        var second = await Create("Doroti Qt second");
        await second.WaitUntilReadyToShowAsync(ct);
        await Size(main.Window, 470, 450);
        await Size(second, 580, 520);
        if (main.Windows.GetWindows().Count != 2 || second.Id == main.Window.Id) throw new Exception("Missing independent native windows.");
        File.WriteAllText(path + ".ready", "two windows");
        // The native evidence driver captures both rendered surfaces before teardown.
        while (!File.Exists(path + ".native.json")) await Task.Delay(25, ct);
        var canceled = main.Window.RegisterClosing((_, _) => Task.FromResult(WindowCloseDecision.Cancel));
        if (await main.Window.CloseAsync(ct)) throw new Exception("Close cancellation failed.");
        canceled.Dispose();
        await main.Window.CloseAsync(ct);
        if (second.State.Closed || main.Windows.GetWindows().Count != 1) throw new Exception("Closing main closed survivor.");
        await Size(second, 600, 540);
        var third = await Create("Doroti Qt after main close");
        await third.WaitUntilReadyToShowAsync(ct);
        await third.CloseAsync(ct);
        await second.CloseAsync(ct);
        if (main.Windows.GetWindows().Count != 0) throw new Exception("Window registry not drained.");
        if (main.Windows.LifetimePolicy == WindowLifetimePolicy.Explicit)
        {
            if (exited != 0) throw new Exception("Explicit lifetime exited with the last window.");
            // Explicit keeps QApplication alive even with no remaining window.
            var reopened = await Create("Doroti Qt explicit reopen");
            await reopened.WaitUntilReadyToShowAsync(ct);
            await reopened.CloseAsync(ct);
            await Task.WhenAll(main.Windows.RequestExitAsync(ct), main.Windows.RequestExitAsync(ct));
        }
        if (exited != 1) throw new Exception("Expected one application exit.");
        File.WriteAllText(path, JsonSerializer.Serialize(new { windows = 3, mainCloseCanceled = true,
            survivorResize = true, createdAfterMainClosed = true, remaining = main.Windows.GetWindows().Count,
            lifetime = main.Windows.LifetimePolicy.ToString(), exits = exited }));
    }
}
