using System.Text.Json;
using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>Optional system bar settings. Null leaves the native setting unchanged.</summary>
public sealed record SystemUiOverlaySettings(
    uint? StatusBarColor = null,
    Brightness? StatusBarBrightness = null,
    Brightness? StatusBarIconBrightness = null,
    bool? SystemStatusBarContrastEnforced = null,
    uint? SystemNavigationBarColor = null,
    uint? SystemNavigationBarDividerColor = null,
    Brightness? SystemNavigationBarIconBrightness = null,
    bool? SystemNavigationBarContrastEnforced = null)
{
    /// <summary>Retains explicitly set values when a later region omits a property.</summary>
    public SystemUiOverlaySettings Merge(SystemUiOverlaySettings update) => new(
        update.StatusBarColor ?? StatusBarColor,
        update.StatusBarBrightness ?? StatusBarBrightness,
        update.StatusBarIconBrightness ?? StatusBarIconBrightness,
        update.SystemStatusBarContrastEnforced ?? SystemStatusBarContrastEnforced,
        update.SystemNavigationBarColor ?? SystemNavigationBarColor,
        update.SystemNavigationBarDividerColor ?? SystemNavigationBarDividerColor,
        update.SystemNavigationBarIconBrightness ?? SystemNavigationBarIconBrightness,
        update.SystemNavigationBarContrastEnforced ?? SystemNavigationBarContrastEnforced);
}

/// <summary>Routes declarative and imperative framework system bar styles to an owning host.</summary>
public sealed class SystemUiOverlayPlatformMessageCapability(
    IPlatformMessageHostCapability fallback,
    Func<SystemUiOverlaySettings, CancellationToken, ValueTask> apply
) : IPlatformMessageHostCapability
{
    public async ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (channel == "flutter/platform" && data is { } message)
        {
            SystemUiOverlaySettings? settings = null;
            try
            {
                using var document = JsonDocument.Parse(message);
                var root = document.RootElement;
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("method", out var method)
                    && method.ValueKind == JsonValueKind.String
                    && method.GetString() == "SystemChrome.setSystemUIOverlayStyle")
                {
                    if (!root.TryGetProperty("args", out var args) || args.ValueKind != JsonValueKind.Object)
                        throw new FormatException();
                    settings = new(
                        Color(args, "statusBarColor"), BrightnessValue(args, "statusBarBrightness"),
                        BrightnessValue(args, "statusBarIconBrightness"), Boolean(args, "systemStatusBarContrastEnforced"),
                        Color(args, "systemNavigationBarColor"), Color(args, "systemNavigationBarDividerColor"),
                        BrightnessValue(args, "systemNavigationBarIconBrightness"), Boolean(args, "systemNavigationBarContrastEnforced"));
                }
            }
            catch (Exception error) when (error is JsonException or FormatException)
            {
                return "[\"error\",\"Invalid SystemUiOverlayStyle\",null]"u8.ToArray();
            }
            if (settings is not null)
            {
                await apply(settings, cancellationToken);
                return "[null]"u8.ToArray();
            }
        }
        return await fallback.SendAsync(channel, data, cancellationToken);
    }

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
        fallback.SetMessageHandler(channel, handler);

    private static JsonElement? Optional(JsonElement args, string name) =>
        args.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null ? value : null;

    private static uint? Color(JsonElement args, string name) => Optional(args, name) is { } value
        ? value.ValueKind == JsonValueKind.Number && value.TryGetUInt32(out var color) ? color : throw new FormatException()
        : null;

    private static bool? Boolean(JsonElement args, string name) => Optional(args, name) is { } value
        ? value.ValueKind is JsonValueKind.True or JsonValueKind.False ? value.GetBoolean() : throw new FormatException()
        : null;

    private static Brightness? BrightnessValue(JsonElement args, string name) => Optional(args, name) is { } value
        ? value.ValueKind == JsonValueKind.String ? value.GetString() switch
        {
            "dark" or "Brightness.dark" => Brightness.dark,
            "light" or "Brightness.light" => Brightness.light,
            _ => throw new FormatException(),
        } : throw new FormatException()
        : null;
}
