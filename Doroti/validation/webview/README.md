# WebView validation

Android commands, calibration and workloads: [Android validation](../platform-views/android/README.md).

Run from the repository root. Every build/test/run child must use the 1,200-second
wrapper. Keep shared-output builds and live UI gates sequential. Source lives here;
generated output lives under `Doroti/artifacts/webview/<date>/windows/`.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/webview/Common/Common.csproj --artifacts-path Doroti/artifacts/webview/common-build
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/verify-windows.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/platform-views/verify-windows-effects.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/verify-windows-calibration.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/measure-windows-workloads.py
```

`verify-windows.py` supports `DOROTI_WEBVIEW_GATE_EXE` for a freshly published copy
and `DOROTI_WEBVIEW_GATE_OUTPUT` for a unique artifact directory. Every run uses its
own WebView user-data directory and closes the owned product window. The fixture
uses the public controller/coordinator of the displayed product, not a separate
SDK test host. Failures are preserved; use a new output directory for a retry.

`verify-windows-effects.py` includes actual OS touch injection and mouse input.
Injected touch uses contact ID zero with a one-contact injection device. These are
automated tests, not physical-input or Korean IME approval.

`verify-windows-calibration.py` captures 11 bounded stages from one live process.
It measures Gaussian 10–90% edge spread separately on WebView and raster sources,
zero/reset, saturation 0/1/2 and independent tint. Captures avoid the sharp text for
the numerical ROI; the existing effect gate covers the foreground/input scene.

`measure-windows-workloads.py` runs 0/1/4 views × idle/animation/scroll/modal for ten
seconds each. It records owner-process private/working memory and available
raster/readback/UI commit timing percentiles. Chromium/GPU memory and scanout/input
latency are not observed. The zero-native fast path does not generate composition
timings. These bounded observations are not a before/after performance acceptance.

Current support and outstanding gates: [Windows contract](../../docs/platform-views/windows-webview.md).

## Web (2026-09-21)

[Product execution, commands and limitations](web-results-2026-09-21.md). BrowserDefault is explicit; use Release for the currently verified product runtime. WebGPU/WebGL product evidence is separate from the DOM adapter harness, physical input and NativeAOT.
