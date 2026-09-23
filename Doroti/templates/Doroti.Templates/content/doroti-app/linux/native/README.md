# Doroti Qt native shim

This directory is the app-owned CMake customization point. The managed runner owns process startup and calls the append-only `doroti.qt-host/v2` C ABI exported by `libdoroti_qt_host.so`.

The default native host uses a Qt 6 `QWindow` and `QVulkanInstance`. It retains metrics/lifecycle, pointer/touch/tablet, key/focus, editing-state IME, clipboard, cursor, accessibility and resize contracts. C ABI v2 (ABI version 4) feature bits 10/11 supply the Vulkan instance, surface, actual enabled instance extensions and API version in the surface descriptor (now 144 bytes). Feature bit 12 requires the appended GPU polling callback (192-byte callback table); rebuild the shim when updating the managed host. Managed Graphite/Vulkan owns the device and swapchain, and reports queue-present acceptance separately from physical scan-out.

Feature bit 13 requires `prepare_present` in the appended 128-byte host API table. Managed rendering calls it only immediately before queuing a Vulkan presentation, after rejecting empty or superseded scenes. Qt completes its presentation notification only on success. A skipped frame retries on an owner-thread timer because no compositor frame callback is promised. Rebuild the shim together with the managed host.

Graphite clears its persistent GPU backing to transparent before a complete frame; a GPU copy reaches the acquired swapchain image. Resizing passes the current swapchain as `oldSwapchain` when creating its replacement before destroying the old handle. Vulkan 1.2 and Vulkan development headers are required. Two bounded frame slots use nonblocking acquisition and fence polling. A Qt owner-thread timer polls pending GPU work every 8 ms, including after the last frame and while hidden, and stops once complete. Close blocks new rendering and hides the window before teardown. Native window teardown drains and destroys the managed swapchain before Qt releases its VkSurfaceKHR. For an explicit legacy comparison, configure CMake with `-DDOROTI_QT_GRAPHITE=OFF` and also set `DOROTI_LINUX_GRAPHITE=0` for the managed process. This builds the retained QOpenGLWindow/FBO implementation; there is no automatic fallback.

`WindowBackdropMode.acrylic` requests compositor blur for the complete client surface. Wayland selects `ext-background-effect-v1` first, falls back to the legacy KDE blur protocol, and finally applies the configured transparent or solid policy when neither protocol is advertised. The framework background colors remain responsible for the acrylic tint and alpha.

Qt is a system dependency for this target. Build and runtime require a Vulkan-enabled Qt 6.5 or newer with Core, Gui, Widgets, OpenGL (for the retained comparison build), the active platform plugin (`wayland` or `xcb`), Wayland client development files, `pkg-config`, and `wayland-scanner`. The shim has no embedded build-path RUNPATH; the system loader and Qt plugin search rules select those libraries. Accessibility, physical Linux IME, and X11 evidence remain separate acceptance gates.

The Quick build no longer requests the Qt OpenGL or OpenGLWidgets CMake components directly; `QOpenGLWindow` and its Qt OpenGL include/link are confined to the Widgets comparison build. Quick still needs Widgets for `QApplication`. Source include and `DT_NEEDED` inspection found no use of OpenGLWidgets. Both QPA targets currently build the Wayland backdrop protocol code, so an xcb-only development dependency profile would need a separate compile path and build matrix; it is not offered without a consumer need. Running with `QT_QPA_PLATFORM=xcb` still requires the published shim's Wayland client runtime dependency.

Official deployment uses the pinned `SkiaSharp.NativeAssets.Linux` package and checks both original managed and native hashes, plus the loaded native module path. The Linux asset needs glibc, libstdc++, fontconfig and their runtime dependencies. Framework-dependent and self-contained directory publishing are supported. Trimming, single-file and NativeAOT publishing are unsupported for this target; the runner rejects them before producing an unusable package. Hardware rendering, permanent GPU stalls and physical input require separate validation.

Hardware and software renderers are accepted by default; llvmpipe needs no environment override. The official Graphite Vulkan backend and Vulkan 1.2 requirements remain in use; the selected device type is logged. This is software validation, not hardware/performance qualification. Current Material scenes report an upstream depth-attachment synchronization issue under validation; preserve those failures.

Feature bit 14 negotiates window titlebars: the configuration is 56 bytes and the
surface descriptor is 144 bytes. Unified Acrylic uses client caption controls in the
same GPU/blur surface, with native move/resize and accessible min/max/close actions.
Solid uses native decorations. Rebuild the shim with the managed host and template.


PlatformView ABI 4 adds feature bit 15 (optional pre-QApplication preparation callback)
and bit 16 (separately versioned Widgets attachment table). `DorotiQtRunner.Run(descriptor,
prepareApplication)` runs the preparation callback on the future Qt GUI thread; a shim
can register schemes here without creating another application. A failing callback
aborts startup. Rebuild custom native hosts alongside the managed host.

`doroti_qt_platform_views.h` documents the optional owner-token API. The current product
path is limited B: non-overlapping child QPushButton/QLineEdit Widgets above the existing
Graphite QWindow, with translation and inward rect clip. Logical geometry is rounded to
Qt integer coordinates. Native-native overlap, foreground raster, input shields,
non-translation transforms and C interleaving are rejected. There is no CPU readback or
renderer switch. Native placement is applied after queue-present acceptance, so atomic
physical display is not advertised. Widgets `adopt_widget` is separate from the
optional product WebEngine Quick attachment below; Widgets gains no interleaving or effects.

The Qt GUI queue uses owner generations and exactly-once accepted task completion.
Owner close cancels queued work and deletes Widgets before the managed closed callback.
Qt-driven native window recreation rebinds each clip container to the same owner.
Callers must keep adopted-widget and callback modules loaded until owner teardown ends.
See `Doroti/validation/linux-qt-contract/README.md` for current contract checks.

## Optional Qt Quick GPU composition

`-DDOROTI_QT_QUICK=ON` builds the Quick/Qml/QuickControls2 backend (Qt 6.6+).
Its bundled Gaussian shader is embedded without ShaderTools or `qsb` at build
time. These tools are needed only to regenerate the shader after source edits;
see `shaders/README.md`. Debug and Release use the same hash-checked asset.
The Testbed selects it through `DorotiQtQuick=true`; the generic runner/template
keeps it optional. Runtime QML modules `QtQuick` and `QtQuick.Controls` are required.

Feature bit 17 negotiates the Quick path without changing callback ABI 4. Qt owns
Vulkan device/queue/WSI; the managed renderer lends completed GPU images through
the separate 48-byte GPU / 96-byte part API in `doroti_qt_quick.h`. Old managed
callbacks without bit 17 are rejected before startup. Quick controls are live QML
items, and QWidget adoption is explicitly unsupported by this backend.

The Vulkan instance must outlive QQuickWindow/QRhi destruction. The basic GUI/render
loop is required. Physical scanout atomicity and arbitrary native view types are
not implied by enabling this option. See the repository's `linux-qt-quick` product
validation for input, overlap, lifetime and Vulkan-layer checks.

`-DDOROTI_QT_WEBENGINE=ON` builds the sibling `libdoroti_webview_qt.so` against
system Qt 6.8+ WebEngineQuick/WebChannel and initializes schemes/WebEngine before
QApplication. The independent WebView command ABI is version 1 / 32 bytes;
host callbacks remain ABI 4 / 192 bytes. The generated runtime manifest records
the build versions and system helper/data/QML closure. No Qt engine is bundled. MSBuild exposes this as `DorotiQtWebEngine=true`; the
Testbed enables it with Quick, while templates keep it optional. PV feature bit
2 negotiates `create` kind 2 (HTML or versioned WebView options); bit 3 negotiates a
bounded live backdrop part in the existing 96-byte Quick packet. See the header
for the Gaussian sigma and sample-bound encoding. Quick owns a source group of
all earlier raster/native items, with the effect and sharp child outside it.
One isotropic effect (sigma <=32; <=4M physical sample pixels) is supported.
PV bit 4 adds color-backdrop kind 4, sigma zero and saturation 0–2, applied after
both blur passes. Tint remains independent foreground raster. Native/raster pixel
calibration is covered; full cross-platform color-space matching is not claimed.
The same native item supports public controller/navigation/JSON JS/profile and
bounded manifest app content. Full profile deletion and arbitrary remote messages
are unsupported; see `Doroti/docs/platform-views/linux-webview.md`.

Quick retains separate published/staging P banks and uses the actual frameSwapped
terminal for common session completion. A superseded render-only pass is closed
before admitting the next frame. The basic loop and queue drain remain; 1,024
pending GUI operations and 128 MiB of active/staging/retiring R/P images are
defensive bounds, not accepted frame-time budgets. No effect host/sample textures
remain at zero native effects. Rapid XWayland resize still has an observed Qt WSI
extent race (`VUID-VkSwapchainCreateInfoKHR-pNext-07781`); physical GPU, IME/Orca,
full device-loss, performance and deployment approvals remain separate.
