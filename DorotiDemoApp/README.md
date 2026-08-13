# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp is the native Windows validation app for Doroti's reviewed C# port of the Flutter framework. It builds a real Material widget tree, presents it through Doroti's Win32 host and strict Skia WGL/OpenGL backend, and exercises framework state and accessibility end to end.

It is both a runnable gallery and the always-blocking product gate for the current Goal 6 milestone.

## What the demo contains

- `MaterialApp.builder` and `MaterialApp.home`/Navigator startup paths
- `Theme`, `Scaffold`, `AppBar`, `Card`, and `ListTile`
- Elevated button, checkbox, radio, switch, slider, and floating action button
- `Row`, `Column`, `Stack`, `SingleChildScrollView`, and `ListView.builder`
- Local `State` updates with measurable raster changes
- Semantics nodes and native UI Automation actions
- Native window and WGL resource-lifecycle checks

The app references promoted product projects under `Doroti/src`; it does not run migration candidates or the former native-list/runtime-v2 demos.

## Requirements

- Windows x64
- .NET SDK 10.0.300 or a compatible latest patch
- A complete DorotiLab checkout with the `Doroti` project
- PowerShell 7 for the full validation gate

The current target package rejects non-Windows execution. Other operating systems and physical devices remain `notVerified`.

## Run the app

From the repository root:

```powershell
dotnet run --project ./DorotiDemoApp
```

The default entry path is `MaterialApp.builder`. To run a short automatic smoke test:

```powershell
dotnet run --project ./DorotiDemoApp -- --smoke --entry builder --frames 3 --duration-ms 15000
```

Use the Navigator-backed `MaterialApp.home` path with:

```powershell
dotnet run --project ./DorotiDemoApp -- --smoke --entry home --frames 3 --duration-ms 15000
```

`--smoke` first posts a native Win32 mouse move/down/up tap into the visible window and requires that tap to traverse the framework hit-test/gesture path and update local state. It then exercises every action/selection control, verifies state and pixel changes, checks the semantics tree and GPU backend, and confirms balanced native resources before exit.

## Full validation

Run the complete G6-3 gate from the repository root:

```powershell
pwsh -File ./Doroti/eng/validate-g6-material-demo.ps1 -Shard All
```

The gate covers:

- `builder` and `home` entry paths
- first-visible-frame native pointer input and framework hit-test propagation
- six external Windows UI Automation actions
- 300 requested frames over 30 seconds
- screenshot colors, text ink, layout bounds, and interaction deltas
- compiler and widget regressions
- Release product build
- a clean external consumer restored from local packages only

The script also accepts `LiveWindows`, `ExternalConsumer`, `Compiler`, `Regression`, or `Evidence` as `-Shard` values when a narrower run is needed.

## Evidence and artifacts

- Committed aggregate evidence: [`../Doroti/migration/flutter-framework/g6-material-demo-evidence.json`](../Doroti/migration/flutter-framework/g6-material-demo-evidence.json)
- Screenshot/layout reference: [`g6-material-reference.json`](g6-material-reference.json)
- Transient run output: `../Doroti/artifacts/g6-material-demo/win-x64/`

The evidence is explicitly scoped to automated Windows x64 native behavior. It must not be generalized to physical devices or non-Windows targets.

## Project files

| File | Purpose |
| --- | --- |
| [`Program.cs`](Program.cs) | Material gallery, native host loop, smoke interactions, and evidence writer |
| [`DorotiDemoApp.csproj`](DorotiDemoApp.csproj) | Product framework, hosting, and Windows target references |
| [`g6-material-reference.json`](g6-material-reference.json) | Expected logical geometry, colors, and pixel tolerances |

For the runtime architecture and broader development commands, see the [Doroti runtime README](../Doroti/README.md). Doroti is distributed under the repository's [BSD 3-Clause license](../LICENSE).
