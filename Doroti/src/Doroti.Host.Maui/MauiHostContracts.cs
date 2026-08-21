using Doroti.Ui;

namespace Doroti.Host.Maui;

internal readonly record struct MauiPaintCompletion(
    long InputSequence,
    long SceneSequence,
    long SurfaceGeneration,
    bool IsNewFrame);

public sealed record MauiSurfaceSnapshot(
    int PixelWidth,
    int PixelHeight,
    double DevicePixelRatio,
    long MetricsGeneration,
    long ContextGeneration,
    long SurfaceGeneration,
    string NativeViewType,
    string GraphicsBackend,
    string? MetalDevice = null,
    string? PixelFormat = null,
    long CommandBuffersCommitted = 0,
    long CommandBuffersCompleted = 0,
    long CommandBuffersErrored = 0,
    long StaleCompletions = 0,
    long CpuReadbacks = 0,
    long FullFrameCopies = 0,
    double LogicalWidth = 0,
    double LogicalHeight = 0,
    long ResizeContinuityActivations = 0,
    long ResizeContinuityDeactivations = 0,
    bool ResizeContinuityActive = false,
    long ResizeSynchronousPresents = 0,
    long ResizeSynchronousMisses = 0);

public sealed record MauiFrameDiagnostics(
    long Submitted,
    long Presented,
    long Replayed,
    long Failed,
    long ContextGeneration,
    long SurfaceGeneration,
    bool PendingScene,
    long ShaderImageFiltersRendered,
    string Backend,
    long Superseded,
    long Dropped,
    long LastInputSequence,
    long LastSubmittedInputSequence,
    long LastPresentedInputSequence,
    long ImageFilterSurfacesCreated,
    long ImageFilterSurfaceReuses,
    long ActiveImageFilterSurfaces,
    long ShaderImageFilterCacheHits,
    long ShaderImageFilterCacheMisses,
    long PictureRasterCacheHits,
    long PictureRasterCacheMisses,
    long PictureRasterCacheEntries,
    IReadOnlyList<DorotiFrameTraceEntry> Trace);

public sealed record MauiSemanticsDiagnostics(
    long UpdatesReceived,
    long UpdatesApplied,
    long UpdatesCoalesced,
    long ElementsCreated,
    long ActiveElements,
    long RetainedNodes,
    long NativePropertyWrites = 0,
    long ImmediateFlushes = 0,
    long StaleCallbacksSuppressed = 0,
    long UpdatesSuppressed = 0,
    long ElementsReused = 0,
    long TopologyUpdatesApplied = 0,
    long ApplyWorkMicroseconds = 0,
    long MaxApplyWorkMicroseconds = 0);

public sealed record MauiHostDiagnostics(
    string ApplicationSource,
    string BootstrapSource,
    string TargetFramework,
    string Rid,
    string MauiVersion,
    string SkiaSharpVersion,
    MauiSurfaceSnapshot Surface,
    MauiFrameDiagnostics Frame,
    long InvalidationsRequested,
    long InvalidationsCoalesced,
    long NativePointerEvents,
    long FrameRequestsCoalesced,
    MauiSemanticsDiagnostics Semantics,
    long SoftwareFallbackFrames);

public interface IMauiSemanticsBridge
{
    MauiSemanticsDiagnostics Diagnostics { get; }

    void AttachFrameTrace(DorotiFrameTrace trace, ulong viewId);

    void Update(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction);

    void Clear();
}

internal sealed class NullMauiSemanticsBridge : IMauiSemanticsBridge
{
    public MauiSemanticsDiagnostics Diagnostics { get; } = new(0, 0, 0, 0, 0, 0);

    public void AttachFrameTrace(DorotiFrameTrace trace, ulong viewId)
    {
        _ = trace;
        _ = viewId;
    }

    public void Update(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction)
    {
        _ = update;
        _ = performAction;
    }

    public void Clear() { }
}
