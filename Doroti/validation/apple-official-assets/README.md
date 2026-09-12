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
