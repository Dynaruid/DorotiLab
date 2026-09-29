# Application navigation and restoration

Opt in on the application view configuration:

```csharp
new DorotiViewConfiguration("My app", new Size(800, 600))
{
    Navigation = new ApplicationNavigationOptions("main-v1", "myapp"),
};
```

`PlatformRouteInformationProvider` and `SystemNavigator` use the view's typed
navigation capability. The existing `Router` parser/delegate controls valid
destinations and its own not-found screen. The existing `RestorationManager`
serializes restoration buckets. Apps still opt individual fields/scopes into
Flutter restoration; arbitrary widget state is not automatically serialized.

`ApplicationActivation` carries a delivery ID, URI, source, cold/warm flag and
optional JSON history state. Before a Router subscribes, at most 32 deliveries
are queued in order. Duplicate delivery IDs are ignored within the last 128 IDs;
revisiting the same URI with a new ID is allowed. Exactly one Router owns a view.
Invalid/control-character/unsafe-scheme URIs are rejected. Unsubscribing and
closing the owner prevent late delivery to a removed Router.

Checkpoint envelope v1 contains location, JSON route state, base64 Flutter
restoration bytes and a clean-shutdown marker. Binary data is capped at 4 MiB.
Unsupported envelope versions and corrupt envelopes fall back to `/` with empty
state and expose `RestoreFailure`; invalid data also discards the saved
clean-shutdown flag. v1 deliberately rejects unknown future
versions instead of guessing a migration. Apps changing their bucket schema
should change `RestorationId` or migrate their own values. A different explicit
cold link discards the saved navigation stack so it cannot override that link.
Each completed restoration update is saved; lifecycle transitions request a
flush. A forced kill can lose uncommitted work since the last checkpoint.

| Host | Connection and storage | Qualification boundary |
| --- | --- | --- |
| Windows App SDK | Protocol URI launch argument; current-user named pipe redirects later launches; atomic file under LocalAppData/Doroti/application-id/restoration | Native cold/warm Router and process restart exercised; installer must register the scheme. No registry mutation during app startup |
| Web main-owned runtime | Same-origin URL/history push/replace/popstate; sessionStorage per restoration ID | Build and history/storage contract tests; real browser unavailable in this session. Worker-owned framework navigation not advertised |
| Android MAUI | `DorotiMauiActivity` forwards OnCreate/OnNewIntent; runner declares IntentFilter; app-private atomic file | Galaxy S25 Android 16 Release/Mono AOT cold/warm/restart route display exercised through ADB intents, not physical user input |
| Qt Linux | LaunchContext protocol argument, current-user IPC, atomic XDG_DATA_HOME checkpoint (absolute fallback to ~/.local/share), hashed application namespace | VM cold/warm, clean restart, committed checkpoint after forced kill, bad-version fallback and cold-link priority pass; OS association and input/selection migration remain notVerified |
| iOS/AppKit/Catalyst | MAUI delivery API exists, native delegate/link association and other adapters are unfinished | notVerified; not claimed as implemented Universal Links |

Android runners should inherit `Doroti.Host.Maui.DorotiMauiActivity` and declare
their application-specific `IntentFilter`. The template now uses that base;
it does not register a placeholder protocol for every generated app. Web storage
denial/quota leaves navigation usable and reports a restoration diagnostic.
Browser history has one document owner; full worker-owned runtime support remains
separate from the existing render worker.
Malformed or foreign browser history state is ignored while its URL is still
delivered. Returning from the back/forward cache marks the checkpoint running
again; closing the owner detaches popstate, pagehide and pageshow handlers.

Run Testbed with `DOROTI_SAMPLE=navigation`; Windows protocol arguments use
`doroti-testbed:/first` and `doroti-testbed:/second`. Web main-owned Debug uses
`?dorotiSample=navigation`. On Android:

```powershell
adb shell am start -W -a android.intent.action.VIEW -d 'doroti-testbed:/first' --es doroti_sample navigation -p dev.doroti.testbed
adb shell am start -W -a android.intent.action.VIEW -d 'doroti-testbed:/second' -p dev.doroti.testbed
```

The sample displays the current URI and provides two navigation buttons plus
editable text/selection in route state. [08 results](../../works/common/08-navigation-restoration.md)
distinguishes common contracts, native callbacks and actual display evidence.


2026-09-29: generated browser runners prepare navigation state asynchronously before
view creation, then route DOM history and restoration through the render-worker control
mailbox. Main-owned runtime does not imply framework JavaScript runs on the DOM thread.
See [actual Chrome and Android follow-up](../../works/results/2026-09-29-web-windows-android.md).
