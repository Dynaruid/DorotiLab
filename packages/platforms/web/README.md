# Web provider

Host and Target sources are owned by this independently versioned provider. Package identities are unchanged. Version: 0.4.0-alpha.1; supported core: [0.4.0-alpha.1, 0.5.0). Native/bootstrap adoption and physical platform acceptance are tracked separately in the design-platform execution record.

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

WebGL bounds outstanding GPU work to two frames using asynchronous completion
fences, including failed or superseded paints. New input can replace a queued
frame while the worker waits for capacity. Shutdown drains those fences before
releasing the renderer; context loss cancels the wait.

Filter captures reuse exact-sized surfaces across changes in adaptive blur pass
order. Mobile WebGL captures expand to a 32-pixel storage grid so small sigma
changes reuse buffers without changing blur strength or sample coordinates.
A surface is borrowed at most once per frame, and only unused surfaces
can be evicted from the pool. The mobile WebGL pool retains at most 32 MiB of
RGBA8 storage; other backends retain the existing 128 MiB limit. These limits
describe retained pool storage, not total GPU/process memory or temporary
captures. Frame-cost diagnostics expose `webgl.inFlight`, `webgl.peakInFlight`
and `managed.cache.FilterSurfaceRgba8Bytes`/`FilterSurfaceRgba8Limit`.

The [2026-10-06 iOS blur qualification](../../../Doroti/docs/validation/2026-10-06-web-ios-blur.json)
records an iPhone 12 on iOS 26.6.1: the original AOT build reloaded during its
second Fast adaptive drag; the changed AOT build completed 30 synthetic drags
across all five modes with no reload or failed frame. Non-AOT comparison runs
reduced filter surface creation from 8,918 to 1,488. Resident GPU/process memory
was not measured. Playwright WebKit 26.4 still fails AOT startup with a stack
overflow in both builds; that browser/runtime combination is not qualified.
