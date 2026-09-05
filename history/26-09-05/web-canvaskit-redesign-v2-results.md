# Web CanvasKit 재설계 v2 실행 결과

2026-09-05. **사용자 요청으로 이번 구현·실행 종료 / PARTIAL · notQualified.** 남은 작업은 [작업계획 2부](../../work.md)에 작성했다. 아래 W0 최초 기록은 당시 실행 결과이며 후속 구현과 구분한다.

## 후속 구현과 검증 기록

- W0: F0/F1/F2 동등 fixture, 같은 NanumGothic 폰트, GPU 픽셀 epoch/size 마커, native→observer 모호성 처리, 연속 시간 가중 content age, managed frame trace와 timer precision probe를 추가했다. 합성 회귀 5 PASS.
- W1: 실제 Publish/PublishAot/PublishAotPartial 서버 모드, Raster rAF(P1), 독립 OffscreenCanvas→bitmaprenderer(P2a exact allocation / P2b exact crop), immediate/rAF 소비 실험을 추가했다. bitmap credit 최대 2, main pending 최대 1, consumed/closed 구분, 재시작 수명 검증이 있다.
- W2/W3a: managed→JS fresh copy의 소유권 이양, UI 중복 slice 제거(전송 pool copy와 replay journal 유지), input/focus/IME/lifecycle 이전에 pending metrics를 적용하는 병합 옵션, immutable Picture snapshot identity와 bounded CPU mapping cache를 추가했다. resource·paragraph·가변 path/paint 그래프는 캐시하지 않는다. cache는 최대 128 blocks/32768 fixed-size commands(보수적 32 MiB accounting envelope), P2 backing/staging/bitmap은 128 MiB, P2 Skia resource cache는 64 MiB다. 실제 GPU 전체 메모리 측정값을 뜻하지 않는다.
- `auto=document-webgl`, CanvasKit opt-in과 기존 기본 scheduling/direct presentation은 유지한다. 채택 전 실험 옵션을 기본으로 승격하지 않는다.
- 공통 FCR-4 retained contract PASS. DisplayList golden PASS: 6330 bytes, SHA-256 `66412CCB5E02519BBD8C11ECAB5E63CE914E2DB745F6D51110BBD03F89CCBE42`.
- `resize-v2-w3-fixture-trace`: F0/F1/F2 × direct / owned-cache / bitmap-crop / bitmap-exact / bitmap-rAF, 네 크기와 bitmap worker restart: 3 tests PASS. F1 resize의 고정 picture 3개 명령이 재사용되고 F2 paragraph 명령은 재매핑된다. 동일 크기 추가 frame에서는 F1 mapping 8→0 commands. 전송/encoding 자체는 여전히 inline이다.
- `resize-v2-flutter-static`: 동등 fixture 정적 비교 1 PASS. F0/F1 위치·색과 F2 줄바꿈 수/오른쪽 edge/텍스트 위쪽 위치(2px 이내)를 비교했다. frame latency 또는 scan-out 비교가 아니다.
- `resize-v2-w2-correctness-combined`: 11 PASS / 1 FAIL. topology, golden, malformed input, buffer/lease terminal, 3회 Raster replacement, DPR2, TextField 색 검증은 PASS. 컴파일과 겹친 warm input latency 256.2ms가 250ms gate를 넘었다. 단독 재실행이 필요하며 이 FAIL을 소급 PASS로 바꾸지 않는다.
- `resize-v2-w5-input-headed`: native selection paint suppression, TextField 아래 hit target, native context-menu endpoint 3 PASS. OS IME 수동 수용은 별도다.
- 정적 viewport 테스트가 `setViewportSize` 반환 직후 이전 observer epoch의 settled를 읽는 race를 발견했다. 요청한 logical size 관측을 먼저 기다리도록 수정했다. 실패 screenshot은 1100×700 root 안에 이전 720×500 frame이었고, DPR 실패는 이전 epoch 2/3과 새 marker 3/4를 비교했다. native 성능 구간에는 이 대기를 추가하지 않았다.

### 빌드 실험의 실패 경계

1. `resize-v2-w1-aot-correctness`: full AOT가 Material 및 aot-instances 컴파일에서 `-1073741571 (0xC00000FD)`로 실패했다. 이는 Windows stack overflow 코드다. [Microsoft 진단 문서](https://learn.microsoft.com/en-us/windows-hardware/drivers/debugger/debugging-a-stack-overflow).
2. workspace 복사본 compiler의 PE stack reserve를 8→64 MiB로 바꾼 실험은 설치 SDK를 수정하지 않았다. 실제 복사본 프로세스 경로를 확인한 재시도에서 available RAM 약 2.09 GiB가 되어 해당 소유 compiler 프로세스만 중지했다. full AOT 성공이 아니다. [PE stack reserve](https://learn.microsoft.com/en-us/cpp/build/reference/stack-stack-allocations?view=msvc-170).
3. Material interpreted / 나머지 AOT / WasmDedup=false인 별도 `PublishAotPartial`을 추가했다. 최초 publish는 성공했지만 browser stack overflow로 시작 실패했다. 이후 공용 bin/obj로 non-AOT를 실행하자 interpreter NIY assertion도 발생했다.
4. 빌드 모드마다 `--artifacts-path .doroti/artifacts/web-<mode>`를 사용해 모든 프로젝트의 bin/obj를 격리했다. 격리 non-AOT의 F0/F1/F2 correctness 3 PASS로 복구됐다. 공용 산출물을 사용한 AOT/runtime 실패는 격리 빌드의 판정으로 대신하지 않는다. 격리 partial-AOT도 publish 성공 후 F0/F1/F2 모두 browser `Maximum call stack size exceeded`로 시작 실패했다(`resize-v2-w1-partial-aot-isolated`). AOT 성능 비교는 불성립이다.

모든 test/build 프로세스는 20분 제한으로 관리한다. 컴파일과 겹친 기능 검증의 latency 로그를 최종 성능 corpus에 포함하지 않는다.

## 이번 작업 종료 시점의 추가 결과

### 추가 구현·검증

- per-producer `DisplayListEncodingCache`: 검증된 table-independent immutable command payload만 재사용한다. resource/string/path/filter는 기존 encode 경로를 유지한다. 최대 8192 entries/8 MiB charged memory, producer dispose 시 해제. cold/warm canonical golden, 새로운 동등 값, 변경 geometry/color, missing resource, invalid opacity, eviction/clear contract PASS.
- `RecordedAtMicroseconds`: 기존 causally clamped timestamp와 구분한 opt-in 실제 phase clock. FCR-3 contract PASS. 기존 timestamp로 build/layout/paint 작업 시간을 계산하지 않는다. 새 clock을 사용하는 F3 native 진단은 사용자 중단으로 미실행이다.
- `resize-v2-w3-encoding-correctness`: F0/F1/F2 × 6가지 direct/cache/bitmap 옵션, resize 및 bitmap restart **3 PASS**. 이때 실제 isolated non-AOT publish를 갱신했다.
- `resize-v2-default-regression`: 4 PASS / 1 skip. `resize-v2-worker-direct-regression`: 5 PASS. shared FCR-4 retained 및 DisplayList contract PASS.
- `resize-v2-w5-pixel-geometry-fixed`: fixtures 3 + DPR 4 PASS, embedded assertion 1 FAIL. assertion 수정 후 `resize-v2-w5-embedded-fixed` 1 PASS. 과거 실패를 삭제하지 않았다.
- 마지막으로 추가한 main/UI 100ms stall, maximize/restore/background 테스트는 TypeScript 확인만 했고 실행하지 않았다. selected encoding 옵션의 DPR/embedded 재실행, idle baseline/active motion age 신규 synthetic 회귀 및 warm input 단독 재실행도 남았다.

### 독립 후보 비교

TopLeft/150ms/reverse, trace off, 각 3회, isolated Release non-AOT publish. 아래는 trial p95의 중앙값(ms)이며 raw latency를 한 모집단으로 합치지 않았다.

| `resize-v2-focus` 후보 | 중앙값 |
| --- | ---: |
| baseline | 62.3 |
| display | 54.3 |
| display + owned copy | 48.5 |
| 위 조합 + frame metrics 병합 | 45.0 |
| 위 조합 + picture cache | 50.5 |
| Raster rAF | 56.3 |
| bitmap crop | 52.2 |
| bitmap exact | 55.8 |
| bitmap main rAF | 55.1 |

별도 `resize-v2-encoding` 3회 비교는 baseline **64.0**, coalesce **50.8**, encoding cache 추가 **46.9**, picture+encoding **50.4ms**였다. 모든 F3 trial의 latency gate는 FAIL. 조합의 baseline 대비 개선과 encoding cache만의 효과를 구분한다. encoding 추가만으로 20% 이득을 입증하지 않았다.

`resize-v2-focus-trace` 진단에서 F3는 29 pictures 중 median 1개(약 3.4%)를 재사용했다. active mapping median 약 1.2/1.4ms, encoding 약 1.9/2.1ms(owned/cache), 전송 약 17.7~18.5 KiB다. UI→Raster median 0.1ms, Raster replay→submit median 3.4ms, Raster terminal→UI 처리 median 18.2ms였다. UI 동기 작업으로 늦어진 ACK를 전송 비용으로 계산하지 않는다. W3b registry와 단일 Worker 확대를 정당화하는 자료가 아니다.

### 확대 측정은 사용자 요청으로 중단

사용자가 “횟수 너무 많이 한다 10회면 충분”이라고 요청해 controller를 중단했다. 완료된 `resize-v2-full`은 **54회**이며 전체 matrix 완료가 아니다. Right의 150/600ms × expand/shrink/reverse, Bottom의 150ms × expand/shrink/reverse에서 각 baseline/encoding 3회다.

| 집계 | baseline | encoding 조합 |
| --- | ---: | ---: |
| 유효 자극 | 27/27 | 27/27 |
| legacy/v2 latency PASS | 0/27 | 0/27 |
| trial p95 중앙값 ms | 61.3 | 47.1 |
| boundary gap 최대 ms | 49.70 | 51.50 |

active >100ms 공백은 0회. 첫 프레임은 Right expand 일부 조건에서 소폭 퇴행했고 Bottom/150ms/shrink의 중앙값 개선은 약 16%였다. 전체 중앙값 약 23% 개선만으로 모든 조건의 채택 기준을 충족했다고 쓰지 않는다.

54회 source identity는 `25ccce2564de05adbf5317a18d6aba04439bffd2460b31da9b0f19cf4fe5a5d2` 하나다. `resize-v2-full/summary.json`과 각 raw report에 source/served hashes·조건별 수치가 있다. 이후 테스트·문서 편집으로 현재 checkout hash는 다르다.

### 공통 캡처와 종료 직전 진단

`resize-v2-common-capture`: 같은 native TopLeft/150ms/reverse, F0/F1/F2 × Doroti/Flutter **각 1회, 총 6회**. 3 test PASS. 동일 WGC callback + GPU pixel marker endpoint이며 framework post-frame과 GPU submit을 비교하지 않았다. 현재 source와 실제 관측 served asset/Flutter revision을 보고서에 저장했다.

| fixture | Doroti/Flutter boundary gap p95 ms | Doroti/Flutter center X error p95 physical px | 상대 width/height·gap 기준 |
| --- | --- | --- | --- |
| F0 | 24.91 / 46.53 | 104.5 / 145.5 | 해당 단일 표본 PASS |
| F1 | 33.70 / 42.38 | 145.5 / 195.5 | 해당 단일 표본 PASS |
| F2 | 56.27 / 62.72 | 262.5 / 178.5 | 해당 단일 표본 PASS |

**F2 center는 Doroti가 더 나빴다.** center/wrap 전체 품질 gate를 통과했다고 해석하지 않는다. 이 비교는 165Hz이며 물리 scan-out·재현성 수용은 notVerified다.

F6-R은 monitor 전체를 캡처하므로 PNG 크기를 창 크기로 쓰지 않도록 native window rect를 사용하고 root/center에서 의도된 창 이동량을 보정했다. 기존 W0/F3 log-only 자료에는 영향이 없다.

`v2cap`의 F0 P1 rAF / P2 crop / P2 exact 각 1회는 test PASS, notification p95 **19.7 / 14.0 / 43.5ms**(마지막 latency FAIL), active marker decode **16/16, 15/15, 14/14**였다. `resize-v2-capture`의 앞선 3건은 긴 Windows 캡처 경로 때문에 드래그 전 setup 실패했다. run-id를 출력 basename으로 줄이고 짧은 artifact label에서 수행했다. setup 실패를 자극 PASS로 재분류하지 않는다.

횟수 축소 요청 이후 완료된 추가 native drag는 **9회**다. 이후 사용자가 외출을 위해 이번 작업 종료를 요청했다. F3 trace+capture는 시작하지 않았으며 추가 측정·테스트·빌드를 실행하지 않고 소유 controller/서버/native 프로세스를 종료했다. 남은 검증과 개선은 2부 계획으로 넘겼다.

**최종 상태: 구현·일부 기능 검증 완료, F3 성능 목표 FAIL, W5 및 사용자 직접 수용 미완료. 기본값 승격 없음.**

## W0 최초 실행 기록

## 변경

- `canvaskit-native-fast-resize.spec.ts` 보고서를 v2로 올리고 v1 `following` 수치·판정을 그대로 보존했다.
- `helpers/resize-following.ts`: target 전체 분모, exact epoch와 caught-up 구분, 미도달/대체 count, 첫 front 대기, 시작·끝 포함 gap, 100ms 초과 정지, 정확한 generation/size/DPR settle을 추가했다. front 0회는 FAIL, 1회는 interval 표본 부족이다.
- 완료 프레임 geometry는 main host의 최신 target epoch 대신 direct-commit payload에서 읽는다. native active 시간에 observer target과 notification front의 logical width/height/area 오차를 시간 가중 적분하고 p95/max를 저장한다. 초기 상태를 알 수 없는 시간은 uncovered로 남긴다.
- content age는 동일 generation 재제출로 초기화하지 않는다. mean은 구간 내 age 증가를 적분하고 max와 coverage를 함께 남긴다. native end settle은 motion 종료 전에 일치한 경우 음수가 될 수 있다.
- stage trace는 기본 off, `DOROTI_FAST_RESIZE_TRACE=1`에서만 on이다. main notification trace는 양쪽 모두 필요하며, 완전히 계측을 제거한 실행을 의미하지 않는다. video/Playwright trace는 기존처럼 off다.
- `helpers/resize-manifest.ts`: HEAD, tracked binary patch/hash, untracked 파일 hash, 관측된 JS/WASM/JSON startup 응답 hash, browser/DPR/GPU를 저장한다. hash 수집은 native drag 전에 끝낸다. Playwright에 노출되지 않는 Worker 요청은 inventory 완전성을 보장하지 않는다.
- `resize-following.spec.ts`: 누락/0·1 front, reverse geometry/중복 제출 age, 잘못된 DPR·generation/late observer settle의 합성 회귀 3개.

렌더러 제품 코드·Flutter 소스·기본 backend 정책은 변경하지 않았다. manifest의 buildKind는 의도적으로 notVerified이며, 아래 빌드 실행과 평가 속성을 구분한다.

## 실행 환경과 결과

- Windows, Chromium 151.0.7922.34, AMD Radeon 780M / ANGLE D3D11, **165Hz**, DPI 192(200%), .NET SDK 10.0.400.
- 현재 demo F3, worker-canvaskit-webgl, 기본 resize scheduling. Windows SendInput 240Hz / 600px / TopLeft / reverse / 150ms. reverse 뒤 100ms hold는 motion interval에서 제외한다.
- Release build: **PASS**, 경고 0 / 오류 0. MSBuild 평가: WasmBuildNative=true, RunAOTCompilation 빈 값, net10.0 / browser-wasm. publish/AOT 실행은 검증하지 않았다.
- TypeScript `npm run check`: **PASS**. 합성 회귀: **3 PASS**. 모든 Playwright 프로세스는 20분 제한으로 실행했다.
- 최초 trace-off baseline 3회: stimulus PASS, latency 모두 FAIL, caught-up p95 **66.6 / 96.5 / 84.3ms**. 이 실행에는 served manifest 추가 전 harness가 로드됐으므로 아래 실행과 구분한다.
- 최종 harness trace-off baseline 3회: stimulus PASS, legacy/v2 latency 모두 FAIL, 미도달 0, geometry uncovered 0, >100ms boundary gap 0. 아래 값은 trial별이며 attachment 복사본을 추가 표본으로 세지 않는다.

| trial | caught-up p95 ms | 첫 notification ms | boundary gap max ms | exact observer settle ms | width/height error p95 logical px | 대체 target 수 |
| --- | ---: | ---: | ---: | ---: | --- | ---: |
| 0 | 65.9 | 38.98 | 43.80 | 60.80 | 233 / 234 | 15 |
| 1 | 90.0 | 39.95 | 75.60 | 55.90 | 267 / 267 | 15 |
| 2 | 73.9 | 36.32 | 45.90 | 46.30 | 199 / 200 | 20 |

최종 baseline source identity: `a5a2d28c35680caaf26aeb93c57c78ef3c1fe6cd8f6b811c56694481514f0d79`. 관측 startup asset은 trial별 260 / 259 / 259개다. 상세 URL/hash, source.patch, 원시 clock/native/trace는 아래 artifact에 있다. 후속 보고서 편집으로 현재 checkout hash가 달라질 수 있다.

## 별도 trace-on 진단

동일 조건 3회 stimulus PASS / latency FAIL. caught-up p95 61.4 / 83.7 / 90.5ms. trace off와 교대 A/B한 것이 아니므로 trace 비용이나 개선율을 추론하지 않는다.

아래는 native active 중 시작한 작업을 추적한 trial별 p95(ms). frame은 callbackId, Raster는 sequence로 시작/종료를 짝지었다. mapping/encoding/validate는 active 안에 기록된 duration이다. 서로 다른 모집단이고 UI frame 안에 포함된 비용이므로 합산하지 않는다. 표본은 UI frame trial당 4개, Raster 4/3/3개라 p95=max이며 안정적인 tail 추정치가 아니다.

| trial | UI frame | mapping | encoding | UI validate+copy | Raster replay→submit |
| --- | ---: | ---: | ---: | ---: | ---: |
| 0 | 25.10 | 4.10 | 6.00 | 0.60 | 3.30 |
| 1 | 24.30 | 2.50 | 6.10 | 0.80 | 28.80 |
| 2 | 29.10 | 4.80 | 6.90 | 0.80 | 38.00 |

UI의 긴 동기 작업과 간헐적인 Raster 지연을 모두 조사할 근거다. 이 표만으로 managed frame, GC, GPU, 브라우저 scheduling 중 주원인을 확정하지 않는다. UI validate+copy는 managed→JS 최초 복사를 포함하지 않는다. 기존 frame trace ring의 build/layout/paint/scene 연결, 전체 copy/bytes, picture 재사용 비율은 아직 보강해야 한다.

## 명령과 artifacts

저장소 root에서 실행했다. wrapper는 build/server/native driver/Playwright를 소유하고 종료 시 서버를 회수했다. 최초 명령은 dotnet build, 후속 SkipBuild는 dotnet run --no-build 경로다.

```powershell
$env:DOROTI_FAST_RESIZE_EDGE='TopLeft'
$env:DOROTI_FAST_RESIZE_MOTION='reverse'
$env:DOROTI_FAST_RESIZE_MS='150'
$env:DOROTI_FAST_RESIZE_RUNS='3'
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -FastResize -Port 5188 -ArtifactLabel resize-v2-w0-baseline
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -SkipBuild -FastResize -Port 5188 -ArtifactLabel resize-v2-w0-manifest-baseline
$env:DOROTI_FAST_RESIZE_TRACE='1'
pwsh -NoProfile -File ./Doroti/eng/run-web-playwright.ps1 -Configuration Release -SkipBuild -FastResize -Port 5188 -ArtifactLabel resize-v2-w0-diagnostic
npm.cmd --prefix Doroti/validation/web-playwright run check
dotnet msbuild DorotiDemoApp/web/DorotiDemoApp.Web.csproj -p:Configuration=Release -getProperty:Configuration,RunAOTCompilation,WasmBuildNative,TargetFramework,RuntimeIdentifier
```

합성 회귀는 `Doroti/validation/web-playwright`에서 `npx playwright test tests/resize-following.spec.ts --project=chromium-hardware`를 실행했다. PowerShell Start-Process/WaitForExit(1200000)으로 프로세스 제한을 적용했다.

Artifact root: `Doroti/validation/web-playwright/artifacts/` 아래:

- `resize-v2-w0-baseline/`: 최초 3회 실행.
- `resize-v2-w0-manifest-baseline/`: 최종 harness 3회 실행, source.patch, served hashes.
- `resize-v2-w0-diagnostic/`: trace-on 3회 실행, `stage-summary.json`.
- `wrapper/<동일 label>/`: 실제 build/server/native-driver/playwright stdout/stderr.

## 판정과 남은 W0

| 항목 | 판정 |
| --- | --- |
| 계측 합성 correctness | PASS, 3 tests |
| 실제 baseline harness/기존 presenter·buffer assertions | PASS, 최종 3 trials; 전체 backend correctness를 뜻하지 않음 |
| stimulus | PASS, 실행한 조건에 한함 |
| latency | FAIL; baseline 수집 exit 0과 분리 |
| capturedPresentation | notVerified; log-only이며 화면/scan-out ACK가 아님 |
| flutterComparability | notComparable; 동등 fixture/공통 captured endpoint 미구현 |
| manualAcceptance | notVerified; 기존 사용자 부족 판정 유지 |

W0 완료 조건은 아직 충족하지 않았다. F0/F1/F2 및 Flutter 동등 fixture, frame phase/picture 재사용 계측, build/publish binary 증명 보강, main/UI/Raster clock 정밀도, native outer rect→content viewport 보정과 native→observer 매칭, observer-active 별도 적분, content age p95/idle 비교, pixel marker/capture를 추가해야 한다. nativeGeometry는 notComparable로 남겼다. 현 v2 status는 구현된 notification gate만 판정하며 이 미완료 gate의 PASS를 뜻하지 않는다.

이번 표본은 한 조건의 165Hz 결과다. 60Hz 고정, 전체 edge/duration/motion matrix, DPR/zoom/embedded/monitor/stall/restart는 미실행이다. 후보 실행 전 native→observer/captured error gate와 메모리 예산을 고정해야 한다. W1~W5 및 제품 성능 개선은 아직 수행하지 않았다.
