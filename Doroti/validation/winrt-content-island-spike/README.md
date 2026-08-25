# WinRT ContentIsland W1 spike

This target is an independent Windows App SDK 2.4 validation program. It does not reference the Doroti framework, product renderer, scheduler, or `Doroti.Host.WindowsAppSdk`.

It creates two standard top-level windows on the same monitor:

- left: one `DesktopAttachedSiteBridge`, one system `ContentIsland`, and one Direct3D 11 composition swap-chain surface with an asymmetric physical-pixel grid;
- right: a bare GDI standard-window control with the same grid markers.

The WinRT window is shown only after its first exact Direct3D present. Resize publication is coalesced to the latest `ContentSiteView` metrics generation. The automated run exercises exact resize, surface recreation, root/island reconnect, and close-after-resize.

## Automated contract run

From the repository root:

```powershell
pwsh -NoProfile -File .\Doroti\eng\validate-winrt-content-island-w1.ps1
```

The command currently exits 2 by design because W1 is a hard FAIL: the exact 2.4 `DesktopAttachedSiteBridge` path requires `ProcessesPointerInput=false` to connect safely in this environment, so pointer ownership remains `raw-hwnd` instead of the W1-required `ContentIsland` primary owner. Build, surface, topology, and lifecycle results do not override that failure.

## Diagnostic-only interactive comparison

```powershell
dotnet run --project .\Doroti\validation\winrt-content-island-spike\Doroti.Validation.WinRtContentIslandSpike.csproj -c Release -- --report .\.doroti\evidence\w1-interactive.json
```

Compare left/top/right/bottom fast `600px/150ms`, medium `600px/300ms`, and slow/fine expand, shrink, and immediate reverse twice per condition. Judge border cadence, cursor-edge tracking, content/grid continuity, and opposite-edge stability separately.

This interactive run is diagnostic-only while the pointer-owner contract is FAIL. It cannot authorize D1 or W2, and no automated report is visible or physical acceptance evidence.
