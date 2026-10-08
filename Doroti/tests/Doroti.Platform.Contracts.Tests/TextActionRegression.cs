using System.Text.Json;
using Doroti.Framework.Services;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;

internal static class TextActionRegression
{
    internal static async Task Run()
    {
        var fallback = new Fallback();
        var calls = new List<(PlatformTextAction Action, string Text)>();
        var capability = new TextActionPlatformMessageCapability(fallback, (action, text, _) =>
        {
            calls.Add((action, text));
            if (text == "fail") throw new InvalidOperationException("Native presentation failed.");
            return ValueTask.CompletedTask;
        });
        var codec = new JSONMethodCodec();
        foreach (var (method, action) in new[] { ("Share.invoke", PlatformTextAction.Share),
            ("LookUp.invoke", PlatformTextAction.LookUp), ("SearchWeb.invoke", PlatformTextAction.SearchWeb) })
        {
            var reply = await capability.SendAsync("flutter/platform",
                codec.encodeMethodCall(new MethodCall(method, " 선택한 한글 😀 &?\n" )).asMemory());
            codec.decodeEnvelope((ByteData)reply!.Value);
            Check.True(calls[^1] == (action, " 선택한 한글 😀 &?\n"), "Selected text was modified or routed incorrectly.");
        }
        var count = calls.Count;
        await capability.SendAsync("flutter/platform", Message("Share.invoke", " \n"));
        Check.True(calls.Count == count, "Empty text launched native UI.");
        foreach (var argument in new object?[] { null, 123, new object() })
        {
            var reply = await capability.SendAsync("flutter/platform", Message("Share.invoke", argument));
            Check.Throws<PlatformException>(() => codec.decodeEnvelope((ByteData)reply!.Value));
        }
        var failure = await capability.SendAsync("flutter/platform", Message("Share.invoke", "fail"));
        Check.Throws<PlatformException>(() => codec.decodeEnvelope((ByteData)failure!.Value));
        await capability.SendAsync("flutter/platform", Message("Clipboard.getData", "text/plain"));
        await capability.SendAsync("another-channel", Message("Share.invoke", "keep"));
        Check.True(fallback.Calls == 2, "Text routing swallowed an unrelated platform request.");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Check.ThrowsAsync<OperationCanceledException>(() => capability.SendAsync("flutter/platform",
            Message("Share.invoke", "cancel"), cancellation.Token).AsTask());

        var original = SystemChannels.processText;
        try
        {
            var channel = new ProcessChannel();
            SystemChannels.processText = channel;
            var service = new DefaultProcessTextService();
            var actions = await service.queryTextActions();
            Check.True(actions.Count == 1 && actions[0].id == "example/Translate", "Default process text channel was not initialized.");
            Check.True(await service.processTextAction(actions[0].id, "한글", true) == "번역", "Process text response was lost.");
            Check.True(channel.Arguments is List<object> args && (string)args[1] == "한글" && (bool)args[2],
                "Process text lost selection or readOnly.");
            var replacement = new ProcessChannel();
            service.setChannel(replacement);
            await service.queryTextActions();
            Check.True(replacement.Calls == 1, "Release builds ignored setChannel.");
        }
        finally { SystemChannels.processText = original; }
        Console.WriteLine("PASS: native text-action routing, Unicode selection, errors/cancellation, fallback and default ProcessText channel.");
    }

    private static ReadOnlyMemory<byte> Message(string method, object? args) =>
        JsonSerializer.SerializeToUtf8Bytes(new { method, args });

    private sealed class Fallback : IPlatformMessageHostCapability
    {
        internal int Calls;
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default)
        { Calls++; return ValueTask.FromResult<ReadOnlyMemory<byte>?>(null); }
        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    }

    private sealed class ProcessChannel() : MethodChannel("test/process-text")
    {
        internal int Calls;
        internal object? Arguments;
        public override Future<T?> invokeMethod<T>(string method, object? arguments = null) where T : default
        {
            Calls++;
            Arguments = arguments;
            object result = method == "ProcessText.queryTextActions"
                ? new Dictionary<string, string> { ["example/Translate"] = "Translate" } : "번역";
            return Future<T?>.fromTask(Task.FromResult((T?)result));
        }
    }
}
