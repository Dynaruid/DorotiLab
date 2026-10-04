using Doroti.Hosting;
using Doroti.Plugins;
using Doroti.Ui;

static void Require(bool value, string message) { if (!value) throw new Exception(message); }
static DorotiApplicationManifest Manifest(params DorotiApplicationPlugin[] plugins) => new("doroti.application-capabilities/v1", "plugin.test", "win-x64", [], plugins);
static DorotiApplicationPlugin Descriptor(IDorotiNativePluginHandler handler, string channel = NativeFeaturesHandler.Channel) =>
    new(handler.PluginId, channel, "json", DorotiCapabilityIds.PlatformPlugins,
        new("win-x64", "Doroti.Plugins", "0.3.0-beta", handler.AbiVersion, handler.GetType().FullName!));
static DorotiApplicationBoundary Boundary(DorotiApplicationManifest manifest, params IDorotiNativePluginHandler[] handlers) =>
    DorotiApplicationBoundary.Create(manifest, typeof(NativeFeatures).Assembly, handlers);
static NativeFeatures Client(DorotiViewCapabilities capabilities) => new(capabilities.Require<IPlatformMessageHostCapability>(1,
    DorotiCapabilityIds.PlatformMessaging, DorotiUiInvocation.Managed("test")));
static async Task Cancelled(Task task)
{
    try { await task.WaitAsync(TimeSpan.FromSeconds(3)); throw new Exception("Expected cancellation."); }
    catch (OperationCanceledException) { }
}
var handler = new NativeFeaturesHandler();
var descriptor = Descriptor(handler);
// Preserve the existing browser adapter marker instead of treating it as a CLR type.
using (var browser = Boundary(Manifest(descriptor with { NativePackage = descriptor.NativePackage with
    { Rid = "browser-wasm", HandlerType = "generated-js-registration" } }) with { TargetRid = "browser-wasm" }, handler)) { }
using (var first = new DorotiPluginContext(new()))
using (var second = new DorotiPluginContext(new()))
{
    var file = new FakeFile(new());
    var id = first.Retain(file);
    try { second.Require<IPickedFile>(id); throw new Exception("Resource crossed view owners."); }
    catch (InvalidOperationException) { }
    first.Dispose();
    Require(file.Disposed, "View resource was not disposed.");
}
foreach (var (manifest, handlers, expected) in new (DorotiApplicationManifest, IDorotiNativePluginHandler[], string)[]
{
    (Manifest(descriptor), [], "not supplied"),
    (Manifest(descriptor, descriptor), [handler], "duplicate plugin"),
    (Manifest(descriptor), [handler, handler], "duplicate native handler"),
    (Manifest(descriptor with { NativePackage = descriptor.NativePackage with { Rid = "linux-x64" } }), [handler], "RID"),
    (Manifest(descriptor with { NativePackage = descriptor.NativePackage with { AbiVersion = "999" } }), [handler], "ABI"),
    (Manifest(descriptor with { NativePackage = descriptor.NativePackage with { HandlerType = "Wrong.Type" } }), [handler], "handler type"),
    (Manifest(descriptor with { Channel = "flutter/reserved" }), [handler], "reserved"),
})
{
    try { using var rejected = Boundary(manifest, handlers); throw new Exception("Invalid registration accepted."); }
    catch (DorotiCapabilityException error) { Require(error.Message.Contains(expected), error.Message); }
}
using (var boundary = Boundary(Manifest(descriptor), handler))
{
    using var unsupported = new DorotiViewCapabilities();
    boundary.Configure(unsupported);
    var client = Client(unsupported);
    var unsupportedFeatures = await client.GetCapabilitiesAsync();
    Require(!unsupportedFeatures.FilePicker && !unsupportedFeatures.UrlLauncher && unsupportedFeatures.UrlSchemes is { Length: 0 }, "Unsupported capabilities.");
    Require((await client.PickFilesAsync()).Status == FilePickStatus.unsupported, "Unsupported picker.");
    Require((await client.LaunchUrlAsync("bad url")).Status == UrlLaunchStatus.invalidUrl, "Invalid URL.");
    Require((await client.LaunchUrlAsync("file:///C:/Windows/notepad.exe")).Status == UrlLaunchStatus.unsupported, "File launch allowed.");
    Require((await client.LaunchUrlAsync("https://example.com")).Status == UrlLaunchStatus.unsupported, "Unsupported launcher.");
    var picker = new FakePicker();
    var launcher = new FakeLauncher();
    using var view = new DorotiViewCapabilities()
        .Register<IFilePickerHostCapability>(DorotiCapabilityIds.FilePicker, picker)
        .Register<IUrlLauncherHostCapability>(DorotiCapabilityIds.UrlLauncher, launcher);
    boundary.Configure(view);
    client = Client(view);
    var registeredFeatures = await client.GetCapabilitiesAsync();
    Require(registeredFeatures.FilePicker && registeredFeatures.UrlLauncher && registeredFeatures.UrlSchemes is { Length: 3 }, "Registered capabilities.");
    Require((await client.LaunchUrlAsync("https://example.com")).Succeeded && launcher.Calls == 1, "URL routing.");
    launcher.Denied = true;
    Require((await client.LaunchUrlAsync("https://example.com")).Status == UrlLaunchStatus.blocked, "Denied launch.");
    picker.Status = FilePickStatus.cancelled;
    Require((await client.PickFilesAsync()).Status == FilePickStatus.cancelled, "User cancel.");
    picker.Status = FilePickStatus.denied;
    Require((await client.PickFilesAsync()).Status == FilePickStatus.denied, "Permission denied.");
    picker.Status = FilePickStatus.selected;
    await using (var selection = await client.PickFilesAsync(new(true)))
    {
        Require(selection.Files.Count == 2 && selection.Files[0].Length == 5_000_000_000, "Large/multiple files.");
        var bytes = new byte[100000];
        Require(await selection.Files[0].ReadAsync(4_000_000_000, bytes) == 65536 && bytes[0] == 42, "Bounded large offset read.");
    }
    Require(picker.Grants.All(file => file.Disposed), "Selection did not release grants.");
    using var cancelled = new CancellationTokenSource();
    cancelled.Cancel();
    await Cancelled(client.PickFilesAsync(token: cancelled.Token).AsTask());
    var outstanding = await client.PickFilesAsync();
    view.Dispose();
    Require(picker.Grants.All(file => file.Disposed), "Owner close retained grants.");
    await outstanding.DisposeAsync();
}
// A non-cooperative plugin cannot publish a reply after owner shutdown or be
// disposed while its native operation is still using it.
var delayed = new DelayedHandler();
using (var boundary = Boundary(Manifest(Descriptor(delayed, "test/delayed")), delayed))
{
    using var view = new DorotiViewCapabilities();
    boundary.Configure(view);
    var messages = view.Require<IPlatformMessageHostCapability>(1, DorotiCapabilityIds.PlatformMessaging, new("test"));
    var pending = messages.SendAsync("test/delayed", null).AsTask();
    view.Dispose();
    await Cancelled(pending);
    boundary.Dispose();
    Require(!delayed.Disposed.Task.IsCompleted, "Handler disposed during active native operation.");
    delayed.Completion.SetResult();
    await delayed.Disposed.Task.WaitAsync(TimeSpan.FromSeconds(3));
}
// Cancellation during an OS picker must dispose files returned after cancellation.
var sharedHandler = new DelayedHandler();
sharedHandler.Completion.SetResult();
using (var firstWindow = Boundary(Manifest(Descriptor(sharedHandler, "test/delayed")), sharedHandler))
using (var initializingWindow = firstWindow.Retain())
using (var secondWindow = initializingWindow.CreateWindowBoundary([]))
using (var firstView = new DorotiViewCapabilities())
using (var secondView = new DorotiViewCapabilities())
{
    firstWindow.Configure(firstView);
    secondWindow.Configure(secondView);
    firstView.Dispose(); firstWindow.Dispose(); initializingWindow.Dispose();
    Require(!sharedHandler.Disposed.Task.IsCompleted, "First window disposed the surviving window's handler.");
    var messages = secondView.Require<IPlatformMessageHostCapability>(1, DorotiCapabilityIds.PlatformMessaging, new("two-window"));
    Require((await messages.SendAsync("test/delayed", null))?.Span[0] == 1, "Survivor plugin dispatch failed.");
    secondView.Dispose(); secondWindow.Dispose();
    Require(sharedHandler.Disposed.Task.IsCompleted && sharedHandler.DisposeCalls == 1, "Last window did not dispose the handler exactly once.");
}
Console.WriteLine("PASS: shared application handler survives first-window close and disposes once after the last owner.");
var latePicker = new FakePicker { Delay = new(TaskCreationOptions.RunContinuationsAsynchronously) };
using (var boundary = Boundary(Manifest(descriptor), handler))
{
    using var view = new DorotiViewCapabilities().Register<IFilePickerHostCapability>(DorotiCapabilityIds.FilePicker, latePicker);
    boundary.Configure(view);
    var pending = Client(view).PickFilesAsync().AsTask();
    view.Dispose();
    await Cancelled(pending);
    latePicker.Delay.SetResult();
    await latePicker.Released.Task.WaitAsync(TimeSpan.FromSeconds(3));
}
Console.WriteLine("PASS: registration diagnostics; capability/denial/cancel; large bounded reads; grant release; late reply; deferred handler disposal.");

// Event delivery has a bounded queue, immutable payload snapshots and view-owned cancellation.
using (var eventHandler = new EventHandlerFixture())
using (var boundary = Boundary(Manifest(Descriptor(eventHandler, "test/events")), eventHandler))
{
    using var view = new DorotiViewCapabilities();
    boundary.Configure(view);
    var events = view.Require<IPlatformPluginEventsHostCapability>(1, DorotiCapabilityIds.PlatformPluginEvents, new("events"));
    Require(events.EventChannels.SequenceEqual(new[] { "test/events" }), "Event capabilities.");
    await using var reader = events.SubscribeAsync("test/events", capacity: 1).GetAsyncEnumerator();
    Require(await reader.MoveNextAsync() && reader.Current.Span[0] == 0, "First event.");
    await eventHandler.ThirdProduced.Task.WaitAsync(TimeSpan.FromSeconds(3));
    Require(eventHandler.Produced <= 3, "Producer did not backpressure at capacity plus one pending event.");
    Require(reader.Current.Span[0] == 0, "Event reused mutable producer storage.");
    view.Dispose();
    await Cancelled(reader.MoveNextAsync().AsTask());
    await eventHandler.Stopped.Task.WaitAsync(TimeSpan.FromSeconds(3));
}
Console.WriteLine("PASS: event capability, bounded backpressure, payload snapshot, owner cancellation and native unsubscribe.");

sealed class FakePicker : IFilePickerHostCapability
{
    public FilePickStatus Status = FilePickStatus.selected;
    public List<FakeFile> Grants = [];
    public TaskCompletionSource? Delay;
    public TaskCompletionSource Released = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public async ValueTask<FilePickResult> PickFilesAsync(FilePickOptions options, CancellationToken cancellationToken = default)
    {
        if (Delay is not null) await Delay.Task;
        if (Status != FilePickStatus.selected) return new(Status, []);
        var files = Enumerable.Range(0, options.AllowMultiple ? 2 : 1).Select(_ => new FakeFile(Released)).ToArray();
        Grants.AddRange(files);
        return new(Status, files);
    }
}
sealed class FakeFile(TaskCompletionSource released) : IPickedFile
{
    public bool Disposed;
    public string Name => "large.bin";
    public long Length => 5_000_000_000;
    public ValueTask<int> ReadAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default)
    { ObjectDisposedException.ThrowIf(Disposed, this); buffer.Span.Fill(42); return ValueTask.FromResult(buffer.Length); }
    public void Dispose() { Disposed = true; released.TrySetResult(); }
}
sealed class FakeLauncher : IUrlLauncherHostCapability
{
    public int Calls;
    public bool Denied;
    public ValueTask<UrlLaunchResult> LaunchUrlAsync(string url, CancellationToken cancellationToken = default)
    { Calls++; if (Denied) throw new UnauthorizedAccessException("denied"); return ValueTask.FromResult(new UrlLaunchResult(UrlLaunchStatus.opened)); }
}
sealed class DelayedHandler : IDorotiNativePluginHandler, IDisposable
{
    public string PluginId => "delayed";
    public string AbiVersion => "1";
    public TaskCompletionSource Completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource Disposed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int DisposeCalls;
    public async ValueTask<ReadOnlyMemory<byte>?> HandleAsync(string channel, string codec, ReadOnlyMemory<byte>? message, CancellationToken cancellationToken = default)
    { await Completion.Task; return new byte[] { 1 }; }
    public void Dispose() { Interlocked.Increment(ref DisposeCalls); Disposed.TrySetResult(); }
}

sealed class EventHandlerFixture : IDorotiPluginEventHandler, IDisposable
{
    public string PluginId => "events";
    public string AbiVersion => "1";
    public int Produced;
    public TaskCompletionSource ThirdProduced = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource Stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public ValueTask<ReadOnlyMemory<byte>?> HandleAsync(string channel, string codec, ReadOnlyMemory<byte>? message, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public async IAsyncEnumerable<ReadOnlyMemory<byte>> SubscribeAsync(DorotiPluginContext context, string channel, string codec,
        ReadOnlyMemory<byte>? arguments, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var bytes = new byte[1];
        try
        {
            for (var i = 0; i < 10; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                bytes[0] = (byte)i;
                Interlocked.Increment(ref Produced);
                if (Produced == 3) ThirdProduced.TrySetResult();
                yield return bytes;
                await Task.Yield();
            }
        }
        finally { Stopped.TrySetResult(); }
    }
    public void Dispose() { }
}
