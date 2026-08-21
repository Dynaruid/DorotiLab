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

$process = [Diagnostics.Process]::Start($startInfo)
if ($null -eq $process) { throw 'Failed to start the Windows Demo process.' }
$closeWithinFiveSeconds = $false
$dragStartedAt = [DateTimeOffset]::UtcNow
try {
    $hwnd = Wait-ForWindow $process ([TimeSpan]::FromSeconds(30))
    Start-Sleep -Seconds 2
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
    for ($step = 0; $step -lt $steps; $step++) {
        $phase = $step / [double]($steps - 1)
        $wave = 0.5 - 0.5 * [Math]::Cos($phase * [Math]::PI * 8)
        $x = $rect.Right - 1 + [Math]::Round(260 * $wave)
        $y = $rect.Bottom - 1 + [Math]::Round(140 * $wave)
        [DorotiResizeLiveNative]::SetCursorPos($x, $y) | Out-Null
        Start-Sleep -Milliseconds 16
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
    $exactSizeMismatches = @($preSwaps | Where-Object {
        $_.surfaceWidth -ne $_.epoch.physicalWidth -or
        $_.surfaceHeight -ne $_.epoch.physicalHeight
    })
    $exceptionPath = "$rawEvidence.exception.txt"
    $summary = [ordered]@{
        schemaVersion = 'doroti.resize-continuity-live/v2'
        capturedAt = [DateTimeOffset]::Now.ToString('o')
        swapInterval = $SwapInterval
        durationSeconds = $DurationSeconds
        inputMethod = 'Win32 WM_NCLBUTTONDOWN HTBOTTOMRIGHT interactive resize plus cursor movement'
        screenRecording = 'notVerified'
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
        finalSwapMicroseconds = [ordered]@{
            p50 = Get-Percentile $postSwapDurations 0.50
            p95 = Get-Percentile $postSwapDurations 0.95
            p99 = Get-Percentile $postSwapDurations 0.99
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
}

$summary = Get-Content -LiteralPath $summaryEvidence -Raw | ConvertFrom-Json
$summary | Add-Member -NotePropertyName closeWithinFiveSeconds -NotePropertyValue $closeWithinFiveSeconds -Force
$summary | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $summaryEvidence -Encoding utf8
if (-not $RetainRawTrace -and (Test-Path -LiteralPath $rawEvidence -PathType Leaf)) {
    Remove-Item -LiteralPath $rawEvidence
}
Write-Output "SUMMARY=$summaryEvidence"
Write-Output ($summary | ConvertTo-Json -Depth 12 -Compress)
