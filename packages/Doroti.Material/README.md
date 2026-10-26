# Doroti.Material

Independent Material widgets. Reference `Doroti.Material` and explicitly import `Doroti.Material` together with `Doroti.Framework.Widgets`.

The widgets are C# ports of Flutter framework sources. Source comments identify the upstream revision used by each port. Package version and compatible core range are declared in [Version.props](Version.props).

The package provides Material styling and localization and depends on `Doroti.Icons.Material` for icons. Rendering and native text input depend on the selected [platform host](../../Doroti/README.md#platform-support).

Material icon definitions, font registration, licensing and Release font subsetting are provided by [Doroti.Icons.Material](../Doroti.Icons.Material/README.md). Existing `Icons` calls retain their namespace. General Web fallback fonts have a separate provider policy.

`AppBar.systemOverlayStyle` takes precedence over `AppBarThemeData.systemOverlayStyle`,
then falls back to brightness inferred from the AppBar background. The AppBar
creates its own `AnnotatedRegion<SystemUiOverlayStyle>`. See the MAUI provider's
[system bar examples and platform limits](../platforms/maui/README.md#system-bars).

`DefaultMaterialLocalizations.delegate` supports English. Apps with unsupported locales resolve through their declared supported locales (English by default), or provide their own delegates; the package does not claim a complete translated locale set. `WidgetsLocalizations` remains in the core.
