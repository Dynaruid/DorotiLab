# M2-A. Desktop 계약과 호스트 연결

원문: [개발 로드맵](../../plan.md) §3 M2-A · 우선순위: **P1** · 작업 상태: **PARTIAL** · 실행 검증: **범위별 PASS / 나머지 notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

기존 Desktop API의 계약·capability·종료 수명을 통합한다. 원문의 Windows 우선 연결은 Windows 작업에서 실행하고 다른 adapter는 각 플랫폼에서 검증한다.

선행: [M1](01-testing.md)의 최소 회귀 경로.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [macOS / AppKit](../platforms/macos.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

원문의 범위와 완료 기준을 유지하면서 2026-09-28~29 실행 기록에 따라 완료 부분과 잔여 부분을 분리했다. 체크된 항목은 명시한 호스트·검증 범위에 한정하며, 아래 날짜별 결과가 근거다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [x] Windows App SDK용 `IWindowHostFactory`/`IWindowHost`를 구현하고 SDK Desktop companion 허용 조건·템플릿·샘플을 함께 연결한다.
- [x] Windows App SDK의 크기·show/hide/focus·제목/native caption·close 취소/render drain을 기존 Desktop 계약으로 연결하고 native 상태·종료 회귀를 검증했다.
- [ ] 크기/DPI 제약·최소/최대 크기·appearance의 전체 지원 범위를 호스트별로 확인한다. 다른 adapter와 미지원 capability 정합은 아래 항목에서 추적한다.
- [ ] 첫 표시, 창 resize, 최소화/복원, 디스플레이 전환, 종료 중 pending GPU 작업을 실제 화면과 리소스 수명으로 검증한다.
- [ ] AppKit/Catalyst/Qt/Windows MAUI의 기존 adapter와 지원표를 맞춘다. 플랫폼이 제공하지 않는 동작은 capability와 명확한 거절 결과로 표현한다.

**완료 기준:** 기본 Windows 명령으로 실행한 샘플이 Desktop companion을 사용하며, native 창 닫기와 API 닫기가 동일한 취소·정리 경로를 따른다. live resize는 실제 표시 프레임의 geometry·깜빡임으로 판정한다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

## 2026-09-28 Windows App SDK 결과

현재 상태 보충: 아래는 단일 창 구현 당시 기록이다. 2026-09-29에는
[09](09-multiwindow.md)에서 추가 창/Explicit을 연결했고, 후속 보강에서 반복 Explicit 종료의
중복 알림을 수정했다. [최신 보강 결과](../README.md#2026-09-29-0010-보강-검토)를 함께 읽는다.

- 상태: **구현 및 native 상태/종료 회귀 PASS / 전체 PARTIAL**. revision: a93c047fe2e93d93cff3e0a6bf3c2789862fea81 + 작업 트리 변경 (미커밋).
- `WindowsAppSdkDesktopWindowHost`가 기존 native HWND/entrypoint를 소유하는 IWindowHost/IWindowHostFactory adapter로 연결됐다. 별도 Window API나 두 번째 root를 만들지 않는다. SDK DOROTIDESKTOP005, 기본 Testbed WindowsAppSdk, 새 앱 템플릿의 Desktop companion을 연결했다.
- Windows 10.0.26200, win-x64, .NET SDK 10.0.400, Debug JIT, 기본 Graphite/Vulkan/native D3D12 presentation. Windows build 경고 0, 오류 0.
- `pwsh -File Doroti/eng/doroti.ps1 validate -ValidationSuite WindowsSmoke` (먼저 Debug Windows build): 실제 창 제목, 500×650 DIP resize, show/hide/focus, 최대화/복원/최소화/전체화면, caption theme/reset을 확인한다. native WM_CLOSE와 API close 각각 첫 호출 취소·둘째 호출 종료, callback 2회·registry 0개·exit 0 PASS.
- first readiness는 native 성공 present에서 완료하며, accepted close는 기존 native render worker retirement·platform 자원 정리·framework unmount 후 controller 완료를 기다린다. 프로세스가 먼저 종료돼 registry callback이 누락되던 경로도 수정했다. DorotiWindowController의 Closed 표시와 callback 완료 사이에 들어온 두 번째 CloseAsync도 기존 close task를 기다리며, callback을 의도적으로 보류하는 결정적 공통 회귀를 추가했다.
- 실제 state.Scale=2 환경을 관찰했다. 450×800 요청은 모니터 작업영역 제약으로 실제 높이 779.5 DIP였고 상태에 요청값 대신 관찰값을 기록했다.
- 당시 capability는 단일 창/OnLastWindowClosed·standard native chrome이었으며 Explicit·추가 창은 거절했다. **2026-09-29에는 추가 창과 Explicit을 지원한다**([09 결과](09-multiwindow.md)). custom chrome 거절 및 backdrop/renderer 배경 변경의 RequiresRecreation 경계는 유지한다. Windows MAUI/AppKit/Qt/Catalyst 지원표의 오래된 일괄 Pending 문구도 host별 표로 교체했다.
- 미검증: Computer Use native pipe unavailable로 live resize 화면·깜빡임, 물리 drag, mixed-monitor 이동을 검증하지 못했다. native state smoke를 화면 품질 PASS로 표시하지 않는다. 다른 adapter는 이번 재실행 없음.
- 성공한 aggregate suite의 원시 로그/JSON은 자동 정리했다. 수동·실패 조사 폴더는 자동 승인 정책의 삭제 차단으로 M0의 정리 보류 목록에 남겼다. 다음은 [03 입력](03-input-accessibility-platformview.md)과 [04 렌더링](04-rendering-lifetime.md)의 물리/표시 검증이다.


## 2026-09-29 Linux / Qt 후속

Qt Quick의 실제 추가 창·두 lifetime·크기/제약/상태/close 취소와 drain을 Wayland/XWayland VM에서 검증했다. 물리 표시/mixed-DPI와 기존 appearance/placement 제한은 남는다.

상세 명령·환경·지원 경계: [Linux Qt 결과](../results/2026-09-29-linux-qt.md).


## 2026-09-29 macOS/AppKit 후속

AppKit 실제 추가 창과 양 Metal renderer의 크기/focus/appearance/최소화/fullscreen/close 취소·render drain을 확인했다. mixed-monitor·물리 live resize는 별도 잔여다.
[구현·명령·결과·잔여](../results/2026-09-29-macos-appkit.md)를 따른다.

2026-09-29 iOS/Catalyst 후속: [별도 구현·실행 결과](../results/2026-09-29-ios-catalyst.md). UIKit 연결·native 재생성·Catalyst 두 scene·activation을 보강했으며 전체 물리 입력·GPU·배포 완료와 구분한다.
