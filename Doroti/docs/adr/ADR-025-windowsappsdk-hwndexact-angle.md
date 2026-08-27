# ADR-025: Windows App SDK HwndExactCpp and managed ANGLE presentation

- Status: Accepted
- Date: 2026-08-26

## Decision

The canonical `windows` runner is the self-contained unpackaged Windows App SDK 2.4 `win-x64` target. It uses the `HwndExactCpp` adapter and a managed ANGLE/EGL-D3D11 Skia presenter. Windows MAUI remains an explicit independent backend; it is not the default and is never selected as a runtime fallback.

The visible window topology is one standard top-level HWND plus one app-owned `WS_CHILD | WS_VISIBLE` render/input HWND. A message-only task HWND carries bounded completion work without recursively pumping the top-level queue.

Ownership is split as follows:

- native C++20 owns the top-level, child, and task HWNDs, AppWindow integration, WndProc, physical client geometry, resize generations, lifecycle, and input ingress;
- managed .NET owns the Doroti framework, immutable scenes, input dispatch, ANGLE display/context, Skia context and surfaces, raster, and presentation;
- the versioned C ABI carries metrics, input, lifecycle, and terminal packets but no GPU object or COM pointer.

## Rendering and presentation

The default presenter requires a hardware Direct3D 11 ANGLE renderer and fails closed for WARP, SwiftShader, reference, or software renderers. It creates an `EGL_FIXED_SIZE_ANGLE` child-HWND surface matching the current physical client extent.

Doroti paints into an exact-size GPU backing surface. Presentation snapshots that backing, performs one full-frame `Src` GPU blit into the Skia-wrapped EGL default framebuffer, resets inherited GL state, flushes/submits, and calls `eglSwapBuffers`. There is no CPU full-frame readback and no provisional full-frame stretch.

The first successful swap on every newly created EGL surface is followed by `DwmFlush` before the resize terminal or initial show proceeds. This ordering fixed the blank initial-window case where the framework had already submitted a scene but DWM had not exposed the first EGL front.

## Resize and mixed-DPI protocol

The render child physical client size is the metrics authority. Each request has an immutable generation and reaches exactly one `Presented`, `Superseded`, or `Failed` terminal. Native scheduling retains at most one pending latest request while one render is in flight, and a resize transaction waits no longer than 100 ms.

During an interactive move, cross-DPI monitor straddling is handled with a boundary-only refresh:

- an 8 ms timer runs only while the window is in interactive move/size state;
- a same-size generation is queued only while the window intersects at least two monitors and no render is pending or in flight;
- the refresh stops outside that straddling state;
- `WM_EXITSIZEMOVE` publishes one final stable refresh.

Managed presentation rebuilds the EGL window surface for those stable same-size generations. This avoids leaving damaged black tiles during a mixed-DPI boundary drag without creating a permanent redraw loop.

## Selection and packaging

- `doroti.ps1 ... -Platform windows` selects Windows App SDK and exports `DOROTI_WINDOWS_ADAPTER=HwndExactCpp` for the child process.
- `-WindowsBackend Maui` selects the independent MAUI runner when the workspace provides it.
- an unset `DOROTI_WINDOWS_PRESENTER` or the explicit value `AngleD3D11` selects ANGLE/EGL-D3D11;
- `DOROTI_WINDOWS_PRESENTER=D3D12` is an explicit legacy diagnostic path only;
- there is no automatic presenter fallback;
- the removed Vulkan presenter and Silk.NET Vulkan dependencies are not product options.

The target package carries the native host, Windows App Runtime bootstrap, and x64 ANGLE runtime in the app directory. Missing files, wrong architecture, and ABI/version mismatches fail before the window is created.

## Evidence boundary

The Windows target/package/default-CLI cutover and the current ANGLE automated runtime, initial visibility, lifecycle, and provenance scopes passed. The user accepted the tested physical resize and mixed-DPI boundary behavior. Strict synthetic resize qualification and pixel/cadence failures remain recorded as failures; user acceptance does not reclassify them.

Automated pointer/key/focus/clipboard coverage passed. IMM32/UIA and lifecycle/device coverage is partial automation only. Physical Korean IME candidate/caret behavior, Narrator and Accessibility Insights, the full DPI/monitor/window-management/device-removal/shutdown matrix, installer/MSIX, and untested edge/speed combinations remain `notVerified`.

The Windows default cutover passed independently. The last full `Doroti.Product.slnx` Release run on Windows still failed when a macOS project invoked the unavailable `sips` tool; that global regression result is not hidden by the Windows result.

The archived implementation and acceptance ledger is in [the 2026-08-26 Windows summary](../../../history/26-08-26/windows-appsdk-hwnd-exact-summary.md).

## Consequences

Current documentation and examples must describe Windows App SDK/HwndExactCpp/ANGLE as the default Windows product path. Older MAUI, WGL, ContentIsland, D3D12, and Vulkan records remain useful only in their stated historical, explicit-alternative, or diagnostic scopes.

Build, automated contract, capture, physical acceptance, accessibility, and cross-platform regression results remain separate evidence classes.
