# Validation contracts and evidence

This directory contains the small, active validation inputs and committed summaries for Doroti.

- `contracts/product-naming-map.json` is read by the Web template/package validation.
- `evidence/app-targets-evidence.json` is written by `eng/validate-app-targets.ps1 -Shard Evidence`.
- `evidence/web/` contains the current Web aggregate and browser-manual summaries written by `eng/validate-web-product.ps1`.

Machine-local traces and generated build output belong under `.doroti/` or `artifacts/`. Historical migration inputs and milestone evidence were removed from the active tree; older summaries remain under the repository `history/` archive.
