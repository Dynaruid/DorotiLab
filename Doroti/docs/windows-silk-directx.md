# Windows DirectX binding migration

2026-09-11: Windows managed DirectX code now uses Silk.NET 2.23.0. Product and
validation project references to Vortice, SharpGen, and SkiaSharp.Direct3D.Vortice
have been removed. Historical documents and ignored comparison artifacts retain
their original evidence.

Silk.NET supplies [Direct3D12](https://www.nuget.org/packages/Silk.NET.Direct3D12/2.23.0),
[Direct3D11](https://www.nuget.org/packages/Silk.NET.Direct3D11/2.23.0),
[DXGI](https://www.nuget.org/packages/Silk.NET.DXGI/2.23.0), and
[D3D11On12](https://www.nuget.org/packages/Silk.NET.Direct3D11.Extensions.D3D11On12/2.23.0)
bindings. SkiaSharp's [GRD3DBackendContext](https://learn.microsoft.com/en-us/dotnet/api/skiasharp.grd3dbackendcontext?view=skiasharp-3.119)
accepts native pointers. The installed SkiaSharp 4.154.0-preview.1.26454.9 assemblies
were also inspected and exercised to verify this API, including GRD3DTextureResourceInfo.

## Implementation

- `src/Doroti.Graphics.DirectX` contains a small managed ownership layer over
  Silk's generated COM structs. These are Doroti owners, not a Vortice dependency.
  Native enum values and COM dispatch come from Silk.NET.
- Every owner adopts exactly one reference. QueryInterface supplies an independent
  reference; Dispose is idempotent. Native calls retain owners through completion,
  including when the GC runs. Factories use static generic constructors without
  reflection activation. This design is not a claim of NativeAOT qualification.
- DirectX DLLs load by absolute Windows system paths and stay loaded for the
  process lifetime. This is required both for valid COM vtables and for the host's
  restricted DLL search policy.
- Skia receives the same adapter/device/queue/resource pointers through its raw
  Direct3D API. Hosts keep these owners alive until Skia surfaces and contexts
  are released. GPU transitions, fence ordering, device selection, Graphite import,
  and presentation choices are preserved.
- MAUI references the helper only for Windows. The Windows App SDK D3D12 presenter
  remains an explicitly deployed Diagnostics artifact. Its product validation can
  include it with `-p:DorotiIncludeD3D12Diagnostics=true`.
- A few collection expressions in MAUI and the product validation now use explicit
  array types to satisfy the installed CsWinRT1032 analyzer without suppressions.
- WinUI ISwapChainPanelNative keeps its existing three-slot IUnknown/fourth-slot
  SetSwapChain ABI; Silk's DXGI package does not provide that WinUI interface.

## Verified results

All runtime commands used `validation/run-with-timeout.py` (1,200-second limit).
Local reports and build logs are under `artifacts/silk-inspect/`.

| Check | Result |
| --- | --- |
| MAUI Windows host and Windows App SDK Diagnostics builds | PASS, zero warnings/errors |
| Nine migrated/current validation projects | PASS builds, zero warnings/errors |
| New helper Release NuGet package | PASS; dependencies are Silk.NET 2.23.0 |
| Restored MAUI Windows / Diagnostics dependency graphs | No Vortice or SharpGen dependency |
| Silk COM ownership validation, Release | PASS; QI, HRESULT, double disposal, module lifetime, 200 forced-GC/fence cycles |
| Native Graphite D3D12 | PASS; AMD Radeon 780M and RTX 4060 Laptop, three sizes each, 72 pixel-verified frames, canceled-frame recovery |
| Windows Composition surface | PASS; 41 BeginDraw, 40 GPU copies, one failure injection, three slots, zero pending callbacks/leaked slots |
| WinRT D3D11 Content Island | PASS automated contract; 19 presents, resize/recreation/reconnection; physical visibility notVerified |
| Ganesh GPU compositing blend and resource owners | PASS |
| Windows App SDK D3D12 product smoke | PASS; 30 presented terminals, zero failed terminals, resize burst drain, one completed device reset, zero operational GPU debug errors |

## Remaining boundaries

- `hwnd-exact-cpp-frame-lease-managed` is migrated at source/reference level but
  cannot build against today's host: its existing `WindowsNativeV1.D3D12HostLease`
  and `D3D12Lease` references no longer exist. The same references were confirmed
  in the pre-change source. Restoring that retired ABI is outside this migration.
- The D3D12 debug layer reports initialization message 1315 four times per Skia
  context (eight with a device reset). A separate run of the original Vortice +
  identical Skia packages reproduced the same four initialization errors. They
  are recorded, not filtered; the migrated product reports zero operational errors.
- The stricter `--resize-cycles 3` probe fails its existing per-request resize count
  assertion when a request is superseded. With resets disabled it observed two
  ResizeBuffers calls, two presented resize generations and one superseded generation,
  with zero failed/unclosed generations or operational GPU errors. This combined
  assertion has not been qualified; its FAIL reports are retained separately from
  the passing standard resize-burst/device-reset smoke.
- MAUI's complete interactive UI, physical scan-out, extended human resize behavior,
  and non-Windows platform builds/runs are not qualified by these checks.

## Reproduce

Run from the repository root:

```powershell
New-Item -ItemType Directory -Force Doroti/artifacts/silk-inspect | Out-Null
dotnet build Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj -r win-x64
dotnet build Doroti/src/Doroti.Host.WindowsAppSdk.Diagnostics/Doroti.Host.WindowsAppSdk.Diagnostics.csproj
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/silk-directx/Doroti.Validation.SilkDirectX.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/native-graphite-d3d12/Doroti.Validation.NativeGraphiteD3D12.csproj -- Doroti/artifacts/silk-inspect/graphite-d3d12.json
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/windows-composition-surface/Doroti.Validation.WindowsCompositionSurface.csproj -- --report Doroti/artifacts/silk-inspect/composition.json
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/winrt-content-island-spike/Doroti.Validation.WinRtContentIslandSpike.csproj -- --automated --report Doroti/artifacts/silk-inspect/winrt-d3d11.json
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj -- --gpu-compositing blend
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj -- --gpu-compositing owners
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj -p:DorotiIncludeD3D12Diagnostics=true -- --presenter D3D12 --smoke-ms 5000 --report Doroti/artifacts/silk-inspect/diagnostics-d3d12.json
```
