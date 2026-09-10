# Android x64 Release AOT 복구와 시작 ANR 수정

## 원인과 변경

`DorotiTestbedApp`의 Material 샘플을 시작할 때 `FocusEvent(hasFocus=true)` 처리가 5초를 넘겨 ANR이 발생했다. 기존 로그에서는 실제 처리가 약 8~13초 뒤 끝났으며, 뒤늦게 수집된 ANR 스택은 이미 메인 루프의 유휴 상태를 가리켰다.

`simpleperf`로 시작 구간을 다시 수집했다. DWARF 스택 수집은 1,932개 샘플 중 오류 1개였고, 전체 CPU 샘플의 약 81%가 HPET 시계 읽기, 약 75%가 `mono_100ns_ticks`를 경유했다. x64 에뮬레이터의 clocksource는 `hpet`이었다. Mono의 실행 시점 코드 준비 비용이 시작 메인 스레드에 집중되는 경로였으며, 기존 AOT-off APK는 프로파일러 없이도 시작 검사에서 ANR이 재현됐다.

공유 Runner SDK의 과거 `android-x64` AOT 강제 비활성화를 제거했다. 현재 .NET/Android 런타임에서 AOT 시작 충돌은 재현되지 않았다. stock AOT 프로파일에 Doroti 프레임워크가 포함되지 않는 문제를 피하도록 x64 Release의 `AndroidEnableProfiledAot` 기본값을 `false`로 지정해 사전 컴파일 가능한 메서드를 모두 포함한다.

| 구성 | RunAOTCompilation | AndroidEnableProfiledAot | AndroidAotMode |
|---|---|---|---|
| x64 Release 기본값 | true | false | Normal |
| x64 Debug | false | SDK 기본값 | Interpreter |
| x64 Release + 명시적 AOT off | false | false | None |
| arm64 Release | true | true | 기존 SDK 설정 유지 |

`Normal` Mono AOT는 동적/DLR 코드에 필요한 JIT 경로를 유지한다. Android JNI marshal methods 설정은 기존 `false`를 유지했다. Android의 AOT, 인터프리터와 동적 코드 실행 방식은 [Microsoft의 런타임 설명](https://learn.microsoft.com/en-us/dotnet/maui/deployment/runtimes-compilation?view=net-maui-10.0)을 참고한다.

## 검증

환경: Windows, .NET SDK 10.0.400, Android workload 36.1.69, Mono runtime 10.0.11, `emulator-5554` / `sdk_gphone64_x86_64` / API 33 / 1920×1200 / DPR 1.5.

| 검사 | 결과 |
|---|---|
| 기존 AOT-off APK의 새 프로세스 시작 | FAIL: input-dispatch ANR 및 Android 오류 대화상자 재현 |
| AOT 비교 APK 시작 | 1회 + 연속 3회 PASS |
| AOT 임시 옵션 없이 최종 SDK 기본값으로 Release 빌드 | PASS: 경고 0, 오류 0 |
| 최종 APK 새 프로세스 시작 | 연속 3회, 회당 20초 관찰 PASS: 프로세스 유지, foreground UI, ANR/오류 대화상자 없음 |
| 최종 APK 실제 조작 | 스크롤, 텍스트 입력, 단일 커서 핸들, 길게 눌러 범위 선택, 핸들 드래그 PASS |
| 앞선 커서 중심 수정 | 최종 APK의 `final-caret.png`에서 커서와 핸들 중심 정렬 확인 |
| 비교 AOT APK의 background/foreground | 동일 PID와 입력/선택 상태 복원 확인 |
| 설치한 APK와 빌드한 서명 APK SHA-256 | 일치 |
| Debug/명시적 AOT-off/arm64 설정 | MSBuild 평가 확인; 해당 구성의 이번 기기 실행 검증은 아님 |

최종 APK는 49,818,774 bytes이며 AOT 모듈 128개를 포함한다. SHA-256:

`fdf8687eae0342307d980872c07aa08556237acb94301d53bdf9b19b63c9dc28`

첫 전체 AOT 빌드와 APK 크기가 증가하는 대가로 시작 시 JIT 부담을 줄였다. 이번 결과는 명시된 x64 에뮬레이터의 검증이며, arm64 실기기·TalkBack·전체 IME 조합 또는 모든 플랫폼의 검증으로 확대하지 않는다. Activity의 `TotalTime`은 첫 콘텐츠 완료 시간과 동일하다고 보지 않았다.

## 재현 및 증거

빌드/외부 검사 명령의 제한 시간은 1,200초다. 설치된 앱의 시작 검사는 다음과 같이 실행한다. 이 검사는 앱 데이터나 기존 시스템 로그를 지우지 않는다.

```powershell
python Doroti/validation/app-runner/android-startup-contract.py `
  --serial emulator-5554 --package dev.doroti.testbed `
  --activity crc64c80c495bd333b69c.MainActivity `
  --output artifacts/android-startup-check --rounds 3 --observe-seconds 20
```

원본 증거: [artifacts/android-anr-fix](../../../artifacts/android-anr-fix). `startup-before`, `startup-aot`, `startup-aot-repeat`, `startup-final`에 각 실행의 로그·UI hierarchy·스크린샷을 보존했다. `aot-build.log`, `default-build.log`, `apk.json`, `dwarf-report.txt`, `before-anr.txt` 및 `final-input.png`, `final-caret.png`, `final-selection.png`을 함께 확인한다.
