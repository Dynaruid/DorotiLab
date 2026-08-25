[CmdletBinding()]
param(
    [string]$OutputDirectory,
    [switch]$SkipBuild,
    [int]$HoldMilliseconds = 250
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if (-not (Test-Path -LiteralPath (Join-Path $repoRoot 'Doroti\Doroti.slnx'))) {
    throw "Unable to locate the DorotiLab repository root from $PSScriptRoot."
}
$projectPath = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-spike\Doroti.Validation.WinRtContentIslandSpike.csproj'
$executablePath = Join-Path $repoRoot 'Doroti\validation\winrt-content-island-spike\bin\Release\net10.0-windows10.0.19041.0\win-x64\Doroti.Validation.WinRtContentIslandSpike.exe'
$runId = 'w1-{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([Guid]::NewGuid().ToString('N').Substring(0, 12))
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot ".doroti\evidence\$runId"
}
$OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$reportPath = Join-Path $OutputDirectory 'w1-report.json'

if (-not $SkipBuild) {
    & dotnet build $projectPath -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw "W1 spike build failed with exit code $LASTEXITCODE." }
}
if (-not (Test-Path -LiteralPath $executablePath)) {
    throw "W1 spike executable is missing: $executablePath"
}

& $executablePath --automated --hold-ms $HoldMilliseconds --report $reportPath
$spikeExitCode = $LASTEXITCODE
if (-not (Test-Path -LiteralPath $reportPath)) {
    throw "W1 spike did not write its report (exit code $spikeExitCode)."
}
$report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json -Depth 100
Write-Host "W1 contract=$($report.contractStatus) visible=$($report.visibleStatus) exitCode=$spikeExitCode"
Write-Host "pointerOwner=$($report.input.pointerOwner) keyboardOwner=$($report.input.keyboardOwner)"
Write-Host "report=$reportPath"
if ($report.contractStatus -ne 'PASS' -or $report.visibleStatus -ne 'PASS') { exit 2 }
