using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Doroti.Host.WindowsAppSdk;

internal static partial class WindowsNativeV1
{
    internal const uint AbiVersion = 1;
    internal const string LibraryName = "doroti_windows_appsdk_host_v1";

    internal enum Status : uint
    {
        Ok = 0,
        InvalidArgument = 1,
        AbiMismatch = 2,
        NotImplemented = 3,
        NativeFailure = 4,
    }

    internal enum FrameTerminalKind : uint
    {
        Presented = 1,
        Superseded = 2,
        Failed = 3,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Utf8
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal nint Data;
        internal ulong ByteLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Metrics
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal ulong Generation;
        internal uint WidthPx;
        internal uint HeightPx;
        internal double Scale;
        internal double LogicalWidth;
        internal double LogicalHeight;
        internal ulong DisplayId;
        internal long TimestampQpc;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct FrameRequest
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal ulong Generation;
        internal uint WidthPx;
        internal uint HeightPx;
        internal ulong CausalFrameId;
        internal long TimestampQpc;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct FrameTerminal
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal ulong Generation;
        internal ulong CausalFrameId;
        internal uint TerminalKind;
        internal uint ErrorCategory;
        internal long AcceptedQpc;
        internal long TerminalQpc;
        internal uint PlatformWaitTimedOut;
        internal uint Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Pointer
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal long TimestampQpc;
        internal uint Change;
        internal uint Kind;
        internal long Device;
        internal double PhysicalX;
        internal double PhysicalY;
        internal double PhysicalDeltaX;
        internal double PhysicalDeltaY;
        internal long Buttons;
        internal double ScrollDeltaX;
        internal double ScrollDeltaY;
        internal uint SignalKind;
        internal uint PointerIdentifier;
        internal double Pressure;
        internal double Tilt;
        internal long PlatformData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Key
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong ViewId;
        internal long TimestampQpc;
        internal uint Type;
        internal uint Repeat;
        internal long Physical;
        internal long Logical;
        internal Utf8 Character;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Host
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal nint HostContext;
        internal nint TopLevelHwnd;
        internal nint ChildHwnd;
        internal nint TaskHwnd;
        internal nint RequestFrame;
        internal nint RequestResize;
        internal nint RequestClose;
        internal nint RequestShow;
        internal nint SetCursor;
        internal nint SetClipboard;
        internal nint RequestClipboard;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Configuration
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal ulong RequiredFeatures;
        internal Utf8 ApplicationId;
        internal Utf8 Title;
        internal uint InitialWidthPx;
        internal uint InitialHeightPx;
        internal uint NCmdShow;
        internal uint Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct Callbacks
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal nint CallbackContext;
        internal nint HostReady;
        internal nint Metrics;
        internal nint Render;
        internal nint FrameTerminal;
        internal nint Log;
        internal nint Pointer;
        internal nint Key;
        internal nint Focus;
        internal nint Clipboard;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct AbiLayout
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal uint PointerSize;
        internal uint Packing;
        internal uint Utf8Size;
        internal uint MetricsSize;
        internal uint FrameRequestSize;
        internal uint HostSize;
        internal uint FrameTerminalSize;
        internal uint ConfigurationSize;
        internal uint CallbacksSize;
        internal uint MetricsGenerationOffset;
        internal uint HostChildHwndOffset;
        internal uint TerminalKindOffset;
        internal uint CallbacksRenderOffset;
        internal uint GpuPointerCount;
        internal uint PointerPacketSize;
        internal uint KeySize;
        internal uint CallbacksPointerOffset;
        internal uint HostSetCursorOffset;
    }

    [LibraryImport(LibraryName, EntryPoint = "doroti_windows_get_abi_version_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint GetAbiVersion();

    [LibraryImport(LibraryName, EntryPoint = "doroti_windows_get_abi_layout_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial Status GetAbiLayout(ref AbiLayout layout);

    [LibraryImport(LibraryName, EntryPoint = "doroti_windows_run_v1")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial Status Run(in Configuration configuration, in Callbacks callbacks);

    internal static AbiLayout ValidateLayout()
    {
        if (GetAbiVersion() != AbiVersion)
            throw new InvalidOperationException("Native host ABI version mismatch.");
        var layout = new AbiLayout
        {
            AbiVersion = AbiVersion,
            StructSize = checked((uint)Marshal.SizeOf<AbiLayout>()),
        };
        var status = GetAbiLayout(ref layout);
        if (status != Status.Ok)
            throw new InvalidOperationException($"Native ABI layout query failed: {status}.");

        AssertEqual("pointer size", checked((uint)IntPtr.Size), layout.PointerSize);
        AssertEqual("packing", 8, layout.Packing);
        AssertEqual("UTF-8 size", SizeOf<Utf8>(), layout.Utf8Size);
        AssertEqual("metrics size", SizeOf<Metrics>(), layout.MetricsSize);
        AssertEqual("frame request size", SizeOf<FrameRequest>(), layout.FrameRequestSize);
        AssertEqual("host size", SizeOf<Host>(), layout.HostSize);
        AssertEqual("frame terminal size", SizeOf<FrameTerminal>(), layout.FrameTerminalSize);
        AssertEqual("configuration size", SizeOf<Configuration>(), layout.ConfigurationSize);
        AssertEqual("callbacks size", SizeOf<Callbacks>(), layout.CallbacksSize);
        AssertEqual("metrics generation offset", OffsetOf<Metrics>(nameof(Metrics.Generation)), layout.MetricsGenerationOffset);
        AssertEqual("host child HWND offset", OffsetOf<Host>(nameof(Host.ChildHwnd)), layout.HostChildHwndOffset);
        AssertEqual("terminal kind offset", OffsetOf<FrameTerminal>(nameof(FrameTerminal.TerminalKind)), layout.TerminalKindOffset);
        AssertEqual("callbacks render offset", OffsetOf<Callbacks>(nameof(Callbacks.Render)), layout.CallbacksRenderOffset);
        AssertEqual("GPU pointer count", 0, layout.GpuPointerCount);
        AssertEqual("pointer packet size", SizeOf<Pointer>(), layout.PointerPacketSize);
        AssertEqual("key packet size", SizeOf<Key>(), layout.KeySize);
        AssertEqual("callbacks pointer offset", OffsetOf<Callbacks>(nameof(Callbacks.Pointer)), layout.CallbacksPointerOffset);
        AssertEqual("host set-cursor offset", OffsetOf<Host>(nameof(Host.SetCursor)), layout.HostSetCursorOffset);
        return layout;
    }

    private static uint SizeOf<T>() where T : struct => checked((uint)Marshal.SizeOf<T>());
    private static uint OffsetOf<T>(string field) where T : struct =>
        checked((uint)Marshal.OffsetOf<T>(field).ToInt64());

    private static void AssertEqual(string name, uint expected, uint actual)
    {
        if (expected != actual)
            throw new InvalidOperationException($"Native ABI {name} mismatch: managed={expected}, native={actual}.");
    }
}
