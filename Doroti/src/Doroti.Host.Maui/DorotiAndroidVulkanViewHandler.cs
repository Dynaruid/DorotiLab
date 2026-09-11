#if ANDROID
using System.Runtime.InteropServices;
using System.Diagnostics;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Doroti.Skia.Vulkan;
using Doroti.Ui;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Doroti.Host.Maui;

public sealed class DorotiAndroidVulkanViewHandler : ViewHandler<DorotiGraphiteView, DorotiAndroidVulkanView>
{
    private static readonly CommandMapper<DorotiGraphiteView, DorotiAndroidVulkanViewHandler> Commands =
        new(ViewCommandMapper) { [nameof(ISKGLView.InvalidateSurface)] = (handler, _, _) => handler.PlatformView.RequestFrame() };
    public DorotiAndroidVulkanViewHandler() : base(ViewMapper, Commands) { }
    protected override DorotiAndroidVulkanView CreatePlatformView() => new(Context);
    protected override void ConnectHandler(DorotiAndroidVulkanView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Connect(VirtualView);
    }
    protected override void DisconnectHandler(DorotiAndroidVulkanView platformView)
    {
        platformView.Disconnect();
        base.DisconnectHandler(platformView);
    }
}

public sealed class DorotiAndroidVulkanView : SurfaceView, ISurfaceHolderCallback
{
    private DorotiGraphiteView? _owner;
    private GraphiteVulkanWindow? _window;
    private nint _nativeWindow;
    private int _width, _height;
    private bool _pending, _live;
    private long _generation;
    private readonly Java.Lang.Runnable _drawCallback;
    private readonly Java.Lang.Runnable _gpuCompletionCallback;
    private readonly Java.Lang.Runnable _allocationProfileCallback;
    private bool _gpuCompletionPending;
    private int _timingFrames;
    private readonly double[] _timingSums = new double[7];
    private readonly double[] _timingMaxima = new double[7];
    private bool _inputTiming;
    private double _density = 1;
    internal double SemanticsDensity => _density;
    internal AndroidX.CustomView.Widget.ExploreByTouchHelper? SemanticsHelper { get; set; }

    protected override bool DispatchHoverEvent(MotionEvent? e) =>
        (e is not null && SemanticsHelper?.DispatchHoverEvent(e) == true) || base.DispatchHoverEvent(e);

    public DorotiAndroidVulkanView(Context context) : base(context)
    {
        _drawCallback = new(DrawFrame);
        _gpuCompletionCallback = new(PollGpuCompletion);
        _allocationProfileCallback = new(() =>
        {
            foreach (var entry in FrameworkWorkProfile.CaptureEntries())
                global::Android.Util.Log.Info("DorotiAllocation", $"kind={entry.Kind} calls={entry.Calls} allocated={entry.AllocatedBytes} self={entry.SelfAllocatedBytes} type={entry.Type}");
        });
        // SurfaceView is composed below the MAUI semantics and IME overlay.
        Holder!.SetFormat(Format.Translucent);
        Focusable = FocusableInTouchMode = true;
    }

    internal void Connect(DorotiGraphiteView owner)
    {
        _owner = owner;
        RefreshDensity();
        _inputTiming = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?
            .Intent?.GetStringExtra("DOROTI_INPUT_TIMING") == "1";
        Holder!.AddCallback(this);
        _live = Holder.Surface?.IsValid == true;
        if (_live && Holder.SurfaceFrame is { } bounds) { _width = bounds.Width(); _height = bounds.Height(); }
        RequestFrame();
    }
    internal void Disconnect()
    {
        Holder?.RemoveCallback(this);
        _live = false;
        ReleaseSurface(); _owner = null;
    }
    public void SurfaceCreated(ISurfaceHolder holder) { _live = true; RequestFrame(); }
    public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
    { _width = width; _height = height; RefreshDensity(); RequestFrame(); }
    public void SurfaceDestroyed(ISurfaceHolder holder) { _live = false; ReleaseSurface(); }

    protected override void OnConfigurationChanged(Android.Content.Res.Configuration? newConfig)
    {
        base.OnConfigurationChanged(newConfig);
        RefreshDensity();
        RequestFrame();
    }

    private void RefreshDensity() =>
        _density = MauiViewEnvironment.ValidScale(Resources?.DisplayMetrics?.Density ?? 1);

    internal void RequestFrame()
    {
        if (_pending || !_live || _owner is null || _width <= 0 || _height <= 0) return;
        _pending = true;
        PostOnAnimation(_drawCallback);
    }

    internal void DrawFromVsync()
    {
        if (!_live || _owner is null || _width <= 0 || _height <= 0) return;
        // MauiHostAdapter has already waited for Choreographer. Posting another
        // animation callback here would defer this frame by an entire refresh.
        // Consume any ordinary invalidation too, so it cannot replay next pulse.
        if (_pending) RemoveCallbacks(_drawCallback);
        DrawFrame();
    }

    private void DrawFrame()
    {
        using var allocationProfile = FrameworkWorkProfile.AllocationEnabled ? FrameworkWorkProfile.Begin(GetType(), 10) : default;
        _pending = false;
        if (!_live || _owner is null) return;
        MauiSkiaPaintContext? paint = null;
        try
        {
            if (_window is null)
            {
                if (!OperatingSystem.IsAndroidVersionAtLeast(24))
                    throw new PlatformNotSupportedException("Doroti Graphite requires Android API 24 or newer and a hardware Vulkan 1.1 device.");
                _nativeWindow = ANativeWindowFromSurface(JNIEnv.Handle, Holder!.Surface!.Handle);
                if (_nativeWindow == 0) throw new InvalidOperationException("ANativeWindow_fromSurface failed.");
                _window = GraphiteVulkanWindow.CreateAndroid(_nativeWindow);
                _window.EnableFrameTiming = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?
                    .Intent?.GetStringExtra("DOROTI_MAUI_EVIDENCE") == "1" || _inputTiming;
                _window.ResourcesReleasing += ReleaseRendererResources;
                _generation++;
                global::Android.Util.Log.Info("DorotiGraphite", $"Vulkan device={_window.DeviceName} generation={_generation}");
            }
            var presented = _window.Render(_width, _height, (surface, width, height) =>
            {
                paint = new(surface, _window.ContextIdentity, width, height, _density,
                    (_generation << 32) | _window.Generation, GetType().FullName!, "Android/SurfaceView/Graphite-Vulkan");
                _owner.PaintGraphite(paint);
            }, () => paint is { SkipPresent: false, SkipRaster: false });
            if (paint?.Completion is { } completion) _owner.CompleteGraphite(completion, !presented);
            if (presented && _window.EnableFrameTiming) RecordFrameTiming(_window.LastFrameTiming);
            if (presented && _inputTiming && _window.LastFrameTiming.TotalMs > 12)
                global::Android.Util.Log.Info("DorotiInputTiming", $"frame={_window.LastFrameTiming}");
            ScheduleGpuCompletion();
            if (!presented || paint?.Completion is null) RequestFrame();
        }
        catch (Exception exception)
        {
            _owner.FailGraphite(paint?.Completion, exception);
            global::Android.Util.Log.Error("DorotiGraphite", exception.ToString());
            if (_window?.IsDeviceLost == true) { ReleaseSurface(); RequestFrame(); }
            else if (_window is null && _nativeWindow != 0) { ANativeWindowRelease(_nativeWindow); _nativeWindow = 0; }
        }
    }

    private void ReleaseRendererResources() => _owner?.ReleaseGraphiteResources();
    private void ScheduleGpuCompletion()
    {
        if (_gpuCompletionPending || !_live || _window?.WindowFramesInFlight is not > 0) return;
        _gpuCompletionPending = true;
        PostOnAnimation(_gpuCompletionCallback);
    }

    private void PollGpuCompletion()
    {
        _gpuCompletionPending = false;
        if (!_live || _window is null) return;
        try
        {
            // Retire the final frame even if the framework becomes idle. This
            // callback polls ownership only; it must not repaint an idle scene.
            _window.PollGpuWork();
            ScheduleGpuCompletion();
        }
        catch (Exception exception)
        {
            _owner?.FailGraphite(null, exception);
            global::Android.Util.Log.Error("DorotiGraphite", exception.ToString());
            if (_window.IsDeviceLost) { ReleaseSurface(); RequestFrame(); }
        }
    }
    private void RecordFrameTiming(VulkanWindowFrameTiming timing)
    {
        ReadOnlySpan<double> values = [timing.AcquireMs, timing.PaintMs, timing.SubmitMs,
            timing.CopyMs, timing.FenceMs, timing.PresentMs, timing.TotalMs];
        for (var i = 0; i < values.Length; i++)
        {
            _timingSums[i] += values[i];
            _timingMaxima[i] = Math.Max(_timingMaxima[i], values[i]);
        }
        if (++_timingFrames != 60) return;
        var means = string.Join(",", _timingSums.Select(value => (value / _timingFrames).ToString("F3", System.Globalization.CultureInfo.InvariantCulture)));
        var maxima = string.Join(",", _timingMaxima.Select(value => value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)));
        global::Android.Util.Log.Info("DorotiFrameTiming", $"frames={_timingFrames} phases=acquire,paint,submit,copy,fence,present,total meanMs={means} maxMs={maxima} submitted={_window?.SubmittedWindowFrames} completed={_window?.CompletedWindowFrames} busy={_window?.BusyWindowFrames} unavailable={_window?.UnavailableWindowImages} maxInFlight={_window?.MaximumWindowFramesInFlight}");
        _timingFrames = 0;
        Array.Clear(_timingSums); Array.Clear(_timingMaxima);
    }
    private void ReleaseSurface()
    {
        RemoveCallbacks(_drawCallback);
        RemoveCallbacks(_gpuCompletionCallback);
        _gpuCompletionPending = false;
        _pending = false;
        // SurfaceHolder callbacks and PostOnAnimation execute on this view's UI
        // thread. Drain before Android can destroy the underlying native surface.
        _window?.Dispose();
        if (_window?.EnableFrameTiming == true)
            global::Android.Util.Log.Info("DorotiFrameTiming", $"drained submitted={_window.SubmittedWindowFrames} completed={_window.CompletedWindowFrames} outstanding={_window.WindowFramesInFlight}");
        _window = null;
        if (_nativeWindow != 0) ANativeWindowRelease(_nativeWindow);
        _nativeWindow = 0;
    }

    public override bool OnTouchEvent(MotionEvent? e)
    {
        using var allocationProfile = FrameworkWorkProfile.AllocationEnabled ? FrameworkWorkProfile.Begin(GetType(), 9) : default;
        if (e is null || _owner?.EnableTouchEvents != true) return false;
        var started = _inputTiming ? Stopwatch.GetTimestamp() : 0;
        if (e.ActionMasked == MotionEventActions.Down) RequestFocus();
        var action = e.ActionMasked switch {
            MotionEventActions.Down or MotionEventActions.PointerDown => SKTouchAction.Pressed,
            MotionEventActions.Up or MotionEventActions.PointerUp => SKTouchAction.Released,
            MotionEventActions.Cancel => SKTouchAction.Cancelled, _ => SKTouchAction.Moved };
        var all = e.ActionMasked is MotionEventActions.Move or MotionEventActions.Cancel;
        for (var i = 0; i < e.PointerCount; i++)
        {
            if (!all && i != e.ActionIndex) continue;
            var contact = action is SKTouchAction.Pressed or SKTouchAction.Moved;
            var device = e.GetToolType(i) switch {
                MotionEventToolType.Mouse => SKTouchDeviceType.Mouse,
                MotionEventToolType.Stylus or MotionEventToolType.Eraser => SKTouchDeviceType.Pen,
                _ => SKTouchDeviceType.Touch };
            var button = (e.ButtonState & MotionEventButtonState.Secondary) != 0 ? SKMouseButton.Right :
                (e.ButtonState & MotionEventButtonState.Tertiary) != 0 ? SKMouseButton.Middle : SKMouseButton.Left;
            var args = new SKTouchEventArgs(e.GetPointerId(i), action, button, device,
                new SKPoint(e.GetX(i), e.GetY(i)), contact, 0, e.GetPressure(i));
            ((ISKGLView)_owner).OnTouch(args);
        }
        if (_inputTiming && (e.ActionMasked != MotionEventActions.Move || Stopwatch.GetElapsedTime(started).TotalMilliseconds > 8))
            global::Android.Util.Log.Info("DorotiInputTiming", $"action={e.ActionMasked} eventMs={e.EventTime} dispatchMs={Stopwatch.GetElapsedTime(started).TotalMilliseconds:F3}");
        if (FrameworkWorkProfile.AllocationEnabled && e.ActionMasked == MotionEventActions.Up)
        {
            RemoveCallbacks(_allocationProfileCallback);
            PostDelayed(_allocationProfileCallback, 2000);
        }
        return true;
    }

    public override bool OnGenericMotionEvent(MotionEvent? e)
    {
        if (e is null || _owner?.EnableTouchEvents != true) return base.OnGenericMotionEvent(e);
        var action = e.ActionMasked switch {
            MotionEventActions.HoverEnter => SKTouchAction.Entered,
            MotionEventActions.HoverExit => SKTouchAction.Exited,
            MotionEventActions.HoverMove => SKTouchAction.Moved,
            MotionEventActions.Scroll => SKTouchAction.WheelChanged,
            _ => (SKTouchAction?)null };
        if (action is null) return base.OnGenericMotionEvent(e);
        ((ISKGLView)_owner).OnTouch(new SKTouchEventArgs(e.GetPointerId(0), action.Value,
            SKMouseButton.Left, SKTouchDeviceType.Mouse, new(e.GetX(), e.GetY()), false,
            (int)(e.GetAxisValue(Axis.Vscroll) * 48), 0));
        return true;
    }

    [DllImport("android", EntryPoint = "ANativeWindow_fromSurface")]
    private static extern nint ANativeWindowFromSurface(nint environment, nint surface);
    [DllImport("android", EntryPoint = "ANativeWindow_release")]
    private static extern void ANativeWindowRelease(nint window);

    protected override void Dispose(bool disposing)
    {
        if (disposing) { Disconnect(); RemoveCallbacks(_allocationProfileCallback); _allocationProfileCallback.Dispose(); _drawCallback.Dispose(); _gpuCompletionCallback.Dispose(); }
        base.Dispose(disposing);
    }
}
#endif
