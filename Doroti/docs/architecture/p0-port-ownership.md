# P0 port ownership and manifest

`doroti.port/v1` is the user-owned workflow entrypoint. It fixes one of two lifecycle modes (`regeneratable-package` or `runtime-adoption`), source revision and license, compiler profile, selected library/symbol set, customization roots and required fixtures. Paths are relative; customization files and roots cannot escape the directory containing `doroti-port.json`.

The `doroti.converter-selection/v4` document remains an internal compiler input. It no longer carries `analyzerProject`; the compiler resolves and validates its owned frontend. During P0, `compilerSelection` bridges the port to that contract. The port input set and compiler profile must exactly match the resolved Migration IR, so the internal selection cannot silently broaden or narrow the user-owned request.

## Ownership

The complete ownership vocabulary is `generated`, `manual-replacement`, `partial-extension`, `platform-port` and `adopted-product`. A replacement manifest uses `doroti.replacements/v1` and identifies a library, symbol or member, its user-owned C# source and the SHA-256 of the generated file it was reviewed against. Duplicate or overlapping whole-symbol/member claims fail with `DORPORT005`; a missing target fails with `DORPORT007`; base drift fails with `DORPORT006`.

P0 records ownership but does not compile manual sources into the effective project. Replacement suppression and effective-project composition belong to P1.

## Workspace boundary

`compile --port` publishes only to `.doroti/workspaces/<workspace-id>/`. The workspace id covers the port manifest, internal compiler identity, selected Dart graph and every referenced manual input. Generated output lives below `generated-base/`; the editable port remains below `migration/ports/` and is never deleted or overwritten.

`port-workspace.json` is a deterministic `doroti.port-workspace/v1` inventory. It records every generated file hash and every selected symbol owner. Reopening an existing content-addressed workspace verifies that inventory before reuse. A changed, missing or added generated file fails with `DORPORT004` instead of being overwritten. A changed source/compiler/manual input gets a new workspace id and is generated in a fresh staging directory, so stale generated files cannot survive publication.
