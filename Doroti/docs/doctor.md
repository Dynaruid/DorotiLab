# Environment doctor

The managed CLI resolves workspace v2 and the selected provider/profile. The PowerShell entry points build the CLI and source tooling when required. Doctor probes do not build or restore the application, install workloads, deploy an app, or launch a device.

Run from the repository root with PowerShell 7:

```powershell
pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform windows -Scope managed
pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform web -Scope target -TimeoutSeconds 15
pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform all -Scope full
```

`-App` and `-Platform` are explicit. Aliases come from the workspace; select a different runner through its declared alias. `-Scope` accepts `managed`, `target`, `native`, `tools`, or `full`. `-TimeoutSeconds` bounds each probe from 1 to 120 seconds, default 15. `-DotnetPath` selects the executable for SDK and runner metadata probes. Its working directory is the runner directory, so the nearest `global.json`, SDK feature band, roll-forward and prerelease policy apply.

Provider tooling supplies required hosts, workloads, native tools and additional evaluated build properties. Missing hosts report `Skipped` without probing. Metadata that cannot be evaluated without restoration reports `Partial`; a missing required tool/workload or incompatible SDK reports `Fail`. Probe timeout reports `Partial` and independent probes continue. Output is typed diagnostics serialized at the CLI boundary. Exit codes are 0 for the selected prerequisite scope or skipped host, 1 for failed prerequisites, and 2 for partial prerequisites. `all` continues declared profiles and accumulates the diagnostic exit status.

The current built-in probes cover SDK selection, resolved TFM/RID/build metadata, declared workloads and selected basic tools. Full/native/tools scope retains a separate runtime-acceptance `Partial`. Detailed Android JDK/Gradle/device development transport, Apple signing/Xcode profiles and complete Qt module/native provenance diagnostics remain migration work; they must not be inferred from a managed scope PASS. Read the [current implementation record](migrations/design-platform/resume-2026-10-05.md) for verified coverage.

The former `doroti.doctor/v4` file-report workflow belongs to the [2026-10-04 validation baseline](validation/2026-10-04-full-review.md). The current provider CLI emits diagnostic JSON to stdout; callers may save it to a run-owned path. Native execution, GPU display, physical input/IME, accessibility, signing and clean-machine installation need separate evidence.
