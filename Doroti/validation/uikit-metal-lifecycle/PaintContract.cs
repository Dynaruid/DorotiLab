using SkiaSharp;

namespace Doroti.Host.Maui;

// Only the paint event payload is substituted. The native view, virtual view,
// Metal submission, Graphite session and completion handlers are product source.
internal readonly record struct MauiPaintCompletion(long Sequence);
internal sealed class MauiSkiaPaintContext(SKSurface surface, object? contextIdentity,
    int pixelWidth, int pixelHeight, double density, long surfaceGeneration,
    string nativeViewType, string graphicsBackend)
{
    internal SKSurface Surface { get; } = surface;
    internal object? ContextIdentity { get; } = contextIdentity;
    internal int PixelWidth { get; } = pixelWidth;
    internal int PixelHeight { get; } = pixelHeight;
    internal double Density { get; } = density;
    internal long SurfaceGeneration { get; } = surfaceGeneration;
    internal string NativeViewType { get; } = nativeViewType;
    internal string GraphicsBackend { get; } = graphicsBackend;
    internal bool SkipPresent { get; set; }
    internal MauiPaintCompletion? Completion { get; set; }
}
