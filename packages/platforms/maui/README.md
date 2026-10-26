# Maui provider

Native hosts for Android, iOS, AppKit macOS, Mac Catalyst and optional Windows MAUI applications. Version: `0.4.1-beta`; supported core: `[0.4.0-alpha.1, 0.5.0)`.

See the [platform prerequisites](../../../Doroti/README.md#platform-prerequisites) and [sample app guide](../../../samples/DorotiTestbedApp/README.md#run-by-platform) for setup and launch commands.

## Window management and application lifecycle

Apple Windowing maps logical requests through each provider policy. AppKit supports
owned Dialog/Popup/Tooltip/Satellite windows and typed NSMenu/menubar registrations;
Catalyst supports activating, unowned Regular scenes with UIKit first visibility.
Unsupported owner, modality, placement and visibility requests fail before allocation.
AppKit character shortcuts are supported; platform roles and logical-key shortcuts
remain Unsupported.

Mobile surfaces borrow one application session and retain separate view identities.
View shutdown first unmounts and quiesces callbacks, then drains typed calls and
UIKit/Android Vulkan GPU retirement. Explicit application shutdown uses
`DorotiMauiApplication.StopAsync()` on the native main thread; Desktop applications
use the window manager's exit contract. Backgrounding and temporary handler loss
retain the application tree.

## System bars

`AppBar.systemOverlayStyle` and `AppBarThemeData.systemOverlayStyle` select a
`Doroti.Framework.Services.SystemUiOverlayStyle`. If neither is set, the AppBar
infers the status bar icon brightness from its background. For a screen without
an AppBar, use `Doroti.Framework.Widgets.AnnotatedRegion<SystemUiOverlayStyle>`:

```csharp
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Material;
using Doroti.Ui;

var screen = new Scaffold(
    appBar: new AppBar(
        title: new Text("System bars"),
        systemOverlayStyle: SystemUiOverlayStyle.light),
    body: new Text("Content"));

var customScreen = new AnnotatedRegion<SystemUiOverlayStyle>(
    value: new SystemUiOverlayStyle(
        statusBarIconBrightness: Brightness.dark, // Android: dark icons
        statusBarBrightness: Brightness.light,   // iOS: light background, dark icons
        systemNavigationBarIconBrightness: Brightness.dark),
    child: new Scaffold(body: new Text("Custom header")));
```

The renderer samples the top and bottom system bar regions every frame. On
Android the top region supplies status bar settings and the bottom region supplies
navigation bar settings. A containing annotation can therefore control the bottom
bar while the AppBar's own annotation controls the top bar. `sized: false` makes an
annotation unbounded. Page push/pop and annotation updates select the visible
region automatically. When no region is present, the last style remains active.

The MAUI Android host applies icon brightness, supported bar/divider colors and
contrast settings on the native main thread. Android 15+ ignores the legacy bar
background color setters; paint the desired background behind the transparent
system bars in the widget tree. Navigation icon styling requires Android 8.0+,
divider colors Android 9+, and contrast control Android 10+ (status bar contrast
control applies before Android 15). iOS applies `statusBarBrightness` through
Doroti's page controller; keep `UIViewControllerBasedStatusBarAppearance` enabled
(the iOS default). iOS does not use Android bar colors, navigation bar settings or
`statusBarIconBrightness`. Desktop/Web hosts have no corresponding mobile bars.

`SystemChrome.setSystemUIOverlayStyle(style)` is also available inside an owning
view callback. Calls in one event coalesce to the last style, and equal styles are
cached per view. Omitted/null fields retain their native values. Styles survive
temporary native handler replacement; the host reapplies them when the surface
loads again. This support concerns overlay appearance; system UI visibility modes
and orientation requests are separate APIs.

## Development sessions

The tooling provider discovers available iOS simulators and connected paired
physical devices through typed device results. Run selects a device matching the
explicit RID and supplies the SDK `_DeviceName`; ambiguous selection fails.
Android Debug Mono development uses the provider-packaged ADB adapter
(Python 3) and declares nonce-matched graceful Stop. The adapter owns the
device/package lease, SDK watcher and recorded USB port independently of the
tool connection. Release/AOT is outside this Hot Reload profile; Android x64
device Hot Reload remains unverified. Apple Debug development uses the packaged
Mac/iOS Python adapters using the declared TFM/RID and runner SDK directory.
AppKit/Catalyst enable their development startup/transport policy; iOS simulators
use local request/frame acknowledgments, .NET 10 iPhones use the SDK LAN relay,
and .NET 11 CoreCLR devices use SDK USB deltas plus CoreDevice acknowledgment files.
The adapters retain process-held session/device leases and nonce-matched graceful
Stop. Apple runtime and installed-extension behavior still need verification on
the selected host and device. See [Hot Reload setup](../../../tools/vscode-doroti/README.md#hot-reload)
for connection requirements.

The shared iOS runtime profile is configured in
`packages/platforms/build/Doroti.IosNativeAot.props`, including for template consumers.
