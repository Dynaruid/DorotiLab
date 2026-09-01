#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [switch] $SkipBuild,
    [switch] $SkipVisibleCapture,
    [switch] $FullCurrentDpiMatrix,
    [ValidateSet('Left','Right','Top','Bottom','TopLeft','TopRight','BottomLeft','BottomRight')]
    [string] $CurrentEdge = 'Right'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$contractPath = Join-Path $repoRoot 'Doroti/validation/contracts/windows-experimental-acrylic.json'
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json -Depth 32
$runId = 'experimental-acrylic-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 10))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti/evidence/$runId"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$startingStatus = @(& git -C $repoRoot status --short)
$demoProject = Join-Path $repoRoot 'DorotiDemoApp/windowsappsdk/DorotiDemoApp.WindowsAppSdk.csproj'
$demoExecutable = Join-Path $repoRoot 'DorotiDemoApp/windowsappsdk/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiDemoApp.WindowsAppSdk.exe'
$opaqueProject = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj'
$opaqueExecutable = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.HwndExactCppProduct.exe'
$abiProject = Join-Path $repoRoot 'Doroti/validation/windowsappsdk-native-abi/Doroti.Validation.WindowsAppSdkNativeAbi.csproj'
$abiOutput = Join-Path $repoRoot 'Doroti/validation/windowsappsdk-native-abi/bin/Release/net10.0-windows10.0.19041.0/win-x64'
$abiExecutable = Join-Path $abiOutput 'Doroti.Validation.WindowsAppSdkNativeAbi.exe'
$abiNative = Join-Path $abiOutput 'doroti_windows_appsdk_host_v1.dll'
$observerBuild = Join-Path $repoRoot '.doroti/build/windows-resize-capture-experimental-acrylic'
$observer = Join-Path $observerBuild 'Release/Doroti.WindowsResizeCapture.exe'

if (-not ('Doroti.ExperimentalAcrylicValidation.NativeMethods' -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
namespace Doroti.ExperimentalAcrylicValidation {
    public static class NativeMethods {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessageW(IntPtr hwnd, uint message, IntPtr wparam, IntPtr lparam);
    }
}
'@
}

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function New-StartInfo {
    param([string] $FileName, [string[]] $ArgumentList, [hashtable] $Environment, [bool] $Visible = $false)
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.WindowStyle = if ($Visible) { [Diagnostics.ProcessWindowStyle]::Normal } else { [Diagnostics.ProcessWindowStyle]::Hidden }
    foreach ($argument in $ArgumentList) { [void]$start.ArgumentList.Add($argument) }
    if ($null -ne $Environment) {
        foreach ($entry in $Environment.GetEnumerator()) { $start.Environment[[string]$entry.Key] = [string]$entry.Value }
    }
    return $start
}

function Invoke-BoundedProcess {
    param(
        [Parameter(Mandatory)][string] $FileName,
        [string[]] $ArgumentList = @(),
        [Parameter(Mandatory)][string] $Name,
        [hashtable] $Environment = @{},
        [switch] $Visible
    )
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = New-StartInfo $FileName $ArgumentList $Environment $Visible.IsPresent
    Assert-True $process.Start() "$Name failed to start."
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        $process.Kill($true)
        throw "$Name exceeded the 20-minute timeout."
    }
    $result = [ordered]@{
        ExitCode = $process.ExitCode
        Stdout = $stdoutTask.GetAwaiter().GetResult()
        Stderr = $stderrTask.GetAwaiter().GetResult()
    }
    if (-not [string]::IsNullOrWhiteSpace($result.Stdout)) { Write-Host $result.Stdout.TrimEnd() }
    if (-not [string]::IsNullOrWhiteSpace($result.Stderr)) { Write-Host $result.Stderr.TrimEnd() }
    $process.Dispose()
    return $result
}

function Start-BoundedApp([hashtable] $Environment) {
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = New-StartInfo $demoExecutable @() $Environment $true
    Assert-True $process.Start() 'experimental Acrylic app failed to start.'
    return [ordered]@{
        Process = $process
        StdoutTask = $process.StandardOutput.ReadToEndAsync()
        StderrTask = $process.StandardError.ReadToEndAsync()
    }
}

function Complete-BoundedApp([System.Collections.IDictionary] $Running, [string] $Name) {
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

function Request-BoundedAppClose([long] $Hwnd) {
    Assert-True ([Doroti.ExperimentalAcrylicValidation.NativeMethods]::PostMessageW(
        [IntPtr]$Hwnd, 0x0010, [IntPtr]::Zero, [IntPtr]::Zero)) 'Failed to request graceful app close.'
}

function Invoke-DemoSmoke {
    param([string] $Name, [string] $ReportPath, [hashtable] $ExtraEnvironment = @{}, [switch] $EmptyPath)
    $environment = @{
        DOROTI_DEMO_EXPERIMENTAL_ACRYLIC = '1'
        DOROTI_WINDOWS_APPSDK_SMOKE_MS = '2500'
        DOROTI_WINDOWS_APPSDK_DIAGNOSTICS = '1'
        DOROTI_WINDOWS_APPSDK_REPORT = $ReportPath
    }
    foreach ($entry in $ExtraEnvironment.GetEnumerator()) { $environment[[string]$entry.Key] = [string]$entry.Value }
    if ($EmptyPath) { $environment.PATH = '' }
    $run = Invoke-BoundedProcess -FileName $demoExecutable -Name $Name -Environment $environment -Visible
    $report = if (Test-Path -LiteralPath $ReportPath -PathType Leaf) {
        Get-Content -LiteralPath $ReportPath -Raw | ConvertFrom-Json -Depth 100
    } else { $null }
    return [ordered]@{ exitCode=$run.ExitCode; report=$report; path=$ReportPath }
}

function Get-BoundedGeometry {
    param($App, $Capture, [string] $Edge, [string] $Profile = 'geometry')
    $presented = @{}
    foreach ($receipt in @($App.resize.receipts | Where-Object { [int]$_.terminal -eq 0 })) {
        $generation = [long]$receipt.target.generation
        Assert-True ($generation -gt 0 -and $generation -le 0xFFF) `
            "Visible validation resize generation exceeds the 12-bit marker contract: $generation"
        $presented[$generation] = $receipt
    }
    $scale = [double]$Capture.windowDpi / 96.0
    $activeBudget = [Math]::Min([int]$contract.geometry.physicalActiveEdgeBudget,
        [int][Math]::Ceiling([double]$contract.geometry.logicalActiveEdgeBudget * $scale))
    $matched = [Collections.Generic.List[object]]::new()
    foreach ($frame in @($Capture.frames | Where-Object { $null -ne $_.frameId })) {
        $visibleGeneration = [long]$frame.frameId
        if (-not $presented.ContainsKey($visibleGeneration)) { continue }
        $receipt = $presented[$visibleGeneration]
        $generation = [long]$receipt.target.generation
        $target = $receipt.target
        $widthDelta = [Math]::Abs([int]$frame.client.width - [int]$target.widthPx)
        $heightDelta = [Math]::Abs([int]$frame.client.height - [int]$target.heightPx)
        $horizontal = $Edge -in @('Left','Right','TopLeft','TopRight','BottomLeft','BottomRight')
        $vertical = $Edge -in @('Top','Bottom','TopLeft','TopRight','BottomLeft','BottomRight')
        $corner = $horizontal -and $vertical
        $activeDelta = [Math]::Max($(if ($horizontal) { $widthDelta } else { 0 }), $(if ($vertical) { $heightDelta } else { 0 }))
        # A corner drag has two active dimensions and therefore no orthogonal
        # extent to classify as inactive. Its stationary opposite edges are
        # validated separately with the asymmetric right/bottom scene markers.
        $inactiveDelta = if ($corner) { 0 } else {
            [Math]::Max($(if ($horizontal) { $heightDelta } else { 0 }), $(if ($vertical) { $widthDelta } else { 0 }))
        }
        $rightMarkerRequired = $Edge -in @('Left','TopLeft','BottomLeft')
        $bottomMarkerRequired = $Edge -in @('Top','TopLeft','TopRight')
        $oppositeEdgeMarkerMiss =
            ($rightMarkerRequired -and -not [bool]$frame.gridRightEdgeMarkerDetected) -or
            ($bottomMarkerRequired -and -not [bool]$frame.gridBottomEdgeMarkerDetected)
        $dragStart100ns = [double]$Capture.dragTiming.dragStartCounter * 10000000.0 / [double]$Capture.clockCalibration.qpcFrequency
        $mouseUp100ns = [double]$Capture.dragTiming.mouseUpCounter * 10000000.0 / [double]$Capture.clockCalibration.qpcFrequency
        $state = if ([long]$frame.systemRelative100ns -lt $dragStart100ns) { 'idle-before' }
            elseif ([long]$frame.systemRelative100ns -le $mouseUp100ns) { 'interactive' }
            else { 'settle' }
        $matched.Add([ordered]@{
            captureIndex=$frame.captureIndex; generation=$generation
            systemRelative100ns=$frame.systemRelative100ns
            state=$state; widthDelta=$widthDelta; heightDelta=$heightDelta
            activeEdgeDelta=$activeDelta; inactiveEdgeDelta=$inactiveDelta
            oppositeEdgeMarkerMiss=$oppositeEdgeMarkerMiss
        })
    }
    $interactive = @($matched | Where-Object state -eq 'interactive')
    $settle = @($matched | Where-Object state -eq 'settle')
    $interactiveReceipts = @($presented.Values | Where-Object {
        [long]$_.target.acceptedTimestamp -ge [long]$Capture.dragTiming.dragStartCounter -and
        [long]$_.target.acceptedTimestamp -le [long]$Capture.dragTiming.mouseUpCounter
    } | Sort-Object { [long]$_.target.acceptedTimestamp })
    $interactiveDurationSeconds =
        ([double]$Capture.dragTiming.mouseUpCounter - [double]$Capture.dragTiming.dragStartCounter) /
        [double]$Capture.clockCalibration.qpcFrequency
    $outerWindowChangeCounters = [Collections.Generic.List[long]]::new()
    $cursorActiveEdgeLags = [Collections.Generic.List[int]]::new()
    $lastOuterWindowRect = $null
    foreach ($sample in @($Capture.windowSamples | Where-Object {
        [long]$_.performanceCounter -ge [long]$Capture.dragTiming.dragStartCounter -and
        [long]$_.performanceCounter -le [long]$Capture.dragTiming.mouseUpCounter
    })) {
        $horizontalLag = switch ($Edge) {
            { $_ -in @('Left','TopLeft','BottomLeft') } {
                [Math]::Abs([int]$sample.intendedWindow.left - [int]$sample.window.left); break
            }
            { $_ -in @('Right','TopRight','BottomRight') } {
                [Math]::Abs([int]$sample.intendedWindow.right - [int]$sample.window.right); break
            }
            default { 0 }
        }
        $verticalLag = switch ($Edge) {
            { $_ -in @('Top','TopLeft','TopRight') } {
                [Math]::Abs([int]$sample.intendedWindow.top - [int]$sample.window.top); break
            }
            { $_ -in @('Bottom','BottomLeft','BottomRight') } {
                [Math]::Abs([int]$sample.intendedWindow.bottom - [int]$sample.window.bottom); break
            }
            default { 0 }
        }
        $cursorActiveEdgeLags.Add([Math]::Max($horizontalLag, $verticalLag))
        $rectKey = '{0},{1},{2},{3}' -f $sample.window.left,$sample.window.top,$sample.window.right,$sample.window.bottom
        if ($rectKey -eq $lastOuterWindowRect) { continue }
        $lastOuterWindowRect = $rectKey
        $outerWindowChangeCounters.Add([long]$sample.performanceCounter)
    }
    $outerWindowIntervalsMilliseconds = [Collections.Generic.List[double]]::new()
    for ($index = 1; $index -lt $outerWindowChangeCounters.Count; $index++) {
        $outerWindowIntervalsMilliseconds.Add(
            ([double]$outerWindowChangeCounters[$index] -
                [double]$outerWindowChangeCounters[$index - 1]) * 1000.0 /
            [double]$Capture.clockCalibration.qpcFrequency)
    }
    $acceptedIntervalsMilliseconds = [Collections.Generic.List[double]]::new()
    for ($index = 1; $index -lt $interactiveReceipts.Count; $index++) {
        $acceptedIntervalsMilliseconds.Add(
            ([double]$interactiveReceipts[$index].target.acceptedTimestamp -
                [double]$interactiveReceipts[$index - 1].target.acceptedTimestamp) * 1000.0 /
            [double]$Capture.clockCalibration.qpcFrequency)
    }
    $minimumAcceptedIntervalMilliseconds = if ($acceptedIntervalsMilliseconds.Count -gt 0) {
        ($acceptedIntervalsMilliseconds | Measure-Object -Minimum).Minimum
    } else { $null }
    $interactiveAcceptedFramesPerSecond = if ($interactiveDurationSeconds -gt 0) {
        [double]$interactiveReceipts.Count / $interactiveDurationSeconds
    } else { $null }
    $mouseUp100ns = [double]$Capture.dragTiming.mouseUpCounter * 10000000.0 / [double]$Capture.clockCalibration.qpcFrequency
    $refreshTicks = 10000000.0 / [Math]::Max(1, [double]$Capture.displayRefreshHz)
    $firstExactSettle = @($settle | Where-Object { $_.activeEdgeDelta -eq 0 -and $_.inactiveEdgeDelta -eq 0 } | Select-Object -First 1)
    $settleIntervals = if ($firstExactSettle.Count -eq 1) {
        ([double]$firstExactSettle[0].systemRelative100ns - $mouseUp100ns) / $refreshTicks
    } else { $null }
    $settleMilliseconds = if ($firstExactSettle.Count -eq 1) {
        ([double]$firstExactSettle[0].systemRelative100ns - $mouseUp100ns) / 10000.0
    } else { $null }
    $maximumActive = if ($interactive.Count) { ($interactive.activeEdgeDelta | Measure-Object -Maximum).Maximum } else { $null }
    $maximumInactive = if ($interactive.Count) { ($interactive.inactiveEdgeDelta | Measure-Object -Maximum).Maximum } else { $null }
    $oppositeEdgeMarkerMissCount = @($interactive | Where-Object oppositeEdgeMarkerMiss).Count
    $maximumCursorActiveEdgeLag = if ($cursorActiveEdgeLags.Count) {
        ($cursorActiveEdgeLags | Measure-Object -Maximum).Maximum
    } else { $null }
    $sortedCursorLags = @($cursorActiveEdgeLags | Sort-Object)
    $p95CursorActiveEdgeLag = if ($sortedCursorLags.Count) {
        $sortedCursorLags[[Math]::Floor(($sortedCursorLags.Count - 1) * 0.95)]
    } else { $null }
    $outerWindowChangesPerSecond = if ($interactiveDurationSeconds -gt 0) {
        [double]$outerWindowChangeCounters.Count / $interactiveDurationSeconds
    } else { $null }
    $minimumResponsiveFramesPerSecond = [double]$contract.scheduling.minimumResponsiveFramesPerSecond
    $framesPerSecondMeasurementTolerance =
        [double]$contract.scheduling.framesPerSecondMeasurementTolerance
    $schedulingPass = $null -ne $maximumCursorActiveEdgeLag -and
        $maximumCursorActiveEdgeLag -le [int]$contract.geometry.maximumCursorActiveEdgeLag -and
        $null -ne $outerWindowChangesPerSecond -and
        $outerWindowChangesPerSecond -ge
            ($minimumResponsiveFramesPerSecond - $framesPerSecondMeasurementTolerance) -and
        $null -ne $interactiveAcceptedFramesPerSecond -and
        $interactiveAcceptedFramesPerSecond -ge
            ($minimumResponsiveFramesPerSecond - $framesPerSecondMeasurementTolerance)
    $activeEdgePass = $maximumActive -le $activeBudget
    $oppositeEdgeMarkerGateApplied = $Profile -ne 'responsiveness' -or
        [bool]$contract.geometry.responsivenessRequiresOppositeEdgeMarker
    $settlePass = $null -ne $settleIntervals -and
        $settleIntervals -le [int]$contract.geometry.finalSettleRefreshIntervals -and
        $settleMilliseconds -le [double]$contract.geometry.finalSettleMaximumMilliseconds
    $pass = $matched.Count -gt 0 -and $interactive.Count -gt 0 -and
        $schedulingPass -and
        ($Profile -eq 'responsiveness' -or $activeEdgePass) -and
        $maximumInactive -le [int]$contract.geometry.inactiveEdgeBudget -and
        (-not $oppositeEdgeMarkerGateApplied -or $oppositeEdgeMarkerMissCount -eq 0) -and
        $settlePass
    return [ordered]@{
        status=if ($pass) { 'PASS' } else { 'FAIL' }
        profile=$Profile; edge=$Edge; scale=$scale; activeBudget=$activeBudget
        activeEdgeGateApplied=$Profile -ne 'responsiveness'
        activeEdgeBudgetSatisfied=$activeEdgePass
        inactiveBudget=[int]$contract.geometry.inactiveEdgeBudget
        matchedFrames=$matched.Count; interactiveFrames=$interactive.Count
        maximumActiveEdgeDelta=$maximumActive; maximumInactiveEdgeDelta=$maximumInactive
        oppositeEdgeMarkerGateApplied=$oppositeEdgeMarkerGateApplied
        oppositeEdgeMarkerMissCount=$oppositeEdgeMarkerMissCount
        finalSettleRefreshIntervals=$settleIntervals
        requiredFinalSettleRefreshIntervals=[int]$contract.geometry.finalSettleRefreshIntervals
        finalSettleMilliseconds=$settleMilliseconds
        requiredFinalSettleMaximumMilliseconds=[double]$contract.geometry.finalSettleMaximumMilliseconds
        scheduling=[ordered]@{
            status=if ($schedulingPass) { 'PASS' } else { 'FAIL' }
            model=[string]$contract.scheduling.model
            outerWindowPolicy=[string]$contract.scheduling.outerWindowPolicy
            metricsAuthority=[string]$contract.scheduling.metricsAuthority
            platformWaitMaximumMilliseconds=[int]$contract.scheduling.platformWaitMaximumMilliseconds
            exitFlush=[string]$contract.scheduling.exitFlush
            observedOuterWindowChanges=$outerWindowChangeCounters.Count
            observedOuterWindowChangesPerSecond=$outerWindowChangesPerSecond
            minimumSampledOuterWindowIntervalMilliseconds=if ($outerWindowIntervalsMilliseconds.Count -gt 0) {
                ($outerWindowIntervalsMilliseconds | Measure-Object -Minimum).Minimum
            } else { $null }
            acceptedInteractiveFrames=$interactiveReceipts.Count
            acceptedInteractiveFramesPerSecond=$interactiveAcceptedFramesPerSecond
            requiredMinimumResponsiveFramesPerSecond=$minimumResponsiveFramesPerSecond
            framesPerSecondMeasurementTolerance=$framesPerSecondMeasurementTolerance
            minimumAcceptedIntervalMilliseconds=$minimumAcceptedIntervalMilliseconds
            maximumCursorActiveEdgeLag=$maximumCursorActiveEdgeLag
            p95CursorActiveEdgeLag=$p95CursorActiveEdgeLag
            requiredMaximumCursorActiveEdgeLag=[int]$contract.geometry.maximumCursorActiveEdgeLag
        }
        frames=$matched
    }
}

if (-not $SkipBuild) {
    foreach ($project in @($demoProject, $opaqueProject, $abiProject)) {
        $build = Invoke-BoundedProcess -FileName 'dotnet' -Name "Release build $project" -ArgumentList @(
            'build',$project,'-c','Release','--nologo')
        Assert-True ($build.ExitCode -eq 0) "Build failed: $project"
    }
    $configure = Invoke-BoundedProcess -FileName 'cmake' -Name 'experimental Acrylic observer configure' -ArgumentList @(
        '-S',(Join-Path $repoRoot 'Doroti/validation/windows-resize-capture'),'-B',$observerBuild,'-A','x64')
    Assert-True ($configure.ExitCode -eq 0) 'Observer configure failed.'
    $observerBuildRun = Invoke-BoundedProcess -FileName 'cmake' -Name 'experimental Acrylic observer build' -ArgumentList @(
        '--build',$observerBuild,'--config','Release')
    Assert-True ($observerBuildRun.ExitCode -eq 0) 'Observer build failed.'
}
foreach ($path in @($demoExecutable,$opaqueExecutable,$abiExecutable,$abiNative,$observer)) {
    Assert-True (Test-Path -LiteralPath $path -PathType Leaf) "Required validation binary is missing: $path"
}

$abiRun = Invoke-BoundedProcess -FileName $abiExecutable -Name 'native ABI validation' -ArgumentList @($abiNative)
$opaqueBeforePath = Join-Path $OutputDirectory 'opaque-before.json'
$opaqueBeforeRun = Invoke-BoundedProcess -FileName $opaqueExecutable -Name 'opaque baseline before' -ArgumentList @('--report',$opaqueBeforePath) -Visible
$opaqueBefore = Get-Content -LiteralPath $opaqueBeforePath -Raw | ConvertFrom-Json -Depth 100

$emptyPath = Invoke-DemoSmoke -Name 'experimental Acrylic empty-PATH launch' -EmptyPath `
    -ReportPath (Join-Path $OutputDirectory 'experimental-empty-path.json')
$optionSmoke = Invoke-DemoSmoke -Name 'experimental Acrylic 500-option burst' `
    -ReportPath (Join-Path $OutputDirectory 'experimental-options.json') `
    -ExtraEnvironment @{ DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_OPTION_SMOKE='1' }
$fallback = Invoke-DemoSmoke -Name 'experimental Acrylic forced pre-show fallback' `
    -ReportPath (Join-Path $OutputDirectory 'experimental-fallback.json') `
    -ExtraEnvironment @{ DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FORCE_FALLBACK='1' }

$cases = @()
if (-not $SkipVisibleCapture) {
    $caseDefinitions = if ($FullCurrentDpiMatrix) {
        foreach ($run in 1..([int]$contract.qualification.requiredConsecutiveRuns)) {
            foreach ($edge in $contract.qualification.edges) {
                foreach ($motion in $contract.qualification.motions) {
                    foreach ($duration in $contract.qualification.dragMilliseconds) {
                        [ordered]@{
                            run=$run; edge=[string]$edge; motion=[string]$motion; duration=[int]$duration
                            profile=if ([int]$duration -eq 600) { 'responsiveness' } else { 'geometry' }
                        }
                    }
                }
            }
        }
    } else {
        @(
            [ordered]@{ run=1; edge=$CurrentEdge; motion='reverse'; duration=3000; profile='geometry' },
            [ordered]@{ run=1; edge=$CurrentEdge; motion='reverse'; duration=600; profile='responsiveness' }
        )
    }
    $caseIndex = 0
    foreach ($definition in $caseDefinitions) {
        $caseIndex++
        $slug = '{0:D3}-{1}-{2}-{3}' -f $caseIndex,$definition.edge,$definition.motion,$definition.duration
        $readyPath = Join-Path $OutputDirectory "$slug.ready.json"
        $appPath = Join-Path $OutputDirectory "$slug.app.json"
        $capturePath = Join-Path $OutputDirectory "$slug.capture.json"
        $environment = @{
            DOROTI_DEMO_EXPERIMENTAL_ACRYLIC='1'
            DOROTI_WINDOWS_APPSDK_SMOKE_MS=[string]([Math]::Min(120000, $definition.duration + 60000))
            DOROTI_WINDOWS_APPSDK_DIAGNOSTICS='1'
            DOROTI_WINDOWS_APPSDK_REPORT=$appPath
            DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=$readyPath
            DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FRAME_MARKER='1'
        }
        $running = Start-BoundedApp $environment
        try {
            $deadline = [DateTime]::UtcNow.AddSeconds(30)
            while (-not (Test-Path -LiteralPath $readyPath -PathType Leaf)) {
                if ($running.Process.HasExited) { throw "Experimental app exited before ready: $($running.Process.ExitCode)" }
                if ([DateTime]::UtcNow -ge $deadline) { throw 'Experimental Acrylic ready timeout.' }
                Start-Sleep -Milliseconds 50
            }
            $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
            $captureRun = Invoke-BoundedProcess -FileName $observer -Name "bounded resize $slug" -ArgumentList @(
                '--hwnd',[string]$ready.hwnd,'--output',$capturePath,'--run-id',"$runId-$slug",
                '--f6r','--decode-frame-id','--capture-only','--no-desktop-duplication','--input-hz','240','--png-stride','10',
                '--edge',[string]$definition.edge,'--drag-pixels','180','--drag-ms',[string]$definition.duration,
                '--motion',[string]$definition.motion)
            Assert-True ($captureRun.ExitCode -eq 0) "Bounded resize observer failed: $slug"
            Request-BoundedAppClose ([long]$ready.hwnd)
            $appExit = Complete-BoundedApp $running "experimental app $slug"
            $running = $null
            $app = Get-Content -LiteralPath $appPath -Raw | ConvertFrom-Json -Depth 100
            $capture = Get-Content -LiteralPath $capturePath -Raw | ConvertFrom-Json -Depth 100
            $geometry = Get-BoundedGeometry $app $capture ([string]$definition.edge) ([string]$definition.profile)
            $resourcePass = $appExit -eq 0 -and $app.mode.effective -eq 'experimentalAcrylic' -and
                $app.frames.gpuCopies -eq [int]$contract.resources.maximumCpuCopies -and
                $app.frames.operationalDebugErrors -eq [int]$contract.resources.maximumGpuErrors -and
                $app.resize.maximumQueueDepth -le [int]$contract.resources.maximumQueueDepth -and
                $app.acrylic.maximumRegisteredSlots -le [int]$contract.resources.maximumRegisteredSlots -and
                $app.acrylic.unavailableSkipCount -le [int]$contract.resources.maximumUnavailableReuseAttempts -and
                $app.resize.platformWaitTimeoutCount -eq 0 -and
                $app.resize.unterminatedCount -eq 0 -and $app.resize.duplicateTerminalCount -eq 0
            $transportPass = $captureRun.ExitCode -eq 0 -and $capture.capturedFrames -gt 0 -and
                $capture.captureErrors -eq 0 -and $capture.captureRingDroppedFrames -eq 0 -and
                $capture.poolCapacityExceededFrames -eq 0 -and @($capture.frames | Where-Object blank).Count -eq 0
            $cases += [ordered]@{
                name=$slug; definition=$definition
                status=if ($resourcePass -and $transportPass -and $geometry.status -eq 'PASS') { 'PASS' } else { 'FAIL' }
                resource=$resourcePass; captureTransport=$transportPass; geometry=$geometry
                appPath=$appPath; capturePath=$capturePath
            }
        }
        finally {
            if ($null -ne $running) {
                if (-not $running.Process.HasExited) { $running.Process.Kill($true) }
                $running.Process.Dispose()
            }
        }
    }
}

$opaqueAfterPath = Join-Path $OutputDirectory 'opaque-after.json'
$opaqueAfterRun = Invoke-BoundedProcess -FileName $opaqueExecutable -Name 'opaque baseline after' -ArgumentList @('--report',$opaqueAfterPath) -Visible
$opaqueAfter = Get-Content -LiteralPath $opaqueAfterPath -Raw | ConvertFrom-Json -Depth 100

$abiPass = $abiRun.ExitCode -eq 0
$opaquePass = $opaqueBeforeRun.ExitCode -eq 0 -and $opaqueAfterRun.ExitCode -eq 0 -and
    $opaqueBefore.status -eq 'PASS' -and $opaqueAfter.status -eq 'PASS'
$emptyPathPass = $emptyPath.exitCode -eq 0 -and $emptyPath.report.mode.effective -eq 'experimentalAcrylic'
$option = $optionSmoke.report.acrylic
$optionPass = $optionSmoke.exitCode -eq 0 -and $option.acceptedOptionRevisions -eq [int]$contract.runtimeOptions.requiredBurstRequests -and
    $option.failedOptionRevisions -eq 0 -and
    $option.appliedOptionRevisions + $option.supersededOptionRevisions -eq $option.acceptedOptionRevisions
$fallbackPass = $fallback.exitCode -eq 0 -and $fallback.report.mode.requested -eq 'experimentalAcrylic' -and
    $fallback.report.mode.effective -eq 'opaque' -and -not [string]::IsNullOrWhiteSpace($fallback.report.mode.fallbackReason)
$visiblePass = $SkipVisibleCapture -or (@($cases).Count -gt 0 -and @($cases | Where-Object status -ne 'PASS').Count -eq 0)
$automatedPartialPass = $abiPass -and $opaquePass -and $emptyPathPass -and $optionPass -and $fallbackPass -and $visiblePass
$videoControllers = @(Get-CimInstance Win32_VideoController | Select-Object Name,DriverVersion,CurrentRefreshRate,VideoModeDescription,PNPDeviceID)
$fullMatrixExecuted = [bool]$FullCurrentDpiMatrix -and -not $SkipVisibleCapture
$manifest = [ordered]@{
    schemaVersion='doroti.windows.experimental-acrylic-manifest/v1'
    runId=$runId
    status=if ($automatedPartialPass) { 'PASS-automated-partial' } else { 'FAIL' }
    generatedAt=[DateTime]::UtcNow.ToString('O')
    contract=$contractPath
    source=[ordered]@{ revision=(& git -C $repoRoot rev-parse HEAD).Trim(); statusAtStart=$startingStatus }
    environment=[ordered]@{
        operatingSystem=[Environment]::OSVersion.VersionString
        windowsBuild=[Environment]::OSVersion.Version.Build
        windowsAppSdk=[string]$contract.windowsAppSdk
        windowsSdk=[string]$contract.windowsSdk
        videoControllers=$videoControllers
    }
    scheduling=$contract.scheduling
    gates=[ordered]@{
        nativeAbi=if ($abiPass) { 'PASS' } else { 'FAIL' }
        opaqueBeforeAndAfter=if ($opaquePass) { 'PASS' } else { 'FAIL' }
        selfContainedEmptyPath=if ($emptyPathPass) { 'PASS' } else { 'FAIL' }
        runtimeOptionBurst500=if ($optionPass) { 'PASS' } else { 'FAIL' }
        deterministicPreShowFallback=if ($fallbackPass) { 'PASS' } else { 'FAIL' }
        boundedResizeCurrentCases=if ($SkipVisibleCapture) { 'notRun' } elseif ($visiblePass) { 'PASS' } else { 'FAIL' }
        fullCurrentDpiMatrix=if ($fullMatrixExecuted -and $visiblePass) { 'PASS' } elseif ($fullMatrixExecuted) { 'FAIL' } else { 'notRun' }
        dpiMatrix100_125_150_200='notVerified'
        refreshMatrix60_120_144_165='notVerified'
        monitorCrossingSnapOcclusionDeviceLoss='notVerified'
        automaticConsecutiveRuns3=if ($fullMatrixExecuted -and $visiblePass) { 'PASS-current-DPI-only' } else { 'notRun' }
        physicalBorderDragScanout='notVerified'
        koreanImeNarratorAccessibilityInsights='notVerified'
    }
    opaque=[ordered]@{ before=$opaqueBeforePath; after=$opaqueAfterPath }
    emptyPath=$emptyPath
    optionSmoke=$optionSmoke
    fallback=$fallback
    visibleCases=$cases
    decision=[ordered]@{
        productIntegration='implemented-opt-in'
        qualification=if ($automatedPartialPass) { 'partial' } else { 'failed' }
        exposeAsStableAcrylic=$false
        opaqueRemainsDefault=$true
        physicalAcceptance='notVerified'
    }
    evidenceBoundary='Automation covers product bootstrap, same-device zero-copy resources, option coalescing, deterministic fallback, and only the listed native-pointer/WGC cases. Unlisted DPI, refresh, monitor, window-management, device-loss, physical scan-out, IME, Narrator, and accessibility combinations are notVerified.'
}
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "experimental Acrylic validator=$($manifest.status) qualification=$($manifest.decision.qualification)"
Write-Host "manifest=$manifestPath"
if (-not $automatedPartialPass) { exit 2 }
exit 0
