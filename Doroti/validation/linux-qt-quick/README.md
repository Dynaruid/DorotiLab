# Linux Qt Quick validation

The Testbed uses `DorotiQtQuick=true`. Its Linux runner also enables
`DorotiQtWebEngine=true`; other applications/templates keep both options opt-in.
Build with Qt 6.6+ Core, Gui, Widgets, Quick, Qml, QuickControls2,
Vulkan headers, Wayland client development files,
`wayland-scanner`, and the active `xcb` or `wayland` plugin. Qt 6.8+ WebEngineQuick/WebChannel and
the `QtWebEngine`/`QtWebChannel` runtime QML modules are needed when WebEngine is enabled.
Quick Controls require the `QtQuick` and `QtQuick.Controls` runtime modules.
The validation driver additionally uses Qt Test and Python Pillow.
The non-Quick comparison backend additionally requires the Qt OpenGL development
component. Quick does not directly request OpenGL or OpenGLWidgets in CMake;
this does not exclude transitive runtime dependencies of Qt or WebEngine.

Run from the repository root. Every command below has a 1,200-second outer
process-tree timeout; the product script also limits its application to 180 seconds.
Run .NET build commands sequentially to avoid shared obj races.

For a bounded indexed run, use `run-suite.py --output <new-directory> --shim
<matching-libdoroti_qt_host.so> --app <matching-app.dll> --qpa wayland --product`.
The suite saves each step's log and `index.json`; omit `--product` for displayless
contracts. `check-native-sync.py` hashes every app/template native file, including
ABI headers, QML, CMake, shaders and metadata. The C++ format policy is the
matching `.clang-format` in both native trees. Package consumers can be built
outside the repository with `verify-package-consumer.py --output <new-directory>
--quick --webengine --qpa wayland`; it packs the Linux dependency closure into a
local feed, installs and removes the template, checks for source ProjectReferences,
and records publish and mapped runtime binaries. Use a new output path for each
attempt. These local runs do not substitute for a clean VM installation.
Add `--desktop` with `--quick` to exercise the Linux Desktop companion and native
window probe; see [Desktop validation](../desktop-window/README.md#linux-qt-quick--2026-09-26).
The package runner now calls `check-runtime.py` before launch. It checks the
published shims and linker dependencies, the selected QPA plugin, required QML
module descriptors and mandatory plugins, and the WebEngine helper, resources
and locale separately. It uses `qtpaths6` when available. On a runtime-only
machine without that developer tool it infers standard Qt directories from the
linked Qt6Core location; custom Qt layouts should pass a matching `--qtpaths`.
For an existing publish directory, run:

```sh
python3 Doroti/validation/linux-qt-quick/check-runtime.py \
  --publish <publish-directory> --qpa wayland --quick --webengine
```

The JSON result can be saved with `--output <file>`. This preflight checks file
presence and direct linker resolution. It cannot prove that a platform plugin
loads or that a compositor, graphics driver, sandbox and QML runtime behave
correctly; the product run remains required.

The Quick summary records sample count and p50/p95/p99 milliseconds for GUI
queue idle, CPU recording, copy submit, copy fence wait and native commit. An
empty sample reports null percentiles. `measure-qt-workloads.py` additionally
records process-tree PSS, WebEngine process count and idle CPU. Qt `frameSwapped`
intervals are not physical display latency. `DOROTI_QT_VALIDATION_KEYCODES=1`
records QPA, native scan code, Qt key, modifiers and repeat without key text for
physical-key mapping evidence; `DOROTI_QT_VALIDATION_ACCESSIBILITY_DUMP=1`
records exposed text/editable interface counts. Orca and physical input still
require separately recorded manual results.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/linux/DorotiTestbedApp.Linux.csproj
DOROTI_TESTBED_MODE=platform-effects \
  python3 Doroti/validation/run-with-timeout.py dotnet DorotiTestbedApp/linux/bin/linux-x64/Debug/net10.0/linux-x64/DorotiTestbedApp.Linux.dll

qt_evidence="$PWD/Doroti/artifacts/platform-views/2026-09-14/linux-qt/rearchitecture"
qt_app="$PWD/DorotiTestbedApp/linux/bin/linux-x64/Debug/net10.0/linux-x64"
python3 Doroti/validation/run-with-timeout.py cmake -S Doroti/validation/linux-qt-quick \
  -B "$qt_evidence/native-validation" -DDOROTI_SHIM="$qt_app/libdoroti_qt_host.so"
python3 Doroti/validation/run-with-timeout.py cmake --build "$qt_evidence/native-validation" -j2
python3 Doroti/validation/run-with-timeout.py "$qt_evidence/native-validation/native-contract"
python3 Doroti/validation/run-with-timeout.py dotnet run \
  --project Doroti/validation/linux-qt-contract/Contract.csproj \
  --artifacts-path "$qt_evidence/contract-build" -- "$qt_evidence/native-validation/libproduct-driver.so"
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/linux-qt-quick/verify-product.py \
  --qpa wayland --app "$qt_app/DorotiTestbedApp.Linux.dll" \
  --driver "$qt_evidence/native-validation/libproduct-driver.so" --output "$qt_evidence/product-wayland"
```

Repeat the product command with `--qpa xcb` for XWayland/X11. Set
`VK_INSTANCE_LAYERS=VK_LAYER_KHRONOS_validation` and, if needed, `VK_LAYER_PATH`
and `LD_LIBRARY_PATH` to the locally extracted validation-layer package. The
driver records whether the validation layer was actually mapped into the process.
An absence of messages without a loaded layer is not Vulkan validation approval.

For rapid resize use `DOROTI_QT_VALIDATION_RESIZE_CYCLES=10` and
`DOROTI_QT_VALIDATION_PLATFORM_VIEWS=<absolute capture prefix>` on the real app.
This hook exits after the requested cycles. Keep its log even on exit 0:
Qt can report Vulkan validation failures without failing the process.

Normal Debug/Release builds use a bundled, hash-checked Vulkan shader and do not
require ShaderTools or `qsb`. Only shader regeneration needs those developer tools;
see the native host's `shaders/README.md`.

In the initial execution on this Ubuntu 26.04 / Qt 6.10.2 VM, ShaderTools development files, `qsb` and
Khronos validation layers were missing. Their matching Ubuntu packages were
downloaded and extracted under `tooling/root`, without changing the system.
CMake was initially configured with `Qt6ShaderTools_DIR` and `Qt6ShaderToolsTools_DIR`
pointing at those extracted package configs. Installed development packages
need no such overrides. The subsequent Release fix removed the build-time shader
compilation dependency entirely; no ShaderTools path override is now required.

`native-contract` checks real owner tokens, fractional placement, prepare-only
immutability, stale/cross-owner/thread rejection, paint-ordered shields,
pass-through effects, invalid sample/multiple-effect rejection, empty frames,
10 native create/dispose cycles, and a 1,024-entry queued-work bound with
exactly-once close cancellation. This fixture is separate from product input.
It also checks multiple mouse buttons, native touch/tablet retention through
move/release, shield rejection, and capture reset on hide/deactivation/lost mouse grab using
synthetic Qt events. Device hardware and mixed-target touch gestures remain
outside this contract.

The managed contract checks all ABI layouts and Quick versus Widgets geometry.
Its optional GPU driver uses a real Qt-owned Vulkan queue and the production
`GraphiteVulkanQuick`: 10 cycles of successful submission followed by rejected
publication, recording cancellation during resize, accepted replacement, and
allocation-budget failure. It checks that published images are never staging
targets and that retained allocations stay bounded. It does not represent
physical presentation or a full device-loss test.

`verify-product.py` runs the actual Material WebView/effect page. Its preload
driver lives only here and sends Qt mouse/key/wheel events through DorotiSurface.
It checks native click versus committed shield, editing and native identity,
animation, scroll, sharp foreground button semantics, two WebViews, effect
movement/removal, and disposal/recreation. The preload JavaScript probes remain validation-only; the product now also has
a separate public controller API, tested by `webview/verify-qt.py`. QQuickWindow captures include native
pixels on both QPA backends. Capture readback is separate from product GPU transport.

See [the current Qt contract and remaining gates](../../docs/platform-views/linux-qt.md).
The missing older Quick result.json files were not recovered or relabeled PASS.
The [2026-09-23 work3 result](work3-results-2026-09-23.md) records the current
partial outcome, including a mapped-layer XWayland resize VUID and a Qt-only
slow-sync reproducer. Use `verify-resize.py` for ten product resize cycles with
actual layer-path evidence, and `verify-lifecycle.py` for window-state evidence.
The independent `wsi/` CMake project builds `qt-wsi-resize` with only Qt Core,
Gui, Quick and Vulkan headers. Set `QT_QPA_PLATFORM=xcb`,
`VK_INSTANCE_LAYERS=VK_LAYER_KHRONOS_validation`, the corresponding layer path,
and `DOROTI_QT_WSI_SYNC_DELAY_MS=50` to reproduce the 10-cycle timing fixture.
It prints the actual swap count and mapped-layer state. Use `verify-wsi.py`
to require a mapped layer and fail on `VUID-` even if the fixture exits 0:

```sh
VK_INSTANCE_LAYERS=VK_LAYER_KHRONOS_validation \
  python3 Doroti/validation/linux-qt-quick/verify-wsi.py \
  --executable <wsi-build>/qt-wsi-resize --qpa xcb \
  --sync-delay-ms 100 --output <new-artifact-directory>
```

Set `VK_LAYER_PATH` and the layer library search path if the validation layer
is installed outside the system Vulkan paths. The fixture now requests Qt
Quick's preferred Vulkan instance extensions, as the product does. The
[2026-09-24 investigation](wsi-investigation-2026-09-24.md) records an
intermittent Qt-only xcb/XWayland extent VUID. Native Wayland is the tested
backend workaround on a Wayland session; it does not qualify xcb.

`verify-controls.py` accepts the same `--app`, `--driver`, `--output`, and `--qpa`
arguments. It records all ten Material composition cases and asserts 10 mounted
dispose/recreate cycles with new identities. The driver waits up to five seconds
for native retirement instead of treating a fixed screenshot delay as disposal
completion. The first fixed-delay attempt captured a hidden retiring pair after
cycle 0; that failed attempt remains in the artifacts.

Per the user's updated instruction, repeat counts are now limited to 10;
`verify-controls.py --cycles` accepts 1–10 and defaults to 10. Earlier completed
100-cycle reports remain historical evidence and are not rerun.

## Linux WebView controller and color effect gates (2026-09-20)

`webview-contract` is built alongside the driver when the optional sibling shim is
present. It creates two actual native Quick owners, negotiates WebView ABI 1,
rejects cross-owner/stale/thread calls, checks JS generations/unbind, runs ten
lifecycles, kills its own renderer, and verifies terminal failure plus explicit
fresh recreation. These are native owner checks, not two full Doroti product owners.

```sh
qt_current="$PWD/Doroti/artifacts/webview/2026-09-20/linux-qt"
qt_app="$PWD/DorotiTestbedApp/linux/bin/linux-x64/Debug/net10.0/linux-x64"
# Build/configure the driver as above, with DOROTI_SHIM from this exact app directory.
python3 Doroti/validation/run-with-timeout.py "$qt_current/driver/webview-contract"
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/webview/verify-qt.py \
  --app "$qt_app/DorotiTestbedApp.Linux.dll" --driver "$qt_current/driver/libproduct-driver.so" \
  --output "$qt_current/api-new" --qpa wayland
# Use a NEW output directory for each attempt. Repeat for --qpa xcb.
# Add --mode calibration for native/raster sigma, zero/reset, saturation and tint.
python3 Doroti/validation/run-with-timeout.py python3 Doroti/validation/webview/measure-qt-workloads.py \
  --app "$qt_app/DorotiTestbedApp.Linux.dll" --driver "$qt_current/driver/libproduct-driver.so" \
  --output "$qt_current/workloads-new" --qpa wayland
```

The API/calibration runner verifies the actual mapped host and WebView shim paths.
A driver linked to the Debug native host must not be used to approve Release
native code: rebuild the driver with `DOROTI_SHIM` pointing at the published app.
API fixtures exercise history, typed JS/errors/cancellation, app resources,
Range rejection, trusted app messages, profile isolation and create/dispose races.
The shared persistent option is tested for creation; arbitrary remote messaging,
full profile deletion and physical IME/accessibility remain unsupported/unqualified.

Workloads use 0/1/4 visible WebViews × idle/animation/scroll/modal, a ten-second
sample, Qt `frameSwapped` intervals and process-tree PSS. These intervals include
native Chromium updates and are not physical scanout, input latency or first
content display measurements. No before-change baseline exists for this execution.

Results and limits: [Linux execution report](../webview/linux-results-2026-09-20.md).

`webview/verify-qt-input.py` takes the same app/driver/output/QPA arguments and
checks synthetic Korean composition events, native Tab/Shift+Tab, and native focus
release to the Doroti text field. This caught the outer WebEngine focus-scope
handoff bug. It does not replace physical IBus/keyboard or full traversal/Orca.
The native WebView contract's ten lifecycles also replace initial HTML immediately,
so terminal callbacks must release the loading latch even without another start.

The Material WebView tab's floating panel can be checked with
`webview/verify-qt-floating-panel.py` using the same `--app`, `--driver`,
`--output`, and `--qpa` arguments under `run-with-timeout.py`. Rebuild the driver
first. It opens the real YouTube-configured tab, then selects the local sample
for deterministic drag/shield/toggle/resize checks. Captures record the live
native effect rectangle, and the gate verifies that browser identity and form
state survive panel movement and visibility changes. Run with default scale and
window size; the icon-only local-sample toolbar action uses its desktop position.
