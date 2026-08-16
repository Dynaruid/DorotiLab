# Doroti product-first development

## Current contract

- `Doroti/src/Doroti.Framework.*` is maintained product source. Its public namespace is `Doroti.Framework.*`, matching the package and assembly names.
- Feature and correctness work starts in the owning framework/runtime/host project and changes the smallest shared contract that fixes the behavior.
- The Dart-to-C# compiler and pinned Flutter checkout are optional migration/reference tools. They do not overwrite product source and are not prerequisites for ordinary product development.
- `DorotiDemoApp` and `doroti-app` template applications are C#-only. Active build and validation commands must not create or consume a Dart package under either application tree.
- Reference comparison remains useful for Flutter behavior fidelity, but a conversion report is not product proof. Build, native live, browser live, physical, and cross-target evidence stay separate.
- Generated artifacts are confined to tool workspaces and `migration/`; no compiler-owned `.g.cs` file is compiled from `Doroti/src/Doroti.Framework.*`.

## Active work

### P1. Native desktop capability closure

- Complete Windows hover/wheel/capture, keyboard, Korean IME, cursor, clipboard, and UIA through the MAUI native handler path.
- Complete the equivalent Mac Catalyst input, text, clipboard, and UIAccessibility path on Apple Silicon macOS.
- Verify resize, density/display change, suspend/resume, and GPU surface/context recreation separately on each native target.

### P2. Web live parity

- Automate browser attach, framework mount/layout/paint, WebGL2 presentation, pointer/wheel/keyboard/composition/clipboard, ARIA actions, resize/DPR, reload, and resource/plugin scenarios.
- Keep the existing package/build/publish proof separate from live browser and physical screen-reader proof.

### P3. Release acceptance

- Run one representative release scenario per target instead of component-by-component matrices.
- Record stability, frame/resource balance, package/static identity, and target-specific limitations in one release summary.
- Keep Windows, Mac Catalyst, Web, physical, and cross-target results independent; unrun gates remain `notVerified`.

## Validation

The active entry points are intentionally small:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
pwsh -File ./Doroti/eng/doroti.ps1 validate
pwsh -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Release
```

- `Developer` validates source ownership, the Release product graph, and the Windows/Mac Catalyst/Web application target graph and builds.
- `Release` adds the Windows GPU presentation run and the Web external template/package publish scenario.
- `migration-audit` is explicit and separate because compiler/provenance regeneration is no longer part of the normal feature loop.

Detailed historical G4-G7 commands and evidence remain under `history/`; they are not active validation instructions.
