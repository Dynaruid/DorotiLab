# PlatformView support matrix

2026-09-20. Overall work1 status: **PARTIAL**. Updates: [Windows](windows-webview.md), [Android](android-webview.md).

| Runner / attachment | Source/build in this execution | Runtime evidence | Remaining |
|---|---|---|---|
| WindowsAppSdk Vulkan BUTTON/EDIT HWND | planner/session, one visible DirectComposition tree with live cloaked HWND sources and backdrop; Release build passed | mounted blur on/off, native repaint, clip/resize restoration, 10 overlap cases, disposal/recreation; gallery OS SendInput clicks/focus/wheel; three scroll cycles captured through DXGI: 553 unique frames, zero missing-content or edge-separation frames | full IME/UIA/device-loss/AOT/performance; multiple overlapping backdrop qualification |
| WindowsAppSdk WebView2 CompositionController | shared controller/widget, navigation/JS/profile/content/message, Gaussian/saturation/tint, pointer forwarding, bounded upload; Release build/publish | public API product tests including origin/profile/late results, OS-injected touch and mouse/shield, two WebViews/resize/recreation; native/raster sigma 4/16 measured 3.989/15.947, reset difference 0 | physical touch/pen, full Tab/IME/UIA, same-process two owners, media/protected coverage, policies, GPU-sharing comparison, performance/AOT; Windows MAUI unconnected |
| Android Button/EditText/WebView | shared controller/instance/session; navigation/JS/profile/content/messages; calibrated RenderEffect sigma, saturation and tint; arm64/x64 Release Mono AOT APKs | Galaxy API36/provider151 API, 11-stage pixels and 12 input/lifecycle assertions; final x64/provider133 API and 13-stage pixels including color-after-blur; final Galaxy color refinement check skippedByUser | delayed native-origin GestureArena, full C/E/physical/TalkBack/two owners/loss/media; performance and GPU/HCPP comparison, clean template/AOT; [Android contract](android-webview.md) |
| AppKit WKWebView/NSView/Metal | common session, GPU retirement leases, public Core Image Gaussian/color backdrop, controller/widget/navigation/JS/profile/content/message; Xcode 27 profile | product scenes plus separate native/Metal spatial edge calibration, zero/reset, saturation and tint against native compositing reference; current evidence in macos/custom-blur | one isotropic rectangular effect, sigma 0–64, saturation 0–2; linear-color Gaussian differs from default Skia encoded-color pixels; full E3/two owners, physical IME/VoiceOver, loss/performance/AOT remain; [AppKit contract](macos.md) |
| Qt Quick | live WebEngine Quick attachment, bounded GPU Gaussian effect, fractional placement, published/staging image banks, actual Qt terminal connected to common session; Testbed and optional template builds | llvmpipe: XWayland and native Wayland WebView/effect product gates passed (19 checks each); real Qt queue cancellation/retirement and native owner/lifetime contracts passed | rapid XWayland Qt WSI extent race remains; full gestures/physical IME/Orca/device-loss/performance/visual matching/deployment |
| Qt Widgets | separate native-child B-only adapter build; managed ABI and rounded/inward geometry checks passed | XWayland llvmpipe mounted QPushButton/QLineEdit, 20 resize cycles and clean exit; Quick evidence does not apply | interleaving, WebEngine Quick and effects unsupported; physical/performance qualification remains open |
| Web DOM | protocol v2 effect adapter and Web host build passed | headless DOM harness passed eight checks | main-DOM/Worker product activation, multi-canvas ACK/resource protocol, live iframe effect pixels |
| Windows MAUI | host build passed with common contracts | not run | separate native hierarchy and WebView composition wiring; WindowsAppSdk evidence does not apply |
| iOS UIKit Graphite Metal | UIView/UIButton/UITextField/WKWebView, transparent Metal segments, common session, public UIKit property-animator material intensity and committed shields; current iOS 27 Debug/Mono simulator build passed; host AOT avoids the full-interpreter calibration GC crash | Current public adapter: four-strength/two-theme pixel checks, zero resume ROI difference and seven effect/input/lifetime scenes plus three full calibrations (zero/decreasing intensity) on iPhone 18 Pro Simulator; earlier private-filter NativeAOT/device evidence is historical | residual UIKit colour bias, other OS/device calibration, native-origin GestureArena, full IME/VoiceOver/Tab, two product owners, device loss, visual equivalence and performance budgets; see [iOS contract](ios.md) |
| Catalyst / iOS Ganesh | existing runner; new UIKit adapter is iOS Graphite only | no new PlatformView execution | separate adapter and qualification; iOS Graphite evidence does not apply |

iOS follow-up (2026-09-18): the current blur uses public `UIBlurEffect` and
`UIViewPropertyAnimator.FractionComplete`, following `ref.md`. Strength is UIKit
material intensity, not an exact Gaussian radius; preset tint remains. Previous
private-filter captures in `ios/flutter-blur/managed-*` and iOS 27 Scene regression
results are historical evidence. Current animator iOS 27 Simulator strength/theme/resume and seven functional
scenes passed; physical-device/NativeAOT/distribution qualification remains open.
See [work1.md](../../../work1.md) for measured gates and remaining scope.

No build-only entry is a physical, accessibility, NativeAOT, or performance approval. Windows WebView and HWND scenes cannot be mixed within one owner composition frame. Current Windows effect limits are four isotropic regions, logical sigma <=32 and native physical sigma <=128. Backend/driver/OS sampling limitations are not resolved by setting the capability flag.

Qt's [current contract](linux-qt.md) and [reproduction commands](../../validation/linux-qt-quick/README.md)
describe its separate limits: one isotropic effect, sigma <=32, <=4M physical sample
pixels, and a 128 MiB R/P allocation guard. The Linux work1 scope remains **PARTIAL**.

Current transport limits are defensive allocation guards, not accepted frame-rate budgets. The Windows staging/active atlas estimate is limited to 256 MiB and 16,384 pixels per atlas dimension. CPU readback and upload remain measurable costs. The source keeps the 0-native fast path; effect-free applications do not initialize a WebView compositor or allocate its D3D/D2D resources.
