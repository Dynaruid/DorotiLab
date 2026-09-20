# Linux Qt Quick WebView

Status: **PARTIAL**. The controller uses the same live WebEngine Quick item as
PlatformView composition; it does not create a second browser or compositor.
Enable `DorotiQtQuick=true` and `DorotiQtWebEngine=true` on the Linux runner and
register `doroti/webview` for `linux-x64` in its application manifest. Widgets is
still a separate B-only backend with no WebView controller support.

```csharp
await using var controller = new WebViewController(owner, new(
    Html: "<!doctype html><title>Example</title><input value='한글'>",
    AllowedOrigins: ["https://example.com"]));
await controller.Ready;
using var loadTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
while (true)
{
    var state = await controller.ExecuteAsync(new(WebViewOperation.State), loadTimeout.Token);
    if (!state.IsLoading && state.DocumentGeneration > 0) break;
    await Task.Delay(50, loadTimeout.Token);
}
var features = (await controller.ExecuteAsync(new(WebViewOperation.Features))).Features;
var result = await controller.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript,
    "({title:document.title,value:document.querySelector('input').value})"));
// Attach new WebViewWidget(controller) in this owner's widget tree.
```

Wait for document completion before evaluating JS. `Ready` is attachment creation,
not first browser content presentation. `State` exposes loading, URL/title and
back/forward availability. Navigation, HTML, reload/stop, back/forward and JSON JS
results use the public Quick API. Undefined is distinct from JSON null. Exceptions,
Promises, cycles, non-JSON and oversized results fail explicitly. Commands/results
are limited to 2 MiB, 32 pending requests, and a bounded script timeout. Cancellation
retires the caller's result; it cannot interrupt JavaScript already executing in
Chromium. Navigation invalidates pending JS immediately; stale callbacks cannot
complete a new document's request. Qt Quick reports load start/completion/failure;
this adapter does not synthesize a native commit or first-content-frame event.

The independent WebView C ABI is version 1, 32 bytes, features `0x1f`; all entrypoints
validate thread and owner/item tokens. It does not replace host callback ABI 4
(192 bytes), the PlatformView ABI, or the 96-byte Quick part. Callback UTF-8 is
borrowed only during the call. Normal retirement unbinds before freeing the managed
context; native owner close destroys its objects before managed callback cleanup.
Renderer process failure is terminal and fails pending commands. Explicit controller
recreation starts a fresh document; history/forms/media are not restored.

## Profiles and app content

The default is one off-the-record profile per view. `SharedPersistent` uses an
explicit shared profile whose storage name is scoped by the application manifest
ID. Different apps do not share the generic Qt application's default profile path.
There is no named isolated-persistent API. Full profile storage deletion is **not
supported** by this adapter: `ClearAllData=false`, and `ClearData` throws
`WebViewError.Unsupported`. Cookie/cache clearing alone is not reported as deletion
of local storage, IndexedDB, service workers and all other profile state.

An ephemeral view may map exact `doroti-app://content/path` routes to manifest
resource keys and MIME types using `WebViewOptions.Resources`. Resources are
integrity-checked by the existing application resource capability before creation.
Limits are 8 MiB per resource, 12 MiB per view, and 256 routes. The native scheme
serves bounded QBuffers owned by each request job, so canceled requests cannot
retain a managed callback. Unknown paths, encoded path escapes, non-GET and Range
requests are rejected. Streaming/Range/media are not advertised. Shared-persistent
profiles reject per-view resource maps because changing one shared scheme handler
would affect other views.

Messages require `MessageOrigins=["doroti-app://content"]` and manifest resources.
The page calls `doroti.postMessage(name, payload)`. QWebChannel lives in
ApplicationWorld, and exposes only a small message receiver, not the profile or
command QObject. A main-frame-only isolated script relays messages; native code
checks the current app URL, document generation, request ID, name and 64 KiB size.
App documents block remote subresources and frames. This is a **trusted app-content
bridge**, not authentication of arbitrary Internet frames. HTTP(S) origins and
untrusted documents receive no message facade. A payload's claimed origin is never
authority. General remote-page messaging remains unsupported.

Navigation allows HTTP(S) within `AllowedOrigins` (null permits HTTP(S), empty
permits none), and configured app content. Credentials, file and external protocols
are rejected. Popups, permission grants, file dialogs, script dialogs, authentication
dialogs, protocol registration, downloads and fullscreen are denied by default;
there are no public policy callbacks for enabling those features yet.

## Runtime and deployment

The source API floor is Qt **6.8.0** (including Quick permission APIs); execution
in this work uses Ubuntu 26.04.1, Qt **6.10.2**, WebEngine distro build
**6.10.2+dfsg-1**, linux-x64. The API floor is not a tested minimum-patch/security
support claim. Build and run against the same distribution's compatible Qt build.
No private Qt headers, WebEngine texture extraction or Chromium download/build is
used. Doroti supplies only `libdoroti_qt_host.so`, `libdoroti_webview_qt.so`, managed
code and `doroti-webview-runtime.json`. Qt/Chromium/helper/resources/locales/QML/QPA
remain system dependencies. Disabling WebEngine removes the shim and its link/startup
dependency, including stale copied outputs when switching the option off.

Ubuntu development dependencies include `qt6-base-dev`, `qt6-declarative-dev`,
`qt6-webengine-dev`, `qt6-webchannel-dev`, Vulkan and Wayland development packages.
Runtime closure includes Qt Core/Gui/Widgets/OpenGL/Quick/Qml/QuickControls2,
WebEngineQuick/Core, WebChannel, the QtQuick/Controls/QtWebEngine/QtWebChannel QML
modules and their distro dependencies, active xcb/Wayland QPA, `QtWebEngineProcess`,
WebEngine .pak data and `qtwebengine_locales`. Do not copy these into the app.
Normal builds use the checked-in Vulkan shader; ShaderTools is needed only to
regenerate it. The runtime manifest records the actual build's Qt/WebEngine version.

Bootstrap checks the helper, required resource packs and en-US locale before
Chromium starts. These produce typed Unsupported errors naming the missing
category. Missing native libraries produce a typed runtime/shim error; missing
WebEngine QML produces a typed creation error plus Qt's detailed import diagnostic.
The loader's host ABI checks remain mandatory. Run as a normal user with the default
Chromium sandbox. Root or `--no-sandbox` results are not product approval.

After system Qt/WebEngine updates, rerun native ABI/two-owner/process-loss, product
API, composition/input, Gaussian/color calibration and deployment checks on each
advertised QPA/GPU. The recorded apt candidate is an observation, not a guarantee
that the distro package contains every current Chromium security fix.

Reproduction and evidence: [Qt validation](../../validation/linux-qt-quick/README.md)
and [2026-09-20 results](../../validation/webview/linux-results-2026-09-20.md).
Native-origin GestureArena, full Tab/IME/Orca, physical GPU/device loss, permission
policy UI, full profile clear, all C/E scenarios, package-only clean-machine
installation and Linux NativeAOT are separate remaining gates.

Public contracts: [WebEngineView](https://doc.qt.io/qt-6/qml-qtwebengine-webengineview.html),
[script collection](https://doc.qt.io/qt-6/qml-qtwebengine-webenginescriptcollection.html),
[Quick profile](https://doc.qt.io/qt-6/qquickwebengineprofile.html),
[deployment](https://doc.qt.io/qt-6/qtwebengine-deploying.html),
[platform requirements](https://doc.qt.io/qt-6/qtwebengine-platform-notes.html),
[Ubuntu package](https://packages.ubuntu.com/en/resolute/qt6-webengine-dev).
