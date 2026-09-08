# Dependency freshness — 2026-09-08

The fixes cover Qt native-library copying, Windows C++/WinRT generation, and
dependency-bound build/reuse through `Doroti/eng/doroti.ps1`. They build on the
WebCIL package-cache fix from the same session.

## Behavior

- Qt copies the selected `.so` to build/publish output even when size and mtime
  match the previous file. Dependency rebuilds pass `--clean-first` to CMake.
- Windows binds generator bytes, pinned SDK/package metadata, and every generated
  header to a content identity. Missing or modified headers regenerate the set.
  Native build and dependency discovery share pinned version properties.
- CLI state v3 includes restored transitive compile/runtime/native assets,
  runtime packs, native file references, and explicitly declared
  `DorotiLaunchDependency` inputs. Framework selection follows MSBuild project
  reference negotiation, including the Android MAUI multi-targeted host.
- Restore precedes identity collection. Changed/untracked dependencies or tools
  force a rebuild. CLI compilation uses fresh compiler processes to avoid warmed
  metadata caches; unchanged MSBuild compilations remain incremental.
- Reuse rejects changed dependencies and old v1/v2 records. Changes during a build
  prevent success recording/launch. Failed builds preserve the previous record.

## Verified

| Check | Result |
| --- | --- |
| Actual Qt copy target, same-size/mtime replacement | New bytes in build and publish output |
| Windows generator fixture | 8 scenarios passed, including no-op, tool/SDK/input changes, missing/tampered secondary headers, missing tool |
| Actual Windows native build and unchanged repeat | Passed; identical native DLL SHA-256 on repeat |
| CLI with isolated transitive NuGet package | 8 scenarios passed; changed bytes rejected for reuse, rebuilt value `TWO`, subsequent source edit still `TWO`, failed build did not replace record |
| Existing launch identity contract | Passed, including 5 artifact rejection cases |
| Actual dependency discovery | Windows, Web, Android x64 succeeded |
| Actual Web Release CLI build | Passed, 0 warnings / 0 errors |
| Subsequent normal Web run | Incremental build succeeded without dependency rebuild |
| Actual Web `-LastSuccessful` | `build=reused restore=reused`, HTTP server started |
| Chromium default renderer and unmodified diagnostic/sample URLs | 3 tests passed with painted content and no runtime errors |
| TypeScript check / PowerShell parse / diff whitespace | Passed |

Evidence: `../web-playwright/artifacts/dependency-audit-20260908/` and
`../web-playwright/artifacts/dependency-freshness-ready/` (local generated files).
Reproducible entry points are `native-dependency-contract.py`,
`dependency-reuse-contract.py`, and the existing `launch-identity-contract.ps1`.
Each test subprocess uses a 20-minute timeout.

## Preserved failures and limits

- The original Qt fixture retained `OLD!` after its source changed to `NEW!`.
- An initial explicit Windows SDK reference-directory attempt lacked required
  metadata; the final generator references the pinned union `Windows.winmd`.
- The initial CLI fix rebuilt through a warmed compiler and still printed `ONE`;
  the final fixture verifies isolated compilation and subsequent source edits.
- The first final browser run started before the server was ready and failed
  with connection refused. Those artifacts remain in `dependency-freshness-final`;
  the successful run after observed server readiness is `dependency-freshness-ready`.
- Qt copying was exercised on Windows with isolated files, not a native Linux
  application. Android discovery is not device deployment/runtime acceptance.
  Apple builds/devices, physical input, and cross-platform performance remain
  notVerified. The common rebuild/reuse guarantee applies to the Doroti CLI;
  direct external build commands do not create or enforce its v3 state.
