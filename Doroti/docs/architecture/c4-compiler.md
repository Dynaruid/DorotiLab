# C4 async, mixin, extension, navigation and platform-port compiler slice

C4 pins the `flutter-aware-c4` compatibility profile and `c4.0` lowering rules. The selected fixture under `migration/fixtures/c4_async_navigation` generates an unpatched .NET project whose tap callback executes an awaitable Doroti `Future<T>`, a method-only mixin, an expression-bodied extension and a typed `NavigatorState.push/pop` result. The tooling gate runs the same state and route-order scenario against hand-written and generated FlutterCompat apps.

`Future<T>` remains a FlutterCompat type rather than a `Task<T>` alias. Explicit adapters expose `fromTask`/`asTask`; single-subscription and broadcast streams retain listener and cancellation order; FutureBuilder and StreamBuilder reject stale completions after disposal. The C4 fixture is deliberately a narrow async/navigation vertical slice and does not claim that the other R9A/R9B app capabilities are complete.

MethodChannel selection emits a public consumer-supplied platform-port interface and `DOTCONV440` warning. The generated adapter cannot silently succeed without an implementation. Factory/named constructor, `dynamic`, isolate, Zone and FFI meanings outside the selected lowering scope remain source-span diagnostics (`DOTCONV441`/`DOTCONV442`) rather than stubs. The exact syntax/API/behavior status is recorded in `migration/compiler-support.json`.

Clean/cache-off, incremental/cache-on and cache-hit output sets must be byte-identical. The isolated generated app and platform-port projects must build without source patches, Dart VM references, compiler internals, backend internals or vendor types.
