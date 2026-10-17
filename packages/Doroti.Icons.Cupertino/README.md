# Doroti.Icons.Cupertino

Cupertino icon definitions, the original TrueType font, and its MIT license.
Requires Doroti's framework widgets; it has no dependency on Material or Cupertino design widgets.

```csharp
using Doroti.Framework.Widgets;
using Doroti.Cupertino;

new Icon(CupertinoIcons.heart, size: 24);
```

The existing `Doroti.Cupertino.CupertinoIcons` namespace and names are retained, with the definitions owned by this assembly.
`Doroti.Cupertino` references this package automatically. An icons-only application can reference this package directly.
`Icon` registers the font as `packages/cupertino_icons/CupertinoIcons` once per View before painting its first glyph.

Each definition is an independent getter. Release executable builds/publishes subset the font from compiled IL references, independently of .NET code trimming.
Composite glyph dependencies are retained. Debug keeps the full font.
Use `-p:DorotiIconTreeShaking=false` for icons selected through externally supplied reflection or dynamically loaded assemblies.
Recognized dynamic `IconData` construction/reflection preserves the full face and reports `DOROTIICON001`.
`doroti-icon-tree-shaking.json` records the result. See [Material icon build behavior](../Doroti.Icons.Material/README.md) for ordering, failure behavior, and the distinction between managed-code trimming and font subsetting.
