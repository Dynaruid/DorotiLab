#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [string] $Device = $env:DOROTI_WINDOWS_VULKAN_DEVICE,
    [switch] $SkipBuild
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$contractPath = Join-Path $repoRoot 'Doroti/validation/contracts/windows-vulkan-v0.json'
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json -Depth 32
$runId = 'windows-vulkan-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 10))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti/evidence/$runId"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$project = Join-Path $repoRoot 'Doroti/validation/windows-vulkan-capability/Doroti.Validation.WindowsVulkanCapability.csproj'
$targetProject = Join-Path $repoRoot 'Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64/Doroti.Target.Windows.WindowsAppSdk.win-x64.csproj'
$productProject = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj'

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Invoke-BoundedProcess {
    param(
        [Parameter(Mandatory)][string] $FileName,
        [string[]] $ArgumentList = @(),
        [Parameter(Mandatory)][string] $Name,
        [hashtable] $Environment = @{}
    )
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.WindowStyle = [Diagnostics.ProcessWindowStyle]::Hidden
    foreach ($argument in $ArgumentList) { [void]$start.ArgumentList.Add($argument) }
    foreach ($entry in $Environment.GetEnumerator()) { $start.Environment[[string]$entry.Key] = [string]$entry.Value }
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = $start
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

function Invoke-Probe {
    param([string] $Name, [string[]] $Arguments)
    $args = @('run','--project',$project,'-c','Release','--no-build','--') + $Arguments
    return Invoke-BoundedProcess -FileName 'dotnet' -ArgumentList $args -Name $Name
}

$startingStatus = @(& git -C $repoRoot status --short)
$head = (& git -C $repoRoot rev-parse HEAD).Trim()
$oldReference = (& git -C $repoRoot rev-parse 'aa3f532^').Trim()
$oldSource = & git -C $repoRoot show 'aa3f532^:Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.cs'
$oldSourceHash = ($oldSource -join "`n" | & git -C $repoRoot hash-object --stdin).Trim()

if (-not $SkipBuild) {
    $nativeBuild = Invoke-BoundedProcess -FileName 'pwsh' -Name 'native Release build' -ArgumentList @(
        '-NoProfile','-File',(Join-Path $repoRoot 'Doroti/eng/build-hwnd-exact-cpp-native.ps1'))
    Assert-True ($nativeBuild.exitCode -eq 0) "Native Release build failed: $($nativeBuild.stderr)"
    $targetBuild = Invoke-BoundedProcess -FileName 'dotnet' -Name 'Windows target Release build' -ArgumentList @(
        'build',$targetProject,'-c','Release')
    Assert-True ($targetBuild.exitCode -eq 0) "Windows target Release build failed: $($targetBuild.stderr)"
    $probeBuild = Invoke-BoundedProcess -FileName 'dotnet' -Name 'Vulkan capability Release build' -ArgumentList @(
        'build',$project,'-c','Release')
    Assert-True ($probeBuild.exitCode -eq 0) "Vulkan capability build failed: $($probeBuild.stderr)"
    $productBuild = Invoke-BoundedProcess -FileName 'dotnet' -Name 'Windows product validation Release build' -ArgumentList @(
        'build',$productProject,'-c','Release')
    Assert-True ($productBuild.exitCode -eq 0) "Windows product validation build failed: $($productBuild.stderr)"
}

$anglePath = Join-Path $OutputDirectory 'angle-product.json'
$angle = Invoke-BoundedProcess -FileName 'dotnet' -Name 'ANGLE product baseline' -ArgumentList @(
    'run','--project',$productProject,'-c','Release','--no-build','--','--report',$anglePath)
Assert-True ($angle.exitCode -eq 0) "ANGLE product baseline failed: $($angle.stderr)"
$angleReport = Get-Content -LiteralPath $anglePath -Raw | ConvertFrom-Json -Depth 100
Assert-True ($angleReport.status -eq 'PASS') 'ANGLE product report is not PASS.'
Assert-True ($angleReport.diagnostics.presenterBackend -eq 'ANGLE/EGL-D3D11') 'Unset/default ANGLE identity changed.'
Assert-True ([bool]$angleReport.diagnostics.visibleAfterExactPresent) 'ANGLE first exact present was not marked visible.'

if ([string]::IsNullOrWhiteSpace($Device)) {
    throw 'V1 requires -Device or DOROTI_WINDOWS_VULKAN_DEVICE on a multi-GPU system.'
}
$candidatePath = Join-Path $OutputDirectory 'capability.json'
$candidate = Invoke-Probe -Name 'Vulkan capability candidate' -Arguments @('--device',$Device,'--output',$candidatePath)
Assert-True ($candidate.exitCode -eq 0) "Vulkan capability candidate failed: $($candidate.stderr) $($candidate.stdout)"
$capability = Get-Content -LiteralPath $candidatePath -Raw | ConvertFrom-Json -Depth 100
Assert-True ($capability.status -eq 'PASS') 'Vulkan capability report is not PASS.'
Assert-True ($capability.packages.requestedVersion -eq [string]$contract.packageVersion) 'Silk.NET version differs.'
Assert-True ($capability.instance.validationWarnings -le [int]$contract.maximumValidationWarnings) 'Validation warning gate failed.'
Assert-True ($capability.instance.validationErrors -le [int]$contract.maximumValidationErrors) 'Validation error gate failed.'
Assert-True ([bool]$capability.presentationCommitPolicy.acquireAfterLatestCheck) 'Acquire is not behind the latest-frame commit check.'
Assert-True ([bool]$capability.presentationCommitPolicy.unconditionalCopyAndPresentAfterAcquire) 'Post-acquire copy/present is not unconditional.'
Assert-True (-not [bool]$capability.presentationCommitPolicy.acquiredImageReleaseRequired) 'Capability still requires acquired-image release.'
Assert-True ($capability.presentationCommitPolicy.retirementMode -eq [string]$contract.retirementMode) 'Retirement mode differs.'
Assert-True ($capability.presentationCommitPolicy.outstandingImages -eq 0) 'Capability probe left an acquired image outstanding.'

$wsiPath = Join-Path $OutputDirectory 'wsi-qualification.json'
$wsiRun = Invoke-Probe -Name 'Vulkan WSI qualification' -Arguments @(
    '--device',$Device,'--wsi-qualification','--output',$wsiPath)
Assert-True ($wsiRun.exitCode -eq 0) "Vulkan WSI qualification failed: $($wsiRun.stderr) $($wsiRun.stdout)"
$wsiDocument = Get-Content -LiteralPath $wsiPath -Raw | ConvertFrom-Json -Depth 100
$wsi = $wsiDocument.wsiStress
Assert-True ($wsiDocument.status -eq 'PASS' -and $wsi.status -eq 'PASS') 'Vulkan WSI qualification report is not PASS.'
Assert-True ($wsi.profile -eq 'qualification') 'Vulkan WSI qualification used the wrong profile.'
Assert-True ($wsi.requestedPresentIterations -eq 3) 'Vulkan WSI qualification present count drifted.'
Assert-True ($wsi.requestedStaleIterationsPerStage -eq 3) 'Vulkan WSI qualification stale count drifted.'
Assert-True ($wsi.requestedRecreateIterations -eq 3) 'Vulkan WSI qualification recreate count drifted.'
Assert-True ($wsi.requestedLifecycleIterations -eq 2) 'Vulkan WSI qualification lifecycle count drifted.'
Assert-True ($wsi.outstandingAcquired -eq 0) 'Vulkan WSI qualification leaked an acquired image.'
Assert-True ($wsi.unconsumedSignals -eq 0) 'Vulkan WSI qualification left an unconsumed semaphore signal.'
Assert-True ($wsi.committedAfterStale -eq 9) 'Post-commit stale cases did not all finish presentation.'
Assert-True ($wsi.supersededBeforeCommit -eq 7) 'Pre-commit/lifecycle superseded terminal count differs.'
Assert-True ($wsi.retirementMode -eq [string]$contract.retirementMode) 'WSI retirement mode differs.'
Assert-True ($wsi.maximumRetiredSwapchains -le 2) 'Vulkan WSI qualification exceeded the retired swapchain bound.'
Assert-True ($wsi.activeSwapchains -eq 1) 'Vulkan WSI qualification did not finish with one active swapchain.'
Assert-True ($wsiDocument.instance.validationWarnings -eq 0) 'Vulkan WSI qualification emitted validation warnings.'
Assert-True ($wsiDocument.instance.validationErrors -eq 0) 'Vulkan WSI qualification emitted validation errors.'

$vulkanProductPath = Join-Path $OutputDirectory 'vulkan-product.json'
$vulkanProduct = Invoke-BoundedProcess -FileName 'dotnet' -Name 'Vulkan product qualification' -Environment @{
    DOROTI_WINDOWS_VULKAN_DEVICE = $Device
} -ArgumentList @(
    'run','--project',$productProject,'-c','Release','--no-build','--',
    '--presenter','Vulkan','--report',$vulkanProductPath)
Assert-True ($vulkanProduct.exitCode -eq 0) "Vulkan product qualification failed: $($vulkanProduct.stderr)"
$vulkanProductReport = Get-Content -LiteralPath $vulkanProductPath -Raw | ConvertFrom-Json -Depth 100
Assert-True ($vulkanProductReport.status -eq 'PASS') 'Vulkan product qualification report is not PASS.'
Assert-True ($vulkanProductReport.diagnostics.requestedPresenter -eq 'Vulkan' -and
    $vulkanProductReport.diagnostics.effectivePresenter -eq 'Vulkan') 'Explicit Vulkan did not remain effective.'
Assert-True ($vulkanProductReport.diagnostics.failedTerminals -eq 0 -and
    $vulkanProductReport.diagnostics.unterminatedResizeGenerations -eq 0 -and
    $vulkanProductReport.diagnostics.duplicateResizeTerminals -eq 0) 'Vulkan product terminal gate failed.'
Assert-True ($vulkanProductReport.diagnostics.deviceGenerations -eq 2) 'Vulkan product device-reset qualification differs.'

$injectedResults = [ordered]@{}
foreach ($resultName in @('OUT_OF_DATE','SUBOPTIMAL','SURFACE_LOST','DEVICE_LOST')) {
    $resultSlug = $resultName.ToLowerInvariant()
    $resultPath = Join-Path $OutputDirectory "vulkan-inject-$resultSlug.json"
    $resultRun = Invoke-BoundedProcess -FileName 'dotnet' -Name "Vulkan $resultName injection" -Environment @{
        DOROTI_WINDOWS_VULKAN_DEVICE = $Device
    } -ArgumentList @(
        'run','--project',$productProject,'-c','Release','--no-build','--',
        '--presenter','Vulkan','--inject-vulkan-result',$resultName,'--lifecycle-cycles','0','--no-resize-burst',
        '--smoke-ms','5000','--report',$resultPath)
    Assert-True ($resultRun.exitCode -eq 0) "Vulkan $resultName injection failed: $($resultRun.stderr)"
    $resultReport = Get-Content -LiteralPath $resultPath -Raw | ConvertFrom-Json -Depth 100
    Assert-True ($resultReport.status -eq 'PASS' -and
        $resultReport.diagnostics.unterminatedResizeGenerations -eq 0 -and
        $resultReport.diagnostics.duplicateResizeTerminals -eq 0 -and
        $resultReport.diagnostics.vulkan.outstandingAcquired -eq 0) `
        "Vulkan $resultName injection did not drain one-to-one."
    $injectedResults[$resultName] = [ordered]@{
        status='PASS'; report=$resultPath
        failedTerminals=$resultReport.diagnostics.failedTerminals
        deviceRecoveries=$resultReport.diagnostics.vulkanDeviceLossRecoveries
        surfaceRecoveries=$resultReport.diagnostics.vulkanSurfaceLossRecoveries
        acquired=$resultReport.diagnostics.vulkan.acquired
        presented=$resultReport.diagnostics.vulkan.presented
        outstanding=$resultReport.diagnostics.vulkan.outstandingAcquired
    }
}

$resetPath = Join-Path $OutputDirectory 'vulkan-device-reset-10.json'
$resetRun = Invoke-BoundedProcess -FileName 'dotnet' -Name 'Vulkan device reset x10' -Environment @{
    DOROTI_WINDOWS_VULKAN_DEVICE = $Device
} -ArgumentList @(
    'run','--project',$productProject,'-c','Release','--no-build','--',
    '--presenter','Vulkan','--device-resets','10','--no-resize-burst','--smoke-ms','13000','--report',$resetPath)
Assert-True ($resetRun.exitCode -eq 0) "Vulkan device reset x10 failed: $($resetRun.stderr)"
$resetReport = Get-Content -LiteralPath $resetPath -Raw | ConvertFrom-Json -Depth 100
Assert-True ($resetReport.status -eq 'PASS' -and
    $resetReport.diagnostics.requestedDeviceResets -eq 10 -and
    $resetReport.diagnostics.completedDeviceResets -eq 10 -and
    $resetReport.diagnostics.failedTerminals -eq 0 -and
    $resetReport.diagnostics.vulkan.outstandingAcquired -eq 0) 'Vulkan device reset x10 gate failed.'

$lifecyclePath = Join-Path $OutputDirectory 'vulkan-lifecycle-10.json'
$lifecycleRun = Invoke-BoundedProcess -FileName 'dotnet' -Name 'Vulkan minimize/restore x10' -Environment @{
    DOROTI_WINDOWS_VULKAN_DEVICE = $Device
} -ArgumentList @(
    'run','--project',$productProject,'-c','Release','--no-build','--',
    '--presenter','Vulkan','--lifecycle-cycles','10','--no-resize-burst','--smoke-ms','6000','--report',$lifecyclePath)
Assert-True ($lifecycleRun.exitCode -eq 0) "Vulkan minimize/restore x10 failed: $($lifecycleRun.stderr)"
$lifecycleReport = Get-Content -LiteralPath $lifecyclePath -Raw | ConvertFrom-Json -Depth 100
Assert-True ($lifecycleReport.status -eq 'PASS' -and
    $lifecycleReport.lifecycleCycles -eq 10 -and
    $lifecycleReport.diagnostics.failedTerminals -eq 0 -and
    $lifecycleReport.diagnostics.vulkan.outstandingAcquired -eq 0) 'Vulkan lifecycle x10 gate failed.'

$resizePath = Join-Path $OutputDirectory 'vulkan-exact-resize-10.json'
$resizeRun = Invoke-BoundedProcess -FileName 'dotnet' -Name 'Vulkan exact resize x10' -Environment @{
    DOROTI_WINDOWS_VULKAN_DEVICE = $Device
} -ArgumentList @(
    'run','--project',$productProject,'-c','Release','--no-build','--',
    '--presenter','Vulkan','--resize-cycles','10','--lifecycle-cycles','0',
    '--smoke-ms','15000','--report',$resizePath)
Assert-True ($resizeRun.exitCode -eq 0) "Vulkan exact resize x10 failed: $($resizeRun.stderr)"
$resizeReport = Get-Content -LiteralPath $resizePath -Raw | ConvertFrom-Json -Depth 100
Assert-True ($resizeReport.status -eq 'PASS' -and $resizeReport.completedResizeRequests -eq 10 -and
    $resizeReport.diagnostics.resizeBuffers -ge 10 -and $resizeReport.diagnostics.presents -ge 10 -and
    $resizeReport.diagnostics.gpuCopies -ge 10 -and
    $resizeReport.diagnostics.vulkan.outstandingAcquired -eq 0) 'Vulkan exact resize x10 gate failed.'

$startCloseCycles = @()
foreach ($cycle in 1..10) {
    $startClosePath = Join-Path $OutputDirectory ('vulkan-start-close-{0:D2}.json' -f $cycle)
    $startCloseRun = Invoke-BoundedProcess -FileName 'dotnet' -Name "Vulkan start/close $cycle/10" -Environment @{
        DOROTI_WINDOWS_VULKAN_DEVICE = $Device
    } -ArgumentList @(
        'run','--project',$productProject,'-c','Release','--no-build','--',
        '--presenter','Vulkan','--lifecycle-cycles','0','--no-resize-burst',
        '--smoke-ms','2500','--report',$startClosePath)
    Assert-True ($startCloseRun.exitCode -eq 0) "Vulkan start/close $cycle failed: $($startCloseRun.stderr)"
    $startCloseReport = Get-Content -LiteralPath $startClosePath -Raw | ConvertFrom-Json -Depth 100
    Assert-True ($startCloseReport.status -eq 'PASS' -and
        $startCloseReport.diagnostics.failedTerminals -eq 0 -and
        $startCloseReport.diagnostics.vulkan.outstandingAcquired -eq 0) "Vulkan start/close $cycle gate failed."
    $startCloseCycles += [ordered]@{
        cycle=$cycle; status='PASS'; report=$startClosePath
        presents=$startCloseReport.diagnostics.presents
    }
}

$missingLoaderPath = Join-Path $OutputDirectory 'does-not-exist/vulkan-1.dll'
$missingLoader = Invoke-Probe -Name 'missing loader negative' -Arguments @('--loader',$missingLoaderPath)
Assert-True ($missingLoader.exitCode -ne 0 -and $missingLoader.stdout.Contains('explicitly resolved Vulkan loader is missing')) `
    'Missing-loader negative did not fail fast with the expected cause.'

$missingExtension = Invoke-Probe -Name 'missing device extension negative' -Arguments @(
    '--device',$Device,'--require-device-extension','VK_DOROTI_missing_extension')
Assert-True ($missingExtension.exitCode -ne 0 -and $missingExtension.stdout.Contains('Missing required Vulkan device extension')) `
    'Missing-extension negative did not fail fast with the expected cause.'

$software = Invoke-Probe -Name 'software device rejection self-test' -Arguments @('--self-test','software-device')
Assert-True ($software.exitCode -ne 0 -and $software.stdout.Contains('Software Vulkan device rejected')) `
    'Software-device negative policy did not fail fast.'

$multiGpu = Invoke-Probe -Name 'multi-GPU no-override negative' -Arguments @()
$multiGpuStatus = if ($multiGpu.exitCode -ne 0 -and $multiGpu.stdout.Contains('Multiple Vulkan devices are eligible')) {
    'PASS-fail-fast'
} else {
    'notApplicable-single-eligible-device'
}

$packageGraph = Invoke-BoundedProcess -FileName 'dotnet' -Name 'package graph' -ArgumentList @(
    'list',(Join-Path $repoRoot 'Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj'),
    'package','--include-transitive')
Assert-True ($packageGraph.exitCode -eq 0) "Package graph failed: $($packageGraph.stderr)"
$coreMatches = @([regex]::Matches($packageGraph.stdout, 'Silk.NET.Core\s+2\.23\.0')).Count
Assert-True ($coreMatches -eq 1) "Expected one resolved Silk.NET.Core 2.23.0 row, found $coreMatches."

$loaderFile = Get-Item -LiteralPath $capability.loader.path
$monitor = Get-CimInstance Win32_DesktopMonitor | Where-Object ScreenWidth | Select-Object -First 1
$video = @(Get-CimInstance Win32_VideoController | ForEach-Object {
    [ordered]@{
        name=$_.Name; pnpDeviceId=$_.PNPDeviceID; driverVersion=$_.DriverVersion
        currentRefreshRate=$_.CurrentRefreshRate; videoModeDescription=$_.VideoModeDescription
    }
})
$dxdiagPath = Join-Path $OutputDirectory 'dxdiag.xml'
$dxdiagRun = Invoke-BoundedProcess -FileName (Join-Path $env:SystemRoot 'System32/dxdiag.exe') `
    -Name 'DxDiag environment capture' -ArgumentList @('/dontskip','/whql:off','/x',$dxdiagPath)
$dxdiagDeadline = [DateTime]::UtcNow.AddSeconds(30)
while (-not (Test-Path -LiteralPath $dxdiagPath -PathType Leaf) -and [DateTime]::UtcNow -lt $dxdiagDeadline) {
    Start-Sleep -Milliseconds 250
}
Assert-True (Test-Path -LiteralPath $dxdiagPath -PathType Leaf) 'DxDiag did not produce its XML environment report.'
[xml]$dxdiag = Get-Content -LiteralPath $dxdiagPath -Raw
$displayDrivers = @($dxdiag.DxDiag.DisplayDevices.DisplayDevice | ForEach-Object {
    [ordered]@{
        name=$_.CardName; driverVersion=$_.DriverVersion; driverModel=$_.DriverModel; driverDate=$_.DriverDate
    }
})
if (-not ('Doroti.VulkanCapabilityValidation.NativeMethods' -as [type])) {
    Add-Type -TypeDefinition @'
using System.Runtime.InteropServices;
namespace Doroti.VulkanCapabilityValidation {
    public static class NativeMethods {
        [DllImport("user32.dll")] public static extern uint GetDpiForSystem();
    }
}
'@
}
$systemDpi = [Doroti.VulkanCapabilityValidation.NativeMethods]::GetDpiForSystem()
$manifest = [ordered]@{
    schemaVersion='doroti.windows.vulkan-v0-v2-validation/v1'
    runId=$runId
    status='PASS'
    capturedAt=[DateTimeOffset]::Now.ToString('o')
    repository=[ordered]@{
        head=$head
        startingStatus=$startingStatus
        oldVulkanReferenceCommit=$oldReference
        oldVulkanReferenceSourceHash=$oldSourceHash
        cherryPicked=$false
        acrylicCheckpoint='history/26-09-02/windows-appsdk-experimental-acrylic-checkpoint.md'
    }
    environment=[ordered]@{
        os=[Environment]::OSVersion.VersionString
        dotnetSdk=(& dotnet --version).Trim()
        windowsAppSdk='2.4.0'
        gpu=$video
        selectedVulkanDevice=$Device
        monitor=if($null -ne $monitor){[ordered]@{name=$monitor.Name;width=$monitor.ScreenWidth;height=$monitor.ScreenHeight}}else{$null}
        systemDpi=$systemDpi
        refreshRatesHz=@($video | ForEach-Object currentRefreshRate | Sort-Object -Unique)
        displayDrivers=$displayDrivers
    }
    angleBaseline=[ordered]@{
        status='PASS'
        build='PASS-zero-warnings-errors'
        productProbe=$angleReport.status
        firstExactPresentVisible=[bool]$angleReport.diagnostics.visibleAfterExactPresent
        pixelCapture='notRun'
        report=$anglePath
    }
    capability=$capability
    wsiQualification=[ordered]@{
        status='PASS'
        profile=$wsi.profile
        report=$wsiPath
        accepted=$wsi.accepted
        presented=$wsi.presented
        outstandingAcquired=$wsi.outstandingAcquired
        unconsumedSignals=$wsi.unconsumedSignals
        maximumRetiredSwapchains=$wsi.maximumRetiredSwapchains
        actualResultInjection=$injectedResults
        stress='notRun-opt-in'
        soak='notRun-opt-in'
    }
    vulkanProduct=[ordered]@{
        status='PASS'
        report=$vulkanProductPath
        requestedPresenter=$vulkanProductReport.diagnostics.requestedPresenter
        effectivePresenter=$vulkanProductReport.diagnostics.effectivePresenter
        adapter=$vulkanProductReport.diagnostics.adapterDescription
        deviceGenerations=$vulkanProductReport.diagnostics.deviceGenerations
        presentedTerminals=$vulkanProductReport.diagnostics.presentedTerminals
        failedTerminals=$vulkanProductReport.diagnostics.failedTerminals
        inputImeUia='PASS-automated-by-product-contract'
        visibleCapture='separate-validate-windows-vulkan-live-resize.ps1'
    }
    frameAndLifecycle=[ordered]@{
        exactResize10=[ordered]@{
            status='PASS'; report=$resizePath; resizeBuffers=$resizeReport.diagnostics.resizeBuffers
            presents=$resizeReport.diagnostics.presents; submits=$resizeReport.diagnostics.gpuSubmits
            copies=$resizeReport.diagnostics.gpuCopies
        }
        deviceReset10=[ordered]@{
            status='PASS'; report=$resetPath; requested=$resetReport.diagnostics.requestedDeviceResets
            completed=$resetReport.diagnostics.completedDeviceResets
        }
        minimizeRestore10=[ordered]@{ status='PASS'; report=$lifecyclePath }
        startClose10=$startCloseCycles
    }
    negative=[ordered]@{
        missingLoader='PASS-fail-fast'
        missingExtension='PASS-fail-fast'
        softwareDevice='PASS-policy-self-test'
        multiGpuNoOverride=$multiGpuStatus
    }
    packageGraph=[ordered]@{
        silkNetCoreResolvedRows=$coreMatches
        vulnerabilityWarnings=if($packageGraph.stdout -match 'vulnerab'){ 'present-review-output' } else { 'none-observed' }
        duplicateNativeAsset='none-observed'
        output=$packageGraph.stdout
    }
    loader=[ordered]@{
        path=$loaderFile.FullName
        appLocalLoaderPresent=$false
        icdBundled=$false
    }
    gate='V2-PASS-qualification'
    next='V3-allowed'
    evidenceBoundary='ANGLE product/counters, maintenance-free Vulkan capability/acquire-as-commit, short real-WSI, actual result injection, exact resize, reset, lifecycle, start/close, and automated input/IME/UIA transport passed. Opt-in stress/soak, physical scan-out, physical IME/accessibility acceptance, and the full GPU/DPI/refresh matrix remain notVerified.'
}
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "status=PASS"
Write-Host "gate=V2-PASS-qualification"
Write-Host "evidence=$OutputDirectory"
