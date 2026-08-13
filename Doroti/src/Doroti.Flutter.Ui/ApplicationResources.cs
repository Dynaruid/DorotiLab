namespace Doroti.Flutter.Ui;

public sealed record FlutterApplicationResource(
    string Key,
    string Kind,
    string? FontFamily,
    string? Locale,
    string Sha256,
    long Length);

/// <summary>Host-neutral access to compiler-registered application assets, fonts, and localization payloads.</summary>
public interface IApplicationResourceHostCapability
{
    IReadOnlyList<FlutterApplicationResource> Resources { get; }

    ValueTask<ReadOnlyMemory<byte>> LoadAsync(string key, CancellationToken cancellationToken = default);

    FlutterApplicationResource ResolveFont(string family);

    FlutterApplicationResource ResolveLocalization(string locale);
}

/// <summary>Opaque native plugin transport. Dart-owned codecs remain in Flutter Services.</summary>
public interface IPlatformPluginHostCapability
{
    IReadOnlyCollection<string> RegisteredChannels { get; }
}
