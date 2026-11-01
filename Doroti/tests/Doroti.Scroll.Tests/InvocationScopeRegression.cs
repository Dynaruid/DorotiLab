using Doroti.Runtime;
using Doroti.Ui;

internal static class InvocationScopeRegression
{
    internal static void Run()
    {
        var clock = new TestClock();
        using var time = DorotiExecutionContext.EnterTimeProvider(clock);
        using var dispatcher = new PlatformDispatcher();
        var first = AddView(dispatcher, 1, HostOperatingSystem.windows);
        var second = AddView(dispatcher, 2, HostOperatingSystem.android);
        using (var app = dispatcher.EnterScope())
        {
            var applicationLifetime = DorotiExecutionContext.CaptureDispatcher()!.Lifetime;
            Check(dispatcher.CurrentInvocationView is null, "application scope has no explicit view");
            using (first.EnterInvocationScope())
            {
                CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);
                using (first.EnterInvocationScope()) CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);
                var before = GC.GetAllocatedBytesForCurrentThread();
                for (var i = 0; i < 20; i++)
                {
                    using var nested = first.EnterInvocationScope();
                }
                var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
                Check(allocated < 8_000, "equivalent nested scopes avoid per-element callback/context-map churn");
                Console.WriteLine($"INFO: 20 repeated view invocation scopes allocate {allocated} bytes");

                using (second.EnterInvocationScope()) CheckCurrent(dispatcher, second, clock, HostOperatingSystem.android);
                CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);
                using (dispatcher.EnterScope())
                    Check(DorotiExecutionContext.CaptureDispatcher()!.Lifetime == applicationLifetime,
                        "explicit application scope selects application cancellation even inside a view");
                CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);

                var alternateClock = new TestClock();
                var alternateEnvironment = new PlatformConfiguration([], Brightness.dark, false, false);
                using var alternateLifetime = new CancellationTokenSource();
                Func<Action, bool> reject = _ => false;
                using (DorotiExecutionContext.EnterTimeProvider(alternateClock))
                using (DorotiExecutionContext.EnterDispatcher(reject, alternateLifetime.Token))
                using (PlatformEnvironmentContext.Enter(alternateEnvironment))
                {
                    using (first.EnterInvocationScope()) CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);
                    Check(ReferenceEquals(DorotiExecutionContext.TimeProvider, alternateClock)
                        && ReferenceEquals(PlatformEnvironmentContext.current, alternateEnvironment)
                        && DorotiExecutionContext.CaptureDispatcher()!.Lifetime == alternateLifetime.Token,
                        "nested view scope restores custom scheduler, time and environment overrides");
                }
                CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);
                try
                {
                    using var throwing = second.EnterInvocationScope();
                    throw new TestException();
                }
                catch (TestException) { }
                CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);

                var queue = new Queue<Action>();
                var alternateQueue = new Queue<Action>();
                Func<Action, bool> post = callback => { queue.Enqueue(callback); return true; };
                Func<Action, bool> alternatePost = callback => { alternateQueue.Enqueue(callback); return true; };
                var delivered = 0;
                using (DorotiExecutionContext.EnterDispatcher(post, first.InvocationLifetime))
                {
                    using (DorotiExecutionContext.EnterDispatcher(post, first.InvocationLifetime))
                        Check(DorotiExecutionContext.CaptureDispatcher()!.TryPost(() =>
                        {
                            Check(ReferenceEquals(dispatcher.CurrentInvocationView, first), "captured callbacks retain their view context");
                            delivered++;
                        }), "equivalent scheduler accepts a callback");
                    using (DorotiExecutionContext.EnterDispatcher(alternatePost, first.InvocationLifetime))
                        Check(DorotiExecutionContext.CaptureDispatcher()!.TryPost(() => delivered++),
                            "a different scheduler with the same lifetime accepts its own callback");
                }
                Check(queue.Count == 1 && alternateQueue.Count == 1, "scheduler identity is part of equivalent-scope matching");
                queue.Dequeue()();
                alternateQueue.Dequeue()();
                Check(delivered == 2, "captured callbacks run after their nested scheduler scopes exit");
                CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);

                var retained = DorotiExecutionContext.CaptureDispatcher()!;
                var flowed = Task.Run(() =>
                {
                    CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);
                    using (second.EnterInvocationScope()) CheckCurrent(dispatcher, second, clock, HostOperatingSystem.android);
                    CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);
                });
                flowed.GetAwaiter().GetResult();
                CheckCurrent(dispatcher, first, clock, HostOperatingSystem.windows);
                first.Dispose();
                Check(retained.Lifetime.IsCancellationRequested && !retained.TryPost(() => throw new Exception()),
                    "captured view callbacks cannot outlive the originating view");
                CheckThrows<ObjectDisposedException>(() => first.EnterInvocationScope(), "disposed view rejects repeated scope entry");
            }
            Check(dispatcher.CurrentInvocationView is null
                && DorotiExecutionContext.CaptureDispatcher()!.Lifetime == applicationLifetime,
                "view exit restores the enclosing application scope");
        }
        Check(ReferenceEquals(DorotiExecutionContext.TimeProvider, clock), "application exit restores outer time provider");
        Console.WriteLine("PASS: nested view scopes, cross-view identity, cancellation, custom overrides and async context restoration");
    }

    private static void CheckCurrent(PlatformDispatcher dispatcher, DorotiView view, TimeProvider clock, HostOperatingSystem os) =>
        Check(ReferenceEquals(PlatformDispatcher.instance, dispatcher) && ReferenceEquals(dispatcher.CurrentInvocationView, view)
            && DorotiExecutionContext.CaptureDispatcher()!.Lifetime == view.InvocationLifetime
            && ReferenceEquals(DorotiExecutionContext.TimeProvider, clock) && PlatformEnvironmentContext.current.operatingSystem == os,
            "the current invocation retains its dispatcher, view, lifetime, time provider and environment");

    private static DorotiView AddView(PlatformDispatcher dispatcher, ulong id, HostOperatingSystem os)
    {
        var host = new Host(id, os);
        return dispatcher.RegisterView(id, new DorotiViewCapabilities("invocation-scope-regression")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host));
    }

    private sealed class TestException : Exception;
    private sealed class TestClock : TimeProvider;
    private sealed class Host(ulong id, HostOperatingSystem os) : IViewHostCapability, IFrameHostCapability, IPlatformEnvironmentHostCapability
    {
        public ViewMetrics Metrics { get; } = new(new Size(320, 240), 1, new(), new(), new(), AppLifecycleState.resumed, 1, 1);
        public DorotiViewEpoch ViewEpoch { get; } = new(id, 1, 1, 320, 240, 320, 240, 1, 1, 0);
        public PlatformConfiguration Configuration { get; } = new([], Brightness.light, false, false, os);
        public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
        public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
        public void ScheduleFrame(Action<TimeSpan> callback) { }
        public void Show() { }
        public void Resize(Size logicalSize) { }
        public void Close() { }
        public void Dispose() { }
    }

    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
    private static void CheckThrows<T>(Action action, string message) where T : Exception
    {
        try { action(); } catch (T) { return; }
        throw new InvalidOperationException(message);
    }
}
