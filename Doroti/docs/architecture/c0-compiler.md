# Compiler C0 contract

## Identity and workspace

C0 consumes `migration/selections/c0.json`. The selection pins the converter and Migration IR versions, generation mode, compatibility profile, Flutter baseline, analyzer project, Dart package root, selected inputs and symbols. The compiler identity records the Dart SDK and analyzer versions, exact Flutter revision, IR schema/version, lowering rules, emitter and FlutterCompat version.

`Converter.ComputeWorkspaceId` hashes the selection, every compiler source under `Doroti.DartToCSharp/src`, the analyzer entrypoint/lock, Flutter baseline, package manifests and selected Dart inputs. `ConvertToWorkspace` writes only to `<workspace-root>/<workspace-id>`. Analyzer results may be cached by a separate content key, but cache hits cannot affect emitted bytes.

## Package and semantic input

The package must already have a resolved `dart pub get` state. C0 reads `dart pub deps --json` and stores a path-free, sorted `doroti.package-graph/v1` graph. The pinned analyzer resolves each compilation unit rather than only parsing it. Migration IR stores stable package library URIs, resolved element kind/type/signature, members and ordered statement spans. Checkout-absolute analyzer URIs never enter the artifacts.

## Lowering and output

C0 supports the fixture's class, immutable field, initializing constructor, expression/block method, local declaration, conditional and return statements. Unsupported declarations or semantics produce `DOTCONVxxx` diagnostics containing severity, package, library, source span, symbol, cause, support state and manual action. They are never replaced by a successful default, null or empty method.

The analyzer model is first normalized into versioned Migration IR. The lowering boundary consumes those IR declarations and source spans, and only the resulting lowered declarations enter the C# backend. Compiler profile policy selects C0-C5 lowering and packaging features without positional boolean combinations.

Each workspace contains deterministic generated C#, `doroti.converter-report/v2`, `doroti.migration-ir/v2`, `doroti.source-map/v1` and a standalone net10.0 project. Generation occurs in a sibling staging directory and is published as one compiler-owned workspace so stale files cannot survive a successful rebuild. The tooling test compares clean/cache-off, incremental/cache-on and cache-hit files byte-for-byte, validates package/element/statement IR and builds that standalone project.
