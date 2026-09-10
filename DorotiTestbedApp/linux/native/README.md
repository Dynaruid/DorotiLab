# Doroti Qt native shim

This directory is the app-owned CMake customization point. The managed runner owns process startup and calls the append-only `doroti.qt-host/v2` C ABI exported by `libdoroti_qt_host.so`.

The default native host uses a Qt 6 `QWindow` and `QVulkanInstance`. It retains metrics/lifecycle, pointer/touch/tablet, key/focus, editing-state IME, clipboard, cursor, accessibility and resize contracts. C ABI v2 feature bit 10 supplies the Vulkan instance, surface and actual enabled instance extensions in a 120-byte surface descriptor. Managed Graphite/Vulkan owns the device and swapchain, and reports queue-present acceptance separately from physical scan-out.

Graphite clears its persistent GPU backing to transparent before a complete frame; a GPU copy reaches the acquired swapchain image. Hardware Vulkan 1.1 and Vulkan development headers are required. Native window teardown drains and destroys the managed swapchain before Qt releases its VkSurfaceKHR. For an explicit legacy comparison, configure CMake with `-DDOROTI_QT_GRAPHITE=OFF` and also set `DOROTI_LINUX_GRAPHITE=0` for the managed process. This builds the retained QOpenGLWindow/FBO implementation; there is no automatic fallback.

`WindowBackdropMode.acrylic` requests compositor blur for the complete client surface. Wayland selects `ext-background-effect-v1` first, falls back to the legacy KDE blur protocol, and finally applies the configured transparent or solid policy when neither protocol is advertised. The framework background colors remain responsible for the acrylic tint and alpha.

Qt is a system dependency for this target. Build and runtime require a Vulkan-enabled Qt 6.5 or newer with Core, Gui, Widgets, OpenGL (for the retained comparison build), the active platform plugin (`wayland` or `xcb`), Wayland client development files, `pkg-config`, and `wayland-scanner`. The shim has no embedded build-path RUNPATH; the system loader and Qt plugin search rules select those libraries. Accessibility, physical Linux IME, and X11 evidence remain separate acceptance gates.
