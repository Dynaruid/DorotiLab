# ADR-007: Converter and generated-source promotion

- Status: Accepted
- Date: 2026-08-01

## Decision

Converter output is a review draft under `migration/generated` or a generated package workspace. It never overwrites hand-written runtime source. Unsupported syntax is an error with a visible diagnostic and retained declaration context.

Promotion requires pinned inputs and converter/IR versions, byte-reproducible output, compile and behavior checks, a human review, and provenance for every file entering `src`. G3-2's `review` and `diff` commands are read-only. `promote` writes an ordinary `.cs` atomically only when the v2 manifest is approved and its old-candidate/current-source/new-candidate comparison is conflict-free; `rebase` remains read-only. Compiler-general defects must be fixed and regenerated, not hidden by a product patch.

## Consequences

Runtime projects exclude all migration generated-candidate paths and tooling assemblies. Promoted files replace the generated marker with a reviewed-source marker so generated-code analyzer suppression does not survive promotion. Product builds reject explicit non-intermediate `.g.cs` compile items, and promotion-managed directories reject ordinary `.cs` files without manifest entries.
