# 플랫폼 기능 공백 보완 W0~W11 작업 요약

- 계획 작성일: **2026-10-03** · 후속 실행 기록: **2026-10-03~04** · 보관일: **2026-10-04**.
- 원본: 루트 `work.md`. 계획 검토 기준 HEAD는 `bd75ced5fc5f8e76cea76b9102651658445115d1`이며, 이전 외부 감사 `Doroti-platform-audit.md`와 기준 커밋 `de9ba2b7b1878899244b43938b0ce6fd0690f3dc`를 참고했다. 외부 감사 문서는 저장소에 없다.
- 결과: **W0~W11 코드·샘플·CLI·템플릿 반영 완료, 전체 제품 인수 PARTIAL**. 체크 완료는 구현과 명시한 자동 검사 완료를 뜻하며 물리 제품 인수와 구분한다.
- 아래 PASS는 원문과 연결된 실행 기록의 후보·환경·범위를 보존한 것이다. 이번 보관에서 제품 빌드·runtime·물리 입력 검사를 다시 실행하지 않았다.

## 목표와 기본 계약

공통 API의 host 등록 누락·no-op·축소된 상태 전달을 보완하고, OS·브라우저·renderer의 제한은 세부 capability와 명확한 결과로 표현했다. Windows 기본 backend는 **Windows App SDK**, Windows MAUI는 선택 backend로 유지한다. 모든 플랫폼에 같은 구현이나 시각 결과를 보장하는 것을 완료 조건으로 삼지 않는다.

실행 순서는 W0~W5의 일반 기능·입력·접근성, W6~W8의 text geometry·혼합 합성·GPU 입력/복수 창, W9~W11의 세션 복구·OS 정책·개발/배포 도구다. 기존 [C 단일 프레임 정책](../../Doroti/docs/migrations/native-frame-c-only.md)과 [Qt 동적 texture 예산](../../Doroti/docs/dynamic-texture-budget.md)은 유지하며 native·resize·replay 직렬 경계와 마지막 consumer 수명을 보존한다.

## 단계별 구현과 검증 이력

전체 명령·후보 identity·실패 수정·조건부 지원은 [W0~W11 전체 실행 기록](../../Doroti/docs/validation/2026-10-03-platform-gap-implementation.md)을 따른다. 계획 작성 당시의 미구현 진단보다 이후 실행 결과를 우선하며, 앞선 후보의 결과를 새 후보에 소급하지 않는다.

| 단계 | 구현과 기록된 결과 | 지원·인수 경계 |
| --- | --- | --- |
| W0 세부 capability | 기존 owner provider에 backend/generation, WebView operation, 입력/semantics, texture format/extent/adapter, effect·혼합 scene query를 연결. 최종 transform/DPR/clip·중간 이미지·overflow를 반영한 allocation preflight와 SolidTint fallback, CPU 계약 **PASS** | 단독 view 생성과 혼합 scene 지원을 구분. attach/loss 뒤 현재 generation 조회. Web capture·native live effect·Qt R/P 예산은 별개이며 unsupported 요청은 native 할당 전에 거절 |
| W1 URL·파일 필터 | null/empty/`*` 전체 선택, extension 중복·대소문자·선행 점·suffix 처리 공통화. picker grant 회수, scheme 지원 조회·Web URL status 전달, CPU 계약 **PASS** | Web `opened`는 브라우저 요청 수락이며 외부 메일 앱 실행 완료가 아님. worker 왕복의 user activation 제한과 실제 picker 인수를 구분 |
| W2 입력 | MAUI·App SDK·Qt configuration-only 갱신, endpoint 상태 이전·callback 억제, Windows TextBox composition range. Windows ABI3·Qt ABI7 및 비Apple build/input **PASS**. Apple 선택·Return·callback 보강 후 native 자동 검사 **PASS** | UTF-16 offset 사용. MAUI selection 방향·endpoint 교체 composition 보존은 false 계약 유지. 물리 한글 IME·OS 키보드/emoji/RTL 인수는 별도 |
| W3 Apple WebView | AppKit/UIKit의 같은 WKWebView에 공통 controller/session을 연결. versioned options, navigation/JS/state/features, profile·ClearData·app content·message·pending/generation 수명 구현. Apple 후속 factory 명령 검사 **PASS** | 초기 전체 후보 Apple 검사는 **SKIPPED**. 이후 AppKit/Catalyst/iOS simulator 검사는 별도 후보이며 실제 iPhone·inline media·focus/clip/lease 인수를 대체하지 않음 |
| W4 Windows MAUI 기본 연결 | HWND-owned picker, owner-local native editor/button/WebView overlay, coordinator/channel/graphics·등록/패키징 연결. 전용 실행과 기본 경로 **PASS** | [W4 기본 연결 기록](../../Doroti/docs/validation/2026-10-03-windows-maui-basic-connections.md)의 제품 인수는 **PARTIAL**. full-window/embedded owner 구분, 미지원 interleaving/effect/transform·native overlap/foreground ordering 사전 거절 유지 |
| W5 접근성 | native hierarchy·역할/상태·탐색 순서·advertised action subset, disabled/readOnly/password·삭제/owner 종료 guard. Windows UIA·Qt QAccessible, Apple 후속 provider·암호 보호 자동 검사 **PASS** | framework semantics와 embedded native editor의 접근성 소유권 구분. 실제 Narrator/VoiceOver/Orca/TalkBack 인수 미완료; provider subset을 전체 AT 지원으로 확대하지 않음 |
| W6 text geometry | 실제 layout의 UTF-16 range/line/selection/bounds/hit test·revision/generation/DPR snapshot, Windows TextProvider/TextRangeProvider·Qt 문자 rect/point/scroll 구현. CPU 및 native provider 검사 **PASS** | node 전체 rect를 glyph geometry로 대체하지 않음. MAUI/UIKit glyph range는 false. 실제 AT 텍스트 탐색 미검증 |
| W7 Windows 혼합 합성 | App SDK opt-in WinUI WebView+editor 동시 HTML/JS, owner frame의 reservation/generation/visibility/input shield/raster/GPU lease와 frame retirement **PASS** | 전체 ordering/effect/물리 입력 인수 **PARTIAL**. MAUI와 App SDK는 별도 topology/검사이며 성공한 조합만 활성화 |
| W8-A MAUI GPU 입력 | 실제 adapter LUID·format/extent/import 조건과 producer/consumer 수명을 연결. 실제 shared GPU texture 12 frame import/release·wrong-adapter 거절 **PASS** | CPU upload와 native handle 비용/지원 구분. visible pixels·camera 실기기·성능과 단순 submission/receipt를 GPU 완료·scanout으로 확대하지 않음 |
| W8-B MAUI 복수 창 | 실제 독립 Window/surface/input/semantics/registry/PlatformView owner 생성, application boundary·close/lifetime·복원 ID 처리. 두 창·두 lifetime·survivor resize/최종 drain **PASS** | 실제 mixed-monitor DPR·두 창 picker/WebView 전체 사용자 경로는 미검증. W8-A와 독립 인수 |
| W9 Web 복구·offline | public loss 상태/event/restart, fresh canvas/session·generation·pending 자원 정리. main WebGL/WebGPU page restart, single-thread worker 재생성, offline 한글 glyph/외부 요청 0·decoder 진단·clipboard 비의도 read 0 **PASS** | threaded profile의 main ownership 유지, unsupported standalone worker는 allocation 전 거절. 직렬화 가능한 navigation/restoration만 복원. Safari/iPhone·한글 화면 pixels·일반 CSP 배포·실제 표시 성능 별도 인수 |
| W10 창 정책·펜 | native close cancellation 생성 전 정책, atomic/checksummed draft recovery, pressure/tilt/orientation 단위·가용 상태 정규화. 수치/draft·Qt Wayland/XWayland 계약 **PASS** | Catalyst native close 취소 false 유지. pure X11/xcb 위치는 outer-frame ACK 조건부이며 실제 위치 미검증. 앞선 XWayland placement/minimize ACK 실패는 성공에 합산하지 않음. 물리 pen·dirty document native close·monitor 이동 미검증 |
| W11 개발·release | 공통 runner/backend 목록, MAUI Debug metadata Run/Reload/Restart/Stop **PASS**. 상위 release의 platform/TFM/RID/host/toolchain 전달·NuGet/template consumer 연결. Android 후속 SDK/ADB Hot Reload·설치 VSIX 루프 **PASS** | Debug metadata 지원 profile과 일반 run/build를 구분하고 Release/NativeAOT metadata reload는 허용하지 않음. package 생성·서명/공증·clean OS·물리 입력 인수는 각각 별도 |

## Apple 후속과 Android 핫리로드

**Apple:** 처음 전체 구현 요청에서는 Apple 코드 변경을 포함하되 빌드·실행 검사를 사용자 요청으로 SKIPPED했다. 별도 후속 요청에서 AppKit / Mac Catalyst / iOS의 W2 선택 복원·native Return trait/action·callback 수명, W5 provider·암호 보호·UIKit 컴파일 오류를 보강하고 W3 실제 WebKit 명령 검사를 추가했다.

[Apple 후속 기록](../../Doroti/docs/validation/2026-10-03-apple-platform-gap-followup.md)과 [결과/identity JSON](../../Doroti/docs/validation/2026-10-03-apple-platform-gap-followup.json)에 macOS 26.6.2 arm64 / Apple M1, .NET SDK 10.0.401, Xcode 27.0 후보의 AppKit·Catalyst·iOS simulator Debug build/native smoke **PASS**를 보존한다. 최종 앱 빌드는 경고 0·오류 0이다. 실제 iPhone Release/NativeAOT, 물리 IME·VoiceOver·pen, 서명/공증·clean OS 인수는 별도다. 이전 [Apple frame 구성 검토](works/results/2026-10-03-apple-frame-configuration-review.md)와 [AppKit 구성 검토](works/results/2026-10-03-appkit-configuration-review.md)도 해당 후보의 이력으로 유지한다.

**Android:** 10월 3~4일 후속에서 restart-only 루프를 Debug Mono SDK metadata Hot Reload와 ADB private-file session으로 교체했다. [Android 핫리로드 기록](../../Doroti/docs/validation/2026-10-03-android-hot-reload.md)의 Galaxy S25 최종 `s25-05`와 설치 VSIX `vsix-01`은 실제 메서드 delta·같은 PID/State/count/한글 text/scroll 보존·컴파일 오류 복구·미지원 변경 시 기존 상태 유지·Stop 정리 **PASS**다. VS Code Run/Hot Reload/Restart/Stop도 확인했다.

이 실기기 검사는 Testbed reload scene에 count=5, text=`한글 유지`, scroll=160을 자동 주입한 결과다. 물리 터치·한글 IME·TalkBack·다른 기기/x64 emulator runtime·성능/장기 사용은 **notVerified**다. Release/optimized/trimmed/AOT/CoreCLR 개발 profile은 빌드 전 명시적으로 거절한다. 앞선 전체 후보의 기기 없음/restart-only 기록은 후속 PASS로 덮어쓰지 않는다.

## 패키지·실행 근거와 문서 연결

Windows MAUI·App SDK의 격리 feed/cache NuGet-only Release template consumer는 실제 첫 frame·두 창·survivor resize/종료를 확인했고, Web Offline consumer는 첫 frame/focus·외부 요청 0과 font/decoder 배포를 확인했다. 최종 MAUI preflight 후보는 `0.3.0-beta.audit.20261003.6`, App SDK는 `.4`, Web Offline은 `.5`다. 전체 snapshot hash·package/payload 목록과 개별 identity는 [보존 identity JSON](../../Doroti/docs/validation/2026-10-03-platform-gap-identities.json)을 따른다.

현재 제품 계약은 [지원 상태](../../Doroti/docs/support-status.md), [PlatformView 지원표](../../Doroti/docs/platform-views/support-matrix.md), [개발/Hot Reload 안내](../../Doroti/docs/development-hot-reload.md), [release candidate 안내](../../Doroti/docs/release-candidates.md)를 따른다. 기존 작업 자료는 [works 보관 인덱스](works/README.md)에 유지한다.

원문의 검사 진입점은 다음과 같다. [.github 지침](../../.github/copilot-instructions.md)에 따라 테스트 timeout은 20분(1,200초), 일반 반복은 30회 이내다. Linux Desktop probe는 `-p:DorotiLinuxDesktop=true`를 사용하며 Qt template/sample native 사본·ABI를 함께 확인한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/validate.py Source
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests/Doroti.Tests.csproj -c Debug --artifacts-path temp/testing/platform-audit/contracts/build
python Doroti/eng/run-with-timeout.py --timeout 1200 node --experimental-transform-types --test Doroti/tests/web_rendering.mts
```

보관 후 `Source` 문서 목록은 삭제된 루트 `work.md` 대신 이 요약을 검사한다. capability query·mock·synthetic input·GPU receipt 통과와 실제 native 연결/물리 사용자 경험은 구분한다.

## 남은 인수와 제외 범위

원문의 미완료 체크는 다음 범위로 남긴다. 후속 Apple 자동 검사를 반영해도 전체 제품 인수는 **PARTIAL**이다.

- W3: 기존 inline media·focus yield·native clip·composition lease 유지와 iOS/Catalyst별 전체 실행 인수.
- W7: editor→WebView / WebView→editor, raster→editor→raster→WebView→raster, popup/shield·blur 조합의 ordering/effect/물리 입력 인수.
- 기본 P1 기능의 모든 host reachable path·직접 실행 인수, Window/GPU/native view/AT/서비스 자원의 두 owner·resize·close·loss 전체 수명 인수.
- 물리 IME·screen reader·pen, 실제 monitor/DPR 이동·표시 품질/FPS·입력 지연·메모리/성능·장기 사용, 실제 iPhone·서명/공증·clean OS 배포 인수. 미확인은 **notVerified**, 미측정은 **notMeasured**로 유지한다.

A/B 프레임 정책 복원, Qt 동적 예산 재구현, 모든 backend의 새 WGSL 구현, native view 임의 affine transform/효과 보장, UIKit material의 Gaussian 픽셀 동등성, browser 보안·OS 창 정책 우회, 무관한 광범위 성능 실험은 범위 밖이다. Catalyst native scene close 취소·전역 위치·topmost/Dock 제어 제한, AppKit application-scoped Dock 정책, Qt QPA별 조건과 **unsupported / conditional** 계약을 구현 TODO나 전체 PASS로 바꾸지 않는다.

## 증거 보존 경계

PASS는 명시된 후보·환경·범위의 결과, PARTIAL은 구현/일부 검사 후 필수 인수가 남은 상태, SKIPPED는 해당 후보의 명시적 생략이다. 실패한 후보·재시도·source/payload identity는 연결된 실행 문서/JSON에 보존하며 다른 날짜·renderer·package의 결과로 합산하지 않는다.

`temp/testing/platform-audit/`, `temp/apple-*.log`, `Doroti/artifacts/`의 raw log·consumer/cache·이미지는 삭제 가능한 로컬 산출물이며 현재 존재를 보장하지 않는다. 원시 파일이 정리된 후보의 집계와 identity는 추적 문서/JSON을 따른다. 이번 작업은 요약·참조 경로 정리·원본 삭제이며 기존의 물리/배포 미완료 상태를 통과 처리하지 않는다.
