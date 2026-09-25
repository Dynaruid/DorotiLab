# Windows MAUI 리사이즈·Acrylic 작업 요약

원본: 삭제한 `windows-maui-resize-report.md`. 최종 기록: **2026-09-24 23:10 KST, PARTIAL**. 검은 띠와 제목 표시줄 위치 불일치는 최종 캡처에서 0이었지만, 콘텐츠와 바깥 경계의 순간 시차가 남았다. 운영 검증 통과를 전체 화면 연속성 완료로 해석하지 않는다.

## 적용 내용과 결정

- 전용 MAUI 전체 창은 HWND → DirectComposition/DXGI → Graphite-Vulkan 출력으로 분리했다. embedded `DorotiMauiSurface`는 기존 XAML 합성·layout 경로를 유지한다.
- resize 때 Vulkan device/Graphite session을 유지하고 imported D3D12 자원만 교체한다. 모니터 크기 용량의 표시 스왑체인 하나를 연결해 통상 resize의 `SetContent` 재연결·`ResizeBuffers` 반복을 없앴다.
- 모든 방향의 예정 크기 프레임을 준비한 뒤 실제 HWND geometry에 맞춰 제출한다. compositor clock 정렬, 취소·불일치 generation 거부, Doroti 소유 작업만 처리하는 UI 대기를 사용한다. STA 재진입을 피하는 kernel wait와 GPU fence 수명 보호는 유지한다.
- 중첩된 `WM_SIZE`의 Present에서 `DwmFlush`를 제거했다. 추가 commit·대기 위치 조정만으로 전체 표시 동기화가 해결되지는 않았다.
- `WindowsRootRedirection`은 빈 region을 전달하는 `DwmEnableBlurBehindWindow`로 사용하지 않는 WinUI root redirection bitmap의 alpha 처리를 보강했다. 실제 Acrylic은 기존 `SystemBackdrop`이 담당한다. composition 변경 시 재적용하고 연결 해제 시 정책을 해제한다.
- 사용자가 Windows 기본 제목 표시줄을 선택했다. `OnPlatformWindowSubclassed`에서 MAUI `NavigationRootManager` 생성 전에 `ResetToDefault()`와 `ExtendsContentIntoTitleBar=false`를 적용해 중복 제목 행을 방지했다. 이 시점에는 기본 caption 외형과 client Acrylic을 사용했다. 생성 후 `WS_EX_NOREDIRECTIONBITMAP` 변경은 적용되지 않아 채택하지 않았다.
- native 출력의 입력은 기존 WinUI 입력 HWND에 연결한다. 최종 기본 caption에서는 위쪽 content inset이 0이고 client 좌표를 사용한다. WinUI focus/IME/accessibility 소유권 유지가 실물 검증 통과를 뜻하지는 않는다.

주요 소스: [표시 표면](../../Doroti/src/Doroti.Host.Maui/WindowsNativeCompositionOutput.cs), [창 메시지·크기](../../Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs), [UI 대기](../../Doroti/src/Doroti.Host.Maui/WindowsResizeDispatchQueue.cs), [마우스 입력](../../Doroti/src/Doroti.Host.Maui/WindowsRootMouseInput.cs), [presenter](../../Doroti/src/Doroti.Host.Maui/WindowsCompositionSurfacePresenter.cs), [root alpha](../../Doroti/src/Doroti.Host.Maui/WindowsRootRedirection.cs).

## 최종 기록된 검증

로컬 NVIDIA RTX 4060 Laptop GPU, 200% DPI 환경의 제한된 관측이다. 모든 명령에 20분 제한을 적용했다.

| 검사 | 23:10 최종 결과 |
| --- | --- |
| 기본 / x64 Release 빌드 | 각각 PASS, 경고·오류 0 |
| 8방향 고밀도 DXGI 캡처 | 636프레임, 검은 띠 검출 0, 정상 종료 |
| 엄격한 오른쪽·아래쪽 화면 검사 | **FAIL**, 81프레임: 검은 띠·흰 띠·제목 소실·caption 불일치 각 0, 배경 노출 6, 콘텐츠 불일치 6 |
| alpha 정책 ON/OFF 대조 | 각 93프레임, 검은 가장자리 ON 0 / OFF 6; 사용자도 검은 띠가 보이지 않는다고 확인 |
| 24회 크기 변경·제안 취소 | PASS, 준비 41, timeout 0, Graphite device 1, 최종 934×629 |
| 준비 시간 p95 | 14.767 ms; 표시 지연 지표 아님 |
| OS 오른쪽·왼쪽 위 드래그 | PASS, sizing 14회; 최대화·복원·종료 PASS |
| Acrylic·클릭 | PASS, OFF 배경 반응 RGB (0,0,0), ON (84,14,102) |
| 경계 검출 회귀 | PASS, 4개 |
| raw WindowsAppSDK 비교 | 87프레임: 검은 띠·caption 불일치 0, 배경 노출 5, 콘텐츠 불일치 17 |

636프레임은 획득한 프레임 수이며 물리 scan-out 전체가 아니다. 최초 획득 전 누적 43프레임과 드래그 중 합쳐진 업데이트 41개를 별도로 기록했다. 검출기는 순수 검정에 가까운 연결 영역을 검사하며 정상 caption·커서를 제외한다. 모든 어두운 색상이나 시각적 이상을 검출하는 검사는 아니다. raw 호스트에서도 남은 콘텐츠 시차를 MAUI의 검은 띠 문제와 구분한다.

## 이전 측정의 정정

- 초기 **36/100 위치 불일치·최대 72 px**는 caption 채움색을 창 경계로 오인한 검사 오류가 포함되어 최신 수치와 직접 비교할 수 없다.
- 22:15 수정 검사는 DWM 2픽셀 테두리와 닫기 버튼 중심을 사용하고 기준점 오류·테두리 누락·중복을 `notMeasured`/exit 1로 거부했다. 대기 제거 전 대조는 103프레임 중 콘텐츠 불일치 16·검은 띠 6이었다.
- 22:15 결과는 **FAIL**, 97프레임 중 콘텐츠 불일치 0·검은 띠 6·caption 불일치 11이었다. Acrylic 재검사는 전면 조건 실패, GDI 캡처는 초기 테두리 실패로 각각 `notMeasured`였다. 이는 23:10 수정 전 결과다.
- QPC timeline은 최대 8,192개 창 메시지·geometry·Present 이벤트와 캡처 CPU 구간을 연결한다. API 반환·CPU 시각은 물리 표시 완료 시각이 아니다.

## 남은 범위와 재실행

콘텐츠와 창 바깥 경계의 표시 시차가 미해결이다. `verify-flicker.py`는 실패 시 기본 exit 1이며 `DOROTI_FLICKER_OBSERVE_ONLY=1`도 JSON의 `status=failed`를 유지한다. 관측 모드를 수용 검사 통과로 사용하지 않는다. 실물 손 드래그 체감, scan-out, 혼합 DPI, touch/pen, 접근성, device loss는 `notVerified`다. Enter 활성화 추가 검사는 수정 전후 모두 실패해 키보드/IME 개선의 통과 근거가 없다.

명령·도구·후속 결과는 [Windows MAUI 검증 README](../../Doroti/validation/windows-maui/README.md)를 따른다. 이 README에는 **09-25 native caption Acrylic 후속 변경**도 있으므로 09-24의 기본 caption 외형 기록과 구분한다.

```powershell
python Doroti/validation/run-with-timeout.py dotnet build DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release
$env:DOROTI_MAUI_VALIDATION_EXE=(Resolve-Path 'DorotiTestbedApp/windows/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiTestbedApp.Windows.exe').Path
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/capture-resize-frames.py
python Doroti/validation/run-with-timeout.py python Doroti/validation/windows-maui/verify-flicker.py
```

당시 삭제 가능한 `Doroti/artifacts/validation/windows-maui/` 실행 ID: `20260924-222926`/`223010` alpha 대조, `223443` raw 비교, `230710` 636프레임, `230818` 최종 실패 화면·timeline, `230835`/`230844`/`230852` 운영 검사. 636프레임 실행 host DLL SHA-256은 `4df0764fe577d38a75f2fdafe34254c751c4633919dc7d70f66da36b25a65e4d`였다. 이전 정정 증거는 `220052`와 `221109`에 기록했다. 이번 보관에서 원시 파일 잔존이나 제품 동작을 재검증하지 않았다.
