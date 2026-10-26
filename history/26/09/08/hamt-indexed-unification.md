# HAMT + indexed single implementation — 2026-09-08

The user explicitly selected HAMT + indexed as the foundation and requested removal
of alternatives after reviewing the combined measurements. This supersedes the
earlier recommendation to keep the Dictionary baseline. It does not change any
historical FAIL, PARTIAL or notVerified result.

## Implementation

- Foundation `PersistentHashMap` always uses immutable bitmap HAMT path copying.
  Removed the Dictionary copy implementation, experimental compilation flag and
  unused placeholder trie types/helpers. No pooling or snapshot eviction.
- Material sample always uses `SectionList` with `SectionExtentIndex`. Removed
  eager and standard SliverList gallery branches and viewport selection. Retained
  stable section owners, visited State, font invalidation, focus coordination,
  scroll anchors and the parked right column. Other widgets still use the shared
  SliverList normally.
- Removed viewport query forwarding and environment configuration from Web boot,
  validation helpers and probes. No HAMT build property is required.
- Updated Korean/English usage and active contract documentation. Historical
  sources, frozen A/B assets and measured results remain as evidence.

## Evidence and limits

The preceding [combined comparison](indexed-hamt-comparison.md) measured map
allocations of 20,400 → 5,760 bytes during the column-transition segment. It did
not demonstrate reduced total live heap/process memory, consistently faster
processing, physical FPS or the original latency goals. This cleanup is not a
new performance qualification.

Seven runtime/build/type-check invocations used a 20-minute timeout, no retries
and one GPU workload at a time. The browser invocation contained two sequential
tests. A separate static comparison checked the saved traces. No new repeated
performance corpus was needed for this removal of alternatives.

| Check | Result | Evidence |
| --- | --- | --- |
| Default HAMT snapshots, collisions, key/value identity, removal, lifetime; five-tick trace | PASS | `Doroti/artifacts/direct-default/unified-map.log` |
| Preserved Dictionary trace and pinned Flutter callbacks | PASS: 20 nodes / 155 events match; Flutter callback sequence matches | `.doroti/wasm-same-work/unified-contract-comparison.json`; full Flutter internal trace notVerified |
| Default indexed materialization, wheel, all 29 visited states, measured extent, end access, width/text-scale reflow | PASS | `Doroti/artifacts/direct-default/unified-scroll.log` |
| Web validation TypeScript | PASS | `Doroti/artifacts/direct-default/unified-web-types.log` |
| Section extent index, forward/reverse focus, queued Tab, dynamic reorder/add/remove | PASS | `Doroti/artifacts/direct-default/unified-section.log` |
| Deep anchors and retained section identity at widths 800/1280/800/1001/1000/390/1501/800 | PASS | `Doroti/artifacts/direct-default/unified-columns.log` |
| Web Release build without feature properties | PASS: 0 warnings / 0 errors | `Doroti/artifacts/direct-default/unified-web-build.log` |
| Browser wide → narrow → wide: right column pixels and upward wheel response | PASS | `Doroti/artifacts/direct-default/unified-browser.log`; `web-playwright/artifacts/unified-hamt-indexed/test-results/.../returned-1.png` visually inspected |
| Browser parked right section: narrow theme change, demand materialization, checkbox value and return | PASS | Same browser log: 2 tests passed, runtime errors 0 |

Browser validation used Chromium with the hardware project, DPR 1 and the default
Web renderer. Physical display/input, other host executions, additional DPRs and
full Flutter target equivalence remain notVerified for this change.

Current preview: `http://127.0.0.1:5189/?dorotiTestbedMode=sample`.
The old `dorotiSectionViewport` query is ignored; no selector is needed.
The preview serves `.doroti/wasm-same-work/unified-wwwroot`, frozen from 562
evaluated endpoints with SHA-256 records in `unified-wwwroot-manifest.json`.
HTTP 200 was verified. The task-owned Dictionary comparison server on 5188 was
stopped; the earlier frozen assets and the unrelated existing 5088 server remain.
