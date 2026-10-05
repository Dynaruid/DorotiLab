# Doroti.Cupertino

Independent Cupertino widgets. Reference `Doroti.Cupertino` and explicitly import `Doroti.Cupertino` together with `Doroti.Framework.Widgets`.

Source: reviewed Doroti C# ports of Flutter framework sources. Retained source comments identify the original conversion baseline; this relocation does not claim synchronization with latest upstream. Initial version: `1.0.0-alpha.1`; core validation baseline: `0.4.0-alpha.1`.

The package owns its localization, icons and design behavior. Applications supply and register icon fonts. GPU display, physical IME and locale coverage require platform-specific acceptance.

Cupertino icons use family `CupertinoIcons` and package key `cupertino_icons`. Register their font with `Doroti.Ui.DorotiUiLibrary.loadFontFromList(bytes, fontFamily: "packages/cupertino_icons/CupertinoIcons")`, where bytes is a `Doroti.Runtime.Uint8List`. The repository sample supplies `CupertinoIcons.ttf` and its MIT license. The core and this design package do not install an icon font automatically.

`DefaultCupertinoLocalizations.delegate` supports English. Unsupported locales use the app's supported-locale resolution (English by default), or app-supplied delegates. This is independent of text font fallback and does not provide new translations. `WidgetsLocalizations` remains in the core.
