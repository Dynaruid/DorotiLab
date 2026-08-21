#Requires -Version 7.0
param(
    [Parameter(Mandatory)]
    [ValidateSet('default', '0', '1')]
    [string] $SwapInterval,

    [ValidateRange(1, 60)]
    [int] $DurationSeconds = 10,

    [string] $EvidenceDirectory = (Join-Path $PSScriptRoot '../validation/evidence/resize'),

    [switch] $RetainRawTrace
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$executable = Join-Path $repoRoot 'DorotiDemoApp/windows/bin/Release/net10.0-windows10.0.19041.0/win-x64/DorotiDemoApp.Windows.exe'
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "Build the Windows Demo Release executable before live resize validation: $executable"
}

$evidenceRoot = [IO.Path]::GetFullPath($EvidenceDirectory)
$allowedEvidenceRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'validation/evidence/resize'))
if (-not $evidenceRoot.StartsWith($allowedEvidenceRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Evidence directory must stay under $allowedEvidenceRoot"
}
[IO.Directory]::CreateDirectory($evidenceRoot) | Out-Null

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$rawEvidence = Join-Path $evidenceRoot "rsz0b-$SwapInterval-$stamp.raw.json"
$summaryEvidence = Join-Path $evidenceRoot "rsz0b-$SwapInterval-$stamp.summary.json"

if (-not ('DorotiResizeLiveNative' -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;

public static class DorotiResizeLiveNative
{
    public const uint WM_CLOSE = 0x0010;
    public const uint WM_NCLBUTTONDOWN = 0x00A1;
    public const int HTBOTTOMRIGHT = 17;
    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint SPI_GETWORKAREA = 0x0030;
    public const uint SWP_SHOWWINDOW = 0x0040;
    public static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);

    [DllImport("user32.dll")]
    public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr dpiContext);

    [DllImport("user32.dll")]
    public static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SystemParametersInfo(uint action, uint parameter, ref RECT value, uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(
        IntPtr hwnd, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    public static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extraInfo);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

    public static IntPtr MakeScreenPointLParam(int x, int y) =>
        (IntPtr)((y << 16) | (x & 0xffff));
}
'@
}

function Get-Percentile([long[]] $Values, [double] $Percentile) {
    if ($Values.Count -eq 0) { return $null }
    $ordered = @($Values | Sort-Object)
    $index = [Math]::Ceiling(($ordered.Count * $Percentile)) - 1
    return $ordered[[Math]::Max(0, [Math]::Min($ordered.Count - 1, $index))]
}

function Wait-ForWindow([Diagnostics.Process] $Process, [TimeSpan] $Timeout) {
    $deadline = [DateTime]::UtcNow + $Timeout
    while ([DateTime]::UtcNow -lt $deadline) {
        if ($Process.HasExited) { throw "Windows Demo exited before creating a native window (exit $($Process.ExitCode))." }
        $Process.Refresh()
        if ($Process.MainWindowHandle -ne [IntPtr]::Zero) { return $Process.MainWindowHandle }
        Start-Sleep -Milliseconds 100
    }
    throw "Windows Demo did not create a native window within $($Timeout.TotalSeconds) seconds."
}

$startInfo = [Diagnostics.ProcessStartInfo]::new()
$startInfo.FileName = $executable
$startInfo.WorkingDirectory = [IO.Path]::GetDirectoryName($executable)
$startInfo.UseShellExecute = $false
$startInfo.Environment['DOROTI_MAUI_EVIDENCE'] = $rawEvidence
$startInfo.Environment['DOROTI_WINDOWS_EGL_SWAP_INTERVAL'] = $SwapInterval

$previousDpiAwareness = [DorotiResizeLiveNative]::SetThreadDpiAwarenessContext(
    [DorotiResizeLiveNative]::DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2)
if ($previousDpiAwareness -eq [IntPtr]::Zero) {
    throw 'Unable to make the validation thread per-monitor DPI aware.'
}

$process = [Diagnostics.Process]::Start($startInfo)
if ($null -eq $process) { throw 'Failed to start the Windows Demo process.' }
$closeWithinFiveSeconds = $false
$dragStartedAt = [DateTimeOffset]::UtcNow
try {
    $hwnd = Wait-ForWindow $process ([TimeSpan]::FromSeconds(30))
    Start-Sleep -Seconds 2
    $windowDpi = [DorotiResizeLiveNative]::GetDpiForWindow($hwnd)
    $dpiScale = [Math]::Max(1, $windowDpi / 96.0)
    $workArea = [DorotiResizeLiveNative+RECT]::new()
    if (-not [DorotiResizeLiveNative]::SystemParametersInfo(
        [DorotiResizeLiveNative]::SPI_GETWORKAREA, 0, [ref] $workArea, 0)) {
        throw 'Unable to query the DPI-aware desktop work area.'
    }
    $validationWidth = [Math]::Min(
        [Math]::Round(640 * $dpiScale),
        ($workArea.Right - $workArea.Left) - 80)
    $validationHeight = [Math]::Min(
        [Math]::Round(360 * $dpiScale),
        ($workArea.Bottom - $workArea.Top) - 80)
    if (-not [DorotiResizeLiveNative]::SetWindowPos(
        $hwnd,
        [IntPtr]::Zero,
        $workArea.Left + 20,
        $workArea.Top + 20,
        $validationWidth,
        $validationHeight,
        [DorotiResizeLiveNative]::SWP_SHOWWINDOW)) {
        throw 'Unable to place the Windows Demo inside the DPI-aware work area.'
    }
    Start-Sleep -Seconds 1
    $rect = [DorotiResizeLiveNative+RECT]::new()
    if (-not [DorotiResizeLiveNative]::GetWindowRect($hwnd, [ref] $rect)) {
        throw 'GetWindowRect failed for the Windows Demo.'
    }
    $initialWidth = $rect.Right - $rect.Left
    $initialHeight = $rect.Bottom - $rect.Top
    if ($initialWidth -lt 640 -or $initialHeight -lt 480) {
        throw "Live range isolation requires an initial window of at least 640x480; actual ${initialWidth}x${initialHeight}."
    }

    [DorotiResizeLiveNative]::SetForegroundWindow($hwnd) | Out-Null
    $resizeStartX = $rect.Right - 4
    $resizeStartY = $rect.Bottom - 4
    [DorotiResizeLiveNative]::SetCursorPos($resizeStartX, $resizeStartY) | Out-Null
    Start-Sleep -Milliseconds 150
    $resizeStarted = [DorotiResizeLiveNative]::PostMessage(
        $hwnd,
        [DorotiResizeLiveNative]::WM_NCLBUTTONDOWN,
        [IntPtr][DorotiResizeLiveNative]::HTBOTTOMRIGHT,
        [DorotiResizeLiveNative]::MakeScreenPointLParam($resizeStartX, $resizeStartY))
    if (-not $resizeStarted) { throw 'Unable to start the Win32 interactive sizing loop.' }
    Start-Sleep -Milliseconds 150
    $dragStartedAt = [DateTimeOffset]::UtcNow
    $steps = [Math]::Max(60, $DurationSeconds * 60)
    $inputSamples = [Collections.Generic.List[object]]::new($steps)
    for ($step = 0; $step -lt $steps; $step++) {
        $phase = $step / [double]($steps - 1)
        $cycle = ($phase * 4) % 1
        $wave = if ($cycle -lt 0.5) { $cycle * 2 } else { 2 - ($cycle * 2) }
        $x = $rect.Right - 1 + [Math]::Round(260 * $wave)
        $y = $rect.Bottom - 1 + [Math]::Round(140 * $wave)
        [DorotiResizeLiveNative]::SetCursorPos($x, $y) | Out-Null
        Start-Sleep -Milliseconds 16
        $sampleRect = [DorotiResizeLiveNative+RECT]::new()
        if ([DorotiResizeLiveNative]::GetWindowRect($hwnd, [ref] $sampleRect)) {
            $inputSamples.Add([pscustomobject]@{
                performanceCounter = [Diagnostics.Stopwatch]::GetTimestamp()
                windowWidth = $sampleRect.Right - $sampleRect.Left
                windowHeight = $sampleRect.Bottom - $sampleRect.Top
            })
        }
    }
    [DorotiResizeLiveNative]::mouse_event([DorotiResizeLiveNative]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Seconds 2

    if (-not (Test-Path -LiteralPath $rawEvidence -PathType Leaf)) {
        throw "Windows Demo did not write resize evidence: $rawEvidence"
    }
    $diagnostics = Get-Content -LiteralPath $rawEvidence -Raw | ConvertFrom-Json
    $trace = @($diagnostics.surface.resizeTrace)
    $targets = @($trace | Where-Object phase -eq 'target')
    $preSwaps = @($trace | Where-Object phase -eq 'pre-swap')
    $postSwaps = @($trace | Where-Object phase -eq 'post-swap')
    $surfaceReady = @($trace | Where-Object phase -eq 'surface-ready')
    $aggregateSwaps = @($trace | Where-Object phase -eq 'swap-boundary-end')
    $dwmFlushes = @($trace | Where-Object phase -eq 'dwm-flush-end')
    $eglStates = @($trace | Where-Object phase -eq 'egl-state')
    $acks = @($trace | Where-Object { $_.phase -eq 'ack' -and $_.terminal -eq 'presented' } | Sort-Object sequence)
    $postSwapDurations = @($postSwaps | ForEach-Object { [long] $_.durationMicroseconds })
    $aggregateSwapDurations = @($aggregateSwaps | ForEach-Object { [long] $_.durationMicroseconds })
    $dwmFlushDurations = @($dwmFlushes | ForEach-Object { [long] $_.durationMicroseconds })
    $targetGenerations = @($targets | ForEach-Object { [long] $_.epoch.generation })
    $distinctTargetGenerations = @($targetGenerations | Sort-Object -Unique)
    $minimumResizeGenerations = [Math]::Max(30, $DurationSeconds * 10)
    if ($distinctTargetGenerations.Count -lt $minimumResizeGenerations) {
        throw "Interactive resize was not captured: expected at least $minimumResizeGenerations distinct generations, actual $($distinctTargetGenerations.Count)."
    }
    $generationRegressions = 0
    for ($index = 1; $index -lt $targetGenerations.Count; $index++) {
        if ($targetGenerations[$index] -lt $targetGenerations[$index - 1]) { $generationRegressions++ }
    }
    $matchedGenerations = @($distinctTargetGenerations | Where-Object {
        $generation = $_
        ($preSwaps.epoch.generation -contains $generation) -and
        ($postSwaps.epoch.generation -contains $generation) -and
        ($acks.epoch.generation -contains $generation)
    })
    $ackIntervals = @()
    for ($index = 1; $index -lt $acks.Count; $index++) {
        $ackIntervals += [long]($acks[$index].timestampMicroseconds - $acks[$index - 1].timestampMicroseconds)
    }
    $over50MillisecondIntervals = @($ackIntervals | Where-Object { $_ -gt 50000 })
    $maxConsecutiveOver50Milliseconds = 0
    $currentConsecutiveOver50Milliseconds = 0
    foreach ($interval in $ackIntervals) {
        if ($interval -gt 50000) {
            $currentConsecutiveOver50Milliseconds++
            $maxConsecutiveOver50Milliseconds = [Math]::Max(
                $maxConsecutiveOver50Milliseconds,
                $currentConsecutiveOver50Milliseconds)
        } else {
            $currentConsecutiveOver50Milliseconds = 0
        }
    }
    $targetToAckDurations = @()
    foreach ($generation in $matchedGenerations) {
        $target = $targets | Where-Object { $_.epoch.generation -eq $generation } | Select-Object -First 1
        $ack = $acks | Where-Object { $_.epoch.generation -eq $generation } | Select-Object -Last 1
        $targetToAckDurations += [long]($ack.timestampMicroseconds - $target.timestampMicroseconds)
    }
    $presentedGenerationLags = @()
    foreach ($ack in $acks) {
        $latestTarget = $targets | Where-Object {
            [long]$_.timestampMicroseconds -le [long]$ack.timestampMicroseconds
        } | Select-Object -Last 1
        if ($null -ne $latestTarget) {
            $presentedGenerationLags += [long]$latestTarget.epoch.generation - [long]$ack.epoch.generation
        }
    }
    $exactSizeMismatches = @($preSwaps | Where-Object {
        $_.surfaceWidth -ne $_.epoch.physicalWidth -or
        $_.surfaceHeight -ne $_.epoch.physicalHeight
    })
    $finalSample = $inputSamples | Select-Object -Last 1
    $borderToTargetCatchUp = @()
    $borderWidthLagPixels = @()
    $borderHeightLagPixels = @()
    if ($null -ne $finalSample -and $targets.Count -gt 0) {
        $chromeWidth = [int]$finalSample.windowWidth - [int]$diagnostics.surface.pixelWidth
        $chromeHeight = [int]$finalSample.windowHeight - [int]$diagnostics.surface.pixelHeight
        $orderedTargets = @($targets | Sort-Object performanceCounter)
        foreach ($sample in $inputSamples) {
            $expectedWidth = [int]$sample.windowWidth - $chromeWidth
            $expectedHeight = [int]$sample.windowHeight - $chromeHeight
            $latest = $orderedTargets | Where-Object {
                [long]$_.performanceCounter -le [long]$sample.performanceCounter
            } | Select-Object -Last 1
            if ($null -ne $latest) {
                $borderWidthLagPixels += [Math]::Abs($expectedWidth - [int]$latest.epoch.physicalWidth)
                $borderHeightLagPixels += [Math]::Abs($expectedHeight - [int]$latest.epoch.physicalHeight)
            }
            if ($null -ne $latest -and
                [int]$latest.epoch.physicalWidth -eq $expectedWidth -and
                [int]$latest.epoch.physicalHeight -eq $expectedHeight) {
                $borderToTargetCatchUp += 0L
                continue
            }
            $matchingTarget = $orderedTargets | Where-Object {
                [long]$_.performanceCounter -gt [long]$sample.performanceCounter -and
                [int]$_.epoch.physicalWidth -eq $expectedWidth -and
                [int]$_.epoch.physicalHeight -eq $expectedHeight
            } | Select-Object -First 1
            if ($null -ne $matchingTarget) {
                $borderToTargetCatchUp += [long][Math]::Round(
                    (([long]$matchingTarget.performanceCounter - [long]$sample.performanceCounter) * 1000000.0) /
                    [Diagnostics.Stopwatch]::Frequency)
            }
        }
    }
    $exceptionPath = "$rawEvidence.exception.txt"
    $summary = [ordered]@{
        schemaVersion = 'doroti.resize-continuity-live/v2'
        capturedAt = [DateTimeOffset]::Now.ToString('o')
        swapInterval = $SwapInterval
        durationSeconds = $DurationSeconds
        inputMethod = 'Win32 WM_NCLBUTTONDOWN HTBOTTOMRIGHT interactive resize plus cursor movement'
        inputMotion = 'constant-velocity triangle wave (4 cycles)'
        inputCursorSamples = $steps
        retainedTraceEntries = $trace.Count
        resizeTargetActivations = $diagnostics.surface.resizeContinuityActivations
        screenRecording = 'notVerified'
        coordinateSampling = [ordered]@{
            dpiAwareness = 'per-monitor-v2'
            windowDpi = $windowDpi
        }
        visualCapture = [ordered]@{
            status = 'notVerified'
            reason = 'GDI screen capture omits the DXGI composition swap chain; use Windows Graphics Capture, external recording, or direct observation.'
        }
        processId = $process.Id
        initialWindow = [ordered]@{ width = $initialWidth; height = $initialHeight }
        rangeIsolation = 'window never intentionally smaller than initial size'
        nativeViewType = $diagnostics.surface.nativeViewType
        graphicsBackend = $diagnostics.surface.graphicsBackend
        dwmCompositionEnabled = $diagnostics.surface.dwmCompositionEnabled
        eglSwapIntervalPolicy = $diagnostics.surface.eglSwapIntervalPolicy
        exactSwapTimingAvailable = $diagnostics.surface.exactSwapTimingAvailable
        targetCount = $targets.Count
        distinctTargetGenerations = $distinctTargetGenerations.Count
        generationMinimum = if ($distinctTargetGenerations.Count -gt 0) { $distinctTargetGenerations[0] } else { $null }
        generationMaximum = if ($distinctTargetGenerations.Count -gt 0) { $distinctTargetGenerations[-1] } else { $null }
        preSwapCount = $preSwaps.Count
        postSwapCount = $postSwaps.Count
        eglStateCount = $eglStates.Count
        presentedAckCount = $acks.Count
        matchedTargetToAckGenerations = $matchedGenerations.Count
        generationRegressions = $generationRegressions
        exactTargetToSwapSizeMismatches = $exactSizeMismatches.Count
        ackIntervalMicroseconds = [ordered]@{
            p50 = Get-Percentile $ackIntervals 0.50
            p95 = Get-Percentile $ackIntervals 0.95
            p99 = Get-Percentile $ackIntervals 0.99
            over50Milliseconds = $over50MillisecondIntervals.Count
            maxConsecutiveOver50Milliseconds = $maxConsecutiveOver50Milliseconds
        }
        targetToAckMicroseconds = [ordered]@{
            p50 = Get-Percentile $targetToAckDurations 0.50
            p95 = Get-Percentile $targetToAckDurations 0.95
            p99 = Get-Percentile $targetToAckDurations 0.99
        }
        latestTargetAtPresentedAck = [ordered]@{
            laggedAckCount = @($presentedGenerationLags | Where-Object { $_ -gt 0 }).Count
            maximumGenerationLag = if ($presentedGenerationLags.Count -gt 0) {
                ($presentedGenerationLags | Measure-Object -Maximum).Maximum
            } else { $null }
        }
        supersededWork = [ordered]@{
            preRaster = @($trace | Where-Object { $_.phase -eq 'ack' -and $_.source -eq 'pre-raster latest target gate' }).Count
            preFlush = @($trace | Where-Object { $_.phase -eq 'ack' -and $_.source -eq 'pre-flush latest target gate' }).Count
            postFlush = @($trace | Where-Object { $_.phase -eq 'ack' -and $_.source -eq 'latest target gate' }).Count
        }
        borderToTarget = [ordered]@{
            sampledWindowRects = $inputSamples.Count
            matchedCatchUps = $borderToTargetCatchUp.Count
            catchUpMicroseconds = [ordered]@{
                p50 = Get-Percentile ([long[]]$borderToTargetCatchUp) 0.50
                p95 = Get-Percentile ([long[]]$borderToTargetCatchUp) 0.95
                p99 = Get-Percentile ([long[]]$borderToTargetCatchUp) 0.99
            }
            widthLagPixels = [ordered]@{
                p50 = Get-Percentile ([long[]]$borderWidthLagPixels) 0.50
                p95 = Get-Percentile ([long[]]$borderWidthLagPixels) 0.95
            }
            heightLagPixels = [ordered]@{
                p50 = Get-Percentile ([long[]]$borderHeightLagPixels) 0.50
                p95 = Get-Percentile ([long[]]$borderHeightLagPixels) 0.95
            }
        }
        finalSwapMicroseconds = [ordered]@{
            p50 = Get-Percentile $postSwapDurations 0.50
            p95 = Get-Percentile $postSwapDurations 0.95
            p99 = Get-Percentile $postSwapDurations 0.99
        }
        surfacePrepareMicroseconds = [ordered]@{
            p50 = Get-Percentile ([long[]]@($surfaceReady.durationMicroseconds)) 0.50
            p95 = Get-Percentile ([long[]]@($surfaceReady.durationMicroseconds)) 0.95
            p99 = Get-Percentile ([long[]]@($surfaceReady.durationMicroseconds)) 0.99
            resizeBuffersCount = @($surfaceReady | Where-Object detail -eq 'resized=True').Count
        }
        aggregateInvalidateMicroseconds = [ordered]@{
            p50 = Get-Percentile $aggregateSwapDurations 0.50
            p95 = Get-Percentile $aggregateSwapDurations 0.95
            p99 = Get-Percentile $aggregateSwapDurations 0.99
        }
        dwmFlushMicroseconds = [ordered]@{
            p50 = Get-Percentile $dwmFlushDurations 0.50
            p95 = Get-Percentile $dwmFlushDurations 0.95
            p99 = Get-Percentile $dwmFlushDurations 0.99
        }
        frameworkException = if (Test-Path -LiteralPath $exceptionPath) {
            Get-Content -LiteralPath $exceptionPath -Raw
        } else { $null }
        rawTraceRetained = [bool] $RetainRawTrace
        rawEvidence = if ($RetainRawTrace) {
            [IO.Path]::GetRelativePath($repoRoot, $rawEvidence).Replace('\', '/')
        } else { $null }
    }
    $summary | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $summaryEvidence -Encoding utf8
}
finally {
    [DorotiResizeLiveNative]::mouse_event([DorotiResizeLiveNative]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    if (-not $process.HasExited) {
        $process.Refresh()
        $hwnd = $process.MainWindowHandle
        if ($hwnd -ne [IntPtr]::Zero) {
            [DorotiResizeLiveNative]::PostMessage($hwnd, [DorotiResizeLiveNative]::WM_CLOSE, [IntPtr]::Zero, [IntPtr]::Zero) | Out-Null
        }
        $closeWithinFiveSeconds = $process.WaitForExit(5000)
        if (-not $closeWithinFiveSeconds) {
            $process.Kill($true)
            $process.WaitForExit()
        }
    } else {
        $closeWithinFiveSeconds = $true
    }
    [DorotiResizeLiveNative]::SetThreadDpiAwarenessContext($previousDpiAwareness) | Out-Null
}

$summary = Get-Content -LiteralPath $summaryEvidence -Raw | ConvertFrom-Json
$summary | Add-Member -NotePropertyName closeWithinFiveSeconds -NotePropertyValue $closeWithinFiveSeconds -Force
$summary | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $summaryEvidence -Encoding utf8
if ($null -ne $summary.frameworkException) {
    throw "Framework exception captured during live resize. See $summaryEvidence"
}
if ($summary.generationRegressions -ne 0 -or
    $summary.exactTargetToSwapSizeMismatches -ne 0) {
    throw "Resize correctness gate failed. See $summaryEvidence"
}
if (-not $RetainRawTrace -and (Test-Path -LiteralPath $rawEvidence -PathType Leaf)) {
    Remove-Item -LiteralPath $rawEvidence
}
Write-Output "SUMMARY=$summaryEvidence"
Write-Output ($summary | ConvertTo-Json -Depth 12 -Compress)
