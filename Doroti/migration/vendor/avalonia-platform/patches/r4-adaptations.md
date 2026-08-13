# R4 Avalonia platform slice adaptations

Source snapshot: `selected-content-sha256:75460dd7cb693dafaf5386a11fb878e222490e8a179e1a3f9d8b1283c10099cd`.

This patch record is intentionally symbol-scoped. The exact source and adapted file hashes are authoritative in `selection.json`.

| Upstream source | R4 decision | Dependency-closure change |
|---|---|---|
| `Interop/UnmanagedMethods.cs` | rewrite | Keep only HWND, message, DPI, cursor, DIB and IMM calls; remove CsWin32, MicroCom and unrelated Win32 APIs. |
| `SimpleWindow.cs` | adapt | Preserve class/GCHandle/HWND lifetime; replace Avalonia callback contract with internal Doroti vendor events and exception rethrow at the pump boundary. |
| `WindowImpl.AppWndProc.cs` | adapt | Preserve close, size, DPI, raw mouse/key and IME message semantics; exclude automation, visual, dispatcher and render-loop paths. |
| `FramebufferManager.cs` | adapt | Preserve validated BGRA DIB present and DC release; replace Avalonia framebuffer abstractions with a pixel-span presenter. |
| `Avalonia.Skia/FramebufferRenderTarget.cs` | adapt | Preserve Skia BGRA allocation/clear/present lifecycle; replace Avalonia drawing contracts with generation-aware internal frames. |
| `WindowImpl.cs` | exclude | Its closure crosses controls, property/window contracts, storage, composition and service-location boundaries. |
| `DirectX/DxgiRenderTarget.cs` | exclude | Its closure crosses EGL, ANGLE and MicroCom; the narrower audited WGL/OpenGL route supplies GPU rendering instead. |

No upstream update is applied automatically. `vendor-review` produces the full current source-to-adaptation review bundle before a future refresh.
