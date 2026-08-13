# G3-T0 Flutter validation bootstrap

G3-T0 removes the handwritten Flutter compatibility assembly instead of preserving it as an alias, bridge, or legacy project. The active product and compiler graph now use `Doroti.Flutter.Runtime` only for host-neutral Dart async primitives and engine/platform ports. Flutter Widget, Material, Cupertino, navigation, layout, and lifecycle behavior was not moved into Runtime.

The historical C0-C5 compatibility profiles and `Converter` facade are no longer compiler entrypoints. `DartCompiler` accepts only `framework-semantic` / `flutter-framework-f0` until G3-1 replaces the single-library selection with the resolved multi-library graph.

## Bootstrap validation

`Doroti/validation/flutter-source.lock.json` pins Flutter revision `56b8e1a851a594b1a154f8ea93270807dab22b9a` to a canonical SHA-256 over every normalized Dart path and file hash under `packages/flutter/lib`.

The compiler validation checks all of the following in one run:

- exactly 13 root public libraries and their exact per-root export counts;
- 640 root export directives, 695 Dart files, and 682 files under `src`;
- the full census SHA-256, so equal counts with different content do not pass;
- absence of the removed source directory, namespace, project, and references from active source/build/compiler files;
- a real analyzer/compiler negative selection that must fail with typed `DOTF0001`, a non-empty source span, symbol, and required action.

Run the gate with the repository-required 15-minute timeout:

```powershell
./eng/doroti.ps1 validate -ValidationSuite compiler
```

The run writes `artifacts/validation/g3-t0-bootstrap.json`. `./eng/doroti.ps1 audit` also executes this validation after the source and vendor provenance audits.

## Completion evidence

On 2026-08-04, the following gates passed:

- `dotnet build Doroti.Product.slnx --nologo`: 0 warnings, 0 errors;
- `dotnet build Doroti.slnx --nologo`: 0 warnings, 0 errors;
- `./eng/doroti.ps1 validate -ValidationSuite compiler`: PASS, 13 roots, 640 exports, 695 Dart files, `DOTF0001` negative diagnostic;
- `./eng/doroti.ps1 audit`: PASS, source/vendor findings 0 and G3-T0 validation PASS.

This milestone does not claim framework generation, API parity, target runtime, or package completion. Those remain owned by the subsequent Goal3 milestones.
