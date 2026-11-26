# Doroti widget translations

Add `Doroti.Localizations.Widgets`, `Doroti.Localizations.Cupertino`, or `Doroti.Localizations.Material` to the app project. Material brings in Cupertino and Widgets resources. Cupertino brings in Widgets resources. An app selects its own `supportedLocales` and delegate order.

| Design | Languages | Locale tags |
| --- | ---: | ---: |
| Widgets | 82 | 122 |
| Cupertino | 81 | 121 |
| Material | 82 | 122 |

Each package exposes its catalog through `SupportedLocales`. The catalog includes regional variants, Serbian Latin, and explicit `zh-Hans` and `zh-Hant` resources. Pashto is available in Widgets and Material. Apps that use both design systems can select the common locales through `GlobalCupertinoLocalizations.SupportedLocales`.

```csharp
using Doroti.Localizations.Material;
using Doroti.Material;

var app = new MaterialApp(
    supportedLocales: GlobalMaterialLocalizations.SupportedLocales,
    localizationsDelegates: GlobalMaterialLocalizations.Delegates,
    home: home);
```

Put an app delegate before the package delegates to override the same resource type. Use `GlobalCupertinoLocalizations.Delegates` in a `CupertinoApp` or `[GlobalWidgetsLocalizations.Delegate]` in a `WidgetsApp`. Core apps can continue using their built-in English delegates.

Material uses Gregorian dates, .NET week and number data, and the pinned time layout for the resolved widget locale. Its forced 24-hour mode pads hours and minutes. Korean date input uses `yyyy. MM. dd.` with literal dots and spaces. Cupertino uses the pinned date-field order and .NET month names and day periods. Date layout, spacing, digits and day-period text depend on the runtime and ICU profile. Widgets provides text direction from the resource and an explicit script where supplied.

Set `BlazorWebAssemblyLoadAllGlobalizationData=true` in a Blazor runner that offers the full catalog. A narrower app can select a shard after validating all its UI and business cultures at startup and after a locale switch. The P0 probe qualified `icudt_CJK.dat` for an English-to-Korean switch in one SDK 10 compatible Debug profile. [P0 contract](p0-contract.md)

For a repository source build, set `DorotiWidgetLocales` to a comma- or semicolon-separated list of exact tags shared by the selected packages. The generator always includes the English template. `-p:DorotiWidgetLocales=ko` embeds `en` and `ko` instead of the full catalog. Setting `supportedLocales` controls negotiation at runtime. A prebuilt NuGet package contains its published catalog.

The packages contain static C# resources and ship the Flutter license and provenance. Text shaping, caret movement and font coverage for complex scripts follow the renderer's qualification. [Translation provenance](widget-provenance.json) · [Paragraph feasibility](paragraph-feasibility.md)
