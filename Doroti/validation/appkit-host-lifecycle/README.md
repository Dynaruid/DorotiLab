# Production AppKit Metal lifetime probe

```sh
python3 Doroti/validation/run-with-timeout.py dotnet \
  /usr/local/share/dotnet/sdk/10.0.400/dotnet.dll build \
  Doroti/validation/appkit-host-lifecycle/Doroti.Validation.AppKitHostLifecycle.csproj \
  -r osx-arm64 -m:1 -p:UseSharedCompilation=false

MTL_DEBUG_LAYER=1 python3 Doroti/validation/run-with-timeout.py \
  'Doroti/artifacts/validation/build/appkit-host-lifecycle/bin/Debug/net10.0-macos/osx-arm64/Doroti AppKit Host Lifecycle.app/Contents/MacOS/Doroti.Validation.AppKitHostLifecycle'
```

The fixture references the real Host.Maui AppKit view/session, using reflection
only to attach a paint callback and inspect lifecycle counters. It verifies
three-frame backpressure with a real Metal shared-event delay, immediate
disconnect, native view disposal while GPU work is pending, stale completion,
and actual owner-thread release. No MAUI page/widget tree is involved.
Set `DOROTI_APPKIT_HOST_LIFECYCLE_EVIDENCE` to retain the JSON report.

The default GPU delay is one second. `DOROTI_METAL_TEST_DELAY_SECONDS=7` selects
an extended diagnostic that can hit the driver's GPU timeout; it must not be
reported as a seven-second retirement PASS if the delay command fails.
Physical input, performance, device loss and permanent stalls remain separate.
