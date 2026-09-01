#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $OutputDirectory,
    [switch] $SkipBuild
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$project = Join-Path $repoRoot 'Doroti/validation/windows-dwm-redirection-alpha-spike/Doroti.Validation.WindowsDwmRedirectionAlphaSpike.csproj'
$executable = Join-Path $repoRoot 'Doroti/validation/windows-dwm-redirection-alpha-spike/bin/Release/net10.0-windows10.0.19041.0/win-x64/Doroti.Validation.WindowsDwmRedirectionAlphaSpike.exe'
$contractPath = Join-Path $repoRoot 'Doroti/validation/contracts/windows-dwm-redirection-alpha-a1.json'
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json -Depth 32
$runId = 'acrylic-a1-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 12))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti/evidence/$runId"
}
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

function Invoke-BoundedProcess {
    param(
        [Parameter(Mandatory)][string] $FileName,
        [Parameter(Mandatory)][string[]] $ArgumentList,
        [Parameter(Mandatory)][string] $Name
    )
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $start.Environment['DOROTI_WINDOWS_DWM_FLUSH'] = '0'
    $start.Environment['DOROTI_WINDOWS_EGL_SWAP_INTERVAL'] = '1'
    foreach ($argument in $ArgumentList) { [void] $start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = $start
    if (-not $process.Start()) { throw "$Name failed to start." }
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
    return [ordered]@{ ExitCode = $process.ExitCode; Stdout = $stdout; Stderr = $stderr }
}

if (-not $SkipBuild) {
    $build = Invoke-BoundedProcess -FileName 'dotnet' -Name 'A1 spike build' -ArgumentList @(
        'build', $project, '-c', 'Release', '--nologo')
    if ($build.ExitCode -ne 0) { throw "A1 spike build failed with exit code $($build.ExitCode)." }
}
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    throw "A1 spike executable is missing: $executable"
}

$matrix = @(
    [ordered]@{ Name = 'opaque-off'; Arm = 'opaque'; Alpha = 'off'; ExpectedStatus = 'PASS' },
    [ordered]@{ Name = 'dwm-off'; Arm = 'dwm'; Alpha = 'off'; ExpectedStatus = 'PASS' },
    [ordered]@{ Name = 'dwm-top'; Arm = 'dwm'; Alpha = 'top'; ExpectedStatus = 'PASS' },
    [ordered]@{ Name = 'dwm-child'; Arm = 'dwm'; Alpha = 'child'; ExpectedStatus = 'FAIL' },
    [ordered]@{ Name = 'dwm-both'; Arm = 'dwm'; Alpha = 'both'; ExpectedStatus = 'FAIL' },
    [ordered]@{ Name = 'controller-off'; Arm = 'controller'; Alpha = 'off'; ExpectedStatus = 'PASS' },
    [ordered]@{ Name = 'controller-top'; Arm = 'controller'; Alpha = 'top'; ExpectedStatus = 'PASS' },
    [ordered]@{ Name = 'controller-child'; Arm = 'controller'; Alpha = 'child'; ExpectedStatus = 'FAIL' },
    [ordered]@{ Name = 'controller-both'; Arm = 'controller'; Alpha = 'both'; ExpectedStatus = 'FAIL' }
)
$results = @()
foreach ($case in $matrix) {
    $reportPath = Join-Path $OutputDirectory "$($case.Name).json"
    $arguments = @(
        '--arm', $case.Arm,
        '--alpha', $case.Alpha,
        '--automated',
        '--duration-ms', '100',
        '--report', $reportPath
    )
    if ($case.Arm -eq 'controller') { $arguments += @('--custom', '--kind', 'thin') }
    $process = Invoke-BoundedProcess -FileName $executable -Name "A1 $($case.Name)" -ArgumentList $arguments
    if (-not (Test-Path -LiteralPath $reportPath -PathType Leaf)) {
        throw "A1 case did not write a report: $($case.Name)"
    }
    $report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json -Depth 100
    $results += [ordered]@{
        name = $case.Name
        expectedStatus = $case.ExpectedStatus
        exitCode = $process.ExitCode
        report = $report
    }
}

$eHandle = -2147024890
$unexpected = @($results | Where-Object {
    $_.report.status -ne $_.expectedStatus -or
    ($_.expectedStatus -eq 'PASS' -and $_.exitCode -ne 0) -or
    ($_.expectedStatus -eq 'FAIL' -and $_.exitCode -eq 0)
})
$childCases = @($results | Where-Object { $_.report.alpha -in @('child', 'both') })
$childAlphaRejected = $childCases.Count -eq 4 -and @($childCases | Where-Object {
    @($_.report.dwmFailures | Where-Object {
        $_.attribute -eq $contract.dwmAttributes.redirectionBitmapAlpha -and $_.hResult -eq $eHandle
    }).Count -eq 0
}).Count -eq 0
$topCases = @($results | Where-Object { $_.report.alpha -eq 'top' })
$topAlphaAccepted = $topCases.Count -eq 2 -and @($topCases | Where-Object {
    @($_.report.dwmCalls | Where-Object {
        $_.attribute -eq $contract.dwmAttributes.redirectionBitmapAlpha -and $_.hResult -eq 0
    }).Count -ne 1
}).Count -eq 0
$controller = @($results | Where-Object { $_.name -eq 'controller-off' })[0].report
$controllerContractPass =
    $controller.setTargetResult -and
    $controller.setTargetCount -eq $contract.arms.controller.setTargetCalls -and
    $controller.controllerCreateCount -eq $contract.arms.controller.controllerCreates -and
    $controller.targetCreateCount -eq $contract.arms.controller.targetCreates -and
    $controller.rootCreateCount -eq $contract.arms.controller.rootCreates -and
    $controller.systemBackdropValue -eq 0 -and
    @($controller.dwmCalls | Where-Object attribute -eq $contract.dwmAttributes.systemBackdropType).Count -eq 0 -and
    $controller.updates.acceptedCount -eq 605 -and
    $controller.updates.invalidRejectedCount -eq 7 -and
    $controller.updates.maxPendingDepth -le 1 -and
    $controller.updates.duplicateTerminalCount -eq 0 -and
    $controller.updates.missingTerminalCount -eq 0 -and
    $controller.operationalGpuErrorCount -eq 0 -and
    $controller.exactMismatchCount -eq 0
$opaque = @($results | Where-Object { $_.name -eq 'opaque-off' })[0].report
$opaqueContractPass =
    $opaque.status -eq 'PASS' -and
    $opaque.dwmCalls.Count -eq 0 -and
    $opaque.controllerCreateCount -eq 0 -and
    $opaque.presentCount -gt 0 -and
    $opaque.presentCount -eq $opaque.gpuSubmitCount -and
    $opaque.presentCount -eq $opaque.gpuCopyCount

$validatorPass = $unexpected.Count -eq 0 -and $childAlphaRejected -and $topAlphaAccepted -and
    $controllerContractPass -and $opaqueContractPass
$manifestPath = Join-Path $OutputDirectory 'manifest.json'
$manifest = [ordered]@{
    schema = 'doroti.windows-dwm-redirection-alpha-a1-manifest/v1'
    runId = $runId
    status = if ($validatorPass) { 'PASS' } else { 'FAIL' }
    generatedAt = (Get-Date).ToUniversalTime().ToString('o')
    source = [ordered]@{
        revision = (& git -C $repoRoot rev-parse HEAD).Trim()
        dirtyFiles = @(& git -C $repoRoot status --short)
    }
    environment = [ordered]@{
        windowsBuild = $opaque.windowsBuild
        operatingSystem = $opaque.operatingSystem
        architecture = $opaque.processArchitecture
        windowsSdk = $contract.windowsSdk
        windowsAppSdk = $contract.windowsAppSdk
        dwmCompositionEnabled = $opaque.dwmCompositionEnabled
        transparencyPolicy = $opaque.transparencyPolicy
        highContrast = $opaque.highContrast
        remoteSession = $opaque.remoteSession
        dwmFlush = '0'
        eglSwapInterval = '1'
    }
    gates = [ordered]@{
        matrix = if ($unexpected.Count -eq 0) { 'PASS' } else { 'FAIL' }
        opaqueContract = if ($opaqueContractPass) { 'PASS' } else { 'FAIL' }
        controllerApiAndOptions = if ($controllerContractPass) { 'PASS' } else { 'FAIL' }
        topLevelRedirectionAlpha = if ($topAlphaAccepted) { 'PASS' } else { 'FAIL' }
        childRedirectionAlpha = if ($childAlphaRejected) { 'FAIL' } else { 'unexpected' }
        visibleCapture = 'notVerified'
        physicalBorderDrag = 'notVerified'
    }
    decision = [ordered]@{
        p0 = 'FAIL'
        reason = 'DWMWA_REDIRECTIONBITMAP_ALPHA rejects the app-owned render child HWND with E_HANDLE on Windows build 26200; top-level success cannot make child ANGLE pixels transparent to the backdrop.'
        proceedToA2 = $false
        proceedToB0 = $true
    }
    controller = $controller
    cases = $results
    scopeBoundary = $contract.scopeBoundary
}
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "A1 validator=$($manifest.status) P0=$($manifest.decision.p0) childAlpha=$($manifest.gates.childRedirectionAlpha) controller=$($manifest.gates.controllerApiAndOptions)"
Write-Host "manifest=$manifestPath"
if (-not $validatorPass) { exit 2 }
