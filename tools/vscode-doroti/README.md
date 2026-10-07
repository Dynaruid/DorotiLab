# Doroti for VS Code

Local extension for VS Code 1.100+ desktop, PowerShell 7 and the repository's .NET 10 SDK. Build tools use Node 24 and npm with the checked-in lockfile. Available Debug development targets come from workspace v2 and the resolved provider profile on the current host. iOS typed development is currently Unsupported; historical standalone Apple helper results do not enable an IDE target.

For Android, select `android`, connect one authorized USB device/emulator and use **Run**. Python 3, Android platform-tools, .NET SDK 10.0.400+ and the matching Android workload are required. `doroti.device` selects a serial when several devices are attached. The workspace RID selects arm64/x64 and must match the device ABI and enables the SDK's startup hook in Debug Mono. Save or **Hot Reload** applies supported C# edits over USB while preserving widget state, text and scroll. The button waits for device request delivery before saving and for a completed reassembly frame before reporting success. **Stop** closes the owned app, watcher and USB reverse port. Native Kotlin/Java, resources, project changes and unsupported C# edits require **Restart (resets state)**. Release/AOT and CoreCLR are outside this development profile. See [development sessions](../../Doroti/docs/development-hot-reload.md).

The extension icon (`images/icon.png`) is a transparent 256×256 PNG rendered from the [official Doroti color symbol](../../Doroti/docs/branding/doroti-symbol-color.svg), preserving its filled blocks and thin transparent cutout spacing. VS Code extension packages require a raster icon; the SVG remains the source of truth. Regenerate it together with the app icons using [the branding instructions](../../Doroti/docs/branding/README.md#파생-자산-재생성).

```powershell
dotnet new install <repository>/Doroti/templates/Doroti.Templates/content/doroti-app
cd <repository>/tools/vscode-doroti
npm ci
npm test
npm run package
code --install-extension doroti-0.2.1.vsix
```

Set `doroti.cliPath` to the absolute repository `Doroti/eng/doroti.ps1` path for development commands. Editing uses the bundled .NET 10/Roslyn 5.9 helper and does not require a repository checkout, CLI path, project selection or an app session. Install the Microsoft C# extension (`ms-dotnettools.csharp`) for normal IntelliSense, signature help, diagnostics, navigation and general C# refactoring. Doroti supplies widget construction templates and structural assists alongside it; fallback type suggestions are suppressed while C# is active. Restore the application's declared dependencies before semantic editing. Suggestions never restore, build, install dependencies or save source files.

Use **Doroti: Create Project**, choose a name and parent folder, then **Select Project**, **Select Target**, **Run**. Existing folders are never overwritten. A canceled/failed creation may leave partial files at the logged destination. The manifest and repository CLI determine available runners. One local workspace folder and one running session are supported. Restricted Mode allows snippets/editing, but cannot execute creation, CLI or reload commands. No launch/tasks settings are overwritten.

Click the **Doroti** symbol in the left Activity Bar, or run **Doroti: Show Sidebar**, to open the native **Workspace** view. **Project** selects the application and provider development target, opens its manifest and creates projects. Expand **Select Target** to choose a declared target directly. **Development** shows connection/reload status and runtime revision, with Run, Hot Reload, Restart, Stop and Show Logs. **Widgets** contains the existing new-widget, snippet and wrap commands for the active Doroti application C# editor. Unavailable actions show their reason and have no click command. The view title has session controls, Refresh Sidebar and an overflow menu with Doroti settings. Opening or refreshing the sidebar reads local context without invoking the CLI or building the app. You can move the view to the secondary sidebar or panel using VS Code's view menus. The monochrome Activity Bar asset (`images/sidebar.svg`) is derived from the [official symbol](../../Doroti/docs/branding/doroti-symbol.svg).

The editor title and status bar contain **Hot Reload**. For a provider-advertised Debug target, edit a C# method body, click Hot Reload, and choose Save and Hot Reload. `dotnet watch` applies the actual metadata delta; a runtime acknowledgment follows widget/render reassembly on the owning UI queue. Saving files normally uses the same watcher. A no-op save does not claim a reload. Compilation errors appear in **Show Logs**; fix the code and retry. Unsupported edits require **Restart (resets state)**. The runtime ID and revision distinguish restarts from state-preserving reloads.

The repository CLI includes the [Carousel Windows watch fix](docs/watch-verification-2026-10-07.md) for duplicate project keys at `Loading projects`. It uses the runner's validated RID for development sessions. Existing installed extensions pick up this CLI change on their next Run.

Web opens an external browser connected to a local session bridge. Wait for its runtime to become ready before using Hot Reload; the button is disabled when no browser is connected. State, text and scroll survive supported edits without refreshing the page. Use one page per session. The main-owned threaded runtime is supported; Release/AOT and the explicit worker-owned runtime are not qualified. The bridge carries request/result IDs only; the .NET SDK applies code. Stop also closes the bridge. See [the development contract](../../Doroti/docs/development-hot-reload.md) for SDK and browser scope.

The workspace declares the iOS TFM/RID and the tooling provider discovers matching simulators/devices with `doroti.device`. Typed iOS `dev` remains Unsupported, so the IDE does not offer that development target. Standalone iOS helper evidence and its signing/network prerequisites are documented in [development sessions](../../Doroti/docs/development-hot-reload.md).

Type `stl`, `stless` or `dstateless` for a StatelessWidget, and `stf`, `stful` or `dstateful` for a StatefulWidget. Choose the suggestion, edit the selected class name, then press Tab to reach build. Stateful widget, State and createState names stay linked. `dstlkey`/`dstfkey` include a key constructor. Imports are inserted with the snippet; aliases and same-name types use qualified references. File names supply valid default names, including Korean identifiers.

**Doroti: New Stateless Widget** and **New Stateful Widget** ask for the class, namespace, file and constructor style. Existing files are never overwritten; cancellation creates no file. **Doroti: Insert Widget Snippet** lists the applicable templates and keeps the existing `d*` entrypoints. Templates now come from one import-aware provider rather than VS Code's static **Insert Snippet** list. User `editor.snippetSuggestions` and `tabCompletion` settings are respected.

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

Settings `doroti.editing.completions`, `.snippets`, `.codeActions`, `.types` and `.hints` independently control these features. Editing remains available while an app runs. Documents are resolved against manifest application/Compile/reference items, not the selected runner; unrelated C# projects receive no Doroti suggestions. Unsaved source/global usings, project aliases, linked files and source/project/reference changes invalidate the appropriate caches.

Restricted Mode permits pure templates and widget-file creation with fully qualified types, without starting the helper or evaluating projects. When the SDK or references are unavailable, the same template fallback remains; semantic refactorings are unavailable and **Wrap with Widget…** explains the limitation. Check **Doroti: Show Logs**, restore references or install the configured .NET 10 SDK, then retry. Analysis is bounded to four evaluated project roots, 64 pending requests/cache entries, 32 simultaneous snapshots (4 MiB characters), and 8 MiB protocol frames. Stale/canceled results are never applied. Unsaved untitled documents and unresolved dynamic selections are outside semantic editing.

Editing validation is separate from Run/Hot Reload. See [the dated editing evidence](docs/editing-verification-2026-10-07.md) for Windows VS Code 1.100.0/1.140.0, C# coexistence, Restricted Mode, generated-code compilation, mounted widget trees and measured response times. Reproduce with a 20-minute cap per command:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti test
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti run package
node tools/vscode-doroti/scripts/template-fixtures.cjs temp/testing/vscode-editing/current/templates.cs
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project tools/Doroti.Editor.Assist/tests/Doroti.Editor.Assist.Tests.csproj -c Release -- temp/testing/vscode-editing/current/templates.cs
node tools/vscode-doroti/scripts/prepare-editing.cjs temp/testing/vscode-editing/current/app
python Doroti/eng/run-with-timeout.py --timeout 1200 node tools/vscode-doroti/dist/test/runHost.js temp/testing/vscode-editing/current/app temp/testing/vscode-editing/current/installed --editing
```

The fixture preparation script uses the product libraries built by the helper tests. Add `--csharp`, `--restricted` or `--version=1.100.0` to use separate profiles for that matrix. Use a fresh fixture directory for each run. `tests/protocol.py <evidence-directory>` checks persistent helper messages, snapshots, cancellation and failed evaluation. Packaging includes helper DLLs and both MSBuild build hosts; runtime operation never searches repository source for the helper.

Stop and extension deactivation terminate the owned process tree. Session files under the app's `.doroti/dev/<session>` contain capability/revision/request IDs and diagnostics, never executable instructions. These files may be removed after stopping the session.

The implementation was written independently after static review of `reference/AvaloniaVSCode-ARCHIVE`; no archive source or dependencies were copied. No preview server, LSP, debug adapter, Marketplace publication, multi-root, remote/device discovery or browser-only editor support is included.

References: [dotnet watch](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch), [metadata update handlers](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.metadata.metadataupdatehandlerattribute?view=net-10.0), [Workspace Trust](https://code.visualstudio.com/api/extension-guides/workspace-trust), [extension tests](https://code.visualstudio.com/api/working-with-extensions/testing-extension).

On macOS, select the AppKit or Mac Catalyst alias advertised by `describe`. TFM/RID/profile selection belongs to `doroti-workspace.json`; the extension does not expose separate platform TFM settings. Current Apple GUI acceptance after the provider migration remains separate from earlier standalone helper evidence.

On Linux, select `linux` and use **Run**, then save C# method-body edits or click **Hot Reload**. PowerShell 7, .NET 10 and the system Qt development/QML dependencies are required. The CLI defaults to polling file changes to handle low inotify limits and mounted source folders; it preserves an explicit watcher environment setting. Supported changes retain PID, State, text and scroll. Native C++/QML/shader assets and unsupported metadata edits need **Restart (resets state)**. The default Qt Quick Debug path has real CLI and installed-extension regression fixtures; see [development sessions](../../Doroti/docs/development-hot-reload.md).
