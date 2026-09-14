# Android PlatformView product validation

Run from `Doroti/`, with the installed Testbed APK matching the current sources:

```powershell
python validation/run-with-timeout.py dotnet build ../DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -m:1 -p:AndroidPackageFormats=apk
python validation/run-with-timeout.py adb -s SERIAL install -r ../DorotiTestbedApp/android/bin/android-arm64/Release/net10.0-android/android-arm64/dev.doroti.testbed-Signed.apk
python validation/run-with-timeout.py python validation/platform-views/android/record.py --serial SERIAL
```

For the x64 emulator, replace both RID path components and the build RID with
`android-x64`. `--overlay` selects the restricted B fixture; `--capture-only`
captures startup without claiming input or lifecycle success. Each invocation
accepts `--lifecycle-only` to reuse passing overlap evidence while checking
editor input, modal, recreation, rotation and resume. Each full invocation
creates a new dated artifact directory and records commands, source hashes,
device identity, screenshots, accessibility trees, process log and result.
Every child process has a 1200 second timeout. ADB taps/typing are automated
device input; they are not physical touch, keyboard or IME qualification.

The manifest opts into `doroti/native-button` and `doroti/native-editor` for
each Android RID. The Graphite handler owns a FrameLayout containing its
SurfaceView, live clipped native Views, raster Views and transparent input
shields. Native instances belong to the owner coordinator, independently of
Vulkan surface generations. Only translation and rectangular clipping are
advertised; arbitrary SurfaceView-producing controls and WebView are excluded.

The default C path records Doroti raster segments with the active Graphite
recorder into an RGBA atlas. The first raster uses the original SurfaceView
directly, and B needs no readback. Empty raster slices are removed. Independent
pictures are split only outside group effects, cropped using enforced scene
ClipRect bounds, and cached by immutable snapshot/scope content, viewport and GPU
epoch. The 32×32 logical-pixel spinner has an inside-aligned stroke, 2px logical
padding and its own 36×36 repaint/clip region, preserving full stroke edges.
Adjacent unbounded pictures remain grouped to avoid multiplying full-screen
buffers. Stable Android raster/shield Views retain their identities and update
only bitmap content when geometry and ordering are unchanged.
After the same-queue copy fence and Graphite
readback completion, the UI thread commits native geometry and sibling paint
order. The ordinary SurfaceView remains below the window. Native content is
not copied into CPU bitmaps, hidden because of raster occlusion, or recreated on surface
loss. The scoped backdrop path below replays preceding live View drawing on HWUI.
A screenshot from `adb exec-out screencap -p` includes native content;
the Graphite raster capture does not. This is an explicit readback compositor,
not SurfaceControl/HCPP, external texture import or a CPU renderer fallback.

Native operation reservations cover the complete batch. A removed instance in
a replayed scene or contended reservation supersedes the frame and retries;
failed/stale frames do not publish a new native placement. Bitmap preparation
precedes native attachment. GPU completion and queue-present acceptance are
distinct from Android window presentation; physical display atomicity is not
advertised. Resource accounting allows at most 256 MiB for the GPU atlas,
packed readback/scratch and two bitmap banks, and 16384 pixels per atlas axis.
HWUI/driver caches are outside that estimate. Frame budget, allocation pressure
and long-run native/HWUI memory remain separate performance gates.

Shields use Android's ordinary touch target retention for each gesture and
forward local coordinates to the Doroti SurfaceView once. Raster Views pass
touches through. Exposed controls use their real Android handlers. Tab is
forwarded to framework traversal, native focus preserves the descendant when
the framework activates its owner, and native semantics placeholders are
removed from the virtual Doroti tree. Full parent-scroll gesture arbitration,
multi-touch/capture changes during a gesture, Korean IME and TalkBack remain
separate acceptance gates.

Reference constraints: [Android SurfaceView ordering](https://developer.android.com/reference/android/view/SurfaceView#setZOrderOnTop(boolean))
and [ViewGroup touch dispatch](https://developer.android.com/develop/ui/views/touch-and-input/gestures/viewgroup).
The shared readback stride follows the packed array returned by
[SkiaSharp SKImageReadPixelsResult.ToArray](https://github.com/mono/SkiaSharp/blob/main/binding/SkiaSharp/SKImageReadPixelsResult.cs),
which strips the native transfer-buffer padding.

Measure both implementations in the same installed APK:

```powershell
python validation/run-with-timeout.py python validation/platform-views/android/benchmark.py --serial SERIAL
```

This records baseline then optimized stationary spinner workloads (3 seconds
warmup, 8 seconds each), per-frame owner/Vulkan/paint/fence timing distributions,
accepted frame counts, raster byte deltas, captures and memory snapshots. It is
not an optical scanout or input-latency measurement. The fixture recorder also
accepts `--raster-mode baseline` for comparison; optimized is the product default.
`spinner-touch.py --serial SERIAL --output NEW_ARTIFACT_DIRECTORY` checks that
the spinner passes one real ADB tap through to its native button in the current C
fixture. Continuous view recreation caused UIAutomator idle detection failures
during development; the optimized compositor now retains those View identities.

## Red backdrop and checkerboard in the Material sample

`PlatformViewFixture` is shared by the isolated fixture and Material sample's
`Platform views` destination. Its background is a retained 20px gray checkerboard.
Only the red rectangle has a clipped sigma-6 backdrop; the green layer stays sharp.
Android 31+ C factories advertise `NativeBackdropBlur`. Earlier Android and other
hosts retain the rectangle without requesting unsupported native sampling.

The planner opts into one rectangular, ungrouped Gaussian region per frame, with
clamp tiling, srcOver and sigma in (0,32]. Default callers, B, complex filters,
grouped effects and native children inside the backdrop remain rejected.
Android replays earlier raster/native Views into a hardware RenderNode, applies
RenderEffect, clips its output to the red bounds, then draws the sharp red tint.
It also caches the first Doroti raster in the sample ROI because SurfaceView
pixels cannot be sampled through ordinary View drawing. Sampling includes a
3-sigma halo, bounded to the viewport and 4096px / 64 MiB. Native edit invalidation
refreshes the effect; native controls keep their identity and input behavior.
This is additional HWUI work, outside the earlier no-blur spinner benchmark.

```powershell
python validation/run-with-timeout.py python validation/platform-views/android/backdrop.py --serial SERIAL --output artifacts/platform-views/DATE/android/backdrop/NEW_RUN
```

This launches `sample`, taps `Platform views`, compares blur disabled/enabled in
the same APK, checks checker/green pixels, native editing and effect refresh,
spinner motion/tap, red input shielding, navigation and dispose/recreate.
The `doroti_platform_view_backdrop=0` launch extra is an explicit A/B probe;
ordinary product launches enable the effect on supported Android C hosts.
Use `spinner-clip.py` for stroke thickness across rotation; it varies sampling
intervals to avoid repeatedly observing the same arc gap.

References: [RenderNode hardware recording](https://developer.android.com/reference/android/graphics/RenderNode)
and [RenderEffect blur, API 31](https://developer.android.com/reference/android/graphics/RenderEffect#createBlurEffect(float,%20float,%20android.graphics.Shader.TileMode)).
