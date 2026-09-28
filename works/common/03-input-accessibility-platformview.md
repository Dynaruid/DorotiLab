# M2-B. 텍스트·접근성·PlatformView

원문: [개발 로드맵](../../plan.md) §3 M2-B · 우선순위: **P1** · 작업 상태: **PARTIAL** · 실행 검증: **범위별 PASS / 나머지 notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

공유 입력·semantics·PlatformView 수명 계약과 재현 장면을 담당한다. 보조기술과 native 입력 결과는 호스트마다 판정한다.

선행: [M1](01-testing.md)의 최소 회귀 경로와 대상 호스트의 입력·view 연결.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [x] 한글 조합 시작/갱신/확정/취소, selection/caret, 후보창 좌표, multiline, clipboard, 단축키, focus loss를 묶은 입력 장면을 만든다.
- [ ] Doroti TextField ↔ native TextBox/EditText/WebView 이동에서 Tab/Shift+Tab, IME 소유권, 키 중복 전달과 눌림 상태 해제를 검증한다.
- [ ] semantics의 이름·역할·값·상태·action·focus·탐색 순서를 검증하고, UIA/TalkBack/VoiceOver/Orca 실제 사용 결과를 별도로 기록한다.
- [ ] PlatformView 복수 owner, clip/transform/z-order, gesture 경쟁, 생성·해제·재생성, 늦게 도착한 비동기 결과를 점검한다.
- [ ] Windows MAUI WebView 연결 및 Catalyst 등 별도 adapter가 필요한 조합은 구현 범위를 명시해 처리한다. 기본 Windows App SDK 결과를 다른 호스트의 통과 근거로 사용하지 않는다.

**완료 기준:** 목표 호스트에서 조합 입력 중 focus 이동과 native view 재생성 후에도 입력·semantics가 정상이다. 미검증 보조기술/기기는 지원표에 그대로 남긴다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

## 2026-09-28 구현 및 검증 결과

- 상태: **PARTIAL**. revision: a93c047fe2e93d93cff3e0a6bf3c2789862fea81 + 작업 트리 변경 (미커밋). Windows 10.0.26200, 기본 Windows App SDK, win-x64 Debug JIT, Graphite/Vulkan.
- Testbed에 `DOROTI_SAMPLE=input` 전용 장면을 추가했다. 앞/뒤 framework TextField, multiline, selection/composing 범위 표시, native TextBox 또는 WebView 전환, remove/create/recreate 버튼을 제공한다. 기존 clipboard·focus·semantics 경로를 사용한다.
- `Doroti.Tests`의 pointer focus 및 합성 한글 조합 시작/갱신/확정/취소·selection 보존, unmount 후 text-client 해제 PASS. 이것은 OS IME나 후보창/보조기술의 물리 검증이 아니다.
- `WindowsSmoke`의 input 장면에서 editor 최초 생성/재생성, WebView 최초 생성/재생성 총 4회, native view 제거 후 API 닫기·취소·registry 0개·exit 0 PASS. WebView와 HWND editor를 동시에 합성하면 기존 호스트가 거절하므로 두 종류는 전환하며 검증한다. 같은 프레임 혼합 지원을 새로 광고하지 않는다.
- framework root/focus/binding 및 Windows render/resource shutdown 순서를 수정해 종료 중 남은 focus microtask와 종료 callback 누락을 방지했다.
- 물리 Windows 앱 조작은 Computer Use의 native pipe 연결 오류로 진행하지 못했다. Korean IME cancel·focus 이동 중 조합, 후보창 좌표, selection/caret, Tab/Shift+Tab, 키 중복, UIA는 **notVerified**. 복수 owner/clip/transform/z-order/gesture·late-result 전체 matrix, Windows MAUI WebView/Catalyst 별도 adapter도 미완료다.
- 현재 지원 경계는 [지원표](../../Doroti/docs/support-status.md)에 기록했다. 성공한 aggregate suite의 원시 실행 파일·로그·JSON은 자동 정리했다. 수동·실패 조사 폴더는 자동 승인 정책의 삭제 차단으로 M0의 정리 보류 목록에 남겼다. 다음은 위 전용 장면으로 물리 입력/보조기술 검증을 수행하고 기능별 결함을 수정하는 것이다.
