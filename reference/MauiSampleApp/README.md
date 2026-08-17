# Doroti MAUI Native Library Interop OCR sample

This fixture follows the [Maui.NativeLibraryInterop](https://github.com/CommunityToolkit/Maui.NativeLibraryInterop) layout: a slim native wrapper, a .NET binding project per platform, and a C# Markup MAUI app that consumes those bindings.

```text
reference/MauiSampleApp/
  android/
    DorotiOcr.Android.Binding/     # .NET for Android binding
    native/dorotiocr/              # Java wrapper + Gradle Maven deps
  macios/
    DorotiOcr.MaciOS.Binding/      # .NET for iOS / Mac Catalyst binding
    native/DorotiOcr/              # Swift wrapper around Vision
  sample/                          # .NET MAUI OCR app
```

## Android (Maven)

The Java wrapper depends on bundled on-device ML Kit from Google Maven:

- `com.google.mlkit:text-recognition:16.0.1` (Latin)
- `com.google.mlkit:text-recognition-korean:16.0.1` (Korean)

`@(AndroidGradleProject)` builds that Gradle module. The MAUI app also declares the same artifacts with `@(AndroidMavenLibrary)` so .NET for Android downloads and packages the AARs instead of relying on a NuGet ML Kit wrapper.

## iOS / Mac Catalyst (Vision)

The Swift wrapper exposes one Objective-C-visible API over `VNRecognizeTextRequest`. `@(XcodeProject)` builds `DorotiOcr.framework` for iPhone OS and the iOS Simulator, with Mac Catalyst enabled. The .NET binding project targets both `net10.0-ios` and `net10.0-maccatalyst`.

Camera and photo-library usage strings live in `Platforms/iOS/Info.plist` and `Platforms/MacCatalyst/Info.plist`. Mac Catalyst also declares camera and user-selected file entitlements.

## Other platforms

- Windows: `Windows.Media.Ocr` in `Platforms/Windows` (no native binding project)

UI stays C#. The only XAML file is the Windows WinUI bootstrap `App.xaml`.

## Build

From the repo root. JDK 17 is required for the Android Gradle wrapper. The Mac Catalyst / iOS binding needs Xcode and is selected only when building on macOS.

`sample/packages.lock.json` follows the host OS target list. macOS records `net10.0-android`, `net10.0-ios`, and `net10.0-maccatalyst`. Windows records `net10.0-android` and `net10.0-windows10.0.19041.0`; omit `--locked-mode` there if NuGet reports NU1004.

```powershell
dotnet restore .\reference\MauiSampleApp\sample\MauiSampleApp.csproj --locked-mode
dotnet build .\reference\MauiSampleApp\android\DorotiOcr.Android.Binding\DorotiOcr.Android.Binding.csproj -c Debug
dotnet build .\reference\MauiSampleApp\sample\MauiSampleApp.csproj -f net10.0-android -c Debug
dotnet build .\reference\MauiSampleApp\sample\MauiSampleApp.csproj -f net10.0-windows10.0.19041.0 -c Debug
```

On macOS:

```bash
dotnet restore ./reference/MauiSampleApp/sample/MauiSampleApp.csproj --locked-mode
dotnet build ./reference/MauiSampleApp/macios/DorotiOcr.MaciOS.Binding/DorotiOcr.MaciOS.Binding.csproj -f net10.0-ios -c Debug
dotnet build ./reference/MauiSampleApp/sample/MauiSampleApp.csproj -f net10.0-ios -c Debug -r iossimulator-arm64
dotnet build ./reference/MauiSampleApp/sample/MauiSampleApp.csproj -f net10.0-maccatalyst -c Debug
```

On a device or emulator, tap **Sample** then **Recognize text**. Gallery and camera pickers are also wired up.
