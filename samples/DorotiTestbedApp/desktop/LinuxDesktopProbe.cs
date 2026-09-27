using System.Text.Json;
using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTestbedApp.Desktop;

internal static class LinuxDesktopProbe
{
    internal static async Task RunAsync(DesktopWindowContext context, string path, CancellationToken ct)
    {
        var window = context.Window;
        var states = new Dictionary<string, object>();
        object Snapshot() => new
        {
            size = new[] { window.State.ClientSize.width, window.State.ClientSize.height },
            window.State.Scale, window.State.Visible, window.State.Focused,
            presentation = window.State.PresentationState.ToString(), window.State.Bounds,
        };
        async Task Observe(string name, Func<bool> condition)
        {
            var limit = DateTime.UtcNow.AddSeconds(10);
            DateTime? stableSince = null;
            while (true)
            {
                if (condition())
                {
                    stableSince ??= DateTime.UtcNow;
                    if (DateTime.UtcNow - stableSince.Value >= TimeSpan.FromMilliseconds(200)) break;
                }
                else stableSince = null;
                if (DateTime.UtcNow >= limit) throw new InvalidOperationException("Qt state not observed: " + name);
                await Task.Delay(25, ct);
            }
            states[name] = Snapshot();
            Console.Error.WriteLine("doroti.qt.desktop.probe=" + name);
        }
        await Observe("initial", () => window.State.Visible && window.State.ClientSize == new Size(600, 500));
        await window.SetTitleAsync("Doroti Qt Desktop Probe", ct);
        await window.SetSizeAsync(new Size(520, 460), ct);
        await Observe("resize", () => window.State.ClientSize == new Size(520, 460));
        await window.SetResizableAsync(false, ct);
        await window.SetSizeAsync(new Size(540, 480), ct);
        await Observe("fixedResize", () => window.State.ClientSize == new Size(540, 480));
        await window.SetResizableAsync(true, ct);
        await window.SetMinimumSizeAsync(new Size(320, 280), ct);
        await window.SetMaximumSizeAsync(new Size(1000, 900), ct);
        await window.SetMinimumSizeAsync(null, ct);
        await window.SetMaximumSizeAsync(null, ct);
        // Let the compositor consume the restored size hints before requesting a
        // state whose eligibility depends on them. This only settles the fixture;
        // a timer never marks a product operation successful.
        await Observe("constraintsRestored", () => window.State.ClientSize == new Size(540, 480));
        await window.MaximizeAsync(ct);
        await Observe("maximized", () => window.State.PresentationState == WindowPresentationState.Maximized);
        await window.RestoreAsync(ct);
        await Observe("restored", () => window.State.PresentationState == WindowPresentationState.Normal);
        if (Environment.GetEnvironmentVariable("QT_QPA_PLATFORM") == "wayland")
        {
            try { await window.MinimizeAsync(ct); throw new InvalidOperationException("Wayland minimize unexpectedly supported"); }
            catch (NotSupportedException) { states["waylandMinimizeRejected"] = true; }
        }
        else
        {
            await window.MinimizeAsync(ct);
            await Observe("minimized", () => window.State.PresentationState == WindowPresentationState.Minimized);
            await window.RestoreAsync(ct);
            await Observe("afterMinimized", () => window.State.PresentationState == WindowPresentationState.Normal);
        }
        await window.SetFullScreenAsync(true, ct);
        await Observe("fullscreen", () => window.State.PresentationState == WindowPresentationState.FullScreen);
        await window.SetFullScreenAsync(false, ct);
        await window.HideAsync(ct);
        await Observe("hidden", () => !window.State.Visible);
        await window.ShowAsync(ct);
        await window.FocusAsync(ct);
        await window.SetSizeAsync(new Size(500, 450), ct);
        await Observe("shown", () => window.State.Visible && window.State.ClientSize == new Size(500, 450));
        var rejected = new List<string>();
        async Task Reject(string name, Func<Task> action)
        {
            try { await action(); }
            catch (NotSupportedException) { rejected.Add(name); return; }
            throw new InvalidOperationException(name + " unexpectedly supported");
        }
        await Reject("Center", () => window.CenterAsync(ct));
        await Reject("Bounds", () => window.SetBoundsAsync(Rect.fromLTWH(0, 0, 500, 450), ct));
        await Reject("Topmost", () => window.SetAlwaysOnTopAsync(true, ct));
        await Reject("Taskbar", () => window.SetSkipTaskbarAsync(true, ct));
        await Reject("Drag", () => window.StartDraggingAsync(ct));
        await Reject("AdditionalWindow", () => context.Windows.CreateWindowAsync(new()
        {
            Options = new() { StartupVisibility = WindowStartupVisibility.PlatformDefault },
            Content = WindowContent.FromEntrypoint(() => throw new Exception("Must not allocate content")),
        }, ct));
        var appearance = await window.ApplyAppearanceAsync(window.State.RequestedAppearance with
        {
            BackgroundColor = new Color(0xffeeeeee),
        }, ct);
        if (appearance.Status != WindowApplyStatus.Rejected || window.State.Bounds is not null)
            throw new InvalidOperationException("Qt unsupported contract failed");
        states["rejected"] = rejected;
        states["appearanceRejected"] = true;
        states["canCancelNativeClose"] = window.Capabilities.CanCancelNativeClose;
        var closes = 0;
        var subscription = window.RegisterClosing((_, _) =>
        {
            File.WriteAllText(path + ".close", (++closes).ToString());
            return Task.FromResult(closes == 1 ? WindowCloseDecision.Cancel : WindowCloseDecision.Allow);
        });
        context.Windows.WindowClosed += Closed;
        void Closed(DorotiWindowController closed)
        {
            File.WriteAllText(path + ".closed", JsonSerializer.Serialize(new
            {
                closed.State.Closed, remaining = context.Windows.GetWindows().Count,
                closingCallbacks = closes,
            }));
            context.Windows.WindowClosed -= Closed;
            subscription.Dispose();
        }
        File.WriteAllText(path, JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true }));
        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (!File.Exists(path + ".native.json"))
        {
            if (DateTime.UtcNow >= deadline) throw new TimeoutException("Qt native evidence driver did not respond.");
            await Task.Delay(25, ct);
        }
        if (Environment.GetEnvironmentVariable("DOROTI_QT_DESKTOP_NATIVE_CLOSE") == "1") return;
        if (await window.CloseAsync(ct)) throw new InvalidOperationException("First close was not canceled");
        await window.SetTitleAsync("Doroti Qt close cancellation survived", ct);
        await window.SetSizeAsync(new Size(510, 460), ct);
        await Observe("afterCanceledClose", () => window.State.ClientSize == new Size(510, 460));
        File.WriteAllText(path, JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true }));
        await window.CloseAsync();
    }
}
