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
#if WINDOWS
    private readonly WindowsResizeContinuityGuard _resizeContinuity;
#endif
    private bool _disposed;

    internal MauiSkglSurface(MauiTextInputBridge textInput, ulong viewId)
    {
        _view = new SKGLView { HasRenderLoop = false, EnableTouchEvents = true };
        _nativeInput = MauiNativeInput.Attach(_view, textInput, viewId, data => Key?.Invoke(data));
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

    private void HandleSizeChanged(object? sender, EventArgs args) => SizeChanged?.Invoke(null);
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
        _nativeInput.Dispose();
    }
}
#endif
