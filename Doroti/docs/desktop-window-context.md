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
