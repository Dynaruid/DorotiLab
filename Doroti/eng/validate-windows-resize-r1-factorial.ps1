[CmdletBinding()]
param(
    [ValidateRange(1,10)] [int] $Iterations = 10,
    [switch] $SkipBuild,
    [string] $ArtifactRoot
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if (-not $ArtifactRoot) {
    $ArtifactRoot = Join-Path $repo ('Doroti\artifacts\windows-resize-r1-factorial\r1-' +
        (Get-Date -Format 'yyyyMMdd-HHmmss') + '-' + [guid]::NewGuid().ToString('N'))
}
$artifactRootPath = [IO.Path]::GetFullPath($ArtifactRoot)
New-Item -ItemType Directory -Force -Path $artifactRootPath | Out-Null
$app = Join-Path $repo 'DorotiDemoApp\windowsappsdk\bin\Release\net10.0-windows10.0.19041.0\win-x64\DorotiDemoApp.WindowsAppSdk.exe'
$observer = Join-Path $repo '.doroti\build\windows-resize-capture-f6r-vs\Release\Doroti.WindowsResizeCapture.exe'

if (-not $SkipBuild) {
    Push-Location $repo
    try {
        & pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 build -App .\DorotiDemoApp -Platform windows
        if ($LASTEXITCODE -ne 0) { throw "R1 Windows Release build failed with exit code $LASTEXITCODE." }
        & cmake --build .\.doroti\build\windows-resize-capture-f6r-vs --config Release
        if ($LASTEXITCODE -ne 0) { throw "R1 observer build failed with exit code $LASTEXITCODE." }
    } finally { Pop-Location }
}
foreach ($path in @($app,$observer)) {
    if (-not (Test-Path -LiteralPath $path)) { throw "R1 binary is missing: $path" }
}

function Get-Percentile([double[]] $Values, [double] $P) {
    if (-not $Values -or $Values.Count -eq 0) { return $null }
    $sorted = @($Values | Sort-Object)
    $index = [Math]::Min($sorted.Count - 1, [Math]::Max(0, [Math]::Ceiling($sorted.Count * $P) - 1))
    return [double]$sorted[$index]
}

function Get-Axis($Rect, [string] $Edge) {
    switch ($Edge) {
        'Left' { [double]$Rect.left }
        'Right' { [double]$Rect.right }
        'Top' { [double]$Rect.top }
        'Bottom' { [double]$Rect.bottom }
    }
}

function Measure-Run($Evidence, [int] $Variant, [string] $Mode, [int] $Iteration) {
    $capture = Get-Content -Raw -LiteralPath $Evidence | ConvertFrom-Json
    $frequency = [double]$capture.clockCalibration.qpcFrequency
    $start = [long]$capture.dragTiming.dragStartCounter
    $end = [long]$capture.dragTiming.mouseUpCounter
    $samples = @($capture.windowSamples | Where-Object {
        [long]$_.performanceCounter -ge $start -and [long]$_.performanceCounter -le $end
    } | Sort-Object performanceCounter)
    $edge = [string]$capture.edge
    $cursor0 = if ($edge -match 'Left|Right') { [double]$samples[0].cursorX } else { [double]$samples[0].cursorY }
    $edge0 = Get-Axis $samples[0].window $edge
    $lags = [Collections.Generic.List[double]]::new()
    $intervals = [Collections.Generic.List[double]]::new()
    $reverse = 0
    $sign = if ($edge -match 'Left|Top') { -1 } else { 1 }
    $previousEdge = $edge0
    $lastChangedEdge = $edge0
    $lastChangedCounter = [long]$samples[0].performanceCounter
    foreach ($sample in $samples) {
        $cursor = if ($edge -match 'Left|Right') { [double]$sample.cursorX } else { [double]$sample.cursorY }
        $nativeEdge = Get-Axis $sample.window $edge
        $lags.Add([Math]::Abs(($cursor - $cursor0) - ($nativeEdge - $edge0)))
        if ($nativeEdge -ne $previousEdge -and ($nativeEdge - $previousEdge) * $sign -lt 0) { $reverse++ }
        if ($nativeEdge -ne $lastChangedEdge) {
            $counter = [long]$sample.performanceCounter
            $intervals.Add(($counter - $lastChangedCounter) * 1000000.0 / $frequency)
            $lastChangedCounter = $counter
            $lastChangedEdge = $nativeEdge
        }
        $previousEdge = $nativeEdge
    }
    $initial = $capture.initialGeometry.outer
    $final = $capture.finalGeometry.outer
    $distance = [Math]::Abs((Get-Axis $final $edge) - (Get-Axis $initial $edge))
    $oppositeDrift = switch ($edge) {
        'Left' { [Math]::Abs([double]$final.right - [double]$initial.right) }
        'Right' { [Math]::Abs([double]$final.left - [double]$initial.left) }
        'Top' { [Math]::Abs([double]$final.bottom - [double]$initial.bottom) }
        'Bottom' { [Math]::Abs([double]$final.top - [double]$initial.top) }
    }
    [pscustomobject]@{
        variant = $Variant; mode = $Mode; edge = $edge; iteration = $Iteration
        durationMicroseconds = [long]$capture.dragTiming.actualDurationMicroseconds
        lagP50 = Get-Percentile $lags.ToArray() .50
        lagP95 = Get-Percentile $lags.ToArray() .95
        lagP99 = Get-Percentile $lags.ToArray() .99
        lagMax = if ($lags.Count) { ($lags | Measure-Object -Maximum).Maximum } else { $null }
        nativeIntervalP50 = Get-Percentile $intervals.ToArray() .50
        nativeIntervalP95 = Get-Percentile $intervals.ToArray() .95
        nativeIntervalMax = if ($intervals.Count) { ($intervals | Measure-Object -Maximum).Maximum } else { $null }
        reverseCount = $reverse
        requiredDistance = $distance
        oppositeEdgeDrift = $oppositeDrift
        finalLag = [Math]::Abs(600 - $distance)
        evidence = $Evidence
    }
}

$variants = [ordered]@{
    1 = 'standard-child-no-visible-surface'
    2 = 'dcomp-idle-exact-front'
    3 = 'dcomp-bounded-reservoir-only'
    4 = 'framework-exact-no-transient'
    5 = 'full-product-dcomp'
}
$edges = @('Left','Top','Right','Bottom')
$results = [Collections.Generic.List[object]]::new()
$groups = [Collections.Generic.List[object]]::new()

foreach ($variantEntry in $variants.GetEnumerator()) {
    $variant = [int]$variantEntry.Key
    $modeOrder = if ($variant % 2) { @('trace-off','trace-on') } else { @('trace-on','trace-off') }
    foreach ($mode in $modeOrder) {
        $groupDirectory = Join-Path $artifactRootPath ("v$variant-$mode")
        New-Item -ItemType Directory -Force -Path $groupDirectory | Out-Null
        $trace = Join-Path $groupDirectory 'causal.jsonl'
        $hostOut = Join-Path $groupDirectory 'host.stdout.log'
        $hostLog = Join-Path $groupDirectory 'host.stderr.log'
        $priorTrace = $env:DOROTI_WINDOWS_RESIZE_TRACE
        $priorRunId = $env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID
        $priorAdapter = $env:DOROTI_WINDOWS_ADAPTER
        $priorVariant = $env:DOROTI_WINDOWS_RESIZE_R1_VARIANT
        $priorDiagnostics = $env:DOROTI_WINDOWS_APPSDK_DIAGNOSTICS
        $env:DOROTI_WINDOWS_RESIZE_TRACE = if ($mode -eq 'trace-on') { $trace } else { $null }
        $env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID = "r1-v$variant-$mode"
        $env:DOROTI_WINDOWS_ADAPTER = 'FlutterEmbedder'
        $env:DOROTI_WINDOWS_RESIZE_R1_VARIANT = $variant.ToString()
        $env:DOROTI_WINDOWS_APPSDK_DIAGNOSTICS = '1'
        try {
            $process = Start-Process -FilePath $app -WorkingDirectory (Split-Path $app) -PassThru `
                -RedirectStandardOutput $hostOut -RedirectStandardError $hostLog
        } finally {
            $env:DOROTI_WINDOWS_RESIZE_TRACE = $priorTrace
            $env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID = $priorRunId
            $env:DOROTI_WINDOWS_ADAPTER = $priorAdapter
            $env:DOROTI_WINDOWS_RESIZE_R1_VARIANT = $priorVariant
            $env:DOROTI_WINDOWS_APPSDK_DIAGNOSTICS = $priorDiagnostics
        }
        try {
            $deadline = [DateTime]::UtcNow.AddSeconds(30)
            $hwnd = [IntPtr]::Zero
            while ([DateTime]::UtcNow -lt $deadline -and -not $process.HasExited) {
                $process.Refresh(); $hwnd = $process.MainWindowHandle
                if ($hwnd -ne [IntPtr]::Zero) { break }
                Start-Sleep -Milliseconds 100
            }
            if ($hwnd -eq [IntPtr]::Zero) { throw "R1 variant $variant did not expose a top-level HWND." }
            foreach ($edge in $edges) {
                for ($iteration = 1; $iteration -le $Iterations; $iteration++) {
                    $runId = "v$variant-$mode-$($edge.ToLowerInvariant())-$iteration"
                    $runDirectory = Join-Path $groupDirectory $runId
                    New-Item -ItemType Directory -Force -Path $runDirectory | Out-Null
                    $evidence = Join-Path $runDirectory 'capture-evidence.json'
                    $observerOut = Join-Path $runDirectory 'observer.stdout.log'
                    $observerLog = Join-Path $runDirectory 'observer.stderr.log'
                    $arguments = @('--hwnd',$hwnd.ToInt64().ToString(),'--output',$evidence,
                        '--visual-child-class','Doroti.Flutter.Windows.View','--run-id',$runId,
                        '--edge',$edge,'--input-hz','1000','--capture-only','--f6r','--log-only',
                        '--motion','expand','--drag-pixels','600','--drag-ms','150')
                    $observerProcess = Start-Process -FilePath $observer -ArgumentList $arguments -PassThru -NoNewWindow `
                        -RedirectStandardOutput $observerOut -RedirectStandardError $observerLog
                    if (-not $observerProcess.WaitForExit(20 * 60 * 1000)) {
                        Stop-Process -Id $observerProcess.Id -Force
                        throw "R1 observer $runId exceeded the required 20-minute timeout."
                    }
                    if ($observerProcess.ExitCode -ne 0) {
                        throw "R1 observer $runId failed: $(Get-Content -Raw $observerLog)"
                    }
                    $results.Add((Measure-Run $evidence $variant $mode $iteration))
                }
            }
            $process.Refresh()
            $cpu = $process.TotalProcessorTime.TotalMilliseconds
        } finally {
            if (-not $process.HasExited) {
                $null = $process.CloseMainWindow()
                if (-not $process.WaitForExit(10 * 1000)) { Stop-Process -Id $process.Id -Force }
            }
            $process.Dispose()
        }
        $traceEvents = if ($mode -eq 'trace-on' -and (Test-Path -LiteralPath $trace)) {
            @(Get-Content -LiteralPath $trace | Where-Object { $_.Trim() } | ForEach-Object { $_ | ConvertFrom-Json })
        } else { @() }
        $flushUs = @($traceEvents | Where-Object event -eq 'dwmFlushCompleted' | ForEach-Object {
            if ([string]$_.detail -match 'microseconds=(\d+)') { [double]$Matches[1] }
        })
        $groups.Add([pscustomobject]@{
            variant=$variant; name=$variantEntry.Value; mode=$mode; processCpuMilliseconds=$cpu
            traceEvents=$traceEvents.Count
            transientCommits=@($traceEvents | Where-Object event -eq 'transientFrontUpdated').Count
            exactPresents=@($traceEvents | Where-Object event -eq 'presented').Count
            dwmFlushCount=$flushUs.Count
            dwmFlushP95Microseconds=Get-Percentile $flushUs .95
            hostLog=$hostLog; trace=if($mode -eq 'trace-on'){$trace}else{$null}
        })
    }
}

$pairedOverhead = [Collections.Generic.List[object]]::new()
foreach ($off in ($results | Where-Object mode -eq 'trace-off')) {
    $on = $results | Where-Object {
        $_.variant -eq $off.variant -and $_.edge -eq $off.edge -and $_.iteration -eq $off.iteration -and $_.mode -eq 'trace-on'
    } | Select-Object -First 1
    $durationDelta = [Math]::Abs([double]$on.durationMicroseconds - $off.durationMicroseconds) / [Math]::Max(1,[double]$off.durationMicroseconds) * 100
    $lagDelta = [Math]::Max(0,[double]$on.lagP95 - $off.lagP95) / [Math]::Max(1,[double]$off.lagP95) * 100
    $pairedOverhead.Add([pscustomobject]@{variant=$off.variant;edge=$off.edge;iteration=$off.iteration
        durationDeltaPercent=$durationDelta;lagP95DeltaPercent=$lagDelta;pass=$durationDelta -le 10 -and $lagDelta -le 10})
}

$variantSummary = [Collections.Generic.List[object]]::new()
foreach ($variant in 1..5) {
    $primary = @($results | Where-Object { $_.variant -eq $variant -and $_.mode -eq 'trace-off' })
    $variantName = ($variants.GetEnumerator() | Where-Object { [int]$_.Key -eq $variant } | Select-Object -First 1).Value
    $variantSummary.Add([pscustomobject]@{
        variant=$variant; name=$variantName
        lagP95Median=Get-Percentile @($primary.lagP95) .50
        nativeIntervalP95Median=Get-Percentile @($primary.nativeIntervalP95) .50
        lagP95ByEdge=[ordered]@{}
        intervalP95ByEdge=[ordered]@{}
    })
    foreach($edge in $edges) {
        $edgeRuns=@($primary|Where-Object edge -eq $edge)
        $variantSummary[-1].lagP95ByEdge[$edge]=Get-Percentile @($edgeRuns.lagP95) .50
        $variantSummary[-1].intervalP95ByEdge[$edge]=Get-Percentile @($edgeRuns.nativeIntervalP95) .50
    }
}

$ownerCandidates = [Collections.Generic.List[object]]::new()
for ($variant = 2; $variant -le 5; $variant++) {
    $previous = $variantSummary | Where-Object variant -eq ($variant - 1)
    $current = $variantSummary | Where-Object variant -eq $variant
    $crossingEdges = @($edges | Where-Object {
        ([double]$current.lagP95ByEdge[$_] - [double]$previous.lagP95ByEdge[$_]) -ge 4 -or
        ([double]$current.intervalP95ByEdge[$_] - [double]$previous.intervalP95ByEdge[$_]) -ge 1000
    })
    $variantName = ($variants.GetEnumerator() | Where-Object { [int]$_.Key -eq $variant } | Select-Object -First 1).Value
    $ownerCandidates.Add([pscustomobject]@{variant=$variant;name=$variantName;crossingEdges=$crossingEdges
        repeated=$crossingEdges.Count -ge 2})
}
$selectedOwner = $ownerCandidates | Where-Object repeated | Select-Object -First 1
$overheadSummary = @($pairedOverhead | Group-Object variant,edge | ForEach-Object {
    [pscustomobject]@{
        key=$_.Name
        durationDeltaMedianPercent=Get-Percentile @($_.Group.durationDeltaPercent) .50
        lagP95DeltaMedianPercent=Get-Percentile @($_.Group.lagP95DeltaPercent) .50
        pass=(Get-Percentile @($_.Group.durationDeltaPercent) .50) -le 10 -and
             (Get-Percentile @($_.Group.lagP95DeltaPercent) .50) -le 10
    }
})
$contractFailures = @($results | Where-Object {
    # Match the observer's explicit drive qualification: Windows input may
    # quantize the final injected cursor point, but it must cover at least 80%
    # of the requested excursion and keep non-dragged edges within 8 pixels.
    $_.requiredDistance -lt 480 -or $_.oppositeEdgeDrift -gt 8 -or $_.reverseCount -ne 0
})
$checks = [ordered]@{
    allRunsCompletedOnce = $results.Count -eq 5 * 2 * 4 * $Iterations
    resizeGeometryContractPass = $contractFailures.Count -eq 0
    traceObserverOverheadAtMostTenPercent = @($overheadSummary | Where-Object { -not $_.pass }).Count -eq 0
    ownerIncreaseRepeated = $null -ne $selectedOwner
    selectedOwnerHasTraceEvidence = $null -ne $selectedOwner -and @($groups | Where-Object {
        $_.variant -eq $selectedOwner.variant -and $_.mode -eq 'trace-on' -and $_.traceEvents -gt 0
    }).Count -eq 1
}
$failed = @($checks.GetEnumerator() | Where-Object { -not $_.Value } | ForEach-Object Key)
$aggregate = [ordered]@{
    schemaVersion='doroti.windows.resize-r1-factorial/v1';generatedAtUtc=[DateTime]::UtcNow.ToString('O')
    status=if($failed.Count -eq 0){'PASS'}else{'FAIL'}
    sourceRevision=(& git -C $repo rev-parse HEAD).Trim();dirty=[bool](& git -C $repo status --porcelain)
    iterations=$Iterations;binarySha256=(Get-FileHash -Algorithm SHA256 -LiteralPath $app).Hash.ToLowerInvariant()
    selectedOwner=$selectedOwner;ownerCandidates=$ownerCandidates;variantSummary=$variantSummary
    groups=$groups;pairedObserverOverhead=$pairedOverhead;observerOverheadSummary=$overheadSummary;contractFailureCount=$contractFailures.Count
    checks=$checks;failedChecks=$failed;results=$results
}
$resultPath=Join-Path $artifactRootPath 'r1-validation.json'
$aggregate|ConvertTo-Json -Depth 30|Set-Content -LiteralPath $resultPath -Encoding utf8NoBOM
Write-Output "EVIDENCE=$resultPath"
Write-Output "STATUS=$($aggregate.status)"
if($selectedOwner){Write-Output "SELECTED_OWNER=variant-$($selectedOwner.variant):$($selectedOwner.name)"}
if($failed.Count){Write-Output "FAILED_CHECKS=$($failed -join ',')";exit 2}
