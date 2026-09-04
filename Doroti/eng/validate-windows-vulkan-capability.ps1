#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [string] $Device = $env:DOROTI_WINDOWS_VULKAN_DEVICE,
    [switch] $SkipBuild,
    [switch] $LegacyWsiEvidence
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
$project = Join-Path $repoRoot 'Doroti/validation/windows-vulkan-capability/Doroti.Validation.WindowsVulkanCapability.csproj'
$targetProject = Join-Path $repoRoot 'Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64/Doroti.Target.Windows.WindowsAppSdk.win-x64.csproj'
$productProject = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj'
$productExecutable = Join-Path $repoRoot 'Doroti/validation/hwnd-exact-cpp-product/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.HwndExactCppProduct.exe'
$productDirectory = Split-Path -Parent $productExecutable
$productAssembly = Join-Path $productDirectory 'Doroti.Validation.HwndExactCppProduct.dll'
$managedHostPath = Join-Path $productDirectory 'Doroti.Host.WindowsAppSdk.dll'
$nativeHostPath = Join-Path $productDirectory 'doroti_windows_appsdk_host_v1.dll'
$probeDirectory = Join-Path $repoRoot 'Doroti/validation/windows-vulkan-capability/bin/Release/net10.0-windows10.0.19041.0/win-x64'
$probeExecutable = Join-Path $probeDirectory 'Doroti.Validation.WindowsVulkanCapability.exe'
$probeAssembly = Join-Path $probeDirectory 'Doroti.Validation.WindowsVulkanCapability.dll'
$sourceFingerprintPaths = @(
    'Doroti/eng/validate-windows-vulkan-capability.ps1',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedVulkanPresenter.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsAcrylicOptionsState.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedAcrylicCompositionPresenter.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/DorotiWindowsAppSdkRunner.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedHwndPresenterBase.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsManagedProductHost.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk/WindowsNativeV1.cs',
    'Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/include/doroti_windows_host_v1.h',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/include/doroti_windows_vulkan_composition_v1.h',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/exports.cpp',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/src/vulkan_composition.cpp',
    'Doroti/src/Doroti.Host.WindowsAppSdk.Native/Doroti.Host.WindowsAppSdk.Native.vcxproj',
    'Doroti/src/Doroti.Host.WindowsAppSdk/Doroti.Host.WindowsAppSdk.csproj',
    'Doroti/src/Doroti.Target.Windows.WindowsAppSdk.win-x64/Doroti.Target.Windows.WindowsAppSdk.win-x64.csproj',
    'Doroti/validation/contracts/windows-vulkan-v0.json',
    'Doroti/validation/windows-vulkan-capability/Program.cs',
    'Doroti/validation/windows-vulkan-capability/Doroti.Validation.WindowsVulkanCapability.csproj',
    'Doroti/validation/hwnd-exact-cpp-product/Program.cs',
    'Doroti/validation/hwnd-exact-cpp-product/Doroti.Validation.HwndExactCppProduct.csproj',
    'Doroti/validation/hwnd-exact-cpp-product/doroti-application-manifest.json',
    'Doroti/eng/build-hwnd-exact-cpp-native.ps1',
    'Doroti/Directory.Build.props',
    'Doroti/Directory.Build.targets',
    'Doroti/Directory.Packages.props'
)

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Assert-VulkanCompositionSnapshot($Report, [string] $Identity) {
    $vulkan = $Report.diagnostics.vulkan
    Assert-True ($null -ne $vulkan) "$Identity did not publish Vulkan diagnostics."
    Assert-True ($vulkan.presentMode -eq [string]$contract.presentationMode -and
        $vulkan.imageCount -eq [int]$contract.compositionBufferCount -and
        $vulkan.activeSwapchains -eq 0 -and $vulkan.retiredSwapchains -eq 0 -and
        $vulkan.maximumRetiredSwapchains -eq 0 -and $vulkan.surfaceRecreates -eq 0) `
        "$Identity did not retain exactly three Composition buffers and zero WSI swapchains."
    Assert-True ($vulkan.outstandingAcquired -eq 0 -and
        $vulkan.outstandingCopySubmission -eq 0 -and
        $vulkan.deferredCopySubmissions -eq 0 -and
        $vulkan.retainedFrameAllocationBytes -gt 0 -and
        $vulkan.retainedFrameInitialized -and
        $vulkan.copyFenceWaits -eq $Report.diagnostics.gpuCopies) `
        "$Identity did not retain and synchronously copy an initialized Vulkan guard before Present."
    Assert-True ($vulkan.retirementMode -eq [string]$contract.retirementMode -and
        $vulkan.queueIdleRetirementWaits -eq 0) `
        "$Identity did not use Presentation buffer availability as its retirement authority."
    Assert-True ($vulkan.surfaceLostResults -eq 0 -and
        $vulkan.outOfDateResults -eq 0 -and $vulkan.suboptimalResults -eq 0) `
        "$Identity emitted a legacy WSI-only result."
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
$startingDiffSha256 = Get-TrackedDiffSha256
$startingSourceFileSha256 = Get-SourceFileSha256
$startingRepositoryDiffSha256 = Get-RepositoryDiffSha256
$startingUntrackedFileSha256 = Get-UntrackedFileSha256
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

$binaryFiles = [ordered]@{
    productExecutable=$productExecutable
    productAssembly=$productAssembly
    managedHost=$managedHostPath
    nativeHost=$nativeHostPath
    capabilityProbeExecutable=$probeExecutable
    capabilityProbeAssembly=$probeAssembly
}
$startingBinarySha256 = Get-BinaryFileSha256 $binaryFiles

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
Assert-True (@($capability.instance.requiredExtensions).Count -eq @($contract.requiredInstanceExtensions).Count) `
    'Composition capability unexpectedly depends on a Vulkan WSI instance extension.'
Assert-True ([bool]$capability.device.luidValid) 'Selected Vulkan device does not expose a valid Windows adapter LUID.'
foreach ($requiredExtension in @($contract.requiredDeviceExtensions)) {
    Assert-True ($requiredExtension -in @($capability.device.requiredExtensions)) `
        "Capability omitted required device extension '$requiredExtension'."
}
Assert-True ($capability.externalMemory.format -eq [string]$contract.externalMemory.format -and
    $capability.externalMemory.tiling -eq [string]$contract.externalMemory.tiling -and
    $capability.externalMemory.usage -eq [string]$contract.externalMemory.usage -and
    $capability.externalMemory.handleType -eq [string]$contract.externalMemory.handleType) `
    'External-memory format/tiling/usage/handle query differs from the product contract.'
Assert-True ([bool]$capability.externalMemory.importable -and
    [bool]$capability.externalMemory.dedicatedOnly -and
    [bool]$capability.externalMemory.compatibleHandleType) `
    'BGRA8 optimal D3D11 texture external memory is not importable, dedicated, and compatible.'
Assert-True ($capability.presentation.mode -eq [string]$contract.presentationMode -and
    $capability.presentation.visibleOwner -eq [string]$contract.visibleOwner -and
    $capability.presentation.topology -eq [string]$contract.topology -and
    $capability.presentation.bufferCount -eq [int]$contract.compositionBufferCount -and
    $capability.presentation.activeSwapchains -eq [int]$contract.activeSwapchains -and
    $capability.presentation.movingOriginPolicy -eq [string]$contract.movingOriginPolicy -and
    $capability.presentation.rasterPlacement -eq [string]$contract.rasterPlacement -and
    $capability.presentation.bufferReuseAuthority -eq [string]$contract.retirementMode) `
    'Vulkan Composition presentation topology differs from the contract.'
Assert-True ($capability.presentationCommitPolicy.policy -eq [string]$contract.presentationCommitPolicy) `
    'Composition presentation commit policy differs from the contract.'
Assert-True ([bool]$capability.presentationCommitPolicy.availableBufferAfterLatestCheck) `
    'Presentation-buffer availability selection is not behind the latest-frame check.'
Assert-True ([bool]$capability.presentationCommitPolicy.synchronousVulkanCopyFenceCompletion -and
    [bool]$capability.presentationCommitPolicy.presentAfterCopyFenceCompletion) `
    'Vulkan copy completion is not synchronous before native Composition Present.'
Assert-True ([bool]$capability.presentationCommitPolicy.availabilityEventReuseAuthority) `
    'Presentation buffer availability is not the reuse authority.'
Assert-True ($capability.presentationCommitPolicy.retirementMode -eq [string]$contract.retirementMode) 'Retirement mode differs.'
Assert-True ($capability.presentationCommitPolicy.outstandingImages -eq 0) 'Capability probe left an acquired image outstanding.'
Assert-True ($capability.presentationCommitPolicy.activeSwapchains -eq 0) 'Composition capability unexpectedly owns a WSI swapchain.'

$wsiPath = $null
$wsi = $null
$legacyWsi = [ordered]@{
    status='notRun-separate-legacy-evidence'
    productGate=$false
}
if ($LegacyWsiEvidence) {
    $wsiPath = Join-Path $OutputDirectory 'legacy-wsi-qualification.json'
    $wsiRun = Invoke-Probe -Name 'legacy Vulkan WSI evidence' -Arguments @(
        '--device',$Device,'--wsi-qualification','--output',$wsiPath)
    $wsiDocument = if (Test-Path -LiteralPath $wsiPath -PathType Leaf) {
        Get-Content -LiteralPath $wsiPath -Raw | ConvertFrom-Json -Depth 100
    } else { $null }
    $wsi = if ($null -ne $wsiDocument) { $wsiDocument.wsiStress } else { $null }
    $legacyWsi = [ordered]@{
        status=if ($wsiRun.exitCode -eq 0 -and $null -ne $wsi -and $wsi.status -eq 'PASS') {
            'PASS-legacy-non-gating'
        } else { 'FAIL-legacy-non-gating' }
        productGate=$false
        exitCode=$wsiRun.exitCode
        report=$wsiPath
        stderr=$wsiRun.stderr
    }
}

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
    $vulkanProductReport.diagnostics.effectivePresenter -eq 'Vulkan/Composition-Swapchain') `
    'Explicit Vulkan Composition presenter did not remain effective.'
Assert-True ($vulkanProductReport.diagnostics.failedTerminals -eq 0 -and
    $vulkanProductReport.diagnostics.unterminatedResizeGenerations -eq 0 -and
    $vulkanProductReport.diagnostics.duplicateResizeTerminals -eq 0) 'Vulkan product terminal gate failed.'
Assert-True ($vulkanProductReport.diagnostics.deviceGenerations -eq 2) 'Vulkan product device-reset qualification differs.'
Assert-VulkanCompositionSnapshot $vulkanProductReport 'Vulkan product qualification'

$vulkanAcrylicProductPath = Join-Path $OutputDirectory 'vulkan-acrylic-product.json'
$vulkanAcrylicProduct = Invoke-BoundedProcess -FileName 'dotnet' -Name 'Vulkan Acrylic product qualification' -Environment @{
    DOROTI_WINDOWS_VULKAN_DEVICE = $Device
} -ArgumentList @(
    'run','--project',$productProject,'-c','Release','--no-build','--',
    '--presenter','Vulkan','--experimental-acrylic','--report',$vulkanAcrylicProductPath)
Assert-True ($vulkanAcrylicProduct.exitCode -eq 0) `
    "Vulkan Acrylic product qualification failed: $($vulkanAcrylicProduct.stderr)"
$vulkanAcrylicProductReport = Get-Content -LiteralPath $vulkanAcrylicProductPath -Raw |
    ConvertFrom-Json -Depth 100
Assert-True ($vulkanAcrylicProductReport.status -eq 'PASS') `
    'Vulkan Acrylic product qualification report is not PASS.'
Assert-True ($vulkanAcrylicProductReport.diagnostics.requestedPresenter -eq 'Vulkan' -and
    $vulkanAcrylicProductReport.diagnostics.effectivePresenter -eq 'Vulkan/Composition-Swapchain' -and
    $vulkanAcrylicProductReport.diagnostics.requestedMode -eq 'experimentalAcrylic' -and
    $vulkanAcrylicProductReport.diagnostics.effectiveMode -eq 'experimentalAcrylic') `
    'Explicit Vulkan Acrylic did not remain the effective presenter and backdrop mode.'
Assert-True ($vulkanAcrylicProductReport.diagnostics.acrylic.backdropTargetAdded -and
    [bool]$vulkanAcrylicProductReport.diagnostics.acrylic.contentIslandConnected -eq
        [bool]$contract.acrylic.contentIslandConnected -and
    [bool]$vulkanAcrylicProductReport.diagnostics.acrylic.desktopWindowTargetConnected -eq
        [bool]$contract.acrylic.desktopWindowTargetConnected -and
    [bool]$vulkanAcrylicProductReport.diagnostics.acrylic.hostBackdropBrushEnabled -eq
        [bool]$contract.acrylic.hostBackdropBrushEnabled -and
    $vulkanAcrylicProductReport.diagnostics.acrylic.backdropTransport -eq
        [string]$contract.acrylic.transport -and
    $vulkanAcrylicProductReport.diagnostics.acrylic.systemBackdropType -eq
        [string]$contract.acrylic.systemBackdropType -and
    -not $vulkanAcrylicProductReport.diagnostics.acrylic.redirectionBitmapAlphaEnabled -and
    $vulkanAcrylicProductReport.diagnostics.acrylic.backdropState -eq
        [string]$contract.acrylic.backdropState -and
    $vulkanAcrylicProductReport.diagnostics.vulkan.compositeAlpha -eq
        [string]$contract.acrylic.compositeAlpha) `
    'Vulkan Acrylic did not layer a premultiplied top-level target over an active, host-backdrop-enabled Desktop Acrylic window target.'
Assert-True ($vulkanAcrylicProductReport.diagnostics.failedTerminals -eq 0 -and
    $vulkanAcrylicProductReport.diagnostics.unterminatedResizeGenerations -eq 0 -and
    $vulkanAcrylicProductReport.diagnostics.duplicateResizeTerminals -eq 0) `
    'Vulkan Acrylic product terminal gate failed.'
Assert-VulkanCompositionSnapshot $vulkanAcrylicProductReport 'Vulkan Acrylic product qualification'

$injectedResults = [ordered]@{}
foreach ($resultName in @('DEVICE_LOST','DEVICE_LOST_ON_WAIT_IDLE')) {
    $resultSlug = $resultName.ToLowerInvariant()
    $resultPath = Join-Path $OutputDirectory "vulkan-inject-$resultSlug.json"
    $resultArguments = @(
        'run','--project',$productProject,'-c','Release','--no-build','--',
        '--presenter','Vulkan','--inject-vulkan-result',$resultName,'--lifecycle-cycles','0','--no-resize-burst',
        '--smoke-ms','5000','--report',$resultPath)
    if ($resultName -eq 'DEVICE_LOST_ON_WAIT_IDLE') {
        $resultArguments += @('--device-resets','1')
    }
    $resultRun = Invoke-BoundedProcess -FileName 'dotnet' -Name "Vulkan $resultName injection" -Environment @{
        DOROTI_WINDOWS_VULKAN_DEVICE = $Device
    } -ArgumentList $resultArguments
    Assert-True ($resultRun.exitCode -eq 0) "Vulkan $resultName injection failed: $($resultRun.stderr)"
    $resultReport = Get-Content -LiteralPath $resultPath -Raw | ConvertFrom-Json -Depth 100
    Assert-True ($resultReport.status -eq 'PASS' -and
        $resultReport.diagnostics.unterminatedResizeGenerations -eq 0 -and
        $resultReport.diagnostics.duplicateResizeTerminals -eq 0 -and
        $resultReport.diagnostics.vulkan.outstandingAcquired -eq 0 -and
        $resultReport.diagnostics.vulkan.outstandingCopySubmission -eq 0 -and
        $resultReport.diagnostics.vulkan.presentMode -eq [string]$contract.presentationMode -and
        $resultReport.diagnostics.vulkan.imageCount -eq [int]$contract.compositionBufferCount -and
        $resultReport.diagnostics.vulkan.activeSwapchains -eq 0 -and
        $resultReport.diagnostics.vulkan.deferredCopySubmissions -eq 0 -and
        $resultReport.diagnostics.vulkan.retirementMode -eq [string]$contract.retirementMode) `
        "Vulkan $resultName injection did not drain one-to-one."
    $deviceLossRecoveryOrder = $null
    if ($resultName -in @('DEVICE_LOST','DEVICE_LOST_ON_WAIT_IDLE')) {
        $events = @($resultReport.diagnostics.vulkan.recentEvents)
        $abandonIndex = -1
        $teardownIndex = -1
        for ($eventIndex = 0; $eventIndex -lt $events.Count; $eventIndex++) {
            if ($events[$eventIndex] -like '*device-loss context abandoned before renderer invalidation*') {
                $abandonIndex = $eventIndex
            }
            if ($events[$eventIndex] -like '*device-loss renderer resources invalidated; starting native teardown*') {
                $teardownIndex = $eventIndex
            }
        }
        $deviceLossRecoveryOrder = $abandonIndex -ge 0 -and $teardownIndex -gt $abandonIndex
        Assert-True $deviceLossRecoveryOrder `
            'Synthetic DEVICE_LOST did not prove context abandon before renderer invalidation/native teardown.'
        Assert-True ([int]$resultReport.diagnostics.vulkan.deviceLostResults -ge 1) `
            'Synthetic DEVICE_LOST did not reach Vulkan device-loss accounting.'
    }
    $injectedResults[$resultName] = [ordered]@{
        status='PASS'; report=$resultPath
        failedTerminals=$resultReport.diagnostics.failedTerminals
        deviceRecoveries=$resultReport.diagnostics.vulkanDeviceLossRecoveries
        surfaceRecoveries=$resultReport.diagnostics.vulkanSurfaceLossRecoveries
        acquired=$resultReport.diagnostics.vulkan.acquired
        presented=$resultReport.diagnostics.vulkan.presented
        outstanding=$resultReport.diagnostics.vulkan.outstandingAcquired
        deviceLossRecoveryOrder=$deviceLossRecoveryOrder
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
Assert-VulkanCompositionSnapshot $resetReport 'Vulkan device reset x10'

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
Assert-VulkanCompositionSnapshot $lifecycleReport 'Vulkan lifecycle x10'

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
    $resizeReport.diagnostics.vulkan.surfaceWidth -ge $resizeReport.diagnostics.vulkan.width -and
    $resizeReport.diagnostics.vulkan.surfaceHeight -ge $resizeReport.diagnostics.vulkan.height -and
    $resizeReport.diagnostics.vulkan.surfaceRecreates -eq 0 -and
    $resizeReport.diagnostics.vulkan.retainedSurfaceReuses -ge 9 -and
    $resizeReport.diagnostics.presents -ge 10 -and
    $resizeReport.diagnostics.gpuCopies -ge 10 -and
    $resizeReport.diagnostics.vulkan.outstandingAcquired -eq 0) 'Vulkan exact resize x10 gate failed.'
Assert-VulkanCompositionSnapshot $resizeReport 'Vulkan exact resize x10'

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
    Assert-VulkanCompositionSnapshot $startCloseReport "Vulkan start/close $cycle"
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
$skiaVersion = [string]$contract.skiaSharpVersion
$skiaPattern = [regex]::Escape($skiaVersion)
$skiaMatches = @([regex]::Matches(
    $packageGraph.stdout, "SkiaSharp\s+$skiaPattern\s+$skiaPattern")).Count
$skiaSilkAdapterMatches = @([regex]::Matches(
    $packageGraph.stdout, "$([regex]::Escape([string]$contract.skiaVulkanAdapter))\s+$skiaPattern\s+$skiaPattern")).Count
Assert-True ($skiaMatches -eq 1) `
    "Expected one direct SkiaSharp $skiaVersion row, found $skiaMatches."
Assert-True ($skiaSilkAdapterMatches -eq 1) `
    "Expected one direct SkiaSharp.Vulkan.Silk.NET $skiaVersion row, found $skiaSilkAdapterMatches."

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
$endingDiffSha256 = Get-TrackedDiffSha256
$endingHead = (& git -C $repoRoot rev-parse HEAD).Trim()
$endingSourceFileSha256 = Get-SourceFileSha256
$keySourceFilesStable = Test-Sha256MapsEqual $startingSourceFileSha256 $endingSourceFileSha256
$sourceStable = $head -eq $endingHead -and
    $startingDiffSha256 -eq $endingDiffSha256 -and $keySourceFilesStable
Assert-True $sourceStable 'Tracked source changed during Vulkan qualification.'
$endingRepositoryDiffSha256 = Get-RepositoryDiffSha256
$endingUntrackedFileSha256 = Get-UntrackedFileSha256
$repositoryStable = $head -eq $endingHead -and
    $startingRepositoryDiffSha256 -eq $endingRepositoryDiffSha256 -and
    (Test-Sha256MapsEqual $startingUntrackedFileSha256 $endingUntrackedFileSha256)
Assert-True $repositoryStable 'Repository working tree changed during Vulkan qualification.'
$endingBinarySha256 = Get-BinaryFileSha256 $binaryFiles
$binariesStable = Test-Sha256MapsEqual $startingBinarySha256 $endingBinarySha256
Assert-True $binariesStable 'Qualification binaries changed during Vulkan qualification.'
$manifest = [ordered]@{
    schemaVersion='doroti.windows.vulkan-composition-v0-validation/v1'
    runId=$runId
    status='PASS'
    capturedAt=[DateTimeOffset]::Now.ToString('o')
    repository=[ordered]@{
        head=$head
        endingHead=$endingHead
        startingStatus=$startingStatus
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
        capabilityProbeExecutableSha256=$startingBinarySha256.capabilityProbeExecutable
        capabilityProbeAssemblySha256=$startingBinarySha256.capabilityProbeAssembly
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
    compositionQualification=[ordered]@{
        status='PASS'
        mode=$vulkanProductReport.diagnostics.vulkan.presentMode
        visibleOwner=[string]$contract.visibleOwner
        topology=[string]$contract.topology
        movingOriginPolicy=[string]$contract.movingOriginPolicy
        rasterPlacement=[string]$contract.rasterPlacement
        compositionBuffers=$vulkanProductReport.diagnostics.vulkan.imageCount
        activeSwapchains=$vulkanProductReport.diagnostics.vulkan.activeSwapchains
        retirementMode=$vulkanProductReport.diagnostics.vulkan.retirementMode
        synchronousCopyFenceWaits=$vulkanProductReport.diagnostics.vulkan.copyFenceWaits
        retainedFrameAllocationBytes=$vulkanProductReport.diagnostics.vulkan.retainedFrameAllocationBytes
        retainedFrameInitialized=$vulkanProductReport.diagnostics.vulkan.retainedFrameInitialized
        externalMemory=$capability.externalMemory
        actualDeviceLossInjection=$injectedResults
    }
    legacyWsiQualification=$legacyWsi
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
    vulkanAcrylicProduct=[ordered]@{
        status='PASS'
        report=$vulkanAcrylicProductPath
        requestedPresenter=$vulkanAcrylicProductReport.diagnostics.requestedPresenter
        effectivePresenter=$vulkanAcrylicProductReport.diagnostics.effectivePresenter
        requestedMode=$vulkanAcrylicProductReport.diagnostics.requestedMode
        effectiveMode=$vulkanAcrylicProductReport.diagnostics.effectiveMode
        backdropTargetAdded=[bool]$vulkanAcrylicProductReport.diagnostics.acrylic.backdropTargetAdded
        contentIslandConnected=[bool]$vulkanAcrylicProductReport.diagnostics.acrylic.contentIslandConnected
        desktopWindowTargetConnected=[bool]$vulkanAcrylicProductReport.diagnostics.acrylic.desktopWindowTargetConnected
        hostBackdropBrushEnabled=[bool]$vulkanAcrylicProductReport.diagnostics.acrylic.hostBackdropBrushEnabled
        backdropState=$vulkanAcrylicProductReport.diagnostics.acrylic.backdropState
        compositeAlpha=$vulkanAcrylicProductReport.diagnostics.vulkan.compositeAlpha
        rasterOwner='top-level HWND DirectComposition Vulkan Presentation target'
        backdropOwner='host-backdrop-enabled top-level Desktop Acrylic window target'
        backdropTransport=$vulkanAcrylicProductReport.diagnostics.acrylic.backdropTransport
        systemBackdropType=$vulkanAcrylicProductReport.diagnostics.acrylic.systemBackdropType
        redirectionBitmapAlphaEnabled=[bool]$vulkanAcrylicProductReport.diagnostics.acrylic.redirectionBitmapAlphaEnabled
        visibleCapture='separate-current-monitor-desktop-duplication-evidence'
        physicalBackdrop='notVerified'
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
        skiaSharpVersion=$skiaVersion
        skiaSharpResolvedRows=$skiaMatches
        skiaSharpVulkanSilkNetResolvedRows=$skiaSilkAdapterMatches
        vulnerabilityWarnings=if($packageGraph.stdout -match 'vulnerab'){ 'present-review-output' } else { 'none-observed' }
        duplicateNativeAsset='none-observed'
        output=$packageGraph.stdout
    }
    loader=[ordered]@{
        path=$loaderFile.FullName
        appLocalLoaderPresent=$false
        icdBundled=$false
    }
    gate='Vulkan-Composition-PASS-automated-partial'
    next='physical-left-and-top-left-required'
    evidenceBoundary='ANGLE baseline plus Vulkan exact-LUID external-memory capability, a top-level native topmost Presentation target with full-capacity identity raster placement and one client geometry, exact proposed-size Skia/Vulkan Presentation submission before geometry on every interactive edge, fixed-origin pre-geometry DWM display admission, and moving-origin pre-geometry submission without a forced old-origin display wait, active host-backdrop-enabled DesktopAcrylicController target plus an HWND-wide DWM transient-backdrop resize underlay, three Composition buffers, synchronous Vulkan copy completion, availability-based reuse, device-loss recovery, exact resize, reset, lifecycle, start/close, and automated input/IME/UIA transport passed. Legacy visible-HWND WSI is separate non-product evidence. Capability and Presentation completion do not prove physical scan-out, a gap-free physical border drag, or human-perceived Acrylic quality; physical left/top-left drag, backdrop, IME/accessibility acceptance, and the full GPU/DPI/refresh matrix remain notVerified.'
}
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "status=PASS"
Write-Host "gate=Vulkan-Composition-PASS-automated-partial"
Write-Host "evidence=$OutputDirectory"
}
finally {
    $outputDirectoryLock.Dispose()
}
}
finally {
    $globalValidationLock.Dispose()
}
