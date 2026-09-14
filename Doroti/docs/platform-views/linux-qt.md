# Linux Qt PlatformView contract

The Qt Quick backend uses live QML items and Qt-owned Vulkan device/queue/WSI.
Graphite renders private R images and copies to distinct P images sampled by QSG.
There is no Doroti CPU raster upload/readback in this path. Chromium's internal
rendering/import strategy is separately selected by Qt WebEngine; this statement
does not claim that Chromium never copies pixels.

The published P bank is never reused as staging, including frames of unchanged
size. Queue drain precedes reuse; successful copy completion precedes native
batch publication. Cancelled recordings, rejected commits and superseded resize
preserve the previous published bank. Active/staging/retiring R/P allocations
share a 128 MiB guard. Replaced QSG nodes destroy their owned texture wrappers;
removed raster items release wrappers rather than retaining hidden old images.
The basic GUI/render loop and queue drain remain mandatory.

Common composition completion follows Qt's actual `frameSwapped` token.
A new render supersedes a previous token that never swapped before admitting
another frame (including render-only capture passes). Close/scene-graph
invalidation also terminates pending tokens. The copy fence retires Graphite
reads; Qt resource lifetime and next-frame queue drain protect QSG reads.
`frameSwapped` is not a GPU fence or physical scanout receipt, and the common
observation remains `BackendAccepted`. Diagnostic `compositionFrames` records
actual shared-session completions.

Quick supports translation and rectangular clipping with fractional logical
coordinates. Direct native mouse/wheel/key/focus routing follows committed
native/shield paint order; effect items themselves are pass-through. GestureArena
is not advertised. Full Tab traversal, native-origin parent gesture arbitration,
touch/tablet capture, physical Korean IME and Orca qualification remain open.

`doroti/webview` is an optional WebEngine Quick item, never a QWidget or a static
snapshot. Enable `DorotiQtQuick=true` and `DorotiQtWebEngine=true`, register the
view type in the application manifest, and rebuild the native shim. The Testbed
enables it by default when Quick is enabled. Initial UTF-8 HTML is limited to
1 MiB; navigation/profile/JS/plugin APIs remain work2 responsibilities. Disabling
WebEngine removes its link dependency and startup initialization. Disabling
Quick selects the separate Widgets B-only backend.

PlatformEffect uses a live `ShaderEffectSource` capturing a group of all earlier
paint-order items, including R/N/R/N. Two GPU shader passes apply normalized,
separable Gaussian weights over ±3 logical sigma. The output is clipped separately
from the expanded sample bounds. The effect and its sharp child remain outside
the source group, with no recursive feedback. Native objects stay alive when
reparented between the source group and the root. Tint is the common sharp
foreground raster; no effect QML items or sample textures remain when the final
native effect disappears. Raster-only scenes continue through common Skia rendering.

Current limits: one bounded isotropic effect, logical sigma ≤32, sample dimensions
≤4,096 physical pixels each and ≤4,194,304 total sample pixels. This bounds two
sample/intermediate textures to roughly 32 MiB (allow 48 MiB for Qt overhead),
in addition to the R/P guard; Qt/Chromium/WSI allocations are not a total-process
memory guarantee. Multiple effects, invalid sample bounds and unsupported
transforms fail before changing the batch. Saturation other than 1 remains
unsupported by the common widget. The finite discrete Gaussian implementation
has not received cross-platform MatchCommon/ExactSigma visual qualification.

The new [validation sources](../../validation/linux-qt-quick/README.md) separate
managed/native/GPU contracts, live product input/captures, and physical tests.
Current VM observations use llvmpipe, not a physical GPU performance baseline.
Rapid XWayland resize reproduced `VUID-VkSwapchainCreateInfoKHR-pNext-07781`
inside Qt WSI: requested extent 757×677 while X surface capabilities still
reported 720×640. This is a remaining Qt swapchain/geometry race; it was not
relabeled as an R/P synchronization success, suppressed, or fixed by changing
the render loop. Device loss, 0/1/4-view workload percentiles, full physical
input/accessibility, distribution and NativeAOT approval remain incomplete.
Linux NativeAOT is currently rejected by the existing runner policy.

Implementation references: [Qt ShaderEffectSource](https://doc.qt.io/qt-6/qml-qtquick-shadereffectsource.html)
defines live source capture and recursion/input behavior;
[QtWebEngineQuick initialization](https://doc.qt.io/qt-6/qtwebenginequick.html)
must precede QApplication; [Qt WebEngine graphics configuration](https://doc.qt.io/qt-6/qtwebengine-features.html)
describes its separate Chromium GPU backend and import constraints.
