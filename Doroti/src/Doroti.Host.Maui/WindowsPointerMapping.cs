using Doroti.Ui;

namespace Doroti.Host.Maui;

internal static class WindowsPointerMapping
{
    // Flutter flutter_window.cc: GetFlutterPointerDeviceKind. Windows promotes
    // pen/touch input to WM_MOUSE* and marks its origin in GetMessageExtraInfo.
    internal static PointerDeviceKind Kind(nint extraInfo)
    {
        var signature = unchecked((uint)extraInfo);
        if ((signature & 0xffffff00u) != 0xff515700u) return PointerDeviceKind.mouse;
        return (signature & 0x80u) != 0 ? PointerDeviceKind.touch : PointerDeviceKind.stylus;
    }
}
