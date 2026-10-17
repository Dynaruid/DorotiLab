# Doroti.Icons.Material

Material icon definitions, the original OpenType font, and its CC BY 4.0 notice.
Requires Doroti's framework widgets; it has no dependency on Material or Cupertino design widgets.

```csharp
using Doroti.Framework.Widgets;
using Doroti.Material;

new Icon(Icons.arrow_back, size: 24);
```

The existing `Doroti.Material.Icons` namespace and names are retained, with the definitions owned by this assembly.
`Doroti.Material` references this package automatically. An icons-only application can reference this package directly.
`Icon` registers the bundled face once per View on first use, before painting the glyph. No sample-specific font loader is needed.

Each definition is an independent getter, so .NET trimming can remove unused definitions.
Release executable builds and publishes also subset the embedded font using compiled IL references, including references from dependencies and adaptive icons.
Publish subsetting runs after ILLink and before ReadyToRun/AOT and single-file bundling; it does not enable whole-application trimming on unsupported hosts.
Debug keeps the complete font. Use `-p:DorotiIconTreeShaking=false` when loading icon definitions/codepoints dynamically, including reflection from external configuration or dynamically loaded assemblies.
Recognized runtime `IconData` construction and icon type reflection preserve the complete face with `DOROTIICON001`.
Unknown/unsupported font structures fail the build with `DOROTIICON002`, rather than producing a damaged font.

The output's `doroti-icon-tree-shaking.json` records retained Unicode codepoints and font sizes.
Without .NET trimming, the font still shrinks, while unused managed getters can remain.
The subset compacts glyph IDs and metrics, remaps composite references, and preserves selected outlines, font identity, and hinting. Unicode codepoints remain unchanged. Text ligatures/contextual shaping are outside the icon API and are removed.
The build tool is bundled in the NuGet package and requires the .NET 10 SDK, with no Python or native font-tool prerequisite.
