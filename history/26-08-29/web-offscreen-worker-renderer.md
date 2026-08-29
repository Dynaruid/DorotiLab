# Web OffscreenCanvas 및 persistent Worker renderer 구현

- 날짜: 2026-08-29
- 사용자 육안 확인: 2026-08-30
- 대상: `Doroti.Host.Web`, `Doroti.Target.Web.browser-wasm`, `Doroti.Runner.Sdk`, Web Playwright
- 최종 정책: `document-webgl`이 `auto` 기본값이며 `offscreen-bitmap`과 `offscreen-worker`는 first-class opt-in backend다.

## 구현

- renderer 선택을 runtime 시작 전에 고정하고 `document-webgl`, same-thread `offscreen-bitmap`, persistent `offscreen-worker` 세 모드를 만들었다.
- offscreen path는 DOM에 붙지 않은 실제 `OffscreenCanvas`의 hardware WebGL2 context를 Skia가 raster하고, `createImageBitmap` 결과를 main thread의 `bitmaprenderer`에 exact size로 commit한다.
- resize generation과 별도의 request id, current+latest 최대 2 mailbox, submitted/superseded/failed exactly-once terminal, stale bitmap close와 bitmap/resource counters를 구현했다.
- worker mode에서는 main Blazor runtime을 시작하지 않는다. worker의 .NET runtime 하나가 Doroti app/framework/Skia를 소유하고 main thread는 DOM, input/IME, semantics, clipboard/plugin과 display commit만 담당한다.
- worker crash는 한 번만 자동 재시작하며, 정상 pagehide에서는 generated `StopWorker`를 거쳐 managed session과 GPU/resource를 정리한다.
- published Worker는 document import map을 상속하지 않으므로 main이 현재 fingerprinted `_framework/dotnet.*.js` URL을 versioned init message로 전달한다. 이 계약으로 누적 publish 폴더의 오래된 stable alias를 잘못 읽는 문제를 제거했다.
- diagnostics/backend identity, publish asset check `DOROTIWEB026`, ADR-020, target manifest와 TypeScript declaration을 갱신했다.

## 검증 결과

| 검증 | 결과 |
| --- | --- |
| Release build | `PASS`, 경고 0, 오류 0 |
| TypeScript `tsc --noEmit` | `PASS` |
| document correctness | `PASS`, 8 tests + worker-only 1 skip |
| same-thread offscreen correctness | `PASS`, 8 tests + worker-only 1 skip |
| worker correctness | `PASS`, 9 tests |
| trimmed publish | `PASS` |
| publish 정적 서버 worker capability/crash recovery | `PASS`, 2/2 |
| FCR-7 Material/widget contract | `PASS` |
| resize contract | `PASS`, 22 request/22 terminal, max queue 2, stale/unterminated 0 |

Chrome hardware A/B 3회 결과:

| mode | p95 표본 | median p95 | max 표본 | 판정 |
| --- | --- | --- | --- | --- |
| `document-webgl` | 26.7 / 26.1 / 26.2 ms | 26.2 ms | 31.8 / 33.9 / 35.9 ms | absolute gate `PASS` |
| `offscreen-bitmap` | 47.8 / 49.1 / 52.9 ms | 49.1 ms | 59.7 / 61.7 / 66.7 ms | absolute/comparison `FAIL` |
| `offscreen-worker` | 44.1 / 47.3 / 41.2 ms | 44.1 ms | 94.5 / 68.8 / 86.1 ms | absolute/comparison `FAIL` |

구조적 구현과 correctness는 완료했지만 현재 장비에서 latency 개선은 입증되지 않았다. 따라서 worker backend를 제거하지 않고 opt-in으로 유지하며, `auto`는 direct path를 선택한다.

## 실제 사용자 확인

2026-08-30 Desktop browser에서 다음 세 backend를 각각 직접 표시했다.

- `/?dorotiRenderer=document-webgl`
- `/?dorotiRenderer=offscreen-bitmap`
- `/?dorotiRenderer=offscreen-worker`

세 모드 모두 기본 화면 렌더링과 육안상 체감 성능이 사용할 수 있는 수준이라는 사용자 확인을 받았다. 이 결과는 기본적인 visible rendering acceptance를 닫지만, 위 A/B latency gate의 실패를 재분류하지 않는다. 즉 offscreen 두 모드는 기능적으로 동작하고 육안상 수용 가능하지만, 현재 계측에서는 direct 대비 성능 개선이 입증되지 않았으므로 opt-in 정책을 유지한다.

## 다음 실시간 창 resize 작업으로 넘길 계약

이번 구조는 후속 창 크기 실시간 추종 작업의 기반이다. 특히 `offscreen-worker`는 main thread가 DOM resize/input을 계속 처리하는 동안 worker가 framework build/layout과 Skia raster를 수행할 수 있어 구조적으로 가장 유력한 후보다. 다만 실제 우위는 물리 window border drag A/B로 판정해야 한다.

후속 작업에서 보존할 계약:

- `ResizeObserver`와 DPR watcher는 immutable resize epoch만 publish하고 visible backing이나 GPU surface를 직접 초기화하지 않는다.
- raster mailbox는 시작된 `current` 하나와 교체 가능한 `latest` 하나만 유지해 오래된 중간 크기가 누적되지 않게 한다.
- resize generation과 request id를 분리하고, stale bitmap은 visible canvas에 전달하지 않고 닫는다.
- 완성된 exact-size bitmap만 visible `bitmaprenderer`에 commit하며, 새 frame 전까지 마지막 정상 front를 유지한다.
- drag 중 event 처리, latest-size 추종 지연, blank/stretch/flicker, drag 종료 후 final exact commit, monitor 간 DPR 이동을 세 backend에서 같은 조건으로 측정한다.
- resize 체감 결과가 좋아도 wheel latency와 기본 backend 승격은 별도 gate로 유지한다. resize 전용 정책이 필요하면 실제 측정 결과를 근거로 결정한다.

## 검증 경계

- 자동 screenshot/pixel 검사는 browser 내부 결과이며 물리 monitor scan-out ACK가 아니다.
- 세 backend의 기본 화면 렌더링과 육안상 체감 성능은 사용자 확인 `PASS`다.
- 실제 precision trackpad 30초 왕복, window border drag/maximize/restore, monitor 간 DPR 이동, 90-165 Hz display는 여전히 `notVerified`다.
- 실제 한글 IME 조합/삭제와 screen reader 접근성은 `notVerified`다.
- Firefox/Safari의 offscreen/worker 성능과 기본 승격은 `notVerified`다.
