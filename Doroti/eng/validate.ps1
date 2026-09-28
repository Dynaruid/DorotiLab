#Requires -Version 7.0
[CmdletBinding()]
param(
    [ValidateSet('Source', 'Build', 'Targets', 'WindowsSmoke', 'Packages', 'Developer', 'Release')]
    [string] $Suite = 'Developer'
)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
& python (Join-Path $PSScriptRoot 'run-with-timeout.py') python (Join-Path $PSScriptRoot 'validate.py') $Suite
if ($LASTEXITCODE -ne 0) { throw "Validation $Suite failed (exit $LASTEXITCODE)." }
