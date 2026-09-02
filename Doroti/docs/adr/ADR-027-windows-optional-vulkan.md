# ADR-027: Experimental optional Silk.NET Vulkan presenter

- Status: Experimental
- Date: 2026-09-02

## Decision

The Windows App SDK `HwndExactCpp` target keeps managed hardware-D3D11 ANGLE/EGL as its default presenter. `DOROTI_WINDOWS_PRESENTER=Vulkan` explicitly selects a separate direct Vulkan/Skia presenter; there is no automatic Vulkan-to-ANGLE fallback. Vulkan and `WindowBackdropMode.experimentalAcrylic` are conflicting selections and fail before the window is created.

The product resolves only the System32 `vulkan-1.dll`, rejects software devices, and requires Vulkan 1.1, one graphics+present queue, FIFO presentation, `VK_KHR_surface`, `VK_KHR_win32_surface`, and `VK_KHR_swapchain`. Swapchain-maintenance extensions are not selection requirements. If exactly one device satisfies the complete contract it is selected; multiple capable devices require `DOROTI_WINDOWS_VULKAN_DEVICE` to be an exact or unique device-name fragment.

## Ownership and synchronization

Skia paints an exact-size device-local Vulkan image. The presenter rejects stale work before acquire; a successful acquire is the presentation commit. From that point it unconditionally submits the copy while waiting on the acquire semaphore, signals a per-image render-finished semaphore, and presents while waiting on that semaphore. Therefore there is no post-acquire stale-return path and no need for `vkReleaseSwapchainImagesKHR` or present fences. Reacquiring the same image proves that the previous present consumed its per-image semaphore. Swapchain recreation runs outside an acquired transaction and uses `vkQueueWaitIdle` on the raster worker before releasing the old swapchain and synchronization slots. Device reset and final disposal may use device idle after Skia context resources have been invalidated.

The native/managed ABI remains GPU-pointer-free. Acrylic composition resources are not shared with the Vulkan presenter.

## Evidence and promotion boundary

The 2026-09-02 run passed package/loader capability checks, a short real-WSI qualification, actual `OUT_OF_DATE`/`SUBOPTIMAL`/`SURFACE_LOST`/`DEVICE_LOST` branch injection and recovery, exact resize/device-reset/minimize-restore/start-close ten-cycle gates, automated input/IME/UIA transport, default ANGLE regression, and self-contained empty-PATH launch on an NVIDIA GeForce RTX 4060 Laptop GPU. An `ANGLE → Vulkan → ANGLE` WGC sequence and all eight borders/corners at 600 px/600 ms reverse motion passed three consecutive runs: 2,166 captured frames, 1,482 decoded generation markers, no marker regressions, no device/surface loss, and no outstanding acquired image.

After adopting acquire-as-presentation-commit, the AMD Radeon 780M on the same machine passed the maintenance-free capability and real-WSI contracts, the product validation, a Release DorotiDemoApp smoke run, and one 600 px/600 ms left-edge reverse WGC probe. That AMD probe is automated partial evidence, not the full NVIDIA resize matrix.

Slow/medium/fast × expand/shrink/reverse full Cartesian resize, per-edge ten-second stress, long WSI soak, human border resize, physical scan-out, the full GPU/driver/DPI/refresh matrix, physical Korean IME, and accessibility acceptance remain `notVerified` or `notRun`. Until those gates pass, Vulkan is experimental optional and must not become the default.
