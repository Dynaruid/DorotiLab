using System.Runtime.InteropServices.JavaScript;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Web;

public static partial class DorotiWebWorkerSurface
{
    private static SKGraphiteContext? _graphiteContext;
    private static SKGraphiteRecorder? _graphiteRecorder;
    private static int _graphiteOwner;
    private static long _graphiteImageRequests;
    private static SKGraphiteImageCache? _graphiteImages;
    private static JSObject? _graphiteInteropModule;

    [JSExport]
    public static async Task InitializeGraphite(string moduleUrl)
    {
        if (_graphiteContext is not null) throw new InvalidOperationException("Graphite already initialized.");
        _graphiteOwner = Environment.CurrentManagedThreadId;
        _graphiteInteropModule = await JSHost.ImportAsync("doroti-webgpu", moduleUrl);
        if (!SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Dawn))
            throw new PlatformNotSupportedException("Skia native Dawn backend is unavailable.");
        using var module = JSHost.DotnetInstance.GetPropertyAsJSObject("Module")
            ?? throw new InvalidOperationException("Graphite render Worker has no native Module.");
        using var handles = GraphiteAttachNative(module);
        _graphiteContext = SKGraphiteContext.CreateDawn(new SKGraphiteDawnBackendContext {
            WgpuInstance = handles.GetPropertyAsInt32("instance"),
            WgpuDevice = handles.GetPropertyAsInt32("device"),
            WgpuQueue = handles.GetPropertyAsInt32("queue"),
        }) ?? throw new InvalidOperationException("Skia Dawn context creation failed.");
        _graphiteContext.MaxBudgetedBytes = ResourceCacheBytes;
        _graphiteImages = new SKGraphiteImageCache();
        _graphiteRecorder = _graphiteContext.CreateRecorder(ResourceCacheBytes, (recorder, image, mipmapped) => {
            _graphiteImageRequests++;
            return _graphiteImages.FindOrCreate(recorder, image, mipmapped);
        }) ?? throw new InvalidOperationException("Skia Dawn recorder creation failed.");
    }

    [JSExport]
    public static string RenderGraphiteFrame(
        [JSMarshalAs<JSType.Number>] long requestId,
        [JSMarshalAs<JSType.Number>] long generation,
        double logicalWidth, double logicalHeight, int width, int height, double dpr,
        [JSMarshalAs<JSType.Number>] long timestamp)
    {
        if (!_initialized || _target is null) return "superseded";
        if (_graphiteOwner != Environment.CurrentManagedThreadId || _graphiteContext is null || _graphiteRecorder is null)
            throw new InvalidOperationException("Graphite frame is not on its recorder owner.");
        if (_target.CaptureSnapshot(_viewId).ResizeEpoch.Generation != generation) return "superseded";
        if (_graphiteContext.IsDeviceLost) throw new InvalidOperationException("Graphite device lost.");
        _graphiteContext.CheckAsyncWorkCompletion();
        var texture = GraphiteAcquire(width, height);
        try
        {
            using var backend = SKGraphiteBackendTexture.CreateDawn(texture)
                ?? throw new InvalidOperationException("Graphite current texture could not be wrapped.");
            using var surface = SkiaGpuSurfaces.Register(
                SKSurface.Create(_graphiteRecorder, backend, SKColorType.Bgra8888)
                ?? throw new InvalidOperationException("Graphite current texture surface failed."), _graphiteRecorder);
            var epoch = new DorotiResizeEpoch(generation, logicalWidth, logicalHeight, width, height, dpr, timestamp);
            // The current texture has grow-only capacity, while layout and
            // raster scale use the exact viewport. Clear unused pixels as
            // WebGPU current textures do not retain the previous frame.
            surface.Canvas.Clear(SKColors.Transparent);
            string result;
            using (new SKAutoCanvasRestore(surface.Canvas, true))
            {
                surface.Canvas.ClipRect(new SKRect(0, 0, width, height), SKClipOperation.Intersect, antialias: false);
                result = _target.PaintSkiaSurface(_viewId, surface, width, height, epoch, requestId);
            }
            using var recording = _graphiteRecorder.Snap()
                ?? throw new InvalidOperationException("Graphite produced no recording.");
            if (result is "exact-rendered" or "replay-rendered")
            {
                if (_graphiteContext.InsertRecording(recording) != SKGraphiteInsertStatus.Success)
                    throw new InvalidOperationException("Graphite recording was rejected.");
                if (!_graphiteContext.Submit(new SKGraphiteSubmitInfo { Sync = false }))
                    throw new InvalidOperationException("Graphite submission failed.");
            }
            return result;
        }
        finally { GraphiteReleaseTexture(texture); }
    }

    [JSExport]
    public static async Task DisposeGraphite()
    {
        if (_graphiteOwner == 0) return; // Device initialization can fail before native attachment.
        if (_graphiteOwner != Environment.CurrentManagedThreadId)
            throw new InvalidOperationException("Graphite disposal left its owner.");
        _graphiteContext?.CheckAsyncWorkCompletion();
        // Completing submissions returns staging buffers to the recorder cache
        // and can start new asynchronous maps. Let their native callbacks finish
        // before any cache owner is destroyed.
        await GraphiteDrainForShutdown();
        _graphiteImages?.Dispose();
        _graphiteImages = null;
        await GraphiteDrainForShutdown();
        _graphiteRecorder?.Dispose();
        _graphiteRecorder = null;
        await GraphiteDrainForShutdown();
        _graphiteContext?.CheckAsyncWorkCompletion();
        await GraphiteDrainForShutdown();
        _graphiteContext?.FreeGpuResources();
        await GraphiteDrainForShutdown();
        _graphiteContext?.Dispose();
        _graphiteContext = null;
        await GraphiteDrainForShutdown();
        _graphiteInteropModule?.Dispose();
        _graphiteInteropModule = null;
        _graphiteOwner = 0;
    }

    [JSImport("attachNative", "doroti-webgpu")]
    private static partial JSObject GraphiteAttachNative(JSObject module);
    [JSImport("acquire", "doroti-webgpu")]
    private static partial int GraphiteAcquire(int width, int height);
    [JSImport("releaseTexture", "doroti-webgpu")]
    private static partial void GraphiteReleaseTexture(int handle);
    [JSImport("drainForShutdown", "doroti-webgpu")]
    private static partial Task GraphiteDrainForShutdown();
}
