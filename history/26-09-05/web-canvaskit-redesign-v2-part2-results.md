# Web CanvasKit 재설계 2부 실행 기록

2026-09-05, 사용자 요청으로 B0~B5 재개. 시작 HEAD `6a45c3b`, Git working tree clean.
이전 1부 중단 및 FAIL은 그대로 유지한다. 기본 `auto=document-webgl`, CanvasKit opt-in.

## 실행 범위와 사전 기준

- native drag 최대 10회: B1 진단 1 + B3 교대 비교 6 + B4 F3 capture 2 + 자극/setup 실패 시 예비 1. full matrix/조건별 반복 없음.
- 모든 build/test 프로세스 20분 제한. native 성능 구간에는 build/기능 테스트/대량 PNG 분석을 겹치지 않는다.
- B3 기준: p95 및 interval ≤33.3ms, exact settle ≤50ms, active >100ms gap 0. 3조건 각 1회이므로 안정적 20% 개선을 주장하지 않는다.
- B4는 TopLeft/150ms/reverse baseline/후보 각각 한 번, 동일 WGC endpoint. callback rect와 marker 기반 root/edge/center geometry를 구분하며 callback은 scan-out이 아니다.
- B4 후보 캡처 전 고정: active marker 전부 decode, 새 generation 2개 이상, boundary gap 및 width/height error의 후보 상대 허용치는 baseline + 각각 display interval 1개와 `1200/150 * displayInterval` physical px. 기하학적 center 오차의 허용치는 그 절반이다. origin은 별도 원시 값으로 보고한다. 실제 content landmark/줄바꿈 품질은 PNG 확인과 분리하며 marker center만으로 전체 품질 PASS를 선언하지 않는다.
- 60Hz, monitor 이동, 실제 OS IME, 물리 scan-out 및 사용자 직접 드래그 수용은 자동 검증 대상 밖이며 `notVerified` 유지.

## B0

- isolated Release non-AOT publish 갱신(`p2b0`), correctness 12 PASS. background visibility 자극 1 FAIL; maximize/restore는 완료됐으나 inactive tab이 visible이었다.
- 명시적인 같은 창 tab 생성과 video/trace off 분리는 같은 실패를 재현했다(`p2b0life`, `p2b0life2`). 제품 오류로 판정하지 않는다.
- 설치된 Playwright source의 기본 `Emulation.setFocusEmulationEnabled(true)` 확인. 별도 CDP session에서 false를 보내도 원래 session의 override는 해제되지 않았다(`p2b0life3` FAIL). 격리된 소유 Chromium에 `connectOverCDP(noDefaults: true)`로 연결하는 방식으로 수정했다. `p2b0life4` **1 PASS**, 실제 같은 창 탭의 hidden→visible 및 정확한 geometry 복구 확인. 브라우저도 종료했다.
- warm input 단독 `p2b0warm2` **1 PASS**, 141.8ms <250ms. 이전 256.2ms FAIL은 유지한다. 문서의 `:21`은 현재 `:19`로 이동했고, 이전 두 선택 시도는 `No tests found`로 실제 테스트가 실행되지 않았다. 최종 실행은 파일 + headless project로 정확히 이 테스트 하나를 실행했다.
- `reanalyze-resize-age.mjs`로 기존 원시 54회만 재계산하고 attachment 복사본은 제외했다. sidecar는 `artifacts/p2b0/previous-age-sidecar.json`. 원래 absolute age max와 차이 0, 원래 54 FAIL 보존. trial active-age p95 중앙값 baseline 64.07ms / 기존 후보 50.80ms. pre-motion idle 최대 약 248ms를 새 active age와 섞지 않는다.

## B1: actual managed phase 진단

`p2b1`: TopLeft/150ms/reverse, trace on/capture off, native **1회**. 자극 PASS, notification p95 44.4ms로 latency FAIL. callback 시작이 motion 안에 있는 7개 scene을 scene sequence로 연결했다. `analyze-resize-phases.py`와 `artifacts/p2b1/phases.json`에 개별 표본을 보존했다.

| 비용 | 중앙값 ms | p95 ms |
| --- | ---: | ---: |
| UI frame 전체 | 19.2 | 22.0 |
| build | 5.6 | 6.1 |
| layout + compositing bits | 3.6 | 4.5 |
| paint | 2.5 | 5.1 |
| scene 구성 + map/encode/interop | 6.3 | 10.3 |
| 위 scene 비용 중 map | 1.2 | 6.2 |
| 위 scene 비용 중 encode | 3.2 | 4.2 |
| scene 잔여 비용(분해값 차감) | 0.4 | 0.7 |
| scene-encoded → send | 1.4 | 2.9 |
| UI send → Raster receive | 0.1 | 0.2 |
| Raster replay → submit | 3.3 | 4.1 |
| Raster terminal → UI 처리 | 17.5 | 20.3 |

managed phase는 `RecordedAtMicroseconds`만 사용했다. stage 간 시각은 browser clock 해상도 한계가 있으며 통계의 포함 관계 때문에 각 중앙값을 더하지 않는다. active semantics flush는 지연되어 독립 측정 표본이 없고 비용 0으로 해석하지 않는다. paragraph interop 표본은 이 7개 callback에서 0이다. ACK 지연을 전송 비용으로 분류하지 않는다.

## B2: scene 인코딩의 중복 UTF-8 변환 제거

scene phase에서 같은 문자열을 occurrence마다 Unicode 검사하고 unique value마다 정렬·쓰기용으로 두 번 UTF-8 변환하던 작업을 **unique value당 strict 변환 1회**로 통합했다. 생성한 bytes를 canonical byte sorting과 출력에 공유하고 장면 밖에 보관하지 않는다. resource/command 검증과 wire bytes는 유지한다.

- DisplayList contract **PASS**: 6330 bytes, SHA-256 `66412CCB5E02519BBD8C11ECAB5E63CE914E2DB745F6D51110BBD03F89CCBE42` 유지. 반복 non-BMP 문자열의 UTF-8 순서·dedup·round-trip과 text/family/locale의 잘못된 surrogate 거부 회귀 추가.
- 실제 isolated Release non-AOT publish 갱신(`p2b2`), 브라우저 golden·transfer terminal 및 F0/F1/F2 direct/cache/bitmap 6종·resize·restart **5 PASS**.
- TypeScript 확인 **PASS**. 새 retained wire, credit 2, 단일 Worker, AOT 재시도, FPS 제한은 추가하지 않았다.
- 이 변경의 단독 F3 개선량을 입증하지는 못했다. B3는 수정된 공통 encoder를 사용하는 **같은 binary의 baseline 옵션과 후보 조합** 비교이며, encoder 수정 전후의 독립 A/B가 아니다.

## B3: 최소 대응 비교

Chromium **151.0.7922.34**, AMD Radeon 780M / ANGLE D3D11, DPI 192(200%), **165Hz**. Release non-AOT, trace/capture off. 계획된 3조건에서 baseline→후보 각 1회, 총 **6회**. 자극 6 PASS / latency 6 FAIL, 미도달 0, geometry uncovered 0, active >100ms gap 0.

| 조건·옵션 | p95 ms | p99/max ms | interval p95 ms | exact settle ms | 첫 front ms | active age p95 ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| TopLeft/150/reverse baseline | 63.6 | 71.1 | 40.9 | 27.6 | 36.33 | 65.75 |
| TopLeft/150/reverse 후보 | 45.2 | 46.0 | 23.2 | 20.3 | 40.45 | 50.00 |
| Left/150/reverse baseline | 63.8 | 69.4 | 42.6 | 24.3 | 33.90 | 64.97 |
| Left/150/reverse 후보 | 61.0 | 65.5 | 34.2 | 38.5 | 35.57 | 67.00 |
| Bottom/600/expand baseline | 56.8 | 62.4 | 44.2 | 26.3 | 29.50 | 61.17 |
| Bottom/600/expand 후보 | 41.3 | 46.0 | 27.0 | 24.4 | 32.27 | 45.90 |

후보의 p95 개선은 각각 약 28.9% / **4.4%** / 27.3%다. 세 조건 모두 33.3ms 목표 미달이며 Left는 interval도 미달하고 active age·settle이 퇴행했다. 첫 front도 세 조건 모두 소폭 늦어졌다. 안정적 20% 개선 또는 기본값 채택 근거가 아니다.

geometry 적분(logical px·ms)은 TopLeft width/height 16694.6/16689.9→13837.0/13860.3, Left width 16402.8→15088.2, Bottom height 11242.6→8328.9였다. 상세 p99/max·coverage·age는 `artifacts/p2-summary.json` 및 `p2b3-{tl,l,b}-{base,candidate}` raw report에 있다.

## B4: F3 캡처와 화면 품질

첫 `p2b4-base`는 native 자극 후 capture ring **39 frame drop**, driver exit 2로 실패했다. PNG 97개와 원시 native JSON을 보존했다. setup 전 실패나 정상 capture로 바꾸어 쓰지 않는다.

원인은 `--capture-only`가 `visualOraclesEnabled=false`를 기록하면서도 `f6r || visualOracles` 조건으로 Windows용 grid/shape 분석을 계속 수행한 것이었다. F6-R의 monitor capture/자극과 oracle 활성화를 분리하여 명시적인 capture-only를 지키도록 고쳤다. 기본 Windows visual oracle 옵션(true)은 유지했고, 수집 오류·drop의 실패 기준과 native 자극은 변경하지 않았다.

예비 1회로 baseline을 재수집한 뒤 후보를 1회 캡처했다(`p2b4r-base`, `p2b4r-candidate`). 두 실행 모두 ring/encoder/capture drop·error **0**, `visualAnalyzed=false` 확인, 자극 PASS. **B1 1 + B3 6 + 첫 capture 실패 1 + 유효 capture 2 = native 총 10회**. 이후 추가 드래그 없음.

PNG 재분석 중 첫 setup frame이 1246×606인 이전 epoch를 담고, 마지막 pre-motion frame은 실제 1254×614임을 확인했다. chrome offset을 첫 PNG에서 구하면 8px 오류가 생기므로 마지막 pre-motion decoded frame을 사용하도록 두 native harness와 sidecar를 수정했다. 저장된 전체 capture entries와 별도로 계산한 new-generation gap·calibration이 일치함을 확인했다. 새 드래그는 하지 않았고 원래 raw report의 `captureFollowing` 값은 보존했다. 최종 수치는 `capture-sidecar.json` 및 `artifacts/p2-capture-comparison.json`을 따른다. 초기 `capture-first-reference.json`은 교정 전 분석이며 수용 근거에서 제외한다.

| WGC captured endpoint | baseline | 후보 | 사전 상대 gate |
| --- | ---: | ---: | --- |
| active marker decode | 23/23 | 24/24 | PASS |
| 새 generation | 3 | 3 | PASS |
| boundary gap p95 ms | 52.08 | 58.94 | **FAIL** (허용 증가 6.06ms) |
| width error p95 physical px | 533 | 566 | PASS (허용 증가 48.48px) |
| height error p95 physical px | 535 | 568 | PASS (허용 증가 48.48px) |
| 기하학적 center X p95 physical px | 200.5 | 283.0 | **FAIL** (허용 증가 24.24px) |
| 기하학적 center Y p95 physical px | 201.5 | 284.0 | **FAIL** (허용 증가 24.24px) |
| origin X/Y p95 physical px | 101/101 | 100/100 | 별도 관측, 전체 품질 PASS 아님 |

- 후보 `frame-000051.png`에서 작은 이전 프레임과 오른쪽·아래 공백이 보였고, 최종 `frame-000146.png`에서는 1254×614로 맞아졌다. 화면 중심은 독립 F3 landmark가 아니라 marker의 raster geometry에서 계산한 값이다.
- 두 PNG에서 고정 폭 ImageFilter 문구의 2줄은 유지됐다. 이 native 범위는 반응형 F3 텍스트의 줄바꿈 전환 전체를 입증하지 않는다. dynamic wrap 수용은 미검증이다.
- Chromium 번역 popup이 상단 일부를 가렸다. B3/B4에서 동일하게 시도한 `--disable-features=Translate`는 이를 막지 못했고 효과 없는 native launch override는 제거했다. popup 부위의 전체 시각 품질은 판정하지 않는다. 재측정은 하지 않았다.
- 이전 F2 raw PNG 재분석은 blue center가 자기 raster의 중심과 정확히 일치함을 확인했다(내부 X/Y 잔차 0). 종전 Doroti 262.5px / Flutter 178.5px center 오차는 유지하며 크기·origin 추종 문제로 구분했다. 원시 text-row bounds도 sidecar에 저장했지만 dynamic wrap equivalence PASS로 확대하지 않는다.
- capture-on notification p95 98.0/77.2ms는 진단 corpus이며 B3 성능 corpus와 합치지 않는다. WGC callback은 물리 scan-out이 아니다.

## B5: 최종 판정과 종료

| 범위 | 판정 |
| --- | --- |
| correctness | **PASS**, 이번 실행 범위: B0 14개 + B2 5개 브라우저 검사, managed wire 계약, TypeScript. 초기 harness FAIL은 보존 |
| stimulus | B1/B3/재수집 B4의 9개 정상 report에서 **PASS**, 첫 capture는 수집 실패로 별도 보존 |
| latency | **FAIL**, B3 후보 p95 45.2/61.0/41.3ms >33.3ms |
| capturedPresentation | **FAIL / partial**, 수집·decode 정상이나 상대 gap·center gate 미달 |
| Flutter comparability | 기존 F0~F2 단일 corpus만 유지. F3 Flutter 비교 없음, 일반화 불가 |
| manualAcceptance | **notVerified**, W5 완료·기본값 승격 금지 |

자동 실행·코드 수정·자료 재분석·판정 기록은 완료했다. 60Hz, monitor 이동, 실제 OS IME, 물리 scan-out, 직접 Right/Bottom/Left/TopLeft·반전·줄바꿈 수용은 사용자의 별도 확인이 필요하다. 성능과 수용이 완료된 것으로 표시하지 않는다. 소유 5188/5189 listener, static server, native driver, lifecycle Chromium 잔류 없음 확인.

## 재현 자료와 build identity

- 최종 앱 publish: `.doroti/publish/web-Publish-Release/wwwroot`. 실제 non-AOT 재빌드 이후에만 B3/B4에서 `-SkipBuild` 사용.
- B1 source: `4626deeb3fe4740fffad76547d5585e33bfedef8d032a7658e8d4357c55a2642`.
- B3 6회 source는 하나: `4605bf7f2dc165d88b07d552f847517b0802d0771beef370c62715adf553a572`.
- B4 유효 pair source: `b797cb35569c076906572b926040a67a31b6206cadb6a4abb7dc70f194b145bd`.
- B3/B4에서 실제 served DisplayList WASM SHA-256: `7b74f7345e988310b32ffff514c1274a004517be029037588f9c7ac6ee7f9db5`. B1은 수정 전 `ce9a104818119134b39d44c741119e3151a98a9ff1bdf83a2150fe155f2e1d6e`.
- B3 native driver SHA-256: `8ab6759143ccf109f96e0d26b538678621a3bc0d169fcff298bfc0afb1f30bc8`. capture-only 수정 후 B4: `d9df2dab15a24f8485d446b54035fb6fce25338bb05c18ecabea417c49fe5b8e`. log-only 자극과 capture 진단의 binary 차이를 숨기지 않는다.
- 전체 served inventory와 source patch는 각 raw report에 포함. Worker startup에서 관측되지 않은 응답까지 inventory 완전성을 보장하지 않는다. 이후 분석 스크립트·계측 보정·문서 수정으로 현재 checkout source identity는 측정 시점과 다르지만 앱 binary는 같다.
- `Doroti/validation/web-playwright/artifacts/wrapper/<label>/`에 20분 제한 build/test/server 로그, `artifacts/<label>/test-results/`에 원시 자료. 최종 TypeScript 및 `git diff --check` PASS. calibration 변경은 저장 corpus로 검증했으며 추가 native 실행은 하지 않았다.
