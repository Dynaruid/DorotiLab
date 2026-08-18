using Doroti.Framework.Foundation;
using Doroti.Ui;

VerifyMethodTearOffListenerRemoval();
VerifyOneInputKeepsOneCausalIdentity();
VerifyTraceIsBoundedWithoutReusingSequenceNumbers();
VerifyFrameTraceKeepsScrollAndAnimationTransitions();

Console.WriteLine($"FCR-5 scroll runtime contract: PASS (configuration={ConfigurationName()})");

static void VerifyMethodTearOffListenerRemoval()
{
    var notifier = new ChangeNotifier();
    var probe = new ListenerProbe();

    notifier.addListener(probe.OnChanged);
    notifier.notifyListeners();
    notifier.removeListener(probe.OnChanged);
    notifier.notifyListeners();

    Require(probe.CallCount == 1,
        "a Flutter method tear-off maps to a stable CLR delegate for listener removal");
    Require(notifier.debugListenerCount == 0,
        "removed scroll listeners do not remain retained by ChangeNotifier");
    notifier.dispose();
}

static void VerifyOneInputKeepsOneCausalIdentity()
{
    var trace = new DorotiScrollTrace();
    var input = trace.Begin(7, DorotiScrollTracePhase.nativeInput, "wheel");
    foreach (var phase in new[]
    {
        DorotiScrollTracePhase.pointerData,
        DorotiScrollTracePhase.hitTest,
        DorotiScrollTracePhase.gesture,
        DorotiScrollTracePhase.activity,
        DorotiScrollTracePhase.viewport,
        DorotiScrollTracePhase.layout,
        DorotiScrollTracePhase.paint,
        DorotiScrollTracePhase.retainedLayer,
        DorotiScrollTracePhase.raster,
        DorotiScrollTracePhase.present,
        DorotiScrollTracePhase.scrollbar,
        DorotiScrollTracePhase.semantics,
    })
    {
        trace.Record(input, 7, phase);
    }

    var entries = trace.Snapshot(input);
    Require(entries.All(entry => entry.InputSequence == input && entry.ViewId == 7),
        "every causally related scroll event keeps its input and view identity");
    Require(entries.Select(entry => entry.Phase).SequenceEqual(new[]
    {
        DorotiScrollTracePhase.nativeInput,
        DorotiScrollTracePhase.pointerData,
        DorotiScrollTracePhase.hitTest,
        DorotiScrollTracePhase.gesture,
        DorotiScrollTracePhase.activity,
        DorotiScrollTracePhase.viewport,
        DorotiScrollTracePhase.layout,
        DorotiScrollTracePhase.paint,
        DorotiScrollTracePhase.retainedLayer,
        DorotiScrollTracePhase.raster,
        DorotiScrollTracePhase.present,
        DorotiScrollTracePhase.scrollbar,
        DorotiScrollTracePhase.semantics,
    }), "the scroll causal trace preserves stage ordering");
    Require(entries.Zip(entries.Skip(1), (left, right) => right.Sequence > left.Sequence && right.TimestampMicroseconds >= left.TimestampMicroseconds).All(value => value),
        "scroll diagnostics are monotonic");
}

static void VerifyTraceIsBoundedWithoutReusingSequenceNumbers()
{
    var trace = new DorotiScrollTrace();
    long finalInput = 0;
    for (var index = 0; index < 600; index++)
    {
        finalInput = trace.Begin(9, DorotiScrollTracePhase.pointerData);
        trace.Record(finalInput, 9, DorotiScrollTracePhase.hitTest);
    }

    var snapshot = trace.Snapshot();
    Require(snapshot.Count <= 512, "scroll trace remains bounded");
    Require(snapshot.Select(entry => entry.Sequence).Distinct().Count() == snapshot.Count,
        "bounded eviction does not reuse trace sequence numbers");
    Require(trace.Snapshot(finalInput).Count == 2, "the newest input remains individually queryable");
}

static void VerifyFrameTraceKeepsScrollAndAnimationTransitions()
{
    var trace = new DorotiFrameTrace();
    trace.Record(DorotiFramePhase.input, 7, DorotiFrameClock.Now, inputSequence: 42);
    trace.RecordScroll(DorotiFramePhase.scrollStart, 7, 73, 100, null, "IdleScrollActivity");
    trace.RecordScroll(DorotiFramePhase.scrollUpdate, 7, 73, 115, 15, "IdleScrollActivity");
    trace.RecordTicker(DorotiFramePhase.animationStart, 91, "scrollbar");
    trace.RecordScroll(DorotiFramePhase.scrollEnd, 7, 73, 115, null, "IdleScrollActivity");
    trace.RecordTicker(DorotiFramePhase.animationEnd, 91, "scrollbar");

    var transitions = trace.Snapshot();
    Require(transitions.Skip(1).All(entry => entry.InputSequence == 42),
        "scroll and ticker transitions retain the latest native input identity");
    var update = transitions.Single(entry => entry.Phase == DorotiFramePhase.scrollUpdate);
    Require(update.ScrollPositionId == 73 && update.ScrollOffset == 115 && update.ScrollDelta == 15 &&
            update.ScrollActivity == "IdleScrollActivity",
        "frame trace records the actual offset, delta, position, and activity");
    Require(transitions.Any(entry => entry.Phase == DorotiFramePhase.animationStart &&
                                     entry.TickerId == 91 && entry.TickerLabel == "scrollbar"),
        "frame trace identifies the animation owner around scroll boundaries");

    for (var index = 0; index < 8300; index++)
        trace.Record(DorotiFramePhase.scheduleFrame, 7, DorotiFrameClock.Now);
    var bounded = trace.Snapshot();
    Require(bounded.Count == 8192, "frame trace retains a complete high-refresh gesture in a bounded ring");
    Require(bounded.Select(entry => entry.Sequence).Distinct().Count() == bounded.Count,
        "frame-trace eviction does not reuse sequence numbers");
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

sealed class ListenerProbe
{
    public int CallCount { get; private set; }

    public void OnChanged() => CallCount++;
}
