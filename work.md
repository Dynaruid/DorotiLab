# Doroti VS Code 코드 작성 지원 확장 작업계획

작성일: 2026-10-06. 구현·검증: 2026-10-07. 대상: `tools/vscode-doroti`.

상태: **E0–E6 구현 및 Windows 편집 검증 완료**. `wholePlanComplete=true` (이 문서의 편집 지원 범위).

2026-10-06에는 소스·공식 문서 검토로 계획을 작성했고, 2026-10-07의 전체 작업 요청에 따라 구현과 설치형 편집 검증을 수행했다. 아래 원래 조사 내용은 구현 전 상태의 근거다. 현재 결과·지원 제한·명령·hash는 [편집 검증 기록](tools/vscode-doroti/docs/editing-verification-2026-10-07.md)과 [README](tools/vscode-doroti/README.md)를 따른다. Windows 편집 PASS를 기기·물리 입력·IME·접근성·scanout·배포 전체 PASS로 확대하지 않는다.

## 1. 목표와 범위

Doroti C# 코드를 작성할 때 `stl`, `stf`를 입력하면 위젯 클래스 구성이 제안되고, 위젯 위에서 `Ctrl+.` 또는 우클릭 Refactor를 열면 감싸기·추출 등 구조 편집이 나타나도록 한다. 타입 이름만 나열하는 현재 제안을 생성자·인자·builder·State 생명주기 문맥에 맞는 작성 지원으로 확장한다.

필수 구현 범위는 E0–E6이다. 우선 사용 가능한 단축 생성 기능을 E1에서 제공하고, 구문·타입 분석 기반 감싸기와 문맥 제안을 E2–E4에서 추가한다. E5는 구조 리팩터링, E6는 설치한 VSIX에서의 검증과 문서화다.

기존 Create Project, 프로젝트/target 선택, Run/Stop, Hot Reload, 로그 기능은 유지한다. Preview/Webview/Inspector, 전체 C# 언어 서버 교체, 공개 Marketplace 배포, 동시 다중 실행은 이번 완료 조건에 포함하지 않는다. Microsoft C# 확장이 제공하는 기본 IntelliSense·진단·정의 이동·일반 리팩터링과 Doroti 전용 지원을 함께 사용한다.

## 2. 현재 코드에서 확인한 내용

| 근거 | 현재 구현과 확인한 한계 |
| --- | --- |
| [확장 manifest](tools/vscode-doroti/package.json) | VS Code `^1.100.0`, C# 활성화, 실행 명령 8개, C# snippet 등록. 감싸기/추출 명령, 편집 설정, 편집 전용 context key는 없다. |
| [widgets.json](tools/vscode-doroti/snippets/widgets.json) | `dstateless`, `dstateful`, `dbuild`, `dinitstate`, `ddispose`, `dsetstate`, `drow`, `dcolumn`, `dcontainer`, `dtext`, `dmaterial`, `dcupertino`의 12개 기본 snippet. `stl`/`stf`와 Flutter 공식 별칭은 없다. |
| [extension.ts](tools/vscode-doroti/src/extension.ts), 223–267행 | completion은 고정 namespace 목록을 타입 이름으로 제안하고 `additionalTextEdits`로 import를 추가한다. 코드 액션은 `CS0246`/`CS0103` import Quick Fix만 등록되어 있다. `RefactorRewrite`/`RefactorExtract`, hover, signature help, 문맥별 snippet provider는 없다. |
| [imports.ts](tools/vscode-doroti/src/imports.ts) | alias·동일 파일 타입·일부 global using·`TextStyle` 중복 namespace를 보수적으로 처리한다. 전체 프로젝트의 심볼을 해석하는 구현은 아니다. `Padding`, `Expanded`, `Align`, `MainAxisAlignment`, `BoxDecoration` 등도 고정 목록에 없다. |
| [extension.ts](tools/vscode-doroti/src/extension.ts)의 `eligible`/`globals` | 선택 전에는 workspace 어디든 manifest가 하나 있으면 file C# 문서가 eligible이다. 선택 후에는 root 포함 여부만 검사한다. 매 요청마다 `Program.cs`와 `src/**/*.cs` 최대 200개를 디스크에서 읽으며, 미저장 버퍼·프로젝트별 Compile/Using 항목·다른 위치의 global using을 완전히 반영하지 못한다. cancellation·캐시도 없다. |
| [README](tools/vscode-doroti/README.md) | 기본 C# 기능은 Microsoft C# 확장에 의존한다고 명시한다. snippet 삽입 자체는 import를 넣지 않으며, 타입 completion 또는 compiler Quick Fix를 별도로 선택해야 한다. |
| [단위 검증](tools/vscode-doroti/src/test/contracts.test.ts), [설치형 검증](tools/vscode-doroti/src/test/host.ts) | import 충돌, 기존 StatefulWidget 이름 연동/Tab, provider 호출/Quick Fix/Undo 검증 코드가 있다. 짧은 prefix의 실제 제안 선택, wrapping/추출/인자 제안은 검사하지 않는다. `host.ts`는 편집 검사 뒤 Windows Run/Hot Reload까지 이어진다. |
| [실행 harness](tools/vscode-doroti/src/test/runHost.ts), [패키징 제외 목록](tools/vscode-doroti/.vscodeignore) | 설치한 VSIX를 별도 profile로 실행하는 기반이 있다. 편집만 검사하는 진입점은 없으며, 새 helper와 catalog를 추가할 때 패키징 포함 여부를 검증해야 한다. |

현재 부족함은 기본 C# 언어 기능 전체의 부재가 아니라 **짧은 코드 생성, Doroti 위젯 구조 편집, 문맥 제안, 넓은 API 범위, 프로젝트 판별과 응답 성능**이다. 일반 생성자 인자나 enum 완성은 C# 확장에서도 제공할 수 있으므로, 실제 공존 환경에서 확인한 뒤 Doroti 고유 템플릿·인자 묶음·builder 본문을 보강한다.

### 실제 Doroti API에 맞춰야 하는 부분

- [framework.cs](Doroti/src/Doroti.Framework.Widgets/framework.cs): `Widget build(BuildContext context)`, `IState createState()`, `State<T>`, 소문자 `initState`/`didUpdateWidget`/`didChangeDependencies`/`dispose`/`setState`를 사용한다. Dart의 선언 형태를 그대로 복사하지 않는다.
- [basic.cs](Doroti/src/Doroti.Framework.Widgets/basic.cs): `Padding`은 `padding:`이 필요하고, `Row`/`Column`은 `List<Widget> children`, `Expanded`/`Flexible`은 Flex parent data를 사용한다. 아무 위치에서나 Expanded를 권장하면 런타임 구조가 잘못될 수 있다.
- [container.cs](Doroti/src/Doroti.Framework.Widgets/container.cs), [single_child_scroll_view.cs](Doroti/src/Doroti.Framework.Widgets/single_child_scroll_view.cs): 단일 자식은 `child:`다. `Container`의 `color`/`decoration` 같은 제약도 템플릿에서 함께 고려한다.
- [layout_builder.cs](Doroti/src/Doroti.Framework.Widgets/layout_builder.cs): `builder`는 `Func<BuildContext, BoxConstraints, Widget>`이다. callback placeholder의 개수와 반환 타입을 실제 delegate에 맞춘다.
- [edge_insets.cs](Doroti/src/Doroti.Framework.Painting/edge_insets.cs), [border_radius.cs](Doroti/src/Doroti.Framework.Painting/border_radius.cs): `EdgeInsets.CreateAll/CreateSymmetric/CreateOnly`, `BorderRadius.CreateCircular`처럼 실제 C# factory 이름을 사용한다.
- [FileUploadPage.cs](samples/DorotiSampleApp2/src/FileUploadPage.cs): block/file namespace, `TextStyle`/`WidgetImage` alias, `State<T>`, `mounted`, 취소·dispose를 포함한 현실적인 회귀 자료로 사용한다.
- [Carousel 샘플](samples/DorotiCarouselApp/src/App.cs), [CustomCarousel](packages/Doroti.CustomCarousel/src/CustomCarousel.cs): 중첩 생성자, primary constructor, collection expression와 spread, 위젯 반환 함수 호출을 다뤄야 한다. `CustomCarousel`은 별도 package의 위젯이고 `effectsBuilder`/`children` 인자가 필요하다. 설치·참조하지 않은 앱에 무조건 제안하지 않는다.

## 3. Flutter 기능과 비교한 부족 기능·우선순위

Flutter 공식 문서는 `stless`/`stful`/`stanim` snippet과 위젯·위젯 목록 감싸기, child/children 변환, `Ctrl+.` assists를 설명한다. 사용자가 요청한 `stl`/`stf`는 Doroti에서 추가할 짧은 별칭이며 Flutter 공식 prefix라고 표기하지 않는다. [Flutter VS Code 편집 안내](https://docs.flutter.dev/tools/vs-code#editing-tips-for-flutter-code)

| 우선순위 | 기능 | Doroti 적용과 완료 조건 |
| --- | --- | --- |
| P0 | `stl`/`stf` 클래스 생성 | `stl`, `stless`, `dstateless`; `stf`, `stful`, `dstateful`을 각각 동일 템플릿의 별칭으로 제공. 위젯/State/createState 이름 연동, Tab 순서, 필요한 타입/import를 함께 처리. |
| P0 | 새 위젯 파일 생성 | Stateless/Stateful 선택, 파일명·클래스명·namespace·key constructor를 구성하는 명령. 기존 파일을 덮어쓰지 않고 취소 시 생성하지 않음. |
| P0 | 단일 위젯 감싸기 | Center, Padding, Container, SizedBox, Align, Row, Column, Stack, Builder, LayoutBuilder, SingleChildScrollView. 커서만 둬도 대상 위젯의 전체 식을 판단. |
| P0 | 위젯 목록 감싸기 | `children: [...]`의 연속된 여러 항목을 Row/Column/Stack으로 감쌈. 쉼표·주석·앞뒤 항목 보존. |
| P0 | import와 제안 범위 | 생성 코드의 import를 한 번만 추가하고 alias·동명 타입은 정확히 한정. 관련 없는 C# 프로젝트에서 Doroti 제안/메뉴가 나타나지 않음. |
| P1 | 일반 위젯 snippet 확대 | Padding/Center/Expanded/Flexible/SizedBox/Stack/Align/ListView/scroll/ClipRRect/입력/버튼/decoration 등을 실제 API로 구성. |
| P1 | 인자·builder 작성 제안 | 현재 생성자의 미입력 인자와 `child:`/`children:`/`builder:` 템플릿, 적절한 callback 본문을 제공. C# 제안과 중복을 억제. |
| P1 | State 생명주기 지원 | State 내부에서 `initState`, `didUpdateWidget`, `didChangeDependencies`, `dispose`, `setState` 제안. 이미 있는 override를 중복 생성하지 않음. |
| P1 | 감싸기 제거 | 자식 하나를 가진 지원 wrapper를 제거. key·callback·기타 인자 의미가 사라지는 변경은 diff preview를 제공. |
| P1 | Extract Widget | 선택한 Widget 식을 새 StatelessWidget으로 추출. 외부 참조를 분석하고 필요한 값을 constructor 인자로 전달. |
| P1 | Stateless → Stateful | build와 constructor/key/프로퍼티 계약을 보존하여 State를 분리. 기존 필드 참조를 `widget.` 등으로 정확히 변경. |
| P1 | 타입·도움말 범위 확대 | 실제 참조된 Framework/Material/Cupertino/추가 package를 기준으로 widget catalog와 설명을 구성. 특정 타입명 하드코딩만 계속 늘리지 않음. |
| P1 | 제안 응답 개선 | 매 키 입력마다 파일 재검색/프로젝트 재로드를 하지 않음. 미저장 snapshot, 캐시 무효화, 요청 취소를 처리. |
| P2 | child/children 및 동등 wrapper 변환 | 대상 생성자의 실제 signature가 허용하는 경우만 제공. `Container`에 `children:`를 넣는 식의 단순 문자열 변환 금지. |
| 후속 | 색상 미리보기·닫힘 라벨·위젯 트리 outline | DocumentColorProvider·decorations 등으로 검토 가능. 필수 편집 지원 완료 후 별도 범위로 진행. |
| 후속 | 실행·진단 편의 | Doctor 안내, 장치 선택 UI, 디버거/F5 통합 등을 별도 검토. 기존 실행 구현 전체를 미구현이라고 다시 분류하지 않음. |

## 4. 구현 구조와 공통 계약

### 4.1 VS Code와 C# 분석의 역할

VS Code의 `CompletionItemProvider`, `SnippetString`, `CodeActionProvider`, `WorkspaceEdit`를 사용한다. 오류 진단 없이도 감싸기가 나타나도록 import Quick Fix와 `RefactorRewrite`/`RefactorExtract`를 구분한다. [VS Code 언어 기능 API](https://code.visualstudio.com/api/language-extensions/programmatic-language-features)

클래스·위젯 템플릿은 JSON catalog를 원본으로 관리하고 import-aware provider와 삽입 명령이 공유한다. `stl`/`stf`를 JSON contribution과 동적 provider 양쪽에서 중복 노출하지 않는다. import 동시 편집이 필요한 항목은 동적 provider로 옮기고 기존 `d*` prefix를 유지한다. 기존 VS Code Insert Snippet 진입점에 미치는 변경은 README와 검증에 명시하고, **Doroti: Insert Widget Snippet** 명령으로 전체 템플릿 선택 경로를 제공한다.

기본 타입 완성·signature·hover·정의 이동은 C# 확장을 우선 사용한다. Doroti provider는 widget 구성용 snippet과 프로젝트별 catalog 정보를 보강한다. C# 확장의 내부 LSP 명령이나 비공개 assembly에 의존하지 않는다.

### 4.2 구문·타입 분석 경로

정규식/괄호 개수만으로 C# 위젯 트리를 변환하지 않는다. Roslyn은 주석·공백·전처리 지시문을 포함한 구문 정보를 보존하고, semantic model로 이름의 실제 타입·참조를 해석할 수 있다. [Roslyn 구문 모델](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/work-with-syntax), [의미 모델](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/work-with-semantics)

기본안은 확장이 소유하는 작은 `.NET` 편집 helper다. 전체 C# 언어 서버를 새로 만들지 않고 필요한 snapshot 분석·catalog·변환 계산만 제공한다. 기존 [DartToCSharp 도구](tools/Doroti.DartToCSharp/Doroti.DartToCSharp.csproj)의 Roslyn 사용은 참고하되, translator에 IDE 세션을 결합하지 않는다. 기존 `4.14.0` 참조를 그대로 선택하지 말고 저장소의 .NET 10/C# 문법과 맞는 Roslyn 버전을 E2에서 확인·고정한다.

- `.NET` 내부는 typed record, `Task`/`CancellationToken`, 필요 시 bounded `Channel<T>`로 처리한다. TypeScript↔helper의 실제 process 경계에서만 versioned JSON 메시지를 사용한다.
- 요청에는 project identity, URI, document version, UTF-16 offset/selection, 미저장 문서 snapshot, request ID를 포함한다. 결과에는 대상 범위, 편집 목록, 필요한 import, 제한 이유를 반환한다.
- 프로젝트는 manifest의 `applicationProject`와 실제 Compile/ProjectReference/PackageReference/Using 정보를 기준으로 해석한다. 임의 첫 `.sln`이나 실행 runner를 작성 문서의 프로젝트로 가정하지 않는다.
- 프로젝트별 참조/구문/semantic 캐시를 유지한다. 요청마다 helper 실행, restore/build, 전체 파일 탐색을 하지 않는다. design-time 프로젝트 로드도 trust가 있는 경우에만 수행한다.
- 오래된 snapshot 결과는 적용하지 않고 취소·재계산한다. helper가 실패하거나 참조 해석이 불완전하면 안전한 템플릿 경로를 남기고, 타입 판별이 필요한 리팩터링은 제한 이유를 제공한다.
- Restricted Mode에서도 순수 템플릿·문서 편집은 허용한다. helper 실행/프로젝트 평가/외부 도구 실행은 하지 않는다. import 확신이 없으면 생성 타입을 `global::`로 한정한다.

### 4.3 모든 편집에 적용할 규칙

- 문서 소유 프로젝트와 실제 `Doroti.Framework.Widgets.Widget` 타입 관계를 판별한다. 동명 `Widget`/`Text` 또는 alias를 이름만 보고 Doroti로 취급하지 않는다.
- 선택은 전체 표현식 또는 collection의 연속된 항목으로 제한한다. `return`, 세미콜론, 부모의 쉼표를 replacement에 잘못 포함하지 않는다.
- nested object creation, target-typed `new`, Widget 반환 함수, lambda/conditional 식, generic, C# collection expression/spread, 일반/verbatim/raw/interpolated string, 전처리 지시문을 구문 모델로 구분한다. 안전하지 않은 미완성 구문은 액션을 숨기거나 제한 이유를 표시한다.
- BOM, LF/CRLF, 들여쓰기, namespace 형태, 주석, alias/global using을 보존한다. 결과 범위만 기존 C# formatter로 정리하며 전체 파일을 재포맷하지 않는다.
- 일반 wrapping과 import는 한 Undo 단위로 적용한다. 큰 구조 변환과 파일 생성은 diff preview·충돌 확인·Undo/Redo를 검증한다. 미저장 문서를 자동 저장하거나 변환 후 Run/Hot Reload를 자동 실행하지 않는다.
- static snippet 설정은 사용자 `editor.snippetSuggestions`/`tabCompletion`을 존중한다. prefix 입력 시 suggestion 선택과 명시적 Insert Widget Snippet 명령을 지원하며 사용자 설정을 덮어쓰지 않는다. snippet의 연결 placeholder/선택 텍스트 활용은 [VS Code snippet 안내](https://code.visualstudio.com/docs/editing/userdefinedsnippets)를 따른다.

## 5. 단계별 작업

### E0 — 편집 기능 분리와 프로젝트 판별 (P0)

- [x] `extension.ts`에서 편집 provider를 `src/editing/`으로 분리한다. 실행 session의 busy 상태와 편집 요청 lifecycle을 분리하여 앱 실행 중에도 작성 지원이 동작하도록 한다.
- [x] `projectContext.ts`를 추가해 문서 소유 앱/참조 package를 판별한다. 프로젝트 선택 전에도 manifest와 문서 위치를 기준으로 관련 범위를 찾고, 파일 또는 연결 프로젝트를 확인한다.
- [x] `doroti.isWidgetDocument` 등 편집 context key와 completion/codeActions/snippets 설정을 추가한다. 메뉴 노출과 provider 내부 검증을 함께 적용한다.
- [x] 기존 global using 탐색을 캐시하고 문서 change/close, manifest·csproj·props/targets·참조 변경으로 무효화한다. 미저장 버퍼를 디스크보다 우선한다.

완료 기준: 관련 없는 C# 파일에는 Doroti 전용 제안이 없고, 프로젝트 선택·Run 없이 앱 소스에서 편집 지원을 사용할 수 있다. 실행/중지/Hot Reload의 기존 계약은 유지된다.

### E1 — `stl`/`stf`, 위젯 생성과 기본 snippet 확장 (P0)

- [x] Stateless alias `stl`/`stless`/`dstateless`, Stateful alias `stf`/`stful`/`dstateful`을 추가한다. 동일 prefix 항목이 제안 목록에 중복되지 않도록 등록 방식을 정리한다.
- [x] 파일명에서 유효한 기본 클래스명을 제안하고 위젯 클래스/State/createState 이름을 연결한다. C# 식별자·예약어·한글 이름을 검증한다.
- [x] 최소 템플릿과 key constructor 포함 템플릿을 제공한다. Stateful은 실제 `IState`/`State<T>` API를 사용하고 기본 build 본문으로 편집 가능한 Widget 식을 넣는다.
- [x] 생성에 필요한 using을 completion 편집과 함께 추가한다. 동명 타입/alias/참조 미확정 시 정확한 타입명으로 한정하고 using을 중복 추가하지 않는다.
- [x] **Doroti: New Stateless Widget**, **New Stateful Widget**, **Insert Widget Snippet**을 추가한다. 파일 생성 명령은 namespace·위치·파일 충돌·취소를 처리하고 생성 파일을 연다.
- [x] Padding/Center/SizedBox/Expanded/Flexible/Stack/Align/scroll/ListView/Builder/LayoutBuilder/ClipRRect/TextStyle/decoration 및 Material/Cupertino 기본 화면·입력·버튼 템플릿을 추가한다. 사용하지 않는 package 템플릿은 문맥 제안에서 제외한다.
- [x] State override/컨트롤러 init·dispose 패턴을 추가한다. 소문자 메서드, 실제 `Dispose`/`dispose` 차이, base 호출 순서와 자원 소유를 해당 API 기준으로 확인한다.

완료 기준: 빈 앱 소스에서 `stl`/`stf` 입력 → 제안 선택 → 클래스명 편집 → Tab → build 본문 작성이 연결되고 필요한 타입이 해석된다. 기존 `d*` 사용 흐름과 Insert Widget Snippet 명령도 검증한다.

### E2 — 분석 helper와 공통 catalog 기반 (P0/P1)

- [x] `tools/Doroti.Editor.Assist/`에 snapshot 분석 helper와 typed 계약을 추가한다. 프로젝트/패키지 평가, source snapshot, type identity, constructor/delegate 정보를 분리한다.
- [x] C# 구문 버전, SDK 선택, restore 전/후 동작, 파일/프로젝트 연결, 미저장 파일 추가를 검증하고 지원 범위를 명시한다.
- [x] process ownership, handshake/schema version, cancellation, 최대 메시지/캐시 크기, 재시작·dispose를 구현한다. helper의 stderr는 로그로 보내고 stdout protocol과 섞지 않는다.
- [x] Core widget API와 참조된 Material/Cupertino/사용자 위젯·추가 package의 catalog를 구성한다. type/namespace, 인자 이름·타입·기본값, child 형태, builder signature, 문서, 구조 제약을 공유한다.
- [x] app 프로젝트 기준 semantic model에 미저장 snapshot을 반영한다. 참조 해석이 안 되는 환경에서는 구문 분석과 안전한 템플릿만 제공하고 semantic 액션을 성공으로 표시하지 않는다.

완료 기준: 샘플의 raw string/collection/primary constructor와 사용자 Widget 타입을 정확히 분석한다. 동일 문서에 대한 연속 요청에서 helper·프로젝트가 매번 다시 실행/로드되지 않는다.

### E3 — 위젯 감싸기와 감싸기 제거 (P0/P1)

- [x] `CodeActionKind.RefactorRewrite`로 단일 위젯 감싸기를 등록한다. 위젯 이름/식 위 커서와 전체 선택 모두 지원한다.
- [x] Center/Container/SizedBox/Align/SingleChildScrollView는 `child:`, Padding은 `padding: EdgeInsets.CreateAll(...)`과 `child:`를 생성한다. Row/Column/Stack은 `children: [원본식]`을 생성한다.
- [x] Builder/LayoutBuilder 감싸기는 올바른 callback 반환식을 생성하고 주변 `context`/lambda 이름 충돌을 피한다. **Wrap with Widget…** Quick Pick은 현재 사용 가능한 wrapper만 제시한다.
- [x] 동일 children collection의 연속된 여러 위젯을 감싼다. 앞뒤 항목, comma와 trivia, spread를 보존하고 의미를 확인할 수 없는 선택을 거부한다.
- [x] Expanded/Flexible/Positioned 등 parent data 의존 wrapper는 적합한 부모 구조를 확인할 때만 제안한다. 컴파일만 되지만 잘못된 배치가 되는 항목을 일반 wrapper 목록에 넣지 않는다.
- [x] 지원 wrapper 제거를 제공한다. 의미를 가진 key·padding·callback 등 제거 영향을 preview에 보여주고, 자식이 여러 개거나 타입/의미를 보존할 수 없는 경우 제한한다.
- [x] import·replacement·formatting·Undo를 공통 편집 경로로 처리한다. 선택 후 문서가 바뀐 경우 이전 범위에 적용하지 않는다.

완료 예시:

```csharp
// 원본 Widget 식 위에서 Ctrl+. → Wrap with Padding
new Text("Hello")

// 필요한 import가 없으면 함께 추가한다.
new Padding(
    padding: EdgeInsets.CreateAll(8),
    child: new Text("Hello"))

// 여러 children 항목 선택 → Wrap with Column
new Column(children:
[
    new Text("A"),
    new Text("B"),
])
```

완료 기준: 오류 진단이 없는 정상 위젯에도 메뉴가 나타나고 단일/목록 wrapping, 취소, stale 결과 거부, Undo/Redo가 동작한다. 실제 위젯 조립 검사에서 parent data가 유효하다.

### E4 — 문맥 제안과 import·도움말 확대 (P1)

- [x] 일반 타입 이름, 클래스 생성, Widget 식, constructor 인자, builder 본문, State 멤버를 구분한다. 주석·문자열·비활성 전처리 영역에서는 구조 snippet을 제안하지 않는다.
- [x] `new Padding(`, `new Column(` 등에서 미입력 인자 묶음과 child/children/builder 패턴을 제안한다. 이미 있는 named argument, 다른 overload, 잘못된 delegate 본문을 삽입하지 않는다.
- [x] enum/factory와 builder 작성 지원을 실제 catalog에서 생성한다. 예: `MainAxisAlignment`, `CrossAxisAlignment`, `EdgeInsets.Create*`, `BorderRadius.Create*`, `(context, constraints) => ...`.
- [x] State 타입에서만 생명주기 override·setState 패턴을 제안하고 이미 구현한 멤버는 제외한다. Controller 패턴은 생성과 해제 위치를 함께 안내한다.
- [x] 고정 import 목록을 catalog 기반으로 확장하고 alias, local/project 타입, global using, csproj Using, implicit/generated using과 namespace 범위를 반영한다. `TextStyle`처럼 후보가 여러 개면 namespace를 명확히 표시한다.
- [x] Microsoft C# 확장 공존 상태에서 중복 completion/hover/signature를 확인한다. 기본 언어 정보는 C# 확장에 맡기고 Doroti 구성 예시·구조 제약·문서 링크만 필요한 위치에 추가한다.
- [x] restore 실패/미참조 package/C# 확장 비활성 등의 상태를 구분해 짧은 안내와 사용할 수 있는 fallback을 제공한다. 앱/파일을 제안 때문에 자동 수정하거나 dependency를 자동 설치하지 않는다.
- [x] 응답 시간을 측정한다. 초기 목표는 준비된 프로젝트에서 completion p95 ≤ 100ms, 캐시된 일반 코드 액션 제안 p95 ≤ 200ms다. 프로젝트 초기 로드와 큰 추출 실행 시간은 별도로 기록한다. 취소된 요청·빠른 연속 입력에서 오래된 결과를 표시하지 않는다.

완료 기준: 실제 인자/생명주기 문맥별 제안과 없는 import가 해결되고, `CustomCarousel`은 참조 앱에서만 실제 필수 인자·builder signature로 제안된다. 위 성능 수치는 목표이며 측정 전 PASS라고 기록하지 않는다.

### E5 — Widget 추출·Stateful 변환·안전한 구조 변환 (P1/P2)

- [x] `RefactorExtract`로 Widget 식을 새 StatelessWidget에 추출한다. 초기 지원은 같은 파일의 새 클래스이며 namespace·접근성·이름 충돌·key constructor를 처리한다.
- [x] 자유 변수, instance member, generic 타입, context, callback과 nullable 타입을 분석해 필요한 constructor 인자를 만든다. 원래 식의 평가 횟수/순서·변수 capture·접근성이 달라지는 경우 제한 이유를 표시한다.
- [x] Stateless → Stateful 변환을 구현한다. primary/일반 constructor, 필드·프로퍼티·base key·attributes와 build 참조를 보존하고 State에 옮길 코드와 Widget에 남길 계약을 구분한다. 안전하지 않은 partial/inheritance 사례는 변환하지 않는다.
- [x] 실제 생성자 계약이 허용하는 child/children 변환과 단순 wrapper 교체를 추가한다. 다른 타입으로 바꿔야 하는 경우 인자 의미까지 확인한다. Stateful → Stateless 역변환과 Swap with Parent는 별도 후속으로 둔다.
- [x] 구조 변경을 diff preview로 검토 가능하게 하고 적용/취소/Undo를 검증한다. 클래스 추가·타입 변경은 기존 Hot Reload 제한에 따라 Restart가 필요할 수 있음을 안내한다.

완료 기준: 추출·변환한 fixture가 컴파일되고 필요한 입력/callback/접근성이 유지된다. mounted State의 위치·타입이 달라지는 리팩터링을 상태 보존 Hot Reload가 보장하는 것으로 표시하지 않는다.

### E6 — 편집 전용 VSIX 검증·패키징·사용 안내 (필수)

- [x] 기존 `contracts.test.ts`의 import 회귀를 유지하고 새 snapshot/catalog/변환 경계 검사를 추가한다. 텍스트 기대값만 맞추지 말고 생성·변환 코드의 구문/컴파일·타입·trivia 보존도 확인한다.
- [x] `src/test/editingHost.ts`와 `runHost.ts --editing` 진입점을 추가한다. 설치한 VSIX에서 편집 기능만 검증하고 Windows Run/Hot Reload 요구와 분리한다. `editingHost.ts`와 `--editing`은 구현되어 설치형 검증에서 실행했다.
- [x] 실제 prefix 입력 후 completion 선택·Tab·이름 연동·import·Undo, 커서/선택 wrapping·목록 wrapping, Extract Widget·Stateful 변환을 VS Code host에서 검사한다. provider 반환값만 검사한 결과와 사용자 제안 선택 경로를 구분한다.
- [x] Microsoft C# 확장 공존/부재, Restricted Mode, 관련 없는 C# 프로젝트, 프로젝트 선택 전, 앱 실행 중, 미저장 파일, alias/global using, CRLF/한글/raw string·collection, restore 실패를 포함한다.
- [x] VS Code 최소 지원 버전 1.100과 현재 안정 버전에서 필요한 API/편집 동작을 확인한다. 구현 당시 버전·SDK·Roslyn·Node/npm을 결과에 고정한다. 미실행 OS/버전은 `notVerified`로 남긴다.
- [x] `.vscodeignore`와 package 스크립트를 갱신하고 catalog/helper DLL·런타임 의존성 포함을 검사한다. repository 밖 생성 앱에서도 source 경로에 기대지 않고 동작하는지 확인한다.
- [x] 대표 wrapper/추출 결과를 실제 Widget 트리로 조립해 parent data와 필수 인자를 검사한다. 기존 Run/Stop/Hot Reload는 변경 영향이 있는 경로만 회귀한다.
- [x] README에 prefix 목록, `Ctrl+.`/우클릭/명령 사용법, C# 확장 역할, 설정, 분석 실패 시 동작, Restart 필요 사례를 정리한다. 증거는 revision·환경·명령·예상/실제·잔여 제한과 함께 남긴다.

완료 기준: 설치한 VSIX에서 핵심 편집 흐름을 재현하고 생성 코드 검증·구조 검증·패키징 검사를 통과한다. Windows 편집 PASS를 macOS/Linux·기기·물리 입력·IME·접근성·scanout·배포 전체 PASS로 확대하지 않는다.

## 6. 구현 파일 구성

TypeScript 편집 provider는 `src/editing/`으로 분리했다. .NET helper의 계약/분석/변환은 `Contracts.cs`, `Engine.cs`, `Refactorings.cs`로 구성했고 `Program.cs`가 process protocol을 소유한다. 아래 설계상의 폴더 책임을 이 파일들로 합쳤다. `src/imports.ts`는 기존 호출을 위한 compatibility export다.

```text
tools/vscode-doroti/
  src/editing/
    projectContext.ts        # 문서/앱/참조 판별 및 캐시
    catalog.ts               # 템플릿·위젯 API 공통 모델
    completions.ts           # prefix·인자·생명주기 작성 지원
    imports.ts               # 기존 importPlan 확장/이동
    codeActions.ts           # Quick Fix·Rewrite·Extract 등록
    edits.ts                 # snapshot/version·import·Undo 적용
    newWidget.ts             # 위젯 파일 생성 및 snippet picker
    assistClient.ts          # helper process 및 취소 계약
  snippets/widgets.json      # 기존 템플릿 원본 확장
  src/test/editingHost.ts
tools/Doroti.Editor.Assist/
  contracts/                 # typed snapshot/result/version 계약
  analysis/                  # 구문·참조·타입·catalog
  refactorings/              # wrap/remove/extract/convert
  tests/                     # 지속 유지할 최소 회귀
```

## 7. 실행 순서와 검증 운영

실행 순서: **E0 → E1 → E2 → E3 → E4 → E5 → E6**. E6의 검증 코드는 각 단계에서 필요한 만큼 같이 추가하고, 마지막 단계에서 설치/패키징/공존 검증을 마무리한다. 먼저 `stl`/`stf`가 동작하는 첫 체크포인트를 만들고 이를 전체 계획 완료로 표시하지 않는다.

| 체크포인트 | 통과 조건 | 다음 단계 |
| --- | --- | --- |
| C1: 짧은 생성 흐름 | E0/E1, prefix/Tab/이름/import/파일 취소/기존 alias | 분석 기반 assists |
| C2: 위젯 구성 | E2/E3, 단일/목록/커서 wrapping, parent data, Undo | 문맥별 제안 확대 |
| C3: 작성 지원 | E4, 인자/builder/State/추가 package/공존/응답시간 | 구조 리팩터링 |
| C4: 구조 편집 | E5, 추출/변환 compile·preview·capture 보존 | 최종 VSIX 검증 |
| C5: 설치형 완료 | E6, 패키징·생성 앱·문서·검증 결과 | E0–E6 완료 판정 |

검증은 [저장소 지침](.github/copilot-instructions.md)에 따라 각 실행에 **20분 timeout**을 적용한다. 임시 앱·설치 profile·결과는 `temp/testing/vscode-editing/<run>/`에 둔다. 수백 회 반복 대신 대표 정상/실패/경계 사례와 회귀를 구성하며 대부분의 반복 측정은 30회 이내로 제한한다. 보존할 최소 결과·환경·hash는 추적 문서에 요약한다.

확장 검증·패키징, helper emit/Element/protocol 및 설치형 `--editing` 명령을 실행했다. 확정된 명령과 profile/환경은 README와 검증 기록에 있다.

```powershell
# Windows repository root에서 실행
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti test
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti run package
```

## 8. 최종 완료 조건과 현재 결과

- [x] `stl`/`stf`와 기존 alias로 생성 → 이름/Tab → 필요한 using → 생성 코드 컴파일이 통과한다.
- [x] 위젯 커서/선택에서 감싸기 메뉴가 보이고 단일/여러 자식·부모 제약·제거·Undo가 검증된다.
- [x] 생성자/child/children/builder/State 문맥에서 정확한 제안이 나타나며 C# 확장 공존·추가 package·제안 응답 기준을 통과한다.
- [x] Widget 추출과 Stateless → Stateful 변환이 입력/callback/namespace/접근성/생성자 계약을 보존한다.
- [x] 설치한 VSIX의 편집 전용 검증, catalog/helper 패키징, repository 밖 생성 앱, 기존 실행 기능 회귀와 README 갱신이 끝난다.
- [x] 실행한 검사의 PASS/PARTIAL/SKIPPED와 미확인 범위를 구분하고 E0–E6 잔여 항목이 없을 때만 `wholePlanComplete=true`로 바꾼다.

현재 결과: **C1–C5 / E0–E6 PASS**. 확장 0.2.0, Node 계약 12개, Roslyn/API/Element 검사 84개, 템플릿 38개 컴파일, helper snapshot/protocol 8개 검사, 설치한 VSIX의 VS Code 1.100.0/1.140.0·C# 2.160.4 공존·Restricted Mode·repository 밖 생성 앱 검증을 수행했다. 준비된 C# 공존 환경의 20회 측정에서 completion p95 8.98ms, 코드 액션 p95 9.56ms로 목표를 통과했다.

추출은 동일 파일의 순수 생성식과 명시적 값/callback 입력을 지원한다. Stateful 변환은 nested State로 private 접근·constructor/key/프로퍼티·primary 값과 build 시점 callback capture를 보존한다. partial/inheritance/base dispatch/mutable capture·해석되지 않는 참조 등은 제한 이유를 제공한다. 전체 C# 변환이나 모든 Widget 패턴 지원을 완료 의미에 포함하지 않는다.

기존 실행 session 기능은 유지하고 영향 있는 계약 회귀를 통과했다. 설치형 development 계약 fixture의 Run 중 편집 및 Hot Reload ack/Stop도 확인했다. 이번에 새로 네이티브 Run/Hot Reload, macOS/Linux, 물리 입력/IME/접근성/scanout/GPU/FPS, signing/clean-machine, NuGet-only/Marketplace 배포를 검증한 것은 아니다. 명령 자동화와 CPU Widget 조립의 범위를 넘는 항목은 **notVerified/범위 제외**다. 기존 Carousel·`.vscode` 소스는 수정하지 않았다.
