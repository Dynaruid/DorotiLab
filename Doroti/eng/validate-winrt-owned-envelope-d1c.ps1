[CmdletBinding()]
param(
    [string]$OutputDirectory,
    [switch]$SkipBuild,
    [int]$HoldMilliseconds = 200
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$timeoutMilliseconds = 20 * 60 * 1000

function Invoke-BoundedProcess {
    param(
        [Parameter(Mandatory)][string]$FileName,
        [Parameter(Mandatory)][string[]]$ArgumentList,
        [Parameter(Mandatory)][string]$Name
    )

    $start = [System.Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.UseShellExecute = $false
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($argument in $ArgumentList) { [void]$start.ArgumentList.Add($argument) }
    $process = [System.Diagnostics.Process]::new()
    $process.StartInfo = $start
    if (-not $process.Start()) { throw "$Name failed to start." }
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit($timeoutMilliseconds)) {
        $process.Kill($true)
        throw "$Name exceeded the 20-minute test timeout."
    }
    $stdout = $stdoutTask.GetAwaiter().GetResult()
    $stderr = $stderrTask.GetAwaiter().GetResult()
    if (-not [string]::IsNullOrWhiteSpace($stdout)) { Write-Host $stdout.TrimEnd() }
    if (-not [string]::IsNullOrWhiteSpace($stderr)) { Write-Host $stderr.TrimEnd() }
    return $process.ExitCode
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if (-not (Test-Path -LiteralPath (Join-Path $repoRoot 'Doroti\Doroti.slnx'))) {
    throw "Unable to locate the DorotiLab repository root from $PSScriptRoot."
}
$projectPath = Join-Path $repoRoot 'Doroti\validation\winrt-owned-envelope-spike\Doroti.Validation.WinRtOwnedEnvelopeSpike.csproj'
$programPath = Join-Path $repoRoot 'Doroti\validation\winrt-owned-envelope-spike\OwnedEnvelopeProgram.cs'
$contractPath = Join-Path $repoRoot 'Doroti\validation\contracts\winrt-owned-envelope-d1c.json'
$sharedProgramPath = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-spike\Program.cs'
$binaryDirectory = Join-Path $repoRoot 'Doroti\validation\winrt-owned-envelope-spike\bin\Release\net10.0-windows10.0.19041.0\win-x64'
$executablePath = Join-Path $binaryDirectory 'Doroti.Validation.WinRtOwnedEnvelopeSpike.exe'
$managedAssemblyPath = Join-Path $binaryDirectory 'Doroti.Validation.WinRtOwnedEnvelopeSpike.dll'
$runId = 'd1c-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 12))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti\evidence\$runId"
}
$OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$reportPath = Join-Path $OutputDirectory 'd1c-report.json'
$manifestPath = Join-Path $OutputDirectory 'd1c-manifest.json'

$programSource = Get-Content -LiteralPath $programPath -Raw
$projectSource = Get-Content -LiteralPath $projectPath -Raw
$contract = Get-Content -LiteralPath $contractPath -Raw | ConvertFrom-Json -Depth 100
$structuralFailures = @()
if ($projectSource -match 'Doroti\.(Ui|Hosting|Host|Renderer)') {
    $structuralFailures += 'The independent D1-C target references Doroti product/framework code.'
}
if ($programSource -notmatch 'WindowStylePopup' -or
    $programSource -notmatch 'SetWindowRgn' -or
    $programSource -notmatch 'RemoveAll\(\)' -or
    $programSource -notmatch 'InsertAtTop\(_fronts\[slot\]\)') {
    $structuralFailures += 'The fixed-envelope, region, or atomic root-front ownership path is missing.'
}
if ($programSource -match 'SetWindowPos\(') {
    $structuralFailures += 'The D1-C target contains an outer HWND resize path.'
}
if ($programSource -match 'CreateWindowEx\([^\r\n]*WindowStyleChild') {
    $structuralFailures += 'The D1-C target creates an application child HWND.'
}
if ($contract.productImplementationAllowed) {
    $structuralFailures += 'The D1-C contract incorrectly permits product implementation.'
}
if ($structuralFailures.Count -ne 0) {
    $structuralFailures | ForEach-Object { Write-Error $_ }
    exit 2
}

if (-not $SkipBuild) {
    $buildExitCode = Invoke-BoundedProcess -FileName 'dotnet' -Name 'D1-C owned-envelope build' -ArgumentList @(
        'build', $projectPath, '-c', 'Release', '--nologo')
    if ($buildExitCode -ne 0) { throw "D1-C build failed with exit code $buildExitCode." }
}
if (-not (Test-Path -LiteralPath $executablePath)) {
    throw "D1-C executable is missing: $executablePath"
}
if (-not (Test-Path -LiteralPath $managedAssemblyPath)) {
    throw "D1-C managed assembly is missing: $managedAssemblyPath"
}

$spikeExitCode = Invoke-BoundedProcess -FileName $executablePath -Name 'D1-C automated contract' -ArgumentList @(
    '--automated', '--hold-ms', [string]$HoldMilliseconds, '--report', $reportPath)
if (-not (Test-Path -LiteralPath $reportPath)) {
    throw "D1-C spike did not write its report (exit code $spikeExitCode)."
}
$report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json -Depth 100
$contractPass = $spikeExitCode -eq 0 -and
    $report.schema -eq 'doroti.winrt-owned-envelope-d1c/v1' -and
    $report.contractStatus -eq 'PASS' -and
    $report.physicalStatus -eq 'notVerified' -and
    $report.topology.applicationCreatedChildHwndCount -eq 0 -and
    $report.topology.platformInternalChildHwndCount -eq 1 -and
    @($report.topology.platformInternalChildWindowClasses).Count -eq 1 -and
    $report.topology.platformInternalChildWindowClasses[0] -eq 'InputSiteWindowClass' -and
    $report.topology.contentIslandCount -eq 1 -and
    $report.topology.siteBridgeCount -eq 1 -and
    $report.topology.preparedSurfaceCount -eq 2 -and
    $report.topology.simultaneouslyVisibleSurfaceCount -eq 1 -and
    $report.topology.envelopeMode -eq 'fixed-work-area-ws-popup' -and
    $report.topology.windowStyle -eq '0x96000000' -and
    $report.presentation.targetGeneration -eq $report.presentation.visibleGeneration -and
    $report.presentation.targetGeneration -eq 236 -and
    $report.presentation.frontSwitchCount -gt 1 -and
    $report.presentation.duplicateVisibleFrontCount -eq 0 -and
    $report.presentation.geometryMismatchCount -eq 0 -and
    $report.presentation.fixedEnvelopeMismatchCount -eq 0 -and
    $report.inputRegion.fullRegionOpenCount -eq 1 -and
    $report.inputRegion.constrainedRegionApplyCount -ge 2 -and
    $report.inputRegion.insideHitPassCount -gt 0 -and
    $report.inputRegion.outsideHitPassCount -gt 0 -and
    -not $report.inputRegion.regionOpen -and
    $report.inputRegion.captureBeginCount -eq 1 -and
    $report.inputRegion.captureEndCount -eq 1 -and
    $report.inputRegion.captureLostCount -eq 0 -and
    $report.shellRisk.snapLayouts -eq 'notVerified' -and
    $report.shellRisk.taskbarPreview -eq 'notVerified' -and
    $report.shellRisk.maximizeRestore -eq 'notVerified' -and
    $report.shellRisk.uiaBounds -eq 'notVerified' -and
    @($report.failures).Count -eq 0

$sourceRevision = (& git -C $repoRoot rev-parse HEAD).Trim()
$dirtyFiles = @(& git -C $repoRoot status --short)
$manifest = [ordered]@{
    schema = 'doroti.winrt-owned-envelope-d1c-manifest/v1'
    runId = $runId
    status = if ($contractPass) { 'PASS' } else { 'FAIL' }
    generatedAt = (Get-Date).ToUniversalTime().ToString('o')
    source = [ordered]@{
        revision = $sourceRevision
        dirty = $dirtyFiles.Count -gt 0
        dirtyFiles = $dirtyFiles
        programSha256 = (Get-FileHash -LiteralPath $programPath -Algorithm SHA256).Hash.ToLowerInvariant()
        sharedPrimitiveSourceSha256 = (Get-FileHash -LiteralPath $sharedProgramPath -Algorithm SHA256).Hash.ToLowerInvariant()
        contractSha256 = (Get-FileHash -LiteralPath $contractPath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    binary = [ordered]@{
        path = [System.IO.Path]::GetRelativePath($repoRoot, $executablePath).Replace('\', '/')
        size = (Get-Item -LiteralPath $executablePath).Length
        sha256 = (Get-FileHash -LiteralPath $executablePath -Algorithm SHA256).Hash.ToLowerInvariant()
        managedAssemblyPath = [System.IO.Path]::GetRelativePath($repoRoot, $managedAssemblyPath).Replace('\', '/')
        managedAssemblySha256 = (Get-FileHash -LiteralPath $managedAssemblyPath -Algorithm SHA256).Hash.ToLowerInvariant()
    }
    structuralContract = [ordered]@{
        status = 'PASS'
        productFrameworkReferences = 0
        applicationCreatedChildHwndCount = 0
        platformInternalChildWindowClass = 'InputSiteWindowClass'
        outerHwndResizeCallsites = 0
        simultaneouslyVisibleSurfaceCount = 1
        inputRegionOwner = 'SetWindowRgn'
    }
    automatedReport = $report
    evidenceQualification = [ordered]@{
        automatedContract = if ($contractPass) { 'PASS' } else { 'FAIL' }
        physicalVisible = 'notVerified'
        outsideClickThrough = 'notVerified'
        postDragClickRecovery = 'notVerified'
        snapLayouts = 'notVerified'
        taskbarPreview = 'notVerified'
        maximizeRestore = 'notVerified'
        uiaBounds = 'notVerified'
        physicalKoreanIme = 'notVerified'
        productImplementation = 'notStarted'
    }
}
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "D1-C contract=$($manifest.status) physical=$($report.physicalStatus) exitCode=$spikeExitCode"
Write-Host "targets=$($report.presentation.targetGeneration) visible=$($report.presentation.visibleGeneration) switches=$($report.presentation.frontSwitchCount) abandoned=$($report.presentation.abandonedPreparedFrontCount)"
Write-Host "appChildren=$($report.topology.applicationCreatedChildHwndCount) platformChildren=$($report.topology.platformInternalChildHwndCount) region=$($report.inputRegion.insideHitPassCount)/$($report.inputRegion.outsideHitPassCount)"
Write-Host "report=$reportPath"
Write-Host "manifest=$manifestPath"
if (-not $contractPass) { exit 2 }
