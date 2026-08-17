# Doroti artifact schemas

All machine-readable R1 outputs are UTF-8 JSON with a top-level `schemaVersion`.
Arrays whose order is not semantically meaningful are sorted using ordinal path or identifier order.
Reports do not contain wall-clock timestamps or absolute checkout paths, so identical inputs produce identical bytes.

Source inventory deliberately does not enumerate or hash an entire reference checkout. It records the official upstream reference and immutable commit when one is pinned, then hashes only files named in the selected migration/dependency closure. This keeps routine audit fast while preserving exact evidence for code that can enter Doroti.

| Artifact | Schema |
|---|---|
| Source inventory and provenance findings | `doroti.source-audit/v1` |
| R2 migration candidate, dependency, test, FlutterCompat semantic inventory, exclusion, and legacy baseline findings | `doroti.migration-asset-audit/v1` |
| Avalonia vendor selection, hash, compile closure, and forbidden-dependency findings | `doroti.vendor-audit/v1` |
| Avalonia shell selected-file/symbol dependency closure | `doroti.avalonia-shell-closure/v1` |
| Flutter/Avalonia external/native source boundary | `doroti.flutter-avalonia-source-boundary/v1` |
| Flutter capability to Avalonia source-port mapping | `doroti.flutter-avalonia-capability-map/v1` |
| Current handwritten owner symbol cutover audit | `doroti.current-owner-audit/v1` |
| G4-0 reproducibility and boundary validation report | `doroti.g4-boundary-audit-report/v1` |
| Avalonia shell source-port audit | `doroti.avalonia-port-audit/v1` |
| Avalonia shell staged copy/adaptation provenance | `doroti.avalonia-port-stage/v1` |
| Avalonia shell selected-file rebase classification | `doroti.avalonia-port-rebase/v1` |
| Non-mutating Avalonia upstream review bundle | `doroti.vendor-review/v1` |
| Avalonia release, resolved package closure, license, and per-host dependency policy | `doroti.host-dependency-matrix/v1` |
| Generated/production review bundle | `doroti.promotion-report/v1` |
| Historical fixture Dart migration intermediate representation | `doroti.migration-ir/v2` |
| Typed framework semantic migration intermediate representation | `doroti.migration-ir/v3` |
| Mechanical Flutter candidate declaration/member/AST coverage | `doroti.framework-coverage/v1` |
| Selected Flutter framework library dependency closure | `doroti.flutter-framework-closure/v1` |
| F0 mechanical candidate and reviewed-promotion evidence | `doroti.flutter-f0-candidate-evidence/v2` |
| F1-F4 resolved inventory closures | `doroti.flutter-f1-closure/v1` through `doroti.flutter-f4-closure/v1` |
| Goal3 implementation truth evidence | `doroti.flutter-framework-evidence/v2` |
| Goal3 symbol-to-target cardinality audit | `doroti.flutter-framework-evidence-audit/v2` |
| Goal3 evidence reset report | `doroti.flutter-framework-evidence-reset/v1` |
| Goal3 framework promotion manifest | `doroti.framework-promotion/v2` |
| Goal3 framework symbol patch | `doroti.framework-symbol-patch/v1` |
| Goal3 promotion review/diff/rebase/result | `doroti.framework-promotion-review/v1`, `doroti.framework-promotion-diff/v1`, `doroti.framework-promotion-rebase/v1`, `doroti.framework-promotion-result/v1` |
| Goal3 G3-2 completion evidence | `doroti.g3-2-evidence/v1` |
| Goal3 registered compiler-profile gate | `doroti.framework-compiler-gate/v1` |
| Flutter 13-root/695-file compiler census graph | `doroti.flutter-source-census-graph/v1` |
| Multi-library SCC/project/reference/namespace graph | `doroti.framework-project-graph/v1` |
| G3-1 compiler milestone evidence | `doroti.g3-1-evidence/v1` |
| F4 Material/Cupertino resolved public export closure | `doroti.flutter-f4-closure/v1` |
| Dart semantic lowering and runtime/platform ownership table | `doroti.dart-semantics/v1` |
| Dart-to-C# diagnostics/report | `doroti.converter-report/v2` |
| Resolved Dart package dependency graph | `doroti.package-graph/v1` |
| Dart span to generated C# line mapping | `doroti.source-map/v1` |
| Retired SceneLab manifest (historical evidence only) | `doroti.scene-result/v1` |
| Doctor report | `doroti.doctor/v1` |
| Flutter revision, input hash and symbol selection baseline | `doroti.flutter-baseline/v1` |
| Deterministic member-level Flutter public API selection | `doroti.flutter-api/v2` |
| Executable behavior fixture | `doroti.behavior-fixture/v1` |
| Reference or Doroti behavior result | `doroti.behavior-result/v1` |
| Cross-runner behavior delta | `doroti.behavior-delta/v1` |
| Widget lifecycle behavior fixture | `doroti.widget-lifecycle-fixture/v1` |
| Flutter reference or Doroti widget lifecycle result | `doroti.widget-lifecycle-result/v1` |
| Package pilot inventory, port workflow, external consumer, and per-dimension results | `doroti.package-pilots/v3` |
| Historical fixture levels and Goal3 framework compiler gate | `doroti.compiler-support/v3` |
| C5 generated NuGet identity, provenance, diagnostics, and artifact hashes | `doroti.package-release/v1` |
| User-owned compiler port workflow | `doroti.port/v1` |
| Manual replacement ownership and generated-base pin | `doroti.replacements/v1` |
| Compiler-owned port workspace and generated-file ownership | `doroti.port-workspace/v1` |
| Compiler/source/manual/effective port state | `doroti.port-state/v1` |
| Effective artifact ownership and upstream provenance | `doroti.port-provenance/v1` |
| Origin-aware effective source map | `doroti.port-source-map/v1` |
| Non-mutating runtime adoption review | `doroti.adoption/v1` |
| Symbol/member upstream and manual drift report | `doroti.rebase-report/v1` |
| FlutterCompat release/baseline difference | `doroti.flutter-compat-release-diff/v1` |
| R10 beta packaging and external validation status | `doroti.r10-release-evidence/v1` |
| Target runtime/performance/verification report | `doroti.runtime-report/v1` |
| Interactive raw/route/gesture/frame replay trace | `doroti.interactive-trace/v1` |
| Interactive Counter behavior fixture | `doroti.interactive-counter-fixture/v1` |
| R8 Counter target runtime report | `doroti.r8-runtime-report/v2` |
| Avalonia host lifecycle trace | `doroti.avalonia-host-trace/v1` |
| H1 Avalonia runtime report | `doroti.h1-avalonia-runtime-report/v1` |
| H1 Avalonia reviewed evidence | `doroti.h1-avalonia-evidence/v1` |
| Tracked R8 target-machine evidence summary | `doroti.r8-target-baseline/v2` |
| G5-1 native input/window evidence | `doroti.g5-1-native-input/v1` |
| G5-1 policy owner audit | `doroti.g5-1-policy-owner-audit/v1` |
| G5-1 closure summary | `doroti.g5-1-closure/v1` |
| G5-5 application compiler, resources, plugins, and external consumer evidence | `doroti.g5-5-evidence/v1` |
| G7-3M macOS source-port provenance | `doroti.g7-macos-source-port-provenance/v1` |
| G7-3M actual AppKit/GPU/input/resource probe | `doroti.g7-macos-live-probe/v1` |
| G7-3M Source/Build/Live/Package aggregate | `doroti.g7-macos-shell-evidence/v1` |
| G7-3 browser toolchain/graph/compile/package aggregate | `doroti.g7-web-build-evidence/v2` |
| G7-3 deployment-neutral browser static artifact hashes | `doroti.static-artifact-manifest/v1` |
| Current TypeScript Web graph, fail-closed compile, package/publish, and browser boundary aggregate | `doroti.web-product-evidence/v2` |
| Current Chromium TypeScript bootstrap stages, loaded modules, canvas, pointer, and explicit live boundaries | `doroti.web-browser-live-manual/v2` |
| TypeScript loader stage, singleton start, stable diagnostic, and original-error contract fixture | `doroti.web-loader-contract/v1` |
| Current deployment-neutral Web artifact hashes with TypeScript-source-to-JavaScript identity | `doroti.static-artifact-manifest/v4` |
| Single-project target graph/build evidence with Web-only TypeScript isolation | `doroti.app-targets-evidence/v3` |
| RID target capability manifest | `doroti.target-manifest/v1` |
| RID target package manifest | `doroti.target-package/v1` |
| Roadmap 2 to Roadmap 3 handoff contract | `doroti.roadmap2-handoff/v1` |
| R9 product-foundation implementation and release-gate evidence | `doroti.r9-foundation-evidence/v1` |

Generated drafts live under `migration/generated/` and are excluded from default compile items. Selection manifests pin the converter version, Migration IR contract version, `review-draft` generation mode, and `flutter-aware-bootstrap` compatibility profile. The IR records selected declarations and the versioned type rules available to that profile. In particular, the R1 `Future<T> -> Task<T>` rule is marked `declaration-only`; it does not claim Flutter completion, error, callback, or cancellation behavior. `promote` only creates a review bundle; it never edits production source.

`migration/vendor/avalonia-platform/selection.json` separates Doroti-owned vendor seams from Avalonia-derived selections. Every owned seam carries an ownership header. Every `copy`, `adapt`, or `rewrite` entry must pin the Avalonia source hash, adapted target hash, and direct `using` dependency set. `audit` resolves each declared vendor project's compiled C# set, rejects files outside those two inventories, and blocks forbidden Avalonia UI/runtime dependencies unless a path-specific allowlist entry records the exception and reason.

`migration/flutter-compat/baseline.json` pins an exact 40-character Flutter Git revision, Dart SDK range, selected input hashes, symbol support state and behavior fixture. `Doroti.SourceTools generate-flutter-api` refuses input drift and emits sorted inputs/symbols without timestamps or absolute paths. Manifest v2 records inheritance, generic bounds, constructors, members, named/optional/required parameters, defaults and deprecation. Architecture tests regenerate the artifact twice, inspect those member contracts and compare it byte-for-byte with the committed `flutter-api.json`.

Compiler selections use `doroti.converter-selection/v4`. The analyzer package is compiler-owned at `tools/Doroti.DartToCSharp/analyzer`; selections cannot inject its implementation path. The report identity pins the Dart SDK/analyzer, the explicit analyzer runtime closure, Flutter revision, Migration IR, lowering rules, emitter, FlutterCompat and content-addressed workspace. `migration-ir.json` contains the normalized package graph, stable library/element identities and declaration/member/statement spans. `source-map.json` maps each selected Dart declaration to generated C# lines. Generated projects contain no checkout-absolute paths and build independently; clean/cache-off, incremental/cache-on and cache-hit outputs must be byte-identical.

Compiler C2 keeps the same selection, IR, report and source-map schemas while pinning the `flutter-aware-c2` profile and `c2.0` lowering rules. Its selected language slice adds callback/lambda expressions, `super` calls, mutable State fields, lifecycle overrides and Widget/State/Key inheritance. `r7-widget-lifecycle` is the executable behavior id for those API bindings; syntax support alone does not promote lifecycle behavior.

Compiler C3 pins `flutter-aware-c3`, `c3.0` lowering and `csharp-net10.4-partial`. The source map includes the top-level Dart `main` span in addition to selected declarations and records `partial-friendly` for declarations that can participate in manual composition. Its const scope means immutable constructor/object-graph emission for the selected Counter package, not Dart const canonicalization. The `r8-counter` fixture promotes only the selected basic-widget and tap/keyboard slice to package-validated behavior.

Compiler C4 pins `flutter-aware-c4` and `c4.0`. Its compatibility rules bind Dart Future/Stream and Navigator to Doroti.FlutterCompat rather than exposing Task or runtime internals. The selected async/navigation app, public MethodChannel port and unsupported-language fixture distinguish behavior-conformant lowering, generated-port warnings and source-span errors. Method-only mixins and expression-bodied extensions are supported; factory/named constructors, dynamic, isolate, Zone and FFI remain explicit diagnostics outside that slice.

Compiler C5 pins `nuget-generated-package`, `flutter-aware-c5`, `c5.0`, and `csharp-net10.5-package-partial`. Selection inputs use stable `package:` URIs resolved through `.dart_tool/package_config.json`; package graph resolution is fixed by `pubspec.lock`, and raw import/export/part/conditional directives are retained in IR. Every NuGet embeds its report, IR, source map, original license, and symbols while excluding Dart source/cache snapshots. Tier C emits `DOTCONV540` and a public port instead of plugin behavior.

P0 adds `doroti.port/v1` as the user-owned entrypoint while retaining converter selections as an internal compiler contract. The manifest separates editable `migration/ports/` inputs from `.doroti/workspaces/<workspace-id>/generated-base`, pins the port mode, source/license, compiler profile, selected symbols, customization roots and required fixtures, and rejects absolute customization paths. `doroti.port-workspace/v1` records the five ownership kinds, generated file hashes and symbol owners. Existing generated workspaces are verified before reuse; direct edits fail instead of being overwritten. `doroti.replacements/v1` rejects duplicate owners and replacements whose reviewed generated-file SHA-256 has drifted.

P1 extends `doroti.port-workspace/v1` with composer identity, the effective project path and a hash inventory for every generated, manual-replacement, partial-extension and platform-port file. Composition suppresses a whole symbol or member only when the generated target is unique and requires its declared manual source to provide the same target exactly once in the generated namespace. The compiler builds `effective/` before atomic publication and verifies both `generated-base/` and `effective/` on cache reuse.

P2 adds `doroti.port-state/v1`, a hash-addressed `manual-snapshot/`, and origin-aware provenance/source maps. The state binds the compiler identity and resolved package graph to aggregate generated-base, manual-input, and effective-artifact hashes. `doroti.adoption/v1` is a review-only runtime-adoption candidate and cannot write product source. `doroti.rebase-report/v1` compares previous/current normalized declaration and member meaning, generated-file context, and manual hashes; it classifies each target as `clean`, `manual-review`, `conflict`, `upstream-symbol-removed`, or `fixture-required`.

Behavior results pin the Flutter revision and case identifiers and are deterministic JSON. Package pilots fix A/B/C version, license, dependency graph and required capabilities. Compiler support reports syntax, API-binding and behavior states separately.

Runtime reports include environment/backend/DPI/resolution, warm-up and first-present, p50/p95/p99 phase/allocation/display-list values, mailbox and process resource deltas, and per-target `pass`, `fail` or `not-verified` evidence. GPU readback evidence records size, per-channel tolerance, maximum and mean delta, and the count/ratio beyond tolerance. Multi-DPI evidence records connected display count, observed scales and scale-changing metrics events. A missing target-machine or readback run is never serialized as pass.

R8 runtime report v2 adds structured post-dispose balance evidence. The target runner warms a fixed raster thread, then performs five isolated create/present/dispose cycles and requires HWND, WGL context, Skia GPU context and GPU frame created/released counts to match, active counts to return to the baseline, and process handle/thread deltas to remain zero within the bounded settling window.

`migration/roadmap2-handoff.json` is the versioned R9 entry contract. It pins the alpha identity and tracked evidence, distinguishes machine-local runtime reports from their committed summary, lists every known unsupported or unverified capability, inventories reusable/missing platform ports, and keeps additional Avalonia-derived candidates explicitly `not-approved`. Architecture tests reject a missing evidence path, an unstated non-pass target result, an empty port inventory, or a candidate that bypasses the vendor approval workflow.

`migration/releases/r9-foundations.json` is the historical R9 exit ledger. G3-0 resets its generated-framework convergence gate to `not-verified`; it cannot promote Goal3 completion from H6 or another pre-Goal3 run.

`migration/flutter-framework/f1-evidence.json` through `f4-evidence.json` use `doroti.flutter-framework-evidence/v2`. `resolvedInventory` is the closure census only. `mechanicalGenerated`, `reviewedGeneratedCs`, `reviewedSourcePortCs`, and `runtimeBound` require one unique Dart element ID and one hashed target per claimed symbol. `compiled` and `behaviorVerified` have independent evidence arrays. The v1 closure `generated`, `manual-adaptation`, `runtime-binding`, and owner fields are legacy inventory annotations and contribute zero Goal3 implementation coverage.

Migration rule: pre-Goal3 F1-F4 evidence cannot be converted by copying its ratios or PASS strings. Regenerate v2 from the pinned closure, retain only candidates produced by a registered semantic compiler profile, reset reviewed/runtime/compile/behavior states to zero, and report every unresolved declaration/member as a blocker. The former F4 support matrix and H6 product evidence were deleted from the active graph without a history copy.

G3-1 adds the general `flutter-framework` profile with an explicit milestone selection. Its `doroti.framework-project-graph/v1` artifact enumerates all 13 public roots and 695 pinned Dart files, then records the selected resolved library graph: stable namespaces, import prefixes, extension candidates, part ownership, SCC membership, generated partitions, project references, and conditional runtime package references. `everyCycleMerged` can be true only when each cyclic SCC is contained by one project partition. Graph-only inputs provide dependency/part evidence but contribute zero mechanical declarations. Unsupported selected syntax emits typed `DOTF0001` span/action diagnostics and a non-zero compiler exit; it never becomes a placeholder or `default` implementation.

G3-2 replaces the historical review-bundle-only `doroti.promotion/v1` contract with `doroti.framework-promotion/v2`. Each canonical Dart element binds a source span, new and old `.g.cs` hashes, reviewer/state, classified issues, optional exact symbol patch, validation cases and ordinary `.cs` target. `review`, `diff`, `promote`, and `rebase` share the same validation. Compiler-general fixes cannot be patched in product source; three-way conflicts are blocking and never overwrite the current target. Product source directories are manifest-closed, and non-intermediate `.g.cs` compile items are rejected.
