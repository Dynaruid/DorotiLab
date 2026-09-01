#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [switch] $SkipBuild,
    [switch] $SkipVisibleCapture,
    [switch] $SkipPublishProbe
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$b0Project = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-content-island-capability/Doroti.Validation.WindowsAcrylicContentIslandCapability.csproj'
$b0Executable = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-content-island-capability/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsAcrylicContentIslandCapability.exe'
$project = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-composition-swapchain-spike/Doroti.Validation.WindowsAcrylicCompositionSwapchainSpike.csproj'
$output = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-composition-swapchain-spike/bin/Release/net10.0-windows10.0.19041.0/win-x64'
$executable = Join-Path $output 'Doroti.Validation.WindowsAcrylicCompositionSwapchainSpike.exe'
$nativeSource = Join-Path $repoRoot 'Doroti/validation/windows-acrylic-composition-swapchain-spike/native'
$nativeBuild = Join-Path $repoRoot '.doroti/build/windows-acrylic-composition-swapchain-p1cs'
$nativeDll = Join-Path $nativeBuild 'Release/doroti_p1cs_native.dll'
$contractPath = Join-Path $repoRoot 'Doroti/validation/contracts/windows-acrylic-composition-swapchain-p1cs.json'
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json -Depth 32
$runId = 'acrylic-p1cs-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 12))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti/evidence/$runId"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$originalDwmFlush = [Environment]::GetEnvironmentVariable('DOROTI_WINDOWS_DWM_FLUSH', 'Process')
$originalSwapInterval = [Environment]::GetEnvironmentVariable('DOROTI_WINDOWS_EGL_SWAP_INTERVAL', 'Process')
$startingStatus = @(& git -C $repoRoot status --short)

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function New-ProcessStartInfo {
    param(
        [string] $FileName,
        [string[]] $ArgumentList,
        [bool] $Visible = $false,
        [bool] $EmptyPath = $false
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
    $start.Environment['DOROTI_WINDOWS_DWM_FLUSH'] = [string]$contract.comparisonEnvironment.dwmFlush
    $start.Environment['DOROTI_WINDOWS_EGL_SWAP_INTERVAL'] = [string]$contract.comparisonEnvironment.eglSwapInterval
    if ($EmptyPath) { $start.Environment['PATH'] = '' }
    foreach ($argument in $ArgumentList) { [void]$start.ArgumentList.Add($argument) }
    return $start
}

function Invoke-BoundedProcess {
    param(
        [Parameter(Mandatory)][string] $FileName,
        [Parameter(Mandatory)][string[]] $ArgumentList,
        [Parameter(Mandatory)][string] $Name,
        [switch] $EmptyPath
    )
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = New-ProcessStartInfo -FileName $FileName -ArgumentList $ArgumentList -EmptyPath $EmptyPath
    Assert-True $process.Start() "$Name failed to start."
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        $process.Kill($true)
        throw "$Name exceeded the 20-minute timeout."
    }
    $stdout = $stdoutTask.GetAwaiter().GetResult()
    $stderr = $stderrTask.GetAwaiter().GetResult()
    if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
    if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
    $result = [ordered]@{ ExitCode=$process.ExitCode; Stdout=$stdout; Stderr=$stderr }
    $process.Dispose()
    return $result
}

function Start-BoundedProcess([string] $FileName, [string[]] $ArgumentList) {
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = New-ProcessStartInfo -FileName $FileName -ArgumentList $ArgumentList -Visible $true
    Assert-True $process.Start() 'P1-CS visible process failed to start.'
    return [ordered]@{
        Process=$process
        StdoutTask=$process.StandardOutput.ReadToEndAsync()
        StderrTask=$process.StandardError.ReadToEndAsync()
    }
}

function Complete-BoundedProcess([System.Collections.IDictionary] $Running, [string] $Name) {
    $process = $Running.Process
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        $process.Kill($true)
        throw "$Name exceeded the 20-minute timeout."
    }
    $stdout = $Running.StdoutTask.GetAwaiter().GetResult()
    $stderr = $Running.StderrTask.GetAwaiter().GetResult()
    if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
    if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
    $exitCode = $process.ExitCode
    $process.Dispose()
    return $exitCode
}

function Get-Percentile([double[]] $Values, [double] $Percentile) {
    if ($Values.Count -eq 0) { return $null }
    $sorted = @($Values | Sort-Object)
    $index = [Math]::Min($sorted.Count - 1,
        [Math]::Max(0, [Math]::Ceiling($Percentile * $sorted.Count) - 1))
    return [double]$sorted[$index]
}

function Get-PngStatistics($Capture) {
    Add-Type -AssemblyName System.Drawing.Common
    $centerColors = [Collections.Generic.HashSet[string]]::new()
    $sceneColors = [Collections.Generic.HashSet[string]]::new()
    $blankFrames = 0
    $pngCount = 0
    foreach ($frame in @($Capture.frames | Where-Object { $_.png })) {
        $path = Join-Path $OutputDirectory ([string]$frame.png)
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { continue }
        $bitmap = [Drawing.Bitmap]::new($path)
        try {
            $left = [int]$frame.client.left
            $top = [int]$frame.client.top
            $width = [int]$frame.client.width
            $height = [int]$frame.client.height
            if ($width -le 16 -or $height -le 16) { continue }
            $pngCount++
            $frameColors = [Collections.Generic.HashSet[string]]::new()
            foreach ($point in @(@(.20,.20), @(.80,.20), @(.20,.80), @(.80,.80))) {
                $x = [Math]::Min($bitmap.Width - 1, $left + [int]($width * $point[0]))
                $y = [Math]::Min($bitmap.Height - 1, $top + [int]($height * $point[1]))
                $pixel = $bitmap.GetPixel($x, $y)
                $key = '{0:X2}{1:X2}{2:X2}' -f $pixel.R,$pixel.G,$pixel.B
                [void]$frameColors.Add($key)
                [void]$sceneColors.Add($key)
            }
            if ($frameColors.Count -lt 4) { $blankFrames++ }
            $center = $bitmap.GetPixel(
                [Math]::Min($bitmap.Width - 1, $left + [int]($width * .50)),
                [Math]::Min($bitmap.Height - 1, $top + [int]($height * .50)))
            [void]$centerColors.Add(('{0:X2}{1:X2}{2:X2}' -f $center.R,$center.G,$center.B))
        }
        finally { $bitmap.Dispose() }
    }
    return [ordered]@{
        pngCount=$pngCount
        alphaSceneDistinctColors=$sceneColors.Count
        transparentCenterDistinctColors=$centerColors.Count
        blankPngFrames=$blankFrames
    }
}

function Get-CaptureExactness($Report, $Capture) {
    $presented = @{}
    foreach ($terminal in @($Report.visible.render.terminalSample | Where-Object { $_.status -eq 'presented' })) {
        $presented[[int]$terminal.generation] = $terminal
    }
    $matched = 0
    $mismatch = 0
    $unmatched = 0
    $firstCapture = @{}
    $visibleLatencies = [Collections.Generic.List[double]]::new()
    $frequency = [double]$Capture.clockCalibration.qpcFrequency
    foreach ($frame in @($Capture.frames | Where-Object { $null -ne $_.frameId })) {
        $id = [int]$frame.frameId
        if (-not $presented.ContainsKey($id)) { $unmatched++; continue }
        $matched++
        $terminal = $presented[$id]
        if ([int]$frame.client.width -ne [int]$terminal.width -or
            [int]$frame.client.height -ne [int]$terminal.height) { $mismatch++ }
        if (-not $firstCapture.ContainsKey($id)) {
            $captureQpc = [double]$frame.systemRelative100ns * $frequency / 10000000.0
            if ($captureQpc -ge [double]$terminal.terminalQpc) {
                $firstCapture[$id] = $true
                $visibleLatencies.Add(
                    ($captureQpc - [double]$terminal.terminalQpc) * 1000000.0 / $frequency)
            }
        }
    }
    $presentLatencies = @($presented.Values | ForEach-Object {
        ([double]$_.terminalQpc - [double]$_.requestedQpc) * 1000000.0 / $frequency
    })
    return [ordered]@{
        matchedFrames=$matched
        wrongSizeOrStaleFrames=$mismatch
        unmatchedDecodedFrames=$unmatched
        presentedGenerationCount=$presented.Count
        targetToPresentP50Microseconds=Get-Percentile $presentLatencies 0.50
        targetToPresentP95Microseconds=Get-Percentile $presentLatencies 0.95
        targetToPresentMaxMicroseconds=if ($presentLatencies.Count -eq 0) { $null } else { ($presentLatencies | Measure-Object -Maximum).Maximum }
        presentToVisibleCount=$visibleLatencies.Count
        presentToVisibleP50Microseconds=Get-Percentile ($visibleLatencies.ToArray()) 0.50
        presentToVisibleP95Microseconds=Get-Percentile ($visibleLatencies.ToArray()) 0.95
        presentToVisibleMaxMicroseconds=if ($visibleLatencies.Count -eq 0) { $null } else { ($visibleLatencies | Measure-Object -Maximum).Maximum }
    }
}

if (-not $SkipBuild) {
    $configure = Invoke-BoundedProcess -FileName 'cmake' -Name 'P1-CS native configure' -ArgumentList @(
        '-S',$nativeSource,'-B',$nativeBuild,'-A','x64')
    Assert-True ($configure.ExitCode -eq 0) 'P1-CS native configure failed.'
    $nativeRun = Invoke-BoundedProcess -FileName 'cmake' -Name 'P1-CS native build' -ArgumentList @(
        '--build',$nativeBuild,'--config','Release')
    Assert-True ($nativeRun.ExitCode -eq 0) 'P1-CS native build failed.'
    foreach ($buildProject in @($b0Project,$project)) {
        $build = Invoke-BoundedProcess -FileName 'dotnet' -Name "build $buildProject" -ArgumentList @(
            'build',$buildProject,'-c','Release','--nologo')
        Assert-True ($build.ExitCode -eq 0) "Build failed: $buildProject"
    }
    Copy-Item -LiteralPath $nativeDll -Destination (Join-Path $output 'doroti_p1cs_native.dll') -Force
}
Assert-True (Test-Path -LiteralPath $b0Executable -PathType Leaf) "B0 executable is missing: $b0Executable"
Assert-True (Test-Path -LiteralPath $executable -PathType Leaf) "P1-CS executable is missing: $executable"
Assert-True (Test-Path -LiteralPath (Join-Path $output 'doroti_p1cs_native.dll') -PathType Leaf) 'P1-CS native bridge is missing.'

$b0Path = Join-Path $OutputDirectory 'b0.json'
$b0Run = Invoke-BoundedProcess -FileName $b0Executable -Name 'fresh B0 capability' -ArgumentList @('--report',$b0Path)
$b0 = Get-Content -LiteralPath $b0Path -Raw | ConvertFrom-Json -Depth 100
$b0Pass = $b0Run.ExitCode -eq 0 -and $b0.status -eq 'PASS' -and
    $b0.backdrop.addTargetResult -and $b0.graphics.directImportSucceeded -and
    $b0.graphics.cpuReadbackCount -eq 0

$capabilityPath = Join-Path $OutputDirectory 'capability-and-buffers.json'
$capabilityRun = Invoke-BoundedProcess -FileName $executable -Name 'P1-CS capability and 500-buffer protocol' -ArgumentList @('--report',$capabilityPath)
$capability = Get-Content -LiteralPath $capabilityPath -Raw | ConvertFrom-Json -Depth 100
$capabilityPass = $capabilityRun.ExitCode -eq 0 -and
    $capability.status -eq 'PASS-capability' -and
    $capability.snapshot.presentationSupported -eq 1 -and
    $capability.snapshot.factoryHresult -ge 0 -and
    $capability.snapshot.managerHresult -ge 0 -and
    $capability.snapshot.surfaceHandleHresult -ge 0 -and
    $capability.snapshot.presentationSurfaceHresult -ge 0 -and
    $capability.compositionSurfaceConnected
$bufferPass = $capability.bufferProtocol.status -eq 'PASS' -and
    $capability.bufferProtocol.presented -eq [int]$contract.requiredBufferPresents -and
    $capability.bufferProtocol.maximumSlots -le [int]$contract.maximumSlots -and
    $capability.bufferProtocol.maximumQueueDepth -le [int]$contract.maximumQueueDepth -and
    $capability.bufferProtocol.unavailableReuseAttempts -eq 0 -and
    $capability.bufferProtocol.wrongSizePresents -eq 0 -and
    $capability.bufferProtocol.stalePresents -eq 0 -and
    $capability.bufferProtocol.cpuCopyCount -eq 0

$packageProbe = [ordered]@{ status='notRun'; exitCode=$null; report=$null; emptyPath=$true }
if (-not $SkipPublishProbe) {
    $publishDirectory = Join-Path $OutputDirectory 'publish'
    $publish = Invoke-BoundedProcess -FileName 'dotnet' -Name 'P1-CS self-contained publish' -ArgumentList @(
        'publish',$project,'-c','Release','-r','win-x64','--self-contained','true',
        '-p:PublishSingleFile=false','-o',$publishDirectory,'--nologo')
    Assert-True ($publish.ExitCode -eq 0) 'P1-CS self-contained publish failed.'
    Copy-Item -LiteralPath $nativeDll -Destination (Join-Path $publishDirectory 'doroti_p1cs_native.dll') -Force
    $packagePath = Join-Path $OutputDirectory 'empty-path.json'
    $packageRun = Invoke-BoundedProcess -FileName (Join-Path $publishDirectory 'Doroti.Validation.WindowsAcrylicCompositionSwapchainSpike.exe') -Name 'P1-CS empty-PATH launch' -EmptyPath -ArgumentList @('--report',$packagePath)
    $packageReport = Get-Content -LiteralPath $packagePath -Raw | ConvertFrom-Json -Depth 100
    $packageProbe = [ordered]@{
        status=if ($packageRun.ExitCode -eq 0 -and $packageReport.status -eq 'PASS-capability') { 'PASS' } else { 'FAIL' }
        exitCode=$packageRun.ExitCode
        report=$packagePath
        emptyPath=$true
    }
}

$visible = [ordered]@{
    status=if ($SkipVisibleCapture) { 'notVerified' } else { 'notRun' }
    reason=if ($SkipVisibleCapture) { 'SkipVisibleCapture' } else { 'notRun' }
    app=$null
    capture=$null
    png=$null
    exactness=$null
}
$visibleInternalPass = [bool]$SkipVisibleCapture
$captureTransportPass = [bool]$SkipVisibleCapture
$alphaPass = [bool]$SkipVisibleCapture
$exactPass = [bool]$SkipVisibleCapture
if (-not $SkipVisibleCapture) {
    $observerBuild = Join-Path $repoRoot '.doroti/build/windows-resize-capture-acrylic-p1cs'
    $observer = Join-Path $observerBuild 'Release/Doroti.WindowsResizeCapture.exe'
    if (-not (Test-Path -LiteralPath $observer -PathType Leaf)) {
        $observerConfigure = Invoke-BoundedProcess -FileName 'cmake' -Name 'P1-CS observer configure' -ArgumentList @(
            '-S',(Join-Path $repoRoot 'Doroti/validation/windows-resize-capture'),'-B',$observerBuild,'-A','x64')
        Assert-True ($observerConfigure.ExitCode -eq 0) 'P1-CS observer configure failed.'
        $observerBuildRun = Invoke-BoundedProcess -FileName 'cmake' -Name 'P1-CS observer build' -ArgumentList @(
            '--build',$observerBuild,'--config','Release')
        Assert-True ($observerBuildRun.ExitCode -eq 0) 'P1-CS observer build failed.'
    }
    $readyPath = Join-Path $OutputDirectory 'visible.ready.json'
    $appPath = Join-Path $OutputDirectory 'visible-app.json'
    $capturePath = Join-Path $OutputDirectory 'visible-capture.json'
    $running = Start-BoundedProcess -FileName $executable -ArgumentList @(
        '--duration','15','--ready-file',$readyPath,'--report',$appPath)
    try {
        $deadline = [DateTime]::UtcNow.AddSeconds(30)
        while (-not (Test-Path -LiteralPath $readyPath -PathType Leaf)) {
            if ($running.Process.HasExited) { throw "P1-CS visible process exited before ready: $($running.Process.ExitCode)" }
            if ([DateTime]::UtcNow -ge $deadline) { throw 'P1-CS visible ready timeout.' }
            Start-Sleep -Milliseconds 50
        }
        $ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
        $captureRun = $null
        for ($attempt = 1; $attempt -le 2; $attempt++) {
            $captureRun = Invoke-BoundedProcess -FileName $observer -Name "P1-CS WGC/native resize attempt $attempt" -ArgumentList @(
                '--hwnd',[string]$ready.hwnd,'--output',$capturePath,'--run-id',"$runId-visible",
                '--duration','4','--input-hz','240','--png-stride','10','--capture-only',
                '--no-desktop-duplication','--edge','Right','--drag-pixels','180',
                '--drag-ms','3000','--motion','reverse')
            if ($captureRun.ExitCode -eq 0 -and (Test-Path -LiteralPath $capturePath -PathType Leaf)) { break }
            Start-Sleep -Milliseconds 250
        }
        $appExitCode = Complete-BoundedProcess -Running $running -Name 'P1-CS visible process'
        $running = $null
        $app = Get-Content -LiteralPath $appPath -Raw | ConvertFrom-Json -Depth 100
        $capture = Get-Content -LiteralPath $capturePath -Raw | ConvertFrom-Json -Depth 100
        $png = Get-PngStatistics $capture
        $exactness = Get-CaptureExactness $app $capture
        $visibleInternalPass = $appExitCode -eq 0 -and $app.status -eq 'PASS-internal' -and
            $app.visible.status -eq 'PASS-internal' -and
            $app.visible.render.accepted -ge [int]$contract.requiredVisibleResizeGenerations -and
            $app.visible.render.maximumQueueDepth -le [int]$contract.maximumQueueDepth -and
            $app.visible.render.maximumSlots -le [int]$contract.maximumSlots -and
            $app.visible.render.unavailableReuseAttempts -eq 0 -and
            $app.visible.render.duplicateTerminals -eq 0 -and
            $app.visible.render.missingTerminals -eq 0
        $captureTransportPass = $captureRun.ExitCode -eq 0 -and
            $capture.capturedFrames -gt 0 -and $capture.encodedPngFrames -gt 0 -and
            $capture.captureErrors -le [int]$contract.maximumCaptureErrors -and
            $capture.captureRingDroppedFrames -le [int]$contract.maximumCaptureRingDrops -and
            $capture.poolCapacityExceededFrames -eq 0
        $alphaPass = $png.alphaSceneDistinctColors -ge [int]$contract.minimumAlphaSceneDistinctColors -and
            $png.transparentCenterDistinctColors -ge [int]$contract.minimumTransparentRoiDistinctColors -and
            $png.blankPngFrames -eq 0
        $exactPass = $exactness.matchedFrames -ge [int]$contract.minimumMatchedCaptureFrames -and
            $exactness.wrongSizeOrStaleFrames -le [int]$contract.maximumWrongSizeOrStaleCaptureFrames
        $visible.status = if ($visibleInternalPass -and $captureTransportPass -and $alphaPass -and $exactPass) { 'PASS' } else { 'FAIL' }
        $visible.reason = if (-not $visibleInternalPass) { 'internal-three-slot-or-terminal-gate' }
            elseif (-not $captureTransportPass) { 'capture-transport-or-native-resize' }
            elseif (-not $alphaPass) { 'acrylic-alpha-scene-not-observed' }
            elseif (-not $exactPass) { 'composition-geometry-and-presented-buffer-were-not-exact-in-the-same-captured-frame' }
            else { 'automated-visible-gates-passed' }
        $visible.app = [ordered]@{ exitCode=$appExitCode; report=$app; path=$appPath }
        $visible.capture = [ordered]@{ exitCode=$captureRun.ExitCode; report=$capture; path=$capturePath }
        $visible.png = $png
        $visible.exactness = $exactness
    }
    finally {
        if ($null -ne $running) {
            if (-not $running.Process.HasExited) { $running.Process.Kill($true) }
            $running.Process.Dispose()
        }
    }
}

$dxdiagPath = Join-Path $OutputDirectory 'dxdiag.txt'
$dxdiagRun = Invoke-BoundedProcess -FileName 'dxdiag.exe' -Name 'dxdiag environment capture' -ArgumentList @('/whql:off','/t',$dxdiagPath)
$driverModels = @()
if ($dxdiagRun.ExitCode -eq 0 -and (Test-Path -LiteralPath $dxdiagPath -PathType Leaf)) {
    $driverModels = @(Get-Content -LiteralPath $dxdiagPath | Where-Object {
        $_ -match 'Driver Model|드라이버 모델|Driver Version|드라이버 버전'
    } | ForEach-Object { $_.Trim() } | Sort-Object -Unique)
}
$videoControllers = @(Get-CimInstance Win32_VideoController | Select-Object Name,DriverVersion,CurrentRefreshRate,VideoModeDescription,PNPDeviceID)
$p05ManifestPath = @(Get-ChildItem -LiteralPath (Join-Path $repoRoot '.doroti/evidence') -Recurse -Filter manifest.json |
    Where-Object { $_.Directory.Name -like 'acrylic-p05-*' } | Sort-Object LastWriteTime | Select-Object -Last 1).FullName
$opaqueBaseline = $null
if ($p05ManifestPath) {
    $p05Manifest = Get-Content -LiteralPath $p05ManifestPath -Raw | ConvertFrom-Json -Depth 100
    $opaqueBaseline = [ordered]@{
        manifest=$p05ManifestPath
        before=$p05Manifest.visible.opaqueBefore
        after=$p05Manifest.visible.opaqueAfter
    }
}

$automatedPass = $b0Pass -and $capabilityPass -and $bufferPass -and
    ($packageProbe.status -in @('PASS','notRun')) -and $visible.status -eq 'PASS'
$status = if ($automatedPass) { 'PASS-automated' } else { 'FAIL' }
$decisionReason = if (-not $b0Pass) { 'Fresh B0 same-device ANGLE/ContentIsland capability failed.' }
    elseif (-not $capabilityPass) { 'Presentation factory/manager/surface capability failed.' }
    elseif (-not $bufferPass) { 'Three-slot available-event buffer protocol failed.' }
    elseif ($visible.status -eq 'FAIL') { $visible.reason }
    else { 'Automated gates passed, but physical/input/IME/UIA/policy/device-loss acceptance remains notVerified.' }
$manifest = [ordered]@{
    schema='doroti.windows-acrylic-composition-swapchain-p1cs-manifest/v1'
    runId=$runId
    status=$status
    generatedAt=[DateTime]::UtcNow.ToString('O')
    source=[ordered]@{
        revision=(& git -C $repoRoot rev-parse HEAD).Trim()
        statusAtValidatorStart=$startingStatus
        productSourceModified=$false
        scope='validation/eng/contracts only; no Doroti/src or public API/ABI changes'
    }
    environment=[ordered]@{
        operatingSystem=$capability.operatingSystem
        windowsBuild=[Environment]::OSVersion.Version.Build
        windowsAppSdk='2.4.0'
        windowsSdk=[string]$contract.windowsSdk
        dpi=if ($null -ne $visible.app) { $visible.app.report.visible.dpi } else { $b0.dpi }
        adapterLuid=[ordered]@{ low=$capability.snapshot.adapterLuidLow; high=$capability.snapshot.adapterLuidHigh }
        adapterVendorId=$capability.snapshot.adapterVendorId
        adapterDeviceId=$capability.snapshot.adapterDeviceId
        angleAdapter=$b0.graphics.clientExtensions
        videoControllers=$videoControllers
        driverModels=$driverModels
        dxdiag=$dxdiagPath
        parentDwmFlush=if ($null -eq $originalDwmFlush) { 'unset' } else { $originalDwmFlush }
        parentEglSwapInterval=if ($null -eq $originalSwapInterval) { 'unset' } else { $originalSwapInterval }
        comparisonDwmFlush=[string]$contract.comparisonEnvironment.dwmFlush
        comparisonEglSwapInterval=[string]$contract.comparisonEnvironment.eglSwapInterval
    }
    gates=[ordered]@{
        freshB0ContentIslandAndDirectImport=if ($b0Pass) { 'PASS' } else { 'FAIL' }
        presentationCapability=if ($capabilityPass) { 'PASS' } else { 'FAIL' }
        availableEventBufferProtocol500=if ($bufferPass) { 'PASS' } else { 'FAIL' }
        selfContainedEmptyPath=$packageProbe.status
        visibleInternalThreeSlot=if ($visibleInternalPass) { 'PASS' } else { 'FAIL' }
        wgcAndNativePointerResize=if ($captureTransportPass) { 'PASS' } else { 'FAIL' }
        acrylicAlphaScene=if ($alphaPass) { 'PASS-capture' } else { 'FAIL' }
        exactPresentedBufferAgainstCapturedClient=if ($exactPass) { 'PASS' } else { 'FAIL' }
        timingAgainstOpaque='notQualifiedBecauseExactnessFailed'
        automaticRuns3='notRunAfterFirstHardGateFail'
        physicalBorderDrag='notVerified'
        monitorDpiCrossing='notVerified'
        snapOcclusionPolicyRdp='notVerified'
        pointerKeyboardImeClipboardUia='notVerified'
        deviceLossMinimizeRestoreClose='notVerified'
    }
    capabilityAndBuffers=$capability
    freshB0=$b0
    packageProbe=$packageProbe
    visible=$visible
    opaqueBaseline=$opaqueBaseline
    decision=[ordered]@{
        p1Cs=if ($automatedPass) { 'notVerified' } else { 'FAIL' }
        reason=$decisionReason
        proceedToProductIntegration=$false
        opaqueRemainsDefault=$true
    }
    evidenceBoundary='WGC/native pointer automation proves capture transport and scripted right-border input only. Physical scan-out, human 8-direction border drag, monitor/DPI crossing, IME, UIA, policy, RDP, and device-loss acceptance remain notVerified.'
}
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "P1-CS validator=$status decision=$($manifest.decision.p1Cs) visible=$($visible.status) exact=$exactPass"
Write-Host "manifest=$manifestPath"
if ($status -eq 'FAIL') { exit 2 }
exit 0
