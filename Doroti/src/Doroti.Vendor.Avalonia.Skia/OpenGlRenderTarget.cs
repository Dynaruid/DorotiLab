// Adapted from Avalonia.Skia GlSkiaGpu and GlRenderTarget; see migration/vendor/avalonia-platform/provenance.json.
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace Doroti.Vendor.Avalonia.Skia;

internal sealed class NativeOpenGlRenderTarget : IDisposable
{
    private const uint GlRgba8 = 0x8058;
    private readonly Func<IDisposable> _makeCurrent;
    private readonly Action _present;
    private readonly GRContext _context;
    private SurfaceGenerationState _state;
    private bool _disposed;

    internal NativeOpenGlRenderTarget(int width, int height, Func<IDisposable> makeCurrent, Action present)
    {
        ValidateSize(width, height);
        _makeCurrent = makeCurrent;
        _present = present;
        _state = new(1, width, height);
        using var current = _makeCurrent();
        using var glInterface = GRGlInterface.Create();
        _context = GRContext.CreateGl(glInterface) ??
            throw new InvalidOperationException("Skia could not create a GPU context from the current WGL context.");
    }

    internal NativeSurfaceDescriptor Descriptor => new(_state.Generation, _state.Width, _state.Height);

    internal INativeRasterFrame BeginFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var current = _makeCurrent();
        try
        {
            if (_context.IsAbandoned)
            {
                throw new NativeGpuContextLostException("The Skia OpenGL context was abandoned.");
            }
            _context.ResetContext();
            var info = new GRGlFramebufferInfo(0, GlRgba8);
            var renderTarget = new GRBackendRenderTarget(_state.Width, _state.Height, 0, 8, info);
            var surface = SKSurface.Create(
                _context,
                renderTarget,
                GRSurfaceOrigin.BottomLeft,
                SKColorType.Rgba8888) ??
                throw new InvalidOperationException("Skia could not wrap the window's OpenGL framebuffer.");
            return new NativeOpenGlFrame(this, _state, current, renderTarget, surface);
        }
        catch
        {
            current.Dispose();
            throw;
        }
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
        _state = new(_state.Generation + 1, width, height);
    }

    internal void Present(SurfaceGenerationState frameState)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (frameState != _state)
        {
            throw new InvalidOperationException("A stale OpenGL framebuffer cannot be presented.");
        }
        if (_context.IsAbandoned)
        {
            throw new NativeGpuContextLostException("The Skia OpenGL context was abandoned before present.");
        }
        _context.Flush(true, false);
        _present();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        IDisposable? current = null;
        try
        {
            current = _makeCurrent();
            _context.AbandonContext(false);
            _context.Dispose();
        }
        catch
        {
            _context.AbandonContext();
            _context.Dispose();
        }
        finally
        {
            current?.Dispose();
        }
    }

    private static void ValidateSize(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
    }

    internal readonly record struct SurfaceGenerationState(ulong Generation, int Width, int Height);
}

internal sealed class NativeGpuContextLostException(string message, Exception? innerException = null) :
    Exception(message, innerException);

internal sealed class NativeOpenGlFrame : INativeRasterFrame
{
    private readonly NativeOpenGlRenderTarget _owner;
    private readonly NativeOpenGlRenderTarget.SurfaceGenerationState _state;
    private readonly IDisposable _current;
    private readonly GRBackendRenderTarget _renderTarget;
    private readonly SKSurface _surface;
    private readonly SKCanvas _canvas;
    private bool _disposed;
    private bool _presented;

    internal NativeOpenGlFrame(
        NativeOpenGlRenderTarget owner,
        NativeOpenGlRenderTarget.SurfaceGenerationState state,
        IDisposable current,
        GRBackendRenderTarget renderTarget,
        SKSurface surface)
    {
        _owner = owner;
        _state = state;
        _current = current;
        _renderTarget = renderTarget;
        _surface = surface;
        _canvas = surface.Canvas;
    }

    public NativeSurfaceDescriptor Descriptor => new(_state.Generation, _state.Width, _state.Height);

    public int SaveCount => _canvas.SaveCount;

    public void Clear(uint argb)
    {
        ThrowIfDisposed();
        _canvas.Clear(ToColor(argb, 1));
    }

    public void Save()
    {
        ThrowIfDisposed();
        _canvas.Save();
    }

    public void SaveLayer(NativeLayerOptions options)
    {
        ThrowIfDisposed();
        NativeLayerSupport.SaveLayer(_canvas, options);
    }

    public void Restore()
    {
        ThrowIfDisposed();
        _canvas.Restore();
    }

    public void Transform(double[] values)
    {
        ThrowIfDisposed();
        if (values.Length != 16)
        {
            throw new ArgumentException("Expected a 4x4 transform.", nameof(values));
        }
        _canvas.Concat(new SKMatrix(
            (float)values[0], (float)values[1], (float)values[3],
            (float)values[4], (float)values[5], (float)values[7],
            (float)values[12], (float)values[13], (float)values[15]));
    }

    public void ClipRect(double left, double top, double right, double bottom) =>
        _canvas.ClipRect(new((float)left, (float)top, (float)right, (float)bottom));

    public void ClipPath(double[] coordinates, bool closed, bool evenOdd)
    {
        ThrowIfDisposed();
        using var path = CreatePath(coordinates, closed, evenOdd);
        _canvas.ClipPath(path, antialias: true);
    }

    public void DrawRect(double left, double top, double right, double bottom, uint argb, double opacity)
    {
        ThrowIfDisposed();
        using var paint = CreatePaint(argb, opacity);
        _canvas.DrawRect(new((float)left, (float)top, (float)right, (float)bottom), paint);
    }

    public void DrawPath(double[] coordinates, bool closed, bool evenOdd, uint argb, double opacity, double blurSigma = 0, bool stroke = false, double strokeWidth = 0)
    {
        ThrowIfDisposed();
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
        ThrowIfDisposed();
        using var bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul));
        pixels.CopyTo(bitmap.GetPixelSpan());
        using var paint = new SKPaint { Color = new SKColor(255, 255, 255, OpacityByte(opacity)), IsAntialias = false };
        var source = new SKRect((float)sourceLeft, (float)sourceTop, (float)sourceRight, (float)sourceBottom);
        var destination = new SKRect(
            (float)destinationLeft,
            (float)destinationTop,
            (float)destinationRight,
            (float)destinationBottom);
        _canvas.DrawBitmap(
            bitmap,
            source,
            destination,
            paint);
    }

    public void DrawText(string text, double x, double y, double fontSize, uint argb, double opacity, string? fontFamily = null)
    {
        ThrowIfDisposed();
        using var paint = CreatePaint(argb, opacity);
        FontFallbackTextRenderer.Draw(_canvas, text, (float)x, (float)y, (float)fontSize, paint, fontFamily);
    }

    public bool TryReadPixels(Span<byte> destination, int rowBytes)
    {
        ThrowIfDisposed();
        var requiredRowBytes = checked(_state.Width * 4);
        if (rowBytes < requiredRowBytes || destination.Length < checked(rowBytes * _state.Height))
        {
            throw new ArgumentException("The pixel destination is smaller than the BGRA8888 frame.", nameof(destination));
        }

        _canvas.Flush();
        using var image = _surface.Snapshot();
        using var bitmap = new SKBitmap(new SKImageInfo(
            _state.Width,
            _state.Height,
            SKColorType.Bgra8888,
            SKAlphaType.Premul));
        if (!image.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0))
        {
            return false;
        }
        var source = bitmap.GetPixelSpan();
        for (var row = 0; row < _state.Height; row++)
        {
            source.Slice(row * bitmap.RowBytes, requiredRowBytes)
                .CopyTo(destination.Slice(row * rowBytes, requiredRowBytes));
        }
        return true;
    }

    public void Present()
    {
        ThrowIfDisposed();
        if (_presented)
        {
            throw new InvalidOperationException("An OpenGL framebuffer can only be presented once.");
        }
        _canvas.Flush();
        _owner.Present(_state);
        _presented = true;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _surface.Dispose();
        _renderTarget.Dispose();
        _current.Dispose();
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    private static SKPaint CreatePaint(uint argb, double opacity) => new()
    {
        Color = ToColor(argb, opacity),
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

    private static SKColor ToColor(uint argb, double opacity) => new(
        (byte)(argb >> 16),
        (byte)(argb >> 8),
        (byte)argb,
        (byte)Math.Round((argb >> 24) * opacity));

    private static byte OpacityByte(double opacity) => (byte)Math.Round(255 * opacity);
}
