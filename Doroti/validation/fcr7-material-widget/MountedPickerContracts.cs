using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using M = Doroti.Framework.Material;

internal static partial class MountedPickerContracts
{
    internal static void VerifyWidgetStartup()
    {
        foreach (var mode in new[] { "default", "immediate", "delayed", "detached", "shutdown", "failure" })
        {
            using var dispatcher = new PlatformDispatcher();
            using var scope = dispatcher.EnterScope();
            using var host = new Host();
            using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
                "startup-fixture", "startup-fixture", "startup-fixture");
            using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("startup-fixture")
                .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
                .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
                .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
                .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
                .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
                .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
                .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
                .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, new ClipboardFixtureHost())
                .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
            var ready = new TaskCompletionSource();
            var created = 0;
            var initialized = 0;
            var pointerDowns = 0;
            Widget Root()
            {
                if (!ReferenceEquals(PlatformDispatcher.instance, dispatcher))
                    throw new Exception("Root construction lost the owning dispatcher.");
                created++;
                return new Listener(onPointerDown: _ => pointerDowns++,
                    child: new ColoredBox(color: new Color(0xff1256ab), child: new SizedBox(width: 32, height: 32)));
            }
            var entrypoint = mode == "default"
                ? new Doroti.Framework.DorotiWidgetEntrypoint(Root)
                : new Doroti.Framework.DorotiWidgetEntrypoint(Root, () =>
                {
                    initialized++;
                    return mode == "immediate" ? Task.CompletedTask : ready.Task;
                });
            void Pointer(PointerChange change) => host.SendPointer(new PointerDataPacket([
                new PointerData(1, DorotiFrameClock.Now, change, PointerDeviceKind.mouse,
                    1, 10, 10, 0, 0, change == PointerChange.down ? 1 : 0, pointerIdentifier: 1)
            ]));
            var errors = new List<FlutterErrorDetails>();
            var previousError = FlutterError.onError;
            FlutterError.onError = errors.Add;
            try
            {
                entrypoint.Bootstrap(dispatcher);
                entrypoint.AttachView(view);
                if (!host.HasPendingFrame) throw new Exception("Entrypoint did not schedule its own bootstrap.");
                host.Fire();
                if (mode is not ("default" or "immediate"))
                {
                    if (created != 0 || initialized != 1) throw new Exception("Root did not await initialization exactly once.");
                    if (mode == "detached") entrypoint.DetachView(view);
                    if (mode == "shutdown") entrypoint.Shutdown();
                    if (mode == "failure") ready.SetException(new InvalidOperationException("startup fixture failure"));
                    else ready.SetResult();
                    if (created != 0) throw new Exception("Async completion attached outside the host event loop.");
                }
                // Resource completion can drain after any native event, before
                // the requested frame. Mouse discovery must be safe in this gap.
                view.DispatchPlatformEvent(() => { });
                Pointer(PointerChange.add);
                Pointer(PointerChange.hover);
                // Deliver only frames actually requested by production code.
                for (var frame = 0; frame < 8 && host.HasPendingFrame; frame++) host.Fire();
                var shouldAttach = mode is "default" or "immediate" or "delayed";
                if (shouldAttach)
                {
                    Pointer(PointerChange.down);
                    Pointer(PointerChange.up);
                    if (pointerDowns != 1) throw new Exception($"Widget startup {mode} lost input after layout.");
                    using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
                    var completion = renderer.Paint(surface, Width, Height)
                        ?? throw new Exception($"Widget startup {mode} did not produce a paintable scene.");
                    renderer.CompletePaint(completion, DorotiFrameTerminal.submitted);
                }
                if (created != (shouldAttach ? 1 : 0) || (shouldAttach && renderer.Diagnostics.Submitted < 1))
                    throw new Exception($"Widget startup {mode}: roots={created}, scenes={renderer.Diagnostics.Submitted}, errors={string.Join("; ", errors.Select(e => e.exceptionThrown))}");
                if (errors.Count != (mode == "failure" ? 1 : 0))
                    throw new Exception($"Widget startup {mode} produced unexpected framework errors: {string.Join("; ", errors.Select(e => e.exceptionThrown))}");
                Console.WriteLine($"Widget startup {mode}: PASS");
            }
            finally
            {
                entrypoint.Shutdown();
                FlutterError.onError = previousError;
            }
        }
    }

    private static bool AppBarRaster => Environment.GetEnvironmentVariable("DOROTI_VALIDATION_APPBAR_RASTER") == "1";
    private static bool SamplePopups => Environment.GetEnvironmentVariable("DOROTI_VALIDATION_SAMPLE_POPUPS") == "1";
    private static int Width => SamplePopups ? int.Parse(Environment.GetEnvironmentVariable("DOROTI_VALIDATION_POPUP_WIDTH")!) : AppBarRaster ? 1275 : 800;
    private static int Height => SamplePopups ? int.Parse(Environment.GetEnvironmentVariable("DOROTI_VALIDATION_POPUP_HEIGHT")!) : AppBarRaster ? 640 : 900;
    internal static void Verify()
    {
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var gpu = Environment.GetEnvironmentVariable("DOROTI_VALIDATION_GPU_RASTER") == "1" ? new GpuRasterFixture() : null;
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null, "picker-fixture", "picker-fixture", "picker-fixture");
        var images = new CountingImages(renderer);
        var errors = new List<FlutterErrorDetails>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        try
        {
            using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("picker-fixture")
                .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
                .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
                .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
                .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
                .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
                .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, images)
                .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
                .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
                .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, new ClipboardFixtureHost())
                .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
            var binding = new WidgetsFlutterBinding(dispatcher);
            renderer.RegisterFontAsync(File.ReadAllBytes("DorotiTestbedApp/assets/fonts/MaterialIcons-Regular.otf"), "MaterialIcons").GetAwaiter().GetResult();
            if (Environment.GetEnvironmentVariable("DOROTI_VALIDATION_SECTION_VIEWPORT") == "1")
            {
                foreach (var count in new[] { 29, 290 })
                {
                    var measuredItems = new HashSet<int>();
                    var disposedItems = new List<int>();
                    var retention = new Dictionary<int, ValueNotifier<bool>>();
                    var index = new Doroti.Framework.Rendering.SectionExtentIndex(count, 300);
                    var scroll = new ScrollController();
                    var children = new SliverChildBuilderDelegate((_, i) =>
                    {
                        measuredItems.Add((int)i);
                        var pin = new ValueNotifier<bool>(true); retention[(int)i] = pin;
                        return new ValueListenableBuilder<bool>(valueListenable: pin,
                            builder: (_, keep, child) => new KeepAlive(keepAlive: keep, child: child!),
                            child: new SectionLifetimeProbe(new LayoutBuilder(builder: (_, constraints) =>
                                new SizedBox(height: 80 + (i % 5) * 10 + constraints.maxWidth / 100,
                                    child: new ColoredBox(color: new Color(0xff123456)))), () => disposedItems.Add((int)i)));
                    }, childCount: count, addAutomaticKeepAlives: false, addRepaintBoundaries: false, addSemanticIndexes: false);
                    view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new Directionality(
                        textDirection: TextDirection.ltr, child: new CustomScrollView(controller: scroll, cacheExtent: 0,
                            slivers: [new SectionList(children, index)])))));
                    void Frames() { for (var i = 0; i < 5; i++) host.Fire(); }
                    Frames();
                    var initial = measuredItems.Count;
                    if (initial > 10) throw new Exception("Initial viewport eagerly materialized offscreen sections");
                    view.DispatchPlatformEvent(() => scroll.jumpTo(scroll.position.maxScrollExtent));
                    Frames();
                    if (!measuredItems.Contains(count - 1) || measuredItems.Count > initial + 15)
                        throw new Exception("End failed to materialize last section without synchronous intermediate construction");
                    view.DispatchPlatformEvent(() => SectionList.RequestItem(scroll, index, count / 2, 7));
                    Frames();
                    if (!measuredItems.Contains(count / 2) || index.AnchorIndex != count / 2 || Math.Abs(index.AnchorOffset - 7) > 1)
                        throw new Exception($"Indexed materialization lost anchor: {index.AnchorIndex}/{index.AnchorOffset}");
                    foreach (var width in new[] { 390, 1280, 390, 1280 })
                    {
                        view.DispatchPlatformEvent(() => host.Resize(width)); Frames();
                        if (index.AnchorIndex != count / 2 || Math.Abs(index.AnchorOffset - 7) > 1)
                            throw new Exception($"Width cache restored pixel offset instead of section anchor at {width}: {index.AnchorIndex}/{index.AnchorOffset}");
                    }
                    Console.WriteLine($"SECTION_VIEWPORT count={count} initial={initial} visited={measuredItems.Count} anchor={index.AnchorIndex}/{index.AnchorOffset}");
                    view.DispatchPlatformEvent(() => retention[0].value = false); Frames();
                    if (disposedItems.Count(i => i == 0) != 1)
                        throw new Exception("Unpinning an offscreen section did not dispose it in the next layout");
                    view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new SizedBox())));
                    Frames(); scroll.dispose();
                    if (disposedItems.Count != measuredItems.Count) throw new Exception("Section subtree disposal is unbalanced");
                    foreach (var pin in retention.Values) pin.dispose();
                }
                var focusIndex = new Doroti.Framework.Rendering.SectionExtentIndex(3, 1000);
                var focusScroll = new ScrollController();
                var focusNodes = Enumerable.Range(0, 6).Select(i => new FocusNode(debugLabel: $"section-test-{i}")).ToArray();
                var focusCreated = new HashSet<int>();
                using var coordinator = new SectionFocusCoordinator(Enumerable.Range(0, 3), id => SectionList.RequestItem(focusScroll, focusIndex, id));
                var focusChildren = new SliverChildBuilderDelegate((_, i) =>
                {
                    focusCreated.Add((int)i);
                    return new KeepAlive(keepAlive: true, child: coordinator.Wrap((int)i, new SizedBox(height: 1000,
                        child: new Column(children: [
                            new Focus(focusNode: focusNodes[i * 2], child: new SizedBox(height: 20, width: 30)),
                            new Focus(focusNode: focusNodes[i * 2 + 1], child: new SizedBox(height: 20, width: 30))]))));
                }, childCount: 3, addAutomaticKeepAlives: false, addRepaintBoundaries: false, addSemanticIndexes: false);
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new Directionality(textDirection: TextDirection.ltr,
                    child: new FocusTraversalGroup(child: new CustomScrollView(controller: focusScroll, cacheExtent: 0,
                        slivers: [new SectionList(focusChildren, focusIndex)]))))));
                void FocusFrames() { for (var i = 0; i < 10; i++) { host.Fire(); Thread.Sleep(1); } }
                FocusFrames();
                if (focusCreated.Contains(1)) throw new Exception("Focus fixture eagerly built its next section");
                view.DispatchPlatformEvent(() => focusNodes[1].requestFocus()); FocusFrames();
                void TwoTabs(bool reverse) => view.DispatchPlatformEvent(() =>
                {
                    if (reverse) host.SendKey(new KeyData(1, TimeSpan.Zero, KeyEventType.down, 0x700e1, Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft.keyId, false));
                    for (var n = 0; n < 2; n++)
                    {
                        host.SendKey(new KeyData(1, TimeSpan.Zero, KeyEventType.down, 0x7002b, Doroti.Framework.Services.LogicalKeyboardKey.tab.keyId, false));
                        host.SendKey(new KeyData(1, TimeSpan.Zero, KeyEventType.up, 0x7002b, Doroti.Framework.Services.LogicalKeyboardKey.tab.keyId, false));
                    }
                    if (reverse) host.SendKey(new KeyData(1, TimeSpan.Zero, KeyEventType.up, 0x700e1, Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft.keyId, false));
                });
                TwoTabs(false); FocusFrames();
                if (!focusCreated.Contains(1) || !ReferenceEquals(FocusManager.instance.primaryFocus, focusNodes[3]))
                    throw new Exception("Tab did not materialize the next section and retain the queued second Tab");
                view.DispatchPlatformEvent(() => focusNodes[2].requestFocus()); FocusFrames();
                TwoTabs(true); FocusFrames();
                if (!ReferenceEquals(FocusManager.instance.primaryFocus, focusNodes[0])) throw new Exception("Reverse section traversal lost a queued Shift+Tab");
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new SizedBox())));
                FocusFrames(); focusScroll.dispose();
                foreach (var node in focusNodes) node.dispose();
                Console.WriteLine("SECTION_FOCUS forward/reverse materialization and queued Tab preservation PASS");
                var dynamicOrder = Enumerable.Range(0, 10).ToArray();
                var dynamicKeys = Enumerable.Range(0, 11).Select(_ => new GlobalKey<IState>()).ToArray();
                var dynamicDisposed = new List<int>();
                var dynamicScroll = new ScrollController();
                var dynamicIndex = new Doroti.Framework.Rendering.SectionExtentIndex(10, 1000);
                Widget DynamicList(int[] retained)
                {
                    var order = dynamicOrder;
                    var adapter = new SliverChildBuilderDelegate((_, item) =>
                    {
                        var id = order[item];
                        return new KeepAlive(key: new ValueKey<int>(id), keepAlive: true,
                            child: new StatefulBuilder(key: dynamicKeys[id], builder: (_, _) =>
                                new SectionLifetimeProbe(new SizedBox(height: 1000), () => dynamicDisposed.Add(id))));
                    }, childCount: order.Length,
                        findChildIndexCallback: key => key is ValueKey<int> value && Array.IndexOf(order, value.value) is var position && position >= 0 ? position : null,
                        addAutomaticKeepAlives: false, addRepaintBoundaries: false, addSemanticIndexes: false);
                    return binding.wrapWithDefaultView(new Directionality(textDirection: TextDirection.ltr,
                        child: new CustomScrollView(controller: dynamicScroll, cacheExtent: 0,
                            slivers: [new SectionList(adapter, dynamicIndex, retainIndices: retained)])));
                }
                view.DispatchPlatformEvent(() => binding.attachRootWidget(DynamicList([]))); FocusFrames();
                view.DispatchPlatformEvent(() => SectionList.RequestItem(dynamicScroll, dynamicIndex, 3, 7)); FocusFrames();
                var dynamicState = dynamicKeys[3].currentState ?? throw new Exception("Dynamic fixture did not mount its anchor");
                dynamicOrder = [3, 1, 2, 4, 5, 6, 7, 8, 9, 10];
                dynamicIndex = new Doroti.Framework.Rendering.SectionExtentIndex(dynamicOrder.Length, 1000);
                view.DispatchPlatformEvent(() =>
                {
                    // Map surviving id 3 and cancel scroll activity owned by the old configuration.
                    SectionList.RequestItem(dynamicScroll, dynamicIndex, 0, 7);
                    binding.attachRootWidget(DynamicList([0]));
                }); FocusFrames();
                if (!ReferenceEquals(dynamicKeys[3].currentState, dynamicState) || dynamicIndex.AnchorIndex != 0 || Math.Abs(dynamicIndex.AnchorOffset - 7) > 1)
                    throw new Exception($"Reordering a surviving section lost State or its mapped anchor: state={ReferenceEquals(dynamicKeys[3].currentState, dynamicState)}, anchor={dynamicIndex.AnchorIndex}/{dynamicIndex.AnchorOffset}, pixels={dynamicScroll.position.pixels}, total={dynamicIndex.Total}, max={dynamicScroll.position.maxScrollExtent}, disposed={string.Join(',', dynamicDisposed)}, errors={string.Join('\n', errors.Select(e => e.exceptionThrown))}");
                if (dynamicKeys[0].currentState is not null || dynamicDisposed.Count(id => id == 0) != 1 || dynamicKeys[10].currentState is not null)
                    throw new Exception("Removing/adding a section violated disposal or lazy construction");
                view.DispatchPlatformEvent(() => SectionList.RequestItem(dynamicScroll, dynamicIndex, 9)); FocusFrames();
                if (dynamicKeys[10].currentState is null) throw new Exception("New dynamic section could not materialize");
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new SizedBox()))); FocusFrames();
                dynamicScroll.dispose();
                if (dynamicDisposed.GroupBy(id => id).Any(group => group.Count() != 1)) throw new Exception("Dynamic section disposed more than once");
                Console.WriteLine("SECTION_DYNAMIC owner-mapped reorder/add/remove, surviving State and anchor, removed disposal PASS");
                if (errors.Count != 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
                return;
            }
            if (Environment.GetEnvironmentVariable("DOROTI_VALIDATION_SAMPLE_COLUMNS") == "1")
            {
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(
                    locale: new Locale("en", "US"), home: new MaterialSample.SampleHome(0, 0, false, false, null, () => { }, _ => { }, _ => { })))));
                var retainedSections = new Dictionary<GlobalKey<IState>, IState>();
                foreach (var width in new[] { 800, 1280, 800, 1001, 1000, 390, 1501, 800 })
                {
                    (int Item, double Offset)? expectedAnchor = null;
                    if (Environment.GetEnvironmentVariable("DOROTI_VALIDATION_SECTION_COLUMNS_DEEP") == "1")
                    {
                        var mountedGallery = Elements(binding.rootElement!).OfType<StatefulElement>().FirstOrDefault(e => e.widget is MaterialSample.ComponentsScreen);
                        if (mountedGallery is not null)
                        {
                            var stateType = mountedGallery.state.GetType();
                            var fields = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                            var activeIndex = (Doroti.Framework.Rendering.SectionExtentIndex?)stateType.GetField("_activeFirstIndex", fields)!.GetValue(mountedGallery.state);
                            var firstScroll = (ScrollController)stateType.GetField("_firstScroll", fields)!.GetValue(mountedGallery.state)!;
                            if (activeIndex is not null)
                            {
                                view.DispatchPlatformEvent(() => SectionList.RequestItem(firstScroll, activeIndex, activeIndex.Count / 2, 7));
                                for (var f = 0; f < 5; f++) host.Fire();
                                expectedAnchor = (activeIndex.Count / 2, 7);
                            }
                        }
                    }
                    view.DispatchPlatformEvent(() => host.Resize(width));
                    long? previousSectionBuilds = null;
                    for (var frame = 0; frame < 130; frame++)
                    {
                        host.Fire(); Thread.Sleep(10);
                        var element = (StatefulElement)Elements(binding.rootElement!).Single(e => e.widget is MaterialSample.ComponentsScreen);
                        var current = (MaterialSample.ComponentsScreen)element.widget;
                        var right = (ScrollController)element.state.GetType().GetField("_secondScroll", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(element.state)!;
                        var rightRetained = (bool)element.state.GetType().GetField("_secondVisited", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(element.state)!;
                        if (current.TwoColumns != (width > 1000) || right.hasClients != (current.TwoColumns || rightRetained))
                            throw new Exception($"Columns disagree with viewport during transition: width={width}, frame={frame}, right={right.hasClients}");
                        var galleryState = (MaterialSample.ComponentsState)element.state;
                        var rebuilds = galleryState.SectionBuildCount - galleryState.SectionFirstBuildCount;
                        if (frame > 2 && previousSectionBuilds != rebuilds)
                            throw new Exception($"Navigation tick rebuilt a section: width={width}, frame={frame}");
                        previousSectionBuilds = rebuilds;
                        var keys = (List<GlobalKey<IState>>)galleryState.GetType().GetField("_sectionKeys", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(galleryState)!;
                        foreach (var key in keys)
                        {
                            if (retainedSections.TryGetValue(key, out var state) && !ReferenceEquals(key.currentState, state))
                                throw new Exception($"Visited section State lost during column transition: width={width}, frame={frame}, section={keys.IndexOf(key)}");
                            if (key.currentState is { } mountedState) retainedSections[key] = mountedState;
                        }
                    }
                    if (errors.Count != 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
                    var screen = (MaterialSample.ComponentsScreen)Elements(binding.rootElement!).Single(e => e.widget is MaterialSample.ComponentsScreen).widget;
                    var home = ((StatefulElement)Elements(binding.rootElement!).Single(e => e.widget is MaterialSample.SampleHome)).state;
                    var controller = (Doroti.Framework.Animation.AnimationController)home.GetType().GetField("_controller", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(home)!;
                    Console.WriteLine($"COLUMNS width={width} two={screen.TwoColumns} controller={controller.value} status={controller.status}");
                    if (screen.TwoColumns != (width > 1000) || controller.value != (width > 1000 ? 1 : 0))
                        throw new Exception("Sample columns did not settle at width " + width);
                    if (expectedAnchor is { } expected)
                    {
                        var state = ((StatefulElement)Elements(binding.rootElement!).Single(e => e.widget is MaterialSample.ComponentsScreen)).state;
                        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                        var split = (int)state.GetType().GetField("_split", flags)!.GetValue(state)!;
                        var indices = (Dictionary<(int First, int Count), Doroti.Framework.Rendering.SectionExtentIndex>)state.GetType().GetField("_indices", flags)!.GetValue(state)!;
                        var sectionAnchorEntry = indices.Single(pair => pair.Key == (screen.TwoColumns && expected.Item >= split ? (split, 29 - split) : (0, screen.TwoColumns ? split : 29)));
                        if (sectionAnchorEntry.Value.AnchorIndex + sectionAnchorEntry.Key.First != expected.Item || Math.Abs(sectionAnchorEntry.Value.AnchorOffset - expected.Offset) > 1)
                            throw new Exception($"Column anchor lost at width {width}: expected {expected}, actual {sectionAnchorEntry.Value.AnchorIndex + sectionAnchorEntry.Key.First}/{sectionAnchorEntry.Value.AnchorOffset}");
                    }
                }
                return;
            }
            if (Environment.GetEnvironmentVariable("DOROTI_VALIDATION_MOUNTED_TEXT") == "1")
            {
                var controller = new TextEditingController();
                var focus = new FocusNode();
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(
                    locale: new Locale("en", "US"), home: new M.Scaffold(body: new Center(child: new SizedBox(width: 240, child: new M.TextField(
                        controller: controller, focusNode: focus))))))));
                Pump("text-initial");
                var editable = (EditableTextState)((StatefulElement)Elements(binding.rootElement!).Single(e => e.widget is EditableText)).state;
                view.DispatchPlatformEvent(() => { focus.requestFocus(); controller.text = "Hello keyboard"; });
                Pump("text-focused");
                Console.WriteLine($"focus={focus.hasFocus}, primary={FocusManager.instance.primaryFocus}, selection={controller.selection}");
                void Send(long physical, long logical, KeyEventType type) => view.DispatchPlatformEvent(() => host.SendKey(new KeyData(1, TimeSpan.Zero, type, physical, logical, false)));
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.down);
                Send(0x70004, 97, KeyEventType.down);
                Send(0x70004, 97, KeyEventType.up);
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.up);
                Pump("text-select-all");
                if (controller.selection.start != 0 || controller.selection.end != controller.text.Length)
                    throw new Exception($"Mounted Ctrl+A failed: {controller.selection}");
                view.DispatchPlatformEvent(() => controller.text = string.Concat(Enumerable.Repeat("Long text input ", 20)));
                Pump("text-long");
                Console.WriteLine($"editable size={editable.renderEditable.size}, maxScroll={editable.renderEditable.maxScrollExtent}, offset={editable.renderEditable.offset.pixels}, clip={editable.renderEditable.clipBehavior}, compositing={editable.renderEditable.needsCompositing}");
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.down);
                foreach (var letter in "ACXV") {
                    Send(0x70004 + letter - 'A', char.ToLowerInvariant(letter), KeyEventType.down);
                    Send(0x70004 + letter - 'A', char.ToLowerInvariant(letter), KeyEventType.up);
                    Pump("text-shortcut-" + letter);
                    if (controller.text.Length != (letter == 'X' ? 0 : 320)) throw new Exception("Clipboard shortcut " + letter);
                    Console.WriteLine($"{letter}: length={controller.text.Length}, selection={controller.selection}");
                }
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.up);
                view.DispatchPlatformEvent(() => controller.text = "one two three");
                Pump("text-navigation-initial");
                view.DispatchPlatformEvent(() => controller.selection = Doroti.Framework.Services.TextSelection.CreateCollapsed(controller.text.Length));
                Send(0x700e1, Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft.keyId, KeyEventType.down);
                Send(0x70050, Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft.keyId, KeyEventType.down);
                Send(0x70050, Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft.keyId, KeyEventType.up);
                Send(0x700e1, Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft.keyId, KeyEventType.up);
                Pump("text-shift-left");
                if(controller.selection.baseOffset != 13 || controller.selection.extentOffset != 12) throw new Exception("Shift+Left: " + controller.selection);
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.down);
                Send(0x7004a, Doroti.Framework.Services.LogicalKeyboardKey.home.keyId, KeyEventType.down);
                Send(0x7004a, Doroti.Framework.Services.LogicalKeyboardKey.home.keyId, KeyEventType.up);
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.up);
                Pump("text-ctrl-home");
                if(controller.selection.extentOffset != 0) throw new Exception("Ctrl+Home: " + controller.selection);
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.down);
                Send(0x7004f, Doroti.Framework.Services.LogicalKeyboardKey.arrowRight.keyId, KeyEventType.down);
                Send(0x7004f, Doroti.Framework.Services.LogicalKeyboardKey.arrowRight.keyId, KeyEventType.up);
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.up);
                Pump("text-ctrl-right");
                if(controller.selection.extentOffset != 3) throw new Exception("Ctrl+Right: " + controller.selection);
                var beforeDelete = controller.text.Length;
                Send(0x7002a, Doroti.Framework.Services.LogicalKeyboardKey.backspace.keyId, KeyEventType.down);
                Send(0x7002a, Doroti.Framework.Services.LogicalKeyboardKey.backspace.keyId, KeyEventType.up);
                Pump("text-backspace");
                if(controller.text.Length != beforeDelete - 1) throw new Exception("Backspace must delete once");
                view.DispatchPlatformEvent(() => {
                    controller.text = "one two three";
                    controller.selection = Doroti.Framework.Services.TextSelection.CreateCollapsed(controller.text.Length);
                });
                Pump("text-word-delete-initial");
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.down);
                foreach (var expected in new[] { "one two ", "one ", "", "" }) {
                    Send(0x7002a, Doroti.Framework.Services.LogicalKeyboardKey.backspace.keyId, KeyEventType.down);
                    Send(0x7002a, Doroti.Framework.Services.LogicalKeyboardKey.backspace.keyId, KeyEventType.up);
                    Pump("text-word-delete-" + expected.Length);
                    if(controller.text != expected || controller.selection.extentOffset != expected.Length)
                        throw new Exception($"Ctrl+Backspace must delete one word: text={controller.text}, selection={controller.selection}");
                }
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.up);
                // Repeat navigation past the first word: a single Ctrl+Right does not
                // exercise the whitespace boundary where the caret used to get stuck.
                foreach (var (text, right, left) in new (string, int[], int[])[] {
                    ("abc def", [3, 7, 7], [4, 0, 0]),
                    ("abc   def", [3, 9, 9], [6, 0, 0]),
                    ("가나다\u00a0라마바", [3, 7, 7], [4, 0, 0]),
                    (" abc def ", [4, 8, 9, 9], [5, 1, 0, 0]),
                    ("   ", [3, 3], [0, 0]),
                    ("", [0, 0], [0, 0]),
                }) {
                    foreach (var forward in new[] { true, false }) {
                        foreach (var extend in new[] { false, true }) {
                            var anchor = forward ? 0 : text.Length;
                            view.DispatchPlatformEvent(() => {
                                controller.text = text;
                                controller.selection = Doroti.Framework.Services.TextSelection.CreateCollapsed(anchor);
                            });
                            Pump("text-word-navigation-initial", frames: 2);
                            Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.down);
                            if (extend) Send(0x700e1, Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft.keyId, KeyEventType.down);
                            var physical = forward ? 0x7004f : 0x70050;
                            var logical = forward ? Doroti.Framework.Services.LogicalKeyboardKey.arrowRight : Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft;
                            foreach (var expected in forward ? right : left) {
                                Send(physical, logical.keyId, KeyEventType.down);
                                Send(physical, logical.keyId, KeyEventType.up);
                                Pump("text-word-navigation", frames: 2);
                                if (controller.text != text || controller.selection.extentOffset != expected ||
                                    controller.selection.baseOffset != (extend ? anchor : expected))
                                    throw new Exception($"Repeated Ctrl+{(extend ? "Shift+" : "")}{(forward ? "Right" : "Left")} in '{text}': expected {expected}, got {controller.selection}");
                            }
                            if (extend) Send(0x700e1, Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft.keyId, KeyEventType.up);
                            Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.up);
                        }
                    }
                }
                Console.WriteLine("Mounted repeated word navigation and selection: PASS");
                var longInput = string.Concat(Enumerable.Repeat("Long native input ", 20));
                view.DispatchPlatformEvent(() => host.Edit(new DorotiTextEditingState(longInput, new(longInput.Length, longInput.Length), null)));
                Pump("text-native-long");
                var caret = Rect.zero;
                view.DispatchPlatformEvent(() => caret = editable.renderEditable.getLocalRectForCaret(new TextPosition(longInput.Length)));
                if(editable.renderEditable.offset.pixels <= 0 || caret.left < -1 || caret.right > editable.renderEditable.size.width + 1)
                    throw new Exception($"Long input caret not visible: {caret}, scroll={editable.renderEditable.offset.pixels}");
                // Compare native raster output around the actual editable viewport.
                var output = System.IO.Path.Combine(WindowsSampleContracts.OutputDirectory, "mounted");
                var viewport = editable.renderEditable.localToGlobal(Offset.zero) & editable.renderEditable.size;
                using var empty = SKBitmap.Decode(System.IO.Path.Combine(output, "text-initial.png"));
                foreach (var stage in new[] { "text-long", "text-shortcut-A", "text-native-long" }) {
                    using var actual = SKBitmap.Decode(System.IO.Path.Combine(output, stage + ".png"));
                    var insideChanges = 0;
                    for(var y = (int)Math.Ceiling(viewport.top); y < (int)viewport.bottom; y++)
                    for(var x = 0; x < Width; x++) {
                        if(actual.GetPixel(x,y) == empty.GetPixel(x,y)) continue;
                        if(x < (int)viewport.left || x >= (int)Math.Ceiling(viewport.right))
                            throw new Exception($"{stage}: editable pixels escaped viewport at {x},{y}");
                        insideChanges++;
                    }
                    if(insideChanges < 50) throw new Exception(stage + ": text/selection must paint visible pixels");
                }
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.down);
                Send(0x7001d, Doroti.Framework.Services.LogicalKeyboardKey.keyZ.keyId, KeyEventType.down);
                Send(0x7001d, Doroti.Framework.Services.LogicalKeyboardKey.keyZ.keyId, KeyEventType.up);
                Pump("text-undo");
                if(controller.text == longInput) throw new Exception("Ctrl+Z did not undo native input");
                Send(0x700e1, Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft.keyId, KeyEventType.down);
                Send(0x7001d, Doroti.Framework.Services.LogicalKeyboardKey.keyZ.keyId, KeyEventType.down);
                Send(0x7001d, Doroti.Framework.Services.LogicalKeyboardKey.keyZ.keyId, KeyEventType.up);
                Send(0x700e1, Doroti.Framework.Services.LogicalKeyboardKey.shiftLeft.keyId, KeyEventType.up);
                Send(0x700e0, Doroti.Framework.Services.LogicalKeyboardKey.controlLeft.keyId, KeyEventType.up);
                Pump("text-redo");
                if(controller.text != longInput) throw new Exception("Ctrl+Shift+Z did not redo native input");
                Console.WriteLine("mounted text: Ctrl+A/C/X/V/Z, Ctrl+Shift+Z, Shift+Left, Ctrl+Home/Right, Backspace, repeated Ctrl+Backspace, native long input caret/scroll and raster clipping PASS");
                return;
            }
            if (SamplePopups)
            {
                var selectedSeed = -1;
                var selectedImage = -1;
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(
                    locale: new Locale("en", "US"), home: new MaterialSample.SampleHome(0, 0, false, false, null,
                        () => { }, value => selectedSeed = value, value => selectedImage = value)))));
                Pump("popups-initial");
                foreach (var (tooltip, label, expected) in new[] {
                    ("Select a seed color", "Blue", 2),
                    ("Select a color extraction image", "Peonies", 1) })
                {
                    // The collapsed rail stays mounted on narrow screens.
                    var actions = Elements(binding.rootElement!).First(element => Width > 1000
                        ? element.widget is M.NavigationRail : element.widget is M.AppBar);
                    var button = Elements(actions).Single(element => element.widget is M.PopupMenuButton<int> popup && popup.tooltip == tooltip);
                    TapElement(button);
                    Pump("popup-open-" + expected);
                    var entry = Elements(binding.rootElement!).SingleOrDefault(element => element.widget is M.PopupMenuItem<int> item && item.value == expected);
                    if (entry is null) throw new Exception($"{tooltip}: pointer tap did not open menu at width {Width}");
                    var rect = Bounds(entry);
                    if (rect.width <= 0 || rect.height <= 0 || rect.left < 0 || rect.top < 0 || rect.right > Width || rect.bottom > Height)
                        throw new Exception($"{label}: menu entry is outside viewport: {rect}");
                    TapElement(entry);
                    Pump("popup-selected-" + expected);
                    if ((expected == 2 ? selectedSeed : selectedImage) != expected)
                        throw new Exception($"{label}: menu selection was not delivered");
                }
                Console.WriteLine($"sample popups: seed/image pointer open, visible bounds and selection at {Width}x{Height} PASS");
                return;
            }
            if (AppBarRaster)
            {
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(
                    locale: new Locale("en", "US"), theme: M.ThemeData.Create(brightness: Brightness.dark),
                    home: new MaterialSample.SampleHome(0, 0, false, false, null, () => { }, _ => { }, _ => { })))));
                Pump("raster-initial");
                var rasterComponents = ((StatefulElement)Elements(binding.rootElement!).Single(element => element.widget is MaterialSample.ComponentsScreen)).state;
                ScrollController Controller(string name) => (ScrollController)rasterComponents.GetType().GetField(name,
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(rasterComponents)!;
                var left = Controller("_firstScroll");
                var right = Controller("_secondScroll");
                view.DispatchPlatformEvent(() => left.jumpTo(220));
                for (var step = 0; step < 70; step++)
                {
                    view.DispatchPlatformEvent(() => {
                        right.jumpTo(step < 35 ? step * 100 : (69 - step) * 100);
                        left.jumpTo(300 + step * 4);
                    });
                    for (var tick = 0; tick < 6; tick++) Pump($"raster-scroll-{step}-{tick}", 1);
                }
                if (renderer.Diagnostics.PictureRasterCacheHits == 0 || renderer.Diagnostics.PictureRasterCacheMisses <= 24)
                    throw new Exception("App bar raster fixture did not exercise cache pressure and reuse");
                Console.WriteLine("two-column app bars: CPU/GPU raster parity through inline bars, cache pressure and reuse PASS");
                return;
            }
            VerifyTabs();
            if (Environment.GetEnvironmentVariable("DOROTI_VALIDATION_TABS") == "1") return;

            void VerifyTabs()
            {
                M.TabController? controller = null;
                long tapped = -1, hovered = -1, focused = -1;
                var labels = new[] { "Video", "Photos", "Audio" };
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(
                    locale: new Locale("en", "US"), home: new M.DefaultTabController(length: 3,
                        child: new Builder(builder: context =>
                        {
                            controller = M.DefaultTabController.of(context);
                            return new Center(child: new SizedBox(height: 80, child: new M.Scaffold(appBar: new M.AppBar(bottom:
                                new M.TabBar(tabs: labels.Select(label => (Widget)new M.Tab(text: label,
                                    icon: new Icon(M.Icons.photo_outlined), iconMargin: Doroti.Framework.Painting.EdgeInsets.zero)).ToList(),
                                    onTap: index => tapped = index, onHover: (_, index) => hovered = index,
                                    onFocusChange: (_, index) => focused = index)))));
                        }))))));
                Pump("tabs-initial");
                foreach (var expected in new[] { 1, 2, 0, 2, 1, 1, 0 })
                {
                    var tab = Elements(binding.rootElement!).Single(element => element.widget is M.Tab value && value.text == labels[expected]);
                    TapElement(tab);
                    Pump("tabs-selected-" + expected);
                    if (tapped != expected || controller!.index != expected || controller.indexIsChanging || controller.animation!.value != expected)
                        throw new Exception($"Tab {expected}: callback={tapped}, selection={controller!.index}, animation={controller.animation!.value}");
                    var bar = Elements(binding.rootElement!).Single(element => element.widget is M.TabBar);
                    var ink = (M.InkWell)Elements(bar).Where(element => element.widget is M.InkWell).ElementAt(expected).widget;
                    view.DispatchPlatformEvent(() => { ink.onHover!(true); ink.onFocusChange!(true); });
                    if (hovered != expected || focused != expected)
                        throw new Exception($"Tab {expected}: hover={hovered}, focus={focused}");
                }
                Console.WriteLine("Tabs: pointer selection, animation completion, repeated taps and hover/focus indices PASS");
            }

            var independentKey = new GlobalKey<IState>();
            var dependentKey = new GlobalKey<IState>();
            double observedWidth = 0;
            var independent = new StatefulBuilder(key: independentKey, builder: (_, _) => new SizedBox(width: 10, height: 10));
            var dependent = new StatefulBuilder(key: dependentKey, builder: (ctx, _) =>
            { observedWidth = MediaQuery.widthOf(ctx); return new SizedBox(width: 10, height: 10); });
            void MoveKeyedChildren(bool right)
            {
                Widget Branch(bool target, double width) => new MediaQuery(data: new MediaQueryData(size: new Size(width, Height)),
                    child: new SizedBox(width: 100, child: new Column(children: target ? [independent, dependent] : [])));
                view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(
                    locale: new Locale("en", "US"), home: new Row(children: [Branch(!right, 111), Branch(right, 222)])))));
                Pump("keyed-reparent-" + right);
            }
            MoveKeyedChildren(false);
            var independentState = independentKey.currentState;
            var dependentState = dependentKey.currentState;
            foreach (var right in new[] { true, false, true })
            {
                MoveKeyedChildren(right);
                if (!ReferenceEquals(independentState, independentKey.currentState) || !ReferenceEquals(dependentState, dependentKey.currentState))
                    throw new Exception("GlobalKey reparenting replaced retained State");
                if (observedWidth != (right ? 222 : 111)) throw new Exception("Reparented dependent did not observe its new inherited value");
            }
            Console.WriteLine("GlobalKey reparent: dependency-free and inherited-dependent State survives repeated parent moves PASS");

            BuildContext? context = null;
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(locale: new Locale("en", "US"),
                home: new Builder(builder: ctx => { context = ctx; return new M.Scaffold(body: new Text("Picker fixture")); })))));
            Pump("home");
            Doroti.Runtime.Future<DateTime?>? date = null;
            view.DispatchPlatformEvent(() => date = M.Date_pickerLibrary.showDatePicker(context!, initialDate: new DateTime(2026, 9, 7),
                firstDate: new DateTime(2024, 1, 1), lastDate: new DateTime(2028, 12, 31)));
            Pump("date-open");
            TapElement(Elements(binding.rootElement!).Single(element => element.widget is Text { data: "18" }));
            Pump("date-selected");
            Press("OK");
            Pump("date-closed");
            if (!date!.GetAwaiter().IsCompleted || date.GetAwaiter().GetResult() != new DateTime(2026, 9, 18)) throw new Exception("Mounted date picker lost selected date");
            view.DispatchPlatformEvent(() => date = M.Date_pickerLibrary.showDatePicker(context!, initialDate: new DateTime(2026, 9, 7),
                firstDate: new DateTime(2024, 1, 1), lastDate: new DateTime(2028, 12, 31)));
            Pump("date-cancel-open");
            Press("Cancel");
            Pump("date-cancel");
            if (!date!.GetAwaiter().IsCompleted || date.GetAwaiter().GetResult() is not null) throw new Exception("Mounted date cancellation was not null");
            Doroti.Runtime.Future<M.TimeOfDay?>? time = null;
            view.DispatchPlatformEvent(() => time = M.Time_pickerLibrary.showTimePicker(context!, new M.TimeOfDay(15, 23),
                builder: (ctx, child) => new MediaQuery(data: MediaQuery.of(ctx).copyWith(alwaysUse24HourFormat: true), child: child!)));
            Pump("time-open");
            TapDial(5.0 / 12, inner: true);
            Pump("time-hour-selected");
            TapDial(45.0 / 60, inner: false);
            Pump("time-selected");
            Press("OK");
            Pump("time-closed");
            if (!time!.GetAwaiter().IsCompleted) throw new Exception("Time picker confirmation did not complete");
            var selected = time.GetAwaiter().GetResult();
            if (selected?.hour != 17 || selected.minute != 45) throw new Exception("Mounted time picker lost selected time");
            Console.WriteLine("mounted native pickers: pointer date select/confirm/cancel, 24h clock hour/minute/confirm, raster scenes PASS");

            using var outer = new ScrollController();
            using var inner = new ScrollController();
            var seen = new List<(long Depth, double Pixels)>();
            var rootBar = new M.AppBar(title: new Text("Root toolbar"), notificationPredicate: notification =>
            { seen.Add((notification.depth, notification.metrics.pixels)); return notification.depth == 0; });
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(locale: new Locale("en", "US"), home:
                new M.Scaffold(appBar: rootBar, body: new SingleChildScrollView(controller: outer, child: new Column(children: [
                    new M.FilledButton(child: new Text("Filled tonal label"), onPressed: () => { }),
                    M.FilledButton.CreateIcon(icon: new Icon(M.Icons.add), label: new Text("Create project"), onPressed: () => { }),
                    new M.SegmentedButton<string>(segments: [new("local", label: new Text("Local image")), new("url", label: new Text("URL image"))], selected: ["local"], onSelectionChanged: _ => { }),
                    new SizedBox(height: 120, child: new CustomScrollView(controller: inner, primary: false, slivers: [
                        M.SliverAppBar.CreateMedium(title: new Text("Nested toolbar")), new SliverToBoxAdapter(child: new SizedBox(height: 300))])),
                    new SizedBox(height: 1200),
                ])))))));
            Pump("buttons-appbar-initial");
            Console.WriteLine("appbar initial notifications: " + string.Join(",", seen));
            seen.Clear();
            view.DispatchPlatformEvent(() => inner.jumpTo(40));
            Pump("buttons-appbar-inner-scroll");
            Console.WriteLine("appbar inner notifications: " + string.Join(",", seen));
            if (seen.Any(value => value.Depth == 0 && value.Pixels == 40)) throw new Exception("Nested scroll leaked into root app bar");
            var barState = ((StatefulElement)Elements(binding.rootElement!).Single(element => ReferenceEquals(element.widget, rootBar))).state;
            var scrolledUnder = barState.GetType().GetProperty("_scrolledUnder", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            if ((bool)scrolledUnder.GetValue(barState)!) throw new Exception("Nested app bar changed the root toolbar surface");
            seen.Clear();
            view.DispatchPlatformEvent(() => outer.jumpTo(80));
            Pump("buttons-appbar-outer-scroll");
            if (!seen.Any(value => value.Depth == 0 && value.Pixels == 80)) throw new Exception("Root app bar lost its own scroll notification");
            if (!(bool)scrolledUnder.GetValue(barState)!) throw new Exception("Root toolbar did not reflect its own scroller");

            using var imageScroll = new ScrollController();
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(locale: new Locale("en", "US"), home:
                new M.Scaffold(body: new SingleChildScrollView(controller: imageScroll, child: new Column(children: [
                    new MaterialSample.SampleImageDemo(), new SizedBox(height: 1000),
                ])))))));
            Pump("image-ready");
            var imageFailures = Elements(binding.rootElement!).Where(element => element.widget is Text { data: not null } text && text.data == "Image could not be loaded.")
                .Select(element => ((Text)element.widget).data).ToArray();
            if (imageFailures.Length > 0) throw new Exception(string.Join("\n", imageFailures));
            if (images.Decodes != 1 || images.Rasterizations != 0) throw new Exception($"Image mount should decode once without color extraction: {images.Decodes}/{images.Rasterizations}");
            for (var step = 0; step < 8; step++)
            {
                view.DispatchPlatformEvent(() => imageScroll.jumpTo(step % 2 == 0 ? 80 : 0));
                host.Fire();
            }
            Pump("image-scrolled");
            if (images.Decodes != 1 || images.Rasterizations != 0) throw new Exception("Scrolling repeated image decode or color extraction");
            TapElement(Elements(binding.rootElement!).Single(element => element.widget is M.FilledButton button && button.child is Text { data: "Extract colors" }));
            Pump("image-colors-extracted");
            if (images.Rasterizations != 2) throw new Exception($"Explicit extraction should rasterize light/dark once each: {images.Rasterizations}");
            view.DispatchPlatformEvent(() => imageScroll.jumpTo(80));
            Pump("image-colors-scrolled");
            if (images.Decodes != 1 || images.Rasterizations != 2) throw new Exception($"Scrolling repeated explicit palette work: {images.Decodes}/{images.Rasterizations}");
            Console.WriteLine("image sample: shared original decode once, explicit light/dark extraction only, no repeated scroll work PASS");

            var sampleBar = new M.AppBar(title: new Text("Sample root"));
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(locale: new Locale("en", "US"), home:
                new M.Scaffold(appBar: sampleBar, body: new MaterialSample.ComponentsScreen(true, new GlobalKey<M.ScaffoldState>()))))));
            Pump("sample-drawer-initial");
            var drawer = Elements(binding.rootElement!).Single(element => element.widget is MaterialSample.GalleryDrawer);
            var destinations = Elements(drawer).Where(element => element.widget is M.NavigationDrawerDestination).ToArray();
            foreach (var destination in destinations)
            {
                var ink = Bounds(Elements(destination).Single(element => element.widget is M.InkWell));
                var indicator = Bounds(Elements(destination).Single(element => element.widget is M.NavigationIndicator));
                if (Math.Abs(ink.left - indicator.left) > 0.01 || Math.Abs(ink.width - indicator.width) > 0.01)
                    throw new Exception($"Drawer selection and InkWell bounds differ: {indicator} / {ink}");
            }
            TapElement(Elements(destinations[1]).First(element => element.widget is Icon));
            Pump("sample-drawer-icon-selected");
            CheckDrawerSelection(1);
            var thirdInk = Bounds(Elements(destinations[2]).Single(element => element.widget is M.InkWell));
            Tap(new Offset(thirdInk.right + 5, thirdInk.center.dy));
            Pump("sample-drawer-outside-tap");
            CheckDrawerSelection(1);
            Tap(new Offset(thirdInk.right - 5, thirdInk.center.dy));
            Pump("sample-drawer-edge-selected");
            CheckDrawerSelection(2);
            Console.WriteLine("sample drawer: indicator/InkWell bounds match; icon and inside edge select; outside edge does not select PASS");
            void CheckDrawerSelection(long expected)
            {
                var current = (M.NavigationDrawer)Elements(drawer).Single(element => element.widget is M.NavigationDrawer).widget;
                if (current.selectedIndex != expected) throw new Exception($"Drawer hit region selected {current.selectedIndex}, expected {expected}");
            }
            Rect Bounds(Element element)
            {
                var box = (Doroti.Framework.Rendering.RenderBox)element.findRenderObject()!;
                return box.localToGlobal(Offset.zero) & box.size;
            }
            var sampleState = ((StatefulElement)Elements(binding.rootElement!).Single(element => ReferenceEquals(element.widget, sampleBar))).state;
            var components = ((StatefulElement)Elements(binding.rootElement!).Single(element => element.widget is MaterialSample.ComponentsScreen)).state;
            var sampleScroll = (ScrollController)components.GetType().GetField("_secondScroll", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(components)!;
            var topBarsPassed = false;
            for (var step = 0; step < 40; step++)
            {
                view.DispatchPlatformEvent(() => sampleScroll.jumpTo(Math.Min(sampleScroll.offset + 120, sampleScroll.position.maxScrollExtent)));
                for (var frame = 0; frame < 3; frame++)
                {
                    host.Fire(); Thread.Sleep(10);
                    if (!(bool)scrolledUnder.GetValue(sampleState)!) throw new Exception($"Sample root app bar reset while scrolling at {sampleScroll.offset}");
                }
                var topBars = Elements(binding.rootElement!).FirstOrDefault(element => element.widget is MaterialSample.ComponentSection section && section.Label == "Top app bars");
                if (topBars is not null && Bounds(topBars).bottom < 56) topBarsPassed = true;
                if (step % 10 == 0) Pump("sample-scroll-" + step);
            }
            if (!topBarsPassed) throw new Exception("Sample did not scroll past its inline app bars");
            view.DispatchPlatformEvent(() => sampleScroll.jumpTo(0));
            Pump("sample-scroll-return-top");
            if ((bool)scrolledUnder.GetValue(sampleState)!) throw new Exception("Sample root app bar did not reset at the outer scroll origin");
            Console.WriteLine("sample app bars: inline section enters/exits viewport in the two-column sample; root state stable until outer scroll returns to zero PASS (reported flicker not reproduced)");

            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new MaterialSample.SampleApp(acrylicAvailable: false))));
            Pump("full-sample-initial");
            var homeBar = Elements(binding.rootElement!).Single(element => element.widget is M.AppBar { title: Text { data: "Doroti Material 3" } });
            var homeBarState = ((StatefulElement)homeBar).state;
            var homeComponents = ((StatefulElement)Elements(binding.rootElement!).Single(element => element.widget is MaterialSample.ComponentsScreen)).state;
            var homeScroll = (ScrollController)homeComponents.GetType().GetField("_firstScroll", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(homeComponents)!;
            var checkedForeignMetrics = false;
            for (var step = 0; step < 120; step++)
            {
                var target = step < 60 ? Math.Min(homeScroll.offset + 150, homeScroll.position.maxScrollExtent) : Math.Max(0, homeScroll.offset - 150);
                view.DispatchPlatformEvent(() => homeScroll.jumpTo(target));
                for (var frame = 0; frame < 4; frame++)
                {
                    host.Fire(); Thread.Sleep(10);
                    if ((bool)scrolledUnder.GetValue(homeBarState)! != (homeScroll.offset > 0))
                        throw new Exception($"Full SampleHome app bar mismatch at {homeScroll.offset}, step {step}, frame {frame}");
                }
                // All sections can be mounted on the initial frame. Capture the
                // scrolled baseline before testing any forwarded inline metrics.
                if (step % 20 == 0) Pump("full-sample-scroll-" + step);
                var inlineBars = Elements(binding.rootElement!).FirstOrDefault(element => element.widget is MaterialSample.ComponentSection section && section.Label == "Top app bars");
                if (!checkedForeignMetrics && inlineBars is not null && homeScroll.offset > 0)
                {
                    var inlineScrolls = Elements(inlineBars).OfType<StatefulElement>().Where(element => element.state is ScrollableState)
                        .Select(element => (ScrollableState)element.state).ToArray();
                    for (var inlineIndex = 0; inlineIndex < inlineScrolls.Length; inlineIndex++)
                    {
                        var inline = inlineScrolls[inlineIndex];
                        // Forwarded metrics can reach an observer at depth zero.
                        // Source identity, rather than that delivery depth alone,
                        // must keep a demo at zero from resetting the outer bar.
                        var metrics = new ScrollMetricsNotification(metrics: inline.position.copyWith(), context: inline.notificationContext!);
                        view.DispatchPlatformEvent(() => metrics.dispatch(homeComponents.context));
                        Pump("appbar-foreign-metrics-" + inlineIndex);
                        if (metrics.depth != 0 || !(bool)scrolledUnder.GetValue(homeBarState)!)
                            throw new Exception("Inline app bar metrics reset the outer app bar through the observer");
                        var actualSurface = (M.Material)Elements(homeBar).First(element => element.widget is M.Material).widget;
                        if (!Equals(actualSurface.color, M.Theme.of(homeBar).colorScheme.surfaceContainer))
                            throw new Exception("Foreign metrics changed the outer app bar surface color");
                        var rasterDirectory = System.IO.Path.Combine(WindowsSampleContracts.OutputDirectory, "mounted");
                        using var beforeRaster = SKBitmap.Decode(System.IO.Path.Combine(rasterDirectory, "full-sample-scroll-0.png"));
                        using var afterRaster = SKBitmap.Decode(System.IO.Path.Combine(rasterDirectory, "appbar-foreign-metrics-" + inlineIndex + ".png"));
                        if (beforeRaster.GetPixel(400, 20) != afterRaster.GetPixel(400, 20))
                            throw new Exception("Foreign metrics changed the rasterized outer app bar background");
                    }
                    if (inlineScrolls.Length != 2) throw new Exception("Medium and Large demo scrollers were not mounted");
                    checkedForeignMetrics = true;
                }
            }
            if (!checkedForeignMetrics) throw new Exception("Foreign app bar scroll source was not exercised");
            Console.WriteLine("full SampleHome app bar scroll down/up lifecycle PASS");

            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(locale: new Locale("en", "US"), home:
                new Builder(builder: ctx => new MediaQuery(data: MediaQuery.of(ctx).copyWith(size: new Size(1280, 900)), child:
                    new MaterialSample.SampleHome(0, 0, false, false, null, () => { }, _ => { }, _ => { })))))));
            Pump("wide-home-initial");
            var wideBar = (StatefulElement)Elements(binding.rootElement!).Single(element => element.widget is M.AppBar { title: Text { data: "Doroti Material 3" } });
            var wideComponents = (MaterialSample.ComponentsState)((StatefulElement)Elements(binding.rootElement!).Single(element => element.widget is MaterialSample.ComponentsScreen)).state;
            foreach (var field in new[] { "_firstScroll", "_secondScroll" })
            {
                var controller = (ScrollController)wideComponents.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(wideComponents)!;
                view.DispatchPlatformEvent(() => controller.jumpTo(80));
                Pump("wide-home" + field);
                if (!(bool)scrolledUnder.GetValue(wideBar.state)!) throw new Exception("An outer sample column lost app bar scroll feedback");
                view.DispatchPlatformEvent(() => controller.jumpTo(0));
                Pump("wide-home" + field + "-top");
                if ((bool)scrolledUnder.GetValue(wideBar.state)!) throw new Exception("An outer sample column did not reset app bar feedback at zero");
            }
            Console.WriteLine("app bar source guard: both outer columns accepted; forwarded Medium/Large metrics rejected; surface color preserved PASS");

            void Press(string label)
            {
                var button = Elements(binding.rootElement!).Single(element => element.widget is M.TextButton widget
                    && widget.child is Text text && string.Equals(text.data, label, StringComparison.OrdinalIgnoreCase));
                TapElement(button);
            }
            void TapElement(Element element)
            {
                var box = (Doroti.Framework.Rendering.RenderBox)element.findRenderObject()!;
                Tap(box.localToGlobal(box.size.center(Offset.zero)));
            }
            void TapDial(double turn, bool inner)
            {
                var element = Elements(binding.rootElement!).Single(element => element.widget is M._Dial__time_picker);
                var box = (Doroti.Framework.Rendering.RenderBox)element.findRenderObject()!;
                var radius = box.size.shortestSide / 2 - 28 - (inner ? 28 : 0);
                var angle = Math.PI / 2 - turn * Math.PI * 2;
                Tap(box.localToGlobal(box.size.center(Offset.zero) + new Offset(radius * Math.Cos(angle), -radius * Math.Sin(angle))));
            }
            void Tap(Offset position)
            {
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new Doroti.Framework.Gestures.PointerDownEvent(
                    viewId: 1, pointer: 1, position: position, buttons: 1, timeStamp: Doroti.Runtime.Duration.Create(milliseconds: Environment.TickCount64))));
                view.DispatchPlatformEvent(() => binding.handlePointerEvent(new Doroti.Framework.Gestures.PointerUpEvent(
                    viewId: 1, pointer: 1, position: position, timeStamp: Doroti.Runtime.Duration.Create(milliseconds: Environment.TickCount64 + 20))));
            }
            void Pump(string stage, int frames = 60)
            {
                for (var frame = 0; frame < frames; frame++) { host.Fire(); Thread.Sleep(10); }
                if (errors.Count != 0) throw new Exception(stage + ": " + string.Join("\n", errors.Select(error => error.exceptionThrown)));
                using var reference = AppBarRaster ? SKSurface.Create(new SKImageInfo(Width, Height)) : null;
                if (reference is not null)
                {
                    var direct = renderer.Paint(reference, Width, Height);
                    if (direct is { } done) renderer.CompletePaint(done, DorotiFrameTerminal.submitted);
                }
                using var surface = gpu is null ? SKSurface.Create(new SKImageInfo(Width, Height)) : SKSurface.Create(gpu.Context, true, new SKImageInfo(Width, Height));
                // Exercise misses and hits for the same picture, unlike CPU-only
                // snapshots, which never entered the native GPU raster cache.
                if (gpu is not null)
                    for (var replay = 0; replay < 2; replay++)
                    {
                        var replayCompletion = renderer.Paint(surface, Width, Height);
                        if (replayCompletion is { } complete) renderer.CompletePaint(complete, DorotiFrameTerminal.submitted);
                    }
                var completion = renderer.Paint(surface, Width, Height);
                if (completion is null || renderer.Diagnostics.Failed > 0) throw new Exception(stage + ": native raster failed");
                renderer.CompletePaint(completion.Value, DorotiFrameTerminal.submitted);
                if (surface.Canvas.SaveCount != 1 || !surface.Canvas.TotalMatrix.IsIdentity ||
                    surface.Canvas.DeviceClipBounds != new SKRectI(0, 0, Width, Height))
                    throw new Exception($"{stage}: scene leaked canvas state: saves={surface.Canvas.SaveCount}, clip={surface.Canvas.DeviceClipBounds}, matrix={surface.Canvas.TotalMatrix}");
                using var image = surface.Snapshot();
                using var png = image.Encode(SKEncodedImageFormat.Png, 100);
                var output = System.IO.Path.Combine(WindowsSampleContracts.OutputDirectory, "mounted");
                Directory.CreateDirectory(output);
                File.WriteAllBytes(System.IO.Path.Combine(output, stage + ".png"), png.ToArray());
                if (reference is not null)
                {
                    using var expectedImage = reference.Snapshot();
                    using var expected = SKBitmap.FromImage(expectedImage);
                    using var actual = SKBitmap.FromImage(image);
                    var regions = Elements(binding.rootElement!).Where(element => element.widget is M.AppBar)
                        .Select(element => (Doroti.Framework.Rendering.RenderBox)element.findRenderObject()!)
                        .Select(box => box.localToGlobal(Offset.zero) & box.size).ToArray();
                    var changed = 0;
                    foreach (var region in regions)
                    {
                        var differing = 0;
                        for (var y = Math.Max(0, (int)Math.Ceiling(region.top)); y < Math.Min(Height, (int)region.bottom); y++)
                        for (var x = Math.Max(0, (int)Math.Ceiling(region.left)); x < Math.Min(Width, (int)region.right); x++)
                        {
                            var a = actual.GetPixel(x, y); var b = expected.GetPixel(x, y);
                            if (Math.Abs(a.Red - b.Red) + Math.Abs(a.Green - b.Green) + Math.Abs(a.Blue - b.Blue) <= 60) continue;
                            // Inline bars move at fractional coordinates. GPU picture
                            // sampling and CPU text antialiasing differ at glyph edges;
                            // compare their solid interiors. Keep the root toolbar's
                            // original full-pixel check, including its text and actions.
                            if (region.top != 0 || region.left != 0)
                            {
                                if (x < 1 || y < 1 || x >= Width - 1 || y >= Height - 1) continue;
                                var solid = true;
                                for (var dy = -1; dy <= 1; dy++)
                                for (var dx = -1; dx <= 1; dx++)
                                    solid &= expected.GetPixel(x + dx, y + dy) == b;
                                if (!solid) continue;
                            }
                            differing++;
                        }
                        changed = Math.Max(changed, differing);
                        if (differing > 250) Console.WriteLine($"Differing app bar region: {region.left},{region.top},{region.width},{region.height}: {differing}");
                    }
                    Console.WriteLine($"{stage}: max appbar differing pixels={changed}, diagnostics={renderer.Diagnostics.PictureRasterCacheHits}/{renderer.Diagnostics.PictureRasterCacheMisses}");
                    if (changed > 250)
                    {
                        using var expectedPng = expectedImage.Encode(SKEncodedImageFormat.Png, 100);
                        File.WriteAllBytes(System.IO.Path.Combine(output, stage + "-direct.png"), expectedPng.ToArray());
                        throw new Exception($"{stage}: GPU cache changed {changed} app bar pixels");
                    }
                }
            }
        }
        finally { FlutterError.onError = previousError; }
    }

    private static IEnumerable<Element> Elements(Element root)
    {
        yield return root;
        var children = new List<Element>(); root.visitChildren(children.Add);
        foreach (var child in children) foreach (var nested in Elements(child)) yield return nested;
    }
    private sealed class CountingImages(SkiaSceneRenderer renderer) : IImageHostCapability
    {
        internal int Decodes, Rasterizations;
        public ValueTask<Doroti.Ui.Image> DecodeAsync(ReadOnlyMemory<byte> bytes, DartUiInvocation invocation, CancellationToken cancellationToken = default)
        { Interlocked.Increment(ref Decodes); return renderer.DecodeAsync(bytes, invocation, cancellationToken); }
        public ValueTask<Doroti.Ui.Image> DecodeSizedAsync(ReadOnlyMemory<byte> bytes, Func<long, long, TargetImageSize?> targetSize,
            bool allowUpscaling, DartUiInvocation invocation, CancellationToken cancellationToken = default)
        { Interlocked.Increment(ref Decodes); return renderer.DecodeSizedAsync(bytes, targetSize, allowUpscaling, invocation, cancellationToken); }
        public ValueTask<Doroti.Ui.Image> RasterizeAsync(Picture picture, int width, int height, DartUiInvocation invocation, CancellationToken cancellationToken = default)
        { Interlocked.Increment(ref Rasterizations); return renderer.RasterizeAsync(picture, width, height, invocation, cancellationToken); }
    }
    private sealed class Host : IViewHostCapability, IFrameHostCapability, ISkiaSceneRendererHost, IPlatformMessageHostCapability, IPlatformEnvironmentHostCapability, IInputHostCapability, IViewFocusRequestCapability, ITextInputHostCapability
    {
        public event System.Action<DorotiTextEditingState>? EditingStateChanged;
        public event System.Action<DorotiFloatingCursorEvent>? FloatingCursorChanged;
        public void FloatCursor(DorotiFloatingCursorEvent point) => FloatingCursorChanged?.Invoke(point);
        public void Edit(DorotiTextEditingState state) => EditingStateChanged?.Invoke(state);
        public event System.Action<DorotiTextInputAction>? ActionPerformed { add { } remove { } }
        public void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState) { }
        public void UpdateState(DorotiTextEditingState state) { }
        public void SetCaretRect(Rect rect) { }
        public void ClearClient() { }
        private System.Action<TimeSpan>? _frame;
        internal bool HasPendingFrame => _frame is not null;
        public System.Action<string, ReadOnlyMemory<byte>?>? MessageSent;
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
        {
            MessageSent?.Invoke(channel, data);
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(channel == "flutter/platform" ? "[null]"u8.ToArray() : (ReadOnlyMemory<byte>?)null);
        }
        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
        public void RequestFocus(ViewFocusState state, ViewFocusDirection direction) { }
        public event System.Action<PointerDataPacket>? PointerData;
        public void SendPointer(PointerDataPacket packet) => PointerData?.Invoke(packet);
        public event System.Action<KeyData>? KeyData;
        public void SendKey(KeyData data) => KeyData?.Invoke(data);
        public event System.Action<RawFocusData>? FocusData { add { } remove { } }
        public void Fire() { var frame = _frame; _frame = null; frame?.Invoke(DorotiFrameClock.Now); }
        public void ScheduleFrame(System.Action<TimeSpan> callback) => _frame = callback;
        public ViewMetrics Metrics { get; private set; } = new(new Size(Width, Height), 1, default, default, default, AppLifecycleState.resumed, 1, 1);
        public void SetInsets(double bottom)
        {
            Metrics = Metrics with { viewInsets = new(0, 0, 0, bottom), generation = Metrics.generation + 1 };
            ViewEpoch = ViewEpoch with { MetricsGeneration = Metrics.generation };
            MetricsChanged?.Invoke(Metrics);
        }
        public void Resize(int width)
        {
            var generation = ViewEpoch.ResizeTargetGeneration + 1;
            Metrics = Metrics with { physicalSize = new Size(width, Height), generation = generation };
            ViewEpoch = new(1, generation, generation, width, Height, width, Height, 1, 1, 0);
            ResizeTarget = new(generation, width, Height, width, Height, 1, 0);
            MetricsChanged?.Invoke(Metrics);
        }
        public DorotiViewEpoch ViewEpoch { get; private set; } = new(1, 1, 1, Width, Height, Width, Height, 1, 1, 0);
        public DorotiResizeEpoch ResizeTarget { get; private set; } = new(1, Width, Height, Width, Height, 1, 0);
        public long InputSequence => 0;
        public long SurfaceGeneration => 1;
        public PlatformConfiguration Configuration { get; init; } = new([new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows);
        public event System.Action<ViewMetrics>? MetricsChanged;
        public event System.Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        public event System.Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
        public event System.Action<long, TimeSpan>? InputReceived { add { } remove { } }
        public event System.Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
        public void Show() { }
        public void Resize(Size size) { }
        public void Close() { }
        public void Dispose() { }
        public void RequestInvalidate() { }
        public void UpdateSemantics(SemanticsUpdate update) { }
        public void ClearSemantics() { }
    }
}
