#if IOS || MACCATALYST
using CoreGraphics;
using CoreAnimation;
using Doroti.Skia.Rendering;
using Foundation;
using Metal;
using MetalKit;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using UIKit;

namespace Doroti.Host.Maui;

public sealed class DorotiUIKitGraphiteViewHandler : ViewHandler<DorotiGraphiteView, DorotiUIKitGraphiteView>
{
    private static readonly CommandMapper<DorotiGraphiteView, DorotiUIKitGraphiteViewHandler> Commands =
        new(ViewCommandMapper) { [nameof(ISKGLView.InvalidateSurface)] = (handler, _, _) => handler.PlatformView.SetNeedsDisplay() };
    public DorotiUIKitGraphiteViewHandler() : base(ViewMapper, Commands) { }
    protected override DorotiUIKitGraphiteView CreatePlatformView() => new();
    protected override void ConnectHandler(DorotiUIKitGraphiteView platformView)
    { base.ConnectHandler(platformView); platformView.Connect(VirtualView); }
    protected override void DisconnectHandler(DorotiUIKitGraphiteView platformView)
    { platformView.Disconnect(); base.DisconnectHandler(platformView); }
}

/// <summary>UIKit owns input/layout; this view owns the Metal queue and Graphite recorder.</summary>
public sealed class DorotiUIKitGraphiteView : MTKView, IMTKViewDelegate
{
    private readonly IMTLCommandQueue _queue;
    private SkiaGraphiteSession? _session;
    private DorotiGraphiteView? _owner;
    // All mutations, including retirement, run on the UIKit/recorder owner thread.
    // A failed terminal-marker submission must keep its native resources rooted.
    private static readonly HashSet<DorotiUIKitGraphiteView> RetiringViews = [];
    // Native NSObject hashes can change when their handles are disposed.
    // Retirement identity must remain stable until the pending entry is removed.
    private readonly HashSet<PendingFrame> _pending = new(ReferenceEqualityComparer.Instance);
    private DorotiGraphiteView? _resourceOwner;
    private bool _releaseRequested;
    private bool _resourcesReleased;
    private bool _faulted;
    private bool _frameBackpressure;
    private nfloat _lastScale;
    private CGSize _lastSize;
    private long _generation;
    private bool _drawing;

    public DorotiUIKitGraphiteView() : base(CGRect.Empty, MTLDevice.SystemDefault ??
        throw new PlatformNotSupportedException("Doroti Graphite requires a Metal-capable device."))
    {
        _queue = Device!.CreateCommandQueue() ?? throw new InvalidOperationException("Metal queue creation failed.");
        ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
        FramebufferOnly = false;
        AutoResizeDrawable = false;
        ContentMode = UIViewContentMode.Redraw;
        Layer.ContentsGravity = CALayer.GravityTopLeft;
        Layer.MasksToBounds = true;
        Paused = true;
        EnableSetNeedsDisplay = true;
        Opaque = false;
        BackgroundColor = UIColor.Clear;
        MultipleTouchEnabled = true;
        Delegate = this;
    }

    internal void Connect(DorotiGraphiteView owner)
    {
        if (_releaseRequested || _resourcesReleased)
            throw new InvalidOperationException("A disconnected Metal view requires a fresh handler/native view.");
        if (RetiringViews.Count != 0)
            throw new InvalidOperationException("Previous UIKit Metal resources are still retiring.");
        _resourceOwner = _owner = owner;
        SetNeedsDisplay();
    }
    internal void Disconnect()
    {
        if (_releaseRequested) return;
        _releaseRequested = true;
        _generation++;
        _owner = null;
        _session?.StopAcceptingFrames();
        RetiringViews.Add(this);
        if (!_drawing && _pending.Count == 0) ReleaseGpuResources();
    }

    public override void MovedToWindow()
    {
        base.MovedToWindow();
        if (!_releaseRequested && Window is not null) SetNeedsDisplay();
    }
    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        if (_releaseRequested || Window is null || Bounds.Width <= 0 || Bounds.Height <= 0 ||
            (Bounds.Size.Equals(_lastSize) && ContentScaleFactor == _lastScale)) return;
        _lastSize = Bounds.Size;
        _lastScale = ContentScaleFactor;
        CATransaction.Begin();
        try
        {
            CATransaction.DisableActions = true;
            Layer.ContentsGravity = CALayer.GravityTopLeft;
            Layer.ContentsScale = ContentScaleFactor;
            DrawableSize = new CGSize(Math.Max(1, Math.Round(Bounds.Width * ContentScaleFactor)),
                Math.Max(1, Math.Round(Bounds.Height * ContentScaleFactor)));
            Draw();
        }
        finally { CATransaction.Commit(); }
        SetNeedsDisplay();
    }
    public void DrawableSizeWillChange(MTKView view, CGSize size) { _generation++; }
    public void Draw(MTKView view)
    {
        var owner = _owner;
        if (_drawing || _releaseRequested || _faulted || Window is null || owner is null) return;
        if (_pending.Count >= 3)
        {
            _frameBackpressure = true;
            return;
        }
        _drawing = true;
        SkiaGraphiteSession.Frame? frame = null;
        ICAMetalDrawable? drawable = null;
        MauiSkiaPaintContext? paint = null;
        var submitted = false;
        try
        {
            drawable = CurrentDrawable;
            if (drawable is null) { SetNeedsDisplay(); return; }
            _session ??= SkiaGraphiteSession.CreateMetal(Device!.Handle, _queue.Handle, Math.Max(1, ++_generation));
            var width = checked((int)drawable.Texture.Width);
            var height = checked((int)drawable.Texture.Height);
            frame = _session.BeginMetalFrame(width, height, drawable.Texture.Handle);
            frame.Surface.Canvas.Clear(SKColors.Transparent);
            var generation = _generation;
            paint = new(frame.Surface, _session, width, height, Math.Max(1, (double)ContentScaleFactor),
                generation, GetType().FullName!, "UIKit/MTKView/Graphite-Metal");
            owner.PaintGraphite(paint);
            if (paint.SkipPresent || _releaseRequested || generation != _generation)
            {
                frame.CancelRecording(); frame = null;
                if (paint.Completion is { } stale) owner.CompleteGraphite(stale, true);
                return;
            }
            submitted = true;
            frame.Submit();
            // Transfer ownership before attempting the terminal marker. Even a
            // failed commit must retain textures; it is not GPU completion.
            var pending = new PendingFrame(frame, drawable, owner, paint.Completion, generation);
            _pending.Add(pending);
            frame = null;
            drawable = null;
            CommitTerminal(pending, present: true);
        }
        catch (Exception exception)
        {
            _faulted = true;
            _session?.StopAcceptingFrames();
            if (frame is not null && submitted)
            {
                var pending = new PendingFrame(frame, drawable!, owner, null, _generation);
                _pending.Add(pending);
                frame = null;
                drawable = null;
                try { CommitTerminal(pending, present: false); }
                catch (Exception markerError) { System.Diagnostics.Trace.TraceError(markerError.ToString()); }
            }
            owner.FailGraphite(paint?.Completion, exception);
        }
        finally
        {
            frame?.CancelRecording();
            drawable?.Dispose();
            _drawing = false;
            if (_releaseRequested && _pending.Count == 0) ReleaseGpuResources();
        }
    }

    private sealed record PendingFrame(SkiaGraphiteSession.Frame Frame, ICAMetalDrawable Drawable,
        DorotiGraphiteView Owner, MauiPaintCompletion? Completion, long Generation);

    private void CommitTerminal(PendingFrame pending, bool present)
    {
        try
        {
            using var command = _queue.CommandBuffer() ??
                throw new InvalidOperationException("Metal terminal buffer creation failed; retaining GPU resources.");
            if (present) command.PresentDrawable(pending.Drawable);
            command.AddCompletedHandler(completed =>
            {
                var status = completed.Status;
                var error = completed.Error?.LocalizedDescription;
                // Dispatch via the application, since the native view may have
                // been disposed while its borrowed drawable remains in flight.
                UIApplication.SharedApplication.BeginInvokeOnMainThread(() => Retire(pending, status, error));
            });
            command.Commit();
        }
        catch
        {
            _faulted = true;
            RetiringViews.Add(this);
            throw;
        }
    }

    private void Retire(PendingFrame pending, MTLCommandBufferStatus status, string? error)
    {
        try
        {
            // An empty later marker reporting Error does not prove earlier
            // queue work completed. Hold unless the context confirms loss.
            if (status != MTLCommandBufferStatus.Completed && _session?.IsDeviceLost != true)
                throw new InvalidOperationException($"Metal terminal failed; retaining GPU resources: {status}: {error}");
            pending.Frame.CompleteGpuWork();
            pending.Drawable.Dispose();
            _pending.Remove(pending);
            if (status != MTLCommandBufferStatus.Completed)
                throw new InvalidOperationException($"Metal presentation failed: {status}: {error}");
            if (pending.Completion is { } completion)
                pending.Owner.CompleteGraphite(completion,
                    _releaseRequested || pending.Generation != _generation || !ReferenceEquals(_owner, pending.Owner));
        }
        catch (Exception exception)
        {
            _faulted = true;
            _session?.StopAcceptingFrames();
            RetiringViews.Add(this);
            pending.Owner.FailGraphite(pending.Completion, exception);
        }
        finally
        {
            if (_releaseRequested && _pending.Count == 0) ReleaseGpuResources();
            if (_frameBackpressure && !_releaseRequested && !_faulted)
            {
                _frameBackpressure = false;
                SetNeedsDisplay();
            }
        }
    }

    private void ReleaseGpuResources()
    {
        if (_resourcesReleased) return;
        _resourceOwner?.ReleaseGraphiteResources();
        _resourceOwner = null;
        _session?.Dispose();
        _session = null;
        _queue.Dispose();
        _resourcesReleased = true;
        RetiringViews.Remove(this);
    }

    private void DispatchTouches(NSSet touches, SKTouchAction action, bool contact)
    {
        if (_owner?.EnableTouchEvents != true) return;
        foreach (UITouch touch in touches.Cast<UITouch>())
        {
            var point = touch.LocationInView(this);
            var args = new SKTouchEventArgs(((IntPtr)touch.Handle).ToInt64(), action,
                new SKPoint((float)(point.X * ContentScaleFactor), (float)(point.Y * ContentScaleFactor)), contact);
            ((ISKGLView)_owner).OnTouch(args);
        }
    }
    public override void TouchesBegan(NSSet touches, UIEvent? evt) { base.TouchesBegan(touches, evt); DispatchTouches(touches, SKTouchAction.Pressed, true); }
    public override void TouchesMoved(NSSet touches, UIEvent? evt) { base.TouchesMoved(touches, evt); DispatchTouches(touches, SKTouchAction.Moved, true); }
    public override void TouchesEnded(NSSet touches, UIEvent? evt) { base.TouchesEnded(touches, evt); DispatchTouches(touches, SKTouchAction.Released, false); }
    public override void TouchesCancelled(NSSet touches, UIEvent? evt) { base.TouchesCancelled(touches, evt); DispatchTouches(touches, SKTouchAction.Cancelled, false); }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { Disconnect(); Delegate = null; }
        base.Dispose(disposing);
    }
}
#endif
