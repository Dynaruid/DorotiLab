using System.Globalization;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
#if WINDOWS
using Microsoft.UI.Xaml.Media;
#endif
using Locale = Doroti.Ui.Locale;
using Rect = Doroti.Ui.Rect;
using Size = Doroti.Ui.Size;

namespace Doroti.Host.Maui;

internal sealed class MauiHostAdapter :
    IViewHostCapability,
    IFrameHostCapability,
    IPlatformEnvironmentHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    IPlatformServicesHostCapability,
    ITextInputHostCapability
{
#if WINDOWS
    // Keep high-refresh compositor callbacks from outrunning the ANGLE swap
    // chain. Common 120/144/165 Hz displays then issue one Doroti frame every
    // two callbacks, while 60/90 Hz displays remain native-rate.
    private static readonly TimeSpan MinimumCompositionFrameInterval = TimeSpan.FromMilliseconds(10);
#endif
    private readonly ulong _viewId;
    private readonly SKGLView _view;
    private readonly object _gate = new();
    private Action<TimeSpan>? _pendingFrameCallback;
    private readonly IMauiSemanticsBridge _semantics;
    private readonly MauiTextInputBridge _textInput;
    private readonly Dictionary<ulong, (double X, double Y)> _pointerPositions = [];
    private readonly IDisposable _nativeInput;
    private Size _logicalSize;
    private double _density;
    private long _metricsGeneration = 1;
    private long _contextGeneration;
    private long _surfaceGeneration;
    private long _invalidationsRequested;
    private long _invalidationsCoalesced;
    private long _nativePointerEvents;
    private long _inputSequence;
    private long _frameRequestsCoalesced;
    private TimeSpan _lastVsyncTimestamp;
    private bool _invalidatePending;
    private bool _isPainting;
    private bool _invalidateAfterPaint;
#if WINDOWS
    private bool _compositionVsyncRequested;
    private bool _compositionVsyncAttached;
    private TimeSpan _lastCompositionInvalidateTimestamp = TimeSpan.MinValue;
#elif ANDROID
    private readonly AndroidFrameCallback _androidFrameCallback;
    private readonly HashSet<ulong> _androidActiveTouchPointers = [];
    private bool _androidFrameCallbackPosted;
    private TimeSpan? _androidVsyncTimestamp;
    private long _androidFrameTimeOriginNanos;
    private TimeSpan _androidFrameTimeOrigin;
#endif
    private bool _disposed;

    internal MauiHostAdapter(ulong viewId, SKGLView view, MauiTextInputBridge textInput,
        Size logicalSize, IMauiSemanticsBridge? semantics = null)
    {
        _viewId = viewId;
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _textInput = textInput ?? throw new ArgumentNullException(nameof(textInput));
        _logicalSize = logicalSize ?? throw new ArgumentNullException(nameof(logicalSize));
        _density = Math.Max(1, DeviceDisplay.Current.MainDisplayInfo.Density);
        _semantics = semantics ?? new NullMauiSemanticsBridge();
#if ANDROID
        _androidFrameCallback = new AndroidFrameCallback(HandleAndroidFrame);
#endif
        _view.HasRenderLoop = false;
        _view.EnableTouchEvents = true;
        _view.Touch += HandleTouch;
        _view.SizeChanged += HandleSizeChanged;
        _view.Focused += HandleFocused;
        _view.Unfocused += HandleUnfocused;
        _textInput.EditingStateChanged += HandleEditingStateChanged;
        _textInput.ActionPerformed += HandleActionPerformed;
        _textInput.FocusChanged += HandleTextFocusChanged;
        if (Application.Current is { } application)
            application.RequestedThemeChanged += HandleRequestedThemeChanged;
        _nativeInput = MauiNativeInput.Attach(_view, _textInput, _viewId, data => KeyData?.Invoke(data));
    }

    internal MauiSurfaceSnapshot Snapshot { get; private set; } = new(
        0, 0, 1, 1, 0, 0, "not-attached", "not-attached");

    internal long InvalidationsRequested => Interlocked.Read(ref _invalidationsRequested);
    internal long InvalidationsCoalesced => Interlocked.Read(ref _invalidationsCoalesced);
    internal long NativePointerEvents => Interlocked.Read(ref _nativePointerEvents);
    internal long InputSequence => Interlocked.Read(ref _inputSequence);
    internal long FrameRequestsCoalesced => Interlocked.Read(ref _frameRequestsCoalesced);
    internal MauiSemanticsDiagnostics SemanticsDiagnostics => _semantics.Diagnostics;
    public ViewMetrics Metrics => new(
        new Size(_logicalSize.width * _density, _logicalSize.height * _density), _density,
        ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed,
        _metricsGeneration, _surfaceGeneration);
    public PlatformConfiguration Configuration => new(
        [ToLocale(CultureInfo.CurrentUICulture)],
        Application.Current?.RequestedTheme == AppTheme.Dark ? Brightness.dark : Brightness.light,
        false, false,
#if WINDOWS
        HostOperatingSystem.windows
#elif MACCATALYST
        HostOperatingSystem.macOS
#elif ANDROID
        HostOperatingSystem.android
#else
#error Doroti.Host.Maui requires an explicit operating-system mapping.
#endif
    );

    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public event Action<PlatformConfiguration>? ConfigurationChanged;
    public event Action<PointerDataPacket>? PointerData;
    public event Action<KeyData>? KeyData;
    public event Action<RawFocusData>? FocusData;
    public event Action<DorotiTextEditingState>? EditingStateChanged;
    public event Action<DorotiTextInputAction>? ActionPerformed;
    internal event Action<long, TimeSpan>? InputReceived;

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RequestInvalidate();
        LifecycleChanged?.Invoke(AppLifecycleState.resumed);
    }

    internal void NotifyLifecycle(AppLifecycleState state)
    {
        if (!_disposed) LifecycleChanged?.Invoke(state);
    }

    internal void NotifyCloseRequested()
    {
        if (!_disposed) CloseRequested?.Invoke();
    }

    public void Resize(Size logicalSize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _logicalSize = logicalSize;
        _metricsGeneration++;
        MetricsChanged?.Invoke(Metrics);
        RequestInvalidate();
    }

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(callback);
        var requestFrame = false;
        lock (_gate)
        {
            // Keep one host request pending. A delayed native paint must not
            // accumulate framework callbacks behind it.
            if (_pendingFrameCallback is not null)
            {
                Interlocked.Increment(ref _frameRequestsCoalesced);
                return;
            }
            _pendingFrameCallback = callback;
            requestFrame = true;
        }
        if (!requestFrame) return;
#if WINDOWS
        SubscribeToCompositionVsync();
#elif ANDROID
        RequestAndroidVsync();
#else
        RequestInvalidate();
#endif
    }

    internal void BeginPaint(SKPaintGLSurfaceEventArgs args, object? context, string nativeViewType, string backend)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate)
        {
            _invalidatePending = false;
            _isPainting = true;
        }
        var previous = Snapshot;
        if (context is not null && !ReferenceEquals(context, _lastContext))
        {
            _lastContext = context;
            _contextGeneration++;
            _surfaceGeneration++;
        }
        var pixelSizeChanged = previous.PixelWidth != args.BackendRenderTarget.Width ||
                               previous.PixelHeight != args.BackendRenderTarget.Height;
        if (pixelSizeChanged)
            _surfaceGeneration++;
        var density = Math.Max(1, DeviceDisplay.Current.MainDisplayInfo.Density);
        var densityChanged = Math.Abs(_density - density) > double.Epsilon;
        _density = density;
        if (pixelSizeChanged || densityChanged)
            _metricsGeneration++;
        Snapshot = new(args.BackendRenderTarget.Width, args.BackendRenderTarget.Height, _density,
            _metricsGeneration, _contextGeneration, _surfaceGeneration, nativeViewType, backend);
        if (pixelSizeChanged || densityChanged)
            MetricsChanged?.Invoke(Metrics);
        Action<TimeSpan>? callback;
        TimeSpan? nativeVsyncTimestamp = null;
        lock (_gate)
        {
            callback = _pendingFrameCallback;
            _pendingFrameCallback = null;
#if ANDROID
            nativeVsyncTimestamp = _androidVsyncTimestamp;
            _androidVsyncTimestamp = null;
#endif
        }
        var now = DorotiFrameClock.ClampForward(
            nativeVsyncTimestamp ?? DorotiFrameClock.Now, _lastVsyncTimestamp);
        _lastVsyncTimestamp = now;
        callback?.Invoke(now);
    }

    internal void EndPaint()
    {
        if (_disposed) return;
        var dispatch = false;
#if WINDOWS
        var unsubscribeVsync = false;
#endif
        lock (_gate)
        {
            _isPainting = false;
#if WINDOWS
            if (_pendingFrameCallback is null && _compositionVsyncRequested)
                unsubscribeVsync = true;
#endif
            if (_invalidateAfterPaint)
            {
                _invalidateAfterPaint = false;
                var compositorOwnsNextFrame = false;
#if WINDOWS
                compositorOwnsNextFrame = _compositionVsyncRequested;
#elif ANDROID
                compositorOwnsNextFrame = _androidFrameCallbackPosted;
#endif
                if (!compositorOwnsNextFrame && !_invalidatePending)
                {
                    _invalidatePending = true;
                    dispatch = true;
                }
            }
        }
#if WINDOWS
        if (unsubscribeVsync) UnsubscribeFromCompositionVsync();
#endif
        if (dispatch) _view.Dispatcher.Dispatch(_view.InvalidateSurface);
    }

    private object? _lastContext;

    internal event Action<int, SemanticsAction, object?>? SemanticsAction;

    internal void AttachFrameworkTrace(DorotiFrameTrace trace) => _semantics.AttachFrameTrace(trace, _viewId);

    internal void UpdateSemantics(SemanticsUpdate update) =>
        _semantics.Update(update, (nodeId, action, arguments) =>
            SemanticsAction?.Invoke(nodeId, action, arguments));

    internal void ClearSemantics() => _semantics.Clear();

    internal void RequestInvalidate()
    {
        Interlocked.Increment(ref _invalidationsRequested);
        lock (_gate)
        {
            // TextureView can discard an InvalidateSurface issued by its active
            // PaintSurface callback. Post that request after the paint completes.
            if (_isPainting)
            {
                if (_invalidateAfterPaint) Interlocked.Increment(ref _invalidationsCoalesced);
                _invalidateAfterPaint = true;
                return;
            }
            if (_invalidatePending)
            {
                Interlocked.Increment(ref _invalidationsCoalesced);
                return;
            }
            _invalidatePending = true;
        }
        _view.Dispatcher.Dispatch(_view.InvalidateSurface);
    }

#if ANDROID
    private void RequestAndroidVsync()
    {
        lock (_gate)
        {
            if (_disposed || _androidFrameCallbackPosted) return;
            _androidFrameCallbackPosted = true;
        }
        _view.Dispatcher.Dispatch(PostAndroidFrameCallback);
    }

    private void PostAndroidFrameCallback()
    {
        lock (_gate)
        {
            if (_disposed ||
                (_pendingFrameCallback is null && _androidActiveTouchPointers.Count == 0))
            {
                _androidFrameCallbackPosted = false;
                return;
            }
        }
        Android.Views.Choreographer.Instance!.PostFrameCallback(_androidFrameCallback);
    }

    private void HandleAndroidFrame(long frameTimeNanos)
    {
        var invalidate = false;
        var repost = false;
        lock (_gate)
        {
            _androidFrameCallbackPosted = false;
            if (_disposed) return;
            var touchActive = _androidActiveTouchPointers.Count > 0;
            if (_pendingFrameCallback is null)
            {
                // Keep the display waiter armed while a finger is down. Input
                // arriving late in this pulse can then use the very next pulse
                // instead of registering after the GL paint has completed.
                repost = touchActive;
            }
            else if (_isPainting)
            {
                // A callback queued during the active GL paint must survive to
                // another pulse.
                repost = true;
            }
            else if (_invalidatePending)
            {
                // The render thread has not consumed this request yet. Keep its
                // timestamp current so a late paint does not animate from a
                // pulse that the surface has already missed.
                _androidVsyncTimestamp = MapAndroidFrameTimestamp(frameTimeNanos);
                repost = touchActive;
            }
            else
            {
                _androidVsyncTimestamp = MapAndroidFrameTimestamp(frameTimeNanos);
                _invalidatePending = true;
                invalidate = true;
                repost = touchActive;
            }
        }
        if (repost) RequestAndroidVsync();
        if (!invalidate) return;
        Interlocked.Increment(ref _invalidationsRequested);
        _view.InvalidateSurface();
    }

    private TimeSpan MapAndroidFrameTimestamp(long frameTimeNanos)
    {
        if (_androidFrameTimeOriginNanos == 0)
        {
            _androidFrameTimeOriginNanos = frameTimeNanos;
            _androidFrameTimeOrigin = DorotiFrameClock.Now;
        }
        var elapsedNanos = Math.Max(0, frameTimeNanos - _androidFrameTimeOriginNanos);
        return _androidFrameTimeOrigin + TimeSpan.FromTicks(elapsedNanos / 100);
    }

    private void CancelAndroidVsync()
    {
        lock (_gate)
        {
            _androidFrameCallbackPosted = false;
            _androidActiveTouchPointers.Clear();
        }
        _view.Dispatcher.Dispatch(() =>
            Android.Views.Choreographer.Instance!.RemoveFrameCallback(_androidFrameCallback));
    }

    private sealed class AndroidFrameCallback(Action<long> callback) :
        Java.Lang.Object,
        Android.Views.Choreographer.IFrameCallback
    {
        public void DoFrame(long frameTimeNanos) => callback(frameTimeNanos);
    }
#endif

#if WINDOWS
    private void SubscribeToCompositionVsync()
    {
        lock (_gate)
        {
            if (_compositionVsyncRequested) return;
            _compositionVsyncRequested = true;
        }
        DispatchCompositionVsyncUpdate();
    }

    private void UnsubscribeFromCompositionVsync()
    {
        lock (_gate)
        {
            if (!_compositionVsyncRequested || _pendingFrameCallback is not null) return;
            _compositionVsyncRequested = false;
        }
        DispatchCompositionVsyncUpdate();
    }

    private void DispatchCompositionVsyncUpdate() =>
        _view.Dispatcher.Dispatch(UpdateCompositionVsyncSubscription);

    private void UpdateCompositionVsyncSubscription()
    {
        bool shouldAttach;
        bool shouldDetach;
        lock (_gate)
        {
            shouldAttach = !_disposed && _compositionVsyncRequested && !_compositionVsyncAttached;
            shouldDetach = (_disposed || !_compositionVsyncRequested) && _compositionVsyncAttached;
        }

        // CompositionTarget is a WinRT UI-thread event. Frame requests can
        // originate in timers or framework microtasks, so both event mutations
        // must cross the MAUI dispatcher before touching the compositor.
        if (shouldAttach)
        {
            lock (_gate) _lastCompositionInvalidateTimestamp = TimeSpan.MinValue;
            CompositionTarget.Rendering += HandleCompositionRendering;
            lock (_gate) _compositionVsyncAttached = true;
        }
        else if (shouldDetach)
        {
            CompositionTarget.Rendering -= HandleCompositionRendering;
            lock (_gate) _compositionVsyncAttached = false;
        }
    }

    private void HandleCompositionRendering(object? sender, object args)
    {
        _ = sender;
        _ = args;
        if (_disposed) return;
        var timestamp = DorotiFrameClock.Now;
        lock (_gate)
        {
            if (_pendingFrameCallback is null || _invalidatePending) return;
            // Flutter-style backpressure: retain the newest framework request,
            // but do not feed ANGLE more swap-chain work than it can present.
            // The first request after idle is always immediate.
            if (_lastCompositionInvalidateTimestamp != TimeSpan.MinValue &&
                timestamp - _lastCompositionInvalidateTimestamp < MinimumCompositionFrameInterval) return;
            if (_isPainting)
            {
                _invalidateAfterPaint = true;
                return;
            }
            _invalidatePending = true;
            _lastCompositionInvalidateTimestamp = timestamp;
        }
        Interlocked.Increment(ref _invalidationsRequested);
        _view.InvalidateSurface();
    }
#endif

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        _ = direction;
        if (state == ViewFocusState.focused) _view.Focus();
        else _view.Unfocus();
    }

    public async ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Clipboard.Default.GetTextAsync();
    }

    public async ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Clipboard.Default.SetTextAsync(text ?? string.Empty);
    }

    public void SetCursor(DorotiMouseCursorKind cursor) => MauiNativeInput.SetCursor(_view, cursor);
    public void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState) =>
        _textInput.SetClient(configuration, initialState);
    public void UpdateState(DorotiTextEditingState state) => _textInput.UpdateState(state);
    public void SetCaretRect(Rect logicalRect) => _textInput.SetCaretRect(logicalRect);
    public void ClearClient() => _textInput.ClearClient();

    public void Close()
    {
        if (_disposed) return;
        CloseRequested?.Invoke();
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
#if WINDOWS
        lock (_gate)
        {
            _compositionVsyncRequested = false;
        }
        DispatchCompositionVsyncUpdate();
#elif ANDROID
        CancelAndroidVsync();
#endif
        _view.Touch -= HandleTouch;
        _view.SizeChanged -= HandleSizeChanged;
        _view.Focused -= HandleFocused;
        _view.Unfocused -= HandleUnfocused;
        _textInput.EditingStateChanged -= HandleEditingStateChanged;
        _textInput.ActionPerformed -= HandleActionPerformed;
        _textInput.FocusChanged -= HandleTextFocusChanged;
        if (Application.Current is { } application)
            application.RequestedThemeChanged -= HandleRequestedThemeChanged;
        _nativeInput.Dispose();
        _textInput.Dispose();
        if (_semantics is IDisposable semantics)
            semantics.Dispose();
        lock (_gate)
        {
            _pendingFrameCallback = null;
            _invalidateAfterPaint = false;
            _isPainting = false;
        }
        Closed?.Invoke();
        GC.KeepAlive(ConfigurationChanged);
        GC.KeepAlive(KeyData);
        GC.KeepAlive(EditingStateChanged);
        GC.KeepAlive(ActionPerformed);
    }

    private void HandleSizeChanged(object? sender, EventArgs args)
    {
        _ = sender;
        _ = args;
        if (_view.Width <= 0 || _view.Height <= 0) return;
        _logicalSize = new(_view.Width, _view.Height);
        _metricsGeneration++;
        MetricsChanged?.Invoke(Metrics);
        RequestInvalidate();
    }

    private void HandleTouch(object? sender, SKTouchEventArgs args)
    {
        _ = sender;
        Interlocked.Increment(ref _nativePointerEvents);
        var inputSequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = DorotiFrameClock.Now;
        InputReceived?.Invoke(inputSequence, timestamp);
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
        var pointer = checked((ulong)Math.Max(0, args.Id));
#if ANDROID
        var keepAndroidVsyncArmed = false;
        lock (_gate)
        {
            if (change == PointerChange.down)
                _androidActiveTouchPointers.Add(pointer);
            else if (change is PointerChange.up or PointerChange.cancel or PointerChange.remove)
                _androidActiveTouchPointers.Remove(pointer);
            keepAndroidVsyncArmed = _androidActiveTouchPointers.Count > 0;
        }
        if (keepAndroidVsyncArmed) RequestAndroidVsync();
#endif
        var hasPrevious = _pointerPositions.TryGetValue(pointer, out var previous);
        var x = args.Location.X;
        var y = args.Location.Y;
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
        PointerData?.Invoke(new([new(_viewId, timestamp, change,
            kind, pointer, x, y,
            hasPrevious ? x - previous.X : 0, hasPrevious ? y - previous.Y : 0, buttons,
            scrollDeltaX: 0, scrollDeltaY: args.ActionType == SKTouchAction.WheelChanged ? -args.WheelDelta : 0,
            signalKind: args.ActionType == SKTouchAction.WheelChanged ? PointerSignalKind.scroll : PointerSignalKind.none,
            pointerIdentifier: pointer, pressure: args.Pressure, pressureMin: 0, pressureMax: 1) ]));
        if (change is PointerChange.remove or PointerChange.cancel) _pointerPositions.Remove(pointer);
        else _pointerPositions[pointer] = (x, y);
        args.Handled = true;
    }

    private void HandleFocused(object? sender, FocusEventArgs args) =>
        FocusData?.Invoke(new(_viewId, true, DorotiFrameClock.Now));

    private void HandleUnfocused(object? sender, FocusEventArgs args) =>
        FocusData?.Invoke(new(_viewId, false, DorotiFrameClock.Now));

    private void HandleTextFocusChanged(bool focused) =>
        FocusData?.Invoke(new(_viewId, focused, DorotiFrameClock.Now));

    private void HandleEditingStateChanged(DorotiTextEditingState state) => EditingStateChanged?.Invoke(state);
    private void HandleActionPerformed(DorotiTextInputAction action) => ActionPerformed?.Invoke(action);

    private void HandleRequestedThemeChanged(object? sender, AppThemeChangedEventArgs args) =>
        ConfigurationChanged?.Invoke(Configuration);

    private static Locale ToLocale(CultureInfo culture)
    {
        var pieces = culture.Name.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return pieces.Length > 1 ? new(pieces[0], pieces[^1]) : new(culture.TwoLetterISOLanguageName);
    }
}
