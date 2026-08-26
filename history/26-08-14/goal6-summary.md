# Doroti 6차 목표 — live Flutter framework bring-up과 DemoApp component coverage [종료]

> 상태: **종료(미완료 항목 Goal7 이관)** — G6-0~G6-5의 검증된 기반은 보존하고, 재개방된 fidelity/input/compositing 및 후속 release gate는 완료로 주장하지 않음
> 작성일: 2026-08-12
> 종료일: 2026-08-14
> 후속 기록: [`goal7-summary.md`](../26-08-16/goal7-summary.md) — 제품 정확성 closure, Web build와 multi-target release
> 기준 Doroti revision: `21ebdbbe36691d8e30d66114f39e8a00aa339c43` + 문서 이관 작업 트리
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`

## 종료 결정

Goal6는 Goal5의 compile/API/synthetic 성공을 실제 framework runtime 성공과 분리하고, 일반 Dart/Flutter widget tree가 Windows native strict-GPU surface에서 실행되는 제품 기반을 만들었다. Widgets 첫 frame, 최소 Material DemoApp, 일반 앱 필수 slice와 Material component 90% presented coverage까지는 자동화된 Windows x64 증거를 확보했다.

그러나 component가 보인다는 사실을 Flutter와의 시각적 일치나 실제 native input 성공으로 확대할 수 없다는 결함도 확인했다. 이에 G6-5R visual/generation, G6-5R-I pointer/cursor, G6-5R-C scene/compositing을 재개방했다. 이 세 gate와 Cupertino/adaptive, generated app differential, physical/cross-target 검증이 모두 닫히기 전에 하위 단계 일부가 선행 구현되어 roadmap 상태가 길고 혼재하게 됐다.

따라서 Goal6를 다음 기준으로 종료한다.

- 아래 “완료로 보존” 항목만 해당 절의 자동화 범위에서 PASS로 유지한다.
- partial 결과는 구현 자산과 현재 증거를 보존하되 milestone 완료로 승격하지 않는다.
- callback 직접 호출은 native pointer interaction 증거가 아니다.
- 실행하지 않은 reference differential, browser, physical device, IME/accessibility와 cross-target 항목은 `notVerified`다.
- 남은 작업은 Goal7의 새 dependency graph와 evidence gate로 이관했으며, 종료 상태는 [`goal7-summary.md`](../26-08-16/goal7-summary.md)에 보존한다.

## 완료로 보존하는 범위

### G6-0 — truth reset과 재현 가능한 live baseline

- evidence를 compile/API, synthetic, managed, native presented와 physical 범위로 분리했다.
- Material/Cupertino public candidate를 자동 census하고 constructed 이후 상태를 기본 `notVerified`로 두었다.
- 최초 framework 오류를 timeout이 아니라 widget/library/source pin/generated digest를 포함한 taxonomy로 기록하는 live smoke를 만들었다.

### G6-1 — Dart 의미·생성기·runtime structural closure

- nullable/super formal, constructor, collection element, extension, generic boundary와 `Future<T>` 등 공용 의미를 micro fixture와 typed IR/lowerer/runtime에서 복구했다.
- clean/incremental 생성 identity, compiler regression과 제품 build를 통과했다.
- generated `.g.cs` 직접 hotfix 없이 producer 경로에서 재생성하는 원칙을 고정했다.

### G6-2 — Widgets live core와 base first frame

- actual HWND와 strict `skia-wgl-opengl-gpu`에서 Widgets tree의 mount/build/layout/paint/present를 연결했다.
- single/multi-child lifecycle, resize/minimize/restore, focus와 semantics 최소 왕복, 30초/300-frame cadence 및 native resource balance를 자동 검증했다.
- 이 증거는 Windows x64 automated native 범위이며 physical/cross-target 증거가 아니다.

### G6-3 — 최소 Material DemoApp

- `DorotiDemoApp`을 promoted Material 제품 프로젝트를 사용하는 단일 reviewed 진입점으로 전환했다.
- `MaterialApp.builder`와 `home` 경로에서 Theme, Scaffold, AppBar, Card, ListTile, FAB, 주요 selection/action control, layout과 scrolling을 strict-GPU로 표시했다.
- 실제 Win32 pointer가 노출한 hit-test storage, covariant event, painter adapter와 `List.sublist` lowering 결함을 공용 producer에서 수정하고 regression으로 고정했다.
- package-only 외부 consumer와 30초/300-frame smoke를 통과했다.

### G6-4 — 일반 앱 필수 vertical slice

- navigation, overlay, form, scrolling, resource/localization, accessibility와 platform channel 7개 slice를 독립 actual HWND로 실행했다.
- native wheel/drag/keyboard scroll, external UI Automation action과 unsupported capability exact failure를 검증했다.
- physical Windows IME/accessibility와 다른 OS/RID는 `notVerified`로 유지했다.

### G6-5 — Material majority presented coverage

- M0~M6 7개 wave를 actual HWND strict-GPU frame으로 실행했다.
- platform-independent Material family 60개 중 54개(90%), Tier A 22/22를 최소 `presented`로 기록했다.
- 미지원 6개 family의 symbol, 원인, owner와 후속 조건을 matrix에 남겼다.
- 기존 24/24 interactive 수치는 direct-callback fixture 범위였으므로 native input 성공으로 사용하지 않는다.

## 부분 구현으로 보존하고 Goal7로 이관하는 범위

### G6-5R — Material visual fidelity와 생성 품질

확보한 결과:

- Roboto/Material Icons 자산과 license, 주요 glyph tofu 방지, RRect/stroke/shadow 전달과 Material Color Utilities `fromSeed` differential을 복구했다.
- compatibility/review 영향 규칙 771개를 inventory화했고 widget type 대체를 0으로 만들었다.
- Windows strict-GPU M0~M6 누적 실행과 M6 capture identity를 기록했다.

열린 gate:

- pinned Flutter `CalendarDatePicker` reference raster와 glyph/baseline/selected-today/corner/shadow differential
- reviewed generated-file 보정 193개와 숫자 local명 의존 규칙 74개 제거
- 전체 temporary compatibility inventory의 구조적 제거 또는 명시적 disposition

### G6-5R-I — native pointer, cursor와 Win32 chrome

확보한 결과:

- `MouseTrackerAnnotation`을 structural C# contract로 생성하고 실제 Win32 hover/down/up 인과 trace를 연결했다.
- CalendarDatePicker hover/leave/click, paint alpha 단일 적용, system cursor 36종과 client/non-client cursor ownership을 자동 검증했다.
- direct-callback-only 23개 family를 `notVerified-native-input`으로 내렸고 native input verified는 1/24로 기록했다.

열린 gate:

- 나머지 interactive family의 실제 좌표 기반 input
- 실제 border drag resize와 pinned Flutter desktop differential

### G6-5R-C — scene/compositing/paint effect

확보한 결과:

- scene/canvas operation 52개를 census하고 typed paint/filter/scene payload와 balanced effect scope를 도입했다.
- group opacity, `saveLayer`, foreground/backdrop filter, color filter/blend의 managed 및 Windows strict-GPU bounded 경로를 구현했다.
- DemoApp의 native blur ON/OFF/ON과 ROI 변화, strict-GPU frame/resource balance를 통과했다.

열린 gate:

- pinned Flutter reference differential과 C1 effect family
- retained replay/generation, path 및 DPI 1.25/2.0 differential
- 100회 toggle/scroll/resize stress와 predecessor managed regression 복구
- C2 operation, physical GPU와 cross-target backend

### G6-6 — Cupertino, adaptive와 Widget Previews

확보한 결과:

- Cupertino public candidate 55/55가 21개 strict-GPU frame에서 construct/mount/layout/paint/presented를 통과했다.
- selected Widget Preview actual frame과 Cupertino public API diff 0을 확인했다.

열린 gate:

- Tier A component별 native interaction/semantics
- Flutter adaptive reference differential
- 같은 session의 Material/Cupertino 100회 전환과 resource leak 검사
- G6-5R 세 선행 gate 완료

### G6-7 — 일반 Dart DemoApp, package와 performance

확보한 결과:

- 일반 Dart package를 compiler로 생성하고 promoted framework/hosting/target package만 참조하는 외부 consumer를 publish/run했다.
- asset/font/localization/plugin ABI, clean/incremental identity와 실제 Flutter SDK analyze diagnostics 0을 검증했다.
- package-only Windows strict-GPU first frame, native button 변화, semantics와 sustained frame 증거를 만들었다.

열린 gate:

- handwritten/generated app의 전체 behavior/visual/semantics pinned differential
- source inventory의 temporary compatibility rule 440개 제거 또는 blocker disposition
- 100회 launch/toggle 안정성과 선행 G6-5R/G6-6 완료

### G6-8 — target 확장과 physical final verification

- Linux X11/Wayland, macOS, physical Windows/non-Windows, 장시간 GPU와 install lifecycle은 시작하지 않았다.
- Web target/build도 Goal6 범위에 구현되지 않았다.

## Goal7 이관 blocker

1. Material pinned reference raster와 generated/review compatibility debt를 닫는다.
2. native interactive family를 direct callback이 아닌 실제 input 인과 trace로 재검증한다.
3. scene/compositing C1, retained generation, reference/stress와 predecessor regression을 닫는다.
4. Cupertino/adaptive/generated Dart product differential을 완료한다.
5. `browser-wasm` Web target/host/build/publish/live validation을 새로 구축한다.
6. Windows physical 및 최소 한 개 non-Windows physical target을 자동화 결과와 분리해 검증한다.

## 보존된 주요 evidence와 validator

- `Doroti/migration/flutter-framework/g6-component-coverage.json`
- `Doroti/migration/flutter-framework/g6-compatibility-audit.json`
- `Doroti/migration/flutter-framework/g6-pointer-interaction-evidence.json`
- `Doroti/migration/flutter-framework/g6-scene-operation-matrix.json`
- `Doroti/migration/flutter-framework/g6-compositing-effects-evidence.json`
- `Doroti/migration/flutter-framework/g6-cupertino-component-matrix.json`
- `Doroti/migration/flutter-framework/g6-generated-demo-evidence.json`
- `Doroti/eng/validate-g6-material-demo.ps1`
- `Doroti/eng/validate-g6-material-fidelity.ps1`
- `Doroti/eng/validate-g6-pointer-interaction.ps1`
- `Doroti/eng/validate-g6-compositing-effects.ps1`
- `Doroti/eng/validate-g6-cupertino-wave.ps1`
- `Doroti/eng/validate-g6-generated-demo.ps1`

> 이전 역사 기록: [`goal5-summary.md`](../26-08-12/goal5-summary.md)
> 문서 성격: Goal6의 종료 시점 요약과 증거 경계. Goal7 종료 상태는 [`goal7-summary.md`](../26-08-16/goal7-summary.md)에 보존한다.
