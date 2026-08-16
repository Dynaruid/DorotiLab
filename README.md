# Doroti

**English** | [한국어](README.ko.md)

### A cross-platform UI runtime built with C# and .NET.

**Doroti** is an experimental cross-platform UI project for bringing a single C# UI codebase to the web, desktop, and mobile. It ports the structure and behavior of the Flutter framework to .NET. Its Windows and macOS desktop shells use selected, provenance-pinned **Avalonia platform source** to connect native windows and operating-system services without composing the application from Avalonia controls.

Doroti translates the semantics of the Dart-based Flutter framework into C#, making familiar widgets such as `MaterialApp`, `Scaffold`, `Text`, and `Button` available across different runtime environments. The target platforms are **Web, Windows, Linux, macOS, Android, and iOS**.

> Doroti does not run Flutter apps inside a WebView, nor does it provide a separate set of Flutter-like UI components.  
> It ports the structure and behavior of the Flutter framework to C# and the .NET runtime as faithfully as possible.

## What works today

Native product validation runs on Windows x64 and Apple Silicon macOS. The same C# `DorotiDemoApp` also builds and publishes for Blazor WebAssembly `browser-wasm`, where it has been confirmed on an actual Chromium GPU canvas.

The current demo app brings the following pieces together:

- `MaterialApp`, `Theme`, `Scaffold`, and `AppBar`
- `Card`, `ListTile`, `Text`, and `Icon`
- Buttons, FABs, checkboxes, radio buttons, switches, and sliders
- `Row`, `Column`, `Stack`, scroll views, and lazy lists
- Pointer interaction, state updates, and semantics
- Selectively ported Avalonia Win32 and AppKit/libAvalonia window, dispatcher, input, text, clipboard, cursor, and accessibility implementations
- Strict GPU rendering with Skia over WGL/OpenGL on Windows and NSOpenGL on `osx-arm64`
- A Blazor WebAssembly host, SkiaSharp WebGL2 surface, separated logical/physical DPR sizing, and bounded backdrop blur on the Web

In addition to `MaterialApp.builder`, the application startup path through `MaterialApp.home` and Navigator is validated against actual native frames.

## Why build it?

Flutter offers polished widgets, a consistent rendering model, and a broad UI package ecosystem. .NET offers powerful languages and tooling, a rich library ecosystem, and runtime environments spanning the web, desktop, and mobile.

Doroti began with a slightly unconventional but exciting question that connects these two technologies:

> **Can the Flutter framework actually run on .NET?**

Instead of building something that merely resembles Flutter, Doroti treats the upstream Flutter source as the behavioral reference. To make that possible, the semantic compiler, runtime, rendering pipeline, and native host are developed together.

## How the platform shell is structured

The platform shell is the layer that connects the Doroti runtime to the operating system. It owns the window lifecycle, dispatcher, pointer and keyboard input, IME, clipboard, cursor, accessibility, and rendering surface. The C# port of the Flutter framework and the Doroti runtime keep ownership of the widget tree, layout, painting, and state lifecycle.

The desktop product shells sit on shared `Doroti.Shell.Core` capabilities. `Doroti.Host.Desktop` receives an `IShellWindowingPlatform`; the `win-x64` and `osx-arm64` composition roots inject their Win32 or AppKit implementation. Selected upstream source and local adaptations are tracked with provenance manifests. Doroti applications are not composed from Avalonia `Control` objects or XAML.

A separate host based on the official `Avalonia.Desktop` package is also maintained. It is not the default product shell; it serves as an A/B reference for comparing the source-port host and validating rendering, input, and window lifecycle behavior.

## How it works

```text
Flutter source
      ↓ semantic compilation
C# framework packages
      ↓
Doroti runtime + widget/rendering pipeline
      ↓
platform host + rendering surface
(Windows: MAUI/WinUI 3/SKSwapChainPanel · macOS: MAUI Mac Catalyst/SKMetalView · Web: Blazor WASM/WebGL2)
      ↓
Web / Windows / Linux / macOS / Android / iOS
```

- The **semantic compiler** analyzes Dart and Flutter types and language semantics, then translates them into C#.
- The **Doroti runtime** connects Flutter's scheduler, widget, element, and rendering lifecycles.
- The **platform host** connects each target's window or view, input, accessibility, and rendering surface. The Windows and macOS hosts use target-specific source ports behind the same typed shell boundary, while the Web host provides the canvas and browser capability bridge.
- The output is reviewable C# source code and .NET packages.

Rather than quietly patching generated code, the project fixes shared semantics in the compiler and runtime, then regenerates the output.

## Try it

The demo is one `DorotiDemoApp.csproj` whose target selector builds **MAUI Windows x64**, **MAUI Mac Catalyst arm64**, or **Blazor WebAssembly `browser-wasm`**. These workflows require .NET SDK 10 and PowerShell 7.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
dotnet run --project ./DorotiDemoApp/DorotiDemoApp.csproj -p:DorotiTarget=Windows
```

Publish the Web app with the following command. Its deployment root is `publish/doroti-demo-web/wwwroot`.

```powershell
dotnet publish ./DorotiDemoApp/DorotiDemoApp.csproj -c Release -p:DorotiTarget=Web -p:RuntimeIdentifier=browser-wasm -o ./publish/doroti-demo-web
```

A manual Chromium smoke of the official publish artifact confirmed a non-empty GPU canvas, Flutter-style DPR sizing, bounded `sigmaX=12`/`sigmaY=6` backdrop blur, the same two-pass shadow model as desktop, a semantics tree, a pointer-driven state change, and zero console errors.

> The new Windows MAUI host has build/publish and automated GPU presentation evidence. Web build/publish and the earlier manual `presented`/basic-pointer smoke are confirmed. Native hover/wheel/keyboard/IME/UIA, Mac Catalyst native execution, physical acceptance, and cross-target parity remain `notVerified`; predecessor Win32/AppKit evidence is not transferred to MAUI.

## Repository layout

| Path | Description |
| --- | --- |
| [`Doroti/`](Doroti/) | Compiler-generated framework, runtime, renderer, and platform hosts |
| [`Doroti/src/Doroti.Host.Maui/`](Doroti/src/Doroti.Host.Maui/) | Shared MAUI lifecycle and externally owned Skia GPU-surface adapter |
| [`Doroti/src/Doroti.App.Sdk/`](Doroti/src/Doroti.App.Sdk/) | Single-project Windows/Mac Catalyst/Web target selection SDK |
| [`Doroti/src/Doroti.Host.Avalonia/`](Doroti/src/Doroti.Host.Avalonia/) | Comparison and validation host based on the official `Avalonia.Desktop` package |
| [`DorotiDemoApp/`](DorotiDemoApp/) | Demo app that displays a real Material widget tree |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | Semantic compiler that translates Dart into C# |
| `reference/` | Local, ignored pinned source trees: `reference/flutter-master` and `reference/Avalonia-main` |
| [`history/26-08-16/goal7-summary.md`](history/26-08-16/goal7-summary.md) | Archived Goal 7 results, remaining Web/release gates, and evidence boundaries |

For detailed build and validation instructions and architecture records, see [`Doroti/README.md`](Doroti/README.md) and [`Doroti/docs/`](Doroti/docs/).

## Roadmap

The goal is not merely to make “Flutter files compile as C#.” It is to build a cross-platform UI runtime where real applications can navigate, accept input, scroll, and integrate with accessibility tools.

Planned work includes closing automated and physical Web release acceptance, navigation and dialogs, forms and physical IME acceptance, large-scale scrolling, assets and localization, and additional Material and Cupertino components. Building on the current Windows, Apple Silicon macOS, and Web implementations, support will expand to **Linux, Intel macOS, Android, and iOS**.

Doroti is still an experimental project under active development. If you are interested in the intersection of compilers, runtimes, rendering, and UI frameworks, follow along as the project evolves.

## Contributions and forks

Doroti is a personal hobby project built for fun, so I may not be able to actively review or merge pull requests. Ideas and feedback about the project are always welcome.

You are also welcome to fork Doroti, experiment with it, and take it in new directions. Explore it in your own way and see what interesting possibilities you can create.

## Related projects

Doroti draws heavily from the source code and design of the following projects:

- [Flutter](https://github.com/flutter/flutter) — the reference for framework structure and behavior
- [Avalonia](https://github.com/AvaloniaUI/Avalonia) — the source foundation for selected Windows and macOS desktop platform implementations

## License

See [`LICENSE`](LICENSE) for Doroti's license and [`Doroti/THIRD-PARTY-NOTICES.md`](Doroti/THIRD-PARTY-NOTICES.md) for notices covering upstream source used by the project.
