# iPhone 텍스트 선택 메뉴 표시 수정

요청: DorotiTestbedApp의 iPhone TextField에서 텍스트 선택 후 컨텍스트 메뉴가 보이지 않는 현상.

## 원인과 수정

현재 MAUI iOS 호스트는 시스템 컨텍스트 메뉴 지원을 선언하지 않으므로 기본 TextField는 Cupertino 프레임워크 툴바를 사용한다. 메뉴 생성/선택은 이루어지지만 다음 렌더링 문제로 표시가 실패했다.

- `SkiaSceneRenderer.ToPath`에서 `Path.arcTo`가 재생되지 않아 메뉴의 둥근 본체가 클립 경로에서 누락됐다. 수정 전 래스터에는 화살표와 그림자만 남았고 `Cut` 레이블의 픽셀 검사에 실패했다. Skia `ArcTo` 재생과 radians → degrees 변환 및 `forceMoveTo` 전달을 추가했다.
- `Path.shift`가 `arcTo`의 타원 좌표를 이동하지 않았다. `PaintingContext.pushClipPath`가 수행하는 offset 이동에 곡선도 함께 반영하도록 수정했다. 각도·sweep·이동 플래그는 유지한다.
- 메뉴가 선택 아래에 배치되면 시작 사분면이 -1이다. Dart와 C#의 나머지 연산 차이로 모서리 목록에 -1 인덱스를 사용해 `ArgumentOutOfRangeException`이 발생했다. Dart처럼 양수 인덱스로 정규화했다. 첫 두 수정 후 상단 필드에서 이 예외를 별도로 재현했다.

Flutter pin `56b8e1a851a594b1a154f8ea93270807dab22b9a`의 `packages/flutter/lib/src/cupertino/text_selection_toolbar.dart`와 비교했다. 원본 SHA-256과 확인한 anchors를 FCR-7 fixture manifest에 추가했다.

## 자동 검증

모든 실행에 `Doroti/validation/run-with-timeout.py`의 1200초 제한을 적용했다.

- FCR-7 `-c Release -- --ios-text-menu`: PASS. 390px 폭, 밝은/어두운 테마 × 선택 위/아래 메뉴 × 길게 누르기/두 번 탭의 8개 조합. 실제 mounted Material TextField에 touch down/up을 전달하고, 래스터 글자 픽셀, 메뉴 버튼 터치, Copy/Cut/빈 필드 Paste 및 닫기를 확인한다. Framework 오류도 수집하여 실패 처리한다.
- FCR-7 `-c Release` 기본 계약: PASS. 6개 플랫폼 컨텍스트 메뉴를 포함한다.
- FCR-7 `-c Release -- --mac-text-menu`: PASS. 공용 Skia 경로 수정 후 macOS 밝은/어두운 메뉴, 곡선·클리핑·그림자와 Copy/Cut/Paste 동작 회귀 검증.
- `dotnet build DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Debug -r iossimulator-arm64 --nologo`: PASS. 경고 0/오류 0, 2분 29초. 시뮬레이터용 빌드이며 시뮬레이터 실행 또는 실기기 검증을 의미하지 않는다.
- `git diff --check`, 추가 Flutter source hash/anchor 확인: PASS.

자동 생성 이미지: `Doroti/artifacts/ios-input/text-menu-{brightness}-{fieldTop}-{longPress}.png`. 밝은 테마의 위쪽 메뉴와 어두운 테마의 아래쪽 메뉴 이미지를 직접 확인했다. 이미지는 데스크톱에서 실행한 실제 Doroti/Skia 래스터 결과이며, iPhone 스크린샷은 아니다.

iPhone 실기기 재설치·터치 검증은 이번 변경에서 수행하지 않았다.

## 실기기 재설치

후속 요청으로 iPhone 12(VL)에 컨텍스트 메뉴 수정본을 재설치했다.

- Release/ios-arm64 빌드: PASS, 경고 0/오류 0, 8분 46초.
- `devicectl device install app`: `dev.doroti.testbed` 업데이트 성공.
- `DOROTI_TESTBED_MODE=sample`, `DOROTI_RESIZE_FIXTURE=none`으로 실행 성공, PID 2255 실행 상태 확인.
- 설치·실행 확인이며 텍스트 선택 메뉴의 실기기 터치 검증은 포함하지 않는다.
