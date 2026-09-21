# Web iframe / PlatformView contract

2026-09-21. **PARTIAL**. This is the product main-DOM/managed-Worker integration, not the earlier standalone DOM harness. Evidence and remaining gates: [Web results](../../validation/webview/web-results-2026-09-21.md).

## Registration and API

Testbed and the generated Web template register `doroti/webview` for `browser-wasm` in `web/application-manifest.json`:

```json
"platformViews": [{ "viewType": "doroti/webview", "rid": "browser-wasm" }]
```

The existing owner-bound `PlatformViewCoordinator` owns the same iframe used by `WebViewController` and `WebViewWidget`. Placement never reparents/recreates the iframe. Use an explicit browser profile:

```csharp
var web = new WebViewController(owner, new WebViewOptions(
    Html: "<!doctype html><input value='Editable HTML'>",
    Profile: WebViewProfile.BrowserDefault));
await web.Ready; // DOM instance exists; not a page-load or first-content-frame receipt
var features = await web.ExecuteAsync(new(WebViewOperation.Features));
// Place new WebViewWidget(web) in a bounded layout. Dispose the controller explicitly.
```

`BrowserDefault` uses the browser's existing cookies/storage. It is not a private or independently persistent profile. Default `Ephemeral` and native `SharedPersistent` creation are rejected with `Unsupported`; native hosts reject `BrowserDefault`. The sample selects BrowserDefault explicitly on Web. Same-origin content is trusted application code: `allow-scripts` plus `allow-same-origin` is **not an isolation boundary** for such content.

| Feature | Current behavior |
|---|---|
| Navigate / HTML | Absolute HTTP(S) URLs and srcdoc HTML, maximum 2 MiB commands/HTML |
| JS | Loaded same-origin document only; JSON value / undefined distinguished; exceptions typed; promise result bounded to 2 MiB |
| Cancellation / lifetime | 32 commands per instance; 30-second waits; caller cancellation, navigation/document replacement and close reject pending results; synchronous page JS still executes on the browser main thread |
| Reload / stop | Same-origin only; cross-origin is explicitly unsupported |
| State | iframe load events; no HTTP success, progress, first-pixel or complete navigation interception proof. `HistoryKnown=false`, `NavigationStateKnown=false`; remote Url is the requested URL, not verified redirect destination |
| History / clear data | Unsupported. False history booleans are placeholders with `HistoryKnown=false`, not a known empty browser history |
| App content | Deploy HTML/CSS/assets under the application's same-origin HTTP(S) host and navigate to its URL. Native `Resources` manifests / `doroti-app` routing are unsupported on this adapter |
| Origin policy | `AllowedOrigins` checks commands issued through this controller. It cannot stop iframe-internal links, script navigation or redirects. Browser CSP/frame-ancestors/COEP/sandbox rules remain authoritative |
| Popup / media / permissions | No application policy API. Sandbox omits popups/top-navigation/download/fullscreen permissions; file selection and permission decisions remain browser behavior |
| Input / accessibility | Direct browser iframe interaction and native DOM semantics; foreground shields forward once to the existing Doroti ingress. Native-origin GestureArena, framework-wide Tab traversal, physical Korean IME and screen-reader qualification remain open |

Feature query reports `BrowserDefaultProfile=true`; JS availability is document-dependent. Cross-origin pages do not receive arbitrary JS/history/cookie capabilities. `load` is not an HTTP-success or Presented event ([iframe contract](https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/iframe), [same-origin policy](https://developer.mozilla.org/en-US/docs/Web/Security/Defenses/Same-origin_policy)).

## Cooperative messages

Opt in with exact `MessageOrigins`, e.g. the application's deployed HTTPS origin. After a load the parent sends `doroti-webview-init` version 1 to that exact target origin, with `documentGeneration` and an unpredictable `nonce`. A cooperating page validates `event.source === parent` and the expected parent origin, then replies to that parent origin:

```js
parent.postMessage({
  type: 'doroti-webview-message', version: 1,
  documentGeneration: init.documentGeneration, nonce: init.nonce,
  requestId: ++requestId, name: 'selection', json: JSON.stringify({ value: 42 })
}, expectedParentOrigin);
```

The receiver checks the actual `MessageEvent.source` window, actual origin, document generation, nonce, increasing positive request ID, bounded name and valid JSON up to 64 KiB. A payload's claimed origin is never trusted. Child-frame, stale, replayed and oversized messages are rejected. If a cross-origin redirect's final origin differs from the requested origin, that document cannot receive the handshake; there is no claim that iframe navigation interception can discover it.

## Composition and effects

The existing Worker WebGPU/Graphite or WebGL/Ganesh presenter remains active. Mixed scenes are split by the common planner into raster / iframe / raster / effect / sharp foreground. **Changed mixed raster slices render with CPU Skia and upload RGBA to main-DOM canvases**; these are not shared GPU textures. Live iframe content is never read back or replaced by a screenshot. Independent pictures within a foreground segment use enforced clip bounds and separate retained canvases, so an integer-translated panel can move without uploading the stationary UI again. Proven-equivalent slices move by CSS transform; changed pixels, clipping or resource scope redraw. Main/Worker ACKs retain the native plan leases until accepted or superseded.

Bounds are logical CSS pixels with physical RGBA backing dimensions. Rectangular clipping and supported axis-aligned placement use stable iframe wrappers. Limits: 16 views, 17 raster segments (up to eight independent slices each), 16 shields, four isotropic backdrop regions, sigma 0–16 and saturation 0–2. Unsupported groups or excessive slice counts retain the single-raster path. Raster frames have a 64 MiB pixel budget; splitting falls back to the original union rasters if overlapping slices would exceed it. Staging, structured-clone transfer and browser backing stores add temporary copies. This is an allocation guard, not a total-process memory or frame-rate budget.

CSS `backdrop-filter: blur(...) saturate(...)` samples preceding siblings. The sharp child and independent ARGB tint stay in the following Doroti raster. Effects have no pointer, focus or semantics endpoint. `CSS.supports` is only initial negotiation: actual iframe/raster pixels were separately calibrated in Chromium 153. Ancestor backdrop roots, opacity, browser differences and protected/media content can change sampling ([CSS backdrop contract](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Properties/backdrop-filter)). Other browser engines, rounded/path/affine/group effects and protected media are not qualified.

Placement belongs to the scene frame; independent `AttachAsync` placement is explicitly rejected. Accepted frame receipts update coordinator Attached/Hidden state without applying DOM placement twice; stale receipts never revive disposing instances. Versioned whole-frame validation precedes DOM mutation. Changed raster canvases are prepared before the synchronous placement/swap; stale epochs, resize generations, retired instances and late frames after close are rejected. A receipt is `BackendAccepted`, not physical atomic display. Shutdown waits for managed coordinator disposal and outstanding control replies before ending the shared-runtime Worker role. Lost GPU contexts are terminal in the main-owned runtime: resources close and a page reload is required; there is no automatic history/form recovery claim. Worker restart replies are sent to the original endpoint, not a newer session.

An app with no platform content creates no composition DOM/canvas/effect resource, makes no extra pixel uploads, and lazily avoids loading the composition/WebView modules. Normal raster drawing still follows the existing GPU path. [WebView panel drag measurements](../../validation/webview/panel-drag-2026-09-21.md) show reduced raster work and no steady-motion uploads for retained content. The earlier 1/4-view active-content workload results predate this optimization and have not been requalified. Two full product owners, real monitor-only DPR transitions, physical input/accessibility, broad browser/provider/GPU coverage, clean-machine deployment, Debug startup and NativeAOT are still open.

The generated template was built/published using locally packed NuGet dependencies, then ran its default zero-view page and a WebView/controller/effect qualification page. A fresh-machine installation is still a separate gate. The shared runner now sets Web Release symbol policy before the SDK derives static PDB assets.

## YouTube sample

The Material WebView sample keeps its local HTML on Web instead of automatically navigating to a YouTube `/watch` page. It includes the requested `M7lc1UVf-VE` `/embed` player inside a card in the existing sample HTML alongside its native input, counter and animation, sized at 560 by 315 pixels (shrinking proportionally on narrow screens), with `strict-origin-when-cross-origin` referrer policy. Only the nested YouTube iframe opts into `credentialless`; the outer WebView's profile and the WASM server's COOP/COEP headers are unchanged. This requires a browser supporting [credentialless iframes](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/IFrame_credentialless) and uses a separate temporary storage context rather than the user's normal YouTube login. YouTube requires [client identification such as HTTP Referer](https://developers.google.com/youtube/iframe_api_reference#onError); an iframe `load` event alone does not prove playback.

Run `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiTestbedApp -Platform web -Configuration Release`, open `http://127.0.0.1:5088/?dorotiTestbedMode=sample`, and select **WebView**. The home button restores the local embed sample after navigating elsewhere. Canonical-run browser evidence is under `Doroti/artifacts/webview/2026-09-21/web/youtube-*`; it postdates the earlier package qualification hashes.

Chromium canonical-run validation passed: Release build has zero warnings/errors, top-level `crossOriginIsolated` stays true, the YouTube document receives the localhost origin as referrer and reports `credentialless: true`. After a CDP pointer click, the actual video remained unpaused with readyState 4 and advanced from 5.637 to 8.644 seconds (`youtube-playback/playback.json`, screenshot alongside). This confirms this public video's playback, not protected-media sampling or other browsers.
