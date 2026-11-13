# Web provider

WebAssembly host, browser bootstrap and development server for Doroti applications. Version: `0.4.1-beta`; supported core: `[0.4.0-alpha.1, 0.5.0)`.

See the [Web configuration guide](../../../Doroti/README.md#web) for renderer requirements and the [sample app guide](../../../samples/DorotiTestbedApp/README.md#web-sample) for run and publish commands.

## Development server

Web development supports an app-root `web_dev_config.jsonc` with ordered
`server.proxy` prefix/regex rules and path replacement, host/port, PEM HTTPS and
custom response headers. The same configuration is used by `dotnet run`, watch,
the workspace CLI and VS Code. `Doroti.Tool.Web` carries only
the development server project and C# sources. Runner startup restores normal
NuGet dependencies and builds the server into the app's `.doroti/cache`; external
DLLs are not bundled into the tool package or browser publish assets.

## Startup splash

The shared TypeScript bootstrap creates a splash using the official transparent
`Doroti/branding/doroti-symbol-color.svg`, served by the host as
`_content/Doroti.Host.Web/doroti-symbol-color.svg`. The SVG logo is displayed at
112 × 112 CSS pixels on the blue splash background. The bootstrap keeps
`#doroti-splash` outside `#app` so canvas creation does not remove it. The loader fades
it out only after the engine reports runtime readiness (`started`) and commits
its first canvas frame; startup
failure keeps the splash visible with a retry button. Reduced motion disables
the loading animation and fade. Pass `splash: false` to `startDoroti` to disable
the splash; it is enabled by default.

## Publishing and AOT

### Execution profiles (opt-in)

`DorotiWebExecutionMode=threaded|compatible|auto` controls the deployment:

| Mode | Deployment and requirements |
| --- | --- |
| omitted / `threaded` | Existing single threaded deployment; secure cross-origin isolation and shared WASM memory are required. |
| `compatible` | One non-threaded Mono runtime in a render Worker, WebGL2, no isolation headers required. Worker, OffscreenCanvas and WebGL2 are still required. |
| `auto` | Two complete, sequential, isolated publishes. The loader selects one before importing .NET or transferring the visible canvas. |

```powershell
dotnet publish samples\DorotiTestbedApp\web\DorotiTestbedApp.Web.csproj -c Release -p:DorotiWebExecutionMode=auto
```

The published root contains a v2 `doroti.runtime-profile.json`, common app/host
assets and `runtimes/threaded/_framework` plus `runtimes/compatible/_framework`.
Deploy the entire root as one generation. Runtime files, compressed variants and
fingerprints are not mixed between builds. The common assets must be byte-identical;
missing/conflicting output fails publish. Runtime preload/import-map entries are
removed before the shared HTML is emitted. Only the selected runtime is loaded.

Child restore/build/AOT/static-assets outputs use a fresh generation below the
runner's `obj/doroti-web` rather than nesting inside the parent `ArtifactsPath`:
Emscripten tools can fail on long Windows paths. This intentionally avoids
cross-generation cache reuse, including old TypeScript/static-assets fingerprints.
`--no-build` is not supported for auto publish. Clean these generated directories
with the normal runner output cleanup when no publish is running.

`startDoroti({ executionMode: "auto" })` is the default loader request. The explicit
configured context overrides the option, then `?dorotiExecutionMode=...` is used
when neither is supplied. Legacy `runtimeLocation` remains supported but conflicts
with an explicit incompatible mode fail. A v1 profile never implies an unpublished
second runtime. Forced threaded without isolation and compatible with explicit
WebGPU fail rather than silently changing the request. Configuration, HTTP/CSP,
runtime/heap mismatches and application errors are not capability fallbacks.

Application asset/font/plugin/navigation bases remain at the app root, not the
selected runtime directory, including subpath hosting. Both transports use protocol
v6 and the same render/DOM bridge. The managed-port Worker observer matches Mono 10
runtime metadata or the exact Mono 11 loader URL, not arbitrary `dotnet.*` scripts.
It still depends on the private `JSWebWorker.RunAsyncVoid` contract; this is not a
general Worker-hosting API or CoreCLR support.

For non-isolated development, rebuild and restart the server with
`-p:DorotiWebExecutionMode=compatible -p:DorotiWebIsolation=false`. The explicit
isolation-off option removes configured COOP/COEP response headers as well.
`run/watch` builds a single mode; auto publish does not provide dual Hot Reload.
Standalone Worker Hot Reload is not qualified: use rebuild/page reload. Threaded
runtime recovery reloads the page; compatible recovery retains its selected
descriptor and existing bounded Worker-session restart policy.

### SDK and runtime qualification

The repository SDK pin stays unchanged. SDK 11 building net10 and Mono 11 running
net11 are separate axes. SDK 10/11 net10 Debug and AOT dual starts are validated.
`DorotiWebTargetFramework=net11.0` is **qualification-only and rejected by default
with DOROTIWEB050**: Mono 11.0.0's affine `JSWebWorker` startup hangs even in the
[runtime-only probe](tests/Mono11ThreadProbe/Probe.csproj), without Doroti, Skia or
native relinking. Native link and compatible-mode success do not qualify its
threaded path. Validation tools explicitly pass `DorotiWebQualificationProbe=true`
for requalification, not product deployment. Common net10 libraries are not
retargeted. CoreCLR/NativeAOT WASM are explicitly unsupported. See the current
matrix and remaining device/performance limits in [work.md](../../../work.md).

Use the system SDK explicitly without changing either repository SDK pin:

```powershell
& 'C:\Program Files\dotnet\dotnet.exe' 'C:\Program Files\dotnet\sdk\11.0.100\dotnet.dll' publish samples\DorotiTestbedApp\web\DorotiTestbedApp.Web.csproj -p:DorotiWebExecutionMode=auto
```

System SDK 11 requires `wasm-tools` for net11 and `wasm-tools-net10` for net10.
Child publishes/devserver builds invoke the parent's exact SDK, not `dotnet` SDK
resolution from a different working directory. The current system pack is named
Emscripten 6.0.2 but reports tool version 6.0.3. SkiaSharp 4.156.0 includes separate
3.1.56 and 6.0.2 native archives; native link and actual render-owner tests, not the
SDK major or archive folder name, determine qualification.

Focused contracts: `pwsh -File packages\platforms\web\tests\run-contract-tests.ps1`.
`qualify-publish.ps1` selects a system SDK and imposes a 1,200-second external
process timeout. Release qualification bounds each complete AOT child publish and
the final merge separately; it does not disable AOT or increase the timeout.
`build-contracts.ps1` checks mode conflicts and unsupported runtime diagnostics;
`PublishContracts` checks the actual SDK loader mapping, stale fingerprints and
atomic preflight failures. Browser-only tooling lives under `tests`, not the application
build: run `npm ci --prefix packages\platforms\web\tests`, install its Playwright
Chromium, then `run-browser-tests.ps1 -WebRoot <published-wwwroot>`. It serves the
same deployment on separate actual isolated/non-isolated origins and checks real
first-frame, runtime build info/heap, unselected downloads, overrides and subpaths.
The fixture permits headless SwiftShader and is not a physical-device/GPU
performance certificate. `devserver-smoke.mjs` separately verifies the actual C#
server's configured header overrides and missing-runtime 404 responses.

Web Release publish defaults to Mono WASM AOT. Debug/watch remains non-AOT;
explicit `-p:RunAOTCompilation=false` opts out for a Release comparison. AOT is
applied during `dotnet publish`, so ordinary `dotnet run -c Release` uses the build
output rather than the AOT publish output. Serve the published `wwwroot` with the
COOP/COEP headers for threaded mode; compatible does not require those headers.
The sample READMEs give commands. This profile increases
publish time and native WASM size.

## File selection

Wrap file-selection buttons in `Doroti.Framework.Widgets.FilePickerActivation`
and pass the same `FilePickOptions` to the wrapper and `PickFilesAsync` callback.
The browser associates its semantics identifier with those options and opens the
file input synchronously during the trusted tap or keyboard/accessibility activation.
This preserves WebKit's user gesture across the managed Worker round trip; only
the selection metadata and bounded reads reach managed code. Disabled controls,
scroll/drag gestures and synthetic DOM events do not pre-open a picker. Other
platforms keep their native picker behavior through the capability's default binding.

```csharp
var options = new FilePickOptions(AllowMultiple: true, Extensions: [".txt"]);
new FilePickerActivation(options: options,
    child: CupertinoButton.CreateFilled(child: new Text("Choose files"),
        onPressed: () => PickFiles(options)));
```

## Safari and memory configuration

Sample2 defaults `DorotiWebInterpretWidgetTree=true`. Safari's Worker
stack overflows during its deep recursive Cupertino widget mount under full AOT.
This opt-in interprets `Doroti.Framework.Widgets`, `Doroti.Framework.Rendering`
and `Doroti.Runtime` (including Dart async continuations), while keeping AOT for
Skia, web interop and the remaining assemblies. It trades
some widget/layout execution speed for a smaller native call stack; it does not
change widget lifecycle or render quality. Other applications can opt in to the
same property; `-p:DorotiWebInterpretWidgetTree=false` restores full AOT.
The WebCIL cache tracks this setting, AOT/interpreter mode and assembly content
so a later interpreted publish does not reuse stripped method bodies.

Sample2 defaults `EmccMaximumHeapSize` to 512 MiB, following the
[.NET guidance for mobile Safari](https://github.com/dotnet/runtime/blob/main/src/mono/wasm/features.md#maximum-memory-size).
Override the property when a different workload needs a larger shared heap.
This bounds WASM linear memory, not browser or GPU resident memory.

Sustained Safari/WebKit responsiveness remains unqualified, including with mixed
AOT. Long-running blur and scroll interactions can reload the page or stop
responding. Check these interactions on the intended device before deployment.
Playwright WebKit 26.4 is unsuitable for AOT startup testing because of a stack
overflow.

## Rendering and diagnostics

WebGL bounds outstanding GPU work to two frames using asynchronous completion
fences, including failed or superseded paints. New input can replace a queued
frame while the worker waits for capacity. If admission stays blocked for 250 ms,
the worker calls `gl.finish()` to confirm actual GPU completion and free the
slots, rather than terminating the renderer on a stale fence status. Ordinary
completion stays asynchronous; an old fence on an idle page alone does not
trigger synchronization. Shutdown uses the same recovery before releasing the
renderer; context loss cancels the wait.

Paint releases temporary native shader wrappers on the render owner immediately
after `SKPaint` retains its own reference. This avoids retaining those wrappers
until GC/finalization during repeated paints.

Filter captures reuse exact-sized surfaces across changes in adaptive blur pass
order. Mobile WebGL captures expand to a 32-pixel storage grid so small sigma
changes reuse buffers without changing blur strength or sample coordinates.
A surface is borrowed at most once per frame, and only unused surfaces
can be evicted from the pool. The mobile WebGL pool retains at most 32 MiB of
RGBA8 storage; other backends retain the existing 128 MiB limit. These limits
describe retained pool storage, not total GPU/process memory or temporary
captures. Frame-cost diagnostics expose `webgl.inFlight`, `webgl.peakInFlight`
and `managed.cache.FilterSurfaceRgba8Bytes`/`FilterSurfaceRgba8Limit`.
They also expose `webgl.capacityWaitMs`, `webgl.maxCapacityWaitMs`,
`webgl.synchronizations`, and `raster.phase`/`raster.phaseAgeMs` to distinguish
GPU admission, native paint, and DOM commit waits.
