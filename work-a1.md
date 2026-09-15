# A1 — 남은 컴파일러 경고의 원인 수정 및 억제 제거

작성일: 2026-09-16  
상태: **계획 작성 완료 / A1 구현 미착수**

## 1. 목표와 범위

`Doroti/src/Doroti.Framework.*`에 남은 파일 단위 `#pragma warning disable`을 실제 코드 수정으로 제거한다. null 허용 여부, 초기화·해제 수명, 상속 계약, 비동기 완료·오류 전달, 동등성 동작을 확인하면서 진행한다.

최종 목표는 **현재 대상 410개 파일의 억제 0개**, Debug·Release의 대상 경고 0개, 변경한 동작 계약의 검증 통과다. 경고만 보이지 않게 만드는 변경은 완료로 인정하지 않는다.

이번 요청에서는 이 계획만 작성한다. 앞선 정리 작업의 소스 변경을 출발점으로 유지하며, 이 문서 작성 때문에 구현이나 빌드를 다시 실행하지 않는다.

### 포함

- 남은 21종의 경고 코드와 수정 과정에서 연쇄적으로 드러나는 관련 경고.
- 위 경고의 원인이 되는 `Doroti.Runtime`, `Doroti.Ui`, 공통 인터페이스·콜백·제네릭 계약.
- 변환기에서 반복 생성되는 문제가 확인된 경우 해당 공통 lowering 규칙과 작은 재현 fixture.
- 수정 범위에 맞는 컴파일·동작·제품 실행 검증, 재발 방지 검사.

### 별도 관리

- Material·Cupertino 프로젝트의 기존 `NoWarn`인 `CS0219`, `CS8524`, `CS8846`은 이번 1,406개 집계에 포함되지 않는다. 시작·종료 시 유지 여부를 기록하고, 별도 원인 분석 없이 함께 제거하지 않는다.
- Android API 호환성용 국소 억제, `MaterialImageQuantizerWu.cs`의 기존 nullable 비활성화, 외부 참조 소스는 이 작업의 일괄 삭제 대상이 아니다.
- 자동 생성 코드의 `#nullable enable`은 유지한다. 일반 제품 소스의 중복 선언 제거와 생성 코드의 nullable 문맥은 구분한다. [Microsoft: nullable 이행과 생성 코드](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/update-applications/nullable-migration-strategies)
- 렌더러·플랫폼 구성 변경, 기존 work1/work2 기능 확장, 전체 프레임워크 재생성은 범위에 포함하지 않는다.

## 2. 현재 확인된 기준선

### 2.1 앞선 정리 결과

| 항목 | 확인값 |
|---|---:|
| 중복 `#nullable enable`을 제거한 제품 소스 | 641개 파일 |
| 기존 포괄 억제가 있던 파일 | 632개 |
| 억제를 완전히 제거한 파일 | 222개 |
| 필요한 코드만 남겨 둔 파일 | 410개 |
| 파일별 억제 코드 수의 합계 | 14,684 → 1,406 |
| 정리한 도달 불가능 코드 진단 위치 | 677곳 |
| 기존 멤버 숨김을 `new`로 명시한 위치 | 154곳 |
| 정리한 미사용 예외 변수 | 10곳 |

2026-09-16 소스 재집계에서도 **410개 파일 / 1,406개 파일·경고 코드 조합**을 확인했다. 이 수는 실제 경고 발생 위치 수가 아니다. 한 파일의 `CS8600` 하나가 여러 위치의 경고를 억제할 수 있다.

앞선 무억제 진단 빌드의 **10,764건**은 후속 코드 정리 전 수치다. 현재 남은 실제 진단 수로 재사용하거나, 정리한 위치 수를 단순 차감하여 확정하지 않는다. A1-0에서 현재 소스 기준으로 다시 측정한다.

앞선 검증은 TestbedApp과 WidgetPreviews 각각의 Debug·Release 빌드 **경고 0 / 오류 0**, `git diff --check` 통과다. 이 결과는 남은 억제가 적용된 상태의 컴파일 증거이며, 억제된 문제가 해결되었다는 증거는 아니다.

로컬 증거: [요약](Doroti/artifacts/warning-cleanup-20260916/summary.json), [남은 파일별 목록](Doroti/artifacts/warning-cleanup-20260916/remaining-pragmas.json), [Debug 빌드](Doroti/artifacts/warning-cleanup-20260916/verify-debug.log), [Release 빌드](Doroti/artifacts/warning-cleanup-20260916/verify-release.log). `artifacts`는 Git 제외 경로이므로 다른 환경에 파일이 없으면 A1-0에서 재수집한다.

### 2.2 모듈별 잔여

| 모듈 (`Doroti.Framework.` 접두사 생략) | 억제 파일 | 파일·코드 조합 |
|---|---:|---:|
| Scheduler | 1 | 1 |
| Services | 14 | 26 |
| Gestures | 1 | 2 |
| Painting | 8 | 13 |
| Semantics | 1 | 4 |
| Rendering | 14 | 21 |
| Widgets | 148 | 524 |
| Cupertino | 48 | 206 |
| Material | 175 | 609 |
| **합계** | **410** | **1,406** |

Foundation·Physics·Animation·WidgetPreviews는 이 대상 억제가 현재 0개다. 공통 계약 수정의 의존성 회귀 검증에는 포함한다.

### 2.3 경고 종류별 잔여

아래 숫자는 해당 코드를 억제한 **파일 수**다. 여러 행에 같은 파일이 포함된다.

| 경고 | 파일 수 | 수정 시 확인할 계약 |
|---|---:|---|
| CS8600 / CS8601 | 373 / 104 | nullable 값의 변환·할당과 지역 변수·필드 타입 |
| CS8602 / CS8603 / CS8604 | 195 / 352 / 177 | 역참조, 반환값, 호출 인수의 null 가능성 |
| CS8605 / CS8629 | 91 / 10 | null unboxing, nullable 값 형식의 값 추출 |
| CS8619 / CS8620 | 12 / 37 | 컬렉션·제네릭·인수의 내부 nullable 계약 |
| CS8609 / CS8613 | 11 / 1 | override·interface 반환 타입의 nullable 계약 |
| CS8622 / CS8765 / CS8767 | 3 / 4 / 1 | delegate·override·interface 매개변수의 nullable 계약 |
| CS8625 / CS8714 | 10 / 5 | null 리터럴 전달, `notnull` 제약과 키 타입 |
| CS0693 | 10 | 외부 타입과 메서드의 제네릭 매개변수 이름 충돌 |
| CS4014 | 4 | 비동기 실행의 완료·오류·취소 소유자 |
| CS0659 | 3 | `Equals`와 `GetHashCode`의 일관성 |
| CS8321 | 2 | 사용되지 않는 지역 함수와 누락된 호출 여부 |
| CS8981 | 1 | 소문자 ASCII 타입 이름과 외부 호환성 |

경고 의미와 수정 방법은 [Microsoft nullable 경고 문서](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/nullable-warnings)를 기준으로 삼되, 각 메서드의 실제 제품 계약은 현재 소스와 참조 구현으로 판단한다.

## 3. 수정 원칙

1. **계약이 선언되는 곳부터 수정한다.** 반환값이 실제 nullable이면 선언·인터페이스·override·delegate·호출부를 함께 맞춘다. 상위 계약 오류를 호출부의 `!`로 덮지 않는다.
2. 정상적인 “값 없음”과 계약 위반을 분리한다. nullable 반환, 조건 분기, 명시적 실패 중 원래 동작에 맞는 것을 선택한다. 경고 제거를 위해 빈 문자열·빈 컬렉션·0을 임의로 반환하지 않는다.
3. `!`, `default!`, `null!`, 이중 캐스트, `dynamic`, `ConvertValue<T>` 호출을 대량 추가하지 않는다. 기존 항목을 무조건 삭제하지도 않으며, 새 assertion은 실행 불변조건과 필요한 근거가 있는 위치로 한정한다.
4. `NotNullWhen`, `MemberNotNull` 등의 속성은 구현이 해당 조건을 실제로 보장할 때만 사용한다. 단순 debug assertion이나 해제 가능한 필드에 거짓 보장을 붙이지 않는다. [Microsoft nullable 분석 속성](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis)
5. `await` 추가는 실행 순서·프레임 타이밍을 바꿀 수 있다. `_ =`만 붙여 경고를 없애지 않고, 완료 대기 또는 독립 실행의 소유자와 오류 경로를 정한다.
6. 공개 API의 nullable 주석도 호출자 컴파일 결과에 영향을 준다. 제네릭 제약, 오버로드 선택, optional 인수, delegate 및 가상 호출 호환성을 함께 확인한다.
7. 전역 `NoWarn`, `.editorconfig` severity 변경, `#nullable disable`, 파일 전체 억제 재확대, `TreatWarningsAsErrors=false`를 제품 해결책으로 사용하지 않는다.
8. 현재 수정 중인 소스와 사용자 변경을 보존한다. 임시 진단용 변경을 되돌릴 때 파일 전체 checkout/reset으로 기존 작업을 덮어쓰지 않는다.
9. 예외적인 국소 억제가 정말 필요하면 위치·이유·대체 방안·재현·재검토 조건을 기록한다. 억제가 남은 상태는 목표 미달로 표시하며, 이를 “전체 제거 완료”로 보고하지 않는다.

## 4. 단계별 실행 계획

### A1-0 — 현재 진단과 계약별 작업 목록 확보

- [ ] 현재 HEAD, dirty diff, .NET SDK, 설정, 대상 프로젝트 그래프와 소스 hash를 기록한다. 기준선은 커밋뿐 아니라 **앞선 미커밋 정리를 포함한 작업 트리**다.
- [ ] 410개 파일의 억제 목록과 기존 전역·국소 억제를 분리해 저장한다.
- [ ] 현재 작업 트리의 격리 복사본에서 대상 pragma만 제거하고 Debug·Release 진단을 수집한다. 기존 산출물 때문에 진단이 생략되지 않도록 격리된 중간 출력과 재컴파일을 보장한다.
- [ ] 진단 수집에 한해 격리 실행의 `TreatWarningsAsErrors=false` 사용을 허용한다. 제품 설정 파일은 변경하지 않으며, 최종 검증에는 이 override를 사용하지 않는다.
- [ ] `(project, configuration, file, symbol, code, location)`을 보존한다. MSBuild 요약 재출력으로 같은 진단을 중복 집계하지 않는다. Debug·Release는 각각 집계하고 합집합도 별도 기록한다.
- [ ] 진단을 `공통 선언 오류 / 지역 흐름 오류 / 수명 오류 / 변환기 패턴 / 외부 계약 한계`로 분류한다. 파일 수, 파일·코드 조합 수, 실제 진단 위치 수를 따로 관리한다.
- [ ] 공개 API와 고위험 경로를 표시하고 첫 작업 묶음을 확정한다.

**완료 기준:** 현재 코드의 재현 가능한 진단 목록, 설정·명령·출력 로그, 수정 순서와 계약 소유 위치 확보. 과거 10,764건을 현재 기준선으로 대체하지 않는다.

### A1-1 — 독립적인 선언·미사용 코드 정리

- [ ] `CS0693`: `Painting/colors.cs`, `Widgets/inherited_model.cs`, `widget_state.cs`, `shortcuts.cs`, `routes.cs`, `router.cs`, `radio_group.cs`, `autocomplete.cs`, Cupertino route/sheet의 메서드 타입 매개변수를 의미에 맞게 구분한다. 몸체·제약·재귀 호출·`typeof` 참조도 함께 수정한다.
- [ ] 특히 `InheritedModel<T>`의 메서드 내부에 같은 이름의 `T`가 다시 선언되어 있다. 단순 이름 교체 전에 외부 aspect 타입과 내부 widget 타입의 의도를 확인하고 `CS8714`와 연결된 잘못된 타입 사용 여부를 검토한다.
- [ ] `CS8321`: Semantics 및 widget inspector의 지역 함수가 실제로 불필요한지, 빠진 연결을 복원해야 하는지 판단한다.
- [ ] `CS8981`: widget inspector의 해당 타입이 내부 구현인지 확인한다. 내부 이름이면 참조까지 변경하고, 공개·직렬화·reflection 이름이면 호환성 경로부터 설계한다.
- [ ] 해결한 경고 코드만 해당 파일의 pragma에서 제거한다.

**완료 기준:** 대상 경고 0, 타입 참조·제네릭 호출 컴파일 통과. 단순 이름 변경에 구현을 그대로 반복하는 테스트는 추가하지 않는다. 실제 계약 오류를 고친 경우에만 의미 있는 재현 검증을 추가한다.

### A1-2 — 공통 Runtime·Ui·Foundation 계약 정리

- [ ] `DartRuntimePrimitives.RequireValue`, `RequireReference`, `ConvertValue`, null-aware helper, `DartAsync.Future<T>`, 컬렉션·callback adapter의 입력·출력 계약을 표로 작성한다.
- [ ] 현재 `ConvertValue<T>(null)`은 `default!`를 반환한다. 이 동작을 non-null 보장으로 취급하지 않는다. 변경이 필요하면 참조형·값형·nullable 값형 호출자와 Dart 변환 의미를 먼저 확인한다.
- [ ] `RequireValue`의 기존 null assertion 예외와 발생 시점을 보존한다. 경고 제거만을 위해 새로운 실패 경로나 예외 종류를 삽입하지 않는다.
- [ ] `Scheduler/ticker.cs`의 nullable `onTimeout` 전달과 `DartAsync.cs`의 `timeout` 오버로드를 추적한다. null callback 허용 여부·오버로드 선택·timeout 결과 전달을 함께 수정한다.
- [ ] 공통 인터페이스의 반환값, 컬렉션 원소, delegate 입력·출력, generic 제약을 호출 그래프에 따라 정리한다. false/true 분기와 정상/예외 경로를 표현할 수 있으면 정확한 분석 속성을 사용한다.
- [ ] 정상값·null·값 없음·실패·취소를 대표하는 작은 계약 검증을 준비한다. 가상 메서드·interface 호출을 모두 확인한다.

**완료 기준:** 수정한 공통 계약의 생산자·소비자가 일치하고, 기존 정상 동작과 명시적 실패 동작을 검증한다. 넓은 영향이 확인되면 공통 변경을 작은 단위로 나눠 완료한 뒤 다음 단계로 진행한다.

### A1-3 — 하위 프레임워크 억제 제거

- [ ] 실제 프로젝트 의존 순서로 Scheduler → Services/Gestures → Painting/Semantics → Rendering을 진행한다.
- [ ] 이미지·asset 조회 실패, 콜백 부재, 포인터·gesture 종료, parent/child 연결, attach/detach, layout 이전·이후의 값 유무를 확인한다.
- [ ] `CS8605`, `CS8629`는 cast나 `.Value` 이전의 존재 보장 또는 nullable 반환 계약으로 수정한다. 참조형 null과 값형 default를 혼동하지 않는다.
- [ ] 반복되는 이중 캐스트는 정적 타입 흐름을 복원한다. 런타임 타입 검사를 없애거나 실패를 기본값으로 삼키지 않는다.
- [ ] 정리한 파일의 pragma를 제거하고 직접 의존 프로젝트를 검증한다. 앞 단계 변경으로 새로 드러난 경고도 원인과 함께 처리한다.

**완료 기준:** 이 단계의 대상 억제 0, 모듈·의존 프로젝트 Debug/Release 통과, 변경한 수명·실패 계약의 검증 통과.

### A1-4 — Widgets 핵심 계약 및 비동기·동등성 수정

다음 묶음 순서로 진행하되 A1-0에서 확인한 실제 의존성에 맞춰 조정한다.

1. `framework.cs`, `binding.cs`, inherited 계열: Element/State 수명, mounted/dispose, context 조회·반환.
2. `navigator.cs`, `routes.cs`, `router.cs`: route 부재, 결과 null, overlay 연결·해제, 취소.
3. focus·shortcuts·actions, `editable_text.cs`, selection, `autocomplete.cs`: 선택적 callback, 해제 후 호출, 입력 상태.
4. scroll/sliver/animation/listener 계열: attach·detach, 완료·취소 후 callback, 연결 객체 존재 조건.
5. inspector·나머지 widget: diagnostics 값 부재와 도구 연결 수명.

- [ ] 해제된 객체를 `!`로 강제 통과시키지 않고, 어느 상태에서 필드가 유효한지와 callback 취소/해제 소유권을 명시한다.
- [ ] `CS4014`: `Services/binding.cs`, `Widgets/dismissible.cs`, `draggable_scrollable_sheet.cs`, `Material/refresh_indicator.cs`를 함께 검토한다. 완료를 기다리는 흐름과 독립 실행을 구분하고 오류·취소 전달을 유지한다. [Microsoft 비동기 경고](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/async-await-errors)
- [ ] 독립 실행이 의도이면 기존 `Observe` 등 관측 경로의 동작을 확인하여 사용한다. Future/Task의 scheduler·프레임 실행 순서·예외 전달 의미를 검증한다.
- [ ] `CS0659`: `framework.cs`, `shortcuts.cs`, `widget_state.cs`의 값 동등성·참조 동등성 의도를 확인한다. 동일한 객체가 같은 hash를 가져야 하며, Dictionary/HashSet 조회·제거 동작을 검증한다. [Microsoft 동등성 경고](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/overloaded-operator-errors)
- [ ] callback 부재, navigation 취소, dispose 이후 비동기 완료, listener 연결·해제, 동등한 key 조회를 대표 회귀 항목으로 삼는다.

**완료 기준:** Widgets 대상 억제 0, 상태 전이·비동기·동등성 검증 통과. 비동기/동등성 수정으로 생긴 의미 있는 동작 차이는 수정 근거와 재현을 남긴다.

### A1-5 — Cupertino·Material 상위 위젯 정리

- [ ] Cupertino를 먼저, 그다음 Material을 공통 theme/style → 단순 control → 복합 입력·menu/search → route/dialog/picker 순으로 정리한다.
- [ ] `input_decorator.cs`, `menu_anchor.cs`, `search_anchor.cs`, `switch.cs`, `tabs.cs`, `segmented_button.cs` 등 복합 계약이 많은 파일은 독립 작업 묶음으로 나눈다.
- [ ] theme fallback, disabled 상태의 callback null, 선택값 없음, 취소된 picker 결과, animation 초기값의 의미를 보존한다.
- [ ] 공개 API·override·delegate nullability와 해당 wrapper·factory 호출자를 함께 수정한다. 설정이 없는 상태와 기본 설정이 있는 상태를 검증한다.
- [ ] 앞 단계에서 정리한 Widgets 계약의 경고가 상위에서 재등장하면 상위 캐스트로 덮지 않고 선언·구현 불일치부터 수정한다.
- [ ] 변경한 control의 mount → interaction → dispose 흐름을 Testbed에서 확인한다. UI 동작을 변경한 경우 Windows 및 Web의 해당 화면으로 확인 범위를 좁힌다.

**완료 기준:** Cupertino·Material 대상 억제 0, Debug/Release 통과, 변경한 control 동작 확인. 모든 화면을 매 작업 묶음마다 반복 실행하지 않는다.

### A1-6 — 변환기 재발 원인 및 검사 자동화

- [ ] `tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering/FrameworkCSharpLowerer.cs`가 여전히 포괄 pragma를 출력하는 사실을 별도 기록한다. 제품 소스 정리와 생성 후보의 상태를 혼합하지 않는다.
- [ ] 제품 수정에서 반복된 원인을 작은 Dart fixture로 재현한다. nullable callback, return, generic 제약, override/interface 계약, FutureOr, 불필요 cast 중 실제로 해당하는 공통 규칙을 수정한다.
- [ ] 생성 코드의 `#nullable enable`은 유지하고, 고친 패턴을 검증하는 fixture에서는 대상 경고를 억제 없이 컴파일하여 재발을 검출한다.
- [ ] 고정된 legacy 후보가 기존 억제를 필요로 한다면 적용 범위·잔여 진단을 기록한다. blanket 출력 문자열 삭제만으로 변환기 수정 완료를 선언하지 않는다.
- [ ] 기존 [virtual-dispatch 검증](tools/Doroti.DartToCSharp/validation/virtual-dispatch/README.md)을 재사용한다. 새로운 nullable fixture가 필요하면 현재 검증 구조에 맞게 추가하되, 생성 후보를 제품 소스에 자동 복사하지 않는다.
- [ ] 제품 대상 디렉터리의 포괄 억제 재도입, `Nullable`·`TreatWarningsAsErrors` 약화, 우회용 `NoWarn` 증가를 검출하는 작은 소스/설정 검사를 추가한다. 기존 제외 항목을 명시하고 전체 저장소의 정당한 국소 억제를 일괄 금지하지 않는다.

**완료 기준:** 실제 수정한 lowering 패턴의 재현·생성·컴파일 검증 통과, 제품 억제 재도입 검사 동작. 전체 변환기의 무경고 지원으로 확대 해석하지 않는다.

### A1-7 — 통합 검증과 종료 판정

- [ ] 원래 설정으로 TestbedApp, WidgetPreviews Debug·Release 빌드를 수행한다.
- [ ] 대상 파일들의 pragma 0, 해당 경고에 대한 다른 억제 경로 추가 0을 확인한다. 간접 계약 수정으로 새로 드러난 경고도 남기지 않는다.
- [ ] 변경 전후 공개 API·nullable 주석·제네릭 제약·오버로드 차이를 검토한다.
- [ ] 변경한 경로의 계약 검증과 필요한 제품 화면 검증을 통합한다. 플랫폼 전용 코드가 영향을 받으면 해당 runner의 빌드/실행을 추가한다.
- [ ] 소스 변경 목록, 제거한 억제, 수정한 계약, 실행 명령, 결과, 남은 위험과 환경상 미검증 항목을 기록한다.
- [ ] `git diff --check` 및 재발 방지 검사를 통과시킨다.

**종료 판정:** 소스·컴파일 목표와 필수 동작 검증을 모두 만족하면 완료. 억제·미해결 진단·필수 검증 공백이 남으면 `PARTIAL`로 남기고 파일/심벌/이유/다음 작업을 명시한다.

## 5. 검증 운영

### 5.1 명령과 timeout

모든 테스트는 `.github/copilot-instructions.md`에 따라 **20분(1,200초) 제한**으로 실행한다. 긴 빌드·진단 프로세스도 같은 상한을 적용한다. child process까지 종료·회수하고 timeout을 실패로 기록하는 wrapper 안에서 아래 명령을 사용한다.

```powershell
# Debug와 Release 각각 실행. 최초 또는 의존성 변경 시 필요한 restore를 먼저 수행한다.
dotnet build DorotiTestbedApp/DorotiTestbedApp.csproj -c Debug --no-restore --nologo --disable-build-servers --tl:off
dotnet build DorotiTestbedApp/DorotiTestbedApp.csproj -c Release --no-restore --nologo --disable-build-servers --tl:off
dotnet build Doroti/src/Doroti.Framework.WidgetPreviews/Doroti.Framework.WidgetPreviews.csproj -c Debug --no-restore --nologo --disable-build-servers --tl:off
dotnet build Doroti/src/Doroti.Framework.WidgetPreviews/Doroti.Framework.WidgetPreviews.csproj -c Release --no-restore --nologo --disable-build-servers --tl:off

# 변환기를 수정한 경우. 이 검증 스크립트는 자체 20분 제한을 제공한다.
pwsh -NoProfile -File tools/Doroti.DartToCSharp/validation/virtual-dispatch/validate.ps1

git diff --check
```

작업 묶음마다 수정 프로젝트와 직접 소비자를 먼저 검사하고, 단계 종료 때 통합한다. 동일 소스에서 이미 통과한 장시간 검증을 근거 없이 반복하지 않는다. fixture 경로와 runner는 구현 시작 시 현 저장소에서 확인하며, 존재하지 않는 과거 검증 프로젝트를 실행 계획에 사용하지 않는다.

### 5.2 증거 수준

| 수준 | 확인하는 것 | 대체할 수 없는 것 |
|---|---|---|
| 소스 검사 | 억제·설정 변화, 대상 범위 | 컴파일과 런타임 안전성 |
| Debug/Release 빌드 | 타입·nullable 계약과 컴파일 진단 | 비동기 실행 순서, null 입력의 실제 결과 |
| 계약 fixture | 특정 입력·수명·실패·동등성 동작 | 실제 UI 입력과 플랫폼 통합 |
| Windows/Web 제품 실행 | 변경한 화면·입력·dispose 흐름 | 다른 OS 및 물리 입력 장치의 동작 |
| 플랫폼/물리 검증 | 해당 환경의 실제 동작 | 실행하지 않은 환경의 보장 |

플랫폼 영향을 받지 않는 소스 정리 때문에 무관한 물리 테스트를 추가하지 않는다. 영향을 받았지만 실행할 수 없는 검증은 `notVerified`, 사용자가 제외한 검증은 `skippedByUser`로 기록한다. 빌드 결과를 제품·물리 검증으로 승격하지 않는다.

## 6. 산출물과 진행 기록

- 구현 단계의 신규 증거 경로: `Doroti/artifacts/warning-remediation-a1/<run-id>/`.
- 기록 파일: `baseline.json`, `diagnostics-debug.json`, `diagnostics-release.json`, `warning-ledger.json`, `api-diff.md`, `validation-summary.md` 및 원본 로그. 이는 생성 예정 이름이며 현재 존재한다고 가정하지 않는다.
- 진단 원장 필수 필드: project, configuration, file, symbol, code, 원인, 계약 소유 위치, 수정 내용, 검증 근거, 상태, 잔여 이유.
- 작업 묶음별로 `진단 수 / 억제 파일 수 / 파일·코드 조합 수 / 새로 드러난 진단 수`를 기록한다. 원인 수정 중 발생한 새로운 경고를 숨겨 숫자만 줄이지 않는다.
- 사용자에게는 완료 모듈, 남은 모듈, 실제 검증 범위와 다음 작업을 요약한다. artifacts가 Git 제외라는 점을 고려해 재현에 필요한 검증 소스·작동 방법은 저장소에 유지한다.

| 단계 | 상태 | 종료 증거 |
|---|---|---|
| A1-0 기준선·진단 | TODO | 현재 소스의 진단 목록과 실행 로그 |
| A1-1 선언·미사용 코드 | TODO | 대상 경고 제거와 컴파일 |
| A1-2 공통 계약 | TODO | 입력·출력·실패·가상 호출 계약 검증 |
| A1-3 하위 Framework | TODO | 모듈 억제 0과 수명 검증 |
| A1-4 Widgets·비동기·동등성 | TODO | 모듈 억제 0과 회귀 검증 |
| A1-5 Cupertino·Material | TODO | 모듈 억제 0과 변경 화면 검증 |
| A1-6 변환기·재발 방지 | TODO | 실제 수정 패턴 fixture와 검사 |
| A1-7 통합 종료 | TODO | 억제 0, 원래 설정 빌드, 필수 검증 |

## 7. 최종 완료 체크리스트

- [ ] 현재 대상 410개 파일의 경고 억제를 모두 제거했다.
- [ ] 전역/국소 다른 경로로 같은 경고를 숨기지 않았다.
- [ ] `!`, 기본값 반환, 캐스트, 분석 속성으로 실제 계약 문제를 덮지 않았다.
- [ ] 공개 계약·수명·비동기·동등성 변경의 근거와 의미 있는 검증을 남겼다.
- [ ] TestbedApp·WidgetPreviews의 Debug·Release가 원래 설정으로 경고 0 / 오류 0이다.
- [ ] 해당하는 변환기 재발 패턴과 제품 억제 재도입 검사를 확인했다.
- [ ] 필수 제품 실행 검증을 수행하고, 미검증 범위는 별도로 표시했다.
- [ ] 앞선 사용자 작업을 보존했고 diff 검사와 결과 기록을 완료했다.

이 목록이 충족되기 전에는 “남은 경고 원인 수정 완료”로 보고하지 않는다.
