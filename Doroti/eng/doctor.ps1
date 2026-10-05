[CmdletBinding(PositionalBinding=$false)]
param([Parameter(Position=0, ValueFromRemainingArguments=$true)] [string[]] $Arguments)
$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'doroti.ps1') doctor @Arguments
exit $LASTEXITCODE
