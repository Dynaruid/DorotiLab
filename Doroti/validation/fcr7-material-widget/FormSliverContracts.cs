using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;

internal static partial class MountedPickerContracts
{
    internal static void VerifyFormAndSliverContracts()
    {
        static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
            "form-sliver", "form-sliver", "form-sliver");
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("form-sliver")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
        var errors = new List<FlutterErrorDetails>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        var binding = new WidgetsFlutterBinding(dispatcher);
        try
        {
            void Pump()
            {
                for (var i = 0; i < 8; i++) host.Fire();
                if (errors.Count > 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
            }
            void Mount(Widget child)
            {
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(
                    new MediaQuery(data: new MediaQueryData(size: new Size(Width, Height), supportsAnnounce: false),
                        child: new Directionality(textDirection: TextDirection.ltr, child: child)))));
                Pump();
            }
            void Change(System.Action action) { view.DispatchPlatformEvent(action); Pump(); }
            var formKey = new GlobalKey<FormState>();
            var textKey = new GlobalKey<FormFieldState<string>>();
            var numberKey = new GlobalKey<FormFieldState<long>>();
            var saved = new List<object?>();
            var text = new FormField<string>(key: textKey, initialValue: "initial", validator: value => string.IsNullOrEmpty(value) ? "required" : null,
                onSaved: value => saved.Add(value), builder: _ => new SizedBox(height: 40));
            var number = new FormField<long>(key: numberKey, initialValue: 7, validator: value => value < 0 ? "negative" : null,
                onSaved: value => saved.Add(value), builder: _ => new SizedBox(height: 40));
            Mount(new Form(key: formKey, child: new Column(children: [text, number])));
            var form = formKey.currentState ?? throw new Exception("Form did not mount");
            var textState = textKey.currentState ?? throw new Exception("String field did not mount");
            var numberState = numberKey.currentState ?? throw new Exception("Integer field did not mount");
            Check(form.fields.Count() == 2, "mounted fields register");
            Change(() => { textState.didChange(""); numberState.didChange(-1); });
            HashSet<object>? invalid = null;
            Change(() => invalid = form.validateGranularly());
            Check(invalid!.SetEquals([textKey.currentState!, numberKey.currentState!]), "granular errors identify heterogeneous mounted fields");
            Check(textState.errorText == "required" && numberState.errorText == "negative", "typed validators report their own errors");
            Change(form.clearError);
            Check(!textState.hasError && !numberState.hasError && !textState.hasInteractedByUser, "clear resets errors and interaction");
            Change(() => { textState.didChange("saved"); numberState.didChange(42); });
            Change(form.save);
            Check(saved.Count == 2 && saved.Contains("saved") && saved.Contains(42L), "mounted save retains each value type");
            Change(form.reset);
            Check(textState.value == "initial" && numberState.value == 7, "reset restores both initial values");
            Mount(new Form(key: formKey, child: new Column(children: [number])));
            Check(form.fields.Count() == 1 && textKey.currentState is null, "removed field unregisters and disposes");
            Console.WriteLine("Mounted forms: heterogeneous registration, granular validation/errors, clear, save, reset, removal PASS");

            var states = new Dictionary<long, IState>();
            var disposed = new List<long>();
            Widget Item(long id) => new SliverContractProbe(id, states, disposed);
            Widget List(params long[] order) => new CustomScrollView(slivers:
                [SliverList.CreateList(children: order.Select(Item).ToList(), addAutomaticKeepAlives: false, addRepaintBoundaries: false, addSemanticIndexes: false)]);
            Mount(List(1, 2, 3));
            var first = states[1]; var second = states[2]; var third = states[3];
            var sliver = Elements(binding.rootElement!).OfType<SliverMultiBoxAdaptorElement>().Single();
            Check(sliver.renderObject.childCount == 3, "sliver inserts all visible children");
            Mount(List(3, 1, 2));
            Check(ReferenceEquals(first, states[1]) && ReferenceEquals(second, states[2]) && ReferenceEquals(third, states[3]) && disposed.Count == 0, "keyed sliver reorder retains states");
            Check(Y(states[3]) < Y(states[1]) && Y(states[1]) < Y(states[2]), "render order follows moved keys");
            Mount(List(3, 2));
            Check(disposed.SequenceEqual([1L]) && sliver.renderObject.childCount == 2, "removed sliver child disposed exactly once");
            Mount(new CustomScrollView(slivers: [SliverPrototypeExtentList.CreateList(children: [Item(4), Item(5)], prototypeItem: new SizedBox(height: 70))]));
            Check(Y(states[5]) - Y(states[4]) == 70, "prototype child supplies fixed extent");
            Mount(new SizedBox());
            Check(disposed.Count == 5 && disposed.Distinct().Count() == 5, "remaining and prototype-list children detach exactly once");
            Console.WriteLine("Mounted slivers: insert, keyed reorder, physical order, remove, prototype extent, detach/dispose PASS");
        }
        finally { FlutterError.onError = previousError; }
    }

    private static double Y(IState state) => ((RenderBox)state.context.findRenderObject()!).localToGlobal(Offset.zero).dy;
    private sealed class SliverContractProbe(long id, Dictionary<long, IState> states, List<long> disposed)
        : StatefulWidget(key: new ValueKey<long>(id))
    {
        internal long Id => id;
        internal Dictionary<long, IState> States => states;
        internal List<long> Disposed => disposed;
        public override IState createState() => new ProbeState();
        private sealed class ProbeState : State<SliverContractProbe>
        {
            public override void initState() { base.initState(); widget.States[widget.Id] = this; }
            public override Widget build(BuildContext context) => new SizedBox(height: 90, child: new ColoredBox(color: new Color(0xff123456)));
            public override void dispose() { widget.Disposed.Add(widget.Id); base.dispose(); }
        }
    }
}
