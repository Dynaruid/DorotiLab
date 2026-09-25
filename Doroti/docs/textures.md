# Texture support

2026-09-22: **CPU textures and Android/Windows native GPU input implemented and exercised.**
iOS/macOS Metal and Linux DMA-BUF/camera adapters are implemented in source only;
their platform builds and execution are **notVerified**. Web Canvas/video/frame GPU
input is implemented on WebGPU and explicit WebGL. See [Browser source textures](web-textures.md)
and [Web validation](../validation/textures/web-results-2026-09-22.md) for the API,
executed checks and separate physical-camera/Debug-runtime limits.

## Existing implementation and completed connection

`Doroti.Framework.Widgets.Texture` already created a `TextureBox`, which emitted a
`TextureLayer`. Previously `SceneBuilder.addTexture` only recorded an anonymous
payload, and `SkiaSceneRenderer.DrawScene` had no texture drawing branch. There was
no producer registry. The widget alone could not display external frames.

The connected path is now:

```text
producer → TextureEntry.PushFrame → view-owned TextureRegistry
                                       ↓ request native raster
Texture → TextureBox → TextureLayer → SceneTexturePayload → Skia DrawImage
```

`graphics.texture` is an optional typed capability registered by Windows App SDK,
MAUI, Web and Qt hosts. The shared renderer owns the registry and closes it on
disposal. Existing PlatformView placement and input routing remain separate.

This follows Flutter's [Texture widget model](https://api.flutter.dev/flutter/widgets/Texture-class.html):
integer registration IDs, parent-defined dimensions, filter quality, freeze, and
producer-driven repaint without a widget rebuild. Flutter also provides native
[platform texture producers](https://api.flutter.dev/javadoc/io/flutter/view/TextureRegistry.html);
Android's native producer entry now accepts Camera2, MediaCodec and MediaPlayer
output Surfaces. Other hosts fail explicitly when asked to create a native surface.

## Usage

```csharp
// Create once for the owning view, for example in didChangeDependencies.
var entry = TextureRegistry.ForView(View.of(context)).CreateTexture();

// Place in a bounded layout. No setState is needed for subsequent PushFrame calls.
var widget = new SizedBox(
    width: 640,
    height: 360,
    child: new Texture(textureId: entry.Id, freeze: false)
);

// sRGB, premultiplied RGBA8888. The caller may reuse pixels after this returns.
entry.PushFrame(pixels, width: 640, height: 360, rowBytes: 640 * 4);

// Stop the producer and unregister when its owner is disposed.
entry.Dispose();
```

Namespaces: `Doroti.Ui` and `Doroti.Framework.Widgets`.
The Material sample's **Texture** tab publishes a moving pattern at approximately
30 Hz and exposes Freeze/Resume. Its timer publishes frames without calling
`setState`; only the Freeze button changes widget state.

## Android native Surface usage

```csharp
// Android 13/API 33+, Graphite/Vulkan with AHB and foreign-queue extensions.
// Select dimensions supported by the camera/decoder (the camera fixture uses 640x480).
var entry = await TextureRegistry.ForView(View.of(context))
    .CreateSurfaceTextureAsync(width: 640, height: 480, cancellationToken);

// Android consumer code: the Surface is borrowed from the entry.
var surface = (Android.Views.Surface)entry.Surface;
mediaPlayer.SetSurface(surface); // or Camera2 request.AddTarget(surface)
var widget = new Texture(textureId: entry.Id);

// Stop and release the camera session/decoder/player first.
entry.Dispose();
```

Android host applications may cast to `Doroti.Host.Maui.AndroidSurfaceTextureEntry`
for the typed `AndroidSurface`, `Error`, received/drawn frame counters and
view-wide imported/retired GPU counters. The sample application requests camera
permission; the rendering library itself does not request permission or open a camera.

```text
Camera2 / MediaCodec / MediaPlayer / hardware Canvas
  → Surface / SurfaceTexture external OES buffer
  → GLES shader conversion, using the producer transform
  → RGBA ImageReader / AHardwareBuffer
  → acquire fence → Vulkan foreign-to-renderer ownership barrier
  → Graphite SKImage → Texture scene composition
  → renderer-to-foreign barrier → GPU completion fence → release source lease
```

The pinned public `SKGraphiteVkTextureInfo` exposes no external-format/YCbCr
conversion descriptor. The GLES step supports the camera/decoder's external OES
buffer and converts it into an importable RGBA allocation without reading pixels
to CPU memory. This is **one GPU conversion pass**, not an assertion of no-copy
direct YUV sampling. No CPU `PushFrame`, bitmap readback or CPU pixel upload is
used by this native path. Diagnostic screenshots are separate OS display captures.

The producer lives on its own HandlerThread/EGL context. Its output acquire fence
has a one-second bounded wait there. Vulkan imports the acquired RGBA buffer,
validates its dimensions/layers/format/usage, and retains the Android Image and
HardwareBuffer until the same-queue completion fence finishes. The current
conservative implementation waits for that fence before returning from a native
texture frame and creates an import wrapper per frame; it does not promise
asynchronous retirement, a persistent import cache, or sustained 60/120 Hz pacing.
Cancellation releases acquired ownership after draining submitted work. Device
loss closes GPU use before releasing leases. Reader destruction is deferred until
all source leases retire. The caller must stop its native producer before disposal.

Current native scope is SDR RGBA8/sRGB, Android API 33+, and non-protected content.
HDR/wide-gamut qualification, DRM/protected surfaces, camera sensor rotation/front
camera mirroring policies, Android 26–32 fence adapters and physical-device tests
remain outside this implementation. Camera orientation/mirroring is consumer
policy; the adapter applies the SurfaceTexture matrix, including source crop/flip.

The external-source adapter (`ISkiaExternalTextureSource` and
`SkiaExternalTextureRegistration`) allows further native backends to reuse the
same texture IDs, retained scene resolution, invalidation and freeze flag.
GPU completion and buffer ownership remain the backend's responsibility.

## Ownership and rendering rules

- Each registration retains at most one pending frame and one last acquired frame.
  New pushes replace the pending frame. Memory is proportional to image size and
  number of live registrations; there is no global byte budget yet.
- Input rows may include padding. Width/height, stride, buffer length, and scene
  bounds are validated before raster access. Source storage is copied on push.
- Producer publication and entry disposal are serialized with acquisition. A draw
  retains a separate image reference, so unregister cannot free its active image.
  The host's invalidation hook is called outside the registry lock. When a renderer
  owns a SynchronizationContext (including browser interop), producer notifications
  are coalesced and posted to that context; queued callbacks become inert on disposal.
  Context-free hosts use their thread-safe native invalidation hooks.
- Unknown, foreign-registry and released IDs draw nothing. IDs are never reused
  during normal process lifetime, so an old retained scene cannot select a new entry.
- `freeze` holds the last acquired frame; if none exists, the first pending frame
  can initialize it. Resume acquires the latest pending frame. The last acquired
  frame belongs to the registration: separate widgets using the same ID share it.
  Independent frozen/live playback requires separate registrations.
- Transforms, clips, opacity and filters use the normal scene composition scopes.
  Retained layers resolve textures again during raster. Platform raster snapshot
  reuse rejects texture commands. Fragment-image-filter input caching is disabled
  for scopes containing textures, including retained descendants.
- CPU images survive GPU context replacement and can be uploaded again. Skia owns
  resources retained by its recorded draws. This contract does not authorize
  reusing an external native allocation after `DrawImage` or host submission.
- `PushFrame` is a CPU copy/upload path. It does not accept Vulkan images, D3D
  shared handles, AHardwareBuffer, CVPixelBuffer, WebGL textures or WebGPU textures.
  It currently supports RGBA8/sRGB only, with no YUV/HDR conversion or timestamps.

## Windows, iOS/macOS and Linux native frames

All three use a separate native registration:

```csharp
var entry = TextureRegistry.ForView(View.of(context)).CreateNativeTexture();
var widget = new SizedBox(width: 640, height: 360,
    child: new Texture(textureId: entry.Id));
// Publish from a producer callback/thread; this retains a separate ownership lease.
using (NativeTextureFrame frame = GetProducerFrame())
    entry.PushFrame(frame);
// Stop/await the producer first, then entry.Dispose().
```

`GetProducerFrame` above represents one of the platform factories below.
`NativeTextureFrame` carries immutable metadata and an exactly-once final release
callback. It retains the camera/decoder pool lease as well as the OS allocation.
A duplicated handle/fd by itself does not stop a producer from recycling pixels.
`PushFrame` retains the caller's frame and replaces only the pending frame. Freeze
retains the displayed frame while pending frames continue to be replaced.

Each frame belongs to one view/renderer consumer. Concurrent Graphite recordings
on that consumer share one imported image, and the final recording releases it
only after its host GPU completion fence/Metal command buffer completes. Canceled
recordings drop their resources after the discarded recording is destroyed.
A Vulkan release timeout retains the allocation rather than authorizing producer
reuse. IDs never encode pointers and are not reusable across views.

### Windows App SDK

```csharp
using Doroti.Host.WindowsAppSdk;
// Camera adapter: selects the renderer's DXGI adapter, requests GPU samples.
var camera = await WindowsGpuTextureCamera.StartAsync(entry);
// Observe camera.Completion for terminal errors and camera.PublishedFrames for progress.
// ...
await camera.DisposeAsync();
entry.Dispose();

// For an existing camera/decoder callback (producer-ready ID3D11Texture2D*):
using var frame = WindowsGpuTextureFrames.FromD3D11Texture(texturePointer, arraySlice);
entry.PushFrame(frame);
```

The built-in Media Foundation adapter uses a DXGI device manager and requires
`IMFDXGIBuffer`. The generic factory copies BGRA, or GPU-converts NV12/YUY2/RGBA,
to an immutable BGRA shared D3D11 KMT allocation. YUV conversion currently assumes
SDR BT.709 limited range; HDR/other colorimetry must be converted by the producer.
It waits for the D3D11 producer query before publishing. Vulkan checks the adapter
LUID and external format support, acquires external queue ownership, and samples
through Graphite. Retirement returns ownership before releasing the D3D allocation.
The product output remains **Graphite/Vulkan → D3D12/DXGI**.

The native input path has no CPU pixel readback/upload. It has a GPU copy/conversion
and bounded CPU fence waits; it is not a zero-copy or nonblocking performance claim.
The opt-in Windows testbed mode is `DOROTI_TESTBED_MODE=texture-native`; set
`DOROTI_TEXTURE_CAMERA=1` for the camera, otherwise it uses a D3D11 GPU pattern.
Camera frames stay local. Shutdown closes the camera; the test records local
screenshots under ignored `Doroti/artifacts/textures/windows/`.

### iOS / macOS / Mac Catalyst — source only

```csharp
using Doroti.Host.Maui;
var camera = await AppleGpuTextureCamera.StartAsync(entry);
// Inspect camera.Error and camera.PublishedFrames. Stop before disposing entry.
await camera.DisposeAsync();
entry.Dispose();

// Existing AVFoundation/VideoToolbox producer: request BGRA, Metal-compatible IOSurface output.
using var frame = AppleGpuTextureFrames.FromPixelBuffer(pixelBuffer);
entry.PushFrame(frame);
```

AVFoundation requests BGRA IOSurface/Metal-compatible buffers. The factory retains
`CVPixelBuffer`; the renderer's own Metal device creates a `CVMetalTextureCache`
view, and the Graphite frame owns that view until command-buffer completion. The
callback uses no `CVPixelBufferLockBaseAddress` or pixel upload. The camera adapter
serializes delivery, discards late frames and drains callbacks during shutdown.
Access is requested only by an explicit `StartAsync` call.

Applications need `NSCameraUsageDescription`; sandboxed macOS/Catalyst applications
also need `com.apple.security.device.camera`. The testbed plists include those keys.
No camera is opened by registration alone. YUV-plane import, color attachments,
rotation/mirroring, HDR and Apple execution remain unqualified. Ganesh paths do
not expose this native factory. **No Apple target build or run was performed.**

### Linux Qt — source only

```csharp
using Doroti.Host.Qt;
using Doroti.Skia.Vulkan;
// The caller owns a pool lease until releaseProducerLease is invoked.
// acquireSyncFileFd is borrowed. Omit it to export/wait the DMA-BUF write fence.
using var frame = LinuxGpuTextureFrames.FromDmaBuf(
    new LinuxDmaBufTextureBuffer(width, height, NativeTextureFormat.Bgra8888,
        fd, allocationSize, offset, rowPitch, drmModifier),
    releaseProducerLease, acquireSyncFileFd);
entry.PushFrame(frame);

// Optional camera/video appsink adapter. Observe Completion for negotiation/driver errors.
var camera = await LinuxGpuTextureCamera.StartAsync(entry, pipelineDescription);
await camera.DisposeAsync();
entry.Dispose();
```

The factory duplicates the DMA-BUF fd, waits a sync-file (explicit or exported via
`DMA_BUF_IOCTL_EXPORT_SYNC_FILE`), and transfers ownership only on success. The
caller remains responsible for its pool lease after a failed factory call.
Vulkan checks the format/modifier and imports the single plane using its explicit
offset/stride. Foreign queue ownership returns only after renderer work completes;
therefore the producer needs no asynchronous release-fence handoff in this version.
No DMA-BUF mapping or CPU conversion is provided.

QWindow enables the four required extensions when available. For Qt Quick set
`DOROTI_LINUX_NATIVE_TEXTURES=1` **before startup**; the updated native shim requests
`VK_KHR_external_memory_fd`, `VK_EXT_external_memory_dma_buf`,
`VK_EXT_image_drm_format_modifier`, and `VK_EXT_queue_family_foreign` before Qt
creates the device. A native feature bit prevents older shims from advertising
the new configuration. The importer requires all four on the selected physical
device. Unsupported formats/devices fail explicitly.

Build the optional camera adapter with `-p:DorotiGStreamerTextures=true`, or CMake
`-DDOROTI_GSTREAMER_TEXTURES=ON`. GStreamer 1.24+ core/app/video/allocators development
packages are required; the runner copies `libdoroti_texture_gstreamer.so` to the
build/publish directory. Other applications keep this dependency disabled.

Set `DOROTI_GSTREAMER_PLUGIN_DIR` to a dedicated directory of reviewed plugins
before opening a camera. The adapter must initialize GStreamer first, restricts
its process-wide search paths, and checks plugin names/licenses. See the native
[plugin policy](../../DorotiTestbedApp/linux/native/README.md#license-policy-and-optional-gstreamer)
for the allowlist. libav/x264/x265 and automatic playback are not enabled. Review
the actual plugin binaries and their transitive dependencies before distribution;
the runtime checks do not certify a vendor's build.

The caller-supplied local pipeline must end with `appsink name=doroti_texture`,
negotiating `video/x-raw(memory:DMABuf),format=DMA_DRM`. Only single-memory,
single-plane DRM `AB24` (RGBA) or `AR24` (BGRA), with `GstVideoMeta`, is accepted.
A camera's NV12/YUY2 output needs a GPU conversion/export stage first (for example,
a supported GStreamer GL or VA driver pipeline). There is deliberately no universal
pipeline string: GPU DMA-BUF export capabilities/modifiers vary by camera and driver.
The adapter preserves each `GstSample` pool lease, bounds appsink queuing to two
samples and rejects CPU memory. The adapter build and plugin-policy initialization
paths are exercised by `validation/license-policy/verify.py --build-gstreamer`.
**Actual camera/GPU DMA-BUF negotiation remains unverified.**

### Remaining qualification

Web native `VideoFrame`/GPU inputs, multi-plane YUV/HDR/protected content, automatic
sensor orientation, hot-unplug/device-loss recovery, sustained pacing/power/memory
stress, and Apple/Linux native execution remain separate work. Source adapters
accept camera/decoder buffers; this does not implement a cross-platform video
player or prove every driver supplies GPU-backed camera frames.

Platform contracts: [Media Foundation D3D11 buffers](https://learn.microsoft.com/en-us/windows/win32/medfound/supporting-direct3d-11-video-decoding-in-media-foundation),
[Core Video Metal cache](https://developer.apple.com/documentation/corevideo/cvmetaltexturecache),
[GStreamer DMA-BUF negotiation](https://gstreamer.freedesktop.org/documentation/additional/design/dmabuf.html),
[Qt device extension setup](https://doc.qt.io/qt-6/qquickgraphicsconfiguration.html#setDeviceExtensions).

## Validation

Run from the repository root; the wrapper enforces the required 20-minute timeout:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/textures/Textures.csproj
```

The executable verifies pixels, producer-buffer ownership, retained scenes,
freeze/resume, padded stride, resized frames, transform/clip/opacity, invalid
input, native-raster invalidation, replay without a new scene submission, context
reset, foreign IDs, unregister/disposal and overlapping producer/render activity.
This common executable uses a CPU Skia surface. Android GPU and native-source
checks are separate; see [Native texture validation](../validation/textures/README.md).
Physical devices and sustained performance remain `notVerified`.

Results on 2026-09-22:

| Check | Result |
| --- | --- |
| Texture fixture | PASS, 29 assertions |
| Existing platform raster motion fixture | PASS, 34 assertions |
| Material sample application build | PASS, zero warnings/errors |
| Windows App SDK host build, win-x64 | PASS, zero warnings/errors |
| Web host managed/TypeScript build | PASS, zero warnings/errors |
| Qt managed host build | PASS, zero warnings/errors; Linux native runtime not exercised |
| MAUI host build, net10.0-android / android-arm64 | PASS, zero warnings/errors |
| Android native GPU input | PASS: native ownership/registration contracts, emulator canvas lifecycle, Camera2 and MediaPlayer; see the linked validation report |
| Windows native D3D11 input | PASS: AMD/NVIDIA GPU pattern lifecycle and AMD real camera; Vulkan/D3D12 errors 0 |
| Apple/Linux native adapters | Source configured only; target build/run notVerified |
| CSharpier and git diff whitespace checks | PASS |
