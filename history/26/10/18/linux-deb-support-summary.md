# Linux `.deb` 지원 구현·검증 요약

- 작업일: 2026-10-18. 보관일: 2026-10-27 KST.
- 원본: 루트 `work.md`의 초기 계획과 8절. 원본 SHA-256: `496316a00f30978758d3d567d1d560224bdfb4905ca8c0a7319f0ac91ccc9c94`.
- 상태: **CLOSED / ARCHIVED · 초기 지원 전체 PARTIAL**. Debian 13 / 시스템 Qt 6.8.2의 패키징·xcb 설치 lifecycle은 PASS, Wayland 종료는 FAIL, 물리 GPU·화면·메뉴·입력은 미검증이다.
- 10월 19일 배포 기준을 앱 전용 Qt 6.12 번들로 바꿨다. 최종 배포 계약과 남은 승인 조건은 [Qt 런타임 번들 요약](../19/qt-runtime-bundle-summary.md)을 따른다. 이 문서는 과거 시스템 Qt 방식의 기록이며 활성 작업계획이 아니다.

사용자 요청으로 원본의 결정·결과·미완료 항목을 날짜별로 요약하고 루트 `work.md`를 삭제했다. 아래 결과는 당시 기록이며, 보관 과정에서 제품 빌드·설치·GUI 검증을 다시 수행하지 않았다.

## 구현한 계약

- 기존 Linux provider의 `publish`에 `packageformat=directory|deb`와 출력 경로를 추가했다. 옵션을 생략하면 기존 폴더 게시를 유지하며, 별도 실행 플랫폼을 만들지 않았다.
- [Qt provider](../../../../packages/platforms/qt/tooling/Extension.cs)가 선택된 dotnet·TFM·RID·게시 인수를 [패키징 도구](../../../../packages/platforms/qt/tooling/linux_deb.py)에 전달한다. 도구가 publish·staging·자식 프로세스를 소유해 취소·실패를 전파하고 완성된 산출물만 원자적으로 반영한다. NuGet에 도구를 포함해 저장소 소스 없는 소비자를 지원했다.
- 초기 조합은 Debian 13 amd64, Release/net10.0/linux-x64, Quick/Graphite, self-contained·untrimmed JIT 디렉터리 게시다. AOT·trimming·single-file·R2R·GStreamer·`UseAppHost=false`는 사전 거부했다. Ubuntu·arm64 등은 별도 검증 범위다.
- MSBuild 평가값으로 application ID·실행 파일·아이콘·버전과 Debian 메타데이터를 읽는다. `DorotiDebPackageId`, `DorotiDebMaintainer`, `DorotiDebDescription`, `DorotiDebCopyright`는 필수다. `DorotiDebVersion`/`DorotiDebRevision`을 지원하며, prerelease는 Debian `~` 규칙으로 변환하고 충돌 가능한 build metadata와 동일 버전 출력 덮어쓰기는 거부한다.
- 설치 ELF의 amd64 아키텍처와 외부 의존성을 검사하고 `dpkg-shlibdeps`로 런타임 Depends를 만든다. 초기 방식은 시스템 Qt/QML 패키지를 사용했다. 컴파일러·CMake·개발 패키지·.NET SDK는 사용자 런타임 의존성에 넣지 않았다.
- 선택적인 .NET `libcoreclrtraceptprovider.so`의 Debian 13 미충족 LTTng 의존성은 `DorotiDebExcludeLttngProvider=true`일 때만 staging에서 제외한다. 원본 publish는 보존하고 receipt에 제외 내역과 LTTng tracing 미지원 범위를 남긴다.
- launcher는 절대 경로와 전달받은 인수를 사용하고 XDG 사용자 경로를 바꾸지 않는다. remove/purge도 application ID에 속한 사용자 데이터를 보존한다. portable 데이터 자동 이관은 미구현이다.

| 설치 위치 | 내용 |
| --- | --- |
| `/opt/<package-id>/` | 앱·native payload·licenses |
| `/usr/bin/<package-id>` | launcher |
| `/usr/share/applications/<application-id>.desktop` | 메뉴 및 앱이 선언한 MIME/URI 처리 |
| `/usr/share/icons/hicolor/<size>/apps/` | 실제 크기의 PNG 아이콘 |
| `/usr/share/doc/<package-id>/` | copyright·notice·changelog |

파일 소유자는 root, 실행 파일은 0755, 일반 데이터는 0644로 기록하되 빌드는 일반 사용자로 수행한다. 설치 스크립트에서 앱·portable 설치기를 실행하거나 사용자 기본 연결을 변경하지 않는다. `.deb`·SHA-256·receipt·contents·shlibdeps·Lintian 보고서를 한 디렉터리에 제공하고, 생성 receipt의 설치/GUI/lifecycle 필드는 별도 검사 전까지 `not-run`으로 둔다.

## 함께 수정한 기존 문제

RID별 `ProjectAssetsFile`을 평가해 NuGet provider 탐색에 전달하고, Testbed·템플릿 Linux props의 상위 앱 메타데이터 상속과 아이콘 게시 시 PDB 오분류를 수정했다. Qt 6.8.2의 QmlMeta를 라이선스 목록에 반영했으며, 6.9 이전 WebEngine profile API와 초기 QML userScripts의 미초기화 adapter 접근을 보완했다. 후자는 native crash와 정상 종료를 gdb로 구분해 확인했다.

## 당시 검증 환경과 결과

일반 사용자 UID 1000의 user namespace/bwrap로 빌드·런타임 Debian rootfs를 분리했다. 깨끗한 런타임에는 저장소·SDK를 마운트하지 않았다.

| 항목 | 고정값 |
| --- | --- |
| 이미지 | `debian:trixie-slim`, amd64 manifest `sha256:918311b7b6c4c6f68b232ba516584925f6c78ad82b6fd534b98979df6438e483` |
| 빌드 | Debian 13, .NET SDK 10.0.400 / runtime 10.0.11, Qt/WebEngine 6.8.2 |
| 시스템 | glibc `2.41-12+deb13u4`, libstdc++/libgcc `14.2.0-19` |
| 도구 | dpkg-deb 1.22.22, binutils 2.44, Lintian 2.122.0 |
| GUI | xcb/Xvfb, Weston 14.0.2 headless, Mesa `25.0.7-2+deb13u1`, software Vulkan |

| 검사 | 당시 결과·범위 |
| --- | --- |
| provider 옵션·기본 폴더 게시·공백/한글 인수·잘못된 설정·취소 | PASS |
| 메타데이터/버전 주입 거부·ELF 아키텍처·실행 중 취소·rollback | PASS |
| Quick/WebView `.deb` 내용·desktop·권한·소유권·payload 해시·Lintian | PASS |
| 개발 도구 없는 clean APT 설치 및 UID 1000 xcb 실행 | Quick/WebView 모두 PASS. WebView native compositionFrames > 0 |
| v1→v2 업데이트·remove·reinstall·purge·바이너리 변경·XDG 데이터 보존 | 두 앱의 개별 lifecycle 필드 PASS |
| fresh NuGet 소비자 및 저장소 없는 provider 게시·clean 설치·xcb lifecycle | PASS |
| typed extension 연결·취소·Stop 소유권 및 기존 폴더/portable 회귀 | PASS |
| 기존 Ubuntu 26.04 / Qt 6.10.2 WebView·Wayland | PASS. Debian/Qt 6.12 승인과는 별개 |
| Debian Qt 6.8.2 / Mesa software Vulkan의 Wayland 종료 | **FAIL** |
| 물리 GPU·display·메뉴 클릭·물리 입력 | **미검증** |

Wayland는 Vulkan loader를 `libvulkan.so.1`로 수정한 뒤에도 swapchain 종료 중 파괴된 queue 접근, `wl_display_flush` mutex 대기·abort가 Weston과 host compositor에서 재현됐다. 전체 receipt는 `failed`로 유지하고 독립적으로 통과한 설치·xcb·데이터 보존 필드는 보존했다. 당시 예제는 xcb 기본값을 사용했으며 단발 성공으로 Wayland를 승인하지 않았다. 다음 날의 Qt 6.12 번들 전환이 이 실패의 해결·재검증 완료를 뜻하지 않는다.

## 기록 위치와 후속 경계

당시 로그는 `temp/testing/linux-deb/current/`의 `contracts-final.log`, `quick-acceptance/`, `webview-acceptance/`, `nuget-acceptance/`, `wayland-gdb.log`, `host-regression.json` 등에 기록했다. `/opt` 배치·알려진 embedded 라이브러리·native 진단 심볼에 한정한 Lintian override를 적용하고, `initial-upload-closes-no-bugs`·`no-manual-page` 경고는 보고서에 보존했다.

이 경로들은 당시 증거 위치이며 보관 과정에서 원시 산출물의 현재 존재·무결성을 확인하지 않았다. `temp/testing`과 `Doroti/artifacts`는 삭제 가능한 로컬 산출물이다. 사용자 절차는 [Qt 호스트 안내](../../../../packages/platforms/qt/README.md)와 [런타임 배포 가이드](../../../../packages/platforms/qt/qt-runtime.md), 후속 미완료 항목은 [10월 19일 요약](../19/qt-runtime-bundle-summary.md)을 참고한다.
