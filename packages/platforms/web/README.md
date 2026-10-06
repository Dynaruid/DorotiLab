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
