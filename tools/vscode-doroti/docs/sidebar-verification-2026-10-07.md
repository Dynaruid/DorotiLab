# Doroti sidebar verification — 2026-10-07

Extension 0.2.1 adds a native Activity Bar container and Workspace tree. Project selection and provider targets, development session commands, logs, widget tools and settings share the existing command implementations. Opening/refreshing the view performs no CLI execution or build. The API follows the [VS Code Tree View guide](https://code.visualstudio.com/api/extension-guides/tree-view).

| Check | Result |
| --- | --- |
| TypeScript compile and existing Node tests | PASS, 12 tests |
| Installed VSIX, VS Code 1.140.0 on Windows | PASS, native view visible, declared target selection, manifest opening command, active editor eligibility |
| Installed VSIX, VS Code 1.100.0 on Windows | PASS, same sidebar suite |
| Development command plumbing | PASS, file-transport contract fixture: Run, reload acknowledgment with unchanged runtime ID, Restart with new runtime ID, Stop |
| State gating | PASS, idle/busy/pending/restart-required/compiler-error/stopping states; unavailable tree rows have no command |
| Polling | PASS, identical state updates do not emit tree refresh events |
| Restricted Mode, installed VSIX | PASS, project creation/CLI/development/wrap actions unavailable; widget creation templates and fully qualified snippet insertion available; helper not spawned |
| VSIX packaging | PASS, 0.2.1 with sidebar JavaScript and monochrome symbol asset |

The development fixture verifies extension command routing and status handling. Actual native application metadata reload, physical mouse/keyboard interaction, IME, accessibility, other operating systems and Marketplace publication were not rerun for this sidebar change. This scope does not replace the [0.2.0 editing verification](editing-verification-2026-10-07.md).

Durable results and the final VSIX SHA-256 are in [sidebar evidence](evidence/2026-10-07/sidebar/verification.json). Raw profiles/apps/logs under `temp/testing/vscode-sidebar/2026-10-07` and the local `.vsix` are disposable artifacts. Every test and package command below uses the repository's 20-minute timeout.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti test
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti run package
# Reuse the existing editing fixture preparation (requires its built fixture libraries).
node tools/vscode-doroti/scripts/prepare-editing.cjs temp/testing/vscode-sidebar/2026-10-07/app
node tools/vscode-doroti/scripts/prepare-editing.cjs temp/testing/vscode-sidebar/2026-10-07/restricted-app
python Doroti/eng/run-with-timeout.py --timeout 1200 node tools/vscode-doroti/dist/test/runHost.js temp/testing/vscode-sidebar/2026-10-07/app temp/testing/vscode-sidebar/2026-10-07/installed --sidebar
python Doroti/eng/run-with-timeout.py --timeout 1200 node tools/vscode-doroti/dist/test/runHost.js temp/testing/vscode-sidebar/2026-10-07/restricted-app temp/testing/vscode-sidebar/2026-10-07/restricted-installed --sidebar --restricted
python Doroti/eng/run-with-timeout.py --timeout 1200 node tools/vscode-doroti/dist/test/runHost.js temp/testing/vscode-sidebar/2026-10-07/app temp/testing/vscode-sidebar/2026-10-07/minimum-installed --sidebar --version=1.100.0
```
