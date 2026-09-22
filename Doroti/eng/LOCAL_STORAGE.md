# Doroti local storage

Doroti-owned transient state stays under the ignored workspace directory `.doroti` instead of the operating-system temporary directory.

- `.doroti/tmp`: invocation-owned build, package-consumer, and validation workspaces. The creating process removes its own directory in `finally` blocks.
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
