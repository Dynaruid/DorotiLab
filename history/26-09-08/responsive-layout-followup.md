# Responsive layout follow-up

The user felt the carousel improvement from `carousel-scroll-performance.md`,
but did not feel an improvement when the window reorganized the gallery. This
is consistent with the earlier change reducing raster work without removing
section transfer/rebuild work. Indexed remains opt-in at
`?dorotiTestbedMode=sample&dorotiSectionViewport=indexed`.

## Changes

- Shared Scaffold slot MediaQuery transformation now subscribes locally. Size
  still reaches every slot and render constraints; the Scaffold itself retains
  its padding/inset dependencies. A size-only resize no longer rebuilds the
  surrounding Scaffold configuration. Keyboard/padding behavior is tested.
- Indexed gallery retains the right scroll owner offstage after its first use,
  with its last finite width. It excludes paint, semantics and focus, and disables
  its ticker subtree while hidden. Cold single-column startup does not create it.
- Shared SectionList adds explicit `suspended` and `restoreRetainedChildren`
  controls. A suspended offstage owner processes releases but does not measure or
  materialize children, so it cannot reclaim a section moved to the visible list.
  Restoring ownership happens before tree finalization.
- Whole section wrappers have stable owner keys in addition to their State keys.
  Visible sections move on demand; invisible right sections keep their existing
  owner. This prevents empty card wrappers from blocking a later State transfer.
- Existing section identities, primary anchor mapping, exact-width measurement,
  29-section pinning, zero speculative extent and eager default remain in place.
  The extra retained object is one right scroll-owner subtree, not duplicate
  section State. Full live/transient memory acceptance remains notVerified.

## Diagnosis and preserved failures

The initial carousel-version resize probe measured callback maxima 163.6 ms in
the same column configuration and 382.5 ms across the breakpoint. Temporary
layout/intrinsic instrumentation was used in `profile-wwwroot`; it added material
diagnostic overhead and was removed from production source after attribution.

Scaffold-only comparison removed 14 Scaffold rebuilds in the wide probe and cut
total rebuild calls 966 -> 686, but callback maxima were only 163.6 -> 154.0 ms and
382.5 -> 385.4 ms across columns. This alone did not resolve the user's complaint.
A gesture-handler/typed-call candidate was built but not adopted or measured;
handler replacement already conditions semantics invalidation on handler presence.

Preserved failed artifacts:

- `responsive-slot-wide.failure.json`: one setup attempt started before build
  freeze completed and got HTTP 404; terminated, counted, no valid timing.
- `responsive-handlers-build.log`: discarded candidate's Action<T> namespace
  collision; fixed build retained, candidate subsequently removed.
- `responsive-scaffold-contract.log`: fixture StateSetter/Action type mismatch;
  corrected fixture passed in `responsive-scaffold-contract-fixed.log`.
- `responsive-columns-contract.log`: deep native transfer exposed State loss for
  section 14 at width 1001. Moving the entire section wrapper fixed the ownership
  hole; `responsive-columns-owner-fixed.log` passes all eight widths and keys.
- `responsive-final-functional.log`: 11 Web checks passed, while the new parked
  checkbox test incorrectly expected null -> true. Option 2 is tristate and starts
  null; one click correctly changes it to false. The fixture now asserts mixed
  initially and false after the click and each ownership transfer.
  `responsive-final-parked-fixed.log` passes the corrected check. No app change
  was needed; the original failed trace remains preserved.

The first retained-owner prototype passed 11 Web checks but failed the deeper
native identity check above. Its 124.3/207.2 ms callback maxima are prototype
evidence only, not final acceptance.

## Builds, methods and evidence

All performance probes and tests use a 20-minute timeout. GPU/browser work and
builds are sequential. The one premature 404 probe is explicitly excluded from
timing conclusions and included in the attempt budget.

- Baseline: `.doroti/gallery-motion/served-wwwroot`, previous carousel revision,
  initially served on 5088; its immutable manifest has 562 endpoint hashes.
- Intermediate builds: `.doroti/responsive/profile-wwwroot`, `slot-wwwroot`,
  discarded `handlers-wwwroot`, and `retained-wwwroot`.
- Final: `.doroti/responsive/final-wwwroot`, `final-wwwroot-manifest.json` and
  `final-source.json`; built before final UI checks and comparisons.
- `measure-state-resize.mjs` performs eight CDP width changes, 45 ms apart.
  Wide widths: 1240,1200,1160,1120,1160,1200,1240,1280. Cross widths:
  980,900,800,1100,1400,900,1100,1280. All use 1280x900 initial size and DPR 1.
- Raw profiles: `Doroti/validation/web-playwright/artifacts/state-resize/responsive-*.json`.
  `summarize-responsive.mjs` produces `responsive-final-summary.json`.
- Logs: `Doroti/artifacts/direct-default/responsive-*.log`.

There are fewer than 20 callback samples in many individual sequences, so p95
equals the maximum. Active commit count includes progressive older generations;
final metrics-to-exact timing checks the actual generation inside commit detail,
not the trace's current target epoch. Neither metric is physical window dragging,
display FPS, scan-out or Flutter-parity acceptance.

## Final results

Validation on the final app build:

| Check | Result / evidence |
| --- | --- |
| Release Web build | PASS, zero warnings/errors; `responsive-final-build.log` |
| Shared Scaffold size/inset/padding contract | PASS; `responsive-scaffold-contract-fixed.log` |
| Deep indexed eight-width exact State identity and anchor | PASS; `responsive-columns-owner-fixed.log` |
| Shared extent/lifetime/focus/dynamic ownership contracts | PASS; `responsive-section-contract.log` |
| Web resize geometry at DPR 1/1.25/1.5/2, two initial widths | 8 PASS; `responsive-final-functional.log` |
| Scrolled column return, progress animation, navigation animation | 3 PASS; same log |
| Hidden owner + theme change + checkbox on-demand transfer and return | PASS after fixture correction; `responsive-final-parked-fixed.log` |
| Frozen final endpoints | All 562 SHA-256 hashes matched |
| Physical window dragging, scan-out FPS, IME/overlay/screen-reader migration | notVerified |

The corrected test report is copied to `artifacts/responsive-final-parked-fixed`
(the invocation initially used the default `auto` report directory). Existing
failed final-suite artifacts remain under `artifacts/responsive-final`. Inspected
final PNGs include the 800-wide DPR 1.25 layout and the scrolled two-column return;
no missing section or empty card wrapper was visible in those captures.

Final comparison against the previous carousel build (milliseconds):

| Mode / scenario / run | Callback max before -> after | Final metrics to exact commit before -> after | Active commits before -> after |
| --- | --- | --- | --- |
| Detailed, same columns, 1 | 156.5 -> 123.0 | 47.4 -> 57.9 | 8 -> 13 |
| Detailed, column crossing, 1 | 386.8 -> 210.7 | 492.1 -> 98.3 | 1 -> 3 |
| Detailed, column crossing, 2 | 377.2 -> 192.5 | 367.8 -> 82.1 | 1 -> 3 |
| Minimal, same columns, 1 | not collected | 51.5 -> 51.6 | 8 -> 16 |
| Minimal, column crossing, 1 | not collected | 686.5 -> 149.7 | 1 -> 4 |

Across the two detailed crossing pairs, callback maxima fell 45.5% and 49.0%.
Rebuild calls fell 2775 -> 1388 and 2637 -> 1390; dependency-change calls fell
956/937 -> 106. The wide-only pair reduced callback max 21.4% and rebuilds
966 -> 728, but **did not improve final metrics-to-exact latency**. The detailed
wide-only final latency regressed 47.4 -> 57.9 ms and minimal stayed effectively
flat. This is a bounded exploratory comparison, not a confidence interval or
proof that every resize is now smooth. The crossing workload also processes
more frames while resizing; its total work is not an identical frame count.

Minimal mode disables detailed framework profiling; compact resize/presenter
tracing still records generation completion. Its crossing final exact commit
fell 686.5 -> 149.7 ms in one pair. No browser runtime errors or failed renderer
frames occurred in the final corpus. Raw rows and work/allocation deltas are in
`artifacts/state-resize/responsive-final-summary.json`.

Performance attempts total **19**: initial baseline 2, instrumented attribution 2,
premature 404 setup 1, Scaffold-only 2, retained prototype 2, final detailed 6 and
final minimal 4. Exceeding the usual 10 was necessary to separate Scaffold and
ownership costs, preserve the failed setup and validate the final State-correct
revision with detailed profiling both enabled and disabled. No further reruns
were used to select a favorable result.

Callback maxima still exceed 100 ms; first materialization and synchronous layout
inside a large section remain expensive. Full memory, physical drag, display FPS
and non-Web platform acceptance remain open. P3/P6 and eager-default promotion
gates in `work.md` remain unchanged.

The final frozen build is now served at the user's original 5088 URL. The prior
5088 baseline and temporary 5090-5093 servers were stopped after comparisons;
their immutable files, manifests and logs are retained for reproduction.
