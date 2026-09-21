# work-a1.md — 컴파일러 경고 원인 수정 및 IDE0002 정리 요약

정리일: 2026-09-21. 2026-09-16에 완료된 `work-a1.md`의 계획과 실행 결과를
요약해 보관하고 저장소 루트의 원문은 삭제했다. 이번 정리는 문서 보관 작업이며
빌드, 분석기, 제품 실행 또는 실기기 검증을 새로 수행하지 않았다. 아래 결과는
삭제 전 원문에 기록된 당시 증거 범위를 따른다.

**프레임워크 410개 파일에 남아 있던 대상 경고 pragma 1,406개 파일·코드 조합을
모두 제거했고, 지정한 프로젝트 범위의 IDE0002도 0개로 정리했다. 컴파일러 설정을
완화하거나 다른 억제로 옮기지 않았으며 계약 검사, 대표 Windows/Web 제품 흐름,
Debug·Release 빌드와 재발 방지 검사를 통과한 것으로 기록됐다.**

## 목표와 기준선

- `Doroti/src/Doroti.Framework.*`의 파일 단위 경고 억제를 실제 nullable, 수명,
  상속, 비동기 완료·오류 전달, 동등성 계약 수정으로 제거하는 작업이었다.
- 시작 기준은 410개 파일, 1,406개 파일·경고 코드 조합이었다. 격리한 Debug와
  Release 진단에서는 구성별 9,923개 고유 진단을 수집했다. 과거에 기록된
  10,764건은 후속 정리 전 수치이므로 최종 기준선으로 재사용하지 않았다.
- 전역 `NoWarn`, severity 완화, `#nullable disable`, 무근거 `!`·`default!`, 빈
  기본값 반환으로 숫자만 줄이는 방식은 완료 조건에서 제외했다.
- 기준 HEAD는 `6a291c8decec559731beb915cbf899818b84ff5c`였다.

## 구현 결과

- Scheduler, Services, Gestures, Painting, Semantics, Rendering, Widgets,
  Cupertino, Material 순으로 선언과 호출자 계약을 맞추고 대상 pragma를
  **410개 파일 / 1,406개 조합에서 0**으로 줄였다.
- 제네릭 형식 매개변수 충돌 18곳, Semantics·Inspector의 누락 연결과 개발자
  메타데이터 전달, nullable 메시지와 codec, route 복원, 기본 theme 및 상태별
  style·color·border 계약을 정리했다.
- `TickerFuture`가 실제 완료 Task를 공유하도록 하고 timeout의 원본 오류와 기한
  만료를 구분했다. 비동기 작업을 단순 폐기하지 않고 완료, 취소, 오류 관측의
  소유권을 보존했다.
- 상태 factory와 restoration, identity/value equality 및 hash 계약을 수정했다.
  상태 map adapter는 지연 해석하며 미해석 값 접근을 명시적으로 실패시키고,
  readonly 상태 속성의 공변성을 지원하도록 정리했다.
- SDK Roslyn의 멤버 접근 판정과 code fix를 사용해 공통 프레임워크, 지원 host,
  도구와 Testbed의 27개 프로젝트에서 **561개 문서 / 34,747곳**의 IDE0002를
  수정했다. 같은 범위의 재검사 결과는 0개였다.
- 변환기의 실제 factory bridge와 lowercase identifier 패턴을 fixture로 검증하고,
  제품 소스에 포괄 pragma나 완화된 compiler 설정이 다시 들어오는 것을 막는
  strict guard와 guard 자체 테스트 3개를 추가했다.

## 당시 최종 검증

| 검증 | 기록된 결과 |
| --- | --- |
| TestbedApp Debug / Release | 각각 경고 0 / 오류 0 |
| WidgetPreviews Debug / Release | 각각 경고 0 / 오류 0 |
| 계약 fixture Debug / Release | 각각 84 assertions 통과 |
| 제품 경고 재발 guard / guard 테스트 | 대상 pragma 0 / 3개 테스트 통과 |
| SDK Roslyn IDE0002 재검사 | 지정 범위 0개 |
| 변환기 virtual-dispatch + nullable fixture | 통과 |
| Windows App SDK Release | 경고 0 / 오류 0 |
| Windows OS 자동 입력 | 스크롤, 버튼, 포커스, 텍스트 입력·blur, 화면 밖 이동·복귀·remount 통과 |
| Web Release / Qt host Release | 각각 경고 0 / 오류 0 |
| Web 제품 자동 입력 | disabled, navigation, checkbox/switch, picker 취소, 텍스트 입력, 연결 해제·remount 통과 |
| `git diff --check` | 통과 |

Windows 검증은 현재 WinUI island에 맞는 포커스·텍스트 gate를 사용했다. Web은
DOM adapter 모형이 아니라 빌드한 제품을 기본 `worker-direct-webgpu` 모드에서
Chrome CDP 입력으로 확인한 결과였다. 마지막 소스 변경은
`material_state.cs`의 EOF 빈 줄 제거였고, 최종 diff 검사도 다시 통과했다.

## 보존할 경계

- Material/Cupertino 프로젝트의 기존 `NoWarn` 3종인 `CS0219`, `CS8524`,
  `CS8846`은 계획의 제외 항목대로 유지했다.
- IDE0002 검증은 기록된 27개 프로젝트 범위다. 생성 파일과 지원하지 않는
  플랫폼 프로젝트를 전체 분석한 결과로 확대하지 않는다.
- 변환기에서 수정한 패턴은 검증했지만 legacy blanket pragma 출력 자체를
  제거하거나 전체 프레임워크 재생성을 무경고로 인증하지는 않았다.
- CLR 형식 소거 때문에 `WidgetStateMapper<T>`가 nullable 참조 `T`와 required
  참조 `T`를 일반적으로 구분하지 못하는 기존 한계는 남는다. 구체적인 required
  factory가 unmatched state를 거부하는 방식으로 계약을 지켰다.
- Windows OS 입력과 Web CDP 입력은 대표 자동화 흐름의 증거이며 모든
  Cupertino/Material 화면의 시각적 완전성, 사람의 물리 입력, 한국어 IME,
  물리 scan-out을 인증하지 않는다.
- Android, iOS, macOS, Linux 실제 제품 실행은 이 작업에서 `notVerified`였다.
- 원문이 가리킨 Git 제외 산출물
  `Doroti/artifacts/warning-remediation-a1/20260916T094602Z/`는 현재 checkout에
  존재하지 않는다. 따라서 위 수치는 삭제 전 문서의 당시 기록이며 이번 보관
  과정에서 산출물 원본을 재확인한 결과가 아니다.

## 유지된 재현 자료

- [검증 절차와 계약 범위](../../Doroti/validation/warning-remediation/README.md)
- [IDE0002 검증 프로젝트 목록](../../Doroti/validation/warning-remediation/validated-projects.json)
- [경고 재발 guard](../../Doroti/validation/warning-remediation/guard.py)
- [통합 검증 진입점](../../Doroti/validation/warning-remediation/verify.py)

검증 명령은 저장소 지침대로
`python Doroti/validation/run-with-timeout.py ...`를 사용해 각 프로세스 트리에
20분 제한을 적용한다. 문서 보관이나 이후 소스 변경만으로 위의 과거 PASS를
현재 checkout의 새 검증 결과로 간주하지 않는다.
