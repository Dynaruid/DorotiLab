using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Ui;

internal static partial class MountedPickerContracts
{
    internal static void VerifyScrollTap()
    {
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.iOS));
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        using var host = new Host { Configuration = new([new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.iOS) };
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null,
            "scroll-tap", "scroll-tap", "scroll-tap");
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("scroll-tap")
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
        using var binding = new WidgetsFlutterBinding(dispatcher);
        var controller = new ScrollController();
        var previousY = 0.0;
        var taps = 0;
        var errors = new List<FlutterErrorDetails>();
        var previousError = FlutterError.onError;
        FlutterError.onError = errors.Add;
        try
        {
            view.DispatchPlatformEvent(() => binding.attachRootWidget(binding.wrapWithDefaultView(
                new Directionality(textDirection: TextDirection.ltr,
                    child: new SingleChildScrollView(controller: controller, physics: new BouncingScrollPhysics(),
                        child: new GestureDetector(onTap: () => taps++, behavior: Doroti.Framework.Rendering.HitTestBehavior.opaque,
                            child: new SizedBox(width: Width, height: 4000)))))));
            Pump(5);
            Touch(1, 0, 200, 0);
            Touch(1, 1, 208, 30);
            Touch(1, 2, 208, 60);
            Pump(2);
            Require(taps == 1, $"idle tap: {taps}, offset={controller.offset}");
            Touch(2, 0, 400, 100);
            Touch(2, 1, 350, 120);
            Touch(2, 1, 280, 140);
            Touch(2, 2, 280, 160);
            Pump(350);
            Console.WriteLine($"settled activity={controller.position.activity}, scrolling={controller.position.isScrollingNotifier.value}, offset={controller.offset}");
            Require(!controller.position.isScrollingNotifier.value, "scroll must settle");
            Touch(3, 0, 200, 4000);
            Touch(3, 1, 208, 4030);
            Touch(3, 2, 208, 4060);
            Pump(2);
            Require(taps == 2, $"post-scroll tap: {taps}, offset={controller.offset}");
            // Pausing before release must finish the drag without starting a fling.
            Touch(4, 0, 400, 4200);
            for (var i = 1; i <= 5; i++) Touch(4, 1, 400 - i * 20, 4200 + i * 10);
            Thread.Sleep(70);
            Touch(4, 2, 300, 4350);
            Require(!controller.position.isScrollingNotifier.value, "paused release unexpectedly started inertia");
            Touch(5, 0, 200, 4360);
            Touch(5, 1, 208, 4390);
            Touch(5, 2, 208, 4420);
            Require(taps == 3, "tap immediately after paused release was swallowed");

            // 1 down + 19 moves wraps the iOS velocity ring to index zero.
            view.DispatchPlatformEvent(() => controller.jumpTo(500));
            Pump(2);
            Touch(6, 0, 400, 4500);
            for (var i = 1; i <= 19; i++) Touch(6, 1, 400 - i * 5, 4500 + i * 10);
            Touch(6, 2, 305, 4700);
            Require(controller.position.activity is BallisticScrollActivity, "wrapped ring must finish drag and start inertia");
            // A stationary touch stops inertia; the very next tap must work.
            Touch(7, 0, 200, 4710);
            Touch(7, 2, 200, 4740);
            Pump(2);
            Require(!controller.position.isScrollingNotifier.value, "stop touch left scrolling active");
            Require(taps == 3, "touch stopping inertia must not activate the child");
            Touch(8, 0, 200, 4750);
            Touch(8, 1, 208, 4780);
            Touch(8, 2, 208, 4810);
            Require(taps == 4, "tap after stopping inertia was swallowed");
            Pump(2);
            Console.WriteLine("iOS scroll tap: idle, settled, paused release, wrapped velocity ring, stop-then-tap PASS");
        }
        finally { FlutterError.onError = previousError; }

        void Touch(long pointer, int phase, double y, int ms)
        {
            var p = new Offset(100, y);
            var time = Doroti.Runtime.Duration.Create(milliseconds: ms);
            Doroti.Framework.Gestures.PointerEvent e = phase switch
            {
                0 => new Doroti.Framework.Gestures.PointerDownEvent(viewId: 1, pointer: pointer, position: p, timeStamp: time),
                1 => new Doroti.Framework.Gestures.PointerMoveEvent(viewId: 1, pointer: pointer, position: p, delta: new Offset(0, y - previousY), timeStamp: time),
                _ => new Doroti.Framework.Gestures.PointerUpEvent(viewId: 1, pointer: pointer, position: p, timeStamp: time)
            };
            previousY = y;
            view.DispatchPlatformEvent(() => binding.handlePointerEvent(e));
        }
        void Pump(int frames)
        {
            for (var i = 0; i < frames; i++) { host.Fire(); Thread.Sleep(10); }
            Require(errors.Count == 0, string.Join("\n", errors.Select(e => e.exceptionThrown)));
        }
        static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
