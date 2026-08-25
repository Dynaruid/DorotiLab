# Validation contracts and retained evidence

This directory retains Doroti validation contracts, fixtures, and committed evidence. The repository no longer ships standalone validation scripts under `Doroti/eng`, so the records here must not be read as commands that can be regenerated from the current checkout.

## Retained material

- `contracts/` contains product naming, Flutter source pins, Windows host ownership mappings, and staged host contract snapshots.
- `evidence/app-targets-evidence.json` records the last committed application-target result.
- `evidence/web/` contains retained Web aggregate and browser-manual summaries.
- `evidence/flutter-conformance/framework-parity-matrix.json` records the FCR-0 Flutter source slice, product/runtime/host closure, asset contracts, and static-risk ownership.
- `evidence/flutter-conformance/baseline-evidence.json` records the inventory result and target-specific baseline boundaries. Submitted or presented counters are not timing, performance, compositor, or visible acceptance.
- Platform-specific fixture directories remain available for direct source inspection or project-level execution where their own README documents a command.

## Evidence boundary

The staged Windows host records cover the historical F0-F7 source, bootstrap, top-level/child HWND, metrics, ANGLE/EGL surface, resize handshake, scheduler, and input/accessibility gates. Their source fingerprints and PASS values describe the checkout that produced each record; they do not establish the current checkout, physical scan-out, visible compositor continuity, Korean IME candidate behavior, Narrator/Accessibility Insights acceptance, or product deployment acceptance.

Machine-local traces and generated build output belong under `.doroti/` or `artifacts/`. Older milestone summaries remain under the repository `history/` archive.
