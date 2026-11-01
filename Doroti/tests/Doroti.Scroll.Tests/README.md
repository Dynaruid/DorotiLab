# Scroll regressions

Run from the repository root with the required 20-minute timeout:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
```

The suite checks queued release velocity, actual pauses, ballistic continuation,
Android MotionEvent clock conversion, semantic snapshot change detection, and
Windows native inertia completion versus recontact. Windows status callbacks use
a small fake COM viewport. A second Windows fixture creates two hidden HWNDs,
checks registration and message delivery through one real DirectManipulation
manager, and verifies both subclasses retire on disposal. It injects no physical
input and does not show a window.
These checks do not establish physical trackpad behavior or display frame pacing.

Invocation-scope regressions check nested same-view builds, cross-view identity,
view/application cancellation, custom scheduler/time/environment overrides and
ExecutionContext flow. Allocation measurements cover 20 actual nested view
scopes; they do not qualify rendered date-picker animation or display pacing.

Inherited-dependency checks use the production element and 512 registered
dependents. Twenty lookups must avoid a linear scan; aspect replacement,
insertion order, callback-time mutation, deactivation and collection after
removal retain their existing behavior.

The frozen-recording regression checks that repeated scroll frames share closed
path/text/numeric recordings, while later producer mutation or disposal cannot
change an admitted frame. Public mutable command inputs still require defensive
copies. Nested live/frozen textures are captured once per texture/freeze pair,
refresh independently on replay, and release exactly once after the last lease.
Closed pictures also share their outer command storage. A public record
`with` expression replacing numeric arguments must regain defensive copying.
Native composition admission traverses retained layers without allocating and
keeps platform views, input shields and excessive depth on the conservative path.
Both Windows hosts also use the linked production Windows inspection helper;
the regression checks nested shields/native views, the 256-layer limit and zero
allocation across repeated inspections. These allocation checks do not measure
scroll frame pacing.
Windows UIA child-list caching checks stable list identity and zero allocation
for repeated queries, live peer values, child order/removal, replaced native
handlers and retry when a peer is not ready. Actual WinUI/UIA heap observations
and their limits are recorded in the
[Windows resume review](../../../history/26/11/01/windows-resume-scroll-review.md).
Scroll semantics checks use a real pipeline/tree: pure offset updates can be
coalesced by an explicitly opting-in host, content/actions win in either order,
and the final flush publishes the exact fractional offset. Extent/policy changes
stay urgent; a disposed renderer releases its listener on a surviving position.
Cached-section fixtures verify that keep-alive moves retain attachment, owner,
parent data and sibling links without recreating children, while real eviction
detaches the complete subtree exactly once.

Native callback checks preserve exact frame timestamps for live owners and allow
late pulses after teardown. Managed dispatcher work stays alive until delivery;
the native notification retains only a token. Failure, duplicate delivery,
concurrent producers and reentrant posts release each registration exactly once.

It also drives the production scheduler/ticker/animation controller at 60/120 Hz
and with missed pulses, checks fractional offsets and final ticker retirement,
unchanged dimensions and recontact, and tests all four semantic scroll boundaries.
Projection checks cover alias-safe allocation-free matrix composition, perspective
and legacy origins, DPR, layout text geometry and obscured text. Android vsync
conversion retains callback age instead of shifting time by the first delayed pulse.

Caret-drag checks reuse immutable semantics text geometry while glyph layout is
unchanged. They verify grapheme ranges and cache invalidation after text edits,
wrapping, text scaling, scroll offsets, RTL/center alignment, obscuring and disposal.
These CPU checks do not qualify physical finger-to-display latency.
[Galaxy cursor-handle drag evidence](../../../history/26/10/31/galaxy-caret-drag.md)
separates the first-drag fix, native magnifier lifetime/IME checks and remaining
sustained-drag performance limits.

Selection-work regressions check that decoration/counter listeners only receive
text changes while the full controller immediately publishes selection/composition,
including reentrant reads and disposal. Projected glyphs are reused without
retaining stale semantic selections; transform, clip, DPR, obscuring and concurrent
projection cases verify invalidation. The
[magnifier/accessibility follow-up](../../../history/26/10/31/galaxy-caret-accessibility-review.md)
separates sample-wide rebuild savings from the independent second-field comparison,
bounded accessibility-consumer diagnostics and remaining motion/display limits.

Collapsed-caret regressions exercise three real RenderEditable peers, unchanged
Material decoration slots, fallback edits/composition/ranges/focus/custom controllers,
and retained leaf freshness. A queued weak notification must keep delivering to a
live owner while allowing an abandoned owner to be collected before the native
callback retires. The [All Components follow-up](../../../history/26/11/01/galaxy-components-caret-fix.md)
records the actual normal-page reproduction, native selection/GC-bridge fix,
delivered APK comparison and IME/lifecycle checks. These remain CPU/injected-touch
evidence rather than physical finger/display acceptance.

Galaxy ADB and continuous single-contact profiling commands and evidence limits:
[2026-10-31 implementation record](../../../history/26/10/31/galaxy-scroll-implementation.md).

For a Windows physical-input investigation, launch with
`DOROTI_WINDOWS_FRAME_TIMING=1`, `DOROTI_WINDOWS_APPSDK_DIAGNOSTICS=1` and
`DOROTI_WINDOWS_APPSDK_REPORT` pointing to a local JSON file. Closing the app writes
the bounded native packet history and the existing framework scroll trace.
Replay its release estimates after building the regression console:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet Doroti/tests/Doroti.Scroll.Tests/bin/Release/net10.0/Doroti.Scroll.Tests.dll temp/scroll-fix/windows-trackpad-report.json
```
