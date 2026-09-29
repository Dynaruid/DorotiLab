#Requires -Version 7.0
[CmdletBinding()]
param(
    [ValidateSet('Source', 'Build', 'Targets', 'WindowsSmoke', 'LinuxSmoke', 'Packages', 'Developer', 'Release')]
    [string] $Suite = 'Developer'
)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$pythonCommand = if (Get-Command python3 -ErrorAction SilentlyContinue) { 'python3' } else { 'python' }
& $pythonCommand (Join-Path $PSScriptRoot 'run-with-timeout.py') $pythonCommand (Join-Path $PSScriptRoot 'validate.py') $Suite
if ($LASTEXITCODE -ne 0) { throw "Validation $Suite failed (exit $LASTEXITCODE)." }
