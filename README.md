# Doroti

**English** | [한국어](README.ko.md)

### A cross-platform UI runtime built with C# and .NET.

**Doroti** is an experimental cross-platform UI project for bringing a single C# UI codebase to the web, desktop, and mobile. It ports the structure and behavior of the Flutter framework to .NET. On Windows desktop, it uses **Avalonia's proven platform implementation as its foundation** to connect native windows and operating system services.

Doroti translates the semantics of the Dart-based Flutter framework into C#, making familiar widgets such as `MaterialApp`, `Scaffold`, `Text`, and `Button` available across different runtime environments. The target platforms are **Web, Windows, Linux, macOS, Android, and iOS**.

> Doroti does not run Flutter apps inside a WebView, nor does it provide a separate set of Flutter-like UI components.  
> It ports the structure and behavior of the Flutter framework to C# and the .NET runtime as faithfully as possible.

## What works today

The first product validation on the path to cross-platform support is taking place on Windows desktop. A Flutter widget tree is created in C#, mounted, laid out, painted, and displayed in a real native window.

The current demo app brings the following pieces together:

- `MaterialApp`, `Theme`, `Scaffold`, and `AppBar`
- `Card`, `ListTile`, `Text`, and `Icon`
- Buttons, FABs, checkboxes, radio buttons, switches, and sliders
- `Row`, `Column`, `Stack`, scroll views, and lazy lists
- Pointer interaction, state updates, and semantics
- Selectively ported Avalonia Win32 window, dispatcher, input, IME, clipboard, and cursor lifecycle implementations
- Strict GPU rendering with Skia over WGL/OpenGL

In addition to `MaterialApp.builder`, the application startup path through `MaterialApp.home` and Navigator is validated against actual native frames.

## Why build it?

Flutter offers polished widgets, a consistent rendering model, and a broad UI package ecosystem. .NET offers powerful languages and tooling, a rich library ecosystem, and runtime environments spanning the web, desktop, and mobile.

Doroti began with a slightly unconventional but exciting question that connects these two technologies:

> **Can the Flutter framework actually run on .NET?**

Instead of building something that merely resembles Flutter, Doroti treats the upstream Flutter source as the behavioral reference. To make that possible, the semantic compiler, runtime, rendering pipeline, and native host are developed together.

## Where does Avalonia fit?

Avalonia is an important foundation for Doroti's Windows desktop platform host. Selected implementations for the Win32 window lifecycle, dispatcher, pointer and keyboard input, IME, clipboard, and cursor are ported from Avalonia upstream and adapted to Doroti's platform contracts. A provenance manifest tracks their upstream revisions and change history.

Doroti applications are not composed from Avalonia `Control` objects or XAML. The C# port of the Flutter framework and the Doroti runtime own the widget tree, layout, painting, and state lifecycle. The platform implementations sourced from Avalonia connect that output to native windows and operating system services.

A separate host based on the official `Avalonia.Desktop` package is also maintained. It is not the primary product host; it serves as an A/B reference for comparing the source-port host and validating rendering, input, and window lifecycle behavior.

## How it works

```text
Flutter source
      ↓ semantic compilation
C# framework packages
      ↓
Doroti runtime + widget/rendering pipeline
      ↓
platform host + rendering surface
(Windows: Avalonia-derived platform source port)
      ↓
Web / Windows / Linux / macOS / Android / iOS
```

- The **semantic compiler** analyzes Dart and Flutter types and language semantics, then translates them into C#.
- The **Doroti runtime** connects Flutter's scheduler, widget, element, and rendering lifecycles.
- The **platform host** connects each target's window or view, input, accessibility, and rendering surface. The current Windows host uses selected platform source ported from Avalonia.
- The output is reviewable C# source code and .NET packages.

Rather than quietly patching generated code, the project fixes shared semantics in the compiler and runtime, then regenerates the output.

## Try it

The currently available demo and live validation run on **Windows x64**. They require .NET SDK 10 and PowerShell 7.

```powershell
pwsh -File ./Doroti/eng/doroti.ps1 build
dotnet run --project ./DorotiDemoApp
```

A short automated smoke run is also available:

```powershell
dotnet run --project ./DorotiDemoApp -- --smoke --entry builder --frames 3 --duration-ms 15000
```

> Windows x64 is the starting point for the current implementation and validation, not the full extent of Doroti's intended platform support. Web, Linux, macOS, Android, and iOS will be implemented and validated in later stages.

## Repository layout

| Path | Description |
| --- | --- |
| [`Doroti/`](Doroti/) | Compiler-generated framework, runtime, renderer, and platform hosts |
| [`Doroti/src/Doroti.Host.Desktop/`](Doroti/src/Doroti.Host.Desktop/) | Primary Windows host using the Avalonia-derived Win32 source port |
| [`Doroti/src/Doroti.Host.Avalonia/`](Doroti/src/Doroti.Host.Avalonia/) | Comparison and validation host based on the official `Avalonia.Desktop` package |
| [`DorotiDemoApp/`](DorotiDemoApp/) | Demo app that displays a real Material widget tree |
| [`tools/Doroti.DartToCSharp/`](tools/Doroti.DartToCSharp/) | Semantic compiler that translates Dart into C# |
| [`goal6.md`](goal6.md) | Roadmap covering the current development status and next steps |

For detailed build and validation instructions and architecture records, see [`Doroti/README.md`](Doroti/README.md) and [`Doroti/docs/`](Doroti/docs/).

## Roadmap

The goal is not merely to make “Flutter files compile as C#.” It is to build a cross-platform UI runtime where real applications can navigate, accept input, scroll, and integrate with accessibility tools.

Planned work includes navigation and dialogs, forms and IME, large-scale scrolling, assets and localization, and additional Material and Cupertino components. Building on the current Windows validation, support will expand to **Web, Linux, macOS, Android, and iOS**.

Doroti is still an experimental project under active development. If you are interested in the intersection of compilers, runtimes, rendering, and UI frameworks, follow along as the project evolves.

## Contributions and forks

Doroti is a personal hobby project built for fun, so I may not be able to actively review or merge pull requests. Ideas and feedback about the project are always welcome.

You are also welcome to fork Doroti, experiment with it, and take it in new directions. Explore it in your own way and see what interesting possibilities you can create.

## Related projects

Doroti draws heavily from the source code and design of the following projects:

- [Flutter](https://github.com/flutter/flutter) — the reference for framework structure and behavior
- [Avalonia](https://github.com/AvaloniaUI/Avalonia) — the foundation of the Windows desktop platform implementation

## License

See [`LICENSE`](LICENSE) for Doroti's license and [`Doroti/THIRD-PARTY-NOTICES.md`](Doroti/THIRD-PARTY-NOTICES.md) for notices covering upstream source used by the project.
