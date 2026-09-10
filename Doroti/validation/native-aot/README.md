# NativeAOT migration validation

This directory contains migration tools and focused contracts. The full Testbed
and an isolated, installed-template NuGet consumer now **publish as signed iOS
NativeAOT apps and run on an iPhone**. Both pass the native-link/bundle Mono
absence check. All 12 main framework assemblies pass Debug/Release zero-DLR
and AOT analysis. Exhaustive G2 device/lifetime and quantitative performance
gates, other-platform execution and G3 default promotion remain open; see
[the execution report](../../docs/validation/nativeaot-2026-09-10.md).

Run commands from the repository root. `run.py` imposes the repository's 1,200
second limit, captures a log, and writes a sibling `.result.json` with the exact
command, elapsed time, exit code and timeout status. Output under `Doroti/artifacts`
is durable local evidence; `.doroti/tmp` is disposable.

## Experimental iOS profile

```powershell
pwsh -File Doroti/eng/doroti.ps1 publish -App DorotiTestbedApp -Platform ios -Rid ios-arm64 -CompilationMode NativeAot
```

The CLI selects the runtime on the executable and places the complete graph in
`.doroti/cache/compilation/<mode>/<rid>`. NativeAOT is opt-in while gates remain
open; `-CompilationMode Mono` is the explicit comparison/recovery profile. The
CLI rejects NativeAOT `run`, because a normal build/run does not compile NativeAOT.
The source Testbed NativeAot profile selects `net11.0-ios` and MAUI
`11.0.0-rc.1.26451.6` across the runner, host, target and native binding. Use
SDK `11.0.100-rc.1.26425.128` and the matching installed iOS workload.
`Doroti/global.json` still selects SDK 10 inside the product directory; run
these commands from the repository root. An isolated packaged-template consumer has published and run successfully.
Direct invocation, including an isolated output path:

```sh
python3 Doroti/validation/native-aot/run.py --log Doroti/artifacts/native-aot/testbed.log -- \
  dotnet publish DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Release -r ios-arm64 \
  -p:DorotiCompilationMode=NativeAot -p:ArtifactsPath=/absolute/path/to/nativeaot-output \
  -p:UseSharedCompilation=false -bl:Doroti/artifacts/native-aot/testbed.binlog -v:normal
```

Do not pass `PublishAot=true` globally to this multi-platform application graph:
it leaks `ios-arm64` NativeAOT selection to neutral libraries and fails NETSDK1203.
Do not add a trimming descriptor or suppress AOT warnings to make this command pass.

## Contracts

- `runtime-async-contract` and `json-codec` (siblings of this directory): publish
  for `osx-arm64` with `-p:PublishAot=true`, then execute the native output. These
  small graphs support a global flag. Both were published and executed successfully.
- `localizations`: `PublishAot` is declared on its executable. Publish with `-r
  osx-arm64`, **without** a global `PublishAot` flag. Its external delegates exercise
  synchronous delivery, mixed value types, deferred completion, reload and failure.
- `ios-probe`: current MAUI/Metal/Graphite host plus native binding. The current
  shared probe exercises shapes, text, image decoding, shader preparation, GC and
  native completion callbacks. Use `net11-probe` for the validated .NET 11 host;
  its scoped global.json selects SDK 11. Viewport/lifetime checks remain open.
  `DorotiProbeMauiVersion` optionally
  changes only this probe's root MAUI package for dependency experiments.
- `maui-minimal`: reproduces the HybridWebView compiler diagnostics with no Doroti
  or Skia dependencies. An unsigned compile can use `-p:EnableCodeSigning=false`;
  it is not installable device evidence.
- `validate-runtime-package.py --feed <packed-runtime-feed> --output <evidence>
  --rid osx-arm64`: copies the runtime contracts into a fresh NuGet consumer, uses
  an isolated package cache, publishes and runs NativeAOT. There is no product
  ProjectReference. Pack `Doroti/src/Doroti.Runtime` into the feed first.
- `tool-contract.py`: checks byte accounting, hash changes, symlinks, missing
  evidence/Mono injection, and Debug/Release Mono → NativeAOT → Mono evaluation.
- `layout`: actual NativeAOT publish and execution of box/sliver callback replacement,
  layout-info delivery and child attach/detach using PipelineOwner.
- `il-audit`: host-only PE metadata reader. Pass compiled DLL paths after `--`
  when using `dotnet run`; stdout is JSON containing actual compiler-generated
  CallSite fields. It does not execute or load product assemblies.
- `collect-blockers.py <publish.log> --output <blockers.json>`: records actual
  ILC/trim diagnostics and removes repeated MSBuild summary messages.

## Extension contracts

Custom enum-shaped values implement `IDartEnumIndex`; CLR enums retain their
numeric conversion. Error callbacks use Action/Func and preserve one/two argument
forms. Use `DartErrorHandlers.Adapt` for narrower exception types, custom delegates
and value-returning callbacks on untyped futures; `AdaptTask` normalizes Task
results. Unsupported shapes fail explicitly. A typed Future's legacy Action
recovery returns `default(T)`; an incompatible value result throws rather than
silently recovering. Handler exceptions propagate without reflection wrappers.

`LocalizationsDelegate<T>` implements `ILocalizationsDelegate`; external subclasses
inherit the bridge without registrations. Its `Future<T>` is returned through the
base Future contract so `SynchronousFuture` delivery remains synchronous.

Shader owners register with
`FrameworkShaderLoader.RegisterResourceOwner(typeof(Owner).Assembly)` before
direct `LoadProgram` calls. Built-in InkSparkle/stretch entrypoints do this
themselves. Hash/ABI checks, async diagnostics, and GPU compilation remain active.
No runtime assembly search/loading fallback is used.

On iOS, `DorotiGraphiteView` now derives from the Doroti-owned `DorotiSkiaView`.
It implements the existing `ISKGLView` handler protocol. Direct consumers of the
previous concrete SKGLView surface API need to rebuild against the new type.
Android and Catalyst retain their existing surface type.

## Evidence

`size-report.py <bundle-or-archive> --mode Mono|NativeAot --rid <rid> --commit <sha>
--output <json>` inventories bytes and SHA-256 without following bundle symlinks.
Raw bundle, ZIP compressed/uncompressed bytes, MB and MiB are separate; installed
and store transfer sizes remain null until measured.

`verify-mono.py` takes `--bundle`, `--publish-result`, `--ilc-response`,
`--native-inputs` (or `--link-response`), `--link-map`, and `--output`.
The net11 probes import `native-evidence.targets`, which records native linker
inputs and a link map. Missing evidence returns
`monoAbsent=notVerified` with nonzero exit. Mono payload/link/symbol matches fail.
The successful-publish log, actual ILC command, runtime pack, native linker and
final bundle must agree before it can pass. The positive path passes against the
signed .NET 11 device probe. Legacy Mono-named ABI functions qualify only when
the link map attributes them to the recorded Xamarin NativeAOT bridge objects;
unknown owners and Mono runtime payloads fail. The linked and signed executable
are tied by their Mach-O UUID, with separate hashes for stripped/signed bytes.
Functional/device/performance gates are always separate from static inspection.

## Image, restoration, focus and control contracts

The new `images`, `restoration`, and `focus` executables publish with `-c Release
-r osx-arm64` (their executable projects select `PublishAot`). Run their published
executables with `--native`. They exercise consumer-defined value/reference image
keys, synchronous and scheduled/deferred key delivery, decoding errors, resize
policies, external restoration values/owners/listeners/disposal, and the three
focus traversal policies. They are focused host contracts, not iOS G2 evidence.

Image-consuming APIs now accept `IImageProvider`. Existing `ImageProvider<T>`
subclasses inherit the key-erasing bridge; arbitrary duck-typed objects must move
to that contract. `RestorableProperty<T>` similarly implements
`IRestorableProperty`; restoration owners and mixin collections use that interface.
`RawRadio` builders receive `IToggleableState`, inherited by
`ToggleableStateMixin<S>`. `DisposableBuildContext<T>` implements
`IDisposableBuildContext` for scroll-aware image providers. These public signatures
require consumers and custom mixin implementations to rebuild/migrate. No runtime
registration or closed-generic reflection is involved.

`fcr7-material-widget --aot-bridges` verifies production ScrollableState bucket
registration, update notifications, replacement, enabled state, and disposal, plus
image factories and focus geometry. `--image-sizing`, `--selection-controls <dir>`,
`--section-index`, and `--ios-text-menu` cover real rasterization and mounted input.
The raster-only image-sizing fixture supplies its own asynchronous microtask
scheduler because it does not register a frame-dispatch host.

`check-clean-layers.py --output <evidence-directory>` builds all 12 main framework assemblies
with AOT analysis in Debug and Release and rejects any CallSite fields. It also
checks that missing and empty input sets fail. The audit tool's `--expect-zero`
flag exits 1 for actual call sites and 2 for missing/invalid inputs.
`.github/workflows/native-aot-contracts.yml` wires this gate and nine NativeAOT
contract publish/execution checks into CI. The workflow has not yet run on GitHub;
full-device coverage and default promotion remain open; signed iPhone smoke and
full-framework zero-DLR checks have passed locally.


## Form, sliver and calendar continuation

`forms` now covers external string/integer FormFieldState implementations plus
actual SliverMultiBoxAdaptorElement render hooks (slot propagation, attached child
insertion/reorder/removal, prototype child and disposal). Its low-level fixture
initializes the field lifecycle directly. The separate mounted host test
`fcr7-material-widget --form-sliver` verifies the full registration/rebuild/reset/
removal lifecycle, real validators, keyed state retention, physical child order,
and prototype extent. `--scroll-tap` remains the pointer/scroll regression.

`calendar` tests Gregorian leap days, time components, existing Doroti null
comparisons, and custom date values. DateTime is supported directly. Custom structs
can implement `ICalendarDate<T>` for Year/Month/Day and `difference(T earlier)`;
CalendarDelegate subclasses may instead override `getDateParts` when appropriate.
Custom DateTimeRange subclasses can override `duration`. The existing Debug range
ordering check still requires comparable endpoints. Unsupported shapes fail
explicitly. CalendarDelegate's existing `null/null => false` behavior is retained;
DateUtils' distinct behavior has not been changed.

FormFieldState<T> inherits `IFormFieldState`; FormState's existing object-based
`fields` and `validateGranularly` return types stay intact. Custom states that
previously relied on duck typing must implement the static interface. Custom
SliverMultiBoxAdaptorElement subclasses that override renderObject must return
RenderSliverMultiBoxAdaptor or a subtype, matching the corrected covariant getter.
The prototype element returns its concrete renderer.

The CI native-contract loop includes `forms` and `calendar` after their successful
local Debug and NativeAOT runs. A remote GitHub workflow run and complete iOS
application execution are still separate, unverified gates.

## Final iOS and package evidence

`net11-testbed-release-publish-retry` is the final successful signed source
publish. `net11-testbed-release-mono-evidence.json` passes actual ILC, native-link
inputs, link-map ownership and final executable UUID/hash checks.
`net11-testbed-final-install.json` and `net11-testbed-final-launch.json` record
the restored Testbed installation after the consumer test.

The recorded Testbed interaction snapshot has 906 native pointer events, 4,414
presented frames, zero failed/fallback frames, and matching final input/presented
sequence 906. The user confirmed smooth interaction and approximately 56 MB
installed size. Tool-measured bundle bytes are 42,616,772 and compressed IPA
bytes are 18,185,146; these are distinct from installed storage.

`pack-ios-feed.py --artifacts <successful-source-build> --output <directory>`
packs the evaluated iOS graph, including the SDKs and template. The resulting
22-package feed is an experimental iOS net11 feed, not a multi-platform release.
`consume-ios-template.py --feed <feed> --output <directory>` installs the actual
template into an isolated hive, uses a local-only Doroti package mapping, adds
external generic State/localization/action types, and publishes an iOS app.
It checks that no Doroti product ProjectReference entered the consumer graph.
`--resume <generated-source-directory>` can resume an interrupted publish.
Only disposable Doroti entries in its own NuGet cache are refreshed; immutable
third-party package versions are reused. Source ZIPs, logs and hashes are retained.
The signing fixture uses `dev.doroti.testbed`; installing it temporarily replaces
the Testbed, which must be restored afterward. This was done in this run.

The consumer passed native compilation, signing, installation, real rendering
and its external generic API checks; `ios-template-consumer/mono-evidence.json`
also passes Mono absence. The Runner sets PublishAot early in SDK props, before
Apple runtime selection; the generated iOS directory respects ArtifactsPath.

The Mono recovery profile targets .NET 10/MAUI 10. .NET 11 iOS explicitly rejects
Mono with NETSDK1242; it otherwise defaults to CoreCLR. The SDK now selects
UseMonoRuntime explicitly for the iOS Mono profile and rejects a silent CoreCLR
substitution. The earlier net11 comparison containing libcoreclr was marked
ineligible as a Mono baseline. Do not claim a controlled Mono size/performance
comparison from that artifact.

## Additional public API migration notes

Routes expose `IModalRoute`/`IPageRoute`; generic routes preserve result types
through `IPopEntry`. Router configuration/delegate factories preserve the
consumer's generic configuration and validate the parser type. Custom implementations
must use these contracts instead of arbitrary duck-typed objects.

Tree nodes implement `ITreeSliverNode`, and `TreeSliverNodesAnimation` is a data
record (`fromIndex`, `toIndex`, `value`). RenderTreeSliver animation maps use this
record. Draggable<T> anchor callbacks accept Draggable<T>; callers of custom
non-generic delegates need an explicit typed adapter.

`AwaitFutureOrValue<T>` now awaits the common Future result bridge before checking
the requested type. Heterogeneous compatible Future results are retained; an
incompatible result raises a cast error instead of being silently replaced with
default. `routing` and `inspector` have standalone native fixtures, and
`fcr7-material-widget --route-bridges` / `--drag-tree` cover mounted lifecycle/input.

Known limits: MAUI KVO observer finalizer warnings remain documented without
suppression, remote orientation control is unsupported by this iPhone, and
exhaustive physical IME/accessibility/lifetime and controlled performance checks
are not complete. The comparison plan is in `performance-plan.json`; it is not a
benchmark result. NativeAOT therefore remains explicit opt-in.

The actual .NET 10 Mono comparison also published successfully. Raw bundle
bytes are 104,701,336 versus NativeAOT 42,616,772 (59.30% reduction); compressed
IPA bytes are 37,351,329 versus 18,185,146. See `final-size-comparison.json`.
.NET/MAUI versions differ, and quantitative performance remains unverified.
The comparison Mono app was not installed over the final NativeAOT Testbed.
