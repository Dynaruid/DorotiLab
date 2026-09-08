# C2 / C3 / C4 execution costs

The product keeps HAMT + indexed sections. C2/C3/C4 were implemented and tested,
then reverted under work.md's no-improvement rule. The reviewable implementation
is preserved in `history/26-09-08/wasm-c234-candidate.patch`. The patch changes reviewed,
product-owned Framework C# sources; the optional Dart importer does not regenerate
them as part of a product build. No generator lowering or public virtual slot is
changed. Keep the `doroti-reviewed-framework-source` ownership markers when
refreshing the Flutter snapshot and review these adaptations explicitly.

- C2: share four stateless intrinsic adapters and use the existing typed DartMap
  lookup/compute/store operations for intrinsic, dry-layout and baseline caches.
  The nested `putIfAbsent` closure is gone, including on a cache hit. Request
  delegates supplied to `_computeIntrinsics` remain; this is not zero allocation.
  Cache equality, null baseline results, exception behavior, invalidation and
  outer-last assignment after reentrant computation stay unchanged. No per-box
  delegate cache, pool or long-lived request reference is introduced.
- C3: bind the known `RenderSliverMultiBoxAdaptor` receiver and `RenderObject`
  parent-data access statically in `SliverMultiBoxAdaptorElement`. Getters and
  insert/move/remove/indexOf remain virtual; receivers are not hoisted across
  callbacks. The general `ConvertValue` contract and unknown dynamic sites remain.
- C4: reuse static comparison delegates in BuildScope and PipelineOwner, retaining
  the original long comparison's sign and the same List.Sort algorithm. Copy a
  dirty layout tail into the swapped queue without the temporary LINQ list.
  Sliver keys still use a snapshot, now an array, before mutation-capable callbacks.

## Reproduce

Run from the repository root. Each test/build has a 20-minute external timeout.
Do not overlap builds, native GPU validation or browser workloads. A fresh
experiment normally stays within ten runs; record a reason before extending to
twenty, including failed setup, retries and warm-up-only executions.

```powershell
& ./Doroti/eng/invoke-work2-check.ps1 -Label execution-cost -Command @('dotnet', 'run', '--project', 'Doroti/validation/framework-work', '-c', 'Release', '--', '--execution-cost', '.doroti/execution-cost/result.json')
& ./Doroti/eng/invoke-work2-check.ps1 -Label execution-dispatch -Command @('dotnet', 'run', '--project', 'Doroti/validation/dynamic-dispatch', '-c', 'Release')
```

The first command works on both the baseline and candidate. It checks cache results and compute counts, virtual sliver dispatch,
new dirty layout merging and queue release, HAMT contracts and the existing
five-tick subtree trace. It records four bounded eight-hit CLR allocation samples.
Compare the `.trace.json` files with `compare-same-work.py`; include the pinned
Flutter callback capture as described in `README.same-work.md`. The second command
asserts that the compiled sliver element has no generated DLR call sites when the
candidate patch is applied (that assertion is preserved in the patch, because the
restored product still has the original DLR sites).

For WASM, freeze each Release build with `web-playwright/freeze-build.py`, serve
the snapshots at distinct origins, and run `measure-indexed-hamt.mjs LABEL` with
`DOROTI_WEB_BASE_URL` selecting the origin. This existing probe is also applicable
to C2/C3/C4: both builds already use HAMT + indexed. It checks materialization of
IDs 0–28 and the exact final-generation commit for eight same-column and eight
breakpoint inputs. Run three pairs in AB / BA / AB order. Never overwrite the
earlier HAMT corpus's labels or summary.

Also run the existing `--section-index`, `--sample-columns`, `--virtual-dispatch`
FCR-7 modes and browser column-return/selection regression. Small descriptive
timing samples and aggregate target counts do not establish full Flutter internal
trace equality, physical FPS, IME/accessibility or whole-process memory acceptance.

See [the execution record](../../../history/26-09-08/wasm-c234-execution.md) for
the exact 20 runs, original failures, raw measurements and non-adoption outcome.
