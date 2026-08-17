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
    string Backend);

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
    long SoftwareFallbackFrames);

public interface IMauiSemanticsBridge
{
    void Update(string serializedTree, Action<int, SemanticsAction, object?> performAction);
}

internal sealed class NullMauiSemanticsBridge : IMauiSemanticsBridge
{
    public void Update(string serializedTree, Action<int, SemanticsAction, object?> performAction)
    {
        _ = serializedTree;
        _ = performAction;
    }
}
