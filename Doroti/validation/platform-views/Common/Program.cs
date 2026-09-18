using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Framework.Services;

var passed = new List<string>();
await Check("owner isolation and stale generation", async () =>
{
    await using var a = Owner(1); await using var b = Owner(2);
    var x = await a.CreateAsync(new PlatformViewRequest(1, "test"));
    var y = await b.CreateAsync(new PlatformViewRequest(1, "test"));
    Assert(x != y);
    await Reject(() => a.SetFocusAsync(y, true).AsTask());
    await a.DisposeAsync(x);
    var next = await a.CreateAsync(new PlatformViewRequest(1, "test"));
    Assert(next.InstanceGeneration > x.InstanceGeneration);
    await a.DisposeAsync(x); Assert(a.Resolve(1) == next);
});
await Check("pure analysis holds no native resource", async () =>
{
    await using var owner = Owner(3);
    var handle = await owner.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
    using var scene = Scene(3, handle);
    using var plan = PlatformCompositionPlanner.Analyze(scene.Commands, new(3, 0, 1, 0), owner.CaptureSnapshot(PlatformViewComposition.InterleavedComposition));
    Assert(!plan.IsAdmitted);
    await owner.DisposeAsync(handle).AsTask().WaitAsync(TimeSpan.FromSeconds(2));
    await Reject(() => { owner.Admit(plan); return Task.CompletedTask; });
});
await Check("admission retains until retirement", async () =>
{
    await using var owner = Owner(4);
    var handle = await owner.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
    using var scene = Scene(4, handle);
    var plan = PlatformCompositionPlanner.Build(scene, new(4, 0, 1, 0), owner);
    var disposal = owner.DisposeAsync(handle).AsTask();
    Assert(!disposal.IsCompleted);
    plan.Dispose(); plan.Dispose();
    await disposal.WaitAsync(TimeSpan.FromSeconds(2));
});
await Check("R/N/R/N/R order and snapshot immutability", async () =>
{
    await using var owner = Owner(5);
    var a = await owner.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
    var b = await owner.CreateAsync(new PlatformViewRequest(2, "test", PlatformViewComposition.InterleavedComposition));
    var snapshot = owner.CaptureSnapshot(PlatformViewComposition.InterleavedComposition);
    using var scene = Scene(5, a, b);
    using var plan = PlatformCompositionPlanner.Analyze(scene.Commands, new(5, 0, 1, 0), snapshot);
    Assert(string.Join("", plan.Parts.Select(p => p is PlatformNativeSegment ? "N" : "R")) == "RNRNR");
    await owner.DisposeAsync(b); Assert(snapshot.Contains(b));
    await Reject(() => { owner.Admit(plan); return Task.CompletedTask; });
});
await Check("unknown foreground never silently lowered", async () =>
{
    await using var owner = Owner(6);
    var handle = await owner.CreateAsync(new PlatformViewRequest(1, "test"));
    using var scene = Scene(6, handle);
    var commands = scene.Commands.Concat([new SceneCommand("unknown-draw", null)]).ToArray();
    await Reject(() => { using var plan = PlatformCompositionPlanner.Analyze(commands, new(6, 0, 1, 0),
        owner.CaptureSnapshot(PlatformViewComposition.NativeOverlay)); return Task.CompletedTask; });
});
await Check("session rejects old frame and releases failed plans", async () =>
{
    await using var owner = Owner(7);
    var handle = await owner.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
    using var scene = Scene(7, handle);
    var presenter = new Presenter();
    await using var session = new PlatformCompositionSession(7, presenter);
    await session.SubmitAsync(PlatformCompositionPlanner.Build(scene, new(7, 0, 2, 0), owner));
    var stale = PlatformCompositionPlanner.Build(scene, new(7, 0, 1, 0), owner);
    await Reject(() => session.SubmitAsync(stale).AsTask()); Assert(stale.IsDisposed);
    presenter.Fail = true;
    var failed = PlatformCompositionPlanner.Build(scene, new(7, 0, 3, 0), owner);
    await Reject(() => session.SubmitAsync(failed).AsTask()); Assert(failed.IsDisposed);
    Assert(session.LastCommit!.Token.FrameNumber == 2);
});
await Check("post-commit cancellation preserves retirement lease", async () =>
{
    await using var owner = Owner(8);
    var handle = await owner.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
    using var scene = Scene(8, handle);
    var retired = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var presenter = new Presenter { Retirement = retired.Task };
    await using var session = new PlatformCompositionSession(8, presenter);
    using var cancellation = new CancellationTokenSource();
    var plan = PlatformCompositionPlanner.Build(scene, new(8, 0, 1, 0), owner);
    await session.SubmitAsync(plan, cancellation.Token); cancellation.Cancel();
    Assert(!plan.IsDisposed);
    var disposal = owner.DisposeAsync(handle).AsTask(); Assert(!disposal.IsCompleted);
    retired.SetResult(); await disposal.WaitAsync(TimeSpan.FromSeconds(2)); Assert(plan.IsDisposed);
});
await Check("late native creation reclaimed on cancellation", async () =>
{
    var factory = new Factory { Pending = new(TaskCreationOptions.RunContinuationsAsynchronously) };
    await using var owner = new PlatformViewCoordinator(9, "fake", new([factory]), new Dispatcher());
    using var cancellation = new CancellationTokenSource();
    var creation = owner.CreateAsync(new PlatformViewRequest(1, "test"), cancellation.Token).AsTask();
    cancellation.Cancel(); await Reject(() => creation);
    var instance = new Instance(); factory.Pending.SetResult(instance);
    await owner.DisposeAsync(); Assert(instance.Disposed);
});
await Check("failed commit still waits for submitted GPU retirement", async () =>
{
    await using var owner = Owner(10);
    var handle = await owner.CreateAsync(new PlatformViewDescriptor("test"));
    using var scene = Scene(10, handle);
    var retirement = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var presenter = new Presenter { Fail = true, Retirement = retirement.Task };
    await using var session = new PlatformCompositionSession(10, presenter);
    var plan = PlatformCompositionPlanner.Build(scene, new(10, 0, 1, 0), owner);
    await Reject(() => session.SubmitAsync(plan).AsTask());
    Assert(!plan.IsDisposed && session.PendingRetirements == 1);
    var removed = owner.DisposeAsync(handle).AsTask(); Assert(!removed.IsCompleted);
    retirement.SetResult(); await removed.WaitAsync(TimeSpan.FromSeconds(2)); Assert(plan.IsDisposed);
});
await Check("client disposal includes late factory recovery", async () =>
{
    var factory = new Factory { Pending = new(TaskCreationOptions.RunContinuationsAsynchronously) };
    await using var owner = new PlatformViewCoordinator(11, "fake", new([factory]), new Dispatcher());
    var client = new PlatformViewClient(owner, new PlatformViewDescriptor("test"));
    var disposal = client.DisposeAsync().AsTask();
    await Reject(() => client.Ready);
    Assert(!disposal.IsCompleted);
    var instance = new Instance(); factory.Pending.SetResult(instance);
    await disposal.WaitAsync(TimeSpan.FromSeconds(2)); Assert(instance.Disposed);
});
await Check("native effect intent survives retained scene planning", async () =>
{
    await using var owner = Owner(12);
    var handle = await owner.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
    foreach (var match in new[] { PlatformEffectMatchPolicy.MatchCommon, PlatformEffectMatchPolicy.ExactSigma })
    {
        var style = new PlatformEffectStyle(Strength: .75, Match: match, ExactSigma: 12);
        var builder = new SceneBuilder(12);
        builder.addPlatformView(handle, width: 100, height: 100);
        builder.pushClipRect(Rect.fromLTWH(10, 10, 50, 50));
        builder.pushBackdropFilter(new ImageFilter(12, 12) { PlatformEffectIntent = style });
        builder.pop(); builder.pop();
        using var scene = builder.build();
        using var plan = PlatformCompositionPlanner.Build(scene, new(12, 0, 1, 0), owner, effects: new(true, 1, 16));
        var effect = plan.Parts.OfType<PlatformBackdropSegment>().Single();
        Assert(effect.Style == style && effect.Style.Match == match);
        Assert(effect.SampleBounds.left < effect.Bounds.left);
    }
});
await Check("native saturation is negotiated and supports zero-radius color adjustment", async () =>
{
    await using var owner = Owner(13);
    var handle = await owner.CreateAsync(new PlatformViewRequest(1, "test", PlatformViewComposition.InterleavedComposition));
    foreach (var sigma in new[] { 0.0, 8.0 })
    {
        var builder = new SceneBuilder(13);
        builder.addPlatformView(handle, width: 100, height: 100);
        builder.pushClipRect(Rect.fromLTWH(10, 10, 50, 50));
        builder.pushBackdropFilter(new ImageFilter(sigma, sigma) {
            PlatformEffectIntent = new(Match: PlatformEffectMatchPolicy.ExactSigma, ExactSigma: sigma, Saturation: 0) });
        builder.pop(); builder.pop();
        using var scene = builder.build();
        await Reject(() => {
            using var rejected = PlatformCompositionPlanner.Build(scene, new(13, 0, 1, 0), owner, effects: new(true, 1, 64));
            return Task.CompletedTask;
        });
        using var plan = PlatformCompositionPlanner.Build(scene, new(13, 0, 2, 0), owner, effects: new(true, 1, 64, true));
        var effect = plan.Parts.OfType<PlatformBackdropSegment>().Single();
        Assert(effect.SigmaX == sigma && effect.Style?.Saturation == 0);
    }
});
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new { status = "PASS", scope = "common-contract-fixture", tests = passed }));

async Task Check(string name, Func<Task> test) { await test(); passed.Add(name); }
static void Assert(bool value) { if (!value) throw new InvalidOperationException("Contract assertion failed."); }
static async Task Reject(Func<Task> action)
{
    try { await action(); } catch (Exception error) when (error is not TimeoutException) { return; }
    throw new InvalidOperationException("Expected explicit rejection.");
}
static PlatformViewCoordinator Owner(ulong id) => new(id, "fake", new([new Factory()]), new Dispatcher());
static Scene Scene(ulong owner, params PlatformViewHandle[] handles)
{
    var builder = new SceneBuilder(owner);
    foreach (var handle in handles) builder.addPlatformView(handle, width: 100, height: 100);
    return builder.build();
}
sealed class Dispatcher : IPlatformViewDispatcher { public ValueTask InvokeAsync(Func<ValueTask> action) => action(); }
sealed class Factory : IPlatformViewFactory
{
    public TaskCompletionSource<IPlatformViewInstance>? Pending;
    public string ViewType => "test";
    public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new("fake", "fixture", "test", true,
        request.Composition, PlatformViewEffects.RectClip | PlatformViewEffects.AffineTransform);
    public ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> onFocused, CancellationToken cancellationToken) =>
        Pending is { } pending ? new(pending.Task) : ValueTask.FromResult<IPlatformViewInstance>(new Instance());
}
sealed class Instance : IPlatformViewInstance
{
    public bool Disposed;
    public ValueTask ApplyAsync(PlatformViewPlacement placement) => ValueTask.CompletedTask;
    public ValueTask DetachAsync() => ValueTask.CompletedTask;
    public ValueTask DisableInputAsync() => ValueTask.CompletedTask;
    public ValueTask SetFocusAsync(bool focused) => ValueTask.CompletedTask;
    public ValueTask DisposeAsync() { Disposed = true; return ValueTask.CompletedTask; }
}
sealed class Presenter : IPlatformCompositionPresenter
{
    public bool Fail;
    public Task Retirement = Task.CompletedTask;
    public ValueTask<IPreparedPlatformComposition> PrepareAsync(PlatformCompositionPlan plan, CancellationToken cancellationToken) =>
        ValueTask.FromResult<IPreparedPlatformComposition>(new Prepared(this));
    private sealed class Prepared(Presenter owner) : IPreparedPlatformComposition
    {
        public Task Retirement => owner.Retirement;
        public ValueTask CommitAsync(CancellationToken cancellationToken)
        { if (owner.Fail) throw new InvalidOperationException("Injected commit failure"); return ValueTask.CompletedTask; }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
