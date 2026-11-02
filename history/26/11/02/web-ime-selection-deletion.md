# Web 한글 조합 중 선택 삭제 수정

날짜: 2026-11-02 (Asia/Seoul)

시작 HEAD: `03b654ae17a243728c896e53ff649a3e19d89700`, 시작 작업 트리 깨끗함.

## 원인과 수정

사용자가 확인한 환경은 Android Chrome에서 실행한 Web이다.
[Web 입력 호스트](../../../../packages/platforms/web/Doroti.Host.Web/Web/doroti.web.ts)의
`setTextInputState`가 IME 조합 중에는 framework의 선택 변경도 모두 무시했다.
모바일의 선택 표시는 framework가 그리지만 소프트 키보드는 숨겨진 DOM textarea의
선택을 사용하므로 두 범위가 달라졌다. 한글 자체의 문자열 삭제 연산 문제가 아니다.

IME 조합 중에도 **현재 DOM과 문자열이 같고 선택이 달라진 상태**는 native 선택에 반영한다.
기존 input-sequence 검사, 조합 중 문자열 덮어쓰기 방지, 동일 선택 재할당 억제,
클라이언트 연결 전 들어온 native 편집 보호는 유지한다.
선택을 적용한 뒤 실제 DOM 상태와 조합 범위를 발행한다.

삭제를 keydown에서 별도로 구현하지 않았다. Gboard 삭제는 `Unidentified` 키와
`beforeinput`/`input`으로 들어왔으며, 조합 중에는 `insertCompositionText`로도 전달됐다.
이벤트 종류에 관한 근거는 [W3C Input Events](https://www.w3.org/TR/input-events-2/)다.

## 실제 Testbed 재현과 결과

환경: Pixel 5 AVD, Android 13 / x86_64, Chrome `109.0.5414.123`,
Gboard `12.4.05.482060964-preload-x86_64`, 한국어 두벌식.
Release Testbed를 `dotnet run --no-build`로 실행했으며 runtime owner는 `main`,
renderer는 `worker-direct-webgl`이었다. AOT publish를 실행한 결과는 아니다.

Gboard의 `ㅎㅏㄴㄱㅡㄹ` 버튼을 실제로 탭해 `한글`을 입력하고, framework 텍스트를
길게 눌러 전체 선택한 다음 Gboard 삭제 버튼을 탭했다.
이 비교에서는 문자열을 DOM에 대입하거나 합성 composition 이벤트로 준비하지 않았다.

| 실행 | 화면/framework 선택 | IME textarea 선택 | 삭제 결과 |
| --- | --- | --- | --- |
| 수정 전 InputLifetimeSample | `[0,2)` | `[2,2)` | `한그`로 바뀜: 실패 |
| 수정 후 InputLifetimeSample | `[0,2)` | `[0,2)` | 빈 문자열: 통과 |
| 수정 후 기본 Components → Text fields → Filled | `[0,2)` | `[0,2)` | 빈 문자열: 통과 |

기본 Components의 `_filled` 컨트롤러를 공유하는 일반·오류·비활성 필드에도 삭제가
반영됐으며 `_outlined` 필드는 빈 상태를 유지했다. 입력·선택·삭제와 화면을 관측했다.
InputLifetimeSample의 미등록 `doroti/native-editor` PlatformView 오류는 별도로 발생했고,
기본 Components 확인에서는 해당 오류가 발생하지 않았다.

추가 production DOM fixture에서는 `앞한글뒤`의 `[0,3)` 선택을 요청했다.
수정 전에는 실제 선택이 `[3,3)`에 남아 삭제 후 `앞한뒤`, 수정 후에는 `[0,3)`이
적용돼 `뒤`가 됐다. 이 선택 불일치는 데스크톱 Chrome에서도 재현됐다.
따라서 Android 전용 오류가 아니라 Web 공통 입력 호스트 오류다.

## 자동 검사와 범위

| 검사 | 결과 / 범위 |
| --- | --- |
| Testbed Web Release 빌드 | 경고 0, 오류 0. strict TypeScript 포함 |
| Chrome `156.0.8078.79` | production DOM 검사 32개 통과. 추가 선택 검사에는 CDP browser composition 사용 |
| Windows WebKit `27.2` | production DOM 검사 32개 통과. 합성 composition 이벤트와 실제 DOM 키 삭제 사용 |
| Android Chrome + Gboard | 위 실제 Testbed 두 화면의 입력·길게 선택·소프트 키보드 삭제 통과 |
| 제공된 Web JS 확인 | 브라우저가 로드한 fingerprint 자산과 Release 생성 JS SHA-256 일치 |
| `git diff --check` | 통과 |

32개 검사는 기존 전체 조합 범위·부분 확정·취소·중복 억제·clear/재연결과,
추가 정방향/역방향/전체/조합 내부/이모지 UTF-16/Latin 조합 선택 삭제,
동일 선택 무재할당·오래된 선택 거절·formatter 문자열 보호·조합 중 커서 이동·확정 한글 삭제를 포함한다.
연속 입력/삭제 검사는 각 12회다. 모든 빌드·검사 명령은 1,200초 wrapper로 제한했다.

Chrome/WebKit DOM fixture의 managed callback과 렌더 Worker는 대체했다.
실제 Testbed 확인은 실제 managed 상태 왕복과 렌더러를 사용했다.
물리 기기의 사용자 Chrome/Samsung Keyboard, Apple Safari/iOS 실기기,
네이티브 플랫폼의 실제 IME 및 입력 성능은 이번 검증 범위에 포함되지 않는다.

## 다른 플랫폼 검토

| 경로 | 현재 소스 확인 |
| --- | --- |
| 공통 EditableText / Material / Cupertino | native의 전체 편집 값과 선택을 반영하는 공통 경로. 이번 삭제 불일치는 Web endpoint 동기화에서 발생 |
| Android MAUI | `UpdateStateCore`가 변경된 선택을 `EditText.SetSelection`에 반영. 동일 편집 echo만 억제하며 조합 전체를 이유로 선택을 차단하지 않음 |
| Apple MAUI | revision 검사 후 변경 선택을 UIKit/AppKit의 native 선택 API로 적용 |
| Windows MAUI / Windows App SDK | MAUI 선택 mapper 및 `ApplyTextCommand`가 변경 선택을 적용 |
| Qt | `UpdateTextState` → `ApplyTextState`가 선택을 저장하고 IME cursor/anchor/selection query를 갱신 |

이 경로들에서는 같은 조합 중 선택 일괄 차단을 찾지 못했다. 소스 검토 결과이며,
각 플랫폼의 실제 IME 무결성을 모두 확인했다는 뜻은 아니다. 제품 코드는 Web 호스트만 수정했다.

## 재현 절차와 산출물

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -v:minimal
dotnet run --project samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release --no-build
```

브라우저에서 `http://127.0.0.1:5088/?dorotiRenderer=worker-direct-webgl`을 열고
Components 아래 Text fields로 이동해 Gboard 한국어 입력 → 길게 선택 → 삭제를 확인한다.
독립 입력 장면은 같은 URL에 `&dorotiSample=input`을 추가한다.

검증 스크립트와 원시 출력은 삭제 가능한 로컬 경로 `temp/testing/android-web-ime/`에 있다.
`regression-result.json`, `testbed-android-gboard-{baseline,fixed}-{longpress,delete}.json`,
`testbed-android-components-filled-{longpress,delete}.json`과 같은 이름의 화면 PNG,
`testbed-asset-receipt.json`에 결과를 기록했다.
빌드 로그는 `temp/testing/android-web-ime-{baseline,fixed}-build.log`다.

- 수정한 `doroti.web.ts` SHA-256: `64755809411688196ca546f4b816fdfae4e9fd095e79e6108f131857c55939ed`
- Release 생성 및 브라우저 제공 JS SHA-256: `2083fd83966c16ba6e8290d27eed5148fbee509546f0ad5f792695d89cb664ce`
- 실제 로드한 자산: `_content/Doroti.Host.Web/doroti.web.4j783utnrd.js`
