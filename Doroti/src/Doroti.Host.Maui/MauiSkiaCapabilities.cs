using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using UiImage = Doroti.Ui.Image;

namespace Doroti.Host.Maui;

internal sealed class MauiSkiaCapabilities :
    ISceneHostCapability,
    IParagraphHostCapability,
    IImageHostCapability,
    ISemanticsHostCapability,
    IDisposable
{
    private readonly MauiHostAdapter _host;
    private readonly SkiaSceneRenderer _renderer;

    internal MauiSkiaCapabilities(
        ulong viewId,
        MauiHostAdapter host,
        Doroti.Ui.Color? backgroundColor,
        Doroti.Ui.Color? darkBackgroundColor)
    {
        _host = host;
        _renderer = new(
            viewId,
            new HostBridge(host),
            backgroundColor,
            darkBackgroundColor,
#if MACOS
            "macos/mtkview",
            DorotiSkiaRuntimeEffects.AppKitMetalBackend,
            "AppKit/MTKView/Metal-Skia",
            // AppKit Ganesh offscreen snapshots can expose a transparent
            // picture before its raster work is visible to the main surface.
            enablePictureRasterCache: false);
#elif MACCATALYST
            "maui/skglview",
            DorotiSkiaRuntimeEffects.MauiGpuBackend,
            "skiasharp-maui-skglview-gpu",
            // Catalyst uses the same Ganesh/Metal offscreen path. During a
            // live resize, cached component pictures can become visible before
            // their replacement raster belongs to the new drawable epoch.
            enablePictureRasterCache: false);
#elif IOS
            "maui/skglview",
            DorotiSkiaRuntimeEffects.MauiGpuBackend,
            "skiasharp-maui-skglview-gpu",
            // iOS uses the same Ganesh/Metal offscreen path as Catalyst.
            // Cached component pictures can otherwise expose a transparent
            // snapshot before their raster work is visible to the drawable.
            enablePictureRasterCache: false);
#else
            "maui/skglview",
            DorotiSkiaRuntimeEffects.MauiGpuBackend,
            "skiasharp-maui-skglview-gpu");
#endif
    }

    public event Action<SemanticsActionEvent>? Action
    {
        add => _renderer.Action += value;
        remove => _renderer.Action -= value;
    }

    internal MauiFrameDiagnostics Diagnostics
    {
        get
        {
            var value = _renderer.Diagnostics;
            return new(
                value.Submitted, value.Presented, value.Replayed, value.Failed,
                value.ContextGeneration, value.SurfaceGeneration, value.PendingScene,
                value.ShaderImageFiltersRendered, value.Backend, value.Superseded,
                value.Dropped, value.LastInputSequence, value.LastSubmittedInputSequence,
                value.LastPresentedInputSequence, value.ImageFilterSurfacesCreated,
                value.ImageFilterSurfaceReuses, value.ActiveImageFilterSurfaces,
                value.ShaderImageFilterCacheHits, value.ShaderImageFilterCacheMisses,
                value.PictureRasterCacheHits, value.PictureRasterCacheMisses,
                value.PictureRasterCacheEntries, value.Trace);
        }
    }

    internal void AttachFrameworkTrace(DorotiFrameTrace frameTrace) =>
        _renderer.AttachFrameworkTrace(frameTrace);

    internal void AttachSurface(Action invalidate) => _renderer.AttachSurface(invalidate);

    public void Submit(ulong viewId, DorotiSceneSubmission submission, DartUiInvocation invocation) =>
        _renderer.Submit(viewId, submission, invocation);

    internal MauiPaintCompletion? Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        out bool shouldPresent)
    {
        var result = _renderer.Paint(surface, pixelWidth, pixelHeight, _host.ResizeTarget);
        shouldPresent = result.Disposition != SkiaPaintDisposition.superseded;
        return result.Completion is { } completion
            ? new(completion.InputSequence, completion.SceneSequence,
                completion.SurfaceGeneration, completion.IsNewFrame, completion.Descriptor)
            : null;
    }

    internal void CompletePaint(MauiPaintCompletion completion) =>
        _renderer.CompletePaint(new(
            completion.InputSequence, completion.SceneSequence,
            completion.SurfaceGeneration, completion.IsNewFrame, completion.Descriptor));

    internal void FailPaint(MauiPaintCompletion completion, string reason) =>
        _renderer.FailPaint(new(
            completion.InputSequence, completion.SceneSequence,
            completion.SurfaceGeneration, completion.IsNewFrame, completion.Descriptor), reason);

    public Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation) =>
        _renderer.Layout(request, invocation);

    public ValueTask<UiImage> DecodeAsync(
        ReadOnlyMemory<byte> bytes,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default) =>
        _renderer.DecodeAsync(bytes, invocation, cancellationToken);

    public void SetEnabled(bool enabled, DartUiInvocation invocation) =>
        _renderer.SetEnabled(enabled, invocation);

    public void Update(SemanticsUpdate update, DartUiInvocation invocation) =>
        _renderer.Update(update, invocation);

    public void Dispose() => _renderer.Dispose();

    private sealed class HostBridge(MauiHostAdapter host) : ISkiaSceneRendererHost
    {
        private readonly MauiHostAdapter _host = host;

        public long InputSequence => _host.InputSequence;
        public long SurfaceGeneration => _host.Snapshot.SurfaceGeneration;
        public DorotiViewEpoch ViewEpoch => _host.ViewEpoch;
        public DorotiResizeEpoch ResizeTarget
            => _host.ResizeTarget;
        public PlatformConfiguration Configuration => _host.Configuration;

        public event Action<int, SemanticsAction, object?>? SemanticsAction
        {
            add => _host.SemanticsAction += value;
            remove => _host.SemanticsAction -= value;
        }

        public event Action<long, TimeSpan>? InputReceived
        {
            add => _host.InputReceived += value;
            remove => _host.InputReceived -= value;
        }

        public event Action<PlatformConfiguration>? ConfigurationChanged
        {
            add => _host.ConfigurationChanged += value;
            remove => _host.ConfigurationChanged -= value;
        }

        public void UpdateSemantics(SemanticsUpdate update) => _host.UpdateSemantics(update);
        public void ClearSemantics() => _host.ClearSemantics();
        public void RequestInvalidate() => _host.RequestInvalidate();
    }
}
