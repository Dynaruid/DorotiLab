# iPhone trimming experiment — 2026-09-10

The guarded full-trim build was installed on the iPhone 12 and rendered the
Material sample, diagnostics gallery and MediaQuery fixture without recorded
frame failures. Its local app bundle is **105.0 MB**, down **22.8%** from 136.0 MB.
Full trimming remains an opt-in experiment. The unguarded build installed on the
iPhone 12, but failed to construct the Material sample: DLR calls in
`LocalizationsLibrary._loadAll` and `_RawViewElement__view.mount` raised
`RuntimeBinderException: NoSuchMember`. Build/install success alone is not a pass.

## Size measurements

Sizes below are the sum of regular file lengths inside the local, signed
`ios-arm64` Release `.app`, in decimal MB. They are not iOS Settings storage
measurements or compressed IPA/download sizes.

| Build | App bytes | Executable bytes | Device outcome |
| --- | ---: | ---: | --- |
| Existing partial-trim Release baseline | 136,038,224 | 88,303,792 | Existing installed build; backed up before the experiment |
| Full trim, JSON source-generation fixes, no dynamic-member descriptor | 96,677,312 | 58,291,376 | Installed; Material startup failed with missing dynamic members |
| Full trim with dynamic-member preservation and compatibility fixes | 105,013,528 | 63,839,616 | Installed; all three screen modes rendered, zero recorded frame failures |

The unguarded result saves 39,360,912 bytes (28.9%), but is not a usable deployment.
The baseline predates the compatibility fixes, so this is an experiment comparison,
not a controlled benchmark of only one MSBuild property. The guarded build saves
31,024,696 bytes while retaining the dynamic members needed by the tested screens.

## Device execution

Environment: .NET SDK 10.0.400, .NET/ILLink 10.0.11, Microsoft.iOS SDK
26.5.10315, `net10.0-ios`, `ios-arm64`, iPhone 12. Evidence reports a 1170×2532
surface at DPR 3 and the `UIKit/MTKView/Graphite-Metal` backend.

| Launch mode | Presented | Replayed | Failed | Evidence |
| --- | ---: | ---: | ---: | --- |
| Material sample | 2 | 1 | 0 | `guarded-sample-evidence.json` |
| Diagnostics | 3 | 2 | 0 | `guarded-diagnostics-evidence.json` |
| MediaQuery | 4 | 2 | 0 | `guarded-media-query-evidence.json`; console also printed the expected metrics JSON |

Each mode was launched as a new process using `--terminate-existing`, with console
output captured. The Material sample was launched again after the other checks
and left open for interaction. The evidence files were copied from the app's
`Documents/doroti-maui-evidence.json` through `devicectl`. Initial snapshots had
zero pointer events, so these measurements establish startup/rendering, not a
complete manual interaction or performance pass. The console still contains
existing MAUI observer-disposal and background-fetch configuration warnings.
The final Material relaunch also recorded 2 presented frames, 1 replay and 0
failures. No exception evidence file was present in Documents, and none of the
guarded launch logs contained the original binder exceptions.

The guarded build completed with 1,691 warnings, primarily dynamic-binding trim
diagnostics. `codesign --verify --deep --strict` passed for the installed candidate.
Executable SHA-256:
`3d564b0448a8452056d7443ea3e36c765f59cff6b3037955d1f11551b50560d9`.

## Compatibility changes

- Use generated JSON serializers for the native bridge, MAUI evidence,
  Skia font-fallback cache keys, and MediaQuery diagnostics. Preserve the existing
  bridge JSON property casing and response shapes.
- Encode Dart JSON channel values directly with `Utf8JsonWriter`. Nested maps,
  lists, strings, booleans, nulls and numbers no longer depend on runtime collection
  metadata. Keys must be strings; arbitrary CLR objects must first be converted to
  JSON values (for example with the existing `jsonEncode` callback). This codec
  does not automatically reflect arbitrary CLR properties.
- Connect LayoutBuilder children through `IRenderObjectWithChild` rather than
  looking up the `child` property through reflection.
- When `TrimMode=full`, the iOS runner includes `ios/TrimmerRoots.xml`. It preserves
  members of retained Doroti/application types, using `required="false"` so
  entirely unused types can still be removed. This deliberately trades some size
  savings for DLR compatibility; it is not a general proof that every dynamic
  target in every app is preserved.
- `DOROTI_MAUI_EVIDENCE=1` writes `doroti-maui-evidence.json` into the app's local
  application-data directory, and failures into the adjacent `.exception.txt`.
  Explicit evidence paths are also supported.

## Reproduce

From the repository root, with a provisioned iPhone connected and unlocked:

```sh
dotnet build DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj \
  -c Release -r ios-arm64 \
  -p:TrimMode=full -p:DebugType=None -p:DebugSymbols=false \
  -p:TrimmerSingleWarn=false -p:ILLinkTreatWarningsAsErrors=false

xcrun devicectl list devices
xcrun devicectl device install app --device DEVICE_ID \
  DorotiTestbedApp/ios/bin/ios-arm64/Release/net10.0-ios/ios-arm64/DorotiTestbedApp.iOS.app
xcrun devicectl device process launch --device DEVICE_ID \
  --terminate-existing --console \
  --environment-variables '{"DOROTI_TESTBED_MODE":"sample","DOROTI_RESIZE_FIXTURE":"none","DOROTI_MAUI_EVIDENCE":"1"}' \
  dev.doroti.testbed
```

The .NET 10.0.11 trimmer crashed in `MessageOrigin.ToString()` while formatting
source locations (`Sequence contains no elements`). Grouping warnings did not
avoid it. Passing `DebugType=None` and `DebugSymbols=false` **globally**, including
project references, allowed the experiment to complete. These flags remove managed
source debugging information; they do not disable trimming analysis or warnings.
`ILLinkTreatWarningsAsErrors=false` is explicitly limited to this experiment command.
Dynamic binding warnings remain visible and unresolved.

To reproduce the unsafe unguarded comparison, additionally pass
`-p:DorotiTrimPreserveDynamicMembers=false`. To return to partial trimming, build
with `-p:TrimMode=partial` and reinstall the resulting app. Keep Mono's existing
`MtouchInterpreter=-all` setting: the framework still needs DLR-generated methods.

## Regression checks

- Application descriptor/native bridge contract: PASS, including platformInfo
  property casing and UI-thread echo with Unicode and escaped quotes.
- FCR-7 `--scaffold-metrics`: PASS, including LayoutBuilder constraints,
  keyboard insets, padding restoration and focused caret reveal.
- The new `validation/json-codec` executable was published self-contained for
  `osx-arm64` with full trimming, then executed: PASS for channel payloads,
  Unicode, numeric precision, nested round-trip, unsupported inputs and cycles.
  Its publish command used `EnableTrimAnalyzer=false` to bypass unrelated
  library-wide compile-time reflection diagnostics; ILLink itself still ran
  with its normal warnings-as-errors behavior and reported zero warnings.

Local build logs, signed baseline/unguarded bundles and file-size inventories are
under `/tmp/doroti-ios-trimming/` on the development Mac. They are temporary evidence,
not repository assets. No production trim-safety or performance claim is made.

References: [MAUI trimming](https://learn.microsoft.com/en-us/dotnet/maui/deployment/trimming?view=net-maui-10.0),
[trimmer source-location formatting](https://github.com/dotnet/runtime/blob/v10.0.0/src/tools/illink/src/linker/Linker/MessageOrigin.cs).
