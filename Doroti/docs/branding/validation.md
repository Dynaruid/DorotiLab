# 기본 앱 아이콘 검증 · 2026-09-29

컬러 원본은 `doroti-symbol-color.svg`, SHA-256 `9eee7440bd6c2f8926f366e6f819b4eaf210b589a88211ff189280e8654a7511`입니다. 플랫폼별 기본값, 변경 방법, 재생성 명령은 [아이콘 안내](README.md)에 있습니다.

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
