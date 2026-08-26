# ADR-006: Public API and v0.x compatibility

- Status: Accepted
- Date: 2026-08-01

## Decision

During v0.x, source-breaking native API changes require a changelog entry and explicit migration note. A Flutter compatibility claim is tied to one exact baseline revision and per-symbol support state; assembly version similarity is not a compatibility claim.

## Consequences

Architecture tests inspect exported member types. The original exact project graph assertion is superseded by goal2 T0's manifest-driven layer and forbidden-edge policy so reviewed source-port projects can be added without weakening backend-leak diagnostics.
