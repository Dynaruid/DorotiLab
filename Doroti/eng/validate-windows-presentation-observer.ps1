#Requires -Version 7.0
param(
    [ValidateRange(1, 10)]
    [int] $Runs = 3,

    [ValidateRange(320, 1200)]
    [int] $LogicalWidth = 420,

    [ValidateRange(240, 720)]
    [int] $LogicalHeight = 300,

    [string] $EvidenceDirectory = (Join-Path $PSScriptRoot '../validation/evidence/resize')
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$evidenceRoot = [IO.Path]::GetFullPath($EvidenceDirectory)
$allowedEvidenceRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'validation/evidence/resize'))
if (-not $evidenceRoot.StartsWith($allowedEvidenceRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Evidence directory must stay under $allowedEvidenceRoot"
}
[IO.Directory]::CreateDirectory($evidenceRoot) | Out-Null

$app = Join-Path $dorotiRoot 'validation/windows-top-level-presentation/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsTopLevelPresentation.exe'
$capture = Join-Path $repoRoot '.doroti/build/windows-resize-capture-vs/Release/Doroti.WindowsResizeCapture.exe'
foreach ($required in @($app, $capture)) {
    if (-not (Test-Path -LiteralPath $required -PathType Leaf)) {
        throw "Build the observer prerequisite first: $required"
    }
}

if (-not ('DorotiObserverNative' -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;

public static class DorotiObserverNative
{
    public const uint WM_CLOSE = 0x0010;
    public const uint WM_CANCELMODE = 0x001F;
    public const uint SWP_SHOWWINDOW = 0x0040;
    public const uint SPI_GETWORKAREA = 0x0030;
    public static readonly IntPtr PMV2 = new IntPtr(-4);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }

    [DllImport("user32.dll")]
    public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr value);

    [DllImport("user32.dll")]
    public static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(
        IntPtr hwnd, IntPtr after, int x, int y, int width, int height, uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SystemParametersInfo(uint action, uint parameter, ref RECT value, uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);
}
'@
}

function Wait-ForWindow([Diagnostics.Process] $Process, [TimeSpan] $Timeout) {
    $deadline = [DateTime]::UtcNow + $Timeout
    while ([DateTime]::UtcNow -lt $deadline) {
        if ($Process.HasExited) { throw "Top-level control exited before creating its window (exit $($Process.ExitCode))." }
        $Process.Refresh()
        if ($Process.MainWindowHandle -ne [IntPtr]::Zero) { return $Process.MainWindowHandle }
        Start-Sleep -Milliseconds 100
    }
    throw 'Top-level control did not create a native window within 20 seconds.'
}

function Get-Percentile([double[]] $Values, [double] $Percentile) {
    if ($Values.Count -eq 0) { return $null }
    $ordered = @($Values | Sort-Object)
    $index = [Math]::Ceiling($ordered.Count * $Percentile) - 1
    return [double]$ordered[[Math]::Max(0, [Math]::Min($ordered.Count - 1, $index))]
}

function Get-Intervals([object[]] $Records, [string] $Property) {
    $ordered = @($Records | Sort-Object { [long]$_.$Property })
    $result = [Collections.Generic.List[double]]::new()
    for ($index = 1; $index -lt $ordered.Count; $index++) {
        $current = [long]$ordered[$index].$Property
        $previous = [long]$ordered[$index - 1].$Property
        if ($current -gt $previous) { $result.Add([double]($current - $previous)) }
    }
    return $result.ToArray()
}

function Get-SourceFingerprint {
    $relativePaths = @(
        'Doroti/validation/windows-resize-capture/main.cpp',
        'Doroti/validation/windows-resize-capture/CMakeLists.txt',
        'Doroti/validation/windows-top-level-presentation/Program.cs',
        'Doroti/eng/validate-windows-presentation-observer.ps1')
    $builder = [Text.StringBuilder]::new()
    foreach ($relativePath in ($relativePaths | Sort-Object)) {
        $path = Join-Path $repoRoot $relativePath
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
        [void]$builder.Append($relativePath).Append('=').Append($hash).Append("`n")
    }
    return [Convert]::ToHexString(
        [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($builder.ToString()))
    ).ToLowerInvariant()
}

function Measure-Qualification([object] $Evidence, [object] $AppEvidence) {
    $frequency = [double]$Evidence.clockCalibration.qpcFrequency
    $refreshHz = [double]$Evidence.displayRefreshHz
    $refreshTicks = $frequency / $refreshHz
    $refreshMicroseconds = 1000000.0 / $refreshHz
    $scale = [double]$Evidence.initialGeometry.rasterScale
    $edgeTolerance = [Math]::Max(1, [Math]::Ceiling($scale))
    $staticStage = @($Evidence.qualificationStages | Where-Object name -eq 'static')[0]
    $wgcStatic = @($Evidence.frames | Where-Object {
        [long]$_.callbackEntryCounter -ge [long]$staticStage.startCounter -and
        [long]$_.callbackEntryCounter -le [long]$staticStage.endCounter
    })
    $desktopStatic = @($Evidence.desktopFrames | Where-Object {
        [long]$_.acquireEntryCounter -ge [long]$staticStage.startCounter -and
        [long]$_.acquireEntryCounter -le [long]$staticStage.endCounter
    })
    $staticFailures = @($wgcStatic + $desktopStatic | Where-Object {
        $_.blank -or
        $null -eq $_.appBarLogicalHeight -or
        ($null -ne $_.circleAspect -and [Math]::Abs([double]$_.circleAspect - 1.0) -gt ([Math]::Max(1.0, [Math]::Ceiling($scale)) / 18.0)) -or
        ($null -ne $_.titleScaleRatio -and [Math]::Abs([double]$_.titleScaleRatio - 1.0) -gt 0.04) -or
        [int]$_.contentLeftGap -lt 0 -or [int]$_.contentRightGap -lt 0 -or
        [int]$_.contentLeftGap -gt $edgeTolerance -or [int]$_.contentRightGap -gt $edgeTolerance
    })

    $phaseResults = [Collections.Generic.List[object]]::new()
    foreach ($event in @($Evidence.qualificationEvents)) {
        $eventStart = [Math]::Min([long]$event.contentIssueCounter, [long]$event.geometryIssueCounter)
        $before = $Evidence.desktopFrames |
            Where-Object { [long]$_.acquireEntryCounter -lt $eventStart -and $null -ne $_.frameId } |
            Sort-Object acquireEntryCounter |
            Select-Object -Last 1
        $baselineId = if ($null -ne $before) { [int]$before.frameId } else { $null }
        $targetId = [int]$event.contentFrameId -band 0x0fff
        $contentFrame = $Evidence.desktopFrames |
            Where-Object {
                [long]$_.acquireEntryCounter -ge [long]$event.contentIssueCounter -and
                $null -ne $_.frameId -and [int]$_.frameId -eq $targetId
            } |
            Sort-Object acquireEntryCounter |
            Select-Object -First 1
        $geometryFrame = $Evidence.desktopFrames |
            Where-Object {
                [long]$_.acquireEntryCounter -ge [long]$event.geometryIssueCounter -and
                [int]$_.window.width -eq [int]$event.afterWindow.width
            } |
            Sort-Object acquireEntryCounter |
            Select-Object -First 1
        # SendMessageTimeout and SetWindowPos are synchronous. The visible change can occur at
        # any point between each call's issue and completion counters, so use that measured
        # interval instead of pretending the nominal DwmFlush count is the complete phase.
        $expectedMinimum = ([long]$event.geometryIssueCounter - [long]$event.contentCompleteCounter) / $refreshTicks
        $expectedMaximum = ([long]$event.geometryCompleteCounter - [long]$event.contentIssueCounter) / $refreshTicks
        $measured = if ($null -ne $contentFrame -and $null -ne $geometryFrame) {
            ([long]$geometryFrame.acquireEntryCounter - [long]$contentFrame.acquireEntryCounter) / $refreshTicks
        } else { $null }
        $directionCorrect = $null -ne $measured -and (
            [int]$event.offsetRefreshes -eq 0 -or
            ($event.direction -eq 'content-before-geometry' -and $expectedMinimum -gt 0 -and $measured -gt 0) -or
            ($event.direction -eq 'geometry-before-content' -and $expectedMaximum -lt 0 -and $measured -lt 0))
        $magnitudeError = if ($null -eq $measured) {
            $null
        } elseif ($measured -lt $expectedMinimum) {
            $expectedMinimum - $measured
        } elseif ($measured -gt $expectedMaximum) {
            $measured - $expectedMaximum
        } else {
            0.0
        }
        $phaseResults.Add([ordered]@{
            direction = $event.direction
            offsetRefreshes = [int]$event.offsetRefreshes
            expectedGeometryMinusContentRefreshes = [ordered]@{
                minimum = $expectedMinimum
                maximum = $expectedMaximum
            }
            measuredGeometryMinusContentRefreshes = $measured
            magnitudeErrorRefreshes = $magnitudeError
            directionCorrect = $directionCorrect
            pass = $directionCorrect -and $null -ne $magnitudeError -and $magnitudeError -le 1.0
        })
    }

    $contentStage = @($Evidence.qualificationStages | Where-Object name -eq 'content-only')[0]
    $contentWgc = @($Evidence.frames | Where-Object {
        [long]$_.callbackEntryCounter -ge [long]$contentStage.startCounter -and
        [long]$_.callbackEntryCounter -le [long]$contentStage.endCounter
    })
    $contentDesktop = @($Evidence.desktopFrames | Where-Object {
        [long]$_.acquireEntryCounter -ge [long]$contentStage.startCounter -and
        [long]$_.acquireEntryCounter -le [long]$contentStage.endCounter
    })
    $wgcSystemIntervals = [Collections.Generic.List[double]]::new()
    $orderedWgc = @($contentWgc | Sort-Object systemRelative100ns)
    for ($index = 1; $index -lt $orderedWgc.Count; $index++) {
        $delta = ([long]$orderedWgc[$index].systemRelative100ns - [long]$orderedWgc[$index - 1].systemRelative100ns) / 10.0
        if ($delta -gt 0) { $wgcSystemIntervals.Add($delta) }
    }
    $desktopIntervals = Get-Intervals $contentDesktop 'lastPresentCounter'
    $desktopIntervalP95Microseconds = if ($desktopIntervals.Count -gt 0) {
        (Get-Percentile $desktopIntervals 0.95) * 1000000.0 / $frequency
    } else { $null }
    $wgcIntervalP95Microseconds = Get-Percentile $wgcSystemIntervals.ToArray() 0.95
    $wgcStrictEligible = $null -ne $wgcIntervalP95Microseconds -and
        $wgcIntervalP95Microseconds -le (2 * $refreshMicroseconds)
    $desktopStrictEligible = $null -ne $desktopIntervalP95Microseconds -and
        $desktopIntervalP95Microseconds -le (2 * $refreshMicroseconds)
    $sourceCadenceMinimum = $refreshHz * 0.90
    $sourceCadenceMaximum = $refreshHz * 1.10
    $sourceCadence = [double]$AppEvidence.qualificationPresentedFramesPerSecond
    $sourceCadencePass = $AppEvidence.qualificationRenderBackend -like 'D3D12*' -and
        $sourceCadence -ge $sourceCadenceMinimum -and $sourceCadence -le $sourceCadenceMaximum
    $integrityPass = [int]$Evidence.captureErrors -eq 0 -and
        [int]$Evidence.captureRingDroppedFrames -eq 0 -and
        [int]$Evidence.encoderDroppedFrames -eq 0 -and
        [int]$Evidence.framePoolRecreateCount -eq 0 -and
        [int]$Evidence.poolCapacityExceededFrames -eq 0 -and
        $Evidence.desktopDuplication.status -eq 'captured' -and
        [int]$Evidence.desktopDuplication.errors -eq 0
    $callbackPass = [double]$Evidence.callbackDurationMicroseconds.p95 -le 1000 -and
        [double]$Evidence.callbackDurationMicroseconds.p99 -le 2000
    $phasePass = $phaseResults.Count -eq 8 -and @($phaseResults | Where-Object { -not $_.pass }).Count -eq 0
    $status = if ($sourceCadencePass -and $integrityPass -and $callbackPass -and $desktopStrictEligible -and
        $staticFailures.Count -eq 0 -and $phasePass) { 'PASS' } else { 'FAIL' }
    return [ordered]@{
        status = $status
        sourceCadencePass = $sourceCadencePass
        sourceBackend = $AppEvidence.qualificationRenderBackend
        sourcePresentedFramesPerSecond = $sourceCadence
        sourceCadenceMinimumFramesPerSecond = $sourceCadenceMinimum
        sourceCadenceMaximumFramesPerSecond = $sourceCadenceMaximum
        integrityPass = $integrityPass
        callbackPass = $callbackPass
        callbackDurationMicroseconds = $Evidence.callbackDurationMicroseconds
        captureRingDroppedFrames = [int]$Evidence.captureRingDroppedFrames
        framePoolRecreateCount = [int]$Evidence.framePoolRecreateCount
        staticWgcFrames = $wgcStatic.Count
        staticDesktopFrames = $desktopStatic.Count
        staticFalseFailures = $staticFailures.Count
        wgcStrictJudge = if ($wgcStrictEligible) { 'qualified' } else { 'diagnosticOnly' }
        wgcIntervalP95Microseconds = $wgcIntervalP95Microseconds
        desktopStrictJudge = if ($desktopStrictEligible) { 'qualified' } else { 'FAIL' }
        desktopIntervalP95Microseconds = $desktopIntervalP95Microseconds
        phasePass = $phasePass
        phase = $phaseResults
    }
}

$previousDpiContext = [DorotiObserverNative]::SetThreadDpiAwarenessContext([DorotiObserverNative]::PMV2)
if ($previousDpiContext -eq [IntPtr]::Zero) { throw 'Unable to set PMv2 DPI awareness for the observer driver.' }
$results = [Collections.Generic.List[object]]::new()
$sourceFingerprint = Get-SourceFingerprint
$gitCommit = (& git -C $repoRoot rev-parse HEAD).Trim()
try {
    for ($runNumber = 1; $runNumber -le $Runs; $runNumber++) {
        $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
        $runId = "win-observer-m1-q$runNumber-$stamp-$([Guid]::NewGuid().ToString('N').Substring(0, 8))"
        $rawEvidence = Join-Path $evidenceRoot "$runId.observer.json"
        $appEvidence = Join-Path $evidenceRoot "$runId.app.json"
        $appStart = [Diagnostics.ProcessStartInfo]::new($app)
        $appStart.UseShellExecute = $false
        foreach ($argument in @('--arm', 'A', '--qualification', '--refresh-hz', '165', '--evidence', $appEvidence)) {
            [void]$appStart.ArgumentList.Add($argument)
        }
        $appProcess = [Diagnostics.Process]::Start($appStart)
        if ($null -eq $appProcess) { throw 'Unable to start the top-level qualification control.' }
        $hwnd = [IntPtr]::Zero
        try {
            $hwnd = Wait-ForWindow $appProcess ([TimeSpan]::FromSeconds(20))
            $dpi = [DorotiObserverNative]::GetDpiForWindow($hwnd)
            $scale = $dpi / 96.0
            $workArea = [DorotiObserverNative+RECT]::new()
            if (-not [DorotiObserverNative]::SystemParametersInfo(
                [DorotiObserverNative]::SPI_GETWORKAREA, 0, [ref]$workArea, 0)) {
                throw 'Unable to query the active work area.'
            }
            $physicalWidth = [Math]::Round($LogicalWidth * $scale)
            $physicalHeight = [Math]::Round($LogicalHeight * $scale)
            if (-not [DorotiObserverNative]::SetWindowPos(
                $hwnd, [IntPtr]::Zero, $workArea.Left + 40, $workArea.Top + 40,
                $physicalWidth, $physicalHeight, [DorotiObserverNative]::SWP_SHOWWINDOW)) {
                throw 'Unable to set the exact qualification outer-window size.'
            }
            Start-Sleep -Seconds 1
            $captureStart = [Diagnostics.ProcessStartInfo]::new($capture)
            $captureStart.UseShellExecute = $false
            $captureStart.CreateNoWindow = $true
            $captureStart.RedirectStandardOutput = $true
            $captureStart.RedirectStandardError = $true
            foreach ($argument in @(
                '--hwnd', $hwnd.ToInt64().ToString([Globalization.CultureInfo]::InvariantCulture),
                '--output', $rawEvidence,
                '--run-id', $runId,
                '--qualification',
                '--duration', '10',
                '--input-hz', '0',
                '--png-stride', '1000',
                '--oracle-stride', '10',
                '--requested-logical-width', $LogicalWidth.ToString([Globalization.CultureInfo]::InvariantCulture),
                '--requested-logical-height', $LogicalHeight.ToString([Globalization.CultureInfo]::InvariantCulture),
                '--no-anomaly-png')) {
                [void]$captureStart.ArgumentList.Add($argument)
            }
            $captureProcess = [Diagnostics.Process]::Start($captureStart)
            if ($null -eq $captureProcess) { throw 'Unable to start the observer qualification capture.' }
            $stdout = $captureProcess.StandardOutput.ReadToEndAsync()
            $stderr = $captureProcess.StandardError.ReadToEndAsync()
            if (-not $captureProcess.WaitForExit([int][TimeSpan]::FromMinutes(20).TotalMilliseconds)) {
                $captureProcess.Kill($true)
                throw 'Observer qualification exceeded the 20-minute test timeout.'
            }
            $captureOutput = $stdout.GetAwaiter().GetResult()
            $captureError = $stderr.GetAwaiter().GetResult()
            if ($captureProcess.ExitCode -ne 0) {
                throw "Observer qualification capture failed (exit $($captureProcess.ExitCode)).`n$captureOutput`n$captureError"
            }
            $evidence = Get-Content -LiteralPath $rawEvidence -Raw | ConvertFrom-Json -Depth 60
            if (-not (Test-Path -LiteralPath $appEvidence -PathType Leaf)) {
                throw "Qualification app did not publish live D3D12 source evidence: $appEvidence"
            }
            $appReport = Get-Content -LiteralPath $appEvidence -Raw | ConvertFrom-Json -Depth 20
            $measurement = Measure-Qualification $evidence $appReport
            $results.Add([ordered]@{
                runNumber = $runNumber
                runId = $runId
                status = $measurement.status
                sourceFingerprint = $sourceFingerprint
                appSha256 = (Get-FileHash -LiteralPath $app -Algorithm SHA256).Hash.ToLowerInvariant()
                observerSha256 = (Get-FileHash -LiteralPath $capture -Algorithm SHA256).Hash.ToLowerInvariant()
                displayRefreshHz = $evidence.displayRefreshHz
                dpi = $evidence.windowDpi
                requestedLogicalOuter = $evidence.requestedLogicalOuter
                initialGeometry = $evidence.initialGeometry
                measurement = $measurement
                appMeasurement = $appReport
                rawEvidence = [IO.Path]::GetRelativePath($repoRoot, $rawEvidence).Replace('\', '/')
                appEvidence = [IO.Path]::GetRelativePath($repoRoot, $appEvidence).Replace('\', '/')
            })
            Write-Output "RUN=$runNumber STATUS=$($measurement.status) SOURCE_FPS=$($measurement.sourcePresentedFramesPerSecond) CALLBACK_P95_US=$($measurement.callbackDurationMicroseconds.p95) DESKTOP_P95_US=$($measurement.desktopIntervalP95Microseconds) PHASE=$($measurement.phasePass)"
        } finally {
            if ($hwnd -ne [IntPtr]::Zero) {
                [DorotiObserverNative]::PostMessage($hwnd, [DorotiObserverNative]::WM_CANCELMODE, [IntPtr]::Zero, [IntPtr]::Zero) | Out-Null
                [DorotiObserverNative]::PostMessage($hwnd, [DorotiObserverNative]::WM_CLOSE, [IntPtr]::Zero, [IntPtr]::Zero) | Out-Null
            }
            if (-not $appProcess.HasExited -and -not $appProcess.WaitForExit(5000)) {
                $appProcess.Kill($true)
                $appProcess.WaitForExit()
            }
        }
    }
} finally {
    [DorotiObserverNative]::SetThreadDpiAwarenessContext($previousDpiContext) | Out-Null
}

$consistent = @($results.status | Sort-Object -Unique).Count -eq 1
$m1Status = if ($results.Count -eq $Runs -and $consistent -and @($results | Where-Object status -ne 'PASS').Count -eq 0) {
    'PASS'
} else {
    'FAIL'
}
$summaryPath = Join-Path $evidenceRoot "win-observer-m1-summary-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
$summary = [ordered]@{
    schemaVersion = 'doroti.windows-presentation-observer-qualification/v1'
    status = $m1Status
    qualificationRunsRequired = $Runs
    consistentVerdict = $consistent
    sourceCommit = $gitCommit
    dirtySourceFingerprint = $sourceFingerprint
    generatedAt = [DateTimeOffset]::Now.ToString('o')
    runs = $results
}
$summary | ConvertTo-Json -Depth 60 | Set-Content -LiteralPath $summaryPath -Encoding utf8
Write-Output "SUMMARY=$summaryPath"
if ($m1Status -ne 'PASS') { exit 2 }
