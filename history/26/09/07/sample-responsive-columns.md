# Material sample responsive columns

The user reported that shrinking the Web window showed two columns, as if the
wide breakpoint were inverted. The breakpoint itself was correct (`width >
1000`, in logical pixels). `ComponentsScreen` partitioned the gallery using that
boolean but displayed its second column using the navigation animation. On grow,
the first column was already truncated while the second still had zero width.
On shrink, the first list already contained all sections while the second list
remained mounted. The same GlobalKey sections could therefore occur in both
lists during the transition. Slow Web construction made the mismatch more visible.

The shared sample now partitions and displays its columns from the same
`TwoColumns` value. Both wide columns have equal flex. The column slide/width
animation is removed; navigation rail/bar animation is retained. This source is
shared by all sample hosts. No Web or platform breakpoint is inverted or patched.

## Evidence

- Before: native settled states passed, but checking the transition's first
  frame failed at width 1280, fraction 0 (`.doroti/columns-transition-before.err`).
- Initial browser geometry investigation: 1 PASS/1 FAIL (`columns-before`), then
  2 FAIL with matching front-generation waits (`columns-before-settled`). Those
  initial checks used approximate button coordinates; the final tests use actual
  gallery group widths, both column positions, and second-column presence.
- Temporary Web tracing showed width 1280/forward/value 0 before the completed
  state. Diagnostic logging has been removed. The diagnostic test run
  `columns-debug` had 2 FAIL, including an overly strict phone button-position
  assumption; it is not final validation.
- Corrected Web direct suite: 4 PASS (`columns-after-corrected`), cold narrow and
  cold wide, DPR 1/2, eight viewport states per case (390, 800, 1000, 1001, 1280,
  1501 including repeats). Captured 1280 and 800 images were visually inspected.
- Windows Release build: 0 warnings/errors (`.doroti/columns-windows-build.log`).
- TypeScript check: PASS.
- Native with current resize epochs: 1,040 intermediate frames PASS
  (`.doroti/columns-native-epoch.log`). At every frame the mounted second scroll
  controller matches the current breakpoint; navigation still reaches its
  expected completed/dismissed endpoint.

The first existing scroll-return regression run had 1 PASS/2 FAIL
(`columns-state-regression`). Its fixed 1.5-second sleep captured the old 800px
front inside the newly enlarged 1280px canvas. The saved image explicitly shows
the old bottom bar and narrow app-bar actions, rather than a completed wide
layout missing its right column. The test now awaits presentation of the new
resize generation and polls the same right-column pixel criterion. This is a
readiness correction, not a claim that resize latency is fixed.

Final scroll-return/state regression: 3 PASS (`columns-state-presented`),
including two deep-scroll wide/narrow/wide cycles at DPR 1 and DPR 2 and selected
state preservation through resize/theme changes. Together with the four column
cases, the current Web direct checks total 7 PASS. Final native epoch test and
TypeScript checks also passed. CanvasKit and physical platform runs were not
repeated for this sample-only change.

The first native fixture attempted to paint a resized scene into its fixed-size
surface and failed (`columns-native-before`). The column contract now observes
mounted widgets/controllers and delivers only requested host frames; it does not
claim raster acceptance. Its resize metrics and scene epochs advance together.
An intermediate collection-expression syntax error failed both native and Web
builds (`columns-native-after`, `columns-after`); it was corrected before the
passing builds above. All test wrappers use a 20-minute timeout.

Physical window/device acceptance and quantitative resize performance are not
qualified by these checks. Existing rendering fixes from the preceding task are
preserved separately.
