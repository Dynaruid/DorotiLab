# Doroti.Intl

P0 qualification implementation of explicit .NET formatting and a small CLDR cardinal subset.
The initial formatter profiles are `en-US` and `ko-KR`; neutral `en`/`ko` select those regions.
The package does not register or replace any Widgets, Material or Cupertino delegate.

```csharp
var format = new Doroti.Intl.IntlFormatter("ko-KR");
string date = format.FormatDate(new DateTime(2024, 2, 29), Doroti.Intl.WidgetDateFormat.Compact);
bool valid = format.TryParseDate(date, out DateTime parsed);
string price = format.FormatCurrency(1234.5m); // Culture default currency: KRW, .NET rounding.
```

Culture objects are read-only and use `useUserOverride:false`. Widget dates use Gregorian
years 1..9999, canonical four-digit-year input, `DateTimeStyles.None`, and no implicit timezone
conversion. Native digits, arbitrary currency selection, compact notation, ICU skeletons and
other cultures are outside this initial API. Numeric values are `long` or `decimal`; NaN and
Infinity are excluded. Precision is 0..28. Number parsing follows .NET `NumberStyles.Number`,
including its permissive grouping grammar; date parsing requires the canonical input pattern.

Cardinal rules support language codes `en`, `ko`, `ru`, `ar` from CLDR 49. Pass the resolved
language code rather than a locale tag. `1m` and `1.0m` retain their distinct visible precision.
An explicit precision can pad/remove zeros but cannot silently round the input. Exact signed
numeric message selectors take precedence over cardinal categories; `Other` is required.
Ordinal rules, offset, `#`, runtime message parsing and locale negotiation are not provided.

The Web consumer owns ICU selection. For a Blazor WebAssembly runner requiring both profiles:

```xml
<InvariantGlobalization>false</InvariantGlobalization>
<BlazorWebAssemblyLoadAllGlobalizationData>true</BlazorWebAssemblyLoadAllGlobalizationData>
```

A qualified CJK subset can use the SDK's `BlazorIcuDataFileName` instead. Browser/Console WASM
uses its own `WasmIcuDataFileName`; the library does not alias these different SDK properties.
Missing/invariant data fails explicitly. See [the .NET WASM ICU design](https://github.com/dotnet/runtime/blob/v10.0.15/docs/design/features/globalization-icu-wasm.md).

Run the bounded contract checks from the repository root:

```powershell
python Doroti/eng/validate-intl.py native --trim --aot --pack
python Doroti/eng/validate-intl.py web --sdk 10 --mode compatible --configuration Debug --firefox
python Doroti/eng/validate-intl.py web --sdk 10 --mode threaded --configuration Release
```

Each child has an external 1200-second timeout. Native, runtime-only Web, Doroti rendering,
physical input and complex-script shaping have separate acceptance scopes. P0 results and
remaining gates are recorded in [work2.md](../../../work2.md).
