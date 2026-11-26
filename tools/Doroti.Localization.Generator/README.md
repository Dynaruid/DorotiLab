# Doroti ARB localization generator

Reference `Doroti.Localization.Generator` as a build-only analyzer in the app project. Use a .NET 10 or 11 SDK CLI. For messages with formatting or plural rules, add `Doroti.Intl` to the same app project.

```xml
<PropertyGroup>
  <DorotiLocalizationClass>AppLocalizations</DorotiLocalizationClass>
  <DorotiLocalizationNamespace>Example</DorotiLocalizationNamespace>
  <DorotiLocalizationTemplate>en</DorotiLocalizationTemplate>
</PropertyGroup>
<ItemGroup>
  <PackageReference Include="Doroti.Localization.Generator" Version="0.4.1-beta" PrivateAssets="all" />
  <PackageReference Include="Doroti.Intl" Version="0.4.1-beta" />
  <AdditionalFiles Include="l10n/*.arb" />
</ItemGroup>
```

Give each file an `@@locale` tag. The template declares the message keys, parameters, types and format options. Each locale in the selected registry needs every key. Use `DorotiLocalizationLocales` to select exact tags or pass only the ARB files the app needs. The generator always includes the template.

```json
{
  "@@locale": "en",
  "greeting": "Hello, {name}!",
  "@greeting": { "placeholders": { "name": { "type": "String" } } },
  "cart": "{count, plural, =0 {Your cart is empty} one {One item} other {{count} items}}",
  "@cart": { "placeholders": { "count": { "type": "long", "format": "decimalPattern" } } }
}
```

Pass `AppLocalizations.Delegate` in the app's delegate list and use `AppLocalizations.SupportedLocales` for locale negotiation. Read messages using `AppLocalizations.Of(context).greeting(name)`. The generated delegate looks for an exact tag, the same declared language/script, or a declared language-only resource. Unknown locales report an error when requested directly with `Get`.

The grammar supports plain text, typed placeholders, nested cardinal plural and string `select`. Exact signed decimal selectors precede plural categories. Each choice requires `other`. Use apostrophe quoting for literal braces and `''` for a literal apostrophe. Use named placeholders for count display inside a plural. Plural operands use `int`, `long`, or `decimal`; the latter retains visible trailing zeros.

The supported parameter types are `String`, `int`, `long`, `decimal` (`num`), `double`, `DateTime` and `TimeOnly`. Unformatted numeric placeholders use invariant formatting. `decimalPattern`, `decimalPatternDigits`, `currency`, `simpleCurrency`, `percentPattern`, and `decimalPercentPattern` use the `IntlFormatter` profile. Digits options use `optionalParameters.decimalDigits` where the formatter supports them. `yMd`, `yMMMd`, `MMMEd`, `yMMMMEEEEd`, `yMMMM`, `MMMd`, and `y` use the corresponding Gregorian widget field combinations. `Hm` forces 24-hour time and `jm` follows the locale profile. The generated methods use the requested locale for .NET formatting and pinned CLDR cardinal rules, including when a parent bundle supplies the translated text.

`DLOC001` reports duplicate keys, missing translations, invalid messages, unsupported formats and placeholder errors in the `.arb` file. Each build compiles the resources into the app assembly. Host projects consume that app assembly through their existing project or package reference.
