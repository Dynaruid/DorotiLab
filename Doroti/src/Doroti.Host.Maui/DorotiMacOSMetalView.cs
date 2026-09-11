#if MACOS
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Doroti.Ui;
using Doroti.Skia.Rendering;
using Foundation;
using Metal;
using MetalKit;
using SkiaSharp;

namespace Doroti.Host.Maui;

/// <summary>
/// AppKit render/input surface. It owns the Metal queue so a frame is acknowledged
/// only after the command buffer that presents its drawable has completed.
/// </summary>
public sealed class DorotiMacOSMetalView : MTKView, IMTKViewDelegate
{
    private readonly object _resourceGate = new();
    private readonly IMTLDevice _metalDevice;
    private readonly IMTLCommandQueue _commandQueue;
    private readonly GRMtlBackendContext _backendContext;
    internal static readonly bool UseGraphite = Environment.GetEnvironmentVariable("DOROTI_MACOS_GRAPHITE") != "0";
    internal static string GraphicsBackendId => UseGraphite ? "AppKit/MTKView/Graphite-Metal" : "AppKit/MTKView/Metal-Skia";
    private GRContext? _grContext;
    private SkiaGraphiteSession? _graphite;
    private DorotiMacOSMetalSurface? _resourceOwner;
    private bool _frameBackpressure;
    private DorotiMacOSMetalSurface? _owner;
    private readonly Dictionary<long, long> _pressedKeys = [];
    private NSTrackingArea? _trackingArea;
    private NSObject? _windowBecameKeyObserver;
    private NSObject? _windowResignedKeyObserver;
    private CGSize _lastDrawableSize;
    private CGSize _lastLogicalSize;
    private CGSize _lastLayoutSize;
    private double _lastBackingScale;
    private double _lastLayoutScale;
    private long _surfaceGeneration = 1;
    private long _contextGeneration = 1;
    private long _commandBuffersCommitted;
    private long _commandBuffersCompleted;
    private long _commandBuffersErrored;
    private long _staleCompletions;
    private double _logicalWidth;
    private double _logicalHeight;
    private double _pixelWidth;
    private double _pixelHeight;
    private double _density = 1;
    private int _inFlight;
    private bool _releaseRequested;
    private bool _resourcesReleased;
    private bool _cursorHidden;
    private bool _controlClick;
    private bool _drawingLayout;
    private int _frameRequestPending;

    public DorotiMacOSMetalView() : base(CGRect.Empty, RequireMetalDevice())
    {
        _metalDevice = Device ?? throw new InvalidOperationException("MTKView did not retain its Metal device.");
        _commandQueue = _metalDevice.CreateCommandQueue() ??
            throw new InvalidOperationException("Metal command queue creation failed.");
        _backendContext = new GRMtlBackendContext { Device = _metalDevice, Queue = _commandQueue };
        ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
        DepthStencilPixelFormat = MTLPixelFormat.Depth32Float_Stencil8;
        SampleCount = 1;
        FramebufferOnly = false;
        // MTKView's automatic resize lets AppKit scale the previous drawable
        // to the new bounds until the next Metal presentation. Own the backing
        // size in Layout so live resize never exposes a stretched frame.
        AutoResizeDrawable = false;
        LayerContentsPlacement = NSViewLayerContentsPlacement.TopLeft;
        if (Layer is { } layer)
        {
            layer.ContentsGravity = CALayer.GravityTopLeft;
            layer.MasksToBounds = true;
        }
        Paused = true;
        EnableSetNeedsDisplay = true;
        Delegate = this;
    }

    public override bool AcceptsFirstResponder() => true;

    internal AppKitPlatformRasterSurface CreatePlatformRasterSurface() => new(_metalDevice, _commandQueue, _grContext);

    internal void Connect(DorotiMacOSMetalSurface owner)
    {
        if (_resourcesReleased || _releaseRequested)
            throw new InvalidOperationException("A disconnected Metal view requires a fresh handler/native view.");
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _resourceOwner = owner;
        _releaseRequested = false;
        AttachWindowObservers();
        PublishDrawableMetrics(DrawableSize, force: true);
        RequestFrame();
    }

    internal void Disconnect()
    {
        var owner = _owner;
        _owner = null;
        if (owner is null && _releaseRequested) return;
        Interlocked.Increment(ref _surfaceGeneration);
        DetachWindowObservers();
        if (_trackingArea is not null)
        {
            RemoveTrackingArea(_trackingArea);
            _trackingArea.Dispose();
            _trackingArea = null;
        }
        RestoreCursorVisibility();
        lock (_resourceGate)
        {
            _releaseRequested = true;
            _graphite?.StopAcceptingFrames();
            if (_inFlight == 0) ReleaseGpuResources();
        }
    }

    internal void RequestFrame()
    {
        if (_releaseRequested) return;
        if (Interlocked.Exchange(ref _frameRequestPending, 1) != 0) return;
        BeginInvokeOnMainThread(() =>
        {
            Interlocked.Exchange(ref _frameRequestPending, 0);
            if (!_releaseRequested) NeedsDisplay = true;
        });
    }

    internal void RequestFocus(bool focused)
    {
        BeginInvokeOnMainThread(() =>
        {
            if (focused) Window?.MakeFirstResponder(this);
            else if (ReferenceEquals(Window?.FirstResponder, this)) Window?.MakeFirstResponder(null);
        });
    }

    internal void SetCursor(DorotiMouseCursorKind cursor)
    {
        BeginInvokeOnMainThread(() =>
        {
            if (cursor == DorotiMouseCursorKind.none)
            {
                if (!_cursorHidden) NSCursor.Hide();
                _cursorHidden = true;
                return;
            }
            RestoreCursorVisibility();
            var native = cursor switch
            {
                DorotiMouseCursorKind.click => NSCursor.PointingHandCursor,
                DorotiMouseCursorKind.text or DorotiMouseCursorKind.verticalText => NSCursor.IBeamCursor,
                DorotiMouseCursorKind.precise => NSCursor.CrosshairCursor,
                DorotiMouseCursorKind.resizeLeftRight => NSCursor.ResizeLeftRightCursor,
                DorotiMouseCursorKind.resizeUpDown => NSCursor.ResizeUpDownCursor,
                _ => NSCursor.ArrowCursor,
            };
            native.Set();
        });
    }

    public override void ViewDidMoveToWindow()
    {
        base.ViewDidMoveToWindow();
        AttachWindowObservers();
        _owner?.RaiseFocus(Window?.IsKeyWindow == true && ReferenceEquals(Window.FirstResponder, this));
        PublishDrawableMetrics(DrawableSize, force: true);
        RequestFrame();
    }

    public override void DidChangeBackingProperties()
    {
        base.DidChangeBackingProperties();
        // AutoResizeDrawable is intentionally disabled, so a Retina scale
        // transition must explicitly run the same exact-backing layout path.
        _lastLayoutScale = 0;
        NeedsLayout = true;
        RequestFrame();
    }

    public override void Layout()
    {
        base.Layout();
        var logicalSize = Bounds.Size;
        var scale = BackingScale();
        if (_drawingLayout || logicalSize.Width <= 0 || logicalSize.Height <= 0) return;
        if (logicalSize.Equals(_lastLayoutSize) && scale.Equals(_lastLayoutScale)) return;

        _lastLayoutSize = logicalSize;
        _lastLayoutScale = scale;
        var drawableSize = new CGSize(
            Math.Max(1, Math.Round(logicalSize.Width * scale)),
            Math.Max(1, Math.Round(logicalSize.Height * scale)));
        try
        {
            _drawingLayout = true;
            CATransaction.Begin();
            try
            {
                CATransaction.DisableActions = true;
                // Synchronize only the live-layout presentation. Keeping this
                // enabled after Layout prevents ordinary input-driven frames
                // from being displayed by this custom command-buffer owner.
                PresentsWithTransaction = true;
                if (Layer is { } layer)
                {
                    layer.ContentsGravity = CALayer.GravityTopLeft;
                    layer.ContentsScale = (System.Runtime.InteropServices.NFloat)scale;
                }
                DrawableSize = drawableSize;
                PublishDrawableMetrics(drawableSize);
                Draw();
            }
            finally
            {
                try
                {
                    CATransaction.Commit();
                }
                finally
                {
                    PresentsWithTransaction = false;
                }
            }
        }
        finally
        {
            _drawingLayout = false;
        }
    }

    public override void UpdateTrackingAreas()
    {
        if (_trackingArea is not null)
        {
            RemoveTrackingArea(_trackingArea);
            _trackingArea.Dispose();
        }
        _trackingArea = new NSTrackingArea(Bounds,
            NSTrackingAreaOptions.ActiveInKeyWindow |
            NSTrackingAreaOptions.InVisibleRect |
            NSTrackingAreaOptions.MouseEnteredAndExited |
            NSTrackingAreaOptions.MouseMoved,
            this, null!);
        AddTrackingArea(_trackingArea);
        base.UpdateTrackingAreas();
    }

    void IMTKViewDelegate.DrawableSizeWillChange(MTKView view, CGSize size)
    {
        _ = view;
        PublishDrawableMetrics(size);
        if (!_drawingLayout) RequestFrame();
    }

    void IMTKViewDelegate.Draw(MTKView view)
    {
        _ = view;
        var owner = _owner;
        var size = DrawableSize;
        if (owner is null || _releaseRequested || size.Width <= 0 || size.Height <= 0) return;
        if (_inFlight >= 3)
        {
            _frameBackpressure = true;
            return;
        }
        var drawable = CurrentDrawable;
        if (drawable?.Texture is null)
        {
            // The first AppKit invalidation can arrive before CAMetalLayer has
            // made a drawable available. Keep the request alive for the next
            // main-run-loop turn instead of consuming the framework wake.
            RequestFrame();
            return;
        }

        var generation = Interlocked.Read(ref _surfaceGeneration);
        MauiPaintCompletion? completion = null;
        var commandBufferTracked = false;
        AppKitPlatformViewHost.PreparedFrame? platformFrame = null;
        SkiaGraphiteSession.Frame? graphiteFrame = null;
        var graphiteSubmissionAttempted = false;
        var compositionTransaction = owner.PlatformViews is not null;
        if (compositionTransaction)
        {
            CATransaction.Begin();
            CATransaction.DisableActions = true;
            PresentsWithTransaction = true;
        }
        try
        {
            if (UseGraphite && _graphite is null)
            {
                _graphite = SkiaGraphiteSession.CreateMetal(_metalDevice.Handle, _commandQueue.Handle,
                    Interlocked.Increment(ref _contextGeneration));
            }
            if (!UseGraphite && _grContext is null)
            {
                _grContext = GRContext.CreateMetal(_backendContext) ??
                    throw new InvalidOperationException("Skia Metal GRContext creation failed.");
                Interlocked.Increment(ref _contextGeneration);
            }
            using var renderTarget = UseGraphite ? null : new GRBackendRenderTarget(
                checked((int)size.Width), checked((int)size.Height), new GRMtlTextureInfo(drawable.Texture));
            if (UseGraphite)
                graphiteFrame = _graphite!.BeginMetalFrame(checked((int)size.Width), checked((int)size.Height), drawable.Texture.Handle);
            using var ganeshSurface = UseGraphite ? null : SKSurface.Create(_grContext, renderTarget!,
                GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888)
                ?? throw new InvalidOperationException("Skia Metal SKSurface creation failed.");
            var surface = graphiteFrame?.Surface ?? ganeshSurface!;
            var scale = Window?.Screen?.BackingScaleFactor ?? NSScreen.MainScreen?.BackingScaleFactor ?? 1;
            _logicalWidth = Bounds.Width;
            _logicalHeight = Bounds.Height;
            _pixelWidth = size.Width;
            _pixelHeight = size.Height;
            _density = (double)scale;
            PublishDrawableMetrics(size);
            generation = Interlocked.Read(ref _surfaceGeneration);
            var paint = new MauiSkiaPaintContext(surface, (object?)_graphite ?? _grContext,
                checked((int)size.Width), checked((int)size.Height), (double)scale, generation,
                GetType().FullName ?? nameof(DorotiMacOSMetalView), GraphicsBackendId);
            completion = owner.RaisePaint(paint);
            platformFrame = owner.PlatformViews?.TakePending();
            if (paint.SkipPresent || generation != Interlocked.Read(ref _surfaceGeneration))
            {
                graphiteFrame?.CancelRecording();
                platformFrame?.Dispose();
                return;
            }
            platformFrame?.Submit();
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
            platformFrame?.Commit();
            var transactionPresentation = _drawingLayout || compositionTransaction;
            if (!transactionPresentation) commandBuffer.PresentDrawable(drawable);
            TrackCommandBuffer(commandBuffer, owner, completion, generation, graphiteFrame, drawable, platformFrame);
            commandBufferTracked = true;
            commandBuffer.Commit();
            var presentationFrame = platformFrame;
            graphiteFrame = null; // The committed buffer now owns retirement, including if Present fails.
            platformFrame = null;
            graphiteSubmissionAttempted = false;
            Interlocked.Increment(ref _commandBuffersCommitted);
            commandBufferTracked = false;
            if (transactionPresentation)
            {
                commandBuffer.WaitUntilScheduled();
                drawable.Present();
                presentationFrame?.Present();
            }
        }
        catch (Exception exception)
        {
            owner.PlatformViews?.CancelPending();
            try { platformFrame?.Abort(); }
            catch (Exception rollbackError) { System.Diagnostics.Trace.TraceError(rollbackError.ToString()); }
            if (commandBufferTracked) CancelCommandBufferTracking();
            if (graphiteFrame is not null && !graphiteSubmissionAttempted)
            {
                graphiteFrame.CancelRecording();
                graphiteFrame = null;
            }
            if (graphiteSubmissionAttempted || platformFrame?.HasSubmitted == true)
            {
                // Every segment uses this same queue. The terminal marker retains all
                // submitted textures and native leases, including partial failures.
                using var marker = _commandQueue.CommandBuffer();
                if (marker is not null)
                {
                    TrackCommandBuffer(marker, owner, null, generation, graphiteFrame, drawable, platformFrame);
                    marker.Commit();
                    platformFrame = null;
                    Interlocked.Increment(ref _commandBuffersCommitted);
                }
            }
            platformFrame?.Dispose();
            owner.RaiseFailure(exception, completion);
        }
        finally
        {
            if (compositionTransaction)
            {
                CATransaction.Commit();
                PresentsWithTransaction = _drawingLayout;
            }
        }
    }

    private void TrackCommandBuffer(IMTLCommandBuffer buffer, DorotiMacOSMetalSurface owner,
        MauiPaintCompletion? completion, long generation,
        SkiaGraphiteSession.Frame? graphiteFrame, ICAMetalDrawable drawable,
        AppKitPlatformViewHost.PreparedFrame? platformFrame = null)
    {
        buffer.AddCompletedHandler(completedBuffer =>
        {
            var status = completedBuffer.Status;
            var error = completedBuffer.Error?.LocalizedDescription;
            BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var stale = generation != Interlocked.Read(ref _surfaceGeneration) ||
                                !ReferenceEquals(owner, _owner);
                    if (stale) Interlocked.Increment(ref _staleCompletions);
                    if (status == MTLCommandBufferStatus.Completed)
                    {
                        Interlocked.Increment(ref _commandBuffersCompleted);
                        if (completion is { } value) owner.RaisePresent(value, stale);
                    }
                    else
                    {
                        Interlocked.Increment(ref _commandBuffersErrored);
                        owner.RaiseFailure(new InvalidOperationException(
                            error ?? status.ToString()),
                            completion);
                    }
                }
                finally
                {
                    platformFrame?.Dispose();
                    graphiteFrame?.CompleteGpuWork();
                    GC.KeepAlive(drawable);
                    lock (_resourceGate)
                    {
                        _inFlight--;
                        if (_releaseRequested && _inFlight == 0) ReleaseGpuResources();
                    }
                    if (_frameBackpressure && !_releaseRequested)
                    {
                        _frameBackpressure = false;
                        RequestFrame();
                    }
                }
            });
        });
        lock (_resourceGate) _inFlight++;
    }

    private void CancelCommandBufferTracking()
    {
        lock (_resourceGate)
        {
            _inFlight--;
            if (_releaseRequested && _inFlight == 0) ReleaseGpuResources();
        }
    }

    public override void MouseEntered(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.add, 0);
    public override void MouseExited(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.remove, 0);
    public override void MouseMoved(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.hover, Buttons());
    public override void MouseDown(NSEvent theEvent)
    {
        _controlClick = (theEvent.ModifierFlags & NSEventModifierMask.ControlKeyMask) != 0;
        DispatchPointer(theEvent, PointerChange.down, _controlClick ? (Buttons() & ~1) | 2 : Buttons() | 1);
    }
    public override void MouseDragged(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.move,
        _controlClick ? (Buttons() & ~1) | 2 : Buttons());
    public override void MouseUp(NSEvent theEvent)
    {
        DispatchPointer(theEvent, PointerChange.up, Buttons() & ~1);
        _controlClick = false;
    }
    public override void RightMouseDown(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.down, Buttons() | 2);
    public override void RightMouseDragged(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.move, Buttons());
    public override void RightMouseUp(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.up, Buttons() & ~2);
    public override void OtherMouseDown(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.down, Buttons() | 4);
    public override void OtherMouseDragged(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.move, Buttons());
    public override void OtherMouseUp(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.up, Buttons() & ~4);

    public override void ScrollWheel(NSEvent theEvent)
    {
        var scale = BackingScale();
        // AppKit reports precise trackpad deltas in points, but a discrete
        // mouse wheel reports lines. Match Flutter's macOS normalization so a
        // wheel notch is not reduced to an effectively invisible one pixel.
        var pixelsPerLine = theEvent.HasPreciseScrollingDeltas ? 1d : 40d;
        var deltaX = -theEvent.ScrollingDeltaX * pixelsPerLine * scale;
        var deltaY = -theEvent.ScrollingDeltaY * pixelsPerLine * scale;
        if ((theEvent.ModifierFlags & NSEventModifierMask.ShiftKeyMask) != 0)
            (deltaX, deltaY) = (deltaY, deltaX);
        DispatchPointer(theEvent, PointerChange.hover, Buttons(),
            deltaX, deltaY, PointerSignalKind.scroll);
    }

    public override void KeyDown(NSEvent theEvent) => DispatchKey(theEvent,
        theEvent.IsARepeat ? KeyEventType.repeat : KeyEventType.down);
    public override void KeyUp(NSEvent theEvent) => DispatchKey(theEvent, KeyEventType.up);

    public override void FlagsChanged(NSEvent theEvent)
    {
        var mask = MacOSKeyMap.ModifierMask(theEvent.KeyCode);
        if (mask == 0) { base.FlagsChanged(theEvent); return; }
        var physical = MacOSKeyMap.Physical(theEvent.KeyCode);
        var down = ((ulong)theEvent.ModifierFlags & mask) != 0;
        if (down == _pressedKeys.ContainsKey(physical)) return;
        EmitKey(TimeSpan.FromSeconds(theEvent.Timestamp), down ? KeyEventType.down : KeyEventType.up,
            physical, MauiKeyMap.Logical("", physical), null);
    }

    private void DispatchPointer(NSEvent native, PointerChange change, int buttons,
        double scrollX = 0, double scrollY = 0, PointerSignalKind signal = PointerSignalKind.none)
    {
        var point = ConvertPointFromView(native.LocationInWindow, null!);
        var scale = BackingScale();
        // NSEvent.Pressure is not valid for scroll-wheel events. Accessing it
        // aborts delivery before the PointerScrollEvent reaches the framework.
        var pressure = signal == PointerSignalKind.none ? native.Pressure : 0;
        _owner?.RaisePointer(new(TimeSpan.FromSeconds(native.Timestamp), change, PointerDeviceKind.mouse, 1,
            point.X * scale, (Bounds.Height - point.Y) * scale, buttons,
            scrollX, scrollY, signal, pressure));
    }

    private void DispatchKey(NSEvent native, KeyEventType type)
    {
        var characters = native.CharactersIgnoringModifiers ?? string.Empty;
        var physical = MacOSKeyMap.Physical(native.KeyCode);
        EmitKey(TimeSpan.FromSeconds(native.Timestamp), type, physical, MauiKeyMap.Logical(characters, physical),
            characters.Length == 1 && !char.IsControl(characters[0]) ? characters : null);
    }

    private void EmitKey(TimeSpan timestamp, KeyEventType type, long physical, long logical, string? character)
    {
        if (type == KeyEventType.up)
        {
            if (!_pressedKeys.Remove(physical, out logical)) return;
        }
        else
        {
            if (_pressedKeys.TryGetValue(physical, out var pressedLogical)) logical = pressedLogical;
            _pressedKeys[physical] = logical;
        }
        _owner?.RaiseKey(new(_owner.ViewId, timestamp, type, physical, logical, false,
            type == KeyEventType.up ? null : character));
    }

    private void ReleasePressedKeys()
    {
        foreach (var (physical, logical) in _pressedKeys)
            _owner?.RaiseKey(new(_owner.ViewId, TimeSpan.Zero, KeyEventType.up, physical, logical, true));
        _pressedKeys.Clear();
    }

    private double BackingScale() =>
        (double)(Window?.Screen?.BackingScaleFactor ?? NSScreen.MainScreen?.BackingScaleFactor ?? 1);

    private static int Buttons()
    {
        var pressed = (ulong)NSEvent.CurrentPressedMouseButtons;
        return ((pressed & 1) != 0 ? 1 : 0) |
               ((pressed & 2) != 0 ? 2 : 0) |
               ((pressed & 4) != 0 ? 4 : 0);
    }

    private void AttachWindowObservers()
    {
        DetachWindowObservers();
        if (Window is null) return;
        _windowBecameKeyObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            NSWindow.DidBecomeKeyNotification, _ => _owner?.RaiseFocus(true), Window);
        _windowResignedKeyObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            NSWindow.DidResignKeyNotification, _ => { ReleasePressedKeys(); _owner?.RaiseFocus(false); }, Window);
    }

    private void DetachWindowObservers()
    {
        if (_windowBecameKeyObserver is not null)
            NSNotificationCenter.DefaultCenter.RemoveObserver(_windowBecameKeyObserver);
        if (_windowResignedKeyObserver is not null)
            NSNotificationCenter.DefaultCenter.RemoveObserver(_windowResignedKeyObserver);
        _windowBecameKeyObserver?.Dispose();
        _windowResignedKeyObserver?.Dispose();
        _windowBecameKeyObserver = null;
        _windowResignedKeyObserver = null;
    }

    private void RestoreCursorVisibility()
    {
        if (!_cursorHidden) return;
        NSCursor.Unhide();
        _cursorHidden = false;
    }

    private void PublishDrawableMetrics(CGSize drawableSize, bool force = false)
    {
        var owner = _owner;
        if (owner is null || drawableSize.Width <= 0 || drawableSize.Height <= 0) return;
        var scale = BackingScale();
        var logicalSize = Bounds.Size;
        if (logicalSize.Width <= 0 || logicalSize.Height <= 0)
            logicalSize = new CGSize(drawableSize.Width / scale, drawableSize.Height / scale);
        if (!force && drawableSize.Equals(_lastDrawableSize) &&
            logicalSize.Equals(_lastLogicalSize) && scale.Equals(_lastBackingScale)) return;
        _lastDrawableSize = drawableSize;
        _lastLogicalSize = logicalSize;
        _lastBackingScale = scale;
        Interlocked.Increment(ref _surfaceGeneration);
        owner.RaiseSizeChanged(
            logicalSize.Width,
            logicalSize.Height,
            checked((int)drawableSize.Width),
            checked((int)drawableSize.Height),
            scale);
    }

    internal MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current) => current with
    {
        PixelWidth = checked((int)_pixelWidth),
        PixelHeight = checked((int)_pixelHeight),
        DevicePixelRatio = _density,
        ContextGeneration = Interlocked.Read(ref _contextGeneration),
        SurfaceGeneration = Interlocked.Read(ref _surfaceGeneration),
        NativeViewType = GetType().FullName ?? nameof(DorotiMacOSMetalView),
        GraphicsBackend = GraphicsBackendId,
        MetalDevice = _metalDevice.Name,
        PixelFormat = ColorPixelFormat.ToString(),
        CommandBuffersCommitted = Interlocked.Read(ref _commandBuffersCommitted),
        CommandBuffersCompleted = Interlocked.Read(ref _commandBuffersCompleted),
        CommandBuffersErrored = Interlocked.Read(ref _commandBuffersErrored),
        StaleCompletions = Interlocked.Read(ref _staleCompletions),
        CpuReadbacks = 0,
        FullFrameCopies = 0,
        LogicalWidth = _logicalWidth,
        LogicalHeight = _logicalHeight,
    };

    private static IMTLDevice RequireMetalDevice() => MTLDevice.SystemDefault ??
        throw new PlatformNotSupportedException("Doroti AppKit requires a Metal-capable device.");

    private void ReleaseGpuResources()
    {
        if (_resourcesReleased) return;
        _resourceOwner?.RaiseGpuResourcesReleasing();
        _resourceOwner = null;
        _graphite?.Dispose();
        _graphite = null;
        _grContext?.Dispose();
        _grContext = null;
        _backendContext.Dispose();
        _commandQueue.Dispose();
        _metalDevice.Dispose();
        _resourcesReleased = true;
    }
}
#endif
