using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Desktop;
using Doroti.Ui;
namespace DorotiTestbedApp.Desktop;
internal static class WindowKindsProbe
{
    internal static async Task RunAsync(DesktopWindowContext context, string output, CancellationToken ct)
    {
        var service = (IWindowService)context.Windows;
        var parent = FindWindowW(null, context.Window.State.RequestedAppearance is not null ? "Doroti Desktop · Sudoku" : "");
        if (parent == 0) throw new InvalidOperationException("Primary HWND was not found.");
        var results = new List<object>();
        var bad = new WindowRequest(WindowKind.Popup, "Rejected popup", new Size(120, 80), context.Window.Id);
        if (service.Evaluate(bad).Availability != WindowAvailability.Unsupported) throw new InvalidOperationException("Missing anchor must fail before allocation.");
        foreach (var kind in new[] { WindowKind.Regular, WindowKind.Dialog, WindowKind.Popup, WindowKind.Tooltip, WindowKind.Satellite })
        {
            var title = "Doroti kind probe " + kind;
            var owner = kind == WindowKind.Regular ? (WindowId?)null : context.Window.Id;
            var anchor = kind is WindowKind.Popup or WindowKind.Tooltip ? new WindowAnchor(context.Window.Id, Rect.fromLTWH(35, 40, 20, 20)) : null;
            var request = new WindowRequest(kind, title, new Size(240, 180), owner, anchor, kind == WindowKind.Dialog, kind != WindowKind.Tooltip);
            service.Evaluate(request).RequireSupported();
            var created = await service.CreateAsync(request, ct);
            if (!context.Windows.TryGetWindow(created.Id, out var controller)) throw new InvalidOperationException("No single-registry controller.");
            await controller!.WaitUntilReadyToShowAsync(ct);
            await service.ExecuteAsync(new(created.Id, WindowAction.Show), ct);
            await Task.Delay(250, ct);
            var hwnd = FindWindowW(null, title);
            if (hwnd == 0 || !IsWindowVisible(hwnd)) throw new InvalidOperationException("Missing native " + kind);
            if (owner is not null && GetWindow(hwnd, 4) != parent) throw new InvalidOperationException("Native owner mismatch for " + kind);
            var exstyle = GetWindowLongPtrW(hwnd, -20).ToInt64();
            if (kind == WindowKind.Tooltip && (exstyle & 0x08000000) == 0) throw new InvalidOperationException("Tooltip can activate.");
            if (kind is WindowKind.Popup or WindowKind.Tooltip && (GetWindowLongPtrW(hwnd, -16).ToInt64() & 0x80000000) == 0) throw new InvalidOperationException("Popup style is absent.");
            if (kind == WindowKind.Dialog && IsWindowEnabled(parent)) throw new InvalidOperationException("Modal owner input was not disabled.");
            var current = service.GetWindows().Single(window => window.Id == created.Id);
            if (current.Kind != kind || current.ViewId is null) throw new InvalidOperationException("Window/view mapping mismatch.");
            await service.CloseAsync(created.Id, ct);
            if (kind == WindowKind.Dialog && !IsWindowEnabled(parent)) throw new InvalidOperationException("Modal input was not restored.");
            results.Add(new { kind = kind.ToString(), nativeOwner = owner is null || GetWindow(hwnd, 4) == 0, viewId = current.ViewId.Value.ToString(), native = true, drained = true });
        }
        File.WriteAllText(output, JsonSerializer.Serialize(new { schemaVersion = "doroti.windows-kinds-probe/v1", result = "PASS", windows = results, remaining = service.GetWindows().Count }));
        await context.Window.CloseAsync(CancellationToken.None);
    }
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern nint FindWindowW(string? name, string title);
    [DllImport("user32.dll")] private static extern nint GetWindow(nint window, uint command);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(nint window);
    [DllImport("user32.dll")] private static extern bool IsWindowEnabled(nint window);
    [DllImport("user32.dll")] private static extern nint GetWindowLongPtrW(nint window, int index);
}
