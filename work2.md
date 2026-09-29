# works 미완료 작업 검토

2026-09-29 후속 구현·검증: [Web·Windows·Android 실행 결과](works/results/2026-09-29-web-windows-android.md). 전체 **PARTIAL**, 범위별 PASS와 미완료를 분리한다. CI workflow는 사용자 지시로 삭제 상태를 유지한다.

검토일: **2026-09-29** · 검토 기준 HEAD: `4b46e2c927339dfad4c728087c9545cda5dc5e2f`

검토 범위는 [works 인덱스](works/README.md), 공통 문서 00~10, 플랫폼 문서 7개와 [지원표](Doroti/docs/support-status.md)다. 체크박스뿐 아니라 2026-09-28~29 실행 결과와 **00~10 보강 검토**를 함께 읽어 남은 범위를 정리했다. 이번 작업은 문서 검토이며 제품 실행·물리 입력·성능·배포를 새로 검증한 결과가 아니다.

후속 문서 수정(2026-09-29): 사용자 요청에 따라 아래 §5의 불일치를 원문과 지원표에 반영했다. 문서 정합 작업은 완료로 표시하고 제품 구현·실행 검증 잔여는 그대로 유지한다.

**M0의 로컬 기반은 완료됐고, M1~M7은 부분 완료 상태다.** 남은 일은 실제 입력·화면·GPU·출시 환경 검증, 다른 플랫폼 adapter 구현, 플러그인 event stream·OS 드롭 송신·추가 창 확장 등이다. Windows Desktop, 테스트 runtime, Windows/Web Hot Reload, Windows 플러그인·드롭 수신·추가 창, Windows/Web/Android Navigation의 기존 구현을 처음부터 다시 만드는 작업으로 해석하지 않는다.

상세 완료 기준과 실행 결과는 원래 `works` 문서에서 관리한다. 이 문서는 후속 작업을 고르는 목록이며, 공통 작업과 플랫폼 작업을 중복 집계하지 않는다.

## 1. 판정 기준과 전체 현황

- **추가 구현:** 공개 계약·adapter·플랫폼 연결 등 기능 자체가 남아 있는 범위.
- **검증 잔여:** 구현 또는 일부 PASS 기록은 있으나 해당 환경·입력·화면·수명 검증이 남은 범위. 실패가 확인됐다는 뜻은 아니다.
- **문서·정리:** 체크박스·지원표의 최신화, 과거 정리 보류 경로 확인 등 관리 작업.
- **후속/미지원:** 현 범위에서 제외되거나 명시적으로 unsupported인 기능. 자동으로 필수 구현 범위에 넣지 않는다.

| 공통 문서 | 최신 기록에서 확보된 범위 | 남은 핵심 작업 |
| --- | --- | --- |
| [00 기반](works/common/00-foundation.md) | CLI 실패 전파, timeout·임시 경로, 로컬 suite, CI 구성 | 원격 CI·clean checkout/OS 확인, 정리 보류 자료 확인 |
| [01 테스트](works/common/01-testing.md) | WidgetTester, 가상 clock, pointer·합성 한글, CPU 리스트/blur/Dialog, 별도 owner-thread 격리, 패키지 소비 | GPU golden·전체 native 자원 격리·호스트별 입력/semantics·진단 산출물 기준 |
| [02 Desktop](works/common/02-desktop-contract.md) | Windows App SDK adapter·native 창 상태·close, 추가 창·Explicit 종료 | 실제 live resize·mixed-monitor·다른 adapter 정합 및 재검증 |
| [03 입력·접근성](works/common/03-input-accessibility-platformview.md) | 입력 장면, 합성 조합·해제, Windows editor/WebView 각각 재생성 | 물리 IME·Tab/focus·보조기술·PlatformView 전체 조합·MAUI/Catalyst 연결 |
| [04 렌더링](works/common/04-rendering-lifetime.md) | CPU geometry/list/layer, Web admission 정책, stale native scene 회귀 | GPU 품질·실제 present/VRAM 예산·실기기 memory/loss·폰트 전체 정책 |
| [05 VS Code](works/common/05-vscode-hot-reload.md) | 로컬 VSIX, snippet/import, Windows/Web metadata Hot Reload와 상태 보존 | 마법사 예외 UI·실제 C# 서비스 조합·Restart/종료 경합·최신 변경 후 통합 재검증 |
| [06 플러그인](works/common/06-plugin-sdk.md) | 공통 SDK, Windows FilePicker/URL, NuGet 소비·취소·수명 | event stream, Web/Android/다른 native adapter, 실제 권한·목표 publish 모드 |
| [07 OS 드롭](works/common/07-os-drag-drop.md) | 공통 계약, Windows Copy 수신, 큰 파일·OLE fixture | 외부 앱 교차 창 전달, 송신·drag 이미지·move/link·virtual file·다른 adapter |
| [08 Navigation](works/common/08-navigation-restoration.md) | 공통·Windows·Web main-owned·Android 연결, 일부 재시작/복원 | 실제 browser history, iOS/다른 desktop, 앱별 migration·입력/selection 복원 |
| [09 멀티윈도우](works/common/09-multiwindow.md) | 창 문맥 ADR, Windows 실제 두 창·native editor island·수명 | 두 창의 물리 IME/DPI/전체 native content, AppKit/Qt/Catalyst, Satellite 경계 |
| [10 출시](works/common/10-release-packaging.md) | Windows/Web package-only 후보·publish, 로컬 portable 설치/업데이트 | 최신 수정 포함 후보, clean OS·서명·실제 배포, Android package-only·장기 회귀 |

## 2. 공통 문서별 남은 작업

### 00. 기반·CI·기록 정리 — P0

근거: [00-foundation](works/common/00-foundation.md), [최신 보강 검토](works/README.md#2026-09-29-0010-보강-검토).

- [ ] **검증 잔여:** 원격 CI에서 Source/Developer/Packages/Targets의 실제 실행 결과를 확인한다. Windows GPU smoke는 interactive self-hosted runner에서 별도로 검증한다. workflow 작성과 로컬 PASS만으로 원격 CI 완료 처리하지 않는다.
- [ ] **검증 잔여:** 기존 `temp/testing`·로컬 artifacts에 의존하지 않는 새 checkout의 최소 검증 경로를 확인한다. clean OS 설치·배포 검증은 10과 묶는다.
- [ ] **문서·정리:** 00과 works 인덱스에 기록된 실패 조사·이전 release 후보·수동 자료의 현재 존재 여부와 보존 필요를 확인한다. 필요한 결과를 요약한 뒤 해당 실행 소유 경로만 정리하고 링크·보류 기록을 갱신한다. 과거 삭제 차단 기록을 현재에도 동일하게 적용되는 제한으로 단정하지 않는다.

CLI 복구, timeout 실행기 신설, `/temp/` 제외, CI workflow 신설 자체는 재구현 대상이 아니다.

### 01. 테스트 runtime의 남은 범위 — P1

근거: [01-testing](works/common/01-testing.md), [04 렌더링 결과](works/common/04-rendering-lifetime.md).

- [ ] **검증 잔여:** GPU/texture/native 자원과 복수 owner의 생성·재생성·종료 격리를 확인한다. CPU binding·focus·layer 해제 PASS를 모든 native 자원 해제로 확대하지 않는다.
- [ ] **검증 잔여:** CPU 이미지 비교, GPU offscreen golden, 실제 화면 캡처의 기준을 분리하고 폰트·DPR·renderer·색 공간·허용 오차를 고정한다. GPU blur 품질과 PlatformView 포함 장면을 보강한다.
- [ ] **검증 잔여:** 실패 시 비교 이미지·트리·frame 정보·재현 명령이 남는지, 의도적인 입력/geometry 결함이 실패하는지 누락된 장면에서 확인한다. 기존 패키지 소비 경로를 활용한다.
- [x] **문서 정합:** 01의 ListView/Dialog/blur·독립 owner-thread 결과와 과거 직렬 실행 범위를 정리했다. timer/진단·임시 경로 제외·성공 실행 정리와 GPU/native/정리 보류 잔여를 분리했다.

같은 thread의 nested tester는 거절하는 계약을 유지한다. 추가 검증 없이 임의의 병렬 사용을 보장하지 않는다.

### 02. Desktop 화면 품질과 다른 adapter — P1

근거: [02-desktop-contract](works/common/02-desktop-contract.md), [Windows](works/platforms/windows.md).

- [ ] **검증 잔여:** Windows의 첫 표시·연속 resize·최소화/복원·디스플레이 이동을 실제 화면으로 확인한다. geometry·깜빡임·mixed-monitor DPI와 종료 중 pending GPU 작업을 함께 본다.
- [ ] **추가 구현/검증:** AppKit·Qt·Catalyst·Windows MAUI의 기존 adapter를 공통 크기/DPI 제약·focus·appearance·close 취소·render drain 계약과 대조하고 필요한 연결을 보완한다. 미제공 기능은 capability와 거절 결과를 맞춘다.
- [x] **문서 정합:** 02의 단일 창/Explicit 미지원 문구를 당시 기록으로 명시하고 현재 추가 창·Explicit 지원을 09 결과와 연결했다.

### 03. 물리 입력·접근성·PlatformView — P1

근거: [03-input-accessibility-platformview](works/common/03-input-accessibility-platformview.md).

- [ ] **검증 잔여:** 실제 한글 IME의 시작/갱신/확정/취소, caret·selection·후보창 좌표, multiline·clipboard·단축키·focus loss를 목표 호스트에서 검증한다.
- [ ] **검증 잔여:** Doroti TextField ↔ native editor/WebView의 Tab/Shift+Tab, 조합 중 focus 이동, IME 소유권, 키 중복 전달·눌림 해제를 확인한다.
- [ ] **검증 잔여:** UIA·TalkBack·VoiceOver·Orca 및 브라우저 보조기술에서 이름·역할·값·상태·action·focus·탐색 순서를 실제 사용으로 확인한다. semantics 트리 조회 PASS와 분리한다.
- [ ] **검증 잔여:** PlatformView 복수 owner, clip/transform/z-order, native-origin gesture 경쟁, 늦은 응답, 재생성·앱 복귀 후 focus를 지원 조합별로 확인한다.
- [ ] **추가 구현:** Windows MAUI WebView/native hierarchy와 Catalyst의 별도 adapter·합성 연결 범위를 조사하고 누락을 구현한다.

Windows HWND editor와 WebView의 같은 프레임 혼합은 현재 미지원 경계다. 각각의 재생성 PASS를 혼합 지원으로 해석하지 않는다.

### 04. GPU 품질·성능 예산·기기 수명 — P1

근거: [04-rendering-lifetime](works/common/04-rendering-lifetime.md), [측정 기준](Doroti/docs/rendering-baselines.md).

- [ ] **검증 잔여:** 긴 리스트·VariableBlur·다중 효과·WebView overlay·이미지/실제 영상 texture·resize/DPR를 목표 기기와 renderer에서 비교한다. CPU로 확인한 장면 외의 GPU/합성 장면을 완성한다.
- [ ] **측정 잔여:** build/layout/paint, GPU 작업, readback/upload, 실제 present 간격, 메모리/VRAM을 분리 측정하고 기기·해상도별 baseline/허용 예산을 채운다. GPU 완료 시간이나 작은 CPU 장면의 수치를 표시 FPS로 사용하지 않는다.
- [ ] **검증 잔여:** blur 축소/adaptive 처리에서 capture 영역·pixel origin·viewport와 픽셀 품질을 확인한다. Web PlatformView CPU upload와 admission/backpressure의 실제 부하 결과도 측정한다.
- [ ] **검증 잔여:** Android/iOS 모바일 Web 및 native 실기기의 장기 스크롤·메모리·pause/resume·context/device/surface loss·GPU retirement를 확인한다.
- [ ] **검증 잔여:** CDN/로컬 asset/offline 폰트, cold-start 초기 교체, 한글 fallback을 실제 호스트에서 확인한다.

### 05. VS Code 통합 사용 흐름 — P1

근거: [05-vscode-hot-reload](works/common/05-vscode-hot-reload.md).

- [ ] **검증 잔여:** clean profile에 설치한 VSIX에서 생성 마법사의 실제 폴더 dialog, 잘못된 이름·기존 폴더 충돌, SDK/템플릿/도구 누락, 취소·생성 실패·확장 종료 중 취소를 확인한다.
- [ ] **검증 잔여:** 실제 Microsoft C# 언어 서비스의 버전·설정과 프로젝트 로딩을 기준으로 auto-import/Quick Fix를 확인한다. 참조 누락·서비스 미설치, 중복/global using·alias·namespace 형태·동명 타입 선택을 포함한다.
- [ ] **검증 잔여:** 12개 snippet의 새/기존 파일 삽입·Tab·연결 이름 변경·Undo·import 충돌을 실제 editor에서 확인한다. 전체 snippet build PASS와 개별 편집 동작 PASS를 구분한다.
- [ ] **검증 잔여:** rude edit 뒤 명시적 Restart, 종료 경합·target 변경·종료한 세션 응답·생성 프로세스 정리와 restricted Workspace Trust profile을 검증한다.
- [ ] **검증 잔여:** 09의 창 문맥 변경 이후 Windows/Web의 설치 VSIX → 실제 metadata update → 화면 갱신·State/입력/스크롤 보존을 재검증한다. 2026-09-28 통합 PASS와 2026-09-29 CPU/단위 회귀만의 결과를 구분한다.

Web Hot Reload의 기존 PASS 범위는 desktop Chrome, main-owned threaded Debug runtime, WebGL/WebGPU다. Release/AOT·worker-owned·다른 브라우저·물리 모바일은 이 결과에 포함하지 않는다.

### 06. 플러그인 확장과 플랫폼별 구현 — P2

근거: [06-plugin-sdk](works/common/06-plugin-sdk.md).

- [x] **추가 구현:** 공개 event stream 구독·취소·bounded backpressure·dispose 계약을 추가했다. 요청/응답 ABI를 유지하는 별도 capability이며 공통 회귀 PASS. 실제 native event source의 플랫폼별 qualification은 남는다.
- [ ] **추가 구현/검증:** Web·Android부터 FilePicker/URL launcher adapter와 파일 접근 수명·취소·권한·미지원 결과를 같은 C# 앱에 연결한다. 이후 AppKit/Qt/iOS/Catalyst를 확장한다.
- [ ] **검증 잔여:** Web module 누락·무결성 진단, 실제 OS 권한 거절·물리 picker 입력, 여러 창과 지연 응답을 플랫폼별로 확인한다.
- [ ] **검증 잔여:** 외부 NuGet-only 소비 앱에서 목표 플랫폼의 restore/build/run/publish·trimming·지원 AOT 조합을 확인한다. 공통 trimmed 소비 PASS를 Windows renderer 전체 trimming/NativeAOT 통과로 확대하지 않는다.

### 07. OS Drag & Drop 수신 확장·송신 — P2

근거: [07-os-drag-drop](works/common/07-os-drag-drop.md).

- [ ] **검증 잔여:** Explorer → 기본 runner의 실제 교차 창 파일/텍스트/URI 전달을 확인한다. 외부 source에서 여러/큰 파일·취소·드래그 중 mixed-monitor DPI 변경을 검증한다.
- [ ] **추가 구현:** AppKit/Qt/Web 수신 adapter를 연결하고 모바일 제공 범위는 capability로 정한다. Finder/파일 관리자/브라우저의 실제 전달로 판정한다.
- [ ] **추가 구현:** Doroti → OS 송신, drag 이미지, move/link 협상, 성공·취소에 따른 원본 처리 책임, virtual file stream, 다중 창 이동을 구현·검증한다.
- [ ] **검증 잔여:** native editor/WebView 자체의 drop과 Doroti view 수신을 구분하고 각 제공 범위를 명시한다.

현재 Windows 구현은 **Copy 수신**이다. 공통 action enum과 실제 OLE fixture PASS가 송신·Move/Link·Explorer 교차 창 검증을 뜻하지 않는다.

### 08. 실제 브라우저 탐색과 플랫폼 복원 — P2

근거: [08-navigation-restoration](works/common/08-navigation-restoration.md).

- [ ] **검증 잔여:** Web main-owned runtime에서 URL ↔ Router, back/forward·새로고침·bfcache 복귀·손상/외부 history state·listener 해제를 실제 브라우저로 확인한다. Node 계약 PASS와 분리한다.
- [ ] **추가 구현:** iOS Universal Link와 AppKit/Qt/Catalyst activation을 cold/warm·대기·중복 처리·lifecycle·복원에 연결한다. Web worker-owned framework navigation도 지원 확대가 필요하면 별도 연결 대상으로 잡는다.
- [ ] **검증 잔여:** 앱별 schema version migration, 사용자 입력·selection의 정상 종료/강제 종료 후 복원, 잘못된 링크·손상 상태·cold 링크 우선순위를 전체 목표 호스트에서 실증한다. 현재 구버전의 빈 상태 fallback을 데이터 migration 구현으로 간주하지 않는다.
- [ ] **추가 구현/검증:** Windows의 OS protocol 등록을 실제 설치 방식과 연결하고 OS에서 링크를 여는 cold/warm 경로를 확인한다. runner에 URI를 전달한 기존 PASS와 구분한다.
- [ ] **검증 잔여:** 최종 공통 수명 수정이 포함된 Android APK를 다시 설치해 cold/warm·재시작을 확인한다. 이전 Galaxy 화면 PASS 이후 최종 APK는 재빌드만 됐다는 기록을 유지한다. back·잘못된 링크·입력 복원도 확인한다.

### 09. 멀티윈도우 실사용과 다른 desktop — P2

근거: [09-multiwindow](works/common/09-multiwindow.md).

- [ ] **검증 잔여:** Windows 실제 두 창에서 독립 스크롤·물리 IME/focus·mixed-monitor DPI·화면 품질·WebView 등 전체 native content를 확인한다. 한 창 종료 중 다른 창 입력·표시·지연 작업이 유지되는지 함께 검증한다.
- [ ] **추가 구현:** AppKit와 Qt의 실제 추가 창, Catalyst scene 기반 생성·종료를 공통 창 문맥 계약에 연결한다. OS 창 두 개와 독립 자원·마지막 창 정책으로 판정한다.
- [ ] **추가 구현/경계 정리:** `_window_*` Satellite를 Desktop 소유권과 연결하거나 지원 경계를 문서화한다. owner/modal/satellite, popup/tooltip, 창 간 이동은 기본 추가 창 이후 순서로 진행한다.

Windows의 실제 두 HWND·native TextBox island·독립 크기·survivor resize·두 lifetime과 중복 Explicit 종료 수정은 이미 기록된 범위다. fake controller 두 개를 만드는 작업으로 되돌리지 않는다.

### 10. 출시 후보와 clean 배포 — P3

근거: [10-release-packaging](works/common/10-release-packaging.md), [후보 계약](Doroti/docs/release-candidates.md).

- [ ] **후보 갱신:** 후속 installer·종료·복원 보강이 포함된 새 후보를 만들고 package/payload hash·revision·toolchain·실행 결과를 일치시킨다. 기존 `0.3.0-beta.rc.20260929010529` 후보에 이후 수정이 포함됐다고 간주하지 않는다.
- [ ] **검증 잔여:** 저장소 소스와 개발자 cache가 없는 clean OS/VM에서 Windows/Web package-only 앱을 설치·기동·업데이트한다. Android package-only 소비를 추가하고 이후 목표 플랫폼으로 확장한다.
- [ ] **추가 구현/검증:** 선택한 실제 배포 형식의 서명·패키징·등록·설치·업데이트·제거·데이터 유지·crash/로그 수집을 완성한다. Windows protocol/필요한 shell 등록도 포함한다. 로컬 portable fixture 성공은 해당 배포 방식의 실증과 구분한다.
- [ ] **검증 잔여:** Release·trimming·Mono AOT/NativeAOT·Web publish 중 실제 제공할 조합을 정하고 검증한다. Windows 전체 renderer trimming/NativeAOT, Web AOT 등 미지원/미검증 조합을 일괄 필수 지원으로 약속하지 않는다.
- [ ] **검증 잔여:** 같은 후보로 장기 스크롤·메모리 누수·창/PlatformView 반복 생성·GPU loss·앱 복귀 회귀를 수행한다. 장기 시험은 목적·예산·종료 조건을 먼저 정한다.
- [ ] **문서·정리:** 출시 지원표·실행 가능한 샘플·migration 안내·문제 보고 양식을 최종 후보에 맞추고 남은 제한을 명시한다.

서명·clean 환경·배포 계정·실기기는 착수 시 실제 이용 가능 여부를 확인한다. 2026-09-29 문서에 기록된 장비/도구 부재를 이번 검토에서 새로 확인한 사실로 취급하지 않는다.

## 3. 플랫폼별로 묶어 실행할 범위

아래 표는 §2를 어느 호스트에서 수행할지 정하는 매핑이다. 각 플랫폼에서 runner/toolchain·renderer·build mode·공통 fixture를 확인하고, 입력·렌더링·플러그인·Navigation·배포 결과를 별도로 기록한다.

| 플랫폼 문서 | 우선 남은 일 | 결과를 분리할 경계 |
| --- | --- | --- |
| [Windows](works/platforms/windows.md) | 물리 IME/Tab/UIA, 실제 resize·mixed-DPI·2창 native content, Explorer 수신, MAUI 연결, 설치 protocol·서명·clean 배포 | App SDK/MAUI, native 상태/화면/물리 입력 |
| [Web](works/platforms/web.md) | 실제 history·새로고침·복원, DOM/iframe 입력·접근성·합성, GPU/upload·모바일 메모리, 플러그인·drop adapter, publish 업데이트 | 브라우저·WebGL/WebGPU, main/worker-owned, desktop/물리 모바일, Debug/Release/AOT |
| [Android](works/platforms/android.md) | 공통 fixture 연결, 물리 IME/TalkBack·PlatformView·복귀, 기기 성능, 플러그인·drop capability, 최종 APK 재설치·입력 복원, package-only/서명 | ADB intent·화면 캡처/물리 터치·키보드, 소스 APK/package-only, ABI·AOT |
| [macOS / AppKit](works/platforms/macos.md) | 기존 Desktop 정합, IME/VoiceOver·WKWebView, Metal 실측·수명, 플러그인/Finder drop/activation, 실제 추가 창, clean 배포 | AppKit/Catalyst, 과거 구현/이번 새 실행 |
| [iOS](works/platforms/ios.md) | renderer별 adapter 확인, 실기기 IME/외부 키보드/VoiceOver·WKWebView, 성능·복귀, 플러그인/drop capability/Universal Link, 서명·AOT·배포 | 실기기/시뮬레이터, Graphite/Ganesh 등 renderer, iOS/Catalyst |
| [Linux / Qt](works/platforms/linux.md) | Qt/native 의존성, Desktop 정합, IME/Orca·PlatformView, GPU 실측·수명, 파일 관리자 drop/플러그인/activation, 실제 추가 창, 패키지 배치 | Qt Quick/Widgets, Wayland/XWayland, VM/물리 GPU |
| [Mac Catalyst](works/platforms/maccatalyst.md) | scene Desktop capability, 별도 native/WebView 연결, IME/VoiceOver·렌더링, 플러그인/drop/activation, 두 scene·종료, 독립 배포 | AppKit·iOS의 결과로 대체하지 않음 |

macOS/iOS/Linux/Catalyst의 `TODO`는 **이번 works 계획의 새 실행 결과가 없다는 뜻**이다. 기존 host·adapter 전체가 미구현이라는 뜻은 아니다.

## 4. 먼저 진행할 순서

1. **원격 CI와 정리 보류 확인:** 문서 상태 정합은 아래 §5와 같이 반영했다. §2의 00에 남은 원격 CI·clean checkout 및 임시 자료 정리를 진행한다.
2. **Windows/Web 실사용 검증:** 02·03·04·05·08·09의 실제 화면·입력·브라우저·설치 VSIX 검증을 진행하고 발견한 결함을 수정한다. Windows 2창, Web history, Hot Reload의 최신 변경 후 동작을 우선 확인한다.
3. **Android 실기기 후속 검증:** 최종 APK 재설치, 물리 IME/TalkBack·PlatformView 복귀, 성능·입력 복원을 확인한다.
4. **기능 확장:** 06의 Web/Android 플러그인, 07의 외부 source 수신·다른 adapter·송신, 08의 나머지 activation을 진행한다. 07과 08은 필수 선후 의존성이 없다.
5. **다른 desktop/iOS:** 장비·toolchain이 준비되는 순서로 기존 adapter를 검증·보완하고 AppKit/Qt 추가 창 → Catalyst scene을 연결한다.
6. **선택한 출시 범위 마무리:** 10의 최신 후보·clean 배포·서명·장기 회귀를 수행한다. 첫 출시에 포함하지 않는 선택 기능까지 모두 기다릴 필요는 없다.

이 순서는 후속 작업 제안이다. 현재 요청으로 제품 구현이나 위 검증을 실행한 것은 아니다.

## 5. 문서 불일치 수정 결과 — 2026-09-29

- [x] Windows/Web 및 공통 체크리스트의 Desktop·개발 도구·플러그인·드롭·Navigation·추가 창·패키지 소비를 기존 완료 부분과 검증 잔여로 분리했다.
- [x] Android 상태를 `PARTIAL / 범위별 PASS`로 바꾸고 “작업 계획만 작성했다”를 실제 intent·route 복원 기록으로 대체했다. 최종 APK 재빌드와 앞선 실기기 실행의 차이를 명시했다.
- [x] 01의 CPU ListView/Dialog/blur 및 독립 owner-thread 검증을 반영했다. 같은 thread nested 거절과 전체 GPU/native 격리 미검증은 유지했다.
- [x] 02의 과거 Explicit/추가 창 미지원 문구를 당시 상태로 표시하고 09의 현재 지원과 연결했다.
- [x] 지원표를 2026-09-28~29 통합 결과로 맞춰 CPU Dialog·Android 실기기·Windows 두 창·Web Release publish 범위를 반영했다. 날짜와 실제 증거 범위는 유지했다.
- [x] 10·works 인덱스·출시 안내·지원표에 기존 후보가 후속 수정의 검증을 포함하지 않는다는 점과 새 후보 필요를 명시했다.
- [x] plan과 works 인덱스의 초기 개발 순서/정적 검토를 현재 잔여와 구분했다. 플랫폼 7개 문서의 일회성 산출물 경로도 공통 규칙인 `temp/testing/`으로 맞췄다.

제품 구현·검증 결과를 새로 추가한 수정은 아니다. 남은 물리 입력·화면·GPU·원격 CI·clean 배포 등의 판정은 바꾸지 않았다.

## 6. 현 필수 범위에 추가하지 않을 후속 후보

05가 명시적으로 제외한 Preview, Inspector/DevTools UI, 별도 C# LSP·시각적 디자이너, CodeLens, multi-root·다중 앱 IDE 실행, Android 기기 자동 검색, 원격 개발/브라우저 전용 VS Code는 별도 범위 결정 후 진행한다. 독립 CLI·Marketplace 공개 등 공개 배포는 M7에서 범위를 정한다.

Web·Android·iOS의 추가 창은 M6의 현재 필수 대상이 아니다. camera/notification/storage 플러그인 전체 추가, 모든 Flutter 테스트 이식, 무차별적인 `NotImplementedException` 제거도 이 목록의 필수 작업으로 확대하지 않는다.

후속 실행에서는 [공통 완료 규칙](works/README.md#공통-완료-규칙)을 따른다. 테스트는 **1,200초 timeout**, 일반 반복은 **30회 이내**로 설계하고, 자동 실행·offscreen·실제 화면·물리 입력·보조기술 증거를 구분한다. 결과 요약 후 실행별 임시 자료를 정리하며, 검증하지 않은 조합은 `notVerified` 또는 `notMeasured`, 의도적인 제한은 `unsupported`로 남긴다.
