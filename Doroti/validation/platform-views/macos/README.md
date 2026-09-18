# AppKit product validation

Run from the repository root on an interactive macOS desktop. All children use
the repository's 20-minute process-tree wrapper. Use fresh output directories;
failed runs are preserved. `screencapture` must be able to capture the app window.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet build \
  DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj -c Debug \
  -p:RuntimeIdentifier=osx-arm64 -p:DorotiMacOSTargetFramework=net10.0-macos27.0

python3 Doroti/validation/run-with-timeout.py dotnet run \
  --project Doroti/validation/platform-views/Common
python3 Doroti/validation/run-with-timeout.py dotnet run \
  --project Doroti/validation/webview/Common

python3 Doroti/validation/run-with-timeout.py python3 \
  Doroti/validation/platform-views/macos/run-probe.py \
  --app 'DorotiTestbedApp/macos/bin/Debug/net10.0-macos27.0/osx-arm64/Doroti Testbed (AppKit).app' \
  --output /tmp/doroti-macos-graphite --renderer graphite
# Repeat with a fresh directory and --renderer ganesh.
```

The opt-in Testbed probe uses the public controller/widget, scene planner, real
WKWebView, Metal foreground and public Core Image backdrop filters. It writes `result.txt`,
window captures and effect bounds/backing scale. It checks initial HTML, JS
errors/JSON/null/undefined, document generation, profile clear, lifecycle/input
hit routing, native DOM animation/scroll, seven effect stages, two WebViews,
resize identity, content scheme and origin-checked messages. Input hit routing
is not physical mouse/IME/accessibility qualification.

For pixel checks install Pillow and NumPy in a local virtual environment, then:

```sh
python3 Doroti/validation/run-with-timeout.py /path/to/venv/bin/python \
  Doroti/validation/platform-views/macos/analyze-pixels.py /tmp/doroti-macos-graphite
```

The pixel gate checks monotonically changing opacity strengths, suppression of
checker detail, identical .375 appearance after 1→.375, live source changes and
sharp foreground contrast. It does not measure Gaussian sigma or common visual
tolerance. Source is paused during strength comparison and running for freshness.

Current evidence directory: `Doroti/artifacts/platform-views/2026-09-18/macos/`.
The first graph/ganesh runs qualify the initial attachment/command adapter;
subsequent runs add content/message and final-source validation. Logs label each
renderer. The failed default-TFM/Xcode mismatch is retained separately. Builds
use normal CoreCLR, not NativeAOT.

NativeAOT publish was attempted with `PublishAot=true` and rejected by the
existing runner guard `DOROTIAOT002` (the repository NativeAot profile currently
accepts only iOS ios-arm64). `nativeaot-publish.log` records this build-time block;
it is not an ILC/link/run success. The guard was not bypassed or relabeled as
successful AOT.

## 2026-09-18 final result

Environment: macOS 26.6.2 (25G83), arm64, Xcode 27.0 (27A266a), macOS SDK 27.0,
.NET SDK 10.0.400 / macOS SDK pack 27.0.10539-xcode27.0. Debug build and Release
CoreCLR publish completed without warnings/errors. Release generated
`DorotiTestbedApp.MacOS-0.2.0.pkg`; pkgutil expanded it into a new temporary
folder and both renderers ran from that extracted app. This validates local
package contents, not notarization, Gatekeeper or a clean second machine.

| Evidence | Result |
|---|---|
| `platform-contract-final.log` | 11 existing composition/lifetime contracts PASS |
| `webview-contract-final.log` | owner/event/command-close/stale-generation/options contracts PASS |
| `release-graphite/` | all seven scenes, WebView API/content/message, resize and pixel gates PASS |
| `release-ganesh/` | same product and pixel gates PASS |
| `nativeaot-publish.log` | BLOCKED by DOROTIAOT002 before native compilation |

Both release runs have source/executable hashes in `run-info.json`. Both have
zero same-strength reset and fixed-source light/dark effect ROI differences.
The initial `graphite-2` content/message run failed because the fire-and-forget
bridge returned WebKit's Promise; the final bridge intentionally returns
undefined and both release runs pass. The failure is preserved. All capture
comparison metrics are in each run's `pixels.json`.

## Custom Gaussian/color backdrop (current)

Run `run-probe.py` with `DOROTI_MACOS_CUSTOM_BLUR_PROBE=1` to capture spatial
edge calibration on live WebView and Metal raster sources, sigma 0/2/4/8/16/32/64,
saturation 0/1/2, zero-radius desaturation and independent green tint. The normal
fixture now also has radius, saturation and tint-opacity controls.

```sh
python3 Doroti/validation/run-with-timeout.py env DOROTI_MACOS_CUSTOM_BLUR_PROBE=1 \
  python3 Doroti/validation/platform-views/macos/run-probe.py \
  --app 'DorotiTestbedApp/macos/bin/Debug/net10.0-macos27.0/osx-arm64/Doroti Testbed (AppKit).app' \
  --output /tmp/doroti-custom-blur-graphite --renderer graphite
python3 Doroti/validation/run-with-timeout.py /path/to/venv/bin/python \
  Doroti/validation/platform-views/macos/analyze-custom-blur.py /tmp/doroti-custom-blur-graphite
```

Use `analyze-custom-blur.py` for the current adapter. The old `analyze-pixels.py`
checks were written for material opacity and are retained for historical runs;
they are insufficient to approve a spatial Gaussian. The current analyzer fits
edge spread (including linear/sRGB working-space distinction), checks unmixed
black/white levels, separately checks filtered Metal raster edges, and verifies
saturation, authored tint, live content, sharp foreground and theme/reset.

The new evidence lives under `2026-09-18/macos/custom-blur/`. Initial calibration
failures are retained: hidden filter slots continued sampling foreground when
effects were disabled, and fixed-delay captures could observe previous state.
The current host clears inactive filter arrays; the capture driver waits for
committed effect parameters and the host serializes native composition frames.

Current Debug results: `custom-blur/graphite-custom-5` and
`custom-blur/ganesh-custom-3` pass all product and custom pixel gates. The spatial
fit recovers 2/4/8/16/32 pt on the native source and 4/16 pt on the Metal source;
64 pt measures about 66.5 pt in this finite fixture (within the 8% gate).
Uniform source color is preserved after converting capture ICC profiles to sRGB.
Zero radius has no transition pixels, saturation 0 is gray, 1 preserves source
and 2 increases chroma. Tint is compared against an independent native sRGB
CALayer overlay with alpha 0.4: difference 0. This reference includes the display
compositing/color-profile behavior; the nominal linear-sRGB alpha equation is
reported diagnostically and is not the reference. Radius reset and theme MAE
are both zero. Foreground gradients remain sharp, and live source MAE is nonzero.

The pending-frame and inactive-filter fixes were driven by preserved failures;
no old material PASS has been promoted to a Gaussian PASS. All child processes
still use the 20-minute timeout. Physical input, two owners, multiple/overlapping
effects, rounded clipping, device loss and performance budgets remain separate.

Current Release: `custom-blur/release-publish.log` completed without warnings or
errors. The generated package was expanded into a fresh temporary directory;
`custom-blur/release-graphite` passed product and spatial/color pixel checks from
that extracted app. `common-final.log` passes 12 contracts, including native
saturation negotiation and zero-radius color adjustment.
