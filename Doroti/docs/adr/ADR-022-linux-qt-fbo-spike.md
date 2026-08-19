# ADR-022: Pause the Linux Qt backend after the direct-FBO spike

- Status: accepted (2026-08-20)
- Scope: LNX-QT-0 only
- Supersedes: none; ADR-021 managed runtime ownership remains in force

## Decision

Keep the managed-owned process, Qt Widgets, and the new `doroti.qt-host/v2` ABI, but do not start LNX-QT-1 or later milestones until the direct-FBO spike passes on a hardware OpenGL renderer.

The current WSLg environment exposes a Qt `QOpenGLWidget` context and non-zero Qt-owned FBO, but identifies the renderer as `llvmpipe`. The Linux backend rejects that software renderer instead of treating it as GPU evidence. Before the guard was added, SkiaSharp `4.151.1` entered `GrGLExtensions::init` while assembling the supplied Qt GL procedure table and terminated with `SIGSEGV`. Direct managed calls through the same `glGetString`, `glGetIntegerv`, and `glGetStringi` addresses succeeded, so the failure is before `GRContext`, `GRBackendRenderTarget`, or `SKSurface` creation and does not establish that Qt's FBO is invalid.

This is a measured LNX-QT-0 failure, not `notVerified`. The fixed-frame pixel, resize/recreate, and swap-ACK acceptance gates remain unpassed. Native code now reports an actionable unsupported-feature error for `llvmpipe`, `softpipe`, and `SwiftShader` before entering Skia.

## Evidence

- Managed ABI layout validation and native `static_assert` compilation pass for v2.
- Windows `Doroti.Host.Qt` Debug build: 0 warnings, 0 errors.
- WSL Ubuntu native CMake build: pass with Qt `6.10.2`.
- WSL runner Debug build: 0 warnings, 0 errors.
- WSLg/xcb diagnostics: QPA `xcb`, GL vendor `Mesa`, renderer `llvmpipe (LLVM 21.1.8, 256 bits)`, OpenGL `4.5 Compatibility Profile`.
- gdb top frames: `GrGLExtensions::init` -> `GrGLMakeAssembledGLInterface` -> `gr_glinterface_assemble_gl_interface` in app-local `libSkiaSharp.so` `4.151.1`.
- No visible Doroti fixed-frame pixels were accepted as evidence.

## Alternatives considered

1. `QOpenGLWindow`: first fallback after the environment is hardware-backed. It changes widget composition but still uses the same Skia GL interface, so it does not address the observed pre-FBO crash by itself.
2. `QSGRenderNode`/QRhi: deferred. It adds Qt Quick and render-thread ownership before the current GL procedure/context question is isolated.
3. Native-owned executable plus `hostfxr`: deferred. Reversing process ownership does not change the app-local SkiaSharp GL binary or the current software renderer.
4. CPU raster fallback: rejected by the roadmap because it would hide the required GPU path and full-frame copy budget.
5. SkiaSharp upgrade/downgrade: excluded from this work; `4.151.1` remains pinned.

## Resume criteria

Resume LNX-QT-0 on WSLg or physical Linux only after diagnostics show a non-software renderer. Re-run the v2 fixed-frame path under gdb, then require visible pixels, 20 resizes, one window/context recreation, and exactly one swap-based terminal ACK for every frame request. If the same `GrGLExtensions::init` crash occurs with hardware GL, compare `QOpenGLWindow` first and record the result in a new ADR before changing process ownership or moving to Qt Quick.
