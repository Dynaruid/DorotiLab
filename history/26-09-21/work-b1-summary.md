# work-b1.md — RequireValue 제거 및 C# null 처리 정리 요약

정리일: 2026-09-21. 같은 날 완료된 `work-b1.md`의 계획과 실행 결과를 요약해
보관하고 저장소 루트의 원문은 삭제했다. 이번 작업은 문서 보관이며 빌드, 분석기,
제품 실행 또는 실기기 검증을 새로 수행하지 않았다. 아래 PASS와 `notVerified`는
삭제 전 원문에 기록된 당시 증거 범위다.

**`Doroti.Runtime.DartRuntimePrimitives.RequireValue`의 제품·변환기 의존성을
제거하고 공개 오버로드 3개를 삭제했다. 최종 제품 호출 문자열·Roslyn 호출 심볼·
런타임 선언·변환기 생성 의존성은 모두 0건이며, 명시적인 C# null 처리로 정상값,
기본값, 실패 시점, 단일 평가와 지연 콜백 동작을 보존했다.**

## 기준선과 목표

- 구현 기준선은 commit `4abb4e45`, clean worktree, .NET SDK `10.0.400`이었다.
- 최초 텍스트 기준선은 `Doroti/src`의 **285개 파일 / 3,618회**였다. 모듈별로
  Material 1,105회, Widgets 921회, Rendering 802회가 가장 큰 비중을 차지했다.
- Roslyn 프로젝트 그래프의 최초 binding 기록은 3,612행이었다. 이미 non-null인
  값 1,409행, nullable 값 2,094행, 참조 109행이며 여러 프로젝트가 공유하는
  파일의 중복 binding을 포함한다.
- 목표는 함수 이름만 없애는 것이 아니라 nullable 값의 0·false·enum 0, 참조 null,
  선택 인자 기본값, 제네릭·dynamic, 단락 평가, 콜백·async·iterator의 실행 시점과
  예외 계약을 유지하는 것이었다.
- `!`, `.Value`, `GetValueOrDefault()`, 새 helper, `RequireReference` 전가, compiler
  설정 완화나 경고 억제 추가는 대체 수단으로 인정하지 않았다.

## 구현 결과

- nullable 값과 참조는 피연산자를 한 번만 평가하는 `?? throw` 또는 명시적
  분기로 바꿨다. 이미 non-null인 값은 직접 식으로 전환하고, 중첩된 필드 대입
  두 곳은 지역 흐름으로 풀어 결과 형식과 평가 순서를 보존했다.
- 일반 프로젝트에서 제외된 플랫폼 원본 5개 파일은 소유 프로젝트 compilation의
  metadata reference로 분석해 55행을 전환했다. 제거된 `engineId` API 때문에
  바인딩되지 않는 13건은 `excluded-stale-engine-id`로 따로 기록하고 명시적 null
  실패식으로 바꿨다.
- 변환기는 Dart postfix `!`와 기존 compatibility 생성 경로를 내부 placeholder로
  통합한 뒤 최종 C#에 단일 평가 switch/property pattern과 같은
  `NullReferenceException` 계약을 출력한다. placeholder와 제거 API 이름은 최종
  생성물에 남지 않는다.
- 미사용 `RuntimeIntrinsic.RequireValue`를 삭제하고 다음 enum 항목을 `1`부터
  명시해 기존 numeric identity를 보존했다.
- `RequireValue` 공개 오버로드 3개를 compatibility shim 없이 삭제했다. 이는
  **소스·바이너리 호환성 변경**이므로 외부 소비자는 명시적인 C# null 처리로
  전환한 뒤 새 Doroti runtime에 맞춰 다시 빌드해야 한다.
- 범위 밖 `RequireReference` 27건은 기준선 그대로 유지했다. 새 `NoWarn` 또는
  warning pragma는 추가하지 않았다.

## 오류 문구 후속 정리

- 제품 예외의 Dart 중심 표현을 Doroti/C# 관점의 중립적인 문구로 바꿨다.
  대표 문구는 `A required value was null.`, `Control flow completed without
  returning a value.`, `Callback completed without returning a value.`,
  `Switch expression did not handle the supplied value.`이다.
- runtime의 type/assert/index/JSON/length/map/covariance 오류와 Future 진단 이름도
  같은 원칙으로 정리했다. 실제 원인 식별에 필요한 Dart analyzer 입력,
  `dart:ui` invocation ID, CLR 형식 이름은 유지했다.
- 제품 소스, 변환기 emission·compatibility 문자열과 계약 검증을 함께 수정했다.
  폐기 문구 재유입 검사는 당시 실행 가능한 C# 소스 1,006개에서 통과했다.

## 당시 최종 검증

| 항목 | 기록된 결과 |
| --- | --- |
| 변경 전 기준선 | warning-remediation 95 assertions PASS, picker-input 가로 PASS |
| 제거 0건 검사 | `require-value-removal verify` PASS, final manifest 0행 |
| 프레임워크 통합 빌드 | `Doroti.Editor.slnx` Debug/Release 경고 0·오류 0 |
| 격리 빌드 | Material 의존성 전체 Release 경고 0·오류 0 |
| runtime 계약 | Debug/Release 각각 103 assertions PASS |
| 변환기 null semantics | analyzer → 변환기 → C# compile → 실행 fixture PASS |
| 변환기 virtual dispatch | 외부 bridge, factory, generic, serial/parallel 결정성 PASS |
| 날짜·시간 피커 | 가로·세로, 초기값·오류·수정값·모드 왕복·inputOnly·Skia paint PASS |
| 화면 확인 | PNG 4장에 framework exception, ErrorWidget, clipping 없음 |
| Windows 제품 | Windows App SDK Release 및 OS SendInput 입력 흐름 PASS |
| Android | `android-arm64` Release build PASS |
| error-message 재발 검사 | 실행 C# 소스 1,006개 PASS |

계약 검사는 0, false, 중립화한 null/type 오류 문구, 피연산자 단일 평가와 지연
콜백 실행을 포함했다. Windows 자동 입력은 스크롤, 클릭, 포커스, native editing,
화면 밖 이동과 remount 이후 입력을 확인했다.

## 보존할 경계

- 저장소 통합 wrapper는 warning guard와 guard 테스트를 통과한 뒤 Testbed의 기존
  IDE0002 10건(6개 파일)에서 조기 중단했다. 이는 B1 변경 파일 밖의 기존 진단으로
  기록됐으며, wrapper 전체 PASS로 표현하지 않는다. B1 관련 빌드·계약·Windows
  제품 입력은 이후 별도 검증 결과를 따른다.
- Android는 연결된 ADB 기기가 없어 설치, 실행, 터치와 실제 화면이
  `notVerified`다. Release build만 통과했다.
- iOS와 macOS 실기기 실행 및 Linux 실제 host 실행은 Windows 작업 환경에서
  `notVerified`다.
- 피커 PNG와 자동 입력은 해당 대표 흐름의 증거다. 모든 위젯·화면의 시각적
  완전성이나 사람의 물리 입력, IME, 접근성, 물리 scan-out을 인증하지 않는다.
- `.doroti`와 `Doroti/artifacts/require-value-removal`의 생성 후보·로그는 제품
  소스를 덮어쓰지 않도록 분리됐다. `artifacts`는 Git 제외 경로일 수 있으므로
  다른 checkout에 존재한다는 보장은 없다.

## 유지된 재현 자료

- [RequireValue 제거 도구와 API 호환성 안내](../../Doroti/validation/require-value-removal/README.md)
- [현재 최종 0건 manifest](../../Doroti/artifacts/require-value-removal/current/final-manifest.json)
- [변환기 null-semantics fixture](../../tools/Doroti.DartToCSharp/validation/null-semantics/README.md)
- [오류 문구 재발 검사](../../Doroti/validation/error-messages/README.md)
- [피커 회귀 검증](../../Doroti/validation/picker-input/README.md)
- [warning-remediation 계약 검증](../../Doroti/validation/warning-remediation/README.md)

문서 보관이나 이후 소스 변경만으로 위 과거 PASS를 현재 checkout의 새 검증으로
간주하지 않는다. 재검증할 때는 저장소 지침에 따라 각 명령을 20분 process-tree
제한 아래 실행해야 한다.
