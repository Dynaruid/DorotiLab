[CmdletBinding(PositionalBinding=$false)]
param([Parameter(Position=0, ValueFromRemainingArguments=$true)] [string[]] $Arguments)
$ErrorActionPreference = 'Stop'
$dotnetExecutable = 'dotnet'
$hostProject = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../tools/Doroti.Tooling/Doroti.Tooling.csproj'))
$hostAssembly = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../tools/Doroti.Tooling/bin/Release/net10.0/Doroti.Tooling.dll'))
if (-not (Test-Path -LiteralPath $hostProject)) { throw "Managed Doroti CLI is not installed: $hostProject" }
& $dotnetExecutable build $hostProject -c Release --nologo 2>&1 | ForEach-Object { [Console]::Error.WriteLine($_.ToString()) }
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& $dotnetExecutable $hostAssembly @Arguments
exit $LASTEXITCODE
