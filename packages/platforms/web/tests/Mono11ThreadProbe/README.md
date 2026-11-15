# JS-affine thread startup probe

This project deliberately excludes Doroti, Skia and native relinking. It calls
`JSWebWorker.RunAsyncVoid` on the browser runtime's JS synchronization context,
using the same affine entry point as the render owner. `Task.Run` is not a substitute.

## 2026-11-15 diagnosis

System SDK 11.0.100, Mono/browser runtime 11.0.0, WASM SDK/Mono targets 11.0.0,
Emscripten SDK pack `6.0.2/11.0.0` (actual Emscripten tool **6.0.3**) reproduce the
hang with the default initial/unused pools of 7/1. Chromium 158.0.8127.18 logs the
caller as managed thread 1 with `JSSynchronizationContext`; `Thread.Start` does not
return within 60 seconds and no affine owner enters. No Worker/CSP/network error
is reported. The same project targeting net10/10.0.15 starts and returns normally.

The managed [JSWebWorker implementation](https://github.com/dotnet/runtime/blob/v11.0.0/src/libraries/System.Runtime.InteropServices.JavaScript/src/System/Runtime/InteropServices/JavaScript/JSWebWorker.cs)
still creates a dedicated thread and installs JS interop on it. The changed boundary
is [Mono 11's pthread loader/pool](https://github.com/dotnet/runtime/blob/v11.0.0/src/mono/browser/runtime/pthreads/ui-thread.ts):
it selects the loader entry URL, sends compiled module/memory in the preload reply,
and tests `worker.loaded` when selecting a spare Worker. The installed Emscripten 6
native loader acknowledges loading with `cmd: 3` but no longer sets that field.
The runtime therefore selects an unready Worker; synchronous `Thread.Start` on the
browser thread can prevent the messages needed to finish loading it.

Diagnostic tracing confirms `Failed to find loaded WebWorker` with spare Workers
still present, rather than a missing Worker URL. Increasing the pool alone changes
timing and is insufficient as a readiness guarantee. Doroti's Mono 11 connection
now configures 16 initial Workers and waits for their native preload receipts,
allowing the runtime handlers to process them before entering managed startup.
The pool leaves room for runtime threads, lazy export binding and the affine owner
without constructing a replacement in that synchronous entry. It uses the public
Worker event boundary; it does not read or modify a live PThread pool or the
`loaded` field. Other messages continue to the runtime unchanged. Mono 10 keeps its
existing initialization and worker-asset handling.

The independent probe with this preload admission passed three fresh Chromium
starts, with `Thread.Start returned` and `owner-enter` on a JS synchronization
context. This is **runtime startup evidence only**. Isolated-generation application
Debug/Release-AOT frame, paint-owner, input, texture and shutdown checks passed
separately. Final incremental-cache requalification was subsequently paused with
a remaining in-place net11 shutdown console error. Physical-device and hardware
performance gates also remain open. DOROTIWEB050 and the net10/threaded defaults
are unchanged. See the exact scope and resume point in [work.md](../../../../../work.md).

## Reproduction

Use an explicit system SDK and separate artifacts paths; do not edit the repository
SDK pins. `run-validation.ps1` supplies the external 1,200-second process limit.

```powershell
$dotnet = 'C:\Program Files\dotnet\dotnet.exe'
$sdk = 'C:\Program Files\dotnet\sdk\11.0.100\dotnet.dll'
$tests = (Resolve-Path 'packages\platforms\web\tests').Path
$output = Join-Path (Get-Location) 'Doroti\artifacts\thread-probe'
$runner = Join-Path $tests 'run-validation.ps1'
& $runner -Executable $dotnet -Arguments @($sdk, 'publish', (Join-Path $tests 'Mono11ThreadProbe\Probe.csproj'), '-c', 'Debug', '--artifacts-path', $output, "-p:PublishDir=$output\publish\") -LogPath "$output\build.log"
if ($LASTEXITCODE) { throw 'Probe publish failed.' }
& $runner -Executable (Get-Command node).Source -Arguments @((Join-Path $tests 'mono-thread-probe.mjs'), "$output\publish\wwwroot", '--expect-blocked') -LogPath "$output\default.log"
if ($LASTEXITCODE) { throw 'Default-pool reproduction did not match its expected boundary.' }
& $runner -Executable (Get-Command node).Source -Arguments @((Join-Path $tests 'mono-thread-probe.mjs'), "$output\publish\wwwroot", '--pool=16', '--wait-preload') -LogPath "$output\preloaded.log"
```

Run these commands in PowerShell 7; all children use `run-validation.ps1`. `--expect-blocked`
recognizes only the exact pre-Thread.Start stall and labels it **blocked**, never
product PASS. `--trace` enables runtime tracing. For the net10 control use SDK
10.0.403 and `-p:DorotiThreadProbeTargetFramework=net10.0` in another artifacts path.
To test a fixed stable runtime pack, override `RuntimeFrameworkVersion`, retain the
unmodified/default-pool reproduction, and requalify the full application matrix.
