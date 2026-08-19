# Doroti Qt native shim

This directory is the app-owned CMake customization point. The managed runner owns process startup and calls the `doroti.qt-host/v1` C ABI exported by `libdoroti_qt_host.so`.

The current slice wires Qt window/surface generation, frame, scale/resize, pointer, key, text/IME and lifecycle callbacks. Rendering, clipboard, cursor and accessibility must be proven on Linux before their evidence can move from `notVerified`.
