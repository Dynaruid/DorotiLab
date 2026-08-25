[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $Evidence,
    [Parameter(Mandatory)] [string] $CausalTrace,
    [string] $HostLog,
    [string] $Output
)

$ErrorActionPreference = 'Stop'
$evidencePath = (Resolve-Path -LiteralPath $Evidence).Path
$tracePath = (Resolve-Path -LiteralPath $CausalTrace).Path
if (-not $Output) { $Output = Join-Path (Split-Path $evidencePath) 'f6r-summary.json' }
$outputPath = [IO.Path]::GetFullPath($Output)
$bundle = Split-Path $evidencePath
$capture = Get-Content -Raw -LiteralPath $evidencePath | ConvertFrom-Json
$trace = @(Get-Content -LiteralPath $tracePath | Where-Object { $_.Trim() } | ForEach-Object { $_ | ConvertFrom-Json })

function Get-Percentile([double[]] $Values, [double] $P) {
    if (-not $Values -or $Values.Count -eq 0) { return $null }
    $sorted = @($Values | Sort-Object)
    $index = [Math]::Min($sorted.Count - 1, [Math]::Max(0, [Math]::Ceiling($sorted.Count * $P) - 1))
    return [double]$sorted[$index]
}

function Get-AxisValue($Value, [string] $Edge, [bool] $Cursor) {
    if ($Cursor) {
        if ($Edge -match 'Left|Right') { return [double]$Value.x }
        return [double]$Value.y
    }
    switch -Regex ($Edge) {
        'Left' { return [double]$Value.left }
        'Right' { return [double]$Value.right }
        'Top' { return [double]$Value.top }
        'Bottom' { return [double]$Value.bottom }
    }
}

$frequency = [double]$capture.clockCalibration.qpcFrequency
$dragStart = [long]$capture.dragTiming.dragStartCounter
$mouseUp = [long]$capture.dragTiming.mouseUpCounter
$actualDurationUs = [long]$capture.dragTiming.actualDurationMicroseconds
$refreshHz = [double]$capture.displayRefreshHz
$refreshUs = 1000000.0 / [Math]::Max(1.0, $refreshHz)
$desktopFrames = @($capture.desktopFrames | Sort-Object acquireEntryCounter)
$presentedTrace = @($trace | Where-Object event -eq 'presented' | Sort-Object qpc)
$inputTimeline = @($capture.windowSamples | Sort-Object performanceCounter)
$windowCaptureFrames = @($capture.frames | Sort-Object callbackEntryCounter | ForEach-Object {
    $frame = $_
    # WGC callbacks may be delivered after a newer window snapshot. Use the
    # frame's compositor timestamp and join it to the most recent accepted
    # present plus the nearest 1 kHz input sample instead of pairing pixels
    # with callback-time geometry.
    $frameQpc = [long][Math]::Round([double]$frame.systemRelative100ns * $frequency / 10000000.0)
    $present = $presentedTrace | Where-Object { [long]$_.qpc -le $frameQpc } | Select-Object -Last 1
    $sample = $inputTimeline | Sort-Object { [Math]::Abs([long]$_.performanceCounter - $frameQpc) } | Select-Object -First 1
    $joinedWindow = if ($present -and $present.topLevelRect) { $present.topLevelRect } else { $frame.window }
    $joinedClient = if ($present -and $present.childRect) { $present.childRect } else { $frame.client }
    $joinedCursor = if ($sample) {
        [pscustomobject]@{ x = [int]$sample.cursorX; y = [int]$sample.cursorY }
    } else { $frame.cursor }
    [pscustomobject]@{
        captureIndex = $frame.captureIndex
        acquireEntryCounter = $frameQpc
        accumulatedFrames = 1
        window = $joinedWindow
        client = $joinedClient
        cursor = $joinedCursor
        # The native observer compares app-bar and title-bar extents inside
        # the same raw compositor frame, so these gaps are already immune to
        # callback-time HWND geometry drift and must not be re-labeled here.
        contentLeftGap = [int]$frame.contentLeftGap
        contentRightGap = [int]$frame.contentRightGap
        png = $frame.png
    }
})
$frames = if ($desktopFrames.Count -gt 0) { $desktopFrames } else { $windowCaptureFrames }
$geometryFrames = if ($frames.Count -gt 0) { $frames } else {
    @($capture.windowSamples | ForEach-Object {
        [pscustomobject]@{
            acquireEntryCounter = [long]$_.performanceCounter
            cursor = [pscustomobject]@{ x = [int]$_.cursorX; y = [int]$_.cursorY }
            window = $_.window
        }
    })
}
$activeFrames = @($geometryFrames | Where-Object {
    [long]$_.acquireEntryCounter -ge $dragStart -and [long]$_.acquireEntryCounter -le $mouseUp
})

$lagValues = [Collections.Generic.List[double]]::new()
$reverseCount = 0
$stallCount = 0
$firstBadIndex = -1
$finalLag = $null
if ($activeFrames.Count -gt 0) {
    $first = $activeFrames[0]
    $cursor0 = Get-AxisValue $first.cursor $capture.edge $true
    $edge0 = Get-AxisValue $first.window $capture.edge $false
    $previousEdge = $edge0
    $previousCursor = $cursor0
    $expectedSign = if ($capture.edge -match 'Left|Top') { -1 } else { 1 }
    if ($capture.motion -eq 'shrink') { $expectedSign *= -1 }
    for ($index = 0; $index -lt $activeFrames.Count; $index++) {
        $frame = $activeFrames[$index]
        $cursor = Get-AxisValue $frame.cursor $capture.edge $true
        $edge = Get-AxisValue $frame.window $capture.edge $false
        $lag = [Math]::Abs(($cursor - $cursor0) - ($edge - $edge0))
        $lagValues.Add($lag)
        $edgeDelta = $edge - $previousEdge
        $cursorDelta = $cursor - $previousCursor
        $phaseSign = $expectedSign
        if ($capture.motion -eq 'reverse' -and $index -ge [Math]::Floor($activeFrames.Count / 2)) {
            $phaseSign *= -1
        }
        $reverse = [Math]::Abs($edgeDelta) -gt 0.1 -and ($edgeDelta * $phaseSign) -lt 0
        if ($reverse) { $reverseCount++ }
        # The three-frame oracle is defined over real monitor frames. The
        # 1 kHz log-only input samples intentionally outnumber display frames
        # and must not be misreported as visible stalls.
        if ($frames.Count -gt 0 -and $index -ge 2) {
            $old = $activeFrames[$index - 2]
            $oldEdge = Get-AxisValue $old.window $capture.edge $false
            $oldCursor = Get-AxisValue $old.cursor $capture.edge $true
            if ([Math]::Abs($edge - $oldEdge) -lt 0.5 -and [Math]::Abs($cursor - $oldCursor) -gt 4) {
                $stallCount++
            }
        }
        if ($firstBadIndex -lt 0 -and ($lag -gt 3 -or $reverse)) { $firstBadIndex = $index }
        $previousEdge = $edge
        $previousCursor = $cursor
    }
    $firstInput = $capture.windowSamples | Select-Object -First 1
    $lastInput = $capture.windowSamples | Select-Object -Last 1
    $initialCursor = if ($capture.edge -match 'Left|Right') { [double]$firstInput.cursorX } else { [double]$firstInput.cursorY }
    # Qualification may adjust the exact outer rect immediately before the
    # measured drag. Pair the first and last 1 kHz samples so the final oracle
    # uses the actual drag baseline rather than the earlier setup geometry.
    $initialEdge = Get-AxisValue $firstInput.window $capture.edge $false
    $finalCursor = if ($capture.edge -match 'Left|Right') { [double]$lastInput.cursorX } else { [double]$lastInput.cursorY }
    $finalEdge = Get-AxisValue $capture.finalGeometry.outer $capture.edge $false
    $finalLag = [Math]::Abs(($finalCursor - $initialCursor) - ($finalEdge - $initialEdge))
}

$presentEvents = @($trace | Where-Object {
    $_.event -eq 'presented' -and [long]$_.qpc -ge $dragStart -and [long]$_.qpc -le $mouseUp
} | Sort-Object qpc)
$presentIntervalsUs = [Collections.Generic.List[double]]::new()
for ($i = 1; $i -lt $presentEvents.Count; $i++) {
    $presentIntervalsUs.Add(([long]$presentEvents[$i].qpc - [long]$presentEvents[$i - 1].qpc) * 1000000.0 / $frequency)
}
$dispatchUs = @($trace | Where-Object event -eq 'windowSizeHandled' | ForEach-Object {
    if ($_.detail -match 'dispatchMicroseconds=(\d+)') { [double]$Matches[1] }
})
$traceStopped = $trace | Where-Object event -eq 'traceStopped' | Select-Object -Last 1
$traceDrops = if ($traceStopped) { [long]$traceStopped.droppedEvents } else { -1 }
$accumulatedDrops = [long](($frames | ForEach-Object { [Math]::Max(0, [int]$_.accumulatedFrames - 1) } | Measure-Object -Sum).Sum)
$activeCaptureFrames = @($frames | Where-Object {
    [long]$_.acquireEntryCounter -ge $dragStart -and [long]$_.acquireEntryCounter -le $mouseUp
})
$captureIntervalsUs = [Collections.Generic.List[double]]::new()
for ($i = 1; $i -lt $activeCaptureFrames.Count; $i++) {
    $captureIntervalsUs.Add(([long]$activeCaptureFrames[$i].acquireEntryCounter - [long]$activeCaptureFrames[$i - 1].acquireEntryCounter) * 1000000.0 / $frequency)
}
$observedCaptureHz = if ($activeCaptureFrames.Count -gt 1) {
    ($activeCaptureFrames.Count - 1) * $frequency /
        ([long]$activeCaptureFrames[-1].acquireEntryCounter - [long]$activeCaptureFrames[0].acquireEntryCounter)
} else { 0 }
$contentGapDeltas = [Collections.Generic.List[double]]::new()
if ($windowCaptureFrames.Count -gt 0) {
    $baselineGap = if ($capture.edge -match 'Left') {
        [double]$windowCaptureFrames[0].contentLeftGap
    } else { [double]$windowCaptureFrames[0].contentRightGap }
    foreach ($frame in $windowCaptureFrames) {
        $gap = if ($capture.edge -match 'Left') { [double]$frame.contentLeftGap } else { [double]$frame.contentRightGap }
        if ($gap -ge 0 -and $baselineGap -ge 0) { $contentGapDeltas.Add([Math]::Max(0, $gap - $baselineGap)) }
    }
}
$maxContentGapDelta = if ($contentGapDeltas.Count) { ($contentGapDeltas | Measure-Object -Maximum).Maximum } else { $null }

$hostSummary = @{}
$shutdownTrace = $trace | Where-Object event -eq 'shutdown' | Select-Object -Last 1
if ($shutdownTrace -and $shutdownTrace.detail) {
    foreach ($part in ([string]$shutdownTrace.detail).Split(';')) {
        $pair = $part.Split('=', 2)
        if ($pair.Count -eq 2) { $hostSummary[$pair[0]] = $pair[1] }
    }
}
if ($HostLog -and (Test-Path -LiteralPath $HostLog)) {
    $summaryLine = Get-Content -LiteralPath $HostLog | Where-Object { $_ -like 'doroti.windowsappsdk.summary=*' } | Select-Object -Last 1
    if ($summaryLine) {
        foreach ($part in $summaryLine.Substring($summaryLine.IndexOf('=') + 1).Split(';')) {
            $pair = $part.Split('=', 2)
            if ($pair.Count -eq 2) { $hostSummary[$pair[0]] = $pair[1] }
        }
    }
}

$captureRequired = [bool]$capture.f6r -and -not [bool]$capture.logOnly
$checks = [ordered]@{
    validDuration = $actualDurationUs -gt 0 -and $actualDurationUs -le 180000
    nativeEdgeNeverReverses = $reverseCount -eq 0
    noThreeFrameStall = $stallCount -eq 0
    finalCursorEdgeLagAtMostOnePixel = $null -ne $finalLag -and $finalLag -le 1
    presentP95AtMostTwoRefreshes = $presentIntervalsUs.Count -gt 0 -and
        (Get-Percentile $presentIntervalsUs.ToArray() .95) -le 2 * $refreshUs
    presentMaxAtMostThreeRefreshes = $presentIntervalsUs.Count -gt 0 -and
        (($presentIntervalsUs | Measure-Object -Maximum).Maximum -le 3 * $refreshUs)
    platformDispatchP99AtMostOneMillisecond = $dispatchUs.Count -gt 0 -and
        (Get-Percentile $dispatchUs .99) -le 1000
    platformDispatchMaxAtMostFourMilliseconds = $dispatchUs.Count -gt 0 -and
        (($dispatchUs | Measure-Object -Maximum).Maximum -le 4000)
    traceDroppedEventsZero = $traceDrops -eq 0
    queueMaxAtMostOne = $hostSummary.ContainsKey('queueMax') -and [int]$hostSummary.queueMax -le 1
    stalePresentZero = $hostSummary.ContainsKey('stalePresent') -and [int]$hostSummary.stalePresent -eq 0
    failedZero = $hostSummary.ContainsKey('failed') -and [int]$hostSummary.failed -eq 0
    terminalCausalMismatchZero = $hostSummary.ContainsKey('causalGap') -and
        $hostSummary.ContainsKey('receiptMismatch') -and
        [int]$hostSummary.causalGap -eq 0 -and [int]$hostSummary.receiptMismatch -eq 0
    captureFramesPresent = -not $captureRequired -or $frames.Count -gt 0
    captureDroppedFramesZero = -not $captureRequired -or $accumulatedDrops -eq 0
    captureCadenceMatchesDisplay = -not $captureRequired -or $observedCaptureHz -ge $refreshHz * .9
    rawFramesEncoded = -not $captureRequired -or
        ((($desktopFrames.Count -gt 0 -and
           [int]$capture.desktopDuplication.encodedRawFrames -eq $frames.Count -and
           [int]$capture.desktopDuplication.encoderDroppedFrames -eq 0) -or
          ($windowCaptureFrames.Count -gt 0 -and
           [int]$capture.encodedPngFrames -eq $frames.Count -and
           [int]$capture.encoderDroppedFrames -eq 0)))
    contentEdgeGapAtMostOnePixel = -not $captureRequired -or
        ($null -ne $maxContentGapDelta -and $maxContentGapDelta -le 1)
}

$failedChecks = @($checks.GetEnumerator() | Where-Object { -not $_.Value } | ForEach-Object Key)
$summary = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk.f6r-summary/v1'
    runId = $capture.runId
    status = if ($failedChecks.Count -eq 0) { 'PASS' } else { 'FAIL' }
    edge = $capture.edge
    motion = $capture.motion
    dragPixels = [int]$capture.dragPixels
    actualDragDurationMicroseconds = $actualDurationUs
    displayRefreshHz = $refreshHz
    inputSamples = [int]$capture.inputSamples
    captureFrames = $frames.Count
    observedCaptureHz = $observedCaptureHz
    accumulatedCaptureDrops = $accumulatedDrops
    maximumContentGapDeltaPixels = $maxContentGapDelta
    cursorEdgeLagPixels = [ordered]@{
        p50 = Get-Percentile $lagValues.ToArray() .50
        p95 = Get-Percentile $lagValues.ToArray() .95
        p99 = Get-Percentile $lagValues.ToArray() .99
        max = if ($lagValues.Count) { ($lagValues | Measure-Object -Maximum).Maximum } else { $null }
        final = $finalLag
    }
    reverseFrames = $reverseCount
    threeFrameStalls = $stallCount
    presentIntervalMicroseconds = [ordered]@{
        count = $presentIntervalsUs.Count
        p50 = Get-Percentile $presentIntervalsUs.ToArray() .50
        p95 = Get-Percentile $presentIntervalsUs.ToArray() .95
        p99 = Get-Percentile $presentIntervalsUs.ToArray() .99
        max = if ($presentIntervalsUs.Count) { ($presentIntervalsUs | Measure-Object -Maximum).Maximum } else { $null }
    }
    captureIntervalMicroseconds = [ordered]@{
        p50 = Get-Percentile $captureIntervalsUs.ToArray() .50
        p95 = Get-Percentile $captureIntervalsUs.ToArray() .95
        p99 = Get-Percentile $captureIntervalsUs.ToArray() .99
        max = if ($captureIntervalsUs.Count) { ($captureIntervalsUs | Measure-Object -Maximum).Maximum } else { $null }
    }
    platformDispatchMicroseconds = [ordered]@{
        p99 = Get-Percentile $dispatchUs .99
        max = if ($dispatchUs.Count) { ($dispatchUs | Measure-Object -Maximum).Maximum } else { $null }
    }
    traceEventCounts = [ordered]@{}
    hostSummary = $hostSummary
    firstBadActiveFrameIndex = $firstBadIndex
    checks = $checks
    failedChecks = $failedChecks
}
foreach ($group in ($trace | Group-Object event)) { $summary.traceEventCounts[$group.Name] = $group.Count }

New-Item -ItemType Directory -Force -Path (Split-Path $outputPath) | Out-Null
$summary | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $outputPath -Encoding utf8NoBOM

if ($frames.Count -gt 0) {
    $center = if ($firstBadIndex -ge 0) {
        [Array]::IndexOf($frames, $activeFrames[$firstBadIndex])
    } else { [Math]::Floor($frames.Count / 2) }
    $start = [Math]::Max(0, $center - 4)
    $end = [Math]::Min($frames.Count - 1, $center + 4)
    $monitor = $capture.monitorRect
    $cards = for ($i = $start; $i -le $end; $i++) {
        $frame = $frames[$i]
        if (-not $frame.png) { continue }
        $left = [double]$frame.cursor.x - [double]$monitor.left
        $top = [double]$frame.cursor.y - [double]$monitor.top
        $src = [Uri]::EscapeUriString(($frame.png -replace '\\','/'))
        "<figure><div class='shot'><img src='$src'><i style='left:${left}px;top:${top}px'></i></div><figcaption>#$($frame.captureIndex) qpc=$($frame.acquireEntryCounter)</figcaption></figure>"
    }
    $html = @"
<!doctype html><meta charset="utf-8"><title>F6-R contact sheet $($capture.runId)</title>
<style>body{background:#171717;color:#eee;font:13px system-ui}.grid{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:12px}figure{margin:0}.shot{position:relative;overflow:hidden}.shot img{display:block;width:100%;height:auto}.shot i{position:absolute;width:15px;height:15px;border:2px solid #ff1744;border-radius:50%;transform:translate(-50%,-50%);box-shadow:0 0 0 1px #fff}figcaption{overflow-wrap:anywhere}</style>
<h1>F6-R first-bad-frame contact sheet</h1><p>Raw monitor frames are unchanged. Red circles are derived cursor markers joined by QPC-adjacent capture records.</p><div class="grid">$($cards -join "`n")</div>
"@
    $html | Set-Content -LiteralPath (Join-Path $bundle 'first-bad-frame-contact-sheet.html') -Encoding utf8NoBOM
}

Write-Output "SUMMARY=$outputPath"
Write-Output "STATUS=$($summary.status)"
if ($failedChecks.Count) { Write-Output "FAILED_CHECKS=$($failedChecks -join ',')" }
if ($summary.status -ne 'PASS') { exit 2 }
