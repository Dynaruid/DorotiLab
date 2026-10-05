using Doroti.Skia.Rendering;
using Doroti.Ui;

namespace Doroti.Testing;

internal sealed class TestHost : IViewHostCapability, IFrameHostCapability, ILatestMetricsFrameHostCapability,
    IInputHostCapability, IViewFocusRequestCapability, IPlatformEnvironmentHostCapability,
    IPlatformMessageHostCapability, ITextInputHostCapability, ISkiaSceneRendererHost, IPlatformServicesHostCapability
{
    public TestHost(Size size, double dpr, HostOperatingSystem operatingSystem = HostOperatingSystem.windows, ulong viewId = 1)
    {
        ViewId = viewId;
        Metrics = new(size * dpr, dpr,
            ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
        Configuration = new([new Locale("en", "US")], Brightness.light, false, false, operatingSystem);
    }
    public ulong ViewId { get; }
    public ViewMetrics Metrics { get; private set; }
    public DorotiViewEpoch ViewEpoch => new(ViewId, Metrics.generation, Metrics.generation,
        Metrics.physicalSize.width / Metrics.devicePixelRatio, Metrics.physicalSize.height / Metrics.devicePixelRatio,
        (int)Metrics.physicalSize.width, (int)Metrics.physicalSize.height,
        Metrics.devicePixelRatio, Metrics.devicePixelRatio, 0);
    public DorotiResizeEpoch ResizeTarget => new(ViewEpoch.ResizeTargetGeneration,
        ViewEpoch.LogicalWidth, ViewEpoch.LogicalHeight, ViewEpoch.PhysicalWidth, ViewEpoch.PhysicalHeight,
        ViewEpoch.DevicePixelRatio, 0);
    public PlatformConfiguration Configuration { get; }
    public long SurfaceGeneration => Metrics.surfaceGeneration;
    public long InputSequence { get; private set; }
    public Action<TimeSpan>? PendingFrame { get; private set; }
    public bool HasTextClient { get; private set; }
    public DorotiTextEditingState? EditingState { get; private set; }
    public SemanticsUpdate? Semantics { get; private set; }
    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public event Action<PointerDataPacket>? PointerData;
    public event Action<KeyData>? KeyData;
    public event Action<RawFocusData>? FocusData;
    public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
    public event Action<int, SemanticsAction, object?>? SemanticsAction;
    public event Action<long, TimeSpan>? InputReceived;
    public event Action<DorotiTextEditingState>? EditingStateChanged;
    public event Action<DorotiTextInputAction>? ActionPerformed;
    public void Show() => LifecycleChanged?.Invoke(AppLifecycleState.resumed);
    public void Resize(Size size)
        => SetMetrics(size, Metrics.devicePixelRatio);
    public void SetMetrics(Size size, double dpr)
    {
        Metrics = Metrics with { physicalSize = size * dpr, devicePixelRatio = dpr,
            generation = Metrics.generation + 1, surfaceGeneration = Metrics.surfaceGeneration + 1 };
        MetricsChanged?.Invoke(Metrics);
    }
    public void Close() => CloseRequested?.Invoke();
    public void ScheduleFrame(Action<TimeSpan> callback) => PendingFrame ??= callback;
    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan> callback) => ScheduleFrame(callback);
    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan, DorotiViewEpoch> callback)
        => ScheduleFrame(time => callback(time, ViewEpoch));
    public void Frame(TimeSpan time) { var callback = PendingFrame; PendingFrame = null; callback?.Invoke(time); }
    public void Pointer(PointerData data)
    {
        InputReceived?.Invoke(++InputSequence, data.timeStamp);
        PointerData?.Invoke(new([data]));
    }
    public void Key(KeyData data) => KeyData?.Invoke(data);
    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction) => FocusData?.Invoke(new(ViewId, state == ViewFocusState.focused, TimeSpan.Zero));
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    public void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState) { HasTextClient = true; EditingState = initialState; }
    public void UpdateState(DorotiTextEditingState state) => EditingState = state;
    public void SetCaretRect(Rect rect) { }
    public void ClearClient() { HasTextClient = false; EditingState = null; }
    public void Edit(DorotiTextEditingState state)
    {
        if (!HasTextClient) throw new InvalidOperationException("Focus an editable widget before sending text.");
        EditingState = state;
        EditingStateChanged?.Invoke(state);
    }
    public Action<DorotiTextEditingState>? CaptureEditingCallback() => EditingStateChanged;
    public void TextAction(DorotiTextInputAction action) => ActionPerformed?.Invoke(action);
    public void UpdateSemantics(SemanticsUpdate update) => Semantics = update;
    public void ClearSemantics() => Semantics = null;
    public void PerformSemanticsAction(int id, SemanticsAction action, object? args) => SemanticsAction?.Invoke(id, action, args);
    public void RequestInvalidate() { }
    private string? _clipboard;
    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(_clipboard);
    public ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default) { _clipboard = text; return ValueTask.CompletedTask; }
    public void SetCursor(DorotiMouseCursorKind cursor) { }
    public void Dispose() { PendingFrame = null; ClearClient(); ClearSemantics(); Closed?.Invoke(); }
}
