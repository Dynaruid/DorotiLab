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
    void RequestFocus(bool focused);
    void SetCursor(DorotiMouseCursorKind cursor);
    MauiSurfaceSnapshot CaptureSnapshot(MauiSurfaceSnapshot current);
}

#if !MACOS && !WINDOWS
internal sealed class MauiSkglSurface : IMauiSkiaSurface
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
        _view = new SKGLView { HasRenderLoop = false, EnableTouchEvents = true };
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
#if WINDOWS
    public event Action<MauiSynchronousResize>? SynchronousResize;
#endif

    public void InvalidateSurface() => _view.InvalidateSurface();
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
            var density = Math.Max(1, Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density);
#if MACCATALYST || IOS || ANDROID
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
        private MacCatalystGestureDelegate? _gestureDelegate;

        internal MacCatalystNativeSubscription(
            SKGLView view,
            Action<MauiSurfacePointerData> dispatch)
        {
            _view = view;
            _dispatch = dispatch;
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
                AllowedScrollTypesMask = UIKit.UIScrollTypeMask.All,
                AllowedTouchTypes = [],
                CancelsTouchesInView = false,
                DelaysTouchesBegan = false,
                DelaysTouchesEnded = false,
            };
            _gestureDelegate = new MacCatalystGestureDelegate();
            _recognizer.Delegate = _gestureDelegate;
            nativeView.AddGestureRecognizer(_recognizer);
        }

        private void HandleScroll(UIKit.UIPanGestureRecognizer recognizer)
        {
            if (_nativeView is not { } nativeView) return;
            if (recognizer.State is not UIKit.UIGestureRecognizerState.Began and
                not UIKit.UIGestureRecognizerState.Changed and
                not UIKit.UIGestureRecognizerState.Ended) return;

            var translation = recognizer.TranslationInView(nativeView);
            recognizer.SetTranslation(CoreGraphics.CGPoint.Empty, nativeView);
            if (translation.X == 0 && translation.Y == 0) return;

            var location = recognizer.LocationInView(nativeView);
            var scale = Math.Max(1, (double)nativeView.ContentScaleFactor);
            _dispatch(new(
                DorotiFrameClock.Now,
                PointerChange.hover,
                PointerDeviceKind.mouse,
                1,
                location.X * scale,
                location.Y * scale,
                0,
                -translation.X * scale,
                -translation.Y * scale,
                PointerSignalKind.scroll,
                0));
        }

        private void DetachCurrent()
        {
            if (_nativeView is not null && _recognizer is not null)
                _nativeView.RemoveGestureRecognizer(_recognizer);
            _recognizer?.Dispose();
            _recognizer = null;
            _gestureDelegate?.Dispose();
            _gestureDelegate = null;
            _nativeView = null;
        }

        public void Dispose()
        {
            _view.HandlerChanged -= HandleHandlerChanged;
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
