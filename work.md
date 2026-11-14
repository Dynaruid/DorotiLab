# Web 자동 고성능/호환 모드: 남은 작업 계획

작성: 2026-11-12 · 현재 상태 정리: 2026-11-14
기준: 2026-11-13까지 work.md에 기록된 구현·검증 결과. 이 문서는 남은 작업의 실행 계약이며, 과거의 체크리스트와 설계 검토는 저장소 이력에서 확인한다.

## 1. 현재 상태와 완료한 작업

공개 실행 모드 DorotiWebExecutionMode(threaded, compatible, auto), v1 호환 및 v2 dual manifest를 구현했다. loader는 .NET을 가져오기 전에 실제 secure context, cross-origin isolation, 공유 메모리 가능 여부를 보고 한 모드를 고른다. 선택된 profile, build ID, runtime, heap을 검증하며 다른 variant를 다운로드하거나 두 번째 managed runtime을 만들지 않는다. threaded는 기존 main-owned shared runtime과 JS-affine render Worker를, compatible은 독립 Worker의 non-shared runtime과 WebGL2를 사용한다. 앱·Skia·GPU의 render owner는 두 모드 모두 render Worker에 둔다.

두 variant의 restore, native link, AOT, static web assets를 generation별로 분리하는 child publish와 최종 통합을 구현했다. 공통 자산의 동일성을 확인한 뒤 manifest를 발행하고, SDK HTML import map의 실제 매핑으로 fingerprint가 붙은 loader를 찾는다. Mono 10의 pthread 자산과 Mono 11의 loader-entry Worker URL을 구분하며 프로토콜은 v6이다. runtime base와 앱 base를 나누고 font/host 경로 및 Testbed texture 초기화를 공통 managed 시작 경로에 맞췄다. non-threaded trim dependency와 net11 WebCIL cache hook 문제도 수정했다.

시스템 SDK 10.0.403/net10 및 SDK 11.0.100/net10에서 Testbed의 양 모드 Debug와 Release/AOT native publish, 통합, Chromium의 실제 첫 WebGL frame·입력·heap·단일 runtime 검증을 통과했다. 격리 헤더 on/off, 격리 상태에서 강제 compatible, 비격리에서 강제 threaded 오류, 하위 경로 deep link를 확인했다. Sample2는 두 SDK의 net10 dual publish와 양 모드 첫 frame·입력 smoke를 통과했다. 다만 Sample2의 기존 mixed-AOT Release 설정은 별도 인수가 필요하다.

SDK 11.0.100/net11/Mono 11.0.0에서도 양 모드 native link와 AOT publish를 통과했고 compatible Debug/AOT의 첫 frame·heap·하위 경로를 확인했다. threaded Debug/AOT는 실행에 실패했다. JSWebWorker.RunAsyncVoid에서 Thread.Start가 반환하지 않아 render owner와 managed port가 만들어지지 않는다. Doroti·Skia·native relink를 제외한 [runtime-only 재현](packages/platforms/web/tests/Mono11ThreadProbe/Probe.csproj)도 같은 지점에서 멈췄다. 따라서 핵심 12개 구성 중 실행 인수는 10개이며, net11 threaded 2개를 통과로 세지 않는다. 정상 net11 Web 빌드는 DOROTIWEB050으로 막고 자격 검증 전용 probe만 허용한다. 제품 기본값은 net10이다.

개발 서버의 격리 헤더 on/off 전달, dual-aware tunnel 검사, 샘플·템플릿의 legacy main 고정 제거, opt-in 게시와 migration 안내를 반영했다. 집중 TypeScript 계약 8개와 publish 계약 5개, 브라우저 fixture, 시스템 SDK CI 행렬을 작성했다. 원격 CI 성공은 아직 확인하지 않았다. 제품·샘플·도구의 호환 가능한 최신 stable 의존성을 정리했으며 주요 Web 조합은 SkiaSharp 4.156.0, net10 runtime 10.0.15, net11 runtime 11.0.0이다. 패키지 갱신의 별도 잔여 항목은 R7에 기록한다.

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

### R0. Mono 11 threaded 정지 원인 분석과 지원 판정 — 최우선 차단

- [ ] [managed render 진입](packages/platforms/web/Doroti.Host.Web/BrowserManagedRenderThread.cs), [Worker 연결](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.managed-worker.ts), [runtime-only probe](packages/platforms/web/tests/Mono11ThreadProbe/Probe.csproj)의 호출 순서와 소유권을 코드에서 먼저 추적한다. Mono 10/11의 실제 Worker 생성, preload, Thread.Start 및 JS-affine 진입 차이를 확인하고 멈춤 원인 가설과 수정 범위를 기록한다.
- [ ] 문제가 Doroti 연결 코드에 있으면 해당 경계만 수정한다. runtime/pack 자체의 재현 문제가 남으면 정확한 SDK·runtime·workload·pack 조합과 최소 재현을 유지하고 해당 프로필의 출시 차단을 지속한다. ThreadPool 우회, 동기 호출 제한 완화, 두 번째 runtime 생성으로 성공을 꾸미지 않는다.
- [ ] 수정 또는 안정 runtime/pack 변경 후 probe의 Thread.Start 반환, managed-port handshake, Skia/interop의 실제 paint thread·JS context·GPU owner·반환 경로를 차례로 확인한다. Debug와 Release/AOT에서 threaded 첫 frame·입력·종료가 모두 통과해야 net11 차단을 해제한다.

완료 기준: net11 threaded의 Debug/AOT 2개 실행 통과와 owner 일치 증거가 있거나, 외부 runtime 차단 상태와 재시도 조건이 정확히 기록되어 있다. 후자의 경우 전체 제품 인수는 계속 미완료다. net10 인수 작업은 독립적으로 계속할 수 있다.

### R1. 공통 세션과 Worker 수명주기 마무리

- [ ] [host](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts), [loader](packages/platforms/web/Doroti.Host.Web/Web/doroti.loader.ts), [protocol](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.protocol.ts)에서 공통 endpoint·세션 세대·종료 사유·runtime-ready·실제 first-frame 계약을 점검하고 빠진 경계만 구현한다. 앱 시작과 서비스 등록이 모드별 runtime 소유권에 따라 갈라지지 않도록 한다.
- [ ] Mono 10의 threads 자산과 Mono 11의 정확한 loader-entry URL에 대한 Worker 관찰을 연결 모듈 안으로 제한한다. 다른 Worker, preload/control 메시지, 잘못된 origin·token·protocol·port, 추가 pthread 장애, 생성자 래핑 복원, 연결 timeout을 다룬다.
- [ ] 취소·종료·재시작·늦은 이전 세션 응답에서 pending 요청과 GPU/texture lease를 원인과 함께 정리한다. runtime·visible canvas가 중복되지 않고 선택 모드가 재시작 시 일관적인지 확인한다.
- [ ] 입력 burst·resize·IME commit 중 down/up/cancel과 lifecycle 경계 순서를 보존한다. move/resize 합치기와 유한 queue를 유지하고 포화·오류를 드러낸다.

완료 기준: 두 모드가 같은 앱 기능과 owner 계약으로 시작·입력·종료하며, handshake 실패와 자원 누수가 무한 splash 또는 원인 불명 timeout으로 남지 않는다.

### R2. 빌드·배포·캐시 무결성

- [ ] [Web targets](packages/platforms/build/Doroti.Web.targets)와 [publish validator](packages/platforms/web/Doroti.Target.Web.browser-wasm/buildTransitive/Doroti.Target.Web.browser-wasm.targets)를 기준으로 threaded → compatible → threaded 재빌드와 SDK 10 → 11 → 10 전환의 restore/native/AOT/WebCIL/TypeScript/static-assets 캐시 격리를 확인하고 결함을 수정한다.
- [ ] 이미 통과한 source checkout·NuGet provider의 publish validator를 기준으로, 한쪽 child 실패, 다른 build ID, 공통 자산 충돌, 필수 파일 누락, 오래된 fingerprint 파일이 최종 배포본을 정상처럼 통과하지 않는지 추가 확인한다.
- [ ] 루트·하위 경로·deep link, 캐시된 이전 HTML과 새 배포의 조합, 압축 응답과 .mjs/.wasm MIME, 동시 페이지 접근을 검사한다. 선택하지 않은 variant의 fetch/preload/Worker 생성을 막는다.
- [ ] net11의 실제 Emscripten 도구 버전과 Skia archive/interop ABI를 산출물로 확인한다. 설치된 pack 이름의 6.0.2와 실제 도구 6.0.3을 구분한다. Sample2 mixed-AOT 및 AOT를 끈 Release도 별도 영향 범위에서 검증한다.
- [ ] 시스템 SDK CI 행렬을 원격에서 실행하고 실패·미실행 job을 구분해 기록한다. 로컬 workflow 존재를 원격 통과로 적지 않는다.

완료 기준: 같은 generation의 완결된 두 variant만 게시되며, SDK·모드 전환과 정적 호스팅에서 잘못된 자산 조합이 조기 진단된다.

### R3. 개발 도구·샘플·템플릿·배포 안내

- [ ] Testbed, Sample2, Demo, Carousel과 새 템플릿에서 실행 모드와 WasmEnableThreads 설정의 충돌을 검사한다. auto 기본값 승격은 아래 기능·성능 인수 후에 한다. 기존 앱의 opt-in·되돌림 절차를 유지한다.
- [ ] run/watch 모드 변경 시 필요한 재빌드와 서버 재시작을 명확히 처리한다. compatible Hot Reload가 실제 동작하는지 확인하고 미지원이면 rebuild/page reload를 안내한다.
- [ ] [tunnel](tools/doroti-cloudflared/tunnel.ps1)과 [Nginx](tools/doroti-cloudflared/nginx.conf)의 root framework, pthread 필수 검사, runtime 경로·404·MIME을 dual 배포에 맞춘다. 격리된 기존 배포도 유지한다.
- [ ] [Web README](packages/platforms/web/README.md), [제품 README](Doroti/README.md), [한국어 README](Doroti/README.ko.md), 샘플 가이드와 target manifest를 실제 지원 상태에 맞춘다. SDK 11/net10과 net11/Mono 11, 실제 미지원 범위를 분명히 구분한다.

완료 기준: 별도 JS 수정이나 배포 폴더 수작업 없이 auto 배포본을 만들 수 있고, 헤더를 설정할 수 없는 HTTPS 정적 호스팅에서는 compatible이 선택된다.

### R4. 기능·브라우저·실기기 인수

- [ ] Windows Chromium과 Firefox, Android Chrome, 실제 iOS/iPadOS Safari에서 격리 on/off의 모드 선택, 첫 frame, 입력·스크롤·종료를 확인한다. 실제 iOS AOT는 Safari 기기에서 확인한다.
- [ ] Sample2의 깊은 widget tree, 반복 blur/scroll, 이미지 decode 중 입력, 화면 복귀·resize, 한글 IME, 접근성, font/image, timers, 파일 선택·취소, clipboard, navigation, texture/effect 및 plugin 호출을 양 모드에서 검증한다.
- [ ] Worker/context loss 이후 pending 요청·GPU lease·외부 자원이 정리되는지 확인한다. CSP·Worker·OffscreenCanvas·WebGL2 미지원은 설명 가능한 오류로 표시한다.
- [ ] Chromium 자동화의 SwiftShader 사용 가능성과 실제 기기·하드웨어 GPU 결과를 별도로 기록한다. 기능별 PASS, 실패, 미검증을 섞지 않는다.

완료 기준: 필수 기능이 두 모드에서 동등하게 동작하고, 지원하지 않는 브라우저·권한·플러그인 경계가 문서화된다.

### R5. 성능 기준선과 회귀 판정

- [ ] 동일 SDK·toolchain·장치·앱·화면·renderer·AOT·캐시 조건에서 기존 threaded와 변경 후 threaded를 비교한다. 그 다음 compatible을 비교하고, SDK 10/11 또는 WebGPU/WebGL2 차이를 모드 효과와 분리한다.
- [ ] cold/warm 시작부터 실제 첫 frame, 입력→frame 지연, frame time p50/p95, managed/WASM 메모리, 전송량, publish 시간·크기를 측정한다. runtime started만으로 첫 frame을 대체하지 않는다.
- [ ] 기존 threaded 대비 주요 지연 지표가 10%를 초과해 악화되면 원인을 분석한다. compatible은 실측으로 허용 지연 목표를 정하고 기능 완료·무응답 없음은 필수로 유지한다.

완료 기준: 고성능 경로의 회귀 여부와 compatible의 실제 성능 차이·미지원 범위가 재현 가능한 조건과 함께 기록된다.

### R6. 최종 지원·출시 판정

- [ ] SDK 10/net10, SDK 11/net10, SDK 11/net11 Mono 각각의 양 모드 × Debug/non-AOT·Release/AOT, 총 12개 구성을 동일한 기준으로 판정한다. 현재 실행 10/12에서 남은 net11 threaded 2개를 통과시키기 전에는 전체 완료로 표시하지 않는다.
- [ ] native 빌드, 실제 browser first-frame, 기능, 종료·자원 정리, 실기기, 성능, 원격 CI 결과를 각각 PASS/FAIL/미검증으로 기록한다. net10 지원 배포와 net11 차세대 지원의 판정을 분리한다.
- [ ] 모두 충족한 뒤에만 새 템플릿의 auto 기본값과 net11 기본 TFM 변경을 각각 별도 출시 결정으로 다룬다. 회귀 시 이전 완결 배포본으로 되돌린다.

완료 기준: 지원 표기·템플릿 기본값·마이그레이션 안내가 증거와 일치하고, 미검증 또는 차단 프로필이 제품 지원으로 노출되지 않는다.

### R7. 패키지 갱신의 별도 잔여 정리

- [ ] Editor helper 일반 빌드가 tests/obj의 생성 assembly attribute를 소스로 포함하는 문제를 코드·프로젝트 glob에서 수정하고 일반 빌드를 재확인한다. 현재는 생성 obj를 제외한 focused 빌드만 통과했다.
- [ ] Dart 전체 분석의 기존 stub screenshot 경고 4개와 npm transitive 갱신 뒤 vsce/minimatch moderate 경고 2개를 각각 원인·해결 가능 버전·영향 범위로 판정한다. SDK/API floor가 맞지 않는 패키지를 경고 제거만을 위해 무리하게 올리지 않는다.

## 4. 검증 운영 규칙

문제 해결은 먼저 코드와 빌드 계약을 분석해 원인 가설·수정안을 만든 뒤, 그 가설을 확인하는 최소 실행 검증으로 진행한다. 모든 테스트 프로세스에는 외부 기준 20분(1,200초) timeout을 둔다. 일반 반복은 시나리오당 3~5회, 추가 검증도 대부분 30회 이내로 제한한다. 관련 구현을 묶어 수정하고 국소 검증을 통과한 뒤 통합 인수를 수행한다.

빌드 성공, native link, managed runtime 시작, 실제 첫 frame, 기능, 성능, 실기기 결과를 분리해 보고한다. raw 로그와 배포본은 삭제 가능한 Doroti/artifacts에 둘 수 있으나, 보존할 판정·재현 조건·차단 이유는 추적 문서에 남긴다. 산출물을 삭제한 뒤 원시 파일이 남아 있다고 보고하지 않는다.
