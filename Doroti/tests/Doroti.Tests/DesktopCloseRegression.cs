using Doroti.Desktop;
using Doroti.Hosting;
using Doroti.Ui;
using Path = System.IO.Path;

internal static class DesktopCloseRegression
{
    internal static async Task Run()
    {
        var noNativeVeto = new WindowCapabilities((_, _) => WindowEvaluation.Supported, false);
        if (noNativeVeto.Evaluate(new() { RequireNativeCloseCancellation = true }).Support != WindowSupport.Unsupported)
            throw new Exception("Required native close protection must fail before creation.");
        var draftPath = Path.Combine(Path.GetTempPath(), "doroti-draft-" + Guid.NewGuid() + ".json");
        try
        {
            var recovery = new DocumentRecoveryStore(draftPath);
            recovery.Save("dirty 한글 😀");
            if (!recovery.TryRead(out var text) || text != "dirty 한글 😀") throw new Exception("Draft checkpoint lost text.");
            File.WriteAllText(draftPath, "{\"Version\":99,\"Text\":\"obsolete\"}");
            if (recovery.TryRead(out _)) throw new Exception("Unknown recovery version admitted.");
            File.WriteAllText(draftPath, "corrupt");
            if (recovery.TryRead(out _)) throw new Exception("Corrupt draft admitted.");
        }
        finally { File.Delete(draftPath); }
        var host = new Host();
        var manager = new DorotiWindowManager(host);
        var window = await manager.CreateMainWindowAsync(new()
        {
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
        await explicitManager.CreateMainWindowAsync(new() { });
        var exits = await Task.WhenAll(explicitManager.RequestExitAsync(), explicitManager.RequestExitAsync());
        if (exits.Any(result => !result) || !await explicitManager.RequestExitAsync() || exitCount != 1)
            throw new Exception($"Explicit exit must complete once; observed {exitCount} notifications.");
        Console.WriteLine("PASS: repeated/concurrent Explicit exit requests emit one lifetime notification.");
        var context = new RetiringContext();
        var retiringManager = new DorotiWindowManager(new Host
        {
            CloseAction = async () => { await Task.Delay(20).ConfigureAwait(false); context.Retired = true; },
        });
        var retiringWindow = await retiringManager.CreateMainWindowAsync(new()
        {
        });
        var previous = SynchronizationContext.Current;
        Task<bool> retiringClose;
        try { SynchronizationContext.SetSynchronizationContext(context); retiringClose = retiringWindow.CloseAsync(); }
        finally { SynchronizationContext.SetSynchronizationContext(previous); }
        await retiringClose.WaitAsync(TimeSpan.FromSeconds(5));
        if (!retiringWindow.State.Closed || context.StrandedPosts != 0)
            throw new Exception("Close completion depended on its retired view dispatcher.");
        Console.WriteLine("PASS: close registry/lifetime completion survives view dispatcher retirement.");
        var reentrantManager = new DorotiWindowManager(new Host());
        var reentrant = await reentrantManager.CreateMainWindowAsync(new() { });
        using (reentrant.RegisterClosing(async (_, _) => { await reentrant.CloseAsync(); return WindowCloseDecision.Allow; }))
        {
            try { await reentrant.CloseAsync().WaitAsync(TimeSpan.FromSeconds(5)); throw new Exception("Reentrant close callback was admitted."); }
            catch (InvalidOperationException error) when (error.Message.Contains("close callback")) { }
        }
        if (reentrant.State.Closed || !await reentrant.CloseAsync()) throw new Exception("Rejected reentrant close lost its retryable window.");
        Console.WriteLine("PASS: reentrant self-close is rejected before deadlock and the window remains retryable.");
        var startupEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var finishStartup = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var startupManager = new DorotiWindowManager(new Host());
        Task? exitReceipt = null;
        startupManager.ExitRequested += () => exitReceipt = startupManager.WaitForInitializationAsync();
        var startupWindow = await startupManager.CreateMainWindowAsync(new()
        {
            OnCreated = async (owner, _) =>
            {
                await owner.Window.CloseAsync();
                startupEntered.TrySetResult();
                await finishStartup.Task;
            },
        });
        await startupEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        try
        {
            if (exitReceipt is null || exitReceipt.IsCompleted || startupWindow.InitializationWork.IsCompleted)
                throw new Exception("Final application exit bypassed its self-closing startup hook.");
        }
        finally { finishStartup.TrySetResult(); }
        await exitReceipt!.WaitAsync(TimeSpan.FromSeconds(5));
        await startupWindow.InitializationWork;
        Console.WriteLine("PASS: final exit joins closed-window startup receipts without self-close deadlock.");
    }
    private sealed class RetiringContext : SynchronizationContext
    {
        internal volatile bool Retired;
        internal int StrandedPosts;
        public override void Post(SendOrPostCallback callback, object? state)
        {
            if (Retired) { Interlocked.Increment(ref StrandedPosts); return; }
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var previous = Current;
                try { SetSynchronizationContext(this); callback(state); }
                finally { SetSynchronizationContext(previous); }
            });
        }
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
        public Task InitializeAsync(WindowOptions options, DesktopWindowContext context, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<WindowState> ExecuteAsync(WindowCommand command, CancellationToken cancellationToken) => Task.FromResult(State);
        public Task<WindowState> ApplyAppearanceAsync(Doroti.Desktop.WindowAppearanceOptions appearance, CancellationToken cancellationToken) => Task.FromResult(State);
        internal Func<Task>? CloseAction { get; init; }
        public Task CloseAsync(CancellationToken cancellationToken) => CloseAction?.Invoke() ?? Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
