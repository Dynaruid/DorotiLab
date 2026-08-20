# Doroti Qt native shim

This directory is the app-owned CMake customization point. The managed runner owns process startup and calls the append-only `doroti.qt-host/v2` C ABI exported by `libdoroti_qt_host.so`.

The native host uses a Qt 6 `QOpenGLWindow` and wires the current GL surface, swap ACK, metrics/lifecycle, pointer/touch/tablet, key/focus, editing-state IME, clipboard, cursor, and resize contracts. Doroti/Skia rasterization stays managed and renders directly to the Qt-bound framebuffer.

The window requests an 8-bit alpha buffer and uses full-frame repaint semantics. Every acquired swapchain framebuffer is cleared to transparent by the native host before Skia renders it; this prevents stale Wayland buffer contents from accumulating while keeping intentional transparent shell pixels available for compositor integration.

`WindowBackdropMode.acrylic` requests compositor blur for the complete client surface. Wayland selects `ext-background-effect-v1` first, falls back to the legacy KDE blur protocol, and finally applies the configured transparent or solid policy when neither protocol is advertised. The framework background colors remain responsible for the acrylic tint and alpha.

Qt is a system dependency for this target. Build and runtime require Qt 6.5 or newer with Core, Gui, Widgets, OpenGL, the active platform plugin (`wayland` or `xcb`), Wayland client development files, `pkg-config`, and `wayland-scanner`. The shim has no embedded build-path RUNPATH; the system loader and Qt plugin search rules select those libraries. Accessibility, physical Linux IME, and X11 evidence remain separate acceptance gates.
