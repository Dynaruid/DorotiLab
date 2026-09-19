using Doroti.Hosting;
using Doroti.Ui;

var factory = new Factory();
var androidIds = new HashSet<long>();
foreach (var device in new[] { int.MinValue, -1, 0, 1, int.MaxValue })
    foreach (var pointerId in new[] { 0, 1, 31 })
        Assert(androidIds.Add(checked((long)Doroti.Host.Maui.AndroidPointerMapping.DeviceIdentifier(device, pointerId))), "Android device/pointer identity must fit signed Flutter IDs and remain unique");
Console.WriteLine("PASS Android virtual/physical device identifiers and multi-pointer isolation");
await using (var rejectedOwner = new PlatformViewCoordinator(3, "test", new([new Factory { Failure = new WebViewException(WebViewError.Unsupported, "provider feature missing") }]), new Dispatcher()))
{
    try { await rejectedOwner.CreateAsync(new PlatformViewRequest(1, "test")); throw new Exception("Expected typed factory rejection."); }
    catch (WebViewException error) when (error.Code == WebViewError.Unsupported) { }
    await rejectedOwner.GetDisposalCompletion(1);
}
Console.WriteLine("PASS typed WebView factory error survives coordinator admission");
await using var a = new PlatformViewCoordinator(1, "test", new([factory]), new Dispatcher());
await using var b = new PlatformViewCoordinator(2, "test", new([new Factory()]), new Dispatcher());
var handle = await a.CreateAsync(new PlatformViewRequest(1, "test"));
var other = await b.CreateAsync(new PlatformViewRequest(1, "test"));
await Reject(() => a.ExecuteWebViewAsync(other, new(WebViewOperation.State)));
var events = 0;
a.WebViewChanged += _ => events++;
factory.Instance!.Send();
Assert(events == 1, "ready event delivered");
var pending = a.ExecuteWebViewAsync(handle, new(WebViewOperation.EvaluateJavaScript));
// An asynchronous command must not hold the placement/disposal semaphore.
await a.AttachAsync(new(handle, Rect.fromLTWH(0, 0, 20, 20), PlatformViewTransform.Identity, null, 0)).AsTask().WaitAsync(TimeSpan.FromSeconds(2));
await a.DisposeAsync(handle).AsTask().WaitAsync(TimeSpan.FromSeconds(2));
await Reject(() => pending);
factory.Instance.Send();
Assert(events == 1, "closed callback filtered");
await Reject(() => a.ExecuteWebViewAsync(handle, new(WebViewOperation.State)));
var replacement = await a.CreateAsync(new PlatformViewRequest(1, "test"));
Assert(replacement.InstanceGeneration > handle.InstanceGeneration, "replacement generation");
await Reject(() => a.ExecuteWebViewAsync(handle, new(WebViewOperation.State)));
Assert((await a.ExecuteWebViewAsync(replacement, new(WebViewOperation.State))).RequestId == 1, "replacement command");
new WebViewOptions(Html: "한글", AllowedOrigins: ["https://example.com"]).Encode();
await Reject(() => Task.FromResult(new WebViewOptions(AllowedOrigins: ["https://example.com/path"]).Encode()));
Console.WriteLine("PASS owner isolation; event lifetime; async command does not block placement/close; pending cancellation; stale generation; options policy");

static void Assert(bool value, string name) { if (!value) throw new Exception(name); }
static async Task Reject(Func<Task> action)
{
    try { await action(); } catch { return; }
    throw new Exception("Expected rejection.");
}
sealed class Dispatcher : IPlatformViewDispatcher
{
    public ValueTask InvokeAsync(Func<ValueTask> action) => action();
}
sealed class Factory : IPlatformViewFactory
{
    public Exception? Failure { get; init; }
    public Instance? Instance;
    public string ViewType => "test";
    public PlatformViewSupport QuerySupport(PlatformViewRequest request) => new("test", "test", "test", true, request.Composition, PlatformViewEffects.RectClip);
    public ValueTask<IPlatformViewInstance> CreateAsync(PlatformViewHandle handle, ReadOnlyMemory<byte> parameters,
        Action<PlatformViewHandle> focused, CancellationToken cancellationToken) => Failure is { } error
            ? ValueTask.FromException<IPlatformViewInstance>(error) : ValueTask.FromResult<IPlatformViewInstance>(Instance = new(handle));
}
sealed class Instance(PlatformViewHandle handle) : IPlatformViewInstance, IPlatformWebViewInstance
{
    private readonly TaskCompletionSource<WebViewResult> _pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public event Action<WebViewEvent>? WebViewChanged;
    public void Send() => WebViewChanged?.Invoke(new(handle, 1, 1, WebViewEventKind.Completed, "about:blank"));
    public Task<WebViewResult> ExecuteAsync(WebViewCommand command, CancellationToken cancellationToken) =>
        command.Operation == WebViewOperation.EvaluateJavaScript ? _pending.Task : Task.FromResult(new WebViewResult(1, 1, 1));
    public ValueTask ApplyAsync(PlatformViewPlacement placement) => ValueTask.CompletedTask;
    public ValueTask DetachAsync() => ValueTask.CompletedTask;
    public ValueTask SetFocusAsync(bool focused) => ValueTask.CompletedTask;
    public ValueTask DisableInputAsync() { _pending.TrySetException(new WebViewException(WebViewError.Closed, "closed")); return ValueTask.CompletedTask; }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
