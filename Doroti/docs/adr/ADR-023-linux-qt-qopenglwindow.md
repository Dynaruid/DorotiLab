# ADR-023: Use QOpenGLWindow for the Linux Qt GPU surface

- Status: accepted (2026-08-20)
- Scope: Linux Qt render surface and host ABI
- Supersedes: the paused decision in ADR-022; ADR-021 runtime ownership remains in force

## Decision

Use a Qt 6 `QOpenGLWindow` as the first Linux GPU surface. The managed process continues to own Doroti startup and lifetime. Qt owns its GUI loop, native window, OpenGL context, input method, clipboard, cursor, and compositor swap. Skia renders the shared Doroti scene directly into the framebuffer bound by `QOpenGLWindow::paintGL`; Qt performs the swap, and `frameSwapped` is the only presented boundary.

The earlier `QOpenGLWidget` branch rendered the fixed spike into its non-zero internal FBO but the complete Doroti scene crashed in Mesa Gallium's indexed draw path. On the same Kubuntu VMware Wayland system, with the same SkiaSharp build and shared renderer, `QOpenGLWindow` rendered the complete Material Gallery through framebuffer 0 and remained stable. A validation run performed 20 alternating resizes, rasterized 56 frames, received terminal swap acknowledgements, and exited normally with code 0.

`SVGA3D; build: RELEASE; LLVM;` is accepted as the VMware Mesa `svga` path when `vmwgfx`, DRI3, and direct rendering are also present. The word `LLVM` in that renderer string is not an LLVMpipe classification. Only explicit software renderer identities such as `llvmpipe`, `softpipe`, and `SwiftShader` are rejected.

The v2 C ABI remains append-only. Its original render/swap/context prefix is unchanged; negotiated feature bits add metrics/lifecycle, pointer, key/focus, editing-state IME, clipboard, cursor, and resize tables. The current managed host requires the complete feature set, so an older native library fails deterministically during ABI validation.

GL procedure resolution is QPA-specific with SkiaSharp 4.151.1. Wayland/EGL uses `QOpenGLContext::getProcAddress`. On xcb/GLX that resolver reached `GrGLExtensions::init` and crashed, while SkiaSharp's platform resolver (`GRGlInterface.Create()`) used the current GLX context successfully. The adapter therefore selects the platform resolver for `xcb` and retains the Qt resolver for Wayland; both branches render the same Qt-bound FBO and neither changes context or swap ownership.

## Consequences

- Qt Widgets embedding is no longer part of the first Linux surface. A future multi-widget shell would need a separate composition decision.
- Full-frame CPU copies and managed framebuffer allocations remain prohibited.
- Web and MAUI scene raster logic is now owned by `Doroti.Skia.Rendering`; Qt consumes the same renderer.
- Physical Linux, X11, assistive technology, and real IBus/Fcitx gates remain separately `notVerified`; success on VMware Wayland does not imply those results.

## Evidence

- Kubuntu 26.04, Wayland, Qt 6.10.2, Mesa 26.0.8, `vmwgfx` 2.21.0.0.
- OpenGL vendor `VMware, Inc.`, renderer `SVGA3D; build: RELEASE; LLVM;`, OpenGL 4.3 compatibility profile.
- `QOpenGLWindow` framebuffer 0, DPR 1.5, direct shared-renderer Material Gallery pixels.
- Debug managed/native builds: zero warnings and zero errors.
- Twenty-resize validation: 56 raster frames, normal exit 0, no terminal-coverage exception.
- Separate QPA runs for `wayland` and XWayland `xcb`: complete scene, resize, and normal exit 0 on both.

## Deferred gates

Window/context replacement, physical Linux Wayland, X11, Korean IME with an installed IBus/Fcitx engine, and AT-SPI inspection remain explicit follow-up evidence rather than inferred passes.
