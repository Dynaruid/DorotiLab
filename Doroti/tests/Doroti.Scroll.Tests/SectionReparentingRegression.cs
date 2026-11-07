using System.Reflection;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;

internal static class SectionReparentingRegression
{
    internal static void Run()
    {
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        var focus = new FocusManager();
        var owner = new BuildOwner(focusManager: focus);
        var binding = DispatchProxy.Create<WidgetsBinding, BindingProxy>();
        ((BindingProxy)binding).Owner = owner;
        var previousBinding = WidgetsBinding._instance;
        WidgetsBinding._instance = binding;
        var errors = new List<FlutterErrorDetails>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        var container = new RenderProxyBox();
        var pipeline = new PipelineOwner { rootNode = container };
        var firstIndex = new SectionExtentIndex(1);
        var secondIndex = new SectionExtentIndex(1);
        var itemKey = new GlobalKey<IState>();
        var stateKey = new GlobalKey<CounterState>();
        var firstOwns = false;
        CounterState.Created = CounterState.Disposed = 0;
        Widget Item() => new KeepAlive(key: itemKey, keepAlive: true, child: new Counter(stateKey));
        Widget View(bool first, bool suspended = false) => new Viewport(
            axisDirection: AxisDirection.down,
            offset: ViewportOffset.CreateFixed(0),
            slivers:
            [
                new SectionList(new SliverChildBuilderDelegate(
                    (_, _) => first == firstOwns ? Item() : null,
                    childCount: 1, addAutomaticKeepAlives: false,
                    addRepaintBoundaries: false, addSemanticIndexes: false),
                    first ? firstIndex : secondIndex, suspended: suspended),
            ]);
        Widget Tree(Widget first, Widget second) => new Directionality(
            textDirection: TextDirection.ltr,
            child: new Row(crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [new Expanded(child: first), new Expanded(child: second)]));
        var root = new RenderObjectToWidgetAdapter<RenderBox>(container: container,
            child: Tree(View(true), View(false))).attachToRenderTree(owner);
        typeof(RenderObject).GetProperty("_constraints", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(container, BoxConstraints.CreateTight(new Size(400, 400)));
        container.scheduleInitialLayout();
        void Flush()
        {
            pipeline.flushLayout();
            owner.finalizeTree();
            if (errors.Count != 0)
                throw new InvalidOperationException("Section transfer reported a framework error.", errors[0].exceptionThrown);
        }
        void Update(Widget? child)
        {
            new RenderObjectToWidgetAdapter<RenderBox>(container: container, child: child)
                .attachToRenderTree(owner, root);
            owner.buildScope(root);
            Flush();
        }
        try
        {
            Flush();
            var original = stateKey.currentState ?? throw new InvalidOperationException("The initial section did not mount.");
            var originalOwner = stateKey.currentContext!.findAncestorRenderObjectOfType<RenderSectionList>();
            original.Value = 42;
            var retainedRight = View(false, suspended: true);
            Update(Tree(View(true), retainedRight));
            firstOwns = true;
            // Scroll only the visible viewport. Neither the old owner nor
            // their shared ancestors are dirty when its section is requested.
            firstIndex.RequestItem(0);
            Flush();
            Check(stateKey.currentContext!.findAncestorRenderObjectOfType<RenderSectionList>() is { } nextOwner
                && !ReferenceEquals(nextOwner, originalOwner), "the section actually moves to another render owner");
            Check(ReferenceEquals(stateKey.currentState, original) && original.Value == 42,
                "the same GlobalKey state transfers from the suspended viewport");
            Check(CounterState.Created == 1 && CounterState.Disposed == 0,
                "transferring a retained section neither recreates nor disposes its state");
            Update(null);
            Check(CounterState.Disposed == 1, "final removal disposes the transferred state exactly once");
            pipeline.rootNode = null;
            root.deactivate();
            root.unmount();
            Check(owner.globalKeyCount == 0, "the fixture retires every GlobalKey registration");
        }
        finally
        {
            pipeline.rootNode = null;
            FlutterError.onError = previousError;
            WidgetsBinding._instance = previousBinding;
            pipeline.dispose();
            // This headless owner never registers platform keyboard handlers.
            focus.rootScope.dispose();
        }
        Console.WriteLine("PASS: GlobalKey section transfer from a suspended viewport preserves state and final teardown");
    }

    private sealed class Counter(GlobalKey<CounterState> key) : StatefulWidget(key: key)
    {
        public override IState createState() => new CounterState();
    }

    private sealed class CounterState : State<Counter>
    {
        internal static int Created, Disposed;
        internal int Value;
        public override void initState() { base.initState(); Created++; }
        public override Widget build(BuildContext context) => new SizedBox(width: 200, height: 100);
        public override void dispose() { Disposed++; base.dispose(); }
    }

    public class BindingProxy : DispatchProxy
    {
        internal BuildOwner Owner = null!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => method?.Name switch
        {
            "get_buildOwner" => Owner,
            _ => throw new NotSupportedException(method?.Name),
        };
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
