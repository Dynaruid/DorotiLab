#if MACOS
using AppKit;
using CoreGraphics;
using Doroti.Ui;
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
    private GRContext? _grContext;
    private DorotiMacOSMetalSurface? _owner;
    private NSTrackingArea? _trackingArea;
    private NSObject? _windowBecameKeyObserver;
    private NSObject? _windowResignedKeyObserver;
    private CGSize _lastDrawableSize;
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
        AutoResizeDrawable = true;
        Paused = true;
        EnableSetNeedsDisplay = true;
        Delegate = this;
    }

    public override bool AcceptsFirstResponder() => true;

    internal void Connect(DorotiMacOSMetalSurface owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _releaseRequested = false;
        AttachWindowObservers();
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
        RequestFrame();
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
        if (size.Equals(_lastDrawableSize)) return;
        _lastDrawableSize = size;
        Interlocked.Increment(ref _surfaceGeneration);
        _owner?.RaiseSizeChanged();
    }

    void IMTKViewDelegate.Draw(MTKView view)
    {
        _ = view;
        var owner = _owner;
        var drawable = CurrentDrawable;
        var size = DrawableSize;
        if (owner is null || size.Width <= 0 || size.Height <= 0) return;
        if (drawable?.Texture is null)
        {
            owner.RaiseFailure(new InvalidOperationException("The AppKit MTKView has no current drawable."));
            return;
        }

        var generation = Interlocked.Read(ref _surfaceGeneration);
        MauiPaintCompletion? completion = null;
        var commandBufferTracked = false;
        try
        {
            if (_grContext is null)
            {
                _grContext = GRContext.CreateMetal(_backendContext) ??
                    throw new InvalidOperationException("Skia Metal GRContext creation failed.");
                Interlocked.Increment(ref _contextGeneration);
            }
            var textureInfo = new GRMtlTextureInfo(drawable.Texture);
            using var renderTarget = new GRBackendRenderTarget(
                checked((int)size.Width), checked((int)size.Height), textureInfo);
            using var surface = SKSurface.Create(_grContext, renderTarget,
                GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888) ??
                throw new InvalidOperationException("Skia Metal SKSurface creation failed.");
            var scale = Window?.Screen?.BackingScaleFactor ?? NSScreen.MainScreen?.BackingScaleFactor ?? 1;
            _logicalWidth = Bounds.Width;
            _logicalHeight = Bounds.Height;
            _pixelWidth = size.Width;
            _pixelHeight = size.Height;
            _density = (double)scale;
            var paint = new MauiSkiaPaintContext(surface, _grContext,
                checked((int)size.Width), checked((int)size.Height), (double)scale, generation,
                GetType().FullName ?? nameof(DorotiMacOSMetalView), "AppKit/MTKView/Metal-Skia");
            completion = owner.RaisePaint(paint);
            surface.Canvas.Flush();
            surface.Flush();
            _grContext.Flush();

            using var commandBuffer = _commandQueue.CommandBuffer() ??
                throw new InvalidOperationException("Metal command buffer creation failed.");
            commandBuffer.PresentDrawable(drawable);
            TrackCommandBuffer(commandBuffer, owner, completion, generation);
            commandBufferTracked = true;
            commandBuffer.Commit();
            Interlocked.Increment(ref _commandBuffersCommitted);
            commandBufferTracked = false;
        }
        catch (Exception exception)
        {
            if (commandBufferTracked) CancelCommandBufferTracking();
            owner.RaiseFailure(exception, completion);
        }
    }

    private void TrackCommandBuffer(IMTLCommandBuffer buffer, DorotiMacOSMetalSurface owner,
        MauiPaintCompletion? completion, long generation)
    {
        lock (_resourceGate) _inFlight++;
        buffer.AddCompletedHandler(completedBuffer =>
        {
            try
            {
                var stale = generation != Interlocked.Read(ref _surfaceGeneration) ||
                            !ReferenceEquals(owner, _owner);
                if (stale) Interlocked.Increment(ref _staleCompletions);
                if (completedBuffer.Status == MTLCommandBufferStatus.Completed)
                {
                    Interlocked.Increment(ref _commandBuffersCompleted);
                    if (completion is { } value) owner.RaisePresent(value, stale);
                }
                else
                {
                    Interlocked.Increment(ref _commandBuffersErrored);
                    owner.RaiseFailure(new InvalidOperationException(
                        completedBuffer.Error?.LocalizedDescription ?? completedBuffer.Status.ToString()),
                        completion);
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

    public override void MouseEntered(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.add, 0);
    public override void MouseExited(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.remove, 0);
    public override void MouseMoved(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.hover, Buttons());
    public override void MouseDown(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.down, Buttons() | 1);
    public override void MouseDragged(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.move, Buttons());
    public override void MouseUp(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.up, Buttons() & ~1);
    public override void RightMouseDown(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.down, Buttons() | 2);
    public override void RightMouseDragged(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.move, Buttons());
    public override void RightMouseUp(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.up, Buttons() & ~2);
    public override void OtherMouseDown(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.down, Buttons() | 4);
    public override void OtherMouseDragged(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.move, Buttons());
    public override void OtherMouseUp(NSEvent theEvent) => DispatchPointer(theEvent, PointerChange.up, Buttons() & ~4);

    public override void ScrollWheel(NSEvent theEvent)
    {
        var scale = BackingScale();
        DispatchPointer(theEvent, PointerChange.hover, Buttons(),
            -theEvent.ScrollingDeltaX * scale, -theEvent.ScrollingDeltaY * scale, PointerSignalKind.scroll);
    }

    public override void KeyDown(NSEvent theEvent) => DispatchKey(theEvent,
        theEvent.IsARepeat ? KeyEventType.repeat : KeyEventType.down);
    public override void KeyUp(NSEvent theEvent) => DispatchKey(theEvent, KeyEventType.up);

    private void DispatchPointer(NSEvent native, PointerChange change, int buttons,
        double scrollX = 0, double scrollY = 0, PointerSignalKind signal = PointerSignalKind.none)
    {
        var point = ConvertPointFromView(native.LocationInWindow, null!);
        var scale = BackingScale();
        _owner?.RaisePointer(new(TimeSpan.FromSeconds(native.Timestamp), change, PointerDeviceKind.mouse, 1,
            point.X * scale, (Bounds.Height - point.Y) * scale, buttons,
            scrollX, scrollY, signal, native.Pressure));
    }

    private void DispatchKey(NSEvent native, KeyEventType type)
    {
        var characters = native.CharactersIgnoringModifiers ?? string.Empty;
        var physical = PhysicalKey(native.KeyCode);
        _owner?.RaiseKey(new(_owner.ViewId, TimeSpan.FromSeconds(native.Timestamp), type,
            physical, LogicalKey(characters, physical), false,
            characters.Length == 1 && !char.IsControl(characters[0]) ? characters : null));
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

    private static long PhysicalKey(ushort keyCode) => keyCode switch
    {
        0x24 => 0x70028, 0x35 => 0x70029, 0x33 => 0x7002a, 0x30 => 0x7002b,
        0x31 => 0x7002c, 0x75 => 0x7004c, 0x7c => 0x7004f, 0x7b => 0x70050,
        0x7d => 0x70051, 0x7e => 0x70052,
        _ => 0x100000000 | keyCode,
    };

    private static long LogicalKey(string key, long physical)
    {
        if (key.Length == 1 && !char.IsControl(key[0])) return char.ToLowerInvariant(key[0]);
        return physical switch
        {
            0x70028 => 0x10000000d, 0x70029 => 0x10000001b, 0x7002a => 0x100000008,
            0x7002b => 0x100000009, 0x7004c => 0x10000007f, 0x7004f => 0x100000303,
            0x70050 => 0x100000302, 0x70051 => 0x100000301, 0x70052 => 0x100000304,
            _ => physical == 0 ? 0 : 0x100000000 | physical,
        };
    }

    private void AttachWindowObservers()
    {
        DetachWindowObservers();
        if (Window is null) return;
        _windowBecameKeyObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            NSWindow.DidBecomeKeyNotification, _ => _owner?.RaiseFocus(true), Window);
        _windowResignedKeyObserver = NSNotificationCenter.DefaultCenter.AddObserver(
            NSWindow.DidResignKeyNotification, _ => _owner?.RaiseFocus(false), Window);
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

    internal MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current) => current with
    {
        PixelWidth = checked((int)_pixelWidth),
        PixelHeight = checked((int)_pixelHeight),
        DevicePixelRatio = _density,
        ContextGeneration = Interlocked.Read(ref _contextGeneration),
        SurfaceGeneration = Interlocked.Read(ref _surfaceGeneration),
        NativeViewType = GetType().FullName ?? nameof(DorotiMacOSMetalView),
        GraphicsBackend = "AppKit/MTKView/Metal-Skia",
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
        _resourcesReleased = true;
        _grContext?.Dispose();
        _grContext = null;
        _backendContext.Dispose();
        _commandQueue.Dispose();
        _metalDevice.Dispose();
    }
}
#endif
