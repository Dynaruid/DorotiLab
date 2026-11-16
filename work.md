# Web 자동 고성능/호환 모드: PARTIAL — 재검증·측정 완료, 성능 인수 FAIL

작성: 2026-11-12 · 이전 중간 마무리: 2026-11-16 00:30 KST · 네 항목 재검증·측정: 2026-11-16
이 문서는 구현·검증·미완료 범위와 재개 순서를 기록한다. 11월 13일까지의 기존 검증과 11월 15일 변경분의 검증을 구분하며, 최종 소스에 대한 전체 인수는 완료하지 않았다.

**PARTIAL — 요청한 종료 오류 수정, 전체 12단계 캐시 전환, 최종 정책의 12개 구성 재검증과 성능 측정을 완료했다.** 기능·종료·캐시 행렬은 PASS지만, 성능의 10% 회귀 기준과 compatible 입력 예산은 FAIL다. §6~§8은 이전 이력이며 **최신 결과는 §9**다. CI와 실기기 인수는 제외하고 제품 기본값·지원 차단은 유지한다.

| 최신 핵심 판정 | 상태 |
| --- | --- |
| Texture source/snapshot | end-of-frame 순서 수정 뒤 양 모드 실제 GPU allocation·freeze·source 교체·종료 PASS. 잘못된 async polling도 수정. |
| 핵심 runtime 행렬 | 최종 정책·종료 어댑터·active SDK inventory 게시 코드로 12/12 native·Chromium frame/owner/input/실제 Worker close·Texture PASS. 배포 hash 6개 재확인. |
| 추가 구성 | Sample2 net10/net11 mixed-AOT 양 모드·두 브라우저 UI/pixel PASS. Release non-AOT net10/net11 양 모드 PASS. |
| 캐시 전환 | 새 label에서 Debug 7단계 + Release/AOT·non-AOT 5단계, 전체 12/12 native·브라우저·실제 Worker 종료 PASS. |
| 성능 측정 | Debug·Release/AOT × 기존 threaded·변경 threaded·compatible × fresh/warm × 3회 = 최종 36개 표본. 측정 완료, 회귀 기준·compatible 입력 예산 FAIL. |
| 전체 완료 여부 | 제품 인수는 미완료. 성능 초과 원인 격리·개선과 실기기 인수가 남았다. net10/threaded 기본값·auto opt-in·net11 제품 차단 유지. |

이전 실패·수정 이력은 §6~§8, 이번 최종 검증과 남은 범위는 §9를 따른다.

## 1. 현재 상태와 완료한 작업

공개 실행 모드 DorotiWebExecutionMode(threaded, compatible, auto), v1 호환 및 v2 dual manifest를 구현했다. loader는 .NET을 가져오기 전에 실제 secure context, cross-origin isolation, 공유 메모리 가능 여부를 보고 한 모드를 고른다. 선택된 profile, build ID, runtime, heap을 검증하며 다른 variant를 다운로드하거나 두 번째 managed runtime을 만들지 않는다. threaded는 기존 main-owned shared runtime과 JS-affine render Worker를, compatible은 독립 Worker의 non-shared runtime과 WebGL2를 사용한다. 앱·Skia·GPU의 render owner는 두 모드 모두 render Worker에 둔다.

두 variant의 restore, native link, AOT, static web assets를 generation별로 분리하는 child publish와 최종 통합을 구현했다. 공통 자산의 동일성을 확인한 뒤 manifest를 발행하고, SDK HTML import map의 실제 매핑으로 fingerprint가 붙은 loader를 찾는다. Mono 10의 pthread 자산과 Mono 11의 loader-entry Worker URL을 구분하며 프로토콜은 v7이다. runtime base와 앱 base를 나누고 font/host 경로 및 Testbed texture 초기화를 공통 managed 시작 경로에 맞췄다. non-threaded trim dependency와 net11 WebCIL cache hook 문제도 수정했다.

11월 13일까지의 기존 검증에서는 시스템 SDK 10.0.403/net10 및 SDK 11.0.100/net10에서 Testbed의 양 모드 Debug와 Release/AOT native publish, 통합, Chromium의 실제 첫 WebGL frame·입력·heap·단일 runtime 검증을 통과했다. 격리 헤더 on/off, 격리 상태에서 강제 compatible, 비격리에서 강제 threaded 오류, 하위 경로 deep link를 확인했다. Sample2도 두 SDK의 net10 dual publish와 양 모드 첫 frame·입력 smoke를 통과했다. 이 기록을 최종 변경분의 PASS로 사용하지 않는다. 당시 미검증이던 Sample2 mixed-AOT의 후속 결과는 §7·§8에 구분한다.

11월 13일 기준으로 SDK 11.0.100/net11/Mono 11.0.0의 threaded Debug/AOT는 JSWebWorker.RunAsyncVoid의 Thread.Start에서 멈췄으며, 당시 실행 인수는 10/12였다. 11월 15일에는 [runtime-only 재현과 원인 기록](packages/platforms/web/tests/Mono11ThreadProbe/README.md)을 기준으로 Mono 11의 native Worker preload 경계를 수정했다. 초기 pool 16개가 native loaded receipt를 처리한 뒤 managed 시작을 허용하며, live PThread 내부를 읽거나 수정하지 않는다. 독립 probe 3회와 실제 앱의 Debug 및 Release/AOT 양 모드 첫 frame·paint owner·입력·종료가 통과했다. 이 결과는 이후 texture snapshot/종료 수정의 전체 행렬 재검증을 대신하지 않는다. 실기기·성능 인수도 미완료다. 정상 net11 Web 빌드는 DOROTIWEB050으로 차단하고 자격 검증 도구만 opt-in을 허용한다. 제품 기본값은 net10이다.

개발 서버의 격리 헤더 on/off 전달, dual-aware tunnel 검사, 샘플·템플릿의 legacy main 고정 제거, opt-in 게시와 migration 안내를 반영했다. 집중 TypeScript 계약과 publish 계약, 브라우저 fixture를 작성했다. 제품·샘플·도구의 호환 가능한 최신 stable 의존성을 정리했으며 주요 Web 조합은 SkiaSharp 4.156.0, net10 runtime 10.0.15, net11 runtime 11.0.0이다. 패키지 갱신의 별도 잔여 항목은 R7에 기록한다. 2026-11-15 사용자 지시로 CI 계획은 범위에서 제외했다.

이 결과는 headless Chromium smoke를 포함한다. SwiftShader가 허용된 환경이므로 실제 모바일 장치, 하드웨어 GPU, 전체 기능 및 성능 인수의 증거로 확대하지 않는다. 검증 원시 산출물은 삭제 가능한 Doroti/artifacts/web-qualification에 두었고, 지속 보존할 결과는 이 문서에 남긴다.

## 2. 고정한 제품 경계

| 항목 | 현재 계약 |
| --- | --- |
| 제품 기본값 | net10과 기존 threaded 동작을 보존한다. auto는 opt-in이다. |
| 차세대 지원 목표 | SDK 11 + net11 + Mono 11의 threaded/compatible 양 모드. SDK 11로 net10을 빌드한 결과와 구분한다. |
| 모드 선택 | 실제 isolation/shared-memory 조건에서만 auto → threaded. 조건 미충족은 compatible. 배포 오류나 runtime 실패는 모드 전환으로 숨기지 않는다. |
| 렌더링 | threaded의 기존 renderer policy를 유지하고 compatible은 WebGL2로 시작한다. 조용한 DPR·blur·품질 하향은 하지 않는다. |
| 런타임 소유권 | 한 세션에 managed runtime 하나. 앱/layout/paint/Skia/GPU는 render Worker가 소유한다. Mono 버전 차이는 연결·빌드 경계에 둔다. |
| 초기 미지원 | CoreCLR WASM, compatible WebGPU, Worker/OffscreenCanvas/WebGL2가 없는 브라우저, 임의의 pthread 의존 플러그인. |
| 실패 경계 | net11 threaded의 Thread.Start 차단을 해소하고 동일한 owner·입력·종료 행렬을 통과하기 전에는 net11 제품 지원이나 net11 기본 TFM을 선언하지 않는다. |

루트와 Doroti/global.json의 SDK pin은 유지한다. 시스템 C:\Program Files\dotnet의 정확한 SDK를 검증 작업별로 선택한다. 같은 dual 배포본의 두 variant는 동일한 runtime flavor·버전·toolchain으로 만든다. 실제 runtime/renderer를 자동 교체하는 fallback을 도입하지 않는다.

## 3. 남은 작업: 우선순위와 완료 기준

### R0. Mono 11 threaded 시작 수정 — 기본 실행 PASS, 제품 인수 미완료

- [x] [managed render 진입](packages/platforms/web/Doroti.Host.Web/BrowserManagedRenderThread.cs), [Worker 연결](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.managed-worker.ts), [runtime-only probe](packages/platforms/web/tests/Mono11ThreadProbe/Probe.csproj)의 호출 순서와 소유권을 코드에서 먼저 추적한다. Mono 10/11의 실제 Worker 생성, preload, Thread.Start 및 JS-affine 진입 차이를 확인하고 멈춤 원인 가설과 수정 범위를 기록한다.
- [x] 문제가 Doroti 연결 코드에 있으면 해당 경계만 수정한다. runtime/pack 자체의 재현 문제가 남으면 정확한 SDK·runtime·workload·pack 조합과 최소 재현을 유지하고 해당 프로필의 출시 차단을 지속한다. ThreadPool 우회, 동기 호출 제한 완화, 두 번째 runtime 생성으로 성공을 꾸미지 않는다.
- [x] preload 수정 후 probe의 Thread.Start 반환, managed-port handshake, 실제 paint owner 일치, 첫 frame·입력·종료를 Debug/Release AOT에서 확인했다. 기본 pool의 upstream 재현은 계속 실패하며, 제품 지원 차단은 남은 실기기·성능·최종 수정 행렬 때문에 유지한다.

완료 기준: net11 threaded의 Debug/AOT 2개 실행 통과와 owner 일치 증거가 있거나, 외부 runtime 차단 상태와 재시도 조건이 정확히 기록되어 있다. 후자의 경우 전체 제품 인수는 계속 미완료다. net10 인수 작업은 독립적으로 계속할 수 있다.

### R1. 공통 세션과 Worker 수명주기 마무리

- [x] [host](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts), [loader](packages/platforms/web/Doroti.Host.Web/Web/doroti.loader.ts), [protocol](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.protocol.ts)에서 공통 endpoint·세션 세대·종료 사유·runtime-ready·실제 first-frame 계약을 점검하고 빠진 경계만 구현한다. 앱 시작과 서비스 등록이 모드별 runtime 소유권에 따라 갈라지지 않도록 한다.
- [x] Mono 10의 threads 자산과 Mono 11의 정확한 loader-entry URL에 대한 Worker 관찰을 연결 모듈 안으로 제한한다. 다른 Worker, preload/control 메시지, 잘못된 origin·token·protocol·port, 추가 pthread 장애, 생성자 래핑 복원, 연결 timeout을 다룬다.
- [x] 취소·종료·재시작·늦은 이전 세션 응답과 texture snapshot/종료·source 전환을 검증했다. source 교체는 texture-free tree의 `endOfFrame` 뒤 등록을 해제하며, Chromium/Firefox의 양 모드에서 실제 GPU allocation·freeze·source 교체·활성 texture 종료와 잔여 자원 0을 확인했다. SDK별 최종 행렬은 R6에서 별도로 판정한다.
- [x] 입력 burst·resize·IME commit 중 down/up/cancel과 lifecycle 경계 순서를 보존한다. move/resize 합치기와 유한 queue를 유지하고 포화·오류를 드러낸다.

완료 기준: 두 모드가 같은 앱 기능과 owner 계약으로 시작·입력·종료하며, handshake 실패와 자원 누수가 무한 splash 또는 원인 불명 timeout으로 남지 않는다.

### R2. 빌드·배포·캐시 무결성

- [x] [Web targets](packages/platforms/build/Doroti.Web.targets)와 [publish validator](packages/platforms/web/Doroti.Target.Web.browser-wasm/buildTransitive/Doroti.Target.Web.browser-wasm.targets)를 기준으로 threaded → compatible → threaded, SDK 10 → 11 → 10, 실제 net10 → net11 → net10 및 Release AOT → non-AOT → AOT의 동일 경로 전환을 확인했다. 새 label의 전체 12단계 native·frame/owner/input·요청/Texture 정리·실제 Worker 종료가 PASS다.
- [x] 이미 통과한 source checkout·NuGet provider의 publish validator를 기준으로, 한쪽 child 실패, 다른 build ID, 공통 자산 충돌, 필수 파일 누락, 오래된 fingerprint 파일이 최종 배포본을 정상처럼 통과하지 않는지 추가 확인한다.
- [x] 루트·하위 경로·deep link, 캐시된 이전 HTML과 새 배포의 조합, 압축 응답과 .mjs/.wasm MIME, 동시 페이지 접근을 검사한다. 선택하지 않은 variant의 fetch/preload/Worker 생성을 막는다.
- [ ] net11의 실제 Emscripten 도구 버전과 Skia archive/interop ABI를 산출물로 확인한다. 설치된 pack 이름의 6.0.2와 실제 도구 6.0.3을 구분한다. Sample2 mixed-AOT 및 AOT를 끈 Release도 별도 영향 범위에서 검증한다.

완료 기준: 같은 generation의 완결된 두 variant만 게시되며, SDK·모드 전환과 정적 호스팅에서 잘못된 자산 조합이 조기 진단된다.

### R3. 개발 도구·샘플·템플릿·배포 안내

- [x] Testbed, Sample2, Demo, Carousel과 새 템플릿에서 실행 모드와 WasmEnableThreads 설정의 충돌을 검사한다. auto 기본값 승격은 아래 기능·성능 인수 후에 한다. 기존 앱의 opt-in·되돌림 절차를 유지한다.
- [x] run/watch 모드 변경 시 rebuild·서버 재시작·page reload가 필요함을 문서화했다. compatible Hot Reload는 미검증으로 명시하며 동작한다고 표기하지 않는다.
- [x] [tunnel](tools/doroti-cloudflared/tunnel.ps1)과 [Nginx](tools/doroti-cloudflared/nginx.conf)의 root framework, pthread 필수 검사, runtime 경로·404·MIME을 dual 배포에 맞춘다. 격리된 기존 배포도 유지한다.
- [x] [Web README](packages/platforms/web/README.md), [제품 README](Doroti/README.md), [한국어 README](Doroti/README.ko.md), 샘플 가이드와 target manifest를 실제 지원 상태에 맞춘다. SDK 11/net10과 net11/Mono 11, 실제 미지원 범위를 분명히 구분한다.

완료 기준: 별도 JS 수정이나 배포 폴더 수작업 없이 auto 배포본을 만들 수 있고, 헤더를 설정할 수 없는 HTTPS 정적 호스팅에서는 compatible이 선택된다.

### R4. 기능·브라우저·실기기 인수

- [ ] Windows Chromium과 Firefox, Android Chrome, 실제 iOS/iPadOS Safari에서 격리 on/off의 모드 선택, 첫 frame, 입력·스크롤·종료를 확인한다. 실제 iOS AOT는 Safari 기기에서 확인한다.
- [ ] Sample2의 깊은 widget tree, 반복 blur/scroll, 이미지 decode 중 입력, 화면 복귀·resize, 한글 IME, 접근성, font/image, timers, 파일 선택·취소, clipboard, navigation, texture/effect 및 plugin 호출을 양 모드에서 검증한다.
- [ ] Worker/context loss 이후 pending 요청·GPU lease·외부 자원이 정리되는지 확인한다. CSP·Worker·OffscreenCanvas·WebGL2 미지원은 설명 가능한 오류로 표시한다.
- [x] Chromium 자동화의 SwiftShader 사용 가능성과 실제 기기·하드웨어 GPU 결과를 별도로 기록한다. 기능별 PASS, 실패, 미검증을 섞지 않는다.

완료 기준: 필수 기능이 두 모드에서 동등하게 동작하고, 지원하지 않는 브라우저·권한·플러그인 경계가 문서화된다.

### R5. 성능 기준선과 회귀 판정 — 측정 완료, 성능 인수 FAIL

- [x] SDK 10.0.403/net10.0/Mono 10.0.15/Emscripten 3.1.56, 같은 앱·viewport·WebGL2에서 기존 threaded·변경 threaded·compatible을 Debug 및 Release/AOT 각각 fresh/reload 3회 비교했다. SDK/renderer 효과를 섞지 않았다.
- [x] 실제 첫 frame, 입력→canvas commit, paint·commit 간격 p50/p95, managed/WASM 메모리와 main/Worker를 합친 실제 전송량·배포 크기를 기록했다. 새 publish 소요 시간은 기록했으나, 과거와 같은 fresh/cache 조건의 시간은 없어 속도 비교에서 제외했다. warm은 HTTP no-store 상태의 같은 페이지 reload이며 OS cold/실제 scanout 측정이 아니다.
- [x] 10% 기준과 사전 compatible 예산으로 판정하고 입력 queue/dispatch·paint·commit 경계를 분석했다. **성능 인수 FAIL**이며 단일 변경 원인은 아직 격리하지 못했다. threshold를 완화하거나 원인을 종료 어댑터로 추정해 통과시키지 않았다. 상세 수치·한계·다음 분석 범위는 §9에 기록했다.

완료 기준: 고성능 경로의 회귀 여부와 compatible의 실제 성능 차이·미지원 범위가 재현 가능한 조건과 함께 기록된다.

### R6. 최종 지원·출시 판정

- [x] SDK 10/net10, SDK 11/net10, SDK 11/net11 Mono의 양 모드 × Debug/non-AOT·Release/AOT를 최종 linker/native cache 정책·종료 어댑터로 재게시했다. **12/12 PASS**이며 실제 frame/owner/input/Worker close·Texture 정리와 6개 dual 배포의 전체 hash를 확인했다. 별도 동일 cache 전환도 12/12 PASS다.
- [x] native 빌드, 실제 browser first-frame, 기능, 종료·자원 정리, 실기기, 성능 결과를 각각 PASS/FAIL/미검증으로 기록한다. net10 지원 배포와 net11 차세대 지원의 판정을 분리한다.
- [x] 인수 미완료이므로 기존 net10/threaded 기본값과 auto opt-in을 유지했다. auto·net11 기본값 승격은 각각 별도 출시 결정으로 남기며, 회귀 시 이전 완결 배포본으로 되돌린다.

완료 기준: 지원 표기·템플릿 기본값·마이그레이션 안내가 증거와 일치하고, 미검증 또는 차단 프로필이 제품 지원으로 노출되지 않는다.

### R7. 패키지 갱신의 별도 잔여 정리

- [x] Editor helper의 기본 소스 glob에서 tests/**를 제외했다. tests/obj의 생성 assembly attribute 중복을 해소했으며, 일반 Debug/Release 빌드 모두 경고 0·오류 0으로 통과했다.
- [x] Dart stub의 존재하지 않는 upstream screenshot 선언을 제거하고 publish_to:none을 명시했다. 전체 dart analyze는 0 issues다. npm의 minimatch 10.2.6 전이 경고는 @vscode/vsce 범위에서 11.0.1로 제한 갱신했다. Node/API floor를 확인했고 npm audit 취약점 0, TypeScript·helper publish·실제 VSIX package까지 통과했다. 라이선스는 유지했다.

## 4. 검증 운영 규칙

문제 해결은 먼저 코드와 빌드 계약을 분석해 원인 가설·수정안을 만든 뒤, 그 가설을 확인하는 최소 실행 검증으로 진행한다. 모든 테스트 프로세스에는 외부 기준 20분(1,200초) timeout을 둔다. 일반 반복은 시나리오당 3~5회, 추가 검증도 대부분 30회 이내로 제한한다. 관련 구현을 묶어 수정하고 국소 검증을 통과한 뒤 통합 인수를 수행한다.

빌드 성공, native link, managed runtime 시작, 실제 첫 frame, 기능, 성능, 실기기 결과를 분리해 보고한다. raw 로그와 배포본은 삭제 가능한 Doroti/artifacts에 둘 수 있으나, 보존할 판정·재현 조건·차단 이유는 추적 문서에 남긴다. 산출물을 삭제한 뒤 원시 파일이 남아 있다고 보고하지 않는다.

## 5. 2026-11-15 확인한 구현·검증

CI 계획은 사용자 지시로 범위에서 제외했다. 사용자는 연결할 실기기가 없다고 확인했다. Android 에뮬레이터를 실기기로 세지 않으며, Android Chrome·iOS/iPadOS Safari의 물리 장치 인수는 미검증으로 유지한다.

### 구현한 경계

- 시작 완료는 같은 세션의 runtime-ready와 실제 admitted first canvas frame을 모두 요구한다. 누락된 준비/프레임은 원인과 함께 120초 안에 실패한다.
- 모드 공통 application environment를 전달하며, Router가 query를 지운 뒤에도 Worker 교체 시 시작 옵션·진단 설정을 유지한다. protocol v7을 사용한다.
- managed-port의 origin/token/protocol/port를 검증하고, 추가 pthread 장애·시작 취소·늦은 이전 세션 응답을 처리한다. runtime-owned pthread는 직접 terminate하지 않고 역할 종료를 요청한다.
- 종료 시 요청·입력 queue·texture/frame lease·platform DOM을 정리한다. 실제 disposed receipt 뒤 DOM owner를 닫으며, 15초 안에 완료를 확인하지 못하면 resources-unconfirmed로 남긴다.
- Ganesh/Graphite paint에서 실제 managed render owner를 검사한다. 시작 thread ID만 확인한 결과와 구분한다.
- 게시본은 양 child와 공통 자산을 검증한 새 staging tree로 만든 뒤 교체한다. 누락·혼합 generation·다른 provenance·오래된 fingerprint·잘못된 loader 매핑을 거부하며, 이전 완결 output을 보존한다. HTML/manifest 세대가 다르면 .NET import 전에 DOROTIWEB055로 실패한다.
- Testbed input scene의 WebView payload를 provider가 요구하는 원문 HTML로 수정했다. 브라우저의 native-editor factory 부재는 별도의 명시적 미지원이며 WebView 성공으로 덮지 않는다.

### 확인한 국소·통합 결과

| 검증 | 판정과 범위 |
| --- | --- |
| TypeScript production compile + 계약 | 23/23 PASS. preload 순서, 양방향 readiness, 세대/취소, 지연 자원 회수, endpoint dispose, 잘못된 protocol, pointer/IME/cancel 순서 포함. |
| 실제 C# publish task 계약 | 17/17 PASS. child/최종 profile 손상, 공통 충돌, 필수 파일·정확한 loader 누락, 오래된 파일 제거와 기존 output 보존 포함. |
| NuGet provider에 포함한 validator | 2/2 PASS. 패키지에서 추출한 실제 task/targets로 완결 배포 승인·다른 generation 거부. 새 템플릿 전체 runtime publish의 증거는 아니다. |
| 샘플 4개 + 새 템플릿 | 모드 3개 속성 평가와 raw thread override 충돌 거부, 20/20 PASS. 전체 sample/template browser 인수와 구분한다. |
| 개발 서버 | 격리 헤더 on/off, configured header override 및 누락 runtime 404 PASS. |
| 정적 HTTP fixture | 실제 격리/비격리 두 origin의 root/subpath/deep link, 동시 module 응답, gzip/Brotli WASM magic·MIME·404 PASS. |
| Testbed Debug — 마지막 texture 변경 전 검증본 | SDK 10/net10, SDK 11/net10, SDK 11/net11 모두 양 모드 native publish + Chromium 실제 첫 frame·단일 runtime·heap·paint owner·입력·종료 PASS, 6개 구성. |
| Testbed Release/AOT — 마지막 texture 변경 전 검증본 | SDK 10/net10 및 SDK 11/net11의 양 모드 native publish + Chromium 실제 첫 frame·heap·paint owner·입력·종료 PASS, 4개 구성. SDK 11/net10의 이번 재게시 작업은 사용자 중단 요청으로 완료하지 않았다. |
| Firefox — 마지막 texture 변경 전 | headless Firefox 159.0에서 양 모드 첫 frame·owner·입력·restart·종료 및 부정 시나리오 PASS. 최종 소스와 Release는 미검증. |
| 추가 기능 — 마지막 texture 변경 전 | Chromium의 양 모드 동시 페이지·Back navigation·한글 DOM text commit·iframe 교체/제거·파일 선택/읽기/취소·자원 종료 PASS. compatible은 context 복구 후 새 Worker/canvas, threaded는 context 오류 진단·owner dispose 후 명시적 page reload를 확인했다. 8개 시나리오 PASS. |
| Texture 전용 최종 완료 여부 | 미완료. 20:57:44 테스트는 threaded에서 FAIL했고, 그 뒤 source 전환 수정은 threaded 게시만 통과했다. 최종 양 모드 실행 PASS는 없다. |

위 브라우저 결과는 Chromium 158.0.8127.18/Firefox 159.0의 headless·소프트웨어 GPU 허용 환경이다. 합성 DOM 한글 commit을 물리 한글 IME 조합 인수로, semantics 노출을 실제 screen reader 인수로 확대하지 않는다. native-editor, camera/video/WebCodecs, 실제 hardware GPU 및 장시간 모바일/Safari 동작은 별도 범위다.

검증 중 소스가 바뀐 중간 게시본은 공통 자산 충돌/SDK import-map 중복으로 차단됐고, 소스를 고정한 재게시에서 SDK 11/net10·net11 Debug가 통과했다. 중간 실패를 완결 배포로 세지 않는다.

## 6. 이전 중간 마무리와 재개 지점 — 21:47 KST 이력

**상태: PAUSED / PARTIAL.** SDK 11/net10 Release/AOT와 in-place SDK/mode 전환의 소유 process tree는 중단했다. 마지막 SDK 10/net10 Debug 재게시도 compatible 단계에서 완료되지 않았으며 현재 해당 프로세스는 없다. 예정했던 Sample2 mixed-AOT와 Release non-AOT는 시작하지 않았다. 이번 정리에서는 새 구현·게시·기능 테스트를 실행하지 않고 현재 파일·로그·프로세스 상태를 확인했다. 커밋·push·외부 배포는 수행하지 않았다.

### 마지막 수정

실제 Texture 장면에서 `ISkiaExternalTextureSource.CaptureFrame`의 기본 예외로 scene 제출이 실패하는 것을 확인했다. Web adapter에 불변 GPU image snapshot과 참조 수명주기를 추가했다. Web의 app submission과 raster가 같은 JS owner에서 실행됨을 확인하며, source 교체·등록 해제 뒤에도 frozen scene의 snapshot이 살아 있도록 한다. 마지막 참조를 놓으면 기존 GPU completion fence를 거쳐 Skia image/backend wrapper를 반환한다. GPU effect의 동일 image wrapper 호출부도 맞췄다.

종료 시 renderer가 frozen scene을 해제하면서 뒤늦게 반환하는 texture lease를 한 번 더 drain하고, live/registration/pending/completion이 0인지 검사한 뒤에만 disposed로 처리하도록 구현했다. 실패하면 resources-unconfirmed 경계를 유지한다. 해당 TypeScript 변경의 production compile와 계약 23/23은 통과했다.

그 뒤 Texture 전용 검사에서 해제한 ID 재제출 오류를 확인했다. `TextureSample.SelectSource`가 source 교체·종료 요청 전에 `_externalId`를 비우고 기존 managed entry를 분리하도록 20:58:59에 수정했다. 이 후속 수정의 효과는 브라우저에서 확인하지 못했으므로 해결 완료로 판정하지 않는다.

### 마지막 검증 결과와 근거

| 단계 | 최종 판정 | 확인한 근거 |
| --- | --- | --- |
| TypeScript compile·계약 | PASS, 23/23 | `remaining-ts-contracts.log`. 마지막 후속 변경은 C# sample에만 있음. |
| Texture 전용 실행 | FAIL — 후속 수정 전 | `remaining-browser-textures.log`, 20:57:44. threaded에서 `Texture submission belongs to a retired or different view registry.` 발생. compatible은 도달하지 못함. |
| 후속 수정의 threaded 게시 | PASS — 빌드/게시 한정 | `DorotiTestbedApp-10.0.403-net10.0-Debug-auto-remaining/threaded.log`, 21:00:32 완료. 브라우저 PASS를 뜻하지 않음. |
| 후속 수정의 compatible 게시 | 미완료 / PAUSED | receipt가 compatible 단계에서 남았으나 완료 프로세스·새 완료 로그는 없음. 기존 compatible 로그는 20:55:26으로 후속 수정 전임. |
| 후속 수정의 dual 통합 | 미실행 | 기존 `finalize.log`는 20:55:44. 현재 통합 배포 폴더는 20:58:59 수정 결과가 아님. |
| 후속 수정 뒤 Texture 브라우저 검사 | 미실행 | 마지막 브라우저 로그가 코드 수정 시각보다 앞섬. 이전 실패를 새 코드의 FAIL로도, 후속 수정을 PASS로도 바꾸지 않음. |
| 프로세스 정리 | 확인 완료 | 21:47 KST 프로세스 목록에서 세션의 qualify-publish/switch-cache/browser-textures/native optimizer 실행 없음. |
| 마무리 파일 검사 | PASS | `git diff --check` 통과. 두 `global.json`의 변경 없음. 새 빌드·기능 테스트는 실행하지 않음. |

중단된 SDK 10/net10 Debug의 `qualification.json`은 `paused`로 바로잡았고, 중단된 compatible 단계·마지막 threaded 게시 완료·수정 전 Texture 실패·최종 코드 실행 미검증을 기록했다. 마지막 source 전환 수정, 실패 로그와 중단된 게시 단계는 보존했다. `browser-sample2.mjs`와 `measure-execution.mjs`는 문법 검사만 통과했으며 실제 기능·성능 검사는 실행하지 않았다.

### 재개 순서 — 명시적 재개 요청 후에만 실행

1. 마지막 texture 수정부터 SDK 10/net10 Debug의 **새 generation**으로 양 모드를 게시하고 Texture 전용 검사를 실행한다. 현재 남은 통합 폴더를 최종 코드로 간주하지 않는다. retired registry 오류의 재발 여부, canvas/ImageBitmap·freeze/resume·source 전환·활성 texture가 있는 종료와 잔여 자원 0을 확인한다. 검사 버튼은 semantics action으로 활성화하므로 물리 pointer 인수와 구분한다.
2. 최종 소스로 Testbed의 핵심 12개 구성 전체를 재검증한다. Mono 11의 초기 pool/default 재현·preload 후보를 구분하고, owner/first-frame/input/disposed 및 texture 잔여 0을 확인한다.
3. `switch-cache.ps1`의 동일 출력 경로에서 threaded→compatible→threaded, SDK 10→11→10의 5단계를 처음부터 완결한다. 이번 중단은 전환 검증 PASS가 아니다.
4. Sample2 mixed-AOT 및 Release non-AOT를 빌드하고 실행한다. `browser-sample2.mjs`는 작성했으나 실행하지 않았다. 깊은 Cupertino tree, font 선택, blur/scroll/slider/resize, 파일/image decode와 화면 복귀의 실제 결과를 기록한다.
5. `measure-execution.mjs`는 아직 실행하지 않았다. 다른 native 빌드가 모두 끝난 동일 조건에서 before/after threaded 및 compatible을 각 3회 fresh/warm으로 비교한다. legacy provenance의 누락 pack 정보는 unknown으로 남기고, 이전/새 publish 시간은 동일한 cold/cached 조건을 확보하기 전에는 비교하지 않는다. 10% 회귀 기준의 판정도 미수행이다.
6. 실제 Android Chrome·iOS/iPadOS Safari, 물리 한글 IME·screen reader·hardware GPU·장시간 사용을 인수한다. 현재 사용자는 실기기가 없다고 확인했으며 에뮬레이터/headless 결과로 대체하지 않는다. CI는 제외 범위를 유지한다. auto/net11 기본값은 승격하지 않는다.

### 산출물·재현 기준

검증 helper는 `packages/platforms/web/tests`에 있고 실제 테스트/빌드 child에는 `run-validation.ps1`의 외부 1,200초 제한을 사용한다. 원시 로그·배포본·qualification receipt는 삭제 가능한 `Doroti/artifacts/web-qualification` 아래에 있다. SDK 10/net10 Debug generation `96a469746cc545d7a24ba089a6a7ca81`은 재게시가 중단되어 child와 통합 폴더의 시점이 서로 다르다. SDK 11/net10 Debug `685c0cf4b1944136b313fd03ebdb53e5`, SDK 11/net11 Debug `20aa411d6cb14a29a462cd389d3fc2c8`, SDK 10/net10 AOT `0837a0bf23014bb480cdd683e3d254f8`, SDK 11/net11 AOT `173f253ce1354bf594fa24cbbedfc025`는 마지막 texture 변경 전 검증본이다. SDK 11/net10 AOT `9da942b84a2c42d7a5f448fa4f273aec`의 이번 재검증도 중단했다. 시스템 SDK를 명시했고 저장소의 두 global.json pin은 변경하지 않았다.

명시적으로 재개한 뒤 첫 검증에 사용할 명령은 다음과 같다. generation을 재사용하지 않고 기존 중단 산출물과 분리한다.

```powershell
& ./packages/platforms/web/tests/qualify-publish.ps1 -Sdk 10.0.403 -Framework net10.0 -Configuration Debug -App DorotiTestbedApp -Label resume-texture
if ($LASTEXITCODE -ne 0) { throw '양 모드 게시가 실패했습니다. 브라우저 검사를 중단합니다.' }
& ./packages/platforms/web/tests/run-validation.ps1 -Executable (Get-Command node).Source -Arguments @('packages/platforms/web/tests/browser-textures.mjs', 'Doroti/artifacts/web-qualification/DorotiTestbedApp-10.0.403-net10.0-Debug-auto-resume-texture/publish/wwwroot') -LogPath 'Doroti/artifacts/web-qualification/resume-textures.log'
```

첫 명령이 실패하면 두 번째 명령을 실행하지 않는다. 위 명령은 재개 안내이며 중간 마무리 정리 중에는 실행하지 않았다.

## 7. 2026-11-15 21:57 KST 재개 중의 구현·검증 이력

21:50의 첫 재개에서는 샌드박스가 PowerShell 실행과 .NET Git 메타데이터 작업의 `C:\Users\parti\.gitconfig` 접근을 거부했다. 이 환경 실패를 제품 테스트 실패나 최종 소스 PASS로 세지 않았다. 21:57 환경 제한 변경과 사용자 재개 요청 뒤 정상 PowerShell 7.7.0 및 시스템 SDK로 새 generation 검증을 시작했다. 기존 SDK pin과 제품 기본값은 바꾸지 않았다.

- 최종 소스의 production TypeScript 컴파일과 실행·수명주기 계약 **23/23 PASS**. 각 테스트 child의 외부 제한은 1,200초다.
- `final-texture` generation `aaf16bea66084e23be0ad97cb6da3741`의 첫 게시 시도는 앞선 제한 환경에서 exit 1로 끝났고 완결 게시본을 만들지 못했다. 이 generation을 브라우저 인수에 사용하지 않는다.
- 제한 해제 뒤 첫 SDK 10/net10 Debug generation `2d4e3cf21257497c9244355ad8b90a4a`는 양 모드 게시·핵심 Chromium smoke를 통과했지만, Texture 검사에서 retired ID 오류를 다시 재현했다. §6의 `_externalId` 선제 초기화만으로는 해결되지 않았다.

### 재현한 원인과 수정

`setState`는 tree 제거를 예약할 뿐 즉시 paint/layer를 교체하지 않는다. 기존 코드는 그 사이 managed pixel entry 또는 JS 외부 source를 해제해, 아직 유효한 이전 layer를 새로운 scene에 제출하면서 strict registry 오류를 냈다. `TextureSample.SelectSource`는 이전 pixel entry를 `using`으로 유지하고 texture-free tree의 `WidgetsBinding.instance.endOfFrame`을 기다린 뒤에만 plugin source를 바꾼다. 등록 해제 오류를 무시하거나 registry 검증을 완화하지 않았다. 수정 전 게시본을 사용하는 negative control은 원래 retired-ID 오류로 실패했고, 수정 후 새 generation은 아래 결과를 통과했다.

검증 fixture에서도 두 경계를 바로잡았다. Playwright의 `waitForFunction(async ...)`는 Promise 자체를 truthy로 받아 완료할 수 있어, 기존 Texture/복구 검사의 일부 대기가 실제 조건을 확인하지 못했다. 이를 `expect.poll`과 awaited `page.evaluate`로 바꾸고, source 버튼의 실제 busy→ready DOM 전이를 확인한 뒤 다음 작업을 수행한다. 이전 fixture의 PASS를 이 새 조건의 PASS로 재사용하지 않는다. Freeze 중 producer sequence가 증가해도 import 수는 증가하지 않으며, active GPU allocation을 관찰한 같은 snapshot을 결과에 기록한다.

### 마지막 incremental build-policy 수정 전 runtime 소스 검증

| 구성 — 양 모드 | 새 generation | native + Chromium frame/owner/input/종료 + Texture |
| --- | --- | --- |
| SDK 10/net10 Debug | `34584db179ac4ae2affed8cf900cb6a0` | PASS. Firefox의 동일 핵심·Texture도 PASS. |
| SDK 10/net10 Release/AOT | `c70399f2be98495ba5bddc51d8773abd` | PASS. Firefox의 동일 핵심·Texture도 PASS. |
| SDK 11/net10 Debug | `2d0db3b869b240b686becf82f54e62fa` | PASS. |
| SDK 11/net11 Debug | `2e9d8c5638614119b8e6217de656c3e5` | PASS. Firefox의 동일 핵심·Texture도 PASS. |
| SDK 11/net10 Release/AOT | `8f68fc85ff7c441caef6e31d43cfe2a2` | PASS. |
| SDK 11/net11 Release/AOT | `90e47f8154f7460ba8f36b107fe7a535` | PASS. Firefox의 동일 핵심·Texture도 PASS. |

이 시점의 runtime 행렬은 **12/12 PASS**다. 모든 구성의 Texture 검사는 canvas/ImageBitmap, freeze/resume, resize, source 교체·Stop, 활성 source가 있는 종료 뒤 live/registration/GPU bytes/pending/completion 및 main source bytes 0을 확인했다. SDK 11/net10에 대한 별도 Firefox 행렬은 실행하지 않았으며, Firefox 결과는 표에 명시한 Mono 10/11 Debug/AOT에 한정한다. 마지막 build-policy 수정까지 포함한 재검증 완료를 뜻하지 않으며 그 범위는 §8을 따른다.

- C# 실제 publish 계약 **17/17**, 제품 지원 차단 **4/4**, 샘플 4개+생성 템플릿 속성 계약 **20/20**, 실제 NuGet validator **2/2 PASS**. 템플릿 runtime 전체 게시의 증거로 확대하지 않는다.
- 독립 Mono 11 probe를 새로 게시했다. 기본 pool은 60초 내 `Thread.Start`가 반환하지 않는 upstream 차단을 다시 재현했고, pool 16+native preload receipt admission은 fresh browser **3/3 PASS**다. 제품 기본값이나 DOROTIWEB050 차단은 유지한다.
- Sample2 SDK 10/net10 Release의 mixed-AOT 양 모드 native 게시·핵심 smoke·실제 UI 기능이 PASS다. generation은 `31b9d507ec714e689001fd1b87482c7a`이다. AOT 로그에서 Widgets/Rendering/Doroti.Runtime은 제외되고 SkiaSharp·Doroti.Skia.Rendering·web interop은 `.bc`로 컴파일됨을 확인했다. counter, 한글 DOM commit·탭 상태 보존, 실제 font 응답·선택, blur/scroll/slider/resize, text/PNG 읽기·화면 복귀, 렌더된 PNG의 red/green pixel, 파일 clear·종료를 검증했다. Firefox도 같은 UI·pixel 검사를 양 모드에서 통과했으며 picker만 아래 controlled 범위다.
- 최종 소스 Chromium의 추가 기능 **8개 시나리오 PASS**: 동시 페이지·Back, input/iframe lifecycle, file picker/read/cancel, 실제 GL loss/restore와 owner 종료·복구. 실제 개발 서버를 다시 빌드해 격리 헤더 on/off·설정 override·runtime 404를 확인했고, dual tunnel 검사는 `validate`만 실행했다. 컨테이너·터널·외부 배포를 시작하지 않았다.

### Firefox native file chooser의 별도 한계

Firefox 159 headless의 Playwright filechooser interception은 payload 주입 전에 실제 `cancel` 이벤트를 낸다. Doroti 없는 plain HTML에서도 `cancel → change(fixture.txt)` 순서를 재현했다. Doroti는 먼저 도착한 취소를 올바르게 완료하고 input을 정리하므로, 이 자동화 경로를 파일 선택 PASS로 세지 않는다. 제품에서 정당한 cancel 처리를 무시하도록 바꾸지 않았다.

명시적 `--controlled-picker` fixture는 trusted button→생성 input→managed file grant/read와 synthetic cancel을 검사한다. 이 설정의 Firefox 추가 기능 8개는 PASS지만 **native dialog 선택 인수는 미검증**이다. 기본 native fixture의 실패 기록과 controlled 결과를 구분한다. 물리 IME·screen reader·실기기·hardware GPU도 여전히 별도 인수다.

### 확인한 Mono 11 native ABI

설치 pack 이름은 Emscripten **6.0.2/11.0.0**, 실제 tools의 `emscripten-version.txt`와 게시 provenance는 **6.0.3**이다. 위 net11 Debug generation의 실제 양 child `emcc-link.rsp`에서 SkiaSharp 4.156.0의 **6.0.2/mt,simd** 및 **6.0.2/st,simd** archive와 `DorotiSkiaInterop.js`를 확인했다. native link와 실제 Skia 156 WebGL paint·external GPU image upload/retirement도 통과했다. WebGPU 또는 하드웨어 ABI 인수로 확대하지 않는다.

- MT archive SHA-256: `B151FB1C9A46DC0BA251950F5ED1B7E4F42DB5FA07B79F0558A6F675514909D2`
- ST archive SHA-256: `46D7DC6BEFD6E1ED751E3FC060D6F5FA96EB42DFF780ED119F0C23AE1F0FA49E`
- interop SHA-256: `CFED909A6175D94EF5C07E76A11A2312FE58AFD01E6E89F3E7634A16479B0DFE`

### in-place 전환에서 추가로 재현한 ILLink cache 오류

`remaining-switch-cache-final`의 첫 threaded 단계는 native/실행 PASS였지만, 두 번째 compatible은 게시 성공 뒤 `JSMarshalerArgument.AssertCurrentThreadContext`에서 중단했다. linked `System.Private.CoreLib`와 `System.Runtime.InteropServices.JavaScript` 및 변환된 WebCIL의 hash가 이전 threaded 배포와 같았다. native JS/WASM과 profile은 non-threaded였으므로, 이 결과를 compatible 또는 전환 PASS로 세지 않는다.

설치 SDK의 `_RunILLink`는 파일 timestamp 기반 `Link.semaphore`를 쓰며 feature 값 변경이나 threads root descriptor 제거를 입력으로 추적하지 않는다. WebCIL 파일만 지워서는 이미 잘못 재사용한 linked DLL이 다시 변환된다. 기존 assembly-content/execution identity를 재사용해 **`PrepareForILLink` 전에 해당 semaphore 하나만 무효화**하도록 수정했다. 같은 identity에서는 cache를 보존한다. 실제 targets를 import한 집중 계약 4개와 지원 차단 4개는 PASS다. 이 변경은 incremental build 경계이며 위 12개 구성의 C#/TypeScript runtime 소스는 바뀌지 않았다. 최종 build-policy 재게시·동일 산출물 확인과 `final-linked` 새 cache label의 12단계 검증을 진행 중이다.

이후 Sample2 net11 mixed-AOT와 Release non-AOT는 아래 범위까지 통과했다. 마지막 build-policy 재검증·12단계 cache 전환·성능 비교는 중단 시점에 미완료다. 성능 도구는 세 variant의 순서를 3회 순환하고 마지막 input의 실제 canvas commit을 기다리며, 누락 trace/latency를 PASS로 만들지 않는다. fresh/warm별 10% 초과 회귀를 별도 출력하고 이전 publish 시간·캐시 조건이 없으면 비교 불가로 남긴다. 측정 전 provisional desktop loopback/headless compatible 예산은 input→commit p95 100ms, fresh-browser first frame 10초·reload 5초로 두었다. 이는 물리 장치 SLA나 hardware GPU 인수 기준이 아니며, 실제 측정은 실행하지 않았다.

## 8. 2026-11-16 00:30 KST 중간 마무리 이력

사용자 요청으로 검증을 중단했다. `final-native-policy-rechecks-v2`의 process tree를 종료했고, 00:30 KST 조회에서 해당 controller와 이 세션의 관련 게시·browser·native 검증 프로세스가 남아 있지 않음을 확인했다. 마지막 fresh-child 대조 검사는 중단 요청 전에 이미 완료된 PASS이며, 중단 뒤 새 build/browser/성능 검사를 시작하지 않았다. 원시 산출물은 삭제하지 않았고 변경사항은 로컬에 보존했다. 커밋·push·외부 배포는 없다.

### 추가 완료 결과

| 범위 | 결과와 정확한 판정 |
| --- | --- |
| Sample2 SDK 11/net11 Release mixed-AOT | generation `af8b0f8587be47d09d37d1904add6018`. 양 모드 native·Chromium owner/frame·UI·실제 PNG pixel PASS. Firefox UI/pixel도 양 모드 PASS이며 file picker는 명시적 controlled 범위다. Widgets/Rendering/Doroti.Runtime은 해석하고 SkiaSharp·web interop은 AOT인 실제 compile 로그를 확인했다. |
| Testbed SDK 10/net10 Release non-AOT | generation `07623ffbad2649798d70747d3c60d9d4`. 양 모드 native·Chromium owner/frame/input·Texture·종료 PASS. 마지막 native-table cache 정책 추가 전 실행이다. |
| Testbed SDK 11/net11 Release non-AOT | generation `61bc811078a14c2fb03cadb6d419af66`. 마지막 cache 정책 및 인자 수정 뒤 양 모드 native·Chromium owner/frame/input·Texture·종료 PASS. |
| 최종 build-policy 핵심 재검증 | `...Debug-auto-final-recheck`, generation `34584db179ac4ae2affed8cf900cb6a0`. SDK 10/net10 Debug 양 모드 게시·핵심·Texture PASS, 주 행렬 2/12. SDK 10/net10 Release/AOT의 threaded 재게시 중 중단했으며 compatible·merge·browser 재검증은 미실행이다. |
| 최종 targets 집중 계약·패키지 | 지원 차단 4개+linker/native cache 무효화·보존 4개 PASS. 마지막 `Doroti.Web.targets`의 실제 package 포함 hash를 확인하고 packaged validator 2/2 PASS. |

`qualify-publish.ps1`의 net11+`-DisableAot`는 한 원소 배열이 문자열로 풀린 뒤 `+=`가 두 MSBuild 옵션을 이어 붙이는 도구 오류도 발견했다. `[string[]]`로 고정해 qualification opt-in과 non-AOT 인자를 분리했고, 위 net11 Release non-AOT 실제 게시·실행으로 확인했다. 제품의 DOROTIWEB050 차단을 완화하지 않았다.

### cache 전환의 잔여 실패 — 해결 완료 아님

1. linker semaphore 수정 뒤 `remaining-switch-cache-final-linked`의 **원래 5단계** threaded→compatible→threaded 및 SDK 10→11/net10→10은 native·실행 PASS다.
2. 추가한 6단계 실제 net10→net11 전환은 Mono `icall.c:6998`에서 실패했다. SDK `_GenerateManagedToNative`가 `runtime-icall-table.h`를 존재 여부만으로 재사용해, 새 runtime과 이전 internal-call table이 섞이는 것을 확인했다. 현재 정책은 identity 변경 시 build/publish의 해당 header와 `m2n_cache.txt` 네 파일만 지우며, 정책 version marker도 identity에 포함한다. linked/WebCIL·native 소스 전체 디렉터리를 무차별 삭제하지 않는다.
3. 이 정책으로 **같은 손상 cache를 수동 cleanup 없이** 재게시한 `final-icall-repair-*`에서는 실제 frame·owner·입력·disposed receipt까지 도달했고, runtime/icall/pinvoke header는 fresh Mono 11 대조본과 같아졌다. 그러나 종료 시 `worker sent an unknown command killThread` console error가 남아 **strict smoke는 FAIL**이다. 원인은 아직 확정하지 않았고 이 메시지를 필터링해 PASS로 바꾸지 않았다. `runtime.o`·`pinvoke.o`가 fresh 대조본과 다른 점도 미해결이다.
4. 별도로 기존 fresh net11 threaded child를 같은 single-mode smoke로 검사한 `final-mono11-fresh-child-shutdown.log`는 PASS다. 따라서 위 잔여 오류를 모든 Mono 11 배포에 공통인 upstream 문제로 단정하지 않는다. 마지막 정책으로 새 12단계 전체 cache 전환은 아직 시작하지 않았다.

### 중단 산출물의 의미와 재개 순서

- SDK 10/net10 Release `final-recheck/qualification.json`은 `paused`로 바로잡았다. generation `c70399f2be98495ba5bddc51d8773abd`의 child cache는 재게시 중이었으므로 이전 완결 배포와 조합하지 않는다. 기존 `-auto-final/publish/wwwroot`는 별도 보존된 이전 완결 배포다. `final-recheck` 완료 여부는 각 receipt와 실제 browser 로그를 함께 확인한다.
- 명시적 재개 후에는 먼저 재사용 cache의 net11 종료 console error를 fresh 대조본과 비교해 원인을 확정한다. 그 다음 새 label의 `switch-cache.ps1 -IncludeAot` 전체 12단계를 완결한다. 기존 5단계 PASS를 전체 12단계 PASS로 확대하지 않는다.
- 최종 build-policy 주 행렬의 나머지 10개를 재게시·재실행한다. 중단된 Release는 양 child부터 다시 확인한다. 전체 배포 hash의 이전 snapshot은 `final-core-publish-hashes-before-recheck.json`에 있으나, 재게시 후 전체 hash 비교는 미실행이다.
- 모든 native 작업이 끝난 뒤에만 Debug/Release AOT의 before/after threaded 및 compatible 성능을 각각 3회 fresh/warm으로 측정한다. **실제 성능 측정과 10% 회귀 판정은 아직 0회**이며 제시한 예산을 달성했다고 보고하지 않는다.
- 실기기 Android/iOS/iPadOS Safari, 물리 한글 IME·screen reader·hardware GPU·장시간 동작·native Firefox dialog 선택은 미인수다. CI 제외, net10/threaded 기본값 및 auto opt-in, net11 제품 차단은 유지한다.

## 9. 2026-11-16 네 항목 재검증 결과 — 기능 PASS, 성능 인수 FAIL

### 종료 오류의 원인과 수정

fresh와 재사용 게시본의 실제 native JS 및 WASM SHA-256은 각각 같았다. `runtime.o`·`pinvoke.o`의 경로 차이만으로 런타임 차이를 추정하지 않는다. 실제 설치 pack의 source map이 가리키는 [Mono 11 원본](https://github.com/dotnet/dotnet/blob/297acf930fb0f5c07fd4e51d78e521b4ad2e5d1f/src/runtime/src/mono/browser/runtime/pthreads/ui-thread.ts#L228-L249)을 확인했다. JS interop을 설치한 dirty Worker의 반환 경로가 문자열 `killThread`를 호출하지만, Emscripten 6.0.3의 생성 코드는 숫자 명령만 처리한다. 수정 전 양 게시본에 같은 종료 대기를 적용한 negative control은 모두 `disposed` 직후에는 오류가 없었고, 1.5초 뒤 같은 console error를 확인했다. 이전 fresh PASS는 실제 pthread 종료까지 기다리지 않은 제한된 결과다.

`DorotiMono11Pthreads.js`는 Mono 11.0.0 + Emscripten 6.0.3 threaded 링크에만 들어간다. Emscripten compiler의 라이브러리 입력에 누락된 명령을 추가해 native 소유권 해제 → Worker 종료 → pthread 메모리 해제 순서를 복원한다. dirty Worker를 pool에 반환하지 않고, 잘못된 pthread owner는 오류로 거부한다. 앱 host에서 live PThread를 관찰·교체하거나 설치 SDK·NuGet cache·게시 JS를 사후 수정하지 않는다. 지원하지 않는 compiler 형상은 링크 시 실패한다. 라이브러리도 NuGet provider에 포함하고 링크 의존성으로 추적한다.

`browser-smoke.mjs`는 managed `disposed` receipt와 별도로 실제 렌더 Worker의 `close` 이벤트를 기다린 뒤 console error를 판정한다. cleanup 순서·소유자 불일치·알 수 없는 명령 보존·비지원 compiler 거부 계약 4/4 PASS. 같은 재사용 cache의 generation `4aee65bd3dc84b52b5ed23217e902158` 재게시와 강화한 smoke는 실제 native/첫 frame/paint owner/입력/잔여 자원 0/Worker 종료 및 console error 없음까지 PASS다. 전체 SDK·모드·AOT 인수와 구분한다.

### 동일 cache 전환과 최종 주 행렬

TypeScript production compile 및 계약 **27/27**, 지원 차단·linker/native cache 보존/무효화 계약 **8/8 PASS**. 새 `remaining-switch-cache-mono11-retirement` label의 전체 전환은 **12/12 PASS**다. Debug 7단계의 SDK 10→11→10·실제 net10→net11→net10, Release 5단계의 threaded→compatible→threaded AOT·AOT→non-AOT→AOT를 같은 cache/output/publish 경로로 실행했다. 각 단계에서 native 게시, 실제 frame/owner/input, 요청·Texture 잔여 0과 렌더 Worker close를 확인했다. 특히 net10→net11 generation `e90252ee2a6946b98dbd3c0546a8d240`에서 이전 `icall.c:6998`·`killThread`가 재발하지 않았다.

`verified-retirement` label의 최종 주 행렬도 **12/12 PASS**다. 각 행은 양 모드 native 재게시·최종 통합, Chromium의 실제 frame/paint owner/input/Worker close, Texture의 canvas/ImageBitmap·freeze/resume·resize·source 교체·활성 source 종료와 잔여 자원 0까지 포함한다.

| SDK / TFM | 구성 | generation | 양 모드 결과 | 최종 파일 수 | 배포 tree SHA-256 앞 12자리 |
| --- | --- | --- | --- | ---: | --- |
| 10.0.403 / net10.0 | Debug | `34584db179ac4ae2affed8cf900cb6a0` | PASS | 699 | `974636b793e5` |
| 10.0.403 / net10.0 | Release/AOT | `c70399f2be98495ba5bddc51d8773abd` | PASS | 552 | `3e4710cdc567` |
| 11.0.100 / net10.0 | Debug | `2d0db3b869b240b686becf82f54e62fa` | PASS | 705 | `4636972c87af` |
| 11.0.100 / net10.0 | Release/AOT | `8f68fc85ff7c441caef6e31d43cfe2a2` | PASS | 558 | `dfd54ab07619` |
| 11.0.100 / net11.0 | Debug | `2e9d8c5638614119b8e6217de656c3e5` | PASS | 714 | `cb59033abc15` |
| 11.0.100 / net11.0 | Release/AOT | `90e47f8154f7460ba8f36b107fe7a535` | PASS | 567 | `c1902d53760e` |

net11 threaded의 Debug·Release/AOT는 Firefox에서도 강화한 실제 Worker close 검사를 별도로 통과했다. 이번 Firefox 실행을 양 모드 전체 Firefox 기능 행렬로 확대하지 않는다. 전체 SHA-256·파일 수·용량·전후 diff와 최종 36개 성능 표본은 [보존용 JSON](history/26/11/16/web-execution-qualification.json)에 남겼다. 마지막 감사에서 실제 배포 파일을 다시 hash해 6개 inventory 및 controller receipt와 모두 일치함을 확인했다.

### 주 행렬 hash 비교 중 발견한 게시 경계

첫 Debug 재게시에서 이전 fingerprint가 child에 남아 최종 배포 파일이 849개로 늘었다. `MergeWebPublish.cs`는 fresh staging을 만들지만 child `_framework` 전체를 복사했고, framework 검증은 wildcard로 아무 native/runtime/worker 파일의 존재만 확인했다. 현재 SDK 10/11의 실제 loader `/*json-start*/.../*json-end*/` boot resources를 읽어 active 의존성의 존재·경로·native 그룹을 검증하고, SDK가 참조하지 않는 fingerprint 및 `.gz`/`.br`를 최종 staging에서 제외하도록 수정했다. 안정된 지원 파일과 문화권별 satellite resource는 유지한다. 설치 pack·앱 runtime/native 소스·기존 child는 바꾸지 않는다.

최종 게시 계약은 SDK 10·11 각각 **29/29 PASS**다. active native/runtime/worker가 누락됐지만 이전 파일이 남은 경우, assembly 누락, boot metadata 부재·잘못된 native 그룹·경로 탈출은 이전 완결 배포를 보존하며 거부한다. Mono 11의 별도 pthread entry 없는 형상, 현재 assembly·satellite 보존과 이전 fingerprint·압축본 제거도 확인했다. 실제 NuGet에는 최종 tasks/targets 및 두 native JS 입력의 원본 SHA가 일치하며, 추출한 validator의 실제 완결 배포 승인·다른 generation 거부 **2/2 PASS**다.

첫 SDK 10/net10 Debug는 native 재컴파일 없이 새 게시 코드로 재통합하고 양 모드 smoke·Texture를 다시 통과했다. 최종 **699개 파일** inventory로 controller의 첫 Debug hash도 갱신했다. 후속 5개 dual 구성은 처음부터 새 publisher를 사용했다. native runtime·build-policy 코드가 바뀌지 않았으므로 완료한 12단계 실제 전환 결과를 유지하며, 변경한 게시 경계는 위 계약·실제 재통합·최종 주 행렬에서 따로 검증했다.

이전 주 행렬과 새 주 행렬은 파일별 hash까지 같지는 않다. 기존 Doroti assembly informational version의 `af604be5...`가 현재 HEAD `8da0a516...`로 바뀌어 assembly·AOT 및 loader fingerprint가 달라졌고, net11 threaded에는 native 종료 어댑터가 들어갔다. 이름이 같은 파일 중 변경된 것은 각 dual root의 manifest 하나이며, fingerprint 파일은 새 이름으로 교체됐다. net10 네 행은 참조되지 않는 이전 자산 6개씩도 제거했다. 전체 hash 차이를 캐시 손상이나 기능 코드 차이로 단정하지 않는다.

### 성능 조건과 측정 도구 보정

모든 native 빌드가 끝난 뒤 같은 Windows 10.0.26300 / Ryzen 7 8845HS / 16 logical CPU / 약 32GB에서 측정했다. SDK 10.0.403, net10.0/Mono 10.0.15, Emscripten 3.1.56, viewport 1180×760, Chromium 158.0.8127.18 headless, worker-direct-webgl과 SwiftShader 허용 조건이다. Debug와 Release/AOT를 분리하고 기존 threaded·변경 threaded·compatible 순서를 세 번 순환했다. 각 variant에서 fresh browser와 같은 페이지 reload를 한 쌍으로 실행해 **최종 36개 표본**을 얻었다. HTTP는 no-store이므로 warm을 HTTP cache hit나 OS cold 대비로 부르지 않는다. 표시 장치 scanout·물리 입력은 측정하지 않았다.

최초 Debug 기준선 `075b0399...`는 Worker의 runtime-relative host module URL이 404여서 첫 frame 전에 실패했다. 해당 배포본은 제외하고, 보존된 실제 동작 기준선 Debug `0c8f6c2e3527435d8d9cf8057677e7a5`와 Release/AOT `0837a0bf23014bb480cdd683e3d254f8`을 사용했다. 기준선 파일·runtime은 수정하지 않았다. 과거 Release manifest의 누락 pack 필드는 unknown으로 남겼다. 기록된 SDK·runtime·compiler·SIMD·AOT·interpretWidgetTree 조건은 양쪽 같음을 도구가 검증한다.

실행 전 작성됐던 측정 도구의 비공개 `module.snapshot` 호출을 공개 diagnostics object로 바꾸고, 실패한 startup의 실제 DOM·console 오류를 즉시 남기도록 했다. 첫 36개 탐색 표본에서는 page Resource Timing이 Worker 다운로드를 제외함을 발견했다. 최종 측정은 Playwright의 완료된 요청별 encoded response body + response header를 main/Worker·navigation 모두 합산하고, 선택된 native WASM 다운로드가 포함되는지 검증했다. 탐색 표본과 최종 표본을 섞지 않는다. 원시 input/paint/commit/network trace도 보관했다. 모든 최종 표본은 입력 16개가 실제 인과적 commit에 도달했고 unmatched input·trace dropped·browser error가 0이다.

### 최종 성능 결과와 판정

다음 수치는 각 fresh/warm 셀 3회 결과의 중앙값이다. 시간 단위는 ms이며 paint와 commit은 각 실행의 p95를 다시 중앙값으로 요약했다. 입력→commit도 화면 scanout 지연과 구분한다.

| 구성 / cache | variant | 첫 frame | 입력→commit p95 | paint p95 | commit 간격 p95 |
| --- | --- | ---: | ---: | ---: | ---: |
| Debug fresh | 기존 threaded | 3184.1 | 348.2 | 23.0 | 139.8 |
| Debug fresh | 변경 threaded | 3061.6 | 371.3 | 30.2 | 148.6 |
| Debug fresh | compatible | 2990.2 | 341.9 | 28.1 | 138.2 |
| Debug warm | 기존 threaded | 2409.5 | 351.4 | 26.5 | 137.3 |
| Debug warm | 변경 threaded | 2359.4 | 346.1 | 24.2 | 209.1 |
| Debug warm | compatible | 2474.4 | 350.7 | 25.3 | 216.8 |
| Release/AOT fresh | 기존 threaded | 2695.8 | 197.4 | 8.05 | 81.1 |
| Release/AOT fresh | 변경 threaded | 2636.2 | 203.6 | 9.36 | 81.2 |
| Release/AOT fresh | compatible | 2352.6 | 217.8 | 8.80 | 77.3 |
| Release/AOT warm | 기존 threaded | 1685.6 | 185.7 | 7.27 | 79.7 |
| Release/AOT warm | 변경 threaded | 1651.4 | 187.6 | 6.76 | 77.5 |
| Release/AOT warm | compatible | 1881.4 | 195.4 | 7.50 | 66.8 |

**10% 회귀 기준 FAIL:** Debug fresh paint p95 **+31.30%**, Debug warm commit 간격 p95 **+52.36%**, Release/AOT fresh paint p95 **+16.27%**다. 최종 표본의 threaded 첫 frame과 입력 p95는 모든 셀에서 10% 이내다. **compatible 예산 FAIL:** 첫 frame은 사전 예산 fresh 10초/reload 5초를 모두 만족했지만, 입력 p95 100ms는 Debug·AOT의 총 12개 compatible 표본 모두 초과했다. 표의 중앙값 범위는 Debug 341.9~350.7ms, AOT 195.4~217.8ms다. 측정 완료를 성능 통과로 바꾸지 않는다.

managed heap 중앙값은 Debug에서 기존 threaded fresh/warm **19.22/18.96MB**, 변경 threaded **18.87/18.57MB**, compatible **21.14/21.04MB**다. AOT는 각각 **15.91/15.91MB**, **15.91/15.93MB**, **16.10/16.02MB**다. WASM linear memory는 threaded **173.67MB**, compatible은 Debug **120.59MB**·AOT **100.47MB**다. 단위는 decimal MB이며 browser 전체 RSS나 GPU 메모리 인수로 확대하지 않는다. p50/p95·실제 전송량은 보존용 JSON에 모두 있다. fresh의 encoded body는 Debug 기존/변경/compatible **10.081/10.092/8.840MB**, AOT **15.292/15.317/14.028MB**다.

dual 배포 크기는 Debug **150,448,932 → 103,389,950 bytes**, AOT **275,418,258 → 200,470,925 bytes**다. 디스크의 이전 fingerprint·압축본 제거 효과를 네트워크 다운로드 개선과 혼동하지 않는다. 이번 dual publish receipt 시간은 Debug **323,754ms**, AOT **1,078,317ms**이며 모두 generation cache 재사용이다. 일치하는 과거 fresh/cache 시간 기록이 없으므로 publish 속도 회귀·향상을 판정하지 않았다.

### 회귀 경계 분석과 실제 남은 작업

코드에서 확인한 범위상 이번 종료 어댑터는 net11 링크에만 들어가므로 net10 성능 측정의 hot path에 실행되지 않는다. 기존/변경 raster Worker JS의 frame dispatch·paint 기록·입력 처리 블록도 같으며, 그 파일의 차이는 시작 옵션 전달·종료의 texture lease drain이다. publisher의 fingerprint 정리는 배포 시 실행된다. 이 사실만으로 managed renderer 또는 browser scheduling 비용까지 같다고 가정하지 않는다.

최종 trace에서 compatible의 main ingress→Worker dispatch 시작 p95 중앙값은 Debug fresh/warm **215.0/148.9ms**, AOT **145.0/133.6ms**다. 실제 입력 handler p95는 각각 **19.3/19.6ms**, **21.0/16.1ms**다. 이는 100ms 예산 초과를 paint 하나로 설명할 수 없고, Worker가 입력을 받기 전 대기와 그 뒤 framework scheduling을 분리해 분석해야 한다는 증거다. rAF callback과 입력·managed paint가 같은 Worker event loop를 쓰는 경계가 우선 후보지만, 단일 변경 원인을 확정한 결과는 아니다.

각 실행은 입력 16개에 paint 6~18개뿐이어서 이 표본의 p95는 사실상 최댓값이다. commit 간격에는 fixture의 click·wheel pacing과 frame이 없는 간격도 들어가며 paint CPU 시간이나 지속 FPS와 같지 않다. sparse tail과 탐색/최종 표본의 변동을 근거로 threshold를 완화하지 않았다. **남은 작업은 이 trace의 Worker queue/framework/paint 경계를 좁혀 성능 초과 원인을 격리·개선하고 같은 조건으로 확인하는 것**이다. 실기기 Android/iOS/iPadOS Safari, hardware GPU·물리 한글 IME·screen reader·장시간 사용·Firefox native dialog는 계속 미인수다. CI는 제외한다.

원시 로그·게시본·controller receipt와 전체 trace는 삭제 가능한 `Doroti/artifacts/web-qualification/resume-*`다. 지속 보존할 결과는 이 문서와 `history/26/11/16/web-execution-qualification.json`에 남겼다. 커밋·push·외부 배포는 하지 않았으며 SDK pin·제품 기본값·DOROTIWEB050 지원 차단은 유지한다.
