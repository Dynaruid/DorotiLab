# ADR-005: Resource leases and frame ACK

- Status: Accepted
- Date: 2026-08-01

## Decision

Resources cross commit boundaries as Doroti-owned `ResourceId` values. `IResourceRegistry.Retain` returns an explicit `IResourceLease`. Each committed frame receives exactly one terminal ACK outcome, including stale, failed and cancelled frames. Leases are released in deterministic commit order after that terminal outcome.

Vendor and backend adapters may own native resource objects, but cannot define frame ACK meaning or keep UI-owned objects alive.

## Consequences

R3 fixes the identity and lease ports. R5 implements immutable image snapshots, the registry, Presented/Stale/Superseded/Failed/Cancelled ACK states and retain/release trace evidence. Every terminal path releases its leases in commit order before publishing the observable ACK; disposal alone is not considered a frame ACK.
