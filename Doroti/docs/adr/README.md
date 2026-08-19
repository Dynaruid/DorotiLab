# Architecture decision records

- [ADR-021: Platform runner workspaces](ADR-021-platform-runner-workspaces.md)
- [ADR-022: Default native platform bridge](ADR-022-default-native-platform-bridge.md)

R3–R8 accept ADR-001 through ADR-015 as the contract baseline for subsequent implementation. A later decision may supersede an ADR, but must not silently edit its ownership or lifecycle rules.

| ADR | Decision |
|---|---|
| [ADR-001](ADR-001-reference-sources-and-provenance.md) | Reference-source roles and provenance |
| [ADR-002](ADR-002-ui-raster-thread-model.md) | UI/raster thread and commit contract |
| [ADR-003](ADR-003-immutable-display-list-layer-tree.md) | Immutable DisplayList and LayerTree |
| [ADR-004](ADR-004-surface-generation-device-loss.md) | Surface generation and device loss |
| [ADR-005](ADR-005-resource-lease-and-ack.md) | Resource retain/release and ACK |
| [ADR-006](ADR-006-public-api-compatibility.md) | Public API and v0.x compatibility |
| [ADR-007](ADR-007-generated-source-promotion.md) | Converter and generated-source promotion |
| [ADR-008](ADR-008-avalonia-platform-slice.md) | Avalonia platform-engine vendor slice |
| [ADR-009](ADR-009-flutter-baseline-and-compat.md) | Flutter baseline and compatibility boundary |
| [ADR-010](ADR-010-package-conversion-and-distribution.md) | Package conversion and distribution promotion |
| [ADR-011](ADR-011-render-phase-and-fault-taxonomy.md) | Render phase and fault taxonomy |
| [ADR-012](ADR-012-render-object-ownership.md) | RenderObject ownership and boundaries |
| [ADR-013](ADR-013-immutable-paragraph-snapshot.md) | Immutable paragraph snapshot |
| [ADR-014](ADR-014-element-inactive-key-lifecycle.md) | Element inactive tree and Key lifecycle boundary |
| [ADR-015](ADR-015-input-route-gesture-lifecycle.md) | Input route snapshot, gesture arena and cancellation boundary |
| [ADR-016](ADR-016-avalonia-host-first.md) | Official Avalonia host boundary and dependency matrix |
| [ADR-017](ADR-017-source-ported-desktop-shell.md) | Source-ported Avalonia desktop shell product cutover |
| [ADR-018](ADR-018-flutter-avalonia-boundary.md) | Flutter framework and Avalonia source-port ownership boundary |
| [ADR-019](ADR-019-product-framework-source-ownership.md) | Product-owned framework source and ordinary development workflow |
| [ADR-020](ADR-020-web-typescript-bootstrap.md) | TypeScript-owned Web bootstrap, loader, and browser interop |
| [ADR-021](ADR-021-platform-runner-workspaces.md) | Platform-neutral app and six fixed-target runner workspaces |
| [ADR-022](ADR-022-default-native-platform-bridge.md) | Default Android/iOS/Mac Catalyst native library and binding graph |
