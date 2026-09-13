# PlatformView validation

`python Doroti/validation/platform-views/record.py windows-product-navigation` (from the repository root)
checks the Material sample's Platform views destination in the bottom bar and navigation rail,
including native editor visibility and disposal/recreation across page navigation.

From the repository root:

```powershell
python Doroti/validation/platform-views/record.py common
python Doroti/validation/platform-views/record.py windows-stacking
python Doroti/validation/platform-views/record.py windows-attachment
python Doroti/validation/platform-views/record.py web-dom
python Doroti/validation/platform-views/record.py web-typescript
python Doroti/validation/platform-views/record.py product-build
python Doroti/validation/platform-views/record.py windows-product-build
python Doroti/validation/platform-views/record.py windows-product-overlay
python Doroti/validation/platform-views/record.py windows-product-live
python Doroti/validation/platform-views/record.py testbed-build
python3 Doroti/validation/platform-views/record.py macos-attachment
python3 Doroti/validation/platform-views/record.py macos-product-build
python3 Doroti/validation/platform-views/record.py macos-product-live
python3 Doroti/validation/platform-views/record.py macos-interleaved
```

Each command invokes `run-with-timeout.py` (1200 s) and records command, SDK scope,
exit status, HEAD, dirty paths, source hashes and evidence classifications under
`Doroti/artifacts/platform-views/<date>/<gate>/<run>/` (one directory per attempt). Run .NET gates sequentially
because project references share intermediate output directories. `product-build`
uses the .NET 10 SDK selected from the Doroti directory and serial MSBuild.

`common` exercises lifecycle races, identity, manifest, legacy wire codec and two-owner
channel callbacks, scene/retained ordering, NativeOverlay foreground rejection, rollback/retirement, real CPU Skia segment
pixels and bounded surface pooling. Fake-host results do not certify native presentation.

`windows-stacking` compares topmost DComp against a lower target with WS_CLIPCHILDREN
using desktop pixel readback. Its BM_CLICK verifies native command wiring, not real
pointer input or physical display behavior. `windows-attachment` runs the product HWND
factory in an independent WinForms UI-thread harness, including hidden/clipped/detached
focus rejection, parent-thread/reparent checks, current-DPI bounds, editing-state preservation,
parent destruction before coordinator cleanup and 100 cycles. It does not change monitor DPI.
Neither test qualifies WindowsAppSdk or MAUI product B/C.

`web-dom` uses the existing web-playwright installation to run the product DOM registry
in Chromium at DPR 1/1.25/1.5/2. It checks real pointer/shield routing, iframe preservation,
stale packet rejection and late-create cleanup. It does not run Graphite WebGPU/WebGL,
transfer OffscreenCanvas segments or qualify PV-4C. Browser screenshots are preserved.

WindowsAppSdk registers the Testbed's optional PlatformView manifest. Select
`DOROTI_TESTBED_MODE=platform-views` and `DOROTI_PLATFORM_VIEW_COMPOSITION=interleaved`
for live HWND/Graphite raster interleaving, or `overlay` for the constrained B fixture.
The Windows path currently requires Graphite/Vulkan. Other presenters report unsupported
native composition; Windows MAUI is a separate integration.

`windows-product-live` executes the real Testbed, checks ten window-pixel overlap states,
native HWND and actual EDIT text preservation, tagged synthetic shield/native pointer
delivery, Tab/Shift+Tab, modal barrier, twenty native create/dispose cycles, GDI counts,
and normal process close. `windows-product-overlay` checks explicit B separately.
The C transport uses a shared-recorder GPU atlas, bounded readback and two banks of
premultiplied layered HWNDs. Timing samples are observations, not performance acceptance.
Physical display atomicity, Korean IME, complete UIA and other DPI/device-loss paths remain
unverified. Product evidence is distinct from the independent attachment/stacking probes.

The AppKit runner now registers the generic factories from its optional manifest.
Select `DOROTI_PLATFORM_VIEW_COMPOSITION=overlay` for its explicit NativeOverlay
fixture; `interleaved` selects the AppKit multi-surface compositor fixture.
`macos-attachment` runs actual NSControls in an isolated AppKit window harness;
`macos-product-live` requires the built Debug Testbed and executes its real Metal
scene path with both Graphite and Ganesh. See [AppKit scope and limitations](../../docs/platform-views/appkit.md).

`macos-interleaved` runs ten stable-instance overlap states on Graphite and Ganesh,
checks real window PNG pixels, native state/identity, ordered hit targets and synthetic
shield tap counts. Physical input, IME, VoiceOver and complete PV-5/PV-10 remain unverified.
