# Web 작업 계획

원문: [plan.md](../../plan.md) M0~M7 · 작업 상태: **PARTIAL** · 새 실행 검증: **범위별 PASS / 나머지 notVerified**

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

- [ ] Web build/기동 smoke와 실패 전파를 M0 실행기·CI에 연결하고 템플릿에서 생성한 앱의 초기 화면을 확인한다. 일부 target만 선언한 manifest도 공통 CLI 계약으로 처리한다.
- [ ] 로컬 VSIX에서 앱 생성 → snippet/import → `Debug` 외부 브라우저 실행 → 로그 → 중지를 검증하고 startup 실패를 진단한다.
- [x] Web의 실제 metadata update 지원 범위를 정하고 Hot Reload 버튼·State/입력/스크롤 보존·컴파일 오류 후 재시도를 검증한다. 지원하지 않는 조합은 이유와 재시작 경로를 표시한다.

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
- [ ] URL ↔ Router, back/forward/새로고침, Router 준비 전 activation, 중복 링크·잘못된 링크를 연결한다.
- [ ] 기존 Web lifecycle을 활용해 route·입력·선택 상태의 저장/복원·version migration·실패 fallback을 확인한다.

## 5. 배포 — M7, P3

- [ ] package-only 앱을 clean 환경에서 Web publish·기동하고 SDK/runner/native/font/Web asset/플러그인 버전 정합과 실제 지원 publish·trimming·AOT 조합을 확인한다.
- [ ] 배포·업데이트 후 자산과 저장 상태의 호환, 로그·오류 수집, 장기 실행·PlatformView 반복 생성·context loss 회귀를 검증한다. 설치·서명 등 해당 배포 형태에 적용되지 않는 항목은 적용 범위를 명시한다.

## 완료 기준

브라우저·renderer별 실제 앱 실행과 URL 탐색/복원, 입력·합성·메모리 결과를 기록한다. GPU 완료 시간과 실제 화면 FPS를 구분한다. Web의 Hot Reload 지원 상태를 별도로 남기며 M3 전체는 최소 한 개발 호스트의 실제 코드 갱신 통과를 요구한다. M6의 desktop 추가 창을 Web 필수 범위에 넣지 않는다.

## 착수 시 확인할 코드·문서

- [Doroti/src/Doroti.Host.Web](../../Doroti/src/Doroti.Host.Web)
- [Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs](../../Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs)
- [Doroti/src/Doroti.Host.Web/Web/doroti.web.ts](../../Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)
- [Doroti/docs/platform-views/web-webview.md](../../Doroti/docs/platform-views/web-webview.md)
- [samples/DorotiTestbedApp/web](../../samples/DorotiTestbedApp/web)

기존 문서의 과거 증거와 현재 구현을 다시 확인한다. 위 경로는 조사 시작점이며 새로운 실행 검증의 근거는 아니다.

## 검증·결과 기록

[공통 완료 규칙과 결과 형식](../README.md#결과-기록-형식)을 적용한다. 테스트는 20분 timeout을 사용하고 일반 반복 검증은 30회 이내로 설계한다. 원시 산출물은 삭제 가능한 `Doroti/artifacts`, 보존할 요약·fixture는 추적되는 tests/docs/history에 둔다.

## 2026-09-28 갱신

현재 공통 00~03의 실행 결과는 [지원표](../../Doroti/docs/support-status.md)와 각 공통 작업 문서에 기록했다. revision: a93c047fe2e93d93cff3e0a6bf3c2789862fea81 + 작업 트리 변경 (미커밋).

Web Debug build 경고 0/오류 0, HTTP server/bootstrap asset smoke PASS. Chrome에서 Material Testbed 실제 화면과 semantics 트리 확인 PASS. 새 DOM PlatformView 입력/물리 IME/Release AOT·성능 검증은 수행하지 않았다.

공통 04·05 추가: 제품 resize admission의 4 in-flight + latest 1 상한/ack/reset, 모바일 backing 축소·회전 메모리 정책 회귀 PASS. 템플릿 생성 앱의 Web Debug/CLI dev 및 Chrome 실제 버튼 클릭·한글 glyph·450×800 리사이즈 후 상태 유지 PASS. 이 결과는 물리 모바일이나 offline 폰트 검증이 아니다. 후속 Web Hot Reload는 main-owned threaded Debug의 Chrome WebGL/WebGPU에서 검증했다. 설치 VSIX Run/요청-응답/컴파일 오류 재시도/Stop과 상태·한글 값·스크롤 유지가 PASS이며, Release/AOT·worker-owned runtime·물리 모바일은 notVerified다. [렌더링 결과](../common/04-rendering-lifetime.md)·[VS Code 결과](../common/05-vscode-hot-reload.md) 참조.
