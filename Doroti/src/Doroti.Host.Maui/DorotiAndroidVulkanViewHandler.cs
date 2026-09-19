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

public sealed class DorotiAndroidVulkanViewHandler : ViewHandler<DorotiGraphiteView, DorotiAndroidViewContainer>
{
    private static readonly CommandMapper<DorotiGraphiteView, DorotiAndroidVulkanViewHandler> Commands =
        new(ViewCommandMapper) { [nameof(ISKGLView.InvalidateSurface)] = (handler, _, _) => handler.PlatformView.RequestFrame() };
    public DorotiAndroidVulkanViewHandler() : base(ViewMapper, Commands) { }
    protected override DorotiAndroidViewContainer CreatePlatformView() => new(Context);
    protected override void ConnectHandler(DorotiAndroidViewContainer platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Connect(VirtualView);
    }
    protected override void DisconnectHandler(DorotiAndroidViewContainer platformView)
    {
        platformView.Disconnect();
        base.DisconnectHandler(platformView);
    }
}

public sealed class DorotiAndroidViewContainer : Android.Widget.FrameLayout
{
    internal DorotiAndroidVulkanView Surface { get; }
    public DorotiAndroidViewContainer(Context context) : base(context)
    {
        Surface = new(context);
        AddView(Surface, new LayoutParams(-1, -1));
    }
    internal void Connect(DorotiGraphiteView owner) => Surface.Connect(owner);
    internal void Disconnect() => Surface.Disconnect();
    internal void RequestFrame() => Surface.RequestFrame();
    internal void DrawFromVsync() => Surface.DrawFromVsync();
}

public sealed class DorotiAndroidVulkanView : SurfaceView, ISurfaceHolderCallback
{
    private DorotiGraphiteView? _owner;
    private GraphiteVulkanWindow? _window;
    private nint _nativeWindow;
    private int _width, _height;
    private bool _pending, _live;
    private Task? _retirement;
    private bool _faulted;
    private static int _retiringGenerations;
    // Failed native cleanup must survive collection of a disconnected Java view.
    private static readonly List<(GraphiteVulkanWindow Window, nint NativeWindow)> FailedRetirements = [];
    private long _generation;
    private readonly Java.Lang.Runnable _drawCallback;
    private readonly Java.Lang.Runnable _gpuCompletionCallback;
    private readonly Java.Lang.Runnable _allocationProfileCallback;
    private bool _gpuCompletionPending;
    private int _timingFrames;
    private readonly double[] _timingSums = new double[7];
    private readonly double[] _timingMaxima = new double[7];
    private bool _inputTiming;
    private bool _platformFrameTiming;
    private double _density = 1;
    private readonly AndroidTrackpadGesture _trackpadInput;
    internal double SemanticsDensity => _density;
    internal AndroidX.CustomView.Widget.ExploreByTouchHelper? SemanticsHelper { get; set; }

    protected override bool DispatchHoverEvent(MotionEvent? e) =>
        (e is not null && SemanticsHelper?.DispatchHoverEvent(e) == true) || base.DispatchHoverEvent(e);

    public DorotiAndroidVulkanView(Context context) : base(context)
    {
        _trackpadInput = new(data => _owner?.DispatchNativePointer(data));
        _drawCallback = new(DrawFrame);
        _gpuCompletionCallback = new(PollGpuCompletion);
        _allocationProfileCallback = new(() =>
        {
            foreach (var entry in FrameworkWorkProfile.CaptureEntries())
                Android.Util.Log.Info("DorotiAllocation", $"kind={entry.Kind} calls={entry.Calls} allocated={entry.AllocatedBytes} self={entry.SelfAllocatedBytes} type={entry.Type}");
        });
        // SurfaceView is composed below the MAUI semantics and IME overlay.
        Holder!.SetFormat(Format.Translucent);
        Focusable = FocusableInTouchMode = true;
    }

    internal void Connect(DorotiGraphiteView owner)
    {
        _owner = owner;
        owner.PlatformViews?.Bind(this, (DorotiAndroidViewContainer)Parent!);
        RefreshDensity();
        _inputTiming = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?
            .Intent?.GetStringExtra("DOROTI_INPUT_TIMING") == "1";
        _platformFrameTiming = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?
            .Intent?.GetStringExtra("DOROTI_PLATFORM_FRAME_PROFILE") == "1";
        Holder!.AddCallback(this);
        _live = Holder.Surface?.IsValid == true;
        if (_live && Holder.SurfaceFrame is { } bounds) { _width = bounds.Width(); _height = bounds.Height(); }
        RequestFrame();
    }
    internal void Disconnect()
    {
        CancelTrackpads();
        Holder?.RemoveCallback(this);
        _live = false;
        ReleaseSurface(); _owner = null;
    }
    public void SurfaceCreated(ISurfaceHolder holder) { _live = true; RequestFrame(); }
    public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
    { _width = width; _height = height; RefreshDensity(); RequestFrame(); }
    public void SurfaceDestroyed(ISurfaceHolder holder) { CancelTrackpads(); _live = false; ReleaseSurface(); }

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
        if (_pending || !_live || _faulted || _retirement is not null || _owner is null || _width <= 0 || _height <= 0) return;
        _pending = true;
        PostOnAnimation(_drawCallback);
    }

    internal void DrawFromVsync()
    {
        if (!_live || _faulted || _retirement is not null || _owner is null || _width <= 0 || _height <= 0) return;
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
        if (!_live || _faulted || _retirement is not null || _owner is null) return;
        MauiSkiaPaintContext? paint = null;
        var frameStarted = _platformFrameTiming ? Stopwatch.GetTimestamp() : 0;
        try
        {
            if (_window is null)
            {
                // A replacement Activity/View must not evade a previous view's
                // retirement hold and accumulate GPU generations during a stall.
                if (Volatile.Read(ref _retiringGenerations) != 0) { RequestFrame(); return; }
                if (!OperatingSystem.IsAndroidVersionAtLeast(24))
                    throw new PlatformNotSupportedException("Doroti Graphite requires Android API 24 or newer and a Vulkan 1.2 device for the official profile.");
                _nativeWindow = ANativeWindowFromSurface(JNIEnv.Handle, Holder!.Surface!.Handle);
                if (_nativeWindow == 0) throw new InvalidOperationException("ANativeWindow_fromSurface failed.");
                _window = GraphiteVulkanWindow.CreateAndroid(_nativeWindow);
                _window.EnableFrameTiming = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?
                    .Intent?.GetStringExtra("DOROTI_MAUI_EVIDENCE") == "1" || _inputTiming || _platformFrameTiming;
                _window.ResourcesReleasing += ReleaseRendererResources;
                _generation++;
                Android.Util.Log.Info("DorotiGraphite", $"Vulkan device={_window.DeviceName} generation={_generation}");
            }
            var presented = _window.Render(_width, _height, (surface, width, height) =>
            {
                paint = new(surface, _window.ContextIdentity, width, height, _density,
                    (_generation << 32) | _window.Generation, GetType().FullName!, "Android/SurfaceView/Graphite-Vulkan");
                _owner.PaintGraphite(paint);
            }, () => paint is { SkipPresent: false, SkipRaster: false } && _owner.PlatformViews?.RejectFrame != true);
            if (_owner.PlatformViews is { } platformViews) presented = platformViews.Finish(presented);
            if (paint?.Completion is { } completion) _owner.CompleteGraphite(completion, !presented);
            if (presented && _window.EnableFrameTiming) RecordFrameTiming(_window.LastFrameTiming);
            if (presented && _platformFrameTiming)
                Android.Util.Log.Info("DorotiPlatformTiming", FormattableString.Invariant(
                    $"frame={_window.SubmittedWindowFrames} ownerMs={Stopwatch.GetElapsedTime(frameStarted).TotalMilliseconds:F3} vulkanMs={_window.LastFrameTiming.TotalMs:F3} paintMs={_window.LastFrameTiming.PaintMs:F3} fenceMs={_window.LastFrameTiming.FenceMs:F3}"));
            if (presented && _inputTiming && _window.LastFrameTiming.TotalMs > 12)
                Android.Util.Log.Info("DorotiInputTiming", $"frame={_window.LastFrameTiming}");
            ScheduleGpuCompletion();
            if (!presented || paint?.Completion is null) RequestFrame();
        }
        catch (Exception exception)
        {
            _owner?.PlatformViews?.Finish(false);
            _owner?.FailGraphite(paint?.Completion, exception);
            Android.Util.Log.Error("DorotiGraphite", exception.ToString());
            _faulted = true;
            if (_window is not null) ReleaseSurface();
            else if (_window is null && _nativeWindow != 0) { ANativeWindowRelease(_nativeWindow); _nativeWindow = 0; }
        }
    }

    private void ReleaseRendererResources() => _owner?.ReleaseGraphiteResources();
    internal Task<Skia.Rendering.SkiaGraphiteReadback> RequestPlatformReadback(SKSurface surface, SKImageInfo info) =>
        (_window ?? throw new InvalidOperationException("Android Vulkan window is unavailable.")).RequestPlatformReadback(surface, info);
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
            Android.Util.Log.Error("DorotiGraphite", exception.ToString());
            _faulted = true;
            ReleaseSurface();
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
        Android.Util.Log.Info("DorotiFrameTiming", $"frames={_timingFrames} phases=acquire,paint,submit,copy,fence,present,total meanMs={means} maxMs={maxima} submitted={_window?.SubmittedWindowFrames} completed={_window?.CompletedWindowFrames} busy={_window?.BusyWindowFrames} unavailable={_window?.UnavailableWindowImages} maxInFlight={_window?.MaximumWindowFramesInFlight}");
        _timingFrames = 0;
        Array.Clear(_timingSums); Array.Clear(_timingMaxima);
    }
    private void ReleaseSurface()
    {
        RemoveCallbacks(_drawCallback);
        RemoveCallbacks(_gpuCompletionCallback);
        _gpuCompletionPending = false;
        _pending = false;
        if (_retirement is not null) return;
        if (_window is not { } window)
        {
            if (_nativeWindow != 0) ANativeWindowRelease(_nativeWindow);
            _nativeWindow = 0;
            return;
        }
        // Close admission synchronously; never acquire/present after this callback.
        // Keep the native window and generation alive while the GPU drains off the
        // UI thread. Surface recreation waits for this exact retirement to finish.
        var nativeWindow = _nativeWindow;
        Interlocked.Increment(ref _retiringGenerations);
        try { _retirement = window.DisposeAfterOwnerDetachedAsync(); }
        catch (Exception exception) { _retirement = Task.FromException(exception); }
        _window = null;
        _nativeWindow = 0;
        _ = FinishRetirementAsync(window, nativeWindow, _retirement);
    }

    private async Task FinishRetirementAsync(GraphiteVulkanWindow window, nint nativeWindow, Task retirement)
    {
        try
        {
            if (await Task.WhenAny(retirement, Task.Delay(TimeSpan.FromSeconds(5))).ConfigureAwait(false) != retirement)
                Android.Util.Log.Error("DorotiGraphite", "Surface retirement exceeded five seconds; retaining generation and blocking replacement (not device loss).");
            await retirement.ConfigureAwait(false);
            ANativeWindowRelease(nativeWindow);
            Interlocked.Decrement(ref _retiringGenerations);
            if (window.EnableFrameTiming)
                Android.Util.Log.Info("DorotiFrameTiming", $"drained submitted={window.SubmittedWindowFrames} completed={window.CompletedWindowFrames} outstanding={window.WindowFramesInFlight}");
            Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
            {
                _retirement = null;
                _faulted = false;
                RequestFrame();
            });
        }
        catch (Exception exception)
        {
            // Keep the failed generation rooted. Neither its native window nor a
            // replacement renderer may be released/admitted on uncertain cleanup.
            lock (FailedRetirements) FailedRetirements.Add((window, nativeWindow));
            Android.Util.Log.Error("DorotiGraphite", "Surface retirement failed; resources retained: " + exception);
        }
    }
    public override bool OnTouchEvent(MotionEvent? e)
    {
        using var allocationProfile = FrameworkWorkProfile.AllocationEnabled ? FrameworkWorkProfile.Begin(GetType(), 9) : default;
        if (e is null || _owner?.EnableTouchEvents != true) return false;
        var started = _inputTiming ? Stopwatch.GetTimestamp() : 0;
        if (e.ActionMasked == MotionEventActions.Down) RequestFocus();
        var change = e.ActionMasked switch {
            MotionEventActions.Down or MotionEventActions.PointerDown => PointerChange.down,
            MotionEventActions.Up or MotionEventActions.PointerUp => PointerChange.up,
            MotionEventActions.Cancel => PointerChange.cancel,
            MotionEventActions.Move => PointerChange.move,
            _ => (PointerChange?)null };
        if (change is null) return base.OnTouchEvent(e);
        var all = e.ActionMasked is MotionEventActions.Move or MotionEventActions.Cancel;
        for (var i = 0; i < e.PointerCount; i++)
        {
            if (!all && i != e.ActionIndex) continue;
            DispatchPointer(e, i, change.Value);
        }
        if (_inputTiming && (e.ActionMasked != MotionEventActions.Move || Stopwatch.GetElapsedTime(started).TotalMilliseconds > 8))
            Android.Util.Log.Info("DorotiInputTiming", $"action={e.ActionMasked} eventMs={e.EventTime} dispatchMs={Stopwatch.GetElapsedTime(started).TotalMilliseconds:F3}");
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
        var change = e.ActionMasked switch {
            MotionEventActions.HoverEnter => PointerChange.add,
            MotionEventActions.HoverExit => PointerChange.remove,
            MotionEventActions.HoverMove or MotionEventActions.Scroll => PointerChange.hover,
            _ => (PointerChange?)null };
        if (change is null || e.PointerCount == 0) return base.OnGenericMotionEvent(e);
        DispatchPointer(e, 0, change.Value);
        return true;
    }

    private void DispatchPointer(MotionEvent e, int index, PointerChange change)
    {
        var kind = AndroidPointerMapping.Kind((int)e.GetToolType(index));
        var device = AndroidPointerMapping.DeviceIdentifier(e.DeviceId, e.GetPointerId(index));
        if (_trackpadInput.Handle(device, change, kind, e.Source == InputSourceType.Mouse,
            (int)e.ButtonState, e.GetX(index), e.GetY(index), DorotiFrameClock.Now)) return;
        var scroll = e.ActionMasked == MotionEventActions.Scroll;
        var config = scroll ? Android.Views.ViewConfiguration.Get(Context!) : null;
        var horizontalFactor = scroll && OperatingSystem.IsAndroidVersionAtLeast(26) ? config!.ScaledHorizontalScrollFactor : 48;
        var verticalFactor = scroll && OperatingSystem.IsAndroidVersionAtLeast(26) ? config!.ScaledVerticalScrollFactor : 48;
        _owner?.DispatchNativePointer(new(DorotiFrameClock.Now, change, kind,
            device, e.GetX(index), e.GetY(index),
            AndroidPointerMapping.Buttons(kind, (int)e.ButtonState),
            scroll ? -e.GetAxisValue(Axis.Hscroll) * horizontalFactor : 0,
            scroll ? -e.GetAxisValue(Axis.Vscroll) * verticalFactor : 0,
            scroll ? PointerSignalKind.scroll : PointerSignalKind.none, e.GetPressure(index),
            Orientation: e.GetOrientation(index), Tilt: e.GetAxisValue(Axis.Tilt, index)));
    }

    private void CancelTrackpads() => _trackpadInput.Cancel(DorotiFrameClock.Now);

    public override void OnWindowFocusChanged(bool hasWindowFocus)
    {
        if (!hasWindowFocus) CancelTrackpads();
        base.OnWindowFocusChanged(hasWindowFocus);
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
