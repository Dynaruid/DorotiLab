using System.Reflection;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

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

    internal async Task<byte[]> ReadAsync(HttpClient http, Uri baseUri, CancellationToken cancellationToken, bool sameOriginOnly = false)
    {
        if (Url is not null)
            {
            var url = new Uri(baseUri, Url);
            if (sameOriginOnly && url.GetLeftPart(UriPartial.Authority) != baseUri.GetLeftPart(UriPartial.Authority))
                throw new InvalidDataException("CSS font URL is outside the application origin.");
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            // A cross-origin redirect must not make an implicit external request.
            // Browser manual redirects are opaque, so strict mode rejects all redirects.
            if (sameOriginOnly && OperatingSystem.IsBrowser()) request.SetBrowserRequestOption("redirect", "error");
            using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            if (response.Content.Headers.ContentLength > BrowserFontData.MaximumBytes)
                throw new InvalidDataException("Font exceeds 30 MB.");
            using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var result = new MemoryStream();
            var chunk = new byte[65536];
            int count;
            while ((count = await source.ReadAsync(chunk, cancellationToken)) != 0)
            {
                if (result.Length + count > BrowserFontData.MaximumBytes) throw new InvalidDataException("Font exceeds 30 MB.");
                result.Write(chunk, 0, count);
            }
            return result.ToArray();
        }
        using var stream = ResourceAssembly!.GetManifestResourceStream(ResourceName!)
            ?? throw new InvalidDataException($"Font resource '{ResourceName}' was not found in {ResourceAssembly.GetName().Name}.");
        using var bytes = new MemoryStream();
        await stream.CopyToAsync(bytes, cancellationToken);
        return bytes.ToArray();
    }
}
