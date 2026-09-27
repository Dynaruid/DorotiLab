# Native framework fonts

Native Windows App SDK, MAUI (Windows/Android/iOS/macOS/Mac Catalyst) and Linux Qt
hosts reference this package and register the bundled Roboto 400/500/700 faces
before their first text layout. OS defaults (Segoe UI, Apple system fonts, etc.)
remain platform-managed. Explicit `Roboto` styles use the bundled family;
application-registered fonts and normal system glyph fallback remain available.

On a source build, `Build/NativeFonts.targets` downloads Flutter's versioned
`3012db47f3130e62f7cc0beabff968a33cbec8d8/fonts.zip` from Google's storage CDN and
extracts just the three faces and their Apache 2.0 license. Every file is checked
against a pinned SHA-256 before an atomic install into `.doroti/fonts/` (ignored
by Git). A file lock serializes simultaneous build restores. Valid cached files
are reused without HTTP requests; corrupt files are fetched again.

The fonts and license are embedded in `Doroti.Skia.Fonts.dll`; the license is
also included in its NuGet package. Runtime registration only reads assembly
resources. There is no native runtime font download and no dependency on the
source cache when using the built application or package.

For a source build that must not access the font CDN:

```powershell
dotnet build -p:DorotiNativeFontsOffline=true
```

The native font cache must already be populated and valid. A missing/corrupt file
fails with `DOROTIFONT001`; it does not silently produce an application without
its default fonts. These settings govern font restoration, not NuGet restore.

Web does not reference this package. Its defaults and missing-glyph fonts load
from CDN at runtime; see `../Doroti.Host.Web/Fonts/README.md`.

Validation (2026-09-28): clean CDN restore, embedded-font registration and metrics,
offline corruption rejection/online repair/cache reuse, and NuGet assembly/license
contents passed. Windows App SDK sample, MAUI Windows, MAUI Android and Qt host
builds passed with zero warnings/errors. Apple compilation and actual native-device
display are not verified by these checks.
