# Runtime .NET migration checks

This entry point checks managed dispatcher/time contracts, the retained Flutter
`Duration` to `TimeSpan` boundary, map/byte ownership baselines, Matrix4 storage,
and numeric typed-list codec wire contracts.
It also prints the current Runtime type/member surface for migration review.
See [the current API changes](api-changes.md) and [inventory](inventory.md).

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/runtime-dotnet/RuntimeDotNet.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/runtime-dotnet/RuntimeDotNet.csproj -c Release -- --api-snapshot
```

The test uses a deterministic fake frame host. It does not establish Web,
mobile, physical-device, display, or performance behavior.

The optional compiler fixture is in
`tools/Doroti.DartToCSharp/validation/runtime-dotnet/`. Its `validate.ps1`
generates a small Dart `Math`/`Random`/`Duration`/`File.path` consumer and builds the emitted
C# against this checkout. Generated files stay under `Doroti/artifacts`.
