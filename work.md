# Doroti Web TypeScript bootstrap 및 browser source 전환

## 상태

계획 작성 완료, 구현은 아직 시작하지 않음. 아래 항목은 구현과 증거 수집 전까지 모두 `notVerified`다.

## 결정

Doroti의 Web 플랫폼 소스는 TypeScript를 제품 소스로 사용하고, 브라우저가 실행하는 JavaScript는 build/publish 산출물로만 만든다.

- 앱 개발자는 `Platforms/Web/src/**/*.ts`를 편집한다.
- `Microsoft.TypeScript.MSBuild 7.0.0`을 Web 전용 compiler/toolchain으로 고정한다.
- Node, npm, Bun, TypeScript npm package, bundler는 필수 도구로 추가하지 않는다.
- TypeScript 7 native compiler가 늘리는 약 54MB의 개발/restore 비용은 허용한다. compiler와 build asset은 앱 publish 결과에 포함하지 않는다.
- 생성된 `.js`와 `.js.map`은 source `wwwroot`에 쓰거나 체크인하지 않고 target/configuration별 `obj` 아래에 둔다.
- 새 bootstrap과 loader뿐 아니라 기존 browser interop 및 plugin 예제도 TypeScript 소유권으로 전환한다. 향후 service worker도 같은 source/build 경계를 사용한다.

Flutter Web의 `flutter_bootstrap.js`처럼 앱이 초기화 정책을 구성할 수 있게 하되, Doroti는 Flutter 엔진이 아니라 Blazor WebAssembly와 정적으로 링크된 SkiaSharp를 사용하므로 Flutter 이름, CanvasKit 옵션, Flutter build token을 복제하지 않는다.

## 목표 구조

```text
doroti-app/
├─ DorotiApp.csproj
├─ Program.cs
├─ src/
│  └─ App.cs
└─ Platforms/
   └─ Web/
      ├─ tsconfig.json
      ├─ src/
      │  ├─ doroti_bootstrap.ts       # 앱 소유 Web 초기화 설정과 hook
      │  └─ plugins/
      │     └─ echo.ts                 # 앱 소유 browser plugin 예제
      └─ wwwroot/
         ├─ index.html                 # compiled bootstrap JS만 참조
         ├─ assets/
         ├─ locales/
         └─ doroti-app-manifest.json

Doroti/src/Doroti.Host.Web/
├─ Web/
│  ├─ doroti.loader.ts                # Doroti 소유, Blazor 정확히 한 번 시작
│  └─ doroti.web.ts                   # DOM/WebGL2/input/IME/accessibility interop
└─ wwwroot/
   └─ doroti.web.css

obj/web/<configuration>/<tfm>/Doroti.Generated/wwwroot/
├─ doroti_bootstrap.js
└─ plugins/
   └─ echo.js

Doroti.Host.Web/obj/<configuration>/<tfm>/Doroti.Web/wwwroot/
├─ doroti.loader.js
└─ doroti.web.js
```

source tree에는 TypeScript와 직접 작성한 HTML/CSS/manifest/resource만 남긴다. publish 결과에는 JavaScript가 다음 URL로 존재한다.

- 앱 bootstrap: `doroti_bootstrap.js`
- 앱 plugin: `plugins/echo.js`
- Doroti loader: `_content/Doroti.Host.Web/doroti.loader.js`
- Doroti browser interop: `_content/Doroti.Host.Web/doroti.web.js`

## 참고한 upstream 계약

- [Flutter Web app initialization](https://docs.flutter.dev/platform-integration/web/initialization): 앱 소유 bootstrap, loader 설정, 시작 단계 callback, loading/error UI 모델을 참고한다.
- [ASP.NET Core Blazor startup](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/startup?view=aspnetcore-10.0): standalone Blazor WebAssembly의 `autostart="false"`와 `Blazor.start(options)`를 실제 실행 계약으로 사용한다.
- [Microsoft.TypeScript.MSBuild](https://www.nuget.org/packages/Microsoft.TypeScript.MSBuild/): `7.0.0` native compiler와 MSBuild integration을 Web build toolchain으로 사용한다.

Flutter와 Doroti의 대응은 다음 범위로 한정한다.

| Flutter 개념 | Doroti 대응 | 비고 |
|---|---|---|
| `flutter_bootstrap.js` source | `Platforms/Web/src/doroti_bootstrap.ts` | 앱 개발자가 편집 |
| build된 `flutter_bootstrap.js` | publish의 `doroti_bootstrap.js` | TypeScript compiler 산출물 |
| `_flutter.loader.load()` | `startDoroti()` | 내부에서 공식 `Blazor.start()`를 정확히 한 번 호출 |
| loader `config` | typed Blazor start option 구성 hook | runtime/resource loading 정책 |
| `onEntrypointLoaded` | `beforeStart`/`afterStarted`/`onStage` | 실제로 관찰 가능한 Blazor 단계만 노출 |
| `{{flutter_build_config}}` | `_framework/blazor.boot.json` 등 Blazor 산출물 | Doroti token 치환을 새로 만들지 않음 |

## TypeScript toolchain 계약

### 패키지와 활성화

- `DorotiTypeScriptVersion`의 기본값은 `7.0.0`으로 고정한다.
- `DorotiWebTypeScript`는 `DorotiTarget=Web`이고 `Platforms/Web/tsconfig.json`이 있을 때 활성화한다.
- `Microsoft.TypeScript.MSBuild`는 해당 조건에서만 `PrivateAssets="all"`로 app graph에 추가한다.
- JS-only 기존 앱은 명시적인 migration 전까지 compiler package를 강제로 받지 않게 한다. 새 template과 DemoApp은 TypeScript 모드를 기본 사용한다.
- package version은 Web 전용 `packages.web.lock.json`에 고정하고 Windows/MacCatalyst lock/restore graph에는 유입시키지 않는다.
- Windows/macOS/Linux x64·arm64의 package compiler executable을 허용 범위로 삼고, 실행 파일을 찾을 수 없는 build host에서는 조용히 건너뛰지 않고 stable `DOROTIWEB` 진단으로 실패한다.

### `tsconfig.json`

template과 DemoApp의 기본 설정은 다음 원칙을 명시한다.

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "ESNext",
    "moduleResolution": "Bundler",
    "lib": ["ES2022", "DOM", "DOM.Iterable"],
    "strict": true,
    "noEmitOnError": true,
    "isolatedModules": true,
    "verbatimModuleSyntax": true,
    "rootDir": "src"
  },
  "include": ["src/**/*.ts"]
}
```

- `outDir`은 앱 source 설정이 아니라 Doroti SDK가 `$(IntermediateOutputPath)` 아래로 강제한다.
- Debug는 source map을 생성·배포할 수 있고 Release는 기본적으로 `.map`을 publish하지 않는다.
- `.ts` import는 browser에서 유효한 최종 `.js` specifier를 사용하며 bundling을 전제로 하지 않는다.
- public loader/plugin ABI에 필요한 `.d.ts`는 Doroti package가 단일 원본으로 제공한다. template에 같은 선언을 복사하여 drift시키지 않는다.
- `Doroti.App.Sdk`는 package의 type declaration 경로를 TypeScript compiler에 전달하고 외부 package-only 소비자에서도 같은 IntelliSense/type-check 결과가 나오게 한다.

### 생성물과 static web asset

`Microsoft.TypeScript.MSBuild`의 기본 compile hook만으로는 build 중 뒤늦게 생성된 JS가 Blazor static asset에 안전하게 포함된다고 가정하지 않는다.

- app output은 `$(IntermediateOutputPath)Doroti.Generated/wwwroot/` 아래에 생성한다.
- `Doroti.Host.Web` output은 자체 `$(IntermediateOutputPath)Doroti.Web/wwwroot/` 아래에 생성한다.
- Doroti MSBuild target은 TypeScript compile 이후, `ResolveCurrentProjectStaticWebAssetsInputs` 전에 생성된 `.js`와 허용된 `.map`을 동적으로 `Content`/static web asset으로 등록한다.
- `EnableDefaultContentItems=false`와 관계없이 `Platforms/Web/tsconfig.json`은 compiler 입력으로 명시 등록하되 publish content에서는 제외한다.
- TypeScript package의 emitted-file/clean 계약은 `obj` 안의 파일만 삭제하게 한다. source `wwwroot`를 output directory로 지정하지 않는다.
- clean build와 첫 publish에서 기존 JS가 미리 존재하지 않아도 동일한 결과가 나와야 한다. 이전 build의 stale JS에 의존하는 구성은 실패로 본다.
- publish에는 `.ts`, `tsconfig.json`, TypeScript compiler binary, MSBuild tool asset, repository 절대 경로가 포함되지 않아야 한다.

## browser bootstrap API

앱의 TypeScript bootstrap은 다음 형태를 기준으로 한다.

```ts
import {
  startDoroti,
  type DorotiBootstrapContext,
} from "./_content/Doroti.Host.Web/doroti.loader.js";

await startDoroti({
  configure(context: DorotiBootstrapContext) {
    // context.blazorOptions.configureRuntime = ...;
    // context.blazorOptions.loadBootResource = ...;
  },
  onStage(stage, context) {
    // "before-start" | "starting" | "started" | "failed"
  },
  onError(error, context) {
    // loading UI와 진단 표시
  },
});
```

`index.html`은 compiled module만 로드한다.

```html
<script src="_framework/blazor.webassembly.js" autostart="false"></script>
<script type="module" src="doroti_bootstrap.js"></script>
```

두 번째 script에는 `async`를 붙이지 않는다. Blazor loader가 준비된 다음 app bootstrap이 실행되는 순서를 유지한다.

API 원칙:

- `doroti.loader.ts`만 `Blazor.start()`를 호출한다.
- `startDoroti()`를 여러 번 호출해도 하나의 시작 Promise만 공유하거나 stable diagnostic으로 실패하며 runtime을 두 번 만들지 않는다.
- `configure`는 시작 전에 한 번 실행되고 typed mutable `blazorOptions`를 제공한다.
- `onStage`는 실제 단계만 알리고 `started`는 `Blazor.start()` Promise가 성공한 뒤에만 발생한다.
- `onError`가 없어도 오류를 삼키지 않고 console과 rejected Promise에 같은 원인을 보존한다.
- custom `loadBootResource`가 `fetch`를 반환하면 전달받은 `integrity`를 보존해야 한다. 기본 경로는 Blazor의 integrity/cache 동작을 바꾸지 않는다.
- loader는 canvas, widget, plugin을 직접 만들지 않는다. managed app이 시작된 뒤의 WebGL2/input/IME/accessibility 연결은 `Doroti.Host.Web`이 계속 소유한다.
- 외부 ES module을 사용해 inline bootstrap을 피한다. CSP nonce, CDN, service worker 같은 앱 정책은 typed hook에서 명시적으로 구성할 수 있게 한다.

## 구현 계획

### W1. TypeScript MSBuild 기반과 fail-closed 진단

대상:

- `Doroti/Directory.Packages.props`
- `Doroti/src/Doroti.App.Sdk/Sdk/Sdk.props`
- `Doroti/src/Doroti.App.Sdk/Sdk/Sdk.targets`
- `Doroti/src/Doroti.Target.Web.browser-wasm/buildTransitive/Doroti.Target.Web.browser-wasm.targets`

작업:

- repository product와 외부 SDK 소비자가 같은 `Microsoft.TypeScript.MSBuild 7.0.0`을 사용하도록 version/property 계약을 추가한다.
- Web + tsconfig 존재 조건에서만 compiler package와 compile target을 활성화한다.
- TypeScript output을 target/configuration별 `obj`로 강제하고 generated JS를 static web asset resolve 전에 등록한다.
- compiler executable, tsconfig, TypeScript source, bootstrap output 누락을 각각 stable `DOROTIWEB` 진단으로 닫는다.
- Web target graph에 compiler version, config, source count, generated JS root를 기록한다.
- unsupported host, TypeScript syntax/type 오류, empty emit, source `wwwroot` emit을 negative fixture로 만든다.

완료 조건:

- clean `dotnet build` 한 번으로 TypeScript compile과 Web static asset 생성이 끝난다.
- TypeScript 오류가 있어도 C# build만 성공하는 false PASS가 없다.
- Windows/MacCatalyst build는 TypeScript compiler를 restore하거나 실행하지 않는다.
- `dotnet clean`은 `obj`의 emitted JS만 제거하고 `.ts`, HTML, CSS, manifest/resource를 보존한다.

### W2. Doroti loader를 TypeScript로 구현

대상:

- `Doroti/src/Doroti.Host.Web/Web/doroti.loader.ts` 추가
- loader public type declaration/package wiring
- Web bootstrap 계약 문서 또는 ADR

작업:

- `startDoroti(options)`의 typed option, 정확히 한 번 시작, stage 전이, 오류 전파를 구현한다.
- `globalThis.Blazor`와 `Blazor.start`가 없으면 stable diagnostic으로 즉시 실패시킨다.
- 시작 Promise, callback 호출 순서, error identity를 contract fixture로 고정한다.
- compiler가 만든 `doroti.loader.js`와 public `.d.ts`가 `Doroti.Host.Web` NuGet/static web asset에 포함되게 한다.
- 앱 bootstrap의 relative runtime import가 외부 template build에서도 type-check되도록 package-owned ambient/module declaration을 연결한다.

완료 조건:

- source tree에는 `doroti.loader.js`가 없고 package/publish에는 compiled JS가 있다.
- 앱 bootstrap은 `Blazor.start()`를 직접 호출하지 않는다.
- callback 예외와 Blazor 시작 실패가 `failed`, `onError`, rejected Promise에 동일한 원인으로 남는다.

### W3. 기존 browser interop을 TypeScript 소유권으로 전환

대상:

- `Doroti/src/Doroti.Host.Web/wwwroot/doroti.web.js` 제거
- `Doroti/src/Doroti.Host.Web/Web/doroti.web.ts` 추가
- managed callback, host snapshot, plugin request/response type

작업:

- 현재 export 이름과 JS interop ABI를 그대로 유지하면서 DOM/WebGL2/input/IME/clipboard/semantics 코드를 strict TypeScript로 옮긴다.
- `any`로 전체 경계를 우회하지 않고 managed callback, host state, GPU identity, pointer sample, text editing payload를 명시적으로 타입화한다.
- `HTMLCanvasElement`, `HTMLTextAreaElement`, `ResizeObserver`, Pointer/Composition/Keyboard event narrowing을 실제 DOM type으로 검증한다.
- runtime URL과 exported function 이름은 기존 C# consumer 및 evidence와 동일하게 유지한다.
- 변환 전후의 export inventory와 대표 payload를 contract fixture로 비교한다.

완료 조건:

- checked-in `doroti.web.js`가 0건이고 compiled `_content/Doroti.Host.Web/doroti.web.js`가 존재한다.
- C# JS import/export 이름, codec, payload shape가 변하지 않는다.
- WebGL2 hardware-only, retained-scene replay, pointer/IME/accessibility 기존 범위를 약화시키지 않는다.

### W4. template과 DemoApp을 TypeScript Web source로 전환

대상:

- template/DemoApp의 `Platforms/Web/tsconfig.json`
- template/DemoApp의 `Platforms/Web/src/doroti_bootstrap.ts`
- template/DemoApp의 `Platforms/Web/src/plugins/echo.ts`
- template/DemoApp의 `Platforms/Web/wwwroot/index.html`
- 기존 `Platforms/Web/wwwroot/plugins/echo.js` 제거

작업:

- 두 index에서 Blazor 자동 시작을 끄고 compiled app bootstrap module을 로드한다.
- template bootstrap에는 no-op 기본 구성과 최소 stage/error 예시만 둔다.
- DemoApp bootstrap에는 live validator가 읽을 수 있는 `document.documentElement.dataset.dorotiBootstrapStage` marker를 둔다.
- `DorotiJavaScriptPlugin ModuleUrl="./plugins/echo.js"`는 runtime URL로 유지하되 source는 `echo.ts` 한 곳만 소유한다.
- template과 DemoApp의 tsconfig/bootstrap/plugin 계약 drift를 validator에서 검사한다.
- `<base href>`를 존중하는 상대 URL만 사용하고 `/` 고정 경로를 추가하지 않는다.

완료 조건:

- 외부 `dotnet new doroti-app` 결과에 `.ts` source와 tsconfig가 있으며 생성 `.js`는 없다.
- build 후 `obj/web/**`와 publish에만 app/plugin JS가 존재한다.
- Windows/MacCatalyst compile/content graph에는 Web TypeScript source와 output이 유입되지 않는다.

### W5. bootstrap 구성과 실패 경로 검증

대표 fixture/live 시나리오:

- 기본 시작: `before-start -> starting -> started`가 한 번씩 발생하고 managed Doroti app이 mount된다.
- runtime 구성: typed `configure`에서 `configureRuntime` 또는 안전한 marker를 설정하고 적용을 확인한다.
- boot resource 관찰: `loadBootResource`가 기본 URI와 integrity를 훼손하지 않고 요청을 관찰한다.
- 진행 UI: stage callback이 `#app` 또는 dataset marker를 갱신하고 최종 `started`에 도달한다.
- 사용자 오류: `configure`/callback 예외가 `failed`, `onError`, rejected Promise에 남고 무한 Loading 상태가 되지 않는다.
- 중복 시작: 두 번째 호출이 별도 runtime을 만들지 않는다.
- compiler 오류: 잘못된 TypeScript가 `dotnet build`를 실패시키고 stale JS로 publish되지 않는다.
- clean-first publish: 사전 생성 JS가 없는 상태에서도 compile 후 올바른 static asset을 얻는다.

완료 조건:

- TypeScript compile, bootstrap hook, managed mount를 각각 관찰한다.
- `started`만으로 WebGL2 presentation을 주장하지 않고 canvas presentation/basic pointer는 별도 Web live assertion으로 확인한다.
- keyboard/IME/clipboard/resize/interactive ARIA 및 physical/cross-target 항목은 실행하지 않았다면 계속 `notVerified`로 기록한다.

### W6. package/publish evidence와 문서 동기화

대상:

- `Doroti/eng/validate-app-targets.ps1`
- `Doroti/eng/validate-web-product.ps1`
- Web artifact/evidence schema
- `Doroti/README.md`, `Doroti/README.ko.md`
- `DorotiDemoApp/README.md`, `DorotiDemoApp/README.ko.md`
- template 설명과 Web bootstrap ADR/문서

작업:

- publish 결과의 framework loader, app bootstrap, Doroti loader, browser interop, plugin을 별도 artifact로 기록한다.
- `.ts` source identity와 compiled `.js` hash를 연결하되 repository 절대 경로를 evidence에 넣지 않는다.
- 반복 publish identity와 저장소 밖 package-only template create/restore/build/publish를 TypeScript toolchain까지 확장한다.
- 앱 개발자가 수정하는 `.ts`, Doroti가 소유하는 host `.ts`, SDK가 생성하는 JS/C# bootstrap을 분리해 설명한다.
- 기본 설정, loading UI, `configureRuntime`, integrity를 보존하는 `loadBootResource`, 향후 service worker 등록 예제를 제공한다.
- Flutter 문서는 API/UX 참고이며 실제 실행 계약은 Blazor 공식 API임을 명시한다.

## 검증 게이트

repository 지침에 따라 각 테스트 실행에는 20분 timeout을 적용한다.

1. 구조/소유권
   - template, DemoApp, `Doroti.Host.Web`의 checked-in generated JS 0건
   - TypeScript source/tsconfig는 Web scope에만 존재
   - generated JS는 target/configuration별 `obj`와 publish에만 존재
   - Flutter token, CanvasKit, Node/npm/Bun/bundler 신규 의존 0건
2. compiler/fail-closed
   - compiler `7.0.0`, strict/noEmitOnError, source count, output root 확인
   - syntax/type 오류, compiler 누락, empty emit, stale output negative fixture
   - clean이 source를 삭제하지 않고 generated output만 제거하는지 확인
3. 제품 빌드
   - `pwsh -NoProfile -File ./Doroti/eng/doroti.ps1 validate -ValidationSuite Developer`
   - Windows, MacCatalyst cross-build, Web Release build
   - native target restore/lock에 TypeScript package가 유입되지 않는지 확인
4. package/publish
   - `validate-web-product.ps1`의 Template, Compile, Publish shard
   - 저장소 밖 template install/create/restore/build/publish
   - clean-first 및 반복 publish static identity
   - publish의 `.ts`, tsconfig, compiler/tool binary, repository-private fallback 0건
5. browser live
   - Chromium에서 stage 순서, managed mount, console/unhandled rejection 0건
   - `_content` loader/interop와 app/plugin compiled module load 확인
   - 기존 WebGL2 canvas presentation과 기본 pointer `fab=0 -> fab=1` 별도 재확인
   - 오류/중복 시작 fixture 결과를 정상 live evidence와 분리
6. 마감
   - `git diff --check`
   - Web product evidence와 영문/한국어 문서 갱신
   - 실행하지 않은 Mac Catalyst native/physical/cross-target gate는 `notVerified` 유지

## 이번 범위에서 하지 않을 것

- Flutter 이름, `_flutter`, Flutter build token, CanvasKit 설정을 제공하지 않는다.
- TypeScript 도입을 이유로 React/Vue/Vite/Webpack/esbuild 같은 framework/bundler를 추가하지 않는다.
- service worker/PWA 동작 자체를 이번 단계에서 구현하지 않는다. 이후 TypeScript source로 추가할 수 있는 build와 bootstrap hook만 보장한다.
- TypeScript compiler나 generated JS를 앱 NuGet source 자산으로 노출하거나 source tree에 되쓰지 않는다.
- 기존 C# application bootstrap과 plugin registration을 JavaScript/TypeScript로 옮기지 않는다. 이들은 계속 `Doroti.App.Sdk`가 `obj/<target>/Doroti.Generated/`에 생성한다.
- bootstrap 성공을 canvas/input/IME/accessibility/physical acceptance 증거로 확대하지 않는다.

## 예상 완료 상태

작업이 끝나면 사용자는 `Platforms/Web/src/doroti_bootstrap.ts`와 plugin TypeScript에서 runtime 옵션, resource loading, loading/error UI, 시작 전후 hook을 타입 안전하게 구성할 수 있어야 한다. Doroti의 loader와 browser interop도 TypeScript를 제품 원본으로 사용하며, 모든 실행 JavaScript는 격리된 build output에서 생성되어 Blazor static web asset으로 배포된다. Windows/MacCatalyst 앱 graph와 공통 `Program.cs`는 이 Web toolchain을 알 필요가 없다.
