# Linux Qt 개선 검토 및 작업계획

검토일: 2026-09-23

현재 Linux Testbed의 기본 경로인 **Qt Quick + Graphite/Vulkan + 선택적 WebEngine**을 중심으로 검토했다. 먼저 GPU 오류·종료 처리를 보강하고, 키보드·포커스·접근성의 실제 동작을 개선하는 것을 권장한다. 성능 개선은 현재 동기화 비용을 측정한 뒤 진행하고, 배포는 저장소 밖 새 프로젝트와 깨끗한 Linux 환경에서 확인한다.

이 문서는 검토와 후속 구현 계획이다. 체크박스는 2026-09-24 현재 **해당 작업 문장**의 수행 여부를 표시한다. 항목 전체의 완료·승인은 아래 완료 기준과 별개다. 검증 범위와 실패·미검증 조건은 [2026-09-23 실행 결과](Doroti/validation/linux-qt-quick/work3-results-2026-09-23.md) 및 [2026-09-24 후속 결과](Doroti/validation/linux-qt-quick/work3-results-2026-09-24.md)에 기록했다. 기존 보고서의 PASS를 이번 실행 결과로 간주하지 않는다.

**전체 상태: PARTIAL.** 실물 GPU 장애, 물리 키보드·IME·touch/tablet, Orca, 전체 Tab 순환, 깨끗한 VM 배포, XWayland WSI 오류 및 일부 DPR/Wayland 창 상태는 완료 기준을 충족하지 못했다. 체크한 작업도 이 범위까지 승인했다는 뜻은 아니다.

## 1. 검토 범위와 현재 상태

- Managed host: [Doroti.Host.Qt](Doroti/src/Doroti.Host.Qt), [GraphiteVulkanQuick.cs](Doroti/src/Doroti.Skia.Vulkan/GraphiteVulkanQuick.cs).
- Native host: [Testbed Linux native](DorotiTestbedApp/linux/native), [앱 템플릿 native](Doroti/templates/Doroti.Templates/content/doroti-app/linux/native).
- 빌드·배포: [Doroti.Qt.targets](Doroti/src/Doroti.Runner.Sdk/Sdk/Doroti.Qt.targets), Linux runner/target 프로젝트.
- 기존 검증: [Qt managed 계약](Doroti/validation/linux-qt-contract/README.md), [Quick 검증](Doroti/validation/linux-qt-quick/README.md), [2026-09-20 Linux 실행 보고서](Doroti/validation/webview/linux-results-2026-09-20.md).
- 제품 계약: [Linux Qt](Doroti/docs/platform-views/linux-qt.md), [Linux WebView](Doroti/docs/platform-views/linux-webview.md).

이미 구현된 기능은 새 작업으로 중복 등록하지 않는다.

- Quick의 native/raster interleaving, 제한된 blur·saturation 효과, 공개 WebView controller API가 있다.
- GPU published/staging bank 분리, 128 MiB R/P 할당 제한, GUI 작업 큐 1,024개 제한, owner·generation 검증이 있다.
- WebView callback 해제 순서, 한글 합성 이벤트, native 내부 Tab 및 Doroti 입력창으로의 포커스 복귀에 대한 기존 회귀 검증이 있다.
- 앱 native와 템플릿 native의 파일 20개를 이번 검토에서 바이트 비교했고 모두 일치했다. 현재 불일치 문제가 아니라 향후 회귀 방지 과제다.
- 기존 실행 보고서는 llvmpipe 환경의 **PARTIAL** 결과다. 실물 GPU·물리 IME·Orca·깨끗한 OS 배포 승인을 의미하지 않는다.

## 2. 우선순위

P0는 안정성과 실패 경로, P1은 기본 사용성과 배포·성능 근거, P2는 확장성과 유지보수 개선이다. `코드 확인`은 정적 코드로 확인한 사실이고, 실제 증상 재현 여부는 별도로 적었다.

| ID | 우선순위 | 개선 항목 | 근거 상태 | 예상 규모 |
| --- | --- | --- | --- | --- |
| QT-01 | P0 | GPU device loss·대기 실패·종료 처리 | 코드 확인, 이번 장애 재현 없음 | 중~대 |
| QT-02 | P0 | 리사이즈·DPR·surface 수명 회귀 | 기존 Qt WSI 실패 보고, 이번 재현 없음 | 중~대 |
| QT-03 | P1 | 물리 키 매핑과 좌우 modifier 구분 | 코드 확인 | 중 |
| QT-04 | P1 | 실제 창 포커스와 framework/native Tab 연결 | 코드 확인 및 기존 미검증 항목 | 중 |
| QT-05 | P1 | 접근성 상태·텍스트 인터페이스·변경 이벤트 | 코드 확인 | 중~대 |
| QT-06 | P1 | native touch/tablet 및 드래그 취소 경로 | 코드 확인 | 중~대 |
| QT-07 | P1 | GUI 스레드 GPU 대기 비용 계측·최적화 | 동기 대기 확인, 병목 기여도 미측정 | 중~대 |
| QT-08 | P1 | 패키지·템플릿·clean-machine 배포 검증 | 기존 보고서에서 미완료 | 중 |
| QT-09 | P2 | ABI feature 협상과 오류 진단 정리 | 코드 확인 | 소~중 |
| QT-10 | P2 | native 코드 구조·복제본·빌드 조합 관리 | 코드 확인 | 중 |

규모는 상대적인 작업량이며 일정 확약은 아니다. QT-02의 Qt 외부 원인과 QT-07의 동기화 설계는 재현·측정 결과에 따라 범위를 조정한다.

## 3. 항목별 작업계획

### QT-01. GPU 오류가 나도 종료 상태를 일관되게 처리

**근거:** `GraphiteVulkanQuick.Begin()`은 `QueueWaitIdle`을 호출하고, copy 완료는 최대 5초의 `WaitForFences`로 기다린다. `Dispose()`는 `Check(QueueWaitIdle(...))` 이후에 자원을 정리하므로 대기 결과가 오류이면 이후 정리에 도달하지 않는다. `_disposed`도 마지막에 설정한다. `QtSkiaSurface.ReleaseGpuResources()` 역시 `quick.Dispose()`가 실패하면 이후 참조 해제·정리를 진행하지 못한다. 이는 오류 처리 구조의 위험이며, 실제 GPU 누수나 멈춤을 이번에 재현한 것은 아니다.

- [ ] 정상 종료, device lost, fence timeout, 초기화 일부 실패를 구분하는 상태와 오류 정보를 정한다.
- [ ] 오류 상태에서는 새 frame·native commit을 차단하고, pending frame/WebView/GUI 작업을 정확히 한 번 종결한다.
- [x] 정상 GPU drain이 불가능할 때의 정리 정책을 분리한다. Qt 소유 device/instance를 managed 측에서 파괴하지 않는다.
- [ ] `Dispose` 재호출, 초기화 중 예외, surface invalidation→close 순서에도 중복 callback·이중 해제가 없도록 한다.
- [x] 대기 시간·실패 operation·Vulkan result·owner/frame token을 진단에 남긴다. host 타이머만으로 드라이버 내부의 무한 대기를 취소할 수 있다고 가정하지 않는다.

**완료 기준:** device-loss/timeout 주입 계약에서 후속 프레임 금지, terminal 1회, 재종료 안전성을 확인한다. 별도 프로세스 hang 검증에는 외부 timeout을 적용한다. 정상 종료와 10회 생성/해제 회귀도 통과해야 한다. 실제 하드웨어 장애 시험은 별도 결과로 기록한다.

### QT-02. 리사이즈·배율 변경과 surface 수명 검증

**근거:** 기존 보고서에 XWayland 급격한 resize 중 `VUID-VkSwapchainCreateInfoKHR-pNext-07781`이 남아 있다. 후속 10회 실행에서 미발생한 기록은 있으나 수정 완료로 판정하지 않았다. native `Resize`/`screenChanged`, managed resize epoch, Quick의 Qt 소유 WSI를 함께 확인해야 한다.

- [x] 실패 시 Qt 버전·QPA·GPU·논리/물리 크기·DPR·resize generation·프레임 token을 한 로그로 연결한다.
- [ ] Wayland와 xcb에서 빠른 resize 10회, maximize/restore, minimize/restore, 화면 숨김/재표시, 종료 직전 resize를 검증한다.
- [ ] DPR 1/1.25/1.5/2에서 클릭·clip·caption·WebView 효과 좌표를 확인하고, 가능하면 다른 배율의 모니터 사이 이동도 검증한다.
- [x] 재현되면 Doroti의 오래된 크기 제출과 Qt WSI 내부 문제를 최소 fixture로 분리한다. Qt 버전 변경이나 프레임 요청 병합은 근거를 확보한 뒤 적용한다. *([Qt 단독 재현·후속 조사](Doroti/validation/linux-qt-quick/wsi-investigation-2026-09-24.md); XWayland 오류 자체는 미해결)*
- [ ] 관련 문서에 남은 Graphite depth-attachment 동기화 경고도 현재 구성에서 재확인하고, 별도 실패 항목으로 보존한다.

**완료 기준:** 제출 시점의 크기·generation이 일치하고, 취소·거부된 프레임은 기존 published bank를 유지한다. validation layer가 실제 로드된 실행에서 오류 여부를 판정한다. 외부 Qt 문제로 남으면 재현 조건과 지원 제한을 명시하며, 단순 exit 0이나 한 번의 미재현을 해결로 처리하지 않는다.

### QT-03. 물리 키와 논리 키 매핑 분리

**근거:** [QtKeyMap.cs](Doroti/src/Doroti.Host.Qt/QtKeyMap.cs)의 `Physical()`은 영문·숫자·기능키 및 modifier를 `qtKey`에서 만든다. 이 분기에서는 전달받은 `nativeScanCode`를 사용하지 않는다. 좌우 Shift/Ctrl/Alt/Meta도 같은 HID 값으로 합쳐지고 Enter와 keypad Enter도 합쳐진다. `QtHostAdapter.ApplyKey()`가 physical 값을 눌린 키 사전의 키로 사용하므로 구분 정보가 중요하다.

- [ ] 실제 Wayland/xcb 입력에서 scan code와 Qt key를 수집해 백엔드별 의미를 확인한다.
- [x] physical은 검증한 native scan code 변환에서, logical은 Qt key·문자에서 계산한다. scan code가 없는 합성 이벤트의 fallback은 따로 정의한다. *(Qt 소스·managed 계약 기준, 실키보드 확인은 아래 완료 기준에 남음)*
- [x] 좌우 modifier, keypad, 비영문 레이아웃, punctuation, repeat, press/release 사이 modifier 변경을 다룬다. *(합성·managed 계약 기준)*
- [x] 기존 focus-loss 시 합성 key-up 동작을 유지하고, 좌우 키를 동시에 누른 상태의 해제 회귀를 추가한다.

**완료 기준:** 레이아웃이 달라도 동일 위치의 physical key는 유지되며 logical key는 해당 레이아웃을 따른다. 좌우 modifier와 keypad가 구분되고, 양쪽 Shift를 눌렀다가 한쪽만 떼어도 나머지 상태가 유지된다. managed 계약과 물리 키보드 결과를 분리해 기록한다.

### QT-04. 실제 창 포커스와 전체 Tab 순환 연결

**근거:** [QtHostAdapter.cs](Doroti/src/Doroti.Host.Qt/QtHostAdapter.cs)의 `RequestFocus()`는 `direction`을 버리고 `FocusData` 이벤트만 발생시킨다. Qt 창 활성화나 native focus 변경 요청을 수행하지 않는다. 기존 검증은 WebView 내부 Tab과 특정 입력창으로의 복귀이며 전체 framework/native 순환의 완료 근거는 아니다.

- [x] `RequestFocus`를 실제 native 요청과 연결하고, 요청과 Qt에서 관측한 포커스 상태를 구분한다. 필요하면 capability 협상을 포함한 ABI 확장을 설계한다.
- [ ] framework 입력창→Quick control→WebView→framework 버튼의 Tab/Shift+Tab 경로를 정한다.
- [ ] hidden/disposed control, modal shield, 창 비활성화, WebView 내부 focus scope에서의 반환을 처리한다.
- [x] 운영체제에서 창 활성화 요청이 수용되지 않을 때도 실제 상태와 managed 상태가 어긋나지 않게 한다. *(관측 callback 기준으로만 상태 반영, 실제 거절 사례 검증은 남음)*

**완료 기준:** 정방향·역방향 순환, 숨김/해제 대상 건너뛰기, modal 내 포커스, 재활성화 복귀를 제품 화면에서 검증한다. Qt 실제 focus와 framework 관측 상태가 일치해야 한다. 물리 한글 IME 조합 중 Tab/클릭 이동은 별도 실입력 항목으로 남긴다.

### QT-05. 접근성 정보 손실과 과도한 전체 갱신 개선

**근거:** `DorotiAccessibleNode`는 Action 인터페이스만 제공한다. editable/selectableText 상태는 설정하지만 Text/EditableText 인터페이스는 제공하지 않고 `setText()`도 비어 있다. managed가 보내는 `selected`, `textSelectionBase`, `textSelectionExtent`를 native `ApplySemantics()`가 읽지 않는다. 갱신 알림은 전체 `ObjectReorder` 중심이다.

- [x] 선택·체크·토글·확장·포커스 등 공통 semantics와 Qt 상태의 대응표를 작성하고 빠진 상태를 연결한다.
- [ ] 텍스트 조회, 선택 범위, caret, 편집 액션에 필요한 접근성 인터페이스를 구현한다. 공통 semantics가 부족하면 전달 모델도 함께 보완한다.
- [x] node 변경을 비교해 focus/value/text/selection/state 이벤트를 보내고, 구조가 바뀔 때만 구조 변경 이벤트를 발생시킨다.
- [x] Doroti semantics 루트와 Quick/WebView 접근성 트리 연결을 검사해 중복 노출·native 하위 트리 누락 여부를 확인한다. *(WebView 하위 트리 누락 확인, 연결·Orca 완료는 남음)*

**완료 기준:** 버튼·체크박스·탭·슬라이더·텍스트 필드의 상태와 값이 접근성 조회에 반영된다. Orca에서 caret/선택/값 변경과 framework↔WebView 이동을 확인하고 자동 인터페이스 검증과 실제 읽기 결과를 구분한다.

### QT-06. native 입력 소유권을 touch/tablet까지 확장

**근거:** `doroti_qt_quick.cpp`의 `DorotiQtQuickNativeInput()`은 key/IME와 mouse/wheel을 처리하고 touch/tablet는 처리하지 않는다. native drag 소유권도 `nativeDrag` boolean 하나다. host에 공통 touch/tablet 변환이 있다는 사실만으로 Quick/WebView의 native touch 지원이 완성되지는 않는다.

- [ ] touch point·device·mouse button별 입력 소유권과 capture 종료 조건을 정의한다.
- [ ] committed paint order와 shield를 기준으로 touch/tablet 시작 대상을 정하고 후속 move/up/cancel을 같은 대상에 전달한다.
- [x] drag 중 native view 삭제·숨김, 창 비활성화, pointer grab 상실, 여러 mouse button 사용 시 상태를 정리한다.
- [ ] native 내부 scroll과 부모 framework scroll의 제스처 중재는 별도 capability로 설계한다. 단순 native event routing과 GestureArena 통합을 구분한다.

**완료 기준:** 지원 대상으로 정한 입력에서 시작·이동·해제/취소가 누락·중복되지 않고 shield를 통과하지 않는다. 합성 이벤트 계약에 더해 touch/tablet 실기기 결과를 기록한다. 지원하지 못한 gesture는 계약에 명시한다.

### QT-07. 동기 GPU 대기의 비용부터 계측

**근거:** Quick은 `QSG_RENDER_LOOP=basic`을 강제하고 GUI/render owner에서 `QueueWaitIdle`과 copy fence 대기를 수행한다. 이 대기는 published P image를 보호하는 현재 수명 규약의 일부다. 기존 0/1/4 WebView workload는 llvmpipe의 10초 관측이며 변경 전 기준선과 성능 합격 예산이 없다.

- [x] 프레임별 CPU recording, queue idle, copy submit/fence wait, native commit, Qt swap interval을 분리해 p50/p95/p99와 sample count를 수집한다.
- [x] 동일 Release·QPA·GPU·화면 크기·DPR에서 0/1/4 WebView × idle/animation/scroll/modal 기준선을 만든다. *([Release/Wayland/llvmpipe 12조합 결과](Doroti/validation/linux-qt-quick/results-2026-09-24/release-workloads.json))*
- [x] process-tree PSS, R/P reserved/peak bytes, retiring layer 수, WebEngine 프로세스 수, idle CPU를 함께 기록한다. *(4 WebView modal에서 PSS 읽기 실패 표본 1개 기록)*
- [x] 계측으로 확인된 병목부터 개선한다. 반복 QML component 준비나 불필요한 frame 요청 등 작은 변경도 먼저 실측한다. *(idle frame 재요청 제거 전후 측정)*
- [ ] queue-wide drain을 줄여야 한다면 Qt sampling 완료를 보장하는 동기화·bank retirement 설계를 먼저 검증한다. `QueueWaitIdle` 제거 또는 threaded loop 전환만으로 끝내지 않는다.

**완료 기준:** 같은 조건의 전후 결과와 변경별 비용을 제시하고 합의한 성능 예산을 별도 기록한다. 기존 bank 격리·거부된 commit 보존·Vulkan validation 계약을 유지해야 한다. swap interval을 물리 표시 지연으로 해석하지 않고, idle sample이 없으면 percentile은 null로 기록한다.

### QT-08. 실제 소비자 기준의 빌드·배포 검증

**근거:** 기존 배포 근거는 같은 개발 머신에서 publish 디렉터리를 이동해 실행한 결과다. Qt/WebEngine은 시스템 의존성이며, package-only 새 프로젝트·깨끗한 OS 설치 검증은 미완료다. CMake는 Quick 사용 여부와 무관하게 Widgets/OpenGL/OpenGLWidgets 및 Wayland 개발 도구를 요구한다.

- [ ] 지원할 Linux 배포판·Qt 조합과 xcb/Wayland 런타임 의존성을 명시한다. CMake의 최소 API 버전과 실제 검증 버전을 분리한다.
- [x] 저장소 밖 새 디렉터리에 템플릿 앱을 만들고 로컬 NuGet 패키지만 사용해 build/publish한다. 소스 ProjectReference 우회를 금지한다.
- [ ] framework-dependent와 self-contained 디렉터리 배포를 깨끗한 VM에서 실행한다. 실제 로드된 shim/Skia/Qt 경로·hash를 남긴다.
- [x] host/QPA plugin/QML module/WebEngine helper·pak·locale 누락을 각각 확인한다. 기존 typed 오류 외에 시작 전 점검으로 설명할 수 있는 누락을 보완한다. *([사전 점검과 누락 주입 결과](Doroti/validation/linux-qt-quick/results-2026-09-24/runtime-preflight.json))*
- [x] Quick+WebEngine ON, Quick ON/WebEngine OFF, Quick OFF/WebEngine OFF 조합과 ON→OFF 재빌드에서 잔여 shim/manifest를 검증한다.
- [x] 불필요한 의존성 분리는 실제 include/link 요구를 조사한 뒤 수행한다. xcb-only 선택 지원도 필요성과 유지 비용을 확인한 뒤 결정한다. *(Quick의 직접 Qt OpenGL·OpenGLWidgets 개발 의존성 제거; xcb-only build는 현재 제공하지 않음)*

**완료 기준:** 지원 조합의 새 앱이 개발용 Qt/QML 경로 없이 실행된다. WebEngine OFF 결과에는 해당 link dependency와 배포 잔여물이 없어야 한다. NativeAOT·trimming·single-file은 현재 거부 정책을 유지하고 지원 전환은 별도 작업으로 분리한다.

### QT-09. feature 협상과 native 오류 정보 개선

**근거:** `QtWebViewSession`은 `_api.Features != 31`이면 실패하고, `QtPlatformViewHost`는 알려지지 않은 feature bit를 거부한다. 기능 추가 시의 호환 규칙이 명확해야 한다. WebView `Check()`는 native status 71/73 이외를 대부분 `Unsupported`로 바꾸므로 invalid argument·wrong thread·native exception 등을 구분하기 어렵다.

- [x] ABI별 필수/선택 feature와 unknown bit 정책을 문서화한다. bit가 모두 단순 추가 기능임이 확인된 경우 required mask 검사와 capability별 노출을 적용한다.
- [x] 버전·struct size·필수 함수 포인터 검증은 유지하고, 임의의 큰 struct 수용은 별도 ABI 설계 없이 도입하지 않는다.
- [x] native status를 공통 오류로 옮기는 표를 만들고 원래 status·operation·owner 정보를 보존한다.
- [ ] 기존 shim, 필수 bit 누락, unknown bit, wrong thread, stale owner, native exception을 독립 계약으로 검증한다.

**완료 기준:** 호환 가능한 기능 추가는 규칙에 따라 수용하고, 비호환 조합은 필요한 정보와 함께 시작 전에 거부한다. 잘못된 호출이 모두 기능 미지원으로 보이지 않아야 한다.

### QT-10. native 유지보수와 회귀 실행 절차 정리

**근거:** `doroti_qt_host.cpp`는 약 2천 줄에 렌더링·입력·IME·caption·접근성·Wayland backdrop을 함께 담고 있다. Quick/WebView 구현에는 여러 처리가 한 줄에 압축된 부분이 많다. native 복제본 20개는 현재 일치하지만 변경 때마다 양쪽을 유지해야 한다. `.github`에는 이번 검토 시 Qt 검증 workflow가 없다.

- [x] 먼저 native format 기준과 source↔template 차이 검사 도구를 추가한다. template의 app-owned customization 구조는 유지한다.
- [ ] 접근성, 입력/IME, backdrop, Quick owner/commit 등을 동작 변경과 별도 단계로 분리한다.
- [x] shader source/qsb/hash metadata, ABI header, QML, CMake까지 복제본 검사에 포함한다.
- [x] 기존 스크립트를 실행하는 통합 명령과 결과 index를 추가한다. build/driver/QPA/artifact 경로를 파라미터화한다.
- [ ] display가 없는 환경의 managed 계약과 실제 compositor/GPU가 필요한 제품 검증을 나눈다. 실행 환경을 확보한 뒤 CI에 연결한다.

**완료 기준:** native 변경 누락은 자동으로 검출되고, 한 명령으로 해당 검증 묶음과 실패 로그를 찾을 수 있다. 구조 변경 전후 ABI·입력·GPU 계약 결과가 같아야 한다.

## 4. 권장 실행 순서

1. **기준선 확보:** 현재 commit·빌드 조합·환경과 기존 계약 결과를 저장한다. QT-10의 template 비교와 통합 결과 index를 먼저 준비한다.
2. **안정성:** QT-01 실패 경로를 보강하고 QT-02 재현·진단을 진행한다. Qt 외부 문제는 독립 추적한다.
3. **기본 데스크톱 사용성:** QT-03 키 매핑 → QT-04 포커스 → QT-05 접근성 → QT-06 입력 소유권 순으로 작은 변경을 만든다.
4. **성능:** 기준선 측정은 초기에 수행할 수 있으나, 동기화 구조 변경은 QT-01/02 계약이 확보된 후 진행한다.
5. **배포:** QT-08을 통해 Testbed뿐 아니라 새 앱 소비 결과를 확인한다. QT-09 호환 정책과 QT-10 구조 분리는 독립 변경으로 진행한다.

각 변경에는 수정 코드·template 동기화·관련 계약·제품 재현 결과를 함께 남긴다. 기능 변경과 대규모 format/refactor는 별도 변경으로 나눠 검토 가능하게 한다.

## 5. 검증 계획

| 검증 단계 | 대상 | 합격 근거 |
| --- | --- | --- |
| Managed 계약 | ABI layout, geometry, 추가 key/focus/error 계약 | 실제 exit code와 계약 결과 |
| Native 계약 | owner/thread/lifetime/input/commit | `native-contract`, 필요 시 `webview-contract` |
| GPU 계약 | bank 격리, rejected commit, cancel, budget, 추가 오류 경로 | 현재 빌드 shim에 연결된 GPU driver 결과 |
| 제품 입력·합성 | Quick control/WebView/effect/floating panel | 기존 verify 스크립트, 캡처 및 동작 assertion |
| QPA·DPR | Wayland와 xcb, 배율·resize·창 상태 전환 | 실제 backend와 validation-layer 로드 근거 |
| 물리 입력·접근성 | 한국어 IME, 키보드, touch/tablet, Orca | 장치·입력기·절차가 기록된 수동 결과 |
| 성능 | 동일 환경 전후 workload | sample count 포함 percentile, PSS, GPU 대기 분해 |
| 배포 | 새 template 앱, package-only, 깨끗한 VM | build/publish/run과 실제 로드 모듈 근거 |

기존 명령은 [Quick 검증 README](Doroti/validation/linux-qt-quick/README.md)를 기준으로 사용한다. 아래는 첫 managed 확인 명령이다.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet run \
  --project Doroti/validation/linux-qt-contract/Contract.csproj
```

- .NET build는 shared obj 충돌을 피하도록 순차 실행한다.
- 반복 lifecycle/resize는 기존 검증 정책에 맞춰 최대 10회로 시작한다.
- 실행마다 새 artifact 디렉터리를 만들고 실패 시도도 보존한다.
- Release 제품 검증 driver는 같은 Release shim을 기준으로 다시 빌드한다.
- xcb가 XWayland 위에서 동작하면 순수 X11 검증과 구분한다.
- 미실행·환경 부족은 `notVerified`, 명시적 비지원은 `unsupported`, 실제 실패는 `failed`로 기록한다.

## 6. 이번 검토에서 수행한 확인

- 소스·빌드 설정·계약 문서·기존 실행 보고서를 정적으로 검토했다.
- 로컬 도구 버전: .NET SDK `10.0.400`, Qt `6.10.2`.
- Testbed/template native 파일 20개가 일치함을 확인했다.
- managed 계약의 첫 `--no-restore` 실행은 `project.assets.json`이 없어 `NETSDK1004`로 중단됐다. 복원을 포함한 위 명령으로 재시도하여 exit 0과 `PASS: Qt ABI and Quick/Widgets geometry contracts`를 확인했다. GPU driver 인자를 전달하지 않았으므로 GPU 계약 통과를 의미하지 않는다.
- 이번 검토에서는 GUI 제품 실행, 물리 입력, GPU 장애, 성능 workload 및 clean-machine 배포를 수행하지 않았다.

## 7. 별도 범위로 남길 기능

여러 blur 효과, 임의 transform, QWidget/Quick 통합 확대, WebView 권한·파일·다운로드·fullscreen 공개 정책, 전체 profile 저장소 삭제, 일반 remote messaging, 여러 완전한 제품 창, Linux NativeAOT는 별도 설계 과제다. 기본 안정성·입력·접근성·배포 검증이 갖춰진 뒤 실제 제품 요구에 따라 우선순위를 정한다.
