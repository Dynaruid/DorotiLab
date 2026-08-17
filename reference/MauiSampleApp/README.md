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

## Other platforms

- iOS / Mac Catalyst: slim Swift API over `VNRecognizeTextRequest`
- Windows: `Windows.Media.Ocr` in `Platforms/Windows` (no native binding project)

UI stays C#. The only XAML file is the Windows WinUI bootstrap `App.xaml`.

## Build

From the repo root. JDK 17 is required for the Android Gradle wrapper.

```powershell
dotnet restore .\reference\MauiSampleApp\sample\MauiSampleApp.csproj --locked-mode
dotnet build .\reference\MauiSampleApp\android\DorotiOcr.Android.Binding\DorotiOcr.Android.Binding.csproj -c Debug
dotnet build .\reference\MauiSampleApp\sample\MauiSampleApp.csproj -f net10.0-android -c Debug
dotnet build .\reference\MauiSampleApp\sample\MauiSampleApp.csproj -f net10.0-windows10.0.19041.0 -c Debug
```

The Mac Catalyst / iOS binding needs Xcode and is selected only when building on macOS.

On a device or emulator, tap **Sample** then **Recognize text**. Gallery and camera pickers are also wired up.
