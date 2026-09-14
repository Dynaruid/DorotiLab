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

The Windows effect fixture is also accessible with `DOROTI_TESTBED_MODE=platform-effects`, or **WebView effects** at the bottom of the native Platform views page. Its own synthetic-HWND input gate is separate from the existing gallery gate's OS SendInput. Both remain separate from physical human input and Korean IME approval. Test runs use isolated WebView profiles under artifacts.

The Web gate compiles the real DOM adapter, then uses a separate headless Chrome profile and a loopback HTTP server. Its result is explicitly a DOM adapter harness, not product Worker integration or GPU visual qualification.

The restored [Linux Qt gates](../linux-qt-quick/README.md) cover real Quick
WebEngine/effect product input and captures, native owner/batch contracts, and
published-image preservation on a Qt-owned Vulkan queue. Quick and Widgets,
XWayland and native Wayland, and automated versus physical input are separate.
