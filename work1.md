# PlatformView 재구성 작업계획

2026-09-14 · 기준 HEAD `227a0b4c7c9534aff2cf2c9edb5b038c9d2656cc`.

설계 기준은 [idea.md](idea.md)다. **이번 수행은 소스·공식 문서 검토와 계획 재작성까지**이며 아래 R0~R7 구현은 시작하지 않았다. 기존 제품 경로는 `PARTIAL`, 재구성 단계는 `TODO`다. 과거 PV-0~PV-10 결과를 지우지 않고 [원본](history/26-09-14/platformview-rearchitecture/work1.original.md)에 보존했다.

현재 요구에 따라 **R6는 플랫폼별 WebView 전략 비교·통합, R-E는 공통 효과 의미와 비주얼 유사성을 목표로 하는 위젯**이다. HCPP 구현은 필수가 아니며 native hierarchy·live texture·bounded readback·GPU compositor 중 적합한 구성을 선택한다. Windows CoreWebView2CompositionController 선택은 유지한다. 상태는 `TODO`/`notVerified`이며 이번에도 제품 코드는 변경하지 않았다.

## 1. 목표와 범위

목표는 native instance를 보존하면서 `Doroti 배경 → native A → Doroti 중간 → native B → Doroti 전경`을 정확히 합성하고, 화면 순서에 맞는 입력·focus·semantics와 제한된 프레임 비용을 제공하는 것이다. 공통 계약을 추출한 뒤 현재 host를 순차 이관한다. WebView navigation/JS/profile은 [work2.md](work2.md)가 소유한다.

다음 C1~C6는 새 단계에서도 유지하는 제품 수용 기준이다.

| 기준 | 필수 장면과 관측 |
|---|---|
| C1 | native 위 불투명 Doroti 부분/전체 가림·해제, 같은 native identity·편집 상태 |
| C2 | native 위 반투명 Doroti, 배경·전경 이중 alpha 합성 없음 |
| C3 | Doroti 위 native 부분/전체 가림, 영역 밖 Doroti 그림 보존 |
| C4 | R/N/R/N/R, 역순·native/native 및 투명 native 조합은 capability별 승인 |
| C5 | 이동·scroll·clip·DPI·resize 동안 geometry/raster/input 정합성, 실패 frame 보존 |
| C6 | 전경 버튼·메뉴·modal shield, 노출 native 입력, 통과 정책, native-origin drag의 부모 중재 |

`NativeOverlay` B는 중간 성과이며 C1~C6 전체 목표의 대체물이 아니다. live texture는 state·입력·접근성이 검증되면 선택 전략에 포함한다. 정지 Snapshot은 명시적 별도 기능이다. group opacity/backdrop/stretch는 별도 효과표에 조건을 기록하며 지원하지 않는 조합을 생략하지 않는다.

추가 필수 장면 E1은 **live WebView → 부분 backdrop/material effect → 선명한 Doroti child**다. E2는 같은 장면의 native scroll/animation·효과 이동/resize와 pass-through/block 입력, E3는 R/N/R/N/effect/foreground에서 앞선 배경 전체 sample이다. backend가 명시적으로 금지한 효과끼리의 겹침은 성공 장면에 섞지 않고 거부 사례로 검증한다.

## 2. 재검토한 현재 상태

이 표의 실행 결과는 **기존 기록 재검토**다. 이번 턴에 앱·기기·테스트를 실행한 결과가 아니다. 파일이 없는 과거 증거를 새 PASS로 재작성하지 않는다.

| 영역 | 현재 확인한 범위 | 남은 범위 / 증거 |
|---|---|---|
| 공통 | owner registry·generation·typed scene·retained planner·lease·placement batch·codec 구현 | 순수 analyzer/planner와 제품 frame session 분리 필요. [소스](Doroti/src/Doroti.Hosting/PlatformCompositionPlan.cs), [검증 안내](Doroti/validation/platform-views/README.md) |
| WindowsAppSdk Vulkan | generic BUTTON/EDIT, HWND/raster readback 제품 B/C 및 10장면·20회 수명 기록 | 전체 IME/UIA/DPI/두 제품 창/device loss/성능/AOT `notVerified`. [현존 실행 기록](Doroti/artifacts/platform-views/2026-09-13/windows-implementation.md) |
| Windows MAUI | WindowsAppSdk와 별도 결합 대상 | 현재 Windows 제품 증거로 지원 승격 불가 |
| Android | native Button/EditText, SurfaceView base·작은 전경 readback·cache·제한 backdrop 구현 | native-origin drag, 최종 bounded revision 실행, 큰 scroll/modal 비용, IME/TalkBack·전체 수명·성능 미승인. [구현](Doroti/artifacts/platform-views/2026-09-14/android/implementation.md), [중단/실패/최종 빌드 기록](Doroti/artifacts/platform-views/2026-09-14/android/scroll/README.md) |
| AppKit | `AppKitPlatformViewHost`의 Metal/native sibling C 소스와 validator 존재 | 과거 문서의 AppKit 실행 README·scope 문서는 현재 없음. 당시 제한 PASS 기록은 원본에 보존, 현 revision 실행 승인 `notVerified` |
| UIKit / Catalyst | 별도 host·검증이 필요한 계획 | AppKit/기존 renderer 검증을 신규 PlatformView 증거로 전용하지 않음 |
| Web | main-DOM registry·iframe identity·shield 독립 harness | 제품 worker protocol 미연결. [소스](Doroti/src/Doroti.Host.Web/Web/doroti.web.platform-views.ts), [harness 범위](Doroti/validation/platform-views/README.md) |
| Linux Quick | Testbed `DorotiQtQuick=true`, Qt 소유 GPU와 R→P copy·Quick Controls C 소스 | [재현 안내](Doroti/validation/linux-qt-quick/README.md). 과거 VM/llvmpipe 실행 기록의 result.json은 현재 없음. 급격한 resize validation 2건 잔여 기록 유지, 물리 GPU/성능·WebEngine Quick 미승인 |
| Linux Widgets | generic native-child Widgets 제한 B와 선택 attachment 소스 | Quick C와 다른 경로. [검증 안내](Doroti/validation/linux-qt-contract/README.md), 과거 implementation artifact 현재 없음 |
| WebView | inappwebview 참조와 계획, Qt 독립 probe 기록 | `Doroti/src`에 WebView 이름의 제품 파일을 찾지 못함. 모든 제품 adapter는 work2 구현 대상 |

Android의 기존 spinner workload 수치를 scroll/backdrop/전체 장면 FPS로 재사용하지 않는다. 최종 arm64 APK는 빌드되었지만 disconnect로 설치·실행되지 않았고, 최종 x64 build/run도 남았다는 handoff를 유지한다. 과거 phone에 설치된 revision과 현재 HEAD도 동일하다고 가정하지 않는다.

현재 없는 문서에는 `Doroti/docs/platform-views/{contract.md,support-matrix.md,appkit.md}`, `Doroti/docs/validation/platform-views/2026-09-11/README.appkit.md`가 있다. Linux Quick의 `005943-534160/result.json`, `rapid-resize-followup/result.json`, Linux Widgets `implementation.md`도 기존 링크 위치에 없다. 존재하지 않는 파일을 링크로 다시 등록하지 않고 R0에서 회수 가능 여부를 기록한다.

## 3. 실행 순서와 단계별 gate

| 단계 | 선행 | 주 산출물 | 종료 조건 |
|---|---|---|---|
| R0 | 없음 | 현재 상태·환경·소스/evidence ledger, baseline fixture | 기존 결과와 현재 revision 검증 범위가 분리됨 |
| R1 | R0 | lifetime/attachment/capability/input/frame DTO와 API 이관표 | 두 owner·late create·old generation·요청/실제 전략 계약이 명확 |
| R2 | R1 | analyzer, mutator stack, coverage/damage, 순수 planner | 기존 장면 동등성·alpha·clip·unknown bounds 검증 |
| R3 | R1/R2 | 실제 제품이 사용하는 composition session, host adapters | 제출 후 취소·commit 실패·retirement·resize 책임 단일화 |
| R4 | R1, 제품 연결은 R3 | native gesture/focus/IME/semantics bridge | native-origin drag와 클릭·shield·Tab이 같은 제품에서 정확 |
| R5 | R2/R3, 입력 승인은 R4 | Windows/Android/Qt/AppKit 이관, Web/UIKit 연결 | backend별 C1~C6와 지원/제한표 |
| R6 | 계약 R1/R3, 해당 backend R5 | WebView 전략 비교·선택·통합 | 선택한 구성으로 C1~C6·입력·수명·효과·성능 예산 통과. HCPP 강제 없음 |
| R-E | 조사 R1, 구현 R2/R3, WebView 승인 R6 | 공통 effect intent·native/CSS/GPU adapter·시각 보정 | E1~E3·공통 비주얼 유사성·입력·동적 native·제한 검증 |
| R7 | 해당 backend R4/R5/R6/R-E | 제품·성능·배포 승인표와 가이드 | 광고할 각 조합에 현재 증거가 있음 |

Android R4의 ingress/arena 타당성 probe는 R1부터 시작한다. Qt WebEngine Quick 최소 probe도 work2와 조기에 수행해 attachment 계약의 불일치를 확인한다. 이 선행 조사가 R2/R3 구현 순서를 생략하는 근거는 아니다.

R6/R-E의 Android 전략별 backdrop sampling과 Windows 호환 Composition tree 검증도 R1부터 앞당긴다. 실제 WebView+효과 장면으로 후보를 비교하고 효과/입력 요구를 충족하지 못하는 전략은 제외한다. AppKit·Web은 native/CSS adapter의 조기 구현 대상으로 삼되 공통 reference 비주얼로 보정한다.

### R0 — 소스와 증거 기준 고정

- 위 상태표와 원본 기록을 출발점으로 환경·RID·renderer·control 종류·실제 source hash를 기록한다. 없는 증거는 unavailable로 유지한다. 결과 회수가 가능하면 먼저 재사용하고 무조건 긴 검증을 반복하지 않는다.
- `PlatformViewFixture`와 실제 Material `Platform views` 페이지의 C1~C6·scroll·spinner·modal을 묶어 fixture ID를 고정한다. 서로 다른 scene/source/빌드 결과를 한 수치로 합치지 않는다.
- Android 최종 bounded arm64 install/run 및 x64 build/run을 필요한 경우 먼저 확인한다. native-origin drag와 배경-origin drag를 별도 시나리오로 만든다.
- 0/1/4 native, idle·작은 animation·큰 scroll·modal transition을 성능 workload로 정한다. 목표 refresh rate별 예산은 `1000 / Hz` ms이며 p95/p99 허용 회귀와 메모리 상한을 변경 전 기록한다. 현재 검증되지 않은 임의 수치를 기존 PASS 기준으로 만들지 않는다.

Stop: 현재 source에 대응하는 baseline을 얻지 못하면 해당 backend 성능 향상 수치는 미산출로 남긴다. source-only 작업은 계속 가능하다.

### R1 — 공통 계약과 API 일원화

수정 위치: `Doroti.Ui/PlatformViewContracts.cs`, `Doroti.Hosting/PlatformViewCoordinator.cs`, `Doroti.Framework.Services/PlatformViewClient.cs`, `PlatformViewChannelAdapter.cs`, `Doroti.Framework.Widgets/PlatformView.cs`.

- creation descriptor, mutable settings, attachment/frame placement를 분리한다. owner allocator·explicit ID 호환·dispose completion·same-owner keep-alive를 정의한다.
- capability를 representation/transport/effect/input/observation과 runtime/control 조건으로 확장한다. 기존 `PlatformViewRequest/Support` facade의 대응과 obsolete 시점을 문서화한다. API 이름 정리는 이 계약 확정 후 한다.
- `PlatformPreferred` 전략 정책과 `PlatformEffectSupport`를 정의한다. effect intent·정규화 strength/tint/saturation·MatchCommon 기본 정책, source sampling·시각 근사·runtime availability를 분리한다. NativeCompositor/HCPP는 후보이며 필수 profile이 아니다.
- `PlatformCompositionToken`과 commit 결과에 identity/geometry/resource generation·ack provenance를 연결한다. 물리 원자성을 약속하지 않는 backend를 표현한다.
- Flutter 이식 `AndroidView/UiKitView/AppKitView/PlatformViewLink/PlatformViewSurface`와 typed `PlatformView`의 실제 호출 경로를 표로 작성한다. 두 instance를 만들지 않는 facade로 이관한다. 지원하지 않는 touch/texture/HCPP method는 명시적 오류를 유지한다.
- native 생성 시 view type뿐 아니라 host가 attach 가능한 native 종류를 검사한다. generic Button factory가 통과했다는 이유로 임의 SDK control을 등록하지 않는다.

완료: two-owner isolation, create 중 dispose/owner close, ID reuse, 늦은 focus/event, 취소 후 native 회수, detach/reattach 계약 검사. 공개 SDK/native handle 노출 없음. 입력 기본값 변경은 다음 R4 제품 gate까지 보류한다.

### R2 — 장면 분석·효과·damage 계획

수정 위치: typed layer/scene contracts, `PlatformCompositionPlan.cs`, `SkiaPlatformRasterContent.cs`, `SkiaSceneRenderer.cs` 및 검증 source.

- analyzer는 ordered mutators/group scope와 logical/device 좌표를 보존한다. backend 제약인 Android 단일 Gaussian·sigma를 common parsing에서 분리한다.
- `PlatformEffectSegment`를 raster/native/shield와 같은 ordered plan에 추가한다. sample source는 앞선 배경이며 자기 자신·child를 제외한다. effect output clip과 kernel sample 영역을 분리하고 source grouping이 paint order를 바꾸지 않게 한다.
- planner 입력은 scene + immutable capability/identity snapshot, 출력은 순서 있는 plan이다. host 호출/retain은 R3 admission에서 수행한다. plan 생성과 admission 사이 dispose race를 검사한다.
- immutable picture의 conservative coverage·content revision을 정의한다. 초기에는 실제 clip/viewport, 후속에는 기록된 draw bounds를 사용한다. unknown operation·stroke/filter 확장을 포함한다. R-tree는 측정 후 선택할 내부 최적화다.
- native overlap 밖 raster를 base에 배치하고 overlap을 제외하여 이중 합성을 방지한다. rounded/path clip hole·transparent native·backdrop sample·group opacity는 제한 정책 또는 정확한 lowering을 사용한다.
- stable slot identity와 content/geometry damage를 분리한다. viewport/DPR/GPU generation/effect 변경 시 캐시를 무효화한다. 저장공간은 active·staging·retiring 모두 계산한다.
- 새 분석 결과를 기존 planner 옆에서 비교하고 화면 제출은 기존 경로 한 번만 한다. 불일치는 picture/handle/scope 근거를 기록한다.

완료: R/N/R/N/R 픽셀 기준, 반투명 분할 재합성, fractional clip edge, non-overlap unknown bounds, retained 재사용, backdrop invalidation, 0 native fast path, budget 초과 거부. CPU Skia golden은 수학·raster 검증이며 native 제품 표시 승인이 아니다.

Stop: 분석 불확실성을 근거로 native 위 전경을 base로 내리거나 그림을 누락하면 이관하지 않는다. 보수적 큰 구간으로 유지한다.

### R3 — 실제 frame session으로 연결

수정 위치: `PlatformCompositionSession`과 Windows/Android/AppKit/Qt Draw·Finish 경로, Skia GPU submission/retirement 접점, Web protocol 제안.

- `IPlatformCompositionPresenter`/prepared 계약을 실제 host의 resource prepare·commit·retire에 맞게 확장한다. 사용하지 않는 또 하나의 session을 추가하지 않는다.
- prepare staging, native operation admission, GPU submit, backend commit, optional compositor ACK, GPU/presentation retirement를 분리한다. 기존 R/P 및 성공한 queue submit의 Vulkan observed-state 계약을 유지한다.
- commit 직전 owner/epoch/instance/surface generation을 재검사한다. 실패나 경합에서는 이전 프레임을 유지한다. 제출 후 취소·partial native commit·owner close 경로를 명시한다.
- scene/picture/texture와 native lease를 admission에서 함께 보존하고 마지막 소비 이후 반환한다. 마지막 native 제거·빈 batch·route 전환 때 이전 native/shield가 남지 않는지 확인한다.
- framework/render/UI 작업 queue와 buffer/resource 상한을 둔다. UI thread가 자신에게 보낸 작업을 기다리지 않도록 한다. GPU 대기 중 input/focus queue starvation도 관측한다.
- 하나의 host를 먼저 연결한다. Android를 첫 제품 대상으로 하고 Windows readback·Qt GPU 경로로 계약의 과도한 플랫폼 가정을 검사한다.

완료: prepare 실패, submit 실패, submit 이후 취소, resize 중 supersede, commit 실패, focus reservation 경합, 늦은 ACK, device/context loss·close에서 double free/조기 재사용 없음. 실제 host trace가 공통 session을 통과함을 확인한다. fault injection은 별도 fixture 결과로 표시한다.

### R4 — native-origin gesture와 입력 일관성

- Android wrapper가 down부터 관측 가능한지 확인하고 native가 먼저 side effect를 발생시키지 않는 delayed dispatch 경로를 만든다. arena 승리 시 replay/forward, 패배 시 폐기 또는 platform-specific cancel을 정확히 처리한다.
- 원래 MotionEvent의 pointer IDs/index/action/downTime/time·좌표 변환을 보존한다. pending queue 상한·capture 변경·multi-touch·dispose·modal 진입·재진입 방지를 구현한다.
- DirectNative를 명시적으로 유지하면서 GestureArena를 opt-in한다. native 위 tap은 한 번, native 위 vertical drag는 의도한 parent scroll로, native 내부 horizontal scroll은 해당 정책대로 동작해야 한다.
- PointerInterceptor/shield는 commit된 geometry와 framework hit-test 정책을 따른다. native 안에서 발생한 동일 이벤트를 native·framework 양쪽에 dispatch하지 않는다.
- FocusNode↔native focus, Tab/Shift+Tab, IME client 양보·한글 composing/selection, semantics subtree/중복 action 방지를 각 host adapter에 연결한다.
- UIKit delayed recognizer, desktop wheel/capture, Web same-origin/cooperative 제한은 별도 구현한다. iframe 밖에서 생긴 shield event만으로 iframe 안 drag의 중재를 주장하지 않는다.

완료: 실제 Material page에서 native tap·native-origin drag·background drag·nested scroll·modal·IgnorePointer/AbsorbPointer, 편집 중 scroll·focus 왕복, late/cancel sequence를 재현한다. 자동 입력과 물리 입력·한국어 IME·screen reader 승인을 구분한다.

Stop: native click이 이미 발생한 후 parent에 복제하는 방식이면 GestureArena capability를 켜지 않는다.

### R5 — backend별 이관과 work2 인계

| 순서 / 대상 | 구체 작업 | backend 종료 조건 |
|---|---|---|
| Android | bounded raster/cache/backdrop·surface 재생성을 공통 plan/session에 연결 | 기존 제한 유지, C1~C6·scroll 개선 재현, 최종 revision 증거 |
| WindowsAppSdk | layered HWND bank와 UI batch를 adapter로 분리 | 원래 B/C pixel·focus 보존, 실제 HWND resource 수명·성능 비교 |
| Linux Quick | Qt 소유 device·P image generation·QSG node·swap terminal 연결 | 현재 basic loop 유지, resize validation 잔여 원인 분리, XWayland/native Wayland 검증 |
| AppKit | NSView/layer order·Metal surface·hit test 연결 | 실제 제품 Graphite/Ganesh 각각 검증. 과거 artifact 부재는 새 실행 전 `notVerified` |
| Web | main DOM registry 활성화, worker multi-canvas resource/placement packet·ACK·close 연결 | worker-direct WebGPU/WebGL별 C1~C6, 2 owner·DPR·context loss·iframe identity. DOM harness와 별도 |
| UIKit / Catalyst | UIView container·Metal 전경·gesture·focus·semantics | simulator/device 및 iOS/Catalyst runner 분리 |
| Windows MAUI / Qt Widgets | runner별 native hierarchy·frame adapter 연결 또는 기존 제한 유지 | WindowsAppSdk/Quick 증거 전용 금지, B-only 조합은 C 미충족 |

선행 플랫폼 결과를 기다리는 동안 다른 backend의 소스 검토·계약 대응은 가능하다. 하지만 이미 통과한 host의 장기 성능 반복은 새 변경/실패가 정당화할 때만 한다.

work2 인계물은 typed attachment·capability snapshot·frame/input policy·dispose completion과 최소 실제 WebView 종류의 attach 검증이다. HWND와 CoreWebView2CompositionController의 RootVisualTarget, QWidget과 Quick item을 다른 attachment 종류로 협상한다. Windows WebView controller는 CoreWebView2CompositionController로 확정하며 WindowsAppSdk/MAUI 모두 적용한다. C 지원이 generic Button에만 있으면 WebView C는 아직 미지원이다.

### R6 — 플랫폼별 WebView 전략 비교·선택·통합

- Android: 기존 native hierarchy/bounded readback, live texture, 공개 GPU surface·HCPP 대응 후보를 비교한다. 실제 WebView scroll/IME/접근성·transparent/media subtree·effect sampling·frame/메모리 예산을 충족하는 구성을 선택한다. HCPP가 유리할 때만 해당 구현을 진행하며 API 34+를 전체 WebView 최소 OS로 강제하지 않는다. 후보별 OS/provider/runtime 요구를 기록한다.
- Windows: CoreWebView2CompositionController를 필수 사용하고 RootVisualTarget·raster visual·backdrop effect의 호환 API family/tree를 확정한다. GPU 전송을 우선 비교하되 bounded upload도 비용과 품질로 판단한다. SendMouseInput/SendPointerInput·cursor/focus·shield를 공통 bridge로 연결한다. WinUI와 Windows Composition 객체를 임의로 혼합하지 않는다.
- UIKit/AppKit: native WKWebView와 Metal 전경 surface·effect view를 같은 owner hierarchy에서 합성한다. Web: iframe/DOM effect와 worker canvas를 browser compositor에 연결한다. Qt: live WebEngine Quick item과 R→P GPU raster/effect를 같은 scene에서 처리한다. 이 경로를 Android Flutter HCPP API 구현으로 부르지 않는다.
- Qt Quick: queue drain/basic loop를 성급하게 바꾸지 않는다. 현재 resize 잔여 문제와 retirement가 해결된 뒤 async handoff의 이득을 측정한다.
- 같은 source/content/환경에서 CPU 전송량, UI/raster/commit p95·p99, memory/retiring backlog를 비교한다. 작은 spinner 수치를 큰 scroll 성능으로 확대하지 않는다.

R6 자체 완료: 실제 WebView로 C1~C6·identity·input·retirement를 통과하고 선택 이유·actual strategy·copy/readback/sample 비용·frame/메모리 예산을 기록한다. 기능·상태 보존을 먼저 통과한 후보끼리 성능을 비교한다. 전체 목표는 R-E와 결합한 E1~E3 및 공통 시각 유사성을 R7/WV-9에서 승인해야 완료된다. 어떤 전략도 충족하지 못하면 `PARTIAL`이며 정지 snapshot·효과 누락으로 성공 처리하지 않는다.

선택은 생성/attach negotiation 시 수행하고 callback·문서·focus identity를 보존한다. runtime 전환이 필요하면 frame/IME/capture 경계와 state-preserving handoff를 검증한다. 새 전략의 성공은 그 전략/renderer의 결과로 기록하며 원래 실패한 경로의 PASS로 바꾸지 않는다.

### R-E — PlatformEffect 위젯과 플랫폼 효과 adapter, 필수 단계

수정 예정: `Doroti.Ui` effect DTO/capability, `Doroti.Framework.Widgets`의 `PlatformEffect`, `Doroti.Framework.Rendering`의 typed effect layer, Hosting plan/session, 각 `Doroti.Host.*` effect adapter 및 Web DOM protocol. WebView 패키지와 독립이며 임의 지원 native view 위에서도 재사용한다.

| 세부 gate | 작업 | 종료 조건 |
|---|---|---|
| FX0 계약·probe | 공통 backdrop intent·strength/tint/saturation·MatchCommon 정책, source/clip/input 정의. reference scene·강도별 허용 시각 편차·플랫폼 매핑 규칙 확정 | 의미 보존·시각 유사성 평가표. material/radius 값 동일함을 유사성 근거로 사용하지 않음. source sample 검증 |
| FX1 위젯·프레임 | bounded layout, effect 뒤 선명한 child, owner/effect generation·stable resource, ordered scene payload와 common prepare/commit/retire | rebuild/scroll에서 효과 host 재생성 최소화, 마지막 effect 제거·late frame·2 owner·dispose race 검증 |
| FX2 AppKit/UIKit | NSVisualEffectView withinWindow / UIVisualEffectView public material 등을 공통 intent에 매핑하고 tint/강도를 보정. window backdrop 상태와 분리 | 공통 reference 비주얼과 WKWebView source 검증. native effect끼리 겹침/alpha/mask 제약은 대안 lowering 또는 명시적 제한 |
| FX3 Web | main DOM effect element, CSS backdrop-filter/tint/clip, pointer-events none 또는 별도 shield, protocol version 협상 | iframe + canvas + effect + foreground, same/cross-origin, backdrop-root·parent opacity·DPR·z-order·stale batch. WebGPU/WebGL 제품 검증 |
| FX4 Windows | 호환 Composition backdrop/effect brush와 SpriteVisual을 WebView2/GPU raster tree에 결합 | 실제 WebView pixels blur와 선명한 child, legacy HWND/Mica와 구분, device/resize·runtime availability 검증 |
| FX5 Android | 선택한 WebView 전략의 RenderNode/RenderEffect·live sampling adapter와 공통 strength/tint 보정 | 실제 source/animation·비주얼 유사성·전송 비용 검증. 별도 surface sample 불가이면 다른 후보 검토. window blur를 inline 효과로 가장하지 않음 |
| FX6 Qt Quick | 앞선 source group의 live GPU ShaderEffectSource + MultiEffect/ShaderEffect, effect/child 제외 | 실제 WebEngine Quick·raster sample, source identity/input 보존, feedback 없음, XWayland/Wayland·GPU lease 검증 |
| FX7 결합 승인 | E1~E3와 공통 체크무늬/글자/사진 reference, 강도·테마별 비교, 동적 source·resize·focus·접근성 | 동일 logical 크기/DPR/색공간에서 blur edge·대비·tint/밝기와 시각 검토. 사전 편차·비용/수명 기준 및 지원표 일치 |

공통 기본값은 MatchCommon과 input pass-through, effect 자체의 focus/semantics 없음이다. child semantics/입력은 유지하고 필요한 전경만 shield로 보호한다. material/blur parameter를 공통 intent로 자동 매핑하는 것은 정상 구현이며 exact-sigma는 advanced opt-in이다. private Apple CAFilter/hidden Android API를 포팅하지 않는다.

Web의 `CSS.supports`와 native API 존재는 gate 통과가 아니다. native scroll/video/animation이 바뀔 때 실제 blur도 변해야 한다. secure/protected content나 compositor sampling 경계는 명시적으로 unsupported다. system transparency/고대비 정책에 의한 material 변경 또는 앱 지정 SolidTint는 resolved 상태로 보고하며 blur 성공으로 표시하지 않는다.

Stop: 선택한 조합이 실제 source sample·live 갱신·공통 시각 편차·입력/예산을 충족하지 못하면 다른 전략/adapter를 검토한다. 모두 실패하면 해당 R6+R-E는 `PARTIAL`이다. 정지 snapshot·blur 없는 tint를 정상 근사로 취급하지 않는다. 앱이 허용한 degraded fallback/접근성 대체는 별도로 보고한다.

### R7 — 제품·성능·배포 마감

- 0/1/4-view × idle/small animation/scroll/modal × 해당 OS/RID/renderer에서 frame percentile·실제 latency 관측 범위·메모리·resource count를 기록한다.
- 두 제품 owner, 100회 제품 attach/detach/create/dispose, 빠른 resize/DPI/cross-monitor, background/resume, device/context loss·close를 수행한다. 이미 동일 source로 통과한 항목은 재사용한다.
- 한국어 IME·접근성·pointer/keyboard/capture는 실제 제품 환경에서 승인한다. 물리 장비가 없으면 `notVerified`; 사용자가 명시한 생략만 `skippedByUser`다.
- opt-in 미사용 앱의 dependency/0-view 비용, Testbed와 생성 template, final publish/install/run, NativeAOT ILC/native link/실행을 따로 검증한다.
- 실제 WebView+PlatformEffect 장면을 포함해 효과 수·sample pixels·추가 GPU pass·native content freshness를 기록한다. 새 위젯을 사용하지 않는 앱과 효과 0개에서 추가 surface/DOM node/pass가 없는지 확인한다.
- 현재 지원/제약/오류/전략 선택을 API 가이드와 지원표에 반영한다. 삭제된 과거 docs를 추정 복구하지 않고 실제 계약과 증거로 새로 작성한다.

완료: 광고할 조합의 R4/R5/R7, WebView의 R6/R-E 결합 증거가 충족된다. 일부 플랫폼·물리·성능·배포가 남으면 전체는 `PARTIAL`이다. 문서 완료와 제품 완료는 별개다.

## 4. 기존 게이트의 인계

| 기존 ID | 새 책임 | 상태 처리 |
|---|---|---|
| PV-0 | R0/R1/R7 지원표·예산 | 기존 기록 보존, 현재 근거로 갱신 |
| PV-1 | R1 registry/controller/channel/lifetime | 구현 기반 유지, 새 facade 이관은 TODO |
| PV-2 | R2/R3 scene·planner·frame | 기존 planner 유지 후 교체 |
| PV-3 | R5 Windows + WebView 필수 R6/R-E | 기존 WindowsAppSdk B/C 결과 유지 |
| PV-4 | R5 Web + R4 | DOM harness와 제품 미완료 분리 |
| PV-5 | R4 + R7 | 기존 shield 성공과 full gesture/IME/accessibility 미완료 분리 |
| PV-6 | R3/R4/R5 Android + WebView 필수 R6/R-E | 최종 bounded 실행 `notVerified` 유지 |
| PV-7/PV-8 | R5 UIKit/AppKit + R4/R7 | runner/증거 별도 |
| PV-9 | R5 Quick/Widgets + work2 WV-7 | Quick 제품 방향 유지, Widgets 이전 경로 보존 |
| PV-10 | R7 | 전체 승인 미완료 |
| PV-X | HCPP 대응 등 추가 전략·최적화와 명시적 Snapshot 기능 | HCPP 자체는 선택. 플랫폼별 적합한 WebView 전략 R6와 공통 효과 R-E는 필수 |

## 5. 검증 실행 규칙

검증 source는 `Doroti/validation/`에, build/capture/trace/report/cache는 `Doroti/artifacts/platform-views/<date>/<target>/<run>/`에 둔다. 원본/개정 이력은 `history/`에 둔다. `[sourceReviewed, build, automated, productLive, physical, nativeAot]`를 독립적으로 기록하고 source hash·dirty state·명령/exit/timeout·실제 renderer/GPU·실패·재개 명령을 보존한다.

모든 build/test/run child는 [.github 지침](.github/copilot-instructions.md)의 **20분 외부 timeout**을 적용한다. 아래는 후속 구현 시 사용할 기존 명령이며 이번 문서 변경에서는 실행하지 않았다.

```powershell
# 저장소 루트. record.py가 내부에서 1200초 timeout과 artifact 출력을 적용한다.
python Doroti/validation/platform-views/record.py common
python Doroti/validation/platform-views/record.py windows-product-live
python Doroti/validation/platform-views/record.py web-dom

# 개별 새 child 명령은 Doroti 디렉터리에서 timeout wrapper로 실행한다.
# python validation/run-with-timeout.py <실제 명령과 인자>
```

Android 명령은 [Android README](Doroti/validation/platform-views/android/README.md), Qt는 [Quick README](Doroti/validation/linux-qt-quick/README.md)를 따른다. 공유 obj/bin을 쓰는 .NET build gate는 순차 실행한다. 기존 증거 재사용 여부는 변경 파일·source hash·환경에 근거하고, 미확인 artifact를 성공 수치로 복사하지 않는다.

## 6. 결정이 필요한 항목과 종료 지점

| 항목 | 결정 단계 | 판단 근거 |
|---|---|---|
| controller 이름·기존 request facade 폐기 시점 | R1 | 모든 기존 caller·호환 API 대응과 실제 lifecycle 검사 |
| draw coverage 메타데이터·index 도입 | R2 | conservative correctness와 planner CPU/메모리 측정 |
| Android gesture delayed-dispatch 방식 | R1 probe/R4 | native tap·nested drag·multi-touch·IME 제품 결과 |
| 공통 frame API의 async/ACK 범위 | R3 | Android readback·Windows UI batch·Qt GPU 세 경로의 실제 소유권 |
| Web canvas 개수·buffer handoff | R5 Web | active runtime topology와 renderer별 C·retirement 검증 |
| 플랫폼별 WebView 전략 선택 | R1 조기 probe/R6 | 합성·입력/IME/접근성·효과·frame budget·memory·안정성·실제 전송 비용 |
| native/Web backdrop sample·material 범위 | R-E FX0/FX2~FX7 | 실제 WebView+effect 장면·동적 source·입력·OS/browser 제약 |
| 플랫폼별 최종 performance/배포 지원 범위 | R0 예산/R7 승인 | 현재 장비·OS·runtime·RID별 증거 |
