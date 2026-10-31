using Doroti.Framework.Scheduler;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;

internal static class ScrollContinuityRegression
{
    internal static void Run()
    {
        // Drive the production AnimationController/Ticker/Scheduler, rather
        // than merely sampling the simulation formula in isolation.
        foreach (var hz in new[] { 60, 120 })
        foreach (var missed in new[] { false, true })
        {
            using var dispatcher = new PlatformDispatcher();
            using var scope = dispatcher.EnterScope();
            using var scheduler = new TestScheduler(dispatcher);
            var provider = new Provider();
            var target = new Target();
            var simulation = new ClampingScrollSimulation(500.125, 1000);
            var activity = new BallisticScrollActivity(target, simulation, provider, true);
            var previous = simulation.x(0);
            for (var i = 0; i <= hz; i++)
            {
                // A real 50 ms scheduling gap, with no fabricated intermediate
                // pulses and no fixed-delta time correction in the engine.
                if (missed && i > hz / 6 && i < hz / 6 + hz / 20) continue;
                var us = (long)Math.Round(i * 1_000_000.0 / hz);
                scheduler.handleBeginFrame(Duration.Create(microseconds: us));
                scheduler.handleDrawFrame();
                var expected = simulation.x(us / 1_000_000.0);
                Check(Math.Abs(target.Pixels - expected) < 1e-8, "ticker offset follows elapsed vsync time");
                Check(target.Pixels >= previous, "fractional ballistic offset never reverses");
                previous = target.Pixels;
            }
            Check(provider.Last is { isActive: false, scheduled: false }, "ballistic ticker retires at rest");
            activity.dispose();
        }

        using var semanticDispatcher = new PlatformDispatcher();
        using var semanticScope = semanticDispatcher.EnterScope();
        using var semanticScheduler = new TestScheduler(semanticDispatcher);
        foreach (var axis in Enum.GetValues<AxisDirection>())
        {
            var context = new Context(axis);
            var position = new Position(context);
            position.applyViewportDimension(300);
            position.applyContentDimensions(0, 1000);
            var (forward, backward) = axis switch
            {
                AxisDirection.up => (SemanticsAction.scrollDown, SemanticsAction.scrollUp),
                AxisDirection.down => (SemanticsAction.scrollUp, SemanticsAction.scrollDown),
                AxisDirection.left => (SemanticsAction.scrollRight, SemanticsAction.scrollLeft),
                _ => (SemanticsAction.scrollLeft, SemanticsAction.scrollRight),
            };
            Check(context.Actions!.SetEquals([forward]), "start boundary exposes only the available scroll action");
            position.correctPixels(500.125);
            position.notifyListeners();
            Check(context.Actions.SetEquals([forward, backward]), "middle exposes both scroll actions");
            var retained = context.Actions;
            var changes = context.Changes;
            for (var i = 0; i < 20; i++)
            {
                position.correctPixels(500.25 + i / 100.0);
                position.notifyListeners();
            }
            Check(ReferenceEquals(retained, context.Actions) && changes == context.Changes,
                "subpixel movement retains unchanged semantics actions");
            position.goBallistic(1000);
            var ballistic = position.activity;
            for (var i = 0; i < 20; i++) position.applyContentDimensions(0, 1000);
            Check(ReferenceEquals(ballistic, position.activity), "unchanged dimensions do not restart ballistic activity");
            var beforeContact = position.pixels;
            position.hold(() => { });
            Check(position.activity is HoldScrollActivity && position.pixels == beforeContact,
                "recontact stops ballistic activity without changing fractional offset");
            position.goIdle();
            position.correctPixels(1000);
            position.notifyListeners();
            Check(context.Actions.SetEquals([backward]), "end boundary removes the unavailable scroll action");
            position.applyContentDimensions(1000, 1000);
            Check(context.Actions.Count == 0, "collapsed extent removes both actions");
            position.dispose();
        }
        Console.WriteLine("PASS: production ticker at 60/120 Hz, missed pulses, fractional offsets, semantic boundaries");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class TestScheduler(PlatformDispatcher dispatcher) : SchedulerBinding(dispatcher)
    {
        public override void scheduleFrame() { }
    }
    private sealed class Provider : TickerProvider
    {
        internal Ticker? Last;
        public Ticker createTicker(Action<Duration> callback) => Last = new(callback);
    }
    private sealed class Target : ScrollActivityDelegate
    {
        internal double Pixels;
        public AxisDirection axisDirection => AxisDirection.down;
        public double setPixels(double pixels) { Pixels = pixels; return 0; }
        public void applyUserOffset(double delta) => Pixels += delta;
        public void goIdle() { }
        public void goBallistic(double velocity) { }
    }
    private sealed class Context(AxisDirection axis) : ScrollContext
    {
        internal HashSet<SemanticsAction>? Actions;
        internal int Changes;
        public BuildContext? notificationContext => null;
        public BuildContext storageContext => null!;
        public TickerProvider vsync { get; } = new Provider();
        public AxisDirection axisDirection => axis;
        public double devicePixelRatio => 3;
        public void setIgnorePointer(bool value) { }
        public void setCanDrag(bool value) { }
        public void setSemanticsActions(HashSet<SemanticsAction> actions) { Actions = actions; Changes++; }
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
}
