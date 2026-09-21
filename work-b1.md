# B1 — RequireValue 제거 및 C# null 처리 정리

작성일: 2026-09-21  
상태: **계획 작성 — 구현 미착수**

## 1. 목표와 범위

`Doroti.Runtime.DartRuntimePrimitives.RequireValue`의 제품 코드 의존성과 변환기의 생성 의존성을 제거한다. 필요한 null 검사는 C#의 명시적인 분기·패턴·`?? throw`로 표현하고, 생략 가능한 인자는 올바른 기본값을 적용한다. 정상 입력, 기본값, 예외 발생 조건, 콜백 실행 시점, 위젯 수명을 보존한다.

이 문서는 작업계획이다. 이번 문서 작성에서는 제품 코드·변환기·검증 코드를 변경하거나 구현 검증을 실행하지 않는다. 앞서 완료한 날짜·시간 피커 수정과 회귀 검증은 구현의 출발점으로 유지한다.

### 포함

- `Doroti/src`의 `RequireValue` 호출, 메서드 참조, 관련 타입·null 흐름.
- `tools/Doroti.DartToCSharp`의 호출 생성 규칙, 생성 결과에 의존하는 호환성 규칙 및 관련 IR 사용 여부.
- 기존 검증의 직접 호출 제거와 동작 계약을 검증하는 fixture로의 전환.
- 모든 소비자 전환 후 `DartRuntimePrimitives.cs`의 `RequireValue` 오버로드 3개 삭제.
- 수정된 계약의 자동 검증, 대표 Testbed 실행 확인 및 미검증 플랫폼 기록.

### 범위에서 제외

- `DartRuntimePrimitives` 클래스 전체, `ConvertValue`, 기타 런타임 의미 지원 기능의 제거.
- 기존 `RequireReference` 27회 사용의 일괄 제거. 다만 `RequireValue`를 이 함수로 바꿔 목표를 달성한 것으로 처리하지 않는다.
- Flutter 참조 소스 변경, 프레임워크 전체 재생성·덮어쓰기, 렌더러·플랫폼 아키텍처 변경.
- 관련 없는 스타일 정리, 경고 억제 확대, 테스트 횟수 확대, 근거 없는 성능 최적화.

## 2. 현재 소스 기준선

2026-09-21에 `Doroti/src` 아래 C# 파일에서 `bin`·`obj`를 제외하고 `DartRuntimePrimitives.RequireValue` 문자열을 집계했다. **285개 파일 / 3,618회**다. 이는 텍스트 출현 수이며, Roslyn으로 확인한 호출 심볼 수나 독립 버그 수가 아니다. 구현 시작 시 별칭·정적 import·명시적 형식 인자·메서드 그룹까지 포함해 다시 집계한다.

| 모듈 (`Doroti.Framework.` 접두사 생략) | 출현 수 | 파일 수 |
|---|---:|---:|
| Material | 1,105 | 83 |
| Widgets | 921 | 80 |
| Rendering | 802 | 35 |
| Painting | 303 | 29 |
| Cupertino | 248 | 25 |
| Gestures | 74 | 13 |
| Semantics | 74 | 2 |
| Animation | 44 | 5 |
| Services | 30 | 10 |
| Scheduler | 11 | 2 |
| Physics | 6 | 1 |
| **합계** | **3,618** | **285** |

현재 구현과 검토 지점:

- [런타임 구현](Doroti/src/Doroti.Runtime/DartRuntimePrimitives.cs): nullable 값 형식, non-nullable 값 형식, 참조 형식용 오버로드가 있다. nullable 값·참조 형식은 null에서 `NullReferenceException("Dart null assertion failed.")`을 발생시키고, non-nullable 값 형식은 입력을 그대로 반환한다. `[NotNull]`에 의한 컴파일러 흐름 정보도 고려해야 한다.
- [TextFormField](Doroti/src/Doroti.Framework.Material/text_form_field.cs): 피커 오류는 옵션 기본값이 실제 builder 콜백에 적용되지 않은 사례다. 현재는 `stylusHandwritingEnabled ?? EditableText.defaultStylusHandwritingEnabled`로 수정돼 있다. 이 문제를 모든 null 검사가 불필요하다는 근거로 일반화하지 않는다.
- [변환기 lowering](tools/Doroti.DartToCSharp/src/Backend/CSharp/Lowering): `Expressions`, `Invocations`, `Declarations`, `Statements`, `Members`, `Compatibility`, `G53Compatibility` 등 여러 경로에서 호출을 만들거나 생성 문자열을 매칭한다.
- [RuntimeIntrinsic](tools/Doroti.DartToCSharp/src/Core/Ir/RuntimeIntrinsic.cs): `RequireValue` 항목이 있다. 실제 생산·소비 경로와 직렬화 계약 여부를 조사한 뒤 처리한다.
- [기존 런타임 계약 검증](Doroti/validation/warning-remediation/Contracts/Program.cs): null 예외, nullable 값의 0, 참조 null 흐름 등을 직접 검사한다. 함수 삭제에 맞춰 의미를 검증하는 테스트로 이관해야 한다.
- [피커 회귀 검증](Doroti/validation/picker-input/README.md): 실제 프레임워크·Skia로 기본 옵션, 입력 모드 왕복, 수정값 유지, 잘못된 값, inputOnly를 검사한다. 네이티브 입력 검증 범위는 별도로 명시돼 있다.

## 3. 동작 보존 원칙과 치환 규칙

| 분류 | 전환 방향 | 반드시 확인할 조건 |
|---|---|---|
| non-nullable 값 형식 | 호출을 제거하고 식을 직접 사용 | 결과 형식, 오버로드 선택, implicit conversion 유지 |
| 이미 null 검사가 끝난 지역 변수 | 패턴으로 추출한 non-null 값 또는 검증된 지역 변수 사용 | 제어 흐름 전체에서 non-null이 입증되는지 확인 |
| nullable 값 형식의 필수값 | `value ?? throw new NullReferenceException(...)` 또는 명시적 분기 | 0·false·enum의 0을 유효한 값으로 유지 |
| nullable 참조 형식의 필수값 | `value ?? throw ...` 또는 `is null` 검사 뒤 사용 | 기존 null 예외 조건·형식·메시지와 downstream null 흐름 유지 |
| 기본값이 있는 선택 인자 | 선언된 계약에 따라 `value ?? defaultValue` 또는 진입점에서 정규화 | 생략과 명시적 null의 의미, 상속된 기본값, builder의 캡처 대상 확인 |
| nullable 자체가 정상값인 API | nullable 형식을 유지하고 그대로 전달 | 원래 Dart/API 계약에 null 허용 근거가 있어야 함 |
| 제네릭·dynamic·형식 변환 결합 | 형식 제약·호출 심볼을 분석하여 분기 또는 지역 변수로 풀기 | `T`, `T?`, `where T : class/struct`, nullable 값·참조 혼합, 동적 바인딩 유지 |
| 중첩 호출·getter·메서드 호출·대입식 | 필요한 경우 임시 지역 변수로 한 번만 평가 | 부수효과 횟수, 좌우 평가 순서, 예외 발생 시점 유지 |

필수 규칙:

1. 정규식으로 모든 호출을 제거하거나 C#의 `!`, `.Value`, `GetValueOrDefault()`로 일괄 치환하지 않는다. `!`는 런타임 검사를 추가하지 않고, `.Value`는 null 예외 계약이 다르며, `GetValueOrDefault()`는 실패를 정상값으로 바꾼다.
2. nullable 메서드 결과·변경 가능한 프로퍼티를 검사 후 다시 읽지 않는다. 검사와 사용 사이에 값이 달라질 수 있으므로 단일 평가 결과를 사용한다.
3. `??` 우변, `?.`, 조건식, 논리 연산의 단락 평가와 콜백·async·iterator의 지연 실행을 유지한다. 콜백 안의 검사를 생성자 밖으로 당겨 실행하지 않는다.
4. `RequireValue`의 `[NotNull]`로 이전에 확보하던 흐름 정보를 분기·패턴·지역 변수로 대체한다. `#nullable disable`, 새 `NoWarn`, 포괄 pragma, 근거 없는 `!`로 경고를 숨기지 않는다.
5. 기본값은 원래 계약이 있는 곳에만 복원한다. 실제 Dart nullable 인자의 명시적 null까지 기본값으로 바꾸지 않는다. 생성된 C#의 null이 생략용 대체 표현인 경우와 실제 nullable 값인 경우를 구분한다.
6. 기본값 정규화는 생성자 본문뿐 아니라 base initializer, field initializer, factory, 상속된 선택 인자, builder 콜백에서 일관되게 적용한다. 값의 평가 횟수와 수명을 보존한다.
7. null 실패가 필요한 경로는 Release에서도 실패해야 한다. `Debug.Assert`나 디버그 전용 검사로 대체하지 않는다.
8. 새 helper·확장 메서드·별칭 또는 `RequireReference`로 기존 구현을 옮기는 방식은 제거 완료로 인정하지 않는다.
9. 공개 런타임 메서드 삭제는 외부 소비자의 소스·바이너리 호환성을 깨뜨릴 수 있다. 호출부 0건과 API 삭제를 별도 단계로 관리하고, 삭제한 API와 재빌드 필요성을 변경 안내에 기록한다. 호환성 shim을 유지한 상태는 최종 완료가 아니다.

## 4. 단계별 작업

### B1-0 — 기준선과 소비자 목록 확보

- [ ] 현재 commit·작업 트리 변경·SDK·기존 검사 결과를 기록하고, 다른 작업 변경을 보존한다.
- [ ] 전체 저장소의 실행 코드에서 런타임 메서드 심볼 참조를 찾는다. 제품·앱·도구·검증, qualified/unqualified 호출, 메서드 그룹을 구분한다.
- [ ] Roslyn SemanticModel로 호출 위치·오버로드·인자/결과 형식·nullable 흐름·부수효과·제네릭 문맥을 분류한다.
- [ ] 각 위치에 적용 규칙·검증 항목·미분류 사유를 연결한 manifest를 만든다. 미분류 항목은 자동 수정하지 않는다.
- [ ] 런타임 공개 API 소비자와 IR 항목의 실제 참조·직렬화 여부를 조사한다.
- [ ] 기존 런타임 계약 및 피커 검증을 필요한 구성에서 실행하고, 기존 실패를 B1 실패와 구분한다.

완료 기준: 모든 호출이 분류되거나 명시적 미분류 목록에 들어가며, 이후 감소량을 비교할 기준선이 있다.

### B1-1 — C# 표현 전략과 자동 수정 도구 준비

- [ ] 작은 fixture로 null/정상값, 기본값, 형식 제약, 평가 순서, nullable 흐름을 먼저 고정한다.
- [ ] 타입 분석에 기반한 수정 도구를 준비한다. dry-run에서 예상 diff·수정 사유·미분류를 출력하고 실제 파일 쓰기는 별도 모드로 수행한다.
- [ ] 확실한 단순 사례부터 자동화한다. 복잡한 식은 명시적 분기·지역 변수로 전환하며 평가 위치를 보존한다.
- [ ] AST trivia, 주석, 기존 수동 수정과 이름 충돌을 보존한다. 두 번째 실행에서 추가 diff가 없어야 한다.
- [ ] 제네릭·dynamic·nullable 형식과 오버로드가 달라지는 사례는 전후 컴파일 및 동작 결과를 비교한다.

완료 기준: 치환 규칙별 작은 계약 검증이 통과하고, 불확실한 사례를 조용히 변환하지 않는다.

### B1-2 — 변환기 생성 규칙 수정

- [ ] `FrameworkCSharpLowerer.*`의 생성 경로를 모두 조사하고, 공통 생성 로직에서 null assertion·null promotion·기본값 복원을 구분한다.
- [ ] Dart의 non-null assertion은 직접 C# 검사로 생성하고, 이미 non-null인 값에는 중복 검사를 생성하지 않는다.
- [ ] 선택 인자의 기본값을 builder가 잘못된 nullable 원본으로 캡처하는 피커 형태의 최소 재현을 추가한다.
- [ ] 상속 생성자·factory·제네릭 bridge·반환값·이벤트/콜백의 형식 보정을 각각 검증한다.
- [ ] `Compatibility`/`G53Compatibility`의 옛 호출 문자열 매칭을 새 생성 방식과 맞춘다. 단순 이름 삭제로 기존 보정이 적용되지 않는 상태를 방지한다.
- [ ] 실제로 쓰이는 IR 항목이면 C# 방출을 수정하고, 미사용 항목이면 참조·직렬화 영향 확인 후 제거한다. enum 값 변경으로 다른 계약을 바꾸지 않는다.
- [ ] 실제 Dart analyzer → 변환기 → 생성 C# 컴파일 → 실행 경로로 fixture를 검증한다. [virtual-dispatch 검증](tools/Doroti.DartToCSharp/validation/virtual-dispatch/README.md)의 방식을 참고하되 별도 null 처리 fixture를 추가한다.
- [ ] 생성 후보는 별도 산출물 폴더에 저장한다. 현재 제품 소스를 전체 재생성 결과로 덮어쓰지 않는다.

완료 기준: 대상 fixture의 생성 코드에 `RequireValue` 호출이 없고, 정상값·null·기본값·평가 순서가 기대 계약과 일치한다.

### B1-3 — 제품 코드 모듈별 전환

각 묶음은 해당 의존성 빌드와 관련 동작 검증을 통과한 후 다음으로 진행한다. 실제 프로젝트 의존성에 따라 순서를 조정하되 한 번에 전체 소스를 변경하지 않는다.

| 순서 | 모듈/경로 | 주요 확인 사항 |
|---|---|---|
| 1 | Scheduler·Services·Physics·Animation | 콜백·시간·값 형식·Tween 제네릭·0값 |
| 2 | Gestures·Semantics | 이벤트 데이터·취소·포인터 상태·접근성 값 |
| 3 | Painting·Rendering | Size/Offset·nullable 레이아웃 값·getter 평가·paint 시점 |
| 4 | Widgets | State·Focus·Form·Text·controller 수명·상속 인자·builder 캡처 |
| 5 | Material·Cupertino | 테마 기본값·선택 인자·입력 위젯·다이얼로그·모드 전환 |
| 6 | 앱·호스트·검증의 추가 소비자 | B1-0에서 발견한 범위와 실제 호출 경로 |

- [ ] 각 묶음의 변경 위치와 미분류 잔여를 manifest에 반영한다.
- [ ] 검증된 단순 사례를 적용하고, 기본값·nullable 계약 오류는 원래 API 의미에 맞춰 수정한다.
- [ ] 중첩된 `RequireValue(RequireValue(...))`를 정리하면서 내부 식의 평가 횟수·실패 시점을 유지한다.
- [ ] 묶음별 Debug·Release 빌드와 관련 계약 검증을 수행한다. 경고 증가·새 억제 없이 통과해야 한다.
- [ ] 모듈별 잔여가 0인지 확인한다. 동작 근거가 불명확한 항목은 설명과 함께 남기고 완료로 처리하지 않는다.

완료 기준: 현재 제품·앱의 호출 심볼 0건, 새 대체 helper 0건, 변경 묶음별 관련 검증 통과.

### B1-4 — 런타임 API와 기존 검증 전환

- [ ] 기존 warning-remediation 계약 검증의 직접 `RequireValue` 호출을 새 표현/생성 코드의 의미 검증으로 이관한다. 함수 삭제를 위해 null 실패 검증 자체를 삭제하지 않는다.
- [ ] 제품·앱·변환기·검증·템플릿에 실행 가능한 소비자가 없는지 확인한 후 런타임 오버로드 3개를 삭제한다.
- [ ] API 삭제 안내와 소비자 재빌드 필요성을 기록한다. `RequireReference` 등 범위 밖 API를 함께 삭제하지 않는다.
- [ ] 이전 바이너리·증분 산출물에 가려지지 않도록 격리된 출력 경로에서 관련 프로젝트를 다시 빌드한다.
- [ ] 전체 저장소의 검색 결과를 실행 코드, 생성 문자열, 문서/과거 기록으로 구분해 최종 확인한다. 문서에 이름이 남는 것은 실패로 계산하지 않는다.

완료 기준: 런타임 선언 0건, 실행 코드의 소비자 0건, 새 생성 코드의 호출 0건. API 제거 후 관련 프로젝트가 재빌드된다.

### B1-5 — 통합 동작·화면 검증

- [ ] 아래 검증 행렬을 수행하고, 각 항목의 구성·명령·결과·실패 근거를 남긴다.
- [ ] Testbed의 날짜·시간 피커 회귀를 가로·세로 화면에서 다시 확인한다. 초기값·수정값·오류 메시지·모드 왕복·inputOnly를 포함한다.
- [ ] 변경이 집중된 일반 TextFormField, Focus/controller 교체·dispose, 테마 선택 인자, 레이아웃·애니메이션의 대표 흐름을 검사한다.
- [ ] 가능한 Windows 제품에서 실행·입력·화면을 확인한다. Android 및 다른 플랫폼은 실제 실행 환경이 있는 경우 별도로 확인한다.
- [ ] 소스/자동 계약, Skia 렌더링, OS 입력, 실기기 실행 증거를 분리한다. 실행하지 못한 항목은 `notVerified`로 기록한다.

완료 기준: 자동 검사와 대표 제품 실행 결과가 각 범위에서 확인되고, 플랫폼별 남은 검증이 명시돼 있다.

### B1-6 — 재발 방지와 최종 보고

- [ ] 제품 호출·런타임 선언·변환기 fixture 생성 결과에 대한 0건 검사를 재현 가능한 검증 명령에 포함한다.
- [ ] 의도치 않은 `RequireReference` 증가, 새 동일 역할 helper, 경고 억제 추가를 diff로 검사한다.
- [ ] 최초/최종 집계, 규칙별 변경 수, 수정한 기본값 오류, API 호환성 영향, 검증 결과·미검증 범위를 정리한다.
- [ ] 이 문서의 체크리스트와 상태를 실제 근거에 맞게 갱신한다. 일부 호출·구현·필수 검증이 남으면 `PARTIAL`로 유지한다.

## 5. 검증 행렬

| 범주 | 사례 | 통과 조건 |
|---|---|---|
| 필수값 검사 | nullable 값/참조의 null 및 non-null, int 0, bool false, enum 0 | 정상값 보존, null은 정한 예외 계약으로 실패 |
| 흐름 분석 | 검사 후 참조 사용, 분기 합류, nullable 값 추출, nullable 활성화 | 추가 경고·억제 없이 컴파일 |
| 기본값 | 생략/명시값/명시적 null, 상속된 기본값, factory/base initializer | 원래 API 계약에 맞는 값, false·0 덮어쓰기 없음 |
| 평가 순서 | 카운터 getter·부수효과 함수·중첩 호출·단락 평가·대입식 | 평가 횟수·순서·예외 시점 동일 |
| 지연 실행 | builder·이벤트·async·iterator | 호출 시점과 캡처한 값의 수명 유지 |
| 제네릭 | unconstrained T, class/struct 제약, nullable T, dynamic, overload | 결과 형식·바인딩·null 동작 보존 |
| 변환기 | 실제 analyzer 결과, 생성 C# 컴파일/실행, 기본값 콜백 재현 | 문자열 스캔뿐 아니라 실행 계약 통과 |
| 위젯 | 두 피커, 일반 폼, controller·Focus 수명 | framework exception/ErrorWidget 없음, 입력값·검증 동작 보존 |
| 화면 | 가로·세로 피커 입력 모드와 재전환 | 레이아웃·값 표시·오류 표시 확인 |
| 통합 | 관련 프로젝트 Debug/Release, 런타임 API 삭제 후 격리 빌드 | 참조 누락·새 경고·오류 없음 |
| 플랫폼 | Windows 실행, 가능한 Android/기타 호스트 | 실제 수행한 범위만 통과 기록 |

### 검증 명령과 제한

저장소 규칙에 따라 **각 검증 프로세스에 20분 timeout**을 적용한다. 수백 회 반복 검증을 추가하지 않는다. 대부분의 반복은 30회 이내로 제한하고, 새로운 수정·실패·미해결 우려가 있을 때만 재실행한다.

현재 존재하는 검증의 실행 예시:

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/warning-remediation/Contracts/Contracts.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/picker-input/PickerInput.csproj -c Release
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/picker-input/PickerInput.csproj -c Release --no-build -- --portrait
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release
```

필요한 Debug 구성은 위 명령의 구성을 바꿔 실행한다. 새 변환기 fixture와 자동 수정 도구의 검증 명령은 B1-1/B1-2에서 실제 구현 후 추가한다. `--no-build`는 같은 소스·구성으로 직전에 빌드한 경우에만 사용한다. 환경 제약이나 기존 실패가 있으면 원인을 기록하고 성공으로 대체하지 않는다.

## 6. 산출물과 최종 완료 기준

예정 산출물:

- `Doroti/validation/require-value-removal/`: 타입 기반 조사/수정 도구 및 의미 검증·0건 검사·실행 안내. 위치는 기존 도구 재사용 가능성을 확인한 후 확정한다.
- `tools/Doroti.DartToCSharp/validation/null-semantics/`: 실제 변환기를 실행하는 최소 Dart fixture와 생성 C# 계약 검증.
- `Doroti/artifacts/require-value-removal/<run-id>/`: 호출 manifest, dry-run diff, 빌드/검증 로그, 생성 후보, 화면 및 요약. Git 제외 산출물은 다른 환경에서 없을 수 있으므로 재현 명령을 함께 보존한다.
- `work-b1.md`: 단계별 실제 진행 상태와 최종 결과.

최종 완료에는 다음 조건이 모두 필요하다.

- [ ] 제품·앱·도구·검증의 실행 가능한 `RequireValue` 참조 0건.
- [ ] 런타임 `RequireValue` 선언 0건, 대체 이름으로 옮긴 동일 역할 helper 0건.
- [ ] 변환기의 새 생성 결과에 호출 0건, 관련 호환성 규칙·IR 잔여 처리 완료.
- [ ] 기본값·필수값·nullable 전달·평가 순서·제네릭 계약 검증 통과.
- [ ] 관련 Debug·Release 및 격리 빌드 통과, 새 nullable 경고/억제 없음.
- [ ] 피커와 대표 위젯의 자동 동작·렌더링 검증 통과 및 Windows 제품 실행 확인.
- [ ] API 삭제 영향과 플랫폼별 `notVerified` 항목이 최종 보고에 명시됨.

호출 수 감소, 빌드 성공 또는 함수 이름 변경만으로 전체 작업을 완료 처리하지 않는다. 제거 목표와 동작 보존을 모두 충족해야 한다.
