#if MACOS
using AppKit;
using Doroti.Host.Maui;
using Doroti.Ui;
using System.Text.Json;
using MauiApplication = Microsoft.Maui.Controls.Application;
using Size = Doroti.Ui.Size;
using Rect = Doroti.Ui.Rect;

namespace DorotiTestbedApp.MacOS;

internal static class AppKitWindowingProbe
{
    internal static async Task RunAsync(string path)
    {
        try
        {
            DorotiView? primaryView = null;
            for (var attempt = 0; attempt < 100 && primaryView is null; attempt++)
            {
                await AppKitUi.Invoke(() => primaryView = MauiApplication.Current!.Windows
                    .Select(window => (window.Page as ContentPage)?.Content).OfType<DorotiMauiSurface>()
                    .Select(surface => surface.FrameworkView).FirstOrDefault(view => view is not null));
                if (primaryView is null) await Task.Delay(50);
            }
            var primary = primaryView ?? throw new TimeoutException("The primary view did not attach.");
            var windows = primary.GetCapabilityOrDefault<IWindowService>(DorotiCapabilityIds.WindowService)
                ?? throw new InvalidOperationException("The primary view has no window service.");
            var primaryRegistered = new TaskCompletionSource<WindowSnapshot>(TaskCreationOptions.RunContinuationsAsynchronously);
            void Changed(WindowEvent value)
            { if (value.Window.Kind == WindowKind.Regular && !value.Window.Closed) primaryRegistered.TrySetResult(value.Window); }
            windows.Changed += Changed;
            WindowSnapshot initial;
            try
            {
                if (windows.GetWindows().FirstOrDefault(window => window.Kind == WindowKind.Regular) is { } registered)
                    primaryRegistered.TrySetResult(registered);
                initial = await primaryRegistered.Task.WaitAsync(TimeSpan.FromSeconds(15));
            }
            finally { windows.Changed -= Changed; }
            var owner = await windows.CreateAsync(new(WindowKind.Regular, "Work3 owner", new Size(800, 700)));
            await windows.ExecuteAsync(new(owner.Id, WindowAction.Show));
            var checks = new Dictionary<string, object>();
            foreach (var kind in new[] { WindowKind.Dialog, WindowKind.Popup, WindowKind.Tooltip, WindowKind.Satellite })
            {
                var request = new WindowRequest(kind, "Work3 " + kind, new Size(160, 100), owner.Id,
                    kind is WindowKind.Popup or WindowKind.Tooltip ? new(owner.Id, new Rect(12, 16, 52, 40)) : null,
                    Modal: kind == WindowKind.Dialog, Activate: kind != WindowKind.Tooltip);
                windows.Evaluate(request).RequireSupported();
                var child = await windows.CreateAsync(request);
                await windows.ExecuteAsync(new(child.Id, WindowAction.Show));
                await AppKitUi.Invoke(() =>
                {
                    var parent = Find("Work3 owner");
                    var native = Find("Work3 " + kind);
                    Require(native is NSPanel && native.IsVisible, "Auxiliary window is not a visible native panel.");
                    Require(Math.Abs(native.ContentLayoutRect.Width - request.Size.width) < .5 && Math.Abs(native.ContentLayoutRect.Height - request.Size.height) < .5,
                        "The actual native client area differs from the logical request.");
                    Require(kind == WindowKind.Dialog ? ReferenceEquals(native.SheetParent, parent) : ReferenceEquals(native.ParentWindow, parent), "Native owner/sheet mapping is absent.");
                    if (kind == WindowKind.Tooltip) Require(!native.CanBecomeKeyWindow && !native.IsKeyWindow, "Tooltip stole focus.");
                    checks[kind.ToString()] = new { panel = true, visible = native.IsVisible, sheet = native.SheetParent is not null,
                        canBecomeKey = native.CanBecomeKeyWindow, width = (double)native.ContentLayoutRect.Width,
                        height = (double)native.ContentLayoutRect.Height, scale = (double)native.BackingScaleFactor };
                });
                await windows.CloseAsync(child.Id);
            }
            var menus = primary.GetCapabilityOrDefault<IPlatformMenuBarHostCapability>(DorotiCapabilityIds.PlatformMenuBar)
                ?? throw new InvalidOperationException("AppKit menubar capability was not registered.");
            var selected = 0;
            var menuRequest = new PlatformMenuBarRequest(initial.Id, primary.viewId, primary.SceneOwner.Generation,
                [new("commands", "Commands", Children: [new("probe", "Probe", Shortcut: new("p", Modifiers: PlatformMenuModifiers.Meta))])]);
            await windows.ExecuteAsync(new(initial.Id, WindowAction.Focus));
            await using (var registration = await menus.SetAsync(menuRequest, action =>
            {
                Require(action.Window == initial.Id && action.ViewId == primary.viewId && action.Generation == primary.SceneOwner.Generation, "Menu action escaped its view generation.");
                selected++;
            }))
            {
                await AppKitUi.Invoke(() =>
                {
                    var menu = NSApplication.SharedApplication.MainMenu.Items.Single(item => item.Title == "Commands").Submenu!;
                    var item = menu.Items.Single();
                    NSApplication.SharedApplication.SendAction(item.Action!, item.Target, item);
                });
                Require(selected == 1, "Native menu action was not delivered exactly once.");
            }
            checks["menu"] = new { selected, typed = true, generation = primary.SceneOwner.Generation };
            var remainingChild = await windows.CreateAsync(new(WindowKind.Tooltip, "Work3 remaining tooltip", new Size(120, 60), owner.Id, Activate: false));
            await windows.ExecuteAsync(new(remainingChild.Id, WindowAction.Show));
            await windows.CloseAsync(owner.Id);
            Require(windows.GetWindows().Count == 1 && windows.GetWindows()[0].Id == initial.Id, "Owner close leaked an auxiliary window or closed the survivor.");
            checks["ownerClose"] = new { survivor = true, auxiliaryRemoved = true };
            File.WriteAllText(path, JsonSerializer.Serialize(checks));
            await windows.CloseAsync(initial.Id);
        }
        catch (Exception error) { File.WriteAllText(path + ".error", error.ToString()); }
    }
    private static NSWindow Find(string title)
    {
        NSWindow? found = null;
        NSApplication.SharedApplication.EnumerateWindows(NSWindowListOptions.OrderedFrontToBack,
            (NSWindow window, ref bool stop) => { if (window.Title == title) { found = window; stop = true; } });
        return found ?? throw new InvalidOperationException("Native window not found: " + title);
    }
    private static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
}
#endif
