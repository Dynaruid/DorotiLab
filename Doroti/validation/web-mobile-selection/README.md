# Web 모바일 텍스트 선택 메뉴

## iOS 네이티브 선택 핸들 표시 수정

이전 네이티브 메뉴·확대경 전환에서 IME textarea의 `filter: opacity(0%)`와
`caret-color: transparent`를 남겨두었다. WebKit은 투명한 caret 색상일 때
네이티브 선택 UI도 숨긴다([WebKit 변경 기록](https://results.webkit.org/commit?id=300554%40main&repository_id=webkit)).

- iOS 브라우저 선택 모드의 활성 IME는 전체 투명 필터를 제거하고 caret/핸들 색상을 표시한다.
  글자만 `-webkit-text-fill-color: transparent`로 숨기며, 선택 배경도 투명하게 해
  캔버스의 글자·선택 배경과 중복되지 않도록 한다.
- 같은 모드에서 Framework 커서(최초 build 및 깜박임 갱신)와 선택 핸들을 숨긴다.
  `BrowserContextMenu`의 명시적 소유권 전환 시 DOM 표시 정책도 함께 갱신한다.
- 기존 runner에 실제 computed CSS의 filter·caret·glyph·selection 상태 검사와
  선택 범위 스크린샷을 추가했다. 변경 전 배포본은 `handles-before`에서
  `iOS native selection paint is not filtered out`로 실패했다.

Release publish PASS (경고·오류 없음), `git diff --check` 및 runner 문법 검사 PASS.
Chrome 153/macOS, WebGL에서 iPhone 390px·iPad 820px의 CSS 표시 정책,
터치 소유권, 메뉴 중복 방지, 선택·입력 동기화 PASS. Android 390px의 기존
메뉴·Copy/Cut/Paste 및 투명 DOM 유지 회귀도 PASS.
배포본은 `Doroti/artifacts/web-mobile-selection/handles-product/`, 실행 증거·제공 자산
hash·스크린샷은 같은 상위 경로의 `handles-iphone`, `handles-ipad`, `handles-android`에 있다.

실기기의 OS 핸들 픽셀·드래그는 Chromium 에뮬레이션 검증에 포함되지 않는다.


## iOS 네이티브 선택 UI로 변경 (2026-09-22 후속)

실제 iPhone 웹에서 OS 메뉴·확대경과 Doroti UI가 중복된다는 제보에 따라,
입력창의 iOS/iPadOS 기본 정책을 브라우저 소유로 변경했다.

- iOS 편집 요소의 contextmenu와 touch callout을 허용한다. 활성 IME textarea의
  터치는 취소하거나 Framework에 중복 전달하지 않는다. 캔버스에서 시작해 캡처한
  포인터는 끝까지 전달해 제스처 상태를 정리한다.
- `EditableText`의 메뉴, overlay 복원 및 선택 핸들 드래그 종료 경로의 메뉴를 차단한다.
  확대경은 일반 길게 누르기와 핸들 드래그 경로 모두 비활성화한다.
- 캔버스 일반 텍스트의 `SelectableRegion`, Android 웹, 네이티브 iOS 앱은 기존 정책을 유지한다.
  `BrowserContextMenu.disableContextMenu()`의 명시적인 메뉴 소유권 전환은 유지한다.
- runner의 iPhone/iPad 항목은 브라우저 메뉴 허용, 실제 터치 이벤트의 기본 동작 허용,
  Doroti 메뉴 부재, DOM 선택·편집의 Framework semantics 반영을 검증한다.
  Chromium 에뮬레이션으로 UIKit 메뉴·확대경의 표시를 검증할 수는 없다.

후속 검증: Release publish PASS (빌드 로그에 경고·오류 없음), `git diff --check`,
`node --check` PASS. Chrome 153/macOS, WebGL에서 다음 결과를 얻었다.

| 프로필 | 결과 |
| --- | --- |
| iPhone 390px | PASS: Framework 제스처에서도 메뉴 없음, 실제 DOM 편집 영역 터치 허용, 선택·텍스트 변경의 semantics 동기화 |
| iPad 데스크톱 UA 820px | PASS: iPhone과 같은 네이티브 정책·입력 검증 |
| Android 390px | PASS: Doroti Copy/Cut/Paste 메뉴와 편집·클립보드 회귀 |
| macOS 데스크톱 1280px | FAIL: 실제 우클릭이 input 대신 루트를 대상으로 전달되어 contextmenu 취소. 변경 전 JS 호스트 정책으로 비교해도 같은 실패. 이번 iOS 변경에서 데스크톱 경로는 수정하지 않음. |

산출물은 `Doroti/artifacts/web-mobile-selection/`의 `iphone-native-final`,
`ipad-native-final`, `android-regression`, `desktop-diagnostic`,
`desktop-before-host-policy`에 있다. 각 폴더에 제공 자산 hash와 이벤트 결과를 보관한다.
변경 전 호스트 정책 비교 후 배포 JS는 최종 수정본으로 복구했다.
초기 iPhone 검증 2회는 입력 영역 대신 장식을 터치해 실패했으며,
runner를 실제 DOM 입력 좌표와 Framework 장식 좌표로 분리한 뒤 통과했다.
데스크톱 역시 실제 DOM 좌표로 보정했지만 위의 우클릭 차단은 계속 재현됐다.
실제 iPhone/iPad Safari·Chrome의 OS 메뉴·확대경 표시는 `notVerified`다.

이어지는 기존 결과는 변경 전 정책과 당시 실행 환경의 이력이다.

## 변경 전 검증 이력

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
