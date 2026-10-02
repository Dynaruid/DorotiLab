# works 폴더 보관 요약

보관일: **2026-10-03** · 원래 위치: 루트 `works/` · 보관 위치: [works 보관 인덱스](works/README.md)

루트 `works` 폴더를 없애면서 기존 계획·체크리스트·실행 근거를 이 날짜 폴더에 보존했다. 원본 문서 35개와 JSON 16개, 총 **51개 파일**을 옮겼으며 상대 링크와 보관 안내를 정리했다. 결과 JSON은 내용을 변경하지 않고 그대로 보존했다. 루트 `works/`는 제거했다.

## 보존한 구성

| 자료 | 수량 | 보관 위치 |
| --- | ---: | --- |
| 전체 계획·실행 인덱스 | Markdown 1개 | [인덱스](works/README.md) |
| 공통 00~10 작업·결과 | Markdown 11개 | [common](works/common/) |
| Windows/Web/Android/macOS/iOS/Linux/Catalyst | Markdown 7개 | [platforms](works/platforms/) |
| 날짜별 실행 보고서와 집계 | Markdown 16개 + JSON 16개 | [results](works/results/) |

공통 문서는 기반·Testing·Desktop·입력/접근성·렌더링 수명·VS Code·플러그인·OS 드롭·Navigation·멀티윈도우·출시 작업의 당시 완료/잔여를 보존한다. 실행 보고서와 JSON에는 2026-09-29~10-03 플랫폼 후속, VariableBlur/Fixed, NativeAOT 설치, iOS C/Fast, 공통 C/단일화/texture 예산, Qt 구성/Hot Reload, Apple/AppKit 검토와 후보 identity·실패/재시도를 남겼다.

## 기존 요약과 연결

- [plan 진행 순서](../26-09-28/plan-summary.md), [work2 미완료 검토](../26-09-29/work2-summary.md)
- [work3 Fixed 후보](../26-10-01/work3-summary.md), [work4 iOS C/Fast](../26-10-02/work4-summary.md)
- [work5 공통 C](../26-10-02/work5-summary.md), [work6 C 단일화·동적 texture 예산](../26-10-02/work6-summary.md)

위 요약과 제품 지원표·계약 문서·테스트 안내·샘플 README의 링크를 새 보관 경로로 연결했다. `Doroti/eng/validate.py`도 루트 `works` 대신 이 보관 자료를 검사하도록 경로를 변경했다. 이후 제품 지원 현황은 [지원표](../../Doroti/docs/support-status.md)와 [제품 문서](../../Doroti/docs/), 실행 가능한 검사는 [테스트 안내](../../Doroti/tests/README.md)를 따른다.

## 판정과 증거 보존 경계

보관 당시 **M0 로컬 기반 완료, M1~M7 PARTIAL**과 각 실행의 `PASS`, `FAILED`, `SKIPPED`, `notVerified`, `notMeasured`, `unsupported`를 그대로 유지한다. 10월 2일 Apple 생략과 10월 3일 새 후보의 자동 검증을 구분하며, JSON의 source/payload hash·과거 경로 문자열도 당시 실행 이력으로 보존한다.

이번 작업은 문서 이동·링크·검증기 경로 정리다. 제품 runtime·물리 입력·접근성·실제 표시 성능·장기 사용·배포를 새로 검증하거나 기존 미완료 인수를 통과 처리하지 않는다. `temp/testing/`·`Doroti/artifacts/` raw는 삭제 가능한 로컬 자료이며 현재 존재를 보장하지 않는다.
