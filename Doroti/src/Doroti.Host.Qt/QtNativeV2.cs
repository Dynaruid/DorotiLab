using System.Runtime.InteropServices;

namespace Doroti.Host.Qt;

internal static unsafe class QtNativeV2
{
    internal const uint AbiVersion = 2;
    internal const ulong RequiredFeatures =
        (1UL << 0) | (1UL << 1) | (1UL << 2) | (1UL << 3);

    internal enum Result : int
    {
        Ok = 0,
        ManagedFatal = 69,
    }

    internal enum TerminalState : uint
    {
        Presented = 1,
        Replayed = 2,
        Superseded = 3,
        Failed = 4,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Utf8(byte* data, ulong length)
    {
        internal readonly byte* Data = data;
        internal readonly ulong Length = length;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Configuration(Utf8 title, int logicalWidth, int logicalHeight)
    {
        internal readonly uint AbiVersion = QtNativeV2.AbiVersion;
        internal readonly uint StructSize = checked((uint)sizeof(Configuration));
        internal readonly ulong RequiredFeatures = QtNativeV2.RequiredFeatures;
        internal readonly Utf8 Title = title;
        internal readonly int LogicalWidth = logicalWidth;
        internal readonly int LogicalHeight = logicalHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Surface
    {
        internal readonly uint AbiVersion;
        internal readonly uint StructSize;
        internal readonly ulong SurfaceGeneration;
        internal readonly ulong ContextIdentity;
        internal readonly uint FramebufferObject;
        internal readonly int PixelWidth;
        internal readonly int PixelHeight;
        internal readonly double DevicePixelRatio;
        internal readonly int SampleCount;
        internal readonly int StencilBits;
        internal readonly uint ColorFormat;
        internal readonly uint GlApi;
        internal readonly uint GlProfile;
        internal readonly int GlMajor;
        internal readonly int GlMinor;
        internal readonly long TimestampMicroseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct HostApi
    {
        internal readonly uint AbiVersion;
        internal readonly uint StructSize;
        internal readonly ulong FeatureBits;
        internal readonly delegate* unmanaged[Cdecl]<nint, ulong, void> RequestFrame;
        internal readonly delegate* unmanaged[Cdecl]<nint, void> RequestClose;
        internal readonly delegate* unmanaged[Cdecl]<nint, Utf8, nint> GetGlProcAddress;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Callbacks
    {
        internal readonly uint AbiVersion;
        internal readonly uint StructSize;
        internal readonly ulong RequiredFeatures;
        internal readonly ulong FeatureBits;
        internal readonly nint CallbackContext;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, HostApi*, int> ViewCreated;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, Surface*, ulong, int> Render;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, ulong, uint, ulong, long, void> FrameTerminal;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, ulong, ulong, void> SurfaceDestroying;
        internal readonly delegate* unmanaged[Cdecl]<nint, Utf8, Utf8, void> Diagnostic;
        internal readonly delegate* unmanaged[Cdecl]<nint, int, Utf8, void> Fatal;

        internal Callbacks(nint callbackContext)
        {
            AbiVersion = QtNativeV2.AbiVersion;
            StructSize = checked((uint)sizeof(Callbacks));
            RequiredFeatures = QtNativeV2.RequiredFeatures;
            FeatureBits = QtNativeV2.RequiredFeatures;
            CallbackContext = callbackContext;
            ViewCreated = &DorotiQtRunner.OnViewCreated;
            Render = &DorotiQtRunner.OnRender;
            FrameTerminal = &DorotiQtRunner.OnFrameTerminal;
            SurfaceDestroying = &DorotiQtRunner.OnSurfaceDestroying;
            Diagnostic = &DorotiQtRunner.OnDiagnostic;
            Fatal = &DorotiQtRunner.OnFatal;
        }
    }

    internal static void ValidateLayout()
    {
        RequireSize<Utf8>(16);
        RequireSize<Configuration>(40);
        RequireSize<Surface>(88);
        RequireOffset<Surface>(nameof(Surface.SurfaceGeneration), 8);
        RequireOffset<Surface>(nameof(Surface.FramebufferObject), 24);
        RequireOffset<Surface>(nameof(Surface.DevicePixelRatio), 40);
        RequireOffset<Surface>(nameof(Surface.TimestampMicroseconds), 80);
        RequireOffset<Callbacks>(nameof(Callbacks.CallbackContext), 24);
    }

    private static void RequireSize<T>(int expected) where T : unmanaged
    {
        var actual = Marshal.SizeOf<T>();
        if (actual != expected)
            throw new TypeLoadException($"Qt ABI v2 {typeof(T).Name} size is {actual}; expected {expected}.");
    }

    private static void RequireOffset<T>(string field, int expected) where T : unmanaged
    {
        var actual = Marshal.OffsetOf<T>(field).ToInt32();
        if (actual != expected)
            throw new TypeLoadException($"Qt ABI v2 {typeof(T).Name}.{field} offset is {actual}; expected {expected}.");
    }
}
