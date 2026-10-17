# Doroti.Cupertino

Independent Cupertino widgets. Reference `Doroti.Cupertino` and explicitly import `Doroti.Cupertino` together with `Doroti.Framework.Widgets`.

Source: reviewed Doroti C# ports of Flutter framework sources. Retained source comments identify the original conversion baseline; this relocation does not claim synchronization with latest upstream. Initial version: `1.0.0-alpha.1`; core validation baseline: `0.4.0-alpha.1`.

The package owns its localization and design behavior and depends on `Doroti.Icons.Cupertino` for icons. GPU display, physical IME and locale coverage require platform-specific acceptance.

Cupertino icon definitions, package-qualified font registration, licensing and Release font subsetting are provided by [Doroti.Icons.Cupertino](../Doroti.Icons.Cupertino/README.md). Existing `CupertinoIcons` calls retain their namespace.

`DefaultCupertinoLocalizations.delegate` supports English. Unsupported locales use the app's supported-locale resolution (English by default), or app-supplied delegates. This is independent of text font fallback and does not provide new translations. `WidgetsLocalizations` remains in the core.
