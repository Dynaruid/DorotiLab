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
