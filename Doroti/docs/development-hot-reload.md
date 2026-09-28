# Development sessions and metadata Hot Reload

`doroti.ps1 describe -App <folder>` returns `doroti.cli-workspace/v1` JSON. The manifest may declare any nonempty subset of the known platform aliases. `developmentTargets` is the declared Windows/Web intersection; consumers must not invent missing runners.

```powershell
pwsh -NoProfile -File Doroti/eng/doroti.ps1 dev -App samples/DorotiTestbedApp -Platform windows
pwsh -NoProfile -File Doroti/eng/doroti.ps1 dev -App samples/DorotiTestbedApp -Platform web
```

`dev` defaults to Debug, rejects Release and unsupported targets, and uses `dotnet watch` for Windows App SDK and Web. Web retains the runner launch profile/URL and the SDK browser-refresh agent. CLI failures remain nonzero. The parent process owns the watcher/host lifetime. Native C++ builds clear the managed SDK's exported MSBuild paths before invoking Visual Studio MSBuild, including when launched from `dotnet watch`.

The Widgets assembly registers a `MetadataUpdateHandler`. A real .NET metadata update queues reassembly on the captured view dispatcher, serializes concurrent updates, and waits for the resulting frame. Reassembly traverses the element tree and invalidates rendering. Framework errors in that frame produce `failed`, never `applied`. Closing the binding cancels its registration before unmount. This mechanism works without VS Code.

For Windows, `DOROTI_DEV_SESSION` and `DOROTI_DEV_SESSION_ID` opt into local session files. `runtime.json` is atomically replaced with schema `doroti.dev/v1`, `sessionId`, `runtimeId`, PID, host identity, `supported`, revision, status, request ID and optional error. Status is `ready`, `applying`, `applied`, `failed` or `closed`; revision increases only after a successful reassembly frame. `supported` requires .NET metadata support and Debug. A changed runtime ID means a restart, with state reset.

An editor writes `request.json` with the current session/runtime IDs and a unique request ID **before saving C# edits**. The file correlates the metadata callback; it is not a command that pretends to apply code. Malformed/stale request files are ignored, so automatic save-based reload can still proceed. The editor handles compiler errors, watcher rude-edit messages, timeout, and explicit restart separately. No request acknowledgment or no-op save counts as success.

For unsupported edits, the piped watcher waits at its restart prompt. It does not consume redirected stdin for console keys ([SDK console implementation](https://github.com/dotnet/sdk/blob/main/src/Dotnet.Watch/Watch/UI/PhysicalConsole.cs)). The extension therefore displays Restart required and leaves the old process/state alive until the user chooses **Restart (resets state)**, which replaces the owned process tree. It does not use `--non-interactive`, which would automatically restart and discard state. After a rude edit, use Restart even if subsequently reverting it; compilation errors alone can be corrected and retried without restarting.

The [VS Code extension](../tools/vscode-doroti/README.md) supplies the commands and status. `doroti.selectTarget` optionally accepts a declared alias; `doroti.hotReload` accepts `true` to explicitly save pending app C# documents for command automation. Interactive commands display the target picker and save/cancel picker. Restart warns that widget/input/scroll state resets. Public remote/debug/preview protocols are outside this contract.

Web uses the main-owned threaded .NET runtime and its existing render worker. .NET SDK 10.0.400's injected browser agent supplies the metadata deltas; no assembly swap, page refresh or fake reassemble substitutes for a delta. The SDK agent uses synchronous JS exports, so development startup selects the .NET 10 runtime's `ThrowWhenBlockingWait` policy only when the SDK browser-refresh script is present. Blocking waits still throw. Normal published startup retains its default policy. This integration is qualified against the repository's pinned SDK/runtime; the runtime option is version-specific.

Browser reassembly preserves the runtime's `JSSynchronizationContext`. EditableText composition callbacks are rebound when Mono changes delegate identity across metadata updates; previous layer subscriptions are canceled. Browser status/request correlation lives in shared managed memory and is exposed by async host exports. CLI-only pages expose status in the root `data-doroti-hot-reload` attribute; saving C# uses the watcher directly.

For the Web button, the extension owns a short-lived HTTP bridge bound to `127.0.0.1` with an unguessable URL token and exact runner-origin validation. It carries status and request IDs only, never code/deltas. The browser URL contains the session connection parameters. The extension waits until the current browser runtime accepts the request before saving; it then waits for that request's successful frame acknowledgment. A closed/stale page disables capability, and Stop closes the bridge and process tree. Use the page opened by Run and one browser page per session. A manual refresh creates a new runtime and loses state, as expected.

Current Web scope: Debug/JIT method-body updates in the default main-owned runtime; tested desktop Chromium. Release/AOT, the explicit worker-owned runtime option, physical mobile browsers and browser families not exercised here are not qualified. Supported edits follow the SDK's actual capability/rude-edit result, not a blanket promise for every C# change. Framework build errors can be corrected and retried; unsupported type/member edits require Restart. See [.NET WebAssembly Hot Reload](https://learn.microsoft.com/en-us/aspnet/core/test/hot-reload?view=aspnetcore-10.0).

Validation commands (each uses a 20-minute cap):

```powershell
dotnet new install Doroti/templates/Doroti.Templates/content/doroti-app
python Doroti/eng/run-with-timeout.py python Doroti/tests/development_fixture.py "temp/testing/dev-session/한글 경로/ReloadApp"
python Doroti/eng/run-with-timeout.py python Doroti/tests/hot_reload_smoke.py temp/testing/dev-session/native "temp/testing/dev-session/한글 경로/ReloadApp"
python Doroti/eng/run-with-timeout.py npm.cmd ci --ignore-scripts --prefix Doroti/tools/vscode-doroti
python Doroti/eng/run-with-timeout.py npm.cmd test --prefix Doroti/tools/vscode-doroti
python Doroti/eng/run-with-timeout.py npm.cmd run package --prefix Doroti/tools/vscode-doroti
python Doroti/eng/run-with-timeout.py node Doroti/tools/vscode-doroti/dist/test/runHost.js "temp/testing/dev-session/한글 경로/ReloadApp" temp/testing/dev-session/editor
python Doroti/eng/run-with-timeout.py node Doroti/tools/vscode-doroti/dist/test/runHost.js "temp/testing/dev-session/한글 경로/ReloadApp" temp/testing/dev-session/web-editor --web
```

The fixture invokes the installed template, then explicitly replaces Doroti package references with repository projects and corresponding SDK/build imports. This validates current source without claiming package-only distribution qualification. Editor tests install the VSIX in a separate profile and invoke real VS Code editor/command APIs. Native state is seeded automatically; this is not physical IME or mouse evidence. Close test hosts before removing evidence. Raw files belong under `temp/testing`; preserve conclusions in the work documents.

The `--web` integration test opens the actual external browser and pauses at file gates in its evidence directory. After `ready.json`, interact with Count, Scroll +160 and the text field, then create `continue`. After `applied.json`, inspect the changed label and retained values, then create `inspected`. After `recovered.json`, inspect again and create `finish`. These gates coordinate browser UI automation/manual inspection with actual extension commands; no direct widget callbacks are injected. Each test still has the outer 20-minute cap and bounded per-stage waits. Use a new evidence directory for every run. Do not run this interactive browser qualification as an unattended CI unit test.
