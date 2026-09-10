using Doroti.Framework.Gestures;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using PointerDownEvent = Doroti.Framework.Gestures.PointerDownEvent;
using PointerMoveEvent = Doroti.Framework.Gestures.PointerMoveEvent;
using PointerUpEvent = Doroti.Framework.Gestures.PointerUpEvent;

internal static class TouchSlopContracts
{
    internal static void Verify()
    {
        VerifyVelocityAfterPause();
        VerifyIosVelocityRing();
        using var platform = PlatformEnvironmentContext.Enter(new PlatformConfiguration(
            [new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.iOS));
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        var host = new TouchHost();
        using var view = dispatcher.RegisterView(1, host.Capabilities());
        using var binding = new WidgetsFlutterBinding(dispatcher);
        foreach (var distance in new[] { 0.0, 1, 2, 8, 17, 18, 19, 30 })
        {
            var taps = 0;
            var drags = 0;
            var tap = new TapGestureRecognizer { onTap = () => taps++ };
            var drag = new VerticalDragGestureRecognizer { onStart = _ => drags++ };
            var down = new PointerDownEvent(pointer: 1, position: new Offset(100, 100));
            tap.addPointer(down);
            drag.addPointer(down);
            binding.gestureArena.close(1);
            binding.pointerRouter.route(down);
            binding.pointerRouter.route(new PointerMoveEvent(pointer: 1,
                position: new Offset(100, 100 + distance), delta: new Offset(0, distance)));
            Console.WriteLine($"touch distance={distance} tolerance={tap.preAcceptSlopTolerance} moved={drag.globalDistanceMoved} drags={drags}");
            binding.pointerRouter.route(new PointerUpEvent(pointer: 1, position: new Offset(100, 100 + distance)));
            binding.gestureArena.sweep(1);
            tap.dispose();
            drag.dispose();
            if (taps != (distance <= 18 ? 1 : 0) || drags != (distance > 18 ? 1 : 0))
                throw new InvalidOperationException($"touch slop {distance}: taps={taps}, drags={drags}");
        }
    }

    private static void VerifyVelocityAfterPause()
    {
        foreach (var tracker in new VelocityTracker[]
        {
            new(PointerDeviceKind.touch),
            new IOSScrollViewFlingVelocityTracker(PointerDeviceKind.touch),
            new MacOSScrollViewFlingVelocityTracker(PointerDeviceKind.touch),
        })
        {
            for (var i = 0; i < 5; i++)
                tracker.addPosition(Doroti.Runtime.Duration.Create(milliseconds: i * 10), new Offset(0, i * 20));
            if (tracker.getVelocity().pixelsPerSecond.dy <= 0)
                throw new InvalidOperationException("Moving touch must retain its fling velocity.");
            Thread.Sleep(70);
            if (tracker.getVelocity().pixelsPerSecond != Offset.zero)
                throw new InvalidOperationException($"{tracker.GetType().Name}: a paused touch must not fling on release.");
        }
    }

    private static void VerifyIosVelocityRing()
    {
        foreach (var count in new[] { 1, 2, 3, 19, 20, 21, 22, 23, 40, 41 })
        {
            var tracker = new IOSScrollViewFlingVelocityTracker(PointerDeviceKind.touch);
            for (var i = 0; i < count; i++)
                tracker.addPosition(Doroti.Runtime.Duration.Create(milliseconds: i * 10), new Offset(0, i * 20));
            var velocity = tracker.getVelocity().pixelsPerSecond.dy;
            var expected = count == 1 ? 0 : count == 2 ? 100 : count == 3 ? 800 : 2000;
            if (Math.Abs(velocity - expected) > .001)
                throw new InvalidOperationException($"iOS sample count={count}: expected velocity={expected}, actual={velocity}");
        }
    }
}

file sealed class TouchHost : IViewHostCapability, IFrameHostCapability, IPlatformEnvironmentHostCapability, IPlatformMessageHostCapability
{
    public ViewMetrics Metrics { get; } = new(new Size(1080, 2400), 3, default, default, default, AppLifecycleState.resumed, 1, 7);
    public PlatformConfiguration Configuration { get; } = new([new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.iOS);
    public DorotiViewEpoch ViewEpoch => new(1, 1, Metrics.generation, 360, 800, 1080, 2400, 3, 3, 0);
    public event System.Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event System.Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
    public event System.Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event Action? CloseRequested { add { } remove { } }
    public event Action? Closed { add { } remove { } }
    public DorotiViewCapabilities Capabilities() => new DorotiViewCapabilities("touch-slop-fixture")
        .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, this)
        .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, this)
        .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, this)
        .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, this);
    public void Show() { } public void Resize(Size size) { } public void Close() { } public void Dispose() { }
    public void ScheduleFrame(System.Action<TimeSpan> callback) { }
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
}
