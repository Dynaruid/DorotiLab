using Doroti.Ui;

namespace Doroti.Host.Maui;

public sealed record MauiSurfaceSnapshot(
    int PixelWidth,
    int PixelHeight,
    double DevicePixelRatio,
    long MetricsGeneration,
    long ContextGeneration,
    long SurfaceGeneration,
    string NativeViewType,
    string GraphicsBackend);

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
    long UpdatesSuppressed = 0);

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
    MauiSemanticsDiagnostics Semantics,
    long SoftwareFallbackFrames);

public interface IMauiSemanticsBridge
{
    MauiSemanticsDiagnostics Diagnostics { get; }

    void Update(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction);

    void Clear();
}

internal sealed class NullMauiSemanticsBridge : IMauiSemanticsBridge
{
    public MauiSemanticsDiagnostics Diagnostics { get; } = new(0, 0, 0, 0, 0, 0);

    public void Update(SemanticsUpdate update, Action<int, SemanticsAction, object?> performAction)
    {
        _ = update;
        _ = performAction;
    }

    public void Clear() { }
}
