# Android virtual accessibility contracts

This fixture source-links `MauiAndroidSemanticsBridge` onto a real Android View;
it does not need Vulkan or the full gallery. The trimmed Release run checks native
node metadata, coordinate conversion, action forwarding, projection feedback,
hidden/stale nodes, focus removal and teardown. It creates no native control per
semantic node. The small partial test class is included only in this fixture.

```powershell
python Doroti/validation/run-with-timeout.py python Doroti/validation/android-semantics-provider/run.py `
  --serial device-serial --output Doroti/artifacts/android-semantics-provider
```

The runner installs its own `dev.doroti.validation.semanticsprovider` package and
retains the build/result/log evidence. Remove that test package after collecting
the result if it is no longer needed. It does not change accessibility services.
The test uses an inline UI dispatcher; actual background/foreground integration
and scroll performance are checked on DorotiTestbedApp separately. See the
[Galaxy timing report](../app-runner/android-touch-timing.md).
