# Cross-platform 부트 작업 계획 보관 요약

- 정리일: **2026-09-07**. 사용자 요청으로 루트 `plan.md`를 요약·원문 보관 후 삭제한다.
- 원래 계획 재작성: 2026-09-06, 분석 HEAD `0499c1d`.
- 기록된 구현 실행: 2026-09-07, 시작 HEAD `ccf61de` clean tree에서 source 재확인 후 수행.
- 종합 판정: **공용/Web/Windows/CLI 구현과 가능한 target 검증 완료, 전체 플랫폼 acceptance 미완료**.
  문서 정리는 제품 수정·새 검증·미완료 gate의 완료 선언이 아니다.

## 보관 자료

- [삭제 직전 plan.md 원문](cross-platform-boot-plan-original.txt): 바이트 단위 복사본.
  SHA-256 `6C13E0AE30D157806E79415A0DEDBF8AF0784ADDCF28725B47CFF4DB8F7A1728`.
  원문 상대 경로는 저장소 루트 기준이며 §1–§2의 변경 전 분석을 최종 구현으로 읽지 않는다.
- [상세 구현·검증 결과](cross-platform-boot-results.md): 실행 label, target matrix, 실패 및 보류 설계.
- [원시 실행 증거](../../.doroti/evidence/boot/20260907-implementation/): 명령·exit·timeout·stdout/stderr·JSON·capture.
  `execution-index.json`의 69개 항목은 개별 실행 수이며 테스트 PASS 수가 아니다.
- [2026-08-28 선행 계획 원문](../26-09-06/cross-platform-first-boot-plan-2026-08-28.md),
  [2026-08-29 MVP 결과](../26-08-29/cross-platform-first-boot-implementation.md)도 유지한다.
- [Material 샘플 별도 작업](material-sample-work-summary.md),
  [Web direct 전환·resize·시작 지연 후속 계획](../../work2.md)은 별도 범위다.

## 1. 목표와 증거 계약

설치된 Release 앱의 첫 유효 content와 입력 준비까지 불필요한 직렬 작업을 줄이고,
CLI의 fingerprint/restore/build/native/AOT/deploy/launch 비용은 앱 runtime 부트와 별도로 다뤘다.

- first-install, process-cold, warm/resume, Web cold/repeat navigation, developer launch를 분리한다.
- launch/host-ready/framework-attached/first-scene/submit/content/input-ready/failure를 기존 trace와 연결한다.
- renderer·configuration·RID·artifact·session/context/surface/scene identity와 clock 기준을 기록한다.
- loader `started`/GPU ready/API present는 첫 content나 physical scan-out과 같지 않다.
  한글 값 삽입·ARIA 확인은 실제 OS 한글 조합기·screen reader 검증이 아니다.
- 같은 조건의 전후 raw launch 비교 없이 package 크기·할당·직렬 대기 감소를 TTID 개선률로 환산하지 않는다.
- 당시 기준은 Windows Vulkan/Composition, Web CanvasKit UI/Raster Worker였다.
  이후 사용자가 요청한 direct 전환은 `work2.md`에서 다루며, 이 부트 작업의 성과로 소급하지 않는다.

## 2. 단계별 구현과 보류

| 단계 | 수행 내용 | 유지·보류 사항 |
| --- | --- | --- |
| P0 부트 계약 | boot evidence v1, first content/clear 구분, F0/Testbed/dark autofocus, scene·identity 연결 | 물리 표시·실제 IME는 별도 |
| P1 Web | manifest 뒤 JS/WASM fetch·길이/hash 병렬 검증, peer abort, CanvasKit init과 dotnet.js import 중첩, 동일 font preload, document 전용 Blazor 옵션·metadata 정리, 동일 asset 재쓰기 방지 | runtime.create/StartWorker는 GPU/text ready 뒤 exactly-once. 두 CanvasKit instance 및 URL load 유지. verified bytes/module 재사용·배포 cache/압축 비교 보류 |
| P2 공용 first frame | MAUI Entry/Editor와 구독을 첫 client별 lazy 생성, 실제 MaterialSample light/dark theme memoization·무효화 | typed manifest 직접 생성/JSON parsing 제거는 보류. semantics 활성화/순서·public JSON validation·DLR 유지 |
| P3 Windows | presenter를 native 검사 전에 선택, normal Vulkan/D3D12의 불필요한 ANGLE 파일 검사 제외, 명시적 ANGLE/full audit·provenance/C9 동기화 | ABI/architecture·LUID/ownership·no app-local Vulkan loader/ICD 유지. 조용한 fallback 없음. 정상 D3D12 live/MAUI 실제 입력 미검증 |
| P4 target | Android 현재 DEX profile rebind/strict 검증, 두 RID publish와 x64 emulator 실행. Linux published Qt/WSLg 명시적 hardware API smoke | arm64 실기기/Apple 3제품/native Linux acceptance 미완료 |
| P5 CLI/SDK/template | binary·상속 MSBuild·evaluated 외부 input graph, dependency 탐색 pruning, source/toolchain/output hash를 묶은 launch-state v2, build 성공 후 atomic 기록, Qt target 공용 SDK 이동, source-wired template unsafe 설정 수리 | mtime-only cache 아님. 전체 미선언 외부 입력의 hermetic cache 아님. TypeScript compile 유지, NuGet release 설치·정량 CLI 성능 미검증 |
| P6 정리 | 원시 결과·artifact/source inventory·판정·실패·잔여 gate 문서화 | 체크된 단계도 모든 physical/performance acceptance PASS를 뜻하지 않음 |

Web 검증 후 Worker의 JS/WASM URL load가 여전히 관측됐다. JS 120,877 bytes,
WASM 7,317,345 bytes, fallback font 2,054,744 bytes는 해당 artifact 값이다.
publish 300개 파일 합 67,924,711 bytes에는 압축 대체 파일도 포함되어 navigation wire 크기가 아니다.
native WASM build를 managed AOT로 표기하지 않는다. cache 관련 음수 responseBodySize는 절감량으로 합산하지 않았다.

## 3. 실행 결과와 범위

| 영역 | 기록된 결과 | 남은 한계 |
| --- | --- | --- |
| 공용 | descriptor/negative/session, dynamic dispatch, FCR3/4/6/7, lazy-input lifecycle PASS | 플랫폼 native handler의 첫 focus 비용·실제 입력 |
| Web | build/publish/typecheck PASS, boot gates 9 PASS, persisted content 3+1 PASS, document input 2 PASS, topology/display-list/resize/recovery | physical IME/accessibility/mobile, managed Worker plugin 전체 roundtrip, 전후 성능 |
| Windows Vulkan/ANGLE | 최종 C9 publish/product/negative PASS, opaque/Acrylic API content·자동 input/semantics | 물리 scan-out/Narrator/IME·기존 resize acceptance |
| Windows MAUI/D3D12 | MAUI publish와 frame/semantics API smoke, D3D12 missing diagnostic의 의도한 거부 | MAUI 실제 text 조작/정상 종료, D3D12 정상 runtime |
| Android x64 | AOT off+trim on signed Release, emulator cold/content/text client/warm PID 유지 확인 | 실기기·정상 한글 조합·TalkBack |
| Android arm64 | 119 assemblies AOT Release publish, 현재 DEX/profile strict PASS | 기기 연결 없음. install/cold/warm/input/ART 상태 미확인 |
| iOS/Catalyst/AppKit | Apple host 부재로 notVerified | 각각 build/sign/launch/Metal/input/lifecycle/VoiceOver |
| Linux | Qt contract/publish PASS. WSLg+명시적 Mesa D3D12의 Wayland/xcb API 4/3 frames, failures 0, semantics 46 | 기본 llvmpipe는 exit 68 FAIL. native Wayland/X11 content/input/IME/Orca 미검증 |
| CLI/template | Testbed와 source-wired 새 template의 build/reuse/negative PASS | NuGet release 설치·완전 hermetic 입력·정량 launch 개선 아님 |

Android profile은 현재 APK에 rebind한 **3,630 rules**를 strict decode로 확인했다.
30-byte empty emulator profile은 채택하지 않았으며 empty decoded rules를 거부하도록 수정했다.
ProfileInstaller 강제 install result 1과 강제 `speed-profile` compile Success는 자연 설치 ART 상태가 아니다.
`am start -W` TotalTime 5,153ms는 OS 측정이며 Doroti TTID/개선률이 아니다.
emulator 캡처의 자모·숫자를 `Boot123` literal 또는 물리 한글 IME PASS로 해석하지 않는다.

Linux 성공 조건은 `GALLIUM_DRIVER=d3d12 MESA_LOADER_DRIVER_OVERRIDE=d3d12`, AMD Radeon 780M WSLg다.
xcb는 XWayland이며 native X11 acceptance가 아니다. CLI positive reuse의 exit -1은 HTTP-ready와
`build=reused` 확인 후 검사 프로세스를 종료한 결과이며 앱 정상 종료 검증이 아니다.

## 4. 보존한 실패와 무효 증거

상세 run은 기존 결과 문서 및 원시 증거에 유지한다.

- `playwright-boot-first` 4 PASS/2 FAIL, `playwright-web-final` 14 PASS/1 FAIL:
  clear capture 시점 및 ARIA mirror/focus owner 기대 오류. 수정된 content/autofocus PASS는 별도 실행이다.
  grep에서 제외된 headed text suite를 실행한 것으로 계산하지 않는다.
- `playwright-typecheck-final` Buffer generic 오류는 수정 후 retry/complete PASS와 구분한다.
- 초기 C9 2회 FAIL: Vulkan의 ANGLE 요구 assertion, 오래된 presenter 이름. 최종 정책 수정 후 C9 PASS.
- Android 이전 DEX checksum mismatch FAIL, 잘못된 activity의 adb exit 0/Error type 3 실패,
  empty profile 거부를 보존한다.
- Linux 외부 reference image 누락·WSL `/tmp` 소실 publish FAIL, 기본 llvmpipe runtime FAIL을 유지한다.
  persistent source/명시적 D3D12 성공으로 이전 실패를 덮지 않는다.
- template unsafe 누락 및 fixture package props/target 연결 FAIL. source-wired 성공은 새 NuGet package 설치 증거가 아니다.
- `cli-missing-output` 원시 PASS는 stale source에서 먼저 거부되어 해당 항목의 증거로 **무효**다.
  원인별 matcher와 fresh state의 `cli-missing-output-specific`/최종 재검증만 채택한다.
- D3D12 missing diagnostics의 raw exit 1/report FAIL과 정확한 거부 메시지의 negative PASS를 각각 남긴다.
- 과거 full/partial AOT startup/compiler FAIL, Web resize/latency FAIL, Windows Vulkan resize/physical gate,
  Windows 전체 solution의 Apple `sips` FAIL은 이 부트 작업으로 닫지 않았다.

## 5. 남은 작업

1. Apple host에서 iOS/Catalyst/AppKit 각각 signed Release와 Metal first content, suspend/resume, IME/VoiceOver.
2. arm64 실기기에서 현재 APK의 install/cold/warm/첫 입력/ART 상태 및 실제 CUJ profile 재수집.
3. Windows MAUI 실제 입력·D3D12 정상 live, Web/mobile/Linux의 물리 IME·접근성·scan-out.
4. 같은 장치/Release/renderer/cache 조건의 전후 launch raw 비교. 현재 **performance notVerified**.
5. managed Worker plugin 전체 roundtrip, verified CanvasKit bytes/module 재사용, typed manifest 직접 생성은
   각 ownership/integrity/호환성 검증 후 별도 설계한다.
6. 필요 시 실제 native Linux Wayland/X11 환경의 content capture/input과 NuGet release 소비 앱 검증을 수행한다.

원래 범위 밖이었던 full/partial AOT 재시도, Worker topology 재설계, raw Win32 전환,
Framework 전체 dynamic 제거·Material assembly 재분할을 이번 문서 정리로 시작하지 않는다.
integrity/ABI/generation/ownership·첫 font/input/semantics 품질을 약화하지 않는다는 계약을 유지한다.

기록된 build/test 외부 harness 제한은 1,200초다. 이번 정리에서는 새 build/성능/기기 검사를 하지 않았고,
원문 hash 일치·요약 링크·루트 파일 삭제만 확인한다. 기존 구현·로그·capture는 그대로 보존한다.
