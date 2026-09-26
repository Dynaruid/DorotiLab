# 데스크톱 창 API 재구성·플랫폼 구현 요약

- 작업 기록: 2026-09-25~26. 보관일: 2026-09-26.
- 대상: 루트 `work.md`의 창 API 설계, W0~W6 계획 및 Windows/AppKit/Mac Catalyst/Linux Qt 실행 기록. 최초 검토 기준은 `7f74005f`다.
- 최종 상태: **전체 PARTIAL**. 공통 Desktop 계약과 네 플랫폼의 기본 창 adapter를 구현했지만, 커스텀 상단바·WindowsAppSDK adapter·환경별 시각/입력 검증은 미완료다. 실제 다중 네이티브 창은 후속 W6 범위다.
- 사용자 요청으로 핵심 결정·실행 결과·잔여 작업을 이 문서에 요약하고 루트 원본을 삭제했다. 아래 PASS/FAILED는 기존 기록이며, 이번 보관에서 제품 빌드·GUI·기기 검증을 재실행하지 않았다.

구현된 API와 지원 조합은 [Desktop API 문서](../../Doroti/docs/desktop-windows.md), 실행 조건·재현 명령·플랫폼별 상세 결과는 [Desktop 검증 기록](../../Doroti/validation/desktop-window/README.md)을 기준으로 한다. 원문의 제안 예제 전체가 구현된 API를 뜻하지 않는다.

## 목표와 확정한 구조

사용자 Sudoku Flutter 앱의 `window_manager` 사용 흐름인 **옵션 선언 → 준비 → 표시·포커스 → 실행 중 제어**를 Doroti에 도입했다. 대표 설정은 client 450×800, 최소 350×500, 일반 제목 표시줄이다. 외부 Sudoku 프로젝트는 수정하지 않았고 Flutter 플러그인을 새 의존성으로 추가하지 않았다.

| 구성 | 책임과 경계 |
| --- | --- |
| `Doroti.Desktop` | 데스크톱 전용 manager/controller/options/startup. 공통 Ui/Hosting/Framework에서 역참조하지 않음 |
| `Doroti.Desktop.Widgets` | 선택적 Widget 콘텐츠 adapter와 scope. 제안된 custom chrome 위젯은 미구현 |
| `DorotiWindowManager` | 앱 범위 창 생성·ID registry·목록·앱 종료 정책 |
| `DorotiWindowController` | 특정 WindowId와 수명에 귀속하는 제어·상태·이벤트·닫기 |
| `WindowCreateOptions` / `WindowContent` | 창 설정과 창별 콘텐츠 factory·생성 hook 분리 |
| `WindowOptions` / `WindowAppearanceOptions` | 크기·위치·동작과 배경·재질·테마·제목 표시줄 조합 |
| Desktop startup / companion | `UseMainWindow`로 기본 창 선언. 공통 앱은 target-neutral 상태로 유지하고 플랫폼별 companion을 SDK가 연결 |

기본 창도 manager가 관리한다. 프로세스 전역의 암묵적인 현재 창을 만들지 않으며, 향후 추가 창은 `CreateWindowAsync`와 같은 controller 계약을 사용하도록 설계했다. **fake-host 두 창 격리 검사는 실제 OS 다중 창 지원이 아니다.** Web·Android·iOS는 패키지/startup 참조를 diagnostic으로 거절하며 데스크톱 브라우저도 Web 대상이다.

## 보존할 동작 계약

- 준비 상태는 창별 `Created → NativeAttached → Initialized → ReadyToShow → Visible/Hidden → Closing → Closed`로 구분한다. hook은 `Task`/취소를 관찰하고, framework attach와 순환 대기를 만들지 않는다. ReadyToShow는 물리 화면 표시 완료를 보장하지 않는다. 숨긴 준비가 불가능한 host는 해당 시작 정책을 거절한다.
- Size/min/max는 **client 논리 단위**, Position/Bounds는 **전역 물리 pixel의 바깥 프레임 좌표**다. 안정적인 좌표 변환이 없는 host는 위치 요청을 거절하고 Bounds=null을 반환한다. caption inset과 SafeArea를 중복 적용하지 않는다.
- 조합별 capability를 평가하고 `Unsupported`/`RequiresRecreation`과 이유를 반환한다. 요청값과 실제값·시스템 fallback을 구분하며, 미지원 요청을 성공한 no-op으로 처리하지 않는다. 적용 실패 시 복구와 실제 부분 상태를 보고한다.
- 외형 변경은 창별 직렬 큐와 revision으로 처리한다. 대기 중인 외형 snapshot만 대체하며 각 요청을 Applied/Superseded/Rejected/Failed/Canceled로 한 번 종결한다. close·readiness·GPU 대기자도 실패/종료 시 정리한다.
- 제목 표시 방식, 배경 재질, 콘텐츠/버튼 소유자를 분리한다. 창 재질·콘텐츠 투명도·창 전체 opacity는 서로 다르다. Scaffold opacity 토글은 창 재질 on/off 검증이 아니다.
- native close 취소와 마지막 창 종료 정책을 분리한다. `OnLastWindowClosed`/`Explicit`의 앱 생존은 창 0개 뒤 reopen 지원과 별개다. Catalyst는 native close 취소를 제공하지 않는다.
- `UseView`와 legacy 외형은 호환 경로로 유지한다. `unified/solid`는 Normal+Backdrop/Normal+Solid 이전 방향이며, 새 API와 legacy가 같은 기본 창을 중복 소유하면 거절한다. 기존 `SingletonDorotiWindow`는 view metrics facade다.
- WebGL2 기본/명시 WebGPU, Windows 렌더러 선택, Flutter `Duration` 계약은 이 작업으로 변경하지 않는다.

## 단계별 보관 상태

| 단계 | 결과와 잔여 범위 |
| --- | --- |
| W0 계약·인벤토리 | 완료. 데스크톱 경계, 기본/추가 창 API와 소유권, 호환·지원 조합 문서화 |
| W1 공통 구현 | 핵심 구현 및 최초 fake-host 25개 PASS. 후속 플랫폼 정책 포함 32개 PASS. 한 창의 다중 view·일부 경합 확대 검증 잔여 |
| W2 Windows MAUI | 기본 제어·hidden readiness·200% DPI 조작 PASS. 최초 노출 전체 프레임·혼합 DPI 검증 잔여 |
| W3 외형·상단바 | Normal+System/Solid/Backdrop 픽셀 회귀 PASS. Hidden/Custom/Frameless, chrome 위젯, App theme bridge 및 전체 동적 변경/환경 gate 미완료 |
| W4 플랫폼 확장 | AppKit·제한된 Catalyst·Qt Quick 기본 창 연결. WindowsAppSDK 미구현. macOS/Qt 전체 수락 gate 미완료 |
| W5 샘플·패키지·이전 | Testbed·선택형 companion/template·지원 문서·플랫폼별 외부 package-only 실행 확보. custom 상단바 예제와 내부 Acrylic 채널 위임 등 잔여 |
| W6 다중 창 | 이번 첫 구현 범위 밖, 미구현. 두 OS 창·owner/modal·창별 입력/자원·마지막 창 종료/reopen 검증 필요 |

## Windows MAUI

Release/Core/Widgets 빌드와 25개 계약, 저장소 밖 package-only 소비를 통과했다. 200% DPI에서 hidden-ready, client 450×800·최소 350×500, resize 500×650, 제목·show/hide/focus·최대화/복원·최소화/전체화면·외형 교체/초기화·native close 취소→허용을 검사했다.

Normal caption의 Backdrop/Solid/System과 Acrylic body 조합은 자동 픽셀 회귀 PASS다. Backdrop의 body 색 응답은 84/14/102, caption은 193/30/233이고 DWM type=3이었다. Solid/System은 body 응답을 유지하면서 caption 응답 0/0/0, DWM type=1이었다. 이는 물리 디스플레이 전체 수락을 뜻하지 않는다.

기존 resize 24회는 native prepared frame 41개, timeout 0, Graphite device 생성 1회, 정상 종료였다. 그러나 표시 프레임 검사는 outer width 961px 준비 뒤 renderer 934px와 native client 기대 935px 불일치로 캡처 전에 실패했다. **리사이즈 시각 연속성은 NOT PASSED / PARTIAL**이며, 조작 성공으로 대체하지 않는다. [기존 Windows MAUI 검증](../../Doroti/validation/windows-maui/README.md)을 별도 기준으로 유지한다.

100/150%·혼합 DPI, 고대비·투명 효과 해제, 최초 노출 전체 프레임과 물리 외형은 notVerified다. Hidden/custom chrome·Snap/IME/접근성, App theme bridge, WindowsAppSDK adapter와 내부 `AcrylicOptionsState`/메시지 채널 위임은 남아 있다. 재생성이 필요한 runtime 재질 mode/frame 변경은 `RequiresRecreation`으로 거절한다.

## AppKit macOS

Apple M1/arm64, macOS 26.6.2, Xcode 27, .NET SDK 10.0.400, MAUI 10.0.90, `net10.0-macos27.0`, Retina 2×에서 Release 빌드 경고/오류 0, 계약 28개와 외부 package-only 실행을 통과했다. 기본 `net10.0-macos`와 Xcode 27 프로필은 별개이며 버전 검사를 우회하지 않는다.

Graphite/Ganesh에서 hidden readiness, 크기/제약, zoom·최소화·native fullscreen 완료, show/hide, Normal 재질 변경/복원, close 취소→허용을 확인했다. 앱 종료 delegate의 취소→허용과 Explicit에서 창 0개 후 앱 생존도 검사했다. native `PerformClose`/`Terminate` 호출 결과이며 물리 Cmd+W/Cmd+Q 입력 검증은 아니다.

숨긴 Metal 준비 중 GPU 대기를 해결하고 Show용 drawable을 보존했으며, 최초 show의 client 높이 축소와 비동기 최소화 완료, 종료 중 중복 lifecycle, Show/Focus 경합을 수정했다. Acrylic/LiquidGlass 재질 전환은 native 조작 PASS이고 캡처를 검토했지만, 통제 배경 blur 픽셀 회귀 전체는 미검증이다.

Position/SetBounds는 거절하고 Bounds=null이다. 창별 Dock 숨김·Hidden/custom/frameless·App theme bridge·programmatic resize·추가 창/owner는 미지원이다. 새 API의 하위 OS LiquidGlass는 명시한 Solid/Transparent fallback을 사용하며 legacy Glass→Acrylic과 구분한다. 실제 소비하지 않는 Acrylic tint/luminosity 값은 거절한다.

W4-M의 M0/M1/M4는 완료, M2/M3/M5는 부분이다. 혼합 화면 좌표, 물리 IME/VoiceOver, 하위 OS Glass fallback, 대비/투명도 OS 설정, 최초 표시·live resize 연속성, 서명/notarization/Gatekeeper·다른 Mac은 미검증이다. [macOS 실행 JSON](../../Doroti/validation/desktop-window/results-macos-2026-09-25.json)에 식별 정보와 기존 결과가 남아 있다.

## Mac Catalyst

AppKit과 별도 UIKit scene adapter를 추가했다. Mac idiom(`UIDeviceFamily=6`)·Graphite·Catalyst 16 이상을 요구하며, 검증한 Xcode 27의 `net10.0-maccatalyst27.0` 프로필은 SDK 최소 17이다. scene 닫기를 위해 multiple-scenes 설정을 켜지만 추가 창 생성은 거절한다.

`PlatformDefault` 시작과 `Explicit` lifetime만 제공한다. UIKit이 최초 표시와 앱 종료를 소유하므로 hidden launch를 보장하지 않는다. 복원된 scene 크기가 초기 요청을 덮을 수 있어 관측된 State와 준비 후 명시 resize를 사용한다. 제목·client 크기/min/max/resizable·기존 scene 활성화·불투명 System/Solid·System/Explicit theme를 연결했다.

Release 경고/오류 0, 계약 30/30, 실제 API 창 조작·API close 취소→허용·native scene disconnect·외부 package-only 실행·Web/Android/iOS negative build와 AppKit 회귀 PASS다. Explicit 생존 확인 후 fixture가 프로세스를 종료한 결과를 정상 앱 종료로 해석하지 않는다.

`CanCancelNativeClose=false`: API close만 managed 결정을 거치고 native 닫기/앱 종료는 UIKit 소유다. AppKit 재질·Acrylic/Glass·위치/센터링·topmost/Dock·hide/숨김 준비·programmatic 최소화/최대화/전체화면·custom chrome은 거절한다. 물리 traffic lights/키보드·전체 OS/디스플레이·Intel·기본 SDK 프로필 검증은 별도다. [Catalyst 실행 JSON](../../Doroti/validation/desktop-window/results-maccatalyst-2026-09-25.json)을 보존 근거로 사용한다.

## Linux Qt Quick

Quick+Graphite/Vulkan 기본 창 adapter, 별도 **Desktop ABI 1**(기존 host ABI 4 유지), owner dispatch·manager·SDK bootstrap·Linux companion/template을 연결했다. Testbed는 `DorotiLinuxDesktop=true`로 선택하고 legacy 기본 구성은 유지한다. Testbed의 Quick 기본값을 전체 SDK/template 기본값으로 일반화하지 않는다.

PlatformDefault, 불투명 System/Solid 배경·System theme·Normal/System native decoration과 제목·client 크기/min/max/resizable·show/hide/focus·최대화/복원/전체화면을 제공한다. native/API close는 취소→허용을 공유하며 자원 해제·Closed·registry 제거 뒤 lifetime 정책을 적용한다. 상태 전환이 확인되지 않으면 5초 내 실패로 종결한다.

Wayland 프로그램 최소화는 확인 가능한 상태가 없어 거절한다. Manual/WhenReady, 초기 최소화, Position/Bounds/Center, topmost/taskbar, deferred drag/resize, custom chrome, Desktop Acrylic·runtime appearance와 추가 창/reopen도 미지원이다. 기존 Wayland blur/client caption과 새 Desktop 지원을 구분한다.

Ubuntu 26.04.1 VM, .NET 10.0.400, Qt 6.10.2, KWin Wayland, software Vulkan에서 다음 기존 결과를 확보했다.

| 검사 | 결과·한계 |
| --- | --- |
| Release·계약 | 빌드 경고/오류 0, Desktop 32/32, Qt ABI/geometry, native/template 23개 byte 일치 PASS |
| Native Wayland | 실제 geometry·제약 복원·상태·native/API close 취소→허용·두 lifetime 정책 PASS. 최종 실행은 layer 실제 로드·VUID 없음 확인 |
| 외부 package/template | 24개 로컬 패키지, 저장소 밖 fresh consumer/cache, Quick ON/WebEngine OFF, source ProjectReference 없이 실행 PASS |
| 경계/전환 | 구형 shim의 Desktop 거절, non-Quick diagnostic, Web/Android/iOS package 거절, legacy→Desktop bootstrap 분리 PASS. 전체 Web/mobile 앱 실행 아님 |
| Legacy 회귀 | Wayland resize 10회, 기존 unified/KDE blur, 정상 종료·실패 frame 0·종료 후 retained Quick bytes 0. 이 실행의 layer 매핑은 별도 확인하지 않음 |
| xcb/XWayland | **FAILED**. 조작·닫기·정리는 통과했지만 layer 로드 실행에서 `VUID-VkSwapchainCreateInfoKHR-pNext-07781` 재현: 요청 540×480, surface 500×450. exit 0이어도 실패 |

실행 중 발견한 Wayland size hints 갱신 문제는 유휴 장면에서도 Quick update를 요청하도록 수정했다. setter signal과 실제 플랫폼 상태 event를 구분하고 timeout을 성공으로 취급하지 않는다. legacy/Desktop 생성 bootstrap 공유 문제는 startup별 파일을 분리해 해결했다. 앞선 실패 시도도 [Linux 실행 JSON](../../Doroti/validation/desktop-window/results-linux-2026-09-26.json)에 남아 있다.

W4-Q의 Q0/Q1 완료, Q2~Q5 부분 상태를 유지한다. Qt 재질·custom chrome, 순수 X11·물리 GPU·한글 IME/Orca·혼합 DPI·clean VM·self-contained 배포·전체 Quick/WebEngine 조합은 미완료/미검증이다. [WSI 조사와 완료 조건](../../Doroti/validation/linux-qt-quick/wsi-investigation-2026-09-24.md)을 별도 gate로 유지하며 Wayland PASS를 xcb 승인으로 확대하지 않는다.

## 후속 작업과 증거 경계

1. W1의 다중 view·경합 계약, W2 최초 표시/혼합 DPI와 기존 Windows resize 시각 gate를 완료한다.
2. W3 Hidden+Native chrome metrics·hit testing, TitleBar/DragRegion/CaptionButtons, Custom/Frameless의 OS 입력·Snap·접근성, App theme bridge와 동적 외형/시스템 fallback을 구현·검증한다.
3. W4 WindowsAppSDK command/event adapter와 내부 Acrylic 채널 위임, AppKit 잔여 입력/좌표/재질 환경 gate, Qt Q2~Q5 및 xcb WSI를 진행한다. 비데스크톱 negative package build를 전체 runner 실행으로 대체 해석하지 않는다.
4. W5 custom 상단바·runtime 예제와 이전 안내를 완성한다. 실제 다중 창·owner/modal·reopen은 W6로 분리한다.

[Runtime 전환 보관 요약](../26-09-24/runtime-dotnet-migration-summary.md)과 [Linux Qt 작업 보관 요약](../26-09-24/linux-qt-improvements-summary.md)의 잔여 작업은 이 창 API 기록으로 완료되지 않는다.

검증 재실행 시 [Desktop 검증 README](../../Doroti/validation/desktop-window/README.md)의 플랫폼별 명령을 사용한다. 명령당 1,200초 timeout, shared obj/GUI 직렬 실행과 새 출력 경로를 유지하고 반복은 보통 30회 이내로 제한한다. `notVerified`(미실행), `unsupported`(명시 비지원), `failed`(실패)를 구별한다. `Doroti/artifacts`와 임시 package/캡처는 삭제 가능한 로컬 산출물이며, 이 요약은 해당 원시 파일의 현재 존재를 보장하지 않는다. 보관 시에는 문서 링크·삭제·diff만 검사했으며 과거 실행을 새 PASS로 갱신하지 않았다.
