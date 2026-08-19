using SkiaSharp;

namespace Doroti.Host.Qt;

internal sealed class QtSkiaSurface(GRGlGetProcedureAddressDelegate getProcedureAddress) : IDisposable
{
    private readonly GRGlGetProcedureAddressDelegate _getProcedureAddress =
        getProcedureAddress ?? throw new ArgumentNullException(nameof(getProcedureAddress));
    private GRGlInterface? _interface;
    private GRContext? _context;
    private GRBackendRenderTarget? _target;
    private SKSurface? _surface;
    private ulong _surfaceGeneration;
    private ulong _contextIdentity;
    private uint _framebufferObject;
    private int _pixelWidth;
    private int _pixelHeight;
    private bool _disposed;

    internal void Render(in QtNativeV2.Surface descriptor)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Validate(descriptor);
        if (RequiresRecreate(descriptor)) CreateSurface(descriptor);

        var canvas = _surface!.Canvas;
        canvas.Clear(new SKColor(0xff, 0xfb, 0xfe));
        using var primary = new SKPaint { Color = new SKColor(0x67, 0x50, 0xa4), IsAntialias = true };
        using var accent = new SKPaint { Color = new SKColor(0xb3, 0x26, 0x1e), IsAntialias = true };
        using var textPaint = new SKPaint { Color = new SKColor(0x1c, 0x1b, 0x1f), IsAntialias = true };
        using var typeface = SKTypeface.FromFamilyName("sans-serif", SKFontStyle.Bold);
        using var font = new SKFont(typeface, Math.Max(18, Math.Min(_pixelWidth, _pixelHeight) / 18f));
        var margin = Math.Max(20, Math.Min(_pixelWidth, _pixelHeight) / 12f);
        canvas.DrawRoundRect(new SKRect(margin, margin, _pixelWidth - margin, _pixelHeight - margin), 28, 28, primary);
        canvas.DrawCircle(_pixelWidth * 0.28f, _pixelHeight * 0.56f,
            Math.Max(24, Math.Min(_pixelWidth, _pixelHeight) * 0.13f), accent);
        canvas.DrawRect(new SKRect(_pixelWidth * 0.52f, _pixelHeight * 0.40f,
            _pixelWidth * 0.82f, _pixelHeight * 0.70f), accent);
        canvas.DrawText("Doroti Qt + Skia GPU", margin * 1.5f, margin * 2.2f,
            SKTextAlign.Left, font, textPaint);
        canvas.Flush();
        _context!.Flush(_surface);
        _context.Submit(false);
    }

    internal void Release(ulong surfaceGeneration, ulong contextIdentity)
    {
        if (_disposed) return;
        if (_surfaceGeneration != surfaceGeneration || _contextIdentity != contextIdentity) return;
        ReleaseGpuResources();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        ReleaseGpuResources();
    }

    private bool RequiresRecreate(in QtNativeV2.Surface descriptor) =>
        _surface is null || _surfaceGeneration != descriptor.SurfaceGeneration ||
        _contextIdentity != descriptor.ContextIdentity ||
        _framebufferObject != descriptor.FramebufferObject ||
        _pixelWidth != descriptor.PixelWidth || _pixelHeight != descriptor.PixelHeight;

    private void CreateSurface(in QtNativeV2.Surface descriptor)
    {
        ReleaseGpuResources();
        _interface = GRGlInterface.Create(_getProcedureAddress);
        if (_interface is null || !_interface.Validate())
            throw new InvalidOperationException("SkiaSharp could not create a valid interface for the current Qt OpenGL context.");
        _context = GRContext.CreateGl(_interface)
            ?? throw new InvalidOperationException("SkiaSharp could not create a GPU context for the current Qt OpenGL context.");
        var framebuffer = new GRGlFramebufferInfo(descriptor.FramebufferObject, descriptor.ColorFormat);
        _target = new GRBackendRenderTarget(descriptor.PixelWidth, descriptor.PixelHeight,
            Math.Max(0, descriptor.SampleCount), Math.Max(0, descriptor.StencilBits), framebuffer);
        _surface = SKSurface.Create(_context, _target, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888)
            ?? throw new InvalidOperationException("SkiaSharp could not wrap the Qt default framebuffer object.");
        _surfaceGeneration = descriptor.SurfaceGeneration;
        _contextIdentity = descriptor.ContextIdentity;
        _framebufferObject = descriptor.FramebufferObject;
        _pixelWidth = descriptor.PixelWidth;
        _pixelHeight = descriptor.PixelHeight;
    }

    private void ReleaseGpuResources()
    {
        _surface?.Dispose();
        _surface = null;
        _target?.Dispose();
        _target = null;
        _context?.Dispose();
        _context = null;
        _interface?.Dispose();
        _interface = null;
        _surfaceGeneration = 0;
        _contextIdentity = 0;
        _framebufferObject = 0;
        _pixelWidth = 0;
        _pixelHeight = 0;
    }

    private static void Validate(in QtNativeV2.Surface descriptor)
    {
        if (descriptor.AbiVersion != QtNativeV2.AbiVersion || descriptor.StructSize < 88)
            throw new InvalidDataException("The Qt surface descriptor does not match doroti.qt-host/v2.");
        if (descriptor.ContextIdentity == 0 || descriptor.PixelWidth <= 0 || descriptor.PixelHeight <= 0)
            throw new InvalidDataException("The Qt surface descriptor is missing a current context or physical size.");
    }
}
