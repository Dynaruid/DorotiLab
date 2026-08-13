// Adapted from A0-pinned Avalonia Imm32InputMethod; see migration/avalonia-shell/a1-source-port-provenance.json.
using System.Runtime.InteropServices;

namespace Doroti.Vendor.Avalonia.Win32;

internal static class Win32ImeInterop
{
    private const uint CandidatePosition = 0x0040;
    private const uint CompositionPoint = 0x0002;

    internal static void SetCandidatePosition(nint window, int x, int y)
    {
        if (window == 0)
        {
            return;
        }
        var context = ImmGetContext(window);
        if (context == 0)
        {
            return;
        }
        try
        {
            var point = new NativePoint(x, y);
            var candidate = new CandidateForm(0, CandidatePosition, point, default);
            var composition = new CompositionForm(CompositionPoint, point, default);
            _ = ImmSetCandidateWindow(context, in candidate);
            _ = ImmSetCompositionWindow(context, in composition);
        }
        finally
        {
            _ = ImmReleaseContext(window, context);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct NativePoint(int X, int Y);

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct NativeRect(int Left, int Top, int Right, int Bottom);

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct CandidateForm(uint Index, uint Style, NativePoint CurrentPosition, NativeRect Area);

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct CompositionForm(uint Style, NativePoint CurrentPosition, NativeRect Area);

    [DllImport("imm32.dll")]
    private static extern nint ImmGetContext(nint window);

    [DllImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ImmReleaseContext(nint window, nint context);

    [DllImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ImmSetCandidateWindow(nint context, in CandidateForm candidate);

    [DllImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ImmSetCompositionWindow(nint context, in CompositionForm composition);
}
