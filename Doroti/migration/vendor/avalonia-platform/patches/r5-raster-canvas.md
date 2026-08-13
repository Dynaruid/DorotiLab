# R5 raster-canvas adaptation

Source snapshot: `selected-content-sha256:75460dd7cb693dafaf5386a11fb878e222490e8a179e1a3f9d8b1283c10099cd`.

R5 keeps `FramebufferRenderTarget.cs` inside the previously approved selection closure and adds internal Skia primitive entrypoints for save/restore, matrix concatenation, clip, rectangle, polygonal path, immutable BGRA image and text drawing. `Doroti.Backends.Skia` adapts the Doroti-owned `IRasterCanvas` values to those primitives.

The vendor file does not reference `Doroti.Composition`, `Doroti.Rendering`, `Doroti.Widgets` or `Doroti.Engine`. It does not own DisplayList decoding, LayerTree traversal, frame ordering, mailbox replacement, resource identity, ACK state or recovery policy. Those meanings remain in the Doroti composition/rendering/engine assemblies.

No additional upstream file or transitive dependency was selected. The exact adapted file hash is authoritative in `selection.json`.
