# Framework virtual/override 전수 감사 및 수정

## 범위와 원인

2026-09-07, 현재 제품 소유 C# `Doroti/src/Doroti.Framework.*` 13개 프로젝트, 675개 소스를 검사했다. 저장소의 Flutter `56b8e1a8` 구현과 비교했다. 앞선 TextField 단어 경계 수정 이후에도 기존 검사가 제한된 lifecycle 이름만 검사하여 재정의 누락이 남아 있었다.

C#에서 기반 가상 멤버와 같은 이름으로 새 `virtual` 멤버를 선언하면 별도 슬롯이 생긴다. 파생 타입으로 직접 호출하는 테스트는 성공해도 기반 타입 또는 상속된 인터페이스로 호출하면 기반 구현이 실행된다. 반환형, nullable 값 형식, 매개변수 개수/형식이 다르면 `override` 키워드만 바꾸어서는 해결되지 않는다.

## 수정 결과

- 같은 시그니처의 숨겨진 가상 멤버 215개 중 198개의 재정의를 복구했다. Material 테마 기본값, range slider 색상/도형, 선택 컨테이너, 스크롤 위치, Cupertino route/gesture, binding 입력/메모리/frame 처리, 렌더링 tween 및 semantics를 포함한다.
- 서로 다른 Dart 라이브러리의 비공개 멤버 17개는 독립된 상태/헬퍼다. `intentional-hiding.json`에 정확한 멤버 쌍과 Dart 원본 파일을 기록했다. C#에서 접근 가능한 숨김에는 `new`를 명시했고, 다른 assembly의 접근 불가능한 internal 멤버에는 불필요한 `new`를 붙이지 않았다.
- 시그니처 불일치 19곳도 수정했다. Cupertino generic 색상 해석, handle-only controls의 cut/copy 8곳, ListTile/Chip/Cupertino stack의 render object 갱신 3곳, carousel metric 복사 2곳, RenderObject diagnostic dump, SemanticsNode diagnostic dump/children, 2차원 viewport 레이아웃 무효화, InputBorder paint다.
- 진단 출력 90곳에 실제 `Object.ToString()` 재정의를 연결했다. Flutter의 optional diagnostic 인자 버전은 유지하면서 object/문자열 보간 경로도 같은 출력으로 연결한다.
- `RenderSliverVariedExtentList.itemExtentBuilder`는 getter/setter가 하나의 기반 가상 프로퍼티를 사용하도록 했다. 기반 layout에서 null callback을 보는 문제가 해결된다.
- Material 기반 Cupertino theme의 nullable Brightness, TimePicker의 nullable elevation, diagnostics delegate의 nullable 시그니처를 기반 계약에 맞췄다. rect callback은 별도 delegate 타입 대신 기반 계약의 `Func<Rect>`를 사용한다.
- Carousel의 추가 item 설정 복사는 `copyWithCarousel`로 보존하고 기반 `copyWith` 슬롯에서 연결했다. Semantics의 child-order overload, viewport의 bool overload, handle-only control의 optional clipboard-status overload도 실제 기반 슬롯을 연결한다.

Flutter 비교의 대표 근거는 `widgets/widget_state.dart`, `widgets/text_selection.dart:TextSelectionHandleControls`, `material/list_tile.dart:updateRenderObject`, `material/chip.dart`, `cupertino/text_field.dart`, `material/carousel.dart`, `rendering/tweens.dart`, `material/range_slider.dart`, `material/input_border.dart`, `semantics/semantics.dart`다. Dart에서 허용하는 covariant parameter와 추가 optional parameter가 C#에서는 별도 overload가 된 경우를 포함한다. 컴파일러 import/reference 도구로 제품 소스를 재생성하지 않았다.

## 재발 방지

`Doroti/eng/validate-framework-virtual-dispatch.ps1`은 최신 Release 의존성을 빌드한 뒤 Roslyn 의미 분석과 기반/인터페이스 타입의 동작 검사를 수행한다. 같은 이름을 찾는 정규식만으로 판단하지 않는다. 숨겨진 메서드/프로퍼티/인덱서/event와 virtual/abstract 시그니처 불일치를 검사하고, 새로운 예외나 의미 분석 오류가 있으면 실패한다. 예외가 사라졌는데 목록에 남아 있어도 실패한다.

최종 결과: 미해결 숨김 0, 시그니처 불일치 0, 의미 분석 오류 프로젝트 0, 의도된 비공개 멤버 17. 상세 JSON은 `.doroti/framework-virtual-dispatch.json`.

AND/OR의 실제 `override`를 일시적으로 `virtual`로 되돌린 mutation 검사에서 두 누락을 정확히 검출하고 exit 1을 반환했다. 원본 파일은 `finally`에서 바이트 그대로 복원했다. `.doroti/virtual-mutation.json`, `.log`, `.err`에 증거가 있다.

## 검증 및 실패 기록

모든 테스트 프로세스 timeout은 1,200초다.

- 신규 기반 타입 동작 검사 PASS: AND/OR 및 중첩 상태 조건식, geometric Tween.transform, range slider 색상/도형, page/wheel/carousel snapshot의 subtype/값 보존, Color generic 해석, handle-only cut/copy 무동작, varied sliver callback 읽기/변경, object diagnostic 출력, ShapeBorder 타입으로 입력 테두리 그리기.
- FCR-7 Release/Debug 전체 PASS. `.doroti/virtual-final-fcr7.log`, `.doroti/virtual-fcr7-debug.log`.
- Dynamic dispatch, FCR-3 scheduler, FCR-4 retained rendering, FCR-5 scroll, FCR-6 semantics Release PASS. `.doroti/virtual-dynamic.log`, `virtual-fcr3.log` ~ `virtual-fcr6.log`.
- Windows sample Release 전체 회귀 검사 및 mounted TextField CPU/GPU 검사 PASS. 결과는 `.doroti/virtual-final-windows.log`, `virtual-final-text-cpu.log`, `virtual-final-text-gpu.log`에 기록한다.
- Windows TestbedApp Release build PASS, warning/error 0. `.doroti/virtual-app-build.log`.
- 최초 일괄 수정 과정에서 nullable delegate, 프로퍼티 setter, nullable enum 시그니처의 compile failure를 발견해 해결했다. 너무 넓은 텍스트 교체로 ThemeData.brightness와 Semantics bridge가 중복 수정된 중간 실패도 복원/해결했다. `.doroti/virtual-build1.log` ~ `virtual-build7.log`, `virtual-behavior2.log`.
- 첫 nullable FractionalOffset 테스트는 null의 원점을 0으로 잘못 기대해 실패했다. pinned Flutter는 중심 0.5에서 보간하므로 중간값 기대를 0.75로 바로잡았다. `.doroti/virtual-behavior.err`, `virtual-behavior2.log`.
- 최초 DLL 탐색에서 native DLL을 managed metadata로 읽은 검사기 오류는 PE metadata 필터로 해결했다. 플랫폼 전체에 대한 탐색용 semantic projection은 MAUI 조건부 정의/소스 생성기 및 플랫폼 reference 부재로 오류가 있었다. 최종 통과 주장은 의미 분석 오류가 없는 Framework 13개 프로젝트에 한정한다.
- 앞선 작업의 Windows sample Debug fixture Localizations assertion 실패는 이전 기록에 유지한다. 이번 FCR-7 Debug 통과를 그 별도 Windows sample Debug fixture의 통과로 간주하지 않는다.

## 검증 경계

현재 소스의 자동 계약/빌드 검증이다. 실제 Windows 물리 키보드, 한글 IME 후보창/조합, 사용자가 직접 조작한 TestbedApp 화면 및 다른 플랫폼의 물리 UI acceptance는 `notVerified`다. 이전 TextField 수정과 Windows native 입력 수정은 유지했다. 전체 Flutter import 동등성이나 모든 virtual 호출의 런타임 안전성을 보증하지 않는다.
