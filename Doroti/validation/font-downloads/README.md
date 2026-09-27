# Automatic font fallback regression

Run from the repository root (all commands use the 20-minute deadline):

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/font-downloads -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/font-downloads -c Release -- --network
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/cupertino-sample -c Release -- --font-update
```

The network mode also exercises `PreloadLanguages = ["ko", "en", "ko-KR"]`
through the production startup loader and WOFF2 wrapper. A fresh collection must
cover all 11,172 modern Hangul syllables and compatibility/conjoining IME jamo
before input, with zero subsequent fallback downloads for new combinations.
`font-assets` covers hint aliases, Chinese variants, shared-script deduplication,
no-hint startup, invalid hints and asset-only network isolation using a fake transport.

2026-09-28 language-hint validation: SampleApp2 Release built with zero warnings
and errors. Chrome displayed `ㄱ 가 각 갂 똠 쀍 힣 한글` in both the Profile text field
and greeting using `PreloadLanguages = ["ko", "en"]`. The disposable screenshot
is `Doroti/artifacts/font-downloads/browser-preload-korean.png`. This is browser
text insertion/display evidence, not a physical keyboard IME composition recording.
The real Korean WOFF2 is 2,058,984 bytes (decoded: 6,223,356 bytes); exhaustive
modern syllable/jamo coverage and zero late downloads passed in the startup test.

The offline tests cover the full catalog, missing/unsupported scalars, coalescing,
bounded 404 alternatives/transient retries, retry success, disabling downloads,
and disposal during a request. The network mode requires Node and uses the production
wrapper with the CDN decoder; it validates the three CDN Roboto weights and 13 font subsets
covering Korean, Japanese, Chinese, Arabic, Devanagari, Thai, Tamil, Georgian,
Hebrew and several emoji. The mounted-widget test checks intrinsic widths change
after the framework processes `fontsChange` without rebuilding the widget.

2026-09-27 actual Chrome SampleApp2 verification: starting with only Roboto,
typing `한글 日本語 ไทย 😀 🚀` downloaded the required subsets and displayed all
characters and colored emoji in both the input and greeting without reloading.
The local evidence is `Doroti/artifacts/font-downloads/browser-auto-fonts.png`.
It is disposable and not tracked. This is a real web GPU display gate; the native
Windows raster reports no COLRv1 color and is not used as the web color gate.

These are font coverage/download tests, not full Arabic/Indic shaping or complex
emoji-sequence parity tests. See the host font documentation for those boundaries.

2026-09-28 CDN-only web update: default Roboto 400/500/700 faces and the remote
decoder passed real-download validation. Web Release built without `wwwroot/fonts`;
the static asset manifest contains no local font assets and the dependency graph
does not contain `Doroti.Skia.Fonts`. The screenshot above predates this CDN-only
default-font change; browser control was disconnected for its final visual rerun.
