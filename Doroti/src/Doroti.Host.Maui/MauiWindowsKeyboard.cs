#if WINDOWS
using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Ui;
using Microsoft.UI.Xaml.Input;

namespace Doroti.Host.Maui;

internal static class MauiWindowsKeyboard
{
    internal static bool OwnsFocus(Microsoft.UI.Xaml.UIElement owner)
    {
        if (owner.XamlRoot is not { } root) return false;
        for (var focused = FocusManager.GetFocusedElement(root) as Microsoft.UI.Xaml.DependencyObject;
            focused is not null; focused = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(focused))
        {
            if (ReferenceEquals(focused, owner)) return true;
        }
        return false;
    }
    internal static KeyData Translate(ulong viewId, KeyRoutedEventArgs args, KeyEventType type)
    {
        var scan = args.KeyStatus.ScanCode | (args.KeyStatus.IsExtendedKey ? 0x100L : 0);
        var character = type == KeyEventType.up ? null : Character((uint)args.Key, args.KeyStatus.ScanCode);
        return new(viewId, DorotiFrameClock.Now,
            type == KeyEventType.down && args.KeyStatus.WasKeyDown ? KeyEventType.repeat : type,
            WindowsKeyboardMap.Physical(scan, (long)args.Key),
            WindowsKeyboardMap.Logical(scan, (long)args.Key, character ?? ""), false, character);
    }

    private static string? Character(uint key, uint scan)
    {
        var state = new byte[256];
        if (!GetKeyboardState(state)) return null;
        var buffer = new char[8];
        // Inspect without consuming TranslateMessage's dead-key composition state.
        var count = ToUnicodeEx(key, scan, state, buffer, buffer.Length, 4, GetKeyboardLayout(0));
        if (count <= 0 || count > buffer.Length) return null;
        var text = new string(buffer, 0, count);
        return text.EnumerateRunes().Any(System.Text.Rune.IsControl) ? null : text;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetKeyboardState([Out] byte[] state);
    [DllImport("user32.dll")]
    private static extern nint GetKeyboardLayout(uint thread);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int ToUnicodeEx(uint key, uint scan, byte[] state, [Out] char[] buffer,
        int length, uint flags, nint layout);
}

// XAML keeps its focused element when the application loses activation; a
// LostFocus handler alone cannot release keys whose key-up goes to another app.
internal sealed class MauiWindowsKeyboardFocus : IDisposable
{
    private readonly Microsoft.Maui.Controls.View _view;
    private readonly Action _release;
    private Microsoft.Maui.Controls.Window? _window;

    internal MauiWindowsKeyboardFocus(Microsoft.Maui.Controls.View view, Action release)
    {
        _view = view;
        _release = release;
        view.Loaded += Loaded;
        view.Unloaded += Unloaded;
        Attach();
    }

    private void Loaded(object? sender, EventArgs args) => Attach();
    private void Unloaded(object? sender, EventArgs args) => Detach();
    private void Deactivated(object? sender, EventArgs args) => _release();
    private void Attach()
    {
        if (ReferenceEquals(_window, _view.Window)) return;
        Detach();
        _window = _view.Window;
        if (_window is not null) _window.Deactivated += Deactivated;
    }
    private void Detach()
    {
        if (_window is not null) _window.Deactivated -= Deactivated;
        _window = null;
        _release();
    }
    public void Dispose()
    {
        _view.Loaded -= Loaded;
        _view.Unloaded -= Unloaded;
        Detach();
    }
}

#endif
