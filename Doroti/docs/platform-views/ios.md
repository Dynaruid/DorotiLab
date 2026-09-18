# iOS UIKit platform composition

The adapter is limited to the Graphite Metal runner. Catalyst and Ganesh are
separate runners. UIButton, UITextField and WKWebView instances remain in an
owner-local clipped UIView alongside the MTKView. Transparent CAMetalLayer
segments carry intervening/foreground Doroti pixels. Committed native, raster,
effect and shield order agree; transparent effect/raster views do not take input.

The common PlatformCompositionSession owns admitted frame lifetime. Native leases
also protect submissions rejected before admission. A same-queue Metal terminal
retires drawables and leases, including cancellation after submission. Native
composition permits one frame in flight so an idle update cannot replay an old
scene over a new one. The ordinary raster path permits three. Estimated foreground
triple-buffered storage is capped at 256 MiB; this is not a performance approval.
Core Animation transaction presentation waits for scheduling, not GPU completion.
Backend acceptance is not physical atomic-display acknowledgement.

WKWebView retains its real native hierarchy and initial HTML. Navigation, JS,
profiles, downloads and permission APIs remain work2. Snapshot readback does not
replace the native source.

## Public UIKit blur intensity

`UIKitPlatformBlurView.cs` uses `UIVisualEffectView`, a fixed Light `UIBlurEffect`,
and a retained, paused `UIViewPropertyAnimator`. The common logical sigma [0,16]
is divided by 16 to recover `PlatformEffectStyle.Strength` [0,1], which drives
`FractionComplete`. Zero clears the effect. This interpolates UIKit's whole
material recipe, including its tint; it does not set a Gaussian radius. Explicit
PlatformEffect tint and the authored sharp child remain in the following raster
segment. Effect/superview alpha stays at 1.

Strength updates scrub the existing animator. Bounds changes, window attachment,
appearance changes and application/scene activation recreate the interpolation
from no effect. Ordinary layout passes with unchanged bounds leave it intact.
Disposal stops/releases the animator and removes both activation observers.
There is no private subview inspection, KVC filter mutation, or blur-specific
Objective-C exception-marshaling requirement.

One isotropic MatchCommon effect with logical sigma <=16 is admitted as a common
visual intent. ExactSigma, generic Gaussian BackdropFilter and non-default
saturation are unsupported. Reduce Transparency requires an explicit SolidTint
alternative. The fixed Light appearance preserves theme-independent treatment;
it cannot remove the preset's intrinsic colour bias through public APIs.

## Evidence boundaries

The previous private Gaussian implementation and earlier public-preset experiments
have historical evidence in `ios/flutter-blur/` and `ios/blur-match/`. Their radius
measurements and pixel passes do not qualify the current public animator adapter.
The calibration probe now captures the production adapter at multiple strengths,
including decreasing from full strength and clearing to zero. See
[validation commands](../../validation/platform-views/ios/README.md).

Current public-adapter validation (2026-09-18): the Debug/Mono iOS 27 simulator
build passed with zero warnings/errors. On iPhone 18 Pro Simulator, four strengths
(.25/.375/.75/1) produced distinct pixels in the WKWebView/Metal fixture. Parent
Light/Dark and same-process Settings-to-app resume each had zero RGB difference
in the effect ROI. All seven effect/input/lifetime scenes passed. Captures and
reports are under `artifacts/platform-views/2026-09-18/ios/public-blur/appearance/`.
These results do not qualify physical devices, NativeAOT, or exact Gaussian matching.

The initial full-interpreter calibration crashed in Mono GC (`copy_object_no_checks`).
`MONO_GC_DEBUG=check-remset-consistency,verify-before-collections` subsequently
reported invalid Frame/Drawable references in the Metal host's `PendingFrame`.
The iOS 27 Debug profile now defaults to `MtouchInterpreter=all,-Doroti.Host.Maui`:
the host uses Mono AOT while the remaining assemblies retain the interpreter.
This is a scoped workaround for the observed execution path, not an upstream
runtime fix. Explicit interpreter settings, Release, and NativeAOT are unchanged.

With the host compiled, three consecutive calibration runs completed (two with GC
verification enabled). Both themes had zero RGB difference for resetting intensity
to zero versus the source and for decreasing from 1 to .375 versus an initial .375.
Evidence is under `public-blur/gc-fix/aot-run-{1,2,3}/`; initial crash evidence stays
under `public-blur/calibration/`. The default-profile rebuild also passed with
zero warnings/errors, followed by the four-strength/two-theme, Settings resume,
and seven functional scenes in `public-blur/gc-fix/appearance/`. Gaussian matching
remains approximate: the fixed Light preset includes tint and measured sigma is
about 30 times strength here.

Native-origin GestureArena interception, full Tab/Korean IME/VoiceOver, protected
media, two product owners, device loss, full E3 pixel qualification and complete
performance budgets remain open. Physical input and screenshot comparison are
separate from programmatic hierarchy/input tests. An OS-specific pass does not
promise future UIKit compatibility or App Store acceptance.

Sources:
- [Expo public blur interpolation](https://github.com/expo/expo/blob/main/packages/expo-blur/ios/BlurEffectView.swift)
- [Per-assembly Mono interpreter/AOT configuration](https://learn.microsoft.com/en-us/dotnet/maui/macios/interpreter)
- [.NET UIViewPropertyAnimator](https://learn.microsoft.com/en-us/dotnet/api/uikit.uiviewpropertyanimator)
- [Visual effect view and alpha constraints](https://developer.apple.com/documentation/uikit/uivisualeffectview)
- [Transaction presentation](https://developer.apple.com/documentation/quartzcore/cametallayer/presentswithtransaction)

## iOS 27 Scene lifecycle and build profile

`DorotiMauiSceneDelegate` derives from MAUI's scene delegate. The app delegate
selects it through a direct type reference, and both Testbed and app template
include the single-window UIApplicationSceneManifest. MAUI owns window creation.
Metal suspends/resumes against its own WindowScene activation state, and the blur
view reapplies its recipe on that scene's activation. Observer removal stays with
host/view disposal. Legacy windows retain application-notification handling.

From the workspace root (.NET 10 SDK), select
`-p:DorotiIosTargetFramework=net10.0-ios27.0`. The shared/template profile chooses
Mono and the matching Host.Maui TFM; Debug AOT-compiles Host.Maui to avoid the
calibration GC crash described above. Existing net11 NativeAOT selection remains
separate. Device and both simulator target manifests identify the iOS 27 TFM.
The SDK pack used was 27.0.10539-xcode27.0 with Xcode 27.0; only its preview notice
is suppressed. ValidateXcodeVersion is not disabled for this profile.

Before the public animator replacement, on iPhone 18 Pro / iOS 27 Simulator, startup, four-strength/two-theme blur pixels,
seven native lifecycle/input scenes, and Settings-to-app resume passed. The same
process resumed, blur ROI difference was zero, and later strength changes rendered.
iPhone 12 / iOS 26.6.1 passed the same Scene startup and blur checks using the
new SDK build. These are Debug/Mono interpreter results. iOS 27 physical hardware,
NativeAOT, multi-scene and distribution qualification remain separate.

See [Apple's Scene migration contract](https://developer.apple.com/documentation/uikit/transitioning-to-the-uikit-scene-based-life-cycle)
and the [Xcode 27 SDK release](https://github.com/dotnet/macios/releases/tag/dotnet-10.0.1xx-xcode27.0-P2-10539).
