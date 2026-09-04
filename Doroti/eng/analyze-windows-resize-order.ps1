#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string] $TracePath,
    [Parameter(Mandatory)][string] $OutputPath,
    [switch] $WindowPosCommit,
    [switch] $AfterGeometry,
    [switch] $Receipt
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
if ($AfterGeometry -and -not $WindowPosCommit) { throw '-AfterGeometry requires -WindowPosCommit.' }
if ($Receipt -and -not $AfterGeometry) { throw '-Receipt requires -AfterGeometry.' }
$rows = @(Import-Csv -LiteralPath $TracePath)
if ($rows.Count -eq 0) { throw 'Empty resize-order trace.' }
$frequency = [double]$rows[0].frequency
if ($frequency -le 0) { throw 'Invalid QPC frequency.' }
$steps = @(foreach ($group in ($rows | Where-Object { [long]$_.epoch -gt 0 } | Group-Object epoch)) {
    $events = @($group.Group | Sort-Object { [long]$_.qpc })
    $entry = @($events | Where-Object stage -eq 'sizing-entry')
    $returned = @($events | Where-Object stage -eq 'sizing-return')
    if ($entry.Count -ne 1 -or $returned.Count -ne 1) { throw "Incomplete sizing epoch $($group.Name)." }
    $start = $entry[0]
    $end = $returned[0]
    $sizeReturned = @($events | Where-Object stage -eq 'size-return' | Select-Object -First 1)
    if ([int]$start.edge -notin @(1,3,4,5,7)) { continue }
    $presents = @($events | Where-Object {
        $_.stage -eq 'present-call' -and [long]$_.qpc -ge [long]$start.qpc -and
        $(if ($WindowPosCommit) {
            $sizeReturned.Count -eq 0 -or [long]$_.qpc -le [long]$sizeReturned[0].qpc
        } else { [long]$_.qpc -le [long]$end.qpc }) -and $_.generation -eq $end.generation
    })
    # Unchanged extents do not queue a raster and cannot test the handoff.
    if ($presents.Count -eq 0) { continue }
    $present = $presents[-1]
    $position = @($events | Where-Object {
        $_.stage -eq 'windowposchanging-entry' -and [long]$_.qpc -gt [long]$start.qpc -and
        (([int]$_.flags -band 3) -ne 3)
    } | Select-Object -First 1)
    $positionReturn = @($events | Where-Object stage -eq 'windowposchanging-return' | Select-Object -First 1)
    $changed = @($events | Where-Object stage -eq 'windowposchanged-entry' | Select-Object -First 1)
    $size = @($events | Where-Object stage -eq 'size-entry' | Select-Object -First 1)
    $copy = @($events | Where-Object {
        $_.stage -eq $(if ($WindowPosCommit) { 'prepare-copy-complete' } else { 'render-copy-complete-handoff' }) -and $_.generation -eq $end.generation -and
        [long]$_.qpc -le [long]$present.qpc
    } | Select-Object -Last 1)
    $terminal = @($events | Where-Object {
        $_.stage -eq 'worker-terminal-notify' -and $_.generation -eq $end.generation -and
        ($WindowPosCommit -or [long]$_.qpc -ge [long]$present.qpc)
    } | Select-Object -First 1)
    $rectMatches = $position.Count -eq 1
    if ($rectMatches) {
        foreach ($field in @('left','top','right','bottom')) {
            if ($start.$field -ne $position[0].$field) { $rectMatches = $false }
        }
    }
    $order = $position.Count -eq 1 -and $positionReturn.Count -eq 1 -and
        $changed.Count -eq 1 -and $size.Count -eq 1 -and $copy.Count -eq 1 -and
        $terminal.Count -eq 1 -and
        [long]$copy[0].qpc -le [long]$present.qpc -and
        [long]$terminal[0].qpc -le [long]$end.qpc -and
        [long]$end.qpc -le [long]$position[0].qpc -and
        [long]$position[0].qpc -le [long]$positionReturn[0].qpc -and
        [long]$positionReturn[0].qpc -le [long]$changed[0].qpc -and
        [long]$changed[0].qpc -le [long]$size[0].qpc
    if ($WindowPosCommit) {
        $order = $order -and $presents.Count -eq 1 -and
            [long]$copy[0].qpc -le [long]$terminal[0].qpc -and
            $(if ($AfterGeometry) {
                [long]$changed[0].qpc -lt [long]$present.qpc -and
                [long]$present.qpc -lt [long]$size[0].qpc
            } else {
                [long]$position[0].qpc -lt [long]$present.qpc -and
                [long]$present.qpc -lt [long]$positionReturn[0].qpc
            }) -and
            $present.thread -eq $position[0].thread -and $copy[0].thread -ne $present.thread
    }
    $presentReturn = @($events | Where-Object { $_.stage -eq 'present-return' -and $_.generation -eq $end.generation -and [long]$_.qpc -ge [long]$present.qpc } | Select-Object -First 1)
    if ($AfterGeometry) {
        $aligned = @($events | Where-Object { $_.stage -eq 'prepared-clock-ready' -and $_.generation -eq $end.generation })
        $order = $order -and $aligned.Count -eq 1 -and $presentReturn.Count -eq 1 -and
            [long]$position[0].qpc -lt [long]$aligned[0].qpc -and
            [long]$aligned[0].qpc -lt [long]$positionReturn[0].qpc -and
            [long]$presentReturn[0].qpc -lt [long]$size[0].qpc -and
            $aligned[0].thread -eq $present.thread
    }
    if ($Receipt) {
        $receiptEvents = @($events | Where-Object { $_.stage -eq 'present-receipt' -and $_.generation -eq $end.generation })
        $order = $order -and $receiptEvents.Count -eq 1 -and [int]$receiptEvents[0].flags -eq 1 -and
            [long]$presentReturn[0].qpc -le [long]$receiptEvents[0].qpc -and
            [long]$receiptEvents[0].qpc -lt [long]$size[0].qpc
    }
    [pscustomobject][ordered]@{
        epoch=[long]$group.Name; generation=[long]$end.generation
        rectMatches=$rectMatches; orderMatches=$order; presentsDuringSizing=$presents.Count
        proposedRect=@($start.left,$start.top,$start.right,$start.bottom)
        windowPosRect=if ($position.Count) { @($position[0].left,$position[0].top,$position[0].right,$position[0].bottom) } else { $null }
        presentToWindowPosMicroseconds=if ($position.Count) { ([long]$position[0].qpc - [long]$present.qpc) * 1000000.0 / $frequency } else { $null }
        presentToWindowPosReturnMicroseconds=if ($positionReturn.Count) { ([long]$positionReturn[0].qpc - [long]$present.qpc) * 1000000.0 / $frequency } else { $null }
        geometryToPresentMicroseconds=if ($changed.Count) { ([long]$present.qpc - [long]$changed[0].qpc) * 1000000.0 / $frequency } else { $null }
        geometryToPresentReturnMicroseconds=if ($changed.Count -and $presentReturn.Count) { ([long]$presentReturn[0].qpc - [long]$changed[0].qpc) * 1000000.0 / $frequency } else { $null }
        vblankBetweenPresentAndWindowPos=if ($position.Count -and [long]$position[0].timingStatus -eq 0) {
            [long]$position[0].vblank -gt [long]$present.qpc -and [long]$position[0].vblank -le [long]$position[0].qpc
        } else { $null }
        stages=@($events | ForEach-Object { "$($_.stage):$($_.qpc)" })
    }
})
$bad = @($steps | Where-Object { -not $_.rectMatches -or -not $_.orderMatches })
$dropped = ($rows | Measure-Object dropped -Maximum).Maximum
$report = [pscustomobject][ordered]@{
    status=if ($steps.Count -gt 0 -and $bad.Count -eq 0 -and $dropped -eq 0) { 'PASS-ordering-only' } else { 'FAIL-review-required' }
    gate=if ($AfterGeometry) { 'prepared-commit-after-geometry' } elseif ($WindowPosCommit) { 'J2' } else { 'J0' }
    tracePath=[IO.Path]::GetFullPath($TracePath)
    traceSha256=(Get-FileHash -LiteralPath $TracePath -Algorithm SHA256).Hash.ToLowerInvariant()
    measuredSteps=$steps.Count; failedSteps=$bad.Count; droppedEvents=$dropped
    observedVblankIntervals=@($steps | Where-Object vblankBetweenPresentAndWindowPos -eq $true).Count
    maximumPresentToWindowPosMicroseconds=($steps | Measure-Object presentToWindowPosMicroseconds -Maximum).Maximum
    maximumPresentToWindowPosReturnMicroseconds=($steps | Measure-Object presentToWindowPosReturnMicroseconds -Maximum).Maximum
    maximumGeometryToPresentMicroseconds=($steps | Measure-Object geometryToPresentMicroseconds -Maximum).Maximum
    maximumGeometryToPresentReturnMicroseconds=($steps | Measure-Object geometryToPresentReturnMicroseconds -Maximum).Maximum
    evidenceBoundary='Baseline mode observes v9 ordering. Prepared modes check one commit through matching WM_SIZE return; AfterGeometry checks clock alignment before geometry and submission after geometry, and Receipt additionally requires the matching observed CompositionFrame receipt before WM_SIZE. Later ordinary application frames are outside that geometry transaction. Copy-complete is recorded after synchronous copy, not at the exact GPU fence timestamp. Neither a clock signal nor a CompositionFrame receipt proves physical scan-out or jitter acceptance; those remain notVerified.'
    steps=$steps
}
$report | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $OutputPath -Encoding utf8
$report | Select-Object status,measuredSteps,failedSteps,droppedEvents,observedVblankIntervals,maximumPresentToWindowPosMicroseconds | Format-List
if ($report.status -ne 'PASS-ordering-only') { exit 1 }
