# Compiler C1 RenderObject fixture

C1 keeps C0's analyzer 9.0.0, Migration IR v2, source maps, content-addressed workspaces and deterministic emitter, and advances the lowering identity to `c1.0` only for the `flutter-aware-c1` profile.

The selected fixture proves required/optional named constructor parameters and defaults, generic declarations, inheritance, collection literals, `foreach`/`if` control flow and stable analyzer elements/spans. Known RenderObject calls bind to public `Doroti.FlutterCompat.BoxConstraints` and `RenderFixture`; generated code does not reference Engine, backend, vendor or native types.

The generated project remains outside the product solution and requires an explicit `DorotiRepositoryRoot` SDK input at build time. Tests generate twice and compare bytes, build the unmodified project, load it in an isolated context and execute layout/paint/hit behavior. Syntax, API binding and behavior are reported separately in `migration/compiler-support.json`.
