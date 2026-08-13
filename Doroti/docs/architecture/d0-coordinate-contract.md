# D0 logical/physical coordinate contract

`WindowMetrics` is the single window-size snapshot. It carries logical client size, the native
integer physical client extent, scale factor, event generation, scale generation, and surface
generation. Logical layout, hit testing, semantics, and text caret geometry never consume the
physical extent. Raster surfaces consume the physical extent directly and must not derive it again
from logical size.

`PixelExtentPolicy` is the only logical-edge conversion policy. Left/top edges use floor and
right/bottom edges use ceiling. An origin-based client extent is therefore the trailing-edge ceil.
This keeps fractional logical rectangles fully covered without mixing per-call `Round`, `Floor`,
and `Ceiling` behavior. Win32 resize, UI Automation bounds, IME caret placement, render surface
allocation, comparison-host capture, and runtime reporting use this policy or an authoritative
native pixel extent.

`RenderViewConfiguration` records both coordinate spaces but applies `DevicePixelRatio` exactly
once as the root layer transform. The committed frame also records physical extent and metrics
surface generation. Window-backed surface frames capture the same generation at `BeginFrame` and
check it again at `Present`; the raster compositor returns a terminal stale ACK when either the
generation or physical extent differs. This covers logical resize and DPI races even when two
logical sizes happen to map to the same integer pixel extent.

`VirtualListView` no longer requires callers to know the viewport. Its preferred constructor takes
an optional `viewportExtentHint`; F2's `RenderSliverFixedExtentViewport` writes the actual bounded layout height
to `SliverScrollController`, and the child builder uses that measured value on the next build. The old
positional constructor remains as a source-compatibility adapter whose value is treated only as a
hint.

The cross-platform D0 suite covers 100%, 125%, 150%, 175%, and 200% fractional edge rounding,
single root scaling, authoritative native extents, same-pixel generation invalidation, compositor
stale ACK, and constraint-derived virtual-list children. Native Windows capture/readback and live
open/resize/monitor/maximize scenarios remain target-machine acceptance checks.
