# ADR-019: Product framework source ownership

## Status

Accepted on 2026-08-16.

## Decision

The C# files under `src/Doroti.Framework.*` are maintained product source. Their public namespaces are `Doroti.Framework.*`, matching their project, assembly, and package identities.

New framework work is implemented directly in the owning product project. Cross-cutting behavior is fixed at the lowest shared framework, runtime, rendering, or host contract and is validated through representative product scenarios. Product changes do not require a full Dart-to-C# regeneration pass.

The Dart-to-C# compiler remains an optional import and reference-differential tool. It may emit candidates only into isolated tool workspaces; it must not overwrite product source. A candidate can enter `src/` only through an explicit review that establishes ownership, API shape, and product validation.

## Consequences

- `Doroti.Generated.*` is not a public or product namespace.
- Compiler-owned `.g.cs` files are not compiled from `src/Doroti.Framework.*`.
- The pinned Flutter checkout is optional for ordinary build and feature development, but remains required when a task explicitly performs source comparison or import.
- Historical source selections and evidence are preserved only in the repository history archive; they do not make the current product source generated or immutable.
- Validation remains capability-based, but the former repository-wide script entry points have been removed. Source ownership, product build, application targets, native live, browser live, and physical acceptance must be recorded independently.
- Native live, browser live, physical, and cross-target claims remain independent; an unrun gate is `notVerified`.

## Change workflow

1. Find the product owner in `Doroti.Framework.*`, `Doroti.Runtime`, rendering, or the target host.
2. Change the shared contract and all affected consumers directly.
3. Add focused tests or assertions near that contract.
4. Run `eng/doroti.ps1 build` and the focused project tests or assertions owned by the changed contract.
5. Before a release claim, run and record the applicable target build, package, native-live, browser-live, physical, and accessibility checks independently.
6. Use the optional Dart-to-C# compiler only for an explicit reference or import task; ordinary product validation does not depend on compiler output.
