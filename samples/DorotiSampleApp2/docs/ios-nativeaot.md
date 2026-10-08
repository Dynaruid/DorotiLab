# iOS NativeAOT 빌드와 설치

DorotiSampleApp2의 SDK별 NativeAOT 게시 설정입니다. 명령은 저장소 루트 `DorotiLab`에서 실행합니다.
Mac, 대상 SDK의 iOS 워크로드, Xcode와 기기에 맞는 개발 인증서·프로비저닝 프로필이 필요합니다.
일반적인 기기 선택·설치는 [공용 배포 도구](../../../helpers/deploy-helper/README.md)를 사용하세요.

## .NET 10 + Xcode 27

이 예제는 .NET SDK 10.0.401, iOS SDK 27.0.10722, MAUI 10.0.110 구성의 게시 옵션입니다.
SDK 버전 검사와 AOT 분석 경고 검사를 유지합니다.

아래는 저장소 루트의 PowerShell 예제입니다. 서명 값은 해당 기기를 포함한 개발 프로필로 바꿉니다.

```powershell
$sample2AotArtifacts = Join-Path (Get-Location).Path 'Doroti/artifacts/sample2-ios-nativeaot-net10'
dotnet publish ./samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj `
    --disable-build-servers -nr:false -c Release -r ios-arm64 `
    -p:DorotiCompilationMode=NativeAot `
    -p:DorotiIosTargetFramework=net10.0-ios27.0 `
    -p:DorotiIosMauiVersion=10.0.110 `
    -p:Registrar=managed-static `
    -p:_UseDynamicDependenciesForMarkNSObjects=false `
    '-p:MtouchExtraArgs=--skip-marking-nsobjects-in-user-assemblies=true' `
    -p:Optimize=true `
    "-p:ArtifactsPath=$sample2AotArtifacts" `
    '-p:CodesignKey=YOUR_DEVELOPMENT_CERTIFICATE' `
    '-p:CodesignProvision=YOUR_PROVISIONING_PROFILE_UUID'

$sample2AotApp = Join-Path $sample2AotArtifacts 'bin/DorotiSampleApp2.iOS/release_ios-arm64/DorotiSampleApp2.iOS.app'
codesign --verify --deep --strict $sample2AotApp
xcrun devicectl device install app --device YOUR_DEVICE_UDID $sample2AotApp
xcrun devicectl device process launch --device YOUR_DEVICE_UDID --terminate-existing `
    --environment-variables '{"DOROTI_IOS_GRAPHITE":"1"}' dev.doroti.sample2
```

`_UseDynamicDependenciesForMarkNSObjects=false`와 `--skip-marking-nsobjects-in-user-assemblies=true`는
iOS SDK 27.0.10722의 NSObject 보존 경로에 대한 호환 설정입니다.
대신 [ios/NativeAotRoots.xml](../ios/NativeAotRoots.xml)이 앱 진입과 Doroti native 뷰·델리게이트를
명시적으로 보존합니다. SDK를 업데이트하면 이 옵션과 보존 목록을 함께 재검증해야 합니다.
설치는 기존 앱 데이터를 유지하며, 개발 서명 프로필의 만료 시점은 로컬 프로필에 따릅니다.

## .NET 11 RC1 + Xcode 27

[.NET 11 RC1 iOS 워크로드](https://github.com/dotnet/macios/releases/tag/dotnet-11.0.1xx-rc1-12193)는
**Xcode 26.6**을 요구합니다. Xcode 27에서 이 프로필을 빌드·게시하려면 직접
`dotnet build` / `dotnet publish` 명령에 **`-p:ValidateXcodeVersion=false`**를 추가해야 합니다.
Native AOT 앱 생성에는 `dotnet publish`를 사용합니다. 이 옵션은 버전 검사만 건너뛰며,
[Microsoft가 지원하는 Xcode 조합](https://learn.microsoft.com/en-us/dotnet/ios/troubleshooting/xcode-requirement)으로 바꾸지는 않습니다.
서명·네이티브 링크·설치·실행 오류는 별도로 확인해야 합니다.

다음은 **저장소 루트 `DorotiLab`의 PowerShell**에서 실행하는 예제입니다.
SampleApp2에는 .NET 11 선택용 `global.json`이 없으므로, 기존 Testbed iOS 폴더의
`global.json`으로 SDK를 선택한 뒤 SampleApp2 프로젝트를 지정합니다.
인증서 이름과 provisioning profile UUID는 로컬 개발 서명 값으로 바꾸세요.

```powershell
$aotArtifacts = Join-Path (Get-Location).Path 'Doroti/artifacts/sample2-ios-nativeaot'
Push-Location ./samples/DorotiTestbedApp/ios
try {
    dotnet --version # 11.0.100-rc.1.26425.128 또는 해당 global.json이 허용하는 패치
    dotnet publish ../../DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj `
        -c Release -r ios-arm64 `
        -p:DorotiCompilationMode=NativeAot `
        -p:ValidateXcodeVersion=false `
        -p:Registrar=managed-static `
        -p:_UseDynamicDependenciesForMarkNSObjects=false `
        '-p:MtouchExtraArgs=--skip-marking-nsobjects-in-user-assemblies=true' `
        -p:PrepareAssemblies=false `
        -p:PostProcessAssemblies=false `
        "-p:ArtifactsPath=$aotArtifacts" `
        '-p:CodesignKey=YOUR_DEVELOPMENT_CERTIFICATE' `
        '-p:CodesignProvision=YOUR_PROVISIONING_PROFILE_UUID'
    if ($LASTEXITCODE -ne 0) { throw 'Native AOT publish failed.' }
} finally {
    Pop-Location
}
```

위의 registrar·NSObject 보존 옵션은 .NET 11 RC1 NativeAOT에서 잘못 생성된 인터페이스 멤버 참조로
발생하는 `IL2037`을 피합니다. [ios/NativeAotRoots.xml](../ios/NativeAotRoots.xml)이 UIKit 진입점과 native 뷰·델리게이트를
명시적으로 보존하며, 공용 설치 CLI도 같은 옵션을 적용합니다.
.NET 11 RC1의 assembly-preparer에서는 같은 설정으로 `MarkNSObjects` 오류(`MT2080`)가 발생하므로
`PrepareAssemblies=false`, `PostProcessAssemblies=false`로 ILLink의 managed registrar 경로를 사용합니다.
버전 검사 우회는 해당 명령에만 적용합니다. 공통 프로젝트 설정에서 검사를 끄지 않습니다.
Xcode 26.6을 선택한 .NET 11 빌드나 Xcode 27을 지원하는 .NET 10 워크로드에는 이 옵션이 필요 없습니다.
[도구 업데이트 스크립트](../../../scripts/update-dotnet-macos.py)로 .NET 11 워크로드를 RC1으로 업데이트해도
RC1의 Xcode 26.6 요구 사항은 유지됩니다.
