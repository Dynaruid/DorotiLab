# Web 작업 계획

2026-09-29: [08](../common/08-navigation-restoration.md)의 main-owned runtime history/저장 경로와
[10](../common/10-release-packaging.md)의 격리 NuGet cache template Release publish를 추가했다.
Node history/storage 계약은 통과했다. Chrome/IAB가 사용 불가이고 browser inventory가 비어
이번 실제 history UI는 notVerified다. worker-owned framework navigation과 Web AOT는 미지원/미검증 범위를 유지한다.

원문 요약: [plan 요약](../../../26-09-28/plan-summary.md) M0~M7 · 작업 상태: **PARTIAL** · 새 실행 검증: **범위별 PASS / 나머지 notVerified**

[전체 작업 인덱스](../README.md)

Windows와 함께 첫 개발·자동화 기준으로 삼는다. 브라우저·renderer·Debug/Release·desktop/mobile Web 결과를 따로 남긴다.

## 공통 작업과 의존성

M0 → M1 최소 경로 → M2 → M4/M5 → M6(대상 플랫폼) → M7 순서로 진행한다. M3의 생성·실행은 M0 후, Hot Reload는 M1과 개발 호스트·실행 세션 준비 후 연결한다. package-only smoke는 M0부터 시작한다. 아래 공통 문서에서 계약을 정하고 이 문서에서 플랫폼별 연결·실측 결과를 추적한다. M3/M6의 플랫폼 적용 범위는 본문의 경계를 따른다.

- M0/M1: [실행·지원표](../common/00-foundation.md), [공통 테스트 런타임](../common/01-testing.md)
- M2: [Desktop 계약](../common/02-desktop-contract.md), [입력·접근성·PlatformView](../common/03-input-accessibility-platformview.md), [렌더링·수명](../common/04-rendering-lifetime.md)
- M3/M4: [개발 도구·Hot Reload](../common/05-vscode-hot-reload.md), [플러그인 SDK](../common/06-plugin-sdk.md)
- M5: [OS Drag & Drop](../common/07-os-drag-drop.md), [Navigation·복원](../common/08-navigation-restoration.md)
- M6/M7: [멀티윈도우](../common/09-multiwindow.md), [패키지 소비·배포](../common/10-release-packaging.md)

## 1. 실행·개발 도구 — M0/M1/M3, P0 → P1

- [x] Web Debug build/HTTP bootstrap smoke·실패 전파를 M0 실행기·CI에 연결했다. 일부 target manifest와 생성 앱의 Chrome 초기 화면·버튼/한글 표시를 확인했다. [00](../common/00-foundation.md)·[04 결과](../common/04-rendering-lifetime.md)
- [ ] 새 checkout/원격 CI 실행을 확인한다. CI 구성과 로컬 실행 PASS를 구분한다.
- [x] clean profile의 설치 VSIX에서 Web Debug Run·외부 Chrome 연결·Hot Reload·Stop을 확인했다. main-owned threaded runtime의 WebGL/WebGPU에서 실제 metadata 갱신·State/입력/스크롤 유지·컴파일 오류 재시도 PASS(2026-09-28). [05 결과](../common/05-vscode-hot-reload.md)
- [ ] 생성 마법사 전체 UI·실제 C# 서비스·snippet 편집 예외·startup 실패/종료 경합을 검증하고, 2026-09-29 창 문맥 변경 뒤 설치 VSIX의 실제 metadata 흐름을 재검증한다. 다른 브라우저·물리 모바일·worker-owned·Release/AOT로 기존 PASS를 확대하지 않는다.

## 2. 입력·접근성·DOM PlatformView — M2-B, P1

- [ ] 한글 조합·selection/caret·multiline·clipboard·단축키·focus loss와 Doroti TextField ↔ DOM/native 편집 요소/iframe의 Tab·IME 소유권을 검증한다.
- [ ] semantics 이름·역할·값·상태·action·focus·탐색 순서를 브라우저 접근성 경로와 실제 보조기술에서 각각 확인한다.
- [ ] DOM iframe/PlatformView의 복수 owner, clip/transform/z-order, gesture 경쟁, 재생성·지연 응답·focus 복구를 검증한다.

## 3. 렌더링·메모리·폰트 — M2-C, P1

- [ ] 대상 브라우저·기기·해상도별 baseline/예산을 정하고 WebGPU/WebGL 등 실제 지원 renderer의 build/layout/paint, GPU, CPU readback/upload, present 간격과 메모리를 분리 측정한다.
- [ ] PlatformView CPU upload와 frame admission/backpressure를 점검하고 긴 리스트·VariableBlur·다중 효과·WebView overlay·이미지/영상 texture를 같은 조건으로 비교한다.
- [ ] resize·DPR 변경에서도 capture 영역·pixel origin·viewport geometry를 보존하고 context/device loss·pause/resume·재생성 수명을 검증한다.
- [ ] Android/iOS 모바일 브라우저의 메모리·복귀·장기 스크롤을 실제 기기에서 검증한다. desktop 브라우저 에뮬레이션 결과와 분리한다.
- [ ] 폰트 CDN/로컬 asset/offline 정책, 초기 글꼴 교체와 한글 fallback을 실제 사용 경로에서 확인한다.

## 4. 플러그인·드롭·URL 복원 — M4/M5, P2

- [ ] FilePicker·URL launcher의 Web module, 취소·브라우저 권한/미지원 결과·파일 접근 수명을 공통 패키지 계약에 연결한다.
- [ ] 브라우저 파일/텍스트/URI drop과 MIME/action·논리 좌표·비동기 읽기를 연결하고 큰 파일·여러 파일·취소·DPR 변화를 검증한다. 송신 등 브라우저가 제공하는 범위를 capability로 구분한다.
- [x] main-owned runtime의 URL ↔ Router, history push/replace/popstate, sessionStorage·lifecycle·listener 해제를 연결했다. 공통/Node의 준비 전 대기·중복·손상 history/checkpoint·bfcache 회귀 PASS. [08 결과](../common/08-navigation-restoration.md)
- [ ] 실제 브라우저의 back/forward·새로고침·bfcache 복귀·잘못된 링크, route·입력·선택 상태 복원과 앱별 version migration/fallback을 검증한다. worker-owned framework navigation은 별도 미완료 범위다.

## 5. 배포 — M7, P3

- [x] private template hive와 격리 NuGet cache의 package-only Release publish 및 비어 있지 않은 bootstrap/wasm payload를 확인했다. trimming/AOT는 끈 조합이다. [10 결과](../common/10-release-packaging.md)
- [ ] 후속 수정이 포함된 후보로 clean 배포 환경의 실제 브라우저 기동과 SDK/runner/native/font/Web asset/플러그인 정합을 검증한다. publish/payload PASS를 화면 표시나 trimming/AOT 검증으로 확대하지 않는다.
- [ ] 배포·업데이트 후 자산과 저장 상태의 호환, 로그·오류 수집, 장기 실행·PlatformView 반복 생성·context loss 회귀를 검증한다. 설치·서명 등 해당 배포 형태에 적용되지 않는 항목은 적용 범위를 명시한다.

## 완료 기준

브라우저·renderer별 실제 앱 실행과 URL 탐색/복원, 입력·합성·메모리 결과를 기록한다. GPU 완료 시간과 실제 화면 FPS를 구분한다. Web의 Hot Reload 지원 상태를 별도로 남기며 M3 전체는 최소 한 개발 호스트의 실제 코드 갱신 통과를 요구한다. M6의 desktop 추가 창을 Web 필수 범위에 넣지 않는다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Web](../../../../Doroti/src/Doroti.Host.Web)
- [Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs](../../../../Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs)
- [Doroti/src/Doroti.Host.Web/Web/doroti.web.ts](../../../../Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)
- [Doroti/docs/platform-views/web-webview.md](../../../../Doroti/docs/platform-views/web-webview.md)
- [samples/DorotiTestbedApp/web](../../../../samples/DorotiTestbedApp/web)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 일회성 테스트·원시 로그·캡처·소비 앱은 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고 요약 후 정리한다. 제품 빌드·release 후보 등 기존 `Doroti/artifacts`는 별도의 삭제 가능한 산출물이며, 필요한 요약·최소 상시 fixture만 추적되는 tests/docs/history에 보존한다.

## 2026-09-28 갱신

현재 공통 00~03의 실행 결과는 [지원표](../../../../Doroti/docs/support-status.md)와 각 공통 작업 문서에 기록했다. revision: a93c047fe2e93d93cff3e0a6bf3c2789862fea81 + 작업 트리 변경 (미커밋).

Web Debug build 경고 0/오류 0, HTTP server/bootstrap asset smoke PASS. Chrome에서 Material Testbed 실제 화면과 semantics 트리 확인 PASS. 새 DOM PlatformView 입력/물리 IME/Release AOT·성능 검증은 수행하지 않았다.

공통 04·05 추가: 제품 resize admission의 4 in-flight + latest 1 상한/ack/reset, 모바일 backing 축소·회전 메모리 정책 회귀 PASS. 템플릿 생성 앱의 Web Debug/CLI dev 및 Chrome 실제 버튼 클릭·한글 glyph·450×800 리사이즈 후 상태 유지 PASS. 이 결과는 물리 모바일이나 offline 폰트 검증이 아니다. 후속 Web Hot Reload는 main-owned threaded Debug의 Chrome WebGL/WebGPU에서 검증했다. 설치 VSIX Run/요청-응답/컴파일 오류 재시도/Stop과 상태·한글 값·스크롤 유지가 PASS이며, Release/AOT·worker-owned runtime·물리 모바일은 notVerified다. [렌더링 결과](../common/04-rendering-lifetime.md)·[VS Code 결과](../common/05-vscode-hot-reload.md) 참조.


## 2026-09-29 후속 실행

[이번 세 플랫폼 실행 기록](../results/2026-09-29-web-windows-android.md)에
새 구현·실제 실행·후보와 미검증 경계를 기록했다. 전체 상태는 PARTIAL이다.
