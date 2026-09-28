# iOS 작업 계획

원문: [plan.md](../../plan.md) M0~M7 · 작업 상태: **TODO** · 새 실행 검증: **notVerified**

[전체 작업 인덱스](../README.md)

UIKit 호스트를 대상으로 하며 실제 기기와 시뮬레이터, Graphite와 다른 renderer의 지원·증거를 각각 기록한다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행 기반 — M0/M1, P0 → P1

- [ ] iOS runner·toolchain·기기/시뮬레이터·renderer·build mode별 실행 경로를 정리하고 M1 공통 회귀 fixture를 연결한다.
- [ ] 기존 PlatformView 지원표의 adapter 범위와 실제 호출 경로를 확인한다. 별도 renderer 조합은 자동 지원으로 간주하지 않고 필요한 연결과 미지원 경계를 기록한다.

## 2. 입력·VoiceOver·PlatformView — M2-B, P1

- [ ] 실기기 한글 조합·selection/caret·후보창·multiline·clipboard·focus loss를 확인하고 Doroti TextField ↔ UITextField/WKWebView의 IME 소유권·키 중복/눌림 해제를 검증한다.
- [ ] 외부 키보드가 있는 환경의 Tab/Shift+Tab과 VoiceOver 이름·역할·값·상태·action·focus·탐색 순서를 실제 사용으로 확인한다.
- [ ] PlatformView 복수 owner·clip/transform/z-order·native-origin gesture 경쟁·생성/해제/재생성·늦은 응답·앱 복귀 후 입력을 검증한다.

## 3. 렌더링·수명 — M2-C, P1

- [ ] 기기·해상도·OS·renderer별 baseline과 예산을 정하고 리스트·VariableBlur·다중 효과·WKWebView overlay·이미지/영상 texture·크기 변화 장면을 측정한다.
- [ ] UI/GPU/readback/upload/present/메모리를 구분하고 capture geometry·pixel origin·축소 렌더링 품질, pause/resume·surface/device loss·해제 수명을 검증한다.
- [ ] 폰트 CDN/asset/offline·초기 교체·한글 fallback을 확인한다. iOS 브라우저 메모리 검증은 Web 문서에서 추적한다.

## 4. 앱 기능·Universal Link·복원 — M4/M5, P2

- [ ] FilePicker·URL launcher의 공통 패키지 연결, 취소·권한 거절·외부 파일 접근 수명·늦은 응답을 검증한다.
- [ ] 모바일 OS Drag & Drop 제공 범위를 정하고 지원 환경의 수신·action·좌표·비동기 읽기·취소를 capability와 함께 검증한다.
- [ ] Universal Link의 cold/warm activation을 Router 준비 전 대기·중복 처리와 연결하고 같은/잘못된 링크의 처리를 확인한다.
- [ ] lifecycle 기반 route·입력·선택 상태 저장, 정상 재시작/강제 종료 후 복원, version migration·실패 fallback을 검증한다.

## 5. 배포 — M7, P3

- [ ] package-only 소비 앱을 clean 환경에서 검증하고 실제 지원 Release·trimming·Mono AOT/NativeAOT 조합을 기기별로 기록한다.
- [ ] 서명·패키징·설치·업데이트·제거·데이터 유지·crash/로그와 장기 실행·반복 PlatformView 생성·GPU loss/앱 복귀 회귀를 검증한다.

## 완료 기준

실기기 입력·VoiceOver·화면·복귀와 배포 결과를 시뮬레이터 결과와 구분해 남긴다. Mac Catalyst 및 다른 renderer의 지원으로 확대 해석하지 않는다. M6 실제 추가 창은 이번 iOS 필수 범위에 추가하지 않는다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Maui](../../Doroti/src/Doroti.Host.Maui)
- [Doroti/src/Doroti.Target.iOS.Maui.ios-arm64](../../Doroti/src/Doroti.Target.iOS.Maui.ios-arm64)
- [Doroti/docs/platform-views/ios.md](../../Doroti/docs/platform-views/ios.md)
- [samples/DorotiTestbedApp/ios](../../samples/DorotiTestbedApp/ios)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 원시 산출물은 삭제 가능한 `Doroti/artifacts`, 보존할 요약·fixture는 추적되는 tests/docs/history에 둔다.

현재 기록: 작업 계획만 작성했다. 플랫폼 구현·build·실기기·성능 검증을 새로 실행하지 않았다. 각 항목의 완료 시 관련 공통 작업 문서와 플랫폼/호스트/renderer/build mode별 지원표를 함께 갱신한다.
