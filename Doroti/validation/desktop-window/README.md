# Desktop window implementation evidence — 2026-09-25

Overall **PARTIAL**. This implements the main-window desktop contract and a
Windows MAUI adapter, not the entire W0–W5 plan. The authoritative remaining work
is in the root [work.md](../../../work.md) and the
[API/support document](../../docs/desktop-windows.md).

All build/test commands used `run-with-timeout.py` (1,200 seconds). Shared build
outputs and native GUI probes were used serially. No hundreds-of-iterations
stress test or physical-display qualification is claimed.

| Gate | Result | Evidence / limits |
| --- | --- | --- |
| Core/Widgets Release build | PASS | Warnings-as-errors projects, zero warnings/errors |
| Fake-host contracts | PASS, 25/25 | Identity, two independent controllers/content factories, stale handles, hooks/readiness, cancellation, rollback, close coalescing/cancel, subscription removal, serial appearance updates, request supersession, close during creation, last close vs pending creation |
| Windows MAUI Release / Release x64 | PASS | Source Testbed with separately compiled desktop companion |
| Native control probe | PASS | Windows 200% DPI: hidden-ready state; client 450×800; min client 350×500 via WM_GETMINMAXINFO; resize 500×650; title; show/hide/focus; maximize/restore; minimize/fullscreen/windowed; appearance replacement/reset; first native close canceled, second clean exit |
| Caption Backdrop + Acrylic body | PASS, automated pixels | Body response 84/14/102; caption response 193/30/233; DWMWA_SYSTEMBACKDROP_TYPE=3; clean exit |
| Caption Solid + Acrylic body | PASS, automated pixels | Body response 84/14/102; caption response 0/0/0; DWM type=1; clean exit |
| Caption System + Acrylic body | PASS, automated pixels | Body response 84/14/102; caption response 0/0/0; DWM type=1; clean exit |
| Existing resize operations | PASS, 24 resizes | 41 native prepared frames, 0 native resize timeouts, Graphite device creations=1, final physical 934×629, clean exit. This is not visible-continuity proof |
| Displayed-frame flicker gate | NOT PASSED / PARTIAL | Current run failed before capture: after 961px outer-width setup, renderer evidence stayed 934px while native client expected 935px. No new geometry-continuity PASS; earlier PARTIAL stays open |
| External core/widget package consumer | PASS | 15 local nupkgs; fresh temp consumer/cache; no source ProjectReference in assets; same 25 contracts |
| Package negative builds | PASS | Web, Android, iOS, MacCatalyst each fail with DOROTIDESKTOP001 |
| External Windows package consumer | PASS | Local App/Runner SDK + Windows MAUI packages; common app + desktop companion; generated bootstrap; native control probe and clean exit |
| Full Web/mobile runner builds | notRun | Negative package builds do not substitute for full platform application builds |
| First-display frame sequence / physical appearance | notVerified | Readiness and native/pixel automation are not a complete first-visible-frame capture or physical-display acceptance |
| 100/150% / mixed DPI, high contrast, transparency off | notVerified | No OS-wide settings were changed to manufacture results |
| Hidden/custom chrome, chrome widgets, App theme bridge | notImplemented | Requests rejected; native input/Snap/IME/accessibility gate remains open |
| WindowsAppSDK/AppKit/Qt adapters | notImplemented | New desktop startup rejected; existing host paths remain intact |
| Native multiple windows / owners | notImplemented, W6 | Fake two-window PASS is not native multi-window support |

## Reproduction

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/desktop-window/Contract.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release -p:Platform=x64
python Doroti/validation/run-with-timeout.py python Doroti/validation/desktop-window/verify-windows.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-acrylic.py
$env:DOROTI_DESKTOP_CAPTION = 'Solid' # also test 'System'; remove for Backdrop
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-acrylic.py
Remove-Item Env:DOROTI_DESKTOP_CAPTION
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-resize.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/desktop-window/verify-package.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/desktop-window/verify-windows-package.py
```

`verify-windows.py` starts only its own process and verifies the sample's observed
state against native client/frame limits. `DOROTI_DESKTOP_SAMPLE=acrylic` selects
the transparent-base native Acrylic options. The existing common Testbed still
owns its Scaffold opacity toggle; this does not test runtime material mode off/on.
Mode/base-transparency changes return RequiresRecreation, not success.

The shared window finder now excludes DWM-cloaked HWNDs. WS_VISIBLE alone is not
the displayed application window during the new hidden first-frame preparation.

## Fixes found by actual execution

- Default native maximum track size clamped the initial 800 DIP height to 779.5
  at 200% DPI. The adapter preserves explicitly requested client size while
  translating native frame/min/max bounds per-window DPI. A window can exceed
  the monitor work area; it is not silently reported as 450×800 after clamping.
- MAUI's title mapping runs after HandlerChanged. Reassert title and caption
  appearance after the content root loads, still under DWM cloak.
- MAUI 10.0.90's caption template part became visible again during Resizable
  changes and attempted PreferredHeightOption on a native caption. The adapter
  preserves the public template part's collapsed state while native chrome owns
  the caption, and unregisters that callback on close.
- Native close now releases window-owned render/material/subclass resources
  before HWND destruction, then removes the controller and evaluates app exit.
- Queued creation prevents premature last-window exit; a native window that
  closes during initialization is never inserted as a live registry entry.

## Evidence retention

The small [results JSON](results-2026-09-25.json) preserves measured summaries and
binary hashes. Raw PNGs/logs/local nupkgs live under `Doroti/artifacts/validation`
and are disposable. Fresh package consumers live in uniquely named OS temporary
directories. These are local execution artifacts, not permanent evidence links.

Captured source build: working tree based on `351d4dd2`. The snapshot includes
uncommitted implementation changes. Never identify that base commit alone as the
tested implementation. Host hashes and execution directories are recorded in
the JSON; later source/binary changes require a fresh run rather than reusing
these PASS results.
