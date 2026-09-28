using Doroti.Skia.Rendering;

namespace Doroti.Host.Web;

/// <summary>Loads startup fonts before view creation.</summary>
public static class BrowserStartupFonts
{
    [System.Runtime.Versioning.SupportedOSPlatform("browser")]
    public static async Task LoadBrowserAsync(BrowserFontFallbackOptions options, HttpClient http, Uri baseUri,
        Action<ReadOnlyMemory<byte>, string> register, CancellationToken cancellationToken = default,
        Action<ReadOnlyMemory<byte>, string, SkiaFontFaceDescriptor>? registerFace = null)
    {
        var faces = await BrowserCssFonts.DiscoverAsync(options);
        await LoadAsync(options, http, baseUri, register, BrowserWoff2Decoder.DecodeAsync, cancellationToken, faces, registerFace);
    }

    public static async Task LoadAsync(BrowserFontFallbackOptions options, HttpClient http, Uri baseUri,
        Action<ReadOnlyMemory<byte>, string> register,
        Func<byte[], Uri, CancellationToken, Task<byte[]>>? decodeWoff2 = null,
        CancellationToken cancellationToken = default,
        IReadOnlyList<BrowserCssFontFace>? cssFaces = null,
        Action<ReadOnlyMemory<byte>, string, SkiaFontFaceDescriptor>? registerFace = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(baseUri);
        ArgumentNullException.ThrowIfNull(register);
        cancellationToken.ThrowIfCancellationRequested();
        if (options.DecoderUrl is { IsAbsoluteUri: false }) options = options with { DecoderUrl = new Uri(baseUri, options.DecoderUrl) };
        ArgumentException.ThrowIfNullOrWhiteSpace(options.DefaultFamily);
        if (options.Enabled && options.DecoderUrl is null)
            throw new ArgumentException("Automatic fallback downloads require a WOFF2 DecoderUrl.", nameof(options));
        if (options.DownloadTimeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(options.DownloadTimeout));
        var assets = options.Assets.ToArray();
        cssFaces ??= [];
        if ((!options.LoadDefaultFontsFromCdn || !string.Equals(options.DefaultFamily, "Roboto", StringComparison.OrdinalIgnoreCase))
            && !assets.Any(a => string.Equals(a.Family, options.DefaultFamily, StringComparison.OrdinalIgnoreCase))
            && !cssFaces.Any(a => string.Equals(a.Family, options.DefaultFamily, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException($"No startup asset is registered for default font '{options.DefaultFamily}'.");
        var languageAssets = BrowserLanguageFonts.Resolve(options);
        if (languageAssets.Length > 0 && (options.DecoderUrl is null || decodeWoff2 is null))
            throw new ArgumentException("PreloadLanguages requires a WOFF2 decoder. For offline fonts, use Assets instead.", nameof(options));
        var defaults = options.LoadDefaultFontsFromCdn
            ? BrowserDefaultFonts.Paths.Select(path => new BrowserFontAsset("Roboto", new Uri(options.BaseUrl, path).AbsoluteUri)) : [];
        assets = defaults.Concat(languageAssets).Concat(assets).ToArray();
        using var concurrency = new SemaphoreSlim(4);
        var downloads = new Dictionary<string, Task<byte[]>>(StringComparer.Ordinal);
        var contents = new Dictionary<string, Task<byte[]>>(StringComparer.Ordinal);
        Task<byte[]> Load(BrowserFontAsset asset, bool strictOrigin = false)
        {
            var key = (strictOrigin ? "same-origin:" : "") + (asset.Url is null ? $"{asset.ResourceAssembly?.FullName}:{asset.ResourceName}" : new Uri(baseUri, asset.Url).AbsoluteUri);
            lock (downloads)
            {
                if (!downloads.TryGetValue(key, out var task)) downloads.Add(key, task = Read(asset, strictOrigin));
                return task;
            }
        }
        async Task<byte[]> Read(BrowserFontAsset asset, bool strictOrigin)
        {
            await concurrency.WaitAsync(cancellationToken);
            try
            {
                using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                deadline.CancelAfter(options.DownloadTimeout);
                var bytes = await asset.ReadAsync(http, baseUri, deadline.Token, strictOrigin);
                if (bytes.Length == 0) throw new InvalidDataException($"Startup font '{asset.Family}' is empty.");
                if (bytes.Length > BrowserFontData.MaximumBytes) throw new InvalidDataException("Font exceeds 30 MB.");
                var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes));
                Task<byte[]> normalized;
                lock (contents)
                {
                    if (!contents.TryGetValue(hash, out normalized!)) contents.Add(hash, normalized = Normalize(bytes, deadline.Token));
                }
                return await normalized;
            }
            finally { concurrency.Release(); }
        }
        async Task<byte[]> Normalize(byte[] bytes, CancellationToken token)
        {
            if (bytes.AsSpan().StartsWith("wOF2"u8))
            {
                if (BrowserFontData.CanUseWoff2Directly(bytes)) return bytes;
                if (options.DecoderUrl is null || decodeWoff2 is null)
                    throw new InvalidDataException("WOFF2 assets require an explicitly configured DecoderUrl.");
                bytes = await decodeWoff2(bytes, options.DecoderUrl, token).WaitAsync(token);
            }
            return BrowserFontData.Normalize(bytes);
        }
        // Await all work before disposing the semaphore, even when a source fails.
        var loadedAssets = await Task.WhenAll(assets.Select(asset => Load(asset)));
        async Task<(BrowserCssFontFace Face, byte[]? Bytes)> LoadFace(BrowserCssFontFace face)
        {
            foreach (var source in face.Sources)
            {
                try
                {
                    var bytes = await Load(new BrowserFontAsset(face.Family, source), options.CssFontsSameOriginOnly);
                    using var data = SkiaSharp.SKData.CreateCopy(bytes);
                    using var probe = SkiaSharp.SKTypeface.FromData(data);
                    if (probe is null || probe.GlyphCount == 0) throw new InvalidDataException("Skia could not decode CSS source.");
                    return (face, bytes);
                }
                catch (Exception error) when (error is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
                { Console.WriteLine($"Doroti CSS fonts ({face.Stylesheet}, {face.Family}, {source}): {error.Message}"); }
            }
            return (face, null);
        }
        var cssLoaded = await Task.WhenAll(cssFaces.Select(LoadFace));
        var registered = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // CSS order is preserved; explicit app assets follow it and take priority.
        foreach (var (face, bytes) in cssLoaded)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (bytes is null) continue;
            try
            {
                if (registerFace is not null) registerFace(bytes, face.Family, face.Descriptor);
                else register(bytes, face.Family);
                registered.Add(face.Family);
            }
            catch (InvalidDataException error) { Console.WriteLine($"Doroti CSS fonts ({face.Stylesheet}, {face.Family}): {error.Message}"); }
        }
        for (var i = 0; i < loadedAssets.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            register(loadedAssets[i], assets[i].Family);
            registered.Add(assets[i].Family);
        }
        if (!registered.Contains(options.DefaultFamily)) throw new InvalidDataException($"Default font '{options.DefaultFamily}' failed to load.");
    }
}
