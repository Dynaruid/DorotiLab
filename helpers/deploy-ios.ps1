<#
.SYNOPSIS
Select, build, install and launch a Doroti iOS sample on macOS.
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-ios.ps1 -App Sample2
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-ios.ps1 -App Testbed -Target simulator
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-ios.ps1 -App Sample2 -DotnetVersion 11 -SkipXcodeValidation
#>
[CmdletBinding()]
param(
    [ValidateSet('Sample2', 'Testbed')][string]$App,
    [ValidateSet('auto', 'device', 'simulator')][string]$Target = 'auto',
    [string]$Device,
    [ValidateSet(10, 11)][int]$DotnetVersion = 10,
    [ValidateSet('Mono', 'NativeAot')][string]$Mode,
    [ValidateSet('Debug', 'Release')][string]$Configuration,
    [string]$CodesignKey,
    [string]$CodesignProvision,
    [switch]$SkipXcodeValidation,
    [string[]]$Environment,
    [switch]$NoLaunch,
    [switch]$List,
    [switch]$DryRun,
    [string]$DotnetPath = 'dotnet',
    [switch]$Help
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$dotnet = (Get-Command $DotnetPath -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$project = Join-Path $PSScriptRoot 'Doroti.IosDeploy/Doroti.IosDeploy.csproj'
$arguments = @('run', '--project', $project, '-c', 'Release', '--no-launch-profile', '--')
if ($Help) {
    $arguments += '--help'
} else {
    $arguments += @('--target', $Target.ToLowerInvariant(), '--dotnet-version', "$DotnetVersion", '--dotnet-path', $dotnet)
    if ($App) { $arguments += @('--app', $(if ($App -ieq 'Sample2') { 'Sample2' } else { 'Testbed' })) }
    if ($Device) { $arguments += @('--device', $Device) }
    if ($Mode) { $arguments += @('--mode', $(if ($Mode -ieq 'Mono') { 'Mono' } else { 'NativeAot' })) }
    if ($Configuration) { $arguments += @('--configuration', $(if ($Configuration -ieq 'Debug') { 'Debug' } else { 'Release' })) }
    if ($CodesignKey) { $arguments += @('--codesign-key', $CodesignKey) }
    if ($CodesignProvision) { $arguments += @('--codesign-provision', $CodesignProvision) }
    foreach ($entry in $Environment) { $arguments += @('--env', $entry) }
    if ($SkipXcodeValidation) { $arguments += '--skip-xcode-validation' }
    if ($NoLaunch) { $arguments += '--no-launch' }
    if ($List) { $arguments += '--list' }
    if ($DryRun) { $arguments += '--dry-run' }
}
# Always compile the helper with the root .NET 10 SDK. The CLI chooses the
# independent .NET 10/11 SDK working directory for the selected iOS app.
Push-Location $repoRoot
try {
    & $dotnet @arguments
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
exit $result
