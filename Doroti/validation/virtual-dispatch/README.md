# Framework virtual dispatch validation

Run from the repository root with PowerShell 7 and .NET 10:

```powershell
./Doroti/eng/validate-framework-virtual-dispatch.ps1
```

The script builds the current Release framework dependencies, audits all 13 `Doroti.Framework.*` projects with Roslyn, and runs the FCR-7 base/interface dispatch behavior contracts. Each process has a 20-minute timeout. The report is written to `.doroti/framework-virtual-dispatch.json`; errors produce a nonzero exit code.

The audit checks hidden virtual/abstract methods, properties, indexers and events, including declarations missing both `virtual` and `override`. It also checks virtual/abstract method overloads whose parameter lists differ from a base slot. Typed overloads must have a real override bridge in the declaring class. Dart unary/binary subtraction operations are separate operations and are recognized by exact type and arity.

`intentional-hiding.json` records exact member/base pairs for distinct Dart library-private identities. These are independent state or helpers, even when their C# names coincide. Inaccessible members from another assembly do not require the C# `new` keyword. New unlisted findings, undocumented/stale exceptions, and semantic compilation errors fail the audit.

The semantic projection uses the current framework source and freshly built Release references. It is scoped to the framework, whose projects do not require platform generators. It does not establish compilation or device acceptance for every MAUI, Web, or native platform configuration. It does not regenerate product-owned C# from the optional Dart import tool.

The behavior contracts also run in the normal FCR-7 executable, or separately:

```powershell
# Apply the same 20-minute timeout when invoking tests outside the wrapper.
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --virtual-dispatch
```

See `history/26-09-07/framework-virtual-dispatch-audit.md` for the initial findings, fixes, mutation-test evidence, and verification boundaries.
