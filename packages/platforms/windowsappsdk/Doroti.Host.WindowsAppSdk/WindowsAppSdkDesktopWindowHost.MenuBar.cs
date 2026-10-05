using System.ComponentModel;
using System.Runtime.InteropServices;
using Doroti.Ui;
namespace Doroti.Host.WindowsAppSdk;
internal sealed partial class WindowsAppSdkDesktopWindowHost
{
    private sealed class MenuBarRegistration(WindowsAppSdkDesktopWindowHost host, PlatformMenuBarRequest request, Action<PlatformMenuEvent> callback) : IPlatformMenuBarRegistration
    {
        internal readonly PlatformMenuBarRequest Request = request;
        internal readonly Action<PlatformMenuEvent> Callback = callback;
        internal readonly Dictionary<uint, PlatformMenuItem> Commands = [];
        internal readonly Dictionary<nint, string> Submenus = [];
        internal nint Handle;
        internal bool Closed;
        public async ValueTask DisposeAsync()
        {
            if (Closed) return;
            await host._ui.InvokeAsync(() => { host.RemoveMenuBar(this); return ValueTask.CompletedTask; });
        }
        internal void Emit(string id, PlatformMenuEventKind kind)
        { if (!Closed) Callback(new(Request.Window, Request.ViewId, Request.Generation, id, kind)); }
    }
    private MenuBarRegistration? _menuBar;
    public WindowCapabilityResult Evaluate(PlatformMenuBarRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (_disposed || WindowContext?.Window.Id != request.Window || WindowContext.Window.ViewId != request.ViewId)
            throw new InvalidOperationException("The menu bar requires this live window and view.");
        if (request.Generation <= 0) throw new ArgumentOutOfRangeException(nameof(request));
        // Reuse the bounded item graph validation without entering a native menu loop.
        ValidateMenuItems(request.Items);
        string? unsupported = null;
        void Visit(IReadOnlyList<PlatformMenuItem> items)
        {
            foreach (var item in items)
            {
                if (item.PlatformRole is not null) unsupported = "Windows does not implement AppKit provided menu roles.";
                if (item.Shortcut is { } shortcut && ShortcutKey(shortcut) is null) unsupported = "This native shortcut is not supported by Windows.";
                if (item.Children is { } children) Visit(children);
            }
        }
        Visit(request.Items);
        return unsupported is null ? WindowCapabilityResult.Supported : new(WindowAvailability.Unsupported, unsupported);
    }
    private static uint? ShortcutKey(PlatformMenuShortcut shortcut)
    {
        if ((shortcut.Modifiers & ~(PlatformMenuModifiers.Control | PlatformMenuModifiers.Alt | PlatformMenuModifiers.Shift)) != 0) return null;
        if (shortcut.Character is { Length: 1 } text && char.IsAsciiLetterOrDigit(text[0])) return char.ToUpperInvariant(text[0]);
        if (shortcut.Character is null && shortcut.LogicalKey is >= 0x20 and <= 0x7e) return char.ToUpperInvariant((char)shortcut.LogicalKey.Value);
        return null;
    }
    public async ValueTask<IPlatformMenuBarRegistration> SetAsync(PlatformMenuBarRequest request, Action<PlatformMenuEvent> callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);
        Evaluate(request).RequireSupported();
        await _attached.Task.WaitAsync(cancellationToken);
        MenuBarRegistration? registration = null;
        await _ui.InvokeAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested(); Evaluate(request).RequireSupported();
            var next = new MenuBarRegistration(this, request, callback);
            uint serial = 0;
            nint Build(IReadOnlyList<PlatformMenuItem> items, bool root)
            {
                var native = root ? CreateMenu() : CreatePopupMenu();
                if (native == 0) throw new Win32Exception(Marshal.GetLastWin32Error());
                try
                {
                    foreach (var item in items)
                    {
                        var flags = (item.Enabled ? 0u : 1u) | (item.Checked ? 8u : 0u);
                        if (item.Separator) { if (!AppendMenuW(native, 0x800, 0, "")) throw new Win32Exception(Marshal.GetLastWin32Error()); continue; }
                        if (item.Children is { Count: > 0 } children)
                        {
                            var child = Build(children, false); next.Submenus.Add(child, item.Id);
                            if (!AppendMenuW(native, flags | 0x10, (nuint)child, item.Label)) { DestroyMenu(child); throw new Win32Exception(Marshal.GetLastWin32Error()); }
                        }
                        else
                        {
                            var id = ++serial; next.Commands.Add(id, item);
                            var label = item.Label;
                            if (item.Shortcut is { } shortcut) label += "\t" + ShortcutLabel(shortcut);
                            if (!AppendMenuW(native, flags, id, label)) throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                    return native;
                }
                catch { DestroyMenu(native); throw; }
            }
            next.Handle = Build(request.Items, true);
            try
            {
                if (!SetMenu(_hwnd, next.Handle)) throw new Win32Exception(Marshal.GetLastWin32Error());
                var previous = _menuBar; _menuBar = next;
                if (previous is not null) { previous.Closed = true; DestroyMenu(previous.Handle); previous.Handle = 0; }
                DrawMenuBar(_hwnd); Publish(); registration = next;
            }
            catch { next.Closed = true; DestroyMenu(next.Handle); throw; }
            return ValueTask.CompletedTask;
        });
        return registration!;
    }
    private static string ShortcutLabel(PlatformMenuShortcut shortcut) =>
        ((shortcut.Modifiers & PlatformMenuModifiers.Control) != 0 ? "Ctrl+" : "") +
        ((shortcut.Modifiers & PlatformMenuModifiers.Alt) != 0 ? "Alt+" : "") +
        ((shortcut.Modifiers & PlatformMenuModifiers.Shift) != 0 ? "Shift+" : "") + (char)ShortcutKey(shortcut)!.Value;
    private void RemoveMenuBar(MenuBarRegistration? expected = null)
    {
        if (_menuBar is not { } current || expected is not null && !ReferenceEquals(current, expected))
        { if (expected is not null) expected.Closed = true; return; }
        _menuBar = null; current.Closed = true;
        if (_hwnd != 0) { SetMenu(_hwnd, 0); DrawMenuBar(_hwnd); }
        DestroyMenu(current.Handle); current.Handle = 0;
    }
    private bool HandleMenuBarMessage(uint message, nuint wparam, nint lparam)
    {
        if (_menuBar is not { Closed: false } current) return false;
        if (message == 0x0111 && lparam == 0 && current.Commands.TryGetValue((uint)(wparam & 0xffff), out var item))
        { if (item.Enabled) current.Emit(item.Id, PlatformMenuEventKind.Selected); return true; }
        if (message is 0x0117 or 0x0125 && current.Submenus.TryGetValue((nint)wparam, out var id))
            current.Emit(id, message == 0x0117 ? PlatformMenuEventKind.Opened : PlatformMenuEventKind.Closed);
        if (message is 0x0100 or 0x0104)
        {
            var modifiers = PlatformMenuModifiers.None;
            if (GetKeyState(0x10) < 0) modifiers |= PlatformMenuModifiers.Shift;
            if (GetKeyState(0x11) < 0) modifiers |= PlatformMenuModifiers.Control;
            if (GetKeyState(0x12) < 0) modifiers |= PlatformMenuModifiers.Alt;
            foreach (var command in current.Commands.Values)
                if (command.Enabled && command.Shortcut is { } shortcut && shortcut.Modifiers == modifiers && ShortcutKey(shortcut) == (uint)wparam)
                { current.Emit(command.Id, PlatformMenuEventKind.Selected); return true; }
        }
        return false;
    }
    [DllImport("user32.dll", SetLastError = true)] private static extern nint CreateMenu();
    [DllImport("user32.dll", SetLastError = true)] private static extern bool SetMenu(nint window, nint menu);
    [DllImport("user32.dll")] private static extern bool DrawMenuBar(nint window);
    [DllImport("user32.dll")] private static extern short GetKeyState(int key);
}
