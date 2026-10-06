# 브랜딩 아이콘 검증

플랫폼별 기본값, 변경 방법, 재생성 명령은 [아이콘 안내](README.md)에 있습니다.

## 브랜드 색상 변경 · 2026-10-06

컬러 심벌은 `#F6F5F5`, 앱 아이콘 배경과 웹 시작 화면은 `#2b42d4`로 변경했습니다. 앱 아이콘의 흰색 심벌, 컬러 심벌의 금색 블록, 도형과 cutout 간격을 유지했습니다. 플랫폼 PNG·ICO·ICNS·파비콘과 VS Code 확장 PNG를 재생성하고 기본 아이콘 설정과 생성 도구도 갱신했습니다.

| 범위 | 확인한 결과 |
| --- | --- |
| SVG·생성 도구 | PASS: 이전 SVG와 비교해 지정 색상만 변경, 단색 SVG 바이트 유지. 현재 SVG의 패스 정의로 변환기 출력을 재생성해 바이트 일치 확인 |
| 파생 자산 | PASS: 앱 PNG의 새 불투명 배경, 흰색 심벌과 안티앨리어싱 경계 유지, 모바일 투명 전경 픽셀 일치, VS Code PNG의 새 브랜드 색·기존 금색·투명도 확인 |
| 플랫폼 기본값 | PASS: `app_icons.py`의 플랫폼별 선택·직접 지정 우선·비활성화·PNG/ICO/ICNS 구조 검사 |
| 소개 페이지 | PASS: 빌드와 SVG 내보내기 테스트 3개. 컬러 로고·외곽선·슬라이더에 새 브랜드 색 반영 |
| 패키지·앱 실행 | notVerified: target 패키지 재빌드·설치·각 OS의 실제 아이콘 표시는 수행하지 않음 |

검사는 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 실행했습니다. 전달용 `doroti-outline (1).svg`가 현재 작업 폴더에 없어 원본 파일을 사용하는 `branding --check` 대신 현재 SVG 패스 기반 재생성을 확인했습니다. 아래 기록은 이전 색상과 도안에 대한 검증입니다.

## 윤곽 패스 교체 · 2026-10-05

`doroti-outline (1).svg`의 다섯 블록 패스를 기존 filled-cutout 스타일로 변환해 세 공식 SVG를 교체했습니다. 원본 패스의 비율·정점 순서·모서리 형태와 겹침 순서를 유지하고, `24 × 24` viewBox 중앙에 배치했습니다. 앱·컬러·단색 파일의 기존 색상, 투명 배경 여부와 `0.8` cutout 경계도 유지합니다.

원본 윤곽 SVG의 SHA-256은 `583c8b9cde6a8a91421580c6de2079300f196c79ac82824db8e21a606774b945`, 생성한 앱 SVG는 `0958e484df1791098ef515600e82ef7b82df31ee5f0de974c44c4bd1ce99cd7d`입니다. Python 변환 도구는 `other/doroti-multi-tool`에 있습니다.

| 범위 | 확인한 결과 |
| --- | --- |
| SVG 구조·도형 | PASS: 원본과 생성 패스의 정점 수·비율·순서 일치, 세 파일의 도형 일치, 내부 참조와 다섯 cutout mask 검사, 재생성 결과 바이트 일치 |
| 렌더링 | PASS: Edge로 원본과 교체 전후 세 SVG를 렌더링하고 둥근 모서리·색상·투명 간격 시각 확인 |
| 파생 자산 | PASS: `packages/platforms/build/Icons`의 PNG·ICO·ICNS·파비콘 및 모바일 배경·전경, VS Code 확장 PNG 재생성. 앱 PNG의 불투명 보라색 배경·흰색 심볼, 투명 전경과 컬러 PNG 픽셀 검사, 파비콘 원본 바이트 일치 |
| 플랫폼 기본값 | PASS: `app_icons.py`의 플랫폼별 선택·직접 지정 우선·비활성화·PNG/ICO/ICNS 구조 검사. 생성기와 검사의 경로를 현재 `packages/platforms/build` 위치에 맞춤 |
| 패키지·앱 실행 | notVerified: 이번 변경으로 target 패키지 빌드·설치·각 OS의 실제 아이콘 표시는 수행하지 않음 |

검사는 `Doroti/eng/run-with-timeout.py --timeout 1200`으로 실행했습니다. 아래 기록은 이전 도안에 대한 검증이며, 이번 도안의 실행 검증 결과로 취급하지 않습니다.

## 기본 앱 아이콘 교체 검증 · 2026-09-29

당시 앱 원본은 `doroti-app-icon.svg`, SHA-256 `be0ca1713bcea2c51738d9e25609610c70ad3a68f3413dc08254639902165c41`입니다. 전달된 `doroti-symbol-white-dotnet-purple-200.svg`의 도형과 색상을 유지하며, SVG 설명 문구만 정리했습니다. 컬러·단색 브랜드 원본도 각각 전달된 filled-cutout-thin-200 SVG와 바이트 일치를 확인했습니다.

| 범위 | 이번 교체에서 확인한 결과 |
| --- | --- |
| 파생 자산 | PASS: Edge로 PNG·ICO·ICNS·파비콘, 투명 흰색 모바일 전경과 보라색 배경 재생성. 앱 PNG와 컬러 VS Code 확장 PNG 시각 확인 |
| SDK 패키지 | PASS: Release 패키지 생성, `app_icons.py`의 플랫폼별 기본값·직접 지정 우선·비활성화·ICO/ICNS 구조·패키지 바이트 일치 검사 |
| Android arm64 | PASS: Testbed Debug `ResizetizeImages`, 생성 PNG 시각 확인 및 일반 아이콘의 불투명 보라색 배경·흰색 심벌 픽셀 확인. 기기 런처 표시는 notVerified |
| iOS simulator arm64 | PASS: Windows에서 Testbed Debug `ResizetizeImages`, 1024px artwork 시각 확인 및 카탈로그 PNG의 불투명 보라색 배경·흰색 심벌 픽셀 확인. Xcode 컴파일·기기 표시는 notVerified |
| 기타 플랫폼 | SDK 기본 자산과 선택 규칙 검증 완료. 새 도안으로 Windows 실행 파일·창, macOS Dock, Linux Qt, Web 개발 서버를 다시 실행하는 검증은 notVerified |

검증용 SDK 패키지는 `temp/testing/app-icons/replacement/packages`에 있습니다. 검사 후 삭제를 시도했으나 실행 정책에서 차단되어 남겨두었습니다. 플랫폼 `obj` 생성물도 유지합니다. 이번 교체의 PASS는 아래 이전 도안의 실행 검증을 새 도안으로 재수행했다는 뜻이 아닙니다.

## 이전 컬러 윤곽 도안의 검증 기록

아래는 교체 전 `doroti-symbol-color.svg`(SHA-256 `9eee7440bd6c2f8926f366e6f819b4eaf210b589a88211ff189280e8654a7511`) 기준 기록입니다.

| 범위 | 결과와 확인한 내용 |
| --- | --- |
| SDK | PASS: 패키지 생성, Icons 자산과 targets의 바이트 일치. `app_icons.py`에서 플랫폼별 선택, 앱의 직접 지정 우선, 로컬 파비콘 우선, 기본값 비활성화, ICO 프레임·ICNS 구조 검사 |
| Windows App SDK | PASS: SampleApp2 Debug 빌드(경고·오류 0). 실행 파일의 그룹 아이콘 리소스 32512 및 실행 중 HWND의 큰·작은 클래스 아이콘을 읽어 Doroti 심벌 확인 |
| Windows MAUI | PASS: Testbed `ResizetizeImages`로 실행 파일용 ICO와 이미지 생성. 전체 MAUI 실행·MSIX 설치는 notVerified |
| Android | PASS: Testbed `ResizetizeImages`로 일반·원형 PNG 및 적응형 background/foreground/monochrome 참조 생성, 생성 PNG 시각 확인. 기기 런처 표시는 notVerified |
| iOS | PASS: Windows에서 Testbed Resizetizer로 `appicon.appiconset`, Contents.json, 1024px artwork 생성 및 PNG 시각 확인. Xcode 카탈로그 컴파일·시뮬레이터·기기 표시는 notVerified |
| Mac Catalyst | SDK의 `MauiIcon` 선택·전경·배율 확인. 카탈로그 빌드와 실제 표시는 notVerified |
| macOS AppKit | 실제 Testbed 프로젝트 평가에서 ICNS BundleResource와 기존 키를 덮어쓰지 않는 PartialAppManifest 확인. ICNS 구조 검사 통과. 앱 번들 빌드·Dock 표시는 notVerified |
| Linux Qt | PASS: WSL Ubuntu Qt 6.10.2, Qt Quick OFF로 네이티브 호스트 빌드. 해당 공유 라이브러리의 PNG 리소스를 Qt로 디코딩하고 offscreen QWindow가 앱 아이콘을 상속하는 것을 확인. 물리 화면/Wayland 런처 표시는 notVerified. Qt Quick 빌드는 이 환경의 Qt6Quick 개발 패키지 부재로 미검증 |
| Web | PASS: SampleApp2 Debug 빌드(경고·오류 0), 정적 자산 ContentRoot/Link 확인, 실제 개발 서버 `/favicon.svg` 응답이 원본 SHA-256과 일치 |
| 템플릿 | 기본 .NET SVG 제거, SDK 기본값 사용. Linux CMake·호스트 소스가 검증한 Testbed와 바이트 일치. 샘플 및 템플릿의 HTML 파비콘 링크 연결 |

주요 재현 명령(저장소 루트):

```powershell
python Doroti/eng/run-with-timeout.py python Doroti/tests/app_icons.py
python Doroti/eng/run-with-timeout.py dotnet build samples/DorotiSampleApp2/windowsappsdk/DorotiSampleApp2.WindowsAppSdk.csproj -c Debug -p:UseSharedCompilation=false
python Doroti/eng/run-with-timeout.py dotnet build samples/DorotiSampleApp2/web/DorotiSampleApp2.Web.csproj -c Debug -p:UseSharedCompilation=false
python Doroti/eng/run-with-timeout.py dotnet msbuild samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -restore -t:ResizetizeImages -p:Configuration=Debug -p:RuntimeIdentifier=android-arm64
python Doroti/eng/run-with-timeout.py dotnet msbuild samples/DorotiTestbedApp/windows/DorotiTestbedApp.Windows.csproj -restore -t:ResizetizeImages -p:Configuration=Debug -p:RuntimeIdentifier=win-x64
python Doroti/eng/run-with-timeout.py dotnet msbuild samples/DorotiTestbedApp/ios/DorotiTestbedApp.iOS.csproj -restore -t:ResizetizeImages -p:Configuration=Debug
```

실행 중인 Windows HWND와 Qt offscreen 리소스 검사는 자동화 검증입니다. 화면에서 사람이 아이콘을 클릭하거나 기기에 설치한 결과를 뜻하지 않습니다. 임시 검증 스크립트·로그·추출 이미지는 결과를 기록한 뒤 제거했으며, 제품의 일반 `bin/obj` 출력은 유지합니다. SDK/템플릿의 외부 패키지 배포는 수행하지 않았습니다.
