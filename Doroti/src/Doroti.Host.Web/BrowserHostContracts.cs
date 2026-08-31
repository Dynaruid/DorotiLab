using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.Web;

public sealed record BrowserGpuIdentity(
    string Api,
    string Vendor,
    string Renderer,
    bool Hardware,
    bool SoftwareFallbackUsed,
    long ContextGeneration = 0);

public sealed record BrowserHostSnapshot(
    string CanvasId,
    double LogicalWidth,
    double LogicalHeight,
    double DevicePixelRatio,
    bool Visible,
    bool Focused,
    string LanguageTag,
    string Brightness,
    string OperatingSystem,
    long Generation,
    long SurfaceGeneration,
    long InputSequence,
    BrowserGpuIdentity Gpu,
    DorotiResizeEpoch ResizeEpoch);

public sealed record BrowserJavaScriptPluginDescriptor(
    string Id,
    string Channel,
    string AbiVersion,
    string ModuleUrl,
    string ExportName);

/// <summary>Browser-only JavaScript boundary. DOM and WebGL types never leave this project.</summary>
[SupportedOSPlatform("browser")]
internal static partial class BrowserInterop
{
    private const string Module = "doroti.web";

    [System.Runtime.InteropServices.JavaScript.JSImport("createHost", Module)]
    internal static partial string CreateHost(
        int hostId,
        string canvasId,
        double logicalWidth,
        double logicalHeight);

    [JSImport("initializeManagedCallbacks", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    internal static partial Task<string> InitializeManagedCallbacksAsync();

    [JSImport("getRendererIdentity", Module)]
    internal static partial string GetRendererIdentity();

    [System.Runtime.InteropServices.JavaScript.JSImport("showHost", Module)]
    internal static partial string ShowHost(int hostId);

    [System.Runtime.InteropServices.JavaScript.JSImport("resizeHost", Module)]
    internal static partial string ResizeHost(int hostId, double logicalWidth, double logicalHeight);

    [System.Runtime.InteropServices.JavaScript.JSImport("requestFrame", Module)]
    internal static partial void RequestFrame(int hostId, int callbackId);

    [JSImport("recordManagedRaster", Module)]
    internal static partial void RecordManagedRaster(
        int hostId, string phase, int surfaceWidth, int surfaceHeight, double durationMicroseconds);

    [JSImport("requestPresent", Module)]
    internal static partial void RequestPresent(
        string canvasId, [JSMarshalAs<JSType.Number>] long generation,
        double logicalWidth, double logicalHeight,
        int physicalWidth, int physicalHeight, double devicePixelRatio,
        [JSMarshalAs<JSType.Number>] long timestampMicroseconds);

    [JSImport("captureResizeTrace", Module)]
    internal static partial string CaptureResizeTrace(int hostId);

    [System.Runtime.InteropServices.JavaScript.JSImport("closeHost", Module)]
    internal static partial void CloseHost(int hostId);

    [System.Runtime.InteropServices.JavaScript.JSImport("resolveResourceUrl", Module)]
    internal static partial string ResolveResourceUrl(string relativeUrl);

    [JSImport("setCursor", Module)]
    internal static partial void SetCursor(int hostId, string cursor);

    [JSImport("requestFocus", Module)]
    internal static partial string RequestFocus(int hostId, bool focused);

    [JSImport("setTextInputState", Module)]
    internal static partial void SetTextInputState(
        int hostId, string text, int selectionBase, int selectionExtent,
        string inputMode, string enterKeyHint, bool readOnly, bool obscureText,
        string autocapitalize, bool autocorrect, int inputAction, bool multiline, bool attach);

    [JSImport("setCaretRect", Module)]
    internal static partial void SetCaretRect(int hostId, double left, double top, double width, double height);

    [JSImport("clearTextInput", Module)]
    internal static partial void ClearTextInput(int hostId);

    [JSImport("readClipboardText", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    internal static partial Task<string> ReadClipboardTextAsync();

    [JSImport("writeClipboardText", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    internal static partial Task<string> WriteClipboardTextAsync(string text);

    [JSImport("updateSemantics", Module)]
    internal static partial void UpdateSemantics(int hostId, string json);

    [JSImport("setApplicationTitle", Module)]
    internal static partial void SetApplicationTitle(int hostId, string title);

    [System.Runtime.InteropServices.JavaScript.JSImport("invokePlugin", Module)]
    [return: System.Runtime.InteropServices.JavaScript.JSMarshalAs<System.Runtime.InteropServices.JavaScript.JSType.Promise<System.Runtime.InteropServices.JavaScript.JSType.String>>]
    internal static partial Task<string> InvokePluginAsync(
        string moduleUrl,
        string exportName,
        string channel,
        string codec,
        string payloadBase64);

    [System.Runtime.InteropServices.JavaScript.JSExport]
    internal static void DispatchAnimationFrame(int hostId, int callbackId, double timestampMilliseconds) =>
        BrowserHostAdapter.DispatchAnimationFrame(hostId, callbackId, timestampMilliseconds);

    [System.Runtime.InteropServices.JavaScript.JSExport]
    internal static void DispatchSnapshot(int hostId, string json) =>
        BrowserHostAdapter.DispatchSnapshot(hostId, json);

    [JSExport]
    internal static void DispatchResizeEpoch(
        int hostId,
        [JSMarshalAs<JSType.Number>] long hostGeneration,
        [JSMarshalAs<JSType.Number>] long generation,
        double logicalWidth,
        double logicalHeight,
        int physicalWidth,
        int physicalHeight,
        double devicePixelRatio,
        [JSMarshalAs<JSType.Number>] long timestampMicroseconds) =>
        BrowserHostAdapter.DispatchResizeEpoch(
            hostId, hostGeneration,
            new DorotiResizeEpoch(
                generation, logicalWidth, logicalHeight, physicalWidth, physicalHeight,
                devicePixelRatio, timestampMicroseconds));

    [JSExport]
    internal static void DispatchPointerBatch(
        int hostId, int phase, int kind, int pointerId, int buttons, int modifiers,
        [JSMarshalAs<JSType.Number>] long inputSequence,
        [JSMarshalAs<JSType.Array<JSType.Number>>] double[] samples) =>
        BrowserHostAdapter.DispatchPointerBatch(hostId, phase, kind, pointerId, buttons, modifiers, inputSequence, samples);

    [JSExport]
    internal static void DispatchWheel(
        int hostId, double x, double y, double deltaX, double deltaY, double timestampMilliseconds, int kind,
        [JSMarshalAs<JSType.Number>] long inputSequence) =>
        BrowserHostAdapter.DispatchWheel(hostId, x, y, deltaX, deltaY, timestampMilliseconds, kind, inputSequence);

    [JSExport]
    internal static void DispatchKey(
        int hostId, bool down, bool repeat, bool synthesized, string code, string key, double timestampMilliseconds,
        [JSMarshalAs<JSType.Number>] long inputSequence) =>
        BrowserHostAdapter.DispatchKey(hostId, down, repeat, synthesized, code, key, timestampMilliseconds, inputSequence);

    [JSExport]
    internal static void DispatchFocus(int hostId, bool focused, double timestampMilliseconds,
        [JSMarshalAs<JSType.Number>] long inputSequence) =>
        BrowserHostAdapter.DispatchFocus(hostId, focused, timestampMilliseconds, inputSequence);

    [JSExport]
    internal static void DispatchTextEditing(
        int hostId, string text, int selectionBase, int selectionExtent, int composingBase, int composingExtent,
        [JSMarshalAs<JSType.Number>] long inputSequence) =>
        BrowserHostAdapter.DispatchTextEditing(hostId, text, selectionBase, selectionExtent, composingBase, composingExtent, inputSequence);

    [JSExport]
    internal static void DispatchTextAction(int hostId, int action,
        [JSMarshalAs<JSType.Number>] long inputSequence) =>
        BrowserHostAdapter.DispatchTextAction(hostId, action, inputSequence);

    [JSExport]
    internal static void DispatchTextConnectionClosed(int hostId,
        [JSMarshalAs<JSType.Number>] long inputSequence) =>
        BrowserHostAdapter.DispatchTextConnectionClosed(hostId, inputSequence);

    [JSExport]
    internal static void DispatchSemanticsAction(
        int hostId,
        [JSMarshalAs<JSType.Number>] long nodeId,
        [JSMarshalAs<JSType.Number>] long action,
        [JSMarshalAs<JSType.Number>] long inputSequence,
        string argumentsJson) =>
        BrowserHostAdapter.DispatchSemanticsAction(hostId, nodeId, action, inputSequence, argumentsJson);

    internal static BrowserHostSnapshot ParseSnapshot(string json) =>
        JsonSerializer.Deserialize<BrowserHostSnapshot>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException("The browser host returned an empty snapshot.");

    internal static IReadOnlyList<DorotiResizeTraceEntry> ParseResizeTrace(string json) =>
        JsonSerializer.Deserialize<DorotiResizeTraceEntry[]>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException("The browser host returned an empty resize trace.");
}

[SupportedOSPlatform("browser")]
public static class BrowserHostRuntime
{
    private static readonly SemaphoreSlim InitializationGate = new(1, 1);
    private static bool _initialized;

    public static async ValueTask EnsureInitializedAsync()
    {
        if (_initialized) return;
        await InitializationGate.WaitAsync();
        try
        {
            if (_initialized) return;
            await JSHost.ImportAsync("doroti.web", "../_content/Doroti.Host.Web/doroti.web.js");
            await BrowserInterop.InitializeManagedCallbacksAsync();
            _initialized = true;
        }
        finally
        {
            InitializationGate.Release();
        }
    }

    public static string ResolveResourceUrl(string relativeUrl) =>
        BrowserInterop.ResolveResourceUrl(relativeUrl);

    public static void SetApplicationTitle(int hostId, string title) =>
        BrowserInterop.SetApplicationTitle(hostId, title);

    public static string RendererIdentity => BrowserInterop.GetRendererIdentity();
}

[SupportedOSPlatform("browser")]
public sealed class BrowserHostAdapter :
    IViewHostCapability,
    IFrameHostCapability,
    ILatestMetricsFrameHostCapability,
    IPlatformEnvironmentHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    IPlatformServicesHostCapability,
    ITextInputHostCapability
{
    private static readonly object RegistryGate = new();
    private static readonly Dictionary<int, WeakReference<BrowserHostAdapter>> Registry = [];
    private static int _nextHostId;

    private readonly object _gate = new();
    private Action<TimeSpan, DorotiViewEpoch>? _pendingFrame;
    private int _pendingFrameId;
    private readonly Dictionary<ulong, (double X, double Y)> _pointerPositions = [];
    private readonly ulong _viewId;
    private int _nextCallbackId;
    private BrowserHostSnapshot _snapshot;
    private PlatformConfiguration _configuration;
    private long _inputSequence;
    private DorotiTextInputConfiguration _textInputConfiguration;
    private bool _disposed;

    public BrowserHostAdapter(ulong viewId, string canvasId, Size logicalSize)
    {
        if (!OperatingSystem.IsBrowser())
            throw new PlatformNotSupportedException("Doroti.Host.Web requires a browser-wasm process.");
        ArgumentException.ThrowIfNullOrWhiteSpace(canvasId);
        ArgumentNullException.ThrowIfNull(logicalSize);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
            throw new ArgumentOutOfRangeException(nameof(logicalSize));

        _viewId = viewId;
        HostId = Interlocked.Increment(ref _nextHostId);
        lock (RegistryGate) Registry.Add(HostId, new(this));
        try
        {
            _snapshot = Validate(BrowserInterop.ParseSnapshot(
                BrowserInterop.CreateHost(HostId, canvasId, logicalSize.width, logicalSize.height)));
            _configuration = ToConfiguration(_snapshot);
            _inputSequence = _snapshot.InputSequence;
        }
        catch
        {
            lock (RegistryGate) Registry.Remove(HostId);
            throw;
        }
    }

    public int HostId { get; }
    public BrowserGpuIdentity Gpu => _snapshot.Gpu;
    public BrowserHostSnapshot Snapshot => _snapshot;
    public IReadOnlyList<DorotiResizeTraceEntry> CaptureResizeTrace()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return BrowserInterop.ParseResizeTrace(BrowserInterop.CaptureResizeTrace(HostId));
    }
    public ViewMetrics Metrics => ToMetrics(_snapshot);
    public DorotiViewEpoch ViewEpoch
    {
        get
        {
            var snapshot = _snapshot;
            var epoch = snapshot.ResizeEpoch;
            return new(
                _viewId,
                epoch.Generation,
                snapshot.Generation,
                epoch.LogicalWidth,
                epoch.LogicalHeight,
                epoch.PhysicalWidth,
                epoch.PhysicalHeight,
                epoch.DeviceScaleX,
                epoch.DeviceScaleY,
                epoch.TimestampMicroseconds);
        }
    }
    public PlatformConfiguration Configuration => _configuration;
    internal long InputSequence => Interlocked.Read(ref _inputSequence);
    internal event Action<long, TimeSpan>? InputReceived;

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
    public event Action? ConnectionClosed;

    public string ResolveResourceUrl(string relativeUrl)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeUrl);
        return BrowserInterop.ResolveResourceUrl(relativeUrl);
    }

    internal void UpdateSemantics(string json)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        BrowserInterop.UpdateSemantics(HostId, json);
    }

    internal void RecordRaster(string phase, int width, int height, TimeSpan? duration = null) =>
        BrowserInterop.RecordManagedRaster(
            HostId, phase, width, height, duration?.Ticks / 10.0 ?? 0);

    internal event Action<long, long, string>? SemanticsAction;

    public async ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var value = await BrowserInterop.ReadClipboardTextAsync();
        cancellationToken.ThrowIfCancellationRequested();
        return value;
    }

    public async ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await BrowserInterop.WriteClipboardTextAsync(text ?? string.Empty);
        cancellationToken.ThrowIfCancellationRequested();
    }

    public void SetCursor(DorotiMouseCursorKind cursor) => BrowserInterop.SetCursor(HostId, CursorName(cursor));

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        _ = direction;
        ObjectDisposedException.ThrowIf(_disposed, this);
        ApplySnapshot(BrowserInterop.ParseSnapshot(
            BrowserInterop.RequestFocus(HostId, state == ViewFocusState.focused)));
    }

    public void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState)
    {
        _textInputConfiguration = configuration;
        SetTextInputState(initialState, attach: true);
    }

    public void UpdateState(DorotiTextEditingState state) => SetTextInputState(state, attach: false);

    private void SetTextInputState(DorotiTextEditingState state, bool attach) =>
        BrowserInterop.SetTextInputState(
            HostId, state.text, state.selection.baseOffset, state.selection.extentOffset,
            InputMode(_textInputConfiguration.inputType), EnterKeyHint(_textInputConfiguration.inputAction),
            _textInputConfiguration.readOnly, _textInputConfiguration.obscureText,
            AutoCapitalize(_textInputConfiguration.textCapitalization),
            _textInputConfiguration.autocorrect && _textInputConfiguration.enableSuggestions,
            (int)_textInputConfiguration.inputAction,
            _textInputConfiguration.inputType == DorotiTextInputType.multiline,
            attach);

    public void SetCaretRect(Rect logicalRect) => BrowserInterop.SetCaretRect(
        HostId, logicalRect.left, logicalRect.top, logicalRect.width, logicalRect.height);

    public void ClearClient() => BrowserInterop.ClearTextInput(HostId);

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ApplySnapshot(BrowserInterop.ParseSnapshot(BrowserInterop.ShowHost(HostId)));
    }

    public void Resize(Size logicalSize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(logicalSize);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
            throw new ArgumentOutOfRangeException(nameof(logicalSize));
        ApplySnapshot(BrowserInterop.ParseSnapshot(
            BrowserInterop.ResizeHost(HostId, logicalSize.width, logicalSize.height)));
    }

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ScheduleFrame(ViewEpoch, (timestamp, _) => callback(timestamp));
    }

    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ScheduleFrame(expectedEpoch, (timestamp, admittedEpoch) =>
        {
            if (admittedEpoch == expectedEpoch) callback(timestamp);
        });
    }

    public void ScheduleFrame(
        DorotiViewEpoch expectedEpoch,
        Action<TimeSpan, DorotiViewEpoch> callback)
    {
        ArgumentNullException.ThrowIfNull(expectedEpoch);
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (expectedEpoch.ViewId != _viewId)
            throw new InvalidOperationException(
                $"View epoch {expectedEpoch.ViewId} cannot schedule a frame for view {_viewId}.");

        int callbackId;
        lock (_gate)
        {
            callbackId = checked(++_nextCallbackId);
            _pendingFrame = callback;
            _pendingFrameId = callbackId;
        }
        BrowserInterop.RequestFrame(HostId, callbackId);
    }

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
        lock (RegistryGate) Registry.Remove(HostId);
        lock (_gate)
        {
            _pendingFrame = null;
            _pendingFrameId = 0;
        }
        BrowserInterop.CloseHost(HostId);
        Closed?.Invoke();
    }

    internal static void DispatchAnimationFrame(int hostId, int callbackId, double timestampMilliseconds)
    {
        if (!TryGet(hostId, out var host)) return;
        Action<TimeSpan, DorotiViewEpoch>? callback;
        lock (host._gate)
        {
            if (host._pendingFrameId != callbackId) return;
            callback = host._pendingFrame;
            host._pendingFrame = null;
            host._pendingFrameId = 0;
        }
        // Browser resize signals can advance several times while one rAF is
        // pending. Admit the newest immutable viewport epoch immediately before
        // framework work starts, matching Flutter Web's single-rAF/latest-metrics
        // scheduling instead of building the epoch captured by the first signal.
        callback?.Invoke(TimeSpan.FromMilliseconds(timestampMilliseconds), host.ViewEpoch);
    }

    internal static void DispatchSnapshot(int hostId, string json)
    {
        if (TryGet(hostId, out var host)) host.ApplySnapshot(BrowserInterop.ParseSnapshot(json));
    }

    internal static void DispatchResizeEpoch(
        int hostId,
        long hostGeneration,
        DorotiResizeEpoch resizeEpoch)
    {
        if (TryGet(hostId, out var host)) host.ApplyResizeEpoch(hostGeneration, resizeEpoch);
    }

    internal static void DispatchPointerBatch(
        int hostId, int phase, int kind, int pointerId, int buttons, int modifiers,
        long inputSequence, double[] samples)
    {
        if (!TryGet(hostId, out var host) || samples.Length == 0 || samples.Length % 7 != 0) return;
        host.AcceptInputSequence(inputSequence, TimeSpan.FromMilliseconds(samples[^1]));
        var ratio = host._snapshot.DevicePixelRatio;
        var pointer = checked((ulong)Math.Max(0, pointerId));
        var data = new List<PointerData>(samples.Length / 7);
        for (var index = 0; index < samples.Length; index += 7)
        {
            var x = samples[index] * ratio;
            var y = samples[index + 1] * ratio;
            var hasPrevious = host._pointerPositions.TryGetValue(pointer, out var previous);
            var change = phase switch
            {
                1 => PointerChange.down,
                2 => PointerChange.up,
                3 => PointerChange.cancel,
                4 => PointerChange.hover,
                5 => PointerChange.add,
                6 => PointerChange.remove,
                _ => PointerChange.move,
            };
            data.Add(new(
                host._viewId,
                TimeSpan.FromMilliseconds(samples[index + 6]),
                change,
                kind switch { 1 => PointerDeviceKind.touch, 2 => PointerDeviceKind.stylus, _ => PointerDeviceKind.mouse },
                pointer,
                x,
                y,
                !hasPrevious || change is PointerChange.add or PointerChange.remove or PointerChange.cancel ? 0 : x - previous.X,
                !hasPrevious || change is PointerChange.add or PointerChange.remove or PointerChange.cancel ? 0 : y - previous.Y,
                buttons,
                pointerIdentifier: pointer,
                pressure: samples[index + 2],
                pressureMin: 0,
                pressureMax: 1,
                orientation: samples[index + 5],
                tilt: Math.Sqrt((samples[index + 3] * samples[index + 3]) + (samples[index + 4] * samples[index + 4]))));
            if (change is PointerChange.remove or PointerChange.cancel) host._pointerPositions.Remove(pointer);
            else host._pointerPositions[pointer] = (x, y);
        }
        host.PointerData?.Invoke(new(data));
    }

    internal static void DispatchWheel(
        int hostId, double x, double y, double deltaX, double deltaY, double timestampMilliseconds, int kind,
        long inputSequence)
    {
        if (!TryGet(hostId, out var host)) return;
        host.AcceptInputSequence(inputSequence, TimeSpan.FromMilliseconds(timestampMilliseconds));
        var ratio = host._snapshot.DevicePixelRatio;
        host.PointerData?.Invoke(new([
            new(host._viewId, TimeSpan.FromMilliseconds(timestampMilliseconds), PointerChange.hover,
                kind == 3 ? PointerDeviceKind.trackpad : PointerDeviceKind.mouse,
                0, x * ratio, y * ratio, 0, 0, 0,
                deltaX * ratio, deltaY * ratio, PointerSignalKind.scroll),
        ]));
    }

    internal static void DispatchKey(
        int hostId, bool down, bool repeat, bool synthesized, string code, string key, double timestampMilliseconds,
        long inputSequence)
    {
        if (!TryGet(hostId, out var host)) return;
        host.AcceptInputSequence(inputSequence, TimeSpan.FromMilliseconds(timestampMilliseconds));
        host.KeyData?.Invoke(new(
            host._viewId,
            TimeSpan.FromMilliseconds(timestampMilliseconds),
            down ? (repeat ? KeyEventType.repeat : KeyEventType.down) : KeyEventType.up,
            BrowserKeyMap.Physical(code),
            BrowserKeyMap.Logical(code, key),
            synthesized,
            BrowserKeyMap.Character(key)));
    }

    internal static void DispatchSemanticsAction(
        int hostId, long nodeId, long action, long inputSequence, string argumentsJson)
    {
        if (!TryGet(hostId, out var host)) return;
        host.AcceptInputSequence(inputSequence, DorotiFrameClock.Now);
        host.SemanticsAction?.Invoke(nodeId, action, argumentsJson);
    }

    internal static void DispatchFocus(int hostId, bool focused, double timestampMilliseconds, long inputSequence)
    {
        if (TryGet(hostId, out var host))
        {
            host.AcceptInputSequence(inputSequence, TimeSpan.FromMilliseconds(timestampMilliseconds));
            host.FocusData?.Invoke(new(host._viewId, focused, TimeSpan.FromMilliseconds(timestampMilliseconds)));
        }
    }

    internal static void DispatchTextEditing(
        int hostId, string text, int selectionBase, int selectionExtent, int composingBase, int composingExtent,
        long inputSequence)
    {
        if (!TryGet(hostId, out var host)) return;
        host.AcceptInputSequence(inputSequence, DorotiFrameClock.Now);
        DorotiTextSelection? composing = composingBase >= 0 && composingExtent >= composingBase
            ? new DorotiTextSelection(composingBase, composingExtent)
            : null;
        host.EditingStateChanged?.Invoke(new(text, new(selectionBase, selectionExtent), composing));
    }

    internal static void DispatchTextAction(int hostId, int action, long inputSequence)
    {
        if (TryGet(hostId, out var host) && Enum.IsDefined(typeof(DorotiTextInputAction), action))
        {
            host.AcceptInputSequence(inputSequence, DorotiFrameClock.Now);
            host.ActionPerformed?.Invoke((DorotiTextInputAction)action);
        }
    }

    internal static void DispatchTextConnectionClosed(int hostId, long inputSequence)
    {
        if (!TryGet(hostId, out var host)) return;
        host.AcceptInputSequence(inputSequence, DorotiFrameClock.Now);
        host.ConnectionClosed?.Invoke();
    }

    private void AcceptInputSequence(long sequence, TimeSpan timestamp)
    {
        if (sequence <= 0) throw new InvalidDataException("Browser input sequence must be positive.");
        var previous = Interlocked.Read(ref _inputSequence);
        if (sequence != previous + 1)
            throw new InvalidDataException(
                $"Browser input sequence is not contiguous: previous={previous}, received={sequence}.");
        Interlocked.Exchange(ref _inputSequence, sequence);
        InputReceived?.Invoke(sequence, timestamp);
    }

    private static bool TryGet(int hostId, out BrowserHostAdapter host)
    {
        lock (RegistryGate)
        {
            if (Registry.TryGetValue(hostId, out var reference) && reference.TryGetTarget(out host!)) return true;
            Registry.Remove(hostId);
            host = null!;
            return false;
        }
    }

    private void ApplySnapshot(BrowserHostSnapshot next)
    {
        next = Validate(next);
        var previous = _snapshot;
        _snapshot = next;
        if (ToMetrics(previous) != ToMetrics(next)) MetricsChanged?.Invoke(ToMetrics(next));
        var previousState = Lifecycle(previous);
        var nextState = Lifecycle(next);
        if (previousState != nextState) LifecycleChanged?.Invoke(nextState);
        var nextConfiguration = ToConfiguration(next);
        if (previous.LanguageTag != next.LanguageTag || previous.Brightness != next.Brightness)
        {
            _configuration = nextConfiguration;
            ConfigurationChanged?.Invoke(nextConfiguration);
        }
    }

    private void ApplyResizeEpoch(long hostGeneration, DorotiResizeEpoch next)
    {
        if (next.Generation <= _snapshot.ResizeEpoch.Generation) return;
        if (next.LogicalWidth <= 0 || next.LogicalHeight <= 0 ||
            !double.IsFinite(next.LogicalWidth) || !double.IsFinite(next.LogicalHeight) ||
            next.PhysicalWidth <= 0 || next.PhysicalHeight <= 0 ||
            next.DevicePixelRatio <= 0 || !double.IsFinite(next.DevicePixelRatio))
            throw new InvalidDataException("The browser returned an invalid resize epoch.");
        ApplySnapshot(_snapshot with
        {
            LogicalWidth = next.LogicalWidth,
            LogicalHeight = next.LogicalHeight,
            DevicePixelRatio = next.DevicePixelRatio,
            Generation = Math.Max(_snapshot.Generation, hostGeneration),
            ResizeEpoch = next,
        });
    }

    private static BrowserHostSnapshot Validate(BrowserHostSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if (snapshot.LogicalWidth <= 0 || snapshot.LogicalHeight <= 0 ||
            !double.IsFinite(snapshot.LogicalWidth) || !double.IsFinite(snapshot.LogicalHeight) ||
            snapshot.DevicePixelRatio <= 0 || !double.IsFinite(snapshot.DevicePixelRatio))
            throw new InvalidDataException("The browser returned invalid canvas metrics.");
        if (!snapshot.Gpu.Hardware || snapshot.Gpu.SoftwareFallbackUsed || snapshot.Gpu.Api != "webgl2")
            throw new PlatformNotSupportedException(
                $"A hardware WebGL2 canvas is required; browser reported '{snapshot.Gpu.Api}/{snapshot.Gpu.Renderer}'.");
        return snapshot;
    }

    private static ViewMetrics ToMetrics(BrowserHostSnapshot snapshot) => new(
        new Size(snapshot.ResizeEpoch.PhysicalWidth, snapshot.ResizeEpoch.PhysicalHeight),
        snapshot.DevicePixelRatio, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero,
        Lifecycle(snapshot), snapshot.Generation, snapshot.SurfaceGeneration);

    private static AppLifecycleState Lifecycle(BrowserHostSnapshot snapshot) =>
        snapshot.Visible ? (snapshot.Focused ? AppLifecycleState.resumed : AppLifecycleState.inactive) : AppLifecycleState.hidden;

    private static PlatformConfiguration ToConfiguration(BrowserHostSnapshot snapshot)
    {
        var pieces = snapshot.LanguageTag.Split('-', StringSplitOptions.RemoveEmptyEntries);
        var locale = pieces.Length switch
        {
            0 => new Locale("en", "US"),
            1 => new Locale(pieces[0]),
            2 => new Locale(pieces[0], pieces[1]),
            _ => new Locale(pieces[0], pieces[^1], pieces[1]),
        };
        var operatingSystem = snapshot.OperatingSystem switch
        {
            "android" => HostOperatingSystem.android,
            "iOS" => HostOperatingSystem.iOS,
            "linux" => HostOperatingSystem.linux,
            "macOS" => HostOperatingSystem.macOS,
            "windows" => HostOperatingSystem.windows,
            _ => HostOperatingSystem.web,
        };
        return new([locale], snapshot.Brightness == "dark" ? Brightness.dark : Brightness.light,
            false, false, operatingSystem);
    }

    private static string InputMode(DorotiTextInputType type) => type switch
    {
        DorotiTextInputType.number => "decimal",
        DorotiTextInputType.phone => "tel",
        DorotiTextInputType.emailAddress => "email",
        DorotiTextInputType.url => "url",
        DorotiTextInputType.none => "none",
        DorotiTextInputType.webSearch => "search",
        _ => "text",
    };

    private static string EnterKeyHint(DorotiTextInputAction action) => action switch
    {
        DorotiTextInputAction.done => "done",
        DorotiTextInputAction.go => "go",
        DorotiTextInputAction.search => "search",
        DorotiTextInputAction.send => "send",
        DorotiTextInputAction.next => "next",
        DorotiTextInputAction.previous => "previous",
        DorotiTextInputAction.newline => "enter",
        _ => string.Empty,
    };

    private static string AutoCapitalize(DorotiTextCapitalization capitalization) => capitalization switch
    {
        DorotiTextCapitalization.words => "words",
        DorotiTextCapitalization.sentences => "sentences",
        DorotiTextCapitalization.characters => "characters",
        _ => "none",
    };

    private static string CursorName(DorotiMouseCursorKind cursor) => cursor switch
    {
        DorotiMouseCursorKind.click => "pointer",
        DorotiMouseCursorKind.forbidden or DorotiMouseCursorKind.noDrop => "not-allowed",
        DorotiMouseCursorKind.wait => "wait",
        DorotiMouseCursorKind.progress => "progress",
        DorotiMouseCursorKind.text => "text",
        DorotiMouseCursorKind.verticalText => "vertical-text",
        DorotiMouseCursorKind.precise => "crosshair",
        DorotiMouseCursorKind.move or DorotiMouseCursorKind.allScroll => "move",
        DorotiMouseCursorKind.grab => "grab",
        DorotiMouseCursorKind.grabbing => "grabbing",
        DorotiMouseCursorKind.resizeLeftRight => "ew-resize",
        DorotiMouseCursorKind.resizeUpDown => "ns-resize",
        DorotiMouseCursorKind.resizeUpLeftDownRight => "nwse-resize",
        DorotiMouseCursorKind.resizeUpRightDownLeft => "nesw-resize",
        DorotiMouseCursorKind.none => "none",
        _ => "default",
    };
}

[SupportedOSPlatform("browser")]
public sealed class BrowserJavaScriptPluginHandler : IDorotiNativePluginHandler
{
    private sealed record PluginResponse(bool HasValue, string Base64);

    private readonly BrowserJavaScriptPluginDescriptor _descriptor;

    public BrowserJavaScriptPluginHandler(BrowserJavaScriptPluginDescriptor descriptor) =>
        _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));

    public string PluginId => _descriptor.Id;
    public string AbiVersion => _descriptor.AbiVersion;

    public async ValueTask<ReadOnlyMemory<byte>?> HandleAsync(
        string channel,
        string codec,
        ReadOnlyMemory<byte>? message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel != _descriptor.Channel)
            throw new InvalidOperationException($"Plugin '{PluginId}' does not own channel '{channel}'.");
        var json = await BrowserInterop.InvokePluginAsync(
            _descriptor.ModuleUrl,
            _descriptor.ExportName,
            channel,
            codec,
            message is null ? string.Empty : Convert.ToBase64String(message.Value.Span));
        cancellationToken.ThrowIfCancellationRequested();
        var response = JsonSerializer.Deserialize<PluginResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException($"Plugin '{PluginId}' returned an invalid response envelope.");
        return response.HasValue ? Convert.FromBase64String(response.Base64) : null;
    }
}
