# Image readback and Material color validation

The shared `Picture.toImage` → `Image.toByteData` → `ColorScheme.fromImageProvider`
path is implemented for Skia hosts and the CanvasKit Raster Worker. The validation
library is independent of Testbed. Its optional Web export runs inside the live
UI Worker only in builds made with `-p:DorotiImageValidation=true`.

## Contract

- A picture requires an attached view with an image host capability. Width/height
  must be positive, fit `int`, and fit a tightly packed RGBA byte buffer.
- Rasterization uses the requested pixel size at DPR 1 and transparent clear.
  Native Skia uses an offscreen CPU surface; CanvasKit uses an offscreen target
  on the Raster owner. It does not submit a visible frame or change resize counters.
- `rawRgba` is tightly packed RGBA8, sRGB, premultiplied alpha; `rawStraightRgba`
  is unpremultiplied. `rawUnmodified` is canonical RGBA8/premultiplied on these hosts.
  PNG is encoded. ByteData views preserve their offset/length.
- A pending read retains image storage until completion. Disposed image/picture
  use fails. PNG-backed CanvasKit images use the existing resource journal so
  committed images can be replayed after a Raster restart.
- UI/Raster messages carry transferred buffers with request IDs and the existing
  session envelope. Maximum 16 pending image operations, 30-second timeout;
  port teardown rejects pending calls. PNG/base64 interchange is intended for
  occasional extraction, not per-frame capture. Restart rejection is implemented
  but a forced-restart-during-read acceptance test has not been run.
- `fromImageProvider` extracts the first frame, scales the longest edge to at most
  112 pixels, removes its listener, disposes intermediate images, and forwards
  decode/raster errors. A 30-second load timeout releases late output.
- Quantization returns at most 128 colors for image themes (public runtime limit
  1–256). Wu rounding, 64-bit histogram accumulation, five-iteration Wsmeans,
  deterministic extra centers, and Score follow pinned Dart MCU 0.13.0. As in that
  reference, Wu excludes nonopaque input but Wsmeans counts its RGB values;
  output colors are opaque. Fully transparent readback produces the blue score fallback.

## Validation

Pixel tests cover opaque/half-alpha/transparent pixels, straight/premultiplied
bytes, PNG round trip, pending read and clone lifetime, invalid sizes/formats,
disposed pictures, synchronous codec completion, and asynchronous codec errors
to normal and ephemeral listeners. Web also uses corrupt `MemoryImage` data,
the actual local-file `MemoryImage` path and the supplied `NetworkImageIo` URL.

The Dart oracle compares **the exact readback bytes from each host**: all palette
keys/populations, selected seed, and 46 light + 46 dark Tonal Spot roles. It also
covers near colors requiring extra centers, uniform white, mixed alpha, and fully
transparent inputs. This does not assert identical decoding/scaling pixels
between native Skia, CanvasKit, and Flutter engines. Such small pixel differences
can change the selected seed. Wide-gamut, animated-image playback, other native
OS live execution and physical display acceptance remain unverified.

From the repository root (the native runner enforces the 20-minute timeout):

```powershell
./Doroti/eng/test-image-pipeline.ps1 `
  -ImageFiles @('mae-mu-9002s2VnOAY-unsplash.webp', '.doroti/evidence/material-image-repair/unsplash-1734210255965.jpg') `
  -DartExecutable C:/Users/parti/flutter/bin/cache/dart-sdk/bin/dart.exe
```

The URL fixture was downloaded from
`https://plus.unsplash.com/premium_photo-1734210255965-0a721514a34e`.
The original JPEG results remain historical evidence; the requested current local
input is the WebP also bundled by `reference/flutter_sample_app`.

For Web, build Testbed with `-p:DorotiImageValidation=true` and run:

```powershell
$env:DOROTI_IMAGE_VALIDATION = '1'
./Doroti/eng/run-web-playwright.ps1 -SkipBuild -HeadlessOnly `
  -RendererMode worker-canvaskit-webgl -TestFile tests/image-pipeline.spec.ts `
  -ArtifactLabel image-pipeline-new -Port 5096
```

`DOROTI_IMAGE_FILE` overrides the default WebP. Feed the resulting `*photo.json`
files to `color-oracle.dart` with the sample's `.dart_tool/package_config.json`.
Unconditional production Web builds do not include the validation export.

The reference Components **Image demo** section can be checked after a standard
`flutter build web --release` using `test-flutter-image-demo.ps1` with a fresh
artifact label. That test activates Flutter semantics and dispatches the controls'
semantic click actions. It does not prove physical pointer hit testing, and is
distinct from Doroti's image-pipeline test.

Source versions, hashes and license mapping are in `source-provenance.json` and
`../../THIRD-PARTY-NOTICES.md`.
