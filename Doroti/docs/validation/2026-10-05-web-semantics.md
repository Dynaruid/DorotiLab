# Web semantics 변경분 전달 — 2026-10-05

**구현 및 변경분 계약 검증 PASS. 확대된 반복 시작·서비스 검증은 PARTIAL.**
현재 소스와 Testbed Debug threaded/main-owned WASM을 검증했다. 성능 FPS나
물리 IME/보조공학 수락을 의미하지 않는다. [구조 계약](../web-host-architecture.md)과
[source/payload hash 및 범위 receipt](2026-10-05-web-semantics.json)를 함께 참고한다.

## 변경

- `BrowserSemanticsTransport`가 현재 노드를 보존하고 첫 연결·Clear·복구에는
  전체 스냅샷, 이후에는 변경 노드·삭제 ID를 보낸다. 값이 같은 갱신은 전송하지 않는다.
- 위치/자식 순서 변경은 기존 내용·액션을 보존하는 compact patch다. 위치만
  바뀌면 자식 목록도 생략한다. 내용 교체는 완전한 내용으로 보내 nullable 필드
  제거도 반영한다. 루트 순서는 topology 변경에만 포함한다.
- DOM은 노드·부모·identifier/controls 관계를 유지한다. 변경 노드, 부모 원점이
  움직인 자식, 관계가 바뀐 controls 참조 노드만 갱신한다. 기존 input/listener와
  선택 상태를 재사용하고 surviving child를 옮긴 뒤 오래된 ancestor를 제거한다.
- stream/revision/baseRevision으로 오래된·중복 packet을 거부한다. delta 기준이
  없으면 한 번만 실제 managed owner에 full snapshot을 요청한다. 복구 요청은
  입력 sequence나 입력 비용 ring의 새 입력으로 기록하지 않는다.
- Framework semantics 생산·urgency·GPU 수명과 protocol envelope version 5는
  유지한다. semantics 내부 packet은 schema version 1이다.

## 검증

| 확인 | 결과와 범위 |
| --- | --- |
| Web Debug build | PASS, 경고/오류 0. SDK 10.0.400, strict TypeScript·JSExport·WASM native relink 포함 |
| 전체 Doroti.Tests Debug | PASS. 새 송신 계약과 기존 CPU/widget/lifetime 회귀 |
| 최종 송신 계약 | PASS. 실제 production encoder의 12단계 JSON을 브라우저 fixture에 전달 |
| Node | PASS 49/49. 새 semantics 6개와 rendering/lifecycle/texture/managed connection 회귀, Node 24.15.0 |
| Chrome 변경분 DOM | PASS. 154.0.8037.93 headless WebGL/main 및 WebGPU/main. 각 12단계의 delta DOM과 full DOM 일치 |
| 포커스·선택·관계 | PASS. 부모 위치·재정렬·reparent·삭제·내용/element kind·identifier 관계·Clear/rebuild. WebGL은 moveBefore 없는 insertion fallback도 실행 |
| 실제 full 복구 | PASS. 각 renderer에서 main→Worker→C#→DOM 요청/응답. 마지막 browser-r6은 synthetic stream 주입 전에 live owner 복구를 검증 |
| 기존 서비스 | WebGL file picker/read/cancel·Back/Forward·text/URL reload restore·합성 drop PASS. WebGPU file picker와 navigation/text restore PASS; 다음 Drop 페이지의 runtime 시작 FAIL |
| 문서/소스 | 링크·Python 문법·git diff whitespace PASS |

고정 **1,001노드** 송신 fixture에서 전체 JSON **221,184자**에 대해 노드 하나의
위치 변경 JSON은 **172자**다. 두 JSON은 ASCII이므로 UTF-8 byte 수와 같다.
geometry delta에는 node 1개만 있으며 children/roots/content가 없다. 값이 같은
노드 갱신은 메시지 0개다. 수신 계약에서도 한 노드만 방문하고 관련 없는 노드
객체를 유지했다. 이는 전송량·방문 범위 증거이며 owner CPU/p95/FPS 개선 측정이 아니다.
공통 Framework의 projection 자체를 증분화한 변경도 아니다.

## 실패와 남은 경계

초기 C# fixture는 `Path`의 UI/IO 이름 충돌로 compile FAIL 후 명시적 IO 경로로
수정했다. 첫 browser-r1은 reload 뒤 노드 대기 timeout이며 상세 DOM 진단은 없었다.
bootstrap 완료/실패 확인을 추가한 browser-r2는 양 renderer 검사를 통과했다.

확대 browser-r3/r4에서 `JSSynchronizationContext:Queue`의 field token과
`System.Runtime` 로딩 실패에 따른 Mono `BadImageFormatException`/deputy startup
`unreachable`을 보존했다. r3은 WebGL reload, r4는 WebGPU Drop 페이지 시작에서
발생했다. browser-r5의 WebGPU reload도 runtime 시작 오류로 FAIL이다.
이 실행들의 부분 통과나 browser-r6의 변경분 계약 PASS로 최초 실패를 지우지 않는다.

해당 시작 오류의 원인은 미확정이다. Doroti export를 호출하지 않는 runtime-only
create 5회는 PASS여서 독립 runtime-only 재현 증거는 확보하지 못했다. 이번 변경으로
시작 오류가 해결됐거나 변경 전에도 발생했다고 확정하지 않는다. 복구 fixture는
live owner의 revision gap 복구를 synthetic 데이터 주입 전에 검증하여, fixture
stream을 비우기 위한 추가 reload와 변경분 복구를 별도로 검사한다.
기존 확대 서비스는 `--services`로 명시 실행할 수 있다.

물리 키보드/한글 IME·screen reader·모바일/Safari/Firefox·standalone worker·GPU
loss/restart 전체 행렬·장기 메모리·실제 표시 FPS는 이번 후보에서 notVerified/notMeasured다.

## 재현과 원시 자료

명령은 모두 1,200초 process-tree timeout으로 실행했고 자동 retry는 없었다.
브라우저 실행은 별도 r1~r6으로 보존했으며 같은 이름으로 실패를 덮어쓰지 않았다.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Tests -c Debug -- --web-semantics temp/testing/web-semantics/new/packets.json
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build samples/DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Debug
python Doroti/eng/run-with-timeout.py --timeout 1200 node --experimental-transform-types --test Doroti/tests/web_semantics.mts
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/web_semantics_browser.py --packets temp/testing/web-semantics/new/packets.json --output temp/testing/web-semantics/new/browser
```

브라우저 명령은 Playwright 환경과 설치된 Chrome이 필요하다. 이번 환경은
`temp/testing/platform-audit/all/venv/Scripts/python.exe`였다. 원시 자료는 삭제 가능한
`temp/testing/web-semantics/2026-10-05/`의 `final-packets.json`, `node.log`,
`source-manifest.json`, `browser-r2/result.json`, `browser-r6/result.json`,
`browser-r3/webgl-services/errors.json`, `browser-r4/webgpu-services/errors.json`,
`browser-r5/failure.json`과 서비스 로그에 있다. receipt의 hash는 관리 assembly와
생성 JS 및 변경 소스를 식별하며 전체 배포 payload나 clean-machine 수락은 아니다.
