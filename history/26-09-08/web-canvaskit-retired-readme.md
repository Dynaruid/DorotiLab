# Retired CanvasKit README evidence

Archived 2026-09-08 when the user requested removal of the Web CanvasKit backend.
The following describes the earlier implementation; commands and assets are historical.

### CanvasKit qualification evidence (2026-08-31)

The targeted hardware-WebGL2 Chromium suite passed `5/5`. Its automatic evidence covers main/UI/Raster ownership (`.NET` 0/1/0 and CanvasKit 0/1/1), exact terminal/receipt accounting, UI heartbeat and input dispatch during a 100 ms Raster stall, DPR2 CSS 1080×720 with a 2160×1440 backing and no CSS transform, three bounded Raster Worker/canvas-lease replacements with resource replay, and malformed-protocol rejection followed by bounded recovery.

The full headless CanvasKit-mode run passed `16`, skipped `5` tests that target other renderer modes, and failed `0`. A separate package audit packed all seven required Doroti packages and restored, built, and published a clean package consumer with Node/npm poison shims; no Node/npm invocation occurred, and the five CanvasKit asset hashes matched from source through nupkg to publish. Missing-license and one-byte-tamper packages failed closed with `DOROTICK101` and `DOROTICK102` respectively.

```powershell
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 `
  -Configuration Release `
  -HeadlessOnly `
  -RendererMode worker-canvaskit-webgl `
  -TestFile tests/canvaskit-worker.spec.ts

pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 `
  -Configuration Release `
  -HeadlessOnly `
  -RendererMode worker-canvaskit-webgl `
  -TestFile tests/canvaskit-display-list.spec.ts
```

This does not prove removal of all legacy `HostPayload` or Web SkiaSharp dependencies, same-worker Skia/pixel-golden parity, composed runtime-effect filters, complete retained-output caching, comparative performance, 30-minute memory churn, compositor scan-out, physical 60/120 Hz resize, precision-trackpad behavior, Korean IME, or screen-reader acceptance. The package still contains the legacy Web Skia dependencies, so that cutover gate is `FAIL`; the other listed gates remain `PARTIAL` or `notVerified`. Firefox and Safari are not promoted to supported product paths.

Press `Ctrl+C` in the terminal that launched the app to stop it. See [Run by platform](../../DorotiTestbedApp/README.md#run-by-platform) below for Android, iOS, macOS, Linux, and Windows MAUI commands.

