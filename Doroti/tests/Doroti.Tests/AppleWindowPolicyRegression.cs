using Doroti.Desktop;
using Doroti.Host.Maui;
using Doroti.Ui;

internal static class AppleWindowPolicyRegression
{
    internal static async Task Run()
    {
        foreach (var catalyst in new[] { false, true })
        {
            var factory = new Factory(catalyst);
            var manager = new DorotiWindowManager(factory, WindowLifetimePolicy.Explicit);
            var regular = new WindowRequest(WindowKind.Regular, "primary", new Size(400, 300));
            var primary = await manager.CreateAsync(regular);
            var options = factory.Last!;
            if (options.SkipTaskbar || options.StartupVisibility != (catalyst ? WindowStartupVisibility.PlatformDefault : WindowStartupVisibility.Manual))
                throw new Exception("Logical requests imposed another OS's visibility or Dock policy.");
            var tooltip = new WindowRequest(WindowKind.Tooltip, "tip", new Size(80, 40), primary.Id,
                new(primary.Id, new Rect(10, 20, 30, 40)), Activate: false);
            var support = manager.Evaluate(tooltip);
            if ((support.Availability == WindowAvailability.Supported) == catalyst)
                throw new Exception("Auxiliary support was inferred from the desktop OS instead of the native bridge.");
            var before = factory.Allocations;
            try { await manager.CreateAsync(tooltip with { Owner = new WindowId(Guid.NewGuid()), Anchor = null }); throw new Exception("Invalid owner admitted."); }
            catch (InvalidOperationException) { }
            try { await manager.CreateWindowAsync(new() { Options = factory.MapRequest(tooltip), OwnerWindowId = new WindowId(Guid.NewGuid()) }); throw new Exception("Conflicting owners admitted."); }
            catch (ArgumentException) { }
            if (factory.Allocations != before) throw new Exception("Invalid owner allocated a native window.");
            if (catalyst)
            {
                try { await manager.CreateAsync(tooltip); throw new Exception("Unsupported Catalyst popup allocated."); }
                catch (NotSupportedException) { }
                if (factory.Allocations != before) throw new Exception("Unsupported native request was allocated.");
            }
            else
            {
                await manager.CreateAsync(tooltip);
                if (factory.Last!.SkipTaskbar || factory.Last.Activate || factory.Last.Anchor != tooltip.Anchor)
                    throw new Exception("AppKit tooltip lost its no-activate/owner-relative mapping.");
                if (manager.Evaluate(tooltip with { Activate = true }).Availability != WindowAvailability.Unsupported)
                    throw new Exception("Activating tooltip advertised.");
                await manager.CreateAsync(new(WindowKind.Dialog, "modal", new Size(200, 100), primary.Id, Modal: true));
            }
            await manager.RequestExitAsync();
            var unsupported = catalyst
                ? MacCatalystDesktopWindowPolicy.Evaluate(new() { StartupVisibility = WindowStartupVisibility.Manual }, null)
                : AppKitDesktopWindowPolicy.Evaluate(new() { StartupVisibility = WindowStartupVisibility.Manual, SkipTaskbar = true }, null);
            if (unsupported.Support != WindowSupport.Unsupported) throw new Exception("Explicit unsupported native options were silently rewritten.");
        }
        Console.WriteLine("PASS: AppKit/Catalyst logical mapping, explicit native restrictions and pre-allocation owner/capability rejection (managed policies; native behavior tested separately).");
    }
    private sealed class Factory(bool catalyst) : IWindowHostFactory
    {
        public int Allocations;
        public WindowOptions? Last;
        public WindowManagerCapabilities Capabilities { get; } = new(true, null);
        public WindowOptions MapRequest(WindowRequest request) => catalyst ? MacCatalystDesktopWindowPolicy.MapRequest(request) : AppKitDesktopWindowPolicy.MapRequest(request);
        public WindowEvaluation Evaluate(WindowCreateOptions request) => catalyst ? MacCatalystDesktopWindowPolicy.Evaluate(request.Options, null) : AppKitDesktopWindowPolicy.Evaluate(request.Options, null);
        public ValueTask<IWindowHost> CreateAsync(WindowId id, WindowCreateOptions request, CancellationToken token)
        { token.ThrowIfCancellationRequested(); Allocations++; Last = request.Options; return ValueTask.FromResult<IWindowHost>(new Host()); }
    }
    private sealed class Host : IWindowHost
    {
        public WindowCapabilities Capabilities { get; } = new((_, _) => WindowEvaluation.Supported);
        public WindowState State { get; } = new(null, new Size(400, 300), 1, false, false, WindowPresentationState.Normal, new(), new(new()), new(0, ViewPadding.zero, 0, 0, 0));
        public Task ReadyToShow => Task.CompletedTask;
        public event Action<WindowState>? StateChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        public Task InitializeAsync(WindowOptions options, DesktopWindowContext context, CancellationToken token) => Task.CompletedTask;
        public Task<WindowState> ExecuteAsync(WindowCommand command, CancellationToken token) => Task.FromResult(State);
        public Task<WindowState> ApplyAppearanceAsync(Doroti.Desktop.WindowAppearanceOptions options, CancellationToken token) => Task.FromResult(State);
        public Task CloseAsync(CancellationToken token) => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
