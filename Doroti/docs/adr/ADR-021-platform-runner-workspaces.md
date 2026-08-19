# ADR-021: Platform runner workspaces

- Status: accepted
- Date: 2026-08-19

## Decision

Doroti applications are split into a target-neutral application assembly and one fixed-target runner project per platform workspace. The application owns `Program : IDorotiApplicationStartup`, shared widget source, and shared logical assets. A runner owns its native entry point, package manifest, platform resources, target framework, runtime identifier, host package, and generated bootstrap.

The workspace aliases are `android`, `ios`, `linux`, `macos`, `web`, and `windows`. Their canonical targets are `Android`, `iOS`, `Linux`, `MacCatalyst`, `Web`, and `Windows`. The `macos` alias intentionally names the workspace while its first product remains Mac Catalyst; true AppKit macOS is outside this cutover.

`doroti-workspace.json` is only a path index. Runner project files and their single `DorotiTargetDescriptor` remain the source of truth for TFM, RID, host, native entry kind, and target package.

## Native interop direction

Native Library Interop is an optional app-to-native binding path:

```text
Doroti runner -> .NET binding -> Kotlin/Java or Swift/Objective-C wrapper -> native SDK
```

It is not an application runner and does not start the .NET runtime from Kotlin or Swift. Android uses `AndroidGradleProject`; iOS uses `XcodeProject` plus a source-controlled Objective-C binding API. Optional interop projects are connected only from the matching runner.

## Runtime ownership

- Windows, Android, iOS, and Mac Catalyst runners own native lifecycle entry points and initialize the shared MAUI host/surface.
- The Web runner owns Blazor startup and exactly one loader-owned `Blazor.start()` call.
- Linux uses a managed-owned process with a Qt C ABI shim. Managed code owns startup and Doroti lifetime; Qt owns the native event loop, window, display backend, and native input/IME/clipboard/accessibility integration.
- Framework host code stays in `Doroti.Host.*`; app platform folders contain only runner entry points, manifests, resources, and explicit customization hooks.

The alternative Linux model, a C++ executable using `hostfxr`/`nethost`, was rejected for the first implementation because it duplicates managed runtime discovery, startup binding, exception propagation, and publish-mode logic before the GPU/input vertical slice is proven. It can be reconsidered only if Qt requires native ownership of process startup to share the graphics surface correctly.

## Feasibility evidence

- Existing application descriptor, runtime async, shader, and target graph gates passed on Windows with .NET SDK 10.0.400.
- Native Library Interop upstream revision `07df778f1f85c2ad06cb74d3c8faa6ee9011191c` built its Android binding as `net10.0-android` on this host. Its iOS binding produced a Windows cross-build assembly as `net10.0-ios`; Xcode framework build, ABI generation, simulator launch, device signing, and archive remain `notVerified` because no Mac/Xcode host was used.
- CMake 4.3.2 is installed. Qt (`qmake`/`qtpaths`) is absent, so Linux native build, X11, Wayland, input-to-present, and retained-scene recovery remain `notVerified` on this host.

## Evidence rules

Structure/build, native live, physical/device, native interop, and package/store evidence are independent. A build result cannot be promoted to input, IME, accessibility, GPU, signing, store, or physical PASS. Unsupported local environments remain `notVerified`; a failed contract is `blocked` only when it prevents implementation progress rather than merely preventing a platform-specific live gate.
