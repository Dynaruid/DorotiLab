using System.Reflection;

namespace Doroti.Host.Web;

/// <summary>A font served by the app, or embedded in an application assembly.</summary>
public sealed class BrowserFontAsset
{
    public BrowserFontAsset(string family, string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(family);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        Family = family;
        Url = url;
    }

    private BrowserFontAsset(string family, Assembly assembly, string resourceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(family);
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);
        Family = family;
        ResourceAssembly = assembly;
        ResourceName = resourceName;
    }

    public string Family { get; }
    public string? Url { get; }
    public Assembly? ResourceAssembly { get; }
    public string? ResourceName { get; }

    public static BrowserFontAsset Embedded(string family, Assembly assembly, string resourceName) =>
        new(family, assembly, resourceName);

    internal async Task<byte[]> ReadAsync(HttpClient http, Uri baseUri, CancellationToken cancellationToken)
    {
        if (Url is not null)
            return await http.GetByteArrayAsync(new Uri(baseUri, Url), cancellationToken);
        using var stream = ResourceAssembly!.GetManifestResourceStream(ResourceName!)
            ?? throw new InvalidDataException($"Font resource '{ResourceName}' was not found in {ResourceAssembly.GetName().Name}.");
        using var bytes = new MemoryStream();
        await stream.CopyToAsync(bytes, cancellationToken);
        return bytes.ToArray();
    }
}
