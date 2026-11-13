# Doroti ICU / intl / 현지화 구성 검토 및 도입 계획

- 작성일: 2026-11-13
- 검토 기준: DorotiLab `0a7fdcfe2ffa`, 현재 제품 소스와 공식 upstream 문서·소스
- 채택 방향: **.NET 우선 + 부족한 국제화 기능만 보강.** 이전의 Dart `intl` 관리형 이식 우선안을 대체한다.
- 상태: **설계 방향 확정, 구현 미착수. 제품 구현·성능 측정·플랫폼 실행 검증은 하지 않았다.**

## 0. 요약과 채택 결정

**Flutter식 Localizations·위젯 번역·ARB 흐름은 유지하되, 날짜·숫자·달력·시간대의 기본 구현은 .NET 공개 API를 재사용한다. Dart `intl` 전체 이식과 Flutter 출력의 완전 일치는 기본 목표에서 제외한다.**

1. **텍스트 엔진:** Unicode 경계, 양방향 텍스트, shaping, 줄바꿈, caret/selection geometry.
2. **`Doroti.Intl`:** `System.Globalization` 등을 감싸는 얇은 어댑터와 .NET에 없는 최소 기능. 날짜·숫자 데이터를 별도로 통째로 복제하지 않는다.
3. **위젯 현지화:** 기존 `LocalizationsDelegate<T>` 위에 선택적으로 추가하는 Widgets/Material/Cupertino 번역.
4. **앱 현지화 도구:** ARB를 빌드 시 검증하여 타입 안전한 C# 리소스와 delegate로 생성.

채택한 기본 정책은 다음과 같다.

- **기본 포맷·파싱은 .NET을 사용한다.** `CultureInfo`, `DateTimeFormatInfo`, `NumberFormatInfo`, `Calendar`, `TimeZoneInfo`, `StringInfo`를 용도에 맞게 재사용한다.
- **없는 기능만 보강한다.** CLDR cardinal plural, 위젯이 요구하는 날짜 필드 조합, 필요한 compact 표기·native digits 등을 개별 요구와 테스트에 따라 추가한다. 처음부터 전체 ICU 패턴 엔진을 만들지 않는다.
- **.NET이 ICU를 사용한다는 것과 ICU 전체 API를 노출한다는 것은 다르다.** 플랫폼별 ICU/NLS 등 실제 globalization 경로를 확인하고, 비공개 runtime 심볼에는 의존하지 않는다.
- **직접 native ICU 도입은 기본 포맷 계층에 요구하지 않는다.** 텍스트 엔진의 부족은 별도 트랙에서 검토하며, SkParagraph 등 후보의 연결 가능성부터 증명한다.
- **기본 영어 구현과 기존 앱 동작은 유지한다.** 다국어 위젯 리소스는 별도 opt-in 패키지로 제공한다.
- **초기 기능 구현과 전체 언어권 렌더링 완료를 분리한다.** 한국어/영어 번역이 동작한다고 RTL·태국어·인도계 문자까지 지원 완료라고 선언하지 않는다.
- **새 ICU, ICU4N, 브라우저 `Intl`을 한꺼번에 중첩 도입하지 않는다.** 추가 바이너리·데이터·런타임마다 소유권과 실제 필요성을 설명할 수 있어야 한다.

이 결정은 **.NET 생태계 재사용·유지보수 비용·배포 단순성**을 우선한다. Flutter와 날짜 패턴·반올림·공백·숫자 표기가 달라도, 선언한 .NET 의미론과 위젯 정책에 맞으면 허용한다. OS/runtime 데이터 차이는 통제 가능한 범위를 명시하고 검증하며, 완전히 동일하다고 약속하지 않는다.

여기서 `.NET intl`은 별도의 공식 단일 패키지를 뜻하지 않는다. .NET globalization API, 선택한 메시지 리소스 방식, 부족한 기능의 보강을 조합하는 구조이다. [N1], [N4]

## 1. "Flutter처럼"의 의미와 호환성 경계

### 1.1 Flutter에서도 ICU와 intl은 다른 계층이다

| 계층 | Flutter의 역할 | Doroti에 적용할 의미 |
| --- | --- | --- |
| Engine Unicode | 네이티브 문단의 Unicode 분석과 텍스트 처리. 기준 소스는 SkParagraph 생성에 `SkUnicodes::ICU::Make()`를 전달한다. | 번역과 독립적인 텍스트 엔진 책임 |
| Shaping / paragraph | 문자·폰트·script·direction을 실제 glyph run과 줄·위치로 변환 | ICU 데이터만 추가해서 해결되지 않는 영역 |
| Dart `intl` | 날짜·숫자 포맷, 파싱, plural/select 관련 계산, Bidi 유틸리티와 로케일 데이터 | 기능 분리는 참고하되 기본 포맷은 .NET, 없는 기능만 보강 |
| `flutter_localizations` | Widgets/Material/Cupertino의 번역 및 위젯별 포맷 정책 | 기존 추상 Localizations 구현을 채우는 opt-in 리소스 |
| `gen-l10n` | ARB 메시지를 타입 있는 Dart API로 생성 | 빌드 시 C# 코드 생성 |
| `Localizations` | 로케일 선택, delegate 로드, subtree 리소스와 방향성 전달 | 현재 Doroti 구조를 재사용 |

근거: [F1], [F2], [F3], [F4], [F6], [I1], [I2].

**ARB가 ICU 계열 메시지 문법을 쓴다고 해서 실행 시 ICU `MessageFormat` 객체가 필수인 것은 아니다.** Flutter의 생성 도구처럼 빌드 시 분기 코드로 바꾸고, plural 계산과 포매터만 런타임에서 호출할 수 있다.

또한 Flutter Web을 전부 "동일한 네이티브 ICU 파일"로 설명해서는 안 된다. 현재 upstream skwasm에는 built-in ICU 경로와 client가 경계 데이터를 공급하는 경로가 따로 존재한다. 이 구조는 backend별 선택이 가능하다는 근거이지, Doroti가 브라우저 `Intl`로 모든 텍스트 처리를 대체할 수 있다는 근거가 아니다. [F5]

### 1.2 보장 수준을 나눈다

- **구조 호환:** delegate, 지원 로케일, ARB, 생성 API, 위젯 번역의 사용 방식.
- **문자열 계약:** 기본 포맷·파싱은 명시한 .NET API·옵션·culture·달력 정책을 따른다. 추가 plural 등은 고정한 규칙·데이터를 따른다. Flutter와의 완전 일치는 보장하지 않는다.
- **텍스트 동작 호환:** shaping, visual order, caret, selection, 줄바꿈이 지원 언어에서 올바름.
- **픽셀 호환:** 폰트 파일, 폰트 fallback, 엔진 revision, DPI까지 통제해야 하는 별도 목표.

초기 목표는 구조 호환과 .NET 기반 문자열 계약이다. Flutter 결과는 차이 파악용 참고이지 기본 포매터의 합격 oracle이 아니다. 정확한 Flutter 출력이 필수인 기능이 생기면 그 범위만 별도 요구·비용으로 재검토한다. 텍스트 동작 호환은 별도 필수 트랙으로 진행하며 같은 ICU 버전이라는 이유만으로 문자열·픽셀 호환을 보장하지 않는다.

## 2. 현재 Doroti에서 확인한 사실

| 영역 | 확인 내용 | 근거 |
| --- | --- | --- |
| 로케일 모델 | `Locale`는 language/country/script를 보관하는 record이며 `toLanguageTag()`를 제공한다. 현재 이 타입 자체에 공통 파서·alias 정규화는 없다. | [ViewContracts.cs](Doroti/src/Doroti.Ui/ViewContracts.cs) |
| 로케일 선택 | preferred locale 목록, language/script/country 매칭, callback과 supportedLocales 기반 해석이 이미 있다. 새 resolver 전체를 만들 필요가 없다. | [app.cs](Doroti/src/Doroti.Framework.Widgets/app.cs), [localizations.cs](Doroti/src/Doroti.Framework.Widgets/localizations.cs) |
| 리소스 로드 | `ILocalizationsDelegate`, `LocalizationsDelegate<T>`, 동기·비동기 로드 및 subtree override가 있다. 같은 리소스 타입은 앞의 지원 delegate가 선택된다. | [localizations.cs](Doroti/src/Doroti.Framework.Widgets/localizations.cs) |
| 기본 위젯 번역 | Material/Cupertino delegate는 영어만 지원한다. Widgets 기본 delegate는 모든 로케일을 수락하지만 영어 문자열과 LTR을 반환한다. | [Material](packages/Doroti.Material/src/material_localizations.cs), [Cupertino](packages/Doroti.Cupertino/src/localizations.cs), [Widgets](Doroti/src/Doroti.Framework.Widgets/localizations.cs) |
| 기본 포맷 | Material 날짜는 영어 배열·미국식 패턴, 숫자는 직접 쉼표를 삽입한다. 전역 intl 계층과 연결되어 있지 않다. | [material_localizations.cs](packages/Doroti.Material/src/material_localizations.cs) |
| 브라우저 로케일 | snapshot은 `navigator.language` 하나를 전달한다. C#은 두 부분 태그를 language/country로 해석하므로 `zh-Hant` 같은 태그를 제대로 구분하지 못한다. | [doroti.web.ts](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts), [BrowserHostContracts.cs](packages/platforms/web/Doroti.Host.Web/BrowserHostContracts.cs) |
| Qt 로케일 | 단순 분리하는 `ResolveLocales`와 script를 구분하는 `ParseLocale`가 공존한다. 정규화 경로의 통합이 필요하다. | [QtHostAdapter.cs](packages/platforms/qt/Doroti.Host.Qt/QtHostAdapter.cs) |
| Web globalization | `InvariantGlobalization=false`, 시간대 지원 활성화. 전체 ICU 데이터 로드 또는 특정 데이터 파일 선택은 이 타깃 설정에 명시되어 있지 않다. | [Web props](packages/platforms/web/Doroti.Target.Web.browser-wasm/build/Doroti.Target.Web.browser-wasm.props) |
| Qt ICU | Qt 6.12 Linux 배포의 private dependency closure는 ICU 73을 요구한다. 이는 Doroti 공통 intl provider가 아니다. | [qt-runtime.md](packages/platforms/qt/qt-runtime.md), [SDK lock](packages/platforms/qt/sdk/qt-6.12.0-linux-x64.lock.json) |
| 텍스트 요청 경계 | `ParagraphRequest`에 이미 locale, direction, runs가 있으며 host capability를 사용한다. | [GraphicsAndSemanticsContracts.cs](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs) |
| 공통 Skia 문단 | `Layout`은 run별 advance를 측정하고 관리형 `Paragraph`로 레이아웃한다. 그리기는 `SKCanvas.DrawText`, glyph width 및 code point별 font fallback을 사용한다. 검토 경로에는 SkParagraph/shaped-run 연결이 없다. | [SkiaSceneRenderer.cs](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs) |
| host 재사용 | Web 및 MAUI의 paragraph capability도 공통 renderer의 `Layout`으로 위임한다. 해당 문제를 각 host의 번역 코드에서 해결할 수 없다. | [BrowserSkiaCapabilities.cs](packages/platforms/web/Doroti.Host.Web/BrowserSkiaCapabilities.cs), [MauiSkiaCapabilities.cs](packages/platforms/maui/Doroti.Host.Maui/MauiSkiaCapabilities.cs) |
| 경계·접근성 | 문자소 경계는 .NET `StringInfo`를 사용한다. 기본 `getWordBoundary`는 공백/비공백 분류이므로 언어별 단어 분석이 아니다. semantics도 별도로 StringInfo 경계를 만든다. | [Paragraph](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs), [SemanticsTextGeometry.cs](Doroti/src/Doroti.Ui/SemanticsTextGeometry.cs) |
| 의존성 | SkiaSharp/HarfBuzz 버전 항목은 있으나 버전 선언만으로 실제 shaping 경로 사용을 증명하지 못한다. | [Directory.Packages.props](Doroti/Directory.Packages.props), [renderer project](Doroti/src/Doroti.Skia.Rendering/Doroti.Skia.Rendering.csproj) |
| 빌드 도구 | 앱은 플랫폼 중립이고 runner가 host/native 경계를 소유한다. Web의 `DorotiLocalizationResource` JSON item 선언만으로 ARB 생성 기능이 있다고 볼 수 없다. | [App SDK](Doroti/src/Doroti.App.Sdk/Sdk/Sdk.targets), [Runner SDK](Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets), [Web props](packages/platforms/web/Doroti.Target.Web.browser-wasm/build/Doroti.Target.Web.browser-wasm.props) |

### 2.1 초기 대화보다 보강해야 할 판단

".NET 국제화를 켜고 번역 delegate를 추가한다"만으로는 Flutter 수준의 텍스트 동작이 완성되지 않는다. 현재 공통 경로에서는 **문자별 폭의 합과 logical-order 그리기에서 shaped glyph run/visual-order 문단으로 넘어가는 작업**이 필요하다.

아랍어 연결형, Indic 재배치, ligature, 혼합 RTL/LTR, 태국어 단어 선택, emoji sequence의 caret/삭제는 별도 인수 대상이다. 번역 문자열이 화면에 보이는 것과 올바른 언어권 지원은 구분한다.

## 3. 권장 패키지와 의존 방향

아래 이름 중 새 패키지·API는 **제안명**이며 현재 존재하는 기능을 뜻하지 않는다.

```text
Application ARB
  -> build-time localization generator
     -> AppLocalizations + typed methods + delegate + supportedLocales

Optional framework translations
  Doroti.Localizations.Widgets   -> Framework.Widgets + Doroti.Intl
  Doroti.Localizations.Cupertino -> Doroti.Cupertino + Widgets resources + Intl
  Doroti.Localizations.Material  -> Doroti.Material + Cupertino/Widgets resources + Intl

Shared formatting
  Doroti.Intl -> Doroti.Runtime
    -> .NET Globalization / Calendar / TimeZoneInfo
    -> only missing features: pinned CLDR plural + required format helpers

Rendering
  TextPainter / ParagraphRequest
    -> existing IParagraphHostCapability
       -> common paragraph backend
          -> qualified shaping / Unicode / font implementation
```

### 3.1 물리적 분리는 기존 디자인 경계를 따른다

- `Doroti.Intl`은 위젯·Skia·host에 의존하지 않는 얇은 .NET 어댑터이다. 공통 로케일 정규화 helper가 필요하면 기존 하위 계층인 `Doroti.Runtime`에 최소 기능만 둔다. 전체 DateFormat/NumberFormat 엔진이나 일반 provider 선택 체계는 만들지 않는다.
- 현재 `Doroti.Ui -> Doroti.Runtime` 방향과 `Locale` 공개 위치는 유지한다. 단순한 문자열 태그·정규화를 위해 새 범용 서비스 프레임워크를 만들지 않는다.
- 위젯 번역은 `packages`의 별도 선택 패키지로 두어, Widgets-only 앱에 Material·아이콘 의존성을 끌어오지 않는다.
- Material 사용자 편의를 위한 delegate 묶음은 제공하되 기존 앱의 기본 delegate를 자동 교체하지 않는다.
- generator는 빌드 도구이다. Roslyn, Dart SDK, Flutter SDK, ARB parser를 제품 런타임 의존성으로 넣지 않는다.
- 텍스트 backend는 기존 `Doroti.Skia.Rendering`과 `IParagraphHostCapability` 경계를 확장한다. 최초부터 별도 공개 provider 생태계나 서비스 locator를 추가하지 않는다.

한 개 `Doroti.Localizations` 패키지에 모두 넣는 것이 초기 구현은 간단하지만, Widgets-only/Cupertino-only 앱이 Material까지 참조하게 된다. 현재 디자인 패키지가 이미 분리되어 있으므로 리소스도 같은 경계를 따르는 편이 낫다.

## 4. intl 구현 대안 비교와 선택

| 대안 | 장점 | 한계·위험 | 판단 |
| --- | --- | --- | --- |
| `CultureInfo`/`DateTimeFormatInfo`/`NumberFormatInfo` 중심 | 기존 .NET 구현·데이터 재사용, 별도 대규모 포매터 이식 불필요, 표준 API·도구 활용 | OS/runtime 데이터 차이와 skeleton·compact·plural·native digits 등의 부족을 명시적으로 다뤄야 함 | **채택. .NET을 기본으로 하고 부족한 기능만 보강** |
| Dart `intl` 기능·데이터의 관리형 이식 | 특정 Flutter 버전의 결과를 맞추는 데 유리 | 포맷·파싱·반올림·데이터 갱신 유지보수, .NET과 중복되는 구현·데이터 | 기본안에서 제외. 정확한 Flutter 출력이 필수인 개별 기능에 한해 재검토 |
| ICU4C 포매터 직접 호출 | skeleton/plural 등 풍부한 API | RID별 배포·WASM 연결·ABI·수명 관리, Dart intl과 결과가 자동 일치하지 않음 | 기본안에서 제외. 공개 .NET API와 제한적 보강으로 해결되지 않는 요구가 있을 때만 재평가 |
| ICU4N | 관리형 BreakIterator 등 다양한 기능 | upstream README는 ICU4J 60.1 기반 부분 포팅·진행 중 상태를 설명한다. 현대 intl 전체 대체로 가정할 수 없음 | 기본 기반으로 비권고. 필요한 단일 기능의 별도 평가만 가능 |
| ICU4X | 작은 모듈, 선택적 데이터, Rust 기반 국제화 | Doroti의 C#/RID/WASM binding, 기능 범위, native pipeline 통합을 새로 검증해야 함 | 장기 대안. Rust 도구가 저장소에 있다는 이유만으로 채택하지 않음 |
| 브라우저 JS `Intl` 직접 호출 | Web에서 접근하기 쉽고 기능이 풍부함 | 브라우저별 데이터·출력·지원 차이, native와 의미론 분리, Worker 호출 경계 | Doroti 기본 어댑터로 추가하지 않음. 선택한 .NET runtime 자체의 내부 경로와는 구분 |

근거: [N1], [N2], [N3], [N4], [N5], [I1], [A1], [A2].

### 4.1 .NET 재사용과 최소 보강의 범위

`Doroti.Intl`은 위젯·ARB가 필요로 하는 동작을 .NET에 연결한다. .NET에 이미 있는 숫자·날짜 알고리즘이나 전체 CLDR 날짜·숫자 데이터를 다시 구현하지 않는다.

| 영역 | 기본 재사용 | Doroti에서 필요한 최소 보강 |
| --- | --- | --- |
| 숫자·통화·percent | .NET 숫자 포맷·파싱 + 명시한 `CultureInfo`/`NumberFormatInfo` | 위젯별 자릿수·zero padding·통화 코드 선택 정책과 옵션 검증 |
| 날짜·요일·월·주 시작일 | `DateTime`/`DateTimeFormatInfo`/`Calendar` | picker의 달력·필드 순서·날짜만의 의미와 엄격한 입력 패턴 |
| 시간·시간대 | .NET 시간 포맷 + `TimeZoneInfo` | Material hour-cycle/24시간 설정, 변환 시점·zone 명시 |
| 문자소 | `StringInfo` | paragraph·IME·semantics와의 경계 일치 검증. shaping/bidi는 별도 |
| 메시지·select | ARB 생성 코드의 정적 문자열·분기 | placeholder 타입·escaping·지원 문법 검증 |
| cardinal plural | 전용 .NET 공개 API로 해결되지 않는 영역 | 고정 CLDR 규칙의 작은 스레드 안전 evaluator와 필요한 locale 데이터 |
| skeleton·compact·native digits | .NET API에 같은 기능이 있다고 가정하지 않음 | 실제 필요한 날짜 필드 조합·compact 형식·숫자 기호에 한해 별도 구현·테스트 |

첫 제품 범위는 기본 날짜·숫자·통화·percent, 위젯별 시간 정책, strict date parsing, cardinal plural/select이다. 기존 API를 연결하는 것과 기능 전체를 공개·지원하는 것은 구분한다. compact 표기·추가 skeleton·ordinal·MessageFormat 2·spellout은 요구가 있을 때 확장한다.

지원하지 않는 생성 옵션·패턴은 빌드/생성 시 구체적으로 실패시킨다. locale 없는 `ToString()`, 영어 문자열, 임의의 scaling으로 성공처럼 대체하지 않는다. DartToCSharp나 Dart/Flutter 설치는 이 기본 경로의 선행 조건이 아니다.

### 4.2 .NET 어댑터의 의미론과 주의점

- **Culture 전달:** resolved locale에 맞는 culture를 각 호출에 명시한다. subtree·다중 view를 위해 `CurrentCulture`/`DefaultThreadCurrentCulture`를 바꾸지 않는다. 재사용 culture는 읽기 전용으로 두고 위젯별 수정이 필요하면 clone한다.
- **사용자 설정:** 자동 OS override를 적용할지 명시한다. 위젯용 기본 culture는 `useUserOverride:false`로 구성하고, host의 12/24시간 등 지원한 설정만 별도 인자로 반영한다. 이 정책도 OS/runtime 데이터 차이를 완전히 제거하지는 않는다.
- **패턴:** .NET format을 내부 기준으로 삼는다. ICU 패턴을 .NET에 그대로 전달하거나 문자 몇 개만 치환하지 않는다. Flutter/ARB의 format metadata는 지원 목록과 명시적 매핑을 거치며, 전체 ICU skeleton 호환을 약속하지 않는다.
- **날짜·파싱:** 요일·월·주 시작일은 .NET 데이터를 사용한다. 위젯 입력에는 허용한 패턴과 calendar를 정한 `TryParseExact` 계열·명시적 styles를 사용하고, 두 자리 연도·유효하지 않은 날짜·시간대 변환을 테스트한다. 잘못된 사용자 입력을 나타내는 표준 실패와 누락 데이터·구성 오류를 구분한다.
- **달력:** 표시 언어와 calendar 계산 모델을 분리한다. 기존 picker는 Gregorian 계산 모델을 유지하며, 위젯용 culture의 calendar도 지원 여부를 확인해 일치시킨다. 앱 일반 포맷의 culture 기본 달력 지원과는 구분하고, 지원되지 않는 조합은 조용히 다른 달력으로 대체하지 않는다.
- **위젯 시간 정책:** Material의 `TimeOfDayFormat`와 `alwaysUse24HourFormat`를 유지한다. upstream도 이 정책을 일반 `DateFormat` 호출 한 번으로 대체하지 않는다. [F2]
- **숫자:** grouping·소수점·기본 반올림은 .NET 의미론을 따른다. 통화 코드 선택, 표시 자릿수, 부호, 음수 0, 큰 정수, NaN/Infinity의 지원 범위만 명시적으로 정한다. Flutter 결과를 맞추기 위해 기본 숫자 엔진을 재작성하지 않는다.
- **Native digits:** `NumberFormatInfo.NativeDigits`는 .NET의 숫자·날짜 포맷/파싱에 적용되지 않는다. 초기 기본 동작은 .NET의 Latin digits이며, 지역 숫자가 필요하면 포맷된 숫자 필드에 한해 opt-in 보강한다. 메시지 전체의 문자 치환이나 설정만으로 지원 완료라고 처리하지 않는다. [N5]
- **수치·날짜 타입:** 지원하는 .NET 타입·범위·precision을 계약으로 정한다. Dart `num`/`DateTime`과의 전체 호환은 요구하지 않으며, 암묵적인 UTC/local 변환이나 `object.ToString()` 입력을 허용하지 않는다.
- **Plural:** cardinal과 ordinal을 혼동하지 않고, 정확한 `=n` 분기를 category보다 먼저 적용한다. CLDR의 visible fraction digits·precision 및 지원한 locale fallback을 명시한다. 규칙 데이터만 버전 고정하고 .NET 날짜·숫자 데이터까지 별도로 복제하지 않는다.
- **스레드:** 어댑터와 추가 plural 계산은 호출별 상태를 사용한다. Dart 구현 일부를 참고하더라도 isolate-local 전역 중간값을 .NET process-global mutable static으로 옮기지 않는다. [I3]
- **문자열 비교:** locale key·message key·캐시 key는 ordinal 의미론으로 비교한다. 사용자 텍스트의 collation과 구분한다.

**.NET을 기본으로 쓰는 것은 fallback이 아니라 채택한 정상 경로이다.** 공개 API에 없는 기능이 생길 때만 요구·규칙·검증 범위를 추가한다. Flutter 문자열과의 불일치를 일반적인 오류로 분류하지 않되, 선언한 .NET/위젯 계약 위반은 허용하지 않는다.

## 5. 텍스트 엔진과 ICU 도입 방안

### 5.1 .NET 포맷 도입과 분리된 검토 트랙

**.NET 포맷·위젯 번역의 도입은 새 native ICU나 paragraph backend 도입을 선행 조건으로 하지 않는다.** `StringInfo`도 문자소 처리에 재사용하지만, .NET globalization만으로 full bidi·언어별 word/line break·shaping이 제공된다고 보지 않는다.

복잡문자 지원을 확대할 때 Flutter와 가까운 장기 후보는 **SkParagraph + SkUnicode/ICU + shaping backend**이다. HarfBuzz와 ICU는 서로의 대체재가 아니다. Unicode 분석·줄바꿈과 glyph shaping·positioning을 함께 연결해야 한다. 그 연결은 해당 script의 지원 선언에 필요한 별도 작업이며, 기본 Intl 어댑터를 native ICU wrapper로 바꾸는 작업은 아니다.

우선순위는 다음과 같다.

1. 현재 고정된 SkiaSharp 버전에서 필요한 paragraph API와 native export가 지원되는지 확인한다.
2. 공식 API만으로 불가능하면 같은 Skia revision·빌드 옵션으로 만든 작은 C ABI bridge의 가능성을 실험한다.
3. bridge가 가능한 경우에만 RID별 패키징과 공개 capability 변경을 확정한다.
4. 기존 바이너리와 맞지 않는다면, 공식 SkiaSharp 유지와 custom Skia 배포 중 하나를 별도 아키텍처 결정으로 선택한다.

이번 공개 소스 검색에서는 사용 가능한 공식 `SKParagraph` API를 확인하지 못했다. 이것은 **텍스트 backend 선택의 선결 검증 사항**이지, .NET 기반 국제화 전체의 도입 차단이나 비공개 C++ 심볼을 P/Invoke해도 된다는 뜻이 아니다.

**금지할 접근:** 다른 revision의 Skia C++ 객체를 `SKCanvas`/`SKTypeface` 포인터로 섞기, 이미 로드된 .NET·Qt ICU 내부 심볼을 추측해서 연결하기, 소유권이 없는 ICU 데이터 메모리를 재사용하기.

### 5.2 기존 paragraph 계약에서 함께 바꿔야 하는 부분

- `ParagraphRequest.Locale`를 font fallback·script·언어별 shaping/분석까지 전달한다.
- 현재 code-unit advance 배열의 합만으로 문단을 표현하지 않는다. logical UTF-16 범위와 visual glyph 위치, cluster, direction run, line metrics가 필요하다.
- 이미 있는 host line/grapheme snapshot 구조와 `ApplyHostLayout`를 출발점으로 사용하되, word boundaries와 visual-order hit testing의 표현 가능성을 먼저 검증한다.
- measurement와 painting이 같은 shaped paragraph 또는 동일한 immutable run snapshot을 사용하게 한다. **측정은 shaping, 그리기는 기존 DrawText**인 혼합 경로는 허용하지 않는다.
- `getWordBoundary`, 줄바꿈, selection boxes, caret affinity, semantics 경계가 서로 다른 알고리즘으로 충돌하지 않게 한다.
- semantics의 StringInfo 결과를 무조건 제거하기보다, 엔진이 제공하는 검증된 경계·geometry를 전달받도록 계약을 확장한다.
- Flutter/기존 Doroti의 editing offset은 UTF-16이다. native API의 UTF-8 byte offset·code point index와 명시적으로 변환하고, cluster 경계를 보존한다.
- font fallback은 단일 code point마다 고르는 것만으로 끝내지 않는다. 결합 문자·emoji·script run의 폰트와 cluster 일관성을 검증한다.
- retained picture가 native paragraph를 참조한다면 snapshot의 수명을 보장한다. 공개 paragraph dispose 후에도 이미 기록한 picture를 그릴 수 있어야 하며, native 자원 해제는 owner 계약을 따른다.

이것은 기존 geometry/selection 동작에 영향을 주는 제품 변경이다. 도입 초기에는 기존 backend를 기본으로 유지하고, 검증된 backend만 명시적으로 활성화한다. 실패를 빈 텍스트나 기존 backend 자동 전환으로 숨기지 않는다.

### 5.3 플랫폼별 ICU 원칙

| 플랫폼 | 권장 원칙 |
| --- | --- |
| Windows/Linux desktop | 기본은 지원 .NET runtime의 globalization 경로이다. 실제 ICU/NLS·OS 데이터와 지원 culture를 확인한다. 출력 재현성이 추가로 필요하면 지원 runtime에 한해 app-local ICU를 검토하고, paragraph ICU와 별도로 관리한다. |
| Linux/Qt | Qt가 요구하는 ICU 73 closure를 임의로 바꾸지 않는다. 새 paragraph ICU가 다르면 symbol namespace·static linking·의존 폐쇄를 검증한다. "Qt에 있으므로 공통 재사용"으로 처리하지 않는다. |
| Android | .NET 공개 globalization API를 사용하고 AOT·실제 기기에서 culture 동작을 확인한다. 시스템 ICU가 있다는 이유로 비공개 native ABI에 의존하지 않는다. 추가 text 모듈을 선택할 때만 NDK·native 패키징을 확장한다. |
| iOS/Mac Catalyst/macOS | 기본 포맷은 .NET 공개 API로 사용한다. 시스템 내부 ICU 심볼에는 의존하지 않는다. 추가 text 모듈이 필요할 때 정적 링크·framework/native asset 등록과 실제 앱 시작을 별도로 검증한다. |
| Web/WASM | 기본 Intl 어댑터가 .NET ICU 데이터를 사용하므로 지원 culture의 데이터 포함을 인수한다. 새 paragraph 엔진용 데이터는 도입할 경우에만 별도로 검증하며 .NET 내부 export·data 포인터 재사용을 전제하지 않는다. |

**"ICU는 한 벌만 있어야 한다"를 강제하지 않는다.** .NET·Qt·paragraph는 서로 다른 ABI와 요구 기능을 가질 수 있다. 대신 각 사본의 소비자·버전·크기·배포 이유를 inventory로 남기고, 중복 제거는 지원되는 결합 방법이 있을 때만 한다. [N1]

## 6. 위젯 번역과 로케일 처리

### 6.1 기본 동작 보존

- 현재 DefaultWidgets/Material/CupertinoLocalizations는 영어 fallback으로 그대로 둔다.
- Global 계열은 opt-in 리소스 패키지에서 구현한다.
- 사용자 delegate가 기본/Global delegate보다 앞에서 같은 리소스 타입을 override하는 기존 규칙을 유지한다.
- Widgets 번역은 편집 메뉴·접근성 문구·textDirection까지 포함한다. Material의 날짜/시간 picker, pagination, keyboard label, semantics도 누락 없이 inventory화한다.
- 새 번역 클래스를 영어 기본 클래스에서 단순 상속하여 몇 문구만 덮는 방식은 피한다. 영어 plural·날짜 정책이 비영어 로케일에 섞이지 않도록 공통 추상 구현과 언어별 데이터를 분리한다. [F2]

시범 로케일은 `en-US`, `ko-KR`로 시작하되, release 후보 검증에는 `fr-FR`, `ar`, `ru`, `hi-IN`, `th`, `zh-Hans`/`zh-Hant`를 포함한다. 지원 선언은 검증한 subset만 노출한다.

### 6.2 하나의 로케일 입력 경로

1. Host는 가능한 경우 우선순위가 있는 언어 목록을 전달한다. Web은 `navigator.languages`를 사용하고 `languagechange`도 전달한다.
2. 공통 helper에서 BCP 47의 지원 범위에 맞춰 language/script/region, 대소문자와 alias를 정규화한다.
3. 공개 `Locale` 모델은 유지하고, 기존 resolver의 callback 순서와 매칭을 사용한다.
4. 리소스 선택과 포매터에는 같은 resolved locale을 명시적으로 전달한다. 포매터는 정규화된 태그에서 지원하는 .NET culture를 만들고, neutral locale의 구체화·지원하지 않는 culture 처리를 명시한다. 생성한 culture를 전역 현재 값으로 설정하지 않는다.
5. subtree override는 해당 subtree만 바꾸며 process-wide `CurrentCulture`를 수정하지 않는다.

현재 `_LocalizationsState`의 locale setter는 `isApplicationLevel` 조건 없이 dispatcher의 `setApplicationLocale`를 호출한다. subtree override가 앱 수준 locale·semantics를 변경하지 않는지 먼저 검증하고, 필요하면 이 호출 경계도 함께 정리한다. 기존 API가 있다는 사실을 scope 격리의 검증 완료로 보지 않는다.

`zh-Hant`의 Hant를 country로 읽지 않고, `sr-Latn-RS`, 숫자 region, `iw`/`he` alias를 검증한다. 확장·variant를 전부 모델링하지 않는 첫 버전이라면, 지원하지 않는 subtag의 처리 정책을 문서화하고 잘못된 region으로 조용히 저장하지 않는다.

**로케일 협상과 리소스 상속은 별개이다.** 기존 Flutter식 supportedLocales 매칭을 자의적인 CLDR likely-subtag 알고리즘으로 교체하지 않는다. 필요하면 정확한 태그에서 부모 리소스를 찾는 별도 lookup을 제공하며, CLDR parentLocales를 지원한다고 선언하려면 그 데이터도 포함한다.

### 6.3 RTL과 로딩 실패 처리

- textDirection은 위젯 리소스에서 제공한다. script가 다른 로케일까지 "특정 언어면 무조건 RTL"로 확대하지 말고 기준 Flutter와 지원 정책을 명시한다.
- Directionality는 레이아웃 기본 방향이지 UAX #9 bidi 알고리즘의 대체가 아니다.
- IME bridge·숨겨진 DOM input·semantics의 언어/방향 정보도 resolved locale과 함께 갱신한다. 화면 문자열만 갱신하고 접근성 언어는 이전 값으로 두지 않는다.
- 기존 비동기 delegate load에 오류·dispose·연속 locale 전환 테스트를 추가한다. 마지막 요청보다 늦게 완료된 이전 로드가 현재 locale을 덮지 않아야 한다.
- 초기 로드 실패 시 오류를 통지하고 first-frame defer가 영원히 남지 않게 한다. 기존 정상 locale을 유지하는 전환 실패는 명시적 정책과 진단을 동반한다.
- 누락 키·지원한다고 선언했지만 없는 데이터는 빌드 오류로 잡는다. 허용한 fallback은 경로와 대상 locale을 명시하고, 배포 누락·손상과 구분한다.

## 7. ARB와 C# 코드 생성

### 7.1 빌드 시 생성이 기본

ARB를 표준 입력으로 채택하고, 실행 중 임의 메시지를 파싱하는 시스템보다 **컴파일 시 생성 코드**를 우선한다.

생성물:

- 앱별 Localizations 추상 API와 locale별 구현.
- 문자열 getter 및 placeholder 타입이 있는 메서드.
- `LocalizationsDelegate<T>`와 지원 로케일 목록.
- 필요한 .NET 포맷 옵션·추가 plural 데이터 목록 및 로케일 provenance metadata.

초기 문법은 plain text, placeholder, cardinal plural, select, 중첩 및 escaping 중 **명시적으로 지원한 Flutter subset**이다. 정확한 `=n` selector, `other` 필수 여부, placeholder 이름·타입의 locale 간 일치, apostrophe/brace escaping을 검증한다.

ICU MessageFormat 전체, Flutter parser 문법, MessageFormat 2는 동일하지 않다. 예를 들어 offset/`#`/ordinal 지원을 기준 parser 확인 없이 약속하지 않는다. 지원 범위를 벗어나면 생성 오류를 낸다. [F4], [U1]

메시지 문법 호환과 포맷 결과 호환도 구분한다. ARB의 날짜·숫자 format metadata는 지원 목록에서 .NET 어댑터 옵션으로 변환한다. Dart `DateFormat`/`NumberFormat`의 모든 이름·패턴이 지원되는 것처럼 통과시키지 않는다.

### 7.2 저장소 통합 위치

- generator는 opt-in NuGet analyzer의 incremental source generator로 제공하는 방안을 우선한다. ARB는 `AdditionalFiles`, 설정은 MSBuild metadata로 전달한다.
- 위젯 번역용 생성과 앱 번역용 생성은 같은 parser·validator·emitter를 재사용한다. 별도의 정규식 치환기를 두 개 만들지 않는다.
- 앱 ARB는 플랫폼 중립 앱 프로젝트가 소유한다. runner마다 번역 C#을 중복 생성하거나 Flutter/Dart 프로세스를 제품 빌드의 필수 조건으로 만들지 않는다.
- 생성 코드는 컴파일러 산출물로 관리하고, 파일 출력이 필요한 도구는 `obj` 아래에 둔다. source tree에 자동 생성물을 매 빌드 덮어쓰지 않는다.
- 설정은 기존 SDK/MSBuild 방식을 우선한다. Flutter 사용자가 익숙하다는 이유만으로 첫 버전에 YAML 설정·parser를 추가하지 않는다.
- 최초에는 정적 생성 리소스를 사용한다. 런타임 다운로드, 번역 hot reload, 별도 CDN manifest, 동적 message compiler는 후속 요구가 생길 때 검토한다.
- 공개 CLI가 필요해지면 기존 도구 진입점을 확장하고 같은 생성 엔진을 사용한다. 새 `doroti gen-l10n` 명령이 이미 존재한다고 가정하지 않는다.
- .NET 우선 정책이 ARB를 `.resx`로 전면 교체한다는 뜻은 아니다. `ResourceManager`/`IStringLocalizer` 연동은 요구가 있을 때 어댑터로 검토하고, 동일 번역을 ARB와 `.resx` 두 군데에서 중복 관리하거나 ASP.NET 의존성을 강제하지 않는다.

### 7.3 AOT/trim과 데이터 선택

- reflection 기반 locale class 탐색, `Assembly.Load`, 런타임 expression compilation을 사용하지 않는다.
- 생성된 factory를 정적으로 참조한다. 모든 locale 구현을 하나의 전역 dictionary로 강하게 root하면 trimming이 자동으로 제거하지 못할 수 있다.
- 초기에는 크기를 먼저 측정하고, 필요할 때 번역/plural의 정적 factory/data shard와 앱별 생성 registry로 줄인다. .NET 날짜·숫자 데이터의 별도 사본을 생성하거나 언어마다 NuGet 패키지를 만드는 방식은 우선하지 않는다.
- UI supportedLocales만으로 사업 로직의 포매터 culture 전체를 추정하지 않는다. 앱이 별도로 포맷하는 통화·보고서 culture까지 runtime 데이터 요구 목록에 포함한다.
- 데이터 slicing은 정적 번역/plural, .NET runtime ICU 데이터, 추가 Unicode 분석, 폰트 coverage를 구분한다. 번역 trimming이 runtime ICU 파일까지 자동 축소하지 않으며, 한국어 번역을 선택했다고 한국어 폰트가 자동 포함되는 것도 아니다.

## 8. Web 배포 및 버전 정책

### 8.1 WASM ICU 데이터를 정확히 구분한다

.NET WebAssembly는 globalization을 켜도 실행 시작 시 선택한 ICU subset만 로드할 수 있다. 영어로 시작한 다음 한국어로 전환하는 앱이 필요한 데이터를 자동으로 갖춘다고 가정하면 안 된다. [N2], [N3]

- Blazor SDK에는 `BlazorWebAssemblyLoadAllGlobalizationData`, `BlazorIcuDataFileName`이 있다.
- 일반 WASM Browser/Console SDK에는 `WasmIcuDataFileName`이 있다.
- 이는 **실제 SDK 속성**이다. Doroti 전용 설정이라고 재정의하거나 SDK 구분 없이 모두 적용하지 않는다.
- 기존 `InvariantGlobalization=false`와 시간대 지원은 보존한다.
- P0에서는 전체 데이터 또는 검증된 CJK 포함 subset으로 locale 전환을 확인한 뒤, 실제 필요 목록을 기준으로 축소한다.
- 커스텀 데이터를 만들 때는 .NET 공식 가이드처럼 locale slicing부터 적용한다. feature를 임의 제거하거나 기본 fallback 데이터를 빼서 크기만 줄이지 않는다.

**채택한 `Doroti.Intl`은 .NET의 포맷·파싱을 사용하므로 runtime ICU 데이터는 기본 기능의 직접 의존성이다.** UI와 사업 로직에서 필요한 culture 전체를 포함한다. 그렇다고 매 배포에 full 데이터를 무조건 넣지는 않으며, 검증한 subset으로 축소할 수 있다. 별도로 버전 고정할 CLDR plural 데이터와는 역할·포함 목록을 구분한다.

### 8.2 기존 threaded/compatible 구조를 유지한다

현재 Web의 소유권·지원 경계는 [work.md](work.md)를 따른다.

- 앱·layout·paint 및 텍스트 엔진의 소유자는 기존 render Worker이다.
- 로케일/번역 도입을 위해 main에 두 번째 managed runtime을 만들지 않는다.
- locale 목록은 기존 host configuration transport로 전달한다. 동기 DOM 왕복을 paragraph의 매 호출에 추가하지 않는다.
- 새 native text 모듈은 threaded와 compatible을 각각 빌드·연결·실행 검증한다. 같은 WASM archive가 양쪽 ABI에 맞는다고 가정하지 않는다.
- module heap, allocator, native handle 및 ICU 초기화 데이터의 수명은 해당 runtime/module owner를 따른다.
- subtree/하위 경로 배포 시 자산 기준 URL과 runtime 기준 URL을 구분하고, 두 publish variant의 데이터 버전과 해시를 검사한다.
- `net11` threaded는 현재 제품 차단 경계를 유지한다. 국제화 연구를 이유로 runtime/SDK pin을 바꾸거나 지원 완료로 간주하지 않는다.

### 8.3 고정해야 할 버전

| 항목 | 정책 |
| --- | --- |
| .NET globalization | 기본 포맷 계약의 기준. runtime/SDK, OS·실제 ICU/NLS 등 경로와 로드 데이터 파일을 기록. OS ICU는 runtime pin만으로 고정되지 않음을 명시 |
| 위젯 포맷 정책 | culture override·calendar·hour cycle·허용 입력 패턴과 어댑터 버전을 기록 |
| 추가 CLDR 규칙 | 실제 포함하는 plural/compact 등 보강 데이터만 release·출처·hash를 고정. 기본 날짜·숫자 데이터는 복제하지 않음 |
| Flutter API/위젯 번역 baseline | 현재 저장소의 `56b8e1a851a594b1a154f8ea93270807dab22b9a`를 추상 API·리소스 inventory 기준으로 사용. 기본 포맷 결과의 oracle은 아님 |
| Dart intl 비교 자료 | baseline pubspec의 `^0.20.3`은 참고 범위일 뿐 기본 제품 의존성 아님. 개별 보강 구현을 참고·이식할 때만 실제 source revision·hash를 고정 [F7] |
| Unicode/ICU/Skia/HarfBuzz | 추가 text backend를 도입할 경우 실제 빌드 버전·옵션·ABI를 기록 |
| 생성기/ARB schema | generator 버전과 지원 문법·포맷 기능 목록을 산출물에 기록 |

검토일의 CLDR 최신 명세는 49이고 참고한 Dart intl 숫자 데이터는 CLDR 48 생성 표기를 갖는다. 이는 .NET이 같은 버전을 사용한다는 뜻이 아니다. .NET·OS 업데이트에 따른 결과 변경과 추가 CLDR 규칙 업데이트를 각각 검증하며, **특정 Flutter 데이터 버전에 모든 포맷을 맞추기 위해 .NET 데이터를 대체하지 않는다.** [I4], [U2]

현재 Flutter 문서는 Material/Cupertino의 새 패키지 이동도 설명한다. 최신 master의 번역·생성 코드를 현재 Doroti 추상 API에 무조건 덮어쓰지 말고, 고정 baseline의 멤버 inventory부터 맞춘다. [F1]

## 9. 단계별 실행 계획

### P0. .NET 기반 계약과 데이터 검증

**작업**

- 위젯이 호출하는 포맷 API·리소스 멤버를 추출하고, .NET 재사용/어댑터 정책/추가 구현으로 나눈다.
- `en-US`/`ko-KR`의 .NET culture·calendar·날짜/숫자/통화·strict parse·hour-cycle 계약과 지원 입력 범위를 확정한다.
- Windows/Linux와 net10 Web에서 실제 globalization 경로·ICU 데이터 파일·culture 전환을 확인한다. 영어 시작 후 한국어 전환도 인수한다.
- cardinal plural의 고정 CLDR 출처·지원 locale·precision을 정하고, 작은 evaluator의 AOT/trim·스레드 안전성을 확인한다.
- 기존 영어 앱과 .NET 어댑터 시범 앱의 크기·최초 localized frame·포맷 비용 baseline을 기록한다. Dart intl 포팅이나 새 native ICU는 이 단계의 요구가 아니다.

**통과 조건:** .NET 기반으로 충족하는 기능과 추가 보강 목록, 필요한 culture 데이터, 플랫폼별 허용 차이가 명확해야 한다. 지원 culture의 데이터 누락·불변 모드·구성 오류를 영어 fallback으로 숨기지 않는다. native paragraph 타당성은 P4에서 독립 판정한다.

### P1. 로케일 계약 정리

**선행:** P0의 기준 확정.

- 공통 태그 정규화와 host의 language 목록 전달을 구현한다.
- resolved locale에서 명시적 .NET culture를 만드는 경로와 neutral/unsupported culture 정책을 일원화한다.
- 기존 resolver callback/기본 fallback/delegate 우선순위를 보존한다.
- locale 전환·subtree override·동시 view·연속 비동기 로딩의 테스트를 추가한다.

**통과 조건:** `zh-Hant`, script+region, alias, 브라우저 언어 변경을 올바르게 처리하고 다른 subtree/view의 locale이 오염되지 않는다.

### P2. 얇은 .NET Intl 어댑터 + 위젯 번역

**선행:** P0의 .NET 계약·데이터 검증, P1의 로케일 계약.

- 날짜·숫자·통화·percent·파싱을 .NET API에 연결하고, 위젯별 Gregorian/24시간/zero padding 정책을 적용한다.
- 없는 cardinal plural과 실제 필요한 필드 조합만 보강한다. compact/native digits 등 후속 기능은 지원 목록과 함께 별도 추가한다.
- Widgets/Material/Cupertino opt-in 리소스 패키지를 추가한다.
- 먼저 `en-US`/`ko-KR`로 시작하고 영어 기본 구현은 변경하지 않는다.

**통과 조건:** 번역·접근성 문구와 .NET 기반 위젯 포맷 계약을 검증한다. Flutter와 다른 포맷 출력은 허용 차이로 기록하되, 위젯의 표시/입력/달력 정책은 일치해야 한다. opt-in하지 않은 기존 영어 앱의 결과는 유지된다.

### P3. ARB 생성기와 앱 통합

**선행:** P2의 .NET 어댑터·plural API. parser 자체는 병행 개발 가능.

- ARB를 AdditionalFiles로 받아 타입 있는 API/delegate를 생성한다.
- 중복 키·잘못된 문법·placeholder 불일치·지원하지 않는 포맷을 위치가 있는 진단으로 보고한다.
- 지원하는 ARB 포맷 metadata를 .NET 어댑터 옵션에 명시적으로 매핑한다.
- 앱 프로젝트에서 생성하고 runner에서 그대로 소비한다.
- 증분 빌드, 오류 후 복구, clean checkout·패키지 소비 프로젝트를 검증한다.

**통과 조건:** Dart/Flutter 설치 없는 제품 소비 환경에서 생성·컴파일·AOT가 가능하며 런타임 메시지 컴파일이 없다.

### P4. 별도 트랙: 텍스트 엔진 타당성 및 전환

**선행:** 해당 text backend·script의 지원 요구 확정. P2/P3과 독립 트랙이며, 기본 .NET 포맷 도입의 선행 조건이 아니다.

- Windows와 net10 Web에서 paragraph 후보를 측정·그리기·hit test까지 연결하여 공식 API/native export·Skia ABI·데이터 중복을 판정한다.
- 기존 바이너리와 연결할 수 없거나 custom Skia 배포가 필요하면 제품 변경 전에 backend·범위를 재결정한다. 성공하지 않은 실험을 지원 완료로 계산하지 않는다.
- shaping·Unicode 분석과 동일 결과의 paint/geometry/semantics를 연결한다.
- mixed-direction, cluster 편집, 줄바꿈, font fallback, retained picture 수명을 검증한다.
- Windows → Web 양 모드 → 나머지 지원 플랫폼 순서로 native 패키징을 자격 검증한다.

**통과 조건:** 아랍어·Indic·태국어·emoji의 화면과 입력 동작을 함께 검증한다. 번역 패키지를 배포했다는 이유로 이 단계를 완료 처리하지 않는다.

### P5. 언어 확대와 배포 크기 최적화

**선행:** 확대할 기능의 P2/P3 인수. 전체 문자권 지원 선언에는 P4도 필요.

- 번역/plural registry와 .NET runtime ICU subset을 각각 최적화하고, 추가 필드 조합·compact/native digits 등을 실제 요구 순서로 확장한다.
- baseline 대비 성능·크기와 Unicode/CLDR 갱신 정책을 검증한다.
- 템플릿·샘플·패키지 문서·라이선스·migration 안내를 갱신한다.

**통과 조건:** 지원 로케일·포맷 API·텍스트 backend·플랫폼 조합을 실제 통과한 범위로만 공개한다.

## 10. 검증 계획과 완료 기준

이 절은 **향후 구현의 인수 계획**이다. 이번 문서 작성에서 테스트를 실행했다는 뜻이 아니다.

### 10.1 핵심 케이스

| 영역 | 필수 검증 |
| --- | --- |
| locale | 정확 일치, language/script/region, 우선순위 목록, alias, invalid/unsupported tag, subtree override, 다중 view |
| 리소스 | 사용자 delegate 우선, 미지원 locale, 누락 키, 연속 전환 역순 완료, load 실패·dispose, 첫 frame 보류 해제 |
| 날짜 | .NET profile별 윤년·월 경계·패턴, 엄격 파싱, 위젯 Gregorian 정책, 주 시작일, 12/24h와 AM/PM, 시간대 암묵 변환 방지 |
| 숫자 | .NET 의미론의 0/음수/큰 정수·반올림 경계, currency별 자릿수, 프랑스어 공백 종류, 인도식 grouping, 기본 Latin digits와 추가한 native digits의 구분 |
| plural | en/ko/ru/ar, `=0`/`=1` 우선순위, one/few/many/other, visible decimals·precision, 동시 호출 |
| 생성기 | 중복 JSON 키, brace/apostrophe escaping, 중첩 분기, 타입 불일치, 미지원 문법·포맷, 잘못된 C# 식별자, 증분 입력 삭제 |
| Unicode/편집 | surrogate, combining mark, ZWJ emoji, 한글 조합, Arabic 연결, Indic 재배치, Thai word boundary, mixed bidi, caret affinity |
| geometry | visual selection boxes·hit test·semantics의 일치, wrap/scale/font/locale 변경 캐시 무효화, immutable snapshot 수명 |
| 배포 | 실제 .NET globalization 경로·선택 ICU 파일, 영어 시작 후 한국어 전환, 누락·손상 데이터의 명시적 실패, 하위 URL, 오프라인 정적 리소스, NuGet 소비·trim·AOT |

기본 포맷은 고정한 .NET 검증 profile의 기대값과 위젯 의미론을 검사한다. OS/runtime 데이터가 다른 profile 사이에는 문서화한 출력 차이를 허용하고, 동일 문자열을 강제하지 않는다. .NET 데이터를 충분히 고정하지 못한 환경에 범용 문자열 golden을 약속하지 않는다. 추가 CLDR plural과 번역 생성은 고정 규칙·리소스 fixture로 검증한다.

profile 내 문자열 비교는 공백·방향 제어 문자까지 확인한다. NBSP/NNBSP나 bidi 제어 문자를 무조건 제거하여 통과시키지 않는다. Flutter 비교 결과는 차이 분석용으로 별도 기록하며, 기본 포매터를 Flutter 출력에 맞추기 위한 자동 수정 근거로 사용하지 않는다. 화면 테스트는 고정 폰트로 수행하되 포맷 문자열 비교와 픽셀 비교를 구분한다.

### 10.2 실행 행렬과 시간 제한

- 포맷/생성기: Windows와 Linux의 지원 .NET 런타임에서 공통 의미론을 검증하고, 정확한 문자열은 각 globalization profile의 기대값과 비교한다. 추가 plural/번역 fixture는 동일한 고정 데이터로 비교한다.
- Web: 현재 지원 기준인 SDK 10/net10, SDK 11/net10 각각 threaded/compatible의 Debug 및 Release/AOT.
- Native: WindowsAppSDK, Linux/Qt, Android 및 Apple의 지원 runner를 플랫폼별로 확인. 빌드 성공과 실제 실행을 구분한다.
- 브라우저: Chromium 기준 자동화 후 Firefox/WebKit 또는 실제 지원 브라우저로 확대한다. 실제 IME·접근성·모바일 입력은 headless 결과로 대체하지 않는다.
- 모든 테스트 프로세스는 **외부 1,200초(20분) timeout**으로 제한한다. runner가 멈추어도 전체 프로세스 트리가 종료되어야 한다.
- 수백 회 반복은 하지 않는다. 일반 회귀는 결정적 fixture로, 성능/수명 반복은 원칙적으로 30회 이내로 확인한다. 표준 Unicode conformance corpus의 개별 케이스 수와 같은 실험의 반복 횟수는 구분한다.
- 전체 빌드를 한 번에 묶어 timeout을 늘리기보다, 기존 저장소 정책대로 bounded 작업을 분리한다.

기존 [CaretGeometryRegression.cs](Doroti/tests/Doroti.Scroll.Tests/CaretGeometryRegression.cs) 등 관련 테스트를 확장한다. 현재 테스트가 일부 grapheme/geometry를 검증한다고 해서 복잡문자 shaping이 인수되었다고 간주하지 않는다.

### 10.3 성능·크기 게이트

아직 측정하지 않았으므로 예상 MB나 ms를 사실처럼 적지 않는다. P0에서 같은 폰트·기기·앱·입력으로 baseline을 만든 후 budget을 고정한다.

- compressed download/install size와 native/managed/data별 증가량.
- 첫 localized frame까지의 시간, locale 전환 지연, cold/warm format 비용.
- 짧은 UI 문구와 긴 혼합 문단의 layout/paint p50·p95, allocation과 retained memory.
- 호환 모드와 threaded 모드의 차이 및 반복 locale 전환 후 자원 회수.
- 초기 제안 budget은 기존 영어 workload의 layout/paint p95 회귀 10% 이내이다. 이를 초과하면 자동 승인하지 않고 개선 또는 명시적 budget 재결정을 한다.
- 복잡문자 workload는 기존 오동작 경로보다 빠른지만 보지 않는다. 정확성 통과 후 별도 절대 budget을 정한다.

## 11. 라이선스·출처·운영

- 기본 포맷은 .NET 배포 규칙을 따르며 Dart intl 소스·날짜/숫자 데이터 이식을 필수로 하지 않는다. Flutter 번역/생성 로직이나 개별 intl 보강 구현을 실제 이식하는 경우에만 해당 BSD 조건·저작권·변경 내역·원본 revision을 보존한다. 추가 CLDR 규칙과 ICU/HarfBuzz/Skia 바이너리는 실제 포함 버전의 조건을 각각 검토한다.
- 기존 [THIRD-PARTY-NOTICES.md](Doroti/THIRD-PARTY-NOTICES.md), [LICENSES](Doroti/LICENSES), SDK의 license registration 경로를 사용한다. Flutter 라이선스 하나가 모든 데이터·바이너리를 대신한다고 가정하지 않는다.
- 현재 notice 문서와 실제 package pin이 다를 수 있으므로, 새 배포 inventory는 실제 manifest/lock/native 산출물을 기준으로 작성한다. 이번 연구에서 기존 notice를 정리하는 변경은 하지 않는다.
- provenance에는 원본 URL/revision/hash, 추출 도구 버전, 로컬 수정, 데이터 subset과 재생성 명령을 남긴다. upstream 생성기가 공개되지 않은 데이터는 제공된 생성 결과의 출처를 고정하고, 재생성 가능성을 과장하지 않는다.
- 유지해야 할 결과와 결정은 추적되는 문서·migration/test fixture로 남긴다. [Doroti/artifacts](Doroti/artifacts)는 삭제 가능한 로컬 산출물이며, 삭제 후 원시 로그나 배포본이 남아 있다고 보고하지 않는다.

## 12. 최종 채택 결정

### 채택 사항

1. 기존 Localizations 구조를 유지하고 로케일 입력·정규화부터 보강한다.
2. **`.NET 우선 + 부족한 국제화 기능만 보강`**을 기본안으로 사용한다. `Doroti.Intl`은 명시적 culture·위젯 정책을 가진 얇은 .NET 어댑터이다.
3. Widgets/Cupertino/Material 번역은 현재 디자인 의존 방향에 맞는 opt-in 패키지로 제공한다.
4. ARB는 빌드 시 C#으로 생성하고 .NET 포맷 옵션과 최소 CLDR plural evaluator에 연결한다. 런타임 메시지 엔진과 중복 설정 체계는 만들지 않는다.
5. 기본 포맷의 Flutter 완전 일치는 요구하지 않는다. .NET·OS 데이터 차이와 위젯 calendar/hour-cycle 정책을 지원 계약·검증 profile에 명시한다.
6. ICU/paragraph 보강은 별도 타당성 게이트를 두고 shaping·paint·geometry를 함께 검증한다. 이를 기본 .NET 포맷 도입의 선행 조건으로 만들지 않는다.
7. 실제 runtime 데이터·보강 규칙·지원 범위·배포 크기를 관리하며 영어 기본 UX를 자동으로 바꾸지 않는다.

### 이번 단계에서 채택하지 않을 것

- Dart `intl` 전체 포팅과 날짜·숫자 CLDR 데이터의 중복 배포.
- 얇은 .NET 어댑터가 Dart intl의 모든 API·ICU skeleton·출력을 지원한다고 선언하기.
- `NativeDigits` 설정만으로 지역 숫자 지원을 선언하거나, 전역 CurrentCulture 변경으로 subtree locale을 구현하기.
- ICU4N 하나로 포맷·Unicode·렌더링이 모두 해결된다고 전제하기.
- .NET/Qt 내부 ICU를 공개 binding 계약 없이 공통 서비스로 끌어오기.
- 측정과 paint가 서로 다른 텍스트 알고리즘을 사용하는 혼합 backend.
- 모든 로케일 데이터·폰트·번역을 core에 무조건 포함하기.
- 런타임 실패를 영어 fallback·기존 renderer로 조용히 숨기기.
- 이 작업과 무관한 SDK/runtime major upgrade 또는 전체 렌더러 재작성.

**다음 구현 착수 단위는 P0이다.** .NET culture·포맷·달력 정책과 Web의 실제 ICU 데이터부터 검증한 뒤, 얇은 어댑터·번역·최소 plural 보강을 진행한다. paragraph 연결 가능성은 복잡문자 지원을 위한 별도 P4 트랙에서 판정한다. 이 결정 변경은 문서에 반영한 것이며 제품 코드를 구현했다는 뜻은 아니다.

## 13. 외부 근거

모두 2026-11-13에 확인한 공개 1차 자료이다. 움직이는 `main`/`master` 및 `latest` 문서는 연구 근거이지 제품 버전 pin이 아니다. 제품 도입 시에는 release·commit·archive hash를 다시 고정한다.

- **[F1] Flutter 국제화 가이드:** 기본 en-US, Global delegate, ARB/gen-l10n, locale·script 및 현재 패키지 이동 안내. <https://docs.flutter.dev/ui/internationalization>
- **[F2] 기준 Flutter의 MaterialLocalizations:** intl formatter 구성, 위젯 시간 정책, 언어 구현 상속 주의. <https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/packages/flutter_localizations/lib/src/material_localizations.dart>
- **[F3] 기준 Flutter의 WidgetsLocalizations:** textDirection, 지원 언어 및 delegate 로딩. <https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/packages/flutter_localizations/lib/src/widgets_localizations.dart>
- **[F4] 기준 Flutter의 gen-l10n:** ARB 로드·parser·타입 있는 코드 생성 경로. <https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/packages/flutter_tools/lib/src/localizations/gen_l10n.dart>
- **[F5] 현재 Flutter skwasm의 Unicode 경로:** [built-in ICU](https://github.com/flutter/flutter/blob/master/engine/src/flutter/skwasm/text/paragraph_builder_builtin_icu.cc), [client ICU](https://github.com/flutter/flutter/blob/master/engine/src/flutter/skwasm/text/paragraph_builder_client_icu.cc).
- **[F6] 기준 Flutter의 native paragraph:** SkParagraph와 SkUnicode ICU 연결. <https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/engine/src/flutter/txt/src/skia/paragraph_builder_skia.cc>
- **[F7] 기준 flutter_localizations 의존성:** intl은 `^0.20.3` 범위이다. <https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/packages/flutter_localizations/pubspec.yaml>
- **[I1] Dart intl 패키지:** 기능·초기화·로케일·독립 관리형 포매터 구조. <https://pub.dev/packages/intl>
- **[I2] DateFormat API:** ICU/JDK 패턴·skeleton, 초기화, 파싱 및 시간대 관련 제한. <https://pub.dev/documentation/intl/latest/intl/DateFormat-class.html>
- **[I3] intl plural rules 소스:** 생성 데이터, precision/visible digits와 평가 중 전역 상태. <https://github.com/dart-lang/i18n/blob/7475ce643ed6210e5e787b310bb5bc24de9a2158/pkgs/intl/lib/src/plural_rules.dart>
- **[I4] intl number symbols 데이터:** CLDR 48 생성 표기와 로케일별 패턴·숫자 기호. <https://github.com/dart-lang/i18n/blob/7475ce643ed6210e5e787b310bb5bc24de9a2158/pkgs/intl/lib/number_symbols_data.dart>
- **[I5] intl 라이선스:** <https://github.com/dart-lang/i18n/blob/7475ce643ed6210e5e787b310bb5bc24de9a2158/pkgs/intl/LICENSE>
- **[N1] .NET globalization과 ICU:** Windows ICU/NLS, OS 데이터 차이, app-local ICU. <https://learn.microsoft.com/en-us/dotnet/core/extensions/globalization-icu>
- **[N2] .NET 10 Blazor globalization:** ICU subset, 전체 데이터 및 BlazorIcuDataFileName. <https://learn.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?view=aspnetcore-10.0>
- **[N3] .NET WASM ICU 설계:** Wasm/Blazor 속성 구분, custom locale slicing과 fallback 데이터 주의. <https://github.com/dotnet/runtime/blob/main/docs/design/features/globalization-icu-wasm.md>
- **[N4] System.Globalization API:** culture, 날짜·숫자 형식, calendar, 문자소 처리 등 .NET 공개 기능. <https://learn.microsoft.com/en-us/dotnet/api/system.globalization?view=net-10.0>
- **[N5] NumberFormatInfo.NativeDigits:** 설정이 실제 포맷·파싱에는 적용되지 않는다는 제한. <https://learn.microsoft.com/en-us/dotnet/api/system.globalization.numberformatinfo.nativedigits?view=net-10.0#remarks>
- **[U1] ICU MessageFormat:** plural/select, nesting, quoting, skeleton과 MessageFormat 2의 구분. <https://unicode-org.github.io/icu/userguide/format_parse/messages/>
- **[U2] UTS #35 / CLDR 49 Numbers:** 숫자 형식과 plural 명세. <https://www.unicode.org/reports/tr35/tr35-79/tr35-numbers.html>
- **[A1] ICU4N upstream README:** ICU4J 60.1 부분 포팅, 진행 상태와 resource 배포. <https://github.com/NightOwl888/ICU4N>
- **[A2] ICU4X upstream README:** Rust 구현, 모듈성·데이터 provider와 다언어 목표. <https://github.com/unicode-org/icu4x>
