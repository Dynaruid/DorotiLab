# 데스크톱 단일 파일 게시

플랫폼 러너에 `-p:PublishSingleFile=true`를 전달합니다. `dotnet build`는 컴파일하며,
배포용 단일 실행 파일은 `dotnet publish`에서 생성합니다. 공용 앱 `.csproj`는
플랫폼 중립 라이브러리이므로 게시 대상으로 사용하지 않습니다.

| 러너 | 단일 파일 게시 | 배포 조건 |
| --- | --- | --- |
| Windows App SDK | 지원 | unpackaged, .NET/Windows App SDK self-contained |
| Windows MAUI | 지원 | unpackaged, .NET/Windows App SDK self-contained |
| Linux Qt | 지원 | Qt/QML/WebEngine 등 시스템 의존성은 대상 Linux에 설치 |
| macOS AppKit / Mac Catalyst | 미지원 | 현재 [.NET Apple SDK](https://github.com/dotnet/macios/blob/main/dotnet/targets/Xamarin.Shared.Sdk.targets)가 `PublishSingleFile`을 거부하므로 `.app`으로 게시 |
| Android / iOS / Web | 적용 대상 아님 | 해당 플랫폼의 APK/앱 번들/Web 게시 사용 |

## Carousel 예제

저장소 루트에서 실행합니다. Linux 명령은 Linux 또는 WSL에서 실행합니다.

```powershell
# Windows: CoreCLR JIT, 단일 EXE
dotnet publish samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Release -p:PublishSingleFile=true -o temp/publish/carousel-windows

# Windows: ReadyToRun + 앱 DLL 난독화 후 단일 EXE
dotnet publish samples/DorotiCarouselApp/windowsappsdk/DorotiCarouselApp.WindowsAppSdk.csproj -c Release -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:DorotiObfuscate=true -o temp/publish/carousel-windows-r2r

# Linux: .NET 런타임 포함, Qt 네이티브 빌드 후 단일 실행 파일
dotnet publish samples/DorotiCarouselApp/linux/DorotiCarouselApp.Linux.csproj -c Release -p:PublishSingleFile=true --self-contained true -p:DorotiBuildQtNative=true -o temp/publish/carousel-linux
```

Linux에서 `--self-contained false`를 선택하면 대상 컴퓨터에 호환 .NET 런타임이
필요합니다. Self-contained도 Qt나 Vulkan 드라이버 등 OS 의존성까지 포함하지는
않습니다. Qt WebEngine은 시스템 Chromium helper/data/QML 모듈도 필요합니다.
소스에서 빌드하지 않을 때는 `DorotiBuildQtNative=false`와 해당 옵션으로 만들어진
`DorotiQtNativeBuildDirectory`를 사용합니다.

직접 `.csproj`에 `<PublishSingleFile>true</PublishSingleFile>`를 선언해도 같은
프로필을 사용합니다. 서로 다른 게시 형식은 별도 `-o` 폴더에 보관하세요. 같은
폴더에 남아 있는 이전 폴더 게시의 DLL까지 자동으로 정리하지는 않습니다.

## 추출과 파일 검증

SDK는 지원 러너의 단일 파일 게시에서 다음 기본값을 적용합니다.

- `IncludeAllContentForSelfExtract=true`
- `IncludeNativeLibrariesForSelfExtract=true`
- Windows의 `EnableMsixTooling=true`
- Windows의 `SelfContained=true`, `WindowsAppSDKSelfContained=true` (값을 지정하지 않은 경우)
- Release의 `CopyOutputSymbolsToPublishDirectory=false`

배포 파일은 하나지만 실행 시 DLL과 콘텐츠를 추출합니다. 이는 Windows App SDK의
DLL redirection과 Doroti의 원본 SkiaSharp 관리/네이티브 파일 해시 검증에 필요합니다.
관리 DLL의 `Assembly.Location`과 `AppContext.BaseDirectory`는 추출된 파일을 가리킵니다.
부분 추출, 트리밍, NativeAOT 조합은 명시적으로 거부합니다. Debug Hot Reload는
기존 디렉터리 빌드를 사용합니다.

추출 위치는 `DOTNET_BUNDLE_EXTRACT_BASE_DIR`로 지정할 수 있습니다. 기본값은 Windows의
`%TEMP%/.net`, Linux의 `$HOME/.net` 아래입니다. 앱이 사용자 데이터를 영구 저장할
위치로 이 추출 폴더를 사용하지 마세요.

Qt의 선택된 네이티브 shim, WebEngine manifest 및 `licenses/doroti-linux/` 고지 파일은
번들 입력으로 등록합니다. Windows App SDK의 `doroti-native-provenance.json`도 번들
생성 전에 실제 게시 대상 DLL의 해시로 생성합니다. `--no-build` 게시에서도 필요한
네이티브 파일이 빠지면 실패합니다.

ReadyToRun과 [Obfuscar](obfuscation.md)를 함께 사용할 수 있습니다. 공용 앱 IL 난독화,
ReadyToRun, 번들 생성 순서로 처리합니다. 원본 파일 해시를 검사하는 `SkiaSharp.dll`은
ReadyToRun 변환에서 제외합니다. 이 프로필은 `PublishReadyToRunComposite=false`를
기본값으로 사용하며, 원본 SkiaSharp 파일을 바꾸는 composite R2R은 거부합니다.
`PublishSingleFile`은 참조 라이브러리의 빌드에는
전달하지 않습니다. 난독화 게시의 기존 `--no-build` 금지는 그대로 적용합니다.

Windows의 요구 조건은 [Windows App SDK 단일 EXE 문서](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/unpackage-winui-app#single-file-exe),
일반적인 추출 동작은 [.NET 단일 파일 게시](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview)를 참고하세요.

## 회귀 검사

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/tests/desktop_single_file.py --output temp/testing/desktop-single-file-contract
```

새 출력 폴더를 지정합니다. 이 검사는 실제 단일 파일을 게시한 뒤 실행 파일만 별도
폴더로 복사하여 추출된 관리 DLL/네이티브 shim/라이선스, no-build 게시의 변경된
네이티브 입력, 잘못된 옵션 거부를 확인합니다. Qt shim은 콘텐츠 검사용 fixture이므로
이 검사만으로 Qt GUI나 GPU 동작을 증명하지 않습니다.

## 2026-10-13 검증

| 검사 | 결과 | 확인 범위 |
| --- | --- | --- |
| Windows / Linux 회귀 fixture | PASS | 단일 파일 게시·EXE/ELF만 복사한 실행·DLL 파일 해시 유지·실제 R2R 헤더·no-build의 변경된 네이티브 콘텐츠·잘못된 조합 거부 |
| Carousel Windows App SDK JIT | PASS | EXE 하나, 네이티브 해시 audit, `VisibleAfterExactPresent=true`, GPU present, 정상 종료 |
| Carousel Windows App SDK R2R + Obfuscar | PASS | 개별 R2R, EXE 하나, 원본 SkiaSharp 해시 audit, GPU present 2회, device loss 0, 정상 종료 |
| Testbed Windows MAUI JIT | PASS | EXE 하나만 복사한 실행, 창 표시·크기/상태/외형 변경, 첫 닫기 취소·두 번째 닫기 허용, 남은 창 0, 정상 종료 |
| Carousel Linux Qt JIT | PASS | self-contained ELF 하나, 별도 폴더 실행, Qt 6.10.2 / WSLg Wayland, 20회 크기 변경, presented 40 / failed 0, 종료 시 GPU 예약/retiring layer 0, 정상 종료 |
| 기존 Qt 빌드 프로필 회귀 | PASS | Ubuntu에서 옵션 분리, 네이티브 복사/오래된 파일 제거, 잘못된 프로필/누락된 shim 거부 |
| Windows / Qt provider `.nupkg` | PASS | 새 게시 `.targets`의 실제 패키지 포함 확인 |

Linux 실행은 software Vulkan 환경입니다. 이 결과는 Linux 실기기 GPU 성능,
FPS/scanout, 전체 물리 입력·접근성 수락을 증명하지 않습니다. macOS/Mac Catalyst,
Linux R2R+Obfuscar 전체 GUI, NuGet 패키지만 사용하는 독립 앱의 전체 실행은
`notVerified`입니다. 원시 EXE/ELF·로그는 `temp/testing/`의 삭제 가능한 로컬 산출물입니다.

Linux 실행 검사에서 일반 폴더 빌드에도 있던 종료 예외를 재현했습니다.
Qt의 `Closed` 이벤트가 뷰를 먼저 해제하기 전에 프레임워크 분기를 분리하도록
수정하여 단일 파일 실행의 정상 종료까지 확인했습니다.
