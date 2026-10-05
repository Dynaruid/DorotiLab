<#
.SYNOPSIS
Select, build, install and launch a Doroti iOS or Android sample.
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1 -Platform android -App Testbed
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy.ps1 -Platform ios -App Sample2 -Target simulator
#>
[CmdletBinding()]
param(
    [ValidateSet('ios', 'android')][string]$Platform,
    [ValidateSet('Sample2', 'Testbed')][string]$App,
    [ValidateSet('auto', 'device', 'simulator', 'emulator')][string]$Target = 'auto',
    [Alias('Serial')][string]$Device,
    [ValidateSet(10, 11)][int]$DotnetVersion = 10,
    [ValidateSet('Mono', 'NativeAot', 'MonoAot', 'CoreClrJit', 'CoreClrR2R')][string]$Mode,
    [ValidateSet('Debug', 'Release')][string]$Configuration,
    [string]$CodesignKey,
    [string]$CodesignProvision,
    [switch]$SkipXcodeValidation,
    [string[]]$Environment,
    [string[]]$Extra,
    [switch]$NoLaunch,
    [switch]$List,
    [switch]$DryRun,
    [string]$DotnetPath = 'dotnet',
    [string]$AdbPath,
    [switch]$Help
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$dotnet = (Get-Command $DotnetPath -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$project = Join-Path $PSScriptRoot 'Doroti.DeployHelper/Doroti.DeployHelper.csproj'
$arguments = @('run', '--project', $project, '-c', 'Release', '--no-launch-profile', '--')
if ($Help) {
    $arguments += '--help'
} else {
    $arguments += @('--target', $Target.ToLowerInvariant(), '--dotnet-version', "$DotnetVersion", '--dotnet-path', $dotnet)
    if ($Platform) { $arguments += @('--platform', $Platform.ToLowerInvariant()) }
    if ($App) { $arguments += @('--app', $App) }
    if ($Device) { $arguments += @('--device', $Device) }
    if ($Mode) { $arguments += @('--mode', $Mode) }
    if ($Configuration) { $arguments += @('--configuration', $Configuration) }
    if ($CodesignKey) { $arguments += @('--codesign-key', $CodesignKey) }
    if ($CodesignProvision) { $arguments += @('--codesign-provision', $CodesignProvision) }
    if ($AdbPath) { $arguments += @('--adb-path', $AdbPath) }
    foreach ($entry in $Environment) { $arguments += @('--env', $entry) }
    foreach ($entry in $Extra) { $arguments += @('--extra', $entry) }
    if ($SkipXcodeValidation) { $arguments += '--skip-xcode-validation' }
    if ($NoLaunch) { $arguments += '--no-launch' }
    if ($List) { $arguments += '--list' }
    if ($DryRun) { $arguments += '--dry-run' }
}
# Compile the helper with the root .NET 10 SDK; app SDK selection is independent.
Push-Location $repoRoot
try {
    & $dotnet @arguments
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $result
