# work2.md — Mono 제거·iOS NativeAOT 전환 작업 요약

보관일: 2026-09-11. 사용자 요청에 따라 `history/26-09-10`에 정리하고 루트 `work2.md`를 삭제했다. 2026-09-10 실행과 2026-09-11 iOS Release 기본값 변경까지 반영한 역사 기록이다. 이번 보관에서는 빌드·publish·실기기 검증을 새로 실행하지 않았다.

**최종 기록: 전체 Testbed와 외부 템플릿 소비 앱의 signed NativeAOT publish·iPhone 실행·Mono 제거 검사가 통과했고, iOS 기기 Release 기본값을 NativeAOT로 변경했다. 전체 기능·접근성·장기 수명·정량 성능·원격 CI 검증이 남아 있으므로 G1의 잔여 수명 검증과 G2/G3 전체 완료로 판정하지 않는다.**

## 보관 자료와 읽는 순서

| 자료 | 내용 |
| --- | --- |
| [원문 보존본](work2.original.md) | 삭제 직전 계획, N0~N12 체크박스, 중단·실패·재개·최종 인계 기록 전체 |
| [보관 manifest](work2-archive-manifest.json) | 원본 크기·SHA-256·보관 시각과 경로 |
| [NativeAOT 실행 보고서](../../Doroti/docs/validation/nativeaot-2026-09-10.md) | 환경·최초 장애물·계약 전환·실기기·패키지·최종 크기·기본값 변경 근거 |
| [검증 도구·사용 계약](../../Doroti/validation/native-aot/README.md) | 실행 명령, NativeAOT 프로필, 공개 API 변경과 fixture 범위 |
| [기존 iPhone Mono trimming 보고서](../../Doroti/docs/validation/ios-trimming-2026-09-10.md) | 전환 이전 비교 자료; NativeAOT 증거와 구별 |
| [MediaQuery·SafeArea 기록](media-query-safe-area-summary.md) | 입력·metrics 관련 별도 검증 범위 |

원문은 byte 단위로 보존했으므로 내부 상대 링크는 작성 당시 **저장소 루트 기준**이다. 이 요약의 링크는 보관 위치 기준이다. 원문의 머리말·§7.1~7.7에 남은 과거 실패/진행 상태는 §7.8 최종 설치·패키지 검증과 §7.9 기본값 변경보다 앞선 기록이다. 아래 결과는 문서에 기록된 당시 증거를 요약하며 현재 기기 상태를 새로 확인한 결과가 아니다.

## 목표와 주요 변경

- 목표는 iOS 배포 앱의 Mono 런타임·인터프리터 의존 제거다. 현재 Doroti C# 제품 코드를 기준으로 삼고 전체 Testbed 기능과 외부 Widget/State/Route/Action/지역화 확장을 유지한다. NativeAOT 자체 GC/runtime 지원은 남는다.
- .NET 10/MAUI 10에서 실제 ILC 진단과 최소 앱으로 HybridWebView 의존성 장애를 분리했다. 이후 .NET SDK `11.0.100-rc.1.26425.128`, iOS SDK `26.5.11720-net11-p6`, MAUI `11.0.0-rc.1.26451.6`으로 실제 호스트와 Testbed를 검증했다. SkiaSharp 기준은 `4.154.0-preview.1.26454.9`다.
- Runtime callback/index, RenderView·지역화·layout, 이미지·복원·focus·form·sliver·calendar, route/router·menu·drag·tree·Inspector를 정적 호출 계약으로 전환했다. shader owner와 JSON 등록도 명시적으로 연결했다. 런타임 DLR/reflection 우회 wrapper로 대체하지 않는다.
- Runner SDK·CLI·템플릿에서 compilation mode, TFM/RID, restore graph, obj/bin, lock·launch identity를 연결하고 Mono interpreter·보존 descriptor가 NativeAOT에 유입되지 않도록 분리했다. 실행 프로젝트의 `PublishAot`를 라이브러리에 전파하지 않는다.
- 검사 도구는 실제 IL의 CallSite, AOT/trim 분석, ILC·native link 입력, 최종 번들·설치 identity, 크기와 변조 여부를 구분한다. `dynamic` 텍스트 수나 Mono 문자열 부재만으로 제거를 판정하지 않는다.
- 번역기 전체 재생성·프레임워크 동일 재현은 범위 밖이다. N3의 제한된 번역기 개선은 미선정 backlog이며 제품 NativeAOT 완료의 필수 게이트가 아니다.

## 기록된 최종 검증 결과

아래 파일명은 실행 보고서의 `Doroti/artifacts/native-aot/` 원자료 식별자다. 이 보관에서는 원자료를 이동하거나 재생성하지 않았다.

| 영역 | 기록된 결과와 근거 |
| --- | --- |
| 실제 호스트 probe | NativeAOT compile/link/서명·iPhone 12 렌더링, 이미지·셰이더·native round-trip 및 GC 후 completion 확인. `net11-probe-mono-evidence.json`: `monoAbsent=pass`, `nativeAotPublish=pass`. 전체 수명 검증은 별도 |
| 전체 framework | `static-ui-clean-layers-final/gate-results.json`: 12개 assembly의 Debug/Release DLR 0개·AOT 분석 통과 |
| 기능 회귀 | `static-ui-regressions/results.json`: route, form/sliver, scaffold metrics, scroll/tap, section index, iOS 텍스트 메뉴, virtual dispatch, 이미지/seed 팝업, image sizing 9개 통과. 드래그·트리·고정 헤더 검사도 통과 |
| Testbed 배포·Mono 제거 | `net11-testbed-release-publish-retry.*`: signed NativeAOT publish 경고·오류 0건. `net11-testbed-release-mono-evidence.json`: publish·Mono 부재 통과. `net11-testbed-final-install.json`, `net11-testbed-final-launch.json`: 재설치·실행 |
| Testbed 실기기 조작 | `net11-testbed-interaction-evidence.json`: 포인터 906회, presented 4,414회, failed 0, software fallback 0, 마지막 입력/표시 sequence 906. 모든 기기 시나리오 통과로 확대하지 않음 |
| 최종 기기 스냅샷 | `net11-testbed-final-device-evidence.json`: 390×844 논리 화면, presented 1,237 / failed 0 / fallback 0. 소비 앱 검증 후 NativeAOT Testbed 복구 기록 |
| 패키지·템플릿 | `ios-package-validation/result.json`: iOS 실험 feed 22개 패키지. `ios-template-consumer/result.json`: 별도 NuGet 캐시·repository 제품 ProjectReference 없는 설치 템플릿 앱의 signed publish·기기 실행 통과. 외부 generic State/LocalizationsDelegate/Action `PASS value=7`, presented 2 / failed 0 / fallback 0, `monoAbsent=pass` |
| 도구·자동화 | mode/descriptor/interpreter·증거 누락/변조·bytes 계약 검사 통과. CI는 12개 framework와 9개 native fixture matrix로 확장했으나 원격 CI 실행은 미검증 |
| 기본값 변경 | `ios-release-default-contract.log`: 7개 계약 테스트 통과. identity 회귀 통과. `ios-release-default-no-rid-publish.result.json`: mode/RID 생략 Release publish 71.06초, exit 0, NativeAOT 코드 생성·서명 IPA 생성 |

## 크기 비교

| 지표 | .NET 10 Mono | .NET 11 NativeAOT |
| --- | ---: | ---: |
| 서명 `.app` 일반 파일 합계 | 104,701,336 bytes | 42,616,772 bytes |
| 압축 IPA | 37,351,329 bytes | 18,185,146 bytes |

`final-size-comparison.json` 기준 `.app`은 **62,084,564 bytes / 59.30% 감소**했다. SDK·MAUI 버전이 달라 Mono 제거만의 효과로 단정하지 않는다. `.app`은 42.62 decimal MB, IPA는 18.19 MB이며 설치 공간·스토어 전송량과 다른 지표다. 사용자의 약 56MB 설치 공간·버벅임 없는 동작 관찰은 별도 사용자 관찰 기록이며 정량 성능 합격을 의미하지 않는다. 비교용 Mono 앱으로 기기 설치 앱을 덮어쓰지 않았다고 기록되어 있다.

## iOS Release 기본값과 복구 경로

2026-09-11 추가 요청에 따라 iOS 기기 `ios-arm64` Release에서 mode를 생략하면 NativeAOT를 선택한다. Release에서 RID도 생략하면 `ios-arm64`를 선택한다.

```powershell
pwsh -File Doroti/eng/doroti.ps1 publish -App DorotiTestbedApp -Platform ios -Configuration Release
```

Debug·명시적 시뮬레이터 RID·다른 플랫폼의 기본값은 유지한다. 명시적 `-CompilationMode Mono`, `-p:DorotiCompilationMode=Mono`, 직접 MSBuild의 `PublishAot=false` 선택은 우선한다. Mono 복구·비교는 .NET 10/MAUI 10 프로필을 사용한다. 실행 기록상 .NET 11 iOS Mono 선택은 `NETSDK1242`로 거부되며 CoreCLR로 조용히 대체하는 비교는 제외했다.

## 남은 게이트와 인계 범위

| 단계 | 보관 시점 판정 |
| --- | --- |
| N0/N10 | 환경·장애물·크기 도구와 Mono 크기 비교 확보. 통제된 startup/frame/memory baseline·반복 비교·정량 성능 판정 미완료 |
| N1/G1·N7 | 실제 호스트·리소스·콜백·Mono 제거 증거 확보. native/view/observer의 장기 수명·GC·dispose 전체 검증은 남음 |
| N2/N4~N6 | 정적 계약 전환·외부 소비 fixture·전체 DLR 제거와 기능 회귀 확보. 원문 N4의 Mono 회귀/descriptor 관련 미체크 조건은 원문대로 보존 |
| N8 | Runner/CLI·프로필 분리·템플릿/NuGet 외부 소비 publish·기기 검증 확보 |
| N9/G2 | 전체 Testbed 배포와 일부 실제 조작 검증 통과. 회전·한글/IME·접근성 전 시나리오, cold start/복귀·반복 view 해제·GC 등 필수표 전체 완료는 아님 |
| N11 | 자동 검사·회귀 fixture 구현. 원격 GitHub CI·서명 CI와 타 OS 호스트 실행·공유 계약 전체 회귀 미검증 |
| N12/G3 | 사용 계약·패키지 소비 및 요청된 iOS Release 기본값 변경 완료. 최종 인계 체크박스와 전체 기능·성능 게이트는 미완료 |

iPhone 회전 명령 미지원과 MAUI KVO observer finalizer 경고의 조사 기록을 유지한다. 장비 제약·중단·디스크 부족·초기 ILC 실패를 PASS로 바꾸지 않는다. 과거 Mono full-trim 성공과 작은 fixture 성공도 전체 NativeAOT 전환 성공으로 합산하지 않는다.

iOS 결과는 Android·Web·Windows·AppKit·Catalyst·Linux 전체의 Mono 제거 또는 NativeAOT 지원 선언이 아니다. 당시 실행 기록의 커밋·작업 트리 해시 구분과 20분 timeout 정책은 원문·검증 보고서에 보존한다. 이번 문서 보관은 미완료 구현이나 검증 게이트를 실행한 작업이 아니다.
