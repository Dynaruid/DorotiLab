# Web provider

Host and Target sources are owned by this independently versioned provider. Package identities are unchanged. Version: 0.4.0-alpha.1; supported core: [0.4.0-alpha.1, 0.5.0). Native/bootstrap adoption and physical platform acceptance are tracked separately in the design-platform execution record.

Web Release publish defaults to Mono WASM AOT. Debug/watch remains non-AOT;
explicit `-p:RunAOTCompilation=false` opts out for a Release comparison. AOT is
applied during `dotnet publish`, so ordinary `dotnet run -c Release` uses the build
output rather than the AOT publish output. Serve the published `wwwroot` with the
required COOP/COEP headers; the sample READMEs give commands. This profile increases
publish time and native WASM size. See the [default-profile qualification](../../../Doroti/docs/validation/2026-10-05-web-aot-default.md).
