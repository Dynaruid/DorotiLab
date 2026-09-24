# Windows MAUI 창 크기 변경 작업 인계

작성일: 2026-09-24

## 최신 결과 — 2026-09-24 23:10 KST

**사용자 선택에 따라 Windows 기본 제목 표시줄로 전환했다. 검은 띠와 제목 표시줄
위치 불일치는 최종 캡처에서 0이었다. 콘텐츠 가장자리의 순간 배경 노출은 남아
있으므로 전체 화면 연속성 판정은 PARTIAL이다.** 아래 22:15 결과는 수정 전 기록이다.

### 원인 비교와 최종 수정

WindowsAppSDK raw 호스트는 HWND를 처음부터 `WS_EX_NOREDIRECTIONBITMAP`으로 만들고
Windows 기본 non-client 제목 표시줄을 사용한다. MAUI는 WinUI가 만든 top-level
redirection bitmap과 별도로 렌더링되는 custom caption을 가지고 있었다. 현재 실행의
차이는 단순한 SDK 패키지 버전 차이보다 **창 생성·합성 표면·제목 표시줄 소유 구조**에
있다. 생성 후 exstyle 변경은 실제 적용되지 않았으므로 해결책으로 사용하지 않는다.

- **검은 띠:** 전용 root composition 출력에서 사용하지 않는 WinUI redirection
  bitmap의 alpha가 무시되는 경로를 수정했다. 새 `WindowsRootRedirection`은
  `DwmEnableBlurBehindWindow`에 빈 region을 전달해 alpha를 존중하도록 설정한다.
  기존 `SystemBackdrop`이 Acrylic을 계속 담당한다. GDI 덧칠·색상 키·앱 픽셀의
  강제 불투명화는 하지 않는다. region은 즉시 해제하고, composition 변경 시 다시
  적용하며 HWND 연결 해제 시 정책도 해제한다. 전용 window content 소유 경로로
  범위를 제한했다.
- 같은 93개 캡처 대조에서 alpha 정책 OFF는 검은 가장자리 **6프레임**, ON은
  **0프레임**이었다. 사용자도 이후 검은 띠가 보이지 않는다고 확인했다. 이 대조는
  이번 환경의 원인 근거이며 모든 WinUI 구성에 대한 일반적인 결론은 아니다.
- **제목 표시줄 시차:** `OnPlatformWindowSubclassed`에서 `ResetToDefault()`와
  `ExtendsContentIntoTitleBar=false`를 적용한다. MAUI 10.0.90의
  `NavigationRootManager` 생성 전에 실행해야 XAML 제목 행이 만들어지지 않는다.
  뒤늦게 reset하면 native caption과 기존 MAUI 행이 중복되는 것을 확인했다.
  이후 custom caption 색상 설정도 생략하고, 앱 제목과 Windows dark-mode만 반영한다.
- 사용자가 “Windows 기본 제목 표시줄로 동기화 우선”을 선택했다. 제목 표시줄은
  Windows 기본 외형이며 투명 Acrylic 제목 표시줄은 아니다. **클라이언트 Acrylic은
  유지**한다. 기본 caption에서는 콘텐츠 위쪽 inset이 0이 되고 입력도 client
  좌표를 사용한다. 이 정책은 전용 `DorotiMauiWinUIApplication`의 native composition
  경로에 적용하며 일반 embedded MAUI 창의 caption을 변경하지 않는다.

공식 근거: [DwmEnableBlurBehindWindow](https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmenableblurbehindwindow)는
alpha 처리와 region 수명, composition 변경 시 재적용을 설명한다. Windows 8 이후
이 API 자체는 실제 blur 효과를 만들지 않는다. 생성 순서는 고정 버전
[MauiWinUIWindow](https://github.com/dotnet/maui/blob/10.0.90/src/Core/src/Platform/Windows/MauiWinUIWindow.cs)와
[NavigationRootManager](https://github.com/dotnet/maui/blob/10.0.90/src/Core/src/Platform/Windows/NavigationRootManager.cs)를 확인했다.

### 최종 검증과 한계

모든 명령은 repository 20분 제한을 적용했다. 기본 Release 출력으로 화면 검사를
수행하고, 마지막에 `Platform=x64` Release 출력도 같은 소스로 다시 빌드했다.

| 검사 | 최종 결과 |
| --- | --- |
| 기본 / x64 Release 빌드 | PASS, 각각 경고 0 / 오류 0 |
| 8방향 고밀도 DXGI 캡처 | 636프레임, 검은 띠 검출 0, 정상 종료 |
| 방향별 프레임 | 오른쪽 76, 아래 80, 오른쪽 아래 80, 왼쪽 77, 위 83, 왼쪽 위 75, 오른쪽 위 77, 왼쪽 아래 88 |
| 별도 오른쪽·아래쪽 화면 검사 | **FAIL**, 81프레임: 검은 띠 0, 흰 띠 0, 제목 소실 0, caption 위치 불일치 0, 배경 노출 6, 콘텐츠 위치 불일치 6 |
| 24회 크기 변경·제안 취소 | PASS, 준비 41, timeout 0, Graphite device 1, 최종 934 × 629 |
| 준비 시간 p95 | 14.767 ms, 표시 지연 지표가 아님 |
| OS 오른쪽·왼쪽 위 드래그 | PASS, sizing 14회, 최대화·복원·종료 PASS |
| Acrylic·클릭 | PASS, OFF 배경 반응 RGB (0,0,0), ON (84,14,102), 정상 종료 |
| 경계 검출 회귀 검사 | PASS, 4개 |

`capture-resize-frames.py`는 드래그 중에는 PNG 저장·픽셀 분석을 하지 않고 제한된
메모리에 캡처를 모은 뒤 **획득한 모든 프레임**을 PNG와 metadata로 저장한다.
기본 검사는 8방향이다. 검은 연결 영역을 검사하며 정상적인 검은 native caption과
마우스 커서 주변은 분리한다. 순수 검정에 가까운 띠를 찾는 검사이므로 임의의 어두운
색상이나 모든 시각적 이상을 검출하는 것은 아니다.

636프레임은 물리 디스플레이의 모든 scan-out을 뜻하지 않는다. 최초 획득의 누적
43프레임은 최초 획득 전 누적이며 따로 기록한다. 드래그 중 DXGI 합쳐진 업데이트는
방향별 0/2/4/9/7/6/9/4개였다. 따라서 “한 프레임도 빠짐없이 검증”했다고 주장하지
않는다. 캡처 도중 전면 창을 확인하고 테스트 창만 조작한다.

Raw WindowsAppSDK도 동일한 엄격한 검사에서는 87프레임 중 검은 띠 0, caption
불일치 0이었지만 배경 노출 5와 콘텐츠 위치 불일치 17이 관측됐다. **남은 콘텐츠
경계 시차를 MAUI만의 검은 띠 문제와 혼동하지 않는다.** 전체 gate는 완화하지 않았고
실패를 그대로 남겼다. 실제 손 드래그 체감, 물리 scan-out, 혼합 DPI, touch/pen,
접근성 및 device loss는 이번 실행에서 `notVerified`다.

### 증거와 재실행

삭제 가능한 `Doroti/artifacts/validation/windows-maui/` 아래 실행 기록:

- `20260924-222926` / `223010`: alpha ON/OFF 대조, 검은 띠 0 / 6.
- `20260924-223443`: raw WindowsAppSDK 화면 검사.
- `20260924-230710`: 최종 8방향 636 PNG와 `dense-result.json`, host DLL SHA256
  `4df0764fe577d38a75f2fdafe34254c751c4633919dc7d70f66da36b25a65e4d`.
- `20260924-230818`: 최종 실패 상태를 유지한 화면 검사와 native timeline.
- `20260924-230835`, `230844`, `230852`: 크기 변경, OS 드래그, Acrylic 통과.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release
$env:DOROTI_MAUI_VALIDATION_EXE=(Resolve-Path 'DorotiTestbedApp/windows/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe').Path
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/capture-resize-frames.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-flicker.py
```

다음 미해결 범위는 콘텐츠와 바깥 경계의 순간 시차다. 검은 띠·caption 전환은 적용했지만
전체 리사이즈 완료를 선언하지 않는다. 커밋·배포하지 않았다.

## 이어서 진행한 결과 — 2026-09-24 22:15 KST

**PARTIAL. 콘텐츠 위치 불일치는 이번 최종 캡처에서 관측되지 않았지만,
닫기 버튼 위치 불일치와 축소 중 오른쪽 검은 띠가 남아 있다. 완료로 판정하지 않는다.**

### 이번에 수정한 사항

- `WindowsNativeCompositionOutput.Present`에서 크기 변경마다 수행하던
  `DwmFlush`를 제거했다. 제출이 중첩된 `WM_SIZE` 안에서 실행되는 동안
  DWM을 기다리면 HWND/WinUI의 바깥쪽 geometry 처리가 끝나기 전에 새 콘텐츠가
  표시될 수 있었다. 사전 프레임 준비와 compositor-clock 정렬은 유지한다.
- GPU 자원 보호용 fence 대기는 유지하며, UI STA에서 메시지를 재진입시킬 수 있는
  CLR `WaitOne`을 5초 제한의 kernel wait로 바꿨다. GPU 완료 실패를 자원 해제
  허가로 취급하지 않는다.
- 화면 검사에서 **제목 표시줄 채움색을 창 오른쪽 경계로 오인하던 오류**를 고쳤다.
  실제 캡처의 채움색 `(187,43,43)`과 배경색 `(192,48,48)`의 차이가 5였는데,
  기존 검사는 허용 오차 6으로 화면 캡처의 끝을 찾았다. 보정 후 경계가 초기
  935 px에 고정되어 이전의 최대 72 px 오차가 과장됐다. 이전의 36/100 수치는
  올바른 창 경계 기준의 결과가 아니므로 최신 결과와 직접 비교하지 않는다.
- 이제 기준 프레임의 DWM 2픽셀 테두리를 찾고, 기준점 오류·테두리 누락·중복은
  `notMeasured`/exit 1로 거부한다. 닫기 버튼 중심을 따로 검사하며, 드래그 종료
  직후 프레임도 콘텐츠 검사에 포함한다. 테스트 창의 전면 여부와 실제 크기 변화도 확인한다.
- 선택적으로 `DOROTI_WINDOWS_RESIZE_TIMELINE` 파일에 최대 8,192개 이벤트를 기록한다.
  `WM_SIZING`, `WM_WINDOWPOSCHANGING`, `WM_WINDOWPOSCHANGED`, `WM_SIZE`의 입출구,
  HWND/client/제목 표시줄 자식 HWND 좌표, Present 입출구와 버퍼 크기·generation을
  QPC로 남긴다. 캡처의 CPU 관측 구간과 연결하는 분석 스크립트도 추가했다.
  **API 반환과 CPU 캡처 시각은 물리 표시 완료 시각이 아니다.**

### 최종 검증

동일한 기본 Release 실행 파일을 사용했고, 모든 검증 명령에 20분 제한을 적용했다.

| 검사 | 이번 결과 |
| --- | --- |
| 기본 Release 빌드 | PASS, 경고 0 / 오류 0 |
| 경계 검출 회귀 검사 | PASS, 4개: 유사 배경색, 누락, 잘못된 기준점, 중복 |
| UI dispatch 계약 | PASS |
| 24회 크기 변경·제안 취소 | PASS, 준비 41회, 타임아웃 0, 최종 콘텐츠 934 × 623 |
| 장치/스왑체인 | Graphite 1개, 연결 1회, 통상 resize-buffer 0회 유지 |
| 사전 준비 시간 p95 | 14.093 ms, 표시 지연 지표가 아님 |
| OS 오른쪽·왼쪽 위 드래그 | PASS, WM_SIZING 준비 14회 |
| 최대화·복원·종료 | PASS |
| **오른쪽·아래쪽 화면 검사** | **FAIL, 97개 캡처: 제목 소실 0, 콘텐츠 위치 불일치 0, 오른쪽 검은 띠 6, 닫기 버튼 위치 불일치 11** |
| Acrylic·클릭 재검사 | **notMeasured**, 테스트 앱이 전면을 유지하지 못해 중단 |
| GDI 교차 캡처 | **notMeasured**, 유효한 초기 테두리를 얻지 못함 |

닫기 버튼 중심 오차는 -24 / +24 physical px였다. CPU 타임라인에는 예를 들어
client/버퍼 폭 959와 새 caption child 좌표가 관측되지만 캡처된 테두리 폭은 935인
구간이 있다. 이 관측은 HWND 좌표 적용·Present 호출과 표시된 경계를 구분해야 함을
보여주며, DWM 내부의 정확한 원인을 확정하지는 않는다.

올바른 테두리 검출을 적용한 대조 실행에서는 대기 제거 전 103개 캡처 중 콘텐츠
위치 불일치 16개, 검은 띠 6개였다. 제거 후 최종 결과는 위 표와 같다. 표본 수와
타이밍이 다른 제한된 실행이며, 모든 방향·혼합 DPI·실물 체감의 해결 증거는 아니다.

유지하지 않은 비교 실험: 추가 DirectComposition commit, 강제 XAML UpdateLayout,
사전 준비 대기 생략, geometry 이후 WM_WINDOWPOSCHANGED에서 제출, 사전 DwmFlush,
기존 XAML 표면 경로와 입력 자식 HWND에 출력 연결. 남은 표시 문제를 해결하지 못해
실험 분기와 옵션을 제거했다. 생성 후 `WS_EX_NOREDIRECTIONBITMAP` 변경도 실제 style이
`0x100`으로 남았으므로 효과를 주장하지 않는다.

### 증거와 다음 작업

삭제 가능한 로컬 산출물은 `Doroti/artifacts/validation/windows-maui/` 아래에 있다.

- `20260924-220052`: 수정된 경계 검사 + DwmFlush 유지 대조 실행.
- `20260924-221109`: 최종 실패한 화면 검사, PNG, `timeline.json`,
  `timeline-correlation.json` (97개 캡처와 458개 이벤트 연결).
- `20260924-221128`: 크기 변경·취소 통과.
- `20260924-221148`: OS 드래그·최대화/복원 통과.
- `20260924-221526`: Acrylic 전면 조건 실패, `status=notMeasured`.

다음 작업은 **WinUI caption/바깥 경계와 콘텐츠를 함께 표시할 수 있는 창 소유 구조**의
검증이다. 기존 WindowsAppSdk raw HWND의 준비/제출 소스는 비교했지만 이번에는 그
실행 파일의 동일 화면 검사를 수행하지 않았다. 단순 대기 위치 변경이나 별도
DirectComposition commit 추가는 해결책으로 확인되지 않았다.
[DirectComposition의 Commit 문서](https://learn.microsoft.com/en-us/windows/win32/api/dcomp/nf-dcomp-idcompositiondevice-commit)도
서로 다른 device의 batch가 같은 시점에 적용됨을 보장하지 않는다.
실물 드래그·scan-out·혼합 DPI·touch/pen·접근성·device loss는 계속 `notVerified`다.
커밋하거나 배포하지 않았다.

추가 명령:

```powershell
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/test-flicker-geometry.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-flicker.py
# 화면 검사 종료 뒤, 출력된 증거 디렉터리를 지정한다.
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/analyze-timeline.py Doroti/artifacts/validation/windows-maui/20260924-221109
```

아래는 이전 인계 기록이다. 화면 관련 수치에는 위에서 확인한 경계 검출 오류가 포함된다.

## 이전 상태: PARTIAL — 크기·위치가 앞뒤로 튀는 현상은 미해결

사용자가 확인한 증상은 모든 방향에서 발생하는 내부 레이아웃의 튐이며,
닫기 버튼도 순간적으로 오른쪽으로 밀려 보인다. 이번 화면 캡처에서도
창 외곽과 내부 내용의 표시 크기가 어긋나는 프레임이 남았다.
**빌드·최종 크기·종료 검증 통과를 시각적 문제 해결로 해석하면 안 된다.**

사용자의 외출 및 작업 마무리 요청에 따라 현재 변경과 검증 결과를 남긴다.
추가 사용자 입력을 기다리는 작업이나 계속 실행 중인 검증 작업은 없다.

## 적용된 변경

- 기본 MAUI 전체 창의 Doroti 렌더링을 HWND에 연결한 DirectComposition/DXGI
  경로로 분리했다. 일반적인 embedded `DorotiMauiSurface`는 기존 XAML 경로를 유지한다.
- 첫 수정의 두 스왑체인 교대 연결을 제거했다. 현재는 모니터 크기를 확보한
  **하나의 표시 스왑체인을 유지**하고 백 버퍼를 갱신한다. 통상적인 크기 변경에서
  `SetContent` 재연결이나 `ResizeBuffers`를 반복하지 않는다.
- 모든 방향에서 예정 크기의 프레임을 먼저 준비하고 실제 HWND 크기가 적용된 뒤
  제출하도록 변경했다. `WM_WINDOWPOSCHANGING`에서 compositor clock을 기다려
  창 변경과 화면 제출 시점을 맞추는 경로를 추가했다.
- UI 대기 중에는 Doroti 소유 작업만 처리한다. CLR STA 대기의 메시지 재진입을
  피하도록 native kernel wait를 사용한다. 제안 크기 취소·불일치는 generation 검사로 처리한다.
- 직접 연결된 표면은 WinUI visual hit test 밖에 있으므로 기존 WinUI 입력 HWND에
  마우스 및 touch/pen 입력을 연결했다. 마우스 좌표에서 제목 표시줄 높이를 제외한다.
  WinUI의 focus/IME/accessibility 구성은 유지하지만 실물 검증까지 완료된 것은 아니다.

주요 파일:

- [표시 표면](Doroti/src/Doroti.Host.Maui/WindowsNativeCompositionOutput.cs)
- [크기 변경 및 창 메시지](Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs)
- [UI 작업 대기](Doroti/src/Doroti.Host.Maui/WindowsResizeDispatchQueue.cs)
- [마우스 입력](Doroti/src/Doroti.Host.Maui/WindowsRootMouseInput.cs)
- [Skia와 표시 경로 연결](Doroti/src/Doroti.Host.Maui/WindowsCompositionSurfacePresenter.cs)

## 이전 실행의 검증 결과 — 화면 수치는 위 정정 참고

로컬 NVIDIA RTX 4060 Laptop GPU, 200% DPI 기준이다.

| 검사 | 결과 |
| --- | --- |
| 사용자 명령과 같은 기본 Release 출력 빌드 | PASS, 경고 0 / 오류 0 |
| 24회 변경 및 제안 크기 취소 | PASS, 준비 타임아웃 0, 최종 934 × 623 |
| 장치 및 표시 표면 | Graphite 장치 1개, `nativeContentAttachments=1`, `nativeSwapChainResizes=0` |
| compositor clock 관측 | 24회 ready |
| 준비 시간 p95 | 16.346 ms; 물리 표시 지연이 아님 |
| OS 마우스로 오른쪽·왼쪽 위 드래그 | PASS, 13회 `WM_SIZING` 준비 관측 |
| 최대화·복원·종료 | PASS |
| 마우스 클릭·Acrylic 배경 반응 | PASS, opaque RGB 변화 `(0,0,0)`, enabled `(84,14,102)` |
| UI dispatch 계약 | PASS |
| **드래그 중 표시 크기·위치 일치** | **FAIL: 100개 DXGI 캡처 중 36개에서 위치 불일치 검출** |

최종 표시 검사에서는 제목이 사라지는 프레임과 오른쪽 배경 노출은 각각 0개였다.
그러나 오른쪽 드래그 30개 캡처 중 24개, 아래쪽 드래그 57개 중 12개에서
내부 제목 또는 하단 바 위치가 표시된 창 경계와 맞지 않았다.
관측된 제목 중심 오차는 최대 72 physical px, 하단 바 오차 범위는 -24~12.5 px였다.
이는 해당 테스트의 픽셀 landmark 측정치이며 모든 창·테마에 일반화한 성능 지표가 아니다.

추가 Enter 버튼 활성화 검사는 수정 전 바이너리와 수정 후 모두 실패했다.
키보드 활성화/IME 개선을 이번 작업의 통과 항목으로 보고하지 않는다.
물리 드래그 체감, scan-out, 혼합 DPI, 실물 touch/pen, 접근성, device loss는 `notVerified`다.

## 남은 문제와 다음 조사 지점

`GetClientRect`에서 관측한 새 크기와 DWM 캡처에서 실제 보이는 외곽 크기가
같은 시점에 바뀌지 않는다. 제목 표시줄/닫기 버튼은 WinUI가, 콘텐츠는 Doroti가
갱신하므로 프레임 준비 완료만으로 두 영역의 동시 표시를 증명할 수 없다.

1. `WM_WINDOWPOSCHANGING` → `WM_WINDOWPOSCHANGED` → `WM_SIZE`와 WinUI 제목 표시줄
   자식 HWND, 실제 표시 프레임의 크기를 같은 타임라인에 연결해야 한다.
2. 최종 표시 주체를 통합하는 설계가 필요할 수 있다. 기존 WindowsAppSdk의 raw HWND
   shell/준비 프레임 프로토콜과 비교해야 하며, 렌더러만 바꾸는 것으로 완료 처리하지 않는다.
3. `DwmFlush` 성공, timeout 0, 프레임 generation 일치는 시각적 완료의 대체 지표가 아니다.
   아래 화면 검사를 통과시키고 실제 사용자 드래그에서도 확인해야 한다.

시도 후 남기지 않은 변경:

- MAUI HWND에 `WS_EX_NOREDIRECTIONBITMAP`을 추가하는 실험: 이후 검사에서
  extended style이 계속 `0x100`으로 관측됐다. 유지되지 않는 이유는 확정하지 않았고
  효과를 증명하지 못해 해당 스타일 변경을 제거했다.
- 제목 표시줄 `ResetToDefault()` 실험: 표시 불일치가 남아 원래 색상 설정으로 복원했다.

## 실행 및 재현

```powershell
dotnet run --project ./DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release
```

검증은 저장소 루트에서 실행한다. 모든 검증에 20분 process-tree 제한을 사용한다.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release
$env:DOROTI_MAUI_VALIDATION_EXE = (Resolve-Path DorotiTestbedApp/windows/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe).Path
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-resize.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-drag.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-acrylic.py
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/windows-maui/ResizeDispatch -c Release
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-flicker.py
```

**마지막 `verify-flicker.py`는 현재 실패하는 회귀 검사다.** 실패를 숨기지 않고 기본 exit 1로
보고한다. 단순 관측만 할 때는 `DOROTI_FLICKER_OBSERVE_ONLY=1`을 설정할 수 있지만,
JSON의 `status=failed`는 그대로 유지된다. 이 모드를 수용 검사 통과로 사용하지 않는다.

캡처 의존성은 전역 Python 환경 대신 삭제 가능한 artifact 디렉터리에 설치했다:

```powershell
python -m pip install --target Doroti/artifacts/validation/capture-python dxcam==0.3.0
```

검사들은 자신이 실행한 프로세스만 조작하고 닫는다. 드래그·화면 검사는 마우스를
사용하고 위치를 복원한다. 화면 검사는 통제된 배경 창을 잠시 연다.

최종 로컬 증거는 `Doroti/artifacts/validation/windows-maui/` 아래에 있다:

- `20260924-112345`: 크기 변경 프로토콜
- `20260924-112406`: **실패한 표시 일치 검사**, `flicker-result.json`과 대표 PNG
- `20260924-112438`: OS 드래그·최대화/복원
- `20260924-112500`: Acrylic·클릭

이 디렉터리들은 정책상 삭제 가능한 산출물이다. 핵심 결과는 위 표에 보존했다.
변경은 현재 작업 트리에 남아 있으며 커밋하거나 배포하지 않았다.
