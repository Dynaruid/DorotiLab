# Paragraph shaping with the current SkiaSharp supply

The Doroti renderer uses SkiaSharp 4.156.0. It offers an opt-in layout that combines Unicode analysis in C# with official HarfBuzz shaping. Its layout snapshot carries UTF-16 cluster ranges, positioned glyphs, line and word boundaries, and visual geometry into drawing and editing. [Renderer](../../src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs) · [Pinned SkiaSharp release](https://github.com/mono/SkiaSharp/releases/tag/v4.156.0)

## 2026-11-26 supply audit

The managed API documentation in the installed 4.156.0 package has no `SKParagraph` binding. The Windows x64 `libSkiaSharp.dll` exposes `sk_canvas_draw_text_blob`. Among its 990 exports there were no paragraph, SkUnicode ICU or shaper entry points. The inspected Web static archives expose `sk_canvas_draw_text_blob` and have no externally defined `sk_paragraph_*`, `skia::textlayout::Paragraph*` or `SkUnicodes::ICU::Make` symbols.

| Artifact | SHA-256 |
| --- | --- |
| win-x64 `libSkiaSharp.dll` | `591421fa5322466196321a5fd216716c74ca02d7af64c0a62cb45edb0147c0d7` |
| Web 3.1.34 `st,simd` | `9bd9ff777eb215a5974ef1fd2441fae043815f6241686e18f2d906e81fa42768` |
| Web 3.1.34 `mt,simd` | `70e2a5d4a3cce0a297959701143af9f9005570b5e8b80c933ada3ceb942f422c` |
| Web 3.1.56 `st,simd` | `c8fe51905156f0ad6949da20d5573a559040800d12d9cc30fed923ec5d68db5a` |
| Web 3.1.56 `mt,simd` | `63a966cb8e84f0d0dfed5a6b95dc773a6973709967ba37ec2f3ce3e62ac52064` |
| Web 6.0.2 `st,simd` | `46d7dc6befd6e1ed751e3fc060d6f5fa96eb42dff780ed119f0c23ae1f0fa49e` |
| Web 6.0.2 `mt,simd` | `b151fb1c9a46dc0ba251950f5ed1b7e4f42db5fa07b79f0558a6f675514909d2` |

The checks used `llvm-readobj --coff-exports` for Windows and `llvm-nm --defined-only --extern-only --demangle` on the package archives under `buildTransitive/netstandard1.0/libSkiaSharp.a`. The stock Windows DLL and Web archives lack the symbols that a SkParagraph bridge would require.

## Official HarfBuzz and managed layout

The chosen P4 direction uses the published SkiaSharp and HarfBuzzSharp binaries for drawing and shaping. A managed layout assembles the bidirectional runs, line and word boundaries, and font fallback. It hands the same UTF-16 cluster ranges and positioned glyphs to Skia paint, caret and selection, and semantics. Web publish links the official archives with the .NET toolchain. The probe uses SkiaSharp 4.156.0 and HarfBuzzSharp 14.4.0.100. [SkiaSharp packages](https://github.com/mono/SkiaSharp/blob/main/documentation/dev/packages.md) · [Pinned `SKShaper`](https://github.com/mono/SkiaSharp/blob/v4.156.0/source/SkiaSharp.HarfBuzz/SkiaSharp.HarfBuzz/SKShaper.cs) · [HarfBuzzSharp Web assets](https://www.nuget.org/packages/HarfBuzzSharp.NativeAssets.WebAssembly/14.4.0.100)

The same [C# probe](../../tests/Doroti.Paragraph.Probe/ParagraphProbe.cs) passed on Windows x64 and Chromium 158.0.8127.18 with net10/Mono 10.0.15 and Emscripten 3.1.56 in both Web modes.

| Fixture | Observed result |
| --- | --- |
| Arabic `سلام` | Contextual glyphs differ from independently shaped letters. RTL clusters are `3,2,1,0`. |
| Devanagari `कि` | The pre-base vowel is drawn before the consonant. Both glyphs belong to cluster `0`. |
| Emoji `👩‍💻🙂` | Two glyphs use UTF-16 starts `0,5`. The buffer receives explicit UTF-16 input. |
| Mixed `abc سلام xyz` | RichTextKit 0.6.175 finds RTL range `4–8` and hit-tests its midpoint at logical index `5`. The recorded picture still paints the glyphs and selection after the layout is cleared. |
| Thai `ภาษาไทยภาษาไทย` | RichTextKit returns start `0` alone on all three profiles. ICU4N 60.1.0-alpha.440 separately found UTF-16 word and line boundaries `0,4,7,11,14` on Windows with culture `th`. |

RichTextKit demonstrates mixed-direction layout with the official binaries. Its line breaker maps script-specific `SA` to `AL` and its shaper guesses the segment language. A shared renderer needs a verified language-aware segmenter and a way to carry the requested locale into shaping. ICU4N demonstrates dictionary segmentation with managed code. Its prerelease package is based on ICU4J 60.1 and carries separate Unicode data. In the untrimmed Windows probe with `SatelliteResourceLanguages=th`, the ICU4N assembly and the two resource assemblies totaled 9,085,952 bytes. Select a segmentation dependency after checking the current Unicode corpus, Web and AOT behavior, and incremental size. [RichTextKit line breaker](https://github.com/toptensoftware/RichTextKit/blob/3fd04bccf9c35c9b8925722eb6ecbdb24f4580db/Topten.RichTextKit/LineBreakAlgorithm/LineBreaker.cs) · [RichTextKit shaper](https://github.com/toptensoftware/RichTextKit/blob/3fd04bccf9c35c9b8925722eb6ecbdb24f4580db/Topten.RichTextKit/TextShaping/TextShaper.cs) · [ICU4N](https://github.com/NightOwl888/ICU4N)

## Product opt-in

Set `-p:DorotiShapedParagraphs=true` when building the runner from source. The flag selects the new `SkiaSceneRenderer` layout path and resolves the official HarfBuzz native assets for the target. Use separate build directories for the opt-in profile and the default advance-based profile.

The managed path uses the Avalonia 12.2.0 Unicode 17.0.0 analysis, with Doroti fixes in bidirectional bracket resolution. The Thai segmenter finds a path through an ICU 79.1 dictionary of 26,383 entries at grapheme boundaries. Its compressed resource is 106,111 bytes. The segmenter uses the loaded words and contains its own managed matching logic. [Input provenance](paragraph-unicode-provenance.json) · [Unicode 17 bidirectional specification](https://www.unicode.org/reports/tr9/tr9-51.html)

`ParagraphBuilder` defers shaping until the first width is available. `ApplyHostLayout` publishes line, cluster, and word geometry together with the retained paint data. Skia draws the positioned blobs from that snapshot. The recorded picture owns its snapshot independently of later relayout, disposal of the public paragraph, and font cache invalidation. [Layout](../../src/Doroti.Skia.Rendering/ShapedText/SkiaSceneRenderer.ShapedParagraph.cs) · [Placement](../../src/Doroti.Skia.Rendering/ShapedText/SkiaSceneRenderer.ParagraphPlacement.cs)

The product probe passed on Windows x64, Windows x64 NativeAOT, WSL Linux x64, and Chromium 158.0.8127.18 with the compatible and threaded .NET 10.0.15 Web profiles. It covers mixed RTL selection and hit testing, Devanagari and emoji UTF-16 cluster ranges, Thai dictionary selection and wrapping, soft-wrap affinity, CRLF, ellipsis, and pixel-identical retained replay after relayout and font changes. The opt-in WindowsAppSDK Demo exposed Korean application and Material labels and the English subtree label through UI Automation. The compatible Web Demo passed synthetic locale switching, DOM semantics and active text-input language updates at `/` and `/demo/`.

The Unicode 17.0.0 corpus passed 91,707 bidirectional character cases, 19,338 line breaks, 1,944 word breaks, and 766 grapheme breaks. The corpus checks the default Unicode algorithms. Thai tailoring is checked through the separate product probe.

### Windows CPU measurements

Release x64 on Ryzen 7 8845HS and the .NET 10 runtime. The layout measurement builds and disposes a new paragraph with a warmed renderer. The paint measurement replays one recorded picture onto a reused CPU bitmap. Each row uses five warmups and 25 samples.

| Workload | Layout p95, advance / HarfBuzz | Paint p95, advance / HarfBuzz | Layout managed bytes, advance / HarfBuzz |
| --- | ---: | ---: | ---: |
| Short English | 11.0 / 55.8 µs | 6.3 / 4.6 µs | 4,968 / 15,280 |
| Wrapped English | 162.8 / 906.4 µs | 95.0 / 22.4 µs | 57,296 / 161,402 |
| Wrapped mixed scripts | 185.1 / 1,175.1 µs | 137.9 / 41.7 µs | 27,128 / 187,712 |

The initial 10% English layout regression budget is open. The advance-based mixed workload is a timing reference with different text correctness. The renderer DLL is 310,272 bytes in Release without the option and 801,792 bytes with it, before trimming or compression. The WindowsAppSDK Debug output contains 2,357,200 bytes across the three HarfBuzz DLLs. These measurements cover local CPU layout, replay and files. Display timing, native retained memory and device download size have separate gates.

The SDK 10 compatible Debug Demo with the full localization catalog added 379,811 Brotli-compressed bytes in the native WebAssembly runtime and the renderer and HarfBuzz managed assemblies combined. The runtime grew by 199,082 bytes, the renderer by 162,153 bytes, and the two HarfBuzz assemblies contributed 18,576 bytes. Both publishes used full ICU. The comparison measures those named runtime and managed assets.

Remaining product qualification includes additional languages with dictionary segmentation, style and font variation fidelity, line-context shaping, font coverage with real app catalogs, physical IME and accessibility, and runner and performance gates.

### Reflow and OpenType

The renderer keeps up to 16 recently laid out paragraphs, with a combined budget of 16,384 UTF-16 code units and shaped glyphs. Width changes reuse the unpositioned glyphs, font leases, and intrinsic widths. A left-aligned paragraph that fits at both widths can reuse its line and selection geometry. Paragraph disposal, font registration, and renderer disposal release the applicable leases. An already recorded picture owns its positioned glyph blobs throughout these operations.

The public HarfBuzzSharp API shapes with the same variation coordinates as the Skia typeface and accepts `TextStyle.fontFeatures`. Each renderer reuses a HarfBuzz buffer for runs up to 4,096 UTF-16 code units. The shaping call copies glyph IDs and positions into the owned run arrays before the buffer is reused. [SkiaSharp stream bridge](https://github.com/mono/SkiaSharp/blob/v4.156.0/source/SkiaSharp.HarfBuzz/SkiaSharp.HarfBuzz/BlobExtensions.cs) · [HarfBuzz buffer lifetime](https://harfbuzz.github.io/harfbuzz-hb-buffer.html#hb-buffer-clear-contents)

Line placement stores one ordered geometry array. Word boundaries are computed on demand and reuse the Thai dictionary results. The ASCII fast path applies the Unicode 17 line break rules for letters, digits, spaces, and infix numeric separators. Twenty-three differential cases cover the fast path, including the break before a decimal separator after a space, and the transition to the general enumerator. The full Unicode 17 corpus still passes. [UAX #14 revision 55](https://www.unicode.org/reports/tr14/tr14-55.html)

The updated product probe passed on Windows x64 JIT and NativeAOT, WSL Linux x64, and Chromium 158.0.8127.18 with both SDK 10 Web profiles. It compares fresh and reused geometry across alignments and widths, stresses entry eviction, replaces a font alias, and checks retained replay byte for byte. The fixed Arabic font exercises proportional and tabular Arabic-Indic digits and the width axis in both layout and raster output. Each profile produced the same default-style probe PNG hash as its corresponding pre-change build.

Firefox 159.0 and Playwright WebKit 27.2 also passed with the same SDK 10 Debug publishes in both modes. Each browser verified the mode's isolation setting and produced the same default-style probe PNG as Chromium.

The CPU probe now times new paragraphs with repeated input, new paragraphs with changing text, and relayout of an existing paragraph at changing widths. The table shows p50 and p95 in µs from the same fixture on Windows x64 Release. Each row uses five warmups and 25 samples. The initial and current HarfBuzz columns use the same shaping workloads and font files.

| Workload | Operation | Advance p50 / p95 | Initial HarfBuzz p50 / p95 | Updated HarfBuzz p50 / p95 |
| --- | --- | ---: | ---: | ---: |
| Short English | Layout, repeated input | 10.7 / 11.1 | 55.0 / 57.8 | 13.0 / 16.0 |
| Short English | Layout, changing text | 11.3 / 46.7 | 62.6 / 113.0 | 13.5 / 48.7 |
| Short English | Relayout, changing width | 0.2 / 0.4 | 26.9 / 42.3 | 0.2 / 0.4 |
| Short English | Paint | 4.8 / 5.7 | 1.9 / 2.1 | 2.0 / 2.2 |
| Wrapped English | Layout, repeated input | 148.5 / 183.1 | 817.7 / 912.1 | 125.8 / 155.5 |
| Wrapped English | Layout, changing text | 166.1 / 174.3 | 825.2 / 1,047.7 | 167.8 / 214.9 |
| Wrapped English | Relayout, changing width | 4.8 / 7.6 | 314.5 / 322.4 | 53.6 / 92.2 |
| Wrapped English | Paint | 69.5 / 98.0 | 17.7 / 18.1 | 18.5 / 34.7 |
| Wrapped mixed scripts | Layout, repeated input | 135.2 / 141.9 | 1,375.9 / 2,056.4 | 630.2 / 707.2 |
| Wrapped mixed scripts | Layout, changing text | 143.3 / 174.7 | 1,213.5 / 2,739.1 | 700.8 / 2,594.7 |
| Wrapped mixed scripts | Relayout, changing width | 3.5 / 6.6 | 537.3 / 583.3 | 40.6 / 46.3 |
| Wrapped mixed scripts | Paint | 123.9 / 150.8 | 34.8 / 35.2 | 23.4 / 39.7 |

For repeated input, the updated HarfBuzz path allocates 5,688 bytes for short English, 47,520 for wrapped English and 72,824 for wrapped mixed scripts. Short English relayout at fully fitting widths has zero measured managed allocation. Short English layout, wrapped English layout for changing text and wrapped English relayout exceed the proposed 10% gate. The mixed text update p95 is well above its median in this sample. The advance-based mixed-script timings represent different rendering behavior. The paragraph path stays opt-in while overall performance and runner qualification are open.

### Accessibility geometry and dictionary coverage

`ParagraphBuilder` uses the current view's `GraphicsText` capability when the framework enters a view scope. `RenderParagraph` and `RenderEditable` capture placed UTF-16 clusters, line indices, directions and selection bounds from that layout. Native geometry projections and wire serialization preserve the boundaries. The text covered by an ellipsis still has logical boundaries and contributes no positioned rectangles. The existing `StringInfo` contract supplies boundaries for unplaced text and the advance-based layout.

The opt-in renderer loads the ICU 79.1 dictionary for each of four scripts on demand. Both word selection and wrap opportunities use the managed segmenter's output at grapheme boundaries. [ICU dictionary boundary analysis](https://unicode-org.github.io/icu/userguide/boundaryanalysis/#dictionary-based-breakiterator) · [Pinned inputs and hashes](paragraph-unicode-provenance.json)

| Language | Entries | Gzip resource bytes |
| --- | ---: | ---: |
| Thai | 26,383 | 106,111 |
| Lao | 30,550 | 130,891 |
| Khmer | 81,028 | 370,681 |
| Burmese | 41,120 | 185,091 |

The three added resources total 686,663 bytes before assembly or deployment compression. The probe checks two-word phrases without spaces. ICU 78's public word and line iterators on WSL return the same UTF-16 split at offset 2 for `ດີຂອບໃຈ`, `តើលោក` and `လူတိုင်း`. The Doroti probe additionally checks line and selection rectangles and raster output for both lines. It exercises the accessibility range around an Arabic discretionary ligature, CRLF, an Indic syllable and a ZWJ emoji using an explicit invocation in a two-view dispatcher.

The extended probe passed on Windows x64 JIT, Windows x64 NativeAOT and WSL Linux x64 JIT. The original fixed paragraph image retained its previous per-platform SHA-256. The renderer package contains the four dictionary notices and the pinned Unicode provenance file. The Windows Release scroll suite passed. The Debug run passed the caret and selection checks and stopped in `InheritedDependencyRegression` on the framework build-target assertion.

The SDK 11.0.100 compatible Release/AOT product probe ran on net10.0.15 in Chromium 158.0.8127.18 and confirmed the four dictionaries and layout-backed semantics. The corrected threaded AOT build was interrupted during IL optimization. The earlier threaded publish predated the probe's browser time-provider scope. Testing the extended probe in Firefox, WebKit and the remaining SDK/configuration profiles is open.

### Linux Qt Demo

The Linux x64 Graphite configuration accepts the original managed assembly and Linux native binary from signed SkiaSharp 4.156.0. The .NET 10 Debug Demo ran against system Qt 6.10.2 under WSL with Xvfb and Mesa software Vulkan. The two starts used process-local `en_US.UTF-8` and `ko_KR.UTF-8` locales and a pinned OFL Noto Sans KR test font. The English and Korean screenshots show the matching application and Material captions and the English subtree label in both runs.

The Qt 6.12.0 bundle uses the pinned Framework SDK with WebEngine disabled. The native window registers its accessibility root before the first managed tree arrives. The bundled Demo ran under WSL with Xvfb and Mesa software Vulkan in `en_US.UTF-8` and `ko_KR.UTF-8`. AT-SPI reported the application and Material captions in each locale and the fixed English subtree. In both runs it also applied text and caret changes and returned nonempty character bounds for the Arabic lam-alef, combining accent and ZWJ emoji. The text query used the UTF-16 offsets exposed by the Qt adaptor. [Qt runtime and SDK settings](../../../packages/platforms/qt/qt-runtime.md)

### Android Demo

The .NET 10 MAUI runner aligns the Lifecycle and Ktx packages with the AndroidX Core and Collection versions selected by Window and WebKit. The accessibility bridge uses the integer checked state from that AndroidX binding. The APK and load-path integrity checks use SHA-256 pins from the signed SkiaSharp.NativeAssets.Android 4.156.0 package. Both ABI pins were verified against the official package. The ARM64 APK contained the exact Skia library and the ARM64 HarfBuzz library. [SkiaSharp Android package](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Android/4.156.0)

The opt-in .NET 10 Debug Demo ran on a Galaxy SM-S931N with Android API 37. The installed base APK matched the signed local APK. Changing the app locale from `en-US` to `ko-KR` updated the generated application caption and Material Cancel label while the `Localizations` override kept its English text. UI Automator and the captured screen confirmed the labels. The test package was removed. This run covers ARM64 startup and app language changes.

From the repository root, use the SDK selected by `Doroti/global.json` and a short artifacts path for Android packaging. The probe uses its own application ID.

```powershell
$repo = (Get-Location).Path
Push-Location "$repo/Doroti"
try {
    python eng/run-with-timeout.py dotnet build "$repo/samples/DorotiDemoApp/android/DorotiDemoApp.Android.csproj" `
        --disable-build-servers -nr:false -c Debug -r android-arm64 -p:DorotiCompilationMode=Mono `
        -p:PublishAot=false -p:UseMonoRuntime=true -p:RunAOTCompilation=false -p:AndroidEnableProfiledAot=false `
        -p:DorotiAndroidTargetFramework=net10.0-android -p:AndroidPackageFormats=apk -p:EmbedAssembliesIntoApk=true `
        -p:DorotiShapedParagraphs=true -p:DorotiIntlProbe=localizations -p:ApplicationId=dev.doroti.work2probe `
        '-p:ApplicationTitle=Doroti Locale Probe' "-p:ArtifactsPath=$repo/Doroti/artifacts/w2a"
} finally { Pop-Location }
```

Install the signed probe APK on the ARM64 test device. Use `cmd locale set-app-locales dev.doroti.work2probe --locales` with each language tag and `am start -W` with the launcher component resolved by `cmd package resolve-activity`. Read the three labels from the foreground test app with UI Automator. The two application captions are `2 localized items` and `현지화 항목 2개`. The Material label changes from `Cancel` to `취소`, and the override reads `One localized item` in both captures.

SDK 10 and SDK 11 Debug profiles also ran with `-r android-x64` on an Android 13 emulator. For SDK 11, select 11.0.100 with `maui-android` installed and use `-p:DorotiAndroidTargetFramework=net11.0-android -p:UseMonoRuntime=false -p:PublishReadyToRun=false` with the non-AOT settings above. The MAUI host applies the AndroidX alignment to both Android target frameworks. Each installed APK matched its signed local file. Android UI Automator confirmed the English and Korean application and Material captions and the fixed English subtree. Both test packages were removed. TalkBack and physical IME composition still require device verification.

### Continued reflow measurements

The renderer keeps its most recent left-aligned placement across widths that preserve all its line-breaking decisions. A width change outside that range recalculates the layout. Width-dependent alignment and elision use their own placement. The glyph recorder writes the positions directly into the public Skia run buffer.

The Windows x64 Release CPU fixture ran after the parallel builds finished. Both profiles used the same font files and logical CPU. Each operation had five warmups and 25 samples.

| English workload | Operation | Advance p50 / p95 (µs) | HarfBuzz p50 / p95 (µs) |
| --- | --- | ---: | ---: |
| Short | New paragraph, same text | 12.0 / 13.2 | 14.9 / 26.8 |
| Short | Change width | 0.3 / 0.4 | 0.2 / 0.3 |
| Wrapped | Change text | 196.7 / 234.0 | 173.3 / 239.4 |
| Wrapped | Change width | 5.2 / 10.7 | 57.6 / 75.7 |
| Wrapped | Replay recorded picture | 73.4 / 86.3 | 19.6 / 20.0 |

The wrapped English resize case allocated 16,025 managed bytes per sample with shaping. The earlier shaping measurement in this document recorded 20,367 bytes for the same case. Short repeated layout and wrapped resize exceed the proposed 10% p95 budget. The renderer keeps the shaping path opt-in. Frame timing on a physical device remains open.

## Reproduce the probe

Use the OFL test fonts from [Google Fonts at the pinned revision](https://github.com/google/fonts/tree/6ebe737e44fd64ee736bbc6feed6136a6dbfd222/ofl). They are fetched into the disposable artifacts directory and provide the probe's fixed glyph coverage.

| Local font | SHA-256 |
| --- | --- |
| `Arabic.ttf` | `63111b5b2e074dd48cc67692e0a2726d86ee94c1c37fe8598257b7b4e87e869e` |
| `Devanagari.ttf` | `14ec4af41f27482216d1c2229f417ff9b1425e1babb014e57d1d40d03229853e` |
| `Thai.ttf` | `5a1c559bb539583c8a1fd99d1c5b9491e5e14478c9cd2bd0970d5c3096cc9ef8` |
| `Emoji.ttf` | `de6c18832938afc99caf132b39d6a30a19bac7f2e812e28db2535b4608d27551` |
| `Lao.ttf` | `9608b94603a82d09a8038946f9775242f99e3b3459b7f1e4d5b335b578cd7ab3` |
| `Khmer.ttf` | `f37a8431a0c5d5ed2f81a767417546aca576a81fb7eff9c924d46aecf828f2ca` |
| `Burmese.ttf` | `7abbbfbe2514105d7ce94937aee3feb2ba89b73a256c8b77b5866bd9b83e32ec` |

From the repository root on Windows, fetch the fixtures and run the same native probe.

```powershell
$repo = (Get-Location).Path
$output = Join-Path $repo 'Doroti/artifacts/work2-paragraph'
$fonts = Join-Path $output 'fonts'
New-Item -ItemType Directory -Force -Path $fonts | Out-Null
$origin = 'https://raw.githubusercontent.com/google/fonts/6ebe737e44fd64ee736bbc6feed6136a6dbfd222/ofl'
$files = @{
    Arabic = 'notosansarabic/NotoSansArabic%5Bwdth%2Cwght%5D.ttf'
    Devanagari = 'notosansdevanagari/NotoSansDevanagari%5Bwdth%2Cwght%5D.ttf'
    Thai = 'notosansthai/NotoSansThai%5Bwdth%2Cwght%5D.ttf'
    Emoji = 'notoemoji/NotoEmoji%5Bwght%5D.ttf'
    Lao = 'notosanslao/NotoSansLao%5Bwdth%2Cwght%5D.ttf'
    Khmer = 'notosanskhmer/NotoSansKhmer%5Bwdth%2Cwght%5D.ttf'
    Burmese = 'notosansmyanmar/NotoSansMyanmar%5Bwdth%2Cwght%5D.ttf'
}
foreach ($asset in $files.GetEnumerator()) {
    Invoke-WebRequest "$origin/$($asset.Value)" -OutFile "$fonts/$($asset.Key).ttf"
    $family = $asset.Value.Split('/')[0]
    Invoke-WebRequest "$origin/$family/OFL.txt" -OutFile "$fonts/$($asset.Key).OFL.txt"
}
Get-ChildItem $fonts -Filter '*.ttf' | Get-FileHash
$run = Join-Path $repo 'Doroti/eng/run-with-timeout.py'
$probe = Join-Path $repo 'Doroti/tests/Doroti.Paragraph.Probe'
python $run dotnet run --project "$probe/win/Paragraph.Win.csproj" --artifacts-path "$output/win-build" -- $fonts "$output/win.png"
```

For Web, use the existing browser test dependencies under `packages/platforms/web/tests` and SDK 10.0.403 with the project WebAssembly workload. Publish the two profiles with separate build directories. The browser test samples the same pixels and confirms the isolation setting for each profile.

```powershell
New-Item -ItemType Directory -Force -Path "$output/sdk10" | Out-Null
'{"sdk":{"version":"10.0.403","rollForward":"latestPatch","allowPrerelease":false}}' |
    Set-Content -Encoding utf8 "$output/sdk10/global.json"
Push-Location "$output/sdk10"
try {
    foreach ($mode in 'compatible','threaded') {
        $threads = ($mode -eq 'threaded').ToString().ToLowerInvariant()
        python $run dotnet publish "$probe/web/Paragraph.Web.csproj" -c Debug --artifacts-path "$output/$mode-build" "-p:WasmEnableThreads=$threads" -o "$output/$mode"
        if ($LASTEXITCODE -ne 0) { throw "$mode publish failed" }
        python $run node "$repo/packages/platforms/web/tests/browser-paragraph-supply.mjs" "$output/$mode/wwwroot" $mode "$output/$mode.png"
        if ($LASTEXITCODE -ne 0) { throw "$mode browser probe failed" }
    }
} finally { Pop-Location }
```

Add `-p:DorotiShapedParagraphs=true` to each `dotnet` command for the product probe and use fresh artifact directories. The receipt begins with `Product paragraph`. The Unicode corpus is also runnable independently using the four matching test files from the [Unicode 17.0.0 database](https://www.unicode.org/Public/17.0.0/ucd/).

The browser verifier accepts `firefox` or `webkit` as the fourth argument after the output PNG path. Use the corresponding browser from the verifier's Playwright installation. It checks the loaded runtime version, worker thread mode and shared heap in addition to cross-origin isolation. The harness restricts network requests to the static publish server.

SDK 10.0.403 and 11.0.100 each published against .NET 10.0.15 in both Web modes. The Debug and Release/AOT outputs passed in Chromium 158.0.8127.18, Firefox 159.0, and WebKit 27.2. These 24 outputs produced the same PNG SHA-256 `cf8ebe36fce85d33a5018071c607693747350a3603a6f9031f8039a46c475f13`. The first SDK 10 threaded Release run in WebKit timed out. The same build passed on retry.

For the Web Demo from the SDK 10 directory, pass `-p:DorotiShapedParagraphs=true`, `-p:DorotiIntlProbe=localizations`, `-p:DorotiWebExecutionMode=compatible` or `threaded`, and `-p:BlazorWebAssemblyLoadAllGlobalizationData=true` to the Demo runner publish. Pass an explicit 32-digit `DorotiWebBuildId` to pin the deployment generation across evaluations. The Demo passed the locale switch and DOM input language checks at `/demo/` in both modes. With `browser-widget-localization.mjs <wwwroot> <mode> /demo/ --icu-errors`, the same profiles reported a missing or corrupt ICU boot asset before producing a first frame. The fetch keeps the runtime's integrity hash.

```powershell
python Doroti/eng/run-with-timeout.py python Doroti/eng/import-paragraph-unicode.py Doroti/artifacts/work2-paragraph/unicode-sources --fetch --check
python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Skia.Rendering.Tests -p:DorotiShapedParagraphs=true -- --unicode-corpus Doroti/artifacts/work2-paragraph/ucd17
```

Use the same font directory for the two CPU measurement profiles.

```powershell
python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Skia.Rendering.Tests -c Release --artifacts-path Doroti/artifacts/work2-paragraph/measure/legacy -- --measure-paragraphs Doroti/artifacts/work2-paragraph/fonts
python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Skia.Rendering.Tests -c Release --artifacts-path Doroti/artifacts/work2-paragraph/measure/shaped -p:DorotiShapedParagraphs=true -- --measure-paragraphs Doroti/artifacts/work2-paragraph/fonts
```
