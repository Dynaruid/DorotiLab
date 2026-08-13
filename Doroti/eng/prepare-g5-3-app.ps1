#Requires -Version 5.1
param([string] $OutputPath)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$fullSelection = Join-Path $dorotiRoot 'migration/selections/g5-3-widgets.json'
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $dorotiRoot 'migration/selections/g5-3-dart-app.json'
}
elseif (-not [IO.Path]::IsPathRooted($OutputPath)) { $OutputPath = Join-Path $dorotiRoot $OutputPath }

& (Join-Path $PSScriptRoot 'prepare-g5-3.ps1') -OutputPath $fullSelection | Write-Output
$selection = Get-Content -LiteralPath $fullSelection -Raw | ConvertFrom-Json
foreach ($input in @($selection.inputs)) { $input.emissionMode = 'graph-only' }
$selection.outputNamespace = 'Doroti.Generated.Application'
$selection.outputAssemblyName = 'Doroti.Generated.Application.G53ExternalApp'
$selection.inputs += [pscustomobject][ordered]@{
    path = '../../validation/cases/g5-3-dart-app/main.dart'
    library = 'package:doroti_validation/g5-3-dart-app/main.dart'
    symbols = @('G53ExternalApp', 'main')
    emissionMode = 'generate'
}
$parent = Split-Path ([IO.Path]::GetFullPath($OutputPath)) -Parent
[IO.Directory]::CreateDirectory($parent) | Out-Null
$json = ($selection | ConvertTo-Json -Depth 20) -replace "`r`n", "`n"
[IO.File]::WriteAllText([IO.Path]::GetFullPath($OutputPath), $json + "`n", [Text.UTF8Encoding]::new($false))
Write-Output "G5-3 external Dart application selection: PASS"
