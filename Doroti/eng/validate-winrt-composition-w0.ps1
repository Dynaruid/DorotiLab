[CmdletBinding()]
param(
    [string]$OutputDirectory,
    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Get-RepositoryRoot {
    $candidate = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
    if (-not (Test-Path -LiteralPath (Join-Path $candidate 'Doroti\Doroti.slnx'))) {
        throw "Unable to locate the DorotiLab repository root from $PSScriptRoot."
    }
    return $candidate
}

function Get-Sha256 {
    param([Parameter(Mandatory)][string]$Path)
    return (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()
}

function Get-RelativePath {
    param(
        [Parameter(Mandatory)][string]$Root,
        [Parameter(Mandatory)][string]$Path
    )
    return [System.IO.Path]::GetRelativePath($Root, $Path).Replace('\', '/')
}

$repoRoot = Get-RepositoryRoot
$dorotiRoot = Join-Path $repoRoot 'Doroti'
$projectPath = Join-Path $dorotiRoot 'validation\windowsappsdk-24-preflight\Doroti.Validation.WindowsAppSdk24Preflight.csproj'
$contractPath = Join-Path $dorotiRoot 'validation\contracts\winrt-composition-w0.json'
$assetsPath = Join-Path $dorotiRoot 'validation\windowsappsdk-24-preflight\obj\project.assets.json'
$outputPath = Join-Path $dorotiRoot 'validation\windowsappsdk-24-preflight\bin\Release\net10.0-windows10.0.19041.0\win-x64'
$executablePath = Join-Path $outputPath 'Doroti.Validation.WindowsAppSdk24Preflight.exe'
$runId = 'w0-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 12))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti\evidence\$runId"
}
$OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$runtimeReportPath = Join-Path $OutputDirectory 'w0-runtime.json'
$manifestPath = Join-Path $OutputDirectory 'w0-manifest.json'

$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json -Depth 100
$centralPropsPath = Join-Path $dorotiRoot 'Directory.Packages.props'
$hostProjectPath = Join-Path $dorotiRoot 'src\Doroti.Host.WindowsAppSdk\Doroti.Host.WindowsAppSdk.csproj'
[xml]$centralProps = Get-Content -LiteralPath $centralPropsPath -Raw
[xml]$hostProject = Get-Content -LiteralPath $hostProjectPath -Raw
$centralVersion = ($centralProps.Project.ItemGroup.PackageVersion |
    Where-Object Include -eq 'Microsoft.WindowsAppSDK').Version
$hostVersionOverride = ($hostProject.Project.ItemGroup.PackageReference |
    Where-Object Include -eq 'Microsoft.WindowsAppSDK').VersionOverride
if ($centralVersion -ne $contract.packagePolicy.centralMicrosoftWindowsAppSdk) {
    throw "Central Microsoft.WindowsAppSDK drift: expected $($contract.packagePolicy.centralMicrosoftWindowsAppSdk), got $centralVersion."
}
if ($hostVersionOverride -ne $contract.packagePolicy.windowsAppSdkHostVersionOverride) {
    throw "Host Microsoft.WindowsAppSDK VersionOverride drift: expected $($contract.packagePolicy.windowsAppSdkHostVersionOverride), got $hostVersionOverride."
}

if (-not $SkipBuild) {
    & dotnet build $projectPath -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw "W0 preflight build failed with exit code $LASTEXITCODE." }
}
if (-not (Test-Path -LiteralPath $executablePath)) {
    throw "W0 preflight executable is missing: $executablePath"
}

$savedPath = $env:Path
try {
    $env:Path = "$env:SystemRoot\System32;$env:SystemRoot"
    & $executablePath --report $runtimeReportPath
    $runtimeExitCode = $LASTEXITCODE
}
finally {
    $env:Path = $savedPath
}
if ($runtimeExitCode -ne 0) {
    throw "W0 runtime activation failed with exit code $runtimeExitCode."
}
$runtimeReport = Get-Content -LiteralPath $runtimeReportPath -Raw | ConvertFrom-Json -Depth 100

$assets = Get-Content -LiteralPath $assetsPath -Raw | ConvertFrom-Json -Depth 100
$targetName = @($assets.targets.PSObject.Properties.Name |
    Where-Object { $_ -like 'net10.0-windows10.0.19041.0/win-x64' })[0]
if ([string]::IsNullOrWhiteSpace($targetName)) { throw 'The win-x64 target is missing from project.assets.json.' }
$target = $assets.targets.$targetName
$packageGraph = @($target.PSObject.Properties |
    Where-Object { $_.Name -match '^Microsoft\.(WindowsAppSDK|Windows\.)' } |
    Sort-Object Name |
    ForEach-Object {
        $dependencies = @()
        if ($null -ne $_.Value.PSObject.Properties['dependencies']) {
            $dependencies = @($_.Value.dependencies.PSObject.Properties |
                Sort-Object Name |
                ForEach-Object { [ordered]@{ id = $_.Name; version = [string]$_.Value } })
        }
        [ordered]@{
            id = $_.Name.Split('/')[0]
            version = $_.Name.Split('/')[1]
            dependencies = $dependencies
            packageSha512 = [string]$assets.libraries.($_.Name).sha512
        }
    })
$resolvedWindowsAppSdk = @($packageGraph | Where-Object id -eq 'Microsoft.WindowsAppSDK')
if ($resolvedWindowsAppSdk.Count -ne 1 -or $resolvedWindowsAppSdk[0].version -ne '2.4.0') {
    throw "Resolved Microsoft.WindowsAppSDK is not exactly 2.4.0."
}

$assemblyNames = @(
    'Microsoft.InteractiveExperiences.Projection.dll',
    'Microsoft.UI.dll',
    'Microsoft.UI.Input.dll',
    'Microsoft.UI.Windowing.dll',
    'Microsoft.WindowsAppRuntime.Bootstrap.dll',
    'Microsoft.WindowsAppRuntime.dll'
)
$assemblies = @($assemblyNames | ForEach-Object {
    $path = Join-Path $outputPath $_
    if (-not (Test-Path -LiteralPath $path)) { throw "Required self-contained assembly is missing: $path" }
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($path)
    [ordered]@{
        file = $_
        size = (Get-Item -LiteralPath $path).Length
        fileVersion = $version.FileVersion
        productVersion = $version.ProductVersion
        sha256 = Get-Sha256 $path
    }
})

$baselinePaths = @(
    (Join-Path $dorotiRoot 'validation\windows-resize-capture'),
    (Join-Path $dorotiRoot 'validation\windowsappsdk-24-preflight'),
    (Join-Path $dorotiRoot 'src\Doroti.Host.WindowsAppSdk\DorotiWindowsAppSdkRunner.cs')
)
$baselineFiles = @($baselinePaths | ForEach-Object {
    if ((Get-Item -LiteralPath $_) -is [System.IO.DirectoryInfo]) {
        Get-ChildItem -LiteralPath $_ -File -Recurse |
            Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
    }
    else { Get-Item -LiteralPath $_ }
} | Sort-Object FullName -Unique | ForEach-Object {
    [ordered]@{
        path = Get-RelativePath $repoRoot $_.FullName
        size = $_.Length
        sha256 = Get-Sha256 $_.FullName
    }
})

$productRoots = @(
    (Join-Path $dorotiRoot 'src\Doroti.Host.WindowsAppSdk'),
    (Join-Path $dorotiRoot 'src\Doroti.Target.Windows.WindowsAppSdk.win-x64')
)
$productFiles = @($productRoots | ForEach-Object {
    Get-ChildItem -LiteralPath $_ -File -Recurse |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
} | Sort-Object FullName -Unique)
$prohibitedPatterns = @('FlutterAngle', 'OwnedGeometryEnvelope', 'WindowsD3D12HostAdapter', 'WindowsAppSdkHostAdapter')
$prohibitedMatches = @()
foreach ($file in $productFiles) {
    $content = Get-Content -LiteralPath $file.FullName -Raw
    foreach ($pattern in $prohibitedPatterns) {
        if ($content -match [regex]::Escape($pattern)) {
            $prohibitedMatches += [ordered]@{
                path = Get-RelativePath $repoRoot $file.FullName
                pattern = $pattern
            }
        }
    }
}
$productDiff = @(& git -C $repoRoot diff --name-only -- `
    'Doroti/src/Doroti.Host.WindowsAppSdk' `
    'Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64')

$sourceRevision = (& git -C $repoRoot rev-parse HEAD).Trim()
$dirtyFiles = @(& git -C $repoRoot status --short)
$pass = $runtimeReport.status -eq 'PASS' -and
    -not $runtimeReport.boundary.contentAppWindowBridgeAvailable -and
    @($runtimeReport.requiredApis | Where-Object { -not $_.available }).Count -eq 0 -and
    @($runtimeReport.nativeEntrypoints | Where-Object { -not $_.available }).Count -eq 0 -and
    $prohibitedMatches.Count -eq 0 -and
    $productDiff.Count -eq 0

$manifest = [ordered]@{
    schema = 'doroti.winrt-composition-w0-manifest/v1'
    runId = $runId
    status = if ($pass) { 'PASS' } else { 'FAIL' }
    generatedAt = (Get-Date).ToUniversalTime().ToString('o')
    source = [ordered]@{
        revision = $sourceRevision
        dirty = $dirtyFiles.Count -gt 0
        dirtyFiles = $dirtyFiles
    }
    packagePolicy = [ordered]@{
        centralMicrosoftWindowsAppSdk = $centralVersion
        hostVersionOverride = $hostVersionOverride
        resolvedTarget = $targetName
        packageGraph = $packageGraph
    }
    assemblies = $assemblies
    runtimeProbe = $runtimeReport
    officialSample = $contract.officialSampleComparison
    baselines = $baselineFiles
    productBoundary = [ordered]@{
        implementationFiles = @($productFiles | ForEach-Object { Get-RelativePath $repoRoot $_.FullName })
        modifiedFiles = $productDiff
        prohibitedMatches = $prohibitedMatches
        runnerFailFast = (Get-Content -LiteralPath (Join-Path $dorotiRoot 'src\Doroti.Host.WindowsAppSdk\DorotiWindowsAppSdkRunner.cs') -Raw) -match 'NotSupportedException'
    }
    evidenceQualification = [ordered]@{
        compileTime = 'PASS'
        runtimeActivation = $runtimeReport.status
        selfContainedRestrictedPath = if ($runtimeExitCode -eq 0) { 'PASS' } else { 'FAIL' }
        visible = 'notVerified'
        physical = 'notVerified'
    }
}
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "W0 status=$($manifest.status) packages=$($packageGraph.Count) assemblies=$($assemblies.Count)"
Write-Host "manifest=$manifestPath"
if (-not $pass) { exit 1 }
