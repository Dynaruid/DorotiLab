# Validation contracts and retained evidence

This directory retains Doroti validation contracts, fixtures, and committed evidence. The supported aggregate entry point is `pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite <suite>`. Target-specific scripts under `Doroti/eng` are maintainer diagnostics; a retained record is reproducible only when its referenced project, script, environment, and source fingerprint still match the current checkout.

## Retained material

- `contracts/` contains product naming, Flutter source pins, Windows host ownership mappings, workspace schemas, and staged contract snapshots.
- Platform-specific fixture directories remain available for direct source inspection or project-level execution where their own README documents a command.
- `evidence/` is currently empty and reserved for machine-readable summaries that are deliberately promoted into source control.
- Current generated reports belong under repository-local `.doroti/evidence` or `artifacts`; archived milestone results remain under `history/`.

## Evidence boundary

The current Windows product is the Windows App SDK 2.4 `HwndExactCpp` child-HWND host with managed hardware-D3D11 ANGLE/EGL/Skia presentation. Its current validator projects cover ABI/topology, exact-generation terminals, ANGLE runtime selection, first-surface ordering, input packets, automated IMM32/UIA, lifecycle/device recreation, and package provenance. Machine-local HwndExactCpp reports and captures are written under `.doroti/evidence`; the decision and final scope are summarized by [ADR-025](../docs/adr/ADR-025-windowsappsdk-hwndexact-angle.md).

The tested physical resize and mixed-DPI monitor-boundary behavior received user acceptance, but strict synthetic qualification and pixel/cadence failures remain failures. Automated IME/UIA and lifecycle results are not physical Korean IME candidate/caret, Narrator/Accessibility Insights, full DPI/monitor/device/window-management, installer, or deployment acceptance.

Older staged F0-F7 and WinRT/ContentIsland records remain historical or independent-spike evidence. Their PASS values describe only the source fingerprint and scope that produced them; they do not override ADR-025 or establish the current product path.

The 2026-09-01 Acrylic investigation is retained as independent A1 and B0/B1
spikes under `windows-dwm-redirection-alpha-spike`,
`windows-acrylic-content-island-capability`, and
`windows-acrylic-composition-spike`. Run their staged validators with:

```powershell
pwsh -NoProfile -File ./Doroti/eng/validate-windows-dwm-redirection-alpha-a1.ps1
pwsh -NoProfile -File ./Doroti/eng/validate-windows-acrylic-composition-b1.ps1
```

Validator PASS means the expected diagnostic decision was reproduced. The A1
manifest still records P0 `FAIL` because child redirection alpha is rejected,
and the B1 manifest records B2/P1 `FAIL` because safe bounded surface reuse was
not proven. Neither validator promotes Acrylic into the product host. See the
[gate result summary](../../history/26-09-01/windows-appsdk-acrylic-p0-p1-gate-results.md).

Machine-local traces and generated build output belong under `.doroti/` or `artifacts/`. Older milestone summaries remain under the repository `history/` archive.
