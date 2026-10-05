using Doroti.Ui;

namespace Doroti.Host.Maui;

// AndroidTouchProcessor.java: getPointerDeviceTypeForToolType and button masks.
// Keep native tool values here rather than round-tripping through SKTouchDeviceType.
internal static class AndroidPointerMapping
{
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
