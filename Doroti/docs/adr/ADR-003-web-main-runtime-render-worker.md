# ADR-003: Main-owned Web runtime and a managed render Worker

- Status: Implemented; automated browser bootstrap, input, and lifecycle checks passed
- Date: 2026-09-08
- Scope: User-authorized replacement of the Testbed threaded-runtime bootstrap

## Decision

The browser document initializes exactly one multithreaded .NET runtime. A
JSWebWorker belonging to that runtime runs the existing direct WebGL role. It
shares the managed/WASM heap and owns the OffscreenCanvas, Skia context, and
current framework frame processing. This is a separation of runtime bootstrap
from the render role, not yet a separation of framework layout from raster.
DOM/input/IME/semantics endpoints remain on the browser main thread.

The host uses the .NET 10 experimental JSWebWorker entry point through one
trim-preserved adapter. Task.Run is not a replacement: it does not install the
JS synchronization context required by synchronous JS interop and GPU work.
No generated runtime files or package-cache files are patched.

The .NET Worker control channel remains runtime-owned. A one-shot, session-token
handshake transfers a dedicated MessagePort to the host. The public Worker
constructor is observed only for same-origin dotnet.native.worker module URLs;
only the matching Doroti handshake is consumed before runtime handlers. All
subsequent Doroti messages, including OffscreenCanvas transfer and exact frame
receipts, use that private port and retain protocol-version validation.

The runtime owns pthread lifetime. Doroti disposes the renderer role on its
owner and lets JSWebWorker finish; it does not terminate a Worker that might
still own a managed stack. A shared-runtime failure is terminal and is not
automatically retried as an independent runtime on a replacement Worker.

## Configuration and constraints

`runtimeLocation: "main"` is a host bootstrap setting, selected by the Testbed.
It requires WasmEnableThreads, cross-origin isolation, and a real shared heap.
Existing single-thread consumers retain their independent Worker bootstrap.
The default renderer is worker-direct-webgpu (Graphite/Dawn); explicit
worker-direct-webgl uses Ganesh/WebGL2. Runtime placement does not add a renderer.

Framework mutation and current Skia rendering remain on the same JS-affine
owner, preserving ADR-002's ownership rules for the combined direct path.
This bootstrap topology does not imply parallel framework layout or a
quantitatively demonstrated performance improvement.

## Evidence and remaining acceptance

The original runtime-in-Worker failure is preserved in
`history/26-09-08/wasm-threads-bootstrap-failure.json` (repository-root path).
A .NET 10.0.11 trimmed Release publish passed first content, resize, wheel,
selection state, and narrow-theme/column restoration checks in Chromium 151.
The document owns one runtime; the render role reports managed thread 8 and
shares its heap. Hardware direct WebGL2 was observed with no runtime errors.
Role disposal and invalid private-port protocol rejection passed separately.
The host marshals timer-triggered JS frame requests to its captured owner
synchronization context and retains DOM endpoints until role disposal completes.

The complete 20-command ledger, including intermediate failures, is recorded in
[the execution report](../../../history/26-09-08/wasm-main-runtime-render-worker.md).
The experimental JSWebWorker reflection boundary is verified for .NET 10.0.11,
including trimming; it must be revalidated when upgrading the runtime.
Quantitative performance improvement, physical-device acceptance, other
browsers, and production hosting headers remain unverified.

## 2026-09-08 work3 follow-up

SkiaSharp was upgraded to 4.154.0-preview.1.26454.9 with the same runtime and
WebGL ownership. A Graphite/Dawn candidate produced Material content on this
Worker, but failed normal shutdown and device-loss acceptance. Its renderer
selection, surface, native exports, and common GPU-cache changes were retired.
The [candidate and execution evidence](../../../history/26-09-08/work3-execution.md)
preserve that initial failure. This historical retirement is superseded by the
subsequent direct-only WebGPU default change; its new validation is recorded separately.

Optional `dorotiLayoutProfile=1` (alongside `dorotiResizeDiagnostics=1`) enables
owner-thread layout self-time instrumentation. It is off by default. The measured
RenderFlex candidate represented less than 0.3% of callback cost and still
included owner traversal, so work3 did not adopt a parallel layout executor.
This diagnostic option neither changes runtime topology nor proves a speedup.

당시 같은 runtime/Worker 소유권을 유지했다. 초기 WebGPU 후보는 수명 gate 실패로 원복했고,
병렬 layout은 순수 계산 비용의 이득 근거가 없어 미채택했다. 상세 결과의 자동
검증과 물리 기기 `notVerified`를 구분한다.

## Numeric layout experiment retired (2026-09-08)

After trying the numeric parallel layout experiment, the user requested its
removal because the benefit was small. The compute engine, dedicated Testbed
root, embedded Material panel, selection options, and dedicated validation have
been removed. Framework layout and raster remain on the render owner; the
main runtime, shared heap, and render Worker lifecycle are unchanged.

The [parallel execution record](../../../history/26-09-08/work3-parallel-execution.json)
and [combination evidence](../../../history/26-09-08/work3-execution.md)
preserve the earlier overlap, geometry, state and pixel results and the failed
combination gates. These describe retired experiment artifacts, not active
product features. General parallel layout and combination work are closed.

## Browser timer ownership (2026-09-08)

The Web runner creates a `BrowserTimeProvider` on its JS-affine owner before
application construction. `PlatformDispatcher` captures that provider and restores
it for every framework callback. Dart `Timer`, `Timer.run`, timed `Future` values,
Future timeout handlers and the shared framework's timed waits use this provider.
Native hosts retain `TimeProvider.System`.

Web deadlines are scheduled with the render Worker's `setTimeout`. Timer change
and cancellation from .NET continuations are posted back to that owner. Generation
checks reject obsolete callbacks. Session shutdown cancels all owned browser timer
handles. Microtasks retain the host wakeup scheduler so nested timers can wake an
otherwise idle view. HTTP font/image requests retain their timeout using a
provider-backed cancellation source rather than HttpClient's internal timer.

This removes Doroti's dependence on the portable .NET TimerQueue that produced
`SynchronizationLockException` in `TimerQueueTimer.Fire`. The upstream runtime
itself is not patched. Application code that creates its own .NET timed operations
should also pass `DartAsyncRuntime.timeProvider`, for example:

```csharp
await Task.Delay(TimeSpan.FromMilliseconds(250), DartAsyncRuntime.timeProvider);
```

The owner provider is execution-context scoped; an unscoped browser Dart timer
fails explicitly instead of silently falling back to the affected .NET timer queue.
The [timer repair record](../../../history/26-09-08/browser-owner-timers.md) preserves
the original failure and separates browser stress/interaction evidence from
physical-device and general runtime acceptance.
