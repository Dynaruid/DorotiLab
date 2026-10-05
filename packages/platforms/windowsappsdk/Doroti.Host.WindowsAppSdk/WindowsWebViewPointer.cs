using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Ui;
using Microsoft.Web.WebView2.Core;

namespace Doroti.Host.WindowsAppSdk;

internal sealed partial class WindowsWebViewComposition
{
    private readonly Dictionary<uint, PlatformViewHandle> _pointerTargets = [];

    private bool PointerMessage(uint message, nuint wparam)
    {
        if (
            message is not (0x245 or 0x246 or 0x247 or 0x249 or 0x24a or 0x24c)
            || !HasVisibleContent
        )
        {
            return false;
        }

        var id = (uint)(wparam & 0xffff);
        if (!PointerNative.GetPointerInfo(id, out var native) || native.Type is not (2 or 3))
        {
            return false;
        }

        var local = native.Pixel;
        Native.ScreenToClient(_parent, ref local);
        var logical = new Offset(local.X / _token.DeviceScaleX, local.Y / _token.DeviceScaleY);
        var hit = _visible
            .OfType<PlatformNativeSegment>()
            .LastOrDefault(p =>
                Bounds(p.Placement).contains(logical)
                && !_visible
                    .OfType<PlatformShieldSegment>()
                    .Any(s =>
                        s.PaintOrder > p.PaintOrder && ShieldBounds(s.Shield).contains(logical)
                    )
            );
        var handle = _pointerTargets.TryGetValue(id, out var captured)
            ? captured
            : hit?.Placement.Handle;
        if (
            handle is not { } target
            || !_instances.TryGetValue(target, out var instance)
            || instance.Placement is not { Visible: true } placement
        )
        {
            return false;
        }

        if (message == 0x246)
        {
            _pointerTargets[id] = target;
            instance.FocusNative();
        }
        var origin = placement.Transform.Map(placement.Bounds.topLeft);
        var offsetX = native.Pixel.X - local.X + (int)Math.Round(origin.dx * _token.DeviceScaleX);
        var offsetY = native.Pixel.Y - local.Y + (int)Math.Round(origin.dy * _token.DeviceScaleY);
        Windows.Foundation.Point Point(Native.Point point) =>
            new(point.X - offsetX, point.Y - offsetY);
        Windows.Foundation.Rect Rect(PointerNative.Rectangle rectangle) =>
            new(
                rectangle.Left - offsetX,
                rectangle.Top - offsetY,
                rectangle.Right - rectangle.Left,
                rectangle.Bottom - rectangle.Top
            );
        var info = instance.Core.Environment.CreateCoreWebView2PointerInfo();
        info.PointerKind = native.Type;
        info.PointerId = native.Id;
        info.FrameId = native.Frame;
        info.PointerFlags = native.Flags;
        info.PixelLocation = Point(native.Pixel);
        info.PixelLocationRaw = Point(native.RawPixel);
        info.HimetricLocation = new(native.Himetric.X, native.Himetric.Y);
        info.HimetricLocationRaw = new(native.RawHimetric.X, native.RawHimetric.Y);
        info.Time = native.Time;
        info.HistoryCount = native.History;
        info.InputData = native.Input;
        info.KeyStates = native.Keys;
        info.PerformanceCount = native.Performance;
        info.ButtonChangeKind = native.Button;
        if (PointerNative.GetPointerDeviceRects(native.Device, out var device, out var display))
        {
            info.PointerDeviceRect = new(
                device.Left,
                device.Top,
                device.Right - device.Left,
                device.Bottom - device.Top
            );
            info.DisplayRect = new(
                display.Left,
                display.Top,
                display.Right - display.Left,
                display.Bottom - display.Top
            );
        }
        if (native.Type == 2 && PointerNative.GetPointerTouchInfo(id, out var touch))
        {
            info.TouchFlags = touch.Flags;
            info.TouchMask = touch.Mask;
            info.TouchContact = Rect(touch.Contact);
            info.TouchContactRaw = Rect(touch.RawContact);
            info.TouchOrientation = touch.Orientation;
            info.TouchPressure = touch.Pressure;
        }
        if (native.Type == 3 && PointerNative.GetPointerPenInfo(id, out var pen))
        {
            info.PenFlags = pen.Flags;
            info.PenMask = pen.Mask;
            info.PenPressure = pen.Pressure;
            info.PenRotation = pen.Rotation;
            info.PenTiltX = pen.TiltX;
            info.PenTiltY = pen.TiltY;
        }
        if (message == 0x24c)
        {
            info.PointerFlags |= 0x8000;
            message = 0x245;
        }
        instance.Controller.SendPointerInput((CoreWebView2PointerEventKind)message, info);
        _pointerEvents++;
        if (message is 0x247 or 0x24a || (info.PointerFlags & 0x8000) != 0)
        {
            _pointerTargets.Remove(id);
        }

        return true;
    }

    private static class PointerNative
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Rectangle
        {
            internal int Left,
                Top,
                Right,
                Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Info
        {
            internal uint Type,
                Id,
                Frame,
                Flags;
            internal nint Device,
                Target;
            internal Native.Point Pixel,
                Himetric,
                RawPixel,
                RawHimetric;
            internal uint Time,
                History;
            internal int Input;
            internal uint Keys;
            internal ulong Performance;
            internal int Button;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Touch
        {
            internal Info Info;
            internal uint Flags,
                Mask;
            internal Rectangle Contact,
                RawContact;
            internal uint Orientation,
                Pressure;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Pen
        {
            internal Info Info;
            internal uint Flags,
                Mask,
                Pressure,
                Rotation;
            internal int TiltX,
                TiltY;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetPointerInfo(uint id, out Info info);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetPointerTouchInfo(uint id, out Touch info);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetPointerPenInfo(uint id, out Pen info);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetPointerDeviceRects(
            nint device,
            out Rectangle deviceRect,
            out Rectangle displayRect
        );
    }
}
