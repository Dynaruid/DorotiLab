# Doroti 편집 지원 검증 — 2026-10-07

대상은 `work.md`의 E0–E6와 확장 0.2.0이다. 기반 revision은 `8a7a67a8e232dd40d7883b12214b4e2feef738e3`이며 구현은 그 위의 작업 트리 변경이다. 완료 판정은 Windows 편집 지원 범위다. 증거 JSON과 최종 코드/패키지 SHA-256은 [evidence](evidence/2026-10-07/verification.json)에 보존한다.

## 환경과 결과

Windows x64, .NET SDK 10.0.400, C# 14/Roslyn 5.9.0, MSBuild Locator 1.11.2, Node 24.15.0, npm 11.17.0, TypeScript 7.0.2, VS Code API 타입 1.100.0을 사용했다. C# 공존 검사는 Microsoft C# 2.160.4를 별도 profile에 설치해 실행했다.

| 검사 | 결과와 근거 |
| --- | --- |
| TypeScript/기존 계약 회귀 | **PASS**, Node 검사 12개. import 충돌, 실행 metadata, argv/프로세스 tree Stop, Web acknowledgment 계약을 포함한다. |
| 생성 API/구조 | **PASS**, Roslyn emit 및 실제 Doroti API 검사 84개. 템플릿 38개를 모두 컴파일했다. |
| 실제 Widget 조립 | **PASS**, Row/Expanded/Padding의 Element 트리 mount/layout/해제, 추출 Widget의 constructor 입력, 변환한 StatefulWidget의 State build를 실제 RenderObject 트리로 검사했다. |
| helper process/snapshot | **PASS**, handshake, UTF-16/한글/# 경로, 미저장 다른 문서의 타입, 버린 snapshot의 디스크 복원, 제한/취소/평가 실패 fallback. 20회 연속 요청에서도 프로젝트 load는 1회였다. |
| 설치한 VSIX: VS Code 1.140.0 + C# | **PASS**, 실제 prefix 입력→제안 선택→이름 변경→Tab, 빈 파일/기존 alias/import, 명시적 snippet/Quick Fix, wrapping/목록/Undo/Redo/stale 거부, 추출/Stateful preview Apply/Cancel/Undo, 한글 파일 생성/충돌/취소, 인자/생명주기 중복 제외. |
| 설치한 VSIX: VS Code 1.100.0, C# 부재 | **PASS**, 편집 전용 동일 핵심 경로. API 최소 지원 버전을 실제 실행했다. |
| Restricted Mode | **PASS**, 실제 `isTrusted=false`에서 prefix/Tab/이름 연동, qualified template, 관련 없는 프로젝트 제외. helper를 시작하지 않았다. `test-electron`의 강제 trust 해제 옵션을 피하는 별도 launcher를 사용한다. |
| repository 밖 앱 | **PASS**, `dotnet new doroti-app`으로 `%TEMP%/Doroti-vscode-editing-20261007`에 생성한 앱, cached App SDK와 앱 안에 복사한 binary references로 설치형 편집/앱 컴파일. helper나 분석 소스를 repository 경로에서 찾지 않는다. 원래 NuGet package-only restore/distribution 검증과는 구분한다. |
| 패키징 | **PASS**, VSIX 내 catalog, editing JS, helper runtimeconfig/deps, Roslyn 및 MSBuild BuildHost 의존성을 검사했다. 런타임 `Microsoft.Build.Framework.dll`은 SDK Locator가 로드한다. |

준비된 프로젝트에서 provider 경로를 각각 20회 측정했다. 최초 helper/project load와 큰 변환 실행 시간은 이 수치에 포함하지 않는다.

| 환경 | completion p95 | code action p95 | 목표 |
| --- | ---: | ---: | --- |
| VS Code 1.140.0, C# 2.160.4 | 8.98 ms | 9.56 ms | 100 / 200 ms 이내 **PASS** |
| VS Code 1.100.0, C# 부재 | 1.28 ms | 1.15 ms | **PASS** |
| repository 밖 생성 앱, C# 부재 | 2.63 ms | 1.77 ms | **PASS** |

## 명령과 산출물

모든 검증 실행은 repository wrapper의 `--timeout 1200`으로 제한했다. 원시 설치 profile/log/app과 VS Code 다운로드 cache는 `temp/testing/vscode-editing/`에 있다. repository 밖 생성 앱만 지정한 `%TEMP%` 경로에 있다. 원시 자료는 삭제 가능한 로컬 산출물이며 이 문서와 evidence JSON은 별도로 보존한다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti test
python Doroti/eng/run-with-timeout.py --timeout 1200 npm.cmd --prefix tools/vscode-doroti run package
node tools/vscode-doroti/scripts/template-fixtures.cjs temp/testing/vscode-editing/2026-10-07/templateCompilation.cs
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project tools/Doroti.Editor.Assist/tests/Doroti.Editor.Assist.Tests.csproj -c Release -- temp/testing/vscode-editing/2026-10-07/templateCompilation.cs
python Doroti/eng/run-with-timeout.py --timeout 1200 python tools/Doroti.Editor.Assist/tests/protocol.py temp/testing/vscode-editing/2026-10-07/protocol-latest
python Doroti/eng/run-with-timeout.py --timeout 1200 node tools/vscode-doroti/dist/test/runHost.js <fresh-app> <evidence> --editing --csharp
# Separate profiles: --editing --version=1.100.0 / --editing --restricted
```

검사 중 Windows `file:///c%3A/...` 정규화, linked snippet의 `State` suffix 한정, 긴 newline-delimited 메시지 처리, named argument label의 잘못된 capture, 빈 파일에서 completion/import edit가 겹치는 문제를 수정하고 다시 검증했다. helper는 독립 process tree로 소유/종료하며, 편집 helper 실패가 실행 session 정리를 막지 않는다.

## 지원 경계

구조 액션은 canonical Framework Widget의 semantic identity와 완전한 expression/연속 collection selection을 요구한다. partial/복잡한 inheritance, base dispatch, mutable primary capture, 추출의 inline closure/side-effecting 호출/외부 computed member/private type 등은 이유를 표시하고 제한한다. C# 분석 실패를 semantic 성공으로 보고하지 않는다. 두 순수 Painting factory 계열인 EdgeInsets/BorderRadius의 Create 메서드는 추출할 수 있다.

문맥/type 정보와 helper/project 캐시는 source/close/global using/reference 변경에 따라 무효화된다. 문서 및 peer snapshot generation이 달라진 결과는 재계산하거나 거부한다. callback은 State의 최신 Widget을 나중에 읽는 방식으로 바꾸지 않고, 해당 build의 Widget을 capture한다. class/type 변경은 Restart가 필요할 수 있다.

기존 Run/Stop/Hot Reload session 코드는 유지했으며 영향 있는 process/metadata/Web 계약 회귀를 실행했다. 설치형 host에서 별도 development 계약 fixture의 Run 동안 편집하고, Hot Reload request/ack/runtime identity/Stop을 검사했다. 이 fixture acknowledgment는 실제 native metadata delta의 증거가 아니다. 이번 작업에서 네이티브 앱 Run/Hot Reload 제품 검증을 새로 통과했다고 주장하지 않는다. 앱 실행과 독립된 편집 lifecycle은 별도 등록으로 보장한다.

macOS/Linux 실행, 실제 키보드/포인터/한글 IME, native Save dialog의 물리 조작, 접근성, display scanout/GPU/FPS, signing/clean-machine 설치, NuGet-only 배포와 Marketplace 공개는 **notVerified 또는 이번 범위 제외**다. 설치형 command automation과 CPU Element layout을 그 항목들의 PASS로 확대하지 않는다.
