# WindowsAppSDK 입력·UI 작업 큐 계약

저장소 루트에서 실행한다. Windows 및 .NET 10이 필요하다.

```powershell
python Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/windows-appsdk-contract/Contract.csproj -c Release
```

실제 제품의 `WindowsKeyMap`, `WindowsKeyboardState`, `WindowsPlatformViewDispatcher`를 참조한다.
29개 검사는 다음을 포함한다.

- AZERTY/QWERTZ 위치, 문장부호, NumLock 전환, main/keypad Enter, 좌우 modifier, Pause/NumLock, scan-code-zero fallback.
- printable numpad의 logical identity, 레이아웃이 달라진 repeat/up의 최초 logical identity 유지.
- 포커스 상실 시 남은 키만 synthesized up으로 정리, 중복 상실과 재입력.
- 실제 message-only HWND에서 worker→UI FIFO, 한 번의 알림으로 여러 작업 처리, 예외 뒤의 후속 작업, async continuation의 owner 유지, 다음 알림 재등록, 종료 전 drain, 종료 후 호출 거절.

키보드는 합성 패킷의 managed 계약이다. 실제 키보드, 한글 IME, dead-key 조합, 전체 native/framework 포커스 순환을 승인하지 않는다. `PostMessageW` 실패 경로는 소스에서 enqueue 이전 실패를 확인했으며, OS 큐 고갈 주입은 수행하지 않았다.

[2026-09-25 검토·실행 결과](review-2026-09-25.md)
