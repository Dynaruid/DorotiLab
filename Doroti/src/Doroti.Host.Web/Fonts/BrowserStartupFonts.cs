namespace Doroti.Host.Web;

/// <summary>Loads startup fonts before view creation; it never silently falls back to a CDN.</summary>
public static class BrowserStartupFonts
{
    [System.Runtime.Versioning.SupportedOSPlatform("browser")]
    public static Task LoadBrowserAsync(BrowserFontFallbackOptions options, HttpClient http, Uri baseUri,
        Action<ReadOnlyMemory<byte>, string> register, CancellationToken cancellationToken = default) =>
        LoadAsync(options, http, baseUri, register, BrowserWoff2Decoder.DecodeAsync, cancellationToken);

    public static async Task LoadAsync(BrowserFontFallbackOptions options, HttpClient http, Uri baseUri,
        Action<ReadOnlyMemory<byte>, string> register,
        Func<byte[], Uri, CancellationToken, Task<byte[]>>? decodeWoff2 = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(baseUri);
        ArgumentNullException.ThrowIfNull(register);
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(options.DefaultFamily);
        if (options.Enabled && options.DecoderUrl is null)
            throw new ArgumentException("Automatic fallback downloads require a WOFF2 DecoderUrl.", nameof(options));
        var assets = options.Assets.ToArray();
        if ((!options.LoadDefaultFontsFromCdn || !string.Equals(options.DefaultFamily, "Roboto", StringComparison.OrdinalIgnoreCase))
            && !assets.Any(asset => string.Equals(asset.Family, options.DefaultFamily, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException($"No startup asset is registered for default font '{options.DefaultFamily}'.");

        if (options.LoadDefaultFontsFromCdn)
        {
            var defaults = await Task.WhenAll(BrowserDefaultFonts.Paths.Select(path =>
                http.GetByteArrayAsync(new Uri(options.BaseUrl, path), cancellationToken)));
            foreach (var bytes in defaults)
            {
                cancellationToken.ThrowIfCancellationRequested();
                register(bytes, "Roboto");
            }
        }

        // Registration follows declaration order, regardless of download timing.
        var loaded = await Task.WhenAll(assets.Select(async asset =>
        {
            var bytes = await asset.ReadAsync(http, baseUri, cancellationToken);
            if (bytes.AsSpan().StartsWith("wOF2"u8))
            {
                if (options.DecoderUrl is null || decodeWoff2 is null)
                    throw new InvalidDataException("WOFF2 assets need an explicitly configured decoder. Use TTF/OTF for asset-only fonts without a decoder.");
                bytes = await decodeWoff2(bytes, options.DecoderUrl, cancellationToken);
            }
            if (bytes.Length == 0)
                throw new InvalidDataException($"Startup font '{asset.Family}' is empty.");
            return bytes;
        }));
        for (var i = 0; i < loaded.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            register(loaded[i], assets[i].Family);
        }
    }
}
