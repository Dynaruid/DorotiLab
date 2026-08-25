# P3 Flutter framework slice pilot

P3 selects the four-declaration `Key -> LocalKey -> UniqueKey/ValueKey` closure from Flutter `packages/flutter/lib/src/foundation/key.dart`. The upstream revision, BSD license, source and license hashes, symbol dependency edges, exclusions, compiler inputs, manual source, adopted product source and behavior artifacts are pinned by `migration/flutter-compat/p3-foundation-key-slice.json` and checked by the normal source audit.

## Frontend boundary

The pinned Flutter source workspace currently requires a newer development Dart dependency graph than the installed stable SDK can resolve. The compiler therefore reads this external source through the explicit selection-level `analysisMode: syntax-only` mode. This mode parses the original file in place; it does not copy, edit or silently stub the source. It preserves declaration spans and import directives but intentionally does not claim resolved element closure.

The pilot closure records the resulting limits:

- `package:meta/meta.dart` supplies annotations only and is excluded from runtime behavior.
- `diagnostics.dart#shortHash` affects `UniqueKey.toString`, which is outside the P3 equality/identity fixture and remains an adoption review concern.
- selected inheritance dependencies are closed within `Key`, `LocalKey`, `UniqueKey` and `ValueKey`.

Future framework slices that need resolved elements must use a compatible pinned Dart/Flutter package graph rather than broadening `syntax-only` into an implicit success path.

## Ownership dispositions

| Symbol | P3 disposition | Evidence |
|---|---|---|
| `Key` | compiler output supported | generated base builds without diagnostics |
| `LocalKey` | compiler output supported | generated base preserves the selected inheritance edge |
| `UniqueKey` | runtime adoption | review-only adoption candidate and `adopted-product` provenance; no product write |
| `ValueKey<T>` | manual replacement | whole-symbol 1:1 suppression pinned to the generated-file SHA-256 |

The generated base remains buildable before customization. The effective port replaces only `ValueKey<T>` and retains compiler ownership for the other selected symbols. The existing `Doroti.FlutterCompat` implementation is the adopted product comparison target; P3 also corrects its equality contract so private `ValueKey` subclasses cannot collide with the public type.

## Behavior and audit gate

`p3-foundation-key.json` covers equal/different values, generic type isolation, private subclass isolation, equal hashes and same/distinct `UniqueKey` identity. The tooling regression executes the same fixture against:

1. the immutable generated base,
2. the manual effective project,
3. the adopted `Doroti.FlutterCompat` product implementation.

All three results must equal the committed Flutter reference. The same regression verifies the original source hash, selected imports, symbol closure, ownership partition, generated-base hash, non-mutating adoption bundle and provenance origin. The historical audit separately rejected source, license, selection, port, manual, adopted-product or fixture hash drift.
