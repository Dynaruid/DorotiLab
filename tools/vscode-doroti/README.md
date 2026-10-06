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
code --install-extension doroti-0.1.0.vsix
```

Set `doroti.cliPath` to the absolute repository `Doroti/eng/doroti.ps1` path. Install the Microsoft C# extension (`ms-dotnettools.csharp`, 2.x or later) for project loading, compiler diagnostics and normal IntelliSense. This extension adds a small Doroti import provider and snippets; it does not replace C# tooling. Package restore failures must be resolved before C# project diagnostics/imports can work. Doroti packages and SDKs referenced by generated templates must be available from the configured NuGet sources.

Use **Doroti: Create Project**, choose a name and parent folder, then **Select Project**, **Select Target**, **Run**. Existing folders are never overwritten. A canceled/failed creation may leave partial files at the logged destination. The manifest and repository CLI determine available runners. One local workspace folder and one running session are supported. Restricted Mode allows snippets/editing, but cannot execute creation, CLI or reload commands. No launch/tasks settings are overwritten.

The editor title and status bar contain **Hot Reload**. For a provider-advertised Debug target, edit a C# method body, click Hot Reload, and choose Save and Hot Reload. `dotnet watch` applies the actual metadata delta; a runtime acknowledgment follows widget/render reassembly on the owning UI queue. Saving files normally uses the same watcher. A no-op save does not claim a reload. Compilation errors appear in **Show Logs**; fix the code and retry. Unsupported edits require **Restart (resets state)**. The runtime ID and revision distinguish restarts from state-preserving reloads.

Web opens an external browser connected to a local session bridge. Wait for its runtime to become ready before using Hot Reload; the button is disabled when no browser is connected. State, text and scroll survive supported edits without refreshing the page. Use one page per session. The main-owned threaded runtime is supported; Release/AOT and the explicit worker-owned runtime are not qualified. The bridge carries request/result IDs only; the .NET SDK applies code. Stop also closes the bridge. See [the development contract](../../Doroti/docs/development-hot-reload.md) for SDK and browser scope.

The workspace declares the iOS TFM/RID and the tooling provider discovers matching simulators/devices with `doroti.device`. Typed iOS `dev` remains Unsupported, so the IDE does not offer that development target. Standalone iOS helper evidence and its signing/network prerequisites are documented in [development sessions](../../Doroti/docs/development-hot-reload.md).

`dstateless`, `dstateful`, `dbuild`, `dinitstate`, `ddispose`, `dsetstate`, `drow`, `dcolumn`, `dcontainer`, `dtext`, `dmaterial`, `dcupertino` insert C# snippets. Snippet descriptions list required namespaces. Choose a Doroti type completion or a compiler CS0246/CS0103 Quick Fix to add an import. Aliases/local declarations use qualified names; global usings are recognized within the selected app. The curated provider is not a full C# semantic resolver; resolve unrelated namespace conflicts with C# tooling or explicit qualification.

Stop and extension deactivation terminate the owned process tree. Session files under the app's `.doroti/dev/<session>` contain capability/revision/request IDs and diagnostics, never executable instructions. These files may be removed after stopping the session.

The implementation was written independently after static review of `reference/AvaloniaVSCode-ARCHIVE`; no archive source or dependencies were copied. No preview server, LSP, debug adapter, Marketplace publication, multi-root, remote/device discovery or browser-only editor support is included.

References: [dotnet watch](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch), [metadata update handlers](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.metadata.metadataupdatehandlerattribute?view=net-10.0), [Workspace Trust](https://code.visualstudio.com/api/extension-guides/workspace-trust), [extension tests](https://code.visualstudio.com/api/working-with-extensions/testing-extension).

On macOS, select the AppKit or Mac Catalyst alias advertised by `describe`. TFM/RID/profile selection belongs to `doroti-workspace.json`; the extension does not expose separate platform TFM settings. Current Apple GUI acceptance after the provider migration remains separate from earlier standalone helper evidence.

On Linux, select `linux` and use **Run**, then save C# method-body edits or click **Hot Reload**. PowerShell 7, .NET 10 and the system Qt development/QML dependencies are required. The CLI defaults to polling file changes to handle low inotify limits and mounted source folders; it preserves an explicit watcher environment setting. Supported changes retain PID, State, text and scroll. Native C++/QML/shader assets and unsupported metadata edits need **Restart (resets state)**. The default Qt Quick Debug path has real CLI and installed-extension regression fixtures; see [development sessions](../../Doroti/docs/development-hot-reload.md).
