using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using PointerDownEvent = Doroti.Framework.Gestures.PointerDownEvent;
using PointerMoveEvent = Doroti.Framework.Gestures.PointerMoveEvent;
using PointerUpEvent = Doroti.Framework.Gestures.PointerUpEvent;
using PointerCancelEvent = Doroti.Framework.Gestures.PointerCancelEvent;

internal static partial class MountedPickerContracts
{
    internal static void VerifyDragAndTreeContracts()
    {
        static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
            "drag-tree", "drag-tree", "drag-tree");
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("drag-tree")
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
            var sourceKey = new GlobalKey<IState>();
            var targetKey = new GlobalKey<IState>();
            var wrongKey = new GlobalKey<IState>();
            var feedbackKey = new GlobalKey<IState>();
            var accepted = new List<int>();
            var ended = new List<bool>();
            var leaves = 0;
            var candidates = 0;
            var rejected = 0;
            var wrongCalls = 0;
            var allow = true;
            Widget Box(Key? key = null) => new SizedBox(key: key, width: 120, height: 120, child: new ColoredBox(color: new Color(0xff336699)));
            var entry = new OverlayEntry(builder: _ => new Column(children:
            [
                new Draggable<int>(data: 42, child: Box(sourceKey), feedback: Box(feedbackKey), onDragEnd: details => ended.Add(details.wasAccepted)),
                new DragTarget<int>(onWillAccept: _ => allow, onAccept: accepted.Add, onLeave: _ => leaves++,
                    builder: (_, incoming, declined) => { candidates = incoming.Count; rejected = declined.Count; return Box(targetKey); }),
                new DragTarget<string>(onWillAccept: _ => { wrongCalls++; return true; }, builder: (_, _, _) => Box(wrongKey))
            ]));
            Mount(new Overlay(initialEntries: [entry]));
            Offset Center(GlobalKey<IState> key)
            {
                var box = (RenderBox)key.currentContext!.findRenderObject()!;
                return box.localToGlobal(box.size.center(Offset.zero));
            }
            long pointer = 0;
            Offset last = Offset.zero;
            void Down()
            {
                pointer++;
                last = Center(sourceKey);
                Change(() => binding.handlePointerEvent(new PointerDownEvent(viewId: 1, pointer: pointer, position: last)));
                Move(last + new Offset(25, 0));
                Check(feedbackKey.currentContext is not null, "drag avatar inserts visible feedback");
            }
            void Move(Offset next)
            {
                Change(() => binding.handlePointerEvent(new PointerMoveEvent(viewId: 1, pointer: pointer, position: next, delta: next - last)));
                last = next;
            }
            void End(bool cancel = false)
            {
                Change(() => binding.handlePointerEvent(cancel
                    ? new PointerCancelEvent(viewId: 1, pointer: pointer, position: last)
                    : new PointerUpEvent(viewId: 1, pointer: pointer, position: last)));
                Check(feedbackKey.currentContext is null && candidates == 0 && rejected == 0, "ending drag removes feedback and target avatars");
            }
            Down(); Move(Center(targetKey));
            Check(candidates == 1 && rejected == 0, "integer target receives typed candidate");
            End();
            Check(accepted.SequenceEqual([42]) && ended.SequenceEqual([true]), "drop keeps integer payload and completion");
            Down(); Move(Center(wrongKey)); End();
            Check(wrongCalls == 0 && accepted.Count == 1 && !ended.Last(), "incompatible string target is excluded");
            allow = false;
            Down(); Move(Center(targetKey));
            Check(candidates == 0 && rejected == 1, "target veto exposes rejected payload");
            End();
            Check(!ended.Last() && leaves == 1, "rejected drag leaves target");
            allow = true;
            Down(); Move(Center(targetKey)); End(cancel: true);
            Check(!ended.Last() && accepted.Count == 1 && leaves == 2, "cancel leaves candidate without accepting");
            Change(entry.remove);
            entry.dispose();
            Mount(new SizedBox());
            Console.WriteLine("Mounted drag: integer payload, feedback lifecycle, candidate/drop, incompatible target, veto and cancellation PASS");

            var child = new TreeSliverNode<int>(2);
            var root = new TreeSliverNode<int>(1, children: [child]);
            var controller = new TreeSliverController();
            var toggled = new List<TreeSliverNode<int>>();
            TreeSliverController? found = null;
            Mount(new CustomScrollView(slivers: [new TreeSliver<int>(tree: [root], controller: controller,
                onNodeToggle: toggled.Add, treeNodeBuilder: (context, node, _) =>
                {
                    found = TreeSliverController.of(context);
                    return new SizedBox(height: 40, child: new Text(node.content.ToString()));
                })]));
            void Animate() { for (var i = 0; i < 35; i++) { host.Fire(); Thread.Sleep(10); } Pump(); }
            Check(ReferenceEquals(found, controller) && ReferenceEquals(controller.getNodeFor(2), child), "integer tree controller lookup and identity");
            Check(controller.isActive(root) && !controller.isActive(child), "collapsed child inactive");
            Change(() => controller.toggleNode(root)); Animate();
            Check(controller.isExpanded(root) && controller.isActive(child) && controller.getActiveIndexFor(child) == 1 && toggled.SequenceEqual([root]), "expansion exposes typed child and invokes callback");
            Change(controller.collapseAll); Animate();
            Check(!controller.isExpanded(root) && !controller.isActive(child), "collapse hides child");
            Change(controller.expandAll); Animate();
            Check(controller.isActive(child) && controller.getNodeFor("2") is null, "expand and reject mismatched content type");
            Mount(new SizedBox());
            Console.WriteLine("Mounted integer tree: controller ancestry, node identity/type, expand/collapse and callback PASS");

            foreach (var pinned in new[] { false, true })
            foreach (var floating in new[] { false, true })
            {
                var scroll = new ScrollController();
                var header = new ContractPersistentHeader();
                Mount(new CustomScrollView(controller: scroll, slivers: [new SliverPersistentHeader(@delegate: header, pinned: pinned, floating: floating), new SliverToBoxAdapter(child: new SizedBox(height: 1800))]));
                Check(header.Builds > 0, "header delegate builds child");
                Change(() => scroll.jumpTo(30));
                Check(header.LastShrink > 0, "header receives shrink offset after scroll");
                Mount(new SizedBox());
                scroll.dispose();
            }
            Console.WriteLine("Mounted persistent headers: regular/pinned/floating/combined build, scroll and detach PASS");
        }
        finally { FlutterError.onError = previousError; }
    }
    private sealed class ContractPersistentHeader : SliverPersistentHeaderDelegate
    {
        public int Builds;
        public double LastShrink;
        public override double minExtent => 40;
        public override double maxExtent => 100;
        public override Widget build(BuildContext context, double shrinkOffset, bool overlapsContent)
        { Builds++; LastShrink = shrinkOffset; return SizedBox.CreateExpand(child: new ColoredBox(color: new Color(0xff336699))); }
        public override bool shouldRebuild(SliverPersistentHeaderDelegate oldDelegate) => true;
    }
}
