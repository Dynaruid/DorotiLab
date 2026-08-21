# Doroti Linux Qt backend 구현 요약

- 기록일: 2026-08-20
- 상태: **핵심 backend와 Kubuntu/VMware 자동 build·publish·Wayland/XWayland live 경로 구현, 물리 Linux·IME·접근성·복구·성능 acceptance는 `notVerified`**
- 원본: 삭제한 루트 `work.md`의 구현 상태를 압축한 역사 기록

## 1. 문서 성격

이 문서는 `DorotiDemoApp/linux`, `Doroti.Host.Qt`, 공용 Skia renderer, template과 validation에 Linux Qt backend를 연결한 작업의 2026-08-20 기준 결과를 보존한다. 새로운 active roadmap이나 모든 Linux 지원의 완료 선언이 아니다.

첫 제품 범위는 .NET 10, `linux-x64`, Qt 6.5 이상, Qt Widgets/OpenGL, 단일 `DorotiView`다. managed process가 Doroti lifetime을 소유하고 Qt가 event loop, window, display, input과 desktop service를 소유한다.

## 2. 최종 렌더링 경계

```text
Doroti widget/runtime
  -> scene submission
  -> Doroti.Skia.Rendering
  -> current QOpenGLWindow FBO
  -> Qt compositor swap
  -> frameSwapped terminal ACK
```

- 직접 C++ C ABI shim과 `doroti.qt-host/v2` 고정 폭 ABI를 사용한다.
- Skia가 Qt의 현재 FBO에 직접 그리며 full-frame CPU copy/readback과 managed framebuffer 할당을 제품 경로에 두지 않는다.
- `paintGL()`이나 Skia flush가 아니라 `QOpenGLWindow::frameSwapped()`를 terminal present 경계로 사용한다.
- `QOpenGLWidget`은 Mesa Gallium indexed-draw 실패가 있어 ADR-022에 기록했고, VMware/Wayland SVGA3D에서 전체 Material Gallery를 표시한 `QOpenGLWindow`를 ADR-023의 제품 표면으로 선택했다.
- `SVGA3D; ... LLVM` 문자열이나 `Accelerated: no` 한 필드만으로 software renderer로 판정하지 않으며, `llvmpipe`, `softpipe`, `SwiftShader`는 명시적으로 거부한다.
- Qt Bridge, PySide6, QML/Qt Quick, `QQuickFramebufferObject`, native executable + `hostfxr`는 첫 구현 범위에서 제외했다.

## 3. 구현한 핵심 계약

### ABI, surface와 frame lifecycle

- ABI version, struct size, feature bits, opaque handle, pointer+length UTF-8, surface/context generation과 GL/FBO descriptor를 v2 계약에 고정했다.
- native `static_assert`와 managed layout 검사, callback exception 격리, GUI-thread queueing과 fatal-state 처리를 추가했다.
- `Doroti.Skia.Rendering`을 공용 scene renderer로 추출해 MAUI/Web/Qt의 scene semantics를 한 곳에서 소유하게 했다.
- `DorotiView`에 frame, input, text input, environment, platform service, scene, image/paragraph/semantics capability를 연결하고 placeholder `PresentedFrames++` 경로를 제거했다.
- frame request coalescing, retained replay, supersede 처리와 swap 기반 exactly-once terminal ACK 경계를 구현했다.

### Desktop input와 service

- mouse/wheel/touch/tablet/key/focus ABI와 Qt event mapping을 추가했다. XWayland 자동 smoke에서 mouse move/click, wheel, key 입력 4건과 30회 resize가 callback에 도달하고 exit 0이었다.
- Qt IME query/event를 surrounding text, selection, composing range, commit/replacement, caret rect의 UTF-16 editing-state 계약으로 연결했다.
- locale, color scheme, DPR/metrics, lifecycle, clipboard, cursor와 비동기 GUI-thread service dispatch를 연결했다.
- Wayland acrylic은 `ext-background-effect-v1`, KDE blur fallback 순서로 요청하고 미지원 compositor에서는 설정된 transparent/solid fallback을 사용한다.
- pruned semantics snapshot을 `QAccessibleInterface` 기반 virtual tree와 action에 연결했다.

### Packaging, template와 diagnostics

- `SkiaSharp.NativeAssets.Linux` 4.151.1과 `libdoroti_qt_host.so`를 Linux publish에 포함하고 app-local load를 확인했다.
- `ldd`/`readelf` 검사로 절대 build RUNPATH를 제거했으며 framework-dependent와 self-contained publish를 검증했다.
- Demo와 template의 CMake/source/header를 같은 ABI v2로 동기화하고 Linux 전용 validation/evidence shard를 추가했다.
- Qt 6.5+ Core/Gui/Widgets/OpenGL, QPA plugin과 Wayland 개발 도구는 system dependency로 문서화했다.

## 4. 확인한 결과

- VMware/Kubuntu Wayland에서 공용 renderer의 전체 Material Gallery 표시: `PASS`
- 20회 resize, 56 raster frame, swap terminal ACK와 정상 exit: `PASS`
- Wayland와 XWayland/xcb 별도 live 실행 및 exit 0: `PASS`
- 저장소 밖 framework-dependent Wayland publish와 self-contained xcb publish의 native library/plugin load: `PASS`
- Demo/template native source와 ABI v2의 독립 CMake build: `PASS`
- full-frame CPU readback/copy, per-frame managed framebuffer allocation, GUI-thread synchronous wait: 기록된 구현 경로에서 0

이 결과는 삭제한 계획 문서에 기록된 당시 evidence의 요약이다. 이 역사 문서 작성 과정에서 build나 live gate를 다시 실행한 것은 아니다.

## 5. 남은 검증 경계

다음은 완료로 승격하지 않고 `notVerified` 또는 미완료로 유지한다.

- WSLg의 고정 Skia frame과 전체 입력/IME/service 시나리오
- 실제 Linux Wayland와 실제 X11 세션, 의도한 hardware GL renderer 확인
- resize 외 별도 window/context 강제 재생성, stale callback 차단과 retained scene recovery
- hidden/minimized/occluded frame backpressure, close veto와 exactly-once dispose 순서
- mouse 전체 sequence, trackpad/touch/stylus, X11 wheel 변환과 DPR 변경 뒤 hit test
- 한글 IBus/Fcitx preedit, candidate caret, emoji, selection replacement와 input action
- clipboard/cursor/locale/theme/DPR/minimize/restore 및 compositor acrylic blur의 실제 시각 acceptance
- editable accessibility interface, AT-SPI tree와 Orca 탐색/action
- clean checkout과 외부 template consumer의 native build/run
- C#/C++ contract test, MAUI/Web/runtime shader 회귀 shard와 golden parity
- 30분 interaction soak, 60분 idle/animation soak, 60 Hz/고주사율 latency와 queue/resource 측정

물리 Linux나 접근성/IME/성능 측정이 없으므로 자동 build 또는 VM 창 표시를 해당 acceptance의 PASS로 해석하지 않는다.

## 6. 재개 시 기준

- `QOpenGLWindow` + current FBO + `frameSwapped` 계약을 유지한다.
- scene renderer를 Qt host에 복제하지 말고 `Doroti.Skia.Rendering`을 단일 소유자로 유지한다.
- 모든 scene은 `presented`, `replayed`, `superseded`, `failed` 중 정확히 하나로 종료해야 한다.
- surface/context/metrics generation과 stale completion을 분리해 검증한다.
- software renderer나 CPU bitmap fallback으로 GPU acceptance를 대신하지 않는다.
- WSLg/VM, 실제 Wayland, 실제 X11, 물리 입력·IME·Orca evidence를 서로 대체하지 않는다.

> 문서 성격: 삭제한 루트 `work.md`의 Linux Qt backend 구현 결과와 남은 acceptance 경계.
