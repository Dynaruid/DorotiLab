# M0. 실행·검증·지원 현황 정리

원문: [개발 로드맵](../../plan.md) §3 M0 · 우선순위: **P0** · 작업 상태: **로컬 기반 완료** · 실행 검증: **범위별 PASS / 나머지 notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

CLI 실패 전파, 테스트 위치, CI, 문서 경로와 지원표를 공통으로 관리한다.

선행: 없음. 모든 작업의 실행기·지원 상태·증거 기록 기반이다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md) · [Android](../platforms/android.md) · [macOS / AppKit](../platforms/macos.md) · [iOS](../platforms/ios.md) · [Linux / Qt](../platforms/linux.md) · [Mac Catalyst](../platforms/maccatalyst.md)

## 원문 작업과 완료 기준

원문의 기능 체크리스트와 완료 기준을 바탕으로, 테스트 위치·산출물 규칙은 [M1의 임시 테스트 관리 계획](01-testing.md#임시-테스트의-배치실행정리)에 맞췄다. 특정 호스트를 명시한 항목은 연결된 플랫폼 문서에서 해당 구현·검증을 추적하고, 이 문서에는 공유 계약 및 통합 결과를 기록한다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**목적:** 새 체크아웃에서 어떤 명령이 동작하고 무엇이 검증됐는지 확인할 수 있게 한다.

- [x] `doroti validate/audit/release`의 호출 경로를 점검하고 유지할 suite를 현재 제품 구조에 맞게 다시 연결한다. 삭제된 과거 suite 이름만 남겨 성공으로 처리하지 않는다.
- [x] 검증 실패·실행기 누락이 최상위 명령의 실패 종료 코드로 이어지도록 하고, 모든 필수 단계가 성공했을 때만 `Release: PASS`를 출력한다.
- [x] 일회성 테스트 스크립트·프로젝트·fixture는 저장소 루트 `temp/testing/<작업-ID>/<실행-ID>/`에 구성하고 검증·요약 후 삭제한다. 상시 유지할 최소 회귀 소스만 `Doroti/tests/`, 공통 실행기만 `Doroti/eng/`에 선별해 남긴다. 삭제된 `Doroti/validation` 전체를 되살리는 방식은 피한다.
- [x] 테스트 실행에 20분 timeout을 적용하고 timeout 시 자식 프로세스 종료와 실패 기록을 남긴다. 테스트 원시 산출물은 해당 `temp/testing/` 실행 폴더에 모으고, 결과 요약 후 정리한다. 일반 제품 빌드 산출물과 기존 `Doroti/artifacts`는 기존 정리 지침을 따른다.
- [x] `/temp/`를 Git ignore와 관련 빌드·프로젝트 검색·watch·pack 대상에서 제외하고, 임시 테스트를 기본 solution·제품 참조·전체 CI에 연결하지 않는다. 실행별 폴더 생성·종료·정리와 기존 일회성 파일 분류는 M1의 임시 테스트 규칙을 따른다.
- [x] README 한·영판, 지원표, 샘플 문서의 깨진 경로·폐기된 도구 요구사항을 수정한다. 필요한 역사적 근거는 `history/`의 실제 존재하는 기록으로 연결한다.
- [x] 플랫폼/호스트/renderer/build mode별 지원표를 하나의 기준으로 정리한다. `구현됨`, `자동 검증`, `실행 검증`, `물리 검증`, `미지원`을 구분한다.
- [x] CI에 소스·문서 경로 검사, 선택한 공통 테스트, Windows 기본 runner와 Web 빌드/기동 smoke를 연결한다. GPU/실기기 검증은 해당 장비가 있는 실행 경로로 분리한다.

**완료 기준:** 새 체크아웃에서 문서에 적힌 최소 검증 명령을 실행할 수 있고, 의도적인 실패가 정상적으로 실패 처리된다. `temp/testing/`과 삭제 가능한 artifacts가 없어도 제품 빌드·상시 회귀 검증이 동작하고 최신 지원 상태와 재현 절차를 읽을 수 있다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

## 2026-09-28 실행 결과

- 상태: **로컬 최소 기반 완료 / PASS**. CI workflow 연결은 완료했으며 원격 CI 실행은 **notVerified**다.
- revision: a93c047fe2e93d93cff3e0a6bf3c2789862fea81 + 작업 트리 변경 (미커밋). Windows 10.0.26200, .NET SDK 10.0.400, Python, PowerShell 7.
- `doroti.ps1 validate -ValidationSuite Source`: 현재 README/works 링크, temp 추적 금지, 실패 종료·timeout 회귀 PASS. 별도 임시 CLI 복제본에서 하위 exit 7 및 실행기 누락을 넣어 validate/audit/release가 실패하고 Release: PASS를 출력하지 않음을 확인했다.
- `Developer`: 실제 framework pointer 경로의 Cupertino 회귀와 virtual timer timeout/종료 정리 PASS. `Packages`: Testing/Cupertino/Desktop 의존 패키지를 만들고 저장소 ProjectReference 없이 별도 소비 앱·별도 NuGet cache에서 회귀 실행 PASS.
- Windows App SDK와 Web Debug build는 경고 0, 오류 0. Windows native/API close 및 native view 수명 smoke PASS. Web HTTP bootstrap asset 및 Chrome 실제 Material 화면·semantics 트리 확인 PASS. physical Windows UI 조작 도구는 native pipe 연결 실패여서 실제 resize 픽셀/물리 IME로 승격하지 않았다.
- 실행기: `Doroti/eng/validate.ps1`, `validate.py`, `run-with-timeout.py`. 모든 suite는 1,200초 제한, 실패 시 원인·실행 경로 출력, 성공 시 요약 후 소유 폴더 삭제. `/temp/` ignore, root/sample props, App SDK source glob, launch fingerprint에서 임시 소스를 제외했다. 테스트 executable은 제품 solution에 추가하지 않았다.
- 문서/지원 기준: [현재 지원표](../../Doroti/docs/support-status.md), [실행 안내](../../Doroti/tests/README.md). 삭제된 validation 디렉터리를 복원하지 않았다. 현재 안내의 끊어진 링크는 존재하는 archive로 연결했다. archive 내부의 역사적 원시 링크 전체를 현행 실행기로 해석하지 않는다.
- 성공한 aggregate suite는 실행 폴더를 자동 정리했다. 수동 확인 폴더와 실패 조사 폴더의 최종 삭제는 자동 승인 정책에 차단되어 아래 정리 보류 목록을 따른다. 제품 bin/obj는 유지했다. 전체 플랫폼 release pack·설치·서명은 이번 PASS 범위가 아니다.
- 다음: M1 최소 경로는 준비됐다. M2-A/B의 물리 입력·표시 검증을 이어가며 04 렌더링 기준을 적용한다.

### 임시 정리 보류

최종 삭제 명령 및 경로를 좁힌 개별 삭제 모두 자동 승인 검토에서 `blocked by policy`로 거절됐다. 추가 이유는 제공되지 않았다. 삭제를 우회하지 않았으며 다음 무시된 로컬 산출물이 남아 있다. 제품 소스/영구 fixture는 삭제 대상이 아니다.

- `temp/testing/m0-m3/`: 수동 build/browser/native 실행 로그와 결과 JSON.
- `temp/testing/packages/14bca38bcff7498bbc1a2993d281af99/`: 수정 전 restore source 인수 실패 조사. 이후 Packages는 PASS.
- `temp/testing/windowssmoke/e9cb6364d0fd4fdf8dbe4143c2e6691b/`: 수정 전 close callback 경쟁 조건 조사. 이후 공통 계약 및 WindowsSmoke는 PASS.
- `temp/testing-widget.log`, `temp/testing-windows-build.log`, `Doroti/tests/Doroti.Tests/bin/`, `Doroti/tests/Doroti.Tests/obj/`: 초기 직접 실행 산출물. Python bytecode도 ignore 대상이다.

정리 시점: 삭제 정책이 허용하는 사용자/후속 세션에서 위 절대 경로가 이 workspace 내인지 확인한 뒤 이 목록만 삭제한다. 성공한 aggregate suite의 실행 폴더는 이미 자동 삭제됐으며 기본 CI/build/pack의 입력에 포함하지 않는다.
