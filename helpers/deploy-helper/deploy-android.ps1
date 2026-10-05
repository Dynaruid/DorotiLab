<#
.SYNOPSIS
Build, install and launch a Doroti Android sample on a device or running emulator.
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-android.ps1 -App Sample2 -Mode CoreClrJit
#>
[CmdletBinding()]
param(
    [ValidateSet('Sample2', 'Testbed')][string]$App,
    [ValidateSet('auto', 'device', 'emulator')][string]$Target = 'auto',
    [Alias('Serial')][string]$Device,
    [ValidateSet(10, 11)][int]$DotnetVersion = 10,
    [ValidateSet('Mono', 'MonoAot', 'CoreClrJit', 'CoreClrR2R')][string]$Mode,
    [ValidateSet('Debug', 'Release')][string]$Configuration,
    [string[]]$Extra,
    [switch]$NoLaunch,
    [switch]$List,
    [switch]$DryRun,
    [string]$DotnetPath = 'dotnet',
    [string]$AdbPath,
    [switch]$Help
)
& (Join-Path $PSScriptRoot 'deploy.ps1') -Platform android @PSBoundParameters
exit $LASTEXITCODE
