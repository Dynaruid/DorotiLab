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
    long CausalFrameId = 0
);

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
    string Reason
)
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
    DorotiFrameMatchResult? MatchResult = null
)
{
    public bool ShouldPresent =>
        Disposition is SkiaPaintDisposition.exact or SkiaPaintDisposition.replay;
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
    DorotiFrameTerminalLedgerSnapshot TerminalLedger,
    SkiaWorkDiagnostics? Work = null
);

public sealed record SkiaWorkDiagnostics(
    long PromotionCount,
    long PromotionMicroseconds,
    long PromotionMaximumMicroseconds,
    long ParagraphCount,
    long ParagraphMicroseconds,
    int WarmupEntries,
    long RasterPixels,
    int TextEntries,
    long CommandCacheHits = 0,
    long CommandRecordings = 0,
    int CommandEntries = 0,
    int RetainedCommands = 0,
    long CommandBytes = 0,
    long TextCacheHits = 0,
    long TextCacheMisses = 0,
    long TextCacheEvictions = 0,
    long FontGeneration = 0,
    int TextEntryLimit = 256,
    long RasterPixelLimit = 16L * 1024 * 1024
);

/// <summary>Cache ownership accounting; byte estimates are not GPU/process resident memory.</summary>
public sealed record SkiaCacheMemoryDiagnostics(
    int TextEntries,
    int TextEntryLimit,
    int TextFontResources,
    long TextBytesEstimate,
    long TextHits,
    long TextMisses,
    long TextEvictions,
    long FontGeneration,
    int RasterEntries,
    long RasterRgba8Bytes,
    long RasterRgba8Limit
);
