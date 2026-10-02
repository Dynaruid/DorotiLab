# Mac Catalyst 작업 계획

원문 요약: [plan 요약](../../../26-09-28/plan-summary.md) M0~M7 · 작업 상태: **PARTIAL** · 새 실행 검증: **범위별 PASS / 잔여 notVerified**

[전체 작업 인덱스](../README.md)

macOS AppKit 및 iOS와 구분해 Catalyst adapter·scene·입력·배포를 독립적으로 검증한다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행·Desktop adapter — M0/M1/M2-A, P0 → P1

- [ ] `maccatalyst` runner·toolchain·renderer·build mode별 실행 경로를 확인하고 지원표 및 M1 회귀 장면을 연결한다.
- [ ] 기존 Desktop adapter의 실제 제공 범위를 조사하고 크기/DPI·제약·show/hide/focus·제목/appearance·close 취소/render drain을 Catalyst capability와 맞춘다.
- [ ] 창/scene의 첫 표시·resize·복귀·종료와 pending GPU 작업 정리를 실제 화면에서 검증한다. 플랫폼이 제공하지 않는 동작은 명확한 거절 결과로 표현한다.

## 2. 별도 PlatformView·입력·렌더링 — M2-B/C, P1

- [ ] Catalyst용 native hierarchy·WebView adapter와 합성 연결의 필요한 범위를 명시하고 구현한다. AppKit 또는 iOS Graphite 결과를 Catalyst 통과 근거로 사용하지 않는다.
- [ ] 한글 조합·selection/caret·후보창·multiline·clipboard·단축키·focus loss와 Doroti 입력 ↔ native view/WebView의 Tab/IME 소유권·키 중복을 검증한다.
- [ ] VoiceOver 이름·역할·값·상태·action·focus·탐색 순서를 실제 사용으로 확인한다. PlatformView 복수 owner·clip/transform/z-order·gesture 경쟁·재생성·늦은 응답도 검증한다.
- [ ] 목표 환경의 baseline/예산을 정하고 리스트·VariableBlur·효과·WebView overlay·texture·resize/DPR의 UI/GPU/upload/present/메모리를 분리 측정한다.
- [ ] geometry·폰트 CDN/asset/offline·한글 fallback·pause/resume·context/device loss·GPU 자원 수명을 확인한다.

## 3. 앱 기능 — M4/M5, P2

- [ ] FilePicker·URL launcher의 Catalyst 구현·asset/등록·취소·권한·파일 접근 수명·창 종료 중 응답을 검증한다.
- [ ] OS Drag & Drop의 실제 제공 범위를 조사해 공통 capability에 연결하고 지원하는 수신/송신·좌표·stream·취소 경로를 검증한다.
- [ ] Catalyst activation 제공 범위를 지원표에 기록하고 URI cold/warm 전달·Router 대기/중복 처리·scene lifecycle 저장·복원·migration·fallback을 연결한다.

## 4. scene 기반 추가 창 — M6, P2

- [ ] 공통 창 문맥 ADR을 Catalyst scene 계약에 맞춰 별도 구현한다. window manager/content factory와 scene 생성·종료의 소유권을 연결한다.
- [ ] 두 실제 창/scene의 독립 focus·입력/IME·DPI·스크롤·Navigator·semantics·PlatformView·GPU surface를 검증한다.
- [ ] 한 창 종료 후 다른 창 유지, 마지막 창/명시적 종료 정책, 닫힌 scene의 비동기 작업 정리와 cache 공유 경계를 확인한다.

## 5. 배포 — M7, P3

- [ ] clean package-only 앱에서 restore/build/run/publish와 실제 지원 Release·trimming·AOT 조합을 독립 검증한다.
- [ ] 서명·패키징·설치·업데이트·제거·데이터 유지·crash/로그와 장기 실행·창/PlatformView 반복 생성·GPU loss·복귀 회귀를 확인한다.

## 완료 기준

Catalyst의 실제 adapter·scene·물리 입력·VoiceOver·표시·배포 결과가 있어야 해당 범위를 완료한다. 별도 adapter나 환경이 준비되지 않은 조합은 `notVerified` 또는 의도한 `unsupported`로 유지한다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Maui](../../../../Doroti/src/Doroti.Host.Maui)
- [Doroti/src/Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64](../../../../Doroti/src/Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64)
- [Doroti/docs/desktop-windows.md](../../../../Doroti/docs/desktop-windows.md)
- [Doroti/docs/platform-views/support-matrix.md](../../../../Doroti/docs/platform-views/support-matrix.md)
- [samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacCatalyst.csproj](../../../../samples/DorotiTestbedApp/macos/DorotiTestbedApp.MacCatalyst.csproj)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 일회성 테스트·원시 로그·캡처·소비 앱은 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고 요약 후 정리한다. 제품 빌드·release 후보 등 기존 `Doroti/artifacts`는 별도의 삭제 가능한 산출물이며, 필요한 요약·최소 상시 fixture만 추적되는 tests/docs/history에 보존한다.

2026-09-29: [iOS/Catalyst 후속 결과](../results/2026-09-29-ios-catalyst.md)에 구현·build·실행 범위를 기록했다.
아래 보강과 기존 체크리스트의 전체 완료는 구분한다. 물리 입력·VoiceOver·GPU 예산·실제 배포 잔여를 유지한다.

- [x] UIKit FilePicker/URL·텍스트/URI Copy drop capability와 scene URL/Universal Link callback 연결.
- [x] 별도 runner/renderer/TFM을 기록하는 유지보수 smoke 및 iOS RID/TFM 참조 경로 보강.
- [x] native editor/WKWebView 각각 생성·재생성 실행.
- [ ] 물리 입력·VoiceOver·권한 대화상자·OS drag/Universal Link의 전체 외부 전달 검증.
- [ ] 실기기 서명/설치·AOT·clean 배포·성능 예산·장기 수명 검증.

- [x] 공통 창 manager → 실제 MAUI scene 생성·독립 크기·survivor 유지·registry 정리. UIKit native close 취소와 프로세스 종료 경계는 결과 문서 참고.


## 2026-10-01 Mac Catalyst metadata Hot Reload

`dev -Platform maccatalyst`와 VS Code `maccatalyst` 타겟을 연결했다. .NET SDK 10.0.401, Xcode 27 workload의 `net10.0-maccatalyst27.0/maccatalyst-arm64`, Mono Debug, Graphite Metal에서 소스 Testbed를 실행했다. 개발 세션은 `MtouchInterpreter=all,-Doroti.Host.Maui`, `MtouchLink=None`과 별도 캐시를 사용한다. 일반 Debug/Release 프로필은 유지한다.

SDK WebSocket agent가 UIKit 초기화 중 연결을 잃는 현상을 재현했다([upstream issue](https://github.com/dotnet/sdk/issues/55488)). 로컬 자식 프로세스로 실행되는 Catalyst에는 SDK 기본 named-pipe 전송을 선택했다. SDK 설치나 agent 바이너리는 변경하지 않는다. SDK의 성공 문자열 대신 실제 완료 프레임·메서드 결과로 검증한다.

- **PASS:** 실제 metadata 변경과 두 번째 delta, 동일 PID/State/count/한글 텍스트/scroll 보존, 컴파일 오류 복구, ENC0009 뒤 기존 앱·상태 유지, Stop과 앱 PID 소멸.
- **PASS:** 별도 profile에 설치한 VSIX의 Run → Hot Reload 명령 → 상태 보존 → Stop. CLI smoke는 `temp/testing/mac-hot-reload/catalyst-3`, 설치 VSIX는 `catalyst-vsix`에 원시 근거를 남겼다.
- **PASS:** Apple 빌드 프로필·기존 iOS 회귀, 일반 Mac Debug/Release 유지, 확장 단위 테스트, SDK/VSIX 패키징과 새 빌드 hook 포함 확인.

입력 상태는 자동으로 주입했다. 물리 IME·VoiceOver·다른 renderer·Release/NativeAOT·모든 native host/binding 편집까지 검증한 결과는 아니다. native 변경은 재빌드/재시작이 필요하다. [개발 계약](../../../../Doroti/docs/development-hot-reload.md)과 [테스트 문서](../../../../Doroti/tests/README.md)에 재현 명령을 기록했다.
