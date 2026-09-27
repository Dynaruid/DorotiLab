# Web startup font assets

Run from the repository root:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/font-assets -c Release
python Doroti/validation/run-with-timeout.py dotnet build samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Release -p:DorotiSampleWebFontSource=Assets
```

The regression uses a transport that rejects every external URL. It verifies app
subpath resolution, zero-HTTP embedded resources, default family aliases and actual
Skia measurements, default CDN coexistence, registration order, cancellation, and
missing/malformed asset configuration with no silent external fallback. TTF/OTF
asset mode also rejects accidental decoder use. The sample's opt-in asset mode
embeds Roboto 400/500/700 plus license and reaches the generated SDK bootstrap through
`DorotiWebFontOptions`; its default mode continues to use CDN fonts.

2026-09-28: the asset build and transport/Skia regressions passed. Actual Chrome at
the sample's asset-mode server rendered proportional text, title weights and
Cupertino icons from embedded resources. Screenshot:
`Doroti/artifacts/font-assets/browser-assets.png` (disposable local evidence).
