# ADR-016: Official Avalonia host boundary

Status: superseded for the product/default-template path by ADR-017 and goal2 A1. `Doroti.Host.Avalonia` remains comparison/history code.

- Status: Accepted
- Date: 2026-08-02

## Context

Doroti owns its Widget/Element/RenderObject lifecycle, DisplayList, frame/resource ACK semantics, FlutterCompat surface and compiler pipeline. Reimplementing every desktop application, window, dispatcher, input/IME, accessibility, monitor/DPI and packaging service is not a product differentiator. Expanding the selected Avalonia-derived vendor source into a full platform fork would also move Avalonia's UI, threading and composition closure into Doroti ownership.

## Decision

The only product platform shell is an official package dependency isolated behind `Doroti.Host.Avalonia`. H0 pins the stable `Avalonia.Desktop` package and release to `12.1.0` through central package management and the host lock file. The machine-readable `migration/host/avalonia-dependency-matrix.json` records the direct package, complete resolved closure, license and per-host distribution policy.

Only `Doroti.Host.Avalonia` may reference official `Avalonia` assemblies in product code. Its internal implementation may use Avalonia types, but its public API must expose only Doroti-owned or BCL types. Dedicated host tests and samples consume the host project rather than adding their own Avalonia package references. Host-neutral runtime, backend-neutral raster assemblies and generated consumer packages retain zero official Avalonia package dependencies. Doroti does not ship a separate native Win32 host, template option or fallback package.

`Doroti.Host.Avalonia` was added in H0 as a package/architecture boundary. H1 implemented the application/window lifecycle and DisplayList presentation vertical slice. H2 now connects the official host to Doroti's single frame clock, bounded mailbox, surface generation, immutable image resource and terminal ACK contracts while retaining the same public boundary.

## Relationship to ADR-001 and ADR-008

This decision supersedes ADR-001's statement that Avalonia can only be a reference source for the new default host: the official package is now an intentional runtime dependency of `Doroti.Host.Avalonia`. ADR-001's provenance rules still govern copied or adapted source.

This decision supersedes ADR-008's Win32 vendor/backend path. `Doroti.Backends.Win32` and `Doroti.Vendor.Avalonia.Win32` are removed rather than retained as conformance or fallback products. Historical evidence is preserved, while reusable golden/trace contracts move to host-neutral or Avalonia-host tests. The backend-neutral Skia framebuffer slice remains internal for raster/headless verification; it is not a platform shell. A new vendor-source exception requires evidence that the pinned official host API cannot satisfy the capability and a separate ADR/review, and may not recreate a dedicated Win32 host.

## Consequences

- `DOTARCH008` rejects official Avalonia assembly references outside `Doroti.Host.Avalonia`.
- The existing public API scan includes the host assembly and rejects Avalonia, Skia, vendor and native types.
- Release package inspection uses the host dependency matrix: `Doroti.Host.Avalonia` must depend only on the approved direct Avalonia package, while all other Doroti and generated packages must retain zero Avalonia dependencies.
- Architecture tests reject reintroduction of the removed Win32 backend/vendor project or package references.
- Avalonia upgrades must update the central pin, host lock file, dependency matrix, license record and architecture tests together.
