using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using C = Doroti.Framework.Cupertino;

internal static partial class MountedPickerContracts
{
    internal static void VerifyRouteBridges()
    {
        static void Check(bool value, string message) { if (!value) throw new Exception(message); }
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null, "route-bridge", "route-bridge", "route-bridge");
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("route-bridge")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
        var errors = new List<FlutterErrorDetails>();
        var prior = FlutterError.onError;
        FlutterError.onError = errors.Add;
        var binding = new WidgetsFlutterBinding(dispatcher);
        var key = new GlobalKey<NavigatorState>();
        var canPop = new ValueNotifier<bool>(false);
        var invocations = new List<(bool DidPop, object? Result)>();
        var flights = 0;
        var found = new Dictionary<Type, IModalRoute>();
        try
        {
            void Pump()
            {
                for (var i = 0; i < 65; i++) { host.Fire(); Thread.Sleep(10); }
                if (errors.Count != 0) throw new Exception(string.Join("\n", errors.Select(e => e.exceptionThrown)));
            }
            Widget HeroContent() => new Center(child: new Hero(tag: "shared", flightShuttleBuilder: (_, _, _, _, _) =>
            {
                flights++;
                return new SizedBox(width: 60, height: 60, child: new ColoredBox(color: new Color(0xff123456)));
            }, child: new SizedBox(width: 60, height: 60, child: new ColoredBox(color: new Color(0xff123456)))));
            Widget Page() => new C.CupertinoPageScaffold(navigationBar: new C.CupertinoNavigationBar(), child: new Builder(builder: context =>
            {
                var current = ModalRoute<object>.untypedOf(context)!;
                found[current.routeBase.GetType()] = current;
                return HeroContent();
            }));
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new M.MaterialApp(navigatorKey: key, home: HeroContent()))));
            Pump();
            var integer = new C.CupertinoPageRoute<int>(title: "Integer", builder: _ => new ValueListenableBuilder<bool>(valueListenable: canPop,
                builder: (_, allowed, _) => new PopScope<object?>(canPop: allowed,
                    onPopInvokedWithResult: (didPop, result) => invocations.Add((didPop, result)), child: Page())));
            Future<int>? integerResult = null;
            view.DispatchPlatformEvent(() => integerResult = key.currentState!.push<int>(integer));
            Pump();
            Check(ReferenceEquals(found[integer.GetType()].routeBase, integer), "untyped lookup retains integer route identity");
            Check(integer.popDisposition == RoutePopDisposition.doNotPop, "object PopScope blocks integer route");
            Future<bool>? prevented = null;
            view.DispatchPlatformEvent(() => prevented = key.currentState!.maybePop<int>(42));
            Pump();
            Check(prevented!.asTask().IsCompletedSuccessfully && !integerResult!.asTask().IsCompleted && invocations.Contains((false, 42)), "blocked pop forwards typed result without popping");
            var text = new C.CupertinoPageRoute<string>(title: "String", builder: _ => Page());
            Future<string?>? stringResult = null;
            view.DispatchPlatformEvent(() => stringResult = key.currentState!.push<string>(text));
            Pump();
            Check(ReferenceEquals(found[text.GetType()].routeBase, text), "untyped lookup retains string route identity");
            Check(((C.ICupertinoRouteTitle)text).previousTitle.value == "Integer", "previous title crosses generic route result types");
            Check(flights >= 2, "Hero transitions run across object/integer/string routes");
            view.DispatchPlatformEvent(() => key.currentState!.pop<string>("done"));
            Pump();
            Check(stringResult!.asTask().IsCompletedSuccessfully && stringResult.asTask().Result == "done", "string pop result retained");
            view.DispatchPlatformEvent(() => canPop.value = true);
            Pump();
            view.DispatchPlatformEvent(() => key.currentState!.pop<int>(42));
            Pump();
            Check(integerResult!.asTask().IsCompletedSuccessfully && integerResult.asTask().Result == 42 && invocations.Contains((true, 42)), "integer pop result retained through object PopScope");
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(new SizedBox())));
            Pump();
            Console.WriteLine("Route bridges: mixed object/int/string results, PopScope veto/update/callback, Cupertino previous title, Hero flights and unmount PASS");
        }
        finally { canPop.dispose(); FlutterError.onError = prior; }
    }
}
