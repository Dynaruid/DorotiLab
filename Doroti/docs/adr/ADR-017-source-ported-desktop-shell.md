# ADR-017: Source-ported desktop shell

Status: accepted for A1; amended by the G7-3M `osx-arm64` composition-root promotion.

## Decision

The product/default-template window owner is `Doroti.Host.Desktop`, built from Doroti-owned `Doroti.Shell.Core` contracts. Target composition roots inject the pinned `Doroti.Vendor.Avalonia.Win32` or `Doroti.Vendor.Avalonia.Native` source adaptation. Official Avalonia binaries are not part of these product graphs. Vendor assemblies remain internal to their target composition path; public API exposes only Doroti/BCL types.

`Doroti.Host.Avalonia` was retained temporarily as a comparison host until the A2 direct GPU/frame gate produced replacement evidence. It has now been removed together with its A/B samples and verification scripts; the source-ported desktop shell is the only current desktop host path.

The selected Avalonia source revision, input hashes, license and local owners are machine-readable in `migration/avalonia-shell/a1-source-port-provenance.json`. Target evidence is separate in `migration/avalonia-shell/a1-target-evidence.json`. The four-scale coordinate contract is deterministic; native `WM_GETOBJECT` is verified through an external Windows UI Automation client, while physical 96/144 DPI displays are not an A1 completion requirement.

## Consequences

- Window lifecycle, dispatcher, DPI, input/IME, clipboard, cursor and semantics DTO ownership move to the source-port graph.
- The default template temporarily selects `Doroti.Backends.Skia` until A2 provides the direct asynchronous GPU surface.
- A/B results may compare package and source hosts, but only the selected source-host project graph counts as the product dependency boundary.
- Target results never inherit across operating systems. Linux remains deferred X0 work; macOS received independent source, build, live, and package evidence when `osx-arm64` was promoted by G7-3M. Intel macOS remains unverified.
