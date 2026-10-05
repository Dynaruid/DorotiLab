using System.Globalization;
using System.Text;
using Doroti.Skia.Rendering;

namespace Doroti.Host.Web;

public sealed record BrowserFontFallbackOptions
{
    public bool DiscoverCssFonts { get; init; } = true;
    public bool CssFontsSameOriginOnly { get; init; }
    public IReadOnlyList<string> CssStylesheets { get; init; } = [];
    public bool LoadDefaultFontsFromCdn { get; init; } = true;
    /// <summary>Language tags whose complete script fonts must load before the first view,
    /// e.g. ["ko", "en"]. Empty by default; unlisted scripts retain on-demand fallback.
    /// These are explicit CDN requests and require DecoderUrl for Noto WOFF2 fonts.</summary>
    public IReadOnlyList<string> PreloadLanguages { get; init; } = [];
    public string DefaultFamily { get; init; } = "Roboto";
    public IReadOnlyList<BrowserFontAsset> Assets { get; init; } = [];
    public bool Enabled { get; init; } = true;
    /// <summary>Mirror must preserve the catalog's relative paths and allow CORS.</summary>
    public Uri BaseUrl { get; init; } = new("https://fonts.gstatic.com/s/");
    public Uri? DecoderUrl { get; init; } = new("https://cdn.jsdelivr.net/npm/woff2-encoder@2.0.0/dist/decompress.js");
    public string? PreferredLanguage { get; init; }
    public TimeSpan DownloadTimeout { get; init; } = TimeSpan.FromSeconds(20);

    /// <summary>No default CDN fonts, automatic fallback downloads, or implicit decoder CDN.</summary>
    public static BrowserFontFallbackOptions AssetsOnly(string defaultFamily, params BrowserFontAsset[] assets) => new()
    {
        DefaultFamily = defaultFamily,
        Assets = assets.ToArray(),
        LoadDefaultFontsFromCdn = false,
        Enabled = false,
        DecoderUrl = null,
        DiscoverCssFonts = false,
        CssFontsSameOriginOnly = true,
    };
}

/// <summary>
/// Batches missing glyphs, downloads only covering Noto subsets, then notifies
/// the host to relayout. Calls and continuations belong to the render worker.
/// </summary>
public sealed class BrowserFontFallbackLoader : IDisposable
{
    private readonly SkiaFallbackFontCollection _fonts;
    private readonly BrowserFontFallbackOptions _options;
    private readonly Func<Uri, CancellationToken, Task<byte[]>> _download;
    private readonly Action _fontsChanged;
    private readonly Action<string> _log;
    private readonly TimeProvider _timeProvider;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly CancellationToken _lifetimeToken;
    private readonly HashSet<int> _pending = [];
    private readonly HashSet<int> _unsupported = [];
    private readonly HashSet<int> _loaded = [];
    private readonly HashSet<int> _failed = [];
    private readonly Dictionary<int, int> _attempts = [];
    private readonly FontFallbackCatalog _catalog = FontFallbackCatalog.Default;
    private readonly object _gate = new();
    private TaskCompletionSource? _idle;
    private volatile bool _disposed;
    public int DownloadCount { get; private set; }
    public int LoadedCount => _loaded.Count;
    public int UnsupportedCount => _unsupported.Count;

    public BrowserFontFallbackLoader(SkiaFallbackFontCollection fonts,
        Func<Uri, CancellationToken, Task<byte[]>> download, Action fontsChanged,
        BrowserFontFallbackOptions? options = null, TimeProvider? timeProvider = null,
        Action<string>? log = null)
    {
        _fonts = fonts;
        _lifetimeToken = _lifetime.Token;
        _download = download;
        _fontsChanged = fontsChanged;
        _options = options ?? new();
        _timeProvider = timeProvider ?? TimeProvider.System;
        _log = log ?? Console.WriteLine;
        if (!_options.BaseUrl.IsAbsoluteUri || !_options.BaseUrl.AbsoluteUri.EndsWith('/')
            || _options.BaseUrl.Scheme is not ("https" or "http"))
            throw new ArgumentException("Font fallback BaseUrl must be an absolute HTTP(S) directory URL.");
        if (_options.DownloadTimeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(options));
        _fonts.CharacterMissing += Request;
    }

    public void Request(int codePoint)
    {
        lock (_gate)
        {
            if (_disposed || !_options.Enabled || _unsupported.Contains(codePoint)
                || !Rune.IsValid(codePoint)) return;
            var category = Rune.GetUnicodeCategory(new Rune(codePoint));
            // Joiners/variation selectors/control characters do not need standalone glyphs.
            if (category is UnicodeCategory.Control or UnicodeCategory.Format
                || codePoint is >= 0xfe00 and <= 0xfe0f or >= 0xe0100 and <= 0xe01ef)
                return;
            _pending.Add(codePoint);
            if (_idle is null)
            {
                _idle = new(TaskCreationOptions.RunContinuationsAsynchronously);
                _ = ProcessAsync(_idle);
            }
        }
    }

    public Task WhenIdleAsync()
    {
        lock (_gate) return _idle?.Task ?? Task.CompletedTask;
    }

    private async Task ProcessAsync(TaskCompletionSource completion)
    {
        await Task.Yield(); // Never register/dispose fonts during layout or paint.
        try
        {
            while (true)
            {
                FontFallbackEntry selected;
                lock (_gate)
                {
                    if (_disposed || _pending.Count == 0)
                    {
                        _idle = null;
                        completion.TrySetResult();
                        return;
                    }
                    _pending.RemoveWhere(cp => _fonts.ContainsCharacter(cp));
                    var coverage = new Dictionary<int, int>();
                    foreach (var cp in _pending.ToArray())
                    {
                        var candidates = _catalog.Candidates(cp).Where(i => !_failed.Contains(i) && !_loaded.Contains(i)).ToArray();
                        if (candidates.Length == 0 || _attempts.GetValueOrDefault(cp) >= 3)
                        {
                            _unsupported.Add(cp);
                            _pending.Remove(cp);
                            _log($"Doroti font fallback: no downloadable font for U+{cp:X}.");
                            continue;
                        }
                        foreach (var index in candidates)
                            coverage[index] = coverage.GetValueOrDefault(index) + 1;
                    }
                    if (coverage.Count == 0)
                    {
                        _idle = null;
                        completion.TrySetResult();
                        return;
                    }
                    selected = _catalog.Select(coverage, _options.PreferredLanguage ?? "en");
                    foreach (var cp in _pending.Where(cp => _catalog.Candidates(cp).Contains(selected.Index)))
                        _attempts[cp] = _attempts.GetValueOrDefault(cp) + 1;
                }
                var bytes = await DownloadAsync(selected);
                lock (_gate)
                {
                    if (_disposed) break;
                    if (bytes is null)
                    {
                        _failed.Add(selected.Index);
                        continue;
                    }
                    try
                    {
                        _fonts.Register(bytes, selected.Family, preferForFallback: selected.IsEmoji);
                        _loaded.Add(selected.Index);
                    }
                    catch (Exception error) when (error is InvalidDataException or ArgumentException)
                    {
                        _failed.Add(selected.Index);
                        _log($"Doroti font fallback: {selected.Family}: {error.Message}");
                        continue;
                    }
                    _log($"Doroti font fallback loaded: {selected.Family}");
                    _fontsChanged();
                }
            }
        }
        catch (OperationCanceledException) when (_disposed) { }
        catch (Exception error)
        {
            _log($"Doroti font fallback failed: {error.Message}");
            completion.TrySetException(error);
            _ = completion.Task.Exception; // Observed even when a host does not await idle.
        }
        finally
        {
            lock (_gate)
            {
                if (ReferenceEquals(_idle, completion)) _idle = null;
                completion.TrySetResult();
            }
        }
    }

    private async Task<byte[]?> DownloadAsync(FontFallbackEntry font)
    {
        for (var attempt = 0; attempt < 2 && !_disposed; attempt++)
        {
            using var timeout = new CancellationTokenSource(_options.DownloadTimeout, _timeProvider);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeToken, timeout.Token);
            try
            {
                DownloadCount++;
                var bytes = await _download(new Uri(_options.BaseUrl, font.Path), linked.Token);
                if (bytes.Length == 0 || bytes.Length > 30 * 1024 * 1024)
                    throw new InvalidDataException("Invalid fallback font size.");
                return bytes;
            }
            catch (OperationCanceledException) when (_disposed) { throw; }
            catch (Exception error) when (error is HttpRequestException or OperationCanceledException or InvalidDataException)
            {
                if (attempt == 1 || error is InvalidDataException
                    || error is HttpRequestException { StatusCode: System.Net.HttpStatusCode.NotFound })
                {
                    _log($"Doroti font fallback unavailable: {font.Family}: {error.Message}");
                    return null;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(250), _timeProvider, _lifetimeToken);
            }
        }
        return null;
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _fonts.CharacterMissing -= Request;
            _lifetime.Cancel();
            _pending.Clear();
            // In-flight tasks own their linked token sources until completion.
            _lifetime.Dispose();
        }
    }
}
