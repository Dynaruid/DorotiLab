#if WINDOWS
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Doroti.Ui;
using Microsoft.UI.Xaml;

namespace Doroti.Host.Maui;

internal static class WindowsNativeCaption
{
    private sealed class Registration;

    private static readonly ConditionalWeakTable<Microsoft.UI.Xaml.Window, Registration> Windows =
        new();

    internal static bool IsEnabled(Microsoft.UI.Xaml.Window window) =>
        Windows.TryGetValue(window, out _);

    internal static void Enable(Microsoft.UI.Xaml.Window window)
    {
        // Run from OnPlatformWindowSubclassed, before MAUI constructs its
        // NavigationRootManager. A late reset leaves its XAML title row alive.
        window.AppWindow.TitleBar.ResetToDefault();
        window.ExtendsContentIntoTitleBar = false;
        Windows.GetValue(window, _ => new());
    }

    internal static void ApplyTheme(Microsoft.UI.Xaml.Window window, WindowBackdropOptions options)
    {
        if (!IsEnabled(window))
            return;
        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var dark =
            options.theme == WindowBackdropTheme.dark
            || (
                options.theme == WindowBackdropTheme.system
                && window.Content is FrameworkElement { ActualTheme: ElementTheme.Dark }
            );
        uint darkMode = dark ? 1u : 0u;
        // Leave the caption color and material under Windows' standard policy.
        // Do not re-enable AppWindow custom title-bar rendering via its colors.
        _ = DwmSetWindowAttribute(handle, 20, in darkMode, sizeof(uint));
    }

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmSetWindowAttribute(
        nint window,
        uint attribute,
        in uint value,
        uint size
    );
}
#endif
