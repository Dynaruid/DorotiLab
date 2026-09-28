# Doroti 작업 진행 순서

기준일: 2026-09-28. 이 문서는 **무엇부터 시작하고, 어느 문서를 열고, 언제 다음 작업으로 넘어갈지** 정하는 진행 안내다. 상세 작업과 체크리스트는 [works/README.md](works/README.md) 아래의 공통·플랫폼 문서에서 관리한다. 기존 정적 검토 내용과 근거는 §2·§6에 보존했다. 이번 수정은 계획 정리이며 제품 구현·실행 검증 결과를 추가하지 않는다.

## 1. 지금 시작할 작업과 전체 순서

**지금은 [00-foundation.md](works/common/00-foundation.md)부터 시작한다.** 첫 작업은 `doroti.ps1`의 `validate/audit/release` 호출 경로와 실패 종료 코드 복구다. 이어서 임시 테스트 경로·정리 규칙, 문서·지원표, Windows/Web 최소 build·기동 검증을 정리한다.

한 작업씩 진행할 때는 아래 표를 위에서부터 따른다. `common/` 문서 하나와 표에 적힌 `platforms/` 문서의 **해당 단계 항목만** 함께 연다. 플랫폼 문서 전체를 한 번에 끝내려고 하지 않는다.

| 순서 | 열 문서 | 이번 차례에 할 일 | 다음 작업으로 넘어갈 조건 |
| --- | --- | --- | --- |
| 1 | [00-foundation — M0](works/common/00-foundation.md) | CLI·실패 전파·임시 테스트 규칙·문서/지원표·Windows/Web smoke | 실행 가능한 최소 검증 경로가 있고 의도한 실패가 실패로 반환된다. |
| 2 | [01-testing — M1](works/common/01-testing.md) | 테스트 runtime 최소 구현과 Cupertino 탭 회귀 하나 | 실제 pointer 경로로 탭을 검증하고 bounded settle·종료 정리가 동작한다. |
| 3 | [02-desktop-contract — M2-A](works/common/02-desktop-contract.md) + [Windows](works/platforms/windows.md) | 기본 Windows App SDK 창과 Desktop companion 연결 | 실제 창의 크기·제목·close 취소/정리와 resize를 확인한다. |
| 4 | [03-input-accessibility-platformview — M2-B](works/common/03-input-accessibility-platformview.md) + [Windows](works/platforms/windows.md) | 한글 입력·native view focus·PlatformView 수명 | 기본 runner에서 조합 입력과 TextField ↔ native TextBox/WebView 이동을 재현·검증한다. |
| 5 | [04-rendering-lifetime — M2-C](works/common/04-rendering-lifetime.md) + [Windows](works/platforms/windows.md)·[Web](works/platforms/web.md) | 대표 장면·측정 기준·geometry/수명 회귀 | 실제 표시를 확인하고 baseline·예산을 기록한다. 다음 개발 작업을 막는 결함을 해결한다. |
| 6 | [05-vscode-hot-reload — M3](works/common/05-vscode-hot-reload.md) | A 설계 → B 생성/실행 → D snippet/import → C Hot Reload → 통합 VSIX | 새 앱 생성부터 작성·실행·실제 코드 갱신까지 통합 검증한다. |
| 7 | [06-plugin-sdk — M4](works/common/06-plugin-sdk.md) | 기존 SDK 확장·FilePicker·URL launcher | 별도 패키지 소비 앱에서 선택한 플랫폼의 기능·취소·오류·수명을 확인한다. |
| 8 | [07-os-drag-drop — M5-A](works/common/07-os-drag-drop.md) | 공통 데이터 계약 + Windows 수신부터 구현 | 실제 파일/텍스트/URI drop과 큰 파일·좌표·취소를 확인한다. |
| 9 | [08-navigation-restoration — M5-B](works/common/08-navigation-restoration.md) | activation·Router·lifecycle·복원 | 선택한 플랫폼의 cold/warm 링크와 상태 복원을 확인한다. |
| 10 | [09-multiwindow — M6](works/common/09-multiwindow.md) + [Windows](works/platforms/windows.md) | 창 문맥 ADR → 실제 Windows 추가 창 | 실제 OS 창 두 개의 독립 입력·렌더링·닫기를 검증한다. |
| 11 | [10-release-packaging — M7](works/common/10-release-packaging.md) + 출시 대상 플랫폼 | package-only 소비·서명·설치·업데이트·장기 실행 | clean 환경에서 선택한 출시 조합을 설치·실행·업데이트한다. |

표의 조건은 **다음 작업 착수 조건**이다. 모든 상세 체크리스트의 완료를 뜻하지 않는다. M1의 나머지 회귀 장면, M2의 보조기술·물리 입력·다른 호스트 검증은 관련 기능 작업에 맞춰 계속 진행하고 미완료 항목을 남긴다. 기능 전체의 완료는 각 `works` 문서의 완료 기준으로 판정한다.

기본 흐름은 **공통 기반 → Windows 기본 동작 → Windows/Web 개발 도구 → 앱 기능 → 실제 추가 창 → 출시 검증**이다. Web과 Android의 M2 연결·검증은 §4의 시점에 합류시킨다. 모든 플랫폼의 완료를 기다리며 다음 공통 작업을 묶어 두지 않는다.

## 2. `ref1.md` 재검토 결과

아래는 기존 로드맵의 정적 검토 기록이다. 현재 구현 완료 상태로 간주하지 않고 각 작업 착수 시 다시 확인한다.

| 기존 제안 | 현재 확인한 상태 | 앞으로 할 일 |
| --- | --- | --- |
| Plugin 시스템 신설 | `IDorotiNativePluginHandler`, 플러그인 manifest, RID/ABI 검사, `AddPlugin`/`AddNativePluginHandler`, SDK 등록 코드 생성이 이미 있다. | 현재 계약을 바탕으로 외부 NuGet 작성·발견·등록·수명 관리와 대표 플러그인을 완성한다. 별도 추상화 패키지는 의존성 분리가 필요할 때 결정한다. |
| OS Drag & Drop | 프레임워크 내부 drag 위젯과 별개인 OS 파일/데이터 드롭의 공통 API 및 호스트 연결은 이번 소스 검색에서 확인하지 못했다. | 수신부터 구현하고 송신·대용량·비동기 데이터 전달로 확장한다. |
| Multi-window | `_window_win32/macos/linux.cs`의 Satellite 생성은 여전히 미구현이다. 한편 `Doroti.Desktop`의 manager/controller/content factory는 이미 존재한다. | 새 Window API를 병렬로 만들지 않는다. 기본 호스트 연결, 창별 문맥 검토, 실제 추가 창 순서로 진행한다. |
| DevTools + Hot Reload | Inspector, service extension 호출, `reassembleApplication`, Preview 설정 타입은 존재한다. 제품 코드에서 `MetadataUpdateHandler`와 이를 연결하는 완성된 개발 도구 경로는 확인하지 못했다. | .NET 코드 갱신 → UI 스레드 reassemble을 우선 연결한다. Inspector/Preview 도구화는 후속으로 보류한다. |
| VS Code 확장 | `reference/AvaloniaVSCode-ARCHIVE`에 TypeScript 확장·.NET AXAML 언어 서버·별도 preview 프로세스·Webview 연결 구조가 있다. Doroti용 확장으로 연결된 구현은 이번 검색에서 확인하지 못했다. | M3에서 프로젝트/target 선택·실행·중지·로그 중심의 최소 확장을 만든다. Preview와 별도 언어 서버는 현재 범위에서 제외한다. |
| `Doroti.Testing` | 제품용 `WidgetTester` 구현/프로젝트는 확인하지 못했다. 컴파일러 검증과 샘플 probe는 남아 있지만 제품 회귀 테스트 전체를 대체하지 못한다. | 결정적 frame/time 제어와 실제 입력 라우팅을 사용하는 최소 테스트 런타임을 만든다. |
| IME + 접근성 | 호스트별 브리지가 있고 PlatformView 지원표에 full IME·UIA·TalkBack·VoiceOver·Orca 미검증 항목이 남아 있다. | 이미 있는 구현을 실제 입력·보조기술로 검증하고 결함을 해당 공통 계층/호스트에서 수정한다. |
| Deep Link / lifecycle / restoration | Router/restoration과 Web lifecycle 전달은 존재한다. OS activation·브라우저 history와 Router를 연결하는 완결된 경로는 이번 검색에서 확인하지 못했다. | 기존 lifecycle 전달을 재사용하고 cold/warm activation, URL, back/forward, 상태 저장·복원을 연결한다. |

### 이번 검토에서 추가로 발견한 선행 과제

- `Doroti/eng/doroti.ps1`의 `validate`, `audit`, `release`는 현재 없는 `eng/validate.ps1`, `eng/validate-local-storage.ps1`을 호출한다. 릴리스 검증을 신뢰하려면 먼저 복구해야 한다.
- 루트 README와 여러 지원 문서가 현재 없는 `Doroti/validation`의 실행기·검증 기록을 링크한다. 과거 기록과 현재 실행 가능한 검증을 구분해야 한다.
- Testbed 기본 Windows runner는 Windows App SDK인데, SDK의 `DOROTIDESKTOP005` 조건은 해당 호스트의 Desktop companion을 허용하지 않는다. 새 Desktop API와 기본 실행 경로 사이에 공백이 있다.
- `desktop-windows.md` 상단의 AppKit/Catalyst/Qt 구현 설명과 중간의 “Other native hosts: Pending” 표가 맞지 않는다. 플랫폼별 최신 표로 통합해야 한다.
- `.github/workflows`는 현재 체크아웃에 없다. 이 저장소에서 실행할 자동 검증 흐름을 명시적으로 추가할 필요가 있다.
- PlatformView 지원표에는 Web CPU upload 비용, Windows MAUI 연결 공백, 복수 owner·device/context loss·물리 입력 등의 제한이 남아 있다. 이는 기능 목록 확장과 별도로 추적할 핵심 품질 작업이다.

## 3. 단계별 실행 범위

### M0. 실행·검증 경로부터 복구 — 순서 1, P0

작업 문서: [00-foundation.md](works/common/00-foundation.md).

1. `validate/audit/release`의 호출 대상·실패 전파를 고치고 의도적인 실패를 확인한다.
2. [M1 임시 테스트 규칙](works/common/01-testing.md#임시-테스트의-배치실행정리)을 읽고 `temp/testing/` 생성·실행·정리, 20분 timeout, 기본 빌드/watch/pack 제외를 실행기에 반영한다. 이 규칙을 먼저 정하는 데 M1 runtime 구현 완료는 필요하지 않다.
3. 깨진 문서 링크와 플랫폼/호스트/renderer/build mode별 지원표를 정리한다.
4. 최소 상시 회귀와 Windows/Web build·기동 smoke를 CI에 연결한다. package-only 소비 앱도 가볍게 확인해 배포 경로의 문제를 조기에 기록한다.

이 단계에서는 삭제된 `Doroti/validation` 전체를 복구하지 않는다. M0의 완료 기준을 충족하면 M1 최소 runtime으로 넘어간다.

### M1. 최소 테스트 runtime만 먼저 확보 — 순서 2, P0 → P1

작업 문서: [01-testing.md](works/common/01-testing.md).

먼저 `pumpWidget`·`pump`·제한 시간 있는 `pumpAndSettle`, 결정적 clock/frame 제어, 실제 hit test/GestureArena를 통과하는 pointer tap, 테스트 종료 후 정리를 구현한다. Cupertino 탭 회귀 하나에서 정상 동작과 의도적 결함 검출을 확인하고 임시 테스트 생성 → 실행 → 결과 요약 → 정리 사이클을 검증한다.

그다음 M2에 착수한다. ListView·한글 입력·Dialog 중간 프레임·VariableBlur·PlatformView·golden·외부 패키지 소비는 해당 기능을 다룰 때 M1 검증 범위를 늘린다. 최소 runtime 통과만으로 M1 전체를 완료 표시하지 않는다.

### M2. Windows 기본 호스트부터 안정화 — 순서 3~5, P1

| 진행 순서 | 공통 문서 | 플랫폼에서 함께 할 일 |
| --- | --- | --- |
| M2-A | [02-desktop-contract.md](works/common/02-desktop-contract.md) | [Windows §1](works/platforms/windows.md)의 Windows App SDK factory/host, SDK companion 조건, 템플릿·Testbed 연결 및 실제 창 검증 |
| M2-B | [03-input-accessibility-platformview.md](works/common/03-input-accessibility-platformview.md) | [Windows §2](works/platforms/windows.md)의 한글 조합·Tab/focus·IME 소유권·native view 재생성부터 확인하고 UIA·복수 owner 등 상세 검증을 추적 |
| M2-C | [04-rendering-lifetime.md](works/common/04-rendering-lifetime.md) | [Windows §2](works/platforms/windows.md)·[Web §2~3](works/platforms/web.md)의 대표 장면·upload/present·geometry·메모리/수명 기준 확보 |

먼저 Windows의 창·입력·합성이 돌아가는 한 경로를 만든다. 이어서 Web 입력·DOM PlatformView·브라우저 시작과 렌더링 경로, Android 실기기 입력·복귀 검증을 같은 공통 장면으로 확장한다. 호스트별 상세 완료는 따로 추적한다.

**M3 착수에 필요한 것은 M1 최소 회귀와 안정적으로 수정·실행할 개발 호스트다.** 모든 보조기술·GPU loss·모든 모바일 기기 검증 완료가 M3의 일괄 선행 조건은 아니다. 다만 실행·입력·표시 결함으로 개발/Hot Reload를 검증할 수 없다면 해당 결함을 먼저 해결한다.

### M3. 생성·작성·실행·Hot Reload를 하나로 연결 — 순서 6, P1

작업 문서: [05-vscode-hot-reload.md](works/common/05-vscode-hot-reload.md). 첫 대상은 [Windows App SDK](works/platforms/windows.md)와 [Web](works/platforms/web.md)이다.

진행 순서는 문서의 알파벳 순서와 달리 **M3-A → M3-B → M3-D → M3-C → 통합 검증**으로 권장한다.

1. A: 참고 archive를 검토하고 확장 명령·설정·프로세스 수명·빌드/VSIX 구조를 정한다.
2. B: 새 프로젝트 생성 → manifest 기반 앱/target 선택 → `Debug` 실행 → 로그 → 중지를 만든다.
3. D: 생성한 C# 프로젝트에서 Widget Snippets·auto-import·Quick Fix를 연결하고 실제 에디터 조작과 build를 확인한다.
4. C: metadata update → 소유 UI 문맥의 reassemble → 화면 갱신을 먼저 검증한 뒤 전용 Hot Reload 버튼을 연결한다.
5. clean profile에 VSIX를 설치해 생성 → snippet/import → 실행 → 코드 수정 → Hot Reload를 통합 검증한다.

Windows/Web 실행 결과와 Hot Reload 지원 상태는 각각 남기고, 최소 한 개발 호스트에서 실제 변경 코드 적용과 State·입력·스크롤 보존을 통과해야 한다. Preview·Inspector UI·별도 C# LSP·전체 플랫폼 IDE 지원·시각적 디자이너는 이번 완료 범위에 넣지 않는다.

### M4. 패키지로 쓸 수 있는 네이티브 기능 — 순서 7, P2

작업 문서: [06-plugin-sdk.md](works/common/06-plugin-sdk.md).

기존 manifest/handler/SDK 등록을 재사용해 수명·dispatch·취소·오류·asset/ABI 계약을 정한 뒤 FilePicker와 URL launcher를 연결한다. 첫 Windows 구현과 별도 패키지 소비 앱을 검증하고 Web/Android, 이후 목표 플랫폼으로 확장한다.

창/view 소유권은 M2 및 M6의 설계 방향과 맞춘다. **실제 멀티윈도우 구현 완료를 기다릴 필요는 없다.** M6에서 추가 창을 연결할 때 플러그인의 창 종료·지연 응답 회귀를 다시 확인한다.

### M5. OS 데이터 전달과 Navigation — 순서 8~9, P2

- **M5-A / [07-os-drag-drop.md](works/common/07-os-drag-drop.md):** M4와 파일 접근·stream 수명을 공유한다. Windows 파일/텍스트/URI 수신 → 여러/큰 파일·좌표·취소 검증 → 송신·drag 이미지 → AppKit/Qt/Web 순으로 확장한다. 모바일은 제공 범위를 capability로 정한다.
- **M5-B / [08-navigation-restoration.md](works/common/08-navigation-restoration.md):** 공통 activation/Router 대기·중복 처리 → Web URL/back/forward/새로고침 → Android intent → iOS Universal Link → Windows protocol activation → 나머지 desktop 순으로 연결한다. 각 플랫폼에서 lifecycle·상태 migration·복원 실패도 함께 검증한다.

한 작업씩 한다면 M5-A 다음 M5-B로 진행하면 된다. 두 기능 사이에 필수 선후 의존성은 없으므로 링크/복원이 더 급하면 M5-B를 먼저 해도 된다.

### M6. 실제 추가 창 — 순서 10, P2

작업 문서: [09-multiwindow.md](works/common/09-multiwindow.md).

선행은 M2-A와 M1 격리·수명 검증이다. binding/view 모델·창별 입력/semantics/PlatformView/GPU 소유권을 짧은 ADR로 정한 뒤 **Windows App SDK → AppKit/Qt → Catalyst scene**으로 확장한다. 한 작업씩 진행할 때 AppKit와 Qt 중에서는 검증 장비가 준비된 쪽을 먼저 선택한다.

Windows에서 실제 OS 창 두 개의 독립 갱신·IME·DPI·native content·닫기와 마지막 창 정책을 먼저 통과시킨다. fake host/controller 두 개 생성은 완료 근거가 아니다. Web·Android·iOS의 추가 창 구현은 이번 필수 범위에 추가하지 않는다.

### M7. 선택한 플랫폼을 출시 가능한 상태로 마무리 — 순서 11, P3

작업 문서: [10-release-packaging.md](works/common/10-release-packaging.md)와 해당 플랫폼의 배포 항목.

M0부터 수행한 package-only smoke를 확대해 **Windows → Web → Android** 순으로 clean 설치·실행·업데이트와 실제 지원 Release/trimming/AOT 조합을 검증한다. 나머지 플랫폼은 장비·toolchain이 준비되는 순서로 확대한다. 각 출시 결과에는 동일 revision/toolchain, 서명/패키징, 데이터 유지, crash/로그, 장기 실행·자원 수명 회귀와 남은 제한을 기록한다.

표는 전체 개발의 권장 순서다. 첫 출시에 M4/M5/M6의 모든 선택 기능이 필요한 것은 아니다. 출시할 기능과 플랫폼을 정하고 그 범위의 선행 작업이 끝나면, 미포함 기능의 상태를 명시한 채 M7을 먼저 수행할 수 있다.

## 4. 플랫폼 문서는 언제 진행하는가

**공통 문서에서 계약을 정하고, 플랫폼 문서에서 그 계약의 연결·실측 결과를 남긴다.** `common/` 11개를 전부 끝낸 뒤 `platforms/`를 시작하는 구조가 아니다.

| 플랫폼 문서 | 합류 시점 | 이번에 따라갈 범위 |
| --- | --- | --- |
| [Windows](works/platforms/windows.md) | M0 smoke부터, M2-A의 첫 구현 대상 | App SDK 창 → 입력/PlatformView → 렌더링 → M3 → M4/M5 → M6 → M7. Windows MAUI는 별도 adapter·검증으로 추적 |
| [Web](works/platforms/web.md) | M0 smoke부터, M2-B/C와 M3의 개발 대상 | 시작·입력/DOM·upload/메모리·폰트 → VSIX 실행/reload → 플러그인/드롭/URL 복원 → Web publish |
| [Android](works/platforms/android.md) | M1 공통 장면이 준비되고 M2 입력·수명을 확장할 때 | 첫 모바일 실기기 입력/TalkBack·PlatformView·복귀 → 플러그인/intent·복원 → 패키지 배포. 기기 자동 검색 IDE 기능은 후속 |
| [macOS / AppKit](works/platforms/macos.md) | Windows Desktop 계약과 공통 M2 장면이 준비된 뒤 | 기존 adapter 정합·VoiceOver·WKWebView/Metal → 앱 기능 → AppKit 추가 창 → 배포 |
| [iOS](works/platforms/ios.md) | 공통 M2 입력/PlatformView·수명 장면이 준비된 뒤 | UIKit/renderer별 물리 입력·VoiceOver·복귀 → 플러그인/Universal Link·복원 → 배포 |
| [Linux / Qt](works/platforms/linux.md) | Windows Desktop 계약과 공통 M2 장면이 준비된 뒤 | Qt Quick/Widgets·display 환경별 입력/Orca·합성 → 앱 기능 → Qt 추가 창 → 배포 |
| [Mac Catalyst](works/platforms/maccatalyst.md) | Desktop 계약과 별도 adapter/scene 검증 환경이 준비된 뒤 | Catalyst 입력/VoiceOver·PlatformView → 앱 기능 → scene 기반 추가 창 → 배포 |

macOS/iOS/Linux/Catalyst는 준비된 장비부터 진행하며 서로의 완료를 기다릴 필요는 없다. AppKit과 Catalyst, Windows App SDK와 MAUI, 각 renderer의 통과 결과를 다른 호스트의 지원 근거로 사용하지 않는다.

### 먼저 착수할 수 있는 작업과 계속 유지할 작업

- M3-A/B는 M0의 CLI·템플릿 실행 경로가 준비되면 먼저 시작할 수 있다. 순서 선택이 부담되면 §1의 기본 순서를 그대로 따른다.
- M3-D는 생성 앱의 C# 프로젝트 로딩 후 시작하고, M3-C는 M1 최소 회귀·개발 호스트·B의 실행 세션 관리가 준비된 뒤 연결한다.
- M1 검증 범위와 M2 플랫폼 품질은 이후 기능 작업에서도 계속 보완한다. 미검증 장비는 `notVerified`, 일부 성공은 `PARTIAL`로 기록한다.
- M7의 package-only smoke는 M0부터 유지한다. 서명·clean 설치·업데이트·장기 실행을 포함하는 최종 출시 판정은 선택한 기능의 선행 조건을 충족한 뒤 수행한다.

## 5. 진행 상황과 공통 완료 규칙

`plan.md`에서는 순서·착수 조건을 관리하고, 실제 체크박스·결과·미완료 항목은 해당 `works/common/` 및 `works/platforms/` 문서에 기록한다. 동일 기능의 상태를 이 문서에 중복 복사하지 않는다. 기록 형식은 [works의 결과 기록 형식](works/README.md#결과-기록-형식)을 사용한다.

- 각 작업은 **공개 계약·호스트 연결·실행 가능한 샘플·필요한 회귀 검증·지원표 갱신**을 함께 완료한다. 문서 작성이나 컴파일 성공만으로 기능 구현을 완료 처리하지 않는다.
- 결과에 commit/revision, OS·기기·renderer, configuration/AOT, 명령, 기대값·실제값을 기록한다. 과거 결과는 날짜와 적용 범위를 표시한다.
- `PASS`는 실행한 범위에만 사용한다. 실행하지 않은 환경은 `notVerified`, 일부 성공은 `PARTIAL`, 의도적으로 제공하지 않는 기능은 `unsupported`로 남긴다.
- 테스트는 저장소 지침대로 **20분 timeout**을 사용한다. 일반 반복 검증은 30회 이내에서 설계하고, 별도 장기 시험이 필요하면 목적·종료 조건을 정한다.
- 일회성 테스트 스크립트·프로젝트와 테스트 원시 로그·캡처·빌드 산출물은 저장소 루트 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고, 결과 요약 후 삭제한다. 최소 상시 회귀 소스·작은 fixture만 `Doroti/tests/`에 선별해 남긴다. 일반 제품 빌드 산출물과 기존 `Doroti/artifacts`는 기존 정리 지침을 따른다.
- 임시 테스트는 기본 빌드·watch·패키징·전체 CI에서 제외한다. 실패 조사로 임시 파일을 남길 때는 보류 사유·정리 시점을 기록하고, 삭제 후에는 원시 파일이 남아 있다고 보고하지 않는다. 검증 파일의 영구 보존 없이도 결과 요약과 재현 절차로 필요한 검증을 기록할 수 있다.
- Flutter는 동작·렌더링·입력 모델의 비교 기준으로 사용한다. C#에서 유지보수하는 제품 소스를 기본으로 삼고, compiler는 필요한 import/의미 비교에 사용한다.

한 작업을 마칠 때는 **어느 공통 작업의 어느 플랫폼 범위까지 확인했는지, 남은 항목은 무엇인지, 다음에 열 문서는 무엇인지**를 함께 기록한다. 착수 조건을 충족한 다음 작업으로 이동하더라도 미완료 기능을 완료 처리하지 않는다.

## 6. 이번 검토의 주요 근거

아래 파일은 현황 판단의 근거다. 문서에 기록된 과거 실행 결과를 이번 검토에서 재실행한 것으로 해석하지 않는다.

- 이전 검토 `ref1.md`(현재 체크아웃에 없음; §2에 요약 보존), [프로젝트 소개](README.md), [프레임워크 구조와 실행 안내](Doroti/README.md)
- [현재 CLI](Doroti/eng/doroti.ps1), [제품 솔루션](Doroti/Doroti.Product.slnx), [기본 runner 선택](samples/DorotiTestbedApp/doroti-workspace.json)
- [플러그인 manifest/handler/ABI 계약](Doroti/src/Doroti.Hosting/DorotiApplicationBoundary.cs), [앱 등록](Doroti/src/Doroti.Hosting/DorotiApplicationBootstrap.cs), [runner 코드 생성과 Desktop 허용 조건](Doroti/src/Doroti.Runner.Sdk/Sdk/Sdk.targets)
- [Desktop 구현 상태](Doroti/docs/desktop-windows.md), [창 관리자](Doroti/src/Doroti.Desktop/DorotiWindowManager.cs), [호스트·content 계약](Doroti/src/Doroti.Desktop/WindowHost.cs)
- [Windows Satellite 미구현](Doroti/src/Doroti.Framework.Widgets/_window_win32.cs), [macOS](Doroti/src/Doroti.Framework.Widgets/_window_macos.cs), [Linux](Doroti/src/Doroti.Framework.Widgets/_window_linux.cs)
- [Widget binding](Doroti/src/Doroti.Framework.Widgets/binding.cs), [Scheduler binding](Doroti/src/Doroti.Framework.Scheduler/binding.cs), [Services binding](Doroti/src/Doroti.Framework.Services/binding.cs)
- [service extension과 reassemble](Doroti/src/Doroti.Framework.Foundation/binding.cs), [Inspector](Doroti/src/Doroti.Framework.Widgets/widget_inspector.cs), [Preview 타입](Doroti/src/Doroti.Framework.WidgetPreviews/widget_previews.cs), [VS Code 참고 자료](reference/AvaloniaVSCode-ARCHIVE/README.md)
- [Avalonia 확장 manifest](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/package.json), [확장 진입점](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/extension.ts), [솔루션 발견](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/services/solutionParser.ts), [Preview 기동](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/commands/previewerProcess.ts)
- [참고 프로젝트 생성 명령](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/commands/createNewProject.ts), [Doroti 앱 템플릿 옵션](Doroti/templates/Doroti.Templates/content/doroti-app/.template.config/template.json)
- [참고 C# snippets](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/csharp.json), [Doroti C# 위젯 샘플](samples/DorotiSampleApp2/src/App.cs)
- [Preview 프로세스 수명](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/previewProcessManager.ts), [Preview 서버](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/services/previewServer.ts), [메시지 parser](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/services/messageParser.ts), [Webview 패널](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/panels/WebPreviwerPanel.ts)
- [AXAML 언어 서버](reference/AvaloniaVSCode-ARCHIVE/src/AvaloniaLSP/AvaloniaLanguageServer/Program.cs), [참고 빌드 스크립트](reference/AvaloniaVSCode-ARCHIVE/build.sh), [참고 테스트](reference/AvaloniaVSCode-ARCHIVE/src/vscode-avalonia/src/test/suite/extension.test.ts), [submodule 목록](reference/AvaloniaVSCode-ARCHIVE/.gitmodules), [참고 코드 라이선스](reference/AvaloniaVSCode-ARCHIVE/LICENSE)
- [PlatformView 지원·미검증 표](Doroti/docs/platform-views/support-matrix.md), [Web host lifecycle](Doroti/src/Doroti.Host.Web/BrowserHostContracts.cs), [Web 브라우저 연결](Doroti/src/Doroti.Host.Web/Web/doroti.web.ts)

검색에서 확인하지 못한 기능은 완전한 부재를 증명한 것이 아니다. 해당 단계 착수 시 현재 branch의 실제 호출 경로와 새로 추가된 구현을 다시 확인하고 중복 작업을 줄인다.
