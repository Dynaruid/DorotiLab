using System.Text.Json;
using Doroti.Ui;

namespace Doroti.Hosting;

public enum PlatformTextAction { Share, LookUp, SearchWeb }

/// <summary>Routes selection actions shared by editable and selectable text to native UI.</summary>
public sealed class TextActionPlatformMessageCapability(
    IPlatformMessageHostCapability fallback,
    Func<PlatformTextAction, string, CancellationToken, ValueTask> invoke
) : IPlatformMessageHostCapability
{
    public async ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel,
        ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel == "flutter/platform" && data is { } message)
        {
            using var document = JsonDocument.Parse(message);
            var root = document.RootElement;
            PlatformTextAction? action = root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty("method", out var method) && method.ValueKind == JsonValueKind.String
                ? method.GetString() switch
                {
                    "Share.invoke" => PlatformTextAction.Share,
                    "LookUp.invoke" => PlatformTextAction.LookUp,
                    "SearchWeb.invoke" => PlatformTextAction.SearchWeb,
                    _ => null,
                } : null;
            if (action is { } kind)
            {
                if (!root.TryGetProperty("args", out var args) || args.ValueKind != JsonValueKind.String)
                    return Error("invalid-arguments", "Text actions require a string.");
                var text = args.GetString()!;
                if (string.IsNullOrWhiteSpace(text)) return "[null]"u8.ToArray();
                try
                {
                    await invoke(kind, text, cancellationToken);
                    return "[null]"u8.ToArray();
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
                catch (Exception error) { return Error("text-action-failed", error.Message); }
            }
        }
        return await fallback.SendAsync(channel, data, cancellationToken);
    }

    private static ReadOnlyMemory<byte> Error(string code, string message) =>
        JsonSerializer.SerializeToUtf8Bytes(new object?[] { code, message, null });

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
        fallback.SetMessageHandler(channel, handler);
}
