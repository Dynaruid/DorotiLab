# Android Graphite surface orientation — 2026-09-12

## Failure and correction

On Galaxy S25 (SM-S931N), rotating the phone left Android's status/navigation bars
correctly oriented while the Material scene appeared sideways and stretched.
The WSI swapchain advertised `preTransform = currentTransform`, although Graphite
painted unrotated SurfaceView coordinates and copied those pixels without rotation.

Android now selects `IdentityBitKhr` and checks that the surface supports it.
Android composition applies orientation to the unrotated image. The host's view
metrics, pointer coordinates and GPU copy/retirement path are unchanged. Qt retains
its existing transform selection. This does not implement app-side pre-rotation
or claim a performance improvement.

The first correction restored the image, but its logs exposed repeated swapchain
recreation: Android returns `SuboptimalKhr` when identity differs from current
orientation. The final correction accepts that usable result on Android, while
retaining recreation for `ErrorOutOfDateKhr` and SurfaceView size changes. Android
also uses the requested SurfaceView extent clamped to supported bounds, avoiding
the stale `currentExtent` observed during rotation. Desktop extent rules remain unchanged.

The [Android WSI implementation](https://android.googlesource.com/platform/frameworks/native/+/85b74b7e07/vulkan/libvulkan/swapchain.cpp)
inverts the declared pre-transform for native-window composition; the declaration
must match what the renderer actually did. App-side rotation is an alternative
that also requires rotating rendering and matching extents, as described in
[Khronos surface rotation](https://docs.vulkan.org/samples/latest/samples/performance/surface_rotation/README.html).

The repository's Flutter reference at `56b8e1a851a594b1a154f8ea93270807dab22b9a`
also selects identity in
`engine/src/flutter/impeller/renderer/backend/vulkan/swapchain/khr/khr_swapchain_impl_vk.cc:193`
and `engine/src/flutter/vulkan/vulkan_swapchain.cc:140`.

## Verification

- Release arm64 build and automatic AAB/bundletool split deployment succeeded.
- Official SkiaSharp Graphite/Vulkan started on Adreno 830; the first portrait
  swapchain reported requested/actual extent `1080x2340`, identity current/pre-transform.
- The first correction's portrait and `2340x1080` landscape screenshots were
  visually checked; the user confirmed that landscape now looks normal. That
  run also recorded physical portrait/landscape transitions. Its 26 swapchain
  generations exposed the repeated recreation addressed by the final correction.
- Final Release split deployment succeeded. Process 27858 started in portrait
  (generation 1, `1080x2340`) and followed the user's physical rotation to
  landscape (generation 2, `2340x1080`), with requested/actual sizes matching.
  The final landscape screenshot is upright and undistorted.
- ADB taps at the displayed Color and Components tabs selected the intended
  screens. Both were visually checked. The generation stayed at 2 throughout
  these interactions; no repeated swapchain creation or fatal exception/signal
  appeared in the captured process log. This is a bounded interaction check,
  not a general frame-time/performance result.
- The phone's rotation preferences were not changed by this task.
- No full platform, performance or synchronization-validation claim is made.

Evidence directory: [android-landscape](../../artifacts/android-landscape/).
Original [failure screenshot](../../artifacts/android-landscape/before.png),
[build/deployment log](../../artifacts/android-landscape/build-run.log),
[post-install log](../../artifacts/android-landscape/logcat-after.txt) and
[portrait screenshot](../../artifacts/android-landscape/after.png) are retained.

Final evidence: [build/deployment](../../artifacts/android-landscape/build-run-final.log),
[process log](../../artifacts/android-landscape/logcat-final.txt),
[landscape](../../artifacts/android-landscape/final-landscape.png),
[Color tap](../../artifacts/android-landscape/final-color.png),
[Components tap](../../artifacts/android-landscape/final-components.png).
The app remains open for user inspection; the desktop log follower is stopped
after verification. Reverse-landscape and a full repeated-rotation matrix were
not run, and the first-correction transition evidence is kept distinct from the final build.
