# Flutter Material 샘플을 통한 Doroti 기능 보완·확장 계획

- 작성일: **2026-09-06**
- 분석 기준: Doroti HEAD `d8efedd` 및 현재 로컬 reference source.
- 상태: **P0 진행 / D01–D03 이미지 공용 경로 및 D04 기본 인자 회귀 PASS / reference Image demo 추가 / Doroti 샘플 이식 미착수 / 전체 PARTIAL**. 실행 결과와 한계는 §7–§8 참고.
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

아래는 최초 사전 검토 당시의 결함 기록이다. 2026-09-06 공용 수정 및 기본 인자 회귀 결과는 §7에 기록했으며, 실제 mounted open/close·focus 검증은 아직 남아 있다.

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

구현 중 이 표를 갱신한다. 항목별로 최소 재현/실패 증거, 수정 파일·공용 계약, 회귀 결과, Testbed 통합 결과, target별 남은 검증을 연결한다. 사전 분석과 후속 실행 증거를 구분한다.

| ID | 기대하는 공용 기능 | 현재 근거/상태 | 소유 계층·완료 증거 |
| --- | --- | --- | --- |
| D01 | Picture 명령을 실제 Image로 rasterize | 기존 크기-only Image FAIL → Skia/CanvasKit 실제 픽셀 fixture PASS | Ui + Rendering/Host; §8, 다른 OS live·복잡한 Picture 전체는 notVerified |
| D02 | Image rawRgba 읽기와 자원 수명 | 기존 capability 예외 FAIL → RGBA/straight/PNG·dispose/clone·Worker 응답 PASS | Ui + Rendering/Host; §8, 강제 restart 중 read 검증은 notVerified |
| D03 | Flutter와 대응하는 이미지 quantization/score | 기존 maxColors/gray/fallback FAIL → pinned Dart MCU palette·seed·46×2 roles 정확 일치 | Material + color runtime; §8, 각 host readback을 동일 입력으로 비교. host 간 decode 픽셀 동일성 아님 |
| D04 | optional 인자를 생략한 SearchAnchor 기본 동작 | 기본 enabled=false, null trailing/callback 실행 FAIL → 공용 수정 후 기본 인자 회귀 4/4 PASS | Material/Widgets; factory 및 route 전달 수정. **mounted open/close·focus 복귀·Testbed 통합은 notVerified** |
| D05 | target 공통 URL 실행 API와 플랫폼 동작 | clipboard/cursor의 view-scoped service 경로 재사용 가능; URL API는 없음 | 공용 Services + optional URL host capability 제안 확정; 실제 구현/사용자 활성화 검증 남음 |
| D06 | paintImage 기본 alignment | 실제 사진 처리 null 예외 → Alignment.center 기본값 복원, 사진 fixture PASS | Framework.Painting, §8 |
| D07 | ImageProvider codec 완료와 오류 전달 | MemoryImage timeout → MultiFrame constructor codec 연결·정상/ephemeral 오류 전달 복원, 실제 MemoryImage/NetworkImageIo PASS | Framework.Painting, §8; animated playback 전체 검증 아님 |
| D08+ | 이식 중 드러나는 theme/layout/scroll/input/semantics 등 | 발견 시 구체 항목으로 추가 | 원인 소유 계층; 최소 재현 → 공용 수정 → 회귀 → Testbed 검증 |

`source 확인 → 실행 재현 FAIL → 수정 중 → 공용 회귀 PASS → Testbed 통합 PASS` 순으로 증거를 남긴다. 환경 부족은 `notVerified`로 기록하며 수정 완료나 미지원 확정으로 바꾸지 않는다.

## 5. 순서 있는 작업 계획

P0–P7은 통합 milestone이다. **각 단계에서 발견한 공용 결함의 재현·수정·검증을 그 기능의 완료 조건에 포함한다.** 의존 기능이 막히면 그 기반부터 고치고 이어간다. 예를 들어 D04는 P3의 검색 연결 전에, D01/D02/D03은 P4의 이미지 theme 연결 전에 완료한다. 독립적인 화면 구성은 진행할 수 있지만 실패한 기능을 생략한 채 해당 milestone을 완료하지 않는다.

### P0 — 기준 고정과 최소 위험 재현

- [x] reference `main.dart`, `lib/src`, pubspec/lock, 사용 Flutter SDK revision과 font/image 입력 hash를 기록한다. reference는 현재 로컬 내용 그대로 기준으로 삼는다. → §7, `reference-inputs.json`.
- [x] 4개 화면/6개 그룹의 항목·상태·callback 목록과 Doroti API 대응표를 작성한다. 이번 정적 확인을 runtime PASS로 기록하지 않는다. → [inventory](Doroti/validation/fcr7-material-widget/material-sample-inventory.md).
- [x] 현재 Testbed의 Windows/Web 대표 실행, G6 입력 및 resize fixture 기준 상태를 확보한다. 기존 FAIL을 보존한다. → Windows startup smoke 4/4, Web 4/4; physical 검증 아님.
- [ ] SearchAnchor 최소 인자 생성/open/close, Picture→Image→bytes, 이미지 quantizer를 작은 독립 fixture로 재현한다.
- [x] URL adapter에 재사용 가능한 기존 service/host 계약을 더 조사하고 실제 수정 범위를 확정한다. → inventory의 D05; 구현은 P4.
- [x] D01–D05의 최소 재현과 의존 관계를 기록하고, 진행 중 발견하는 공용 결함을 같은 추적표에 등록한다. → §4.1/§7 및 inventory. D05는 API 부재로 source 조사이며 실행 재현으로 표시하지 않는다.

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

- [x] 공용 Picture rasterization 및 Image rawRgba readback을 구현한다. 크기·stride·RGBA/ABGR·premultiplied alpha·종료/dispose 계약을 명시한다. §8의 Skia/CanvasKit 범위.
- [ ] Windows와 기본 Web Worker 경로에서 동일 pixel fixture를 읽고, 나머지 host는 구현/미지원 상태를 각각 기록한다.
- [x] Celebi quantization 및 scoring을 고정 reference와 대조해 보완한다. 기존 HCT seed→role 경로는 재사용하되 역할별 색상을 검증한다. §8의 동일 readback 입력 differential.
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

## 7. 2026-09-06 P0 실행 및 D04 첫 공용 수정

**전체 PARTIAL, P0 진행 중.** 샘플 네 화면 이식/P1–P7과 기본 화면 전환은 아직 수행하지 않았다. 시작 checkout은 clean, HEAD `48343b7c8511c040faba8704e59aa32a80062374`였다. §3의 사전 source 분석과 이번 실행 증거를 구분한다.

### 7.1 구현과 고정 입력

- [MaterialSampleContracts.cs](Doroti/validation/fcr7-material-widget/MaterialSampleContracts.cs): Testbed에 의존하지 않는 public SearchAnchor 기본/명시 인자 검사, 2×1 red/transparent Picture readback, 내부 quantizer/Score 독립 재현을 추가했다. 내부 색상 경로는 public 이미지 경로가 막혀 reflection으로 한정 호출한다.
- [search_anchor.cs](Doroti/src/Doroti.Framework.Material/search_anchor.cs): CreateBar의 기본 `enabled=true`, nullable bar/view trailing의 typed 전달, optional lifecycle callback 직접 전달, context-menu 기본값의 실제 builder 전달을 수정했다. route 생성과 view content 생성 경로의 nullable trailing도 함께 수정했다. 앱 측 빈 callback/배열 우회는 추가하지 않았다.
- Search 회귀 4개를 기존 FCR-7 기본 실행에도 연결했다. 별도 `--material-sample`은 남은 이미지 결함을 그대로 FAIL/exit 1로 반환한다.
- [inventory](Doroti/validation/fcr7-material-widget/material-sample-inventory.md)에 4개 화면/6개 그룹의 상태·callback·공용 API·D01–D05 소유권/의존성을 기록했다. 실제 reference의 InputChip 삭제 callback은 빈 함수이므로 P3의 삭제 항목은 reference 범위를 확인해 처리해야 한다.
- [snapshot-material-sample.ps1](Doroti/eng/snapshot-material-sample.ps1)이 source/tests/pubspec/lock/LICENSE/font와 6개 PNG의 SHA-256을 기록한다. SDK는 sample package config가 가리키는 `C:/Users/parti/flutter`, revision `6b182d2c7585eba26d4edce0f97630effd256c33`; SDK `pubspec.lock`의 기존 수정도 기록한다. 이미지는 `.doroti` validation 입력이며 제품 bundle은 아니다. HTTP 다운로드 성공은 Web CORS/decode 성공을 뜻하지 않는다.
- [material-color-oracle.dart](Doroti/validation/fcr7-material-widget/material-color-oracle.dart)를 sample의 pinned `material_color_utilities 0.13.0`으로 실행했다. maxColors=1은 단일 cluster `{4290476659:3}`, 회색 100/빨강 10의 선택은 `0xffff0000`, 빈 score fallback은 `0xff4285f4`다. Doroti의 현재 세 결과는 모두 불일치한다.

### 7.2 실행 증거

로컬 evidence root: `.doroti/evidence/material-sample-p0/`. 모든 build/test 프로세스는 **20분 timeout**으로 실행했으며 소유한 앱·server는 종료했다.

| 검증 | 결과 | 증거/한계 |
| --- | --- | --- |
| 수정 전 독립 contracts | **FAIL 5** | `contracts-before-v2.stdout.log`: Search 기본값/null 3개 + Picture/readback + quantizer 실패, explicit options 1개 PASS. 최초 fixture compile의 Uint8List 원소형 오류는 `contracts-before.*`에 보존 |
| 수정 후 Search 기본 인자 | **PASS 4/4** | `search-final.stdout.log`; 생성/builder/callback 전달 검증이며 mounted route/input 검증 아님 |
| 기존 FCR-7 + Search 통합 회귀 | **PASS** | `integrated-regression/contracts.stdout.log`: Material 3, shortcut, context-menu 등 기존 회귀 포함 |
| 이미지/색상 blocker 최종 재현 | **FAIL 4** | `contracts-final.stdout.log`, `final-byte-view-contract/contracts.stdout.log`: readback 1, quantization/score 3; Search 4개는 PASS. ByteData view의 offset/length를 존중하도록 fixture를 보완한 후에도 동일한 결과. 실패를 기대 성공으로 바꾸지 않음 |
| Dart color oracle | **PASS 실행/결과 확보** | `color-oracle.json`, `color-oracle.stderr.log`; Doroti parity PASS 아님 |
| Windows Release build | **PASS**, warning/error 0 | `windows-build.*` |
| Windows Vulkan gallery/F0/F1/F2 startup smoke | **PASS 4/4**, exit 0 | `windows-{gallery,F0,F1,F2}-v2.json` 및 로그. 각각 visible-after-exact-present/terminal 보고, startup 자동 검증만 의미 |
| 최초 Windows hidden 직접 실행 | **실행 조건 FAIL 보존** | `windows-{gallery,F0,F1,F2}.*`: Hidden 때문에 visible-after-present 요구 실패. hidden CLI helper에서 정상 app launch로 수정한 v2만 유효 startup 기준 |
| Web Release build | **PASS**, warning/error 0 | `Doroti/validation/web-playwright/artifacts/wrapper/material-sample-p0-baseline/build.stdout.log` |
| 최초 Web 호출 | **실행 인자 FAIL 보존** | `material-sample-p0-baseline/playwright.*`: pwsh 외부 호출에서 두 test 경로가 comma 문자열이 되어 No tests found. 배열 직접 전달한 v2에서 실행 |
| Web CanvasKit Worker, Chromium hardware headless | **PASS 4/4** | `Doroti/validation/web-playwright/artifacts/wrapper/material-sample-p0-baseline-v2/playwright.stdout.log`; F0/F1/F2 direct/bitmap crop resize + G6 semantics/pointer/keyboard/native text endpoint. screenshot/diagnostics는 `artifacts/material-sample-p0-baseline-v2/test-results/` |
| 새 sample, mounted Search focus, physical resize/IME/accessibility, 다른 OS | **notVerified** | 기존 diagnostics 자동 PASS로 대체하지 않음 |

재실행:

```powershell
# 20분 timeout을 내장한 공용 회귀/결함 실행기. Blockers는 현재 FAIL이다.
./Doroti/eng/test-material-sample.ps1 -Suite Regression
./Doroti/eng/test-material-sample.ps1 -Suite Search
./Doroti/eng/test-material-sample.ps1 -Suite Blockers

./Doroti/eng/snapshot-material-sample.ps1 -FetchImages `
  -OutputDirectory .doroti/evidence/material-sample-inputs-new

./Doroti/eng/run-web-playwright.ps1 -HeadlessOnly `
  -RendererMode worker-canvaskit-webgl `
  -TestFile @('tests/input-regression.spec.ts', 'tests/canvaskit-resize-fixtures.spec.ts') `
  -ArtifactLabel material-sample-next-baseline -Port 5096
```

**다음 순서:** P0의 mounted SearchAnchor 생성→open→close→focus 복귀 fixture를 완료한다. D04는 현재 기본 인자 회귀만 PASS다. 이후 P1 diagnostics 분리/명시적 모드/root 골격을 진행하고, 이미지 연결 전에 D01/D02의 view/resource/Worker readback 계약과 D03의 실제 Celebi/Score를 구현한다. P7 이전까지 기존 기본 화면과 renderer 기본값은 유지한다.

## 8. 2026-09-06 이미지 공용 수리와 reference Image demo

사용자의 후속 요청으로 D01–D03을 먼저 구현했다. §7은 수리 전 이력이다.
이번 범위는 **공용 이미지 경로 및 reference 섹션 추가 완료**, 전체 P0–P7은
계속 **PARTIAL**이다. Doroti의 네 화면 이식·기본 진입점 전환은 수행하지 않았다.

### 8.1 공용 변경

- `Picture.toImage`가 활성 view의 image capability로 실제 rasterization을 요청한다.
  Skia는 별도 CPU surface, CanvasKit은 Raster Worker offscreen target을 사용한다.
  visible frame/resize ledger에는 넣지 않는다.
- `Image.toByteData`는 RGBA8 premultiplied/straight 및 PNG를 반환하고 비동기 읽기
  동안 storage clone을 보유한다. 크기·format·dispose 오류를 명시한다.
  CanvasKit 요청은 기존 session envelope, transferable buffer, 최대 16 pending,
  30초 timeout/port 종료 실패를 사용한다. 생성 PNG는 기존 resource journal에 보존한다.
- 실제 MCU Wu/Wsmeans/Score를 연결했다. 이전 NuGet Wu의 정수 오버플로·정수 나눗셈,
  centroid 반올림, Wsmeans 초기화/반복 정책 차이는 pinned Dart 0.13.0 기준으로 수정했다.
  이미지 bytes는 view 범위 안에서 RGBA→ARGB로 명시 변환하며, quantization 뒤의
  잘못된 채널 재변환을 제거했다. **MCU 동일 ARGB 입력 parity**를 검증한 것이며
  Flutter SDK의 endian 재해석 경로를 포함한 모든 이미지 public API parity를 주장하지 않는다.
- `paintImage`의 기본 `Alignment.center` 누락(D06), `MultiFrameImageStreamCompleter`
  생성자의 codec 완료 연결 및 오류 listener 목록 누락(D07)을 실제 사진/MemoryImage로
  재현해 수정했다. 완료 전 해제, 동기 프레임, normal/ephemeral error도 검증한다.
- 계약·한계·재실행: [image-pipeline/README.md](Doroti/validation/image-pipeline/README.md).
  채택한 소스와 라이선스는 [source-provenance.json](Doroti/validation/image-pipeline/source-provenance.json),
  [THIRD-PARTY-NOTICES.md](Doroti/THIRD-PARTY-NOTICES.md)에 기록했다.

### 8.2 현재 입력과 reference UI

- 로컬 최종 입력: `mae-mu-9002s2VnOAY-unsplash.webp`, 680,944 bytes,
  SHA-256 `67D80A7D869C6983B4ECB79B26D62233473B74FFB3CA2F36267655C7274CA5A2`.
  최초 `marcel-l-PQewPJqNKwQ-unsplash.jpg` 결과는 이력으로 보존한다.
- URL: `https://plus.unsplash.com/premium_photo-1734210255965-0a721514a34e`.
  네이티브 다운로드 입력 SHA-256
  `D4B657DE982CCD6985E5DF9964059A83AD0BFEF8BBDB230D725B363E02A46A06`.
  Web은 URL을 실제 요청하고 `NetworkImageIo`에서도 로드한다.
- [reference Image demo](reference/flutter_sample_app/lib/src/image_demo.dart)는
  Components의 Text inputs 뒤에 추가했다. 로컬/URL, Contain/Cover, 실패 재시도,
  라이트·다크 Primary/Secondary/Tertiary와 RGB 표시를 제공한다. 비동기 추출은
  선택 generation/mounted를 확인해 이전 선택 결과가 새 선택을 덮지 못하게 한다.
  WebP를 reference asset으로 복사했으며 루트의 사용자 원본 파일은 유지했다.
  기존 6개 이미지 테마 선택은 유지한다. 이 섹션은 사용자 요청에 따른 reference 확장이다.
- 새 입력 snapshot은 `reference-webp-snapshot/reference-inputs.json`에 별도로 보관했다.
  snapshot 실행기는 assets도 포함하며 기존 manifest 덮어쓰기를 거부한다.

### 8.3 실행 결과와 남은 경계

증거 root는 `.doroti/evidence/material-image-repair/`이다. 모든 테스트/빌드에는
20분 timeout을 적용했으며 실행기가 소유한 앱/server는 종료한다.

| 검증 | 결과 | 증거/경계 |
| --- | --- | --- |
| Search + 이미지 독립 contracts | **PASS 8/8** | `contracts-final/contracts.stdout.log`; 실제 2×1/3×1 readback·alpha·PNG·dispose·stream 검증 포함 |
| Native WebP + URL 파일 + 합성 4종 | **PASS 6/6 differential** | `final-webp-native/oracle.stdout.log`; 각 palette 전체 key/count, seed, 46 light+46 dark roles 정확 일치 |
| Web CanvasKit 실제 MemoryImage/NetworkImageIo | **PASS 2개 사진 + 픽셀/수명/손상 이미지** | `artifacts/wrapper/material-image-repair-webp-final/playwright.stdout.log`; `webp-oracle-final.stdout.log` 정확 일치 |
| 기존 FCR-7 통합 | **PASS** | `regression-final/contracts.stdout.log` |
| 기존 Web F0/F1/F2 resize + 입력 + JPEG/URL 이미지 | **PASS 5/5** | `artifacts/wrapper/material-image-repair-v3/playwright.stdout.log`; 이후 local WebP만 교체한 별도 최종 이미지 재검증 PASS |
| Windows/Web Release | **PASS**, warning/error 0 | `windows-build-final.*`, `web-validation-build-final.*` |
| Windows Vulkan gallery 시작 | **8초 smoke PASS**, 최초 2.5초 smoke **FAIL 보존** | `windows-smoke-v2.json`: visible-after-exact-present, failed terminal 0. 최초 `windows-smoke-final.*`는 presented=0으로 실패. 짧은 startup 예산 결과를 PASS로 재해석하지 않음 |
| Flutter analyze + 전체 widget tests | **PASS**, issues 0 / 15 tests | `flutter-analyze.*`, `flutter-tests-v1.*`; 실제 palette 추출 및 390 폭 source/fit 전환 포함 |
| Flutter WebP Web build | **PASS** | `flutter-webp-build.*` |
| Flutter reference 실제 asset/URL + palette | **PASS**, 1280×1000 screenshot 확인 | `.doroti/evidence/flutter-image-demo-webp-v5/`; `artifacts/flutter-image-demo-webp-v5/.../{local,url}-palette.png`. semantics click 검증이며 physical pointer acceptance 아님 |
| 이전 실패/불충분 검사 | **보존** | compile 중간 로그, `native-oracle-v3/v5`, Web image v1 timeout, Flutter browser v1–v3 harness 실패; v4는 URL 선택 후 이전 palette 상태를 기다리지 않아 불충분, v5에서 clear→새 완료를 기다리도록 보완 |

표의 `artifacts/` 경로는 `Doroti/validation/web-playwright/artifacts/` 기준이다.
동일 파일도 host decoding/리샘플링 차이로 픽셀이 조금 달라질 수 있다. 이번 WebP의
seed는 native `0xFFF38301`, Web `0xFFF78C02`; URL은 둘 다 `0xFF769296`였다.
각 readback의 MCU differential은 PASS지만 **cross-host 전체 픽셀/seed 동일성은 아니다**.
강제 Worker restart 중 read, animated playback, wide-gamut, 다른 OS live, physical
scan-out/IME/accessibility 및 Doroti sample 통합은 **notVerified**로 유지한다.

**다음 순서:** mounted Search open/close/focus gate를 완료하고 P1부터 Doroti sample
이식을 이어간다. 이미지 공용 경로는 이제 사용할 수 있다. P4의 6개 테마 이미지 UI와
latest-selection/error/retry 통합, D05 URL 실행, P5–P7은 별도로 남아 있다.
