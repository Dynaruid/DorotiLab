# Web 텍스트 편집 정책과 검증

## 현재 구성

| 영역 | 동작 | 담당 |
| --- | --- | --- |
| 메뉴용 클립보드 상태 | Flutter와 같이 웹 전용 notifier가 Paste를 제공하고 `update()`는 조회하지 않음 | `EditableTextState.clipboardStatus` |
| 명시적 클립보드 조회 | `Clipboard.hasStrings()`는 실제 텍스트 유무 조회, `getData`/Paste는 실제 읽기 | Services → platform capability → Clipboard API |
| iOS 키보드 포커스 | 짧은 탭의 pointerup 안에서 DOM focus; 스크롤·길게 누르기·취소·보조 포인터 제외 | `doroti.web.text-focus.ts` |
| 입력 연결 | Worker attach 전 selectionchange 보류, 먼저 도착한 IME 입력은 보존 | `doroti.web.ts` |
| 표시·숨김과 위치 갱신 | Show/Hide 요청은 별도 처리; 텍스트 ACK·caret geometry 갱신은 포커스를 가져오지 않음 | BrowserHostAdapter / DOM bridge |
| 선택 UI | iOS 메뉴·확대경·핸들·커서·선택 배경은 Doroti; DOM 입력은 opacity 0 / pointer-events auto, 화면 포인터는 root에서 가로채 전달 | Framework + Web CSS |
| 웹검색·공유 | 웹검색은 trusted 탭 안에서 탭을 열고 Worker의 검색어로 이동; API 차단 시 재실행 링크/버튼, 공유 취소는 정상 종료 | `doroti.web.text-actions.ts` |
| 메뉴 글꼴 | Cupertino 테마 글꼴과 앱 fallback, Web Roboto fallback 사용 | Cupertino toolbar button |
| 확대경 | safe area/키보드 안에 배치하고 확대 좌표 보정; 기존 layout/scroll 프레임에서 위치 추적 | Cupertino magnifier / TextSelectionOverlay |

브라우저에서 제공하지 않는 시스템 사전 `Look Up` 항목은 숨긴다. 네이티브 앱의 메뉴 및
플랫폼 클립보드 계약은 변경하지 않는다. DOM fallback 대화상자도 host 종료 시 정리한다.

## Flutter 대조

저장소의 `reference/flutter-master`를 기준으로 다음 두 계층을 구분했다.

- `packages/flutter/lib/src/widgets/editable_text.dart`: 웹에서는
  `_WebClipboardStatusNotifier`를 사용한다. 초기 상태는 `pasteable`, `update()`는 no-op이다.
  Doroti의 동일 구현을 유지한다.
- `engine/src/flutter/lib/web_ui/lib/src/engine/clipboard.dart`:
  `hasStringsMethodCall`은 `getData()` 결과의 `isNotEmpty`를 반환한다.
  Doroti도 명시적인 `Clipboard.hasStrings()`가 실제 조회하도록 유지한다.

클립보드 호스트에서 무조건 true를 반환하는 변경은 적용하지 않았다.
[대조 파일과 SHA-256](flutter-reference.json)을 기록했다. 현재 기준 제품의 첫 탭 검사에서
클립보드 읽기는 0회였다. 기존 문제를 자동 클립보드 조회 탓으로 단정했던 설명은 정정한다.
기준 제품은 trusted pointerup 종료 시 아직 canvas가 포커스되어 있어 새 포커스 검사에서 실패했다.
실제 클립보드 읽기(Paste 또는 명시적 조회)에는 브라우저 권한 UI가 나타날 수 있다.

### Flutter의 웹검색과 Doroti의 차이

Flutter iOS 앱은 `SearchWeb.invoke` → `FlutterPlatformPlugin.searchWeb` →
`UIApplication.openURL(x-web-search://?검색어)`로 시스템에 검색을 요청한다.
Flutter 웹은 기본적으로 `BrowserContextMenu.enabled == true`이므로 브라우저 메뉴를
사용하고 `EditableText.showToolbar()`는 Flutter 메뉴를 표시하지 않는다.
웹 엔진 `platform_dispatcher.dart`에는 `SearchWeb.invoke` 구현이 없다.
따라서 Doroti의 웹검색 모달은 Flutter 기본 동작이 아니라 별도로 추가했던 대체 처리다.

iPhone에서 보고된 모달은 Worker 응답 뒤의 `window.open`이 차단되었을 때 표시된다.
이를 피하도록 Cupertino 웹검색 버튼에 언어와 무관한 semantics 식별자를 붙이고,
trusted 탭 이벤트 안에서 빈 탭을 열어 둔 후 기존 플랫폼 메시지의 검색어로 이동한다.
취소·드래그·비활성 버튼에는 탭을 열지 않으며, 사용하지 않은 탭은 timeout/host 종료 시
정리한다. 정상 처리된 검색 탭은 앱 종료 시 닫지 않는다. 새 창 자체가 차단된 환경에서만
기존 재실행 링크를 제공한다. 접근성 click/키보드 활성화도 Worker 전달 전에 탭을 연다.

이전 E2E의 `window.open` 대역은 호출 시점과 무관하게 성공해서 실제 기기 문제를 놓쳤다.
이제 trusted pointerup 밖에서 새 창을 열면 실패하는 대역으로 검사하고, 검색어 전달이
한 번만 일어나는지와 모달 부재를 확인한다.

Flutter의 `SystemContextMenu`는 웹에서 지원되지 않는다. 웹의 브라우저 메뉴 허용은
네이티브 텍스트 선택 동작을 다시 허용하는 것이므로, 메뉴만 네이티브로 바꾸고
핸들·확대경의 네이티브 터치 처리를 배제하는 API로 사용할 수 없다.

## 검증

각 브라우저 실행은 새 출력 폴더를 사용하며 20분 process-tree timeout을 적용한다.
브라우저 GPU 실행은 순차로 수행한다.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -o Doroti/artifacts/web-mobile-selection/review-product
node Doroti/validation/web-mobile-selection/focus-contracts.mjs Doroti/artifacts/web-mobile-selection/review-product/wwwroot
python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/review-iphone Doroti/artifacts/web-mobile-selection/review-product/wwwroot iphone
DOROTI_EARLY_INPUT=1 python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/review-ipad-ime Doroti/artifacts/web-mobile-selection/review-product/wwwroot ipad worker-direct-webgl 820
python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/review-android Doroti/artifacts/web-mobile-selection/review-product/wwwroot android
DOROTI_SEARCH_FIELD=1 python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/review-iphone-search Doroti/artifacts/web-mobile-selection/review-product/wwwroot iphone
```

- 클립보드를 미리 채운 뒤 첫 탭: Clipboard 읽기 0회, Paste-only 메뉴 부재, 이벤트 안의 focus.
- 비활성 입력창, 탭/스크롤/길게 누르기/취소, Worker attach 전에 시작된 IME 조합.
- 선택·핸들 드래그, Cut/Paste, 웹검색·공유의 실제 Framework→Worker→DOM 전달.
- 브라우저 팝업 차단·공유 취소/재실행, Show/Hide 상태 및 대화상자 focus 유지.
- 화면 상단의 확대경과 자동 스크롤 후 확대 대상 추적 캡처.
- 실제 `SearchAnchor` 검색 화면 진입·복귀, 상단 검색 입력창의 확대경·드래그·메뉴 캡처.

웹검색/공유 API는 테스트 브라우저에서 대역으로 확인하며 외부 검색·전송을 실행하지 않는다.
Chromium 에뮬레이션은 실제 iPhone 소프트 키보드·WebKit 권한·OS 공유 UI 검증을 대신하지 않는다.

## 2026-09-22 실행 결과

Release publish와 탭 포커스 계약 검사를 통과했다. 아래는 모두 같은 publish 산출물을
사용한 Chromium 모바일 에뮬레이션 결과다. 입력 중간에 발생한 page exception과 renderer
failure는 없었다. 검색/일반 입력창 확대경 캡처를 열어 화면 안의 위치와 확대 대상을 확인했다.

| 실행 | 결과 | 기록 |
| --- | --- | --- |
| iPhone 선택·클립보드·메뉴·포커스 | PASS | [result](../../artifacts/web-mobile-selection/review-iphone/result.json) |
| iPad + Worker attach 이전 IME 입력 | PASS | [result](../../artifacts/web-mobile-selection/review-ipad-ime/result.json) |
| Android 선택·Cut/Paste 회귀 | PASS | [result](../../artifacts/web-mobile-selection/review-android/result.json) |
| iPhone SearchAnchor 검색 입력창 | PASS | [result](../../artifacts/web-mobile-selection/review-iphone-search/result.json) |

검색 입력창의 확대경은 [길게 누르기](../../artifacts/web-mobile-selection/review-iphone-search/search-top-magnifier.png)와
[드래그](../../artifacts/web-mobile-selection/review-iphone-search/search-top-magnifier-drag.png) 모두
화면 상단 안에 표시된다. 일반 입력창의 [자동 스크롤 뒤 확대경](../../artifacts/web-mobile-selection/review-iphone/top-edge-magnifier.png)도
현재 글자를 확대한다. 실제 iPhone의 소프트 키보드와 네이티브 공유 화면은 미검증이다.

이전 정책의 실험 결과는 [policy-history.md](policy-history.md)에 보관했다.

### 웹검색의 추가 모달 수정 검증

```sh
python3 Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -o Doroti/artifacts/web-mobile-selection/search-gesture-product
node Doroti/validation/web-mobile-selection/search-contracts.mjs Doroti/artifacts/web-mobile-selection/search-gesture-product/wwwroot
DOROTI_SEARCH_FIELD=1 python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/search-gesture-search-final Doroti/artifacts/web-mobile-selection/search-gesture-product/wwwroot iphone
python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/search-gesture-iphone-final Doroti/artifacts/web-mobile-selection/search-gesture-product/wwwroot iphone
```

Release publish, 새 탭 수명/검색어 인코딩 계약 검사,
[검색 입력창 17개 검사](../../artifacts/web-mobile-selection/search-gesture-search-final/result.json),
[일반 입력창 44개 검사](../../artifacts/web-mobile-selection/search-gesture-iphone-final/result.json)를 통과했다.
탭 이벤트 안의 새 탭 열기 → Worker 검색어 전달 1회 → 모달 부재를 확인했다.
취소·드래그에서는 새 탭을 열지 않았다. 일반 입력·선택·클립보드·확대경 회귀도 통과했다.
브라우저 API는 대역이며, 실제 iPhone Safari의 새 탭 전환은 기기에서 재확인이 필요하다.

### 스페이스바 길게 누르기 커서 이동

네이티브 핸들 간섭을 막으면서 `.doroti-ime`에 적용했던 `pointer-events: none`이
WebKit의 키보드 커서 위치 계산도 방해했다. WebKit의
[`visiblePositionInFocusedNodeForPoint`](https://github.com/WebKit/WebKit/blob/main/Source/WebKit/WebProcess/WebPage/Cocoa/WebPageCocoa.mm)는
입력창 내부로 좌표를 제한한 뒤 `frame.visiblePositionForPoint`로 커서를 계산한다.
입력창이 hit testing에서 빠지면 입력창 뒤의 canvas를 대상으로 계산하게 된다.

입력창의 `opacity: 0`은 유지하고 `pointer-events: auto`로 복원했다.
네이티브 선택 assistant는 투명도로 계속 억제한다. 화면의 터치는 root에서
`preventDefault()` 후 Doroti로 전달하고, 키보드가 바꾼 커서는 기존 `selectionchange`로
Framework에 전달한다. 별도 키보드 드래그 감지나 클립보드 조회를 추가하지 않았다.

iOS 26.5 Simulator의 WKWebView에서 동일한 투명 입력창으로 비교했다.

| 입력창 hit testing | x=45 요청 | x=80 요청 | 네이티브 선택 assistant |
| --- | --- | --- | --- |
| `none` (수정 전) | offset 0 | offset 0 | 억제됨 |
| `auto` (수정 후) | offset 2 | offset 5 | 억제됨 |

[원시 관측값](../../artifacts/web-mobile-selection/keyboard-cursor-webkit/observations.json)과
[결과](../../artifacts/web-mobile-selection/keyboard-cursor-webkit/result.json)를 기록했다.
테스트 전용 앱에서 WebKit의 native point-selection SPI를 호출한 검사이며,
실제 소프트 키보드의 스페이스바 제스처를 자동화한 검사는 아니다. SPI는 제품에 포함하지 않는다.

```sh
# Booted iOS Simulator UUID를 지정한다. Xcode/Apple Silicon 환경 필요.
python3 Doroti/validation/web-mobile-selection/ios-keyboard-cursor-probe.py SIMULATOR_UUID Doroti/artifacts/web-mobile-selection/keyboard-cursor-webkit
python3 Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -o Doroti/artifacts/web-mobile-selection/keyboard-cursor-product
DOROTI_SEARCH_FIELD=1 python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/keyboard-cursor-search Doroti/artifacts/web-mobile-selection/keyboard-cursor-product/wwwroot iphone
python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-mobile-selection/run.mjs Doroti/artifacts/web-mobile-selection/keyboard-cursor-iphone Doroti/artifacts/web-mobile-selection/keyboard-cursor-product/wwwroot iphone
```

Release publish, WebKit 비교 검사,
[검색창 21개 검사](../../artifacts/web-mobile-selection/keyboard-cursor-search/result.json),
[일반 입력창 44개 검사](../../artifacts/web-mobile-selection/keyboard-cursor-iphone/result.json)를 통과했다.
검색창에서는 텍스트 입력·화면 터치 없이 커서만 이동시켜 Framework에 반영되는지도 확인했다.
일반 입력창에서는 DOM 입력창을 대상으로 한 터치가 Doroti 선택으로 전달되는지,
핸들 드래그·확대경·메뉴·Cut/Paste·웹검색이 유지되는지를 확인했다.
실제 iPhone 키보드의 스페이스바 길게 누르기 제스처는 기기에서 재확인이 필요하다.
