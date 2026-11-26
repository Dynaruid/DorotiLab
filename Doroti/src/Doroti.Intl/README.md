# Doroti.Intl

`Doroti.Intl` provides explicit .NET formatting and CLDR 49 cardinal rules. It accepts normalized language, script and region tags. Neutral `en` and `ko` select `en-US` and `ko-KR`. The Tagalog profile uses `fil-PH` for formatting. Other neutral tags use the runtime's specific culture. Applications register widget delegates through the opt-in [localization packages](../../docs/localization/widget-packages.md).

```csharp
var format = new Doroti.Intl.IntlFormatter("ko-KR");
string date = format.FormatDate(new DateTime(2024, 2, 29), Doroti.Intl.WidgetDateFormat.Compact);
bool valid = format.TryParseDate(date, out DateTime parsed);
string price = format.FormatCurrency(1234.5m); // Culture default currency: KRW, .NET rounding.
```

Culture objects are read-only and use `useUserOverride:false`. Widget dates use Gregorian years 1–9999, canonical four-digit-year input and `DateTimeStyles.None`. Number formatting accepts `long` and `decimal` with up to 28 fractional digits. Number parsing uses the .NET `NumberStyles.Number` grammar. A macroregion with no single currency code reports `XXX` and formats with the runtime's generic currency symbol.

Cardinal rules use the 39 pinned CLDR 49 groups for plain decimals. Pass the resolved locale tag so regional rules such as `pt-PT` are used. `1m` and `1.0m` retain their distinct visible precision. An explicit precision can pad or remove zeros when the value is exactly representable. Exact signed numeric message selectors take precedence over cardinal categories, and each choice requires `Other`.

The Web runner selects its ICU data. A Blazor runner serving a broad locale catalog can load the full file.

```xml
<InvariantGlobalization>false</InvariantGlobalization>
<BlazorWebAssemblyLoadAllGlobalizationData>true</BlazorWebAssemblyLoadAllGlobalizationData>
```

A qualified CJK subset can use the SDK's `BlazorIcuDataFileName` instead. Browser/Console WASM
uses `WasmIcuDataFileName`. The formatter rejects invariant mode and cultures the runtime cannot resolve. Validate the actual published data for each required culture. [The .NET WASM ICU design](https://github.com/dotnet/runtime/blob/v10.0.15/docs/design/features/globalization-icu-wasm.md)

Run the bounded contract checks from the repository root:

```powershell
python Doroti/eng/validate-intl.py native --trim --aot --pack
python Doroti/eng/validate-intl.py web --sdk 10 --mode compatible --configuration Debug --firefox
python Doroti/eng/validate-intl.py web --sdk 10 --mode threaded --configuration Release
```

Each child has an external 1200-second timeout. Native, runtime-only Web, Doroti rendering,
physical input and complex-script shaping have separate acceptance scopes. P0 results and
remaining gates are recorded in [work2.md](../../../work2.md).
