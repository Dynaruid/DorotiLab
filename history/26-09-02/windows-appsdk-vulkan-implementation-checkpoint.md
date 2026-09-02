# Windows App SDK optional Vulkan implementation checkpoint

- Date: 2026-09-02
- Decision: `experimental optional`; default remains `AngleD3D11`
- Implementation: PASS
- Automated qualification: PASS-partial
- Physical acceptance: `notVerified`

## Result

Silk.NET Vulkan 2.23.0 is integrated into the opaque `HwndExactCpp` product path behind `DOROTI_WINDOWS_PRESENTER=Vulkan`. The implementation requires System32 Vulkan, Vulkan 1.1, hardware graphics+present, FIFO, and the core surface/Win32-surface/swapchain extensions. It never silently falls back to ANGLE and cannot share the Acrylic topology.

Stale work now terminates before acquire; successful acquire commits the image to unconditional copy/present. This removes acquired-image release, signal-drain, swapchain-maintenance, and present-fence requirements. Recreate retires the old swapchain with raster-thread `vkQueueWaitIdle`, and same-image reacquire proves previous present-wait semaphore consumption.

The tested NVIDIA GeForce RTX 4060 Laptop GPU passed the existing comprehensive qualification. The AMD Radeon 780M, which lacks `VK_KHR_swapchain_maintenance1`, passed the revised maintenance-free capability/WSI contracts, product path, DorotiDemoApp Release smoke run, and one automated live-resize probe.

## Evidence ledger

| Class | Status | Evidence / boundary |
|---|---|---|
| Comprehensive qualification | PASS | `.doroti/evidence/windows-vulkan-final-qualification7/manifest.json`; ANGLE baseline, capability, WSI, product, exact resize/reset/lifecycle/start-close, negative contracts all passed |
| AMD comprehensive qualification | PASS | `.doroti/evidence/windows-vulkan-amd-acquire-commit-final/manifest.json`; maintenance-free capability/WSI/product, result injection, exact resize/reset/lifecycle/start-close, ANGLE/package negative gates all passed |
| ANGLE default build/product | PASS | Before/after runs kept `AngleD3D11` as requested/effective default |
| Vulkan capability | PASS | System32 loader, validation warning/error 0, and hardware device selection passed on NVIDIA and AMD without a maintenance requirement |
| Short real WSI qualification | PASS | AMD: 3 normal present, 3 per stale stage, 3 recreate, 2 lifecycle; pre-acquire supersede 7, post-acquire committed present 9, acquired=presented, outstanding/unconsumed 0 |
| Vulkan product | PASS | Direct Skia/Vulkan copy/present, terminal ledger, one-time device/surface recovery, and clean shutdown passed |
| Automated input/IME/UIA | PASS-partial | Product contract passed; physical IME candidate/caret and Narrator remain `notVerified` |
| Self-contained publish | PASS | `.doroti/evidence/windows-vulkan-c9-publish-final.json`; Silk.NET assemblies present, app-local Vulkan loader/ICD absent, empty-PATH Vulkan launch exit 0 |
| WGC visible scene/pixels | PASS-automated | `.doroti/evidence/windows-vulkan-visible-v3/capture.json`; first frame captured, blank/error 0, opaque and alpha-over pixel samples matched |
| Error-result injection | PASS | `OUT_OF_DATE`, `SUBOPTIMAL`, `SURFACE_LOST`, and `DEVICE_LOST` product injections passed; loss cases recovered exactly once and ended with outstanding 0 |
| Exact resize/reset/lifecycle/start-close | PASS | Ten cycles each; exact resize kept terminal/resource accounting clean |
| Eight-edge live reverse resize | PASS-automated-partial | `.doroti/evidence/windows-vulkan-live-eight-edges-3x-fixed/manifest.json`; 24/24, 2,166 captured, 1,482 decoded, marker regression/device/surface loss 0 |
| Diagnostic snapshot probe | PASS-automated-partial | `.doroti/evidence/windows-vulkan-live-diagnostics-probe2/manifest.json`; recreate reason, retirement latency, first/target/present QPC, bounded 256-event ring populated |
| AMD product and Demo | PASS | `.doroti/evidence/windows-vulkan-amd-acquire-commit-postcleanup-product.json` and `windows-vulkan-amd-acquire-commit-demo-v2.json`; effective Vulkan, visible exact present, acquired=presented, device/surface loss and outstanding acquired 0 |
| AMD live resize probe | PASS-automated-partial | `.doroti/evidence/windows-vulkan-amd-acquire-commit-postcleanup-live-probe/manifest.json`; one left-edge 600 px/600 ms reverse case plus ANGLE before/after |
| Stress/soak | `notRun` | Deliberately opt-in after user feedback; `--wsi-stress` and `--wsi-soak` are not default gates |
| Physical resize/scan-out | `notVerified` | No human eight-edge drag, direction-reversal, or physical scan-out acceptance in this run |
| Full GPU/DPI/refresh/global matrix | `notRun` | AMD product plus one live case passed, but the full NVIDIA resize matrix has not been repeated on AMD |

## Remaining promotion gates

Error-result recovery, pixel capture, and the repeated 600 px/600 ms direction-reversal regression are now automated PASS. Vulkan remains experimental until the slow/medium/fast × motion Cartesian run, per-edge ten-second stress, physical scan-out/human drag, broader GPU/driver/DPI/refresh/window-management matrix, physical Korean IME, and accessibility acceptance pass. None of these remaining gates changes the default ANGLE result.
