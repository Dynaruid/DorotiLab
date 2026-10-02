# Doroti local storage

Doroti-owned tool caches stay under `.doroti`. New test workspaces and evidence use the repository-root `temp/testing/<task>/<run>/` policy in [the testing guide](../tests/README.md).

- `temp/testing`: invocation-owned test scripts/projects, package consumers, logs, captures and builds. Successful suites summarize then remove their run; failed runs remain only for investigation. This path is not relocated by `DOROTI_LOCAL_ROOT` and the legacy cleaner does not delete it.
- `.doroti/tmp`: legacy tool/build workspaces. New test runners use `temp/testing`.
- `.doroti/cache`: reusable analyzer, package-config, and Flutter SDK compatibility data. Cache entries are deterministic and shared instead of copied once per process.
- `Doroti/artifacts`: disposable local build and validation output, including logs, traces, and screenshots. The local-state cleaner removes it with `-Action artifacts` or `-Action all`. Copy any evidence that must be retained elsewhere before cleaning.

The default root is `<workspace>/.doroti`. Set `DOROTI_LOCAL_ROOT` to an absolute path, or to a path relative to the workspace, to move temporary state and caches to another disk.

`DOROTI_LOCAL_ROOT` relocates temporary state and caches only. Artifact cleanup always targets this checkout's `Doroti/artifacts`.

```powershell
# Show current usage.
./Doroti/eng/clean-local-state.ps1

# Remove abandoned temporary entries older than 24 hours.
./Doroti/eng/clean-local-state.ps1 -Action temporary

# Remove all local build and validation artifacts.
./Doroti/eng/clean-local-state.ps1 -Action artifacts -Force

# Remove all temporary, reusable cache, and artifact entries.
./Doroti/eng/clean-local-state.ps1 -Action all -Force
```

Do not place irreplaceable evidence or manually reviewed migration output under `.doroti` or `Doroti/artifacts`; retain those in their tracked documentation or migration paths. Cleanup does not reset historical validation results, but deleted raw artifacts must not be described as still available.

## Repository-wide cleanup

The repository-root `cleanup-plan-all.ps1` discovers current disposable output
instead of using dated release or test-run paths. Its default invocation only
previews targets and their sizes:

```powershell
pwsh -NoProfile -File ./cleanup-plan-all.ps1
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -Execute
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -Scope All
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -Scope All -Execute
pwsh -NoProfile -File ./cleanup-plan-all.ps1 -IncludeDependencies -Execute -WhatIf
```

The default `Project` scope covers `Doroti/artifacts` (including every release),
`temp/testing`, `.tmp`, per-workspace `.doroti/cache` and `.doroti/tmp`, .NET
`bin/obj`, and recognized test/build caches. Raw logs, captures and traces within
the selected directories are deleted too. Retain evidence elsewhere before
executing. Directories with Git-tracked files are skipped, as are upstream
`reference` snapshots and linked targets or ancestors. Ordinary source directories
named `bin` need .NET project or build metadata to qualify. Links inside selected
directories are removed without following them. Parent targets subsume children
so size estimates do not double-count nested output. Shared APFS blocks may make
the actual reclaimed space smaller.

`-IncludeDependencies` additionally selects project `node_modules`; reinstall
those dependencies afterward. The cleaner preserves `.doroti/dev` and
`.doroti/evidence`. It only discovers caches in this checkout; external
`DOROTI_LOCAL_ROOT` locations remain outside its scope.

`DeveloperCaches` selects user NuGet, npm, Python, Dart and, on macOS, VS Code
caches. Only VS Code extension directories explicitly marked obsolete qualify;
active extension versions remain. `AppCaches` selects macOS Chrome, Figma and
CoreDevice app-installation caches. `All` combines these with `Project`.
User-cache locations use the standard profile paths. `-UserCacheRoot` can select
an alternate absolute profile root with the same directory layout; arbitrary
custom cache locations are not discovered. Close builds and affected applications
before `-Execute`.
Dependencies and caches may need to be downloaded or regenerated afterward.
Installed SDKs, simulator runtimes/data, Docker data and browser profiles are
outside these scopes.

Cleanup regression tests use isolated temporary repositories, including actual
deletion and symlink escape checks:

```sh
python3 Doroti/eng/run-with-timeout.py python3 Doroti/tests/cleanup_plan_all.py
```
