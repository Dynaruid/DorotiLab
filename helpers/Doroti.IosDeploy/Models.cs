using System.Globalization;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

namespace Doroti.IosDeploy;

public sealed record SampleApp(string Key, string Folder, string BundleId, string ArtifactSlug)
{
    public string Project(string root) => Path.Combine(root, "samples", Folder, "ios", Folder + ".iOS.csproj");
    public static readonly SampleApp[] All =
    [
        new("Sample2", "DorotiSampleApp2", "dev.doroti.sample2", "sample2"),
        new("Testbed", "DorotiTestbedApp", "dev.doroti.testbed", "testbed"),
    ];
}

public sealed record Target(string Kind, string Name, string Udid, string Identifier, string State, string OsVersion)
{
    public string Label => $"[{Kind}] {Name} / iOS {OsVersion} / {State} / {Udid}";
}

public sealed record Identity(string Sha1, string Name)
{
    public string Label => $"{Name} / {Sha1}";
}

public sealed record ProvisionProfile(string Path, Dictionary<string, object?> Data)
{
    public string Uuid => Plist.String(Data, "UUID");
    public string Name => Plist.String(Data, "Name", Uuid);
    public string Label => $"{Name} / {Uuid} / expires " +
        (Data.GetValueOrDefault("ExpirationDate") is DateTimeOffset expiry
            ? expiry.UtcDateTime.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture) : "unknown");

    public bool Matches(SampleApp app, Target target, Identity identity, DateTimeOffset? now = null)
    {
        var entitlements = Plist.Dictionary(Data, "Entitlements");
        if (Data.GetValueOrDefault("ExpirationDate") is not DateTimeOffset expiry || expiry <= (now ?? DateTimeOffset.UtcNow)) return false;
        if (!Plist.Array(Data, "Platform").Contains("iOS") || entitlements.GetValueOrDefault("get-task-allow") is not true) return false;
        if (!Plist.Array(Data, "ProvisionedDevices").OfType<string>().Contains(target.Udid, StringComparer.OrdinalIgnoreCase)) return false;
        if (!Plist.Array(Data, "DeveloperCertificates").OfType<byte[]>()
            .Select(cert => Convert.ToHexString(SHA1.HashData(cert))).Contains(identity.Sha1, StringComparer.OrdinalIgnoreCase)) return false;
        var identifier = Plist.String(entitlements, "application-identifier");
        var separator = identifier.IndexOf('.');
        if (separator < 0 || !Plist.Array(Data, "ApplicationIdentifierPrefix").Contains(identifier[..separator])) return false;
        var pattern = identifier[(separator + 1)..];
        return pattern == app.BundleId || (pattern.EndsWith('*') && app.BundleId.StartsWith(pattern[..^1], StringComparison.Ordinal));
    }
}

public static class Plist
{
    public static Dictionary<string, object?> Parse(string xml)
    {
        // Apple plists declare an external DTD. Ignore it and never fetch it.
        using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null });
        var root = XDocument.Load(reader).Root;
        if (root?.Name != "plist" || root.Elements().SingleOrDefault() is not { } value)
            throw new FormatException("Expected an Apple plist document.");
        return Read(value) as Dictionary<string, object?> ?? throw new FormatException("Expected a plist dictionary.");
    }

    private static object? Read(XElement element)
    {
        switch (element.Name.LocalName)
        {
            case "dict":
                var elements = element.Elements().ToArray();
                if (elements.Length % 2 != 0) throw new FormatException("Unpaired plist dictionary key.");
                var dictionary = new Dictionary<string, object?>(StringComparer.Ordinal);
                for (var i = 0; i < elements.Length; i += 2)
                {
                    if (elements[i].Name != "key") throw new FormatException("Expected a plist dictionary key.");
                    dictionary.Add(elements[i].Value, Read(elements[i + 1]));
                }
                return dictionary;
            case "array": return element.Elements().Select(Read).ToArray();
            case "string": return element.Value;
            case "data": return Convert.FromBase64String(element.Value);
            case "date": return DateTimeOffset.Parse(element.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            case "true": return true;
            case "false": return false;
            case "integer": return long.Parse(element.Value, CultureInfo.InvariantCulture);
            case "real": return double.Parse(element.Value, CultureInfo.InvariantCulture);
            default: throw new FormatException($"Unsupported plist value: {element.Name}.");
        }
    }

    public static string String(Dictionary<string, object?> data, string key, string fallback = "") => data.GetValueOrDefault(key) as string ?? fallback;
    public static object?[] Array(Dictionary<string, object?> data, string key) => data.GetValueOrDefault(key) as object?[] ?? [];
    public static Dictionary<string, object?> Dictionary(Dictionary<string, object?> data, string key) => data.GetValueOrDefault(key) as Dictionary<string, object?> ?? new(StringComparer.Ordinal);
}
