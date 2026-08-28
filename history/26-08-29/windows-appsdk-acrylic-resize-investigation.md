# Windows App SDK 아크릴 도입 후 resize 떨림 분석

- 기록일: 2026-08-29
- 분석 기준 checkout: `82be4421` + 작업 트리의 Windows 다크모드 변경
- 재현 명령: `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 run -App ./DorotiDemoApp -Platform windows`
- 비교 기준: 아크릴 도입 전 `WindowsAppSdk` + `HwndExactCpp` + managed ANGLE/EGL-D3D11
- 최종 결정: **아크릴·투명 composition 실험은 롤백하고 Windows 다크모드 인식만 유지한다.**
- 사용자-visible 판정: **아크릴 경로는 FAIL. 롤백한 불투명 ANGLE 경로는 사용자가 다시 창 크기를 잘 따라가는 것으로 확인했다.**

이 문서는 2026-08-29에 Desktop Acrylic을 Windows 제품 경로에 도입한 뒤 발생한 심한 깜빡임, 계단식 resize, 떨림, 실시간 추종 실패를 분석하고 롤백한 기록이다. 빌드와 자동 카운터가 통과한 단계에서도 사용자가 실제 창에서 결함을 계속 확인했으므로, 사용자-visible 결과를 최종 판정으로 사용한다.

## 1. 결론

문제의 핵심은 아크릴 blur 연산 비용 하나가 아니었다. 아크릴을 표시하기 위해 가시 표면의 소유권을 기존 child HWND의 ANGLE/EGL window surface에서 DirectComposition 계층으로 바꾸면서, 기존에 하나였던 resize/present 타임라인이 여러 개로 분리됐다.

```text
아크릴 도입 전

shell WM_SIZE
  -> child HWND physical extent
  -> exact metrics / exact scene
  -> EGL_FIXED_SIZE_ANGLE surface
  -> Skia submit -> eglSwapBuffers -> resize DwmFlush
  -> visible HWND frame

아크릴 도입 후

shell HWND geometry ───────────────────────────────┐
framework metrics -> layout -> ANGLE GPU raster ──┼─> composition visual
DirectComposition target/swap-chain size/commit ──┘
```

아크릴 이전 경로에서는 child HWND와 EGL window surface가 같은 physical extent를 사용하고 native `WM_SIZE`가 exact present 완료를 bounded wait한다. 창 테두리, child client, EGL surface가 사실상 한 resize transaction으로 움직인다.

아크릴 경로에서는 shell window 크기는 즉시 바뀌지만 framework layout, ANGLE raster, alpha surface transfer, DirectComposition swap-chain/visual commit은 각자 다른 시점에 완료된다. 이 차이를 동기식으로 막으면 창 테두리 자체가 raster 완료를 기다리며 계단식으로 움직이고, 비동기로 풀면 shell geometry보다 composition content가 늦거나 source 크기가 반복 교체되어 떨렸다.

따라서 관찰된 현상은 다음 상충에서 발생했다.

- shell resize를 기다리게 하면 실시간 창 geometry가 막힌다.
- shell resize를 기다리지 않으면 exact raster와 composition visible front가 다른 세대가 된다.
- 매 세대 `ResizeBuffers`를 수행하면 composition이 사용하던 source extent가 드래그 중 계속 바뀐다.
- 드래그 중 buffer를 고정하면 마지막 frame은 안정적이지만 framework layout이 실시간으로 따라오지 않는다.

현재 Doroti의 exact-generation 계약을 그대로 유지하면서 아크릴만 추가하는 것은 충분하지 않았다. 아크릴 제품화에는 별도의 zero-copy composition presenter와 하나의 visible-front 전환 계약이 필요하다.

## 2. 아크릴 이전 경로가 잘 따라가는 이유

현재 복원된 [WindowsManagedAngleEglPresenter.cs](../../Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedAngleEglPresenter.cs)는 다음 계약을 사용한다.

- hardware D3D11 ANGLE display/context를 사용한다.
- visible child HWND에 `EGL_FIXED_SIZE_ANGLE` window surface를 만든다.
- exact-size GPU backing에 Skia scene을 raster한 뒤 default framebuffer로 한 번 복사한다.
- `eglSwapBuffers`가 성공한 뒤 새 surface의 첫 present와 resize present에서 `DwmFlush`를 수행한다.
- 크기가 바뀌면 이전 Skia/EGL window target을 해제하고 새 physical extent에 맞는 surface를 만든다.

[exports.cpp](../../Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/exports.cpp)의 child `WM_SIZE`는 physical client extent를 metrics generation으로 게시하고, render를 queue한 다음 `WaitForExactResize`로 bounded wait한다. 이 방식은 UI thread를 잠시 기다리게 하지만 visible owner가 child HWND/EGL 하나이므로, 대기 뒤 표시되는 front와 창의 client extent가 직접 대응한다.

롤백 후 실제 Demo 실행 진단은 다음과 같았다.

| 항목 | 결과 |
| --- | ---: |
| presenter | `ANGLE/EGL-D3D11` |
| accepted resize generations | 57 |
| presented resize generations | 56 |
| superseded resize generations | 1 |
| failed resize generations | 0 |
| unterminated / duplicate terminals | 0 / 0 |
| EGL/GLES operational errors | 0 |
| process exit | 0 |

자동 수치만으로 화면 품질을 확정하지 않았다. 이 실행 이후 사용자가 실제 창에서 다시 크기를 잘 따라가는 것으로 판정했다. 이 사용자 판정은 현재 비교 조건에 대한 PASS이며 모든 DPI, monitor, edge, 속도 조합의 일반화는 아니다.

## 3. 아크릴 경로에서 시도한 구성과 결과

### 3.1 Windows App SDK Desktop Acrylic + ANGLE DirectComposition surface

첫 구성은 native top-level HWND에 Windows App SDK `DesktopAcrylicController`와 `SystemBackdropConfiguration`을 연결하고, Doroti content는 alpha가 있는 ANGLE DirectComposition surface로 올리는 방식이었다. 투명 content가 보이도록 top/child HWND에 composition-only extended style을 적용하고 renderer clear도 transparent로 바꿨다.

결과:

- 아크릴과 투명 content 자체는 화면에 나타났다.
- 초기에는 resize 시 심한 깜빡임이 발생했다.
- 깜빡임을 줄인 뒤에도 content가 창 테두리를 계단식으로 따라오며 떨렸다.
- 사용자는 반복해서 “아크릴 도입 전에는 떨림이 없었다”, “창 크기를 실시간으로 못 따라간다”고 판정했다.

원인:

- native `WM_SIZE`가 managed raster/compositor 완료를 기다리면 shell sizing loop가 block됐다.
- wait를 제거하면 shell geometry와 ANGLE/DirectComposition visible content가 비동기화됐다.
- HWND auto-size와 managed exact raster size가 동시에 surface 크기에 관여해 source/destination timing이 이중화됐다.

### 3.2 동일 ANGLE surface에서의 조정

다음 조정을 차례로 적용했지만 physical 떨림은 남았다.

- resize마다 EGL surface를 destroy/recreate하지 않고 surface를 유지했다.
- opaque path만 `EGL_WIDTH`/`EGL_HEIGHT`를 갱신하고 DirectComposition path는 HWND auto-size에 맡겼다.
- resize마다 수행하던 `DwmFlush`를 제거하고 첫 surface에만 남겼다.
- interactive `WM_SIZE`의 exact wait를 제거하고 latest-only 비동기 frame으로 바꿨다.
- `WS_EX_NOREDIRECTIONBITMAP`을 top/child HWND 모두에 적용했다.
- exact scene이 준비되기 전에는 surface를 파괴적으로 바꾸지 않도록 admission gate를 추가했다.

이 변경들은 심한 깜빡임이나 blank 노출 일부를 줄였지만, 사용자가 본 계단식 추종과 떨림을 제거하지 못했다. 같은 `EGL_DIRECT_COMPOSITION_ANGLE` 경로를 계속 미세 조정하는 것으로는 해결되지 않는다고 판정했다.

### 3.3 D3D12 composition swap chain

ANGLE의 내장 DirectComposition surface를 피하기 위해 premultiplied-alpha DXGI composition swap chain을 Doroti가 직접 소유하는 경로를 시도했다.

결과는 runtime FAIL이었다.

- D3D12 debug error ID 1315가 descriptor heap 사용에서 발생했다.
- 실제 Demo scene의 Skia submit에서 D3D12 error ID 1422가 반복됐다.
- runner는 operational GPU error를 감지해 fail-closed로 종료했다.

이는 기존 D3D12 진단 경로의 알려진 실패와 같은 계열이다. 아크릴 문제를 해결하기 위해 검증되지 않은 D3D12 presenter를 제품 기본값으로 승격할 수 없었다.

### 3.4 D3D11 raster upload composition path

D3D12를 피하기 위해 Skia CPU raster surface를 D3D11 composition swap chain으로 upload하는 경로도 시도했다.

결과는 기능 계약 FAIL이었다.

- DemoApp의 `ImageFilter.shader`는 active Skia GPU recording context를 요구한다.
- software capture는 runtime에서 명시적으로 금지되어 `NotSupportedException`으로 종료됐다.

따라서 전체 renderer를 CPU surface로 바꾸는 fallback은 사용할 수 없었다.

### 3.5 ANGLE GPU raster + CPU readback + D3D11 composition upload

GPU shader 기능을 유지하기 위해 ANGLE offscreen GPU surface에서 raster한 뒤 CPU로 readback하고, premultiplied D3D11 composition swap chain에 upload하는 hybrid path를 구현했다.

자동 진단은 겉보기에는 정상적이었다.

| 항목 | 아크릴 hybrid 실행 |
| --- | ---: |
| accepted resize generations | 37 |
| presented resize generations | 36 |
| failed resize generations | 0 |
| `ResizeBuffers` | 35 |
| GPU operational errors | 0 |

그러나 사용자는 이 경로에서도 떨림을 확인했다. 이 결과는 counter가 깨끗하고 거의 모든 resize generation을 present해도 화면이 매끄럽다는 뜻은 아님을 보여준다.

이 경로의 병목과 불안정 요소는 다음과 같다.

- ANGLE GPU submit 뒤 동기 readback이 추가됐다.
- readback한 전체 frame을 다시 D3D11 texture로 upload했다.
- exact resize generation마다 composition swap chain `ResizeBuffers`가 반복됐다.
- DirectComposition이 이전 buffer를 사용 중인 동안 source extent가 새 exact extent로 계속 교체됐다.

### 3.6 드래그 중 마지막 composition buffer 고정

원본 buffer 크기가 드래그 중 반복 변경되는 문제를 확인하기 위해 interactive resize 동안 마지막 완성 buffer를 유지하고, mouse-up에서 한 번만 exact buffer로 교체했다.

진단상 `ResizeBuffers`는 35회에서 1회로 줄었고 GPU/terminal 오류는 없었다. 하지만 사용자는 즉시 “이번엔 실시간으로 못 따라간다”고 판정했다. 떨림 대신 stale layout을 stretch하는 방식이므로 요구를 충족하지 못했다.

이 실험은 full-frame provisional stretch나 mouse-up-only relayout이 제품 해법이 아니라는 기존 결론을 다시 확인했다.

## 4. 근본 원인 분류

### 4.1 visible owner가 하나에서 둘 이상으로 늘어남

기존 제품 경로의 visible owner는 child HWND에 연결된 EGL window surface 하나다. 아크릴 경로에서는 top-level backdrop target, child composition target 또는 ANGLE internal DirectComposition surface, managed exact backing이 함께 관여했다.

각 object가 합법적으로 동작하더라도 어느 front가 현재 shell extent의 authoritative visible content인지 한 transaction으로 정의되지 않았다. 아크릴 controller를 켰다는 사실만으로 Doroti scene의 resize/present ordering이 자동으로 composition ordering과 합쳐지지 않는다.

### 4.2 세 개의 clock

아크릴 경로에는 최소 세 clock이 존재했다.

1. shell의 `WM_SIZING`/`WM_SIZE`와 child HWND geometry
2. Doroti metrics/layout/scene/ANGLE raster generation
3. DXGI/DirectComposition surface resize, commit, DWM composition

동기 wait는 1을 2와 3 뒤에 묶어 계단식 window movement를 만들었다. 비동기 처리는 1을 먼저 진행시키지만 2와 3의 늦은 frame이 보이게 했다. `Present`, `eglSwapBuffers`, `Commit`, `DwmFlush` 중 어느 하나도 세 clock 전체의 원자적 visible acceptance를 보장하지 않는다.

### 4.3 `ResizeBuffers`는 frame cadence와 별개의 visible source 변경

hybrid 경로에서 37개 accepted resize 중 35회 `ResizeBuffers`가 수행됐다. 이는 단순히 frame을 많이 그렸다는 뜻이 아니라 DirectComposition visual이 참조하는 source resource의 extent가 드래그 동안 거의 매번 바뀌었다는 뜻이다.

shell destination extent도 변하고 source extent도 다른 cadence로 변하므로, 각 단계가 exact하더라도 전환 순간은 사람 눈에 떨림이나 계단으로 보일 수 있다.

### 4.4 zero-copy가 아니었던 hybrid 경로

ANGLE GPU → CPU readback → D3D11 upload는 GPU shader 기능을 유지했지만 한 frame에 GPU/CPU/GPU 경계를 추가했다. 이 경로는 진단용으로는 유용했지만 제품 resize latency를 줄이는 구조가 아니었다.

## 5. 롤백 범위와 현재 상태

아크릴 이후 변경은 모두 제거했다.

- `DesktopAcrylicController`, backdrop target/configuration 제거
- transparent backdrop ABI/configuration 제거
- `EGL_DIRECT_COMPOSITION_ANGLE` 경로 제거
- composition-only HWND style 제거
- transparent renderer clear 제거
- D3D11/DirectComposition package와 hybrid presenter 제거
- interactive resize wait/queue 실험을 기존 동작으로 복원
- 아크릴용 product validator 변경 제거

다음 Windows 다크모드 변경만 유지한다.

- registry `AppsUseLightTheme`에서 초기 platform brightness 결정
- `WM_SETTINGCHANGE`/`WM_THEMECHANGED`에서 brightness 갱신
- top-level HWND에 `DWMWA_USE_IMMERSIVE_DARK_MODE` 적용
- native ABI로 초기 brightness와 변경 callback 전달
- managed `PlatformConfiguration.platformBrightness` 갱신

롤백 후 검증:

| gate | 상태 | 근거 |
| --- | --- | --- |
| native x64 build | **PASS** | native DLL 생성, x64 확인 |
| native ABI C1 | **PASS** | ABI v1, app-directory load, brightness contract, GPU pointer 0 |
| ANGLE product C5-A | **PASS** | 18 resize generation presented, failed/duplicate/unterminated 0, EGL/GLES error 0 |
| 실제 Demo 명령 | **PASS** | exit 0, hardware ANGLE/D3D11, failed resize 0 |
| rollback 후 visible resize | **PASS by user** | 사용자가 다시 창 크기를 잘 따라가는 것으로 확인 |
| 실제 OS light/dark 전환 | `notVerified` | ABI/managed callback 자동 검증과 실제 테마 UI 확인은 분리 |

## 6. 아크릴을 다시 시도할 때 필요한 선행 조건

현재 제품 경로에 같은 방식으로 아크릴을 다시 붙이지 않는다. 재도입은 별도 prototype과 다음 gate를 먼저 요구한다.

1. **visible owner 하나**: top/child/backdrop/content 중 최종 Doroti front의 owner를 하나로 정의한다.
2. **zero-copy GPU 경로**: ANGLE GPU 결과를 CPU readback 없이 D3D11 shared texture 또는 composition surface로 전달한다.
3. **resize source 안정성**: 매 `WM_SIZE`마다 visible source resource를 `ResizeBuffers`하지 않으면서 exact content를 표시할 설계가 필요하다. full-frame stretch, edge repetition, mouse-up-only relayout은 허용하지 않는다.
4. **shell 비차단**: interactive sizing UI thread에서 raster/compositor 완료를 동기 대기하지 않는다.
5. **generation-to-commit ledger**: `WM_SIZE`, metrics, scene, GPU submit, composition commit, terminal의 QPC와 size를 한 evidence에 기록한다.
6. **실제 Demo physical gate 우선**: build, terminal count, GPU error 0만으로 PASS하지 않는다. 사용자가 실제 border drag에서 실시간 추종, 왜곡, 떨림, exposed band를 확인해야 한다.
7. **D3D12 선행 결함 분리**: Skia D3D12 error ID 1315/1422를 아크릴 작업과 섞지 않는다. 별도 presenter가 debug-clean해지기 전에는 제품 아크릴 해법으로 사용하지 않는다.

## 7. 최종 해석

아크릴 효과 자체를 켜는 API 호출은 성공했다. 실패한 것은 Doroti의 기존 HWND exact-present 계약과 composition 기반 투명 material의 visible-front 계약을 통합하는 일이었다.

현재 불투명 ANGLE 경로가 더 잘 따라가는 이유는 단순히 효과가 없어서 빠른 것이 아니라, child HWND geometry와 EGL visible surface가 동일한 owner와 bounded transaction 안에 있기 때문이다. 아크릴 재도입은 이 단일 ownership을 잃지 않는 새 presenter 설계가 마련될 때까지 보류한다.

> 문서 성격: 2026-08-29 아크릴 도입·실패·롤백 분석 기록. 새로운 active roadmap이나 아크릴 재도입 승인이 아니다.
