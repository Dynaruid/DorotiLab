#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [string] $Device = $env:DOROTI_WINDOWS_VULKAN_DEVICE,
    [switch] $SkipBuild,
    [switch] $Probe,
    [switch] $FullEightEdgeMatrix,
    [switch] $FullCurrentDpiMatrix,
    [switch] $ExternalValidation,
    [switch] $SaveAnomalyPngs,
    [ValidateSet('Left','Right','Top','Bottom','TopLeft','TopRight','BottomLeft','BottomRight')]
    [string] $FocusedEdge
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$runId = 'windows-vulkan-live-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'),
    ([Guid]::NewGuid().ToString('N').Substring(0, 10))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti/evidence/$runId"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
$globalLockDirectory = Join-Path $repoRoot '.doroti/locks'
New-Item -ItemType Directory -Path $globalLockDirectory -Force | Out-Null
$globalLockPath = Join-Path $globalLockDirectory 'windows-vulkan-validator.lock'
try {
    $globalValidationLock = [IO.FileStream]::new(
        $globalLockPath, [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite,
        [IO.FileShare]::None, 1, [IO.FileOptions]::DeleteOnClose)
} catch [IO.IOException] {
    throw 'Another repository-wide Vulkan validator is already building or running shared artifacts.'
}
try {
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$lockPath = Join-Path $OutputDirectory '.doroti-vulkan-validation.lock'
try {
    $outputDirectoryLock = [IO.FileStream]::new(
        $lockPath, [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite,
        [IO.FileShare]::None, 1, [IO.FileOptions]::DeleteOnClose)
} catch [IO.IOException] {
    throw "Another Vulkan validator already owns output directory '$OutputDirectory'."
}
try {
    $existingOutput = @(Get-ChildItem -LiteralPath $OutputDirectory -Force |
        Where-Object FullName -ne $lockPath)
    if ($existingOutput.Count -ne 0) {
        throw "Vulkan validation output directory must be empty: '$OutputDirectory'."
    }
$productProject = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj'
$productExecutable = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.HwndExactCppProduct.exe'
$productDirectory = Split-Path -Parent $productExecutable
$productAssembly = Join-Path $productDirectory 'Doroti.Validation.HwndExactCppProduct.dll'
$managedHostPath = Join-Path $productDirectory 'Doroti.Host.WindowsAppSdk.dll'
$nativeHostPath = Join-Path $productDirectory 'doroti_windows_appsdk_host_v1.dll'
$observerBuild = Join-Path $repoRoot '.doroti/build/windows-resize-capture-vulkan'
$observer = Join-Path $observerBuild 'Release/Doroti.WindowsResizeCapture.exe'
$deadline = [DateTime]::UtcNow.AddMinutes(20)
$sourceFingerprintPaths = @(
    'Doroti/eng/validate-windows-vulkan-live-resize.ps1',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsAcrylicOptionsState.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedAcrylicCompositionPresenter.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedHwndPresenterBase.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsNativeV1.cs',
    'Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/include/doroti_windows_host_v1.h',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/include/doroti_windows_vulkan_composition_v1.h',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/exports.cpp',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/vulkan_composition.cpp',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/Doroti.Host.WindowsAppSdk.Native.vcxproj',
    'Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj',
    'Doroti/validation/hwnd-exact-cpp-product/Program.cs',
    'Doroti/validation/hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj',
    'Doroti/validation/hwnd-exact-cpp-product/doroti-application-manifest.json',
    'Doroti/validation/windows-resize-capture/CMakeLists.txt',
    'Doroti/validation/windows-resize-capture/main.cpp',
    'Doroti/validation/windows-resize-capture/grid_oracle.h',
    'Doroti/validation/windows-resize-capture/grid_oracle_tests.cpp',
    'Doroti/eng/build-hwnd-exact-cpp-native.ps1',
    'Doroti/Directory.Build.props',
    'Doroti/Directory.Build.targets',
    'Doroti/Directory.Packages.props'
)

if (-not ('Doroti.VulkanLiveValidation.NativeMethods' -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
namespace Doroti.VulkanLiveValidation {
    public static class NativeMethods {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessageW(IntPtr hwnd, uint message, IntPtr wparam, IntPtr lparam);
    }
}
'@
}

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Get-TrackedDiffSha256 {
    $diff = @(& git -C $repoRoot diff --binary --no-ext-diff HEAD -- @sourceFingerprintPaths)
    Assert-True ($LASTEXITCODE -eq 0) 'Could not fingerprint the implementation working-tree diff.'
    $bytes = [Text.Encoding]::UTF8.GetBytes(($diff -join "`n"))
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
}

function Get-RepositoryDiffSha256 {
    $diff = @(& git -C $repoRoot diff --binary --no-ext-diff HEAD)
    Assert-True ($LASTEXITCODE -eq 0) 'Could not fingerprint the repository working-tree diff.'
    $bytes = [Text.Encoding]::UTF8.GetBytes(($diff -join "`n"))
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
}

function Get-UntrackedFileSha256 {
    $hashes = [ordered]@{}
    $relativePaths = @(& git -C $repoRoot ls-files --others --exclude-standard)
    Assert-True ($LASTEXITCODE -eq 0) 'Could not enumerate untracked repository files.'
    foreach ($relativePath in $relativePaths | Sort-Object) {
        $absolutePath = Join-Path $repoRoot $relativePath
        if (Test-Path -LiteralPath $absolutePath -PathType Leaf) {
            $hashes[$relativePath] =
                (Get-FileHash -LiteralPath $absolutePath -Algorithm SHA256).Hash.ToLowerInvariant()
        }
    }
    return $hashes
}

function Get-SourceFileSha256 {
    $hashes = [ordered]@{}
    foreach ($relativePath in $sourceFingerprintPaths) {
        $absolutePath = Join-Path $repoRoot $relativePath
        $hashes[$relativePath] = (Get-FileHash -LiteralPath $absolutePath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    return $hashes
}

function Get-BinaryFileSha256([System.Collections.IDictionary] $Files) {
    $hashes = [ordered]@{}
    foreach ($entry in $Files.GetEnumerator()) {
        Assert-True (Test-Path -LiteralPath $entry.Value -PathType Leaf) "Missing binary: $($entry.Value)"
        $hashes[[string]$entry.Key] =
            (Get-FileHash -LiteralPath $entry.Value -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    return $hashes
}

function Test-Sha256MapsEqual(
    [System.Collections.IDictionary] $Starting,
    [System.Collections.IDictionary] $Ending) {
    return ($Starting | ConvertTo-Json -Compress) -ceq ($Ending | ConvertTo-Json -Compress)
}

function New-StartInfo {
    param(
        [string] $FileName,
        [string[]] $ArgumentList,
        [hashtable] $Environment = @{},
        [switch] $Visible
    )
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.WindowStyle = if ($Visible) {
        [Diagnostics.ProcessWindowStyle]::Normal
    } else {
        [Diagnostics.ProcessWindowStyle]::Hidden
    }
    foreach ($argument in $ArgumentList) { [void]$start.ArgumentList.Add($argument) }
    foreach ($entry in $Environment.GetEnumerator()) {
        $start.Environment[[string]$entry.Key] = [string]$entry.Value
    }
    return $start
}

function Invoke-BoundedProcess {
    param([string] $FileName, [string[]] $ArgumentList, [string] $Name)
    Assert-True ([DateTime]::UtcNow -lt $deadline) 'The Vulkan live-resize validator exceeded 20 minutes.'
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = New-StartInfo $FileName $ArgumentList
    Assert-True $process.Start() "$Name failed to start."
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        $process.Kill($true)
        throw "$Name exceeded the 20-minute timeout."
    }
    $result = [ordered]@{
        exitCode = $process.ExitCode
        stdout = $stdoutTask.GetAwaiter().GetResult()
        stderr = $stderrTask.GetAwaiter().GetResult()
    }
    $process.Dispose()
    return $result
}

function Get-MarkerCorner([string] $Edge) {
    if ($Edge.Contains('Left')) { return 'TopRight' }
    if ($Edge.Contains('Right')) { return 'TopLeft' }
    if ($Edge.Contains('Top')) { return 'BottomRight' }
    return 'TopRight'
}

function Request-AppClose([long] $Hwnd) {
    [void][Doroti.VulkanLiveValidation.NativeMethods]::PostMessageW(
        [IntPtr]$Hwnd, 0x0010, [IntPtr]::Zero, [IntPtr]::Zero)
}

function Get-Percentile([object[]] $Values, [double] $Quantile) {
    $numbers = @($Values | Where-Object { $null -ne $_ } |
        ForEach-Object { [double]$_ } | Sort-Object)
    if ($numbers.Count -eq 0) { return $null }
    $index = [Math]::Min(
        $numbers.Count - 1,
        [Math]::Max(0, [Math]::Ceiling($Quantile * $numbers.Count) - 1))
    return [Math]::Round($numbers[$index], 1)
}

function Get-ResizeCadence($Capture, $Details) {
    $frequency = [double]$Capture.clockCalibration.qpcFrequency
    $samples = @($Capture.windowSamples)
    $changeCounters = [Collections.Generic.List[long]]::new()
    for ($index = 1; $index -lt $samples.Count; $index++) {
        $previous = $samples[$index - 1].window
        $current = $samples[$index].window
        if ([int]$previous.left -ne [int]$current.left -or
            [int]$previous.top -ne [int]$current.top -or
            [int]$previous.right -ne [int]$current.right -or
            [int]$previous.bottom -ne [int]$current.bottom) {
            $changeCounters.Add([long]$samples[$index].performanceCounter)
        }
    }

    if ($frequency -le 0 -or $changeCounters.Count -lt 2) {
        return [ordered]@{
            measured=$false
            source='resize-receipt-terminal-qpc-within-actual-window-motion'
            qpcFrequency=[long]$frequency
            outerRectChanges=$changeCounters.Count
            platformWaitTimeoutCount=[int]$Details.resize.platformWaitTimeoutCount
        }
    }

    $motionStart = $changeCounters[0]
    $motionEnd = $changeCounters[$changeCounters.Count - 1]
    $durationMicroseconds = ($motionEnd - $motionStart) * 1000000.0 / $frequency
    $receipts = @($Details.resize.receipts)
    $activeReceipts = @($receipts | Where-Object {
        [long]$_.target.acceptedTimestamp -ge $motionStart -and
        [long]$_.target.acceptedTimestamp -le $motionEnd
    })
    $presentedReceipts = @($receipts | Where-Object { [int]$_.terminal -eq 0 } |
        Sort-Object { [long]$_.terminalTimestamp })
    $presentedDuringMotion = @($presentedReceipts | Where-Object {
        [long]$_.terminalTimestamp -ge $motionStart -and
        [long]$_.terminalTimestamp -le $motionEnd
    })

    $presentationCounters = @($presentedDuringMotion |
        ForEach-Object { [long]$_.terminalTimestamp })
    $boundaries = @($motionStart) + $presentationCounters + @($motionEnd)
    $gaps = [Collections.Generic.List[double]]::new()
    for ($index = 1; $index -lt $boundaries.Count; $index++) {
        $gaps.Add(($boundaries[$index] - $boundaries[$index - 1]) * 1000000.0 / $frequency)
    }

    $acceptedToNext = [Collections.Generic.List[double]]::new()
    foreach ($receipt in $activeReceipts) {
        $accepted = [long]$receipt.target.acceptedTimestamp
        $next = @($presentedReceipts | Where-Object {
            [long]$_.terminalTimestamp -ge $accepted -and
            [long]$_.terminalTimestamp -le $motionEnd
        } | Select-Object -First 1)
        $terminal = if ($next.Count) { [long]$next[0].terminalTimestamp } else { $motionEnd }
        $acceptedToNext.Add([Math]::Max(0, ($terminal - $accepted) * 1000000.0 / $frequency))
    }

    $presentedLatencies = @($presentedDuringMotion | ForEach-Object {
        ([long]$_.terminalTimestamp - [long]$_.target.acceptedTimestamp) * 1000000.0 / $frequency
    })
    $gapMaximum = if ($gaps.Count) { ($gaps | Measure-Object -Maximum).Maximum } else { $null }
    $nextMaximum = if ($acceptedToNext.Count) {
        ($acceptedToNext | Measure-Object -Maximum).Maximum
    } else { $null }
    $latencyMaximum = if ($presentedLatencies.Count) {
        ($presentedLatencies | Measure-Object -Maximum).Maximum
    } else { $null }

    return [ordered]@{
        measured=$true
        source='resize-receipt-terminal-qpc-within-actual-window-motion'
        qpcFrequency=[long]$frequency
        motionStartQpc=$motionStart
        motionEndQpc=$motionEnd
        motionDurationMicroseconds=[Math]::Round($durationMicroseconds, 1)
        outerRectChanges=$changeCounters.Count
        acceptedTargetsDuringMotion=$activeReceipts.Count
        presentedTerminalsDuringMotion=$presentedDuringMotion.Count
        supersededTargetsDuringMotion=@($activeReceipts | Where-Object { [int]$_.terminal -eq 1 }).Count
        presentationRateHz=[Math]::Round(
            $presentedDuringMotion.Count * 1000000.0 / $durationMicroseconds, 2)
        presentationGapP95Microseconds=Get-Percentile $gaps 0.95
        presentationGapMaxMicroseconds=if ($null -eq $gapMaximum) { $null } else {
            [Math]::Round([double]$gapMaximum, 1)
        }
        acceptedToNextPresentationP95Microseconds=Get-Percentile $acceptedToNext 0.95
        acceptedToNextPresentationMaxMicroseconds=if ($null -eq $nextMaximum) { $null } else {
            [Math]::Round([double]$nextMaximum, 1)
        }
        presentedTargetLatencyP95Microseconds=Get-Percentile $presentedLatencies 0.95
        presentedTargetLatencyMaxMicroseconds=if ($null -eq $latencyMaximum) { $null } else {
            [Math]::Round([double]$latencyMaximum, 1)
        }
        platformWaitTimeoutCount=[int]$Details.resize.platformWaitTimeoutCount
    }
}

function Invoke-CapturedCase {
    param(
        [string] $Presenter,
        [string] $Slug,
        [string] $Edge,
        [string] $Motion,
        [int] $DragMilliseconds,
        [int] $DragPixels = 600
    )
    Assert-True ([DateTime]::UtcNow -lt $deadline) 'The Vulkan live-resize validator exceeded 20 minutes.'
    $readyPath = Join-Path $OutputDirectory "$Slug.ready.json"
    $appPath = Join-Path $OutputDirectory "$Slug.app.json"
    $detailsPath = Join-Path $OutputDirectory "$Slug.details.json"
    $capturePath = Join-Path $OutputDirectory "$Slug.capture.json"
    $process = [Diagnostics.Process]::new()
    $validationLayerRequested = $Presenter -eq 'Vulkan' -and $ExternalValidation
    $caseEnvironment = @{
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=$readyPath
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FRAME_MARKER='1'
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FRAME_MARKER_CORNER=(Get-MarkerCorner $Edge)
        DOROTI_WINDOWS_APPSDK_REPORT=$detailsPath
        DOROTI_WINDOWS_DWM_FLUSH='0'
        DOROTI_WINDOWS_VULKAN_DEVICE=$Device
        VK_INSTANCE_LAYERS=''
        VK_LAYER_VALIDATE_SYNC=''
        VK_LOADER_DEBUG=''
    }
    if ($validationLayerRequested) {
        $caseEnvironment.VK_INSTANCE_LAYERS = 'VK_LAYER_KHRONOS_validation'
        $caseEnvironment.VK_LAYER_VALIDATE_SYNC = '1'
        $caseEnvironment.VK_LOADER_DEBUG = 'layer'
    }
    $process.StartInfo = New-StartInfo -FileName $productExecutable -ArgumentList @(
        '--presenter',$Presenter,'--lifecycle-cycles','0','--external-resize',
        '--smoke-ms','15000','--report',$appPath
    ) -Environment $caseEnvironment -Visible
    Assert-True $process.Start() "$Slug product failed to start."
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    try {
        $readyDeadline = [DateTime]::UtcNow.AddSeconds(12)
        while (-not (Test-Path -LiteralPath $readyPath -PathType Leaf)) {
            if ($process.HasExited) {
                throw "$Slug exited before ready: $($stderrTask.GetAwaiter().GetResult())"
            }
            if ([DateTime]::UtcNow -ge $readyDeadline) { throw "$Slug ready timeout." }
            Start-Sleep -Milliseconds 50
        }
        $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
        $captureArguments = @(
            '--hwnd',[string]$ready.hwnd,'--output',$capturePath,'--run-id',"$runId-$Slug",
            '--decode-frame-id','--f6r','--input-hz','240','--png-stride','60',
            '--capture-ring-size','64','--edge',$Edge,'--drag-pixels',[string]$DragPixels,
            '--drag-ms',[string]$DragMilliseconds,'--motion',$Motion,'--duration','1')
        if ($SaveAnomalyPngs) { $captureArguments += '--gap-anomaly-only' }
        else { $captureArguments += '--capture-only' }
        $captureRun = Invoke-BoundedProcess -FileName $observer -Name "$Slug WGC border resize" `
            -ArgumentList $captureArguments
        Assert-True ($captureRun.exitCode -eq 0) `
            "$Slug WGC border resize failed: $($captureRun.stderr) $($captureRun.stdout)"
        Request-AppClose ([long]$ready.hwnd)
        if (-not $process.WaitForExit(30000)) { throw "$Slug did not close in 30 seconds." }
        $stdout = $stdoutTask.GetAwaiter().GetResult()
        $stderr = $stderrTask.GetAwaiter().GetResult()
        $app = Get-Content -LiteralPath $appPath -Raw | ConvertFrom-Json -Depth 100
        $details = Get-Content -LiteralPath $detailsPath -Raw | ConvertFrom-Json -Depth 100
        $capture = Get-Content -LiteralPath $capturePath -Raw | ConvertFrom-Json -Depth 100
        $frames = @($capture.frames)
        $decodedIds = @($frames | Where-Object { $null -ne $_.frameId } | ForEach-Object { [long]$_.frameId })
        $markerRegressions = 0
        for ($index = 1; $index -lt $decodedIds.Count; $index++) {
            if ($decodedIds[$index] -lt $decodedIds[$index - 1]) { $markerRegressions++ }
        }
        $lastFrame = if ($frames.Count -eq 0) { $null } else { $frames[-1] }
        $finalWidth = [int]$capture.finalGeometry.clientScreen.width
        $finalHeight = [int]$capture.finalGeometry.clientScreen.height
        $finalExact = $null -ne $lastFrame -and [int]$lastFrame.client.width -eq $finalWidth -and
            [int]$lastFrame.client.height -eq $finalHeight
        $transportPass = $captureRun.exitCode -eq 0 -and [int]$capture.capturedFrames -gt 0 -and
            [int]$capture.captureErrors -eq 0 -and [int]$capture.captureRingDroppedFrames -eq 0 -and
            [int]$capture.poolCapacityExceededFrames -eq 0 -and
            @($frames | Where-Object blank).Count -eq 0
        $markerPass = $decodedIds.Count -gt 0 -and $markerRegressions -eq 0
        $pixelCoveragePass = [int]$capture.visualOracle.validationBackgroundObservedFrames -gt 0 -and
            [int]$capture.visualOracle.validationBackgroundRightGapFrames -eq 0
        $diagnostics = $app.diagnostics
        $combinedOutput = "$stdout`n$stderr"
        $validationMessageCount = [regex]::Matches(
            $combinedOutput, '(?im)^\s*Validation (?:Error|Warning):').Count
        $validationMessagesClean = $validationMessageCount -eq 0
        $validationLayerActivationProven = $validationLayerRequested -and
            $combinedOutput -match '(?im)Inserted (?:instance|device) layer "VK_LAYER_KHRONOS_validation"' -and
            $combinedOutput -match '(?im)Enabled By:\s*Environment Variable VK_INSTANCE_LAYERS'
        $originMovingEdge = $Edge -in @('Left','Top','TopLeft','TopRight','BottomLeft')
        if ($Presenter -ne 'Vulkan') {
            $compositionFrameContract = $true
        } elseif ($originMovingEdge) {
            $compositionFrameContract =
                [long]$diagnostics.vulkan.compositionFrameWaits -eq 0 -and
                [long]$diagnostics.vulkan.compositionFrameObserved -eq 0 -and
                [long]$diagnostics.vulkan.compositionFrameWaitTimeouts -eq 0
        } else {
            $compositionFrameContract =
                [long]$diagnostics.vulkan.compositionFrameWaits -gt 0 -and
                [long]$diagnostics.vulkan.compositionFrameWaits -eq
                    [long]$diagnostics.vulkan.compositionFrameObserved -and
                [long]$diagnostics.vulkan.compositionFrameWaitTimeouts -eq 0
        }
        $resourcePass = $process.ExitCode -eq 0 -and $app.status -eq 'PASS' -and
            $validationMessagesClean -and
            (-not $validationLayerRequested -or $validationLayerActivationProven) -and
            [int]$diagnostics.operationalDebugErrors -eq 0 -and
            [int]$diagnostics.failedTerminals -eq 0 -and
            [int]$diagnostics.unterminatedResizeGenerations -eq 0 -and
            [int]$diagnostics.duplicateResizeTerminals -eq 0 -and
            ($Presenter -ne 'Vulkan' -or (
                $diagnostics.presenterBackend -eq 'Vulkan/Composition-Swapchain' -and
                $details.ownership.visibleOwner -eq 'top-level DirectComposition Vulkan Presentation target' -and
                [int]$diagnostics.vulkan.deviceLostResults -eq 0 -and
                [int]$diagnostics.vulkan.surfaceLostResults -eq 0 -and
                [int]$diagnostics.vulkan.outOfDateResults -eq 0 -and
                [int]$diagnostics.vulkan.suboptimalResults -eq 0 -and
                [int]$diagnostics.vulkan.outstandingAcquired -eq 0 -and
                [int]$diagnostics.vulkan.outstandingCopySubmission -eq 0 -and
                [int]$diagnostics.vulkan.maximumOutstandingAcquired -le 1 -and
                [long]$diagnostics.vulkan.acquired -eq [long]$diagnostics.vulkan.presented -and
                [long]$diagnostics.vulkan.presented -eq [long]$diagnostics.vulkan.successfulPresents -and
                [long]$diagnostics.vulkan.successfulPresents -eq [long]$diagnostics.presents -and
                [long]$diagnostics.vulkan.presented -le [long]$diagnostics.gpuCopies -and
                [long]$diagnostics.vulkan.deferredCopySubmissions -eq 0 -and
                [long]$diagnostics.vulkan.copyFenceWaits -eq [long]$diagnostics.gpuCopies -and
                [long]$diagnostics.vulkan.retainedFrameAllocationBytes -gt 0 -and
                [bool]$diagnostics.vulkan.retainedFrameInitialized -and
                $diagnostics.vulkan.presentMode -eq 'CompositionSwapchain' -and
                [int]$diagnostics.vulkan.imageCount -eq 3 -and
                [int]$diagnostics.vulkan.surfaceWidth -ge [int]$diagnostics.vulkan.width -and
                [int]$diagnostics.vulkan.surfaceHeight -ge [int]$diagnostics.vulkan.height -and
                [long]$diagnostics.vulkan.surfaceRecreates -eq 0 -and
                [int]$diagnostics.vulkan.activeSwapchains -eq 0 -and
                [int]$diagnostics.vulkan.retiredSwapchains -eq 0 -and
                [int]$diagnostics.vulkan.maximumRetiredSwapchains -eq 0 -and
                [long]$diagnostics.vulkan.queueIdleRetirementWaits -eq 0 -and
                $compositionFrameContract -and
                $diagnostics.vulkan.retirementMode -eq 'presentation-buffer-availability'))
        $status = if ($transportPass -and $markerPass -and $pixelCoveragePass -and
            $finalExact -and $resourcePass) { 'PASS' } else { 'FAIL' }
        return [ordered]@{
            name=$Slug; status=$status; presenter=$Presenter
            definition=[ordered]@{ edge=$Edge; motion=$Motion; dragMilliseconds=$DragMilliseconds; dragPixels=$DragPixels }
            transport=$transportPass; marker=$markerPass; pixelCoverage=$pixelCoveragePass
            finalExact=$finalExact; resource=$resourcePass
            validationMessagesClean=$validationMessagesClean; validationMessageCount=$validationMessageCount
            validationLayerRequested=$validationLayerRequested
            validationLayerActivationProven=if ($validationLayerRequested) { $validationLayerActivationProven } else { $null }
            capturedFrames=[int]$capture.capturedFrames; inputSamples=[int]$capture.inputSamples
            displayRefreshHz=[double]$capture.displayRefreshHz
            decodedFrames=$decodedIds.Count; markerRegressions=$markerRegressions
            validationBackgroundObservedFrames=[int]$capture.visualOracle.validationBackgroundObservedFrames
            validationBackgroundRightGapFrames=[int]$capture.visualOracle.validationBackgroundRightGapFrames
            maximumValidationBackgroundRightGapPixels=[int]$capture.visualOracle.maximumValidationBackgroundRightGapPixels
            frameIdFirst=if ($decodedIds.Count) { $decodedIds[0] } else { $null }
            frameIdLast=if ($decodedIds.Count) { $decodedIds[-1] } else { $null }
            captureIntervalP95Microseconds=$capture.captureIntervalMicroseconds.p95
            cadence=Get-ResizeCadence $capture $details
            vulkan=if ($Presenter -eq 'Vulkan') { [ordered]@{
                visibleOwner=$details.ownership.visibleOwner
                presentationMode=$diagnostics.vulkan.presentMode
                selectedAvailableBuffers=$diagnostics.vulkan.acquired; presented=$diagnostics.vulkan.presented
                copiedButSupersededBeforePresent=([long]$diagnostics.gpuCopies - [long]$diagnostics.vulkan.presented)
                outstanding=$diagnostics.vulkan.outstandingAcquired
                outstandingCopySubmission=$diagnostics.vulkan.outstandingCopySubmission
                maximumOutstanding=$diagnostics.vulkan.maximumOutstandingAcquired
                deferredCopySubmissions=$diagnostics.vulkan.deferredCopySubmissions
                copyFenceWaits=$diagnostics.vulkan.copyFenceWaits
                maximumCopyFenceWaitMicroseconds=$diagnostics.vulkan.maximumCopyFenceWaitMicroseconds
                backingAllocationBytes=$diagnostics.vulkan.backingAllocationBytes
                retainedFrameAllocationBytes=$diagnostics.vulkan.retainedFrameAllocationBytes
                retainedFrameInitialized=$diagnostics.vulkan.retainedFrameInitialized
                backingAllocations=$diagnostics.vulkan.backingAllocations
                backingReuses=$diagnostics.vulkan.backingReuses
                viewportWidth=$diagnostics.vulkan.width; viewportHeight=$diagnostics.vulkan.height
                compositionBufferWidth=$diagnostics.vulkan.surfaceWidth
                compositionBufferHeight=$diagnostics.vulkan.surfaceHeight
                compositionBufferCount=$diagnostics.vulkan.imageCount
                compositionBufferReuses=$diagnostics.vulkan.retainedSurfaceReuses
                legacySurfaceRecreates=$diagnostics.vulkan.surfaceRecreates
                deviceLost=$diagnostics.vulkan.deviceLostResults; surfaceLost=$diagnostics.vulkan.surfaceLostResults
                outOfDate=$diagnostics.vulkan.outOfDateResults; suboptimal=$diagnostics.vulkan.suboptimalResults
                activeSwapchains=$diagnostics.vulkan.activeSwapchains; retiredSwapchains=$diagnostics.vulkan.retiredSwapchains
                retirementMode=$diagnostics.vulkan.retirementMode
                queueIdleRetirementWaits=$diagnostics.vulkan.queueIdleRetirementWaits
                compositionFrameWaits=$diagnostics.vulkan.compositionFrameWaits
                compositionFrameObserved=$diagnostics.vulkan.compositionFrameObserved
                compositionFrameWaitTimeouts=$diagnostics.vulkan.compositionFrameWaitTimeouts
                resizeOrdering='top-level-current-plus-latest-moving-origin'
                maximumRecreateLatencyMicroseconds=$diagnostics.vulkan.maximumRecreateLatencyMicroseconds
                maximumSwapchainCreateLatencyMicroseconds=$diagnostics.vulkan.maximumSwapchainCreateLatencyMicroseconds
            } } else { $null }
            appPath=$appPath; capturePath=$capturePath
            detailsPath=$detailsPath
            stdout=$stdout.Trim(); stderr=$stderr.Trim()
        }
    }
    finally {
        if (-not $process.HasExited) {
            if (Test-Path -LiteralPath $readyPath) {
                $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
                Request-AppClose ([long]$ready.hwnd)
                [void]$process.WaitForExit(3000)
            }
            if (-not $process.HasExited) { $process.Kill($true) }
        }
        $process.Dispose()
    }
}

$startingStatus = @(& git -C $repoRoot status --short)
$head = (& git -C $repoRoot rev-parse HEAD).Trim()
$startingDiffSha256 = Get-TrackedDiffSha256
$startingSourceFileSha256 = Get-SourceFileSha256
$startingRepositoryDiffSha256 = Get-RepositoryDiffSha256
$startingUntrackedFileSha256 = Get-UntrackedFileSha256
if (-not $SkipBuild) {
    $native = Invoke-BoundedProcess -FileName 'pwsh' -Name 'native Release build' -ArgumentList @(
        '-NoProfile','-File',(Join-Path $repoRoot 'Doroti/eng/build-hwnd-exact-cpp-native.ps1'))
    Assert-True ($native.exitCode -eq 0) "Native build failed: $($native.stderr)"
    $product = Invoke-BoundedProcess -FileName 'dotnet' -Name 'product Release build' -ArgumentList @(
        'build',$productProject,'-c','Release','--nologo')
    Assert-True ($product.exitCode -eq 0) "Product build failed: $($product.stderr)"
    $configure = Invoke-BoundedProcess -FileName 'cmake' -Name 'WGC observer configure' -ArgumentList @(
        '-S',(Join-Path $repoRoot 'Doroti/validation/windows-resize-capture'),'-B',$observerBuild,'-A','x64')
    Assert-True ($configure.exitCode -eq 0) "Observer configure failed: $($configure.stderr)"
    $observerResult = Invoke-BoundedProcess -FileName 'cmake' -Name 'WGC observer build' -ArgumentList @(
        '--build',$observerBuild,'--config','Release')
    Assert-True ($observerResult.exitCode -eq 0) "Observer build failed: $($observerResult.stderr)"
}
Assert-True (Test-Path -LiteralPath $productExecutable -PathType Leaf) "Missing product executable: $productExecutable"
Assert-True (Test-Path -LiteralPath $observer -PathType Leaf) "Missing WGC observer: $observer"
$binaryFiles = [ordered]@{
    productExecutable=$productExecutable
    productAssembly=$productAssembly
    managedHost=$managedHostPath
    nativeHost=$nativeHostPath
    resizeObserver=$observer
}
$startingBinarySha256 = Get-BinaryFileSha256 $binaryFiles

$definitions = if (-not [string]::IsNullOrWhiteSpace($FocusedEdge)) {
    @([ordered]@{ run=1; edge=$FocusedEdge; motion='reverse'; duration=600 })
} elseif ($Probe) {
    @([ordered]@{ run=1; edge='TopLeft'; motion='reverse'; duration=600 })
} elseif ($FullCurrentDpiMatrix) {
    @(
        foreach ($run in 1..3) {
            foreach ($edge in @('Left','Right','Top','Bottom','TopLeft','TopRight','BottomLeft','BottomRight')) {
                foreach ($motion in @('expand','shrink','reverse')) {
                    foreach ($duration in @(1200,600,150)) {
                        [ordered]@{ run=$run; edge=$edge; motion=$motion; duration=$duration }
                    }
                }
            }
        }
    )
} elseif ($FullEightEdgeMatrix) {
    @(
        foreach ($run in 1..3) {
            foreach ($edge in @('Left','Right','Top','Bottom','TopLeft','TopRight','BottomLeft','BottomRight')) {
                [ordered]@{ run=$run; edge=$edge; motion='reverse'; duration=600 }
            }
        }
    )
} else {
    @(
        [ordered]@{ run=1; edge='Left'; motion='reverse'; duration=600 }
        [ordered]@{ run=1; edge='TopLeft'; motion='reverse'; duration=600 }
    )
}

$runAngleBaselines = [bool]$FullEightEdgeMatrix -or [bool]$FullCurrentDpiMatrix
$angleBefore = $null
$angleAfter = $null
if ($runAngleBaselines) {
    $angleBefore = Invoke-CapturedCase -Presenter 'AngleD3D11' -Slug 'angle-before' `
        -Edge 'Left' -Motion 'reverse' -DragMilliseconds 600
}
$cases = @()
$caseIndex = 0
$definitionCount = @($definitions).Count
foreach ($definition in $definitions) {
    $caseIndex++
    $slug = 'vulkan-{0:D3}-r{1}-{2}-{3}-{4}' -f $caseIndex,$definition.run,
        $definition.edge,$definition.motion,$definition.duration
    Write-Host "[$caseIndex/$definitionCount] $slug"
    $cases += Invoke-CapturedCase -Presenter 'Vulkan' -Slug $slug -Edge $definition.edge `
        -Motion $definition.motion -DragMilliseconds $definition.duration
}
if ($runAngleBaselines) {
    $angleAfter = Invoke-CapturedCase -Presenter 'AngleD3D11' -Slug 'angle-after' `
        -Edge 'Left' -Motion 'reverse' -DragMilliseconds 600
}

$baselineCadenceValid = $runAngleBaselines -and
    $angleBefore.status -eq 'PASS' -and $angleAfter.status -eq 'PASS' -and
    $angleBefore.cadence.measured -and $angleAfter.cadence.measured -and
    [int]$angleBefore.cadence.outerRectChanges -ge 8 -and
    [int]$angleAfter.cadence.outerRectChanges -ge 8 -and
    [int]$angleBefore.cadence.presentedTerminalsDuringMotion -ge 8 -and
    [int]$angleAfter.cadence.presentedTerminalsDuringMotion -ge 8
$referenceRate = if ($baselineCadenceValid) {
    [Math]::Min(
        [double]$angleBefore.cadence.presentationRateHz,
        [double]$angleAfter.cadence.presentationRateHz)
} else { $null }
$referenceGapP95 = if ($baselineCadenceValid) {
    [Math]::Max(
        [double]$angleBefore.cadence.presentationGapP95Microseconds,
        [double]$angleAfter.cadence.presentationGapP95Microseconds)
} else { $null }
$referenceGapMax = if ($baselineCadenceValid) {
    [Math]::Max(
        [double]$angleBefore.cadence.presentationGapMaxMicroseconds,
        [double]$angleAfter.cadence.presentationGapMaxMicroseconds)
} else { $null }
$referenceLatencyP95 = if ($baselineCadenceValid) {
    [Math]::Max(
        [double]$angleBefore.cadence.presentedTargetLatencyP95Microseconds,
        [double]$angleAfter.cadence.presentedTargetLatencyP95Microseconds)
} else { $null }
$referenceLatencyMax = if ($baselineCadenceValid) {
    [Math]::Max(
        [double]$angleBefore.cadence.presentedTargetLatencyMaxMicroseconds,
        [double]$angleAfter.cadence.presentedTargetLatencyMaxMicroseconds)
} else { $null }
$referenceTimeouts = if ($baselineCadenceValid) {
    [Math]::Max(
        [int]$angleBefore.cadence.platformWaitTimeoutCount,
        [int]$angleAfter.cadence.platformWaitTimeoutCount)
} else { $null }

$matchedCadenceCases = 0
foreach ($case in $cases) {
    $matched = $case.definition.motion -eq 'reverse' -and
        [int]$case.definition.dragMilliseconds -eq 600 -and
        [int]$case.definition.dragPixels -eq 600
    if (-not $matched) {
        $case['cadenceComparison'] = [ordered]@{ status='notRun'; reason='no matched ANGLE duration/motion baseline' }
        continue
    }
    if (-not $runAngleBaselines) {
        $case['cadenceComparison'] = [ordered]@{
            status='notRun'
            reason='focused validation omits ANGLE baselines; use -FullEightEdgeMatrix for cadence qualification'
        }
        continue
    }

    $matchedCadenceCases++
    $cadencePass = $baselineCadenceValid -and $case.cadence.measured -and
        [int]$case.cadence.outerRectChanges -ge 8 -and
        [int]$case.cadence.presentedTerminalsDuringMotion -ge 3 -and
        [double]$case.cadence.presentationRateHz -ge (0.50 * $referenceRate) -and
        [double]$case.cadence.presentationGapP95Microseconds -le (2.0 * $referenceGapP95) -and
        [double]$case.cadence.presentationGapMaxMicroseconds -le (4.0 * $referenceGapMax) -and
        [int]$case.cadence.platformWaitTimeoutCount -le $referenceTimeouts
    $refreshIntervalMicroseconds = if ([double]$case.displayRefreshHz -gt 0) {
        1000000.0 / [double]$case.displayRefreshHz
    } else { [double]::PositiveInfinity }
    $targetDeltaPass = $baselineCadenceValid -and $case.cadence.measured -and
        [double]$case.cadence.presentedTargetLatencyP95Microseconds -le
            ($referenceLatencyP95 + $refreshIntervalMicroseconds) -and
        [double]$case.cadence.presentedTargetLatencyMaxMicroseconds -le
            ($referenceLatencyMax + 2.0 * $refreshIntervalMicroseconds)
    $comparisonPass = $cadencePass -and $targetDeltaPass
    $case['cadenceComparison'] = [ordered]@{
        status=if ($comparisonPass) { 'PASS' } else { 'FAIL' }
        starvationGuard=if ($cadencePass) { 'PASS' } else { 'FAIL' }
        targetToPresentAngleDelta=if ($targetDeltaPass) { 'PASS' } else { 'FAIL' }
        reference=[ordered]@{
            presentationRateHz=$referenceRate
            presentationGapP95Microseconds=$referenceGapP95
            presentationGapMaxMicroseconds=$referenceGapMax
            presentedTargetLatencyP95Microseconds=$referenceLatencyP95
            presentedTargetLatencyMaxMicroseconds=$referenceLatencyMax
            platformWaitTimeoutCount=$referenceTimeouts
        }
        allowances=[ordered]@{
            minimumPresentationRateHz=[Math]::Round(0.50 * $referenceRate, 2)
            maximumPresentationGapP95Microseconds=[Math]::Round(2.0 * $referenceGapP95, 1)
            maximumPresentationGapMicroseconds=[Math]::Round(4.0 * $referenceGapMax, 1)
            refreshIntervalMicroseconds=[Math]::Round($refreshIntervalMicroseconds, 1)
            maximumTargetLatencyP95Microseconds=[Math]::Round(
                $referenceLatencyP95 + $refreshIntervalMicroseconds, 1)
            maximumTargetLatencyMicroseconds=[Math]::Round(
                $referenceLatencyMax + 2.0 * $refreshIntervalMicroseconds, 1)
        }
    }
    if (-not $comparisonPass) { $case.status = 'FAIL' }
}

$videoControllers = @(Get-CimInstance Win32_VideoController |
    Select-Object Name,DriverVersion,CurrentRefreshRate,VideoModeDescription,PNPDeviceID)
$casePass = @($cases | Where-Object status -ne 'PASS').Count -eq 0
$anglePass = -not $runAngleBaselines -or
    ($angleBefore.status -eq 'PASS' -and $angleAfter.status -eq 'PASS')
$cadencePass = $runAngleBaselines -and $matchedCadenceCases -gt 0 -and
    @($cases | Where-Object {
        $_.cadenceComparison.status -ne 'notRun' -and $_.cadenceComparison.starvationGuard -ne 'PASS'
    }).Count -eq 0
$targetDeltaPass = $runAngleBaselines -and $matchedCadenceCases -gt 0 -and
    @($cases | Where-Object {
        $_.cadenceComparison.status -ne 'notRun' -and $_.cadenceComparison.targetToPresentAngleDelta -ne 'PASS'
    }).Count -eq 0
$reverse600Cases = @($cases | Where-Object {
    $_.definition.motion -eq 'reverse' -and
    [int]$_.definition.dragMilliseconds -eq 600 -and
    [int]$_.definition.dragPixels -eq 600
})
$expectedReverse600Cases = if ($Probe) { 1 } elseif ($runAngleBaselines) { 24 } else { 2 }
$reverse600Qualified = $reverse600Cases.Count -eq $expectedReverse600Cases -and
    @($reverse600Cases | Where-Object { $_.status -ne 'PASS' }).Count -eq 0 -and
    (-not $runAngleBaselines -or
        @($reverse600Cases | Where-Object { $_.cadenceComparison.status -ne 'PASS' }).Count -eq 0)
$endingDiffSha256 = Get-TrackedDiffSha256
$endingHead = (& git -C $repoRoot rev-parse HEAD).Trim()
$endingSourceFileSha256 = Get-SourceFileSha256
$keySourceFilesStable = Test-Sha256MapsEqual $startingSourceFileSha256 $endingSourceFileSha256
$sourceStable = $head -eq $endingHead -and
    $startingDiffSha256 -eq $endingDiffSha256 -and $keySourceFilesStable
$endingRepositoryDiffSha256 = Get-RepositoryDiffSha256
$endingUntrackedFileSha256 = Get-UntrackedFileSha256
$repositoryStable = $head -eq $endingHead -and
    $startingRepositoryDiffSha256 -eq $endingRepositoryDiffSha256 -and
    (Test-Sha256MapsEqual $startingUntrackedFileSha256 $endingUntrackedFileSha256)
$endingBinarySha256 = Get-BinaryFileSha256 $binaryFiles
$binariesStable = Test-Sha256MapsEqual $startingBinarySha256 $endingBinarySha256
$externalValidationPass = -not $ExternalValidation -or
    @($cases | Where-Object {
        -not $_.validationLayerRequested -or
        -not $_.validationLayerActivationProven -or
        -not $_.validationMessagesClean
    }).Count -eq 0
$sequence = @($cases)
if ($runAngleBaselines) { $sequence = @($angleBefore) + @($cases) + @($angleAfter) }
$manifest = [ordered]@{
    schemaVersion='doroti.windows.vulkan-composition-live-resize/v1'
    runId=$runId
    generatedAt=[DateTime]::UtcNow.ToString('O')
    status=if ($casePass -and $anglePass -and $sourceStable -and $repositoryStable -and $binariesStable) { 'PASS-automated-partial' } else { 'FAIL' }
    source=[ordered]@{
        revision=$head
        endingRevision=$endingHead
        statusAtStart=$startingStatus
        implementationDiffSha256=$startingDiffSha256
        sourceStableDuringRun=$sourceStable
        endingImplementationDiffSha256=$endingDiffSha256
        endingFileSha256=$endingSourceFileSha256
        keySourceFilesStableAtEndpoints=$keySourceFilesStable
        repositoryDiffSha256=$startingRepositoryDiffSha256
        endingRepositoryDiffSha256=$endingRepositoryDiffSha256
        untrackedFileSha256=$startingUntrackedFileSha256
        endingUntrackedFileSha256=$endingUntrackedFileSha256
        repositoryStableDuringRun=$repositoryStable
        fileSha256=$startingSourceFileSha256
        buildPerformed=(-not $SkipBuild)
        sourceToBinaryCorrespondence=if ($SkipBuild) { 'notVerified-skip-build' } else { 'PASS-built-after-source-fingerprint' }
        binarySha256AtStart=$startingBinarySha256
        binarySha256AtEnd=$endingBinarySha256
        binariesStableDuringRun=$binariesStable
        productExecutableSha256=$startingBinarySha256.productExecutable
        productAssemblySha256=$startingBinarySha256.productAssembly
        managedHostSha256=$startingBinarySha256.managedHost
        nativeHostSha256=$startingBinarySha256.nativeHost
        resizeObserverSha256=$startingBinarySha256.resizeObserver
    }
    environment=[ordered]@{
        operatingSystem=[Environment]::OSVersion.VersionString
        windowsBuild=[Environment]::OSVersion.Version.Build
        videoControllers=$videoControllers
        selectedVulkanDevice=$Device
        observedWindowDpi=if ($cases.Count) {
            (Get-Content -LiteralPath $cases[0].capturePath -Raw | ConvertFrom-Json).windowDpi
        } else { $null }
        observedRefreshHz=if ($cases.Count) {
            (Get-Content -LiteralPath $cases[0].capturePath -Raw | ConvertFrom-Json).displayRefreshHz
        } else { $null }
    }
    sequence=$sequence
    gates=[ordered]@{
        angleBeforeAfter=if (-not $runAngleBaselines) { 'notRun' } elseif ($anglePass) { 'PASS' } else { 'FAIL' }
        vulkanCurrentMonitor=if ($casePass) { 'PASS' } else { 'FAIL' }
        topLevelRasterOwnerAndThreeBuffers=if (@($cases | Where-Object {
            $_.vulkan.visibleOwner -ne 'top-level DirectComposition Vulkan Presentation target' -or
            $_.vulkan.presentationMode -ne 'CompositionSwapchain' -or
            [int]$_.vulkan.compositionBufferCount -ne 3 -or
            [int]$_.vulkan.activeSwapchains -ne 0
        }).Count -eq 0) { 'PASS' } else { 'FAIL' }
        synchronousVulkanCopyAndAvailabilityRetirement=if (@($cases | Where-Object {
            [int]$_.vulkan.outstandingCopySubmission -ne 0 -or
            [long]$_.vulkan.deferredCopySubmissions -ne 0 -or
            $_.vulkan.retirementMode -ne 'presentation-buffer-availability'
        }).Count -eq 0) { 'PASS' } else { 'FAIL' }
        resizePresentationCadence=if (-not $runAngleBaselines) { 'notRun' } elseif ($cadencePass) { 'PASS' } else { 'FAIL' }
        focusedLeftAndTopLeftReverse600msOnce=if ($runAngleBaselines -or $Probe) { 'notRun' } elseif ($reverse600Qualified) { 'PASS' } else { 'FAIL' }
        focusedTopLeftReverse600msOnce=if (-not $Probe) { 'notRun' } elseif ($reverse600Qualified) { 'PASS' } else { 'FAIL' }
        eightEdgesReverse600msThreeRuns=if (-not $runAngleBaselines) { 'notRun' } elseif ($reverse600Qualified) { 'PASS' } else { 'FAIL' }
        slowMediumFastExpandShrinkReverse=if ($FullCurrentDpiMatrix) {
            'notVerified-no-matched-angle-cadence'
        } else { 'notRun' }
        slowMediumFastExpandShrinkReverseTransportResource=if ($FullCurrentDpiMatrix) {
            if ($casePass) { 'PASS' } else { 'FAIL' }
        } else { 'notRun' }
        edgeStress10Seconds='notRun'
        dpiMatrix100_125_150_200='notVerified'
        refreshMatrix60_120_144_165='notVerified'
        mixedDpiSnapMaximizeAltTabOcclusion='notVerified'
        physicalBorderDragAndScanout='notVerified'
        targetToPresentAngleDelta=if (-not $runAngleBaselines) { 'notRun' } elseif ($targetDeltaPass) { 'PASS' } else { 'FAIL' }
        externalValidationLayer=if (-not $ExternalValidation) { 'notRun' } elseif ($externalValidationPass) { 'PASS' } else { 'FAIL' }
        sourceStableDuringRun=if ($sourceStable) { 'PASS' } else { 'FAIL' }
        keySourceFilesStableAtEndpoints=if ($keySourceFilesStable) { 'PASS' } else { 'FAIL' }
        repositoryStableDuringRun=if ($repositoryStable) { 'PASS' } else { 'FAIL' }
        binariesStableDuringRun=if ($binariesStable) { 'PASS' } else { 'FAIL' }
    }
    cases=$cases
    evidenceBoundary='Focused validation runs only left/top-left Vulkan cases and proves a top-level native topmost DirectComposition target with an identity-transformed full-capacity Presentation source, one top-level client geometry, nonblocking current-plus-latest moving-origin delivery, three Composition buffers, synchronous Vulkan copy completion, availability-based buffer reuse, native non-client input, WGC transport, monotonic visible generation markers, raw validation-background coverage, final exact geometry, terminal accounting, zero active WSI swapchains, and no Vulkan device loss or legacy WSI results. ANGLE cadence/latency comparison and the repeated eight-edge matrix run only when -FullEightEdgeMatrix or -FullCurrentDpiMatrix is explicitly selected. External validation is qualified only when the loader positively reports VK_LAYER_KHRONOS_validation activation and zero validation warning/error messages. Source diff and key source files are SHA-256 fingerprinted; product executable/IL, managed/native host, and observer binaries are hashed before and after execution, while source-to-binary correspondence is PASS only when the validator performed the build. Composition Present receipts and WGC do not prove physical scan-out, transient DWM shell pixels, Acrylic blur quality, or human-perceived drag smoothness; those remain notVerified and require direct physical left/top-left and backdrop checks.'
}
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "Vulkan live resize=$($manifest.status)"
Write-Host "manifest=$manifestPath"
$validationExitCode = if ($manifest.status -eq 'FAIL') { 2 } else { 0 }
}
finally {
    $outputDirectoryLock.Dispose()
}
}
finally {
    $globalValidationLock.Dispose()
}
exit $validationExitCode
