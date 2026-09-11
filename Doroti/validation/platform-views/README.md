# PlatformView validation

From the repository root:

```powershell
python Doroti/validation/platform-views/record.py common
python Doroti/validation/platform-views/record.py windows-stacking
python Doroti/validation/platform-views/record.py windows-attachment
python Doroti/validation/platform-views/record.py web-dom
python Doroti/validation/platform-views/record.py web-typescript
python Doroti/validation/platform-views/record.py product-build
python Doroti/validation/platform-views/record.py windows-product-build
python Doroti/validation/platform-views/record.py testbed-build
```

Each command invokes `run-with-timeout.py` (1200 s) and records command, SDK scope,
exit status, HEAD, dirty paths, source hashes and evidence classifications under
`Doroti/docs/validation/platform-views/<date>/<gate>/`. Run .NET gates sequentially
because project references share intermediate output directories. `product-build`
uses the .NET 10 SDK selected from the Doroti directory and serial MSBuild.

`common` exercises lifecycle races, identity, manifest, legacy wire codec and two-owner
channel callbacks, scene/retained ordering, rollback/retirement, real CPU Skia segment
pixels and bounded surface pooling. Fake-host results do not certify native presentation.

`windows-stacking` compares topmost DComp against a lower target with WS_CLIPCHILDREN
using desktop pixel readback. Its BM_CLICK verifies native command wiring, not real
pointer input or physical display behavior. `windows-attachment` runs the product HWND
factory in an independent WinForms UI-thread harness, including focus and 100 cycles.
Neither test qualifies WindowsAppSdk or MAUI product B/C.

`web-dom` uses the existing web-playwright installation to run the product DOM registry
in Chromium at DPR 1/1.25/1.5/2. It checks real pointer/shield routing, iframe preservation,
stale packet rejection and late-create cleanup. It does not run Graphite WebGPU/WebGL,
transfer OffscreenCanvas segments or qualify PV-4C. Browser screenshots are preserved.

The testbed fixture can be selected with `DOROTI_TESTBED_MODE=platform-views` after
a runner implements and registers the actual native compositor. It reports missing
capability explicitly on current runners.
