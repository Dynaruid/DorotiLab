# Doroti local storage

Doroti-owned transient state stays under the ignored workspace directory `.doroti` instead of the operating-system temporary directory.

- `.doroti/tmp`: invocation-owned build, package-consumer, and validation workspaces. The creating process removes its own directory in `finally` blocks.
- `.doroti/cache`: reusable analyzer, package-config, and Flutter SDK compatibility data. Cache entries are deterministic and shared instead of copied once per process.
- `Doroti/artifacts`: durable validation evidence. It is not temporary state and is not removed by the local-state cleaner.

The default root is `<workspace>/.doroti`. Set `DOROTI_LOCAL_ROOT` to an absolute path, or to a path relative to the workspace, to move all local state to another disk.

```powershell
# Show current usage.
./Doroti/eng/clean-local-state.ps1

# Remove abandoned temporary entries older than 24 hours.
./Doroti/eng/clean-local-state.ps1 -Action temporary

# Remove all temporary and reusable cache entries.
./Doroti/eng/clean-local-state.ps1 -Action all -Force
```

Do not place evidence or manually reviewed migration output under `.doroti`; those belong in their tracked migration paths or in `Doroti/artifacts`.
