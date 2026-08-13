# ADR-013: Immutable paragraph snapshot

## Decision

`IParagraphLayout` runs on the UI thread and returns one `ImmutableParagraphSnapshot` containing text, size and line metrics. `RenderParagraph` stores that snapshot and uses the same instance for size, paint commands and offset-to-text hit testing until text, style or constraints invalidate layout.

Backends may shape and rasterize the recorded runs later, but backend raw-string measurement is not a layout authority. A future shaping port may enrich the snapshot with glyph runs without changing RenderObject ownership or the immutable commit boundary.
