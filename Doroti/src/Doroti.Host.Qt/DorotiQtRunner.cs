using System.Runtime.InteropServices;
using Doroti.Hosting;

namespace Doroti.Host.Qt;

/// <summary>
/// Owns the managed process and hands a stable callback table to the app-owned Qt shim.
/// The C ABI deliberately contains no Qt types, so Qt remains a native implementation detail.
/// </summary>
public static partial class DorotiQtRunner
{
    private const string NativeLibrary = "doroti_qt_host";

    public static int Run(DorotiApplicationDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        if (!OperatingSystem.IsLinux())
        {
            throw new PlatformNotSupportedException(
                "Doroti.Host.Qt can only launch on Linux. Build/publish graph validation may run on another host.");
        }

        using var application = DorotiApplicationBoundary.Load(
            descriptor.ManifestAssembly,
            descriptor.ApplicationAssembly,
            descriptor.LaunchContext.RuntimeIdentifier);
        using var session = new DorotiHostSession(descriptor.EntrypointFactory());
        var state = new QtManagedState(session);
        var stateHandle = GCHandle.Alloc(state);
        try
        {
            session.Start(deferFrameworkBootstrap: true);
            var callbacks = QtCallbacks.Create();
            var configuration = descriptor.ViewConfiguration;
            return NativeMethods.Run(
                configuration.title,
                checked((int)configuration.logicalSize.width),
                checked((int)configuration.logicalSize.height),
                GCHandle.ToIntPtr(stateHandle),
                in callbacks);
        }
        finally
        {
            stateHandle.Free();
        }
    }

    private sealed class QtManagedState(DorotiHostSession session)
    {
        public DorotiHostSession Session { get; } = session;
        public long SurfaceGeneration;
        public long PresentedFrames;
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly unsafe struct QtCallbacks
    {
        private readonly delegate* unmanaged[Cdecl]<nint, double, void> _frame;
        private readonly delegate* unmanaged[Cdecl]<nint, int, int, double, long, void> _resize;
        private readonly delegate* unmanaged[Cdecl]<nint, int, void> _lifecycle;
        private readonly delegate* unmanaged[Cdecl]<nint, int, double, double, int, long, void> _pointer;
        private readonly delegate* unmanaged[Cdecl]<nint, int, int, int, long, void> _key;
        private readonly delegate* unmanaged[Cdecl]<nint, nint, void> _text;
        private readonly delegate* unmanaged[Cdecl]<nint, long, void> _surfaceChanged;

        private QtCallbacks(
            delegate* unmanaged[Cdecl]<nint, double, void> frame,
            delegate* unmanaged[Cdecl]<nint, int, int, double, long, void> resize,
            delegate* unmanaged[Cdecl]<nint, int, void> lifecycle,
            delegate* unmanaged[Cdecl]<nint, int, double, double, int, long, void> pointer,
            delegate* unmanaged[Cdecl]<nint, int, int, int, long, void> key,
            delegate* unmanaged[Cdecl]<nint, nint, void> text,
            delegate* unmanaged[Cdecl]<nint, long, void> surfaceChanged)
        {
            _frame = frame;
            _resize = resize;
            _lifecycle = lifecycle;
            _pointer = pointer;
            _key = key;
            _text = text;
            _surfaceChanged = surfaceChanged;
        }

        public static QtCallbacks Create() => new(&OnFrame, &OnResize, &OnLifecycle, &OnPointer, &OnKey, &OnText, &OnSurfaceChanged);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static void OnFrame(nint context, double timestampSeconds)
    {
        _ = timestampSeconds;
        GetState(context).PresentedFrames++;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static void OnResize(nint context, int pixelWidth, int pixelHeight, double scale, long surfaceGeneration)
    {
        _ = (pixelWidth, pixelHeight, scale);
        GetState(context).SurfaceGeneration = surfaceGeneration;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static void OnLifecycle(nint context, int lifecycle) => _ = (GetState(context), lifecycle);

    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static void OnPointer(nint context, int kind, double x, double y, int buttons, long timestampMicroseconds) =>
        _ = (GetState(context), kind, x, y, buttons, timestampMicroseconds);

    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static void OnKey(nint context, int key, int scanCode, int modifiers, long timestampMicroseconds) =>
        _ = (GetState(context), key, scanCode, modifiers, timestampMicroseconds);

    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static void OnText(nint context, nint utf8Text) => _ = (GetState(context), utf8Text);

    [UnmanagedCallersOnly(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    private static void OnSurfaceChanged(nint context, long surfaceGeneration) =>
        GetState(context).SurfaceGeneration = surfaceGeneration;

    private static QtManagedState GetState(nint context) =>
        (QtManagedState)(GCHandle.FromIntPtr(context).Target
            ?? throw new InvalidOperationException("The Qt managed callback context is no longer available."));

    private static partial class NativeMethods
    {
        [LibraryImport(NativeLibrary, EntryPoint = "doroti_qt_run", StringMarshalling = StringMarshalling.Utf8)]
        internal static partial int Run(
            string title,
            int width,
            int height,
            nint managedContext,
            in QtCallbacks callbacks);
    }
}
