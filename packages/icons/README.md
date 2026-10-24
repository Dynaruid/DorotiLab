# Doroti icon packages

`Doroti.Icons.Material` and `Doroti.Icons.Cupertino` own their icon definitions, bundled fonts and licenses.
Their only runtime package dependency is `Doroti.Framework.Widgets` (core `0.4.1-beta` or later within `0.4.x`).
The design packages reference the corresponding icon package; icons-only applications can reference an icon package directly.
Existing namespaces and icon names remain `Doroti.Material.Icons` and `Doroti.Cupertino.CupertinoIcons`.

The framework owns `Icon`, `IconData`, `IconTheme`, and `IconFont`. Each face registers once per View when an `Icon` first uses it.
An asynchronous font host reserves the icon's layout and waits for registration before painting the glyph.
Definitions are independently trimmable getters, with descriptors cached only when requested, rather than one initializer allocating every icon.
The samples no longer embed or initialize icon fonts themselves.

Release executable builds and publishes enable `DorotiIconTreeShaking` by default. The bundled managed build tool reads compiled IL references from the application and dependencies, follows aliases/adaptive icons, and derives compact OpenType fonts with the same Unicode codepoints.
It remaps glyph IDs, TrueType composite references, metrics and CFF names/outlines together. Unsupported formats fail closed.
Source/NuGet package assemblies and source fonts are left intact; only application output or an intermediate publish staging directory is rewritten.
Publishing subsets after ILLink and before ReadyToRun/AOT/bundling, retaining SDK post-processing metadata.
WebCIL caches are invalidated when the policy, managed inputs or build tool changes, including changes with old timestamps.

Font subsetting and .NET managed-code trimming are independent. The latter requires the application's supported `PublishTrimmed` configuration; this feature does not enable trimming globally on hosts that disallow it.
Known dynamic `IconData` construction, icon type reflection and preservation attributes retain the complete font with `DOROTIICON001`.
Use `-p:DorotiIconTreeShaking=false` for reflection selected through external configuration, dynamically loaded code, or direct text shaping with the icon face.
Debug defaults to complete fonts for development/Hot Reload.

The output's `doroti-icon-tree-shaking.json` records retained codepoints and font sizes.
