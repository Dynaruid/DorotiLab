# Doroti 5차 목표 — Flutter Framework 제품 경로 완결과 다중 target release [종료]

> 상태: **종료(미완료 항목 Goal6 이관)** — G5-0~G5-6W의 당시 산출물은 보존하되 Goal5 최종 완료를 주장하지 않음
> 종료 판정: reviewed framework의 compile/API coverage와 synthetic gallery가 실제 `DorotiDemoApp` Material frame 실행 가능성을 증명하지 못함
> 작성일: 2026-08-10
> 종료일: 2026-08-12
> 후속 계획: [`goal6.md`](../../goal6.md) — live framework bring-up과 component runtime coverage를 우선 수행
> 기준 Doroti revision: `b94daa9e255536de7c8c774c97b31ac963ba0132` + 현재 compiler 작업 트리
> Flutter source pin: `56b8e1a851a594b1a154f8ea93270807dab22b9a`
> Avalonia source pin: `f159423f691946e713f454447a780d4677d8a0d2`

## 종료 결정

Goal5는 2026-08-12 기준으로 더 진행하지 않는다. G5-0~G5-6W에서 생성기, public API, reviewed aggregate build, application/package boundary와 Windows RID synthetic host까지는 의미 있는 산출물을 만들었지만, 실제 `DorotiDemoApp`에 reviewed `MaterialApp`/`Scaffold`/`AppBar`를 연결하자 첫 프레임 전에 framework runtime 오류가 연속해서 드러났다.

확인된 공백은 nullable super parameter 전달, binding/mixin 초기화, implicit view, generic covariance, `Future` 값 보존, restoration, route dispatch, animation controller 초기화, multi-child render object 연결과 Material theme/component 기본값 등이다. 이는 한 위젯의 국소 결함이 아니라 analyzer/IR/lowerer/runtime/Widgets/Material을 가로지르는 bring-up 공백이다.

따라서 다음과 같이 상태를 고정한다.

- G5의 `✅`는 각 절에 적힌 당시 compiler/build/API/synthetic 범위만 뜻한다. live Material/Cupertino application readiness로 확대하지 않는다.
- G5-4 gallery differential은 constructor/property/callback contract 기반 synthetic trace이며 실제 native widget mount/layout/paint/present 증거가 아니다.
- G5-6L, G5-6M, G5-7과 G5-8은 실행하지 않았다. 이 항목과 모든 physical `notVerified` ledger는 Goal6 후반부로 이관한다.
- Goal5 최종 완료 정의는 충족되지 않았다. 실패를 성공으로 바꾸지 않고 역사적 계획과 산출물만 보존한다.
- 이후 작업은 [`goal6.md`](../../goal6.md)의 live-first gate를 따른다. 실제 DemoApp 첫 프레임이 통과하기 전에는 full framework, gallery behavior 또는 application-ready를 완료로 기록하지 않는다.

> 이전 역사 기록: [`goal4-summary.md`](../26-08-10/goal4-summary.md)
> 문서 성격: 종료된 roadmap과 당시 산출물의 역사 기록. 현재 active roadmap은 [`goal6.md`](../../goal6.md)

Goal5는 Goal4에서 만든 Flutter/Avalonia 경계를 제품 전체에 끝까지 적용한다. 목표는 compiler가 많은 파일을 생성하는 것이 아니라, reviewed Flutter framework가 UI 의미의 유일한 owner가 되고 Avalonia source-port가 native/platform 실행의 유일한 owner가 되는 배포 가능한 제품 경로다.

## 0. 완료 작업 요약

### ✅ G5-0 — Evidence truth reset과 compiler 회귀 closure (2026-08-10)

- evidence를 `previousBaseline`/`currentRun`으로 분리하고 candidate, aggregate, promotion, product behavior, native evidence를 독립 상태로 고정했다.
- G4-3은 58 libraries, G4-4는 42 outputs / 353 declarations, G4-5는 101 outputs / 663 declarations 기준으로 aggregate warning/error 0과 관련 promotion/product/package 회귀를 통과했다.
- 40-cell full performance matrix와 measured run 120회를 완료했다. elapsed budget, cache footprint, compiler/solution format, Release build와 `git diff --check`가 모두 PASS다.

### ✅ G5-1 — Physics·Animation·Gestures 자동화/product cutover (2026-08-10)

- compatibility recognizer를 제거하고 reviewed Gestures가 recognizer/animation policy의 유일한 owner가 됐다. host adapter에는 raw packet 변환과 lifetime handoff만 남겼다.
- Win32 `WM_POINTER`, capture-loss exactly-once cancel, arena/ticker/reference parity, actual HWND synthetic input과 180-frame pacing을 검증했다.
- public API, owner audit, package-only consumer와 `eng/validate-g4-4.ps1`이 PASS다. physical mouse/trackpad/touch recording은 완료 범위에 포함하지 않으며 최종 G5-8까지 `notVerified`로 유지한다.
- evidence: `Doroti/migration/flutter-avalonia/bridge-validation/g5-1.json`, `Doroti/artifacts/g5-1/`

### ✅ G5-2 — Painting·Rendering·Semantics Windows automated current-machine closure (2026-08-10)

- 663/663 declarations를 reviewed framework source/package로 승격했고 public API missing/extra, product `.g.cs`, duplicate product owner가 모두 0이다.
- typed Canvas/Scene, HarfBuzz, image/resource lifetime, one-in-flight/one-latest mailbox와 terminal ACK를 실제 HWND/WGL strict-GPU 경로에서 검증했다. 241 submitted, 181 presented, queue high-watermark 2, recovery 1이며 visual MAE는 `0.0002607421875 <= 2.0`이다.
- 실제 `WM_GETOBJECT/UIAutomationCore` entrypoint와 외부 UI Automation client를 통해 semantics tree 및 focus/invoke/toggle/setText/scroll round trip을 검증했다.
- 11개 dependency package와 repository 밖 consumer, `eng/validate-g4-4.ps1`, 전체 Release solution build가 PASS다. physical cross-monitor DPI와 Linux/macOS 실기기 실행은 최종 G5-8 범위다.
- evidence: `Doroti/migration/flutter-avalonia/bridge-validation/g5-2.json`

### ✅ G5-3 — Avalonia 실행 기반과 Widgets/Dart application vertical cutover (2026-08-12)

- `g5-3-current68` 185 files / 1,428 diagnostics / 211 errors 기준선과 6-category taxonomy를 동결했고 분류율 100%, unclassified 0을 기록했다. G4-3 fresh Scheduler/Services compile/API/promotion/package/regression도 0 warning / 0 error로 복구했다.
- actual HWND에서 bootstrap → attach → request frame → WGL GPU present → terminal ACK → detach/shutdown을 동일 view identity로 검증했다. 12개 typed capability, Flutter surface 1, frame clock owner 1, official Avalonia binary 및 mirror control tree 0이다.
- W0–W7 누적 dependency slice를 각각 clean/incremental byte identity와 0 warning / 0 error로 빌드했다. root/stateless/stateful/key/focus/action/overlay/route/scroll/image/editable-text product behavior gate가 모두 PASS다.
- full 186-library / 1,715-declaration candidate를 185개 파일로 clean regenerate해 0 warning / 0 error로 닫고 `Doroti.Flutter.Framework.Widgets`에 승격했다. `widgets.dart` exported library 169개, public declaration occurrence 952개, API missing/extra 0, disposition 1,715개, unowned 0, handwritten Widget/Element product owner 0이다.
- navigation, route, `EditableText`를 포함한 실제 Dart application을 1개 generated C# file로 변환·빌드했다. 12개 NuGet package를 repository 밖 isolated consumer에서 restore/build/run했으며 repository-private fallback은 0이다.
- 자동화 증적은 `Doroti/migration/flutter-framework/g5-3-evidence.json`을 aggregate index로 사용한다. physical Windows IME/accessibility/sustained GPU/cross-monitor DPI는 성공으로 간주하지 않고 G5-8 `DorotiDemoApp`까지 `notVerified`로 유지한다.

---

## 1. Roadmap

### ✅ 1.1 G5-3 — Avalonia 실행 기반과 Widgets/Dart application vertical cutover

진입 조건: G5-2 완료.

G5-3의 중심은 `211 errors -> 0` 반복이 아니라 Flutter Widgets 의미를 실행 가능한 dependency slice로 살리는 framework bring-up이다. Avalonia source-port는 Widget/Element/State 의미를 구현하지 않으며, Flutter 코드가 OS와 만나는 지점의 window, dispatcher, frame, input, IME, clipboard, cursor, surface/GPU와 accessibility capability만 제공한다.

공통 경계:

- `Doroti.Flutter.Framework.Widgets`에는 Avalonia type/reference와 Avalonia Control/Visual/Layout/Styling/XAML owner를 넣지 않는다.
- WidgetsBinding과 framework source는 concrete host를 찾지 않고 per-view typed capability만 요구한다.
- Avalonia source 추가는 `migration/avalonia-shell/port-selection.json`의 pinned revision에서 필요한 symbol/dependency closure만 선택한다. `import`/`adapt`/`Doroti-port`/`exclude-with-owner` disposition, license, source hash와 local adaptation owner를 모두 기록한다.
- 기존 G5-1/G5-2 Windows source-port 기능을 재작성하거나 다시 성공으로 포장하지 않는다. vertical slice에서 드러난 platform gap만 보강하고, official Avalonia binary 및 전체 Controls tree를 우회책으로 추가하지 않는다.
- generated `.g.cs`와 Migration IR 직접 수정, slice 전용 product shim, unsupported capability의 silent no-op를 금지한다.

#### G5-3A — 기준선 동결과 predecessor closure 복구

작업:

- `g5-3-current68`의 compiler identity, selection hash, candidate digest, 185 files / 1,428 diagnostics / 211 unique C# errors를 재현 가능한 기준선으로 동결한다. 이후 candidate 이름 증가는 진척 증거로 사용하지 않는다.
- 211개 오류를 `compiler-lowering`, `dart-runtime`, `dart-ui-contract`, `predecessor-framework`, `dart-model-representation`, `host-capability/avalonia-port`로 전수 분류한다. 각 항목에 Dart source symbol, generated C# location, 최초 필요 slice, 실제 수정 owner와 재현 test를 연결한다.
- G4-3 Scheduler/Services를 현재 compiler와 의도된 제품 계약에서 fresh regenerate한다. Services compile error를 0으로 만들고 product API `missing 6 / extra 6` drift를 review된 promotion baseline으로 해소한다.
- historical candidate를 현재 제품에 억지로 맞추지 않고 current product contract, fresh candidate와 package consumer를 함께 다시 고정한다.

완료 gate:

- baseline 재생성 byte identity PASS, error taxonomy 분류율 100%, unclassified 0
- G4-3 fresh aggregate 0 warnings / 0 errors, reviewed API missing/extra 0
- Scheduler/Services behavior, promotion, package-only consumer와 G4-0/1/2 regression PASS
- predecessor evidence와 G5-3 Widgets evidence가 별도 상태로 기록됨

#### G5-3B — selected Avalonia Windows application foundation

작업:

- 기존 `Doroti.Vendor.Avalonia.Base/Skia/Win32`와 `Doroti.Host.Desktop(.Flutter)`를 시작점으로, generated Dart entrypoint가 reviewed framework binding, per-view host, 단일 native window와 단일 Flutter surface까지 부팅되는 Windows application path를 만든다.
- pinned Avalonia source에서 다음 platform closure를 dependency-driven으로 재감사한다: dispatcher/render timer, window/view lifecycle, logical/physical size와 DPI, focus/capture/raw input, IME/text-input caret, clipboard/cursor, screen/monitor, WGL/OpenGL surface·present·device-loss, UI Automation provider.
- 기존 local port가 계약을 만족하면 재사용하고, 누락 symbol만 `Doroti.AvaloniaPort`의 selection/graph/stage/review 흐름으로 추가한다. upstream source와 local adaptation의 normalized hash, owner와 제외 사유를 갱신한다.
- framework scheduler와 native dispatcher 사이 frame request owner를 하나로 만들고, beginFrame/drawFrame, mailbox terminal ACK와 shutdown/dispose 순서를 한 lifecycle trace로 잇는다.
- capability registry는 view별로 구성하고 missing/unsupported capability를 capability ID, target identity와 호출 Flutter symbol을 포함한 명시적 진단으로 실패시킨다.

완료 gate:

- selected Avalonia closure `unclassified=0`, source/license/hash/provenance audit PASS
- product graph의 official Avalonia binary와 Avalonia Controls/Visual/Layout/Styling/XAML dependency 0
- actual HWND에서 bootstrap -> attach root -> request frame -> GPU present -> terminal ACK -> shutdown automated smoke PASS
- resize/DPI, pointer/key/focus, text-input, clipboard/cursor, accessibility capability가 동일 view identity로 왕복하고 trace 누락/중복 0
- native shell의 Flutter surface 1, Avalonia mirror control tree 0, frame clock/request owner 1
- physical input/IME/cross-monitor DPI/sustained GPU는 실행하지 않고 G5-8까지 `notVerified`

#### G5-3C — Widgets diagnostic vertical slices

아래 batch는 최종 범위를 줄이는 단계가 아니라 compile 문제의 실제 owner를 찾고 behavior를 검증하기 위한 진단·승격 단위다. 각 batch는 pinned Flutter source의 dependency closure를 계산하며 앞 batch를 포함한 누적 회귀를 통과해야 한다.

| Slice | reviewed Flutter closure | 필수 실행/behavior gate | platform 접점 |
|---|---|---|---|
| W0 bootstrap | BindingBase, WidgetsBinding, BuildOwner, root attach | application entrypoint, attach/detach, 첫 frame lifecycle | view lifecycle, frame dispatch |
| W1 stateless tree | Widget, Element, BuildContext, StatelessWidget/Element | mount/build/update/deactivate/unmount 순서와 tree 결과 | 없음 |
| W2 stateful lifecycle | StatefulWidget, State, StatefulElement, `setState` | init/changeDependencies/build/update/deactivate/dispose, dirty coalescing | frame request |
| W3 dependency/key | InheritedWidget/Element, LocalKey, GlobalKey | dependency invalidation, keyed reconciliation, GlobalKey reparent | 없음 |
| W4 focus/action | FocusManager/Node, Actions, Shortcuts | focus traversal, shortcut -> intent -> action, focus loss | focus, keyboard |
| W5 overlay/route | Overlay, Navigator, Route, dialog | push/pop/replace, overlay order, modal result와 disposal | window/view lifecycle |
| W6 scrolling/image | Scrollable, controller/position/physics, image widgets | 1,000-item list, scroll extent/reuse, async image completion/lifetime | pointer signal, image/resource |
| W7 editable text | EditableText, selection, composition, caret, text actions | edit state/revision, selection/composition/caret/action/detach | IME/text input, clipboard |

각 slice 작업 계약:

1. Flutter source/export/dependency manifest와 public API 기대값을 먼저 고정한다.
2. clean candidate를 생성·컴파일하고 오류를 G5-3A taxonomy에 귀속한다.
3. 오류를 공용 analyzer/typed IR/lowerer, Dart runtime, `dart:ui`, predecessor framework 또는 typed host capability의 실제 owner에서 수정한다.
4. generated candidate 검토 후 해당 slice만 reviewed product source/package로 승격하고 duplicate handwritten owner를 제거한다.
5. 동일 scenario의 Flutter reference trace와 C# managed trace를 event order, state snapshot, final tree/semantics 기준으로 비교한다.
6. platform 접점이 있는 slice만 G5-3B Windows host에서 automated native round trip을 추가하고, 이전 slice와 G5-1/G5-2 regression을 다시 실행한다.

slice 완료 gate:

- candidate compile 0 warnings / 0 errors와 clean/incremental byte identity PASS
- slice public API missing/extra 0, review disposition 100%, unowned/unclassified 0
- reference behavior trace와 managed product trace parity PASS
- promoted symbol의 handwritten duplicate lifecycle/policy owner 0
- 필요한 capability의 native round trip PASS 또는 platform 접점 없음이 manifest에 명시됨
- candidate, promotion, product behavior, automated native와 physical evidence가 각각 별도 status로 기록됨

#### G5-3D — full Widgets aggregate, product cutover와 external Dart app

작업:

- W0-W7에서 얻은 공용 규칙으로 full 186-library / 1,715-declaration Widgets candidate를 clean regenerate한다. 211개 기준선이 0으로 수렴해야 하지만 compile 0만으로 milestone을 완료하지 않는다.
- `widgets.dart` export/public API를 전수 review하고 `Doroti.Flutter.Framework.Widgets` package로 승격한다.
- `Doroti.Widgets`와 `Doroti.Engine`의 handwritten Widget/Element/BuildOwner/composition owner를 단계별로 제거하며, 제거마다 W0-W7 누적 behavior gate를 실행한다.
- navigation, dialog, EditableText, image와 1,000-item list를 포함한 Dart application을 G5-3B composition root에서 실행한다.
- 같은 app을 repository 밖 isolated NuGet consumer에서 generate/restore/build/run하고 repository-private candidate/compiler fallback이 없음을 감사한다.

G5-3 최종 완료 gate:

- full 186-library / 1,715-declaration aggregate 0 warnings / 0 errors, `widgets.dart` public export/API 100%
- W0-W7 key reconciliation, rebuild, focus, route/overlay, text editing, image와 long-list differential PASS
- handwritten C# Widget/Element lifecycle owner 0
- Avalonia Control mirror tree 0, native shell에는 단일 Flutter surface만 존재
- actual Windows native-host resize/pointer/keyboard/text-input/accessibility/GPU automated scenario PASS
- repository 밖 generated Dart app consumer automated run PASS
- G5-8 `DorotiDemoApp`이 사용할 capability/diagnostic hook 준비 완료
- physical Windows IME/accessibility/GPU/DPI 실행은 G5-8까지 `notVerified`

완료 결과 (2026-08-12):

1. `g5-3-current68`과 211-error taxonomy 동결, G4-3 Services fresh predecessor closure 복구 PASS.
2. selected Avalonia source-port 기반 W0 actual HWND/frame/application foundation PASS.
3. W0–W7 clean/incremental identity, compile, review/promotion과 product behavior 누적 gate PASS.
4. full Widgets aggregate/API/disposition/owner audit와 external generated Dart app package-only gate PASS.
5. physical Windows IME/accessibility/sustained GPU/cross-monitor DPI는 G5-8까지 `notVerified`.

상세 상태는 `migration/flutter-framework/g5-3-evidence.json`을 aggregate index로 두고 taxonomy, slice 및 native evidence를 하위 artifact로 연결한다.

### 1.2 G5-4 — Material·Cupertino·Widget Previews와 full framework ✅ (compile/API/synthetic 범위)

진입 조건: G5-3 완료.

작업:

- `widget_previews.dart`와 base framework public root/export graph를 완료한다.
- Material primitive/theme/shape/ink부터 component family 순서로 승격한다.
- Cupertino theme/navigation/form/text/dialog/selection을 dependency batch로 승격한다.
- adaptive behavior는 Flutter source가 결정하고 OS capability 값만 host에서 받는다.
- 13개 public root와 695-file census를 current pin에서 다시 계산한다.

완료 gate:

- public root/export/file census 100%, unclassified/unowned 0
- Material/Cupertino public API manifest diff 0
- gallery behavior/visual/input/semantics differential PASS
- 같은 source-ported shell에서 Material과 Cupertino app 모두 실행
- Avalonia Controls/theme/XAML dependency 0

완료 결과 (2026-08-12):

1. Flutter `56b8e1a…` source lock에서 13 public root/695 Dart file을 재계산하고, 694 resolved libraries/5,355 declarations/49,007 members의 현재 source SHA 일치를 확인했다.
2. 9개 Material/Cupertino/Widget Previews batch, 252 product libraries/2,091 declarations를 compiler error·unclassified AST·silent omission 0으로 생성하고 249-file reviewed aggregate를 warning/error 0으로 빌드했다.
3. `material.dart` 181 exported libraries/521 public declarations와 `cupertino.dart` 52/121 public declarations의 API manifest diff 0을 확인했다.
4. 동일 source-ported gallery shell에서 `MaterialApp`과 `CupertinoApp`의 behavior/visual/input/semantics differential이 PASS했다.
5. reviewed framework와 gallery의 Avalonia Controls/theme/XAML reference 및 XAML file count는 모두 0이다.
6. physical Windows IME/accessibility/sustained GPU/cross-monitor DPI는 정책대로 G5-8까지 `notVerified`다.

상세 상태는 `migration/flutter-framework/g5-4-evidence.json`과 `docs/architecture/g5-4-material-cupertino-full-framework.md`에 기록한다.

### 1.3 G5-5 ✅ — 일반 application compiler와 asset/plugin boundary (compiler/package 자동화 범위)

진입 조건: G5-3 완료. G5-4와 dependency가 겹치지 않는 범위는 병행 가능하다.

작업:

- 일반 Dart package/import/conditional graph를 reviewed framework package에 bind한다.
- asset, font, localization manifest를 UI capability와 host resource loader로 전달한다.
- generated application은 framework package와 host bootstrap package만 참조한다.
- platform channel plugin의 Dart API/codec과 target native handler를 분리한다.
- native plugin ABI를 RID별 package와 capability manifest에 등록한다.
- changed library와 dependent SCC만 재생성하고 clean/incremental byte identity를 검증한다.
- fixture-name hard-code와 historical generated application fallback을 제거한다.

완료 gate:

- Material app 2개, Cupertino app 1개, base Widgets/Semantics app 1개 생성·실행
- asset/font/localization/platform channel이 clean external consumer에서 동작
- generated app source의 platform/vendor concrete reference 0
- clean/incremental output byte identity PASS
- unsupported plugin/capability의 silent success 0

완료 결과 (2026-08-12):

1. application entry point와 `package_config.json`에서 일반 package/import/conditional graph를 발견하고, fixture별 library/symbol 입력 0으로 reviewed Material/Cupertino/Widgets framework에 bind했다.
2. 2개 Material, 1개 Cupertino, 1개 base Widgets application을 각각 clean/incremental 생성하고 warning/error 0으로 빌드했으며, 격리 NuGet consumer에서 네 application의 생성 타입을 실행했다.
3. asset/font/localization manifest를 embedded capability manifest와 `IApplicationResourceHostCapability`로 연결하고, 외부 consumer에서 SHA-256/길이 무결성과 각 resource payload를 확인했다.
4. Dart `MethodChannel`/`StandardMethodCodec`과 `win-x64` native handler를 분리하고, `doroti.plugin-abi/v1` RID package 및 capability manifest로 ABI를 검증했다.
5. changed library와 dependent SCC 3개만 재생성하고 unaffected conditional branch 1개를 재사용했으며, 네 application의 clean/incremental product byte identity가 모두 PASS했다.
6. generated application의 직접 참조를 selected framework와 `Doroti.Flutter.Hosting`으로 제한하고 platform/vendor concrete reference 및 repository-private fallback을 0으로 확인했다.
7. 미지원 RID plugin은 정확히 한 개의 `DOTAPP005`로 실패하며 silent success는 0이다. physical Windows plugin/font/localization UI는 G5-8까지 `notVerified`다.

상세 상태는 `migration/flutter-framework/g5-5-evidence.json`과 `docs/architecture/g5-5-application-compiler-boundary.md`에 기록한다.

### 1.4 G5-6 — Target source-port와 실기기 검증 준비

G5-3B가 Windows에서 Flutter application을 실행하기 위한 selected source-port와 capability closure를 먼저 만든다. G5-6은 그 경로를 RID package로 고정하고 Linux/macOS의 동등한 target closure를 구성하는 단계다. framework bring-up에 필요한 Windows 기반을 G5-6까지 미루지 않는다.

이 단계는 target 구현, build/package와 자동화 진입점까지만 완료한다. physical machine/device 실행은 하지 않으며 모든 실기기 gate는 G5-8까지 `notVerified`로 유지한다.

#### G5-6W — Windows 준비 ✅ (RID package/synthetic host 범위)

- G5-3B의 WGL/OpenGL strict GPU, input/IME, clipboard, accessibility, monitor/DPI와 recovery capability를 Windows RID package에 고정하고 source-port selection/provenance를 release input으로 승격한다.
- `DorotiDemoApp`에서 사용할 frame/input/automation/resource diagnostic hook와 target identity 수집 계약을 준비한다.
- Windows publish artifact와 isolated restore/build/synthetic smoke를 통과시킨다.

완료 결과 (2026-08-12):

1. `Doroti.Target.Windows.win-x64/0.2.0-beta` RID package를 추가하고 Win32 lifecycle/input/IME/clipboard/UIA와 strict WGL/OpenGL/Skia/recovery closure를 고정했다.
2. package 내부에 target manifest, reviewed `port-selection.json`, A1/A2/vendor provenance aggregate와 third-party notice를 포함하고 각 SHA-256 및 Avalonia pin을 검증했다.
3. 실제 attached Flutter view의 12개 capability ID와 `doroti.target-identity/v1`, `doroti.desktop-flutter-target-diagnostics/v1`을 고정했다. frame/input/automation/resource hook는 published consumer에서 수집됐다.
4. repository 밖 package-only consumer를 isolated NuGet cache로 restore/build/publish하고 actual HWND synthetic pointer/key, `WM_GETOBJECT`, strict GPU, terminal ACK와 injected recovery smoke를 통과했다.
5. publish/product graph의 official Avalonia binary와 repository-private fallback은 0이며, RID host 재pack 전후 Widgets framework package SHA-256은 동일했다.
6. physical mouse/precision touchpad/touch/Korean IME/cross-monitor DPI/external accessibility/sustained GPU는 성공으로 사용하지 않았고 G5-8까지 `notVerified`다.

상세 상태는 `migration/flutter-avalonia/bridge-validation/g5-6w.json`, `migration/flutter-avalonia/target-capabilities/win-x64.json`과 `docs/architecture/g5-6w-windows-rid-package.md`에 기록한다.

#### G5-6L — Linux Avalonia source-port 준비 (미착수, Goal6 이관)

- pinned Avalonia의 X11/Wayland/FreeDesktop/Skia closure를 stage/review한다.
- Windows와 같은 capability ID를 Linux implementation에 등록한다.
- Linux RID package와 `DorotiDemoApp` target build/publish 경로를 구성한다.
- X11/Wayland physical run은 수행하지 않고 G5-8까지 각각 `notVerified`로 남긴다.

#### G5-6M — macOS Avalonia source-port 준비 (미착수, Goal6 이관)

- managed macOS bridge와 필요한 Avalonia.Native closure를 재현 가능한 native build로 구성한다.
- source revision, flags, architecture, signing/notarization input과 license를 기록한다.
- macOS RID package와 `DorotiDemoApp` target build/publish 경로를 구성한다.
- physical macOS run은 수행하지 않고 G5-8까지 `notVerified`로 남긴다.

공통 완료 gate:

- target product graph의 official Avalonia binary dependency 0
- framework package 재생성 없이 host/RID package를 교체할 수 있음
- Windows/Linux/macOS target package와 `DorotiDemoApp` build/publish 진입점 존재
- capability/diagnostic contract와 target identity schema가 RID 간 일치
- physical target/backend/device 결과를 G5-6 성공 판정에 사용한 항목 0

### 1.5 G5-7 — Packaging, release와 독립 rebase (미착수, Goal6 이관)

진입 조건: G5-4, G5-5와 G5-6 target 준비 완료. 실기기 검증은 진입 조건이 아니며 G5-8에서 수행한다.

작업:

- Runtime, UI contract, framework, host와 RID native package dependency graph를 고정한다.
- isolated NuGet cache와 repository 밖 consumer에서 restore/build/publish 및 자동화 smoke를 수행한다.
- trimming, single-file, ReadyToRun, native asset probing과 source/crash mapping을 검증한다.
- capability matrix와 `notVerified` ledger를 release artifact에 포함한다.
- Flutter/Avalonia license, revision, selected source와 local adaptation provenance를 포함한다.
- Flutter revision만 변경한 framework rebase와 Avalonia revision만 변경한 platform rebase를 분리 실행한다.
- API/capability 변경은 명시적 bridge compatibility review 없이는 product에 반영하지 않는다.

완료 gate:

- framework-only package의 vendor/native/private compiler binary 0
- RID host package의 official Avalonia binary 0
- native asset 누락과 암묵적 software fallback 0
- Windows와 Linux/macOS 중 최소 한 개 non-Windows RID external consumer restore/build/publish PASS
- Flutter-only/Avalonia-only rebase report와 conflict가 독립적으로 재현됨
- 승인 없는 source/API/capability 변경의 product 유입 0

### 1.6 G5-8 — `DorotiDemoApp` 기반 최종 실기기 검증 (미착수, Goal6 이관)

상태: **Goal5의 가장 마지막 작업 — 그전까지 모든 physical gate는 `notVerified`**

진입 조건: G5-3, G5-4, G5-5, G5-6과 G5-7 완료.

목적: 완성된 framework/host/RID release package를 사용하는 `DorotiDemoApp` 하나로 누적된 모든 실기기 gate를 마지막에 일괄 검증한다. 중간 milestone은 physical gate 때문에 막지 않되, G5-8 전에는 Goal5를 완료 처리하지 않는다.

`DorotiDemoApp` 변경:

- reviewed Widgets/Material/Cupertino 경로로 navigation, dialog, EditableText, image, asset/font/localization, platform channel과 1,000-item list를 포함한 최종 검증 화면을 구성한다.
- final verification mode가 G5-7 release package와 RID native asset을 isolated restore하여 사용하게 한다. repository-private compiler/candidate와 제품 밖 project reference에 의존하지 않는다.
- interactive physical-input recorder와 target identity 수집을 추가해 mouse, precision touchpad, touch, keyboard, Korean IME, clipboard와 cursor event의 source/timestamp/physical·logical coordinate를 기록한다.
- monitor 이동, 100/125/150/200% DPI, resize/minimize/restore, strict GPU, surface/device-loss recovery와 장시간 frame mailbox/resource metric을 한 run에서 수집한다.
- native accessibility entrypoint를 외부 client에서 읽고 focus/invoke/toggle/setText/scroll을 Flutter callback까지 왕복시킨다.
- Windows와 최소 한 개 physical non-Windows target에서 같은 scenario/app identity를 실행한다. Linux는 X11/Wayland, macOS는 architecture/signing/backend identity를 구분한다.
- 실행할 수 없는 OS/backend/device/IME/accessibility 항목은 환경과 이유를 기록하고 `notVerified`로 유지한다.

최종 실행 계약:

```powershell
DorotiDemoApp --verify-g5-8 --duration-minutes 30 --artifact-root <target-artifact-path>
```

`--verify-g5-8`은 target identity를 자동 기록하고 automated scenario 뒤 interactive physical-input/IME/accessibility checklist를 이어서 실행한다. 옵션과 evidence schema는 G5-8 구현 시 고정한다.

최종 완료 gate:

- G5-1에서 이월된 physical mouse/trackpad/touch recording과 reference differential PASS
- Windows physical multi-monitor/DPI, pointer/keyboard/Korean IME, clipboard, accessibility와 strict-GPU sustained run PASS
- 최소 한 개 physical non-Windows target에서 GPU, input/text, scale, clipboard, accessibility와 packaged app run PASS
- release package만 사용하는 fresh `DorotiDemoApp` install/run/uninstall 또는 동등한 배포 lifecycle PASS
- frame latency, dropped/replaced frame, terminal ACK, resource/handle count와 peak memory가 tolerance와 함께 기록됨
- physical action과 native accessibility action이 Flutter behavior/state update로 왕복
- 모든 artifact가 `Doroti/artifacts/g5-8-doroti-demo-app/<rid>/`에 있고 종합 evidence가 `migration/flutter-avalonia/bridge-validation/g5-8.json`에 기록됨
- 실행하지 않은 target/backend/device를 성공으로 기록한 항목 0

## 2. 공통 진행 기준

최종 제품 경로는 다음 하나다.

```text
Dart application
  -> reviewed Doroti.Flutter.Framework.* packages
  -> Doroti.Flutter.Ui managed dart:ui contract
  -> Doroti.Flutter.Hosting
  -> Doroti.Host.Desktop.Flutter composition bridge
  -> Doroti shell/graphics neutral contracts
  -> Doroti.Vendor.Avalonia.* source-port
  -> native OS + GPU
```

- Flutter `packages/flutter/lib`가 framework 동작을, Avalonia source-port가 native/platform 실행을 소유한다.
- Flutter Engine/embedder/native platform source와 official Avalonia binary package를 제품 compile/dependency graph에 넣지 않는다.
- Avalonia Controls, Visual/Layout tree, Styling, XAML과 theme를 Flutter UI tree의 대체 owner로 사용하지 않는다.
- `Doroti.Flutter.Runtime`은 Dart language/runtime 의미만 소유한다.
- bridge는 변환, marshalling, capability registration과 lifetime handoff만 수행하며 gesture/layout/native event/GPU 정책의 owner가 될 수 없다.
- Windows 성공을 Linux/macOS 성공으로 확대하지 않는다.

candidate, aggregate build, API review/promotion, product owner cutover, managed behavior, automated native-host, package-only consumer, performance와 physical-target evidence를 독립적으로 기록한다. 앞 단계의 PASS는 뒤 단계의 PASS가 아니며 unrun 또는 blocked gate를 skip-success로 기록하지 않는다.

generated `.g.cs`와 Migration IR을 직접 고쳐 오류를 숨기지 않는다. 해결은 공용 analyzer/typed IR/lowerer/runtime contract 또는 reviewed promotion source에 귀속한다.

남은 필수 제품 순서는 다음과 같다. G5-3의 compile, managed behavior와 automated native-host gate는 현재 machine에서 닫되, physical device/monitor/IME와 cross-OS 검증은 package/rebase까지 고정한 뒤 `DorotiDemoApp`을 수정해 G5-8에서 한 번에 수행한다.

```text
G5-3A baseline taxonomy + predecessor closure
  -> G5-3B selected Avalonia Windows application foundation
  -> G5-3C W0-W7 Widgets vertical slices
  -> G5-3D full Widgets aggregate + Dart application cutover
  -> G5-4 Material·Cupertino·Widget Previews full framework
  -> G5-5 general app/assets/plugins compiler
  -> G5-6 Windows RID packaging + Linux/macOS source-port readiness
  -> G5-7 package/release/rebase
  -> G5-8 DorotiDemoApp physical target validation
```

G5-1 physical input, G5-2 physical cross-monitor DPI, G5-3 physical IME/accessibility/GPU와 G5-6 cross-OS 실행 gate는 G5-8로 이월한다. 이월된 항목은 G5-8 실행 전까지 `notVerified`이며, 선행 milestone의 compiler/product/automated-native 완료를 막지 않는다.

성능 판정은 elapsed time 우선, memory 2순위다. memory는 evidence에 관찰값으로 기록하되 elapsed budget을 만족하는 구현을 memory 단독 이유로 실패시키지 않는다.

## 3. 필수 validation 명령

모든 long command의 timeout은 15분이다. 각 milestone은 아래 중 자신이 변경한 경로뿐 아니라 명시된 predecessor gate를 함께 실행한다.

```powershell
Push-Location tools/Doroti.DartToCSharp/analyzer
dart format --output=none --set-exit-if-changed .
dart analyze
dart test
Pop-Location

dotnet build tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj --configuration Release --nologo
dotnet run --project Doroti/validation/Doroti.Validation.Compiler --configuration Release -- --refactor-only

Push-Location Doroti
./eng/doroti.ps1 validate -ValidationSuite compiler
./eng/validate-g4-3.ps1
./eng/validate-g4-4-compiler.ps1
./eng/validate-g4-5-compiler.ps1
./eng/validate-g4-4.ps1
./eng/measure-dart-to-csharp.ps1
Pop-Location

dotnet build Doroti/Doroti.slnx --configuration Release --nologo
dotnet format Doroti/Doroti.slnx --verify-no-changes --no-restore --verbosity minimal
git diff --check
```

Native automated gate는 compile suite와 구분한다. physical/interactive input·IME·accessibility, cross-monitor visual/DPI, sustained performance와 cross-OS gate는 G5-8의 `DorotiDemoApp` 실기기 run으로만 닫으며, machine/device/backend, raw trace와 tolerance를 별도 artifact에 남긴다.

G5-3 구현 시 다음 전용 entrypoint를 추가한다. 이름만 존재하는 script를 성공 증거로 삼지 않고 각 script가 자신이 소유한 evidence를 원자적으로 갱신해야 한다.

```powershell
Push-Location Doroti

# G5-3A: current68 freeze/taxonomy + G4-3 fresh promotion closure
./eng/validate-g5-3-predecessor.ps1

# G5-3B: pinned Avalonia selection/audit + actual HWND application bootstrap
dotnet run --project tools/Doroti.AvaloniaPort/Doroti.AvaloniaPort.csproj -- audit
./eng/validate-g5-3-platform-foundation.ps1

# G5-3C: W0-W7 individually; each run includes all completed predecessor slices
./eng/validate-g5-3-slice.ps1 -Slice W0

# G5-3D: full aggregate, promotion, owner audit, app and external consumer
./eng/validate-g5-3.ps1

Pop-Location
```

## 4. Artifact 계획

기존 artifact를 유지하되 schema를 현재 truth model에 맞춘다.

- `Doroti/migration/flutter-avalonia/source-boundary.json`
- `Doroti/migration/flutter-avalonia/capability-map.json`
- `Doroti/migration/flutter-avalonia/current-owner-audit.json`
- `Doroti/migration/flutter-avalonia/bridge-validation/<milestone>.json`
- `Doroti/migration/flutter-framework/<milestone>-evidence.json`
- `Doroti/migration/flutter-framework/<milestone>-api-manifest.json`
- `Doroti/migration/flutter-framework/<milestone>-disposition.json`
- `Doroti/migration/flutter-framework/g5-3-error-taxonomy.json`
- `Doroti/migration/flutter-framework/g5-3-slices.json`
- `Doroti/migration/flutter-framework/g5-3-slices/<slice>-selection.json`
- `Doroti/migration/flutter-framework/g5-3-slices/<slice>-evidence.json`
- `Doroti/migration/flutter-avalonia/bridge-validation/g5-3-platform-foundation.json`
- `Doroti/migration/flutter-avalonia/bridge-validation/g5-3-slices/<slice>.json`
- `Doroti/artifacts/validation/dart-to-csharp-performance.json`
- `Doroti/artifacts/validation/target-capabilities/<rid>.json`
- `Doroti/artifacts/validation/native-input/<rid>/`
- `Doroti/artifacts/validation/rendering/<rid>/`
- `Doroti/artifacts/validation/automation/<rid>/`
- `Doroti/artifacts/release/<version>/`
- `Doroti/artifacts/g5-8-doroti-demo-app/<rid>/`
- `Doroti/migration/flutter-avalonia/bridge-validation/g5-8.json`

각 evidence에는 최소한 compiler identity, Flutter/Avalonia revision, selection hash, candidate digest, product digest, validation command, target identity, status, blocker와 `notVerified` 목록을 포함한다. G5-3 slice evidence는 추가로 dependency closure, public API diff, taxonomy 항목, promotion disposition, reference/product behavior trace digest, capability ID와 `candidate`/`promotion`/`behavior`/`automatedNative`/`physical` 독립 status를 기록한다.

## 5. Goal5 원래 최종 완료 정의 (미충족)

Goal5는 다음이 모두 사실일 때만 완료할 예정이었으나, 종료 시점에 충족되지 않았다.

- 목표 Flutter framework closure가 reviewed C# packages로 존재하고 public API diff가 0이다.
- Scheduler, Services, Gestures, Painting, Rendering, Semantics, Widgets, Material과 Cupertino가 같은 Dart application에서 연결된다.
- Flutter framework assembly는 host/vendor/native type을 모르고 Avalonia vendor assembly는 Flutter framework type을 모른다.
- Flutter framework behavior의 handwritten 중복 owner와 Avalonia Control/Layout mirror tree가 없다.
- 모든 low-level call은 typed UI/service capability를 통과하며 unsupported capability의 silent success가 없다.
- G5-7 release package를 사용하는 `DorotiDemoApp`이 Windows와 최소 한 개 physical non-Windows target에서 strict GPU, input/IME, accessibility와 배포 lifecycle gate를 통과한다.
- frame mailbox terminal ACK, resize/DPI/device-loss와 sustained-runtime evidence가 수치로 존재한다.
- external automation client action이 native provider를 거쳐 Flutter semantics로 왕복한다.
- full compiler performance matrix가 elapsed budget을 통과한다. memory는 별도 관찰 지표로 기록한다.
- Flutter/Avalonia provenance, license, target matrix, `notVerified` ledger와 독립 rebase report가 release artifact에 포함된다.
- G5-8보다 앞선 milestone에서 실기기 gate를 완료 조건으로 요구하거나 성공으로 기록한 항목이 없다.
- 실행하지 않은 OS, backend, device, IME, GPU 또는 accessibility gate를 성공으로 기록한 항목이 없다.

성공 기준은 많은 facade나 candidate가 존재하는 것이 아니다. reviewed Flutter source가 UI framework 의미를 소유하고, reviewed Avalonia source-port가 native 실행을 소유하며, 두 영역 사이의 모든 호출이 typed boundary와 최종 `DorotiDemoApp` 실기기 evidence로 검증된 배포 제품이어야 한다.
