# macOS / AppKit 작업 계획

원문 요약: [plan 요약](../../../26-09-28/plan-summary.md) M0~M7 · 작업 상태: **PARTIAL** · 새 실행 검증: **범위별 PASS / 잔여 notVerified**

[전체 작업 인덱스](../README.md)

2026-09-29: [AppKit 구현·실행 결과](../results/2026-09-29-macos-appkit.md). 아래 복합 체크리스트는 구현 완료 부분과 물리/전체 qualification 잔여를 함께 포함하므로 미검증 항목을 일괄 체크하지 않는다.

AppKit 호스트를 대상으로 한다. Mac Catalyst 작업과 지원 결과는 별도 문서에서 관리한다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행·Desktop adapter — M0/M1/M2-A, P0 → P1

- [x] macOS runner·toolchain·renderer·build mode별 실행 경로를 확인하고 문서와 지원표를 맞춘다. M1 공통 회귀 fixture를 연결한다.
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

- [x] 공통 창 문맥 ADR과 Windows 선행 연결을 바탕으로 AppKit 실제 추가 창을 구현했다. 창별 focus·입력·scheduler·Navigator·semantics·PlatformView·GPU surface와 공유 cache 소유권을 지킨다.
- [ ] 두 창의 독립 스크롤·IME·DPI·native content·rendering·닫기, 마지막 창 정책과 닫힌 창의 pending 작업 종료를 검증한다.
- [x] `_window_macos.cs` Satellite의 지원 경계를 명시했다. Satellite는 Desktop manager와 연결하지 않으며 현재 unsupported다. owner/modal/satellite·popup/tooltip은 기본 추가 창 이후 진행한다.

## 5. 배포 — M7, P3

완료한 부분: 격리 NuGet-only Release/CoreCLR 소비 앱, .pkg 확장 후 실제 추가 창 실행, ad-hoc app 서명 검사, 로컬 추출 앱의 설치/업데이트/제거·한글 userdata fixture 보존. 최종 후보 경로와 해시는 [실행 결과](../results/2026-09-29-macos-appkit.md)에 기록했다. 아래 clean 환경/실제 배포 전체 기준은 아직 미완료다.

- [ ] package-only 소비 앱과 실제 지원 Release·trimming·AOT 조합을 clean 환경에서 검증한다.
- [ ] 선택한 배포 방식의 서명·패키징·설치·업데이트·제거·데이터 유지·crash/로그와 장기 실행·리소스 누수 회귀를 검증한다.

## 완료 기준

AppKit 실제 창·VoiceOver·물리 입력·실제 표시와 clean 배포 결과로 판정한다. 다른 Apple 호스트의 통과 근거를 재사용하지 않는다. macOS VS Code의 Run/Hot Reload/Stop 검증은 아래 2026-10-01 결과를 따른다. 마법사와 전체 IDE 예외 UI 검증은 별도 잔여다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Maui](../../../../packages/platforms/maui/Doroti.Host.Maui)
- [Doroti/src/Doroti.Target.MacOS.Maui.osx-arm64](../../../../packages/platforms/maui/Doroti.Target.MacOS.Maui.osx-arm64)
- [Doroti/docs/platform-views/macos.md](../../../../Doroti/docs/platform-views/macos.md)
- [Doroti/src/Doroti.Framework.Widgets/_window_macos.cs](../../../../Doroti/src/Doroti.Framework.Widgets/windowing.cs)
- [samples/DorotiTestbedApp/macos](../../../../samples/DorotiTestbedApp/macos)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 일회성 테스트·원시 로그·캡처·소비 앱은 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고 요약 후 정리한다. 제품 빌드·release 후보 등 기존 `Doroti/artifacts`는 별도의 삭제 가능한 산출물이며, 필요한 요약·최소 상시 fixture만 추적되는 tests/docs/history에 보존한다.

현재 기록: 실제 M1 Mac의 AppKit 두 창·Metal Graphite/Ganesh·Desktop 상태·NSOpenPanel 취소·native pasteboard·WKWebView 재생성·LaunchServices warm URL·route 복원을 새로 검증했다. 자세한 합격/잔여와 출시 결과는 [이번 실행 결과](../results/2026-09-29-macos-appkit.md)를 따른다. 각 항목의 완료 시 관련 공통 작업 문서와 플랫폼/호스트/renderer/build mode별 지원표를 함께 갱신한다.


## 2026-10-01 AppKit metadata Hot Reload

`dev -Platform macos`와 VS Code `macos` 타겟을 연결했다. .NET SDK 10.0.401, Xcode 27 workload의 `net10.0-macos27.0/osx-arm64`, CoreCLR Debug, Graphite Metal에서 소스 Testbed를 실행했다. SDK WebSocket agent가 실제 metadata delta를 적용하고 Doroti가 완료 프레임을 응답한다. `LinkMode=None`과 별도 개발 캐시를 사용하며 일반 Debug/Release 프로필은 유지한다.

- **PASS:** 실제 코드 변경과 두 번째 delta, 동일 PID/State/count/한글 텍스트/scroll 보존, 컴파일 오류 복구, ENC0009 뒤 기존 앱·상태 유지, Stop과 앱 PID 소멸.
- **PASS:** 별도 profile에 설치한 VSIX의 Run → Hot Reload 명령 → 상태 보존 → Stop. CLI smoke는 `temp/testing/mac-hot-reload/appkit-5`, 설치 VSIX는 `appkit-vsix`에 원시 근거를 남겼다.
- **PASS:** Apple 빌드 프로필 회귀, 기존 iOS 설정, 일반 Mac Debug/Release 유지, 확장 단위 테스트와 SDK/VSIX 패키징.

입력 상태는 자동으로 주입했다. 물리 IME·VoiceOver·다른 renderer·Release/NativeAOT·모든 native host/binding 편집까지 검증한 결과는 아니다. native 변경은 재빌드/재시작이 필요하다. 재현 명령은 [개발 계약](../../../../Doroti/docs/development-hot-reload.md), [테스트 문서](../../../../Doroti/tests/README.md)를 따른다.
