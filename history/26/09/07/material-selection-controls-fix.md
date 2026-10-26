# Material sample slider / switch repair

## Request and comparison

Reviewed `DorotiTestbedApp` at `http://127.0.0.1:5088/?dorotiTestbedMode=sample`
against the local `reference/flutter_sample_app` and installed Flutter SDK sources.
Also opened the existing Flutter Web build at local port 5090 for visual comparison.
Existing scroll-related working-tree changes were retained.

## Changes

- Material Slider and RangeSlider now use floating-point division for tick positions
  and value snapping. Five divisions produce six distinct positions; intermediate
  values no longer truncate to zero. Slider's state normalization is repaired too.
- ParagraphBuilder preserves an unspecified line height so the native text host
  uses actual font metrics. MaterialIcons switch glyphs now have the natural 16px
  height/baseline instead of an invented 19.2px line. Explicit/inherited heights
  still work. Switch icon offsets remain aligned with Flutter's implementation.
- RoundRangeSliderThumbShape uses its sliderTheme parameter instead of casting
  textDirection to SliderThemeData; the mounted regression exposed this exception.
- Disabled Switch tap/cancel callbacks remain null, avoiding a null callback
  invocation found by the pointer regression.
- During Web verification, a fresh build stalled at Localizations before rendering
  SampleHome. SynchronousFuture's value-returning continuations now dispatch
  virtually through Future and Future<T> and preserve synchronous chained results.
  TickerFuture's matching method is an override. Ordinary Future callbacks still
  queue through the captured scheduler. Fresh Web startup succeeded after this fix.
  Temporary diagnostic instrumentation was removed from worker/localization sources.

## Evidence

All test runners used the repository's 20-minute process timeout.

- `.doroti/evidence/selection-controls-before-v8`: original RangeSlider thumb
  invalid-cast failure. Earlier before directories retain test-harness setup errors.
- `.doroti/evidence/selection-controls-before-v9`: two distinct tick positions
  instead of six; .19/.41/.59/.81 snap to zero; icon measured 16 x 19.2.
  The initial icon assertion incorrectly expected 18px; it was corrected to
  Flutter's actual 16px constant. The measured excess line height remains a real
  failure; the old log is preserved, not relabeled as PASS.
- `.doroti/evidence/selection-controls-after-input`: disabled Switch click exception.
- `.doroti/evidence/selection-controls-final`: PASS for tick geometry, continuous
  mode, state/render snapping, pointer tap/toggle/disabled behavior, LTR/RTL,
  light/dark, check/close glyph metrics, natural/explicit/inherited paragraph
  heights, synchronous Future dispatch/chains and ordinary asynchronous dispatch.
  Native raster PNGs are included in this directory.
- `.doroti/evidence/selection-controls-regression-final`: FCR-7 / Material sample /
  themes / shortcuts / context menus / virtual dispatch PASS.
- `.doroti/evidence/selection-controls-windows-final`: existing native text/pixels,
  pickers, tabs, image sample, drawer and app-bar regressions PASS.
- Web Release build: 0 warnings, 0 errors. Server restarted on 5088 with final build.
- Live Chrome, default `worker-direct-webgl`: actual content visible after reload;
  slider ticks visible; pointer click selected 60 and drag selected 40; switch
  toggled check/close with centered glyphs; disabled switch retained its state;
  light and dark themes visually checked. Flutter's local Web build was inspected
  for the corresponding tick/icon appearance, not a quantified pixel-diff gate.

These are targeted source, automated native and live Web checks. Physical-device
acceptance, full application accessibility and quantitative cross-host pixel parity
remain notVerified.

## Reproduce automated checks

```powershell
./Doroti/eng/test-material-sample.ps1 -Suite Selection
./Doroti/eng/test-material-sample.ps1 -Suite Regression
./Doroti/eng/test-material-sample.ps1 -Suite Windows
```
