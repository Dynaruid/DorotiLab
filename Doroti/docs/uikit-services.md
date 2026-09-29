# iOS and Mac Catalyst services

The UIKit host registers view-owned FilePicker, URL launcher and OS drop capabilities.
Use the common `Doroti.Plugins` and `Doroti.Ui` contracts from the application;
no UIKit references are needed in shared widget code.

`UIKitFilePicker` presents `UIDocumentPickerViewController` in the requesting
window. One request may run per view. Request cancellation and owner disposal
cancel the result and await native dismissal before another request may start.
Selected regular files retain a security-scoped URL and read handle; reads use
64-bit offsets and return at most 64 KiB per call. Disposing a grant or its owner
revokes access. Directories and virtual files are not random-access file grants.
Document-provider download/coordination failures are reported as failures, not
successful empty selections. iCloud/offline provider and actual permission-dialog
qualification remain separate from local-file tests.

UIKit drop uses a native `UIDropInteraction` on the renderer view, retained across
handler attachment. It accepts Copy for text and URI lists, at most 128 items and
1 MiB of decoded payload. Provider loading is asynchronous; view close cancels
pending reads, and registration replacement prevents late delivery to a different
receiver. Bounds and coordinates use the receiving view's logical points. Native
editors/WebViews keep their own drop interactions. File streams, drag source,
drag images, Move/Link and virtual files are **unsupported** in this adapter.
Query `OsDropSupport` before constructing a receiver; enum members alone do not
indicate platform support.

The registered scene delegates deliver connection URL contexts and browsing
`NSUserActivity` to the application activation queue before MAUI creates its
window. Warm callbacks deliver through the same main-view Navigation owner.
Malformed locations are ignored, the queue is bounded, and lifecycle transitions
checkpoint the existing restoration store. Additional Catalyst windows have
independent routes and no shared main restoration namespace or activation lease.

Custom schemes belong in the app's `CFBundleURLTypes`; Testbed registers
`doroti-testbed`. Real Universal Links additionally require an app-owned HTTPS
domain, matching Associated Domains entitlement and hosted `apple-app-site-association`.
The framework does not register a developer's domain or add a fictitious production
association. Follow [Apple's associated-domain setup](https://developer.apple.com/documentation/xcode/supporting-associated-domains).
A direct native activity callback test does not establish OS website association.

Catalyst Desktop requires Mac idiom, multiple-scene adoption, Graphite and
`WindowLifetimePolicy.Explicit`. Create additional windows through
`DesktopWindowContext.Windows`; MAUI `OpenWindow` supplies the actual scene.
Each scene owns its dispatcher, input, semantics, native hierarchy and GPU queue;
application plugin resources share a retained boundary. Framework unmount precedes
GPU retirement and scene destruction. UIKit does not provide cancellable native
close: `CanCancelNativeClose=false`, while API close can still be cancelled.
Unsupported positioning, hidden launch, native minimize/fullscreen commands and
AppKit materials remain explicit policy rejections. Explicit exit drains the
managed windows/resources; UIKit owns process termination. Native New Window and
automatic restoration of extra scenes have no content factory and are rejected.

For repeatable native smoke commands and precise evidence boundaries see
[the Apple work2 results](../../works/results/2026-09-29-ios-catalyst.md).
