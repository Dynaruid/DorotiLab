using System.Globalization;
using System.Text.Json;
using Doroti.Skia.Rendering;
using Doroti.Ui;

namespace Doroti.Host.Qt;

internal sealed unsafe class QtHostAdapter :
    IViewHostCapability,
    IFrameHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    ITextInputHostCapability,
    IPlatformEnvironmentHostCapability,
    IPlatformServicesHostCapability,
    ISkiaSceneRendererHost
{
    private delegate void TextStateCallback(QtNativeV2.TextState* state);
    private readonly object _gate = new();
    private readonly nint _viewHandle;
    private readonly QtNativeV2.HostApi _hostApi;
    private Action<TimeSpan>? _pendingFrame;
    private readonly Dictionary<ulong, TaskCompletionSource<string?>> _clipboardRequests = [];
    private ulong _nextFrameToken = 1UL << 63;
    private ulong _nextClipboardRequest;
    private long _metricsGeneration;
    private long _inputSequence;
    private long _nativeClockOriginMicroseconds = -1;
    private TimeSpan _dorotiClockOrigin;
    private bool _disposed;

    internal QtHostAdapter(nint viewHandle, in QtNativeV2.HostApi hostApi, int logicalWidth, int logicalHeight)
    {
        _viewHandle = viewHandle;
        _hostApi = hostApi;
        Metrics = new(new Size(logicalWidth, logicalHeight), 1, ViewPadding.zero,
            ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
        Configuration = new(ResolveLocales(), Brightness.light, false, false, HostOperatingSystem.linux);
    }

    public ViewMetrics Metrics { get; private set; }
    public PlatformConfiguration Configuration { get; private set; }
    public long InputSequence => Volatile.Read(ref _inputSequence);
    public long SurfaceGeneration => Metrics.surfaceGeneration;
    public long MetricsGeneration => Metrics.generation;
    public DorotiResizeEpoch ResizeTarget => new(
        Metrics.generation,
        Metrics.logicalSize.width,
        Metrics.logicalSize.height,
        Math.Max(0, checked((int)Math.Round(Metrics.physicalSize.width))),
        Math.Max(0, checked((int)Math.Round(Metrics.physicalSize.height))),
        Metrics.devicePixelRatio,
        DorotiFrameClock.Now.Ticks / 10);

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
        if (!logicalSize.IsFinite || logicalSize.IsEmpty) throw new ArgumentOutOfRangeException(nameof(logicalSize));
        _hostApi.Resize(_viewHandle, logicalSize.width, logicalSize.height);
    }

    public void Close()
    {
        if (!_disposed) _hostApi.RequestClose(_viewHandle);
    }

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate) _pendingFrame = callback;
        RequestInvalidate();
    }

    public void RequestInvalidate()
    {
        if (_disposed) return;
        var token = unchecked(++_nextFrameToken);
        if (token == 0) token = unchecked(++_nextFrameToken);
        _hostApi.RequestFrame(_viewHandle, token);
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _ = direction;
        FocusData?.Invoke(new(1, state == ViewFocusState.focused, DorotiFrameClock.Now));
    }

    internal void BeginFrame(in QtNativeV2.Surface surface)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var next = new ViewMetrics(new Size(surface.PixelWidth, surface.PixelHeight),
            surface.DevicePixelRatio, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero,
            Metrics.lifecycleState, Interlocked.Increment(ref _metricsGeneration),
            checked((long)surface.SurfaceGeneration));
        if (next.physicalSize != Metrics.physicalSize || next.devicePixelRatio != Metrics.devicePixelRatio ||
            next.surfaceGeneration != Metrics.surfaceGeneration)
        {
            Metrics = next;
            MetricsChanged?.Invoke(next);
        }
        Action<TimeSpan>? callback;
        lock (_gate)
        {
            callback = _pendingFrame;
            _pendingFrame = null;
        }
        callback?.Invoke(MapTimestamp(surface.TimestampMicroseconds));
    }

    internal void ApplyMetrics(in QtNativeV2.Metrics metrics)
    {
        var lifecycle = Enum.IsDefined((AppLifecycleState)metrics.LifecycleState)
            ? (AppLifecycleState)metrics.LifecycleState
            : AppLifecycleState.detached;
        var next = new ViewMetrics(new Size(metrics.PixelWidth, metrics.PixelHeight),
            metrics.DevicePixelRatio, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero,
            lifecycle, checked((long)metrics.MetricsGeneration), checked((long)metrics.SurfaceGeneration));
        if (next != Metrics)
        {
            Metrics = next;
            MetricsChanged?.Invoke(next);
        }
    }

    internal void ApplyLifecycle(uint value)
    {
        if (!Enum.IsDefined((AppLifecycleState)value)) return;
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
        if (!Enum.IsDefined((PointerChange)value.Change) || !Enum.IsDefined((PointerDeviceKind)value.Kind)) return;
        var sequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = MapTimestamp(value.TimestampMicroseconds);
        PointerData?.Invoke(new([
            new(1, timestamp, (PointerChange)value.Change, (PointerDeviceKind)value.Kind,
                value.Device, value.PhysicalX, value.PhysicalY, value.PhysicalDeltaX,
                value.PhysicalDeltaY, value.Buttons, value.ScrollDeltaX, value.ScrollDeltaY,
                Enum.IsDefined((PointerSignalKind)value.SignalKind)
                    ? (PointerSignalKind)value.SignalKind : PointerSignalKind.unknown,
                value.PointerIdentifier, pressure: value.Pressure, tilt: value.Tilt,
                platformData: value.PlatformData)
        ]));
        InputReceived?.Invoke(sequence, timestamp);
    }

    internal void ApplyKey(in QtNativeV2.Key value, string character)
    {
        if (!Enum.IsDefined((KeyEventType)value.Type)) return;
        var sequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = MapTimestamp(value.TimestampMicroseconds);
        var eventType = (KeyEventType)value.Type;
        var eventCharacter = eventType == KeyEventType.up || string.IsNullOrEmpty(character)
            ? null : character;
        KeyData?.Invoke(new(1, timestamp, (KeyEventType)value.Type,
            QtKeyMap.Physical(value.Physical, value.Logical), QtKeyMap.Logical(value.Logical, character),
            false, eventCharacter));
        InputReceived?.Invoke(sequence, timestamp);
    }

    internal void ApplyFocus(bool focused, long timestampMicroseconds) =>
        FocusData?.Invoke(new(1, focused, MapTimestamp(timestampMicroseconds)));

    internal void ApplyTextEditing(string text, int selectionBase, int selectionExtent,
        int composingBase, int composingExtent)
    {
        DorotiTextSelection? composing = composingBase >= 0 && composingExtent >= composingBase
            ? new(composingBase, composingExtent) : null;
        EditingStateChanged?.Invoke(new(text, new(selectionBase, selectionExtent), composing));
    }

    internal void ApplyTextAction(uint action)
    {
        if (Enum.IsDefined((DorotiTextInputAction)action))
            ActionPerformed?.Invoke((DorotiTextInputAction)action);
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

    internal void ApplyConfiguration(string languageTags, uint brightness, bool alwaysUse24HourFormat)
    {
        var locales = languageTags.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseLocale).ToArray();
        if (locales.Length == 0) locales = ResolveLocales().ToArray();
        var next = Configuration with
        {
            locales = locales,
            platformBrightness = brightness == 0 ? Brightness.dark : Brightness.light,
            alwaysUse24HourFormat = alwaysUse24HourFormat,
        };
        if (next != Configuration)
        {
            Configuration = next;
            ConfigurationChanged?.Invoke(next);
        }
    }

    public void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState)
    {
        var native = new QtNativeV2.TextConfiguration((uint)configuration.inputType,
            (uint)configuration.inputAction, (uint)configuration.textCapitalization,
            configuration.readOnly, configuration.obscureText, configuration.autocorrect,
            configuration.enableSuggestions);
        var nativePointer = &native;
        WithTextState(initialState, state => _hostApi.SetTextClient(_viewHandle, nativePointer, state));
    }

    public void UpdateState(DorotiTextEditingState state) =>
        WithTextState(state, native => _hostApi.UpdateTextState(_viewHandle, native));

    public void SetCaretRect(Rect logicalRect) =>
        _hostApi.SetCaretRect(_viewHandle, logicalRect.left, logicalRect.top,
            logicalRect.width, logicalRect.height);

    public void ClearClient() => _hostApi.ClearTextClient(_viewHandle);

    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var completion = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        ulong requestId;
        lock (_gate)
        {
            requestId = unchecked(++_nextClipboardRequest);
            if (requestId == 0) requestId = unchecked(++_nextClipboardRequest);
            _clipboardRequests.Add(requestId, completion);
        }
        if (cancellationToken.CanBeCanceled)
            cancellationToken.Register(() =>
            {
                lock (_gate) _clipboardRequests.Remove(requestId);
                completion.TrySetCanceled(cancellationToken);
            });
        _hostApi.RequestClipboardText(_viewHandle, requestId);
        return new(completion.Task);
    }

    public ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(text);
        cancellationToken.ThrowIfCancellationRequested();
        WithUtf8(text, value => _hostApi.SetClipboardText(_viewHandle, value));
        return ValueTask.CompletedTask;
    }

    public void SetCursor(DorotiMouseCursorKind cursor) => _hostApi.SetCursor(_viewHandle, (uint)cursor);
    public void UpdateSemantics(SemanticsUpdate update)
    {
        var nodes = update.nodes.Select(node => new
        {
            node.id, node.label, node.value, role = node.role.ToString(),
            actions = (long)node.actions, children = node.children,
            flags = node.flags is null ? null : new
            {
                selected = node.flags.isSelected.toBoolOrNull(),
                enabled = node.flags.isEnabled.toBoolOrNull(),
                focused = node.flags.isFocused.toBoolOrNull(),
                button = node.flags.isButton, textField = node.flags.isTextField,
                header = node.flags.isHeader, hidden = node.flags.isHidden,
                image = node.flags.isImage, slider = node.flags.isSlider,
                readOnly = node.flags.isReadOnly,
            },
            node.textSelectionBase, node.textSelectionExtent,
            rect = new[] { node.rect.left, node.rect.top, node.rect.right, node.rect.bottom },
        });
        WithUtf8(JsonSerializer.Serialize(new { generation = update.generation, nodes }),
            json => _hostApi.UpdateSemantics(_viewHandle, json));
    }

    public void ClearSemantics() => _hostApi.ClearSemantics(_viewHandle);

    internal void ApplySemanticsAction(long nodeId, long action, string argumentsJson)
    {
        if (nodeId is < int.MinValue or > int.MaxValue) return;
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
        if (_disposed) return;
        _disposed = true;
        lock (_gate)
        {
            _pendingFrame = null;
            foreach (var request in _clipboardRequests.Values) request.TrySetCanceled();
            _clipboardRequests.Clear();
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

    private static IReadOnlyList<Locale> ResolveLocales()
    {
        var parts = CultureInfo.CurrentUICulture.Name.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return [new Locale(parts.FirstOrDefault() ?? "en", parts.Skip(1).FirstOrDefault())];
    }

    private static Locale ParseLocale(string tag)
    {
        var parts = tag.Split('-', StringSplitOptions.RemoveEmptyEntries);
        var script = parts.FirstOrDefault(part => part.Length == 4);
        var country = parts.Skip(1).FirstOrDefault(part => part.Length is 2 or 3 && part != script);
        return new(parts.FirstOrDefault() ?? "en", country, script);
    }

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
        WithUtf8(state.text, text =>
        {
            var composingBase = state.composingRange?.baseOffset ?? -1;
            var composingExtent = state.composingRange?.extentOffset ?? -1;
            var native = new QtNativeV2.TextState(text, state.selection.baseOffset,
                state.selection.extentOffset, composingBase, composingExtent);
            callback(&native);
        });
    }

    private static void WithUtf8(string value, Action<QtNativeV2.Utf8> callback)
    {
        var byteCount = System.Text.Encoding.UTF8.GetByteCount(value);
        Span<byte> bytes = byteCount <= 1024 ? stackalloc byte[byteCount] : new byte[byteCount];
        System.Text.Encoding.UTF8.GetBytes(value, bytes);
        fixed (byte* data = bytes)
            callback(new QtNativeV2.Utf8(data, checked((ulong)bytes.Length)));
    }

    private static object? ConvertJson(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString(),
        JsonValueKind.Number when value.TryGetInt64(out var integer) => integer,
        JsonValueKind.Number => value.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Array => value.EnumerateArray().Select(ConvertJson).ToArray(),
        JsonValueKind.Object => value.EnumerateObject().ToDictionary(
            property => property.Name, property => ConvertJson(property.Value), StringComparer.Ordinal),
        _ => null,
    };
}
