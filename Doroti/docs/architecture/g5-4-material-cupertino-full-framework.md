# G5-4 Material, Cupertino, and Widget Previews full framework

G5-4 closes the pinned Flutter public framework graph through the Material, Cupertino, and Widget Previews product partitions. Flutter source remains the owner of adaptive behavior and UI semantics; the host supplies typed OS capabilities only.

## Pinned census and ownership

- The source lock is Flutter `56b8e1a851a594b1a154f8ea93270807dab22b9a`.
- The 13 public roots cover all 695 Dart files under `packages/flutter/lib`. The analyzer-resolved product closure contains 694 libraries because `src/dart_plugin_registrant.dart` is not reachable from those public roots.
- The resolved closure contains 5,355 declarations and 49,007 members. Analyzer errors, unsupported blockers, unclassified declarations/members, and unowned libraries are all zero.
- The disposition is exhaustive: 3,264 declarations are owned by reviewed predecessor framework packages and 2,091 declarations are owned by the G5-4 generated partitions.
- Validation hashes every resolved library against the current source tree, so the declared pin cannot pass with silently drifted source content.

## Generation and review boundary

The generated surface is divided into nine dependency batches:

- Material: `M1-theme`, `M2-shape-ink`, `M3-components-a-m`, and `M4-components-n-z`.
- Cupertino: `C0-theme`, `C1-navigation`, `C2-form-text`, and `C3-dialog-selection`.
- Widget Previews: `P0-widget-previews`.

Together they select 252 product libraries and 2,091 declarations. Each compiler batch requires zero compiler errors, zero unclassified AST nodes, and zero silent omissions. Staging produces 249 generated C# files because three selected libraries contain no emitted declarations.

`review-g5-4-generated.ps1` applies deterministic C# compatibility adaptations to the staged candidate. Its report records every changed file and enforces zero declaration or file removals. The reviewed Material, Cupertino, and Widget Previews projects then build as one solution with zero warnings and zero errors.

## Public API and gallery gates

The public API audit starts from the `material.dart` and `cupertino.dart` export graphs and compares analyzer declarations with compiler-emitted public symbols per source library.

- Material: 181 exported libraries, 521 public declaration occurrences, diff 0.
- Cupertino: 52 exported libraries, 121 public declaration occurrences, diff 0.

`Doroti.Validation.G5Gallery` uses one source-ported shell factory with Material and Cupertino branches. It constructs and executes `MaterialApp` and `CupertinoApp` contracts and compares:

- visual shell title, home widget, and application color;
- button and switch callback behavior;
- editable text callbacks and read-only state;
- shared semantics label and button properties.

The four dimensions must all produce `G5-4-GALLERY-DIFFERENTIAL-PASS`.

## Platform dependency boundary

The reviewed projects and gallery harness are audited for Avalonia Controls, Avalonia themes, Avalonia XAML loading, `x:Class`, and `.xaml` files. Every count must remain zero. G5-4 does not add a parallel Avalonia control tree or theme implementation.

## Reproduction and evidence

Run the complete automated gate from the repository root:

```powershell
& Doroti/eng/validate-g5-4.ps1
```

The aggregate result is `migration/flutter-framework/g5-4-evidence.json`. It links the closure, batch index, Material/Cupertino API manifests, gallery differential result, and reviewed build log.

Physical Windows IME, external accessibility, sustained GPU, and cross-monitor DPI proof are deliberately not used to close G5-4. They remain `notVerified` and are deferred to the G5-8 `DorotiDemoApp` target-machine stage.
