using System.Collections.Concurrent;
using Doroti.Hosting;
using Doroti.Ui;

static class Check
{
    public static void True(bool value, string reason)
    {
        if (!value) throw new InvalidOperationException(reason);
    }
    public static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}.");
    }
    public static async Task ThrowsAsync<T>(Func<Task> action) where T : Exception
    {
        try { await action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}.");
    }
}

sealed class Probe(List<string>? trace = null, string name = "shared") : IDisposable
{
    public int Disposals { get; private set; }
    public void Dispose() { Disposals++; trace?.Add($"dispose:{name}"); }
}

sealed class OwnerDispatcher : IDorotiApplicationDispatcher, IDisposable
{
    private readonly BlockingCollection<Action> _queue = new();
    private readonly Thread _thread;
    private volatile bool _disposed;
    public int ThreadId => _thread.ManagedThreadId;
    public bool HasThreadAccess => Environment.CurrentManagedThreadId == ThreadId;
    public OwnerDispatcher()
    {
        _thread = new Thread(() => { foreach (var action in _queue.GetConsumingEnumerable()) action(); })
            { IsBackground = true, Name = "platform-contract-owner" };
        _thread.Start();
    }
    public ValueTask InvokeAsync(Action callback, CancellationToken cancellationToken = default) =>
        new(InvokeAsync(() => { callback(); return true; }, cancellationToken).AsTask());
    public ValueTask<T> InvokeAsync<T>(Func<T> callback, CancellationToken cancellationToken = default)
    {
        var result = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (_disposed) return ValueTask.FromException<T>(new ObjectDisposedException(nameof(OwnerDispatcher)));
        Action work = () =>
        {
            try { cancellationToken.ThrowIfCancellationRequested(); result.SetResult(callback()); }
            catch (OperationCanceledException error) { result.TrySetCanceled(error.CancellationToken); }
            catch (Exception error) { result.SetException(error); }
        };
        try { _queue.Add(work); }
        catch (InvalidOperationException) when (_disposed)
        { result.TrySetException(new ObjectDisposedException(nameof(OwnerDispatcher))); }
        return new(result.Task);
    }
    public void Dispose() { _disposed = true; _queue.CompleteAdding(); Check.True(_thread.Join(5000), "Owner thread did not stop."); _queue.Dispose(); }
}

sealed class FakeView(ulong id, List<string> trace) : IViewHostCapability, IDorotiPlatformViewLease
{
    public ulong ViewId => id;
    public bool FailDrain { get; set; }
    public bool Drained { get; private set; }
    public bool NativeDisposed { get; private set; }
    public int CapabilityDisposals { get; private set; }
    public ViewMetrics Metrics { get; } = new(new Size(100, 100), 1, ViewPadding.zero,
        ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 1, 1);
    public DorotiViewEpoch ViewEpoch => new(id, 1, 1, 100, 100, 100, 100, 1, 1, 0);
    public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event Action? CloseRequested { add { } remove { } }
    public event Action? Closed { add { } remove { } }
    public void Show() { }
    public void Resize(Size logicalSize) { }
    public void Close() { }
    public void Dispose()
    {
        Check.True(Drained, "A capability was disposed before its native consumer drained.");
        CapabilityDisposals++;
        trace.Add($"capability-dispose:{id}");
    }
    public ValueTask DrainAsync(CancellationToken cancellationToken)
    {
        trace.Add($"drain:{id}");
        if (FailDrain) return ValueTask.FromException(new TimeoutException("Injected GPU drain timeout."));
        Drained = true;
        return ValueTask.CompletedTask;
    }
    public ValueTask DisposeAsync()
    {
        Check.True(Drained, "Native resources released before drain.");
        NativeDisposed = true;
        trace.Add($"native-dispose:{id}");
        return ValueTask.CompletedTask;
    }
}

sealed class FakeFramework(List<string> trace, OwnerDispatcher owner) : IDorotiViewEntrypoint
{
    public HashSet<ulong> Views { get; } = [];
    public PlatformDispatcher? Dispatcher { get; private set; }
    public int Bootstraps { get; private set; }
    public bool FailBootstrap { get; set; }
    public bool FailAttach { get; set; }
    private void AssertOwner() => Check.True(Environment.CurrentManagedThreadId == owner.ThreadId, "Framework escaped its application owner.");
    public void Bootstrap(PlatformDispatcher dispatcher) { AssertOwner(); Dispatcher = dispatcher; Bootstraps++; trace.Add("bootstrap"); if (FailBootstrap) throw new InvalidOperationException("Injected Bootstrap failure."); }
    public void AttachView(DorotiView view) { AssertOwner(); Check.True(Views.Add(view.viewId), "Duplicate attachment."); trace.Add($"attach:{view.viewId}"); if (FailAttach) throw new InvalidOperationException("Injected attachment failure."); }
    public void DetachView(DorotiView view) { AssertOwner(); Views.Remove(view.viewId); trace.Add($"detach:{view.viewId}"); }
    public void Shutdown() { AssertOwner(); trace.Add("framework-shutdown"); }
}

sealed class FakeProvider : IDorotiPlatformProvider, IDorotiPreparedPlatform
{
    public string Id => "test-headless";
    public List<string> Trace { get; } = [];
    public OwnerDispatcher Owner { get; } = new();
    public IDorotiApplicationDispatcher ApplicationDispatcher => Owner;
    public Probe Shared { get; }
    public List<FakeView> NativeViews { get; } = [];
    public List<IDorotiCapabilityRegistrar> Registrars { get; } = [];
    public TaskCompletionSource? PreparationGate { get; set; }
    public TaskCompletionSource PreparationEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource? ViewGate { get; set; }
    public TaskCompletionSource ViewEntered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Action<IDorotiCapabilityRegistrar, ulong>? AdditionalCapabilities { get; set; }
    public bool MissingViewCapability { get; set; }
    public bool FailRegistration { get; set; }
    private bool _sharedTransferred;
    public int ProcessDisposals { get; private set; }
    public FakeFramework Framework { get; }
    public FakeProvider() { Shared = new(Trace); Framework = new(Trace, Owner); }
    public async ValueTask<IDorotiPreparedPlatform> PrepareProcessAsync(DorotiProcessContext context, CancellationToken cancellationToken)
    {
        Trace.Add("prepare");
        PreparationEntered.TrySetResult();
        if (PreparationGate is not null) await PreparationGate.Task;
        return this;
    }
    public void RegisterApplicationCapabilities(DorotiCapabilityLifetime lifetime)
    {
        lifetime.Own(Shared);
        _sharedTransferred = true;
        if (FailRegistration) throw new InvalidOperationException("Injected application registration failure.");
    }
    public async ValueTask<IDorotiPlatformViewLease> CreateViewAsync(DorotiPlatformViewRequest request,
        IDorotiCapabilityRegistrar capabilities, CancellationToken cancellationToken)
    {
        var view = new FakeView((ulong)NativeViews.Count + 1, Trace);
        NativeViews.Add(view);
        Registrars.Add(capabilities);
        if (!MissingViewCapability)
            capabilities.Register(DorotiCapabilityIds.ViewLifecycleMetrics, view, DorotiCapabilityOwnership.Owned);
        capabilities.Register("test.shared", Shared, DorotiCapabilityOwnership.Borrowed);
        AdditionalCapabilities?.Invoke(capabilities, view.ViewId);
        Trace.Add($"register:{view.ViewId}");
        ViewEntered.TrySetResult();
        if (ViewGate is not null) await ViewGate.Task;
        return view;
    }
    public DorotiLaunchPlan Plan(TimeSpan? timeout = null) => new(this,
        DorotiLaunchContext.Create("test", "headless"), () =>
        {
            Check.True(Environment.CurrentManagedThreadId == Owner.ThreadId, "Configure escaped owner.");
            Trace.Add("configure");
            return new(() => { Trace.Add("entrypoint-factory"); return Framework; },
                typeof(FakeProvider).Assembly, typeof(FakeProvider).Assembly,
                new("test", new Size(100, 100)), DorotiLaunchContext.Create("test", "headless"), [], []);
        }, timeout);
    public ValueTask DisposeAsync()
    {
        ProcessDisposals++;
        Trace.Add("process-dispose");
        if (!_sharedTransferred) Shared.Dispose();
        Owner.Dispose();
        return ValueTask.CompletedTask;
    }
}

static class Program
{
    static void Ownership()
    {
        var lifetime = new DorotiCapabilityLifetime();
        var shared = lifetime.Own(new Probe());
        var first = new DorotiViewCapabilities("first", lifetime);
        var second = new DorotiViewCapabilities("second", lifetime);
        first.Register("shared", shared, DorotiCapabilityOwnership.Borrowed);
        second.Register("shared", shared, DorotiCapabilityOwnership.Borrowed);
        Check.Throws<InvalidOperationException>(() => first.Register("owned", shared, DorotiCapabilityOwnership.Owned));
        Check.Throws<InvalidOperationException>(() => first.Register("shared", new Probe()));
        Check.Throws<DorotiCapabilityException>(() => first.Require<string>(1, "shared", new("wrong-type")));
        using (var call = second.Acquire<Probe>(2, "shared", new("typed-call")))
        {
            first.Dispose();
            Check.True(shared.Disposals == 0, "A borrowing view released the application service.");
            Check.Throws<InvalidOperationException>(second.Dispose);
            Check.Throws<InvalidOperationException>(lifetime.Dispose);
        }
        second.Dispose();
        lifetime.Dispose(); lifetime.Dispose();
        Check.True(shared.Disposals == 1, "Shared service was not released exactly once.");
        var late = new DorotiViewCapabilities("late", lifetime);
        Check.Throws<ObjectDisposedException>(() => late.Register("shared", shared, DorotiCapabilityOwnership.Borrowed));
        late.Dispose();
        var exclusiveLifetime = new DorotiCapabilityLifetime();
        var owned = new Probe();
        var a = new DorotiViewCapabilities("a", exclusiveLifetime);
        var b = new DorotiViewCapabilities("b", exclusiveLifetime);
        a.Register("owned", owned, DorotiCapabilityOwnership.Owned);
        Check.Throws<InvalidOperationException>(() => b.Register("owned", owned, DorotiCapabilityOwnership.Owned));
        a.Dispose(); b.Dispose(); exclusiveLifetime.Dispose();
        Check.True(owned.Disposals == 1, "Owned service was not released exactly once.");
    }

    static async Task StagesAndSurvivor()
    {
        var provider = new FakeProvider();
        var plan = provider.Plan();
        Check.True(provider.Trace.Count == 0, "Plan evaluation executed application/provider code.");
        await using var app = new DorotiPlatformApplication(plan);
        await app.StartAsync();
        Check.True(app.State == DorotiPlatformApplicationState.ViewAttached, "First frame was claimed without a submission.");
        Check.Throws<InvalidOperationException>(() => app.StartAsync());
        Check.Throws<InvalidOperationException>(() => provider.Registrars[0].Register("late", new Probe(), DorotiCapabilityOwnership.Owned));
        await app.CreateViewAsync();
        Check.True(provider.Framework.Bootstraps == 1 && provider.Framework.Views.Count == 2, "Views do not share one framework session.");
        await app.CloseViewAsync(1);
        Check.True(provider.Framework.Views.SetEquals([2]), "Closing primary detached its survivor.");
        Check.True(provider.ProcessDisposals == 0 && provider.Shared.Disposals == 0, "Closing primary ended the application.");
        Check.Throws<InvalidOperationException>(() => app.MarkFirstFrameSubmitted(1, 1));
        app.MarkFirstFrameSubmitted(2, 2);
        await app.StopAsync(); await app.StopAsync();
        Check.True(provider.Shared.Disposals == 1 && provider.ProcessDisposals == 1, "Shutdown was not exactly once.");
        Check.True(provider.Trace.IndexOf("prepare") < provider.Trace.IndexOf("configure") &&
            provider.Trace.IndexOf("configure") < provider.Trace.IndexOf("register:1") &&
            provider.Trace.IndexOf("register:1") < provider.Trace.IndexOf("bootstrap"), "Bootstrap ordering failed.");
    }

    static async Task LatePreparation()
    {
        var provider = new FakeProvider { PreparationGate = new(TaskCreationOptions.RunContinuationsAsynchronously) };
        await using var app = new DorotiPlatformApplication(provider.Plan());
        var start = app.StartAsync();
        await provider.PreparationEntered.Task;
        var stop = app.StopAsync();
        provider.PreparationGate.SetResult();
        await Check.ThrowsAsync<OperationCanceledException>(() => start);
        await stop;
        Check.True(!provider.Trace.Contains("configure") && provider.ProcessDisposals == 1,
            "Late process preparation executed Configure or leaked its lease.");
    }

    static async Task LateView()
    {
        var provider = new FakeProvider { ViewGate = new(TaskCreationOptions.RunContinuationsAsynchronously) };
        await using var app = new DorotiPlatformApplication(provider.Plan());
        var start = app.StartAsync();
        await provider.ViewEntered.Task;
        var stop = app.StopAsync();
        provider.ViewGate.SetResult();
        await Check.ThrowsAsync<OperationCanceledException>(() => start);
        await stop;
        Check.True(!provider.Trace.Contains("attach:1") && provider.NativeViews[0].NativeDisposed,
            "Late native allocation was attached or leaked after Stop.");
    }

    static async Task DrainRetry()
    {
        var provider = new FakeProvider();
        await using var app = new DorotiPlatformApplication(provider.Plan());
        await app.StartAsync();
        provider.NativeViews[0].FailDrain = true;
        await Check.ThrowsAsync<DorotiPlatformDrainException>(app.StopAsync);
        Check.True(!provider.NativeViews[0].NativeDisposed && provider.Shared.Disposals == 0 && provider.ProcessDisposals == 0,
            "A failed drain released native/application/process leases.");
        provider.NativeViews[0].FailDrain = false;
        await app.StopAsync();
        Check.True(provider.NativeViews[0].NativeDisposed && provider.Shared.Disposals == 1 && provider.ProcessDisposals == 1,
            "Retry did not reclaim the retained leases.");
    }


    static async Task StartupFailures()
    {
        foreach (var stage in new[] { "configure", "application-registration", "view-registration", "bootstrap", "attach" })
        {
            var provider = new FakeProvider();
            var plan = provider.Plan();
            if (stage == "configure")
                plan = new(provider, plan.LaunchContext, () => throw new InvalidOperationException("Injected Configure failure."));
            provider.FailRegistration = stage == "application-registration";
            provider.MissingViewCapability = stage == "view-registration";
            provider.Framework.FailBootstrap = stage == "bootstrap";
            provider.Framework.FailAttach = stage == "attach";
            await using var app = new DorotiPlatformApplication(plan);
            await Check.ThrowsAsync<InvalidOperationException>(() => app.StartAsync());
            Check.True(app.State == DorotiPlatformApplicationState.Failed, "Startup failure was promoted to running.");
            Check.True(provider.ProcessDisposals == 1 && provider.Shared.Disposals == 1,
                $"{stage} leaked application/process resources.");
            Check.True(provider.Framework.Views.Count == 0 && provider.NativeViews.All(view => view.NativeDisposed),
                $"{stage} leaked a partially attached branch/native lease.");
            if (stage is "bootstrap" or "attach")
                Check.True(provider.Trace.IndexOf("shutdown-framework") < provider.Trace.IndexOf("capability-dispose:1"),
                    "Framework cleanup followed native capability disposal.");
            if (stage == "bootstrap")
                Check.True(provider.Trace.IndexOf("shutdown-framework") < provider.Trace.IndexOf("drain:1"),
                    "Partially bootstrapped framework outlived native drain.");
            await app.StopAsync();
        }
    }

    sealed class Plugin(List<string> trace, string id = "plugin") : IDorotiNativePluginHandler, IDisposable
    {
        public string PluginId => id;
        public string AbiVersion => "test/v1";
        public int Disposals { get; private set; }
        public ValueTask<ReadOnlyMemory<byte>?> HandleAsync(string channel, string codec,
            ReadOnlyMemory<byte>? message, CancellationToken cancellationToken = default) => new(message);
        public void Dispose() { Disposals++; trace.Add("dispose:plugin"); }
    }

    public sealed class Startup : IDorotiApplicationStartup
    {
        public static FakeProvider? Provider { get; set; }
        public void Configure(DorotiApplicationBuilder builder)
        {
            var provider = Provider!;
            Check.True(Environment.CurrentManagedThreadId == provider.Owner.ThreadId, "Startup constructor/configuration escaped owner.");
            provider.Trace.Add("configure");
            builder.UseView(new("test", new Size(100, 100))).UseEntrypoint(() => provider.Framework);
        }
    }

    static async Task PluginFactories()
    {
        var provider = new FakeProvider();
        Startup.Provider = provider;
        Plugin? plugin = null;
        var plan = DorotiLaunchPlan.Create<Startup>(provider, DorotiLaunchContext.Create("test", "headless"),
            [() => { provider.Trace.Add("plugin-factory"); return plugin = new(provider.Trace); }]);
        Check.True(plugin is null && provider.Trace.Count == 0, "Launch plan constructed a plugin before process preparation.");
        await using (var app = new DorotiPlatformApplication(plan))
        {
            await app.StartAsync();
            Check.True(provider.Trace.IndexOf("prepare") < provider.Trace.IndexOf("configure") &&
                provider.Trace.IndexOf("configure") < provider.Trace.IndexOf("plugin-factory"), "Plugin factory bootstrap order failed.");
            await app.CloseViewAsync(1);
            Check.True(plugin!.Disposals == 0, "Closing primary disposed the application plugin.");
            await app.StopAsync();
            Check.True(plugin.Disposals == 1, "Plugin was not disposed exactly once after all native views drained.");
        }
        Startup.Provider = null;
        var trace = new List<string>();
        var first = new Plugin(trace);
        var rejected = new Plugin(trace);
        using (var builder = new DorotiApplicationBuilder(typeof(Startup).Assembly, plan.LaunchContext))
        {
            builder.AddNativePluginHandler(first);
            Check.Throws<InvalidOperationException>(() => builder.AddNativePluginHandlerFactory(() => rejected));
            Check.True(first.Disposals == 0 && rejected.Disposals == 1, "Rejected factory result leaked or disposed existing owner.");
        }
        Check.True(first.Disposals == 1, "Failed Configure did not dispose an acquired plugin.");
    }

    static async Task OwnerDispatch()
    {
        var provider = new FakeProvider();
        await using var app = new DorotiPlatformApplication(provider.Plan());
        await app.StartAsync();
        var view = provider.Framework.Dispatcher!.GetView(1, new("owner-contract"));
        Check.Throws<InvalidOperationException>(() => view.EnterInvocationScope());
        var callbacks = 0;
        var fontsChanged = 0;
        provider.Framework.Dispatcher.FontsChanged += () =>
        {
            Check.True(provider.Owner.HasThreadAccess, "Font invalidation ran outside its application owner.");
            fontsChanged++;
        };
        await provider.Framework.Dispatcher.NotifyFontsChangedAsync();
        Check.True(fontsChanged == 1, "Typed font invalidation did not complete exactly once.");
        await view.DispatchPlatformEventAsync(() =>
        {
            Check.True(provider.Owner.HasThreadAccess, "Native callback ran outside the explicit application owner.");
            Check.True(ReferenceEquals(PlatformDispatcher.instance.RequireInvocationView(new("typed-owner-callback")), view),
                "Typed owner callback selected another view.");
            callbacks++;
        });
        await Check.ThrowsAsync<InvalidOperationException>(() => view.DispatchPlatformEventAsync(
            () => throw new InvalidOperationException("Injected typed callback failure.")).AsTask());
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Check.ThrowsAsync<OperationCanceledException>(() => view.DispatchPlatformEventAsync(() => callbacks++, cancellation.Token).AsTask());
        Check.True(callbacks == 1, "Canceled/failed callback ran an extra completion.");
        var completed = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Task? continuation = null;
        await view.DispatchPlatformEventAsync(() => continuation = ContinueOnOwner());
        completed.SetResult(19);
        await continuation!.WaitAsync(TimeSpan.FromSeconds(5));
        var lateResult = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var lateVoid = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task? rejectedResult = null, rejectedVoid = null;
        await view.DispatchPlatformEventAsync(() =>
        {
            rejectedResult = AwaitLateResult();
            rejectedVoid = AwaitLateVoid();
        });
        await app.CreateViewAsync();
        await app.CloseViewAsync(1);
        await provider.Framework.Dispatcher.DispatchApplicationEventAsync(() =>
        {
            Check.True(provider.Owner.HasThreadAccess, "Application cleanup escaped its owner after the originating view closed.");
            callbacks++;
        });
        lateResult.SetResult(7); lateVoid.SetResult();
        await Check.ThrowsAsync<OperationCanceledException>(() => rejectedResult!.WaitAsync(TimeSpan.FromSeconds(5)));
        await Check.ThrowsAsync<OperationCanceledException>(() => rejectedVoid!.WaitAsync(TimeSpan.FromSeconds(5)));
        await app.StopAsync();
        Check.Throws<ObjectDisposedException>(() => view.DispatchPlatformEventAsync(() => callbacks++));
        async Task ContinueOnOwner()
        {
            Check.True(await Doroti.Runtime.Future<int>.fromTask(completed.Task) == 19, "Future result changed.");
            Check.True(provider.Owner.HasThreadAccess, "Future await continuation escaped the application owner.");
            Check.True(ReferenceEquals(PlatformDispatcher.instance.RequireInvocationView(new("future-owner")), view), "Future continuation lost its view identity.");
        }
        async Task AwaitLateResult() { await Doroti.Runtime.Future<int>.fromTask(lateResult.Task); throw new Exception("Rejected Future result resumed successfully."); }
        async Task AwaitLateVoid() { await Doroti.Runtime.Future.fromTask(lateVoid.Task); throw new Exception("Rejected void Future resumed successfully."); }
    }

    static async Task ReentrantStop()
    {
        var provider = new FakeProvider { PreparationGate = new(TaskCreationOptions.RunContinuationsAsynchronously) };
        await using var app = new DorotiPlatformApplication(provider.Plan());
        Task? reentrantStop = null;
        Task? secondStop = null;
        app.StageChanged += state =>
        {
            if (state == DorotiPlatformApplicationState.PreparingProcess) reentrantStop = app.StopAsync();
            if (state == DorotiPlatformApplicationState.Stopping) secondStop = app.StopAsync();
        };
        var start = app.StartAsync();
        await provider.PreparationEntered.Task;
        Check.True(reentrantStop is not null && !reentrantStop.IsCompleted,
            "Reentrant Stop completed while startup still owned late process preparation.");
        provider.PreparationGate.SetResult();
        await Check.ThrowsAsync<OperationCanceledException>(() => start);
        await reentrantStop!;
        Check.True(ReferenceEquals(reentrantStop, secondStop) && provider.ProcessDisposals == 1 &&
            provider.Shared.Disposals == 1 && !provider.Trace.Contains("configure"),
            "Reentrant Stop created another shutdown or leaked the prepared process.");
    }

    static async Task Main(string[] arguments)
    {
        if (arguments.Length != 0)
        {
            if (arguments.Length == 1 && arguments[0] == "--transport") { await TypedTransportRegression.Run(); return; }
            if (arguments.Length == 1 && arguments[0] == "--frames") { FrameSubmissionRegression.Run(); return; }
            if (arguments.Length == 1 && arguments[0] == "--reattach") { SharedTreeRegression.RunZeroViewReattach(); return; }
            throw new ArgumentException("Unknown contract fixture arguments.");
        }
        Ownership();
        SharedSessionCloseRegression.Run();
        await NativeRouteCoordinatorRegression.Run();
        SharedTreeRegression.Run();
        SharedTreeRegression.RunZeroViewReattach();
        ViewFocusSelectionRegression.Run();
        await StagesAndSurvivor();
        await LatePreparation();
        await LateView();
        await DrainRetry();
        await StartupFailures();
        await PluginFactories();
        await OwnerDispatch();
        await ReentrantStop();
        await TypedTransportRegression.Run();
        Console.WriteLine("PASS: ownership, typed invocation leases, staged startup, shared session, primary/survivor, cancellation, late allocation and drain retry (headless contracts; no native/physical acceptance).");
    }
}
