# Dart modulo semantics

Dart `%` uses Euclidean modulo; C# `%` uses the sign of the dividend.
See the [Dart num operator contract](https://api.dart.dev/dart-core/num/operator_modulo.html).
`DartNumeric.Modulo` centralizes the behavior for int/long/double and boxed Dart numbers.
It handles negative divisors, `long.MinValue % -1`, positive zero, NaN and infinities.
Floating-point rounding follows the Dart SDK, including a rounded result equal to the divisor.

## Product audit

Run from the repository root:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/modulo-semantics/ModuloAudit.csproj -c Release -- verify . Doroti/artifacts/modulo-semantics/after.json
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/modulo-semantics/contracts/ModuloContracts.csproj -c Release
```

The Roslyn audit loads Material/Cupertino and their dependencies. It distinguishes numeric
operators from user-defined operators by symbol/type, ignoring comments, strings and operator
declarations. `scan` writes an inventory; `apply` rewrites only numeric `%` expressions in
`Doroti.Framework.*` and the `Doroti.Ui` geometry operator bodies. `verify` fails if a numeric
operator remains, a framework source is not semantically loaded, or a new `%=` needs lowering.
Do not regenerate the product framework from Dart to perform this migration.

The September 27 audit migrated 129 numeric operators in 33 files. Existing hand-written
double-modulo workarounds were reduced to one call, including the runtime palette hue normalizer.
The final inventory contains **zero raw Dart numeric modulo expressions**, 16 user-defined
operator calls, and 27 native C# remainder expressions. The retained native expressions are
the helper implementation itself, unsigned digit/random extraction, bounded color/counter/ring
indices, and divisibility checks in host/rendering code. They do not need Dart lowering.

Product contracts cover negative coordinates, insets, Radius operator dispatch, constraints,
backwards looping-list indices and negative hue extrapolation. The existing
`Doroti/validation/cupertino-sample` also exercises spinner animation and pointer-driven tabs.

## Compiler regression

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project tools/Doroti.DartToCSharp/validation/modulo-semantics/Compiler.Modulo.Validation.csproj -c Release -- .
```

The fixture goes through the Dart analyzer, C# generator, Roslyn compilation and execution.
An installed Dart SDK supplies the floating-point and side-effect oracle. On Windows the test
finds `flutter/bin/cache/dart-sdk/bin/dart.exe` from PATH; alternatively pass the Dart executable
as a second argument after the repository path.

Coverage includes integer limits, zero divisors, both signs, mixed numeric types, boxed/dynamic
numbers, constants, nested expressions, custom operators, `%=` results, receiver/index/getter/
RHS/setter evaluation order, static members, null-aware assignment, cascade property/index
assignments and awaited RHS expressions. Generated output stays under `.doroti/compiler-modulo-semantics`.
`num` retains the compiler's existing double representation; this change does not redesign that
type mapping or Dart's separate `remainder` method.

Validation establishes arithmetic, generated-code and framework behavior. It does not establish
native device input, displayed pixels on every backend, or cross-platform performance.
