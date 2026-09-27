# Doroti 웹폰트 지원 작업계획

검토일: 2026-09-28. 이번 변경은 조사와 계획 작성이며, 아래 구현 항목은 아직 수행하지 않았다.

## 1. 결론과 목표

**지원 가능하다.** 기존 폰트 바이트 등록 경로를 확장하여 다음 두 가지 사용법을 지원한다.

1. 앱의 `web/wwwroot/index.html`에 일반적인 폰트 CSS `<link>`를 추가한 뒤, Doroti `TextStyle.fontFamily` 또는 테마에서 해당 family를 선택한다.
2. 앱에 WOFF/WOFF2 파일을 포함하고, 로컬 CSS 또는 기존 `BrowserFontAsset`으로 등록하여 사용한다.

CSS 링크를 추가하면 폰트가 **사용 가능한 상태로 등록**되는 것이 목표다. 화면 전체 기본 폰트 변경은 Doroti 테마/기본 family 설정으로 지정한다. HTML의 `body { font-family: ... }`를 Doroti 위젯 스타일로 상속시키는 기능은 이번 범위에 포함하지 않는다.

핵심 경로는 `CSS/에셋 선언 → 실제 폰트 바이트 → 필요 시 압축 해제 → Skia 등록 → 측정·렌더링`이다. 브라우저 CSS 폰트 로딩 완료만으로 Doroti의 Skia 렌더러가 그 폰트를 사용할 수 있는 것은 아니다.

## 2. 현재 코드에서 확인한 상태

| 영역 | 현재 상태 | 필요한 작업 |
| --- | --- | --- |
| URL/내장 에셋 | `BrowserFontAsset`이 URL과 `EmbeddedResource`를 지원 | 기존 API 유지, CSS 선언과 공통 로더로 연결 |
| 시작 시 등록 | `BrowserStartupFonts`가 첫 뷰 생성 전에 다운로드·등록 | CSS에서 발견한 폰트도 시작 단계에 합류 |
| WOFF2 | `wOF2` 시그니처를 확인하고 `BrowserWoff2Decoder`로 해제 | 실제 SUITE 파일 검증, 로컬 디코더 패키징 개선 |
| WOFF 1 | 명시적 `wOFF` 정규화 분기 없음 | 실제 웹 빌드의 직접 디코딩 확인 후 필요 시 해제 구현 |
| CSS 링크 | `@font-face` 수집 및 worker 전달 경로 없음 | HTML/CSS와 폰트 로더 사이 연결 추가 |
| family/face 선택 | alias, 파일 내부 weight/slant를 사용 | CSS descriptor, width, 가변 축, subset별 coverage 반영 |
| 가변 폰트 | `Doroti.Ui.TextStyle.fontVariations`는 있으나 현재 Skia 텍스트 자원 생성/키에 축 반영 없음 | 실제 가변 typeface 생성과 측정·캐시·직렬화 경로 연결 |
| 변경 통지 | 등록 후 캐시 무효화 및 `fontsChange` 경로 존재 | CSS 등록도 같은 경로 사용, 여러 face 변경은 묶어서 통지 |
| 외부 통신 없는 폰트 모드 | `AssetsOnly`는 기본/폴백/디코더 CDN을 끔 | 로컬 WOFF2 사용 시 디코더와 부속 WASM도 명시적으로 앱에 포함 |

주요 근거 파일:

- `Doroti/src/Doroti.Host.Web/Fonts/BrowserFontAsset.cs`
- `Doroti/src/Doroti.Host.Web/Fonts/BrowserStartupFonts.cs`
- `Doroti/src/Doroti.Host.Web/Fonts/BrowserFontFallbackLoader.cs`
- `Doroti/src/Doroti.Host.Web/Fonts/BrowserWoff2Decoder.cs`
- `Doroti/src/Doroti.Host.Web/Web/doroti.web.fonts.ts`
- `Doroti/src/Doroti.Host.Web/BrowserFrameworkHost.cs`
- `Doroti/src/Doroti.Target.Web.browser-wasm/DorotiWebWorkerRunner.cs`
- `Doroti/src/Doroti.Skia.Rendering/SkiaFallbackFontCollection.cs`
- `Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs`
- `samples/DorotiSampleApp2/web/WebFonts.cs`

### 실제 입력 파일 확인

- 요청한 [Galmuri CSS](https://cdn.jsdelivr.net/npm/galmuri/dist/galmuri.css)를 직접 조회했다. 현재 응답의 패키지 버전은 `2.40.3`, CSS 응답은 `Access-Control-Allow-Origin: *`, `Cross-Origin-Resource-Policy: cross-origin`이다. 버전 미지정 URL의 내용은 이후 바뀔 수 있으므로 재현용 fixture는 버전을 고정한다.
- CSS에는 Galmuri11의 400/700 및 condensed face, Galmuri14/9/7, Mono 계열이 있다. `local(...)`, 상대 경로 `.woff2`, `.ttf` 순으로 source 후보를 선언한다. CSS URL 기준 상대 경로 처리와 face 구분이 필요하다.
- `reference/SUITE-Variable-woff2/SUITE-Variable.css`는 family `SUITE Variable`, weight `300 900`, `format('woff2-variations')`를 선언한다.
- 해당 로컬 WOFF2를 fontTools로 읽은 결과: 535,788바이트, `wght` 축 최소 300/기본 300/최대 900, cmap 항목 2,933개. 내부 이름은 `SUITE Variable` 및 `SUITE Variable Light`다. CSS alias를 보존하고 요청 weight를 축에 적용해야 한다. 파일을 등록하기만 하면 정상적인 400/700이 구현되었다고 볼 수 없다.
- 폰트 메타데이터만 확인했으며, 이번 조사에서 Doroti 브라우저 화면 렌더링이나 전체 한글 coverage를 검증한 것은 아니다.

## 3. 웹 문서 검토 결과

### CSS 폰트와 Skia 폰트는 별도로 연결해야 한다

CSS Font Loading API는 `FontFace`를 URL/ArrayBuffer로 생성하고 document/worker의 폰트 집합에 추가하는 기능을 제공한다. 공개 `FontFace` API에는 이미 로드된 폰트를 원본 바이트로 추출하는 기능이 없다. 따라서 `document.fonts.ready`만 기다리는 방식으로는 해결되지 않는다. CSS의 source URL을 읽어 바이트를 가져오거나, 앱이 제공한 바이트를 등록해야 한다. [MDN CSS Font Loading API](https://developer.mozilla.org/en-US/docs/Web/API/CSS_Font_Loading_API), [MDN FontFace](https://developer.mozilla.org/en-US/docs/Web/API/FontFace).

### 일반적인 외부 CSS 링크도 지원하되 CORS를 지켜야 한다

외부 stylesheet의 `cssRules`는 origin-clean 조건을 충족하지 않으면 `SecurityError`를 낸다. CSSOM으로 읽을 수 있으면 사용하고, 읽을 수 없는 일반 `<link>`는 해당 URL을 CORS fetch하여 해석하는 보완 경로가 필요하다. `crossorigin` 속성을 사용자의 필수 설정으로 만들지 않는다. 단, 서버가 CORS fetch를 허용하지 않으면 임의의 외부 CSS를 읽을 수는 없다. 이때 자체 호스팅/명시적 에셋 등록을 안내한다. [CSSOM cssRules 규정](https://drafts.csswg.org/cssom/#dom-cssstylesheet-cssrules).

CSS source는 여러 후보, `local()`, format/tech 힌트를 포함한다. Doroti의 일반 웹 환경에서는 로컬 OS 폰트의 바이트를 얻을 수 없으므로 `local()`을 건너뛰고 지원 가능한 URL 후보를 순서대로 시도한다. `woff2-variations` 같은 기존 표기도 받아들여야 한다. [MDN @font-face src](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/At-rules/@font-face/src).

### WOFF/WOFF2와 가변 폰트 처리는 별개의 문제다

WOFF 1과 WOFF2는 별도 컨테이너 형식이다. WOFF2는 Brotli 및 폰트 테이블 변환을 포함하므로 범용 압축 해제만으로 처리하면 안 된다. 기존 전용 디코더를 재사용하고, WOFF 1은 별도로 확인한다. [W3C WOFF 1](https://www.w3.org/TR/WOFF/), [W3C WOFF2](https://www.w3.org/TR/WOFF2/).

가변 weight 범위 선언은 파일의 `wght` 축 선택과 연결되어야 한다. Skia에는 가변 좌표를 지정한 typeface clone 기능이 있으며, 최신 SkiaSharp 문서에도 해당 API가 있다. 저장소에서 사용하는 패키지/네이티브 바이너리의 실제 제공 여부를 먼저 확인한다. [MDN font-weight descriptor](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/At-rules/@font-face/font-weight), [Skia SkTypeface](https://api.skia.org/classSkTypeface.html), [SkiaSharp SKTypeface.Clone](https://learn.microsoft.com/en-us/dotnet/api/skiasharp.sktypeface.clone).

공식 사용 예에서도 Galmuri와 SUITE는 CSS 링크로 배포된다. 이 선언을 Doroti 폰트 로더에 연결하는 방식을 채택한다. [Galmuri 공식 저장소](https://github.com/quiple/galmuri), [SUITE 공식 저장소](https://github.com/sun-typeface/SUITE).

## 4. 목표 사용법

### A. 외부 CSS 링크

구현 후 다음 HTML을 그대로 사용할 수 있게 한다. 별도 Doroti 전용 HTML 속성이나 C# 폰트 목록 중복 작성은 요구하지 않는다.

```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/galmuri/dist/galmuri.css">
```

Doroti 위젯에서는 `fontFamily: "Galmuri11"`을 지정하고, 해당 family의 regular/bold를 `fontWeight`로 선택한다. 앱 테마의 기본 family로도 선택할 수 있어야 한다.

### B. 로컬 CSS + WOFF2

앱의 `wwwroot/fonts/SUITE/` 아래에 원본 CSS, WOFF2, LICENSE를 포함한다. `reference/` 폴더 자체는 배포 에셋이 아니므로 샘플 프로젝트에서 복사/링크 포함을 명시한다.

```html
<link rel="stylesheet" href="fonts/SUITE/SUITE-Variable.css">
```

기존 CSS의 `./SUITE-Variable.woff2`를 올바르게 해석하고 `fontFamily: "SUITE Variable"`로 사용한다. 300/400/500/700/900은 실제 가변 축으로 렌더링한다.

### C. CSS 없는 명시적 에셋 등록

다음은 **이미 있는 API**다. WOFF2 해제용 `DecoderUrl`이 구성되어야 하며, 가변 굵기의 완전한 적용은 아래 구현 작업이 필요하다.

```csharp
new BrowserFontFallbackOptions
{
    Assets = [new BrowserFontAsset("SUITE Variable", "fonts/SUITE/SUITE-Variable.woff2")],
};
```

`BrowserFontAsset.Embedded(...)`도 동일한 처리 경로로 지원한다. URL 방식과 내장 리소스 방식을 모두 샘플/문서에 남긴다. 외부 통신 없이 사용하려면 `AssetsOnly(...) with { DecoderUrl = 로컬디코더절대Uri }` 형태로 구성하고 디코더의 JS/WASM 의존 파일까지 포함한다. 폰트 파일만 로컬에 있다고 외부 요청이 사라지는 것은 아니다.

## 5. 구현 단계

### 1단계 — 실제 폰트와 백엔드 지원 범위 확정

- [ ] Galmuri regular/bold WOFF2·TTF, SUITE Variable WOFF2, 유효한 WOFF 1을 검증 입력으로 확보한다. 원본 라이선스와 버전/해시를 기록한다.
- [ ] 현재 browser-wasm SkiaSharp 빌드에서 TTF/OTF/WOFF/WOFF2 직접 등록 가능 여부를 작은 로딩 검증으로 구분한다. CanvasKit의 지원 결과를 Doroti의 SkiaSharp 지원 결과로 간주하지 않는다.
- [ ] SUITE 해제 후 `fvar`/`gvar` 등 가변 정보 보존과 SkiaSharp clone API/네이티브 export를 확인한다. API가 없으면 바인딩 확장 또는 호환 버전 도입을 별도 변경으로 처리한다.
- [ ] 앱 상대 URL을 실제 worker fetch로 읽어 200·정상 바이트·시그니처를 검증한다. HTML fallback 응답, 빈 응답, 하위 경로 배포를 확인한다.

완료 기준: 포맷별 성공/실패 원인이 확인되고, SUITE를 원하는 축 좌표로 생성할 수 있는 구현 경로가 확정된다.

### 2단계 — CSS 폰트 선언 수집

- [ ] 메인 스레드에 수집 모듈을 추가한다. 시작 시 활성 `<link rel="stylesheet">`, `<style>`, 접근 가능한 stylesheet를 조사하여 `@font-face`를 추출한다.
- [ ] 일반 외부 링크의 CSSOM 접근 실패 시 CORS fetch를 수행한다. fetch의 최종 응답 URL을 기준으로 상대 폰트 URL과 `@import`를 해석한다. 인라인 CSS는 `document.baseURI` 기준으로 해석한다.
- [ ] family, weight 범위, style, stretch, unicode-range, source 후보, variation 설정, 원본 CSS URL을 공통 descriptor로 보존한다. CSS alias와 폰트 내부 family 이름을 구분한다.
- [ ] CSS 전체를 정규식 하나로 파싱하지 않는다. 검증된 파서 또는 CSSOM을 이용하되 `@import`, 중첩 조건, 주석/escape/따옴표/URL 쉼표를 다룬다. `CSSStyleSheet.replace()`는 `@import`를 제거하므로 import 탐색을 별도 처리한다.
- [ ] `@media`/`@supports`와 disabled stylesheet의 활성 여부, import 순환/깊이/중복을 처리한다. 지원하지 않는 descriptor는 진단에 남기고 완전한 CSS 동작을 지원한다고 표기하지 않는다.
- [ ] 신규 옵션 후보 `DiscoverCssFonts`로 자동 발견을 제어한다. 일반 모드는 기본 활성화하고 `AssetsOnly`는 자동 외부 CSS 탐색을 비활성화한다. 로컬 CSS를 쓰는 폐쇄형 구성에는 명시적인 로컬 stylesheet 목록/동일 출처 제한을 제공한다. 옵션명은 구현 시 확정한다.

완료 기준: 사용자가 제시한 Galmuri 링크와 원본 SUITE CSS에서 올바른 descriptor·절대 URL을 생성한다.

### 3단계 — worker 전달과 공통 다운로드/등록

- [ ] `doroti.web.ts` 및 실제 시작 메시지 경로에서 descriptor를 render worker로 전달한다. DOM 접근은 메인 스레드에서, 디코딩/Skia 등록은 폰트 컬렉션 소유 스레드에서 수행한다.
- [ ] `DorotiWebWorkerRunner`/`BrowserStartupFonts`에 CSS 폰트를 합류시킨다. 명시적 앱 에셋을 우선하고, CSS face는 문서 순서를 보존한다. 기본 family 유효성 검사를 CSS 등록 목록까지 포함하도록 수정한다.
- [ ] 최초 버전은 시작 시 발견한 유효 face를 제한된 동시성으로 로드하여 첫 레이아웃 전에 등록한다. CSS `font-display: swap`과 같은 브라우저 표시 시간 정책까지 동일하게 구현했다고 주장하지 않는다.
- [ ] 파일 다운로드·디코딩은 URL/내용 기준으로 공유하고, alias/style별 등록은 별도로 유지한다. CSS source 후보 실패 시 다음 지원 후보로 넘어간다.
- [ ] WOFF2는 기존 디코더를 공통 경로로 정리한다. WOFF 1 직접 지원이 없으면 검증된 해제 경로로 SFNT를 복원한다. 시그니처, 길이, 출력 크기, 손상 데이터, 취소/시간 제한을 검증한다.
- [ ] 암묵적으로 발견한 CSS의 실패는 출처/family/이유를 진단하고 기존 폴백으로 계속한다. 앱이 필수로 지정한 에셋·기본 폰트 실패는 명시적 시작 오류로 유지한다.
- [ ] 등록 완료 시 기존 `Changed`/`fontsChange` 경로를 이용한다. 여러 face 등록은 가능한 한 통지를 묶고, dispose 이후 지연 등록을 막는다.
- [ ] CSP의 `style-src`/`font-src` 외에 직접 fetch의 `connect-src`, 디코더 모듈의 `script-src` 및 WASM 정책, 현재 worker의 COOP/COEP 조건을 실제 환경에서 확인한다.

완료 기준: CSS 링크만 추가한 Galmuri와 로컬 SUITE가 Doroti 텍스트에서 실제로 선택되며, 에셋 모드에서는 의도하지 않은 외부 요청이 없다.

### 4단계 — face 선택과 가변 굵기 적용

- [ ] CSS weight/style/stretch와 파일 메타데이터를 구분해서 등록한다. Galmuri11의 normal/condensed가 같은 weight라는 이유로 잘못 선택되지 않도록 width matching을 반영한다. stretch를 지정할 공개 API가 없다면 normal 선택을 보장하고 condensed 선택 지원 범위를 명시한다.
- [ ] family 요청과 glyph coverage를 함께 매칭한다. 동일 family의 여러 `unicode-range` subset이 뒤에 등록된 face 하나로 덮이지 않도록 한다.
- [ ] `fontWeight`를 `wght` 좌표에 연결하고 `fontVariations`의 명시적 축 설정 우선순위 및 축 범위 처리를 정의한다. SUITE 기본 좌표 300 대신 요청한 400을 실제로 생성한다.
- [ ] variation 좌표를 정규화하여 typeface/text/paragraph/picture 캐시 키와 필요한 worker 직렬화에 포함한다. 측정과 그리기에서 같은 typeface를 사용한다.
- [ ] 가변 축으로 처리한 굵기에 synthetic bold를 중복 적용하지 않는다. clone 캐시는 상한/해제를 갖추고 등록 변경 시 관련 캐시를 무효화한다.

완료 기준: SUITE의 300/400/500/700/900과 명시적 중간 `wght` 좌표가 실제 outline에 반영되고, 폭·줄바꿈·커서/선택 영역이 표시 결과와 맞는다.

### 5단계 — 샘플과 사용 문서

- [ ] SampleApp2에 Roboto/Galmuri11/SUITE Variable을 전환하고 굵기를 비교하는 화면을 추가한다. 한글/영문/숫자/줄바꿈과 편집 가능한 입력을 포함한다.
- [ ] CSS CDN, 로컬 CSS+WOFF2, URL 에셋, 내장 리소스 예제를 제공한다. 기존 `DorotiSampleWebFontSource=Assets` 사용법과 기본 CDN 모드가 계속 동작하게 한다.
- [ ] 로컬 decoder 패키징을 opt-in으로 제공하고 JS 상대 import/WASM 자산도 함께 publish되도록 한다. 기본 웹 패키지의 외부 폰트 번들 정책과 혼동하지 않는다.
- [ ] `Fonts/README.md`에 family 선택, 가변 굵기, CORS 오류, 하위 경로 배포, 폴백, 지원 CSS 범위, 라이선스 포함 방법을 기록한다.

## 6. 검증과 완료 기준

`.github/copilot-instructions.md`에 따라 모든 테스트 실행은 **20분(1,200초) timeout**을 적용한다. 기존 `Doroti/validation/run-with-timeout.py` 사용법을 확인하여 실행하고, 수백 회 반복 검증은 하지 않는다.

| 검증 | 통과 조건 |
| --- | --- |
| CSS 수집 | Galmuri 원본 링크, SUITE 원본 CSS, 상대 URL, import, CORS 접근 실패, 여러 source 후보를 올바르게 처리 |
| 포맷 | 실제 TTF/OTF/WOFF/WOFF2 로드 성공 및 손상 파일의 명확한 오류; WOFF2 mock 성공만으로 완료 처리하지 않음 |
| 가변 축 | SUITE 300/400/500/700/900 및 중간 좌표의 typeface 좌표·glyph outline/픽셀 차이 확인 |
| family와 subset | alias, regular/bold/condensed 선택, 동일 family subset의 glyph coverage와 폴백 순서 확인 |
| 실제 화면 | 브라우저의 Doroti 캔버스에서 Galmuri/SUITE 서체 확인; DOM 텍스트만 바뀐 결과는 제외 |
| 입력/재레이아웃 | 실제 입력 경로로 한글 IME, 커서, 선택 영역, 줄바꿈 확인; 폰트 변경 이후 측정·표시 일치 |
| 배포 | 루트 및 `/sample/` 같은 하위 경로, 개발 서버와 publish 결과에서 폰트·decoder 바이트 정상 제공 |
| 외부 통신 없는 폰트 구성 | 앱/디코더를 로컬 제공하고 외부 origin을 차단해 폰트가 표시됨; 네트워크 기록으로 외부 폰트·decoder 요청 0건 확인 |
| 기존 동작 | `font-assets`, `font-downloads`, `font-resolution`의 관련 회귀 검증 및 기본 Roboto/Noto·아이콘 표시 유지 |

빌드 성공, 메타데이터 검사, 브라우저 실제 표시 결과를 구분하여 기록한다. 최종 완료는 사용자가 제시한 HTML 링크 방식과 로컬 SUITE WOFF2 방식이 모두 실제 Doroti 화면에서 동작하고 가변 굵기까지 확인된 시점이다.

## 7. 후속 확장 범위

첫 완료 범위는 시작 시 선언된 CSS/에셋과 SUITE의 실제 가변 굵기다. 이후 동적 `<link>` 삽입·CSSOM 변경 감지/재스캔 API, family/weight/문자별 지연 다운로드를 추가할 수 있다. 대규모 unicode-range CSS는 최초 버전의 전체 preload가 비효율적일 수 있으므로 전송량/시작 시간을 측정하고 한계를 명시한다. 원격 폰트를 무제한 미리 받거나 브라우저 CSS 전체와 동일하다고 간주하지 않는다.
