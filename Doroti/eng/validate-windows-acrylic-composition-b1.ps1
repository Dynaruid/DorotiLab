#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [switch] $SkipBuild,
    [switch] $SkipVisibleCapture
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$b0Project = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-content-island-capability/Doroti.Validation.WindowsAcrylicContentIslandCapability.csproj'
$b1Project = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-composition-spike/Doroti.Validation.WindowsAcrylicCompositionSpike.csproj'
$b0Executable = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-content-island-capability/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsAcrylicContentIslandCapability.exe'
$b1Executable = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-composition-spike/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsAcrylicCompositionSpike.exe'
$contractPath = Join-Path $repoRoot 'Doroti/validation/contracts/windows-acrylic-composition-b1.json'
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json -Depth 32
$runId = 'acrylic-b1-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 12))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti/evidence/$runId"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Invoke-BoundedProcess {
    param(
        [Parameter(Mandatory)][string] $FileName,
        [Parameter(Mandatory)][string[]] $ArgumentList,
        [Parameter(Mandatory)][string] $Name
    )
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.Environment['DOROTI_WINDOWS_DWM_FLUSH'] = '0'
    $start.Environment['DOROTI_WINDOWS_EGL_SWAP_INTERVAL'] = '1'
    foreach ($argument in $ArgumentList) { [void] $start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = $start
    Assert-True $process.Start() "$Name failed to start."
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        $process.Kill($true)
        throw "$Name exceeded the 20-minute timeout."
    }
    $stdout = $stdoutTask.GetAwaiter().GetResult()
    $stderr = $stderrTask.GetAwaiter().GetResult()
    if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
    if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
    return [ordered]@{ ExitCode = $process.ExitCode; Stdout = $stdout; Stderr = $stderr }
}

function Start-BoundedProcess {
    param(
        [Parameter(Mandatory)][string] $FileName,
        [Parameter(Mandatory)][string[]] $ArgumentList
    )
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.WindowStyle = [Diagnostics.ProcessWindowStyle]::Hidden
    $start.Environment['DOROTI_WINDOWS_DWM_FLUSH'] = '0'
    $start.Environment['DOROTI_WINDOWS_EGL_SWAP_INTERVAL'] = '1'
    foreach ($argument in $ArgumentList) { [void] $start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = $start
    Assert-True $process.Start() 'B1 visible process failed to start.'
    return $process
}

if (-not $SkipBuild) {
    foreach ($project in @($b0Project, $b1Project)) {
        $build = Invoke-BoundedProcess -FileName 'dotnet' -Name "build $project" -ArgumentList @(
            'build', $project, '-c', 'Release', '--nologo')
        Assert-True ($build.ExitCode -eq 0) "Build failed: $project"
    }
}
Assert-True (Test-Path -LiteralPath $b0Executable -PathType Leaf) "B0 executable is missing: $b0Executable"
Assert-True (Test-Path -LiteralPath $b1Executable -PathType Leaf) "B1 executable is missing: $b1Executable"

$b0Path = Join-Path $OutputDirectory 'b0.json'
$b0Run = Invoke-BoundedProcess -FileName $b0Executable -Name 'B0 capability' -ArgumentList @('--report', $b0Path)
$b0 = Get-Content -LiteralPath $b0Path -Raw | ConvertFrom-Json -Depth 100
$b0Pass = $b0Run.ExitCode -eq 0 -and
    $b0.status -eq $contract.b0.requiredStatus -and
    $b0.backdrop.addTargetResult -eq [bool]$contract.b0.requiredBackdropTarget -and
    $b0.graphics.directImportSucceeded -eq [bool]$contract.b0.requiredDirectImport -and
    $b0.graphics.cpuReadbackCount -le [int]$contract.b0.maximumCpuReadbacks -and
    $b0.graphics.beginDrawCount -eq $b0.graphics.endDrawCount -and
    $b0.graphics.offsetX -ge 0 -and $b0.graphics.offsetY -ge 0

$b1Path = Join-Path $OutputDirectory 'b1.json'
$b1Run = Invoke-BoundedProcess -FileName $b1Executable -Name 'B1/B2 capability' -ArgumentList @(
    '--duration', '4', '--report', $b1Path,
    '--ready-file', (Join-Path $OutputDirectory 'b1.ready.json'))
$b1 = Get-Content -LiteralPath $b1Path -Raw | ConvertFrom-Json -Depth 100
$b1Pass = $b1Run.ExitCode -eq 2 -and
    $b1.b1Status -eq $contract.b1.requiredStatus -and
    $b1.scene.surfacePoolSize -le [int]$contract.b1.maximumSurfacePool -and
    $b1.scene.successfulCommits -eq 3 -and
    $b1.scene.controllerCreateCount -eq 1 -and
    $b1.scene.addTargetCount -eq 1 -and
    @($b1.scene.backdropApplications).Count -eq [int]$contract.b1.requiredBackdropApplications -and
    $b1.scene.beginDrawCount -eq $b1.scene.endDrawCount -and
    $b1.scene.cpuReadbackCount -le [int]$contract.b1.maximumCpuReadbacks -and
    $b1.coordination.maxQueueDepth -le [int]$contract.b1.maximumQueueDepth -and
    $b1.coordination.duplicateTerminals -eq [int]$contract.b1.requiredDuplicateTerminals -and
    $b1.coordination.missingTerminals -eq [int]$contract.b1.requiredMissingTerminals
$b2ExpectedFail = $b1.b2Status -eq $contract.b2.requiredStatusUntilSafeRetirementIsProven -and
    $b1.p1Status -eq $contract.b2.requiredP1StatusUntilSafeRetirementIsProven -and
    -not $b1.scene.safeRetirementProven -and
    $b1.scene.safeReuseCount -eq 0 -and
    $b1.scene.noSafeSlotFailures -gt 0 -and
    $b1.scene.surfacePoolSize -le [int]$contract.b2.maximumSurfacePool

$visible = [ordered]@{
    status = 'notVerified'
    capturedFrames = 0
    encodedFrames = 0
    captureErrors = 0
    distinctAlphaSceneSamples = 0
    report = $null
}
if (-not $SkipVisibleCapture) {
    $observerBuild = Join-Path $repoRoot '.doroti/build/windows-resize-capture-acrylic-b1'
    $observer = Join-Path $observerBuild 'Release/Doroti.WindowsResizeCapture.exe'
    if (-not (Test-Path -LiteralPath $observer -PathType Leaf)) {
        $configure = Invoke-BoundedProcess -FileName 'cmake' -Name 'capture observer configure' -ArgumentList @(
            '-S', (Join-Path $repoRoot 'Doroti/validation/windows-resize-capture'),
            '-B', $observerBuild, '-A', 'x64')
        Assert-True ($configure.ExitCode -eq 0) 'Capture observer configure failed.'
        $observerBuildRun = Invoke-BoundedProcess -FileName 'cmake' -Name 'capture observer build' -ArgumentList @(
            '--build', $observerBuild, '--config', 'Release')
        Assert-True ($observerBuildRun.ExitCode -eq 0) 'Capture observer build failed.'
    }

    $readyPath = Join-Path $OutputDirectory 'visible.ready.json'
    $visibleReportPath = Join-Path $OutputDirectory 'visible-b1.json'
    $capturePath = Join-Path $OutputDirectory 'visible-capture.json'
    $process = Start-BoundedProcess -FileName $b1Executable -ArgumentList @(
        '--duration', '12', '--ready-file', $readyPath, '--report', $visibleReportPath)
    try {
        $deadline = [DateTime]::UtcNow.AddSeconds(10)
        while (-not (Test-Path -LiteralPath $readyPath -PathType Leaf)) {
            if ([DateTime]::UtcNow -ge $deadline) { throw 'B1 visible ready-file timeout.' }
            Start-Sleep -Milliseconds 50
        }
        $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
        $captureRun = Invoke-BoundedProcess -FileName $observer -Name 'B1 WGC capture' -ArgumentList @(
            '--hwnd', [string]$ready.hwnd,
            '--output', $capturePath,
            '--run-id', "$runId-visible",
            '--duration', '3',
            '--input-hz', '120',
            '--png-stride', '10',
            '--capture-only',
            '--no-desktop-duplication',
            '--edge', 'Right',
            '--drag-pixels', '180',
            '--drag-ms', '800')
        Assert-True ($process.WaitForExit($timeoutMilliseconds)) 'B1 visible process exceeded the 20-minute timeout.'
        $stdout = $process.StandardOutput.ReadToEnd()
        $stderr = $process.StandardError.ReadToEnd()
        if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
        if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
        $capture = Get-Content -LiteralPath $capturePath -Raw | ConvertFrom-Json -Depth 100
        $distinctSamples = 0
        $firstPngFrame = @($capture.frames | Where-Object { $_.png } | Select-Object -First 1)
        if ($firstPngFrame.Count -eq 1) {
            Add-Type -AssemblyName System.Drawing.Common
            $pngPath = Join-Path $OutputDirectory ([string]$firstPngFrame[0].png)
            $bitmap = [Drawing.Bitmap]::new($pngPath)
            try {
                $client = $firstPngFrame[0].client
                $left = [int]$client.left
                $top = [int]$client.top
                $width = [int]$client.width
                $height = [int]$client.height
                $points = @(
                    [pscustomobject]@{ X = $left + [int]($width * .25); Y = $top + [int]($height * .25) }
                    [pscustomobject]@{ X = $left + [int]($width * .75); Y = $top + [int]($height * .25) }
                    [pscustomobject]@{ X = $left + [int]($width * .25); Y = $top + [int]($height * .75) }
                    [pscustomobject]@{ X = $left + [int]($width * .75); Y = $top + [int]($height * .75) }
                    [pscustomobject]@{ X = $left + [int]($width * .50); Y = $top + [int]($height * .50) }
                )
                $keys = @($points | ForEach-Object {
                    $pixel = $bitmap.GetPixel([int]$_.X, [int]$_.Y)
                    '{0:X2}{1:X2}{2:X2}' -f $pixel.R, $pixel.G, $pixel.B
                } | Sort-Object -Unique)
                $distinctSamples = $keys.Count
            }
            finally { $bitmap.Dispose() }
        }
        $visible.status = if (
            $captureRun.ExitCode -eq 0 -and
            $process.ExitCode -eq 2 -and
            $capture.capturedFrames -gt 0 -and
            $capture.encodedPngFrames -gt 0 -and
            $capture.captureErrors -eq 0 -and
            $capture.captureRingDroppedFrames -eq 0 -and
            $capture.poolCapacityExceededFrames -eq 0 -and
            $distinctSamples -ge 4) { 'PASS-capture-transport-and-alpha-scene' } else { 'FAIL' }
        $visible.capturedFrames = $capture.capturedFrames
        $visible.encodedFrames = $capture.encodedPngFrames
        $visible.captureErrors = $capture.captureErrors
        $visible.distinctAlphaSceneSamples = $distinctSamples
        $visible.report = $capturePath
    }
    finally {
        if (-not $process.HasExited) { $process.Kill($true) }
        $process.Dispose()
    }
}

$visiblePass = $SkipVisibleCapture -or $visible.status -eq 'PASS-capture-transport-and-alpha-scene'
$validatorPass = $b0Pass -and $b1Pass -and $b2ExpectedFail -and $visiblePass
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$manifest = [ordered]@{
    schema = 'doroti.windows-acrylic-composition-b1-manifest/v1'
    runId = $runId
    status = if ($validatorPass) { 'PASS' } else { 'FAIL' }
    generatedAt = (Get-Date).ToUniversalTime().ToString('o')
    source = [ordered]@{
        revision = (& git -C $repoRoot rev-parse HEAD).Trim()
        dirtyFiles = @(& git -C $repoRoot status --short)
    }
    environment = [ordered]@{
        operatingSystem = $b0.operatingSystem
        windowsAppSdk = $b0.windowsAppSdk
        dpi = $b1.dpi
        dwmFlush = '0'
        eglSwapInterval = '1'
    }
    gates = [ordered]@{
        b0Capability = if ($b0Pass) { 'PASS' } else { 'FAIL' }
        b1TopologyAndDirectImport = if ($b1Pass) { 'PASS-capability' } else { 'FAIL' }
        b2SafeRetirement = 'FAIL'
        visibleCapture = $visible.status
        physicalBorderDrag = 'notVerified'
        inputImeUia = 'notVerified'
        deviceLoss = 'notVerified'
    }
    decision = [ordered]@{
        p1 = 'FAIL'
        reason = $b1.decision
        proceedToProductIntegration = $false
        opaqueRemainsDefault = $true
    }
    visible = $visible
    b0 = $b0
    b1 = $b1
}
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "B1 validator=$($manifest.status) B0=$($manifest.gates.b0Capability) B1=$($manifest.gates.b1TopologyAndDirectImport) B2=$($manifest.gates.b2SafeRetirement) P1=$($manifest.decision.p1) visible=$($manifest.gates.visibleCapture)"
Write-Host "manifest=$manifestPath"
if (-not $validatorPass) { exit 2 }
