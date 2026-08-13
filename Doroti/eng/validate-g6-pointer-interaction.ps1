#Requires -Version 5.1
param()

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$project = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialGallery/Doroti.Validation.G6MaterialGallery.csproj'
$dll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialGallery/bin/Release/net10.0/Doroti.Validation.G6MaterialGallery.dll'
$evidence = Join-Path $dorotiRoot 'migration/flutter-framework/g6-pointer-interaction-evidence.json'
$artifacts = Join-Path $dorotiRoot 'artifacts/g6-pointer-interaction/win-x64'

if ([Environment]::OSVersion.Platform -ne [PlatformID]::Win32NT) { throw 'G6-5R-I native pointer validation requires Windows.' }
[IO.Directory]::CreateDirectory($artifacts) | Out-Null
dotnet build $project --configuration Release --nologo
if ($LASTEXITCODE -ne 0) { throw "G6-5R-I pointer validation build failed ($LASTEXITCODE)." }
dotnet $dll --wave M6 --evidence $evidence --artifact-dir $artifacts --native-pointer
if ($LASTEXITCODE -ne 0) { throw "G6-5R-I native pointer validation failed ($LASTEXITCODE)." }

$proof = Get-Content -LiteralPath $evidence -Raw | ConvertFrom-Json
if ($proof.status -ne 'verified-windows-x64-strict-gpu-native-input' -or
    [long]$proof.result.directCallbackInvocationCount -ne 0 -or
    [long]$proof.result.callbackCount -ne 1 -or
    [string]$proof.result.selectedDate -ne '2026-08-14' -or
    [long]$proof.result.activeHoverChangedPixels -lt 10 -or
    [long]$proof.result.activeHoverMaxChannelDelta -lt 8 -or
    [long]$proof.result.leaveChangedPixels -lt 10 -or
    [long]$proof.result.stress.iterations -ne 100 -or
    [long]$proof.result.stress.stuckHoverOrCapture -ne 0 -or
    -not [bool]$proof.result.resourceClosure.isBalanced) {
    throw 'G6-5R-I pointer evidence contract failed.'
}

Write-Output "G6-5R-I native pointer interaction: PASS ($evidence)"
