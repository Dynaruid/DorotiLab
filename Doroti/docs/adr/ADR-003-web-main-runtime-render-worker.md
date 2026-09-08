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
