using System.Reflection;
using Doroti.Framework.Animation;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Ui;

VerifyMethodTearOffListenerRemoval();
VerifyOneInputKeepsOneCausalIdentity();
VerifyTraceIsBoundedWithoutReusingSequenceNumbers();
VerifyFrameTraceKeepsScrollAndAnimationTransitions();
VerifyScrollMetricsDepthConversion();
VerifyNestedScrollbarOwnership();

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

static void VerifyScrollMetricsDepthConversion()
{
    var metrics = Metrics(min: 0, max: 840, pixels: 120, viewport: 240);
    foreach (var depth in new long[] { 0, 1, 2 })
    {
        var metricsNotification = new ScrollMetricsNotification(metrics, null!) { _depth = depth };
        var update = metricsNotification.asScrollUpdate();
        Require(update.depth == depth,
            $"ScrollMetricsNotification.asScrollUpdate preserves depth {depth}");
        Require(Scroll_notificationLibrary.defaultScrollNotificationPredicate(update) == (depth == 0),
            $"the default predicate accepts only local depth for depth {depth}");
    }
}

static void VerifyNestedScrollbarOwnership()
{
    var outer = new ScrollbarOwnershipProbe("outer-scrollbar");
    var inner = new ScrollbarOwnershipProbe("inner-scrollbar");
    var outerInitial = Metrics(min: 0, max: 1_200, pixels: 180, viewport: 360);
    var innerInitial = Metrics(min: 0, max: 420, pixels: 30, viewport: 120);

    outer.HandleMetrics(new ScrollMetricsNotification(outerInitial, null!), "outer-position");
    inner.HandleMetrics(new ScrollMetricsNotification(innerInitial, null!), "inner-position");
    var outerBeforeInner = outer.Snapshot();

    inner.Handle(new ScrollStartNotification(innerInitial, null!), "inner-position", "start");
    var remoteStart = new ScrollStartNotification(innerInitial, null!) { _depth = 1 };
    outer.Handle(remoteStart, "inner-position", "start");

    var innerProgress = Metrics(min: 0, max: 420, pixels: 210, viewport: 120);
    inner.Handle(new ScrollUpdateNotification(innerProgress, null!, scrollDelta: 180, depth: 0),
        "inner-position", "update");
    outer.Handle(new ScrollUpdateNotification(innerProgress, null!, scrollDelta: 180, depth: 1),
        "inner-position", "update");

    inner.Handle(new ScrollEndNotification(innerProgress, null!), "inner-position", "end");
    var remoteEnd = new ScrollEndNotification(innerProgress, null!) { _depth = 1 };
    outer.Handle(remoteEnd, "inner-position", "end");

    var innerResized = Metrics(min: 0, max: 510, pixels: 240, viewport: 90);
    var resizedMetrics = new ScrollMetricsNotification(innerResized, null!);
    inner.HandleMetrics(resizedMetrics, "inner-position");
    resizedMetrics._depth = 1;
    outer.HandleMetrics(resizedMetrics, "inner-position");

    Require(outer.Snapshot() == outerBeforeInner,
        "inner start/update/end and viewport metrics leave the outer painter metrics, thumb, and fade ownership unchanged");
    Require(inner.LastMetrics == MetricsSnapshot.From(innerResized),
        "the inner scrollbar receives the resized inner metrics");
    Require(inner.Snapshot().Thumb != outerBeforeInner.Thumb,
        "numerically distinct inner content and viewport metrics produce a distinct inner thumb");

    var innerBeforeOuter = inner.Snapshot();
    var outerProgress = Metrics(min: 0, max: 1_200, pixels: 600, viewport: 360);
    outer.Handle(new ScrollUpdateNotification(outerProgress, null!, scrollDelta: 420, depth: 0),
        "outer-position", "update");
    Require(outer.LastMetrics == MetricsSnapshot.From(outerProgress),
        "an outer drag starts with and keeps outer metrics without a recovery update");
    Require(inner.Snapshot() == innerBeforeOuter,
        "an outer drag does not mutate the inner scrollbar state");

    Require(outer.Diagnostics.Where(entry => entry.Source == "inner-position").All(entry => entry.Depth == 1 && !entry.Accepted),
        "every bubbled inner event is diagnosed as remote and rejected by the outer scrollbar");
    Require(inner.Diagnostics.Where(entry => entry.Source == "inner-position").All(entry => entry.Depth == 0 && entry.Accepted),
        "the same inner event sequence is local and accepted by the inner scrollbar");

    foreach (var entry in outer.Diagnostics.Concat(inner.Diagnostics))
    {
        Console.WriteLine(
            $"FCR-5 notification receiver={entry.Receiver}; source={entry.Source}; event={entry.EventType}; " +
            $"depth={entry.Depth}; axis={entry.Axis}; pixels={entry.Pixels}; viewport={entry.Viewport}; " +
            $"min={entry.Min}; max={entry.Max}; accepted={entry.Accepted}");
    }
}

static FixedScrollMetrics Metrics(double min, double max, double pixels, double viewport) => new(
    minScrollExtent: min,
    maxScrollExtent: max,
    pixels: pixels,
    viewportDimension: viewport,
    axisDirection: AxisDirection.down,
    devicePixelRatio: 1);

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

sealed class ScrollbarOwnershipProbe
{
    private static readonly PropertyInfo ThumbRectProperty = typeof(ScrollbarPainter).GetProperty(
        "_thumbRect", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("ScrollbarPainter._thumbRect is unavailable.");
    private readonly ScrollbarPainter _painter = new(
        color: new Color(0xff202124L),
        fadeoutOpacityAnimation: new AlwaysStoppedAnimation<double>(1),
        textDirection: TextDirection.ltr,
        thickness: 6,
        padding: EdgeInsets.zero);

    public ScrollbarOwnershipProbe(string identity) => Identity = identity;

    public string Identity { get; }
    public int AcceptedEventCount { get; private set; }
    public int FadeOwnershipTransitions { get; private set; }
    public MetricsSnapshot? LastMetrics { get; private set; }
    public ThumbSnapshot? LastThumb { get; private set; }
    public List<ScrollDiagnostic> Diagnostics { get; } = [];

    public void HandleMetrics(ScrollMetricsNotification notification, string source)
    {
        Handle(notification.asScrollUpdate(), source, "metrics");
    }

    public void Handle(ScrollNotification notification, string source, string eventType)
    {
        var accepted = Scroll_notificationLibrary.defaultScrollNotificationPredicate(notification);
        Diagnostics.Add(ScrollDiagnostic.From(Identity, source, eventType, notification, accepted));
        if (!accepted) return;

        AcceptedEventCount++;
        if (notification is ScrollStartNotification or ScrollEndNotification || eventType == "metrics")
            FadeOwnershipTransitions++;
        LastMetrics = MetricsSnapshot.From(notification.metrics);
        _painter.update(notification.metrics, notification.metrics.axisDirection);
        var commands = new List<PathCommand>();
        _painter.paint(new Canvas(commands), new Size(720, 360));
        var rect = (Rect?)ThumbRectProperty.GetValue(_painter)
            ?? throw new InvalidOperationException("Accepted scroll metrics did not produce a thumb rect.");
        LastThumb = new(rect.left, rect.top, rect.right, rect.bottom);
    }

    public ScrollbarSnapshot Snapshot() => new(
        AcceptedEventCount,
        FadeOwnershipTransitions,
        LastMetrics,
        LastThumb);
}

sealed record MetricsSnapshot(
    Axis Axis,
    double Pixels,
    double Viewport,
    double Min,
    double Max)
{
    public static MetricsSnapshot From(ScrollMetrics metrics) => new(
        metrics.axis,
        metrics.pixels,
        metrics.viewportDimension,
        metrics.minScrollExtent,
        metrics.maxScrollExtent);
}

sealed record ThumbSnapshot(double Left, double Top, double Right, double Bottom);

sealed record ScrollbarSnapshot(
    int AcceptedEventCount,
    int FadeOwnershipTransitions,
    MetricsSnapshot? Metrics,
    ThumbSnapshot? Thumb);

sealed record ScrollDiagnostic(
    string Receiver,
    string Source,
    string EventType,
    long Depth,
    Axis Axis,
    double Pixels,
    double Viewport,
    double Min,
    double Max,
    bool Accepted)
{
    public static ScrollDiagnostic From(
        string receiver,
        string source,
        string eventType,
        ScrollNotification notification,
        bool accepted) => new(
            receiver,
            source,
            eventType,
            notification.depth,
            notification.metrics.axis,
            notification.metrics.pixels,
            notification.metrics.viewportDimension,
            notification.metrics.minScrollExtent,
            notification.metrics.maxScrollExtent,
            accepted);
}
