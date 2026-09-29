# Doroti 아이콘

Doroti의 공식 아이콘 원본입니다. `other/doroti-intro/exports`에서 선택한 두 SVG를 형태와 색상 변경 없이 보관합니다.

| 아이콘 | 용도 |
| --- | --- |
| ![컬러 아이콘](doroti-symbol-color.svg) [doroti-symbol-color.svg](doroti-symbol-color.svg) | 기본 브랜드 아이콘, 소개 페이지 로고, 파비콘 |
| ![단색 아이콘](doroti-symbol.svg) [doroti-symbol.svg](doroti-symbol.svg) | 단색 툴바, 문서, 단색 인쇄 |

두 파일은 투명 배경의 `24 × 24` SVG이며, 다섯 블록의 겹침과 `0.8` 선 두께를 유지합니다. 컬러는 보라색 `#7843bb`와 금색 `#ffc107`, 단색 기본값은 회색 `#808080`입니다.

단색 SVG는 `currentColor`를 사용하지만 루트에 기본 `color`가 지정되어 있습니다. 인라인 SVG로 넣어 색을 바꾸려면 해당 SVG 요소의 `color`를 지정하세요. `<img>`로 불러오면 부모의 CSS `color`는 내부로 전달되지 않습니다.

소개 페이지는 이 폴더의 컬러 SVG를 직접 참조합니다. 배포 시 Vite가 해당 자산을 포함합니다. `other`는 Git 추적 제외 폴더이므로 공식 아이콘을 수정할 때는 이 폴더를 기준으로 합니다.

## 앱의 기본 아이콘

`Doroti.Runner.Sdk`에 기본 아이콘 자산과 `Doroti.AppIcons.targets`가 포함됩니다. 샘플과 `doroti-app` 템플릿 모두 별도 아이콘을 지정하지 않으면 컬러 심벌을 사용합니다. 파생 파일은 `Doroti/src/Doroti.Runner.Sdk/Sdk/Icons`에 있습니다.

| 플랫폼 | 기본 적용 방식 | 앱별 변경 |
| --- | --- | --- |
| Windows App SDK | 다중 해상도 ICO를 실행 파일에 삽입하고 네이티브 창 클래스에서 로드 | `ApplicationIcon` 또는 `Resources/AppIcon/appicon.ico` |
| Windows MAUI | `MauiIcon`으로 실행 파일·패키지 아이콘 생성 | `MauiIcon` |
| Android | 밝은 배경과 0.65 배율의 전경으로 일반·원형·적응형 아이콘 생성 | `MauiIcon`과 AndroidManifest의 아이콘 이름 |
| iOS / Mac Catalyst | 밝은 불투명 배경과 0.8 배율의 전경으로 앱 아이콘 카탈로그 생성 | `MauiIcon` |
| macOS AppKit | ICNS 번들 리소스와 기본 `CFBundleIconFile` 설정 | `DorotiMacOSIcon` 또는 `Resources/AppIcon/appicon.icns`; 기존 plist 키 우선 |
| Linux Qt | PNG를 Qt 리소스로 포함해 기본 창 아이콘 설정; 배포 폴더에도 `appicon.png` 복사 | `DorotiLinuxIcon` 또는 `Resources/AppIcon/appicon.png` |
| Web | `favicon.svg`를 정적 자산으로 포함; 샘플·템플릿 HTML에서 참조 | `wwwroot/favicon.svg` 또는 HTML의 아이콘 링크 |

`DorotiUseDefaultAppIcon=false`로 SDK 기본값을 끌 수 있습니다. 기존 `MauiIcon` 항목이나 `Resources/AppIcon/appicon.svg`가 있으면 앱의 설정을 우선합니다. 새 템플릿에서는 예전 .NET 아이콘 파일을 제거했습니다. Linux 데스크톱 메뉴 등록용 `.desktop` 파일은 앱 배포자가 작성하며 `Icon`에 설치한 PNG 경로를 지정합니다.

모바일의 밝은 배경은 불투명 아이콘 및 적응형 마스크를 위한 것입니다. 심벌 자체의 색상·선·겹침은 원본과 같습니다. [MAUI 앱 아이콘](https://learn.microsoft.com/dotnet/maui/user-interface/images/app-icons?view=net-maui-10.0)의 플랫폼별 생성 규칙을 따릅니다.

## 파생 자산 재생성

Playwright와 Chromium 또는 Edge가 있는 개발 환경에서 저장소 루트 기준으로 실행합니다. 앱 빌드에는 브라우저나 Node.js가 필요하지 않습니다.

```powershell
node Doroti/eng/generate-app-icons.mjs <Playwright 패키지의 절대 경로> msedge
python Doroti/eng/run-with-timeout.py python Doroti/tests/app_icons.py
```

세 번째 인자를 생략하면 Playwright의 Chromium을 사용합니다. SVG 원본을 변경한 경우 파생 자산을 다시 생성하고 SDK 패키지를 다시 빌드합니다. VS Code 확장의 `images/icon.png`는 별도 256×256 파생 자산입니다.
