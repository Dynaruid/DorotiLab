#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $ProjectPath = (Join-Path $PSScriptRoot '../validation/windowsappsdk-flutter-input/Doroti.Validation.WindowsAppSdkFlutterInput.csproj'),
    [string] $ContractValidatorPath = (Join-Path $PSScriptRoot 'validate-windowsappsdk-flutter-input.ps1'),
    [string] $OutputRoot = (Join-Path $PSScriptRoot '../artifacts/windowsappsdk-flutter-input-manual'),
    [string] $AccessibilityInsightsPath = (Join-Path $env:LOCALAPPDATA 'DorotiTools/AccessibilityInsights-1.1.2924.01/PFiles/AccessibilityInsights/1.1/AccessibilityInsights.exe'),
    [switch] $LaunchAccessibilityInsights
)

$ErrorActionPreference = 'Stop'
$testTimeout = [TimeSpan]::FromMinutes(20)
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectFullPath = (Resolve-Path $ProjectPath).Path
$contractValidatorFullPath = (Resolve-Path $ContractValidatorPath).Path
$outputRootFullPath = [IO.Path]::GetFullPath($OutputRoot)

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

$sourceValidation = & $contractValidatorFullPath | Out-String | ConvertFrom-Json -Depth 64
Assert-True ([string]$sourceValidation.status -eq 'PASS') 'F7 static contract did not PASS before the manual run.'

[IO.Directory]::CreateDirectory($outputRootFullPath) | Out-Null
$runDirectory = Join-Path $outputRootFullPath (
    'f7-manual-' + [DateTimeOffset]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($runDirectory) | Out-Null
$publishDirectory = Join-Path $runDirectory 'publish'

& dotnet publish $projectFullPath -c Release -r win-x64 --self-contained true --nologo -o $publishDirectory
if ($LASTEXITCODE -ne 0) { throw 'F7 manual fixture publish failed.' }

$executablePath = Join-Path $publishDirectory 'Doroti.Validation.WindowsAppSdkFlutterInput.exe'
Assert-True (Test-Path -LiteralPath $executablePath -PathType Leaf) 'F7 manual fixture executable is missing.'
$evidencePath = Join-Path $runDirectory 'f7-manual-evidence.json'
$executableHash = (Get-FileHash -LiteralPath $executablePath -Algorithm SHA256).Hash.ToLowerInvariant()

$accessibilityProcess = $null
if ($LaunchAccessibilityInsights) {
    Assert-True (Test-Path -LiteralPath $AccessibilityInsightsPath -PathType Leaf) (
        "Accessibility Insights is missing: $AccessibilityInsightsPath")
    $accessibilityProcess = Start-Process -FilePath $AccessibilityInsightsPath -WindowStyle Normal -PassThru
}

Write-Host 'A visible Doroti F7 window is opening.'
Write-Host 'Directly observe each checklist item, toggle F1-F6, then press F8. Esc/close records notVerified.'
Write-Host 'Narrator shortcut: Win+Ctrl+Enter.'

$startInfo = [Diagnostics.ProcessStartInfo]::new()
$startInfo.FileName = $executablePath
$startInfo.WorkingDirectory = $publishDirectory
$startInfo.UseShellExecute = $false
$startInfo.CreateNoWindow = $false
$startInfo.Environment['PATH'] = 'C:\Windows\System32'
foreach ($argument in @(
        '--manual',
        '--evidence', $evidencePath,
        '--source-fingerprint', [string]$sourceValidation.sourceFingerprint,
        '--published-executable-sha256', $executableHash)) {
    [void]$startInfo.ArgumentList.Add($argument)
}
$process = [Diagnostics.Process]::Start($startInfo)
Assert-True ($null -ne $process) 'Could not start the F7 manual fixture.'
if (-not $process.WaitForExit([int]$testTimeout.TotalMilliseconds)) {
    $process.Kill($true)
    $process.WaitForExit()
    throw 'F7 manual fixture exceeded the 20-minute timeout.'
}

if ($null -ne $accessibilityProcess -and -not $accessibilityProcess.HasExited) {
    $accessibilityProcess.CloseMainWindow() | Out-Null
}

Assert-True (Test-Path -LiteralPath $evidencePath -PathType Leaf) 'F7 manual evidence was not written.'
$evidence = Get-Content -LiteralPath $evidencePath -Raw | ConvertFrom-Json -Depth 64
Assert-True ([string]$evidence.schemaVersion -eq 'doroti.windowsappsdk-flutter-input-manual-evidence/v1') (
    'Unexpected F7 manual evidence schema.')
Assert-True ([string]$evidence.sourceFingerprint -eq [string]$sourceValidation.sourceFingerprint) (
    'F7 manual evidence source fingerprint drifted.')
Assert-True ([bool]$evidence.selfAttestation.allRequiredChecks) 'F7 manual checklist is incomplete.'
Assert-True ([bool]$evidence.selfAttestation.completedExplicitlyWithF8) 'F7 manual run was not explicitly completed with F8.'
Assert-True ($process.ExitCode -eq 0 -and [string]$evidence.status -eq 'PASS') (
    "F7 manual acceptance did not PASS (exit $($process.ExitCode), status $($evidence.status)).")

[ordered]@{
    schemaVersion = 'doroti.windowsappsdk-flutter-input-manual-validation/v1'
    status = 'PASS'
    sourceFingerprint = [string]$sourceValidation.sourceFingerprint
    evidencePath = $evidencePath
    evidence = $evidence
    boundaries = @(
        'self-attested physical F7 input, IME, focus, and accessibility only',
        'no auxiliary DesktopChildSiteBridge was created',
        'FG product/compositor acceptance remains notVerified'
    )
} | ConvertTo-Json -Depth 64
