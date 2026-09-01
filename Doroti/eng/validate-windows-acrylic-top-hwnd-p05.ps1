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
$project = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-top-hwnd-spike/Doroti.Validation.WindowsAcrylicTopHwndSpike.csproj'
$executable = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-top-hwnd-spike/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsAcrylicTopHwndSpike.exe'
$contractPath = Join-Path $repoRoot 'Doroti/validation/contracts/windows-acrylic-top-hwnd-p05.json'
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json -Depth 32
$runId = 'acrylic-p05-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 12))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti/evidence/$runId"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$originalDwmFlush = [Environment]::GetEnvironmentVariable('DOROTI_WINDOWS_DWM_FLUSH', 'Process')
$originalSwapInterval = [Environment]::GetEnvironmentVariable('DOROTI_WINDOWS_EGL_SWAP_INTERVAL', 'Process')
$startingStatus = @(& git -C $repoRoot status --short)

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function New-ProcessStartInfo {
    param([string] $FileName, [string[]] $ArgumentList, [bool] $Visible = $false)
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.WindowStyle = if ($Visible) {
        [Diagnostics.ProcessWindowStyle]::Normal
    } else {
        [Diagnostics.ProcessWindowStyle]::Hidden
    }
    $start.Environment['DOROTI_WINDOWS_DWM_FLUSH'] = '0'
    $start.Environment['DOROTI_WINDOWS_EGL_SWAP_INTERVAL'] = '1'
    foreach ($argument in $ArgumentList) { [void] $start.ArgumentList.Add($argument) }
    return $start
}

function Invoke-BoundedProcess {
    param(
        [Parameter(Mandatory)][string] $FileName,
        [Parameter(Mandatory)][string[]] $ArgumentList,
        [Parameter(Mandatory)][string] $Name
    )
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = New-ProcessStartInfo -FileName $FileName -ArgumentList $ArgumentList
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
    $result = [ordered]@{ ExitCode = $process.ExitCode; Stdout = $stdout; Stderr = $stderr }
    $process.Dispose()
    return $result
}

function Start-BoundedProcess {
    param([string] $FileName, [string[]] $ArgumentList)
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = New-ProcessStartInfo -FileName $FileName -ArgumentList $ArgumentList -Visible $true
    Assert-True $process.Start() 'Visible P0.5 process failed to start.'
    return [ordered]@{
        Process = $process
        StdoutTask = $process.StandardOutput.ReadToEndAsync()
        StderrTask = $process.StandardError.ReadToEndAsync()
    }
}

function Complete-BoundedProcess {
    param([System.Collections.IDictionary] $Running, [string] $Name)
    $process = $Running.Process
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        $process.Kill($true)
        throw "$Name exceeded the 20-minute timeout."
    }
    $stdout = $Running.StdoutTask.GetAwaiter().GetResult()
    $stderr = $Running.StderrTask.GetAwaiter().GetResult()
    if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
    if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
    $exitCode = $process.ExitCode
    $process.Dispose()
    return $exitCode
}

function Get-Percentile([double[]] $Values, [double] $Percentile) {
    if ($Values.Count -eq 0) { return $null }
    $sorted = @($Values | Sort-Object)
    $index = [Math]::Min($sorted.Count - 1, [Math]::Max(0, [Math]::Ceiling($Percentile * $sorted.Count) - 1))
    return [double]$sorted[$index]
}

function Get-RenderTiming($Report) {
    $frequency = [Diagnostics.Stopwatch]::Frequency
    $values = @($Report.frames | ForEach-Object {
        ([double]$_.endQpc - [double]$_.beginQpc) * 1000000.0 / $frequency
    })
    return [ordered]@{
        count = $values.Count
        p50Microseconds = Get-Percentile $values 0.50
        p95Microseconds = Get-Percentile $values 0.95
        maxMicroseconds = if ($values.Count -eq 0) { $null } else { ($values | Measure-Object -Maximum).Maximum }
    }
}

function Invoke-AutomatedCase([string] $Name, [string] $Arm, [string] $Alpha) {
    $reportPath = Join-Path $OutputDirectory "$Name.json"
    $arguments = @(
        '--top-direct', '--arm', $Arm, '--alpha', $Alpha, '--automated',
        '--resize-count', [string]$contract.automatedResizeCount,
        '--duration-ms', '100', '--report', $reportPath)
    if ($Arm -eq 'controller') { $arguments += @('--custom', '--kind', 'thin') }
    $run = Invoke-BoundedProcess -FileName $executable -ArgumentList $arguments -Name "P0.5 $Name"
    Assert-True (Test-Path -LiteralPath $reportPath -PathType Leaf) "P0.5 $Name did not write its report."
    return [ordered]@{
        exitCode = $run.ExitCode
        report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json -Depth 100
        reportPath = $reportPath
    }
}

function Get-PngStatistics($Capture) {
    Add-Type -AssemblyName System.Drawing.Common
    $samples = [Collections.Generic.List[object]]::new()
    $alphaSceneColors = [Collections.Generic.HashSet[string]]::new()
    $blankFrames = 0
    foreach ($frame in @($Capture.frames | Where-Object { $_.png })) {
        $path = Join-Path $OutputDirectory ([string]$frame.png)
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { continue }
        $bitmap = [Drawing.Bitmap]::new($path)
        try {
            $client = $frame.client
            $left = [int]$client.left
            $top = [int]$client.top
            $width = [int]$client.width
            $height = [int]$client.height
            if ($width -le 8 -or $height -le 8) { continue }
            $y = [Math]::Min($bitmap.Height - 1, $top + [int]($height * .55))
            $transparent = $bitmap.GetPixel(
                [Math]::Min($bitmap.Width - 1, $left + [int]($width * .10)), $y)
            $samples.Add([ordered]@{ r=$transparent.R; g=$transparent.G; b=$transparent.B })
            foreach ($ratio in @(.10, .30, .50, .70, .90)) {
                $pixel = $bitmap.GetPixel(
                    [Math]::Min($bitmap.Width - 1, $left + [int]($width * $ratio)), $y)
                [void]$alphaSceneColors.Add(('{0:X2}{1:X2}{2:X2}' -f $pixel.R,$pixel.G,$pixel.B))
            }
            $probe = @(
                $bitmap.GetPixel($left + [int]($width * .2), $top + [int]($height * .2)),
                $bitmap.GetPixel($left + [int]($width * .8), $top + [int]($height * .2)),
                $bitmap.GetPixel($left + [int]($width * .2), $top + [int]($height * .8)),
                $bitmap.GetPixel($left + [int]($width * .8), $top + [int]($height * .8)))
            $probeKeys = @($probe | ForEach-Object {
                '{0:X2}{1:X2}{2:X2}' -f $_.R,$_.G,$_.B
            } | Sort-Object -Unique)
            if ($probeKeys.Count -eq 1) { $blankFrames++ }
        }
        finally { $bitmap.Dispose() }
    }
    $median = [ordered]@{ r=0; g=0; b=0 }
    if ($samples.Count -gt 0) {
        foreach ($channel in @('r','g','b')) {
            $ordered = @($samples | ForEach-Object { [int]$_[$channel] } | Sort-Object)
            $median[$channel] = $ordered[[int][Math]::Floor($ordered.Count / 2)]
        }
    }
    $roiKeys = @($samples | ForEach-Object { '{0:X2}{1:X2}{2:X2}' -f $_.r,$_.g,$_.b } | Sort-Object -Unique)
    return [ordered]@{
        pngCount = $samples.Count
        transparentRoiMedian = $median
        transparentRoiDistinctColors = $roiKeys.Count
        alphaSceneDistinctColors = $alphaSceneColors.Count
        blankPngFrames = $blankFrames
    }
}

function Get-CaptureExactness($Report, $Capture) {
    $renderById = @{}
    foreach ($frame in @($Report.frames)) {
        $renderById[[int]([uint64]$frame.generation -band 0xFFF)] = $frame
    }
    $matched = 0
    $mismatch = 0
    $latencies = [Collections.Generic.List[double]]::new()
    $firstCapture = @{}
    $frequency = [double]$Capture.clockCalibration.qpcFrequency
    foreach ($frame in @($Capture.frames | Where-Object { $null -ne $_.frameId })) {
        $id = [int]$frame.frameId
        if (-not $renderById.ContainsKey($id)) { continue }
        $render = $renderById[$id]
        $matched++
        if ([int]$frame.client.width -ne [int]$render.width -or
            [int]$frame.client.height -ne [int]$render.height) { $mismatch++ }
        $captureQpc = [double]$frame.systemRelative100ns * $frequency / 10000000.0
        $endQpc = [double]$render.endQpc
        if ($captureQpc -ge $endQpc -and (-not $firstCapture.ContainsKey($id))) {
            $firstCapture[$id] = $true
            $latencies.Add(($captureQpc - $endQpc) * 1000000.0 / $frequency)
        }
    }
    return [ordered]@{
        matchedFrames = $matched
        wrongSizeOrStaleFrames = $mismatch
        visibleLatencyCount = $latencies.Count
        visibleLatencyP50Microseconds = Get-Percentile ($latencies.ToArray()) 0.50
        visibleLatencyP95Microseconds = Get-Percentile ($latencies.ToArray()) 0.95
        visibleLatencyMaxMicroseconds = if ($latencies.Count -eq 0) { $null } else { ($latencies | Measure-Object -Maximum).Maximum }
    }
}

function Invoke-VisibleCase([string] $Name, [string] $Arm, [string] $Alpha, [bool] $Churn) {
    $readyPath = Join-Path $OutputDirectory "$Name.ready.json"
    $reportPath = Join-Path $OutputDirectory "$Name-app.json"
    $capturePath = Join-Path $OutputDirectory "$Name-capture.json"
    $arguments = @('--top-direct','--arm',$Arm,'--alpha',$Alpha,'--duration-ms','9000','--ready',$readyPath,'--report',$reportPath)
    if ($Arm -eq 'controller') { $arguments += @('--custom','--kind','thin') }
    if ($Churn) { $arguments += '--runtime-churn' }
    $running = Start-BoundedProcess -FileName $executable -ArgumentList $arguments
    try {
        $deadline = [DateTime]::UtcNow.AddSeconds(10)
        while (-not (Test-Path -LiteralPath $readyPath -PathType Leaf)) {
            if ($running.Process.HasExited) { throw "P0.5 $Name exited before ready." }
            if ([DateTime]::UtcNow -ge $deadline) { throw "P0.5 $Name ready timeout." }
            Start-Sleep -Milliseconds 50
        }
        $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
        $hwndText = [string]$ready.topHwnd
        $hwndDecimal = [Convert]::ToInt64($hwndText.Substring(2), 16)
        $captureRun = $null
        for ($attempt = 1; $attempt -le 3; $attempt++) {
            $captureRun = Invoke-BoundedProcess -FileName $script:observer -Name "P0.5 $Name WGC attempt $attempt" -ArgumentList @(
                '--hwnd',[string]$hwndDecimal,
                '--output',$capturePath,
                '--run-id',"$runId-$Name",
                '--duration','4','--input-hz','240','--png-stride','5',
                '--capture-only','--no-desktop-duplication',
                '--edge','Right','--drag-pixels','180','--drag-ms','3000','--motion','reverse')
            if ($captureRun.ExitCode -eq 0 -and (Test-Path -LiteralPath $capturePath -PathType Leaf)) { break }
            Start-Sleep -Milliseconds 250
        }
        $appExit = Complete-BoundedProcess -Running $running -Name "P0.5 $Name"
        $running = $null
        Assert-True (Test-Path -LiteralPath $reportPath -PathType Leaf) "P0.5 $Name app report is missing."
        Assert-True (Test-Path -LiteralPath $capturePath -PathType Leaf) "P0.5 $Name capture report is missing."
        $report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json -Depth 100
        $capture = Get-Content -LiteralPath $capturePath -Raw | ConvertFrom-Json -Depth 100
        return [ordered]@{
            appExitCode = $appExit
            captureExitCode = $captureRun.ExitCode
            report = $report
            capture = $capture
            png = Get-PngStatistics $capture
            exactness = Get-CaptureExactness $report $capture
            renderTiming = Get-RenderTiming $report
            reportPath = $reportPath
            capturePath = $capturePath
        }
    }
    finally {
        if ($null -ne $running) {
            if (-not $running.Process.HasExited) { $running.Process.Kill($true) }
            $running.Process.Dispose()
        }
    }
}

if (-not $SkipBuild) {
    $build = Invoke-BoundedProcess -FileName 'dotnet' -Name 'P0.5 spike build' -ArgumentList @(
        'build',$project,'-c','Release','--nologo')
    Assert-True ($build.ExitCode -eq 0) 'P0.5 spike build failed.'
}
Assert-True (Test-Path -LiteralPath $executable -PathType Leaf) "P0.5 executable is missing: $executable"

$observerBuild = Join-Path $repoRoot '.doroti/build/windows-resize-capture-acrylic-p05'
$script:observer = Join-Path $observerBuild 'Release/Doroti.WindowsResizeCapture.exe'
if (-not $SkipVisibleCapture -and -not (Test-Path -LiteralPath $script:observer -PathType Leaf)) {
    $configure = Invoke-BoundedProcess -FileName 'cmake' -Name 'P0.5 capture configure' -ArgumentList @(
        '-S',(Join-Path $repoRoot 'Doroti/validation/windows-resize-capture'),'-B',$observerBuild,'-A','x64')
    Assert-True ($configure.ExitCode -eq 0) 'P0.5 capture configure failed.'
    $observerBuildRun = Invoke-BoundedProcess -FileName 'cmake' -Name 'P0.5 capture build' -ArgumentList @(
        '--build',$observerBuild,'--config','Release')
    Assert-True ($observerBuildRun.ExitCode -eq 0) 'P0.5 capture build failed.'
}

$opaqueBefore = Invoke-AutomatedCase 'opaque-before' 'opaque' 'off'
$candidate = Invoke-AutomatedCase 'candidate' 'controller' 'top'
$opaqueAfter = Invoke-AutomatedCase 'opaque-after' 'opaque' 'off'
$automatedCorePass = @(@($opaqueBefore,$candidate,$opaqueAfter) | Where-Object {
    $_.exitCode -ne 0 -or $_.report.status -ne 'PASS' -or
    $_.report.topology -ne 'top-hwnd-direct' -or
    $_.report.visibleHwndCount -ne [int]$contract.requiredVisibleHwndCount -or
    $_.report.visibleChildRenderHwndCount -ne [int]$contract.requiredVisibleChildRenderHwndCount -or
    $_.report.createdChildRenderHwndCount -ne 0 -or
    $_.report.maximumRenderQueueDepth -gt [int]$contract.maximumQueueDepth -or
    $_.report.exactMismatchCount -ne 0 -or $_.report.renderFailureCount -ne 0 -or
    $_.report.operationalGpuErrorCount -ne 0 -or
    $_.report.presentCount -ne $_.report.gpuSubmitCount -or
    $_.report.presentCount -ne $_.report.gpuCopyCount
}).Count -eq 0
$topAlphaPass = @($candidate.report.dwmCalls | Where-Object {
    $_.attribute -eq 39 -and $_.hResult -eq 0
}).Count -eq 1
$controllerPass = $candidate.report.setTargetResult -and
    $candidate.report.controllerCreateCount -eq 1 -and
    $candidate.report.targetCreateCount -eq 1 -and
    $candidate.report.rootCreateCount -eq 1 -and
    $candidate.report.updates.acceptedCount -eq [int]$contract.requiredBackdropUpdates -and
    $candidate.report.updates.maxPendingDepth -le 1 -and
    $candidate.report.updates.duplicateTerminalCount -eq 0 -and
    $candidate.report.updates.missingTerminalCount -eq 0
$inputPass = $candidate.report.focusMessageCount -gt 0 -and
    $candidate.report.pointerMessageCount -gt 0 -and
    $candidate.report.keyboardMessageCount -gt 0 -and
    $candidate.report.cursorMessageCount -gt 0 -and
    $candidate.report.hitTestResult -ne 0

$visible = [ordered]@{ status='notVerified'; reason='SkipVisibleCapture'; opaqueBefore=$null; candidate=$null; opaqueAfter=$null }
$visiblePass = [bool]$SkipVisibleCapture
$timingPass = [bool]$SkipVisibleCapture
if (-not $SkipVisibleCapture) {
    $visible.opaqueBefore = Invoke-VisibleCase 'visible-opaque-before' 'opaque' 'off' $false
    $visible.candidate = Invoke-VisibleCase 'visible-candidate' 'controller' 'top' $true
    $visible.opaqueAfter = Invoke-VisibleCase 'visible-opaque-after' 'opaque' 'off' $false
    $captures = @($visible.opaqueBefore,$visible.candidate,$visible.opaqueAfter)
    $captureTransportPass = @($captures | Where-Object {
        $_.appExitCode -ne 0 -or $_.captureExitCode -ne 0 -or
        $_.capture.capturedFrames -le 0 -or $_.capture.encodedPngFrames -le 0 -or
        $_.capture.captureErrors -gt [int]$contract.maximumCaptureErrors -or
        $_.capture.captureRingDroppedFrames -gt [int]$contract.maximumCaptureRingDrops -or
        $_.capture.poolCapacityExceededFrames -ne 0 -or
        $_.exactness.matchedFrames -lt 10 -or
        $_.exactness.wrongSizeOrStaleFrames -ne 0 -or
        $_.png.blankPngFrames -ne 0 -or $_.png.alphaSceneDistinctColors -lt 4
    }).Count -eq 0
    $candidateMedian = $visible.candidate.png.transparentRoiMedian
    $opaqueMedians = @($visible.opaqueBefore.png.transparentRoiMedian,$visible.opaqueAfter.png.transparentRoiMedian)
    $roiDistance = ($opaqueMedians | ForEach-Object {
        [Math]::Sqrt(
            [Math]::Pow([double]$candidateMedian.r - [double]$_.r,2) +
            [Math]::Pow([double]$candidateMedian.g - [double]$_.g,2) +
            [Math]::Pow([double]$candidateMedian.b - [double]$_.b,2))
    } | Measure-Object -Minimum).Minimum
    $acrylicObserved = $roiDistance -ge 6 -and
        $visible.candidate.png.transparentRoiDistinctColors -ge 2
    $refreshHz = [Math]::Max(1.0,[double]$visible.candidate.capture.displayRefreshHz)
    $refreshMicroseconds = 1000000.0 / $refreshHz
    $opaqueRenderP95 = [Math]::Max(
        [double]$visible.opaqueBefore.renderTiming.p95Microseconds,
        [double]$visible.opaqueAfter.renderTiming.p95Microseconds)
    $opaqueRenderMax = [Math]::Max(
        [double]$visible.opaqueBefore.renderTiming.maxMicroseconds,
        [double]$visible.opaqueAfter.renderTiming.maxMicroseconds)
    $opaqueVisibleP95 = [Math]::Max(
        [double]$visible.opaqueBefore.exactness.visibleLatencyP95Microseconds,
        [double]$visible.opaqueAfter.exactness.visibleLatencyP95Microseconds)
    $opaqueVisibleMax = [Math]::Max(
        [double]$visible.opaqueBefore.exactness.visibleLatencyMaxMicroseconds,
        [double]$visible.opaqueAfter.exactness.visibleLatencyMaxMicroseconds)
    $timingPass =
        [double]$visible.candidate.renderTiming.p95Microseconds -le
            $opaqueRenderP95 + [int]$contract.timing.p95RefreshIntervalsAboveOpaque * $refreshMicroseconds -and
        [double]$visible.candidate.renderTiming.maxMicroseconds -le
            $opaqueRenderMax + [int]$contract.timing.maxRefreshIntervalsAboveOpaque * $refreshMicroseconds -and
        [double]$visible.candidate.exactness.visibleLatencyP95Microseconds -le
            $opaqueVisibleP95 + [int]$contract.timing.p95RefreshIntervalsAboveOpaque * $refreshMicroseconds -and
        [double]$visible.candidate.exactness.visibleLatencyMaxMicroseconds -le
            $opaqueVisibleMax + [int]$contract.timing.maxRefreshIntervalsAboveOpaque * $refreshMicroseconds
    $visiblePass = $captureTransportPass -and $acrylicObserved
    $visible.status = if ($visiblePass) { 'PASS' } else { 'FAIL' }
    $visible.reason = if (-not $captureTransportPass) { 'capture-transport-or-exactness' }
        elseif (-not $acrylicObserved) { 'acrylic-transparent-roi-differential-not-observed' }
        else { 'acrylic-roi-and-exact-marker-observed' }
    $visible['roiDistanceFromOpaque'] = $roiDistance
    $visible['acrylicObserved'] = $acrylicObserved
    $visible['timingPass'] = $timingPass
    $visible['refreshIntervalMicroseconds'] = $refreshMicroseconds
}

$automatedPass = $automatedCorePass -and $topAlphaPass -and $controllerPass -and
    $inputPass -and $visiblePass -and $timingPass
$p05Decision = if ($automatedPass) { 'notVerified' } else { 'FAIL' }
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$video = @(Get-CimInstance Win32_VideoController | Select-Object Name,DriverVersion,CurrentRefreshRate,VideoModeDescription,PNPDeviceID)
$manifest = [ordered]@{
    schema = 'doroti.windows-acrylic-top-hwnd-p05-manifest/v1'
    runId = $runId
    status = if ($automatedPass) { 'PASS-automated' } else { 'FAIL' }
    generatedAt = (Get-Date).ToUniversalTime().ToString('o')
    source = [ordered]@{
        revision = (& git -C $repoRoot rev-parse HEAD).Trim()
        startingDirtyFiles = $startingStatus
        endingDirtyFiles = @(& git -C $repoRoot status --short)
    }
    environment = [ordered]@{
        operatingSystem = $candidate.report.operatingSystem
        windowsBuild = $candidate.report.windowsBuild
        windowsAppSdk = $contract.windowsAppSdk
        windowsSdk = $contract.windowsSdk
        dpi = $candidate.report.dpi
        adapterDescription = $candidate.report.adapterDescription
        videoControllers = $video
        parentDwmFlush = if ($null -eq $originalDwmFlush) { 'unset' } else { $originalDwmFlush }
        parentEglSwapInterval = if ($null -eq $originalSwapInterval) { 'unset' } else { $originalSwapInterval }
        comparisonDwmFlush = '0'
        comparisonEglSwapInterval = '1'
    }
    gates = [ordered]@{
        automatedCore = if ($automatedCorePass) { 'PASS' } else { 'FAIL' }
        topRedirectionAlpha = if ($topAlphaPass) { 'PASS' } else { 'FAIL' }
        controllerAndOptions = if ($controllerPass) { 'PASS' } else { 'FAIL' }
        oneVisibleOwner = if ($automatedCorePass) { 'PASS' } else { 'FAIL' }
        automaticInputMessages = if ($inputPass) { 'PASS' } else { 'FAIL' }
        visibleCaptureAndAcrylicRoi = if ($SkipVisibleCapture) { 'notVerified' } elseif ($visiblePass) { 'PASS' } else { 'FAIL' }
        timingAgainstOpaque = if ($SkipVisibleCapture) { 'notVerified' } elseif ($timingPass) { 'PASS' } else { 'FAIL' }
        physicalBorderDrag = 'notVerified'
        monitorDpiCrossing = 'notVerified'
        imeClipboardUia = 'notVerified'
        policyDeviceLossRdp = 'notVerified'
    }
    decision = [ordered]@{
        p05 = $p05Decision
        reason = if ($automatedPass) {
            'Automated API, exact-frame, WGC ROI, native pointer resize, and timing gates passed; physical scan-out, IME, UIA, monitor/DPI, and policy acceptance remain notVerified.'
        } else {
            'One or more P0.5 automated hard gates failed; proceed to P1-CS capability and presenter investigation.'
        }
        proceedToP1Cs = -not $automatedPass
        proceedToProductIntegration = $false
        opaqueRemainsDefault = $true
    }
    automated = [ordered]@{ opaqueBefore=$opaqueBefore; candidate=$candidate; opaqueAfter=$opaqueAfter }
    visible = $visible
}
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "P0.5 validator=$($manifest.status) decision=$($manifest.decision.p05) visible=$($manifest.gates.visibleCaptureAndAcrylicRoi) timing=$($manifest.gates.timingAgainstOpaque)"
Write-Host "manifest=$manifestPath"
if (-not $automatedPass) { exit 2 }
