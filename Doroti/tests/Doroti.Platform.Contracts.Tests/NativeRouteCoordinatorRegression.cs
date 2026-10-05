using Doroti.Framework;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;

internal static class NativeRouteCoordinatorRegression
{
    private sealed class FilteredRoute() : RawDialogRoute<string>((_, _, _) => new SizedBox())
    {
        public override ImageFilter? filter => new(sigmaX: 5, sigmaY: 5);
    }
    private sealed class Windows : IWindowService
    {
        internal readonly WindowId Primary = new(Guid.NewGuid());
        internal readonly WindowId Popup = new(Guid.NewGuid());
        internal readonly List<WindowSnapshot> Snapshots = [];
        internal Action? Create;
        internal Action? Close;
        internal int Closes;
        internal int Creates;
        public WindowCapabilityResult Evaluate(WindowRequest request) => WindowCapabilityResult.Supported;
        public IReadOnlyList<WindowSnapshot> GetWindows() => Snapshots.ToArray();
        public event Action<WindowEvent>? Changed;
        public ValueTask<WindowSnapshot> CreateAsync(WindowRequest request, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var value = new WindowSnapshot(Popup, request.Kind, 2, request.Size, 1, false, false);
            Creates++; Snapshots.Add(value); Create!();
            return ValueTask.FromResult(value);
        }
        public ValueTask<WindowSnapshot> ExecuteAsync(WindowActionRequest request, CancellationToken token = default) => ValueTask.FromResult(Snapshots.Single(window => window.Id == request.Window));
        public ValueTask<bool> CloseAsync(WindowId window, CancellationToken token = default)
        {
            if (Snapshots.FirstOrDefault(value => value.Id == window) is { } value)
            {
                Snapshots.Remove(value); Closes++;
                Changed?.Invoke(new(value with { Closed = true })); Close!();
            }
            return ValueTask.FromResult(true);
        }
    }
    private sealed class OwnerContext(OwnerDispatcher owner) : SynchronizationContext
    {
        public override void Post(SendOrPostCallback callback, object? state) => _ = owner.InvokeAsync(() =>
        {
            var previous = Current;
            try { SetSynchronizationContext(this); callback(state); }
            finally { SetSynchronizationContext(previous); }
        }).AsTask().ContinueWith(task => { _ = task.Exception; }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }
    internal static async Task Run()
    {
        using var owner = new OwnerDispatcher();
        var task = await owner.InvokeAsync(() =>
        {
            var previous = SynchronizationContext.Current;
            try { SynchronizationContext.SetSynchronizationContext(new OwnerContext(owner)); return RunOnOwnerAsync(owner); }
            finally { SynchronizationContext.SetSynchronizationContext(previous); }
        });
        await task.WaitAsync(TimeSpan.FromSeconds(10));
    }
    private static async Task RunOnOwnerAsync(OwnerDispatcher owner)
    {
        BuildContext? caller = null;
        BuildContext? native = null;
        var entrypoint = new DorotiWidgetEntrypoint(() => new WidgetsApp(color: new Color(0xffffffff),
            builder: (_, _) => new Navigator(onGenerateInitialRoutes: (_, _) => [new RawDialogRoute<object>(
                pageBuilder: (_, _, _) => new Builder(builder: context => { caller = context; return new SizedBox(); }),
                barrierDismissible: false)])));
        using var session = new DorotiHostSession(entrypoint, applicationDispatcher: owner);
        using var ownerScope = session.dispatcher.EnterScope();
        using var capabilities = new DorotiCapabilityLifetime();
        var windows = capabilities.Own(new Windows());
        windows.Snapshots.Add(new(windows.Primary, WindowKind.Regular, 1, new Size(48, 48), 1, true, false));
        using var first = new SharedTreeRegression.Surface(session.dispatcher, 1, capabilities, windows);
        SharedTreeRegression.Surface? second = null;
        windows.Create = () => { second = new(session.dispatcher, 2, capabilities, windows); session.AttachView(second.View); };
        windows.Close = () => { session.DetachView(second!.View); second.View.Dispose(); };
        session.Start(deferFrameworkBootstrap: true); session.AttachView(first.View);
        first.Frame(TimeSpan.Zero);
        Check.True(caller is not null, "Caller route did not mount.");
        var originalNavigator = Navigator.of(caller!);
        var route = new RawDialogRoute<string>(pageBuilder: (_, _, _) => new Builder(builder: context => { native = context; return new SizedBox(); }), barrierDismissible: true);
        var result = NativeWindowPresentation.ShowPopupRoute(caller!, route, WindowPresentation.Native).asTask();
        try
        {
            for (var step = 1; native is null && step <= 20; step++)
            {
                await Task.Yield();
                first.Frame(TimeSpan.FromMilliseconds(step * 16));
                second!.Frame(TimeSpan.FromMilliseconds(step * 16));
            }
            Check.True(native is not null, "Native popup route did not mount.");
            using (second!.View.EnterInvocationScope())
            {
                var presented = Navigator.of(caller!);
                Check.True(!ReferenceEquals(presented, originalNavigator) && ReferenceEquals(presented, Navigator.of(native!)),
                    "A callback capturing the caller context resolved to the caller's Navigator instead of its native route.");
                presented.pop("selected");
            }
            Check.True(await result.WaitAsync(TimeSpan.FromSeconds(5)) == "selected", "Native route selection lost its result.");
            Check.True(windows.Closes == 1 && ReferenceEquals(Navigator.of(caller!), originalNavigator), "Popup cleanup changed caller navigation or closed twice.");
            var allocations = windows.Creates;
            Check.Throws<NotSupportedException>(() => NativeWindowPresentation.ShowPopupRoute(caller!, new FilteredRoute(), WindowPresentation.Native));
            var filtered = new FilteredRoute();
            var overlay = NativeWindowPresentation.ShowPopupRoute(caller!, filtered, WindowPresentation.Auto).asTask();
            first.Frame(TimeSpan.FromMilliseconds(500));
            Check.True(windows.Creates == allocations && ReferenceEquals(filtered.navigator, originalNavigator),
                "A filtered preview allocated a native window without caller backdrop support.");
            originalNavigator.pop("overlay");
            Check.True(await overlay.WaitAsync(TimeSpan.FromSeconds(5)) == "overlay", "Auto Overlay lost its result.");
            first.Frame(TimeSpan.FromMilliseconds(1000));
            session.ShutdownFramework();
            Console.WriteLine("PASS: native popup routes resolve captured caller navigation in the child view, return one selection and restore caller navigation; filtered previews reject Native and use Auto Overlay before allocation (CPU provider fixture).");
        }
        finally
        {
            if (second is not null) { session.DetachView(second.View); second.Dispose(); }
            session.DetachView(first.View); first.View.Dispose();
            session.ShutdownFramework();
        }
    }
}
