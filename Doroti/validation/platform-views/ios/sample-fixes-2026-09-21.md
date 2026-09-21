# iOS Testbed sample fixes — 2026-09-21

## Changes

- Platform views used the legacy Gaussian `BackdropFilter` gate. UIKit intentionally
  advertises that as unsupported, while supporting public material interpolation
  through `Capabilities.Effect`. The page now uses `PlatformEffect` / MatchCommon
  when supported, retaining the legacy path for other compatible hosts.
- The WebView sample unnecessarily required `NativeBackdropBlur` in addition to
  the effect capabilities. Removing that requirement admits UIKit's floating panel.
- UIKit loaded local HTML with a null base URL. It now supplies
  `https://<lowercase bundle identifier>/`, enables inline media playback, and the
  sample requests `playsinline=1`. This follows the
  [YouTube app identity guidance](https://developers.google.com/youtube/terms/required-minimum-functionality#api-client-identity-and-credentials).
  No ATS exceptions or internet permission changes were needed.

## Validation

Both Debug/Mono Graphite builds used `net10.0-ios27.0`, `-m:1`, and the repository's
1200-second timeout wrapper. Simulator and device builds completed with zero warnings
and zero errors. This is not Release/NativeAOT qualification.

| Check | iPhone 18 Pro Simulator / iOS 27.0 | iPhone 12 / iOS 26.6.1 |
| --- | --- | --- |
| Platform-view case 5 contains a live UIKit effect at strength .375 | PASS | PASS |
| Ten overlap cases, native identity/state, shield hit tests, focus/text insertion | PASS | PASS |
| Ten dispose/recreate cycles | PASS | PASS |
| Actual Material WebView page has a visible effect at strength .625 | PASS | PASS |
| HTML base URL and YouTube iframe `document.referrer` | `https://dev.doroti.testbed/` | `https://dev.doroti.testbed/` |
| YouTube iframe player exists and has no displayed configuration error | PASS | PASS |

Simulator screenshots were inspected: the Platform views foreground blurs the
native editor/checkerboard, and the WebView floating panel blurs the HTML underneath
while the YouTube player thumbnail loads. Device checks are programmatic hierarchy,
input, and in-frame JavaScript checks. Physical touch, video playback, and device
pixel comparison are not asserted.

The first WebView probe timed out because `WKWebView.Reload()` tried to navigate to
the synthetic base URL. The probe was corrected to reload its captured local HTML
with the same base URL; both subsequent simulator and device checks passed. The
sample's Reset page action already recreates the fallback view and is unaffected.

Artifacts: `Doroti/artifacts/platform-views/2026-09-21/ios-sample-fixes/`, including
build logs, simulator screenshots/results, and `device-webview/` /
`device-platform-views/` command logs and results. The connected phone has the
updated Debug build installed; it was returned to the WebView sample with evidence
collection disabled.

Reproduce the device checks with [run-probe.py](run-probe.py), using modes
`platform-views` and `webview-sample`; see [README](README.md) for build commands.
