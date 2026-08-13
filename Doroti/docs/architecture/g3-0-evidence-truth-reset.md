# G3-0 evidence truth reset

G3-0 separates resolved Flutter source inventory from implemented framework code. The committed F1-F4 closure JSON remains useful for library, declaration, member, dependency, and analyzer census only. Its broad `generated`, `manual-adaptation`, `runtime-binding`, and owner annotations are not implementation evidence.

Each `doroti.flutter-framework-evidence/v2` artifact has independent `resolvedInventory`, `mechanicalGenerated`, `reviewedGeneratedCs`, `reviewedSourcePortCs`, `runtimeBound`, `compiled`, and `behaviorVerified` states. An implementation state count is valid only when it has the same number of unique symbol-to-target rows. Every row identifies one canonical Dart element, one source library, one concrete hashed target, and one registered compiler profile. A marker in a shared handwritten file cannot satisfy multiple Dart symbols.

Only `reviewed-generated-cs`, `reviewed-source-port-cs`, and `runtime-binding` count toward completed implementation. A `.g.cs` file is always `mechanical-generated`, even when it compiles. Compile and behavior results never promote its disposition automatically.

G3-0 initially registered only `flutter-framework-f0`; G3-1 then registered the general `flutter-framework` profile for its bounded multi-library compiler selection. G3-2 reviews and promotes that bounded F0/G3-1 set, but it still does not register F1-F4 milestone coverage. F1-F4 therefore retain their G3-0 zero-reviewed baseline until G3-B0 and later milestones feed their selections through the promotion manifest. Their resolved inventories contain the inherited F0 mechanical candidate, but all symbols remain blockers in those milestone-specific evidence files:

| Milestone | Resolved declarations | Resolved members | Missing mechanical symbols | Completion blockers |
|---|---:|---:|---:|---:|
| F1 | 742 | 4,262 | 5,003 | 5,004 |
| F2 | 3,907 | 27,803 | 31,709 | 31,710 |
| F3 | 3,907 | 27,803 | 31,709 | 31,710 |
| F4 | 5,346 | 48,969 | 54,314 | 54,315 |

Run the deterministic reset and the compiler validation from the Doroti directory:

```powershell
dotnet run --project tools/Doroti.SourceTools -- framework-evidence-reset
./eng/doroti.ps1 validate -ValidationSuite compiler
```

Validation regenerates the four evidence files in a temporary root and compares bytes, audits symbol cardinality and target hashes, confirms that no F1-F4 milestone profile is registered, regenerates the F0 and G3-1 candidates from pinned Flutter source, and rejects any imported pre-Goal3 compile/behavior PASS result.
