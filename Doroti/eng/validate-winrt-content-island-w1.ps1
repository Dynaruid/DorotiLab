[CmdletBinding()]
param(
    [string]$OutputDirectory,
    [switch]$SkipBuild,
    [int]$HoldMilliseconds = 250
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

Write-Error 'W1-A is superseded by the D0 ownership correction. Run validate-winrt-content-island-w1r.ps1.' -ErrorAction Continue
exit 2
