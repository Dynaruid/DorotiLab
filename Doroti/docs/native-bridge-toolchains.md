# Native bridge toolchains and provenance

The default bridge is application source, not a checked-in native binary. Generated AAR, framework, XCFramework, Gradle `build`/`.gradle`, Xcode DerivedData, and `xcuserdata` remain build output.

| Platform | Build input | Pinned contract | Dependency provenance |
|---|---|---|---|
| Android | `android/native/build.gradle.kts` | Gradle 8.10.2, AGP 8.6.1, Java 17, compile SDK 34, min SDK 21 | Google/Maven Central repositories declared by the app-owned Gradle project; no third-party runtime dependency in the default bridge |
| iOS | `ios/native/*.xcodeproj` | Xcode 16 contract, Swift 5, deployment target 15 | Foundation/UIKit from the selected Apple SDK; app-added SPM packages belong to the Xcode project and must add license/provenance here |
| Mac Catalyst | `macos/native/*.xcodeproj` | Xcode 16 contract, Swift 5, deployment target 15 | Foundation/UIKit for Mac Catalyst; not AppKit macOS |

The Android wrapper JAR SHA-256 is `e996d452d2645e70c01c11143ca2d3742734a28da2bf61f25c82bdc288c9e637`. The machine-readable ABI/toolchain record is `validation/contracts/native-platform-bridge.json`.

Java 17 is the Android source and bytecode compatibility level. The 2026-08-19 Windows validation used the .NET Android OpenJDK 21.0.8 host; `native doctor` accepts non-GraalVM OpenJDK 17 through 21 and reports the selected build JDK separately from the source level.
