using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Vendor.Avalonia.Skia;

namespace Doroti.Backends.Skia;

public enum SurfaceBackendPreference
{
    Auto,
    SkiaSoftware,
    ManagedSoftware,
    Gpu,
}

public enum SurfaceBackendKind
{
    OpenGlGpu,
    SkiaSoftware,
    ManagedSoftware,
}

public enum HardwareFallbackPolicy
{
    AllowSoftware,
    HoldFramesUntilGpuRecovers,
}

public readonly record struct GpuResourceSnapshot(
    int ActiveSkiaContexts,
    long SkiaContextsCreated,
    long SkiaContextsReleased,
    int ActiveFrames,
    long FramesCreated,
    long FramesReleased)
{
    public bool IsBalanced =>
        ActiveSkiaContexts == 0 &&
        ActiveFrames == 0 &&
        SkiaContextsCreated == SkiaContextsReleased &&
        FramesCreated == FramesReleased;
}

public interface IGpuResourceDiagnostics
{
    GpuResourceSnapshot Snapshot { get; }
}

/// <summary>Diagnostic-only fault injection used to prove strict-GPU recovery on the active native surface.</summary>
public interface IGpuSurfaceRecoveryDiagnostics
{
    void FailNextFrame();
}

public sealed class SurfaceCreationResult
{
    private readonly SurfaceBackendKind _backend;
    private readonly string _diagnostic;

    internal SurfaceCreationResult(IRenderSurface surface, SurfaceBackendKind backend, string diagnostic)
    {
        Surface = surface;
        _backend = backend;
        _diagnostic = diagnostic;
        ResourceDiagnostics = surface as IGpuResourceDiagnostics ?? EmptyGpuResourceDiagnostics.Instance;
    }

    public IRenderSurface Surface { get; }

    public SurfaceBackendKind Backend => Surface is IAdaptiveRenderSurface adaptive ? adaptive.Backend : _backend;

    public string Diagnostic => Surface is IAdaptiveRenderSurface adaptive ? adaptive.Diagnostic : _diagnostic;

    public IGpuResourceDiagnostics ResourceDiagnostics { get; }
}

/// <summary>Selects the isolated Skia framebuffer target and an explicit managed fallback.</summary>
public static class SkiaSurfaceFactory
{
    /// <summary>
    /// Creates the A2 product surface. It never allocates or presents a managed full-frame
    /// framebuffer and never turns a software renderer into a successful hardware run.
    /// </summary>
    public static SurfaceCreationResult CreateHardware(
        IWindow window,
        HardwareFallbackPolicy fallbackPolicy = HardwareFallbackPolicy.HoldFramesUntilGpuRecovers)
    {
        ArgumentNullException.ThrowIfNull(window);
        if (fallbackPolicy is not HardwareFallbackPolicy.HoldFramesUntilGpuRecovers)
        {
            throw new ArgumentOutOfRangeException(nameof(fallbackPolicy));
        }
        if (!window.TryGetFeature<IOpenGlWindowTarget>(out var target) || target is null)
        {
            throw new NotSupportedException("A2 hardware mode requires a window-owned OpenGL target; software fallback is forbidden.");
        }
        return new(
            new HardwareOnlyRenderSurface(target),
            SurfaceBackendKind.OpenGlGpu,
            "Strict WGL/OpenGL initialization is pending on the raster thread; software fallback is forbidden.");
    }

    public static SurfaceCreationResult Create(IWindow window, SurfaceBackendPreference preference = SurfaceBackendPreference.Auto)
    {
        ArgumentNullException.ThrowIfNull(window);
        if (!window.TryGetFeature<IBgra8888FramebufferTarget>(out var target) || target is null)
        {
            throw new NotSupportedException("The window does not expose a backend-neutral BGRA8888 framebuffer target.");
        }

        if (preference is SurfaceBackendPreference.ManagedSoftware)
        {
            return new(
                new ManagedSoftwareRenderSurface(target),
                SurfaceBackendKind.ManagedSoftware,
                "Managed BGRA8888 software fallback was explicitly selected.");
        }

        if ((preference is SurfaceBackendPreference.Auto or SurfaceBackendPreference.Gpu) &&
            window.TryGetFeature<IOpenGlWindowTarget>(out var openGlTarget) &&
            openGlTarget is not null)
        {
            return new(
                new GpuFallbackRenderSurface(openGlTarget, target),
                SurfaceBackendKind.OpenGlGpu,
                "WGL/OpenGL GPU initialization is pending on the raster thread.");
        }

        if (preference is SurfaceBackendPreference.Gpu)
        {
            return new(
                new ManagedSoftwareRenderSurface(target),
                SurfaceBackendKind.ManagedSoftware,
                "The window has no OpenGL target; using the managed BGRA8888 fallback.");
        }

        try
        {
            return new(
                new SkiaRenderSurface(target),
                SurfaceBackendKind.SkiaSoftware,
                "Avalonia-derived Skia framebuffer target is active; no Avalonia render loop or composition types are loaded.");
        }
        catch (Exception exception) when (preference is SurfaceBackendPreference.Auto && IsSkiaInitializationFailure(exception))
        {
            return new(
                new ManagedSoftwareRenderSurface(target),
                SurfaceBackendKind.ManagedSoftware,
                $"Skia initialization failed ({exception.GetType().Name}: {exception.Message}); using the managed BGRA8888 fallback.");
        }
    }

    private static bool IsSkiaInitializationFailure(Exception exception) =>
        exception is DllNotFoundException or EntryPointNotFoundException or TypeInitializationException or BadImageFormatException;
}

internal sealed class HardwareOnlyRenderSurface : IRenderSurface, IAdaptiveRenderSurface, IGpuResourceDiagnostics, IGpuSurfaceRecoveryDiagnostics
{
    private readonly IOpenGlWindowTarget _target;
    private readonly GpuResourceTracker _resources = new();
    private GpuRenderSurface? _active;
    private SurfaceGeneration _physicalGeneration;
    private int _failNextFrame;
    private bool _disposed;

    internal HardwareOnlyRenderSurface(IOpenGlWindowTarget target) => _target = target;

    public SurfaceBackendKind Backend => SurfaceBackendKind.OpenGlGpu;

    public string Diagnostic { get; private set; } =
        "Strict WGL/OpenGL initialization is pending on the raster thread; software fallback is forbidden.";

    public SurfaceGeneration Generation { get; private set; } = new(1);

    public Size PixelSize => _active?.PixelSize ?? _target.Metrics.PixelSize;

    public GpuResourceSnapshot Snapshot => _resources.Snapshot;

    public void FailNextFrame() => Interlocked.Exchange(ref _failNextFrame, 1);

    public ISurfaceFrame BeginFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        EnsureActive();
        try
        {
            if (Interlocked.Exchange(ref _failNextFrame, 0) != 0)
            {
                throw new InvalidOperationException("G5-2 injected native GPU frame loss.");
            }
            var frame = _active!.BeginFrame();
            if (frame.Generation != _physicalGeneration)
            {
                _physicalGeneration = frame.Generation;
                Generation = Generation.Next();
            }
            return new HardwareOnlyFrame(this, frame, Generation);
        }
        catch (Exception exception)
        {
            Invalidate(exception);
            throw new SurfaceDeviceLostException(
                "The strict A2 GPU surface was lost. Frames are held until GPU recreation succeeds; no CPU fallback was selected.",
                exception);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _active?.Dispose();
        _active = null;
    }

    private void EnsureActive()
    {
        if (_active is not null)
        {
            return;
        }
        var gpu = new GpuRenderSurface(_target, _resources);
        if (!gpu.IsHardwareAccelerated)
        {
            var renderer = gpu.Renderer;
            gpu.Dispose();
            throw new NotSupportedException($"OpenGL renderer '{renderer}' is software-only; A2 hardware mode refuses it.");
        }
        _active = gpu;
        _physicalGeneration = gpu.Generation;
        Diagnostic = $"Strict Skia GPU rendering is active through WGL/OpenGL ({gpu.Renderer}; {gpu.Version}); software fallback is forbidden.";
    }

    private void Invalidate(Exception exception)
    {
        _active?.Dispose();
        _active = null;
        Generation = Generation.Next();
        Diagnostic = $"GPU context invalidated ({exception.GetType().Name}: {exception.Message}); recreation is required before another frame.";
    }

    private sealed class HardwareOnlyFrame(
        HardwareOnlyRenderSurface owner,
        ISurfaceFrame inner,
        SurfaceGeneration generation) : ISurfaceFrame, IPixelReadableSurfaceFrame, IMetricsBoundSurfaceFrame
    {
        public SurfaceGeneration Generation { get; } = generation;

        public long MetricsGeneration => inner is IMetricsBoundSurfaceFrame metrics ? metrics.MetricsGeneration : 0;

        public Size PixelSize => inner.PixelSize;

        public IRasterCanvas Canvas => inner.Canvas;

        public void Clear(Color color) => inner.Clear(color);

        public bool TryReadPixels(Span<byte> destination, int rowBytes) =>
            inner is IPixelReadableSurfaceFrame readable && readable.TryReadPixels(destination, rowBytes);

        public void Present()
        {
            try
            {
                inner.Present();
            }
            catch (Exception exception)
            {
                owner.Invalidate(exception);
                throw new SurfaceDeviceLostException(
                    "The strict A2 GPU present failed. Software fallback is forbidden.",
                    exception);
            }
        }

        public void Dispose() => inner.Dispose();
    }
}

internal interface IAdaptiveRenderSurface
{
    SurfaceBackendKind Backend { get; }

    string Diagnostic { get; }
}

internal sealed class GpuFallbackRenderSurface : IRenderSurface, IAdaptiveRenderSurface, IGpuResourceDiagnostics
{
    private readonly IOpenGlWindowTarget _gpuTarget;
    private readonly IBgra8888FramebufferTarget _softwareTarget;
    private readonly GpuResourceTracker _resources = new();
    private IRenderSurface? _active;
    private SurfaceGeneration _physicalGeneration;
    private bool _gpuAttempted;
    private bool _disposed;

    internal GpuFallbackRenderSurface(IOpenGlWindowTarget gpuTarget, IBgra8888FramebufferTarget softwareTarget)
    {
        _gpuTarget = gpuTarget;
        _softwareTarget = softwareTarget;
    }

    public SurfaceBackendKind Backend { get; private set; } = SurfaceBackendKind.OpenGlGpu;

    public string Diagnostic { get; private set; } = "WGL/OpenGL GPU initialization is pending on the raster thread.";

    public SurfaceGeneration Generation { get; private set; } = new(1);

    public Size PixelSize => _active?.PixelSize ?? _gpuTarget.Metrics.PixelSize;

    public GpuResourceSnapshot Snapshot => _resources.Snapshot;

    public ISurfaceFrame BeginFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        EnsureActive();
        try
        {
            return Wrap(_active!.BeginFrame());
        }
        catch (Exception exception) when (Backend is SurfaceBackendKind.OpenGlGpu)
        {
            SwitchToSoftware(exception);
            return Wrap(_active!.BeginFrame());
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _active?.Dispose();
    }

    private void EnsureActive()
    {
        if (_active is not null)
        {
            return;
        }
        _gpuAttempted = true;
        try
        {
            var gpu = new GpuRenderSurface(_gpuTarget, _resources);
            if (!gpu.IsHardwareAccelerated)
            {
                var renderer = gpu.Renderer;
                gpu.Dispose();
                throw new NotSupportedException($"OpenGL renderer '{renderer}' is software-only.");
            }
            _active = gpu;
            _physicalGeneration = gpu.Generation;
            Backend = SurfaceBackendKind.OpenGlGpu;
            Diagnostic = $"Skia GPU rendering is active through WGL/OpenGL ({gpu.Renderer}; {gpu.Version}).";
        }
        catch (Exception exception)
        {
            SwitchToSoftware(exception);
        }
    }

    private void SwitchToSoftware(Exception failure)
    {
        _active?.Dispose();
        _active = CreateSoftwareFallback(out var backend, out var fallbackDiagnostic);
        _physicalGeneration = _active.Generation;
        Backend = backend;
        Diagnostic = $"GPU unavailable ({failure.GetType().Name}: {failure.Message}); {fallbackDiagnostic}";
        if (_gpuAttempted)
        {
            Generation = Generation.Next();
            _gpuAttempted = false;
        }
    }

    private ISurfaceFrame Wrap(ISurfaceFrame frame)
    {
        if (frame.Generation != _physicalGeneration)
        {
            _physicalGeneration = frame.Generation;
            Generation = Generation.Next();
        }
        return new AdaptiveSurfaceFrame(this, frame, Generation);
    }

    private IRenderSurface CreateSoftwareFallback(out SurfaceBackendKind backend, out string diagnostic)
    {
        try
        {
            backend = SurfaceBackendKind.SkiaSoftware;
            diagnostic = "using the Skia BGRA8888 software fallback.";
            return new SkiaRenderSurface(_softwareTarget);
        }
        catch (Exception exception) when (
            exception is DllNotFoundException or EntryPointNotFoundException or TypeInitializationException or BadImageFormatException)
        {
            backend = SurfaceBackendKind.ManagedSoftware;
            diagnostic = $"Skia software initialization also failed ({exception.GetType().Name}); using the managed BGRA8888 fallback.";
            return new ManagedSoftwareRenderSurface(_softwareTarget);
        }
    }

    private sealed class AdaptiveSurfaceFrame(
        GpuFallbackRenderSurface owner,
        ISurfaceFrame inner,
        SurfaceGeneration generation) : ISurfaceFrame, IPixelReadableSurfaceFrame, IMetricsBoundSurfaceFrame
    {
        public SurfaceGeneration Generation { get; } = generation;

        public long MetricsGeneration => inner is IMetricsBoundSurfaceFrame metrics ? metrics.MetricsGeneration : 0;

        public Size PixelSize => inner.PixelSize;

        public IRasterCanvas Canvas => inner.Canvas;

        public void Clear(Color color) => inner.Clear(color);

        public bool TryReadPixels(Span<byte> destination, int rowBytes) =>
            inner is IPixelReadableSurfaceFrame readable && readable.TryReadPixels(destination, rowBytes);

        public void Present()
        {
            try
            {
                inner.Present();
            }
            catch (Exception exception) when (owner.Backend is SurfaceBackendKind.OpenGlGpu)
            {
                inner.Dispose();
                owner.SwitchToSoftware(exception);
                throw new SurfaceDeviceLostException("The OpenGL surface failed and switched to software rendering.", exception);
            }
        }

        public void Dispose() => inner.Dispose();
    }
}

internal sealed class GpuRenderSurface : IRenderSurface
{
    private readonly IOpenGlWindowTarget _target;
    private readonly IOpenGlWindowContext _context;
    private readonly NativeOpenGlRenderTarget _surface;
    private readonly GpuResourceTracker _resources;
    private int _contextCounted;
    private bool _disposed;

    internal GpuRenderSurface(IOpenGlWindowTarget target, GpuResourceTracker resources)
    {
        _target = target;
        _resources = resources;
        _context = target.CreateContext();
        var size = ReadPixelSize(target.Metrics);
        try
        {
            _surface = new(size.Width, size.Height, _context.MakeCurrent, _context.Present);
            _resources.SkiaContextCreated();
            _contextCounted = 1;
        }
        catch
        {
            _context.Dispose();
            throw;
        }
    }

    internal string Renderer => _context.Renderer;

    internal string Version => _context.Version;

    internal bool IsHardwareAccelerated => _context.IsHardwareAccelerated;

    public SurfaceGeneration Generation => SkiaAdapterBoundary.Convert(_surface.Descriptor).Generation;

    public Size PixelSize => SkiaAdapterBoundary.Convert(_surface.Descriptor).PixelSize;

    public ISurfaceFrame BeginFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var size = ReadPixelSize(_target.Metrics);
        _surface.Recreate(size.Width, size.Height);
        try
        {
            var metrics = _target.Metrics;
            return new MetricsBoundSurfaceFrame(
                new GpuResourceFrame(new SkiaSurfaceFrame(_surface.BeginFrame()), _resources),
                () => _target.Metrics,
                metrics.SurfaceGeneration,
                metrics.PixelSize);
        }
        catch (NativeGpuContextLostException exception)
        {
            throw new SurfaceDeviceLostException(exception.Message, exception);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        try
        {
            _surface.Dispose();
        }
        finally
        {
            try
            {
                _context.Dispose();
            }
            finally
            {
                if (Interlocked.Exchange(ref _contextCounted, 0) != 0)
                {
                    _resources.SkiaContextReleased();
                }
            }
        }
    }

    private static (int Width, int Height) ReadPixelSize(WindowMetrics metrics) =>
        (Math.Max(1, checked((int)metrics.PixelSize.Width)), Math.Max(1, checked((int)metrics.PixelSize.Height)));
}

internal sealed class MetricsBoundSurfaceFrame(
    ISurfaceFrame inner,
    Func<WindowMetrics> readMetrics,
    long metricsGeneration,
    Size expectedPixelSize) : ISurfaceFrame, IPixelReadableSurfaceFrame, IMetricsBoundSurfaceFrame
{
    public SurfaceGeneration Generation => inner.Generation;

    public long MetricsGeneration { get; } = metricsGeneration;

    public Size PixelSize => inner.PixelSize;

    public IRasterCanvas Canvas => inner.Canvas;

    public void Clear(Color color) => inner.Clear(color);

    public bool TryReadPixels(Span<byte> destination, int rowBytes) =>
        inner is IPixelReadableSurfaceFrame readable && readable.TryReadPixels(destination, rowBytes);

    public void Present()
    {
        var current = readMetrics();
        if (current.SurfaceGeneration != MetricsGeneration || current.PixelSize != expectedPixelSize)
        {
            throw new SurfaceStaleFrameException(
                $"Metrics changed from generation {MetricsGeneration} to {current.SurfaceGeneration} before present.");
        }
        inner.Present();
    }

    public void Dispose() => inner.Dispose();
}

internal sealed class GpuResourceFrame : ISurfaceFrame, IPixelReadableSurfaceFrame
{
    private readonly ISurfaceFrame _inner;
    private readonly GpuResourceTracker _resources;
    private int _counted = 1;

    internal GpuResourceFrame(ISurfaceFrame inner, GpuResourceTracker resources)
    {
        _inner = inner;
        _resources = resources;
        _resources.FrameCreated();
    }

    public SurfaceGeneration Generation => _inner.Generation;

    public Size PixelSize => _inner.PixelSize;

    public IRasterCanvas Canvas => _inner.Canvas;

    public void Clear(Color color) => _inner.Clear(color);

    public bool TryReadPixels(Span<byte> destination, int rowBytes) =>
        _inner is IPixelReadableSurfaceFrame readable && readable.TryReadPixels(destination, rowBytes);

    public void Present() => _inner.Present();

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _counted, 0) == 0)
        {
            return;
        }
        try
        {
            _inner.Dispose();
        }
        finally
        {
            _resources.FrameReleased();
        }
    }
}

internal sealed class GpuResourceTracker : IGpuResourceDiagnostics
{
    private long _contextsCreated;
    private long _contextsReleased;
    private long _framesCreated;
    private long _framesReleased;

    public GpuResourceSnapshot Snapshot
    {
        get
        {
            var contextsCreated = Interlocked.Read(ref _contextsCreated);
            var contextsReleased = Interlocked.Read(ref _contextsReleased);
            var framesCreated = Interlocked.Read(ref _framesCreated);
            var framesReleased = Interlocked.Read(ref _framesReleased);
            return new(
                checked((int)(contextsCreated - contextsReleased)),
                contextsCreated,
                contextsReleased,
                checked((int)(framesCreated - framesReleased)),
                framesCreated,
                framesReleased);
        }
    }

    internal void SkiaContextCreated() => Interlocked.Increment(ref _contextsCreated);

    internal void SkiaContextReleased() => Interlocked.Increment(ref _contextsReleased);

    internal void FrameCreated() => Interlocked.Increment(ref _framesCreated);

    internal void FrameReleased() => Interlocked.Increment(ref _framesReleased);
}

internal sealed class EmptyGpuResourceDiagnostics : IGpuResourceDiagnostics
{
    internal static EmptyGpuResourceDiagnostics Instance { get; } = new();

    public GpuResourceSnapshot Snapshot => default;
}

internal sealed class SkiaRenderSurface : IRenderSurface
{
    private readonly IBgra8888FramebufferTarget _target;
    private readonly NativeFramebufferRenderTarget _surface;
    private bool _disposed;

    internal SkiaRenderSurface(IBgra8888FramebufferTarget target)
    {
        _target = target;
        var size = ReadPixelSize(target.Metrics);
        _surface = new(size.Width, size.Height, target.Present);
    }

    public SurfaceGeneration Generation => SkiaAdapterBoundary.Convert(_surface.Descriptor).Generation;

    public Size PixelSize => SkiaAdapterBoundary.Convert(_surface.Descriptor).PixelSize;

    public ISurfaceFrame BeginFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var size = ReadPixelSize(_target.Metrics);
        _surface.Recreate(size.Width, size.Height);
        var metrics = _target.Metrics;
        return new MetricsBoundSurfaceFrame(
            new SkiaSurfaceFrame(_surface.BeginFrame()),
            () => _target.Metrics,
            metrics.SurfaceGeneration,
            metrics.PixelSize);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _surface.Dispose();
    }

    private static (int Width, int Height) ReadPixelSize(WindowMetrics metrics) =>
        (Math.Max(1, checked((int)metrics.PixelSize.Width)), Math.Max(1, checked((int)metrics.PixelSize.Height)));
}

internal sealed class SkiaSurfaceFrame : ISurfaceFrame, IPixelReadableSurfaceFrame
{
    private readonly INativeRasterFrame _frame;

    internal SkiaSurfaceFrame(INativeRasterFrame frame)
    {
        _frame = frame;
        Canvas = new NativeRasterCanvas(frame);
    }

    public SurfaceGeneration Generation => SkiaAdapterBoundary.Convert(_frame.Descriptor).Generation;

    public Size PixelSize => SkiaAdapterBoundary.Convert(_frame.Descriptor).PixelSize;

    public IRasterCanvas Canvas { get; }

    public void Clear(Color color) => _frame.Clear(color.Value);

    public bool TryReadPixels(Span<byte> destination, int rowBytes) => _frame.TryReadPixels(destination, rowBytes);

    public void Present() => _frame.Present();

    public void Dispose() => _frame.Dispose();
}

internal sealed class ManagedSoftwareRenderSurface : IRenderSurface
{
    private readonly IBgra8888FramebufferTarget _target;
    private byte[] _pixels = [];
    private long _metricsGeneration;
    private bool _disposed;

    internal ManagedSoftwareRenderSurface(IBgra8888FramebufferTarget target)
    {
        _target = target;
        RecreateIfNeeded();
    }

    public SurfaceGeneration Generation { get; private set; } = new(1);

    public Size PixelSize { get; private set; }

    public ISurfaceFrame BeginFrame()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RecreateIfNeeded();
        return new ManagedSoftwareFrame(this, Generation, _metricsGeneration, PixelSize, _pixels);
    }

    public void Dispose()
    {
        _disposed = true;
        _pixels = [];
    }

    internal void Present(SurfaceGeneration generation, byte[] pixels, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RecreateIfNeeded();
        if (generation != Generation || !ReferenceEquals(pixels, _pixels))
        {
            throw new SurfaceStaleFrameException("A stale framebuffer cannot be presented to a recreated surface.");
        }
        _target.Present(pixels, width, height, checked(width * 4));
    }

    private void RecreateIfNeeded()
    {
        var metrics = _target.Metrics;
        var width = Math.Max(1, checked((int)metrics.PixelSize.Width));
        var height = Math.Max(1, checked((int)metrics.PixelSize.Height));
        if (PixelSize == new Size(width, height) && _metricsGeneration == metrics.SurfaceGeneration)
        {
            return;
        }
        if (_pixels.Length != 0)
        {
            Generation = Generation.Next();
        }
        PixelSize = new(width, height);
        _metricsGeneration = metrics.SurfaceGeneration;
        _pixels = GC.AllocateUninitializedArray<byte>(checked(width * height * 4));
    }

    private sealed class ManagedSoftwareFrame : ISurfaceFrame, IPixelReadableSurfaceFrame, IMetricsBoundSurfaceFrame
    {
        private readonly ManagedSoftwareRenderSurface _owner;
        private readonly byte[] _pixels;
        private bool _disposed;
        private bool _presented;

        internal ManagedSoftwareFrame(
            ManagedSoftwareRenderSurface owner,
            SurfaceGeneration generation,
            long metricsGeneration,
            Size size,
            byte[] pixels)
        {
            _owner = owner;
            Generation = generation;
            MetricsGeneration = metricsGeneration;
            PixelSize = size;
            _pixels = pixels;
            Canvas = new SoftwareRasterCanvas(pixels, checked((int)size.Width), checked((int)size.Height));
        }

        public SurfaceGeneration Generation { get; }

        public long MetricsGeneration { get; }

        public Size PixelSize { get; }

        public IRasterCanvas Canvas { get; }

        public void Clear(Color color)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ((SoftwareRasterCanvas)Canvas).Clear(color);
        }

        public bool TryReadPixels(Span<byte> destination, int rowBytes)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var width = checked((int)PixelSize.Width);
            var height = checked((int)PixelSize.Height);
            var requiredRowBytes = checked(width * 4);
            if (rowBytes < requiredRowBytes || destination.Length < checked(rowBytes * height))
            {
                throw new ArgumentException("The pixel destination is smaller than the BGRA8888 frame.", nameof(destination));
            }
            for (var row = 0; row < height; row++)
            {
                _pixels.AsSpan(row * requiredRowBytes, requiredRowBytes)
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
            _owner.Present(Generation, _pixels, checked((int)PixelSize.Width), checked((int)PixelSize.Height));
            _presented = true;
        }

        public void Dispose() => _disposed = true;
    }
}
