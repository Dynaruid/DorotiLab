using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Host.Shared;
using Doroti.Skia.Rendering;
using Doroti.Ui;

namespace Doroti.Host.Qt;

internal sealed unsafe class QtHostAdapter
    : IViewHostCapability,
        IFrameHostCapability,
        IInputHostCapability,
        IViewFocusRequestCapability,
        ITextInputHostCapability,
        IPlatformEnvironmentHostCapability,
        IPlatformServicesHostCapability,
        IUrlLauncherHostCapability,
        ISkiaSceneRendererHost
{
    private delegate void TextStateCallback(QtNativeV2.TextState* state);
    private readonly object _gate = new();
    private readonly nint _viewHandle;
    private readonly ulong _viewId;
    private readonly QtNativeV2.HostApi _hostApi;
    private readonly nint _submitSemantics;
    private readonly NativeSemanticsTransport _semantics = new(qt: true);
    private Action<TimeSpan>? _pendingFrame;
    private readonly Dictionary<ulong, TaskCompletionSource<string?>> _clipboardRequests = [];
    private readonly Dictionary<long, long> _pressedLogicalKeys = [];
    private ulong _nextFrameToken = 1UL << 63;
    private ulong _nextClipboardRequest;
    private long _metricsGeneration;
    private ulong _nativeMetricsGeneration;
    private long _inputSequence;
    private long _nativeClockOriginMicroseconds = -1;
    private TimeSpan _dorotiClockOrigin;
    private bool _disposed;

    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_request_focus_v2")]
    private static extern int RequestNativeFocus(nint viewHandle);

    internal QtHostAdapter(
        nint viewHandle,
        in QtNativeV2.HostApi hostApi,
        int logicalWidth,
        int logicalHeight, ulong viewId, nint submitSemantics
    )
    {
        _viewId = viewId;
        _viewHandle = viewHandle;
        _hostApi = hostApi;
        _submitSemantics = submitSemantics;
        Metrics = new(
            new Size(logicalWidth, logicalHeight),
            1,
            ViewPadding.zero,
            ViewPadding.zero,
            ViewPadding.zero,
            AppLifecycleState.resumed,
            0,
            0
        );
        Configuration = new(
            ResolveLocales(),
            Brightness.light,
            false,
            false,
            HostOperatingSystem.linux
        );
    }

    public ValueTask<UrlLaunchResult> LaunchUrlAsync(
        string absoluteUrl,
        CancellationToken cancellationToken = default
    ) => QtUrlLauncher.LaunchUrlAsync(absoluteUrl, cancellationToken);

    private long _resizeGeneration;
    public ViewMetrics Metrics { get; private set; }
    internal bool SupportsIdleFrameElision => (_hostApi.FeatureBits & (1UL << 20)) != 0;
    public PlatformConfiguration Configuration { get; private set; }
    public long InputSequence => Volatile.Read(ref _inputSequence);
    public long SurfaceGeneration => Metrics.surfaceGeneration;
    public DorotiResizeEpoch ResizeTarget =>
        new(
            _resizeGeneration,
            Metrics.logicalSize.width,
            Metrics.logicalSize.height,
            Math.Max(0, checked((int)Math.Round(Metrics.physicalSize.width))),
            Math.Max(0, checked((int)Math.Round(Metrics.physicalSize.height))),
            Metrics.devicePixelRatio,
            DorotiFrameClock.Now.Ticks / 10
        );
    public DorotiViewEpoch ViewEpoch
    {
        get
        {
            var metrics = Metrics;
            var target = ResizeTarget;
            return new(
                _viewId,
                target.Generation,
                metrics.generation,
                target.LogicalWidth,
                target.LogicalHeight,
                target.PhysicalWidth,
                target.PhysicalHeight,
                target.DeviceScaleX,
                target.DeviceScaleY,
                target.TimestampMicroseconds
            );
        }
    }

    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public event Action<PointerDataPacket>? PointerData;
    public event Action<KeyData>? KeyData;
    public event Action<RawFocusData>? FocusData;
    public event Action<DorotiTextEditingState>? EditingStateChanged;
    public event Action<DorotiTextInputAction>? ActionPerformed;
    public event Action<PlatformConfiguration>? ConfigurationChanged;
    public event Action<int, SemanticsAction, object?>? SemanticsAction;
    public event Action<long, TimeSpan>? InputReceived;

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RequestInvalidate();
    }

    public void Resize(Size logicalSize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(logicalSize);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(logicalSize));
        }

        _hostApi.Resize(_viewHandle, logicalSize.width, logicalSize.height);
    }

    public void Close()
    {
        if (!_disposed)
        {
            _hostApi.RequestClose(_viewHandle);
        }
    }

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate)
        {
            _pendingFrame = callback;
        }

        RequestInvalidate();
    }

    public void RequestInvalidate()
    {
        if (_disposed)
        {
            return;
        }

        var token = Interlocked.Increment(ref _nextFrameToken);
        if (token == 0)
        {
            token = Interlocked.Increment(ref _nextFrameToken);
        }

        _hostApi.RequestFrame(_viewHandle, token);
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        // Framework focus listeners can run while a closed branch is unmounted.
        // Qt has already removed that native window before publishing detached.
        if (Metrics.lifecycleState == AppLifecycleState.detached) return;
        _ = direction;
        if (state == ViewFocusState.focused && (_hostApi.FeatureBits & (1UL << 19)) != 0)
        {
            var status = RequestNativeFocus(_viewHandle);
            if (status != 0)
                throw new InvalidOperationException($"Qt activation request failed (status {status}).");
        }
        // Only ApplyFocus, called from Qt's observed activate/deactivate event,
        // publishes the actual window focus state to the framework.
    }

    internal void BeginFrame(in QtNativeV2.Surface surface, bool prepareFramework = true)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var next = new ViewMetrics(
            new Size(surface.PixelWidth, surface.PixelHeight),
            surface.DevicePixelRatio,
            Metrics.viewPadding,
            Metrics.viewInsets,
            Metrics.systemGestureInsets,
            Metrics.lifecycleState,
            Interlocked.Increment(ref _metricsGeneration),
            checked((long)surface.SurfaceGeneration)
        )
        {
            gestureSettings = Metrics.gestureSettings,
        };
        if (
            next.physicalSize != Metrics.physicalSize
            || next.devicePixelRatio != Metrics.devicePixelRatio
            || next.surfaceGeneration != Metrics.surfaceGeneration
        )
        {
            if (
                next.physicalSize != Metrics.physicalSize
                || next.devicePixelRatio != Metrics.devicePixelRatio
            )
            {
                _resizeGeneration++;
            }

            Metrics = next.Validate();
            MetricsChanged?.Invoke(next);
        }
        Action<TimeSpan>? callback;
        if (!prepareFramework) return;
        lock (_gate)
        {
            callback = _pendingFrame;
            _pendingFrame = null;
        }
        callback?.Invoke(MapTimestamp(surface.TimestampMicroseconds));
    }

    internal void ApplyMetrics(in QtNativeV2.Metrics metrics)
    {
        if (metrics.MetricsGeneration < _nativeMetricsGeneration)
        {
            return;
        }

        _nativeMetricsGeneration = metrics.MetricsGeneration;
        var lifecycle = Enum.IsDefined((AppLifecycleState)metrics.LifecycleState)
            ? (AppLifecycleState)metrics.LifecycleState
            : AppLifecycleState.detached;
        var next = new ViewMetrics(
            new Size(metrics.PixelWidth, metrics.PixelHeight),
            metrics.DevicePixelRatio,
            metrics.ViewPadding,
            metrics.ViewInsets,
            metrics.SystemGestureInsets,
            lifecycle,
            Interlocked.Increment(ref _metricsGeneration),
            checked((long)metrics.SurfaceGeneration)
        )
        {
            gestureSettings = new(metrics.PhysicalTouchSlop > 0 ? metrics.PhysicalTouchSlop : null),
        };
        if (next != Metrics)
        {
            if (
                next.physicalSize != Metrics.physicalSize
                || next.devicePixelRatio != Metrics.devicePixelRatio
            )
            {
                _resizeGeneration++;
            }

            Metrics = next.Validate();
            MetricsChanged?.Invoke(next);
        }
    }

    internal void ApplyLifecycle(uint value)
    {
        if (!Enum.IsDefined((AppLifecycleState)value))
        {
            return;
        }

        var lifecycle = (AppLifecycleState)value;
        if (Metrics.lifecycleState != lifecycle)
        {
            Metrics = Metrics with { lifecycleState = lifecycle };
            LifecycleChanged?.Invoke(lifecycle);
        }
    }

    internal void RaiseCloseRequested() => CloseRequested?.Invoke();

    internal void RaiseClosed() => Closed?.Invoke();

    internal void ApplyPointer(in QtNativeV2.Pointer value)
    {
        if (
            !Enum.IsDefined((PointerChange)value.Change)
            || !Enum.IsDefined((PointerDeviceKind)value.Kind)
        )
        {
            return;
        }

        var sequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = MapTimestamp(value.TimestampMicroseconds);
        PointerData?.Invoke(
            new([
                new(
                    _viewId,
                    timestamp,
                    (PointerChange)value.Change,
                    (PointerDeviceKind)value.Kind,
                    value.Device,
                    value.PhysicalX,
                    value.PhysicalY,
                    value.PhysicalDeltaX,
                    value.PhysicalDeltaY,
                    value.Buttons,
                    value.ScrollDeltaX,
                    value.ScrollDeltaY,
                    Enum.IsDefined((PointerSignalKind)value.SignalKind)
                        ? (PointerSignalKind)value.SignalKind
                        : PointerSignalKind.unknown,
                    value.PointerIdentifier,
                    pressure: value.Pressure,
                    tilt: PenMeasurements.Tilt(value.Tilt),
                    orientation: PenMeasurements.Orientation(value.Orientation),
                    penSupport: value.Kind is 2 or 3 ? new(
                        (value.PenFields & 1) != 0 ? PenFieldSupport.Supported : PenFieldSupport.Unsupported,
                        (value.PenFields & 2) != 0 ? PenFieldSupport.Supported : PenFieldSupport.Unsupported,
                        (value.PenFields & 4) != 0 ? PenFieldSupport.Supported : PenFieldSupport.Unsupported,
                        PenOrientationReference.BarrelRotation) : null,
                    platformData: value.PlatformData,
                    panX: value.StructSize >= 168 ? value.PanX : 0,
                    panY: value.StructSize >= 168 ? value.PanY : 0,
                    panDeltaX: value.StructSize >= 168 ? value.PanDeltaX : 0,
                    panDeltaY: value.StructSize >= 168 ? value.PanDeltaY : 0,
                    scale: value.StructSize >= 168 ? value.Scale : 1,
                    rotation: value.StructSize >= 168 ? value.Rotation : 0
                ),
            ])
        );
        InputReceived?.Invoke(sequence, timestamp);
    }

    internal void ApplyKey(in QtNativeV2.Key value, string character)
    {
        if (!Enum.IsDefined((KeyEventType)value.Type))
        {
            return;
        }

        var sequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = MapTimestamp(value.TimestampMicroseconds);
        var eventType = (KeyEventType)value.Type;
        var eventCharacter =
            eventType == KeyEventType.up || string.IsNullOrEmpty(character) ? null : character;
        var physical = QtKeyMap.Physical(value.Physical, value.Logical);
        var logical = QtKeyMap.Logical(value.Logical, character);
        if (eventType == KeyEventType.up)
        {
            if (_pressedLogicalKeys.Remove(physical, out var pressedLogical))
            {
                logical = pressedLogical;
            }
        }
        else
        {
            if (_pressedLogicalKeys.TryGetValue(physical, out var pressedLogical))
            {
                logical = pressedLogical;
            }

            _pressedLogicalKeys[physical] = logical;
        }
        KeyData?.Invoke(new(_viewId, timestamp, eventType, physical, logical, false, eventCharacter));
        InputReceived?.Invoke(sequence, timestamp);
    }

    internal void ApplyFocus(bool focused, long timestampMicroseconds)
    {
        var timestamp = MapTimestamp(timestampMicroseconds);
        if (!focused)
        {
            foreach (var (physical, logical) in _pressedLogicalKeys)
            {
                KeyData?.Invoke(new(_viewId, timestamp, KeyEventType.up, physical, logical, true));
            }

            _pressedLogicalKeys.Clear();
        }
        FocusData?.Invoke(new(_viewId, focused, timestamp));
    }

    internal void ApplyTextEditing(
        string text,
        int selectionBase,
        int selectionExtent,
        int composingBase,
        int composingExtent
    )
    {
        DorotiTextSelection? composing =
            composingBase >= 0 && composingExtent >= composingBase
                ? new(composingBase, composingExtent)
                : null;
        EditingStateChanged?.Invoke(new(text, new(selectionBase, selectionExtent), composing));
    }

    internal void ApplyTextAction(uint action)
    {
        if (Enum.IsDefined((DorotiTextInputAction)action))
        {
            ActionPerformed?.Invoke((DorotiTextInputAction)action);
        }
    }

    internal void CompleteClipboard(ulong requestId, string text)
    {
        TaskCompletionSource<string?>? completion;
        lock (_gate)
        {
            _clipboardRequests.Remove(requestId, out completion);
        }
        completion?.TrySetResult(text);
    }

    internal void ApplyConfiguration(
        string languageTags,
        uint brightness,
        bool alwaysUse24HourFormat,
        bool highContrast = false
    )
    {
        var locales = Locale.ParseLanguageTags(languageTags.Split('\n'), CultureInfo.CurrentUICulture.Name);

        var next = Configuration with
        {
            locales = locales,
            platformBrightness = brightness == 0 ? Brightness.dark : Brightness.light,
            alwaysUse24HourFormat = alwaysUse24HourFormat,
            accessibilityFeatures = new(false, false, false, false, highContrast, false, false),
        };
        if (!next.HasSameValues(Configuration))
        {
            Configuration = next;
            ConfigurationChanged?.Invoke(next);
        }
    }

    public void SetClient(
        DorotiTextInputConfiguration configuration,
        DorotiTextEditingState initialState
    )
    {
        var native = new QtNativeV2.TextConfiguration(
            (uint)configuration.inputType,
            (uint)configuration.inputAction,
            (uint)configuration.textCapitalization,
            configuration.readOnly,
            configuration.obscureText,
            configuration.autocorrect,
            configuration.enableSuggestions
        );
        var nativePointer = &native;
        WithTextState(
            initialState,
            state => _hostApi.SetTextClient(_viewHandle, nativePointer, state)
        );
    }

    public TextInputFeatures Features => new(true, true, true, true);

    public void UpdateConfiguration(DorotiTextInputConfiguration configuration)
    {
        var native = new QtNativeV2.TextConfiguration((uint)configuration.inputType,
            (uint)configuration.inputAction, (uint)configuration.textCapitalization,
            configuration.readOnly, configuration.obscureText, configuration.autocorrect, configuration.enableSuggestions);
        _hostApi.UpdateTextConfiguration(_viewHandle, &native);
    }

    public void UpdateState(DorotiTextEditingState state) =>
        WithTextState(state, native => _hostApi.UpdateTextState(_viewHandle, native));

    public void SetCaretRect(Rect logicalRect) =>
        _hostApi.SetCaretRect(
            _viewHandle,
            logicalRect.left,
            logicalRect.top,
            logicalRect.width,
            logicalRect.height
        );

    public void ClearClient() => _hostApi.ClearTextClient(_viewHandle);

    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Native QClipboard replies arrive on the Qt GUI thread. Preserve that
        // thread for awaiting editing/status updates instead of forcing them
        // onto the ThreadPool (Qt has no managed SynchronizationContext).
        var completion = new TaskCompletionSource<string?>();
        ulong requestId;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            requestId = unchecked(++_nextClipboardRequest);
            if (requestId == 0)
            {
                requestId = unchecked(++_nextClipboardRequest);
            }

            _clipboardRequests.Add(requestId, completion);
        }
        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() =>
            {
                lock (_gate)
                {
                    _clipboardRequests.Remove(requestId);
                }

                completion.TrySetCanceled(cancellationToken);
            });
        }

        _hostApi.RequestClipboardText(_viewHandle, requestId);
        return new(completion.Task);
    }

    public ValueTask SetClipboardTextAsync(
        string text,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(text);
        cancellationToken.ThrowIfCancellationRequested();
        WithUtf8(text, value => _hostApi.SetClipboardText(_viewHandle, value));
        return ValueTask.CompletedTask;
    }

    public void SetCursor(DorotiMouseCursorKind cursor) =>
        _hostApi.SetCursor(_viewHandle, (uint)cursor);

    public void UpdateSemantics(SemanticsUpdate update)
    {
        lock (_gate)
        {
            if (_disposed) return;
            _semantics.Update(update, _submitSemantics == 0 ? null : SubmitSemantics, SendLegacySemantics);
        }
    }

    private uint SubmitSemantics(byte[] bytes, long revision, long baseRevision)
    {
        fixed (byte* data = bytes)
        {
            var callback = (delegate* unmanaged[Cdecl]<nint, QtNativeV2.Utf8, ulong, ulong, uint>)_submitSemantics;
            return callback(_viewHandle, new(data, (ulong)bytes.Length), (ulong)revision, (ulong)baseRevision);
        }
    }

    private void SendLegacySemantics(byte[] bytes)
    {
        fixed (byte* data = bytes)
            _hostApi.UpdateSemantics(_viewHandle, new(data, (ulong)bytes.Length));
    }

    public void ClearSemantics()
    {
        lock (_gate)
        {
            _semantics.Clear();
            if (!_disposed) _hostApi.ClearSemantics(_viewHandle);
        }
    }

    internal void ApplySemanticsAction(long nodeId, long action, string argumentsJson)
    {
        if (nodeId is < int.MinValue or > int.MaxValue)
        {
            return;
        }

        object? arguments = null;
        if (!string.IsNullOrWhiteSpace(argumentsJson) && argumentsJson != "null")
        {
            using var document = JsonDocument.Parse(argumentsJson);
            arguments = ConvertJson(document.RootElement);
        }
        SemanticsAction?.Invoke(checked((int)nodeId), (SemanticsAction)action, arguments);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        TaskCompletionSource<string?>[] clipboardRequests;
        lock (_gate)
        {
            _pendingFrame = null;
            clipboardRequests = _clipboardRequests.Values.ToArray();
            _clipboardRequests.Clear();
        }
        // Continuations can now run inline; never invoke them while enumerating
        // requests or holding the adapter lock.
        foreach (var request in clipboardRequests)
        {
            request.TrySetCanceled();
        }

        GC.KeepAlive(MetricsChanged);
        GC.KeepAlive(LifecycleChanged);
        GC.KeepAlive(CloseRequested);
        GC.KeepAlive(Closed);
        GC.KeepAlive(PointerData);
        GC.KeepAlive(KeyData);
        GC.KeepAlive(FocusData);
        GC.KeepAlive(EditingStateChanged);
        GC.KeepAlive(ActionPerformed);
        GC.KeepAlive(ConfigurationChanged);
        GC.KeepAlive(SemanticsAction);
        GC.KeepAlive(InputReceived);
    }

    private static IReadOnlyList<Locale> ResolveLocales() =>
        Locale.ParseLanguageTags([], CultureInfo.CurrentUICulture.Name);

    private TimeSpan MapTimestamp(long nativeMicroseconds)
    {
        lock (_gate)
        {
            if (_nativeClockOriginMicroseconds < 0)
            {
                _nativeClockOriginMicroseconds = nativeMicroseconds;
                _dorotiClockOrigin = DorotiFrameClock.Now;
            }
            var elapsed = Math.Max(0, nativeMicroseconds - _nativeClockOriginMicroseconds);
            return _dorotiClockOrigin + TimeSpan.FromTicks(elapsed * 10);
        }
    }

    private static void WithTextState(DorotiTextEditingState state, TextStateCallback callback)
    {
        WithUtf8(
            state.text,
            text =>
            {
                var composingBase = state.composingRange?.baseOffset ?? -1;
                var composingExtent = state.composingRange?.extentOffset ?? -1;
                var native = new QtNativeV2.TextState(
                    text,
                    state.selection.baseOffset,
                    state.selection.extentOffset,
                    composingBase,
                    composingExtent
                );
                callback(&native);
            }
        );
    }

    private static void WithUtf8(string value, Action<QtNativeV2.Utf8> callback)
    {
        var byteCount = System.Text.Encoding.UTF8.GetByteCount(value);
        Span<byte> bytes = byteCount <= 1024 ? stackalloc byte[byteCount] : new byte[byteCount];
        System.Text.Encoding.UTF8.GetBytes(value, bytes);
        fixed (byte* data = bytes)
        {
            callback(new QtNativeV2.Utf8(data, checked((ulong)bytes.Length)));
        }
    }

    private static object? ConvertJson(JsonElement value) =>
        value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number when value.TryGetInt64(out var integer) => integer,
            JsonValueKind.Number => value.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => value.EnumerateArray().Select(ConvertJson).ToArray(),
            JsonValueKind.Object => value
                .EnumerateObject()
                .ToDictionary(
                    property => property.Name,
                    property => ConvertJson(property.Value),
                    StringComparer.Ordinal
                ),
            _ => null,
        };
}
