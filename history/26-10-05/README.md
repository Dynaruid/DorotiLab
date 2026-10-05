# 2026-10-05 작업 문서 보관

보관일: **2026-10-05 KST**. 루트 `work.md`, `work2.md`, `work3.md`의 목적·구조 계약·구현·검증 이력과 남은 수락을 요약했다. 원본 세 파일은 요약과 참조 경로를 확인한 뒤 삭제한다.

| 원본 | 보관 문서 | 보관 시점의 상태 |
| --- | --- | --- |
| `work.md` | [종합 리뷰 후속 결과](work-summary.md) | F01–F10·A01–A08·D01–D07 수정 및 자동화, 선택 호스트 검증. 제품 전체 **PARTIAL** |
| `work2.md` | [디자인·플랫폼 독립 구조와 Windowing](work2-summary.md) | 구조 전환의 기준 계약과 DS0–DS9·PW0–PW8. 실행 항목 21개 체크, 80개 미완료; 잔여는 work3로 이관 |
| `work3.md` | [플랫폼별 후속 실행과 잔여 수락](work3-summary.md) | 공통·Apple·iPhone Debug/Mono AOT/NativeAOT·Android·Windows 후속 기록. 44개 항목의 최종 완료 기준은 미충족, 전체 **PARTIAL** |

문서 관계는 **리뷰 수정 결과 → 구조 전환 계약 → 플랫폼별 후속 실행**이다. work2의 미완료 체크는 부분 구현이 없다는 뜻이 아니며, work3의 후속 PASS는 같은 후보·환경·검사 범위에만 적용한다. 리뷰 당시 Apple/Android SKIPPED를 다른 날짜의 후속 결과로 소급 변경하지 않는다.

보관 시점의 [execution.json](../../Doroti/docs/migrations/design-platform/execution.json)은 `state=inProgress`, `complete=false`이고, [latest-checkpoint.json](../../Doroti/docs/migrations/design-platform/latest-checkpoint.json)은 `wholePlanComplete=false`다. 초기 iPhone NativeAOT 입력 검사에서 관찰한 `Metal terminal NotEnqueued` 1회는 후속 3회 통과 뒤에도 원인 미확정으로 남아 있다. iOS Components의 60 FPS 유지·엄격한 회전 phase 예산, 전체 provider/프로파일 및 물리·배포 수락도 미완료다.

이 보관 작업은 문서 요약·링크/검사 경로 정리·원본 삭제다. 제품 빌드·장치·성능 검증을 새로 수행하거나 기존 미완료 항목을 완료 처리하지 않는다. `PASS`, `FAIL`, `PARTIAL`, `SKIPPED`, `notVerified`, `notMeasured`, `Unsupported`는 각 기록의 범위를 유지한다.

추적되는 validation·migration 문서와 JSON에 명령·후보/패키지/native hash·판정 범위를 보존한다. `temp/testing/`와 `Doroti/artifacts/`의 로그·이미지·캐시·빌드는 삭제 가능한 로컬 산출물이며, 요약에 경로가 등장해도 현재 파일의 존재를 보장하지 않는다.
