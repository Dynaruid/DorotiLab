using Doroti.Ui;

namespace Doroti.Skia.Rendering;

public interface ISkiaSceneRendererHost
{
    long InputSequence { get; }
    long SurfaceGeneration { get; }
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
    bool IsNewFrame);

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
    IReadOnlyList<DorotiFrameTraceEntry> Trace);
