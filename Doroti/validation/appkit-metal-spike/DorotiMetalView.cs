using AppKit;
using CoreGraphics;
using Doroti.Skia.Rendering;
using Foundation;
using Metal;
using MetalKit;
using SkiaSharp;

namespace Doroti.Validation.AppKitMetalSpike;

internal sealed class DorotiMetalView : MTKView, IMTKViewDelegate
{
    private readonly object _resourceGate = new();
    private readonly object _diagnosticsGate = new();
    private readonly IMTLDevice _metalDevice;
    private readonly IMTLCommandQueue _commandQueue;
    private readonly GRMtlBackendContext _backendContext;
    private readonly string _metalDeviceName;
    private GRContext? _grContext;
    private DorotiMetalSurface? _owner;
    private CGSize _lastDrawableSize;
    private long _surfaceGeneration = 1;
    private long _metricsGeneration = 1;
    private long _commandBuffersCommitted;
    private long _commandBuffersCompleted;
    private long _commandBuffersErrored;
    private long _staleCompletions;
    private int _inFlight;
    private bool _releaseRequested;
    private bool _resourcesReleased;
    private double _logicalWidth;
    private double _logicalHeight;
    private double _pixelWidth;
    private double _pixelHeight;
    private double _dpr = 1;

    internal DorotiMetalView() : base(CGRect.Empty, RequireMetalDevice())
    {
        _metalDevice = Device ?? throw new InvalidOperationException("MTKView did not retain its Metal device.");
        _metalDeviceName = _metalDevice.Name;
        _commandQueue = _metalDevice.CreateCommandQueue() ??
            throw new InvalidOperationException("Metal command queue creation failed.");
        _backendContext = new GRMtlBackendContext
        {
            Device = _metalDevice,
            Queue = _commandQueue,
        };
        ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
        DepthStencilPixelFormat = MTLPixelFormat.Depth32Float_Stencil8;
        SampleCount = 1;
        FramebufferOnly = false;
        AutoResizeDrawable = true;
        Paused = true;
        EnableSetNeedsDisplay = true;
        Delegate = this;
    }

    internal void Connect(DorotiMetalSurface owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        owner.ConnectNativeView(this);
        RequestFrame();
    }

    internal void Disconnect()
    {
        _owner = null;
        Interlocked.Increment(ref _surfaceGeneration);
        lock (_resourceGate)
        {
            _releaseRequested = true;
            if (_inFlight == 0) ReleaseGpuResources();
        }
    }

    internal void RequestFrame()
    {
        if (_releaseRequested) return;
        BeginInvokeOnMainThread(() =>
        {
            if (!_releaseRequested) NeedsDisplay = true;
        });
    }

    void IMTKViewDelegate.DrawableSizeWillChange(MTKView view, CGSize size)
    {
        _ = view;
        if (size.Equals(_lastDrawableSize)) return;
        _lastDrawableSize = size;
        Interlocked.Increment(ref _metricsGeneration);
        Interlocked.Increment(ref _surfaceGeneration);
    }

    void IMTKViewDelegate.Draw(MTKView view)
    {
        _ = view;
        var owner = _owner;
        var drawable = CurrentDrawable;
        var size = DrawableSize;
        if (owner is null || drawable?.Texture is null || size.Width <= 0 || size.Height <= 0)
            return;
        var scale = Window?.Screen?.BackingScaleFactor ?? NSScreen.MainScreen?.BackingScaleFactor ?? 1;
        lock (_diagnosticsGate)
        {
            _logicalWidth = Bounds.Width;
            _logicalHeight = Bounds.Height;
            _pixelWidth = size.Width;
            _pixelHeight = size.Height;
            _dpr = (double)scale;
        }

        _lastDrawableSize = size;
        _grContext ??= GRContext.CreateMetal(_backendContext) ??
            throw new InvalidOperationException("Skia Metal GRContext creation failed.");
        var surfaceGeneration = Interlocked.Read(ref _surfaceGeneration);
        SkiaPaintCompletion? completion = null;
        var commandBufferTracked = false;
        try
        {
            var textureInfo = new GRMtlTextureInfo(drawable.Texture);
            using var renderTarget = new GRBackendRenderTarget(
                checked((int)size.Width),
                checked((int)size.Height),
                textureInfo);
            using var surface = SKSurface.Create(
                _grContext,
                renderTarget,
                GRSurfaceOrigin.TopLeft,
                SKColorType.Bgra8888) ??
                throw new InvalidOperationException("Skia Metal SKSurface creation failed.");

            completion = owner.Paint(
                surface,
                checked((int)size.Width),
                checked((int)size.Height),
                surfaceGeneration);
            surface.Canvas.Flush();
            surface.Flush();
            _grContext.Flush();

            using var commandBuffer = _commandQueue.CommandBuffer() ??
                throw new InvalidOperationException("Metal command buffer creation failed.");
            commandBuffer.PresentDrawable(drawable);
            TrackCommandBuffer(commandBuffer, owner, completion, surfaceGeneration);
            commandBufferTracked = true;
            commandBuffer.Commit();
            commandBufferTracked = false;
            Interlocked.Increment(ref _commandBuffersCommitted);
        }
        catch (Exception exception)
        {
            if (commandBufferTracked) CancelCommandBufferTracking();
            Console.Error.WriteLine($"[DorotiMetalView] draw failed: {exception}");
            owner.FailPaint(completion, exception.ToString());
        }
    }

    private void TrackCommandBuffer(
        IMTLCommandBuffer commandBuffer,
        DorotiMetalSurface owner,
        SkiaPaintCompletion? completion,
        long surfaceGeneration)
    {
        lock (_resourceGate) _inFlight++;
        commandBuffer.AddCompletedHandler(buffer =>
        {
            try
            {
                var stale = surfaceGeneration != Interlocked.Read(ref _surfaceGeneration) ||
                            !ReferenceEquals(owner, _owner);
                if (stale) Interlocked.Increment(ref _staleCompletions);
                if (buffer.Status == MTLCommandBufferStatus.Completed)
                {
                    Interlocked.Increment(ref _commandBuffersCompleted);
                    if (completion is { } completed) owner.CompletePaint(completed, stale);
                }
                else if (buffer.Status != MTLCommandBufferStatus.Completed)
                {
                    Interlocked.Increment(ref _commandBuffersErrored);
                    owner.FailPaint(completion, buffer.Error?.LocalizedDescription ?? buffer.Status.ToString());
                }
            }
            finally
            {
                lock (_resourceGate)
                {
                    _inFlight--;
                    if (_releaseRequested && _inFlight == 0) ReleaseGpuResources();
                }
            }
        });
    }

    private void CancelCommandBufferTracking()
    {
        lock (_resourceGate)
        {
            _inFlight--;
            if (_releaseRequested && _inFlight == 0) ReleaseGpuResources();
        }
    }

    internal object CaptureDiagnostics()
    {
        lock (_diagnosticsGate)
        {
            return new
            {
                nativeViewType = GetType().FullName,
                metalDevice = _metalDeviceName,
                pixelFormat = "BGRA8Unorm",
                sampleCount = 1,
                stencil = "Depth32Float_Stencil8",
                logicalSize = new { width = _logicalWidth, height = _logicalHeight },
                pixelSize = new { width = _pixelWidth, height = _pixelHeight },
                dpr = _dpr,
                metricsGeneration = Interlocked.Read(ref _metricsGeneration),
                contextGeneration = 1,
                surfaceGeneration = Interlocked.Read(ref _surfaceGeneration),
                commandBuffersCommitted = Interlocked.Read(ref _commandBuffersCommitted),
                commandBuffersCompleted = Interlocked.Read(ref _commandBuffersCompleted),
                commandBuffersErrored = Interlocked.Read(ref _commandBuffersErrored),
                staleCompletions = Interlocked.Read(ref _staleCompletions),
            };
        }
    }

    private static IMTLDevice RequireMetalDevice() => MTLDevice.SystemDefault ??
        throw new PlatformNotSupportedException("The AppKit Metal spike requires a Metal-capable device.");

    private void ReleaseGpuResources()
    {
        if (_resourcesReleased) return;
        _resourcesReleased = true;
        _grContext?.Dispose();
        _grContext = null;
        _backendContext.Dispose();
        _commandQueue.Dispose();
        _metalDevice.Dispose();
    }
}
