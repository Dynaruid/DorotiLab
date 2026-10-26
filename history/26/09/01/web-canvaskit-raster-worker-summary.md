# Web CanvasKit UI/Raster Worker 전환 요약

- 보관일: 2026-09-01
- 실행 기록 기준일: 2026-08-31
- 대상: `Doroti.Host.Web`, `Doroti.Graphics.DisplayList`, Web Playwright, Web package/consumer 계약
- 최종 상태: `PARTIAL / notQualified`
- 제품 기본값: `auto=document-webgl`
- 신규 backend: `worker-canvaskit-webgl` experimental opt-in

## 결론

Web main thread, UI Worker, Raster Worker를 분리하고 CanvasKit WebGL2가 transferred visible canvas를 직접 소유하는 `worker-canvaskit-webgl` 경로를 구현했다. UI Worker는 managed UI와 CPU-only CanvasKit text/resource service를, Raster Worker는 typed DisplayList decode와 GPU raster/present를 담당한다.

구조 분리, DisplayList v2, resource replay, direct resize, package asset/provenance, clean packed consumer와 주요 browser 자동 검증은 동작했다. 그러나 context recovery 10회 계약과 Web Skia dependency 0 조건이 실패했고, current Skia 대비 전체 pixel/text/filter differential, 성능 qualification, 장시간 churn 및 실제 입력·IME·접근성·scan-out 검증도 끝나지 않았다.

따라서 완료 정의는 `FAIL`, 전체 상태는 `PARTIAL / notQualified`다. `auto`는 계속 `document-webgl`을 선택하며 forced CanvasKit mode는 silent fallback 없이 fail-closed로 유지한다.

## 구현된 범위

- `canvaskit-wasm@0.42.0` default variant를 exact lockfile dependency로 고정하고, Host.Web source build의 allowlist·SHA-256 provenance·Static Web Asset·nupkg/buildTransitive 검증을 추가했다.
- source SDK build/pack은 npm을 사용할 수 있지만, 배포된 package consumer는 Node/npm 없이 restore/build/publish되도록 경계를 유지했다.
- little-endian typed `Doroti.Graphics.DisplayList` v2 schema와 managed/browser/Raster Worker encoder·validator·decoder를 추가했다.
- UI↔Raster 전용 `MessageChannel`, current+latest mailbox, transferable buffer pool, scene receipt/terminal ledger, resource journal/replay와 visible canvas lease lifecycle을 구현했다.
- UI Worker에는 CPU-only CanvasKit paragraph layout/metrics service를, Raster Worker에는 hardware WebGL2 CanvasKit renderer와 Embind object accounting을 배치했다.
- basic geometry, paint/path/image/paragraph, layer/filter/shader와 top-level direct two-pass runtime-effect image filter replay를 구현했다.
- logical CSS geometry는 main DOM이, physical backing과 GPU surface는 Worker가 소유하도록 resize 경계를 고정했다.
- exact scene을 같은 GPU context의 staging surface에 완성한 뒤 visible backing에 commit하는 present transaction을 구현했다.
- scene 단위 font collection 공유, 최대 8-slot GPU image-filter surface pool, stale Raster session 차단과 Worker replacement 시 resource replay를 추가했다.
- CanvasKit topology, stall, DPR2, replacement, malformed protocol, DisplayList, resize/input 회귀를 canonical Playwright wrapper에 연결했다.

## CK0~CK9 판정

| Gate | 판정 | 확인된 범위 | 남은 조건 또는 실패 원인 |
| --- | --- | --- | --- |
| CK0 | `PARTIAL` | CanvasKit 0.42.0/default pin, same-origin asset, hardware Raster-only WebGL owner, package 7/7과 clean consumer zero-Node/npm을 확인했다. | cold source `npm ci`, current renderer baseline/startup/heap corpus, context-loss 10회와 전체 negative input matrix가 없다. |
| CK1 | `PARTIAL` | typed DisplayList, 6330-byte v2 managed golden, browser 2/2 계약과 malformed recovery가 통과했다. | pre-wire `HostPayload`, same-worker Skia loopback과 old-direct pixel golden이 남았다. |
| CK2 | `PARTIAL` | UI-owned CanvasKit text service, metrics hash, logical resource registry/journal과 restart replay를 구현했다. | 전체 문자열·wrap·ellipsis·hit-test current-Skia differential과 stale-handle fixture가 없다. |
| CK3 | `PASS` | main/UI/Raster .NET owner `0/1/0`, UI/Raster CanvasKit `1/1`, Raster WebGL owner `1`, direct channel, stall 중 UI/input 진행, bounded mailbox와 terminal/receipt를 확인했다. | CK3 자체 자동 blocker는 없다. |
| CK4 | `PARTIAL` | v2 opcode 검증, representative render, no-blank sample과 top-level runtime filter가 동작했다. | strict pixel/all-opcode corpus가 없고 composed/nested runtime-effect filter는 `DOROTIWEB032`로 fail-closed다. |
| CK5 | `FAIL` | Raster Worker 3회 replacement, resource replay, lease terminal, malformed recovery와 context restore 1회가 통과했다. | context recovery가 fatal restart budget 3회를 함께 소비하므로 동일 session 10회 recovery gate를 통과할 수 없다. |
| CK6 | `PARTIAL` | continuous resize/wheel no-blank, viewport A-B-C, pinch zoom, startup과 DPR2 geometry를 확인했다. | DPR 1.25/1.5, native border/maximize/restore, fast/long resize 3회 정량 gate와 direct 비교가 없다. |
| CK7 | `FAIL` | Host asset/provenance, nupkg/buildTransitive, clean consumer와 tamper/missing-license negative gate가 통과했다. | Web publish에 SkiaSharp/native 및 Doroti Skia dependencies가 남아 Web dependency 0 조건을 실패했다. |
| CK8 | `PARTIAL` | buffer pool, journal, shared font collection, bounded filter-surface pool과 object counter를 구현했다. | 주요 object cache, shader warmup/negative cache, retained filter identity, 비교 성능과 30분 churn이 없다. |
| CK9 | `notVerified` | rollback mode와 forced-mode fail-closed 정책을 유지했다. | CK5/CK7 실패와 전체 qualification 미완료로 기본값 승격을 하지 않았다. |

계획은 앞 gate가 `PASS`일 때만 다음 단계로 진행하도록 정의했지만, topology와 package feasibility를 확인하기 위해 CK0/CK1이 `PARTIAL`인 상태에서 후속 experimental implementation까지 진행했다. 이 계획 이탈은 CK0/CK1의 판정을 바꾸지 않는다.

## 자동 검증 기록

- DisplayList managed contract: `PASS`, 6330 bytes, SHA-256 `66412CCB5E02519BBD8C11ECAB5E63CE914E2DB745F6D51110BBD03F89CCBE42`.
- DisplayList browser matrix: `2/2 PASS`.
- CanvasKit topology/lifecycle suite: `5/5 PASS`.
- `worker-canvaskit-webgl` 전체 headless suite: `16 PASS / 5 SKIP / 0 FAIL`, 약 3.8분.
- Release Web wrapper와 browser-wasm Target, Qt, MAUI, Windows host regression build: warning/error 0으로 `PASS`.
- TypeScript check, FCR-3/4/5/6/7, resize-contract와 당시 `git diff --check`: `PASS`.
- hardware WebGL2 owner는 Google/AMD ANGLE Radeon 780M D3D11로 기록됐고 software fallback은 `false`였다.
- continuous resize sample 221개, target/front generation `82/82`, scene `254/254`, failed 0, outstanding transfer buffer 0이었다.
- wheel 60/60 commit의 p95는 53.1 ms, max는 66.6 ms였다.
- context restore 1회와 Raster replacement 3회 뒤 exact front, resource replay와 lease terminal을 확인했다.
- DPR2에서 logical CSS `1080x720`, physical backing `2160x1440`, CSS transform 없음이 확인됐다.

이 결과는 자동 submit/DOM/screenshot/counter 범위이며 physical compositor/monitor scan-out, 실제 사용자의 입력 경험이나 접근성 acceptance를 증명하지 않는다.

## Package와 consumer 계약

다음 7개 `0.2.0-beta` package를 dependency 순서로 Release pack했고 모두 성공했다.

- `Doroti.Graphics.DisplayList`
- `Doroti.Runtime`
- `Doroti.Ui`
- `Doroti.Skia.RuntimeEffects`
- `Doroti.Skia.Rendering`
- `Doroti.Hosting`
- `Doroti.Host.Web`

새 consumer와 격리된 NuGet cache에서 Node/npm 실행을 실패시키는 poison shim을 `PATH` 앞에 둔 상태로 restore/build/publish했고, 세 단계 모두 성공했으며 poison invocation은 0이었다. 다섯 CanvasKit asset과 생성된 Doroti JavaScript는 source output, Host.Web nupkg와 consumer publish에서 byte/SHA가 일치했고 nupkg에 `node_modules`는 포함되지 않았다.

변조 negative gate도 동작했다.

- `canvaskit.js` 1-byte 변조: build가 `DOROTICK102`로 실패했다.
- `LICENSE` 제거: build가 `DOROTICK101`로 실패했다.
- 두 경우 모두 consumer Node/npm invocation은 0이었다.

단, source pack은 warm npm state에서 실행했다. cold `npm ci --ignore-scripts`, registry/network capture와 trimmed browser-wasm consumer publish는 검증하지 않았다.

## TextField 후속 수정

### 공백 뒤 text와 공통 text-run

`WW WW`가 `[0,3)`까지만 layout되어 공백 뒤 단어가 보이지 않는 문제를 수정했다. unconstrained CanvasKit paragraph 측정 뒤 intrinsic width를 logical pixel 위로 올리고 line range, hard break와 `didExceedMaxLines`가 유지되는지 bounded retry로 검증하도록 바꿨다.

또한 마지막 `TextStyle` 하나로 평탄화하던 경로를 없애고 family fallback, weight/slant, spacing/height/locale, background/decoration, shadow, font feature/variation을 immutable run list로 보존했다. UI CanvasKit 측정과 Raster CanvasKit 재구성은 동일 normalized run list를 사용하며 이 wire 변경을 DisplayList v2에 반영했다.

Release Web build, managed/browser DisplayList v2 계약과 bundled Chromium headed TextField regression은 `PASS`했다. 이 판정은 해당 text-run 자동 회귀만 다루며 실제 한글 IME와 전체 current-Skia differential은 계속 `notVerified`다.

### 캐럿 Y 좌표

CanvasKit adapter가 `BoxHeightStyle.strut` 요청에도 tight grapheme bounds를 반환해 캐럿이 약 2 logical px 위로 이동하던 문제를 수정했다. UI service snapshot과 Raster metrics hash에 strut top/bottom을 분리 보존하고, strut 요청에만 strut bounds를 사용하도록 했다.

TypeScript, FCR-7, Release Web build, 두 renderer의 headed caret pixel regression과 CanvasKit topology/lifecycle suite는 `PASS`했다. 수정 후 실제 한글 IME 입력과 사용자 시각 acceptance는 여전히 `notVerified`다.

## 남은 작업과 재개 조건

- context recovery를 fatal/crash restart budget과 분리하거나 same-worker GPU recovery로 바꾸고 동일 session 10회를 검증한다.
- qualification과 rollback 결정을 거친 뒤 Web graph/nuspec/publish에서 SkiaSharp/native/Doroti Skia dependency를 제거하고 trimmed publish를 다시 확인한다.
- pre-wire `HostPayload`를 제거하고 동일 scene bytes의 current Skia↔CanvasKit strict pixel/text/filter differential corpus를 만든다.
- composed runtime-effect filter, retained filter identity/cache, bounded cache eviction과 baseline recovery를 구현·검증한다.
- cold npm acquisition, lock/integrity/variant 변조, CSP와 runtime asset/network negative fixture를 추가한다.
- direct renderer 대비 startup/resize/handoff/long-task/memory를 같은 session에서 3회 비교하고 30분 churn을 실행한다.
- 실제 방향별·모서리 border drag, maximize/restore, monitor DPR 이동, 60/120 Hz, precision trackpad, 한글 IME/긴 Backspace, screen reader, crash 직후 focus/editing과 compositor scan-out을 검증한다.

이 조건이 완료될 때까지 `worker-canvaskit-webgl`은 experimental opt-in이고 `auto=document-webgl`이 제품 기본값이다.
