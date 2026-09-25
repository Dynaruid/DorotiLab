# Desktop window API — implementation status, 2026-09-25

The first implementation provides `Doroti.Desktop` and optional
`Doroti.Desktop.Widgets`. **The complete W0–W5 plan is PARTIAL.** Windows MAUI
has the native adapter. WindowsAppSDK raw, AppKit and Qt adapters and custom
title-bar widgets are still unimplemented. Native multi-window execution is W6,
outside this first implementation.

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
| AppKit | `AppKitWindowBackdrop`, macOS platform application | Existing path retained; new adapter pending |
| Qt | `DorotiQtRunner`, `QtTitlebarAppearance`, QML/native ABI | Existing path retained; new adapter pending |
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

| Capability | Windows MAUI | Other native hosts |
| --- | --- | --- |
| Main-window manager/controller + companion | Implemented | Adapter pending, startup rejected |
| Client size, physical bounds, center, min/max | Implemented; mixed-monitor matrix notVerified | Pending |
| Show/hide/focus/title/topmost/taskbar/resizable | Implemented | Pending |
| Minimize/maximize/restore/fullscreen | Implemented; not all states live-qualified | Pending |
| Native System/Solid/Backdrop caption | Implemented; Acrylic pixel response verified separately | Pending |
| Tint/kind/theme/reset | Snapshot replacement implemented | Pending |
| Material mode or renderer background change | RequiresRecreation, rejected | Pending |
| App theme bridge | Unsupported; System/Explicit supported | Pending |
| Hidden/custom/frameless chrome | Unsupported | Pending |
| Native pointer drag/resize commands | Pressed primary pointer required; command implemented, API-input matrix notVerified | Pending |
| Actual multiple native windows / owners | Unsupported | Pending |

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
