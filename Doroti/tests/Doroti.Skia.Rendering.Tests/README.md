# Skia text replay regressions

Run from the repository root:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Skia.Rendering.Tests/Doroti.Skia.Rendering.Tests.csproj -c Release -m:1 -nr:false
```

The production renderer is compared pixel by pixel with its original scalar
drawing path at 1, 2.75 and 4 times scale, with fractional origin/translation and
clipping. Cases include Hangul, surrogate pairs, combining marks, fallback fonts,
letter/word spacing, negative spacing and malformed UTF-16. Repeated cached draws
must allocate no managed objects; eviction and resource retirement must dispose
native blobs. The bounded cache retains at most 32 entries and 2,048 UTF-16 code
units per font/style resource; oversized text keeps the original drawing path.

Shadow picture checks compare the production direct and retained drawing paths
pixel by pixel, including paragraph text, nested transforms, translucent fills,
opaque/transparent occluders, clips, affine transforms and DPR changes. Fractional
translations must reuse the same vector recording, while changed scale/DPR and
font registration retire old native pictures. Perspective retains direct drawing.

Web hosts opt in to deferring new raster cache surfaces during active scrolling
and the final quiet interval. A virtual clock checks settled resumption and the
unchanged default policy for other hosts. Empty font variation keys/normalization
must avoid query-pipeline allocations while preserving nonempty axis semantics.

This uses native Skia CPU surfaces. It does not establish browser GPU appearance,
Android input latency, or physical display frame pacing.
