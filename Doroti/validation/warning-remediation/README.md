# Framework warning remediation

Run from the repository root. Every build, test, and analyzer command must use
`python Doroti/validation/run-with-timeout.py` (20-minute process-tree deadline).

```powershell
python Doroti/validation/run-with-timeout.py python Doroti/validation/warning-remediation/baseline.py
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/warning-remediation/Contracts -c Debug
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/warning-remediation/Contracts -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/warning-remediation/test_guard.py
python Doroti/validation/warning-remediation/guard.py
```

`baseline.py` copies the current product sources (including dirty/untracked source
changes), leaves the original compiler settings intact, removes only framework
warning pragmas in the isolated copy, and captures Debug/Release diagnostics.
Only this isolated diagnostic build overrides warnings-as-errors. The copy has
fresh intermediate/output directories. The baseline records hashes, HEAD, dirty
state, commands, logs, and the existing Material/Cupertino NoWarn exceptions.

Build `SourceTools/SourceTools.csproj`, then invoke its DLL with one of:

- `index <evidence-directory>`: attach enclosing syntax symbols to captured diagnostics.
- `rename-generics <solution-or-project> <report.json>`: rename shadowed method type
  parameters with Roslyn symbols, preserving nested references and virtual slots.
- `simplify-casts <solution-or-project> <report.json>`: remove intermediate object
  casts only when Roslyn classifies the direct conversion as identity/reference.
  Retain the outer cast and therefore the expression's static type. Exclude
  numeric, boxing/unboxing, dynamic and user-defined conversions.
- `simplify-casts ... --module=ProjectName`: restrict the pass to one module,
  including a module whose warning pragmas were already removed. Null-to-reference
  casts become explicitly nullable; null-to-value casts are never transformed.
- `annotate-null-casts <solution-or-project> <report.json> <compiler-log>`:
  annotate reference casts reported as CS8600. Use a log for the exact current
  source; required consumers still fail and need contract review. This is an
  intermediate diagnostic aid, not a completion check.
- `fix-ide0002 <solution-or-project> <report.json>` / `check-ide0002 ...`: invoke
  the SDK Roslyn IDE0002 predicate on member accesses and its document code fix.
  This avoids the combined analyzer's unrelated qualified-type-name pass. It
  requires the pinned SDK's internal analyzer API and fails on incompatible SDKs.
  Use the check command after changes; builds alone do not validate IDE0002.

Use a solution containing only the projects being validated. Do not include
generated/reference/history sources or unsupported platform projects implicitly.
The tools do not regenerate or adopt the framework from Dart.

The strict warning guard is an exit gate: all framework pragmas must be gone.
During work, `--baseline <baseline.json>` only checks that existing suppressions
were not expanded; success in that mode is **not** completion. Native host API
compatibility suppressions and Runtime's retained quantizer nullable setting are
outside the framework guard's scope. The Material/Cupertino CS0219/CS8524/CS8846
project exceptions remain explicitly allowed by work-a1.md.

## Contract changes under validation

The contract runner covers timeout completion/error provenance, ticker completion
and cancellation, nullable messaging/codecs, identity and value hashing, mapped
widget-state factories, generic state factory assignability, restoration payloads,
and default/null theme values. State-map adapters resolve lazily and reject reads
of unresolved style/geometry. `WidgetStateMapper<T>` cannot distinguish nullable
and required reference `T` after CLR type erasure; the concrete required-value
factories explicitly reject unmatched states. This pre-existing generic limitation
is not claimed to be solved for all Dart types.

The generator fixture validates the actual factory bridge and lowercase identifier
patterns changed in this task. It does not remove the generator's legacy blanket
warning pragma or qualify regeneration of every framework source. The production
framework guard rejects adopting such suppression into reviewed product sources.

## Full automated verification

`validated-projects.json` records the IDE0002 scope (27 common, supported host,
compiler/tooling and testbed projects). Generated files and unsupported platform
projects are excluded. Source hashes ensure that validation is not reported against
sources edited during the run.

```powershell
python Doroti/validation/warning-remediation/verify.py --run <evidence-directory> --windows
```

This runs the strict guard, its tests, IDE0002 check, TestbedApp/WidgetPreviews and
contract builds in Debug/Release, the compiler fixture, Windows product build and
OS-input navigation/scroll/focus test, and diff checks. Each command is protected
by the 20-minute process-tree timeout. Windows validation opens the test product
and closes only that launched process. Physical human-input and other platforms
remain separate evidence categories.

### Web product gate

Build `DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj` in Release under the timeout
wrapper. Start its dev server with `dotnet run --project
DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release --no-build --no-launch-profile`.
Use the listening URL printed by that server (the SDK may ignore `--urls`).

```powershell
python Doroti/validation/run-with-timeout.py node Doroti/validation/warning-remediation/web-product.mjs <new-evidence-directory> http://127.0.0.1:5000
```

The test creates its own headless Chrome profile and isolation proxy, retaining
the default `worker-direct-webgpu` mode. It checks disabled buttons, navigation,
checkbox/switch changes after scrolling, date-picker cancellation, text entry,
text connection closure and remount. Screenshots, events and renderer state are
saved. It closes its own browser and proxy; stop the separately launched dev
server afterwards. CDP browser input is automated product evidence.

The legacy Win32 HWND focus-equality test does not apply to WinUI TextBox islands.
`verify.py` uses the WinUI focus/text gate with normal typing after the pointer-up
handoff. Physical human input remains a separate qualification.
