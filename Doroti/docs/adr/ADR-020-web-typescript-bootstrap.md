# ADR-020: TypeScript-owned Web bootstrap and browser source

## Status

Accepted.

## Decision

Doroti Web application policy, the Doroti loader, browser interop, and JavaScript plugin examples use TypeScript as product source. Browser-executed JavaScript is a build/publish artifact only.

- Applications edit runner-owned `web/src/**/*.ts` and handwritten files under `web/wwwroot`.
- Doroti edits `Doroti.Host.Web/Web/*.ts` and owns the loader declaration package.
- `Microsoft.TypeScript.MSBuild` 7.0.0 is restored only by a Web runner with `web/tsconfig.json`.
- The compiler writes under target/configuration-specific `obj`; Release publish excludes maps, TypeScript source, config, and compiler/tool assets.
- Node, npm, Bun, and bundlers are not required.
- `doroti.loader.ts` selects `worker-canvaskit-webgl` by default, or explicit `document-webgl`, `worker-direct-webgl`, `offscreen-bitmap`, or `offscreen-worker` before any managed runtime starts. Repeated `startDoroti()` calls share one startup promise.
- Same-thread modes load `_framework/blazor.webassembly.js` and call `Blazor.start()` exactly once.
- Worker modes do not call `Blazor.start()` on main. Main TypeScript owns DOM input, IME, semantics, plugins, clipboard, and runtime supervision. One persistent module Worker starts the .NET runtime from `_framework/dotnet.js` and exclusively owns the Doroti app/framework and Skia. The legacy Worker path returns `ImageBitmap` objects to main; the direct qualification path owns Worker rAF and WebGL2 on the transferred visible canvas.

The Flutter Web initialization model is an API/UX reference only. Application HTML executes only the compiled `doroti_bootstrap.js` module. The Web runner enables .NET 10 HTML asset placeholder replacement, and application HTML declares the framework preload, fingerprinted Blazor loader preload, and import-map placeholders in `head`. In same-thread modes the shared Doroti loader reads the generated loader URL, injects it with `autostart="false"`, waits for the script to load, and then calls `Blazor.start()` exactly once. In worker mode the loader creates the DOM host directly and starts `doroti.raster.worker.js`; main and worker managed runtimes are never started together.

```html
<link rel="preload" id="webassembly" />
<link rel="preload" id="doroti-blazor-loader"
      href="_framework/blazor.webassembly#[.{fingerprint}].js"
      as="script" fetchpriority="high" />
<script type="importmap"></script>
<script type="module" src="doroti_bootstrap.js"></script>
```

`OverrideHtmlAssetPlaceholders` is required for Web runners. Build and publish replace these placeholders with high-priority framework preload links, import-map content, and the fingerprinted Blazor loader URL. The loader uses the `doroti-blazor-loader` link URL and falls back to resolving the stable loader path against `document.baseURI`; it copies preload integrity, cross-origin, and referrer-policy metadata to the injected script so the browser can reuse the preloaded response. It reports `DOROTIWEB024` when the script cannot be loaded and `DOROTIWEB020` when the loaded script does not expose `Blazor.start()`.

## Application bootstrap

```ts
/// <reference types="doroti-loader" />
import { startDoroti } from "./_content/Doroti.Host.Web/doroti.loader.js";

await startDoroti({
  configure(context) {
    context.blazorOptions.configureRuntime = (runtime) => {
      // Configure the runtime builder supported by the selected Blazor patch.
      void runtime;
    };

    context.blazorOptions.loadBootResource = (type, name, defaultUri, integrity) => {
      console.debug("Doroti boot resource", { type, name, defaultUri, integrity });
      // Returning undefined preserves Blazor's default URI, integrity, and cache behavior.
      return undefined;
    };
  },
  onStage(stage) {
    document.documentElement.dataset.dorotiBootstrapStage = stage;
  },
  onError(error) {
    document.getElementById("app")?.replaceChildren(`Startup failed: ${String(error)}`);
  },
});
```

When a custom `loadBootResource` performs `fetch`, it must pass the received integrity value, for example `fetch(defaultUri, { integrity, cache: "no-cache" })`. Service-worker registration can be added later in the same TypeScript bootstrap; this ADR does not add PWA behavior.

## Renderer policy and ownership

- `document-webgl` preserves the retained front/staging fallback on the visible WebGL2 canvas.
- `offscreen-bitmap` runs the managed runtime on main but rasters to a detached `OffscreenCanvas`, awaits `createImageBitmap`, and commits an exact bitmap to the visible `bitmaprenderer`.
- `offscreen-worker` uses the same exact bitmap contract across a versioned main/worker protocol. Input samples and immutable resize epochs cross to the worker; completed `ImageBitmap` objects cross back as transferables. Per-frame scene JSON, CPU readback, Blob/PNG encoding, and a second main managed runtime are forbidden.
- `worker-direct-webgl` transfers the visible canvas exactly once. The persistent Worker owns Emscripten WebGL2 framebuffer 0, Skia submission, surface/context generation, and Worker rAF. The direct path creates no `ImageBitmap`, has no `bitmaprenderer` display receipt, and remains explicit opt-in while qualification is incomplete.
- Main/Worker messages use runtime-validated protocol v2 envelopes. Unknown versions, kinds, non-positive sequences, or illegal runtime transitions fail closed. A fatal direct Worker is restarted at most once; its external leases are closed as `failed(runtime-lost)`, the dead transferred canvas node is replaced, and DOM/input/IME/semantics endpoints are rebound through a new host.
- All async presenters keep one started `current` and one replaceable `latest` request. Request id, resize generation, context generation, terminal, and bitmap ownership remain paired. A main display receipt is named `submitted`, not scan-out/presented.
- As of 2026-09-05, `auto` and omitted renderer selection use `worker-canvaskit-webgl` by product decision. Explicit renderer overrides remain available; prior evidence and outstanding qualification gates remain recorded in history.

## Failure and evidence boundary

Missing compiler/config/source/output, unsupported compiler hosts, and TypeScript errors fail with stable `DOROTIWEB` diagnostics. Package/publish evidence separately records the framework loader, application bootstrap, Doroti loader, browser interop, and plugin.

A successful `started` stage proves startup only. Canvas presentation/basic pointer, keyboard/IME/clipboard/resize/interactive ARIA, native targets, physical devices, and cross-target parity retain their separate evidence gates.
