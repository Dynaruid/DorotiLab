# WinRT owned-envelope D1-C spike

This independent Windows App SDK 2.4 validation target does not reference Doroti product/framework code. It keeps one work-area-sized `WS_POPUP` HWND/AppWindow fixed and moves/resizes an app-owned frame inside one root system `ContentIsland`.

Two composition surfaces exist only as visible/hidden preparation slots. Exact pixels are rendered into the hidden slot first; the root removes its sole child and inserts the prepared front with its offset and extent in one composition transaction. Exactly one surface is simultaneously visible. The outer HWND never changes during owned resize.

The idle HWND region is constrained to the visible owned rect with `SetWindowRgn`. A custom drag temporarily opens the envelope under capture and constrains it again only after the final exact front is visible. Automated evidence verifies fixed envelope bounds, one root child, latest-generation visibility, and region inside/outside isolation. The application creates no child HWND; the exact 2.4 runtime creates one internal `InputSiteWindowClass` child for the connected system `ContentIsland`, which is recorded separately and is not a Doroti render or packet-producing child.

This spike does not establish Snap Layouts, system menu, taskbar preview, maximize/restore, UIA bounds, physical Korean IME, or product acceptance. Those shell risks remain `notVerified` because the full fixed envelope and visible owned rect are intentionally different.

## Automated contract

```powershell
pwsh -NoProfile -File .\Doroti\eng\validate-winrt-owned-envelope-d1c.ps1
```

## Interactive physical gate

```powershell
dotnet run --project .\Doroti\validation\winrt-owned-envelope-spike\Doroti.Validation.WinRtOwnedEnvelopeSpike.csproj -c Release -- --report .\.doroti\evidence\d1c-interactive.json
```

Drag the colored title strip to move and the outer 10-pixel frame to resize. Test Left/Top/Right/Bottom and corners at fast, medium, and slow/fine speeds, then verify clicks outside the idle frame and clicks after drag. Press `Alt+F4` or `Esc` to close.
