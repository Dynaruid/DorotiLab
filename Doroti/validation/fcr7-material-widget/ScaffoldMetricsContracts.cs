using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using M = Doroti.Framework.Material;

internal static partial class MountedPickerContracts
{
    internal static void VerifyScaffoldMetrics()
    {
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "metrics", "metrics", "metrics");
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("metrics")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, renderer)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
            .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, new ClipboardFixtureHost())
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
        var errors = new List<FlutterErrorDetails>();
        var previous = FlutterError.onError; FlutterError.onError = errors.Add;
        try
        {
            var binding = new WidgetsFlutterBinding(dispatcher);
            var data = new MediaQueryData(size: new Size(800, Height), padding: EdgeInsets.CreateOnly(top: 24, bottom: 20),
                viewPadding: EdgeInsets.CreateOnly(top: 24, bottom: 20));
            MediaQueryData? bodyData = null, barData = null;
            double bodyWidth = 0;
            System.Action<System.Action>? change = null;
            var scaffold = new MetricsScaffold(
                body: new Builder(builder: context =>
                {
                    bodyData = MediaQuery.of(context);
                    return new LayoutBuilder(builder: (_, constraints) => { bodyWidth = constraints.maxWidth; return SizedBox.CreateExpand(); });
                }),
                bottom: new SizedBox(height: 50, child: new Builder(builder: context =>
                { barData = MediaQuery.of(context); return new SizedBox(); })));
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(
                locale: new Locale("en", "US"), home: new StatefulBuilder(builder: (_, setter) =>
                { change = setter; return new MediaQuery(data: data, child: scaffold); })))));
            void Pump() { for (var frame = 0; frame < 8; frame++) host.Fire(); }
            Pump();
            var builds = scaffold.State!.Builds;
            foreach (var width in new[] { 900, 1100, 800 })
            {
                view.DispatchPlatformEvent(() => { host.Resize(width); change!(() => data = data.copyWith(size: new Size(width, Height))); });
                Pump();
                if (bodyData!.size.width != width || barData!.size.width != width || bodyWidth != width)
                    throw new Exception("Scaffold slot failed to receive current size/constraints");
                if (scaffold.State.Builds != builds) throw new Exception("Size-only update rebuilt Scaffold");
            }
            view.DispatchPlatformEvent(() => change!(() => data = data.copyWith(
                viewInsets: EdgeInsets.CreateOnly(bottom: 300), padding: EdgeInsets.CreateOnly(top: 24))));
            Pump();
            if (bodyData!.padding.top != 0 || bodyData.padding.bottom != 0 || bodyData.viewInsets.bottom != 0 ||
                barData!.viewInsets.bottom != 300 || barData.padding.top != 0 || scaffold.State.Builds <= builds)
                throw new Exception("Scaffold keyboard/padding transformation changed");
            view.DispatchPlatformEvent(() => change!(() => data = data.copyWith(viewInsets: EdgeInsets.zero,
                padding: EdgeInsets.CreateOnly(top: 24, bottom: 20))));
            Pump();
            if (barData!.padding.bottom != 20 || bodyData!.viewInsets.bottom != 0)
                throw new Exception("Scaffold slot padding did not restore");
            if (errors.Count != 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
            Console.WriteLine("Scaffold slot metrics PASS: size/constraints propagation without Scaffold rebuild, keyboard insets and padding restore");
        }
        finally { FlutterError.onError = previous; }
    }

    private sealed class MetricsScaffold(Widget body, Widget bottom) : M.Scaffold(
        appBar: new PreferredSize(preferredSize: new Size(0, 56), child: new SizedBox()), body: body, bottomNavigationBar: bottom)
    {
        internal MetricsScaffoldState? State;
        public override IState createState() => State = new MetricsScaffoldState();
    }
    private sealed class MetricsScaffoldState : M.ScaffoldState
    {
        internal int Builds;
        public override Widget build(BuildContext context) { Builds++; return base.build(context); }
    }
}

