# Native plugin packages

The M4 increment reuses `IDorotiNativePluginHandler`, application manifests and
the runner's generated registrations. `Doroti.Plugins` supplies the shared
FilePicker/URL client and its stateless `NativeFeaturesHandler`. The first host
adapters are Windows App SDK, browser DOM through the render-worker bridge, Android Storage Access Framework, and Linux Qt FilePicker/xdg-open. Other native adapters remain follow-up
work; `NativeFeatures.ForView(view)` reports unsupported when its channel is absent.

## Consume

Reference `Doroti.Plugins` version `0.3.0-beta` from the application. Its
`buildTransitive` target contributes the handler to Windows, Web, Android and Linux runners; the runner
must use the matching updated `Doroti.Runner.Sdk`. No product source copying,
reflection discovery, or new Source Generator is required.

```csharp
var features = NativeFeatures.ForView(view);
var supported = await features.GetCapabilitiesAsync(cancellationToken);
if (supported.FilePicker)
{
    await using var selection = await features.PickFilesAsync(
        new FilePickOptions(AllowMultiple: true, Extensions: [".txt"]), cancellationToken);
    foreach (var file in selection.Files)
    {
        var buffer = new byte[65536];
        var count = await file.ReadAsync(0, buffer, cancellationToken);
        // Consume buffer[..count]. Repeat with an increased long offset as needed.
    }
}
var launch = await features.LaunchUrlAsync("https://example.com", cancellationToken);
```

`FilePickStatus` distinguishes selected, cancelled, denied, unsupported and failed.
User dismissal returns `cancelled`; request/owner cancellation throws
`OperationCanceledException`. URL results distinguish opened, blocked, unsupported,
invalidUrl and failed. Only http/https/mailto are accepted; `opened` means shell
acceptance, not remote page loading. Invalid options/protocols are programmer errors.

Files expose a display name, snapshot length and bounded random reads (at most
64 KiB per request, 64-bit offsets). Native paths do not cross the plugin boundary.
Windows retains a read-only OS handle; it neither reads the entire file nor creates
a persistent permission grant. Dispose each selection/file to release it. Owner
close also revokes handles. Other programs may change a selected file, so reads can
return fewer bytes than the original length suggests. This first API has no save,
directory picker, write grant, or permission persistence across process restarts.

## Package format and discovery

An external package contains its managed handler assembly, ordinary NuGet
dependencies, and `buildTransitive/<PackageId>.targets`:

```xml
<Project>
  <ItemGroup Condition="'$(DorotiTarget)' == 'Windows'">
    <DorotiNativePlugin Include="example.plugin"
      Channel="example/plugin" Codec="json" AbiVersion="1"
      HandlerType="Example.PluginHandler" PackageId="Example.Plugin"
      Version="1.0.0" Rid="$(RuntimeIdentifier)" />
  </ItemGroup>
</Project>
```

The handler must have a public parameterless constructor. The runner emits
`new global::Example.PluginHandler()` into its existing registration array and
embeds `Doroti.Application.PackagePlugins` alongside the existing application
manifest. Loading merges descriptors, validates duplicate ids/channels, reserved
`flutter/` channels, RID, ABI, missing handlers and handler types. Existing browser
manifests retain their `generated-js-registration` marker. Handlers omitted from a
manifest are rejected instead of being silently ignored.

Use standard `runtimes/<rid>/native/` NuGet layout for native libraries. Extra
required deployment files may use `DorotiPluginAsset Include="..." Link="..."`;
the runner checks their source exists and copies them to output/publish. Assets
must be declared by the target-specific package. Native loader dependency and
ABI checks remain the handler's initialization responsibility. Build diagnostics
`DOROTIPLUGIN001..006` cover incomplete metadata, RID, duplicates, missing declared
assets and metadata syntax. CLR registration code and JSON only accept simple
qualified type names and restricted metadata characters.

Web modules continue to use the existing `DorotiJavaScriptPlugin` items
(`Channel`, `ModuleUrl`, `ExportName`) and `DorotiApplicationPluginRegistration` /
`BrowserJavaScriptPluginHandler`. Package modules as browser content under a stable
URL; do not attempt to load native RID assets in the browser. NativeFeatures has no
Web module yet. Build-time module existence/integrity verification is not newly
provided by this increment.

## Ownership, dispatch and cancellation

Handlers are application-owned. `IDorotiViewPluginHandler` is an optional ABI
extension receiving a fresh `DorotiPluginContext` per configured view. It must not
store window state on the shared handler. Context resource tokens are isolated
between views; `Retain`/`Require`/`Release` manage `IDisposable` resources. Pass the
request token when retaining newly acquired resources so cancellation before reply
delivery releases them. Retaining after owner close disposes the resource and fails.

The message scope links caller and owner cancellation. Shutdown stops new requests,
cancels waiters and rejects late replies. A handler that ignores cancellation keeps
its application lifetime until the operation finishes; it cannot keep its caller
waiting indefinitely. Such a permanently hung native operation still needs host
process recovery; forced unsafe disposal is deliberately avoided. Handler disposal
is once-only, deferred until outstanding calls finish. Cleanup attempts every resource
even when another disposal throws.

Calls can arrive from a framework/render thread. Host adapters own UI dispatch;
plugins must not assume a process-global SynchronizationContext. `WindowsFilePicker`
is created on the HWND thread, runs each dialog on a dedicated STA thread,
allows one dialog per owner and drains before HWND teardown. The runner closes
the plugin scope before disposing the picker. Its private dispatcher delivers
[IFileDialog.Close](https://learn.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ifiledialog-close)
inside the dialog's modal loop. This avoids blocking the framework or owner UI
thread and closes the real dialog on caller cancellation. The native HWND remains
the dialog owner; results and read grants return through the same plugin channel.

The current wire contract is request/reply JSON (`capabilities`, `pick`, `read`,
`release`, `launch`). Event streams use the separate `platform.plugin-events` capability. Implement
`IDorotiPluginEventHandler.SubscribeAsync(context, channel, codec, arguments, token)`
and consume `IPlatformPluginEventsHostCapability.SubscribeAsync`. `EventChannels`
reports which registered handlers implement the extension; unsupported channels throw
`NotSupportedException`. The handler ABI and request/reply channels remain unchanged.

Each subscription has a lossless bounded queue (default 16; allowed 1..1024), one
pending producer item, and a 64 KiB limit per event. Delivery copies each payload.
Producers must await demand and release native listeners in their iterator `finally`.
Caller cancellation, early enumeration disposal and owner close stop delivery. A
noncooperative native iterator retains the handler lease until it actually exits;
it cannot deliver a late event into a closed view. App callbacks run on the reader's
chosen context; marshal to the view dispatcher before touching widgets.

Browser file selection requires transient user activation. Grants expose bounded
reads and no local paths. Cancelling detaches the input and ignores late selections;
programmatically dismissing the browser's OS chooser is not guaranteed. Android uses
an unexported proxy Activity, closes its picker on cancellation and keeps grants
view-scoped without taking persistent URI permissions. Unknown-length documents are
rejected; nonseekable providers are reopened and skipped using bounded memory.
Custom browser hosts with navigation must await
`BrowserFrameworkHost.PrepareNavigationAsync(restorationId)` before `CreateView`;
the generated runner does this automatically.

## Reproduce and evidence boundaries

All commands use the repository's 20-minute process-tree timeout:

```powershell
python Doroti/eng/run-with-timeout.py dotnet run --project Doroti/tests/Doroti.Plugin.Tests/Doroti.Plugin.Tests.csproj
python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_packages.py
python Doroti/eng/run-with-timeout.py python Doroti/tests/plugin_windows_packages.py
$env:DOROTI_SAMPLE = 'plugins'
pwsh Doroti/eng/doroti.ps1 run --app samples/DorotiTestbedApp --platform windows
```

The last package suite requires an interactive Windows desktop and opens one
loopback URL in the default browser. It selects its own temporary fixture using
dialog messages; this is actual OS dialog/handle execution with synthetic input,
not physical mouse/keyboard evidence. Package suites isolate the NuGet cache and
remove raw evidence after success. On failure they print the investigation path.

Recorded results and remaining work: [M4](../../history/26-10-03/works/common/06-plugin-sdk.md),
[Windows](../../history/26-10-03/works/platforms/windows.md), [support index](support-status.md).


## Linux Qt

The host passes generated `NativePluginHandlers` to the application boundary.
The app-owned shim implements asynchronous QFileDialog selection with view/caller
cancellation. Selected files become CLOEXEC regular-file read grants; FIFO/device/
directory selection is rejected without blocking. `NativeFeatures` retains grants
in its view scope and releases them with the owner. The URL adapter accepts
http/https/mailto and uses xdg-open. [Linux qualification](../../history/26-10-03/works/results/2026-09-29-linux-qt.md)
records actual dialog automation, cancellation, denied file access, Unicode reads
and plugin-to-default-browser loopback delivery. Physical dialog/portal permission
flows remain separate. Rebuild the template/sample native shim with the host.


## AppKit macOS

NSOpenPanel sheets belong to the requesting view's NSWindow. Caller cancellation
and owner disposal cancel the panel. Security-scoped URLs and open file handles
provide bounded random-access grants; release or view closure revokes them.
NSWorkspace implements the URL launcher; the common NativeFeatures client keeps
its http/https/mailto policy. [AppKit results](../../history/26-10-03/works/results/2026-09-29-macos-appkit.md)
separate actual native panel cancellation and 128 MiB file reads from physical
selection, permission denial and native event source qualification.
