# Doroti.Material

Independent Material widgets. Reference `Doroti.Material` and explicitly import `Doroti.Material` together with `Doroti.Framework.Widgets`.

Source: reviewed Doroti C# ports of Flutter framework sources. Retained source comments identify the original conversion baseline; this relocation does not claim synchronization with latest upstream. Initial version: `1.0.0-alpha.1`; core validation baseline: `0.4.0-alpha.1`.

The package owns its localization, icons and design behavior. Applications supply and register icon fonts. GPU display, physical IME and locale coverage require platform-specific acceptance.

Material icons use the `MaterialIcons` font family with no package key. Register the font with `Doroti.Ui.DorotiUiLibrary.loadFontFromList(bytes, fontFamily: "MaterialIcons")`, where bytes is a `Doroti.Runtime.Uint8List`, before showing icons. The repository testbed supplies `MaterialIcons-Regular.otf` and its CC BY 4.0 notice; the core and this design package do not automatically install an icon font. General Web fallback fonts have a separate provider policy.

`DefaultMaterialLocalizations.delegate` supports English. Apps with unsupported locales resolve through their declared supported locales (English by default), or provide their own delegates; the package does not claim a complete translated locale set. `WidgetsLocalizations` remains in the core.
