# Desktop window API — implementation status, 2026-09-26

The 2026-10-02 [native frame pipeline](native-frame-pipeline.md) adds common C
admission and separately tracks Vulkan producer and D3D12 consumer completion in
both Windows hosts. Embedded MAUI composition and Ganesh retain serial fallback.
This does not add native MAUI Windows Editor/WebView adapters; the new input
probe's unsupported result is retained in the candidate report.

The first implementation provides `Doroti.Desktop` and optional
`Doroti.Desktop.Widgets`. **The complete W0–W5 plan is PARTIAL.** Windows MAUI
and AppKit macOS have native main-window adapters. Mac Catalyst has a restricted UIKit scene adapter, and Linux Qt Quick has main/additional-window adapters. Windows App SDK now has a main-window adapter (2026-09-28); custom
title-bar widgets remain unimplemented. The 2026-09-29 Windows App SDK increment
adds native multi-window execution and Explicit lifetime; see the
[window context ADR](desktop-window-context.md) and [M6 evidence](../../works/common/09-multiwindow.md).

## Architecture and inventory

| Layer | Owner / source | Status |
| --- | --- | --- |
| Application startup | `Hosting/DorotiApplicationBootstrap.cs`, SDK generated descriptor | Common app stays target-neutral |
| Desktop declaration | `DesktopApplication.Configure<TStartup>`, separate companion project | Implemented; descriptor-scoped registration |
| Window identity/lifetime | `DorotiWindowManager`, `DorotiWindowController` | Implemented; fake two-window contracts |
| Content | `WindowContent.FromEntrypoint`, `WidgetWindowContent.Create`, `DesktopWindowScope.Of` | Fresh factory and explicit window context; no current-window singleton |
| Windows MAUI creation | `DorotiMauiApplication.CreateWindow`, `WindowsDesktopWindowHost` | Main window uses manager; native additional creation rejected before allocation |
| First display | `DorotiMauiPlatformApplications` before native activation; `DorotiMauiSurface.CompleteNativePaint` | DWM cloak until first non-stale present; no readiness timer |
| Close | AppWindow Closing → controller decision → render drain → detach/dispose → native close → registry removal | Native/API close share one cancellable decision |
| Material/caption | `WindowsWindowBackdrop`, `WindowsNativeCaption` | Same appearance snapshot, native System/Solid/Backdrop caption |
| Existing raw Acrylic channel | `WindowsAcrylicOptionsState` | Existing implementation retained; controller adapter still pending |
| AppKit | `AppKitDesktopWindowHost`, `AppKitDesktopWindowPolicy`, macOS platform application | Opt-in desktop launch, ordered-out Metal preparation, native controls/materials and cancellable window/app exit |
| Qt | `QtDesktopWindowHost`, `QtDesktopWindowPolicy`, optional Desktop ABI 1 | Quick main/additional windows, PlatformDefault startup, native chrome; legacy paths retained |
| Legacy facade | `SingletonDorotiWindow`, Ui metrics, `WindowTitlebar` | Unchanged; not promoted to process-wide window manager |

SDK properties `DorotiDesktopProject` and `DorotiDesktopStartupType` compile the
companion separately and register it only in a native desktop runner. This
release additionally rejects unsupported desktop host adapters at build time
(`DOROTIDESKTOP005`). Manual bootstrap bypassing that check fails when the host
does not explicitly consume the registered desktop definition. Common Ui,
Hosting and Framework packages have no Desktop reference.

`DesktopApplicationBuilder.LegacyMainWindow` is an explicit migration input. A
companion may reuse its factory once. The common declaration remains the
Web/mobile configuration; it does not attach another desktop root. Repeating
`UseMainWindow` is an error. Apps without a companion retain their existing path.

## Public contract

`WindowOptions` is an immutable request, not live state. Size/min/max are **client
DIP**, and Position/outer Bounds are **global physical desktop pixels**. This
explicit distinction avoids multiplying mixed-monitor coordinates by one DPI.
`State` reports observed bounds, scale, visibility, focus and presentation.
OS focus policy still decides whether a focus request succeeds.

The manager has no Show/Focus methods without a target. `MainWindowId` is never
promoted or reused. `GetWindows()` returns a read-only snapshot. An independent
window closing does not close another controller. `OnLastWindowClosed` and
`Explicit` select app exit policy; hidden windows count as alive.

Creation cancellation before return destroys its resources; cancellation of the
same token after return does not own the window. Readiness waiter cancellation
affects only that waiter. The creation hook is observed through
`InitializationWork` / `InitializationError` and manager `InitializationFailed`.
A hook may await ReadyToShow without blocking framework attachment. A pre-ready
failure faults readiness and cleans up; a post-ready failure preserves readiness.

Appearance commands serialize per window. Pending full replacements can return
Superseded; functional updates read the latest requested snapshot inside the
queue. Results distinguish Applied/Superseded/Rejected/Failed/Canceled. Failed
native application attempts rollback; rollback failure is retained in an
AggregateException with the host's observed state. Appearance command revisions
are independent of renderer generation. Accepted close cancels deferred work
before draining the command queue.

OwnerWindowId is reserved and explicitly rejected. Modal/owner lifecycle, actual
multi-native-window support, cross-window widget transfer and global opacity
are not provided in this release. Shared singleton Flutter bindings are not
claimed to support W6 because a fake host can create two controllers.

## Examples

Desktop companion only (use `Doroti.Ui.Size`; appearance type names also exist
in legacy Ui, so use an alias or fully qualify the new types):

```csharp
public sealed class DesktopStartup : IDorotiDesktopApplicationStartup
{
    public void Configure(DesktopApplicationBuilder desktop)
    {
        desktop.UseMainWindow(desktop.LegacyMainWindow with
        {
            Options = new WindowOptions
            {
                Title = "Sudoku", Size = new Size(450, 800),
                MinimumSize = new Size(350, 500),
                StartupVisibility = WindowStartupVisibility.Manual,
            },
            OnCreated = async (context, ct) =>
            {
                await context.Window.EnsureInitializedAsync(ct);
                await context.Window.WaitUntilReadyToShowAsync(ct);
                await context.Window.ShowAsync(ct);
                await context.Window.FocusAsync(ct);
            },
        });
    }
}
```

Omit OnCreated and keep the default WhenReady for automatic display. For a new
widget root use `WidgetWindowContent.Create(() => new MyApp())` from the optional
widget package; the factory injects DesktopWindowScope. Embedded MAUI surfaces
never receive implicit window control.

Native Acrylic appearance (transparent framework content is also necessary):

```csharp
new Doroti.Desktop.WindowAppearanceOptions
{
    BackgroundColor = new Doroti.Ui.Color(0x00000000),
    Backdrop = new Doroti.Desktop.WindowBackdropOptions
    {
        Mode = Doroti.Desktop.WindowBackdropMode.Acrylic,
        AcrylicKind = Doroti.Desktop.WindowAcrylicKind.Thin,
    },
    TitleBar = new WindowTitleBarOptions
    {
        Background = WindowTitleBarBackground.Backdrop,
    },
};
```

For a separate solid native caption, set TitleBar.Background=Solid and optionally
TitleBar.BackgroundColor. For OS default caption use System. The DWM caption
material uses the OS variant and does not promise exact client tint matching.
An opaque Scaffold covers the client material; the existing Testbed Acrylic
toggle changes Scaffold opacity, not window material mode.

`Style=Hidden, Buttons=Native` is the reserved search/title-bar example. It
currently returns Unsupported before native allocation. Custom buttons and
frameless mode are also rejected. No helper with unverified hit testing or
accessibility is advertised as native-equivalent.

Future additional-window call (the signature compiles, Windows MAUI rejects it
before allocation until W6):

```csharp
var child = await context.Windows.CreateWindowAsync(new WindowCreateOptions
{
    Options = new WindowOptions { Title = "Settings" },
    Content = WindowContent.FromEntrypoint(() => new SettingsEntrypoint()),
}, cancellationToken);
await child.CloseAsync(cancellationToken);
```

## Current support

Current host/renderer/build-mode evidence is centralized in [support status](support-status.md).

| Host | Main-window adapter | Current limitation |
| --- | --- | --- |
| Windows App SDK | Main and additional HWNDs; Debug and NuGet-only Release native lifetime checks | OnLastWindowClosed/Explicit; backdrop/background changes require recreation; physical resize/IME/mixed-monitor and full native-content coverage notVerified |
| Windows MAUI | Implemented; historical results below | Separate native PlatformView wiring and current revalidation pending |
| AppKit | Main/additional windows; Graphite and Ganesh native Desktop/lifetime qualification (2026-09-29) | Physical input/VoiceOver, mixed DPI and clean signed deployment remain separate |
| Mac Catalyst | UIKit scene adapter with common additional-window factory | PlatformDefault startup; two scenes/independent size/survivor/Explicit drain verified separately; native close cannot be cancelled; [Apple results](../../works/results/2026-09-29-ios-catalyst.md) |
| Qt Quick | Main/additional QQuickWindow, independent owner resources | Wayland/XWayland VM execution; physical input/mixed DPI and bounds/appearance restrictions remain |
| Qt Widgets | No Desktop companion adapter | SDK rejects startup |

Custom/hidden chrome remains unsupported. Additional native windows are implemented for Windows App SDK only. Each App SDK adapter owns one HWND and content root; native/API close share controller cancellation and native render retirement. Readiness is completed by a successful native present, not a timer.


The legacy records remain available; no blanket Obsolete attributes or Flutter
API casing/Duration changes were introduced. WebGL2/WebGPU and renderer selection
policy remain unchanged. Linux in-app blur is not advertised as desktop Acrylic.

## Platform references

DWM cloak keeps composition running without exposing the window:
[DWMWINDOWATTRIBUTE](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute).
Client sizing uses
[AppWindow.ResizeClient](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.appwindow.resizeclient).
MAUI 10.0.90's native-caption conflict was traced through
[WindowRootView](https://github.com/dotnet/maui/blob/10.0.90/src/Core/src/Platform/Windows/WindowRootView.cs)
and [NavigationRootManager](https://github.com/dotnet/maui/blob/10.0.90/src/Core/src/Platform/Windows/NavigationRootManager.cs).
The adapter tracks the public XAML caption template part while native chrome owns
the title bar and unregisters the property callback at close.

## AppKit macOS

Set `DorotiDesktopProject` and `DorotiDesktopStartupType` in the AppKit runner,
using the same separate companion as Windows. The Testbed now enables this
path; the template exposes it through the optional desktop companion. Apps
without desktop startup retain the upstream MAUI AppKit launch. Mac Catalyst uses a separate UIKit startup with the limitations below.

The pinned MAUI AppKit preview constructs its WindowHandler directly and calls
MakeKeyAndOrderFront during allocation. The desktop launch therefore owns its
native window and handler, reuses MAUI's application/content handlers and menu
bar, and connects a controller before ordering the window. A hidden Metal
frame completes GPU work without waiting for presentation. Its drawable is
retained for first show. Close drains Metal work before surface disposal and
native close. Cmd+Q uses AppKit's deferred termination reply and the same
controller close decision. `Explicit` leaves the app running after the last window
closes. The manager can create further independent top-level windows. Each window
owns its dispatcher/session, native views and Metal surface; application resources
and plugin handlers share an application lease. Widgets unmount before capabilities
and GPU resources are retired. See the [2026-09-29 AppKit results](../../works/results/2026-09-29-macos-appkit.md).

| Feature | AppKit behavior |
| --- | --- |
| Size/min/max | Unobscured client area in points; native caption is excluded. Explicit sizes may exceed the screen work area. |
| Position/outer Bounds | Position and SetBounds are rejected; State.Bounds is null until a global physical-pixel mapping is implemented. Center uses AppKit. |
| Show/hide/focus | Manual/WhenReady preparation, native ordering and key-window request; OS focus policy still applies. |
| Minimize/zoom/full screen | Minimize and native full-screen transitions await delegate completion. Maximize uses AppKit zoom. |
| Topmost/Dock | Native floating window level; no claim to cover other full-screen Spaces. SkipTaskbar=true is rejected because Dock visibility is app-wide. |
| Native title bar | Normal with System/Solid/Backdrop; Solid has a separate opaque caption fill, native traffic lights remain. Explicit caption color requires Solid. |
| Materials | System/Solid/Transparent/Acrylic/LiquidGlass, runtime replacement and reset. MacOSBackdrop takes precedence. |
| Liquid Glass fallback | OS <26 uses the explicitly selected Solid/Transparent fallback. The legacy path retains its old Acrylic fallback. |
| Tint | Acrylic tint/luminosity requests rejected. Glass accepts TintColor and optional TintOpacity; luminosity rejected. |
| Accessibility policy | Reduce Transparency selects solid and reports SystemPolicyFallback; system/explicit appearance and effective theme are tracked. |
| Unsupported | Hidden/custom/frameless chrome, app theme bridge, programmatic resize initiation, owned/modal/Satellite windows. |
| Renderer base color | Changing BackgroundColor/DarkBackgroundColor at runtime returns RequiresRecreation. |

Native operation tests and screenshots are listed in the
[desktop validation record](../../history/26-09-26/desktop-window-api-summary.md). They do
not qualify physical mixed-monitor input, VoiceOver/IME, a complete first-frame
capture sequence, all OS accessibility settings, or notarized distribution.


## Mac Catalyst

Mac Catalyst is now an allowed desktop target, with a separate UIKit scene
adapter. It requires Mac idiom (`UIDeviceFamily=6`), the scene manifest and
registered `DorotiMacCatalystSceneDelegate`, `UIApplicationSupportsMultipleScenes=true`,
Graphite, and Mac Catalyst 16+. UIKit requires multi-scene adoption to destroy
even the only scene. The adapter still rejects additional native windows.
The Xcode 27 build profile uses `DorotiMacCatalystTargetFramework=net10.0-maccatalyst27.0`
and the SDK-required minimum Catalyst 17. The default profile and existing
non-desktop mobile boundaries remain intact.

```csharp
public void Configure(DesktopApplicationBuilder desktop)
{
    desktop.LifetimePolicy = WindowLifetimePolicy.Explicit;
    desktop.UseMainWindow(desktop.LegacyMainWindow with
    {
        Options = new WindowOptions
        {
            Title = "My Catalyst app",
            Size = new Size(600, 500),
            MinimumSize = new Size(350, 300),
            StartupVisibility = WindowStartupVisibility.PlatformDefault,
        },
    });
}
```

`PlatformDefault` deliberately lets UIKit own first visibility. ReadyToShow
still waits for initialized geometry and a completed Graphite frame; it does
not promise that the native window was hidden before readiness. AppKit and
Windows reject this startup mode because their adapters own first visibility.
Catalyst requires Explicit manager lifetime: the manager does not request
process exit; UIKit owns native application termination. Scene restoration can
replace initial size/placement; State reports the observed client size. Use
SetSizeAsync after readiness when a specific post-launch size is required.

| Feature | Catalyst behavior |
| --- | --- |
| Size/min/max/resizable | UIKit geometry requests and scene restrictions; UIKit resolves actual geometry. Mac points require Mac idiom. |
| Title/show/focus | Scene title and UIWindow.MakeKeyAndVisible; OS policy decides application activation. |
| Appearance | Opaque System/Solid body and native System titlebar; System/Explicit theme. Base-color replacement requires recreation. |
| API CloseAsync | Controller cancellation, GPU retirement, scene destruction, then registry removal. |
| Native close/quit | UIKit owns these paths. `Capabilities.CanCancelNativeClose=false`; RegisterClosing does not intercept native close. Scene disconnect retires the managed host. |
| Unsupported | Manual/WhenReady startup, hidden/custom/frameless chrome, Acrylic/LiquidGlass/transparent desktop material, MacOSBackdrop override, app theme bridge, placement/centering, hide, topmost/Dock policy, programmatic minimize/maximize/restore/full-screen/drag/resize, additional windows/owners. |

Fullscreen changes made using native controls are observable; the adapter reports
Normal/FullScreen only, so PresentationState is not a minimized/maximized detector.
UIKit safe-area insets remain available to rendered content. Bounds remains
null because global physical-pixel geometry is not mapped. Native caption
reservation is owned by UIKit, so the adapter does not invent custom chrome
metrics. Current results are in the [Catalyst validation record](../../history/26-09-26/desktop-window-api-summary.md).

The behavior boundaries follow Apple's public
[geometry request](https://developer.apple.com/documentation/uikit/uiwindowscene/requestgeometryupdate(_:errorhandler:))
and [scene lifecycle](https://developer.apple.com/documentation/uikit/uiwindowscene)
contracts. Native AppKit window selectors are not used to fill gaps in UIKit.

## Linux Qt Quick — 2026-09-29

Enable `DorotiQtQuick=true` and select the template's
`LinuxDesktopStartup` companion. The source Testbed uses the opt-in build property
`-p:DorotiLinuxDesktop=true`; its default legacy runner is preserved.
The adapter requires Graphite/Vulkan and the rebuilt app-owned native shim.
Desktop ABI 1 (48-byte table, 40-byte command/state) is separate from host ABI 6.
Desktop bootstrap sources use a startup-specific filename, so a legacy build or
evaluation cannot overwrite the source selected by a Desktop build.
The current managed host requires rebuilt app-owned shims for services and additional-window exports; do not mix it with older native payloads. Non-Quick Desktop startup is rejected at the SDK and native boundaries.

| Feature | Current behavior |
| --- | --- |
| Creation | Main and additional QQuickWindow, fresh content factory and independent dispatcher/session/native owner; one QApplication. Explicit can reopen after all windows close |
| Startup | `PlatformDefault`; readiness follows the first composed `frameSwapped`, including a valid replay. No hidden-first-frame promise. Manual/WhenReady and initially minimized startup rejected |
| Size | Integral logical client size/min/max in [1, 16777215], resizability and null limit restoration; native decorations excluded |
| Commands | Title, show/hide, focus request, maximize/restore/full-screen; presentation commands wait for an observed platform state, with a five-second failure deadline |
| Minimize | xcb only; native Wayland rejects programmatic minimize because acknowledged minimized state is unavailable |
| Placement/system policy | Bounds=null; Position/SetBounds/Center, topmost, taskbar changes, deferred system drag/resize rejected |
| Appearance | Opaque System/Solid client background, optional system dark background, system theme, Normal/System native decorations. Changed runtime snapshots require recreation and are rejected |
| Close/lifetime | Native `QCloseEvent` and API close share cancellable decisions. Approved destruction drains window resources before Closed/registry removal. OnLastWindowClosed quits; Explicit leaves the process running |

Native state is read on the Qt GUI thread. The adapter separates requested
`windowStateChanged` signals from platform `WindowStateChange` events, since
[Qt's setter also emits the signal](https://github.com/qt/qtbase/blob/v6.10.2/src/gui/kernel/qwindow.cpp#L1379-L1396).
Compositors can refuse focus or state changes; a timed-out presentation request
fails rather than setting a synthetic state. Size constraints remain native WM
hints. Changed constraints request a Quick update so idle rendering still commits
the Wayland surface's size hints.

Legacy Wayland compositor blur/client caption and Quick in-app blur are retained.
They are not exposed as new Desktop Acrylic/Hidden+Native support. Custom chrome,
App/Explicit theme, runtime background changes, physical input/Orca, mixed DPI,
clean-machine deployment and the known xcb/XWayland Vulkan extent race remain
outside the verified basic adapter. See the [Linux execution evidence](../../history/26-09-26/desktop-window-api-summary.md).


The [2026-09-29 Linux run](../../works/results/2026-09-29-linux-qt.md) exercises
both QPA paths, native editor owners, main-close/survivor resize and new-window
creation, zero-window Explicit reopen and one exit notification. Application-wide
queued work survives main-window closure. Owner/modal/Satellite/popup/tooltip and
cross-window reparenting remain unsupported. `_window_linux.cs` is not a second
supported Desktop creation path.
