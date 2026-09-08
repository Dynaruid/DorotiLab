# Web threaded runtime 부팅 수정 결과

2026-09-08. 사용자가 “메인 런타임 초기화 + 렌더 Worker 분리로 전환”을 선택한
후속 작업이다. 기존 [flag 활성화의 최초 FAIL](wasm-threads-bootstrap.md)은 보존한다.

## 결과와 소유권

최종 trimmed Release publish에서 부팅·입력·종료·protocol 회귀 검사가 PASS다.
메인 document가 .NET runtime **1개**를 초기화하고, 같은 runtime의 JSWebWorker가
기존 framework/layout/Skia direct WebGL 역할을 실행한다. DOM·input·IME·semantics
endpoint는 메인에 남는다. `WasmEnableThreads=true`, `worker-direct-webgl`을 유지했다.
layout과 raster는 여전히 같은 Worker owner에서 실행한다.

- .NET 10.0.11, runtime hash `e2f47b0110ed922f21a1522da67279133ce28f32`.
- Chromium 151.0.7922.34, document/Worker isolation true, shared WASM heap 확인.
- 메인 runtime count 1, 독립 Worker runtime count 0, 렌더 managed thread ID 8.
- AMD Radeon 780M의 hardware WebGL2, software fallback false.
- 첫 화면 1,380개 sampled color, resize 800→1280의 exact generation 확인,
  wheel 후 pixels 변화, page/runtime/console errors 0.
- 선택 컨트롤 상태와 narrow 화면 테마 변경 후 열 복귀 2개 회귀 case PASS.
- 정상 role disposal 및 잘못된 private-port protocol 거부 2개 통합 case PASS.
  disposal은 1번이며 메인 runtime을 종료하거나 두 번째 runtime을 생성하지 않는다.

[미리보기](http://127.0.0.1:5197/?dorotiTestbedMode=sample)는
`.doroti/threads-fix/repaired-wwwroot`의 고정 산출물을 제공한다. 검증은 headless
hardware Chromium 자동 입력과 screenshot 확인이다. 물리 화면·IME·접근성의 사용자
수용, 다른 브라우저/기기, 운영 hosting header, managed 계산 중첩, layout 병렬화,
정량적 성능 개선은 **notVerified**다. work3 전체 완료를 의미하지 않는다.

## 원인과 수정

원래 Doroti 일반 Worker에서 `dotnet.create()`를 실행하자 runtime의
`__mono_message__ / monoRegistered / UI Thread` 메시지가 Doroti protocol에 섞였다.
이것이 version undefined 오류를 일으켰다. 별도로 runtime pthread attach 경로의
`dispatchEvent` 대상이 없어 초기화가 실패했다. 실제 수신 envelope는
[원본 진단](wasm-main-runtime-evidence/fix-before.json)에 있다.

설치된 native runtime 소스에서 `ENVIRONMENT_IS_WORKER` 값이
`initializeReplacements` 전에 복사되는 순서와 Worker-root 초기화 경로를 확인했다.
동일 계열의 [공식 runtime issue #114140](https://github.com/dotnet/runtime/issues/114140)도
대조했다. 이번 수정은 upstream 버그 패치가 아니라 사용자가 선택한 메인-root
구조로의 전환이다. runtime 생성 파일·NuGet cache·private PThread 상태는 수정하지 않았다.

`BrowserManagedRenderThread`가 .NET의 JSWebWorker로 렌더 역할을 실행한다.
일반 Task.Run과 달리 JS synchronization context를 갖는다. 현재 public reference에
노출되지 않는 experimental API이므로 공식 소스의 reflection 진입점
`RunAsyncVoid`를 작은 adapter에 격리하고 trimming dependency를 명시했다.
[설치 버전의 JSWebWorker 소스](https://github.com/dotnet/runtime/blob/v10.0.11/src/libraries/System.Runtime.InteropServices.JavaScript/src/System/Runtime/InteropServices/JavaScript/JSWebWorker.cs)를
기준으로 하며 runtime upgrade 시 재검증이 필요하다.

공개 Worker constructor에서 same-origin runtime worker의 일회용 token handshake를
받아 전용 MessagePort를 연결한다. runtime 제어 메시지는 원래 처리기로 전달하고
Doroti 메시지는 전용 port에서 엄격한 version 검증을 유지한다. OffscreenCanvas도
이 port로 넘긴다. renderer role을 정리할 때 runtime 소유 Worker를 강제 종료하지 않는다.

실제 scroll 검증에서 timer의 frame 요청이 다른 managed thread에서 JS interop을
호출하는 문제도 발견했다. BrowserHostAdapter가 캡처한 owner SynchronizationContext로
JS wakeup을 보내도록 수정했다. 종료 경로는 중복 dispose를 방지하고, managed host의
`closed`와 GPU/role의 `disposed`를 구분하며, 후자까지 DOM endpoint를 유지한다.
설계는 [ADR-003](../../Doroti/docs/adr/ADR-003-web-main-runtime-render-worker.md)에 있다.

## 실행 ledger

후속 수정은 **20개 명령**, 각각 외부 timeout 20분, retry 0, build/browser 순차 실행이다.
최초 flag 활성화의 2회는 별도 원본 ledger다. 10회 이후에는 trimmed publish,
JS owner와 종료 순서의 독립 검증 필요성을 확인해 최대 20회까지 확장했다.
실패·probe setup 문제·중단도 포함한다. 한 lifecycle/회귀 명령의 case 수는 아래에
명시했으며 케이스들을 숨겨 한 번의 입력으로 세지 않는다. 소스 조회·자산 복사·hash는
검증 실행에 포함하지 않는다. 최종 제품 소스는 17번 publish 이후 바뀌지 않았다.

모든 label에 `threads-fix-` 접두사를 붙인다. stdout/stderr 원문은
[evidence 디렉터리](wasm-main-runtime-evidence)에 보존했다.

| 번호 | label 뒤 부분 | 결과 및 발견 |
| --- | --- | --- |
| 1 | 01-diagnose | 진단 수집 성공, 기존 Worker-root runtime FAIL 재현 |
| 2 | 02-main-runtime | PASS: 메인 runtime만 생성, threads/shared heap/errors 0 |
| 3 | 03-build | FAIL: EventTarget 전환 후 TS event data/error typing, 1:22.42 |
| 4 | 04-build | PASS: Release build 48.61초, warning/error 0 |
| 5 | 05-browser | 중단/notAccepted: 약 4분, idle pthread CDP 평가에서 probe 정지; 최종 JSON 없음 |
| 6 | 06-browser | FAIL: first exact가 초기 단색 surface여서 initialColors 1 |
| 7 | 07-content | FAIL: 실제 content/resize/scroll 후 timer의 requestFrame JS owner 오류 |
| 8 | 08-owner-build | PASS: owner marshal 수정 Release build 11.80초, warning/error 0 |
| 9 | 09-owner-browser | PASS: 실제 content/resize/scroll, errors 0 |
| 10 | 10-publish | PASS: trimmed Release publish |
| 11 | 11-publish-browser | PASS: publish content/resize/scroll |
| 12 | 12-lifecycle | 2 cases: shutdown FAIL(중복 disposed→disposing), invalid protocol PASS |
| 13 | 13-final-publish | PASS: dispose idempotence 반영 publish |
| 14 | 14-final-browser | PASS: content/resize/scroll |
| 15 | 15-final-lifecycle | 2 cases: shutdown FAIL(제거된 DOM host로 late cursor), invalid protocol PASS |
| 16 | 16-shutdown-publish | PASS: DOM teardown 지연 반영 publish |
| 17 | 17-closed-publish | PASS: 소스 검토에서 찾은 누락된 closed protocol 등록, 최종 publish |
| 18 | 18-repaired-lifecycle | 2 cases 모두 PASS: shutdown 4.3초, protocol 4.0초, 총 8.9초 |
| 19 | 19-regression | 2 cases 모두 PASS: selection 18.1초, theme/columns 24.5초, 총 43.5초 |
| 20 | 20-verified-browser | PASS: 최종5197 first content/resize/scroll/shared heap/owner, errors 0 |

5번은 idle .NET Worker가 Atomics.wait에 있을 때 모든 Worker를 CDP evaluate하는
probe 문제였다. 제품 정상으로 세지 않았다. 이후에는 메인 runtime API와 렌더 역할의
실제 exported thread ID를 검증한다. 6번 이후 first exact receipt만으로 화면을
수용하지 않고 실제 sampled content를 추가 대기한다. 7/12/15번 제품 오류는 각각
원인을 수정한 후 새 label로 검증했으며 최초 실패 log를 덮어쓰지 않았다.

## 재현 경로와 보존 증거

빌드 산출물 격리: `dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj
-c Release --artifacts-path .doroti/threads-fix/artifacts`.
외부 timeout wrapper는 `Doroti/eng/invoke-work2-check.ps1`다. publish의 wwwroot를
고정 복사하고 `Doroti/eng/serve-isolated-web.py`로 COOP/COEP를 제공했다.

브라우저 작업 경로는 `Doroti/validation/web-playwright`, 환경변수
`DOROTI_WEB_BASE_URL=http://127.0.0.1:5197`이다. 새 실행 시 고유 label을 사용한다.

- 최종 bootstrap: `node probe-threaded-bootstrap.mjs <새-label>`.
- Lifecycle: `npx playwright test tests/threaded-runtime.spec.ts --project=chromium-hardware`.
- 회귀: `npx playwright test tests/material-sample-selection.spec.ts tests/retained-columns.spec.ts --project=chromium-hardware --grep "shared state|parked right"`.
- [최종 browser JSON](wasm-main-runtime-evidence/main-verified.json),
  [첫 화면](wasm-main-runtime-evidence/main-verified-initial.png),
  [스크롤 화면](wasm-main-runtime-evidence/main-verified-scrolled.png).
- [source/asset/evidence SHA-256 manifest](wasm-main-runtime-manifest.json):
  base HEAD `9464a84e87fbdd2f9e546eac7baca6c30ea93880` + dirty 구현 소스,
  306개 served asset, 47개 evidence 사본의 원본 hash 일치 확인.

기존 단일 스레드 비교 산출물과 5189 preview는 보존했다. 중간 수정 산출물도
삭제하지 않았으며 임시 5192–5196 서버만 종료하고 최종 5197 preview를 유지한다.
