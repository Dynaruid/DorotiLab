# NativeAOT migration execution — 2026-09-10

**Resumed after the user installed .NET 11 and the MAUI iOS workload. The real
Doroti host probe now publishes with NativeAOT and runs on the iPhone 12.
The full Testbed (G2) and product distribution path (G3) remain incomplete.**
Changes are uncommitted. Historical stop-point results below are retained; the
resumed-device section at the end supersedes their status.

## Environment and dependency blocker

Starting checkout: `a0bda5621ad04478dd323c281ffe1e2f3c709d9d`. All changes remain in
the working tree. macOS 26.6 / arm64, .NET SDK 10.0.400, runtime/ILC 10.0.11,
Microsoft.iOS 26.5.10315, Xcode 26.6 (17F113). An iPhone 12 was connected and an
Apple Development signing identity was available. The only existing provisioning
profile was for `dev.doroti.testbed`; no separate probe profile was available.

The first global `PublishAot=true` attempt failed NETSDK1203 in neutral libraries.
The Runner's new `DorotiCompilationMode=NativeAot` sets `PublishAot` on the
executable, allowing restore and actual ILC analysis. Testbed analysis then failed
with 3,207 reported errors. The diagnostic collector produced 2,094 unique
code/message records; these are diagnostics, not independent source fixes.

The host probe isolated two issues:

1. SkiaSharp 4.154.0-preview.1.26454.9 `SKGLView` constructs a string-path MAUI
   Binding and triggers IL2026. An iOS-only Doroti-owned View now implements the
   existing ISKGLView handler protocol without that binding. The probe no longer
   reports this error. Existing Metal/Graphite renderers and touch handlers remain.
2. MAUI HybridWebView's nested exported handlers are referenced by a generated
   module initializer and trigger IL2026/IL3050. The compiler response already
   contains `Microsoft.Maui.RuntimeFeature.IsHybridWebViewSupported=false`.
   Both managed-static and trimmable-static registrar experiments failed. MAUI
   10.0.101 also failed. A separate minimal MAUI application with **no Doroti or
   Skia references** reproduced 24 IL diagnostics plus MSB3077 (25 errors).

The official [MAUI fix PR #35626](https://github.com/dotnet/maui/pull/35626) was
merged into `net11.0` at `71b880aa107cdace29ef93dc211ec82d2dc20382`. The
[MAUI 11 Preview 6 release](https://github.com/dotnet/maui/releases/tag/11.0.0-preview.6.26360.8)
lists it. This is a supported investigation direction, **not a Doroti .NET 11
success result**. The .NET 11 SDK candidate discovered from Microsoft's release
metadata was `11.0.100-rc.1.26425.128`; NuGet also lists MAUI
`11.0.0-rc.1.26451.6`.

An isolated SDK download was attempted under `.doroti/cache/dotnet11`. Its SHA-512
matched Microsoft's metadata, but extraction failed because the environment's
Python 3.9 does not support `tarfile.extractall(filter=...)`. The archive remains
at `.doroti/cache/dotnet11/sdk.tar.gz`; no SDK was extracted or installed and no
workload installation was started. The user then requested stopping work.

## Implemented and verified

| Area | Change | Evidence/result |
| --- | --- | --- |
| Runtime callbacks | Explicit typed error adapters, one/two argument dispatch, Future/Task recovery, original handler exceptions, explicit invalid-result errors; removed DynamicInvoke/GetParameters | NativeAOT publish and native execution PASS |
| Index/predicate | IDartEnumIndex, FontWeight/TextInputType implementations, typed collection predicate invocation, direct rendering indices | Runtime contracts and product Release build PASS |
| Runtime packaging | IsAotCompatible enabled on Runtime; isolated NuGet consumer without product ProjectReferences | Package build, NativeAOT consumer publish and execution PASS |
| JSON | Existing explicit Dart JSON codec | NativeAOT publish and execution PASS |
| Localizations | ILocalizationsDelegate bridge, static type/support/load/reload calls, unchanged generic subclass extension path | External delegate NativeAOT fixture PASS: synchronous delivery, mixed types, deferred Task, reload and original errors |
| RenderView | Covariant RenderView property and direct initial-frame/child operations | Product build and focused dispatch checks PASS |
| Shader resources | Explicit resource-owner registration; built-in entrypoints register themselves; removed runtime assembly search/load | Existing shader hash/ABI/GPU contract PASS |
| Runner/CLI | Opt-in compilation mode; interpreter/descriptor exclusion; mode/RID paths, lockfiles, launch identity v4; no NativeAOT normal-run reuse | Evaluation contract and launch identity regression PASS |
| Evidence tooling | PE metadata CallSite inventory, diagnostic collector, bundle/ZIP byte/hash inventory, conservative Mono evidence checker, 1,200-second command runner | Accounting, hash, link, missing-evidence and Mono-injection rejection tests PASS; positive iOS evidence-checker path still unqualified |

The actual Release metadata inventory contained 2,194 CallSite fields:
Widgets 1,098; Material 748; Cupertino 285; Painting 56; Rendering 2; Testbed 5.
This inventory includes methods not reached by a particular app; it is not the
same as publish reachability or a count of textual `dynamic` tokens.

The complete Testbed's neutral Release build passed with zero warnings/errors.
The modified iOS host compiled during probe attempts, but device behavior has not
been verified. A signed Mono baseline publish was still compiling Mono AOT when
the user requested stopping; its dotnet/native compiler processes were terminated.
It is **interrupted**, not a successful baseline. No bundle from it was installed.

## Durable local evidence

All paths below are under `Doroti/artifacts/native-aot/`, which is ignored local
evidence storage. `.result.json` siblings contain exact commands and exit status.

| Evidence | Path |
| --- | --- |
| Initial property propagation failure | `initial-publish.log`, `.binlog` |
| First full Testbed ILC diagnostics | `profile-publish.log`, `.binlog`, `initial-blockers.json` |
| Original host dependency repro | `ios-probe-analysis3.log`, `.binlog` |
| Owned View + alternative registrar | `ios-probe-owned-view.log`, `.binlog` |
| MAUI 10.0.101 experiment | `ios-probe-maui101.log`, `.binlog` |
| Independent MAUI-only repro | `maui-minimal.log`, `.binlog` |
| Runtime/JSON/localization native results | `runtime-publish.log`, `runtime-execute.log`, `json-publish.log`, `json-execute.log`, `localizations-publish3.log`, `localizations-execute.log` |
| NuGet package and consumer | `packages/`, `runtime-pack.log`, `runtime-package-consumer/result.json`, `publish.log`, `execute.log` |
| Product/regression checks | `framework-build2.log`, `shader-contract.log`, `dynamic-dispatch2.log`, `launch-identity2.log`, `tool-contract.log` |
| IL inventory | `release-il-audit.json` |
| Interrupted baseline | `mono-baseline-publish.log`, `user-stop.json` |
| SDK preparation failure | `dotnet11-install.log` |

The Runtime NuGet package SHA-256 was
`b42329191903985307ece91c85e261baba10cbb438d254ff14d8e30b91c9a4e8`;
its isolated NativeAOT consumer executable SHA-256 was
`290a9612dc6a0a279fbbe1b73065ea7a467a314c5da6e5f7cf562d10f318a411`.
These are desktop contract artifacts, not signed iOS apps.

## Alternative requested at the stop point

Doroti does not use HybridWebView, and its NativeAOT feature switch is already
false. Simply dropping the feature cannot remove the generated registrar roots
observed in the minimal repro. Keeping .NET 10 requires investigating a compatible
MAUI/registrar fix or a corrected package. The .NET 11 option is on hold; no new
implementation or validation was started after the stop request.

## Resume after choosing the dependency direction

1. Confirm the installed .NET 11 SDK and matching iOS/MAUI workload. Installing
   .NET 11 alone does not change this checkout: `Doroti/global.json` still pins
   `10.0.400`, projects still select `net10.0-ios`, and MAUI remains at 10.0.90.
2. Select .NET 11 explicitly in an isolated minimal repro, use a `net11.0-ios`
   executable and a MAUI 11 version containing #35626, then publish with warnings
   as errors. Do not silently switch the entire product graph first.
3. If that passes, repeat the actual host/native-binding probe; complete its
   text/image/shader, GC/callback, signed-device and Mono-absence evidence.
4. Resume the work2 N4–N6 framework contracts and Testbed DLR removal. .NET 11
   does not make C# dynamic/DLR execution NativeAOT-compatible.
5. Complete package/template/CLI consumption, device behavior and measurements
   before changing iOS's production default. Android/Web/other hosts have not
   been promoted to NativeAOT.

No startup/frame/memory acceptance result, size reduction, iOS NativeAOT success,
or overall migration completion is claimed. Final status remains
`monoAbsent=notVerified`, `nativeAotPublish=fail` for the iOS attempts,
`functional=notVerified`, `performance=notVerified`, `sizeDeltaBytes=null`.


## Resumed .NET 11 device verification

The user installed SDK `11.0.100-rc.1.26425.128` and the MAUI iOS workload.
The selected iOS pack is `26.5.11720-net11-p6`, NativeAOT runtime/ILC
`11.0.0-rc.1.26425.128`, and the probe uses MAUI `11.0.0-rc.1.26451.6`.
Scoped probe global.json files select SDK 11; the product SDK pin remains 10.
The cached SDK archive was subsequently extracted with tar and its local workload
installed successfully before the user's global workload installation.

`net11-minimal.log` confirms the HybridWebView dependency error is resolved.
`net11-probe-signed3.log` records actual ILC, native linking, signing and publish
with zero warnings/errors. The app was installed using the existing Testbed
provisioning ID `dev.doroti.testbed`. `net11-probe-install3.json` records installation;
`net11-probe-launch4.log` records successful execution after the user unlocked it.
The console command waits for app termination; collection was explicitly ended by
terminating probe PID 2588 at 20:55 KST after evidence had been collected.

The device console reports `dynamicCode=False`, native roundtrip completion after
forced GC, decoded image and prepared shader, and two submitted scenes.
`net11-probe-device-evidence-final.json` reports the actual host metadata
`.NETCoreApp,Version=v11.0/iOS26.5`, MAUI 11 RC1, Graphite-Metal,
two presented frames, zero failed frames and zero software fallback frames.
There were no pointer events, so this does not establish interaction coverage.

Remaining probe limitations: its logical viewport is 320×480 because the empty
launch-screen dictionary was stripped. A nonempty UILaunchScreen source change
has not yet been published or verified. Forced GC emitted two observer disposal
warnings, and UIKit emitted a background-fetch manifest warning. Lifetime and
full-screen validation remain open. Probe bundle size is not comparable to the
full Testbed baseline, and startup/frame/memory acceptance remains unmeasured.

The native evidence checker attributes legacy Mono-named ABI symbols to actual
NativeAOT Xamarin bridge objects from the recorded link map. It rejects unknown
owners and Mono runtime payloads; names alone are insufficient. Each rebuild
requires regenerating the evidence against its current ILC inputs, link map and
signed bundle. Historical evidence hashes must not be used for a later rebuild.

Box/sliver LayoutBuilder now uses static callback/layout-info and child contracts.
The Debug fixture exercises real PipelineOwner layout, callback replacement and
attach/detach (`layout-contract-debug4.log`, PASS). Widgets Release CallSite count
fell from 1,098 to 1,081; this is partial framework progress, not DLR elimination.
Latest-fixture NativeAOT validation is in progress. Locale-load race/error cleanup,
remaining Widgets/Material/Cupertino DLR, full Testbed publish, packages, templates
and all platform regressions are still outstanding.

Follow-up verification: the latest layout fixture publishes and runs successfully
(`layout-native-publish-final.log`, `layout-native-execute-final.log`).
`net11-probe-mono-check-final.log` successfully regenerates
`net11-probe-mono-evidence.json` against signed3: `monoAbsent=pass` and
`nativeAotPublish=pass` for this probe only. Device/performance gates remain separate.

The opt-in source Testbed profile now selects .NET 11/MAUI 11 across runner, host,
target and native binding, including RID-less restore discovery and the embedded
target manifest. Five evidence/mode/dependency-graph tests pass in
`net11-testbed-graph-contract.log`. A full Testbed publish is running to collect
remaining framework errors; this does not qualify the packaged-template path.

The initial .NET 11 full Testbed publish failed in ILC with 12 aggregate AOT/trim
diagnostics plus compiler failure (`net11-testbed-profile-publish.log`). The profile
now sets `TrimmerSingleWarn=false` to reveal individual method diagnostics without
suppressing warnings. Detailed collection is in `net11-testbed-diagnostics.log`.

Rendering's final dynamic reference-count increment now uses long arithmetic.
LayerHandle construction also acquires the initial reference, matching later setter
acquisitions. An external Layer subclass verifies shared ownership, same-instance
assignment, replacement and exactly-once disposal after the final release.
Debug and actual NativeAOT execution pass (`layout-layer-debug.log`,
`layout-layer-native-publish.log`, `layout-layer-native-execute.log`).
`rendering-layer-il-audit.json` records zero Rendering CallSite fields in both
Debug and Release. This does not establish zero DLR in other framework layers.

Detailed .NET 11 Testbed ILC collection is now complete:
`net11-testbed-diagnostics.log` and `net11-testbed-blockers.json` contain 1,994
unique diagnostics (634 IL2026; 1,360 IL3050), all attributed to RuntimeBinder or
CallSite use. These are diagnostic records, not independent source locations.
No HybridWebView or Skia Binding dependency errors were reported in this attempt.
The full publish failed; G2/G3 remain incomplete. The next contract boundary is
ImageProvider/ResizeImage and their Widgets consumers, followed by the remaining
State/Material/Cupertino dispatch contracts.


## Continued contract migration (same checkout, uncommitted)

The complete Testbed remains **G2 failed / G3 incomplete**. This continuation did
not install or launch another iPhone bundle. The earlier G1 device evidence is
unchanged. HEAD remains `a0bda5621ad04478dd323c281ffe1e2f3c709d9d` plus the working
tree changes; no automatic commit or default-profile promotion was performed.

Product changes:

- `IImageProvider` forwards typed keys, resolution/cache/eviction, buffer/image
  decoding and external virtual overrides without DLR. ResizeImage, decoration,
  Image/FadeInImage/ImageIcon, scroll-aware providers, image colors and thumb/avatar
  images use the contract. FadeInImage factories now construct their providers
  before invoking the typed constructor. The Testbed's retry path uses the typed
  provider and a real empty ImageConfiguration.
- `IRestorableProperty` carries registration, typed default/restored values,
  serialization, notifications and ownership. All restoration-mixin copies in
  Widgets/Material/Cupertino and their controller/property call sites use the
  bridge. External properties retain virtual dispatch and nullable values.
- Focus geometry, reparent/detach, text selection and editable-text render calls
  are static. FocusNode/FocusManager now expose their existing diagnostic children
  through DiagnosticableTree. RawRadio and both radio painters use
  IToggleableState; checkbox/slider/switch defaults retain their theme base types.
- Painting's unsupported WebImageInfoIo stubs throw typed exceptions. ImageInfo's
  Debug disposal check now uses the actual image handle's disposed state; the
  existing static stack-trace stub always returns an empty list and incorrectly
  rejected valid disposal. This Debug-only correction followed the iOS diagnostic
  publish and was validated by the actual image-sizing regression.
- Painting/Rendering enable `IsAotCompatible`. The IL auditor now fails on empty
  or missing inputs and supports `--expect-zero`. The clean-layer script and CI
  workflow were added; remote GitHub execution has not been observed.

Public API changes are documented in `validation/native-aot/README.md`. Existing
compiled consumers must rebuild; custom providers/mixin owners use the explicit
interfaces. No broad descriptor, warning suppression, runtime binder fallback,
widget/resource deletion or NativeAOT default promotion was added.

### Reproduction and results

All commands use `validation/native-aot/run.py` and its 1,200-second process-tree
limit. Each log has a sibling `.result.json` containing the exact command, SDK
invocation, elapsed time and exit code. Paths below are relative to
`Doroti/artifacts/native-aot/`.

| Contract | Evidence | Result |
| --- | --- | --- |
| External image keys/decoders/scheduling/resize | `image-bridge-debug.log`, `image-bridge-native-publish.log`, `image-bridge-native-execute.log` | Debug and actual osx-arm64 NativeAOT PASS; no AOT/trim warnings |
| External restoration values/owners/listeners | `restoration-debug.log`, `restoration-native-publish.log`, `restoration-native-execute.log` | Debug and actual NativeAOT PASS |
| Focus geometry, three traversal policies | `focus-native-publish.log`, `focus-native-execute.log`, `bridges-final-debug.log` | NativeAOT and Debug PASS |
| Full Material/Cupertino fixture compilation | `final-debug-build.log` | Zero warnings/errors |
| Actual ScrollableState restoration and image factories | `bridges-final-debug.log` | PASS |
| Actual resize pixels/alpha/WebP and failure cleanup | `image-sizing-final-debug.log` | PASS |
| Mounted selection controls and pointer interaction | `selection-controls-final-debug.log`, `selection-controls/` | PASS |
| Focus section traversal | `focus-section-final-debug.log` | PASS |
| iOS-style menu theme/pagination/copy/cut/paste | `text-selection-final-debug.log` | PASS on the host fixture; not physical-device evidence |
| SDK 10 AOT-analyzed Painting/Rendering Debug/Release | `clean-layer-gate/gate-results.json` | PASS, zero CallSite fields; missing/empty-input checks PASS |
| Full .NET 11 iOS Testbed NativeAOT | `net11-testbed-final-contracts-publish.log`, `.result.json`, `net11-testbed-final-contracts.binlog` | ILC reached; publish FAILED, no signed deployable output |

The initial focus fixture assertions incorrectly matched exception message text;
it now checks the actual ArgumentError/AssertionError contracts in Release/Debug.
The first image-sizing run exposed the disposal check above, and the next exposed
its offscreen fixture's missing microtask/frame pump. The fixture now supplies an
explicit asynchronous scheduler. Neither failure was suppressed or counted as a
pass before correction.

Latest full iOS diagnostics (`net11-testbed-final-contracts-blockers.json`):
**1,094 unique DLR diagnostics: IL2026 360 / IL3050 734**, down from the preceding
1,994-record inventory. Counts describe compiler diagnostics, not unique source
locations. The ILC failure and these errors remain open; no G2/G3 success is claimed.

Actual IL inventories, with assembly hashes in `final-il-audit-debug.json` and
`final-il-audit-release.json`:

| Assembly | Debug CallSite fields | Release CallSite fields |
| --- | ---: | ---: |
| Painting | 0 | 0 |
| Rendering | 0 | 0 |
| Widgets | 693 | 599 |
| Cupertino | 154 | 144 |
| Material | 250 | 250 |
| Testbed | not audited | 0 |

Release total: **993** across those six assemblies. Release inputs are the actual
.NET 11 iOS graph's neutral framework assemblies. The historical initial total
2,194 also predates earlier continuation changes; it is not solely this patch's
contribution. A zero count for Painting/Rendering/Testbed is not a zero count for
the complete application closure.

Remaining contract groups include WidgetInspector, SliverMultiBoxAdaptorElement,
Hero/ModalRoute/overlay, menu/drag-target/selection delegates, CalendarDelegate and
Cupertino navigation/context-menu render boundaries. Full NuGet/template iOS
consumption, device interaction/lifetime/accessibility, baseline/performance,
platform-wide regression and G3 promotion remain unverified.


## Form, sliver and calendar continuation

Following the request to continue, the same uncommitted checkout gained these
additional static contracts:

- FormState tracks `IFormFieldState` instances and calls typed registration,
  save/reset, validation, focus and error-clear operations. FormFieldState<T>
  inherits the bridge; heterogeneous public enumeration and granular failure sets
  retain their previous object return types.
- SliverMultiBoxAdaptorElement and the prototype-list element expose their actual
  renderer types. Child parent data, constraints, index updates, insertion, move,
  removal and parent invalidation use direct calls. No list item or keep-alive
  feature was removed.
- CalendarDelegate and DateTimeRange use DateTime directly or `ICalendarDate<T>`
  for application-defined components and duration arithmetic. The protected
  `getDateParts` extension point supports delegate-specific representations.
  Existing null-comparison and Debug endpoint-ordering semantics are preserved.

Validation evidence under `artifacts/native-aot/`:

| Evidence | Result |
| --- | --- |
| `forms-native-publish.log`, `forms-native-execute.log` | NativeAOT compile/link and execution PASS with no AOT/trim diagnostics. External field values, granular validation identity, native sliver render hooks/prototype child and resource disposal covered. |
| `forms-debug.log` | Same low-level contracts PASS in Debug. The fixture explicitly initializes State lifecycle; it is not a full native widget-tree boot. |
| `calendar-native-publish.log`, `calendar-native-execute.log`, `calendar-debug.log` | Gregorian/custom calendar comparisons, normalized ranges, leap/time behavior, and custom difference dispatch PASS in NativeAOT and Debug. |
| `form-sliver-debug.log` | Actual mounted Form validation/errors/clear/save/reset/unregistration and sliver keyed state retention, physical reorder, child removal, prototype extent and exactly-once widget disposal PASS. |
| `form-sliver-scroll-regression.log` | iOS-style scroll-stop/tap pointer regression PASS on the host fixture. |
| `net11-form-sliver-calendar-publish.log`, `net11-form-sliver-calendar.binlog` | Full iOS graph compiled to ILC; NativeAOT publish still FAILED. |

Exact commands and timing are in the sibling `.result.json` files, all with
1,200-second limits. Initial C# fixture build errors were corrected before the
reported passes. The production prototype-element override also needed the more
specific renderer return type when its parent getter was corrected.

Latest iOS compiler inventory: **996 unique DLR diagnostics** (IL2026 **328**,
IL3050 **668**), compared with the previous 1,094. The diagnostic collector output
is `net11-form-sliver-calendar-blockers.json`. These are diagnostics, not distinct
source edits. G2/G3 remain incomplete; no new signed iPhone bundle was produced.

Actual metadata counts in `form-sliver-calendar-il-{debug,release}.json`:

| Assembly | Debug | Release |
| --- | ---: | ---: |
| Widgets | 635 (was 693) | 543 (was 599) |
| Material | 223 (was 250) | 223 (was 250) |
| Cupertino | 154 | 144 |
| Painting | 0 | 0 |
| Rendering | 0 | 0 |

Release total for these framework assemblies: **910**, down from 993. The selected
Form, sliver/prototype, CalendarDelegate and DateTimeRange types no longer contain
CallSite fields. The new fixture entrypoints were added to the existing CI native
loop; remote CI has not been run. See the validation README for consumer API
migration details. Inspector, route/overlay/Hero, menu/drag/selection and Cupertino
navigation remain among the unresolved groups, followed by full package/device/
performance gates.


## UI static dispatch continuation

The full Testbed first passed an **unsigned** iOS NativeAOT publish in
`net11-static-ui-publish.log` (418.1 seconds, exit 0): actual ILC, native linking
and IPA creation. `net11-static-ui-blockers.json` contains zero AOT/trim diagnostics.
`static-ui-release-il.json` confirms zero compiler-generated CallSite fields in
the complete Widgets, Material and Cupertino Release assemblies. This is not
signed-device, performance or G3 distribution evidence. Later Runtime/Tree fixes
require a fresh candidate; signed validation is in progress.

Mixed-result mounted navigation (`--route-bridges`) and the standalone native
`routing` contract passed. They cover PopScope veto/update/results, Hero flights,
Cupertino previous titles, external configuration structs, parser type checks,
local-history identity and strict incompatible-result rejection. The mounted
drag scenarios pass; tree and persistent-header follow-up continues.

The native Runtime suite passed after fixing invariant generic Future handling
in `AwaitFutureOrValue<T>`, including Future<List<int>> consumed as IEnumerable<int>,
boxed values, delayed completion, void/null, incompatible types and original
exceptions. The inspector fixture is being rerun against that fix.

The AOT analyzer and Debug/Release IL guard now cover all 12 framework layers;
the expanded gate still needs its final run. CI has nine separate native contract
matrix jobs; remote CI has not run. The Runner SDK now packages native link-map
and input collection. Template profile wiring has been updated, but an isolated
NuGet/template consumer still needs to publish. Default promotion remains gated.

`performance-plan.json` freezes sampling and tolerances before any full candidate
performance measurements. It is a plan, not benchmark evidence.


## Final signed Testbed and installed-template consumer

| Check | Evidence | Result |
| --- | --- | --- |
| Latest signed Testbed ILC/link/publish | `net11-testbed-release-publish-retry.{log,result.json,binlog}` | PASS, zero warnings/errors |
| Latest Testbed Mono absence | `net11-testbed-release-mono-evidence.json` | PASS |
| Final Testbed restored to iPhone | `net11-testbed-final-{install,launch}.json` | PASS |
| Actual interaction | `net11-testbed-interaction-evidence.json` | 906 pointer events, 4,414 presented, failed/fallback 0 |
| All 12 framework layers Debug/Release | `static-ui-clean-layers-final/gate-results.json` | PASS, zero DLR |
| Mounted route/form/metrics/input/menu/image regression | `static-ui-regressions/results.json` | 9/9 PASS |
| Drag, integer tree and four persistent-header combinations | `drag-tree-debug.log` | PASS |
| Native routing/inspector/Future contracts | `routing-native-run.log`, `inspector-native-run.log`, `static-ui-runtime-native-run.log` | PASS |
| Isolated local feed | `ios-package-validation/result.json` | 22 packages |
| Installed template + package consumer | `ios-template-consumer/result.json` | Native publish, sign, install, external generic APIs and rendering PASS |
| Consumer Mono absence | `ios-template-consumer/mono-evidence.json` | PASS |

Final Testbed raw bundle: **42,616,772 bytes** (42.62 decimal MB); compressed IPA:
**18,185,146 bytes** (18.19 MB). The user's approximately 56 MB installed-storage
observation is separately recorded in `user-device-observation.json`, along with
their confirmation of smooth interaction. No controlled performance pass is
inferred from that observation.

Package testing found and fixed two additional SDK issues: PublishAot must be set
before the Apple SDK chooses the runtime, and template BaseIntermediateOutputPath/
BaseOutputPath must not override ArtifactsPath. The consumer checks actual
NuGet asset files and rejects product ProjectReference dependencies. It uses
external generic State, LocalizationsDelegate and Action types and logged
`NATIVEAOT_CONSUMER ... PASS value=7` on the iPhone. Rendering presented two frames
without failure/fallback. The consumer was deliberately stopped and Testbed restored.

.NET 11 iOS no longer supports Mono (installed SDK diagnostic NETSDK1242). An
earlier attempted net11 Mono comparison actually contained libcoreclr and is
explicitly ineligible as a Mono baseline. The Mono profile now selects its runtime
explicitly and uses the supported .NET 10/MAUI 10 recovery path. A net10 comparison
build was started separately; only a successful result may be counted.

Disk-full attempts are failures, not successful publishes. Failed consumer caches
and the obsolete private extracted SDK cache were reclaimed; its verified installer
archive, sources, logs and final artifacts were retained. The global installed SDK
and workloads were not removed.

Remaining formal gates: complete physical rotation/IME/accessibility coverage,
repeated view teardown/native lifetime, controlled startup/frame/memory comparison,
other-host execution, remote CI and default promotion. Remote orientation commands
are unsupported by this iPhone. MAUI observer warnings are investigated in
`maui-observer-investigation.json`; installed MAUI OnLoaded code removes KVO
observers without disposing the wrappers, a plausible source of the finalizer
warning emitted by [macios NSObject.Observer](https://github.com/dotnet/macios/blob/main/src/Foundation/NSObject2.cs).
The particular warning objects were not allocation-traced, so complete attribution
and lifetime clearance are not claimed. NativeAOT remains an explicit supported
experimental profile, not a silently promoted default.


The actual .NET 10 Mono comparison publish subsequently completed successfully
(`net10-mono-baseline-publish.result.json`, exit 0). Its bundle contains Mono
`.aotdata.arm64` payloads. Bundle size is **104,701,336 bytes**, compared with
NativeAOT **42,616,772 bytes**: **62,084,564 bytes / 59.30% smaller**. IPA bytes
are **37,351,329 → 18,185,146**. `final-size-comparison.json` preserves the
.NET/MAUI version difference and explicitly leaves performance unverified.
This comparison bundle was not installed over the final NativeAOT Testbed.
The final device snapshot is 390×844 logical pixels, 1,237 presented frames,
zero failed frames and zero software fallback.
