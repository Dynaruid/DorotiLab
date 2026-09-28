# Doroti 개발 로드맵

검토 기준일: 2026-09-28. `ref1.md`의 이전 제안과 현재 소스·SDK·샘플·지원 문서를 비교한 계획이다. 이번 검토는 정적 검토이며, 빌드·실기기·성능 검증을 새로 수행한 결과는 아니다. 아래 체크박스는 앞으로 할 작업이다.

## 1. 방향과 우선순위

Doroti는 Material/Cupertino 위젯과 공통 렌더링 계층, 여러 네이티브 호스트를 이미 갖추고 있다. 다음 목표는 **새 앱을 만들고, 수정하고, 테스트하고, 네이티브 기능을 붙여 배포하는 전체 과정을 재현 가능하게 만드는 것**이다.

우선순위는 다음과 같다.

1. **P0 — 개발·검증 경로 복구:** 끊어진 CLI와 문서, 지원 상태를 정리하고 작은 자동 회귀 테스트 기반을 마련한다.
2. **P1 — 실제 앱의 기본 동작 확보:** 기본 Windows 호스트의 Desktop API, 텍스트 입력·접근성·PlatformView 경계, 렌더링 안정성을 완성한다.
3. **P1 — 개발 생산성:** Widget 테스트와 VS Code 실행 확장을 마련하고 Hot Reload를 연결한다. Inspector·Preview는 후속 후보로 둔다.
4. **P2 — 앱 기능 확장:** 기존 플러그인 기반을 패키지 생태계로 확장하고 OS Drag & Drop, Deep Link·복원을 연결한다.
5. **P2 — 멀티윈도우:** 창별 실행 문맥과 리소스 소유권을 먼저 정리한 뒤 실제 추가 창을 구현한다.
6. **P3 — 배포 품질:** 목표 플랫폼별 패키지 소비, AOT, 서명·설치·업데이트와 장기 실행을 검증한다.

Windows App SDK와 Web을 첫 개발·자동화 기준으로 삼고 Android 실기기를 첫 모바일 기준으로 삼는 것을 제안한다. macOS/iOS/Linux/Catalyst는 공통 계약을 공유하되 각 호스트의 실제 검증 결과로 지원 상태를 올린다. 모든 플랫폼의 완료를 기다리며 앞선 플랫폼의 개선을 묶어 두지는 않는다.

## 2. `ref1.md` 재검토 결과

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

## 3. 단계별 로드맵

### M0. 실행·검증·지원 현황 정리 — P0

**목적:** 새 체크아웃에서 어떤 명령이 동작하고 무엇이 검증됐는지 확인할 수 있게 한다.

- [ ] `doroti validate/audit/release`의 호출 경로를 점검하고 유지할 suite를 현재 제품 구조에 맞게 다시 연결한다. 삭제된 과거 suite 이름만 남겨 성공으로 처리하지 않는다.
- [ ] 검증 실패·실행기 누락이 최상위 명령의 실패 종료 코드로 이어지도록 하고, 모든 필수 단계가 성공했을 때만 `Release: PASS`를 출력한다.
- [ ] 일회성 테스트 스크립트·프로젝트·fixture는 저장소 루트 `temp/testing/<작업-ID>/<실행-ID>/`에 구성하고 검증·요약 후 삭제한다. 상시 유지할 최소 회귀 소스만 `Doroti/tests/`, 공통 실행기만 `Doroti/eng/`에 선별해 남긴다. 삭제된 `Doroti/validation` 전체를 되살리는 방식은 피한다.
- [ ] 테스트 실행에 20분 timeout을 적용하고 timeout 시 자식 프로세스 종료와 실패 기록을 남긴다. 테스트 원시 산출물은 해당 `temp/testing/` 실행 폴더에 모으고, 결과 요약 후 정리한다. 일반 제품 빌드 산출물과 기존 `Doroti/artifacts`는 기존 정리 지침을 따른다.
- [ ] `/temp/`를 Git ignore와 관련 빌드·프로젝트 검색·watch·pack 대상에서 제외하고, 임시 테스트를 기본 solution·제품 참조·전체 CI에 연결하지 않는다. 실행별 폴더 생성·종료·정리와 기존 일회성 파일 분류는 M1의 임시 테스트 규칙을 따른다.
- [ ] README 한·영판, 지원표, 샘플 문서의 깨진 경로·폐기된 도구 요구사항을 수정한다. 필요한 역사적 근거는 `history/`의 실제 존재하는 기록으로 연결한다.
- [ ] 플랫폼/호스트/renderer/build mode별 지원표를 하나의 기준으로 정리한다. `구현됨`, `자동 검증`, `실행 검증`, `물리 검증`, `미지원`을 구분한다.
- [ ] CI에 소스·문서 경로 검사, 선택한 공통 테스트, Windows 기본 runner와 Web 빌드/기동 smoke를 연결한다. GPU/실기기 검증은 해당 장비가 있는 실행 경로로 분리한다.

**완료 기준:** 새 체크아웃에서 문서에 적힌 최소 검증 명령을 실행할 수 있고, 의도적인 실패가 정상적으로 실패 처리된다. `temp/testing/`과 삭제 가능한 artifacts가 없어도 제품 빌드·상시 회귀 검증이 동작하고 최신 지원 상태와 재현 절차를 읽을 수 있다.

### M1. 테스트 런타임과 핵심 회귀 장면 — P0 → P1

**선행:** M0의 실행기·산출물 규칙.

**관리 원칙:** 일회성 검증은 `temp/testing/`에서 작성·실행하고 결과 요약 후 삭제한다. 제품용 `Doroti.Testing`과 선별한 최소 상시 회귀만 유지한다. 상세 배치·실행·정리 및 완료 기준은 [M1 작업 계획](works/common/01-testing.md#임시-테스트의-배치실행정리)을 따른다.

- [ ] `Doroti.Testing`의 최소 계약을 정한다: `pumpWidget`, `pump`, 제한 시간이 있는 `pumpAndSettle`, `find.byType/byKey/text`, `tap/drag`, 키·텍스트 입력, semantics 조회.
- [ ] 테스트가 clock/frame scheduler를 제어하도록 하고, pending timer/animation과 처리되지 않은 예외를 실패 진단에 포함한다.
- [ ] 테스트 종료 후 binding/root/focus/texture/native resource가 남지 않게 한다. 초기에는 직렬 실행을 허용하고 static binding 격리가 검증되기 전 병렬 실행을 약속하지 않는다.
- [ ] pointer hit test → GestureArena → callback 경로를 통과시킨다. 위젯 콜백 직접 호출만으로 입력 테스트를 통과시키지 않는다.
- [ ] 우선 회귀 장면을 고정한다: Cupertino 탭 전환, ListView 스크롤, 한글 조합/selection, Dialog 중간 프레임, VariableBlur viewport/DPR/축소 렌더링 크기, PlatformView 생성·해제.
- [ ] golden 비교는 폰트·DPR·renderer·색 공간·허용 오차를 고정한다. CPU 이미지 비교, GPU offscreen 비교, 실제 화면 캡처를 별도 결과로 기록한다.
- [ ] 테스트 실패 시 비교 이미지, 트리, frame 정보와 재현 명령을 출력한다. 외부 앱도 패키지만 참조해 간단한 위젯 테스트를 작성할 수 있게 한다.

**완료 기준:** 의도적으로 탭 대상·hit test·blur geometry를 깨뜨리면 해당 테스트가 실패하며, 복구 후 통과한다. 지속 animation에서는 `pumpAndSettle`이 무한 대기하지 않는다.

### M2. 기본 호스트와 입력·렌더링 신뢰성 — P1

**선행:** M1의 최소 회귀 경로. 아래 항목은 호스트별로 완료 처리한다.

#### M2-A. Windows 기본 실행 경로와 Desktop API

- [ ] Windows App SDK용 `IWindowHostFactory`/`IWindowHost`를 구현하고 SDK Desktop companion 허용 조건·템플릿·샘플을 함께 연결한다.
- [ ] 크기/DPI/최소·최대 크기, show/hide/focus, 제목·native caption·appearance, close 취소·render drain을 기존 Desktop 계약으로 통합한다.
- [ ] 첫 표시, 창 resize, 최소화/복원, 디스플레이 전환, 종료 중 pending GPU 작업을 실제 화면과 리소스 수명으로 검증한다.
- [ ] AppKit/Catalyst/Qt/Windows MAUI의 기존 adapter와 지원표를 맞춘다. 플랫폼이 제공하지 않는 동작은 capability와 명확한 거절 결과로 표현한다.

**완료 기준:** 기본 Windows 명령으로 실행한 샘플이 Desktop companion을 사용하며, native 창 닫기와 API 닫기가 동일한 취소·정리 경로를 따른다. live resize는 실제 표시 프레임의 geometry·깜빡임으로 판정한다.

#### M2-B. 텍스트·접근성·PlatformView

- [ ] 한글 조합 시작/갱신/확정/취소, selection/caret, 후보창 좌표, multiline, clipboard, 단축키, focus loss를 묶은 입력 장면을 만든다.
- [ ] Doroti TextField ↔ native TextBox/EditText/WebView 이동에서 Tab/Shift+Tab, IME 소유권, 키 중복 전달과 눌림 상태 해제를 검증한다.
- [ ] semantics의 이름·역할·값·상태·action·focus·탐색 순서를 검증하고, UIA/TalkBack/VoiceOver/Orca 실제 사용 결과를 별도로 기록한다.
- [ ] PlatformView 복수 owner, clip/transform/z-order, gesture 경쟁, 생성·해제·재생성, 늦게 도착한 비동기 결과를 점검한다.
- [ ] Windows MAUI WebView 연결 및 Catalyst 등 별도 adapter가 필요한 조합은 구현 범위를 명시해 처리한다. 기본 Windows App SDK 결과를 다른 호스트의 통과 근거로 사용하지 않는다.

**완료 기준:** 목표 호스트에서 조합 입력 중 focus 이동과 native view 재생성 후에도 입력·semantics가 정상이다. 미검증 보조기술/기기는 지원표에 그대로 남긴다.

#### M2-C. 렌더링 성능과 수명

- [ ] 대표 장면을 고정한다: 긴 리스트, VariableBlur, 여러 효과, WebView 위 overlay, 이미지/영상 texture, resize/DPR 변경.
- [ ] UI build/layout/paint, GPU 작업, readback/upload, 실제 present 간격, 메모리/VRAM을 구분해 측정한다. 대상 기기·해상도별 baseline과 허용 예산을 먼저 정한다.
- [ ] Web PlatformView CPU upload와 frame admission/backpressure, 모바일 Web 메모리, pause/resume, context/device loss 복구를 우선 점검한다.
- [ ] VariableBlur의 축소 품질·adaptive 처리에서도 논리 capture 영역, pixel origin, viewport 크기를 보존한다. 성능 개선의 완료 조건에 geometry 회귀 검증을 포함한다.
- [ ] 폰트 CDN/로컬 asset/offline 정책, 초기 글꼴 교체와 한글 fallback을 실제 호스트에서 재검증한다.

**완료 기준:** 정한 장면·기기·예산으로 전후 수치를 재현할 수 있고, 성능 변경이 크기·입력·합성 정확성을 깨뜨리지 않는다. GPU 완료 시간만으로 화면 FPS를 주장하지 않는다.

### M3. VS Code 실행 확장과 Hot Reload — P1

**현재 범위:** **새 프로젝트 생성 마법사 → Widget Snippets·import 지원으로 코드 작성 → 프로젝트·target 선택 → 실행 → 확장 전용 Hot Reload 버튼 → 로그 확인·중지**를 이번 확장 범위로 구현한다. 여기서 import는 C#의 `using` 자동 추가·누락 해결을 뜻한다. Preview는 후속 검토로 보류한다. 생성·실행 기능은 먼저 개발할 수 있지만, 이번 VSIX의 완료 조건에는 M3-C의 Hot Reload와 M3-D의 작성 지원까지 포함한다.

#### M3-A. AvaloniaVSCode 구조 검토와 최소 설계

검토 대상은 로컬 `reference/AvaloniaVSCode-ARCHIVE` 체크아웃이다. 구조를 정적으로 참고했으며 해당 확장을 빌드하거나 실행해 검증한 결과는 아니다.

| 참고 영역 | 확인한 역할 | 이번에 사용할 범위 |
| --- | --- | --- |
| `src/vscode-avalonia/package.json`, `src/extension.ts`, `src/commands/` | 명령·설정·메뉴 등록과 활성화 | Doroti용 manifest/명령 등록과 기능별 모듈 분리를 참고한다. |
| `src/commands/createNewProject.ts` | 이름·폴더 입력 후 `dotnet new` 실행과 생성 폴더 열기 | Doroti 템플릿을 사용하는 생성 마법사로 구성하고 취소·이름 검증·기존 폴더 충돌·생성 실패 처리를 보완한다. |
| `src/vscode-avalonia/csharp.json`, `package.json`의 snippets 등록 | C# 코드 snippet과 prefix·placeholder 제공 | 등록 구조를 참고해 Doroti Widget Snippets를 제공한다. Avalonia property/event 본문은 Doroti의 실제 widget API로 교체한다. |
| `src/services/solutionParser.ts` | 솔루션 발견과 프로젝트 metadata cache | 발견·선택 책임 분리만 참고한다. Doroti는 `doroti-workspace.json`을 기준으로 앱과 runner를 선택한다. |
| `src/commands/previewerProcess.ts`, `src/previewProcessManager.ts` | child process 기동·재사용·종료 | 실행 프로세스의 소유권·종료 관리만 참고하고 preview 전용 프로세스/통신은 만들지 않는다. |
| `src/client.ts`, `src/runtimeManager.ts`, `src/AvaloniaLSP/` | .NET AXAML 언어 서버 연결 | 이식하지 않는다. C# 완성·진단·디버깅은 기존 C# 도구를 사용한다. |
| `src/services/previewServer.ts`, `src/services/messageParser.ts`, `src/panels/WebPreviwerPanel.ts` | TCP/BSON designer 통신과 Webview preview | 현재 범위에서 제외한다. 별도 DevTools endpoint·PreviewHost·Webview도 확장 MVP에 필요하지 않다. |
| `build.sh`, `src/test/`, `.gitmodules`, `LICENSE` | 보조 도구 빌드·테스트 진입점·submodule·MIT 고지 | Doroti 전용 빌드/VSIX 패키징과 실제 실행 검증을 구성하고, 코드 차용 시 원 저작권·라이선스 고지와 출처를 포함한다. |

참고 구현의 `solutionParser.ts`는 첫 `.sln`/첫 workspace를 선택하고 cache를 솔루션 basename으로 구분한다. 이 선택 방식을 그대로 이식하지 않는다. `src/test/`의 테스트 본문은 예제 assertion 수준이며, 현재 로컬 `src/SolutionParser`와 `src/AvaloniaVS` submodule 디렉터리는 비어 있으므로 archive 전체가 바로 재빌드된다고 전제하지 않는다.

제안하는 신규 배치는 아래와 같다. 모두 구현 예정 경로다.

```text
Doroti/tools/vscode-doroti/
  package.json          # 명령·설정·지원 VS Code 버전
  src/extension.ts      # 활성화와 disposable 정리
  src/commands/         # create project, select project/target, run, hot reload, stop, show logs
  src/services/         # manifest 발견, template/CLI adapter, 실행·reload 수명
  src/editing/          # import 연동·snippet 삽입 보조
  snippets/widgets.json # C# Widget Snippets와 placeholder
  test/                 # 핵심 로직·Extension Development Host 검증
  README.md             # 설치·실행·제한 안내
```

- [ ] archive 의존성 버전을 그대로 복사하지 않고 착수 시 지원 VS Code/Node 조합을 정한다. 패키지 관리자는 하나로 정하고 lockfile 및 빌드·테스트·VSIX 생성 명령을 고정한다.
- [ ] VS Code 의존성을 framework에 추가하지 않는다. 별도 .NET 보조 서버·protocol·복잡한 plugin 계층은 실제 필요가 생길 때 도입한다.

#### M3-B. 프로젝트 생성 마법사·선택·실행·로그

**선행:** M0의 CLI 실행 경로·실패 전파와 사용 가능한 Doroti 템플릿. 이 단계의 개발은 M1·Hot Reload 완료 전 시작할 수 있으며, 최종 VSIX 검증은 M3-C/D와 통합한다.

- [ ] `Doroti: Create Project`, `Select Project`, `Select Target`, `Run`, `Hot Reload`, `Stop`, `Show Logs`와 선택 상태 표시를 구현한다. 별도 Build/Doctor 화면은 만들지 않는다.
- [ ] 새 프로젝트 생성 마법사에서 프로젝트 이름·생성 위치를 입력받고 기존 `dotnet new doroti-app` 템플릿을 호출한다. 현재 템플릿의 기본값을 사용하며, 첫 실행 target 선택은 생성 후 manifest에 선언된 항목에서 제공한다. 생성할 플랫폼을 줄이는 새 템플릿 옵션은 이번 필수 범위에 넣지 않는다.
- [ ] SDK·템플릿 누락 시 설치 방법을 안내하고, 잘못된 이름·기존 비어 있지 않은 폴더·입력 취소·생성 중 취소·생성 실패를 구분한다. 기존 파일을 덮어쓰지 않고 실패/취소 시 이번 생성으로 남은 경로를 알린다. 성공하면 생성 폴더 열기와 앱 선택으로 이어지게 한다.
- [ ] `doroti-workspace.json`의 `applicationProject`/`platforms`에서 앱과 runner를 찾는다. 여러 후보가 있으면 선택하게 하고 첫 `.sln`을 임의 실행하지 않는다. 첫 버전은 선택한 앱 하나·실행 세션 하나만 지원하며 multi-root와 동시 다중 앱 실행은 후속으로 둔다.
- [ ] 현재 CLI는 모든 플랫폼 alias를 요구하므로 일부 플랫폼만 선언한 앱의 검증 계약을 CLI에서 정리한다. 확장에는 선언된 target만 표시하고 runner 선택 로직을 중복 구현하지 않는다.
- [ ] 기존 `Doroti/eng/doroti.ps1` 호출을 재사용하고 CLI 경로를 설정 가능하게 한다. 첫 버전은 저장소 CLI와 설치된 템플릿 사용을 명시하며, 마법사가 만든 앱도 이 구성으로 실행 검증한다. 독립 CLI 배포는 M7로 넘긴다. device discovery·범용 debug protocol은 제외하되 Hot Reload 요청·결과 전달에 필요한 최소 계약은 M3-C에서 구현한다.
- [ ] 첫 대상은 로컬 Windows App SDK와 Web으로 잡고 개발 실행에는 `Debug`를 명시한다(현재 CLI 기본값은 `Release`). Windows는 실제 앱 창, Web은 외부 브라우저로 실행한다. Run에서 기존 build/run 흐름과 출력을 사용하고 추가 UI는 필요한 최소 상태만 표시한다.
- [ ] PowerShell/.NET SDK/선택 target의 도구 누락, build 실패·실행 실패를 명확하게 보여 준다. 실패 종료 코드·중복 Run·실행 중 target 변경·Stop·확장 종료 시 자신이 시작한 프로세스 정리는 첫 버전에서도 구현한다. build와 앱 실행 사이 프로세스 소유권이 끊기는 host는 CLI 계약을 보완한다.
- [ ] C# 완성·오류 표시·디버깅은 기존 C# 확장과 사용자 설정을 활용하고 Doroti import·Widget Snippets는 M3-D에서 연결한다. `launch.json`/`tasks.json` 자동 생성, 자체 LSP/debug adapter, Inspector UI는 추가하지 않는다.
- [ ] Restricted Mode에서는 템플릿 실행·앱 실행·Hot Reload를 비활성화하고 workspace trust 변경을 반영한다. 실행 파일/인자는 배열로 전달하며 프로젝트 경로를 shell 문자열에 이어 붙이지 않는다. [Workspace Trust 공식 지침](https://code.visualstudio.com/api/extension-guides/workspace-trust)을 적용한다.
- [ ] [VS Code 확장 테스트 지침](https://code.visualstudio.com/api/working-with-extensions/testing-extension)에 따라 생성 마법사·명령·앱 선택·실패 전파·실행/종료를 검증한다. 테스트는 20분 timeout을 적용하고, clean VS Code profile에 VSIX를 설치해 생성된 앱의 실제 Windows/Web 실행과 M3-C의 Hot Reload 버튼을 통합 검증한다.

**완료 기준:** 마법사로 새 앱 생성 → 폴더 열기 → 앱/target 선택 → 실행 → 로그 확인 → 중지를 재현하고 기존 앱도 동일하게 실행한다. 한글/공백 경로·생성 취소·폴더 충돌·잘못된 manifest·누락 SDK/템플릿·build 실패·중복 실행·VS Code 종료를 처리하며 소유한 실행 프로세스가 남지 않는다. 이번 VSIX 완료에는 아래 M3-C/D 통합 검증도 필요하다.

#### M3-C. Hot Reload runtime과 확장 전용 버튼 — 이번 구현 범위

**선행:** M1의 최소 회귀 경로와 검증 가능한 개발 호스트. 버튼 통합에는 M3-B의 실행 세션 관리가 필요하다.

- [ ] .NET metadata update 지원 환경을 확정하고 update hook에서 소유 UI 문맥으로 `reassembleApplication()`을 요청한다. pending frame과 중복 reload를 직렬화한다.
- [ ] 지원되는 코드 변경에서 State·입력 값·스크롤 위치를 보존한다. 지원되지 않는 변경은 재시작 필요를 표시하고 오류 후 재수정이 가능하게 한다.
- [ ] 먼저 CLI/개발 호스트에서 코드 변경의 metadata update → UI 갱신을 검증하고, 같은 경로를 `Doroti: Hot Reload` 명령과 실행 중 표시되는 전용 버튼에 연결한다. 버튼은 저장되지 않은 변경의 저장/취소를 처리한 뒤 코드 갱신을 요청한다. `reassembleApplication()`만 호출해 변경 코드가 적용된 것으로 처리하지 않는다.
- [ ] 현재 실행 세션의 지원 capability·요청 id·처리 중/성공/실패/재시작 필요 결과를 전달하는 최소 계약을 정한다. 기존 CLI 제어 경로를 우선 활용하고 부족한 연결만 추가한다. 연속 클릭과 저장 기반 reload가 중복 적용되지 않도록 직렬화하며 종료한 세션의 늦은 응답을 무시한다.
- [ ] 실행 전·Release·미지원 host에서는 버튼을 비활성화하고 이유를 표시한다. 코드 컴파일 오류는 로그로 연결하고 수정 후 다시 시도할 수 있게 한다. 재시작이 필요한 변경은 상태 초기화를 알리는 Restart 동작으로 연결한다.

**완료 기준:** 설치한 VSIX에서 마법사로 만든 앱 실행 → 코드 수정 → Hot Reload 버튼 클릭 → State·입력·스크롤을 보존한 UI 갱신을 재현한다. 컴파일 오류 후 재시도·연속 클릭·재시작 필요·실행 종료를 검증한다. Windows/Web의 지원 여부는 각각 기록하고 최소 한 개발 호스트에서 실제 갱신을 통과해야 이번 확장을 완료 처리한다. runtime 갱신 경로는 VS Code 밖에서도 사용할 수 있게 유지한다.

#### M3-D. import 지원·Widget Snippets — 이번 구현 범위

**선행:** 실제 Doroti C# API와 M3-B에서 생성한 앱의 정상적인 C# 프로젝트 로딩. Hot Reload 구현 전 독립적으로 개발할 수 있다.

- [ ] Doroti 타입의 완성 후보를 선택하면 필요한 `using`이 추가되고, 이미 입력한 미해결 타입은 Quick Fix로 import할 수 있게 구성한다. 기존 C# 언어 서비스의 완성·code action을 우선 연동하고 지원 확장·버전·설정을 명시한다. 프로젝트 로딩 실패·참조 누락·언어 서비스 미설치를 구분해 안내한다. 기본 편집 기능은 [VS Code C# 안내](https://code.visualstudio.com/docs/languages/csharp)를 기준으로 검토하되 Doroti 타입에 대한 실제 동작을 확인한다.
- [ ] 중복 `using`, `global using`, alias, file-scoped/block namespace, 같은 이름의 다른 타입을 처리한다. 여러 namespace가 가능한 경우 사용자 선택을 제공하고 일괄 namespace 추가로 모호성을 만들지 않는다. 기존 C# 기능으로 충족되지 않는 Doroti 전용 import 보조만 추가하며 전체 C# 언어 서버는 만들지 않는다.
- [ ] `snippets/widgets.json`을 `csharp` 언어용으로 등록한다. 첫 목록은 StatelessWidget, StatefulWidget + `State<T>`, `build`, `initState`/`dispose`/`setState`, Row/Column/Container/Text, Material/Cupertino 앱 뼈대로 정한다. prefix는 `dstateless`, `dstateful`, `drow`처럼 Doroti용으로 구분하고 클래스명·child/children·본문에 연결 placeholder와 최종 cursor 위치를 제공한다. [Snippet 등록 지침](https://code.visualstudio.com/api/language-extensions/snippet-guide)을 따른다.
- [ ] snippet의 생성자·override 이름·반환형·namespace·수명 처리는 현재 C# 제품 API와 샘플에 맞춘다. Dart 문법이나 Avalonia 코드를 그대로 복사하지 않는다. 필요한 import를 각 snippet의 metadata/설명에 명시하고, 삽입 후 누락 import를 해결할 수 있게 연결한다. 정적 snippet JSON만으로 자동 `using` 편집이 완료된다고 가정하지 않고 필요한 삽입 보조·code action 경로를 함께 검증한다.
- [ ] 새 파일과 기존 파일에서 prefix 완성·삽입·Tab 이동·연결된 이름 변경·Undo와 import 처리를 실제 에디터에서 검증한다. snippet별 최소 사용 예제를 생성해 build하고, 중복 import·타입 충돌·기존 C# snippet과의 충돌도 확인한다. 테스트는 공통 20분 timeout을 적용한다.

**완료 기준:** 설치한 VSIX에서 마법사로 앱 생성 → StatefulWidget snippet 삽입 → 이름/본문 편집 → 필요한 `using` 추가 → build/run → 코드 수정 후 Hot Reload를 재현한다. 일반 타입 완성 시 auto-import와 미해결 타입의 Quick Fix도 각각 검증하며, snippet 텍스트 삽입만 성공한 상태를 작성 지원 완료로 간주하지 않는다.

#### 후속 후보 — 현재 구현·완료 기준에서 제외

- **Preview 전체:** `WidgetPreviews` 실행 host, preview 등록/검색, Webview, 크기·테마·locale 제어. 현재는 별도 구현 계획을 확정하지 않는다.
- **Inspector/DevTools UI:** 트리 탐색·highlight·소스 이동·성능 timeline·원격 진단 endpoint. 디버깅 필요가 구체화되면 범위를 다시 정한다.
- **IDE 편의 기능:** CodeLens, `launch.json`/`tasks.json` 자동 생성.
- **지원 범위 확대:** multi-root·동시 다중 앱, Android 기기 자동 검색·로그 통합, macOS/Linux 검증, WSL/SSH/Dev Container 및 browser-only VS Code.
- **공개 배포:** 독립 CLI 제공, package-only 앱 통합 검증, Marketplace 공개는 M7에서 다룬다. 첫 산출물은 로컬 설치 가능한 VSIX다.

### M4. 플러그인 SDK와 대표 네이티브 기능 — P2

**선행:** M0/M1, 창·view 문맥 계약은 M2/M6와 일치시킨다.

- [ ] 현재 manifest/handler/SDK 등록 구조를 문서화하고 외부 NuGet이 target별 구현·native asset·Web module을 제공하는 최소 패키지 형식을 정한다.
- [ ] 앱 단위와 view/window 단위 플러그인 수명, UI thread dispatch, cancellation, event stream, dispose, 오류·권한 거절·미지원 결과를 정의한다.
- [ ] RID/ABI 불일치, 중복 id/channel, 누락된 handler/asset을 build 또는 초기화 단계에서 구체적으로 진단한다.
- [ ] 등록 생성은 기존 SDK 경로를 확장한다. Source Generator 도입은 reflection 회피·사용성·AOT 이득이 분명할 때 결정한다.
- [ ] 첫 대표 기능은 FilePicker와 URL launcher로 잡는다. 선택 취소와 외부 파일 접근 수명까지 포함한다. 이후 camera/notification/storage는 요구와 플랫폼 검증 장비에 맞춰 추가한다.
- [ ] 네이티브/Web 구현을 같은 C# 앱에서 사용하고, 미지원 플랫폼은 capability 확인과 예측 가능한 오류를 제공한다.
- [ ] 저장소 project reference 없이 패키지만 설치한 소비 앱에서 restore/build/run/publish, trimming과 목표 AOT 모드를 검증한다.

**완료 기준:** 별도 앱과 별도 플러그인 패키지에서 코드 복사 없이 기능을 사용할 수 있다. 권한 거절·취소·창 종료 중 응답·ABI 불일치가 검증된다.

### M5. OS Drag & Drop와 플랫폼 Navigation — P2

**선행:** M1/M2. 파일 데이터 계약은 M4와 공유한다. 두 기능은 서로 독립적으로 진행할 수 있다.

#### M5-A. OS Drag & Drop

- [ ] enter/over/leave/drop, 허용 action(copy/move/link), MIME/type, 취소, 논리 좌표 변환을 공통 계약으로 정의한다.
- [ ] Windows 파일/텍스트/URI 수신을 첫 장면으로 구현하고 AppKit/Qt/Web으로 확장한다. Web과 모바일의 제공 범위는 별도 capability로 표현한다.
- [ ] 전체 파일을 즉시 메모리에 올리지 않도록 stream/비동기 읽기와 접근 수명을 정의한다. 드롭된 경로나 데이터만으로 파일 실행을 수행하지 않는다.
- [ ] 이후 Doroti → OS 송신, drag 이미지, 창 밖 취소와 여러 창 사이 이동을 지원한다. move 성공·취소에 따른 원본 처리 책임을 명확히 한다.

**완료 기준:** Explorer/Finder/파일 관리자 또는 브라우저 파일 드롭으로 실제 파일을 전달할 수 있다. 여러 파일·큰 파일·취소·DPI 변경에서 내용과 좌표가 보존된다.

#### M5-B. Deep Link / lifecycle / restoration

- [ ] 공통 activation event에 URI·출처·cold/warm start를 전달하고 Router 준비 전 들어온 요청의 대기·중복 처리 규칙을 정한다.
- [ ] Web URL ↔ Router와 back/forward/새로고침, Android intent, iOS Universal Link, Windows protocol activation을 순차 연결한다. 나머지 desktop activation도 지원표로 추적한다.
- [ ] 기존 lifecycle 이벤트를 활용해 비활성화/복귀/종료 시 상태 저장 시점을 정한다. 강제 종료 후 복원과 정상 재시작을 구분한다.
- [ ] route·사용자 입력·선택 상태의 저장 형식과 version migration을 정하고, 잘못된 링크·오래된 상태·복원 실패의 fallback을 제공한다.

**완료 기준:** 종료된 앱과 실행 중 앱에 같은 링크를 전달해 목적 화면으로 이동한다. Web back/forward와 새로고침, 프로세스 재시작 후 상태 복원이 서로 충돌하지 않는다.

### M6. 실제 멀티윈도우 — P2, 구조 변경 위험 높음

**선행:** M2-A, M1의 격리·수명 테스트. 구현 전에 창 문맥 설계를 짧은 ADR로 고정한다.

- [ ] `WidgetsBinding`, `SchedulerBinding`, `ServicesBinding`의 static 상태와 root/view 구조를 조사해, 하나의 binding 아래 여러 view를 둘지 독립 실행 문맥이 필요한지 결정한다. 무조건 singleton을 전부 분리하지 않는다.
- [ ] 창별 focus, pointer/keyboard, scheduler target, Navigator/restoration, semantics, PlatformView owner, texture/GPU surface 소유권을 정의한다. 공유 가능한 cache와 창 종료 시 해제할 자원을 구분한다.
- [ ] 기존 `DorotiWindowManager.CreateWindowAsync`와 fresh `WindowContent` factory를 실제 Windows App SDK 추가 창 생성에 연결한다.
- [ ] 두 창의 독립 갱신·DPI·IME·렌더링·닫기와 `OnLastWindowClosed`/`Explicit` 수명을 검증한다. 닫힌 창으로 향하는 작업은 안전하게 종료한다.
- [ ] AppKit와 Qt에 확장하고 Catalyst는 scene 계약에 맞춰 별도 구현한다. owner/modal/satellite, popup/tooltip과 창 간 이동은 기본 추가 창 이후에 붙인다.
- [ ] `_window_*`의 Satellite API는 Desktop 소유권으로 연결하거나 지원 경계를 문서화한다. 두 구현이 같은 native 창을 중복 소유하지 않게 한다.

**완료 기준:** 실제 OS 창 두 개에서 서로 다른 입력·스크롤·DPI·native content가 동작하고, 한 창을 닫아도 다른 창이 유지된다. fake host 두 controller 생성은 이 단계 완료 근거가 아니다.

### M7. 외부 소비·배포와 플랫폼별 출시 기준 — P3

**선행:** 출시 대상 기능의 앞선 단계 완료. 모든 선택 기능 완료가 첫 배포의 필수 조건은 아니다.

- [ ] `dotnet new doroti-app`에서 package-only 앱을 만들고 Windows/Web/Android부터 clean 환경 설치·실행을 검증한다. 나머지 host를 순차 확대한다.
- [ ] SDK/runner/native library/font/Web asset/플러그인 버전의 호환 정책, API 변경 안내와 migration 문서를 정한다.
- [ ] Release, trimming, Mono AOT/NativeAOT/Web publish 중 실제 지원하는 조합을 명시하고 각 조합을 검증한다.
- [ ] 플랫폼별 서명·패키징·설치·업데이트·제거와 앱 데이터 유지, crash/로그 수집을 검증한다. 개발자 머신 성공을 clean 배포 성공으로 간주하지 않는다.
- [ ] 리소스 누수·장기 스크롤·창/PlatformView 반복 생성·GPU loss/앱 복귀 회귀를 릴리스 후보에서 확인한다.
- [ ] 제한과 미검증 항목이 포함된 출시 지원표, 실행 가능한 샘플, 문제 보고 양식을 함께 제공한다.

**완료 기준:** 선택한 플랫폼에서 저장소 소스와 개발자 로컬 cache 없이 앱을 설치·실행·업데이트할 수 있다. 배포 패키지와 검증 결과가 같은 revision·toolchain을 가리킨다.

## 4. 실행 순서와 첫 작업 묶음

기본 순서는 **M0 → M1 최소 구현 → M2 → M3 → M4/M5 → M6 → M7**이다. M1의 coverage는 이후 단계에서 필요한 만큼 늘린다. M2의 플랫폼 검증은 계속 유지하고, M3은 기본 호스트가 안정되는 대로 시작할 수 있다. M7의 package-only smoke는 M0부터 가볍게 시작해 마지막에 문제를 몰아 발견하지 않게 한다.

첫 작업 묶음은 다음 다섯 개로 제한한다.

1. `doroti.ps1`의 끊어진 validate/audit/release 경로와 실패 전파를 복구한다.
2. 문서 링크와 Desktop/PlatformView 지원표를 현재 코드에 맞춘다.
3. 최소 테스트 host로 `pumpWidget`·pointer tap·bounded settle을 만들고 Cupertino 탭 회귀 하나를 자동화한다.
4. Windows App SDK Desktop adapter를 붙여 Testbed 기본 runner에서 창 크기·제목·close를 시연한다.
5. 같은 runner에서 한글 조합 입력과 native TextBox/WebView focus 이동을 재현하는 장면 및 실측 결과를 남긴다.

M3-A/B의 VS Code 프로젝트 생성·실행 기능은 M0의 CLI 복구 후 독립적으로 진행하고, 앱의 C# 프로젝트 로딩이 준비되면 M3-D의 import·Widget Snippets를 연결할 수 있다. 위 묶음과 M1의 최소 회귀 경로가 준비되면 M3-C의 Hot Reload runtime·전용 버튼을 연결한다. 생성 마법사 → snippet 작성·import → 실행 → Hot Reload까지 통합 검증한 VSIX를 이번 확장 산출물로 삼는다. Preview·Inspector UI·전체 플랫폼 IDE 지원·별도 C# LSP·시각적 drag & drop 디자이너, 모든 플러그인 동시 구현, 전체 Flutter 테스트 일괄 이식, 무차별적인 `NotImplementedException` 제거는 초기 범위에서 제외한다.

## 5. 공통 완료 규칙

- 각 작업은 **공개 계약·호스트 연결·실행 가능한 샘플·필요한 회귀 검증·지원표 갱신**을 함께 완료한다. 문서 작성이나 컴파일 성공만으로 기능 구현을 완료 처리하지 않는다.
- 결과에 commit/revision, OS·기기·renderer, configuration/AOT, 명령, 기대값·실제값을 기록한다. 과거 결과는 날짜와 적용 범위를 표시한다.
- `PASS`는 실행한 범위에만 사용한다. 실행하지 않은 환경은 `notVerified`, 일부 성공은 `PARTIAL`, 의도적으로 제공하지 않는 기능은 `unsupported`로 남긴다.
- 테스트는 저장소 지침대로 **20분 timeout**을 사용한다. 일반 반복 검증은 30회 이내에서 설계하고, 별도 장기 시험이 필요하면 목적·종료 조건을 정한다.
- 일회성 테스트 스크립트·프로젝트와 테스트 원시 로그·캡처·빌드 산출물은 저장소 루트 `temp/testing/<작업-ID>/<실행-ID>/`에 모으고, 결과 요약 후 삭제한다. 최소 상시 회귀 소스·작은 fixture만 `Doroti/tests/`에 선별해 남긴다. 일반 제품 빌드 산출물과 기존 `Doroti/artifacts`는 기존 정리 지침을 따른다.
- 임시 테스트는 기본 빌드·watch·패키징·전체 CI에서 제외한다. 실패 조사로 임시 파일을 남길 때는 보류 사유·정리 시점을 기록하고, 삭제 후에는 원시 파일이 남아 있다고 보고하지 않는다. 검증 파일의 영구 보존 없이도 결과 요약과 재현 절차로 필요한 검증을 기록할 수 있다.
- Flutter는 동작·렌더링·입력 모델의 비교 기준으로 사용한다. C#에서 유지보수하는 제품 소스를 기본으로 삼고, compiler는 필요한 import/의미 비교에 사용한다.

## 6. 이번 검토의 주요 근거

아래 파일은 현황 판단의 근거다. 문서에 기록된 과거 실행 결과를 이번 검토에서 재실행한 것으로 해석하지 않는다.

- [이전 검토](ref1.md), [프로젝트 소개](README.md), [프레임워크 구조와 실행 안내](Doroti/README.md)
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
