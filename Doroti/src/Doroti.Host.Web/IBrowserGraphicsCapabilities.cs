using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Web;

/// <summary>
/// Browser Skia graphics capabilities used by the framework host.
/// </summary>
internal interface IBrowserGraphicsCapabilities :
    ISceneHostCapability,
    IParagraphHostCapability,
    IFontHostCapability,
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
