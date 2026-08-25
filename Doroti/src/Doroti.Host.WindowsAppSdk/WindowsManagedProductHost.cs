using System.Globalization;
using System.Diagnostics;
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
    ISkiaSceneRendererHost
{
    private readonly object _gate = new();
    private readonly WindowsNativeV1.Host _native;
    private readonly WindowsManagedResizeCoordinator _coordinator = new(TimeSpan.FromMilliseconds(100));
    private readonly HashSet<long> _resizeTerminalGenerations = [];
    private readonly Dictionary<ulong, TaskCompletionSource<string?>> _clipboardRequests = [];
    private readonly Queue<Action> _pendingInput = [];
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
            native.HostContext == 0 || native.TopLevelHwnd == 0 || native.ChildHwnd == 0 || native.TaskHwnd == 0 ||
            native.RequestFrame == 0 || native.RequestResize == 0 || native.RequestClose == 0 || native.RequestShow == 0 ||
            native.SetCursor == 0 || native.SetClipboard == 0 || native.RequestClipboard == 0)
            throw new InvalidDataException("The native product host table is invalid.");
        _native = native;
        Metrics = new(new Size(logicalWidth, logicalHeight), 1, ViewPadding.zero,
            ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
        Configuration = new(ResolveLocales(), Brightness.light, false, false, HostOperatingSystem.windows);
    }

    internal nint ChildHwnd => _native.ChildHwnd;
    internal nint TopLevelHwnd => _native.TopLevelHwnd;
    internal WindowsResizeCoordinatorSnapshot ResizeSnapshot => _coordinator.Snapshot();
    internal bool IsLatestResizeGeneration(ulong generation) =>
        generation <= long.MaxValue && _coordinator.IsLatest((long)generation);
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

    internal void BeginFrame(in WindowsNativeV1.FrameRequest request)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (request.AbiVersion != WindowsNativeV1.AbiVersion ||
            request.StructSize < sizeof(WindowsNativeV1.FrameRequest) || request.ViewId != 1 ||
            !_coordinator.ValidateExact(checked((long)request.Generation),
                checked((int)request.WidthPx), checked((int)request.HeightPx)))
            throw new InvalidDataException("Native frame request failed exact admission.");
        Action<TimeSpan, DorotiViewEpoch>? callback;
        Action[] input;
        lock (_gate)
        {
            input = [.. _pendingInput];
            _pendingInput.Clear();
            callback = _pendingFrame;
            _pendingFrame = null;
        }
        foreach (var dispatch in input) dispatch();
        callback?.Invoke(DorotiFrameClock.Now, ViewEpoch);
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
        var key = new KeyData(1, timestamp, (KeyEventType)value.Type,
            value.Physical, value.Logical, false,
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

    internal void CompleteClipboard(ulong requestId, string text)
    {
        TaskCompletionSource<string?>? completion;
        lock (_gate) _clipboardRequests.Remove(requestId, out completion);
        completion?.TrySetResult(text);
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

    internal void MarkNativeStopped() => _nativeActive = false;

    public void UpdateSemantics(SemanticsUpdate update) => _ = update;
    public void ClearSemantics() { }

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
