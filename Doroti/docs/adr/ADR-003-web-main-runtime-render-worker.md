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
The renderer selection remains worker-direct-webgl; no CanvasKit substitution
is implied by runtime placement.

Framework mutation and current Skia rendering remain on the same JS-affine
owner, preserving ADR-002's ownership rules for the combined direct path.
Future work3.md compute jobs must use immutable inputs and owner-side result
validation. This bootstrap change alone neither implements parallel layout nor
demonstrates performance improvement.

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
including trimming; it must be revalidated when upgrading the runtime. Managed
compute overlap, parallel layout, performance improvement, physical-device
acceptance, other browsers, and production hosting headers remain unverified.

## 2026-09-08 work3 follow-up

SkiaSharp was upgraded to 4.154.0-preview.1.26454.9 with the same runtime and
WebGL ownership. A Graphite/Dawn candidate produced Material content on this
Worker, but failed normal shutdown and device-loss acceptance. Its renderer
selection, surface, native exports, and common GPU-cache changes were retired.
The [candidate and execution evidence](../../../history/26-09-08/work3-execution.md)
preserve that failure; WebGPU is not an enabled product mode.

Optional `dorotiLayoutProfile=1` (alongside `dorotiResizeDiagnostics=1`) enables
owner-thread layout self-time instrumentation. It is off by default. The measured
RenderFlex candidate represented less than 0.3% of callback cost and still
included owner traversal, so work3 did not adopt a parallel layout executor.
This diagnostic option neither changes runtime topology nor proves a speedup.

같은 runtime/Worker 소유권을 유지한다. WebGPU 후보는 수명 gate 실패로 원복했고,
병렬 layout은 순수 계산 비용의 이득 근거가 없어 미채택했다. 상세 결과의 자동
검증과 물리 기기 `notVerified`를 구분한다.

## User-requested numeric layout experiment

The subsequent request to implement a visible parallel version overrides the
profiling-based stop decision for a bounded experiment. `PreparedTreemapLayout`
now computes two immutable, weighted rectangle subtrees on two managed threads
in the same runtime. Its one-batch queue is bounded; readiness and completion
are awaited outside synchronous layout, and shutdown cancels pending work.
The `parallel-layout` Testbed root explicitly owns and disposes this executor.
The owner coalesces resize generations and applies only current results.

Chromium Release validation observed owner 8 and compute threads 11/12, positive
compute overlap, identical serial/parallel coordinates and pixels, resize/seed
continuity, normal render-role shutdown, and zero runtime errors. Existing
Material sample state/resize/theme regression also passed on this build.
This is a specialized numeric layout island, not an automatic parallelization
of RenderFlex, RenderBox callbacks or arbitrary widget trees. General T3–T6,
speedup acceptance and WebGPU combination J0 remain unverified. The earlier G3
failure and T1 cost measurement remain unchanged.

직접 비교: `?dorotiTestbedMode=parallel-layout&dorotiLayoutMode=parallel`.
`dorotiLayoutMode=serial`은 같은 입력과 알고리즘을 owner에서 계산한다.

The user subsequently requested a WebGPU combination. An isolated candidate
adds the same numeric panel to the Material sample with `dorotiParallelLayout=1`.
It fixes missing canvas texture-binding/copy-source usage and switches the
shared numeric executor to Channels so owner submission avoids the synchronous
semaphore API used inside BlockingCollection.TryAdd. The root WebGPU candidate
remains retired. Numeric/thread/state checks passed, but exact combined pixels
and the existing required shutdown gate failed; J0 acceptance remains partial.
See the [combination evidence](../../../history/26-09-08/work3-combined-candidate/README.md).
