# Automatic web font fallback

`DorotiWebWorkerRunner` enables fallback downloads by default. The three Roboto faces, active CSS font faces, and any explicit `PreloadLanguages`
or font assets load before the first frame. Language hints load complete Noto faces before view creation,
preventing first-use missing-glyph boxes during IME input for the covered script.
When a
glyph is absent, the render worker batches missing Unicode scalars, chooses Noto
fonts using the browser language and coverage, and downloads the selected subsets.
The catalog contains all 727 entries in the pinned Flutter reference, including
CJK, Indic, RTL and historic scripts, symbols and Noto Color Emoji subsets.

The default base is `https://fonts.gstatic.com/s/`; these are versioned font URLs.
The browser's normal HTTP cache applies. There is no private persistent font cache.
Existing loaded coverage and in-flight work are deduplicated within the worker.
The original text is never sent to the CDN: requests contain only font paths.

The worker probes the actual browser SkiaSharp build and font bytes, including
a nonzero glyph count, and uses native WOFF2 decoding when supported; otherwise it lazily
imports `woff2-encoder@2.0.0` from jsDelivr. Only Doroti's small TypeScript interop
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
        PreloadLanguages = ["ko", "en"], // default: []; independent of browser language
        DownloadTimeout = TimeSpan.FromSeconds(20),
    });
```

A mirror must preserve catalog paths and `BrowserDefaultFonts.Paths`, return font
bytes and allow CORS. CSP must permit the font and stylesheet origins in `connect-src`, the decoder
origin in `script-src`, and WebAssembly. The pinned `woff2-encoder@2.0.0` decoder
also requires `script-src 'unsafe-eval'` for its generated bindings;
`'wasm-unsafe-eval'` alone is insufficient. Use TTF/OTF/WOFF1 when this policy is unavailable.
`Enabled = false` disables missing-glyph downloads; the runner still needs its
Roboto defaults unless `LoadDefaultFontsFromCdn` is also disabled. Use `AssetsOnly`
below to disable both with one setting.

`PreloadLanguages = ["ko", "en"]` adds one complete Noto Sans KR WOFF2 download;
English reuses CDN Roboto. The Korean font includes IME compatibility/conjoining
jamo and all 11,172 modern syllables, including uncommon intermediate combinations.
Language aliases such as `ko` and `ko-KR` deduplicate by font family. Startup downloads use at most four concurrent operations. CSS faces register in
document order, followed by Roboto, hinted languages, then explicit app assets.
Explicit assets take matching priority; global fallback prefers explicit/default
fonts before incidental CSS families. Startup waits for download, decoding and registration; a preload
failure is a startup error, rather than silently allowing missing input glyphs.
Complete fonts cost more startup bandwidth/memory than a few text subsets, but
avoid later per-combination requests. Subsequent visits use normal HTTP caching.

Supported primary language tags are `en`, `ko`, `ja`, `zh`, `ar`, `fa`, `ur`, `he`,
`hi`, `mr`, `ne`, `bn`, `ta`, `te`, `kn`, `ml`, `gu`, `th`, `lo`, `km`, `my`, `ka`,
`hy`, `de`, `fr`, `es`, `it`, `pt`, `nl`, `sv`, `da`, `no`, `fi`, `pl`, `cs`, `sk`,
`hu`, `ro`, `tr`, `vi`, `id`, `ms`, `ru`, `uk`, `bg`, `sr`, and `el`.
Region suffixes are accepted; Chinese distinguishes `zh-Hans`/`zh-CN`,
`zh-Hant`/`zh-TW`/`zh-MO`, and `zh-HK`. Other tags use the primary language's
default script. Unknown primary tags fail before downloads; use explicit `Assets`
for other languages/scripts or a specific typeface. Hints are font coverage
choices, not shaping or localization settings, and cannot predict every symbol,
emoji or borrowed character a user might enter.

`PreferredLanguage` only influences on-demand subset selection; it does not
preload glyphs. Hints are explicit CDN requests, independent of `Enabled` and
`LoadDefaultFontsFromCdn`, and require `DecoderUrl`. `AssetsOnly` has empty hints
and continues to make no implicit CDN requests. For offline Korean input, include
a complete Korean TTF/OTF in `Assets` instead of adding a language hint.

There are no build-time font downloads or font/third-party decoder binaries in the
host web package. The host's `wwwroot/fonts/` is ignored by Git and excluded from static web assets,
even if an old local cache exists. The initial uncached web startup needs network
access to the default fonts; failures are surfaced by the existing startup error UI.
Subsequent loads use normal browser HTTP caching. Native hosts instead use the
build-time bundled fonts in `Doroti.Skia.Fonts` and need no runtime font network.

## App font assets instead of CDN defaults

For the normal SDK-generated web runner, set a C# options expression in the **web
project** (not the shared application project). No generated bootstrap edits are
needed:

```xml
<PropertyGroup>
  <DorotiWebFontOptions>global::MyApp.Web.WebFonts.Options</DorotiWebFontOptions>
</PropertyGroup>
```

Add `WebFonts.cs` to that web project:

```csharp
using Doroti.Host.Web;
namespace MyApp.Web;

public static class WebFonts
{
    public static BrowserFontFallbackOptions Options =>
        BrowserFontFallbackOptions.AssetsOnly("MySans",
            new BrowserFontAsset("MySans", "fonts/MySans-Regular.ttf"),
            new BrowserFontAsset("MySans", "fonts/MySans-Bold.ttf"));
}
```

Place those files in the application's own `wwwroot/fonts/` directory. Relative
URLs resolve against the application base URI, including deployment under a
subpath. The host's Git ignore rule does not exclude application font assets.
Each face registers under its declared family, and `DefaultFamily` is used when
text does not specify a family or a requested family cannot be resolved.

Alternatively, embed fonts in the web or shared application assembly:

```xml
<ItemGroup>
  <EmbeddedResource Include="assets/fonts/MySans-Regular.ttf" LogicalName="MyApp.Fonts.Regular.ttf" />
</ItemGroup>
```

```csharp
BrowserFontFallbackOptions.AssetsOnly("MySans",
    BrowserFontAsset.Embedded("MySans", typeof(WebFonts).Assembly,
        "MyApp.Fonts.Regular.ttf"));
```

Use the assembly that owns the resource. Embedded startup fonts require no font
HTTP requests. All assets load and register before the first view layout. Missing
files/resources and an absent default family fail startup explicitly; the loader
never switches to a CDN to hide a configuration error.

`AssetsOnly` disables CDN defaults, automatic Noto downloads and the implicit WOFF2
decoder CDN. Prefer TTF/OTF in this mode. WOFF2 assets on builds without native WOFF2 support require an explicitly supplied
`DecoderUrl` (host the decoder locally for no external requests). Include every
script/icon font the app needs: unsupported characters do not trigger downloads.
These settings cover font requests, not offline installation/caching of the app.

To combine custom defaults with automatic Noto fallback, use ordinary options with
`LoadDefaultFontsFromCdn = false`, `DefaultFamily = "MySans"`, and `Assets = [...]`;
leave `Enabled = true` and a decoder URL configured. To simply add asset families
alongside CDN Roboto, set only `Assets`. Custom runners can pass the same options
directly to `DorotiWebWorkerRunner.RunAsync`.

Transient network failures get at most two attempts per font. HTTP 404 and invalid
font data are terminal for that candidate. At most three candidates are attempted
per missing scalar; an unsupported scalar is then negatively cached for the
session. Disposal cancels in-flight requests and prevents late registration.
Reloading the application starts a fresh failure/retry session. Unavailable fonts
do not prevent the application from starting or processing input.

## Scope and maintenance

This implements automatic font discovery/download/registration, not a new text
shaper. Automatic fallback coverage is limited to the pinned Flutter/Noto catalog;
custom fonts retain their own glyph coverage and CSS unicode ranges. Complex
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

## CSS links and variable fonts

Ordinary links require no Doroti HTML attributes and no duplicate C# font list:

```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/galmuri@2.40.3/dist/galmuri.css">
<link rel="stylesheet" href="fonts/SUITE/SUITE-Variable.css">
```

Copy SUITE's original CSS, WOFF2 and LICENSE into that application directory.
Select `fontFamily: "Galmuri11"` or `fontFamily: "SUITE Variable"` in a widget
`TextStyle`, or in the app's theme. Adding a link makes families available; it
does not change the theme. HTML `body { font-family: ... }` is not inherited by
Doroti widgets. `DefaultFamily` may refer to a successfully loaded CSS alias.
A missing required default family is a startup error.

CSS is inspected on the DOM thread and descriptors cross the existing worker
control channel. Font bytes are downloaded, normalized and registered on the
render owner before the first view. CSSOM is preferred; inaccessible external
stylesheets are fetched with CORS, without requiring a `crossorigin` attribute.
Relative URLs use the **final stylesheet response URL**, including imports;
inline styles use `document.baseURI`. A CORS-denied CSS response cannot be read:
self-host it or register the font with `BrowserFontAsset`. Logs include the
stylesheet, family and failed URL. Implicit CSS failures try the next URL source
and continue with fallback. Explicit assets/language hints remain required.

The browser CSS parser validates `@font-face` declarations. Supported startup
scope: active links, style elements, adopted stylesheets, imports, media/supports
conditions, layers, family aliases, weight ranges, normal/italic/oblique style,
static stretch, unicode-range subsets, source alternatives and variation settings.
`local()` is skipped because it does not expose font bytes. TTF, OTF, WOFF1,
WOFF2 and legacy `woff2-variations` format hints are accepted. Unsupported
source technologies/descriptors are diagnosed. Oblique angles and stretch ranges
are not implemented; the widget API selects normal width, keeping Galmuri's
condensed face out of ordinary regular/bold selection. To explicitly choose a
condensed file, give it its own asset family alias. This is not the browser's
complete CSS font matching or `font-display` timing model.

Discovery is bounded to 64 fetched stylesheets, import depth 8, 128 faces, 2 MB
per fetched CSS response and a 20-second discovery budget. All discovered faces
are preloaded, so large subset catalogs should use explicit assets. Dynamic
link insertion/CSSOM changes and per-glyph CSS lazy loading are follow-up scope.
Downloads share URL tasks and decoded content hashes; each file is limited to
30 MB and `DownloadTimeout`. TTF/OTF signatures and WOFF lengths are checked;
WOFF1 tables are reconstructed with bounded zlib output and checksums. WOFF2
uses browser-native Skia decoding when supported, with the dedicated decoder as
a compatibility fallback. Both retain SUITE's fvar/gvar tables. Do not infer browser support from Windows-native Skia or CanvasKit: the browser
qualification reports its own byte counts and actual typeface glyph counts.

`fontWeight` sets the `wght` axis. Explicit `fontVariations` override the automatic
coordinate and CSS face variation defaults; the last occurrence of an explicit
axis wins. Unknown axes are ignored, finite coordinates clamp to the font's axis
bounds, and invalid/non-finite axis entries are ignored. The same instantiated
face drives measurement and painting. Variation keys sort axes and include their
values; clones belong to the existing 256-entry text-resource LRU and are disposed
on eviction/font invalidation. Variable weight disables duplicate synthetic bold.
No unbounded global clone cache is introduced. Picture/display-list text styles
already carry variation coordinates; registrations invalidate renderer caches.

## Local CSS and decoder without external font traffic

```csharp
BrowserFontFallbackOptions.AssetsOnly("SUITE Variable") with
{
    CssStylesheets = ["fonts/SUITE/SUITE-Variable.css"],
    DecoderUrl = new Uri("fonts/decoder/decompress.js", UriKind.Relative),
};
```

`AssetsOnly` disables `DiscoverCssFonts`, implicit defaults, fallback downloads
and the CDN decoder. It enables `CssFontsSameOriginOnly`; explicit
`CssStylesheets` are still inspected. To discover local HTML links instead, set
`DiscoverCssFonts = true` while retaining the same-origin restriction. Strict
mode rejects stylesheet/font redirects, including same-origin redirects, to
avoid following an implicit external redirect. Explicit `Assets` are still
explicit requests and can point to a chosen external service.

Relative decoder URLs resolve against the application base after host startup.
With `/sample/` deployment, set HTML `<base href="/sample/">` and serve all
assets under that prefix. A 200 HTML fallback is not a valid font response.

SampleApp2 supports `-p:DorotiBundleWoff2Decoder=true` independently of its normal
CDN mode. `-p:DorotiSampleWebFontSource=Assets` enables it automatically, packages
local Galmuri/SUITE CSS and fonts, uses the asset HTML, and embeds Roboto. The
pinned 295,397-byte `woff2-encoder@2.0.0/dist/decompress.js` contains its WASM as a
base64 data payload and has no relative imports or separate WASM dependency.
Its LICENSE is published beside it. These files belong to the opt-in **sample**,
not the default Doroti.Host.Web package. SUITE 2.040 and Galmuri 2.40.3 use SIL OFL;
ship their LICENSE files when copying fonts. Fixture hashes and upstream URLs
are in `Doroti/validation/css-fonts/fixtures/manifest.json`.

The web host still requires COOP `same-origin` and COEP `require-corp` for shared
memory. Browser CSS loading uses `style-src`/`font-src`; Doroti's byte loading
also uses `connect-src`. Native WOFF2 decoding requires no JS decoder policy or decoder request. If the
compatibility module is needed, it uses `script-src` and, for this pinned decoder,
`unsafe-eval`. Offline font requests are independent of app
installation/service-worker caching. See `Doroti/validation/css-fonts/README.md`
for runnable tests, browser evidence and the physical IME boundary.
