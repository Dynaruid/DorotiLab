# Window execution contexts

Decision (2026-09-29): Windows App SDK additional top-level windows use one
framework dispatcher/session and one native UI thread per window. This extends
`DorotiWindowManager` and its fresh `WindowContent` factory; it does not introduce
a second public window API. Each native loop already owns its render worker.

The existing widget entrypoint attaches a single root. A shared binding with a
`ViewCollection` would also require replacing that public entrypoint contract,
implicit-view services and native frame admission. Independent contexts preserve
those contracts. Binding accessors and mutable service singletons must resolve
through the active dispatcher, including async continuations. Dispatch remains
serialized within each dispatcher; different windows can render independently.

Ownership:

| Resource | Owner |
| --- | --- |
| Widgets/Scheduler/Services bindings, root, focus, pointer arena, keyboard/IME | dispatcher/window |
| Navigator, restoration, semantics, image/asset caches | dispatcher/window |
| PlatformView, plugin replies, textures, native GPU surface | view/native host |
| Application plugin handlers | shared application lease; disposed once after the final window and active call drain |
| Font/type descriptions, constants, immutable shared data | process |
| Desktop registry and application lifetime policy | manager |

Closing a window cancels its callbacks, unmounts the root, drains native/GPU work,
disposes its capabilities and removes its controller. It cannot reset another
context's text client or binding. Additional windows currently have independent
in-memory navigation and do not inherit the main window's persisted checkpoint.
Stable secondary-window restoration identities remain follow-up work.
`OnLastWindowClosed` exits after every native loop is drained;
`Explicit` requires an explicit manager exit request. Owner/modal/satellite and
cross-window widget reparenting are outside the initial implementation.

Validation must include two real HWNDs, independent framework updates, one-window
close with survivor activity and last-window cleanup. Context isolation tests
are a prerequisite, not a replacement for that native evidence. Mixed-monitor
DPI, physical IME and other host adapters retain separate qualification.


## Qt ownership (2026-09-29)

Qt uses **one QApplication/GUI thread** with a separate dispatcher/session for each
QQuickWindow. Rendering, input and lifecycle callbacks enter their window's dispatcher scope. Separate
PlatformView factories, renderer, Vulkan instance, plugin view scope and surface
prevent native identities from crossing owners. Immutable application resources
and handlers share the existing application lease. Additional windows have
independent in-memory navigation; main-window XDG restoration remains main-owned.
Application dispatch outlives the main window and ends with QApplication. A dropped
queued request receives a closed completion. Native surfaces retire before their
callback GCHandles are released. Both last-window and Explicit reopen policies have
[real Qt window evidence](../../history/26-10-03/works/results/2026-09-29-linux-qt.md).


## AppKit ownership (2026-09-29)

AppKit uses one main UI thread with a distinct framework dispatcher/session,
PlatformView factory and Metal surface per NSWindow. Additional windows share
the application boundary through leases; closing the main window does not dispose
services still in use. The manager retains the application lease until app exit.
The main view receives OS activation and owns persistent restoration; secondary
views use independent in-memory navigation. Root unmount precedes view/capability
disposal and Metal retirement completes before native close. OnLastWindowClosed
and Explicit have [real two-window evidence](../../history/26-10-03/works/results/2026-09-29-macos-appkit.md).
The framework `_window_macos.cs` Satellite API is not connected to this manager.
Owned/modal/Satellite/popup/tooltip windows remain unsupported.

2026-09-29 UIKit/Catalyst: [implementation, smoke commands and remaining qualification](../../history/26-10-03/works/results/2026-09-29-ios-catalyst.md). iOS Simulator, device signing and Catalyst scenes have separate evidence; package-only publish does not imply clean signed distribution.
