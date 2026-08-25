[CmdletBinding()]
param(
    [string]$OutputDirectory,
    [switch]$SkipBuild,
    [int]$HoldMilliseconds = 250
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
$projectPath = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-spike\Doroti.Validation.WinRtContentIslandSpike.csproj'
$programPath = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-spike\Program.cs'
$executablePath = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-spike\bin\Release\net10.0-windows10.0.19041.0\win-x64\Doroti.Validation.WinRtContentIslandSpike.exe'
$managedAssemblyPath = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-spike\bin\Release\net10.0-windows10.0.19041.0\win-x64\Doroti.Validation.WinRtContentIslandSpike.dll'
$runId = 'w1r-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 12))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti\evidence\$runId"
}
$OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$reportPath = Join-Path $OutputDirectory 'w1r-report.json'
$manifestPath = Join-Path $OutputDirectory 'w1r-manifest.json'

$programSource = Get-Content -LiteralPath $programPath -Raw
$projectSource = Get-Content -LiteralPath $projectPath -Raw
$structuralFailures = @()
if ($projectSource -match 'Doroti\.(Ui|Hosting|Host|Renderer)') {
    $structuralFailures += 'The independent W1R target references Doroti product/framework code.'
}
if ($programSource -match 'Input(Pointer|Keyboard|PreTranslateKeyboard)Source\.GetForIsland|InputFocusController\.GetForIsland') {
    $structuralFailures += 'The system root island registers a forbidden Input*Source owner.'
}
if ($programSource -notmatch '_siteBridge\.ProcessesPointerInput = false' -or
    $programSource -notmatch '_siteBridge\.ProcessesKeyboardInput = true') {
    $structuralFailures += 'The desktop bridge does not explicitly disable pointer processing and preserve the keyboard runtime floor.'
}
if ($programSource -notmatch 'class WinRtTopLevelNativeIngress') {
    $structuralFailures += 'WinRtTopLevelNativeIngress is missing.'
}
if ($programSource -notmatch 'class WinRtNativeMessageTrace') {
    $structuralFailures += 'The independent WndProc native-message trace is missing.'
}
if ($programSource -match 'Native\.TranslateMessage' -or
    $programSource -notmatch 'DispatchTranslatedMessage') {
    $structuralFailures += 'TranslateMessage is not isolated behind the single native message-loop owner.'
}
if ($programSource -match 'CreateWindowEx\([^\r\n]*WsChild') {
    $structuralFailures += 'A child render/input HWND creation path was found.'
}
if ($structuralFailures.Count -ne 0) {
    $structuralFailures | ForEach-Object { Write-Error $_ }
    exit 2
}

if (-not $SkipBuild) {
    $buildExitCode = Invoke-BoundedProcess -FileName 'dotnet' -Name 'W1R spike build' -ArgumentList @(
        'build', $projectPath, '-c', 'Release', '--nologo')
    if ($buildExitCode -ne 0) { throw "W1R spike build failed with exit code $buildExitCode." }
}
if (-not (Test-Path -LiteralPath $executablePath)) {
    throw "W1R spike executable is missing: $executablePath"
}
if (-not (Test-Path -LiteralPath $managedAssemblyPath)) {
    throw "W1R managed assembly is missing: $managedAssemblyPath"
}
$spikeExitCode = Invoke-BoundedProcess -FileName $executablePath -Name 'W1R automated contract' -ArgumentList @(
    '--automated', '--hold-ms', [string]$HoldMilliseconds, '--report', $reportPath)
if (-not (Test-Path -LiteralPath $reportPath)) {
    throw "W1R spike did not write its report (exit code $spikeExitCode)."
}
$report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json -Depth 100
$contractPass = $spikeExitCode -eq 0 -and
    $report.schema -eq 'doroti.winrt-content-island-w1r/v3' -and
    $report.contractStatus -eq 'PASS' -and
    $report.visibleStatus -eq 'notVerified' -and
    $report.topology.applicationCreatedChildHwndCount -eq 0 -and
    $report.topology.platformInternalChildHwndCount -eq 1 -and
    @($report.topology.platformInternalChildWindowClasses).Count -eq 1 -and
    $report.topology.platformInternalChildWindowClasses[0] -eq 'InputSiteWindowClass' -and
    $report.topology.connectedIslandCount -eq 1 -and
    $report.topology.siteBridgeCount -eq 1 -and
    $report.topology.visibleSurfaceCount -eq 1 -and
    $report.input.pointerOwner -eq 'WinRtTopLevelNativeIngress' -and
    $report.input.keyboardOwner -eq 'WinRtTopLevelNativeIngress' -and
    $report.input.focusOwner -eq 'WinRtTopLevelNativeIngress' -and
    $report.input.cursorOwner -eq 'WinRtTopLevelNativeIngress' -and
    $report.input.automationRootOwner -eq 'WinRtTopLevelNativeIngress' -and
    $report.input.keyboardBridgeMode -eq 'bridge-processing-enabled-without-root-input-source-registration' -and
    $report.input.keyboardPacketProducer -eq 'top-level-wndproc-only-despite-bridge-runtime-floor' -and
    $report.input.textServiceOwner -eq 'WinRtTextInputAdapter-reserved' -and
    $report.input.physicalKoreanImeStatus -eq 'notVerified' -and
    -not $report.input.bridgeProcessesPointerInput -and
    $report.input.bridgeProcessesKeyboardInput -and
    $report.input.rootIslandInputSourceRegistrationCount -eq 0 -and
    $report.input.enableMouseInPointerCallCount -eq 1 -and
    $report.input.mouseCompatibilityPacketCount -eq 0 -and
    $report.input.duplicatePacketCount -eq 0 -and
    $report.input.rawKeyboardMessageCount -eq $report.input.keyboardPacketCount -and
    $report.input.nativeTraceKeyboardMessageCount -eq $report.input.rawKeyboardMessageCount -and
    $report.input.duplicateKeyboardPacketCount -eq 0 -and
    $report.input.keyboardDefWindowProcDelegationCount -eq 0 -and
    $report.input.imeMessageCount -eq 3 -and
    $report.input.imeDefWindowProcDelegationCount -eq $report.input.imeMessageCount -and
    $report.input.nativeTraceImeMessageCount -eq $report.input.imeMessageCount -and
    $report.input.imeStartCount -eq 1 -and
    $report.input.imeCompositionCount -eq 1 -and
    $report.input.imeEndCount -eq 1 -and
    $report.input.pointerTerminalMissingCount -eq 0 -and
    $report.input.keyTerminalMissingCount -eq 0 -and
    $report.input.focusTransitionCount -ge 2 -and
    $report.input.deadCharacterCount -eq 1 -and
    $report.input.surrogatePairCount -eq 1 -and
    $report.input.duplicateProviderRootCount -eq 0 -and
    $report.input.callbackWaitCount -eq 0 -and
    $report.input.fixturePassed -and
    $report.counters.duplicateTerminalCount -eq 0 -and
    $report.counters.missingTerminalCount -eq 0 -and
    $report.counters.stalePresentCount -eq 0 -and
    $report.counters.metricsReversalCount -eq 0 -and
    $report.counters.firstShowBeforeExactCount -eq 0

$sourceRevision = (& git -C $repoRoot rev-parse HEAD).Trim()
$dirtyFiles = @(& git -C $repoRoot status --short)
$manifest = [ordered]@{
    schema = 'doroti.winrt-content-island-w1r-manifest/v3'
    runId = $runId
    status = if ($contractPass) { 'PASS' } else { 'FAIL' }
    generatedAt = (Get-Date).ToUniversalTime().ToString('o')
    source = [ordered]@{
        revision = $sourceRevision
        dirty = $dirtyFiles.Count -gt 0
        dirtyFiles = $dirtyFiles
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
        applicationCreatedChildHwndCount = 0
        platformInternalChildWindowClass = 'InputSiteWindowClass'
        rootIslandInputSourceRegistrationCount = 0
        nativeIngressOwner = 'WinRtTopLevelNativeIngress'
        keyboardRuntimeFloor = 'bridge-processing-enabled-without-root-input-source-registration'
        keyboardPacketProducer = 'top-level-wndproc-only-despite-bridge-runtime-floor'
        translateMessageOwner = 'Native.DispatchTranslatedMessage'
        productFrameworkReferences = 0
    }
    automatedReport = $report
    evidenceQualification = [ordered]@{
        automatedContract = if ($contractPass) { 'PASS' } else { 'FAIL' }
        physicalFourEdge = 'notVerified'
        visibleComparison = 'notVerified'
        external240FpsVideo = 'notVerified'
        d1Decision = 'notStarted'
    }
}
$manifest | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Host "W1R contract=$($manifest.status) visible=$($report.visibleStatus) exitCode=$spikeExitCode"
Write-Host "pointerOwner=$($report.input.pointerOwner) keyboardOwner=$($report.input.keyboardOwner) rootInputSources=$($report.input.rootIslandInputSourceRegistrationCount)"
Write-Host "keyboardTrace=$($report.input.nativeTraceKeyboardMessageCount)/$($report.input.keyboardPacketCount) imeTrace=$($report.input.nativeTraceImeMessageCount)/$($report.input.imeMessageCount)"
Write-Host "report=$reportPath"
Write-Host "manifest=$manifestPath"
if (-not $contractPass) { exit 2 }
