# G7-3M macOS `osx-arm64` source-port and RID package

> Historical bootstrap record. The milestone validator has been retired; ADR-019 defines the current product ownership boundary.

Status: implemented and verified on Apple Silicon macOS on 2026-08-15.

G7-3M promotes `osx-arm64` from deferred desktop work to a required Goal 7 target. The product uses a real AppKit `NSWindow`, an arm64 `libAvalonia.dylib` built from reviewed Objective-C++ source, and Skia over a hardware NSOpenGL context. It does not add Avalonia UI, Control, XAML, visual-tree, or Composition binaries to the product graph.

## Ownership and dependency direction

`Doroti.Shell.Core` owns backend-neutral typed capabilities for windowing, input, text editing, clipboard, cursor, graphics, focus, input test injection, and accessibility. `Doroti.Host.Desktop` consumes those capabilities and no longer constructs or references Win32 concrete types.

```text
DorotiDemoApp
    -> Doroti.Host.Desktop.Framework
    -> Doroti.Host.Desktop
    -> Doroti.Shell.Core

Doroti.Target.macOS.osx-arm64
    -> MacOsShellPlatformFactory
    -> Doroti.Vendor.Avalonia.Native
    -> AppKit / NSOpenGL / CoreGraphics
```

`Doroti.Target.Windows.win-x64` remains the Windows composition root and injects `Win32ShellPlatformFactory`. The demo chooses one target project from the host OS or explicit RID, so an `osx-arm64` publish does not import the Win32 target graph. Both targets implement the shared `IDesktopFrameworkTarget` managed contract.

## Managed/native boundary

`Doroti.Vendor.Avalonia.Native` owns the managed P/Invoke declarations, the AppKit service adapters, the generated C ABI header, and the Objective-C++ implementation. The selected Avalonia revision is `f159423f691946e713f454447a780d4677d8a0d2`; source mapping, local adaptation hashes, dependency closure, and license identity are pinned in `migration/avalonia-shell/g7-macos-source-port-provenance.json`.

The native layer owns:

- `NSApplication` pumping and AppKit main-thread dispatch;
- `NSWindow`/`NSView` creation, resize, minimize/restore, focus, scale, close, and screen metrics;
- pointer hover/drag/capture cancellation, precise fractional wheel deltas, key events, and `NSTextInputClient` composition;
- `NSPasteboard`, `NSCursor`, caret geometry, and the `NSAccessibility` action bridge;
- arm64 NSOpenGL context creation, thread affinity, framebuffer presentation, and native resource counters.

The UI thread owns AppKit lifecycle and event delivery. GPU work uses the raster thread only after the native context has been created through the AppKit-safe path. Terminal frame success is recorded only after GPU present; a queued or rasterized frame is not counted as presented.

## Packaging

`Doroti.Target.macOS.osx-arm64` is the composition package. Its dependency graph carries `Doroti.Vendor.Avalonia.Native`, whose NuGet package contains:

- `runtimes/osx-arm64/native/libAvalonia.dylib` with the `@rpath/libAvalonia.dylib` install name;
- the generated C ABI header and reviewed Objective-C++ source;
- applicable Avalonia and repository third-party notices.

The target package also includes its target manifest and G7-3M provenance record. The package validator restores an external consumer outside the repository with an isolated NuGet package cache, preventing a same-version prerelease package from being satisfied by stale global cache content. The published native asset hash must match the build-shard hash.

## Retained verification evidence

The committed aggregate is `migration/macos/g7-macos-shell-evidence.json`. The passing closure requires zero Avalonia UI/Control/Composition binary dependencies, repository-private fallbacks, CPU full-frame fallbacks, and unhandled exceptions. Live evidence records a non-empty Apple M1 strict-GPU frame, terminal ACKs for every submitted generated-app frame, target-causal input/text/clipboard/accessibility traces, focus and lifecycle transitions, and balanced window/OpenGL resources after shutdown.

Automated input of Korean text proves the managed/native text-state path but not physical Korean IME candidate-window placement. Physical VoiceOver navigation, precise trackpad gestures, and `osx-x64` remain `notVerified` and are not inferred from the `osx-arm64` result.
