# PlatformView rearchitecture validation

All build/test/run children use `../run-with-timeout.py` with a 1,200-second process-tree timeout. Run from the repository root. Generated outputs must stay outside this source directory.

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/platform-views/Common/Common.csproj --artifacts-path Doroti/artifacts/platform-views/common-build
python Doroti/validation/run-with-timeout.py pwsh -NoProfile -File Doroti/eng/build-hwnd-exact-cpp-native.ps1
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/platform-views/verify-windows-effects.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-acrylic-composition/verify-sample-input.py
python Doroti/validation/run-with-timeout.py dotnet build Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj
python Doroti/validation/run-with-timeout.py python Doroti/validation/platform-views/verify-web-dom.py
```

The common console fixture checks owner/generation, snapshot/admission races, lease retirement, stale frames, commit failure, cancellation, and client disposal. It uses fake native objects and does not prove product rendering.

## WinUI 3 native-control backdrops

The Windows App SDK native button/editor use real WinUI 3 controls. Their live
`CompositionVisualSurface` sources and preceding Doroti raster slices feed a
bounded Gaussian effect on the WinUI compositor. Each effect samples only the
layers before it; native HWNDs retain input/focus. Effect bridge HWNDs use
`WS_EX_TRANSPARENT` and their visual trees are excluded from hit testing; disabling
the bridge alone does not prevent Windows 11 from swallowing mouse/wheel input.
Foreground input shields are also applied inside each XAML island, because XAML
pointer delivery can bypass its HWND subclass. Unhandled native wheel events
bubble to the surrounding Doroti list. Supported limits are four effects, isotropic logical sigma <= 32 and
physical sigma <= 128, rectangular clips, and 256 MiB of estimated effect staging.
WebView2 continues to use its separate Windows.UI.Composition path.

```powershell
pwsh -NoProfile -File Doroti/eng/validate-windows-winui-controls.ps1
```

This runner applies its own 1,200-second child-process timeout. It requires Python
with Pillow and writes to `Doroti/artifacts/windows-winui-controls`. It checks
default startup, native templates/layout, blur on/off, a negative control that
omits native samples, WebView2 effects, and clean XAML shutdown. An opt-in,
in-process Windows.Graphics.Capture probe captures only the host's own client
area for pixel comparison; capture is never the backdrop transport. The pixel
gate checks softened edges, native contribution, and unchanged pixels outside
the clip, and writes `pixels.json` and `winui-blur-comparison.png`. This evidence
does not qualify physical keyboard/pointer/IME input or atomic display timing.

The same runner also opens the native page through the default gallery/sample
route and captures 30 consecutive client frames. `continuity.json` requires the
editor and backdrop to remain stable while the spinner changes, with at most
three initial placement batches. This detects transient layer disappearance
that a single screenshot cannot detect. Animation-only frames retain child HWND
order/geometry and the effect's sampled visual tree; geometry-only updates do
not raise sibling windows again.

`input/result.json` exercises the default native page using OS `SendInput`: wheel
scroll, foreground clicks over blur, native button/text input after scrolling,
blur pass-through, native wheel input, scrolling controls out of view and back,
and navigation away/back with new native controls. Mouse presses are held across
animation frames. Direct `SendMessage` dispatch is used only as a responsiveness
probe; it is not sufficient to verify Windows 11 composition input routing.
This automated evidence remains separate from physical human input and Korean IME.

The Windows effect fixture is also accessible with `DOROTI_TESTBED_MODE=platform-effects`, or **WebView effects** at the bottom of the native Platform views page. Its own synthetic-HWND input gate is separate from the existing gallery gate's OS SendInput. Both remain separate from physical human input and Korean IME approval. Test runs use isolated WebView profiles under artifacts.

The Web gate compiles the real DOM adapter, then uses a separate headless Chrome profile and a loopback HTTP server. Its result is explicitly a DOM adapter harness, not product Worker integration or GPU visual qualification.

The restored [Linux Qt gates](../linux-qt-quick/README.md) cover real Quick
WebEngine/effect product input and captures, native owner/batch contracts, and
published-image preservation on a Qt-owned Vulkan queue. Quick and Widgets,
XWayland and native Wayland, and automated versus physical input are separate.
