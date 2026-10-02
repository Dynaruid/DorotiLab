# Dynamic texture allowance

[DynamicTextureBudget](../src/Doroti.Skia.Rendering/DynamicTextureBudget.cs)
is a shared demand/pressure policy. The Qt Quick Vulkan R/P allocator uses it
in place of the fixed 128 MiB image guard. Other graphics backends and Skia's
internal cache limits retain their own contracts.

Each new image uses its actual Vulkan memory requirement and compatible memory
type. Pixel extent, DPR, raster segments, published images, two frame banks and
retiring images therefore contribute their real allocation sizes. The allowance
grows with demand, adds 25% slack rounded to 64 KiB, and shrinks only after the
owner has actually freed allocations. Live GPU-owned bytes remain a floor.

The allocator queries the advertised `VK_EXT_memory_budget` physical-device
properties when available. It uses per-heap process budget/usage, counts known
owned bytes even if the usage estimate lags, and preserves 20% of reported free
headroom. These estimates can change and are allocation guidelines, not
guarantees. See the [Vulkan budget/usage contract](https://docs.vulkan.org/refpages/latest/refpages/source/VkPhysicalDeviceMemoryBudgetPropertiesEXT.html).

The policy also retains 10% of heap capacity, and of host capacity for
software/host-visible memory. If the driver budget is unavailable, diagnostics
say `heap-size-estimate`; host capacity is a bound, not measured free RAM.
Compatible device-local heaps are considered without changing the GPU,
backend or queue. If no compatible heap admits the request, the error records
required bytes, owned bytes, limit and the estimate source.

Budget pressure never frees a published image, recording or texture still used
by Vulkan or Qt. Existing producer/copy and final Qt consumer fences determine
reuse and release. Timeouts cannot authorize retirement. No pixel downscaling,
CPU readback, altered blur kernel or extra frame admission is introduced.

Qt diagnostics expose `quickTextureBudgetBytes`, `quickPeakTextureBudgetBytes`
and `quickTextureBudgets` with heap ownership, limit/source and rejected
allocation requests. Rejections may be proposals for a full heap before another
compatible heap is selected. Reserved allocation bytes remain separate from the
allowance and from total process VRAM. Successful shutdown records zero owned
bytes and zero current allowance; a failed drain retains its actual ownership.

CPU regression tests exercise growth, pressure with live allocations, retirement
shrink, pressure recovery, host/device bounds and overflow. The Vulkan fixture
retains two unfinished multi-layer banks above 128 MiB, then verifies resize
shrink and last-consumer shutdown. These are allocation/lifetime checks, not
scanout, physical input or hardware overlap qualification.
