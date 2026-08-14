using Doroti.Graphics;

namespace Doroti.Composition;

public sealed class SurfaceDeviceLostException(string message, Exception? innerException = null) :
    Exception(message, innerException);

/// <summary>
/// Reports that a frame captured an older surface generation and must be ACKed as stale rather than
/// treated as a recoverable device failure.
/// </summary>
public sealed class SurfaceStaleFrameException(string message) : Exception(message);

public readonly record struct SurfaceGeneration(ulong Value)
{
    public SurfaceGeneration Next()
    {
        if (Value == ulong.MaxValue)
        {
            throw new InvalidOperationException("Surface generation overflowed.");
        }

        return new(Value + 1);
    }
}

public interface IRenderSurface : IDisposable
{
    SurfaceGeneration Generation { get; }

    Size PixelSize { get; }

    ISurfaceFrame BeginFrame();
}

public interface ISurfaceFrame : IDisposable
{
    SurfaceGeneration Generation { get; }

    Size PixelSize { get; }

    IRasterCanvas Canvas { get; }

    void Clear(Color color);

    void Present();
}

/// <summary>
/// Optional window-surface capability that binds a physical frame to the metrics generation
/// captured at BeginFrame. Compositors use it to reject resize/DPI races before raster or present.
/// </summary>
public interface IMetricsBoundSurfaceFrame
{
    long MetricsGeneration { get; }
}

/// <summary>
/// Optional backend-neutral frame capability for copying the current pixels in top-down premultiplied BGRA8888 order.
/// </summary>
public interface IPixelReadableSurfaceFrame
{
    bool TryReadPixels(Span<byte> destination, int rowBytes);
}

public enum RasterPaintStyle
{
    Fill,
    Stroke,
}

public enum RasterBlendMode
{
    Clear,
    Source,
    Destination,
    SourceOver,
    DestinationOver,
    SourceIn,
    DestinationIn,
    SourceOut,
    DestinationOut,
    SourceAtop,
    DestinationAtop,
    Xor,
    Plus,
    Modulate,
    Screen,
    Overlay,
    Darken,
    Lighten,
    ColorDodge,
    ColorBurn,
    HardLight,
    SoftLight,
    Difference,
    Exclusion,
    Multiply,
    Hue,
    Saturation,
    Color,
    Luminosity,
}

public enum RasterTileMode
{
    Clamp,
    Repeat,
    Mirror,
    Decal,
}

public enum RasterImageFilterKind
{
    Blur,
    Matrix,
    Compose,
    ColorFilter,
}

public enum RasterColorFilterKind
{
    Mode,
    Matrix,
    LinearToSrgbGamma,
    SrgbToLinearGamma,
}

public sealed record RasterColorFilter(
    RasterColorFilterKind Kind,
    Color? Color = null,
    RasterBlendMode BlendMode = RasterBlendMode.SourceOver,
    IReadOnlyList<double>? Matrix = null)
{
    public RasterColorFilter Validate()
    {
        if (Kind == RasterColorFilterKind.Mode && Color is null)
            throw new ArgumentException("A mode color filter requires a color.");
        if (Kind == RasterColorFilterKind.Matrix &&
            (Matrix is not { Count: 20 } || Matrix.Any(value => !double.IsFinite(value))))
            throw new ArgumentException("A color matrix must contain 20 finite values.");
        return this;
    }
}

public sealed record RasterImageFilter(
    RasterImageFilterKind Kind,
    double SigmaX = 0,
    double SigmaY = 0,
    RasterTileMode TileMode = RasterTileMode.Clamp,
    IReadOnlyList<double>? Matrix = null,
    RasterImageFilter? Outer = null,
    RasterImageFilter? Inner = null,
    RasterColorFilter? ColorFilter = null)
{
    public RasterImageFilter Validate()
    {
        switch (Kind)
        {
            case RasterImageFilterKind.Blur when
                !double.IsFinite(SigmaX) || !double.IsFinite(SigmaY) || SigmaX < 0 || SigmaY < 0:
                throw new ArgumentOutOfRangeException(nameof(SigmaX), "Blur sigma must be finite and non-negative.");
            case RasterImageFilterKind.Matrix when
                Matrix is not { Count: 16 } || Matrix.Any(value => !double.IsFinite(value)):
                throw new ArgumentException("An image-filter matrix must contain 16 finite values.");
            case RasterImageFilterKind.Compose when Outer is null || Inner is null:
                throw new ArgumentException("A composed image filter requires outer and inner filters.");
            case RasterImageFilterKind.ColorFilter when ColorFilter is null:
                throw new ArgumentException("A color image filter requires a color filter.");
        }
        Outer?.Validate();
        Inner?.Validate();
        ColorFilter?.Validate();
        return this;
    }
}

public readonly record struct RasterLayerOptions(
    Rect? Bounds = null,
    double Opacity = 1,
    RasterBlendMode BlendMode = RasterBlendMode.SourceOver,
    RasterImageFilter? ImageFilter = null,
    RasterImageFilter? BackdropFilter = null,
    RasterColorFilter? ColorFilter = null)
{
    public RasterLayerOptions Validate()
    {
        if (Bounds is { IsFinite: false })
            throw new ArgumentException("Layer bounds must be finite.", nameof(Bounds));
        if (!double.IsFinite(Opacity) || Opacity is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(Opacity));
        ImageFilter?.Validate();
        BackdropFilter?.Validate();
        ColorFilter?.Validate();
        return this;
    }
}

public readonly record struct RasterPaint(
    Color Color,
    double Opacity = 1,
    double BlurSigma = 0,
    RasterPaintStyle Style = RasterPaintStyle.Fill,
    double StrokeWidth = 0,
    RasterBlendMode BlendMode = RasterBlendMode.SourceOver,
    bool IsAntiAlias = true,
    RasterColorFilter? ColorFilter = null)
{
    public RasterPaint Validate()
    {
        if (!double.IsFinite(Opacity) || Opacity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(Opacity), "Opacity must be finite and between zero and one.");
        }
        if (!double.IsFinite(BlurSigma) || BlurSigma < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(BlurSigma), "Blur sigma must be finite and non-negative.");
        }
        if (!double.IsFinite(StrokeWidth) || StrokeWidth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(StrokeWidth), "Stroke width must be finite and non-negative.");
        }
        ColorFilter?.Validate();
        return this;
    }
}

/// <summary>Low-level backend-neutral canvas owned by one surface frame.</summary>
public interface IRasterCanvas
{
    int SaveCount { get; }

    void Save();

    /// <summary>
    /// Begins a bounded offscreen group. Restore filters and composites the group exactly once.
    /// A backdrop filter samples the destination as it existed before this call.
    /// </summary>
    void SaveLayer(RasterLayerOptions options);

    void Restore();

    void Transform(Matrix transform);

    void ClipRect(Rect rect);

    void ClipPath(PathGeometry path);

    void MultiplyOpacity(double opacity);

    void DrawColor(Color color);

    void DrawRect(Rect rect, RasterPaint paint);

    void DrawPath(PathGeometry path, RasterPaint paint);

    void DrawImage(ImageResourceSnapshot image, Rect source, Rect destination, double opacity = 1);

    void DrawText(string text, Offset origin, double fontSize, RasterPaint paint, string? fontFamily = null);
}
