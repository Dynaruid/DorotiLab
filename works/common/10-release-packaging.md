# M7. 외부 패키지 소비·배포·출시 기준

원문: [개발 로드맵](../../plan.md) §3 M7 · 우선순위: **P3** · 작업 상태: **PARTIAL — 선택 플랫폼 후보 패키지·격리 소비** · 실행 검증: **범위별 PASS / 배포 notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

호환 정책·migration·clean 소비 앱·지원표·출시 증거 형식을 담당한다. 서명·설치·업데이트·AOT는 플랫폼별 조합을 검증한다.

선행: 출시 대상 기능의 선행 단계. package-only smoke는 [M0](00-foundation.md)부터 시작하며 모든 선택 기능 완료를 첫 출시의 필수 조건으로 삼지 않는다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

원문의 범위와 완료 기준을 유지하면서 2026-09-28~29 실행 기록에 따라 완료 부분과 잔여 부분을 분리했다. 체크된 항목은 명시한 호스트·검증 범위에 한정하며, 아래 날짜별 결과가 근거다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**선행:** 출시 대상 기능의 앞선 단계 완료. 모든 선택 기능 완료가 첫 배포의 필수 조건은 아니다.

- [x] private template hive의 `dotnet new doroti-app`과 격리 NuGet cache로 Windows/Web package-only Release publish를 검증했다. Windows self-contained JIT 실제 창 실행 및 로컬 portable 설치/업데이트/제거는 아래 범위에서 PASS다.
- [ ] 후속 보강을 포함한 새 후보를 만들고 같은 revision/toolchain·package/payload hash로 결과를 연결한다. 기존 후보에 이후 수정의 PASS를 합치지 않는다.
- [ ] clean OS/VM의 Windows/Web 설치·실행·업데이트와 Android package-only 소비를 검증한다. 나머지 host를 순차 확대한다.
- [x] SDK/runner/native library/font/Web asset/플러그인 버전의 호환 정책, API 변경 안내와 migration 문서를 정한다.
- [ ] Release, trimming, Mono AOT/NativeAOT/Web publish 중 실제 지원하는 조합을 명시하고 각 조합을 검증한다.
- [ ] 플랫폼별 서명·패키징·설치·업데이트·제거와 앱 데이터 유지, crash/로그 수집을 검증한다. 개발자 머신 성공을 clean 배포 성공으로 간주하지 않는다.
- [ ] 리소스 누수·장기 스크롤·창/PlatformView 반복 생성·GPU loss/앱 복귀 회귀를 릴리스 후보에서 확인한다.
- [ ] 제한과 미검증 항목이 포함된 출시 지원표, 실행 가능한 샘플, 문제 보고 양식을 함께 제공한다.

**완료 기준:** 선택한 플랫폼에서 저장소 소스와 개발자 로컬 cache 없이 앱을 설치·실행·업데이트할 수 있다. 배포 패키지와 검증 결과가 같은 revision·toolchain을 가리킨다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

## 2026-09-29 구현·검증

후속 보강: installer는 staging 복사·checksum 검증 후 version 디렉터리로 승격한다.
두 번째 파일 복사 실패 주입→이전 current 유지→같은 버전 재시도, 변조/추가 DLL 거절,
제거 후 한글 userdata 보존 회귀 PASS. 개발 머신의 작은 fixture이며 실제 서명/clean VM
배포 증거는 아니다. [보강 검토](../README.md#2026-09-29-0010-보강-검토).

Revision: `8834d7596597b3087a0139f527148baee9a46583` + 이번 작업 트리.
계약·재현·지원 조합: [release-candidates.md](../../Doroti/docs/release-candidates.md).

- `release-candidate.py` 및 `doroti release -Platform windows|web`에 선택 플랫폼 Release
  package graph, 새 버전, private template hive, `dotnet new doroti-app`, 별도 NuGet/HTTP cache,
  package-only publish 경로를 추가했다. source revision·작업 트리 hash·toolchain·패키지 및 payload
  SHA-256과 검증 결과를 같은 candidate manifest에 기록한다. 공개 저장소로 업로드하지 않는다.
- Windows self-contained JIT 소비 앱에서 실제 native 창 2개·첫 present·resize·종료 **PASS**.
  Web은 trimming/AOT를 끈 Release publish와 비어 있지 않은 bootstrap/wasm payload **PASS**.
  처음 package-only Web 빌드에서 발견한 loader의 texture type 선언 누락을 두 SDK package에 수정했다.
- versioned portable 설치/업데이트/제거 도구를 추가했다. payload hash를 확인하고 current 포인터를
  원자적으로 바꾸며 userdata를 유지한다. MSIX/Start Menu/protocol 등록·신뢰 서명을 대신하지 않는다.
- GUI crash를 LocalAppData의 앱별 `last-crash.txt`에 남기며 실행 실패는 계속 전파한다.
- Galaxy S25 Android 16에서 Testbed Release/Mono AOT 설치 및 업데이트 후 cold/warm 링크와
  강제 종료 후 route 복원 **PASS**. 이는 소스 기반 Testbed APK이며 Android package-only 소비와 다르다.

서명 인증서·격리된 출시 설치 환경·배포 계정은 제공되지 않았다. clean VM 설치,
서명·공개 업데이트·스토어 제거, Windows 전체 renderer trimming/NativeAOT, Web AOT,
Android package-only 소비 및 장기 soak/device-loss는 **notVerified**다.
개발자 머신의 격리 cache 성공을 clean OS 배포 완료로 체크하지 않는다.

### 2026-09-29 후보 생성 당시의 최종 후보와 로컬 설치 결과

후보 생성 당시 최종 버전: `0.3.0-beta.rc.20260929010529`.
이후 00~10 보강에서는 새 Release 후보를 만들지 않았다. installer staging/retry·종료·복원
수정의 로컬 회귀 PASS는 이 후보 payload의 검증 결과가 아니며, 출시에 앞서 새 후보가 필요하다.
`Doroti/artifacts/release/0.3.0-beta.rc.20260929010529/`에 NuGet 27개,
Windows payload 549개 파일, Web payload 796개 파일과 `candidate.json`을 생성했다.
candidate의 source-tree SHA-256은
`0af89d547b6bc6e91dfa85fe89a621e934a3fccbbbb6993191f2ecc4a650a930`이다.
문서/테스트 후속 편집과 구분되는 후보 생성 시점의 snapshot이다.

이전 후보 `0.3.0-beta.rc.20260929004147`를 전용 임시 디렉터리에 설치한 후 최종 후보로
업데이트했다. 설치된 self-contained exe의 두 native 창 실행/종료 exit 0,
실행 중 NuGet cache 비어 있음, 설치 payload 변조 시 거절·current 유지,
버전 제거 후 userdata의 한글 canary 유지가 **PASS**다.
이는 developer 머신의 portable install/update/remove 검증이며 실제 앱의 모든 입력 상태
migration이나 clean VM 설치·서명을 뜻하지 않는다.

마지막 Android Release/Mono AOT 재빌드는 경고 0·오류 0이다. 앞선 route 실기기 검증 뒤
ADB 목록에서 기기가 사라져, 마지막 재빌드 APK의 재설치는 수행하지 못했다.


## 2026-09-29 Web·Windows·Android 후속 실행

[최신 구현·실행 근거와 잔여](../results/2026-09-29-web-windows-android.md)를 참조한다.
이 문서의 이전 실행 결과를 새 PASS로 확대하지 않는다.


## 2026-09-29 Linux / Qt 후속

Linux 후보 0.3.0-beta.qt.20260929: 25개 패키지·격리 cache 소비·Release/JIT publish/run, portable install/update/변조 거절/remove·userdata 보존 PASS. 서명·clean OS·글로벌 protocol 등록·장기 soak는 notVerified.

상세 명령·환경·지원 경계: [Linux Qt 결과](../results/2026-09-29-linux-qt.md).


## 2026-09-29 macOS/AppKit 후속

macOS 후보 생성/격리 NuGet template consumer/Release pkg 확장·실행/서명 검사 경로를 추가했다. macOS non-trim은 SDK 요구에 맞춰 PublishTrimmed=true + LinkMode=None이다. 최종 후보와 clean OS/Developer ID·공증 잔여를 아래 결과에 분리한다.
[구현·명령·결과·잔여](../results/2026-09-29-macos-appkit.md)를 따른다.
