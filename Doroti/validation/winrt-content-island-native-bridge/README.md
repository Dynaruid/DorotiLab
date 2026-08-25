# WinRT ContentIsland bounded native bridge

This diagnostic-only C++/WinRT DLL calls the `DesktopAttachedSiteBridge` input-processing setters on an existing WinRT object. It does not create a window, island, renderer, or second input owner.

Build from the repository root:

```powershell
pwsh -NoProfile -File .\Doroti\eng\build-winrt-content-island-native-bridge.ps1
```

The build pins the Windows App SDK projection metadata to the transitive versions resolved by `Microsoft.WindowsAppSDK 2.4.0` and generates C++/WinRT headers with the installed Windows 10 SDK.

On the 2026-08-25 exact 2.4 W1R run, both native calls returned `0x80131509` when disabling keyboard processing after connecting the system visual island. The managed setter returned the same HRESULT. Therefore this bridge proves that the failure is below the C# projection boundary; it is not a product workaround and does not make D0 or W1R pass.
