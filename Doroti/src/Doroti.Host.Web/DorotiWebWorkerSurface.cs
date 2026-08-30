using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Web;

/// <summary>
/// Worker-owned Skia surface. The JavaScript worker presenter owns the actual
/// OffscreenCanvas/WebGL2 context and calls this singleton through JS exports.
/// </summary>
[SupportedOSPlatform("browser")]
public static partial class DorotiWebWorkerSurface
{
    private const int ResourceCacheBytes = 256 * 1024 * 1024;
    private static IDorotiBrowserTarget? _target;
    private static ulong _viewId;
    private static GRGlInterface? _glInterface;
    private static GRContext? _context;
    private static GRBackendRenderTarget? _renderTarget;
    private static SKSurface? _surface;
    private static SKSizeI _surfaceSize;
    private static int _framebuffer;
    private static long _contextGeneration;
    private static bool _initialized;

    public static void Initialize(IDorotiBrowserTarget target, ulong viewId)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (_initialized) throw new InvalidOperationException("The Doroti worker surface is already initialized.");
        InterceptBrowserObjects();
        _target = target;
        _viewId = viewId;
        _initialized = true;
        target.AttachSkiaSurface(viewId, RequestPresent);
        RequestPresent();
    }

    [JSExport]
    public static string RenderFrame(
        [JSMarshalAs<JSType.Number>] long requestId,
        [JSMarshalAs<JSType.Number>] long generation,
        double logicalWidth,
        double logicalHeight,
        int physicalWidth,
        int physicalHeight,
        double devicePixelRatio,
        [JSMarshalAs<JSType.Number>] long timestampMicroseconds,
        int framebuffer,
        int stencilBits,
        int sampleCount,
        [JSMarshalAs<JSType.Number>] long contextGeneration,
        bool glStateDirty)
    {
        if (!_initialized || _target is null) return "superseded";
        var target = new DorotiResizeEpoch(
            generation, logicalWidth, logicalHeight, physicalWidth, physicalHeight,
            devicePixelRatio, timestampMicroseconds);
        if (_target.CaptureSnapshot(_viewId).ResizeEpoch.Generation != generation)
            return "superseded";
        EnsureSurface(physicalWidth, physicalHeight, framebuffer, stencilBits, sampleCount, contextGeneration);
        if (glStateDirty) _context!.ResetContext(GRGlBackendState.All);
        string result;
        using (new SKAutoCanvasRestore(_surface!.Canvas, true))
            result = _target.PaintSkiaSurface(
                _viewId, _surface, physicalWidth, physicalHeight, target, requestId);
        _surface.Canvas.Flush();
        _context!.Flush();
        return result;
    }

    [JSExport]
    public static void CompleteFrame(
        [JSMarshalAs<JSType.Number>] long requestId,
        [JSMarshalAs<JSType.Number>] long generation,
        string terminal,
        string reason)
    {
        _ = generation;
        _target?.CompleteSkiaSurfacePaint(_viewId, requestId, generation, terminal, reason);
    }

    [JSExport]
    public static void ContextLost(
        [JSMarshalAs<JSType.Number>] long requestId,
        [JSMarshalAs<JSType.Number>] long generation)
    {
        _ = generation;
        _target?.InvalidateSkiaGpuContext(
            _viewId, requestId, "worker WebGL context lost");
        ReleaseGpu();
    }

    [JSExport]
    public static void ContextRestored()
    {
        ReleaseGpu();
        RequestPresent();
    }

    public static void Dispose()
    {
        _initialized = false;
        ReleaseGpu();
        _target = null;
        _viewId = 0;
    }

    private static void RequestPresent()
    {
        if (!_initialized || _target is null) return;
        var target = _target.CaptureSnapshot(_viewId).ResizeEpoch;
        BrowserInterop.RequestPresent(
            canvasId: "doroti-surface",
            generation: target.Generation,
            logicalWidth: target.LogicalWidth,
            logicalHeight: target.LogicalHeight,
            physicalWidth: target.PhysicalWidth,
            physicalHeight: target.PhysicalHeight,
            devicePixelRatio: target.DevicePixelRatio,
            timestampMicroseconds: target.TimestampMicroseconds);
    }

    private static void EnsureSurface(
        int width, int height, int framebuffer, int stencilBits, int sampleCount, long contextGeneration)
    {
        if (_context is not null && _contextGeneration != contextGeneration) ReleaseGpu();
        if (_context is null)
        {
            _glInterface = GRGlInterface.Create();
            _context = GRContext.CreateGl(_glInterface)
                ?? throw new InvalidOperationException("Doroti could not create the worker WebGL Skia context.");
            _context.SetResourceCacheLimit(ResourceCacheBytes);
            _contextGeneration = contextGeneration;
        }
        var size = new SKSizeI(width, height);
        if (_renderTarget is null || _surfaceSize != size || _framebuffer != framebuffer || !_renderTarget.IsValid)
        {
            if (_renderTarget is not null) _target?.InvalidateSkiaWindowSurface(_viewId);
            _surface?.Dispose();
            _surface = null;
            _renderTarget?.Dispose();
            var glInfo = new GRGlFramebufferInfo((uint)framebuffer, SKColorType.Rgba8888.ToGlSizedFormat());
            _renderTarget = new GRBackendRenderTarget(
                width, height, Math.Max(0, sampleCount), Math.Max(0, stencilBits), glInfo);
            _surfaceSize = size;
            _framebuffer = framebuffer;
        }
        _surface ??= SKSurface.Create(
            _context, _renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888)
            ?? throw new InvalidOperationException("Doroti could not wrap the worker WebGL framebuffer.");
    }

    private static void ReleaseGpu()
    {
        _surface?.Dispose();
        _surface = null;
        _renderTarget?.Dispose();
        _renderTarget = null;
        _context?.AbandonContext();
        _context?.Dispose();
        _context = null;
        _glInterface?.Dispose();
        _glInterface = null;
        _surfaceSize = SKSizeI.Empty;
        _framebuffer = 0;
        _contextGeneration = 0;
    }

    [DllImport("libSkiaSharp", EntryPoint = "DorotiInterceptBrowserObjects")]
    private static extern void InterceptBrowserObjects();
}
