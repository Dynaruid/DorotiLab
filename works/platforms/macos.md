# macOS / AppKit 작업 계획

원문: [plan.md](../../plan.md) M0~M7 · 작업 상태: **TODO** · 새 실행 검증: **notVerified**

[전체 작업 인덱스](../README.md)

AppKit 호스트를 대상으로 한다. Mac Catalyst 작업과 지원 결과는 별도 문서에서 관리한다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행·Desktop adapter — M0/M1/M2-A, P0 → P1

- [ ] macOS runner·toolchain·renderer·build mode별 실행 경로를 확인하고 문서와 지원표를 맞춘다. M1 공통 회귀 fixture를 연결한다.
- [ ] 기존 AppKit adapter의 크기/DPI·제약·show/hide/focus·제목/caption/appearance·close 취소/render drain을 Desktop 계약에 맞춘다.
- [ ] 첫 표시·live resize·최소화/복원·디스플레이 전환·pending GPU 작업 중 종료를 실제 화면과 자원 수명으로 검증한다. 미제공 동작은 capability로 표현한다.

## 2. 입력·VoiceOver·WKWebView·렌더링 — M2-B/C, P1

- [ ] 한글 조합·selection/caret·후보창·multiline·clipboard·단축키·focus loss와 Doroti 입력 ↔ NSView/WKWebView의 Tab/IME 소유권을 검증한다.
- [ ] VoiceOver semantics·action·focus·탐색 순서를 실제 사용으로 확인하고 PlatformView 복수 owner·clip/transform/z-order·gesture 경쟁·재생성·늦은 응답을 검증한다.
- [ ] Metal 등 목표 renderer별 baseline/예산을 정하고 리스트·VariableBlur·효과·WKWebView overlay·texture·resize/DPR의 UI/GPU/upload/present/메모리를 구분 측정한다.
- [ ] capture geometry·폰트 CDN/asset/offline·한글 fallback·pause/resume·device loss·GPU retirement 수명을 확인한다.

## 3. 앱 기능 — M4/M5, P2

- [ ] FilePicker·URL launcher와 파일 접근 수명·취소·권한/오류·창 종료 중 응답을 공통 SDK 계약으로 연결한다.
- [ ] Finder 파일/텍스트/URI drop의 좌표/action/MIME·여러/큰 파일·stream·취소를 검증한 뒤 OS 송신·drag 이미지·창 간 move 책임을 확장한다.
- [ ] macOS activation 제공 범위를 지원표에 명시하고 cold/warm URI → Router와 대기·중복 처리·상태 저장/복원·migration·fallback을 연결한다.

## 4. 추가 창 — M6, P2

- [ ] 공통 창 문맥 ADR과 Windows 선행 연결을 바탕으로 AppKit 실제 추가 창을 구현한다. 창별 focus·입력·scheduler·Navigator·semantics·PlatformView·GPU surface와 공유 cache 소유권을 지킨다.
- [ ] 두 창의 독립 스크롤·IME·DPI·native content·rendering·닫기, 마지막 창 정책과 닫힌 창의 pending 작업 종료를 검증한다.
- [ ] `_window_macos.cs` Satellite를 Desktop 소유권으로 연결하거나 지원 경계를 명시한다. owner/modal/satellite·popup/tooltip은 기본 추가 창 이후 진행한다.

## 5. 배포 — M7, P3

- [ ] package-only 소비 앱과 실제 지원 Release·trimming·AOT 조합을 clean 환경에서 검증한다.
- [ ] 선택한 배포 방식의 서명·패키징·설치·업데이트·제거·데이터 유지·crash/로그와 장기 실행·리소스 누수 회귀를 검증한다.

## 완료 기준

AppKit 실제 창·VoiceOver·물리 입력·실제 표시와 clean 배포 결과로 판정한다. 다른 Apple 호스트의 통과 근거를 재사용하지 않는다. macOS VS Code 확장 검증은 M3 후속 후보로 유지한다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Maui](../../Doroti/src/Doroti.Host.Maui)
- [Doroti/src/Doroti.Target.MacOS.Maui.osx-arm64](../../Doroti/src/Doroti.Target.MacOS.Maui.osx-arm64)
- [Doroti/docs/platform-views/macos.md](../../Doroti/docs/platform-views/macos.md)
- [Doroti/src/Doroti.Framework.Widgets/_window_macos.cs](../../Doroti/src/Doroti.Framework.Widgets/_window_macos.cs)
- [samples/DorotiTestbedApp/macos](../../samples/DorotiTestbedApp/macos)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 원시 산출물은 삭제 가능한 `Doroti/artifacts`, 보존할 요약·fixture는 추적되는 tests/docs/history에 둔다.

현재 기록: 작업 계획만 작성했다. 플랫폼 구현·build·실기기·성능 검증을 새로 실행하지 않았다. 각 항목의 완료 시 관련 공통 작업 문서와 플랫폼/호스트/renderer/build mode별 지원표를 함께 갱신한다.
