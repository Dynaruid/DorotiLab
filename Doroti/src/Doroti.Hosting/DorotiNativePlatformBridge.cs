using System.Text.Json;

namespace Doroti.Hosting;

public static class DorotiNativePlatformBridgeContract
{
    public const string PluginId = "doroti.native.platform";
    public const string Channel = "doroti/native-platform";
    public const string AbiVersion = "doroti.native-platform-bridge/v1";
    public const string BridgeVersion = "1.0.0";
}

public sealed record DorotiNativePlatformInfo(
    string Platform,
    string OsVersion,
    string BridgeVersion)
{
    public static DorotiNativePlatformInfo Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var value = JsonSerializer.Deserialize<DorotiNativePlatformInfo>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException("The native platform bridge returned an empty platformInfo payload.");
        if (string.IsNullOrWhiteSpace(value.Platform) ||
            string.IsNullOrWhiteSpace(value.OsVersion) ||
            value.BridgeVersion != DorotiNativePlatformBridgeContract.BridgeVersion)
        {
            throw new InvalidDataException(
                $"The native platform bridge returned an incompatible platformInfo payload: '{json}'.");
        }
        return value;
    }
}

public interface IDorotiNativePlatformBridge : IDorotiNativePluginHandler
{
    DorotiNativePlatformInfo PlatformInfo();

    string Echo(string value);

    ValueTask<string> EchoOnUiThreadAsync(
        string value,
        CancellationToken cancellationToken = default);
}

public abstract class DorotiNativePlatformBridgeBase : IDorotiNativePlatformBridge
{
    public string PluginId => DorotiNativePlatformBridgeContract.PluginId;

    public string AbiVersion => DorotiNativePlatformBridgeContract.AbiVersion;

    public abstract DorotiNativePlatformInfo PlatformInfo();

    public abstract string Echo(string value);

    public abstract ValueTask<string> EchoOnUiThreadAsync(
        string value,
        CancellationToken cancellationToken = default);

    public async ValueTask<ReadOnlyMemory<byte>?> HandleAsync(
        string channel,
        string codec,
        ReadOnlyMemory<byte>? message,
        CancellationToken cancellationToken = default)
    {
        if (channel != DorotiNativePlatformBridgeContract.Channel)
            throw new ArgumentException($"Unsupported native platform bridge channel: {channel}", nameof(channel));
        if (codec != "json")
            throw new ArgumentException($"Unsupported native platform bridge codec: {codec}", nameof(codec));
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        using var request = JsonDocument.Parse(message.Value);
        var method = request.RootElement.GetProperty("method").GetString();
        object response = method switch
        {
            "platformInfo" => PlatformInfo(),
            "echo" => Echo(request.RootElement.GetProperty("value").GetString() ?? string.Empty),
            "echoOnUiThread" => await EchoOnUiThreadAsync(
                request.RootElement.GetProperty("value").GetString() ?? string.Empty,
                cancellationToken).ConfigureAwait(false),
            _ => throw new MissingMethodException($"Unsupported native platform bridge method: {method}"),
        };
        return JsonSerializer.SerializeToUtf8Bytes(response);
    }
}
