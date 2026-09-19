# Windows WebView2 controller and effects

2026-09-19. **Windows work1/work2 status: PARTIAL.** The implemented and executed
path is WindowsAppSDK, Graphite/Vulkan, `CoreWebView2CompositionController`, and a
host-owned `Windows.UI.Composition` tree. Windows MAUI is still unconnected; these
results do not qualify its Microsoft.UI.Composition/MAUI presenter.

## Public API and ownership

`WebViewController` and `WebViewWidget` use the existing owner coordinator and its
native instance. `Ready` means controller creation, not loaded content or first
display. Placement, clipping, effect changes and widget detach preserve the
controller. Dispose closes admission and events, cancels pending commands, and
lets the coordinator retire frame leases before closing the native controller.

```csharp
var web = new WebViewController(owner, new WebViewOptions(
    AllowedOrigins: [],
    Resources: new()
    {
        ["/index.html"] = new("webview/index.html", "text/html"),
        ["/style.css"] = new("webview/style.css", "text/css")
    },
    MessageOrigins: ["doroti-app://content"]));
await web.Ready;
await web.LoadAppContentAsync("webview/index.html");
// Mount new WebViewWidget(web). Await Completed before evaluating the document.
var value = await web.ExecuteAsync(new(WebViewOperation.EvaluateJavaScript, "document.title"));
// Dispose the controller explicitly after its widgets have finished using it.
await web.DisposeAsync();
```

The application manifest must register `doroti/webview` and the referenced resource
keys. Unknown resources fail before native creation. The existing Material sample
now discovers the command capability and uses its controller UI automatically.

| Feature | WindowsAppSDK behavior |
|---|---|
| Navigation | HTTP(S), app content, HTML, reload/stop/back/forward; URL/title/loading/history and started/committed/completed/failure events |
| Navigation policy | Canonical AllowedOrigins checked on redirects as well as initial navigation; null permits HTTP(S), empty rejects remote navigation. Only the exact host-requested HTML data URL is admitted internally |
| JavaScript | JSON values, explicit undefined, evaluation/serialization errors; promises rejected. 2 MiB command/result bounds, 16 outstanding operations, 15-second wait limit. Canceling the waiter does not pretend to stop Chromium execution |
| Document lifetime | New navigation invalidates JS immediately; stale generations and late completion rejected; process failure is terminal and requires explicit recreation |
| Profiles | Default per-view unique InPrivate profile; explicit `SharedPersistent` uses `DorotiShared` in the environment user-data directory. Persistent data is not deleted on dispose |
| Explicit clear | Await WebView2 AllProfile completion; additionally clear the fixed registered app origin using Storage.clearDataForOrigin, because runtime AllProfile left that origin's localStorage intact in the product test |
| App content | Registered `doroti-app://content` secure authority scheme; exact manifest paths/MIME, relative CSS/fetch, GET/HEAD and one byte range. 8 MiB per resource, 64 MiB per view. No filesystem route or loopback server |
| Messages | Native top-level WebMessageReceived source origin checked against opt-in MessageOrigins, then version/document/request/name/payload validated; 64 KiB envelope. Payload origin claims are not trusted; subframe handlers are not registered |
| Policies | Popup/external protocols/downloads blocked; permissions denied without profile persistence; no host objects or default script dialogs. App-facing file chooser/download/permission/fullscreen policy APIs remain TODO |
| Input | Existing mouse/capture/shield/focus plus X buttons and touch/pen pointer-info forwarding. OS-injected touch followed by OS mouse and shield checks passed; physical touch/pen, full Tab/IME/UIA and native-origin GestureArena remain unqualified |

The WinRT SDK is used directly; there is no replacement windowed controller.
[Microsoft's CoreWebView2 API](https://learn.microsoft.com/en-us/microsoft-edge/webview2/reference/winrt/microsoft_web_webview2_core/corewebview2)
documents native events and the distinction between navigation completion and
content presentation. The data-clear scope follows
[CoreWebView2Profile](https://learn.microsoft.com/en-us/microsoft-edge/webview2/reference/winrt/microsoft_web_webview2_core/corewebview2profile).

## Composition and evidence

Raster slices continue to use bounded Graphite readback and CPU upload into the
same Windows.UI.Composition tree as live WebView visuals. Native pages are never
replaced by screenshots. Four rectangular isotropic effects, logical sigma <=32,
physical sigma <=128 and saturation 0–2 are supported. Tint remains the independent
common foreground fill. Zero-radius saturation is supported. WinUI island/HWND
mixing remains rejected, and the separate WinUI effect does not advertise saturation.

Native callback invalidation now carries a revision. It also requests a framework
frame with the current input sequence: an otherwise idle retained frame can be
rejected by the existing stale-input guard after a mouse or touch event. That guard
is preserved. This fixed intermittent stale composition evidence after recreation.

Executed on the local AMD Radeon 780M / 200% DPI Windows product:

- Public commands: JSON/Korean text, undefined/null/errors, promise/cycle rejection,
  caller cancellation, stale/late document results, app HTML/CSS/range, trusted and
  untrusted messages, private isolation, explicit shared persistence, storage clear,
  ten create/dispose races and disposal during evaluation.
- Effect/input fixture: two WebViews with intermediate raster, sharp foreground,
  live pixels, OS touch injection, mouse pass-through/shield, effect move, resize,
  identity preservation, last removal, recreation and clean process exit.
- Pixel calibration: native and raster requested sigma 4/16 measured 3.989/15.947;
  reset difference 0; saturation 0 produced RGB (100,100,100) from (180,80,60),
  saturation 2 produced (255,60,20); independent half-blue tint produced (90,40,158).
  These spatial tests do not establish cross-platform pixel equality.
- Common contract and WebView fixtures passed. Release build and self-contained
  publish succeeded; commands/lifetime and pixel calibration also passed from a
  new package directory. A forced Vulkan device reset (one request, one completion)
  passed the effect/input/recreation gate without failed terminals or GPU debug
  errors; this is not real hardware device loss.
- All twelve 0/1/4-view workload combinations exited cleanly. Four-view animation
  recorded raster/readback p95 15.37 ms and UI commit p95 5.09 ms. Some owner-process
  private-memory peaks exceeded 1.2 GB, excluding Chromium/GPU memory. Performance
  remains unapproved; these are bounded observations, not a before/after baseline.
  Idle timings have few startup samples and must not be interpreted as steady-state
  frame-rate percentiles. Reproduction and per-run results are in the
  [Windows validation guide](../../validation/webview/README.md).

Remaining gates are Windows MAUI integration, full focus/Tab/Korean IME/UIA and
physical input, two owners in one product process, full fault/process recovery,
policy APIs, media/protected source coverage, template/clean-machine install,
GPU-sharing comparison and performance acceptance. NativeAOT publish was actually
attempted and rejected by the existing iOS-only `DOROTIAOT002` runner guard; no ILC,
native link or Windows AOT execution occurred. A fresh publish directory on this
machine is not a clean-machine deployment or NativeAOT result.

The dated [execution ledger](../../validation/webview/windows-results-2026-09-19.md)
identifies successful runs and the preserved failures they supersede.
