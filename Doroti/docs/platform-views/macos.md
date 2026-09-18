# AppKit macOS PlatformView / WebView

The native AppKit runner (`osx-arm64`) registers `doroti/webview` alongside native
button/editor attachments. Mac Catalyst is a separate UIKit runner and does not
inherit this implementation or its evidence.

## Composition and effects

A single WKWebView belongs to the existing PlatformView coordinator. Position,
rectangular clip, hiding, widget detach and reattachment preserve the native
instance. AppKit owns point-to-backing-pixel conversion. The host composites
native clips, intermediate Metal rasters, one NSVisualEffectView and foreground
Metal rasters in paint order. Shields participate in the same committed order.
GPU leases protect native instances even when submission fails before the common
session accepts the frame. Retained raster storage is limited to 256 MiB.

The inline effect uses the public `NSView.BackgroundFilters` API with
`LayerUsesCoreImageFilters=true`: `CIGaussianBlur`, followed by optional
`CIColorControls`. It has no NSVisualEffectView material or intrinsic tint.
Its opacity stays at 1. `MatchCommon.Strength` maps to logical sigma 0–16;
`ExactSigma` directly sets the numeric Gaussian radius in logical points, up to
64. Saturation is independent in [0,2]. Authored ARGB tint is a separate
foreground draw and the child stays sharp. Transparent Metal segment layers
are explicitly tagged sRGB. Tint is validated against an independent native
sRGB/alpha overlay through the same display compositor. A zero-radius saturation-only effect
is supported. Zero radius with saturation 1 removes the native filter array.

Core Image blurs in its working linear color space. Its edge profile and color
processing differ from the default Skia encoded-color path; numeric radius
control does not imply bit-identical cross-platform pixels. The calibration converts screenshot ICC profiles to sRGB and
fits both sRGB and linear-sRGB profiles rather than confusing increased
brightness with spatial blur. A single rectangular/isotropic effect is
currently qualified. Overlapping/multiple effects, arbitrary transforms,
rounded/path clipping, group opacity and native-origin GestureArena remain
outside this adapter's advertised scope.

When disabling/removing an effect, the host clears BackgroundFilters before
hiding/reusing its paint slot. Keeping filters on a hidden empty view can blur
the new foreground. Native composition/removal also admits only one pending
GPU frame to avoid stale scene replay; ordinary raster-only rendering retains
the existing three-frame limit. No UI-thread wait for GPU completion is added.
Reduce Transparency requires the explicit `SolidTint` alternative.

```csharp
new PlatformEffect(
    style: new PlatformEffectStyle(
        Match: PlatformEffectMatchPolicy.ExactSigma,
        ExactSigma: 12,
        Saturation: 1.25,
        Tint: 0x223c82f6),
    child: new Text("Sharp foreground"));
```

Use a bounded layout around the component. The macOS fixture provides radius,
saturation and tint-opacity sliders and clear/blue tint buttons. Native backends
without saturation support reject it in the planner. Raster-only PlatformEffect
applies a Skia saturation matrix after its Gaussian filter.

Public API contracts: [NSView backgroundFilters](https://developer.apple.com/documentation/appkit/nsview/backgroundfilters),
[CALayer backgroundFilters](https://developer.apple.com/documentation/quartzcore/calayer/backgroundfilters).
The old WithinWindow material-opacity captures are historical evidence only.

## WebView API and ownership

The initial shared API lives in existing `Doroti.Ui` (DTO/capability),
`Doroti.Hosting` (coordinator), `Doroti.Framework.Services` (controller) and
`Doroti.Framework.Widgets` (widget) assemblies. These use net10.0 and
source-generated creation JSON. Other backends reject controller creation until
their factory advertises WebViewCommands; settings JSON is never sent to their
legacy raw-HTML attachment. There is no new engine package or second native
instance. The native adapter remains in `Doroti.Host.Maui` with the macOS TFM.

```csharp
var web = new WebViewController(owner, new WebViewOptions(
    Html: "<!doctype html><title>Example</title><input value='Hello'>",
    Profile: WebViewProfile.Ephemeral,
    AllowedOrigins: ["https://example.com"]));
// Place in a bounded layout; dispose web explicitly when its owner is done.
var widget = new SizedBox(width: 620, height: 400, child: new WebViewWidget(web));
await web.Ready; // native instance ready; not page completion or first pixels
var state = await web.ExecuteAsync(new(WebViewOperation.State));
var result = await web.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript,
    "document.title", state.DocumentGeneration));
await web.DisposeAsync();
```

A controller can have one attached widget in its owner. Widget detach preserves
it; controller disposal immediately stops admission/input/listeners and cancels
pending commands, then the coordinator waits for GPU retirement before releasing
the native view. Commands before Ready fail with NotReady. The adapter admits at
most 32 asynchronous commands with 30-second timeout and cancellation. Async JS
never holds the placement semaphore or prevents close. Cancellation stops waiting for a result; WebKit does not undo an already-running script or data deletion. Results include request,
navigation and document generation; explicit stale document commands and late
results after navigation/close are rejected.

| Feature | Implemented scope / limitation |
|---|---|
| Navigation | HTTP(S), initial/replace HTML, app scheme, reload, stop, back/forward, URL/title/loading/history query; main-frame started/committed/completed/error events |
| Navigation policy | Optional exact HTTP(S) origin allowlist; empty list blocks remote navigation. File, data, javascript and external protocols denied. New-window/popup requests denied. TLS defaults preserved |
| JS | JSON primitive/array/map/null and explicit undefined; errors, cycles, BigInt, Promise and nonserializable top-level results rejected. No Promise-await API. Max script/result 2 MiB |
| Messages | Explicit MessageOrigins opt-in; WebKit native security origin and main-frame identity checked, version/document/request/name envelope, 64 KiB maximum; no native objects or reflection bridge |
| Profiles | Default per-view nonpersistent store, or explicit shared persistent WebKit default store. No named persistent isolated profile or ephemeral store sharing |
| ClearData | Explicitly removes all WebKit website data types since distant past in the selected store; shared-store deletion affects other views. Disposal does not clear persistent data |
| App content | Exact URL-path to compiler manifest key/MIME mapping, hash/length validated by application resource capability, relative resources, GET, at most 16 pending requests, bounded 8 MiB resources, cancellation on stop/close |
| Process failure | Terminal typed error and event. Recreate controller explicitly; history/form/media restoration is not claimed |
| Not implemented | Download/file chooser/permission/fullscreen customization, Range/media scheme responses, secure-context or service-worker promises, native first-content-frame timing, isolated persistent profiles |

`Features` reflects optional message/content configuration. A material capability
is not a promise of ExactSigma or cross-platform visual equivalence. Completion
of a native navigation is not a first-presentation notification. Same-document
URL changes can be queried with State; dedicated SPA navigation events are not
implemented.

App content configuration uses `doroti-app://content/index.html`:

```csharp
new WebViewOptions(AllowedOrigins: [],
    Resources: new() {
        ["/index.html"] = new("webview/index.html", "text/html"),
        ["/style.css"] = new("webview/style.css", "text/css")
    }, MessageOrigins: ["doroti-app://content"]);
```

Only listed routes resolve, and each key must exist in the runner's application
manifest. There is no general filesystem access or loopback server. A trusted
main-frame document can call `doroti.postMessage(name, payload)` after commit;
`Changed` emits a Message event on the owner dispatcher. The message payload's
self-declared origin has no authority. The navigation allowlist does not filter
all subresource requests or act as a network sandbox.

Official SDK contracts: [WKWebView](https://developer.apple.com/documentation/webkit/wkwebview),
[WKURLSchemeHandler](https://developer.apple.com/documentation/webkit/wkurlschemehandler),
[WebKit profiles](https://webkit.org/blog/14423/building-profiles-with-new-webkit-api/).

## Toolchain and qualification

Select `-p:DorotiMacOSTargetFramework=net10.0-macos27.0` with the installed
Xcode 27 .NET 10 SDK pack. Runner, native binding, target manifest and host use the
same TFM; target minimum macOS remains 14.0. The default TFM remains net10.0-macos
for the corresponding Xcode 26 toolchain. Xcode version validation is never
bypassed. The installed 27.0.10539-xcode27.0 pack still carries the SDK's preview
label; that informational warning alone is suppressed by the explicit profile.
[SDK releases](https://github.com/dotnet/macios/releases).

See [reproduction](../../validation/platform-views/macos/README.md) for current
commands and evidence. Physical Korean IME/VoiceOver, two product owners,
context/device loss, full E3, clean distribution/NativeAOT and performance budgets
remain separate gates. macOS 26.6.2 runtime evidence is not macOS 27 runtime evidence.

NativeAOT publish was attempted with `PublishAot=true` and rejected by the
existing runner guard `DOROTIAOT002` (the repository NativeAot profile currently
accepts only iOS ios-arm64). `nativeaot-publish.log` records this build-time block;
it is not an ILC/link/run success. The guard was not bypassed or relabeled as
successful AOT.

Release CoreCLR publish also passed. The generated package was expanded into a
fresh temporary directory and that app passed the same Graphite and Ganesh
product/pixel checks. This is local packaging evidence; notarization, Gatekeeper
and installation on a clean second machine remain unverified.
