# Windows / Web resize pipeline 재설계 요약

- 기록일: 2026-08-21
- 상태: **구현 완료, `COMMON-CONTRACT` PASS, Windows/Web의 확인 가능한 동작은 개선됨, 전체 제품 acceptance는 일부 `notVerified`, `CROSS-SMOKE`는 macOS 도구 부재로 failed**
- 원본: 삭제한 루트 `work.md`, `work2.md`의 계획·실험·최종 결과를 시간순으로 통합한 역사 기록
- 기준 구현 resize source fingerprint: `3368a3ec00cfefa2144597935d31f1b801309b80837d4cba83c3d087df3a40d6`
- 고주사율 후속 개선 fingerprint: `51b03b2acd1af2b89b73aad7627513188b6a4f173bae0001577421912e6f1e7e`
- 16 ms 근접 후속 개선 fingerprint: `585181e9dfdb047e1f454482a2547bc38d1a8a31a125fa9a7578bcebfc3d6947`

## 1. 문서 관계와 최종 해석

두 원본은 같은 문제를 다룬 연속 기록이다.

- `work.md`는 기존 SkiaSharp `SKGLView` 경계 안에서 EGL swap interval, `DwmFlush`, public handler/panel 계측을 먼저 검증한 초기 계획과 실패 결과를 담았다.
- 초기 최소 수정은 실제 primary DXGI Present가 계속 `SyncInterval=1`이었고 20 ms ACK 간격 게이트도 통과하지 못했다. 첫 Doroti-owned ANGLE window-surface spike는 `EGL_BAD_PARAMETER`, wrong-thread 예외와 창 소실을 일으켜 제거하고 안정 경로로 복원했다.
- `work2.md`는 이 결과를 받아 UI thread의 동기 render transaction 자체를 제거하고, Windows와 Web 모두 Doroti가 surface와 frame queue를 소유하는 전면 재설계로 전환한 최종 실행 문서다.

따라서 `work.md`의 `WIN-RSZ-2 failed`는 최종 구현이 다시 실패했다는 뜻이 아니라, **초기 ANGLE/EGL 접근을 폐기하고 `work2.md`의 DXGI/D3D12 + Skia presenter로 방향을 바꾼 근거**다. 최종 상태와 gate 판정은 이 문서의 뒤쪽 결과를 기준으로 한다.

## 2. 해결 대상과 근본 원인

- Windows: 창 테두리 drag 중 `WM_SIZE` 처리에서 layout, framework build, Skia raster, ANGLE swap과 `DwmFlush`가 UI/window thread에 직렬화되어 창이 렌더링 속도에 맞춰 계단식으로 뒤쫓았다.
- Web: root, window, canvas observer와 SkiaSharp private scheduler가 동시에 크기와 backing store를 갱신해 이전 크기와 최신 크기가 교대로 표시될 수 있었다.
- Windows paint surface 크기가 framework metrics를 역방향으로 갱신하고, Web target 관찰과 실제 backing-store commit이 서로 다른 세대로 움직이는 feedback 경로도 있었다.

swap interval이나 debounce만 바꾸는 것으로는 이 소유권 문제를 해결할 수 없다고 판정했다. 최종 구현은 크기 관찰, scene build, GPU surface commit, raster와 present terminal을 세대 기반 pipeline으로 분리했다.

## 3. 고정한 공통 계약

- `ResizeTargetGeneration`, `FrameworkFrameGeneration`, `SurfaceGeneration`, `PresentSequence`의 의미를 분리한다.
- 모든 resize frame은 logical/physical size, DPR, target/metrics generation과 scene sequence를 가진 immutable descriptor를 사용한다.
- 플랫폼마다 size authority와 GPU context/surface owner는 각각 하나만 둔다.
- queue는 `current + latest next` 두 칸을 넘지 않으며, 대체된 중간 frame은 `superseded` terminal을 받는다.
- 승인된 최신 target과 physical size가 정확히 같은 frame만 surface commit과 present를 허용한다.
- 모든 frame은 `presented/submitted`, `superseded`, `dropped`, `failed` 중 정확히 하나의 terminal을 가진다.
- 0-size/minimize는 surface 생성 대상으로 보지 않고, restore/context recreation 뒤 마지막 정상 scene 또는 최신 exact scene을 replay한다.
- paint/surface의 실제 크기는 framework metrics를 역방향으로 갱신하지 않고 일치 여부 검증에만 사용한다.
- build, callback 또는 process 생존은 visible resize PASS를 대신하지 않는다.

## 4. 구현 결과

### 공통 renderer와 validation

- target/frame/surface/present 세대를 분리하고 immutable frame descriptor, exact-size 승인, latest-only mailbox와 terminal ledger를 구현했다.
- deterministic resize contract에 정상 present, stale reject, A→B→C coalescing, 중복 size, DPR 변경, minimize/restore, context recreation을 포함한 8개 순열을 고정했다.
- validator를 문자열 존재 검사에서 state-machine assertion과 source fingerprint 검사 중심으로 교체했다.

### Windows

- UI thread의 synchronous framework build/raster/present/`DwmFlush` transaction과 paint-size metrics 역기록을 제거했다.
- stock `SKSwapChainPanel`/borrowed EGL 경로를 Doroti-owned `SwapChainPanel + DXGI/D3D12 + Skia` presenter로 교체했다.
- raster thread가 latest-only target mailbox, exact-size pre-present gate, surface resize, raster, present와 ACK를 소유한다.
- 200% DPI에서 물리 픽셀 swap chain이 XAML logical 영역보다 커지던 문제는 `IDXGISwapChain2.MatrixTransform` 역배율로 수정했다.
- live validator를 per-monitor-v2 DPI aware로 만들고 window/cursor 좌표를 물리 좌표로 통일해 실제 작업영역 안의 창 우하단을 drag하도록 보강했다.
- GDI `CopyFromScreen`은 DXGI composition swap chain을 PNG에 포함하지 못하므로 시각 증거로 사용하지 않았다. 이 캡처는 JSON에서 `notVerified`로 기록하고 사용자 직접 관찰과 trace를 사용했다.

### Web

- root `ResizeObserver`와 DPR resample coordinator만 size authority로 남기고 target observation과 backing-store commit을 분리했다.
- 제품 경로에서 `SKGLView`/`SKHtmlCanvas` private monkey patch를 제거하고 Doroti-owned canvas/WebGL/Skia presenter를 구현했다.
- browser rAF 하나와 managed latest callback 하나만 유지하고, exact frame에서만 backing store를 commit한다.
- native interop module을 `libSkiaSharp`로 맞추고 Demo Web runner에 `WasmBuildNative=true`를 고정해 일반 Release publish에도 `DorotiSkiaInterop.js`가 링크되게 했다.
- 동일 physical size에서 `canvas.width/height`를 다시 쓰지 않도록 해 반복 resize가 WebGL drawing buffer를 지우는 문제를 제거했다.

### 정리

- legacy Windows borrowed-EGL/stock-handler 경로를 제거했다.
- Windows DPI matrix, Web native interop link와 source fingerprint를 resize validator가 검사하도록 했다.
- 다른 플랫폼에는 Windows/Web native surface 구현을 강제로 공유하지 않고 공통 scheduler/trace 계약만 적용했다.

## 5. 최종 gate 결과

| Gate | 최종 상태 | 확인 결과 | 남은 경계 |
| --- | --- | --- | --- |
| `COMMON-CONTRACT` | **PASS** | 8개 순열 PASS, generated 10/terminal 10, unterminated 0, max queue depth 2, stale present 0, surface generation 9. Web host와 Windows MAUI host Release build warning/error 0 | native 창/브라우저의 가시적 동작을 대신하지 않음 |
| `WINDOWS-LIVE` | **notVerified** | 사용자가 실제 창 resize를 “굉장히 부드러운 수준”으로 확인. 200% DPI corner drag에서 target 388, presented ACK 419, regression/mismatch 0, target→ACK p95 30.498 ms, final swap p95 0.638 ms, framework exception 0. maximize/restore 20회와 minimize/restore, pointer/key 절차도 regression/mismatch/exception 0, 5초 내 종료 | 100/125/150%와 monitor 이동, IME/semantics/focus 개별 acceptance, Windows Graphics Capture 녹화 미실행 |
| `WEB-LIVE` | **notVerified** | native-linked Chrome publish가 렌더됨. 기본 CSS 1280×609/backing 2560×1218/DPR 2 일치. 40회 resize 뒤 CSS/backing/document 1024×700 일치, blank와 이전 크기 복귀가 보이지 않음. 버튼 Pressed 0→1과 ArrowDown dispatch 확인 | 실제 창 테두리 drag, throttling, 실제 zoom, `WEBGL_lose_context` 복구 미실행 |
| `CROSS-SMOKE` | **failed** | Android x64, iOS simulator x64, Linux managed Qt runner Release build warning/error 0 | macOS는 managed/AppKit compile 뒤 Windows에 `sips`가 없어 app icon 단계가 MSB3073 code 9009로 실패. 각 플랫폼 native/live도 별도 경계 |

`WINDOWS-LIVE`와 `WEB-LIVE`는 핵심 현상 개선을 관찰했지만 계획에 적은 모든 환경과 조작을 수행하지 않았으므로 전체 PASS로 승격하지 않는다. `CROSS-SMOKE` 실패는 resize source compile 오류가 아니라 Windows 환경에 macOS 도구가 없는 packaging 경계다.

## 6. 보존한 주요 evidence와 재실행 지점

- 공통 validation: `Doroti/validation/resize-contract/`
- Windows 200% DPI live summary: `Doroti/validation/evidence/resize/rsz0b-default-20260821-222746.summary.json`
- Windows 원시 trace: 위 summary와 대응하는 `raw.json`
- Windows interaction matrix: `Doroti/validation/evidence/resize/windows-matrix-20260821-222845.raw.json`
- Web product evidence: `Doroti/validation/evidence/web/web-product-evidence.json`
- 초기 EGL/ETW baseline과 실패 분석: `Doroti/validation/evidence/resize/` 아래 `rsz0b-*` 및 `*-gpu-*` 결과

주요 재실행 명령:

```powershell
pwsh -NoProfile -File ./Doroti/eng/validate-resize-continuity.ps1 -Shard Contract
pwsh -NoProfile -File ./Doroti/eng/validate-resize-continuity-live.ps1 -SwapInterval default -DurationSeconds 10 -RetainRawTrace
pwsh -NoProfile -File ./Doroti/eng/validate-web-product.ps1 -Shard Publish
```

모든 build/test는 저장소 지침에 따라 20분 timeout을 적용한다. 이 역사 문서를 작성하면서 위 build/live 절차를 다시 실행하지는 않았다.

## 7. 재개 시 남은 acceptance

- Windows: 100/125/150% DPI, 서로 다른 DPI monitor 이동, IME/semantics/focus, Windows Graphics Capture 기반 영상 증거, 가능하면 120/144 Hz.
- Web: 실제 browser window border drag, zoom/DPR 조작, DevTools throttling, WebGL context loss/restore, Chrome 외 Edge/Firefox와 가능한 환경의 Safari.
- Cross-platform: Android/iOS simulator 또는 device run, Linux Qt native/live, macOS host의 `sips` 포함 bundle build와 native live.
- 각 플랫폼의 visible 동작, physical input과 lifecycle/resource cleanup은 build 성공과 분리해 판정한다.

> 문서 성격: 삭제한 루트 `work.md`의 초기 조사·실패 분기와 `work2.md`의 최종 구현·검증 결과를 합친 역사 요약. 새로운 active plan이나 네 gate 전체 PASS 선언이 아니다.

## 8. 고주사율 빠른 drag 후속 개선

165 Hz 출력에서 빠르게 창 크기를 바꾸면 content가 경계를 계단식으로 따라오는 현상을 추가 확인했다.

- 예전 ANGLE presenter를 전제로 남아 있던 Windows 고정 `10 ms` composition frame 제한을 제거했다. Doroti-owned DXGI/D3D12 latest-only pipeline은 이제 `CompositionTarget.Rendering`의 실제 display cadence를 그대로 사용한다.
- target만 도착하고 exact scene은 아직 없는 상태에서 `ResizeBuffers`를 먼저 호출하던 wake를 제거했다. 첫 present 전 startup만 즉시 깨우고, 그 이후에는 마지막 정상 buffer를 compositor가 stretch하도록 유지한 뒤 exact scene invalidation에서 surface resize와 present를 함께 commit한다.
- 빨라진 cadence에서 드러난 disposed `Picture` race는 scene이 immutable picture command snapshot을 소유하도록 수정했다.
- 최종 200% DPI 10초 자동 corner drag는 target 555, presented ACK 649, generation regression 0, exact-size mismatch 0, framework exception 없음, 5초 내 종료였다. scene 없는 raster/resize miss는 비교 trace의 1,246회에서 8회로 감소했다.
- 최종 evidence: `Doroti/validation/evidence/resize/rsz0b-default-20260821-224845.summary.json` 및 대응 raw trace.
- 사용자는 수정 후 빠른 resize를 직접 보고 “꽤 괜찮다”고 확인했다. 영상 캡처와 다른 DPI/monitor acceptance는 계속 `notVerified`다.

## 9. 약 100 ms 지연 후속 분석

- 사용자가 실행한 workspace CLI는 `Configuration` 기본값을 Release로 선언했지만 공통 `build/run/publish`의 `dotnet` 인자에는 이를 전달하지 않아 모든 플랫폼 runner가 실질적으로 Debug 구성을 사용했다. 공통 인자에 `--configuration`을 추가해 기본 Release와 명시적 Debug 선택이 실제 빌드·실행·게시 구성에 반영되도록 수정했다. 별도 native binding build 경로는 이미 구성을 전달하고 있었다.
- 앞선 자동 drag의 ACK interval p99 113.670 ms에는 cosine 궤적이 방향 전환점에서 감속하면서 동일 픽셀에 머문 입력 공백이 섞여 있었다. 실제 target→ACK p99는 38.382 ms였다. live validator 입력을 4회 일정 속도 triangle wave로 바꿔 이후 trace에서는 입력 생성 공백과 renderer 지연을 분리한다.
- workspace CLI 기본 `build`가 Windows runner와 모든 의존 프로젝트를 `bin/Release`에 생성하고 warning/error 0으로 끝나는 것을 확인했다. 새 200% DPI 일정 속도 live trace는 target 544, presented ACK 708, regression/mismatch/exception 0, 5초 내 종료였고 ACK interval p99 51.562 ms(50 ms 초과 8회, 최대 연속 1회), target→ACK p50 21.463 ms/p95 27.703 ms/p99 31.317 ms였다.
- 후속 evidence: `Doroti/validation/evidence/resize/rsz0b-default-20260821-225630.summary.json` 및 대응 raw trace. 공통 contract도 8개 순열, fingerprint `51b03b2acd1af2b89b73aad7627513188b6a4f173bae0001577421912e6f1e7e`, build warning/error 0으로 다시 PASS했다. 사용자의 실제 빠른 drag 체감과 미검증 DPI/monitor 범위는 별도 acceptance로 유지한다.

## 10. 16 ms 목표 후속 개선

- 정상 배율 경로를 구간 계측한 결과 매 target의 exact `ResizeBuffers` 준비 비용은 p50 약 8 ms, p99 약 15 ms였고 framework scene 예약과 직렬로 이어져 target→ACK p50 약 23 ms가 됐다.
- 여유 back buffer와 `IDXGISwapChain2.SetSourceSize`로 `ResizeBuffers`를 피한 실험은 target→ACK p50 12.811 ms를 보였지만 사용자가 내부 Skia 배율 이상을 확인해 즉시 철회했다. frame-latency waitable swap chain, native resize frame 즉시 dispatch 단독 적용, 비동기 `Context.Submit(false)`도 개선이 없거나 악화되어 남기지 않았다.
- 최종 경로는 물리 target과 back buffer의 1:1 크기 및 기존 DPI 역행렬을 유지한다. 첫 present 이후 target-only wake는 raster thread에서 exact buffer 준비만 먼저 수행하고 scene 없는 Paint/Present는 하지 않는다. 동시에 WinUI `SizeChanged` pulse에서 metrics frame을 즉시 build해 `ResizeBuffers`와 framework 작업을 겹치며, 전용 raster thread는 `AboveNormal` 우선순위를 사용한다.
- 최종 200% DPI 일정 속도 10초 trace는 target 326, presented ACK 515, generation regression 0, exact-size mismatch 0, framework exception 없음, 5초 내 종료였다. target→ACK는 p50 16.331 ms/p95 23.003 ms/p99 26.942 ms, final swap은 p50 0.419 ms/p95 0.615 ms/p99 0.834 ms였다. 50 ms 초과 ACK 공백은 1회이며 연속되지 않았다.
- 최종 evidence: `Doroti/validation/evidence/resize/rsz0b-default-20260821-231533.summary.json` 및 대응 raw trace. 공통 contract 8개 순열과 Windows/Web Release build도 warning/error 0으로 PASS했다. 자동 trace는 내부 배율의 시각적 정상 여부를 증명하지 못하므로, 사용자가 최종 빌드를 직접 보고 배율과 빠른 drag를 다시 확인하기 전에는 visible acceptance를 `notVerified`로 유지한다.

## 11. 최신 창 크기 추종 분석

- 기존 `targetCount`는 4,096개 ring buffer에 남은 trace 꼬리만 세어 전체 입력 수로 잘못 해석될 수 있었다. resize trace capacity를 16,384로 늘리고 각 entry에 프로세스 간 공통 QPC timestamp를 추가해 10초 구간 전체를 보존했다.
- validator는 600개의 cursor pulse마다 실제 Win32 window rect를 채집하고 app target과 QPC로 대응한다. 200% DPI 최종 trace에서 경계→target catch-up은 p50 0 us/p95 98 us, target 크기 차이는 p95 가로 3 px/세로 1 px였다. 병목은 WinUI `SizeChanged` 전달이 아니라 target→exact ACK 구간이다.
- Skia draw 전에 target이 바뀐 경우 host `BeginPaint/EndPaint`만 통과해 `_invalidatePending`을 해제하고 raster를 생략하는 gate를 추가했다. 또한 Paint 직후·GPU flush 전에 latest generation을 다시 검사해 뒤처진 command를 submit하지 않는다. `Context.Submit(true)` 중복 제거 실험은 `DXGI_ERROR_INVALID_CALL`을 발생시켜 즉시 철회했으며 최종 코드에는 남아 있지 않다.
- 최종 자동 trace는 target 599, presented ACK 940, target→ACK p50 16.641 ms/p95 23.123 ms/p99 30.108 ms, generation regression 0, exact-size mismatch 0, framework exception 없음이었다. resized surface 준비는 p50 8.060 ms/p95 9.699 ms, Skia Paint는 p50 1.519 ms/p95 2.684 ms, GPU flush는 p50 1.371 ms/p95 2.904 ms였다. pre-flush에서 2건, submit 이후 pre-swap gate에서 155건을 supersede했다. ACK 기록 시점에 1세대 뒤였던 1건은 pre-swap 검사와 `Present()` 사이 수백 us 동안 새 target이 들어온 취소 불가능 race로 별도 관측한다.
- evidence: `Doroti/validation/evidence/resize/rsz0b-default-20260821-234320.summary.json` 및 대응 raw trace. live validator는 이제 framework exception과 correctness regression을 실패 처리한다. 실제 화면에서 내부 layout이 한 frame 덜 뒤따르는지는 사용자의 빠른 drag 확인 전까지 `notVerified`다.
