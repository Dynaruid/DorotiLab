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

public readonly record struct RasterPaint(
    Color Color,
    double Opacity = 1,
    double BlurSigma = 0,
    RasterPaintStyle Style = RasterPaintStyle.Fill,
    double StrokeWidth = 0)
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
        return this;
    }
}

/// <summary>Low-level backend-neutral canvas owned by one surface frame.</summary>
public interface IRasterCanvas
{
    int SaveCount { get; }

    void Save();

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
