# Public UIKit blur tuning — 2026-09-21

The user selected public APIs and requested a React Native comparison. The
[research note](../../../docs/platform-views/ios-blur-research-2026-09-21.md) separates
Expo's public material interpolation, community blur's internal effect settings,
and Skia-only backdrop filtering.

## Implementation

- Fixed Light UIKit material remains. Animator fraction now uses requested logical
  sigma / 30, replacing sigma / 16. It is still approximate MatchCommon, not
  ExactSigma or native saturation support.
- WebView sample adds `Blur settings`: strength, tint opacity, White/Dark/Blue tint,
  and explicit theme following. Default is fixed white at 20%, with theme following
  off. Doroti owns this overlay independently of UIKit's intrinsic material tint.
- Zero clears the native material. The existing lifetime, native hierarchy,
  input shields, clip and foreground composition paths are preserved.

## Pixel measurements

All captures use the production UIKit adapter over the same reference patterns.
These are whole-window DrawViewHierarchy pixels, not physical display measurements.
Analysis normalizes the black/white edge to measure blur spread separately from
material color bias. The matching tolerance is 1.25 logical points.

| Requested logical sigma | iPhone 18 Pro Simulator, iOS 27.0 | iPhone 12, iOS 26.6.1 |
| --- | --- | --- |
| 2.4 | 2.50 | 2.25 |
| 4 | 4.00 | 4.00 |
| 6 | 6.00 | 6.00 |
| 12 | 12.00 | 12.00 |
| 16 | 16.00 | 15.75 |

All fixed-source Light/Dark mean RGB differences were zero. Decreasing from maximum
to Strength .375 matched the initial .375 image, and setting zero matched the source
(each mean RGB difference below 1, as required by the analyzer).

Actual WKWebView/Metal appearance tests also passed at four strengths in both
themes. The fixed-source effect ROI had zero Light/Dark and Settings-to-app resume
RGB difference, and every adjacent strength produced distinct pixels. All seven
effect/shield/native-identity/remove/recreate scenes passed on the simulator.
The actual Material WebView sample passed its visible calibrated panel and YouTube
Referer/player-loading checks on the phone. Physical slider gestures and video
playback are not covered by these probes.

Intrinsic material tint remains observable: at requested sigma 6, a source gray of
32 becomes approximately 45; at sigma 16 it becomes approximately 68. On the earlier
uncalibrated simulator, Strength .375 measured sigma 11.5 and raised that gray to
57. Thus the corrected mapping reduces overblur and the accompanying material bias,
but it cannot provide a tint-free Gaussian. An authored tint alpha of zero only
removes Doroti's overlay.

## Build and reproduction

Debug/Mono Graphite builds for `iossimulator-arm64` and `ios-arm64`, targeting
`net10.0-ios27.0`, completed with zero warnings/errors. Existing build output roots
under `ios-sample-fixes/build` and `ios-sample-fixes/device-build` were reused;
new logs and measurements are under
`Doroti/artifacts/platform-views/2026-09-21/ios-public-blur-tuning/`.

Run [run-blur-calibration.py](run-blur-calibration.py) with `--simulator` or
`--device`, the corresponding built app, and a fresh output folder. Use a Python
environment with Pillow and invoke it through `validation/run-with-timeout.py`.
Each launch uses a unique capture directory; source captures, analysis and commands
are preserved. The repository's 20-minute process-tree deadline applies.

Release/NativeAOT, every OS/device, and exact native color equivalence are outside
these results. No React Native app was built for the source comparison.
