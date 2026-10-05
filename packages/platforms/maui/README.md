# Maui provider

Host and Target sources are owned by this independently versioned provider. Package identities are unchanged. Version: 0.4.0-alpha.1; supported core: [0.4.0-alpha.1, 0.5.0). Native/bootstrap adoption and physical platform acceptance are tracked separately in the design-platform execution record.

Apple Windowing maps logical requests through each provider policy. AppKit supports
owned Dialog/Popup/Tooltip/Satellite windows and typed NSMenu/menubar registrations;
Catalyst supports activating, unowned Regular scenes with UIKit first visibility.
Unsupported owner, modality, placement and visibility requests fail before allocation.
AppKit character shortcuts are supported; platform roles and logical-key shortcuts
remain Unsupported. See [desktop window behavior](../../../Doroti/docs/desktop-windows.md).

Mobile surfaces borrow one application session and retain separate view identities.
View shutdown first unmounts and quiesces callbacks, then drains typed calls and
UIKit GPU retirement. Explicit application shutdown uses
`DorotiMauiApplication.StopAsync()` on the native main thread; Desktop applications
use the window manager's exit contract. Backgrounding and temporary handler loss
retain the application tree. This does not yet qualify every staged lifecycle race.

The tooling provider discovers available iOS simulators and connected paired
physical devices through typed device results. Run selects a device matching the
explicit RID and supplies the SDK `_DeviceName`; ambiguous selection fails.
Mobile typed development transport is still Unsupported. The iOS runtime profile
is owned only by `packages/platforms/build/Doroti.IosNativeAot.props`, including
template consumers. Latest scope and remaining gates are in
[work3](../../../work3.md#14-2026-10-05-실행-결과).
