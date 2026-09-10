#if IOS && !MACCATALYST
using SKGLView = Doroti.Host.Maui.DorotiSkiaView;
#endif
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using SkiaSharp;
#if !MACOS
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
#endif

namespace Doroti.Host.Maui;

internal sealed class MauiSkiaPaintContext(
    SKSurface surface,
    object? contextIdentity,
    int pixelWidth,
    int pixelHeight,
    double density,
    long surfaceGeneration,
    string nativeViewType,
    string graphicsBackend)
{
    internal SKSurface Surface { get; } = surface;
    internal object? ContextIdentity { get; } = contextIdentity;
    internal int PixelWidth { get; } = pixelWidth;
    internal int PixelHeight { get; } = pixelHeight;
    internal double Density { get; } = density;
    internal long SurfaceGeneration { get; } = surfaceGeneration;
    internal string NativeViewType { get; } = nativeViewType;
    internal string GraphicsBackend { get; } = graphicsBackend;
    internal bool SkipRaster { get; set; }
    internal bool SkipPresent { get; set; }
    internal MauiPaintCompletion? Completion { get; set; }
}

internal readonly record struct MauiSurfacePointerData(
    TimeSpan Timestamp,
    PointerChange Change,
    PointerDeviceKind Kind,
    ulong Pointer,
    double X,
    double Y,
    int Buttons,
    double ScrollDeltaX,
    double ScrollDeltaY,
    PointerSignalKind SignalKind,
    double Pressure);

/// <summary>
/// Small platform surface boundary shared by the SKGLView and AppKit Metal paths.
/// Native view and command-buffer ownership stay behind this contract.
/// </summary>
internal interface IMauiSkiaSurface : IDisposable
{
    View Element { get; }
    IDispatcher Dispatcher { get; }
    double Width { get; }
    double Height { get; }
    DorotiResizeEpoch? ResizeTarget => null;
    event Action<MauiSkiaPaintContext>? Paint;
    event Action<MauiPaintCompletion, bool>? PresentCompleted;
    event Action<MauiPaintCompletion?, Exception>? PaintFailed;
    event Action<MauiSurfacePointerData>? Pointer;
    event Action<KeyData>? Key;
    event Action<bool>? FocusChanged;
    event Action<DorotiResizeEpoch?>? SizeChanged;
    void InvalidateSurface();
    // The caller already owns the current display pulse. Platforms may draw
    // immediately instead of scheduling a second pulse.
    void InvalidateSurfaceFromVsync() => InvalidateSurface();
    void RequestFocus(bool focused);
    void SetCursor(DorotiMouseCursorKind cursor);
    MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current);
}

internal interface IMauiGraphiteSurface
{
    event Action? GpuResourcesReleasing;
}

#if !MACOS && !WINDOWS
internal sealed class MauiSkglSurface : IMauiSkiaSurface, IMauiGraphiteSurface
#if WINDOWS
    , IMauiSynchronousResizeSurface
#endif
{
    private readonly SKGLView _view;
    private readonly IDisposable _nativeInput;
#if MACCATALYST || IOS || ANDROID
    private readonly DorotiResizeTargetCoordinator _resizeTargets = new();
#endif
#if MACCATALYST
    private static readonly TimeSpan MacCatalystResizeQuiescence = TimeSpan.FromMilliseconds(150);
    private readonly MacCatalystNativeSubscription _macCatalystNative;
    private long _macCatalystResizePulse;
#endif
#if WINDOWS
    private readonly WindowsResizeContinuityGuard _resizeContinuity;
#endif
    private bool _disposed;

    internal MauiSkglSurface(MauiTextInputBridge textInput, ulong viewId)
    {
        _view = DorotiGraphiteView.Enabled ? new DorotiGraphiteView() : new SKGLView();
        _view.HasRenderLoop = false;
        _view.EnableTouchEvents = true;
        if (_view is DorotiGraphiteView graphite)
        {
            graphite.GraphitePaint += HandleGraphitePaint;
            graphite.GraphitePresentCompleted += HandleGraphiteCompleted;
            graphite.GraphiteFailed += HandleGraphiteFailed;
            graphite.GpuResourcesReleasing += HandleGraphiteRelease;
        }
#if MACCATALYST
        // The UIKit SKTouchHandler discards device kind and UIEvent.ButtonMask.
        // Own this stream so secondary clicks are not converted to primary taps.
        _view.EnableTouchEvents = false;
#endif
        _nativeInput = MauiNativeInput.Attach(_view, textInput, viewId, data => Key?.Invoke(data));
#if MACCATALYST
        _macCatalystNative = new(_view, data => Pointer?.Invoke(data));
#endif
#if WINDOWS
        _resizeContinuity = new(_view, PrepareSynchronousResize, CompleteSynchronousPresent);
#endif
        _view.PaintSurface += HandlePaintSurface;
        _view.Touch += HandleTouch;
        _view.SizeChanged += HandleSizeChanged;
        _view.Focused += HandleFocused;
        _view.Unfocused += HandleUnfocused;
    }

    public View Element => _view;
    public IDispatcher Dispatcher => _view.Dispatcher;
    public double Width => _view.Width;
    public double Height => _view.Height;
#if MACCATALYST || IOS || ANDROID
    public DorotiResizeEpoch? ResizeTarget => _resizeTargets.Latest;
#endif
    public event Action<MauiSkiaPaintContext>? Paint;
    public event Action<MauiPaintCompletion, bool>? PresentCompleted;
    public event Action<MauiPaintCompletion?, Exception>? PaintFailed;
    public event Action<MauiSurfacePointerData>? Pointer;
    public event Action<KeyData>? Key;
    public event Action<bool>? FocusChanged;
    public event Action<DorotiResizeEpoch?>? SizeChanged;
    public event Action? GpuResourcesReleasing;
    private void HandleGraphiteRelease() => GpuResourcesReleasing?.Invoke();
    private void HandleGraphiteCompleted(MauiPaintCompletion completion, bool replay) => PresentCompleted?.Invoke(completion, replay);
    private void HandleGraphiteFailed(MauiPaintCompletion? completion, Exception exception) => PaintFailed?.Invoke(completion, exception);
    private void HandleGraphitePaint(MauiSkiaPaintContext context)
    {
        PublishDrawableMetrics(context.PixelWidth, context.PixelHeight, context.Density);
        Paint?.Invoke(context);
    }
#if WINDOWS
    public event Action<MauiSynchronousResize>? SynchronousResize;
#endif

    public void InvalidateSurface() => _view.InvalidateSurface();
    public void InvalidateSurfaceFromVsync()
    {
#if ANDROID
        if (_view.Handler is DorotiAndroidVulkanViewHandler graphite)
        {
            graphite.PlatformView.DrawFromVsync();
            return;
        }
#endif
        InvalidateSurface();
    }
    public void RequestFocus(bool focused)
    {
        if (focused) _view.Focus();
        else _view.Unfocus();
    }
    public void SetCursor(DorotiMouseCursorKind cursor) => MauiNativeInput.SetCursor(_view, cursor);
    public MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current)
    {
#if WINDOWS
        return _resizeContinuity.CaptureSnapshot(current);
#else
        return current;
#endif
    }

    private void HandlePaintSurface(object? sender, SKPaintGLSurfaceEventArgs args)
    {
        _ = sender;
        if (args.Surface is null || _view.GRContext is null)
        {
            PaintFailed?.Invoke(null, new InvalidOperationException(
                "Strict Doroti MAUI mode requires a GPU-backed SKSurface and GRContext."));
            return;
        }
        try
        {
            var nativeType = _view.Handler?.PlatformView?.GetType().FullName ?? "unknown";
            var density = MauiViewEnvironment.ValidScale(Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density);
#if MACCATALYST || IOS
            // DeviceDisplay describes the main monitor, which need not own
            // this window. Match the scale used by the Metal drawable and
            // SKTouchHandler's conversion from UIKit points to pixels.
            if (_view.Handler?.PlatformView is UIKit.UIView nativeView)
                density = MauiViewEnvironment.ValidScale((double)nativeView.ContentScaleFactor);
#endif
#if MACCATALYST || IOS || ANDROID
#if ANDROID
            if (_view.Handler?.PlatformView is Android.Views.View androidView)
                density = MauiViewEnvironment.ValidScale(androidView.Resources?.DisplayMetrics?.Density ?? density);
#endif
            PublishDrawableMetrics(
                args.BackendRenderTarget.Width, args.BackendRenderTarget.Height, density);
#endif
#if WINDOWS
            _resizeContinuity.ObserveCurrentEgl(
                args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
#endif
            var context = new MauiSkiaPaintContext(args.Surface, _view.GRContext,
                args.BackendRenderTarget.Width, args.BackendRenderTarget.Height, density, 0,
                nativeType,
#if WINDOWS
                "WinUI3/SKSwapChainPanel/ANGLE-DirectX-Skia"
#elif MACCATALYST
                "UIKit-MacCatalyst/SKMetalView/Metal-Skia"
#elif IOS
                "UIKit-iOS/SKMetalView/Metal-Skia"
#elif ANDROID
                "Android/MauiSKGLTextureView/OpenGL-ES-Skia"
#endif
            );
            var rasterStarted = DorotiFrameClock.Now;
#if WINDOWS
            _resizeContinuity.RecordRasterStart(
                args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
#endif
            Paint?.Invoke(context);
#if WINDOWS
            _resizeContinuity.RecordRasterEnd(
                args.BackendRenderTarget.Width, args.BackendRenderTarget.Height,
                DorotiFrameClock.Now - rasterStarted);
#endif
            if (context.Completion is not { } completion) return;
#if WINDOWS
            if (!_resizeContinuity.CaptureSynchronousCompletion(completion))
                Dispatcher.DispatchDelayed(TimeSpan.Zero, () => PresentCompleted?.Invoke(completion, false));
#else
            PresentCompleted?.Invoke(completion, false);
#endif
        }
        catch (Exception exception)
        {
            PaintFailed?.Invoke(null, exception);
        }
    }

    private void HandleTouch(object? sender, SKTouchEventArgs args)
    {
        _ = sender;
        var change = args.ActionType switch
        {
            SKTouchAction.Pressed => PointerChange.down,
            SKTouchAction.Released => PointerChange.up,
            SKTouchAction.Cancelled => PointerChange.cancel,
            SKTouchAction.Entered => PointerChange.add,
            SKTouchAction.Exited => PointerChange.remove,
            SKTouchAction.WheelChanged => PointerChange.hover,
            _ => PointerChange.move,
        };
        var buttons = args.InContact ? args.MouseButton switch
        {
            SKMouseButton.Right => 2,
            SKMouseButton.Middle => 4,
            _ => 1,
        } : 0;
        var kind = args.DeviceType switch
        {
            SKTouchDeviceType.Mouse => PointerDeviceKind.mouse,
            SKTouchDeviceType.Pen => PointerDeviceKind.stylus,
            _ => PointerDeviceKind.touch,
        };
        Pointer?.Invoke(new(DorotiFrameClock.Now, change, kind,
            checked((ulong)Math.Max(0, args.Id)), args.Location.X, args.Location.Y, buttons,
            0, args.ActionType == SKTouchAction.WheelChanged ? -args.WheelDelta : 0,
            args.ActionType == SKTouchAction.WheelChanged ? PointerSignalKind.scroll : PointerSignalKind.none,
            args.Pressure));
        args.Handled = true;
    }

    private void HandleSizeChanged(object? sender, EventArgs args)
    {
        _ = sender;
        _ = args;
#if MACCATALYST
        // UIKit layout and MTKView's drawable resize are separate callbacks.
        // Do not publish an estimated physical size from the layout callback:
        // the next Metal paint publishes the exact drawable dimensions first.
        ScheduleMacCatalystResizeCompletion();
#elif IOS || ANDROID
        // The GPU drawable is the physical-size authority. Layout only requests
        // a paint; that paint publishes exact pixels and density as one epoch.
        _view.InvalidateSurface();
#else
        SizeChanged?.Invoke(null);
#endif
    }

#if MACCATALYST
    private void ScheduleMacCatalystResizeCompletion()
    {
        var pulse = checked(++_macCatalystResizePulse);
        // DorotiMacCatalystMetalView draws synchronously from LayoutSubviews.
        // Do not also start MTKView's display-link loop: its independent paint
        // can present between the window-origin and drawable-size commits when
        // the bottom edge is moving, producing a one-frame vertical jump.
        Dispatcher.DispatchDelayed(MacCatalystResizeQuiescence, () =>
        {
            if (_disposed || pulse != _macCatalystResizePulse) return;
            // Retain one final invalidation after live resize so a drawable
            // that was temporarily unavailable is retried at the settled size.
            _view.InvalidateSurface();
        });
    }

#endif

#if MACCATALYST || IOS || ANDROID
    private void PublishDrawableMetrics(int pixelWidth, int pixelHeight, double density)
    {
        if (pixelWidth <= 0 || pixelHeight <= 0) return;
        var logicalWidth = pixelWidth / density;
        var logicalHeight = pixelHeight / density;
        var previousGeneration = _resizeTargets.Latest?.Generation;
        var target = _resizeTargets.Publish(logicalWidth, logicalHeight, density);
        if (target.Generation != previousGeneration) SizeChanged?.Invoke(target);
    }
#endif

    private void HandleFocused(object? sender, FocusEventArgs args) => FocusChanged?.Invoke(true);
    private void HandleUnfocused(object? sender, FocusEventArgs args) => FocusChanged?.Invoke(false);

#if WINDOWS
    private void PrepareSynchronousResize(MauiSynchronousResize resize) =>
        SynchronousResize?.Invoke(resize);

    private void CompleteSynchronousPresent(MauiPaintCompletion completion) =>
        PresentCompleted?.Invoke(completion, false);

    public void RecordResizePhase(string phase, DorotiResizeEpoch epoch, TimeSpan? duration = null,
        string? terminal = null, string? detail = null) =>
        _resizeContinuity.Record(phase, epoch, "maui-host-adapter", duration,
            terminal: terminal, detail: detail);
#endif

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_view is DorotiGraphiteView graphite)
        {
            // Native GPU owner releases while renderer callbacks are still attached.
            _view.Handler?.DisconnectHandler();
            graphite.GraphitePaint -= HandleGraphitePaint;
            graphite.GraphitePresentCompleted -= HandleGraphiteCompleted;
            graphite.GraphiteFailed -= HandleGraphiteFailed;
            graphite.GpuResourcesReleasing -= HandleGraphiteRelease;
        }
        _view.PaintSurface -= HandlePaintSurface;
        _view.Touch -= HandleTouch;
        _view.SizeChanged -= HandleSizeChanged;
        _view.Focused -= HandleFocused;
        _view.Unfocused -= HandleUnfocused;
#if WINDOWS
        _resizeContinuity.Dispose();
#endif
#if MACCATALYST
        _macCatalystNative.Dispose();
#endif
        _nativeInput.Dispose();
    }

#if MACCATALYST
    /// <summary>
    /// SKTouchHandler on UIKit forwards direct touches only. Mac Catalyst wheel
    /// and trackpad scrolling arrive through a pan recognizer whose allowed
    /// scroll types explicitly include indirect continuous and discrete input.
    /// </summary>
    private sealed class MacCatalystNativeSubscription : IDisposable
    {
        private readonly SKGLView _view;
        private readonly Action<MauiSurfacePointerData> _dispatch;
        private UIKit.UIView? _nativeView;
        private UIKit.UIPanGestureRecognizer? _recognizer;
        private UIKit.UIPanGestureRecognizer? _wheelRecognizer;
        private MacCatalystPointerRecognizer? _pointerRecognizer;
        private UIKit.UIContextMenuInteraction? _contextMenuInteraction;
        private MacCatalystContextMenuDelegate? _contextMenuDelegate;
        private UIKit.UIHoverGestureRecognizer? _hoverRecognizer;
        private MacCatalystGestureDelegate? _gestureDelegate;
        private readonly MauiScrollMomentum _momentum = new();
        private CoreAnimation.CADisplayLink? _displayLink;
        private CoreGraphics.CGPoint _scrollLocation;
        private int _mouseButtons;
        private readonly Foundation.NSObject _deactivationObserver;

        internal MacCatalystNativeSubscription(
            SKGLView view,
            Action<MauiSurfacePointerData> dispatch)
        {
            _view = view;
            _dispatch = dispatch;
            _deactivationObserver = UIKit.UIApplication.Notifications.ObserveWillResignActive((_, _) => StopMomentum());
            _view.HandlerChanged += HandleHandlerChanged;
            AttachCurrent();
        }

        private void HandleHandlerChanged(object? sender, EventArgs args)
        {
            _ = sender;
            _ = args;
            AttachCurrent();
        }

        private void AttachCurrent()
        {
            DetachCurrent();
            if (_view.Handler?.PlatformView is not UIKit.UIView nativeView) return;

            _nativeView = nativeView;
            // UIKit's default scale-to-fill behavior stretches the last Metal
            // drawable between live-resize callbacks. Redraw keeps MTKView in
            // charge of producing content for each new bounds value.
            nativeView.ContentMode = UIKit.UIViewContentMode.Redraw;
            if (nativeView is MetalKit.MTKView metalView)
            {
                // DorotiMacCatalystMetalView updates DrawableSize atomically
                // with LayoutSubviews. Letting MTKView also resize it later
                // reintroduces a one-frame stretched raster.
                metalView.AutoResizeDrawable = false;
                metalView.PreferredFramesPerSecond = Math.Max(
                    60, UIKit.UIScreen.MainScreen.MaximumFramesPerSecond);
            }
            _recognizer = new UIKit.UIPanGestureRecognizer(HandleScroll)
            {
                AllowedScrollTypesMask = UIKit.UIScrollTypeMask.Continuous,
                AllowedTouchTypes = [],
                CancelsTouchesInView = false,
                DelaysTouchesBegan = false,
                DelaysTouchesEnded = false,
            };
            _gestureDelegate = new MacCatalystGestureDelegate();
            _recognizer.Delegate = _gestureDelegate;
            nativeView.AddGestureRecognizer(_recognizer);
            _wheelRecognizer = new UIKit.UIPanGestureRecognizer(HandleScroll)
            {
                AllowedScrollTypesMask = UIKit.UIScrollTypeMask.Discrete,
                AllowedTouchTypes = [],
                CancelsTouchesInView = false,
                Delegate = _gestureDelegate,
            };
            nativeView.AddGestureRecognizer(_wheelRecognizer);
            _pointerRecognizer = new MacCatalystPointerRecognizer(data =>
            {
                if (data.Change == PointerChange.down) StopMomentum();
                if (data.Kind == PointerDeviceKind.mouse) _mouseButtons = data.Buttons;
                _dispatch(data);
            }) { Delegate = _gestureDelegate };
            nativeView.AddGestureRecognizer(_pointerRecognizer);
            // In the Mac idiom, UIKit routes secondary clicks through the
            // context-menu interaction rather than the raw primary touch stream.
            // Doroti paints the menu, so decline UIKit's native menu after
            // forwarding the context-click position to the framework.
            _contextMenuDelegate = new MacCatalystContextMenuDelegate(location =>
            {
                StopMomentum();
                var scale = MauiViewEnvironment.ValidScale((double)nativeView.ContentScaleFactor);
                var down = new MauiSurfacePointerData(DorotiFrameClock.Now, PointerChange.down,
                    PointerDeviceKind.mouse, 1, location.X * scale, location.Y * scale,
                    2, 0, 0, PointerSignalKind.none, 0);
                _dispatch(down);
                _dispatch(down with { Change = PointerChange.up, Buttons = 0 });
                if (Environment.GetEnvironmentVariable("DOROTI_TRACE_MAC_INPUT") == "1")
                    Console.WriteLine("[MacCatalyst pointer] context click: down buttons=2, up buttons=0");
            });
            _contextMenuInteraction = new UIKit.UIContextMenuInteraction(_contextMenuDelegate);
            nativeView.AddInteraction(_contextMenuInteraction);
            _hoverRecognizer = new UIKit.UIHoverGestureRecognizer(recognizer =>
            {
                // Hover callbacks during a drag must not clear the pressed mask.
                if (_mouseButtons != 0) return;
                var location = recognizer.LocationInView(nativeView);
                var scale = MauiViewEnvironment.ValidScale((double)nativeView.ContentScaleFactor);
                var change = recognizer.State switch
                {
                    UIKit.UIGestureRecognizerState.Began => PointerChange.add,
                    UIKit.UIGestureRecognizerState.Ended => PointerChange.remove,
                    _ => PointerChange.hover,
                };
                _dispatch(new(DorotiFrameClock.Now, change, PointerDeviceKind.mouse, 1,
                    location.X * scale, location.Y * scale, 0, 0, 0, PointerSignalKind.none, 0));
            }) { Delegate = _gestureDelegate };
            nativeView.AddGestureRecognizer(_hoverRecognizer);
        }

        private void HandleScroll(UIKit.UIPanGestureRecognizer recognizer)
        {
            if (_nativeView is not { } nativeView) return;
            if (recognizer.State is UIKit.UIGestureRecognizerState.Began or
                UIKit.UIGestureRecognizerState.Cancelled or UIKit.UIGestureRecognizerState.Failed)
                StopMomentum();
            if (recognizer.State is not UIKit.UIGestureRecognizerState.Began and
                not UIKit.UIGestureRecognizerState.Changed and
                not UIKit.UIGestureRecognizerState.Ended) return;

            var translation = recognizer.TranslationInView(nativeView);
            recognizer.SetTranslation(CoreGraphics.CGPoint.Empty, nativeView);
            _scrollLocation = recognizer.LocationInView(nativeView);
            DispatchScroll(-translation.X, -translation.Y);
            if (recognizer.State == UIKit.UIGestureRecognizerState.Ended && ReferenceEquals(recognizer, _recognizer))
            {
                var velocity = recognizer.VelocityInView(nativeView);
                if (_momentum.Start(-velocity.X, -velocity.Y, DorotiFrameClock.Now))
                {
                    _displayLink = CoreAnimation.CADisplayLink.Create(() =>
                    {
                        if (_nativeView?.Window is null) { StopMomentum(); return; }
                        var delta = _momentum.Advance(DorotiFrameClock.Now);
                        DispatchScroll(delta.X, delta.Y);
                        if (!_momentum.IsActive) StopMomentum();
                    });
                    _displayLink.AddToRunLoop(Foundation.NSRunLoop.Main, Foundation.NSRunLoopMode.Common);
                }
            }
        }

        private void DispatchScroll(double x, double y)
        {
            if (_nativeView is not { } nativeView || (x == 0 && y == 0)) return;
            var scale = MauiViewEnvironment.ValidScale((double)nativeView.ContentScaleFactor);
            _dispatch(new(
                DorotiFrameClock.Now,
                PointerChange.hover,
                PointerDeviceKind.mouse,
                1,
                _scrollLocation.X * scale,
                _scrollLocation.Y * scale,
                0,
                x * scale,
                y * scale,
                PointerSignalKind.scroll,
                0));
        }

        private void StopMomentum()
        {
            _momentum.Stop();
            _displayLink?.Invalidate();
            _displayLink?.Dispose();
            _displayLink = null;
        }

        private void DetachCurrent()
        {
            StopMomentum();
            _mouseButtons = 0;
            foreach (var gesture in new UIKit.UIGestureRecognizer?[] { _wheelRecognizer, _pointerRecognizer, _hoverRecognizer })
            {
                if (gesture is null) continue;
                _nativeView?.RemoveGestureRecognizer(gesture);
                gesture.Dispose();
            }
            _wheelRecognizer = null;
            _pointerRecognizer = null;
            if (_contextMenuInteraction is not null) _nativeView?.RemoveInteraction(_contextMenuInteraction);
            _contextMenuInteraction?.Dispose();
            _contextMenuInteraction = null;
            _contextMenuDelegate?.Dispose();
            _contextMenuDelegate = null;
            _hoverRecognizer = null;
            if (_nativeView is not null && _recognizer is not null)
                _nativeView.RemoveGestureRecognizer(_recognizer);
            _recognizer?.Dispose();
            _recognizer = null;
            _gestureDelegate?.Dispose();
            _gestureDelegate = null;
            _nativeView = null;
        }

        private sealed class MacCatalystContextMenuDelegate(Action<CoreGraphics.CGPoint> show)
            : UIKit.UIContextMenuInteractionDelegate
        {
            public override UIKit.UIContextMenuConfiguration? GetConfigurationForMenu(
                UIKit.UIContextMenuInteraction interaction, CoreGraphics.CGPoint location)
            {
                show(location);
                return null;
            }
        }

        private sealed class MacCatalystPointerRecognizer : UIKit.UIGestureRecognizer
        {
            private readonly Action<MauiSurfacePointerData> _dispatch;
            private static readonly bool TraceInput = Environment.GetEnvironmentVariable("DOROTI_TRACE_MAC_INPUT") == "1";
            private readonly HashSet<nint> _mouseTouches = [];
            private readonly HashSet<nint> _secondaryTouches = [];
            private bool _controlClick;

            internal MacCatalystPointerRecognizer(Action<MauiSurfacePointerData> dispatch)
            {
                _dispatch = dispatch;
                CancelsTouchesInView = false;
                DelaysTouchesBegan = false;
                DelaysTouchesEnded = false;
            }

            public override void TouchesBegan(Foundation.NSSet touches, UIKit.UIEvent evt)
            {
                base.TouchesBegan(touches, evt);
                Send(touches, evt, PointerChange.down);
            }
            public override void TouchesMoved(Foundation.NSSet touches, UIKit.UIEvent evt)
            {
                base.TouchesMoved(touches, evt);
                Send(touches, evt, PointerChange.move);
            }
            public override void TouchesEnded(Foundation.NSSet touches, UIKit.UIEvent evt)
            {
                base.TouchesEnded(touches, evt);
                Send(touches, evt, PointerChange.up);
            }
            public override void TouchesCancelled(Foundation.NSSet touches, UIKit.UIEvent evt)
            {
                base.TouchesCancelled(touches, evt);
                Send(touches, evt, PointerChange.cancel);
            }

            private void Send(Foundation.NSSet touches, UIKit.UIEvent evt, PointerChange change)
            {
                if (View is not { } view) return;
                var scale = MauiViewEnvironment.ValidScale((double)view.ContentScaleFactor);
                foreach (var touch in touches.Cast<UIKit.UITouch>())
                {
                    var handle = (nint)touch.Handle;
                    // The context-menu interaction owns this sequence even on
                    // UIKit versions that also expose it as raw touches.
                    if (change == PointerChange.down && (evt.ButtonMask & UIKit.UIEventButtonMask.Secondary) != 0)
                        _secondaryTouches.Add(handle);
                    if (_secondaryTouches.Contains(handle))
                    {
                        if (change is PointerChange.up or PointerChange.cancel) _secondaryTouches.Remove(handle);
                        continue;
                    }
                    var mouse = touch.Type == UIKit.UITouchType.IndirectPointer || evt.ButtonMask != 0 || _mouseTouches.Contains(handle);
                    if (mouse && change == PointerChange.down) _mouseTouches.Add(handle);
                    var kind = mouse ? PointerDeviceKind.mouse : touch.Type == UIKit.UITouchType.Stylus
                        ? PointerDeviceKind.stylus : PointerDeviceKind.touch;
                    var buttons = mouse ? (int)evt.ButtonMask : 1;
                    if (change == PointerChange.down)
                        _controlClick = mouse && (buttons & 1) != 0 && (evt.ModifierFlags & UIKit.UIKeyModifierFlags.Control) != 0;
                    if (_controlClick) buttons = (buttons & ~1) | 2;
                    if (change is PointerChange.up or PointerChange.cancel) buttons = 0;
                    var location = touch.LocationInView(view);
                    if (TraceInput) Console.WriteLine($"[MacCatalyst pointer] {change} native={touch.Type} mask={evt.ButtonMask} kind={kind} buttons={buttons}");
                    _dispatch(new(TimeSpan.FromSeconds(touch.Timestamp), change, kind,
                        mouse ? 1UL : unchecked((ulong)(nint)touch.Handle), location.X * scale, location.Y * scale,
                        buttons, 0, 0, PointerSignalKind.none, (double)touch.Force));
                    if (change is PointerChange.up or PointerChange.cancel)
                    {
                        _controlClick = false;
                        _mouseTouches.Remove(handle);
                    }
                }
            }
        }

        public void Dispose()
        {
            _view.HandlerChanged -= HandleHandlerChanged;
            _deactivationObserver.Dispose();
            DetachCurrent();
        }

        private sealed class MacCatalystGestureDelegate : UIKit.UIGestureRecognizerDelegate
        {
            public override bool ShouldRecognizeSimultaneously(
                UIKit.UIGestureRecognizer gestureRecognizer,
                UIKit.UIGestureRecognizer otherGestureRecognizer)
            {
                _ = gestureRecognizer;
                _ = otherGestureRecognizer;
                return true;
            }
        }
    }
#endif
}
#endif
