# ADR-020: TypeScript-owned Web bootstrap and browser source

## Status

Accepted; updated 2026-09-08 for Worker-only rendering.

## Decision

Applications edit runner-owned `web/src/**/*.ts` and handwritten HTML under
`web/wwwroot`. Doroti owns `Doroti.Host.Web/Web/*.ts` and the public loader declarations.
`Microsoft.TypeScript.MSBuild` 7.0.0 compiles TypeScript into configuration-specific
`obj` directories. Full declaration checking remains enabled. Runtime JavaScript is
a build/publish artifact; source, compiler tools and source maps are excluded from
Release publish. CanvasKit npm acquisition and its runtime assets have been removed.
The .NET WASM SDK still owns its native build tools.

Each TypeScript compile replaces its isolated generated output directory. The build
validates that this is a strict child of the intermediate root before cleanup, so a
removed source module cannot survive in static assets or a subsequent package.

`doroti.loader.ts` supports `worker-direct-webgl` (default) and `offscreen-worker`.
Omitted, `auto`, and unknown values select direct. Retired `worker-canvaskit-webgl`,
`document-webgl`, and `offscreen-bitmap` URLs therefore select direct under the same
unknown-value policy. Required Worker/WebGL capabilities failing is an error; no
same-thread or CanvasKit fallback is available.

- Direct transfers the visible canvas to the render Worker once. SkiaSharp WASM and
  WebGL2 render there; main clips the completed capacity without stretching old content.
- `offscreen-worker` renders in the Worker and transfers completed ImageBitmaps to
  main's `bitmaprenderer`. This remains distinct from the retired same-thread bitmap mode.
- Main owns DOM/input/IME/semantics, clipboard and plugin endpoints. Framework/layout,
  Skia and GPU objects belong to the render Worker.
- `runtimeLocation: "worker"` initializes one runtime in that Worker. `"main"` requires
  threads and cross-origin isolation, initializes one runtime on main, and starts its
  shared-runtime JSWebWorker render role. See [ADR-003](ADR-003-web-main-runtime-render-worker.md).

## Bootstrap contract

HTML executes the compiled `doroti_bootstrap.js` module. Framework preload and import-map
placeholders are resolved by `OverrideHtmlAssetPlaceholders` during build/publish:

```html
<link rel="preload" id="webassembly" />
<script type="importmap"></script>
<script type="module" src="doroti_bootstrap.js"></script>
```

The loader resolves the fingerprinted `dotnet.js` module through that import map.
There is no `Blazor.start()` path, Blazor loader link, or `blazorOptions` callback API.
The generated managed `Main` has no UI work; generated `StartWorker`/`StopWorker`
exports start and dispose the render role. The Blazor WebAssembly SDK remains the
build/asset pipeline; it does not own document-rendered Doroti components.

```ts
await startDoroti({
  configure(context) {
    context.runtimeLocation = "worker";
  },
  onStage(stage) {
    document.documentElement.dataset.dorotiBootstrapStage = stage;
  },
  onError(error) {
    document.getElementById("app")!.textContent = `Doroti failed to start: ${error}`;
  },
});
```

Repeated calls share one startup promise. Stage/error callbacks preserve the original
failure. `started` proves runtime readiness, not first visible content, physical input,
performance, IME or accessibility acceptance.

## Historical decisions

The original same-thread front/staging and detached bitmap paths, the CanvasKit split
UI/Raster experiment, and the 2026-09-07 direct-default change are retired or superseded.
Their original failures and bounded qualification remain in history, including the
[direct-default report](../../../history/26-09-07/web-direct-default-execution.md) and
[retired CanvasKit README evidence](../../../history/26-09-08/web-canvaskit-retired-readme.md).
The 2026-09-08 removal does not reclassify those results as passing.
