# Material 3 Demo

저장소 루트의 `material_3_demo`를 이 Flutter 앱에 구성했습니다.
기본 진입점은 `lib/main.dart`이며 컴포넌트, 색상, 타이포그래피, 높이 화면과
Material 3 기반의 밝은/어두운 테마와 시드·이미지 기반 색상 선택을 제공합니다.

## 실행

Flutter SDK는 `pubspec.yaml`의 Dart `^3.12.2` 조건을 충족해야 합니다.
저장소 루트에서 다음 명령을 실행합니다.

```powershell
cd reference/flutter_sample_app
flutter pub get
flutter run -d windows
# 웹 실행
flutter run -d chrome
```

패키지 이름과 기존 플랫폼 프로젝트는 `flutter_sample_app`을 유지합니다.
원본의 `resolution: workspace` 및 `../analysis_defaults` 의존성은 사용하지 않고,
기존 `flutter_lints` 설정과 `url_launcher`, `integration_test`를 사용합니다.
이미지 기반 색상 선택에는 인터넷 연결이 필요합니다.
원본 Flutter samples의 저작권 헤더와 BSD 라이선스(`LICENSE`)를 포함합니다.

## Image demo

Components 맨 아래의 **Image demo**에서 로컬 사진과 이미지 URL을 전환하고
Contain/Cover 표시를 비교할 수 있습니다. **Extract colors**는 선택한 이미지의
라이트·다크 Primary/Secondary/Tertiary와 RGB 값을 표시합니다.
로딩 실패 시 **Retry image**로 재시도할 수 있으며, 추출 실패는 섹션 안에 표시됩니다.

- 로컬: `assets/images/mae-mu-9002s2VnOAY-unsplash.webp` (사용자 제공, Mae Mu / Unsplash)
- URL: `https://plus.unsplash.com/premium_photo-1734210255965-0a721514a34e` (사용자 제공)

로컬 사진은 asset으로 포함되어 오프라인에서도 사용할 수 있습니다.
URL 사진은 네트워크 연결과 브라우저 CORS 허용이 필요합니다.
이 섹션은 reference 앱에 추가한 로컬 확장이며 기존 6개 테마 이미지 선택은 유지합니다.

## 검증

```powershell
flutter analyze
flutter test --timeout 20m
flutter build web --release
flutter build windows --debug
```

테스트/빌드 프로세스는 저장소 규칙에 따라 최대 20분으로 제한합니다.
원본 데모의 위젯 테스트와 통합 테스트를 이 패키지의 import 경로로 옮겼습니다.
기기 통합 테스트는 `flutter test integration_test -d windows --timeout 20m`으로 실행할 수 있습니다.

## 기존 웹 비교 화면

기존 소스는 `lib/differential_main.dart`와 `lib/resize_fixture.dart`에 보존했습니다.
웹 전용이므로 다음과 같이 별도 진입점으로 실행합니다.

```powershell
flutter run -d chrome -t lib/differential_main.dart
flutter build web --release -t lib/differential_main.dart
```

리사이즈 fixture는 이 실행 화면의 URL에 `?dorotiResizeFixture=F0`을 붙입니다
(`F1`, `F2`도 지원). `Doroti/eng/run-web-flutter-differential.ps1`도 이 진입점을 빌드합니다.
두 진입점의 웹 빌드는 같은 `build/web`을 사용하므로 전환할 때 다시 빌드해야 합니다.
기존 renderer 쿼리 설정(`canvaskit` 기본값, `skwasm`은 `--wasm` 빌드 필요)은 유지합니다.
