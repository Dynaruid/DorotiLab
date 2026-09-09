# AppKit Metal / Graphite qualification

The existing `net10.0-macos` / `osx-arm64` validation app compares Ganesh with the shared native Graphite session. It is separate from product promotion.

```sh
python3 Doroti/validation/appkit-metal-spike/run-graphite.py
```

Run from the repository root on an Apple Silicon Mac with Xcode and the .NET 10 `macos` workload. The runner builds once and executes three independently reported checks:

- `contract`: 36 external-texture frames over three sizes/context generations, raster upload, text, gradient, runtime shader, blur, compatible GPU offscreen capture and asynchronous pixel assertions. Also tests cancellation, duplicate submit/completion rejection, thread ownership, independent sessions, bounded frames and rejection of disposal with pending work.
- `ganesh`: retained scene with text and a Doroti runtime shader, 20 window size changes, minimize/restore, hide/unhide and shutdown with an outstanding frame.
- `graphite`: the same window scene and lifecycle through `SkiaGraphiteSession`, with no presentation readback/copy and no per-frame synchronous Graphite submission.

Use `--mode contract|ganesh|graphite` to select a check and `--no-build` only when intentionally reusing the current validation binary. Every child has an external 1,200-second timeout that kills its process group. Timestamped reports, raw stdout/stderr and command/log hashes are retained under `Doroti/artifacts/native-graphite/apple-runs/`. The actual app executable is launched directly so `Metal API Validation Enabled` and native errors reach the logs. PASS is based on the report and process result, including completed resource release for the window checks.

Window completion callbacks return to the recorder's owner thread. A deliberately stale completion during shutdown is counted but never acknowledged as a new visible frame. Output resources and renderer GPU caches are returned before the session, queue and device are released. The v2 report records the actual loaded Skia managed/native identities instead of historical hardcoded package versions.

The separately selected **product** candidate uses `DOROTI_MACOS_GRAPHITE=1` with the regular `DorotiTestbedApp` AppKit runner. See [session ownership](../../docs/architecture/native-graphite-session.md) and [Apple execution evidence](../../docs/validation/native-graphite-apple-2026-09-09.md). Device loss, physical screen/input/accessibility approval, iOS/Catalyst and performance qualification remain separate gates. The product default remains Ganesh/Metal.
