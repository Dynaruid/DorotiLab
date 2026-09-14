# Graphite depth/stencil synchronization correction — 2026-09-14

The two reported `WRITE_AFTER_WRITE` synchronization errors are resolved on
the tested Windows AMD Radeon 780M and NVIDIA RTX 4060 Laptop paths. Release
build passed with zero warnings/errors. All build/test children used the
repository's 1,200-second timeout.

## Cause and correction

Installed SkiaSharp `4.154.0-preview.1.26454.9` identifies SkiaSharp commit
`143a933a753dbfeca1909524b2c06c546c5c3e20`. Its
[Skia submodule](https://github.com/mono/SkiaSharp/tree/143a933a753dbfeca1909524b2c06c546c5c3e20/externals/skia)
is `cc43af052d3d98e605bee4ddc98671dafded1c57`.

In that revision's
[VulkanCommandBuffer.cpp](https://github.com/mono/skia/blob/cc43af052d3d98e605bee4ddc98671dafded1c57/src/gpu/graphite/vk/VulkanCommandBuffer.cpp#L880),
`setup_texture_layouts` requests a writable depth/stencil attachment layout
with destination stage `EARLY_FRAGMENT_TESTS`. The validation layer reports
subsequent depth writes from `vkCmdDraw` and `vkCmdDrawIndexed` in
`LATE_FRAGMENT_TESTS`, outside that destination scope. Image layout transitions
are writes and must be synchronized against subsequent attachment writes;
see [Vulkan synchronization examples](https://docs.vulkan.org/guide/latest/synchronization_examples.html).

`VulkanObserver.Synchronization.cs` strengthens the stage mask immediately
before forwarding `vkCmdPipelineBarrier`. It adds only `LATE_FRAGMENT_TESTS`
when the existing destination includes early tests, lacks late/all-graphics/
all-commands coverage, and contains a writable depth/stencil image barrier
targeting `DEPTH_STENCIL_ATTACHMENT_OPTIMAL`. Source stages, access masks,
layouts, queue families, and borrowed barrier arrays remain unchanged.

This correction is part of the shared Graphite Vulkan dispatch, not a
validation-message filter. `generate-dispatch.py` and its generated callback
were both updated. A diagnostic counter proves the corrected path was exercised
and accumulates across device resets. No custom native Skia build or package
upgrade was introduced; both runs loaded the same original `libSkiaSharp.dll`
SHA-256 `07ce51fd59e099b9561b0327223c27b21aa5605b5b8f4484dd297fdb8c8725a1`.

## Evidence

The [before-correction report](../../artifacts/validation/windows-d3d12-output/20260914-173524-NoPreference/report.json)
contains the two synchronization errors. Run the same product gate with both
the Vulkan synchronization validator and D3D12 debug layer enabled:

```powershell
python -c "import subprocess; subprocess.run(['python','Doroti/validation/windows-d3d12-output/verify.py','--vulkan-validation'],timeout=1200,check=True)"
python -c "import subprocess; subprocess.run(['python','Doroti/validation/windows-d3d12-output/verify.py','--gpu','HighPerformancePreference','--vulkan-validation'],timeout=1200,check=True)"
```

| Device | Vulkan errors/warnings | D3D12 errors/warnings | Corrected barrier calls | Output copies completed | Result |
| --- | --- | --- | --- | --- | --- |
| AMD Radeon 780M | 0/0 | 0/0 | 2,307 | 135/135 | [PASS](../../artifacts/validation/windows-d3d12-output/20260914-174408-NoPreference/result.json) |
| NVIDIA RTX 4060 Laptop | 0/0 | 0/0 | 2,619 | 159/159 | [PASS](../../artifacts/validation/windows-d3d12-output/20260914-174417-HighPerformancePreference/result.json) |

Each run also covered visible rendered content, eight synthetic HWND resize
steps including three prepared moving-origin commits, one DXGI capacity growth,
minimize/restore, one device reset, two drained/destroyed owners, and exit 0.

This resolves the reported errors in those scenarios. Physical input/IME,
perceived smoothness, before/after performance, and runtime validation of the
shared correction on Android/Linux remain `notVerified`.
