# Android WebView / PlatformEffect contract

2026-09-20. Overall Android work1/work2 qualification is **PARTIAL**.

## Product connection

`AndroidPlatformViewHost.Instance` implements `IPlatformWebViewInstance`. The public
`WebViewController` and `WebViewWidget` use this same native `android.webkit.WebView`,
owner registry, placement/session and GPU retirement path. There is no second browser
instance or separate WebView compositor. Android Graphite uses a native View hierarchy
with bounded raster readback. Ganesh does not acquire this capability implicitly.

The Android target references and runner's transitive Host.Maui reference keep the
selected RID and `net10.0-android` together. This prevents an Android build from using
the stale RID-less `obj/project.assets.json`. AndroidX WebKit is pinned to 1.14.0.1,
which restores with the repository's MAUI dependency versions. The installed WebView
engine is supplied and updated by Android, not bundled with Doroti.

## API and limits

| Function | Android behavior |
|---|---|
| Ready | Native attachment created; not page load or first visible content. If no initial HTML is supplied, no synthetic blank navigation is queued. Load a document before evaluating JS. |
| Navigation | HTTP(S), HTML, back/forward/reload/stop; native URL/title/history/loading and navigation events. No arbitrary file/content/external protocols. |
| JavaScript | JSON values, null, distinct undefined, evaluation/serialization errors. Promise results are rejected. 2 MiB command/result limit, 16 native pending operations, 15-second native wait deadline. |
| Cancellation | Caller/document/close invalidates the result. Cancellation does not stop Chromium execution; its pending slot remains occupied until the native callback. |
| App content | Exact manifest key/path/MIME, preloaded before native creation; 8 MiB per resource, 64 MiB per view. GET/HEAD/single byte range, 404/416 failure. No filesystem or loopback server. |
| Messages | Opt-in `MessageOrigins`; provider `WEB_MESSAGE_LISTENER` required. Native origin and main-frame checks, version/document/request validation, 64 KiB and 128-character name limit. No `AddJavascriptInterface` reflection object. |
| Profile | Default view-specific transient profile requires `MULTI_PROFILE` and `DELETE_BROWSING_DATA`; unsupported providers reject creation. `SharedPersistent` explicitly uses the provider's default profile. |
| ClearData | Provider-acknowledged browsing-data deletion for the selected profile. Unsupported providers return `Unsupported`; cookie-only clearing is not reported as complete storage clearing. |
| Policy defaults | Popup/external protocol denied, permission/geolocation denied, file chooser cancelled, file/content access disabled, mixed content denied, media requires user gesture. |
| Process failure | Terminal `ProcessFailed`; commands are rejected. Recreate the controller explicitly; history/form/media state is not reported as recovered. |
| Input | DirectNative. Framework foreground input uses the committed shield. GestureArena remains explicitly unsupported. |

`LoadAppContentAsync("webview/index.html")` resolves a declared route at
`doroti-app://content/index.html`. Android maps that logical URL to
`https://appassets.androidplatform.net/index.html` and serves immutable manifest bytes
through `ShouldInterceptRequest`. Relative CSS/fetch and native message origins use
that HTTPS origin. JavaScript therefore observes the HTTPS origin, while public
controller URLs use the logical app-content URL. The reserved host never falls
through to network on a missing route. Remote content must not be given that origin.
See Android's [local-content guidance](https://developer.android.com/develop/ui/views/layout/webapps/load-local-content).

Android transient profiles **may write to disk**; they are not WebKit's in-memory
nonpersistent store. On normal disposal, the view is detached/destroyed and profile
browsing-data deletion is awaited. Empty profile shells are removed before profiles
are loaded in the next process. After abrupt process death, cleanup occurs on the
next start. The provider rejects deletion of a loaded profile even after destroying
its WebView; deleting that profile synchronously during disposal was reproduced and
fixed. An acknowledged clear is distinct from secure erasure or an on-disk deletion
fence. See [ProfileStore](https://developer.android.com/reference/androidx/webkit/ProfileStore)
and [WebStorageCompat](https://developer.android.com/reference/androidx/webkit/WebStorageCompat).

Feature flags report what the installed provider and options support. They are not
OS-only claims or evidence of every page/media subtree. Mutable settings, named
persistent profiles and public application callbacks for popup/download/file chooser/
permission/fullscreen are not implemented.

## Effects and input

API 31+ uses a hardware recording Canvas/RenderNode and RenderEffect, sampling only
preceding native/raster Views. One rectangular effect, logical sigma at most 32 and
saturation 0–2 are supported. Tint and the sharp Doroti child remain separate.
Saturation uses common luminance weights (0.2126, 0.7152, 0.0722) and is applied
after blur, including clamped colored edges; zero blur applies color alone.
Sub-half-device-pixel sigma reduces to identity. Protected content and independently
composed SurfaceView/video subtrees are not qualified as sampleable sources.

The public intent is sigma. Android's native implementation converts a blur radius
to `radius * 0.57735 + 0.5`, so the adapter applies the inverse before calling
RenderEffect. Before correction, sigma 16 yielded a 73-pixel 10–90% transition at
scale 3; afterward it measures logical sigma 16.26. Native and raster sources pass
the same spatial calibration. This is a public RenderEffect call with parameter
mapping, not private texture access or cross-window blur. See
[AOSP RenderEffect](https://android.googlesource.com/platform/frameworks/base/+/7d59d89f035a/libs/hwui/jni/RenderEffect.cpp).

The hardware backdrop schedules visible resampling independently of Doroti scene
revisions, preserving live CSS animation and WebView scroll. A hidden/detached effect
does not keep scheduling frames. Raster readback/cache cost is separate from native
source sampling; the configured allocation guards are not accepted performance budgets.

Android pointer identifiers preserve signed native device IDs in a nonnegative
48-bit composite identifier. This covers ADB's device -1 without overflowing the
Flutter converter's signed `long`, and preserves pointer separation. Both Graphite
and the existing Android pointer subscription use this conversion.

## Evidence and remaining gates

Evidence root: `Doroti/artifacts/webview/2026-09-20/android/`.
Reproduction: [Android validation](../../validation/platform-views/android/README.md).
Final outcomes and measured costs: [results](../../validation/webview/android-results-2026-09-20.md).

Galaxy S25, Android 16/API 36, arm64, WebView 151.0.7922.199: public controller
JSON/error/cancellation, native identity, manifest HTML/CSS/Range, trusted/untrusted
messages, private isolation, shared storage, clear, ten create/dispose races and
JS during disposal passed. Product screenshots show live source blur, sharp child,
native click and scroll, Samsung IME Korean composition and Home/resume retention.
These are automated inputs on a physical device, not human physical-input approval.

The final automatic arm64 split installation passed the full public API suite and
12 input/lifecycle assertions. The final x64 Release APK passed the same API suite
on API36/provider133. Removing the unnecessary initial blank navigation corrected
that older provider's delayed finish callback. Emulator software-GPU startup is
slower; live capture waits for a committed native/raster frame rather than a fixed
sleep or native creation alone.

The 11-stage calibration measured sigma 4 as 4.064 and sigma 16 as 16.256, with
native/raster agreement, zero/reset, saturation and independent tint checks passing.
After the final color-order refinement, the x64 emulator passed 13 stages, including
colored-edge analytical agreement and zero-blur reset (sigma 4.050/16.052).
Both final ABI builds passed with zero warnings/errors. Galaxy USB disconnected
before deploying that final refinement; the user explicitly skipped reconnection
and that physical-device color check (`skippedByUser`). Earlier Galaxy API/input
and calibration evidence is tied to the preceding installed revision.

Initial 0/1/4-view measurements found substantial jank when a Doroti progress
animation runs alongside WebView. Some cells in the initial desktop-sized 4-view
fixture were clipped on the phone; the responsive fixture and final measurements
must be used for visible 4-view claims. Android `gfxinfo` is an observation of window
frames, not Graphite submission/physical scanout or WebView input latency. A zero-frame
idle run has no percentile; the Android placeholder 4950 ms is not a measurement.

Remaining acceptance: native-origin delayed GestureArena and nested parent scroll;
full multi-touch/selection/autofill/TalkBack and physical input; two product owners;
real process-loss recovery; media/protected/SurfaceView source coverage; full C1–C6/
E1–E3, wider OS/provider/GPU matrix, memory/latency budgets and alternate GPU/HCPP
strategy comparison; package-only clean template deployment. The Android NativeAOT
publish request is rejected by the current iOS-only runner guard, before ILC/native
link/run. Mono AOT Release is not NativeAOT.
