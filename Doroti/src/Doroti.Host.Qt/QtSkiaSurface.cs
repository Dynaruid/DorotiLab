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
    private bool _usePlatformGlResolver;
    private bool _disposed;

    internal void Render(in QtNativeV2.Surface descriptor, Action<SKSurface> render)
    {
        ArgumentNullException.ThrowIfNull(render);
        ObjectDisposedException.ThrowIf(_disposed, this);
        Validate(descriptor);
        if (RequiresRecreate(descriptor)) CreateSurface(descriptor);
        // The native host clears the Qt-bound FBO before this callback. Forget
        // Skia's cached GL state so it cannot assume state left by the previous
        // frame (notably scissor and color-write masks).
        _context!.ResetContext();
        render(_surface!);
        _surface!.Canvas.Flush();
        _context!.Flush(_surface);
        _context.Submit(false);
    }

    internal void Release(ulong surfaceGeneration, ulong contextIdentity)
    {
        if (_disposed) return;
        if (_surfaceGeneration != surfaceGeneration || _contextIdentity != contextIdentity) return;
        ReleaseGpuResources();
    }

    internal void SetQpaPlatform(string platform) =>
        _usePlatformGlResolver = string.Equals(platform, "xcb", StringComparison.OrdinalIgnoreCase);

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
        var contextChanged = _context is null || _contextIdentity != descriptor.ContextIdentity;
        if (contextChanged)
        {
            ReleaseGpuResources();
            // Qt's GLX resolver returned thunk addresses that SkiaSharp 4.151.1 could not
            // safely assemble on this Mesa/xcb path. libGL's current-context resolver
            // is correct for GLX; Wayland/EGL continues to use Qt's resolver.
            _interface = _usePlatformGlResolver
                ? GRGlInterface.Create()
                : GRGlInterface.Create(_getProcedureAddress);
            if (_interface is null || !_interface.Validate())
                throw new InvalidOperationException("SkiaSharp could not create a valid interface for the current Qt OpenGL context.");
            _context = GRContext.CreateGl(_interface)
                ?? throw new InvalidOperationException("SkiaSharp could not create a GPU context for the current Qt OpenGL context.");
        }
        else
        {
            ReleaseRenderTarget();
        }
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
        ReleaseRenderTarget();
        _context?.Dispose();
        _context = null;
        _interface?.Dispose();
        _interface = null;
        _contextIdentity = 0;
    }

    private void ReleaseRenderTarget()
    {
        _surface?.Dispose();
        _surface = null;
        _target?.Dispose();
        _target = null;
        _surfaceGeneration = 0;
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
