# iOS NativeAOT 빌드와 설치

DorotiSampleApp2의 SDK별 NativeAOT 게시 설정입니다. 명령은 저장소 루트 `DorotiLab`에서 실행합니다.
Mac, 대상 SDK의 iOS 워크로드, Xcode와 기기에 맞는 개발 인증서·프로비저닝 프로필이 필요합니다.
일반적인 기기 선택·설치는 [공용 배포 도구](../../../helpers/deploy-helper/README.md)를 사용하세요.

## .NET 10 + Xcode 27

이 예제는 .NET 10 SDK와 MAUI 10.0.110을 사용하며, iOS 플랫폼 버전은 설치된 워크로드의 기본값을 따릅니다.
`27.0`을 고정하지 않아 워크로드 업데이트 후 `NETSDK1140`이 발생하는 것을 방지합니다.
아래 NSObject 호환 설정은 .NET SDK 10.0.401, iOS SDK 27.0.10722에서 사용한 옵션입니다.
SDK 버전 검사와 AOT 분석 경고 검사를 유지합니다.

아래 예제는 저장소 루트에서 시작해 `Doroti/global.json`으로 .NET 10을 선택합니다.
서명 값은 해당 기기를 포함한 개발 프로필로 바꿉니다.

```powershell
$sample2AotArtifacts = Join-Path (Get-Location).Path 'Doroti/artifacts/sample2-ios-nativeaot-net10'
Push-Location ./Doroti
try {
    dotnet publish ../samples/DorotiSampleApp2/ios/DorotiSampleApp2.iOS.csproj `
        --disable-build-servers -nr:false -c Release -r ios-arm64 `
        -p:DorotiCompilationMode=NativeAot `
        -p:DorotiIosTargetFramework=net10.0-ios `
        -p:DorotiIosMauiVersion=10.0.110 `
        -p:Registrar=managed-static `
        -p:_UseDynamicDependenciesForMarkNSObjects=false `
        '-p:MtouchExtraArgs=--skip-marking-nsobjects-in-user-assemblies=true' `
        -p:Optimize=true `
        "-p:ArtifactsPath=$sample2AotArtifacts" `
        '-p:CodesignKey=YOUR_DEVELOPMENT_CERTIFICATE' `
        '-p:CodesignProvision=YOUR_PROVISIONING_PROFILE_UUID'
    if ($LASTEXITCODE -ne 0) { throw 'Native AOT publish failed.' }
} finally { Pop-Location }

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

## .NET 11과 Xcode 27.2

Xcode 27.2에서 .NET SDK `11.0.100`, 워크로드 집합 `11.0.100.1`(iOS `27.2.11039`)과 MAUI `11.0.10`을 사용합니다.
NativeAOT 앱은 Xcode 버전 검사를 활성화한 상태로 `dotnet publish`에서 생성합니다. [공식 iOS 워크로드 안내](https://github.com/dotnet/macios/releases/tag/dotnet-11.0.1xx-xcode27.2.11039)

저장소 루트 `DorotiLab`에서 시작합니다. SampleApp2의 `ios/global.json`이 SDK를 선택합니다.
인증서와 프로비저닝 프로필은 로컬 개발 서명 값으로 바꾸세요.

```powershell
$aotArtifacts = Join-Path (Get-Location).Path 'Doroti/artifacts/sample2-ios-nativeaot'
Push-Location ./samples/DorotiSampleApp2/ios
try {
    dotnet --version
    dotnet publish DorotiSampleApp2.iOS.csproj `
        -c Release -r ios-arm64 `
        -p:DorotiCompilationMode=NativeAot `
        "-p:ArtifactsPath=$aotArtifacts" `
        '-p:CodesignKey=YOUR_DEVELOPMENT_CERTIFICATE' `
        '-p:CodesignProvision=YOUR_PROVISIONING_PROFILE_UUID'
    if ($LASTEXITCODE -ne 0) { throw 'Native AOT publish failed.' }
} finally {
    Pop-Location
}
```

UIKit 진입점과 네이티브 뷰·델리게이트는 NativeAOT 보존 목록에서 유지합니다. [NativeAotRoots.xml](../ios/NativeAotRoots.xml)
