# Doroti Web 연속 스크롤·창 크기 조절 Playwright 작업 계획

- 작성일: 2026-08-29
- 앱 실행: `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform web -Configuration Release`
- 자동화 대상: Playwright Chromium / Desktop Chrome / WebGL2 / browser-wasm
- 기준 URL: `http://127.0.0.1:5088/?dorotiResizeDiagnostics=1`
- 현재 저장소 상태: Playwright validation project, Chromium/DPR2/Desktop Chrome projects, 진단 helper, 20분 제한 wrapper 구현 완료.
- 현재 판정:
  - 한글 font fallback: 사용자 확인 `PASS`
  - 외부 창 이동 시 TextField focus 해제, 복귀 후 재입력: 사용자 확인 `PASS`
  - Playwright synthetic wheel/resize/flicker: `PASS`
  - 실제 trackpad 연속 스크롤: 수정 전 사용자 `FAIL`, 수정 후 재확인 `notVerified`
  - 실제 브라우저 창 border drag: 수정 전 사용자 `FAIL`, 수정 후 재확인 `notVerified`
  - 실제 화면 raster 깜빡임: 자동 blank 표본 0, 물리적 scan-out 재확인 `notVerified`

## 2026-08-29 실행 결과

| 게이트 | 결과 | 증거 |
| --- | --- | --- |
| Release browser-wasm build | `PASS` | 경고 0, 오류 0 |
| Playwright TypeScript | `PASS` | `npm run check` |
| Playwright 전체 suite | `PASS` | hardware Chromium, DPR 2 Chromium, headed Desktop Chrome; 8 tests |
| wheel immediate dispatch | `PASS` | 1,200 sample ingress/dispatch 1:1, 같은 task의 동기 managed 반환, queue depth 최대 2 |
| wheel → exact front latency | `PASS` | 최종 60 sample p95 21.1 ms, max 29.6 ms; 33.4/100 ms gate 이내 |
| viewport/window resize | `PASS` | A→B→C final exact front, headed `Browser.setWindowBounds`, stale/failed terminal 0 |
| DPR | `PASS` | DPR 2에서 1080×720 logical → 2160×1440 physical/front generation 일치 |
| flicker automation | `PASS` | idle 60초, synthetic wheel 30초, viewport resize 30초에서 blank/clear-only 후보 0 |
| context restore | `PASS` | `WEBGL_lose_context` loss/restore 후 context generation 증가와 latest exact front 복구 |
| input/semantics smoke | `PASS` | invisible semantics DOM activation, canvas pointer, keyboard/native text endpoint, 한국어 문자열 입력 |
| FCR-7 Material/widget | `PASS` | Release runtime contract |
| resize contract v4 | `PASS` | 22/22 terminal, max queue depth 2, stale present 0 |
| 실제 trackpad/물리적 border drag/scan-out | `notVerified` | Playwright synthetic 입력·screenshot으로 대체하지 않음 |
| 실제 한글 IME 조합/스크린리더/browser zoom | `notVerified` | 자동 문자열 입력·DPR project와 실제 OS 입력/도구를 분리 |

핵심 수정은 `schedulePresenter`의 microtask 대기 제거다. 기존 경로는 exact scene이 준비된 뒤에도 managed frame callback의 나머지가 반환될 때까지 raster 시작을 약 한 refresh 늦췄고, 같은 Playwright 표본에서 wheel→front p95가 61.9 ms였다. current+latest/terminal 계약을 유지한 동기 drain으로 전환한 뒤 최종 60 sample p95 21.1 ms, max 29.6 ms로 통과했다.

## 검증 원칙

Playwright를 반복 가능한 회귀 검증의 주 진입점으로 사용한다. 서버 준비, 입력 발생, viewport/window resize, runtime trace 수집, screenshot/video/trace 보존, console/page error 판정을 하나의 실행으로 묶는다.

Playwright 증거의 한계도 명확히 유지한다.

- `page.mouse.wheel()`과 `WheelEvent`는 synthetic 입력이므로 실제 precision trackpad의 OS sample cadence와 장치 판별을 증명하지 않는다.
- `page.setViewportSize()`는 `ResizeObserver`와 exact backing-size 계약을 검증하지만 실제 Windows 창 테두리 drag의 compositor/scan-out 동작과 같지 않다.
- headed Chrome에서 CDP `Browser.setWindowBounds`를 반복하면 실제 창 resize에 가까운 자동 표본을 만들 수 있지만, 이것도 사용자의 물리적 border drag를 완전히 대체하지 않는다.
- Playwright screenshot과 canvas pixel sampling은 blank/clear-only raster 회귀를 찾는 자동 oracle로 사용하되 실제 모니터 scan-out 무결성의 최종 증거로 부르지 않는다.
- 따라서 Playwright 통과 후에도 실제 trackpad 30초, 실제 창 테두리 drag 30초, 실제 화면 깜빡임 확인을 별도 사용자 게이트로 남긴다.

## 목표와 완료 조건

| 영역 | Playwright 자동 게이트 | 최종 게이트 |
| --- | --- | --- |
| startup | Release 서버가 5088에서 준비되고 `.doroti-root`와 WebGL canvas가 나타나며 first exact front commit이 timeout 안에 발생한다. `console.error`, `pageerror`, WebGL context 생성 실패가 0이어야 한다. | 첫 화면과 한글 glyph 사용자 확인 `PASS` 유지 |
| wheel ingress | pixel/line/page `WheelEvent`와 `page.mouse.wheel()` 표본 각각이 동일 task에서 framework dispatch까지 도달한다. event별 timestamp와 delta가 보존되고 host rAF 누적이 없어야 한다. | 실제 trackpad 장치 kind와 30초 연속 왕복 scroll 체감 `PASS` |
| scroll frame | 10초 synthetic 연속 wheel 중 rendering만 display rAF 기준 latest-only로 합쳐진다. queue depth 최대 2, `failed=0`, terminal 누락 0, stale present 0이어야 한다. | scroll boundary와 실제 trackpad 관성 구간 사용자 확인 `PASS` |
| scroll latency | wheel timestamp부터 exact front commit까지 표본을 JSON으로 저장하고 p95가 60 Hz 기준 33.4 ms 이내이며 100 ms 이상 stall이 없어야 한다. | 실제 입력 체감이 느리면 수치와 무관하게 `FAIL` 유지 |
| resize metrics | viewport size sequence와 headed window-bounds sequence마다 latest logical size/DPR이 같은 `ResizeObserver` callback epoch로 publish된다. | 실제 창 테두리 drag에서 내용이 즉시 따라와야 함 |
| resize present | 중간 epoch는 latest-only로 교체할 수 있지만 시작된 epoch의 판정은 immutable해야 한다. 최종 resize 후 2 refresh interval 안에 canvas backing size가 최종 physical target과 일치하고 size-mismatch/stale present가 0이어야 한다. | maximize/restore, border drag, zoom/DPR 사용자 확인 `PASS` |
| flicker | continuous animation, wheel, resize 각각에서 주기적 screenshot/pixel probe와 presenter terminal을 수집한다. blank/clear-only 표본, `failed`, context loss가 0이어야 한다. | 실제 화면 60초 확인 또는 화면 녹화에서 깜빡임 0 |
| 회귀 | Playwright로 first content, focus 이동/복귀, ASCII 입력, 기본 keyboard/pointer/semantics smoke를 실행한다. IME는 automation 가능 범위와 실제 조합 입력을 분리한다. | 한글 IME 조합과 접근성/실제 입력은 미실행 시 `notVerified` |

## Playwright validation 구성

S0에서 아래 전용 구성을 만든다. 애플리케이션 배포물에는 Playwright 의존성을 넣지 않는다.

```text
Doroti/validation/web-playwright/
  package.json
  package-lock.json
  playwright.config.ts
  tests/
    helpers/doroti-diagnostics.ts
    startup.spec.ts
    wheel-continuity.spec.ts
    resize-continuity.spec.ts
    flicker.spec.ts
    input-regression.spec.ts
  artifacts/                 # gitignore, 실패 시 trace/video/screenshot/JSON 저장
Doroti/eng/run-web-playwright.ps1
```

구성 규칙:

- `@playwright/test` 버전을 lockfile로 고정하고 `npx playwright install chromium`으로 브라우저를 준비한다.
- 기본 project는 bundled Chromium headless, 연속 창 resize project는 설치된 Desktop Chrome `channel: "chrome"`, `headless: false`로 분리한다.
- GPU 경로가 필요한 테스트에는 software renderer로 조용히 fallback하지 않도록 WebGL2/GPU identity를 시작 시 assert한다.
- 공유 포트와 GPU trace가 섞이지 않도록 `workers: 1`, `fullyParallel: false`를 사용한다.
- repository 지침에 따라 Playwright의 test timeout과 PowerShell wrapper timeout을 모두 20분으로 둔다.
- `trace: "retain-on-failure"`, `video: "retain-on-failure"`, `screenshot: "only-on-failure"`를 기본값으로 하고, continuity test의 진단 JSON은 성공 여부와 관계없이 attach한다.
- wrapper가 Release build, 5088 서버 시작, readiness 확인, Playwright 실행, 서버 process tree 정리를 소유한다. 기존 5088 프로세스를 임의 재사용하지 않고 포트 충돌이면 명시적으로 실패한다.

## 테스트용 진단 계약

현재 `__dorotiResizeDiagnostics`와 `data-doroti-resize-diagnostics`를 확장해 Playwright가 추측 없이 판정할 수 있는 읽기 전용 계약을 만든다.

- `snapshot(hostId)`: logical/physical size, DPR, generation, surface generation, GPU, focus, visibility
- `presenter(canvasId)`: current/latest request, queue depth, context generation/loss, front/staging generation
- `trace(hostId)`: wheel ingress, framework dispatch, schedule/admit, scene, raster, exact front commit, resize observed/metrics/terminal
- `reset(hostId)`: 각 테스트 시작 시 trace와 counter만 초기화하고 runtime state는 변경하지 않음
- 모든 trace entry에 monotonic timestamp, input/request/resize generation, terminal, queue depth를 포함한다.
- 진단 query가 없는 일반 실행에서는 DOM JSON publish와 상세 trace 비용을 비활성화한다.
- 테스트는 private managed object나 Skia 내부를 직접 호출하지 않고 공개된 diagnostic snapshot만 읽는다.

## 확인된 구조 차이와 작업 가설

1. Flutter Web은 wheel event를 개별적으로 즉시 framework에 전달하고 `scheduleFrame` 중복만 rAF에서 제거한다. Doroti의 기존 wheel delta 누적은 고해상도 sample cadence와 timestamp를 잃는다.
2. Flutter Web은 host element의 `ResizeObserver`에서 physical size를 계산해 `onMetricsChanged`를 즉시 호출하고 canvas backing resize는 실제 frame render 직전에 수행한다.
3. Doroti resize observer가 exact front 준비 전에 default framebuffer를 resize/blit하면 asynchronous managed raster와 WebGL state ownership이 충돌할 수 있다.
4. 5088 runtime baseline 한 표본은 약 69.3초 동안 `present-requested=1039`, `submitted=486`, `superseded/ack=553`이었다. exact front 제출이 약 7 Hz여서 wheel/resize보다 presenter 왕복과 backpressure가 먼저 병목일 가능성이 크다.
5. Playwright는 위 가설을 동일한 입력 sequence와 machine-readable trace로 A/B 비교하는 도구다. synthetic 테스트가 실제 hardware 증거보다 우선하지는 않는다.

## 실행 순서

### S0. Playwright harness와 baseline 고정 — `PASS`

- `git status --short`, `git diff --check`, 영역별 diff를 확인해 기존 사용자 변경을 보호한다.
- 전용 validation package, config, PowerShell wrapper, artifact ignore 규칙을 추가한다.
- wrapper가 현재 source fingerprint로 Release build하고 5088 서버를 기동한 뒤 `/`의 HTTP 준비와 Doroti canvas first commit을 각각 기다리게 한다.
- 현재 worktree 기준으로 startup, 10초 idle animation, 10초 synthetic wheel, viewport resize sequence를 실행하고 trace JSON을 baseline artifact로 저장한다.
- baseline은 현재 `FAIL` 재현 자료이며 성공 기준으로 승격하지 않는다.

### S1. 안정적인 Playwright oracle 추가 — `PASS`

- `pageerror`, `console.error`, request failure, WebGL context loss를 공통 fixture에서 즉시 수집한다.
- 진단 API가 준비될 때까지 DOM timeout만 기다리지 말고 snapshot의 front generation과 exact commit을 readiness 조건으로 사용한다.
- trace terminal마다 request/generation이 정확히 한 번 종료되는지 helper에서 검증한다.
- screenshot은 전체 페이지보다 canvas bounding box를 대상으로 하고, 연속 표본 간 완전 blank/단색 clear-only 후보를 검출한다.
- 색상 변화가 정상 animation인 장면에서 pixel hash 동일 여부를 성공 조건으로 사용하지 않는다. blank 후보만 자동 실패시키고 나머지는 artifact로 남긴다.

### S2. presenter 왕복과 frame cadence 수정 — `PASS`

- `DorotiWebGlSurface.RenderFrame` 안의 nested async JS interop를 제거하고 browser-wasm에서 한 번의 frame-boundary 호출로 `RenderFrame → exact blit → CompleteFrame`을 직렬화한다.
- 여러 invalidate는 latest pending scene 하나로 합치되 이미 raster를 시작한 frame은 terminal을 정확히 하나 기록한다.
- `wheel-continuity.spec.ts`의 동일 seed/delta sequence 전후 trace를 비교해 submitted cadence, queue depth, p95 latency, terminal 누락을 판정한다.
- 개선이 없으면 synchronous interop 가설을 기각하고 해당 변경만 되돌린 뒤 managed paint/Skia flush 비용을 별도 profile한다.

### S3. Flutter 방식 wheel 처리와 Playwright 검증 — `PASS`

- DOM wheel sample을 rAF 누적 없이 즉시 managed `PointerDataPacket`으로 전달한다.
- `DOM_DELTA_PIXEL/LINE/PAGE`를 Flutter Web 기준의 pixel delta로 변환한다.
- 테스트 A: `page.mouse.wheel()`로 사용자 경로와 preventDefault/scroll 반응을 확인한다.
- 테스트 B: `page.evaluate()`의 `WheelEvent`로 delta mode, timestamp, 작은 연속 delta, 큰 mouse-wheel delta를 결정적으로 주입한다.
- 테스트 C: 10초 동안 60/120 Hz에 가까운 burst sequence를 보내 event 수, 순서, delta 합계, dispatch 지연, frame coalescing을 trace로 확인한다.
- synthetic event로 `isTrusted`나 실제 trackpad kind를 위조하지 않는다. 실제 장치 kind 판정은 사용자 게이트로 남긴다.

### S4. Flutter 방식 resize 처리와 Playwright 검증 — `PASS`

- root `ResizeObserver`와 DPR watcher는 최신 metrics publish만 담당하고 observer callback에서 canvas backing reset, framebuffer bind, clear, blit을 수행하지 않는다.
- 이전 preserved front는 새 exact staging frame이 끝날 때까지 유지한다.
- 테스트 A: `page.setViewportSize()`로 A→B→C와 최종 크기를 반복해 deterministic resize 계약을 검증한다.
- 테스트 B: headed Desktop Chrome에서 CDP `Browser.setWindowBounds`를 10~16 ms 간격으로 변경해 실제 창 resize에 가까운 연속 표본을 수집한다.
- 테스트 C: device scale factor project와 browser zoom 표본을 분리해 logical/physical/DPR 조합이 섞이지 않는지 확인한다.
- A/B intermediate generation은 `superseded`, C final generation은 `submitted`여야 하며 final canvas backing size가 C physical target과 일치해야 한다.

### S5. flicker와 context restore 자동 회귀 — `PASS`

- idle animation 60초, synthetic wheel 30초, automated window resize 30초 동안 diagnostic terminal과 canvas screenshot/pixel probe를 수집한다.
- `__dorotiResizeDiagnostics.loseContext/restoreContext`를 사용한 별도 test에서 context generation 증가, resource 재생성, first restored exact commit을 검증한다.
- 정상 continuous rendering 중 blank/clear-only candidate, `failed`, unpaired terminal, stale size commit이 하나라도 있으면 Playwright test를 실패시킨다.
- 실패 시 해당 시점 전후 trace JSON, screenshot, video, Playwright trace를 같은 test artifact 묶음으로 남긴다.

### S6. 회귀·실기기 검증과 문서화 — 자동 `PASS`, 물리적 게이트 `notVerified`

- focused managed validation과 Playwright 전체 suite를 모두 실행한다.
- Playwright 통과 후 Desktop Chrome에서 실제 trackpad scroll, 실제 window border drag, maximize/restore, zoom/DPR, 한글 IME를 확인한다.
- 자동 `PASS`와 사용자 `PASS`를 결과 표에서 별도 열로 기록한다. 실행하지 않은 Edge/Firefox, 60~165 Hz display, 접근성 도구, 실제 배포 환경은 `notVerified`로 남긴다.
- 원인, 폐기한 실험, final counters, Playwright artifact 위치, 사용자 체감 결과를 `history/26-08-29/` 아래 dated 문서에 기록한다.

## 실행 명령과 20분 timeout

```powershell
# Release build
dotnet build ./DorotiDemoApp/web/DorotiDemoApp.Web.csproj -c Release

# focused managed validation
dotnet run --project ./Doroti/validation/fcr7-material-widget/Doroti.Validation.Fcr7MaterialWidget.csproj -c Release
dotnet run --project ./Doroti/validation/resize-contract/Doroti.Validation.ResizeContract.csproj -c Release

# 최초 Playwright 준비
npm ci --prefix ./Doroti/validation/web-playwright
npx --prefix ./Doroti/validation/web-playwright playwright install chromium

# Release server lifecycle + Playwright suite
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release

# 문제별 재실행
npx --prefix ./Doroti/validation/web-playwright playwright test tests/wheel-continuity.spec.ts
npx --prefix ./Doroti/validation/web-playwright playwright test tests/resize-continuity.spec.ts --project=desktop-chrome-headed
```

모든 test runner와 외부 command 호출은 `.github/copilot-instructions.md`에 따라 20분 timeout을 적용한다. 단일 60초 표본을 줄여 timeout을 우회하지 않는다.

## MVP와 후속 경계

### 이번 MVP

- Playwright 기반 Web 회귀 harness와 진단 artifact 파이프라인
- Windows Chrome에서 wheel ingress, presenter cadence, resize epoch/backing-size 자동 검증
- resize/raster WebGL ownership 분리
- focus/font/IME 기존 `PASS` 보호
- 실제 trackpad, 실제 border drag, 실제 화면 flicker 사용자 재검증

### 후속

- iframe scroll boundary의 parent propagation
- Firefox의 mouse/trackpad 판별 한계와 브라우저별 delta matrix
- Edge/Firefox Playwright project 승격
- 90/120/144/165 Hz display별 latency budget
- mobile browser virtual keyboard resize와 orientation change
- OffscreenCanvas/worker renderer 도입 여부

## 작업 중 판정 규칙

- Playwright build/test/counter가 깨끗해도 실제 trackpad나 border resize가 버벅이면 `FAIL`을 유지한다.
- 실제 사용자 입력이 통과해도 deterministic Playwright 회귀가 실패하면 자동 계약은 `FAIL`로 기록한다.
- intermediate frame의 `superseded`는 latest-only 정책상 허용하지만 terminal 누락, queue depth 2 초과, stale/size-mismatch present는 허용하지 않는다.
- `page.mouse.wheel()` 결과를 실제 trackpad 검증으로, screenshot을 display scan-out acknowledgement로 부르지 않는다.
- 한글 font/focus 사용자 확인은 독립 `PASS`로 보존하며 scroll/resize `FAIL`을 가리지 않는다.
- 시각 실험이 악화되면 실패 artifact와 rollback 경계를 기록하고 마지막 사용자 확인 경로로 되돌린다.
