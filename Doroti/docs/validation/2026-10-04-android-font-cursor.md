# Android Fonts 여러 줄 커서 이동 수정

2026-10-04, `samples/DorotiSampleApp2`, Galaxy S25 `SM-S931N`, Android 16,
삼성 키보드 `com.samsung.android.honeyboard/.service.HoneyBoardService`에서 확인했다.
런타임은 .NET 10 / Release / CoreClrJit / android-arm64이다.

## 원인과 변경

Fonts의 `maxLines: 3` 입력은 MAUI의 숨겨진 Editor를 IME 연결점으로 사용한다.
이 Editor의 크기는 화면 커서 크기인 약 6 × 85 물리 픽셀이므로 네이티브 줄 배치가
화면의 줄 배치와 다르다. 수정 전에는 `KEYCODE_DPAD_UP`을 보내도 같은 화면 줄에서
왼쪽으로 움직였다.

- Android의 일반 위·아래 방향 키를 `RenderEditable`의 실제 줄 배치로 전달한다.
  자동 줄바꿈과 원래 열 위치를 사용하며, 문자 입력·삭제·좌우 이동은 기존 네이티브 경로를 유지한다.
  수정키가 붙은 방향 입력은 기존 경로를 유지한다.
- 나중에 생성되는 Entry/Editor에도 네이티브 키 구독을 연결하고, 구독 종료 시 해제한다.
- `VerticalCaretMovementRun`의 캐시 조회는 `TryGetValue`로 키 존재를 확인한다.
  C#의 값 형식 `MapEntry` 기본값을 캐시된 위치로 취급하던 오류를 수정한다.
- 세로 이동 Action이 정상 종료 시 `null`을 반환하도록 수정한다.
- `TextSelection.CreateCollapsed`와 `CreateFromPosition`은 생성자로 선택 끝점과
  상속받은 `TextRange.start/end`를 함께 초기화한다.

## 검증

| 항목 | 결과 |
| --- | --- |
| 전체 `Doroti.Tests` Release CPU 회귀 | PASS |
| 추가 회귀: 짧은 줄의 열 위치 유지, 문서 양끝, 자동 줄바꿈, 이동 후 한글 composing 상태 | PASS |
| CoreClrJit APK 빌드·설치·실행 | PASS, 오류 0, SDK의 기존 XA1040 경고 1 |
| 실제 Fonts 페이지에서 ADB 위·아래 방향 입력 | PASS-automated-device |
| 실제 삼성 키보드 스페이스바 길게 누름 → 위·아래 커서 제어 | PASS-automated-device-IME |
| 이동 후 삼성 키보드 `q` 입력과 Backspace | PASS-automated-device-IME |
| 해당 앱 PID의 crash 로그 | 기록 없음 |
| 손가락 직접 조작, 다른 기기·키보드, 다른 런타임 모드 | notVerified |

키보드 터치는 ADB `input motionevent DOWN/MOVE/UP`으로 주입했다.
스페이스바를 700 ms 누른 뒤 작은 MOVE로 이동 기준점을 만들고 두 번째 MOVE로
위·아래 이동을 발생시켰다. 사람의 손가락 직접 입력으로 검증한 결과는 아니다.

스크린샷에서 파란 커서의 물리 픽셀 범위를 측정했다.

| 입력 | 커서 범위 `[left, top, right, bottom]` |
| --- | --- |
| 둘째 줄에서 시작 | `[339, 657, 344, 740]` |
| 위 방향 입력 | `[358, 554, 363, 637]` |
| 아래 방향 입력 | `[339, 657, 344, 740]` |
| 삼성 키보드 좌측 이동 후 | `[298, 650, 303, 733]` |
| 스페이스바 커서 제어로 위 이동 | `[296, 554, 301, 637]` |
| 스페이스바 커서 제어로 아래 이동 | `[298, 650, 303, 733]` |
| `q` 입력 후 Backspace | `[298, 650, 303, 733]` |

원시 로그·스크린샷·SHA-256·측정 JSON은 정리 가능한
`temp/testing/android-font-cursor/2026-10-04/`에 있다. 위 표는 이 실행의 검증 기록이다.

## 재검증 명령

저장소 루트에서 실행한다. 테스트 제한 시간은 20분이다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Release -- --text-input-vertical-cursor
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Release
pwsh -NoProfile -File samples/DorotiSampleApp2/android/deploy-android.ps1 -Mode CoreClrJit
```

기기에서는 Fonts를 열어 여러 줄 필드에 커서를 놓고 삼성 키보드의 스페이스바를
길게 누른 다음 위·아래로 움직인다. 화면의 인접 줄 이동, 원래 열 위치 복귀,
이동 후 문자 입력·삭제를 확인한다.
