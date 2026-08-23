#Requires -Version 7.0
param(
    [ValidateSet('Default', 'Skia')]
    [string] $Renderer = 'Default',

    [ValidateRange(1, 300)]
    [int] $DurationSeconds = 10,

    [ValidateRange(1, 1000)]
    [int] $PngStride = 10,

    [switch] $SkipBuild,

    [switch] $CollectAcceptanceFailures,

    [ValidateSet('Left', 'Right', 'Top', 'Bottom', 'TopLeft', 'TopRight', 'BottomLeft', 'BottomRight')]
    [string[]] $Edges = @('Left', 'Right', 'Top', 'Bottom', 'TopLeft', 'TopRight', 'BottomLeft', 'BottomRight'),

    [string] $EvidenceDirectory = (Join-Path $PSScriptRoot '../validation/evidence/flutter')
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$flutterSdk = Join-Path $repoRoot 'reference/flutter-master'
$flutterProject = Join-Path $repoRoot 'reference/flutter_sample_app'
$flutterExecutable = Join-Path $flutterProject 'build/windows/x64/runner/Release/flutter_sample_app.exe'
$captureSource = Join-Path $dorotiRoot 'validation/windows-resize-capture'
$captureBuild = Join-Path $repoRoot '.doroti/build/windows-resize-capture-vs'
$captureExecutable = Join-Path $captureBuild 'Release/Doroti.WindowsResizeCapture.exe'
$evidenceRoot = [IO.Path]::GetFullPath($EvidenceDirectory)
$allowedEvidenceRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'validation/evidence/flutter'))
if (-not $evidenceRoot.StartsWith($allowedEvidenceRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Evidence directory must stay under $allowedEvidenceRoot"
}
[IO.Directory]::CreateDirectory($evidenceRoot) | Out-Null
$fixedFlutterCommit = '56b8e1a851a594b1a154f8ea93270807dab22b9a'
$actualFlutterCommit = (& git -C $flutterSdk rev-parse HEAD).Trim()
if ($actualFlutterCommit -ne $fixedFlutterCommit) {
    throw "Flutter reference drifted: $actualFlutterCommit != $fixedFlutterCommit"
}

Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class FlutterResizeNative {
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }
    [DllImport("user32.dll", SetLastError=true)]
    public static extern bool SetWindowPos(IntPtr hwnd, IntPtr after, int x, int y, int width, int height, uint flags);
    [DllImport("user32.dll")]
    public static extern bool PostMessage(IntPtr hwnd, uint message, IntPtr wparam, IntPtr lparam);
    [DllImport("user32.dll")]
    public static extern uint GetDpiForWindow(IntPtr hwnd);
    [DllImport("user32.dll")]
    public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr context);
}
'@

$previousDpiAwareness = [FlutterResizeNative]::SetThreadDpiAwarenessContext([IntPtr]::new(-4))
if ($previousDpiAwareness -eq [IntPtr]::Zero) {
    throw 'Could not enable per-monitor-v2 DPI awareness for the Flutter baseline driver.'
}

function Invoke-CheckedProcess([string] $FileName, [string[]] $Arguments, [string] $WorkingDirectory) {
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.WorkingDirectory = $WorkingDirectory
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($argument in $Arguments) { [void]$start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::Start($start)
    if ($null -eq $process) { throw "Could not start $FileName" }
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit([int][TimeSpan]::FromMinutes(20).TotalMilliseconds)) {
        $process.Kill($true)
        throw "$FileName exceeded the 20-minute subprocess timeout."
    }
    $output = $stdout.GetAwaiter().GetResult()
    $errorOutput = $stderr.GetAwaiter().GetResult()
    if ($process.ExitCode -ne 0) { throw "$FileName failed ($($process.ExitCode)).`n$output`n$errorOutput" }
}

function Wait-MainWindow([Diagnostics.Process] $Process, [TimeSpan] $Timeout) {
    $deadline = [DateTime]::UtcNow + $Timeout
    while ([DateTime]::UtcNow -lt $deadline) {
        if ($Process.HasExited) { throw "Flutter exited before opening a window ($($Process.ExitCode))." }
        $Process.Refresh()
        if ($Process.MainWindowHandle -ne [IntPtr]::Zero) { return $Process.MainWindowHandle }
        Start-Sleep -Milliseconds 100
    }
    throw 'Flutter did not open a top-level window within 30 seconds.'
}

function Get-Percentile([double[]] $Values, [double] $Percentile) {
    if ($null -eq $Values -or $Values.Count -eq 0) { return $null }
    $ordered = @($Values | Sort-Object)
    $index = [Math]::Min($ordered.Count - 1, [Math]::Max(0, [Math]::Ceiling($ordered.Count * $Percentile) - 1))
    return [double]$ordered[$index]
}

function Get-SourceFingerprint([string[]] $RelativePaths) {
    $builder = [Text.StringBuilder]::new()
    foreach ($relativePath in ($RelativePaths | Sort-Object)) {
        $path = Join-Path $repoRoot $relativePath
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
        [void]$builder.Append($relativePath.Replace('\', '/')).Append('=').Append($hash).Append("`n")
    }
    return [Convert]::ToHexString(
        [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($builder.ToString()))
    ).ToLowerInvariant()
}

if (-not $SkipBuild) {
    Invoke-CheckedProcess (Join-Path $flutterSdk 'bin/flutter.bat') @('build', 'windows', '--release', '--no-pub') $flutterProject
}
Invoke-CheckedProcess 'cmake' @('-S', $captureSource, '-B', $captureBuild, '-G', 'Visual Studio 18 2026', '-A', 'x64') $repoRoot
Invoke-CheckedProcess 'cmake' @('--build', $captureBuild, '--config', 'Release') $repoRoot
if (-not (Test-Path -LiteralPath $flutterExecutable -PathType Leaf) -or
    -not (Test-Path -LiteralPath $captureExecutable -PathType Leaf)) {
    throw 'Flutter or WGC validator executable is missing after build.'
}

$groupId = "flutter-rsz-$($Renderer.ToLowerInvariant())-$((Get-Date).ToString('yyyyMMdd-HHmmss'))-$([Guid]::NewGuid().ToString('N').Substring(0, 8))"
$groupSummaryPath = Join-Path $evidenceRoot "$groupId.summary.json"
$runs = [Collections.Generic.List[object]]::new()
$sourceFingerprint = Get-SourceFingerprint @(
    'reference/flutter_sample_app/lib/main.dart',
    'reference/flutter_sample_app/windows/runner/main.cpp',
    'Doroti/validation/windows-resize-capture/CMakeLists.txt',
    'Doroti/validation/windows-resize-capture/main.cpp',
    'Doroti/eng/validate-flutter-windows-resize-baseline.ps1')

foreach ($edge in $Edges) {
    Write-Host "Flutter $Renderer resize baseline: $edge ($DurationSeconds seconds)"
    $runId = "$groupId-$($edge.ToLowerInvariant())"
    $rawPath = Join-Path $evidenceRoot "$runId.wgc.raw.json"
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $flutterExecutable
    $start.WorkingDirectory = [IO.Path]::GetDirectoryName($flutterExecutable)
    $start.UseShellExecute = $false
    if ($Renderer -eq 'Skia') { $start.Environment['DOROTI_FLUTTER_RENDERER'] = 'skia' }
    $flutterProcess = [Diagnostics.Process]::Start($start)
    if ($null -eq $flutterProcess) { throw 'Could not start the Flutter baseline.' }
    try {
        $hwnd = Wait-MainWindow $flutterProcess ([TimeSpan]::FromSeconds(30))
        if (-not [FlutterResizeNative]::SetWindowPos($hwnd, [IntPtr]::Zero, 20, 20, 1280, 720, 0x0040)) {
            throw 'Could not normalize the Flutter baseline window.'
        }
        Start-Sleep -Seconds 2
        $capture = [Diagnostics.ProcessStartInfo]::new()
        $capture.FileName = $captureExecutable
        $capture.WorkingDirectory = [IO.Path]::GetDirectoryName($captureExecutable)
        $capture.UseShellExecute = $false
        $capture.CreateNoWindow = $true
        $capture.RedirectStandardOutput = $true
        $capture.RedirectStandardError = $true
        foreach ($argument in @(
            '--hwnd', $hwnd.ToInt64().ToString([Globalization.CultureInfo]::InvariantCulture),
            '--output', $rawPath,
            '--run-id', $runId,
            '--edge', $edge,
            '--duration', $DurationSeconds.ToString([Globalization.CultureInfo]::InvariantCulture),
            '--input-hz', '0',
            '--png-stride', $PngStride.ToString([Globalization.CultureInfo]::InvariantCulture),
            '--oracle-stride', $PngStride.ToString([Globalization.CultureInfo]::InvariantCulture),
            '--no-anomaly-png')) { [void]$capture.ArgumentList.Add($argument) }
        $captureProcess = [Diagnostics.Process]::Start($capture)
        $stdout = $captureProcess.StandardOutput.ReadToEndAsync()
        $stderr = $captureProcess.StandardError.ReadToEndAsync()
        if (-not $captureProcess.WaitForExit([int][TimeSpan]::FromMinutes(20).TotalMilliseconds)) {
            $captureProcess.Kill($true)
            throw "$edge Flutter capture exceeded the 20-minute timeout."
        }
        $captureOutput = $stdout.GetAwaiter().GetResult()
        $captureError = $stderr.GetAwaiter().GetResult()
        if ($captureProcess.ExitCode -ne 0) {
            throw "$edge Flutter capture failed ($($captureProcess.ExitCode)).`n$captureOutput`n$captureError"
        }
    }
    finally {
        if ($flutterProcess -and -not $flutterProcess.HasExited) {
            [void][FlutterResizeNative]::PostMessage($hwnd, 0x0010, [IntPtr]::Zero, [IntPtr]::Zero)
            if (-not $flutterProcess.WaitForExit(5000)) { $flutterProcess.Kill($true) }
        }
    }

    $raw = Get-Content -LiteralPath $rawPath -Raw | ConvertFrom-Json -Depth 40
    $frequency = [Diagnostics.Stopwatch]::Frequency
    $refreshIntervalMicroseconds = 1000000.0 / [double]$raw.displayRefreshHz
    $transitions = [Collections.Generic.List[object]]::new()
    $previousKey = $null
    foreach ($sample in @($raw.windowSamples)) {
        $key = "$($sample.window.left),$($sample.window.top),$($sample.window.right),$($sample.window.bottom)"
        if ($key -ne $previousKey) { $transitions.Add($sample); $previousKey = $key }
    }
    # The drag path is cyclic, so the same size appears in later cycles. Match only
    # within the current one-second monotonic half-cycle and require the same size
    # direction. WGC's ContentSize is the DWM capture item, while input samples use
    # GetWindowRect; calibrate their fixed non-client difference from the initial
    # stable frame. Flutter handles WM_SIZE at about capture cadence here, so allow
    # one observed native transition step as interpolation error and report it.
    $inputToVisibleMicroseconds = [Collections.Generic.List[double]]::new()
    $matchedTransitions = [Collections.Generic.HashSet[long]]::new()
    $referenceFrame = @($raw.frames | Select-Object -First 1)[0]
    $captureToWindowWidth = if ($referenceFrame) {
        ([int]$referenceFrame.window.right - [int]$referenceFrame.window.left) - [int]$referenceFrame.width
    } else { 0 }
    $captureToWindowHeight = if ($referenceFrame) {
        ([int]$referenceFrame.window.bottom - [int]$referenceFrame.window.top) - [int]$referenceFrame.height
    } else { 0 }
    $isHorizontal = $edge -match 'Left|Right'
    $transitionEvents = [Collections.Generic.List[object]]::new()
    $transitionSteps = [Collections.Generic.List[int]]::new()
    $previousPrimary = $null
    $transitionDirection = 0
    foreach ($sample in $transitions) {
        $width = [int]$sample.window.right - [int]$sample.window.left
        $height = [int]$sample.window.bottom - [int]$sample.window.top
        $primary = if ($isHorizontal) { $width } else { $height }
        if ($null -ne $previousPrimary) {
            $step = $primary - $previousPrimary
            if ($step -ne 0) {
                $transitionDirection = [Math]::Sign($step)
                $transitionSteps.Add([Math]::Abs($step))
            }
        }
        $transitionEvents.Add([pscustomobject]@{
            sample = $sample
            performanceCounter = [long]$sample.performanceCounter
            width = $width
            height = $height
            direction = $transitionDirection
        })
        $previousPrimary = $primary
    }
    $geometryTolerance = if ($transitionSteps.Count) {
        [int](($transitionSteps.ToArray() | Measure-Object -Maximum).Maximum)
    } else { 1 }
    $geometryErrors = [Collections.Generic.List[int]]::new()
    $lastTransitionCounter = if ($transitions.Count) {
        [long]$transitions[$transitions.Count - 1].performanceCounter
    } else { 0L }
    $finalLatency = $null
    $previousFrameWidth = $null
    $previousFrameHeight = $null
    $frameDirection = 0
    foreach ($frame in @($raw.frames | Sort-Object { [long]$_.systemRelative100ns })) {
        # SystemRelativeTime is the WGC frame timestamp. The callback QPC can be
        # much later when pixel readback/oracle work backs up delivery.
        $frameCounter = [long][Math]::Round(
            [double]$frame.systemRelative100ns * $frequency / 10000000.0)
        $width = [int]$frame.width + $captureToWindowWidth
        $height = [int]$frame.height + $captureToWindowHeight
        if ($width -eq $previousFrameWidth -and $height -eq $previousFrameHeight) { continue }
        $primary = if ($isHorizontal) { $width } else { $height }
        $previousFramePrimary = if ($isHorizontal) { $previousFrameWidth } else { $previousFrameHeight }
        if ($null -ne $previousFramePrimary -and $primary -ne $previousFramePrimary) {
            $frameDirection = [Math]::Sign($primary - $previousFramePrimary)
        }
        $previousFrameWidth = $width
        $previousFrameHeight = $height
        $minimumCounter = $frameCounter - $frequency
        $candidate = $transitionEvents | Where-Object {
            $_.performanceCounter -le $frameCounter -and $_.performanceCounter -ge $minimumCounter -and
            ($frameDirection -eq 0 -or $_.direction -eq $frameDirection)
        } | ForEach-Object {
            [pscustomobject]@{
                event = $_
                error = [Math]::Abs($_.width - $width) + [Math]::Abs($_.height - $height)
            }
        } | Sort-Object error, @{ Expression = { $_.event.performanceCounter }; Descending = $true } | Select-Object -First 1
        if ($candidate -and [int]$candidate.error -le $geometryTolerance) {
            $sampleCounter = [long]$candidate.event.performanceCounter
            if ($matchedTransitions.Add($sampleCounter)) {
                $latency = (($frameCounter - $sampleCounter) * 1000000.0) / $frequency
                $inputToVisibleMicroseconds.Add($latency)
                $geometryErrors.Add([int]$candidate.error)
                if ($sampleCounter -eq $lastTransitionCounter) { $finalLatency = $latency }
            }
        }
    }
    if ($null -eq $finalLatency -and $transitionEvents.Count) {
        $finalEvent = $transitionEvents[$transitionEvents.Count - 1]
        $finalFrame = $raw.frames | Where-Object {
            $timestamp = [long][Math]::Round(
                [double]$_.systemRelative100ns * $frequency / 10000000.0)
            $visibleWidth = [int]$_.width + $captureToWindowWidth
            $visibleHeight = [int]$_.height + $captureToWindowHeight
            $timestamp -ge $finalEvent.performanceCounter -and
            ([Math]::Abs($visibleWidth - $finalEvent.width) +
                [Math]::Abs($visibleHeight - $finalEvent.height)) -le $geometryTolerance
        } | Sort-Object { [long]$_.systemRelative100ns } | Select-Object -First 1
        if ($finalFrame) {
            $finalFrameCounter = [long][Math]::Round(
                [double]$finalFrame.systemRelative100ns * $frequency / 10000000.0)
            $finalLatency = (($finalFrameCounter - $finalEvent.performanceCounter) * 1000000.0) / $frequency
        }
    }
    $p50 = Get-Percentile ([double[]]$inputToVisibleMicroseconds) 0.50
    $p95 = Get-Percentile ([double[]]$inputToVisibleMicroseconds) 0.95
    $p99 = Get-Percentile ([double[]]$inputToVisibleMicroseconds) 0.99
    $oracle = $raw.visualOracle
    $edgeTolerance = [Math]::Max(1, [Math]::Ceiling([double]$raw.windowDpi / 96.0))
    $minimumCircleSamples = [Math]::Max(4, [Math]::Ceiling([long]$raw.capturedFrames / [double]$PngStride * 0.1))
    $visualStatus = if ([long]$raw.captureErrors -eq 0 -and [long]$raw.encoderDroppedFrames -eq 0 -and
        [long]$raw.capturedFrames -ge ($DurationSeconds * 10) -and [long]$oracle.blankFrames -eq 0 -and
        [long]$oracle.appBarHeightFailures -eq 0 -and [long]$oracle.circleObservedFrames -ge $minimumCircleSamples -and
        [long]$oracle.circleAspectFailures -eq 0 -and [long]$oracle.titleObservedFrames -gt 0 -and
        [long]$oracle.titleNonUniformScaleFailures -eq 0 -and [long]$oracle.contentEdgeGapFrames -eq 0 -and
        [long]$oracle.finalContentLeftGapPixels -ge 0 -and [long]$oracle.finalContentRightGapPixels -ge 0 -and
        [long]$oracle.finalContentLeftGapPixels -le $edgeTolerance -and
        [long]$oracle.finalContentRightGapPixels -le $edgeTolerance) { 'PASS' } else { 'FAIL' }
    $cadenceStatus = if ($null -ne $p95 -and $null -ne $p99 -and $null -ne $finalLatency -and
        $p95 -le (2 * $refreshIntervalMicroseconds) -and $p99 -le (4 * $refreshIntervalMicroseconds) -and
        $finalLatency -le (2 * $refreshIntervalMicroseconds)) { 'PASS' } else { 'FAIL' }
    $runs.Add([ordered]@{
        edge = $edge
        runId = $runId
        rawEvidence = [IO.Path]::GetRelativePath($repoRoot, $rawPath).Replace('\', '/')
        visual = $visualStatus
        cadence = $cadenceStatus
        inputSamples = $raw.inputSamples
        nativeWindowTransitions = $transitions.Count
        capturedFrames = $raw.capturedFrames
        displayRefreshHz = $raw.displayRefreshHz
        inputIntervalMicroseconds = $raw.inputIntervalMicroseconds
        captureIntervalMicroseconds = $raw.captureIntervalMicroseconds
        inputToVisibleMicroseconds = [ordered]@{
            matched = $inputToVisibleMicroseconds.Count
            captureToWindowWidthPixels = $captureToWindowWidth
            captureToWindowHeightPixels = $captureToWindowHeight
            geometryTolerancePixels = $geometryTolerance
            maximumMatchedGeometryErrorPixels = if ($geometryErrors.Count) {
                [int](($geometryErrors.ToArray() | Measure-Object -Maximum).Maximum)
            } else { $null }
            p50 = $p50; p95 = $p95; p99 = $p99; final = $finalLatency
            p95RefreshIntervals = if ($null -ne $p95) { $p95 / $refreshIntervalMicroseconds } else { $null }
            p99RefreshIntervals = if ($null -ne $p99) { $p99 / $refreshIntervalMicroseconds } else { $null }
            finalRefreshIntervals = if ($null -ne $finalLatency) { $finalLatency / $refreshIntervalMicroseconds } else { $null }
        }
        oracle = $oracle
    })
}

$visualStatus = if (@($runs | Where-Object visual -ne 'PASS').Count) { 'FAIL' } else { 'PASS' }
$cadenceStatus = if (@($runs | Where-Object cadence -ne 'PASS').Count) { 'FAIL' } else { 'PASS' }
$summary = [ordered]@{
    schemaVersion = 'doroti.flutter-windows-resize-baseline/v1'
    groupId = $groupId
    capturedAt = [DateTimeOffset]::Now.ToString('o')
    renderer = $Renderer
    rendererSelection = if ($Renderer -eq 'Skia') { 'DartProject::set_impeller_switch(ImpellerSwitch::Disabled)' } else { 'DartProject default' }
    flutterCommit = $actualFlutterCommit
    flutterVersion = (& (Join-Path $flutterSdk 'bin/flutter.bat') --version --machine | ConvertFrom-Json)
    gitCommit = (& git -C $repoRoot rev-parse HEAD).Trim()
    sourceFingerprint = $sourceFingerprint
    durationSecondsPerEdge = $DurationSeconds
    edges = $Edges
    visual = $visualStatus
    cadence = $cadenceStatus
    cadenceAcceptance = 'native transition to directionally matched WGC ContentSize within one observed transition step: p95 <= 2 refresh, p99 <= 4 refresh, final <= 2 refresh'
    runs = $runs.ToArray()
}
[IO.File]::WriteAllText($groupSummaryPath, ($summary | ConvertTo-Json -Depth 30), [Text.UTF8Encoding]::new($false))
Write-Output "SUMMARY=$groupSummaryPath"
Write-Output ($summary | ConvertTo-Json -Depth 30 -Compress)
if (($visualStatus -ne 'PASS' -or $cadenceStatus -ne 'PASS') -and -not $CollectAcceptanceFailures) {
    throw "Flutter $Renderer resize baseline failed. See $groupSummaryPath"
}
