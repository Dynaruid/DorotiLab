# Indexed section viewport experiment

`work.md` owns promotion gates. The existing eager gallery remains the default
until the quantitative, memory, focus and platform gates are qualified.

The shared implementation is `SectionExtentIndex` in Rendering and `SectionList`
in Widgets. It reuses `SliverMultiBoxAdaptorElement`, RenderBox layout, existing
sliver painting/hit testing and the standard keep-alive bucket. It does not
serialize Material State or interrupt an active build/layout flush.

## Ownership and geometry

- One index belongs to one list configuration, with 1–4096 sections in stable
  delegate order. Stable delegate keys identify sections across reorder; changing
  the item set requires a corresponding index and explicit anchor mapping.
- Prefix sums and offset lookup use a Fenwick tree. Search/measurement is O(log N).
  Visible children use normal exact layout. Unseen heights remain estimates;
  the total extent is provisional until those sections are measured.
- Each index stores at most two exact cross-axis widths and metric revisions.
  Numeric storage is at most `2 * (20 * count + 8)` bytes, excluding object/array
  headers and shared revision references. A width is never rounded for reuse.
  The sample's 29 / 12 / 17 partition indices therefore retain at most 2,368
  numeric bytes in total; at most 29 visited section subtrees remain pinned.
- `metricRevision` must be immutable and include metrics such as text scale,
  font generation, locale/direction and content metrics. `Invalidate(item)` marks
  content-derived measurements stale across both configurations. Actual layout
  always rechecks visible children, including dirty children at equal constraints.
- `SectionList.RequestItem(scroll, index, item, offset)` cancels an old scroll
  activity and requests target materialization. Direct `index.RequestItem` leaves
  scroll-activity ownership to the caller. End can seek the provisional extent
  and lay out the last section without building intermediate sections.
- The sample preserves the primary (left) section anchor when changing column
  count. When it belongs to the new right partition, it is restored there. The
  other column restores its own saved section anchor. Same-width cache restores
  also correct by section and internal offset, not old absolute pixels.
- The indexed sample retains the right scroll owner after its first use, keeping
  its last finite width while single-column mode hides it. Its controller remains
  attached. Whole-section GlobalKeys move sections to the visible owner on demand;
  unseen right sections stay with their existing owner. Returning to two columns
  re-adopts visited right sections before tree finalization. This adds 29 owner
  keys and one retained scroll-owner subtree, without duplicating section State.
  All visited Material subtrees remain pinned until the screen is disposed;
  controllers/text selection/focus/overlays are not reconstructed from copied values.
- `suspended` is only for an explicitly offstage owner. It processes keep-alive
  releases but skips child materialization and measurement. The caller must
  exclude paint, semantics and focus and disable hidden tickers; the sample uses
  Offstage, ExcludeFocus and TickerMode. `restoreRetainedChildren` explicitly
  re-adopts retained children when ownership returns from another viewport.
- The experimental sample uses zero speculative cache extent. Unvisited offscreen
  preparation is therefore zero; no timer or async yield disguises synchronous
  construction cost. Long individual sections and visited-state transfer can still
  exceed the frame budget.

`SectionFocusCoordinator` connects section-boundary Tab/Shift+Tab to materialization,
uses ordinary reading-order traversal inside a section, and preserves queued Tab
input while mounting the next section. Pointer input cancels pending traversal.
The mounted contract sends rapid forward/reverse key events across an initially
unmounted boundary. Releasing a cached child's `KeepAlive` disposes it on the next
layout through an explicit release notification, without scanning all retained
children on steady frames. The sample intentionally pins its finite set of visited
Material subtrees; it does not claim arbitrary State can be serialized and evicted.

Screen-reader exploration of unknown descendants, physical IME/overlay migration,
and automatic item-set reconciliation are not yet qualified. Explicit owner-mapped
reorder/add/remove is covered by a mounted contract: surviving State and anchor,
deleted-child disposal and lazy creation of the new item. Metric invalidation in
the sample conservatively includes all ThemeData changes; color-only reuse is not
optimized. These remaining contracts keep the experiment behind an explicit switch.

## Reproduction

| Resource | Owner / lifetime |
| --- | --- |
| Section descriptors and GlobalKeys | ComponentsState; stable while gallery is mounted |
| Text/scroll controllers and selected values | ComponentsState; disposed with the gallery |
| Internal Material State, selection, overlay anchors | Original section subtree; pinned once visited |
| Section FocusNodes and queued traversal | SectionFocusCoordinator; cancelled on pointer input, disposed with gallery |
| Height arrays | Partition index; two exact configurations per index, discarded with gallery |
| Font listener | Registered in initState, removed in dispose |
| Cached RenderBox | Standard keep-alive owner; explicit unpin releases on next layout |

Do not reuse one index concurrently in independent lists. Dynamic item replacement
requires a fresh index, a stable-key delegate mapping and an explicit surviving
anchor mapping supplied by the owner (the mounted contract exercises this);
automatic reconciliation is outside this
experimental contract. Keep arbitrary stateful sections pinned until an owner can
prove their restoration/disposal contract. Focus traversal tests do not qualify
physical IME, open overlay migration or screen-reader behavior.

All test/benchmark commands require a 20-minute outer timeout, retry 0 and one GPU
workload at a time. Native contracts:

```powershell
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --section-index
$env:DOROTI_SAMPLE_SECTION_VIEWPORT = 'indexed'
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --sample-columns
$env:DOROTI_VALIDATION_SECTION_COLUMNS_DEEP = '1'
dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --sample-columns
```

The Web comparison URL is `?dorotiTestbedMode=sample&dorotiSectionViewport=indexed`.
`eager` selects the reference path; `sliver-list` is the rejected standard-list
spike retained only for reproducing its State-loss contract failure.
For existing Playwright tests set `DOROTI_SAMPLE_SECTION_VIEWPORT=indexed`.
For performance scripts set `DOROTI_PERF_QUERY=&dorotiSectionViewport=indexed`.

`freeze-build.py` consumes the full evaluated `staticwebassets.build.json` and
copies uncompressed endpoints, including referenced `_content` assets and their
fingerprinted routes, into an isolated directory with SHA-256 metadata. Copying
the app's `bin/.../wwwroot` alone is insufficient for a static build server.
`summarize-wasm-structure.mjs` reports each independent run separately, preserving
allocation/GC counts, superseded resize targets and boundary-inclusive gaps.

한국어: 기본 경로 승격은 `work.md`의 기능·성능·메모리 gate에 따른다. 위 옵션은
개발용 비교 경로이며, 자동 검사 통과가 실제 기기 표시·입력·IME·접근성 수용을
뜻하지 않는다. 원본 실패와 timeout은 성능 0ms나 PASS로 집계하지 않는다.
