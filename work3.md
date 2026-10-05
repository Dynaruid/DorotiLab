# work2 후속 작업계획 — AppKit·Mac Catalyst·iOS·Android 및 잔여 수락

- 작성일: 2026-10-05 KST
- 상태: **PARTIAL — 공통·AppKit·Mac Catalyst·iOS 구현 및 범위별 검증 진행. 전체 완료 아님.** 최신 결과와 남은 기준은 [§14](#14-2026-10-05-실행-결과)에 기록한다.
- 기준: [work2.md](work2.md)의 구조·소유권·공개 계약과 현재 작업 트리. 후속 실행 항목은 이 문서에서 관리하고, 기존 설계·완료 체크·검증 이력은 work2에 유지한다.
- 현황 근거: [최신 checkpoint](Doroti/docs/migrations/design-platform/latest-checkpoint.json), [재개 기록](Doroti/docs/migrations/design-platform/resume-2026-10-05.md), [재개 검증 receipt](Doroti/docs/migrations/design-platform/resume-verification-2026-10-05.json), [실행 상태](Doroti/docs/migrations/design-platform/execution.json).
- 조사 방식: §1의 표와 우선 연결 문제는 실행 전 기준이다. 기존 PASS와 이번 실행의 새 PASS를 구분하며 새 실행 근거는 §14와 별도 receipt에 둔다.
- 범위: 네 플랫폼의 신규 계약 이식, 공통 디자인/Windowing 미구현, Qt·Windows·Web 잔여 수락, 도구·SDK·NuGet·문서 정리. `tools/Doroti.DartToCSharp/`는 계속 제외한다.

## 1. 현재 현황과 이어서 해결할 문제

work2의 디자인 이동·Runtime 색상 의존 제거·자산 등록, 디자인별 테스트 분리와 세 템플릿 선택은 이미 구현됐다. DS2·DS3·DS4, DS6-2~4, DS7-1~3/5, DS8-1은 기존 완료 범위를 유지하고 관련 변경 시 회귀를 확인한다. 미완료 체크가 있는 단계에도 부분 구현·통과 기록이 있으므로 항목 전체를 처음부터 재작성하지 않는다.

| 범위 | 현재 소스·기록에서 확인한 기반 | 남은 작업과 수락 |
|---|---|---|
| 공통 구조 | Ui의 typed Window/frame 계약, Hosting staged coordinator, shared application tree, provider/Target schema, 독립 버전 pack 경로 | 모든 provider의 실제 startup/종료 연결, 완전한 API·소유권 감사, 실패/경쟁·trim/AOT 수락 |
| AppKit macOS | `AppKitDesktopWindowHost/Policy`, factory의 shared session, OS 종료 조정, `macos_smoke.py` | 신규 WindowRequest 연결, 보조 창 역할·NSMenu, thread/좌표/close 수명, 현재 구조의 macOS build/runtime |
| Mac Catalyst | 별도 `MacCatalystDesktopWindowHost/Policy`, scene delegate, UIKit/Graphite 경로 | scene과 application 수명 연결, 신규 WindowRequest와 표시 정책 충돌 해결, 실제 지원 capability·메뉴·입력·재생성 |
| iOS | provider/Target, UIKit handler·Metal/Graphite, device/simulator/NativeAOT 관련 props와 기존 개발 helper | staged adopt/attach, 장치 검색·개발 전송의 typed 서비스 연결, 프로파일 테스트 이행, simulator/device별 수락 |
| Android | provider/Target, Vulkan handler와 surface generation/retirement, typed ADB 장치 검색·RID 필터·run 선택 | activity/surface 수명 연결, 개발 전송·owned Stop/Restart, 프로파일·NuGet 소비자·실제 장치 수락 |
| WindowsAppSdk·Windows MAUI | Windows shared tree/창 종류/메뉴/디자인 표현·NuGet 소비자, MAUI Graphite integration의 기존 PASS | G1 slow consumer·modal/close 재진입·late IME, 물리 입력·mixed DPI·접근성 및 확장 동작 수락 |
| Qt·Web | Qt strict WSLg/Wayland subset와 profile fixture, Web 독립 NuGet/Offline 첫 frame·focus의 기존 PASS | Qt 추가 창 종류·OS menu·staged lifecycle, Web 모든 profile의 등록/실패/Stop 및 실제 입력 수락 |
| 도구·IDE | managed CLI, installed provider 발견/거절 fixture, doctor 계약, VS Code argv/Stop 등의 기존 PASS | builtin template/native 서비스, 모바일 전송, 실제 editor Run/Reload/Restart/Stop, 독립 tool lifecycle·호환 조합 |

현재 `wholePlanComplete=false`다. `currentFailing=[]`는 수행한 검사의 알려진 실패가 해결되었다는 기록이며 위 미구현과 미확인 범위의 완료를 의미하지 않는다.

### 소스 대조에서 확인한 우선 연결 문제

1. [공통 WindowRequest 변환](Doroti/src/Doroti.Desktop/DorotiWindowManager.Windowing.cs)은 `StartupVisibility=Manual`, 보조 창은 `SkipTaskbar=true`를 지정한다. AppKit 정책은 창별 Dock 숨김을 거절하고, Catalyst 정책은 `PlatformDefault` 이외의 최초 표시를 거절한다. 공통 요청과 각 OS의 기능을 구분해 연결해야 하며 policy 거절을 제거하는 것만으로 지원을 선언하지 않는다.
2. AppKit/Catalyst factory에는 shared session이 있지만, 완전한 staged coordinator·application dispatcher·view별 서비스·최종 drain 연결은 별도로 감사해야 한다. iOS/Android의 surface별 생성·재연결도 동일 application 수명과 구분한다.
3. [MAUI tooling Extension](packages/platforms/maui/tooling/Extension.cs)의 iOS device 서비스는 현재 `unsupported-service`이고 Android/iOS `dev`는 광고하지 않는다. Android 장치 선택 fixture PASS는 mobile 개발이나 장치 배포 PASS가 아니다.
4. [Apple profile 검사](Doroti/tests/apple_build_profiles.py)는 없어진 `Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.IosNativeAot.props`와 템플릿 복사본의 동일성을 아직 요구한다. 실제 소유자인 `packages/platforms/build/`와 독립 provider 소비 계약으로 전환해야 한다.
5. MAUI Host에 샘플 friend assembly가 남고, Apple/Android 샘플·템플릿에 native bridge/binding·빌드 입력이 남아 있다. framework 공통 구현과 앱 고유 native module을 구분한 후에 이동·제거한다.
6. [현재 소유권 목록](Doroti/docs/migrations/design-platform/current-ownership-2026-10-05.json)은 파일·선언의 textual inventory다. 평가된 모든 플랫폼 그래프와 Roslyn/public API의 semantic audit를 대신하지 않는다.

## 2. 유지할 계약과 완료 판단

하나의 application은 하나의 논리 widget tree와 framework build owner를 소유한다. native 창/scene/surface는 WindowId·viewId·generation을 가지며, focus·IME·Semantics·Navigator/restoration·frame 자원은 view별로 격리한다. native factory는 앱 entrypoint나 widget root를 생성하지 않는다.

시작 순서는 `정적 launch plan → PrepareProcess → 앱 startup/Configure → descriptor → OS primary 생성 또는 adopt → view 서비스 등록/Seal → branch attach → FirstFrameSubmitted`다. ProcessPrepared/ViewAttached/FirstFrameSubmitted를 서로 다른 receipt로 기록한다. 이미 OS가 띄운 scene의 최초 표시 제약은 해당 provider의 capability에 반영한다.

내부 .NET 호출은 typed interface/request/result·Task/ValueTask·취소/context를 사용한다. 직렬화는 JS/worker, 실제 process wire, native ABI, manifest/기록 경계에 둔다. Owned/Borrowed·application/view/frame lease와 Future 완료 순서, bounded queue·GPU consumer 완료 이전의 자원 보존을 유지한다.

Native/Auto/Overlay는 요청에 필요한 전체 기능으로 평가한다. Auto의 Overlay 선택은 Unsupported일 때만 허용하며, owner 오류·취소·native 실행 실패를 Overlay 성공으로 바꾸지 않는다. 지원하지 않는 WindowKind·modal·placement·close-veto는 명시적으로 거절한다.

공통 core/design/tool 빌드는 다른 OS workload나 provider native 재빌드를 요구하지 않아야 한다. provider와 Target은 별도 버전·core 범위·protocol/native ABI를 갖고, 독립 소비자는 prebuilt nupkg만으로 연결한다. OS 필수 shell·앱 ID/권한/서명/아이콘·앱 고유 native module은 runner 입력으로 남는다.

각 체크박스는 아래 완료 기준과 해당 환경의 증거가 갖춰질 때 체크한다. 환경이 없어 SKIPPED인 native/device 작업은 완료 체크로 바꾸지 않는다. work2 원 항목은 연결된 모든 work3 항목의 필수 기준이 충족됐을 때만 완료로 갱신한다.

## 3. 실행 순서와 선행 조건

| 순서 | 실행 묶음 | 선행 조건 | 통과 gate |
|---|---|---|---|
| 1 | C1~C6: 현황·빌드·bootstrap·창 요청·소유권 연결 | 현재 작업 트리와 기존 receipt 확보 | G0의 단계/실패 trace·typed 경계·view/frame lease, 플랫폼별 평가 그래프 |
| 2 | U1~U4와 R1: 디자인·G1·Windows 잔여 | C2~C4 | route/result·Tooltip·focus/IME 격리, 실제 Windows 두 창의 확장 수락 |
| 3 | A1~A5: AppKit | 공통 계약과 Apple toolchain 선택 | 현재 구조의 macOS startup·지원 창/메뉴·종료·독립 소비자 |
| 4 | K1~K4: Mac Catalyst | 공통 계약; AppKit에서 발견한 공통 수정 반영 | UIKit scene 수명·지원/거절·멀티 scene shared tree |
| 5 | I1~I5: iOS | 공통 계약과 UIKit 수명 정리 | simulator와 device의 별도 build/runtime·개발·AOT 결과 |
| 6 | D1~D5: Android | 공통 계약; Android profile·허가된 matching device | activity/surface 재생성·개발 전송·장치 Stop/Restart·독립 소비자 |
| 7 | T1~T4, R2~R3: 도구·Qt·Web | 관련 provider 계약 | builtin/외부 확장과 실제 CLI·IDE·OS 실행 |
| 8 | P1~P4, V1~V4: 후보·문서·전체 수락 | 각 플랫폼 구현 및 필요한 회귀 | package/ABI/지원표·증거 일치, 원 계획 잔여 감사 |

이 표는 의존 순서다. Windows에서 진행 가능한 공통 계약·Android 도구/fixture 작업은 Apple 호스트 확보 전에도 수행할 수 있다. Android 장치 실행과 Apple 실행은 가용 장치/호스트를 확인한 뒤 별도 gate로 진행한다. 첫 provider의 G0/G1에 모든 OS 이식 완료를 선행 조건으로 추가하지 않는다.

## 4. 공통 이식 기반

주요 진입점: [DorotiPlatformApplication](Doroti/src/Doroti.Hosting/DorotiPlatformApplication.cs), [SharedHostSession](Doroti/src/Doroti.Hosting/DorotiSharedHostSession.cs), [Windowing 계약](Doroti/src/Doroti.Ui/Windowing.cs), [MAUI Host](packages/platforms/maui/Doroti.Host.Maui/), [provider build](packages/platforms/build/), [MAUI bootstrap](packages/platforms/maui/bootstrap/Doroti.Provider.Bootstrap.targets).

- [ ] **C1 — 소유권·API·실제 지원표 확정.** DS0, PW0, PW1-5를 이어 파일/타입/resource/참조/friend assembly의 구→신 대응과 공개 signature를 평가 그래프·컴파일 진단으로 보완한다. core→design/provider 역참조, Cupertino→Material, 중복 Compile·resource, 숨은 reflection 경로를 검사한다. 참고 Flutter/C# 위치와 미적용 범위도 유지한다. 완료: 각 closure와 실제 구현/검증 capability, 버전·ABI 소유자가 기계 판독 목록과 문서에 일치한다.

- [ ] **C2 — 모든 provider의 staged startup/종료 연결.** MAUI OS entrypoint·builder·surface와 기존 Hosting coordinator 사이의 책임을 통합한다. 정적 plan에서 startup/plugin constructor를 실행하지 않고 PrepareProcess 뒤 생성하며, primary adopt·등록/Seal·attach를 명시한다. OS activation/deep link 입력과 process/application/view별 종료를 연결한다. 완료: 플랫폼별 실제 trace와 중복 Start·부분 등록 실패·초기화 중 Stop·늦은 completion·Restart의 단일 완료/역순 cleanup 증거가 있다.

- [ ] **C3 — 플랫폼별 WindowRequest 매핑.** 공통 manager의 Manual/SkipTaskbar 가정과 각 provider의 표시·Dock·owner·modality·anchor 계약을 정리한다. 논리 창 종류와 OS 정책을 분리하고, 최초 hidden 준비가 불가능한 profile은 그 제한을 선언한다. 완료: 지원 요청은 해당 OS 모델로 전달되고 미지원/잘못된 owner/cycle/generation 요청은 native allocation 전에 거절된다. Windows 동작은 유지된다.

- [ ] **C4 — 공유 tree·서비스·frame 수명.** application dispatcher와 native/render owner를 연결하고 services의 Owned/Borrowed 및 application/view/frame lease를 감사한다. view detach가 다른 창이나 application 서비스를 종료하지 않게 하며 종료 뒤 등록·callback·frame admission을 거절한다. 완료: fake G0에서 partial failure·drop/cancel/resize/device-loss·GPU drain timeout·shared Dispose 단일 실행과 surviving view의 진행을 확인한다.

- [ ] **C5 — 빌드 역할·TFM/RID/profile 분리.** 공통 props/targets, packages/provider Version.props, Apple/MAUI import, 중앙 의존·obj/bin/lock 경계를 평가한다. 기본/명시 profile·RID 없는 restore·design-time·global Version override의 영향을 검사한다. 완료: core/design/tool 전용 빌드에 OS workload·native rebuild가 섞이지 않고 resolved assets/nuspec/AssemblyRef와 개별 버전·README/license/SourceLink가 일치한다.

- [ ] **C6 — provider bootstrap·native·공개 확장 정리.** MAUI 샘플 friend 접근을 사용처별 public/protected 등록·진단 계약으로 대체한다. framework native bridge·ABI/binding·생성 자산·전용 eng는 provider가 공급하도록 하고 runner의 필수 OS shell·앱 고유 module은 보존한다. 완료: 소스 복사 없이 독립 Target 소비자가 bootstrap/native 자산을 사용하고 restore/describe/design-time에서 앱 실행·장치 접속·native 재빌드가 발생하지 않는다. 구 복사본 제거는 전환 검증 뒤 수행한다.

## 5. 공통 디자인·입력·다중 View 잔여

진입점: [NativeWindowPresentation](Doroti/src/Doroti.Framework.Widgets/NativeWindowPresentation.cs), [Cupertino context menu](packages/Doroti.Cupertino/src/context_menu.cs), [RawTooltip](Doroti/src/Doroti.Framework.Widgets/raw_tooltip.cs), [ApplicationViews](Doroti/src/Doroti.Framework.Widgets/DorotiApplicationViews.cs).

- [ ] **U1 — Cupertino context menu와 route coordinator.** native preview/action branch·Auto/Native/Overlay를 연결하고 dialog/menu의 caller theme/locale·Navigator 결과·restoration·focus·Semantics·dismiss/barrier 전달을 완성한다. 완료: 두 디자인의 native/Overlay에서 선택·취소·owner 제거·재생성·reentrant close가 단일 완료이고 필요한 기능 누락은 Unsupported다. 이미 구현된 dialog/theme 캡처를 보존한다.

- [ ] **U2 — Tooltip의 실제 동작과 늦은 설치.** Raw/디자인 wrapper/native branch의 hover·long press·지연·dismiss·no-activate·anchor 갱신을 연결한다. pointer 이탈·owner 닫기·pending show 중 unmount·늦게 준비된 창의 설치 취소를 확인한다. 완료: app별 목록과 lease가 정리되고 focus/IME를 빼앗거나 종료 view를 다시 attach하지 않는다.

- [ ] **U3 — Raw 위젯·디자인 입력 경계.** 메뉴 키보드/Semantics·radio group·ExpansionTile 상태/controller·listener dispose와 core-only EditableText/SelectableRegion fixture를 검증한다. 명시 style/selectionControls/contextMenuBuilder, 디자인별 selection/magnifier/toolbar·page transition 정책을 유지한다. 완료: managed/synthetic 회귀와 플랫폼 물리 IME/접근성 결과를 분리하며 Cupertino의 Android 외관 정책이 보존된다.

- [ ] **U4 — 확장 G1과 API 소유권 감사.** Windows 두 실제 Regular 창에서 shared state/InheritedWidget, 별도 focus·selection/IME·Semantics·Navigator/restoration, primary close 후 survivor, 재생성·slow consumer·modal/close 재진입을 확인한다. late callback은 view/generation으로 거절하고 native factory의 entrypoint/root 결합을 최종 제거한다. 완료: freeze·bounded queue·consumer lease·async scope·최종 drain 증거가 있고 오래된 한 view의 결과가 다른 view에 적용되지 않는다.

## 6. AppKit macOS

진입점: [AppKitDesktopWindowHost](packages/platforms/maui/Doroti.Host.Maui/AppKitDesktopWindowHost.cs), [AppKit 정책](packages/platforms/maui/Doroti.Host.Maui/AppKitDesktopWindowPolicy.cs), [MAUI OS application](packages/platforms/maui/Doroti.Host.Maui/DorotiMauiPlatformApplications.cs), [macOS runner](samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacOS.csproj).

- [ ] **A1 — NSApplication과 application 수명.** C2/C4를 AppKit delegate·native 첫 창·추가 창·종료 승인에 연결한다. AppKit main thread와 framework dispatcher의 동기 wait 순환을 제거하고 initialization/drain과 NSApplication termination을 조정한다. 완료: 준비 전 앱 코드 실행 없음, 첫 창 중복 allocation 없음, primary close 후 survivor의 입력/frame 진행, 마지막 Regular/Explicit 정책·종료 중 Start 실패 cleanup이 실제 macOS에서 확인된다.

- [ ] **A2 — AppKit 창 종류·owner·좌표.** Regular부터 Dialog/Popup/Tooltip/Satellite를 요청별 구현하고 NSWindow/NSPanel·sheet 등 실제 지원 모델을 선택한다. modal/modeless·key/main window·Tooltip no-activate·outside click/Escape·owner-relative anchor·visibleFrame/scale 변환을 연결한다. Dock 정책·다중 owner/docking·배치 미지원은 별도 거절한다. 완료: requested/actual bounds·지원 종류·focus 복구·owner close/recreation·resource drain의 native 증거가 있다.

- [ ] **A3 — NSMenu typed capability.** 메뉴/menubar 생성·수정·shortcut/action을 typed 계약으로 연결한다. 앱 공통 menubar의 active owner와 WindowId/viewId/generation을 명시하고 메뉴 추적 중 종료·취소·늦은 action을 처리한다. 완료: 디자인 메뉴와 OS 메뉴의 역할이 구분되고 닫힌 창에 action이 전달되지 않으며 내부 codec 왕복이 없다.

- [ ] **A4 — surface·native view·입력 회귀.** AppKit Graphite/Ganesh의 선언된 경로, backing scale/resize, backdrop 정책, PlatformView/WebView·picker·drop·clipboard·navigation·Semantics·IME를 새 view 수명에 연결한다. 완료: resize/재연결/close에서 stale surface와 lease가 정리되며 renderer별 실제 실행과 한글 IME·VoiceOver·mixed display 수락을 별도로 기록한다.

- [ ] **A5 — 현재 구조의 AppKit 수락.** `macos_smoke.py`, `macos_package_smoke.py`와 독립 세 디자인 template 소비자를 현재 provider 경로로 실행한다. 앱 시작→정상 사용→restart/reconnect→약 1분 동작→정상 종료와 최종 drain을 확인한다. 완료: SDK/Xcode/OS/TFM/RID/renderer·package/native hash별 build/runtime receipt가 있고 서명·공증·clean-machine/Finder 설치 수락의 결과가 별도로 남는다.

## 7. Mac Catalyst

진입점: [Catalyst Host](packages/platforms/maui/Doroti.Host.Maui/MacCatalystDesktopWindowHost.cs), [Catalyst 정책](packages/platforms/maui/Doroti.Host.Maui/MacCatalystDesktopWindowPolicy.cs), [SceneDelegate](packages/platforms/maui/Doroti.Host.Maui/DorotiMacCatalystSceneDelegate.cs), [UIKit Graphite handler](packages/platforms/maui/Doroti.Host.Maui/DorotiUIKitGraphiteViewHandler.cs).

- [ ] **K1 — scene과 application 연결.** UIKit scene 생성/adopt·pending scene·disconnect/reconnect를 C2/C4와 연결하고 하나의 application tree 아래 scene별 branch를 배치한다. 현재 Explicit lifetime, multiple-scenes manifest, Mac idiom·Graphite 조건을 평가 입력으로 유지한다. 완료: 초기화 중 scene 제거·primary scene close·survivor 진행·handler 재연결에서 중복 branch/session·shared Dispose가 없다.

- [ ] **K2 — Catalyst 지원/거절 계약.** C3의 표시 정책을 연결하고 현재 public UIKit bridge가 제공하는 resize/focus/scene 종료 범위를 고정한다. native close veto·hidden first-frame preparation·global placement·topmost/Dock·AppKit backdrop처럼 제공하지 않는 기능은 거절한다. 보조 WindowKind는 실제 구현 가능한 요청만 지원한다. 완료: Native는 필요한 기능 누락 시 실패하고 Auto는 해당 Unsupported에만 Overlay를 선택하며 AppKit 수락을 Catalyst 수락으로 사용하지 않는다.

- [ ] **K3 — 메뉴·키보드·UIKit 입력.** Catalyst가 제공할 수 있는 menu/command 모델과 active scene owner를 typed 계약에 연결한다. 구현되지 않은 OS menu는 Unsupported로 남긴다. UIKit text input·hardware keyboard·context menu·magnifier·Semantics와 view별 focus를 확인한다. 완료: scene close/recreation의 action·IME callback 격리, modal/result/focus 복구와 Metal/Graphite surface retirement가 확인된다.

- [ ] **K4 — Catalyst build/runtime·독립 소비.** `apple_smoke.py --target maccatalyst --activation os`와 `apple_package_smoke.py`를 현재 profile로 이행한다. 세 디자인과 실제 두 scene의 shared state·close/recreation, Run/Reload/Restart/Stop을 검사한다. 완료: 설정한 OS/SDK/Xcode/TFM/RID별 receipt와 앱 manifest/entitlement/서명·clean-machine 수락이 분리되어 있다.

## 8. iOS

진입점: [MAUI surface](packages/platforms/maui/Doroti.Host.Maui/DorotiMauiSurface.cs), [UIKit text input](packages/platforms/maui/Doroti.Host.Maui/DorotiUIKitTextInput.cs), [iOS profile props](packages/platforms/build/Doroti.IosNativeAot.props), [iOS runner](samples/DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj), [기존 개발 helper](Doroti/eng/ios-development.py).

- [ ] **I1 — OS callback·view adopt·background 수명.** AppDelegate/native activation·UIApplication/scene와 C2/C4를 연결한다. surface attach/detach·handler 재생성·background/foreground·rotation·safe area를 application 수명과 분리한다. 추가 native 창은 scene 지원이 실제 있는 profile에서만 구현·광고한다. 완료: background 또는 임시 surface 상실이 앱 tree를 임의 종료하지 않고 등록/Seal 이전 attach·중복 root·늦은 view callback이 없다.

- [ ] **I2 — SDK·runtime·simulator/device 프로파일.** `apple_build_profiles.py`의 구 SDK/템플릿 복사본 검사를 provider 소유·실제 import/생성 자산 검사로 전환한다. net10 기본/ios27 Mono·interpreter, net11 CoreCLR 개발, net11 NativeAOT와 arm64/x64 simulator를 별도 평가한다. SDK/global.json·workload·Xcode·MAUI/runtime feed·registrar·trim roots·native assets를 profile 입력으로 고정한다. 완료: RID 없는 restore·Debug/Release·명시 override·잘못된 혼합을 검사하고 해당 profile의 matching toolchain에서 build한다.

- [ ] **I3 — typed 장치 검색·개발 전송.** simulator와 실제 authorized/paired device 목록·선택·RID/OS 조건을 provider 서비스로 연결한다. 기존 helper의 SDK Hot Reload agent·prepared acknowledgment·device/network transport를 명시적인 typed plan/process adapter로 옮긴다. 완료: device 선택 오류·준비 실패·장치 분리·late response·tool 재연결·owned Stop/Restart cleanup을 확인한 뒤에만 해당 profile에 dev/debug capability를 광고한다. Release/NativeAOT에는 Hot Reload를 광고하지 않는다.

- [ ] **I4 — UIKit frame·입력·디자인 회귀.** Metal/Graphite의 display loop·rotation/viewport epoch·GPU lease·PlatformView/WebView·picker/clipboard, 두 디자인의 Overlay/native 선택과 text selection/keyboard/Semantics를 검사한다. 완료: background/foreground·native view 재연결 중 frame 진행과 stale generation 거절이 확인되고 실제 한글 IME·VoiceOver·터치/키보드 결과가 synthetic 결과와 분리된다.

- [ ] **I5 — simulator·device·AOT·배포 수락.** `apple_smoke.py --target ios --activation native-callback`, `ios_frame_loop_lifecycle_device.py`, `ios_hot_reload_smoke.py` 및 package 소비 검사를 이행한다. Mono 개발 실행과 Release/AOT 실행을 각각 확인하고 shader/icon/locale/typed plugin 등록의 trim 보존을 검사한다. 완료: simulator PASS와 실제 ios-arm64 장치 PASS가 별도이며 signing/provisioning·설치/재설치·restart/reconnect·약 1분 사용·정상 종료의 증거가 있다.

## 9. Android

진입점: [Android Vulkan handler](packages/platforms/maui/Doroti.Host.Maui/DorotiAndroidVulkanViewHandler.cs), [Android 장치 검색](packages/platforms/maui/tooling/AndroidDevices.cs), [Android runner](samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj), [기존 개발 helper](Doroti/eng/android-development.py), [session 소유권](Doroti/eng/android_session_ownership.py), [port 소유권](Doroti/eng/android_port_ownership.py).

- [ ] **D1 — Application/Activity·view lifecycle.** MainApplication/MainActivity의 OS shell과 provider bootstrap을 분리하고 C2/C4를 연결한다. activity recreate·configuration change·pause/resume·surface destroy/create를 application/branch 수명과 구분한다. 완료: process 준비/Configure/Seal/attach trace와 초기화 중 취소·중복 activity callback·Back/정상 종료·process 재시작의 단일 cleanup이 확인된다. 추가 native WindowKind는 미지원 profile에서 Overlay로 평가한다.

- [ ] **D2 — Vulkan surface·GPU retirement·입력.** 현재 generation/retirement 보존을 신규 view/frame 계약에 연결한다. resize/rotation·background·device loss/slow consumer·handler 교체, Android PlatformView/WebView/texture·IME/Semantics를 검사한다. 완료: 살아 있는 GPU consumer 이전에 native window/buffer를 해제하지 않고 drain timeout을 device loss나 성공으로 오인하지 않으며 재개 후 fresh surface의 frame이 진행된다.

- [ ] **D3 — ADB 개발 session·Stop/Restart.** 이미 구현된 authorized/ABI 장치 검색·serial 검증·복수 장치 선택을 재사용한다. 기존 helper의 SDK agent·prepared handshake·ADB reverse·run-as 파일·session nonce·PID/start identity·port/device lease를 provider plan/execution adapter로 옮긴다. 완료: 장치 분리·취소·crash·tool reconnect·동일 장치 경쟁에서 소유 session만 정리하고 앱 재연결/Stop/Restart·Hot Reload 결과를 확인한 뒤 dev를 광고한다.

- [ ] **D4 — runtime profile·doctor·native build.** android-arm64/x64, Debug 개발 runtime과 Mono/CoreCLR의 실제 지원 조합, Release trim/AOT 조합을 평가된 설정으로 확정한다. JDK/Android SDK/API/NDK·Gradle wrapper/native binding 요구를 실제 build metadata에서 읽고 obj/bin·device selection을 분리한다. 완료: 개발/Release 혼합과 잘못된 ABI를 거절하고 nupkg 소비가 저장소 native 재빌드를 요구하지 않으며 prerequisite doctor와 장치 실행 결과가 구분된다.

- [ ] **D5 — 장치·NuGet·제품 수락.** `android_development_profile.py`, `android_development_bridge.py`, `android_hot_reload_smoke.py`, `native_frame_android_lifecycle.py`를 현재 구조로 이어가고 aggregate Android gate를 추가한다. 두 실제 샘플과 세 디자인 template의 restore/build/install/start/input/recreate/close를 확인한다. 완료: emulator/x64와 arm64 물리 장치 결과가 분리되고 shader/icon/locale·trim/AOT·서명 APK 설치·약 1분 정상 사용·restart/reconnect·정상 종료 증거가 있다.

## 10. 도구·IDE·다른 provider의 남은 작업

진입점: [Tooling Contracts](Doroti/src/Doroti.Tooling.Contracts/), [Extension SDK](Doroti/src/Doroti.Tooling.Extension.Sdk/), [managed CLI](Doroti/tools/Doroti.Tooling/), [VS Code 확장](Doroti/tools/vscode-doroti/), [Qt provider](packages/platforms/qt/), [Web provider](packages/platforms/web/).

- [ ] **T1 — builtin 서비스와 provider doctor.** configuration/device/diagnostics/templates/operations를 builtin provider별 실제 응답으로 완성하고 template registry·native-maintenance/rebuild를 지원 profile에서 선언한다. OS/TFM/RID/SDK/CWD/global.json·workload/native tool·timeout 조건은 provider 평가값을 사용한다. 완료: 미설치/미지원/활성화 실패/호출 실패가 구분되고 새로운 alias/profile/service 추가가 CLI의 고정 목록 수정을 요구하지 않는다. doctor PASS는 선택 prerequisite scope에 한정한다.

- [ ] **T2 — .NET/process 확장과 execution 수명.** dotnet-inproc ALC/DependencyResolver·공유 Contracts identity·동시 invocation·협력 종료·남은 참조를 검사한다. 실제 child process로 handshake timeout·초기화 중 종료·과대/잘못된 frame·중복 namespace·late response·crash/reconnect/shutdown을 확인한다. 완료: typed/process 결과·오류·취소가 같고 도구 연결 재생성 중 앱이 유지되며 그 뒤 owned Stop/Restart가 된다. in-process fatal crash 격리·강제 unload는 보장으로 표기하지 않는다.

- [ ] **T3 — CLI·IDE 실제 흐름.** 얇은 doroti.ps1, describe schema v2·capability·typed argv plan과 actual VS Code target 선택/Run/Hot Reload/Restart/Stop을 연결한다. mobile 준비·device 선택·restart-required·debug/OS 제한과 Web browser session 수명을 표현한다. 완료: builtin과 신규 외부 alias를 실제 editor에서 실행하고 tool 재연결·cancel·Stop 뒤 잔여 owned process/device/port가 없는지 확인한다. 현재 argv/metadata fixture PASS와 GUI 수락을 구분한다.

- [ ] **T4 — 외부 provider와 typed/wire 경계.** test-headless의 독립 Contracts/Extension.Sdk/provider nupkg 소비자로 configuration/device/template/diagnostics/operations·startup/Stop을 실행한다. 내부 codec 0, Future/Task owner 순서와 JSON required/null/enum/64-bit ID/version·reflection fallback 거절, C ABI layout/encoding/buffer/callback lifetime·worker wire를 검사한다. 완료: core/CLI/IDE 변경 없이 설치/실행되고 native/JS/process 실제 경계만 직렬화하며 runtime/AOT/ALC 수락은 해당 환경별로 남는다.

- [ ] **R1 — Windows 잔여 native 수락.** 기존 five-kind/menu/design PASS 위에 modal 입력 차단·focus 복구·Escape/outside click·content-size·work area/mixed DPI·owner close/recreation·GPU slow consumer를 검사한다. Windows MAUI에도 C2/C4와 공개 확장/독립 package 소비를 마무리한다. 완료: 자동화·native 존재/Visible·물리 입력/접근성·scanout 항목이 나뉘며 U4와 최종 drain을 만족한다.

- [ ] **R2 — Qt 창·메뉴·staged lifecycle.** 기존 primary close/GUI owner 수정과 native 소스 단일 소유를 유지하고 C2~C4, Dialog/Popup/Tooltip/Satellite·owner/anchor/modality·typed OS menu를 실제 지원 범위대로 연결한다. 완료: 단일 GUI loop·freeze/lease·survivor·close 결과와 Wayland/XWayland/display 없음·software/hardware GPU 결과가 분리된다. 기존 strict subset PASS를 전체 창 종류로 확대하지 않는다.

- [ ] **R3 — Web profile·mobile Overlay·종료.** main/worker/runtime profile의 process/view 등록·cross-origin isolation·첫 frame/focus·OffscreenCanvas·browser texture lease와 cancel/late bitmap close를 확인한다. 추가 native 창 미지원 정책에서 두 디자인의 Overlay 입력/결과/locale·view 제거를 검증한다. 완료: Offline assets/worker 실패·tool reconnect·browser-owned Stop와 모든 선택 profile의 runtime receipt가 있고 Web/mobile 결과를 desktop native Windowing 수락으로 쓰지 않는다.

## 11. 패키지·템플릿·릴리스·문서 마무리

진입점: [release 후보 도구](Doroti/eng/release_candidate_v2.py), [템플릿](Doroti/templates/Doroti.Templates/content/doroti-app/), [Runner SDK](Doroti/src/Doroti.Runner.Sdk/), [App SDK](Doroti/src/Doroti.App.Sdk/), [migration 문서](Doroti/docs/migrations/design-platform/).

- [ ] **P1 — SDK·세 디자인·모든 선택 profile.** 완료된 widgets/material/cupertino 선택을 유지하고 App/Runner SDK의 암묵 디자인 참조·혼합 identity 거절·provider 생성/asset pack·native registry·design-time을 재확인한다. Apple/Android runner와 기존 두 샘플의 locale/font/shader·앱 고유 plugin/native module을 연결한다. 완료: repo와 외부 nupkg에서 앱/runner/provider 책임이 같고 잘못된 core/design/provider 혼합이 명확한 진단으로 거절된다.

- [ ] **P2 — 독립 버전·호환 범위 소비자.** 고정 core+변경 design, 고정 design+변경 core, 고정 core/design+변경 provider, 고정 provider+호환 core를 독립 캐시 소비자로 restore/build/run한다. 최소 검증 core·현재 core·범위 밖·protocol/native ABI 불일치·중복 등록을 포함한다. 완료: 선언 range·resolved identity·nuspec·AssemblyRef가 일치하고 실제 검사한 조합만 지원표에 남는다.

- [ ] **P3 — profile별 release 후보·receipt.** 현재 independent version/root 탐색을 유지하고 임의 workspace runner 경로 매핑, provider의 여러 profile 선택과 Apple/mobile build/native assets·runtime qualifier를 확장한다. desktop two-window receipt를 mobile 실행에 강제하지 않는다. 완료: 각 후보의 package별 버전/core range/protocol/ABI/native provenance·source/resource hash·template/alias/profile·실행 범위가 남고 prebuilt 소비자에 원본 소스 참조가 없다. 공개 배포는 실제 수행한 경우만 기록한다.

- [ ] **P4 — 문서·구 구조 제거·원 계획 정합성.** README 한/영, 패키지·SDK·도구·샘플·template·migration·support 문서를 실제 구조로 맞춘다. 구 경로/schema/namespace·shim·static flag·null 기본 owner·legacy bootstrap/manifest·중복 구현을 활성 제품에서 제거하고 역사 기록/거절 fixture의 허용 잔여를 명시한다. 완료: 링크·사용 명령·지원표가 실제 metadata와 일치하고 work2와 execution/checkpoint에 부분/완료 증거를 일관되게 갱신한다.

## 12. 프로파일과 검증 운영 계획

### 현재 선언과 실행 환경

| 플랫폼 | 현재 기본/명시 입력 | 실행 환경과 별도 수락 |
|---|---|---|
| AppKit | `net10.0-macos` / 명시 `net10.0-macos27.0`, `osx-arm64` | matching macOS SDK/workload/Xcode. smoke 기본 TFM은 macos27이므로 일반 기본과 구분 |
| Catalyst | `net10.0-maccatalyst` / 명시 `net10.0-maccatalyst27.0`, `maccatalyst-arm64` | macOS, 선택 SDK/Xcode와 OS minimum; Desktop의 multiple-scenes/Mac idiom/Graphite 조건 |
| iOS | 기본 `net10.0-ios`, 명시 `net10.0-ios27.0` 또는 `net11.0-ios`; `ios-arm64` / `iossimulator-arm64` / `iossimulator-x64` | Mono/interpreter 개발·CoreCLR 개발·NativeAOT를 각각 선택. simulator build/runtime와 장치 signing/runtime 별도 |
| Android | `net10.0-android`, `android-arm64` / `android-x64` | 실제 SDK/workload/JDK/native 요구, authorized matching device. 개발 runtime과 Release trim/AOT 별도 |

위 표는 현재 소스의 선언이다. 전 조합의 build 성공이나 설치 가능한 toolchain을 이번에 확인한 표가 아니다. SDK/Xcode/OS minimum·runtime 버전은 구현 시작 시 actual MSBuild metadata·global.json·provider profile로 다시 확정한다.

- [ ] **V1 — 공통 회귀 집계.** 기존 Build/Packages/Targets/Developer/Release에 연결된 bootstrap/frame/tool/typed/design 계약을 유지하고 변경한 G0/G1·실패 fixture를 해당 suite에 반영한다. Source 문서 검사 범위에 work3를 추가한다. 실제 평가 그래프/소비자를 검사하며 문자열 존재 검사만으로 완료하지 않는다.

- [ ] **V2 — host/profile/device별 aggregate.** Apple profile 구 경로를 먼저 고친 뒤 MacOSSmoke/CatalystSmoke/IOSSmoke와 Android aggregate를 구성한다. 현재 validate.py에는 AndroidSmoke가 없고 Targets/Release의 native runner build가 WindowsAppSdk/Web 중심이므로 mobile/Apple 검사를 자동 포함한 것으로 간주하지 않는다. device/renderer·simulator/physical·host/display 누락 사유를 receipt에 남긴다.

- [ ] **V3 — trim/AOT·물리·배포 수락.** static typed 등록·shader/icon/localization·native callback·plugin entrypoint의 trim/AOT 보존을 matching host에서 확인한다. 실제 한글 IME·키보드/터치·screen reader·mixed DPI·GPU/scanout·서명/설치/clean-machine 수락은 각각 측정·관찰 결과로 남긴다. 빌드나 synthetic 이벤트는 이 항목의 PASS를 대신하지 않는다.

- [ ] **V4 — 최종 잔여 감사와 인계.** 아래 대응표와 work2 최종 체크리스트의 각 항목을 구현/자동화/runtime/physical 상태에 연결한다. 미구현과 환경 미확인을 구분하고 재개 가능한 명령·선택 profile·증거 위치·다음 blocker를 남긴다. 모든 필수 미완료가 해소되기 전에는 wholePlanComplete를 true로 변경하지 않는다.

모든 테스트는 [.github/copilot-instructions.md](.github/copilot-instructions.md)에 따라 외부 1,200초 timeout을 사용한다. 반복은 보통 30회 이내로 하고 의미 있는 실패/변경 없이 전체 suite를 반복하지 않는다. 아래는 aggregate 재개 명령이다. 이번에 실제 수행한 명령·profile·결과는 §14의 receipt를 따른다.

저장소 루트, 현재 작업에 맞는 Python/SDK 환경에서:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py Build
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py Packages
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py WindowsSmoke
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py Developer
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py Release
```

matching Apple 호스트에서 I2/V2 이행과 toolchain 선택 뒤:

```sh
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/tests/apple_build_profiles.py
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/eng/validate.py MacOSSmoke
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/eng/validate.py CatalystSmoke
python3 Doroti/eng/run-with-timeout.py --timeout 1200 python3 Doroti/eng/validate.py IOSSmoke
```

Android/iOS의 실제 장치 검사는 typed device 서비스로 선택한 ID, 정확한 app/package/entrypoint·profile·output을 명시한다. 기존 장치 스크립트의 기본 package/activity를 새 빌드의 값으로 추정하지 않는다. Android aggregate 명령은 V2에서 구현한 후 사용 문서에 추가한다.

원시 로그는 `temp/testing/work3/<platform>/<run>/` 또는 테스트가 지정한 disposable run에 둔다. `Doroti/artifacts`도 삭제 가능하다. 보존할 요약·hash·trace·receipt는 `Doroti/docs/migrations/design-platform/`에 저장하고 원시 파일 삭제 뒤 파일이 남아 있다고 보고하지 않는다.

| 결과 표기 | 의미 |
|---|---|
| PASS | 명시한 profile·환경·검사 범위가 통과 |
| PARTIAL | 일부 범위만 수행하거나 doctor 등 bounded probe가 timeout으로 미완료 |
| SKIPPED | 해당 host/device/display/toolchain이 없어 실행하지 못함. 이유·재개 조건 기록 |
| notVerified / notMeasured | 필요한 runtime/물리/서명·설치 수락 또는 측정을 아직 수행하지 않음 |
| Unsupported | provider가 해당 요청을 제공하지 않음. 올바른 거절 검사는 PASS일 수 있으나 기능 지원은 아님 |

각 결과에는 구현 상태와 검증 상태를 따로 둔다. test timeout은 성공으로 처리하지 않고, doctor timeout의 PARTIAL 정책과 일반 테스트 실패/미완료를 구분한다.

## 13. work2 잔여 항목 대응표

이 표는 원 계획의 미완료 항목을 후속 실행 묶음으로 옮긴 추적표다. 원 항목의 세부 계약은 work2를 따른다. 여러 묶음에 걸친 항목은 일부 결과만으로 완료 처리하지 않는다.

| work2 원 항목 | work3 후속 항목 | 남은 완료 판단 |
|---|---|---|
| DS0-1~4 | C1, C5, P2, V4 | semantic/evaluated 소유권·의존·버전/범위 감사 |
| DS1-1~5 | C5, C6, P1~P3 | 공통 설정·독립 버전·metadata·TFM/RID·prebuilt 소비 |
| DS5-1~5 | U1~U3, A4, K3, I4, D2, V3 | Raw/wrapper·입력·transition·실제 접근성 |
| DS6-1 | C2, C6, P1 | SDK 암묵 디자인 없음·provider bootstrap 책임 |
| DS7-4 | P2, P3 | 디자인/core 독립 갱신·최소/현재/범위 밖 소비 |
| DS8-2~4 | A5, K4, I5, D5, R1~R3, V2~V3 | 샘플·플랫폼·자산/trim/AOT·runtime/physical |
| DS9-1~5 | P3, P4, V4 | release/profile·문서·최종 제거·증거 |
| PW0-1~6 | C1~C4, P2, V4 | 실제 capability·schema/ABI·ADR·upstream·G0/G1 |
| PW1-1~7 | C1, C4~C6, T4, P1 | 등록·typed service·native 소유·public 계약·lease |
| PW2-1~6 | C2, C4, C6, A1, K1, I1, D1, R2, R3 | prepare/adopt/Seal/attach·실패·OS bootstrap·종료 |
| PW3-1~8 | C5, C6, T1~T4, P1~P3 | workspace/Target·생성 자산·typed operation·외부 provider·ALC/prebuilt |
| PW4-1~7 | C4, U4, A1, K1, I1, D1, R2 | shared tree·view-local 입력·G1/frame·factory 결합 제거 |
| PW5-1~5 | C3, U4, R1, V3 | Windows 실제 kind·owner·focus/modal·좌표·drain/physical |
| PW6-1~4 | U1~U3, A2~A3, K2~K3, R1~R2 | 디자인 Native/Overlay·route·typed OS menu |
| PW7-1~6 | T1~T4, I3, D3~D4, P3 | CLI/doctor/IDE·provider services·개발 execution·후보 |
| PW8-1~7 | A1~A5, K1~K4, I1~I5, D1~D5, R1~R3, T2, T4, P2~P4, V1~V4 | 전 provider 이식·집계·wire/ABI·process·AOT·지원표 |
| work2 §8 최종 수락 체크리스트 전체 | C1~C6, U1~U4, 플랫폼별 항목, T1~T4, R1~R3, P1~P4, V1~V4 | 구조/패키지/수명/도구/runtime/physical별 증거를 항목마다 대조 |

이번 실행은 C1 평가 그래프와 C2/C3/C4 bootstrap·창 요청·수명 연결에서 시작했다. 기존 완료 항목은 유지하고 아래에 실제 변경 및 미충족 수락 기준을 기록한다.

## 14. 2026-10-05 실행 결과

사용자가 요청한 공통·AppKit·Mac Catalyst·iOS 범위를 작업했다. **전체 PARTIAL**이며 필수 기준이 남은 체크박스는 유지한다. 기계 판독 결과는 [이번 검증 receipt](Doroti/docs/migrations/design-platform/work3-verification-2026-10-05.json), 전체 계획의 상태는 [checkpoint](Doroti/docs/migrations/design-platform/latest-checkpoint.json)와 [execution](Doroti/docs/migrations/design-platform/execution.json)을 따른다. Android·Qt·Web·Windows의 새 native 수락은 이번 실행 범위가 아니다.

| 항목 | 이번 구현·확인 | 남은 필수 기준 |
|---|---|---|
| C1 | 실제 MSBuild로 core 6개·두 디자인·tool·Apple Host 4개 profile의 참조/Compile/resource/friend 입력을 평가. core 역참조·Cupertino→Material·중복 입력 거절 | 전체 provider closure와 Roslyn/public API·reflection 의미 감사 |
| C2 | UIKit/Android OS startup에서 PrepareProcess를 사용자 Configure보다 먼저 실행. MAUI 명시 application dispatcher와 공유 application session 연결 | 모든 provider를 Hosting staged coordinator로 통합; OS Stop/Restart·초기화 중 종료 등의 실제 receipt |
| C3 | factory별 WindowRequest 매핑. AppKit Manual/Dock 비설정, Catalyst PlatformDefault. owner 충돌·누락·미지원 modality/anchor/activation을 native allocation 전에 거절 | 모든 provider 및 Windows 실제 동작의 최종 수락 |
| C4 | branch unmount와 typed callback quiesce 후 per-view invocation/GPU drain. iOS에서도 실제 retirement await. 공유 Stop/Dispose join·실패 재시도·survivor 보존. 공유 SKTypeface wrapper의 lease를 추가해 한 View 종료 후 다른 View의 한글 편집 crash 수정 | 모든 staged failure/device-loss/slow-consumer 경합의 native 확대 수락 |
| C5 | SDK가 app project/name을 먼저 확정한 뒤 provider profile import. iOS arm64/x64 simulator·device·Debug/Release·override·잘못된 개발 혼합 평가 | .NET 11 matching toolchain build, 모든 default/RID-less/design-time/version/nuspec 조합 |
| C6 | iOS profile을 provider 단일 소유로 전환하고 template 복사본·중복 import 제거 | MAUI sample friend 접근을 public/protected 진단·등록으로 전환; native bridge/binding 공급 책임 감사 |
| U1 | preview/action popup route 진입점, caller theme/locale/directionality 캡처와 child Navigator 결과 연결. Cupertino long-press·Overlay 선택/닫기 구현. 공통 ModalScope의 빈 animation 구독과 0이 되던 duration 나눗셈 수정; route별 tween 격리 | native Cupertino backdrop sampling·restoration/focus/Semantics의 완전한 수락. 필터가 필요한 요청은 Native Unsupported, Auto Overlay |
| U2/U3 | 실제 Material/Raw menu·radio·Tooltip·ExpansionTile, Cupertino context menu·dialog·selection과 G0 EditableText 회귀 통과 | native Tooltip hover/delay/late-install 확장·물리 IME/접근성 수락 |
| U4 | CPU shared tree·view별 focus/selection/IME와 late callback·primary detach/survivor 회귀 | Windows 실제 확장 G1·slow consumer/modal/reentrant close·완전한 API 감사 |
| A1/A2 | AppKit main-thread mutation·공유 session·per-view drain. Regular 및 owned Dialog/Popup/Tooltip/Satellite, sheet, no-activate, owner-relative anchor/visibleFrame, popup Escape/로컬 outside click, owner cascade·focus 복구 | 여러 display/scale·앱 밖 클릭·재생성/초기화 종료 경쟁·모든 좌표/focus 수락 |
| A3 | typed NSMenu popup/menubar, enabled/checked/submenu·문자 shortcut. active WindowId/viewId/generation action, 취소/close·stale action 거절·기본 앱 메뉴 복구 | popup tracking 중 종료/shortcut·실제 디자인 menu 흐름 확대. platform role/logical-key shortcut은 Unsupported |
| A4/A5 | Graphite/Ganesh 실제 resize·Metal retirement/reconnect/final drain, native 서비스·editor/WebView·Semantics·navigation, 두 lifetime의 survivor와 60초 실행/정상 종료. 독립 widgets NuGet 소비자의 실제 두 창·resize·close와 local extracted-app 복사/동일 후보 교체/제거·한글 데이터 보존 PASS | Material/Cupertino 및 모든 profile 독립 소비자·Run/Reload/Restart 전체, 물리 한글 IME/VoiceOver/mixed display·서명/공증/Finder/clean OS·서로 다른 후보 upgrade |
| K1/K2 | shared session·명시 dispatcher·destroyed primary dispatcher 제거, typed/GPU drain. activating/unowned Regular 추가 scene 지원; aux/owner/modal/no-activate/hidden 준비 명시 거절 | scene 초기화 취소·재생성/복원·모든 lifecycle race |
| K3/K4 | 실제 UIKit 서비스·synthetic 입력·editor/WebView/Semantics·navigation/restoration·두 scene/primary close/survivor 검증 | OS menu Unsupported. 물리 키보드/IME·scene action 재생성·독립 세 디자인/개발/배포 수락 |
| I1/I4 | scene surface가 application session을 공유하고 branch/view identity를 따로 소유. background/handler 상실은 app tree 유지. Destroying에서 비동기 detach/drain. actual completion frame acknowledgment 연결. 실제 Stop join·view dispose·Metal pending 0 확인 | background/reconnect/Restart의 전체 trace·물리 터치/IME/VoiceOver |
| I2 | provider profile 실제 import/단일 소유·프로파일 평가와 net10.0-ios27.0 simulator build | .NET 11 CoreCLR/NativeAOT matching SDK/runtime·x64 실제 build/runtime·장치 Release/AOT |
| I3 | typed simctl/CoreDevice discovery, available iOS·connected paired physical 필터, bounded JSON/취소, RID와 명시 ID 선택 및 SDK _DeviceName 연결 | SDK agent/prepared acknowledgment·network/device 개발 transport·owned Stop/Restart. typed dev는 계속 Unsupported |
| I5/P1~P4/V1~V4 | Apple aggregate에 profile/소유권/실제 iOS Stop 검사 연결, iOS multi-scene 미지원 manifest를 SKIPPED로 명시. 후보의 provider RID restore 분리·Apple copy-mode publish·평가된 assets 경로·AppKit desktop fixture 연결과 v2 native receipt/pkg 추출 검사 수정. 문서/지원표/receipt 갱신 | package/profile별 전체 후보·호환 범위·trim/AOT·물리/서명/설치와 원 계획의 모든 잔여 |

실행 환경은 macOS **26.6.2 (25G83)** / Apple M1 arm64, .NET SDK **10.0.401**, Xcode **27.0 (27A266a)**다. native build는 AppKit `net10.0-macos27.0/osx-arm64`, Catalyst `net10.0-maccatalyst27.0/maccatalyst-arm64`, iOS `net10.0-ios27.0/iossimulator-arm64`로 구분한다. iOS simulator는 iPhone 17/iOS 27.0이며 당시 connected-state 필터가 반환한 실제 paired 장치는 0개였다. 아래 실기기 후속 검사에서 idle CoreDevice tunnel도 연결 확인 후 검색하도록 수정했다. 설치된 .NET 11 rc1 SDK는 선언된 rc2 개발 runtime의 matching build 증거로 사용하지 않는다.

독립 AppKit 후보는 core `0.4.0-alpha.1` 고정, provider `0.4.0-alpha.2`, 디자인 `widgets`, `net10.0-macos/osx-arm64` 선언과 실제 SDK platform version `27.0`으로 실행했다. 26개 prebuilt nupkg와 별도 cache의 template 소비자이며 repository project 참조가 없음을 평가된 assets에서 확인했다. Apple SDK가 요구하는 trim pipeline은 `copy`/`LinkMode=None`으로 사용했다. 이것은 NativeAOT/실제 trimming 수락이 아니다. native 실행은 해시 검증된 `.pkg`를 임시 경로에 추출한 앱에서 실행했고 앱 signing의 adhoc 검증만 했다. OS Installer·공증·clean OS 수락은 아니다. local 교체는 같은 후보끼리 수행했으므로 서로 다른 버전의 upgrade로 보고하지 않는다.

원시 로그·native summary는 `temp/testing/work3/`에 두고 보존할 핵심 결과·명령·hash는 별도 receipt에 남긴다. PASS는 receipt에 명시된 실행 범위만 의미한다. 기존 Windows/Qt/Web/NuGet PASS를 새 Apple 소스의 전체 수락으로 확대하지 않는다. `wholePlanComplete=false`를 유지한다.

## 15. 2026-10-05 iOS 실기기 후속 검사

iPhone 12 / iOS **26.6.1 (23G83)** / USB / Developer Mode에서 Testbed를 개발 서명하여 설치·실행했다. profile은 `net10.0-ios27.0` / `ios-arm64` / **Debug Mono**이며 엔진·의존성은 Mono AOT, 앱·iOS 진입 어셈블리는 interpreter인 기존 기본 설정을 사용한다. 최종 서명 빌드는 경고·오류 0이다. 기존 앱을 제거하거나 데이터를 초기화하지 않았다. 원시 결과는 `temp/testing/work3/ios-device/`에 두고 보존 요약은 [실기기 receipt](Doroti/docs/migrations/design-platform/work3-ios-device-verification-2026-10-05.json)에 남긴다.

- **장치 검색 수정:** CoreDevice가 USB로 접근 가능한 paired 기기의 idle tunnel을 `disconnected`로 반환해 누락하던 문제를 수정했다. connectable 후보에 bounded `device info details` 확인을 하고, 같은 UDID의 physical/paired/connected 결과만 제공한다. unreachable·다른 UDID·malformed JSON·취소 회귀와 실제 장치 1개 검색이 통과했다.
- **probe 경로 수정:** iOS의 모든 probe 출력 경로를 Documents로 정규화하고 부모 폴더를 생성해 실기기 container에서 결과를 회수한다. 실기기 smoke는 빈 environment 인자를 생략하고, 정적 장면의 복귀를 GPU 완료 프레임 진행으로 검사한다.
- **회전 계측 및 정책:** 매 pulse에서 전체 GPU/Semantics trace를 복사하던 계측을 cached geometry로 바꾸고 pulse callback 뒤에 샘플링한다. 사용자의 최종 지시에 따라 강제 FPS 제한을 제거하고 UIKit 기본 cadence를 사용한다. 중간 raster 크기·최종 pixels/safe area·display link 종료·frame failure 0을 수락 기준으로 삼는다. 동기화 오차는 측정값으로 남기며 기존 평균 5%/최대 10% 엄격한 기준의 통과를 주장하지 않는다. 별도 성능 수락은 smoke의 `--assert-rotation-sync`로 재현할 수 있다.

실제 UIKit 파일 128MiB offset read·취소·owner disposal/drop capability, native editor/WebView 생성·재생성 4회, WKWebView HTML/JS/앱 자산/message/stale generation/close와 Semantics native action, 양방향 scene rotation, shared Stop join·view dispose·Metal pending 0을 확인했다. 사용자는 실기기를 직접 사용했을 때 **“특별한 이상 없음”**이라고 답했다. 이 피드백은 물리 한글 IME·VoiceOver의 정식 수락을 대신하지 않는다.

Settings로 background 전환한 뒤 같은 PID로 복귀하는 3회 검사에서 GPU 완료 프레임이 **4→5→6→7**, frame failure는 0이었다. 최종 일반 Material 샘플의 실제 Metal 완료 프레임과 **1170×2532** 캡처를 확인하고 앱을 foreground로 남겼다. 회전은 방향별로 서로 다른 raster 폭 11개를 확인했다. 평균/최대 동기화 오차는 가로 **23.10/48.54pt**, 세로 **26.57/60.06pt**이며 별도의 엄격한 오차 예산을 충족하지 않는다. 이번 PASS는 사용자와 정한 기능 수락 범위에 한정한다.

장치 Release/NativeAOT/CoreCLR, package-only 장치 소비자, 배포 provisioning/signing·clean installation, 실제 scanout FPS 및 엄격한 회전 성능 수락은 별도다. 이 후속 결과도 전체 계획의 `wholePlanComplete=false`를 유지한다.

## 16. 회전 버벅임 재현 및 Release Mono AOT 설치

사용자가 실제 Components 화면에서 심한 회전 버벅임을 보고했다. 앞선 자동 rotation의 `reload` 화면은 훨씬 단순하여 실제 화면의 성능을 대신하지 못했다. 진단 writer·상세 profiling을 끈 Components 화면에서도 Debug Mono의 viewport 갱신 간격이 평균 **107.50/93.97ms**, 최대 **142.88/147.39ms**(가로/세로)였다. 앞선 기능 PASS를 이 화면의 성능 PASS로 해석하지 않는다.

사용자의 AOT 설치 요청에 따라 NativeAOT를 먼저 시도했다. feature/shutdown probe의 reflection JSON을 typed source-generated context로 바꾸어 IL2026/IL3050 오류를 해결했다. iOS/macOS feature probe 복사본도 일치시켰다. .NET 10/Xcode 27 NativeAOT는 SDK가 생성한 explicit-interface 보존 정보의 IL2037 오류로 실패했고, .NET 11 NativeAOT는 설치된 workload가 Xcode 26.6을 요구해 현재 Xcode 27.0을 거절했다. SDK 검사를 우회하거나 오류를 숨기지 않았다.

현재 도구 체인에서 가능한 **Release Mono AOT/LLVM**를 `net10.0-ios27.0` / `ios-arm64`, `UseInterpreter=false`, `MtouchInterpreter=-all`, `Optimize=true`로 publish했다. 경고·오류 0이며 앱·iOS 진입·Host의 `.llvm.o` 생성과 서명 검사 후 기존 앱 데이터 보존 상태로 설치했다. **NativeAOT가 아닌 Mono AOT**다. 세부 profile·hash·실패 기록은 [Release AOT receipt](Doroti/docs/migrations/design-platform/work3-ios-release-aot-verification-2026-10-05.json)를 따른다.

동일한 Components 화면·기기·cold process·진단 writer 없음 조건에서 Release Mono AOT의 갱신 간격은 평균 **39.96/37.89ms**, 최대 **50.55/52.72ms**로 줄었다. 방향별 raster 폭은 **10/11개**였다. 이는 viewport callback 계측이며 scanout FPS가 아니다. 평균/최대 phase 오차는 **20.73/68.26pt**, **22.00/51.85pt**로 엄격한 동기화 오차 기준은 여전히 충족하지 않는다. 사용자가 허용한 짧은 회전의 무제한 FPS 정책을 유지한다.

Release 앱에서 native 서비스·editor/WebView·Semantics·양방향 실제 scene rotation·joined Stop/Metal drain·동일 PID background/foreground 3회 기능 검사가 통과했다. smoke에 `--sample material`과 상세 profiling opt-in을 추가했고 최종 일반 실행에는 probe/evidence/profiling 환경변수를 남기지 않는다. 현재 아이폰에는 이 일반 **Release Mono AOT** 앱을 foreground로 남겼으며 물리 체감 확인을 요청했다. NativeAOT·엄격한 회전 성능·배포/clean installation 및 전체 계획은 미완료로 유지한다.

설치 후 사용자는 실제 회전이 **“약간 개선됨”**이라고 답했다. 상세 계측의 startup 이후 구간에서 callback은 평균 **14.914ms**/최대 **33.212ms**, raster는 평균 **15.313ms**/최대 **35.452ms**였고 drawable 획득 평균 **0.677ms**, GPU scheduling 대기 평균 **0.491ms**였다. 이 instrumented 구간은 정확한 회전-only/scanout 계측이 아니며 남은 렌더링·UI callback 지연을 추적하는 자료다. AOT 설치와 기능 검사 통과를 버벅임 완전 해결로 보고하지 않는다. 상세 계측 후에도 probe/evidence/profiling 없이 일반 Release 앱을 실행해 두었다.
