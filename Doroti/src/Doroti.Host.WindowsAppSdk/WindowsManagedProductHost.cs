using System.Globalization;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Doroti.Skia.Rendering;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe class WindowsManagedProductHost :
    IViewHostCapability,
    IFrameHostCapability,
    ILatestMetricsFrameHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    IPlatformServicesHostCapability,
    IPlatformEnvironmentHostCapability,
    ITextInputHostCapability,
    ISkiaSceneRendererHost
{
    private readonly object _gate = new();
    private readonly WindowsNativeV1.Host _native;
    private readonly WindowsManagedResizeCoordinator _coordinator = new(TimeSpan.FromMilliseconds(100));
    private readonly HashSet<long> _resizeTerminalGenerations = [];
    private readonly Dictionary<ulong, TaskCompletionSource<string?>> _clipboardRequests = [];
    private readonly Queue<Action> _pendingInput = [];
    private readonly Dictionary<long, long> _pressedLogicalKeys = [];
    private Action<TimeSpan, DorotiViewEpoch>? _pendingFrame;
    private bool _disposed;
    private long _inputSequence;
    private bool _nativeActive = true;
    private ulong _nextClipboardRequest;
    private long _nativeClockOrigin = -1;
    private TimeSpan _dorotiClockOrigin;

    internal WindowsManagedProductHost(in WindowsNativeV1.Host native, int logicalWidth, int logicalHeight)
    {
        if (native.AbiVersion != WindowsNativeV1.AbiVersion ||
            native.StructSize < sizeof(WindowsNativeV1.Host) ||
            native.HostContext == 0 || native.TopLevelHwnd == 0 || native.ChildHwnd == 0 ||
            native.OpaqueChildHwnd == 0 || native.TaskHwnd == 0 ||
            native.RequestFrame == 0 || native.RequestResize == 0 || native.RequestClose == 0 || native.RequestShow == 0 ||
            native.RequestOpaqueFallback == 0 ||
            native.SetCompositionChild == 0 ||
            native.SetCursor == 0 || native.SetClipboard == 0 || native.RequestClipboard == 0 ||
            native.SetTextClient == 0 || native.UpdateTextState == 0 || native.SetCaretRect == 0 ||
            native.ClearTextClient == 0 || native.UpdateSemantics == 0 || native.ClearSemantics == 0 ||
            !Enum.IsDefined((Brightness)native.InitialPlatformBrightness))
            throw new InvalidDataException("The native product host table is invalid.");
        _native = native;
        Metrics = new(new Size(logicalWidth, logicalHeight), 1, ViewPadding.zero,
            ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
        Configuration = new(ResolveLocales(), (Brightness)native.InitialPlatformBrightness,
            false, false, HostOperatingSystem.windows);
    }

    internal nint ChildHwnd => _native.ChildHwnd;
    internal nint TopLevelHwnd => _native.TopLevelHwnd;
    internal WindowsResizeCoordinatorSnapshot ResizeSnapshot => _coordinator.Snapshot();
    internal bool IsLatestResizeGeneration(ulong generation) =>
        generation <= long.MaxValue && _coordinator.IsLatest((long)generation);
    internal bool IsInputSequenceCurrent(long inputSequence) => inputSequence >= InputSequence;
    public ViewMetrics Metrics { get; private set; }
    public PlatformConfiguration Configuration { get; private set; }
    public long InputSequence => Volatile.Read(ref _inputSequence);
    public long SurfaceGeneration => Metrics.surfaceGeneration;
    public DorotiResizeEpoch ResizeTarget => new(
        Metrics.generation,
        Metrics.logicalSize.width,
        Metrics.logicalSize.height,
        checked((int)Math.Round(Metrics.physicalSize.width)),
        checked((int)Math.Round(Metrics.physicalSize.height)),
        Metrics.devicePixelRatio,
        DorotiFrameClock.Now.Ticks / 10);
    public DorotiViewEpoch ViewEpoch
    {
        get
        {
            var target = ResizeTarget;
            return new(1, target.Generation, Metrics.generation,
                target.LogicalWidth, target.LogicalHeight,
                target.PhysicalWidth, target.PhysicalHeight,
                target.DeviceScaleX, target.DeviceScaleY,
                target.TimestampMicroseconds);
        }
    }

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
    public event Action<int, SemanticsAction, object?>? SemanticsAction;
    public event Action<long, TimeSpan>? InputReceived;

    internal void ApplyMetrics(in WindowsNativeV1.Metrics metrics)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (metrics.AbiVersion != WindowsNativeV1.AbiVersion ||
            metrics.StructSize < sizeof(WindowsNativeV1.Metrics) || metrics.ViewId != 1 ||
            metrics.Generation == 0 || metrics.WidthPx == 0 || metrics.HeightPx == 0 ||
            !double.IsFinite(metrics.Scale) || metrics.Scale <= 0)
            throw new InvalidDataException("Native product metrics are invalid.");
        var target = _coordinator.Publish(1, checked((int)metrics.WidthPx), checked((int)metrics.HeightPx),
            metrics.Scale, 0, checked((long)metrics.Generation));
        if (target.Generation != checked((long)metrics.Generation))
            throw new InvalidDataException("Native and managed resize generations diverged.");
        var next = new ViewMetrics(new Size(metrics.WidthPx, metrics.HeightPx), metrics.Scale,
            ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed,
            checked((long)metrics.Generation), checked((long)metrics.Generation));
        Metrics = next;
        MetricsChanged?.Invoke(next);
    }

    internal bool BeginFrame(in WindowsNativeV1.FrameRequest request)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (request.AbiVersion != WindowsNativeV1.AbiVersion ||
            request.StructSize < sizeof(WindowsNativeV1.FrameRequest) || request.ViewId != 1 ||
            !_coordinator.ValidateExact(checked((long)request.Generation),
                checked((int)request.WidthPx), checked((int)request.HeightPx)))
            throw new InvalidDataException("Native frame request failed exact admission.");
        Action<TimeSpan, DorotiViewEpoch>? callback;
        while (true)
        {
            Action[] input;
            lock (_gate)
            {
                input = [.. _pendingInput];
                _pendingInput.Clear();
                if (input.Length == 0)
                {
                    // Input dispatch can schedule the scene needed by this
                    // native render request. Take the callback only after all
                    // input already queued ahead of it has been applied; taking
                    // it before dispatch would present the retained pre-input
                    // scene once, which is visible as a wheel-scroll flash.
                    callback = _pendingFrame;
                    _pendingFrame = null;
                    break;
                }
            }
            foreach (var dispatch in input) dispatch();
        }
        callback?.Invoke(DorotiFrameClock.Now, ViewEpoch);
        return callback is not null;
    }

    internal void CompleteTerminal(in WindowsNativeV1.FrameTerminal terminal)
    {
        if (terminal.AbiVersion != WindowsNativeV1.AbiVersion ||
            terminal.StructSize < sizeof(WindowsNativeV1.FrameTerminal) || terminal.ViewId != 1)
            throw new InvalidDataException("Native frame terminal is invalid.");
        var kind = (WindowsNativeV1.FrameTerminalKind)terminal.TerminalKind switch
        {
            WindowsNativeV1.FrameTerminalKind.Presented => WindowsResizeTerminal.Presented,
            WindowsNativeV1.FrameTerminalKind.Superseded => WindowsResizeTerminal.Superseded,
            WindowsNativeV1.FrameTerminalKind.Failed => WindowsResizeTerminal.Failed,
            _ => throw new InvalidDataException($"Unknown native terminal {terminal.TerminalKind}."),
        };
        var generation = checked((long)terminal.Generation);
        lock (_gate)
        {
            if (!_resizeTerminalGenerations.Add(generation)) return;
        }
        if (_coordinator.IsComplete(generation)) return;
        if (!_coordinator.TryComplete(
                generation, kind, $"native causal frame {terminal.CausalFrameId}", enforceLatest: false))
            throw new InvalidDataException($"Resize generation {terminal.Generation} received an invalid terminal.");
    }

    internal void ApplyPointer(in WindowsNativeV1.Pointer value)
    {
        if (value.AbiVersion != WindowsNativeV1.AbiVersion || value.StructSize < sizeof(WindowsNativeV1.Pointer) ||
            value.ViewId != 1 || !Enum.IsDefined((PointerChange)value.Change) ||
            !Enum.IsDefined((PointerDeviceKind)value.Kind) || !Enum.IsDefined((PointerSignalKind)value.SignalKind) ||
            value.Device < 0)
            throw new InvalidDataException("Native pointer packet is invalid.");
        var sequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = MapTimestamp(value.TimestampQpc);
        var packet = new PointerDataPacket([
            new(1, timestamp, (PointerChange)value.Change, (PointerDeviceKind)value.Kind,
                checked((ulong)value.Device), value.PhysicalX, value.PhysicalY,
                value.PhysicalDeltaX, value.PhysicalDeltaY, value.Buttons,
                value.ScrollDeltaX, value.ScrollDeltaY, (PointerSignalKind)value.SignalKind,
                value.PointerIdentifier, pressure: value.Pressure, tilt: value.Tilt,
                platformData: value.PlatformData)
        ]);
        EnqueueInput(() =>
        {
            PointerData?.Invoke(packet);
            InputReceived?.Invoke(sequence, timestamp);
        });
    }

    internal void ApplyKey(in WindowsNativeV1.Key value, string character)
    {
        if (value.AbiVersion != WindowsNativeV1.AbiVersion || value.StructSize < sizeof(WindowsNativeV1.Key) ||
            value.ViewId != 1 || !Enum.IsDefined((KeyEventType)value.Type))
            throw new InvalidDataException("Native key packet is invalid.");
        var sequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = MapTimestamp(value.TimestampQpc);
        var type = (KeyEventType)value.Type;
        var physical = WindowsKeyMap.Physical(value.Physical, value.Logical);
        var logical = WindowsKeyMap.Logical(value.Physical, value.Logical, character);
        if (type == KeyEventType.up)
        {
            if (_pressedLogicalKeys.Remove(physical, out var pressedLogical)) logical = pressedLogical;
        }
        else _pressedLogicalKeys[physical] = logical;
        var key = new KeyData(1, timestamp, type,
            physical, logical, false,
            value.Type == (uint)KeyEventType.up || string.IsNullOrEmpty(character) ? null : character);
        EnqueueInput(() =>
        {
            KeyData?.Invoke(key);
            InputReceived?.Invoke(sequence, timestamp);
        });
    }

    internal void ApplyFocus(bool focused, long timestampQpc)
    {
        var data = new RawFocusData(1, focused, MapTimestamp(timestampQpc));
        EnqueueInput(() => FocusData?.Invoke(data));
    }

    internal void ApplyLifecycle(uint value)
    {
        if (!Enum.IsDefined((AppLifecycleState)value))
            throw new InvalidDataException($"Native lifecycle state {value} is invalid.");
        var lifecycle = (AppLifecycleState)value;
        if (Metrics.lifecycleState == lifecycle) return;
        Metrics = Metrics with { lifecycleState = lifecycle };
        LifecycleChanged?.Invoke(lifecycle);
    }

    internal void ApplyPlatformBrightness(uint value)
    {
        if (!Enum.IsDefined((Brightness)value))
            throw new InvalidDataException($"Native platform brightness {value} is invalid.");
        var brightness = (Brightness)value;
        if (Configuration.platformBrightness == brightness) return;
        Configuration = Configuration with { platformBrightness = brightness };
        ConfigurationChanged?.Invoke(Configuration);
    }

    internal void CompleteClipboard(ulong requestId, string text)
    {
        TaskCompletionSource<string?>? completion;
        lock (_gate) _clipboardRequests.Remove(requestId, out completion);
        completion?.TrySetResult(text);
    }

    internal void ApplyTextEditing(string text, int selectionBase, int selectionExtent,
        int composingBase, int composingExtent)
    {
        if (selectionBase < 0 || selectionBase > text.Length || selectionExtent < 0 || selectionExtent > text.Length ||
            !((composingBase == -1 && composingExtent == -1) ||
              (composingBase >= 0 && composingBase <= text.Length && composingExtent >= 0 && composingExtent <= text.Length)))
            throw new InvalidDataException("Native text editing ranges are invalid.");
        DorotiTextSelection? composing = composingBase >= 0 ? new(composingBase, composingExtent) : null;
        EnqueueInput(() => EditingStateChanged?.Invoke(
            new(text, new(selectionBase, selectionExtent), composing)));
    }

    internal void ApplyTextAction(uint action)
    {
        if (!Enum.IsDefined((DorotiTextInputAction)action))
            throw new InvalidDataException($"Native text action {action} is invalid.");
        EnqueueInput(() => ActionPerformed?.Invoke((DorotiTextInputAction)action));
    }

    internal void ApplySemanticsAction(long nodeId, long action, string argumentsJson)
    {
        if (nodeId is < int.MinValue or > int.MaxValue || action == 0)
            throw new InvalidDataException("Native semantics action is invalid.");
        object? arguments = null;
        if (!string.IsNullOrWhiteSpace(argumentsJson) && argumentsJson != "null")
        {
            using var document = JsonDocument.Parse(argumentsJson);
            arguments = document.RootElement.Clone();
        }
        var node = checked((int)nodeId);
        EnqueueInput(() => SemanticsAction?.Invoke(node, (SemanticsAction)action, arguments));
    }

    public void Show() => Invoke(_native.RequestShow);
    public void Close()
    {
        if (!_disposed && _nativeActive) Invoke(_native.RequestClose);
    }

    public void Resize(Size logicalSize)
    {
        ArgumentNullException.ThrowIfNull(logicalSize);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty) throw new ArgumentOutOfRangeException(nameof(logicalSize));
        var width = checked((uint)Math.Round(logicalSize.width * Metrics.devicePixelRatio));
        var height = checked((uint)Math.Round(logicalSize.height * Metrics.devicePixelRatio));
        var resize = (delegate* unmanaged[Cdecl]<nint, uint, uint, uint>)_native.RequestResize;
        var status = resize(_native.HostContext, width, height);
        if (status != 0) throw new InvalidOperationException($"Native resize request failed: {status}.");
    }

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ScheduleFrame(ViewEpoch, (timestamp, _) => callback(timestamp));
    }

    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ScheduleFrame(expectedEpoch, (timestamp, _) => callback(timestamp));
    }

    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan, DorotiViewEpoch> callback)
    {
        ArgumentNullException.ThrowIfNull(expectedEpoch);
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate) _pendingFrame = callback;
        RequestInvalidate();
    }

    public void RequestInvalidate()
    {
        if (!_disposed && _nativeActive) Invoke(_native.RequestFrame);
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        _ = direction;
        ApplyFocus(state == ViewFocusState.focused, 0);
    }

    public void SetCursor(DorotiMouseCursorKind cursor)
    {
        var function = (delegate* unmanaged[Cdecl]<nint, uint, uint>)_native.SetCursor;
        var status = function(_native.HostContext, (uint)cursor);
        if (status != 0) throw new InvalidOperationException($"Native cursor request failed: {status}.");
    }

    public ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(text);
        cancellationToken.ThrowIfCancellationRequested();
        var count = System.Text.Encoding.UTF8.GetByteCount(text);
        Span<byte> bytes = count <= 1024 ? stackalloc byte[count] : new byte[count];
        System.Text.Encoding.UTF8.GetBytes(text, bytes);
        fixed (byte* data = bytes)
        {
            var value = new WindowsNativeV1.Utf8
            {
                AbiVersion = WindowsNativeV1.AbiVersion,
                StructSize = checked((uint)sizeof(WindowsNativeV1.Utf8)),
                Data = (nint)data,
                ByteLength = checked((ulong)bytes.Length),
            };
            var function = (delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.Utf8, uint>)_native.SetClipboard;
            var status = function(_native.HostContext, value);
            if (status != 0) throw new InvalidOperationException($"Native clipboard write failed: {status}.");
        }
        return ValueTask.CompletedTask;
    }

    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var completion = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        ulong requestId;
        lock (_gate)
        {
            requestId = ++_nextClipboardRequest;
            _clipboardRequests.Add(requestId, completion);
        }
        var function = (delegate* unmanaged[Cdecl]<nint, ulong, uint>)_native.RequestClipboard;
        var status = function(_native.HostContext, requestId);
        if (status != 0)
        {
            lock (_gate) _clipboardRequests.Remove(requestId);
            throw new InvalidOperationException($"Native clipboard read failed: {status}.");
        }
        return new(completion.Task);
    }

    public void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState)
    {
        ValidateTextState(initialState);
        var native = new WindowsNativeV1.TextConfiguration
        {
            AbiVersion = WindowsNativeV1.AbiVersion,
            StructSize = checked((uint)sizeof(WindowsNativeV1.TextConfiguration)),
            InputType = (uint)configuration.inputType,
            InputAction = (uint)configuration.inputAction,
            Capitalization = (uint)configuration.textCapitalization,
            Flags = (configuration.readOnly ? 1u : 0u) |
                    (configuration.obscureText ? 2u : 0u) |
                    (configuration.autocorrect ? 4u : 0u) |
                    (configuration.enableSuggestions ? 8u : 0u),
        };
        var bytes = Encoding.UTF8.GetBytes(initialState.text);
        fixed (byte* data = bytes)
        {
            var state = CreateTextState(initialState, data, bytes.Length);
            var function = (delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.TextConfiguration*, WindowsNativeV1.TextState*, uint>)_native.SetTextClient;
            var status = function(_native.HostContext, &native, &state);
            if (status != 0) throw new InvalidOperationException($"Native text set-client failed: {status}.");
        }
    }

    public void UpdateState(DorotiTextEditingState state) =>
        InvokeTextState(state, (WindowsNativeV1.TextState* native) =>
        {
            var function = (delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.TextState*, uint>)_native.UpdateTextState;
            return function(_native.HostContext, native);
        }, "update-state");

    public void SetCaretRect(Rect logicalRect)
    {
        if (!logicalRect.IsFinite) throw new ArgumentOutOfRangeException(nameof(logicalRect));
        var function = (delegate* unmanaged[Cdecl]<nint, double, double, double, double, uint>)_native.SetCaretRect;
        var status = function(_native.HostContext, logicalRect.left, logicalRect.top,
            logicalRect.width, logicalRect.height);
        if (status != 0) throw new InvalidOperationException($"Native text caret request failed: {status}.");
    }

    public void ClearClient()
    {
        if (!_disposed && _nativeActive) Invoke(_native.ClearTextClient);
    }

    internal void MarkNativeStopped() => _nativeActive = false;

    public void UpdateSemantics(SemanticsUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);
        var nodes = update.nodes.Select(node => new
        {
            node.id, node.label, node.value, role = node.role.ToString(),
            actions = (long)node.actions, children = node.children,
            node.identifier, node.hint, node.tooltip, node.headingLevel, node.linkUrl,
            node.increasedValue, node.decreasedValue,
            validationResult = node.validationResult.ToString(),
            node.minValue, node.maxValue, node.scrollPosition, node.scrollExtentMin, node.scrollExtentMax,
            flags = node.flags is null ? null : new
            {
                enabled = node.flags.isEnabled.toBoolOrNull() ?? true,
                focused = node.flags.isFocused.toBoolOrNull() ?? false,
                focusable = node.flags.isFocused != Tristate.none,
                button = node.flags.isButton,
                textField = node.flags.isTextField,
                hidden = node.flags.isHidden,
                slider = node.flags.isSlider,
                readOnly = node.flags.isReadOnly,
                @checked = node.flags.isChecked.ToString(),
                selected = node.flags.isSelected.toBoolOrNull(),
                toggled = node.flags.isToggled.toBoolOrNull(),
                expanded = node.flags.isExpanded.toBoolOrNull(),
                required = node.flags.isRequired.toBoolOrNull(),
                mutuallyExclusive = node.flags.isInMutuallyExclusiveGroup,
                header = node.flags.isHeader,
                image = node.flags.isImage,
                liveRegion = node.flags.isLiveRegion,
                link = node.flags.isLink,
                obscured = node.flags.isObscured,
            },
            node.textSelectionBase, node.textSelectionExtent,
            rect = new[] { node.rect.left, node.rect.top, node.rect.right, node.rect.bottom },
        });
        InvokeUtf8(_native.UpdateSemantics,
            JsonSerializer.Serialize(new { generation = update.generation, nodes }),
            "semantics-update");
    }

    public void ClearSemantics()
    {
        if (!_disposed && _nativeActive) Invoke(_native.ClearSemantics);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _coordinator.Close();
        lock (_gate) _pendingFrame = null;
        lock (_gate)
        {
            _pendingInput.Clear();
            foreach (var request in _clipboardRequests.Values) request.TrySetCanceled();
            _clipboardRequests.Clear();
        }
        GC.KeepAlive(MetricsChanged);
        GC.KeepAlive(LifecycleChanged);
        GC.KeepAlive(CloseRequested);
        GC.KeepAlive(Closed);
        GC.KeepAlive(ConfigurationChanged);
        GC.KeepAlive(PointerData);
        GC.KeepAlive(KeyData);
        GC.KeepAlive(FocusData);
        GC.KeepAlive(EditingStateChanged);
        GC.KeepAlive(ActionPerformed);
        GC.KeepAlive(SemanticsAction);
        GC.KeepAlive(InputReceived);
        _coordinator.Dispose();
    }

    private void Invoke(nint callback)
    {
        var function = (delegate* unmanaged[Cdecl]<nint, uint>)callback;
        var status = function(_native.HostContext);
        if (status != 0) throw new InvalidOperationException($"Native host task request failed: {status}.");
    }

    private delegate uint TextStateCall(WindowsNativeV1.TextState* state);

    private static void InvokeTextState(DorotiTextEditingState state, TextStateCall call, string operation)
    {
        ValidateTextState(state);
        var bytes = Encoding.UTF8.GetBytes(state.text);
        fixed (byte* data = bytes)
        {
            var native = CreateTextState(state, data, bytes.Length);
            var status = call(&native);
            if (status != 0) throw new InvalidOperationException($"Native text {operation} failed: {status}.");
        }
    }

    private static WindowsNativeV1.TextState CreateTextState(
        DorotiTextEditingState state, byte* data, int byteLength) => new()
    {
        AbiVersion = WindowsNativeV1.AbiVersion,
        StructSize = checked((uint)sizeof(WindowsNativeV1.TextState)),
        Text = new WindowsNativeV1.Utf8
        {
            AbiVersion = WindowsNativeV1.AbiVersion,
            StructSize = checked((uint)sizeof(WindowsNativeV1.Utf8)),
            Data = (nint)data,
            ByteLength = checked((ulong)byteLength),
        },
        SelectionBase = state.selection.baseOffset,
        SelectionExtent = state.selection.extentOffset,
        ComposingBase = state.composingRange?.baseOffset ?? -1,
        ComposingExtent = state.composingRange?.extentOffset ?? -1,
    };

    private static void ValidateTextState(DorotiTextEditingState state)
    {
        ArgumentNullException.ThrowIfNull(state.text);
        // Flutter represents an attached controller with no current selection as (-1, -1).
        ValidateSelection(state.selection, state.text.Length, nameof(state.selection), allowAbsent: true);
        if (state.composingRange is not { } composing) return;
        ValidateSelection(composing, state.text.Length, nameof(state.composingRange), allowAbsent: false);
    }

    private void InvokeUtf8(nint callback, string text, string operation)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        fixed (byte* data = bytes)
        {
            var value = new WindowsNativeV1.Utf8
            {
                AbiVersion = WindowsNativeV1.AbiVersion,
                StructSize = checked((uint)sizeof(WindowsNativeV1.Utf8)),
                Data = (nint)data,
                ByteLength = checked((ulong)bytes.Length),
            };
            var function = (delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.Utf8, uint>)callback;
            var status = function(_native.HostContext, value);
            if (status != 0) throw new InvalidOperationException($"Native {operation} failed: {status}.");
        }
    }

    private static void ValidateSelection(
        DorotiTextSelection selection, int length, string name, bool allowAbsent)
    {
        if (allowAbsent && selection is { baseOffset: -1, extentOffset: -1 }) return;
        if (selection.baseOffset < 0 || selection.baseOffset > length ||
            selection.extentOffset < 0 || selection.extentOffset > length)
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }

    private void EnqueueInput(Action dispatch)
    {
        lock (_gate) _pendingInput.Enqueue(dispatch);
        RequestInvalidate();
    }

    private static IReadOnlyList<Locale> ResolveLocales()
    {
        var parts = CultureInfo.CurrentUICulture.Name.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return [new Locale(parts.FirstOrDefault() ?? "en", parts.Skip(1).FirstOrDefault())];
    }

    private TimeSpan MapTimestamp(long qpc)
    {
        lock (_gate)
        {
            if (_nativeClockOrigin < 0 || qpc == 0)
            {
                _nativeClockOrigin = qpc;
                _dorotiClockOrigin = DorotiFrameClock.Now;
                return _dorotiClockOrigin;
            }
            return _dorotiClockOrigin + Stopwatch.GetElapsedTime(_nativeClockOrigin, qpc);
        }
    }
}
