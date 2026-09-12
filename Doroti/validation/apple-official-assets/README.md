# Apple official bundle assets

```sh
python3 Doroti/validation/run-with-timeout.py python3 \
  Doroti/validation/apple-official-assets/verify-bundle.py path/to/App.app \
  --platform macos --output assets.json
```

Use `--platform maccatalyst` for Catalyst. The verifier requires one deployed
Skia image (framework symlinks resolve to the same file), checks the cached
asset against the pinned NuGet archive, and compares the arm64 UUID and every
file-backed Mach-O section. Apple thinning, stripping and codesigning can
change whole-file hashes, which are recorded separately. This is a bundle
check, not runtime-load or rendering evidence.

Verify both NuGet archives with `dotnet nuget verify --all`, and collect the
running process's actual Skia image path separately. App ID and both version
values from the final Info.plist are included for comparison with runner inputs.

For iPhone device bundles use `--platform ios`; `--platform iossimulator`
selects the official simulator asset's arm64 slice. Both use the bundle-root
Info.plist. The device and simulator assets are never substituted for each other.

The iOS product smoke installs a signed bundle, clears only the old diagnostic
snapshot, verifies default Graphite/Metal frames, backgrounds the app by opening
Settings, and resumes the same process:

```sh
python3 Doroti/validation/native-aot/run.py --log /absolute/evidence/smoke.log -- \
  python3 Doroti/validation/apple-official-assets/smoke-ios.py path/to/App.app \
  --device <devicectl-id> --output /absolute/evidence/product
```

It leaves the product installed and running. The external timeout is 1,200
seconds. These are frame/load/resume checks; physical input, exhaustive feature
coverage, permanent stalls, device loss and performance require separate evidence.

For load evidence, capture a separate `devicectl device process launch --console`
run with `--environment-variables '{"DYLD_PRINT_LIBRARIES":"1"}'`. Pass that log
with `--loader-log path/to/console.log`: the verifier requires its dyld UUID and
bundle-relative Skia path to match the bundle's arm64 image. Without this option,
runtime load stays `notVerified`. The separate console run may be terminated for
cleanup; that termination is not normal GPU shutdown evidence. LLDB attachment
was unavailable during the recorded iPhone run, so dyld diagnostics were used.
