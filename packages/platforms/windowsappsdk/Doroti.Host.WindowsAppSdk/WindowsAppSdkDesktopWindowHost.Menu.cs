using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Ui;
namespace Doroti.Host.WindowsAppSdk;
internal sealed partial class WindowsAppSdkDesktopWindowHost
{
    private bool _menuActive;
    public WindowCapabilityResult Evaluate(PlatformMenuRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!Enum.IsDefined(request.Presentation)) throw new ArgumentException("Unknown menu presentation.");
        if (request.Presentation == PlatformMenuPresentation.Overlay) return new(WindowAvailability.Unsupported, "Overlay presentation belongs to the design package.");
        if (_disposed || WindowContext?.Window.Id != request.Window) throw new InvalidOperationException("The requested menu owner is not this live native window.");
        if (request.Items.Count == 0 || !double.IsFinite(request.Anchor.left) || !double.IsFinite(request.Anchor.top) || !double.IsFinite(request.Anchor.bottom) || !double.IsFinite(request.Anchor.right)) throw new ArgumentException("Menu items and a finite logical anchor are required.");
        ValidateMenuItems(request.Items);
        bool Unsupported(IReadOnlyList<PlatformMenuItem> items) => items.Any(item => item.Shortcut is not null || item.PlatformRole is not null || (item.Children is { } children && Unsupported(children)));
        return Unsupported(request.Items)
            ? new(WindowAvailability.Unsupported, "Windows popup menus do not implement shortcut dispatch or platform-provided roles.")
            : WindowCapabilityResult.Supported;
    }
    private static void ValidateMenuItems(IReadOnlyList<PlatformMenuItem> items)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        void Validate(IReadOnlyList<PlatformMenuItem> items, int depth)
        {
            if (depth > 16) throw new ArgumentException("Menu depth exceeds 16.");
            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item.Id) || !ids.Add(item.Id) || ids.Count > 4096 || item.Label.Length > 4096) throw new ArgumentException("Invalid or duplicate menu item.");
                if (item.Separator && (item.Children is { Count: > 0 } || item.Shortcut is not null || item.PlatformRole is not null))
                    throw new ArgumentException("A menu separator cannot contain commands or children.");
                if (item.Children is { } children) Validate(children, depth + 1);
            }
        }
        Validate(items, 0);
    }
    public async ValueTask<PlatformMenuResult> ShowAsync(PlatformMenuRequest request, CancellationToken cancellationToken = default)
    {
        Evaluate(request).RequireSupported();
        await _attached.Task.WaitAsync(cancellationToken);
        PlatformMenuResult? result = null;
        await _ui.InvokeAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Evaluate(request).RequireSupported();
            var map = new Dictionary<uint, string>();
            uint index = 0;
            nint Create(IReadOnlyList<PlatformMenuItem> items)
            {
                var menu = CreatePopupMenu();
                if (menu == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
                try
                {
                    foreach (var item in items)
                    {
                        var flags = (item.Enabled ? 0u : 1u) | (item.Checked ? 8u : 0u);
                        if (item.Separator) { if (!AppendMenuW(menu, 0x800, 0, "")) throw new Win32Exception(Marshal.GetLastWin32Error()); continue; }
                        if (item.Children is { Count: > 0 } children)
                        {
                            var child = Create(children);
                            if (!AppendMenuW(menu, flags | 0x10, (nuint)child, item.Label)) { DestroyMenu(child); throw new Win32Exception(Marshal.GetLastWin32Error()); }
                        }
                        else
                        {
                            var id = ++index;
                            map.Add(id, item.Id);
                            if (!AppendMenuW(menu, flags, id, item.Label)) throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                    return menu;
                }
                catch { DestroyMenu(menu); throw; }
            }
            if (_menuActive) throw new InvalidOperationException("A native menu is already active for this window.");
            var native = Create(request.Items);
            _menuActive = true;
            try
            {
                var scale = GetDpiForWindow(_hwnd) / 96.0;
                var point = new MutablePoint { X = (int)Math.Round(request.Anchor.left * scale), Y = (int)Math.Round(request.Anchor.bottom * scale) };
                if (!ClientToScreen(_hwnd, ref point)) throw new Win32Exception(Marshal.GetLastWin32Error());
                using var cancel = cancellationToken.Register(() => PostMessageW(_hwnd, 0x001F, 0, 0));
                SetForegroundWindow(_hwnd);
                SetLastError(0);
                var selected = TrackPopupMenuEx(native, 0x180, point.X, point.Y, _hwnd, 0);
                var error = Marshal.GetLastWin32Error();
                cancellationToken.ThrowIfCancellationRequested();
                if (selected == 0 && error != 0) throw new Win32Exception(error);
                result = selected != 0 ? new(PlatformMenuDismissal.Selected, map[selected]) : new(_disposed ? PlatformMenuDismissal.WindowClosed : PlatformMenuDismissal.Canceled);
            }
            finally { _menuActive = false; DestroyMenu(native); }
            return ValueTask.CompletedTask;
        });
        return result!;
    }
    [DllImport("user32.dll")] private static extern bool EndMenu();
    [DllImport("user32.dll", SetLastError = true)] private static extern nint CreatePopupMenu();
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern bool AppendMenuW(nint menu, uint flags, nuint id, string text);
    [DllImport("user32.dll", SetLastError = true)] private static extern uint TrackPopupMenuEx(nint menu, uint flags, int x, int y, nint owner, nint parameters);
    [DllImport("user32.dll")] private static extern bool DestroyMenu(nint menu);
    [DllImport("kernel32.dll")] private static extern void SetLastError(uint error);
}
