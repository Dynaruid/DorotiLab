# Native frame pipeline

The shared [policy](../src/Doroti.Skia.Rendering/NativeFramePipeline.cs) uses
C as the only native frame policy. Web is independent. VariableBlur's Fast
adaptive kernel, sigma, capture bounds, DPR and default scale 0.25 are unchanged.

Framework preparation precedes normal GPU admission. New shader-only scenes
use up to two logical GPU frames and asynchronous output where supported.
Native/shield, replay and resize/rotation work drains outstanding frames before
serial admission and required output synchronization. A backend without
two-frame capability uses one frame within the same C policy.

Use no frame selector, or `DOROTI_NATIVE_FRAME_MODE=C`. A/B and unknown values
fail configuration. All nonempty legacy frame/presentation selectors were
removed, including values that previously selected C. Hosts validate settings
during initialization; Android checks both environment and Intent extras.
See the [API and configuration migration](migrations/native-frame-c-only.md).

Framework requests coalesce into one pending callback and renderer submissions
retain one newest pending scene. GPU retries rasterize a prepared scene without
consuming a callback scheduled for the next pulse. The renderer atomically
rechecks fresh shader eligibility against current viewport/context generations.
Native insertion, shields, replay and resize do not record ahead of old GPU work.

Two logical frames are distinct from front images, Qt texture banks and DXGI
back buffers. Each producer recording keeps private resources. Vulkan copy
completion retires the recording; D3D12 copy completion or Qt sampling completion
separately permits a shared texture bank to be reused. A present receipt is not
scanout or GPU completion. Timeouts retain GPU-owned resources; only confirmed
completion or context loss permits retirement.

Qt R/P image allocations use a [dynamic texture allowance](dynamic-texture-budget.md)
based on actual image demand, heap/host capacity and available driver budget.
An allowance change never authorizes early release of GPU-owned images.

| Host | Normal C ownership and retirement | Serial fallback |
| --- | --- | --- |
| UIKit / Catalyst | Metal completion owns recording/drawable lifetime | Native, replay, rotation/resize |
| AppKit | Prepare before `_inFlight` and drawable admission; Metal completion | Native/replay/layout, Ganesh |
| Android | Choreographer preparation before Vulkan frame slot/acquire; Vulkan fences | Native/shield/replay/swapchain resize |
| Windows App SDK | Two private Vulkan banks; external GPU signal and D3D12 consumer fence | Ganesh, native/replay/resize |
| Windows MAUI | UI preparation before raster admission; two Vulkan source banks, deferred D3D12 copy/present | Embedded CompositionDrawingSurface, Ganesh, native/replay/resize |
| Qt Quick | `beforeFrameBegin` prepares before QRhi admission; same-queue copy and consumer fence after `afterFrameEnd` | Native/shield/replay/resize; basic render loop required |
| Qt Vulkan window | Prepare before managed slot/acquire; Vulkan completion polling | Replay/native/resize |
| Qt OpenGL | Existing serial Ganesh context | Two-frame capability is unavailable |

Qt host ABI is **6**, with a **208-byte** callback table. Consumer completion
uses offset 192 / feature bit 21; preparation uses offset 200 / feature bit 22.
Managed host and app-owned native shim must be rebuilt together. Sample2,
Testbed and template copies have the same contract. Qt queue operations stay on
the existing basic-loop owner. Threaded Quick is not newly supported.

Opt-in diagnostics expose mode, capability/fallback, pending maximum and
retirement evidence. They describe submission/consumer state, not displayed FPS
or hardware execution overlap. The bounded collector and physical-input limits
are described in [tests](../tests/README.md). Current builds, raw failures,
source/payload identities and open acceptance items are in the
[2026-10-02 result](../../works/results/2026-10-02-native-frame-pipeline.md).
