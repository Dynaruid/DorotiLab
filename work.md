# Flutter Material 샘플을 통한 Doroti 기능 보완·확장 계획

- 작성일: **2026-09-06**
- 분석 기준: Doroti HEAD `d8efedd` 및 현재 로컬 reference source.
- 상태: **사전 소스 검토 완료 / 샘플 이식 구현 미착수 / 샘플 실행 검증 notVerified**.
- 최초 계획 작성: 사전 검토와 계획을 작성하고, 사용자 의도에 따라 **샘플 구현 과정에서 Doroti의 미흡·누락 기능을 고치고 만드는 것**을 주목적으로 명시했다. 당시 제품·테스트·Flutter source는 변경하지 않았고 build/앱 실행/성능 측정은 하지 않았다.
- 후속 범위 정리: 참조 앱과 Doroti Material을 Material 3 전용으로 정리하고 이 계획의 구성·테마 조작·비교 조건도 이에 맞췄다. 아래 샘플 이식 단계의 완료를 의미하지 않는다.
- 참조 경로: 요청의 `reference\flutter\_sample\_app`는 현재 checkout에 없다. 실제 존재하고 요청 내용에 해당하는 **[reference/flutter_sample_app](reference/flutter_sample_app/README.md)** 기준이다.
- [plan.md](plan.md)의 cross-platform 부트 개선 계획은 별도 범위로 유지한다.

## 1. 결론과 작업 범위

**이 작업의 주목적은 Flutter Material 샘플을 실제 사용·검증 기준으로 삼아 Doroti의 미흡한 구현을 고치고 누락 기능을 완성하는 것이다.** `DorotiTestbedApp`의 샘플 구성은 문제를 발견하고 수정 결과를 계속 확인하는 제품 사용 사례다. Framework·Runtime·Ui·Rendering·Host의 필요한 공용 변경을 본 작업의 정식 범위로 포함한다.

네 화면과 Material 데모를 C#으로 구성하면서 드러나는 제약은 해결할 개발 항목으로 다룬다. 현재 API만으로 지원되지 않는다는 이유로 해당 기능을 제외하지 않는다. 최종 결과는 샘플 화면과 함께 **다른 Doroti 앱에서도 사용할 수 있는 공용 API·구현·회귀 검증**을 갖춘 상태다.

핵심 제약은 이미지 기반 색상 추출의 실제 픽셀 생성·읽기 부재, 간소화된 이미지 색상 선정 알고리즘, SearchAnchor 기본 인자의 null 처리, 외부 URL 실행 연결 부재다. 나머지 위젯도 클래스 존재와 실제 실행·입력·시각적 일치를 구분해야 한다.

앱 측 산출물은 `lib/main.dart`의 Material 데모를 기본 화면으로 구성한 Testbed다. `lib/differential_main.dart`와 `lib/resize_fixture.dart`는 기존 진단 reference이며 새 데모의 기준 화면이 아니다. 제품은 C# 전용 구조를 유지하고 Dart project를 Testbed 내부에 만들거나 framework 전체를 재변환하지 않는다.

초기 이식 단계와 전체 완료를 구분한다. 이미지/URL 등이 남으면 **PARTIAL**이며, 버튼을 제거하거나 고정 색상을 넣어 전체 기능 완료로 판정하지 않는다. 기본 적용은 아래 전환 gate를 통과한 뒤 수행한다.

### 1.1 모든 단계에 적용할 개발 방식

1. 샘플의 기능을 Doroti public API로 구성하고 Flutter의 기대 동작과 비교한다.
2. 실패하면 앱 조립 문제, 공용 계약 결함, 누락 구현, 플랫폼 capability 차이 중 원인을 식별한다. 아래에서 이미 찾은 항목에 한정하지 않고 진행 중 발견한 관련 결함도 범위에 추가한다.
3. 공용 결함은 Testbed 상태나 특정 화면에 의존하지 않는 최소 재현을 만든다. 실패 조건과 기대 결과를 먼저 기록한다.
4. 해당 소유 계층에서 수정·구현한다. 공용 상태·정책·자원 계약을 먼저 정하고 OS/브라우저 의존 부분만 host adapter로 분리한다.
5. 최소 재현과 영향받는 기존 검증을 통과시킨 후 Testbed에서 실제 기능·입력·시각 결과를 다시 확인한다. 지원 target별 결과를 별도로 기록한다.

앱에는 화면 구성·데모 데이터·사용자 선택 상태를 둔다. reusable widget 동작, image readback, 색상 알고리즘, focus/overlay, URL 실행 계약은 공용 구현에 둔다. 공용 수정은 Testbed의 내부 클래스나 전용 label·환경변수에 의존해서는 안 된다.

진행 중 임시 미지원 표시가 필요하더라도 결함 목록에는 미완료로 남긴다. public API에 필요한 변경이 생기면 영향받는 호출부·문서·관련 template 사용까지 함께 점검한다. 샘플과 그 기반 계약에서 드러나는 문제를 해결하되, 무관한 Flutter 전체 API 이식이나 별도 부트 최적화까지 자동 확장하지 않는다.

## 2. 참조 앱에서 재현할 내용

| 영역 | 실제 reference 동작 | Doroti 목표 |
| --- | --- | --- |
| 앱 상태 | Material 3 전용, system brightness, M3 Baseline seed | root StatefulWidget에서 brightness·seed/image 선택을 소유 |
| 테마 조작 | 밝기 토글, 9개 seed, 6개 네트워크 이미지 | 같은 선택지·선택 표시·theme 갱신. 이미지 로딩/실패 처리 추가 |
| Home | Components / Color / Typography / Elevation | 같은 4개 destination과 화면별 스크롤 |
| 반응형 Home | **폭 ≤1000**: bottom bar·단일 목록, **1000<폭≤1500**: rail·두 목록, **폭>1500**: extended rail·확장 설정 | 논리 크기 기준으로 경계와 전환 방향 재현 |
| 전환 | controller 1000ms, bar interval 0–0.5, rail interval 0.5–1.0, OneTwoTransition | 초기 크기에서는 완료 상태로 시작, resize 중 reverse와 dispose 처리 |
| 설정 영역 | extended rail 설정은 높이 >740이면 일반 배치, 그 이하는 scroll | 낮은 높이에서도 설정 접근 가능 |
| Color | content 폭 <500이면 role chip 목록, ≥500이면 폭 902의 SchemePreview를 FittedBox로 축소. Light/Dark 모두 표시 | `scheme.dart`, `color_box.dart`까지 이식. 선택한 primary로 생성하는 reference 흐름 유지 |
| Typography | Display/Headline/Title/Label/Body 각 Large/Medium/Small, 총 15개 | TextTheme의 실제 style과 onSurface 사용 |
| Elevation | tint only / tint+shadow / shadow only, 각 6단계 | 0/1/3/6/8/12dp, 표시값 0/5/8/11/12/14%, content 폭 <450에서 3열, 나머지 6열 |

`constants.dart`의 450px navigation 설명과 일부 test 설명보다 **home.dart의 실제 `>1000`, `>1500` 분기**를 기준으로 삼는다. Color의 주석 역시 실제 분기와 구성이 다르므로 실행 코드를 따른다. Elevation은 전체 창 폭이 아니라 sliver의 crossAxisExtent를 사용한다.

Components의 범위는 다음과 같다. 단순 대표 위젯 몇 개로 축소하지 않는다.

| 그룹 | 포함 예제 |
| --- | --- |
| Actions | Elevated/Filled/Filled tonal/Outlined/Text 버튼 및 icon 변형, FAB 크기·extended, icon toggle, 단일·복수 SegmentedButton |
| Communication | badge navigation, circular/linear progress, snackbar 및 Close |
| Containment | modal/non-modal bottom sheet, Elevated/Filled/Outlined card, Carousel 2종(각 20개, snapping 차이), 일반/fullscreen dialog, divider |
| Navigation | bottom app bar, navigation bar/drawer/rail 예제, modal end drawer, tabs, 검색·제안·검색 이력, top app bar 변형 |
| Selection | checkbox/list tile, chips, date/time picker, 메뉴·하위 메뉴·DropdownMenu, RadioGroup, slider, switch 및 enabled/disabled 상태 |
| Text inputs | filled/outlined/disabled, error·helper·prefix/suffix·clear 등 `TextFields`의 각 상태 |

참조에서 원래 빈 callback인 전시용 버튼은 동일한 데모 범위로 유지한다. 반면 값을 바꾸거나 overlay를 여는 예제를 무동작 callback으로 대체하지 않는다. `dynamic_color`는 Color 화면의 안내 링크일 뿐 실제 plugin 사용이 아니므로 OS wallpaper 기반 dynamic color 구현은 이번 범위가 아니다.

## 3. 현재 소스에서 확인한 공용 보완 과제

### 3.1 이미지 기반 색상 — 확정된 선행 차단 요인

호출 경로:

```text
App._handleImageSelect
  -> ColorScheme.fromImageProvider(NetworkImage)
  -> _imageProviderToScaled (최대 112px)
  -> Picture.toImage
  -> Image.toByteData(rawRgba)
  -> QuantizerCelebi / Score
  -> MaterialColorSchemeRuntime
```

- [PaintingTypes.cs](Doroti/src/Doroti.Ui/PaintingTypes.cs)의 `Picture.toImage`는 현재 명령을 rasterize하지 않고 크기만 가진 `new Image(...)`를 반환한다.
- [GraphicsAndSemanticsContracts.cs](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs)의 `Image.toByteData`는 현재 구현에서 항상 `DorotiCapabilityException`을 던진다. 네트워크 이미지를 화면에 표시할 수 있더라도 색상 추출은 별개다.
- [color_scheme.cs](Doroti/src/Doroti.Framework.Material/color_scheme.cs)의 `QuantizerCelebi`는 raw pixel별 빈도 집계이며 `maxColors`에 따른 실제 quantization을 하지 않는다. `Score`도 빈도순 선택이다. 픽셀 경로만 고쳐도 Flutter의 이미지 seed 선택과 같아지지 않는다.
- 같은 파일의 `_imageProviderToScaled`는 async listener에서 resize 후 완료한다. 예외 전파·임시 Image/Picture 해제·timeout 경계를 함께 검토해야 한다.
- seed 기반 색상은 [MaterialColorSchemeRuntime.cs](Doroti/src/Doroti.Runtime/MaterialColorSchemeRuntime.cs)의 HCT/role 계산 경로가 이미 있다. **이미지 quantizer 문제를 seed 생성 전체 미구현으로 확대해서 판단하지 않는다.** 색상 일치는 별도 differential로 확인한다.

처리: 공용 picture rasterization/readback과 byte format 계약을 완성하고 quantizer/score를 reference와 대조한다. Web은 UI/Raster Worker 사이 이미지 소유권·응답·해제까지 포함한다. 앱에서 고정 seed로 대신하는 것은 임시 미지원 상태일 뿐 완료 구현이 아니다.

### 3.2 SearchAnchor — 기본 호출과 충돌하는 null 처리

- 샘플의 `SearchAnchor.bar`는 hint와 suggestionsBuilder만 넘기며 trailing/open/close callback을 생략한다.
- [search_anchor.cs](Doroti/src/Doroti.Framework.Material/search_anchor.cs)의 `_SearchAnchorWithSearchBar__search_anchor`는 `barTrailing.Cast<Widget>()`와 `viewOnOpen: () => onOpen()`, `viewOnClose: () => onClose()`를 사용한다. 해당 인자는 null 기본값이므로 기본 구성 또는 열기/닫기에서 예외가 발생할 경로가 있다.
- 같은 factory/constructor의 scrollPadding·contextMenuBuilder 기본값 전달도 확인 대상이다.

처리: 최소 재현으로 확인 후 공용 widget에서 optional 인자 계약을 수정한다. 앱에서 빈 배열·무동작 callback을 강제로 공급하는 것으로 문제를 감추지 않는다. 검색 입력, 제안 선택, 이력 재열기, Esc/뒤로가기와 focus 복귀까지 검증한다. 현재는 **source로 발견한 결함이며 실행 재현은 미수행**이다.

### 3.3 외부 링크와 네트워크

- Flutter는 [pubspec.yaml](reference/flutter_sample_app/pubspec.yaml)의 `url_launcher`를 이용해 `https://pub.dev/packages/dynamic_color`를 연다. Doroti `src`의 C#/TS에서 대응 URL launcher 이름/호출을 찾지 못했다. **Flutter plugin을 직접 재사용할 수 없으며 host capability 연결 조사·추가가 필요**하다.
- 이미지 6종은 `flutter.github.io`의 원격 PNG다. [Painting project](Doroti/src/Doroti.Framework.Painting/Doroti.Framework.Painting.csproj)는 `_network_image_web.cs`를 제외하고 공용 `NetworkImageIo`를 사용한다. [HTTP runtime](Doroti/src/Doroti.Runtime/DartCollectionsAndConvert.cs)은 .NET HttpClient를 사용한다. 실제 Web Worker fetch/CORS, native network 설정, decode/등록 성공은 미검증이다.
- URL은 공용 서비스 계약과 플랫폼 구현으로 연결한다. Web에서는 Worker 요청을 main으로 전달하되 실제 클릭의 사용자 활성화와 popup 차단을 검증한다. 미지원 플랫폼은 실패 결과와 사용자 피드백을 반환한다.
- 이미지 선택에는 loading/error/retry, 화면 dispose 후 갱신 방지, 늦게 도착한 이전 요청이 새 선택을 덮지 않도록 revision을 둔다. 실패 시 마지막 성공 theme를 보존한다.
- 오프라인 색상 검증은 같은 이미지 bytes를 고정 fixture로 확보한다. 실제 네트워크 성공과 분리하며, 제품 offline bundle을 추가할 경우 출처/배포 조건을 먼저 확인한다.

### 3.4 위젯 이식과 런타임 검증 범위

`NavigationBar/Rail/Drawer`, `SegmentedButton`, `Badge`, `CarouselView`, `DropdownMenu`, `MenuAnchor`, `SearchAnchor`, `RadioGroup`, `SliverLayoutBuilder`는 현재 C# 클래스가 존재한다. `ColorScheme.CreateFromSeed`, `SearchAnchor.CreateBar`, `CarouselView` 등 Doroti factory/생성자에 맞춰 수동으로 이식한다. Dart mixin, named constructor, generic nullable/collection, callback/Future를 기계적인 문자 치환으로 옮기지 않는다.

다음은 누락 확정이 아니라 **추가 검증이 필요한 위험 영역**이다.

- 메뉴·dialog·picker·sheet: Navigator/Overlay/ScaffoldMessenger, localization, modal barrier, focus trap/restore, dismiss 결과, 화면 전환 후 controller dispose.
- `MenuAnchor` shortcut label은 `MenuSerializableShortcut` 밖의 activator에 명시적 제한이 있다. 샘플이 요구하는 shortcut만 대조하고 무관한 모든 activator 지원으로 확장하지 않는다.
- Components의 `BuildSlivers`/`_RenderCacheHeight`는 custom render object와 scroll extent 추정을 사용한다. 1열↔2열, theme/font/text scale 변경 시 height cache 유효성, 마지막 항목 접근, scrollbar 안정성을 검증한다.
- TextField/Dropdown/Search/Radio는 마우스뿐 아니라 Tab/Shift+Tab·방향키·Enter·Esc·한글 IME·selection·clipboard·semantics까지 실행 확인이 필요하다.
- 많은 widget, 진행 indicator, 전환 animation을 한꺼번에 추가하면 현재 단일 갤러리와 layout/raster 부하가 달라진다. 처음부터 전 화면을 eager 생성하지 않고 reference의 화면 생명주기를 기준으로 구성한다.
- Material 3 API 존재만으로 Flutter와의 시각적 일치를 확정하지 않는다. theme 변경 후 stale color/style/cache가 남는지도 확인한다.

### 3.5 기존 Testbed·테스트·시각 조건

- [App.cs](DorotiTestbedApp/src/App.cs)는 현재 720×640의 단일 `MaterialGallery`와 shader/blur/backdrop/state 진단을 함께 갖고 있다. `DOROTI_RESIZE_FIXTURE=F0/F1/F2`, `DemoEntryMode.Builder/Home`, entrypoint 진단 속성을 보존해야 한다.
- [input-regression.spec.ts](Doroti/validation/web-playwright/tests/input-regression.spec.ts)와 [flutter-differential.spec.ts](Doroti/validation/web-playwright/tests/flutter-differential.spec.ts)는 기존 G6 label/워크로드에 의존한다. [g6-material-reference.json](DorotiTestbedApp/g6-material-reference.json)은 기존 720×640 좌표 기준이다. 새 화면의 screenshot baseline으로 재사용할 수 없다.
- 현재 Windows 앱은 Acrylic 및 투명 배경/수정 palette를 요청한다. 참조 앱은 일반 Material surface를 사용하므로 기존 DemoTheme를 그대로 쓰면 색상·Elevation 비교가 달라진다. 새 sample은 opaque reference theme를 사용하고, 기존 Acrylic/Shader 검증은 diagnostics 모드로 유지한다.
- 참조는 NanumGothic을 asset으로 선언하지만 `main.dart`에서 기본 fontFamily로 지정하지 않는다. Doroti Web Worker는 NanumGothic fallback을 등록한다. 폰트가 같다고 가정하지 말고 target별 실제 family/weight/fallback, MaterialIcons glyph를 맞춘 후 text geometry를 비교한다.
- 현재 source 기본값은 **Windows Vulkan**, **Web worker-canvaskit-webgl**이다. 이번 화면 이식으로 renderer 기본값을 바꾸지 않는다. 과거 ANGLE/다른 Web 경로 기록은 당시 조건으로 보존한다.
- Flutter samples source를 각색한 C# 파일은 원 저작권 헤더와 [LICENSE](reference/flutter_sample_app/LICENSE)를 보존하고 배포 notice에 출처를 기록한다.

## 4. 파일 구성과 변경 소유권

아래 새 경로는 제안이며 아직 생성하지 않았다.

| 파일/소유자 | 현재 → 변경 | 검증 |
| --- | --- | --- |
| `DorotiTestbedApp/src/App.cs` | bootstrap·gallery 혼합 → bootstrap/모드 선택과 view 설정 | 기본 진입, diagnostics, F0/F1/F2 각각 진입 |
| `src/Diagnostics/LegacyMaterialGallery.cs` (신규) | 기존 gallery/theme/진단 동작 보존 | 기존 label·state·pixel fixture 회귀 |
| `src/MaterialSample/SampleApp.cs`, `SampleState.cs`, `SampleConstants.cs` (신규) | root theme 상태와 9 seed/6 image/4 destination | 변경 알림·system/manual theme·async 선택 |
| `src/MaterialSample/Home.cs`, `Transitions/` (신규) | reference home 및 rail/bar/one-two 전환 | 경계 크기·animation reverse·dispose |
| `src/MaterialSample/Screens/`, `Components/`, `ThemeActions/` (신규) | 4개 화면, 6개 component 그룹, seed/image 설정 | reference 항목별 표시와 상호작용 |
| `Doroti.Framework.Material/search_anchor.cs` | optional 인자 전달 보완 | 최소 인자 SearchAnchor 생성·open/close |
| `Doroti.Ui/PaintingTypes.cs`, `GraphicsAndSemanticsContracts.cs` 및 renderer/host | Picture→Image→bytes의 실제 capability | 작은 알려진 pixel fixture, format/크기/dispose 오류 |
| `Doroti.Framework.Material/color_scheme.cs` 및 공용 color runtime | quantization/score와 async lifecycle | 동일 이미지의 선택 seed·role ARGB differential |
| 공용 hosting/service 및 target host | URL launcher adapter | 실제 사용자 클릭·실패 응답·target 분리 |
| `Doroti/validation/web-playwright/` 및 공용 validation | 기존 diagnostics 테스트 모드 고정 + 새 sample 시나리오 | 서로 다른 baseline/워크로드 유지 |
| `DorotiTestbedApp/README.md`, `README.ko.md`, third-party notices | 모드/실행/현재 지원 범위 기록 | 명령과 구현 일치 |

공용 framework 결함은 그 소유 계층에서 수정한다. gallery만을 위한 예외 처리, 숨은 대체 renderer, 고정 이미지 palette로 parity를 가장하지 않는다. startup 최적화는 `plan.md`에 맡기고 여기서는 추가 화면의 생성·상태·자원 비용을 제어한다.

### 4.1 공용 결함·누락 추적표

구현 중 이 표를 갱신한다. 항목별로 최소 재현/실패 증거, 수정 파일·공용 계약, 회귀 결과, Testbed 통합 결과, target별 남은 검증을 연결한다. 표의 현재 상태는 실행 검증 결과가 아니다.

| ID | 기대하는 공용 기능 | 현재 근거/상태 | 소유 계층·완료 증거 |
| --- | --- | --- | --- |
| D01 | Picture 명령을 실제 Image로 rasterize | 크기만 가진 Image 반환, source 확인 | Ui + Rendering/Host; 알려진 그림을 독립 fixture에서 rasterize하고 픽셀 비교 |
| D02 | Image rawRgba 읽기와 자원 수명 | toByteData가 항상 capability 예외, source 확인 | Ui + Rendering/Host; D01과 연결, format/크기/dispose 및 Worker 응답 검증 |
| D03 | Flutter와 대응하는 이미지 quantization/score | 빈도 집계·정렬 구현, source 확인 | Material + color runtime; 동일 입력 seed/role differential |
| D04 | optional 인자를 생략한 SearchAnchor 기본 동작 | null 처리 결함 경로, 실행 재현 필요 | Material/Widgets; 최소 public API 호출로 생성·열기·닫기·focus 복귀 |
| D05 | target 공통 URL 실행 API와 플랫폼 동작 | 대응 연결 미발견, 재사용 계약 조사 필요 | 공용 service + Host; 독립 호출 및 실제 사용자 클릭의 성공/실패 |
| D06+ | 이식 중 드러나는 theme/layout/scroll/input/semantics 등 | 발견 시 구체 항목으로 추가 | 원인 소유 계층; 최소 재현 → 공용 수정 → 회귀 → Testbed 검증 |

`source 확인 → 실행 재현 FAIL → 수정 중 → 공용 회귀 PASS → Testbed 통합 PASS` 순으로 증거를 남긴다. 환경 부족은 `notVerified`로 기록하며 수정 완료나 미지원 확정으로 바꾸지 않는다.

## 5. 순서 있는 작업 계획

P0–P7은 통합 milestone이다. **각 단계에서 발견한 공용 결함의 재현·수정·검증을 그 기능의 완료 조건에 포함한다.** 의존 기능이 막히면 그 기반부터 고치고 이어간다. 예를 들어 D04는 P3의 검색 연결 전에, D01/D02/D03은 P4의 이미지 theme 연결 전에 완료한다. 독립적인 화면 구성은 진행할 수 있지만 실패한 기능을 생략한 채 해당 milestone을 완료하지 않는다.

### P0 — 기준 고정과 최소 위험 재현

- [ ] reference `main.dart`, `lib/src`, pubspec/lock, 사용 Flutter SDK revision과 font/image 입력 hash를 기록한다. reference는 현재 로컬 내용 그대로 기준으로 삼는다.
- [ ] 4개 화면/6개 그룹의 항목·상태·callback 목록과 Doroti API 대응표를 작성한다. 이번 정적 확인을 runtime PASS로 기록하지 않는다.
- [ ] 현재 Testbed의 Windows/Web 대표 실행, G6 입력 및 resize fixture 기준 상태를 확보한다. 기존 FAIL을 보존한다.
- [ ] SearchAnchor 최소 인자 생성/open/close, Picture→Image→bytes, 이미지 quantizer를 작은 독립 fixture로 재현한다.
- [ ] URL adapter에 재사용 가능한 기존 service/host 계약을 더 조사하고 실제 수정 범위를 확정한다.
- [ ] D01–D05의 최소 재현과 의존 관계를 기록하고, 진행 중 발견하는 공용 결함을 같은 추적표에 등록한다.

완료 조건: 확정 blocker와 실행 미검증 항목이 구분되고, 최소 재현과 각 수정 소유자가 결정됨. P0 실패가 있으면 해당 기능의 선행 보완부터 진행한다.

### P1 — 기존 진단 보존과 새 앱 골격

- [ ] 기존 MaterialGallery를 diagnostics 파일로 분리하되 label/상태/entrypoint contract를 유지한다.
- [ ] 명시적 sample/diagnostics 모드 선택을 추가한다. 예: 신규 `DOROTI_TESTBED_MODE=sample|diagnostics`; 실제 env/config 전달은 native와 Web 양쪽에서 검증한다. 기존 `DOROTI_RESIZE_FIXTURE`를 최우선으로 유지한다.
- [ ] 새 sample root의 StatefulWidget/theme state와 4개 destination을 구성한다. 앱 기본값 전환은 P7에서 수행한다.
- [ ] sample의 opaque surface와 diagnostics의 Acrylic view 설정을 같은 모드 선택에 연결한다. 무관한 host 기본값은 변경하지 않는다.
- [ ] 기존 자동화는 diagnostics 모드를 명시하도록 먼저 갱신한다.

완료 조건: 두 모드와 resize fixture가 각각 의도한 root를 표시하고 기존 테스트 대상을 잃지 않음.

### P2 — 기본 테마와 네 화면의 정적 내용

- [ ] Material 3 전용으로 system 초기값·light/dark 토글·9개 seed를 구현한다. theme cache가 선택 변화에 반응하도록 한다.
- [ ] Home 1열/2열/extended rail 및 action 배치를 구현한다. animation 전에는 최종 geometry를 먼저 검증한다.
- [ ] Color의 chip/SchemePreview, Typography 15종, Elevation 3×6 card를 이식한다.
- [ ] reference와 같은 role 값, 표시 문자열, spacing, typography, enabled/disabled style을 대조한다.

완료 조건: 네 destination이 동작하고 theme/seed가 모든 화면에 반영됨. 이 단계만으로 전체 앱 완료를 선언하지 않는다.

### P3 — Components 전체와 공용 widget 보완

- [ ] Actions → Communication → Containment → Navigation → Selection → Text inputs 순서로 이식한다.
- [ ] SearchAnchor의 null/default 인자 문제를 공용 source에서 고치고 재현 fixture를 통과시킨 뒤 검색 화면을 연결한다.
- [ ] snackbar/sheet/dialog/drawer/picker/menu의 open/close·취소/확정·focus 복귀와 controller dispose를 구현한다.
- [ ] 검색 suggestions/history, 메뉴 선택, chip 삭제, radio/segmented 다중 선택, carousel snapping 등 상태 변화를 대조한다.
- [ ] Sliver height cache와 두 목록의 scroll ownership, 입력 focus 순서를 재현한다. 폭 변경으로 cache가 stale해지면 원인을 공용/앱 소유 경계에 맞춰 수정한다.

완료 조건: 그룹별 누락이 없고 reference에서 동작하는 callback이 실제 상태·화면에 반영됨. Framework exception/overflow는 해당 항목 FAIL로 남김.

### P4 — 이미지 색상·URL 완성

- [ ] 공용 Picture rasterization 및 Image rawRgba readback을 구현한다. 크기·stride·RGBA/ABGR·premultiplied alpha·종료/dispose 계약을 명시한다.
- [ ] Windows와 기본 Web Worker 경로에서 동일 pixel fixture를 읽고, 나머지 host는 구현/미지원 상태를 각각 기록한다.
- [ ] Celebi quantization 및 scoring을 고정 reference와 대조해 보완한다. 기존 HCT seed→role 경로는 재사용하되 역할별 색상을 검증한다.
- [ ] 6개 이미지의 thumbnail·색상 선택·light/dark 전환과 loading/error/retry/latest selection 처리.
- [ ] URL 실행 공용 adapter/target 구현과 Color 안내 링크를 연결한다. Web popup과 native shell 실패 결과를 검증한다.

완료 조건: 실제 이미지 bytes로 추출한 theme가 생성되고 여섯 선택이 성공함. 네트워크 차단/재시도/연속 선택에서도 마지막 유효 상태 유지. 임시 고정 palette나 미지원 버튼 상태는 P4 미완료다.

### P5 — 반응형 motion·시각·입력 정합

- [ ] Bar/Rail/OneTwo transition과 reverse를 연결한다. resize 중 state·focus·scroll 손실 및 마지막 frame geometry를 확인한다.
- [ ] Home 폭 999/1000/1001, 1499/1500/1501과 Color content 폭 499/500/501, Elevation crossAxisExtent 449/450/451, 확장 action 높이 739/740/741을 확인한다.
- [ ] 대표 전체 viewport 390×844, 800×900, 1280×900, 1600×1000에서 overflow/스크롤 끝/설정 접근을 확인한다. threshold는 physical pixel이 아닌 logical/content 크기로 판정한다.
- [ ] Material 3의 light/dark 대표 화면을 고정 font/DPR/OS 조건으로 Flutter와 비교한다. 동일 seed의 Color role ARGB는 정확 일치를 목표로 하고 pixel AA/그림자 허용차는 영역별 근거를 기록한다.
- [ ] 실제 pointer hit test, keyboard traversal, IME 조합/취소, selection/clipboard, 접근성 label/selected/disabled 상태를 확인한다.

완료 조건: screenshot만 비슷한 상태가 아니라 표시와 hit test·focus·상태가 함께 맞음. animation 자동 측정과 실제 resize/스크롤 감각 결과를 분리함.

### P6 — 플랫폼별 검증

- [ ] Windows App SDK 기본 Vulkan, Web 기본 CanvasKit Worker를 우선 검증한다. ANGLE/다른 renderer는 관련 공용 수정에 필요한 회귀로 선택한다.
- [ ] Android, native AppKit macOS, Mac Catalyst, iOS, Linux는 각 runner의 build와 live 동작을 분리 기록한다. 해당 OS/device에서만 확인 가능한 입력/URL/readback/accessibility는 다른 target 결과로 대체하지 않는다.
- [ ] 새 sample validation은 reference `test/*.dart`의 항목·상태를 acceptance 자료로 활용한다. Flutter test 자체를 Doroti 테스트로 간주하지 않는다.
- [ ] `integration_test/integration_test.dart`는 `app.main()` 호출만 수행하므로 전체 상호작용 증거로 쓰지 않는다.
- [ ] 기존 G6/입력/resize 진단과 새 sample 결과를 각각 보관한다. 기존 `flutter-differential.spec.ts`의 counter workload는 새 Material sample 비교로 재해석하지 않는다.
- [ ] 추가·수정한 공용 API가 Testbed 내부 상태 없이 독립 fixture에서 사용되는지 확인한다. 관련 public API 변경은 다른 소비자와 template의 영향을 검증한다.

완료 조건: target/renderer별 build·live·visual·input·physical 결과가 있는 matrix 작성. 실행하지 못한 target은 **notVerified**이며 전체 플랫폼 parity는 미완료로 유지한다.

### P7 — 기본 화면 전환과 정리

- [ ] P1–P5 및 P6의 Windows/Web 대표 gate 통과 후 sample을 기본 진입점으로 바꾼다. diagnostics와 F0/F1/F2 진입은 보존한다.
- [ ] README 한·영에 기본 화면, 진단 실행, 네트워크 요구, 플랫폼별 잔여 항목을 갱신한다.
- [ ] adaptation source의 저작권/notice를 반영한다. 새 screenshot/상태 baseline에는 reference revision·viewport·DPR·font·theme·renderer를 기록한다.
- [ ] `work.md`에 실행 명령·증거 경로·PASS/FAIL/PARTIAL/notVerified를 갱신한다. 다른 플랫폼 미검증이 있으면 기본 전환 완료와 전체 parity 완료를 구분한다.

## 6. 검증 규칙과 최종 완료 기준

모든 테스트 프로세스는 [.github/copilot-instructions.md](.github/copilot-instructions.md)에 따라 **20분 timeout**을 적용한다. 구현 단계에서 공용 계약을 바꾼 영역의 기존 validation과 최소 결함 재현 테스트를 실행한다. 문서 작성만 한 이번 단계에서 build/실행 PASS를 만들지 않는다.

- 자동화: 화면·상태 inventory, 사용자 입력에 따른 결과, overlay lifecycle, 알려진 pixel readback, 이미지 seed/role 대조, font/layout/screenshot, Framework/runtime error 확인.
- Web 자동화는 기존 [run-web-playwright.ps1](Doroti/eng/run-web-playwright.ps1) 흐름과 Playwright browser를 사용한다. 실제 pointer click과 semantics 조작 결과를 구분한다.
- 실제 검증: 창 가장자리 resize/스크롤 체감, IME, OS URL 실행, screen reader/device 입력. 자동 capture·counter를 physical scan-out PASS로 간주하지 않는다.
- 성능은 동일 화면·입력·폰트·해상도·renderer 조건에서 측정한다. 불일치한 counter 앱과 새 갤러리 간 성능 수치를 비교하지 않는다.
- source에서 발견한 문제, 실행 재현 FAIL, 수정 후 자동 PASS, 실제 사용자 acceptance를 각각 기록한다.

최종 완료에는 다음 두 산출물이 모두 필요하다.

- **Doroti 공용 기능:** 샘플 구현 중 확인한 공용 결함·누락이 해당 소유 계층에서 해결되고, public API를 사용하는 독립 재현/회귀 검증이 남아 다른 앱에서도 같은 기능을 사용할 수 있다. D01–D05 및 이후 발견한 관련 결함의 미해결 항목을 숨기지 않는다.
- **Testbed 통합:** 4개 화면 + Components 6개 그룹 전체 + Material 3·brightness·9 seed·6 image + 반응형 전환 + 외부 링크가 보완된 Doroti 기능으로 동작하며 기존 diagnostics를 계속 실행할 수 있다.

샘플 기본 화면 전환은 중간 산출물의 배포 기준이다. 공용 결함이나 target별 검증이 남아 있으면 전체 작업은 PARTIAL/notVerified로 유지한다. 픽셀/입력 parity 및 전 플랫폼 완료는 각각의 검증 gate를 추가로 통과해야 한다.

현재 체크 상태는 모두 미착수다. 우선 실행할 작업은 **P0의 SearchAnchor·이미지 readback 최소 재현과 기존 diagnostics 기준 확보**다.
