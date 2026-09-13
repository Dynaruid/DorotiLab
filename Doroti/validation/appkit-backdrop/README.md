# AppKit window backdrop validation

This fixture references the production Host.Maui Metal view and switches between
transparent, acrylic, Liquid Glass, solid, system, and acrylic again. A striped native
window behind it makes blur and glass visible; the Metal surface draws a magenta marker
over a transparent clear. It checks real completed GPU frames, native effect types,
resize alignment, input pass-through, duplicate removal and restoration of window state
on disconnect, then waits for GPU resource retirement. Each stage captures the entire
main display (`*-fullscreen.png`) to include the windows sampled by native effects.
Window-only captures are retained for comparison, but cannot validate desktop blending.

Run from the repository root on a logged-in macOS desktop:

```sh
dotnet /usr/local/share/dotnet/sdk/10.0.400/dotnet.dll build \
  Doroti/validation/appkit-backdrop/Doroti.Validation.AppKitBackdrop.csproj \
  -r osx-arm64 -m:1 -p:UseSharedCompilation=false

DOROTI_BACKDROP_EVIDENCE=/tmp/doroti-backdrop-graphite MTL_DEBUG_LAYER=1 \
  'Doroti/artifacts/validation/build/appkit-backdrop/bin/Debug/net10.0-macos/osx-arm64/Doroti AppKit Backdrop.app/Contents/MacOS/Doroti.Validation.AppKitBackdrop'
```

Repeat with `DOROTI_MACOS_GRAPHITE=0` and a different output directory to check Ganesh.
The fixture requires screen capture access. `result.json` reports assertions and backend;
inspect the PNGs to assess visual output. On macOS 14/15 the Liquid Glass stage expects
the blur fallback. Running on macOS 26 does not exercise that older-OS runtime branch.
System accessibility adaptations and physical input remain separate manual checks.

## Recorded run — 2026-09-13

macOS 26.6.2, Xcode 26.6, .NET SDK 10.0.400:

| Check | Result |
| --- | --- |
| AppKit testbed and fixture build | Passed, zero warnings/errors |
| Graphite / Metal | All six stages passed; 74 completed buffers; GPU resources released |
| Ganesh / Metal | All six stages passed; 77 completed buffers; GPU resources released |
| Full-display visual review | Blur diffuses the broad colored bands; Liquid Glass reveals them with visible edge refraction; Metal marker stays opaque |
| Material sample, each effect | Graphite frames presented with zero reported failures |

The run's JSON and full-display screenshots are in
`Doroti/artifacts/validation/appkit-backdrop/{graphite,ganesh,product}/`.
The older-macOS fallback was not executed on this macOS 26 machine.
