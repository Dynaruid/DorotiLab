#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [switch] $SkipBuild,
    [switch] $Probe,
    [switch] $FullCurrentDpiMatrix
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$runId = 'windows-vulkan-live-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'),
    ([Guid]::NewGuid().ToString('N').Substring(0, 10))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti/evidence/$runId"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$productProject = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj'
$productExecutable = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.HwndExactCppProduct.exe'
$observerBuild = Join-Path $repoRoot '.doroti/build/windows-resize-capture-vulkan'
$observer = Join-Path $observerBuild 'Release/Doroti.WindowsResizeCapture.exe'
$deadline = [DateTime]::UtcNow.AddMinutes(20)

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
    if (-not $process.WaitForExit(60000)) {
        $process.Kill($true)
        throw "$Name exceeded 60 seconds."
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
    $capturePath = Join-Path $OutputDirectory "$Slug.capture.json"
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = New-StartInfo -FileName $productExecutable -ArgumentList @(
        '--presenter',$Presenter,'--lifecycle-cycles','0','--external-resize',
        '--smoke-ms','15000','--report',$appPath
    ) -Environment @{
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE=$readyPath
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FRAME_MARKER='1'
        DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FRAME_MARKER_CORNER=(Get-MarkerCorner $Edge)
    } -Visible
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
        $captureRun = Invoke-BoundedProcess -FileName $observer -Name "$Slug WGC border resize" -ArgumentList @(
            '--hwnd',[string]$ready.hwnd,'--output',$capturePath,'--run-id',"$runId-$Slug",
            '--capture-only','--decode-frame-id','--f6r','--input-hz','240','--png-stride','60',
            '--capture-ring-size','64','--edge',$Edge,'--drag-pixels',[string]$DragPixels,
            '--drag-ms',[string]$DragMilliseconds,'--motion',$Motion,'--duration','1')
        Assert-True ($captureRun.exitCode -eq 0) `
            "$Slug WGC border resize failed: $($captureRun.stderr) $($captureRun.stdout)"
        Request-AppClose ([long]$ready.hwnd)
        if (-not $process.WaitForExit(30000)) { throw "$Slug did not close in 30 seconds." }
        $stdout = $stdoutTask.GetAwaiter().GetResult()
        $stderr = $stderrTask.GetAwaiter().GetResult()
        $app = Get-Content -LiteralPath $appPath -Raw | ConvertFrom-Json -Depth 100
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
        $diagnostics = $app.diagnostics
        $resourcePass = $process.ExitCode -eq 0 -and $app.status -eq 'PASS' -and
            [int]$diagnostics.operationalDebugErrors -eq 0 -and
            [int]$diagnostics.failedTerminals -eq 0 -and
            [int]$diagnostics.unterminatedResizeGenerations -eq 0 -and
            [int]$diagnostics.duplicateResizeTerminals -eq 0 -and
            ($Presenter -ne 'Vulkan' -or (
                [int]$diagnostics.vulkan.deviceLostResults -eq 0 -and
                [int]$diagnostics.vulkan.surfaceLostResults -eq 0 -and
                [int]$diagnostics.vulkan.outstandingAcquired -eq 0 -and
                [int]$diagnostics.vulkan.maximumOutstandingAcquired -le 1 -and
                [int]$diagnostics.vulkan.activeSwapchains -le 1 -and
                [int]$diagnostics.vulkan.retiredSwapchains -le 2))
        $status = if ($transportPass -and $markerPass -and $finalExact -and $resourcePass) { 'PASS' } else { 'FAIL' }
        return [ordered]@{
            name=$Slug; status=$status; presenter=$Presenter
            definition=[ordered]@{ edge=$Edge; motion=$Motion; dragMilliseconds=$DragMilliseconds; dragPixels=$DragPixels }
            transport=$transportPass; marker=$markerPass; finalExact=$finalExact; resource=$resourcePass
            capturedFrames=[int]$capture.capturedFrames; inputSamples=[int]$capture.inputSamples
            decodedFrames=$decodedIds.Count; markerRegressions=$markerRegressions
            frameIdFirst=if ($decodedIds.Count) { $decodedIds[0] } else { $null }
            frameIdLast=if ($decodedIds.Count) { $decodedIds[-1] } else { $null }
            captureIntervalP95Microseconds=$capture.captureIntervalMicroseconds.p95
            vulkan=if ($Presenter -eq 'Vulkan') { [ordered]@{
                acquired=$diagnostics.vulkan.acquired; presented=$diagnostics.vulkan.presented
                outstanding=$diagnostics.vulkan.outstandingAcquired
                maximumOutstanding=$diagnostics.vulkan.maximumOutstandingAcquired
                deviceLost=$diagnostics.vulkan.deviceLostResults; surfaceLost=$diagnostics.vulkan.surfaceLostResults
                outOfDate=$diagnostics.vulkan.outOfDateResults; suboptimal=$diagnostics.vulkan.suboptimalResults
                activeSwapchains=$diagnostics.vulkan.activeSwapchains; retiredSwapchains=$diagnostics.vulkan.retiredSwapchains
                retirementMode=$diagnostics.vulkan.retirementMode
                queueIdleRetirementWaits=$diagnostics.vulkan.queueIdleRetirementWaits
            } } else { $null }
            appPath=$appPath; capturePath=$capturePath
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

$definitions = if ($Probe) {
    @([ordered]@{ run=1; edge='Left'; motion='reverse'; duration=600 })
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
} else {
    @(
        foreach ($run in 1..3) {
            foreach ($edge in @('Left','Right','Top','Bottom','TopLeft','TopRight','BottomLeft','BottomRight')) {
                [ordered]@{ run=$run; edge=$edge; motion='reverse'; duration=600 }
            }
        }
    )
}

$angleBefore = Invoke-CapturedCase -Presenter 'AngleD3D11' -Slug 'angle-before' `
    -Edge 'Left' -Motion 'reverse' -DragMilliseconds 600
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
$angleAfter = Invoke-CapturedCase -Presenter 'AngleD3D11' -Slug 'angle-after' `
    -Edge 'Left' -Motion 'reverse' -DragMilliseconds 600

$videoControllers = @(Get-CimInstance Win32_VideoController |
    Select-Object Name,DriverVersion,CurrentRefreshRate,VideoModeDescription,PNPDeviceID)
$casePass = @($cases | Where-Object status -ne 'PASS').Count -eq 0
$anglePass = $angleBefore.status -eq 'PASS' -and $angleAfter.status -eq 'PASS'
$manifest = [ordered]@{
    schemaVersion='doroti.windows.vulkan-live-resize/v1'
    runId=$runId
    generatedAt=[DateTime]::UtcNow.ToString('O')
    status=if ($casePass -and $anglePass) { 'PASS-automated-partial' } else { 'FAIL' }
    source=[ordered]@{ revision=$head; statusAtStart=$startingStatus }
    environment=[ordered]@{
        operatingSystem=[Environment]::OSVersion.VersionString
        windowsBuild=[Environment]::OSVersion.Version.Build
        videoControllers=$videoControllers
        observedWindowDpi=if ($cases.Count) {
            (Get-Content -LiteralPath $cases[0].capturePath -Raw | ConvertFrom-Json).windowDpi
        } else { $null }
        observedRefreshHz=if ($cases.Count) {
            (Get-Content -LiteralPath $cases[0].capturePath -Raw | ConvertFrom-Json).displayRefreshHz
        } else { $null }
    }
    sequence=@($angleBefore) + @($cases) + @($angleAfter)
    gates=[ordered]@{
        angleBeforeAfter=if ($anglePass) { 'PASS' } else { 'FAIL' }
        vulkanCurrentMonitor=if ($casePass) { 'PASS' } else { 'FAIL' }
        eightEdgesReverse600msThreeRuns=if (-not $Probe -and -not $FullCurrentDpiMatrix -and $casePass) {
            'PASS'
        } elseif ($FullCurrentDpiMatrix -and $casePass) { 'PASS' } else { 'notRun' }
        slowMediumFastExpandShrinkReverse=if ($FullCurrentDpiMatrix -and $casePass) { 'PASS' } else { 'notRun' }
        edgeStress10Seconds='notRun'
        dpiMatrix100_125_150_200='notVerified'
        refreshMatrix60_120_144_165='notVerified'
        mixedDpiSnapMaximizeAltTabOcclusion='notVerified'
        physicalBorderDragAndScanout='notVerified'
        targetToPresentAngleDelta='notVerified'
    }
    cases=$cases
    evidenceBoundary='Automation proves native non-client input, WGC transport, monotonic visible generation markers, final exact geometry, terminal accounting, bounded Vulkan resources, and no Vulkan device/surface loss for the listed cases. WGC does not prove physical scan-out or human-perceived drag smoothness; unlisted monitor, DPI, refresh, window-management, and physical cases remain notVerified.'
}
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "Vulkan live resize=$($manifest.status)"
Write-Host "manifest=$manifestPath"
if ($manifest.status -eq 'FAIL') { exit 2 }
exit 0
