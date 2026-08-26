# I0 pointer signal and scrolling contract

I0 makes `RawPointerEvent.ScrollDelta` a logical-pixel value whose positive main-axis direction
moves content toward the trailing scroll extent. Platform hosts own conversion before the event
reaches `InputDispatcher`: Windows applies `WHEEL_DELTA` and the current system lines-per-scroll
setting, while X11/Wayland follows Flutter's 53-pixel convention and preserves fractional values.
No widget applies a wheel multiplier. F2 removes the unused `wheelScrollExtent`
constructor/property shape, so the platform-normalized logical-pixel delta is the only input value.

`InputDispatcher` creates one `PointerScrollEvent` and visits the hit-test path deepest first.
Each movable scroll target may register with `PointerSignalResolver`; only the first registration is
called after routing. A clamped inner scrollable does not register, allowing the nearest movable
ancestor to consume the signal. The original timestamp and fractional delta remain unchanged.

`SliverScrollController` owns a `ScrollPosition`. The position owns content/viewport metrics,
`ScrollPhysics`, the current idle/drag/ballistic `ScrollActivity`, boundary handling, pointer scroll,
focus reveal, and a deterministic clamping simulation. Interactive hosts bind their single frame
dispatcher during input routing so drag-end ballistic activity advances on the presentation clock.
Wheel, trackpad, drag, Page Up/Down, arrows, Home/End, semantics actions, and item reveal all mutate
the same position.

`RenderSliverFixedExtentViewport` records its actual bounded layout constraint in that position. Offset-only
changes update mounted child parent-data offsets and dirty paint, keeping hit-test and semantics
bounds aligned without layout. `VirtualListViewState` rebuilds only when the visible/cache index
range escapes the currently mounted range; application ancestors do not rebuild. This keeps the
1,000-item path virtualized while avoiding a rebuild/layout for each fractional signal.

G5-1 adds an actual HWND target-controller run for Windows wheel normalization, coordinate tolerance,
capture loss and sustained frame pacing. It remains synthetic-source evidence. Physical Windows
mouse/touchpad, physical touch, X11/Wayland and macOS trackpad traces remain target-machine evidence;
an available device or injected native message is never promoted to a physical-device PASS.
