using Doroti.Framework.Scheduler;
using Doroti.Runtime;
using Doroti.Ui;

VerifyFlutterSchedulerOrdering();
VerifyMonotonicTimestampFence();
VerifyBoundedTraceCausality();
VerifyMetricsActivityUsesArrivalClock();
VerifyRecordingClockIsIndependentOfHostTimestamp();
VerifyPlatformEventDrainsMicrotasks();
VerifyLatestMetricsFrameAdmission();

Console.WriteLine($"FCR-3 scheduler runtime contract: PASS (configuration={ConfigurationName()})");

static void VerifyRecordingClockIsIndependentOfHostTimestamp()
{
    var trace = new DorotiFrameTrace { MeasureRecordingTime = true };
    var before = DorotiFrameClock.Now.Ticks / 10;
    trace.Record(DorotiFramePhase.beginFrame, 1, TimeSpan.FromDays(1));
    trace.Record(DorotiFramePhase.layout, 1, DorotiFrameClock.Now);
    var after = DorotiFrameClock.Now.Ticks / 10;
    var entries = trace.Snapshot();
    Require(entries[0].TimestampMicroseconds == entries[1].TimestampMicroseconds,
        "Historical causal timestamps keep their forward clamp.");
    Require(entries.All(entry => entry.RecordedAtMicroseconds >= before && entry.RecordedAtMicroseconds <= after),
        "Diagnostic phase timing uses actual runtime recording time even after a future host timestamp.");
}

static void VerifyFlutterSchedulerOrdering()
{
    using var dispatcher = new PlatformDispatcher();
    using var binding = new FixtureSchedulerBinding(dispatcher);
    var order = new List<string>();
    binding.scheduleFrameCallback(_ => order.Add("transient"), scheduleNewFrame: false);
    binding.addPersistentFrameCallback(_ =>
    {
        order.Add("persistent");
        binding.addPostFrameCallback(_ => order.Add("post-during-persistent"));
    });
    binding.addPostFrameCallback(_ => order.Add("post-before-frame"));

    binding.handleBeginFrame(Duration.Create(microseconds: 10_000));
    binding.handleDrawFrame();

    Require(order.SequenceEqual(["transient", "persistent", "post-before-frame", "post-during-persistent"]),
        "Flutter transient, persistent, and post-frame ordering");
    var phases = binding.frameTrace.Snapshot().Select(entry => entry.Phase).ToArray();
    Require(InOrder(phases, DorotiFramePhase.beginFrame, DorotiFramePhase.transientCallbacks,
        DorotiFramePhase.midFrameMicrotasks, DorotiFramePhase.persistentCallbacks,
        DorotiFramePhase.postFrameCallbacks, DorotiFramePhase.drawFrame),
        "scheduler phase trace order");
}

static void VerifyMonotonicTimestampFence()
{
    using var dispatcher = new PlatformDispatcher();
    using var binding = new FixtureSchedulerBinding(dispatcher);
    var timestamps = new List<long>();
    binding.scheduleFrameCallback(timestamp => timestamps.Add(timestamp.inMicroseconds), scheduleNewFrame: false);
    binding.handleBeginFrame(Duration.Create(microseconds: 20_000));
    binding.handleDrawFrame();
    binding.scheduleFrameCallback(timestamp => timestamps.Add(timestamp.inMicroseconds), scheduleNewFrame: false);
    binding.handleBeginFrame(Duration.Create(microseconds: 10_000));
    binding.handleDrawFrame();

    Require(timestamps.Count == 2 && timestamps[1] >= timestamps[0],
        "stale native vsync cannot move scheduler time backwards");
}

static void VerifyBoundedTraceCausality()
{
    var trace = new DorotiFrameTrace();
    trace.Record(DorotiFramePhase.input, 1, TimeSpan.FromMilliseconds(20), inputSequence: 7);
    trace.Record(DorotiFramePhase.sceneSubmitted, 1, TimeSpan.FromMilliseconds(10), inputSequence: 7,
        sceneSequence: 3);
    trace.Record(DorotiFramePhase.present, 1, TimeSpan.FromMilliseconds(30), inputSequence: 7,
        sceneSequence: 3);
    var entries = trace.Snapshot();
    Require(entries.Count == 3, "frame trace retains causal entries");
    Require(entries.Zip(entries.Skip(1), (left, right) => right.TimestampMicroseconds >= left.TimestampMicroseconds).All(value => value),
        "frame trace timestamps are monotonic");
    Require(entries[^1].InputSequence == 7 && entries[^1].SceneSequence == 3,
        "present can be attributed to input and scene sequence");
}

static void VerifyMetricsActivityUsesArrivalClock()
{
    var trace = new DorotiFrameTrace();
    // Simulate a host/browser timestamp that is far ahead of this runtime's
    // stopwatch. Trace ordering may clamp to it, but live-resize activity must
    // still expire according to managed arrival time.
    trace.Record(DorotiFramePhase.present, 1, TimeSpan.FromDays(1));
    trace.Record(DorotiFramePhase.metrics, 1, DorotiFrameClock.Now);
    trace.Record(DorotiFramePhase.metrics, 1, DorotiFrameClock.Now);
    Require(trace.HasActiveMetricsActivity, "a rapid metrics stream is classified as active resize");
    Thread.Sleep(150);
    Require(!trace.HasActiveMetricsActivity,
        "metrics activity expires on the runtime arrival clock despite a future host trace timestamp");
}

static void VerifyPlatformEventDrainsMicrotasks()
{
    using var dispatcher = new PlatformDispatcher();
    var capabilities = new DorotiViewCapabilities("validation")
        .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, new FixtureViewHost());
    using var view = dispatcher.RegisterView(1, capabilities);
    var order = new List<string>();

    view.DispatchPlatformEvent(() =>
    {
        order.Add("event");
        DartAsyncRuntime.scheduleMicrotask(() => order.Add("microtask"));
    });

    Require(order.SequenceEqual(["event", "microtask"]),
        "native platform events drain Dart microtasks before returning to the host");
}

static void VerifyLatestMetricsFrameAdmission()
{
    using var dispatcher = new PlatformDispatcher();
    var host = new FixtureLatestMetricsHost();
    var scenes = new FixtureSceneHost();
    var capabilities = new DorotiViewCapabilities("latest-metrics-fixture")
        .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
        .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
        .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, scenes);
    using var view = dispatcher.RegisterView(1, capabilities);
    dispatcher.drawFrame += frameView =>
    {
        using var scene = new Scene(frameView.viewId, Array.Empty<SceneCommand>());
        frameView.render(scene);
    };

    var epochA = host.ViewEpoch;
    view.ScheduleFrame(DartUiInvocation.Managed("fcr3/latest-metrics"));
    Require(host.PendingCallbackCount == 1, "latest-metrics host keeps one pending callback");
    host.Publish(width: 900, height: 700);
    var epochB = host.ViewEpoch;
    Require(epochB.ResizeTargetGeneration > epochA.ResizeTargetGeneration,
        "fixture advances to epoch B before the pending callback");
    host.Fire();
    host.Publish(width: 1000, height: 720);
    var epochC = host.ViewEpoch;

    Require(scenes.Submissions.Count == 1, "one framework frame submits one scene");
    var token = scenes.Submissions[0].BuildToken;
    Require(token is not null && token.ViewEpoch == epochB,
        "pending callback admits the latest epoch B");
    Require(token!.FrameworkFrameNumber == 1 && dispatcher.frameData.frameNumber == 1,
        "PlatformDispatcher owns one framework frame number");
    var descriptor = DorotiFrameDescriptor.FromBuildToken(token, sceneSequence: 1);
    Require(descriptor.ResizeTargetGeneration == epochB.ResizeTargetGeneration &&
            descriptor.MetricsGeneration == epochB.MetricsGeneration &&
            descriptor.ResizeTargetGeneration != epochC.ResizeTargetGeneration,
        "submitted scene identity is not relabelled after epoch C arrives");
}

static bool InOrder(IReadOnlyList<DorotiFramePhase> values, params DorotiFramePhase[] expected)
{
    var position = 0;
    foreach (var value in values)
    {
        if (position < expected.Length && value == expected[position]) position++;
    }
    return position == expected.Length;
}

static string ConfigurationName() =>
#if DEBUG
    "Debug";
#else
    "Release";
#endif

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class FixtureSchedulerBinding : SchedulerBinding
{
    public FixtureSchedulerBinding(PlatformDispatcher dispatcher) : base(dispatcher) { }
}

sealed class FixtureViewHost : IViewHostCapability
{
    public ViewMetrics Metrics { get; } = new(
        Size.zero, 1, default, default, default, AppLifecycleState.resumed, 1, 1);

    public DorotiViewEpoch ViewEpoch { get; } = new(1, 1, 1, 0, 0, 0, 0, 1, 1, 0);

    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;

    public void Show() { }
    public void Resize(Size logicalSize) => _ = logicalSize;
    public void Close() => Closed?.Invoke();
    public void Dispose()
    {
        GC.KeepAlive(MetricsChanged);
        GC.KeepAlive(LifecycleChanged);
        GC.KeepAlive(CloseRequested);
    }
}

sealed class FixtureLatestMetricsHost : IViewHostCapability, IFrameHostCapability,
    ILatestMetricsFrameHostCapability
{
    private Action<TimeSpan, DorotiViewEpoch>? _callback;
    private long _generation = 1;
    private DorotiViewEpoch _epoch = new(1, 1, 1, 800, 600, 800, 600, 1, 1, 0);

    public int PendingCallbackCount => _callback is null ? 0 : 1;
    public ViewMetrics Metrics => new(
        new Size(_epoch.PhysicalWidth, _epoch.PhysicalHeight), 1,
        default, default, default, AppLifecycleState.resumed,
        _epoch.MetricsGeneration, _epoch.ResizeTargetGeneration);
    public DorotiViewEpoch ViewEpoch => _epoch;
    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;

    public void Publish(int width, int height)
    {
        var generation = ++_generation;
        _epoch = new(1, generation, generation, width, height, width, height, 1, 1,
            DorotiFrameClock.Now.Ticks / 10);
        MetricsChanged?.Invoke(Metrics);
    }

    public void Fire()
    {
        var callback = _callback ?? throw new InvalidOperationException("No frame is pending.");
        _callback = null;
        callback(DorotiFrameClock.Now, _epoch);
    }

    public void ScheduleFrame(Action<TimeSpan> callback) =>
        ScheduleFrame(_epoch, (timestamp, _) => callback(timestamp));
    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan> callback) =>
        ScheduleFrame(expectedEpoch, (timestamp, admitted) =>
        {
            if (admitted == expectedEpoch) callback(timestamp);
        });
    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan, DorotiViewEpoch> callback)
    {
        _ = expectedEpoch;
        _callback = callback;
    }
    public void Show() { }
    public void Resize(Size logicalSize) => _ = logicalSize;
    public void Close() => Closed?.Invoke();
    public void Dispose()
    {
        _callback = null;
        GC.KeepAlive(LifecycleChanged);
        GC.KeepAlive(CloseRequested);
    }
}

sealed class FixtureSceneHost : ISceneHostCapability
{
    public List<DorotiSceneSubmission> Submissions { get; } = [];
    public void Submit(ulong viewId, DorotiSceneSubmission submission, DartUiInvocation invocation)
    {
        _ = viewId;
        _ = invocation;
        Submissions.Add(submission);
    }
}
