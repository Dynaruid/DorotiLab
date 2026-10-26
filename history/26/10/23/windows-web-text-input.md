# Windows·Web 텍스트 입력 수정 및 검증

날짜: 2026-10-23 (Asia/Seoul)

상태: **Windows 일반 MAUI R1·Windows App SDK R2/R3·Web R5 구현 및 아래 자동 입력 검사 완료. OS IME·Apple Safari·화면 성능·Windows 앱 정상 종료는 notVerified.**

## 입력과 범위

- 시작 HEAD: `04ad305c92bc5cc354c8f1d86cf84d910006dff3`, 시작 작업 트리 깨끗함.
- 시작 `memo.md` SHA-256: `12f8e4cc5161b1f3bd938b0963017da75fabc80db76abcd6e420ff0bb7ce3e72`.
- 당시 `memo.md`의 Windows MAUI R1, Windows App SDK R2/R3, Web R5와 관련 회귀 검사만 변경했다. 검토 내용은 [플랫폼별 입력 검토 요약](../22/text-input-platform-review-summary.md)에 보존했다. Apple R1·Qt R4·Android 구현·`work.md`는 변경하지 않았다.
- 저장소 지침에 따라 모든 빌드·테스트 명령을 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 제한했다. 반복 입력·삭제는 각 12회로 검사했다.

## 수정 내용

### Windows MAUI

[Windows 입력 구독](../../../../packages/platforms/maui/Doroti.Host.Maui/MauiTextInputBridge.Windows.cs)은 일반 Entry/Editor의 native TextBox `SelectionStart/SelectionLength`를 읽는다. 반복 문자·공백·emoji 편집의 위치를 문자열 차이로 추정하지 않는다. PasswordBox·보안 모드·선택 API 부재·handler 부재와 잘못된 선택 값은 기존 fallback으로 남는다.

WinUI는 TextChanging으로 native buffer를 바꾼 뒤 TextChanged/MAUI 문자열 반영을 지연할 수 있다. 실제 검사에서 native는 `aaaa`인데 지연 선택 callback이 managed의 이전 `aaaaa`를 발행하는 경우를 확인했다. 이 구간에는 선택 snapshot 발행을 막고, MAUI 매퍼 뒤에 등록한 native TextChanged 구독에서 같은 revision의 문자열·선택을 함께 발행한다. 발행은 해당 native callback 안에서 수행한다. 제품 코드는 native 전체 문자열을 추가로 읽지 않고 MAUI가 이미 만든 문자열을 사용한다.

[Release probe](../../../../samples/DorotiTestbedApp/windows/TextInputEditingProbe.cs)는 native 문자열을 별도로 읽어 **모든 전달 상태**를 비교한다. 이 읽기는 검증용 비용이며 제품 경로에 포함되지 않는다. opt-in 환경 변수는 `DOROTI_TEXT_EDITING_PROBE`다.

### Windows App SDK

[IMM production 상태 모델](../../../../packages/platforms/windowsappsdk/native/src/ime_editing_state.h)을 [메시지 처리](../../../../packages/platforms/windowsappsdk/native/src/exports.cpp)에서 사용한다.

- START의 원래 선택 영역과 실제로 삽입한 preedit 소유 상태를 구분한다.
- GCS 값 없는 취소와 END는 소유한 미확정 문자열만 제거한다. commit 뒤 END와 중복 END는 확정 텍스트를 보존한다.
- 빈 composition string과 IMM 조회 실패를 구분한다. 실패를 빈 문자열로 바꾸어 문서를 지우지 않는다.
- GCS_CURSORPOS를 문자열 적용 뒤 처리한다. cursor-only 메시지도 발행하고 음수 조회 결과·범위 경계·UTF-16 offset을 처리한다.
- result와 다음 preedit가 한 메시지에 있으면 result를 보존하고 다음 preedit만 조합 상태로 둔다.

계약 근거: [WM_IME_COMPOSITION](https://learn.microsoft.com/en-us/windows/win32/intl/wm-ime-composition), [IMM 문자열 조회](https://learn.microsoft.com/en-us/windows/win32/api/imm/nf-imm-immgetcompositionstringw), [GCS 값](https://learn.microsoft.com/en-us/windows/win32/intl/ime-composition-string-values).

### Web

[BrowserTextComposition](../../../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.text-composition.ts)은 조합 시작 시 선택 교체 영역 양쪽의 문서를 저장한다. 전체 조합 범위를 커서와 독립적으로 계산하고, 이벤트 데이터가 현재 DOM과 일치할 때 부분 확정을 반영한다. `compositionupdate`가 DOM 변경 전에 와도 이전 문자열과 다음 범위를 섞어 발행하지 않는다. `input` 또는 후속 microtask에서 동기화하며, clear/reattach가 이전 callback을 무효화한다.

조합 데이터가 없어도 양쪽 문서로 범위를 유지하고, 그 문서가 일치하지 않으면 추측한 범위를 보내지 않는다. 기존 selection direction·중복 억제·stale acknowledgement 거절·조합 중 formatter/value/selection 쓰기 방지는 유지했다. [UI Events 조합·입력 순서](https://www.w3.org/TR/uievents/#events-compositionevents)와 [Chrome 브라우저 조합 명령](https://chromedevtools.github.io/devtools-protocol/tot/Input/#method-imeSetComposition)을 확인했다.

## 확인 결과

| 검사 | 결과 | 증거 범위 |
| --- | --- | --- |
| Windows MAUI Testbed Release x64 | 빌드 PASS, 경고 0·오류 0 | `net10.0-windows10.0.19041.0`, `win-x64`, `Optimize=true`. `PublishAot` 미설정. `PublishReadyToRun=true`는 평가된 설정이며 이번 `build`를 publish/R2R/AOT 실행 증거로 사용하지 않음. |
| 실제 WinUI Entry/Editor 편집 | 16개 시나리오, 60회 편집, callback 68개, 불일치 0 | 각 호스트에서 반복 문자·공백·emoji 삽입/삭제·선택 교체, 12회 연속 삽입과 12회 삭제. native SelectedText/Select 합성 명령. 반응하는 실제 창 관측. |
| Windows App SDK native Release | PASS | MSVC v145, Windows SDK 10.0.26100.0, warning-as-error 빌드. |
| production C++ IMM 상태 | 16개 PASS | cancel/END/commit/cursor-only/빈 preedit/선택 보존/result+preedit/UTF-16/조회 실패/역방향 조합 범위. OS IMM context나 실제 메시지를 실행한 검사는 아님. |
| Web host Release | PASS, 경고 0·오류 0 | strict TypeScript와 managed host 빌드. 아래 브라우저가 Release에서 생성한 JS와 새 모듈을 로드함. |
| WebKit 27.2 | 21개 PASS | 실제 DOM에 합성 composition/input·selectionchange를 전달. Windows WebKit이며 Apple Safari 검사 아님. |
| Chrome 155.0.8059.86 | 22개 PASS | 공통 DOM 검사와 CDP browser composition의 확정·취소 2개 흐름. 조합 중간 커서와 cursor-only도 확인. |
| Python 검사 스크립트 구문 | PASS | 세 새 runner의 py_compile. |
| Windows 앱 정상 종료 | notVerified | probe는 window.Close 호출. 이후 5초 내 프로세스 종료가 완료되지 않아 runner가 자신이 생성한 프로세스를 종료함. 결과의 `processCleanup=runner-terminated-after-probe`, `processExitCode=1`은 입력 실패가 아닌 runner 정리 결과임. 정상 종료 PASS로 사용하지 않음. 원인은 별도 확인 필요. |

Web 공통 검사는 전체 조합 범위와 커서의 분리, 시작/끝 cursor-only, backward selection, pre-DOM update, 문자열 축소, emoji, 조합 데이터 부재, 부분 확정, 선택 교체, 취소, 반복 주변 문자, formatter/stale 응답, 중복 억제, clear/reattach, 새 field, 12회 입력/삭제와 input sequence 증가를 확인한다. 렌더 worker는 대체했고 Windows WebKit의 없는 transferable-canvas 기능은 DOM fixture에서만 대체했다. 렌더링·실제 managed worker 왕복의 증거가 아니다.

Windows 합성 편집 시작→callback 관측은 전체 68개에서 4.859–1,188.581ms, 중앙값 6.828ms였다. 최초 입력과 준비 비용을 포함한다. 각 12회 연속 입력/삭제 그룹의 중앙값은 5.910–6.830ms, 최댓값 20.718ms였다. probe 자체의 조회와 앱 작업을 포함한 시간이며 입력 callback 본문의 CPU 시간, 물리 키 입력→표시 지연, FPS 또는 프레임 페이싱을 뜻하지 않는다. 입력 지연 개선의 통과 판정으로 사용하지 않는다.

## 재실행

저장소 루트에서 실행한다. `windows_text_input.py`와 브라우저 검사는 새 output 디렉터리를 요구한다. 아래 `rerun` 경로는 실행할 때마다 새 이름으로 바꾼다. Playwright Python은 이번 실행에 사용한 로컬 환경이며, 다른 환경에서는 Playwright와 WebKit/Chrome을 준비한 Python으로 지정한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release -p:Platform=x64 -v:minimal
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/windows_text_input.py --output temp/testing/windows-web-text-input/maui-rerun
python Doroti/eng/run-with-timeout.py --timeout 1200 pwsh -NoProfile -File packages/platforms/windowsappsdk/eng/build-hwnd-exact-cpp-native.ps1
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/windows_ime_contract.py
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build packages/platforms/web/Doroti.Host.Web/Doroti.Host.Web.csproj -c Release -v:minimal
$playwrightPython = 'temp/testing/modal-navigation-20261021/venv/Scripts/python.exe'
python Doroti/eng/run-with-timeout.py --timeout 1200 $playwrightPython Doroti/tests/web_text_input_browser.py --js-root packages/platforms/web/Doroti.Host.Web/obj/Release/net10.0/Doroti.Web/wwwroot --output temp/testing/windows-web-text-input/browser-rerun
```

최종 원시 결과는 `temp/testing/windows-web-text-input/maui-final/result.json`, `browser-release/result.json`, `native-contract/`에 있다. 최종 빌드 로그는 `temp/testing/text-input-maui-build-6.log`, `text-input-native-build-2.log`, `text-input-web-host-build.log`다. 이들은 삭제 가능한 로컬 산출물이며, 삭제 후에도 위 결과와 아래 해시는 추적 문서에 남는다.

## 검증 입력 해시 (SHA-256)

| 대상 | 해시 |
| --- | --- |
| 공통 MauiTextInputBridge.cs | `9c4ffa8471322d60008462692f87f3fa23b4ffe796ab0f9dd19469c3d6b0b6e6` |
| MauiTextInputBridge.Windows.cs | `a058d2ef4062e1abd2d6a5e3c540fadf70184cd6b86b17fd9f0cf0ac4dade255` |
| Windows native exports.cpp | `19f7699376d60923e751b938e1198df9f28eab1984c616d27394c058cab4b8af` |
| ime_editing_state.h | `eecdc5375ac05164c42b25cc5dc5b6c214ccbcce21e4d598a09f146aadd84c15` |
| native Release DLL | `b1357ffc8a0b4cc6f40aa2c436b68e58d892d2d425eb2e6ae7c2354b27025813` |
| 실행된 Doroti.Host.Maui.dll | `c9a43c74b73d1ce0c69ea7cbcd31a8f3109ada6509f88bfe659169f3683a5e72` |
| 실행된 Testbed managed assembly | `3185dbb24c545637cd572dfdff1cff4ca5819c3f89fc6d6543da1c5edc165f23` |
| Web host doroti.web.ts | `95244a78c9f32e442872f5f3e960916ef6d1f302172865bb06a5b12263835194` |
| doroti.web.text-composition.ts | `7363895db9b05c99226ccf7d7a0461069750fd17a9e4ad6fc6250c5f634d50ed` |
| 브라우저가 로드한 Release doroti.web.js | `088ced934d363acf23791521b1935748d1a35c52ce7b4e0dae2eff9583b89ad0` |

## 남은 검증

실제 Windows 한국어·일본어·중국어 IME의 취소·커서·지연 managed 상태 왕복, MAUI 보안 입력 fallback, OS IME를 사용하는 Chrome/WebKit과 Apple 기기 Safari, 실제 managed worker/formatter/붙여넣기 왕복, 입력→화면 표시 시간과 프레임 페이싱, Windows 앱 정상 종료를 확인해야 한다. Apple R1·Qt R4의 구현은 별도 작업이다.
