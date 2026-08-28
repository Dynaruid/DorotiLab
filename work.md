# Doroti cross-platform 최초 부트 작업 결과

- 실행일: 2026-08-29
- 대상: `plan.md` S2~S8 MVP
- 판정: 구현 가능한 MVP는 완료했다. OS/장치/배포 환경이 없는 gate와 구조상 A/B 또는 operator 결정이 필요한 항목은 `notVerified`/후속으로 남긴다.

## 구현 결과

| 영역 | 결과 | 판정 |
| --- | --- | --- |
| 공용 bootstrap | `DorotiApplicationManifest`를 source-generated `JsonSerializerContext`로 읽어 launch-time reflection metadata 생성을 제거했다. RID/resource hash/plugin ABI fail-closed는 유지했다. | `PASS`, 성능 크기는 `expectedImprovement` |
| Theme | `MaterialApp`와 router API에 additive lazy theme factory를 추가했다. active palette만 생성하고 factory는 1회 memoize하며 eager+lazy 중복은 fail-closed한다. Demo의 light/dark 동시 생성을 제거했다. | FCR-7 `PASS` |
| MAUI service | Android/iOS/Mac Catalyst/AppKit/Windows의 hidden Entry/Editor native handler를 첫 text client 활성화까지 attach하지 않는다. 접근성 활성 여부의 신뢰 가능한 OS 계약이 없어 semantics layer는 지연하지 않았다. | Android 실제 text activation `PASS`; Apple/accessibility-active `notVerified` |
| MAUI package | 사용하지 않는 `CommunityToolkit.Maui.Markup` package와 registration을 제거했다. | Windows MAUI/Android build `PASS` |
| Windows default | 일반 launch는 세 native 파일의 존재/길이/mtime/x64 PE/ABI만 검사한다. SHA-256은 build/publish manifest에 기록하고 `DOROTI_WINDOWS_NATIVE_AUDIT=1`에서만 다시 계산한다. | C5-A/C9 `PASS` |
| Windows diagnostics | Vortice/D3D12 presenter를 `Doroti.Host.WindowsAppSdk.Diagnostics`로 분리하고 기본 ANGLE artifact에서 dependency를 제거했다. explicit D3D12 요청에 assembly가 없으면 fail-closed한다. | 기본 publish Vortice/D3D12 0, diagnostic project build `PASS` |
| Windows publish | Release publish 뒤 PDB를 제외하고 provenance JSON을 생성한다. | PDB 0, manifest hash 64자 3개, `PASS` |
| Android profile | `R3CY30KZA4B`에서 reset→cold gallery→TextField→`Startup123` CUJ를 snapshot하고 HRF 3,630줄 및 binary profile/metadata를 생성했다. Runner SDK가 `assets/dexopt`에 패키징한다. | APK entry 2개, `profgen` arm64/x64 strict `PASS`; device `speed-profile/install-dm` `PASS` |
| Android x64 | full AOT off를 유지하고 trimming을 다시 켰다. marshal methods는 알려진 startup fault 때문에 계속 off다. | Release build `PASS`, APK 22,990,835 bytes, assembly store 11,214,208 bytes; emulator runtime `notVerified` |
| Web | product `PublishTrimmed=true`, 금지 gate 제거, Release PDB 제외를 적용했다. | clean publish/desktop Chrome live `PASS` |
| Linux | S2 공용 변경을 공유하며 managed runner build와 Qt ABI를 확인했다. | build/ABI `PASS`; 실제 Wayland/X11 `notVerified` |
| CLI | `-NoBuild`, `-NoRestore`, `-LastSuccessful`과 source/native/config/RID fingerprint state를 추가했다. 실행 artifact와 생략/실행 단계를 출력한다. | normal→same fingerprint reuse `PASS`; missing state fail-closed `PASS` |

## 실행 증거

- FCR-3 scheduler, FCR-4 retained rendering, FCR-6 semantics, FCR-7 Material/theme: Release `PASS`.
- FCR-0: 현재 checkout에는 `doroti.ps1`이 참조하는 aggregate `eng/validate.ps1`/FCR-0 script가 없어 `notVerified`. 다른 FCR PASS로 대체하지 않는다.
- Windows C5-A: ANGLE/D3D11 hardware, visible exact present, presented 18, failed 0, resize terminal invariant `PASS`.
- Windows C9: empty `PATH` normal launch와 full-hash audit launch exit 0; missing native/ANGLE, wrong architecture/version는 모두 explicit exit 1 `PASS`.
- Windows CLI: normal과 `-LastSuccessful`이 같은 fingerprint `75a9a8...58defe`를 실행했고 둘 다 presented 3/failed 0이었다.
- Android arm64: signed APK 27,400,780 bytes, install `PASS`; post-install/cold 단일 smoke 301/291 ms, warm task resume 8 ms. 이 값은 `am start -W` 단일 진단값이며 TTID 성능 수치로 승격하지 않는다. PID/foreground/screenshot, ASCII text commit와 한글 keyboard 표시, crash/ANR 0 `PASS`.
- Android profile: `baseline.prof` 5,751 bytes, `baseline.profm` 702 bytes, x64에서도 3,630 rule strict decode `PASS`.
- Web fresh trimmed payload: 72 logical files, uncompressed 27,049,342 bytes, Brotli 7,447,738 bytes, gzip 9,791,516 bytes, PDB 0. 계획의 이전 snapshot 46,255,091/12,932,420/16,976,949 bytes보다 작지만 인과 TTID 수치는 `notVerified`.
- Desktop Chrome: canvas/WebGL first content, `Doroti 한글 123`, button/checkbox/switch, scroll lazy rows, ARIA tree, console warning/error 0 `PASS`. fresh profile, clipboard, mobile과 실제 배포 HTTP compression/cache header는 `notVerified`.
- Linux managed Release build와 Qt ABI contract `PASS`.
- Windows MAUI Release build `PASS`. 독립 live cold/warm는 `notVerified`.
- 전체 `Doroti.Product.slnx` Release: `FAIL` 1 error. Windows에서 AppKit icon 생성 도구 `sips`가 없어 `DorotiDemoApp.MacOS` packaging이 실패했다. 그 전 iOS simulator/Mac Catalyst managed compile은 완료됐지만 Apple runtime PASS가 아니다.

## 남은 경계

- `notVerified`: profiler-free TTID/TTFD 반복 측정, Windows 실제 한글 IME/Narrator/mixed-DPI, Android 한글 조합 commit/TalkBack/x64 emulator, iOS physical/Mac Catalyst/AppKit runtime·signing, Linux Wayland/X11/IME/Orca, Web fresh profile/mobile/production headers.
- 후속: Windows App SDK identity 대 raw Win32 ADR, ReadyToRun/AOT tuning, Material↔Cupertino assembly 재분할, Web lazy assembly/locale policy, marshal methods 재활성화 matrix.
- 안전상 미적용: accessibility 상태를 확실히 알 수 없어 semantics first-present 지연을 하지 않았다. Cupertino는 Material adaptive API의 실제 compile dependency라 임의 삭제하지 않았다.
- 전체 solution의 macOS `sips` 실패는 이 Windows 작업에서 우회해 PASS로 바꾸지 않았다.

## 작업 범위 보존

작업 시작 시 `git status --short`는 clean이었다. 변경은 `plan.md`의 bootstrap/theme/MAUI/Windows/Android/Web/Linux/CLI와 결과 문서에 한정했으며 기존 `history/26-08-29/windows-appsdk-acrylic-resize-investigation.md`는 수정하지 않았다.
