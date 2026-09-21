# React Native blur research and Doroti application — 2026-09-21

The selected constraint is public iOS APIs. Representative React Native libraries
do not provide a public API that independently controls live UIKit backdrop sigma,
saturation and native material tint.

## Source findings

| Implementation | iOS mechanism | Relevance |
| --- | --- | --- |
| [Expo BlurEffectView](https://github.com/expo/expo/blob/main/packages/expo-blur/ios/BlurEffectView.swift) | UIVisualEffectView / UIBlurEffect, interpolated using UIViewPropertyAnimator.fractionComplete | Same basic mechanism already used by Doroti. Its intensity interpolates the whole material. |
| [community BlurEffectWithAmount](https://github.com/Kureev/react-native-blur/blob/master/ios/BlurEffectWithAmount.m) | Changes the runtime class of a UIBlurEffect and overrides undocumented effectSettings, setting blurRadius through KVC | Direct radius control depends on UIKit internals; excluded by the selected public-API constraint. It still starts with a native material. |
| [React Native Skia backdrop filters](https://shopify.github.io/react-native-skia/docs/backdrops-filters/) | Skia image filters and clipping inside the canvas rendering path | Useful for content rendered by Skia. This is not evidence that an arbitrary live sibling WKWebView can be sampled; replacing Doroti's native compositor with it would not solve that boundary. |

Expo's [TintStyle mapping](https://github.com/expo/expo/blob/main/packages/expo-blur/ios/TintStyle.swift)
maps `tint` to a UIBlurEffect.Style, rather than an independent RGBA overlay.
Its [current documentation](https://docs.expo.dev/versions/latest/sdk/blur-view/#tint)
explicitly says every tint adds a translucent color layer and no value renders blur
alone. Therefore a `tint="none"` option cannot be borrowed from Expo.

Expo documents [blurReductionFactor](https://docs.expo.dev/versions/latest/sdk/blur-view/#blurreductionfactor)
to compensate Android intensity relative to iOS. The useful transferable idea is
backend-specific calibration, not its numerical divisor (4), which applies to a
different renderer. These are source/documentation findings; no React Native app
was built or pixel-tested for this comparison.

## Doroti changes

1. Preserve the public animator and fixed Light effect. Previous captures measured
   approximately `sigma = 30 * fraction`, but the implementation used `sigma / 16`.
   The corrected mapping is `fraction = requestedSigma / 30`, with the existing
   logical sigma range 0–16. UIKit's intrinsic material bias remains, but lowering
   the fraction also reduces its contribution. This is calibrated MatchCommon,
   not an ExactSigma claim or a guarantee for every future OS.
2. Keep Doroti's ARGB tint in its own foreground raster layer. The WebView sample
   adds independent strength, tint opacity, tint color and explicit theme-following
   controls. Default tint is fixed white at 20%, with theme following off; previously
   it automatically switched between 60% white and dark tint.
3. Keep foreground labels/icons legible using the chosen tint palette. The WebView's
   HTML can still change with the system theme; different source pixels naturally
   produce different blur pixels. Theme independence is tested with a fixed source.
4. Verify the effective blur using edge-spread measurements, independently of the
   animator's reported fraction. Also verify zero/reset, decreasing strength,
   identical-source Light/Dark, and actual WKWebView/Metal lifecycle behavior.

Existing baseline: `artifacts/platform-views/2026-09-18/ios/public-blur/gc-fix/aot-run-3/captures/analysis.json`.
For Strength .25/.375/.75/1, the old measured sigmas were 7.5/11.5/22.5/29.25;
the common requested sigmas are 4/6/12/16. The old fixed-source Light/Dark RGB
difference was already zero: the reported theme difference is consistent with the
sample's authored tint and HTML changing, rather than evidence of an adaptive
material in the current fixed-Light adapter.

Updated measurements are recorded separately from this research in the validation
results. Native saturation control and tint-free UIKit backdrops remain unsupported.
