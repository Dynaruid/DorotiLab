# M3. VS Code 실행 확장·작성 지원·Hot Reload

원문: [개발 로드맵](../../plan.md) §3 M3 · 우선순위: **P1** · 작업 상태: **PARTIAL** · 실행 검증: 로컬 VSIX/Windows·Web metadata reload **PASS**, 마법사 전체 UI·모든 예외 조합 **notVerified**

[작업 인덱스와 공통 완료 규칙](../README.md)

## 담당 범위와 선행 작업

확장·snippet/import·reload runtime과 세션 계약을 담당한다. 첫 실행 대상은 로컬 Windows App SDK와 Web이며 전체 플랫폼 IDE 지원은 후속이다.

선행: M3-A/B는 [M0](00-foundation.md) 후 시작한다. M3-D는 생성 앱의 C# 프로젝트 로딩, M3-C는 [M1](01-testing.md)과 개발 호스트·실행 세션 관리에 의존한다.

플랫폼별 연결·검증: [Windows](../platforms/windows.md) · [Web](../platforms/web.md)

## 원문 작업과 완료 기준

원문의 범위와 완료 기준을 유지하면서 2026-09-28~29 실행 기록에 따라 완료 부분과 잔여 부분을 분리했다. 체크된 항목은 명시한 호스트·검증 범위에 한정하며, 아래 날짜별 결과가 근거다. 공통 구현 완료가 모든 플랫폼 검증 완료를 뜻하지 않는다.

**현재 범위:** **새 프로젝트 생성 마법사 → Widget Snippets·import 지원으로 코드 작성 → 프로젝트·target 선택 → 실행 → 확장 전용 Hot Reload 버튼 → 로그 확인·중지**를 이번 확장 범위로 구현한다. 여기서 import는 C#의 `using` 자동 추가·누락 해결을 뜻한다. Preview는 후속 검토로 보류한다. 생성·실행 기능은 먼저 개발할 수 있지만, 이번 VSIX의 완료 조건에는 M3-C의 Hot Reload와 M3-D의 작성 지원까지 포함한다.

### M3-A. AvaloniaVSCode 구조 검토와 최소 설계

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

구현 배치는 `Doroti/tools/vscode-doroti/`다. `src/extension.ts`는 명령/편집 등록, `contracts.ts`·`processes.ts`·`imports.ts`는 세션/실행/작성 보조, `src/test`는 단위·설치 VSIX Extension Host 검증을 담당한다. 아래의 초기 제안 디렉터리는 책임 구분을 보존한 참고 구조다.

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

- [x] archive 의존성 버전을 그대로 복사하지 않고 착수 시 지원 VS Code/Node 조합을 정한다. 패키지 관리자는 하나로 정하고 lockfile 및 빌드·테스트·VSIX 생성 명령을 고정한다.
- [x] VS Code 의존성을 framework에 추가하지 않는다. 별도 .NET 보조 서버·protocol·복잡한 plugin 계층은 실제 필요가 생길 때 도입한다.

### M3-B. 프로젝트 생성 마법사·선택·실행·로그

**선행:** M0의 CLI 실행 경로·실패 전파와 사용 가능한 Doroti 템플릿. 이 단계의 개발은 M1·Hot Reload 완료 전 시작할 수 있으며, 최종 VSIX 검증은 M3-C/D와 통합한다.

- [x] `Doroti: Create Project`, `Select Project`, `Select Target`, `Run`, `Hot Reload`, `Stop`, `Show Logs`와 선택 상태 표시를 구현한다. 별도 Build/Doctor 화면은 만들지 않는다.
- [x] 새 프로젝트 생성 마법사에서 프로젝트 이름·생성 위치를 입력받고 기존 `dotnet new doroti-app` 템플릿을 호출한다. 현재 템플릿의 기본값을 사용하며, 첫 실행 target 선택은 생성 후 manifest에 선언된 항목에서 제공한다. 생성할 플랫폼을 줄이는 새 템플릿 옵션은 이번 필수 범위에 넣지 않는다.
- [x] 생성 마법사의 이름/폴더 충돌·취소·오류 안내를 구현했다. 경로/이름과 프로세스 처리의 단위 회귀는 아래 결과를 따른다.
- [ ] 실제 마법사 UI에서 SDK·템플릿 누락 안내, 잘못된 이름·비어 있지 않은 폴더·입력/생성 중 취소·생성 실패를 검증한다. 기존 파일 미덮어쓰기·실패/취소 후 남은 경로 안내·성공 후 폴더 열기와 앱 선택까지 확인한다.
- [x] `doroti-workspace.json`의 `applicationProject`/`platforms`에서 앱과 runner를 찾는다. 여러 후보가 있으면 선택하게 하고 첫 `.sln`을 임의 실행하지 않는다. 첫 버전은 선택한 앱 하나·실행 세션 하나만 지원하며 multi-root와 동시 다중 앱 실행은 후속으로 둔다.
- [x] CLI의 모든 플랫폼 alias 요구를 해제해 일부 플랫폼만 선언한 앱도 허용한다. 확장에는 선언된 target만 표시하고 runner 선택 로직을 중복 구현하지 않는다.
- [x] 기존 `Doroti/eng/doroti.ps1` 호출을 재사용하고 CLI 경로를 설정 가능하게 한다. 첫 버전은 저장소 CLI와 설치된 템플릿 사용을 명시하며, 마법사가 만든 앱도 이 구성으로 실행 검증한다. 독립 CLI 배포는 M7로 넘긴다. device discovery·범용 debug protocol은 제외하되 Hot Reload 요청·결과 전달에 필요한 최소 계약은 M3-C에서 구현한다.
- [x] 첫 대상은 로컬 Windows App SDK와 Web으로 잡고 개발 실행에는 `Debug`를 명시한다(현재 CLI 기본값은 `Release`). Windows는 실제 앱 창, Web은 외부 브라우저로 실행한다. Run에서 기존 build/run 흐름과 출력을 사용하고 추가 UI는 필요한 최소 상태만 표시한다.
- [x] 실행 오류 안내·단일 세션·프로세스 트리 정리를 구현하고 설치 VSIX의 Run/중복 Run/Stop, 단위 테스트의 실패 전파·자식 프로세스 종료를 확인했다.
- [ ] PowerShell/.NET SDK/선택 target 도구 누락·build/실행 실패의 실제 UI, 실행 중 target 변경·Stop/확장 종료 경합을 검증한다. build와 앱 실행 사이 프로세스 소유권·잔여 프로세스가 없는지 확인하고 필요한 CLI 계약을 보완한다.
- [x] C# 완성·오류 표시·디버깅은 기존 C# 확장과 사용자 설정을 활용하고 Doroti import·Widget Snippets는 M3-D에서 연결한다. `launch.json`/`tasks.json` 자동 생성, 자체 LSP/debug adapter, Inspector UI는 추가하지 않는다.
- [x] Restricted Mode에서는 템플릿 실행·앱 실행·Hot Reload를 비활성화하고 workspace trust 변경을 반영한다. 실행 파일/인자는 배열로 전달하며 프로젝트 경로를 shell 문자열에 이어 붙이지 않는다. [Workspace Trust 공식 지침](https://code.visualstudio.com/api/extension-guides/workspace-trust)을 적용한다.
- [x] clean profile의 설치 VSIX와 생성 앱에서 Windows/Web Run·metadata Hot Reload·Stop을 통합 검증했다(2026-09-28). 마법사 전체 UI 및 실제 C# 서비스는 이 PASS에 포함하지 않는다.
- [ ] [VS Code 확장 테스트 지침](https://code.visualstudio.com/api/working-with-extensions/testing-extension)에 따라 마법사·앱 선택·실패 전파·restricted profile의 남은 실제 UI 조합과 최신 변경 후 통합 흐름을 검증한다. 테스트는 20분 timeout을 적용한다.

**완료 기준:** 마법사로 새 앱 생성 → 폴더 열기 → 앱/target 선택 → 실행 → 로그 확인 → 중지를 재현하고 기존 앱도 동일하게 실행한다. 한글/공백 경로·생성 취소·폴더 충돌·잘못된 manifest·누락 SDK/템플릿·build 실패·중복 실행·VS Code 종료를 처리하며 소유한 실행 프로세스가 남지 않는다. 이번 VSIX 완료에는 아래 M3-C/D 통합 검증도 필요하다.

### M3-C. Hot Reload runtime과 확장 전용 버튼 — 이번 구현 범위

**선행:** M1의 최소 회귀 경로와 검증 가능한 개발 호스트. 버튼 통합에는 M3-B의 실행 세션 관리가 필요하다.

- [x] .NET metadata update 지원 환경을 확정하고 update hook에서 소유 UI 문맥으로 `reassembleApplication()`을 요청한다. pending frame과 중복 reload를 직렬화한다.
- [x] Windows와 Web main-owned threaded Debug의 실제 metadata update에서 State·입력·스크롤 보존, 컴파일 오류 후 수정·재시도, 중복 요청 직렬화를 확인했다(2026-09-28). native ENC0009는 재시작 필요로 표시하고 기존 상태를 유지한다.
- [ ] rude edit 후 명시적 Restart·종료 경합과 2026-09-29 창 문맥 변경 이후 설치 VSIX의 실제 metadata 흐름을 재검증한다. 미지원 편집을 되돌린 경우의 Restart 경로와 단순 컴파일 오류 재시도를 구분한다.
- [x] 먼저 CLI/개발 호스트에서 코드 변경의 metadata update → UI 갱신을 검증하고, 같은 경로를 `Doroti: Hot Reload` 명령과 실행 중 표시되는 전용 버튼에 연결한다. 버튼은 저장되지 않은 변경의 저장/취소를 처리한 뒤 코드 갱신을 요청한다. `reassembleApplication()`만 호출해 변경 코드가 적용된 것으로 처리하지 않는다.
- [x] 현재 실행 세션의 지원 capability·요청 id·처리 중/성공/실패/재시작 필요 결과를 전달하는 최소 계약을 정한다. 기존 CLI 제어 경로를 우선 활용하고 부족한 연결만 추가한다. 연속 클릭과 저장 기반 reload가 중복 적용되지 않도록 직렬화하며 종료한 세션의 늦은 응답을 무시한다.
- [x] 실행 전·Release·미지원 host에서는 버튼을 비활성화하고 이유를 표시한다. 코드 컴파일 오류는 로그로 연결하고 수정 후 다시 시도할 수 있게 한다. 재시작이 필요한 변경은 상태 초기화를 알리는 Restart 동작으로 연결한다.

**완료 기준:** 설치한 VSIX에서 마법사로 만든 앱 실행 → 코드 수정 → Hot Reload 버튼 클릭 → State·입력·스크롤을 보존한 UI 갱신을 재현한다. 컴파일 오류 후 재시도·연속 클릭·재시작 필요·실행 종료를 검증한다. Windows/Web의 지원 여부는 각각 기록하고 최소 한 개발 호스트에서 실제 갱신을 통과해야 이번 확장을 완료 처리한다. runtime 갱신 경로는 VS Code 밖에서도 사용할 수 있게 유지한다.

### M3-D. import 지원·Widget Snippets — 이번 구현 범위

**선행:** 실제 Doroti C# API와 M3-B에서 생성한 앱의 정상적인 C# 프로젝트 로딩. Hot Reload 구현 전 독립적으로 개발할 수 있다.

- [x] curated Doroti 타입의 import 완성·Quick Fix를 구현하고 설치 VSIX의 editor API로 using 추가·Undo를 확인했다. Quick Fix 진단은 테스트가 공급했으며 실제 C# 서비스의 프로젝트 로딩 증거는 아니다.
- [ ] 실제 C# 언어 서비스의 완성·code action과 함께 auto-import/Quick Fix를 검증하고 지원 확장·버전·설정을 명시한다. 프로젝트 로딩 실패·참조 누락·언어 서비스 미설치를 구분해 안내한다. 기본 편집 기능은 [VS Code C# 안내](https://code.visualstudio.com/docs/languages/csharp)를 기준으로 검토한다.
- [ ] 중복 `using`, `global using`, alias, file-scoped/block namespace, 같은 이름의 다른 타입을 처리한다. 여러 namespace가 가능한 경우 사용자 선택을 제공하고 일괄 namespace 추가로 모호성을 만들지 않는다. 기존 C# 기능으로 충족되지 않는 Doroti 전용 import 보조만 추가하며 전체 C# 언어 서버는 만들지 않는다.
- [x] `snippets/widgets.json`을 `csharp` 언어용으로 등록한다. 첫 목록은 StatelessWidget, StatefulWidget + `State<T>`, `build`, `initState`/`dispose`/`setState`, Row/Column/Container/Text, Material/Cupertino 앱 뼈대로 정한다. prefix는 `dstateless`, `dstateful`, `drow`처럼 Doroti용으로 구분하고 클래스명·child/children·본문에 연결 placeholder와 최종 cursor 위치를 제공한다. [Snippet 등록 지침](https://code.visualstudio.com/api/language-extensions/snippet-guide)을 따른다.
- [x] snippet의 생성자·override 이름·반환형·namespace·수명 처리는 현재 C# 제품 API와 샘플에 맞춘다. Dart 문법이나 Avalonia 코드를 그대로 복사하지 않는다. 필요한 import를 각 snippet의 metadata/설명에 명시하고, 삽입 후 누락 import를 해결할 수 있게 연결한다. 정적 snippet JSON만으로 자동 `using` 편집이 완료된다고 가정하지 않고 필요한 삽입 보조·code action 경로를 함께 검증한다.
- [x] 12개 snippet을 실제 JSON에서 확장한 최소 사용 예제로 Debug build를 통과했다. StatefulWidget의 연결 이름·Tab 및 import·Undo는 설치 VSIX의 editor API로 확인했다.
- [ ] 모든 snippet의 새/기존 파일 prefix 완성·삽입·Tab·연결 이름·Undo·import 조합, 중복 import·타입 충돌·기존 C# snippet 충돌을 실제 에디터에서 확인한다. 테스트는 공통 20분 timeout을 적용한다.

**완료 기준:** 설치한 VSIX에서 마법사로 앱 생성 → StatefulWidget snippet 삽입 → 이름/본문 편집 → 필요한 `using` 추가 → build/run → 코드 수정 후 Hot Reload를 재현한다. 일반 타입 완성 시 auto-import와 미해결 타입의 Quick Fix도 각각 검증하며, snippet 텍스트 삽입만 성공한 상태를 작성 지원 완료로 간주하지 않는다.

### 후속 후보 — 현재 구현·완료 기준에서 제외

- **Preview 전체:** `WidgetPreviews` 실행 host, preview 등록/검색, Webview, 크기·테마·locale 제어. 현재는 별도 구현 계획을 확정하지 않는다.
- **Inspector/DevTools UI:** 트리 탐색·highlight·소스 이동·성능 timeline·원격 진단 endpoint. 디버깅 필요가 구체화되면 범위를 다시 정한다.
- **IDE 편의 기능:** CodeLens, `launch.json`/`tasks.json` 자동 생성.
- **지원 범위 확대:** multi-root·동시 다중 앱, Android 기기 자동 검색·로그 통합, macOS/Linux 검증, WSL/SSH/Dev Container 및 browser-only VS Code.
- **공개 배포:** 독립 CLI 제공, package-only 앱 통합 검증, Marketplace 공개는 M7에서 다룬다. 첫 산출물은 로컬 설치 가능한 VSIX다.

## 산출물과 결과 기록

- 공개 계약·구현 변경, 실행 가능한 샘플, 필요한 회귀 검증, 지원표 갱신을 함께 남긴다.
- 이 문서의 완료 기준과 플랫폼별 적용 결과를 연결하고, 미실행 조합은 `notVerified`로 유지한다.
- 결과는 [공통 기록 형식](../README.md#결과-기록-형식)에 revision·환경·명령·기대값·실제값·남은 작업을 남긴다.

### 2026-09-28 구현·검증

revision: `a93c047fe2e93d93cff3e0a6bf3c2789862fea81` + working tree. Windows 10.0.26200, .NET 10.0.400 Debug, VS Code 1.139.1, Node 24.15.0, npm 11.17.0. npm lockfile/VSIX 빌드를 고정했고 archive 코드는 복사하지 않았다. 확장 license는 저장소 BSD-3-Clause다.

- [로컬 확장](../../Doroti/tools/vscode-doroti/README.md)에 생성·앱/target 선택·Run·Hot Reload·Restart·Stop·Logs, 상태/전용 버튼, trust gate, 단일 세션, argv 배열·프로세스 트리 정리, 이름/폴더 충돌/취소/오류 안내를 구현했다. C# snippet 12개와 curated import 완성/Quick Fix를 제공한다. 별도 LSP/preview 서버는 없다.
- [개발 세션 계약](../../Doroti/docs/development-hot-reload.md): `describe`는 CLI가 해석한 manifest JSON, `dev`는 명시적 Debug Windows/Web 실행이다. 일부 alias만 선언한 manifest를 허용한다. Windows `dotnet watch`의 실제 metadata callback → 소유 UI queue → 직렬 reassemble → frame 완료 후 revision/요청 ID ack로 연결했다. save/no-op만으로 성공 처리하지 않는다. Web Hot Reload도 후속 구현·검증을 통과했다(아래 기록).
- 기존 reassemble의 무조건 예외와 빠진 element-tree reassemble을 수정했다. 종료 중 새 등록이 생기지 않도록 막고 pending registration을 취소한다. watcher가 export한 MSBuild 경로 때문에 C++ toolset을 못 찾던 문제도 native build 진입점에서 수정했다.
- 설치한 `dotnet new doroti-app`을 **한글·공백 경로**에 생성했다. 검증 fixture는 명시적으로 현재 저장소 프로젝트/SDK와 target build props에 연결했다. 원본 package-only 배포의 재검증은 M7이며 이 결과로 대체하지 않는다.
- 실제 Windows App SDK / Graphite Vulkan + native D3D12 호스트에서 `Before reload`→`After metadata reload` 변경을 확인했다. 동일 PID=37452, 동일 State ID, count=5, 입력=`한글 유지`, scroll=160, reassemble=0→1, runtime revision=1/status=applied. 값은 자동 seed이며 물리 입력은 주장하지 않는다.
- 별도 clean profile에 VSIX를 설치하고 실제 VS Code editor API로 StatefulWidget snippet 연결 이름/Tab, import 완성·Undo·CS0246 Quick Fix, 생성 앱 Run/중복 Run/Hot Reload/Stop을 통과했다. Quick Fix 진단은 테스트가 공급했으며 Microsoft C# 언어 서비스의 실제 프로젝트 로딩 결과와 혼동하지 않는다. 테스트는 설치된 VSIX를 사용하며 development-source 확장으로 대체하지 않는다.
- 생성 앱 Web Debug 빌드·CLI dev 실행, Chrome에서 실제 버튼 클릭/한글 표시/resize를 확인했다. 후속 설치 VSIX 검증에서 Web Run의 외부 브라우저 열기·연결도 통과했다.

명령은 위 개발 세션 문서에 보존했다. `npm test`는 경로/이름/import/watcher 분류/argv 보존/실패 전파/자식 프로세스 종료 **7개 PASS**. 로컬 산출물은 `Doroti/tools/vscode-doroti/doroti-0.1.0.vsix`이며 `npm run package`로 재생성한다. 설치 테스트 결과와 raw 로그는 `temp/testing/m4-m5/`의 disposable evidence다.

남은 검증: 생성 마법사의 실제 폴더 dialog·취소·충돌·도구 누락을 포함한 전 UI 흐름, 실제 C# 확장 버전과 프로젝트 로딩/타입 충돌 조합, 모든 snippet의 편집 제스처 조합, rude edit 후 Restart·종료 경합, 확장 종료 중 생성 작업 취소. 필수 구현과 통과한 경로를 제공하되 이 미검증 항목 때문에 M3 전체 완료로 표시하지 않는다. Workspace Trust는 등록 enablement와 실행 시 guard 양쪽에 적용되며 restricted-profile 실제 실행 시험은 아직 **notVerified**다.

추가 최종 검증: 설치 VSIX에서 컴파일 오류 시 revision 불변, 수정 후 동일 runtime/State로 재시도, 동시에 들어온 두 reload 명령 직렬화까지 **PASS** (`vscode-final/result.json`). 12개 snippet을 실제 JSON에서 확장한 `SnippetCompilation.cs`를 생성 앱에 포함해 Debug build **0 warnings / 0 errors**로 확인했다. 모든 snippet의 개별 에디터 제스처 조합까지 검증한 것은 아니다.

native `dotnet watch`에서 field `int`→`long` 변경의 ENC0009/restart 필요를 확인했고, 자동 재시작 없이 이전 process/State/표시값이 유지됨을 검증했다. piped watcher의 console prompt는 stdin 응답을 받지 않으므로 확장은 대기 상태를 유지하며 명시적 Restart로 프로세스 트리를 교체한다. 미지원 편집을 되돌린 경우에도 이 경로에서는 Restart를 사용한다. 단순 컴파일 오류의 수정·재시도와는 구분한다.

### Web Hot Reload 후속 구현·검증 (2026-09-28)

사용자 요청에 따라 Web도 `dev`의 `dotnet watch` 경로에 연결했다. .NET SDK 10.0.400 / runtime 10.0.11, Windows의 실제 Chrome, main-owned threaded WASM을 검증했다. 렌더링은 **worker-direct-webgl / worker-direct-webgpu 모두 PASS**. Release/AOT·명시적 worker-owned runtime·물리 모바일·다른 브라우저는 이 결과에 포함하지 않는다.

- SDK browser-refresh/metadata agent를 그대로 사용한다. .NET 10 threaded runtime의 기본 sync JSExport 차단과 SDK agent의 sync 진입점 충돌을 개발 모드 `ThrowWhenBlockingWait`로 해결했다. SDK script가 없는 정상 startup은 기존 정책을 유지하며, 동기 blocking wait는 계속 예외로 차단한다.
- Web의 JS synchronization context를 보존한다. Mono metadata 갱신 후 EditableText callback delegate identity가 달라져 assert/ErrorWidget/overflow가 발생하던 문제를 기존 layer callback 해제·새 callback 연결로 수정했다. 포커스된 입력·selection reassemble CPU 회귀도 추가했다.
- framework frame 오류를 관찰해 `failed`로 반환하고 revision을 올리지 않는다. browser status/request는 공통 managed 메모리에 보관하며 async JS export로 조회한다. VSIX의 loopback bridge는 nonce/origin/session을 검증하고 **브라우저 request 준비 응답 → 파일 저장 → 실제 metadata update → 성공 frame 응답** 순서를 지킨다. 코드/delta는 bridge로 전달하지 않는다.
- 실제 화면: `Before reload` → `Web reload verified`, counter=1, 한글=`한글 상태 유지`, 첫 표시 Row=4 유지. runtimeId=`094baa88-1434-477c-9fd1-77c7efbd5241`, revision=0→1, status=applied. 새로고침 없이 화면/DOM 모두 확인했다.
- clean profile의 설치 VSIX: Run이 연 Chrome에 연결, Hot Reload 명령으로 `Web VSIX Hot Reload passed` 표시, counter=1 / `VSIX 한글 유지` / Row 4 유지. runtimeId=`ff6126d3-b7ae-41e1-b012-abe38217a3d9` 불변, 요청 ID와 revision=1 응답 일치. 컴파일 오류에서 revision 불변, 수정·중복 요청 후 revision=2 및 같은 상태로 복구. 후속 저장 갱신도 revision=3으로 반영됐다. Stop까지 테스트 host exit=0.
- 별도 WebGPU 페이지: `WebGPU reload verified` 표시, counter=1 / `WebGPU 한글 유지` / Row 4 유지. runtimeId=`6fa85417-5019-4b08-b442-3878cedd8580` 불변, revision=0→1, applied. 브라우저 자동 입력이며 물리 IME 조합이나 성능 계측은 아니다.

재현: [개발 세션 문서](../../Doroti/docs/development-hot-reload.md), `runHost.js <fixture> <evidence> --web`. 테스트 파일 게이트는 브라우저 도구의 실제 입력/화면 검사와 확장 명령을 연결한다. raw 로그·결과는 `temp/testing/web-reload/`의 disposable evidence이며 지속되는 요약은 본 문서다. npm 단위 테스트 8개, Web 정책/bridge 파서 테스트 4개와 focused input 포함 widget 회귀가 통과했다.

2026-09-29: 창 문맥 분리 후 두 owner-thread CPU runtime을 동시에 reassemble하고
각 binding·focus·한글 값·semantics·종료가 독립적으로 유지되는 회귀를 추가해 통과했다.
이번 세션에는 Windows UI/브라우저 연결이 없어 설치 VSIX의 실제 metadata update 흐름을
재실행하지 않았다. 위 2026-09-28 결과와 이번 CPU 회귀의 증거 범위를 구분한다.
