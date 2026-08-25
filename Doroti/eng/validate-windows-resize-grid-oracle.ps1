[CmdletBinding()]
param(
    [switch] $SkipBuild,
    [string] $ArtifactRoot
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if (-not $ArtifactRoot) {
    $ArtifactRoot = Join-Path $repo ('Doroti\artifacts\windows-resize-grid-oracle\r0-' +
        (Get-Date -Format 'yyyyMMdd-HHmmss') + '-' + [guid]::NewGuid().ToString('N'))
}
$artifactRootPath = [IO.Path]::GetFullPath($ArtifactRoot)
New-Item -ItemType Directory -Force -Path $artifactRootPath | Out-Null
$app = Join-Path $repo 'DorotiDemoApp\windowsappsdk\bin\Release\net10.0-windows10.0.19041.0\win-x64\DorotiDemoApp.WindowsAppSdk.exe'
$observerBuild = Join-Path $repo '.doroti\build\windows-resize-capture-f6r-vs'
$observer = Join-Path $observerBuild 'Release\Doroti.WindowsResizeCapture.exe'
$fixtures = Join-Path $observerBuild 'Release\Doroti.WindowsResizeGridOracleTests.exe'
$evidence = Join-Path $artifactRootPath 'capture-evidence.json'
$trace = Join-Path $artifactRootPath 'causal.jsonl'
$hostOut = Join-Path $artifactRootPath 'host.stdout.log'
$hostLog = Join-Path $artifactRootPath 'host.stderr.log'
$observerOut = Join-Path $artifactRootPath 'observer.stdout.log'
$observerLog = Join-Path $artifactRootPath 'observer.stderr.log'
$fixtureOut = Join-Path $artifactRootPath 'grid-fixtures.json'

function Invoke-Checked([scriptblock] $Operation, [string] $Description) {
    & $Operation
    if ($LASTEXITCODE -ne 0) { throw "$Description failed with exit code $LASTEXITCODE." }
}

if (-not $SkipBuild) {
    Push-Location $repo
    try {
        Invoke-Checked { pwsh -NoProfile -File .\Doroti\eng\doroti.ps1 build -App .\DorotiDemoApp -Platform windows } 'R0 Windows Release build'
        Invoke-Checked { cmake -S .\Doroti\validation\windows-resize-capture -B $observerBuild -G 'Visual Studio 18 2026' -A x64 } 'R0 observer configure'
        Invoke-Checked { cmake --build $observerBuild --config Release } 'R0 observer build'
    } finally { Pop-Location }
}
foreach ($path in @($app,$observer,$fixtures)) {
    if (-not (Test-Path -LiteralPath $path)) { throw "R0 binary is missing: $path" }
}

& $fixtures | Set-Content -LiteralPath $fixtureOut -Encoding utf8NoBOM
if ($LASTEXITCODE -ne 0) { throw "R0 synthetic grid fixtures failed with exit code $LASTEXITCODE." }
$fixture = Get-Content -Raw -LiteralPath $fixtureOut | ConvertFrom-Json

$priorTrace = $env:DOROTI_WINDOWS_RESIZE_TRACE
$priorRunId = $env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID
$priorAdapter = $env:DOROTI_WINDOWS_ADAPTER
$env:DOROTI_WINDOWS_RESIZE_TRACE = $trace
$env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID = 'r0-grid-live'
$env:DOROTI_WINDOWS_ADAPTER = 'FlutterEmbedder'
try {
    $process = Start-Process -FilePath $app -WorkingDirectory (Split-Path $app) -PassThru `
        -RedirectStandardOutput $hostOut -RedirectStandardError $hostLog
} finally {
    $env:DOROTI_WINDOWS_RESIZE_TRACE = $priorTrace
    $env:DOROTI_WINDOWS_RESIZE_TRACE_RUN_ID = $priorRunId
    $env:DOROTI_WINDOWS_ADAPTER = $priorAdapter
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
    if ($hwnd -eq [IntPtr]::Zero) { throw 'R0 product did not expose a visible top-level HWND.' }
    $arguments = @(
        '--hwnd', $hwnd.ToInt64().ToString(), '--output', $evidence,
        '--visual-child-class', 'Doroti.Flutter.Windows.View', '--run-id', 'r0-grid-live',
        '--edge', 'Right', '--input-hz', '1000', '--capture-only',
        '--capture-ring-size', '64', '--no-anomaly-png', '--f6r',
        '--motion', 'expand', '--drag-pixels', '600', '--drag-ms', '150'
    )
    $observerProcess = Start-Process -FilePath $observer -ArgumentList $arguments -PassThru -NoNewWindow `
        -RedirectStandardOutput $observerOut -RedirectStandardError $observerLog
    if (-not $observerProcess.WaitForExit(20 * 60 * 1000)) {
        Stop-Process -Id $observerProcess.Id -Force
        throw 'R0 observer exceeded the required 20-minute timeout.'
    }
    if ($observerProcess.ExitCode -ne 0) {
        throw "R0 observer failed with exit code $($observerProcess.ExitCode): $(Get-Content -Raw $observerLog)"
    }
} finally {
    if (-not $process.HasExited) {
        $null = $process.CloseMainWindow()
        if (-not $process.WaitForExit(10 * 1000)) { Stop-Process -Id $process.Id -Force }
    }
    $process.Dispose()
}

$capture = Get-Content -Raw -LiteralPath $evidence | ConvertFrom-Json
$frames = @($capture.frames)
$parsed = @($frames | Where-Object gridParsed)
$initial = $parsed | Select-Object -First 1
$final = $parsed | Select-Object -Last 1
$expected = [double]$capture.gridOracle.expectedPhysicalInterval
$safeFillDistinct = @($frames | Where-Object {
    $null -ne $_.gridRightTail -and [double]$_.gridRightTail -gt $expected -and
    [int]$_.detectedUncoveredLeftGap -ge 0 -and [int]$_.detectedUncoveredLeftGap -le 1 -and
    [int]$_.detectedUncoveredRightGap -ge 0 -and [int]$_.detectedUncoveredRightGap -le 1
}).Count -gt 0
$checks = [ordered]@{
    fixturePass = [string]$fixture.status -eq 'PASS' -and [int]$fixture.parseFailures -eq 0
    observerSchemaV3 = [string]$capture.schemaVersion -eq 'doroti.windows-presentation-observer/v3'
    gridSchemaV1 = [string]$capture.gridOracle.schemaVersion -eq 'doroti.windows.resize-grid/v1'
    liveFramesPresent = $frames.Count -gt 1
    liveParseFailuresZero = $parsed.Count -eq $frames.Count
    initialSpacingWithinOnePixel = $null -ne $initial -and
        [Math]::Abs([double]$initial.gridSpacingX - $expected) -le 1 -and
        [Math]::Abs([double]$initial.gridSpacingY - $expected) -le 1
    finalSpacingWithinOnePixel = $null -ne $final -and
        [Math]::Abs([double]$final.gridSpacingX - $expected) -le 1 -and
        [Math]::Abs([double]$final.gridSpacingY - $expected) -le 1
    initialFinalEdgeMarkersDetected = $null -ne $initial -and $null -ne $final -and
        [bool]$initial.gridRightEdgeMarkerDetected -and [bool]$initial.gridBottomEdgeMarkerDetected -and
        [bool]$final.gridRightEdgeMarkerDetected -and [bool]$final.gridBottomEdgeMarkerDetected
    initialFinalScaleUniform = $null -ne $initial -and $null -ne $final -and
        [Math]::Abs([double]$initial.gridNonUniformScaleRatio - 1) -le 0.02 -and
        [Math]::Abs([double]$final.gridNonUniformScaleRatio - 1) -le 0.02
    childLocalOriginStable = $null -ne $initial -and $null -ne $final -and
        [Math]::Abs([double]$initial.gridOriginOffsetX - [double]$final.gridOriginOffsetX) -le 1 -and
        [Math]::Abs([double]$initial.gridOriginOffsetY - [double]$final.gridOriginOffsetY) -le 1
    safeFillAndGridTailDiffer = $safeFillDistinct
    rawCaptureDropsZero = [int]$capture.captureRingDroppedFrames -eq 0 -and
        [int]$capture.encoderDroppedFrames -eq 0 -and [int]$capture.captureErrors -eq 0
    rawFramesEncoded = [int]$capture.encodedPngFrames -eq $frames.Count
}
$failed = @($checks.GetEnumerator() | Where-Object { -not $_.Value } | ForEach-Object Key)
$result = [ordered]@{
    schemaVersion = 'doroti.windows.resize-grid-validation/v1'
    generatedAtUtc = [DateTime]::UtcNow.ToString('O')
    status = if ($failed.Count -eq 0) { 'PASS' } else { 'FAIL' }
    sourceRevision = (& git -C $repo rev-parse HEAD).Trim()
    dirty = [bool](& git -C $repo status --porcelain)
    expectedGridIntervalPhysicalPixels = $expected
    frameCount = $frames.Count
    fixture = $fixture
    initial = $initial
    final = $final
    safeFillAndGridTailDiffer = $safeFillDistinct
    observerSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $observer).Hash.ToLowerInvariant()
    productSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $app).Hash.ToLowerInvariant()
    checks = $checks
    failedChecks = $failed
}
$resultPath = Join-Path $artifactRootPath 'r0-validation.json'
$result | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $resultPath -Encoding utf8NoBOM
Write-Output "EVIDENCE=$resultPath"
Write-Output "STATUS=$($result.status)"
if ($failed.Count) { Write-Output "FAILED_CHECKS=$($failed -join ',')"; exit 2 }
