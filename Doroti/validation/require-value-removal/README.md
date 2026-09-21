# RequireValue removal validation

This Roslyn tool resolves calls by symbol before changing them. It classifies the
nullable value, reference, and already non-nullable value overloads, writes a JSON
manifest, and refuses unexpected invocation shapes. `apply` performs the edit;
`scan` is a dry run; and `verify` fails unless executable framework consumers are
zero.

```powershell
dotnet run --project Doroti/validation/require-value-removal/RequireValueRemoval.csproj -- scan Doroti/Doroti.Editor.slnx Doroti/artifacts/require-value-removal/current/manifest.json
dotnet run --project Doroti/validation/require-value-removal/RequireValueRemoval.csproj -- apply Doroti/Doroti.Editor.slnx Doroti/artifacts/require-value-removal/current/manifest.json
dotnet run --project Doroti/validation/require-value-removal/RequireValueRemoval.csproj -- verify Doroti/Doroti.Editor.slnx Doroti/artifacts/require-value-removal/current/final-manifest.json
```

The replacement evaluates the operand exactly once. Nullable value and reference
operands become `operand ?? throw new NullReferenceException(...)`; operands bound
to the non-nullable struct overload are unwrapped without adding a runtime check.
Run CSharpier after `apply`, then build both Debug and Release configurations.

## Compatibility note

`DartRuntimePrimitives.RequireValue` was a public runtime API and its three
overloads have been removed. This is a source and binary compatibility break for
external callers. Consumers must replace calls with an explicit C# branch,
pattern, or `?? throw new NullReferenceException("A required value was null.")`
as appropriate, then rebuild against the updated Doroti runtime. No compatibility
shim is retained.
