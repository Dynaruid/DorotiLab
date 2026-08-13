// Adapted from Avalonia.Skia FramebufferRenderTarget; see migration/vendor/avalonia-platform/provenance.json.
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace Doroti.Vendor.Avalonia.Skia;

internal delegate void NativeFramebufferPresenter(ReadOnlySpan<byte> pixels, int width, int height, int rowBytes);

internal sealed class NativeFramebufferRenderTarget : IDisposable
{
    private readonly NativeFramebufferPresenter _presenter;
    private SKBitmap? _bitmap;
    private SurfaceGenerationState _state;
    private bool _disposed;

    internal NativeFramebufferRenderTarget(int width, int height, NativeFramebufferPresenter presenter)
    {
        _presenter = presenter;
        _state = new(1, width, height);
        _bitmap = AllocateBitmap(width, height);
    }

    internal NativeSurfaceDescriptor Descriptor => new(_state.Generation, _state.Width, _state.Height);

    internal NativeFramebufferFrame BeginFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return new(this, _state, _bitmap!);
    }

    internal void Recreate(int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ValidateSize(width, height);
        if (_state.Width == width && _state.Height == height)
        {
            return;
        }
        if (_state.Generation == ulong.MaxValue)
        {
            throw new InvalidOperationException("Surface generation overflowed.");
        }

        var replacement = AllocateBitmap(width, height);
        _bitmap!.Dispose();
        _bitmap = replacement;
        _state = new(_state.Generation + 1, width, height);
    }

    internal void Present(SurfaceGenerationState frameState, SKBitmap bitmap)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (frameState != _state || !ReferenceEquals(bitmap, _bitmap))
        {
            throw new InvalidOperationException("A stale framebuffer cannot be presented to a recreated surface.");
        }

        _presenter(bitmap.GetPixelSpan(), bitmap.Width, bitmap.Height, bitmap.RowBytes);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _bitmap?.Dispose();
        _bitmap = null;
    }

    private static SKBitmap AllocateBitmap(int width, int height)
    {
        ValidateSize(width, height);
        return new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
    }

    private static void ValidateSize(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
    }

    internal readonly record struct SurfaceGenerationState(ulong Generation, int Width, int Height);
}

internal interface INativeRasterFrame : IDisposable
{
    NativeSurfaceDescriptor Descriptor { get; }

    int SaveCount { get; }

    void Clear(uint argb);

    void Save();

    void Restore();

    void Transform(double[] values);

    void ClipRect(double left, double top, double right, double bottom);

    void ClipPath(double[] coordinates, bool closed, bool evenOdd);

    void DrawRect(double left, double top, double right, double bottom, uint argb, double opacity);

    void DrawPath(double[] coordinates, bool closed, bool evenOdd, uint argb, double opacity, double blurSigma = 0, bool stroke = false, double strokeWidth = 0);

    void DrawImage(
        ReadOnlySpan<byte> pixels,
        int width,
        int height,
        double sourceLeft,
        double sourceTop,
        double sourceRight,
        double sourceBottom,
        double destinationLeft,
        double destinationTop,
        double destinationRight,
        double destinationBottom,
        double opacity);

    void DrawText(string text, double x, double y, double fontSize, uint argb, double opacity, string? fontFamily = null);

    bool TryReadPixels(Span<byte> destination, int rowBytes);

    void Present();
}

internal sealed class NativeFramebufferFrame : INativeRasterFrame
{
    private readonly NativeFramebufferRenderTarget _owner;
    private readonly NativeFramebufferRenderTarget.SurfaceGenerationState _state;
    private readonly SKBitmap _bitmap;
    private readonly SKCanvas _canvas;
    private bool _disposed;
    private bool _presented;

    internal NativeFramebufferFrame(
        NativeFramebufferRenderTarget owner,
        NativeFramebufferRenderTarget.SurfaceGenerationState state,
        SKBitmap bitmap)
    {
        _owner = owner;
        _state = state;
        _bitmap = bitmap;
        _canvas = new SKCanvas(bitmap);
    }

    public NativeSurfaceDescriptor Descriptor => new(_state.Generation, _state.Width, _state.Height);

    public int SaveCount => _canvas.SaveCount;

    public void Clear(uint argb)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _canvas.Clear(new SKColor(
            red: (byte)(argb >> 16),
            green: (byte)(argb >> 8),
            blue: (byte)argb,
            alpha: (byte)(argb >> 24)));
    }

    public void Save()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _canvas.Save();
    }

    public void Restore()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _canvas.Restore();
    }

    public void Transform(double[] values)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (values.Length != 16)
        {
            throw new ArgumentException("Expected a 4x4 transform.", nameof(values));
        }
        var matrix = new SKMatrix(
            (float)values[0], (float)values[1], (float)values[3],
            (float)values[4], (float)values[5], (float)values[7],
            (float)values[12], (float)values[13], (float)values[15]);
        _canvas.Concat(matrix);
    }

    public void ClipRect(double left, double top, double right, double bottom) =>
        _canvas.ClipRect(new((float)left, (float)top, (float)right, (float)bottom));

    public void ClipPath(double[] coordinates, bool closed, bool evenOdd)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var path = CreatePath(coordinates, closed, evenOdd);
        _canvas.ClipPath(path, antialias: true);
    }

    public void DrawRect(double left, double top, double right, double bottom, uint argb, double opacity)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var paint = CreatePaint(argb, opacity);
        _canvas.DrawRect(new((float)left, (float)top, (float)right, (float)bottom), paint);
    }

    public void DrawPath(double[] coordinates, bool closed, bool evenOdd, uint argb, double opacity, double blurSigma = 0, bool stroke = false, double strokeWidth = 0)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var path = CreatePath(coordinates, closed, evenOdd);
        using var paint = CreatePaint(argb, opacity);
        paint.Style = stroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill;
        paint.StrokeWidth = (float)strokeWidth;
        if (blurSigma > 0)
        {
            paint.ImageFilter = SKImageFilter.CreateBlur((float)blurSigma, (float)blurSigma);
        }
        _canvas.DrawPath(path, paint);
    }

    public void DrawImage(
        ReadOnlySpan<byte> pixels,
        int width,
        int height,
        double sourceLeft,
        double sourceTop,
        double sourceRight,
        double sourceBottom,
        double destinationLeft,
        double destinationTop,
        double destinationRight,
        double destinationBottom,
        double opacity)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
        pixels.CopyTo(bitmap.GetPixelSpan());
        using var paint = new SKPaint { Color = new SKColor(255, 255, 255, OpacityByte(opacity)), IsAntialias = false };
        var source = new SKRect((float)sourceLeft, (float)sourceTop, (float)sourceRight, (float)sourceBottom);
        var destination = new SKRect(
            (float)destinationLeft,
            (float)destinationTop,
            (float)destinationRight,
            (float)destinationBottom);
        _canvas.DrawBitmap(bitmap, source, destination, paint);
    }

    public void DrawText(string text, double x, double y, double fontSize, uint argb, double opacity, string? fontFamily = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var paint = CreatePaint(argb, opacity);
        FontFallbackTextRenderer.Draw(_canvas, text, (float)x, (float)y, (float)fontSize, paint, fontFamily);
    }

    public bool TryReadPixels(Span<byte> destination, int rowBytes)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var requiredRowBytes = checked(_state.Width * 4);
        if (rowBytes < requiredRowBytes || destination.Length < checked(rowBytes * _state.Height))
        {
            throw new ArgumentException("The pixel destination is smaller than the BGRA8888 frame.", nameof(destination));
        }
        var source = _bitmap.GetPixelSpan();
        for (var row = 0; row < _state.Height; row++)
        {
            source.Slice(row * _bitmap.RowBytes, requiredRowBytes)
                .CopyTo(destination.Slice(row * rowBytes, requiredRowBytes));
        }
        return true;
    }

    public void Present()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_presented)
        {
            throw new InvalidOperationException("A framebuffer can only be presented once.");
        }
        _canvas.Flush();
        _owner.Present(_state, _bitmap);
        _presented = true;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _canvas.Dispose();
    }

    private static SKPaint CreatePaint(uint argb, double opacity) => new()
    {
        Color = new SKColor(
            (byte)(argb >> 16),
            (byte)(argb >> 8),
            (byte)argb,
            (byte)Math.Round((argb >> 24) * opacity)),
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
    };

    private static SKPath CreatePath(double[] coordinates, bool closed, bool evenOdd)
    {
        if (coordinates.Length < 4 || coordinates.Length % 2 != 0)
            throw new ArgumentException("Path coordinates must contain x/y pairs.", nameof(coordinates));
        var path = new SKPath { FillType = evenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding };
        path.MoveTo((float)coordinates[0], (float)coordinates[1]);
        for (var index = 2; index < coordinates.Length; index += 2)
            path.LineTo((float)coordinates[index], (float)coordinates[index + 1]);
        if (closed) path.Close();
        return path;
    }

    private static byte OpacityByte(double opacity) => (byte)Math.Round(255 * opacity);
}
