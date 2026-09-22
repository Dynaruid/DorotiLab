# Web 모바일 텍스트 선택 메뉴

2026-09-22: **Release publish 및 Chromium 자동 검증 PASS**. 실제 Android/iOS,
Safari/WebKit, 모바일 키보드/IME, TalkBack/VoiceOver는 `notVerified`.

## 변경과 Flutter 비교

- `reference/flutter-master/packages/flutter/lib/src/widgets/editable_text.dart`의
  `_webContextMenuEnabled`는 웹 전체에서 렌더링 toolbar를 차단한다.
  같은 참조의 `selectable_region.dart`는 Android/iOS를 제외한다. Doroti의
  `EditableTextState`도 후자의 정책을 적용해 모바일 터치 선택의 toolbar를 렌더링한다.
  데스크톱과 `BrowserContextMenu` API 상태는 유지한다.
- Flutter 엔진의
  `view_embedder/embedding_strategy/full_page_embedding_strategy.dart`는
  `user-select: none`과 `-webkit-user-select: none`을 적용한다. Doroti는 캔버스와
  비편집 semantics 노드에 선택 방지를 적용하고, IME/접근성 input·textarea는
  `user-select: text`로 유지한다. 접근성 트리를 숨기거나 제거하지 않는다.
- Android의 `navigator.platform = Linux armv8l`을 UA와 함께 판별한다.
  MacIntel + 다중 터치인 iPadOS도 모바일로 판별한다. 모바일에서는 Doroti 소유
  편집 요소의 네이티브 contextmenu와 WebKit touch callout을 억제한다.
  PlatformView의 별도 HTML 입력 요소에는 모바일 억제를 확장하지 않는다.

## 재현

저장소 루트에서 실행한다. 각 실행은 20분 process-tree timeout을 사용하며,
runner는 자체 HTTP 서버와 새 headless Chrome 프로필을 만들고 종료한다.
출력 폴더는 매번 새 경로를 사용한다. `DOROTI_CHROME`으로 실행 파일을 지정할 수 있다.

```powershell
python Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -o Doroti/artifacts/web-mobile-selection/product
python Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/android-final Doroti/artifacts/web-mobile-selection/product/wwwroot android
python Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/iphone-final Doroti/artifacts/web-mobile-selection/product/wwwroot iphone
python Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/ipad-final Doroti/artifacts/web-mobile-selection/product/wwwroot ipad worker-direct-webgl 820
python Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/desktop-final Doroti/artifacts/web-mobile-selection/product/wwwroot desktop worker-direct-webgl 1280
```

## 결과

| Chromium 설정 | 결과 |
| --- | --- |
| Android UA + Linux armv8l, 390px, 터치 | PASS: 길게 누르기 → 단어 선택 → 렌더링 메뉴 → 잘라내기·붙여넣기 |
| iPhone UA, 390px, 터치 | PASS: 포커스된 필드 길게 누르기 → Select All → 잘라내기·붙여넣기 |
| iPad 데스크톱 UA + MacIntel + 5 touch points, 820px | PASS: 모바일 정책 및 iOS 메뉴·편집 동작 |
| Windows 데스크톱, 1280px | PASS: 실제 CDP 오른쪽 클릭의 contextmenu가 취소되지 않으며 렌더링 메뉴가 중복되지 않음 |

공통으로 입력 유지, 캔버스/비편집 semantics의 선택 방지 CSS, 편집 요소의 선택 가능
상태, Chrome AX 트리의 textbox 노출, 렌더 오류/페이지 예외 부재를 확인한다.
모바일에서는 선택 오프셋, 잘라내기 후 semantics 값, 클립보드 내용, 붙여넣기 결과,
HTML document selection이 비어 있음을 검사한다. 테스트 브라우저에만 클립보드 권한을
부여한다. `toolbar.png`와 iOS의 `selected-toolbar.png`에서 실제 렌더링 메뉴도 확인했다.

각 로컬 결과 폴더에 `result.json`, DOM, CSS 상태, 스크린샷, Chrome 버전 및 UA,
실제로 제공한 자산의 SHA-256을 저장한다. 이 폴더들은 삭제 가능한 산출물이다.
빌드는 마지막 실행에서 경고·오류 없이 성공했다. 초기 publish 중 CSS 수정으로 생긴
압축 자산 경로 오류는 소스 변경 완료 후 재publish하여 해소했다.
초기 headed CDP 입력 응답 timeout, 중첩 스크롤 및 검색창 화면 전환으로 실패한
검증 시도는 PASS로 세지 않았다. 최종 runner는 headless, 화면 바깥쪽 gutter 스크롤,
Filled/Outlined 일반 텍스트필드를 사용한다.

이 결과는 Chromium의 플랫폼/터치 에뮬레이션과 WebGL 경로 증거다. iPhone/iPad 이름은
실제 Safari 실행을 의미하지 않는다. 네이티브 데스크톱 팝업의 메뉴 문구·픽셀,
물리 터치, OS 선택 UI, 화면 판독기 사용성 검증을 대신하지 않는다.
