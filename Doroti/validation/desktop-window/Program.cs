using Doroti.Desktop;
using Doroti.Hosting;
using Doroti.Ui;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;

var passed = 0;
async Task Check(string name, Func<Task> test)
{
    await test().WaitAsync(TimeSpan.FromSeconds(10));
    Console.WriteLine($"PASS {name}");
    passed++;
}
void Assert(bool value)
{
    if (!value)
        throw new Exception("Assertion failed");
}
async Task Throws<T>(Func<Task> action)
    where T : Exception
{
    try
    {
        await action();
    }
    catch (T)
    {
        return;
    }
    throw new Exception($"Expected {typeof(T).Name}");
}
WindowCreateOptions Request(Func<DesktopWindowContext, CancellationToken, Task>? hook = null) =>
    new()
    {
        Options = new() { StartupVisibility = WindowStartupVisibility.Manual },
        Content = WindowContent.FromEntrypoint(() => new Content()),
        OnCreated = hook,
    };

await Check(
    "invalid options allocate no host",
    async () =>
    {
        var f = new Factory();
        var m = new DorotiWindowManager(f);
        await Throws<ArgumentOutOfRangeException>(() =>
            m.CreateMainWindowAsync(
                Request() with
                {
                    Options = new() { Size = new(double.NaN, 3) },
                }
            )
        );
        Assert(f.Hosts.Count == 0);
        await Throws<ArgumentException>(() =>
            m.CreateMainWindowAsync(
                Request() with
                {
                    Options = new() { MinimumSize = new(1000, 1000) },
                }
            )
        );
    }
);
await Check(
    "single host rejects additional creation before allocation",
    async () =>
    {
        var f = new Factory(single: true);
        var m = new DorotiWindowManager(f);
        var w = await m.CreateMainWindowAsync(Request());
        await Throws<NotSupportedException>(() => m.CreateWindowAsync(Request()));
        Assert(f.Hosts.Count == 1);
        await w.CloseAsync();
        await Throws<NotSupportedException>(() => m.CreateWindowAsync(Request()));
    }
);
await Check(
    "two windows have isolated factories state and scope",
    async () =>
    {
        var f = new Factory();
        var m = new DorotiWindowManager(f);
        var a = await m.CreateMainWindowAsync(Request());
        var b = await m.CreateWindowAsync(Request());
        Assert(a.Id != b.Id && m.MainWindowId == a.Id && m.GetWindows().Count == 2);
        Assert(!ReferenceEquals(f.Hosts[0].Content, f.Hosts[1].Content));
        Assert(
            f.Hosts[0].Context!.Window == a
                && f.Hosts[1].Context!.Window == b
                && f.Hosts[0].Context!.Windows == m
        );
        await a.SetTitleAsync("a");
        Assert(f.Hosts[0].Title == "a" && f.Hosts[1].Title != "a");
        await a.CloseAsync();
        Assert(m.GetWindows().Count == 1 && m.TryGetWindow(b.Id, out var same) && same == b);
        Assert(!m.TryGetWindow(a.Id, out _));
        await Throws<ObjectDisposedException>(() => a.SetTitleAsync("stale"));
        var c = await m.CreateWindowAsync(Request());
        Assert(c.Id != a.Id && m.MainWindowId == a.Id);
    }
);
await Check(
    "readiness hook does not deadlock create",
    async () =>
    {
        var f = new Factory();
        var m = new DorotiWindowManager(f);
        var w = await m.CreateMainWindowAsync(
            Request(
                async (ctx, ct) =>
                {
                    await ctx.Window.WaitUntilReadyToShowAsync(ct);
                    await ctx.Window.ShowAsync(ct);
                }
            )
        );
        Assert(!w.State.Visible);
        f.Hosts[0].Ready.SetResult();
        await w.InitializationWork;
        Assert(w.State.Visible);
        await w.EnsureInitializedAsync();
        await w.EnsureInitializedAsync();
        Assert(f.Hosts[0].Initializations == 1);
    }
);
await Check(
    "close terminates pending ready and show",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        var pending = w.ShowAsync();
        await w.CloseAsync();
        await Throws<ObjectDisposedException>(() => pending);
    }
);
await Check(
    "close cancel and concurrent close share decision",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        var decision = new TaskCompletionSource<WindowCloseDecision>();
        var calls = 0;
        using var subscription = w.RegisterClosing(
            (_, _) =>
            {
                Interlocked.Increment(ref calls);
                return decision.Task;
            }
        );
        var a = w.CloseAsync();
        var b = w.CloseAsync();
        decision.SetResult(WindowCloseDecision.Cancel);
        Assert(!await a && !await b && calls == 1 && !w.State.Closed && f.Hosts[0].Closes == 0);
        subscription.Dispose();
        Assert(await w.CloseAsync());
        Assert(f.Hosts[0].Closes == 1);
    }
);
await Check(
    "native close uses same cancellation callback",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        var called = new TaskCompletionSource();
        using var sub = w.RegisterClosing(
            (_, _) =>
            {
                called.SetResult();
                return Task.FromResult(WindowCloseDecision.Cancel);
            }
        );
        f.Hosts[0].RequestClose();
        await called.Task;
        Assert(!w.State.Closed);
    }
);
await Check(
    "last window policy includes hidden windows",
    async () =>
    {
        var f = new Factory();
        var m = new DorotiWindowManager(f);
        var exits = 0;
        m.ExitRequested += () => exits++;
        var a = await m.CreateMainWindowAsync(Request());
        var b = await m.CreateWindowAsync(Request());
        await a.CloseAsync();
        Assert(exits == 0);
        await b.CloseAsync();
        Assert(exits == 1);
        var explicitManager = new DorotiWindowManager(new Factory(), WindowLifetimePolicy.Explicit);
        explicitManager.ExitRequested += () => throw new Exception("unexpected exit");
        await (await explicitManager.CreateMainWindowAsync(Request())).CloseAsync();
        Assert(explicitManager.GetWindows().Count == 0);
        await explicitManager.CreateWindowAsync(Request());
    }
);
await Check(
    "factory failure rolls back",
    async () =>
    {
        var f = new Factory();
        var m = new DorotiWindowManager(f);
        await Throws<InvalidOperationException>(() =>
            m.CreateMainWindowAsync(
                Request() with
                {
                    Content = WindowContent.FromEntrypoint(() =>
                        throw new InvalidOperationException("factory")
                    ),
                }
            )
        );
        Assert(f.Hosts[0].Disposed && m.GetWindows().Count == 0 && m.MainWindowId is null);
    }
);
await Check(
    "initialization cancellation rolls back",
    async () =>
    {
        var f = new Factory { HoldInitialize = true };
        var m = new DorotiWindowManager(f);
        using var cts = new CancellationTokenSource();
        var task = m.CreateMainWindowAsync(Request(), cts.Token);
        cts.Cancel();
        await Throws<OperationCanceledException>(() => task);
        Assert(f.Hosts[0].Disposed && m.GetWindows().Count == 0);
    }
);
await Check(
    "creation token no longer owns returned window",
    async () =>
    {
        var f = new Factory();
        using var cts = new CancellationTokenSource();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request(), cts.Token);
        cts.Cancel();
        await w.SetTitleAsync("alive");
        Assert(!w.State.Closed);
    }
);
await Check(
    "ready waiter cancellation does not poison others",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        using var cts = new CancellationTokenSource();
        var wait = w.WaitUntilReadyToShowAsync(cts.Token);
        cts.Cancel();
        await Throws<OperationCanceledException>(() => wait);
        f.Hosts[0].Ready.SetResult();
        await w.WaitUntilReadyToShowAsync();
    }
);
await Check(
    "hook failure before ready fails waiters and destroys host",
    async () =>
    {
        var f = new Factory();
        var m = new DorotiWindowManager(f);
        var w = await m.CreateMainWindowAsync(
            Request((_, _) => Task.FromException(new InvalidOperationException("hook")))
        );
        await Throws<InvalidOperationException>(() => w.InitializationWork);
        await Throws<InvalidOperationException>(() => w.WaitUntilReadyToShowAsync());
        Assert(f.Hosts[0].Disposed && m.GetWindows().Count == 0);
    }
);
await Check(
    "hook failure after ready preserves readiness",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(
            Request(
                async (ctx, ct) =>
                {
                    await ctx.Window.WaitUntilReadyToShowAsync(ct);
                    throw new InvalidOperationException("late hook");
                }
            )
        );
        f.Hosts[0].Ready.SetResult();
        await Throws<InvalidOperationException>(() => w.InitializationWork);
        await w.WaitUntilReadyToShowAsync();
        Assert(!w.State.Closed);
    }
);
await Check(
    "appearance updates serialize using latest snapshot",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        var a = w.UpdateAppearanceAsync(x => x with { ThemeSource = WindowThemeSource.Explicit });
        var b = w.UpdateAppearanceAsync(x => x with { Theme = WindowTheme.Dark });
        var results = await Task.WhenAll(a, b);
        Assert(results.All(x => x.Status == WindowApplyStatus.Applied));
        Assert(
            w.State.RequestedAppearance.ThemeSource == WindowThemeSource.Explicit
                && w.State.RequestedAppearance.Theme == WindowTheme.Dark
        );
        Assert(results[0].Revision != results[1].Revision);
    }
);
await Check(
    "queued replacement supersedes only appearance",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        var h = f.Hosts[0];
        h.HoldCommand = new();
        var command = w.SetTitleAsync("keep");
        var a = w.ApplyAppearanceAsync(new() { Theme = WindowTheme.Dark });
        var b = w.ApplyAppearanceAsync(new());
        h.HoldCommand.SetResult();
        await command;
        Assert((await a).Status == WindowApplyStatus.Superseded);
        Assert((await b).Status == WindowApplyStatus.Applied && h.Title == "keep");
    }
);
await Check(
    "failed appearance rolls back and reports error",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        f.Hosts[0].FailAppearance = true;
        var result = await w.ApplyAppearanceAsync(new() { Theme = WindowTheme.Dark });
        Assert(
            result.Status == WindowApplyStatus.Failed
                && w.State.RequestedAppearance.Theme == WindowTheme.Light
        );
    }
);
await Check(
    "subscriptions detach",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        var calls = 0;
        var sub = w.Subscribe(_ => calls++);
        await w.SetTitleAsync("first");
        sub.Dispose();
        var before = calls;
        await w.SetTitleAsync("second");
        Assert(calls == before && before > 0);
    }
);
await Check(
    "reused content and owned windows rejected",
    async () =>
    {
        var f = new Factory();
        var m = new DorotiWindowManager(f);
        var content = new Content();
        var request = Request() with { Content = WindowContent.FromEntrypoint(() => content) };
        var w = await m.CreateMainWindowAsync(request);
        await Throws<InvalidOperationException>(() => m.CreateWindowAsync(request));
        await Throws<NotSupportedException>(() =>
            m.CreateWindowAsync(Request() with { OwnerWindowId = w.Id })
        );
        Assert(m.GetWindows().Count == 1);
    }
);
await Check(
    "closed notification follows host disposal",
    async () =>
    {
        var f = new Factory();
        var m = new DorotiWindowManager(f);
        var w = await m.CreateMainWindowAsync(Request());
        var notified = false;
        m.WindowClosed += _ =>
        {
            Assert(f.Hosts[0].Disposed);
            notified = true;
        };
        await w.CloseAsync();
        Assert(notified);
    }
);
await Check(
    "accepted close cancels deferred commands before drain",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(Request());
        f.Hosts[0].HoldCommand = new();
        var command = w.SetTitleAsync("deferred");
        await w.CloseAsync();
        await Throws<OperationCanceledException>(() => command);
        Assert(w.State.Closed);
    }
);
await Check(
    "automatic startup failure is observed without waiting for a frame",
    async () =>
    {
        var f = new Factory();
        var w = await new DorotiWindowManager(f).CreateMainWindowAsync(
            Request((_, _) => Task.FromException(new InvalidOperationException("auto hook"))) with
            {
                Options = new(),
            }
        );
        await Throws<InvalidOperationException>(() => w.InitializationWork);
        Assert(f.Hosts[0].Disposed);
    }
);
await Check(
    "last close waits for in-flight creation rollback",
    async () =>
    {
        var f = new Factory();
        var manager = new DorotiWindowManager(f);
        var first = await manager.CreateMainWindowAsync(Request());
        var exits = 0;
        manager.ExitRequested += () => exits++;
        f.HoldInitialize = true;
        using var cancellation = new CancellationTokenSource();
        var pending = manager.CreateWindowAsync(Request(), cancellation.Token);
        await first.CloseAsync();
        Assert(exits == 0);
        cancellation.Cancel();
        await Throws<OperationCanceledException>(() => pending);
        Assert(exits == 1 && manager.GetWindows().Count == 0);
    }
);
await Check(
    "native close during creation never enters registry",
    async () =>
    {
        var f = new Factory { CloseDuringInitialize = true };
        var manager = new DorotiWindowManager(f);
        await Throws<ObjectDisposedException>(() => manager.CreateMainWindowAsync(Request()));
        Assert(
            manager.GetWindows().Count == 0 && manager.MainWindowId is null && f.Hosts[0].Disposed
        );
    }
);
await Check(
    "manual native readiness failure destroys the hidden window",
    async () =>
    {
        var f = new Factory();
        var manager = new DorotiWindowManager(f);
        var closed = new TaskCompletionSource();
        manager.WindowClosed += _ => closed.TrySetResult();
        var window = await manager.CreateMainWindowAsync(Request());
        f.Hosts[0].Ready.SetException(new InvalidOperationException("first frame failed"));
        await Throws<InvalidOperationException>(() => window.WaitUntilReadyToShowAsync());
        await closed.Task;
        Assert(f.Hosts[0].Disposed && manager.GetWindows().Count == 0);
    }
);
Console.WriteLine($"{passed}/{passed} contracts passed. Native visual/input evidence is separate.");

sealed class Content : IDorotiViewEntrypoint
{
    public void Bootstrap(PlatformDispatcher dispatcher) { }

    public void Shutdown() { }

    public void AttachView(DorotiView view) { }

    public void DetachView(DorotiView view) { }
}

sealed class Factory(bool single = false) : IWindowHostFactory
{
    public List<Host> Hosts { get; } = [];
    public bool HoldInitialize { get; set; }
    public bool CloseDuringInitialize { get; init; }
    public WindowManagerCapabilities Capabilities => new(!single, single ? 1 : null);

    public WindowEvaluation Evaluate(WindowCreateOptions options) => WindowEvaluation.Supported;

    public ValueTask<IWindowHost> CreateAsync(
        WindowId id,
        WindowCreateOptions options,
        CancellationToken ct
    )
    {
        var h = new Host
        {
            HoldInitialize = HoldInitialize,
            CloseDuringInitialize = CloseDuringInitialize,
        };
        Hosts.Add(h);
        return ValueTask.FromResult<IWindowHost>(h);
    }
}

sealed class Host : IWindowHost
{
    public TaskCompletionSource Ready { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Task ReadyToShow => Ready.Task;
    public TaskCompletionSource? HoldCommand { get; set; }
    public bool HoldInitialize { get; init; }
    public bool CloseDuringInitialize { get; init; }
    public bool FailAppearance { get; set; }
    public bool Disposed { get; private set; }
    public int Initializations { get; private set; }
    public int Closes { get; private set; }
    public string? Title { get; private set; }
    public IDorotiViewEntrypoint? Content { get; private set; }
    public DesktopWindowContext? Context { get; private set; }
    public WindowCapabilities Capabilities { get; } = new((_, _) => WindowEvaluation.Supported);
    public WindowState State { get; private set; } =
        new(
            null,
            new(800, 600),
            1,
            false,
            false,
            WindowPresentationState.Normal,
            new(),
            new(new()),
            new(0, ViewPadding.zero, 0, 0, 0)
        );
    public event Action<WindowState>? StateChanged;
    public event Action? CloseRequested;
    public event Action? Closed;

    public async Task InitializeAsync(
        WindowOptions options,
        DesktopWindowContext context,
        IDorotiViewEntrypoint content,
        CancellationToken ct
    )
    {
        Initializations++;
        Content = content;
        Context = context;
        if (CloseDuringInitialize)
            Closed?.Invoke();
        if (HoldInitialize)
            await Task.Delay(Timeout.Infinite, ct);
    }

    public async Task<WindowState> ExecuteAsync(WindowCommand command, CancellationToken ct)
    {
        if (HoldCommand is { } hold)
            await hold.Task.WaitAsync(ct);
        if (command.Kind == WindowCommandKind.Title)
            Title = (string)command.Value!;
        if (command.Kind == WindowCommandKind.Show)
            State = State with { Visible = true };
        StateChanged?.Invoke(State);
        return State;
    }

    public Task<WindowState> ApplyAppearanceAsync(Appearance appearance, CancellationToken ct)
    {
        if (FailAppearance)
        {
            FailAppearance = false;
            throw new InvalidOperationException("native failure");
        }
        State = State with
        {
            RequestedAppearance = appearance,
            EffectiveAppearance = new(appearance),
        };
        return Task.FromResult(State);
    }

    public void RequestClose() => CloseRequested?.Invoke();

    public Task CloseAsync(CancellationToken ct)
    {
        Closes++;
        Closed?.Invoke();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Disposed = true;
        return ValueTask.CompletedTask;
    }
}
