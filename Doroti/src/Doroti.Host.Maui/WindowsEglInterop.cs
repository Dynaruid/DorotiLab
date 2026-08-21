#if WINDOWS
using System.Runtime.InteropServices;

namespace Doroti.Host.Maui;

/// <summary>
/// Minimal access to the EGL state already owned by SkiaSharp Views. Doroti
/// never creates, destroys, makes current, or swaps this surface.
/// </summary>
internal static class WindowsEglInterop
{
    private const string Library = "libEGL.dll";

    internal const int EglDraw = 0x3059;
    internal const int EglSuccess = 0x3000;

    [DllImport(Library)]
    internal static extern nint eglGetCurrentDisplay();

    [DllImport(Library)]
    internal static extern nint eglGetCurrentSurface(int readdraw);

    [DllImport(Library)]
    internal static extern int eglSwapInterval(nint display, int interval);

    [DllImport(Library)]
    internal static extern int eglGetError();
}
#endif
