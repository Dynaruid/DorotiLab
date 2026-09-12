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

This is a Debug Mono/interpreter diagnostic. Physical input, permanent GPU
stall, device loss, Release/AOT, and product performance are separate gates.

Set `DOROTI_METAL_TEST_DELAY_SECONDS=7` only for the separate extended-delay
diagnostic. On the tested M1, the injected waiting command hit a Metal GPU
timeout before seven seconds; that run is retained as FAIL, not lifetime PASS.
The probe reports the delay-buffer status/error and the event value before
cleanup so driver cancellation cannot masquerade as the requested delay.
