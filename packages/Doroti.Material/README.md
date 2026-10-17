# Doroti.Material

Independent Material widgets. Reference `Doroti.Material` and explicitly import `Doroti.Material` together with `Doroti.Framework.Widgets`.

Source: reviewed Doroti C# ports of Flutter framework sources. Retained source comments identify the original conversion baseline; this relocation does not claim synchronization with latest upstream. Initial version: `1.0.0-alpha.1`; core validation baseline: `0.4.0-alpha.1`.

The package owns its localization and design behavior and depends on `Doroti.Icons.Material` for icons. GPU display, physical IME and locale coverage require platform-specific acceptance.

Material icon definitions, font registration, licensing and Release font subsetting are provided by [Doroti.Icons.Material](../Doroti.Icons.Material/README.md). Existing `Icons` calls retain their namespace. General Web fallback fonts have a separate provider policy.

`DefaultMaterialLocalizations.delegate` supports English. Apps with unsupported locales resolve through their declared supported locales (English by default), or provide their own delegates; the package does not claim a complete translated locale set. `WidgetsLocalizations` remains in the core.
