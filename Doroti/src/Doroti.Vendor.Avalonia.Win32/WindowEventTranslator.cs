// Adapted from A0-pinned Avalonia WindowImpl.AppWndProc; see migration/avalonia-shell/a1-source-port-provenance.json.
namespace Doroti.Vendor.Avalonia.Win32;

internal static class WindowEventTranslator
{
    private const uint PointerFlagInContact = 0x00000004;
    private const uint PointerFlagFirstButton = 0x00000010;
    private const uint PointerFlagCancelled = 0x00008000;
    private const uint PointerFlagDown = 0x00010000;
    private const uint PointerFlagUp = 0x00040000;
    private const long PromotedPointerSignature = 0xFF515700;
    private const uint LeftButton = 0x0001;
    private const uint RightButton = 0x0002;
    private const uint MiddleButton = 0x0010;
    private const uint Shift = 0x0004;
    private const uint Control = 0x0008;

    internal static bool TryTranslatePointer(
        ulong windowId,
        NativeInterop.WindowMessage message,
        nuint wParam,
        nint lParam,
        double scaleFactor,
        out NativePointerEvent pointer)
    {
        if ((NativeInterop.GetMessageExtraInfo().ToInt64() & unchecked((long)0xFFFFFF00)) == PromotedPointerSignature)
        {
            pointer = default;
            return false;
        }
        var phase = message switch
        {
            NativeInterop.WindowMessage.MouseMove => NativePointerPhase.Move,
            NativeInterop.WindowMessage.LeftButtonDown or
            NativeInterop.WindowMessage.RightButtonDown or
            NativeInterop.WindowMessage.MiddleButtonDown => NativePointerPhase.Down,
            NativeInterop.WindowMessage.LeftButtonUp or
            NativeInterop.WindowMessage.RightButtonUp or
            NativeInterop.WindowMessage.MiddleButtonUp => NativePointerPhase.Up,
            NativeInterop.WindowMessage.MouseWheel or NativeInterop.WindowMessage.MouseHorizontalWheel => NativePointerPhase.Wheel,
            NativeInterop.WindowMessage.MouseLeave => NativePointerPhase.Removed,
            _ => (NativePointerPhase?)null,
        };
        if (phase is null)
        {
            pointer = default;
            return false;
        }

        var value = unchecked((uint)lParam.ToInt64());
        var x = unchecked((short)(value & 0xffff));
        var y = unchecked((short)(value >> 16));
        var scale = scaleFactor > 0 ? scaleFactor : 1;
        var nativeButtons = unchecked((uint)wParam.ToUInt64());
        var buttons = 0u;
        if ((nativeButtons & LeftButton) != 0)
        {
            buttons |= 1;
        }
        if ((nativeButtons & RightButton) != 0)
        {
            buttons |= 2;
        }
        if ((nativeButtons & MiddleButton) != 0)
        {
            buttons |= 4;
        }

        if (message is NativeInterop.WindowMessage.MouseMove && buttons == 0)
        {
            phase = NativePointerPhase.Hover;
        }
        var wheelDelta = message is NativeInterop.WindowMessage.MouseWheel or NativeInterop.WindowMessage.MouseHorizontalWheel
            ? unchecked((short)(nativeButtons >> 16)) / 120d
            : 0;
        var modifiers = 0u;
        if ((nativeButtons & Shift) != 0)
        {
            modifiers |= 1;
        }
        if ((nativeButtons & Control) != 0)
        {
            modifiers |= 2;
        }

        pointer = new(
            windowId,
            1,
            NativePointerDeviceKind.Mouse,
            phase.Value,
            x / scale,
            y / scale,
            buttons,
            message is NativeInterop.WindowMessage.MouseHorizontalWheel ? -wheelDelta : 0,
            message is NativeInterop.WindowMessage.MouseWheel ? wheelDelta : 0,
            modifiers,
            NativeInterop.GetMessageTime());
        return true;
    }

    internal static bool TryTranslatePointerMessage(
        nint window,
        ulong windowId,
        NativeInterop.WindowMessage message,
        nuint wParam,
        double scaleFactor,
        out NativePointerEvent pointer)
    {
        if (message is not (NativeInterop.WindowMessage.PointerDown or
            NativeInterop.WindowMessage.PointerUpdate or
            NativeInterop.WindowMessage.PointerUp))
        {
            pointer = default;
            return false;
        }
        var pointerId = unchecked((uint)wParam.ToUInt64()) & 0xffff;
        if (!NativeInterop.GetPointerInfo(pointerId, out var info))
        {
            pointer = default;
            return false;
        }
        var point = info.PixelLocation;
        if (!NativeInterop.ScreenToClient(window, ref point))
        {
            pointer = default;
            return false;
        }
        var phase = message switch
        {
            NativeInterop.WindowMessage.PointerDown => NativePointerPhase.Down,
            NativeInterop.WindowMessage.PointerUp => NativePointerPhase.Up,
            _ when (info.PointerFlags & PointerFlagCancelled) != 0 => NativePointerPhase.Cancelled,
            _ when (info.PointerFlags & PointerFlagDown) != 0 => NativePointerPhase.Down,
            _ when (info.PointerFlags & PointerFlagUp) != 0 => NativePointerPhase.Up,
            _ => NativePointerPhase.Move,
        };
        var deviceKind = info.PointerType switch
        {
            NativeInterop.PointerInputType.Touch => NativePointerDeviceKind.Touch,
            NativeInterop.PointerInputType.Pen => NativePointerDeviceKind.Pen,
            _ => NativePointerDeviceKind.Mouse,
        };
        var scale = scaleFactor > 0 ? scaleFactor : 1;
        var modifiers = 0u;
        if ((info.KeyStates & Shift) != 0) modifiers |= 1;
        if ((info.KeyStates & Control) != 0) modifiers |= 2;
        pointer = new(
            windowId,
            pointerId,
            deviceKind,
            phase,
            point.X / scale,
            point.Y / scale,
            (info.PointerFlags & (PointerFlagInContact | PointerFlagFirstButton)) != 0 ? 1u : 0u,
            0,
            0,
            modifiers,
            info.Time);
        return true;
    }

    internal static bool TryTranslateKey(
        ulong windowId,
        NativeInterop.WindowMessage message,
        nuint wParam,
        nint lParam,
        out NativeKeyEvent key)
    {
        var phase = message switch
        {
            NativeInterop.WindowMessage.KeyDown or NativeInterop.WindowMessage.SystemKeyDown =>
                (unchecked((ulong)lParam.ToInt64()) & (1UL << 30)) != 0 ? NativeKeyPhase.Repeat : NativeKeyPhase.Down,
            NativeInterop.WindowMessage.KeyUp or NativeInterop.WindowMessage.SystemKeyUp => NativeKeyPhase.Up,
            _ => (NativeKeyPhase?)null,
        };
        if (phase is null)
        {
            key = default;
            return false;
        }

        var keyData = unchecked((ulong)lParam.ToInt64());
        key = new(
            windowId,
            unchecked((uint)wParam.ToUInt64()),
            unchecked((uint)((keyData >> 16) & 0xff)) | ((keyData & (1UL << 24)) != 0 ? 0x100u : 0),
            phase.Value,
            ReadKeyModifiers(),
            NativeInterop.GetMessageTime());
        return true;
    }

    private static uint ReadKeyModifiers()
    {
        var modifiers = 0u;
        if (NativeInterop.GetKeyState(0x10) < 0)
        {
            modifiers |= 1;
        }
        if (NativeInterop.GetKeyState(0x11) < 0)
        {
            modifiers |= 2;
        }
        if (NativeInterop.GetKeyState(0x12) < 0)
        {
            modifiers |= 4;
        }
        if (NativeInterop.GetKeyState(0x5B) < 0 || NativeInterop.GetKeyState(0x5C) < 0)
        {
            modifiers |= 8;
        }
        return modifiers;
    }
}
