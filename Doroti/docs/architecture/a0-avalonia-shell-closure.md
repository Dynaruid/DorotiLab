# A0 Avalonia shell source closure

A0 replaces the former two-file Skia selection as the planning and provenance boundary for the source-ported desktop shell. The source is pinned to Avalonia upstream `main@2026-07-31`, commit `f159423f691946e713f454447a780d4677d8a0d2`; the selected 845 files were content-compared with that commit when the snapshot was created. Routine validation checks revision identity, selected files, classification, dependencies and provenance without using content-hash equality as an acceptance gate.

`migration/avalonia-shell/port-selection.json` is the reviewed input. It selects shared platform, dispatcher and render contracts; Win32 lifecycle, services, automation and surfaces; separate X11, Wayland and FreeDesktop closures; the managed macOS bridge plus libAvalonia native source/build inputs; and shared Skia/OpenGL surfaces. Avalonia Control/styling/XAML/visual-tree and composition dependencies are not silently dropped: each is assigned to an `exclude-with-owner` or `Doroti-port` boundary.

`migration/avalonia-shell/shell-dependency-graph.json` is deterministic output. Every selected file carries its source hash, declared symbols, direct dependency node ids, disposition, target project, platform and owner. Dependency nodes use exactly one of `import`, `adapt`, `Doroti-port`, or `exclude-with-owner`; an unclassified node fails the audit. The compile stages A0-W0 through A0-W3 describe how the Windows closure reaches an Avalonia-binary-free build, while Linux and macOS stay explicitly assigned to X0 target work.

The tool commands are:

```powershell
dotnet run --project tools/Doroti.AvaloniaPort -- update
dotnet run --project tools/Doroti.AvaloniaPort -- audit
dotnet run --project tools/Doroti.AvaloniaPort -- stage --output artifacts/avalonia-port-stage
dotnet run --project tools/Doroti.AvaloniaPort -- rebase --previous-source <old> --current-source <new> --output artifacts/avalonia-rebase.json
```

`update` is a review action because it changes selected hashes and the dependency graph. `audit` is read-only and runs through both `eng/doroti.ps1 audit` and the `shell-source-audit` suite. `stage` writes only to the caller-selected review directory, applies configured namespace rewrites and exact text patches, copies the license, and emits source/adapted hashes. `rebase` classifies selected files as `clean`, `added`, `removed`, or `manual-review`; it never overwrites product source.

A0 does not claim that A1/A2 or X0 runtime work is complete. No new source-ported product project is added at this milestone, and the comparison-only `Doroti.Host.Avalonia` package path remains until its A1 removal gate.
