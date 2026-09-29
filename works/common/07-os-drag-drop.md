# M5-A. OS Drag & Drop

원문: [개발 로드맵](../../plan.md) §3 M5-A · 우선순위: **P2** · 작업 상태: **PARTIAL — 공통 계약·Windows 수신 구현** · 실행 검증: 공통/Windows native·실제 OLE fixture **PASS**, Explorer 교차 창 입력 등 **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

이벤트·좌표·action·stream·접근 수명을 정의한다. Windows 수신부터 시작하고 호스트가 제공하는 범위를 capability로 구분한다.

선행: [M1](01-testing.md)·M2. 파일 데이터 계약은 [M4](06-plugin-sdk.md)와 공유한다. [M5-B](08-navigation-restoration.md)와 독립적으로 진행할 수 있다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

원문의 범위와 완료 기준을 유지하면서 2026-09-28~29 실행 기록에 따라 완료 부분과 잔여 부분을 분리했다. 체크된 항목은 명시한 호스트·검증 범위에 한정하며, 아래 날짜별 결과가 근거다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

- [x] enter/over/leave/drop, 허용 action(copy/move/link), MIME/type, 취소, 논리 좌표 변환을 공통 계약으로 정의한다.
- [x] Windows 파일/텍스트/URI의 Copy 수신을 연결하고 native/OLE fixture·큰 파일·96/192 DPI 좌표·수명·패키지 소비를 검증했다.
- [ ] Explorer → 기본 runner의 실제 교차 창 전달·드래그 중 mixed-monitor DPI·외부 취소를 검증한다. AppKit/Qt/Web 수신으로 확장하고 Web/모바일 제공 범위는 capability로 표현한다.
- [x] 전체 파일을 즉시 메모리에 올리지 않도록 stream/비동기 읽기와 접근 수명을 정의한다. 드롭된 경로나 데이터만으로 파일 실행을 수행하지 않는다.
- [ ] 이후 Doroti → OS 송신, drag 이미지, 창 밖 취소와 여러 창 사이 이동을 지원한다. move 성공·취소에 따른 원본 처리 책임을 명확히 한다.

**완료 기준:** Explorer/Finder/파일 관리자 또는 브라우저 파일 드롭으로 실제 파일을 전달할 수 있다. 여러 파일·큰 파일·취소·DPI 변경에서 내용과 좌표가 보존된다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

## 2026-09-28 — 공통 계약과 Windows 수신

Revision: `36b566d11741d2f4971366a75169ce9c06044904` + 06/07 작업 트리.
Windows 10.0.26200 / .NET SDK 10.0.400 / win-x64 / Windows App SDK.
기본 Testbed Graphite/Vulkan Debug JIT, 별도 NuGet 소비 앱 Release JIT.
검증 명령은 모두 `Doroti/eng/run-with-timeout.py`의 1,200초 제한을 사용했다.

- `Doroti.Ui`의 OS drop capability와 `OsDropReceiver`에 enter/over/leave/drop/error,
  MIME/type, view 논리 좌표·영역, source/receiver action 협상, 소유권·취소를 추가했다.
  연속 over는 합치되 leave/enter 경계의 순서를 보존한다.
- Windows OLE `RegisterDragDrop`를 기본 runner의 HWND에 연결했다. CF_HDROP,
  CF_UNICODETEXT, Unicode URL/UTF-8 URI list를 수신하며 app 이벤트는 framework 입력
  큐로 보낸다. OS 콜백 안에서 app UI를 직접 실행하지 않는다.
- 파일은 06의 `IPickedFile`과 같은 read-only handle 구현을 사용한다. 전체 읽기 없이
  64-bit offset·비동기 읽기·선택적 취소를 제공하고, payload/구독/owner 종료 시 해제한다.
  native 데이터는 callback 중 snapshot하고 STGMEDIUM은 항상 반환한다.
- 이번 Windows capability는 Copy 수신만 제공한다. Move/Link 요청은 거절하고
  송신·drag 이미지·virtual file·다른 플랫폼은 지원한 것으로 광고하지 않는다.
- `DOROTI_SAMPLE=drop`에 파일명/길이/첫 바이트, 텍스트·URI, event/action/논리 좌표 표시를 추가했다.

| 실행 | 결과·증거 종류 |
| --- | --- |
| `dotnet run --project Doroti/tests/Doroti.Drop.Tests/Doroti.Drop.Tests.csproj` | **PASS** — format/action/영역, over 병합·세션 순서, 비동기 dispatch, 거절·부분 실패, 구독/owner 해제 뒤 queued payload 폐기 |
| `dotnet run --project Doroti/tests/Doroti.Drop.Windows.Tests/Doroti.Drop.Windows.Tests.csproj -- temp/testing/m5a-native/large` 및 `--dpi-unaware` | **PASS** — 실제 HWND 등록/해제·IDataObject/HGLOBAL·다중 파일, 한글·URI, 실제 5GB sparse 파일/4GB offset 데이터·취소된 읽기, source 파일 유지. 192 DPI 및 96 DPI 변환. Native callback fixture이며 Explorer 입력이 아님 |
| `python Doroti/tests/plugin_windows_packages.py --drop` | **PASS** — 저장소 ProjectReference 없이 host/공통 패키지만으로 restore/build/Release publish/run, 격리 NuGet cache, 위 native 회귀 및 96 DPI 실행 |
| `--interactive` + computer-use drag | **PASS** — 실제 OLE DoDragDrop→등록 target 경로로 두 파일(한글.txt/second.txt), 한글 텍스트, URI 전달. 화면에 Copy와 위치 및 동일 데이터가 표시됨. 동일 fixture 창의 source/target이며 합성 pointer 입력 |
| 기본 Testbed `DOROTI_SAMPLE=drop` | **PASS** — 실제 표시 및 접근성 트리의 OS Drag & Drop·Completed drops 표시. 창 간 Explorer 전달 결과로 승격하지 않음 |
| `validate.py Developer` 및 `windows_smoke.py` | **PASS** — 문서/CLI 실패 전파·widget·plugin·OS drop·Web rendering 회귀, 기존 Windows API/native close 취소·정리·입력 수명. OLE 등록 추가 후에도 native 종료가 성공함 |
| Windows/Web Testbed 최종 Debug build | **PASS** — 두 runner 모두 경고 0·오류 0. Web OS drop adapter의 구현·실행 증거는 아님 |

Explorer에서 실제 테스트 파일 두 개를 선택했으나, computer-use가 다른 창 위치로 향하는
drag endpoint를 `outside window bounds`로 거절했다. Explorer→Testbed 전달은 수행되지
않았으므로 **notVerified**다. 같은 프로세스의 실제 OLE drag와 callback fixture를
이 완료 조건의 대체 증거로 표기하지 않는다.

원시 로그·fixture·5GB sparse 파일·임시 NuGet 소비 앱은 `temp/testing/m5a-*`에서 실행하고
요약 후 정리했다. 공개 사용법·제한·재현 절차는 [OS drop 계약](../../Doroti/docs/os-drag-drop.md)에 기록했다.

### 남은 범위

- Explorer/Finder/브라우저 등 실제 외부 source와 기본 runner 사이 전달, 물리 입력,
  진행 중 mixed-monitor DPI 변경, 외부 드래그 중 취소는 **notVerified**다.
- AppKit/Qt/Web/mobile adapter, Doroti→OS 송신·drag 이미지, move 완료/원본 삭제 책임,
  virtual file stream과 다중 창 동작은 미구현이다. Windows native editor/WebView 자체의
  drop 동작을 Doroti view 수신으로 보장하지 않는다.
- 공통/Windows 수신 기반은 준비됐다. 다음 독립 작업은 [08 Navigation/restoration](08-navigation-restoration.md)이며,
  위 07 잔여 항목과 전체 완료 기준은 계속 유지한다.


## 2026-09-29 Web·Windows·Android 후속 실행

[최신 구현·실행 근거와 잔여](../results/2026-09-29-web-windows-android.md)를 참조한다.
이 문서의 이전 실행 결과를 새 PASS로 확대하지 않는다.


## 2026-09-29 Linux / Qt 후속

Qt Copy receiver 및 별도 IOsDragSourceHostCapability(QDrag text/URI/PNG/Copy·Move·Link)를 구현했다. native 합성 Copy/5GiB·해제·송신 취소 PASS; 외부 source/성공한 move·link/image/virtual-file 확대는 남는다.

상세 명령·환경·지원 경계: [Linux Qt 결과](../results/2026-09-29-linux-qt.md).


## 2026-09-29 macOS/AppKit 후속

AppKit NSView의 파일/text/URI Copy 수신을 연결하고 native pasteboard 해석/파일 grant를 검증했다. Finder 교차 창 물리 전달은 notVerified, 송신/Move/Link/virtual file은 unsupported다.
[구현·명령·결과·잔여](../results/2026-09-29-macos-appkit.md)를 따른다.

2026-09-29 iOS/Catalyst 후속: [별도 구현·실행 결과](../results/2026-09-29-ios-catalyst.md). UIKit 연결·native 재생성·Catalyst 두 scene·activation을 보강했으며 전체 물리 입력·GPU·배포 완료와 구분한다.
