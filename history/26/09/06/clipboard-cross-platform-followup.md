# 클립보드·빈 필드 메뉴의 플랫폼별 후속 검토

## 발견한 문제와 수정

1. **Web**: `_WebClipboardStatusNotifier`의 `value`가 부모의 속성을 숨겼다. `ClipboardStatusNotifier` 또는 `ValueListenable`로 읽는 EditableText는 `unknown`을 받아 프레임워크 Paste 메뉴를 비활성/누락 처리했다. 부모 생성자에 `pasteable`을 전달하도록 수정했다. 브라우저 네이티브 입력·메뉴 소유권은 유지하며, 웹 상태 조회는 클립보드 읽기 권한을 요청하지 않는다.
2. **Qt/Linux**: 네이티브 QClipboard 작업은 이미 GUI 큐에서 실행됐으나, managed `TaskCompletionSource`의 `RunContinuationsAsynchronously` 때문에 실제 편집/메뉴 갱신의 await 이후 코드가 ThreadPool로 넘어갔다. 강제 비동기 실행 옵션을 제거해 GUI 응답 콜백에서 이어지게 했다. 인라인 continuation을 고려해 dispose 시 대기 요청 취소를 lock/열거 밖으로 옮겼고, disposed host의 새 읽기는 거부한다.
3. **MAUI Android/iOS/macOS/Mac Catalyst/Windows**: 읽기·쓰기·상태 조회를 `MainThread.InvokeOnMainThreadAsync`로 전달한다. UI 큐 대기 중 취소도 실행 직전에 확인한다. [MAUI Clipboard 문서](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/data/clipboard?view=net-maui-10.0)는 UI 스레드 접근을 요구한다.
4. **공통 Services**: `IPlatformServicesHostCapability.HasClipboardTextAsync`를 추가하고 `Clipboard.hasStrings`를 연결했다. 기존 호스트는 읽기 기반 default 구현을 유지하고, MAUI는 `Clipboard.Default.HasText`를 사용한다. 특히 iOS 구현은 [UIPasteboard.HasStrings와 실제 String 읽기를 분리](https://raw.githubusercontent.com/dotnet/maui/main/src/Essentials/src/Clipboard/Clipboard.ios.cs)하므로, 메뉴 상태 확인만으로 텍스트 전체를 읽을 필요가 없다.

Qt 템플릿과 DemoApp native host 모두 `QMetaObject::invokeMethod(..., Qt::QueuedConnection)`으로 clipboard 작업을 전달하는 것을 확인했다. 이 native 부분은 변경하지 않았다. Web UI Worker와 CanvasKit Worker의 clipboard 제어 요청이 main으로 전달되는 코드도 확인했다.

## 검증

모든 프로세스에 1,200초 제한을 적용했다.

| 검사 | 결과와 범위 |
| --- | --- |
| 수정 전 Web 상태 회귀 | **FAIL 재현**, base notifier API가 pasteable이 아님. `.doroti/clipboard-crossplatform-before.log` |
| 수정 전 Qt 지연 응답 | **FAIL 재현**, await continuation이 GUI 콜백 스레드를 벗어남. `.doroti/clipboard-qt-before.log` |
| 수정 후 전체 FCR-7 | **PASS**. Web notifier, 상태 전용 API 호출 및 content read 미호출, 기존 실제 Clipboard/EditableText 경로와 6개 플랫폼 메뉴 검사 포함. `.doroti/clipboard-crossplatform-material.log` |
| Qt ABI + 키보드 + clipboard callback/disposal | **PASS**. 실제 managed adapter와 ABI 함수 포인터를 사용하며 native GUI callback은 대역. `.doroti/clipboard-qt-after.log` |
| Web DemoApp 빌드 | **PASS**, `.doroti/clipboard-crossplatform-web-build.log` |
| Chromium auto | **2 PASS**, `.doroti/clipboard-crossplatform-browser-auto.log` |
| Chromium worker-direct-webgl | **2 PASS**, `.doroti/clipboard-crossplatform-browser-worker-direct-webgl.log` |
| Chromium worker-canvaskit-webgl | **2 PASS**, `.doroti/clipboard-crossplatform-browser-worker-canvaskit-webgl.log` |
| MAUI Android/iOS/macOS/Mac Catalyst/Windows | **build PASS**, `.doroti/clipboard-crossplatform-{android,ios,macos,catalyst,maui-windows}.log` |
| WindowsAppSdk DemoApp | **build PASS**, `.doroti/clipboard-crossplatform-windows-demo.log` |

각 Chromium 모드의 두 검사는 main browser clipboard bridge와 OS clipboard 간 한글·이모지·빈 문자열 왕복, native textarea의 Ctrl+A/C/X/V·선택 영역 교체·Shift+Tab/Tab 이동을 확인한다. UI Worker 내부의 Services clipboard RPC 호출이나 브라우저 기본 메뉴 픽셀을 직접 검증한 것으로 확대 해석하지 않는다. 프레임워크 Web Paste 상태는 FCR-7로 별도 검증했다.

Android/Apple 실제 기기, Linux Qt 실제 GUI, MAUI UI 스레드에서의 기기 클립보드 동작은 **notVerified**다. Apple 빌드는 Windows에서 managed host 컴파일을 확인한 것이며 native link/package나 Apple 실행을 의미하지 않는다. 검증용 Web 서버는 종료한다.
