# R6 RenderObject and Compiler C1

R6 adds a backend-neutral, headless RenderObject tree to `Doroti.Rendering`. `BoxConstraints`, `RenderObject`, `RenderBox`, `RenderView` and `PipelineOwner` enforce ownership, normalized constraints, finite sizes, depth-ordered dirty work, phase recovery and immutable commit. `PaintingContext` records DisplayLists and builds reusable Layers. DPI scaling is applied at the RenderView-to-LayerTree commit boundary.

The basic render set is `RenderColoredBox`, `RenderPadding`, `RenderPositionedBox`, `RenderFlex`, `RenderStack` and `RenderParagraph`. Transform-aware hit paths and `localToGlobal`/`globalToLocal` reject singular transforms explicitly. Paragraph layout, paint and hit testing share one `ImmutableParagraphSnapshot`.

`migration/behavior/r6-box-constraints.json` is executed by the pinned Dart reference runner and `Doroti.BehaviorRunner`; their versioned delta must be empty. `migration/goldens/r6-render-tree.json` pins framebuffer dimensions at DPR 1.0 and 1.5; tests require visible multi-color output without an exact pixel hash. `Doroti.R6.Tests` proves invalid relationship/size/phase failures, dirty recovery, unchanged layout/paint counts, transforms, basic boxes and paragraph snapshot identity.

Compiler C1 accepts named/optional/required parameters, constructor field formals, generics, inheritance, collection literals and for/if control flow used by `c1_render_fixture`. It binds generated calls only to `Doroti.FlutterCompat.BoxConstraints` and `RenderFixture`, builds outside the product solution with an explicit SDK-root input, and executes the generated layout/paint fixture without source patches.

`Doroti.Samples.RenderTree --smoke --verify-target` exercises the real Windows WGL/OpenGL path and writes `artifacts/r6/runtime-report.json`. `IPixelReadableSurfaceFrame` copies the raster-thread frame as top-down premultiplied BGRA8888 without exposing Skia types. The sample renders the same non-text snapshot through GPU and managed software, then checks channel tolerance 2 and a 0.05% maximum outlier ratio. It also uses `IWindowPlacementController` to move the window across every connected display and requires at least two observed DPI scales and a scale-changing metrics event.

The 2026-08-01 target run used AMD Radeon 780M OpenGL 4.6 at 1280x720. Its maximum channel delta was 1, mean delta was 0.0175, and zero of 3,686,400 channels exceeded tolerance. Two connected displays produced 125% and 200% scale with two DPI-change events, so every runtime verification is `pass`.
