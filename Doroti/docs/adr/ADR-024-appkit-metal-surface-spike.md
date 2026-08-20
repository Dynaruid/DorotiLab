# ADR-024: AppKit-owned Metal surface in a permanent dual macOS backend

- Status: accepted and productized
- Date: 2026-08-20

## Decision

Doroti will add a native AppKit macOS product alongside its permanent Mac
Catalyst product. The `macos` alias selects only AppKit and the `maccatalyst`
alias selects only Mac Catalyst; neither backend falls back to the other.

The AppKit product will use the experimental
`Microsoft.Maui.Platforms.MacOS` AppKit backend, exact-pinned at
`0.1.0-preview.12.26368.2`, behind Doroti-owned adapters. The product surface
will be an `MTKView` that owns its Metal device, command queue, Skia Metal
context, drawable render target, and presentation command buffer.

The AppKit path will not use `SkiaSharp.Views.Maui.Controls.SKGLView` or the
stock `SkiaSharp.Views.Mac.SKMetalView`. The stock view commits presentation
after its public paint callback and does not expose command-buffer completion,
so it cannot implement Doroti's terminal present acknowledgement contract.

`presented` is emitted only from `IMTLCommandBuffer.AddCompletedHandler` after
the buffer reaches `Completed`. A completion from an obsolete surface
generation is rejected and cannot acknowledge a newer frame. The surface
performs no software fallback, CPU readback, or full-frame copy.

## Evidence

The disposable runner at `validation/appkit-metal-spike` uses the public
`MacOSViewHandler<TVirtualView,TPlatformView>` API and renders a retained scene
through `Doroti.Skia.Rendering`. Its automated live run performed 20 window
resizes plus minimize/restore and hide/unhide. The recorded run committed and
completed 39 Metal buffers with one new presentation, 38 retained replays, no
errors, no stale completions, and no CPU fallback path.

The dependency graph resolved MAUI 10.0.90 together with the backend's minimum
10.0.41 dependency and registered CommunityToolkit.Maui.Markup 8.0.0 at
runtime. Selected compile/runtime assets contain no Mac Catalyst target.

The product runner was then launched from a clean Debug bundle. The visible
Material gallery committed and completed four Metal buffers (two new frames
and two retained replays), with zero failures, stale completions, CPU
readbacks, or full-frame copies. Bundle inspection found an arm64 macOS
executable, AppKit linkage, only the AppKit native bridge framework, and no
UIKit/iOSSupport linkage or UIKit plist keys. The Swift bridge passed
`platformInfo`, `echo`, and a main-thread-checked async echo. The compact record
is `validation/evidence/appkit-macos/product-live.json`.

## Consequences

- AppKit and Mac Catalyst remain independent first-class products with separate
  runners, target packages, native bridges, templates, artifacts, and evidence.
- Neither backend is a temporary compatibility path. Removing or deprecating
  either requires a separate user decision, ADR, migration release, and
  deprecation period.
- A build or runtime failure in one backend is reported directly and never
  replaced by the other backend's artifact or pass evidence.
- Backend internal types and reflection are outside the accepted design.
- The backend is experimental and unsupported; upgrades require a new exact
  pin and a repeat of this live gate.
- Framework-dependent publish is currently not a valid gate: the .NET macOS
  SDK requires trimming, while ILLink rejects trimming framework-dependent
  output with `NETSDK1102`. This remains explicitly blocked rather than pass.

## Product implementation

The accepted surface now lives in `Doroti.Host.Maui` behind `IMauiSkiaSurface`.
`MauiSkglSurface` preserves the Windows/iOS/Android/Mac Catalyst path, while
`DorotiMacOSMetalSurface` and `DorotiMacOSMetalView` own AppKit/MTKView/Metal.
The product graph exposes `Doroti.Target.MacOS.Maui.osx-arm64`, an
`AppKit-Main` bootstrap, `DorotiDemoApp.MacOS`, the `macos` workspace alias,
and a separate `maccatalyst` alias. Build evidence does not promote unrun IME,
VoiceOver, physical-input, signing, or notarization gates beyond
`notVerified`.
