# Validation contracts and evidence

This directory contains the small, active validation inputs and committed summaries for Doroti.

- `contracts/product-naming-map.json` is read by the Web template/package validation.
- `evidence/app-targets-evidence.json` is written by `eng/validate-app-targets.ps1 -Shard Evidence`.
- `evidence/web/` contains the current Web aggregate and browser-manual summaries written by `eng/validate-web-product.ps1`.
- `evidence/flutter-conformance/framework-parity-matrix.json` pins the FCR-0 Flutter source slice, product/runtime/host closure, asset contracts, and static-risk ownership.
- `evidence/flutter-conformance/baseline-evidence.json` records the current inventory result and target-specific baseline boundaries. Existing submitted/presented counters are not a timing or performance PASS.

Run the compact FCR-0 gate with:

```powershell
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Fcr0
```

The gate fails on Flutter source hash drift, missing product/runtime/host/evidence ownership, missing shader/font/data contracts, and unclassified static candidates. It records `implemented`, `adapted`, `explicitUnsupported`, and `notVerified` separately; later differential, native live, physical, and performance gates remain explicit.

Machine-local traces and generated build output belong under `.doroti/` or `artifacts/`. Historical migration inputs and milestone evidence were removed from the active tree; older summaries remain under the repository `history/` archive.
