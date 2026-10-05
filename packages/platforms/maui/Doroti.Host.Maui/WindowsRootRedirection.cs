#if WINDOWS
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Doroti.Host.Maui;

/// <summary>
/// Honor alpha in the otherwise unused WinUI top-level redirection bitmap.
/// The raw WindowsAppSdk shell creates its HWND with NOREDIRECTIONBITMAP;
/// WinUI owns HWND creation here, so its bitmap must remain transparent below
/// Doroti's native Composition target, including during resize clipping.
/// </summary>
internal sealed class WindowsRootRedirection(nint window) : IDisposable
{
    private const uint EnableFlag = 1;
    private const uint RegionFlag = 2;
    private bool _enabled;

    internal void Apply()
    {
        // This documented API honors window alpha. An empty region requests
        // no legacy blur; the existing WinUI SystemBackdrop owns Acrylic.
        // No GDI painting, color key, or opacity is applied to application pixels.
        var region = CreateRectRgn(0, 0, 0, 0);
        if (region == 0)
            throw new InvalidOperationException("Cannot allocate the root alpha region.");
        try
        {
            var blur = new Blur
            {
                Flags = EnableFlag | RegionFlag,
                Enable = 1,
                Region = region,
            };
            Marshal.ThrowExceptionForHR(DwmEnableBlurBehindWindow(window, in blur));
            _enabled = true;
        }
        finally
        {
            DeleteObject(region);
        }
        WindowsResizeTimeline.Record(window, "redirection-alpha-enabled");
    }

    public void Dispose()
    {
        if (!_enabled)
            return;
        _enabled = false;
        var blur = new Blur { Flags = EnableFlag };
        var result = DwmEnableBlurBehindWindow(window, in blur);
        if (result < 0)
            Debug.WriteLine($"Cannot release root alpha policy: 0x{result:x8}");
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Blur
    {
        internal uint Flags;
        internal int Enable;
        internal nint Region;
        internal int Transition;
    }

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmEnableBlurBehindWindow(nint window, in Blur blur);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    private static extern nint CreateRectRgn(int left, int top, int right, int bottom);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject(nint value);
}
#endif
