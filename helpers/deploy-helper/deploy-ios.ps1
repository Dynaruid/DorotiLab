<#
.SYNOPSIS
Select, build, install and launch a Doroti iOS sample on macOS.
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Testbed -Target simulator
.EXAMPLE
pwsh -NoProfile -File ./helpers/deploy-helper/deploy-ios.ps1 -App Sample2 -DotnetVersion 11 -SkipXcodeValidation
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

& (Join-Path $PSScriptRoot 'deploy.ps1') -Platform ios @PSBoundParameters
exit $LASTEXITCODE
