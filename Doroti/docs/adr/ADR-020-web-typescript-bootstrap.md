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
- `doroti.loader.ts` is the only owner of `Blazor.start()`. Repeated `startDoroti()` calls share one promise.

The Flutter Web initialization model is an API/UX reference only. Doroti uses the official standalone Blazor WebAssembly contract: load `_framework/blazor.webassembly.js` with `autostart="false"`, then load the compiled application module without `async`.

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

## Failure and evidence boundary

Missing compiler/config/source/output, unsupported compiler hosts, and TypeScript errors fail with stable `DOROTIWEB` diagnostics. Package/publish evidence separately records the framework loader, application bootstrap, Doroti loader, browser interop, and plugin.

A successful `started` stage proves startup only. Canvas presentation/basic pointer, keyboard/IME/clipboard/resize/interactive ARIA, native targets, physical devices, and cross-target parity retain their separate evidence gates.
