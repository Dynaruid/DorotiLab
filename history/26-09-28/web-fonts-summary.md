# 웹폰트 지원 및 공통 앱 폰트 에셋 정리 요약

- 구현·검증 및 보관일: 2026-09-28.
- 대상: 루트 `work.md`의 웹폰트 지원 1~5단계 구현, 브라우저 검증, 공통 앱 assets 이전 기록.
- 최종 상태: **1~5단계 구현 완료**. 실제 Chromium Doroti 캔버스와 browser-wasm Skia 검증은 PASS이며, 물리 키보드·OS 한글 IME는 **notVerified**다.
- 아래 결과는 원문에 기록된 검증 이력이다. 이번 보관 작업에서 제품 빌드나 브라우저 검증을 다시 실행하지 않았다.

## 목표와 사용 방법

폰트 처리 경로는 **CSS/에셋 선언 → 폰트 바이트 확보 → 필요 시 압축 해제 → Skia 등록 → 측정·렌더링**이다. CSS 로딩 완료만으로 Skia 등록을 대체하지 않는다.

| 사용 방식 | 설정과 동작 |
| --- | --- |
| 외부 CSS | HTML에 `https://cdn.jsdelivr.net/npm/galmuri/dist/galmuri.css` stylesheet 링크를 추가하고 Doroti `fontFamily: "Galmuri11"` 선택 |
| 로컬 CSS + WOFF2 | `fonts/SUITE/SUITE-Variable.css`를 링크하고 `fontFamily: "SUITE Variable"` 선택 |
| 명시적 에셋 | 기존 `BrowserFontAsset(family, url)` 또는 `BrowserFontAsset.Embedded(...)` 사용 |
| 외부 통신 없는 폰트 구성 | `AssetsOnly`, 명시적인 로컬 CSS, 로컬 디코더를 함께 구성; SampleApp2는 `DorotiSampleWebFontSource=Assets` 사용 |

CSS 링크는 폰트를 사용 가능한 상태로 등록한다. 앱 기본 폰트는 Doroti 테마/기본 family로 지정하며, HTML `body`의 `font-family`를 위젯에 상속하는 기능은 포함하지 않는다.

사용법과 설정은 [웹 폰트 문서](../../Doroti/src/Doroti.Host.Web/Fonts/README.md), 실행 예제는 [SampleApp2 문서](../../samples/DorotiSampleApp2/README.md)를 참고한다.

## 주요 구현

| 영역 | 구현 결과 |
| --- | --- |
| CSS 수집 | `doroti.web.css-fonts.ts`가 CSSOM을 우선 사용하고 외부 CSS는 CORS fetch로 보완. 최종 응답 URL 기준 상대 경로, import 순환·깊이·중복, media/supports/layer, source 후보와 descriptor 보존 |
| worker 전달 | `BrowserCssFonts`와 `css-fonts` 제어 요청으로 DOM 수집 결과를 render owner에 전달 |
| 공개 옵션 | `DiscoverCssFonts`, `CssFontsSameOriginOnly`, `CssStylesheets`. `AssetsOnly`는 자동 발견을 끄고 명시한 로컬 CSS만 동일 출처 제한으로 처리 |
| 다운로드·등록 | `BrowserStartupFonts`/`BrowserFontAsset`: 동시 다운로드 4개, URL·내용 hash 공유, source 실패 시 다음 후보, 크기·취소·시간 제한, HTML fallback 거부, 명시적 에셋 우선 |
| 초기화·갱신 | 첫 뷰 생성 전에 등록하고 CSS 기본 family도 검사. 이후 변경은 기존 Changed/cache invalidation/fontsChange 경로 사용 |
| 포맷 | `BrowserFontData`가 파일별 WOFF2 직접 지원을 확인하고 실패 시 전용 디코더 사용. WOFF1은 크기·테이블 checksum 검증 후 SFNT로 정규화 |
| face 매칭 | CSS alias, weight 범위, slant/width, unicode-range와 실제 glyph coverage 반영. Galmuri normal/condensed와 동일 family subset 구분 |
| 가변 축 | 요청 weight를 `wght`로 연결하고 명시 축 우선·범위 clamp 적용. 측정과 그리기에 같은 clone 사용, 축을 캐시 키에 포함, synthetic bold 중복 방지 |
| 자원 수명 | clone은 기존 256-entry text LRU에서 소유하며 eviction/폰트 변경 시 해제. 기존 DisplayList/paragraph/picture 축 전달도 확인 |
| 샘플·패키징 | Fonts 탭에 Roboto/Galmuri11/SUITE 전환, 300/400/500/700/900 및 연속 wght, 다국어·여러 줄 입력 추가. `DorotiBundleWoff2Decoder` opt-in 및 HTML 충돌 방지용 `DorotiWebWwwrootExclude` 제공 |

암묵적으로 발견한 CSS의 실패는 진단 후 폴백으로 계속하며, 필수 에셋·기본 폰트 실패는 시작 오류로 처리한다. 일반 외부 CSS 링크에 `crossorigin` 속성을 필수로 요구하지 않지만 서버의 CORS 허용은 필요하다. OS 로컬 폰트의 바이트를 얻을 수 없는 `local()` 후보는 건너뛴다.

## 포맷과 가변 축 검증 기록

browser-wasm 실제 render owner에서 확인한 결과다.

| 입력 | 원본 바이트 | 직접 등록한 glyph 수 | 결과 |
| --- | ---: | ---: | --- |
| Galmuri11 TTF | 5,376,428 | 20,968 | PASS |
| MaterialIcons OTF | 1,645,184 | 8,661 | PASS |
| Galmuri11 WOFF1 | 878,860 | 20,968 | PASS |
| SUITE Variable WOFF2 | 535,788 | 2,953 | PASS |

- SUITE 2.040의 300/400/450/500/700/900 요청 좌표와 native typeface 좌표가 일치했고, 글리프 A의 outline SHA-256은 모두 달랐다.
- 직접 WOFF2 경로는 원본 535,788바이트로 검증했다. 별도 JS/WASM 디코더는 1,332,100바이트 SFNT를 생성하고 fvar/gvar 및 300~900 축을 보존했다.
- Windows native 검사는 TTF 성공, WOFF/WOFF2 직접 로딩 실패였다. browser-wasm 결과를 다른 백엔드의 지원으로 일반화하지 않는다.
- Galmuri 재현 에셋은 2.40.3으로 고정했다. 버전 없는 원본 CDN URL의 응답은 이후 달라질 수 있다.

## 자동·화면 검증 기록

| 항목 | 기록된 결과 |
| --- | --- |
| CSS parser/CORS/import/redirect/source/same-origin | PASS — `collect.cjs` |
| 실제 WOFF2 해제·손상 header·WOFF1 checksum/정규화 | PASS — `decode.mjs`, C# css-fonts |
| native renderer 굵기별 픽셀·중간 450·측정·clamp·subset·condensed | PASS |
| publish 루트 `/` 및 하위 `/sample/` | PASS — 실제 캔버스, 폰트 전환·굵기 slider·입력, 정상 폰트/디코더 바이트 |
| 외부 통신 없는 폰트 구성 | PASS — 외부 origin 차단 상태에서 루트·하위 경로 각각 외부 요청 0건 |
| 브라우저 입력·커서·선택·줄바꿈 | PASS — 144자 한글·영문, CDP 조합 완료, 키보드 선택, viewport 재배치 |
| 물리 키보드·OS 한글 IME | **notVerified** — CDP 조합 입력과 별도 |
| 기존 font-assets/font-downloads/font-resolution | PASS |
| 기존 Cupertino 레이아웃·탭·입력·다이얼로그·테마 | PASS |
| 원본 Galmuri URL 기본 CDN 모드 | PASS — `cdn.cjs`, 실제 캔버스 Galmuri/SUITE 전환 |
| 기본 웹 Release 빌드 | 성공 — 경고 0개, 오류 0개. opt-in probe 없는 일반 개발 서버로 최종 화면 확인 |

검증 프로세스에는 `run-with-timeout.py`의 1,200초 제한을 적용했다. CDN 모드 단일 관측은 폰트 응답 14개·본문 합계 6,344,096바이트, 탐색 시작부터 worker ready까지 약 6.96초였다. WASM 초기화와 네트워크를 포함하므로 반복 benchmark나 성능 보장이 아니다. 호환 디코더 CDN 요청도 발생했으며, 모든 WOFF2가 디코더 없이 동작한다는 의미는 아니다.

## 공통 앱 assets 이전과 SDK 수정

- SUITE 원본 CSS/WOFF2/LICENSE를 [공통 SUITE 폴더](../../samples/DorotiSampleApp2/assets/fonts/SUITE/)로 이동했다.
- 샘플이 참조하던 `Doroti/validation/css-fonts/fixtures`가 체크아웃에 없어 Galmuri 2.40.3 WOFF2 세 파일·라이선스·로컬 CSS를 [공통 Galmuri 폴더](../../samples/DorotiSampleApp2/assets/fonts/Galmuri/)에 포함했다. 검증 폴더를 런타임 폰트 공급원으로 사용하지 않는다.
- 웹 프로젝트는 `../assets/fonts/**/*`를 `wwwroot/fonts/%(RecursiveDir)%(Filename)%(Extension)`로 배포한다. 공개 URL `fonts/SUITE/...`, CSS 상대 경로 및 기본 CDN/Assets 선택은 유지된다.
- 개발 서버에서 Link 항목의 ContentRoot가 `web/wwwroot`로 기록되어 CSS가 HTTP 200·빈 본문으로 반환되는 결함을 발견했다. SDK의 `DorotiResolveLinkedWebContentRoots`가 실제 원본 폴더와 Computed source를 기록하도록 수정해 외부 소스 폴더도 읽게 했다.
- 후속 검증은 삭제된 validation 디렉터리 복원 없이 임시 helper의 1,200초 프로세스 트리 timeout으로 수행했다. 기본 CDN 빌드, Assets 빌드, Assets publish가 성공했다.
- 개발 서버와 `/sample/` publish의 CSS/폰트/라이선스 8개 응답이 원본과 byte-for-byte 일치했다. 두 환경의 Chromium Doroti 캔버스에서 SUITE/Galmuri 전환을 확인했고 외부 요청은 0건이었다.

## 지원 제한과 후속 범위

- 시작 시 유효 face를 preload한다. 브라우저 전체 CSS font matching/`font-display`, 동적 link·CSSOM 재스캔, family/weight/glyph별 지연 다운로드는 후속 범위다.
- 수집 제한은 CSS 64개, import 깊이 8, face 128개, CSS당 2 MB, 수집 20초, 폰트당 30 MB다. 대규모 subset CSS는 전송량·시작 시간 측정이 필요하다.
- 일반 위젯 API는 normal width를 선택하며 condensed는 별도 family alias로 지정할 수 있다. oblique 각도/stretch 범위의 완전한 지원은 주장하지 않는다.
- 동일 출처 전용 CSS 모드는 외부 요청을 막기 위해 redirect 자체를 거부한다.
- 직접 fetch에는 CSP `connect-src`, 디코더에는 `script-src`/WASM 정책과 worker COOP/COEP 조건이 관련된다. pin된 `woff2-encoder@2.0.0`은 WASM을 JS 내부 base64로 포함하여 JS와 LICENSE를 publish한다. 이 호환 디코더 실행에는 `script-src 'unsafe-eval'`이 필요하며, `wasm-unsafe-eval`만 허용한 구성은 실제 실패했다.
- 폰트·디코더는 샘플 opt-in 자산이며 `Doroti.Host.Web` 기본 패키지에 강제로 포함하지 않는다.
- 물리 키보드·OS IME 검증은 미완료로 남긴다.

## 증거 보존 경계

원문은 `Doroti/artifacts/css-fonts/`의 `browser-report.json`, `collector.json`, offline/selection/wrap/suite PNG와 `Doroti/artifacts/shared-font-assets/`를 검증 산출물로 기록했다. 이들은 삭제 가능한 로컬 산출물이며 현재 존재를 보장하지 않는다.

원문에 나온 `Doroti/validation/css-fonts/README.md`는 보관 시점 체크아웃에 없다. 위 실행 이름과 PASS는 과거 기록을 보존한 것이며, 삭제된 검증 폴더를 복원하거나 재실행한 것으로 해석하지 않는다.
