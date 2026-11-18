<#
.SYNOPSIS
Publish and run a Doroti Windows App SDK sample as .NET 10 Release Native AOT for win-x64.
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-windows.ps1 -App Demo
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-windows.ps1 -App Testbed -NoLaunch
#>
[CmdletBinding()]
param(
    [ValidateSet('Sample2', 'Testbed', 'Carousel', 'Demo')][string]$App,
    [string[]]$Environment,
    [switch]$NoLaunch,
    [switch]$List,
    [switch]$DryRun,
    [string]$DotnetPath = 'dotnet',
    [switch]$Help
)
& (Join-Path $PSScriptRoot 'deploy.ps1') -Platform windows @PSBoundParameters
exit $LASTEXITCODE
