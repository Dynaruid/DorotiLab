# Android touch transition timing

The Galaxy report concerned pauses around finger down/up and stopping a scroll.
On 2026-09-11, SM-S931N / Android 36 / Adreno 830 reproduced long CPU intervals
coincident with Java GC. Profiling and an accessibility-on/off experiment isolated
the hidden native accessibility controls as a major contributor. Managed frame
allocation continued with semantics disabled, but recurring explicit Java GC
disappeared. The final fix keeps accessibility enabled.

## Implementation

- Android Graphite now exposes retained semantic data through AndroidX
  `ExploreByTouchHelper` on the render view. Native `AccessibilityNodeInfo`
  objects are constructed when queried; there are no hidden MAUI Button,
  Entry, Switch, Slider, etc. instances per semantic node.
- Labels, roles, bounds, checkbox/radio/toggle state, text selection, ranges,
  focus, editing, cursor movement, activation and scrolling actions are projected
  into the virtual nodes. Hidden subtrees and removed focus are handled.
- Virtual source identity is set before text selection. Android 16's selection
  anchors refer to that identity; letting the helper set it only afterward lost
  selection offsets. This was caught by the physical-device contract test.
- Choreographer registration avoids an extra UI dispatch when already on the UI
  thread, and retains its Java peer. Surface density updates on connection,
  surface/configuration changes instead of querying native resources every frame.
- Allocation attribution extends the existing bounded, thread-local profiler
  behind `DOROTI_STAGE_TRACE=1` plus `DOROTI_ALLOCATION_PROFILE=1` (and
  `DOROTI_LAYOUT_PROFILE=1` for nested layout). Default builds do not capture it.

The virtual hierarchy uses the Android-recommended
[ExploreByTouchHelper contract](https://developer.android.com/reference/androidx/customview/widget/ExploreByTouchHelper).
The selection ordering follows the
[Android 16 implementation](https://android.googlesource.com/platform/frameworks/base/+/refs/heads/android16-release/core/java/android/view/accessibility/AccessibilityNodeInfo.java).
The exploratory 16 MiB nursery setting and semantics-disable hook were removed.
**GC settings and AOT defaults are unchanged.** Other platform bridges are unchanged
apart from opt-in profiling scopes.

## Reproduction

Build and install a Release APK. Comparisons held compilation mode constant with
`-p:RunAOTCompilation=false -p:AndroidEnableProfiledAot=false`. Each build/test was
bounded by the repository's 1200-second timeout. Keep the phone unlocked and
inspect the screen before using these physical-pixel coordinates (1080 x 2340).

```powershell
python Doroti/validation/run-with-timeout.py python Doroti/validation/app-runner/android-touch-timing.py `
  --serial R3CY30KZA4B --activity crc64c80c495bd333b69c.MainActivity `
  --output Doroti/artifacts/touch-jank/final-verified `
  --open-sample 300 390 --swipe 20 1650 20 950 --rounds 32
```

The script launches the diagnostics gallery with `DOROTI_INPUT_TIMING=1`, opens
Material, and alternates 500 ms swipes with a 400 ms pause. It preserves logs,
screenshot, memory, activity and summary evidence without clearing app data or
system logs. The summary excludes gallery navigation and the first swipe.
Input logs contain all down/up events and moves exceeding 8 ms; frame logs
contain successful renders exceeding 12 ms. Percentiles of all events/frames
cannot be inferred from this subset. The timings cover CPU work, not scan-out.

## Results

Evidence is in ignored local artifacts under `Doroti/artifacts/touch-jank/`.

| Experiment (7 warm swipes) | Input >32 ms | Render >32 ms | Explicit Java GC |
| --- | ---: | ---: | ---: |
| Original (`instrumented`) | 3 | 7 | 12 |
| Nursery tuning only (`gc16m`) | 2 | 4 | 5 |
| Queue/density + nursery, old native projection (`fix`) | 1 | 1 | 5 |
| Profiled old accessibility (`allocation-on`) | 1 | 2 | 5 |
| Profiled semantics disabled, diagnostic only (`allocation-off`) | 0 | 1 | 0 |
| Profiled virtual accessibility enabled (`virtual-profile`) | 0 | 0 | 0 |

The last three rows use allocation instrumentation and must not be treated as
uninstrumented latency measurements. During the on/off experiment, captured
render scopes allocated about 70.3 MB / 60.1 MB and input scopes 18.3 MB / 18.2 MB;
gesture event/frame counts varied slightly with Android injection. Those are
managed allocation totals, not retained memory or leaks. Native control removal
addressed the recurring Java collection stalls despite continued managed work.

| Longer run (31 warm swipes) | Old projection + tuning (`fix-long`) | Virtual nodes, default GC (`final-default-gc`) | Final APK (`final-verified`) |
| --- | ---: | ---: | ---: |
| Input dispatches >32 ms | 3 | 0 | 0 |
| Render work intervals >32 ms | 11 | 1 | 0 |
| Explicit Java collections | 20 | 1 | 1 |
| Max logged input ms | 40.055 | 2.678 | 4.131 |
| Max logged render ms | 71.991 | 37.950 | 29.311 |
| Post-run PSS KiB | 481465 | 362674 | 369303 |

All longer runs finished foreground without observed runtime errors. The final
run includes the source-before-selection fix validated by the native contracts.
PSS is a snapshot, not peak memory or a leak test. A preceding virtual-node run
still had one 37.95 ms render coincident with a Java collection. This establishes
a reduction in recurring pauses, not zero-jank or sustained 120 Hz qualification.
Natural finger cadence, TalkBack interaction, and physical presentation timing
remain `notVerified`. Android accessibility services remained enabled; system
accessibility settings were not changed.

`final-apk.json` identifies the installed APK and confirms no AOT libraries for
this controlled JIT comparison. `final-runtime.log`, `final-activity.txt`,
`final-tree.xml`, and `final-screen.png` capture the normal, profiling-disabled
launch and background/foreground return. The temporary regression app was removed.

## Regression validation

```powershell
python Doroti/validation/run-with-timeout.py python Doroti/validation/android-semantics-provider/run.py `
  --serial R3CY30KZA4B --output Doroti/artifacts/touch-jank/provider-contract
```

The source-linked Android Release/trimmed fixture uses a real native view and
node provider. It passes visibility/hidden subtree, zero native controls, null
flags, pixel bounds, checkbox/toggle state, text and selection, range metadata,
click/edit/selection/word movement/range/scroll actions, disabled/unsupported
actions, immutable query snapshots, no projection feedback, unchanged updates,
stale focus/action rejection, clear and delegate detachment checks.

`framework-work` passes with allocation capture both enabled and disabled. The
exact-allocation loop uses `DOTNET_TieredCompilation=0` in that test process to
exclude tiering warmup allocations. Its pre-existing sliver fixture override was
updated to the current typed getter. No runtime compiler setting was changed.
Android app and provider Release builds completed with zero warnings/errors.
