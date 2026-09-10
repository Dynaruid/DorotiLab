# MQ-0–MQ-9 implementation evidence

Date: 2026-09-10. **Implementation and available automation completed; full platform product qualification remains incomplete.** `work.md` retains the original physical-device completion gates.

## Automated contracts and builds

| Evidence | Scope |
|---|---|
| [contracts.json](contracts.json) | Commands, exit codes and 1200-second limits for common contracts, framework-work, resize, Qt/Windows ABI, Web JSON wire, Scaffold/caret, full Material suite and browser environment fixtures. |
| [builds.json](builds.json) | MAUI Windows, Android arm64/x64, iOS arm64/simulator arm64/x64, Catalyst arm64, AppKit arm64; Web/Windows App SDK/Qt host builds and shared testbed build. Apple entries are managed cross-compilation against reference SDKs. |
| [products.json](products.json) | Windows App SDK, MAUI Windows, Web and Android x64 consumer builds; Web host, Windows native target and templates packages. |
| [source-provenance.json](source-provenance.json) | Pinned Flutter revision and SHA-256 evidence for reviewed framework changes and reference tests. |
| [coverage.json](coverage.json) | 30 MediaQuery fields/aspects × 9 host/device categories: owner, source API, minimum version, initial value, event, fallback and device status. |

The common harness checks **1,074 contracts**, including every mounted MediaQuery aspect, `of`, parent overrides, two-view settings, stale and identical metrics, DPI fractions, copy/removal semantics, SafeArea flags/minimum/nesting, SliverSafeArea reverse/RTL and observer lifetime. The Scaffold fixture additionally asserts actual body height and focused-caret reveal after a host IME event; it preserves the existing size-only Scaffold optimization. Full FCR-7 and resize contracts pass.

Windows native was rebuilt with MSVC and Windows SDK 10.0.26100.0. Qt native was built in Ubuntu WSL with Qt 6.10.2, including the guarded 6.9 safe-area and 6.10 contrast APIs. Their size/offset and managed adapter fixtures pass. Execution against Qt 6.5 remains `notVerified`.

## Actual execution

| Evidence | Observed result | Limits |
|---|---|---|
| [android-final.json](android-final.json) (latest APK), [earlier run](android-emulator.json) | API 33 x86_64 emulator, NVIDIA Graphite/Vulkan. Drawable and native view stay 1920×1200; DPR 1.5; IME 0→627→0 physical / 0→418→0 logical; bottom viewPadding remains 90 physical; surface generation unchanged; Scaffold avoidance disabled does not cause MAUI resize/pan; focused caret is visible above the keyboard. | Emulator automation, not physical Android. API24–29, API34 nonlinear scaling, foldable/cutout variants and other navigation/IME modes remain unverified. |
| [windows-native-smoke.json](windows-native-smoke.json) | Actual Windows App SDK Graphite/Vulkan startup, MediaQuery snapshot and normal owned-window shutdown; native presentation summary reports no failed frames. | No touch-keyboard, DPI-switch or full interactive matrix claim. |
| [maui-windows-smoke.json](maui-windows-smoke.json), [runtime](maui-windows-runtime.json) | Actual WinUI MAUI Graphite/Vulkan startup, native/drawable metrics, presented frames and clean shutdown. | No touch-keyboard or multiple-monitor qualification. |
| [browser-product.json](browser-product.json) | Installed Chrome 152.0.7977.83, AMD hardware WebGPU and WebGL2. Both render the diagnostic screen, resize to 900×700 and visibly apply reduced-motion without changing resize/surface generations. Page errors: zero. | Desktop full-page Chrome only. Mobile Safari/Chromium, Firefox, Safari desktop and embedded product matrix remain unverified. |

Screenshots: [Android caret](android-final-caret.png), [Android avoidance disabled](android-final-no-resize.png), [WebGPU](worker-direct-webgpu.png), [WebGL](worker-direct-webgl.png). Web screenshots are captured after compositor settling and checked for rendered pixel variation; a worker receipt alone is not considered visible-content evidence.

Physical Apple devices, native Apple rendering/linking/signing, every UIKit keyboard/scene/animation mode, Wayland/X11 interactive Linux output and the remaining hardware/browser combinations are **notVerified**. Linux/Apple package product qualification is not inferred from managed cross-builds, native Qt compilation or earlier renderer work.

## Issues found and fixed during execution

- Missing `LocalizationsResolver` constructor resolution and observer registration caused the initial Android MaterialApp without an explicit locale to fail. The pinned constructor and nullable locale-list handling are restored and tested.
- Calling `ScrollPosition.moveTo` through `ViewportOffset` passed the base declaration's null optional argument in C#. Normalizing it to the override's true default restores keyboard-triggered caret scrolling.
- `SystemTextScaler.Equals` prematurely cast the comparison operand and made its pinned `noScaling` case unreachable. The case now runs and has a regression check.
- Custom HWND startup rejected `AccessibilitySettings.HighContrastChanged` with HRESULT `0x80070490`. Both Windows providers use the desktop Win32 contrast query and settings broadcast.
- MAUI Windows' dedicated metrics worker cannot read XAML properties (`0x8001010E`). Native environment data is collected on the XAML thread before target publication; metrics/frame work stays on its existing worker.
- Web display-feature serialization cannot expose all of `Ui.Rect`'s computed aliases. Dedicated wire DTOs avoid case-insensitive JSON metadata conflicts; actual worker startup and a JSON fixture verify this boundary.
- RID-less MAUI restore and inner builds previously selected different assets paths, losing the new WindowJava reference in consumers. They now use one outer/inner restore path, with framework-specific output directories.

Earlier failure logs/screenshots are retained as diagnostic history. `android-host-prior-run.json` was copied from an old device evidence file and is explicitly excluded from current results. Playwright's bundled Chromium failed WebGPU adapter creation while loading `dxil.dll`; the successful product run uses installed Chrome. `browser-debug.json` records that environment issue. `web-publish-nobuild-attempt.log` records the failed no-build publish attempt; normal Debug publish succeeded.

## Reproduce

```powershell
python Doroti/validation/media-query-safe-area/run-validation.py all
python Doroti/validation/media-query-safe-area/windows-smoke.py native
python Doroti/validation/media-query-safe-area/windows-smoke.py maui
```

For the browser product run, publish the Web consumer, serve its `wwwroot` with `Doroti/eng/serve-isolated-web.py` on port 5197, then run `browser-product.mjs` through `validation/run-with-timeout.py`. It defaults to installed Chrome; `DOROTI_BROWSER_CHANNEL` selects another installed channel. Stop only the server process started for that run. Android diagnostic launch uses `--es doroti_testbed_mode media-query --es DOROTI_MAUI_EVIDENCE 1` on the testbed Activity. The APK SHA-256 in its evidence binds the observed run to that artifact.
