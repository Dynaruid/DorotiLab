#Requires -Version 7.0
param(
    [Parameter(Mandatory)]
    [string] $EtlPath,

    [Parameter(Mandatory)]
    [string] $SummaryPath,

    [string] $OutputPath
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$evidenceRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'validation/evidence/resize'))
$resolvedEtl = (Resolve-Path -LiteralPath $EtlPath).Path
$resolvedSummary = (Resolve-Path -LiteralPath $SummaryPath).Path
foreach ($path in @($resolvedEtl, $resolvedSummary)) {
    if (-not $path.StartsWith($evidenceRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Resize evidence must stay under ${evidenceRoot}: $path"
    }
}
if (-not $OutputPath) {
    $OutputPath = [IO.Path]::ChangeExtension($resolvedEtl, '.analysis.json')
}
$resolvedOutput = [IO.Path]::GetFullPath($OutputPath)
if (-not $resolvedOutput.StartsWith($evidenceRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Analysis output must stay under ${evidenceRoot}: $resolvedOutput"
}

$summary = Get-Content -LiteralPath $resolvedSummary -Raw | ConvertFrom-Json
$processLabel = "DorotiDemoApp.Windows.exe ($($summary.processId))"
$generationMinimum = [long] $summary.generationMinimum
$generationMaximum = [long] $summary.generationMaximum
$markerProvider = '{5a846f8d-54a1-4a4c-9e56-5b5a84e3b3c1}'
$dxgiProvider = '{ca11c036-0102-4a2d-a6ad-f03cfed5d3c9}'
$xperf = (Get-Command xperf.exe).Source

$markerHeader = @(
    'event', 'timestamp', 'process', 'threadId', 'cpu', 'activityId',
    'relatedActivityId', 'userSid', 'sessionId', 'phase', 'generation',
    'physicalWidth', 'physicalHeight', 'surfaceWidth', 'surfaceHeight',
    'managedThreadId', 'source')
$generationsByActivity = @{}

& $xperf -i $resolvedEtl -a dumper -provider $markerProvider 2>$null |
    Where-Object { $_.StartsWith('Doroti-Windows-Resize/ResizeMarker/', [StringComparison]::Ordinal) } |
    ForEach-Object {
        $event = $_ | ConvertFrom-Csv -Header $markerHeader
        if ($event.process.Trim() -ne $processLabel) { return }
        $generation = [long] $event.generation.Trim()
        if ($generation -lt $generationMinimum -or $generation -gt $generationMaximum) { return }
        $activityId = $event.activityId.Trim().ToLowerInvariant()
        if ([string]::IsNullOrWhiteSpace($activityId)) { return }
        if (-not $generationsByActivity.ContainsKey($activityId)) {
            $generationsByActivity[$activityId] = [ordered]@{
                generation = $generation
                target = $null
                preSwap = $null
                postSwap = $null
                presents = [Collections.Generic.List[object]]::new()
            }
        }
        $entry = $generationsByActivity[$activityId]
        switch ($event.phase.Trim()) {
            'target' { $entry.target = [long] $event.timestamp.Trim() }
            'pre-swap' { $entry.preSwap = [long] $event.timestamp.Trim() }
            'post-swap' { $entry.postSwap = [long] $event.timestamp.Trim() }
        }
    }
if ($LASTEXITCODE -ne 0) { throw "xperf marker dump failed with exit code $LASTEXITCODE." }

$presentHeader = @(
    'event', 'timestamp', 'process', 'threadId', 'cpu', 'activityId',
    'relatedActivityId', 'userSid', 'sessionId', 'swapChain', 'flags',
    'syncInterval', 'dirtyRects', 'scrollRects')
$allProcessPresents = [Collections.Generic.List[object]]::new()
& $xperf -i $resolvedEtl -a dumper -provider $dxgiProvider 2>$null |
    Where-Object { $_.StartsWith('Microsoft-Windows-DXGI/Present/win:Start,', [StringComparison]::Ordinal) } |
    ForEach-Object {
        $event = $_ | ConvertFrom-Csv -Header $presentHeader
        if ($event.process.Trim() -ne $processLabel) { return }
        $present = [pscustomobject]@{
            timestamp = [long] $event.timestamp.Trim()
            activityId = $event.activityId.Trim().ToLowerInvariant()
            swapChain = $event.swapChain.Trim().ToLowerInvariant()
            syncInterval = [int] $event.syncInterval.Trim()
        }
        $allProcessPresents.Add($present)
        if ($generationsByActivity.ContainsKey($present.activityId)) {
            $generationsByActivity[$present.activityId].presents.Add($present)
        }
    }
if ($LASTEXITCODE -ne 0) { throw "xperf DXGI dump failed with exit code $LASTEXITCODE." }

$classified = @($generationsByActivity.Values |
    Where-Object { $null -ne $_.target } |
    Sort-Object generation |
    ForEach-Object {
        $entry = $_
        $presentTimes = @($entry.presents | ForEach-Object timestamp)
        [pscustomobject]@{
            generation = $entry.generation
            leading = @($presentTimes | Where-Object { $_ -lt $entry.preSwap }).Count
            final = @($presentTimes | Where-Object {
                $_ -ge $entry.preSwap -and $_ -le $entry.postSwap
            }).Count
            afterPostSwap = @($presentTimes | Where-Object { $_ -gt $entry.postSwap }).Count
        }
    })

$primarySwapChain = $allProcessPresents |
    Group-Object swapChain |
    Sort-Object Count -Descending |
    Select-Object -First 1
$primaryPresents = @($allProcessPresents | Where-Object swapChain -eq $primarySwapChain.Name)
$traceStats = @(& $xperf -i $resolvedEtl -a tracestats 2>$null)
$lostBuffersLine = $traceStats | Where-Object { $_ -match '^Total # Lost Buffers' } | Select-Object -First 1
$lostEventsLine = $traceStats | Where-Object { $_ -match '^Total # Lost Events' } | Select-Object -First 1
$lostBuffers = if ($lostBuffersLine -match ':\s*(\d+)') { [long] $matches[1] } else { $null }
$lostEvents = if ($lostEventsLine -match ':\s*(\d+)') { [long] $matches[1] } else { $null }

$exactlyOneFinal = @($classified | Where-Object final -eq 1).Count
$status = if (
    $generationsByActivity.Count -eq [int] $summary.distinctTargetGenerations -and
    $classified.Count -eq [int] $summary.distinctTargetGenerations -and
    $exactlyOneFinal -eq $classified.Count -and
    @($classified | Where-Object afterPostSwap -ne 0).Count -eq 0 -and
    $lostBuffers -eq 0 -and $lostEvents -eq 0) { 'PASS' } else { 'failed' }

$analysis = [ordered]@{
    schemaVersion = 'doroti.resize-continuity-etw-analysis/v2'
    analyzedAt = [DateTimeOffset]::Now.ToString('o')
    status = $status
    etl = [ordered]@{
        path = [IO.Path]::GetRelativePath($repoRoot, $resolvedEtl).Replace('\', '/')
        bytes = (Get-Item -LiteralPath $resolvedEtl).Length
        sha256 = (Get-FileHash -LiteralPath $resolvedEtl -Algorithm SHA256).Hash
        lostBuffers = $lostBuffers
        lostEvents = $lostEvents
    }
    liveSummary = [IO.Path]::GetRelativePath($repoRoot, $resolvedSummary).Replace('\', '/')
    processId = [int] $summary.processId
    generationRange = [ordered]@{
        minimum = $generationMinimum
        maximum = $generationMaximum
        expected = [int] $summary.distinctTargetGenerations
        correlated = $classified.Count
    }
    dxgi = [ordered]@{
        allProcessPresentStarts = $allProcessPresents.Count
        primarySwapChain = $primarySwapChain.Name
        primaryPresentStarts = $primaryPresents.Count
        primarySyncInterval0 = @($primaryPresents | Where-Object syncInterval -eq 0).Count
        primarySyncInterval1 = @($primaryPresents | Where-Object syncInterval -eq 1).Count
        activityCorrelatedPresentStarts = ($classified.leading | Measure-Object -Sum).Sum +
            ($classified.final | Measure-Object -Sum).Sum +
            ($classified.afterPostSwap | Measure-Object -Sum).Sum
        leadingPresentStarts = ($classified.leading | Measure-Object -Sum).Sum
        finalPresentStarts = ($classified.final | Measure-Object -Sum).Sum
        generationsWithExactlyOneFinalPresent = $exactlyOneFinal
        generationsWithNoFinalPresent = @($classified | Where-Object final -eq 0).Count
        generationsWithMultipleFinalPresents = @($classified | Where-Object final -gt 1).Count
        presentsAfterPostSwap = ($classified.afterPostSwap | Measure-Object -Sum).Sum
        leadingPresentsPerGeneration = @($classified.leading |
            Group-Object | Sort-Object { [int] $_.Name } |
            ForEach-Object { [ordered]@{ count = [int] $_.Name; generations = $_.Count } })
    }
    conclusion = 'ResizeEpoch activity IDs classify DXGI Present starts before pre-swap as leading and exactly one Present between pre-swap and post-swap as final.'
}
[IO.File]::WriteAllText(
    $resolvedOutput,
    (($analysis | ConvertTo-Json -Depth 12) -replace "`r`n", "`n") + "`n",
    [Text.UTF8Encoding]::new($false))
Write-Output "ANALYSIS=$resolvedOutput"
Write-Output ($analysis | ConvertTo-Json -Depth 12 -Compress)
if ($status -ne 'PASS') { exit 1 }
