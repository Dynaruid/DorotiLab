# WinRT ContentIsland W1R spike

This target is an independent Windows App SDK 2.4 validation program. It does not reference the Doroti framework, product renderer, scheduler, or `Doroti.Host.WindowsAppSdk`.

It creates two standard top-level windows on the same monitor:

- left: one `DesktopAttachedSiteBridge`, one system `ContentIsland`, and one Direct3D 11 composition swap-chain surface with an asymmetric physical-pixel grid;
- right: a bare GDI standard-window control with the same grid markers.

The WinRT window is shown only after its first exact Direct3D present. Resize publication is coalesced to the latest `ContentSiteView` metrics generation. `DesktopAttachedSiteBridge` disables pointer processing but keeps keyboard processing enabled as an explicit Windows App SDK 2.4 runtime floor. The system root island registers no keyboard, pre-translate, focus, or pointer `Input*Source`; the sole top-level `WinRtTopLevelNativeIngress` produces Doroti pointer/keyboard packets and owns focus observation, client cursor, capture, and the HWND UIA root.

The automated run exercises exact resize, surface recreation, root/island reconnect, close-after-resize, pointer history and capture/cancel terminals, ordinary/system/dead/surrogate keyboard input, focus, client/non-client cursor ownership, and `WM_GETOBJECT` root delegation. An independent WndProc trace proves one observed keyboard message produces one Doroti keyboard packet; a synthetic IME start/update/end lifecycle is observed and delegated to `DefWindowProc` while the future `WinRtTextInputAdapter` remains reserved.

The bounded C++/WinRT bridge under `../winrt-content-island-native-bridge` remains diagnostic evidence that the runtime floor is not a C# projection artifact. It is not built, copied, or loaded by W1R and is not an alternate input path.

## Automated contract run

From the repository root:

```powershell
pwsh -NoProfile -File .\Doroti\eng\validate-winrt-content-island-w1r.ps1
```

The validator accepts automated contract `PASS` while keeping `visibleStatus=notVerified` and physical Korean IME status `notVerified`. Synthetic IME lifecycle coverage is not physical IME, candidate-window, or caret proof. The superseded `validate-winrt-content-island-w1.ps1` remains an intentional exit-code-2 historical W1-A gate.

## Diagnostic-only interactive comparison

```powershell
dotnet run --project .\Doroti\validation\winrt-content-island-spike\Doroti.Validation.WinRtContentIslandSpike.csproj -c Release -- --report .\.doroti\evidence\w1r-interactive.json
```

Compare left/top/right/bottom fast `600px/150ms`, medium `600px/300ms`, and slow/fine expand, shrink, and immediate reverse twice per condition. Judge border cadence, cursor-edge tracking, content/grid continuity, and opposite-edge stability separately.

Use the exact binary hash recorded by the automated W1R manifest. Automated contract evidence is not visible or physical acceptance evidence. Record the user-visible comparison separately before making the D1 decision; W2 remains prohibited until D1 selects the standard-shell candidate.
