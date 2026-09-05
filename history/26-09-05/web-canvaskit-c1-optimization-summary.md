# Web CanvasKit C1 기반 최적화 종료 요약

작성일: 2026-09-05. 루트 `work2.md`의 계획·실행·후속 채택을 압축한 보관 기록이다. 원본 계획은 이 요약과 상세 실행 기록으로 정리하고 삭제했다. 새 자동 실행 계획이나 전체 성능 수용 선언이 아니다.

## 최종 선택

**C1 MediaQuery 개선 + encoder + path mapper 최적화를 유지한다.** 기본 Release non-AOT publish를 갱신했고, 최종 CanvasKit 브라우저 회귀 20개를 통과했다. 사용자는 마지막 직접 확인에서 “그럭저럭 잘되네”라고 평가했다. 이 피드백은 현재 구현에 대한 실사용 의견이며 전체 방향·플랫폼·물리 화면 검증 PASS로 확대하지 않는다.

전체 판정은 **PARTIAL / notQualified**다. 임시 할당 감소는 확인했지만 실제 FPS·WASM .NET GC 정지 시간·총 RAM의 일관된 개선은 입증하지 않았다. 기존 native latency FAIL과 document renderer wrap FAIL을 보존한다.

## 실행과 결정의 흐름

| 단계 | 수행 및 판정 |
| --- | --- |
| N0 | 당시 C0 source/publish 및 비교 조건을 보존하고 isolated publish 지원을 추가했다. |
| N1 | bounded framework counter와 frame 전후 resize/managed 적용 계측을 추가했다. Left native 진단 1회로 비용을 분해했다. |
| N2 | MediaQuery dependent 검사 122회에서 실제 알림은 1회였다. 기존 typed enum 집합을 object HashSet으로 복제하던 할당·boxing을 제거한 C1을 구현했다. 비교·알림 의미는 유지했다. |
| N3 | C1 관련 managed/browser correctness, 실제 raster 줄바꿈, DPR·입력·수명 계약을 검증했다. 기본 document renderer의 기존 clipping 실패는 별도로 재현했다. |
| N4 | Left·TopLeft·Bottom C0/C1 각 1회, 총 6회 비교했다. 일부 지표 개선과 퇴행이 섞이고 절대 latency 목표에 미달해 당시 C1을 철회했다. |
| N5 | N4 기각에 따라 조건부 캡처를 실행하지 않았다. 새 WGC/content/wrap-in-motion 증거가 없다. |
| N6 | 실패·미검증·source/publish identity를 기록하고 당시 C0를 복원했다. 이후 결정과 구분해 이력을 보존했다. |
| 사용자 재선택 | 직접 비교한 FPS가 비슷하다는 의견과 임시 할당 감소를 근거로 C1을 재적용했다. managed 16개·browser 3개 재검증과 기본 publish 갱신을 완료했다. |
| Chromium 메모리 비교 | C0/C1 각각 새 프로세스로 4회 비교했다. 전체 메모리 절감은 확인하지 못했다. |
| C1 후속 최적화 | encoder와 path mapper를 순서대로 개선·검증했다. ACK 대기의 대부분이 기록된 UI 동기 작업과 겹쳐 별도 scheduling 변경은 적용하지 않았다. |

N 라운드 native 실행은 **진단 1 + 비교 6 = 7/10회**로 종료했다. 이후 메모리·최적화 검증은 비native이며 추가 드래그는 0회다. 미소비 3회는 자동 후속 실행 지시가 아니다. 이전 2부의 별도 10회 예산도 다시 열지 않는다.

## 유지한 개선과 근거

| 변경 | 측정된 결과 | 범위 |
| --- | --- | --- |
| C1 MediaQuery | 준비된 numeric-aspect 검사 10,000회 임시 할당 4,560,000→0 bytes | CLR 국소 검사. WASM 전체 할당/GC 횟수와 구분 |
| Encoder | cache on fixture 임시 할당 36.85~41.53% 감소 | resource/string 임시 writer 2개 제거, 최종 배열 직접 쓰기, 이전 command 길이에 따른 초기 capacity 지정 |
| Path mapper | fixture 임시 할당 67.68~84.70% 감소 | argument 개수 사전 계산, 명령별 LINQ iterator와 List 확장 감소 |
| UI Worker ACK 분석 | 기존 trace의 대기 합계 97.50ms 중 94.00ms(96.4%)가 기록된 UI busy 구간과 겹침 | 5 scene의 제한된 trace 분석. 미귀속 시간을 전송 비용으로 단정하지 않음 |

Encoder 크기 힌트는 producer별 정수 하나이며 256 bytes~1MiB로 제한한다. 큰 buffer 자체를 보관하거나 전송 중인 배열을 재사용하지 않는다. 작은 scene 성공과 `Clear()`에 맞춰 힌트를 갱신한다. Mapper의 최종 immutable snapshot 및 document 방어적 복사는 유지한다.

C1/C2(encoder)/C3(encoder+mapper)를 정순·역순으로 실행한 stationary 비교 6개는 모두 통과했다. 실제 metrics 변경 frame 16개/후보의 UI frame 중앙값은 17.25/18.15/17.50ms였다. 이 결과는 일관된 FPS 개선을 보여주지 않으며, 국소 CLR 할당 감소율을 화면 성능 향상률로 환산하지 않는다.

## 메모리 실측의 경계

앞선 C0/C1 비교는 Playwright headless Chromium 151.0.7922.34, Windows, Radeon 780M / ANGLE D3D11, DPR 1에서 같은 viewport 전환으로 수행했다.

- .NET WASM linear-memory capacity는 모든 실행에서 **138MiB**로 같았다.
- 프로세스 트리 private bytes 중앙값은 **C0 978.97 / C1 986.31MiB**였고, paired 차이의 방향이 바뀌어 전체 절감을 확인하지 못했다.
- V8 GC 후 보조 수치도 따로 보존했다. V8 heap/GC와 WASM 안의 .NET live heap/GC는 구분한다.
- 이 메모리 corpus는 encoder·mapper 후속 변경 **전**의 C0/C1 결과다. 최종 C3의 총 RAM을 측정한 결과로 사용하지 않는다.

## 검증·미해결 사항

- 최종 managed wire golden 6330 bytes와 기존 SHA-256을 보존했다. roundtrip, malformed/resource/Unicode/fuzz, cache eviction, 큰/작은 scene 전환, 이전 output 불변 및 path 좌표·명령 순서 검사를 통과했다.
- 최종 기본 publish에서 CanvasKit **20 PASS**: golden, transfer/lease, resize pixels, DPR, synthetic input/composition, focus 순서, Worker ownership/stall/restart/resource replay, malformed protocol, lifecycle 회복, 실제 raster wrap을 확인했다.
- 기본 `auto=document-webgl` 검사에서는 golden/terminal **2 PASS**, 기존 raster wrap clipping **expected failure 1개**를 재현했다. Playwright의 `3 passed` 집계를 기능 3 PASS로 해석하지 않는다.
- 기존 native 성능 6회 모두 notification p95 목표 33.3ms에 미달했다. interval ≤33.3ms, exact settle ≤50ms, active >100ms gap 0과 첫/끝 frame·geometry 기준을 완화하지 않는다.
- N5 미실행에 따른 새 capturedPresentation, dynamic wrap-in-motion, 실제 OS IME, 60Hz/monitor 이동, 물리 scan-out은 미검증이다. 사용자 의견과 자동 검사로 대체하지 않는다.
- main=DOM/input/IME/semantics/logical CSS, UI Worker=.NET/layout, Raster Worker=visible OffscreenCanvas/WebGL2 소유권을 유지했다. epoch·surface 검증, input/lifecycle 앞 metrics barrier, 최신 pending 상태, transfer/resource 수명과 queue 상한도 보존했다.
- 기본 renderer 선택과 Windows/ANGLE/Vulkan 경로는 변경하지 않았다. 고정 FPS 제한, 필요한 layout 생략, CSS stretch, cache 무제한 확대는 도입하지 않았다. 모든 build/test는 20분 timeout으로 수행했다.

## 재현과 상세 기록

선택 옵션:

```text
?dorotiRenderer=worker-canvaskit-webgl&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1
```

최종 기본 publish는 `.doroti/publish/web-Publish-Release`, 비교용 C3는 `.doroti/publish/c1-next-c3`, C1은 `.doroti/publish/n2-c1`, C0는 `.doroti/publish/n1-c0`에 보존했다. `.doroti/checkpoints/c1-next/`에 baseline/final patch, source hash, managed binaries/logs, ACK 분석을 보관했다. 앱 identity는 실제 응답 WASM hash로 판별하며 오래된 fingerprint 파일이나 harness checkout만으로 확정하지 않는다.

정리 시점의 수동 비교 서버는 5188=C0, 5189=C1, 5190=최종 C1+encoder+mapper다. 서버는 세션 자원이므로 영구 URL이 아니다. 자동 검증 서버·브라우저는 종료했다.

- [N0~N6 실행·기각·재채택 기록](web-canvaskit-n-results.md)
- [C0/C1 Chromium 메모리 실측](web-canvaskit-c0-c1-memory-results.md)
- [Encoder·mapper 및 ACK 후속 실행](web-canvaskit-c1-followup.md)
- [당시 기각 후 재적용한 C1 패치](web-canvaskit-n-c1-rejected.patch): `rejected` 파일명은 당시 이력이다.
- [선행 resize redesign 종료 요약](web-canvaskit-redesign-v2-summary.md)

이번 보관으로 `work2.md`의 실행·정리 작업은 종료한다. 미해결 성능·화면 품질 항목은 위 기록에 남기며 새 구현이나 전체 qualification 완료로 간주하지 않는다.
