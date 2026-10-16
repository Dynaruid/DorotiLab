# Doroti VS Code 편집 지원 작업 요약

원문: 루트 `work.md` · 계획 작성: **2026-10-06** · 구현·검증: **2026-10-07** · 요약 보관: **2026-10-16 KST**.
사용자가 지정한 이력 경로 `history/26-10-07`에 보관한다.

원문의 최종 상태는 **E0–E6 / C1–C5 PASS**, `wholePlanComplete=true`이며 **Windows 편집 지원 범위**에 한정한다. 체크 항목 50개 완료, 미완료 0개다. 이 문서는 당시 결과를 보존하는 요약이며 이번 보관 과정에서 제품 검증을 다시 실행한 결과가 아니다.

대상은 확장 0.2.0과 `tools/Doroti.Editor.Assist`다. 상세 검증 문서와 evidence JSON은 2026-10-16에 삭제했다. 아래 내용은 당시 결과를 옮긴 요약이며 원본 증거·코드·패키지 해시 전체를 대체하지 않는다. 현재 사용법과 검사 명령은 [확장 README](../../tools/vscode-doroti/README.md)를 따른다.

## 목표와 범위

Doroti C# 작성 시 짧은 prefix로 위젯 클래스를 만들고, `Ctrl+.` 또는 Refactor에서 위젯 감싸기·추출·변환을 수행하도록 확장했다. 생성자 인자·builder·State 생명주기와 실제 참조 package에 맞춘 제안을 제공한다.

기존 Create Project, 프로젝트/target 선택, Run/Stop, Hot Reload, 로그 기능은 유지했다. 기본 IntelliSense·진단·정의 이동은 Microsoft C# 확장과 함께 사용한다. Preview/Webview/Inspector, 전체 C# 언어 서버 교체, Marketplace 공개, 동시 다중 실행은 완료 범위 밖이다.

## 단계별 구현

| 단계 | 완료한 내용 |
| --- | --- |
| E0 — 편집 분리·프로젝트 판별 | `src/editing/`으로 provider를 분리하고 실행 session과 독립된 lifecycle을 구성했다. 문서 소유 앱/참조 package, 편집 context key와 설정, 미저장 버퍼 우선 처리, global using·프로젝트 캐시 무효화를 구현했다. 관련 없는 C# 문서에는 Doroti 제안을 노출하지 않는다. |
| E1 — 생성·snippet | Stateless `stl`/`stless`/`dstateless`, Stateful `stf`/`stful`/`dstateful` 별칭과 이름 연동·Tab·import를 제공했다. New Stateless/Stateful Widget 및 Insert Widget Snippet 명령, 파일명·namespace·충돌·취소 처리와 기본 위젯·State 자원 패턴을 추가했다. |
| E2 — 분석 helper·catalog | Roslyn 기반 snapshot 분석, 실제 Widget type identity·constructor/delegate·참조 package catalog를 구현했다. versioned JSON/typed 계약, handshake, 취소, 메시지·캐시 제한, process 소유·재시작·dispose를 처리했다. 참조 해석 실패 시 안전한 템플릿으로 제한한다. |
| E3 — 감싸기·제거 | 단일 Widget 식의 커서/선택과 연속 children 항목을 지원한다. Center/Padding/Container/SizedBox/Align/Row/Column/Stack/Builder/LayoutBuilder/SingleChildScrollView를 구성하고, Expanded/Flexible/Positioned는 부모 제약을 확인한다. wrapper 제거 영향 preview, 주석·쉼표·spread 보존, stale 결과 거부와 Undo/Redo를 처리했다. |
| E4 — 문맥 제안·import | 미입력 생성자 인자, child/children/builder, State override·setState, enum/factory를 실제 API로 제안한다. alias·local/project type·global/csproj/generated using을 반영하고 C# 확장과의 중복, 미참조 package, restore 실패를 구분한다. `CustomCarousel`은 참조 앱의 실제 signature를 따른다. |
| E5 — 구조 리팩터링 | 같은 파일의 새 StatelessWidget으로 추출하고 명시적 값/callback 입력을 전달한다. Stateless → Stateful은 nested State로 constructor/key/private 접근/프로퍼티/primary 값/build 시점 capture를 보존한다. 실제 생성자 계약이 허용하는 child/children·wrapper 변환과 preview Apply/Cancel/Undo를 구현했다. |
| E6 — 설치형 검증·문서 | `editingHost.ts`와 `runHost.ts --editing`을 추가했다. VSIX 편집 흐름, C# 공존/부재, Restricted Mode, 외부 생성 앱, 생성 코드·Element 조립, catalog/helper 의존성 패키징과 영향 있는 실행 계약을 검증하고 README·증거를 정리했다. |

진행 순서는 E0 → E1 → E2 → E3 → E4 → E5 → E6이었다. 체크포인트 C1 생성, C2 위젯 구성, C3 문맥 작성, C4 구조 편집, C5 설치형 완료를 모두 통과한 것으로 기록했다.

## 구현 계약과 지원 제한

- C# 구조 변환은 문자열 치환이 아닌 Roslyn 구문·semantic model과 canonical Framework Widget identity를 사용한다. 전체 식 또는 동일 collection의 연속 항목만 변환한다.
- helper 요청은 프로젝트 identity, URI, 문서 version, UTF-16 범위, 미저장 snapshot과 request ID를 포함한다. 문서·peer snapshot generation이 달라진 결과는 재계산하거나 거부한다.
- helper는 trust가 있을 때만 실행한다. Restricted Mode에서는 순수 템플릿·문서 편집을 제공하고 불확실한 타입은 `global::`로 한정한다. 제안 때문에 restore/build·dependency 설치를 자동 수행하지 않는다.
- BOM/LF/CRLF, 들여쓰기, namespace, 주석, alias를 보존하고 import와 wrapping을 공통 편집/Undo 경로로 적용한다. 미저장 파일을 자동 저장하거나 편집 후 Run/Hot Reload를 자동 실행하지 않는다.
- 추출은 순수 생성식과 명시적 값/callback 입력을 지원한다. partial/복잡한 inheritance/base dispatch/mutable capture/미해석 참조와 안전하지 않은 inline closure·부작용 호출·외부 computed member/private type은 제한 이유를 제공한다. 전체 C# 또는 모든 Widget 패턴 지원을 의미하지 않는다.
- 타입·클래스 구조 변경에는 Restart가 필요할 수 있다. Stateful → Stateless, Swap with Parent, 색상 미리보기·닫힘 라벨·위젯 outline, Doctor·장치 선택·F5 개선은 별도 후속 범위다.

편집 provider는 [src/editing](../../tools/vscode-doroti/src/editing)에 있고, helper는 [Doroti.Editor.Assist](../../tools/Doroti.Editor.Assist)의 `Contracts.cs`, `Engine.cs`, `Refactorings.cs`, `Program.cs`로 구성했다. 원래 계획의 contracts/analysis/refactorings 폴더 책임을 해당 파일들로 합쳤다. `src/imports.ts`는 기존 호출을 위한 compatibility export로 남겼다.

## 기록된 검증 결과

환경은 Windows x64, .NET SDK 10.0.400, C# 14/Roslyn 5.9.0, MSBuild Locator 1.11.2, Node 24.15.0, npm 11.17.0, TypeScript 7.0.2였다. 기반 revision은 `8a7a67a8e232dd40d7883b12214b4e2feef738e3`이며 당시 구현은 그 위의 작업 트리 변경이었다.

| 검사 | 당시 결과 |
| --- | --- |
| 계약·생성 코드 | **PASS** — Node 계약 12개, Roslyn/API/Element 검사 84개, 템플릿 38개 컴파일 |
| helper snapshot/protocol | **PASS** — 8개 검사; handshake, UTF-16/한글 경로, 미저장 peer 문서, snapshot 복원, 제한·취소·평가 실패 fallback. 20회 연속 요청에 프로젝트 load 1회 |
| 설치한 VSIX | **PASS** — VS Code 1.140.0 + Microsoft C# 2.160.4, 최소 지원 VS Code 1.100.0 + C# 부재에서 prefix 입력·선택·이름·Tab·import·wrapping·추출·변환·Undo 경로 |
| Restricted Mode | **PASS** — 실제 `isTrusted=false`, helper 미실행, qualified template와 관련 없는 프로젝트 제외 |
| repository 밖 생성 앱 | **PASS** — `dotnet new doroti-app`, cached App SDK와 앱 내부 binary references로 편집·컴파일. NuGet-only 배포 인수와 구분 |
| 패키징·Widget 조립 | **PASS** — catalog/editing JS/helper runtime·Roslyn·BuildHost 포함, CPU Element mount/layout/해제 및 parent data·입력·State build |
| 기존 실행 기능 영향 | **PASS** — process/metadata/Web 계약 회귀, development 계약 fixture의 Run 중 편집·Hot Reload ack/runtime identity/Stop. 실제 native metadata delta 증거는 아님 |

준비된 provider 경로를 각각 20회 측정했다. completion/code action p95는 C# 공존에서 **8.98/9.56ms**, VS Code 1.100.0·C# 부재에서 **1.28/1.15ms**, 외부 생성 앱·C# 부재에서 **2.63/1.77ms**였다. 목표 100/200ms를 통과했으며 최초 helper/project load와 큰 변환 실행 시간은 제외했다.

당시 검증 명령은 각 실행에 20분 timeout을 적용했다. 대표 명령은 다음과 같다. 상세 검증 기록은 삭제되었으므로 현재 helper·설치형 검사 명령은 확장 README를 참고한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti test
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti run package
```

## 미검증 범위와 증거 보존

새로운 네이티브 Run/Hot Reload 제품 인수, macOS/Linux 실행, 물리 키보드·포인터·한글 IME·native Save dialog, 접근성, display scanout/GPU/FPS, signing/clean-machine 설치, NuGet-only/Marketplace 배포는 **notVerified 또는 당시 범위 제외**다. Windows command automation과 CPU Widget 조립 PASS를 이 항목의 PASS로 확대하지 않는다. 기존 Carousel·`.vscode` 소스는 원문 작업에서 수정하지 않았다고 기록했다.

`temp/testing/vscode-editing/`, `%TEMP%/Doroti-vscode-editing-20261007`, `Doroti/artifacts` 등의 원시 로그·profile·앱·패키지는 삭제 가능한 로컬 산출물이다. 이 요약은 해당 파일의 현재 존재를 보증하지 않는다. 상세 검증 문서와 evidence JSON도 삭제되었으며, 이 문서에 옮긴 당시 결과만 남아 있다.

보관 직전 원문 `work.md`의 SHA-256은 `ea09f22b9c1fbd63b7bf66a2693df228ab27230e70ab12602a3e3fd87566a050`으로 기록했다. 삭제된 evidence JSON의 `sourceHashes["work.md"]`는 당시 검증 입력 해시였으며 이 보관 원문 해시와는 별개다. 루트 `work.md`도 삭제되어 현재는 이 요약을 참조한다.
