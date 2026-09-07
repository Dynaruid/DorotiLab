using System.Reflection;
using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using M = Doroti.Framework.Material;

internal static partial class MountedPickerContracts
{
    internal static void VerifySampleScroll()
    {
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows));
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
            "sample-scroll", "sample-scroll", "sample-scroll");
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("sample-scroll")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, new CountingImages(renderer))
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
            .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, new ClipboardFixtureHost())
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
        var binding = new WidgetsFlutterBinding(dispatcher);
        renderer.RegisterFontAsync(File.ReadAllBytes("DorotiTestbedApp/assets/fonts/MaterialIcons-Regular.otf"), "MaterialIcons").GetAwaiter().GetResult();
        var errors = new List<FlutterErrorDetails>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        try
        {
            System.Action<System.Action>? rebuild = null;
            var width = 800.0;
            var scale = 1.0;
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(
                new M.MaterialApp(locale: new Locale("en", "US"), home: new StatefulBuilder(builder: (ctx, change) =>
                {
                    rebuild = change;
                    return new Align(child: new SizedBox(width: Math.Min(width, Width), height: Height,
                        child: new MediaQuery(data: MediaQuery.of(ctx).copyWith(size: new Size(width, Height), textScaleFactor: scale),
                            child: new MaterialSample.SampleHome(0, 0, false, false, null, () => { }, _ => { }, _ => { }))));
                })))));
            Pump("initial");
            foreach (var configuration in new[] { (800.0, 1.0), (600.0, 1.25), (1280.0, 1.0), (800.0, 1.0) })
            {
                view.DispatchPlatformEvent(() => rebuild!(() => { width = configuration.Item1; scale = configuration.Item2; }));
                Pump("reflow");
                var components = (StatefulElement)Elements(binding.rootElement!).Single(e => e.widget is MaterialSample.ComponentsScreen);
                var states = SectionStates(components);
                foreach (var field in width >= 1000 ? new[] { "_firstScroll", "_secondScroll" } : new[] { "_firstScroll" })
                {
                    var controller = (ScrollController)components.state.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(components.state)!;
                    view.DispatchPlatformEvent(() => controller.jumpTo(0));
                    Pump("origin");
                    var extent = controller.position.maxScrollExtent;
                    Require(extent > 0 && double.IsFinite(extent), "finite initial scroll extent");
                    var scrollElement = Elements(components).OfType<StatefulElement>().Single(e =>
                        e.state is ScrollableState scroll && ReferenceEquals(scroll.position, controller.position));
                    var scrollBox = (RenderBox)scrollElement.findRenderObject()!;
                    var wheelPosition = scrollBox.localToGlobal(new Offset(2, 24));
                    view.DispatchPlatformEvent(() => binding.handlePointerEvent(new Doroti.Framework.Gestures.PointerScrollEvent(
                        viewId: 1, position: wheelPosition, scrollDelta: new Offset(0, 120))));
                    Pump("wheel");
                    Require(controller.offset > 0, "wheel input did not move the outer column");
                    Require(Math.Abs(controller.position.maxScrollExtent - extent) < .01, "wheel changed the initial extent");
                    Require(states.SequenceEqual(SectionStates(components)), "wheel mounted or replaced component section state");
                    // Each target is based on the INITIAL extent: never chase an expanding end.
                    foreach (var fraction in new[] { .15, .35, .6, .85, 1.0, .5, 0.0 })
                    {
                        view.DispatchPlatformEvent(() => controller.jumpTo(extent * fraction));
                        Pump("scroll");
                        Require(Math.Abs(controller.position.maxScrollExtent - extent) < .01,
                            $"extent changed during first traversal: {extent} -> {controller.position.maxScrollExtent}, width={width}, scale={scale}, column={field}");
                        Require(Math.Abs(controller.offset - extent * fraction) < .01, "scroll target was corrected unexpectedly");
                        Require(states.SequenceEqual(SectionStates(components)), "scroll mounted or replaced component section state");
                        if (fraction == 1 && field == (width >= 1000 ? "_secondScroll" : "_firstScroll"))
                        {
                            var image = Elements(components).Single(e => e.widget is MaterialSample.SampleImageDemo);
                            var box = (RenderBox)image.findRenderObject()!;
                            var bottom = box.localToGlobal(Offset.zero).dy + box.size.height;
                            Require(bottom > 0 && bottom <= Height + .01, "initial end did not reveal the final image section");
                        }
                    }
                    Console.WriteLine($"sample scroll: width={width}, scale={scale}, column={field}, stableExtent={extent:F2}, retainedSections={states.Length} PASS");
                }
                Require(states.Length == 29, $"expected all 29 sections ready before scroll, got {states.Length}");
            }
            Console.WriteLine("Sample scroll: wheel input, first-traversal extent, direct end, section lifetime, width/text-scale reflow and one/two-column transitions PASS; physical cadence notVerified");
        }
        finally { FlutterError.onError = previousError; }

        IState[] SectionStates(Element root) => Elements(root).OfType<StatefulElement>()
            .Where(e => e.widget is MaterialSample.ComponentSection or MaterialSample.SampleImageDemo).Select(e => e.state).ToArray();
        void Pump(string stage)
        {
            for (var frame = 0; frame < 35; frame++) { host.Fire(); Thread.Sleep(10); }
            Require(errors.Count == 0, stage + ": " + string.Join("\n", errors.Select(e => e.exceptionThrown)));
            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
            if (renderer.Paint(surface, Width, Height) is { } completion)
                renderer.CompletePaint(completion, DorotiFrameTerminal.submitted);
        }
        static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
