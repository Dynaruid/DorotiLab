# Linux Qt backend implementation plan

- 작성일: 2026-08-20
- 대상: `DorotiDemoApp/linux`, `Doroti/src/Doroti.Host.Qt`, 공용 Skia 렌더링 계층, 템플릿/검증/문서
- 첫 제품 범위: `net10.0`, `linux-x64`, Qt 6.5 이상, Qt Widgets + OpenGL, 단일 `DorotiView`
- 상태 표기: 미실행 플랫폼 게이트는 실패가 아니라 `notVerified`로 남긴다.

## 0. 현재 상태와 문제 정의

현재 Linux runner는 다음까지만 되어 있다.

- WSL Ubuntu에서 .NET SDK, CMake, Ninja, Qt 6 개발 도구를 설치했고 Debug/Release 빌드가 가능하다.
- `DorotiDemoApp.Linux.csproj`가 CMake로 `libdoroti_qt_host.so`를 만들고 관리 프로세스에서 이를 로드한다.
- C++ shim은 `QApplication`, `QWidget`, `QOpenGLWidget`을 만들고 WSLg 창을 표시한다.
- 관리 코드는 `DorotiApplicationBoundary`와 `DorotiHostSession`을 시작한다.

그러나 이것은 아직 Qt backend가 아니다.

- `paintGL()`은 managed callback을 부를 뿐 Doroti scene을 그리지 않는다.
- `DorotiQtRunner.OnFrame()`은 `PresentedFrames`만 증가시키며 `DorotiView`를 생성/등록/attach하지 않는다.
- resize/lifecycle/pointer/key/text callback의 대부분은 값을 버린다.
- 현재 `doroti_qt_callbacks_v1`에는 FBO/context 정보, 프레임 요청, present 완료, wheel/touch, 조합 중인 IME 문자열, clipboard/cursor, semantics 계약이 없다.
- 빌드 성공과 WSLg 창 표시는 실제 픽셀, 입력, IME, 접근성, X11/Wayland, GPU present의 증거가 아니다.

따라서 완료 조건은 “Qt 창이 뜬다”가 아니라 다음 전체 경로가 실제로 연결되는 것이다.

```text
Doroti widget/runtime
  -> Scene submission
  -> shared Skia scene renderer
  -> current QOpenGLWidget FBO
  -> Qt compositor swap
  -> terminal present ACK

Qt native events/services
  -> versioned C ABI
  -> Doroti host capabilities
  -> PlatformDispatcher / DorotiView
```

## 1. 고정할 아키텍처 결정

- [ ] ADR-021의 runtime ownership을 유지한다: managed process가 startup과 Doroti lifetime을 소유하고 Qt는 event loop/window/display/input/IME/clipboard/accessibility를 소유한다.
- [ ] 첫 backend는 현재의 직접 C++ C ABI shim과 Qt Widgets를 확장한다.
- [ ] 첫 렌더 표면은 `QOpenGLWidget`의 현재 context와 `defaultFramebufferObject()`를 사용한다.
- [ ] Skia가 Qt FBO에 직접 렌더하게 하며 전체 프레임 CPU readback/copy나 managed framebuffer 할당을 허용하지 않는다.
- [ ] `QOpenGLWidget::frameSwapped()`를 실제 present 완료 경계로 사용한다. `paintGL()` 진입이나 Skia flush만으로 presented 처리하지 않는다.
- [ ] MAUI/Web에 중복된 scene/paragraph/image/runtime-effect 로직을 Qt에 복사하지 않고 공용 `Doroti.Skia.Rendering` 프로젝트로 먼저 추출한다.
- [ ] Qt Bridge, PySide6, QML/Qt Quick, C++ executable + `hostfxr`는 첫 구현에 넣지 않는다.
- [ ] `QQuickFramebufferObject`는 Qt 문서상 legacy이며 OpenGL 전용이므로 fallback 후보에서도 제외한다.
- [ ] LNX-QT-0의 직접 FBO spike가 실패할 때만 `QOpenGLWindow`, `QSGRenderNode`/QRhi, native-owned process를 별도 ADR로 재평가한다.
- [ ] 기존 SkiaSharp 버전 `4.151.1`은 유지하고 같은 버전의 `SkiaSharp.NativeAssets.Linux`만 추가한다. 이번 작업에서 임의 dependency upgrade를 하지 않는다.
- [ ] 첫 제품 범위는 단일 window/view와 `linux-x64`다. ABI와 관리 객체는 이후 multi-view를 막는 전역 singleton으로 만들지 않는다.

## 2. 완료 정의

아래 항목이 모두 충족되어야 Linux Qt backend를 완료로 표시한다.

- [ ] 실제 데모 widget tree가 Qt 창에 GPU 렌더되고 resize/DPR/theme 변화 후에도 정상 표시된다.
- [ ] 제출된 모든 scene이 `presented`, `replayed`, `superseded`, `failed` 중 정확히 하나의 terminal ACK를 받는다.
- [ ] mouse/trackpad/wheel/keyboard/focus와 최소 touch/stylus 계약이 Doroti input pipeline에 들어간다.
- [ ] 한글 IME preedit, selection, replacement, commit 및 후보창 caret 위치가 동작한다.
- [ ] clipboard, cursor, locale, brightness, lifecycle, close 순서가 host capability로 연결된다.
- [ ] Doroti semantics tree와 action이 Qt accessibility tree를 통해 노출된다.
- [ ] WSLg Wayland/XWayland, 실제 Linux Wayland/X11 검증을 서로 다른 evidence로 기록한다.
- [ ] framework-dependent 및 self-contained publish 산출물의 native dependency와 로딩 규칙을 검증한다.
- [ ] Demo, template, package/solution map, validators, docs, evidence schema가 같은 계약을 사용한다.

## 3. 실행 순서

### LNX-QT-0. 렌더링 경계 spike와 ABI v2 확정

목표: 현재 managed-owned 구조에서 Skia가 Qt의 FBO에 직접 그릴 수 있음을 가장 먼저 증명한다.

- [ ] `doroti_qt_callbacks_v1`을 소리 없이 변경하지 말고 `doroti.qt-host/v2` ABI를 새로 정의한다.
- [ ] 양방향 table에 `abi_version`, `struct_size`, `feature_bits`, opaque view/context handle을 둔다.
- [ ] C ABI의 bool/enum/size를 `uint32_t`, `int32_t`, `int64_t`, `double`처럼 고정 폭으로 정의한다.
- [ ] 모든 UTF-8 값은 null 종료 가정 대신 pointer + byte length로 전달한다.
- [ ] native `static_assert`와 managed `Marshal.SizeOf`/`Marshal.OffsetOf` 테스트로 layout을 잠근다.
- [ ] 필수 callback, ABI version/size, 지원하지 않는 feature를 native 진입 전에 검증하고 결정적인 오류 코드를 반환한다.
- [ ] callback exception이 ABI 경계를 넘지 않게 한다. managed callback은 예외를 잡아 fatal state를 저장하고 Qt close를 요청하며, C++ 진입점도 예외를 오류 코드로 변환한다.
- [ ] 각 callback/function의 허용 thread를 계약에 기록한다. Qt 객체 접근은 GUI thread로 queue한다.
- [ ] `paintGL()` callback에 다음 surface descriptor를 전달한다.
  - surface/context generation과 context identity token
  - Qt default FBO id
  - physical pixel width/height와 DPR
  - sample count, stencil bits, color format
  - desktop GL/OpenGL ES 구분과 profile/version
  - Qt monotonic timestamp
- [ ] 관리 spike에서 현재 GL context로 `GRGlInterface`, `GRContext`, `GRBackendRenderTarget`, `SKSurface`를 만든다.
- [ ] Qt가 bind한 FBO를 보존하고 FBO 0을 기본 framebuffer로 가정하지 않는다.
- [ ] 데모 전체가 아니라 고정 색 배경 + 두 도형 + 텍스트를 Skia로 직접 그려 실제 픽셀을 확인한다.
- [ ] Skia flush 뒤 Qt가 swap을 소유하게 하고 `frameSwapped()`에서만 terminal present callback을 보낸다.
- [ ] resize 전후 FBO id가 달라져도 새 generation으로 surface를 다시 만들고 stale surface를 사용하지 않는다.
- [ ] `QOpenGLContext::aboutToBeDestroyed`와 widget destructor 양쪽에서 현재 context를 확보한 뒤 Skia GPU resource를 해제한다.
- [ ] GL vendor/renderer/version, QPA platform, FBO/sample/stencil 정보를 진단 산출물에 남긴다.

LNX-QT-0 종료 조건:

- [ ] WSLg 창에서 고정 Skia frame이 CPU framebuffer 복사 없이 표시된다.
- [ ] resize 20회와 window 재생성 뒤에도 crash, stale generation, 검은 frame이 없다.
- [ ] 모든 그리기 요청에 swap 기반 terminal ACK가 정확히 한 번 기록된다.
- [ ] 실패 시 다음 단계로 진행하지 않고 실패 원인과 대안 선택을 새 ADR에 기록한다.

### LNX-QT-1. 공용 Skia scene renderer 추출

목표: Qt용 세 번째 renderer 복사본을 만들지 않고 MAUI/Web/Qt가 같은 scene semantics를 사용하게 한다.

- [ ] `Doroti/src/Doroti.Skia.Rendering` 프로젝트를 추가하고 product solution/README/package map/naming validator에 등록한다.
- [ ] `MauiSkiaCapabilities`와 `BrowserSkiaCapabilities`의 공통 영역을 식별한다.
  - immutable pending/latest scene과 retained last frame
  - scene command dispatch와 clip/transform/save/restore
  - paint, path, text, image, paragraph, shader/runtime effect 처리
  - image/font/effect cache와 resource lifetime
  - supersede/replay/failure/present terminal state
- [ ] host-neutral renderer가 `SKSurface`, backend id, physical size, DPR, generation, background color를 입력으로 받게 한다.
- [ ] surface 생성, frame request, compositor present, platform service, native semantics transport는 각 host adapter에 남긴다.
- [ ] internal scene payload 접근은 임의 public API 확장 대신 의도적인 shared contract 또는 `InternalsVisibleTo`로 동기화한다.
- [ ] `Doroti.Skia.RuntimeEffects`를 공용 renderer에서 재사용하고 Qt 전용 shader 경로를 만들지 않는다.
- [ ] 추출 전후 고정 scene의 command/trace/terminal ACK/golden 결과가 동일한 characterization test를 만든다.
- [ ] MAUI와 Web의 기존 검증 shard를 먼저 통과시킨 뒤 Qt에서 공용 renderer를 참조한다.
- [ ] Web의 CanvasKit/브라우저 표면과 native `SKSurface` 차이는 작은 backend strategy로 격리한다.

LNX-QT-1 종료 조건:

- [ ] MAUI/Web에 동작 복제가 남지 않고 공용 renderer가 scene 결과의 단일 소유자가 된다.
- [ ] 기존 MAUI/Web 테스트와 runtime shader contract가 회귀 없이 통과한다.
- [ ] Qt spike가 같은 renderer로 고정 scene과 runtime effect를 그린다.

### LNX-QT-2. `DorotiView`와 host capability 완전 연결

목표: placeholder counter를 제거하고 Qt window 하나를 정식 Doroti view로 등록한다.

- [ ] `QtManagedState`를 `QtHostAdapter`와 명시적 lifetime 객체로 교체한다.
- [ ] 다음 capability를 생성하고 `DorotiView`에 등록한다.
  - `IViewHostCapability`
  - `IFrameHostCapability`
  - `IInputHostCapability`
  - `ITextInputHostCapability`
  - `IPlatformEnvironmentHostCapability`
  - `IPlatformServicesHostCapability`
  - `ISceneHostCapability`
  - paragraph/image/semantics capability
- [ ] MAUI의 검증된 순서를 따른다: capability 구성 -> platform message 구성 -> dispatcher register -> trace 연결 -> session attach -> surface 연결 -> show.
- [ ] `session.Start(deferFrameworkBootstrap: true)` 뒤 view를 register/attach해야만 framework bootstrap이 일어나게 한다.
- [ ] `ViewMetrics`에 physical size, DPR, insets, lifecycle, metrics generation, surface generation을 정확히 반영한다.
- [ ] close request를 Doroti에 먼저 전달하고 승인된 close 후 `Closed`, detach, resource dispose, session shutdown 순서를 보장한다.
- [ ] Qt event loop 종료 전 callback table과 `GCHandle`을 유지하고, 종료 후 native가 callback을 다시 부르지 못하게 한다.
- [ ] partial initialization 실패에서도 view/session/native resource를 역순으로 한 번만 정리한다.

LNX-QT-2 종료 조건:

- [ ] `DorotiDemoApp`의 실제 widget tree가 Qt surface에 표시된다.
- [ ] 첫 frame, resize, background/foreground, close에서 expected lifecycle/metrics trace가 나온다.
- [ ] placeholder `PresentedFrames++` 경로가 제거된다.

### LNX-QT-3. frame scheduler, present, surface recovery

목표: Qt repaint와 Doroti frame pipeline을 backpressure가 있는 하나의 clock으로 연결한다.

- [ ] `IFrameHostCapability.ScheduleFrame`은 최신 callback 하나만 pending으로 보유한다.
- [ ] managed frame request를 Qt GUI thread의 queued invocation으로 보내고 `QOpenGLWidget::update()`로 compositor repaint를 요청한다.
- [ ] `paintGL()`에서만 현재 surface를 획득하여 begin-frame -> scene raster -> flush를 실행한다.
- [ ] Qt `QElapsedTimer`와 `DorotiFrameClock` 사이에 단일 monotonic origin을 두고 epoch clock과 섞지 않는다.
- [ ] 새 scene이 없으면 retained frame replay를 사용하고 replay terminal ACK를 구분한다.
- [ ] 새 scene이 pending scene을 대체하면 이전 scene은 `superseded`로 즉시 종료한다.
- [ ] `frameSwapped()`가 surface/view generation과 일치할 때만 `presented`로 완료한다.
- [ ] hidden/minimized/occluded/inactive 상태에서 frame request가 무한 누적되지 않게 한다.
- [ ] resize, screen 이동, DPR 변경, GL context loss/recreate를 서로 다른 generation 변화로 추적한다.
- [ ] stale paint/present callback은 최신 view를 완료하지 못하게 하고 진단 counter만 남긴다.
- [ ] recoverable device/context loss와 programming/raster fault를 구분한다.
- [ ] recovery 중 마지막 정상 scene을 보존하고 새 surface에서 정확히 한 번 replay한다.

LNX-QT-3 종료 조건:

- [ ] 모든 scene의 terminal ACK 합이 submit 합과 같다.
- [ ] pending/outstanding queue high-watermark가 2를 넘지 않는다.
- [ ] full-frame CPU readback/copy, per-frame managed framebuffer allocation, GUI-thread 동기 wait가 각각 0이다.
- [ ] resize/DPR/context 재생성 중 failed frame, 영구 black frame, 무한 repaint가 없다.

### LNX-QT-4. pointer, wheel, touch, stylus, keyboard, focus

목표: Qt event를 Doroti의 canonical input data로 손실 없이 변환한다.

- [ ] ABI에 pointer device id, kind, change, buttons, physical position, pressure/tilt, signal kind, scroll delta, timestamp를 추가한다.
- [ ] Qt logical position에 DPR을 정확히 한 번 적용하여 Doroti physical coordinate로 변환한다.
- [ ] mouse add/hover/down/move/up/cancel/remove와 enter/leave/capture 상실을 완전한 sequence로 만든다.
- [ ] `QWheelEvent`의 `pixelDelta`, `angleDelta`, phase, inverted, device type을 전달한다.
- [ ] X11의 `pixelDelta` 신뢰성 차이를 고려해 angle delta 변환 규칙과 line/step 크기를 명시하고 테스트한다.
- [ ] horizontal/vertical wheel, high-resolution trackpad, scroll begin/update/end를 구분한다.
- [ ] Qt 6 pointer/touch event에서 다중 pointer id와 개별 point state를 보존한다.
- [ ] `QTabletEvent`의 stylus kind, pressure, tilt, buttons를 매핑하고 지원하지 않는 필드는 명시적 기본값을 쓴다.
- [ ] `QtKeyMap`을 별도 구성하여 native scan code/Qt key/modifiers를 Doroti physical/logical key id로 변환한다.
- [ ] press/release/auto-repeat을 down/up/repeat으로 구분하고 left/right modifier를 가능한 범위에서 보존한다.
- [ ] key event의 printable text와 IME commit을 중복 삽입하지 않는다.
- [ ] focus in/out은 lifecycle과 섞지 않고 `FocusData`로 보내며 window activation/minimize는 별도 lifecycle로 보낸다.

LNX-QT-4 종료 조건:

- [ ] button, hover, drag, nested vertical/horizontal scroll, keyboard shortcut, focus traversal 자동화 테스트가 통과한다.
- [ ] WSLg와 실제 Linux에서 mouse wheel 및 trackpad trace가 방향/크기/device id를 보존한다.
- [ ] resize/DPR 변화 후 hit test 위치가 보이는 픽셀과 일치한다.

### LNX-QT-5. text input과 한글 IME

목표: 단순 commit string 전달을 정식 editing-state 왕복 계약으로 교체한다.

- [ ] native에 현재 text input client의 surrounding text, selection, composing range, input type/action, multiline/obscure 설정, caret rect mirror를 둔다.
- [ ] `ITextInputHostCapability.SetClient`, `UpdateState`, `SetCaretRect`, `ClearClient`를 ABI의 managed -> Qt 함수로 연결한다.
- [ ] `QWidget::inputMethodQuery()`에서 최소 `ImEnabled`, `ImCursorRectangle`, `ImCursorPosition`, `ImAnchorPosition`, `ImSurroundingText`, `ImCurrentSelection`, hints를 반환한다.
- [ ] `QInputMethodEvent`의 preedit text, commit text, replacement start/length, selection/cursor/text-format attribute를 관리 editing event로 변환한다.
- [ ] selection/composing offset은 UTF-16 code unit 계약으로 정규화하고 native UTF-8 byte offset과 혼용하지 않는다.
- [ ] caret/selection/surrounding text가 바뀔 때 `QGuiApplication::inputMethod()->update(...)`를 호출한다.
- [ ] preedit underline/cursor 정보를 Doroti text field가 그릴 수 있는 composing 상태로 유지한다.
- [ ] input action(enter/done/next), backspace/delete, selection replace, emoji/surrogate pair를 테스트한다.
- [ ] focus/clear client/window close 때 composing session을 한 번만 종료한다.

LNX-QT-5 종료 조건:

- [ ] 한글 2벌식에서 preedit이 보이고 글자 조합/분해/commit이 중복 없이 동작한다.
- [ ] candidate window가 최신 physical caret 위치에 나타난다.
- [ ] 영문, 한글, emoji, selection replacement, password/multiline 입력 fixture가 통과한다.
- [ ] WSLg 결과와 실제 Linux IME(예: IBus/Fcitx)는 별도 evidence로 기록한다.

### LNX-QT-6. platform environment와 services

목표: 앱이 Linux 데스크톱 환경 변화를 Doroti 공용 계약으로 관찰하고 제어하게 한다.

- [ ] `QLocale::uiLanguages()` 등에서 locale 목록을 만들고 locale change를 전파한다.
- [ ] Qt palette/color-scheme change를 Doroti brightness로 매핑한다.
- [ ] screen/DPR/available geometry 변화에서 metrics와 configuration generation을 갱신한다.
- [ ] active/inactive, minimized/restored, hidden/shown, close requested/closed를 Doroti lifecycle 순서로 변환한다.
- [ ] `QClipboard`로 plain text read/write와 clipboard 변경을 연결한다.
- [ ] Doroti cursor 종류를 Qt cursor shape로 매핑하고 지원하지 않는 cursor의 deterministic fallback을 정한다.
- [ ] 24-hour format, text scale, spell-check 같은 Linux/Qt에서 직접 보장할 수 없는 값은 근거 있는 기본값과 `notSupported` 진단을 둔다.
- [ ] UI-thread affinity가 필요한 service 호출은 await 가능한 queued request/response로 처리하며 동기 cross-thread wait를 금지한다.

LNX-QT-6 종료 조건:

- [ ] clipboard copy/paste, cursor 변화, locale/theme/DPR 전환, minimize/restore, close veto가 live test에서 동작한다.
- [ ] service 실패가 crash나 deadlock이 아니라 명시적 실패 결과와 진단으로 반환된다.

### LNX-QT-7. semantics와 Linux accessibility

목표: canvas 한 장이 아니라 Doroti의 의미 트리와 action을 Linux 보조 기술에 노출한다.

- [ ] 기존 `SemanticsUpdateDiffer`의 snapshot/delta를 Qt adapter에 연결한다.
- [ ] `QAccessibleWidget`/`QAccessibleInterface` 기반 root와 virtual child 객체를 구성한다.
- [ ] stable semantics id로 parent/child/index, role, state, name, value, hint, focus, bounds를 제공한다.
- [ ] child hit testing과 bounds를 DPR 및 window screen coordinate에 맞게 변환한다.
- [ ] tap/focus/increment/decrement/set value/scroll/text selection 등의 action을 Doroti semantics action으로 역전달한다.
- [ ] editable text에는 필요한 `QAccessibleTextInterface`/`QAccessibleValueInterface`/`QAccessibleActionInterface`를 구현한다.
- [ ] semantics delta마다 전체 tree를 재생성하지 않고 stable object identity와 coalesced Qt accessibility event를 유지한다.
- [ ] view close와 node removal 뒤 stale accessible object가 managed callback을 호출하지 못하게 한다.

LNX-QT-7 종료 조건:

- [ ] automated tree dump가 Doroti semantics fixture의 hierarchy/role/name/state/bounds와 일치한다.
- [ ] 실제 Linux에서 AT-SPI inspector와 Orca로 탐색, focus, button action, text value를 확인한다.
- [ ] WSLg accessibility 미지원/차이는 물리 Linux PASS로 대체하지 않고 `notVerified`로 분리한다.

### LNX-QT-8. packaging, template, diagnostics

목표: 개발 머신에서만 우연히 로드되는 backend가 아니라 재현 가능한 Linux runner 산출물을 만든다.

- [ ] `SkiaSharp.NativeAssets.Linux` `4.151.1`을 중앙 버전 관리에 추가하고 Linux target/host에서만 참조한다.
- [ ] `linux-x64` publish 출력에 `libdoroti_qt_host.so`, `libSkiaSharp.so`, 필요한 managed assembly가 존재하는지 검사한다.
- [ ] `LibraryImport("doroti_qt_host")`의 .NET Linux 이름 변형/검색 규칙과 app-local 배치가 맞는지 테스트한다.
- [ ] `ldd`, `readelf -d`, `objdump`로 missing dependency, RPATH/RUNPATH, GL/Qt/Skia dependency를 산출한다.
- [ ] framework-dependent와 self-contained publish를 각각 검증한다.
- [ ] Qt를 system dependency로 둘지 함께 배포할지 별도 배포 결정으로 문서화한다.
- [ ] Qt bundle을 선택할 경우 Qt platform plugin(`wayland`, `xcb`), plugin search path/`qt.conf`, `$ORIGIN` RUNPATH, license/third-party notice를 함께 다룬다.
- [ ] system Qt 정책을 선택할 경우 최소 Qt 6.5와 필수 module/plugin 진단 및 actionable startup error를 제공한다.
- [ ] `DorotiDemoApp/linux/native`와 `Doroti/templates/.../linux`의 CMake/source/manifest를 같은 계약으로 동기화한다.
- [ ] template 생성 -> restore -> native Debug build -> Release publish를 깨끗한 임시 경로에서 검증한다.
- [ ] 기존 cross-host `Package` shard는 `DorotiBuildQtNative=false` 구조 검증으로 유지하고 Linux native 결과로 잘못 승격하지 않는다.
- [ ] 별도 `LinuxQtNative` validator/evidence shard를 추가한다.
- [ ] 진단 schema에 Qt/QPA/GL/Skia identity, metrics/surface generation, frame terminal counts, queue watermark, input counts, IME/semantics capability, dependency 목록을 포함한다.
- [ ] README와 ADR-021의 오래된 toolchain 상태를 현재 evidence에 맞게 갱신한다.

LNX-QT-8 종료 조건:

- [ ] 새 checkout에서 문서화된 명령만으로 Linux native runner를 build/run할 수 있다.
- [ ] publish 디렉터리를 별도 위치로 복사해 실행해도 native library/plugin을 찾는다.
- [ ] template runner와 Demo runner가 동일한 ABI/capability/evidence 계약을 통과한다.

### LNX-QT-9. acceptance와 장기 안정성

목표: build PASS와 실제 Linux backend PASS를 분리하여 최종 증거를 닫는다.

#### 자동 계약 게이트

- [ ] C# unit/contract test: ABI layout, view attach/detach, metrics generation, frame ACK, input map, IME state, platform services, semantics diff.
- [ ] C++ QtTest/CTest: ABI validation, enum/UTF-8 conversion, wheel/key map, inputMethodQuery/event, lifecycle ordering, accessible tree adapter.
- [ ] shared renderer golden/replay/resource-lifetime test. Offscreen raster 결과는 GPU live 증거로 승격하지 않는다.
- [ ] 기존 Inventory, Contracts, MAUI/Web, runtime-shader validator 회귀 확인.
- [ ] Debug와 Release는 공유 obj/bin lock을 피하도록 순차 실행한다.
- [ ] 모든 test/build process timeout은 저장소 지침대로 20분을 사용한다.

#### WSLg live 게이트

- [ ] WSLg가 정상 display/DRI/shared-memory 상태인지 진단하고, 환경 복구 절차와 제품 backend 변경을 구분한다.
- [ ] Wayland (`QT_QPA_PLATFORM=wayland`)와 X11/XWayland (`xcb`)를 별도 실행한다.
- [ ] 실제 demo pixels, resize, DPR, theme, mouse/drag/wheel/key, 한글 IME, clipboard, cursor, close lifecycle을 화면과 trace로 확인한다.
- [ ] context recreate와 retained scene replay를 강제로 발생시킨다.
- [ ] WSLg 결과는 VM/live harness evidence이며 물리 Linux evidence로 표시하지 않는다.

#### 물리 Linux 게이트

- [ ] 지원 기준 배포판과 GPU/driver/desktop/compositor/Qt 버전을 evidence에 기록한다.
- [ ] 실제 Wayland session과 실제 X11 session에서 각각 실행한다.
- [ ] GL renderer가 의도하지 않은 `llvmpipe`/software renderer가 아닌지 확인한다.
- [ ] mouse/trackpad/touch/stylus 가용 장치, keyboard, IBus/Fcitx 한글 IME, clipboard/cursor를 확인한다.
- [ ] AT-SPI inspector와 Orca 접근성 시나리오를 실행한다.
- [ ] screen 이동/DPR 변화, minimize/restore, suspend/resume 또는 session 재활성화, 20회 context/window cycle을 실행한다.
- [ ] 30분 interaction soak과 60분 idle/animation soak에서 memory/resource/frame counter를 비교한다.

#### 성능/정확성 게이트

- [ ] fixed scene을 MAUI/Web reference와 비교하고 허용 오차 및 backend 차이를 fixture에 명시한다.
- [ ] input sequence -> scene submit -> raster -> swap present까지 같은 monotonic trace id로 연결한다.
- [ ] 60 Hz와 가능한 고주사율 환경에서 p50/p95/p99, first present, present interval, input-to-present를 기록한다.
- [ ] 제출된 scene의 terminal ACK coverage 100%, failed 0, software fallback 0을 요구한다.
- [ ] queue high-watermark <= 2, full-frame CPU copy/readback 0, managed framebuffer allocation 0, synchronous GUI wait 0을 요구한다.
- [ ] 주사율별 latency 기준은 기존 Doroti runtime report/FCR budget과 같은 단위를 사용하고, 측정 후 기준을 완화해 PASS시키지 않는다.
- [ ] 성능 미달은 `notVerified`가 아니라 측정된 실패로 기록하고 submit/raster/present 중 병목 구간을 함께 남긴다.

## 4. 예상 파일 변경 범위

새 파일/프로젝트 후보:

- `Doroti/src/Doroti.Skia.Rendering/*`
- `Doroti/src/Doroti.Host.Qt/QtHostAdapter.cs`
- `Doroti/src/Doroti.Host.Qt/QtSkiaSurface.cs`
- `Doroti/src/Doroti.Host.Qt/QtKeyMap.cs`
- `DorotiDemoApp/linux/native/include/doroti_qt_host_v2.h`
- `DorotiDemoApp/linux/native/src/*`의 surface/input/IME/accessibility 분리 파일
- 대응하는 `Doroti/templates/.../linux/native/*`
- Qt C++ contract test와 `Doroti/tests/*Qt*` 관리 테스트
- `Doroti/eng/validate-linux-qt.ps1`
- `Doroti/validation/evidence/linux-qt/*`

수정 대상:

- `Doroti/src/Doroti.Host.Maui/MauiSkiaCapabilities.cs`
- `Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs`
- `Doroti/src/Doroti.Ui/Doroti.Ui.csproj`
- `Doroti/src/Doroti.Host.Qt/DorotiQtRunner.cs`
- `DorotiDemoApp/linux/DorotiDemoApp.Linux.csproj`
- `DorotiDemoApp/linux/native/CMakeLists.txt`
- `Doroti/Directory.Packages.props`
- product solution/package/naming/target validators
- Demo와 template Linux source
- README 및 ADR-021/evidence 문서

실제 구현 시에는 각 milestone에서 변경 범위를 다시 확인하고 사용자 소유의 unrelated worktree edit를 보존한다.

## 5. 명시적 제외 범위

- Qt Bridge/PySide6를 product renderer나 runtime hot path에 넣지 않는다.
- QML/Qt Quick migration과 디자인 시스템 재작성은 하지 않는다.
- 앱 전용 `paintGL()` 데모 그림으로 framework backend 완료를 주장하지 않는다.
- generated product만 고치거나 공용 host/runtime 계약을 우회하지 않는다.
- GPU 연결 실패를 software/CPU renderer로 숨기지 않는다.
- `linux-arm64`, AppImage, Flatpak, Snap, distro store/signing은 `linux-x64` backend closure 뒤의 후속 작업으로 둔다.
- multi-window/multi-view, embedded EGLFS, Vulkan/QRhi backend는 첫 범위에 포함하지 않는다.
- WSLg shared-memory/Copy Mode 복구를 저장소 제품 코드에 하드코딩하지 않는다.
- unrelated dependency upgrade와 MAUI/Web 동작 변경을 이 작업에 섞지 않는다.

## 6. 구현 중 중단하고 재결정할 조건

다음 중 하나가 확인되면 workaround를 누적하지 말고 해당 milestone을 멈춰 ADR/설계를 갱신한다.

- [ ] SkiaSharp가 Qt GUI thread의 current FBO/context를 안정적으로 wrap할 수 없다.
- [ ] Qt compositor swap 완료를 신뢰성 있게 scene terminal ACK에 연결할 수 없다.
- [ ] `QOpenGLWidget` context/FBO 재생성 때문에 retained resource lifetime을 결정적으로 관리할 수 없다.
- [ ] Wayland와 X11 중 하나에서 필수 input/IME/accessibility가 현재 Widgets 구조로 제공되지 않는다.
- [ ] managed-owned process가 Qt plugin/deployment/surface ownership을 근본적으로 방해한다.

이 경우 후보 비교 순서는 `QOpenGLWindow` -> `QSGRenderNode`/QRhi -> native-owned executable + `hostfxr`이며, 각 대안은 rendering, input/IME/accessibility, packaging 복잡도와 기존 ABI 폐기 비용을 함께 평가한다.

## 7. 참고 근거

- [Qt `QOpenGLWidget`](https://doc.qt.io/qt-6/qopenglwidget.html): current context, Qt-owned FBO, resize/context recreation과 cleanup 규칙
- Qt input method [event](https://doc.qt.io/qt-6/qinputmethodevent.html)/[query](https://doc.qt.io/qt-6/qinputmethod.html): preedit/commit/replacement와 custom editor query 의무
- [Qt wheel event](https://doc.qt.io/qt-6/qwheelevent.html): pixel/angle delta, device, phase, X11 주의점
- [Qt QWidget accessibility](https://doc.qt.io/qt-6/accessible-qwidget.html): `QAccessibleInterface`/`QAccessibleWidget`와 accessibility update
- [`QQuickFramebufferObject`](https://doc.qt.io/qt-6/qquickframebufferobject.html)의 legacy/OpenGL-only 제약
- [Qt Linux deployment](https://doc.qt.io/qt-6/linux-deployment.html): shared library, platform plugin, RPATH와 CMake deployment
- [.NET native library loading](https://learn.microsoft.com/dotnet/standard/native-interop/native-library-loading)과 Linux library-name resolution
- [`SkiaSharp.NativeAssets.Linux` 4.151.1](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Linux/4.151.1)
