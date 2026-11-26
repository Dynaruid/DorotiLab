# P0 .NET formatting and data contract

This file records the initial en-US and ko-KR qualification. The expanded formatter and widget catalog are documented in [Doroti.Intl](../../src/Doroti.Intl/README.md) and [the widget packages guide](widget-packages.md).

Established 2026-11-16 for the first implementation unit in the [localization history](../../../history/26/11/13/intl-localization-summary.md).

## Source inventory and ownership

[widget-inventory.json](widget-inventory.json) records every current public abstract/virtual
resource member, including virtual English defaults: Widgets **17**, Cupertino **45**, Material
**157**, total **219**. Each entry retains its source location, signature and source SHA-256.
Hashes use UTF-8 text without a BOM and LF line endings so Windows/Linux checkouts agree.
The inventory separates .NET formatting with widget policy, calendar/hour/direction policy,
messages with numeric/cardinal branches, and translated messages/interpolation. It is a current
Doroti API inventory, not a claim that all pinned Flutter translations have been implemented.

Regenerate with `python Doroti/eng/localization-inventory.py`; use `--check` to detect stale API
snapshots. Formatting mappings are intentionally small:

| Existing members | .NET reuse | Required Doroti policy/addition |
| --- | --- | --- |
| Material `formatDecimal`; picker year/day/hour/minute/second | Explicit culture, `long.ToString`, fixed standard formats | Grouping, zero padding, allowed field ranges |
| Material date formats; Cupertino month/day/medium date | `DateTime`, Gregorian `DateTimeFormatInfo`, .NET custom patterns | Supported field combinations, date order, date-only values |
| Material `parseCompactDate` | `DateTime.TryParseExact`, `DateTimeStyles.None` | Four-digit Gregorian year, canonical separators/padding, invalid input vs configuration failure |
| `narrowWeekdays`, first weekday, picker date order | Culture weekday/month data | Narrow labels and widget order remain P2 policy; no generic ICU skeleton engine |
| `timeOfDayFormat`, hour/minute/day period strings | `TimeOnly` and culture day-period data | Explicit 12/24-hour override, day period placement and padding |
| Counted license/row/character/timer messages | Typed generated interpolation and .NET numeric formatting | CLDR cardinal subset; exact numeric selectors before category |
| Menus, tooltips, keyboard labels, semantics, direction | Existing delegate interfaces | Complete opt-in resource data; not supplied by CultureInfo |

`Doroti.Intl` currently depends only on .NET. A future common locale helper can add the lower
`Doroti.Runtime` dependency in P1; no Widgets, Skia or host dependency is required for P0.

## Public P0 support boundary

- Formatting profiles: `en-US`, `ko-KR`, case-insensitive. Neutral `en` resolves to `en-US`, `ko`
  to `ko-KR`. Underscores, aliases other than those two neutral names, extensions, variants and
  other profiles fail explicitly. This is not the P1 BCP 47 parser or locale negotiator.
- Each formatter owns a cached read-only `CultureInfo`, constructed with `useUserOverride:false`.
  `CurrentCulture`, `CurrentUICulture`, thread defaults and application locale are not modified.
- Numeric input: `long` for grouped integers, `decimal` for numbers, percent ratios and money.
  Precision is 0..28. The .NET standard formatter owns rounding, signs and grouping. NaN,
  infinity, arbitrary `object.ToString`, double approximation and native digits are excluded.
- Default currencies are USD for en-US and KRW for ko-KR, using the culture's default currency
  precision; callers may explicitly override fractional digits. Arbitrary ISO currency-code
  selection remains a P2 extension. A percent input of `0.125m` means 12.5 percent.
- Number parsing uses .NET `NumberStyles.Number`, including its permissive grouping placement.
  It is not advertised as a strict grouped-number grammar.
- Widget dates use Gregorian years 0001..9999. `DateTime` fields are formatted without implicit
  local/UTC conversion. The adapter does not expose a timezone conversion API yet.
- Date input is exactly `MM/dd/yyyy` or `yyyy. MM. dd.`. Parsing rejects whitespace, two-digit
  years, unpadded fields, invalid leap days, time components and zone suffixes. Invalid user input
  returns `false`; successful values have `DateTimeKind.Unspecified`.
- The default 12-hour pattern is `h:mm tt` in English and `tt h:mm` in Korean. The explicit
  24-hour override is `HH:mm`; hour/minute picker fields use `HH`/`mm` zero padding as declared.
  AM/PM text comes from .NET data, including documented differences between profiles below.

| Date fields | en-US .NET pattern | ko-KR .NET pattern |
| --- | --- | --- |
| Compact/input | `MM/dd/yyyy` | `yyyy. MM. dd.` |
| Short | `MMM d, yyyy` | `yyyy년 M월 d일` |
| Medium | `ddd, MMM d` | `M월 d일 (ddd)` |
| Full | `dddd, MMMM d, yyyy` | `yyyy년 M월 d일 dddd` |
| Month/year | `MMMM yyyy` | `yyyy년 M월` |
| Short month/day | `MMM d` | `M월 d일` |
| Year | `yyyy` | `yyyy` |

These are explicit widget field policies, not ICU skeleton substitutions or promises of exact
Flutter output. Both initial profiles use Sunday as the first weekday. Raw diagnostic cultures
fr-FR, hi-IN and th-TH are inspected to document grouping, spaces and calendar differences;
they are not added to the formatter's support list.

## Cardinal rules

The [provenance manifest](../../src/Doroti.Intl/data/plural-provenance.json) pins CLDR **49**,
revision `96e54b273d2da4ec6fbad350ba19ff23fa24a588`, full upstream XML SHA-256
`69a30c57a621de9c52ec6ca95fe7bf9f06df6f8c13ca71a72e447b532c92cff7`, and the Unicode license
hash. The small evaluator independently transcribes only en/ko/ru/ar cardinal conditions.
There is no runtime XML parser, data generator, global mutable operand state or ordinal engine.

Callers pass a resolved language code, not an arbitrary locale tag. Unknown codes throw; they
do not silently select English or `other`. `decimal` scale is visible fraction precision:
English `1m` is `One`, `1.0m` is `Other`. Explicit visible precision can add/remove trailing zeros
but must represent the value exactly; callers round first when required by their display policy.
Arabic `3.0m` is `Few`, `3.5m` is `Other`: CLDR integer-range membership matters.
Exact signed selectors (`=0`, `=1`, `=-1`) precede categories, and an `Other` branch is required.

There is no plural/select message parser, ARB compiler, offset, `#`, ordinal, compact exponent
or arbitrary ICU MessageFormat support in this unit. The 39 fixed boundary fixtures and 16
concurrent tasks exercise the selected rules without unbounded repetition.

## Runtime data and allowed differences

The actual tested data provider must be recorded, rather than inferred from SDK version alone.
`--inspect-profile` prints loaded ICU modules and raw culture fields. Observed examples:

| Environment | Actual data | Allowed observed output |
| --- | --- | --- |
| Windows 10.0.26300, .NET 10.0.15 | `C:\Windows\System32\icu.dll`, file 72.1.0.4 | Korean `오후 1:05` |
| WSL Ubuntu 26.04, .NET 10.0.14 | `libicuuc`/`libicui18n`/`libicudata.so.78.2` | Korean `PM 1:05` |
| net10 browser, Mono 10.0.15 | Published/requested `icudt` file and its SHA-256 in the receipt | Korean `오후 1:05` in the full-data profile |

The ICU 78 Korean day-period difference is retained as .NET output. Replacing it with our own
translation or rejecting it as missing data would contradict the adopted .NET-first contract.
The library checks supported culture enumeration and required localized January data before
using a profile, including invariant mode with `PredefinedCulturesOnly=false`. These checks
catch the missing-culture fixtures; SDK boot integrity checks protect published data bytes.

Raw French grouping retains U+202F, and currency spacing retains its actual U+0020/U+00A0 or
other profile value. Tests do not strip these characters or direction controls to pass.
Raw Thai culture uses `ThaiBuddhistCalendar`; it is not silently promoted into the Gregorian
widget support list. OS/.NET ICU updates require a fresh profile comparison.

## Web and distribution qualification

The isolated probe fixes `net10.0`/Mono `10.0.15`, with SDK 10 and SDK 11 selecting separate
intermediate trees. Debug exercises managed runtime code; Release explicitly enables native
build and Mono WASM AOT. It does not use NativeAOT or claim net11 product qualification.

The Blazor consumer selects `BlazorWebAssemblyLoadAllGlobalizationData` or a verified
`BlazorIcuDataFileName`. Browser/Console WASM uses its own `WasmIcuDataFileName`. The library
does not redefine these settings or enlarge existing English apps' data automatically.
See the [.NET WASM ICU design](https://github.com/dotnet/runtime/blob/v10.0.15/docs/design/features/globalization-icu-wasm.md).

Browser checks start in English, switch to Korean without restarting the runtime, exercise
root and `/demo/` asset bases, and record the actual requested ICU asset. Missing/corrupt data
must fail before English output can appear successful. The probe also bounds initialization
because a runtime download/integrity error can leave .NET 10's `create()` promise pending.
This 30-second probe diagnostic is separate from the external 1200-second process-tree limit.

Native tests exercise full trim and NativeAOT, and an isolated NuGet consumer verifies the
package and transitive Unicode license with no Doroti source reference, Dart or Flutter tools.
The final Linux verification SDK is cached under `/home/parti/.cache/doroti/intl-dotnet10`;
the system SDK and repository pins were not changed. Its SDK/ILCompiler cohort avoids suppressing the older
10.0.400 toolchain's audit failure or mixing incompatible compiler packages.

The runtime-only probe's DOM timing is not a Doroti frame. The Demo opt-in page provides a
separate real-renderer comparison using the same layout/font setup and English strings with
legacy/default formatting versus the new adapter. `DorotiIntlProbe=baseline|adapter` selects
that page; omitting it keeps the Components gallery. Korean output on that page tests explicit
formatting and font drawing; translated framework delegates and resolved locale propagation
remain P1/P2 work. Headless canvas commits are not physical scanout or hardware acceptance.

For this specific SDK10/net10 compatible Debug profile, the shipped and requested CJK shard
passed English startup and Korean switching in both the runtime probe and actual Demo page.
Its size is 956,416 bytes, versus 1,526,128 bytes for the full file. This qualification does not
automatically select CJK for other apps or cover additional business-logic formatting cultures.

## Remaining work after P0

P1 still owns common BCP 47 normalization, host language lists, subtree/application locale
isolation, multi-view behavior and async delegate failure/race handling. P2 owns complete
opt-in widget translations and their bindings to this format contract. P3 owns ARB generation.
P4 owns paragraph/shaping/Unicode/paint/geometry feasibility and platform acceptance. P5 owns
qualified expansion and size optimization. None is completed by this P0 package/probe alone.
