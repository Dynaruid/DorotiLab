using Doroti.Hosting;
using Doroti.Platform;
using Doroti.Ui;

namespace Doroti.Host.Desktop.Framework;

/// <summary>Common package-target surface used by generated desktop consumers.</summary>
public interface IDesktopFrameworkTarget : IDisposable
{
    string Rid { get; }
    string GraphicsBackend { get; }
    DorotiView CreateView(DorotiHostSession session, ulong viewId, DorotiViewConfiguration configuration,
        DorotiApplicationBoundary? application = null);
    void PumpPendingMessages();
    DesktopFrameworkTargetDiagnostics CaptureDiagnostics(ulong viewId);
    Task<DesktopFrameworkPixelReadback> CaptureNextFrameAsync(ulong viewId);
    NativeResourceSnapshot CaptureResourceSnapshot();
    nint GetNativeWindowHandle(ulong viewId);
    void PostPointerMoveForValidation(ulong viewId, double logicalX, double logicalY);
    void PostPointerDownForValidation(ulong viewId, double logicalX, double logicalY);
    void PostPointerUpForValidation(ulong viewId, double logicalX, double logicalY);
    void PostPointerTapForValidation(ulong viewId, double logicalX, double logicalY);
}
