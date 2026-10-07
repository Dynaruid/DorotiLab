# Carousel Windows watch regression — 2026-10-07

The installed Doroti 0.2.1 extension failed when running `samples/DorotiCarouselApp` / `windows` with .NET SDK 10.0.400. The provider's generic development plan appended `-r win-x64` to `dotnet watch`. The runner removes that global RID on its platform-neutral application reference, but other graph paths retained it. MSBuild produced 31 nodes with six duplicate path/TFM identities, including `Doroti.Framework.Scheduler / net10.0`. The SDK watcher then threw `ArgumentException: An item with the same key has already been added` at `Loading projects`.

[DotnetToolExtension](../../../Doroti/src/Doroti.Tooling.Extension.Sdk/DotnetToolExtension.cs) now uses the runner's declared RID for generic `dev` plans. The CLI validates that RID against the manifest before planning. Removing the command-line RID yields 25 graph nodes with no duplicate path/TFM identities. Build, run and publish keep their explicit RID; device-specific Android development planning is unchanged. This is a repository CLI fix, so the installed extension can use it on its next Run without reinstalling the VSIX.

The first native run also exposed a separate Debug paint error in the Carousel Record Box preview: a top-only border shared a `BoxDecoration` with a radius. The [sample](../../../samples/DorotiCarouselApp/src/App.cs) now leaves the radius to the existing enclosing `ClipRRect`. The 2px translucent top edge and rounded clipping remain; Debug painting and reload frame acknowledgments succeed.

| Check | Result |
| --- | --- |
| Original watcher command with `-r win-x64` | Reproduced the same duplicate-key exception after a successful build |
| MSBuild graph, final development properties | PASS, 25 nodes, zero duplicate path/TFM identities |
| Typed tooling contract suite | PASS, including new development RID regression and explicit build/run/publish RID checks; the new regression failed before the fix |
| Extension compile and Node contracts | PASS, 12 tests |
| Installed VSIX on Windows / VS Code 1.140.0 | PASS, actual Carousel Windows App SDK Run |
| Actual method-body edit via Doroti: Hot Reload | PASS, `ready` revision 0 to `applied` revision 1, same runtime ID and PID 41776 |
| Restore sample source and reload | PASS, revision 2 with successful frame acknowledgment |
| Doroti: Stop | PASS, session ended and application PID terminated |

The installed test uses the repository CLI and actual native runner, rather than a session fixture. It restores its temporary MaterialApp title edit. [Durable runtime results](evidence/2026-10-07/watch/result.json) retain both startup and reload identities. Raw graph probes, profiles and logs under `temp/testing/watch-duplicate-2026-10-07` are disposable. This check does not establish physical input, IME, pixel comparison, other platforms or clean-machine release acceptance.

All test commands use the repository's 20-minute timeout:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/tool_extension_contract.py
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti test
python Doroti/eng/run-with-timeout.py --timeout 1200 node tools/vscode-doroti/dist/test/runHost.js samples/DorotiCarouselApp temp/testing/watch-duplicate-2026-10-07/fixed-installed --carousel
```

The installed test requires the existing `tools/vscode-doroti/doroti-0.2.1.vsix`; regenerate it with the normal package command if absent.
