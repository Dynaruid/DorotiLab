# Doroti documentation map

Platform widget behavior: [context menus / 플랫폼별 컨텍스트 메뉴](context-menus.md).

Start with [ADR-019](adr/ADR-019-product-framework-source-ownership.md). For Web source and startup policy, continue with [ADR-020](adr/ADR-020-web-typescript-bootstrap.md). The current default Windows host and presenter are defined by [ADR-025](adr/ADR-025-windowsappsdk-hwndexact-angle.md).

- `adr/`: durable decisions. Later ADRs supersede conflicting earlier decisions.
- `architecture/`: implementation records from the G3-G7 bootstrap and migration period. Files named for a milestone preserve what was built and verified at that time; their `validate-g*`, `prepare-g*`, `promote-g*`, and review commands are retired historical commands.
- `validation/`: contracts, fixtures, and committed target/Web evidence. `doroti.ps1 validate` is the supported aggregate entry point; target-specific scripts under `eng/` are maintainer diagnostics with their own evidence boundaries.

Current commands are:

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 doctor
pwsh -File ./Doroti/eng/doroti.ps1 build -App ./DorotiDemoApp -Platform windows
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows
pwsh -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows -WindowsBackend Maui
```

The first Windows command selects Windows App SDK 2.4, `HwndExactCpp`, and managed ANGLE/EGL-D3D11. The second selects the independent MAUI backend; neither path silently falls back to the other.

Build output is not native-live, browser-live, physical-device, accessibility, compositor,
or visible-product acceptance. Retained FCR and platform records describe only the source
fingerprints that produced them.
