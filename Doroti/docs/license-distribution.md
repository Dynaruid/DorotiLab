# 라이선스 등록과 배포

라이브러리가 자기 고지 파일을 `DorotiLicense` 항목으로 등록합니다. 공통 빌드 규칙은
등록된 파일을 `licenses/<PackageId>/`에 복사합니다. 참조하지 않은 라이브러리는
고지를 추가하지 않습니다. 이 기준은 메서드 호출 여부가 아니라 프로젝트·패키지 참조입니다.

```xml
<ItemGroup>
  <DorotiLicense Include="LICENSE" />
  <DorotiLicense Include="licenses/third-party-NOTICE.txt" />
</ItemGroup>
```

앱 프로젝트에서도 같은 항목을 사용합니다. `PackageId`가 없으면 프로젝트 이름을
폴더 이름으로 사용합니다. 명시적인 경로가 필요하면
`TargetPath="licenses/MyLibrary/NOTICE.txt"`를 지정합니다.

저장소의 제품 프로젝트는 공통 BSD 라이선스를 자기 라이브러리 폴더에 등록합니다.
추가 문구는 그 코드를 사용하는 라이브러리에 선언합니다.

| 등록 주체 | 추가 고지 |
| --- | --- |
| Doroti.Material | 직접 이식한 Dart Random과 Material 색상 코드의 BSD/Apache 고지 |
| Doroti.Skia.Rendering | 직접 이식한 Flutter superellipse의 BSD 고지 |
| Doroti.Skia.Fonts | 실제 복원한 Roboto 라이선스 |
| Doroti.CustomCarousel / Carousel 앱 | 각자 사용하는 원본 코드·에셋의 BSD 문구 |
| Doroti.Host.Qt | Qt LGPLv3, GPLv3와 Wayland 고지 |
| Qt의 GStreamer 어댑터 | 활성화한 경우에만 LGPL-2.1 전문 |

프로젝트 참조는 .NET의 콘텐츠 전달 규칙을 사용합니다. NuGet 패키지를 만들 때는
등록된 고지와 이를 복사하는 `buildTransitive/<PackageId>.targets`를 함께 패키징합니다.
기존 라이브러리의 buildTransitive 동작을 보존하며, 전이 참조한 소비자에게도 전달합니다.

App/Runner SDK는 NuGet 캐시나 복원 그래프에서 외부 패키지의 라이선스를 자동으로
수집하지 않습니다. .NET 런타임 팩도 같은 기준을 적용합니다. 각 라이브러리가
명시적으로 등록한 파일만 전달하며, NuGet으로 배포한 라이브러리의 자체
buildTransitive 등록도 유지합니다. SkiaSharp·Silk.NET 등 외부 NuGet 의존성의 고지는
별도로 등록하여 복사하지 않습니다. 직접 이식한 소스의 고지는 해당 소스를 소유한
라이브러리에서 등록합니다.

모든 등록 파일은 `CopyToOutputDirectory`와 `CopyToPublishDirectory`를 적용합니다.
단일 파일 게시에서는 실행 없이 읽을 수 있도록 `ExcludeFromSingleFile=true`로
실행 파일 옆 `licenses/`에 보관합니다. 실행 파일과 이 폴더를 함께 배포합니다.
의존성을 제거한 뒤에는 새 게시 폴더를 사용하여 이전 배포의 파일이 남지 않게 합니다.

자동 복사는 원문 전달을 돕습니다. 실제 배포하는 Qt/WebEngine·네이티브 바이너리의
버전에 맞는 제삼자 고지, 해당 소스 제공과 교체 허용 등은
[의존성 정책](license-policy.md)에 따라 별도로 확인합니다.

## 검증

2026-10-13~14 Windows에서 다음을 확인했습니다. 모든 검사 명령은 20분 제한을 적용합니다.

- PASS: 프로젝트 참조, NuGet 전이 참조와 기존 buildTransitive 동작 보존.
  확장자 없는 원본·변경한 파일명도 등록 경로 그대로 패키징.
- PASS: 참조하지 않은 라이브러리의 제외와 참조 제거 후 새 게시물에서의 고지 제거.
- PASS: JIT/R2R 단일 파일 실행, 옆의 라이선스 폴더, no-build에서 변경된 고지 반영.
- PASS: GStreamer 비활성화 시 LGPL-2.1 제외. 외부 NuGet 패키지와 .NET 런타임 팩의
  라이선스 자동 수집 없음.
- PASS: Carousel 공통 앱 Release 빌드·게시, 실제 Windows App SDK 단일 파일 게시의
  직접 이식한 Material·Flutter 소스와 Roboto 고지, 외부 NuGet 의존성의 고지 제외 확인.
- PASS: Qt 관리 호스트 Release 빌드와 Qt/Wayland 고지 전달. Qt 네이티브 재빌드·CMake
  설치, Web/Android/Apple 배포물 내부 위치는 이 변경에서 `notVerified`입니다.

재현 명령은 [데스크톱 게시 문서](desktop-publishing.md#회귀-검사)에 있습니다.
