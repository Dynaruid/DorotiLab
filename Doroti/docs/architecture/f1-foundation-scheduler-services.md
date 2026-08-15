# F1 foundation, scheduler, and services resolved inventory

The pinned F1 closure contains 120 libraries, 742 declarations, and 4,262 members rooted at `foundation.dart`, `scheduler.dart`, and `services.dart`. In Goal3 this is a resolved analyzer inventory, not implementation coverage.

The old ownership marker audit and broad `manual-adaptation` ratios were removed by G3-0. `migration/flutter-framework/f1-evidence.json` now records one inherited F0 mechanical candidate, zero reviewed/runtime-bound symbols, 5,003 symbols without a mechanical candidate, and 5,004 completion blockers. See [`g3-0-evidence-truth-reset.md`](g3-0-evidence-truth-reset.md) for the new cardinality and migration rules.

Regenerate the source inventory independently when pinned Flutter source changes:

```powershell
Push-Location ../tools/Doroti.DartToCSharp/analyzer
dart run tool/closure/extract_f1_closure.dart ../../../reference/flutter-master/packages/flutter/lib ../../../Doroti/migration/flutter-framework/f1-closure.json
Pop-Location
dotnet run --project tools/Doroti.SourceTools -- framework-evidence-reset
```
