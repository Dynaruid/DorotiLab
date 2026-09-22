# Browser source textures

Load the module after the loader reports `started`. Registration belongs to the
canvas/view's render endpoint. Its ID is a **decimal Int64 string**: send only this
control value to C#, parse with `long.Parse(id, CultureInfo.InvariantCulture)`, and
use the existing `Texture(textureId: id)`. DOM/GPU objects never enter C#.

```js
import { texturesForCanvas } from './_content/Doroti.Host.Web/doroti.web.textures.js';
const textures = texturesForCanvas('doroti-surface');
const entry = await textures.registerCanvas(canvas);
entry.onError = error => showSourceError(error.message);
entry.markFrameAvailable(); // explicit; no canvas mutation observer
// Supply entry.textureId to the owning view once. Frame updates need no setState.
await entry.dispose();
```

Canvas uses `createImageBitmap` with premultiplied alpha. The adapter does not take
its drawing context, call `transferControlToOffscreen`, or clear its backing. One
snapshot per entry may be outstanding; dirty requests coalesce. Late snapshot
results close after replacement/disposal and cannot be displayed.

```js
const entry = await textures.registerVideo(video);
// Borrowed video: the application controls play(), pause(), seek, src and audio.
entry.replaceVideo(otherVideo); // same ID, new source generation
entry.replaceCanvas(canvas);
entry.markFrameAvailable();
await entry.dispose(); // does not pause borrowed videos or stop borrowed streams

const producer = await textures.createFrameProducer();
function decoded(frame) { // VideoDecoder output callback
  try { if (!producer.pushFrame(frame)) frame.close(); }
  catch (error) { frame.close(); throw error; }
}
```

`pushFrame` accepts VideoFrame/ImageBitmap. **true gives the entry ownership**,
including while waiting for credit and if a later post fails. Do not use/close it
afterward. false/throw leaves ownership with the caller. ImageBitmap callers should
use `createImageBitmap(source, {premultiplyAlpha:'premultiply'})`; WebGL ignores
unpack alpha transformations for ImageBitmap. Inputs are SDR/sRGB, top-left oriented.
VideoFrame uses browser-defined display dimensions/crop/orientation. Wide gamut,
HDR, protected video and foreign-device GPU objects are outside this contract.

Video requires `requestVideoFrameCallback` and `VideoFrame(video)`. It skips frame
creation without credit, captures ready paused/seeked/ended frames and invalidates
pending frames on `emptied`/source replacement. Missing APIs fail explicitly.
The local MP4 and VP8 WebCodecs sample are opt-in; the latter checks both encoder
and decoder configuration support before creating codecs.

## Camera and local canvas

```js
startButton.onclick = async () => {
  cameraEntry = await textures.startCamera({video:true, audio:false});
  showTexture(cameraEntry.textureId);
};
stopButton.onclick = async () => { await cameraEntry?.dispose(); };
```

Only `startCamera` creates/owns a stream. It stops all owned tracks on disposal,
view loss or setup failure. Registration and ordinary sample startup do not ask
permission or play video. getUserMedia failures retain their DOMException names
(NotAllowedError, NotFoundError, etc.). Media readiness, origin-clean/CORS failure,
unsupported input, transfer failure and owner loss are distinct from renderer
fallback. Source frames stay local: the transport is an in-process MessagePort.

On the **existing render Worker**, after texture initialization:

```js
import { registerLocalCanvas } from './doroti.web.texture-worker.js';
const canvas = new OffscreenCanvas(320, 180);
const ctx = canvas.getContext('2d');
ctx.fillStyle = 'red'; ctx.fillRect(0, 0, 320, 180);
const entry = registerLocalCanvas(canvas);
entry.markFrameAvailable(); // owner-local copy preserves canvas backing
await entry.dispose();
```

This does not move an initialized canvas from another Worker. Other producers use
explicit transferable frames. The testbed has a runnable owner-local example in
`web/src/plugins/texture-local.ts` and a capability-checked WebCodecs example in
`web/src/plugins/textures.ts`. Managed invocation must marshal to the captured
render synchronization context; view event serialization alone does not move a
call to a JS-affine thread.

## Lifetime and backend contract

Protocol v5 adds texture/texture-response/texture-error. The private view endpoint,
process-unique string ID, registration generation, source generation and monotonic
sequence identify input. Stale/malformed transferred frames close in the receiver.
Source errors reach `lastError`/`onError`. ACK returns credit, not presentation proof.

| Resource | Limit / ownership |
|---|---|
| Registrations | 16 per view |
| Dimensions | Positive, within device limit, at most 16 MiB estimated RGBA per source |
| Main source ownership | One transfer and one latest candidate per entry; outstanding snapshots/candidates/transfers share 64 MiB |
| Worker source ownership | One latest pending frame per entry; combined 64 MiB |
| GPU destinations | At most three per entry, 64 MiB combined including retired allocations |
| Composition snapshots | Existing 64 MiB raster-frame limit; one composition packet in flight |

These bounds cover application-owned resources, not decoder/driver allocations or
total process memory. Budget pressure rejects caller-owned frames or retains a
bounded latest input until retirement frees storage. Freeze preserves the displayed
destination, including first-frame admission. Replacement keeps the last displayed
frame until the new source produces one. Resize creates new storage.

WebGPU uses Doroti's current device/queue: copyExternalImageToTexture into RGBA8
COPY_DST/TEXTURE_BINDING/RENDER_ATTACHMENT storage, Dawn native import, CreateDawn,
then SKImage.FromTexture. It creates no second recorder/SkiaGraphiteSession. WebGL
uses the actual owner's image-source upload, registers the texture in Emscripten's
GL table, restores unpack/binding state, resets Ganesh's cache and wraps that GL name.

Source objects close after copy/upload consumes them. SKImages/native wrappers/GPU
destinations remain rooted through last GPU use: queue completion or device loss
on WebGPU, fences or context loss on WebGL. Retirement starts after snap/submit,
including canceled/failed recordings. It does not await completion for every frame.
Shutdown stops admission, closes pending input, unregisters and drains retirement
before destroying the device/context. Loss requires a fresh owner, without stale
handle reuse or automatic backend switching.

Texture-containing PlatformView slices render in the same GPU context and transfer
as ImageBitmaps; main accepts them with bitmaprenderer. Native identity/paint order
are preserved and these mutable slices bypass immutable raster reuse. Existing
non-texture raster transport is unchanged. No source-to-texture path invokes
getImageData/readPixels/copyTo, PNG, Base64 pixels or C# byte-array pixel transport.
Control-only sample strings use the existing plugin control codec. Display captures
are validation only.

`await textures.diagnostics()` schema 1 reports received/accepted/rejected/closed,
dropped/imported/drawn/retired, pending/current/retiring allocations, current/peak
GPU bytes and source generations. Entry counters separately record accepted,
posted, acknowledged and locally closed input. A transferred object is no longer
main-owned; sender and receiver close counts are not two leases on the same object.

Public declarations ship in `types/doroti-textures/index.d.ts`, referenced by
doroti-loader. The existing TypeScript static-asset pipeline packages both input
modules. Hardware decoding, internal browser copy count, zero-copy, sustained
performance and physical presentation latency are not established by this API.

References: [WebGPU external image copy](https://gpuweb.github.io/gpuweb/#dom-gpuqueue-copyexternalimagetotexture),
[WebCodecs ownership](https://www.w3.org/TR/webcodecs/#raw-media-memory-model),
[video callbacks](https://wicg.github.io/video-rvfc/),
[WebGL 2](https://registry.khronos.org/webgl/specs/latest/2.0/).
