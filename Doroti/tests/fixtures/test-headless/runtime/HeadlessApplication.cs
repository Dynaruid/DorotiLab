using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.TestProvider.Runtime;

/// <summary>External headless contract fixture; not a native window provider.</summary>
public static class HeadlessApplication
{
    public static async Task RunAsync()
    {
        var provider = new Provider();
        await using var app = new DorotiPlatformApplication(DorotiLaunchPlan.Create<Startup>(provider, new("headless", "test-headless", [])));
        await app.StartAsync();
        await app.CreateViewAsync();
        await app.CloseViewAsync(1);
        await app.StopAsync();
        Console.WriteLine("PASS: external provider PrepareProcess -> Configure -> view registration/Seal -> shared session -> primary close -> survivor -> Stop");
    }
    public sealed class Startup : IDorotiApplicationStartup
    {
        public Startup() => Console.WriteLine("startup-constructor");
        public void Configure(DorotiApplicationBuilder builder)
        {
            Console.WriteLine("Configure");
            builder.UseView(new("Headless contract", new Size(120, 80)));
            builder.UseEntrypoint(() => new Framework());
        }
    }
    private sealed class Framework : IDorotiViewEntrypoint
    {
        public void Bootstrap(PlatformDispatcher dispatcher) => Console.WriteLine("Bootstrap");
        public void AttachView(DorotiView view) => Console.WriteLine($"Attach:{view.viewId}");
        public void DetachView(DorotiView view) => Console.WriteLine($"Detach:{view.viewId}");
        public void Shutdown() => Console.WriteLine("Shutdown");
    }
    private sealed class Provider : IDorotiPlatformProvider
    {
        public string Id => "test-headless";
        public ValueTask<IDorotiPreparedPlatform> PrepareProcessAsync(DorotiProcessContext context, CancellationToken token)
        { token.ThrowIfCancellationRequested(); Console.WriteLine("PrepareProcess"); return ValueTask.FromResult<IDorotiPreparedPlatform>(new Prepared()); }
    }
    private sealed class Owner : IDorotiApplicationDispatcher
    {
        // This fixture has no OS threads; all framework work is explicitly invoked here.
        public bool HasThreadAccess => true;
        public ValueTask InvokeAsync(Action callback, CancellationToken token = default) { token.ThrowIfCancellationRequested(); callback(); return ValueTask.CompletedTask; }
        public ValueTask<T> InvokeAsync<T>(Func<T> callback, CancellationToken token = default) { token.ThrowIfCancellationRequested(); return ValueTask.FromResult(callback()); }
    }
    private sealed class Prepared : IDorotiPreparedPlatform
    {
        private ulong _id;
        public IDorotiApplicationDispatcher ApplicationDispatcher { get; } = new Owner();
        public void RegisterApplicationCapabilities(DorotiCapabilityLifetime lifetime) { }
        public ValueTask<IDorotiPlatformViewLease> CreateViewAsync(DorotiPlatformViewRequest request, IDorotiCapabilityRegistrar registrar, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var view = new HeadlessView(++_id);
            registrar.Register<IViewHostCapability>(DorotiCapabilityIds.WindowLifecycle, view, DorotiCapabilityOwnership.Owned);
            registrar.Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, view, DorotiCapabilityOwnership.Owned);
            return ValueTask.FromResult<IDorotiPlatformViewLease>(view);
        }
        public ValueTask DisposeAsync() { Console.WriteLine("process-dispose"); return ValueTask.CompletedTask; }
    }
    private sealed class HeadlessView(ulong id) : IViewHostCapability, IDorotiPlatformViewLease
    {
        public ulong ViewId => id;
        public ViewMetrics Metrics { get; } = new(new Size(120, 80), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 1, 1);
        public DorotiViewEpoch ViewEpoch => new(id, 1, 1, 120, 80, 120, 80, 1, 1, 0);
        public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
        public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        public void Show() { } public void Resize(Size size) { } public void Close() { } public void Dispose() => Console.WriteLine($"capability-dispose:{id}");
        public ValueTask DrainAsync(CancellationToken token) { token.ThrowIfCancellationRequested(); Console.WriteLine($"drain:{id}"); return ValueTask.CompletedTask; }
        public ValueTask DisposeAsync() { Console.WriteLine($"native-dispose:{id}"); return ValueTask.CompletedTask; }
    }
}
