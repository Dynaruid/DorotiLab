# Doroti 국제화·현지화와 합성 문단 요약

원본 `work2.md` · 작성 2026-11-13 · 최종 검증 2026-11-26 · 보관 2026-11-26 KST

**PARTIAL.** 공통 로케일 처리, .NET 기반 포맷, 위젯 번역, ARB 생성과 언어 확대를 구현했다. 합성 문단은 `-p:DorotiShapedParagraphs=true`로 선택한다. 마지막 Windows CPU 표본의 영어 레이아웃과 기록 그림 재생은 10% 회귀 예산을 충족했고, Apple 실행과 실제 기기의 입력·접근성·표시 성능 인수가 남았다.

## 설계 결정

- Flutter식 `LocalizationsDelegate<T>`, subtree 리소스와 ARB의 사용 흐름을 따른다. `Doroti.Intl`은 공개 .NET API와 명시적 culture로 날짜·시간·숫자·통화·percent를 포맷하고, Gregorian picker 및 엄격한 날짜 파싱을 지원한다. 필요한 cardinal plural은 CLDR 규칙으로 보강한다. 출력과 파싱은 선언한 .NET/OS 데이터 및 위젯 정책을 기준으로 검증한다. [Intl API와 지원 범위](../../../../Doroti/src/Doroti.Intl/README.md)
- Windows App SDK·MAUI·Qt·Web의 우선 언어 목록은 공통 파서로 정규화한다. language·script·region과 주요 alias를 처리하고 variant·extension·private-use 태그는 거부한다. `zh-CHS`와 `zh-CHT`는 `zh-Hans`와 `zh-Hant`로 정규화한다. Web은 `navigator.languages` 변경을 전달한다. 앱과 subtree는 확정된 리소스·방향·semantics를 함께 바꾼다. 비동기 로드가 겹치면 마지막 요청이 우선한다. 초기 실패는 첫 프레임 대기를 풀고, 전환 실패는 이전 리소스를 유지한다.
- 영어 기본 delegate 위에 Widgets·Material·Cupertino 번역을 opt-in 패키지로 제공한다. 앱과 위젯은 같은 증분 생성기로 ARB에서 타입 있는 정적 C# API·delegate를 만든다. `AdditionalFiles`의 추가·삭제를 추적하고 키, placeholder, 메시지 문법, 포맷 오류에 소스 위치를 보고한다. [위젯 패키지](../../../../Doroti/docs/localization/widget-packages.md) · [ARB 생성기](../../../../tools/Doroti.Localization.Generator/README.md)
- Web 번역 데이터와 .NET WASM ICU 파일은 각각 선택한다. full ICU 또는 요구 culture를 담은 subset을 사용하며 누락·손상은 시작 오류로 보고한다. 앱과 텍스트 엔진은 기존 렌더 Worker의 소유권을 따른다. [Web 실행 모드와 지원 범위](../12/web-execution-modes-summary.md)

## 구현·검증 이력

| 단계 | 당시 결과 |
| --- | --- |
| P0 · 11월 16일 | `en-US`/`ko-KR` 포맷·파싱, CLDR 49 cardinal subset과 위젯 API 219개 inventory를 마련했다. Windows·WSL Linux에서 공통 계약·trim·NativeAOT·독립 NuGet 소비를 확인했다. SDK 10/11의 net10 × 양 모드 × Debug/Release AOT에 해당하는 8개 Chromium 런타임 probe와 제한된 Firefox 검사가 통과했다. CJK 데이터로 한영 전환을 확인했고 EFIGS-only의 한국어 전환은 명시적으로 실패했다. [P0 계약](../../../../Doroti/docs/localization/p0-contract.md) · [실행 증거](../../../../Doroti/docs/localization/p0-evidence.json) · [위젯 API inventory](../../../../Doroti/docs/localization/widget-inventory.json) |
| P1~P3 · 11월 26일 | Windows·WSL에서 태그 우선순위, 다중 view/subtree 격리, 비동기 역순·실패를 확인했다. CLDR 49의 39개 그룹에서 추출한 소수 예제 1,651개와 `pt-PT` 규칙이 통과했다. SDK 10/11 analyzer NuGet 소비, `DLOC001`, ARB 삭제 뒤 registry 갱신, Windows NativeAOT 실행을 확인했다. |
| P5 · 번역 확대와 크기 | Widgets 82개 언어·122개 태그, Material 82개·122개, Cupertino 81개·121개를 Windows·WSL에서 로드했다. `DorotiWidgetLocales=ko`로 en/ko만 등록할 때 SDK 10 Debug의 trim 전 세 위젯 DLL 합계는 1,407,488→73,216바이트였다. 검증한 full/CJK ICU 파일은 각각 1,526,128/956,416바이트였다. [번역 출처와 변경 내역](../../../../Doroti/docs/localization/widget-provenance.json) |

## P4 합성 문단

공식 SkiaSharp 4.156.0/HarfBuzzSharp 14.4.0.100에서 HarfBuzz 합성과 C# 문단 배치를 연결했다. Unicode 17.0 분석과 ICU 79.1 사전으로 태국어·라오어·크메르어·미얀마어의 단어 선택과 줄바꿈을 처리한다. 확정된 한 배치의 UTF-16 클러스터·방향·좌표를 그리기, 캐럿, 선택, 접근성에 전달한다. 합쳐진 Material 입력란의 좌표 변경과 암호 입력란 좌표 비노출도 검증했다. OpenType 기능·가변 글꼴 축을 전달하고, 제한된 캐시에 합성과 배치를 저장한다. 기록된 그림은 당시 글꼴과 글리프 위치를 보유한다. [후보 검토·제품 통합·재현 명령](../../../../Doroti/docs/localization/paragraph-feasibility.md) · [Unicode 출처](../../../../Doroti/docs/localization/paragraph-unicode-provenance.json)

| 환경 | 11월 26일 확인 범위 |
| --- | --- |
| Windows·Linux 계약 | Windows x64 JIT/NativeAOT·WSL Linux x64 JIT 문단 probe에서 bidi·ZWJ·선택·캐럿·사전 경계와 픽셀을 확인했다. Unicode 17 표준 113,755건, ASCII 비교 23건, 기본 렌더러 spaced-text 21건과 retained shadow/text 24건이 통과했다. Debug 스크롤 스위트의 상속 알림 중단을 수정한 뒤 Windows Debug·Release 스크롤 회귀 전체가 통과했다. |
| Web 제품 probe | SDK 10.0.403·11.0.100의 net10에서 compatible·threaded, Debug·Release/AOT, Chromium·Firefox·WebKit **24개 조합**이 통과했다. 외부 요청을 차단한 재실행에서도 24장 PNG 해시가 일치했다. SDK 10 threaded Release의 첫 WebKit 시도는 타임아웃에 도달했고 동일 산출물의 재시도에서 통과했다. Chromium Demo에서는 언어 전환, 활성 DOM 입력 `lang`/`dir`, 영어 subtree, ICU 파일 누락·손상을 확인했다. |
| Windows App SDK·Qt UI | Windows Demo의 UI Automation `TextPattern`에서 ZWJ 범위·좌표·선택을 확인했다. Qt 6.12 WebEngine 제외 번들을 WSL/Xvfb의 Mesa 소프트웨어 Vulkan으로 실행해 두 로케일의 앱·위젯 문구, AT-SPI 캐럿, 아랍어 합자·결합 문자·ZWJ 문자 사각형을 확인했다. Qt 6.10.2의 초기 화면 검증과 별도의 결과다. |
| Android 앱 | SDK 10 ARM64 Debug Demo를 갤럭시 SM-S931N/API 37에서 실행했다. 공식 Skia 4.156.0 및 ARM64 HarfBuzz 패키지를 확인했고 en-US→ko-KR 전환과 영어 subtree를 검증했다. API 33 x64 에뮬레이터에서도 .NET 10과 .NET 11/CoreCLR APK로 두 로케일을 확인했다. 두 검증의 설치 APK 해시를 확인하고 이후 테스트 앱을 제거했다. |

## 최종 성능과 남은 인수

마지막 변경은 폭 변경 시 불변 줄 배치를 먼저 만들고, 왼쪽 정렬 ASCII 누적 너비와 줄 단위 캐럿 좌표를 재사용한다. 선택·접근성과 첫 그리기는 필요한 시점에 좌표를 구성한다. 같은 Windows x64 Release 기본 JIT CPU fixture에서 줄바꿈 영어 폭 변경 p95는 이전 HarfBuzz 54.2→4.3µs였고 기존 advance 방식은 8.3µs였다. 폭 변경과 캐럿 조회는 60.7→8.9µs, 폭 변경과 첫 그리기는 p50 67.0→39.1µs, p95 78.4→79.7µs였다. 이 표본에서 영어 레이아웃과 기록 그림 재생의 예산을 충족했다. [최종 계측과 이전 기준 비교](../../../../Doroti/docs/localization/paragraph-feasibility.md#deferred-paragraph-geometry-and-paint)

마지막 문단 수정은 Windows JIT/NativeAOT, WSL Linux JIT, Chromium 양 모드에서 다시 검증했고 플랫폼별 고정 예문의 PNG 해시가 일치했다. Firefox·WebKit 전 구성 통과는 그보다 앞선 제품 probe를 가리킨다. Apple 지원 runner, 실제 IME 조합과 스크린 리더, 첫 현지화 프레임, 실제 GPU·표시 프레임 및 전체 배포 크기 인수가 남았다. 합성 문단은 opt-in으로 제공한다.

원시 로그와 게시본은 `Doroti/artifacts/intl-p0`, `Doroti/artifacts/work2-paragraph`의 삭제 가능한 로컬 산출물이다. 이 문서는 당시 검증을 요약한다. 원문은 `git show a9a524647d276f89d8871128c2e3e5accd1f3337:work2.md`로 조회할 수 있다.
