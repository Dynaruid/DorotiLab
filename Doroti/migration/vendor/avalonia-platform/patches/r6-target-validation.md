# R6 target-machine validation additions

- Added top-down premultiplied BGRA8888 readback to the selected Skia framebuffer and WGL/OpenGL frame adapters.
- Added the minimal Win32 monitor enumeration and window-placement interop needed to move a Doroti window across connected displays without exposing HWND or HMONITOR through public contracts.
- Kept GPU context ownership and pixel copying on the raster thread; the public surface exposes only Doroti-owned pixel and display value contracts.
