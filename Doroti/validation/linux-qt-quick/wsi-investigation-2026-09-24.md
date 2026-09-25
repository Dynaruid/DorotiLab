# Qt Quick Vulkan / XWayland resize investigation — 2026-09-24

**Unresolved for xcb on XWayland.** This is Vulkan swapchain creation during a window resize. The validation layer reported `VUID-VkSwapchainCreateInfoKHR-pNext-07781`: Qt requested an image extent of 720×640 while the X surface allowed only 757×677. The [Vulkan valid-usage rule](https://docs.vulkan.org/refpages/latest/refpages/source/VkSwapchainCreateInfoKHR.html) requires the requested extent to be within the surface capability bounds. The process can exit 0 despite this validation failure.

The [Qt 6.10.2 QRhi Vulkan source](https://github.com/qt/qtbase/blob/v6.10.2/src/gui/rhi/qrhivulkan.cpp#L2204-L2332) obtains surface capabilities in `recreateSwapChain()`, but supplies the earlier `swapChainD->pixelSize` as `VkSwapchainCreateInfoKHR::imageExtent`. A resize can make that size stale while the function waits for the device and constructs the new swapchain. This is an inference from the source and the captured extent mismatch; a Qt-side correction has not been built or qualified here. [Qt 6.11.2 source](https://github.com/qt/qtbase/blob/v6.11.2/src/gui/rhi/qrhivulkan.cpp) still shows the same extent assignment, so changing Qt version alone is not a verified fix.

## Evidence in this VM

Ubuntu 26.04.1, Qt 6.10.2, KWin Wayland, xcb through XWayland, Mesa llvmpipe; `VK_LAYER_KHRONOS_validation` was mapped for every result linked below. The standalone `wsi/` fixture links Qt Core/Gui/Quick and Vulkan headers; it has no Doroti or WebEngine shim. It now requests Qt Quick's preferred Vulkan instance extensions, matching the product and removing a separate Wayland color-space-extension VUID from the isolation test.

| Run | Result |
| --- | --- |
| [Qt-only xcb, 100ms sync delay, attempt 1](results-2026-09-24/qt-only-wsi-final-xcb/result.json) | No VUID in this attempt; 15 swaps. |
| [Qt-only xcb, 100ms sync delay, attempt 2](results-2026-09-24/qt-only-wsi-final-xcb-attempt2/result.json) | **Failed** `07781`; Qt requested 720×640, surface required 757×677; 14 swaps, fixture exit 0. [Raw log](results-2026-09-24/qt-only-wsi-final-xcb-attempt2/fixture.log). |
| [Qt-only native Wayland, same delay](results-2026-09-24/qt-only-wsi-final-wayland/result.json) | Passed, 16 swaps, no VUID in this attempt. |
| [Release product native Wayland, 10 resizes](results-2026-09-24/wsi-wayland/result.json) | Passed with mapped layer. |
| [Release product xcb/XWayland, 10 resizes](results-2026-09-24/wsi-xcb/result.json) | Passed in this attempt only; [2026-09-23](work3-results-2026-09-23.md) recorded a mapped-layer failure of the same VUID. |

An experimental Qt-only fixture deferred resize delivery by 10, 30 and 60ms while retaining the 50ms sync delay. All three configurations still emitted the extent VUID. The experiment was removed; no application debounce or swapchain override was installed. Repeated passes do not establish an xcb fix because the failure is intermittent.

## Current use and closure condition

On a Wayland session, run the Qt Quick product through the native Wayland QPA plugin instead of forcing xcb:

```sh
QT_QPA_PLATFORM=wayland dotnet <published-app>.dll
```

This selects a different window-system path, not a repair of Qt's xcb swapchain handling. Native Wayland still needs qualification on the target compositor and GPU. An xcb/XWayland Vulkan qualification requires a Qt/WSI change followed by repeated layer-loaded Qt-only and product resize runs, with zero VUIDs, correct dimensions, and no hidden frame or lifecycle regressions. Doroti must not rewrite Qt's owned swapchain or remove its GPU lifetime drain to suppress this validation error.

`verify-wsi.py` now returns failure for a VUID even when the Qt-only fixture exits 0. It records the loaded-layer assertion, swap count, VUID IDs and raw log in a new artifact directory.

## Desktop adapter regression — 2026-09-26

The new Desktop window probe reproduced `07781` on xcb/XWayland with the
validation layer mapped: requested 540×480, surface capabilities 500×450,
process exit 0. Window commands and close cancellation passed, but the overall
run is **failed**. The final native Wayland Desktop runs passed with a mapped
layer. This does not close the xcb issue; see the [Desktop execution record](../desktop-window/README.md#linux-qt-quick--2026-09-26)
and its tracked result summary.
