#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ProjectPath = (Join-Path $PSScriptRoot '../validation/windowsappsdk-flutter-bootstrap/Doroti.Validation.WindowsAppSdkFlutterBootstrap.csproj'),

    [string] $ContractValidatorPath = (Join-Path $PSScriptRoot 'validate-windowsappsdk-flutter-bootstrap.ps1'),

    [string] $ProductRunnerPath = (Join-Path $PSScriptRoot '../../DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj'),

    [string] $OutputRoot = (Join-Path $PSScriptRoot '../artifacts/windowsappsdk-flutter-bootstrap')
)

$ErrorActionPreference = 'Stop'
$testTimeout = [TimeSpan]::FromMinutes(20)
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectFullPath = (Resolve-Path $ProjectPath).Path
$contractValidatorFullPath = (Resolve-Path $ContractValidatorPath).Path
$productRunnerFullPath = (Resolve-Path $ProductRunnerPath).Path
$outputRootFullPath = [IO.Path]::GetFullPath($OutputRoot)

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Invoke-ProcessWithinTestTimeout(
    [string] $FileName,
    [string[]] $Arguments,
    [string] $Description,
    [bool] $RestrictPath
) {
    $startInfo = [Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = $FileName
    $startInfo.WorkingDirectory = $dorotiRoot
    $startInfo.UseShellExecute = $false
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.CreateNoWindow = $true
    if ($RestrictPath) {
        # The native bootstrap resolves its selected artifacts by absolute path;
        # retain only system DLL lookup for the live no-PATH-fallback probe.
        $startInfo.Environment['PATH'] = 'C:\Windows\System32'
    }
    foreach ($argument in $Arguments) { [void] $startInfo.ArgumentList.Add($argument) }

    $process = [Diagnostics.Process]::new()
    $process.StartInfo = $startInfo
    Assert-True $process.Start() "Could not start $Description."
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit([int] $testTimeout.TotalMilliseconds)) {
        $process.Kill($true)
        $process.WaitForExit()
        throw "$Description exceeded the 20-minute timeout."
    }
    $stdout = $stdoutTask.GetAwaiter().GetResult()
    $stderr = $stderrTask.GetAwaiter().GetResult()
    if ($process.ExitCode -ne 0) {
        throw "$Description failed with exit code $($process.ExitCode).`n$stdout`n$stderr"
    }
    return $stdout
}

function Invoke-ContractValidator([string] $EvidencePath, [bool] $RequireEvidence) {
    if ($RequireEvidence) {
        $json = & $contractValidatorFullPath -EvidencePath $EvidencePath -RequireEvidence
    }
    else {
        $json = & $contractValidatorFullPath
    }
    if ($null -ne $LASTEXITCODE -and $LASTEXITCODE -ne 0) {
        throw 'F1 static contract validation failed.'
    }
    return ($json | Out-String | ConvertFrom-Json -Depth 32)
}

function Get-PublishedDependencyVersion(
    [object] $Deps,
    [string] $ProjectName,
    [string] $PackageName
) {
    $targetName = [string] $Deps.runtimeTarget.name
    $target = $Deps.targets.PSObject.Properties[$targetName]
    Assert-True ($null -ne $target) "Publish deps has no runtime target '$targetName'."
    $project = @($target.Value.PSObject.Properties | Where-Object {
            $_.Name.StartsWith("$ProjectName/", [StringComparison]::Ordinal)
        })
    Assert-True ($project.Count -eq 1) "Publish deps has no unique '$ProjectName' entry."
    $dependency = $project[0].Value.dependencies.PSObject.Properties[$PackageName]
    Assert-True ($null -ne $dependency) "Publish deps omits $PackageName from the $ProjectName graph."
    return [string] $dependency.Value
}

$sourceValidation = Invoke-ContractValidator '' $false
[IO.Directory]::CreateDirectory($outputRootFullPath) | Out-Null
$publishDirectory = Join-Path $outputRootFullPath (
    'f1-live-' + [DateTimeOffset]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N'))
Assert-True (-not (Test-Path -LiteralPath $publishDirectory)) "F1 publish directory already exists: $publishDirectory"

$restoreOutput = Invoke-ProcessWithinTestTimeout 'dotnet' @(
    'restore', $projectFullPath, '--force-evaluate', '--nologo', '-m:1'
) 'F1 clean restore' $false
Write-Host $restoreOutput

$publishOutput = Invoke-ProcessWithinTestTimeout 'dotnet' @(
    'publish', $projectFullPath, '-c', 'Release', '-r', 'win-x64', '--self-contained', 'true',
    '--no-restore', '--nologo', '-o', $publishDirectory
) 'F1 self-contained publish' $false
Write-Host $publishOutput

$fixtureBaseName = 'Doroti.Validation.WindowsAppSdkFlutterBootstrap'
$executablePath = Join-Path $publishDirectory "$fixtureBaseName.exe"
$depsPath = Join-Path $publishDirectory "$fixtureBaseName.deps.json"
$anglePath = Join-Path $publishDirectory 'av_libglesv2.dll'
$skiaPath = Join-Path $publishDirectory 'libSkiaSharp.dll'
$windowsAppRuntimePath = Join-Path $publishDirectory 'Microsoft.WindowsAppRuntime.dll'
foreach ($requiredPath in @($executablePath, $depsPath, $anglePath, $skiaPath, $windowsAppRuntimePath)) {
    Assert-True (Test-Path -LiteralPath $requiredPath -PathType Leaf) "F1 publish is missing: $requiredPath"
}
Assert-True (@(Get-ChildItem -LiteralPath $publishDirectory -File -Filter 'av_libglesv2.dll').Count -eq 1) (
    'F1 publish must contain exactly one selected ANGLE DLL.')
Assert-True (@(Get-ChildItem -LiteralPath $publishDirectory -File -Filter 'libSkiaSharp.dll').Count -eq 1) (
    'F1 publish must contain exactly one selected Skia native DLL.')

$angleHash = (Get-FileHash -LiteralPath $anglePath -Algorithm SHA256).Hash.ToLowerInvariant()
$skiaHash = (Get-FileHash -LiteralPath $skiaPath -Algorithm SHA256).Hash.ToLowerInvariant()
Assert-True ($angleHash -eq '53191a77fe783cd757ca7767077c2a64a662e7043777a5b4ab74980d4a0b73e3') (
    'F1 publish ANGLE DLL hash drifted.')
Assert-True ($skiaHash -eq '0d98e29c028b3315d0e0344d02cd7aa4080abdf17fa67086847da13435320f2a') (
    'F1 publish Skia native DLL hash drifted.')

$deps = Get-Content -LiteralPath $depsPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ((Get-PublishedDependencyVersion $deps $fixtureBaseName 'Microsoft.WindowsAppSDK') -eq '2.4.0') (
    'F1 publish did not select Microsoft.WindowsAppSDK 2.4.0.')
Assert-True ((Get-PublishedDependencyVersion $deps $fixtureBaseName 'Avalonia.Angle.Windows.Natives') -eq '2.1.27548.20260419') (
    'F1 publish did not select the pinned Avalonia ANGLE native package.')
Assert-True ((Get-PublishedDependencyVersion $deps $fixtureBaseName 'SkiaSharp.NativeAssets.Win32') -eq '4.151.1') (
    'F1 publish did not select the pinned Skia native package.')

$productPublishDirectory = Join-Path $publishDirectory 'product-runner'
$productPublishOutput = Invoke-ProcessWithinTestTimeout 'dotnet' @(
    'publish', $productRunnerFullPath, '-c', 'Release', '-r', 'win-x64', '--self-contained', 'true',
    '--nologo', '-o', $productPublishDirectory
) 'F1 Windows product self-contained publish' $false
Write-Host $productPublishOutput

$productBaseName = 'DorotiDemoApp.WindowsAppSdk'
$productExecutablePath = Join-Path $productPublishDirectory "$productBaseName.exe"
$productDepsPath = Join-Path $productPublishDirectory "$productBaseName.deps.json"
$productAnglePath = Join-Path $productPublishDirectory 'av_libglesv2.dll'
$productSkiaPath = Join-Path $productPublishDirectory 'libSkiaSharp.dll'
$productWindowsAppRuntimePath = Join-Path $productPublishDirectory 'Microsoft.WindowsAppRuntime.dll'
foreach ($requiredPath in @(
        $productExecutablePath, $productDepsPath, $productAnglePath, $productSkiaPath,
        $productWindowsAppRuntimePath)) {
    Assert-True (Test-Path -LiteralPath $requiredPath -PathType Leaf) (
        "F1 product publish is missing: $requiredPath")
}
$productAngleHash = (Get-FileHash -LiteralPath $productAnglePath -Algorithm SHA256).Hash.ToLowerInvariant()
$productSkiaHash = (Get-FileHash -LiteralPath $productSkiaPath -Algorithm SHA256).Hash.ToLowerInvariant()
Assert-True ($productAngleHash -eq $angleHash) 'F1 product publish ANGLE native artifact hash drifted.'
Assert-True ($productSkiaHash -eq $skiaHash) 'F1 product publish Skia native artifact hash drifted.'
$productDeps = Get-Content -LiteralPath $productDepsPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ((Get-PublishedDependencyVersion $productDeps $productBaseName 'Microsoft.WindowsAppSDK') -eq '2.4.0') (
    'F1 product publish did not select Microsoft.WindowsAppSDK 2.4.0.')

$executableHash = (Get-FileHash -LiteralPath $executablePath -Algorithm SHA256).Hash.ToLowerInvariant()
$evidencePath = Join-Path $publishDirectory 'f1-live-evidence.json'
$liveOutput = Invoke-ProcessWithinTestTimeout $executablePath @(
    '--evidence', $evidencePath,
    '--source-fingerprint', [string] $sourceValidation.sourceFingerprint,
    '--published-executable-sha256', $executableHash
) 'F1 STA/AppWindow and ANGLE/Skia live validation' $true
Write-Host $liveOutput

$liveValidation = Invoke-ContractValidator $evidencePath $true
$evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json -Depth 32
$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-bootstrap-live-validation/v1'
    status = 'PASS'
    sourceFingerprint = [string] $liveValidation.sourceFingerprint
    publish = [ordered]@{
        deployment = 'self-contained-unpackaged'
        runtimeIdentifier = 'win-x64'
        directory = $publishDirectory
        executablePath = $executablePath
        executableSha256 = $executableHash
        depsPath = $depsPath
        windowsAppSdkPackageVersion = '2.4.0'
    }
    productPublish = [ordered]@{
        deployment = 'self-contained-unpackaged'
        runtimeIdentifier = 'win-x64'
        directory = $productPublishDirectory
        executablePath = $productExecutablePath
        depsPath = $productDepsPath
        windowsAppSdkPackageVersion = '2.4.0'
        angleSha256 = $productAngleHash
        skiaSha256 = $productSkiaHash
    }
    nativeArtifacts = [ordered]@{
        angle = [ordered]@{ path = $anglePath; sha256 = $angleHash }
        skia = [ordered]@{ path = $skiaPath; sha256 = $skiaHash }
    }
    platformThread = $evidence.platformThread
    rasterThread = $evidence.rasterThread
    startup = $evidence.startup
    evidencePath = $evidencePath
    notVerified = @(
        'F2 raw top-level and child HWND product ownership',
        'metrics, first-frame show, resize, input, lifecycle, and visible acceptance'
    )
}
$summaryPath = Join-Path $publishDirectory 'f1-live-validation.json'
[IO.File]::WriteAllText(
    $summaryPath,
    (($summary | ConvertTo-Json -Depth 32) -replace "`r`n", "`n") + "`n",
    [Text.UTF8Encoding]::new($false))
$summary | ConvertTo-Json -Depth 32
