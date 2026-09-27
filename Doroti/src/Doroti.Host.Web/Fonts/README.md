# Automatic web font fallback

`DorotiWebWorkerRunner` enables fallback downloads by default. Only the three
Roboto faces are downloaded from the versioned Google Fonts CDN before the first frame. When a
glyph is absent, the render worker batches missing Unicode scalars, chooses Noto
fonts using the browser language and coverage, and downloads the selected subsets.
The catalog contains all 727 entries in the pinned Flutter reference, including
CJK, Indic, RTL and historic scripts, symbols and Noto Color Emoji subsets.

The default base is `https://fonts.gstatic.com/s/`; these are versioned font URLs.
The browser's normal HTTP cache applies. There is no private persistent font cache.
Existing loaded coverage and in-flight work are deduplicated within the worker.
The original text is never sent to the CDN: requests contain only font paths.

The worker lazily imports `woff2-encoder@2.0.0` from jsDelivr because SkiaSharp
cannot decode these WOFF2 files directly. Only Doroti's small TypeScript interop
wrapper ships with the application. Registration clears every renderer's text/picture
cache sharing that collection and sends `flutter/system` `fontsChange` to the
framework. Mounted paragraphs and editable text invalidate both their layout
and intrinsic measurement caches before drawing another frame.

## Configuration

Custom web runners can pass options to the existing startup API:

```csharp
await DorotiWebWorkerRunner.RunAsync<MyStartup>(typeof(MyStartup).Assembly,
    fontFallbackOptions: new BrowserFontFallbackOptions
    {
        BaseUrl = new Uri("https://assets.example.com/fonts/"),
        DecoderUrl = new Uri("https://assets.example.com/woff2/decompress.js"),
        PreferredLanguage = "ko-KR", // omitted: browser language
        DownloadTimeout = TimeSpan.FromSeconds(20),
    });
```

A mirror must preserve catalog paths and `BrowserDefaultFonts.Paths`, return font
bytes and allow CORS. CSP must permit the font origin in `connect-src`, the decoder
origin in `script-src`, and WebAssembly under the same policy as the application.
`Enabled = false` disables missing-glyph downloads; the runner still needs its
Roboto defaults. Fully offline web deployment requires a local font/decoder mirror
or a custom runner that registers application assets.

There are no build-time font downloads or font/third-party decoder binaries in the
web package. `wwwroot/fonts/` is ignored by Git and excluded from static web assets,
even if an old local cache exists. The initial uncached web startup needs network
access to the default fonts; failures are surfaced by the existing startup error UI.
Subsequent loads use normal browser HTTP caching. Native hosts instead use the
build-time bundled fonts in `Doroti.Skia.Fonts` and need no runtime font network.

Transient network failures get at most two attempts per font. HTTP 404 and invalid
font data are terminal for that candidate. At most three candidates are attempted
per missing scalar; an unsupported scalar is then negatively cached for the
session. Disposal cancels in-flight requests and prevents late registration.
Reloading the application starts a fresh failure/retry session. Unavailable fonts
do not prevent the application from starting or processing input.

## Scope and maintenance

This implements automatic font discovery/download/registration, not a new text
shaper. Font coverage is limited to the pinned Flutter/Noto catalog. Complex
script shaping, bidirectional ordering, variation selectors and multi-codepoint
emoji ligatures (ZWJ, flags, skin-tone sequences) are not made Flutter-equivalent
by downloading fonts. Single-codepoint color emoji works in the web FreeType/Skia
backend; native Windows DirectWrite COLRv1 rendering differs.

Run `python Doroti/eng/generate-font-fallbacks.py` from the repository root to
regenerate the binary catalog from the checked-in Flutter reference. It validates
indices and complete Unicode range coverage. Builds use the generated resource
and never read the reference checkout or query Google Fonts CSS. Source hash and
counts are recorded in `catalog-source.txt`; Flutter's license is included.

Validation and browser evidence: `Doroti/validation/font-downloads/README.md`.
