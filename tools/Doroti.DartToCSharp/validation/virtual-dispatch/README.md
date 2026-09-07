# Compiler virtual dispatch validation

From the repository root, with PowerShell 7, .NET 10 and the pinned Dart analyzer available:

```powershell
./tools/Doroti.DartToCSharp/validation/virtual-dispatch/validate.ps1
# Also inspect eight target method slots generated from pinned Flutter sources:
./tools/Doroti.DartToCSharp/validation/virtual-dispatch/validate.ps1 -Upstream
```

The wrapper enforces a 20-minute timeout. It generates candidates only in `.doroti/compiler-dispatch-*`; it does not adopt or copy generated files into `Doroti/src` or `DorotiTestbedApp`.

The test invokes the real Dart analyzer and compiler, compiles the emitted C# with Roslyn, and invokes base/interface members. It covers ordinary and abstract overrides, materialized mixins, getter/field dispatch, covariant parameter checks in expression bodies, renamed positional arguments, reordered named defaults, optional-parameter bridges to referenced bases, nullable/generic signatures, multi-level generic substitutions, getter/setter slot sharing, object diagnostics, the Ui Color generic context entry point, and separate Dart library-private state. The referenced-base fixture models a fixed CLR API independently of this generation run.

A second selection intentionally omits an annotated override's base contract and must produce `DOTCONV902`. Serial and parallel runs must produce identical generated C# and source maps. `fixtures/upstream-selection.json` is a partial Flutter selection: its eight target overrides are checked, but missing-dependency/unsupported diagnostics are retained, and no aggregate build pass is inferred.

This compiler uses selected Dart contract families to determine emitted signatures. Include base libraries in the semantic graph; use `graph-only` for referenced bases. Runtime ports may add CLR contracts not present in Dart (such as `Color.resolveFrom<TContext>`), which require an explicit bridge. Passing these fixtures is not full Flutter import qualification.
