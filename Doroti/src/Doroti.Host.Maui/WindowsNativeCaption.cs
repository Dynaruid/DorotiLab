#if WINDOWS
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Doroti.Ui;
using Microsoft.UI.Xaml;

namespace Doroti.Host.Maui;

internal static class WindowsNativeCaption
{
    private const uint SystemBackdropType = 38; // DWMWA_SYSTEMBACKDROP_TYPE
    private const uint TransientWindowBackdrop = 3; // DWMSBT_TRANSIENTWINDOW (Desktop Acrylic)

    private sealed class Registration
    {
        internal uint? PreviousBackdrop;
    }

    private static readonly ConditionalWeakTable<Microsoft.UI.Xaml.Window, Registration> Windows =
        new();

    internal static bool IsEnabled(Microsoft.UI.Xaml.Window window) =>
        Windows.TryGetValue(window, out _);

    internal static void Enable(Microsoft.UI.Xaml.Window window)
    {
        // Run from OnPlatformWindowSubclassed, before MAUI constructs its
        // NavigationRootManager. A late reset leaves its XAML title row alive.
        window.ExtendsContentIntoTitleBar = false;
        window.AppWindow.TitleBar.ResetToDefault();
        Windows.GetValue(window, _ => new());
    }

    internal static void ApplyAppearance(Microsoft.UI.Xaml.Window window, WindowBackdropOptions options)
    {
        if (!Windows.TryGetValue(window, out var registration))
            return;
        // MAUI/WinUI can install its caption again while constructing the
        // content. Reset once the backdrop attaches as well as before MAUI
        // chooses its title-row layout; DWM attributes do not style that
        // custom caption. Never write AppWindow title-bar colors afterwards.
        window.AppWindow.TitleBar.ResetToDefault();
        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var dark =
            options.theme == WindowBackdropTheme.dark
            || (
                options.theme == WindowBackdropTheme.system
                && window.Content is FrameworkElement { ActualTheme: ElementTheme.Dark }
            );
        uint darkMode = dark ? 1u : 0u;
        // Keep the standard non-client caption, including native hit testing.
        _ = DwmSetWindowAttribute(handle, 20, in darkMode, sizeof(uint));

        // WinUI's SystemBackdrop covers the client; it does not select the
        // material behind the native caption. Ask DWM explicitly for Acrylic.
        // Older Windows versions retain their standard caption appearance.
        if (
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621)
            && options.mode is WindowBackdropMode.acrylic or WindowBackdropMode.experimentalAcrylic
        )
        {
            if (registration.PreviousBackdrop is null)
            {
                Marshal.ThrowExceptionForHR(
                    DwmGetWindowAttribute(handle, SystemBackdropType, out var previous, sizeof(uint))
                );
                registration.PreviousBackdrop = previous;
            }
            var backdrop = TransientWindowBackdrop;
            Marshal.ThrowExceptionForHR(
                DwmSetWindowAttribute(handle, SystemBackdropType, in backdrop, sizeof(uint))
            );
        }
        else
        {
            RestoreBackdrop(window);
        }
    }

    internal static void RestoreBackdrop(Microsoft.UI.Xaml.Window window)
    {
        if (
            !Windows.TryGetValue(window, out var registration)
            || registration.PreviousBackdrop is not { } previous
        )
            return;

        registration.PreviousBackdrop = null;
        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        // Disposal can run after the native window has started closing.
        _ = DwmSetWindowAttribute(handle, SystemBackdropType, in previous, sizeof(uint));
    }

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmGetWindowAttribute(
        nint window,
        uint attribute,
        out uint value,
        uint size
    );

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmSetWindowAttribute(
        nint window,
        uint attribute,
        in uint value,
        uint size
    );
}
#endif
