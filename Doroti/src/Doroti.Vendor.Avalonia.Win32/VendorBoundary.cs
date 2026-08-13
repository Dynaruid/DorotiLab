namespace Doroti.Vendor.Avalonia.Win32;

// Doroti-owned A1 seam. Avalonia-derived files are tracked by the A1 provenance manifest.
internal readonly record struct NativeWindowEvent(
    ulong WindowId,
    double LogicalWidth,
    double LogicalHeight,
    int PhysicalWidth,
    int PhysicalHeight,
    double ScaleFactor,
    bool IsMinimized);

internal enum NativeWindowNotificationKind
{
    Activated,
    Deactivated,
    MetricsChanged,
    CaptureCancelled,
    CloseRequested,
    Closed,
}

internal readonly record struct NativeWindowNotification(
    NativeWindowNotificationKind Kind,
    NativeWindowEvent Metrics);

internal enum NativePointerPhase
{
    Added,
    Hover,
    Down,
    Move,
    Up,
    Wheel,
    Removed,
    Cancelled,
}

internal enum NativePointerDeviceKind
{
    Mouse,
    Touch,
    Pen,
}

internal readonly record struct NativePointerEvent(
    ulong WindowId,
    ulong DeviceId,
    NativePointerDeviceKind DeviceKind,
    NativePointerPhase Phase,
    double LogicalX,
    double LogicalY,
    uint Buttons,
    double WheelDeltaX,
    double WheelDeltaY,
    uint Modifiers,
    uint TimestampMilliseconds);

internal enum NativeKeyPhase
{
    Down,
    Repeat,
    Up,
}

internal readonly record struct NativeKeyEvent(
    ulong WindowId,
    uint VirtualKey,
    uint ScanCode,
    NativeKeyPhase Phase,
    uint Modifiers,
    uint TimestampMilliseconds);

internal enum NativeTextEventKind
{
    Text,
    CompositionStarted,
    CompositionUpdated,
    CompositionEnded,
}

internal readonly record struct NativeTextEvent(NativeTextEventKind Kind, string Text);

internal static class VendorBoundary
{
    internal static string ContractVersion => "doroti.vendor.avalonia-win32/a1";

    internal static uint ReadWheelScrollLines()
    {
        const uint defaultLines = 3;
        return NativeInterop.SystemParametersInfo(NativeInterop.GetWheelScrollLines, 0, out var lines, 0) && lines <= 100
            ? lines
            : defaultLines;
    }

    internal static bool TouchDigitizerPresent
    {
        get
        {
            var flags = NativeInterop.GetSystemMetrics(94);
            return (flags & 0x80) != 0 && (flags & 0x03) != 0;
        }
    }

    internal static bool PenDigitizerPresent
    {
        get
        {
            var flags = NativeInterop.GetSystemMetrics(94);
            return (flags & 0x80) != 0 && (flags & 0x0C) != 0;
        }
    }
}
