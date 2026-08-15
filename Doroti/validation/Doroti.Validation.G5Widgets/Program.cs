using System.Text;
using System.Text.Json;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Gestures;
using Doroti.Generated.Framework.Painting;
using Doroti.Generated.Framework.Rendering;
using Doroti.Generated.Framework.Services;
using Doroti.Generated.Framework.Widgets;
using Path = System.IO.Path;

var failures = new List<string>();
var traces = new Dictionary<string, List<string>>(StringComparer.Ordinal);
using var dispatcher = new PlatformDispatcher();
using var scope = dispatcher.EnterScope();
var fixtureHost = new FixtureHost();
using var view = dispatcher.RegisterView(533, new DorotiViewCapabilities("g5-3-widgets-managed")
    .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, fixtureHost)
    .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, fixtureHost)
    .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, fixtureHost));
using var binding = new WidgetsFlutterBinding(dispatcher);

if (args.Length >= 1 && string.Equals(args[0], "--g7-focus-frame-dispatch-probe", StringComparison.Ordinal))
{
    var probeEvidencePath = args.Length >= 2
        ? Path.GetFullPath(args[1])
        : Path.Combine(FindDorotiRoot(Environment.CurrentDirectory), "migration", "flutter-framework", "g7-managed-regression.json");
    DorotiCapabilityException? blocker = null;
    try
    {
        RunFocusShortcutAction(traces, failures);
    }
    catch (DorotiCapabilityException exception)
    {
        blocker = exception;
    }

    traces.TryGetValue("W4", out var focusTrace);
    var passed = blocker is null &&
        failures.Count == 0 &&
        focusTrace is not null &&
        focusTrace.SequenceEqual(["focus:editor", "shortcut:accepted", "intent:invoked", "focus:root"]);
    WriteJson(probeEvidencePath, new
    {
        schemaVersion = "doroti.g7-managed-regression/v1",
        milestone = "G7-1C",
        capturedAtUtc = DateTimeOffset.UtcNow,
        status = passed ? "pass" : "failed",
        fixture = "G5 Widgets focus request on a managed view with an explicit per-view frame dispatcher",
        expected = new
        {
            capabilityId = DorotiCapabilityIds.ViewFrameDispatch,
            viewId = 533,
            targetIdentity = "g5-3-widgets-managed",
            elementId = "dart:ui#PlatformDispatcher.microtask",
            registration = "per-view fixture frame dispatcher",
        },
        actual = new
        {
            frameDispatchCount = fixtureHost.FrameDispatchCount,
            focusTrace,
            exception = blocker?.ToString(),
        },
        owner = "managed view bootstrap and per-view frame-dispatch capability",
        followUpMilestone = (string?)null,
    });
    Console.WriteLine($"G7-1C focus/frame-dispatch managed regression: {(passed ? "PASS" : "FAIL")}");
    return passed ? 0 : 1;
}

RunStatelessLifecycle(traces, failures);
RunStatefulLifecycle(traces, failures);
RunInheritedAndKeyedReconciliation(traces, failures);
RunFocusShortcutAction(traces, failures);
RunOverlayAndRoute(traces, failures);
await RunScrollingAndImageAsync(traces, failures);
RunEditableText(traces, failures);
RunHitTestWrap(traces, failures);

var root = FindDorotiRoot(Environment.CurrentDirectory);
var evidencePath = args.Length == 0
    ? Path.Combine(root, "migration", "flutter-framework", "g5-3-widgets-behavior.json")
    : Path.GetFullPath(args[0]);
var expected = new Dictionary<string, string[]>(StringComparer.Ordinal)
{
    ["W0-W1"] = ["build:first", "mounted:true", "scheduled", "build:second", "updated", "unmounted:false"],
    ["W2"] = ["initState", "didChangeDependencies", "build:0", "setState:1", "scheduled", "build:1", "scheduled", "didUpdateWidget", "build:1", "scheduled", "deactivate", "dispose"],
    ["W3"] = ["dependency:1", "dependency:2", "keyed-state:preserved"],
    ["W4"] = ["focus:editor", "shortcut:accepted", "intent:invoked", "focus:root"],
    ["W5"] = ["overlay:back", "overlay:front", "route:install", "route:push", "route:pop:result", "route:complete:result", "route:dispose"],
    ["W6"] = ["list:1000", "extent:50/50", "boundary:20/0", "image:async", "image:lifetime"],
    ["W7"] = ["edit:doroti", "selection:1-4", "composition:-1--1", "revision:3"],
    ["hit-test-wrap"] = ["box:shared", "sliver:shared"],
};
foreach (var (name, reference) in expected)
{
    if (!traces.TryGetValue(name, out var actual) || !actual.SequenceEqual(reference))
    {
        failures.Add($"{name}: reference={string.Join(',', reference)} actual={string.Join(',', actual ?? [])}");
    }
}

WriteJson(evidencePath, new
{
    schemaVersion = "doroti.g5-3-widgets-behavior/v1",
    milestone = "G5-3C",
    capturedAtUtc = DateTimeOffset.UtcNow,
    status = failures.Count == 0 ? "verified-managed-product" : "failed",
    reference = "Flutter 56b8e1a8 lifecycle contracts encoded as exact ordered traces",
    traces,
    verifiedSlices = new[] { "W0", "W1", "W2", "W3", "W4", "W5", "W6", "W7" },
    pendingSlices = Array.Empty<string>(),
    failures,
});

Console.WriteLine($"G5-3 Widgets W0-W7 and hit-test wrap managed product behavior: {(failures.Count == 0 ? "PASS" : "FAIL")}");
foreach (var failure in failures) Console.Error.WriteLine(failure);
return failures.Count == 0 ? 0 : 1;

static void RunStatelessLifecycle(Dictionary<string, List<string>> traces, List<string> failures)
{
    var events = new List<string>();
    var owner = NewOwner(events);
    var root = new RootWidget(child: new TraceStateless(events, "first"));
    var element = root.attach(owner);
    events.Add($"mounted:{element.mounted.ToString().ToLowerInvariant()}");
    owner.buildScope(element);
    new RootWidget(child: new TraceStateless(events, "second")).attach(owner, element);
    owner.buildScope(element);
    events.Add("updated");
    element.unmount();
    events.Add($"unmounted:{element.mounted.ToString().ToLowerInvariant()}");
    traces["W0-W1"] = events;
}

static void RunStatefulLifecycle(Dictionary<string, List<string>> traces, List<string> failures)
{
    var events = new List<string>();
    var owner = NewOwner(events);
    var key = new GlobalKey<IState>("stateful");
    var probe = new StateProbe();
    var element = new RootWidget(child: new TraceStateful(events, probe, key)).attach(owner);
    owner.buildScope(element);
    if (probe.State is not TraceState state)
    {
        failures.Add("W2: GlobalKey did not expose the mounted State.");
        return;
    }
    state.Increment();
    owner.buildScope((Element)state.context);
    new RootWidget(child: new TraceStateful(events, probe, key)).attach(owner, element);
    owner.buildScope(element);
    new RootWidget(child: null).attach(owner, element);
    owner.buildScope(element);
    owner.finalizeTree();
    traces["W2"] = events;
}

static void RunInheritedAndKeyedReconciliation(Dictionary<string, List<string>> traces, List<string> failures)
{
    var events = new List<string>();
    var owner = NewOwner();
    var keyed = new GlobalKey<IState>("reparent");
    var probe = new StateProbe();
    var first = new PairWidget([
        new TraceInherited(1, new DependencyReader(events)),
        new TraceStateful(events, probe, keyed),
    ]);
    var element = new RootWidget(child: first).attach(owner);
    owner.buildScope(element);
    var originalState = probe.State;
    var second = new PairWidget([
        new TraceStateful(events, probe, keyed),
        new TraceInherited(2, new DependencyReader(events)),
    ]);
    new RootWidget(child: second).attach(owner, element);
    owner.buildScope(element);
    if (originalState is not null && ReferenceEquals(originalState, probe.State))
    {
        events.Add("keyed-state:preserved");
    }
    else
    {
        failures.Add("W3: GlobalKey reparent did not preserve State identity.");
    }
    element.deactivate();
    element.unmount();
    traces["W3"] = events.Where(item => item.StartsWith("dependency:", StringComparison.Ordinal) || item == "keyed-state:preserved").ToList();
}

static void RunFocusShortcutAction(Dictionary<string, List<string>> traces, List<string> failures)
{
    var events = new List<string>();
    var owner = NewOwner();
    var context = new RootWidget(child: new TestLeaf()).attach(owner);
    owner.buildScope(context);
    var node = new FocusNode(debugLabel: "editor");
    var attachment = node.attach(context);
    attachment.reparent(owner.focusManager.rootScope);
    node.requestFocus();
    owner.focusManager.applyFocusChangesIfNeeded();
    events.Add(ReferenceEquals(owner.focusManager.primaryFocus, node) ? "focus:editor" : "focus:missing");

    var keyEvent = new KeyDownEvent(PhysicalKeyboardKey.keyA, LogicalKeyboardKey.keyA, character: "a", timeStamp: Duration.zero);
    owner.focusManager.rootScope.context?.owner?.focusManager.applyFocusChangesIfNeeded();
    var keyboard = ServicesBinding.instance.keyboard;
    keyboard.handleKeyEvent(keyEvent);
    var activator = new SingleActivator(LogicalKeyboardKey.keyA);
    if (activator.accepts(keyEvent, keyboard)) events.Add("shortcut:accepted");
    var intent = new ProbeIntent();
    var action = new CallbackAction<ProbeIntent>(_ => { events.Add("intent:invoked"); return null; });
    action.invoke(intent, context);
    keyboard.handleKeyEvent(new KeyUpEvent(PhysicalKeyboardKey.keyA, LogicalKeyboardKey.keyA, timeStamp: Duration.zero));

    node.unfocus();
    owner.focusManager.applyFocusChangesIfNeeded();
    events.Add(ReferenceEquals(owner.focusManager.primaryFocus, owner.focusManager.rootScope) ? "focus:root" : "focus:missing");
    attachment.detach();
    node.dispose();
    context.unmount();
    traces["W4"] = events;
}

static void RunOverlayAndRoute(Dictionary<string, List<string>> traces, List<string> failures)
{
    var events = new List<string>();
    var owner = NewOwner();
    var back = new OverlayEntry(_ => new TraceOverlayLeaf(events, "back"));
    var front = new OverlayEntry(_ => new TraceOverlayLeaf(events, "front"));
    var root = new RootWidget(child: new Directionality(textDirection: TextDirection.ltr, child: new Overlay(initialEntries: [back, front])));
    var element = root.attach(owner);
    owner.buildScope(element);
    element.deactivate();
    element.unmount();

    var route = new TraceRoute(events);
    route.install();
    _ = route.didPush();
    route.didPop("result");
    route.dispose();
    traces["W5"] = events;
}

static async Task RunScrollingAndImageAsync(Dictionary<string, List<string>> traces, List<string> failures)
{
    var events = new List<string>();
    var owner = NewOwner();
    var context = new RootWidget(child: new TestLeaf()).attach(owner);
    owner.buildScope(context);
    var built = 0;
    var list = new SliverChildBuilderDelegate(
        (_, index) => { built++; return new TestLeaf(); },
        childCount: 1000,
        addAutomaticKeepAlives: false,
        addRepaintBoundaries: false,
        addSemanticIndexes: false);
    for (var index = 0L; index < 1000; index++)
    {
        if (list.build(context, index) is null) failures.Add($"W6: item {index} was not built.");
    }
    if (list.build(context, 1000) is not null) failures.Add("W6: builder exceeded childCount.");
    events.Add($"list:{built}");

    var metrics = new FixedScrollMetrics(0, 100, 50, 50, AxisDirection.down, 1);
    events.Add($"extent:{metrics.extentBefore:0}/{metrics.extentAfter:0}");
    var physics = new ClampingScrollPhysics();
    events.Add($"boundary:{physics.applyBoundaryConditions(metrics, 120):0}/{physics.applyBoundaryConditions(metrics, 80):0}");

    var completion = new TaskCompletionSource<ImageInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
    var completer = new TraceImageCompleter(Future<ImageInfo>.fromTask(completion.Task));
    var stream = new ImageStream();
    var received = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var listener = new ImageStreamListener((info, synchronous) =>
    {
        events.Add(synchronous ? "image:sync" : "image:async");
        info.dispose();
        received.TrySetResult();
    });
    stream.addListener(listener);
    stream.setCompleter(completer);
    var keepAlive = completer.keepAlive();
    completion.SetResult(new ImageInfo(new Doroti.Ui.Image(533, 2, 2), debugLabel: "g5-3"));
    await received.Task.WaitAsync(TimeSpan.FromSeconds(5));
    stream.removeListener(listener);
    if (completer.Disposed) failures.Add("W6: image completer disposed while keepAlive was held.");
    keepAlive.dispose();
    events.Add(completer.Disposed ? "image:lifetime" : "image:not-disposed");
    context.unmount();
    traces["W6"] = events;
}

static void RunEditableText(Dictionary<string, List<string>> traces, List<string> failures)
{
    var events = new List<string>();
    using var controller = new TextEditingController("seed");
    var revisions = 0;
    controller.addListener(() => revisions++);
    controller.value = new TextEditingValue(
        text: "doroti",
        selection: new TextSelection(baseOffset: 1, extentOffset: 4),
        composing: new TextRange(1, 4));
    events.Add($"edit:{controller.text}");
    events.Add($"selection:{controller.selection.start}-{controller.selection.end}");
    controller.clearComposing();
    events.Add($"composition:{controller.value.composing.start}-{controller.value.composing.end}");
    controller.selection = TextSelection.CreateCollapsed(2);
    events.Add($"revision:{revisions}");
    traces["W7"] = events;
}

static void RunHitTestWrap(Dictionary<string, List<string>> traces, List<string> failures)
{
    var events = new List<string>();
    var target = new ProbeHitTestTarget();

    var boxRoot = new HitTestResult();
    var boxWrap = BoxHitTestResult.CreateWrap(boxRoot);
    boxWrap.add(new HitTestEntry<HitTestTarget>(target));
    events.Add(boxRoot.path.Single().target == target ? "box:shared" : "box:detached");

    var sliverRoot = new HitTestResult();
    var sliverWrap = SliverHitTestResult.CreateWrap(sliverRoot);
    sliverWrap.add(new HitTestEntry<HitTestTarget>(target));
    events.Add(sliverRoot.path.Single().target == target ? "sliver:shared" : "sliver:detached");

    traces["hit-test-wrap"] = events;
}

static BuildOwner NewOwner(List<string>? events = null) =>
    new(events is null ? null : () => events.Add("scheduled"), new FocusManager());

static string FindDorotiRoot(string start)
{
    for (var directory = new DirectoryInfo(Path.GetFullPath(start)); directory is not null; directory = directory.Parent)
    {
        if (File.Exists(Path.Combine(directory.FullName, "Doroti.slnx"))) return directory.FullName;
        var nested = Path.Combine(directory.FullName, "Doroti");
        if (File.Exists(Path.Combine(nested, "Doroti.slnx"))) return nested;
    }
    throw new DirectoryNotFoundException("Doroti.slnx was not found.");
}

static void WriteJson(string path, object value)
{
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    var temporary = path + ".tmp-" + Guid.NewGuid().ToString("N");
    File.WriteAllText(temporary, JsonSerializer.Serialize(value, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    }) + "\n", new UTF8Encoding(false));
    File.Move(temporary, path, true);
}

sealed class TraceStateless(List<string> events, string value) : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        events.Add($"build:{value}");
        return new TestLeaf();
    }
}

sealed class StateProbe
{
    public TraceState? State { get; set; }
}

sealed class TraceStateful(List<string> events, StateProbe probe, Key? key = null) : StatefulWidget(key)
{
    public override IState createState() => probe.State = new TraceState(events);
}

sealed class TraceState(List<string> events) : State<TraceStateful>
{
    private int _value;

    public override void initState()
    {
        events.Add("initState");
        base.initState();
    }

    public override void didChangeDependencies()
    {
        events.Add("didChangeDependencies");
        base.didChangeDependencies();
    }

    public override Widget build(BuildContext context)
    {
        events.Add($"build:{_value}");
        return new TestLeaf();
    }

    public override void didUpdateWidget(TraceStateful oldWidget)
    {
        events.Add("didUpdateWidget");
        base.didUpdateWidget(oldWidget);
    }

    public override void deactivate()
    {
        events.Add("deactivate");
        base.deactivate();
    }

    public override void dispose()
    {
        events.Add("dispose");
        base.dispose();
    }

    public void Increment() => setState(() => events.Add($"setState:{++_value}"));
}

sealed class TraceInherited(int value, Widget child) : InheritedWidget(child)
{
    public int Value { get; } = value;
    public override bool updateShouldNotify(InheritedWidget oldWidget) => oldWidget is TraceInherited old && old.Value != Value;
}

sealed class DependencyReader(List<string> events) : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        events.Add($"dependency:{context.dependOnInheritedWidgetOfExactType<TraceInherited>()?.Value}");
        return new TestLeaf();
    }
}

sealed class PairWidget(IReadOnlyList<Widget> children) : Widget
{
    public IReadOnlyList<Widget> Children { get; } = children;
    public override Element createElement() => new PairElement(this);
}

sealed class PairElement(PairWidget widget) : Element(widget)
{
    private List<Element> _children = [];

    public override void mount(Element? parent, object? newSlot)
    {
        base.mount(parent, newSlot);
        _children = updateChildren(_children, ((PairWidget)this.widget).Children.ToList());
    }

    public override void update(Widget newWidget)
    {
        base.update(newWidget);
        _children = updateChildren(_children, ((PairWidget)this.widget).Children.ToList());
    }

    public override void visitChildren(System.Action<Element> visitor)
    {
        foreach (var child in _children) visitor(child);
    }

    public override void forgetChild(Element child)
    {
        _children.Remove(child);
        base.forgetChild(child);
    }
}

sealed class TestLeaf : Widget
{
    public override Element createElement() => new TestLeafElement(this);
}

sealed class TestLeafElement(Widget widget) : Element(widget);

sealed class ProbeHitTestTarget : HitTestTarget
{
    public void handleEvent(Doroti.Generated.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
    }
}

sealed class TraceOverlayLeaf(List<string> events, string label) : StatelessWidget
{
    public override Widget build(BuildContext context)
    {
        events.Add($"overlay:{label}");
        return new TestLeaf();
    }
}

sealed class ProbeIntent : Intent;

sealed class TraceRoute(List<string> events) : Route<string>
{
    public override void install() => events.Add("route:install");
    public override Doroti.Generated.Framework.Scheduler.TickerFuture didPush()
    {
        events.Add("route:push");
        return Doroti.Generated.Framework.Scheduler.TickerFuture.CreateComplete();
    }
    public override bool didPop(string? result)
    {
        events.Add($"route:pop:{result}");
        return base.didPop(result);
    }
    public override void didComplete(string? result)
    {
        events.Add($"route:complete:{result}");
        base.didComplete(result);
    }
    public override void dispose()
    {
        events.Add("route:dispose");
        base.dispose();
    }
}

sealed class TraceImageCompleter(Future<ImageInfo> image) : OneFrameImageStreamCompleter(image)
{
    public bool Disposed { get; private set; }
    public override void onDisposed() => Disposed = true;
}

sealed class FixtureHost : IViewHostCapability, IFrameHostCapability, IPlatformMessageHostCapability
{
    public int FrameDispatchCount { get; private set; }
    public ViewMetrics Metrics { get; } = new(new Size(800, 600), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
    public event System.Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event System.Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event System.Action? CloseRequested { add { } remove { } }
    public event System.Action? Closed { add { } remove { } }
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    public void ScheduleFrame(System.Action<TimeSpan> callback)
    {
        FrameDispatchCount++;
        callback(TimeSpan.FromMilliseconds(FrameDispatchCount * 16));
    }
    public void Show() { }
    public void Resize(Size logicalSize) { }
    public void Close() { }
    public void Dispose() { }
}
