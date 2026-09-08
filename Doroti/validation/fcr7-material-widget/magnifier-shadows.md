# Magnifier shadow regression (2026-09-09)

Flutter's Material and Cupertino magnifiers both supply default BoxShadows.
Doroti already preserves those values. Material uses RawMagnifier's negative
clip to exclude the lens interior from the decoration.

`Rect.largest` previously used double extrema, which overflow to infinities in
Skia's float paths. The resulting negative clip suppressed the Material shadow.
Flutter uses finite bounds of -1e9 to +1e9; the shared Rect now matches them.
Cupertino's default outer shadow with Clip.none already worked. Cupertino with
clipping enabled benefits from the same correction.

References in the checked-in Flutter source:

- `engine/src/flutter/lib/ui/geometry.dart`: Rect.largest / _giantScalar.
- `packages/flutter/lib/src/material/magnifier.dart`: default shadow and clip.
- `packages/flutter/lib/src/cupertino/magnifier.dart`: outer shadow and clip.
- `packages/flutter/lib/src/widgets/magnifier.dart`: RawMagnifier / _NegativeClip.

Run from the repository root with the required 20-minute timeout:

```powershell
python -c "import subprocess; subprocess.run(['dotnet','run','--project','Doroti/validation/fcr7-material-widget','--','--magnifier-shadows'],timeout=1200,check=True)"
```

The test mounts production widgets and renders through Skia. It compares default
shadows against explicitly empty shadow lists, checks pixels outside the lens,
checks that the lens interior is unchanged, and checks four repeated renders.
PNG captures are written under `%TEMP%/doroti-magnifier-shadows`.

- Before correction: Material failed with 0 shadow pixels outside the lens.
- After correction: Material PASS (50 pixels); Cupertino PASS (1255 pixels);
  Cupertino with Clip.hardEdge PASS (1255 pixels).
- Related `--mac-text-menu` rendering and interaction contracts PASS.
- Android arm64 Release build: 0 warnings / 0 errors.
- Galaxy SM-S931N: installed the rebuilt signed Release APK with `adb install -r
  --user 0`, launched Material 3 sample mode, entered text in the Filled field,
  and captured a long press. The magnifier's lower shadow is visible on-device.
  Capture: `.doroti/evidence/magnifier-shadows-20260909/galaxy-magnifier.png`
  (relative to the repository root; ignored local evidence).
- `dotnet run` built the sample-mode APK but its subsequent DeployToDevice stage
  failed with the already documented DOTNET_HOST_PATH / MSB4221 / MSB4027 issue.
  Installing that newly built APK directly with ADB succeeded.

These software raster checks do not establish iOS physical-device appearance
or pixel parity with Flutter on a physical device.
