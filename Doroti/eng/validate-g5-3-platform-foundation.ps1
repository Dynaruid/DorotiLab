#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$evidencePath = Join-Path $dorotiRoot 'migration/flutter-avalonia/bridge-validation/g5-3-platform-foundation.json'
$nativeInputPath = Join-Path $dorotiRoot 'artifacts/g5-3/platform-native-input.json'
[IO.Directory]::CreateDirectory((Split-Path $nativeInputPath -Parent)) | Out-Null

function Invoke-Checked {
    param([scriptblock] $Command)
    $global:LASTEXITCODE = 0
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code $LASTEXITCODE." }
}

Invoke-Checked {
    Push-Location $dorotiRoot
    try {
        dotnet run --project "$dorotiRoot/tools/Doroti.AvaloniaPort/Doroti.AvaloniaPort.csproj" `
            --configuration Release -- audit
    }
    finally {
        Pop-Location
    }
}

$boundaryRoots = @(
    "$dorotiRoot/src/Doroti.Vendor.Avalonia.Base",
    "$dorotiRoot/src/Doroti.Vendor.Avalonia.Skia",
    "$dorotiRoot/src/Doroti.Vendor.Avalonia.Win32",
    "$dorotiRoot/src/Doroti.Host.Desktop.Flutter"
)
$boundaryFiles = Get-ChildItem -Path $boundaryRoots -Recurse -File -Include *.cs,*.csproj |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
$forbiddenMirror = @($boundaryFiles | Select-String -Pattern 'Avalonia\.Controls|Avalonia\.Visual|Avalonia\.Styling|Avalonia\.Markup\.Xaml')
if ($forbiddenMirror.Count -ne 0) {
    throw "The platform foundation contains $($forbiddenMirror.Count) Avalonia Control/Visual/Styling/XAML mirror references."
}
$productProjects = @((Get-Content "$dorotiRoot/Doroti.Product.slnx") |
    Select-String -Pattern 'Project Path="(?<path>[^"]+\.csproj)"' |
    ForEach-Object { Join-Path $dorotiRoot $_.Matches[0].Groups['path'].Value })
$officialPackages = @($productProjects | ForEach-Object {
    Select-String -Path $_ -Pattern '<PackageReference Include="Avalonia(\.|"|>)'
})
if ($officialPackages.Count -ne 0) {
    throw "The product graph contains $($officialPackages.Count) official Avalonia package references."
}

Invoke-Checked { dotnet build "$dorotiRoot/Doroti.Product.slnx" --configuration Release --nologo }
Invoke-Checked {
    dotnet run --project "$dorotiRoot/validation/Doroti.Validation.G5PlatformFoundation/Doroti.Validation.G5PlatformFoundation.csproj" `
        --configuration Release -- $evidencePath
}
Invoke-Checked {
    dotnet run --project "$dorotiRoot/validation/Doroti.Validation.G5NativeInput/Doroti.Validation.G5NativeInput.csproj" `
        --configuration Release -- $nativeInputPath
}
Invoke-Checked {
    dotnet run --project "$dorotiRoot/validation/Doroti.Validation.G5WidgetsTextInput/Doroti.Validation.G5WidgetsTextInput.csproj" `
        --configuration Release
}
Invoke-Checked { & "$dorotiRoot/eng/validate-g5-2-automation.ps1" }

$evidence = Get-Content $evidencePath -Raw | ConvertFrom-Json
if ($evidence.status -ne 'verified-windows-automated' -or
    $evidence.application.foundationDiagnostics.flutterSurfaces -ne 1 -or
    $evidence.application.frameClockOwnerCount -ne 1 -or
    -not $evidence.capabilityRoundTrips.accessibilityWmGetObject) {
    throw 'G5-3B platform foundation evidence did not close its automated gates.'
}

Write-Output 'G5-3B Avalonia audit, application lifecycle, actual HWND capability round trips, and predecessor native automation: PASS'
