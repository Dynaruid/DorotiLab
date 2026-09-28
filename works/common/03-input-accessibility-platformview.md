# M2-B. 텍스트·접근성·PlatformView

원문: [개발 로드맵](../../plan.md) §3 M2-B · 우선순위: **P1** · 작업 상태: **TODO** · 실행 검증: **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

공유 입력·semantics·PlatformView 수명 계약과 재현 장면을 담당한다. 보조기술과 native 입력 결과는 호스트마다 판정한다.

선행: [M1](01-testing.md)의 최소 회귀 경로와 대상 호스트의 입력·view 연결.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

아래는 원문의 체크리스트와 완료 기준을 보존한 것이다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [ ] 한글 조합 시작/갱신/확정/취소, selection/caret, 후보창 좌표, multiline, clipboard, 단축키, focus loss를 묶은 입력 장면을 만든다.
- [ ] Doroti TextField ↔ native TextBox/EditText/WebView 이동에서 Tab/Shift+Tab, IME 소유권, 키 중복 전달과 눌림 상태 해제를 검증한다.
- [ ] semantics의 이름·역할·값·상태·action·focus·탐색 순서를 검증하고, UIA/TalkBack/VoiceOver/Orca 실제 사용 결과를 별도로 기록한다.
- [ ] PlatformView 복수 owner, clip/transform/z-order, gesture 경쟁, 생성·해제·재생성, 늦게 도착한 비동기 결과를 점검한다.
- [ ] Windows MAUI WebView 연결 및 Catalyst 등 별도 adapter가 필요한 조합은 구현 범위를 명시해 처리한다. 기본 Windows App SDK 결과를 다른 호스트의 통과 근거로 사용하지 않는다.

**완료 기준:** 목표 호스트에서 조합 입력 중 focus 이동과 native view 재생성 후에도 입력·semantics가 정상이다. 미검증 보조기술/기기는 지원표에 그대로 남긴다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

현재 기록: 문서 분리만 수행했다. 이 작업 묶음의 구현·실행 결과는 아직 기록하지 않았다.
