# Linux Qt PlatformView contract

Host ABI 6 adds framework preparation before QRhi admission and Qt consumer
retirement after `afterFrameEnd`; see the [native pipeline](../native-frame-pipeline.md).
Sample2, Testbed and template shims must be rebuilt together. The basic render
loop remains required. Software Vulkan evidence is separate from hardware and
physical-input qualification.

The Qt Quick backend uses live QML items and Qt-owned Vulkan device/queue/WSI.
Graphite renders private R images and copies to distinct P images sampled by QSG.
There is no Doroti CPU raster upload/readback in this path. Chromium's internal
rendering/import strategy is separately selected by Qt WebEngine; this statement
does not claim that Chromium never copies pixels.

The published P bank is never reused as staging, including frames of unchanged
size. Producer/copy fences retire recordings and `afterFrameEnd` submits a
same-queue fence covering Qt's sampling. A replaced bank is reused only after
all its consumers complete. Shader-only C publication follows copy submission;
the same-queue barriers order Qt sampling after the copy. Serial/native frames
wait for copy completion before batch publication. Cancelled recordings,
rejected commits and superseded resize
preserve the previous published bank. Active/staging/retiring R/P allocations
share a 128 MiB guard. Replaced QSG nodes destroy their owned texture wrappers;
removed raster items release wrappers rather than retaining hidden old images.
The basic GUI/render loop remains mandatory. Normal C frames use nonblocking
fence polls; queue idle is reserved for failed-submission cleanup and teardown.
GPU failure is terminal for the current Quick generation. A failed idle/fence
wait records the Vulkan result, operation, owner thread and frame token; no new
frame is admitted. If completion cannot be proven, the managed generation keeps
its GPU allocations rooted until process exit rather than freeing memory that
Qt or the driver could still use. The Qt-owned device and instance are never
destroyed by Doroti. A host timer cannot interrupt a driver call that never
returns; the validation driver has an outer process-tree deadline.

Common composition completion follows Qt's actual `frameSwapped` token.
A new render supersedes a previous token that never swapped before admitting
another frame (including render-only capture passes). Close/scene-graph
invalidation also terminates pending tokens. The copy fence retires Graphite
reads; the separate Qt consumer fence protects QSG reads.
`frameSwapped` is not a GPU fence or physical scanout receipt, and the common
observation remains `BackendAccepted`. Diagnostic `compositionFrames` records
actual shared-session completions.

Quick supports translation and rectangular clipping with fractional logical
coordinates. Direct native mouse/wheel/key/focus routing follows committed
native/shield paint order; effect items themselves are pass-through. Mouse
capture persists through the final button release. Touch ownership is assigned
per device at the first point and tablet ownership per device at press, then
retained to end/cancel. Removing or hiding a control, window deactivation and
close clear capture. Mixed native/framework points in one touch event and
native-origin parent GestureArena arbitration are unsupported. Full Tab
traversal, physical touch/tablet, Korean IME and Orca qualification remain open.
Physical keys use Qt's xcb/Wayland XKB scan code (evdev code plus eight) for
USB HID mapping; scan-code-zero synthetic events use a Qt-key fallback. This
distinguishes left/right modifiers and keypad Enter. Actual layout and device
results must be recorded separately from the managed key contract. The offset
is visible in Qt 6.10.2's [Wayland input code](https://github.com/qt/qtbase/blob/v6.10.2/src/plugins/platforms/wayland/qwaylandinputdevice.cpp)
and xcb passes its [XKB keycode](https://github.com/qt/qtbase/blob/v6.10.2/src/plugins/platforms/xcb/qxcbkeyboard.cpp)
through the native scan-code field.

Doroti semantics are exposed through a Qt accessibility root on the surface.
The current state and event mapping is:

| Doroti semantics | Qt accessibility | Change notification |
| --- | --- | --- |
| `focused`, `enabled`, `hidden` | `focused`, `disabled`, `invisible` | `StateChanged`; focus gain also sends `Focus` |
| `selected` | `selectable`, `selected` | `StateChanged` |
| `checkedState`, `toggled` | checkbox role, `checkable`, `checked`, mixed state | `StateChanged` |
| `expanded` | `expandable`, `expanded`, `collapsed` | `StateChanged` |
| `textField`, `readOnly`, `obscured`, `multiline` | text role, edit/password/multiline state; Text and writable EditableText interfaces | text update or value change |
| `textSelectionBase`, `textSelectionExtent` | selection and caret offsets | `TextSelection` |
| node label, children or parent | name and child hierarchy | `NameChanged`; `ObjectReorder` only for hierarchy changes |

Editable text actions dispatch back to the framework; a password value is not
returned. Character rectangles and hit-test offsets are not available from the
current semantics payload. The native Quick/WebView accessibility descendants
have not been shown to connect to this root, and Orca reading remains unverified.

Host ABI 6 requires its documented mask and function pointers. Feature bit 19
advertises the optional `doroti_qt_request_focus_v2` export; a request asks Qt
to activate the window, while only a subsequent Qt focus callback changes the
framework's observed window focus. A compositor may refuse activation. Bit 20
allows a known empty framework scene at the already published pixel extent to
end without another timer retry. Contention, first frame and resize still retry;
older shims continue to use the prior retry result. Host
ABI struct sizes and version remain fixed; unknown additive feature bits are
ignored after required bits are checked. PlatformView ABI 1 likewise requires
bit 0, treats bits 1–4 as optional capabilities and ignores unknown additive
bits. WebView ABI 1 requires bits 0–4 for its current public API and ignores
unknown additive bits. Removing or reinterpreting a bit requires a new ABI
version. Native WebView statuses retain status, operation and owner in
`WebViewException`: 64/72 map to `InvalidRequest`, 71/73 to `Closed`,
69/70/80–83 to `ProcessFailed`, and unsupported/unknown statuses to
`Unsupported`.

`doroti/webview` is an optional WebEngine Quick item, never a QWidget or a static
snapshot. Quick is the runner/template default. Enable `DorotiQtWebEngine=true`, register the
view type in the application manifest, and rebuild the native shim. The Testbed
enables it by default when Quick is enabled. The same native item now implements
the public controller, navigation/JSON JS, profiles and trusted manifest app content;
see [the Linux WebView contract](linux-webview.md) for limits and typed errors. Disabling
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
transforms fail before changing the batch. PV feature bit 4 negotiates color-backdrop
part kind 4 without changing its 96-byte layout: sigma is in id, saturation is in
image, both IEEE754 doubles. Legacy kind 3 retains saturation=1. The vertical pass
applies common luminance coefficients after both blur passes, supports saturation
0–2 and sigma zero, and clamps premultiplied color. Independent tint stays in the
sharp foreground. Native/raster edge widths, reset, saturation and tint have live
pixel evidence; full cross-platform color-space equivalence is not claimed.

The maintained [validation sources](../../tests/README.md) separate
managed/native/GPU contracts, live product input/captures, and physical tests.
Current VM observations use llvmpipe, not a physical GPU performance baseline.
Rapid XWayland resize reproduced `VUID-VkSwapchainCreateInfoKHR-pNext-07781`
inside Qt WSI: requested extent 757×677 while X surface capabilities still
reported 720×640. This is a remaining Qt swapchain/geometry race; it was not
relabeled as an R/P synchronization success, suppressed, or fixed by changing
the render loop. Physical device loss, full physical input/accessibility, performance acceptance,
package-only clean-machine distribution and NativeAOT approval remain incomplete.
The 2026-09-23 10-cycle xcb run reproduced the inverse mismatch (requested
720×640, capabilities 757×677) with the validation layer mapped and exit 0.
A Qt-only Quick/Vulkan fixture reproduced that exact mismatch when its sync
callback took 50 ms, without Doroti or WebEngine. An undelayed 10-cycle fixture
did not reproduce it. The Qt/XWayland WSI issue remains open; this evidence
does not justify removing the queue drain or declaring a Qt version workaround.
The [2026-09-24 follow-up](../../../history/26-09-24/linux-qt-improvements-summary.md)
reproduced the same extent VUID in the Qt-only xcb/XWayland fixture after
matching the product's Vulkan instance extensions. Delaying resize delivery
by 10/30/60 ms did not prevent it. A mapped-layer native Wayland product run
passed ten resizes in this VM; an xcb run also passed once, which does not
resolve the intermittent xcb failure. On Wayland sessions, use
`QT_QPA_PLATFORM=wayland` rather than forcing xcb while this Qt/WSI issue is
open. This is a backend workaround, not xcb Vulkan qualification.
The 2026-09-20 report includes bounded 0/1/4-view interval/PSS observations,
relocated Release runs and synthetic Korean composition/native Tab/focus return.
Linux NativeAOT is currently rejected by the existing runner policy.

Implementation references: [Qt ShaderEffectSource](https://doc.qt.io/qt-6/qml-qtquick-shadereffectsource.html)
defines live source capture and recursion/input behavior;
[QtWebEngineQuick initialization](https://doc.qt.io/qt-6/qtwebenginequick.html)
must precede QApplication; [Qt WebEngine graphics configuration](https://doc.qt.io/qt-6/qtwebengine-features.html)
describes its separate Chromium GPU backend and import constraints.

## Build configuration

The runner SDK, Sample2, Testbed and new app template select Quick/Graphite
Vulkan by default. WebEngine is optional in the SDK/template and enabled in the
two samples. GStreamer textures remain optional. Use these build profiles:

| Backend | MSBuild properties | Minimum Qt API | Runtime selection |
| --- | --- | --- | --- |
| Quick / Graphite Vulkan | defaults | 6.6 | default C, basic render loop |
| Quick with WebView | `DorotiQtWebEngine=true` | 6.8 | default C, native scenes serial |
| Widgets / Vulkan window | `DorotiQtQuick=false`, `DorotiQtWebEngine=false` | 6.5 | separate Vulkan-window contract |
| Widgets / OpenGL comparison | the Widgets properties plus `DorotiQtGraphite=false` | 6.5 | `DOROTI_LINUX_GRAPHITE=0`, serial; Wayland remains unqualified |

Quick requires Core/Gui/Widgets/Quick/Qml/QuickControls2 development libraries
and the QtQuick/QtQuick.Controls runtime QML modules. WebEngine adds
WebEngineQuick/WebChannel, their QML modules and system Chromium helpers/data.
Only the OpenGL comparison directly requires the Qt OpenGL component. All
profiles currently require Wayland client development files and
`wayland-scanner`, including xcb builds. Vulkan profiles require Vulkan headers
and a Vulkan 1.2 device; software Vulkan is accepted separately from hardware
performance qualification. Use the native Wayland QPA on Wayland sessions.

Native build caches are separated by configuration and Quick/Graphite/
WebEngine/GStreamer choices. Switching options recompiles the selected shim
and removes disabled optional libraries from build/publish output. Invalid
boolean values, Quick without Graphite, WebEngine without Quick, and the Linux
Desktop adapter without Quick fail with `DOROTIQT002`–`DOROTIQT005`.
`DOROTIQT006` rejects missing native output, including `publish --no-build`
for a profile that has never been built. With `DorotiBuildQtNative=false`, set
`DorotiQtNativeBuildDirectory` to a matching prebuilt directory containing the
host and every enabled optional shim/manifest. Rebuild older app-owned native
sources together with managed ABI 6; an existing directory name is not proof
of ABI or backend compatibility.


## 2026-09-29 host integration

Qt built-in button/editor creation accepts an optional JSON `text` string, matching
the common input fixture. Quick supports separate native control owners in actual
additional windows; editor/WebView recreation and two editor-bearing rendered
surfaces pass in the VM on Wayland and XWayland. Widgets has a separate
`DOROTI_SAMPLE=qt-widgets` ClipRect-only scene; native parent identity, initial text,
synthetic key input and resize pass. The full Quick input scene is deliberately
rejected on Widgets because its foreground raster is not proven disjoint. None of
these runs qualifies physical IME, Tab traversal or Orca. See the
[Linux result](../../../works/results/2026-09-29-linux-qt.md).
