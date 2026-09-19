# Android work1/work2 results — 2026-09-20

**Overall: PARTIAL.** Source implementation, scoped product API/input/effects and
deployment passed. Native-origin GestureArena, full acceptance and performance
budgets are not complete. Only Galaxy reconnection and physical-device verification
of the last color-after-blur refinement are `skippedByUser`, following the user's
explicit `skip` after USB disconnected. Earlier Galaxy evidence remains valid for
its tested revision; other remaining gates are not covered by that skip.

Start HEAD: `e38eca7204a15608bdb7ce7dec4aef73526a883f`, initially clean worktree.
Evidence: `Doroti/artifacts/webview/2026-09-20/android/`.

| Evidence boundary | Result / artifact |
|---|---|
| sourceReviewed | Same native instance/controller/session; bounded native JS; provider profile/content/message; sigma conversion; signed Android device IDs; RID/TFM reference repair |
| build | Final color-order arm64 canonical and x64 Release Mono AOT builds passed with zero warnings/errors: `build-effect-order.log`, `build-x64-effect-order.log`. Earlier full arm64 rebuild emitted one pre-existing Gradle SDK XML-version warning. |
| automated common | Negative/positive Android device IDs and distinct pointers fit signed Flutter IDs; typed factory error; two coordinator owners; late events, cancellation and placement/close isolation. `common-final.log` |
| productLive / API | Galaxy API36/provider151 on the last successful automatic split install: all command checks passed, including oversized/stale/child-frame rejection. Final color-order x64 API36/provider133 APK passed the same checks. `commands-split-final/`, `x64-color-order-commands/` |
| productLive / pixels | Galaxy passed 11 stages before the last color-order refinement: sigma 4→4.064, 16→16.256, native/raster agreement, zero/reset and saturation/tint. `calibration-final/pixels.json`. Final x64 emulator passed all 13 stages including colored edges: sigma 4→4.050, 16→16.052, color-after-blur midpoint RGB [149.5,68.5,147.5] versus analytical [148.608,68.608,148.608]. `x64-color-order/pixels.json`. Final Galaxy refinement check is `skippedByUser`. |
| automated physical-device input | 12 assertions including configuration: foreground tap once, pass-through/shield, Samsung Korean composing, native↔Doroti typing, rotation/Home-resume identity and text, second view, effect movement, disposal/recreation. `input-final2/results.txt` and screenshots |
| deployment | Canonical `run -NoBuild` deployed with bundletool, launched, and exited 0 after explicit testbed stop. Pulled base + arm64/ko/xxhdpi splits contain exactly one official arm64 `libSkiaSharp.so`. `run-canonical-final.log`, `installed/provenance.json` |
| final revision provenance | `current-source.json` records final source/APK hashes and official Skia verification. Final x64 APK was installed and pixel-tested. Final arm64 redeploy stopped at `No connected devices found` (`run-effect-order.log`); its last color-order refinement was not installed on Galaxy. Earlier installed split hashes are separate. |
| physical | Human touch/pen, human IME review and TalkBack navigation remain `notVerified`; ADB/onscreen-keyboard injection is automated input on a device. |
| nativeAot | Unsupported by the current runner: explicit Android publish request exited 1 at the iOS-only guard, before ILC/native link/run. `nativeaot.log`. Mono AOT is separate. |

## Performance observations

Final responsive fixture displays all four cells. Eight bounded cases completed
without process exit. Five-second warmup follows native/raster commit; `gfxinfo` is
reset before a four-second sampling interval. Owner-process PSS is a snapshot after
sampling, not total Chromium/GPU memory. These are window timing observations, not
physical scanout or input latency. Source/logs: `workloads-final/metrics.json` and the
per-case gfx/memory/composition files.
These measurements precede the final color-order refinement and use saturation 1;
they are not a repeat performance measurement of the final APK.

| Case | Window frames | p50 / p95 / p99 ms | Owner PSS KiB |
|---|---:|---:|---:|
| 0 idle | 0 | not observed | 266207 |
| 0 animation | 0 | not observed | 332614 |
| 1 idle | 267 | 9 / 10 / 11 | 421970 |
| 1 animation | 135 | 69 / 81 / 121 | 455267 |
| 1 native scroll + Doroti animation | 105 | 85 / 117 / 129 | 431936 |
| 4 idle | 280 | 10 / 13 / 13 | 479356 |
| 4 animation | 106 | 85 / 121 / 129 | 517384 |
| 4 modal + Doroti animation | 84 | 109 / 150 / 150 | 487144 |

Zero-view SurfaceView frames are not observed by this window `gfxinfo` counter.
The 4950 ms empty histogram value was discarded, not reported as a measured latency.
One-view animation accumulated 1,948,284,000 readback bytes across 362 readbacks over
the complete run, whereas idle accumulated 10,764,000 bytes across two readbacks.
Counters include warmup/startup and must not be divided by only the four-second
window sample. Current readback/cache behavior does **not** meet performance approval.
GPU transfer/HCPP alternatives and before/after budget acceptance remain open.

## Repairs established by failures

- Android target framework discovery dropped RID and read a stale RID-less assets
  file. Fixed direct and transitive target references; canonical build/run now pass.
- The Android manifest lacked the WebView HTML/CSS resources. Both ABI manifests now
  declare the existing embedded resources and hashes.
- Provider-loaded profiles reject immediate deletion after destroying a view.
  Await profile data clearing during disposal and remove empty shells next process.
- Android radius was incorrectly treated as sigma. Corrected mapping, measured pixels.
- Saturation now uses common luminance weights and applies after blur, matching
  the common effect order at clamped colored edges. Final emulator analytical
  colored-edge and zero-blur reset checks passed; final Galaxy repetition was skipped.
- Negative native device IDs overflowed the converter's signed ID. A shared bounded
  composite identifier fixes Graphite and the existing Android pointer subscription.
- Provider133 finished an unnecessary initial blank navigation after the actual app
  document had loaded. No synthetic blank load is now issued without initial HTML.
  A tested 1×1 hidden viewport workaround did not fix it and was removed.
- Input automation initially used fixed button coordinates after a label changed the
  Wrap layout, and the resumed keyboard obscured lower controls. The final test queries
  current semantic button bounds and hides the observed keyboard; product behavior
  is not inferred from that failed automation.

## Still open

Native-origin delayed GestureArena and nested parent scroll; full C1–C6/E1–E3 and
two live product owners; multi-touch/selection/autofill/TalkBack; renderer/device-loss
recovery; SurfaceView/video/protected source sampling; application policy callbacks
for popup/download/file chooser/permission/fullscreen; broad OS/provider/GPU matrix;
performance/total memory budgets; clean package-only template deployment and Android
NativeAOT support. The implemented default policy rejects/cancels these application
requests; it does not advertise an unimplemented policy API.

See [Android contract](../../docs/platform-views/android-webview.md) and
[reproduction](../platform-views/android/README.md).
