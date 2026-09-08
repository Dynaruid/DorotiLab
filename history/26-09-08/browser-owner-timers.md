# Browser owner timers — 2026-09-08

## 문제와 수정 범위

이전 WebGPU 편집 검증은 `Control+X` 부근에서
`System.Threading.SynchronizationLockException: Lock_Exit_SynchronizationLockException`
으로 .NET WASM runtime이 종료됐다. 스택은 `System.Threading.Lock.Exit` →
`TimerQueueTimer.Fire` → `ThreadPoolWorkQueue.Dispatch`였다. 원본은
`.doroti/webgpu-direct/webgpu-picker-final.log`와
`Doroti/validation/web-playwright/artifacts/webgpu-picker-final/`에 그대로 남아 있다.
[.NET runtime #129900](https://github.com/dotnet/runtime/issues/129900)에 동일한
멀티스레드 WASM 타이머 큐 오류가 보고되어 있다. 그 내부 버그 자체를 고쳤다고
주장하지 않는다. 이번 수정은 Doroti 웹의 타이머 작업이 그 큐를 사용하지 않게 한다.

WebGPU 기본값, 명시적 WebGL, main-owned shared runtime/렌더 Worker 구조는 유지했다.

## 구현

- `DartAsyncRuntime`에 실행 컨텍스트별 `TimeProvider` scope를 추가했다.
  `PlatformDispatcher`는 생성 당시 provider를 캡처하고 각 framework callback에서
  복원한다. 기존 공개 dispatcher/session constructor 시그니처는 유지한다.
- `Doroti.Runtime.Timer`는 `TimeProvider.CreateTimer`를 사용한다. 예약 전에 handle을
  확정해 zero-delay race를 막고, one-shot 실행 시 handle을 해제한다. 취소된 queued
  callback 억제와 periodic self-cancellation 계약을 유지한다. `Timer.run`도 같은
  이벤트 경로를 사용한다.
- `Future`의 두 delay 형태, timeout과 timeout recovery, 공용 framework의 timed wait,
  debug print delay를 provider에 연결했다. timeout recovery는 캡처한 Dart scheduler에서
  실행한다. microtask 안에서 만든 timer도 host wakeup scheduler를 보존한다.
- `BrowserTimeProvider`는 렌더 Worker의 `setTimeout`을 사용한다. 다른 .NET thread의
  Change/Dispose는 JS owner로 전달하고 generation으로 오래된 예약을 거부한다.
  긴 지연은 브라우저 int32 제한 내에서 다시 예약하며, callback 전에 만료 여부를 확인한다.
  DisposeAsync는 진행 중인 callback 완료를 기다린다.
- Web runner가 앱 생성 전 provider scope를 설치하고 종료 시 모든 소유 timer를
  정리한다. 폰트 및 이미지 HTTP 요청의 기존 100초 제한은 provider 기반 cancellation으로
  유지한다. 웹에서 scope 없는 Dart timer는 문제의 System timer로 자동 전환하지 않는다.
- 네이티브 호스트는 기본 `TimeProvider.System`을 계속 사용한다.
- diagnostics에 backend, owner thread, active handles, cross-thread operation 및
  `System.Threading.Timer.ActiveCount`를 추가했다. 테스트 전용 C# stress export는
  `DorotiTimerValidation=true`에서만 컴파일하고 일반 제품 publish에는 제외한다.

## 검증 결과

모든 build/test 프로세스 timeout은 1,200초다. Playwright 자동 retry는 0이며,
명시적 반복 검사는 원래 실패 동작을 3개 새 context에서 각각 수행했다.

| 검사 | 결과 |
| --- | --- |
| Runtime async 계약 | PASS: 기본 native timer, fake host clock, cancellation, Timer.run, periodic cancel, 두 Future delay, timeout, nested timer wakeup |
| WebGPU timer stress, 24 periodic timers / 60초 | PASS: 175,724 callbacks |
| WebGL timer stress, 24 periodic timers / 60초 | PASS: 266,408 callbacks |
| 위 두 stress의 thread/queue/resource 검사 | wrongThread=0, peakSystemTimers=0, 취소 callback=0, 종료 후 liveTimers=0 및 JS pending=0 |
| 최신 일반 제품 빌드의 원래 picker/편집/clipboard 동작 | 3/3 PASS; 42.5 / 39.5 / 38.8초 |
| 일반 제품의 이미지 theme·responsive state·정상/invalid protocol/device loss 종료 | 5/5 PASS |
| Host 및 Playwright TypeScript 선언 검사 | PASS |
| FCR-7 Material/widget runtime | PASS, Debug; 최종 로그는 동반 JSON 참조 |
| threaded Release native publish | PASS; validation export 없는 일반 제품도 별도 publish |
| 변경된 Runtime/Ui/Foundation/Material/Host/Target package | 6개 pack PASS |

최초 브라우저 구현 검증에서도 267,512 timer callbacks 및 수명 4개 검사가 통과했다.
이후 timeout recovery의 owner 복귀와 nested microtask wakeup을 보완한 최종 stress는
위의 WebGPU/WebGL 숫자다. 여러 버전의 callback 수를 합쳐 최종 성능 수치로 제시하지 않는다.

각 stress는 Task delay, Future delay, Future timeout, cancellation timeout,
다른 thread에서의 timer Change/Dispose도 포함한다. 원래 편집 검증은 date/time picker,
메뉴, 검색, 입력, 전체 선택, 복사, 잘라내기, 붙여넣기와 focus 이동까지 수행했다.
일반 제품의 종료 검사는 native/managed timer registry 및 JS pending handle이 모두 0임을
검증한다. 의도적 GPU loss의 native map cancellation 진단과 정상 실행 오류는 구분한다.

원본 TimerQueue FAIL은 보존하며, 후속 수정의 통과 결과로 이를 덮어쓰지 않는다.
초기 구현 빌드의 `TimeProvider.System` 이름 충돌과 Web target의 명시적 Runtime 참조 누락도
실패 로그로 보존했고, 두 항목을 수정한 뒤 최종 build/test를 수행했다.

## 한계와 증거

이 결과는 Doroti가 소유한 timer/Future/HTTP/wait 경로에 대한 수정과 검증이다.
외부 라이브러리나 앱이 직접 만든 `System.Threading.Timer`, provider를 넘기지 않은
`Task.Delay` 등은 자동으로 바뀌지 않는다. 앱의 추가 .NET 지연 작업은 host scope 안에서
`DartAsyncRuntime.timeProvider`를 명시할 수 있다. 다른 브라우저·실기기·물리 IME,
장시간 soak 및 성능 향상은 이번 검사로 입증하지 않았다.

- 로그/배포/패키지: `.doroti/browser-timers/`.
- 최종 제품: `.doroti/browser-timers/final-product-publish/wwwroot`.
- stress 전용 제품: `.doroti/browser-timers/final-validation-publish/wwwroot`.
- [명령·로그 hash·stress 원본·검증 요약](browser-owner-timers-audit.json).
- Playwright artifacts: `browser-timers-initial`, `browser-timers-stress-webgpu`,
  `browser-timers-stress-webgl`, `browser-timers-product-editing`, `browser-timers-product-regression`.
- 설계: [ADR-003 browser timer ownership](../../Doroti/docs/adr/ADR-003-web-main-runtime-render-worker.md#browser-timer-ownership-2026-09-08).

검증용으로 시작한 5220/5221/5222 서버와 테스트 브라우저는 종료했다.
