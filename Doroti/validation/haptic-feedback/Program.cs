using System.Text;
using Doroti.Framework.Services;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;

var calls = new List<HapticFeedbackKind>();
var fallback = new Fallback();
var messages = new HapticFeedbackPlatformMessageCapability(fallback, (kind, _) =>
{
    calls.Add(kind);
    return ValueTask.CompletedTask;
});
var previousChannel = SystemChannels.platform;
try
{
    // Run the real public framework APIs and JSON codec through the production host routing.
    SystemChannels.platform = new OptionalMethodChannel("flutter/platform", new JSONMethodCodec(), new Messenger(messages));
    await HapticFeedback.vibrate();
    await HapticFeedback.lightImpact();
    await HapticFeedback.mediumImpact();
    await HapticFeedback.heavyImpact();
    await HapticFeedback.selectionClick();
    await HapticFeedback.successNotification();
    await HapticFeedback.warningNotification();
    await HapticFeedback.errorNotification();
    Require(calls.SequenceEqual(Enum.GetValues<HapticFeedbackKind>()), "all eight Flutter calls reach the host exactly once and retain their type");
    Require(fallback.Sends == 0, "haptic requests do not fall through to a missing plugin");

    try
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate", "HapticFeedbackType.unknown");
        throw new InvalidOperationException("invalid feedback was accepted");
    }
    catch (PlatformException) { }
    Require(calls.Count == 8, "invalid feedback does not actuate hardware");

    await SystemChannels.platform.invokeMethod<object?>("SystemChrome.restoreSystemUIOverlays");
    Require(fallback.Sends == 1, "other platform methods retain their existing route");
    await messages.SendAsync("custom/channel", "not-json"u8.ToArray());
    Require(fallback.Sends == 2, "custom payloads bypass the JSON parser");
    PlatformMessageHandler handler = (_, _) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    messages.SetMessageHandler("flutter/platform", handler);
    Require(ReferenceEquals(fallback.Handler, handler), "inbound handlers remain delegated");
    messages.SetMessageHandler("flutter/platform", null);
    Require(fallback.Handler is null, "handler removal remains delegated");

    var request = Encoding.UTF8.GetBytes("{\"method\":\"HapticFeedback.vibrate\",\"args\":null}");
    using var cancellation = new CancellationTokenSource();
    cancellation.Cancel();
    try
    {
        await messages.SendAsync("flutter/platform", request, cancellation.Token);
        throw new InvalidOperationException("cancelled feedback was accepted");
    }
    catch (OperationCanceledException) { }
    Require(calls.Count == 8, "cancelled request does not actuate hardware");

    var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var asynchronousHost = new HapticFeedbackPlatformMessageCapability(fallback, (_, _) => new ValueTask(completion.Task));
    var pending = asynchronousHost.SendAsync("flutter/platform", request).AsTask();
    Require(!pending.IsCompleted, "platform response waits for UI-thread dispatch");
    completion.SetResult();
    Require(Encoding.UTF8.GetString((await pending)!.Value.Span) == "[null]", "successful feedback returns a JSON null envelope");

    var unsupportedHost = new HapticFeedbackPlatformMessageCapability(fallback, (_, _) => ValueTask.CompletedTask);
    Require((await unsupportedHost.SendAsync("flutter/platform", request)).HasValue, "unsupported hardware completes as a no-op");
    // Only the browser interop call is replaced; exercise production Flutter duration mapping.
#pragma warning disable CA1416
    foreach (var kind in Enum.GetValues<HapticFeedbackKind>())
        await Doroti.Host.Web.BrowserHapticFeedback.PerformAsync(kind, default);
#pragma warning restore CA1416
    Require(Doroti.Host.Web.BrowserInterop.Durations.SequenceEqual(new[] { 50, 10, 20, 30, 10, 20, 20, 30 }),
        "browser durations match Flutter for all eight feedback types");
    Console.WriteLine("PASS: framework haptics, JSON envelopes, routing, cancellation, asynchronous completion, unsupported hardware and browser durations.");
}
finally
{
    SystemChannels.platform = previousChannel;
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class Fallback : IPlatformMessageHostCapability
{
    public int Sends { get; private set; }
    public PlatformMessageHandler? Handler { get; private set; }
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
    {
        Sends++;
        return ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    }
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) => Handler = handler;
}

sealed class Messenger(IPlatformMessageHostCapability capability) : BinaryMessenger
{
    public async Future<ByteData?> send(string channel, ByteData? message)
    {
        var response = await capability.SendAsync(channel, message is null ? null : (ReadOnlyMemory<byte>)message);
        return response is { } bytes ? (ByteData)bytes : null;
    }
    public Future handlePlatformMessage(string channel, ByteData? data, Action<ByteData?>? callback) => throw new NotSupportedException();
    public void setMessageHandler(string channel, Func<ByteData?, Future<ByteData?>?>? handler) => throw new NotSupportedException();
}
