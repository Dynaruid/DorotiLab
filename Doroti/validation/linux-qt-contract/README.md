# Qt host contracts

Run the managed checks from the repository root:

```sh
python3 Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/linux-qt-contract/Doroti.Validation.LinuxQtContract.csproj
```

They check ABI layout, keyboard/clipboard/environment contracts, common/macOS material
routing, legacy appearance compatibility and titlebar alpha/compositing at Retina scale.

On Linux with Qt 6, Wayland development files and Xvfb, build the native OpenGL shim
and run the caption event probe:

```sh
cmake -S DorotiTestbedApp/linux/native -B /tmp/doroti-qt-build -DDOROTI_QT_GRAPHITE=OFF
cmake --build /tmp/doroti-qt-build
g++ -std=c++20 Doroti/validation/linux-qt-contract/titlebar.cpp \
  -I DorotiTestbedApp/linux/native/include $(pkg-config --cflags --libs Qt6Core Qt6Gui) \
  -ldl -o /tmp/doroti-titlebar-probe
xvfb-run -a /tmp/doroti-titlebar-probe /tmp/doroti-qt-build/libdoroti_qt_host.so unified
xvfb-run -a /tmp/doroti-titlebar-probe /tmp/doroti-qt-build/libdoroti_qt_host.so solid
```

This probe runs the real native event loop and checks window flags, caption descriptors,
client input isolation, all eight resize cursor directions, queued client cursor
updates, cursor restoration on leave/re-entry, maximize/restore, fullscreen
safe-area changes, accessible caption buttons and closing. Resize hit testing and
cursor feedback share a five-logical-pixel border; maximized/fullscreen windows
do not expose a client resize cursor. It accepts frames without drawing an application scene.
`native.cpp` is the separate Vulkan ABI/lifecycle probe; it also accepts frames without
GPU submission. Neither probe establishes Wayland compositor blur appearance or physical
move/resize behavior. These require visual testing on the target desktop.

The titlebar extension uses feature bit 14, 56-byte configuration and 144-byte surface
descriptors. Rebuild the native shim with the managed host. The app-owned native source
and template source must stay identical.


## Linux PlatformView (PV-9)

The Qt product runner now supplies the optional `doroti/native-button` and
`doroti/native-editor` factories selected by the application manifest. The Testbed
Linux manifest selects both. Set `DOROTI_TESTBED_MODE=platform-views` and
`DOROTI_PLATFORM_VIEW_COMPOSITION=overlay` for the limited B fixture.

This path uses live, non-overlapping child Widgets above the existing Graphite
QWindow. Placement batches validate before GPU rendering and apply only after
queue-present acceptance; failed or contended admission leaves native placement
unchanged. This does not guarantee physical atomic presentation. Coordinates
are logical pixels: bounds round to Qt integers and clip edges round inward.
Foreground raster, native-native overlap, affine transforms and shields with
native content are explicitly rejected. The sample's interleaved page reports
unavailability. `CaptureIncludesNative`, accessibility qualification, gesture
mediation and synchronized placement remain false.

The `doroti.qt-host/v2` export now carries **ABI 4**: callbacks are **192 bytes**,
with pre-application preparation at offset 184 / feature bit 15. Feature bit 16
exposes the separate 64-byte attachment API / 80-byte placements. Existing
configuration, surface and host API table sizes remain 56/144/128 bytes. The
managed runner, app native source and template must be updated together.
The existing runner CMake target discovers the added native source and copies
the rebuilt library through its normal build/publish graph.

`DorotiQtRunner.Run(descriptor, prepareApplication)` invokes opt-in preparation
before QApplication creation, on the future GUI thread. It permits an external
shim to register URL schemes without linking WebEngine into the generic host.
Native providers can adopt a hidden, parentless QWidget through the separate
C ABI; ownership transfers on success. The built-in managed dispatcher only
uses synchronous GUI operations. A product WebEngine managed factory/controller
and plugin package are still work2 tasks, not supplied by this change.

Recorded gates (each subprocess has an external 1200-second timeout):

```sh
python3 Doroti/validation/linux-qt-contract/record-platform-views.py build
python3 Doroti/validation/linux-qt-contract/record-platform-views.py managed
python3 Doroti/validation/linux-qt-contract/record-platform-views.py owners-xcb
python3 Doroti/validation/linux-qt-contract/record-platform-views.py owners-wayland
python3 Doroti/validation/linux-qt-contract/record-platform-views.py attachment-xcb
python3 Doroti/validation/linux-qt-contract/record-platform-views.py attachment-wayland
python3 Doroti/validation/linux-qt-contract/record-platform-views.py abi-xcb
python3 Doroti/validation/linux-qt-contract/record-platform-views.py abi-wayland
python3 Doroti/validation/linux-qt-contract/record-platform-views.py product-build
python3 Doroti/validation/linux-qt-contract/record-platform-views.py product-xcb
python3 Doroti/validation/linux-qt-contract/record-platform-views.py product-wayland
python3 Doroti/validation/linux-qt-contract/record-platform-views.py opengl-titlebar
python3 Doroti/validation/linux-qt-contract/record-platform-views.py accessibility
python3 Doroti/validation/linux-qt-contract/record-platform-views.py dependencies
python3 Doroti/validation/platform-views/record.py common
```

Run .NET gates sequentially and run window capture gates one at a time when
collecting visual evidence. `attachment-*` requires the system Qt WebEngineWidgets
and Qt Test development packages; it uses the actual native host but does not draw
Graphite. `owners-*` uses the production attachment source in a two-owner harness.
Only `product-*` executes the managed Graphite product. `dependencies` checks that
the generic shared library does not link WebEngine/WebChannel/Quick and compares
every app-owned native source with the template. No sandbox-disabling flags are used.

2026-09-14 Korea / 2026-09-13 UTC: native and managed gates, the two-owner queue,
100 create/remove cycles and both QPA product resize/close gates passed on Ubuntu
26.04.1, Qt 6.10.2, a VMware VM with llvmpipe Vulkan, DPR 1. `xcb` was **XWayland**,
not native X11. XWayland captured live controls and the native WebEngine probe;
Wayland window capture was unavailable and the WebEngine probe emitted an EGL
surface warning. Its lifetime/JS success is not visual or GPU approval.
See the local evidence under `Doroti/artifacts/platform-views/2026-09-13/linux-qt/`
and the review at `Doroti/artifacts/platform-views/2026-09-14/linux-qt/implementation.md`.

PV-9 remains **PARTIAL**. C1-C6 interleaving, an actual Quick host prototype,
product WebEngine integration, complete pointer/Tab/IME/Orca behavior, two product
windows, fractional/cross-monitor DPR, device loss, performance budgets,
NativeAOT and clean package deployment are not qualified by these gates.
