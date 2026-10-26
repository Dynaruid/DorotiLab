# Web CanvasKit N0–N6 실행 기록

**현재 선택: C1 기반 encoder·path mapper 후속 최적화 적용.** 아래 자동 라운드의 C1 기각은 당시 결정이며 실패 자료는 그대로 보존한다. 루트 `work2.md` 삭제 후 전체 흐름과 최종 상태는 [C1 최적화 종료 요약](web-canvaskit-c1-optimization-summary.md)을 따른다.

2026-09-05 사용자 `work2.md의 전체 작업` 요청으로 계획의 구현 라운드를 시작했다. 문서 작성 당시의 미실행 범위는 당시 기록으로 보존한다.

## 후보를 보기 전 고정한 조건

- C0: 시작 clean HEAD `44ab3eb38f377b5bb11a0079c8b2bc10a5c89fa9`, 현재 선호 구현 포함. `.doroti/checkpoints/n0-20260905/source.zip`, `tracked.patch`(빈 diff), `publish-existing/`, `publish-inventory.json`으로 보존. 기존 publish는 stale fingerprint 가능성이 있어 재현용 C0를 별도 새 디렉터리에 non-AOT publish한다.
- C0 URL: `?dorotiRenderer=worker-canvaskit-webgl&dorotiResizeScheduling=display&dorotiCopyOwnership=owned&dorotiMetricsCoalescing=frame&dorotiEncodingCache=1`.
- C0/C1은 동일 계측을 적용하고, 국소 opt-in이 부적절하면 별도 publish root를 명시한다. wrapper에 `-PublishDirectory`를 추가했다. 실제 응답 inventory/hash는 browser harness에서 수집한다.
- 모든 build/test는 20분 timeout. native 중 build/기능 검사/PNG 분석을 겹치지 않는다.
- N1: 비native fixture와 overhead 확인 후 Left/150ms/reverse trace on/capture off 최대 1회.
- N2: N1의 한 원인만 선택. 동작 보존과 phase 감소가 없으면 C1 철회. 진단 계측은 성능 실행에서 off.
- N4: Left C0→C1, TopLeft C1→C0, Bottom/600ms/expand C0→C1, 각 1회. Left 큰 퇴행/자극/binary 오류는 즉시 중단. 목표 notification p95 및 interval ≤33.3ms, settle ≤50ms, active >100ms gap 0. 첫 front/active age/geometry 퇴행도 별도 판정. 1 pair로 통계적 우월성 주장 금지.
- N5: 유지 가치가 있을 때만 TopLeft/150ms/reverse C0/C1 capture 각 1회. 실제 환경 interval/speed로 상대 gate 산출, popup 사전 확인, 마지막 pre-motion marker 교정, 손실 0 및 독립 content/wrap 검증. 물리 scan-out은 notVerified.
- N1 1 + N4 6 + N5 2 + 실패 대응 예비 1 = native 최대 10회. 실패도 포함. 병목/이득 근거가 부족하면 N1/N2에서 종료하며 잔여 횟수 소진 금지.

## Native 실행 ledger

1. `n1l`: N1 C0+계측 / Left / 150ms / reverse / trace on / capture off — 실행 완료, 자극 PASS. native 1/10. active callback 6개, 원시 자료 및 `artifacts/n1l/phases.json` 보존.
2. `n4l0`: Left/150/reverse C0 trace/capture off — 자극 PASS, latency FAIL.
3. `n4l1`: Left/150/reverse C1 trace/capture off — 자극 PASS, latency FAIL. p95 51.6→46.2ms, first front 35.92→31.76ms, settle 33.5→20.7ms, active age p95 56.75→51.15ms. interval p95 25.9→26.9ms 및 width error p95 183→184px의 소폭 증가도 보존. 큰 퇴행/자극 오류 없음, 다음 조건 진행.
4. `n4t1`: TopLeft/150/reverse C1 — 자극 PASS, latency FAIL.
5. `n4t0`: TopLeft/150/reverse C0 — 자극 PASS, latency FAIL.
6. `n4b0`: Bottom/600/expand C0 — 자극 PASS, latency FAIL.
7. `n4b1`: Bottom/600/expand C1 — 자극 PASS, latency FAIL. 종료: **7/10**, N5/예비 미소비.

## 진행

- N0: 새 isolated C0 publish 완료. `n0v` F0/F1/F2 direct/cache/bitmap/resize/restart 3 PASS. 최초 `n0`는 headed filter에 의해 No tests found, 실제 테스트 미실행.
- `n0s1` 기존 C0와 `n1s1` 계측 C0를 같은 비native fixture로 순차 실행, 각각 1 PASS. startup served inventory, 실제 응답 hash, source patch, trace 및 stationary transition 자료 보존. 첫 `n0s`/`n1s`는 trace off에서 존재하지 않는 collect hook을 호출한 harness 오류로 FAIL, trace on에서만 수집하도록 수정.
- N1 계측: frame 밖 resize 적용/JSON/managed 적용/dispatch 대기, listener barrier 이유, scene/recorded clock 연결, bounded numeric work counters 2048 sample ring. 노드/stack/tree 보관 없음. GC/할당 바이트의 WASM 의미는 검증하지 않아 사용하지 않는다.
- reviewed product framework와 pinned Flutter `56b8e1a851a594b1a154f8ea93270807dab22b9a`의 BuildScope/RenderObject/MediaQuery 비교. ADR-019 소유권 확인. 기존 dedup/fast path와 의미 유지, generated base 변경 없음.
- managed 계약 15 PASS: 동일 값/aspect/DPR/insets/text scale/accessibility, layout fast path/명시적 invalidation, counter ring wrap. 새 equal-content displayFeatures list가 notify하는 관찰은 성능 원인 확정이 아니다. 첫 compile은 `EdgeInsets.all` API명 오류, `CreateAll` 수정 후 통과.
- stationary 4전환 평균 frame 시간의 중앙값: C0 trace off 14.47/13.68ms, on 12.87/12.32ms; 계측 C0 off 12.52/12.44ms, on 14.64/13.36ms. 진단 on 비용/프레임 수 변동이 있으며 이를 native 개선량이나 정확한 overhead 비율로 해석하지 않는다. 계측 off의 큰 퇴행은 관찰되지 않았고 trace on native 1회는 비용 분해 용도로만 사용한다.

## N1 근거와 N2 사전 선택

- Left active 6 callback 중앙값: UI frame 20.70ms, build 5.90ms, layout+compositing 4.75ms, paint 2.75ms, scene 5.80ms(내부 map 1.35ms, encode 3.15ms). terminal→UI 처리 대기 19.75ms, transport 0.20ms. 포함 관계의 중앙값을 더하지 않는다.
- active resize apply 9개 중앙값 0.90ms: epoch 적용 0.30ms, JSON은 timer 해상도에서 0ms, managed snapshot 0.50ms. dispatch 전 대기 0.20ms. `snapshot` 등 listener barrier도 기록한다. 0ms는 비용 부재를 뜻하지 않는다.
- 프레임당 enqueue 8, duplicate 0, rebuild 62(강제 54), initial sort 1/resort 3, layout 268진입/20fast/248work/dirty+same constraints 1, 새 picture 28. 필요한 크기 변경 작업을 생략할 근거 없음.
- MediaQuery dependent 검사 122회/구독 합 290/실제 알림 1. 기존 `HashSet<object>(dependencies.Cast<object>())`가 매 검사마다 typed enum 집합을 새 object 집합으로 복제한다. N2는 **이 임시 집합과 boxing만 제거**한다. equality 정책과 aspect switch, 최초 dirty 전파는 유지한다. equal-content list semantics 수정은 이번 원인에 섞지 않는다.
- C1 반증: 같은 nonnative fixture에서 동일 동작/작업량 유지 및 build 구간 감소가 확인되지 않으면 기각. 프레임 밖으로 작업 이동/새 cache/전역 객체 보관 없음. C0는 `.doroti/publish/n1-c0`, C1은 별도 `.doroti/publish/n2-c1`에서 비교한다.
- 이 코드는 ADR-019의 reviewed/adopted product source다. pinned Flutter의 Set<Object>는 Dart 원형이며, C#의 이미 typed인 override 집합에 직접 순회한다. lowerer의 generic inherited-model bridge(FrameworkCSharpLowerer.Members.cs:660,1366) 소유권 확인; compiler-owned generated base/선택 입력은 바꾸지 않는다. importer의 의미 오류가 아니라 product에서 불필요한 C# 재복제 제거다.

## N2 결과

- C1은 typed HashSet을 직접 순회하며 기존 aspect 비교와 short circuit을 유지한다. 새로운 전역 cache, metrics 생략, scheduling 변경 없음.
- `n1s1`/`n2s`의 실제 MediaQuery 검사 프레임 8개를 비교했다. 모두 dependent 122/rebuild 62, width 전환 layout 248, height 전환 76으로 동일했다.
- build ms C0→C1: 14.199→9.9, 5.101→3.9, 5.601→4.2, 6.199→5.7 / 14.5→9.4, 5.4→3.7, 5.6→4.7, 6.5→5.7. 첫 width 전환 cold 비용을 warm과 섞어 일반화하지 않는다. UI 전체는 1표본 19.5→20.60ms 증가도 있어 모든 phase 개선이라고 주장하지 않는다.
- CLR numeric aspect 검사 10,000회는 warm 후 0 bytes. WASM GC/바이트 수로 확대하지 않는다. C1 계약 16 PASS. C1 publish의 Widgets WASM `0ea1e4630886de4bc9e1e9c6f7695e88e4e780f60f25321402736003864c2933`, 계측 C0는 `3e71e02f03cfab0c12d011de1ed414c5f545497c81b8b99e1529bd55fc47d4fe`.

## N3 correctness 범위

- `n3`: CanvasKit 12 PASS — all-opcode golden, transfer/lease terminals, F0/F1/F2 direct/cache/bitmap resize/restart, DPR 1/1.25/1.5/2, synthetic composition/input barrier, main/UI stall, 실제 F3 responsive wrap.
- `n3life`: maximize/restore, 실제 same-window hidden→visible 1 PASS. 기존 noDefaults CDP 보정 유지.
- 실제 F3 `Reviewed Material … strict Skia GPU` 문구의 CanvasKit threshold 402/403 logical px: 2/1 raster ink bands, bounds 높이 40/20px. `wrap-402.png`/`wrap-403.png` 시각 확인, 줄 내용/경계 내 raster 확인. 고정 폭 ImageFilter 문구를 대용으로 쓰지 않았다.
- `n3doc` default document의 입력/semantics/warm front 검사 PASS(130.70ms). 추가로 적용한 document wrap 검사는 FAIL; `n3doc0`의 원본 C0에서도 같은 463/464px semantics/ink 불일치 재현. `n3doc1`에서 raster를 직접 찾도록 보강했을 때 320px에서도 한 줄 clipping 확인. 따라서 이 문제는 **기존 default document renderer FAIL**, C1 회귀가 아니다. 신규 테스트는 default에서 명시적 expected failure로 보존하며 수정 시 unexpected PASS가 나도록 했다. product 기본 renderer의 이 별도 결함을 이번 단일 원인 변경에 섞지 않는다.
- N3의 C1 관련 correctness와 default 최소 입력 회귀는 PASS; 위 추가 default wrap FAIL과 전체 renderer qualification은 별도로 남긴다. 이후 N4는 C1 상대 회귀 검증 범위이며 default wrap/플랫폼 전체 PASS를 주장하지 않는다.
- 보강된 raster threshold fixture와 CanvasKit warm input `n3w1` 2 PASS, warm 126.20ms. default의 미해결 wrap을 숨겨 전체 correctness PASS로 묶지 않는다.

## N4: 조건별 C0/C1 결과와 기각

실제 환경은 Chromium **151.0.7922.34**, Radeon 780M / ANGLE D3D11, DPI **192**, **165Hz**, Release non-AOT였다. 아래 표는 trace/capture off 6회의 독립 performance corpus다. 모든 실행의 프로세스 exit 0 및 자극 PASS, **latency 6 FAIL**. C0 분모는 기존 선택 URL을 쓴 계측 C0이며 과거 30fps baseline이 아니다.

단위 ms. settle은 observer 기준 exact settle이며 native 종료 기준은 별도 아래에 기록한다.

| 조건 | 후보 | notification p95 / p99 / max | interval p95 | 첫 front | boundary gap max | exact settle | active age p95 |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Left / 150 / reverse | C0 | 51.6 / 51.7 / 51.7 | 25.9 | 35.92 | 35.92 | 33.5 | 56.75 |
| Left / 150 / reverse | C1 | 46.2 / 50.2 / 50.2 | 26.9 | 31.76 | 31.76 | 20.7 | 51.15 |
| TopLeft / 150 / reverse | C0 | 54.1 / 62.1 / 62.1 | 29.8 | 38.52 | 38.52 | 27.7 | 63.45 |
| TopLeft / 150 / reverse | C1 | 51.5 / 55.7 / 55.7 | 28.4 | 32.38 | 32.38 | **36.0** | 54.65 |
| Bottom / 600 / expand | C0 | 39.7 / 42.0 / 42.1 | 21.8 | 24.61 | 24.61 | 22.4 | 43.91 |
| Bottom / 600 / expand | C1 | **40.8 / 48.2 / 50.7** | **31.1** | **29.75** | **34.90** | **28.7** | 42.90 |

- native 종료 기준 settle C0→C1: Left 42.62→28.56, TopLeft 36.73→**50.78**, Bottom 33.82→40.96ms. TopLeft C1은 이 기준의 50ms도 넘었다.
- 미도달 0, active >100ms gap 0, logical geometry uncovered 0. pre-motion idle age는 244.98~248.19ms로 별도 보존했고 active age와 혼합하지 않았다.
- time-weighted logical geometry integral(px·ms) C0→C1: Left width 14754.19→14510.90; TopLeft width/height 15470.39/15447.62→14121.31/14131.89; Bottom height 8873.74→8326.91. 평균 geometry는 줄었지만 Left width p95 183→184, Bottom height max 23→27px 증가. 이 수치는 실제 content landmark나 물리 화면 geometry가 아니다. raw `nativeGeometry=notComparable`도 그대로 보존한다.
- notification p95 상대 변화는 Left -10.47%, TopLeft -4.81%, Bottom +2.77%. 조건당 1 pair라 안정적 개선이나 통계적 우월성을 주장하지 않는다. Bottom cadence/첫 front/끝 frame 퇴행과 TopLeft settle 퇴행을 noise라고 임의 기각하지 않았다.
- **C1 기각**: 국소 build/할당 감소는 유효한 관찰이지만 사용자-facing native 지표는 섞여 있고 절대 목표도 미달이다. C1만 철회하고 C0 의미와 선택 URL을 유지한다.
- 전체 p99/max/coverage/원시 source·응답 inventory: `Doroti/validation/web-playwright/artifacts/n4-comparison.json`; 재생성은 `summarize-n-pairs.py`를 사용한다.

## N5: 조건부 미실행

N4에서 C1을 철회했으므로 F3 WGC C0/C1 캡처 pair를 시작하지 않았다. capture/preflight native 자극 0회, 예비 미소비. 새 WGC PNG, popup 사전 확인, content landmark/공백 폭·지속 비교, 상대 gap/center gate는 **notVerified**다. 따라서 기존 capturedPresentation FAIL을 새 PASS로 바꾸지 않는다. 준비 중이던 미실행 preflight helper 변경도 철회했다.

CanvasKit stationary wrap threshold 402/403px는 native 자극의 최소 logical width 약 627px 밖이었다. **dynamic wrap-in-motion notVerified**. stationary PNG는 이 항목의 대체 증거가 아니다.

## N6: 최종 판정과 보존

| 범위 | 최종 판정 |
| --- | --- |
| C1 관련 correctness | scoped PASS: managed 16개, CanvasKit N3 15개(12+1+2), default 최소 입력 PASS. 기존 default wrap FAIL 별도 |
| stimulus | **PASS**, native 7회 모두 유효. 추가 native 없음 |
| latency | **FAIL**, 성능 6회 모두 notification p95 목표 미달, C1 일부 퇴행 |
| capturedPresentation | **notVerified (이번 라운드 미실행)**, 이전 FAIL 유지 |
| manualAcceptance | **notVerified**, C0 선호 피드백을 C1 수용으로 사용하지 않음 |
| 채택 | **C0 유지 / C1 철회 / PARTIAL / notQualified** |

유지한 작업은 opt-in bounded 진단 계측, source/publish 분리 wrapper, managed/비native 회귀와 분석 도구다. compiler-owned generated base, 기본 renderer 선택, Windows/ANGLE/Vulkan 경로는 변경하지 않았다. C1이 없으므로 새 후보의 사용자 수용을 요청하지 않는다. 실제 OS IME, 60Hz, monitor 이동, 물리 scan-out은 여전히 미검증이다.

재현 및 identity:

- 원본 C0: `.doroti/checkpoints/n0-20260905/source.zip`, 빈 `tracked.patch`, `publish-existing/` 및 `publish-inventory.json`; clean 시작 HEAD와 관련 untracked 0개 확인.
- fresh 원본 C0: `.doroti/checkpoints/n0-20260905/publish-c0`; Widgets WASM `ef28776a2796b7c552ff19e6807475895cdacf22b10b1fcc7b1160b5ee06a042`.
- **최종 제품 소스는 측정한 N1 C0와 동일**: native `n1l/source.patch`의 `Doroti/src/` diff와 최종 diff를 전부 비교하고 untracked `FrameworkWorkCounters.cs` hash도 확인. `.doroti/checkpoints/n0-20260905/final-product-identity.json`에 기록. 해당 검증 binary는 `.doroti/publish/n1-c0`이고 명시적 `-PublishDirectory`로 재사용할 수 있다.
- C1 binary `.doroti/publish/n2-c1`는 비교 증거로만 보존하며 선택하지 않는다. 다시 적용 가능한 [기각된 C1 패치](web-canvaskit-n-c1-rejected.patch)를 별도 보존했고 `git apply --check` 통과.
- raw report의 `manifest.source`는 측정 harness checkout identity다. 별도 serve root의 앱 revision을 이 값과 혼동하지 않는다. 위 app source/checkpoint와 실제 served Widgets WASM hash로 C0/C1을 구분한다. Worker에서 관측되지 않은 asset까지 BrowserContext inventory가 완전하다고 주장하지 않는다.
- 후보 복원 후 처음 CLR 검사에는 Copy-Item이 보존한 과거 timestamp 때문에 C1 incremental binary가 남았다(`contract-final`, 수용 증거에서 제외). timestamp를 갱신하여 다시 컴파일한 `contract-final-r1`에서 최종 C0 15 PASS, numeric aspect 10,000회 **4,560,000 bytes** 확인. C1의 같은 CLR 검사는 0 bytes였다. publish N1/N2는 별도 보존돼 이 문제의 영향을 받지 않았다.
- `artifacts/wrapper/n*`의 build/test/server 로그와 각 `artifacts/n*/test-results`의 원시 report/PNG/실패를 보존했다. 모든 build/test 프로세스는 20분 timeout으로 실행했다.
- 최종 복원 C0의 실제 raster threshold 재검사 `n6wrap` PASS. `n6doc`는 동일 default clipping이 **expected failure로 재현**됐으며 Playwright가 이를 `1 passed`로 집계해도 기능 PASS로 해석하지 않는다. 최종 TypeScript, `git diff --check`, 기각 패치 적용 가능성 검사 PASS.
- 종료 확인: 소유 5188/5189 listener, static server, native capture driver 및 lifecycle Chromium 잔류 없음. 사용자 프로세스를 종료하지 않았다.

이번 라운드는 N0→N4 실행, N5 조건부 중단, N6 기각/복원/기록으로 종료했다. 성능·시각 품질의 전체 수용 완료를 뜻하지 않는다.

## 후속 직접 확인과 C1 채택

- 사용자에게 C0 `http://127.0.0.1:5188`과 C1 `http://127.0.0.1:5189`의 동일 CanvasKit 선택 옵션을 제공했다. 실제 served Widgets WASM hash가 각 보존 binary와 일치하는 것을 확인했다. 서버 소유 PID와 경로는 `.doroti/manual/c1-review/session.json`, 응답 검증은 `verified-urls.json`에 있다.
- 사용자 피드백: “육안으로 보기엔 둘이 거의 동일한 수준의 fps같이 보인다, 메모리에서 c1이 더 이득이라면 c1으로 하고싶은데”. 이를 **육안 FPS 차이 미미 + 임시 할당 감소를 근거로 한 C1 선택**으로 반영했다. 모든 방향·IME·물리 scan-out의 수용 PASS로 확대하지 않는다.
- 입증한 메모리 관련 이득은 CLR의 준비된 numeric aspect 검사 10,000회에서 C0 4,560,000 bytes, C1 0 bytes이다. 프레임마다 dependent 집합을 새 object HashSet으로 복사하는 작업과 enum boxing을 제거한다. WASM 앱의 총 heap/RSS/peak memory 감소량은 측정하지 않았으므로 그 수치를 주장하지 않는다.
- 기각 패치를 다시 적용했고 MediaQuery 소스 SHA-256 `5866fd4b53049b0d465ea32b4a8b8bb07631b18caedb0e82a89e5f7652f2da05`가 보존 C1과 일치했다. `n2s/source.patch`의 전체 product diff 및 untracked product source hash와도 대조해 직접 비교한 C1과 같은 제품 소스임을 확인했다.
- `contract-adopt-c1` 16 PASS, warm numeric aspect loop 0 bytes 재확인. 채택한 할당 특성을 회귀 검사의 assertion으로 복원했다. 새 native 측정 없이 기존 성능 실패/퇴행 및 7/10 ledger를 보존한다.
- 현재 적용 상태는 **C1 채택 / PARTIAL · notQualified**. 기본 `auto=document-webgl` renderer 선택 및 Windows/ANGLE/Vulkan 경로는 바꾸지 않았다. C0 source/publish와 기존 C1 비교용 publish도 보존한다.
- 기본 `.doroti/publish/web-Publish-Release`를 실제 Release non-AOT publish로 갱신했다(`adopt-c1`). 갱신된 기본 publish에서 all-opcode golden, transfer/lease terminal, F3 responsive wrap **3 PASS**. 실제 browser가 `Doroti.Framework.Widgets.ed9iv0a1r5.wasm`을 200 응답으로 받았으며 SHA-256 `0ea1e4630886de4bc9e1e9c6f7695e88e4e780f60f25321402736003864c2933`이 직접 비교한 C1과 일치했다. 오래된 fingerprint 파일의 존재만으로 binary를 판정하지 않았다.
- 채택 재검증은 managed 16 + browser 3 PASS, `git diff --check` PASS, 빌드/검사 모두 20분 timeout. 임시 검증 서버 5191은 종료됐고 직접 확인용 C0/C1 서버 5188/5189는 유지한다. 추가 native 자극 없음.

## 후속 Playwright Chromium 메모리 실측

사용자 요청으로 headless Chromium 151.0.7922.34에서 독립 실행 C0/C1 각 4회 비교를 완료했다. .NET WASM linear-memory capacity는 모두 **138MiB**, 전체 process private bytes 중앙값은 **C0 978.97 / C1 986.31MiB**였다. paired C1−C0 범위 −24.23~+16.01MiB로 방향이 바뀌어 전체 메모리 절감은 확인하지 못했다. V8 GC 후 보조 결과도 구분해 보존했다. 따라서 위 채택 시점의 “WASM 총 heap/RSS 미측정” 이후 실제 process/V8/linear-memory 측정은 추가됐지만, live .NET heap과 누적 WASM managed 할당은 여전히 미측정이다.

상세 조건, raw evidence, 최초 harness 실패와 재실행은 [C0/C1 메모리 결과](web-canvaskit-c0-c1-memory-results.md)에 있다. 측정용 Chromium 전부 종료, 수동 서버 유지, native 0회 추가. 현재 C1 선택과 PARTIAL · notQualified를 유지하며, 입증한 이득은 임시 할당 제거로 한정한다.
