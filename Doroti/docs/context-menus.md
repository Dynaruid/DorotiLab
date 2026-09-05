# 플랫폼별 컨텍스트 메뉴

`Material.TextField`, `Material.SelectableText`, `Material.SelectionArea`는 기본 context-menu builder를 통해 Flutter의 플랫폼별 텍스트 메뉴를 사용한다. 모양은 `ThemeData.platform`을 따르며, 메뉴 항목은 실제 선택 범위, 편집 가능 여부, 클립보드 및 호스트 기능에 따라 결정한다.

| 플랫폼 | 기본 표시 |
| --- | --- |
| Android | Material 텍스트 선택 툴바 |
| iOS | 지원되는 편집 필드에서는 시스템 메뉴, 그 외에는 Cupertino 텍스트 선택 툴바 |
| macOS | Cupertino 데스크톱 메뉴 |
| Windows / Linux | Material 데스크톱 메뉴 |
| Web | 브라우저 기본 메뉴. 활성 입력 필드의 네이티브 편집·선택 상태를 사용 |

Cupertino 위젯은 `CupertinoAdaptiveTextSelectionToolbar`를 통해 모바일에서는 Cupertino 툴바, 데스크톱에서는 Cupertino 데스크톱 메뉴를 사용한다. Fuchsia는 Material adaptive toolbar에서 데스크톱 컨테이너와 Material 텍스트 버튼을 사용하며 Flutter의 분기를 따른다.

선택된 일반 텍스트에는 잘라내기·복사·붙여넣기를 상태에 맞게 제공한다. 읽기 전용 필드는 잘라내기·붙여넣기를 제외하고, 비밀번호는 복사·잘라내기·공유를 제외한다. macOS는 전체 선택 항목을 제외하고, iOS는 커서만 있을 때 전체 선택을 제공한다. Android의 공유는 전체 선택 앞에, iOS의 공유는 조회·웹 검색 뒤에 온다. OS 작업의 실제 실행은 각 호스트가 제공하는 기능에 의존한다.

공용 `ContextMenuController`는 호출 위치의 테마를 루트 오버레이로 전달한다. 따라서 지역적으로 설정한 플랫폼 모양과 밝기·색상도 메뉴에 적용된다. 이미 열린 메뉴를 갱신할 때도 호출 위치의 테마를 다시 캡처한다. `ContextMenuButtonItem.onPressed == null`인 사용자 항목은 비활성 상태를 유지한다.

Windows/Linux/macOS의 기본 메뉴는 빈 편집 가능 필드에서도 붙여넣기 항목을 제공한다. 클립보드가 비었거나 조회 중이면 비활성 상태로 표시하고, 텍스트가 있으면 활성화한다. 클립보드 읽기·쓰기와 상태 조회는 각 호스트의 `IPlatformServicesHostCapability`를 사용한다.

사용자 메뉴도 플랫폼별 모양을 재사용할 수 있다.

```csharp
using Doroti.Framework.Material;
using Doroti.Framework.Widgets;
using Doroti.Ui;

// context: 호출 위젯의 BuildContext
// position: 루트 오버레이 좌표계의 메뉴 위치
var menu = new ContextMenuController();
menu.show(context, menuContext => AdaptiveTextSelectionToolbar.CreateButtonItems(
    anchors: new TextSelectionToolbarAnchors(position),
    buttonItems: new List<ContextMenuButtonItem>
    {
        new(onPressed: () => { menu.remove(); OpenDetails(); }, label: "상세 보기"),
        new(onPressed: null, label: "사용할 수 없는 작업"),
    }));
```

텍스트 위젯의 기본 메뉴에는 별도 설정이 필요 없다. 웹에서 프레임워크 메뉴를 명시적으로 선택하는 앱은 서비스 바인딩 초기화 후 `BrowserContextMenu.disableContextMenu()`를 호출할 수 있고, `enableContextMenu()`로 브라우저 메뉴를 복원할 수 있다. Doroti의 웹 기본값은 브라우저 메뉴 활성화다.

기준: [Flutter AdaptiveTextSelectionToolbar](https://api.flutter.dev/flutter/material/AdaptiveTextSelectionToolbar-class.html), [플랫폼별 버튼](https://api.flutter.dev/flutter/material/AdaptiveTextSelectionToolbar/getAdaptiveButtons.html), 저장소의 고정 Flutter reference.

자동 검증은 `validation/fcr7-material-widget/ContextMenuContracts.cs`에서 플랫폼별 위젯 선택·메뉴 항목·콜백·오버레이 테마 전달을 검사한다. 실제 OS 메뉴 픽셀, 터치·마우스 동작, 시스템 조회·공유 실행 및 접근성 검증은 별도다.
