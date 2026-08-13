# ADR-010: Flutter package conversion and distribution promotion

- Status: Accepted
- Date: 2026-08-01

## Decision

Each converted Flutter package is generated into an isolated workspace with its source package identity, version, Flutter baseline, converter/IR versions, input/output hashes, unsupported diagnostics and behavior fixtures. Generated workspaces never become implicit inputs to the Doroti runtime build.

Distribution promotion requires deterministic regeneration, successful compile and tests, license/provenance closure and an explicit support report. A converted package may depend on the public FlutterCompat facade, never engine/backend/vendor internals.

## Consequences

R10 implements the C5 package pilots, local NuGet feed, symbols, project template, external consumer, trimmed/single-file/ReadyToRun publish and package-content audit. Candidate packages remain unpublished while the clean Windows VM, injected mapped-crash trace and release performance/resource capture in `migration/releases/r10-beta.json` are `not-verified`.
