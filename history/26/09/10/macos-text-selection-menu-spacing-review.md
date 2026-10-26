# macOS 텍스트 선택 메뉴 여백 검토

요청: Mac 타깃의 컨텍스트 메뉴 여백도 좁게 보이는 문제 검토.

## 확인 결과

아이폰 메뉴에서 발견한 다른 플랫폼 상수 참조 문제는 macOS 메뉴에는 없다. 기본 메뉴 및 버튼이 자체 데스크톱 상수를 사용하며, Flutter 원본과 여백 및 글꼴 값이 일치한다.

| 항목 | Flutter | Doroti |
| --- | --- | --- |
| 메뉴 너비 | 222 | 222 |
| 메뉴 안쪽 여백 | 사방 6 | 사방 6 |
| 버튼 여백 | 좌우 8, 위 2, 아래 5 | 좌우 8, 위 2, 아래 5 |
| 글꼴 | 14, w400, 자간 -0.15 | 동일 |
| 버튼 최소 높이 | 0, 글자와 여백으로 결정 | 동일 |
| Helvetica 글자 높이 / 행 높이 | 14 / 21 | 14 / 21 |
| 실제 버튼 너비 | 210 | 210 |

즉 메뉴 테두리에서 글자까지 왼쪽 여백은 14이며, 행 높이 21은 Flutter의 조밀한 데스크톱 메뉴 설계다. 코드에서 여백이 유실되거나 잘못 축소된 결과는 아니다. Flutter 원본 주석은 이 값을 macOS 13.2의 메뉴 화면에서 측정한 것으로 설명한다. 최신 macOS 네이티브 메뉴와의 동일성을 의미하지는 않는다.

## 비교 및 검증

- 저장소 Flutter reference pin `56b8e1a851a594b1a154f8ea93270807dab22b9a`의 `cupertino/desktop_text_selection_toolbar.dart`, `desktop_text_selection_toolbar_button.dart`를 대조했다. 두 파일의 SHA-256 및 관련 source anchors를 FCR-7 fixture manifest에 추가했다.
- 로컬 Flutter SDK `6b182d2c7585eba26d4edce0f97630effd256c33`의 해당 두 파일은 reference와 SHA-256이 같다. 이 SDK로 800×900 화면의 밝음/어두움 메뉴를 실제 렌더링했다. Flutter 테스트용 Ahem 폰트 대신 Doroti 데스크톱 검증의 기본 Helvetica를 명시적으로 로드한 동일 스타일 Text 자식을 사용했다.
- Flutter와 Doroti의 Cut/Copy/Paste 글자 너비는 각각 약 21.3/32.1/35.0, 행 높이는 21, 텍스트의 버튼 내부 좌표는 (8,2)로 일치했다. Flutter oracle의 직접 지정 anchor와 실제 TextField의 anchor는 세로 1 차이가 있어 절대 좌표를 동일하다고 판정하지 않았다.
- `python3 Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/fcr7-material-widget -c Release -- --mac-text-menu`: PASS. 밝음/어두움 실제 텍스트·버튼 크기와 여백, hover 둥근 모서리·대비색, pressed opacity, Copy/Cut/Paste 및 닫기 검증.
- oracle 프로젝트에서 `/Users/ceramic/flutter/bin/flutter test test/mac_menu_test.dart --reporter expanded`: PASS. 이 실행에도 `run-with-timeout.py` 제한을 적용했다.

증거: `Doroti/artifacts/mac-input/text-menu-{brightness}.png`, `flutter-{brightness}.png`; 로그와 재현용 oracle 소스·pubspec은 `Doroti/artifacts/mac-input/spacing-validation/`에 보관했다. Flutter와 Doroti 라이트 메뉴를 직접 확인했다. 전체 이미지의 자동 픽셀 일치 검증이나 Mac 네이티브 앱 재실행 검증은 아니다.

사용자가 “원본 여백 유지하고 이상 여부만 확인”을 선택하여 제품 스타일은 유지했다. 기존 mounted 검증에 실제 여백·행 크기 검사를 추가했다. `git diff --check`도 통과했다.
