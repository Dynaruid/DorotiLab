using Doroti.Ui;

namespace Doroti.Skia.Rendering;

public interface ISkiaSceneRendererHost
{
    long InputSequence { get; }
    long SurfaceGeneration { get; }
    DorotiViewEpoch ViewEpoch { get; }
    DorotiResizeEpoch ResizeTarget { get; }
    PlatformConfiguration Configuration { get; }
    event Action<int, SemanticsAction, object?>? SemanticsAction;
    event Action<long, TimeSpan>? InputReceived;
    event Action<PlatformConfiguration>? ConfigurationChanged;
    void UpdateSemantics(SemanticsUpdate update);
    void ClearSemantics();
    void RequestInvalidate();
}

public readonly record struct SkiaPaintCompletion(
    long InputSequence,
    long SceneSequence,
    long SurfaceGeneration,
    bool IsNewFrame,
    DorotiFrameDescriptor Descriptor,
    long CausalFrameId = 0);

/// <summary>
/// Immutable receipt emitted only after a Skia paint completion has crossed
/// the native submission boundary.  Hosts can join their callback, raster,
/// swap, and present timestamps with <see cref="CausalFrameId"/> without
/// relabelling a scene descriptor.
/// </summary>
public readonly record struct SkiaFrameReceipt(
    long CausalFrameId,
    long InputSequence,
    long SceneSequence,
    long SurfaceGeneration,
    DorotiFrameDescriptor Descriptor,
    DorotiFrameTerminal Terminal,
    TimeSpan Timestamp,
    bool IsNewFrame,
    SkiaPaintDisposition Disposition,
    string Reason)
{
    public bool HasCausalFrameId => CausalFrameId > 0;
}

public enum SkiaPaintDisposition
{
    empty,
    exact,
    replay,
    superseded,
}

public readonly record struct SkiaPaintResult(
    SkiaPaintDisposition Disposition,
    SkiaPaintCompletion? Completion,
    DorotiFrameDescriptor? Descriptor,
    DorotiFrameMatchResult? MatchResult = null)
{
    public bool ShouldPresent => Disposition is SkiaPaintDisposition.exact or SkiaPaintDisposition.replay;
}

public sealed record SkiaFrameDiagnostics(
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
    IReadOnlyList<DorotiFrameTraceEntry> Trace,
    long SceneAccepted,
    long CausalPaintAttempts,
    DorotiFrameTerminalLedgerSnapshot TerminalLedger);
