# Doroti.Cupertino

Independent Cupertino widgets. Reference `Doroti.Cupertino` and explicitly import `Doroti.Cupertino` together with `Doroti.Framework.Widgets`.

The widgets are C# ports of Flutter framework sources. Source comments identify the upstream revision used by each port. Package version and compatible core range are declared in [Version.props](Version.props).

The package provides Cupertino styling and localization and depends on `Doroti.Icons.Cupertino` for icons. Rendering and native text input depend on the selected [platform host](../../Doroti/README.md#platform-support).

Cupertino icon definitions, package-qualified font registration, licensing and Release font subsetting are provided by [Doroti.Icons.Cupertino](../Doroti.Icons.Cupertino/README.md). Existing `CupertinoIcons` calls retain their namespace.

`DefaultCupertinoLocalizations.delegate` supports English. Unsupported locales use the app's supported-locale resolution (English by default), or app-supplied delegates. This is independent of text font fallback and does not provide new translations. `WidgetsLocalizations` remains in the core.
