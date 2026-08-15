# ADR-016: Official Avalonia host boundary

Status: superseded by ADR-017 and removed after the A2 replacement gate. The remaining text and evidence describe the historical H0-H4 implementation only.

- Status: Accepted
- Date: 2026-08-02

## Context

Doroti owns its Widget/Element/RenderObject lifecycle, DisplayList, frame/resource ACK semantics, FlutterCompat surface and compiler pipeline. Reimplementing every desktop application, window, dispatcher, input/IME, accessibility, monitor/DPI and packaging service is not a product differentiator. Expanding the selected Avalonia-derived vendor source into a full platform fork would also move Avalonia's UI, threading and composition closure into Doroti ownership.

## Decision

H0 isolated the official `Avalonia.Desktop` 12.1.0 package behind `Doroti.Host.Avalonia`. The historical `migration/host/avalonia-dependency-matrix.json` records that direct package, resolved closure, license and distribution policy; it is not a current dependency manifest.

During H0-H4 only `Doroti.Host.Avalonia` could reference official `Avalonia` assemblies. That exception, its tests and samples have been removed. Current product, validation and tooling projects must retain zero official Avalonia binary dependencies.

`Doroti.Host.Avalonia` was added in H0 as a package/architecture boundary. H1 implemented the application/window lifecycle and DisplayList presentation vertical slice. H2 connected the official host to Doroti's frame clock, bounded mailbox, surface generation, immutable image resource and terminal ACK contracts before the path was superseded.

## Relationship to ADR-001 and ADR-008

This decision temporarily superseded ADR-001 by allowing an official package runtime dependency. ADR-017 and A2 ended that exception; ADR-001 provenance rules continue to govern copied or adapted source.

This decision supersedes ADR-008's Win32 vendor/backend path. `Doroti.Backends.Win32` and `Doroti.Vendor.Avalonia.Win32` are removed rather than retained as conformance or fallback products. Historical evidence is preserved, while reusable golden/trace contracts move to host-neutral or Avalonia-host tests. The backend-neutral Skia framebuffer slice remains internal for raster/headless verification; it is not a platform shell. A new vendor-source exception requires evidence that the pinned official host API cannot satisfy the capability and a separate ADR/review, and may not recreate a dedicated Win32 host.

## Consequences

- `DOTARCH008` rejects every official Avalonia binary assembly reference.
- The existing public API scan includes the host assembly and rejects Avalonia, Skia, vendor and native types.
- Release package inspection requires every current Doroti and generated package to retain zero official Avalonia binary dependencies; the old host dependency matrix is historical evidence only.
- Architecture tests reject reintroduction of the removed Win32 backend/vendor project or package references.
- Avalonia upgrades must update the central pin, host lock file, dependency matrix, license record and architecture tests together.
