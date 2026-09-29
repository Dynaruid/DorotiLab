# M4. 플러그인 SDK와 대표 네이티브 기능

원문: [개발 로드맵](../../plan.md) §3 M4 · 우선순위: **P2** · 작업 상태: **PARTIAL — 첫 Windows 범위 구현·검증 완료** · 실행 검증: 공통/Windows 패키지 소비 **PASS**, 나머지 플랫폼 **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

기존 manifest/handler/SDK 등록을 확장하고 FilePicker·URL launcher의 공통 패키지·수명·오류 계약을 담당한다. OS/Web 구현은 각 플랫폼 작업이다.

선행: [M0](00-foundation.md)·[M1](01-testing.md). 창·view 수명은 [M2-A](02-desktop-contract.md)·[M6](09-multiwindow.md)와 맞춘다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

원문의 범위와 완료 기준을 유지하면서 2026-09-28~29 실행 기록에 따라 완료 부분과 잔여 부분을 분리했다. 체크된 항목은 명시한 호스트·검증 범위에 한정하며, 아래 날짜별 결과가 근거다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**선행:** M0/M1, 창·view 문맥 계약은 M2/M6와 일치시킨다.

- [x] 현재 manifest/handler/SDK 등록 구조를 문서화하고 외부 NuGet이 target별 구현·native asset·Web module을 제공하는 최소 패키지 형식을 정한다.
- [x] 앱/view 수명·dispatch·취소·dispose·오류/거절/미지원 계약을 연결하고 창 종료 뒤 늦은 응답 폐기·handler 지연 dispose·여러 창의 application lease 공유 회귀를 검증했다.
- [x] 공개 event stream 구독·취소·backpressure·dispose 계약과 공통 회귀를 추가했다. 플랫폼별 native event source 검증은 후속이다.
- [x] RID/ABI 불일치, 중복 id/channel, 누락된 handler/asset을 build 또는 초기화 단계에서 구체적으로 진단한다.
- [x] 등록 생성은 기존 SDK 경로를 확장한다. Source Generator 도입은 reflection 회피·사용성·AOT 이득이 분명할 때 결정한다.
- [x] 첫 대표 기능은 FilePicker와 URL launcher로 잡는다. 선택 취소와 외부 파일 접근 수명까지 포함한다. 이후 camera/notification/storage는 요구와 플랫폼 검증 장비에 맞춰 추가한다.
- [ ] 네이티브/Web 구현을 같은 C# 앱에서 사용하고, 미지원 플랫폼은 capability 확인과 예측 가능한 오류를 제공한다.
- [x] 공통 NuGet-only 소비 앱의 Release trimmed publish/실행과 Windows host/plugin NuGet-only Release JIT 소비를 검증했다.
- [ ] 나머지 목표 플랫폼의 package-only 소비와 지원 trimming/AOT 조합을 검증한다. Windows renderer 전체 trimming/NativeAOT·실제 OS 권한 거절·물리 입력은 기존 결과에 포함하지 않는다.

**완료 기준:** 별도 앱과 별도 플러그인 패키지에서 코드 복사 없이 기능을 사용할 수 있다. 권한 거절·취소·창 종료 중 응답·ABI 불일치가 검증된다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

## 2026-09-28 — M4 Windows 첫 구현 결과

기준 revision: `36b566d11741d2f4971366a75169ce9c06044904` + 이번 작업 트리.
환경: Windows 10.0.26200 / win-x64 / .NET SDK 10.0.400. 공통 Debug JIT,
별도 공통 소비 앱 Release self-contained + trimming, Windows 소비 앱 Release JIT.
모든 검증은 `Doroti/eng/run-with-timeout.py`의 1,200초 제한으로 실행했다.

- `Doroti.Plugins`: 같은 C# 클라이언트의 capability 확인·FilePicker·URL launcher,
  JSON source-generated codec, 최대 64 KiB/요청의 64-bit offset 읽기와 파일 grant 해제.
- 기존 runner SDK: 외부 NuGet의 `DorotiNativePlugin`을 기존 handler 등록 코드로 생성하고,
  `Doroti.Application.PackagePlugins` manifest supplement를 내장한다. 별도 Source Generator 없음.
  필수 metadata/RID/중복/누락 asset을 `DOROTIPLUGIN001..006`으로 진단한다.
- Hosting: 앱 handler와 view 자원을 분리했다. 창 종료가 호출자 대기를 취소하고,
  늦은 응답은 폐기한다. 진행 중인 native 호출이 끝나기 전에는 handler를 dispose하지 않는다.
  파일 token은 다른 view에서 접근할 수 없다. 기존 Web registration marker도 유지한다.
- Windows App SDK runner: HWND 소유의 파일 선택과 URL capability 연결. 파일 선택은 별도
  STA에서 `IFileOpenDialog`를 실행하며, 취소 시 해당 apartment에서 `Close`한다.
  실제 대화상자가 남는 취소 결함을 재현한 뒤 수정했고, 호출자 취소·owner 종료를 재검증했다.
- 샘플: `DOROTI_SAMPLE=plugins`로 Testbed의 파일 선택/첫 16바이트 표시/URL 실행 장면을 연다.

| 검증 | 결과와 근거 종류 |
| --- | --- |
| `dotnet run --project Doroti/tests/Doroti.Plugin.Tests/Doroti.Plugin.Tests.csproj` | **PASS** — ABI/RID/type/missing/duplicate/reserved channel, 미지원/거절/취소, 5GB 길이·4GB offset의 synthetic 파일 읽기, grant 해제, view 격리, 늦은 응답·handler 지연 dispose |
| `python Doroti/tests/plugin_packages.py` | **PASS** — 격리 NuGet cache, product ProjectReference 없는 소비 앱 restore/build/run, generated manifest 로딩, Release trimmed publish/실행, RID·중복 id·누락 asset의 의도적 build 실패 |
| `python Doroti/tests/plugin_windows_packages.py` | **PASS** — host와 plugin까지 NuGet으로만 소비한 별도 앱 restore/build/Release publish/실행. 실제 HWND 파일 선택창, 선택 파일 OS handle 읽기·해제, 사용자 취소, 호출자 취소 후 실제 dialog 닫힘, owner 종료 중 취소·drain. 기본 브라우저를 통한 loopback HTTP 도달 |
| Windows Testbed build + `windows_smoke.py` | **PASS** — 기존 runner의 등록 생성/manifest 적용과 기본 창 API/native close·입력 수명 smoke 유지 |
| `validate.py Developer` + Web Testbed build | **PASS** — 문서/CLI 실패 전파·widget·plugin·Web rendering 회귀, 기존 Web Debug build. Windows/Web 최종 build 경고·오류 0 |

실제 파일 선택창 조작은 합성 dialog command다. 물리 입력이나 화면 픽셀 판정으로 승격하지 않는다.
권한 거절과 큰 파일은 공통 fake host 계약 회귀이며, 실제 OS ACL 거절·5GB 파일을 검증한 것은 아니다.
원시 로그·fixture·소비 앱·격리 cache는 `temp/testing/m4-*`에서 실행하고 결과 요약 후 정리했다.

공개 계약·패키지 작성법·재현 명령: [plugins.md](../../Doroti/docs/plugins.md).

### 당시 남은 항목과 다음 작업

아래는 후속 Web/Android·event stream 구현 전의 기록이다. 현재 범위는 문서 끝의 후속 실행 링크를 따른다.

2026-09-29 M6 연계: application handler를 참조 횟수 있는 boundary lease로 공유했다.
첫 창이 닫힌 뒤 살아 있는 창에서 호출 가능하고, 마지막 owner/active call 정리 뒤
handler가 한 번만 dispose되는 공통 회귀를 추가해 통과했다. 플랫폼별 물리 picker 결과를
확대한 것은 아니다.

- event stream은 현재 요청/응답 v1에서 **unsupported**다. native subscription 자원은
  view context에 붙일 수 있지만 공개 구독·backpressure API는 아직 없다.
- Web/Android/다른 native FilePicker adapter와 같은 앱의 플랫폼 간 실증, Web module
  누락/무결성 검증, 실제 권한 거절·물리 입력은 **notVerified**다.
- Windows renderer 전체 trimming과 NativeAOT는 이 결과에 포함하지 않는다. 기존 runner SDK의
  NativeAOT 대상은 실험적 iOS ios-arm64이며 이번 Windows 단계의 지원 모드가 아니다.
- 첫 Windows 패키지 기능·취소·오류·수명 조건을 충족했으므로 다음 문서는
  [07 OS Drag & Drop](07-os-drag-drop.md)다. 위 잔여 항목은 M4 전체 완료로 표시하지 않고 유지한다.


## 2026-09-29 Web·Windows·Android 후속 실행

[최신 구현·실행 근거와 잔여](../results/2026-09-29-web-windows-android.md)를 참조한다.
이 문서의 이전 실행 결과를 새 PASS로 확대하지 않는다.


## 2026-09-29 Linux / Qt 후속

Qt FilePicker·descriptor handler·Linux package 등록·취소·regular-file grant·URL을 연결했다. 실제 Qt dialog 자동 선택/취소·파일 권한 거절 및 xdg-open 기본 브라우저 loopback GET PASS.

상세 명령·환경·지원 경계: [Linux Qt 결과](../results/2026-09-29-linux-qt.md).
