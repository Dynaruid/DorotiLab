# iOS 스크롤 직후 탭 복구

사용자 증상: DorotiTestbedApp을 iPhone에서 실행하면 스크롤이 멈춘 직후 버튼 탭이 잘 받아들여지지 않음.

## Flutter 비교와 수정

기준은 저장소의 Flutter pin `56b8e1a851a594b1a154f8ea93270807dab22b9a`의 `packages/flutter/lib/src/gestures/velocity_tracker.dart`, `constants.dart`, `monodrag.dart`다. 기본 touch slop은 양쪽 모두 논리 픽셀 18이며 이를 늘리지 않았다.

- Flutter는 마지막 이동 이후 40ms를 초과하면 fling 속도를 0으로 처리한다. Dart `Stopwatch.start(); reset();`은 계속 실행되지만 C# `Start(); Reset();`은 정지한다. Doroti의 generic/iOS tracker를 `Restart()`로 고쳐 멈췄다가 손을 떼었을 때 과거 속도로 불필요한 관성이 시작되지 않도록 했다. 같은 구현을 상속하는 macOS tracker에도 적용된다.
- iOS tracker는 최근 20개 표본에서 음수 상대 인덱스로 속도를 계산한다. Dart `%`는 음수 피연산자를 양수 범위로 감싸지만 C# `%`는 음수가 될 수 있다. 표본이 적거나 버퍼가 순환할 때 발생하던 `ArgumentOutOfRangeException`을 양수 modulo 정규화로 수정했다. 드래그 종료 콜백이 예외로 중단되어 스크롤 상태와 탭 차단이 남는 경로를 제거한다.

공식 의미 비교: [Dart Stopwatch.reset](https://api.dart.dev/dart-core/Stopwatch/reset.html), [Dart modulo](https://api.dart.dev/dart-core/num/operator_modulo.html), [Flutter pin velocity tracker](https://github.com/flutter/flutter/blob/56b8e1a851a594b1a154f8ea93270807dab22b9a/packages/flutter/lib/src/gestures/velocity_tracker.dart).

## 검증

각 실행에 `Doroti/validation/run-with-timeout.py`의 1200초 제한을 적용했다.

수정 전 재현:

1. FCR-5에서 `VelocityTracker: a paused touch must not fling on release` 실패.
2. 타이머 수정 후 iOS 표본 검사에서 `_previousVelocityAt`의 `ArgumentOutOfRangeException` 재현.

수정 후:

- `dotnet run --project Doroti/validation/fcr5-scroll -c Release`: 기본 0/1/2/8/17/18/19/30px 탭 경계, generic/iOS/macOS 정지 후 속도, iOS 표본 1/2/3/19/20/21/22/23/40/41개 검사 PASS.
- `dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --scroll-tap`: iOS 환경·BouncingScrollPhysics의 실제 mounted 위젯에서 평상시 탭, 관성 종료 후 탭, 손가락 정지 후 release와 즉시 탭, 20개 표본 순환, 관성을 멈추는 터치와 다음 탭 PASS. FlutterError도 수집하여 입력 처리 중 예외를 실패로 처리한다.
- `dotnet build DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Debug -r iossimulator-arm64 --nologo`: PASS, 경고 0/오류 0, 4분 4초. 시뮬레이터용 빌드이며 실행 또는 실기기 검증을 의미하지 않는다.

기존 작업 트리에 있던 FCR-5 TouchSlopContracts와 Program 연결을 보존하고, 컴파일용 이벤트 타입 alias 및 binding용 host fixture를 보완했다.

Flutter 원본 실행 비교는 로컬 SDK에 독립 Git checkout 메타데이터가 없어 실행하지 못했다. 위 비교는 pin 소스와 Doroti 자동 테스트에 근거한다. iPhone 실기기에서의 손가락 입력 체감 검증은 별도로 필요하다.

## 실기기 재설치

후속 요청으로 연결된 iPhone 12(VL, iOS 26.6.1)에 수정본을 재설치했다.

- `dotnet build DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Release -r ios-arm64 --nologo`: 경고 0/오류 0, 8분 37초.
- `devicectl device install app`으로 `dev.doroti.testbed` 업데이트 성공.
- `DOROTI_TESTBED_MODE=sample`, `DOROTI_RESIZE_FIXTURE=none`으로 실행 성공, PID 2193의 실행 상태 확인.
- 설치·실행 확인이며 실기기 제스처 체감 검증은 포함하지 않는다.
