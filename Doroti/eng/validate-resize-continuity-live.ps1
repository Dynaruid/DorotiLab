#Requires -Version 7.0
param(
    [Parameter(Mandatory)]
    [ValidateSet('default', '0', '1')]
    [string] $SwapInterval,

    [ValidateRange(10, 300)]
    [int] $DurationSeconds = 60,

    [ValidateRange(0, 120)]
    [int] $PostDragObservationSeconds = 15,

    [switch] $KeepWindowOpen,

    [ValidateSet('Left', 'Right', 'Top', 'Bottom', 'TopLeft', 'TopRight', 'BottomLeft', 'BottomRight')]
    [string] $Edge = 'Left',

    [string] $EvidenceDirectory = (Join-Path $PSScriptRoot '../validation/evidence/resize'),

    [switch] $RetainRawTrace,

    [switch] $WindowsGraphicsCapture,

    [ValidateRange(0, 1000)]
    [int] $InputHz = 0,

    [ValidateRange(1, 1000)]
    [int] $PngStride = 1,

    [string] $VisualChildClass,

    [ValidateRange(320, 1200)]
    [int] $InitialLogicalWidth = 640,

    [ValidateRange(240, 720)]
    [int] $InitialLogicalHeight = 360,

    [switch] $AllowCadenceFailure,

    [switch] $AllowVisualFailure
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
if ($InputHz -gt 0 -and $InputHz -lt 30) {
    throw 'InputHz must be 0 for active-display auto detection or at least 30 Hz.'
}

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$edgeSlug = $Edge.ToLowerInvariant()
$runId = "win-rsz-$SwapInterval-$edgeSlug-$stamp-$([Guid]::NewGuid().ToString('N').Substring(0, 8))"
$rawEvidence = Join-Path $evidenceRoot "$runId.app.raw.json"
$visualEvidence = Join-Path $evidenceRoot "$runId.wgc.raw.json"
$summaryEvidence = Join-Path $evidenceRoot "$runId.summary.json"
$captureSource = Join-Path $dorotiRoot 'validation/windows-resize-capture'
$captureBuild = Join-Path $repoRoot '.doroti/build/windows-resize-capture-vs'
$captureExecutable = Join-Path $captureBuild 'Release/Doroti.WindowsResizeCapture.exe'

if (-not ('DorotiResizeLiveNative' -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;

public static class DorotiResizeLiveNative
{
    public const uint WM_CLOSE = 0x0010;
    public const uint WM_CANCELMODE = 0x001F;
    public const uint WM_NCLBUTTONDOWN = 0x00A1;
    public const int HTLEFT = 10;
    public const int HTRIGHT = 11;
    public const int HTTOP = 12;
    public const int HTTOPLEFT = 13;
    public const int HTTOPRIGHT = 14;
    public const int HTBOTTOM = 15;
    public const int HTBOTTOMLEFT = 16;
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

function Get-SourceFingerprint([string[]] $RelativePaths) {
    $builder = [Text.StringBuilder]::new()
    foreach ($relativePath in ($RelativePaths | Sort-Object)) {
        $path = Join-Path $repoRoot $relativePath
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Source fingerprint input is missing: $relativePath"
        }
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
        [void]$builder.Append($relativePath.Replace('\', '/')).Append('=').Append($hash).Append("`n")
    }
    $bytes = [Text.Encoding]::UTF8.GetBytes($builder.ToString())
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
}

$sourceFingerprint = Get-SourceFingerprint @(
    'Doroti/Directory.Packages.props',
    'Doroti/src/Doroti.Host.Maui/Doroti.Host.Maui.csproj',
    'Doroti/src/Doroti.Host.Maui/DorotiWindowsDxgiSurface.cs',
    'Doroti/src/Doroti.Host.Maui/WindowsCompositionSurfacePresenter.cs',
    'Doroti/src/Doroti.Host.Maui/MauiHostAdapter.cs',
    'Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs',
    'Doroti/src/Doroti.Ui/ResizeLifecycle.cs',
    'Doroti/eng/validate-resize-continuity-live.ps1',
    'Doroti/validation/windows-resize-capture/CMakeLists.txt',
    'Doroti/validation/windows-resize-capture/main.cpp')
$gitCommit = (& git -C $repoRoot rev-parse HEAD).Trim()
$operatingSystem = Get-CimInstance Win32_OperatingSystem
$graphicsAdapters = @(Get-CimInstance Win32_VideoController | ForEach-Object {
    [ordered]@{
        name = $_.Name
        driverVersion = $_.DriverVersion
        currentWidth = $_.CurrentHorizontalResolution
        currentHeight = $_.CurrentVerticalResolution
        currentRefreshHz = $_.CurrentRefreshRate
        pnpDeviceId = $_.PNPDeviceID
    }
})

if ($WindowsGraphicsCapture) {
    & cmake -S $captureSource -B $captureBuild -G 'Visual Studio 18 2026' -A x64
    if ($LASTEXITCODE -ne 0) { throw "Windows Graphics Capture configure failed (exit $LASTEXITCODE)." }
    & cmake --build $captureBuild --config Release
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $captureExecutable -PathType Leaf)) {
        throw "Windows Graphics Capture build failed (exit $LASTEXITCODE)."
    }
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
        [Math]::Round($InitialLogicalWidth * $dpiScale),
        ($workArea.Right - $workArea.Left) - 80)
    $validationHeight = [Math]::Min(
        [Math]::Round($InitialLogicalHeight * $dpiScale),
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

    $movesLeft = $Edge -in @('Left', 'TopLeft', 'BottomLeft')
    $movesRight = $Edge -in @('Right', 'TopRight', 'BottomRight')
    $movesTop = $Edge -in @('Top', 'TopLeft', 'TopRight')
    $movesBottom = $Edge -in @('Bottom', 'BottomLeft', 'BottomRight')
    $visualDiagnostics = $null
    if ($WindowsGraphicsCapture) {
        $dragStartedAt = [DateTimeOffset]::UtcNow
        $captureStart = [Diagnostics.ProcessStartInfo]::new()
        $captureStart.FileName = $captureExecutable
        $captureStart.WorkingDirectory = [IO.Path]::GetDirectoryName($captureExecutable)
        $captureStart.UseShellExecute = $false
        $captureStart.CreateNoWindow = $true
        $captureStart.RedirectStandardOutput = $true
        $captureStart.RedirectStandardError = $true
        $captureArguments = @(
            '--hwnd', $hwnd.ToInt64().ToString([Globalization.CultureInfo]::InvariantCulture),
            '--output', $visualEvidence,
            '--run-id', $runId,
            '--edge', $Edge,
            '--duration', $DurationSeconds.ToString([Globalization.CultureInfo]::InvariantCulture),
            '--input-hz', $InputHz.ToString([Globalization.CultureInfo]::InvariantCulture),
            '--png-stride', $PngStride.ToString([Globalization.CultureInfo]::InvariantCulture),
            '--oracle-stride', $PngStride.ToString([Globalization.CultureInfo]::InvariantCulture),
            '--no-anomaly-png')
        if ($VisualChildClass) {
            $captureArguments += @('--visual-child-class', $VisualChildClass)
        }
        foreach ($argument in $captureArguments) {
            [void]$captureStart.ArgumentList.Add([string]$argument)
        }
        $captureProcess = [Diagnostics.Process]::Start($captureStart)
        if ($null -eq $captureProcess) { throw 'Unable to start the Windows Graphics Capture validator.' }
        $captureStdout = $captureProcess.StandardOutput.ReadToEndAsync()
        $captureStderr = $captureProcess.StandardError.ReadToEndAsync()
        if (-not $captureProcess.WaitForExit([int][TimeSpan]::FromMinutes(20).TotalMilliseconds)) {
            $captureProcess.Kill($true)
            throw 'Windows Graphics Capture validator exceeded the 20-minute test timeout.'
        }
        $captureOutput = $captureStdout.GetAwaiter().GetResult()
        $captureError = $captureStderr.GetAwaiter().GetResult()
        if ($captureProcess.ExitCode -ne 0) {
            throw "Windows Graphics Capture validator failed with exit code $($captureProcess.ExitCode).`n$captureOutput`n$captureError"
        }
        $visualDiagnostics = Get-Content -LiteralPath $visualEvidence -Raw | ConvertFrom-Json -Depth 40
        if ($visualDiagnostics.runId -ne $runId) {
            throw "Windows Graphics Capture run ID mismatch: $($visualDiagnostics.runId) != $runId"
        }
        $inputSamples = @($visualDiagnostics.windowSamples | ForEach-Object {
            [pscustomobject]@{
                performanceCounter = [long]$_.performanceCounter
                windowLeft = [int]$_.window.left
                windowTop = [int]$_.window.top
                windowRight = [int]$_.window.right
                windowBottom = [int]$_.window.bottom
                windowWidth = [int]$_.window.width
                windowHeight = [int]$_.window.height
            }
        })
    } else {
        $hitTest = switch ($Edge) {
            'Left' { [DorotiResizeLiveNative]::HTLEFT }
            'Right' { [DorotiResizeLiveNative]::HTRIGHT }
            'Top' { [DorotiResizeLiveNative]::HTTOP }
            'Bottom' { [DorotiResizeLiveNative]::HTBOTTOM }
            'TopLeft' { [DorotiResizeLiveNative]::HTTOPLEFT }
            'TopRight' { [DorotiResizeLiveNative]::HTTOPRIGHT }
            'BottomLeft' { [DorotiResizeLiveNative]::HTBOTTOMLEFT }
            'BottomRight' { [DorotiResizeLiveNative]::HTBOTTOMRIGHT }
        }
        [DorotiResizeLiveNative]::SetForegroundWindow($hwnd) | Out-Null
        $resizeStartX = if ($movesLeft) {
            $rect.Left + 4
        } elseif ($movesRight) {
            $rect.Right - 4
        } else {
            [Math]::Round(($rect.Left + $rect.Right) / 2.0)
        }
        $resizeStartY = if ($movesTop) {
            $rect.Top + 4
        } elseif ($movesBottom) {
            $rect.Bottom - 4
        } else {
            [Math]::Round(($rect.Top + $rect.Bottom) / 2.0)
        }
        [DorotiResizeLiveNative]::SetCursorPos($resizeStartX, $resizeStartY) | Out-Null
        Start-Sleep -Milliseconds 150
        $resizeStarted = [DorotiResizeLiveNative]::PostMessage(
            $hwnd,
            [DorotiResizeLiveNative]::WM_NCLBUTTONDOWN,
            [IntPtr]$hitTest,
            [DorotiResizeLiveNative]::MakeScreenPointLParam($resizeStartX, $resizeStartY))
        if (-not $resizeStarted) { throw 'Unable to start the Win32 interactive sizing loop.' }
        Start-Sleep -Milliseconds 150
        $dragStartedAt = [DateTimeOffset]::UtcNow
        $estimatedSteps = [Math]::Max(60, $DurationSeconds * 60)
        $inputSamples = [Collections.Generic.List[object]]::new($estimatedSteps)
        $dragWatch = [Diagnostics.Stopwatch]::StartNew()
        while ($dragWatch.Elapsed -lt [TimeSpan]::FromSeconds($DurationSeconds)) {
            $cycle = ($dragWatch.Elapsed.TotalSeconds % 2.0) / 2.0
            $wave = if ($cycle -lt 0.5) { $cycle * 2 } else { 2 - ($cycle * 2) }
            $horizontalMotion = if ($movesLeft) {
                [Math]::Round(260 * $wave)
            } elseif ($movesRight) {
                -[Math]::Round(260 * $wave)
            } else { 0 }
            $verticalMotion = if ($movesTop) {
                [Math]::Round(140 * $wave)
            } elseif ($movesBottom) {
                -[Math]::Round(140 * $wave)
            } else { 0 }
            $x = $resizeStartX + $horizontalMotion
            $y = $resizeStartY + $verticalMotion
            [DorotiResizeLiveNative]::SetCursorPos($x, $y) | Out-Null
            Start-Sleep -Milliseconds 16
            $sampleRect = [DorotiResizeLiveNative+RECT]::new()
            if ([DorotiResizeLiveNative]::GetWindowRect($hwnd, [ref] $sampleRect)) {
                $inputSamples.Add([pscustomobject]@{
                    performanceCounter = [Diagnostics.Stopwatch]::GetTimestamp()
                    windowLeft = $sampleRect.Left
                    windowTop = $sampleRect.Top
                    windowRight = $sampleRect.Right
                    windowBottom = $sampleRect.Bottom
                    windowWidth = $sampleRect.Right - $sampleRect.Left
                    windowHeight = $sampleRect.Bottom - $sampleRect.Top
                })
            }
        }
        [DorotiResizeLiveNative]::mouse_event([DorotiResizeLiveNative]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    }
    if ($PostDragObservationSeconds -gt 0) {
        Start-Sleep -Seconds $PostDragObservationSeconds
    }
    if ($KeepWindowOpen -and -not $process.HasExited) {
        Write-Host 'Windows resize validation is complete. Close the Demo window after visual inspection to finish evidence collection.'
        $process.WaitForExit()
    }

    if (-not (Test-Path -LiteralPath $rawEvidence -PathType Leaf)) {
        throw "Windows Demo did not write resize evidence: $rawEvidence"
    }
    $diagnostics = Get-Content -LiteralPath $rawEvidence -Raw | ConvertFrom-Json
    $trace = @($diagnostics.surface.resizeTrace)
    $targets = @($trace | Where-Object phase -eq 'target')
    $preSwaps = @($trace | Where-Object phase -eq 'pre-swap')
    $postSwaps = @($trace | Where-Object phase -eq 'post-swap')
    $surfaceReady = @($trace | Where-Object phase -eq 'surface-ready')
    $backingStoreResizeEvents = @($surfaceReady | Where-Object { $_.detail -match '^backingStoreResized=True;' })
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
    $newerTargetKnownAtPrePresent = @($preSwaps | Where-Object {
        if ($_.detail -notmatch 'prePresentTargetGeneration=(\d+); presentedGeneration=(\d+); nativeCommitTargetGeneration=(\d+)') {
            return $true
        }
        return [long]$Matches[1] -ne [long]$Matches[2] -or
            [long]$Matches[3] -ne [long]$Matches[2]
    })
    $targetAdvancedDuringPresent = @($trace | Where-Object {
        $_.phase -eq 'target-advanced-during-present' -and
        $_.source -eq 'targetAdvancedDuringPresent'
    })
    $outsideWorkAreaSamples = @($inputSamples | Where-Object {
        $_.windowLeft -lt $workArea.Left -or $_.windowTop -lt $workArea.Top -or
        $_.windowRight -gt $workArea.Right -or $_.windowBottom -gt $workArea.Bottom
    })
    $finalSample = $inputSamples | Select-Object -Last 1
    $borderToTargetCatchUp = @()
    $borderWidthLagPixels = @()
    $borderHeightLagPixels = @()
    if ($null -ne $finalSample -and $targets.Count -gt 0) {
        $chromeWidth = [int]$finalSample.windowWidth - [int]$diagnostics.surface.pixelWidth
        $chromeHeight = [int]$finalSample.windowHeight - [int]$diagnostics.surface.pixelHeight
        $orderedTargets = @($targets | Sort-Object performanceCounter)
        $orderedInputSamples = @($inputSamples | Sort-Object performanceCounter)
        foreach ($target in $orderedTargets) {
            $latestInput = $orderedInputSamples | Where-Object {
                [long]$_.performanceCounter -le [long]$target.performanceCounter
            } | Select-Object -Last 1
            if ($null -eq $latestInput) { continue }
            $latestExpectedWidth = [int]$latestInput.windowWidth - $chromeWidth
            $latestExpectedHeight = [int]$latestInput.windowHeight - $chromeHeight
            $borderWidthLagPixels += [Math]::Abs($latestExpectedWidth - [int]$target.epoch.physicalWidth)
            $borderHeightLagPixels += [Math]::Abs($latestExpectedHeight - [int]$target.epoch.physicalHeight)
            $matchingInput = $orderedInputSamples | Where-Object {
                [long]$_.performanceCounter -le [long]$target.performanceCounter -and
                ([int]$_.windowWidth - $chromeWidth) -eq [int]$target.epoch.physicalWidth -and
                ([int]$_.windowHeight - $chromeHeight) -eq [int]$target.epoch.physicalHeight
            } | Select-Object -Last 1
            if ($null -ne $matchingInput) {
                $borderToTargetCatchUp += [long][Math]::Round(
                    (([long]$target.performanceCounter - [long]$matchingInput.performanceCounter) * 1000000.0) /
                    [Diagnostics.Stopwatch]::Frequency)
            }
        }
    }
    $refreshIntervalMicroseconds = if ($WindowsGraphicsCapture) {
        1000000.0 / [double]$visualDiagnostics.displayRefreshHz
    } else { $null }
    $ackP50 = Get-Percentile $ackIntervals 0.50
    $ackP95 = Get-Percentile $ackIntervals 0.95
    $ackP99 = Get-Percentile $ackIntervals 0.99
    $missedTwoRefreshIntervals = if ($WindowsGraphicsCapture) {
        @($ackIntervals | Where-Object { $_ -gt (2 * $refreshIntervalMicroseconds) }).Count
    } else { $null }
    $maximumConsecutiveMissedRefresh = 0
    if ($WindowsGraphicsCapture) {
        $consecutiveMissedRefresh = 0
        foreach ($interval in $ackIntervals) {
            if ($interval -gt (2 * $refreshIntervalMicroseconds)) {
                $consecutiveMissedRefresh++
                $maximumConsecutiveMissedRefresh = [Math]::Max(
                    $maximumConsecutiveMissedRefresh,
                    $consecutiveMissedRefresh)
            } else {
                $consecutiveMissedRefresh = 0
            }
        }
    }
    $lastTargetToAckMicroseconds = $null
    if ($targets.Count -gt 0 -and $acks.Count -gt 0) {
        $lastTarget = $targets | Sort-Object performanceCounter | Select-Object -Last 1
        $lastAck = $acks | Where-Object {
            [long]$_.epoch.generation -eq [long]$lastTarget.epoch.generation
        } | Select-Object -Last 1
        if ($null -ne $lastAck) {
            $lastTargetToAckMicroseconds = [long]$lastAck.timestampMicroseconds - [long]$lastTarget.timestampMicroseconds
        }
    }
    $cadenceStatus = if (-not $WindowsGraphicsCapture) {
        'notVerified'
    } elseif ($null -ne $ackP95 -and $null -ne $ackP99 -and $null -ne $lastTargetToAckMicroseconds -and
        $ackP95 -le (2 * $refreshIntervalMicroseconds) -and
        $ackP99 -le (4 * $refreshIntervalMicroseconds) -and
        $lastTargetToAckMicroseconds -le (2 * $refreshIntervalMicroseconds)) {
        'PASS'
    } else {
        'FAIL'
    }
    $exceptionPath = "$rawEvidence.exception.txt"
    $summary = [ordered]@{
        schemaVersion = 'doroti.resize-continuity-live/v5'
        runId = $runId
        capturedAt = [DateTimeOffset]::Now.ToString('o')
        gitCommit = $gitCommit
        sourceFingerprint = $sourceFingerprint
        environment = [ordered]@{
            os = [ordered]@{
                caption = $operatingSystem.Caption
                version = $operatingSystem.Version
                buildNumber = $operatingSystem.BuildNumber
                architecture = $operatingSystem.OSArchitecture
            }
            graphicsAdapters = $graphicsAdapters
            application = [ordered]@{
                path = [IO.Path]::GetRelativePath($repoRoot, $executable).Replace('\', '/')
                sha256 = (Get-FileHash -LiteralPath $executable -Algorithm SHA256).Hash.ToLowerInvariant()
                fileVersion = [Diagnostics.FileVersionInfo]::GetVersionInfo($executable).FileVersion
            }
        }
        swapInterval = $SwapInterval
        edge = $Edge
        durationSeconds = $DurationSeconds
        postDragObservationSeconds = $PostDragObservationSeconds
        keepWindowOpen = [bool]$KeepWindowOpen
        inputMethod = if ($WindowsGraphicsCapture) {
            $visualDiagnostics.inputMethod
        } else {
            "Win32 WM_NCLBUTTONDOWN HT$($Edge.ToUpperInvariant()) interactive resize plus cursor movement"
        }
        inputMotion = 'constant-velocity two-second triangle wave'
        inputHzRequested = if ($WindowsGraphicsCapture) { $visualDiagnostics.inputHzRequested } else { $null }
        inputIntervalMicroseconds = if ($WindowsGraphicsCapture) { $visualDiagnostics.inputIntervalMicroseconds } else { $null }
        inputCursorSamples = $inputSamples.Count
        retainedTraceEntries = $trace.Count
        resizeTargetActivations = $diagnostics.surface.resizeContinuityActivations
        screenRecording = if ($WindowsGraphicsCapture) { 'windows-graphics-capture-png-frame-sequence' } else { 'notVerified' }
        coordinateSampling = [ordered]@{
            dpiAwareness = 'per-monitor-v2'
            windowDpi = $windowDpi
            displayRefreshHz = if ($WindowsGraphicsCapture) { $visualDiagnostics.displayRefreshHz } else { $null }
        }
        visualCapture = if ($WindowsGraphicsCapture) {
            [ordered]@{
                status = 'captured'
                api = $visualDiagnostics.captureApi
                capturedFrames = $visualDiagnostics.capturedFrames
                encodedPngFrames = $visualDiagnostics.encodedPngFrames
                encoderDroppedFrames = $visualDiagnostics.encoderDroppedFrames
                captureErrors = $visualDiagnostics.captureErrors
                captureIntervalMicroseconds = $visualDiagnostics.captureIntervalMicroseconds
                oracle = $visualDiagnostics.visualOracle
                rawEvidence = [IO.Path]::GetRelativePath($repoRoot, $visualEvidence).Replace('\', '/')
                frameSequence = [IO.Path]::GetRelativePath(
                    $repoRoot,
                    (Join-Path $evidenceRoot "$runId.frames")).Replace('\', '/')
            }
        } else {
            [ordered]@{
                status = 'notVerified'
                reason = 'GDI screen capture omits the DXGI composition swap chain; rerun with -WindowsGraphicsCapture.'
            }
        }
        processId = $process.Id
        initialWindow = [ordered]@{
            requestedLogicalWidth = $InitialLogicalWidth
            requestedLogicalHeight = $InitialLogicalHeight
            width = $initialWidth
            height = $initialHeight
        }
        rangeIsolation = 'window border moves inward and returns; the window stays inside the desktop work area'
        outsideWorkAreaSamples = $outsideWorkAreaSamples.Count
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
            p50 = $ackP50
            p95 = $ackP95
            p99 = $ackP99
            over50Milliseconds = $over50MillisecondIntervals.Count
            maxConsecutiveOver50Milliseconds = $maxConsecutiveOver50Milliseconds
        }
        cadence = [ordered]@{
            status = $cadenceStatus
            failureAllowedForMatrixCollection = [bool]$AllowCadenceFailure
            displayRefreshHz = if ($WindowsGraphicsCapture) { $visualDiagnostics.displayRefreshHz } else { $null }
            refreshIntervalMicroseconds = $refreshIntervalMicroseconds
            inputRateHz = [Math]::Round($inputSamples.Count / [double]$DurationSeconds, 3)
            nativeTargetRateHz = [Math]::Round($targets.Count / [double]$DurationSeconds, 3)
            appPresentRateHz = [Math]::Round($acks.Count / [double]$DurationSeconds, 3)
            interPresentRefreshIntervals = if ($WindowsGraphicsCapture) {
                [ordered]@{
                    p50 = [Math]::Round($ackP50 / $refreshIntervalMicroseconds, 3)
                    p95 = [Math]::Round($ackP95 / $refreshIntervalMicroseconds, 3)
                    p99 = [Math]::Round($ackP99 / $refreshIntervalMicroseconds, 3)
                }
            } else { $null }
            intervalsOverTwoRefresh = $missedTwoRefreshIntervals
            maximumConsecutiveIntervalsOverTwoRefresh = if ($WindowsGraphicsCapture) {
                $maximumConsecutiveMissedRefresh
            } else { $null }
            lastTargetToExactCommitMicroseconds = $lastTargetToAckMicroseconds
            lastTargetToExactCommitRefreshIntervals = if ($WindowsGraphicsCapture -and
                $null -ne $lastTargetToAckMicroseconds) {
                [Math]::Round($lastTargetToAckMicroseconds / $refreshIntervalMicroseconds, 3)
            } else { $null }
            borderContentPhase = if ($WindowsGraphicsCapture) {
                [ordered]@{
                    contentGapFrames = $visualDiagnostics.visualOracle.contentEdgeGapFrames
                    maximumConsecutiveFrames = $visualDiagnostics.visualOracle.maximumConsecutiveContentGapFrames
                    maximumDurationMicroseconds = $visualDiagnostics.visualOracle.maximumContentGapDurationMicroseconds
                    maximumDurationRefreshIntervals = [Math]::Round(
                        $visualDiagnostics.visualOracle.maximumContentGapDurationMicroseconds /
                        $refreshIntervalMicroseconds,
                        3)
                    maximumLeftGapPixels = $visualDiagnostics.visualOracle.maximumContentLeftGapPixels
                    maximumRightGapPixels = $visualDiagnostics.visualOracle.maximumContentRightGapPixels
                }
            } else { $null }
            scanOutAcknowledgement = 'Windows Graphics Capture observes composed window frames; app presented ACK is not a hardware scan-out ACK.'
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
            scheduler = @($trace | Where-Object { $_.phase -eq 'ack' -and $_.source -like '*scheduler latest-work gate' }).Count
            prePresentTarget = @($trace | Where-Object { $_.phase -eq 'ack' -and $_.source -eq 'pre-present latest target gate' }).Count
            nativeCommitTarget = @($trace | Where-Object { $_.phase -eq 'ack' -and $_.source -eq 'native final target gate' }).Count
        }
        presentRace = [ordered]@{
            newerTargetKnownAtPrePresent = $newerTargetKnownAtPrePresent.Count
            targetAdvancedDuringPresent = $targetAdvancedDuringPresent.Count
        }
        borderToTarget = [ordered]@{
            sampledWindowRects = $inputSamples.Count
            matchedCatchUps = $borderToTargetCatchUp.Count
            unmatchedTargets = $targets.Count - $borderToTargetCatchUp.Count
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
            backingStoreResizeCount = $backingStoreResizeEvents.Count
            policy = 'exact offscreen raster, fenced ResizeBuffers, GPU-only 1:1 copy, Present(0), resize-only DwmFlush'
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
    $summary | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $summaryEvidence -Encoding utf8
}
finally {
    [DorotiResizeLiveNative]::mouse_event([DorotiResizeLiveNative]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    if (-not $process.HasExited) {
        $process.Refresh()
        $hwnd = $process.MainWindowHandle
        if ($hwnd -ne [IntPtr]::Zero) {
            [DorotiResizeLiveNative]::PostMessage($hwnd, [DorotiResizeLiveNative]::WM_CANCELMODE, [IntPtr]::Zero, [IntPtr]::Zero) | Out-Null
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
$summary | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $summaryEvidence -Encoding utf8
if ($null -ne $summary.frameworkException) {
    throw "Framework exception captured during live resize. See $summaryEvidence"
}
if ($summary.generationRegressions -ne 0 -or
    $summary.presentRace.newerTargetKnownAtPrePresent -ne 0 -or
    $summary.presentRace.targetAdvancedDuringPresent -ne 0 -or
    $summary.latestTargetAtPresentedAck.laggedAckCount -ne 0 -or
    $summary.exactTargetToSwapSizeMismatches -ne 0 -or
    $summary.supersededWork.scheduler -ne 0) {
    throw "Resize correctness gate failed. See $summaryEvidence"
}
if ($summary.outsideWorkAreaSamples -ne 0) {
    throw "Window range isolation failed: $($summary.outsideWorkAreaSamples) samples left the desktop work area. See $summaryEvidence"
}
if ($summary.nativeViewType -ne 'Win32 child HWND hosted by DorotiWindowsDxgiHost' -or
    $summary.graphicsBackend -ne 'Win32/child-HWND/offscreen-copy/DXGI-D3D12-Skia') {
    throw "Native HWND presenter host gate failed. See $summaryEvidence"
}
if ($WindowsGraphicsCapture) {
    $oracle = $summary.visualCapture.oracle
    $inputIntervalLimit = (1000000.0 / [double]$summary.inputHzRequested) * 1.25
    $minimumInputSamples = [Math]::Floor($DurationSeconds * [double]$summary.inputHzRequested * 0.95)
    $edgeTolerance = [Math]::Max(1, [Math]::Ceiling($windowDpi / 96.0))
    if ($summary.inputCursorSamples -lt $minimumInputSamples -or
        [double]$summary.inputIntervalMicroseconds.p95 -gt $inputIntervalLimit) {
        throw "Native input cadence gate failed: samples=$($summary.inputCursorSamples)/$minimumInputSamples p95=$($summary.inputIntervalMicroseconds.p95)us limit=$([Math]::Round($inputIntervalLimit, 3))us. See $summaryEvidence"
    }
    $visualFailure = $summary.visualCapture.captureErrors -ne 0 -or
        $summary.visualCapture.encoderDroppedFrames -ne 0 -or
        $summary.visualCapture.capturedFrames -lt ($DurationSeconds * 10) -or
        $oracle.blankFrames -ne 0 -or
        $oracle.appBarHeightFailures -ne 0 -or
        $oracle.circleObservedFrames -eq 0 -or
        $oracle.circleAspectFailures -ne 0 -or
        $oracle.titleObservedFrames -eq 0 -or
        $oracle.titleNonUniformScaleFailures -ne 0 -or
        $oracle.contentEdgeGapFrames -ne 0 -or
        $oracle.finalContentLeftGapPixels -lt 0 -or
        $oracle.finalContentRightGapPixels -lt 0 -or
        $oracle.finalContentLeftGapPixels -gt $edgeTolerance -or
        $oracle.finalContentRightGapPixels -gt $edgeTolerance
    $summary.visualCapture.status = if ($visualFailure) { 'FAIL' } else { 'PASS' }
    $summary | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $summaryEvidence -Encoding utf8
    if ($visualFailure -and -not $AllowVisualFailure) {
        throw "Windows Graphics Capture visual correctness gate failed. See $summaryEvidence"
    }
    if ($summary.cadence.status -ne 'PASS' -and -not $AllowCadenceFailure) {
        throw "Refresh-normalized cadence gate failed: p95=$($summary.cadence.interPresentRefreshIntervals.p95) refresh, p99=$($summary.cadence.interPresentRefreshIntervals.p99) refresh, final=$($summary.cadence.lastTargetToExactCommitRefreshIntervals) refresh. See $summaryEvidence"
    }
}
if (-not $RetainRawTrace -and (Test-Path -LiteralPath $rawEvidence -PathType Leaf)) {
    Remove-Item -LiteralPath $rawEvidence
}
Write-Output "SUMMARY=$summaryEvidence"
Write-Output ($summary | ConvertTo-Json -Depth 20 -Compress)
