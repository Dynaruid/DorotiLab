# Optional desktop companion

This assembly is intentionally separate from the common app. It is currently
available to **Windows MAUI**, **AppKit macOS**, **Mac Catalyst**, and **Linux Qt Quick** runners. The template's default WindowsAppSDK
runner remains unchanged; its desktop controller adapter is not implemented yet.

In a Windows MAUI or AppKit macOS runner, set:

```xml
<DorotiDesktopProject>../desktop/DorotiTemplateApp.Desktop.csproj</DorotiDesktopProject>
<DorotiDesktopStartupType>DorotiTemplateApp.Desktop.DesktopStartup</DorotiDesktopStartupType>
```

The SDK registers the companion startup and references its assembly. Never add
this project to the common app, Web, Android or iOS runner. These
references fail with a `DOROTIDESKTOP` diagnostic. Custom/hidden title bars and
the WindowsAppSDK desktop adapter remain pending.

AppKit sizes are the unobscured client area in points. Per-window Dock hiding,
global physical-pixel Position/SetBounds and programmatic resize initiation
are rejected. Liquid Glass needs macOS 26; earlier systems use the explicitly
selected Solid/Transparent fallback. AppKit Acrylic does not accept Windows
tint/luminosity controls. Mac Catalyst uses the separate startup below.


For a Mac Catalyst runner, select the UIKit-specific startup instead:

```xml
<DorotiDesktopProject>../desktop/DorotiTemplateApp.Desktop.csproj</DorotiDesktopProject>
<DorotiDesktopStartupType>DorotiTemplateApp.Desktop.MacCatalystDesktopStartup</DorotiDesktopStartupType>
```

This requires the scene manifest in the Catalyst Info.plist, Mac idiom
(UIDeviceFamily=6), Graphite and Mac Catalyst 16 or newer. The Xcode 27 profile
uses `-p:DorotiMacCatalystTargetFramework=net10.0-maccatalyst27.0` and requires
Mac Catalyst 17 or newer. The default profile retains its existing minimum.

Catalyst supports native System titlebar, opaque System/Solid background,
title, client size/min/max/resizability, show/focus requests and explicit theme.
Use `PlatformDefault` startup and `Explicit` manager lifetime. UIKit owns first
visibility, native close and application termination. API CloseAsync can be
canceled, but `Capabilities.CanCancelNativeClose` is false. Desktop blur, hidden
startup/chrome, hide, placement, topmost, Dock policy and programmatic
minimize/maximize/full-screen are explicitly rejected.

When enabling the Catalyst desktop companion on the default TFM, set
`SupportedOSPlatformVersion` to at least `16.0`; the Xcode 27 profile requires
`17.0`. The non-desktop template retains its older minimum.

Catalyst scene close requires `UIApplicationSupportsMultipleScenes=true` (already
set in the template plist). The Desktop adapter still rejects additional native
windows. UIKit can restore the previous initial size; use SetSizeAsync after
readiness when an exact post-launch size is required.

For Linux Qt, use the separate startup and explicitly enable Quick:

```xml
<DorotiQtQuick>true</DorotiQtQuick>
<DorotiDesktopProject>../desktop/DorotiTemplateApp.Desktop.csproj</DorotiDesktopProject>
<DorotiDesktopStartupType>DorotiTemplateApp.Desktop.LinuxDesktopStartup</DorotiDesktopStartupType>
```

Qt supports one Graphite/Vulkan Quick window with PlatformDefault startup,
opaque System/Solid background and Normal/System native decorations. Title,
integral logical client size/min/max/resizability, show/hide/focus requests,
maximize/restore/full-screen and cancellable native/API close are available.
Programmatic minimize is restricted to xcb; native Wayland rejects it because
the protocol does not expose an acknowledged minimized state. Hidden startup,
global physical placement/centering, topmost/taskbar policy, deferred drag/resize,
custom chrome and runtime appearance changes are rejected. Legacy Wayland blur
is retained for apps without a Desktop companion.

OnLastWindowClosed ends the Qt loop after window teardown and registry removal.
Explicit keeps the process alive; additional windows/reopen are unsupported.
Use the companion only when that lifetime fits the application. Rebuild the
app-owned native shim: Desktop ABI 1 is separate from unchanged host ABI 4.
Native Wayland and xcb/XWayland require separate qualification; the known
XWayland Vulkan resize issue remains open.
