using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Web;

/// <summary>
/// Transitional browser graphics boundary.  The existing Skia implementation
/// and the CanvasKit DisplayList implementation can be selected per runtime
/// without leaking their ownership decisions into the framework host.
/// </summary>
internal interface IBrowserGraphicsCapabilities :
    ISceneHostCapability,
    IParagraphHostCapability,
    IImageHostCapability,
    ISemanticsHostCapability,
    IDisposable
{
    new bool CoalesceGeometryDuringActiveMetrics { get; }

    BrowserFrameDiagnostics Diagnostics { get; }

    void AttachSurface(Action invalidate);

    void AttachFrameworkTrace(DorotiFrameTrace trace);

    string Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        DorotiResizeEpoch target,
        long requestId);

    void CompletePaint(long requestId, string terminal, string reason);

    void InvalidateGpuContext(long requestId, string reason);

    void InvalidateWindowSurfaceResources();
}
