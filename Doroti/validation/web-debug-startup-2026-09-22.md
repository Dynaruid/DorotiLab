# Debug threaded WASM startup — 2026-09-22

**PASS:** the default Debug native build now starts Mono and renders through both
WebGPU and WebGL. Windows Chrome 153.0.8010.53; .NET SDK 10.0.400, runtime
10.0.11, Emscripten 3.1.56. All build/browser commands used the repository's
20-minute timeout wrapper.

## Cause and correction

The baseline failed before managed application startup with Invalid UTF-8 and
`mono-threads-wasm.c:201`. Disassembly of the actual native WASM identified the
failing check as Mono's non-null pthread stack lower bound assertion.

At `-O0`, [Emscripten 3.1.56's linker](https://github.com/emscripten-core/emscripten/blob/3.1.56/tools/link.py)
implicitly enables `STACK_FIRST` unless `GLOBAL_BASE` is explicitly supplied.
The baseline `emscripten_stack_init` used bounds **0–5,242,880**, so the stack was
valid to Emscripten but its zero lower address failed Mono's check. See also the
[pinned Mono stack bounds implementation](https://github.com/dotnet/dotnet/blob/e2f47b0110ed922f21a1522da67279133ce28f32/src/runtime/src/mono/mono/utils/mono-threads-wasm.c).

`Doroti.Runner.Sdk/Sdk/Doroti.Web.targets` now prepends `-sGLOBAL_BASE=1024` to
the native linker flags for threaded Web Debug builds, before the SDK constructs
its native response files. This makes Emscripten retain its normal data-first
layout. Existing project flags follow it, allowing an explicit project
`GLOBAL_BASE` to take precedence. The resulting stack bounds are
**1,712,000–6,954,880**: the same 5 MiB stack at a nonzero address.

Native `-O0`, `-g`, pthreads, and assertions remain enabled. The generated runtime
configuration still has `debugLevel: -1` and the output contains 21 managed PDBs.
No package upgrade, optimization workaround, or runtime assertion removal was
needed. An IDE breakpoint/debugger attachment was not tested.

## Evidence

Paths below are relative to `Doroti/artifacts/`.

| Gate | Result / evidence |
| --- | --- |
| Baseline Debug build | PASS, zero warnings/errors; `debug-wasm-baseline-build.log` |
| Baseline Debug startup | FAIL reproduced; `debug-wasm/baseline/events.json` |
| Baseline/fixed native stack | `debug-wasm/baseline-stack.txt`, `debug-wasm/fixed-stack.txt` |
| Baseline/fixed linker arguments | `debug-wasm/baseline-link.rsp`, `debug-wasm/fixed-link.rsp` |
| Fixed Debug build | PASS, zero warnings/errors; `debug-wasm-fixed-build.log` |
| Debug WebGPU runtime | PASS, 15 execution + 46 pixel checks; `debug-wasm/fixed-gpu-fresh/` |
| Debug WebGL runtime | PASS, 15 execution + 46 pixel checks; `debug-wasm/fixed-gl/` |
| SDK package | PASS, packed `Sdk/Doroti.Web.targets` byte-equals source; `debug-wasm/packages/` |
| Release/single-thread scope | No added `GLOBAL_BASE`; `debug-wasm/release-flags.txt`, `debug-wasm/single-thread-flags.txt` |

Each successful browser directory includes `result.json`, `pixels.json`, console
events, screenshots, and `assets.json` with browser version and fetched asset
SHA-256 values. The checks cover actual managed Skia rendering, media sources,
resize, native WebView overlap, balanced resource retirement, and clean shutdown.
There were no unhandled exceptions in either successful execution.
Both browsers fetched the exact rebuilt native WASM (SHA-256
`540de45304e1d4ca392d6c26d7daf243b4e677f00471ec69a1160613a48171df`),
verified against the linker output in `debug-wasm/verified-build.json`.

The first post-build request to the already-running dev server failed to fetch
the new fingerprinted runtime module (`debug-wasm/fixed-gpu/events.json`). That
server had retained its old static asset manifest. The authoritative successful
runs use a freshly started dev server and fresh browser profiles. Restart the
dev server after rebuilding native WASM.

Browser evidence uses automated headless Chrome, an isolation-header proxy, and
a synthetic camera. It does not establish physical camera, input/display latency,
other browser engines, or IDE debugger acceptance. Historical Release evidence
and its independent limitations remain in [the texture report](textures/web-results-2026-09-22.md).

## Reproduce

From the repository root:

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Debug
# Start a fresh server after the build. Use its printed URL below.
dotnet run --project DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Debug --no-build --no-launch-profile
```

In another shell, with a new output directory for each run:

```powershell
python Doroti/validation/run-with-timeout.py node Doroti/validation/textures/verify-web.mjs <gpu-output> http://127.0.0.1:5000 worker-direct-webgpu
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/analyze-web.py <gpu-output>
python Doroti/validation/run-with-timeout.py node Doroti/validation/textures/verify-web.mjs <gl-output> http://127.0.0.1:5000 worker-direct-webgl
python Doroti/validation/run-with-timeout.py python Doroti/validation/textures/analyze-web.py <gl-output>
python Doroti/validation/run-with-timeout.py dotnet pack Doroti/src/Doroti.Runner.Sdk/Doroti.Runner.Sdk.csproj -c Release -o Doroti/artifacts/debug-wasm/packages
```
