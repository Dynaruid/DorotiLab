<#
.SYNOPSIS
Compatibility entry point for DorotiSampleApp2 Android deployment.
.EXAMPLE
pwsh -NoProfile -File ./samples/DorotiSampleApp2/android/deploy-android.ps1 -Mode CoreClrJit -Serial YOUR_DEVICE_SERIAL
#>
[CmdletBinding()]
param(
    [ValidateSet(10, 11)][int]$DotnetVersion = 10,
    [ValidateSet('Mono', 'MonoAot', 'CoreClrJit', 'CoreClrR2R')][string]$Mode,
    [Alias('Device')][string]$Serial,
    [ValidateSet('auto', 'device', 'emulator')][string]$Target = 'auto',
    [ValidateSet('Debug', 'Release')][string]$Configuration,
    [string[]]$Extra,
    [switch]$NoLaunch,
    [switch]$List,
    [switch]$DryRun,
    [string]$DotnetPath = 'dotnet',
    [string]$AdbPath,
    [switch]$Help
)
$helper = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../helpers/deploy-helper/deploy-android.ps1'))
& $helper -App Sample2 @PSBoundParameters
exit $LASTEXITCODE
