# Android 메타데이터 핫리로드 구성

작업 시작: 2026-10-03, 후속 검사: 2026-10-04 (Asia/Seoul). 사용자 요청: **“여기 안드로이드 핫리로드 되게 구성해줘.”**

기준 HEAD: `6c8c0d5a3e69ed79359b1721c523be7205ce51f5` + 이번 미커밋 변경. 앞선 W11의 Android restart-only/기기 없음 기록은 당시 후보의 결과로 보존한다.

## 구현

- [Android launcher](../../eng/android-development.py)는 소스 저장 때 재설치하던 루프를 SDK `dotnet watch`의 메타데이터 업데이트로 교체했다. 하나의 승인된 ADB 기기를 자동 선택하며 복수 기기는 `-Device`로 지정한다. arm64/x64 ABI를 확인하고 잘못된 RID를 배포 전에 거절한다.
- [개발 프로필](../../src/Doroti.Runner.Sdk/Sdk/Doroti.AndroidDevelopment.targets)은 Debug Mono, portable symbols, startup hooks, untrimmed/non-AOT APK를 사용한다. 기본 Release Mono/profiled AOT 프로필은 유지된다. SDK 10.0.400 이상과 Android workload의 `HotReloadWebSockets` capability가 필요하다.
- SDK의 원본 delta agent와 인증된 WebSocket을 Android workload가 패키징하고 USB `adb reverse`로 연결한다. Mono의 `STARTUP_HOOKS` 설정을 runtimeconfig 생성 **이전**에 넣어 실제 agent가 시작되도록 한다.
- `adb run-as`는 앱의 private `files/Doroti.Dev/<session>`에 있는 상태·요청 JSON만 전달한다. 요청을 임시 파일로 쓰고 atomic rename을 완료한 뒤 `prepared.json`을 발행한다. 에디터는 이 확인 전에 파일을 저장하지 않으며, [Widgets metadata handler](../../src/Doroti.Framework.Widgets/DorotiHotReload.cs)의 실제 reassembly frame 완료 뒤에만 `applied`를 표시한다. PID/runtime/session이 다른 요청과 종료된 프로세스의 오래된 상태는 인정하지 않는다.
- [MAUI host](../../src/Doroti.Host.Maui/Doroti.Host.Maui.csproj)는 개발 세션의 공통 framework references에서 RID를 제거한다. `dotnet watch`의 path+TFM 식별에 같은 프로젝트가 중복되는 오류를 해결했다. Windows aapt2 경로 제한을 피하도록 별도 개발 캐시를 `Doroti/artifacts/ad/<project-sdk-rid-hash>`에 둔다.
- [VS Code extension](../../tools/vscode-doroti/src/extension.ts)은 Android Run/Hot Reload/Restart/Stop과 pre-save 요청 확인을 연결한다. Stop은 launcher에 `stop.json`을 보내 앱·watcher·이번 세션의 USB 포트를 정리한다. 프로젝트/리소스/네이티브 Kotlin·Java 변경과 SDK rude edit은 명시적인 Restart가 필요하다.
- 저장소 [VS Code task](../../../.vscode/tasks.json) `Doroti: Android Hot Reload`에서 Sample2/Testbed를 선택할 수 있다. `.vscode`의 나머지 개인 설정은 계속 Git ignore 대상이다.

## 검증

모든 검사는 `eng/run-with-timeout.py`의 **1,200초 제한**으로 실행한다. 연결 기기는 Galaxy S25 (`SM-S931N`, `R3CY30KZA4B`, arm64)이며 .NET SDK `10.0.400`, Android workload pack `36.1.69`를 사용한다.

| 검사 | 결과 |
|---|---|
| Android session isolation/pre-save delivery/disconnection/port ownership/잘못된 JSON 요청 | PASS, Python 7개 |
| VS Code extension 계약·process·Web bridge 회귀 | PASS, 8개 및 TypeScript compile |
| Sample2 Android arm64/x64 개발 프로필 평가·실제 SDK runtimeconfig 생성 | PASS, Debug Mono/StartupHookSupport=true/trim=false/AOT=false, `STARTUP_HOOKS=Microsoft.Extensions.DotNetDeltaApplier` |
| 일반 Sample2 Release 프로필 | PASS, Mono/profiled AOT/trim=true/startup hooks=false 유지 |
| Release/optimized/trimmed/AOT/CoreCLR 개발 프로필 사용 | PASS, `DOROTIANDROIDDEV001`로 빌드 전 거절 |
| Galaxy 실제 메타데이터 delta·상태 보존·컴파일 오류 복구 | PASS, `s25-05`, 실제 delta 2회·same PID/State/count/text/scroll·컴파일 오류 후 복구·rude edit의 상태 유지·Stop 앱/watcher/USB 정리 |
| 설치 VSIX Android Run/Hot Reload/Restart/Stop | PASS, `vsix-01`, 실제 VS Code 명령·pre-save 확인·state-preserving delta·Restart의 새 PID/runtime·Stop 기기 앱 종료 |

실기기 회귀는 [reload scene](../../../samples/DorotiTestbedApp/src/MaterialSample/HotReloadSample.cs)에 counter=5, text=`한글 유지`, scroll=160을 **자동 주입**한다. C# `Message()` 반환값을 바꿔 같은 PID/State/runtime에서 실제 새 코드가 실행되는지 비교한다. 물리 터치·한글 IME, TalkBack, 다른 기기와 x64 emulator runtime, CoreCLR/Release/AOT 핫리로드, 성능·장기 사용은 `notVerified`다.

실기기 PASS identity: runtime `46d88cb1-6bba-46bb-82b2-928d0c3a59c1`, PID `8591`, State `30e84e48-b78c-4612-b768-d5b482ad11b8`. `Before reload` → `Android metadata reload passed` → 컴파일 오류 `CS0103` → `Android compile recovery passed`로 변경했고 revision 0 → 1 → 2에서 count=5/text=`한글 유지`/scroll=160을 보존했다. 필드 타입 변경은 `ENC0009`로 Restart를 요구하고 같은 프로세스·상태를 유지했다. 캡처에서 변경된 label/count/한글/스크롤 위치를 확인했다. Stop 뒤 PID가 없고 세션 status=closed/supported=false이며 해당 USB reverse port가 제거됐다. 테스트 변경한 C# 원문은 복원했다.

설치 VSIX PASS identity: runtime `06fc4d79-e22f-4495-80a5-1fab7438826c`, PID `12238`, State `ffaaeaec-df6a-4547-8fde-6520cf624666`. **Hot Reload** 명령으로 `Android VSIX Hot Reload passed`가 실행되고 revision=1에서 State/count/text/scroll/PID를 보존했다. **Restart**는 runtime `1e36a1f9-a219-4100-932d-3261e76eeee9`, PID `16805`를 새로 만들었다. **Stop** 뒤 두 세션 모두 closed/supported=false, 기기 앱 PID 없음, USB reverse 목록 비어 있음, 테스트 호스트 exit=0을 확인했다. C# 원문은 복원했고 `doroti-local.doroti` VSIX를 사용자의 기본 VS Code에도 설치했다.

현재 기능 결과는 **Android Debug Mono 메타데이터 핫리로드 PASS**이며, 제품 전체/물리 입력/다른 runtime·기기 인수로 확대하지 않는다. Sample2는 프로필·SDK runtimeconfig 계약을 확인했고, 실기기 코드·에디터 회귀는 Testbed reload scene을 사용했다. 소스는 미커밋 상태이며 패키지 전용 consumer 배포 검증은 이번 범위에 포함하지 않았다.

```powershell
python Doroti/eng/run-with-timeout.py python Doroti/tests/android_development_bridge.py
python Doroti/eng/run-with-timeout.py python Doroti/tests/android_development_profile.py Doroti/artifacts/android-hot-reload/<fresh-profile-run>
python Doroti/eng/run-with-timeout.py npm.cmd test --prefix Doroti/tools/vscode-doroti
python Doroti/eng/run-with-timeout.py python Doroti/tests/android_hot_reload_smoke.py Doroti/artifacts/android-hot-reload/<fresh-run> --device R3CY30KZA4B
python Doroti/eng/run-with-timeout.py npm.cmd run package --prefix Doroti/tools/vscode-doroti
$env:DOROTI_TEST_ANDROID_DEVICE = 'R3CY30KZA4B'
python Doroti/eng/run-with-timeout.py node Doroti/tools/vscode-doroti/dist/test/runHost.js samples/DorotiTestbedApp Doroti/artifacts/android-hot-reload/<fresh-vsix-run> --android
```

최초 후보에서 긴 Windows 경로의 aapt2 실패와 framework 중복 평가를 발견했다. 다음 후보는 앱·상태 relay까지 실행됐지만 Mono startup config에 agent가 빠져 delta가 전달되지 않아 성공으로 인정하지 않았다. 중간 후보 두 개는 runtimeconfig 생성 내용을 확인한 뒤 설치 완료 전에 종료하고, hook을 입력 캐시 생성 전에 명시하는 방식으로 수정했다. 최종 `s25-05`와 `vsix-01`만 실기기 핫리로드 PASS로 인정한다. 원시 산출물은 삭제 가능한 `Doroti/artifacts` 아래에 있으며 지속 보존되는 결론은 이 문서를 따른다.

## 실행

```powershell
pwsh -NoProfile -File Doroti/eng/doroti.ps1 dev -App samples/DorotiSampleApp2 -Platform android
# 여러 기기가 연결되어 있다면 -Device <serial> 추가
```

VS Code: **Doroti: Select Project → Select Target → android → Run**, C# 메서드 변경 후 저장 또는 **Hot Reload**. 또는 **Terminal → Run Task → Doroti: Android Hot Reload**. 종료는 확장 기능의 **Stop** 또는 CLI의 Ctrl+C를 사용한다. 지원하지 않는 변경은 **Restart (resets state)**로 새 빌드를 실행한다.

SDK transport 참고: [MobileAppModel](https://github.com/dotnet/sdk/blob/v10.0.401/src/Dotnet.Watch/Watch/AppModels/MobileAppModel.cs), [Android 모바일 연결 요구사항](https://github.com/dotnet/sdk/issues/52492).
