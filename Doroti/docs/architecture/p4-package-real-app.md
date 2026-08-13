# P4 package and real-app expansion

P4 moves the retained C5 package pilots from selection-only conversion to the user-owned `doroti.port/v1` workflow. `migration/package-pilots.json` is the support matrix and points to both the internal selection and public port manifest for every A/B/C candidate.

## Effective package ownership

| Pilot | Tier | Effective ownership consumed by the external app |
|---|---:|---|
| `collection 1.19.1` | A | generated `identity` plus `CollectionPilotExtensions` |
| `vector_math 2.2.0` | A | generated `mix` |
| `provider 6.1.5` | B | hash-pinned whole-symbol replacement for `ReassembleHandler` |
| `shared_preferences 2.5.3` | C | generated `ISharedPreferencesPlatformPort` plus user-owned in-memory implementation |

The tier-C `DOTCONV540` warning remains in the generated report. Its package URI and source span must match an entry in the origin-aware port source map; composing an implementation does not erase the reason the implementation is required.

## Generated real app and consumer boundary

`p4-real-app` is a C5 package with an application entry. Its generated C# references the public `Doroti.FlutterCompat` surface and its own generated namespace only. It does not reference Doroti engine/backend internals or a repository project. `R10.ExternalConsumer` consumes the five generated packages from a local feed and executes the real-app root, A algorithms, the extension, replacement, and platform port.

`eng/doroti.ps1 release` is the persistent external gate: restore, build/run, trimmed self-contained single-file ReadyToRun publish, published executable run, source-linked diagnostic audit, package-content audit, and provider rebase. The checked-in `migration/releases/p4-provider-upgrade-rebase.json` is the normalized upgrade review evidence; the gate regenerates the full compiler report from the current effective workspace.
