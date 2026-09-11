using System.Text.Json;
using Doroti.Ui;

namespace Doroti.Hosting;

public enum SystemSoundKind
{
    click,
    tick,
    alert,
}

/// <summary>Routes Flutter system sounds to the host without changing haptic feedback.</summary>
public sealed class SystemSoundPlatformMessageCapability(
    IPlatformMessageHostCapability fallback,
    Func<SystemSoundKind, CancellationToken, ValueTask> play) : IPlatformMessageHostCapability
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
                method.ValueKind == JsonValueKind.String && method.GetString() == "SystemSound.play")
            {
                SystemSoundKind? kind = root.TryGetProperty("args", out var args) && args.ValueKind == JsonValueKind.String
                    ? args.GetString() switch
                    {
                        "SystemSoundType.click" => SystemSoundKind.click,
                        "SystemSoundType.tick" => SystemSoundKind.tick,
                        "SystemSoundType.alert" => SystemSoundKind.alert,
                        _ => null,
                    }
                    : null;
                if (kind is null)
                    return "[\"error\",\"Invalid SystemSoundType\",null]"u8.ToArray();

                await play(kind.Value, cancellationToken);
                return "[null]"u8.ToArray();
            }
        }
        return await fallback.SendAsync(channel, data, cancellationToken);
    }

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
        fallback.SetMessageHandler(channel, handler);
}
