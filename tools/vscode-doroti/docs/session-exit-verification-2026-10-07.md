# Desktop session exit regression — 2026-10-07

Closing the Carousel Windows App SDK window left **Stop** enabled and **Run** disabled. The runtime had published `status: closed`, but `dotnet watch` remained alive and waited for another source edit. That wait is [normal SDK behavior](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch#description); the extension previously ended a session only when its launcher process exited.

Doroti 0.2.2 recognizes the watcher's application-exit line followed by its complete waiting-for-changes line for file-transport development targets. It then uses the existing Stop path to terminate the owned watcher tree and restore idle controls. An exit line alone does not end the session: rebuild/relaunch output invalidates that exit, and a build failure without an app exit keeps the watcher available. Closing a browser still waits for reconnection. Polling and Hot Reload cannot re-enable runtime controls while the session is stopping.

The output recognizer handles project/TFM labels, ANSI colors, suppressed emojis, nonzero application exits, and stdout/stderr chunks retained across callbacks. It uses the same bounded 8 KiB output tail as the existing compiler/restart diagnostics and the SDK's English watcher messages.

| Check | Result |
| --- | --- |
| TypeScript compile and Node contracts | PASS, 14 tests |
| Installed Doroti 0.2.2 / VS Code 1.140.0, actual Carousel Windows App SDK window close | PASS, normal close acknowledgment |
| Owned launcher/watcher/application tree cleanup | PASS, all six captured PIDs terminated |
| Sidebar after close | PASS, Run enabled; Stop, Hot Reload and Restart disabled |
| Run again, new session/runtime and explicit Stop | PASS |

[Durable native results](evidence/2026-10-07/session-exit/result.json) retain both session/runtime identities and the captured process tree. The output included:

```text
dotnet watch ⌚ [DorotiCarouselApp.WindowsAppSdk (net10.0-windows10.0.19041.0)] Exited
dotnet watch ⏳ Waiting for a file to change before restarting ...
Application exited; stopping the development watcher. Use Run to launch again.
```

The native test entrypoint is [sessionExitHost.ts](../src/test/sessionExitHost.ts). It captures only its own launcher/watcher/app process tree, waits for the native main window, sends a normal window-close request, checks the closed runtime acknowledgment and process termination, asserts sidebar commands, starts a new session through **Run**, and verifies explicit **Stop**. Physical mouse input is outside this automated check.

Reproduce on Windows with no other development session holding the CLI/provider build outputs:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti test
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti run package
python Doroti/eng/run-with-timeout.py --timeout 1200 node tools/vscode-doroti/dist/test/runHost.js samples/DorotiCarouselApp temp/testing/vscode-session-exit/current --session-exit
```

The test runner also accepts `DOROTI_TEST_CLI` to select an isolated launcher. The first attempts here encountered an existing user session holding the managed CLI/provider DLLs. That session was preserved. Native validation uses a disposable launcher: `describe` runs the separately built real managed CLI, and `dev` invokes the real `dotnet watch` Windows runner using the manifest TFM and the production session/watcher environment. This bypasses only preparatory tooling builds, not native application startup, runtime publication, window close or extension process ownership. It does not establish a fresh end-to-end run through `Doroti/eng/doroti.ps1`.

Raw profiles, launcher and logs under `temp/testing/vscode-session-exit-2026-10-07` are disposable. Broader editing, Hot Reload deltas, other desktop/device targets and physical close-button input are not requalified by this change.
