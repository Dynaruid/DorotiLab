# Web · Windows · Android 후속 실행 — 2026-09-29

기준: `142386447ba810b747ac15d8dcac176107a233d9` + 이번 작업 트리.
요청 범위: [work2.md](../../work2.md)의 세 플랫폼. **전체 상태는 PARTIAL**이다.
아래 PASS는 실제 실행한 범위에 한정하며, 남은 구현·물리 검증을 완료 처리하지 않는다.
원격 CI workflow는 사용자의 명시적 후속 지시에 따라 삭제 상태로 유지했다.

## 구현과 발견한 결함

- **플러그인:** Web/Android의 공통 NativeFeatures 등록, 브라우저 view별 File grant,
  Android Storage Access Framework의 비공개 proxy Activity·선택·취소·수명 처리를 추가했다.
  Android는 persistent URI 권한을 취득하지 않으며 길이 미제공 문서는 거절한다.
  비탐색 provider는 다시 열고 제한된 버퍼로 offset까지 읽는다.
- **Event stream:** `IDorotiPluginEventHandler`와 `platform.plugin-events` capability.
  용량 1..1024(default 16), event 최대 64 KiB, 복사한 payload, lossless backpressure,
  caller/owner 취소·구독 해제·진행 중 handler lease를 공통 SDK에 연결했다.
- **Web worker 연결:** main-owned runtime도 C# framework/JSImport은 render worker에서
  실행된다. 기존 Navigation의 `typeof window` 검사로 실제 브라우저 연결이 생략되던 결함을
  재현하고 기존 control mailbox로 DOM navigation·파일·drop을 연결했다.
  초기 history/checkpoint를 비동기로 준비하고, 저장 실패를 managed host에 보고한다.
  picker는 사람의 선택 시간을 일반 30초 RPC timeout으로 제한하지 않는다.
- **Web drop:** Copy 수신, 파일·텍스트·URI, CSS 좌표·bounds, listener 및 파일 grant 해제.
  외부 OS 송신·Move/Link·virtual-file 생성은 이번 구현에 포함하지 않는다.
- **입력/semantics:** Material TextField가 화면의 버튼까지 합쳐져 HTML input의 자식이 되던
  문제를 독립 semantics container로 수정했다. 브라우저와 Android에서 실제 입력 노드와
  형제 버튼의 분리를 확인했다. TalkBack/스크린리더 사용 검증과는 별개다.
- **샘플:** plugin/drop에 SafeArea를 적용했다. Navigation RouterDelegate에 Navigator를
  제공하여 입력 시 selection Overlay 부재로 실패하던 문제를 수정했다.
  Navigation 이외의 Testbed 장면은 브라우저 query를 named route로 해석하지 않는다.
- **출시:** `release-candidate.py --targets windows web android --android-rid android-x64`
  경로를 추가했다. Android는 평가된 MAUI 의존성 그래프와 선택한 RID로 pack하며,
  외부 소비 앱의 APK에는 assemblies를 포함한다. Debug APK의 Fast Deployment 의존으로
  ADB 단독 설치 직후 종료하던 경우를 확인해 검증 명령에 `EmbedAssembliesIntoApk=true`를 적용했다.
- **Windows 설치 protocol:** current-user 등록·업데이트 시 새 exe로 연결·제거,
  설치 소유권/경로 검사, 실패 시 current 포인터와 등록 rollback을 추가했다.
  다른 앱의 scheme을 덮어쓰지 않는다. 공개 서명·OS installer 배포와는 별도다.

## 이번 실행 결과

모든 테스트/빌드는 `Doroti/eng/run-with-timeout.py` 또는 CLI의 **1,200초 제한**을 사용했다.
일반 반복은 30회 이내. Windows 10.0.26200, .NET SDK 10.0.400, Chrome 153.0.8010.54.

| 실행 | 결과 / 근거 |
| --- | --- |
| 새로운 detached worktree + 변경 소스 복사, `validate.py Source` | **PASS** — 이전 temp/artifacts 없이 문서·CLI 실패 전파·installer 계약. 공유 설치 toolchain/NuGet cache를 썼으므로 clean OS 증거가 아님 |
| `validate -ValidationSuite Developer` | **PASS** — widget, plugin, drop, Web 정책/Navigation/file-grant 회귀 |
| `Doroti.Plugin.Tests` 최종 별도 실행 | **PASS** — event queue backpressure, payload 복사, owner 취소와 unsubscribe 포함 |
| `node --test ...web_rendering.mts` | **PASS**, 6 tests — owner 격리·경계/취소 중 파일 read 포함 |
| Windows App SDK Debug build + WindowsSmoke | **PASS** — 창 상태/종료, editor/WebView 재생성, 두 HWND·survivor와 두 lifetime |
| Web Debug build | **PASS**, 최종 경고/오류 0 |
| `web_services_smoke.py`, WebGL와 WebGPU | **각 PASS** — 실제 Chrome file chooser → C# 파일 읽기/해제·취소, 실제 Back/Forward, 입력·route 새로고침 복원, 독립 textbox semantics. DOM DataTransfer → worker → C# drop은 합성 입력 |
| VS Code 확장 `npm test` | **PASS**, 8 tests |
| 새 profile 설치 VSIX, Windows `runHost.js` | **PASS** — snippet 연결 이름/Tab, import/Quick Fix/Undo, 실제 metadata update, State/입력/scroll 보존, compile-error 수정 재시도, 중복 요청, stop |
| 새 profile 설치 VSIX, Web `runHost.js --web` | **PASS** — 실제 Chrome 화면에서 Count 1·`Web 상태 유지`·Row 4 유지, label 변경/오류 복구. `runtimeId=ca4fc8ac-b51c-4726-bba3-67dd8a4493fb`, revision 0→1→2. 새로고침 없음 |
| Android x64 Debug standalone APK build/install | **PASS** — 최신 source APK 재설치, Android emulator `emulator-5556`, Graphite Vulkan(호스트 RTX 4060 Laptop GPU), 1080×2340. 마지막 build 경고/오류 0 |
| Android picker/복귀 | **PASS** — ADB 합성 tap으로 실제 DocumentsUI에서 `doroti-work2.txt` 선택, 18 bytes와 첫 16 bytes의 hex 화면 확인, 앱 복귀 |
| Android Navigation | **PASS** — cold/warm VIEW intent; 최신 APK에서 `android-state` 입력 → `#/second` → force-stop → 일반 재기동 후 입력·route를 Android hierarchy와 화면에서 확인 |
| `installer_contract.py` | **PASS** — interrupted install 재시도, tamper/extra-file 거절, update/remove 후 userdata 유지 |
| `protocol_install.py` | **PASS** — 실제 OS URI 실행, 인수 보존, 업데이트된 설치 경로 실행, 타 owner/경로 이탈 거절, 등록 제거와 userdata 보존. GUI 앱의 warm-instance와 묶은 테스트는 아님 |
| 세 플랫폼 Release candidate | **PASS** — 각각 별도 NuGet-only 소비 앱/격리 NuGet·HTTP cache, Windows 실제 두 창/resize/close, Web publish, Android x64 APK publish |
| Release Web/Android 후보 실행 | **PASS** — Web 실제 Chrome·Android emulator에서 asset/localization 화면과 Increment 0→1 확인 |

Web/Android 입력은 브라우저/ADB 자동 입력이다. 물리 한글 IME 입력·보조기술·mixed-DPI·GPU 품질/예산을 증명하지 않는다.

## 후보와 재현

최종 제품 패키지 후보: `Doroti/artifacts/release/0.3.0-beta.rc.20260929045407/`.
`candidate.json`은 source snapshot hash, 패키지/payload SHA-256, toolchain과 실행 목록을 가진다.
Windows/Web은 Release JIT/비-AOT, Android는 x64 Release·개발용 서명이며 정식 서명이 아니다.
추가 installer protocol 검증은 저장소 도구의 후속 실행으로 구분한다. 이전 후보
`0.3.0-beta.rc.20260929043023`은 Web worker 연결의 최종 수정 전 후보다.

```powershell
python Doroti/eng/run-with-timeout.py python Doroti/tests/web_services_smoke.py --url http://127.0.0.1:5199/ --renderer webgl --output temp/testing/browser-services/webgl
python Doroti/eng/run-with-timeout.py python Doroti/tests/web_services_smoke.py --url http://127.0.0.1:5199/ --renderer webgpu --output temp/testing/browser-services/webgpu
python Doroti/eng/run-with-timeout.py python Doroti/tests/protocol_install.py temp/testing/protocol-install/new-run
python Doroti/eng/run-with-timeout.py python Doroti/eng/release-candidate.py --targets windows web android --android-rid android-x64
```

Web 서비스 회귀는 Python Playwright·설치된 Chrome 및 빌드한 Testbed server가 필요하다.
실행 로그/화면/설치 VSIX profile은 `temp/testing/work2/current/`에 모았다.
성공한 aggregate 및 candidate 내부 소비 디렉터리는 자체 정리했다.
이전 정리 보류 목록의 m0-m3/plan-all 및 지정된 package/developer/release-candidate 실행 폴더는
이번 확인에서 없었다. `temp/testing/build/fd3416cf43254cd09d5a3afc72173251`은 남아 있으며,
로그는 수정 전 Explicit 종료 3회 통지 실패다. 현재 Developer/WindowsSmoke/VSIX 검증과 구분한다.
고정 allowlist cleanup 및 이 이전 실행 폴더 삭제 명령은 자동 승인 검토에서
`blocked by policy`로 거절됐다. 추가 사유 없음. 삭제를 우회하지 않았고 해당 자료는 보류한다.
이번 실행의 `temp/testing/work2/current/` 및 초기 `temp/work2-web-build.log` 삭제도
같은 자동 승인 거절로 보류했다. 성공 여부와 별개로 원시 자료가 아직 남아 있다.
새 detached worktree `temp/testing/work2/clean`은 정상 제거했다. 서버와 검증용 브라우저 탭도 종료했다.
정리 시점은 삭제가 허용되는 후속 세션이며, 재생성 가능한 위 실행 소유 경로만 대상으로 한다.

## work2 전체 완료를 위해 남은 범위

| 공통 작업 | 이번 실행 뒤 잔여 |
| --- | --- |
| 00 | 원격 CI는 사용자 요청으로 제외. clean OS/VM 및 이전 산출물 삭제 보류 |
| 01 | 고정 GPU golden/실제 화면 기준, 전체 texture/native owner 격리·진단 결함 주입 |
| 02 | 연속 live resize 픽셀·mixed-monitor DPI·GPU drain 실측, Windows MAUI Desktop 계약 보완 |
| 03 | 물리 한글 IME·Tab/focus·UIA/TalkBack/브라우저 보조기술, 전체 PlatformView 조합·Windows MAUI 합성 |
| 04 | 장기 스크롤·blur/영상 texture·present/VRAM·GPU loss·mobile Web·폰트/offline 전체 baseline |
| 05 | 마법사 실제 예외 dialog, Microsoft C# 언어 서비스 전체 조합, 모든 snippet 편집, rude-edit Restart/종료 경쟁/Workspace Trust |
| 06 | 실제 권한 거절·물리 picker·긴 native event source/복수 창 지연 응답, 목표 AOT/trim 조합 |
| 07 | Explorer 실제 교차 창 전달, Windows/Web OS 송신·drag 이미지·Move/Link·virtual file·창 간 이동. Android 수신/송신 capability는 unsupported |
| 08 | bfcache·외부/손상 history 실제 브라우저 확대, 앱별 schema migration·selection 복원 전체, 설치 protocol과 GUI cold/warm 통합 |
| 09 | Windows 두 창 물리 IME·mixed-DPI·전체 native content와 Satellite 경계 |
| 10 | 선택한 실제 배포 형식/서명/계정·clean OS 설치·업데이트·장기 회귀·최종 지원표 qualification |

PC의 코드서명 인증서는 Jarvis/Assistar용이며 Doroti 배포용으로 선택하지 않았다.
Hyper-V `Get-VM` 도구는 제공되지 않았고, Galaxy 실기기는 ADB에 연결되어 있지 않았다.
이 조건과 미구현 항목을 이유로 전체 완료 또는 출시 승인으로 표시하지 않는다.
