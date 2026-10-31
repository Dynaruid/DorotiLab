using Doroti.Ui;

namespace Doroti.Host.Maui;

// AndroidTouchProcessor.java: getPointerDeviceTypeForToolType and button masks.
// Keep native tool values here rather than round-tripping through SKTouchDeviceType.
internal static class AndroidPointerMapping
{
    // MotionEvent uses SystemClock.uptimeMillis, whereas the framework owns a
    // process-relative monotonic clock. Preserve the event's age when crossing
    // that boundary so UI queue stalls do not change drag/fling timing.
    internal static TimeSpan Timestamp(TimeSpan now, long uptimeMilliseconds, long eventMilliseconds)
    {
        if (eventMilliseconds <= 0 || eventMilliseconds > uptimeMilliseconds)
        {
            return now;
        }
        var ageMilliseconds = uptimeMilliseconds - eventMilliseconds;
        return ageMilliseconds >= now.TotalMilliseconds
            ? TimeSpan.Zero
            : now - TimeSpan.FromMilliseconds(ageMilliseconds);
    }

    // Android's virtual devices may have negative IDs (adb uses -1). Keep all
    // 32 device bits without overflowing Flutter's signed 64-bit identifier.
    internal static ulong DeviceIdentifier(int deviceId, int pointerId)
    {
        if ((uint)pointerId > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(pointerId));
        }

        return ((ulong)(uint)deviceId << 16) | (uint)pointerId;
    }

    internal static PointerDeviceKind Kind(int toolType) =>
        toolType switch
        {
            1 => PointerDeviceKind.touch,
            2 => PointerDeviceKind.stylus,
            3 => PointerDeviceKind.mouse,
            4 => PointerDeviceKind.invertedStylus,
            _ => PointerDeviceKind.unknown,
        };

    internal static int Buttons(PointerDeviceKind kind, int buttonState) =>
        kind switch
        {
            PointerDeviceKind.mouse => buttonState & 0x1f,
            PointerDeviceKind.stylus => (buttonState >> 4) & 0xf,
            _ => 0,
        };
}
