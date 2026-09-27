# WindowsAppSDK 입력·UI 작업 큐 계약

저장소 루트에서 실행한다. Windows 및 .NET 10이 필요하다.

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/windows-appsdk-contract/Contract.csproj -c Release
```

실제 제품의 `WindowsKeyMap`, `WindowsKeyboardState`, `WindowsFrameInputState`, `WindowsPlatformViewDispatcher`를 참조한다.
103개 검사는 다음을 포함한다.

- AZERTY/QWERTZ 위치, 문장부호, NumLock 전환, main/keypad Enter, 좌우 modifier, Pause/NumLock, scan-code-zero fallback.
- printable numpad의 logical identity, 레이아웃이 달라진 repeat/up의 최초 logical identity 유지.
- 포커스 상실 시 남은 키만 synthesized up으로 정리, 중복 상실과 재입력.
- 24프레임 연속 입력 중 매번 scene 제출 후 새 입력이 도착해도 완성된 프레임 표시 허용.
  scene에는 dispatch 완료된 입력 번호만 기록하며, 이미 처리한 입력보다 오래된 scene과
  새 native 입력 이후의 prepared resize buffer는 계속 거부한다.
- 실제 message-only HWND에서 worker→UI FIFO, 한 번의 알림으로 여러 작업 처리, 예외 뒤의 후속 작업, async continuation의 owner 유지, 다음 알림 재등록, 종료 전 drain, 종료 후 호출 거절.

키보드는 합성 패킷의 managed 계약이다. 실제 키보드, 한글 IME, dead-key 조합, 전체 native/framework 포커스 순환을 승인하지 않는다. `PostMessageW` 실패 경로는 소스에서 enqueue 이전 실패를 확인했으며, OS 큐 고갈 주입은 수행하지 않았다.

연속 입력 검증은 제품의 표시 허용 조건을 검사한다. 실제 물리 트랙패드 입력 주기나
화면 표시 지연을 측정한 결과는 아니다. Variable Blur 페이지의 합성 pan/zoom과
Vulkan 중간 프레임 픽셀은 `cupertino-sample --variable-blur`에서 별도로 확인한다.

2026-09-27: Windows 샘플의 Variable Blur 탭에서, 수정된 앱을 사용한 사용자가
실제 트랙패드로 제스처 종료 전에도 리스트가 움직임을 확인했다.
이는 해당 증상의 수동 확인이며 프레임률이나 지연 시간 측정은 아니다.

[2026-09-25 검토·실행 결과](review-2026-09-25.md)
