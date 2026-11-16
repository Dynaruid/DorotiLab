<#
.SYNOPSIS
Publish a Doroti Web sample and serve it until the local preview server stops.
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Demo -DotnetVersion 11
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-web.ps1 -App Testbed -Mode Mono -Port 5192
#>
[CmdletBinding()]
param(
    [ValidateSet('Sample2', 'Testbed', 'Carousel', 'Demo')][string]$App,
    [ValidateSet(10, 11)][int]$DotnetVersion = 10,
    [ValidateSet('Mono', 'MonoAot')][string]$Mode,
    [ValidateSet('Debug', 'Release')][string]$Configuration,
    [ValidateSet('threaded', 'compatible', 'auto')][string]$WebExecutionMode,
    [ValidateRange(0, 65535)][int]$Port,
    [switch]$NoLaunch,
    [switch]$List,
    [switch]$DryRun,
    [string]$DotnetPath = 'dotnet',
    [switch]$Help
)
& (Join-Path $PSScriptRoot 'deploy.ps1') -Platform web @PSBoundParameters
exit $LASTEXITCODE
