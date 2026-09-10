using System.Text.Json;
using Doroti.Ui;

namespace Doroti.Hosting;

public enum HapticFeedbackKind
{
    standard,
    lightImpact,
    mediumImpact,
    heavyImpact,
    selectionClick,
    successNotification,
    warningNotification,
    errorNotification,
}

/// <summary>Connects Flutter's JSON platform channel to a host's tactile feedback API.</summary>
public sealed class HapticFeedbackPlatformMessageCapability(
    IPlatformMessageHostCapability fallback,
    Func<HapticFeedbackKind, CancellationToken, ValueTask> perform) : IPlatformMessageHostCapability
{
    public async ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel == "flutter/platform" && data is { } message)
        {
            using var document = JsonDocument.Parse(message);
            var root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("method", out var method) &&
                method.ValueKind == JsonValueKind.String && method.GetString() == "HapticFeedback.vibrate")
            {
                var argument = root.TryGetProperty("args", out var args) ? args : default;
                var kind = argument.ValueKind switch
                {
                    JsonValueKind.Null or JsonValueKind.Undefined => HapticFeedbackKind.standard,
                    JsonValueKind.String => ParseKind(argument.GetString()),
                    _ => null,
                };
                if (kind is null)
                    return "[\"error\",\"Invalid HapticFeedbackType\",null]"u8.ToArray();

                await perform(kind.Value, cancellationToken);
                // JSONMethodCodec success is a one-element array, not a missing-plugin null response.
                return "[null]"u8.ToArray();
            }
        }
        return await fallback.SendAsync(channel, data, cancellationToken);
    }

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
        fallback.SetMessageHandler(channel, handler);

    private static HapticFeedbackKind? ParseKind(string? value) => value switch
    {
        "HapticFeedbackType.lightImpact" => HapticFeedbackKind.lightImpact,
        "HapticFeedbackType.mediumImpact" => HapticFeedbackKind.mediumImpact,
        "HapticFeedbackType.heavyImpact" => HapticFeedbackKind.heavyImpact,
        "HapticFeedbackType.selectionClick" => HapticFeedbackKind.selectionClick,
        "HapticFeedbackType.successNotification" => HapticFeedbackKind.successNotification,
        "HapticFeedbackType.warningNotification" => HapticFeedbackKind.warningNotification,
        "HapticFeedbackType.errorNotification" => HapticFeedbackKind.errorNotification,
        _ => null,
    };
}
