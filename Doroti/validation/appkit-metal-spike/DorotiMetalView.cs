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
    internal static bool UseGraphite => Environment.GetEnvironmentVariable("DOROTI_APPKIT_SPIKE_GRAPHITE") == "1";
    private GRContext? _grContext;
    private SkiaGraphiteSession? _graphite;
    private int _peakInFlight;
    private long _nilDrawables;
    private long _backpressure;
    private DorotiMetalSurface? _owner;
    private DorotiMetalSurface? _resourceOwner;
    private readonly TaskCompletionSource _shutdown = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _shutdownStartedWithInFlight;
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
        _resourceOwner = owner;
        owner.ConnectNativeView(this);
        RequestFrame();
    }

    internal void Disconnect()
    {
        if (_releaseRequested) return;
        _owner = null;
        Interlocked.Increment(ref _surfaceGeneration);
        lock (_resourceGate)
        {
            _releaseRequested = true;
            _shutdownStartedWithInFlight = _inFlight;
            _graphite?.StopAcceptingFrames();
            if (_inFlight == 0) ReleaseGpuResources();
        }
    }

    internal Task ShutdownAsync()
    {
        Disconnect();
        return _shutdown.Task;
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
        var size = DrawableSize;
        if (owner is null || _releaseRequested || size.Width <= 0 || size.Height <= 0) return;
        if (_inFlight >= 3)
        {
            Interlocked.Increment(ref _backpressure);
            return; // Completion requests the next frame.
        }
        var drawable = CurrentDrawable;
        if (drawable?.Texture is null)
        {
            Interlocked.Increment(ref _nilDrawables);
            RequestFrame();
            return;
        }
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
        var surfaceGeneration = Interlocked.Read(ref _surfaceGeneration);
        SkiaPaintCompletion? completion = null;
        var commandBufferTracked = false;
        SkiaGraphiteSession.Frame? graphiteFrame = null;
        var graphiteSubmissionAttempted = false;
        try
        {
            if (UseGraphite)
                _graphite ??= SkiaGraphiteSession.CreateMetal(_metalDevice.Handle, _commandQueue.Handle, 1);
            else
                _grContext ??= GRContext.CreateMetal(_backendContext) ??
                    throw new InvalidOperationException("Skia Metal GRContext creation failed.");
            if (!UseGraphite) _grContext!.SetResourceCacheLimit(SkiaGraphiteSession.ContextBudgetBytes);
            using var renderTarget = UseGraphite ? null : new GRBackendRenderTarget(
                checked((int)size.Width), checked((int)size.Height), new GRMtlTextureInfo(drawable.Texture));
            if (UseGraphite)
                graphiteFrame = _graphite!.BeginMetalFrame(checked((int)size.Width), checked((int)size.Height), drawable.Texture.Handle);
            using var ganeshSurface = UseGraphite ? null : SKSurface.Create(_grContext, renderTarget!,
                GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888)
                ?? throw new InvalidOperationException("Skia Metal SKSurface creation failed.");
            var surface = graphiteFrame?.Surface ?? ganeshSurface!;

            completion = owner.Paint(
                surface,
                checked((int)size.Width),
                checked((int)size.Height),
                surfaceGeneration, (double)scale);
            if (graphiteFrame is not null)
            {
                graphiteSubmissionAttempted = true;
                graphiteFrame.Submit();
            }
            else
            {
                surface.Canvas.Flush();
                surface.Flush();
                _grContext!.Flush(submit: true, synchronous: false);
            }

            using var commandBuffer = _commandQueue.CommandBuffer() ??
                throw new InvalidOperationException("Metal command buffer creation failed.");
            commandBuffer.PresentDrawable(drawable);
            TrackCommandBuffer(commandBuffer, owner, completion, surfaceGeneration, graphiteFrame, drawable);
            commandBufferTracked = true;
            commandBuffer.Commit();
            graphiteFrame = null; // Ownership transferred to the completion callback.
            commandBufferTracked = false;
            Interlocked.Increment(ref _commandBuffersCommitted);
        }
        catch (Exception exception)
        {
            if (commandBufferTracked) CancelCommandBufferTracking();
            if (graphiteFrame is not null)
            {
                if (!graphiteSubmissionAttempted) graphiteFrame.CancelRecording();
                else
                {
                    // Even a failed submission attempt needs a same-queue fence
                    // before its surface/recording can be released.
                    using var fence = _commandQueue.CommandBuffer();
                    if (fence is not null)
                    {
                        TrackCommandBuffer(fence, owner, null, surfaceGeneration, graphiteFrame, drawable);
                        fence.Commit();
                        Interlocked.Increment(ref _commandBuffersCommitted);
                    }
                }
            }
            Console.Error.WriteLine($"[DorotiMetalView] draw failed: {exception}");
            owner.FailPaint(completion, exception.ToString());
        }
    }

    private void TrackCommandBuffer(
        IMTLCommandBuffer commandBuffer,
        DorotiMetalSurface owner,
        SkiaPaintCompletion? completion,
        long surfaceGeneration,
        SkiaGraphiteSession.Frame? graphiteFrame,
        CoreAnimation.ICAMetalDrawable drawable)
    {
        commandBuffer.AddCompletedHandler(buffer =>
        {
            var status = buffer.Status;
            var error = buffer.Error?.LocalizedDescription;
            // Graphite context/recorder and renderer caches stay on their owner.
            BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var stale = surfaceGeneration != Interlocked.Read(ref _surfaceGeneration) ||
                                !ReferenceEquals(owner, _owner);
                    if (stale) Interlocked.Increment(ref _staleCompletions);
                    if (status == MTLCommandBufferStatus.Completed)
                    {
                        Interlocked.Increment(ref _commandBuffersCompleted);
                        if (completion is { } completed) owner.CompletePaint(completed, stale);
                    }
                    else if (status != MTLCommandBufferStatus.Completed)
                    {
                        Interlocked.Increment(ref _commandBuffersErrored);
                        owner.FailPaint(completion, error ?? status.ToString());
                    }
                }
                finally
                {
                    graphiteFrame?.CompleteGpuWork();
                    GC.KeepAlive(drawable);
                    lock (_resourceGate)
                    {
                        _inFlight--;
                        if (_releaseRequested && _inFlight == 0) ReleaseGpuResources();
                    }
                    if (!_releaseRequested && Interlocked.Exchange(ref _backpressure, 0) != 0) RequestFrame();
                }
            });
        });
        lock (_resourceGate)
        {
            _inFlight++;
            _peakInFlight = Math.Max(_peakInFlight, _inFlight);
        }
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
                graphite = UseGraphite,
                contextBudgetBytes = SkiaGraphiteSession.ContextBudgetBytes,
                recorderBudgetBytes = UseGraphite ? SkiaGraphiteSession.RecorderBudgetBytes : 0,
                maxInFlight = 3,
                peakInFlight = _peakInFlight,
                outstandingFrames = _inFlight,
                resourcesReleased = _resourcesReleased,
                shutdownStartedWithInFlight = _shutdownStartedWithInFlight,
                nilDrawables = Interlocked.Read(ref _nilDrawables),
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
        _resourceOwner?.ReleaseRendererResources();
        _resourceOwner = null;
        _graphite?.StopAcceptingFrames();
        _graphite?.Dispose();
        _graphite = null;
        _grContext?.Dispose();
        _grContext = null;
        _backendContext.Dispose();
        _commandQueue.Dispose();
        _metalDevice.Dispose();
        _resourcesReleased = true;
        _shutdown.TrySetResult();
    }
}
