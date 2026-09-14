# PlatformView support matrix

2026-09-14. Overall work1 status: **PARTIAL**.

| Runner / attachment | Source/build in this execution | Runtime evidence | Remaining |
|---|---|---|---|
| WindowsAppSdk Vulkan BUTTON/EDIT HWND | migrated planner/session; build passed | existing mounted gallery regression passed with OS SendInput, overlap/foreground click, focus and wheel, two navigation cycles, clean exit | native backdrop unsupported; full IME/UIA/device-loss/AOT/performance |
| WindowsAppSdk WebView2 CompositionController | new actual visual attachment, common effect, mouse/capture/shield, clipped raster upload; build passed | mounted blur on/off, live source, sharp child, input pass/block, two WebViews, effect movement, disposal/recreation; see execution report for final resize run | pen/touch, full Tab/IME/UIA, protected/media source coverage, GPU-sharing comparison, full performance/AOT |
| Android Button/EditText/WebView | shared session/effect policy, native WebView factory; arm64 host build passed | no new device execution | WebView live sampling, native-origin gesture arena, visual similarity, device lifecycle/performance |
| AppKit NSView/Metal | shared session with existing GPU frame retirement; macOS host source build passed | not run | native WKWebView/effect adapter, current product qualification |
| Qt Quick | live WebEngine Quick attachment, bounded GPU Gaussian effect, fractional placement, published/staging image banks, actual Qt terminal connected to common session; Testbed and optional template builds | llvmpipe: XWayland and native Wayland WebView/effect product gates passed (19 checks each); real Qt queue cancellation/retirement and native owner/lifetime contracts passed | rapid XWayland Qt WSI extent race remains; full gestures/physical IME/Orca/device-loss/performance/visual matching/deployment |
| Qt Widgets | separate native-child B-only adapter build; managed ABI and rounded/inward geometry checks passed | XWayland llvmpipe mounted QPushButton/QLineEdit, 20 resize cycles and clean exit; Quick evidence does not apply | interleaving, WebEngine Quick and effects unsupported; physical/performance qualification remains open |
| Web DOM | protocol v2 effect adapter and Web host build passed | headless DOM harness passed eight checks | main-DOM/Worker product activation, multi-canvas ACK/resource protocol, live iframe effect pixels |
| Windows MAUI | host build passed with common contracts | not run | separate native hierarchy and WebView composition wiring; WindowsAppSdk evidence does not apply |
| UIKit / Catalyst | common contracts compile through shared projects; no new native adapter | not run | UIView/WKWebView/effect/session/input implementation and runner build/run |

No build-only entry is a physical, accessibility, NativeAOT, or performance approval. Windows WebView and HWND scenes cannot be mixed within one owner composition frame. Current Windows effect limits are four isotropic regions, logical sigma <=32 and native physical sigma <=128. Backend/driver/OS sampling limitations are not resolved by setting the capability flag.

Qt's [current contract](linux-qt.md) and [reproduction commands](../../validation/linux-qt-quick/README.md)
describe its separate limits: one isotropic effect, sigma <=32, <=4M physical sample
pixels, and a 128 MiB R/P allocation guard. The Linux work1 scope remains **PARTIAL**.

Current transport limits are defensive allocation guards, not accepted frame-rate budgets. The Windows staging/active atlas estimate is limited to 256 MiB and 16,384 pixels per atlas dimension. CPU readback and upload remain measurable costs. The source keeps the 0-native fast path; effect-free applications do not initialize a WebView compositor or allocate its D3D/D2D resources.
