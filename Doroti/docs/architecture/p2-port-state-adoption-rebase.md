# P2 port state, adoption, and rebase

P2 connects every effective artifact to the compiler identity, resolved upstream graph, immutable generated base, and the exact user-owned inputs used to compose it. A successful `compile --port` publishes these deterministic documents beside `port-workspace.json`:

- `port-state.json` (`doroti.port-state/v1`) records compiler/source/package-graph identity, aggregate generated/manual/effective hashes, required fixtures, every manual input hash, and every effective artifact hash/origin.
- `provenance.json` (`doroti.port-provenance/v1`) identifies generated, replacement, extension, and platform-port ownership and attaches symbol/member plus reviewed generated-base hash where applicable.
- `source-map.json` (`doroti.port-source-map/v1`) extends the compiler's Dart span map with effective-path origins. Manual entries retain their user-owned source path and replacement target.
- `manual-snapshot/` stores each input below an origin and SHA-256 directory. It is review evidence, not an editing location.

The four documents and `manual-snapshot/` are compiler-owned and hash-verified on workspace reuse. Clean and cache-backed compilation must produce byte-identical state, provenance, source map, and snapshot inventories.

## Adoption

```powershell
dotnet run --project ../tools/Doroti.DartToCSharp -- adopt `
  --port migration/ports/c0-adoption/doroti-port.json `
  --symbol CounterModel `
  --output artifacts/adoption-counter
```

`adopt` accepts only `runtime-adoption` ports. It extracts the selected generated declaration into a review candidate with upstream library, revision, license, and adoption-base hash headers. The bundle includes `adoption-report.json`, adopted-product provenance, manual snapshots, and the required fixtures. It never accepts a product target and never writes product source; promotion remains an explicit later review action.

## Rebase

```powershell
dotnet run --project ../tools/Doroti.DartToCSharp -- rebase `
  --port migration/ports/c0/doroti-port.json `
  --source-revision package-revision-v2 `
  --previous-workspace ../.doroti/workspaces/<workspace-id> `
  --output artifacts/rebase-review
```

If `--previous-workspace` is omitted, the CLI selects the latest workspace with the same mode and selected symbol set. Rebase generates a fresh base in the review bundle without composing stale replacements or changing the port/workspace. `rebase-report.json` compares normalized declaration/member meaning, generated-file hashes, and previous/current manual hashes using these states:

| State | Meaning |
|---|---|
| `clean` | target-level upstream and manual inputs are unchanged |
| `manual-review` | replacement source or its generated-file context changed without target semantic drift |
| `conflict` | upstream meaning changed beneath a manual replacement |
| `upstream-symbol-removed` | a previously selected symbol/member disappeared |
| `fixture-required` | a compiler-owned target is new or semantically changed |

`conflict` and `upstream-symbol-removed` are blocking. The report preserves other states so automation and reviewers can require the named behavior fixtures before accepting a new port state.
