# Doroti 접근성 트리의 위치 증분 갱신 계획

2026-11-26 KST · 소스 검토 기준 `34a26d8c2de1` · 상태 **계획 작성 완료 / 구현 대기**

## 1. 수정 판단

노드 ID, 읽기 순서, 노출 여부, 레이블·값·동작·포커스가 같고 위치만 움직이면 기존 접근성 노드에 새 좌표를 적용한다. 현재 **Windows App SDK와 Linux Qt는 이때도 네이티브 트리 자료구조를 다시 만들고, AppKit과 Mac Catalyst는 Apple 접근성 속성과 액션을 다시 설정한다.** 이 플랫폼을 우선 수정한다. Android는 가상 노드 방식에서 접근성 소비자가 활성일 때와 MAUI 오버레이를 쓸 때 각각 줄일 작업이 있다. Web은 기존 DOM 증분 갱신을 활용하면서 공통 좌표 변환을 개선한다.

코드에서 반복되는 작업과 잘못 분류되는 알림을 확인했다. 플랫폼별 처리 시간과 프레임 개선량은 미측정이다. 이번 변경은 계획 문서다.

### 11월 24일 iOS 수정에서 가져올 기준

`fb07c525`의 iOS 전용 분기에서 `UpdateNode`는 최신 노드 데이터를 보관하면서 `bounds`·`scroll`만 바뀐 경우 Apple 접근성 속성 갱신 호출을 건너뛴다. 같은 커밋의 MAUI 공통 변경은 부모 기준 좌표가 실제로 달라질 때만 `SetLayoutBounds`를 호출한다. 구조가 바뀌면 기존 자식의 소속과 순서를 비교해 필요한 컨테이너만 옮긴다. 따라서 단순 이동은 주로 좌표 쓰기로 끝나며, 변경 전에는 노드가 재사용되어도 Apple 레이블·값·자식 배열·사용자 지정 액션을 반복해서 설정했다. [노드 갱신](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.cs#L11), [공통 자식 순서와 위치 반영](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs#L434), [Apple 접근성 속성](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.Apple.cs#L60)

같은 커밋에 iOS 전용 스크롤 병합도 있다. VoiceOver와 Switch Control이 모두 꺼진 활성 스크롤 중에는 `automatic` 스냅샷의 필터링·비교를 67ms 적용 간격의 콜백까지 미루고, 대기 중 받은 최신 스냅샷을 적용한다. 일반 위젯 경로의 빌더는 기본 `automatic`을 쓰므로 이 값만으로 좌표 전용 갱신임을 판별할 수 없다. 다른 플랫폼에 옮길 기준은 **좌표와 접근성 속성의 분리**다. [iOS 스크롤 분기](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs#L77), [빌더의 갱신 생성](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs#L4064)

### 현재 경로별 근거

| 경로 | 단순 이동에서 현재 발생하는 일 | 판단 |
| --- | --- | --- |
| macOS AppKit · Mac Catalyst | `MauiSemanticsBridge.Apply`가 `none`을 포함한 모든 노드에서 `UpdateState`를 호출한다. iOS 외 분기는 `Node` setter를 거쳐 `RefreshAppleSemantics`를 수행한다. AppKit은 `NSObject` 값, 자식과 액션 배열을, Catalyst는 UIKit 자식·액션 배열과 레이블·traits를 다시 설정한다. 부모 기준 좌표 캐시는 이 속성 갱신에 관여하지 않는다. [MAUI 적용](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs#L333), [분기](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.cs#L11), [Apple 갱신](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.Apple.cs#L60) | **수정.** iOS와 같은 속성 분리를 플랫폼별로 적용한다. |
| Windows App SDK | 관리 호스트가 전체 노드를 JSON으로 직렬화하고 네이티브 호스트가 갱신마다 `WM_APP` 메시지를 게시한다. `AccessibilityBridge::Update`는 숨김 필터를 거쳐 기존·신규 노드 맵을 각각 복사하고, 이전 자식 목록도 복제하며, 노드·부모·루트를 다시 만든다. 속성 이벤트 전송 전에는 생존 노드 전체를 비교한다. 구조 알림 자체는 비교 결과로 제어하며 UIA 노드의 Runtime ID는 노드 ID를 사용한다. [관리 전송](packages/platforms/windowsappsdk/Doroti.Host.WindowsAppSdk/WindowsManagedProductHost.cs#L890), [네이티브 큐](packages/platforms/windowsappsdk/native/src/exports.cpp#L1107), [UIA 트리와 이벤트](packages/platforms/windowsappsdk/native/src/accessibility_bridge.cpp#L701), [UIA Runtime ID](packages/platforms/windowsappsdk/native/src/accessibility_bridge.cpp#L296) | **수정.** 네이티브 자료구조와 전송량 양쪽에서 위치 전용 경로를 만든다. |
| Linux Qt | 전체 JSON을 `QueuedConnection`으로 넘기고 콜백과 `ApplySemantics`에서 같은 JSON을 두 번 파싱한다. 새 노드 맵과 부모 관계를 만들고, 생존 노드는 변경 종류를 검사하기 전에 `Accessible(id)`를 부른다. 통합 제목 표시줄에서는 기존 맵에만 있는 네이티브 버튼이 크기 비교와 ID 제거에 먼저 반영되고, 이후 버튼을 다시 추가한다. 이 순서 때문에 같은 앱 트리의 위치 갱신에도 `ObjectReorder`가 발생하고 캐시된 제목 표시줄 인터페이스가 제거될 수 있다. [관리 전송](packages/platforms/qt/Doroti.Host.Qt/QtHostAdapter.cs#L548), [큐와 파싱](packages/platforms/qt/native/src/doroti_qt_host.cpp#L558), [제목 표시줄](packages/platforms/qt/native/src/doroti_qt_host.cpp#L1297), [트리와 이벤트](packages/platforms/qt/native/src/doroti_qt_host.cpp#L2037) | **수정.** 거짓 순서 변경과 이중 파싱을 없애고 위치를 제자리 갱신한다. |
| Android MAUI | 기본 Vulkan 경로는 가상 노드 ID를 재사용한다. 소비자가 없으면 최신 스냅샷을 보관하고 조회·액션 시 반영한다. 소비자가 활성일 때는 좌표만 바뀌어도 `EnsureLatestNodes`가 노드 맵과 노출 순서를 다시 만들고 `InvalidateRoot()`를 호출한다. MAUI 오버레이 경로는 별도로 `RefreshNativeSemantics`에서 delegate를 새로 만든다. [브리지 선택](packages/platforms/maui/Doroti.Host.Maui/DorotiMauiSurface.cs#L277), [가상 노드 적용](packages/platforms/maui/Doroti.Host.Maui/MauiAndroidSemanticsBridge.cs#L35), [오버레이 delegate](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.cs#L36) | **경로별 수정.** 가상 노드의 좌표·알림과 오버레이의 delegate 수명을 개선한다. |
| Web | 빈 변경은 전송하지 않고, 위치 변경은 `contentUnchanged` 패치로 보내 DOM 요소와 리스너를 재사용한다. 부모가 이동하면 필요한 자식의 상대 좌표를 다시 계산하며 실제 CSS 값이 달라질 때만 위치 값을 쓴다. 스크롤 오프셋 변경은 전송 계층에서 별도 콘텐츠 비교에 들어가지만 DOM의 콘텐츠 서명에서는 빠진다. [전송](packages/platforms/web/Doroti.Host.Web/BrowserSemanticsTransport.cs#L22), [패치 적용](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.semantics.ts#L41), [DOM 적용](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts#L1632) | **공통 개선 적용 및 회귀 검증.** 오프셋 전송 비용은 별도로 계측한다. |
| Windows MAUI · iOS | Windows MAUI의 `SemanticsPeer`는 조회 시 최신 `Node`를 읽고 자식 peer 목록을 캐시한다. iOS는 위치·스크롤 분리를 적용했다. 두 경로 모두 MAUI `Apply`의 트리 인덱스 생성, 전체 순회와 액션 콜백 재설정을 공유한다. [WinUI peer](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.Windows.cs#L27), [공통 적용](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs#L333), [액션 연결](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs#L715) | **공통 개선 적용 및 회귀 검증.** |

모든 경로 앞의 `SemanticsOwner.sendSemanticsUpdate`는 dirty 노드만 보낸다. `SkiaSceneRenderer.Update`가 이를 누적한 뒤 루트에서 도달할 수 없는 노드를 검사하고, `ToViewCoordinates`로 전체 좌표를 투영한 뒤 정렬해 완전한 스냅샷을 전달한다. 자식이 하나만 움직여도 여기서 전체 작업으로 확대된다. 프레임워크에는 이미 스크롤 중 순수 geometry를 보류하고 정지 후 내보내는 경로가 있다. 오프셋까지 같은 방식으로 보류하는 선택은 현재 소비자가 비활성인 Android 가상 노드 경로에서 제공한다. [dirty 노드 생성](Doroti/src/Doroti.Framework.Semantics/semantics.cs#L4034), [공통 스냅샷](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs#L1413), [좌표 변환](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs#L3486), [프레임워크 보류](Doroti/src/Doroti.Framework.Widgets/binding.cs#L1340), [Android 조건](packages/platforms/maui/Doroti.Host.Maui/MauiHostAdapter.cs#L179)

## 2. 구현 순서

### W3-01 · AppKit·Mac Catalyst와 MAUI 공통 적용

- [ ] 최신 노드 및 액션 소유자는 항상 갱신한다. Apple 속성은 플랫폼에서 읽거나 쓰는 값이 바뀔 때만 다시 투영한다. 자식 배열은 소속·순서·네이티브 핸들이 바뀔 때 갱신하고, 액션 배열은 노드 ID·핸들·허용 액션이 바뀔 때 갱신한다. 핸들 부착·교체, 노드 제거, 노출 상태 변화는 각각 처리한다. 현재 differ가 텍스트 사각형 변경을 `metadata | bounds`로 표시하므로 Apple에서 소비하는 필드 비교를 기준으로 한다. [Apple 속성과 액션](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.Apple.cs#L60), [변경 분류](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs#L3806)
- [ ] 노출과 구조가 같으면 필터링된 노드 목록과 부모·자식 인덱스를 재사용한다. 현재 `SemanticsTreeSnapshot`은 자식 목록을 복사하고 노드마다 조상 경로의 순환을 확인한다. 보류 여부 판단과 `Apply`가 같은 기준 트리를 쓰면 diff 결과를 재사용한다. 좌표 갱신에서는 이동한 노드와 부모 원점의 영향을 받는 자식의 상대 좌표를 계산해 실제 변화에만 쓰기를 수행한다. 동일 스냅샷도 최신 콜백 소유권을 전달하며 기존 바인딩을 재사용한다. [수신과 적용](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs#L77), [트리 인덱스 생성](Doroti/src/Doroti.Ui/SemanticsTreeSnapshot.cs#L10)
- [ ] 위치만 바뀐 AppKit·Catalyst·iOS 노드의 네이티브 객체·액션 배열·자식 배열을 유지한다. 새 핸들 연결과 실제 자식 삽입·제거에서는 읽기 순서 및 액션이 즉시 반영되는지 확인한다. 같은 차이를 Windows MAUI peer의 현재 속성 조회와 자식 캐시로 검증한다.

### W3-02 · Windows App SDK·Qt 네이티브 반영

- [ ] Windows는 최종 노출 트리에서 구조 변경과 내용·좌표 변경을 구분한다. 구조가 같으면 네이티브 노드에 바뀐 값과 좌표만 반영하고 부모·루트 및 기존 Runtime ID를 재사용한다. UIA 이벤트에 필요한 이전·현재 값을 변경 노드에서만 수집하고 트리를 게시한 뒤 잠금 밖에서 알린다. `UiaClientsAreListening()`은 알림 작업을 생략하는 데만 사용한다. 이후 `WM_GETOBJECT`와 좌표·텍스트·액션 조회는 항상 현재 데이터를 읽어야 한다. [UIA 적용](packages/platforms/windowsappsdk/native/src/accessibility_bridge.cpp#L701), [Microsoft UI Automation provider 계약](https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-serversideprovider#events-from-providers)
- [ ] Qt에서 검증한 `QJsonDocument`를 `ApplySemantics`에 전달해 한 번만 파싱한다. 같은 제목 표시줄 상태를 포함한 최종 트리끼리 비교하고 실제 삭제된 ID의 인터페이스만 해제한다. 구조가 같으면 노드 위치를 제자리에서 갱신하고, 이벤트가 필요한 값이 달라진 뒤에만 `Accessible(id)`를 요청한다. 네이티브 제목 표시줄의 최소화·최대화·닫기, 첫 앱 트리 전의 루트와 `Clear` 뒤의 루트도 검증한다. 알림 비용을 `QAccessible::isActive()`로 줄이는 경우에도 트리 조회는 유지한다. [Qt 전송 콜백](packages/platforms/qt/native/src/doroti_qt_host.cpp#L558), [Qt 적용](packages/platforms/qt/native/src/doroti_qt_host.cpp#L2054), [Qt 접근성 조회와 알림 계약](https://doc.qt.io/qt-6/qaccessible.html#isActive)

### W3-03 · 공통 좌표 투영과 Windows·Qt 전송

- [ ] `SkiaSceneRenderer`에서 view별 원본 노드·부모 관계·투영 결과를 보존한다. 내용만 바뀌면 해당 노드의 기존 bounds를 재사용한다. 좌표가 바뀌면 해당 노드와 조상 변환·클립의 영향을 받은 하위 노드만 다시 투영한다. 텍스트 레이아웃·클립·변환의 변화는 텍스트 사각형에 따로 반영하고 선택 범위는 필요한 내용 갱신으로 보낸다. 자식 구성·도달성·정렬 키가 달라질 때 해당 인덱스를 갱신한다. 루트 DPR 변경과 뷰 재부착에서 필요한 캐시를 무효화한다. 전체 스냅샷을 소비하는 호스트에는 동일한 순서와 내용을 유지하고, 첫 구현은 현행 전체 투영 결과와 노드별로 대조한다. [공통 렌더러](Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs#L1413), [전체 좌표 투영 기준](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs#L3486)
- [ ] Windows와 Qt는 첫 연결·재연결에 전체 스냅샷을 보내고, 이후 좌표만 바뀌면 ID와 변경 좌표를 보낸다. 스크롤 값과 텍스트 사각형은 실제 변화가 있을 때 해당 필드만 함께 보낸다. 동일 결과는 전송을 생략한다. 실제 내용과 구조 변경은 필드 갱신·추가·제거로 보낸다. 숨김 조상 및 기존 호스트의 노출 규칙을 적용한 결과를 기준으로 삼는다. 기존 네이티브 공급과 호환되는 전체 스냅샷 경로를 두고, 증분 형식은 상대가 지원하는 경우에 사용한다. [Windows 네이티브 계약](packages/platforms/windowsappsdk/native/include/doroti_windows_host_v1.h#L179), [Qt 네이티브 계약](packages/platforms/qt/native/include/doroti_qt_host_v2.h#L229)
- [ ] 전송 순서는 view와 연결 수명별 `revision`·기준 `revision`으로 관리한다. 프레임워크의 기본 경로는 매 `sendSemanticsUpdate`에서 새 빌더를 쓰며 `build()`가 빌더 내부에서 세대 값을 올린다. 따라서 기존 `generation`만으로 순서나 유실을 판단할 수 없다. 새 전송 순번은 UIA TextRange 수명이나 문단 revision과 분리한다. 최신 좌표로 병합할 때는 아직 반영되지 않은 변경을 합치거나 마지막 반영 기준에서 다시 차이를 만든다. 첫 연결·기준 불일치·`Clear`·재부착을 처리하고 늦은 콜백은 이전 연결에 귀속시킨다. [빌더 생성](Doroti/src/Doroti.Framework.Widgets/binding.cs#L754), [빌더 세대](Doroti/src/Doroti.Ui/GraphicsAndSemanticsContracts.cs#L4064), [UIA TextRange 유효성](packages/platforms/windowsappsdk/native/src/accessibility_text.inc#L21), [Web의 기준 불일치 복구](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.semantics.ts#L41)

### W3-04 · Android 가상 노드와 MAUI 오버레이

- [ ] 가상 노드 경로는 기존 소비자 판별, 최신 대기 스냅샷, 조회 시 적용을 재사용한다. 활성 소비자에게는 기존 ID와 노출 순서를 유지하며 좌표가 바뀐 노드만 갱신한다. 추가·삭제·노출·순서 변화 시 루트 알림을 사용하고, 단일 노드의 속성·위치 변경은 `InvalidateVirtualView`가 제공하는 알림으로 TalkBack에서 검증한다. 큰 스크롤 묶음에서는 노드별 알림을 무더기로 보내기보다 기존 루트 알림 한 번과 비교한다. 네이티브 조회·액션은 최신 좌표와 노드를 사용하고 제거된 포커스는 기존 순서대로 정리한다. [Android 적용과 포커스 정리](packages/platforms/maui/Doroti.Host.Maui/MauiAndroidSemanticsBridge.cs#L92), [AndroidX 가상 노드 알림 계약](https://developer.android.com/reference/androidx/customview/widget/ExploreByTouchHelper#invalidateVirtualView(int,int))
- [ ] 오버레이 경로는 동일 핸들에 delegate를 재사용하고 새 핸들 부착 시 설치한다. Android 접근성 중요도, 자식 프록시의 노출 여부와 폐기 처리를 실제 상태 변화에 맞춰 갱신한다. Vulkan 가상 노드와 오버레이를 각각 실행해 읽기·탭·스크롤·포커스·갤럭시 입력란의 선택을 확인한다. [오버레이 갱신](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.cs#L27), [오버레이 delegate](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsLayout.Android.cs#L9)

### W3-05 · Web 및 공통 시나리오 확인

- [ ] Web main·Worker에서 기존 DOM 요소·리스너·포커스를 유지하면서 좌표 패치와 전체 재동기화가 동작하는지 확인한다. W3-03 이후 전체 비교·스크롤 오프셋 전송이 주요 비용으로 남으면 해당 필드만 후속 패치로 보낸다. 키보드·텍스트 입력·브라우저 접근성 트리와 연동되는 기존 동작을 검증한다. [Web 관리 호스트](packages/platforms/web/Doroti.Host.Web/BrowserSkiaCapabilities.cs#L312), [Web DOM/Worker 진입점](packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts#L1615)

## 3. 수락 기준과 검증

| 사례 | 통과 조건 |
| --- | --- |
| 동일 스냅샷과 큰 고정 트리에서 한 노드 이동 | 동일 결과의 전송·네이티브 쓰기는 0이다. 한 노드 이동은 필요한 좌표만 보내고 반영한다. 접근성 객체·ID, 액션·리스너, 노출 순서가 유지되며 구조·콘텐츠 변경 알림은 0이다. 전체 노드 자료구조 복제 및 제목 표시줄 peer 제거가 발생하지 않는다. |
| 부모 이동·스크롤·리사이즈 | 실제 반영 후 조회한 bounds와 hit test가 최종 위치와 맞는다. 상대 좌표가 같은 자식의 네이티브 배치를 재설정하지 않는다. 스크롤이 끝나면 마지막 분수 오프셋과 좌표가 남는다. 조상 변환, 클립·가시성 전환, DPI/DPR 변경, 기존 루트 없는 flat 입력도 포함한다. |
| 이동과 실제 변경이 같은 구간에 발생 | 탭 전환·팝업, 가상화로 인한 삽입/제거·재부모화, 레이블·값·포커스·선택·액션·읽기 순서 변화가 정확한 순서로 반영된다. 플랫폼별 스크롤 액션과 텍스트 범위 조회가 동작한다. 한글 IME, RTL, ZWJ, 텍스트 선택과 암호 위치 비노출을 확인한다. |
| 시작·소비자 연결·종료 | 첫 보조기술 조회, 화면 이동 중 뒤늦게 연결하는 소비자, 두 창 독립 갱신, `Clear`·뷰 교체·종료, 전송 기준 불일치에서 현재 트리가 보인다. 폐기한 노드·연결로 들어온 콜백은 현재 트리를 바꾸지 않는다. |

- [ ] 기존 `ScrollSemanticsRegression`, `ProjectionRegression`, `BrowserSemanticsRegression`, `WindowsPeerChildrenRegression`을 실행한다. 한 노드 이동에서 전체 자료구조를 재생성하는 회귀와 Qt 제목 표시줄의 거짓 `ObjectReorder`, 늦은 전달과 초기 스냅샷 복구를 기존 스위트가 놓치는 경우에 전용 회귀를 추가한다. 테스트는 20분 타임아웃을 적용하며 대부분의 반복 검증은 30회 이내로 끝낸다. [공통 테스트 안내](Doroti/tests/Doroti.Scroll.Tests/README.md), [저장소 지침](.github/copilot-instructions.md)

  ```powershell
  python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
  ```

- [ ] Demo/Testbed의 동일 화면과 제스처로 Windows App SDK·MAUI의 UI Automation과 Narrator, Qt AT-SPI(통합·기본 제목 표시줄), Android TalkBack(가상 노드·오버레이), iOS·AppKit·Catalyst VoiceOver, Web main·Worker를 확인한다. iOS의 Switch Control과 Windows·Qt의 기존 텍스트 범위·캐럿 probe도 포함한다. 자동화 조회 결과와 실제 보조기술 탐색·동작의 결과를 각각 기록한다. [샘플 실행](samples/DorotiDemoApp/README.ko.md), [텍스트와 네이티브 검증 기록](Doroti/docs/localization/paragraph-feasibility.md), [Web smoke 실행](packages/platforms/web/tests/run-browser-tests.ps1)
- [ ] 구현 후 같은 앱·화면·입력·DPI/DPR·소비자 조건에서 전후 자료를 수집한다. 프레임워크 semantics flush, 공통 좌표 투영, 직렬화·파싱, 네이티브 반영과 이벤트의 시간·할당, 전송 바이트·노드 수, 대기 깊이, peer 생성·이동·속성 쓰기를 측정한다. 보조기술 연결 유무를 맞춰 비교한다. 단계별 p95와 프레임 간격을 기록하고, 현 단계의 수락은 반복 작업 감소와 접근성 동작 보존으로 판정한다. [기존 프레임 추적](Doroti/src/Doroti.Ui/FrameLifecycle.cs#L52), [MAUI 카운터](packages/platforms/maui/Doroti.Host.Maui/MauiSemanticsBridge.cs#L46), [Web 카운터](packages/platforms/web/Doroti.Host.Web/BrowserSemanticsTransport.cs#L67)
