# Windows Vulkan rendering with D3D12 output — 2026-09-14

The default Windows App SDK path now renders with Graphite/Vulkan and presents
through a same-adapter D3D12 DIRECT queue, DXGI flip-sequential swapchain, and
DirectComposition visual. Runtime identity is `Graphite/Vulkan/D3D12/DXGI`.
Leave `DOROTI_WINDOWS_PRESENTER` unset or set it to `Vulkan`: the setting selects
the renderer. The existing explicit `D3D12` selector still names the separately
deployed Ganesh diagnostic renderer; it is not needed for the new output.

## Ownership and synchronization

- D3D12 allocates three committed, shared BGRA8 source textures on the Vulkan
  adapter's exact LUID. Vulkan imports them as dedicated `D3D12_RESOURCE` memory.
- Vulkan copies the retained rendered image into a selected available source,
  releases queue ownership to `EXTERNAL` in `GENERAL`, and signals an imported
  D3D12 timeline fence. D3D12 queues a GPU wait for that value before reading it.
- D3D12 transitions the source `COMMON → COPY_SOURCE → COMMON` and the current
  DXGI back buffer `PRESENT → COPY_DEST → PRESENT`. After `Present`, the queue
  signals a completion fence. That fence authorizes source/allocator reuse;
  DXGI owns the distinct displayed back buffers and their presentation lifetime.
- Capacity grows monotonically. `ResizeBuffers` runs only after the output
  queue drain and release of old back-buffer references. There is no CPU pixel
  readback/upload in the main output path.
- Prepared moving-origin resize waits for the matching DXGI present count.
  Startup/fixed-origin admission retains its ordinary `DwmFlush` boundary.
  A DWM boundary is not labeled a matching DXGI frame receipt.
- Reset and shutdown drain Vulkan plus D3D12 before freeing imports/resources.
  A failed completion signal poisons the output owner, so it cannot be reused
  or destroyed as if idle. The existing managed quarantine path retains unsafe
  native owners for process reclamation.

The prior D3D11/Presentation-API main output implementation was removed.
Native PlatformView raster slices still use their existing D3D11
`BeginDraw`/`UpdateSubresource` API with a lazily created, shared device. Acrylic
keeps its desktop backdrop target beneath the premultiplied main output.

Vulkan frame completion still performs its CPU fence wait, and the new DXGI
path adds one output GPU copy and three back buffers. Lower CPU wait, lower
input latency, improved frame rate, and reduced memory use are **not established**.

The implementation follows Microsoft's
[D3D12 swapchain queue contract](https://learn.microsoft.com/en-us/windows/win32/direct3d12/swap-chains)
and [composition swapchain creation contract](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nf-dxgi1_2-idxgifactory2-createswapchainforcomposition).
Khronos recommends importing D3D12 fence payloads into timeline semaphores and
using timeline submit values; see
[D3D12 fence semaphore interop](https://docs.vulkan.org/refpages/latest/refpages/source/VkD3D12FenceSubmitInfoKHR.html).

## Scoped verification

Release Testbed and native host build: **PASS**, zero warnings/errors, with the
repository's 1,200-second build/test timeout. Tested native host SHA-256:
`ae65215315ce24ca1c444ca31eaaeb4f13dce4deda453464a5afd84ebdb6fbdc`.

| Gate | Observed result | Evidence |
| --- | --- | --- |
| AMD Radeon 780M, D3D12 debug enabled | PASS: 275/275 output copies complete, one DXGI resize, three prepared commits, two drained/destroyed owners, zero D3D12 errors/warnings, exit 0 | [result](../../artifacts/validation/windows-d3d12-output/20260914-173500-NoPreference/result.json) |
| NVIDIA RTX 4060 Laptop, D3D12 debug enabled | PASS: 399/399 copies complete, one DXGI resize, three prepared commits, two drained/destroyed owners, zero D3D12 errors/warnings, exit 0 | [result](../../artifacts/validation/windows-d3d12-output/20260914-173508-HighPerformancePreference/result.json) |
| Acrylic/PlatformView mounted regression | PASS: ten overlap/alpha states, native identity/text, Tab/Shift+Tab, modal input shield, three create/dispose cycles, GDI 12 → 12, exit 0 | [result](../../artifacts/validation/platform-views/windows-acrylic-composition/20260914-173419/result.json) |
| Khronos Vulkan synchronization validation, AMD, before stage correction | **FAIL (historical)**: two depth/stencil `WRITE_AFTER_WRITE` hazards reported for `vkCmdDraw`/`vkCmdDrawIndexed`; D3D12 errors/warnings remain zero, process exit 0 | [report](../../artifacts/validation/windows-d3d12-output/20260914-173524-NoPreference/report.json) |

The initial Vulkan failure is resolved by a narrowly scoped destination-stage
correction at the Graphite Vulkan dispatch boundary. The packaged Skia source
prepares writable depth/stencil attachments for `EARLY_FRAGMENT_TESTS` only;
the callback now also includes `LATE_FRAGMENT_TESTS` when that coverage is
missing. The native SkiaSharp DLL is unchanged and no validation messages are
suppressed. See the [synchronization fix and strict revalidation](vulkan-depth-stencil-sync-2026-09-14.md).

| Strict validation after correction | Result | Evidence |
| --- | --- | --- |
| AMD Radeon 780M | PASS: Vulkan errors/warnings 0/0, D3D12 errors/warnings 0/0, 2,307 corrected barrier calls | [result](../../artifacts/validation/windows-d3d12-output/20260914-174408-NoPreference/result.json) |
| NVIDIA RTX 4060 Laptop | PASS: Vulkan errors/warnings 0/0, D3D12 errors/warnings 0/0, 2,619 corrected barrier calls | [result](../../artifacts/validation/windows-d3d12-output/20260914-174417-HighPerformancePreference/result.json) |

Both strict runs also passed three prepared resize commits, one DXGI capacity
increase, one device reset, and two drained/destroyed output owners. These close
the two reported synchronization errors; broader graphics qualification remains
**PARTIAL** for the physical, platform, and performance limits below.

The passing output gates exercise synthetic HWND resize messages, visible
content, capacity growth, minimize/restore, and a device reset. The report is
captured before presenter disposal (one active DXGI swapchain); debug teardown
records plus successful process exit cover subsequent owner destruction.
Physical keyboard/mouse/IME, mixed DPI, perceived resize smoothness, atomic
scan-out, and before/after performance remain `notVerified`.

Reproduction: [D3D12 output gate](../../validation/windows-d3d12-output/README.md)
and [Acrylic/PlatformView gate](../../validation/windows-acrylic-composition/README.md).
