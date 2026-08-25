# ADR-001: Reference sources and provenance

- Status: Accepted
- Date: 2026-08-01

## Decision

Avalonia is a selectively vendored platform-engine seed, and the pinned Flutter checkout is an API and behavior reference. Doroti owns every runtime contract and lifetime. Reference assemblies and repositories never become runtime dependencies.

`migration/source-manifest.json` records each source root, license and audited file set. Any promoted or adapted file records its source path, exact revision or selected-content revision, source/adapted hashes, license, local changes and patches. `THIRD-PARTY-NOTICES.md` is the human-readable distribution index; manifests remain the machine-enforced record.

## Consequences

Unproven legacy code stays in migration tests. A source with missing license, revision, hash, dependency closure or provenance cannot be promoted.
