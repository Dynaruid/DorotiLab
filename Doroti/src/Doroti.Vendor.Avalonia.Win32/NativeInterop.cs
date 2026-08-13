// Adapted from A0-pinned Avalonia Win32 interop; see migration/avalonia-shell/a1-source-port-provenance.json.
using System.Runtime.InteropServices;

namespace Doroti.Vendor.Avalonia.Win32;

internal static partial class NativeInterop
{
    internal const int ColorWindow = 5;
    internal const int CursorArrow = 32512;
    internal const int CursorIBeam = 32513;
    internal const int CursorWait = 32514;
    internal const int CursorCross = 32515;
    internal const int CursorUpArrow = 32516;
    internal const int CursorSizeNorthWestSouthEast = 32642;
    internal const int CursorSizeNorthEastSouthWest = 32643;
    internal const int CursorSizeWestEast = 32644;
    internal const int CursorSizeNorthSouth = 32645;
    internal const int CursorSizeAll = 32646;
    internal const int CursorNo = 32648;
    internal const int CursorHand = 32649;
    internal const int CursorAppStarting = 32650;
    internal const int CursorHelp = 32651;

    internal const int HitTestClient = 1;
    internal const int ClassDoubleClicks = 0x0008;
    internal const int ClassOwnDc = 0x0020;
    internal const int CreateUseDefault = unchecked((int)0x80000000);
    internal const int PeekRemove = 0x0001;
    internal const int RasterOperationSourceCopy = 0x00CC0020;
    internal const int SetWindowNoActivate = 0x0010;
    internal const int SetWindowNoZOrder = 0x0004;
    internal const uint BgraBytesPerPixel = 4;
    internal const uint DibRgbColors = 0;
    internal const uint GcsCompositionString = 0x0008;
    internal const uint GcsResultString = 0x0800;
    internal const nint UiaRootObjectId = -25;
    internal const uint TrackMouseEventLeave = 0x00000002;
    internal const uint GetWheelScrollLines = 0x0068;

    [Flags]
    internal enum PixelFormatFlags : uint
    {
        DrawToWindow = 0x00000004,
        SupportOpenGl = 0x00000020,
        DoubleBuffer = 0x00000001,
    }

    internal enum PixelType : byte
    {
        Rgba = 0,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PixelFormatDescriptor
    {
        internal ushort Size;
        internal ushort Version;
        internal PixelFormatFlags Flags;
        internal PixelType PixelType;
        internal byte ColorBits;
        internal byte RedBits;
        internal byte RedShift;
        internal byte GreenBits;
        internal byte GreenShift;
        internal byte BlueBits;
        internal byte BlueShift;
        internal byte AlphaBits;
        internal byte AlphaShift;
        internal byte AccumBits;
        internal byte AccumRedBits;
        internal byte AccumGreenBits;
        internal byte AccumBlueBits;
        internal byte AccumAlphaBits;
        internal byte DepthBits;
        internal byte StencilBits;
        internal byte AuxiliaryBuffers;
        internal byte LayerType;
        internal byte Reserved;
        internal uint LayerMask;
        internal uint VisibleMask;
        internal uint DamageMask;

        internal static PixelFormatDescriptor Create() => new()
        {
            Size = checked((ushort)Marshal.SizeOf<PixelFormatDescriptor>()),
            Version = 1,
            Flags = PixelFormatFlags.DrawToWindow | PixelFormatFlags.SupportOpenGl | PixelFormatFlags.DoubleBuffer,
            PixelType = PixelType.Rgba,
            ColorBits = 32,
            AlphaBits = 8,
            DepthBits = 24,
            StencilBits = 8,
        };
    }

    [Flags]
    internal enum WindowStyles : uint
    {
        Overlapped = 0x00000000,
        Caption = 0x00C00000,
        SystemMenu = 0x00080000,
        ThickFrame = 0x00040000,
        MinimizeBox = 0x00020000,
        MaximizeBox = 0x00010000,
        Visible = 0x10000000,
        OverlappedWindow = Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox,
    }

    internal enum ShowWindowCommand
    {
        Hide = 0,
        ShowNormal = 1,
        ShowMinimized = 2,
        Restore = 9,
    }

    internal enum WindowMessage : uint
    {
        Destroy = 0x0002,
        Size = 0x0005,
        Activate = 0x0006,
        SetFocus = 0x0007,
        SetCursor = 0x0020,
        GetObject = 0x003D,
        CancelMode = 0x001F,
        KillFocus = 0x0008,
        Close = 0x0010,
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        Character = 0x0102,
        SystemKeyDown = 0x0104,
        SystemKeyUp = 0x0105,
        MouseMove = 0x0200,
        LeftButtonDown = 0x0201,
        LeftButtonUp = 0x0202,
        RightButtonDown = 0x0204,
        RightButtonUp = 0x0205,
        MiddleButtonDown = 0x0207,
        MiddleButtonUp = 0x0208,
        MouseWheel = 0x020A,
        MouseHorizontalWheel = 0x020E,
        CaptureChanged = 0x0215,
        MouseLeave = 0x02A3,
        PointerUpdate = 0x0245,
        PointerDown = 0x0246,
        PointerUp = 0x0247,
        PointerCaptureChanged = 0x024C,
        DpiChanged = 0x02E0,
        ImeStartComposition = 0x010D,
        ImeEndComposition = 0x010E,
        ImeComposition = 0x010F,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WindowClass
    {
        internal uint Size;
        internal uint Style;
        internal WindowProcedure? WindowProcedure;
        internal int ClassExtraBytes;
        internal int WindowExtraBytes;
        internal nint Instance;
        internal nint Icon;
        internal nint Cursor;
        internal nint BackgroundBrush;
        internal string? MenuName;
        internal string? ClassName;
        internal nint SmallIcon;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;

        internal readonly int Width => Right - Left;
        internal readonly int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonitorInfo
    {
        internal uint Size;
        internal NativeRect Monitor;
        internal NativeRect WorkArea;
        internal uint Flags;

        internal static MonitorInfo Create() => new() { Size = checked((uint)Marshal.SizeOf<MonitorInfo>()) };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Message
    {
        internal nint Window;
        internal uint Id;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal int PointX;
        internal int PointY;
        internal uint Private;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    internal enum PointerInputType : uint
    {
        Pointer = 1,
        Touch = 2,
        Pen = 3,
        Mouse = 4,
        Touchpad = 5,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PointerInfo
    {
        internal PointerInputType PointerType;
        internal uint PointerId;
        internal uint FrameId;
        internal uint PointerFlags;
        internal nint SourceDevice;
        internal nint WindowTarget;
        internal NativePoint PixelLocation;
        internal NativePoint HimetricLocation;
        internal NativePoint PixelLocationRaw;
        internal NativePoint HimetricLocationRaw;
        internal uint Time;
        internal uint HistoryCount;
        internal int InputData;
        internal uint KeyStates;
        internal ulong PerformanceCount;
        internal uint ButtonChangeType;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TrackMouseEventRequest
    {
        internal uint Size;
        internal uint Flags;
        internal nint Window;
        internal uint HoverTime;

        internal static TrackMouseEventRequest Leave(nint window) => new()
        {
            Size = checked((uint)Marshal.SizeOf<TrackMouseEventRequest>()),
            Flags = TrackMouseEventLeave,
            Window = window,
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BitmapInfoHeader
    {
        internal uint Size;
        internal int Width;
        internal int Height;
        internal ushort Planes;
        internal ushort BitCount;
        internal uint Compression;
        internal uint ImageSize;
        internal int PixelsPerMeterX;
        internal int PixelsPerMeterY;
        internal uint ColorsUsed;
        internal uint ColorsImportant;
    }

    internal delegate nint WindowProcedure(nint window, uint message, nuint wParam, nint lParam);

    internal delegate bool MonitorEnumerationProcedure(nint monitor, nint deviceContext, ref NativeRect rectangle, nint data);

    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern ushort RegisterClass(ref WindowClass windowClass);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint CreateWindow(
        uint extendedStyle,
        ushort classAtom,
        string? title,
        uint style,
        int x,
        int y,
        int width,
        int height,
        nint parent,
        nint menu,
        nint instance,
        nint parameter);

    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
    internal static partial nint DefaultWindowProcedure(nint window, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(nint window);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint window, ShowWindowCommand command);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UpdateWindow(nint window);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetClientRect(nint window, out NativeRect rectangle);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetWindowRect(nint window, out NativeRect rectangle);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowPos(nint window, nint insertAfter, int x, int y, int width, int height, uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumDisplayMonitors(
        nint deviceContext,
        nint clipRectangle,
        MonitorEnumerationProcedure callback,
        nint data);

    [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetMonitorInfo(nint monitor, ref MonitorInfo info);

    [LibraryImport("user32.dll")]
    internal static partial uint GetDpiForWindow(nint window);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetProcessDpiAwarenessContext(nint awarenessContext);

    [LibraryImport("user32.dll")]
    internal static partial uint GetMessageTime();

    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(int index);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetPointerInfo(uint pointerId, out PointerInfo pointerInfo);

    [LibraryImport("user32.dll")]
    internal static partial nint GetMessageExtraInfo();

    [LibraryImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SystemParametersInfo(uint action, uint parameter, out uint value, uint update);


    [LibraryImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostMessage(nint window, uint message, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    internal static partial short GetKeyState(int virtualKey);

    [LibraryImport("user32.dll")]
    internal static partial nint SetCapture(nint window);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ReleaseCapture();

    [LibraryImport("user32.dll")]
    internal static partial nint GetCapture();

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TrackMouseEvent(ref TrackMouseEventRequest request);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ScreenToClient(nint window, ref NativePoint point);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ClientToScreen(nint window, ref NativePoint point);

    [LibraryImport("user32.dll")]
    internal static partial nint SetFocus(nint window);

    [LibraryImport("user32.dll", EntryPoint = "PeekMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PeekMessage(out Message message, nint window, uint minimum, uint maximum, uint remove);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(in Message message);

    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    internal static partial nint DispatchMessage(in Message message);

    [LibraryImport("user32.dll", EntryPoint = "GetDC")]
    internal static partial nint GetDc(nint window);

    [LibraryImport("user32.dll", EntryPoint = "ReleaseDC")]
    internal static partial int ReleaseDc(nint window, nint deviceContext);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    internal static partial int ChoosePixelFormat(nint deviceContext, in PixelFormatDescriptor descriptor);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetPixelFormat(nint deviceContext, int format, in PixelFormatDescriptor descriptor);

    [LibraryImport("gdi32.dll")]
    internal static partial int GetPixelFormat(nint deviceContext);

    [LibraryImport("gdi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SwapBuffers(nint deviceContext);

    [LibraryImport("opengl32.dll", EntryPoint = "wglCreateContext", SetLastError = true)]
    internal static partial nint WglCreateContext(nint deviceContext);

    [LibraryImport("opengl32.dll", EntryPoint = "wglDeleteContext", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WglDeleteContext(nint context);

    [LibraryImport("opengl32.dll", EntryPoint = "wglMakeCurrent", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WglMakeCurrent(nint deviceContext, nint context);

    [LibraryImport("opengl32.dll", EntryPoint = "wglGetCurrentContext")]
    internal static partial nint WglGetCurrentContext();

    [LibraryImport("opengl32.dll", EntryPoint = "wglGetCurrentDC")]
    internal static partial nint WglGetCurrentDc();

    [LibraryImport("opengl32.dll", EntryPoint = "glGetString")]
    internal static partial nint GlGetString(uint name);

    [LibraryImport("gdi32.dll")]
    internal static partial int StretchDIBits(
        nint deviceContext,
        int destinationX,
        int destinationY,
        int destinationWidth,
        int destinationHeight,
        int sourceX,
        int sourceY,
        int sourceWidth,
        int sourceHeight,
        nint bits,
        in BitmapInfoHeader bitmapInfo,
        uint usage,
        uint rasterOperation);

    [LibraryImport("user32.dll", EntryPoint = "LoadCursorW")]
    internal static partial nint LoadCursor(nint instance, nint cursorName);

    [LibraryImport("user32.dll")]
    internal static partial nint SetCursor(nint cursor);

    [LibraryImport("imm32.dll")]
    internal static partial nint ImmGetContext(nint window);

    [LibraryImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ImmReleaseContext(nint window, nint context);

    [LibraryImport("imm32.dll", EntryPoint = "ImmGetCompositionStringW")]
    internal static partial int ImmGetCompositionString(nint context, uint index, nint buffer, uint bufferLength);
}
