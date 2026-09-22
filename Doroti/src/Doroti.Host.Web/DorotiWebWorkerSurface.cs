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
    private static int ResourceCacheBytes = 256 * 1024 * 1024;
    internal static long PictureRasterCachePixels { get; private set; } = 16L * 1024 * 1024;
    private static bool _mobileMemoryProfile;

    [JSExport]
    public static void ConfigureMemoryProfile(bool mobile, bool webgpu)
    {
        if (_initialized || _context is not null || _graphiteContext is not null)
            throw new InvalidOperationException(
                "Memory profile must be selected before renderer creation."
            );
        _mobileMemoryProfile = mobile;
        // The 64MiB/16MiB mobile trial increased Graphite frame latency and
        // did not bound its active native working set. Keep its established
        // budgets until that backend has independent performance evidence.
        var compact = mobile && !webgpu;
        ResourceCacheBytes = (compact ? 64 : 256) * 1024 * 1024;
        PictureRasterCachePixels = (compact ? 4L : 16L) * 1024 * 1024;
    }

    private static IDorotiBrowserTarget? _target;
    private static ulong _viewId;
    private static GRGlInterface? _glInterface;
    private static GRContext? _context;
    private static GRBackendRenderTarget? _renderTarget;
    private static SKSurface? _surface;
    private static SKSizeI _surfaceSize;
    private static int _framebuffer;
    private static int _sampleCount;
    private static int _stencilBits;
    private static long _contextGeneration;
    private static bool _initialized;

    public static void Initialize(IDorotiBrowserTarget target, ulong viewId)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (_initialized)
        {
            throw new InvalidOperationException(
                "The Doroti worker surface is already initialized."
            );
        }

        InterceptBrowserObjects();
        _target = target;
        _viewId = viewId;
        _initialized = true;
        target.AttachSkiaSurface(viewId, RequestPresent);
        RequestPresent();
    }

    [JSExport]
    public static string CaptureDiagnostics() =>
        System.Text.Json.JsonSerializer.Serialize(
            new
            {
                clockMicroseconds = DorotiFrameClock.Now.Ticks / 10,
                frame = _target?.CaptureFrameDiagnostics(_viewId),
                work = FrameworkWorkCounters.Snapshot(),
                profile = FrameworkWorkProfile.Snapshot(),
                components = FrameworkComponentProfile.Snapshot(),
                backend = _graphiteContext is null ? "Ganesh/WebGL" : "Graphite/Dawn",
                nativeSkia = SkiaSharpVersion.Native.ToString(),
                managedSkia = typeof(SKSurface).Assembly.GetName().Version?.ToString(),
                graphiteImageProviderCalls = _graphiteImageRequests,
                graphiteBudgetedBytes = _graphiteContext?.CurrentBudgetedBytes ?? 0,
            }
        );

    private static object? CaptureGaneshMemory()
    {
        if (_context is null)
            return null;
        _context.GetResourceCacheUsage(out var count, out var bytes);
        return new { resourceCount = count, budgetedBytes = bytes };
    }

    [JSExport]
    public static void TrimUnusedGpuResources()
    {
        // Called between owner frames. Skia only purges resources whose native
        // references are no longer in use; pending recordings/maps stay owned.
        _graphiteContext?.CheckAsyncWorkCompletion();
        _graphiteContext?.PerformDeferredCleanup(TimeSpan.FromSeconds(5));
        _context?.PurgeUnusedResources(5000);
    }

    private static long _costOwnerAllocated;
    private static long _costTotalAllocated;

    [JSExport]
    public static void BeginCostInterval()
    {
        _costOwnerAllocated = GC.GetAllocatedBytesForCurrentThread();
        _costTotalAllocated = GC.GetTotalAllocatedBytes();
    }

    [JSExport]
    public static string CaptureCostDiagnostics()
    {
        // Read allocation before creating any export objects. BeginCostInterval
        // runs after the previous export so serialization is outside the delta.
        var ownerAllocatedBytes = GC.GetAllocatedBytesForCurrentThread() - _costOwnerAllocated;
        var totalAllocatedBytes = GC.GetTotalAllocatedBytes() - _costTotalAllocated;
        var frame = _target?.CaptureFrameDiagnostics(_viewId);
        var skia = frame?.Skia;
        return System.Text.Json.JsonSerializer.Serialize(
            new
            {
                clockMicroseconds = DorotiFrameClock.Now.Ticks / 10,
                ownerAllocatedBytes,
                totalAllocatedBytes,
                managedHeapBytes = GC.GetTotalMemory(false),
                memoryProfile = _mobileMemoryProfile ? "mobile" : "desktop",
                nativeCacheBudgetBytes = ResourceCacheBytes,
                graphiteContextBudgetedBytes = _graphiteContext?.CurrentBudgetedBytes,
                graphiteContextLimitBytes = _graphiteContext?.MaxBudgetedBytes,
                graphiteRecorderBudgetedBytes = "notMeasured: installed API does not expose recorder usage",
                ganesh = CaptureGaneshMemory(),
                cache = TextureRenderers.TryGetValue(_viewId, out var renderer)
                    ? renderer.CaptureCacheMemory()
                    : null,
                gcCollections = new[]
                {
                    GC.CollectionCount(0),
                    GC.CollectionCount(1),
                    GC.CollectionCount(2),
                },
                enabled = FrameworkWorkCounters.Enabled,
                layoutEnabled = FrameworkWorkProfile.LayoutEnabled,
                allocationEnabled = FrameworkWorkProfile.AllocationEnabled,
                entries = FrameworkWorkProfile.CaptureEntries(),
                components = FrameworkComponentProfile.Snapshot(),
                frame = frame is null ? null : frame with { Skia = null },
                skia = skia is null ? null : skia with { Trace = [] },
                scroll = skia
                    ?.Trace.Where(e => e.ScrollOffset is not null)
                    .GroupBy(e => e.ScrollPositionId)
                    .Select(g => g.Last())
                    .ToArray(),
            }
        );
    }

    [JSExport]
    public static string RenderFrame(
        [JSMarshalAs<JSType.Number>] long requestId,
        [JSMarshalAs<JSType.Number>] long generation,
        double logicalWidth,
        double logicalHeight,
        int physicalWidth,
        int physicalHeight,
        int backingWidth,
        int backingHeight,
        double devicePixelRatio,
        [JSMarshalAs<JSType.Number>] long timestampMicroseconds,
        int framebuffer,
        int stencilBits,
        int sampleCount,
        [JSMarshalAs<JSType.Number>] long contextGeneration,
        bool glStateDirty
    )
    {
        if (!_initialized || _target is null)
        {
            return "superseded";
        }

        var target = new DorotiResizeEpoch(
            generation,
            logicalWidth,
            logicalHeight,
            physicalWidth,
            physicalHeight,
            devicePixelRatio,
            timestampMicroseconds
        );
        if (_target.CaptureSnapshot(_viewId).ResizeEpoch.Generation != generation)
        {
            return "superseded";
        }

        if (backingWidth < physicalWidth || backingHeight < physicalHeight)
        {
            throw new InvalidDataException(
                $"Worker backing {backingWidth}x{backingHeight} is smaller than exact frame "
                    + $"{physicalWidth}x{physicalHeight}."
            );
        }

        EnsureSurface(
            backingWidth,
            backingHeight,
            framebuffer,
            stencilBits,
            sampleCount,
            contextGeneration
        );
        if (glStateDirty)
        {
            _context!.ResetContext(GRGlBackendState.All);
        }

        string result;
        using (new SKAutoCanvasRestore(_surface!.Canvas, true))
        {
            // The transferred visible framebuffer is wrapped at a stable
            // capacity. Restrict Skia to the exact top-left viewport; the DOM
            // root clips the unused capacity without CSS scaling.
            _surface.Canvas.ClipRect(
                new SKRect(0, 0, physicalWidth, physicalHeight),
                SKClipOperation.Intersect,
                antialias: false
            );
            result = _target.PaintSkiaSurface(
                _viewId,
                _surface,
                physicalWidth,
                physicalHeight,
                target,
                requestId
            );
        }
        // SkiaSceneRenderer flushes the canvas at the end of browser paints.
        // Submit that work once through the owning GPU context; JavaScript then
        // finalizes any newly-hidden band before yielding to the browser.
        _context!.Flush();
        return result;
    }

    [JSExport]
    public static void CompleteFrame(
        [JSMarshalAs<JSType.Number>] long requestId,
        [JSMarshalAs<JSType.Number>] long generation,
        string terminal,
        string reason
    )
    {
        _ = generation;
        _target?.CompleteSkiaSurfacePaint(_viewId, requestId, generation, terminal, reason);
    }

    [JSExport]
    public static void ContextLost(
        [JSMarshalAs<JSType.Number>] long requestId,
        [JSMarshalAs<JSType.Number>] long generation
    )
    {
        _ = generation;
        _target?.InvalidateSkiaGpuContext(_viewId, requestId, "worker WebGL context lost");
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
        _textureModule?.Dispose();
        _textureModule = null;
    }

    private static void RequestPresent()
    {
        if (!_initialized || _target is null)
        {
            return;
        }

        var target = _target.CaptureSnapshot(_viewId).ResizeEpoch;
        BrowserInterop.RequestPresent(
            canvasId: "doroti-surface",
            generation: target.Generation,
            logicalWidth: target.LogicalWidth,
            logicalHeight: target.LogicalHeight,
            physicalWidth: target.PhysicalWidth,
            physicalHeight: target.PhysicalHeight,
            devicePixelRatio: target.DevicePixelRatio,
            timestampMicroseconds: target.TimestampMicroseconds
        );
    }

    private static void EnsureSurface(
        int width,
        int height,
        int framebuffer,
        int stencilBits,
        int sampleCount,
        long contextGeneration
    )
    {
        if (_context is not null && _contextGeneration != contextGeneration)
        {
            ReleaseGpu();
        }

        if (_context is null)
        {
            _glInterface = GRGlInterface.Create();
            _context =
                GRContext.CreateGl(_glInterface)
                ?? throw new InvalidOperationException(
                    "Doroti could not create the worker WebGL Skia context."
                );
            _context.SetResourceCacheLimit(ResourceCacheBytes);
            _contextGeneration = contextGeneration;
        }
        var size = new SKSizeI(width, height);
        sampleCount = Math.Max(0, sampleCount);
        stencilBits = Math.Max(0, stencilBits);
        if (
            _renderTarget is null
            || _surfaceSize != size
            || _framebuffer != framebuffer
            || _sampleCount != sampleCount
            || _stencilBits != stencilBits
            || !_renderTarget.IsValid
        )
        {
            if (_renderTarget is not null)
            {
                _target?.InvalidateSkiaWindowSurface(_viewId);
            }

            _surface?.Dispose();
            _surface = null;
            _renderTarget?.Dispose();
            var glInfo = new GRGlFramebufferInfo(
                (uint)framebuffer,
                SKColorType.Rgba8888.ToGlSizedFormat()
            );
            _renderTarget = new GRBackendRenderTarget(
                width,
                height,
                sampleCount,
                stencilBits,
                glInfo
            );
            _surfaceSize = size;
            _framebuffer = framebuffer;
            _sampleCount = sampleCount;
            _stencilBits = stencilBits;
        }
        _surface ??=
            SKSurface.Create(
                _context,
                _renderTarget,
                GRSurfaceOrigin.BottomLeft,
                SKColorType.Rgba8888
            )
            ?? throw new InvalidOperationException(
                "Doroti could not wrap the worker WebGL framebuffer."
            );
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
        _sampleCount = 0;
        _stencilBits = 0;
        _contextGeneration = 0;
    }

    [DllImport("libSkiaSharp", EntryPoint = "DorotiInterceptBrowserObjects")]
    private static extern void InterceptBrowserObjects();
}
