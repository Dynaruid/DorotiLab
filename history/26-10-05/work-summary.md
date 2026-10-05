# 종합 리뷰 후속 작업 결과 요약 — work.md

원문 작성일: **2026-10-04 KST** · 보관일: **2026-10-05 KST**.

**F01–F10·A01–A08·D01–D07의 수정·자동화와 선택 호스트 검증 완료, 제품 전체 PARTIAL**을 보존한다. 원문의 실행 체크는 54개 완료·9개 미완료다. 체크 완료는 명시한 구현/자동화 범위이며 Apple·Android·물리·배포 인수를 함께 완료했다는 뜻이 아니다.

검토 보고서 기준은 `61f0f23b8f112e8e277dc896cf0b2c6627aca878`, 실행 checkout은 `4bbcd6eb95d32c092054815f78f63dcf19323fcf`와 당시 변경이다. 외부 보고서 `Doroti-full-review-61f0f23.md`는 doctor 후속 검토 당시 저장소에 없었다. 변경 전 checkout의 별도 실행 결과를 주장하지 않는다. 상세 명령·환경·후보 identity는 [실행 결과](../../Doroti/docs/validation/2026-10-04-full-review.md)와 [결과 JSON](../../Doroti/docs/validation/2026-10-04-full-review.json)을 따른다.

## 목표와 보존 계약

Future 완료와 callback 소유권, 프레임 중간 microtask, 최신 semantics, 탐색 정책, startup/종료/연결 수명, 실제 Scene snapshot, 설치 복구와 개발 세션 소유권을 수정했다. doctor는 선택 app/runner/SDK/작업의 필수 조건을 진단하도록 보강했다.

플랫폼 중립 App/Runner 분리, view별 capability·dispatcher scope, exact-frame transaction, GPU consumer 완료 뒤 자원 회수, 기존 플랫폼 제약을 유지했다. 새 위젯/호스트, 대규모 재작성, renderer 기본값 전환이나 CI 복원은 범위 밖이다. 후속 [구조 전환](work2-summary.md)의 검증으로 이전 리뷰 PASS를 재사용하지 않는다.

## 단계별 수정과 회귀

실행 순서는 **R0 → R1 → R2 → R3 → R4 → R5 → R6 → R6-D → R7 → R8**이었다.

| 단계·지적 | 구현과 확인한 동작 | 검증 경계 |
| --- | --- | --- |
| R0 / F06·F10 | `AllowedOrigins=null`은 미설정, `[]`는 HTTP(S) 허용 origin 없음. 거절 시 navigation 상태 유지. TypeScript Build/Clean에 동일한 정규화·경로 경계·OS별 case·symlink/reparse 검증 적용 | 실제 Windows와 Linux ext4 MSBuild fixture로 sentinel 보존 확인. iframe 내부 redirect 관찰과 app-content 정책은 별도 |
| R1 / F01·F02 | captured dispatcher의 acceptance/lifetime 보존, 거절·취소·예외에도 Future 단일 terminal. typed/untyped 오류 handler와 filter도 owner queue에 전달. scheduled task 동기 예외는 completer 오류로 완료 | 두 dispatcher 격리·owner 종료·경쟁 및 Debug/Release 회귀. 다음 task의 진행과 원래 stack 보존 |
| R2 / F03 | begin 뒤 draw 전에 같은 microtask queue drain, warm-up/forced/nested/다중 view의 phase·scene token 유지 | 실제 `A M B C` 순서와 같은 frame의 상태 반영 확인. GPU 표시/FPS 증거와 구분 |
| R3 / F07·A02 | semantics A 적용→B 대기→새 A에서 pending B 취소/대체. Apple checked/toggled/mixed·disabled/obscured 투영 정책 | 공통 정책 회귀 PASS. AppKit/Catalyst/iOS 실제 provider와 VoiceOver 수락은 당시 미검증. Android non-Graphite fallback과 Graphite를 구분 |
| R4 / F04·A01·A05·A06 | startup await 뒤 lifetime 재확인, cleanup 예외를 모으고 후속 정리/terminal 계속 진행. managed 연결 ACK·취소·늦은 port/role 종료. desktop canvas는 부족한 축만 확장하고 dimension/byte 상한 적용 | production Worker 실패 주입·timeout/재연결·부분 생성·중복 dispose·두 owner 격리. runtime pthread 임의 terminate 및 GPU timeout 조기 해제 금지. canvas 계산을 실제 GPU 메모리/FPS로 해석하지 않음 |
| R5 / F05·A03 | renderer capability를 통한 owned Scene rasterization, readback/encode/재페인팅 가능한 Image. 공유 storage identity로 `isCloneOf` 판정 | RGBA/alpha/clip/transform·PNG 왕복·RepaintBoundary/SnapshotWidget·dispose/lease 검사. 미지원 renderer/native content/sync는 생성 때 명시 거절 |
| R6 / F08·F09·A04·A07·A08 | owned `current.new` 복구와 설치 교체 실패/재시도/userdata 보존. run 전용 macOS receipt 검증. stdout/stderr 동시 drain·bounded process tree 종료. Python 절대 경로 공유. Android device/package/session 소유권과 stale Stop 보호 | Linux 실제 설치·MSBuild와 프로세스/mock ADB 회귀. 실제 Mac 후보 receipt·Android 장치 delta/Restart/Stop은 당시 SKIPPED |
| R6-D / D01–D07 | 실제 runner/CWD/dotnet/global.json·profile 기반 prerequisites, OS별 wrapper/Xcode·SDK/native 도구, bounded probe, 구조화된 `doroti.doctor/v4` report/exit와 CLI 회귀 | fake tool/workspace 및 Windows/WSL spot check. 공통 도구 PASS·build 준비 PASS·실제 build/runtime/device 수락을 별도 판정 |
| R7 | 새 회귀를 로컬 집계에 등록, 실제 템플릿·소스 runner·격리 feed/cache의 독립 NuGet-only Release 소비자, 지원/개발/릴리스 문서 정합성 | 각 F/A/D ID에 명령·상태·해시 연결. 과거 후보의 결과를 새 후보에 소급하지 않음 |
| R8 | 선택 Windows App SDK/MAUI·Chromium 및 package-only/native smoke 수행 | 선택 조합의 일부 인수. 물리 입력·AT·display/scanout·장기 운전·실제 서명/clean-machine 인수는 남음 |

## doctor의 작업별 의미

`-DoctorProfile common|build|dev|validation|release|compiler-development`를 제공했다. 대상 미지정 기본값은 common, 대상 지정 기본값은 build다. 요구 버전은 실제 project/native metadata를 읽고, 선택한 작업에 해당하는 조건만 검사한다.

- Windows App SDK와 MAUI의 runner/workload/native graph를 구분하고 Vulkan/D3D12/WebView2 실행은 runtime 수락으로 남긴다.
- Web 일반 build에 Node/Rust를 무조건 요구하지 않는다. 브라우저 COOP/COEP/CSP·SharedArrayBuffer·adapter는 실제 서버/브라우저 검사 대상이다.
- Android build에는 연결 장치가 필수가 아니다. dev에는 authorized serial·ABI·Debug metadata profile이 필요하고 signing은 release 범위다.
- Apple은 선택 SDK/global.json·full Xcode/target SDK를 확인하며 simulator/device/provisioning/NativeAOT 조건을 선택 profile에만 적용한다.
- Qt는 Quick/Widgets/WebEngine·native build flags별 버전/도구를 확인하고 display/QPA/driver 실행과 managed-only cross-build를 구분한다.

필수 누락은 FAIL/nonzero, 필수 미확인/timeout은 PARTIAL/nonzero, optional 부재는 WARN/notApplicable이다. doctor는 restore/build/workload 설치·다운로드·장치 실행을 자동 수행하지 않는다. `all`의 foreign-host/미확인은 전체 준비 PASS로 합산하지 않는다. 현재 도구 계약은 [doctor 안내](../../Doroti/docs/doctor.md)를 따른다.

## 기록된 검증과 남은 수락

수행한 범위는 Source/Developer/Targets/Packages, Debug/Release CPU, production Worker, Windows App SDK/MAUI·세 Chromium 조합, Linux ext4 설치/MSBuild 및 새 독립 NuGet 후보다. 당시 Apple native build/provider/macOS Release와 Android 실제 장치/metadata delta/Stop은 환경 부재로 미검증이었다.

| 미완료 원 항목 | 보존하는 잔여 |
| --- | --- |
| R3-4 | Apple 실제 Material Checkbox/Switch provider 상태와 VoiceOver 발화·action |
| R6-5 | 실제 Mac NuGet-only Release candidate의 이번 receipt·exit·서명 범위 |
| R6-10 | 실제 Android에서 stale Stop/새 owner 경쟁, metadata delta·상태 유지·Restart/Stop |
| RD-11 | 실제 Mac/Linux/Windows의 선택 profile과 이후 동일 context 실행의 전체 대조 |
| R8-0 | Linux app graph의 30초 doctor probe timeout **PARTIAL**. 실제 Qt build/smoke PASS와 구분 |
| R8-2·R8-3 | 물리 IME·AT 발화/action·monitor/DPR 이동·scanout/FPS. native/synthetic resize/loss/다중 창/회수 PASS로 대체하지 않음 |
| R8-4 | 세 Chrome profile의 restart 뒤 60초 PASS 외 모든 native host의 1분 지속·재연결 |
| R8-5 | 실제 서명/배포 대상의 clean-machine 설치·업데이트 중단 복구·제거·userdata 유지 |

후속 Apple/Android 결과는 [work3 요약](work3-summary.md)에 별도 날짜·후보·실행 범위로 보관했다. 리뷰의 SKIPPED나 미완료 체크를 후속 전체 PASS로 바꾸지 않는다.

## 재실행 진입점과 증거

`Doroti/eng/validate.py`의 Source/Developer/Targets/Packages, `Doroti/tests/Doroti.Tests`, `doctor_contract.py`, `full_review_tools.py`, `android_development_bridge.py`, Web worker/rendering/texture 회귀와 host별 smoke를 사용했다. 테스트 외부 timeout은 **1,200초**, 통상 반복은 **30회 이내**다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py Source
python Doroti/eng/run-with-timeout.py --timeout 1200 pwsh -NoProfile -File Doroti/eng/doroti.ps1 doctor -App samples/DorotiTestbedApp -Platform windows -WindowsBackend WindowsAppSdk -DoctorProfile build
```

상세 명령/후보는 연결한 validation 문서·JSON을 따른다. `temp/testing/full-review/`와 `Doroti/artifacts/`의 raw는 현재 존재를 보장하지 않는다. 이번 보관은 제품 검사를 새로 수행하지 않았다.
