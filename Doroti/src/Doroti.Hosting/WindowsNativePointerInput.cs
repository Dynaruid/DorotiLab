using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Touch and pen WM_POINTER ingress for dedicated render HWNDs.</summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsNativePointerInput : IDisposable
{
    private readonly nint _window;
    private readonly ulong _viewId;
    private readonly Action<PointerDataPacket> _dispatch;
    private readonly SubclassProc _procedure;
    private readonly Dictionary<uint, PointerData> _pointers = [];
    private readonly HashSet<uint> _down = [];
    private static long _nextPointer = 1L << 55;
    private readonly nuint _id;
    private bool _disposed;

    public WindowsNativePointerInput(nint window, ulong viewId, Action<PointerDataPacket> dispatch)
    {
        _window = window; _viewId = viewId; _dispatch = dispatch; _procedure = Message;
        _id = checked((nuint)Interlocked.Increment(ref _nextPointer));
        if (!SetWindowSubclass(window, _procedure, _id, 0))
            throw new InvalidOperationException("Cannot attach native touch/pen input to the window thread.");
    }

    private nint Message(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data)
    {
        if (!_disposed)
        {
            try
            {
                // Windows promotes touch/pen to mouse messages as well. The
                // WM_POINTER stream below already owns those contacts.
                if ((message is >= 0x0200 and <= 0x020e) &&
                    (unchecked((uint)GetMessageExtraInfo()) & 0xffffff00u) == 0xff515700u) return 0;
                var pointer = (uint)(wparam & 0xffff);
                if (message == 0x024c) { Remove(pointer, true); return 0; }
                if (message is 0x0245 or 0x0246 or 0x0247 or 0x0249 or 0x024a)
                {
                    if (message == 0x024a && _pointers.ContainsKey(pointer)) { Remove(pointer, _down.Contains(pointer)); return 0; }
                    if (GetPointerInfo(pointer, out var info) && info.Type is 2 or 3)
                    {
                        if (message == 0x024a) return 0; // A touch was already removed on up.
                        PenInfo pen = default;
                        if (info.Type == 3 && !GetPointerPenInfo(pointer, out pen)) pen = default;
                        var point = info.Pixel;
                        ScreenToClient(_window, ref point);
                        Process(message, in info, in pen, point.X, point.Y);
                        return 0;
                    }
                }
                if (message is 0x0008 or 0x001f) Cancel();
                if (message == 0x0082) Dispose();
            }
            catch (Exception error) { Console.Error.WriteLine($"[Doroti pointer] {error.Message}"); Cancel(); }
        }
        return DefSubclassProc(hwnd, message, wparam, lparam);
    }

    private void Process(uint message, in PointerInfo info, in PenInfo pen, double x, double y)
    {
        var id = info.Id;
        var kind = info.Type == 2 ? PointerDeviceKind.touch
            : (pen.Flags & 6) != 0 ? PointerDeviceKind.invertedStylus : PointerDeviceKind.stylus;
        if (_pointers.TryGetValue(id, out var previous) && previous.kind != kind) Remove(id, true);
        var exists = _pointers.TryGetValue(id, out previous);
        var contact = (info.Flags & 4) != 0;
        var change = (info.Flags & 0x8000) != 0 ? PointerChange.cancel : message switch
        {
            0x0246 => PointerChange.down,
            0x0247 => PointerChange.up,
            _ => contact ? PointerChange.move : PointerChange.hover,
        };
        var buttons = (long)((info.Flags >> 4) & 0x1f);
        if (kind is PointerDeviceKind.stylus or PointerDeviceKind.invertedStylus && (pen.Flags & 1) != 0) buttons |= 2;
        if (contact && buttons == 0) buttons = 1;
        if (change is PointerChange.up or PointerChange.cancel) buttons = 0;
        var pressure = info.Type == 3 && (pen.Mask & 1) != 0 ? pen.Pressure / 1024d : contact ? 1 : 0;
        var tiltX = (pen.Mask & 4) != 0 ? pen.TiltX * Math.PI / 180 : 0;
        var tiltY = (pen.Mask & 8) != 0 ? pen.TiltY * Math.PI / 180 : 0;
        var packet = new PointerData(_viewId, DorotiFrameClock.Now, change, kind,
            (1UL << 52) | ((ulong)kind << 48) | id, x, y,
            exists ? x - previous.physicalX : 0, exists ? y - previous.physicalY : 0, buttons,
            pointerIdentifier: exists ? previous.pointerIdentifier : 0, pressure: pressure, pressureMin: 0, pressureMax: 1,
            orientation: (pen.Mask & 2) != 0 ? pen.Rotation * Math.PI / 180 : 0,
            tilt: Math.Acos(Math.Clamp(Math.Cos(tiltX) * Math.Cos(tiltY), -1, 1)));
        if (!exists) Emit(id, packet with { change = PointerChange.add, buttons = 0, physicalDeltaX = 0, physicalDeltaY = 0 });
        if (change == PointerChange.down || (change == PointerChange.move && !_down.Contains(id)))
        {
            packet = packet with { pointerIdentifier = checked((ulong)Interlocked.Increment(ref _nextPointer)),
                change = PointerChange.down, physicalDeltaX = 0, physicalDeltaY = 0, synthesized = change != PointerChange.down };
            _down.Add(id);
        }
        Emit(id, packet);
        if (change is PointerChange.up or PointerChange.cancel)
        {
            _down.Remove(id);
            if (kind == PointerDeviceKind.touch || change == PointerChange.cancel) Remove(id, false);
        }
    }

    private void Emit(uint id, PointerData packet) { _pointers[id] = packet; _dispatch(new([packet])); }
    private void Remove(uint id, bool cancel)
    {
        if (!_pointers.TryGetValue(id, out var packet)) return;
        if (_down.Remove(id) && cancel)
            _dispatch(new([packet with { timeStamp = DorotiFrameClock.Now, change = PointerChange.cancel, buttons = 0 }]));
        _dispatch(new([packet with { timeStamp = DorotiFrameClock.Now, change = PointerChange.remove, buttons = 0 }]));
        _pointers.Remove(id);
    }
    private void Cancel() { foreach (var id in _pointers.Keys.ToArray()) Remove(id, true); }
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true; Cancel(); RemoveWindowSubclass(_window, _procedure, _id);
    }

    [StructLayout(LayoutKind.Sequential)] private struct Point { public int X, Y; }
    [StructLayout(LayoutKind.Sequential)]
    private struct PointerInfo
    {
        public uint Type, Id, FrameId, Flags;
        public nint SourceDevice, Target;
        public Point Pixel, Himetric, PixelRaw, HimetricRaw;
        public uint Time, HistoryCount;
        public int InputData;
        public uint Keys;
        public ulong PerformanceCount;
        public uint ButtonChange;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct PenInfo { public PointerInfo Pointer; public uint Flags, Mask, Pressure, Rotation; public int TiltX, TiltY; }
    private delegate nint SubclassProc(nint hwnd, uint message, nuint wparam, nint lparam, nuint id, nuint data);
    [DllImport("comctl32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool SetWindowSubclass(nint hwnd, SubclassProc proc, nuint id, nuint data);
    [DllImport("comctl32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool RemoveWindowSubclass(nint hwnd, SubclassProc proc, nuint id);
    [DllImport("comctl32.dll")] private static extern nint DefSubclassProc(nint hwnd, uint message, nuint wparam, nint lparam);
    [DllImport("user32.dll")] private static extern nint GetMessageExtraInfo();
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool GetPointerInfo(uint id, out PointerInfo info);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool GetPointerPenInfo(uint id, out PenInfo info);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool ScreenToClient(nint hwnd, ref Point point);
}
