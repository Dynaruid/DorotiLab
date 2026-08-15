# DorotiDemoApp

**English** | [한국어](README.ko.md)

DorotiDemoApp is the native desktop validation app for Doroti's reviewed C# port of the Flutter framework. It builds a real Material widget tree and presents it through either the Win32/WGL target or the AppKit/NSOpenGL `osx-arm64` target while exercising framework state and accessibility end to end.

It is both a runnable gallery and the shared generated-product scenario used by the Windows and G7-3M macOS native gates.

## What the demo contains

- `MaterialApp.builder` and `MaterialApp.home`/Navigator startup paths
- `Theme`, `Scaffold`, `AppBar`, `Card`, and `ListTile`
- Elevated button, checkbox, radio, switch, slider, and floating action button
- `Row`, `Column`, `Stack`, `SingleChildScrollView`, and `ListView.builder`
- Local `State` updates with measurable raster changes
- Semantics nodes and target-native accessibility actions
- Native window and GPU resource-lifecycle checks

The app references promoted product projects under `Doroti/src`; it does not run migration candidates or the former native-list/runtime-v2 demos.

## Requirements

- Windows x64 or Apple Silicon macOS (`osx-arm64`)
- .NET SDK 10.0.300 or a compatible latest patch
- A complete DorotiLab checkout with the `Doroti` project
- PowerShell 7 for the full validation gate

The project selects exactly one target composition root from the host OS or explicit RID. Linux, Intel macOS, mobile, and physical-device acceptance remain `notVerified` unless covered by their own target evidence.

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

`--smoke` first posts a target-native pointer move/down/up tap into the visible window and requires that tap to traverse the framework hit-test/gesture path and update local state. It then exercises every action/selection control, verifies state and pixel changes, checks the semantics tree and strict GPU backend, and confirms balanced native resources before exit.

## Target validation

Run the complete G6-3 gate from the repository root:

```powershell
pwsh -File ./Doroti/eng/validate-g6-material-demo.ps1 -Shard All
```

The Windows gate covers:

- `builder` and `home` entry paths
- first-visible-frame native pointer input and framework hit-test propagation
- six external Windows UI Automation actions
- 300 requested frames over 30 seconds
- screenshot colors, text ink, layout bounds, and interaction deltas
- compiler and widget regressions
- Release product build
- a clean external consumer restored from local packages only

The script also accepts `LiveWindows`, `ExternalConsumer`, `Compiler`, `Regression`, or `Evidence` as `-Shard` values when a narrower run is needed.

On Apple Silicon macOS, run the four G7-3M shards:

```powershell
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Source
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Build
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Live
pwsh -File ./Doroti/eng/validate-g7-macos-shell.ps1 -Shard Package
```

The macOS gate verifies an actual NSWindow, AppKit lifecycle/focus, pointer and fractional wheel input, key/text input, clipboard restoration, an NSAccessibility action, Apple M1 strict-GPU presentation, repeat publish identity, and an external package-only consumer. Physical Korean IME candidate placement, VoiceOver navigation, and precise trackpad gestures remain explicitly `notVerified`.

## Evidence and artifacts

- Committed aggregate evidence: [`../Doroti/migration/flutter-framework/g6-material-demo-evidence.json`](../Doroti/migration/flutter-framework/g6-material-demo-evidence.json)
- G7-3M macOS aggregate evidence: [`../Doroti/migration/macos/g7-macos-shell-evidence.json`](../Doroti/migration/macos/g7-macos-shell-evidence.json)
- Screenshot/layout reference: [`g6-material-reference.json`](g6-material-reference.json)
- Transient run output: `../Doroti/artifacts/g6-material-demo/win-x64/`

Each evidence file is target-scoped. Windows results do not transfer to macOS, macOS automation does not claim physical IME/VoiceOver/trackpad acceptance, and neither target proves unsupported operating systems.

## Project files

| File | Purpose |
| --- | --- |
| [`Program.cs`](Program.cs) | Material gallery, native host loop, smoke interactions, and evidence writer |
| [`DorotiDemoApp.csproj`](DorotiDemoApp.csproj) | Product framework, hosting, and OS/RID-conditioned desktop target references |
| [`g6-material-reference.json`](g6-material-reference.json) | Expected logical geometry, colors, and pixel tolerances |

For the runtime architecture and broader development commands, see the [Doroti runtime README](../Doroti/README.md). Doroti is distributed under the repository's [BSD 3-Clause license](../LICENSE).
