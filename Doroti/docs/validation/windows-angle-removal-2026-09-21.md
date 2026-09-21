# Windows ANGLE removal — 2026-09-21

Windows App SDK now supports the default Vulkan presenter and the separately
deployed D3D12 diagnostic presenter. `AngleD3D11` is rejected at presenter selection.
The ANGLE EGL presenter, ANGLE Acrylic presenter, unused MAUI ANGLE spike and
unreachable Windows SKGL branches have been removed. Vulkan retains its Acrylic
option channel, snapshot fields and edge budgets.

`Avalonia.Angle.Windows.Natives` is no longer referenced or shipped. Build and
publish provenance use `doroti.windows.native-provenance/v2`, hashing the native
host and Windows App Runtime bootstrap without an ANGLE dependency. Both the
repository runner and packaged target emit this schema. Historical reports and
the upstream SkiaSharp third-party license text are retained.

Validation used a 1200-second timeout for each build, publish and process check.
Local logs and reports are in `Doroti/artifacts/validation/windows-angle-removal/`.

| Check | Result |
| --- | --- |
| Windows App SDK testbed Release build | PASS, 0 warnings / 0 errors |
| MAUI Windows host Release build | PASS, 0 warnings / 0 errors |
| Publish to a fresh directory | PASS |
| Published files and dependency manifest | PASS, no ANGLE runtime/package |
| Published provenance hashes | PASS, both files match SHA-256 |
| Packaged target provenance task | PASS without ANGLE DLLs |
| Published app with default presenter and full native audit | PASS, exit 0, Graphite/Vulkan/D3D12/DXGI on AMD Radeon 780M |
| Rendering and Acrylic option smoke | PASS, 8 presents, 0 failed terminals, 0 operational debug errors, Acrylic active, 0 failed option revisions |
| Removed `AngleD3D11` selection | PASS, nonzero exit with unsupported-presenter error |
| Tampered native-host hash in manifest | PASS, nonzero exit with provenance mismatch; manifest restored afterward |
| `git diff --check` | PASS |

Physical resize smoothness, mixed-DPI monitor movement, other GPUs and other
platform runtime behavior were not verified by this removal check. The separate
D3D12 diagnostic artifact was not deployed or executed.
