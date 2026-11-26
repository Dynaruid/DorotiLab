using System.Reflection;
using System.Runtime.CompilerServices;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Scheduler;
using Doroti.Framework.Semantics;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using SemanticsBinding = Doroti.Framework.Semantics.SemanticsBinding;

internal static class ScrollSemanticsRegression
{
    internal static void Run()
    {
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var scheduler = new TestScheduler(dispatcher);
        Check(!((ISemanticsHostCapability)new PassiveHost()).CoalesceScrollOffsetsDuringActiveScroll,
            "unknown hosts retain immediate scroll updates unless they explicitly opt in");
        var oldBinding = SemanticsBinding._instance;
        SemanticsBinding._instance = DispatchProxy.Create<SemanticsBinding, SemanticsProxy>();
        var position = new Position(new Context());
        position.applyViewportDimension(200);
        position.applyContentDimensions(0, 1000);
        var scroll = Create(position);
        var root = new RenderSemanticsAnnotations(child: scroll, container: true, textDirection: TextDirection.ltr,
            properties: new SemanticsProperties(label: "before", textDirection: TextDirection.ltr));
        var updates = new List<SemanticsUpdate>();
        var owner = new PipelineOwner(onSemanticsUpdate: updates.Add) { rootNode = root };
        var handle = owner.ensureSemantics();
        try
        {
            root.layout(BoxConstraints.CreateTight(new Size(200, 200)));
            root.scheduleInitialSemantics();
            owner.flushSemantics();
            Check(updates.Count > 0 && !owner.hasPendingSemanticsUpdate, "initial semantics are built");
            void Move(double pixels) { position.correctPixels(pixels); position.notifyListeners(); }
            Move(12.5);
            Move(25.25);
            var semantics = typeof(RenderObject).GetProperty("_semantics", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(scroll)!;
            var provider = semantics.GetType().GetProperty("configProvider")!.GetValue(semantics)!;
            Check(provider.GetType().GetProperty("_originalConfiguration", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(provider) is null,
                "repeated queued offsets leave the configuration lazy until flush");
            Check(owner.hasPendingSemanticsUpdate && !owner.hasPendingSemanticsNonGeometryUpdate,
                "repeated interior scroll values retain coalescible work");
            Check(owner.hasPendingSemanticsContentUpdate, "the conservative legacy content predicate is preserved");
            root.properties = new SemanticsProperties(label: "changed", value: "latest", onTap: () => { }, textDirection: TextDirection.ltr);
            Check(owner.hasPendingSemanticsNonGeometryUpdate, "label/value/action changes stay urgent during scrolling");
            Move(40.5);
            Check(owner.hasPendingSemanticsNonGeometryUpdate, "later scroll notification cannot downgrade pending content");
            owner.flushSemantics();
            Check(updates[^1].nodes.Any(n => n.label == "changed" && n.value == "latest" && (n.actions & SemanticsAction.tap) != 0),
                "interactive content reaches the real semantics builder");
            Check(updates[^1].nodes.Any(n => n.scrollPosition == 40.5), "flush publishes the latest fractional offset");
            Move(50.75);
            Check(!owner.hasPendingSemanticsNonGeometryUpdate, "a completed content flush does not poison later geometry classification");
            owner.flushSemantics();
            Check(updates[^1].nodes.Any(n => n.scrollPosition == 50.75), "final geometry flush retains the exact offset");
            var child = ((RenderProxyBox)((RenderProxyBox)scroll).child!).child!;
            child.markNeedsSemanticsGeometryUpdate();
            Check(!owner.hasPendingSemanticsNonGeometryUpdate, "layout-only ancestor work can be coalesced");
            child.markNeedsSemanticsUpdate();
            Check(owner.hasPendingSemanticsNonGeometryUpdate, "content arriving through a geometry-dirty ancestor stays urgent");
            owner.flushSemantics();
            position.applyContentDimensions(0, 900);
            position.notifyListeners();
            Check(owner.hasPendingSemanticsNonGeometryUpdate, "extent changes retain the content path");
            owner.flushSemantics();
            root.properties = new SemanticsProperties(label: "content first", textDirection: TextDirection.ltr);
            Move(60.125);
            Check(owner.hasPendingSemanticsNonGeometryUpdate, "content-first notification order also wins");
            owner.flushSemantics();
            scroll.GetType().GetProperty("allowImplicitScrolling")!.SetValue(scroll, false);
            Move(70.25);
            Check(owner.hasPendingSemanticsNonGeometryUpdate, "scroll action policy changes stay urgent");
            owner.flushSemantics();
            CheckMergedEditableTextGeometry();
        }
        finally
        {
            handle.dispose();
            owner.rootNode = null;
            root.dispose(); scroll.dispose(); position.dispose(); owner.dispose();
            SemanticsBinding._instance = oldBinding;
        }
        var externalPosition = new Position(new Context());
        var disposed = DisposedRenderer(externalPosition);
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        Check(!disposed.IsAlive, "a surviving position does not retain disposed scroll renderers or their subtrees");
        externalPosition.dispose();
        Console.WriteLine("PASS: scroll semantics coalescing, content priority, exact offsets, disposed listeners and merged editable text geometry");
    }

    private static void CheckMergedEditableTextGeometry()
    {
        const string value = "x 👩‍💻";
        var textGeometry = new SemanticsTextGeometry(value, 1,
            [new SemanticsTextRun(2, 7, new Rect(4, 5, 18, 19), 0, 7)]);
        var editable = new EditableSemanticsBox(textGeometry);
        var padding = new RenderPadding(EdgeInsets.CreateOnly(left: 10, top: 20)) { child = editable };
        var field = new RenderSemanticsAnnotations(child: padding, container: true, textDirection: TextDirection.ltr,
            properties: new SemanticsProperties(label: "edit label", textDirection: TextDirection.ltr));
        var root = new RenderProxyBox(field);
        var updates = new List<SemanticsUpdate>();
        var owner = new PipelineOwner(onSemanticsUpdate: updates.Add) { rootNode = root };
        var handle = owner.ensureSemantics();
        try
        {
            root.layout(BoxConstraints.CreateTight(new Size(100, 60)));
            root.scheduleInitialSemantics();
            owner.flushSemantics();
            var initial = updates[^1].nodes.Single(node => node.value == value);
            Check(initial.label == "edit label" && initial.textGeometry?.Runs.Single().Bounds == new Rect(14, 25, 28, 39),
                "the container's editable value carries child text geometry in container coordinates");
            Check(initial.textGeometry!.Snap(4) == 2 && initial.textGeometry.Snap(4, true) == 7,
                "merged text geometry retains the emoji boundary");

            padding.padding = EdgeInsets.CreateOnly(left: 16, top: 23);
            owner.flushLayout();
            owner.flushSemantics();
            var moved = updates[^1].nodes.Single(node => node.id == initial.id);
            Check(moved.textGeometry?.Runs.Single().Bounds == new Rect(20, 28, 34, 42),
                "a moved child republishes bounds even when its paragraph revision is unchanged");

            editable.Obscure();
            owner.flushSemantics();
            var obscured = updates[^1].nodes.Single(node => node.id == initial.id);
            Check(obscured.textGeometry is null && obscured.flags?.isObscured == true,
                "an obscured field clears the merged text geometry");
        }
        finally
        {
            handle.dispose();
            owner.rootNode = null;
            root.dispose(); field.dispose(); padding.dispose(); editable.dispose(); owner.dispose();
        }
    }

    private sealed class EditableSemanticsBox(SemanticsTextGeometry geometry) : RenderProxyBox
    {
        private bool _obscured;

        public void Obscure()
        {
            _obscured = true;
            markNeedsSemanticsUpdate();
        }

        public override void describeSemanticsConfiguration(SemanticsConfiguration config)
        {
            base.describeSemanticsConfiguration(config);
            config.isTextField = true;
            config.isObscured = _obscured;
            config.textDirection = TextDirection.ltr;
            config.value = _obscured ? new string('•', geometry.Text.Length) : geometry.Text;
            config.hostTextGeometry = _obscured ? null : geometry;
        }
    }

    private static RenderBox Create(ScrollPosition position) => (RenderBox)Activator.CreateInstance(
        typeof(Scrollable).Assembly.GetType("Doroti.Framework.Widgets._RenderScrollSemantics__scrollable")!,
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
        [position, true, Axis.vertical, (long?)10, new RenderConstrainedBox(child: new RenderProxyBox(),
            additionalConstraints: BoxConstraints.CreateTight(new Size(200, 200)))], null)!;
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference DisposedRenderer(ScrollPosition position)
    {
        var render = Create(position);
        render.dispose();
        return new WeakReference(render);
    }
    private sealed class PassiveHost : ISemanticsHostCapability
    {
        public event Action<SemanticsActionEvent>? Action { add { } remove { } }
        public void SetEnabled(bool enabled, DorotiUiInvocation invocation) { }
        public void Update(SemanticsUpdate update, DorotiUiInvocation invocation) { }
    }
    public class SemanticsProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? method, object?[]? args) => method?.Name switch
        {
            "createSemanticsUpdateBuilder" => new SemanticsUpdateBuilder(),
            _ => throw new NotSupportedException(method?.Name),
        };
    }
    private sealed class TestScheduler(PlatformDispatcher dispatcher) : SchedulerBinding(dispatcher)
    {
        public override void scheduleFrame() { }
    }
    private sealed class Provider : TickerProvider
    {
        public Ticker createTicker(Action<Duration> callback) => new(callback);
    }
    private sealed class Context : ScrollContext
    {
        public BuildContext? notificationContext => null;
        public BuildContext storageContext => null!;
        public TickerProvider vsync { get; } = new Provider();
        public AxisDirection axisDirection => AxisDirection.down;
        public double devicePixelRatio => 3;
        public void setIgnorePointer(bool value) { }
        public void setCanDrag(bool value) { }
        public void setSemanticsActions(HashSet<SemanticsAction> actions) { }
        public void saveOffset(double offset) { }
    }
    private sealed class Position(Context context)
        : ScrollPositionWithSingleContext(new ClampingScrollPhysics(), context, keepScrollOffset: false)
    {
        public override void didStartScroll() { }
        public override void didEndScroll() { }
        public override void didUpdateScrollPositionBy(double delta) { }
        public override void didUpdateScrollMetrics() { }
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

