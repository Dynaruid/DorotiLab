# Qt host contracts

Run the managed checks from the repository root:

```sh
dotnet run --project Doroti/validation/linux-qt-contract/Doroti.Validation.LinuxQtContract.csproj
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
client input isolation, maximize/restore, fullscreen safe-area changes, accessible
caption buttons and closing. It accepts frames without drawing an application scene.
`native.cpp` is the separate Vulkan ABI/lifecycle probe; it also accepts frames without
GPU submission. Neither probe establishes Wayland compositor blur appearance or physical
move/resize behavior. These require visual testing on the target desktop.

The titlebar extension uses feature bit 14, 56-byte configuration and 144-byte surface
descriptors. Rebuild the native shim with the managed host. The app-owned native source
and template source must stay identical.
