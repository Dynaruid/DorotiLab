# Android 작업 계획

2026-09-29: [08](../common/08-navigation-restoration.md)에서 `DorotiMauiActivity`와 Testbed protocol
IntentFilter를 연결했다. Galaxy S25(SM-S931N)/Android 16에 Release Mono AOT APK를 업데이트 설치하고
cold `/first`·warm `/second`·강제 종료 후 `/second` 복원을 실제 화면으로 확인했다.
ADB intent/실제 표시이며 물리 IME 입력과 구분한다. Android package-only·출시 서명은 아직 검증하지 않았다.

원문 요약: [plan 요약](../../../26-09-28/plan-summary.md) M0~M7 · 작업 상태: **PARTIAL — intent·route 복원 연결** · 실행 검증: **범위별 PASS / 나머지 notVerified**

[전체 작업 인덱스](../README.md)

첫 모바일 물리 검증 대상으로 삼는다. ABI·renderer·build mode·기기·WebView provider별 결과와 에뮬레이터 결과를 구분한다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행 기반 — M0/M1, P0 → P1

- [ ] manifest의 Android runner와 문서의 build/install/run 경로를 점검하고 필요한 SDK·기기 조건을 지원표에 기록한다.
- [ ] 공통 Cupertino 탭·ListView·한글 입력·Dialog 중간 프레임·VariableBlur·PlatformView fixture를 Android 샘플에 연결한다.

## 2. 입력·TalkBack·PlatformView — M2-B, P1

- [ ] 실기기 한글 조합 시작/갱신/확정/취소, selection/caret·후보창·multiline·clipboard·focus loss를 검증한다.
- [ ] Doroti TextField ↔ EditText/WebView의 IME 소유권·키 중복·눌림 해제를 확인하고 키보드가 있는 환경에서 Tab/Shift+Tab도 검증한다.
- [ ] TalkBack의 이름·역할·값·상태·action·focus·탐색 순서를 실제 사용으로 확인한다.
- [ ] PlatformView 복수 owner, clip/transform/z-order, native-origin gesture 경쟁, 생성·해제·재생성·늦은 응답과 앱 복귀 후 입력을 확인한다.

## 3. 렌더링·수명 — M2-C, P1

- [ ] 목표 실기기·해상도·renderer별 baseline/예산을 정하고 긴 리스트·VariableBlur·다중 효과·WebView overlay·texture·크기 변화 장면을 측정한다.
- [ ] UI/GPU/readback/upload/present/메모리를 구분하고 viewport·pixel origin·축소 품질을 검증한다. pause/resume·surface/device loss·리소스 해제를 확인한다.
- [ ] 폰트 CDN/asset/offline 정책과 한글 fallback을 해당 호스트에서 검증한다. Android 브라우저 검증은 Web 문서에 별도로 남긴다.

## 4. 네이티브 기능·Navigation — M4/M5, P2

- [ ] FilePicker·URL launcher의 target별 handler/native asset·등록·취소·권한 거절·파일 접근 수명을 검증한다.
- [ ] OS Drag & Drop의 제공 가능 범위를 조사하고 공통 capability에 연결한다. 지원하는 환경의 수신·좌표·stream·취소만 실행 결과로 기록한다.
- [x] `DorotiMauiActivity`·Testbed IntentFilter를 공통 activation/Router에 연결한다. Galaxy S25 / Android 16 / Release Mono AOT에서 ADB intent cold `/first`·warm `/second`와 강제 종료 후 `/second` 화면 복원을 확인했다(2026-09-29, [08 결과](../common/08-navigation-restoration.md)).
- [ ] 최종 공통 수명 수정 이후 재빌드한 APK를 다시 설치해 같은 경로를 재검증한다. 같은/잘못된 링크·back·lifecycle 충돌, 정상 재시작·입력/선택 상태 복원, 앱별 migration·실패 fallback을 확인한다. 기존 route 화면 PASS는 이 전체 범위를 포함하지 않는다.

## 5. 배포 — M7, P3

- [ ] clean package-only 앱의 restore/build/run/publish와 지원 ABI·Release·trimming·Mono AOT 등 실제 제공 모드를 검증한다.
- [ ] 목표 배포 패키지의 서명·설치·업데이트·제거·데이터 유지·crash/로그 수집을 실기기에서 확인한다.
- [ ] 장기 스크롤·PlatformView 반복 생성·GPU loss·앱 복귀 회귀를 검증하고 동일 revision/toolchain의 결과를 보존한다.

## 완료 기준

빌드·설치만으로 완료하지 않고 실제 화면·물리 입력·TalkBack·복귀·배포 결과를 기록한다. 미실행 ABI/기기는 `notVerified`로 유지한다. Android 기기 자동 검색·IDE 로그 통합은 M3 후속 후보이며 M6 추가 창 구현은 이번 필수 범위가 아니다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Maui](../../../../packages/platforms/maui/Doroti.Host.Maui)
- [Doroti/src/Doroti.Target.Android.Maui.android-arm64](../../../../packages/platforms/maui/Doroti.Target.Android.Maui.android-arm64)
- [Doroti/src/Doroti.Target.Android.Maui.android-x64](../../../../packages/platforms/maui/Doroti.Target.Android.Maui.android-x64)
- [Doroti/docs/platform-views/android-webview.md](../../../../Doroti/docs/platform-views/android-webview.md)
- [samples/DorotiTestbedApp/android](../../../../samples/DorotiTestbedApp/android)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 일회성 테스트·원시 로그·캡처·소비 앱은 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고 요약 후 정리한다. 제품 빌드·release 후보 등 기존 `Doroti/artifacts`는 별도의 삭제 가능한 산출물이며, 필요한 요약·최소 상시 fixture만 추적되는 tests/docs/history에 보존한다.

현재 기록: 2026-09-29 소스 기반 Testbed Release Mono AOT의 설치/업데이트와 ADB intent·route 복원 화면은 PASS다. 이후 최종 공통 수명 수정 뒤 재빌드는 통과했지만 기기 연결이 끊겨 최종 APK의 재설치는 수행하지 못했다. 물리 IME·TalkBack·전체 PlatformView·성능·package-only·출시 서명은 notVerified다. [08 결과](../common/08-navigation-restoration.md)와 [10 결과](../common/10-release-packaging.md)의 revision별 범위를 따르며 관련 지원표를 함께 갱신한다.


## 2026-09-29 후속 실행

[이번 세 플랫폼 실행 기록](../results/2026-09-29-web-windows-android.md)에
새 구현·실제 실행·후보와 미검증 경계를 기록했다. 전체 상태는 PARTIAL이다.
