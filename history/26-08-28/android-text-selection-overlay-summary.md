# Android 텍스트 선택 확대경·컨텍스트 메뉴 수정 요약

- 기록일: 2026-08-28
- 대상: `DorotiDemoApp` / Android x86_64 에뮬레이터 / Material `TextField`
- 주요 범위: `TextSelectionHandleControls`, `RawMagnifier`, Material selection toolbar, Dart→C# compatibility lowering
- 최종 판정: **Android 에뮬레이터에서 길게 누르는 동안 Magnifier가 표시되고, 손을 뗀 뒤 Cut / Copy / Share 컨텍스트 메뉴가 표시되는 것을 실제 화면으로 확인해 PASS했다. 실제 Android 물리 기기는 `notVerified`다.**

## 1. 증상

`DorotiDemoApp`을 Android 에뮬레이터에서 실행해 `TextField`의 텍스트를 길게 누르면 selection highlight는 생겼지만 다음 overlay UI가 나타나지 않았다.

- 드래그·길게 누르기 중 Android Magnifier
- 선택을 마친 뒤 Material 컨텍스트 메뉴

Overlay 삽입 자체와 selection 변경은 동작하고 있었으므로 입력 gesture나 Android native text control의 문제가 아니라, 변환된 Doroti Framework의 selection overlay 타입 관계와 render dispatch를 검토했다.

## 2. 원인

### `TextSelectionHandleControls` mixin의 CLR 표현 손실

Flutter의 `TextSelectionHandleControls`는 concrete selection controls superclass 뒤에 적용되는 member-less mixin이며, `EditableText`는 이 타입 관계를 확인해 `contextMenuBuilder` 경로를 선택한다.

기존 C# 출력에서는 이를 `TextSelectionControls`를 상속하는 abstract class로 만들었다. CLR에서는 Material/Cupertino concrete superclass와 이 class를 동시에 상속할 수 없어 G53 compatibility 단계가 Material/Cupertino controls에서 해당 타입을 제거했다. 그 결과 runtime 타입 검사가 실패해 legacy toolbar 경로로 들어갔고, handle controls의 `buildToolbar()`가 `SizedBox.shrink()`를 반환하면서 메뉴가 비어 있었다.

### Magnifier render paint의 잘못된 dispatch

`_RenderMagnification__magnifier.paint()`가 `override`가 아니라 새 `virtual` 메서드로 출력돼 있었다. RenderObject pipeline은 base implementation으로 dispatch했고, backdrop filter와 확대 transform이 실제 paint에 참여하지 않았다.

### Material toolbar RenderObject override 누락

`_TextSelectionToolbarTrailingEdgeAlignRenderBox__text_selection_toolbar`의 다음 메서드도 새 `virtual` 메서드로 출력돼 있었다.

- `performLayout()`
- `paint()`
- `hitTestChildren()`
- `setupParentData()`
- `applyPaintTransform()`

따라서 render pipeline이 `RenderProxyBox` 구현을 호출해 `ToolbarItemsParentData`를 설치·사용하는 custom layout/paint가 실행되지 않았다. Overlay와 toolbar widget은 만들어졌지만 최종 메뉴 surface가 정상적으로 배치되지 않았다.

Toolbar 갱신 시 child widget 목록을 toolbar container 타입으로 잘못 cast하는 변환 결과도 함께 바로잡았다.

## 3. 최종 수정

### Selection controls marker 보존

- `TextSelectionHandleControls`를 CLR marker interface로 변경했다.
- Material/Cupertino의 mobile·desktop handle controls가 이 interface를 구현하도록 했다.
- `EditableText`의 runtime type check가 다시 성립해 modern `contextMenuBuilder` 경로를 사용한다.

### RenderObject override 복구

- `_RenderMagnification__magnifier.paint()`를 실제 override로 변경했다.
- Material trailing-edge toolbar render box의 layout, paint, hit test, parent data, paint transform 메서드를 모두 실제 override로 변경했다.
- toolbar child 비교에서 잘못된 `Cast<_TextSelectionToolbarOverflowable__text_selection_toolbar>()`를 제거하고 widget 목록을 직접 비교하도록 했다.

### 재생성 내구성

생성된 framework source만 수정하면 다음 Dart→C# 재생성에서 결함이 되살아나므로 `FrameworkCSharpLowerer.G53Compatibility`에도 같은 계약을 추가했다.

- Widgets selection mixin을 marker interface로 출력
- Material/Cupertino controls에서 marker 제거 규칙 폐기
- Magnifier paint override 복구
- Material toolbar RenderObject override와 child-list 비교 복구

FCR-7에는 Material controls의 marker 관계, Magnifier paint override, toolbar layout/paint/parent-data override를 reflection으로 확인하는 회귀 검증을 추가했다.

## 4. 검증 결과

### 자동 검증

| 검증 | 결과 |
| --- | --- |
| `Doroti.DartToCSharp` Release build | **PASS**, 경고 0개 / 오류 0개 |
| FCR-7 Material/Widget runtime contract | **PASS** |
| `DorotiDemoApp.Android` Release `android-x64` build | **PASS**, 경고 0개 / 오류 0개 |
| `git diff --check` | **PASS** |

### Android 에뮬레이터 runtime

빌드한 signed APK를 `emulator-5554`에 다시 설치한 뒤 Material `TextField`에서 텍스트를 입력하고 길게 눌러 확인했다.

- touch hold 중 선택 지점 위에 Magnifier가 표시됐다.
- touch release 뒤 selection highlight와 함께 `Cut`, `Copy`, `Share` 메뉴가 표시됐다.
- 최종 실행 로그에서 `FATAL EXCEPTION`, managed unhandled exception, ANR은 없었다.

초기 진단 과정에서 앱의 첫 frame 준비가 끝나기 전에 ADB key event를 보낸 한 번의 input timeout은 selection overlay 실행 이후의 정지가 아니었다. 최종 APK 재실행에서는 충분히 startup을 기다린 뒤 같은 selection 흐름을 검증했고 ANR이 재현되지 않았다.

실제 Android 물리 기기, 제조사별 Android skin, 접근성 서비스, hardware keyboard와 다양한 API level은 이번 검증 범위에 포함하지 않았으므로 `notVerified`로 유지한다.

## 5. 주요 변경 파일

- `Doroti/src/Doroti.Framework.Widgets/text_selection.cs`
- `Doroti/src/Doroti.Framework.Widgets/magnifier.cs`
- `Doroti/src/Doroti.Framework.Material/text_selection.cs`
- `Doroti/src/Doroti.Framework.Material/desktop_text_selection.cs`
- `Doroti/src/Doroti.Framework.Material/text_selection_toolbar.cs`
- `Doroti/src/Doroti.Framework.Cupertino/text_selection.cs`
- `Doroti/src/Doroti.Framework.Cupertino/desktop_text_selection.cs`
- `Doroti/validation/fcr7-material-widget/Program.cs`
- `tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.G53Compatibility.cs`

## 6. 보존할 구현 원칙

- Dart mixin을 CLR로 변환할 때 member뿐 아니라 runtime type identity가 관찰되는지 확인한다.
- RenderObject의 Flutter `@override` 메서드는 C#에서도 base virtual slot을 실제로 override해야 한다. 같은 이름의 새 `virtual` 메서드는 widget build 성공과 무관하게 layout/paint에서 누락될 수 있다.
- Overlay insert/build 로그만으로 visible UI를 PASS로 판정하지 않는다. 실제 build, render dispatch와 최종 화면을 함께 확인한다.
- 생성된 framework source의 수정은 lowerer compatibility와 회귀 검증에 같이 반영해 재생성 후에도 유지한다.
- 에뮬레이터 visible PASS와 Android 물리 기기·접근성 검증을 구분한다.

> 문서 성격: 2026-08-28 Android Material TextField selection overlay의 Magnifier와 컨텍스트 메뉴 결함, 원인, 생성기 보존 수정 및 에뮬레이터 검증을 기록하는 역사 문서다. 새로운 active roadmap이 아니다.
