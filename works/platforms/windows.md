# Windows 작업 계획

2026-09-29: [08](../common/08-navigation-restoration.md) protocol cold/warm/restart,
[09](../common/09-multiwindow.md) 실제 HWND 2개·native TextBox island·독립 resize·두 lifetime,
[10](../common/10-release-packaging.md) NuGet-only Release 소비를 추가했다.
기존 WindowsSmoke의 API/native close와 editor/WebView 재생성도 다시 통과했다.
native UI 도구 pipe가 없어 물리 IME·화면 resize 품질·mixed-monitor 판정은 미검증이다.

원문: [plan.md](../../plan.md) M0~M7 · 작업 상태: **PARTIAL** · 새 실행 검증: **범위별 PASS / 나머지 notVerified**

[전체 작업 인덱스](../README.md)

Windows App SDK를 기본 runner와 첫 desktop 검증 대상으로 삼는다. Windows MAUI는 별도 호스트 작업으로 추적한다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 기본 실행·Desktop 연결 — M0/M1/M2-A, P0 → P1

- [x] 기본 Windows App SDK runner와 Desktop companion을 연결하고 로컬 Debug build·WindowsSmoke 및 CI 실행 경로를 구성했다. [00 결과](../common/00-foundation.md)
- [ ] clean checkout/OS와 원격 CI에서 실행을 확인한다. GPU smoke는 interactive self-hosted runner 결과로 따로 기록한다.
- [x] Windows App SDK용 `IWindowHostFactory`/`IWindowHost`, `DOROTIDESKTOP005` 허용 조건, 템플릿·Testbed를 연결했다. 실제 창 크기·제목·show/hide/focus·최소화/복원·caption 및 API/native close 취소·정리 회귀 PASS. [02 결과](../common/02-desktop-contract.md)
- [ ] 최소/최대 크기·DPI 제약과 appearance의 전체 지원 범위를 확인하고 미지원 동작은 capability로 명시한다. 실제 화면·mixed-monitor 결과는 다음 항목에서 검증한다.
- [ ] 첫 표시·live resize·최소화/복원·디스플레이 이동·종료 중 pending GPU 작업을 실제 화면과 자원 수명으로 검증한다. native 닫기와 API 닫기가 같은 취소·정리 경로를 거치는지 확인한다.

## 2. 입력·접근성·합성·렌더링 — M2-B/C, P1

- [ ] 한글 조합 시작/갱신/확정/취소, caret·selection·후보창 좌표, multiline·clipboard·단축키·focus loss를 검증한다.
- [ ] Doroti TextField ↔ native TextBox/EDIT/WebView2 사이 Tab/Shift+Tab, IME 소유권, 키 중복·눌림 해제를 검증하고 UIA 이름·역할·값·상태·action·탐색 순서를 실제 보조기술로 확인한다.
- [ ] PlatformView 복수 owner, clip/transform/z-order, gesture 경쟁, 생성·해제·재생성 및 늦은 비동기 응답을 검증한다.
- [ ] Windows MAUI의 native hierarchy와 WebView 합성 연결 범위를 조사·구현하고 독립 장면으로 검증한다. 호스트별 미지원 조합은 capability와 명확한 오류로 표현한다.
- [ ] 긴 리스트·VariableBlur·다중 효과·WebView overlay·이미지/영상 texture·resize/DPR 장면에서 build/layout/paint, GPU, readback/upload, present 간격, 메모리/VRAM을 구분해 측정한다. device loss 복구와 폰트 CDN/asset/offline·한글 fallback도 확인한다.

## 3. 개발 도구 — M3, P1

- [x] 생성 앱과 clean profile의 설치 VSIX에서 snippet/import의 editor API 경로, Windows App SDK Debug Run·중복 Run·Hot Reload·Stop을 확인했다(2026-09-28). 실제 C# 서비스가 아닌 공급된 Quick Fix 진단을 사용한 범위는 [05 결과](../common/05-vscode-hot-reload.md)를 따른다.
- [x] 실제 metadata update 후 동일 runtime/State·입력·스크롤 유지, 컴파일 오류 후 수정·재시도, 중복 요청 직렬화를 확인했다. 미지원 편집은 restart 필요로 보고하고 실행 전·Release·미지원 환경의 버튼 비활성화 계약을 구현했다.
- [ ] 실제 생성 마법사 전체 UI·C# 서비스 로딩/타입 충돌·모든 snippet 편집·restricted profile·Restart/종료 경합을 검증한다. 2026-09-29 창 문맥 변경 후 설치 VSIX의 실제 metadata 흐름도 재검증한다. Windows 결과와 Web 결과를 분리한다.

## 4. 앱 기능과 추가 창 — M4/M5/M6, P2

- [x] FilePicker·URL launcher를 공통 플러그인 계약으로 연결하고 취소·파일 접근 수명·권한/오류·창 종료 중 응답을 검증한다. 2026-09-28 첫 Windows App SDK 범위: 실제 OS picker·취소·파일 읽기·브라우저 loopback·NuGet-only Release 소비 PASS. 권한 거절은 synthetic 계약 회귀이며 실제 ACL/물리 입력/MAUI는 notVerified. [M4 상세 결과](../common/06-plugin-sdk.md)
- [x] Windows OLE Copy 수신을 연결했다. 실제 OLE fixture의 다중 파일/한글/URI, 5GB sparse 파일·4GB offset·96/192 DPI·수명·NuGet-only Release 소비 PASS(2026-09-28). [M5-A 결과](../common/07-os-drag-drop.md)
- [ ] Explorer 교차 창 전달·물리 입력·드래그 중 mixed-DPI·취소를 검증하고, OS 송신·drag 이미지·move/link·원본 처리 책임·virtual file·창 간 이동을 확장한다.
- [x] opt-in Windows protocol 전달을 Router에 연결하고 실제 runner cold/warm·강제 종료 뒤 route 복원을 확인했다. [08 결과](../common/08-navigation-restoration.md)
- [ ] 설치 단계의 OS protocol 등록·OS 링크 열기, 앱별 schema migration·사용자 입력/선택의 전체 복원 사례를 검증한다.
- [x] 창 문맥 ADR과 `CreateWindowAsync`/fresh `WindowContent`를 실제 추가 창에 연결했다. 두 HWND·TextBox island·독립 크기·survivor resize·`OnLastWindowClosed`/`Explicit` 종료 및 NuGet 소비 PASS. [09 결과](../common/09-multiwindow.md)
- [ ] 두 창의 물리 입력/IME·스크롤·mixed-monitor DPI·실제 화면 품질·WebView 등 전체 native content·창 간 이동을 검증한다.
- [ ] `_window_win32.cs` Satellite의 Desktop 연결 또는 지원 경계를 정리해 native 창 중복 소유를 막는다.

## 5. 배포 — M7, P3

- [x] 격리 NuGet cache의 package-only Release self-contained JIT 소비 앱과 두 실제 창 실행, 개발 머신의 portable 설치/업데이트/제거·userdata 보존을 검증했다. [10 결과](../common/10-release-packaging.md)
- [ ] 후속 수정이 포함된 새 후보로 clean OS 설치/실행을 검증하고 실제 지원하는 Release·trimming·AOT 조합을 명시한다. 기존 후보에 이후 보강 결과를 합치지 않는다.
- [ ] 선택한 Windows 배포 형식의 서명·설치·업데이트·제거·데이터 유지·crash/로그를 검증하고 장기 실행·반복 창/PlatformView 생성·GPU loss 회귀를 확인한다.

## 완료 기준

기본 명령으로 실행한 Testbed가 Desktop companion을 사용하며 실제 표시·입력·닫기 수명이 정상이어야 한다. Windows App SDK와 MAUI 결과를 각각 기록한다. M6는 실제 OS 창 두 개로, M7은 저장소와 개발자 cache가 없는 환경으로 판정한다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.WindowsAppSdk](../../Doroti/src/Doroti.Host.WindowsAppSdk)
- [Doroti/src/Doroti.Host.Maui](../../Doroti/src/Doroti.Host.Maui)
- [Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets](../../Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets)
- [Doroti/src/Doroti.Desktop](../../Doroti/src/Doroti.Desktop)
- [Doroti/docs/platform-views/windows-webview.md](../../Doroti/docs/platform-views/windows-webview.md)
- [samples/DorotiTestbedApp/windowsappsdk](../../samples/DorotiTestbedApp/windowsappsdk)
- [samples/DorotiTestbedApp/windows](../../samples/DorotiTestbedApp/windows)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 일회성 테스트·원시 로그·캡처·소비 앱은 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고 요약 후 정리한다. 제품 빌드·release 후보 등 기존 `Doroti/artifacts`는 별도의 삭제 가능한 산출물이며, 필요한 요약·최소 상시 fixture만 추적되는 tests/docs/history에 보존한다.

## 2026-09-28 갱신

현재 공통 00~03의 실행 결과는 [지원표](../../Doroti/docs/support-status.md)와 각 공통 작업 문서에 기록했다. revision: a93c047fe2e93d93cff3e0a6bf3c2789862fea81 + 작업 트리 변경 (미커밋).

Windows App SDK Debug build, 실제 창 API/native close 취소와 registry 정리, editor/WebView 각각의 생성·재생성 smoke PASS. 물리 IME/Tab/UIA·live resize 화면 검증은 native UI 도구 연결 오류로 notVerified. MAUI 및 다른 renderer/build mode로 통과 범위를 확대하지 않는다.

공통 04·05 추가: 루트 renderer layer 영구 종료 후 잔여 0 회귀 PASS. 생성 앱·설치 VSIX에서 Windows metadata Hot Reload로 실제 문구 변경과 State/count/한글 값/scroll 유지, 컴파일 오류 후 수정·재시도, 중복 요청 직렬화, Stop을 확인했다. 자동 seed/VS Code API 경로이며 물리 입력·GPU 표시 시간 측정과 구분한다. [렌더링 결과](../common/04-rendering-lifetime.md)·[VS Code 결과](../common/05-vscode-hot-reload.md)에 범위와 미검증 조합을 기록했다.


## 2026-09-29 후속 실행

[이번 세 플랫폼 실행 기록](../results/2026-09-29-web-windows-android.md)에
새 구현·실제 실행·후보와 미검증 경계를 기록했다. 전체 상태는 PARTIAL이다.
