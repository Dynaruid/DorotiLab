# iPhone 텍스트 선택 메뉴 스타일 및 Flutter 비교

요청: iPhone에서 DorotiTestbedApp 텍스트 선택 메뉴 디자인이 이상한 문제 수정, Flutter 코드 대조.

## 원인

Cupertino 모바일 메뉴를 선택하는 분기는 정상이다. 그러나 모바일 구현이 이름이 같은 데스크톱 라이브러리의 private 상수를 참조했다. 모바일 파일에 올바른 상수가 선언되어 있었지만 사용되지 않았다. 이전 표시 수정의 래스터 검증은 글자가 보이는지만 검사하여 스타일 오류를 잡지 못했다.

## Flutter 코드 비교

저장소 reference와 FCR-7 manifest의 Flutter pin `56b8e1a851a594b1a154f8ea93270807dab22b9a`를 기준으로 비교했다. `text_selection_toolbar.dart`의 SHA-256은 manifest와 일치한다. 버튼 파일의 SHA-256과 스타일 관련 source anchor를 manifest에 추가했다.

| 항목 | Flutter 모바일 원본 | 수정 전 Doroti | 수정 |
| --- | --- | --- | --- |
| 버튼 글꼴 | 15, w400, 자간 -0.15 | 데스크톱 14, w400, 자간 -0.15 | 모바일 파일의 글꼴 상수 사용 |
| 버튼 여백 | 좌우 16, 상하 18 | 좌우 8, 위 2, 아래 5 | 모바일 파일의 여백 상수 사용 |
| 메뉴 배경 | 밝음 `#FFF6F6F6`, 어두움 `#FF222222` | 데스크톱 반투명 배경 | 모바일 파일의 불투명 배경 사용 |
| 모서리 반경 | 모바일 파일의 8 | 데스크톱 파일의 8 | 같은 값이지만 모바일 소유 상수로 연결 |
| 화살표 화면 여백 | toolbar 파일의 26 | 구형 selection 파일의 26 | 같은 값이지만 toolbar 소유 상수로 연결 |
| 버튼 글자/Live Text 색 | 버튼 파일의 동적 흑/백 | toolbar 파일의 동적 흑/백 | 버튼 소유 상수로 연결 |
| 크기 애니메이션 완료 콜백 | nullable `onEnd`를 그대로 전달 | null도 호출하는 람다로 감쌈 | State → render widget → RenderAnimatedSize 전달 모두 원본처럼 수정 |

관련 원본:

- `reference/flutter-master/packages/flutter/lib/src/cupertino/text_selection_toolbar_button.dart`: 스타일 선언, `build`, `_getContentWidget`.
- `reference/flutter-master/packages/flutter/lib/src/cupertino/text_selection_toolbar.dart`: `_defaultToolbarBuilder`, `build`, `_RenderCupertinoTextSelectionToolbarShape`, `_RenderCupertinoTextSelectionToolbarItems.performLayout`.
- `reference/flutter-master/packages/flutter/lib/src/cupertino/adaptive_text_selection_toolbar.dart`: 모바일/데스크톱 위젯 선택 분기.
- `reference/flutter-master/packages/flutter/lib/src/widgets/animated_size.dart`: `onEnd: widget.onEnd`, `onEnd: onEnd` 전달. 이 파일의 source hash/anchors도 manifest에 추가했다.

화살표 크기 14×7, 화면 여백 8, 선택 영역과의 간격 8, 구분선 색/두께, 메뉴 페이지 나누기 및 앞뒤 버튼 배치는 비교한 원본과 같은 로직이다. 기존 `arcTo` 렌더링 수정과 음수 나머지 정규화는 유지한다. 최신 UIKit 시스템 메뉴를 새로 구현한 것은 아니다.

## 검증

기존 `--ios-text-menu`에 실제 mounted 버튼의 15px 글꼴·좌우/상하 여백, 밝고 어두운 배경의 래스터 픽셀, 390px 폭의 넘침 메뉴 다음/이전 페이지 터치 검증을 추가했다. 길게 누르기/두 번 탭, 선택 위/아래 위치, Copy/Cut/빈 필드 Paste 및 닫기 검증도 유지한다.

모든 실행은 `Doroti/validation/run-with-timeout.py`의 1200초 제한을 적용했다.

- `dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --ios-text-menu`: PASS. 밝음/어두움 × 선택 위/아래 × 두 번 탭/길게 누르기 8개 조합. 다크 모드 4개 조합도 별도로 모두 통과했다.
- 같은 프로젝트 `-c Release --no-build` 기본 계약: PASS. 6개 플랫폼 컨텍스트 메뉴, disabled callback, overlay theme 포함.
- 같은 프로젝트 `-c Release --no-build -- --mac-text-menu`: PASS. macOS 밝음/어두움 메뉴 렌더링, Copy/Cut/Paste 및 닫기.
- `dotnet build DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -c Debug -r iossimulator-arm64 --nologo`: PASS. 경고 0/오류 0, 3분 12초. 시뮬레이터용 빌드이며 실행 검증은 아니다.
- 변경 관련 Flutter source SHA-256/anchors, `git diff --check`: PASS.

이미지: `Doroti/artifacts/ios-input/text-menu-{brightness}-{fieldTop}-{longPress}.png`, 같은 이름의 `-overflow.png`. 밝은 메뉴와 다크 모드의 아래쪽 메뉴·overflow 페이지를 직접 확인했다. 다크 배경의 RGB(34,34,34)는 자동 픽셀 검증도 통과했다. 수정 전 이미지 사본은 `Doroti/artifacts/ios-input/style-before/`, 로그는 `Doroti/artifacts/ios-input/style-validation/`에 있다.

이미지는 데스크톱에서 Doroti/Skia로 그린 결과다. 이번 변경에서 iPhone 실기기 재설치와 터치 검증은 수행하지 않았다.

수정 전 새 검증은 `Cut` 글꼴이 14인 문제로 실패했다. 스타일 수정 뒤에는 overflow 페이지 터치 후 `AnimatedSize`의 null 완료 콜백에서 `NullReferenceException`을 재현했다. 메뉴 너비가 바뀌는 애니메이션을 새 테스트가 실행하면서 드러난 문제로, 메뉴에서 임시 콜백을 넣는 대신 공용 위젯의 전달 동작을 Flutter 원본에 맞췄다.

## 라이트 모드 추가 확인

사용자의 생성 이미지 피드백 후, 로컬 Flutter SDK `6b182d2c7585eba26d4edce0f97630effd256c33`로 밝음/어두움 Cupertino 툴바를 실제 렌더링했다. 이 SDK의 모바일 toolbar와 button 소스 SHA-256은 위 reference 파일과 동일하다.

Flutter 테스트 전용 Ahem 폰트로는 실제 여백을 비교할 수 없어 Doroti 데스크톱 검증의 기본 폰트인 Helvetica를 명시적으로 로드하고, 같은 15px/w400/-0.15 스타일의 Text를 툴바 버튼 자식으로 주었다. 390px 화면에서 첫 페이지의 Cut/Copy/Paste/Look Up 버튼 너비는 각각 약 54.9/66.4/69.6/86.8, 글자 높이는 15, 버튼 높이는 51로 Doroti와 일치했다. 따라서 화살표 부분을 제외한 본체에서 보이는 수직 여백은 각 11이다(18에서 화살표 공간 7을 제외). 그림자까지 버튼 여백으로 계산하지 않는다. Flutter 캡처는 직접 지정한 anchor를 사용하므로 실제 TextField 선택 위치와의 절대 좌표 비교는 아니다.

실행: oracle 프로젝트에서 `/Users/ceramic/flutter/bin/flutter test test/menu_test.dart --reporter expanded`: PASS. 캡처 `Doroti/artifacts/ios-input/flutter-light.png`, `flutter-dark.png`; 재현용 테스트와 pubspec 및 로그는 `style-validation/flutter-oracle/`, `style-validation/doroti-flutter-menu.log`에 보관했다. Arial로 먼저 렌더링한 이미지의 17px 글자 높이는 폰트 차이였고 제품 코드를 바꾸는 근거로 삼지 않았다. 전체 화면의 자동 픽셀 일치 판정은 수행하지 않았다.

사용자가 후속 생성 이미지에서 정상적으로 보인다고 확인하여 추가 스타일 조정 없이 마무리했다.
