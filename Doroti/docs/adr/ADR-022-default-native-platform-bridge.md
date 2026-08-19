# ADR-022: Default native platform bridge

- Status: Accepted
- Date: 2026-08-19

## Decision

Every generated Doroti application owns three native library workspaces by default: an Android Gradle library, an iOS Xcode framework, and an independent Mac Catalyst Xcode framework. Each native project is consumed by one platform-specific .NET binding project, and each binding is referenced by exactly one final .NET runner through `DorotiNativeBindingProject`.

The final application process remains owned by the .NET runner. `AndroidGradleProject` produces the AAR consumed by the Android binding; `XcodeProject` produces the framework/XCFramework consumed by the Apple binding. Neither build item is a replacement application runner, and reverse runtime embedding is outside this decision.

The shared application sees only `IDorotiNativePlatformBridge`. Generated Java/Objective-C binding types stay inside the runner adapter. The default ABI is `doroti.native-platform-bridge/v1` with `platformInfo`, `echo`, and `echoOnUiThread`; callbacks are marshalled by the native library to the platform UI thread and exceptions are not swallowed.

## Ownership and isolation

- `android/native` and `android/binding` are Android-only; Gradle output is under binding `obj/<rid>/native/android/<configuration>`.
- `ios/native` and `ios/binding` are iOS-only.
- `macos/native` and `macos/binding` are Mac Catalyst-only and do not claim AppKit support.
- Apple output is under binding `obj/<rid>/<configuration>/native/xcode` when Xcode executes.
- The root `net10.0` app cannot reference platform binding assemblies or SDK types.
- A missing, escaping, duplicated, or wrong-target binding fails before native compilation with a `DOROTIRUNNER3xx` diagnostic.

## Evidence boundary

Managed/cross-build success proves the project/reference/ABI graph only. Android Studio sync, emulator/device execution, Xcode framework builds, simulator/device execution, signing, archive, accessibility, and store acceptance are recorded independently. A Windows Apple cross-build never becomes Xcode or native-live evidence.

## Consequences

New projects contain ten `.csproj` files: one neutral app, six runners, and three native bindings. Native toolchain work is incremental and runner-local. The former optional `scaffold-interop` path is obsolete; `doroti native doctor|build|open|add` operates on the default workspace.
