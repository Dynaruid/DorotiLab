# Native frame policy C only

Native hosts now use C as their only frame policy. A/B execution modes and
`NativeFrameLoopOptions` were removed. Web's policy is independent.

Launch with no frame settings. `DOROTI_NATIVE_FRAME_MODE=C` remains accepted;
A, B and unknown values fail initialization before GPU resource creation.
Delete all nonempty values of `DOROTI_VARIABLE_BLUR_SERIAL_FRAMES`,
`DOROTI_VARIABLE_BLUR_PIPELINE`, `DOROTI_NATIVE_PRESENTATION` and
`DOROTI_IOS_SHADER_PRESENTATION`, including values previously selecting C.
Explicit C does not hide obsolete settings. Android validates both process
environment and Intent extras, so remove obsolete extras from launch commands.

This is a source and binary API change in `Doroti.Skia.Rendering`:

- `NativeFrameLoopOptions`, its constructor and resolvers no longer exist.
- `NativeFrameAdmissionPolicy.Decide` and `PrepareAndDecide` no longer accept
  pipeline/asynchronous mode booleans. Pass scene, pending work, native/resize
  state and backend capability; use the admission's presentation decision.
- `NativeFrameConfiguration.ValidateEnvironment` / `ValidateSettings` reject
  obsolete configuration during initialization. `Mode` is always C.

Rebuild consuming native hosts with matching packages. Ship this change as a new
prerelease candidate using the repository's `0.3.0-beta.rc.*` convention; do not
replace the payload behind an already qualified candidate. Local package checks
and archive hashes are recorded in the execution result. Full release, signing
and clean-machine deployment remain separate qualification.

C still drains and serializes native/shield, replay and resize/rotation work.
Backend capability may limit admission to one frame. Neither case selects A/B.
Producer and final consumer completion remain distinct. Blur kernels/defaults,
graphics backend selection, Web and native ABI are unchanged by mode removal.

Current collectors execute only C. Desktop `--baseline-exe` compares preserved
before-C and rebuilt after-C payloads; it is a cross-payload regression check.
GPU pixel tests compare C with an intervening drain against two unfinished C
recordings. Archived A/B/C reports retain their original meaning and hashes.

See the [frame contract](../native-frame-pipeline.md),
[archived follow-up summary](../../../history/26-10-02/work6-summary.md) and
[execution result](../../../history/26-10-03/works/results/2026-10-02-native-frame-c-only.md).
