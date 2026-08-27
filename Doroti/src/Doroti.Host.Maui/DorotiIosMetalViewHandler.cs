#if IOS && !MACCATALYST
using CoreAnimation;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using SkiaSharp.Views.Maui;
using UIKit;

namespace Doroti.Host.Maui;

/// <summary>
/// iOS MAUI handler that keeps Doroti on Metal. SkiaSharp's stock iOS
/// SKGLViewHandler still creates its deprecated OpenGL SKGLView; only its
/// Mac Catalyst handler creates an SKMetalView.
/// </summary>
public sealed class DorotiIosMetalViewHandler : ViewHandler<ISKGLView, SKMetalView>
{
    private static readonly PropertyMapper<ISKGLView, DorotiIosMetalViewHandler> Mapper =
        new(ViewHandler.ViewMapper)
        {
            [nameof(ISKGLView.EnableTouchEvents)] = MapEnableTouchEvents,
            [nameof(ISKGLView.IgnorePixelScaling)] = MapIgnorePixelScaling,
            [nameof(ISKGLView.HasRenderLoop)] = MapHasRenderLoop,
        };

    private static readonly CommandMapper<ISKGLView, DorotiIosMetalViewHandler> Commands =
        new(ViewHandler.ViewCommandMapper)
        {
            [nameof(ISKGLView.InvalidateSurface)] = InvalidateSurface,
        };

    private DorotiIosTouchRecognizer? _touchRecognizer;
    private CADisplayLink? _pendingDisplayLink;
    private SKSizeI _lastCanvasSize;
    private GRContext? _lastContext;

    public DorotiIosMetalViewHandler() : base(Mapper, Commands) { }

    protected override SKMetalView CreatePlatformView() => new DorotiIosMetalView
    {
        BackgroundColor = UIColor.Clear,
        Opaque = false,
    };

    protected override void ConnectHandler(SKMetalView platformView)
    {
        platformView.PaintSurface += HandlePaintSurface;
        _touchRecognizer = new(HandleTouch, ScaleTouchPoint);
        base.ConnectHandler(platformView);
        platformView.Paused = !VirtualView.HasRenderLoop;
        platformView.EnableSetNeedsDisplay = !VirtualView.HasRenderLoop;
        platformView.SetNeedsDisplay();
    }

    protected override void DisconnectHandler(SKMetalView platformView)
    {
        _pendingDisplayLink?.Invalidate();
        _pendingDisplayLink?.Dispose();
        _pendingDisplayLink = null;
        platformView.PaintSurface -= HandlePaintSurface;
        if (_touchRecognizer is not null)
        {
            platformView.RemoveGestureRecognizer(_touchRecognizer);
            _touchRecognizer.Dispose();
            _touchRecognizer = null;
        }
        _lastCanvasSize = default;
        _lastContext = null;
        base.DisconnectHandler(platformView);
    }

    private static void InvalidateSurface(
        DorotiIosMetalViewHandler handler,
        ISKGLView view,
        object? args)
    {
        _ = view;
        _ = args;
        handler.RequestFrame();
    }

    private void RequestFrame()
    {
        if (_pendingDisplayLink is not null ||
            PlatformView is not { Paused: true, EnableSetNeedsDisplay: true }) return;

        _pendingDisplayLink = CADisplayLink.Create(() =>
        {
            var displayLink = _pendingDisplayLink;
            _pendingDisplayLink = null;
            displayLink?.Invalidate();
            displayLink?.Dispose();
            if (PlatformView is { Paused: true, EnableSetNeedsDisplay: true } platformView)
                platformView.SetNeedsDisplay();
        });
        _pendingDisplayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
    }

    private static void MapHasRenderLoop(DorotiIosMetalViewHandler handler, ISKGLView view)
    {
        if (handler.PlatformView is not { } platformView) return;
        platformView.Paused = !view.HasRenderLoop;
        platformView.EnableSetNeedsDisplay = !view.HasRenderLoop;
    }

    private static void MapIgnorePixelScaling(DorotiIosMetalViewHandler handler, ISKGLView view)
    {
        if (handler.PlatformView is not DorotiIosMetalView platformView) return;
        platformView.IgnorePixelScaling = view.IgnorePixelScaling;
        platformView.SetNeedsDisplay();
    }

    private static void MapEnableTouchEvents(DorotiIosMetalViewHandler handler, ISKGLView view)
    {
        if (handler.PlatformView is not { } platformView || handler._touchRecognizer is null) return;
        var attached = platformView.GestureRecognizers?.Contains(handler._touchRecognizer) == true;
        if (view.EnableTouchEvents && !attached)
            platformView.AddGestureRecognizer(handler._touchRecognizer);
        else if (!view.EnableTouchEvents && attached)
            platformView.RemoveGestureRecognizer(handler._touchRecognizer);
    }

    private void HandlePaintSurface(object? sender, SKPaintMetalSurfaceEventArgs args)
    {
        if (VirtualView is not { } view) return;
        if (_lastCanvasSize != args.Info.Size)
        {
            _lastCanvasSize = args.Info.Size;
            view.OnCanvasSizeChanged(_lastCanvasSize);
        }
        if (sender is SKMetalView metalView && !ReferenceEquals(_lastContext, metalView.GRContext))
        {
            _lastContext = metalView.GRContext;
            view.OnGRContextChanged(_lastContext);
        }
        view.OnPaintSurface(new SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs(
            args.Surface, args.BackendRenderTarget, args.Origin, args.Info, args.RawInfo));
    }

    private void HandleTouch(SKTouchEventArgs args) => VirtualView?.OnTouch(args);

    private SKPoint ScaleTouchPoint(double x, double y)
    {
        if (VirtualView?.IgnorePixelScaling != true && PlatformView is { } platformView)
        {
            x *= platformView.ContentScaleFactor;
            y *= platformView.ContentScaleFactor;
        }
        return new((float)x, (float)y);
    }

    private sealed class DorotiIosMetalView : SKMetalView
    {
        private CoreGraphics.CGSize _lastLayoutSize;

        internal bool IgnorePixelScaling { get; set; }

        public override void MovedToWindow()
        {
            base.MovedToWindow();
            if (Window is not null) SetNeedsDisplay();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (Window is null || Bounds.Size.Equals(_lastLayoutSize)) return;
            _lastLayoutSize = Bounds.Size;
            SetNeedsDisplay();
        }

        protected override void OnPaintSurface(SKPaintMetalSurfaceEventArgs args)
        {
            if (IgnorePixelScaling)
            {
                var logicalSize = new SKSizeI((int)Bounds.Width, (int)Bounds.Height);
                args.Surface.Canvas.Scale((float)ContentScaleFactor);
                args.Surface.Canvas.Save();
                args = new(args.Surface, args.BackendRenderTarget, args.Origin,
                    args.Info.WithSize(logicalSize), args.Info);
            }
            base.OnPaintSurface(args);
        }
    }

    private sealed class DorotiIosTouchRecognizer(
        Action<SKTouchEventArgs> dispatch,
        Func<double, double, SKPoint> scale) : UIGestureRecognizer
    {
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            DispatchTouches(touches, evt, SKTouchAction.Pressed, true, ignoreUnhandled: true);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            DispatchTouches(touches, evt, SKTouchAction.Moved, true, ignoreUnhandled: false);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            DispatchTouches(touches, evt, SKTouchAction.Released, false, ignoreUnhandled: false);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            DispatchTouches(touches, evt, SKTouchAction.Cancelled, false, ignoreUnhandled: false);
        }

        private void DispatchTouches(
            NSSet touches,
            UIEvent evt,
            SKTouchAction action,
            bool inContact,
            bool ignoreUnhandled)
        {
            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                var location = touch.LocationInView(View);
                var args = new SKTouchEventArgs(
                    ((IntPtr)touch.Handle).ToInt64(), action,
                    scale(location.X, location.Y), inContact);
                dispatch(args);
                if (ignoreUnhandled && !args.Handled) IgnoreTouch(touch, evt);
            }
        }
    }
}
#endif
