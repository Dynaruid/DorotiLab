# Android 중간 입력 시 일시적인 커서 위치 오차 수정

2026-10-22. Galaxy에서 기존 텍스트의 중간에 빠르게 입력할 때 커서가 1~2글자 뒤에 잠깐 표시됐다가 돌아오는 문제의 입력 상태 전달 경로를 수정했다.

## 원인과 변경

`MauiTextInputBridge.HandleTextChanged`는 변경 전후 문자열의 공통 접두·접미로 커서 위치를 추정해 즉시 프레임워크에 전달했다. 같은 글자가 이어지면 변경 위치를 문자열만으로 확정할 수 없다. 예를 들어 `aaaa`의 offset 2에서 `a`를 삽입하면 실제 커서는 3인데 추정값은 5다. 이후 선택 변경 알림이 실제 위치를 전달하기 전까지 잘못된 커서 상태가 노출됐다.

Android에서는 동일 텍스트 revision의 네이티브 `EditText.SelectionStart/SelectionEnd`를 읽어 텍스트와 함께 전달하도록 바꿨다. 네이티브 선택이 유효하지 않거나 해당 텍스트와 일치하지 않는 경우와 다른 플랫폼에는 기존 계산을 유지한다. 연속 Backspace가 지연된 UI callback을 굶길 수 있으므로 텍스트의 즉시 전달, 선택 알림의 coalescing, 동일 상태를 네이티브 입력창에 재기록하지 않는 정책을 유지한다.

참고한 플랫폼 구현: [MAUI Android Entry의 텍스트·선택 알림](https://github.com/dotnet/maui/blob/10.0.0/src/Core/src/Handlers/Entry/EntryHandler.Android.cs), [Android BaseInputConnection의 선택·텍스트 교체](https://android.googlesource.com/platform/frameworks/base/+/refs/heads/master/core/java/android/view/inputmethod/BaseInputConnection.java).

수정: [공통 MAUI 입력 브리지](../../../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.cs).
회귀 검사: [Android 입력 프로브](../../../../samples/DorotiTestbedApp/android/TextInputEditingProbe.cs), [실행 진입점](../../../../samples/DorotiTestbedApp/android/MainActivity.cs), [Android aggregate](../../../../Doroti/tests/android_smoke.py).

## 검증 결과

실기기 Galaxy S25 `SM-S931N`, 기기에서 조회한 Android `17`, .NET 10 `net10.0-android` / `android-arm64` Debug / Mono에서 확인했다. 후속 속성 조회에서 `AndroidUseInterpreter=true`를 확인해 초기 기록의 JIT 표기를 정정했다. 테스트·빌드는 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 실행했다. APK에 managed assemblies를 포함하고 앱 데이터 삭제 없이 재설치했다.

| 검사 | 결과 |
| --- | --- |
| 수정 전 네이티브 callback 추적 | 실제 3 / 전달 5, 실제 2 / 전달 4를 재현. 일반·여러 줄 입력의 중간 삽입·삭제에서 일시적 불일치 52개 기록. |
| 수정 후 편집 프로브 | PASS, Entry/Editor 각각 9개 시나리오, 총 18개. 전달된 모든 상태를 네이티브 텍스트·선택과 비교해 불일치 0개. |
| 연속 중간 삽입·삭제 | 각 12회가 같은 UI turn에서 즉시 전달됨. 큐에 남은 선택 알림 이후에도 최신 상태 유지. |
| 한글·선택·유니코드 | 한글 조합 → 조합 갱신 → commit, 선택 범위 교체, UTF-16 emoji 삽입 통과. 동기적인 프레임워크 echo 이후에도 조합 span 유지. |
| 기존 포커스 프로브 | PASS, Entry/Editor 2개. 배경 touch 이후 포커스/조합 유지, commit, native paste, clear 이후 표면 포커스 복귀. |
| 최종 APK 빌드 | PASS, 경고 0 / 오류 0. Python runner 구문 검사와 `git diff --check` 통과. |
| 검사 후 앱 상태 | 프로브 extra 없이 샘플을 다시 실행하고 PID 및 resumed Activity 확인. 수정된 Debug APK 설치 상태. |

수정 전 초기 프로브는 callback 불일치를 기록한 뒤, UI turn 사이에 실제 IME가 조합을 끝내는 영향으로 조합 span 검사에서 종료됐다. 최종 프로브는 synthetic 편집과 조합 검사를 같은 UI turn에서 수행하고 마지막에 지연 선택 알림도 검사한다. 수정 전 전체 18개 시나리오 완료로 해석하지 않는다.

첫 커서 수정 검증의 Debug APK SHA-256: `20c567834da2eb215d5184d943a67b8e98379745c71ae04cca7804906b291dc4`.
원시 로그/JSON은 삭제 가능한 `temp/testing/text-editing/`, APK는 `Doroti/artifacts/te26/`에 둔다. 해당 로컬 산출물과 별개로 위 결과를 이 문서에 보존한다.

## 재실행과 검증 범위

검사 방법은 [테스트 안내](../../../../Doroti/tests/README.md)에 기록했다. 직접 실행 시 `doroti_text_editing_probe=1` intent extra가 `text-editing-probe.json`을 생성한다. 기존 `android_smoke.py --cases input`도 포커스와 편집 프로브를 모두 실행한다.

이 결과는 실기기 네이티브 InputConnection 자동 명령의 상태 전달 검증이다. Samsung 키보드 UI에서 사람이 빠르게 타이핑할 때의 화면, 물리 터치, 화면 표시 타이밍, Release/AOT, 다른 기기·IME·플랫폼의 동작은 이번 실행으로 검증하지 않았다.

## 후속 입력 성능 확인

커서 동작 개선 이후 입력이 조금 버벅인다는 보고를 받았다. 직전 설치본은 검증용 Debug이며, 실제 MSBuild 속성은 `AndroidUseInterpreter=true`, `Optimize=false`, `DebugSymbols=true`, `RunAOTCompilation=false`였다. 이 실행 조건은 체감 지연을 키울 수 있지만, 사람의 타이핑 화면을 측정하지 않았으므로 원인을 Debug 하나로 확정하지 않는다. [Android 공식 성능 검사 안내](https://developer.android.com/topic/performance/appstartup/setup-env)는 성능 비교에 Release 사용을 권장한다.

커서 확인에 추가했던 `native.Text`는 Java 텍스트 전체를 managed 문자열로 다시 복사했다. 이를 MAUI가 이미 받은 `input.Text`의 일치 검사와 네이티브 `EditableText.Length()` 검사로 바꿨다. 네이티브 선택 위치 조회, 유효 범위 검사, fallback 및 동기적인 텍스트 전달 정책은 유지한다. 추가 로깅이나 계측은 opt-in 프로브에만 있다.

동일 Galaxy의 Debug 프로브에서 각 경로 2회 준비 후 12회 읽기를 비교했다. 아래 값은 조회 구간의 중앙값과 현재 스레드의 managed 할당량이며, 입력부터 화면 표시까지의 지연 측정이 아니다.

| UTF-16 길이 | 전체 문자열 읽기 | managed 값 + 버퍼 길이 | 읽기당 문자열 경로 할당 → 개선 경로 할당 |
| --- | --- | --- | --- |
| 32 | 11.0 μs | 7.3 μs | 88 → 0 bytes |
| 8192 | 38.6 μs | 17.9 μs | 16408 → 0 bytes |

개선된 Debug의 편집 18개 시나리오는 PASS, 전달 위치 불일치 0개였다. 이 작은 조회 비용의 감소를 전체 앱의 버벅임 해결이나 FPS 개선으로 해석하지 않는다.

### Release 설치 및 재검증

기본 Release 프로필로 다시 빌드해 같은 기기에 앱 데이터를 유지하며 설치했다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -p:AndroidPackageFormats=apk -p:EmbedAssembliesIntoApk=true -p:ArtifactsPath=C:/Users/parti/Labo/DorotiLab/Doroti/artifacts/te26 --nologo
```

- MSBuild: `AndroidUseInterpreter=False`, `Optimize=true`, `DebugSymbols=false`, `RunAOTCompilation=true`, `AndroidEnableProfiledAot=true`, `PublishTrimmed=true`, `UseMonoRuntime=true`.
- 빌드 PASS, 오류 0. Gradle의 SDK XML 도구 버전 경고 1개는 앞선 최초 Debug 빌드와 동일한 경고다.
- APK에 ARM64 Mono AOT 이미지 139개와 `Doroti.Host.Maui` AOT 이미지를 확인했다. .NET NativeAOT 프로필로 바꾼 결과가 아니다.
- 같은 편집 프로브 18개 PASS, 전달 위치 불일치 0개. 한글 조합 span과 즉시 텍스트 전달 유지.
- 기존 포커스·붙여넣기 프로브 Entry/Editor 2개 PASS.
- Release 조회 구간의 중앙값: 길이 32에서 전체 읽기 3.4 μs → 개선 경로 2.0 μs, 길이 8192에서 35.0 μs → 2.6 μs. 개선 경로의 managed 할당은 읽기당 0 bytes. 각각 2회 준비 및 12회 측정.
- Python runner 구문 검사와 `git diff --check` 통과. 검사 뒤 extra 없이 Material 샘플을 다시 열고 PID와 resumed Activity를 확인했다.

현재 설치된 Release APK SHA-256: `98a7f211f69917a31d0ecefdbdcba76b05cbd4e51d91364f0b343eee9a25477b`.
로컬 증거는 `temp/testing/text-editing/perf-debug-final/`, `perf-release/`, `perf-release-focus/`, `perf-*-profile.json`과 `perf-*-build.log`에 있다. 현재 APK는 `Doroti/artifacts/te26/bin/DorotiTestbedApp.Android/release_android-arm64/`에 있다.

이 재검증으로 Release/AOT에서 입력 상태 전달을 확인했다. 실제 Samsung 키보드 타이핑의 체감 버벅임, 입력부터 화면 표시까지의 지연과 프레임 페이싱은 별도 확인이 남아 있다.

## iOS / Cupertino 영향 검토

`CupertinoTextField`도 공통 `EditableText`와 플랫폼 입력 호스트를 사용한다. 따라서 Android의 Cupertino 입력에도 이번 네이티브 선택 조회 수정이 적용된다. iOS 전용 위젯이라는 뜻으로 Cupertino를 해석하지 않는다.

이번 실제 선택 조회와 문자열 복사 제거는 `#if ANDROID` 분기에 있다. iOS의 `ReadSelectionAfterTextChange`는 여전히 `ResolveSelectionAfterTextChange`의 문자열 비교 fallback을 사용한다. 같은 문자가 이어지는 중간 편집의 모호성은 iOS에도 남아 있으므로, iOS에서 동일 증상이 없다고 결론 내리지 않는다. 이 위험은 iOS의 Material/Cupertino 양쪽에 해당한다. Android 전용 최적화가 iOS 입력 경로에 추가 비용을 넣는 변경은 아니다.

다음 CPU 커서 회귀를 1200초 제한으로 실행해 PASS했다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project packages/Doroti.Cupertino/tests/Doroti.Cupertino.Tests -c Release -- --text-input-tap-cursor
```

이 검사는 iOS/Android/Windows/macOS/Linux 환경 설정의 Cupertino에서 첫 focus/refocus, 여러 줄·빈 줄·soft wrap, Unicode, 칠해진 커서 위치와 후속 synthetic IME 삽입을 확인한다. UIKit의 실제 텍스트/선택 callback 순서나 iPhone 입력 성능은 확인하지 않는다. 기존 Apple feature 프로브는 선택을 유지한 설정 전환과 Return 동작을 검사하며, 이번 Android 프로브와 같은 빠른 중간 입력의 모든 callback 비교를 대체하지 않는다. 이 검토에서는 UIKit 코드를 변경하거나 Apple 기기에 배포하지 않았다.
