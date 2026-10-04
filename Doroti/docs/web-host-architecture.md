# Web host ownership and module boundaries

Full-review follow-up: empty `AllowedOrigins` denies controller HTTP(S) navigation; null leaves origin restrictions unset. Iframe internal redirects retain their documented observability limitations. Startup lifetime checks each await; late resources are retired, managed role ports use accept/reject ACKs and cancellation, and disposal errors report fatal with retained resources where consumer completion is unconfirmed. Canvas growth is per-axis and dimension/byte bounded. See [dated evidence](validation/2026-10-04-full-review.md).

`Doroti.Host.Web` keeps DOM services on the browser thread and rendering on the
render endpoint. `doroti.web.ts` is the JavaScript interop facade: its exported
names remain the entry points used by C# and the render Worker.

| Responsibility | Owning module |
| --- | --- |
| Host snapshots, input/focus, semantics projection and Worker supervision | [doroti.web.ts](../src/Doroti.Host.Web/Web/doroti.web.ts) |
| Main-owned .NET render thread and MessagePort connection | [doroti.web.managed-worker.ts](../src/Doroti.Host.Web/Web/doroti.web.managed-worker.ts) |
| Correlated requests, admission, deadlines and caller cancellation | [doroti.web.requests.ts](../src/Doroti.Host.Web/Web/doroti.web.requests.ts) |
| Per-frame raster/capture ownership before transfer | [doroti.web.platform-frames.ts](../src/Doroti.Host.Web/Web/doroti.web.platform-frames.ts) |
| Owner-local DOM composition and iframe commands | [doroti.web.composition.ts](../src/Doroti.Host.Web/Web/doroti.web.composition.ts) |
| Render scheduling and GPU shutdown | [doroti.raster.worker.ts](../src/Doroti.Host.Web/Web/doroti.raster.worker.ts) |
| Source frame admission/transfer and GPU consumer lifetime | [doroti.web.textures.ts](../src/Doroti.Host.Web/Web/doroti.web.textures.ts), [doroti.web.texture-worker.ts](../src/Doroti.Host.Web/Web/doroti.web.texture-worker.ts) |
| File grants, navigation and drop across the thread boundary | [doroti.web.services.ts](../src/Doroti.Host.Web/Web/doroti.web.services.ts) and its service modules |

## Request ownership

`WorkerRequestMailbox<T>` installs the request before sending it, so both a
synchronous response and a synchronous transfer failure settle the same caller.
Unknown, duplicate and retired request IDs do not affect live requests. IDs keep
increasing across `rejectAll()` and endpoint replacement.

| Caller | Pending limit | Deadline |
| --- | --- | --- |
| Worker DOM/service controls | 256 | 30 seconds; file chooser waits for user action or owner close |
| Browser texture controls | 64 | 30 seconds |
| Frame cost diagnostics | 4 | 15 seconds |

Response, failure, timeout and close remove the request and clear its timer.
A texture timeout reports `BrowserTextureError.code = "Timeout"` to the timed-out
caller before disconnecting its owner; other pending requests receive `Disposed`.
Closing rejects remaining callers and prevents new requests. A supervisor can use
`rejectAll()` to cancel an old generation while admitting a replacement.

Worker shutdown first drains capture/texture/GPU work, stops the managed host and
drains PlatformViews. It then closes and drains the control mailbox, including any
remaining chooser request without a deadline. Caller cancellation is not GPU
completion and cannot replace the existing texture fences or GPU shutdown waits.

## Platform frame ownership

`PlatformFrameStager` groups each sealed composition packet with its own captures.
It permits one sealed packet and one commit in flight. A second concurrent commit
or an attempt to overwrite a sealed packet fails explicitly.

Commit awaits only the packet's captures. Captures staged for the next packet do
not delay it. Discard invalidates staged and in-flight packets and closes captured
bitmaps. If one capture fails, siblings that resolve later also close their
bitmaps. The transfer list belongs to the committed packet; send failure, rejected
acceptance and discard during the acceptance wait release its local handles.

The DOM receipt still means `BackendAccepted`, not physical display. Source texture
dimension/adaptive budgets, the separate 64 MiB composition limit, renderer
selection and threaded/main versus standalone/worker ownership remain defined by
their existing modules and [texture contract](web-textures.md).

## Review scope and checks

The 2026-10-04 review found repeated request lifetime code in three channels and
global capture task arrays whose ownership could cross frame boundaries. The new
modules isolate these responsibilities without changing the interop exports or
wire protocol. Input, semantics and supervision still share the facade's host
state; further extraction should preserve focus/input ordering and host identity.

[web_worker_lifecycle.mts](../tests/web_worker_lifecycle.mts) exercises timeout,
shutdown, request pairing and capture/commit races. It runs in `Developer` and
`Release` validation. [web_textures.mts](../tests/web_textures.mts) checks the actual
texture adapter, including timeout cause and timer cleanup. Run both through the
repository's 1200-second wrapper; browser/device coverage is recorded separately
in [the validation record](validation/2026-10-04-web-structure.md).
