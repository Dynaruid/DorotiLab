# Same-work WASM experiment

`work.md` section 9 records the research and subsequent user-directed adoption.
HAMT is the only PersistentHashMap representation in every build. The sample uses
only indexed sections. No feature flag is required. Adoption reflects the measured
map allocation reduction, not a newly qualified latency or total-memory result.

The HAMT copies the modified hash path, retains old snapshots, handles full-hash
collisions, and always stores a replacement value even when values compare equal.
It retains the original equal key. Enumeration membership is preserved; order is
unspecified. Current product consumers only use lookup/put for inherited data.
There is no pooling, State eviction, delayed work, changed notification policy,
or new Worker. The public map API is unchanged.

## Reproduce / 재현

Run from the repository root, one command at a time, using the existing external
20-minute wrapper. Do not overlap builds with browser/GPU workloads. These are
examples for a future bounded corpus, not instructions to rerun the completed
20-run corpus automatically.

```powershell
& ./Doroti/eng/invoke-work2-check.ps1 -Label same-work-hamt -Command @('dotnet', 'run', '--project', 'Doroti/validation/framework-work', '-c', 'Release', '--', '--same-work', '.doroti/same-work-hamt.json')
```

Compare this capture with a preserved pre-change capture using
`compare-same-work.py BEFORE AFTER --flutter FLUTTER --output RESULT`.
The archived Dictionary baseline is evidence, not an alternate build option.
To regenerate the Flutter callback fixture, run the pinned SDK's `flutter.bat test
--reporter expanded` in `validation/framework-work/flutter` with the same 20-minute
wrapper and `DOROTI_SAME_WORK_OUTPUT` set to an absolute output path.

Use the repository-local Flutter revision
`56b8e1a851a594b1a154f8ea93270807dab22b9a`, not the separately installed SDK.
The Dart fixture compares the observable callbacks of the same five-tick subtree.
The complete Doroti internal trace is compared strictly A/B. Flutter's internal
wrapper trace, full sample equivalence, IME and semantics are **not** covered.
Flutter runs with debug assertions; Doroti runs Release. This is a functional
fixture, not a Flutter/Doroti performance comparison.

`FrameworkWorkTrace.Start/SetTick/Register/Stop` is an explicit thread-local
contract session. Register stable logical IDs for fixture nodes; automatically
assigned IDs and type names are retained as the wrapper correspondence table.
Events contain numbers; type names are materialized only at Stop. Weak keys avoid
retaining Elements/States/RenderObjects. Overflow invalidates a capture. A/B
comparison reports the first mismatch without filtering events. The current
layout payload captures BoxConstraints; flag bit 4 identifies other constraint
types whose complete payload is not captured. This is intentionally incomplete
Q1 coverage, not a universal parity tracer.

`FrameworkComponentProfile` is enabled by the existing `DOROTI_STAGE_TRACE=1`.
Each four-value slot is calls, inclusive 100ns ticks, managed allocated bytes,
and input item count. Nested intrinsic calls overlap: do not add inclusive times
or allocations to obtain totals. Inheritance put has no nested profile scope in
this workload. Section IDs are cumulative within the current diagnostic thread.
Both diagnostics are disabled during ordinary use; the contract trace is also
disabled during performance runs.

For browser tests, freeze `staticwebassets.build.json` (absolute AssetFile paths),
not `staticwebassets.build.endpoints.json` (relative paths), using
`web-playwright/freeze-build.py`. Serve each frozen root at its own origin.
`measure-state-resize.mjs LABEL resize-break --visit-all` performs 12 wheel inputs
of 600px per column, returns each by -20000px, asserts IDs 0–28, then performs
exactly eight resize inputs. It requires detailed diagnostics. The current indexed sample materializes visible sections initially; all 29
sections must be visited before comparing the fully materialized workload. `summarize-same-work.py` preserves all six raw
runs and verifies an exact final-generation commit.

## Result / 결과

See [2026-09-08 execution record](../../../history/26-09-08/wasm-same-work-execution.md).
The candidate reduces map allocations, but full target-trace coverage, minimal
instrumentation, control workloads and complete memory/physical acceptance are
missing. The later [user-directed unification](../../../history/26-09-08/hamt-indexed-unification.md)
removes the build flag without relabeling these missing gates as PASS.
