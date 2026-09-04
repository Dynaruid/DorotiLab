# Windows Vulkan/Acrylic 창 조절 해결 및 작업 종료 요약

- 정리일: 2026-09-05
- 원본: 저장소 루트 `problem.md`, `work.md` (0~33.7절). 두 문서를 이 요약으로 통합하고 원본은 삭제했다.
- 최종 판정: **사용자 관측 환경의 Windows 창 조절 PASS-observed**, 자동 검증 PASS-partial.
- 사용자 확인: “이제 윈도우 창조절은 잘되는거 같다”. 기존 `work.md` 33.7의 “전부 잘된다” 확인에 이어 현재 결과를 재확인하고 이력 정리를 요청했다.
- 기본 presenter는 `AngleD3D11`, Vulkan은 명시적 opt-in을 유지한다. 이번 정리는 기본값 변경이나 전체 GPU/DPI/주사율 조합의 안정 버전 승격이 아니다.

## 문제와 해결 과정

최초 direct Vulkan WSI에서는 exact resize마다 발생하는 `vkCreateSwapchainKHR`가 AMD에서 보통 19~25 ms의 지연을 만들었다. Retained capacity로 재생성 비용을 줄인 뒤에도 좌측·상단처럼 origin과 extent가 함께 바뀌는 resize에서 검정/흰 노출, Acrylic material tail, raster의 이중 이동처럼 보이는 떨림이 남았다.

Top-level과 visible child의 geometry가 따로 갱신되는 구조를 제거하고, Vulkan offscreen raster를 Windows Presentation으로 전달하는 구조로 전환했다. Top-level 하나를 visible clip으로 사용해도 USER32의 창 geometry와 DirectComposition/Presentation의 frame은 별도 transaction이다. 사전 제출·사후 제출·DwmFlush만으로는 모든 시점의 일치를 확보하지 못했다. `problem.md`의 v9 미해결 결론과 “고정 envelope가 필요하다”는 당시 판단은 이후 후보 구현과 사용자 확인 이전의 기록이다. 현재는 표준 HWND를 유지한 clock/geometry/receipt 경로에서 사용자 확인을 얻었다.

| 단계 | 결론 및 보존할 이력 |
|---|---|
| 9월 2일 direct WSI 및 acquire/present hardening | Stale 작업은 acquire 전에 취소하고 acquire 후 terminal/retirement 계약을 강화했다. 초기 구현·수명주기 검증은 통과했지만 exact swapchain 재생성의 지연 gate는 FAIL이었다. [이전 체크포인트](../26-09-02/windows-appsdk-vulkan-implementation-checkpoint.md)는 당시 구조의 기록이다. |
| 9월 3~4일 retained child, actual WM_SIZE, ContentIsland, top-level/child Presentation 비교 | 일부 자동 PASS와 한시적 사용자 확인 뒤에도 좌·상단 떨림·material tail이 다시 보고됐다. 과거 자동 PASS를 최종 물리 해결의 근거로 재사용하지 않는다. |
| v8 prepared/post-geometry 및 v9 pre-submit/no-display-wait | Exact/final/resource gate와 일부 캡처는 통과했으나 간헐 gap 및 사용자 떨림 보고가 남았다. |
| 40 FPS geometry+raster pacing (v10) | 사용자 물리 FAIL. 고정 FPS 제한과 v10 전용 계약을 제거하고 v9로 복귀했다. Arm N/fixed-envelope/custom non-client 구조는 채택하지 않았다. |
| J0~J2 just-in-time pre-geometry commit | 계측·prepared-slot 분리·WINDOWPOS callback을 구현했으나 Acrylic gate 실패로 당시 후보를 보관하고 v9로 롤백했다. 사용자의 v9 재확인도 FAIL이었다. |
| 새 clock/geometry/receipt 비교 | Worker clock 또는 platform pre-geometry clock/Present는 느린 Acrylic Left에서 17 px 노출. Post-geometry 제출에 receipt 대기가 없으면 TopLeft에서 9 px, pre-clock을 제거하고 receipt만 기다리면 19 px 노출로 FAIL이었다. |
| 최종 clock → actual geometry → prepared Present → matching receipt | 16개 focused 자동 시험과 최종 DLL 재검증을 통과했고, 이후 사용자가 창 조절이 잘된다고 확인했다. **현재 구현 및 사용자 PASS는 이 경로에 한정한다.** |

과거 FAIL manifest, anomaly PNG와 폐기 후보 source/binary는 `.doroti/evidence/`에 보존한다. 현재 PASS로 과거 실패를 재분류하지 않는다.

## 최종 구현과 유지하는 계약

정책 식별자는 `moving-origin-clock-geometry-prepared-commit-receipt`다.

1. `WM_SIZING`: raster worker가 proposed exact-size Skia scene과 synchronous Vulkan copy를 완료하고 slot 하나를 비노출 상태로 예약한다. 일반 acquire는 예약 slot을 제외한다.
2. `WM_WINDOWPOSCHANGING`: 기본 min/max 처리 후 최종 WINDOWPOS와 epoch/generation/input/edge/outer/client/scale을 확인한다. Platform thread에서 compositor clock을 최대 32 ms, 한 번만 기다린다. 아직 새 pixels를 제출하지 않는다.
3. USER32가 실제 top-level origin/extent를 적용한다.
4. `WM_WINDOWPOSCHANGED`: 실제 geometry를 다시 검증한 뒤 같은 platform thread에서 준비된 slot을 즉시 Present한다. 여기서는 raster·Vulkan copy·Vulkan fence wait를 하지 않는다. 이어 최대 50 ms 동안 동일 present ID/content tag와 비어 있지 않은 display-instance array를 가진 `ICompositionFramePresentStatistics`를 기다린다. 이 native mode 2는 `DwmFlush`를 receipt로 대체하지 않는다.
5. Matching `WM_SIZE`는 acknowledgement만 수행한다. Mismatch, stale input, 취소, DPI 변경, reset/loss/close는 예약과 terminal을 drain하고 bounded exact fallback으로 처리한다.

Clock timeout은 창을 무기한 막지 않고 제출을 계속하되 실패 counter를 남겨 qualification을 FAIL로 만든다. Receipt timeout/failed commit도 성공으로 취급하지 않는다. Native feature bit 5 `PREPARED_GEOMETRY_RECEIPT_V1`를 필수로 요구해 새 managed host와 이전 DLL이 섞이는 실행을 거부한다.

Fixed-origin Right/Bottom의 pre-geometry 제출/DWM wait는 유지한다. Full-frame stretch, 임의 배경색 masking, 고정 FPS limiter, app-owned custom shell을 도입하지 않았다. Standard HWND의 geometry와 Presentation을 앱이 원자적으로 합치는 API를 만든 것은 아니며, 실제 clock과 해당 frame의 합성 통지를 사용해 다음 geometry 진행을 직렬화한다.

구현 소유권과 패키지는 다음과 같다.

- Managed host가 Silk.NET Vulkan instance/device/queue, Skia raster, copy/fence와 Presentation slot 수명을 소유한다. System32 Vulkan 1.1, 정확한 adapter LUID, dedicated/importable D3D11 texture 외부 메모리를 요구한다. Vulkan에서 ANGLE로 조용히 fallback하지 않는다.
- 현재 visible transport는 3-buffer Windows Presentation이고 active Vulkan WSI swapchain은 0이다. Top-level HWND의 native topmost DirectComposition target에 full-capacity identity source를 연결하고 top-level client만 visible clip으로 쓴다. Hidden child는 capacity/monitor/DPI probe 용도다.
- Silk.NET 2.23.0, SkiaSharp 계열 `4.152.0-rc.1.26426.14`, `SkiaSharp.Vulkan.Silk.NET`의 typed `GRSilkNetBackendContext`를 사용한다.
- Windows 11 24H2+의 explicit Vulkan+Acrylic은 host-backdrop-enabled non-topmost `DesktopWindowTarget`과 active `DesktopAcrylicController`, HWND-wide transient system underlay, premultiplied topmost Vulkan target으로 구성한다. 별도 흰 USER32 backing plane을 제거하며 kind/theme/tint/luminosity 옵션은 기존 controller에 적용한다. 필수 지원 조건 실패는 fail-fast한다.
- Capability contract 파일은 baseline v9로 남아 있다. 실제 실행 policy는 product `candidatePolicy` 및 native feature로 식별하고 aggregate에 `baselineMovingOriginPolicy`와 실제 `movingOriginPolicy`를 분리해 기록한다. 사용자 확인에 따른 schema 승격이나 runtime 변경은 이번 문서 정리에서 수행하지 않았다.

## 검증 결과와 증거

자동 시험 환경은 AMD Radeon 780M, 96 DPI, 165 Hz다. 테스트/process timeout은 저장소 규칙에 따라 20분을 적용했다. 아래는 기존 실행 결과의 요약이며 이력 정리 중 빌드·UI 테스트를 다시 실행하지 않았다.

| 검증 | 결과 |
|---|---|
| Native Release, DemoApp Windows Release, capability/product/ABI projects | PASS; managed builds warning 0/error 0 |
| Native ABI 및 prepared-frame fixture | PASS; prepared 10/cancelled 8/committed 2/reserved 0, presents 3. Stale/mismatch/double commit/no alignment/cancel/reset/loss/shutdown 및 clock 주입 실패 포함 |
| Acrylic Left/Top/TopLeft × reverse 150/600/1200 ms | 9 cases PASS-automated-partial |
| Opaque TopLeft reverse 600 ms × 5, Right/Bottom 각 1회 | 7 cases PASS-automated-partial |
| 16-case 합계 | 1,389 captures, prepared/commit 540/540, 기준 초과 gap frames 0, clock failures/platform timeouts/reserved 0, moving-origin QPC ordering 540 steps PASS |
| 최종 DemoApp과 같은 DLL 조합의 Acrylic TopLeft 1200 ms | PASS-partial; 135 captures, 기준 초과 gap 0, 최대 측정 1 px, prepare/commit/clock 69/69/69, 실패 0, 54.55 presentations/s |
| Source-built capability aggregate r2 | PASS; ANGLE baseline, Vulkan/Acrylic, exact-LUID import, device-loss 주입, reset/minimize-restore/exact-resize/start-close 각 10회, source/repository/binary 안정성 |
| Clock timeout 실제 주입 r2 | 실패 정확히 1회, commit 36/reserved 0, final exact/transport/marker PASS. 정상 qualification은 의도대로 FAIL, failure handling은 PASS |
| 사용자 직접 확인 | **창 조절 PASS-observed**. 초기 실행 FPS 관찰과 미수행 환경 조합은 별도 항목 |

주요 evidence 경로는 저장소 루트 기준이다.

- `.doroti/evidence/codex-postgeometry-receipt-checks-20260905/`: `checks.json`, `matrix-summary.json`, `matrix-cadence.json`, `active-binaries.json`, `matrix-runtime-source-identity.json`, `clock-timeout-r2-check.json`.
- `.doroti/evidence/codex-receipt-final-matrix-*/manifest.json` 및 `.doroti/evidence/codex-receipt-final-binary-acrylic-topleft-20260905/manifest.json`.
- `.doroti/evidence/codex-postgeometry-receipt-capability-r2-20260905/manifest.json`.
- `.doroti/evidence/codex-postgeometry-receipt-clock-timeout-r2-20260905/manifest.json` (의도된 FAIL).
- 실패 후보 보관: `codex-clock-worker-rejected-source-20260905/`, `codex-jit-clock-before-geometry-rejected-20260905/`, `codex-postgeometry-clock-unacknowledged-20260905/`, `codex-j2-rejected-source-20260905/` (모두 `.doroti/evidence/` 아래).

첫 aggregate는 ANGLE의 stale-input 취소 때문에 `Presents 24 != GpuCopies 25`로 실패했다. 조건을 완화하지 않고 같은 source로 r2를 통과했으며 최초 실패를 보존했다. 첫 clock-timeout 시도는 product startup이 환경변수를 초기화해 주입되지 않았다. Fault helper는 FAIL로 기록했고, 명시적 `--inject-vulkan-result` 전달을 연결한 r2에서 실제 실패 1회를 확인했다. QPC analyzer의 PowerShell `$receipt`/`$Receipt` 이름 충돌 및 단일 focused 실행의 미수행 gate 표기도 수정했으며 과거 manifest는 변경하지 않았다.

검증기에는 Acrylic caption/desktop 색이 비슷하면 정지 화면의 창 오른쪽을 잘못 추정하는 문제가 있었다. 고정 right-edge focused 시험은 시작 client-right screen anchor와 exact `#10243a` sentinel을 사용하고, 모든 frame의 usable evidence와 atomic outer-right/DPI 불변성을 확인하도록 고쳤다. 기존 허용치 `ceil(scale × 2)`는 유지했다. 원본 PNG SHA를 보존한 J2 Top 0/26/88 재분석은 기존 43/118/48 px를 모두 1 px로 측정했다 (`codex-clock-worker-checks-20260905/historical-oracle-replay.json`). Synthetic C++ tests와 실제 17/9/19 px 실패도 보존해 오탐 수정과 실제 gap을 구분했다. Right edge가 움직이는 다른 capture의 caption oracle 한계는 남아 있다.

## 성능 관찰 및 별도 후속

최종 matrix의 Acrylic moving-origin은 52.50–58.18 presentations/s, opaque TopLeft는 57.39–59.58이었다. Acrylic accepted-target→presented-terminal P95는 약 17.55–19.71 ms, 동일 환경 Right/Bottom은 82.29/84.61 presentations/s였다. 고정 FPS 제한은 없지만 clock/receipt 동기화 비용이 있다. 이 측정은 matched ANGLE 대비 성능 PASS를 뜻하지 않으며, 사용자 창 조절 확인과 함께 보존한다.

사용자가 별도로 관찰한 “극초반에 fps가 일시적으로 낮음”은 startup 성능 항목으로 남는다. 원인이 JIT인지 GPU/shader/asset 준비인지 분리하지 않았다. 기존 `work.md` 33.7의 ReadyToRun publish는 exit 0, 5초 startup smoke는 exit 0이었다. App runner/DemoApp/Windows host/Skia renderer/Widgets DLL의 실제 R2R signature, Vulkan/Composition-Swapchain + experimentalAcrylic/Active, 첫 exact present 이후 visible, failed terminals/operational errors 0을 확인했다. 증거는 `.doroti/evidence/codex-r2r-20260905/`의 `publish.stdout.txt`, `r2r-headers.json`, `smoke-summary.json`이다. **R2R 생성·실행 PASS이며 초기 FPS 개선량은 notVerified**다.

다른 GPU/driver/DPI/주사율, 전체 eight-edge/ANGLE-matched cadence, mixed-DPI/Snap/maximize/Alt-Tab/occlusion, 물리 IME/accessibility, 실제 device-removal 및 이번 후보의 external Vulkan validation layer 미수행 범위까지 사용자 PASS를 확장하지 않는다. 자동 capture/Composition receipt 자체도 물리 scan-out 증거와 구분한다.

## 실행 명령과 산출물 식별

저장소 루트에서 사용한 일반 Release 명령:

```powershell
$env:DOROTI_WINDOWS_PRESENTER = 'Vulkan'
$env:DOROTI_WINDOWS_VULKAN_DEVICE = 'AMD'
$env:DOROTI_DEMO_EXPERIMENTAL_ACRYLIC = '1'
pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows -Configuration Release
```

별도 R2R 게시 및 실행 명령 (정리 시점 `doroti.ps1`에는 R2R 인자 전달 옵션이 없음):

```powershell
dotnet publish ./DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj -c Release -r win-x64 --self-contained true -p:PublishReadyToRun=true -o ./.doroti/publish/DorotiDemoApp-win-x64-r2r
& ./.doroti/publish/DorotiDemoApp-win-x64-r2r/DorotiDemoApp.WindowsAppSdk.exe
```

R2R 실행도 위 세 환경변수를 설정한 shell에서 실행한다. 당시 DemoApp Release와 product validator의 DLL SHA-256은 다음과 같았고, R2R publish의 native DLL도 동일했다. R2R 변환된 managed DLL을 일반 Release managed hash와 동일하다고 주장하지 않는다.

- Native `doroti_windows_appsdk_host_v1.dll`: `2d254b2be6514804e5c1230a6b99bd4a3a3b4d116830a5e429c439c1eefd8ebd`
- 일반 Release `Doroti.Host.WindowsAppSdk.dll`: `f07e15a73364209dc265b2d06eeef429ff1a5edcc3a8488ed01f2e9a3e3f2c92`

구현 상세와 이전 amendment는 [ADR-027](../../Doroti/docs/adr/ADR-027-windows-optional-vulkan.md)에 남긴다. 이전 9월 2일 문서와 ADR의 과거 미해결/확인 대기 문구는 당시 기록이며, 현재 창 조절의 사용자 판정은 이 요약의 2026-09-05 PASS-observed가 최신이다.
