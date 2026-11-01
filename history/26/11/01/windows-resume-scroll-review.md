# Windows 백그라운드 복귀·스크롤 검토

- 날짜: 2026-11-01, Asia/Seoul.
- 기준 HEAD: `a3ce8df0f6afcb2f5ebbbcb9515e68dca757e71d` + 기존 Android/공통 미커밋 변경.
- 범위: 기본 Windows App SDK/HwndExactCpp와 별도 Windows MAUI 러너.
- 판정: Windows 장면 검사 할당 수정·회귀·두 Release 빌드 통과. 기본 러너의 GC/복귀 기능 검사와 사용자 복귀 스크롤 체감 확인 통과. 별도 트랙패드·표시 frame pacing은 `notVerified`, 전체 성능은 `PARTIAL`.
- 수치·로그 발췌·소스/바이너리 hash: [결과 JSON](windows-resume-scroll-results.json).

## 확인한 문제와 수정

기본 Windows 호스트의 `WindowsPlatformViewHost.Draw`는 shader-only 화면에서도 매 프레임
retained 장면 트리를 검사한다. 기존 재귀 `Any`는 깊이를 캡처하는 closure와 열거자를
각 retained 층에서 만들었다. Windows MAUI의 같은 검사도 인터페이스 열거자를 반복 생성했다.
스크롤 중 불필요한 GC 압력을 추가하는 실제 할당 경로다. 이 할당을 Android에서 관측한
복귀 후 Mono/Java GC bridge 정지와 같은 원인이라고 판정하지는 않았다.

두 러너가 이미 함께 컴파일하는 `packages/platforms/shared/Windows`에
`WindowsPlatformSceneInspection`을 두고 인덱스로 순회하도록 고쳤다. 매번 현재 명령 트리를
읽으므로 native view와 input shield를 계속 발견한다. 기본 러너의 256단 깊이 제한을 유지하고
MAUI에도 같은 제한을 적용했다. 장면 판정을 캐시하거나 GPU 캐시를 지우는 처리는 추가하지 않았다.

수정 전 Release 바이너리의 private 메서드를 reflection으로 찾아 직접 delegate로 호출하고,
동일한 24단 retained fixture를 새 production helper에 전달했다. 각각 5회 준비 후 20회 검사:

| 검사 | 수정 전 Windows App SDK | 수정 후 공용 Windows 검사 |
| --- | ---: | ---: |
| 현재 스레드 관리 할당 | 44,640 bytes | 0 bytes |

이 값은 장면 검사만의 결과다. 전체 프레임 할당·스크롤 FPS 개선 수치가 아니다.
기준 바이너리는 측정 뒤 재빌드로 교체되어 이전 DLL hash는 기록하지 못했다. 결과 JSON의
바이너리 hash는 최종 수정본이다. 기본 러너 프로파일의 host DLL hash는 `fe3fdcf6…`로 별도 보존했다.
이후 공용 폴더에 추가한 UIA helper는 기본 러너에서는 사용되지 않지만 함께 컴파일된다.
최종 App SDK 빌드와 진단 없는 일반 실행도 별도로 확인했다. 기존 공통 immutable picture/scene 수정은 그대로 포함한다.

### MAUI 접근성 조회의 임시 객체 증가

추가 MAUI 실행에서 강제 GC 후에도 관리 힙 311.12 → 225.02 → 290.81MB가 수집됐다.
큰 객체 대부분은 WinRT/COM `ManagedObjectWrapperHolder[]`와 UIA `IList<AutomationPeer>`
관련 테이블이었다. `SemanticsPeer.GetChildrenCore`가 같은 구조에서도 매번 새 LINQ 결과 목록을
WinRT에 반환하는 경로를 확인했다.

native 자식의 참조·순서가 같으면 동일한 읽기 전용 peer 목록을 반환하는
`WindowsPeerChildrenCache`로 수정했다. 자식 추가/제거/순서/handler 교체는 새 목록으로
반영하고 준비되지 않은 peer는 다음 호출에서 재시도한다. label/value는 기존 peer가 최신
semantic node를 읽는 방식을 유지한다. 변경 없는 조회 20회에서 helper 할당은 0 bytes였다.

최종 MAUI 실행에서는 관리 힙이 54.75 → 39.45 → 38.64MB로 수집됐다. 서로 다른 실행 시간과
UIA 조회 이력을 가진 관측이므로 통제된 스크롤 성능 전후 비교로 쓰지 않는다. 접근성 기능은
유지하며 Typography의 새 글자 목록, Components 복귀 시 이전 `Display Large` 제거와
현재 `Actions Common buttons` 출현을 실제 UIA 트리에서 확인했다.

## 실행 검증

저장소에 문서화된 `dotnet run --project ... -c Release` 경로로 실행했다.
빌드 후 실행에는 `--no-build`를 사용했다. 기본 러너에는 기존 opt-in 진단 환경 변수만 설정했다.
실제 창 조작은 computer-use/Sky로 수행했다.

- 기본 러너 수정 전: 두 Components 목록 휠 이동, 최소화 중 GC, 복귀 후 휠 이동, 정상 닫기.
- 기본 러너 수정 후: 두 목록을 이동해 콘텐츠를 준비하고, **최소화 → GC → 복귀 → 긴 휠 스크롤 3회**.
- 세 번째 복귀 후 Platform views를 열어 실제 native editor의 값을 `Windows resume check`로 변경했다.
  Components로 돌아오면 native editor가 UI 트리에서 사라지고 shader 화면이 복구됨을 확인했다.
- 기본 러너 종료: exit 0, failed/dropped 0, terminal ledger 등록/완료 5,821/5,821,
  active/duplicate 0, 마지막 `complete=true pending=0`.
- GPU/수명: context/surface generation 1/1, device generation 1, surface recreate/loss recovery 0.
  D3D12 copy 1,327/1,327, outstanding acquire/copy 0. 종료 전 pipeline snapshot의 pending 1과
  이후 최종 drain pending 0은 서로 다른 시점의 기록이다.
- 최종 Scroll 회귀 13개 출력 통과. 실제 Windows DirectManipulation manager의 두 HWND 등록·해제,
  native 관성 종료와 재접촉, 새 Windows helper의 native/shield/깊이 제한·무할당 검사를 포함한다.
- 최종 Windows App SDK Release: 경고/오류 0/0, 27.55초. 최종 Windows MAUI Release: 0/0, 34.91초.
- MAUI 첫 실행에서 사용자 입력 감지로 자동 검사를 보류하고 상태를 갱신했다. 사용자가
  입력 공백을 알려준 뒤 긴 휠·최소화·GC·복귀·정상 종료를 통과했다. UIA 수정 후에는 새
  프로세스에서 최소화/복귀 2회, GC 3회, 긴 휠, Typography/Components 교체와 정상 종료(exit 0)를 확인했다.

`dotnet-gcdump 10.0.750501`을 작업 전용 `temp` 도구 폴더에 설치해 대상 PID의 GC를 유도했다.
오래 백그라운드에 두고 기다리는 대신 최소화 중 collect를 실행했다.

| 수정본 GC | 관리 힙 bytes | 관리 객체 |
| --- | ---: | ---: |
| 1 | 19,079,725 | 214,173 |
| 2 | 19,197,295 | 214,434 |
| 3 | 19,220,245 | 214,495 |

이 3회에서 큰 누적 증가는 관측하지 않았다. 스냅샷 시점의 화면·일시 객체 차이는 남는다.
도구는 .NET 10에서 lossy EventPipe buffering을 사용하며 non-lossy 모드는 .NET 11 이상이
필요하다고 알린다. 전체 native/GPU 메모리 누수 부재를 증명하는 검사는 아니다.

MAUI 최종 수집은 54,745,612 / 39,448,220 / 38,635,436 bytes였다.
마지막 두 수집은 같은 두 번째 최소화 구간에서 수행했다.

## 측정 경계와 재실행

주입 입력은 mouse wheel로 기록됐다(수정본 7 packets, panZoom 0). 실제 손가락을 뗀 뒤
트랙패드 관성은 이 검사로 판정할 수 없다. UI screenshot과 GPU submit도 표시 FPS 증거가 아니다.
전체 진단 실행의 초기 2.263초 시점에 owner 1,287.756ms가 있었고 lazy loading·화면 전환을
포함한 다른 긴 작업도 남았다. 전체 실행 최대값을 로드 완료 후 복귀 스크롤의 측정값으로
해석하지 않는다. 사용자는 기본 Windows 일반 실행의 복귀 스크롤이 부드럽다고 확인했다.
입력 기기가 특정되지 않아 별도 물리 트랙패드 관성과 frame deadline 판정은 남긴다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj -c Release -m:1 -nr:false
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -c Release -m:1 -nr:false
```

최소화한 테스트 프로세스에 GC를 유도할 때:

```powershell
$testbedTargets = @(Get-Process DorotiTestbedApp.WindowsAppSdk -ErrorAction Stop)
if ($testbedTargets.Count -ne 1) { throw 'Testbed 프로세스를 하나만 실행하고 다시 확인하세요.' }
$testbedProcessId = $testbedTargets[0].Id
python Doroti/eng/run-with-timeout.py --timeout 1200 ./temp/windows-resume-review/tools/dotnet-gcdump.exe collect -p $testbedProcessId -o ./temp/windows-resume-review/recheck.gcdump
```

도구 폴더가 없으면 같은 timeout wrapper로
`dotnet tool install dotnet-gcdump --tool-path ./temp/windows-resume-review/tools`를 먼저 실행한다.

`temp/windows-resume-review`의 dump/원시 JSON/도구는 삭제 가능한 로컬 산출물이다.
추적되는 결과 JSON에 필요한 집계·느린 작업·최종 drain·hash를 별도 보존했다.
기존 Android 기록 및 사용자 확인 범위는 유지한다.

## 종료와 일반 실행 전달

모든 진단 실행은 정상 닫아 exit 0을 확인했다. MAUI 검사 프로세스는 남기지 않았다.
최종 기본 Windows App SDK 실행 파일을 진단 환경 변수 없이 일반 실행으로 열었다.
전달 시 PID는 5824, HWND는 655390이며 시작 화면은 Components다. 프로세스/사용자 범위의
`DOROTI_WINDOWS_FRAME_TIMING`과 `DOROTI_WINDOWS_APPSDK_REPORT`는 설정되어 있지 않다.
자동 입력을 끝낸 뒤 사용자가 2026-11-01(Asia/Seoul)에 **“복귀 뒤에도 부드러워”**라고 확인했다.
기본 Windows 일반 실행에서 충분히 스크롤하고 최소화·복귀한 조건의 체감 확인으로 기록한다.
입력 기기 종류와 표시 FPS를 측정한 결과로 확대하지 않는다. 커밋은 만들지 않았다.
