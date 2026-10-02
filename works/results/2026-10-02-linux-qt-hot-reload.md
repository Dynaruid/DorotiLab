# Linux Qt 핫리로드 — 2026-10-02

**요청한 Linux Qt Debug 개발 구성 및 명시한 CLI/VS Code 검증 PASS.**
[work5.md](../../work5.md) §15에 반영했다. 실제 결과·소스/VSIX identity는 [JSON](2026-10-02-linux-qt-hot-reload.json), 원본은 `temp/testing/linux-hot-reload/`에 있다. 이전 [Qt 구성 검토](2026-10-02-linux-qt-configuration-review.md)의 payload 결과와 구분한다.

## 구성

- CLI `describe`와 `dev`가 선언된 `linux` runner를 개발 대상으로 제공한다. 선택한 `.NET` 경로를 실제 watcher 실행에 사용한다.
- `DorotiQtDevelopment=true`는 Linux/x64 Qt Debug, 최적화하지 않은 코드, portable 심볼, startup hook을 사용한다. `DOROTIQT007`은 Release/AOT/trim/single-file 및 호환되지 않는 override를 거절한다.
- Linux 개발 실행은 설정이 없을 때 `DOTNET_USE_POLLING_FILE_WATCHER=1`을 사용한다. 기존 환경 설정은 보존한다. 이는 SDK의 [공식 polling 설정](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch#environment-variables)이며 현재 머신의 inotify 인스턴스 제한 128 문제를 우회한다.
- VS Code가 Linux 대상과 로컬 runtime/request protocol을 사용한다. C# 저장과 **Hot Reload**는 실제 .NET metadata delta를 적용하고 Qt owner queue에서 위젯을 재구성한다. 단순 저장이나 별도 코드 재실행을 적용 성공으로 보고하지 않는다.
- 지원하지 않는 metadata 변경은 기존 프로세스와 상태를 유지하고 **Restart (resets state)**를 요구한다. 명시 Restart는 새 runtime/State를 만든다. native C++/QML·baked shader·project/dependency 변경도 재빌드/재시작 대상이다.

공통 `DorotiHotReload`의 metadata handler와 view reassembly 경로는 그대로 사용했다. Linux 대상 차단을 CLI/확장에서 해제하고 개발 실행을 연결했으며, Quick/Graphite/C 및 ABI 6/208바이트를 변경하지 않았다.

## 검증

Ubuntu 26.04, .NET SDK 10.0.400/runtime 10.0.11, Qt 6.10.2, Wayland/llvmpipe에서 기본 Quick 단일 창을 검증했다. 샘플 원본은 편집하지 않았으며, 격리된 template hive에서 생성한 앱을 repository ProjectReference에 연결했다.

| 검사 | 결과 |
| --- | --- |
| CLI와 MSBuild 구성 | Linux manifest/describe, Debug profile, invalid override 거절, custom .NET 경로, polling 기본값/override, Release CLI 거절 PASS |
| CLI 실제 핫리로드 | method-body 변경 후 matching request와 completed reassembly `applied`, 같은 PID/State/count/Hangul text/scroll, revision 0→1→2 PASS |
| 오류·지원하지 않는 변경 | CS0103 수정 후 두 번째 delta PASS. field int→long은 ENC0009/Restart 요구, 이전 runtime·상태 유지 PASS |
| CLI Stop | watcher/app process group 종료 및 Qt PID 소멸, fixture 원문 복원 PASS |
| 확장 기본 검사 | TypeScript compile 및 Node contract/process/bridge 검사 8개 PASS |
| 설치한 VSIX | 격리 editor profile에서 Linux Run, Hot Reload command, 상태 유지, compiler recovery, 중복 클릭, rude edit, 명시 Restart로 새 runtime/State, Stop PASS |
| 현재 IDE 설정 | 검증한 로컬 VSIX를 현재 VS Code에 설치, `doroti-local.doroti@0.1.0` 확인. `.vscode/settings.json`의 CLI/.NET 경로 구성 |
| 정합 | Source audit, 변경 문서 링크와 `git diff --check` PASS |

카운터 5, 텍스트 `한글 유지`, 스크롤 160은 자동 seed다. 실제 키보드/IME 입력을 시험한 결과로 확대하지 않는다. metadata 적용과 reassembly 성공은 물리 표시 완료 또는 GPU 성능 측정이 아니다.

초기 실행 실패도 보존했다. `wayland`는 watcher의 `-p` project alias와 MSBuild property 충돌로 시작하지 못했으며 `--property:`로 수정했다. `wayland-2`는 inotify 제한 때문에 시작하지 못했으며 polling으로 보완했다. `wayland-3`은 metadata/오류 복구를 통과한 뒤 Stop 직후 PID 검사에서 실패했고, 이미 종료 중인 child를 기다리는 bounded 검사로 수정했다. 최초 `vsix` 검사는 테스트 모드의 modal dialog 거절 때문에 Restart 단계가 진행되지 않아 종료했다. 최종 `vsix-final`은 명시적 command 확인 인자로 Restart를 실행하여 통과했다. 일반 사용자 Restart의 상태 초기화 확인은 유지한다.

## 사용

이 워크스페이스에서는 **Doroti: Select Project → Select Target → linux → Run**을 사용한다. 지원하는 C# 코드를 편집한 뒤 저장하거나 **Hot Reload**를 누른다. 오류는 **Show Logs**, 지원하지 않는 변경은 **Restart (resets state)**, 종료는 **Stop**을 사용한다.

```sh
pwsh -NoProfile -File Doroti/eng/doroti.ps1 dev -App samples/DorotiSampleApp2 -Platform linux
```

Testbed reload 장면은 다음과 같이 실행한다.

```sh
DOROTI_SAMPLE=reload pwsh -NoProfile -File Doroti/eng/doroti.ps1 dev -App samples/DorotiTestbedApp -Platform linux
```

회귀 검사 재실행은 [검사 문서](../../Doroti/tests/README.md), SDK/transport 범위는 [개발 계약](../../Doroti/docs/development-hot-reload.md)을 따른다. NuGet-only 개발, 여러 창·Widgets/OpenGL/Vulkan-window 핫리로드, native assets의 동적 교체, 물리 IME·display/performance는 이번 검증 범위 밖이다.
