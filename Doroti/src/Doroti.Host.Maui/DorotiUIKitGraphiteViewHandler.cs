#if IOS || MACCATALYST
using CoreGraphics;
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
    private CGSize _lastSize;
    private long _generation;
    private bool _drawing;

    public DorotiUIKitGraphiteView() : base(CGRect.Empty, MTLDevice.SystemDefault ??
        throw new PlatformNotSupportedException("Doroti Graphite requires a Metal-capable device."))
    {
        _queue = Device!.CreateCommandQueue() ?? throw new InvalidOperationException("Metal queue creation failed.");
        ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
        FramebufferOnly = false;
        AutoResizeDrawable = true;
        Paused = true;
        EnableSetNeedsDisplay = true;
        Opaque = false;
        BackgroundColor = UIColor.Clear;
        MultipleTouchEnabled = true;
        Delegate = this;
    }

    internal void Connect(DorotiGraphiteView owner) { _owner = owner; SetNeedsDisplay(); }
    internal void Disconnect()
    {
        // Draw completion is drained on the owner thread before it returns.
        _owner?.ReleaseGraphiteResources();
        _session?.Dispose(); _session = null; _owner = null;
    }

    public override void MovedToWindow()
    {
        base.MovedToWindow();
        if (Window is not null) SetNeedsDisplay();
    }
    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        if (Window is null || Bounds.Size.Equals(_lastSize)) return;
        _lastSize = Bounds.Size;
        DrawableSize = new CGSize(Math.Max(1, Bounds.Width * ContentScaleFactor), Math.Max(1, Bounds.Height * ContentScaleFactor));
        SetNeedsDisplay();
    }
    public void DrawableSizeWillChange(MTKView view, CGSize size) { _generation++; }
    public void Draw(MTKView view)
    {
        if (_drawing || Window is null || _owner is null) return;
        _drawing = true;
        SkiaGraphiteSession.Frame? frame = null;
        MauiSkiaPaintContext? paint = null;
        var submitted = false;
        try
        {
            using var drawable = CurrentDrawable;
            if (drawable is null) { SetNeedsDisplay(); return; }
            _session ??= SkiaGraphiteSession.CreateMetal(Device!.Handle, _queue.Handle, Math.Max(1, ++_generation), 1);
            var width = checked((int)drawable.Texture.Width);
            var height = checked((int)drawable.Texture.Height);
            frame = _session.BeginMetalFrame(width, height, drawable.Texture.Handle);
            frame.Surface.Canvas.Clear(SKColors.Transparent);
            paint = new(frame.Surface, _session, width, height, Math.Max(1, (double)ContentScaleFactor),
                _generation, GetType().FullName!, "UIKit/MTKView/Graphite-Metal");
            _owner.PaintGraphite(paint);
            if (paint.SkipPresent)
            {
                frame.CancelRecording(); frame = null;
                if (paint.Completion is { } stale) _owner.CompleteGraphite(stale, true);
                return;
            }
            submitted = true;
            frame.Submit();
            using var command = _queue.CommandBuffer() ?? throw new InvalidOperationException("Metal present buffer creation failed.");
            command.PresentDrawable(drawable);
            command.Commit();
            command.WaitUntilCompleted();
            frame.CompleteGpuWork(); frame = null;
            if (command.Status != MTLCommandBufferStatus.Completed)
                throw new InvalidOperationException($"Metal presentation failed: {command.Status}: {command.Error?.LocalizedDescription}");
            if (paint.Completion is { } completion) _owner.CompleteGraphite(completion);
        }
        catch (Exception exception) { _owner?.FailGraphite(paint?.Completion, exception); }
        finally
        {
            if (frame is not null)
            {
                if (!submitted) frame.CancelRecording();
                else
                {
                    using var marker = _queue.CommandBuffer() ?? throw new InvalidOperationException("Metal shutdown marker unavailable.");
                    marker.Commit(); marker.WaitUntilCompleted(); frame.CompleteGpuWork();
                }
            }
            _drawing = false;
        }
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
        if (disposing) { Disconnect(); Delegate = null; _queue.Dispose(); }
        base.Dispose(disposing);
    }
}
#endif
