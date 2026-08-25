[CmdletBinding()]
param(
    [ValidateSet('Left','Right','Top','Bottom','TopLeft','TopRight','BottomLeft','BottomRight')]
    [string[]] $Edge = @('Left','Right','Top','Bottom'),
    [ValidateSet('expand','shrink','reverse')]
    [string[]] $Motion = @('expand'),
    [ValidateRange(1,3)] [int] $Iterations = 1,
    [ValidateRange(20,10000)] [int] $DragMilliseconds = 150,
    [ValidateRange(1,4000)] [int] $DragPixels = 600,
    [ValidateRange(30,1000)] [int] $InputHz = 1000,
    [switch] $CaptureOnly,
    [switch] $LogOnly,
    [switch] $GeometryOnly,
    [ValidateRange(0,1000)] [int] $RendererDelayMilliseconds = 0,
    [switch] $SkipBuild,
    [string] $ArtifactRoot
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if (-not $ArtifactRoot) {
    $ArtifactRoot = Join-Path $repo ('Doroti\artifacts\windowsappsdk-flutter-resize-trace\f6r-' +
        (Get-Date -Format 'yyyyMMdd-HHmmss') + '-' + [guid]::NewGuid().ToString('N'))
}
$artifactRootPath = [IO.Path]::GetFullPath($ArtifactRoot)
New-Item -ItemType Directory -Force -Path $artifactRootPath | Out-Null
$app = Join-Path $repo 'DorotiDemoApp\windowsappsdk\bin\Release\net10.0-windows10.0.19041.0\win-x64\DorotiDemoApp.WindowsAppSdk.exe'
$observerBuild = Join-Path $repo '.doroti\build\windows-resize-capture-f6r-vs'
$observer = Join-Path $observerBuild 'Release\Doroti.WindowsResizeCapture.exe'
$analyzer = Join-Path $PSScriptRoot 'analyze-windowsappsdk-flutter-resize-trace.ps1'

function Invoke-Checked([scriptblock] $Operation, [string] $Description) {
    & $Operation
    if ($LASTEXITCODE -ne 0) { throw "$Description failed with exit code $LASTEXITCODE." }
}

if (-not $SkipBuild) {
    Push-Location $repo
    try {
        Invoke-Checked { pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 build -App .\DorotiDemoApp -Platform windows } 'Windows Release build'
        Invoke-Checked { cmake -S .\Doroti\validation\windows-resize-capture -B $observerBuild -G 'Visual Studio 18 2026' -A x64 } 'F6-R observer configure'
        Invoke-Checked { cmake --build $observerBuild --config Release } 'F6-R observer build'
    } finally { Pop-Location }
}
if (-not (Test-Path -LiteralPath $app)) { throw "Product executable is missing: $app" }
if (-not (Test-Path -LiteralPath $observer)) { throw "F6-R observer is missing: $observer" }

$modes = if ($GeometryOnly -or $LogOnly) { @('log-only') } elseif ($CaptureOnly) { @('capture') } else { @('log-only','capture') }
$effectiveRendererDelay = if ($GeometryOnly -and $RendererDelayMilliseconds -eq 0) { 50 } else { $RendererDelayMilliseconds }
$results = [Collections.Generic.List[object]]::new()
$gitCommit = (& git -C $repo rev-parse HEAD).Trim()
$gitDirty = [bool](& git -C $repo status --porcelain)

foreach ($motionName in $Motion) {
    foreach ($edgeName in $Edge) {
        for ($iteration = 1; $iteration -le $Iterations; $iteration++) {
            foreach ($mode in $modes) {
                $runId = ('{0}-{1}-{2}-{3}' -f $edgeName.ToLowerInvariant(), $motionName, $iteration, $mode)
                $runDirectory = Join-Path $artifactRootPath $runId
                New-Item -ItemType Directory -Force -Path $runDirectory | Out-Null
                $trace = Join-Path $runDirectory 'causal.jsonl'
                $evidence = Join-Path $runDirectory 'capture-evidence.json'
                $hostOut = Join-Path $runDirectory 'host.stdout.log'
                $hostLog = Join-Path $runDirectory 'host.stderr.log'
                $observerOut = Join-Path $runDirectory 'observer.stdout.log'
                $observerLog = Join-Path $runDirectory 'observer.stderr.log'

                $priorTrace = $env:DOROTI_WINDOWS_RESIZE_TRACE
                $priorRunId = $env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID
                $priorAdapter = $env:DOROTI_WINDOWS_ADAPTER
                $priorRendererDelay = $env:DOROTI_WINDOWS_RESIZE_RENDERER_DELAY_MS
                $env:DOROTI_WINDOWS_RESIZE_TRACE = $trace
                $env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID = $runId
                $env:DOROTI_WINDOWS_ADAPTER = 'FlutterEmbedder'
                $env:DOROTI_WINDOWS_RESIZE_RENDERER_DELAY_MS = $effectiveRendererDelay.ToString()
                try {
                    $process = Start-Process -FilePath $app -WorkingDirectory (Split-Path $app) -PassThru `
                        -RedirectStandardOutput $hostOut -RedirectStandardError $hostLog
                } finally {
                    $env:DOROTI_WINDOWS_RESIZE_TRACE = $priorTrace
                    $env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID = $priorRunId
                    $env:DOROTI_WINDOWS_ADAPTER = $priorAdapter
                    $env:DOROTI_WINDOWS_RESIZE_RENDERER_DELAY_MS = $priorRendererDelay
                }

                try {
                    $deadline = [DateTime]::UtcNow.AddSeconds(30)
                    $hwnd = [IntPtr]::Zero
                    while ([DateTime]::UtcNow -lt $deadline -and -not $process.HasExited) {
                        $process.Refresh()
                        $hwnd = $process.MainWindowHandle
                        if ($hwnd -ne [IntPtr]::Zero) { break }
                        Start-Sleep -Milliseconds 100
                    }
                    if ($hwnd -eq [IntPtr]::Zero) { throw "The product did not expose a visible top-level HWND." }

                    $arguments = @(
                        '--hwnd', $hwnd.ToInt64().ToString(),
                        '--output', $evidence,
                        '--visual-child-class', 'Doroti.Flutter.Windows.View',
                        '--run-id', $runId,
                        '--edge', $edgeName,
                        '--input-hz', $InputHz.ToString(),
                        '--capture-only',
                        '--capture-ring-size', '64',
                        '--no-anomaly-png',
                        '--f6r',
                        '--motion', $motionName,
                        '--drag-pixels', $DragPixels.ToString(),
                        '--drag-ms', $DragMilliseconds.ToString()
                    )
                    if ($mode -eq 'log-only') { $arguments += '--log-only' }
                    $observerAttempts = 1
                    $observerProcess = Start-Process -FilePath $observer -ArgumentList $arguments -PassThru -NoNewWindow `
                        -RedirectStandardOutput $observerOut -RedirectStandardError $observerLog
                    if (-not $observerProcess.WaitForExit(20 * 60 * 1000)) {
                        Stop-Process -Id $observerProcess.Id -Force
                        throw "F6-R observer exceeded the required 20-minute timeout."
                    }
                    if ($observerProcess.ExitCode -ne 0) {
                        throw "F6-R observer failed with exit code $($observerProcess.ExitCode): $(Get-Content -Raw $observerLog)"
                    }
                } finally {
                    if (-not $process.HasExited) {
                        $null = $process.CloseMainWindow()
                        if (-not $process.WaitForExit(10 * 1000)) { Stop-Process -Id $process.Id -Force }
                    }
                    $process.Dispose()
                }

                $summaryPath = Join-Path $runDirectory 'f6r-summary.json'
                $analyzerArguments = @('-NoProfile','-File',$analyzer,'-Evidence',$evidence,'-CausalTrace',$trace,
                    '-HostLog',$hostLog,'-Output',$summaryPath)
                if ($GeometryOnly) { $analyzerArguments += '-GeometryOnly' }
                & pwsh @analyzerArguments |
                    Tee-Object -FilePath (Join-Path $runDirectory 'analysis.log')
                $analysisExit = $LASTEXITCODE
                $summary = Get-Content -Raw -LiteralPath $summaryPath | ConvertFrom-Json
                $results.Add([pscustomobject]@{
                    runId = $runId
                    edge = $edgeName
                    motion = $motionName
                    iteration = $iteration
                    mode = $mode
                    observerAttempts = $observerAttempts
                    status = $summary.status
                    analysisExitCode = $analysisExit
                    actualDragDurationMicroseconds = $summary.actualDragDurationMicroseconds
                    platformDispatchP99Microseconds = $summary.platformDispatchMicroseconds.p99
                    platformDispatchMaxMicroseconds = $summary.platformDispatchMicroseconds.max
                    presentP95Microseconds = $summary.presentIntervalMicroseconds.p95
                    presentMaxMicroseconds = $summary.presentIntervalMicroseconds.max
                    visibleFrontP95Microseconds = $summary.visibleFrontIntervalMicroseconds.p95
                    visibleFrontMaxMicroseconds = $summary.visibleFrontIntervalMicroseconds.max
                    finalExactPresentLatencyMicroseconds = $summary.finalExactPresentLatencyMicroseconds
                    cursorEdgeLagP95Pixels = $summary.cursorEdgeLagPixels.p95
                    cursorEdgeLagMaxPixels = $summary.cursorEdgeLagPixels.max
                    nativeEdgeUpdateP95Microseconds = $summary.nativeEdgeUpdateIntervalMicroseconds.p95
                    nativeEdgeUpdateMaxMicroseconds = $summary.nativeEdgeUpdateIntervalMicroseconds.max
                    captureFrames = $summary.captureFrames
                    observedCaptureHz = $summary.observedCaptureHz
                    failedChecks = @($summary.failedChecks)
                    directory = $runDirectory
                })
            }
        }
    }
}

$overhead = [Collections.Generic.List[object]]::new()
foreach ($group in ($results | Group-Object edge,motion,iteration)) {
    $log = $group.Group | Where-Object mode -eq 'log-only' | Select-Object -First 1
    $captured = $group.Group | Where-Object mode -eq 'capture' | Select-Object -First 1
    if (-not $log -or -not $captured) { continue }
    $durationChange = if ($log.actualDragDurationMicroseconds) {
        [Math]::Abs($captured.actualDragDurationMicroseconds - $log.actualDragDurationMicroseconds) /
            [double]$log.actualDragDurationMicroseconds * 100
    } else { $null }
    $latencyChange = if ($log.cursorEdgeLagP95Pixels) {
        [Math]::Max(0, $captured.cursorEdgeLagP95Pixels - $log.cursorEdgeLagP95Pixels) /
            [double]$log.cursorEdgeLagP95Pixels * 100
    } else { $null }
    $overhead.Add([pscustomobject]@{
        key = $group.Name
        durationChangePercent = $durationChange
        activeCursorEdgeLagP95ChangePercent = $latencyChange
        pass = $null -ne $durationChange -and $durationChange -le 10 -and
            $null -ne $latencyChange -and $latencyChange -le 10
    })
}

$overallPass = @($results | Where-Object status -ne 'PASS').Count -eq 0 -and
    @($overhead | Where-Object { -not $_.pass }).Count -eq 0
$aggregate = [ordered]@{
    schemaVersion = 'doroti.windowsappsdk.f6r-validation/v1'
    generatedAtUtc = [DateTime]::UtcNow.ToString('O')
    status = if ($overallPass) { 'PASS' } else { 'FAIL' }
    gitCommit = $gitCommit
    gitDirty = $gitDirty
    configuration = 'Release'
    rid = 'win-x64'
    adapter = 'FlutterEmbedder'
    geometryOnly = [bool]$GeometryOnly
    rendererDelayMilliseconds = $effectiveRendererDelay
    windowsVersion = [Environment]::OSVersion.VersionString
    observer = $observer
    product = $app
    observerSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $observer).Hash.ToLowerInvariant()
    productSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $app).Hash.ToLowerInvariant()
    results = $results
    pairedObserverOverhead = $overhead
}
$aggregatePath = Join-Path $artifactRootPath 'f6r-validation.json'
$aggregate | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $aggregatePath -Encoding utf8NoBOM
Write-Output "EVIDENCE=$aggregatePath"
Write-Output "STATUS=$($aggregate.status)"
if (-not $overallPass) { exit 2 }
