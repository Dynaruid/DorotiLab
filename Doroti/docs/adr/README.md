# Architecture decision records

- [ADR-021: Platform runner workspaces](ADR-021-platform-runner-workspaces.md)
- [ADR-022: Default native platform bridge](ADR-022-default-native-platform-bridge.md)
- [ADR-022: Linux Qt QOpenGLWidget spike](ADR-022-linux-qt-fbo-spike.md)
- [ADR-023: Linux Qt QOpenGLWindow surface](ADR-023-linux-qt-qopenglwindow.md)
- [ADR-024: AppKit-owned Metal surface and permanent dual macOS backend](ADR-024-appkit-metal-surface-spike.md)
- [ADR-025: Windows App SDK HwndExactCpp and managed ANGLE presentation](ADR-025-windowsappsdk-hwndexact-angle.md)
- [ADR-026: Opt-in Windows experimental Acrylic Composition Swapchain](ADR-026-windows-experimental-acrylic.md)

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
| [ADR-009](ADR-009-flutter-baseline-and-compat.md) | Flutter baseline and compatibility boundary |
| [ADR-010](ADR-010-package-conversion-and-distribution.md) | Package conversion and distribution promotion |
| [ADR-011](ADR-011-render-phase-and-fault-taxonomy.md) | Render phase and fault taxonomy |
| [ADR-012](ADR-012-render-object-ownership.md) | RenderObject ownership and boundaries |
| [ADR-013](ADR-013-immutable-paragraph-snapshot.md) | Immutable paragraph snapshot |
| [ADR-014](ADR-014-element-inactive-key-lifecycle.md) | Element inactive tree and Key lifecycle boundary |
| [ADR-015](ADR-015-input-route-gesture-lifecycle.md) | Input route snapshot, gesture arena and cancellation boundary |
| [ADR-019](ADR-019-product-framework-source-ownership.md) | Product-owned framework source and ordinary development workflow |
| [ADR-020](ADR-020-web-typescript-bootstrap.md) | TypeScript-owned Web bootstrap, loader, and browser interop |
| [ADR-021](ADR-021-platform-runner-workspaces.md) | Platform-neutral app and seven fixed-target runner aliases, including separate AppKit and Catalyst products |
| [ADR-022](ADR-022-default-native-platform-bridge.md) | Default Android/iOS/Mac Catalyst native library and binding graph |
| [ADR-022 Linux](ADR-022-linux-qt-fbo-spike.md) | Failed QOpenGLWidget full-scene spike retained as superseded evidence |
| [ADR-023](ADR-023-linux-qt-qopenglwindow.md) | QOpenGLWindow Linux Qt GPU surface and QPA-specific GL resolution |
| [ADR-024](ADR-024-appkit-metal-surface-spike.md) | AppKit-owned MTKView, Metal completion ACK, exact preview dependency, and permanent dual macOS backend boundary |
| [ADR-025](ADR-025-windowsappsdk-hwndexact-angle.md) | Default Windows App SDK 2.4 child-HWND host, managed ANGLE/EGL-D3D11 presentation, bounded resize, and evidence boundary |
| [ADR-026](ADR-026-windows-experimental-acrylic.md) | Opt-in ContentIsland Acrylic, same-device three-slot Composition Swapchain, bounded active-edge resize, deterministic pre-show fallback, and evidence boundary |
