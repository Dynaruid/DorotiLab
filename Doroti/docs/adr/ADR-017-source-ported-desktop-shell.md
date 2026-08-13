# ADR-017: Source-ported desktop shell

Status: accepted for A1; A2 rendering cutover pending.

## Decision

The product/default-template window owner is `Doroti.Host.Desktop`, built from Doroti-owned `Doroti.Shell.Core` contracts and the pinned `Doroti.Vendor.Avalonia.Win32` source adaptation. Official Avalonia binaries are not part of this product graph. The vendor assembly remains internal and can be consumed only by the desktop composition root; public API exposes only Doroti/BCL types.

`Doroti.Host.Avalonia` remains in the full repository as a comparison host until the A2 direct GPU/frame gate has replacement evidence. It is removed from `Doroti.Product.slnx` and from the default template at A1, so it cannot silently remain the shipped window owner.

The selected Avalonia source revision, input hashes, license and local owners are machine-readable in `migration/avalonia-shell/a1-source-port-provenance.json`. Target evidence is separate in `migration/avalonia-shell/a1-target-evidence.json`. The four-scale coordinate contract is deterministic; native `WM_GETOBJECT` is verified through an external Windows UI Automation client, while physical 96/144 DPI displays are not an A1 completion requirement.

## Consequences

- Window lifecycle, dispatcher, DPI, input/IME, clipboard, cursor and semantics DTO ownership move to the source-port graph.
- The default template temporarily selects `Doroti.Backends.Skia` until A2 provides the direct asynchronous GPU surface.
- A/B results may compare package and source hosts, but only the selected source-host project graph counts as the product dependency boundary.
- Linux/macOS do not inherit Windows verification and remain X0 work.
