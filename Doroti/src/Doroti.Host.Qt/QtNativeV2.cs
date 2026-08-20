using System.Runtime.InteropServices;

namespace Doroti.Host.Qt;

internal static unsafe class QtNativeV2
{
    internal const uint AbiVersion = 2;
    internal const ulong RequiredFeatures =
        (1UL << 0) | (1UL << 1) | (1UL << 2) | (1UL << 3) |
        (1UL << 4) | (1UL << 5) | (1UL << 6) | (1UL << 7) | (1UL << 8) | (1UL << 9);

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
    internal readonly struct Metrics
    {
        internal readonly uint AbiVersion;
        internal readonly uint StructSize;
        internal readonly ulong SurfaceGeneration;
        internal readonly int PixelWidth;
        internal readonly int PixelHeight;
        internal readonly double DevicePixelRatio;
        internal readonly uint LifecycleState;
        internal readonly uint Reserved;
        internal readonly ulong MetricsGeneration;
        internal readonly long TimestampMicroseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Pointer
    {
        internal readonly uint AbiVersion;
        internal readonly uint StructSize;
        internal readonly ulong Device;
        internal readonly ulong PointerIdentifier;
        internal readonly uint Change;
        internal readonly uint Kind;
        internal readonly long Buttons;
        internal readonly double PhysicalX;
        internal readonly double PhysicalY;
        internal readonly double PhysicalDeltaX;
        internal readonly double PhysicalDeltaY;
        internal readonly double Pressure;
        internal readonly double Tilt;
        internal readonly uint SignalKind;
        internal readonly uint PlatformData;
        internal readonly double ScrollDeltaX;
        internal readonly double ScrollDeltaY;
        internal readonly long TimestampMicroseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Key
    {
        internal readonly uint AbiVersion;
        internal readonly uint StructSize;
        internal readonly long Physical;
        internal readonly long Logical;
        internal readonly uint Type;
        internal readonly uint Modifiers;
        internal readonly Utf8 Character;
        internal readonly long TimestampMicroseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct TextConfiguration(
        uint inputType, uint inputAction, uint capitalization, bool readOnly,
        bool obscureText, bool autocorrect, bool enableSuggestions)
    {
        internal readonly uint AbiVersion = QtNativeV2.AbiVersion;
        internal readonly uint StructSize = checked((uint)sizeof(TextConfiguration));
        internal readonly uint InputType = inputType;
        internal readonly uint InputAction = inputAction;
        internal readonly uint Capitalization = capitalization;
        internal readonly uint ReadOnly = readOnly ? 1u : 0u;
        internal readonly uint ObscureText = obscureText ? 1u : 0u;
        internal readonly uint Autocorrect = autocorrect ? 1u : 0u;
        internal readonly uint EnableSuggestions = enableSuggestions ? 1u : 0u;
        internal readonly uint Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct TextState(Utf8 text, int selectionBase, int selectionExtent,
        int composingBase, int composingExtent)
    {
        internal readonly uint AbiVersion = QtNativeV2.AbiVersion;
        internal readonly uint StructSize = checked((uint)sizeof(TextState));
        internal readonly Utf8 Text = text;
        internal readonly int SelectionBase = selectionBase;
        internal readonly int SelectionExtent = selectionExtent;
        internal readonly int ComposingBase = composingBase;
        internal readonly int ComposingExtent = composingExtent;
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
        internal readonly delegate* unmanaged[Cdecl]<nint, double, double, void> Resize;
        internal readonly delegate* unmanaged[Cdecl]<nint, Utf8, void> SetClipboardText;
        internal readonly delegate* unmanaged[Cdecl]<nint, ulong, void> RequestClipboardText;
        internal readonly delegate* unmanaged[Cdecl]<nint, uint, void> SetCursor;
        internal readonly delegate* unmanaged[Cdecl]<nint, TextConfiguration*, TextState*, void> SetTextClient;
        internal readonly delegate* unmanaged[Cdecl]<nint, TextState*, void> UpdateTextState;
        internal readonly delegate* unmanaged[Cdecl]<nint, double, double, double, double, void> SetCaretRect;
        internal readonly delegate* unmanaged[Cdecl]<nint, void> ClearTextClient;
        internal readonly delegate* unmanaged[Cdecl]<nint, Utf8, void> UpdateSemantics;
        internal readonly delegate* unmanaged[Cdecl]<nint, void> ClearSemantics;
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
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, Metrics*, void> MetricsChanged;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, uint, long, void> LifecycleChanged;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, void> CloseRequested;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, void> Closed;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, Pointer*, void> Pointer;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, Key*, void> Key;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, uint, long, void> Focus;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, TextState*, void> TextEditing;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, uint, void> TextAction;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, ulong, Utf8, void> ClipboardText;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, Utf8, uint, uint, void> ConfigurationChanged;
        internal readonly delegate* unmanaged[Cdecl]<nint, nint, long, long, Utf8, void> SemanticsAction;

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
            MetricsChanged = &DorotiQtRunner.OnMetricsChanged;
            LifecycleChanged = &DorotiQtRunner.OnLifecycleChanged;
            CloseRequested = &DorotiQtRunner.OnCloseRequested;
            Closed = &DorotiQtRunner.OnClosed;
            Pointer = &DorotiQtRunner.OnPointer;
            Key = &DorotiQtRunner.OnKey;
            Focus = &DorotiQtRunner.OnFocus;
            TextEditing = &DorotiQtRunner.OnTextEditing;
            TextAction = &DorotiQtRunner.OnTextAction;
            ClipboardText = &DorotiQtRunner.OnClipboardText;
            ConfigurationChanged = &DorotiQtRunner.OnConfigurationChanged;
            SemanticsAction = &DorotiQtRunner.OnSemanticsAction;
        }
    }

    internal static void ValidateLayout()
    {
        RequireSize<Utf8>(16);
        RequireSize<Configuration>(40);
        RequireSize<Surface>(88);
        RequireSize<Metrics>(56);
        RequireSize<Pointer>(120);
        RequireSize<Key>(56);
        RequireSize<TextConfiguration>(40);
        RequireSize<TextState>(40);
        RequireSize<HostApi>(120);
        RequireSize<Callbacks>(176);
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
