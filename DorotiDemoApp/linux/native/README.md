# Doroti Qt native shim

This directory is the app-owned CMake customization point. The managed runner owns process startup and calls the append-only `doroti.qt-host/v2` C ABI exported by `libdoroti_qt_host.so`.

The native host uses a Qt 6 `QOpenGLWindow` and wires the current GL surface, swap ACK, metrics/lifecycle, pointer/touch/tablet, key/focus, editing-state IME, clipboard, cursor, and resize contracts. Doroti/Skia rasterization stays managed and renders directly to the Qt-bound framebuffer.

Qt is a system dependency for this target. Build and runtime require Qt 6.5 or newer with Core, Gui, Widgets, OpenGL, and the active platform plugin (`wayland` or `xcb`). The shim has no embedded build-path RUNPATH; the system loader and Qt plugin search rules select those libraries. Accessibility, physical Linux IME, and X11 evidence remain separate acceptance gates.
