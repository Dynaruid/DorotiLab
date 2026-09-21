# Compiler null-semantics validation

Run from the repository root:

```powershell
./tools/Doroti.DartToCSharp/validation/null-semantics/validate.ps1
```

The 20-minute-bounded validation invokes the pinned Dart analyzer and the real
converter, stores generated candidates under `.doroti/compiler-null-semantics`,
compiles those candidates with Roslyn, and executes nullable value, reference,
generic, zero/false, default-argument, and null-failure contracts. It also rejects
both the removed runtime API name and the converter's internal placeholder in the
final generated C#.
