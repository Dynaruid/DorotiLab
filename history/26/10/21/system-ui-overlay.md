# AppBar / AnnotatedRegion 시스템 표시줄 스타일

날짜: 2026-10-21

상태: 프레임워크 연결·MAUI Android/iOS 구현 및 아래 회귀/컴파일 검증 완료.
실제 Android/iOS 기기의 표시줄 외관과 native window/handler 재생성은 `notVerified`.
기존 `work.md`의 Qt/Linux 배포 작업은 이번 변경 범위에 포함하지 않았다.

## 구현

- 기존 `AppBar.systemOverlayStyle`, `AppBarThemeData.systemOverlayStyle`,
  `AnnotatedRegion<T>`, `RenderAnnotatedRegion<T>`, `RenderView._updateSystemChrome`
  경로를 사용한다. API를 중복 생성하지 않았다.
- `Doroti.Hosting/SystemUiOverlayPlatformMessageCapability.cs`가
  `flutter/platform`의 `SystemChrome.setSystemUIOverlayStyle`을 받아 8개 optional
  필드를 검증하고 owning native host로 전달한다. 나머지 메시지와 수신 핸들러 등록은
  기존 채널에 전달한다. JSON 오류/잘못된 필드는 method error envelope를 반환한다.
- `SystemUiOverlaySettings.Merge`는 null/생략 필드의 이전 native 값을 보존한다.
  색상은 ARGB uint32이며 brightness는 Doroti 및 Flutter 직렬화 표기를 모두 받는다.
- `SystemChrome`의 pending/latest 캐시는 `ViewLocal`로 이동했다. 한 이벤트의 마지막
  요청만 전달하고, 같은 스타일인 다른 창도 각각 갱신하며, detached 캐시 초기화와
  비동기 전달은 원래 창에 속한다.
- MAUI Android는 현재 surface의 Activity/Window에 아이콘 밝기, 지원되는 배경·구분선
  색상, 대비 설정을 적용한다. Android 15+의 legacy 배경색 제한과 버전별 기능을
  구분한다. 전역 CurrentActivity를 사용하지 않는다.
- MAUI iOS는 Doroti 전용 PageHandler/PageViewController에서
  `statusBarBrightness`를 상태 표시줄 스타일로 변환한다. 전역 UIApplication 스타일이나
  앱 테마를 변경하지 않는다. inherited CurrentView의 weak ownership을 유지한다.
- Native 갱신은 main thread에서 실행하고, surface Loaded/HandlerChanged 시 저장한
  스타일을 재적용한다. host Closed에서 이벤트를 해제한다.
- Android 상태 표시줄 필드는 상단 annotation에서, 내비게이션 바 필드는 하단
  annotation에서 선택한다. iOS는 상단만 사용한다. annotation이 없으면 마지막 스타일을
  유지한다. 페이지를 pop하면 아래 페이지의 annotation이 다시 선택된다.

## 검증 결과

모든 dotnet 명령은 `python Doroti/eng/run-with-timeout.py --timeout 1200`으로 감쌌다.

| 실행 | 결과 / 증거 범위 |
| --- | --- |
| `dotnet run --project Doroti/tests/Doroti.Tests -c Release -- --system-ui-overlay` | PASS. 채널 8개 필드, ARGB, 잘못된 입력, 취소, fallback, null/false 보존, Android/iOS DPR 2 영역 선택, sized true/false, 변경·제거, 페이지 push/pop, coalescing, detached 초기화, 두 창의 독립 pending/latest. CPU/synthetic |
| `dotnet run --project packages/Doroti.Material/tests/Doroti.Material.Tests -c Release -- --system-ui-overlay` | PASS. AppBar explicit → theme → 배경 밝기 우선순위, 상위 annotation의 하단 바 소유권, AppBar 없는 화면. CPU/synthetic |
| `dotnet run --project packages/Doroti.Material/tests/Doroti.Material.Tests -c Release` | PASS. 새 시스템 UI 검증과 기존 shader/color/IME/menu/radio/tooltip/expansion 회귀. CPU/synthetic |
| `dotnet run --project Doroti/tests/Doroti.Tests -c Release --no-build -- --android-back` | PASS. 기존 Back/navigation/restoration/lifecycle 회귀. CPU/synthetic |
| `dotnet build packages/platforms/maui/Doroti.Host.Maui/Doroti.Host.Maui.csproj -c Release -f net10.0-android -p:DorotiHostTargetFrameworks=net10.0-android --nologo -v:q` | PASS, 경고 0 / 오류 0. Android host 컴파일 |
| 위 build 명령의 TFM과 property 값을 `net10.0-ios`로 변경 | PASS, 경고 0 / 오류 0. Windows에서 iOS host managed 컴파일. Mac native link, simulator, 기기 실행 증거가 아님 |
| `git diff --check` | PASS |

실제 기기의 Android gesture/3-button navigation, API별 색상/대비 표시,
Activity recreation 및 iOS UIKit 상태 표시줄/scene 재연결 검증은 남아 있다.
Windows/Web의 실제 앱 실행이나 전체 제품 검증을 수행한 것으로 간주하지 않는다.

## 사용법과 참고

- [MAUI 시스템 표시줄 예제와 제한](../../../../packages/platforms/maui/README.md#system-bars)
- [Material 안내](../../../../packages/Doroti.Material/README.md)
- [유지되는 테스트 명령](../../../../Doroti/tests/README.md)
- [Flutter AppBar.systemOverlayStyle](https://api.flutter.dev/flutter/material/AppBar/systemOverlayStyle.html)
- [Flutter SystemChrome.setSystemUIOverlayStyle](https://api.flutter.dev/flutter/services/SystemChrome/setSystemUIOverlayStyle.html)
- [Android edge-to-edge](https://developer.android.com/develop/ui/views/layout/edge-to-edge)
- [MAUI 10.0.20 PageHandler](https://github.com/dotnet/maui/blob/10.0.20/src/Core/src/Handlers/Page/PageHandler.iOS.cs)

## 샘플 적용 — 2026-10-21

- `samples/DorotiTestbedApp`: 기본 Material 화면의 AppBar에 명시적 스타일을 전달하고,
  MaterialApp builder의 annotation으로 테마·seed 변경 시 Android 하단 바도 갱신한다.
- `samples/DorotiCarouselApp`: SafeArea 바깥에 annotation을 배치해 커스텀 헤더 화면의
  밝은 배경과 어두운 상태/내비게이션 아이콘을 선택한다.
- `samples/DorotiSampleApp2`: CupertinoApp builder에서 현재 Cupertino 테마를 읽어
  annotation을 만들며 Settings의 System/Light/Dark 선택을 따른다.
  CupertinoNavigationBar의 자체 상단 영역 선택은 유지한다.
- 각 샘플 README와 Testbed 한국어 README에 적용 방식·Android 15 배경색 제한을 안내했다.
- 각 샘플 공용 csproj에 `dotnet build <project> -c Release --nologo -v:q` 실행:
  세 프로젝트 모두 PASS, 각각 경고 0 / 오류 0.
- `dotnet run --project packages/Doroti.CustomCarousel/tests/Doroti.CustomCarousel.Tests -c Release -- --pointer-packets`:
  5개 데모 mouse/trackpad packet 및 animated hover hit testing PASS (CPU/synthetic).
- 위 명령도 모두 1,200초 timeout wrapper를 사용했다. 실제 모바일 실행·표시 확인은 `notVerified`.
