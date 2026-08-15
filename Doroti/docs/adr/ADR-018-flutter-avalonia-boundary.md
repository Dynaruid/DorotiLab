# ADR-018: Flutter framework / Avalonia source-port boundary

- Status: Accepted for G4-0
- Date: 2026-08-07
- Flutter revision: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- Avalonia revision: `f159423f691946e713f454447a780d4677d8a0d2`

Current-status note: the platform-verification statements below describe the G4-0 decision date. G7-3M later added independent `osx-arm64` source, build, live NSWindow, and package-only evidence without transferring Windows results; Linux remains unverified.

## Context

Doroti previously grouped many Flutter external dependencies under `runtime-binding` and retained handwritten Flutter-like behavior in `Doroti.Core`, `Doroti.Platform`, `Doroti.Rendering`, `Doroti.Widgets`, `Doroti.Engine`, and `Doroti.Flutter.Runtime`. That grouping did not distinguish Dart language semantics, managed `dart:ui` API, Flutter framework behavior, concrete native platform behavior, or narrow adaptation. It could therefore report apparent coverage without identifying an executable owner.

The product already has a Windows shell and strict Skia/OpenGL surface adapted from pinned Avalonia source. It does not yet have the managed `dart:ui` contract or the composition adapter that lets reviewed Flutter Scheduler, Services, Rendering, and Widgets use those capabilities.

## Decision

Flutter `packages/flutter/lib` is the only owner of framework API and behavior. Avalonia source-port projects are the only owner of native windowing, dispatcher, input, IME, clipboard, cursor, automation, surface, and platform-service behavior. A managed `dart:ui` contract and a concrete composition adapter will connect them without importing Flutter Engine/embedder source and without exposing Avalonia, Skia, or native handles through Flutter package API.

Every Flutter boundary occurrence has exactly one of these dispositions:

- `flutter-framework`
- `dart-runtime`
- `dart-ui-contract`
- `avalonia-binding`
- `doroti-glue`
- `tooling-only`
- `excluded-with-owner`
- `unsupported-blocker`

`runtime-binding` is not a G4 disposition and contributes no completion credit. An `avalonia-binding` occurrence must name a capability whose map includes Flutter element patterns, `dart:ui` members where applicable, Avalonia upstream symbols/source paths, local targets, operating systems, state, and a validation owner. A planned target is an ownership decision, not implementation evidence.

Flutter Engine source may be consulted read-only for API shape or ordering. It cannot satisfy a product binding or turn an unsupported item into success. In particular, the `flutter/platform_views` embedder channel remains an explicit blocker rather than causing native embedder source or a no-op stub to enter the product.

## Machine-readable records

- `migration/flutter-avalonia/source-boundary.json` is generated from all 13 public roots and all 695 Dart files. It records external URI symbols, dependency paths from public roots, conditional branches, VM pragmas, native/external declarations, and platform channels. Wildcard `dart:ui` imports are narrowed with the pinned `flutter-api.json` symbol inventory.
- `migration/flutter-avalonia/capability-map.json` connects every `avalonia-binding` capability to pinned Avalonia upstream and local target ownership. At G4-0, Windows was the only verified platform and Linux/macOS remained `not-verified`; the later G7-3M macOS promotion is tracked by its own provenance and evidence rather than rewriting this historical manifest.
- `migration/flutter-avalonia/current-owner-audit.json` classifies every declared C# type/delegate in the six pre-cutover owner projects as `keep-bridge`, `move-to-framework`, `move-to-ui-contract`, `replace-by-avalonia`, or `remove-after-cutover`.
- `migration/avalonia-shell/source-port-boundaries.json` v2 is the project dependency policy.

The boundary audit regenerates the current-owner inventory, rejects stale or unclassified entries, rejects broad `runtime-binding`, checks the pinned Flutter census hash, requires every Avalonia binding to resolve to a complete capability record, and verifies that the project policy declares an executable forbidden-edge fixture.

## Dependency direction

The target dependency direction is:

```text
Doroti.Flutter.Framework.* -> Doroti.Flutter.Runtime + Doroti.Flutter.Ui
Doroti.Flutter.Hosting     -> Doroti.Flutter.Framework.* + Doroti.Flutter.Ui
Doroti.Host.Desktop.Flutter -> Flutter.Hosting + Host.Desktop + neutral contracts
Doroti.Host.Desktop        -> neutral contracts + Doroti.Vendor.Avalonia.*
Doroti.Vendor.Avalonia.*   -> Doroti.Shell.Core + approved vendor peers
```

`DOTARCH009` rejects host/platform references from new Flutter Runtime, Ui, Framework, and Hosting assemblies. The negative project at `validation/architecture/forbidden-flutter-host` proves through an actual `dotnet build` that `Doroti.Flutter.Framework.* -> Doroti.Platform` fails.

## Recorded transitions

Two existing edges are debt, not accepted final architecture:

- `Doroti.Flutter.Runtime -> Doroti.Platform` expires at G4-1 when clipboard/channel behavior moves to Flutter Services, `dart:ui`, and the host binding.
- `Doroti.Engine -> Doroti.Widgets` expires at G4-6 when the generated Widgets composition root replaces `InteractiveApplication`.

Only the exact current `Doroti.Flutter.Runtime` assembly receives the direct transition allowance. Existing Foundation/Physics assemblies receive its unavoidable transitive `Core`/`Graphics`/`Platform` compiler metadata closure, but no direct project-reference permission; a new runtime/framework assembly cannot copy either allowance. G4-1 removes the direct edge and these transitive references together. Both transitions remain visible in the project manifest and current-owner audit.

## Consequences

Foundation batch 2 and all later promotion reviews must use the new dispositions and capability IDs. Handwritten behavior can remain temporarily only with a symbol-level cutover destination and milestone. Neutral DTO marshalling, immutable frame/resource protocols, lifetime handoff, and coordinate conversion may remain as bridge code; gesture policy, layout, semantics construction, native message handling, IME state machines, and GPU context policy may not acquire a new Doroti owner.

The manifests state ownership and validation responsibility but do not claim that G4-1 or later bridge implementations exist. `source-ported-awaiting-*`, `planned-*`, and `not-verified` statuses are deliberately non-success states.

## Validation

Run:

```bash
Doroti/eng/validate-g4-boundary.sh
```

The gate reproduces the full Flutter boundary manifest byte-for-byte, validates all three manifests, and requires the forbidden dependency fixture to fail with `DOTARCH009`.
