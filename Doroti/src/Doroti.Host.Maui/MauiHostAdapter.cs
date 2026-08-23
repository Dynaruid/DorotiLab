using System.Globalization;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
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
    private readonly ulong _viewId;
    private readonly IMauiSkiaSurface _surface;
    private readonly object _gate = new();
    private Action<TimeSpan>? _pendingFrameCallback;
    private readonly IMauiSemanticsBridge _semantics;
    private readonly MauiTextInputBridge _textInput;
    private readonly Dictionary<ulong, (double X, double Y)> _pointerPositions = [];
    private readonly IDisposable _nativeInput;
    private Size _logicalSize;
    private double _density;
    private long _metricsGeneration = 1;
    private DorotiViewEpoch _viewEpoch;
    private long _contextGeneration;
    private long _surfaceGeneration;
    private long _invalidationsRequested;
    private long _invalidationsCoalesced = 0;
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
#elif ANDROID
    private readonly AndroidFrameCallback _androidFrameCallback;
    private readonly HashSet<ulong> _androidActiveTouchPointers = [];
    private bool _androidFrameCallbackPosted;
    private TimeSpan? _androidVsyncTimestamp;
    private long _androidFrameTimeOriginNanos;
    private TimeSpan _androidFrameTimeOrigin;
#endif
    private bool _disposed;

    internal MauiHostAdapter(ulong viewId, IMauiSkiaSurface surface, MauiTextInputBridge textInput,
        Size logicalSize, IMauiSemanticsBridge? semantics = null)
    {
        _viewId = viewId;
        _surface = surface ?? throw new ArgumentNullException(nameof(surface));
        _textInput = textInput ?? throw new ArgumentNullException(nameof(textInput));
        _logicalSize = logicalSize ?? throw new ArgumentNullException(nameof(logicalSize));
        _density = Math.Max(1, DeviceDisplay.Current.MainDisplayInfo.Density);
        var initialTarget = _surface.ResizeTarget ?? new DorotiResizeEpoch(
            _metricsGeneration,
            _logicalSize.width,
            _logicalSize.height,
            Math.Max(0, checked((int)Math.Round(_logicalSize.width * _density))),
            Math.Max(0, checked((int)Math.Round(_logicalSize.height * _density))),
            _density,
            DorotiFrameClock.Now.Ticks / 10);
        _viewEpoch = ToViewEpoch(initialTarget, _metricsGeneration);
        _semantics = semantics ?? new NullMauiSemanticsBridge();
#if ANDROID
        _androidFrameCallback = new AndroidFrameCallback(HandleAndroidFrame);
#endif
        _surface.Pointer += HandlePointer;
        _surface.Key += HandleKey;
        _surface.SizeChanged += HandleSizeChanged;
        _surface.FocusChanged += HandleFocusChanged;
        _textInput.EditingStateChanged += HandleEditingStateChanged;
        _textInput.ActionPerformed += HandleActionPerformed;
        _textInput.FocusChanged += HandleTextFocusChanged;
        if (Application.Current is { } application)
            application.RequestedThemeChanged += HandleRequestedThemeChanged;
        _nativeInput = _surface;
    }

    private MauiSurfaceSnapshot _snapshot = new(
        0, 0, 1, 1, 0, 0, "not-attached", "not-attached");
    internal MauiSurfaceSnapshot Snapshot => _surface.CaptureSnapshot(_snapshot);

    internal long InvalidationsRequested => Interlocked.Read(ref _invalidationsRequested);
    internal long InvalidationsCoalesced => Interlocked.Read(ref _invalidationsCoalesced);
    internal long NativePointerEvents => Interlocked.Read(ref _nativePointerEvents);
    internal long InputSequence => Interlocked.Read(ref _inputSequence);
    internal long FrameRequestsCoalesced => Interlocked.Read(ref _frameRequestsCoalesced);
    internal DorotiResizeEpoch ResizeTarget => _surface.ResizeTarget ?? ToResizeEpoch(ViewEpoch);
    public DorotiViewEpoch ViewEpoch => Volatile.Read(ref _viewEpoch);
    internal MauiSemanticsDiagnostics SemanticsDiagnostics => _semantics.Diagnostics;
    public ViewMetrics Metrics
    {
        get
        {
            var epoch = ViewEpoch;
            return new(
                new Size(epoch.PhysicalWidth, epoch.PhysicalHeight), epoch.DevicePixelRatio,
                ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed,
                epoch.MetricsGeneration, Interlocked.Read(ref _surfaceGeneration));
        }
    }
    public PlatformConfiguration Configuration => new(
        [ToLocale(CultureInfo.CurrentUICulture)],
        Application.Current?.RequestedTheme == AppTheme.Dark ? Brightness.dark : Brightness.light,
        false, false,
#if WINDOWS
        HostOperatingSystem.windows
#elif MACCATALYST
        HostOperatingSystem.macOS
#elif IOS
        HostOperatingSystem.iOS
#elif ANDROID
        HostOperatingSystem.android
#elif MACOS
        HostOperatingSystem.macOS
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
#if WINDOWS
        // A requested size is not a metrics publication. Let WinUI layout the
        // panel and publish the resulting panel epoch through SizeChanged.
        _surface.Dispatcher.Dispatch(() =>
        {
            _surface.Element.WidthRequest = logicalSize.width;
            _surface.Element.HeightRequest = logicalSize.height;
        });
#else
        _logicalSize = logicalSize;
        _metricsGeneration++;
        var target = new DorotiResizeEpoch(
            _metricsGeneration,
            logicalSize.width,
            logicalSize.height,
            Math.Max(0, checked((int)Math.Round(logicalSize.width * _density))),
            Math.Max(0, checked((int)Math.Round(logicalSize.height * _density))),
            _density,
            DorotiFrameClock.Now.Ticks / 10);
        Volatile.Write(ref _viewEpoch, ToViewEpoch(target, _metricsGeneration));
        MetricsChanged?.Invoke(Metrics);
        RequestInvalidate();
#endif
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

    internal void BeginPaint(MauiSkiaPaintContext paint)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate)
        {
            _invalidatePending = false;
            _isPainting = true;
        }
        var previous = Snapshot;
        if (paint.ContextIdentity is not null && !ReferenceEquals(paint.ContextIdentity, _lastContext))
        {
            _lastContext = paint.ContextIdentity;
            _contextGeneration++;
            _surfaceGeneration++;
        }
        var pixelSizeChanged = previous.PixelWidth != paint.PixelWidth ||
                               previous.PixelHeight != paint.PixelHeight;
        if (paint.SurfaceGeneration > 0)
            _surfaceGeneration = paint.SurfaceGeneration;
        else if (pixelSizeChanged)
            _surfaceGeneration++;
        var density = Math.Max(1, paint.Density);
        var expectedWidth = Math.Max(0, checked((int)Math.Round(_logicalSize.width * _density)));
        var expectedHeight = Math.Max(0, checked((int)Math.Round(_logicalSize.height * _density)));
        _snapshot = new(paint.PixelWidth, paint.PixelHeight, _density,
            _metricsGeneration, _contextGeneration, _surfaceGeneration,
            paint.NativeViewType, paint.GraphicsBackend,
            LogicalWidth: _logicalSize.width,
            LogicalHeight: _logicalSize.height);
        _ = density;
        _ = expectedWidth == paint.PixelWidth && expectedHeight == paint.PixelHeight;
#if !WINDOWS
        TimeSpan? nativeVsyncTimestamp = null;
#endif
#if ANDROID
        lock (_gate)
        {
            nativeVsyncTimestamp = _androidVsyncTimestamp;
            _androidVsyncTimestamp = null;
        }
#endif
#if !WINDOWS
        DispatchPendingFrame(nativeVsyncTimestamp ?? DorotiFrameClock.Now);
#endif
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
        if (dispatch) _surface.Dispatcher.Dispatch(_surface.InvalidateSurface);
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
#if WINDOWS
        // The HWND presenter owns a serial + AutoResetEvent coalescer and can
        // accept a wake from any thread. Reusing the TextureView-style
        // _invalidatePending gate here can lose the exact-scene wake after an
        // older scene consumed the prior raster pulse during a tight resize.
        _surface.InvalidateSurface();
        return;
#else
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
        _surface.Dispatcher.Dispatch(_surface.InvalidateSurface);
#endif
    }

#if ANDROID
    private void RequestAndroidVsync()
    {
        lock (_gate)
        {
            if (_disposed || _androidFrameCallbackPosted) return;
            _androidFrameCallbackPosted = true;
        }
        _surface.Dispatcher.Dispatch(PostAndroidFrameCallback);
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
        _surface.InvalidateSurface();
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
        _surface.Dispatcher.Dispatch(() =>
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
        _surface.Dispatcher.Dispatch(UpdateCompositionVsyncSubscription);

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
            // CompositionTarget already paces callbacks to the active display.
            // The Windows surface owns a latest-only DXGI/D3D12 pipeline, so an
            // additional fixed interval would halve 120/144/165 Hz resize
            // updates and make the content visibly trail the window border.
            if (_isPainting)
            {
                _invalidateAfterPaint = true;
                return;
            }
            _invalidatePending = true;
        }
        DispatchPendingFrame(timestamp);
        Interlocked.Increment(ref _invalidationsRequested);
        _surface.InvalidateSurface();
    }
#endif

    private void DispatchPendingFrame(TimeSpan timestamp)
    {
        Action<TimeSpan>? callback;
        lock (_gate)
        {
            callback = _pendingFrameCallback;
            _pendingFrameCallback = null;
        }
        if (callback is null) return;
        var now = DorotiFrameClock.ClampForward(timestamp, _lastVsyncTimestamp);
        _lastVsyncTimestamp = now;
        callback(now);
    }

#if WINDOWS
    private void DispatchWindowsResizeFrame()
    {
        lock (_gate)
        {
            if (_disposed || _pendingFrameCallback is null) return;
        }

        // SizeChanged is already a native, UI-thread-paced resize pulse. Build
        // its metrics frame now while raster prepares a detached exact staging chain.
        DispatchPendingFrame(DorotiFrameClock.Now);
        UnsubscribeFromCompositionVsync();
    }
#endif

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        _ = direction;
        _surface.RequestFocus(state == ViewFocusState.focused);
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

    public void SetCursor(DorotiMouseCursorKind cursor) => _surface.SetCursor(cursor);
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
        _surface.Pointer -= HandlePointer;
        _surface.Key -= HandleKey;
        _surface.SizeChanged -= HandleSizeChanged;
        _surface.FocusChanged -= HandleFocusChanged;
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

    private void HandleSizeChanged(DorotiResizeEpoch? publishedTarget)
    {
        DorotiResizeEpoch target;
        if (publishedTarget is not null)
        {
            target = publishedTarget;
        }
        else
        {
            if (_surface.Width <= 0 || _surface.Height <= 0) return;
            var surface = _surface.CaptureSnapshot(_snapshot);
            var density = Math.Max(1, surface.DevicePixelRatio);
            target = new(
                checked(_metricsGeneration + 1),
                _surface.Width,
                _surface.Height,
                Math.Max(0, checked((int)Math.Round(_surface.Width * density))),
                Math.Max(0, checked((int)Math.Round(_surface.Height * density))),
                density,
                DorotiFrameClock.Now.Ticks / 10);
        }
        if (!target.HasDrawableSize) return;
        var current = ViewEpoch;
        if (current.ResizeTargetGeneration == target.Generation &&
            current.LogicalWidth == target.LogicalWidth &&
            current.LogicalHeight == target.LogicalHeight &&
            current.PhysicalWidth == target.PhysicalWidth &&
            current.PhysicalHeight == target.PhysicalHeight &&
            current.DeviceScaleX == target.DeviceScaleX &&
            current.DeviceScaleY == target.DeviceScaleY) return;
        _logicalSize = new(target.LogicalWidth, target.LogicalHeight);
        _density = target.DevicePixelRatio;
        _metricsGeneration++;
        Volatile.Write(ref _viewEpoch, ToViewEpoch(target, _metricsGeneration));
        MetricsChanged?.Invoke(Metrics);
#if WINDOWS
        DispatchWindowsResizeFrame();
#else
        RequestInvalidate();
#endif
    }

    private DorotiViewEpoch ToViewEpoch(DorotiResizeEpoch target, long metricsGeneration) => new(
        _viewId,
        target.Generation,
        metricsGeneration,
        target.LogicalWidth,
        target.LogicalHeight,
        target.PhysicalWidth,
        target.PhysicalHeight,
        target.DeviceScaleX,
        target.DeviceScaleY,
        target.TimestampMicroseconds);

    private static DorotiResizeEpoch ToResizeEpoch(DorotiViewEpoch epoch) => new(
        epoch.ResizeTargetGeneration,
        epoch.LogicalWidth,
        epoch.LogicalHeight,
        epoch.PhysicalWidth,
        epoch.PhysicalHeight,
        epoch.DeviceScaleX,
        epoch.DeviceScaleY,
        epoch.TimestampMicroseconds);

    private void HandlePointer(MauiSurfacePointerData args)
    {
        Interlocked.Increment(ref _nativePointerEvents);
        var inputSequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = args.Timestamp;
        InputReceived?.Invoke(inputSequence, timestamp);
        var change = args.Change;
        var pointer = args.Pointer;
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
        var x = args.X;
        var y = args.Y;
        PointerData?.Invoke(new([new(_viewId, timestamp, change,
            args.Kind, pointer, x, y,
            hasPrevious ? x - previous.X : 0, hasPrevious ? y - previous.Y : 0, args.Buttons,
            scrollDeltaX: args.ScrollDeltaX, scrollDeltaY: args.ScrollDeltaY,
            signalKind: args.SignalKind,
            pointerIdentifier: pointer, pressure: args.Pressure, pressureMin: 0, pressureMax: 1) ]));
        if (change is PointerChange.remove or PointerChange.cancel) _pointerPositions.Remove(pointer);
        else _pointerPositions[pointer] = (x, y);
    }

    private void HandleKey(KeyData data) => KeyData?.Invoke(data);

    private void HandleFocusChanged(bool focused) =>
        FocusData?.Invoke(new(_viewId, focused, DorotiFrameClock.Now));

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
