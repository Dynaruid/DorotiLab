using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Framework.Services;
using Doroti.Skia.Rendering;
using SkiaSharp;

var results = new List<string>();
await Check("legacy-v1-manifest-without-platform-views", () =>
{
    using var boundary = DorotiApplicationBoundary.Load(System.Reflection.Assembly.GetExecutingAssembly(), "win-x64");
    Assert(boundary.Manifest.PlatformViews.Length == 0, "missing optional field must retain an empty registration list");
    Assert(DorotiApplicationBoundary.CreatePlatformViewRegistry(boundary.Manifest, []).ViewTypes.Count == 0, "legacy manifest unexpectedly enabled a factory");
    return Task.CompletedTask;
});
await Check("legacy-codec-wire-format", () =>
{
    var codec = new StandardMethodCodec();
    // Flutter StandardMethodCodec: string x, then an unaligned 32-bit integer.
    var encoded = codec.encodeMethodCall(new MethodCall("x", 7L));
    Assert(encoded.asMemory().Span.SequenceEqual(new byte[] { 7, 1, 120, 3, 7, 0, 0, 0 }), "scalar integer wire format");
    var decoded = codec.decodeMethodCall(encoded);
    Assert(decoded.arguments is long id && id == 7, "Dart integer ID type");
    var message = new StandardMessageCodec();
    foreach (var length in new[] { 253, 254, 65535, 65536 })
    {
        var value = new string('a', length);
        Assert((string)message.decodeMessage(message.encodeMessage(value)) == value, "extended size wire format");
    }
    var bytes = new Doroti.Runtime.Uint8List(new byte[] { 1, 2, 3 });
    Assert(message.decodeMessage(message.encodeMessage(bytes)) is Doroti.Runtime.Uint8List roundtrip && roundtrip.Count == 3, "creation byte parameter type");
    return Task.CompletedTask;
});
await Check("owners-generation-focus-retirement", async () =>
{
    var factory = new FakeFactory();
    await using var a = Owner(1, factory);
    await using var b = Owner(2, factory);
    var first = await a.CreateAsync(new(7, "control"));
    var other = await b.CreateAsync(new(7, "control"));
    Assert(first != other, "owner collision");
    await Throws(() => a.AttachAsync(Placement(other)).AsTask());
    int focusA = 0, focusB = 0;
    a.ViewFocused += _ => focusA++;
    b.ViewFocused += _ => focusB++;
    await a.AttachAsync(Placement(first));
    await b.AttachAsync(Placement(other));
    factory.Focus[first](first);
    Assert(focusA == 1 && focusB == 0, "cross-owner callback");
    await a.DetachAsync(first);
    Assert(a.GetState(first) == PlatformViewState.Detached, "detach state");
    await a.AttachAsync(Placement(first));
    var lease = a.Retain(first);
    var removal = a.DisposeAsync(first).AsTask();
    await factory.Instances[first].InputDisabled.Task;
    Assert(!removal.IsCompleted && factory.Instances[first].Disposals == 0, "released before retirement");
    factory.Focus[first](first);
    Assert(focusA == 1, "removed instance callback");
    await Throws(() => Task.FromResult(a.Resolve(first.InstanceId)));
    lease.Dispose(); lease.Dispose();
    await removal;
    await a.DisposeAsync(first);
    Assert(factory.Instances[first].Disposals == 1, "duplicate disposal");
    var replacement = await a.CreateAsync(new(7, "control"));
    Assert(replacement.InstanceGeneration > first.InstanceGeneration, "generation reuse");
    await a.DisposeAsync(first);
    Assert(a.Resolve(7) == replacement, "stale dispose killed replacement");
    await a.DisposeAsync();
    Assert(a.LiveInstanceCount == 0, "owner leak");
});
await Check("create-cancel-late-success", async () =>
{
    var factory = new FakeFactory { CreationGate = new(TaskCreationOptions.RunContinuationsAsynchronously) };
    await using var owner = Owner(3, factory);
    using var cancellation = new CancellationTokenSource();
    var creation = owner.CreateAsync(new(1, "control"), cancellation.Token).AsTask();
    await factory.Started.Task;
    cancellation.Cancel();
    await Throws(() => creation);
    var close = owner.DisposeAsync().AsTask();
    Assert(!close.IsCompleted, "close ignored in-flight creation");
    factory.CreationGate.SetResult();
    await close;
    Assert(factory.Instances.Values.Single().Disposals == 1 && owner.LiveInstanceCount == 0, "late success leaked");
});
await Check("create-failure-and-missing-factory", async () =>
{
    var factory = new FakeFactory { FailCreation = true };
    await using var owner = Owner(4, factory);
    await Throws(() => owner.CreateAsync(new(1, "control")).AsTask());
    await owner.DisposeAsync();
    Assert(owner.LiveInstanceCount == 0, "failed entry leaked");
    await using var missing = Owner(5, new FakeFactory());
    await Throws(() => missing.CreateAsync(new(1, "missing")).AsTask());
    await Throws(() => missing.CreateAsync(new(1, "control", PlatformViewComposition.ExternalTexture)).AsTask());
    Assert(!DorotiCapabilityIds.RequiredDesktop.Contains(DorotiCapabilityIds.PlatformViews), "capability became mandatory");
});
await Check("legacy-dispose-before-create-reply", async () =>
{
    var factory = new FakeFactory { CreationGate = new(TaskCreationOptions.RunContinuationsAsynchronously) };
    await using var owner = Owner(40, factory);
    using var adapter = new PlatformViewChannelAdapter(owner, new EmptyMessages());
    var codec = new StandardMethodCodec();
    var args = new Doroti.Runtime.DartMap<string, object> { ["id"] = 1L, ["viewType"] = "control" };
    var creation = adapter.SendAsync("flutter/platform_views", codec.encodeMethodCall(new MethodCall("create", args)).asMemory()).AsTask();
    await factory.Started.Task;
    await adapter.SendAsync("flutter/platform_views", codec.encodeMethodCall(new MethodCall("dispose", 1L)).asMemory());
    await Throws(() => creation);
    factory.CreationGate.SetResult();
    await owner.DisposeAsync();
    Assert(owner.LiveInstanceCount == 0 && factory.Instances.Values.Single().Disposals == 1, "legacy late success leaked");
});
await Check("retained-raster-native-raster-order", async () =>
{
    var factory = new FakeFactory();
    await using var owner = Owner(6, factory);
    var a = await owner.CreateAsync(new(1, "control"));
    var b = await owner.CreateAsync(new(2, "control"));
    var builder = new SceneBuilder(6);
    var retained = builder.pushOffset(12, 23);
    builder.pushClipRect(Rect.fromLTWH(0, 0, 100, 100));
    Picture(builder, 0xffff0000);
    builder.addPlatformView(a, width: 80, height: 50);
    Picture(builder, 0xff00ff00);
    builder.addPlatformView(b, width: 80, height: 50);
    Picture(builder, 0xff0000ff);
    builder.pop(); builder.pop();
    var reuse = new SceneBuilder(6);
    reuse.addRetained(retained);
    using var plan = PlatformCompositionPlanner.Build(reuse.build(), new(6, 1, 1, 1), owner);
    Assert(string.Join(",", plan.Parts.Select(p => p is PlatformRasterSegment ? "R" : "N")) == "R,N,R,N,R", "paint order");
    var native = ((PlatformNativeSegment)plan.Parts[1]).Placement;
    Assert(native.Transform.Dx == 12 && native.Transform.Dy == 23 && native.Clip == Rect.fromLTWH(12, 23, 100, 100), "placement transform/clip");
    Assert(!plan.RasterCaptureIncludesNative, "capture lies about native pixels");
    foreach (var raster in plan.Parts.OfType<PlatformRasterSegment>())
    {
        var depth = 0;
        foreach (var command in raster.Commands)
        {
            if (command.Operation is "offset" or "clipRect") depth++;
            if (command.Operation == "pop") depth--;
            Assert(depth >= 0 && command.Operation != "platformView", "invalid segment");
        }
        Assert(depth == 0, "unbalanced segment");
    }
});
await Check("unsupported-effects-zero-view-and-layout", async () =>
{
    await using var owner = Owner(7, new FakeFactory());
    var handle = await owner.CreateAsync(new(1, "control"));
    var builder = new SceneBuilder(7);
    builder.pushOpacity(128);
    builder.addPlatformView(handle, width: 10, height: 10);
    builder.pop();
    await Throws(() => Task.FromResult(PlatformCompositionPlanner.Build(builder.build(), new(7, 0, 1, 0), owner)));
    await Throws(() => owner.AttachAsync(Placement(handle) with { Bounds = Rect.fromLTWH(0, 0, double.PositiveInfinity, 10) }).AsTask());
    var empty = new SceneBuilder(7);
    empty.pushOpacity(128); Picture(empty, 0xff00ff00); empty.pop();
    using var zero = PlatformCompositionPlanner.Build(empty.build(), new(7, 0, 1, 0), owner);
    Assert(zero.Parts.Count == 1 && zero.RasterCaptureIncludesNative, "zero-view path");
    var scaled = new SceneBuilder(7);
    scaled.pushTransform(new double[] { 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 });
    scaled.addPlatformView(handle, new Offset(10, 20), 30, 40); scaled.pop();
    using var scaledPlan = PlatformCompositionPlanner.Build(scaled.build(), new(7, 0, 2, 0, 2, 2), owner);
    Assert(scaledPlan.Parts.OfType<PlatformNativeSegment>().Single().Placement.Transform == PlatformViewTransform.Identity, "native placement applied DPR twice");
});
await Check("frame-rollback-stale-epoch-and-retirement", async () =>
{
    var factory = new FakeFactory();
    await using var owner = Owner(8, factory);
    var handle = await owner.CreateAsync(new(1, "control"));
    var presenter = new FakePresenter();
    await using var session = new PlatformCompositionSession(8, presenter);
    await session.SetEpochAsync(1, 1);
    PlatformCompositionPlan Plan(long epoch, long frame)
    {
        var builder = new SceneBuilder(8);
        builder.addPlatformView(handle, width: 10, height: 10);
        return PlatformCompositionPlanner.Build(builder.build(), new(8, epoch, frame, 1), owner);
    }
    await Throws(() => session.SubmitAsync(Plan(0, 1)).AsTask());
    Assert(presenter.Prepared.Count == 0, "stale plan reached presenter");
    presenter.FailCommit = true;
    await Throws(() => session.SubmitAsync(Plan(1, 1)).AsTask());
    Assert(presenter.Prepared[0].Disposed, "failed prepare leaked");
    presenter.FailCommit = false;
    await session.SubmitAsync(Plan(1, 1));
    await Throws(() => session.SubmitAsync(Plan(1, 1)).AsTask());
    var removal = owner.DisposeAsync(handle).AsTask();
    await factory.Instances[handle].InputDisabled.Task;
    Assert(!removal.IsCompleted, "native freed while committed frame lives");
    presenter.Prepared[1].Retired.SetResult();
    await removal;
});
await Check("100-create-dispose-cycles", async () =>
{
    var factory = new FakeFactory();
    await using var owner = Owner(9, factory);
    for (int i = 0; i < 100; i++)
    {
        var handle = await owner.CreateAsync(new(1, "control"));
        await owner.AttachAsync(Placement(handle));
        await owner.DisposeAsync(handle);
    }
    Assert(owner.LiveInstanceCount == 0 && factory.Instances.Values.All(value => value.Disposals == 1), "lifecycle leak");
});
await Check("legacy-channel-two-owner-callback-routing", async () =>
{
    using var dispatcher = new PlatformDispatcher();
    var factory = new FakeFactory();
    await using var a = Owner(20, factory);
    await using var b = Owner(21, factory);
    using var adapterA = new PlatformViewChannelAdapter(a, new EmptyMessages());
    using var adapterB = new PlatformViewChannelAdapter(b, new EmptyMessages());
    DorotiView Register(ulong id, PlatformViewCoordinator coordinator, PlatformViewChannelAdapter adapter) => dispatcher.RegisterView(id,
        new DorotiViewCapabilities("fake").Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, new FakeViewHost())
            .Register<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews, coordinator)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, adapter));
    var viewA = Register(20, a, adapterA);
    var viewB = Register(21, b, adapterB);
    UiKitViewController first, second;
    int focusA = 0, focusB = 0;
    using (PlatformViewsService.EnterOwner(viewA)) first = await PlatformViewsService.initUiKitView(7, "control", onFocus: () => focusA++);
    using (PlatformViewsService.EnterOwner(viewB)) second = await PlatformViewsService.initUiKitView(7, "control", onFocus: () => focusB++);
    var handleA = a.Resolve(7); var handleB = b.Resolve(7);
    await a.AttachAsync(Placement(handleA)); await b.AttachAsync(Placement(handleB));
    factory.Focus[handleA](handleA); factory.Focus[handleB](handleB);
    Assert(focusA == 1 && focusB == 1, "legacy focus crossed owners");
    using (PlatformViewsService.EnterOwner(viewB)) await first.dispose();
    Assert(a.LiveInstanceCount == 0 && b.Resolve(7) == handleB, "legacy dispose used current owner instead of originating owner");
    await first.dispose(); await second.dispose();
    Assert(b.LiveInstanceCount == 0, "legacy dispose leaked");
});
await Check("manifest-optional-rid-factory-validation", async () =>
{
    var manifest = new DorotiApplicationManifest("doroti.application-capabilities/v1", "test", "win-x64", [], [])
        { PlatformViews = [new("control", "win-x64")] };
    Assert(DorotiApplicationBoundary.CreatePlatformViewRegistry(manifest, [new FakeFactory()]).ViewTypes.Single() == "control", "manifest factory missing");
    await Throws(() => Task.FromResult(DorotiApplicationBoundary.CreatePlatformViewRegistry(manifest, [])));
    await Throws(() => Task.FromResult(DorotiApplicationBoundary.CreatePlatformViewRegistry(manifest with { TargetRid = "android-arm64" }, [new FakeFactory()])));
    Assert(DorotiApplicationBoundary.CreatePlatformViewRegistry(manifest with { PlatformViews = [] }, []).ViewTypes.Count == 0, "optional registry required a factory");
});
await Check("raster-segment-pixels-and-overlay-budget", async () =>
{
    await using var owner = Owner(30, new FakeFactory());
    var a = await owner.CreateAsync(new(1, "control"));
    var b = await owner.CreateAsync(new(2, "control"));
    var builder = new SceneBuilder(30);
    builder.pushOffset(4, 5); Picture(builder, 0xffff0000);
    builder.addPlatformView(a, width: 20, height: 20); Picture(builder, 0xff00ff00);
    builder.addPlatformView(b, width: 20, height: 20); Picture(builder, 0xff0000ff); builder.pop();
    using var plan = PlatformCompositionPlanner.Build(builder.build(), new(30, 0, 1, 0), owner);
    using var renderer = new SkiaSceneRenderer(30, new FakeSkiaHost(), null, null, "fake", "cpu-test", "cpu-test", enablePictureRasterCache: false);
    using var pool = new SkiaPlatformOverlayPool(2, 32 * 32 * 4 * 2, 1);
    var expected = new[] { SKColors.Red, SKColors.Lime, SKColors.Blue };
    int index = 0;
    foreach (var segment in plan.Parts.OfType<PlatformRasterSegment>())
    {
        using var lease = pool.Rent(32, 32, 32 * 32 * 4, () => SKSurface.Create(new SKImageInfo(32, 32, SKColorType.Bgra8888, SKAlphaType.Premul)));
        lease.Surface.Canvas.Clear(SKColors.Transparent);
        renderer.DrawPlatformRasterSegment(lease.Surface.Canvas, segment.Commands, 32, 32);
        using var pixels = lease.Surface.PeekPixels();
        Assert(pixels.GetPixelColor(10, 10) == expected[index++] && pixels.GetPixelColor(1, 1).Alpha == 0, "raster segment pixels/translation");
    }
    Assert(pool.Reuses == 2 && pool.LiveSurfaces == 1, "overlay reuse");
    var first = pool.Rent(32, 32, 4096, () => throw new Exception("expected pool reuse"));
    var second = pool.Rent(32, 32, 4096, () => SKSurface.Create(new SKImageInfo(32, 32)));
    // Keep the rest synchronous: pool and Graphite recorder stay on their render owner thread.
    bool rejected = false;
    try { pool.Rent(32, 32, 4096, () => throw new Exception("must reject before allocation")); }
    catch (InvalidOperationException) { rejected = true; }
    Assert(rejected, "overlay hard cap");
    pool.Invalidate(1);
    Assert(pool.LiveSurfaces == 2, "context loss freed in-flight surfaces");
    first.Dispose(); second.Dispose();
    Assert(pool.LiveSurfaces == 0 && pool.ReservedBytes == 0, "old generation surfaces leaked");
});
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new { schemaVersion = "doroti.platform-views.tests/v1", automated = "passed", tests = results, productLive = "notVerified", physical = "notVerified" }));

async Task Check(string name, Func<Task> test) { await test(); results.Add(name); Console.WriteLine($"PASS {name}"); }
static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
static async Task Throws(Func<Task> action) { try { await action(); } catch { return; } throw new Exception("Expected rejection."); }
static PlatformViewCoordinator Owner(ulong id, FakeFactory factory) => new(id, "fake", new([factory]), new FakeDispatcher());
static PlatformViewPlacement Placement(PlatformViewHandle handle) => new(handle, Rect.fromLTWH(0, 0, 50, 50), PlatformViewTransform.Identity, null, 0);
static void Picture(SceneBuilder builder, uint color)
{
    var recorder = new PictureRecorder();
    var canvas = new Canvas(recorder);
    canvas.drawRect(Rect.fromLTWH(0, 0, 20, 20), new Paint { color = new Color(color) });
    builder.addPicture(Offset.zero, recorder.endRecording());
}
sealed class FakeDispatcher : IPlatformViewDispatcher { public ValueTask InvokeAsync(Func<ValueTask> action) => action(); }
sealed class FakeFactory : IPlatformViewFactory
{
    public string ViewType => "control";
    public bool FailCreation;
    public TaskCompletionSource? CreationGate;
    public TaskCompletionSource Started = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Dictionary<PlatformViewHandle, FakeInstance> Instances = [];
    public Dictionary<PlatformViewHandle, Action<PlatformViewHandle>> Focus = [];
    public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new("fake", "managed", ViewType,
        request.Composition is PlatformViewComposition.NativeOverlay or PlatformViewComposition.InterleavedComposition,
        request.Composition, PlatformViewEffects.RectClip | PlatformViewEffects.AffineTransform);
    public async ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters, Action<PlatformViewHandle> focus, CancellationToken cancellationToken)
    {
        Started.TrySetResult();
        if (CreationGate is not null) await CreationGate.Task;
        if (FailCreation) throw new Exception("injected factory failure");
        Focus.Add(handle, focus);
        var instance = new FakeInstance(); Instances.Add(handle, instance); return instance;
    }
}
sealed class FakeInstance : IPlatformViewInstance
{
    public int Disposals;
    public TaskCompletionSource InputDisabled = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public ValueTask ApplyAsync(PlatformViewPlacement placement) => ValueTask.CompletedTask;
    public ValueTask DetachAsync() => ValueTask.CompletedTask;
    public ValueTask SetFocusAsync(bool focused) => ValueTask.CompletedTask;
    public ValueTask DisableInputAsync() { InputDisabled.TrySetResult(); return ValueTask.CompletedTask; }
    public ValueTask DisposeAsync() { Disposals++; return ValueTask.CompletedTask; }
}
sealed class FakePresenter : IPlatformCompositionPresenter
{
    public bool FailCommit;
    public List<FakePrepared> Prepared = [];
    public ValueTask<IPreparedPlatformComposition> PrepareAsync(PlatformCompositionPlan plan, CancellationToken cancellationToken)
    { var prepared = new FakePrepared(FailCommit); Prepared.Add(prepared); return ValueTask.FromResult<IPreparedPlatformComposition>(prepared); }
}
sealed class FakePrepared(bool fail) : IPreparedPlatformComposition
{
    public bool Disposed;
    public TaskCompletionSource Retired = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Task Retirement => Retired.Task;
    public ValueTask CommitAsync(CancellationToken cancellationToken) => fail ? ValueTask.FromException(new Exception("injected commit failure")) : ValueTask.CompletedTask;
    public ValueTask DisposeAsync() { Disposed = true; return ValueTask.CompletedTask; }
}
sealed class EmptyMessages : IPlatformMessageHostCapability
{
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
}
sealed class FakeViewHost : IViewHostCapability
{
    public ViewMetrics Metrics => new(new Size(500, 500), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 0, 0);
    public DorotiViewEpoch ViewEpoch => throw new NotSupportedException("No frames in this channel test.");
    public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event Action? CloseRequested { add { } remove { } }
    public event Action? Closed { add { } remove { } }
    public void Show() { }
    public void Resize(Size logicalSize) { }
    public void Close() { }
    public void Dispose() { }
}
sealed class FakeSkiaHost : ISkiaSceneRendererHost
{
    public long InputSequence => 0;
    public long SurfaceGeneration => 0;
    public DorotiViewEpoch ViewEpoch => throw new NotSupportedException();
    public DorotiResizeEpoch ResizeTarget => throw new NotSupportedException();
    public PlatformConfiguration Configuration => new([], Brightness.light, false, false);
    public event Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
    public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
    public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
    public void UpdateSemantics(SemanticsUpdate update) { }
    public void ClearSemantics() { }
    public void RequestInvalidate() { }
}
