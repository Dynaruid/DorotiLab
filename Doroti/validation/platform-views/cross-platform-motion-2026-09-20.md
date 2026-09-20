# Cross-platform retained platform raster motion

Implementation follows the Android panel fixes without replacing native browser
instances or changing native input ownership. Artifacts are under
`../../artifacts/platform-views/2026-09-20/cross-platform-motion/`.

## Changes

- **Shared policy:** `SkiaPlatformRasterContent.CacheScope`, `CanReuse` and
  `Coverage` check owner, view/surface epoch, pixel extent, scale, background and
  resource generation. Immutable pictures with equivalent bitmap-local transforms
  and hard rectangular clips may move without rasterization. Changed content,
  fractional phase, unsupported effects or clipping conservatively redraw.
- **Android:** the existing bitmap/RenderNode optimization now uses the same
  scope checks, including background color and resource generation. The native
  WebView GPU source layer and previous touch/navigation fixes remain intact.
- **Windows WebView2:** determine changed slices before building the GPU readback
  atlas. Retain unchanged drawing surfaces and move their sprite visuals. Changed
  pixels receive a fresh surface/brush while keeping the sprite visual mounted;
  unchanged ordering no longer detaches the complete WebView composition tree.
  A staged frame records its dependency on the committed cache revision; a stale
  dependency is rejected for replay before native mutation. Failed commits clear
  cache eligibility, and moved visuals restore image/size/offset on rollback.
- **Windows WinUI/HWND:** reuse stationary raster surfaces before readback and
  upload, including all-reused frames with no pixel payload. The HWND bridge
  intentionally redraws translated raster regions: its existing full-window
  surface contract does not expose an independent bitmap offset. Native visual
  backdrop sources remain live, without native-content readback.
- **Qt Quick:** publish retained Vulkan P images for matching content instead of
  redrawing/copying them. Foreground targets use bounded clip coverage. Borrowed
  published images never enter the writable staging bank; their native identity
  and lifetime remain protected through rejected frames and queue retirement.
  Resizing discards obsolete staging storage within the existing budget. Cache
  ownership includes the GPU instance, and metadata advances after native commit.
  Existing queue/fence retirement waits remain; this is not a threading rewrite.
- **iOS/UIKit and macOS/AppKit:** render foregrounds into cropped Metal drawables;
  unchanged content keeps the layer's currently presented image and changes only
  its frame. Cache metadata advances on presentation, not preparation/submission.
  Hidden/recreated layers redraw, failed preparation invalidates eligibility, and
  rollback retains the previous layer geometry. Native layers are no longer
  hidden/reordered every frame when their visibility and order are unchanged.
- **Web DOM:** retain iframe/control nodes, use translate for position-only moves,
  and avoid identical style/filter writes. Hiding updates the style cache so a
  returning view becomes visible again. Input shields retain pointer capture and
  event forwarding.

The Apple approach uses the layer's current displayed image; it does not retain
extra drawable objects or render into a drawable already in use. See Apple's
[CAMetalLayer content contract](https://developer.apple.com/documentation/quartzcore/cametallayer)
and [drawable lifetime guidance](https://developer.apple.com/library/archive/documentation/3DDrawing/Conceptual/MTLBestPracticesGuide/Drawables.html).

## Validation

| Check | Evidence and boundary |
| --- | --- |
| Shared raster policy | `raster-cache-tests.log`: 31 checks passed, including changed owner/epoch/DPI/extent/background/resource generation, translated reuse, changed clipping and conservative fallbacks. |
| Composition lifetime | `composition-contract-tests.log`: 12 common fixture checks passed; covers stale admission, cancellation, commit failure and GPU retirement leases. This is not native presentation proof. |
| Windows product | `build-windows-final.log`: Release build, zero warnings/errors. `windows-effects-final/observed.json`: 11 live product assertions passed, including touch, shields, movement, resize, removal/recreation and visible native content. 204 commits included 207 retained raster uses and 180 uploads; these are workload observations, not a controlled speedup percentage. |
| WinUI controls | `windows-winui/result.json`: six product modes, blur pixel comparisons and 30-frame continuity passed. Editor/backdrop maximum difference was zero while spinner difference was 23.12; 207 commits used one placement batch. This gate preceded the final WebView-only visual-retention refinement. |
| Web | `build-web.log` and `web-dom.log`: host/TypeScript build and 12 browser DOM assertions passed, including no mutations on an unchanged batch, translated bounds, viewport/node identity and hide/show recovery. Worker-integrated rendering is not covered. |
| Qt | `build-qt-managed.log`: Windows-hosted managed compilation passed. Linux execution and Vulkan/Qt native runtime verification are **skipped by user request**. |
| macOS | `build-macos.log`: host library managed compilation passed, zero warnings/errors. No macOS product launch or physical display verification in this run. |
| iOS | Default configuration requested .NET 11 and failed with NETSDK1045 on this .NET 10 SDK (`build-ios.log`). Explicit .NET 10/iOS + MAUI 10 compatibility host build passed (`build-ios-compat.log`); this is not default-profile, NativeAOT/link, signing or device proof. Product target defaults were not changed. |
| Android | `build-android.log`: arm64 Release product build passed, zero warnings/errors; updated APK installed on the connected Galaxy S25. `android-motion` reached the moved endpoint, but system UI appeared during the final accessibility query. This run has no accepted timing/gesture verdict; no unrelated system UI text is retained. |

The subsequent app-only Android log sample (`android-observed-cache.json`) shows
7 changed and 293 reused slices across 100 commits. This confirms that the common
cache path runs on device, but the interaction was uncontrolled and is not a
replacement for the interrupted gesture/performance gate. APK and source hashes
are retained in the same artifact directory.

Legacy non-compositing hosts and Mac Catalyst receive the shared widget fixes;
this change does not add missing native-platform-view capabilities to those hosts.
Implementation is complete for the listed composition paths; cross-platform
physical/performance qualification remains **PARTIAL**. Build success and automated
input are not claims of sustained refresh rate or physical input-to-display latency.

## Commands

All children used `python Doroti/validation/run-with-timeout.py` (1,200 seconds).
Builds and product gates were sequential. The scoped commands were:

```
dotnet build DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
python Doroti/validation/platform-views/verify-windows-effects.py
pwsh -NoProfile -File Doroti/eng/validate-windows-winui-controls.ps1 -NoBuild -OutputDirectory NEW_DIRECTORY
dotnet build Doroti/src/Doroti.Host.Qt/Doroti.Host.Qt.csproj -c Release
dotnet build Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj -c Release -f net10.0-macos -r osx-arm64
dotnet build Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj -c Release -f net10.0-ios -r ios-arm64 -p:DorotiIosTargetFramework=net10.0-ios -p:DorotiIosMauiVersion=10.0.20
dotnet build Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj
python Doroti/validation/platform-views/verify-web-dom.py
dotnet run --project Doroti/validation/platform-views/RasterMotion/RasterMotion.csproj -c Release
dotnet run --project Doroti/validation/platform-views/Common/Common.csproj -c Release
dotnet build DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64
```
