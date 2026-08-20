# ADR-022: Pause the Linux Qt backend after the direct-FBO spike

- Status: superseded by ADR-023 (2026-08-20)
- Scope: LNX-QT-0 only
- Supersedes: none; ADR-021 managed runtime ownership remains in force

## Decision

This ADR records the failed `QOpenGLWidget` branch. ADR-023 resumes the backend with the successful `QOpenGLWindow` branch while retaining managed process ownership and `doroti.qt-host/v2`.

The current WSLg environment exposes a Qt `QOpenGLWidget` context and non-zero Qt-owned FBO, but identifies the renderer as `llvmpipe`. The Linux backend rejects that software renderer instead of treating it as GPU evidence. Before the guard was added, SkiaSharp `4.151.1` entered `GrGLExtensions::init` while assembling the supplied Qt GL procedure table and terminated with `SIGSEGV`. Direct managed calls through the same `glGetString`, `glGetIntegerv`, and `glGetStringi` addresses succeeded, so the failure is before `GRContext`, `GRBackendRenderTarget`, or `SKSurface` creation and does not establish that Qt's FBO is invalid.

A 2026-08-20 Kubuntu 26.04 VMware retest uses the intended Mesa `svga`/kernel `vmwgfx` direct-rendering path. Qt Wayland exposed `SVGA3D; build: RELEASE; LLVM;`, `LIBGL_DEBUG=verbose` reported `using driver vmwgfx`, `Using DRI3`, and `direct rendering: Yes`, and `vmwgfx` reports version `2.21.0.0`. Mesa documents this renderer as the VMware guest driver that provides access to the host GPU; the `LLVM` suffix is not LLVMpipe. The `GLX_MESA_query_renderer` field `Accelerated: no` is therefore recorded but is not used alone to classify this renderer as a software fallback. The fixed spike frame rendered visibly to Qt FBO 1 at DPR 1.5. A subsequent experimental full Doroti scene crashed inside Mesa Gallium's indexed draw path during `GrGLOpsRenderPass::onDrawIndexed`, so the next action is the documented `QOpenGLWindow` comparison rather than a software-renderer rejection.

This was a measured `QOpenGLWidget` failure, not `notVerified`. It does not classify the later `QOpenGLWindow` result. Native code reports an actionable unsupported-feature error for `llvmpipe`, `softpipe`, and `SwiftShader` before entering Skia.

## Evidence

- Managed ABI layout validation and native `static_assert` compilation pass for v2.
- Windows `Doroti.Host.Qt` Debug build: 0 warnings, 0 errors.
- WSL Ubuntu native CMake build: pass with Qt `6.10.2`.
- WSL runner Debug build: 0 warnings, 0 errors.
- WSLg/xcb diagnostics: QPA `xcb`, GL vendor `Mesa`, renderer `llvmpipe (LLVM 21.1.8, 256 bits)`, OpenGL `4.5 Compatibility Profile`.
- gdb top frames: `GrGLExtensions::init` -> `GrGLMakeAssembledGLInterface` -> `gr_glinterface_assemble_gl_interface` in app-local `libSkiaSharp.so` `4.151.1`.
- No visible Doroti fixed-frame pixels were accepted as evidence.
- Kubuntu 26.04 VMware/Wayland native and managed Debug build: pass with .NET SDK `10.0.400`, Qt `6.10.2`, CMake `4.2.3`, and zero warnings/errors.
- Kubuntu fixed-frame screenshot: visible direct-FBO Skia background, text, circle, and rectangle; Qt FBO `1`, DPR `1.500`.
- Kubuntu renderer qualification: Mesa `svga`/`vmwgfx` direct-rendering path confirmed; `Accelerated: no` retained as a diagnostic field, not a fallback verdict.
- Kubuntu full-scene gdb frames: Mesa `libgallium` -> `GrGLOpsRenderPass::onDrawIndexed` -> `GrDrawingManager::flush` -> `gr_direct_context_flush_and_submit`.
- The later `QOpenGLWindow` comparison and resumed milestones are recorded in ADR-023 and the current Linux Qt evidence.

## Alternatives considered

1. `QOpenGLWindow`: first fallback after the environment is hardware-backed. It changes widget composition but still uses the same Skia GL interface, so it does not address the observed pre-FBO crash by itself.
2. `QSGRenderNode`/QRhi: deferred. It adds Qt Quick and render-thread ownership before the current GL procedure/context question is isolated.
3. Native-owned executable plus `hostfxr`: deferred. Reversing process ownership does not change the app-local SkiaSharp GL binary or the current software renderer.
4. CPU raster fallback: rejected by the roadmap because it would hide the required GPU path and full-frame copy budget.
5. SkiaSharp upgrade/downgrade: excluded from this work; `4.151.1` remains pinned.

## Resume criteria

Resume LNX-QT-0 when diagnostics show a real GPU driver path rather than `llvmpipe`, `softpipe`, or SwiftShader. For VMware, the documented `SVGA3D` renderer with `vmwgfx`, DRI3, and direct rendering qualifies even if `GLX_MESA_query_renderer` reports `Accelerated: no`. Compare `QOpenGLWindow` against `QOpenGLWidget`, then require visible pixels, 20 resizes, one window/context recreation, and exactly one swap-based terminal ACK for every frame request. If the Gallium indexed-draw crash occurs on both Qt surface types, record the result in a new ADR before changing process ownership or moving to Qt Quick.
