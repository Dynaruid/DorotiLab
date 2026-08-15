using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;
using Doroti.Platform;

namespace Doroti.Host.Desktop.Flutter;

/// <summary>Common package-target surface used by generated desktop consumers.</summary>
public interface IDesktopFlutterTarget : IDisposable
{
    string Rid { get; }
    string GraphicsBackend { get; }
    FlutterView CreateView(FlutterHostSession session, ulong viewId, FlutterViewConfiguration configuration,
        FlutterApplicationBoundary? application = null);
    void PumpPendingMessages();
    DesktopFlutterTargetDiagnostics CaptureDiagnostics(ulong viewId);
    Task<DesktopFlutterPixelReadback> CaptureNextFrameAsync(ulong viewId);
    NativeResourceSnapshot CaptureResourceSnapshot();
    nint GetNativeWindowHandle(ulong viewId);
    void PostPointerMoveForValidation(ulong viewId, double logicalX, double logicalY);
    void PostPointerDownForValidation(ulong viewId, double logicalX, double logicalY);
    void PostPointerUpForValidation(ulong viewId, double logicalX, double logicalY);
    void PostPointerTapForValidation(ulong viewId, double logicalX, double logicalY);
}
