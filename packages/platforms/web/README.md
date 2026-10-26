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

Web Release publish defaults to Mono WASM AOT. Debug/watch remains non-AOT;
explicit `-p:RunAOTCompilation=false` opts out for a Release comparison. AOT is
applied during `dotnet publish`, so ordinary `dotnet run -c Release` uses the build
output rather than the AOT publish output. Serve the published `wwwroot` with the
required COOP/COEP headers; the sample READMEs give commands. This profile increases
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
