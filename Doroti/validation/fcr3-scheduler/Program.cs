using Doroti.Framework.Scheduler;
using Doroti.Runtime;
using Doroti.Ui;

VerifyFlutterSchedulerOrdering();
VerifyMonotonicTimestampFence();
VerifyBoundedTraceCausality();

Console.WriteLine($"FCR-3 scheduler runtime contract: PASS (configuration={ConfigurationName()})");

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
