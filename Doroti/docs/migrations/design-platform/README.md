# Implementation checkpoint

The authoritative task status is [execution.json](execution.json), compared with the archived [work2 structure and task summary](../../../../history/26-10-05/work2-summary.md) and [work3 follow-up](../../../../history/26-10-05/work3-summary.md). This is an in-progress implementation, not a declaration that all milestones passed.

Current resumption: [implementation and coverage](resume-2026-10-05.md). Current file/namespace/declaration mapping: [ownership inventory](current-ownership-2026-10-05.json), a textual audit that does not replace evaluated graph/API validation.

Verified artifacts (each receipt defines its own time and scope):

- [Design packages](design-package-verification.json): evaluated graph, NuGet-only core/Cupertino/Material/both consumers, independent version candidates and custom template hive.
- [Shared headless application](bootstrap-verification.json): staged startup, service ownership, shared State/InheritedWidget and two CPU views; synthetic EditableText focus/selection/IME isolation.
- [External provider](platform-provider-verification.json): independent runtime/tool NuGet packages and actual managed CLI describe/SDK doctor/config/device/template/build/run.
- [Native Windows shared tree](windows-shared-tree-verification.json): actual Regular HWND pair, shared application owner/tree, accepted and presented frames, primary detach/survivor and native/GPU cleanup completion.

Tooling projects are `Doroti/src/Doroti.Tooling.Contracts`, `Doroti/src/Doroti.Tooling.Extension.Sdk` and the managed CLI in `Doroti/tools/Doroti.Tooling`. The CLI never loads a runtime Host. Its process executor owns app/build processes independently of tool connections.

Builtin provider sources are in `packages/platforms/windowsappsdk`, `web`, `qt` and `maui`; old paths in historical validation prose describe the former source baseline. Core Windowing interfaces are in `Doroti.Ui/Windowing.cs`, the shared registry is the existing Desktop manager, and Widgets window branches attach explicit views under a shared tree.

Tests use the external 1200-second timeout. Actual native probes also bound their application lifetimes. Failure artifacts remain under `temp/testing/platform-decoupling` for investigation. Successful JSON summaries identify the exact scope and exclusions. No push, PR, deployment, signing, or public NuGet publication was performed.

Latest native evidence also includes [five actual HWND kinds](windows-kinds-verification.json) and [independent prebuilt Windows NuGet consumer](windows-provider-package-verification.json). These are scoped acceptances; physical input/mixed DPI/modal reentrancy/design result coverage is not implied. The strict G1 latest executable rerun is recorded in windows-shared-tree-verification.json.

Latest Windows design results are in [windows-design-verification.json](windows-design-verification.json). Independent Windows/Web candidate versions, browser first-frame/focus and Windows MAUI scope are collected in the resumption record. Apple/mobile, physical input, full native context menu/restoration, Qt/AppKit menu/kind expansion and mobile development transport remain unfinished.
