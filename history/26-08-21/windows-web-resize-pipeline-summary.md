# Windows / Web resize pipeline 재설계 요약

- 기록일: 2026-08-21
- 상태: **구현 완료, `COMMON-CONTRACT` PASS, Windows/Web의 확인 가능한 동작은 개선됨, 전체 제품 acceptance는 일부 `notVerified`, `CROSS-SMOKE`는 macOS 도구 부재로 failed**
- 원본: 삭제한 루트 `work.md`, `work2.md`의 계획·실험·최종 결과를 시간순으로 통합한 역사 기록
- 최종 resize source fingerprint: `3368a3ec00cfefa2144597935d31f1b801309b80837d4cba83c3d087df3a40d6`

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
