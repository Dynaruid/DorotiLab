#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ProjectPath = (Join-Path $PSScriptRoot '../validation/windowsappsdk-flutter-egl-surface/Doroti.Validation.WindowsAppSdkFlutterEglSurface.csproj'),

    [string] $ContractValidatorPath = (Join-Path $PSScriptRoot 'validate-windowsappsdk-flutter-egl-surface.ps1'),

    [string] $OutputRoot = (Join-Path $PSScriptRoot '../artifacts/windowsappsdk-flutter-egl-surface')
)

$ErrorActionPreference = 'Stop'
$testTimeout = [TimeSpan]::FromMinutes(20)
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectFullPath = (Resolve-Path $ProjectPath).Path
$contractValidatorFullPath = (Resolve-Path $ContractValidatorPath).Path
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
        # The fixture must resolve ANGLE and Skia from its publish directory;
        # system DLL lookup remains available for Windows itself only.
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
        throw 'F4 static EGL surface contract validation failed.'
    }
    return ($json | Out-String | ConvertFrom-Json -Depth 64)
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
    'f4-live-' + [DateTimeOffset]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N'))
Assert-True (-not (Test-Path -LiteralPath $publishDirectory)) "F4 publish directory already exists: $publishDirectory"

$restoreOutput = Invoke-ProcessWithinTestTimeout 'dotnet' @(
    'restore', $projectFullPath, '--force-evaluate', '--nologo', '-m:1'
) 'F4 clean restore' $false
Write-Host $restoreOutput

$publishOutput = Invoke-ProcessWithinTestTimeout 'dotnet' @(
    'publish', $projectFullPath, '-c', 'Release', '-r', 'win-x64', '--self-contained', 'true',
    '--no-restore', '--nologo', '-o', $publishDirectory
) 'F4 self-contained publish' $false
Write-Host $publishOutput

$fixtureBaseName = 'Doroti.Validation.WindowsAppSdkFlutterEglSurface'
$executablePath = Join-Path $publishDirectory "$fixtureBaseName.exe"
$depsPath = Join-Path $publishDirectory "$fixtureBaseName.deps.json"
$anglePath = Join-Path $publishDirectory 'av_libglesv2.dll'
$skiaPath = Join-Path $publishDirectory 'libSkiaSharp.dll'
$windowsAppRuntimePath = Join-Path $publishDirectory 'Microsoft.WindowsAppRuntime.dll'
foreach ($requiredPath in @($executablePath, $depsPath, $anglePath, $skiaPath, $windowsAppRuntimePath)) {
    Assert-True (Test-Path -LiteralPath $requiredPath -PathType Leaf) "F4 publish is missing: $requiredPath"
}
Assert-True (@(Get-ChildItem -LiteralPath $publishDirectory -File -Filter 'av_libglesv2.dll').Count -eq 1) (
    'F4 publish must contain exactly one selected ANGLE DLL.')
Assert-True (@(Get-ChildItem -LiteralPath $publishDirectory -File -Filter 'libSkiaSharp.dll').Count -eq 1) (
    'F4 publish must contain exactly one selected Skia native DLL.')

$angleHash = (Get-FileHash -LiteralPath $anglePath -Algorithm SHA256).Hash.ToLowerInvariant()
$skiaHash = (Get-FileHash -LiteralPath $skiaPath -Algorithm SHA256).Hash.ToLowerInvariant()
Assert-True ($angleHash -eq '53191a77fe783cd757ca7767077c2a64a662e7043777a5b4ab74980d4a0b73e3') (
    'F4 publish ANGLE DLL hash drifted.')
Assert-True ($skiaHash -eq '0d98e29c028b3315d0e0344d02cd7aa4080abdf17fa67086847da13435320f2a') (
    'F4 publish Skia native DLL hash drifted.')

$deps = Get-Content -LiteralPath $depsPath -Raw | ConvertFrom-Json -Depth 64
Assert-True ((Get-PublishedDependencyVersion $deps $fixtureBaseName 'Microsoft.WindowsAppSDK') -eq '2.4.0') (
    'F4 publish did not select Microsoft.WindowsAppSDK 2.4.0.')
Assert-True ((Get-PublishedDependencyVersion $deps $fixtureBaseName 'Avalonia.Angle.Windows.Natives') -eq '2.1.27548.20260419') (
    'F4 publish did not select the pinned Avalonia ANGLE native package.')
Assert-True ((Get-PublishedDependencyVersion $deps $fixtureBaseName 'SkiaSharp.NativeAssets.Win32') -eq '4.151.1') (
    'F4 publish did not select the pinned Skia native package.')

$executableHash = (Get-FileHash -LiteralPath $executablePath -Algorithm SHA256).Hash.ToLowerInvariant()
$evidencePath = Join-Path $publishDirectory 'f4-live-evidence.json'
$liveOutput = Invoke-ProcessWithinTestTimeout $executablePath @(
    '--evidence', $evidencePath,
    '--source-fingerprint', [string] $sourceValidation.sourceFingerprint,
    '--published-executable-sha256', $executableHash
) 'F4 raster-thread EGL child-window surface live validation' $true
Write-Host $liveOutput

$liveValidation = Invoke-ContractValidator $evidencePath $true
$evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json -Depth 64
# The F4 fixture records GetGuiResources after warmup beside EGL/Skia leak counters.
$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-egl-surface-live-validation/v1'
    status = 'PASS'
    scopeBoundary = [string] $evidence.scopeBoundary
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
    nativeArtifacts = [ordered]@{
        angle = [ordered]@{ path = $anglePath; sha256 = $angleHash }
        skia = [ordered]@{ path = $skiaPath; sha256 = $skiaHash }
    }
    rasterThread = $evidence.rasterThread
    surface = $evidence.surface
    context = $evidence.context
    renderer = $evidence.renderer
    resources = $evidence.resources
    startup = $evidence.startup
    validation = $evidence.validation
    evidencePath = $evidencePath
    notVerified = @(
        'F5 bounded resize completion protocol, F6 scheduler/vsync, F7 input, F8 lifecycle, and F9 product-runner selection',
        'output-level visible blank or white-frame, compositor continuity, and hardware-visible acceptance (FG)'
    )
}
$summaryPath = Join-Path $publishDirectory 'f4-live-validation.json'
[IO.File]::WriteAllText(
    $summaryPath,
    (($summary | ConvertTo-Json -Depth 64) -replace "`r`n", "`n") + "`n",
    [Text.UTF8Encoding]::new($false))
$summary | ConvertTo-Json -Depth 64
