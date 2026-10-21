# Texture 유휴 재생 및 Windows 입력 후 정지 수정

작성일: 2026-10-21

`samples/DorotiTestbedApp`의 Texture 화면에서 위젯 갱신이 없거나 창에 포커스를 주는 입력 이후 텍스처가 정지하는 문제를 수정했다. 샘플의 33ms 생산 타이머와 위젯 코드는 변경하지 않았다.

## 원인과 수정

1. `SkiaSceneRenderer.Submit`이 텍스처를 불변 프레임으로 캡처한 뒤, retained scene replay에서도 제출 당시 프레임을 계속 사용했다. 새 제출의 스냅샷은 보존하고, replay에서는 `freeze: false`인 텍스처만 별도로 캡처한다. 중첩 retained subtree도 방문하며 새 캡처의 수명을 Graphite GPU recording에 연결한다. 등록 해제된 live texture는 다음 replay에서 비운다.
2. Windows의 표시 허용 검사가 replay에도 원래 장면의 입력 번호를 적용했다. 클릭/hover 등 입력이 처리되어도 위젯 변경이 없으면 입력 번호가 오래된 상태로 남아 텍스처 replay가 계속 거부됐다. 현재 표시 장면의 replay는 허용하고, 새 장면의 오래된 입력 차단과 prepared resize buffer의 최신 입력 검사는 유지한다.

`freeze: true`는 제출에 보관된 프레임을 유지한다. 같은 텍스처 ID를 frozen/live 두 곳에 사용해도 live 갱신이 frozen 인스턴스를 바꾸지 않는다. 제출/그리기 중인 프레임은 그대로 보존하고 별도의 replay 캡처를 사용하므로 생산자가 다음 프레임을 보내거나 등록을 해제해도 진행 중인 스냅샷은 변하지 않는다.

## 검증

모든 빌드·테스트·앱 실행은 `python Doroti/eng/run-with-timeout.py --timeout 1200`으로 제한했다.

| 검증 | 결과와 범위 |
| --- | --- |
| 수정 전 재현 | 파란 생산 프레임을 보낸 뒤 replay 픽셀이 빨간색으로 남는 실패를 확인했다. 별도로 입력 처리 후 replay 표시가 거부되는 실패도 확인했다. |
| `Doroti.Platform.Contracts.Tests --frames` | PASS. 단일 제출 후 연속 CPU/external 프레임, 최신 pending 프레임, 생산자 스레드, 최초 빈 텍스처, retained/filter subtree, freeze/resume, frozen/live 공존, unregister, 프레임 해제, 입력 후 replay와 오래된 새 장면 거부를 검사한다. 기존 불변 제출·소비자 수명·완료 순서 검사도 통과했다. |
| `Doroti.Tests --shader-frame-pipeline` | PASS. C 프레임 정책, 제한된 새 장면 admission, native/replay 대기, 완료 순서 및 동적 텍스처 예산 회귀 검사가 통과했다. CPU 계약 검증이다. |
| Windows App SDK Testbed Debug 빌드 | PASS, 경고 0 / 오류 0. |
| 실제 Windows Testbed | `DOROTI_SAMPLE=native-texture-probe`로 실행했다. 포커스/scroll/click 입력 후 유휴 애니메이션, Freeze 정지와 Resume 재생을 Computer Use로 확인했다. 창 제목 `Doroti Material Testbed`, `Responding=True`, 60초 smoke 종료 코드 0. |

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Platform.Contracts.Tests -c Debug -- --frames
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Debug -- --shader-frame-pipeline
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Debug --no-restore -v minimal
```

최종 Windows 실행의 로컬 원시 보고서는 `temp/testing/texture-idle-20261021/windows-focus.json`이다. AMD Radeon 780M의 Graphite/Vulkan → D3D12/DXGI 경로에서 present 1,929회, renderer replay 1,665회, stale input present 차단 0회, 초기화/운영 GPU debug error 0회, renderer failed 0회를 기록했다. terminal ledger는 active 0 / duplicate 0이다. 이는 실행 중 수집한 횟수이며 화면 FPS나 scanout 측정값이 아니다.

실제 UI 실행에는 hover/scroll/button으로 발생한 위젯 장면 변경도 포함된다. 위젯 장면을 한 번만 제출하고 생산자 프레임만 바꿔 재생되는 조건은 별도의 CPU 픽셀 회귀 검사에서 검증했다.

이번 실제 실행은 CPU 픽셀 생산자를 Windows GPU로 합성한 범위다. Vulkan validation layer는 비활성 상태였으며, 네이티브 카메라/디코더 GPU 입력, 다른 GPU, Android/Apple/Linux/Web 런타임, 물리 scanout 및 GPU fence 스트레스 검증은 수행하지 않았다. 원시 보고서는 삭제 가능한 로컬 산출물이다.
