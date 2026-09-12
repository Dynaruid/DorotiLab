# UIKit Metal lifetime regression

On an Apple Silicon Mac with the .NET 10 Mac Catalyst workload:

```sh
python3 Doroti/validation/uikit-metal-lifecycle/run.py \
  --dotnet-sdk /usr/local/share/dotnet/sdk/10.0.400/dotnet.dll
```

This app compiles the production `DorotiUIKitGraphiteViewHandler.cs` and
`DorotiGraphiteView.cs` with the real shared Graphite session and official
Mac Catalyst NativeAssets. Only the framework paint-event payload is replaced
by a small fixture, so this does not qualify the entire MAUI application.

It exercises actual drawable completion, recording cancellation, a real Metal
one-second shared-event queue delay, three-frame backpressure, nonblocking disconnect,
view disposal before GPU completion, stale callbacks, return of resources on
the owner thread, and rejection/reopening of a new generation during/after
retirement. Build and run each have the external 1,200-second timeout.
Raw logs, commands and runtime JSON are retained in the output directory.

For a paired iPhone with the .NET 10 iOS workload, add `--device <devicectl-id>`.
This builds the `ios` project with the production iOS `DorotiSkiaView`, installs
and runs it, and copies the JSON report from its app container. Its development
signing ID is `dev.doroti.testbed`: it temporarily replaces the Testbed app.
Reinstall the final product bundle after the probe (application data is retained).

The shared probe also swaps drawable width/height in both directions and checks
that a paint happens before layout returns, using the new pixel dimensions.
On iOS these layout paints must use Core Animation transaction presentation;
ordinary paints must return to asynchronous presentation. This checks the
rendering contract, not the perceived smoothness of physical device rotation.
The iOS probe additionally composites center-marker bitmaps with the production
view's content gravity/scale at portrait, intermediate and landscape bounds.
It checks both the pixel centroid and the original 20×20 marker dimensions,
compares with a centered UIView, and reproduces top-left drift and scale-to-fill
stretching as negative controls. This uses Core Animation bitmap layers;
it does not capture the system rotation animation or actual Metal layer output.
See [the Flutter comparison and scope](rotation-center.md).
The iOS resize path follows Apple's
[transaction presentation contract](https://developer.apple.com/documentation/quartzcore/cametallayer/presentswithtransaction):
commit the terminal buffer, wait for scheduling (not GPU completion), and present
the drawable directly before committing the layout transaction. Resource retirement
continues through the existing asynchronous GPU completion callback.

The shared probe also directly fires the retirement
deadline callback while three real frames are pending: it must report a timeout,
retain all resources, leave device loss false, and recover after GPU completion.
This callback injection is **not** evidence of an actual five-second GPU stall.
On iOS, synthetic UIKit activation notifications verify that inactive frame
admission stops and activation reopens it; actual OS background/resume is checked
separately by `apple-official-assets/smoke-ios.py` on the full Release product.

This is a Debug Mono/interpreter diagnostic. Physical input, permanent GPU
stall, device loss, Release/AOT, and product performance are separate gates.

Set `DOROTI_METAL_TEST_DELAY_SECONDS=7` only for the separate extended-delay
diagnostic. On the tested M1, the injected waiting command hit a Metal GPU
timeout before seven seconds; that run is retained as FAIL, not lifetime PASS.
The probe reports the delay-buffer status/error and the event value before
cleanup so driver cancellation cannot masquerade as the requested delay.
