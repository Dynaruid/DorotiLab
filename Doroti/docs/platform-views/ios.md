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

## Flutter-style Gaussian in C#

The user selected internal Gaussian control on 2026-09-17 and subsequently asked
for a .NET iOS implementation. `UIKitPlatformBlurView.cs` contains both the effect
view and `UIKitGaussianFilter`; there is no custom Objective-C source, P/Invoke
bridge or static-library build step. It uses bound UIKit/Foundation APIs:

- Find UIKit-created backdrop/effect subviews by their native class names.
- Read the layer's filter array through KVC and locate a Gaussian filter with a
  numeric radius and copy support.
- Copy it, set `inputRadius`, verify the value, and retain only that filter.
- Clear the internal material background. Explicit PlatformEffect tint and the
  authored, sharp child remain in the following raster segment.

This follows Flutter's approach and depends on undocumented UIKit structure even
though the calling code is C#. It does not instantiate a private class. The view
pins only its effect appearance to Light; the native content and app retain their
own appearance. Reapplication after layout, attachment, appearance and activation
handles UIKit rebuilding its recipe. Effect/superview alpha stays at 1.

The isolated structural probe supplies the advertised effect capability. Missing
structure/filter or a converted Objective-C exception returns an unsupported
reason. A failed live reapplication hides the effect and disables the capability;
the commit path rejects failures instead of presenting the default material as a
successful Gaussian. Mutation exceptions attempt to restore the old filter array
and colour. The view removes its activation observer when disposed.

Foundation bindings rely on .NET iOS's default Objective-C exception conversion
(`MarshalObjectiveCExceptionMode=throwmanagedexception`). The transitive target
rejects explicit incompatible settings. A normal C# catch around raw objc_msgSend
would not be an equivalent replacement. The opt-in calibration deliberately uses
an unknown KVC key and requires an `ObjCException` catch before visual validation.

One isotropic MatchCommon effect with logical sigma <=16 is admitted. Radius is
set directly from logical sigma, but ExactSigma, generic Gaussian BackdropFilter
and non-default saturation remain unqualified. Reduce Transparency requires an
explicit SolidTint alternative. UIVibrancyEffect is not applied to common content:
it changes foreground colour treatment rather than exposing blur radius.

## Evidence boundaries

The former public material/animator attempt failed the adjacent-strength pixel
gate. Its logs remain in `artifacts/platform-views/2026-09-17/ios/blur-match/`.
The internal Gaussian implementation is tracked separately in `ios/flutter-blur/`;
the initial Objective-C bridge and final C# builds/captures are separate evidence.
See [validation commands](../../validation/platform-views/ios/README.md).

Final C# evidence on iPhone 12 / iOS 26.6.1: four strength/theme captures and seven
functional scenes pass; measured sigma 4.25/6.25/11.5/15 at requested 4/6/12/16,
with Gaussian-reference RGB MAE 1.29/1.10/0.78/0.64 out of 255. The isolated pattern
has zero theme difference. NativeAOT also catches the deliberate KVC exception.
iOS 26.5 Simulator captures and functional scenes are recorded separately. The
earlier iOS 27 startup failure and SDK mismatch were subsequently addressed by
the Scene lifecycle and iOS 27 profile below.

Native-origin GestureArena interception, full Tab/Korean IME/VoiceOver, protected
media, two product owners, device loss, full E3 pixel qualification and complete
performance budgets remain open. Physical input and screenshot comparison are
separate from programmatic hierarchy/input tests. An OS-specific pass does not
promise future UIKit compatibility or App Store acceptance.

Sources:
- [Flutter internal filter implementation](https://api.flutter.dev/ios-embedder/_flutter_platform_views_8mm_source.html)
- [.NET iOS exception marshaling](https://learn.microsoft.com/en-us/dotnet/ios/advanced-concepts/exception-marshaling)
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
Mono and the matching Host.Maui TFM. Existing net11 NativeAOT selection remains
separate. Device and both simulator target manifests identify the iOS 27 TFM.
The SDK pack used was 27.0.10539-xcode27.0 with Xcode 27.0; only its preview notice
is suppressed. ValidateXcodeVersion is not disabled for this profile.

On iPhone 18 Pro / iOS 27 Simulator, startup, four-strength/two-theme blur pixels,
seven native lifecycle/input scenes, and Settings-to-app resume passed. The same
process resumed, blur ROI difference was zero, and later strength changes rendered.
iPhone 12 / iOS 26.6.1 passed the same Scene startup and blur checks using the
new SDK build. These are Debug/Mono interpreter results. iOS 27 physical hardware,
NativeAOT, multi-scene and distribution qualification remain separate.

See [Apple's Scene migration contract](https://developer.apple.com/documentation/uikit/transitioning-to-the-uikit-scene-based-life-cycle)
and the [Xcode 27 SDK release](https://github.com/dotnet/macios/releases/tag/dotnet-10.0.1xx-xcode27.0-P2-10539).
