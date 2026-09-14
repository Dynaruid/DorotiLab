# PlatformView contracts and current adapters

2026-09-14. This describes implemented source; runtime qualification is separate in `support-matrix.md`.

## Creation and lifetime

`PlatformViewHandle` remains `(OwnerViewId, InstanceId, InstanceGeneration)`. The existing explicit-ID `PlatformViewRequest` is retained. `PlatformViewDescriptor` adds owner allocation, `PlatformPreferred`/`RequireRequested`, and an explicit input policy. Both widget constructors and the legacy channel use the same coordinator. Preferred selection tries the requested composition, then queries NativeOverlay; it does not silently replace a live control with a snapshot. A gesture-arena request is rejected until an adapter implements and qualifies delayed dispatch.

Creation parameter bytes are copied by the coordinator. Placement remains separate in `PlatformViewPlacement`; changing geometry does not recreate the SDK object. Detach preserves identity. Explicit IDs cannot be reused until retirement finishes. The client exposes `DisposalCompletion`; cancelled creation waits for late SDK object recovery during client disposal. Owner close cancels creation and drains native leases. Mutable settings and a public same-owner keep-alive API remain unimplemented.

| Entry | Current path | Boundary |
|---|---|---|
| `PlatformView(owner, request)` | client → coordinator → registered factory | explicit ID, direct native input |
| `PlatformView(owner, descriptor)` | owner allocation and selection → same client/coordinator | no second registry |
| `AndroidView`, `UiKitView`, `AppKitView` | existing `PlatformViewsService` → owner channel adapter | unsupported texture/touch/HCPP messages still reject |
| `PlatformViewLink` / `PlatformViewSurface` | existing supplied controller/render facade | gesture parity is not newly qualified |

## Analysis and frames

`CaptureSnapshot` freezes identities and queried placement capabilities. `PlatformCompositionPlanner.Analyze` only reads scene commands and this snapshot. It does not call a host, retain native objects, or mutate native state. `Admit` rechecks all identities under the coordinator lock and acquires the native leases as one batch. The existing `Build` convenience method performs both steps for migrated callers.

The ordered plan contains raster, native, shield, and effect segments. Unknown coverage is conservative. A picture's cull hint is not a clip. `PlatformBackdropSegment.SampleBounds` expands the output bounds by the kernel extent; the foreground child is outside the sampled background. Backend effect count/sigma limits are supplied by the host. Unsupported group effects and nonrectangular native clips continue to fail explicitly.

Windows, Android, Qt, and AppKit source now calls `PlatformCompositionSession`. Windows/Android/Qt use `CommitRetiredFrame` after their producer's GPU work completes. AppKit supplies its existing prepared-frame retirement. The session bounds pending retirement to three frames and keeps leases after commit failure/cancellation when the producer still has GPU work in flight. Backend acceptance is recorded as `BackendAccepted`; it is not a physical display receipt. Full submit/device-loss/partial-commit fault qualification is still open.

## PlatformEffect

`PlatformEffect` requires bounded layout and clips a backdrop behind its sharp child. It adds no shield, focus node, or semantics action. Use `PointerInterceptor` only where input must be blocked. `PlatformEffectStyle` defaults to MatchCommon, maps strength `[0,1]` to logical sigma `[0,16]`, and paints tint in the sharp foreground. `SolidTint` is explicit and applies no blur. ExactSigma is an opt-in checked by each backend. Common widget saturation other than 1 currently throws rather than silently ignoring it.

Windows `doroti/webview` uses the **WinRT CoreWebView2CompositionController** and one **Windows.UI.Composition** tree for WebView visuals, raster drawing surfaces, and backdrop effect brushes. Environment and Core wrappers are strongly retained for the attachment lifetime; the owner shares the environment across recreated controllers. Raster transfer uses a clipped Graphite atlas, completed GPU readback, and D2D upload. This is a working bounded-upload path, not a zero-copy claim. Legacy BUTTON/EDIT HWNDs remain a separate attachment kind and cannot be sampled by this tree. Mica/Desktop Acrylic is unrelated to the inline effect.

The Windows compositor implementation follows Microsoft's [RootVisualTarget contract](https://learn.microsoft.com/en-us/microsoft-edge/webview2/reference/winrt/microsoft_web_webview2_core/corewebview2compositioncontroller?view=webview2-winrt-1.0.3719.77) and [CreateBackdropBrush](https://learn.microsoft.com/en-us/uwp/api/windows.ui.composition.compositor.createbackdropbrush?view=winrt-26100). Mouse coordinates are mapped into the WebView client, with capture, hover leave, cursor mapping, and committed shields. Pen/touch injection, full Tab/IME/UIA, and capture/modal corner cases are not qualified.

Android includes a native WebView factory alongside Button/EditText. API 31+ uses the existing RenderNode/RenderEffect sampler, with one region and logical sigma up to 32; visible effects invalidate on animation so native content can change without a Doroti scene rebuild. Separate/protected surfaces and cross-platform visual calibration remain unverified.

The Web DOM adapter's batch protocol is **v2**, adding bounded CSS effect placements, source-order validation, explicit CSS availability rejection, pass-through hit testing, and effect cleanup. It is still not connected to the product Worker multi-canvas protocol. CSS syntax support and the DOM harness do not qualify live product composition.
