# ADR: application root, view ownership and staged platform preparation

Status: accepted implementation direction; M0 implementation in progress. M1–M3 are pending.
Authority: the former repository-root `work2.md` ([archived structure and task summary](../../../../history/26-10-05/work2-summary.md)), implementation request on 2026-10-05.
Baseline: [baseline.json](baseline.json), [provider-files.json](provider-files.json), [boundaries.json](boundaries.json).

## Implemented common boundaries

- `IDorotiPlatformProvider.PrepareProcessAsync(DorotiProcessContext, CancellationToken)` obtains the process lease without invoking app code.
- `IDorotiPreparedPlatform` owns the application dispatcher and creates native/view leases through a limited `IDorotiCapabilityRegistrar`.
- `DorotiLaunchPlan` stores a typed app factory. `DorotiPlatformApplication` invokes it after ProcessPrepared, creates one `DorotiHostSession`, seals view registrations through RegisterView, and attaches branches on the explicit application owner.
- The native factory receives configuration, application/view generation and Ui dispatcher, and never receives a Widget, entrypoint or framework root.
- ProcessPrepared, ViewAttached and FirstFrameSubmitted are distinct states. A frame marker validates the attached view generation; it is not presentation or GPU completion evidence.
- `DorotiCapabilityLifetime` owns shared services. Views register those services as Borrowed, and view-owned implementations as Owned. Conflicting owners, post-seal registrations and post-lifetime borrowing fail. Typed invocation leases prevent disposal while calls are active.
- Shutdown unmounts framework branches, drains native consumers, then releases capability/view/application/process leases. Drain failure retains the resource-bearing scopes and permits StopAsync retry.
- `DorotiWidgetEntrypoint(Func<DorotiApplicationViews, Widget>)` creates one application root. Put shared State/InheritedWidget above `DorotiApplicationViewCollection`; each keyed View branch owns a PipelineOwner/RenderView and local focus scope.
- Build, layout, paint, semantics and scene submission enter an explicit view invocation scope. Frame submissions retain that view's epoch even when another view triggers the application frame.
- Channel listeners belong to the application dispatcher; outbound operations require an explicit live view when multiple views exist. Image/font loading no longer chooses the first view.
- Explicit view invocation scopes also carry platform environment and view cancellation; focus listeners re-enter their own branch scope. Decoded image caches belong to a view and are closed after branch unmount.
- `ViewLocal<T>` and connection-captured TextInput ownership isolate typed IME clients. A previous view's close/late edit cannot clear or edit the survivor.

## Remaining gates

The headless/CPU regressions exercise real shared State/InheritedWidget, RenderViews, EditableText focus transfer/selection, typed IME callbacks, per-view caches, staged Configure/registration/Bootstrap/Attach failures, reentrant Stop and typed owner dispatch. Off-owner synchronous framework scope access fails; the typed asynchronous callback returns owner completion/error/cancellation.

Scene submission now carries a closed frozen command snapshot. Images and submitted CPU/native texture storage are retained independently of producer/registration disposal. The renderer keeps one latest pending scene and at most two unfinished raster consumers, retires replaced/rejected snapshots, preserves the newer replay under late completions, and retains a snapshot in Graphite's RecordingFrame until its actual GPU completion fence. Application/view incarnation is separate from resize metrics; old frames fail after view ID reuse or application restart. CPU probes and build checks do not establish native GPU/window acceptance. External browser/Android producer adapters still require snapshot migration in M3.

This remains partial G0: Window service/native-root separation, remaining startup/race/transport coverage and role-specific/independent consumption checks are pending. Maintained entrypoints are `platform_bootstrap_contract.py` and `frame_submission_contract.py`, registered in Build/Packages/Developer/Release. Successful runs retain JSON summaries and remove their own raw artifacts.

Windows now has an explicit application owner and one shared tree/session across native loops. Native shared-tree, five-kind, menu, design presentation and independent prebuilt NuGet probes passed within their recorded scopes. Qt uses one shared session and GUI owner; its latest strict native rerun is tracked in the [resumption record](resume-2026-10-05.md). MAUI prepares its process before app construction and waits actual startup work on final exit; Windows MAUI native controls/WebView/picker/cancellation/clean close passed. These results do not establish matching Apple/mobile, physical input or full G1 acceptance.

Do not move every provider or promote mock/CPU success to native Windowing acceptance. Preserve the existing Windows native-loop, renderer admission and GPU retirement policies while implementing the explicit application owner and immutable frame bridge. Qt retains its single GUI loop. Retained GPU resources must not be forcibly disposed on timeout.

Design identities, shaders, independent NuGet versions, tooling Contracts/Extension.Sdk, schema v2, five WindowKind implementations, design/native presentation, OS menus, other-provider migration and release receipts remain work in progress or pending. This ADR does not change their acceptance criteria.
