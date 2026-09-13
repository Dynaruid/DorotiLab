# Window appearance

`DorotiViewConfiguration.appearance` separates the window material from its titlebar.
The default titlebar style is `unified`. Select `solid` for a separate native caption.

```csharp
new DorotiViewConfiguration(
    title: "My app",
    logicalSize: new Size(1000, 720),
    backgroundColor: new Color(0x00000000L),
    darkBackgroundColor: new Color(0x00000000L),
    appearance: new WindowAppearanceOptions(
        backdrop: new WindowBackdropOptions(WindowBackdropMode.acrylic),
        titlebarStyle: WindowTitlebarStyle.unified,
        macOSBackdrop: new WindowBackdropOptions(WindowBackdropMode.liquidGlass)));
```

This requests Acrylic on Windows and Qt/Linux, and Liquid Glass on native macOS.
Omit `macOSBackdrop` to use the common material everywhere. Set it to
`new(WindowBackdropMode.acrylic)` to explicitly choose macOS blur. Liquid Glass falls
back to native blur before macOS 26. Material-specific tint, theme and Acrylic kind remain
in `WindowBackdropOptions` and can differ between the common material and macOS override.

| Host | `unified` with a material effect | `solid` |
| --- | --- | --- |
| Native AppKit macOS | One native blur/glass view behind both caption and body; no titlebar separator | Separate native titlebar; material behind the body |
| Windows App SDK | Native DWM Desktop Acrylic caption, preserving native caption buttons and client coordinates | Explicit opaque caption color following system light/dark appearance |
| Qt/Linux | Caption and controls drawn in the same GPU surface and compositor blur region as the body | Desktop-owned window decorations |

On Qt, caption drag/edge resize use `QWindow::startSystemMove` and `startSystemResize`.
Minimize, maximize/restore, close, double-click maximize and Alt+F4 are handled by the
native host. Caption buttons are exposed through Qt accessibility. The caption height is
included in `viewPadding.top` and removed in fullscreen; applications should respect safe areas.
Wayland blur still depends on `ext-background-effect-v1` or KDE blur support; configured
backdrop fallback applies when neither is available. A desktop theme may customize native
decorations in `solid` mode. OS material colors are not guaranteed to match across platforms.

Without a supported material effect, `unified` retains the normal native titlebar.
These desktop integrations are native AppKit, Windows App SDK and Qt. Mac Catalyst,
mobile, Web and the optional Windows MAUI renderer retain their existing behavior.

The old `DorotiViewConfiguration.backdrop` parameter remains supported. If `appearance`
is supplied, it takes precedence as a whole; unspecified fields do not inherit from the
legacy parameter. `new WindowAppearanceOptions()` therefore explicitly selects the system
background even when a legacy Acrylic option is also present.

AppKit's `DorotiMacOSMetalView.SetWindowAppearance(...)` can switch material and titlebar
style without moving the renderer. Existing `SetBackdrop(...)` calls retain the unified
default. Windows and Qt select the titlebar at window creation.

In the testbed, `DOROTI_TITLEBAR=unified|solid` applies across desktop runners and
`DOROTI_MACOS_BACKDROP=acrylic|liquidGlass` selects the macOS material. The previous
`DOROTI_MACOS_TITLEBAR` alias remains accepted if `DOROTI_TITLEBAR` is absent. The sample's
effect toggle changes widget opacity; these launch options configure the native window.

Qt native ABI version 3 now requires feature bit 14, a 56-byte configuration and a
144-byte surface descriptor. Rebuild the native shim together with the managed host;
the app template contains the same native implementation. Windows uses negotiated
titlebar feature bits 6/7 without changing configuration layout; older shims reject these
features instead of silently ignoring the selection.

Validation: the AppKit fixture exercises both materials/styles, repeated configuration,
resizing, input pass-through and restoration. The Qt managed contract checks API routing,
ABI layout and caption compositing; the native probes check input, accessibility and
fullscreen metrics under Xvfb. Windows managed compilation can run on macOS with PRI
generation disabled; native Windows execution and compositor visual acceptance require
their respective desktop environments.

Native references: [DWM backdrop types](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwm_systembackdrop_type),
[Qt system move and resize](https://doc.qt.io/qt-6/qwindow.html#startSystemMove).
