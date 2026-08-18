using Doroti.Ui;

VerifyOneInputKeepsOneCausalIdentity();
VerifyTraceIsBoundedWithoutReusingSequenceNumbers();

Console.WriteLine($"FCR-5 scroll runtime contract: PASS (configuration={ConfigurationName()})");

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
