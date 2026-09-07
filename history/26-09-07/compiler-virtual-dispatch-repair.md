# Compiler virtual dispatch 수정 검토 및 검증

2026-09-07. 앞선 Framework 675개 소스의 virtual/override 감사 결과를 컴파일러의 생성 규칙으로 반영했다. 사용자 요청에 따라 수정 대상은 `tools/Doroti.DartToCSharp` 및 이 기록이다. 생성된 코드를 제품에 채택하거나 복사하지 않았다.

## 원인과 수정 방식

- **Mixin의 Dart 소유자와 실제 CLR 슬롯 소유자를 혼동했다.** 기반 클래스에 복사된 mixin 메서드/필드도 원래 소유자가 interface이면 `override`를 제거했다. 이제 실제 클래스 상속 경로에서 발견한 슬롯을 재정의한다. 참조 API를 위한 nonvirtual 예외도 생성 중인 가상 클래스에 적용하지 않는다.
- **의도적으로 재정의를 취소하는 예외가 남아 있었다.** `_WidgetStateAnd/Or`, `_SliverResizingHeader`, `itemExtentBuilder`, 2차원 viewport의 `markNeedsLayout` 처리를 바로잡았다. Material/Cupertino selection controls 및 InputBorder에서 출력 문자열의 `override`를 지우던 후처리를 제거했다. item extent callback 보정도 새 override 선언에 맞췄다.
- **상속 계약의 인자 처리가 불완전했다.** 이름 있는 인자는 이름으로, 위치 인자는 위치로 대응한다. 재정렬된 named 인자의 기본값과 본문 이름을 다른 인자에 덮어쓰지 않는다. 기반 클래스가 함께 생성되는 경우 공통 시그니처를 사용하고, 선택 범위 밖의 참조 기반 클래스에는 기존 시그니처를 유지하는 override bridge를 생성해 추가 선택 인자가 있는 구현으로 연결한다. 추상 구현과 nullable generic 메서드도 검사했다.
- **타입 대입이 직계 상속에서 끝났다.** 제네릭 기반 타입을 여러 단계 추적하고 타입 인자는 동시에 대입한다. T→U, U→int를 순차 문자열 치환해서 T까지 int로 바꾸지 않는다. 필드가 getter를 구현할 때도 대입된 기반 프로퍼티 타입을 사용한다.
- **식 본문에서는 covariant 인자 검사가 빠졌다.** 블록 본문처럼 명시적으로 좁혀진 로컬 변수를 만들고 식의 식별자를 연결한다. 위치 인자의 이름이 달라져도 올바른 CLR 인자에서 읽는다. String.length는 resolved Dart symbol에 따라 Length로 변환한다. 잘못된 타입을 전달했을 때 InvalidCastException이 나는 것도 확인했다.
- **getter에 대응하는 setter 슬롯이 없었다.** 파생 클래스에서 setter를 정의하면 기반 getter 본문을 보존하면서 같은 CLR 프로퍼티에 가상 setter 계약을 둔다. 여러 단계의 파생 클래스와 Dart 라이브러리 privacy를 고려한다.
- **Dart와 포트된 CLR API의 진입점이 달랐다.** 선택 인자가 있는 toString에는 Object.ToString bridge를 생성한다. Ui.Color의 generic resolveFrom 진입점도 typed context overload로 연결한다.
- **기반 계약 누락을 성공 처리했다.** 선언에 `@override`가 있지만 상속 계약을 찾지 못하면 `DOTCONV902` 오류를 반환한다. 후보 출력은 검토 목적으로 남을 수 있으나 성공으로 보고하지 않는다. 필요한 기반 라이브러리는 semantic selection에 포함해야 하며 참조 기반은 graph-only를 사용할 수 있다.

비공개 멤버의 같은 철자를 무조건 합치지 않았다. 서로 다른 Dart 라이브러리에서 선언된 private 상태는 별도 CLR 슬롯을 유지하는 테스트가 있다. 컴파일러 수정은 기존 typed lowering/printing 경로에서 수행하며 제품 C#의 일괄 문자열 교체나 재생성을 하지 않았다.

## 검증

모든 테스트 프로세스는 1,200초 timeout을 사용했다.

- 신규 suite는 실제 Dart analyzer → Core IR → C# lowering → Roslyn compile → 기반/인터페이스 호출을 실행한다. 일반/추상 override, mixin getter·메서드·field, 인자 narrowing, renamed positional/named defaults, 외부 기반 bridge, nullable generic, 다단계 generic, property getter/setter, Object 진단 출력, Ui Color, private 상태 독립성 검증 PASS.
- 불완전한 선택은 `DOTCONV902`를 반환하는 negative 검사 PASS. 단일/병렬 실행의 생성 C# 및 source-map.json 바이트 동일성 PASS.
- Dart analyzer protocol 테스트 3개 PASS. `.doroti/compiler-analyzer-test.log`.
- 컴파일러 Release 및 검증 프로젝트 빌드/실행 PASS. `.doroti/compiler-validation-final.log`.
- pinned Flutter 7개 파일에서 상태 조건식 2곳, TextBoundary 구현 3곳, 렌더링 Tween 3곳의 실제 override 생성을 검사한다. `.doroti/compiler-validation-upstream-final.log`, `.doroti/compiler-dispatch-upstream/`.
- 이 upstream 선택은 전체 의존성을 포함하지 않는다. `DOTCONV001` 12개 및 `DOTCONV902` 8개의 진단은 보존하며 전체 후보의 aggregate build PASS로 간주하지 않는다. 예를 들어 Animation 기반 계약이 선택에 없으면 `value`/`toStringDetails`는 명시적으로 검토 필요 상태가 된다. 전체 Flutter import 자격 검증은 이번 작업 범위에 포함하지 않는다.
- 수정 전 generated C#의 CS0546(재정의 가능한 setter 없음), CS1061(object.Count)을 실제로 재현했다. `.doroti/compiler-test-before.err`.
- 중간 검사에서 식 본문의 String.length 처리, 이름을 바꾼 인자 로컬, named 인자 재정렬에 따른 값 오류를 발견하고 수정했다. `.doroti/compiler-test-after1.err`, `compiler-test-after3.err`, `compiler-test-after7.err`. 실패를 최종 PASS로 덮어쓰지 않았다.
- 컴파일러 변경 파일의 whitespace format 및 git diff --check PASS.

## 제품 소스 보호

작업 전후 `Doroti/src`와 `DorotiTestbedApp`의 bin/obj/.doroti를 제외한 **1,111개 파일** SHA-256 비교에서 변경 0개를 확인했다. `.doroti/compiler-dispatch-product-unchanged.json`. 생성 출력은 `.doroti/compiler-dispatch-*`에만 있으며 adopt/promotion/제품 소스 복사를 실행하지 않았다.

재실행:

```powershell
./tools/Doroti.DartToCSharp/validation/virtual-dispatch/validate.ps1 -Upstream
```
