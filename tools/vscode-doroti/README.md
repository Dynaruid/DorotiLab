# Doroti for VS Code

Doroti project creation, platform launch commands, Hot Reload and C# widget editing in VS Code.

Requires VS Code 1.100+ desktop, PowerShell 7 and the repository's .NET 10 SDK. Available Debug development targets come from `doroti-workspace.json` and the provider profile for the current host. Building the extension requires Node 24 and npm with the checked-in lockfile.

## Install

```powershell
dotnet new install <repository>/Doroti/templates/Doroti.Templates/content/doroti-app
cd <repository>/tools/vscode-doroti
npm ci
npm run package
code --install-extension doroti-0.2.3.vsix
```

Set `doroti.cliPath` to the absolute repository `Doroti/eng/doroti.ps1` path for development commands. Editing uses the bundled .NET 10/Roslyn 5.9 helper and does not require a repository checkout, CLI path, project selection or an app session. Install the Microsoft C# extension (`ms-dotnettools.csharp`) for normal IntelliSense, signature help, diagnostics, navigation and general C# refactoring. Doroti supplies widget construction templates and structural assists alongside it; fallback type suggestions are suppressed while C# is active. Restore the application's declared dependencies before semantic editing. Suggestions never restore, build, install dependencies or save source files.

## Create and run a project

Use **Doroti: Create Project**, choose a name and parent folder, then **Select Project**, **Select Target**, **Run**. Existing folders are never overwritten. A canceled/failed creation may leave partial files at the logged destination. The manifest and repository CLI determine available runners. One local workspace folder and one running session are supported. Restricted Mode allows snippets/editing, but cannot execute creation, CLI or reload commands. No launch/tasks settings are overwritten.

Click the **Doroti** symbol in the left Activity Bar, or run **Doroti: Show Sidebar**, to open the **Workspace** view. **Project** selects the application and target, opens its manifest and creates projects. Expand **Select Target** to choose a declared target directly. **Development** shows connection/reload status and runtime revision, with Run, Hot Reload, Restart, Stop and Show Logs. **Widgets** contains the new-widget, snippet and wrap commands for the active Doroti application C# editor. Unavailable actions show their reason. The view title has session controls, Refresh Sidebar and an overflow menu with Doroti settings. Opening or refreshing the sidebar reads local context without building the app. You can move the view to the secondary sidebar or panel using VS Code's view menus.

## Hot Reload

The editor title and status bar contain **Hot Reload**. For a provider-advertised Debug target, edit a C# method body, click Hot Reload, and choose Save and Hot Reload. `dotnet watch` applies the actual metadata delta; a runtime acknowledgment follows widget/render reassembly on the owning UI queue. Saving files normally uses the same watcher. A no-op save does not claim a reload. Compilation errors appear in **Show Logs**; fix the code and retry. Unsupported edits require **Restart (resets state)**. The runtime ID and revision distinguish restarts from state-preserving reloads.

### Android

Select `android`, connect an authorized USB device/emulator and use **Run**. Python 3, Android platform-tools, .NET SDK 10.0.400+ and the matching Android workload are required. `doroti.device` selects a serial when several devices are attached. The workspace RID must match the device ABI. Save or **Hot Reload** applies supported C# edits over USB while preserving widget state, text and scroll. **Stop** closes the owned app, watcher and USB reverse port. Native Kotlin/Java, resources, project changes and unsupported C# edits require **Restart (resets state)**. Release/AOT and CoreCLR are outside this Debug Mono development profile; Android x64 device Hot Reload remains unverified.

### Web

Web opens an external browser connected to a local session bridge. Wait for its runtime to become ready before using Hot Reload; the button is disabled when no browser is connected. State, text and scroll survive supported edits without refreshing the page. Use one page per session. The main-owned threaded runtime is supported; Release/AOT and the explicit worker-owned runtime are not qualified. The bridge carries request/result IDs only; the .NET SDK applies code. Stop also closes the bridge.

### Apple platforms

The workspace declares the TFM, RID and profile; `doroti.device` selects a matching device where discovery is supported. Use **Select Target** to choose an alias advertised by the provider on the current host.

| Target | Development connection |
| --- | --- |
| AppKit macOS (`macos`) | Local Debug session through the packaged Mac adapter |
| Mac Catalyst (`maccatalyst`) | Local Debug session with the editable interpreter profile |
| iOS simulator (`ios`) | Matching simulator RID and local reload acknowledgments |
| iPhone / .NET 10 Mono | SDK LAN relay; Mac and iPhone must share a LAN |
| iPhone / .NET 11 CoreCLR | SDK USB transport with CoreDevice acknowledgments |

Apple development support is experimental. Runtime and installed-extension behavior need verification with the selected host, SDK and device; the .NET 10 iPhone LAN path remains unverified with the current provider. Release and NativeAOT do not support this Hot Reload workflow.

Apple development requires Python 3, Xcode and the matching .NET SDK/workloads. The manifest and runner must agree on TFM/RID; the provider uses the runner directory for SDK selection and does not pass a global RID to `dotnet watch`. On .NET 10 iPhones, keep the Mac and phone on the same LAN and allow local network access. The CLI's optional `-HotReloadHost <Mac-LAN-IPv4>` chooses that relay address; it is not an iOS TFM/SDK override. The selected SDK remains responsible for metadata deltas. Reload waits for request delivery and a completed frame acknowledgment. Stop uses a session-matched file signal, waits up to 60 seconds for adapter cleanup, and releases process-held session/device leases.

### Linux

Select `linux` and use **Run**, then save C# method-body edits or click **Hot Reload**. PowerShell 7, .NET 10 and the system Qt development/QML dependencies are required. The CLI defaults to polling file changes to handle low inotify limits and mounted source folders; it preserves an explicit watcher environment setting. Supported changes retain PID, State, text and scroll. Native C++/QML/shader assets and unsupported metadata edits need **Restart (resets state)**.

## Widget editing

Type `stl`, `stless` or `dstateless` for a StatelessWidget, and `stf`, `stful` or `dstateful` for a StatefulWidget. Choose the suggestion, edit the selected class name, then press Tab to reach build. Stateful widget, State and createState names stay linked. `dstlkey`/`dstfkey` include a key constructor. Imports are inserted with the snippet; aliases and same-name types use qualified references. File names supply valid default names, including Korean identifiers.

**Doroti: New Stateless Widget** and **New Stateful Widget** ask for the class, namespace, file and constructor style. Existing files are never overwritten; cancellation creates no file. **Doroti: Insert Widget Snippet** lists the applicable templates, including the `d*` prefixes below. Templates add the required imports and respect user `editor.snippetSuggestions` and `tabCompletion` settings.

| Templates | Prefixes |
| --- | --- |
| Layout | `drow`, `dcolumn`, `dcontainer`, `dtext`, `dpadding`, `dcenter`, `dsizedbox`, `dstack`, `dalign` |
| Flex children | `dexpanded`, `dflexible` (direct Row/Column/Flex children only) |
| Scroll/build/paint | `dlistview`, `dscroll`, `dbuilder`, `dlayoutbuilder`, `dcliprrect`, `dtextstyle`, `ddecoration` |
| Material | `dmaterial`, `dscaffold`, `dtextfield`, `dbutton` |
| Cupertino | `dcupertino`, `dcupertinoscreen`, `dcupertinoinput`, `dcupertinobutton` |
| State members | `dbuild`, `dinitstate`, `ddependencies`, `dupdatewidget`, `ddispose`, `dsetstate` |
| State-owned controllers | `dtextcontroller`, `dscrollcontroller` (generate init/dispose together; do not use for borrowed controllers) |

Material/Cupertino and additional widget constructor templates appear only for declared/referenced APIs. The semantic catalog includes application and referenced project widgets. A constructor's missing named arguments supply child/children/builder examples; existing arguments and lifecycle overrides are excluded. Builder parameter names avoid surrounding declarations. `CustomCarousel` supplies its actual three-parameter effectsBuilder and children contract in an app that references it.

Place the cursor on a resolved Widget expression, or select complete expressions, then use **Ctrl+.**, **Refactor** or **Doroti: Wrap with Widget…**. Center, Padding, Container, SizedBox, Align, Row, Column, Stack, Builder, LayoutBuilder and SingleChildScrollView are supported. Multiple consecutive children items can be wrapped together, including typed collection spreads. Expanded/Flexible/Positioned assists enforce their parent structure. Removal and supported wrapper/child-list replacements open a diff preview. Wrapping and imports are one Undo transaction; edits keep the buffer unsaved.

**Extract Doroti Widget…** creates a same-file StatelessWidget and passes local/parameter/read-only member values, nullable callbacks and generic constraints through its constructor. Inline closures, side-effecting method calls, mutations, inaccessible types and preprocessor/nested-class boundaries show a restriction instead of a speculative rewrite. **Convert to StatefulWidget** preserves constructors, key, attributes, fields and properties, using a nested State for private access. Each build captures its Widget instance so callbacks retain that instance. Primary constructor values are supported unless their capture is mutable. Partial/inheritance/base-dispatch/recursive-build cases remain restricted. Both changes have Apply/Cancel previews and may require **Restart (resets state)**; they do not promise mounted-state preservation through Hot Reload.

## Settings and session management

Settings `doroti.editing.completions`, `.snippets`, `.codeActions`, `.types` and `.hints` independently control these features. Editing remains available while an app runs. Documents are resolved against manifest application/Compile/reference items, not the selected runner; unrelated C# projects receive no Doroti suggestions. Unsaved source/global usings, project aliases, linked files and source/project/reference changes invalidate the appropriate caches.

Restricted Mode permits pure templates and widget-file creation with fully qualified types, without starting the helper or evaluating projects. When the SDK or references are unavailable, the same template fallback remains; semantic refactorings are unavailable and **Wrap with Widget…** explains the limitation. Check **Doroti: Show Logs**, restore references or install the configured .NET 10 SDK, then retry. Analysis is bounded to four evaluated project roots, 64 pending requests/cache entries, 32 simultaneous snapshots (4 MiB characters), and 8 MiB protocol frames. Stale/canceled results are never applied. Unsaved untitled documents and unresolved dynamic selections are outside semantic editing.

Closing a desktop app ends its development session when `dotnet watch` reports that the app exited and it is waiting for another file change. The extension stops the remaining watcher and enables **Run** again. Build failures and SDK-driven rebuild/restart sequences keep the session active. Web browser disconnects continue to wait for reconnection.

Stop and extension deactivation terminate the owned process tree. Session files under the app's `.doroti/dev/<session>` contain capability/revision/request IDs and diagnostics, never executable instructions. These files may be removed after stopping the session.

The extension supports local desktop workspaces. A preview server, LSP, debug adapter, Marketplace distribution, multi-root workspaces, remote hosts and browser-only editors are outside its scope. Connected Android/iOS device selection is provided through the repository CLI.

## Extension assets

The extension icon (`images/icon.png`) and Activity Bar icon (`images/sidebar.svg`) use the [official Doroti symbol](../../Doroti/branding/doroti-symbol-color.svg). Regenerate them with the app icons using [the branding instructions](../../Doroti/branding/README.md#파생-자산-재생성).

References: [dotnet watch](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch), [metadata update handlers](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.metadata.metadataupdatehandlerattribute?view=net-10.0), [Workspace Trust](https://code.visualstudio.com/api/extension-guides/workspace-trust).
