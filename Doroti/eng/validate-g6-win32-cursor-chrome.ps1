#Requires -Version 5.1
param()

$ErrorActionPreference = 'Stop'
$env:DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER = '1'
$env:MSBUILDDISABLENODEREUSE = '1'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$project = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialGallery/Doroti.Validation.G6MaterialGallery.csproj'
$dll = Join-Path $dorotiRoot 'validation/Doroti.Validation.G6MaterialGallery/bin/Release/net10.0/Doroti.Validation.G6MaterialGallery.dll'
$evidence = Join-Path $dorotiRoot 'migration/flutter-framework/g6-win32-cursor-chrome-evidence.json'
$artifacts = Join-Path $dorotiRoot 'artifacts/g6-win32-cursor-chrome/win-x64'

if ([Environment]::OSVersion.Platform -ne [PlatformID]::Win32NT) { throw 'G6-5R-I cursor/chrome validation requires Windows.' }
[IO.Directory]::CreateDirectory($artifacts) | Out-Null
dotnet build $project --configuration Release --nologo
if ($LASTEXITCODE -ne 0) { throw "G6-5R-I cursor/chrome validation build failed ($LASTEXITCODE)." }
dotnet $dll --wave M6 --evidence $evidence --artifact-dir $artifacts --win32-cursor-chrome
if ($LASTEXITCODE -ne 0) { throw "G6-5R-I cursor/chrome validation failed ($LASTEXITCODE)." }

$proof = Get-Content -LiteralPath $evidence -Raw | ConvertFrom-Json
if ($proof.status -ne 'verified-windows-x64-strict-gpu' -or
    @($proof.cursorMappings).Count -ne 36 -or
    [long]$proof.unsupportedSilentSuccessCount -ne 0 -or
    @($proof.nonClientOwnership).Count -ne 8 -or
    @($proof.resize.operations).Count -ne 8 -or
    [long]$proof.stress.iterations -ne 100 -or
    -not [bool]$proof.resourceClosure.isBalanced) {
    throw 'G6-5R-I cursor/chrome evidence contract failed.'
}

Write-Output "G6-5R-I Win32 cursor/chrome: PASS ($evidence)"
