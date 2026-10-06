# Web provider

Host and Target sources are owned by this independently versioned provider. Package identities are unchanged. Version: 0.4.0-beta; supported core: [0.4.0-alpha.1, 0.5.0). Native/bootstrap adoption and physical platform acceptance are tracked separately in the design-platform execution record.

The samples and app template show an HTML splash using the official
`Doroti/docs/branding/doroti-app-icon.svg`, served by the host as
`_content/Doroti.Host.Web/doroti-app-icon.svg`. Keep `#doroti-splash` outside
`#app` so canvas creation does not remove it. The shared TypeScript loader fades
it out only after the engine reports runtime readiness (`started`) and commits
its first canvas frame; startup
failure keeps the splash visible with a retry button. Reduced motion disables
the loading animation and fade. Custom HTML without `#doroti-splash` continues
to use the same loader.

Web Release publish defaults to Mono WASM AOT. Debug/watch remains non-AOT;
explicit `-p:RunAOTCompilation=false` opts out for a Release comparison. AOT is
applied during `dotnet publish`, so ordinary `dotnet run -c Release` uses the build
output rather than the AOT publish output. Serve the published `wwwroot` with the
required COOP/COEP headers; the sample READMEs give commands. This profile increases
publish time and native WASM size. See the [default-profile qualification](../../../Doroti/docs/validation/2026-10-05-web-aot-default.md).

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

`Doroti/tests/web_files.mts` covers delayed Worker admission, cancellation,
owner disposal and file-grant revocation. `Doroti/tests/web_file_picker_smoke.py`
checks Sample2's actual canvas input, native picker, managed preview and retry
in a browser served from an isolated build; browser automation is separate from
physical iPhone input qualification.
The [2026-10-06 picker verification](../../../Doroti/docs/validation/2026-10-06-web-file-picker.md)
records the browser and device checks and their limits.

Sample2 additionally defaults `DorotiWebInterpretWidgetTree=true`. Safari's Worker
stack overflows during its deep recursive Cupertino widget mount under full AOT.
This opt-in interprets `Doroti.Framework.Widgets` and `Doroti.Framework.Rendering`
while keeping AOT for Skia, web interop and the remaining assemblies. It trades
some widget/layout execution speed for a smaller native call stack; it does not
change widget lifecycle or render quality. Other applications can opt in to the
same property; `-p:DorotiWebInterpretWidgetTree=false` restores full AOT.
The WebCIL cache tracks this setting, AOT/interpreter mode and assembly content
so a later interpreted publish does not reuse stripped method bodies.

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
until GC/finalization during repeated paints. CPU snapshot regression tests
verify gradient pixels still render correctly after the wrapper is released.

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

Sample2 defaults `EmccMaximumHeapSize` to 512 MiB, following the
[.NET guidance for mobile Safari](https://github.com/dotnet/runtime/blob/main/src/mono/wasm/features.md#maximum-memory-size).
Override the property when a different workload needs a larger shared heap.
This bounds WASM linear memory, not browser or GPU resident memory.
The [sustained Safari check](../../../Doroti/docs/validation/2026-10-06-web-ios-blur-liveness.json)
records a 100-second, five-mode slider PASS and verified stale-fence recovery
in WebKit and Chromium. A physical slider/list-scroll repeat still failed its
application control check and later reloaded, so sustained physical Safari
responsiveness is not fully qualified. Its last samples showed continuing
native frames, rather than a blocked GPU queue. An earlier Safari stress run
has a WebContent `highwater` jetsam report at approximately 1.50 GiB.

The [iOS 27 simulator check](../../../Doroti/docs/validation/2026-10-06-web-ios27-simulator.json)
records the original full-AOT startup stack overflow and the mixed-AOT fix.
Safari 27.0 on the iPhone 17 simulator completed 180 seconds across all five blur
modes, 10,572 continuous slider moves and a tab round trip after each mode:
8,234 submitted frames, zero failed frames, GPU peak two, and retained filter
storage below 32 MiB in one mixed-AOT run. Submitted frames do not measure displayed FPS.
The final source publish, including immediate shader-wrapper release, still
failed a slider/list-scroll control check and stopped emitting main-page heartbeat
during a separate slider-only run's second mode. The startup overflow is fixed;
sustained simulator stability remains unqualified. A diagnostic runtime 10.0.12
comparison also failed the simulator control check, so the pinned 10.0.11 version
was retained. The final physical Safari
retest could not launch because the iPhone was locked; earlier iOS 26.6.1
observations remain separate evidence.

The [2026-10-06 iOS blur qualification](../../../Doroti/docs/validation/2026-10-06-web-ios-blur.json)
records iOS Chrome (WebKit) on an iPhone 12 with iOS 26.6.1: the original AOT build reloaded during its
second Fast adaptive drag; the changed AOT build completed 30 synthetic drags
across all five modes with no reload or failed frame. Non-AOT comparison runs
reduced filter surface creation from 8,918 to 1,488. Resident GPU/process memory
was not measured. Playwright WebKit 26.4 still fails AOT startup with a stack
overflow in both builds; that browser/runtime combination is not qualified.
The initial record labeled physical runs as Safari; the recorded `CriOS` UA
corrects that attribution. These zero-ending sweeps also do not qualify
sustained rendering or application input responsiveness.
