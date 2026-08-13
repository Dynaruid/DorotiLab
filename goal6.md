# Doroti 6차 목표 — live Flutter framework bring-up과 DemoApp component coverage

> 상태: G6-5R 🚧 진행 중 — G6-5 시각/생성 품질과 native interaction/cursor 재개방, G6-6 진입 보류
> 작성일: 2026-08-12
> 선행 상태: Goal5 종료. compiler/API/build/synthetic 산출물은 입력으로 재사용하되 runtime readiness는 재검증
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> Avalonia source pin: `f159423f691946e713f454447a780d4677d8a0d2`
> 최우선 제품 gate: 실제 `DorotiDemoApp`의 reviewed `MaterialApp` widget tree가 native window에서 mount/layout/paint/present되고 오류 없이 상호작용할 것

## 0. Goal5에서 배운 현재 상태

Goal5는 Flutter source census, public API, generated/reviewed project build, synthetic gallery와 Windows RID host를 상당 부분 준비했다. 그러나 이 증거는 실제 framework widget tree 실행을 충분히 검증하지 않았다.

`DorotiDemoApp`에 reviewed Material을 연결한 live smoke에서 다음 계층의 문제가 순차적으로 드러났다.

- Dart 의미: nullable super parameter, null-aware call/return, generic covariance, dynamic dispatch, `Future<T>` 값 보존
- 생성/초기화: redirecting/factory constructor, field initializer, mixin 초기화, binding singleton과 owner 연결
- Widgets: route generic type, base member dispatch, restoration type, focus, localization, element/render-object child 연결
- Animation/Rendering: ticker/controller 초기화, multi-child container generic bridge, parent data와 child type
- Material: theme/default resolution과 `ListTile`, `AppBar` 같은 실제 component build 경로

현재 `181 Material exported libraries`, `52 Cupertino exported libraries`, API diff 0과 reviewed build 0 errors는 계속 유용한 정적 증거다. 하지만 component가 실제로 `construct -> mount -> build -> layout -> paint -> present -> interact`하는지는 별도 증거가 필요하다.

Goal6은 이 차이를 닫는다. 많은 파일을 한 번에 생성하는 것보다 작은 live vertical slice를 먼저 완성하고, 같은 gate를 누적하면서 component family를 확장한다.

## 1. 목표와 범위

Goal6의 1차 목표는 Doroti framework만으로 일반적인 데스크톱 DemoApp을 구성할 수 있게 하는 것이다. 2차 목표는 Flutter의 Material/Cupertino public component 대부분을 실제 실행 가능한 상태로 넓히는 것이다.

최종 DemoApp은 최소한 다음을 포함한다.

- app/theme: `MaterialApp`, light/dark theme, inherited theme, localization
- layout/display: `Scaffold`, `AppBar`, `Text`, `Icon`, `Card`, `ListTile`, `Divider`, `Row`, `Column`, `Stack`, `Container`, `Padding`, `Align`, `Center`
- action/selection: FAB, Material button variants, icon button, menu, checkbox, radio, switch, slider
- navigation/overlay: named and imperative navigation, route transition, dialog, snack bar, bottom sheet, tooltip
- input/form: `TextField`, `Form`, validation, keyboard/focus, clipboard, Windows IME
- scrolling/data: `SingleChildScrollView`, `ListView`, `GridView`, lazy 1,000-item list
- resources: image, asset, font, localization and one platform-channel example
- semantics: label/focus/invoke/toggle/setText/scroll action round trip

Linux/macOS target 확장, release packaging과 physical matrix는 framework live gate 뒤에만 수행한다. Windows에서 최소 DemoApp조차 present되지 않는 동안 cross-target package 수를 늘리지 않는다.

## 2. 증거 상태 모델

모든 library/component는 아래 상태를 독립적으로 기록한다. 앞 상태의 PASS가 뒤 상태의 PASS가 아니다.

1. `discovered`: pinned Flutter export/closure에 존재
2. `analyzed`: analyzer/IR unsupported 또는 silent omission 0
3. `generated`: clean/incremental identity가 같은 C# 생성
4. `compiled`: reviewed product project warning/error 0
5. `constructed`: public constructor와 기본값 초기화 성공
6. `mounted`: Element/State/Inherited dependency 연결 성공
7. `laidOut`: finite constraint/size와 parent data 검증 성공
8. `painted`: non-empty display list/raster output 생성
9. `presented`: native strict-GPU surface에 frame 제출·표시
10. `interactive`: pointer/keyboard/text/focus/scroll callback과 state update 왕복
11. `semantic`: native accessibility tree/action 왕복
12. `packagedPhysical`: repository 밖 release package와 실제 target/device에서 검증

각 상태에는 실행 명령, target identity, source/product digest, trace 또는 screenshot과 실패 원인을 기록한다. 실행하지 않은 상태는 `notVerified`이며 `compiled`를 `presented`로 환산하지 않는다.

component coverage matrix는 Flutter public API manifest에서 자동 생성한다. 수동 allowlist는 component 분류와 platform 제약 사유에만 사용하고 성공 상태를 직접 지정할 수 없다.

## 3. Roadmap

### G6-0 ✅ — truth reset과 재현 가능한 live baseline

목적: Goal5 증거를 폐기하지 않되 실제 실행 증거와 synthetic 증거를 분리하고, 현재 첫 실패를 안정적으로 재현한다.

작업:

- G5-3/G5-4/G5-5 evidence를 `compileApi`, `syntheticContract`, `managedBehavior`, `nativePresented`, `physical`로 재분류한다.
- 기존 G5-4 gallery trace에는 `syntheticContract`를 표시하고 live widget mount/present로 사용하지 못하게 schema validator를 추가한다.
- `DorotiDemoApp`에 deterministic smoke mode를 고정한다. 첫 Flutter error, unhandled exception, frame count, backend, non-empty pixel bounds와 resource count를 저장한다.
- 현재 Material live 실패를 source pin, generated digest와 stack trace가 포함된 baseline taxonomy로 동결한다.
- 최소 Dart fixture와 equivalent reviewed C# fixture가 동일한 framework path를 사용하게 한다. 장기적으로 DemoApp 제품 진입점은 일반 Dart application compiler 산출물을 사용한다.
- 테스트 하나가 20분을 넘지 않도록 compiler, managed runtime, live Windows, physical test를 shard한다.

완료 gate:

- 이전 PASS를 live PASS로 잘못 분류한 항목 0
- current Material failure가 clean checkout/build에서 동일 taxonomy로 재현됨
- framework error 발생 시 smoke가 frame timeout 대신 최초 원인과 widget/library를 보고함
- `git diff --check`, compiler build와 현재 product solution build PASS

산출물:

- `Doroti/migration/flutter-framework/g6-component-coverage.json`
- `Doroti/migration/flutter-framework/g6-runtime-error-taxonomy.json`
- `Doroti/migration/flutter-framework/g6-evidence.json`
- `Doroti/eng/validate-g6-baseline.ps1`

완료 증적 (2026-08-12):

- G5-3/G5-4/G5-5의 PASS를 5개 evidence class로 재분류했고, live PASS 오분류는 0건이다.
- public API manifest에서 Material/Cupertino 642개 선언과 193개 component candidate를 자동 생성했다. `constructed` 이후 상태는 모두 `notVerified`로 시작한다.
- clean build 뒤 live smoke를 두 번 실행해 `framework.material.app-bar.build.null-reference` taxonomy, `AppBar`, source library, source pin과 generated digest가 동일함을 확인했다.
- smoke는 8초 timeout을 기다리지 않고 최초 `FlutterError`의 `NullReferenceException`, widget/library, 0 presented frame, strict GPU backend와 resource snapshot을 기록한다.
- compiler, managed runtime, product, live Windows shard와 `git diff --check`가 PASS했다. physical/native presented 상태는 실행하지 않았으므로 `notVerified`를 유지한다.

### G6-1 ✅ — Dart 의미·생성기·runtime structural closure

진입 조건: G6-0 완료.

목적: Material 실행 중 드러난 문제를 파일별 문자열 치환으로 쫓지 않고 analyzer/typed IR/lowerer/runtime의 공용 의미로 해결한다.

필수 semantic family:

- nullable/super formal/default argument와 상위 constructor 전달
- generative, factory, redirecting, named constructor와 field/mixin initializer 순서
- generic variance, erased Dart runtime type와 C# invariant generic boundary
- `Future`, `Future<T>`, async/await, then callback과 typed value 보존
- null-aware access/call/cascade, nullable return과 `late`/required semantics
- override/base member dispatch, extension method와 mixin member resolution
- tear-off, closure/delegate adaptation, optional named callback
- collection spread/if/for, pattern/switch와 dynamic invocation

작업 방식:

1. 각 runtime crash를 10~50줄 Dart micro fixture로 축소한다.
2. pinned Dart/Flutter reference trace와 generated C# trace를 비교한다.
3. analyzer/IR에 의미 정보가 없으면 IR schema를 먼저 보강한다.
4. 공용 lowerer/runtime contract에서 수정하고 clean regenerate한다.
5. library 이름이나 generated local 번호에 의존하는 compatibility rewrite는 임시 disposition과 제거 milestone 없이는 추가하지 않는다.
6. 기존 `FrameworkCSharpLowerer.G53Compatibility` 규칙을 `structural`, `temporary`, `obsolete`로 분류하고 temporary count를 매 단계 감소시킨다.

완료 gate:

- baseline taxonomy의 language/runtime 항목 100%에 독립 micro fixture 존재
- 모든 fixture의 reference/generated behavior differential PASS
- clean/incremental byte identity PASS
- demo widget 이름을 검사하는 새 compiler special case 0
- direct generated `.g.cs` hotfix 0; regenerate 후 수정 내용 유지
- compiler와 G5 predecessor regression PASS

산출물:

- `Doroti/validation/fixtures/g6-language-runtime/`
- `Doroti/migration/flutter-framework/g6-language-runtime-evidence.json`
- `Doroti/migration/flutter-framework/g6-compatibility-disposition.json`
- `Doroti/eng/validate-g6-language-runtime.ps1`

완료 증적 (2026-08-12):

- language/runtime과 constructor/initialization taxonomy를 8개 독립 Dart micro fixture로 고정했고, pinned Dart reference와 generated C# trace가 전부 일치한다.
- analyzer typed IR에 extension과 collection spread/if/for node를 등록하고, extension member, 중첩 collection element, if-case pattern, constructor tear-off, nullable argument, enum name과 List invariant boundary를 공용 lowerer에서 처리한다.
- public concrete type의 named generative constructor와 `Future<T>` callback invariant adapter를 공용 생성 규칙으로 보강하고, `DiagnosticsStackTrace.showSeparator` runtime 계약과 owner audit를 동기화했다.
- clean/incremental generated digest `d4a8594ec3a25d22c2a181e742ce094090eabdc5ef216cec9ffb5e67b1b8b9bf`가 동일하며 compiler diagnostic, unclassified AST, silent omission과 generated compile error는 모두 0이다.
- G5-3 compatibility rewrite 383개를 structural 9개와 temporary 374개로 전수 분류했고, nullable Brightness 임시 규칙 1개를 제거해 temporary count를 375에서 374로 줄였다.
- demo/fixture 이름 compiler special case와 direct generated `.g.cs` hotfix는 0개이며, compiler refactor, G5 predecessor, G4 regenerate/package/consumer와 `Doroti.Product.slnx` Release build가 PASS했다.
- base Widgets/Material native first frame과 physical target은 G6-2 이후 범위이므로 `notVerified`를 유지한다.

### G6-2 ✅ — Widgets live core와 base first-frame closure

진입 조건: G6-1 완료.

목적: Material을 올리기 전에 base Widgets application이 실제 native surface에서 안정적으로 frame을 낼 수 있게 한다.

vertical slice:

- binding/owner: Scheduler, Services, Semantics, Renderer, Widgets binding 초기화와 singleton/lifetime
- tree: Widget/Element/State mount, update, rebuild, deactivate/unmount, inherited dependency
- rendering: PipelineOwner, RenderView, constraints, parent data, single/multi child, layout/paint/compositing
- frame: schedule/begin/draw/present, terminal ACK, resize와 dispose
- supporting runtime: focus, semantics, localization, restoration, ticker/animation 기본 경로

base live app은 `Directionality -> ColoredBox/Container -> Flex -> Text`에서 시작하고, 단계별로 scrolling, focus와 semantics를 추가한다. 각 slice는 이전 slice를 포함한다.

완료 gate:

- actual HWND + strict `skia-wgl-opengl-gpu`에서 non-empty first frame PASS
- 300 frame/30초 smoke 동안 FlutterError, unhandled exception, invalid constraint와 leaked window/context 0
- single-child와 multi-child tree의 mount/update/reorder/remove behavior PASS
- resize/minimize/restore 후 frame 재개와 terminal ACK PASS
- text/focus/semantics 최소 왕복 PASS
- base Widgets reference trace와 layout/paint tolerance PASS

산출물:

- `Doroti/validation/Doroti.Validation.G6WidgetsLive/`
- `Doroti/migration/flutter-framework/g6-widgets-live-evidence.json`
- `Doroti/eng/validate-g6-widgets-live.ps1`

완료 증적 (2026-08-12):

- promoted Widgets binding이 매 frame `BuildOwner.buildScope -> layout -> compositing -> paint -> present -> BuildOwner.finalizeTree`를 실행하도록 복구했고, constructor body 누락은 공용 lowerer와 owner-shadow fixture로 고정했다.
- actual HWND의 strict `skia-wgl-opengl-gpu`에서 `Directionality -> ColoredBox -> Container -> Flex -> Text` 첫 frame과 scrolling/restoration, focus, semantics 누적 slice를 실행했다. 첫 frame은 1280x840 physical pixel 전체가 non-transparent이고 기준 accent 영역은 320x120으로 일치했다.
- single/multi child mount/update/keyed reorder/remove, inherited dependency, resize의 stale terminal ACK 뒤 present, minimize/restore frame 재개와 shutdown unmount/dispose trace가 reference 순서를 통과했다.
- 30초 cadence에서 300개 요청 frame을 포함해 307 frame이 present됐고 submitted 308 = presented 307 + resize stale 1, failed/cancelled 0, queue/active frame 0이다.
- 별도 `System.Windows.Automation` client의 native Button `SetFocus`/`Invoke`가 `Semantics.onFocus` 2회와 `Semantics.onTap` 1회로 framework까지 왕복했다.
- 종료 뒤 active HWND와 WGL context는 각각 0이고 created/released가 1/1로 균형을 이뤘다. G6-1 fixture, G5 Widgets managed regression과 `Doroti.Product.slnx` Release build도 PASS했다.
- 이 증거는 automated Windows x64 native 결과다. physical device/비-Windows target과 Material component 상태는 성공으로 확장하지 않으며 `notVerified`를 유지한다.

### G6-3 ✅ — 최소 Material DemoApp

진입 조건: G6-2 완료.

목적: 요청한 최소 DemoApp을 실제 reviewed Material widget으로 구성하고 이를 Goal6의 상시 blocking gate로 만든다.

구현 순서:

1. `MaterialApp.builder` + `Theme` + `Scaffold` + body text
2. `AppBar`, `Card`, `ListTile`, FAB
3. button, checkbox, radio, switch, slider와 local state update
4. `Row`/`Column`/`Stack`, scroll view와 lazy list
5. `MaterialApp.home` + Navigator initial route도 통과시켜 builder 우회를 영구 해법으로 남기지 않는다.

DemoApp smoke는 root type만 검사하지 않는다. 실제 display list/raster가 비어 있지 않고, component별 semantics node와 지정된 색/텍스트 영역이 존재하며, 입력 후 pixel/state/semantics가 바뀌는지 검증한다.

완료 gate:

- 위 최소 component가 모두 `presented` 상태
- action/selection component가 모두 `interactive` 상태
- `MaterialApp.builder`와 `MaterialApp.home` 두 진입 경로 PASS
- 30초/300 frame strict-GPU run 중 framework error 0
- baseline screenshot과 text/layout geometry가 정한 tolerance 내 PASS
- `DorotiDemoApp`이 migration candidate 경로가 아니라 promoted product package를 참조
- clean external consumer에서도 같은 DemoApp smoke PASS

산출물:

- `DorotiDemoApp/`의 Goal6 Material gallery 첫 화면
- `Doroti/migration/flutter-framework/g6-material-demo-evidence.json`
- `Doroti/artifacts/g6-material-demo/win-x64/`
- `Doroti/eng/validate-g6-material-demo.ps1`

완료 증적 (2026-08-13):

- reviewed product `Material`/`Cupertino` 프로젝트를 승격하고 `DorotiDemoApp`이 migration candidate가 아닌 `Doroti.Flutter.Framework.Material` 제품 프로젝트를 참조하도록 전환했다. clean 외부 소비자는 동일 소스를 로컬 `.nupkg`만으로 restore/build한 뒤 builder/home smoke를 모두 통과했다.
- clean stage에서 reviewed candidate를 다시 생성한 뒤 promotion을 재실행했고, 승격 대상 259개 파일이 product source와 byte 단위로 모두 일치함을 확인했다.
- `MaterialApp.builder`와 `MaterialApp.home`/Navigator 두 진입 경로에서 `Theme`, `Scaffold`, `AppBar`, `Card`, `ListTile`, FAB, button/checkbox/radio/switch/slider, `Row`/`Column`/`Stack`, scroll view와 `ListView.builder`가 actual HWND의 strict `skia-wgl-opengl-gpu`로 present됐다.
- 6개 action/selection control은 local state, semantics와 raster pixel을 모두 변경했고, 별도 `System.Windows.Automation` client가 6개 native control을 invoke했다. builder evidence의 총 interaction count는 12다.
- 30초/300 cadence gate는 30,035.3313ms 동안 357 frame을 present했고 framework/raster failed 0, cancelled 0, software fallback 0, submitted/terminal ACK와 native HWND/WGL resource closure가 균형을 이뤘다.
- baseline reference의 Card/Stack/text geometry tolerance, 지정 색 영역과 text ink coverage, interaction 전후 changed pixel minimum을 builder/home 및 clean package-only 외부 소비자에서 검증했다.
- `validate-g6-material-demo.ps1`의 compiler fixture, G5 Widgets W0-W7 regression, `Doroti.Product.slnx` Release build(30 projects, warning/error 0), G6-2 committed evidence와 G6-3 aggregate evidence gate가 PASS했다.
- `validate-g6-language-runtime.ps1 -Shard All`도 298.5초에 PASS했고, G4-3 후보/승격/package 회귀와 최종 제품 빌드(37.04초, warning/error 0)를 함께 확인했다.
- protected framework의 모든 식별자에 semantic lookup을 수행하던 architecture analyzer 병목을 qualified backend type 후보로 제한했다. Material 202파일/10.03MiB cold analyzer build는 46.79초, 최종 G6-3 `All`은 234.4초에 완료됐고 종료 후 Doroti build process 0을 확인했다.
- `DorotiDemoApp`의 과거 native list/A2/runtime-v2 진입점과 소스를 제거하고 reviewed Material 앱을 표준 `Program.cs` 단일 진입점으로 승격했다. baseline 및 package-only external consumer 검증도 같은 제품 소스를 사용한다. 이 정리 중 포인터 hit-test가 노출한 stale nullable value-return 출력을 현재 structural lowerer 결과와 동기화했고, builder/home, 외부 UIA 6개 control, 30초/300 frame strict-GPU 및 clean package-only consumer gate를 다시 통과했다.
- 첫 화면 직후 native Win32 mouse move/down/up을 보내는 회귀 gate를 추가했다. `BoxHitTestResult.wrap`/`SliverHitTestResult.wrap`이 원본 `HitTestResult` path/transform storage를 공유하지 않아 첫 클릭 대상이 유실되던 생성기 출력을 공용 copy-wrap lowering으로 수정했고, covariant `handleEvent`의 사용되지 않는 invalid entry cast도 제거했다. 실제 포인터가 추가로 노출한 scrollbar custom-painter adapter cast와 한 인자 `List.sublist(start)`의 잘못된 `GetRange(start, list.Count)` 길이 lowering도 생성기/제품 양쪽에서 수정했다. short builder smoke는 native pointer interaction 1, hit-test target 23개(`RenderPointerListener` 포함), 총 interaction 7로 PASS했고 G5 Widgets 관리형 box/sliver wrap 회귀도 PASS했다. 최종 visible Windows live shard는 외부 UIA를 포함해 interaction 14, 30,037.836ms 동안 cadence 337 frame, 전체 presented 431, failed/cancelled 0, strict GPU와 native resource balance로 PASS했다.
- 증거 범위는 automated Windows x64 native다. physical device와 Linux/macOS 및 다른 RID는 성공으로 확장하지 않고 `notVerified`를 유지한다.

### G6-4 — 일반 앱 필수 기능 vertical slices

진입 조건: G6-3 완료. 이후 모든 변경은 G6-3 DemoApp gate를 회귀 실행한다.

slice:

- N0 navigation: push/pop, named route, transition, back/focus restoration
- O0 overlay: dialog, snack bar, tooltip, popup menu, modal/persistent bottom sheet
- F0 form: TextField, Form validation, selection, clipboard, keyboard와 Windows IME composition
- S0 scrolling: ListView/GridView/custom scroll, 1,000-item lazy lifecycle, wheel/drag/keyboard scroll
- R0 resource: asset image, network/file error path, bundled font, localization and locale switch
- A0 accessibility: external native client focus/invoke/toggle/setText/scroll round trip
- P0 plugin: one platform channel happy path와 unsupported capability exact failure

slice 완료 gate:

- reference behavior trace와 managed/live native trace PASS
- 해당 component의 `presented`, `interactive`, 필요한 경우 `semantic` 상태 PASS
- completed predecessor slice 전체 회귀 PASS
- unsupported/platform-dependent 경로의 silent success 0
- physical IME/accessibility가 필요한 세부 항목은 automated native와 분리해 `notVerified` 유지

산출물:

- `Doroti/migration/flutter-framework/g6-app-slices.json`
- `Doroti/migration/flutter-framework/g6-app-slices/<slice>-evidence.json`
- `Doroti/eng/validate-g6-app-slice.ps1 -Slice <N0|O0|F0|S0|R0|A0|P0>`

완료 증적 (2026-08-13):

- N0/O0/F0/S0/R0/A0/P0를 독립 reference fixture와 managed contract, visible actual HWND의 strict `skia-wgl-opengl-gpu` live slice로 실행했고 aggregate와 7개 slice evidence가 모두 `verified-windows-x64-strict-gpu`다. 모든 slice의 framework/raster failed와 cancelled, software fallback, unsupported silent success는 0이며 HWND/WGL resource closure가 균형을 이뤘다.
- navigation route/back/focus, dialog/snack bar/tooltip/popup/sheet, form validation/selection/clipboard/keyboard composition, resource/localization과 platform channel success/exact failure를 interaction 전후 raster와 state/reference trace로 검증했다.
- S0는 Win32 pointer focus, wheel, drag와 PageDown을 실제 host packet으로 보냈다. 누락돼 있던 `PointerData.signalKind=scroll` host 변환을 복구한 뒤 live `ScrollController.offset > 0`, interaction raster 변화와 1,000개 중 일부만 생성되는 lazy lifecycle을 함께 gate로 고정했다.
- R0 text renderer는 rune별 `SKFontManager.MatchCharacter` fallback과 HarfBuzz shaping을 사용한다. 현재 Windows에서 `Malgun Gothic`/`Segoe UI` 한글 fallback, bundled Segoe UI font와 ko-KR localization resource를 확인했으며 큰 시스템 한글 폰트를 제품에 복제하지 않는다.
- A0는 별도 `System.Windows.Automation` 프로세스가 화면에 표시된 native 창을 대상으로 focus/invoke/toggle/setText/scroll 5개 동작을 수행했다. P0는 `MethodChannel` echo와 `g6/unsupported`의 capability/channel/view/target identity를 보존하는 exact failure를 확인했다.
- `validate-g6-app-slice.ps1 -Slice All`, G6-3 committed evidence gate와 `validate-g6-material-demo.ps1 -Shard Regression`이 PASS했다. 증거 범위는 automated Windows x64 native이며 physical Windows IME/accessibility, physical device, Linux/macOS와 다른 RID는 `notVerified`를 유지한다.

### G6-5 ✅ — Material component majority coverage

진입 조건: G6-4 완료.

목적: “Material 전체가 빌드된다”가 아니라 public component 대부분이 자동 gallery에서 실제로 표시되고 동작하게 한다.

public manifest를 다음 wave로 자동 분류한다.

- M0 foundations: color scheme, typography, theme, icon, ink, shape, elevation
- M1 layout/display: scaffold family, app bar, card, list tile, badge, chip, divider, banner
- M2 actions: button families, FAB, icon button, segmented button, menu
- M3 selection/input: checkbox, radio, switch, slider, text field, search, autocomplete
- M4 navigation: navigation bar/rail/drawer, tabs, stepper, page routes
- M5 overlays/feedback: dialog, sheets, snack bar, tooltip, progress indicators
- M6 data/date/time: data table, date picker, time picker, calendar and adaptive variants

각 component fixture는 default, disabled, focused, hovered, pressed/selected, error와 text-scale/DPI 변형 중 해당되는 상태를 가진다. visual golden만으로 callback을 대신하지 않고 state/semantics trace도 함께 검증한다.

완료 gate:

- DemoApp 필수 Tier A component의 `presented`와 해당 `interactive/semantic` 100%
- platform 비의존 public Material UI component library 중 90% 이상이 최소 `presented`
- 상호작용을 제공하는 검증 대상 component family 100%에 input/state test 존재
- 미지원 10% 이하는 library/symbol/원인/owner/후속 조건이 component matrix에 명시됨
- 모든 wave가 actual native first-frame과 aggregate 30초 gallery smoke PASS
- Material public API diff 0과 direct Avalonia Controls/theme/XAML dependency 0 유지

산출물:

- `Doroti/validation/Doroti.Validation.G6MaterialGallery/`
- `Doroti/migration/flutter-framework/g6-material-component-matrix.json`
- `Doroti/migration/flutter-framework/g6-material-gallery-evidence.json`
- `Doroti/eng/validate-g6-material-wave.ps1 -Wave <M0..M6>`

완료 증적 (2026-08-13):

- M0 foundations부터 M6 data/date/time까지 7개 wave를 독립 actual HWND와 strict `skia-wgl-opengl-gpu`로 실행했다. 모든 wave가 native first frame, callback 직접 호출 전후 raster 변화, semantics node, state trace, failed/cancelled 0, software fallback 0과 HWND/WGL resource balance를 통과했다. 이 callback 변화는 실제 pointer interaction 증거가 아니며 아래 G6-5R-I에서 재검증한다.
- platform 비의존 public Material UI를 60개 component/foundation family로 분류했고 54개를 실제 `presented`하여 90%를 달성했다. 필수 Tier A는 22/22다. 기존 interactive family 24/24 수치는 callback-state/semantic fixture coverage이며 native hover/click coverage로는 `notVerified`로 정정한다.
- 미지원 6개(10%)는 `SegmentedButton`, `DropdownMenu`, `SearchAnchor`, `Stepper`, `DatePickerDialog`, `TimePickerDialog`이며 component matrix에 library/symbol, 원인, owner와 승격 후속 조건을 기록했다. calendar body인 `CalendarDatePicker`는 M6 live PASS지만 dialog route까지 성공으로 확장하지 않았다.
- M6 aggregate smoke는 30,108.7926ms 동안 cadence frame 305개를 present했고 failed/cancelled와 software fallback은 0, native resource closure는 balanced다. state fixture는 default/disabled/focused/hovered/pressed-or-selected, M3 error, text scale 1.0/1.3과 actual window DPI를 기록한다.
- live bring-up에서 icon shadow null, RichText text scaler, semantics action id 0, checkbox/radio/switch state property, TextField formatter, navigation closure, unbounded animation controller, button constraint map, table child/iterator, PageView/GridView optional value와 rounded-rect GPU host 경계를 compiler lowering과 promoted product에 함께 복구했다.
- predecessor M0~M5를 누적 실행하는 `validate-g6-material-wave.ps1 -Wave M6`, G5-4 reviewed candidate build/API/gallery/dependency gate와 G6-4 `validate-g6-app-slice.ps1 -Slice All`이 PASS했다. Material public API diff와 direct Avalonia Controls/theme/XAML dependency, unsupported silent success는 모두 0이다.
- 증거 범위는 automated Windows x64 native다. physical Windows IME/accessibility, physical device, Linux/macOS와 다른 RID는 실행하지 않았으며 `notVerified`를 유지한다.

### G6-5R 🚧 — Material visual fidelity와 생성 품질 재감사

진입 조건: G6-5 coverage gate 완료. G6-5의 90% component 수치는 보존하되, `presented`만으로 Flutter Material과 시각적으로 일치한다고 간주하지 않는다.

재개방 사유 (2026-08-13):

- M6 `CalendarDatePicker`가 live/presented PASS였지만 Material Icons PUA glyph가 tofu 사각형으로 표시됐고 Roboto/type style, 선택일 stroke, 7열 grid 방향, 초기 page, rounded shape와 elevation shadow가 실제 화면에서 달랐다.
- `review-g5-4-generated.ps1`의 파일명/숫자 suffix 기반 후처리, compatibility 단계의 C# 문자열 치환, Flutter `Ink`를 `DecoratedBox`로 바꾸는 widget 대체 같은 우회가 coverage gate를 통과할 수 있었다.
- `ColorScheme.fromSeed` compatibility가 seed/variant/contrast에서 실제 tonal palette를 계산하지 않고 고정 role 색을 반환한다. 현재 purple fixture의 외형 개선을 일반 `fromSeed` 호환으로 확대할 수 없다.
- 생성된 local의 숫자 suffix 자체는 source-offset identity로 허용할 수 있지만, 후처리 규칙이 그 이름을 참조하거나 `default!`, nullable 값, named-constructor forwarding을 의미와 다르게 만들면 실패로 분류해야 한다.
- first-frame readback, interaction raster 변화와 component count는 glyph/shape/shadow/layout의 정확성을 증명하지 못하므로 reference visual differential이 별도로 필요하다.

작업:

- G6-0~G6-5에서 추가된 모든 `Update-GeneratedFile`, lowerer `source.Replace`, reviewed product 직접 보정을 machine-readable inventory로 만든다. 각 규칙에 원인 계층, 최소 fixture, owner, permanent/temporary, 제거 milestone을 기록한다.
- filename, source offset 또는 생성 local 번호에 의존하는 수정을 analyzer/IR/공용 lowering/runtime 의미 수정으로 올린다. Flutter widget type을 다른 widget으로 대체해 paint를 우회하는 규칙은 허용하지 않는다.
- collection-if/spread가 있는 Set, nullable restoration, named constructor/super-parameter forwarding, override dispatch와 null-aware snapshot을 독립 fixture로 고정하고 clean regenerate 뒤 제품 직접 diff 없이 재현한다.
- bundled Roboto/Material Icons의 license와 package 경로, `TextStyle.fontFamily`/color/weight 전달, glyph coverage와 system fallback 순서를 검증한다. 알려진 Material Icons codepoint가 tofu 대체 glyph로 rasterize되면 실패한다.
- `Path`/`RRect`/clip/transform, fill/stroke/strokeWidth/anti-alias와 elevation ambient/spot shadow를 UI → display list → composition → Skia GPU 및 managed reference까지 손실 없이 전달한다. bounds 사각형으로의 silent 축약은 unsupported 또는 실패로 보고한다.
- `ColorScheme.fromSeed`는 pinned Flutter가 사용하는 Material Color Utilities와 동일한 seed/brightness/dynamicSchemeVariant/contrastLevel 결과로 교체하고 서로 다른 seed 및 light/dark metamorphic fixture를 둔다.
- 동일한 locale, text scale, DPI, window size, theme, 날짜를 사용한 pinned Flutter reference와 Doroti capture를 쌍으로 저장한다. 구조 mask와 component별 tolerance를 두고 glyph, baseline, 7열 배치, selected/today state, corner radius와 shadow extent를 각각 판정한다.
- screenshot은 실제 strict-GPU first presented frame에서 수집하고 이미지 SHA-256, source/product/compiler identity를 evidence에 포함한다. repaint가 없는 빈 후속 frame 또는 fixture 설명 문자열은 visual PASS로 인정하지 않는다.

완료 gate:

- G6-0~G6-5 compatibility/review 규칙 inventory 100%, owner/fixture/제거 조건 누락 0
- G6-5 경로의 generated `.g.cs` 직접 hotfix 0, widget type 대체 0, 숫자 local명에 의존하는 후처리 0
- clean/incremental regenerate identity와 promoted product identity PASS; collection/named-constructor/nullability/override fixture PASS
- Roboto/Material Icons glyph coverage PASS, known icon tofu 0, font/license package gate PASS
- 최소 3개 seed × light/dark의 `ColorScheme.fromSeed` role differential PASS; 고정 seed palette fallback 0
- CalendarDatePicker reference differential PASS: horizontal 7 columns, 지정 월/선택일, glyph/baseline, 28dp shape와 elevation shadow가 개별 tolerance 안에 있음
- rounded path/clip, fill/stroke와 managed/strict-GPU raster contract regression PASS
- M0~M6 actual HWND strict-GPU 누적 실행, M6 visual artifact와 30초 smoke PASS; failed/cancelled/software fallback 0과 resource balance 유지
- physical IME/accessibility, Linux/macOS와 다른 RID는 실행 전까지 계속 `notVerified`

산출물:

- `Doroti/migration/flutter-framework/g6-compatibility-audit.json`
- `Doroti/migration/flutter-framework/g6-material-visual-differential.json`
- `Doroti/artifacts/g6-material-gallery/win-x64/M6/visual.png`
- `Doroti/eng/validate-g6-material-fidelity.ps1`

현재 복구 증적 (2026-08-13, G6-5R 전체 완료를 의미하지 않음):

- UI text/paragraph에서 `TextStyle`의 font family와 color를 host/measurer까지 전달하고 Roboto 및 Material Icons 자산/라이선스를 제품 package에 포함했다. M6의 월 이동/드롭다운 glyph는 tofu 없이 rasterize된다.
- Set collection-if, `GridView` named constructor forwarding, nullable restoration, Material ink paint override/null-aware snapshot을 생성기 또는 reviewed producer에 반영했다. CalendarDatePicker는 Flutter의 `Ink`를 유지하며 `DecoratedBox` 대체 규칙을 제거했다.
- `RRect` corner radii, paint fill/stroke/strokeWidth, anti-alias와 elevation blur를 strict-GPU 경로에 전달했다. M6 capture에서 August 2026의 horizontal 7 columns, selected 13, 28dp rounded card와 6dp shadow를 확인했다.
- M6 visual capture는 강제 full repaint 뒤 light-background-only 픽셀을 거부하고 SHA-256을 aggregate evidence에 저장한다. semantics는 첫 stable frame 뒤 활성화해 attach/layout 중 owner lifecycle race를 피한다.
- `validate-g6-language-runtime.ps1 -Shard Fixtures`, full `validate-g5-4.ps1`, reviewed product promotion과 누적 `validate-g6-material-wave.ps1 -Wave M6`를 현재 구현에서 다시 PASS했다.
- `g6-compatibility-audit.json`은 G6-0~G6-5의 compatibility/review 영향과 규칙 771개(`Update-GeneratedFile` 호출 81개 포함)를 모두 목록화했고 owner/최소 fixture/permanence/제거 milestone 누락은 0이다. reviewed product를 clean candidate에서 다시 승격해 제품 직접 diff는 0으로 만들었지만, reviewed 생성 파일 보정 193개와 숫자 generated local에 의존하는 review/lowerer 규칙 74개가 남아 audit status는 `open`이다. widget type 대체는 0이다.
- `ColorScheme.fromSeed`의 고정 purple role stub을 제거하고 seed/brightness/dynamicSchemeVariant/contrastLevel을 처리하는 Material Color Utilities runtime으로 교체했다. Dart `material_color_utilities` 0.13.0 기준 3개 seed × light/dark × contrast -1/0/1 및 9개 variant의 34 case/15 role differential은 mismatch 0으로 PASS했고 scheme/role cache 뒤에도 같은 결과를 유지했다.
- 실제 `SoftwareRasterCanvas` managed reference가 transformed path clip을 bounds로 축약하지 않고 non-zero/even-odd fill, fill/stroke/strokeWidth와 bounded blur extent를 보존한다. 독립 regression에서 triangle clip 외부, hollow stroke 내부, shadow 외곽을 픽셀로 판정했고 Roboto의 calendar text 및 Material Icons U+E5C5/U+E5CB/U+E5CC/U+E5C8 glyph 0/tofu 여부와 두 license file을 함께 PASS했다.
- 최종 누적 live 재실행은 M0~M6, 30,100.8074ms 동안 요청 301 cadence frame/실제 present 308개, failed/cancelled/software fallback 0, strict `skia-wgl-opengl-gpu`와 resource balance를 확인했다. M6 first-presented-frame SHA-256은 `5c7adc584eb748de5a158ece7555f571bd36a48327becd0e80e4986d6246660b`이며 source/product/compiler identity와 tolerance를 visual evidence에 기록했다.
- pinned Flutter revision의 CalendarDatePicker reference raster는 아직 수집하지 못해 visual differential의 glyph/baseline/selected-today/corner/shadow 판정은 `notVerified`다. generated hotfix 193개와 숫자 local 의존 규칙 74개도 제거 gate를 만족하지 않으므로 G6-5R 전체와 후속 G6-5R-I는 진행 중 상태를 유지한다.

#### G6-5R-I 🚧 — 실제 pointer interaction, MouseTracker cursor와 Win32 window chrome

재개방 사유와 확인된 원인 (2026-08-13):

- M6 gallery의 `GalleryState.Exercise()`는 등록된 component callback을 직접 순회한다. `CalendarDatePicker:callback-state-semantics`와 `hovered = true` fixture는 실제 마우스 이동, hit-test, hover state 또는 클릭을 거치지 않으므로 기존 `interactive` 판정의 인과 증거로 사용할 수 없다.
- migration IR에는 `RenderMouseRegion`의 `interfaces: [MouseTrackerAnnotation]`이 있으나 생성 C#의 `RenderMouseRegion`은 그 계약을 구현하지 않는다. `MouseTracker._hitTestInViewResultToAnnotations()`는 hit-test target 중 `MouseTrackerAnnotation`만 수집하므로 현재 `MouseRegion`의 enter/exit, cursor와 이를 이용한 `InkResponse` hover state가 구조적으로 누락된다. 제품 파일 직접 상속 보정이 아니라 Dart `implements`를 보존하는 공용 type lowering/emitter 문제로 수정한다.
- framework의 `SystemMouseCursor`는 optional `flutter/mousecursor` 채널로 `activateSystemCursor`를 보내지만 desktop host에는 이 채널의 built-in handler가 등록되지 않았다. `DesktopPlatformServicesCapability.SetCursor()`는 존재해도 채널에서 capability로 이어지지 않아 요청이 조용히 소실된다.
- Win32 창은 `OverlappedWindow | ThickFrame`으로 생성되어 native resize hit-test는 가능하지만 `SimpleWindow`가 모든 `WM_SETCURSOR`에서 Flutter client cursor를 강제로 설정하고 `1`을 반환한다. 이 때문에 `HTLEFT/HTRIGHT/HTTOP/HTBOTTOM`과 네 모서리에서도 `DefWindowProc`의 resize cursor가 가려진다. 즉 당장의 결함은 Avalonia Controls/XAML이 아니라 Avalonia에서 가져온 Doroti Win32 shell/source-port의 cursor ownership 문제다.
- Flutter desktop의 `WidgetStateMouseCursor.adaptiveClickable`은 의도상 일반 화살표(`basic`)를 선택하므로 달력 날짜 위에서 손 모양이 되지 않는 것만으로 Flutter 불일치라 판정하지 않는다. 대신 hover overlay와 click은 반드시 동작해야 하며, 명시적 `SystemMouseCursors.click/text` 및 window chrome resize cursor는 실제 cursor shape로 검증한다. 제품 정책으로 날짜 셀에 손 모양을 원하면 reference parity와 분리한 명시적 override로 기록한다.

작업:

- Dart class/interface/mixin 관계를 IR에서 C# contract로 손실 없이 내리고 `RenderMouseRegion`이 `MouseTracker`가 소비하는 annotation contract를 실제 구현하도록 한다. `proxy_box.cs` 직접 hotfix는 금지하고 최소 compiler fixture, clean regenerate와 promoted product identity로 고정한다.
- native validation controller에 좌표 기반 pointer move/leave와 down/up API를 추가한다. `WM_MOUSEMOVE/LEAVE` → `NativePointerPhase.Hover/Removed` → `PointerDataPacket` → `PointerHoverEvent` → render hit-test → MouseTracker annotation enter/exit → `WidgetState.hovered` → Ink overlay repaint의 단계별 좌표·device·view·counter를 기록하고 최초 단절 지점을 실패 원인으로 남긴다.
- 클릭은 hover와 별도로 `down/up` packet, hit-test path, gesture arena/TapGestureRecognizer, `InkResponse.onTap`, `CalendarDatePicker.onDateChanged`까지 causal id를 전달한다. callback 직접 호출이나 설명 문자열만 있는 trace는 input PASS로 세지 않는다.
- desktop host가 standard method codec으로 `flutter/mousecursor/activateSystemCursor`를 해석해 현재 view/window의 `IPlatformServicesHostCapability.SetCursor()`로 연결한다. Flutter가 선언하는 system cursor kind를 host/platform/Win32까지 전수 매핑하고 지원하지 않는 kind는 silent success가 아닌 명시적 unsupported로 보고한다.
- `SimpleWindow`의 `WM_SETCURSOR`는 `lParam`의 hit-test code가 `HTCLIENT`일 때만 Flutter cursor를 적용한다. border/corner/caption 등 non-client 영역은 `DefWindowProc`에 위임해 OS cursor와 resize behavior를 보존하며, client cursor 변경이 non-client cursor를 오염시키지 않게 한다.
- M6 CalendarDatePicker에서 창 밖 → 활성 날짜 셀 → 비활성 날짜 셀 → 창 밖 순서의 실제 hover와 활성 날짜 down/up을 주입한다. 각 단계의 screenshot/state/cursor/callback을 수집하고 선택일·semantic value·raster가 동일한 입력 인과 id로 변경되는지 검증한다.
- 기존 component matrix의 `interactive` 의미를 `nativeInputObserved && targetHit && callbackObserved && stateOrRasterObserved`로 강화한다. G6-0~G6-5의 direct-callback-only 항목은 실제 입력 재실행 전까지 `notVerified`로 재분류하고 aggregate 수치와 문서가 이를 숨기지 못하게 한다.
- client mouse cursor와 window chrome cursor를 분리한 cross-backend contract test를 둔다. 우선 Windows Win32 shell을 실제 HWND로 검증하고, Avalonia backend는 동일 ownership 계약을 별도 gate로 실행하기 전까지 `notVerified`로 유지한다.

완료 gate:

- IR의 Dart `implements`와 generated C# annotation contract가 일치하고 `RenderMouseRegion` enter/exit가 각각 정확히 1회 발생; clean/incremental regenerate 및 reviewed promotion PASS
- 실제 좌표 기반 hover에서 CalendarDatePicker 활성 날짜의 `WidgetState.hovered`와 overlay raster가 변하고 leave 후 복귀; 비활성 날짜는 click callback 0
- 실제 down/up에서 선택 날짜 callback 정확히 1회, selected date/semantics/raster 일치; direct callback invocation 0으로 M6 native interaction PASS
- 명시적 `basic/click/text/precise/forbidden/none`과 Flutter system cursor 전수 mapping contract PASS; 누락 채널·미지원 silent success 0
- `HTCLIENT`에서는 framework cursor, 좌/우/상/하와 네 corner에서는 OS resize cursor가 일치하고 drag resize 후 client metrics/surface generation/presentation 정상
- 100회 hover enter/leave, click 및 8방향 resize 반복 후 stuck hover/capture/cursor 0, failed/cancelled frame 0, HWND/WGL/cursor resource balance 유지
- pinned Flutter desktop와 동일 fixture에서 hover/click state differential PASS. 날짜 셀의 기본 arrow는 reference 동작으로 기록하고 제품별 hand cursor override는 parity 수치와 분리
- G6-5 component matrix와 aggregate evidence에서 실제 input을 거치지 않은 항목은 `interactive`로 집계되지 않으며 G6-6은 본 gate 완료 전 진입하지 않음

산출물:

- `Doroti/migration/flutter-framework/g6-pointer-interaction-evidence.json`
- `Doroti/migration/flutter-framework/g6-win32-cursor-chrome-evidence.json`
- `Doroti/eng/validate-g6-pointer-interaction.ps1`
- `Doroti/eng/validate-g6-win32-cursor-chrome.ps1`

현재 결과 (2026-08-14):

- compiler가 Dart `implements MouseTrackerAnnotation`을 structural C# companion interface로 내리며 `RenderMouseRegion : ... IMouseTrackerAnnotation`과 explicit callback adapter를 clean Services/Rendering candidate에서 재생성했다. Material clean regenerate도 nullable `InkResponse` callback을 non-null forwarding closure가 아닌 nullable method group으로 보존하고 promotion evidence에 compiler identity/SHA를 기록했다.
- Windows strict-GPU native pointer gate는 창 밖 → 활성 14일 hover → leave/re-enter → down/up → 비활성 15일 → leave의 실제 Win32 message/packet 경로를 통과했다. 활성 hover 5,430 pixels, max channel delta 15, leave 복귀 5,430 pixels, callback 1회/selected `2026-08-14`/direct callback 0이며 비활성 날짜 callback은 0이다.
- hover가 육안으로 보이지 않던 원인은 `Paint.color`의 alpha와 `RasterPaint.Opacity`에 같은 alpha를 중복 적용해 8% state layer가 약 0.64%로 축소된 host bridge였다. Flutter paint → desktop display-list 변환에서 color alpha를 한 번만 적용하도록 수정하고 검증 gate를 target alpha 도달 및 max channel delta 8 이상으로 강화했다.
- `flutter/mousecursor` standard method codec/handler와 Flutter system cursor 36종 mapping을 연결했다. unknown kind는 explicit `unsupported-cursor`; 실제 HWND에서 36 client cursor, `HTCLIENT` ownership, 8 non-client edge/corner OS cursor가 일치했다. 날짜 셀의 기본 cursor는 Flutter reference 정책대로 `basic`이다.
- 100회 hover enter/leave/down/up 스트레스에서 enter/exit/down/up 각 100, callback delta 0, stuck hover/capture 0이다. cursor/chrome 100회 stress와 8회 programmatic resize도 failed/cancelled/software fallback 0 및 HWND/WGL resource balance를 유지했다.
- component matrix의 direct-callback-only 23개 항목은 `notVerified-native-input`으로 내렸고 실제 native input 집계는 `CalendarDatePicker` 1/24만 verified다. physical hover overlay와 click은 사용자 수동 입력으로 별도 확인했다. 실제 border drag resize, pinned Flutter desktop differential, Avalonia/Linux/macOS는 아직 `notVerified`이므로 G6-5R-I 전체 상태는 진행 중으로 유지한다.

### G6-6 — Cupertino, adaptive와 Widget Previews live coverage

진입 조건: G6-5R 완료.

작업:

- Cupertino theme/navigation/form/text/dialog/selection component를 C0~C4 dependency wave로 실행한다.
- adaptive constructor가 platform capability에 따라 Flutter source와 동일한 widget/behavior를 선택하는지 검증한다.
- Material과 Cupertino 화면을 같은 DemoApp/session에서 전환하고 theme/focus/navigation/resource leak을 검사한다.
- Widget Previews는 metadata/build contract뿐 아니라 selected preview가 실제 mount/layout/paint되는 경로를 검증한다.

완료 gate:

- DemoApp Cupertino Tier A component `presented`/`interactive` 100%
- platform 비의존 public Cupertino UI component library 중 90% 이상 `presented`
- adaptive reference differential PASS
- Material/Cupertino 반복 전환 100회 후 window/context/listener/ticker leak 0
- Widget Previews selected component actual frame PASS
- Cupertino public API diff 0 유지

산출물:

- `Doroti/migration/flutter-framework/g6-cupertino-component-matrix.json`
- `Doroti/migration/flutter-framework/g6-adaptive-preview-evidence.json`
- `Doroti/eng/validate-g6-cupertino-wave.ps1 -Wave <C0..C4>`

### G6-7 — 일반 Dart DemoApp cutover, package와 performance

진입 조건: G6-3~G6-6 완료.

목적: handwritten C# composition fixture를 최종 제품으로 남기지 않고 일반 Dart application compiler와 promoted framework package 경로를 증명한다.

작업:

- Goal6 DemoApp 화면을 Dart source package로 두고 일반 package/import/asset/font/localization/plugin pipeline으로 생성한다.
- generated app은 promoted `Doroti.Flutter.Framework.*`, Hosting과 target package만 참조한다.
- repository 밖 isolated restore/build/publish/run에서 같은 component/interaction scenario를 실행한다.
- clean/incremental compiler, startup, first frame, steady frame, memory/handle/ticker/listener를 수치화한다.
- compatibility disposition의 temporary rule을 제거하거나 명시적 blocker로 남긴다.

완료 gate:

- handwritten fixture와 generated Dart DemoApp behavior/visual/semantics differential PASS
- repository-private candidate/project reference 0
- fresh package-only consumer의 strict-GPU first frame와 app-essential scenario PASS
- clean/incremental output identity PASS
- compiler/test shard 각각 20분 이내, first-frame과 sustained budget 수치 기록
- unsupported syntax/plugin/capability silent success 0

산출물:

- `DorotiDemoApp/dart/`
- `Doroti/migration/flutter-framework/g6-generated-demo-evidence.json`
- `Doroti/artifacts/g6-release/<version>/`
- `Doroti/eng/validate-g6-generated-demo.ps1`

### G6-8 — target 확장과 physical final verification

진입 조건: G6-7 완료. framework live/component gate보다 먼저 시작하지 않는다.

작업:

- G5에서 미착수한 Linux X11/Wayland와 macOS target source-port/package를 현재 framework contract에 맞춰 구현한다.
- Windows와 최소 한 개 non-Windows physical target에서 같은 packaged Dart DemoApp을 실행한다.
- mouse, precision touchpad, touch, keyboard, Korean IME, clipboard, cursor, multi-monitor DPI와 accessibility external client를 검증한다.
- resize/minimize/restore, surface/device loss, 30분 strict-GPU sustained run과 install/run/uninstall lifecycle을 기록한다.

완료 gate:

- Windows physical input/IME/accessibility/multi-monitor/DPI/sustained GPU PASS
- 최소 한 개 physical non-Windows target의 GPU/input/text/scale/clipboard/accessibility/package lifecycle PASS
- automated result와 physical result가 분리된 target matrix 존재
- 실행하지 못한 OS/backend/device를 성공으로 기록한 항목 0
- release provenance, license, independent rebase와 package hash evidence PASS

산출물:

- `Doroti/artifacts/g6-physical/<rid>/`
- `Doroti/migration/flutter-avalonia/bridge-validation/g6-targets.json`
- `Doroti/eng/validate-g6-target.ps1 -Rid <rid>`

## 4. 단계 의존성과 중단 규칙

```text
G6-0 truth reset
  -> G6-1 language/runtime structural closure
  -> G6-2 base Widgets live first frame
  -> G6-3 minimum Material DemoApp
  -> G6-4 app-essential slices
  -> G6-5 Material majority coverage
  -> G6-5R Material visual/generation fidelity audit
  -> G6-5R-I native pointer/cursor and Win32 chrome closure
  -> G6-6 Cupertino/adaptive/previews
  -> G6-7 generated Dart app/package/performance
  -> G6-8 cross-target physical verification
```

- 새 component crash가 lower layer 공용 의미 문제이면 해당 lower milestone fixture와 regression을 먼저 보강한다.
- component별 임시 수정으로 다음 wave를 진행하지 않는다.
- completed milestone의 live gate가 깨지면 후속 wave를 중단하고 최초 regression부터 복구한다.
- Windows live first frame 전에는 Linux/macOS target 작업을 시작하지 않는다.
- physical 장치가 없는 항목은 `notVerified`로 유지하되 automated native gate를 대신 실패시키지는 않는다.

## 5. 필수 validation 계약

모든 테스트 명령은 `.github/copilot-instructions.md`에 따라 20분 timeout 이내의 shard로 구성한다. 전체 aggregate가 20분을 넘으면 timeout을 늘리지 않고 wave/slice를 더 나눈다.

```powershell
# compiler/runtime structural tests
./Doroti/eng/validate-g6-language-runtime.ps1

# actual native Widgets first frame
./Doroti/eng/validate-g6-widgets-live.ps1

# always-blocking minimum Material DemoApp
./Doroti/eng/validate-g6-material-demo.ps1

# cumulative app/component waves
./Doroti/eng/validate-g6-app-slice.ps1 -Slice N0
./Doroti/eng/validate-g6-material-wave.ps1 -Wave M0
./Doroti/eng/validate-g6-material-fidelity.ps1
./Doroti/eng/validate-g6-pointer-interaction.ps1
./Doroti/eng/validate-g6-win32-cursor-chrome.ps1
./Doroti/eng/validate-g6-cupertino-wave.ps1 -Wave C0

# generated external app/package
./Doroti/eng/validate-g6-generated-demo.ps1

dotnet build Doroti/Doroti.slnx --configuration Release --nologo
dotnet format Doroti/Doroti.slnx --verify-no-changes --no-restore --verbosity minimal
git diff --check
```

각 live validator는 최소한 다음을 자동 검사한다.

- first FlutterError/unhandled exception 0
- native window, target/RID와 backend identity
- strict GPU와 software fallback 여부
- build/layout/paint/present counter 및 non-empty pixel bounds
- component/state/interaction/semantics scenario 결과. interaction은 실제 native input 좌표, target hit, callback과 state/raster 변화의 인과 trace가 모두 있을 때만 PASS
- frame terminal ACK, window/context/resource/ticker/listener count
- compiler/source/candidate/product/package digest
- 실행하지 않은 gate와 blocker

## 6. 공통 구현 원칙

- Flutter source가 framework behavior의 단일 owner다. Avalonia source-port는 native window/input/text/accessibility/GPU capability만 소유한다.
- generated `.g.cs` 직접 수정은 원인 확인용 실험에만 허용하며 제품 수정으로 인정하지 않는다. 최종 변경은 analyzer/IR/lowerer/runtime 또는 reviewed promotion source에서 재생성되어야 한다.
- filename/local-number 기반 문자열 치환은 새 공용 해법이 아니다. 불가피한 temporary rule에는 fixture, owner, 제거 milestone과 failure diagnostic을 둔다.
- API census와 build 0 errors는 유지하되 runtime coverage와 분리한다.
- synthetic shell/property trace는 `syntheticContract`로만 기록한다. 실제 mount/layout/paint/present를 거치지 않으면 gallery behavior/visual PASS로 부르지 않는다.
- normal build/run은 빠르게 유지한다. architecture, full census, clean regeneration과 physical suite는 명시적 validator로 분리한다.
- Windows 결과를 Linux/macOS 결과로 확대하지 않는다.
- 성능은 elapsed time을 1순위, memory를 2순위로 기록하되 first-frame latency, steady frame과 resource leak을 별도 gate로 둔다.

## 7. Goal6 최종 완료 정의

Goal6는 다음이 모두 사실일 때 완료한다.

- generated Dart `DorotiDemoApp`이 promoted framework/release package만 사용해 native strict-GPU frame을 표시한다.
- 최소 DemoApp Tier A component가 모두 `presented`이고 상호작용/semantics가 필요한 항목은 각각 `interactive`/`semantic`이다.
- platform 비의존 public Material 및 Cupertino UI component library의 90% 이상이 실제 `presented` 상태다.
- navigation, overlay, form/text/IME, scrolling, resource, accessibility와 plugin vertical slice가 누적 PASS다.
- component coverage matrix가 compile/API/synthetic/live/physical 상태를 분리하고 미지원 항목을 숨기지 않는다.
- analyzer/IR/lowerer/runtime의 공용 의미 수정으로 clean regenerate가 가능하며 direct generated hotfix가 0이다.
- Windows와 최소 한 개 physical non-Windows target에서 packaged app 핵심 scenario와 lifecycle이 PASS다.
- compiler identity, Flutter/Avalonia provenance, package digest, performance, resource와 physical evidence가 release artifact에 포함된다.
- 실행하지 않은 syntax/component/OS/backend/device/IME/accessibility 항목을 성공으로 기록한 경우가 0이다.

Goal6의 성공 기준은 “Flutter 파일 대부분이 C#으로 존재한다”가 아니다. 일반 Dart app이 실제 Flutter framework 의미를 통해 component를 mount/layout/paint/present하고, 사용자의 입력과 native accessibility action이 같은 widget state까지 왕복하며, 그 범위가 component별 증거로 측정되는 제품 상태다.
