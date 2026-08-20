# Doroti AppKit Metal spike

This disposable validation runner proves the risky boundary before the AppKit
backend is added alongside the permanent Mac Catalyst backend:

- `net10.0-macos` / `osx-arm64` starts through `MacOSMauiApplication`.
- A public `MacOSViewHandler<TVirtualView,TPlatformView>` hosts a Doroti-owned
  `MTKView`.
- `Doroti.Skia.Rendering` rasterizes the retained scene directly into the
  drawable texture with no software fallback, CPU readback, or full-frame copy.
- A frame becomes presented only from `IMTLCommandBuffer.AddCompletedHandler`.
- Surface generations reject stale Metal completions after resize or detach.

The backend is experimental and unsupported. Its package version is exact-pinned
in `Directory.Packages.props`.

Run the automated live probe with a .NET 10 SDK that has the `macos` workload:

```sh
DOROTI_APPKIT_SPIKE_AUTOMATE=1 \
DOROTI_APPKIT_SPIKE_EVIDENCE=/tmp/doroti-appkit-metal-spike.json \
dotnet run --project validation/appkit-metal-spike/Doroti.Validation.AppKitMetalSpike.csproj
```

From the `Doroti` directory, the repeatable validation entrypoint is:

```sh
pwsh eng/validate-appkit-metal-spike.ps1
```
