# GPU shared raster transport — 2026-09-21

Android and Windows PlatformView interleaving now publish Doroti raster content
without a GPU → CPU pixels → GPU round trip on the supported shared-buffer path.
Native WebView content is still rendered by its native provider and is never
captured into these buffers.

## Implementation

- `VulkanSharedRaster` records a private Graphite image R in the active recorder,
  then copies it on the same Vulkan queue to an imported output P. R's observed
  layout is restored; P is released to the external/foreign queue in GENERAL.
- Android: immutable RGBA AHardwareBuffer → hardware Bitmap → existing
  View/RenderNode/RenderEffect hierarchy. The producer fence completes before
  wrapping/publishing. Bitmap/native references retain the allocation after the
  Vulkan import is destroyed. Published buffers are never overwritten/recycled.
  Android 29+ and both AHB/foreign-queue Vulkan extensions are required. Older
  or unsupported devices retain the bounded readback compatibility path.
- Windows: immutable BGRA D3D11 shared texture on the Vulkan adapter, imported
  into Vulkan, then GPU copied/drawn into Windows.UI.Composition WebView surfaces,
  Microsoft.UI.Composition WinUI backdrop sources, or DirectComposition HWND
  raster surfaces. There is no managed pixel atlas/readback in this host path.
  Original D3D resources have separate producer and UI-handoff references because
  KMT handles do not themselves retain an allocation. Native CPU-upload exports
  remain for ABI compatibility, but the current managed host does not use them.
- Retained-content/translation caching and stable native View/Visual identity
  remain active. Failed GPU retirement cannot release an in-flight source.
- Removing the last WebView now returns to the primary output and hides obsolete
  backdrop-only HWNDs. This fixes an input obstruction found by removal/remount
  validation. GPU data readiness and actual display completion remain separate.

This removes CPU **pixel transfer**, not all CPU synchronization: changed raster
publications still wait for the producer fence. Outputs are immutable allocations;
no consumer-fence-based output pool or end-to-end frame-rate guarantee is claimed.

## Validation

Evidence root: `Doroti/artifacts/platform-views/2026-09-21/gpu-sharing/`.
Builds and validation commands used the repository's 1,200-second wrapper.

| Check | Result |
| --- | --- |
| Windows native + product Release build | PASS, zero errors/warnings |
| Windows final-source WebView effects gate | PASS, 11 mounted-product checks including touch forwarding, shields, movement, resize, removal and recreation |
| Windows final-source transport | 111 commits, 122 GPU raster copies, 166 retained reuses, zero CPU readback/upload bytes |
| Windows Vulkan validation layer | Enabled; zero validation errors/warnings, zero device-loss results |
| WinUI controls | Six modes, OS-injected input checks, blur pixels and 30-frame continuity PASS; active transport tested before final source-lifetime/empty-scene cleanup |
| Android x86_64 Release build/install | PASS, zero errors/warnings; installed on `emulator-5554` |
| Android final-source drag to bottom and re-drag | PASS, two isolated injected drags, two Down/Up pairs, no Cancel |
| Android final-source transport | 223 commits; 6 changed/663 cached slices during recording; zero readback bytes; 34 cumulative shared slices |
| Android final-source UI commit timing | Median 0.296 ms, maximum 2.476 ms; this is only native commit work, not total frame time/input latency |
| CSS pulse disappearance | No missing block in 419 captured frames, including drag; no frame-pacing claim |
| Linux runtime | `skippedByUser` |
| Physical Android device / human input / sustained frame pacing | `notVerified` |

Android runs on the existing Android 13 x86_64 emulator with its host NVIDIA GPU.
Its WebView 109 does not support the default transient-profile isolation contract.
The testbed launch option `--es doroti_sample_webview_profile shared` explicitly
selects a persistent shared profile for the sample; ordinary launches keep the
default transient profile. The framework never silently downgrades that policy.

The emulator's UIAutomator cannot reliably reach idle during the CSS animation.
`measure-android-panel.py --visual-header X Y W H --require-gpu` instead tracks an
inspected title-text crop in screenshots, requires >= 0.75 match confidence, and
checks isolated input counts plus zero readback. The final title moved from y=383
to y=935 and returned to y=383. Screenshots retain the system navigation area.

Pulse analysis uses the unobscured ROI `(470,900,585,150)`, with the panel moved to
the right before vertical drags. The original saturation-only detector reported
16 low-color frames; inspection showed the block remained visible while its
animated color became nearly neutral. The explicit `--metric dark` contrast test
on the same video found 10,875–10,911 block pixels in every frame (threshold 1,000).
The ROI contains no other dark content. This is a disappearance check, not a
promise that every display frame meets its deadline.

Final evidence: `windows-final-source/observed.json`, `windows-final-source/report.json`,
`winui/result.json`, `android-final/result.json`, `android-drag.mp4`,
`android-pulse-contrast/result.json`, build logs, APK and source hashes.

Official API contracts used: [Vulkan AHardwareBuffer sharing](https://docs.vulkan.org/refpages/latest/refpages/source/VK_ANDROID_external_memory_android_hardware_buffer.html),
[Android hardware Bitmap wrapping](https://developer.android.com/reference/android/graphics/Bitmap#wrapHardwareBuffer(android.hardware.HardwareBuffer,%20android.graphics.ColorSpace)),
[D3D11 resource sharing](https://learn.microsoft.com/en-us/windows/win32/api/d3d11/ne-d3d11-d3d11_resource_misc_flag).
