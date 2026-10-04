#Requires -Version 7.0
[CmdletBinding()]
param(
    [ValidateSet('Source', 'Build', 'Targets', 'WindowsSmoke', 'LinuxSmoke', 'MacOSSmoke', 'IOSSmoke', 'CatalystSmoke', 'Packages', 'Developer', 'Release')]
    [string] $Suite = 'Developer'
)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
. (Join-Path $PSScriptRoot 'python-tools.ps1')
$pythonCommand = Resolve-DorotiPython
& $pythonCommand (Join-Path $PSScriptRoot 'run-with-timeout.py') $pythonCommand (Join-Path $PSScriptRoot 'validate.py') $Suite
if ($LASTEXITCODE -ne 0) { throw "Validation $Suite failed (exit $LASTEXITCODE)." }
