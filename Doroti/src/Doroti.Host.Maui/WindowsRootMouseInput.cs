#if WINDOWS
using System.Runtime.InteropServices;
using Doroti.Ui;

namespace Doroti.Host.Maui;

/// <summary>
/// The HWND Composition plane is outside WinUI's visual hit-test tree. Receive
/// mouse messages at its existing input HWND, while WinUI still owns focus/IME.
/// </summary>
internal sealed class WindowsRootMouseInput : IDisposable
{
    private readonly nint _window;
    private readonly Func<int> _contentTop;
    private readonly Action<MauiSurfacePointerData> _pointer;
    private readonly Procedure _procedure;
    private readonly nuint _id;
    private static long _nextId;
    private bool _disposed;
    private bool _added;
    private bool _tracking;
    private int _buttons;
    private NativePoint _last;
    private nint _cursor;

    internal WindowsRootMouseInput(
        nint window,
        Func<int> contentTop,
        Action<MauiSurfacePointerData> pointer
    )
    {
        _window = window;
        _contentTop = contentTop;
        _pointer = pointer;
        _procedure = Message;
        _id = (nuint)Interlocked.Increment(ref _nextId);
        SetCursor(DorotiMouseCursorKind.basic);
        if (!SetWindowSubclass(window, _procedure, _id, 0))
        {
            throw new InvalidOperationException("Cannot attach the root Composition mouse input.");
        }
    }

    internal static nint FindInputWindow(nint parent) =>
        FindWindowExW(parent, 0, "Microsoft.UI.Content.DesktopChildSiteBridge", null);

    internal void SetCursor(DorotiMouseCursorKind cursor)
    {
        _cursor = WindowsClientResizeSource.ResolveCursor(cursor);
        if (_added)
        {
            SetNativeCursor(_cursor);
        }
    }

    private nint Message(nint window, uint message, nuint wParam, nint lParam, nuint id, nuint data)
    {
        if (_disposed)
            return DefSubclassProc(window, message, wParam, lParam);
        if (message == 0x0082)
        {
            Dispose();
        }
        else if (message == 0x0020 && unchecked((short)lParam) == 1 && _added)
        {
            SetNativeCursor(_cursor);
            return 1;
        }
        else if (message is 0x001f or 0x0215)
        {
            Cancel();
        }
        else if (message == 0x02a3)
        {
            _tracking = false;
            if (_buttons == 0)
                Remove();
        }
        else if (message is >= 0x0200 and <= 0x020e)
        {
            if (WindowsPointerMapping.Kind(GetMessageExtraInfo()) != PointerDeviceKind.mouse)
            {
                return 0; // Touch/pen are owned by WindowsNativePointerInput.
            }
            var point = new NativePoint
            {
                X = unchecked((short)lParam),
                Y = unchecked((short)((long)lParam >> 16)),
            };
            var wheel = message is 0x020a or 0x020e;
            if (wheel)
                ScreenToClient(window, ref point);
            point.Y -= _contentTop();
            if (point.Y < 0 && _buttons == 0)
            {
                Remove();
                return DefSubclassProc(window, message, wParam, lParam);
            }
            if (_buttons == 0 && message is 0x0201 or 0x0204 or 0x0207 or 0x020b)
            {
                // Establish HWND focus before beginning the pointer sequence;
                // the framework may then focus WinUI's IME endpoint on down.
                SetFocus(window);
            }
            _last = point;
            var previous = _buttons;
            var keys = (ushort)wParam;
            _buttons =
                ((keys & 1) != 0 ? 1 : 0)
                | ((keys & 2) != 0 ? 2 : 0)
                | ((keys & 16) != 0 ? 4 : 0)
                | ((keys & 32) != 0 ? 8 : 0)
                | ((keys & 64) != 0 ? 16 : 0);
            if (!_added)
            {
                Emit(PointerChange.add, 0);
                _added = true;
            }
            if (!_tracking)
            {
                var tracking = new Tracking
                {
                    Size = (uint)Marshal.SizeOf<Tracking>(),
                    Flags = 2,
                    Window = window,
                };
                _tracking = TrackMouseEvent(ref tracking);
            }
            var change =
                previous == 0 && _buttons != 0 ? PointerChange.down
                : previous != 0 && _buttons == 0 ? PointerChange.up
                : _buttons == 0 ? PointerChange.hover
                : PointerChange.move;
            var delta = unchecked((short)((ulong)wParam >> 16));
            Emit(change, _buttons, message == 0x020e ? delta : 0, message == 0x020a ? -delta : 0);
            if (change == PointerChange.down)
                SetCapture(window);
            else if (change == PointerChange.up && GetCapture() == window)
                ReleaseCapture();
            return message is 0x020b or 0x020c or 0x020d ? 1 : 0;
        }
        return DefSubclassProc(window, message, wParam, lParam);
    }

    private void Emit(PointerChange change, int buttons, double dx = 0, double dy = 0) =>
        _pointer(
            new(
                DorotiFrameClock.Now,
                change,
                PointerDeviceKind.mouse,
                1,
                _last.X,
                _last.Y,
                buttons,
                dx,
                dy,
                dx != 0 || dy != 0 ? PointerSignalKind.scroll : PointerSignalKind.none,
                buttons == 0 ? 0 : 1
            )
        );

    private void Remove()
    {
        if (!_added)
            return;
        Emit(PointerChange.remove, 0);
        _added = false;
    }

    private void Cancel()
    {
        if (_buttons != 0)
            Emit(PointerChange.cancel, 0);
        _buttons = 0;
        Remove();
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        Cancel();
        if (GetCapture() == _window)
            ReleaseCapture();
        RemoveWindowSubclass(_window, _procedure, _id);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X,
            Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Tracking
    {
        internal uint Size,
            Flags;
        internal nint Window;
        internal uint Time;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate nint Procedure(
        nint window,
        uint message,
        nuint wParam,
        nint lParam,
        nuint id,
        nuint data
    );

    [DllImport("comctl32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowSubclass(
        nint window,
        Procedure procedure,
        nuint id,
        nuint data
    );

    [DllImport("comctl32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RemoveWindowSubclass(nint window, Procedure procedure, nuint id);

    [DllImport("comctl32.dll", ExactSpelling = true)]
    private static extern nint DefSubclassProc(
        nint window,
        uint message,
        nuint wParam,
        nint lParam
    );

    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    private static extern nint FindWindowExW(
        nint parent,
        nint after,
        string className,
        string? title
    );

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint GetMessageExtraInfo();

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ScreenToClient(nint window, ref NativePoint point);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TrackMouseEvent(ref Tracking tracking);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint SetCapture(nint window);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint SetFocus(nint window);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint GetCapture();

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll", EntryPoint = "SetCursor", ExactSpelling = true)]
    private static extern nint SetNativeCursor(nint cursor);
}
#endif
