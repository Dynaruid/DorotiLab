using Doroti.Desktop;
using Doroti.Hosting;
using Doroti.Ui;

internal static class DesktopCloseRegression
{
    internal static async Task Run()
    {
        var host = new Host();
        var manager = new DorotiWindowManager(host);
        var window = await manager.CreateMainWindowAsync(new()
        {
            Content = WindowContent.FromEntrypoint(() => new Entrypoint()),
        });
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        manager.WindowClosed += _ => { entered.TrySetResult(); release.Task.GetAwaiter().GetResult(); };
        var first = window.CloseAsync();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Task<bool>? second = null;
        try
        {
            second = window.CloseAsync();
            if (second.IsCompleted) throw new Exception("A second close waiter bypassed pending registry callbacks.");
        }
        finally { release.TrySetResult(); }
        await first.WaitAsync(TimeSpan.FromSeconds(5));
        await second!.WaitAsync(TimeSpan.FromSeconds(5));
        if (manager.GetWindows().Count != 0) throw new Exception("Closed window remained registered.");
        Console.WriteLine("PASS: concurrent close waiters join registry/lifetime completion (fake host contract).");
        var explicitManager = new DorotiWindowManager(new Host(), WindowLifetimePolicy.Explicit);
        var exitCount = 0;
        explicitManager.ExitRequested += () => exitCount++;
        await explicitManager.CreateMainWindowAsync(new() { Content = WindowContent.FromEntrypoint(() => new Entrypoint()) });
        var exits = await Task.WhenAll(explicitManager.RequestExitAsync(), explicitManager.RequestExitAsync());
        if (exits.Any(result => !result) || !await explicitManager.RequestExitAsync() || exitCount != 1)
            throw new Exception($"Explicit exit must complete once; observed {exitCount} notifications.");
        Console.WriteLine("PASS: repeated/concurrent Explicit exit requests emit one lifetime notification.");
    }
    private sealed class Entrypoint : IDorotiViewEntrypoint
    {
        public void Bootstrap(PlatformDispatcher dispatcher) { }
        public void AttachView(DorotiView view) { }
        public void DetachView(DorotiView view) { }
        public void Shutdown() { }
    }
    private sealed class Host : IWindowHost, IWindowHostFactory
    {
        public WindowCapabilities Capabilities { get; } = new((_, _) => WindowEvaluation.Supported);
        WindowManagerCapabilities IWindowHostFactory.Capabilities { get; } = new(false);
        public WindowState State { get; private set; } = new(null, new Size(800, 600), 1, false, false,
            WindowPresentationState.Normal, new(), new(new()), new(0, ViewPadding.zero, 0, 0, 0));
        public Task ReadyToShow => Task.CompletedTask;
        public event Action<WindowState>? StateChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        public WindowEvaluation Evaluate(WindowCreateOptions options) => WindowEvaluation.Supported;
        public ValueTask<IWindowHost> CreateAsync(WindowId id, WindowCreateOptions options, CancellationToken cancellationToken) => ValueTask.FromResult<IWindowHost>(this);
        public Task InitializeAsync(WindowOptions options, DesktopWindowContext context, IDorotiViewEntrypoint content, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<WindowState> ExecuteAsync(WindowCommand command, CancellationToken cancellationToken) => Task.FromResult(State);
        public Task<WindowState> ApplyAppearanceAsync(Doroti.Desktop.WindowAppearanceOptions appearance, CancellationToken cancellationToken) => Task.FromResult(State);
        public Task CloseAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
