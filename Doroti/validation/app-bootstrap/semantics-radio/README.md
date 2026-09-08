# Native radio semantics regression

Run from the repository root with the required 20-minute timeout:

```powershell
python -c "import subprocess; subprocess.run(['dotnet','run','--project','Doroti/validation/app-bootstrap/semantics-radio'],timeout=1200,check=True)"
```

This uses the production MauiSemanticsBridge and real MAUI RadioButtons with
an inline dispatcher; it does not replace the projection logic with a fake.

On Galaxy SM-S931N, changing SegmentedButton selection from a later item to an
earlier item could immediately revert it. Diagnostic logs showed one pointer
hit followed by two selections: `Day (previous Year)`, then `Year (previous Day)`.
MAUI implicitly grouped all radio projections in the same AbsoluteLayout. While
applying Day's selected state, MAUI unchecked Year; Year's CheckedChanged callback
ran outside its own update guard and emitted another semantics tap.

The regression fails on the original bridge with:
`Projecting selection 1 emitted native actions: (4, tap)`.

The fix isolates native radio groups so the framework owns their selection and
forwards only checked/activation events, not deselection notifications. Checks
cover forward/reverse selection, unrelated groups, genuine native activation,
deselection, removal, and clear/recreate. Debug and Release both pass.

Standalone and sample-widget pointer tests also passed before this fix; they
did not include the native accessibility projection and could not expose this
feedback. The defect is in the MAUI bridge, not in SegmentedButton's selection
algorithm and not inherently limited to touch input.

The rebuilt Android arm64 Release APK (0 warnings/errors) was installed on the
Galaxy. Touch selection `Year -> Day -> Month -> Week -> Year -> Day` passed,
including captured highlight pixels for each step. Injected mouse selection
`Year -> Day` also passed. Local captures and the original duplicate-callback
log are under `.doroti/evidence/segmented-radio-20260909/` at the repository root.
TalkBack interaction and other physical MAUI platforms were not tested here.

## Whole-tree projection boundary

`SemanticsProjectionContracts.cs` extends this regression to Entry, CheckBox,
Switch, Slider, Button, and radio state. It checks cross-element notifications
during an update, native text selection, repeated values, rapid changes back to
the previous value before another framework update, and disabled/read-only/
removed/disposed controls. Real native control changes must still dispatch
actions. These managed tests do not exercise a platform handler or screen reader.

The cross-platform review and separate Web, Windows UIA, and Qt validation are
recorded in [accessibility-state-and-actions.md](../../../docs/architecture/accessibility-state-and-actions.md).
