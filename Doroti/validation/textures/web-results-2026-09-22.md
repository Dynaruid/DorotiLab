# Web source texture implementation and validation — 2026-09-22

**P0–P6 implementation and automated gates: PASS. Physical camera qualification:
notVerified.** These results use the actual published Release product, Windows
Chrome 153.0.8010.48, isolated loopback origins and the installed hardware GPU.
They do not establish physical input/display timing or hardware decoding.

API/lifetime: [Browser source textures](../../docs/web-textures.md).
Evidence root: `Doroti/artifacts/textures/web/`; build logs are in `Doroti/artifacts/`.

## Implemented path

- `Web/doroti.web.textures.ts`: view-bound registry, string Int64 IDs, Canvas,
  borrowed video, owned camera and transferable VideoFrame/ImageBitmap producers.
  One transfer plus one latest candidate, snapshot coalescing, generation rejection,
  source byte limits and late-result cleanup are enforced before/after transfer.
- `Web/doroti.web.texture-worker.ts`: bounded pending input, owner-local
  OffscreenCanvas, real WebGPU/WebGL allocations, diagnostics and GPU retirement.
- `DorotiWebWorkerSurface.Textures.cs`: existing renderer external registration,
  actual Dawn/Ganesh SKImage wrappers, freeze admission and managed image ownership.
  No extra device/recorder or common DOM dependency was introduced.
- `doroti.webgpu.ts`: source copy on the presenter's queue; separate native handle
  release and GPUTexture destruction after completion. Canceled recordings snap
  and dispose their recording before releasing the surface handle.
- Texture-containing interleaved WebView slices use GPU rendering plus transferable
  ImageBitmap/bitmaprenderer. Native iframe identity/order remain intact. Existing
  non-texture raster reuse still passes its regression tests.
- The Texture sample exposes Canvas, Video, Camera, ImageBitmap, VideoFrame,
  WebCodecs, Freeze/Resume, Recreate, Stop, Resize, Update, pause/play and seek.
  Normal startup preserves the CPU sample and never opens media/camera automatically.
  The original responsive preview remains; the opt-in `texture-web` fixture uses
  a stable 320×180 preview for pixel measurements.

Frame wakeups advance the existing renderer texture revision and request raster,
without rebuilding widgets. The protocol is v5; control metadata and transferable
payloads share the existing private endpoint, not a JSON pixel channel.

## Final executed evidence

| Gate | Result / evidence |
|---|---|
| P0 bridge and owners | Both actual GPU contexts produce correctly oriented SKImage pixels; native references retire separately from JS allocation ownership |
| P1 bounded source/transport | `web-texture-js-contracts.log`: 13 assertion groups, plus final summary; synthetic posting failures, slow consumer/burst, stale/foreign generations, snapshot disposal, DOMException identity, source budgets, borrowed video and asynchronous retirement/copy failure |
| P2 WebGPU | `final-gpu/result.json`, `pixels.json`: 15 product/lifecycle checks and **46 pixel checks**, no WebGPU validation failure |
| P3 explicit WebGL | `final-gl/result.json`, `pixels.json`: same **15 + 46** checks, no source upload errors |
| P4 sample/source API | Canvas, VideoFrame, ImageBitmap, local MP4, synthetic camera, owner-local canvas and capability-checked VP8 encode/decode example all render through Texture |
| P5 input and existing behavior | `input-gpu/`, `input-gl/`: real sample Canvas/Freeze/Resume buttons, pixel holds/updates, no rebuild during production, CDP pointer and synthetic Korean text into overlapping iframe |
| P5 DOM regression | `dom-regression/`, `web-texture-dom-regression.log`: **18 PASS**, two independent DOM owners, stale identities/receipts, retained CPU raster pixels/geometry and primary canvas restoration |
| P5 device/context loss | `final-gpu-loss/`, `final-gl-loss/`: four checks each, all imported images/source frames returned, no unhandled exception, stale owner rejects new registration |
| Common CPU regression | `web-texture-cpu-tests.log`: **29 assertions PASS** plus final summary |
| Common native regression | `web-texture-native-contracts.log`: **25 assertions PASS** (8 ownership + 6 registration + 11 cross-platform lease contracts) |
| P6 publish/package | Release publish and host NuGet pack succeed; `package-check.json` verifies modules, public declarations against pinned emitter, sample media/modules and native release exports |
| Formatting/build | CSharpier checks eight changed C# files; strict TypeScript and managed builds pass; `git diff --check` passes |

Both final source runs used Chrome 153.0.8010.48 with `crossOriginIsolated=true`.
WebGPU reported AMD `rdna-3`, hardware=true; WebGL reported ANGLE / AMD Radeon 780M
/ D3D11, hardware=true. Neither used software fallback or changed backend.
Exact response hashes and browser build are in each run's `assets.json`.
`source-provenance.json` ties the worktree, publish tree and package to this report.

| Final run | received / closed | imported / retired | live / pending | peak owned GPU bytes |
|---|---:|---:|---:|---:|
| WebGPU `final-gpu` | 300 / 300 | 301 / 301 | 0 / 0 | 691,200 |
| WebGL `final-gl` | 300 / 300 | 302 / 302 | 0 / 0 | 691,200 |
| WebGPU injected loss | 8 / 8 | 7 / 7 | 0 / 0 | 691,200 |
| WebGL injected loss | 11 / 11 | 11 / 11 | 0 / 0 | 691,200 |

Local OffscreenCanvas copies do not receive a transferred frame, so imports and
received counts need not match. The two relevant invariants are received=closed
and imported=retired after teardown. Source objects and GPU destinations are
different resources. Reported byte counts exclude browser decoder/driver memory.

Pixel checks cover quadrant orientation, premultiplied alpha for Canvas/bitmap/frame,
clip, transform, opacity, scaling filter, size changes, first frozen frame, freeze
hold, latest resume, retained RepaintBoundary updates, moving source pixels,
pause/seek, preserved local-canvas backing, WebCodecs output, interleaved native
paint order, and DPR 1.5/2. Browser lifecycle freeze/active uses explicit CDP focus
emulation to restore visibility; it is synthetic lifecycle evidence.

Camera checks use Chrome's **fake media device**, grant permission only for the
test origin, and verify owned tracks end on Stop. A denied permission stays denied;
this Chrome build returns either NotAllowedError or NotFoundError depending on
device enumeration timing. The adapter preserves that original browser failure.
For a tainted Canvas, Chrome creates the ImageBitmap but rejects its transfer with
DataCloneError (`Non-origin-clean ImageBitmap cannot be transferred`). This is
preserved as a typed error, and local ownership/budget is returned. It is not
misreported as a GPU failure or silently converted to pixels.

## Issues found and resolved

- Mixed native scenes previously went through CPU SKBitmap rasterization. GPU
  texture slices now use ImageBitmap transport; immutable slice reuse cannot keep
  an old texture forever.
- Producer unregister during loss/shutdown could admit a new presentation lease
  after the shutdown sweep. Late GPU raster snapshots also need draining/cancellation before owner teardown. The Worker now rejects new frame admission once
  disposing/disposed/context-lost. The reproduced loss test then passed.
- Browser DOMException names were lost when normalizing non-Error rejections.
  Snapshot and transfer errors now preserve their browser identity/code.
- Sample export invocations must post to the captured render synchronization
  context. `DispatchPlatformEvent` serializes a view event but does not itself move
  a main-thread call to the JS-affine render owner.
- A linked MP4 had no usable dev-server content root. The Web sample now packages
  its local static MP4 (identical to the existing native fixture's test clip).

Early failed/interrupted harness runs remain under the artifact root. Windows
Python's default `.mjs` MIME type was unsuitable; `serve-web.py` explicitly serves
module/WASM types. Headless lifecycle activation required focus emulation before
video could resume. These are not accepted product results; the final evidence
directories above are the authoritative executions. Input/DOM checks precede only the final late-snapshot cancellation guard; the final 15+46 and loss cases were rerun after that guard, and a deterministic late-capture contract verifies canceled packet cleanup.

## Limits and outstanding qualification

- **Physical camera: notVerified**. No real sensor video was captured in this Web
  run; operator-visible permission UI, sensor orientation/mirroring, unplug/replug,
  and physical display/input acceptance have no evidence. Virtual camera success
  does not close those gates.
- **Default Debug runtime startup: historical FAIL; fixed in the follow-up**.
  `gpu-2/events.json` records `mono-threads-wasm.c:201` and Invalid UTF-8 before
  managed product startup. Release is independently verified; no runtime guard,
  renderer fallback or CPU conversion was used to hide the Debug failure.
  The subsequent [Debug startup correction and validation](../web-debug-startup-2026-09-22.md)
  closes this startup failure with 15 execution and 46 pixel checks per backend;
  the original failure evidence remains intact.
- Native InsertRecording/Submit error codes were not individually fault-injected.
  Real device/context loss and synthetic failed-copy/queue retirement are separate
  evidence. Sustained performance, power, driver-resident memory, hardware decode,
  zero-copy and physical scan-out latency remain unmeasured.
- Apple/Linux/native app execution is outside this Web change's required scope.
  No native producer/importer implementation was changed. Other browser engines,
  HDR/protected content and foreign-device texture sharing are not qualified.

## Reproduce

From the repository root, with a new output directory for each browser run:

```powershell
python Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -p:RunAOTCompilation=false -o Doroti/artifacts/textures/web/publish
python Doroti/validation/textures/serve-web.py Doroti/artifacts/textures/web/publish/wwwroot 5089
# In another shell; run the browser cases sequentially:
python Doroti/validation/run-with-timeout.py node Doroti/validation/textures/web-contracts.mjs
python Doroti/validation/run-with-timeout.py node Doroti/validation/textures/verify-web.mjs <output> http://127.0.0.1:5089 worker-direct-webgpu
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/analyze-web.py <output>
# Repeat the previous two commands with a new output and worker-direct-webgl.
python Doroti/validation/run-with-timeout.py node Doroti/validation/textures/verify-web-loss.mjs <loss-output> http://127.0.0.1:5089 worker-direct-webgpu
python Doroti/validation/run-with-timeout.py node Doroti/validation/textures/verify-web-input.mjs <input-output> http://127.0.0.1:5089 worker-direct-webgpu
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/analyze-web.py <input-output> --input
# Run the loss/input cases for WebGL as well.
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/textures/Textures.csproj
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/textures/NativeContracts/NativeContracts.csproj
python Doroti/validation/run-with-timeout.py dotnet pack Doroti/src/Doroti.Host.Web/Doroti.Host.Web.csproj -c Release --no-build -o Doroti/artifacts/textures/web/packages
python Doroti/validation/run-with-timeout.py C:/Users/parti/.nuget/packages/microsoft.typescript.msbuild/7.0.0/tools/runtimes/win32-x64/tsc.exe --project Doroti/src/Doroti.Host.Web/Web/tsconfig.json --declaration --emitDeclarationOnly --outDir Doroti/artifacts/textures/web/types-final
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/verify-web-package.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/capture-web-provenance.py
git diff --check
```

The wrapper imposes the repository's 1,200-second process-tree test deadline.
Browser screenshots are display captures used only by the validation scripts;
they are not an alternate input/production pixel transport.
