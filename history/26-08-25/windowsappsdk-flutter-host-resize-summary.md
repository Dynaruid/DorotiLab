# Windows App SDK Flutter-style host 전환과 F6-R 분석 요약

- 기록일: 2026-08-25
- 원본: 루트 `work2.md`에 누적됐던 완료 작업, 실험, 검증 결과와 실패 경계
- Flutter source 기준: `reference/flutter-master` commit `56b8e1a851a594b1a154f8ea93270807dab22b9a`
- Windows App SDK 기준: exact `2.4.0`, self-contained unpackaged
- 문서 분리일: 2026-08-25
- 최종 역사 상태: **F0~F7 자동/계약 범위와 F7 사용자 수용은 PASS, F8~F10 구현·자동 smoke는 완료, F6-R native cadence와 exact-grid coverage는 FAIL, FG는 notVerified**

이 문서는 Windows 제품 host를 Flutter-style raw HWND 구조로 전환한 과정과 2026-08-25까지 수행한 high-speed resize 분석을 압축한 역사 기록이다. 작성 과정에서 build나 runtime 검증을 다시 실행하지 않았으며, 앞으로 할 일은 루트 `work2.md`에만 둔다.

## 1. 채택한 제품 구조

기존 fixed work-area envelope, custom non-client, app-owned window region, Arm N dual-front 구조를 Windows가 소유하는 표준 top-level과 하나의 child render HWND 구조로 교체했다.

```text
standard Win32 top-level HWND
└─ Doroti child view HWND
   ├─ physical client metrics authority
   ├─ pointer / keyboard / IME / UIA boundary
   └─ raster/presenter target
```

고정한 ownership은 다음과 같다.

- Windows가 caption, border, move, resize, Snap, system menu를 소유한다.
- top-level `WM_SIZE`가 child를 client 전체로 배치하고 child physical pixels가 framework metrics authority다.
- platform STA, framework MTA, raster MTA, frame-clock MTA를 분리한다.
- WndProc는 metrics를 latest mailbox에 게시하고 즉시 반환한다.
- exact generation/extent가 아닌 frame은 exact present로 승인하지 않는다.
- first exact swap 뒤에만 top-level을 표시한다.
- monitor work-area는 placement에만 사용하고 render capacity로 쓰지 않는다.
- Arm N과 MAUI는 rollback adapter로 보존한다.

## 2. F0~F10 구현·검증 경계

| 범위 | 역사 상태 | 보존할 판정 |
|---|---|---|
| F0-A/F0-V Flutter protocol lock | **PASS** | pinned source의 parent/child HWND, metrics, raster surface, first-frame 계약과 tracked source-only validator 확인 |
| F1 bootstrap | **PASS** | Windows App SDK 2.4 self-contained bootstrap, ANGLE ABI, platform/raster preflight 확인 |
| F2 HWND tree | **PASS** | standard top-level + child view의 same-STA structural gate 확인 |
| F3 metrics/display | **PASS** | child-client physical authority, immutable generation, 4-DPI matrix 확인 |
| F4 ANGLE/EGL fixture | **PASS** | dedicated raster thread의 child-HWND EGL recreate/swap와 context-loss recovery 확인 |
| F5 handshake | **fixture PASS / product wait 금지** | Flutter 100ms poll contract는 fixture에서 확인했지만 product native geometry gate로 사용하지 않음 |
| F6 scheduler | **기본 계약 PASS** | latest-only queue, native DWM timing, causal callback/raster/swap/present와 deterministic refresh matrix 확인 |
| F7 input/IME/accessibility | **PASS** | child HWND pointer/capture/focus/cursor, keyboard, IMM32, clipboard, UIA 자동 gate와 사용자의 Korean IME/Narrator/Accessibility Insights 포함 직접 확인 수용 |
| F8 lifecycle/recovery | **구현 완료 / 자동 smoke PASS** | lifecycle manager와 recovery wiring 완료. mixed-DPI/monitor/sleep/RDP 물리 matrix는 deferred |
| F9 target/runner/package | **구현 완료 / build·publish·launch PASS** | WindowsAppSdk target/runner/package와 rollback 선택 연결. 일부 negative deployment와 MAUI live launch는 notVerified |
| F10 default cutover | **구현 완료 / 자동 smoke PASS** | CLI, Demo, target, template의 Windows 기본을 FlutterEmbedder로 전환. release 확정과 legacy cleanup은 FG 뒤로 보류 |
| FG 제품 acceptance | **notVerified** | F6-R 실패로 physical/visible 전체 acceptance에 진입하지 않음 |

F4는 ANGLE/EGL fixture 계약 증거다. 이후 실제 product child HWND에 연결한 ANGLE/EGL spike의 visible resize 실패를 대체하지 않는다.

## 3. 최초 F6-R 원인과 geometry ownership 복구

초기 four-edge baseline `f6r-20260825-155130-a3ec8278daa24e0bbac4e6cbefb02b4d`에서 cursor-edge lag p95는 128~148px였다.

원인은 다음 prepared-front protocol이었다.

- `WM_WINDOWPOSCHANGING` proposed rect를 current/prepared extent로 rewrite했다.
- mouse-up에서 저장한 geometry를 replay했다.
- framework metrics drain을 이전 present terminal까지 막았다.
- provisional raster가 native admission을 기다렸다.

이 구조에서는 약 20~35ms의 framework/present 시간이 native border의 64~160px lag로 증폭됐다. 따라서 `WINDOWPOS` rewrite, mouse-up replay, provisional raster admission wait, present-terminal framework drain lock을 제거했다.

renderer를 50ms 지연한 geometry-only run `f6r-20260825-172851-04b19fe842054fe5a2545907a3cc1553`은 네 edge p95 16px, max left/right 20px와 top/bottom 32px로 PASS했다. renderer readiness가 native geometry admission 조건으로 되돌아가지 않았다는 scoped 증거다.

## 4. presenter 실험과 폐기 경계

### Product ANGLE/EGL spike

실제 product child HWND의 ANGLE/EGL window surface는 build와 표시에는 성공했지만 high-speed active resize에서 exact EGL surface가 계속 stale 되는 동안 큰 black band를 만들었고 active exact present가 없었다. 단순 EGL surface recreate/swap만으로 retained visible continuity가 생기지 않는다고 판정해 product F6-R 후보에서 제외했다.

### DComp full-frame stretch

DComp `SetSourceSize`를 새 창 크기에 맞춘 transient는 텍스트, 카드, 원형을 X/Y로 비균일 확대하고 content가 창 밖으로 넘쳐 보였다. 사용자가 캡처의 scale/overflow를 직접 지적했고 즉시 revert했다. 이 방식은 재사용 금지다.

### DComp safe-background reservoir

다음 provisional presenter를 구현했다.

- visible DComp root/front는 하나만 유지한다.
- last exact scene body는 X/Y 1:1로 보존한다.
- 새로 노출된 영역은 known-safe background row/column으로 채운다.
- initial client + 768px bounded reservoir를 사용하며 virtual-desktop permanent capacity를 제거했다.
- safe fill은 `transientFrontUpdated`로 기록하고 exact present로 계상하지 않는다.

사용자는 이 binary를 육안으로 보고 거의 즉시 반응한다고 확인했다. 이는 scoped 체감 continuity 개선이며 exact-content 또는 FG PASS가 아니다.

## 5. four-edge 자동 결과

bounded reservoir를 사용한 capture `f6r-20260825-181711-96eb57df3031458d89d334e234b59b75` 결과는 다음과 같다.

| Edge | cursor-edge p95/max | uncovered detector | final exact |
|---|---:|---:|---:|
| Left | 34/39px | 0px | 24.8ms |
| Top | 39/52px | 0px | 40.0ms |
| Right | 32/40px | 한 frame 30px | 20.9ms |
| Bottom | 41/56px | 0px | 39.5ms |

165Hz에서 `600px/150ms` 입력의 1-refresh 이동량은 약 24px다. 네 방향 모두 p95 gate를 넘었고 top/bottom은 max 2-refresh 조건도 넘었다. 모든 방향의 final lag는 0px였다.

## 6. `gap=0` 오판과 진단 격자

기존 analyzer의 `contentEdgeGap`은 app-bar와 비슷한 색이 client 끝까지 존재하는지만 봤다. safe-background fill도 content로 인식하므로 `gap=0`은 exact raster coverage 증거가 아니었다.

데모 body에 32 logical-pixel 진단 격자를 추가한 left/top run `f6r-20260825-182301-16ac9263701941ed85fe4e43852ce7f3`에서 uncovered gap은 0px였지만 격자는 old exact extent에서 끊기고 뒤에는 단색 reservoir가 이어졌다. left/top native p95는 각각 34px였고 top은 final exact 66.5ms와 causal gap 2를 기록했다.

따라서 판정을 다음처럼 분리하기로 했다.

- native geometry cadence
- black/transparent 영역을 보는 uncovered edge gap
- last valid grid/scene에서 client edge까지의 transient grid tail
- grid X/Y spacing과 non-uniform scale
- actual child generation/extent와 일치하는 exact present
- mouse-up 뒤 final convergence

마지막 foreground grid 대비 변경은 source에만 있으며 build/runtime는 notVerified다.

## 7. 효과 없거나 악화돼 revert한 실험

- resize 중 frame-clock `DwmFlush` 생략: `f6r-20260825-181447-f4bec7878cd142d1a7d8fb15185cc389`에서 p95/max lag 52/72px, native max interval 16.6ms로 악화.
- native window 60fps 제한 제안: `4px/ms` 입력에서 약 67px step을 만들고 Windows geometry ownership을 다시 앱 admission에 묶으므로 구현하지 않음.
- framework metrics/layout만 60Hz latest-only coalesce: `f6r-20260825-181947-0c0580c678c14ef2a364e83806dae715`에서 p95 left/top/right/bottom 33/43/35/41px로 개선 없음. revert.
- causal event마다 HWND/cursor geometry 조회 제거: observer effect는 줄였지만 left/top p95는 34px로 남아 계측 최적화만으로 해결되지 않음.

platform resize dispatch는 대부분 p99/max 약 0.2ms 이하였고 queue max 1, stale exact present 0이었다. 현재 증거만으로 WndProc CPU 시간을 남은 32~41px lag의 주원인으로 결론내리지 않는다.

## 8. observer와 validator 보강

- F6-R analyzer에 active cursor-edge p50/p95/max, native edge update interval, final exact latency와 platform dispatch gate를 추가했다.
- capture/log-only provenance를 분리했다.
- native observer가 첫 실패 뒤 window restore로 geometry를 바꾸던 retry 오류를 수정하고 stationary opposite edge, required distance, reverse excursion을 검증하게 했다.
- validator의 다중 observer retry를 제거해 한 trace에 여러 resize episode가 섞이지 않게 했다.
- causal trace geometry snapshot은 `presented`에만 남기고 dedicated native sampler와 QPC로 join하도록 경량화했다.
- `contentEdgeGap`은 `uncoveredEdgeGap` 의미로 교정 중이며 grid-tail oracle은 아직 미구현이다.

## 9. 보존할 acceptance 경계

- 사용자의 “육안상 거의 즉시” 관찰은 safe-fill transient의 scoped manual evidence다.
- build, source contract, process 생존, exact final present와 자동 input/IME/UIA PASS는 high-speed visible continuity를 대신하지 않는다.
- corner, shrink/reverse 3회, 300ms/slow, size/DPI/refresh matrix는 notVerified다.
- mixed-DPI monitor 이동, sleep/resume, RDP, negative deployment, MAUI rollback live launch도 남아 있다.
- F6-R과 FG는 FAIL/notVerified다.
- FG 전체 PASS와 사용자 최종 제품 수용 전에는 Arm N/MAUI rollback 삭제, legacy cleanup, release 확정을 하지 않는다.

## 10. 재개 지점

새 active 계획은 루트 [`work2.md`](../../work2.md)에 있다. exact resume point는 다음 순서다.

```text
R0 grid oracle correction
→ R1 native cadence factorial A/B
→ R2 single-front transient scene lifecycle
→ R3 content/GPU bottleneck correction
→ R4 paired matrix
→ FG physical/visible acceptance
```

> 문서 성격: 2026-08-25까지 완료했거나 폐기한 Windows App SDK Flutter-style host/F6-R 작업의 역사 요약. 새로운 active plan이나 제품 완료 선언이 아니다.
