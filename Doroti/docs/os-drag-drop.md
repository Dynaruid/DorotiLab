# OS Drag & Drop reception

The first implementation receives filesystem files, Unicode text and URI lists on
Windows App SDK and the browser DOM through the render-worker bridge. It is independent of framework `Draggable`/`DragTarget` widgets.
The public contracts live in `Doroti.Ui`; `OsDropReceiver` in `Doroti.Hosting`
handles negotiation, delivery and resource lifetime. Other hosts, including Android, report no
`platform.os-drag-drop` capability yet. Browser regression uses synthetic DataTransfer
events and does not establish Explorer gestures. Both adapters currently receive Copy only.

## Application API

Register one receiver for the current view and dispose it with the receiving screen:

```csharp
var support = OsDragDrop.support(view);
if (!support.CanReceive) return;
var registration = OsDragDrop.register(view,
    new OsDropOptions(OsDropAction.Copy,
        [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]),
    onEvent);
// registration.Update(options with { Bounds = logicalRect });
// registration.Dispose() when the receiving screen leaves the view.
```

Events are `Enter`, `Over`, `Leave`, `Drop` and `Error`. Positions are view-client
logical pixels, not screen pixels or widget-local coordinates. Optional `Bounds`
uses that same space. Update it after layout/scroll/transform changes; the first API
does not automatically register transformed widget hit-test regions. Only one
registration is active per view. Duplicate registration throws rather than routing
a drop unpredictably. `Over` delivery coalesces within consecutive over events;
it never crosses a leave/enter/drop boundary.

The native callback negotiates actions from the source offer, receiver options and
host capabilities. App callbacks run later on the framework input/frame queue.
Windows currently advertises **Copy only**, `CanSend=false`, `VirtualFiles=false`.
Shift requests Move, Alt/Ctrl+Shift requests Link and Ctrl requests Copy. Unsupported
explicit actions return None. A plain drop prefers Copy. No path/URI is executed,
no file is moved/deleted and no original data ownership transfers to the receiver.
Copy acceptance means the payload was acquired and queued, not that application
processing or persistence has completed. Future Move support must define durable
receiver completion and source deletion/rollback; it cannot reuse this receipt.

## Data and cancellation

`OsDropEvent.Data` is non-null only for a delivered drop. The receiver owns it:

```csharp
using var data = item.Data;
if (data is null) return;
foreach (var file in data.Files)
{
    var buffer = new byte[65536];
    var count = await file.ReadAsync(0, buffer, data.Lifetime);
    // Continue with a long offset as needed; the whole file is never preloaded.
}
```

Files share M4's `IPickedFile` contract. WindowsFilePicker and OS drops now use the
same `WindowsReadFile` read-only handle implementation. Names and snapshot lengths
are available without reading contents. Reads support cancellation and 64-bit
offsets; callers bound their buffer size. The read handle permits other programs
to modify/delete the original, so length/content can change. Permissions can fail
at acquisition or reading and must be handled by the app.

Disposing a payload cancels its `Lifetime` token and closes every file. Disposing
the registration or closing the view revokes retained payloads, including payloads
still waiting in the dispatch queue. A queued callback cannot reach a removed or
replaced registration. Data acquired after concurrent unregister is disposed and
the native drop is rejected. Leave/cancel performs no file acquisition. There is
no persistent file permission grant, directory traversal or write permission API.

## Windows boundary

`WindowsOsDropTarget` initializes OLE on the owner UI thread and registers the main
and rendering HWNDs. It uses the standard
[RegisterDragDrop / IDataObject path](https://learn.microsoft.com/en-us/windows/win32/shell/dragdrop).
The runner revokes targets before native window destruction, then releases payloads.
External hosts must follow that same ordering; `Dispose` runs on the registration
thread. The receiver remains separate from native child controls' own drop behavior.

Hover uses `QueryGetData` only. Drop acquires `CF_HDROP`, `CF_UNICODETEXT`,
`UniformResourceLocatorW` or UTF-8 `text/uri-list` HGLOBAL data. Every `STGMEDIUM`
is released in a finally block. No cross-thread `IDataObject` is retained.
See [Shell clipboard formats](https://learn.microsoft.com/en-us/windows/win32/shell/clipboard).
Only receiver-accepted formats are materialized. Files are limited to 1,024 per
drop and text/URI metadata to 1 MiB. Invalid/unterminated text, malformed URIs,
missing files or partial acquisition reject the whole drop and release resources.
Virtual/delayed files (`FILEDESCRIPTOR`/`FILECONTENTS`), directories and unsupported
clipboard formats are not advertised as supported.

Every callback converts screen coordinates relative to the rendering client HWND
using the current DPI. Nonclient and out-of-view positions are rejected. A native
dialog/control above the rendering surface can own its own OLE target; this change
does not steal native text editor/WebView drop behavior or promise it as a Doroti
view drop. Window-spanning routing is separate from M6 multiwindow work.

## Reproduce

Run all tests with the repository's 1,200-second timeout:

```powershell
python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Drop.Tests/Doroti.Drop.Tests.csproj
python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Drop.Windows.Tests/Doroti.Drop.Windows.Tests.csproj -- temp/testing/m5a/manual
python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_windows_packages.py --drop
$env:DOROTI_SAMPLE = 'drop'
pwsh Doroti/eng/doroti.ps1 run --app samples/DorotiTestbedApp --platform windows
```

The native executable also accepts `--dpi-unaware` (96 DPI coordinate contract)
or `--interactive` (real OLE drag source labels and a receiving HWND in one window).
The automated native suite uses callbacks with real Windows IDataObject/HGLOBAL
and filesystem handles; it does not pretend to be an Explorer gesture. It creates
a 5GB sparse fixture and verifies bytes at offset 4GB without allocating a 5GB buffer.
The package suite publishes a ProjectReference-free Release consumer, uses a fresh
NuGet cache, checks 96/192 DPI on this machine and cleans its successful raw run.
Direct native test invocations leave their explicitly supplied evidence directory
for inspection; record results and remove it after use.

Actual cross-process Explorer drops, live mixed-monitor DPI changes, cancellation
during an external drag and physical input remain distinct from these results.
See [M5-A results](../../history/26-10-03/works/common/07-os-drag-drop.md) for executed coverage and
remaining AppKit/Qt/Web/mobile/sending work.


## Qt receiver and optional drag source (2026-09-29)

Linux registers a view-owned Copy receiver using the same `OsDropReceiver` as the
other adapters. Native MIME offers use logical window coordinates. Only selected
formats are acquired; regular-file handles remain bounded/random-access and are
revoked on receiver or window disposal. Native controls retain their own drop
handling when the framework receiver does not accept the offer.

The separate `platform.os-drag-source` capability implements
`IOsDragSourceHostCapability`. `StartDragAsync(new OsDragSourceData(Text: "text",
Uris: [new Uri("https://example.com")]))` starts QDrag from an application drag
gesture. Optional `ImagePng`/`ImageHotspot` provide a drag image. Copy/Move/Link are
negotiated; `OsDragResult.Action` reports the destination decision. The application
owns any source deletion after Move. Existing file URIs are supported references;
virtual-file stream providers are unsupported. There is one native drag per process.

Testbed `DOROTI_SAMPLE=drop` exposes a draggable text/link area on supporting hosts.
[Linux evidence](../../history/26-10-03/works/results/2026-09-29-linux-qt.md) covers synthetic native
Copy delivery, 5GiB read lifetime and source cancellation. External file-manager
reception and successful cross-application Move/Link/image delivery remain
notVerified. Windows/Web receive capabilities do not imply this source capability.


## AppKit macOS Copy receiver

The Doroti Metal NSView registers native file URL, URL and UTF-8 pasteboard types.
Enter/over/drop negotiation uses view-local logical coordinates and OsDropReceiver.
Only accepted formats are acquired, with at most 1,024 items / 1 MiB text; file
contents use bounded read grants instead of copying whole files into memory.
Native editor/WKWebView children retain their own drop handling. CanReceive is
true, CanSend is false, Actions is Copy. Move/Link, drag images, virtual files and
OS drag source sessions remain unsupported. [Native pasteboard evidence](../../history/26-10-03/works/results/2026-09-29-macos-appkit.md)
does not qualify physical Finder delivery across windows.
